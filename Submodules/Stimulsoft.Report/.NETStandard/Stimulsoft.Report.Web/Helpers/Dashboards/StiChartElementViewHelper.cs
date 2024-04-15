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
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Gauge;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiChartElementViewHelper
    {
        #region Methods
        private static bool IsAllowInteractive(IStiChartElement chartElement)
        {
            return chartElement.GetArgumentByIndex(0) != null || chartElement.GetSeries() != null;
        }

        public static string GetArgumentColumnPath(IStiChartElement chartElement)
        {
            if (!IsAllowInteractive(chartElement))
                return null;

            string expression = null;
            int argumentIndex = 0;
            if (chartElement is IStiDrillDownElement) argumentIndex = ((IStiDrillDownElement)chartElement).DrillDownCurrentLevel;

            var argument = chartElement.GetArgumentByIndex(argumentIndex);
            if (argument != null)
                expression = StiExpressionHelper.RemoveFunction(argument.Expression);

            expression = expression?.Split(',').FirstOrDefault();

            var column = StiDataExpressionHelper.GetDataColumnFromExpression(chartElement.Page as IStiDashboard, expression) as StiDataColumn;
            if (column?.DataSource == null) return null;

            return $"{column.DataSource.Name}.{column.Name}";
        }

        public static string GetSeriesColumnPath(IStiChartElement chartElement)
        {
            if (!IsAllowInteractive(chartElement))
                return null;

            string expression = null;

            var series = chartElement.GetSeries();
            if (series != null)
                expression = StiExpressionHelper.RemoveFunction(series.Expression);

            var column = StiDataExpressionHelper.GetDataColumnFromExpression(chartElement.Page as IStiDashboard, expression) as StiDataColumn;
            if (column?.DataSource == null)
                return null;

            return $"{column.DataSource.Name}.{column.Name}";
        }

        public static string GetBubleXColumnPath(IStiChartElement chartElement)
        {
            if (!IsAllowInteractive(chartElement))
                return null;

            string expression = null;

            var argument = chartElement.GetArgumentByIndex(0);
            if (argument != null)
                expression = StiExpressionHelper.RemoveFunction(argument.Expression);

            var column = StiDataExpressionHelper.GetDataColumnFromExpression(chartElement.Page as IStiDashboard, expression) as StiDataColumn;
            if (column?.DataSource == null) return null;

            return $"{column.DataSource.Name}.{column.Name}";
        }

        public static string GetBubleYColumnPath(IStiChartElement chartElement)
        {
            if (!IsAllowInteractive(chartElement))
                return null;

            string expression = null;

            var value = chartElement.GetValueByIndex(0);
            if (value != null)
                expression = StiExpressionHelper.RemoveFunction(value.Expression);

            var column = StiDataExpressionHelper.GetDataColumnFromExpression(chartElement.Page as IStiDashboard, expression) as StiDataColumn;
            if (column?.DataSource == null) return null;

            return $"{column.DataSource.Name}.{column.Name}";
        }

        public static List<double?[]> GetSeriesValues(StiChart chart)
        {
            var rootList = new List<double?[]>();
            foreach (IStiSeries series in chart.Series)
            {
                rootList.Add(series.Values);
            }

            return rootList;
        }

        public static object GetChartValuesFromCache(string cacheGuid, StiRequestParams requestParams)
        {
            return requestParams.Cache.Helper.GetObjectInternal(requestParams, cacheGuid);
        }

        public static void SaveChartValuesToCache(string cacheGuid, StiPage page, StiRequestParams requestParams)
        {
            foreach (var component in page.Components)
            {
                if (component is StiChart)
                {
                    requestParams.Cache.Helper.SaveObjectInternal(GetSeriesValues(component as StiChart), requestParams, cacheGuid);
                }
            }
        }

        public static object GetChartAnimationsFromCache(string cacheGuid, StiRequestParams requestParams)
        {
            return requestParams.Cache.Helper.GetObjectInternal(requestParams, cacheGuid + "_animations");
        }

        public static void SaveChartAnimationsToCache(IStiElement element, string cacheGuid, StiPage page, StiRequestParams requestParams)
        {
            if (element is IStiChartElement chartElement)
            {
                foreach (var component in page.Components)
                {
                    if (component is StiChart chart)
                    {
                            //chartElement.PreviousAnimations = chart.PreviousAnimations;

                        requestParams.Cache.Helper.SaveObjectInternal(chart.PreviousAnimations, requestParams, cacheGuid + "_animations");
                    }
                }
            }
            else if (element is IStiGaugeElement gaugeElement)
            {
                requestParams.Cache.Helper.SaveObjectInternal(gaugeElement.PreviousAnimations, requestParams, cacheGuid + "_animations");
            }
        }

        public static void RemoveChartAnimationsFromCache(string cacheGuid, StiRequestParams requestParams)
        {
            requestParams.Cache.Helper.RemoveObject(cacheGuid + "_animations");
        }

        public static bool IsBubble(IStiChartElement chartElement)
        {
            var value = chartElement.GetValueByIndex(0);
            if (value != null)
            {
                var seriesType = StiInvokeMethodsHelper.GetPropertyValue(value, "SeriesType");
                if (seriesType != null && seriesType.ToString() == "Bubble")
                    return true;
            }

            return false;
        }

        internal static ArrayList GetUserViewStates(IStiChartElement chartElement)
        {
            var viewStates = new ArrayList();

            (chartElement as IStiUserViewStates).UserViewStates.ForEach(viewState =>
            {
                viewStates.Add(
                    new Hashtable()
                    {
                        ["name"] = viewState.Name,
                        ["key"] = viewState.Key,
                        ["seriesType"] = viewState.SeriesType
                    });
            });

            return viewStates;
        }

        internal static Hashtable ChangeChartElementViewState(StiReport report, StiRequestParams requestParams)
        {
            var elementName = requestParams.GetString("chartElementName");
            var viewStateKey = requestParams.GetString("chartElementViewStateKey");

            var chartElement = report != null && elementName != null ? report.Pages.GetComponentByName(elementName) as IStiChartElement : null;
            if (chartElement != null)
            {
                var elementViewStates = chartElement as IStiUserViewStates;
                elementViewStates.SwitchSelectedViewState(viewStateKey);

                var elements = ((IStiDashboard)chartElement.Page).GetElements(true).Where(e => e.IsEnabled).ToList();
                var renderSingleElement = !string.IsNullOrEmpty(requestParams.Viewer.ElementName) && requestParams.Action != StiAction.DashboardDrillDown;
                var totalFixedHeight = 0;
                List<StiRangeBand> bands = null;

                StiReportHelper.CalculatePositionForEachBand(requestParams, elements, chartElement.Page, out bands, out totalFixedHeight);

                return StiReportHelper.GetElementAttributes(chartElement.Page, chartElement, renderSingleElement, requestParams, bands, totalFixedHeight);
            }

            return null;
        }
        #endregion
    }
}
