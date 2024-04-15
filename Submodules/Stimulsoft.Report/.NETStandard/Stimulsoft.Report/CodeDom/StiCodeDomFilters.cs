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
using System.Globalization;
using System.CodeDom;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using System.Threading;

namespace Stimulsoft.Report.CodeDom
{
    internal class StiCodeDomFilters
    {
        internal static object ConvertValueToObject(StiCodeDomSerializator serializator, StiFilterDataType type, string value)
        {
            string sep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            switch (type)
            {
                case StiFilterDataType.Numeric:
                    return decimal.Parse(value.Replace(".", ",").Replace(",", sep));

                case StiFilterDataType.DateTime:
                    var currentCulture = Thread.CurrentThread.CurrentCulture;
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                    DateTime dtResult;
                    if (!DateTime.TryParse(value, out dtResult))
                    {
                        dtResult = DateTime.Now;
                    }

                    Thread.CurrentThread.CurrentCulture = currentCulture;

                    return dtResult;

                case StiFilterDataType.Boolean:
                    return value.ToLower(CultureInfo.InvariantCulture) == "true";
            }
            return value.ToLower(CultureInfo.InvariantCulture);
        }


        internal static CodeExpression GetCodeExpressionFromFilterValue(
            StiCodeDomSerializator serializator, StiFilterDataType dataType, string textValue)
        {
            if (dataType != StiFilterDataType.Expression)
            {
                object value = ConvertValueToObject(serializator, dataType, textValue);

                if (value is DateTime)
                    return serializator.GetObjectCreateExpression(typeof(DateTime), value);

                if (value is string)
                {
                    return
                        new CodeMethodInvokeExpression(
                            new CodeMethodInvokeExpression(
                                new CodeThisReferenceExpression(), "ToString", new CodePrimitiveExpression(value)),
                            "ToLower");
                }

                return new CodePrimitiveExpression(value);
            }

            return new CodeSnippetExpression(textValue);
        }


        /// <summary>
        /// Returns CodeExpression for filter.
        /// </summary>
        internal static CodeExpression GetFilterExpression(StiCodeDomSerializator serializator,
            IStiFilter filterComp, StiFilter filter, bool isConditions, bool isSingle = false)
        {
            var dataSourceComp = filterComp as IStiDataSource;

            try
            {
                #region Expression
                if (filter.Item == StiFilterItem.Expression)
                {
                    string expression = filter.Expression.Value.Trim();
                    if (expression.StartsWith("{", StringComparison.InvariantCulture)) expression = expression.Substring(1);
                    if (expression.EndsWith("}", StringComparison.InvariantCulture)) expression = expression.Substring(0, expression.Length - 1);
                    if (string.IsNullOrWhiteSpace(expression)) return null;
                    expression = StiCodeDomFunctions.ParseFunctions(serializator, expression);
                    expression = StiCodeDomTotalsFunctionsParser.ProcessTotals(serializator, expression, null);
                    if (expression.StartsWith("(") && expression.EndsWith(")") || isSingle) return new CodeSnippetExpression(expression);
                    return new CodeSnippetExpression("(" + expression + ")");
                }
                #endregion

                #region Condition
                else
                {
                    string dataSourceName = string.Empty;

                    if (!isConditions && dataSourceComp != null && (!string.IsNullOrEmpty(dataSourceComp.DataSourceName)))
                        dataSourceName = StiNameValidator.CorrectName(dataSourceComp.DataSourceName, serializator.report);

                    if (filterComp is StiVirtualSource)
                        dataSourceName = StiNameValidator.CorrectName(((StiVirtualSource)filterComp).NameInSource, serializator.report);

                    if (filter.Column.Length > 0)
                    {
                        string columnName = filter.Column;

                        if (dataSourceName.Length > 0)
                        {
                            columnName = $"{dataSourceName}.{filter.Column}";
                        }

                        #region Null Operations
                        if (filter.Condition == StiFilterCondition.IsNull ||
                            filter.Condition == StiFilterCondition.IsNotNull)
                        {
                            CodeExpression expDS = null;

                            int posDot = columnName.LastIndexOf('.');
                            if (posDot < 0)
                            {
                                expDS = new CodeSnippetExpression(columnName);
                            }
                            else
                            {
                                string dsName = columnName.Substring(0, posDot);
                                string colName = columnName.Substring(posDot + 1);

                                expDS = new CodeIndexerExpression(
                                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), dsName),
                                    new CodePrimitiveExpression(colName));
                            }

                            var expr1 = new CodeBinaryOperatorExpression(
                                expDS,
                                CodeBinaryOperatorType.IdentityEquality,
                                new CodePrimitiveExpression(null));

                            var expr2 = new CodeBinaryOperatorExpression(
                                expDS,
                                CodeBinaryOperatorType.IdentityEquality,
                                new CodePropertyReferenceExpression(new CodeTypeReferenceExpression("DBNull"), "Value"));

                            CodeExpression expr3 = new CodeBinaryOperatorExpression(expr1, CodeBinaryOperatorType.BooleanOr, expr2);

                            return filter.Condition == StiFilterCondition.IsNull
                                ? expr3
                                : new CodeBinaryOperatorExpression(expr3, CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(false));
                        }
                        #endregion

                        var codeExpr1 = GetCodeExpressionFromFilterValue(serializator, filter.DataType, filter.Value1);

                        #region String Operations
                        if (filter.Condition == StiFilterCondition.BeginningWith ||
                            filter.Condition == StiFilterCondition.EndingWith ||
                            filter.Condition == StiFilterCondition.Containing ||
                            filter.Condition == StiFilterCondition.NotContaining)
                        {
                            if (filter.DataType == StiFilterDataType.Expression)
                            {
                                codeExpr1 = new CodeMethodInvokeExpression(
                                    new CodeMethodInvokeExpression(
                                        new CodeThisReferenceExpression(), "ToString",
                                        new CodeSnippetExpression(filter.Value1)),
                                    "ToLower");
                            }

                            #region Containing
                            if (filter.Condition == StiFilterCondition.Containing)
                            {
                                var expr1 = new CodeMethodInvokeExpression(
                                    new CodeThisReferenceExpression(), "ToString",
                                    new CodeFieldReferenceExpression(
                                        new CodeThisReferenceExpression(), columnName));

                                return
                                    new CodeBinaryOperatorExpression(
                                        new CodeMethodInvokeExpression(
                                            new CodeMethodInvokeExpression(expr1, "ToLower"),
                                            "IndexOf", codeExpr1),
                                        CodeBinaryOperatorType.IdentityInequality,
                                        new CodePrimitiveExpression(-1));
                            }
                            #endregion

                            #region NotContaining

                            if (filter.Condition == StiFilterCondition.NotContaining)
                            {
                                CodeExpression expr2 = new CodeMethodInvokeExpression(
                                    new CodeThisReferenceExpression(), "ToString", new CodeFieldReferenceExpression(
                                        new CodeThisReferenceExpression(), columnName));

                                return
                                    new CodeBinaryOperatorExpression(
                                        new CodeMethodInvokeExpression(
                                            new CodeMethodInvokeExpression(expr2, "ToLower"),
                                            "IndexOf", codeExpr1),
                                        CodeBinaryOperatorType.ValueEquality,
                                        new CodePrimitiveExpression(-1));
                            }
                            #endregion

                            #region BeginningWith
                            else if (filter.Condition == StiFilterCondition.BeginningWith)
                            {
                                CodeExpression expr3 = new CodeMethodInvokeExpression(
                                    new CodeThisReferenceExpression(), "ToString", new CodeFieldReferenceExpression(
                                        new CodeThisReferenceExpression(), columnName));

                                return
                                    new CodeMethodInvokeExpression(
                                        new CodeMethodInvokeExpression(expr3, "ToLower"),
                                        "StartsWith", codeExpr1);
                            }
                            #endregion

                            #region EndingWith
                            else if (filter.Condition == StiFilterCondition.EndingWith)
                            {
                                CodeExpression expr4 = new CodeMethodInvokeExpression(
                                    new CodeThisReferenceExpression(), "ToString", new CodeFieldReferenceExpression(
                                        new CodeThisReferenceExpression(), columnName));

                                return
                                    new CodeMethodInvokeExpression(
                                        new CodeMethodInvokeExpression(expr4, "ToLower"),
                                        "EndsWith", codeExpr1);
                            }
                            #endregion
                        }
                        #endregion

                        #region Other Operations
                        else
                        {
                            CodeExpression columnExpr =
                                new CodeFieldReferenceExpression(
                                    new CodeThisReferenceExpression(), columnName);

                            CodeExpression columnCastedExpr = columnExpr;

                            #region Prepare Column
                            if (filter.DataType == StiFilterDataType.String)
                            {
                                columnCastedExpr = new CodeMethodInvokeExpression(
                                    new CodeMethodInvokeExpression(
                                        columnCastedExpr, "ToString"),
                                    "ToLower");
                            }
                            else if (filter.DataType != StiFilterDataType.Expression)
                            {
                                object value = ConvertValueToObject(serializator, filter.DataType, filter.Value1);

                                columnCastedExpr =
                                    new CodeCastExpression(value.GetType(),
                                        new CodeMethodInvokeExpression(
                                            new CodeTypeReferenceExpression("StiReport"), "ChangeType", columnExpr, new CodeTypeOfExpression(value.GetType()), new CodePrimitiveExpression(true)));
                            }
                            #endregion

                            #region StiFilterCondition.Between & StiFilterCondition.NotBetween
                            if (filter.Condition == StiFilterCondition.Between ||
                                filter.Condition == StiFilterCondition.NotBetween)
                            {
                                CodeExpression codeExpr2 = GetCodeExpressionFromFilterValue(serializator, filter.DataType, filter.Value2);

                                CodeExpression expr1 =
                                    new CodeBinaryOperatorExpression(
                                        columnCastedExpr, CodeBinaryOperatorType.GreaterThanOrEqual,
                                        codeExpr1);

                                CodeExpression expr2 =
                                    new CodeBinaryOperatorExpression(
                                        columnCastedExpr, CodeBinaryOperatorType.LessThanOrEqual,
                                        codeExpr2);

                                CodeExpression expr3 =
                                    new CodeBinaryOperatorExpression(
                                        expr1, CodeBinaryOperatorType.BooleanAnd,
                                        expr2);

                                if (filter.Condition == StiFilterCondition.Between) return expr3;
                                else
                                {
                                    return new CodeBinaryOperatorExpression(
                                        expr3, CodeBinaryOperatorType.IdentityEquality,
                                        new CodePrimitiveExpression(false));
                                }
                            }
                            #endregion

                            #region Other operations
                            else
                            {
                                #region Operation
                                CodeBinaryOperatorType oper = CodeBinaryOperatorType.BooleanAnd;
                                switch (filter.Condition)
                                {
                                    case StiFilterCondition.GreaterThan:
                                        oper = CodeBinaryOperatorType.GreaterThan;
                                        break;

                                    case StiFilterCondition.GreaterThanOrEqualTo:
                                        oper = CodeBinaryOperatorType.GreaterThanOrEqual;
                                        break;

                                    case StiFilterCondition.LessThan:
                                        oper = CodeBinaryOperatorType.LessThan;
                                        break;

                                    case StiFilterCondition.LessThanOrEqualTo:
                                        oper = CodeBinaryOperatorType.LessThanOrEqual;
                                        break;

                                    case StiFilterCondition.EqualTo:
                                        oper = CodeBinaryOperatorType.ValueEquality;
                                        break;

                                    case StiFilterCondition.NotEqualTo:
                                        oper = CodeBinaryOperatorType.IdentityInequality;
                                        break;
                                }
                                #endregion

                                return
                                    new CodeBinaryOperatorExpression(
                                        columnCastedExpr, oper, codeExpr1);
                            }
                            #endregion
                        }
                        #endregion
                    }
                }
                #endregion
            }
            catch (Exception e)
            {
                StiLogService.Write(typeof(StiCodeDomFilters), e);
            }
            return null;
        }


        internal static CodeExpression AddFilters(StiCodeDomSerializator serializator, IStiFilter filterComp)
        {
            if (filterComp == null) return null;
            var mode = filterComp.FilterMode;
            var filters = filterComp.Filters;
            if (filters == null || filters.Count == 0) return null;

            var list = new List<CodeExpression>();

            foreach (StiFilter filter in filters)
            {
                var expr = GetFilterExpression(serializator, filterComp, filter, false, filters.Count == 1);
                if (expr != null) list.Add(expr);
            }

            var op = CodeBinaryOperatorType.BooleanAnd;
            if (mode == StiFilterMode.Or) op = CodeBinaryOperatorType.BooleanOr;

            CodeExpression baseExpr = null;

            if (list.Count == 0) return null;

            foreach (var expr in list)
            {
                if (baseExpr == null)
                    baseExpr = expr;
                else
                    baseExpr = new CodeBinaryOperatorExpression(baseExpr, op, expr);
            }
            return baseExpr;
        }


        /// <summary>
        /// Generate filters to code.
        /// </summary>
        internal static void AddFilters(StiCodeDomSerializator serializator,
            string parent, IStiFilter filterComp)
        {
            var baseExpr = AddFilters(serializator, filterComp);

            if (baseExpr == null) return;

            var ev = new StiGetFilterEvent();
            string eventName = parent + "__" + ev + string.Empty;

            var stats = new CodeStatementCollection();

            stats.Add(new CodeCommentStatement(StiCodeDomSerializator.GetCheckerInfoString(parent, "Filter")));

            stats.Add(
                new CodeAssignStatement(
                new CodeFieldReferenceExpression(
                new CodeArgumentReferenceExpression("e"), "Value"),
                baseExpr));

            serializator.GenEventMethod(eventName, ev, stats);
        }

    }
}
