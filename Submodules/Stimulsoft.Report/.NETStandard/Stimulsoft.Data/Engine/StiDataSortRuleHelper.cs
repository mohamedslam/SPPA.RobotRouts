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
using System.Data;
using System.Linq;
using System.Text;
using Stimulsoft.Base;

namespace Stimulsoft.Data.Engine
{
    public class StiDataSortRuleHelper
    {
        #region Methods.Static
        /// <summary>
        /// Used in the CodeDom report compiler.
        /// </summary>
        public static List<StiDataSortRule> ToList(params StiDataSortRule[] rules)
        {
            return rules.ToList();
        }

        internal static List<StiDataSortRule> Validate(List<StiDataSortRule> rules, List<string> columnKeys)
        {
            if (columnKeys == null || columnKeys.Count == 0)
                return rules;

            return rules.Where(r => StiKeyHelper.IsKey(r.Key) && columnKeys.Contains(r.Key)).ToList();
        }

        internal static string GetDataTableSortQuery(List<StiDataSortRule> rules, IEnumerable<IStiAppDataColumn> columns)
        {
            var columnKeys = columns.Select(c => c?.GetKey()).Where(c => c != null).ToList();
            var columnNames = columns.Select(c => c?.GetName()).Where(c => c != null).ToList();
            
            return GetDataTableSortQuery(rules, columnKeys, columnNames);
        }

        internal static string GetDataTableSortQuery(List<StiDataSortRule> rules, List<string> columnKeys, List<string> columnNames, bool normalize = false)
        {
            if (rules == null || !rules.Any())
                return string.Empty;

            rules = Validate(rules, columnKeys);
            var sb = new StringBuilder();

            foreach (var rule in rules.Where(r => !string.IsNullOrWhiteSpace(r.Key)))
            {
                if (sb.Length > 0)
                    sb = sb.Append(", ");

                var columnIndex = columnKeys.IndexOf(rule.Key);
                if (columnIndex == -1)
                {
                    columnIndex = columnNames.IndexOf(rule.Key);
                    if (columnIndex == -1)continue;
                }

                var columnName = StiDataColumnRuleHelper.GetGoodColumnName(columnNames[columnIndex], normalize);

                if (rule.Direction == StiDataSortDirection.Descending)
                    sb = sb.Append($"{columnName} DESC");
                else
                    sb = sb.Append(columnName);
            }

            return sb.ToString();
        }

        internal static DataTable OrderDataTableWithCommaInColumnNames(DataTable table, List<StiDataSortRule> rules, List<string> columnKeys, List<string> columnNames)
        {
            var oldNames = table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
            NormalizeCommas(table);

            table.DefaultView.Sort = GetDataTableSortQuery(rules, columnKeys, columnNames, true);
            table = table.DefaultView.ToTable();

            var columnIndex = 0;
            table.Columns.Cast<DataColumn>().ToList().ForEach(c => c.ColumnName = oldNames[columnIndex++]);

            return table;
        }

        internal static bool IsCommaInColumnNames(DataTable table)
        {
            return table.Columns.Cast<DataColumn>()
                .Any(c => c.ColumnName.Contains(",") || c.ColumnName.Contains("'"));
        }

        internal static void NormalizeCommas(DataTable table)
        {
            table.Columns.Cast<DataColumn>()
                .Where(c => c.ColumnName.Contains(",") || c.ColumnName.Contains("'"))
                .ToList()
                .ForEach(c => c.ColumnName = c.ColumnName.Replace(",", "").Replace("'", ""));
        }

        public static StiDataSortDirection GetSortDirection(List<StiDataSortRule> rules, string columnKey)
        {
            if (rules == null)
                return StiDataSortDirection.None;

            var rule = rules.FirstOrDefault(r => string.Equals(r.Key, columnKey, StringComparison.InvariantCultureIgnoreCase));

            return rule == null
                ? StiDataSortDirection.None
                : rule.Direction;
        }

        public static List<StiDataSortRule> SetSortDirection(List<StiDataSortRule> rules, List<string> columnKeys, string columnKey, StiDataSortDirection direction)
        {
            rules = Validate(rules, columnKeys);
            var rule = rules.FirstOrDefault(r => string.Equals(r.Key, columnKey, StringComparison.InvariantCultureIgnoreCase));

            if (rule == null && direction != StiDataSortDirection.None)
            {
                rule = new StiDataSortRule(columnKey, direction);
                rules.Add(rule);
            }
            else
            {
                if (direction == StiDataSortDirection.None)
                    rules.Remove(rule);
                else
                    rule.Direction = direction;
            }

            return rules;
        }
        #endregion
    }
}