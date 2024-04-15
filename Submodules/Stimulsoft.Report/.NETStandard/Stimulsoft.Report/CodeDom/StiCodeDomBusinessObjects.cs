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

namespace Stimulsoft.Report.CodeDom
{
	internal class StiCodeDomBusinessObjects
	{
	    internal static void Serialize(StiCodeDomSerializator serializator, StiReport report)
		{
            foreach (StiBusinessObject data in report.Dictionary.BusinessObjects)
            {
                data.Dictionary = report.Dictionary;

                Serialize(serializator, report, data, true, null, null);
            }
		}

        private static void Serialize(StiCodeDomSerializator serializator, StiReport report, StiBusinessObject data, bool isParent, 
            CodeTypeDeclaration parentType, CodeConstructor parentConstructor)
        {
            string dataName = StiNameValidator.CorrectName(data.Name, report);
            string stBusinessObject = "BusinessObject";

            if ((parentType != null) && (dataName + stBusinessObject == parentType.Name))
            {
                stBusinessObject += "_";
            }

            #region data = new DataType
            CodeTypeReference dataSourceType = new CodeTypeReference(dataName + stBusinessObject);

            if (isParent)
            {
                serializator.memberMethod.Statements.Insert(0,
                    new CodeAssignStatement(
                    new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), dataName),
                    new CodeObjectCreateExpression(
                    dataSourceType)));
            }
            else
            {
                parentConstructor.Statements.Add(
                    new CodeAssignStatement(
                    new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), dataName),
                    new CodeObjectCreateExpression(
                    dataSourceType)));
            }
            #endregion

            #region Generate typed Business Object
            var ctd = new CodeTypeDeclaration(dataName + stBusinessObject);

            #region Create constructor typed BusinessObject
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
            ctd.Comments.Add(new StiCodeRegionStart("BusinessObject " + dataName));
            ctd.Comments.Add(new StiCodeRegionEnd("BusinessObject " + dataName));
            #endregion

            ctd.IsClass = true;
            ctd.BaseTypes.Add(data.GetType());
            ctd.TypeAttributes = TypeAttributes.Public;
            
            #region Collection of columns
            var argColumns = new List<CodeExpression>();
            #endregion

            #region Generate columns of the BusinessObject
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
            #endregion

            #region Add columns to BusinessObject
            if (argColumns.Count > 0)
            {
                var args = argColumns.ToArray();

                var args2 = new CodeExpression[]
                {
                    new CodeArrayCreateExpression(
                        typeof(StiDataColumn),
                        args)
                };

                if (isParent)
                {
                    serializator.memberMethod.Statements.Add(
                        new CodeMethodInvokeExpression(
                        new CodePropertyReferenceExpression(
                        new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), dataName), "Columns"),
                        "AddRange",
                        args2
                        ));
                }
                else
                {
                    parentConstructor.Statements.Add(
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

            #region BusinessObjects.Add(data)
            if (isParent)
            {
                serializator.memberMethod.Statements.Add(new CodeMethodInvokeExpression(
                    new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(
                    new CodeThisReferenceExpression(), "Dictionary"), "BusinessObjects"),
                    "Add",
                    new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), dataName)));
            }
            else 
            {
                parentConstructor.Statements.Add(new CodeMethodInvokeExpression(
                   new CodePropertyReferenceExpression(
                   new CodeThisReferenceExpression(), "BusinessObjects"),
                   "Add",
                   new CodeFieldReferenceExpression(
                   new CodeThisReferenceExpression(), dataName)));
            }
            #endregion

            #region Generate child Business Objects
            foreach (StiBusinessObject businessObject in data.BusinessObjects)
            {
                Serialize(serializator, report, businessObject, false, ctd, constr);
            }
            #endregion

            var businessObjectField = new CodeMemberField(new CodeTypeReference(ctd.Name), dataName)
            {
                Attributes = MemberAttributes.Public
            };

            if (Array.IndexOf(StiCodeDomSerializator.ReservedWords, dataName) != -1)
                businessObjectField.Attributes |= MemberAttributes.New;

            if (isParent)
            {
                serializator.Members.Add(businessObjectField);
                serializator.Members.Add(ctd);
            }
            else
            {
                parentType.Members.Add(businessObjectField);
                parentType.Members.Add(ctd);
            }
        }
	}
}
