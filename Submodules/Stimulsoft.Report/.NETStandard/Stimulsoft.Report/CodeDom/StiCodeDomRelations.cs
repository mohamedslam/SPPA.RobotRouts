#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
{																	}
{	Copyright (C) 2003-2022 Stimulsoft     							}
{	ALL RIGHTS RESERVED												}
{																	}
{	The entire contents of this file is protected by U.S. and		}
{	International Copyright Laws. Unauthorized reproduction,		}
{	reverse-engineering, and distribution of all or any portion of	}
{	the code contained in this file is strictly prohibited and may	}
{	result in severe civil and criminal penalties and will be		}
{	prosecuted to the maximum extent possible under the law.		}
{																	}
{	RESTRICTIONS													}
{																	}
{	THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES			}
{	ARE CONFIDENTIAL AND PROPRIETARY								}
{	TRADE SECRETS OF Stimulsoft										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using System;
using System.Collections.Generic;
using System.Collections;
using System.CodeDom;
using System.Reflection;
using Stimulsoft.Report.Dictionary;

namespace Stimulsoft.Report.CodeDom
{
    internal class StiCodeDomRelations
    {
        internal static void Serialize(StiCodeDomSerializator serializator, StiReport report)
        {
            if (report.Dictionary.Relations.Count <= 0) return;

            var relationsDict = new Hashtable();
            bool isFirst = true;

            foreach (StiDataRelation relation in report.Dictionary.Relations)
            {
                if (relation.ParentSource == null || relation.ChildSource == null) continue;
                var relationName = StiCodeDomSerializator.GetParentRelationName(report, relation.Name);

                #region Check name
                var relName = relationName;
                var nameIndex = 1;
                while (relationsDict[relName] != null)
                {
                    relName = relationName + nameIndex++;
                }
                relationName = relName;
                relationsDict[relationName] = relationName;
                #endregion

                #region relation = new Relation
                #region Prepare parameters for constructor

                #region parentColumns
                var parentColumns = new CodeExpression[relation.ParentColumns.Length];
                int index = 0;
                foreach (string str in relation.ParentColumns)
                    parentColumns[index++] = new CodePrimitiveExpression(str);
                #endregion

                #region childColumns
                var childColumns = new CodeExpression[relation.ChildColumns.Length];
                index = 0;
                foreach (string str in relation.ChildColumns)
                    childColumns[index++] = new CodePrimitiveExpression(str);
                #endregion

                var args = new List<CodeExpression>
                {
                    new CodePrimitiveExpression(relation.NameInSource),
                    new CodePrimitiveExpression(relation.Name),
                    new CodePrimitiveExpression(relation.Alias),
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), StiNameValidator.CorrectName(relation.ParentSource.Name, report)),
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), StiNameValidator.CorrectName(relation.ChildSource.Name, report)),
                    new CodeArrayCreateExpression(typeof(string), parentColumns),
                    new CodeArrayCreateExpression(typeof(string), childColumns)
                };
                if (!string.IsNullOrWhiteSpace(relation.Key))
                    args.Add(new CodePrimitiveExpression(relation.Key));

                if (relation.Active)
                    args.Add(new CodePrimitiveExpression(relation.Active));
                #endregion

                serializator.memberMethod.Statements.Insert(0,
                    new CodeAssignStatement(
                        new CodeFieldReferenceExpression(
                            new CodeThisReferenceExpression(), relationName),
                        new CodeObjectCreateExpression(
                            new CodeTypeReference(relation.GetType()),
                            args.ToArray())));
                #endregion

                #region this.Dictionary.Relations.Add(relation)
                serializator.memberMethod.Statements.Add(new CodeMethodInvokeExpression(
                    new CodePropertyReferenceExpression(
                        new CodePropertyReferenceExpression(
                            new CodeThisReferenceExpression(), "Dictionary"), "Relations"),
                    "Add",
                    new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), relationName)));
                #endregion

                var relationDeclare = new CodeMemberField(relation.GetType(), relationName) { Attributes = MemberAttributes.Public };
                if (Array.IndexOf(StiCodeDomSerializator.ReservedWords, relationName) != -1)
                    relationDeclare.Attributes |= MemberAttributes.New;

                serializator.Members.Insert(0, relationDeclare);

                #region Generate typed relation
                var ctd = new CodeTypeDeclaration(relationName + "Relation");

                #region Create constructor of relation
                var constr2 = new CodeConstructor { Attributes = MemberAttributes.Public };
                constr2.Parameters.Add(new CodeParameterDeclarationExpression(typeof(StiDataRow), "dataRow"));
                constr2.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("dataRow"));

                ctd.Members.Add(constr2);
                #endregion

                #region Comment
                if (isFirst)
                {
                    ctd.Comments.Add(new CodeCommentStatement(StiCodeDomSerializator.GetCheckerInfoString("*Relations*", "*None*")));
                    isFirst = false;
                }
                ctd.Comments.Add(new StiCodeRegionStart($"Relation {relationName}"));
                ctd.Comments.Add(new StiCodeRegionEnd($"Relation {relationName}"));
                #endregion

                ctd.IsClass = true;
                ctd.BaseTypes.Add(typeof(StiDataRow));
                ctd.TypeAttributes = TypeAttributes.Public;

                #region Collection of columns
                var argColumns = new List<CodeExpression>();
                #endregion

                #region Generate members (columns) relation
                foreach (StiDataColumn column in relation.ParentSource.Columns)
                {
                    argColumns.Add(serializator.GetObjectCreateExpression(column.GetType(), column));

                    var property = serializator.GetColumnProperty(column, true);
                    if (column.Name == "Dictionary" ||
                        column.Name == "Row")
                        property.Attributes |= MemberAttributes.New;

                    ctd.Members.Add(property);
                }
                #endregion

                #region Generate parent relations
                var relations = relation.ParentSource.GetParentRelations();
                foreach (StiDataRelation parentRelation in relations)
                {
                    var parentRelationName = StiCodeDomSerializator.GetParentRelationName(report, parentRelation.Name);
                    var parentProp = new CodeMemberProperty
                    {
                        Attributes = MemberAttributes.Public,
                        HasGet = true,
                        Name = StiNameValidator.CorrectName(parentRelation.Name, report),
                        Type = new CodeTypeReference(parentRelationName + "Relation")
                    };

                    var create =
                        new CodeObjectCreateExpression(
                            new CodeTypeReference($"{parentRelationName}Relation"), new CodeMethodInvokeExpression(
                                new CodeThisReferenceExpression(),
                                "GetParentData", new CodePrimitiveExpression(
                                    parentRelation.NameInSource)));

                    parentProp.GetStatements.Add(new CodeMethodReturnStatement(create));
                    ctd.Members.Add(parentProp);
                }
                #endregion

                serializator.Members.Add(ctd);
                #endregion
            }
        }
    }
}
