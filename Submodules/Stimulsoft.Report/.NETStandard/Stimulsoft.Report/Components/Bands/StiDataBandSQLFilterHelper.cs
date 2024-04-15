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

using System.Text;
using Stimulsoft.Report.Dictionary;

namespace Stimulsoft.Report.Components
{
	public static class StiDataBandSQLFilterHelper
	{
        #region Methods
        public static string GetFilter(StiDataBand band, bool useDataSourceNames)
        {
            var dataSource = band.DataSource as StiSqlSource;
            if (!band.FilterOn || band.Filters.Count <= 0 || dataSource == null) return null;

            var filterExpression = new StringBuilder(" ");
            for (var index = 0; index < band.Filters.Count; index++)
            {
                var filter = band.Filters[index];
                filterExpression.Append("(");
                    
                if (useDataSourceNames)
                    filterExpression.Append(GetFilterExpression(filter, dataSource.Name + "." + filter.Column));
                else
                    filterExpression.Append(GetFilterExpression(filter,
                        filter.Item == StiFilterItem.Value ? dataSource.Columns[filter.Column].NameInSource : ""));
                    
                filterExpression.Append(")");
                if (index < band.Filters.Count - 1)
                    filterExpression.Append(band.FilterMode == StiFilterMode.And ? " AND " : " OR ");
            }

            return filterExpression.ToString();
        }

        private static string GetFilterExpression(StiFilter filter, string fullColumnName)
        {
            var filterExpression = new StringBuilder();

            #region filter.Item == StiFilterItem.Expression
            if (filter.Item == StiFilterItem.Expression)
            {
                var st = filter.Expression.Value;
                if (st != null && st.Length >= 2)
                    filterExpression.Append(st.Substring(1, st.Length - 2).Replace("==", "="));
            }
            #endregion

            #region filter.Item == StiFilterItem.Value
            else if (filter.Item == StiFilterItem.Value)
            {
                #region StiFilterDataType.String
                if (filter.DataType == StiFilterDataType.String)
                {
                    switch (filter.Condition)
                    {
                        case StiFilterCondition.EqualTo:
                            filterExpression.Append($"{fullColumnName} = '{filter.Value1}'");
                            break;

                        case StiFilterCondition.NotEqualTo:
                            filterExpression.Append($"{fullColumnName} <> '{filter.Value1}'");
                            break;

                        case StiFilterCondition.Containing:
                            filterExpression.Append($"{fullColumnName} LIKE '%{filter.Value1}%'");
                            break;

                        case StiFilterCondition.NotContaining:
                            filterExpression.Append($"{fullColumnName} NOT LIKE '%{filter.Value1}%'");
                            break;

                        case StiFilterCondition.BeginningWith:
                            filterExpression.Append($"{fullColumnName} LIKE '{filter.Value1}%'");
                            break;

                        case StiFilterCondition.EndingWith:
                            filterExpression.Append($"{fullColumnName} LIKE '%{filter.Value1}'");
                            break;
                    }
                }
                #endregion

                #region filter.DataType == StiFilterDataType.Numeric
                else if (filter.DataType == StiFilterDataType.Numeric)
                {
                    switch (filter.Condition)
                    {
                        case StiFilterCondition.EqualTo:
                            filterExpression.Append($"{fullColumnName} = {filter.Value1}");
                            break;

                        case StiFilterCondition.NotEqualTo:
                            filterExpression.Append($"{fullColumnName} <> {filter.Value1}");
                            break;

                        case StiFilterCondition.Between:
                            filterExpression.Append($"{fullColumnName} >= {filter.Value1} AND {fullColumnName} <= {filter.Value2}");
                            break;

                        case StiFilterCondition.NotBetween:
                            filterExpression.Append($"{fullColumnName} < {filter.Value1} OR {fullColumnName} > {filter.Value2}");
                            break;

                        case StiFilterCondition.GreaterThan:
                            filterExpression.Append($"{fullColumnName} > {filter.Value1}");
                            break;

                        case StiFilterCondition.GreaterThanOrEqualTo:
                            filterExpression.Append($"{fullColumnName} >= {filter.Value1}");
                            break;

                        case StiFilterCondition.LessThan:
                            filterExpression.Append($"{fullColumnName} < {filter.Value1}");
                            break;

                        case StiFilterCondition.LessThanOrEqualTo:
                            filterExpression.Append($"{fullColumnName} <= {filter.Value1}");
                            break;
                    }
                }
                #endregion

                #region filter.DataType == StiFilterDataType.DateTime
                else if (filter.DataType == StiFilterDataType.DateTime)
                {
                    var dt1 = string.Empty;
                    if (!string.IsNullOrEmpty(filter.Value1))
                    {
                        var parts = filter.Value1.Split('/');
                        dt1 = $"'{parts[2]}-{parts[0]}-{parts[1]}'";
                    }
                    var dt2 = string.Empty;
                    if (!string.IsNullOrEmpty(filter.Value2))
                    {
                        var parts = filter.Value2.Split('/');
                        dt2 = $"'{parts[2]}-{parts[0]}-{parts[1]}'";
                    }

                    switch (filter.Condition)
                    {
                        case StiFilterCondition.EqualTo:
                            filterExpression.Append($"{fullColumnName} = {dt1}");
                            break;

                        case StiFilterCondition.NotEqualTo:
                            filterExpression.Append($"{fullColumnName} <> {dt1}");
                            break;

                        case StiFilterCondition.Between:
                            filterExpression.Append($"{fullColumnName} >= {dt1} AND {fullColumnName} <= {dt2}");
                            break;

                        case StiFilterCondition.NotBetween:
                            filterExpression.Append($"{fullColumnName} < {dt1} OR {fullColumnName} > {dt2}");
                            break;

                        case StiFilterCondition.GreaterThan:
                            filterExpression.Append($"{fullColumnName} > {dt1}");
                            break;

                        case StiFilterCondition.GreaterThanOrEqualTo:
                            filterExpression.Append($"{fullColumnName} >= {dt1}");
                            break;

                        case StiFilterCondition.LessThan:
                            filterExpression.Append($"{fullColumnName} < {dt1}");
                            break;

                        case StiFilterCondition.LessThanOrEqualTo:
                            filterExpression.Append($"{fullColumnName} <= {dt1}");
                            break;
                    }
                }
                #endregion

                #region filter.DataType == StiFilterDataType.Boolean
                else if (filter.DataType == StiFilterDataType.Boolean)
                {
                    switch (filter.Condition)
                    {
                        case StiFilterCondition.EqualTo:
                            filterExpression.Append($"{fullColumnName} = {filter.Value1}");
                            break;

                        case StiFilterCondition.NotEqualTo:
                            filterExpression.Append($"{fullColumnName} <> {filter.Value1}");
                            break;
                    }
                }
                #endregion

                #region filter.DataType == StiFilterDataType.Expression
                else if (filter.DataType == StiFilterDataType.Expression)
                {
                    switch (filter.Condition)
                    {
                        case StiFilterCondition.EqualTo:
                            filterExpression.Append($"{fullColumnName} = {filter.Value1}");
                            break;

                        case StiFilterCondition.NotEqualTo:
                            filterExpression.Append($"{fullColumnName} <> ){filter.Value1}");
                            break;

                        case StiFilterCondition.Between:
                            filterExpression.Append($"{fullColumnName} >= {filter.Value1} AND {fullColumnName} <= {filter.Value2}");
                            break;

                        case StiFilterCondition.NotBetween:
                            filterExpression.Append($"{fullColumnName} < {filter.Value1} OR {fullColumnName} > {filter.Value2}");
                            break;

                        case StiFilterCondition.GreaterThan:
                            filterExpression.Append($"{fullColumnName} > {filter.Value1}");
                            break;

                        case StiFilterCondition.GreaterThanOrEqualTo:
                            filterExpression.Append($"{fullColumnName} >= {filter.Value1}");
                            break;

                        case StiFilterCondition.LessThan:
                            filterExpression.Append($"{fullColumnName} < {filter.Value1}");
                            break;

                        case StiFilterCondition.LessThanOrEqualTo:
                            filterExpression.Append($"{fullColumnName} <= {filter.Value1}");
                            break;
                    }
                }
                #endregion
            }
            #endregion

            return filterExpression.ToString();
        }
        #endregion
    }
}