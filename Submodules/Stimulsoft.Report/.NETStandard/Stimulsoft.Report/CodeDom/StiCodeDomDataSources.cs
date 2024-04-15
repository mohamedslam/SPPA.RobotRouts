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
using System.CodeDom;
using System.Reflection;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Events;
using Stimulsoft.Data.Engine;
using System.Linq;

namespace Stimulsoft.Report.CodeDom
{
    internal class StiCodeDomDataSources
    {
        internal static void Serialize(StiCodeDomSerializator serializator, StiReport report)
        {
            if (report.Dictionary.DataSources.Count > 0)
            {
                bool isFirst = true;
                foreach (StiDataSource data in report.Dictionary.DataSources)
                {
                    data.Dictionary = report.Dictionary;

                    string dataName = StiNameValidator.CorrectName(data.Name, report);

                    #region data = new DataType
                    CodeTypeReference dataSourceType = new CodeTypeReference(dataName + "DataSource");

                    serializator.memberMethod.Statements.Insert(0,
                        new CodeAssignStatement(
                        new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), dataName),
                        new CodeObjectCreateExpression(
                        dataSourceType)));
                    #endregion

                    #region Generate typed Data Source
                    var ctd = new CodeTypeDeclaration(dataName + "DataSource");

                    #region Create constructor typed DataSource
                    var constr = new CodeConstructor
                    {
                        Attributes = MemberAttributes.Public
                    };
                    constr.BaseConstructorArgs.AddRange(
                        new CodeExpressionCollection(
                        serializator.GetArguments(data.GetType(), data)));
                    ctd.Members.Add(constr);
                    #endregion

                    #region Comments
                    if (isFirst)
                    {
                        ctd.Comments.Add(new CodeCommentStatement(StiCodeDomSerializator.GetCheckerInfoString("*DataSources*", "*None*")));
                        isFirst = false;
                    }
                    ctd.Comments.Add(new StiCodeRegionStart("DataSource " + dataName));
                    ctd.Comments.Add(new StiCodeRegionEnd("DataSource " + dataName));
                    #endregion

                    ctd.IsClass = true;
                    ctd.BaseTypes.Add(data.GetType());
                    ctd.TypeAttributes = TypeAttributes.Public;

                    #region Collection of columns
                    var argColumns = new List<CodeExpression>();
                    #endregion

                    #region Generate columns of the DataSource
                    foreach (StiDataColumn column in data.Columns)
                    {
                        argColumns.Add(serializator.GetObjectCreateExpression(column.GetType(), column));
                        var property = serializator.GetColumnProperty(column, false);
                        property.Attributes = MemberAttributes.Public;
                        if (column.Name == "Name" ||
                            column.Name == "Position" ||
                            column.Name == "Inherited" ||
                            column.Name == "RealCount" ||
                            column.Name == "IsBof" ||
                            column.Name == "IsEof" ||
                            column.Name == "IsEmpty" ||
                            column.Name == "Alias")
                            property.Attributes |= MemberAttributes.New;
                        ctd.Members.Add(property);
                    }
                    #endregion

                    var sqlSource = data as StiSqlSource;
                    var argParameters = new List<CodeExpression>();
                    if (sqlSource != null && sqlSource.Parameters.Count > 0)
                    {
                        #region Generate parameters of the DataSource
                        foreach (StiDataParameter parameter in sqlSource.Parameters)
                        {
                            argParameters.Add(serializator.GetObjectCreateExpression(parameter.GetType(), parameter));
                        }
                        #endregion
                    }

                    #endregion

                    #region Add columns to DataSource
                    if (argColumns.Count > 0)
                    {
                        int maxCountPerLine = 100;  //fix
                        int currentPos = 0;
                        while (currentPos < argColumns.Count)
                        {
                            int remainCount = argColumns.Count - currentPos;
                            if (remainCount > maxCountPerLine) remainCount = maxCountPerLine;
                            var args = argColumns.GetRange(currentPos, remainCount).ToArray();
                            currentPos += remainCount;

                            var args2 = new CodeExpression[]
                            {
                                new CodeArrayCreateExpression(typeof(StiDataColumn), args)
                            };

                            serializator.memberMethod.Statements.Add(
                                new CodeMethodInvokeExpression(
                                new CodePropertyReferenceExpression(
                                new CodeFieldReferenceExpression(
                                new CodeThisReferenceExpression(), dataName), "Columns"),
                                "AddRange",
                                args2
                                ));
                        }
                    }
                    #endregion

                    #region Add parameters to DataSource
                    if (argParameters.Count > 0)
                    {
                        var args = argParameters.ToArray();

                        var args2 = new CodeExpression[]
                        {
                            new CodeArrayCreateExpression(typeof (StiDataParameter), args)
                        };

                        serializator.memberMethod.Statements.Add(
                            new CodeMethodInvokeExpression(
                            new CodePropertyReferenceExpression(
                            new CodeFieldReferenceExpression(
                            new CodeThisReferenceExpression(), dataName), "Parameters"),
                            "AddRange",
                            args2
                            ));
                    }
                    #endregion

                    #region VirtualSource
                    StiVirtualSource virtualSource = data as StiVirtualSource;
                    if (virtualSource != null)
                    {
                        string correctedDataName = StiNameValidator.CorrectName(data.Name, report);

                        #region Generate Sort Methods
                        string[] strs = virtualSource.Sort.Clone() as string[];
                        for (int index = 0; index < strs.Length; index++)
                        {
                            string str = strs[index].Clone() as string;
                            if (str.StartsWith("{", StringComparison.InvariantCulture) && str.EndsWith("}", StringComparison.InvariantCulture))
                            {
                                string methodName = "Get" + correctedDataName + "_Sort" + index;
                                str = str.Substring(1, str.Length - 2);

                                serializator.GenReturnMethodForExpresion(methodName, str, typeof(object));
                                strs[index] = "{" + methodName + "}";
                            }
                        }
                        #endregion

                        serializator.Assign(
                            serializator.memberMethod,
                            correctedDataName,
                            "Sort",
                            strs);


                        serializator.Assign(
                            serializator.memberMethod,
                            correctedDataName,
                            "GroupColumns",
                            virtualSource.GroupColumns);

                        serializator.Assign(
                            serializator.memberMethod,
                            correctedDataName,
                            "Results",
                            virtualSource.Results);

                        StiCodeDomFilters.AddFilters(serializator, correctedDataName, virtualSource);

                    }
                    #endregion

                    #region DataTransformation
                    var transformation = data as StiDataTransformation;
                    if (transformation != null)
                    {
                        var correctedDataName = StiNameValidator.CorrectName(data.Name, report);

                        var initSorts = transformation.Sorts.Select(s => serializator.GetObjectCreateExpression(typeof(StiDataSortRule), s)).ToArray();
                        if (initSorts.Any())
                        {
                            serializator.memberMethod.Statements.Add(
                                new CodeAssignStatement(
                                    new CodeFieldReferenceExpression(
                                        new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), correctedDataName), "Sorts"),
                                    new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(StiDataSortRuleHelper)), "ToList", initSorts)));
                        }

                        var initFilters = transformation.Filters.Select(s => serializator.GetObjectCreateExpression(typeof(StiDataFilterRule), s)).ToArray();
                        if (initFilters.Any())
                        {
                            serializator.memberMethod.Statements.Add(
                                new CodeAssignStatement(
                                    new CodeFieldReferenceExpression(
                                        new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), correctedDataName), "Filters"),
                                    new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(StiDataFilterRuleHelper)), "ToList", initFilters)));
                        }

                        var initActions = transformation.Actions.Select(s => serializator.GetObjectCreateExpression(typeof(StiDataActionRule), s)).ToArray();
                        if (initActions.Any())
                        {
                            serializator.memberMethod.Statements.Add(
                                new CodeAssignStatement(
                                    new CodeFieldReferenceExpression(
                                        new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), correctedDataName), "Actions"),
                                    new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(StiDataActionRuleHelper)), "ToList", initActions)));
                        }
                    }
                    #endregion

                    #region this.Dictionary.DataSources.Add(data)
                    serializator.memberMethod.Statements.Add(new CodeMethodInvokeExpression(
                        new CodePropertyReferenceExpression(
                        new CodeThisReferenceExpression(), "DataSources"),
                        "Add",
                        new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), dataName)));
                    #endregion

                    #region Generate parent relations
                    StiDataRelationsCollection relations = data.GetParentRelations();
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

                        if (parentRelation.Name == "Name" ||
                            parentRelation.Name == "Alias")
                            parentProp.Attributes |= MemberAttributes.New;

                        CodeExpression create =
                            new CodeObjectCreateExpression(
                            new CodeTypeReference(parentRelationName + "Relation"), new CodeMethodInvokeExpression(
                                new CodeThisReferenceExpression(),
                                "GetParentData", new CodePrimitiveExpression(parentRelation.NameInSource)));

                        parentProp.GetStatements.Add(new CodeMethodReturnStatement(create));
                        ctd.Members.Add(parentProp);
                    }
                    #endregion

                    #region Generate code for support parameters
                    if (sqlSource != null)
                    {
                        var ev = new StiConnectingEvent(sqlSource.SqlCommand);
                        var eventMethodName = $"Get{dataName}_SqlCommand";
                        serializator.GenAddEvent(eventMethodName, dataName, ev);

                        var stats = new CodeStatementCollection();
                        var sqlSourceSqlCommand = sqlSource.SqlCommand;

                        var filter = sqlSource.GetSqlFilterQuery();
                        if (filter != null)
                        {
                            if (sqlSourceSqlCommand != null && sqlSourceSqlCommand.ToLowerInvariant().Contains("where"))
                                sqlSourceSqlCommand += " AND " + filter;
                            else
                                sqlSourceSqlCommand += " WHERE " + filter;
                        }

                        CodeExpression sqlQuery;
                        if (sqlSource.AllowExpressions && !(sqlSource is StiMongoDbSource))
                            sqlQuery = serializator.GetTextScriptExpression(null, sqlSourceSqlCommand, true, false, dataName, null, false);
                        else
                            sqlQuery = new CodePrimitiveExpression(sqlSourceSqlCommand);


                        stats.Add(new CodeAssignStatement(
                            new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                            new CodeThisReferenceExpression(), dataName), "SqlCommand"), sqlQuery));

                        #region Generate parameters of the DataSource
                        foreach (StiDataParameter parameter in sqlSource.Parameters)
                        {
                            if (string.IsNullOrEmpty(parameter.Expression)) continue;

                            var script = StiCodeDomFunctions.ParseFunctions(serializator, parameter.Value);

                            stats.Add(new CodeAssignStatement(
                                new CodePropertyReferenceExpression(
                                new CodeIndexerExpression(
                                new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(
                                new CodeThisReferenceExpression(), dataName), "Parameters"),
                                new CodePrimitiveExpression(parameter.Name)), "ParameterValue"),
                                new CodeSnippetExpression(script)));
                        }
                        #endregion

                        serializator.GenEventMethod(eventMethodName, ev, stats);
                    }
                    #endregion

                    if (sqlSource != null && sqlSource.Type == StiSqlSourceType.StoredProcedure)
                    {
                        var assingType = new CodeAssignStatement(
                            new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                            new CodeThisReferenceExpression(), dataName), "Type"),
                            new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(StiSqlSourceType)), "StoredProcedure"));

                        serializator.memberMethod.Statements.Add(assingType);
                    }

                    var dataSourceField = new CodeMemberField(new CodeTypeReference(ctd.Name), dataName)
                    {
                        Attributes = MemberAttributes.Public
                    };

                    if (Array.IndexOf(StiCodeDomSerializator.ReservedWords, dataName) != -1)
                        dataSourceField.Attributes |= MemberAttributes.New;

                    serializator.Members.Add(dataSourceField);
                    serializator.Members.Add(ctd);
                }

                var listDataSources = Engine.StiVariableHelper.GetDataSourcesWithRequestFromUserVariablesInCommand(report);
                if (listDataSources.Count > 0)
                {
                    var inits = new CodeExpression[listDataSources.Count];

                    for (int index = 0; index < listDataSources.Count; index++)
                        inits[index] = new CodePrimitiveExpression(listDataSources[index]);

                    var right = new CodeArrayCreateExpression(typeof(string), inits);

                    var left = new CodeFieldReferenceExpression(
                        new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), "Dictionary"), "ReconnectListForRequestFromUserVariables");

                    serializator.Statements.Add(new CodeAssignStatement(left, right));
                }

            }
        }
    }
}
