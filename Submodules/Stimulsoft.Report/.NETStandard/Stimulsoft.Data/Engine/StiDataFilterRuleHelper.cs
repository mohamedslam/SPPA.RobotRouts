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

using Stimulsoft.Base;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Wizards;
using Stimulsoft.Data.Exceptions;
using Stimulsoft.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace Stimulsoft.Data.Engine
{
    public class StiDataFilterRuleHelper
    {
        #region Methods.Static
        /// <summary>
        /// Used in the CodeDom report compiler.
        /// </summary>
        public static List<StiDataFilterRule> ToList(params StiDataFilterRule[] rules)
        {
            return rules.ToList();
        }

        internal static List<StiDataFilterRule> Validate(List<StiDataFilterRule> rules, List<string> columnKeys)
        {
            return rules.Where(r => StiKeyHelper.IsKey(r.Key) && columnKeys.Contains(r.Key)).ToList();
        }

        internal static string GetDataTableFilterQuery(List<StiDataFilterRule> rules, IEnumerable<IStiAppDataColumn> columns, IStiReport report)
        {
            return GetDataTableFilterQuery(rules,
                columns.Select(c => c?.GetName()).ToList(),
                columns.Select(c => c?.GetDataType()).ToList(), report);
        }

        internal static StiTableFiltersGroupsType GetTableFiltersGroupsType(List<StiDataFilterRule> rules)
        {
            if (rules != null && rules.Count > 1)
            {
                var groups = rules
                    .Where(r => !string.IsNullOrWhiteSpace(r.Path))
                    .GroupBy(GetFullPath);

                if (groups.Count() > 1)
                    return groups.Any(g => g.Count() > 1)
                        ? StiTableFiltersGroupsType.Complex
                        : StiTableFiltersGroupsType.Simple;
            }

            return StiTableFiltersGroupsType.None;
        }

        internal static string GetDataTableFilterQuery(List<StiDataFilterRule> rules,
            List<string> columnNames, List<Type> columnTypes, IStiReport report)
        {
            if (rules == null)
                return string.Empty;

            var resultQuery = new StringBuilder();
            if (rules.Any(r => r.Condition == StiDataFilterCondition.IsFalse))
                rules = rules.FirstOrDefault(r => r.Condition == StiDataFilterCondition.IsFalse).ToList();

            var groups = rules
                .Where(r => r.IsEnabled && !string.IsNullOrWhiteSpace(r.Path))
                .OrderBy(r => columnNames?.FindIndex(k => k == r.Path))
                .GroupBy(GetFullPath);

            var filterOperation = groups.Count() < 2 || rules.Any(r => r.Operation == StiDataFilterOperation.AND) ? " AND " : " OR ";

            foreach (var group in groups)
            {
                var groupRules = rules.Where(r => r.IsEnabled && GetFullPath(r) == group.Key);
                var groupQuery = GetFilterGroupQuery(groupRules, columnNames, columnTypes, report);

                if (groupQuery.Length > 0)
                {
                    if (resultQuery.Length > 0)
                        resultQuery = resultQuery.Append(filterOperation);

                    if (groupRules.Count() > 1 && groups.Count() > 1)
                        resultQuery = resultQuery.Append("(");

                    resultQuery = resultQuery.Append(groupQuery);

                    if (groupRules.Count() > 1 && groups.Count() > 1)
                        resultQuery = resultQuery.Append(")");
                }
            }

            return resultQuery.ToString();
        }
        
        private static string GetFullPath(StiDataFilterRule rule)
        {
            if (rule.ElementKey == null)
                return rule.Path;

            else
                return rule.Path + rule.ElementKey;
        }

        private static StringBuilder GetFilterGroupQuery(IEnumerable<StiDataFilterRule> groupRules,
            List<string> columnNames, List<Type> columnTypes, IStiReport report)
        {
            var querySingleOr = new StringBuilder();
            var querySingleAnd = new StringBuilder();
            var queryListOr = new StringBuilder();
            var queryListAnd = new StringBuilder();

            // IsNull condition for display NULL values
            string ruleIsNull = null;

            foreach (var rule in groupRules)
            {
                if (rule.Condition == StiDataFilterCondition.IsNull || rule.Condition == StiDataFilterCondition.IsNotNull || rule.Condition == StiDataFilterCondition.IsBlankOrNull)
                    ruleIsNull = string.Empty;

                var columnIndex = GetColumnIndex(columnNames, rule.Path);
                var columnName = GetColumnName(columnNames, columnIndex, rule.Path);
                var columnType = GetColumnType(columnTypes, columnIndex);
                
                var value = rule.IsExpression
                    ? GetValue(columnType, StiExpressionHelper.ParseReportExpression(report, rule.Value, false, true))
                    : GetValue(columnType, rule.Value);

                var isListType = columnType.IsNumericType() || columnType.IsStringType() || columnType.IsBooleanType();

                // List for EqualTo condition
                if (isListType && rule.Condition == StiDataFilterCondition.EqualTo)
                {
                    var queryValue = GetQueryValue(value, columnType, true);
                    queryListOr.Append(queryListOr.Length == 0 ? $"{columnName} IN (" : ",");
                    queryListOr.Append(queryValue);
                }
                // List for NotEqualTo condition
                else if (isListType && rule.Condition == StiDataFilterCondition.NotEqualTo)
                {
                    var queryValue = GetQueryValue(value, columnType, true);
                    queryListAnd.Append(queryListAnd.Length == 0 ? $"{columnName} NOT IN (" : ",");
                    queryListAnd.Append(queryValue);

                    if (ruleIsNull == null)
                        ruleIsNull = $"{columnName} IS NULL";
                }
                else
                {
                    // It is used only for Between, NotBetween and PairEqualTo conditions
                    var value2 = value;

                    // It is used only for PairEqualTo condition
                    var columnName2 = columnName;
                    var columnType2 = columnType;

                    if (rule.Condition == StiDataFilterCondition.Between ||
                        rule.Condition == StiDataFilterCondition.NotBetween ||
                        rule.Condition == StiDataFilterCondition.PairEqualTo)
                    {
                        value2 = rule.IsExpression
                            ? GetValue(columnType2, StiExpressionHelper.ParseReportExpression(report, rule.Value2, false, true))
                            : GetValue(columnType2, rule.Value2);

                        if (rule.Condition == StiDataFilterCondition.PairEqualTo)
                        {
                            var columnIndex2 = GetColumnIndex(columnNames, rule.Path2);
                            columnName2 = GetColumnName(columnNames, columnIndex2, rule.Path2);
                            columnType2 = GetColumnType(columnTypes, columnIndex2);
                        }
                    }

                    if (rule.Condition == StiDataFilterCondition.NotEqualTo && ruleIsNull == null)
                        ruleIsNull = $"{columnName} IS NULL";

                    var condition = GetCondition(columnName, columnName2, rule.Condition, value, value2, columnType, columnType2);
                    var operation = GetFilterOperation(rule);

                    if (operation == StiDataFilterOperation.OR)
                        querySingleOr.Append(querySingleOr.Length > 0 ? $" OR {condition}" : condition);
                    else
                        querySingleAnd.Append(querySingleAnd.Length > 0 ? $" AND {condition}" : condition);
                }
            }

            // Make the group query
            var groupQuery = new StringBuilder();

            if (queryListOr.Length > 0)
                groupQuery.Append($"{queryListOr})");

            if (querySingleOr.Length > 0)
            {
                if (queryListOr.Length > 0)
                    groupQuery.Append(" OR ");

                groupQuery.Append(querySingleOr);

                if (groupQuery.ToString().IndexOf(" OR ") > 0 && queryListAnd.Length > 0 && querySingleAnd.Length > 0)
                {
                    groupQuery.Insert(0, "(");
                    groupQuery.Append(")");
                }
            }

            if (queryListAnd.Length > 0)
            {
                if (groupQuery.Length > 0)
                    groupQuery.Append(" AND ");

                groupQuery.Append(!string.IsNullOrEmpty(ruleIsNull) ? $"({queryListAnd}) OR {ruleIsNull})" : $"{queryListAnd})");
            }

            if (querySingleAnd.Length > 0)
            {
                if (groupQuery.Length > 0)
                    groupQuery.Append(" AND ");

                groupQuery.Append(!string.IsNullOrEmpty(ruleIsNull) ? $"({querySingleAnd} OR {ruleIsNull})" : querySingleAnd.ToString());
            }

            return groupQuery;
        }

        private static int GetColumnIndex(List<string> columnNames, string path)
        {
            if (columnNames == null || string.IsNullOrEmpty(path))
                return 0;

            var columnIndex = columnNames.IndexOf(path) != -1
                ? columnNames.IndexOf(path)
                : columnNames.IndexOf(GetCleanedPath(path));

            if (columnIndex == -1)
            {
                columnNames = columnNames.Select(c => c?.Replace(" ", "")).ToList();
                columnIndex = columnNames.IndexOf(path) != -1
                    ? columnNames.IndexOf(path)
                    : columnNames.IndexOf(GetCleanedPath(path));
            }

            return columnIndex;
        }

        private static string GetCleanedPath(string path)
        {
            if (path.StartsWith("["))
                path = path.Substring(1);

            if (path.EndsWith("]"))
                path = path.Substring(0, path.Length - 1);

            return path;
        }

        private static string GetColumnName(List<string> columnNames, int columnIndex, string path)
        {
            return StiDataColumnRuleHelper.GetGoodColumnName(columnNames != null && columnIndex != -1 ? columnNames[columnIndex] : path);
        }

        private static Type GetColumnType(List<Type> columnTypes, int columnIndex)
        {
            return columnTypes != null && columnIndex != -1 ? columnTypes[columnIndex] : typeof(object);
        }

        /// <summary>
        /// Tries to resave numbers in en-US culture.
        /// </summary>
        private static string GetValue(Type type, string value)
        {
            if (type == typeof(string))
                return value;

            if (type != typeof(double) && type != typeof(float) && type != typeof(decimal))
                return value == null ? string.Empty : value;

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                if (type == typeof(double))
                {
                    var doubleValue = StiValueHelper.TryToDouble(value);
                    return doubleValue.ToString();
                }

                if (type == typeof(float))
                {
                    var floatValue = StiValueHelper.TryToFloat(value);
                    return floatValue.ToString();
                }

                if (type == typeof(decimal))
                {
                    var decimalValue = StiValueHelper.TryToDecimal(value);
                    return decimalValue.ToString();
                }

                return value;
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        private static string GetCondition(string columnName, string columnName2, StiDataFilterCondition condition, string value, string value2, Type columnType, Type columnType2)
        {
            switch (condition)
            {
                case StiDataFilterCondition.EqualTo:
                    value = GetQueryValue(value, columnType, true);
                    return columnType.IsDateType()
                        ? $"({columnName} >= #{value} 00:00:00# AND {columnName} <= #{value} 23:59:59#)"
                        : $"{columnName} = {value}";

                case StiDataFilterCondition.NotEqualTo:
                    value = GetQueryValue(value, columnType, true);
                    return columnType.IsDateType()
                        ? $"({columnName} < #{value} 00:00:00# OR {columnName} > #{value} 23:59:59#)"
                        : $"{columnName} <> {value}";

                case StiDataFilterCondition.GreaterThan:
                    value = GetQueryValue(value, columnType, true);
                    return columnType.IsDateType()
                        ? $"{columnName} > #{value} 23:59:59#"
                        : $"{columnName} > {value}";

                case StiDataFilterCondition.GreaterThanOrEqualTo:
                    value = GetQueryValue(value, columnType, true);
                    return columnType.IsDateType()
                        ? $"{columnName} >= #{value} 00:00:00#"
                        : $"{columnName} >= {value}";

                case StiDataFilterCondition.LessThan:
                    value = GetQueryValue(value, columnType, true);
                    return columnType.IsDateType()
                        ? $"{columnName} < #{value} 00:00:00#"
                        : $"{columnName} < {value}";

                case StiDataFilterCondition.LessThanOrEqualTo:
                    value = GetQueryValue(value, columnType, true);
                    return columnType.IsDateType()
                        ? $"{columnName} <= #{value} 23:59:59#"
                        : $"{columnName} <= {value}";

                case StiDataFilterCondition.Between:
                    value = GetQueryValue(value, columnType, true);
                    value2 = GetQueryValue(value2, columnType, true);
                    return columnType.IsDateType()
                        ? $"({columnName} >= #{value} 00:00:00# AND {columnName} <= #{value2} 23:59:59#)"
                        : $"({columnName} >= {value} AND {columnName} <= {value2})";

                case StiDataFilterCondition.NotBetween:
                    value = GetQueryValue(value, columnType, true);
                    value2 = GetQueryValue(value2, columnType, true);
                    return columnType.IsDateType()
                        ? $"({columnName} < #{value} 00:00:00# OR {columnName} > #{value2} 23:59:59#)"
                        : $"({columnName} < {value} OR {columnName} > {value2})";

                case StiDataFilterCondition.Containing:
                    value = GetQueryValue(value, columnType, false);
                    return $"{columnName} LIKE '*{value}*'";

                case StiDataFilterCondition.NotContaining:
                    value = GetQueryValue(value, columnType, false);
                    return $"NOT ({columnName} LIKE '*{value}*')";

                case StiDataFilterCondition.BeginningWith:
                    value = GetQueryValue(value, columnType, false);
                    return $"{columnName} LIKE '{value}*'";

                case StiDataFilterCondition.EndingWith:
                    value = GetQueryValue(value, columnType, false);
                    return $"{columnName} LIKE '*{value}'";

                case StiDataFilterCondition.IsNull:
                    return $"{columnName} IS NULL";

                case StiDataFilterCondition.IsNotNull:
                    return $"{columnName} IS NOT NULL";

                case StiDataFilterCondition.IsBlank:
                    return $"TRIM({columnName}) = ''";
                    
                case StiDataFilterCondition.IsNotBlank:
                    return $"TRIM({columnName}) <> ''";

                case StiDataFilterCondition.IsBlankOrNull:
                    return $"(TRIM({columnName}) = '' OR {columnName} IS NULL)";

                case StiDataFilterCondition.IsFalse:
                    return "FALSE";

                case StiDataFilterCondition.PairEqualTo:
                    var oper = value == null ? "IS" : "=";
                    var oper2 = value2 == null ? "IS" : "=";

                    value = value != null || columnType.IsDateType() ? GetQueryValue(value, columnType, true) : "NULL";
                    value2 = value2 != null || columnType2.IsDateType() ? GetQueryValue(value2, columnType2, true) : "NULL";

                    var part = columnType.IsDateType()
                        ? $"{columnName} >= #{value} 00:00:00# AND {columnName} <= #{value} 23:59:59# AND "
                        : $"{columnName} {oper} {value} AND ";

                    return columnType2.IsDateType()
                        ? part + $"{columnName2} >= #{value2} 00:00:00# AND {columnName2} <= #{value2} 23:59:59#"
                        : part + $"{columnName2} {oper2} {value2}";

                case StiDataFilterCondition.MapEqualTo:
                    value = GetQueryValue(value, columnType, true);
                    return $"{columnName} = {value}";

                default:
                    throw new StiTypeNotRecognizedException(condition);
            }
        }

        private static string GetQueryValue(string value, Type type, bool stringQuotes)
        {
            if (value == null)
                value = string.Empty;

            if (type == null)
                return value;

            if (type.IsNumericType())
            {
                if (string.IsNullOrEmpty(value))
                    return "0";

                decimal num;
                if (!decimal.TryParse(value.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out num))
                    return "0";

                return value;
            }

            if (type == typeof(bool))
                return (value != null && value.ToLower() == "true").ToString().ToUpper();

            if (type.IsDateType())
            {
                var currentCulture = Thread.CurrentThread.CurrentCulture;
                try
                {
                    DateTime date;
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                    if (!DateTime.TryParseExact(value, "MM'/'dd'/'yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    {
                        var date2 = StiValueHelper.TryToNullableDateTime(value);
                        if (date2 != null)
                            date = date2.Value;
                        else
                            date = new DateTime(1800, 1, 1);
                    }

                    return date.ToString("MM-dd-yyyy");
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                }                
            }

            if (type.IsEnum && Enum.IsDefined(type, value))
            {
                value = ParseEnum(value, type);
            }

            if (!string.IsNullOrEmpty(value))
                value = value.Replace("'", "''");

            if (stringQuotes)
                return $"'{value}'";

            return value;
        }

        private static string ParseEnum(string value, Type type)
        {
            var valueStr = value as string;
            if (valueStr != null)
            {
                var names = Enum.GetNames(type);
                if (names.Any(n => n.ToLowerInvariant() == valueStr.ToLowerInvariant()))
                    return ((int)Enum.Parse(type, valueStr, true)).ToString();
            }

            if (value != null)
                return Enum.GetName(type, value);

            return value;
        }

        private static StiDataFilterOperation GetFilterOperation(StiDataFilterRule rule)
        {
            return
                rule.Condition == StiDataFilterCondition.EqualTo ||
                rule.Condition == StiDataFilterCondition.BeginningWith ||
                rule.Condition == StiDataFilterCondition.EndingWith ||
                rule.Condition == StiDataFilterCondition.Between ||
                rule.Condition == StiDataFilterCondition.Containing ||
                rule.Condition == StiDataFilterCondition.GreaterThan ||
                rule.Condition == StiDataFilterCondition.GreaterThanOrEqualTo ||
                rule.Condition == StiDataFilterCondition.LessThan ||
                rule.Condition == StiDataFilterCondition.LessThanOrEqualTo ||
                rule.Condition == StiDataFilterCondition.IsNull ||
                rule.Condition == StiDataFilterCondition.IsBlank ||
                rule.Condition == StiDataFilterCondition.IsBlankOrNull ||
                rule.Condition == StiDataFilterCondition.PairEqualTo ||
                rule.Condition == StiDataFilterCondition.MapEqualTo
                    ? StiDataFilterOperation.OR
                    : StiDataFilterOperation.AND;
        }

        internal static int GetFilterRulesHash(IStiApp app, IEnumerable<StiDataFilterRule> rules)
        {
            if (rules == null || !rules.Any())
                return 0;

            var variables = app?.GetDictionary()?.FetchVariables();
            if (variables == null || !variables.Any())
                return 0;

            return rules
                .Select(r => GetFilterRuleHash(variables, r))
                .Aggregate(0, (c1, c2) => unchecked(c1 + c2));
        }

        private static int GetFilterRuleHash(IEnumerable<IStiAppVariable> variables, StiDataFilterRule rule)
        {
            if (!rule.IsExpression)
                return 0;

            switch (rule.Condition)
            {
                case StiDataFilterCondition.Between:
                case StiDataFilterCondition.NotBetween:
                    return unchecked(GetFilterRuleHash(variables, rule.Value) + GetFilterRuleHash(variables, rule.Value2));

                default:
                    return GetFilterRuleHash(variables, rule.Value);
            }
        }

        private static int GetFilterRuleHash(IEnumerable<IStiAppVariable> variables, string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return 0;

            expression = expression.ToLowerInvariant();

            var hash = 0;

            foreach (var variable in variables)
            {
                if (expression.Contains(variable.GetName().ToLowerInvariant()))
                {
                    var value = variable.GetValue()?.ToString();
                    var variableHash = value != null ? value.GetHashCode() : 0;

                    hash = unchecked(hash + variableHash);
                }
            }

            return hash;
        }
        #endregion
    }
}