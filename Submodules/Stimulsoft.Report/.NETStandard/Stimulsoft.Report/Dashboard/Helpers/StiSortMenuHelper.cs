#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using Stimulsoft.Base.Localization;
using Stimulsoft.Data.Engine;
using System.Linq;

namespace Stimulsoft.Report.Dashboard.Helpers
{
    internal static class StiSortMenuHelper
    {
        internal static string GetSortElementName(string key, IStiElement element)
        {
            var tableElement = element as IStiTableElement;
            if (tableElement != null)
            {
                var tableColumn = tableElement.FetchAllMeters().FirstOrDefault(t => t.Key == key);
                if (tableColumn != null)
                {
                    var tableIndex = tableElement.FetchAllMeters().IndexOf(tableColumn);
                    return string.IsNullOrWhiteSpace(tableColumn.Label) ? tableColumn.Expression : tableColumn.Label;
                }
            }

            return Loc.Get("FormBand", "NoSort");
        }

        public static bool IsAllowUserSorting(IStiElement element)
        {
            var topN = element as IStiDataTopN;
            if (topN != null && topN.TopN.Mode != StiDataTopNMode.None)
                return false;

            var interaction = element as IStiElementInteraction;
            if (interaction != null)
            {
                var userSorting = interaction.DashboardInteraction as IStiAllowUserSortingDashboardInteraction;
                if (userSorting != null && !userSorting.AllowUserSorting)
                    return false;
            }

            var chartElement = element as IStiChartElement;
            if (chartElement != null)
                return (chartElement.IsAxisAreaChart && !chartElement.IsParetoChart && !chartElement.IsScatterChart) || chartElement.IsAxisAreaChart3D ||
                    chartElement.IsPictorialStackedChart || chartElement.IsPieChart || chartElement.IsPie3dChart || chartElement.IsDoughnutChart || chartElement.IsAxisAreaChart3D;

            if (element is IStiGaugeElement && (element as IStiGaugeElement)?.GetSeries() != null && (element as IStiManuallyEnteredData)?.DataMode == StiDataMode.UsingDataFields)
                return true;

            if (element is IStiProgressElement && (element as IStiProgressElement)?.GetSeries() != null && (element as IStiManuallyEnteredData)?.DataMode == StiDataMode.UsingDataFields)
                return true;

            if (element is IStiIndicatorElement && (element as IStiIndicatorElement)?.GetSeries() != null && (element as IStiManuallyEnteredData)?.DataMode == StiDataMode.UsingDataFields)
                return true;

            return false;
        }
    }
}
