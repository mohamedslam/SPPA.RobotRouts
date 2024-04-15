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
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Stimulsoft.Base;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Data.Comparers;
using Stimulsoft.Data.Functions;
using Stimulsoft.Data.Helpers;

namespace Stimulsoft.Data.Engine
{
    public class StiDataActionRuleHelper
    {
        #region Methods.Static
        /// <summary>
        /// Used in the CodeDom report compiler.
        /// </summary>
        public static List<StiDataActionRule> ToList(params StiDataActionRule[] rules)
        {
            return rules.ToList();
        }

        internal static List<StiDataActionRule> Validate(List<StiDataActionRule> rules, List<string> columnKeys)
        {
            return rules.Where(r => StiKeyHelper.IsKey(r.Key) && columnKeys.Contains(r.Key)).ToList();
        }

        private static int GetColumnIndex(StiDataActionRule action, List<string> columnKeys, List<string> columnNames)
        {
            var columnIndex = -1;

            if (columnNames != null && !string.IsNullOrEmpty(action.Path))
                columnIndex = columnNames.IndexOf(action.Path);

            if (columnIndex == -1 && columnKeys != null && !string.IsNullOrEmpty(action.Key))
                columnIndex = columnKeys.IndexOf(action.Key);

            return columnIndex;
        }

        public static void ApplyActions(DataTable table, List<StiDataActionRule> actions, List<string> columnKeys, List<string> columnNames, IStiReport report)
        {
            actions.Sort(new StiDataActionComparer());

            foreach (var action in actions)
            {
                var columnIndex = GetColumnIndex(action, columnKeys, columnNames);
                switch (action.Type)
                {
                    case StiDataActionType.Limit:
                        ApplyLimitAction(table, action.StartIndex, action.RowsCount);
                        break;

                    case StiDataActionType.Replace:
                        ApplyReplaceAction(table, columnIndex, action.ValueFrom, action.ValueTo, action.MatchCase, action.MatchWholeWord, report);
                        break;

                    case StiDataActionType.RunningTotal:
                        ApplyRunningTotalAction(table, columnIndex, action.InitialValue, report);
                        break;

                    case StiDataActionType.Percentage:
                        ApplyPercentageAction(table, columnIndex);
                        break;
                }
            }
        }

        private static void ApplyLimitAction(DataTable table, int startIndex, int rowsCount)
        {
            if (rowsCount < 0)
                rowsCount = table.Rows.Count;

            var rows = table.AsEnumerable().Skip(startIndex).Take(Math.Max(0, rowsCount));
            table.AsEnumerable()
                .Except(rows)
                .ToList()
                .ForEach(r => r.Delete());
            table.AcceptChanges();
        }

        private static void ApplyReplaceAction(DataTable table, int columnIndex, string valueFrom, string valueTo, bool matchCase, bool matchWholeWord, IStiReport report)
        {
            if (columnIndex == -1) return;
            if (table.PrimaryKey.Any(c => c == table.Columns[columnIndex])) return;

            valueFrom = StiExpressionHelper.ParseReportExpression(report, valueFrom, true);
            valueTo = StiExpressionHelper.ParseReportExpression(report, valueTo, true);

            table.AsEnumerable()
                .ToList()
                .ForEach(r =>
                {
                    var value = StiValueHelper.TryToString(r[columnIndex]);
                    if (string.IsNullOrEmpty(value)) return;

                    var regex = new Regex(matchWholeWord ? $@"\b{valueFrom}\b" : valueFrom, matchCase ? RegexOptions.CultureInvariant : RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                    r[columnIndex] = regex.Replace(value, valueTo);
                });
        }

        private static void ApplyRunningTotalAction(DataTable table, int columnIndex, string initialValue, IStiReport report)
        {
            if (columnIndex == -1) return;
            if (table.PrimaryKey.Any(c => c == table.Columns[columnIndex])) return;

            initialValue = StiExpressionHelper.ParseReportExpression(report, initialValue, true);

            decimal currentValue;
            decimal.TryParse(initialValue.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out currentValue);

            table.AsEnumerable()
                .ToList()
                .ForEach(r => r[columnIndex] = currentValue += StiValueHelper.TryToDecimal(r[columnIndex]));
        }

        private static void ApplyPercentageAction(DataTable table, int columnIndex)
        {
            if (columnIndex == -1) return;
            if (table.PrimaryKey.Any(c => c == table.Columns[columnIndex])) return;

            var sum = Funcs.Sum(table.AsEnumerable().Select(r => r[columnIndex]));
            table.AsEnumerable()
                .ToList()
                .ForEach(r => r[columnIndex] = Math.Round(StiValueHelper.TryToDecimal(r[columnIndex]) / sum * 100, 2));
        }
        #endregion
    }
}