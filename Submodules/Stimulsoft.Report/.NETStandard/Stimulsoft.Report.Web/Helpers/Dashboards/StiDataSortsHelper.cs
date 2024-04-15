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

using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Engine;
using Stimulsoft.Data.Helpers;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiDataSortsHelper
    {
        #region Methods
        public static void ApplySorting(StiReport report, Hashtable parameters)
        {
            if (parameters == null || parameters.Count == 0) return;

            var element = report.Pages.GetComponentByName(parameters["elementName"] as string) as IStiElement;
            var sorts = parameters["sorts"] as ArrayList;

            ApplySortsToElement(element, sorts);
        }

        public static void ApplySortsToElement(IStiElement element, ArrayList sorts)
        {
            if (element == null || sorts == null) return;

            var sortElement = element as IStiUserSorts;
            if (sortElement != null)
            {
                sortElement.UserSorts = new List<StiDataSortRule>();

                foreach (Hashtable sort in sorts)
                {
                    var sortDirection = (StiDataSortDirection)Enum.Parse(typeof(StiDataSortDirection), sort["direction"] as string);
                    sortElement.UserSorts.Add(new StiDataSortRule(sort["key"] as string, sortDirection));
                }
            }
        }

        public static ArrayList GetElementSorts(IStiElement element)
        {
            ArrayList sorts = new ArrayList();
            var sortElement = element as IStiUserSorts;

            if (sortElement != null)
            {
                foreach (var sortRule in sortElement.UserSorts)
                {
                    sorts.Add(SortRuleItem(sortRule));
                }
            }

            return sorts;
        }
        #endregion

        #region Methods.Helpers
        public static Hashtable SortRuleItem(StiDataSortRule sortRule)
        {
            Hashtable sortRuleItem = new Hashtable();
            sortRuleItem["typeItem"] = "SortRule";
            sortRuleItem["key"] = sortRule.Key;
            sortRuleItem["direction"] = sortRule.Direction;

            return sortRuleItem;
        }

        public static ArrayList GetSortMenuItems(IStiElement element)
        {
            var items = new ArrayList();
            var sortingDashboardInteraction = (element as IStiElementInteraction)?.DashboardInteraction as IStiAllowUserSortingDashboardInteraction;
            var userSorts = (element as IStiUserSorts)?.UserSorts;
            var sortDirection = GetSortDirection(userSorts).ToString();

            if (sortingDashboardInteraction != null && sortingDashboardInteraction.AllowUserSorting && StiSortMenuHelper.IsAllowUserSorting(element))
            {
                var manualDataTable = GetManualDataTable(element);

                //Arguments
                var allArguments = FetchAllArguments(element, manualDataTable);
                if (allArguments != null)
                {
                    var arguments = allArguments.Select(a => StiLabelHelper.GetLabel(a)).Where(a => !string.IsNullOrWhiteSpace(a));
                    if (arguments.Any())
                    {
                        var argument = allArguments.FirstOrDefault();
                        var item = new Hashtable();
                        item["text"] = string.Join(", ", arguments);
                        item["checked"] = userSorts != null && userSorts.Any(r => r.Key == argument.Key);
                        item["key"] = argument.Key;
                        item["sortDirection"] = sortDirection;
                        items.Add(item);
                    }
                }

                //Values
                var allValues = FetchAllValues(element, manualDataTable);
                if (allValues != null)
                {
                    allValues.ForEach(value =>
                    {
                        var column = StiLabelHelper.GetLabel(value);
                        if (!string.IsNullOrWhiteSpace(column))
                        {
                            var item = new Hashtable();
                            item["text"] = column;
                            item["checked"] = userSorts != null && userSorts.Any(r => r.Key == value.Key);
                            item["key"] = value.Key;
                            item["sortDirection"] = sortDirection;
                            items.Add(item);
                        }
                    });
                }

                //Series
                var series = GetSeries(element, manualDataTable);
                if (series != null)
                {
                    var column = StiLabelHelper.GetLabel(series);
                    if (!string.IsNullOrWhiteSpace(column))
                    {
                        var item = new Hashtable();
                        item["text"] = column;
                        item["checked"] = userSorts != null && userSorts.Any(r => r.Key == series.Key);
                        item["key"] = series.Key;
                        item["sortDirection"] = sortDirection;
                        items.Add(item);
                    }
                }

                if (element is IStiAllowSortByVariation && allArguments != null && allValues != null)
                {
                    var item = new Hashtable();
                    item["isSortByVariation"] = true;
                    item["checked"] = StiDataSortVariation.IsVariationSort(userSorts);
                    item["key"] = StiDataSortVariation.Ident;
                    item["sortDirection"] = sortDirection;
                    items.Add(item);
                }
            }

            return items;
        }

        private static StiDataSortDirection GetSortDirection(List<StiDataSortRule> userSorts)
        {
            if (userSorts == null || userSorts.Count == 0)
                return StiDataSortDirection.Ascending;

            return userSorts.FirstOrDefault().Direction;
        }

        private static List<IStiMeter> FetchAllArguments(IStiElement element, StiDataTable manualDataTable)
        {
            if (element == null)
                return null;

            else if (element is IStiChartElement)
            {
                var chartElement = element as IStiChartElement;
                if (manualDataTable != null)
                {
                    return IsArgumentPresentedInManualData(manualDataTable)
                        ? manualDataTable?.Meters?.Where(m => m.Key == "Argument")?.ToList()
                        : null;
                }

                return chartElement.FetchAllArguments();
            }

            else if (element is IStiIndicatorElement && ((IStiIndicatorElement)element).GetSeries() != null && ((IStiIndicatorElement)element).GetTarget() != null)
                return new List<IStiMeter> { ((IStiIndicatorElement)element).GetTarget() };

            else if (element is IStiProgressElement && ((IStiProgressElement)element).GetSeries() != null && ((IStiProgressElement)element).GetTarget() != null)
                return new List<IStiMeter> { ((IStiProgressElement)element).GetTarget() };

            else if (element is IStiGaugeElement && ((IStiGaugeElement)element).GetSeries() != null && ((IStiGaugeElement)element).GetTarget() != null)
                return new List<IStiMeter> { ((IStiGaugeElement)element).GetTarget() };

            else
                return null;
        }

        private static List<IStiMeter> FetchAllValues(IStiElement element, StiDataTable manualDataTable)
        {
            if (element == null)
                return null;

            else if (element is IStiChartElement)
            {
                var chartElement = element as IStiChartElement;

                if (manualDataTable != null)
                {
                    return IsValuePresentedInManualData(manualDataTable)
                        ? manualDataTable?.Meters?.Where(m => m.Key == "Value")?.ToList()
                        : null;
                }
                return chartElement.FetchAllValues();
            }

            else if (element is IStiIndicatorElement && ((IStiIndicatorElement)element).GetSeries() != null && ((IStiIndicatorElement)element).GetValue() != null)
                return new List<IStiMeter> { ((IStiIndicatorElement)element).GetValue() };

            else if (element is IStiProgressElement && ((IStiProgressElement)element).GetSeries() != null && ((IStiProgressElement)element).GetValue() != null)
                return new List<IStiMeter> { ((IStiProgressElement)element).GetValue() };

            else if (element is IStiGaugeElement && ((IStiGaugeElement)element).GetSeries() != null && ((IStiGaugeElement)element).GetValue() != null)
                return new List<IStiMeter> { ((IStiGaugeElement)element).GetValue() };

            else
                return null;
        }

        private static IStiMeter GetSeries(IStiElement element, StiDataTable manualDataTable)
        {
            if (element == null)
                return null;

            else if (element is IStiChartElement)
            {
                var chartElement = element as IStiChartElement;

                if (manualDataTable != null)
                {
                    return IsSeriesPresentedInManualData(manualDataTable)
                        ? manualDataTable?.Meters?.Where(m => m.Key == "Series")?.FirstOrDefault()
                        : null;
                }

                return chartElement.GetSeries();
            }

            else if (element is IStiIndicatorElement)
                return ((IStiIndicatorElement)element).GetSeries();

            else if (element is IStiProgressElement)
                return ((IStiProgressElement)element).GetSeries();

            else if (element is IStiGaugeElement)
                return ((IStiGaugeElement)element).GetSeries();

            else
                return null;
        }

        private static StiDataTable GetManualDataTable(IStiElement element)
        {
            if (element is IStiChartElement &&
                element is IStiManuallyEnteredData manuallyEnteredData &&
                manuallyEnteredData.DataMode == StiDataMode.ManuallyEnteringData)
            {
                return manuallyEnteredData.GetManuallyEnteredDataTable();
            }

            return null;
        }

        private static bool IsValuePresentedInManualData(StiDataTable manualDataTable)
        {
            var meter = manualDataTable.Meters.FirstOrDefault(m => m.Key == "Value");
            if (meter == null)
                return false;

            var meterIndex = manualDataTable.Meters.IndexOf(meter);

            return manualDataTable.Rows.Select(r => r[meterIndex]).Distinct().Count() > 1;
        }

        private static bool IsArgumentPresentedInManualData(StiDataTable manualDataTable)
        {
            var meter = manualDataTable.Meters.FirstOrDefault(m => m.Key == "Argument");
            if (meter == null)
                return false;

            var meterIndex = manualDataTable.Meters.IndexOf(meter);

            return manualDataTable.Rows.Select(r => r[meterIndex]).Distinct().Count() > 1;
        }

        private static bool IsSeriesPresentedInManualData(StiDataTable manualDataTable)
        {
            var meter = manualDataTable.Meters.FirstOrDefault(m => m.Key == "Series");
            if (meter == null)
                return false;

            var meterIndex = manualDataTable.Meters.IndexOf(meter);

            return manualDataTable.Rows.Select(r => r[meterIndex]).Distinct().Count() > 1;
        }
        #endregion
    }
}
