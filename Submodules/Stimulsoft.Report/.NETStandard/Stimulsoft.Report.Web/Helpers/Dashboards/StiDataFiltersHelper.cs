#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Engine;
using Stimulsoft.Data.Extensions;
using Stimulsoft.Data.Helpers;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Helpers;
using Stimulsoft.Report.Dashboard.Styles;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiDataFiltersHelper
    {
        #region Methods
        public static void ApplyFiltering(StiReport report, Hashtable parameters)
        {
            if (parameters == null || parameters.Count == 0) return;

            var element = report.Pages.GetComponentByName(parameters["elementName"] as string) as IStiElement;
            var filters = parameters["filters"] as ArrayList;

            ApplyFiltersToElement(element, filters);

            StiPivotToContainerCache.Clean(report.Key);
            StiPivotTableToCrossTabCache.Clean(report.Key);
            StiPivotToConvertedStateCache.Clean(report.Key);
        }

        public static void ApplyFiltersToElement(IStiElement element, ArrayList filters)
        {
            if (element == null || filters == null) return;

            if (element is IStiDatePickerElement && StiDatePickerElementViewHelper.IsVariablePresent((IStiDatePickerElement)element))
            {
                ApplyDatePickerFiltersToVariable((IStiDatePickerElement)element, filters);
                return;
            }

            var filterElement = element as IStiUserFilters;
            if (filterElement != null)
            {
                //reset all nested elements filters
                if (!StiCrossLinkedFilterHelper.IsCrossLinkedFilter(element as IStiFilterElement))
                {
                    element.Page.GetComponents().ToList()
                    .Where(c => c is IStiFilterElement)
                    .Cast<IStiFilterElement>()
                    .Where(l => l.GetParentKey() == element.Key)
                    .Where(c => c != null && c is IStiUserFilters)
                    .ToList()
                    .ForEach(c =>
                    {
                        ((IStiUserFilters)c).UserFilters = new List<StiDataFilterRule>();
                    });
                }

                filterElement.UserFilters = new List<StiDataFilterRule>();

                foreach (Hashtable filter in filters)
                {
                    var filterCondition = (StiDataFilterCondition)Enum.Parse(typeof(StiDataFilterCondition), filter["condition"] as string);

                    filterElement.UserFilters.Add(new StiDataFilterRule(
                         filter["key"] as string,
                         filter["path"] as string,
                         filter["path2"] as string,
                         filterCondition,
                         filter["value"] as string,
                         filter["value2"] as string,
                         true,
                         false));
                }
            }
        }

        public static ArrayList GetElementFilters(IStiElement element)
        {
            ArrayList filters = new ArrayList();
            var filterElement = element as IStiUserFilters;

            if (filterElement != null && filterElement.UserFilters != null)
            {
                filterElement.UserFilters.ForEach(f => filters.Add(FilterRuleItem(f)));
            }

            return filters;
        }

        public static ArrayList GetDrillDownFilters(IStiDrillDownElement drillDownElement)
        {
            ArrayList filters = new ArrayList();
            if (drillDownElement != null && drillDownElement.DrillDownFilters != null)
            {
                drillDownElement.DrillDownFilters.ForEach(f => filters.Add(FilterRuleItem(f)));
            }

            return filters;
        }

        public static ArrayList GetDrillDownFiltersList(IStiDrillDownElement drillDownElement)
        {
            ArrayList filtersList = new ArrayList();
            if (drillDownElement != null && drillDownElement.DrillDownFiltersList != null)
            {
                drillDownElement.DrillDownFiltersList.ForEach(fs =>
                {
                    var filters_ = new ArrayList();
                    filtersList.Add(filters_);
                    fs.ForEach(f => filters_.Add(FilterRuleItem(f)));
                });
            }

            return filtersList;
        }

        public static Hashtable GetFilterItems(StiReport report, StiRequestParams requestParams)
        {
            var elementName = requestParams.Interaction.DashboardFiltering["elementName"] as string;
            var columnIndex = Convert.ToInt32(requestParams.Interaction.DashboardFiltering["columnIndex"]);
            var element = report.Pages.GetComponentByName(elementName) as IStiElement;

            if (element != null)
            {
                var meters = element.GetMeters().AsEnumerable().Where(m => m is IStiTableColumn).ToList();
                return GetFilterItemsHelper(element.Page as IStiDashboard, meters, columnIndex, (element as IStiUserSorts)?.UserSorts, (element as IStiUserFilters)?.UserFilters, element);
            }
            else
                return null;
        }

        private static StiDataTable GetDataTable(IStiElement element)
        {
            var dataTable = (element as IStiManuallyEnteredData)?.GetManuallyEnteredDataTable();
            if (dataTable != null)
                return dataTable;

            return StiElementDataCache.TryToGetOrCreate(element);
        }

        public static Hashtable GetViewData(StiReport report, StiRequestParams requestParams)
        {
            var elementName = requestParams.Interaction.DashboardFiltering["elementName"] as string;
            var element = report.Pages.GetComponentByName(elementName) as IStiElement;
            var cloneElement = (element as StiComponent)?.Clone() as IStiElement;
            var result = new Hashtable();
            var resultData = new List<object>();
            var dataCells = new List<object>();
            var headerCells = new List<object>();

            if (element != null && cloneElement != null)
            {
                #region Settings
                var style = StiDashboardStyleHelper.GetStyle(element);
                var tableStyle = StiDashboardStyleHelper.GetTableStyle(style);
                var tempTableElement = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.Table.StiTableElement") as IStiTableElement;
                tempTableElement.Style = tableStyle.Ident;
                result["settings"] = StiTableElementViewHelper.GetTableSettings(tempTableElement);
                #endregion

                #region Data                
                cloneElement.Report = element.Report;

                if (cloneElement is IStiUserSorts)
                    ((IStiUserSorts)cloneElement).UserSorts = new List<StiDataSortRule>();

                var dataTable = GetDataTable(cloneElement);
                if (dataTable != null)
                {
                    var netTable = RemoveSystemMeters(cloneElement, dataTable);
                    var sortDirection = requestParams.Interaction.DashboardFiltering["sortDirection"] as string;
                    var sortColumnIndex = requestParams.Interaction.DashboardFiltering["columnIndex"] != null ? Convert.ToInt32(requestParams.Interaction.DashboardFiltering["columnIndex"]) : -1;

                    if (sortColumnIndex >= 0 && sortDirection != null && sortColumnIndex < netTable.Columns.Count)
                    {
                        var columnName = netTable.Columns[sortColumnIndex].ColumnName;
                        netTable.DefaultView.Sort = $"[{columnName}] {(sortDirection == "Ascending" ? "Asc" : "Desc")}";
                        netTable = netTable.DefaultView.ToTable(true);
                    }

                    var columnIndex = 0;

                    foreach (DataColumn column in netTable.Columns)
                    {
                        var columnObj = new Hashtable();
                        columnObj["columnIndex"] = columnIndex;
                        columnObj["label"] = column.Caption;
                        columnObj["labelCorrect"] = column.Caption;
                        columnObj["dataType"] = TypeToString(column.DataType);
                        columnObj["align"] = TypeExt.IsNumericType(column.DataType) ? "Left" : "Right";
                        columnObj["cellForeColor"] = StiReportHelper.GetHtmlColor(tableStyle.CellForeColor);
                        columnObj["cellForeColorInterlaced"] = StiReportHelper.GetHtmlColor(tableStyle.AlternatingCellForeColor);

                        if (sortColumnIndex >= 0 && sortDirection != null && columnIndex == sortColumnIndex)
                            columnObj["sortLabel"] = new Hashtable() { ["direction"] = sortDirection };

                        headerCells.Add(columnObj);

                        var rowIndex = 0;
                        foreach (DataRow row in netTable.Rows)
                        {
                            if (dataCells.Count < netTable.Rows.Count)
                                dataCells.Add(new List<object>());

                            var cellObject = new Hashtable();
                            cellObject["text"] = row[columnIndex];

                            ((List<object>)dataCells[rowIndex]).Add(cellObject);
                            rowIndex++;
                        }
                        columnIndex++;
                    }

                    if (headerCells.Count > 0)
                        resultData.Add(headerCells);

                    if (dataCells.Count > 0)
                        resultData.AddRange(dataCells);

                    result["data"] = resultData;
                }
                #endregion
            }

            return result;
        }

        private static DataTable RemoveSystemMeters(IStiElement element, StiDataTable dataTable)
        {
            var table = dataTable.ToNetTable();

            //#region Check Gauge
            if (element is IStiGaugeElement)
            {
                var minMeter = dataTable.Meters.FirstOrDefault(m => m is IStiMinGaugeMeter);
                var maxMeter = dataTable.Meters.FirstOrDefault(m => m is IStiMaxGaugeMeter);

                var minMeterIndex = minMeter != null ? dataTable.Meters.IndexOf(minMeter) : -1;
                var maxMeterIndex = maxMeter != null ? dataTable.Meters.IndexOf(maxMeter) : -1;

                if (minMeterIndex != -1)
                    table.Columns.RemoveAt(minMeterIndex);

                if (maxMeterIndex != -1)
                {
                    if (maxMeterIndex > minMeterIndex)
                        maxMeterIndex--;

                    table.Columns.RemoveAt(maxMeterIndex);
                }
            }
            //#endregion

            //#region Check is Generic
            var removeIndexlist = new List<int>();

            for (var index = 0; index < table.Columns.Count; index++)
            {
                if (table.Columns[index].DataType.IsGenericType)
                {
                    removeIndexlist.Add(index);
                }
            }

            removeIndexlist.Reverse();
            removeIndexlist.ForEach(i => table.Columns.RemoveAt(i));
            //#endregion

            return table;
        }

        public static string GetDataTableFilterQueryStringRepresentation(IStiElement element)
        {
            var filter = element as IStiUserFilters;
            if (filter == null)
                return "";

            var filters = filter.UserFilters.Select(f => f.Clone()).Cast<StiDataFilterRule>().ToList();
            filters.ForEach(f =>
            {
                var index = f.Path != null ? f.Path.IndexOfInvariant(".") : -1;
                if (index != -1)
                    f.Path = f.Path.Substring(index + 1);

                var value = StiValueHelper.TryToNullableDateTime(f.Value);
                if (value != null)
                    f.Value = value.Value.ToShortDateString();

                var value2 = StiValueHelper.TryToNullableDateTime(f.Value2);
                if (value2 != null)
                    f.Value2 = value2.Value.ToShortDateString();
            });

            return StiDataFilterRuleHelper.GetDataTableFilterQuery(filters, null, null, null);
        }

        public static bool IsBlankData(object data)
        {
            return data == null || data == DBNull.Value || (data is string && string.IsNullOrEmpty(data as string));
        }

        public static bool IsStringColumnType(IStiElement element)
        {
            string columnPath = null;

            if (element is IStiListBoxElement)
                columnPath = StiListBoxElementViewHelper.GetColumnPath(element as IStiListBoxElement);
            else if (element is IStiComboBoxElement)
                columnPath = StiComboBoxElementViewHelper.GetColumnPath(element as IStiComboBoxElement);
            else if (element is IStiTreeViewElement)
                columnPath = StiTreeViewElementViewHelper.GetColumnPath(element as IStiTreeViewElement);
            else if (element is IStiTreeViewBoxElement)
                columnPath = StiTreeViewBoxElementViewHelper.GetColumnPath(element as IStiTreeViewBoxElement);

            var dataColumn = StiDataColumn.GetDataColumnFromColumnName(element.Report.Dictionary, columnPath);
            return dataColumn?.Type == typeof(string);
        }

        internal static void ResetAllFilters(StiReport report, StiRequestParams requestParams)
        {
            if (requestParams.Viewer.PageNumber < report.Pages.Count)
            {
                var page = report.Pages[requestParams.Viewer.PageNumber];

                if (page.IsDashboard)
                {
                    page.GetComponents().ToList()
                    .Where(c => c is IStiElement && c is IStiUserFilters)
                    .ToList()
                    .ForEach(c => { ((IStiUserFilters)c).UserFilters = new List<StiDataFilterRule>(); });

                    ApplyDefaultFiltersForFilterElements(page);
                }
            }
        }
        #endregion

        #region Methods.Helpers
        public static Hashtable FilterRuleItem(StiDataFilterRule filterRule)
        {
            Hashtable filterRuleItem = new Hashtable();
            filterRuleItem["typeItem"] = "FilterRule";
            filterRuleItem["key"] = filterRule.Key;
            filterRuleItem["path"] = filterRule.Path;
            filterRuleItem["condition"] = filterRule.Condition;
            filterRuleItem["value"] = filterRule.Value;
            filterRuleItem["value2"] = filterRule.Value2;
            filterRuleItem["isEnabled"] = filterRule.IsEnabled;
            filterRuleItem["isExpression"] = filterRule.IsExpression;

            return filterRuleItem;
        }

        private static Hashtable SortFilterMenuItem(object item, Type type)
        {
            Hashtable menuItem = new Hashtable();
            menuItem["type"] = TypeToString(type);
            menuItem["displayString"] = StiDataFiltersHelper.ToDisplayString(item, type);
            menuItem["filterString"] = StiDataFiltersHelper.ToFilterString(item, type);

            return menuItem;
        }

        public static Hashtable GetFilterItemsHelper(IStiQueryObject query, List<IStiMeter> meters, int columnIndex, List<StiDataSortRule> sorts, List<StiDataFilterRule> filters, IStiElement element = null)
        {
            var itemsHelper = new Hashtable();
            if (meters == null || columnIndex >= meters.Count || meters[columnIndex] == null) return null;

            var dataColumn = StiDataExpressionHelper.GetDataColumnFromExpression(query, meters[columnIndex].Expression) as StiDataColumn;
            if (dataColumn == null) return null;

            var dataColumnTable = StiDataPicker.GetFromCache(dataColumn.DataSource);
            if (dataColumnTable == null) return null;

            var columnPath = $"{dataColumn.DataSource.Name}.{dataColumn.Name}";
            var dataColumnIndex = dataColumnTable.Columns.IndexOf(columnPath);
            if (dataColumnIndex == -1) return null;

            itemsHelper["columnPath"] = columnPath;

            var columnType = dataColumnTable.Columns[dataColumnIndex].DataType;
            var comparer = new StiDataFilterComparer();
            var items = Distinct(dataColumnTable.AsEnumerable()
                .Select(r => r[dataColumnIndex]))
                .Where(IsValueCanBeFiltered)
                .OrderBy(c => c, comparer)
                .ToList();

            itemsHelper["mainItems"] = items
                .Where(i => i != null && i != DBNull.Value && !string.IsNullOrWhiteSpace(i.ToString()))
                .Select(i => SortFilterMenuItem(i, columnType))
                .ToList();

            itemsHelper["haveBlanks"] = items.Any(i => i != null && i != DBNull.Value && string.IsNullOrWhiteSpace(i.ToString()));
            itemsHelper["haveNulls"] = items.Any(i => i == null || i == DBNull.Value);

            return itemsHelper;
        }

        public static string TypeToString(Type type)
        {
            if (type == null) return "null";
            if (type == typeof(bool)) return "bool";
            if (type == typeof(byte)) return "byte";
            if (type == typeof(byte[])) return "byte[]";
            if (type == typeof(char)) return "char";
            if (type == typeof(DateTime)) return "datetime";
            if (type == typeof(DateTimeOffset)) return "datetimeoffset";
            if (type == typeof(decimal)) return "decimal";
            if (type == typeof(double)) return "double";
            if (type == typeof(Guid)) return "guid";
            if (type == typeof(short)) return "short";
            if (type == typeof(int)) return "int";
            if (type == typeof(long)) return "long";
            if (type == typeof(sbyte)) return "sbyte";
            if (type == typeof(float)) return "float";
            if (type == typeof(string)) return "string";
            if (type == typeof(TimeSpan)) return "timespan";
            if (type == typeof(ushort)) return "ushort";
            if (type == typeof(uint)) return "uint";
            if (type == typeof(ulong)) return "ulong";
            if (type == typeof(Image)) return "image";

            if (type == typeof(bool?)) return "bool (Nullable)";
            if (type == typeof(byte?)) return "byte (Nullable)";
            if (type == typeof(char?)) return "char (Nullable)";
            if (type == typeof(DateTime?)) return "datetime (Nullable)";
            if (type == typeof(DateTimeOffset?)) return "datetimeoffset (Nullable)";
            if (type == typeof(decimal?)) return "decimal (Nullable)";
            if (type == typeof(double?)) return "double (Nullable)";
            if (type == typeof(Guid?)) return "guid (Nullable)";
            if (type == typeof(short?)) return "short (Nullable)";
            if (type == typeof(int?)) return "int (Nullable)";
            if (type == typeof(long?)) return "long (Nullable)";
            if (type == typeof(sbyte?)) return "sbyte (Nullable)";
            if (type == typeof(float?)) return "float (Nullable)";
            if (type == typeof(TimeSpan?)) return "timespan (Nullable)";
            if (type == typeof(ushort?)) return "ushort (Nullable)";
            if (type == typeof(uint?)) return "uint (Nullable)";
            if (type == typeof(ulong?)) return "ulong (Nullable)";

            if (type == typeof(object)) return "object";

            return type.ToString();
        }

        public static string ToFilterString(object value, Type type = null)
        {
            if (value == DBNull.Value || value == null)
                return null;

            if (type == null) type = value.GetType();

            if (string.IsNullOrWhiteSpace(value.ToString()))
                return string.Empty;

            if (type != null && type.IsEnum && Enum.IsDefined(type, value))
                return Enum.GetName(type, value);

            if (type != null && TypeExt.IsNumericType(type))
                return value.ToString().Replace(",", ".");

            if (value is DateTime)
                return ((DateTime)value).ToString("MM'/'dd'/'yyyy");

            return value.ToString();
        }

        public static string ToDisplayString(object value, Type type = null)
        {
            if (type == null) type = value.GetType();

            if (value == DBNull.Value || value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return string.Empty;

            if (type.IsEnum && Enum.IsDefined(type, value))
                return Enum.GetName(type, value);

            if (value is DateTime)
                return ((DateTime)value).ToShortDateString();

            var str = value.ToString();

            return str.Length > 100 ? $"{str.Substring(0, 100)}..." : str;
        }

        private static IEnumerable<object> Distinct(IEnumerable<object> items)
        {
            if (!(items.FirstOrDefault() is DateTime))
                return items.Distinct();

            return items
                .Select(StiValueHelper.TryToNullableDateTime)
                .Where(d => d != null)
                .Cast<DateTime>()
                .GroupBy(x => x.Date)
                .Select(x => x.Key)
                .Cast<object>()
                .Distinct();
        }

        private static bool IsValueCanBeFiltered(object value)
        {
            if (value == null || value == DBNull.Value)
                return true;

            var type = value.GetType();
            return type.IsNumericType() || type.IsDateType() || type == typeof(string) || type == typeof(bool);
        }

        private static int GetLevel(IStiFilterElement element, List<IStiFilterElement> elements)
        {
            if (StiCrossLinkedFilterHelper.IsCrossLinkedFilter(element))
                return 0;

            var level = 0;
            while (true)
            {
                if (string.IsNullOrWhiteSpace(element.GetParentKey()))
                    return level;

                element = elements.FirstOrDefault(e => e.GetKey() == element.GetParentKey());
                if (element == null) break;
                level++;
            }
            return level;
        }

        public static void ApplyDefaultFiltersForFilterElements(StiReport report)
        {
            var dashboards = report?.Pages.OfType<StiPage>().Where(p => p.Enabled && p.IsDashboard).ToList();

            dashboards.ForEach(d => ApplyDefaultFiltersForFilterElements(d));
        }

        private static void ApplyDefaultFiltersForFilterElements(StiPage dashboard)
        {
            var filterElements = dashboard.GetComponents().ToList().Where(c => c.IsEnabled && c is IStiFilterElement).Cast<IStiFilterElement>().ToList();
            var filterElementList = filterElements?.OrderBy(f => GetLevel(f, filterElements)).ToList();

            if (filterElementList.Any())
                filterElementList.ForEach(f => f.ApplyDefaultFilters());
        }

        private static void ApplyDatePickerFiltersToVariable(IStiDatePickerElement datePickerElement, ArrayList filters)
        {
            var variable = StiVariableExpressionHelper.GetVariableSpecifiedAsExpression(datePickerElement, datePickerElement.GetValueMeter()?.Expression) as StiVariable;
            if (variable != null)
                foreach (Hashtable filter in filters)
                {
                    var value = variable.Type == typeof(DateTimeRange)
                        ? new DateTimeRange(TryToDateTime(filter["value"] as string), TryToDateTime(filter["value2"] as string))
                        : (object)TryToDateTime(filter["value"] as string);

                    StiVariableHelper.SetVariableValue(datePickerElement.Report, variable, value);

                    if (variable.InitBy == StiVariableInitBy.Value)
                        variable.ValueObject = value;

                    StiCacheCleaner.Clean(datePickerElement.Report.Key);
                }
        }

        internal static DateTime TryToDateTime(string stringValue)
        {
            DateTime value = DateTime.Now;
            DateTime.TryParse(stringValue, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out value);
            return value;
        }
        #endregion
    }
}
