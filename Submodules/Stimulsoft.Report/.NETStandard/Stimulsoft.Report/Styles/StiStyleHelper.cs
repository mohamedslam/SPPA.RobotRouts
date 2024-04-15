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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Gauge;

namespace Stimulsoft.Report
{
    /// <summary>
    /// This class contains method which helps convert component location to its border sides.
    /// </summary>
    internal static class StiStylesHelper
    {
        internal static StiBorderSides GetBorderSidesFromLocation(StiComponent component)
        {
            var sides = StiBorderSides.None;

            var compLeft = Math.Round((decimal)component.Page.Unit.ConvertToHInches(component.Left) / 10, 0);
            var compTop = Math.Round((decimal)component.Page.Unit.ConvertToHInches(component.Top) / 10, 0);

            var parentWidth = Math.Round((decimal)component.Page.Unit.ConvertToHInches(component.Parent.Width) / 10, 0);
            var parentHeight = Math.Round((decimal)component.Page.Unit.ConvertToHInches(component.Parent.Height) / 10, 0);
            var compRight = Math.Round((decimal)component.Page.Unit.ConvertToHInches(component.Left + component.Width) / 10, 0);
            var compBottom = Math.Round((decimal)component.Page.Unit.ConvertToHInches(component.Top + component.Height) / 10, 0);

            if (compLeft <= 0)
                sides |= StiBorderSides.Left;

            if (compTop <= 0)
                sides |= StiBorderSides.Top;

            if (compRight >= parentWidth)
                sides |= StiBorderSides.Right;

            if (compBottom >= parentHeight)
                sides |= StiBorderSides.Bottom;

            return sides;
        }

        internal static void ChangeComponentStyleName(StiComponent comp, string oldName, string newName)
        {
            if (comp.ComponentStyle == oldName)
                comp.ComponentStyle = newName;

            if (comp.Conditions != null && comp.Conditions.Count > 0)
            {
                for (int index = 0; index < comp.Conditions.Count; index++)
                {
                    var condition = comp.Conditions[index] as StiCondition;
                    if (condition != null && condition.Style == oldName)
                        condition.Style = newName;
                }
            }

            ChangeDataBandStyleName(comp, oldName, newName);
            ChangeElementStyleName(comp, oldName, newName);
            ChangeChartStyleName(comp, oldName, newName);
            ChangeGaugeStyleName(comp, oldName, newName);
        }

        private static void ChangeDataBandStyleName(StiComponent comp, string oldName, string newName)
        {
            var dataBand = comp as StiDataBand;
            if (dataBand == null) return;

            if (dataBand.EvenStyle == oldName)
                dataBand.EvenStyle = newName;

            if (dataBand.OddStyle == oldName)
                dataBand.OddStyle = newName;
        }

        private static void ChangeElementStyleName(StiComponent comp, string oldName, string newName)
        {
            var elementStyle = comp as IStiDashboardElementStyle;
            if (elementStyle == null || elementStyle.CustomStyleName != oldName) return;

            elementStyle.CustomStyleName = newName;
        }

        private static void ChangeChartStyleName(StiComponent comp, string oldName, string newName)
        {
            var chart = comp as StiChart;
            if (chart == null || chart.CustomStyleName != oldName) return;

            chart.CustomStyleName = newName;

            var customStyle = chart.Style as StiCustomStyle;
            if (customStyle == null) return;

            customStyle.Name = newName;

            var core = customStyle.Core as StiCustomStyleCoreXF;
            if (core == null) return;

            core.ReportStyleName = newName;
        }

        private static void ChangeGaugeStyleName(StiComponent comp, string oldName, string newName)
        {
            var gauge = comp as StiGauge;
            if (gauge == null || gauge.CustomStyleName != oldName) return;

            gauge.CustomStyleName = newName;
        }
    }
}
