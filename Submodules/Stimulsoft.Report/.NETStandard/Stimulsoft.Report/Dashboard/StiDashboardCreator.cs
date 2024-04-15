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
{	TRADE SECRETS OF Stimulsoft										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using System.Linq;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Dashboard
{
	public static class StiDashboardCreator
	{
        public static IStiDashboard CreateDashboard(StiReport report)
        {
            var comp = StiOptions.Services.Components.FirstOrDefault(c => c is IStiDashboard)?.Clone() as StiComponent;
            if (comp != null)
            {
                comp.Report = report;

                if (comp.Guid != null)
                    comp.NewGuid();
            }

            return comp as IStiDashboard;
        }

        public static IStiCardsElement CreateCardsElement(StiReport report)
        {
            var comp = StiOptions.Services.Components.FirstOrDefault(c => c is IStiCardsElement)?.Clone() as StiComponent;
            if (comp != null)
            {
                comp.Page = report.GetCurrentPage();

                if (comp.Guid != null)
                    comp.NewGuid();
            }

            return comp as IStiCardsElement;
        }

        public static IStiButtonElement CreateButtonElement(StiReport report)
        {
            var comp = StiOptions.Services.Components.FirstOrDefault(c => c is IStiButtonElement)?.Clone() as StiComponent;
            if (comp != null)
            {
                comp.Page = report.GetCurrentPage();

                if (comp.Guid != null)
                    comp.NewGuid();
            }

            return comp as IStiButtonElement;
        }

        public static IStiChartElement CreateChartElement(StiReport report)
        {
            var comp = StiOptions.Services.Components.FirstOrDefault(c => c is IStiChartElement)?.Clone() as StiComponent;
            if (comp != null)
            {
                comp.Page = report.GetCurrentPage();

                if (comp.Guid != null)
                    comp.NewGuid();
            }

            return comp as IStiChartElement;
        }

        public static IStiShapeElement CreateShapeElement(StiReport report)
        {
            var comp = StiOptions.Services.Components.FirstOrDefault(c => c is IStiShapeElement)?.Clone() as StiComponent;
            if (comp != null)
            {
                comp.Page = report.GetCurrentPage();

                if (comp.Guid != null)
                    comp.NewGuid();
            }

            return comp as IStiShapeElement;
        }

        public static IStiGaugeElement CreateGaugeElement(StiReport report)
        {
            var comp = StiOptions.Services.Components.FirstOrDefault(c => c is IStiGaugeElement)?.Clone() as StiComponent;
            if (comp != null)
            {
                comp.Page = report.GetCurrentPage();

                if (comp.Guid != null)
                    comp.NewGuid();
            }

            return comp as IStiGaugeElement;
        }

	    public static IStiPivotTableElement CreatePivotTableElement(StiReport report)
	    {
	        var comp = StiOptions.Services.Components.FirstOrDefault(c => c is IStiPivotTableElement)?.Clone() as StiComponent;
            if (comp != null)
            {
                comp.Page = report.GetCurrentPage();

                if (comp.Guid != null)
                    comp.NewGuid();
            }

            return comp as IStiPivotTableElement;
	    }

	    public static IStiIndicatorElement CreateIndicatorElement(StiReport report)
	    {
	        var comp = StiOptions.Services.Components.FirstOrDefault(c => c is IStiIndicatorElement)?.Clone() as StiComponent;
            if (comp != null)
            {
                comp.Page = report.GetCurrentPage();

                if (comp.Guid != null)
                    comp.NewGuid();
            }

            return comp as IStiIndicatorElement;
	    }

	    public static IStiProgressElement CreateProgressElement(StiReport report)
	    {
	        var comp = StiOptions.Services.Components.FirstOrDefault(c => c is IStiProgressElement)?.Clone() as StiComponent;
            if (comp != null)
            {
                comp.Page = report.GetCurrentPage();

                if (comp.Guid != null)
                    comp.NewGuid();
            }

            return comp as IStiProgressElement;
	    }

	    public static IStiRegionMapElement CreateRegionMapElement(StiReport report)
	    {
	        var comp = StiOptions.Services.Components.FirstOrDefault(c => c is IStiRegionMapElement)?.Clone() as StiComponent;
            if (comp != null)
            {
                comp.Page = report.GetCurrentPage();

                if (comp.Guid != null)
                    comp.NewGuid();
            }

            return comp as IStiRegionMapElement;
	    }

	    public static IStiOnlineMapElement CreateOnlineMapElement(StiReport report)
	    {
	        var comp = StiOptions.Services.Components.FirstOrDefault(c => c is IStiOnlineMapElement)?.Clone() as StiComponent;
            if (comp != null)
            {
                comp.Page = report.GetCurrentPage();

                if (comp.Guid != null)
                    comp.NewGuid();
            }

            return comp as IStiOnlineMapElement;
	    }

	    public static IStiComboBoxElement CreateComboBoxElement(StiReport report)
	    {
	        var comp = StiOptions.Services.Components.FirstOrDefault(c => c is IStiComboBoxElement)?.Clone() as StiComponent;
            if (comp != null)
            {
                comp.Page = report.GetCurrentPage();

                if (comp.Guid != null)
                    comp.NewGuid();
            }

            return comp as IStiComboBoxElement;
	    }

	    public static IStiDatePickerElement CreateDatePickerElement(StiReport report)
	    {
	        var comp = StiOptions.Services.Components.FirstOrDefault(c => c is IStiDatePickerElement)?.Clone() as StiComponent;
            if (comp != null)
            {
                comp.Page = report.GetCurrentPage();

                if (comp.Guid != null)
                    comp.NewGuid();
            }

            return comp as IStiDatePickerElement;
	    }

	    public static IStiListBoxElement CreateListBoxElement(StiReport report)
	    {
	        var comp = StiOptions.Services.Components.FirstOrDefault(c => c is IStiListBoxElement)?.Clone() as StiComponent;
            if (comp != null)
            {
                comp.Page = report.GetCurrentPage();

                if (comp.Guid != null)
                    comp.NewGuid();
            }

            return comp as IStiListBoxElement;
	    }

	    public static IStiTreeViewBoxElement CreateTreeViewBoxElement(StiReport report)
	    {
	        var comp = StiOptions.Services.Components.FirstOrDefault(c => c is IStiTreeViewBoxElement)?.Clone() as StiComponent;
            if (comp != null)
            {
                comp.Page = report.GetCurrentPage();

                if (comp.Guid != null)
                    comp.NewGuid();
            }

            return comp as IStiTreeViewBoxElement;
	    }

	    public static IStiTreeViewElement CreateTreeViewElement(StiReport report)
	    {
	        var comp = StiOptions.Services.Components.FirstOrDefault(c => c is IStiTreeViewElement)?.Clone() as StiComponent;
            if (comp != null)
            {
                comp.Page = report.GetCurrentPage();

                if (comp.Guid != null)
                    comp.NewGuid();
            }

            return comp as IStiTreeViewElement;
	    }

        public static IStiImageElement CreateImageElement(StiReport report)
        {
            var comp = StiOptions.Services.Components.FirstOrDefault(c => c is IStiImageElement)?.Clone() as StiComponent;
            if (comp != null)
            {
                comp.Page = report.GetCurrentPage();

                if (comp.Guid != null)
                    comp.NewGuid();
            }

            return comp as IStiImageElement;
        }

        public static IStiRegionMapElement CreateMapElement(StiReport report)
        {
            var comp = StiOptions.Services.Components.FirstOrDefault(c => c is IStiRegionMapElement)?.Clone() as StiComponent;
            if (comp != null)
            {
                comp.Page = report.GetCurrentPage();

                if (comp.Guid != null)
                    comp.NewGuid();
            }

            return comp as IStiRegionMapElement;
        }

        public static IStiPivotTableElement CreatePivotElement(StiReport report)
        {
            var comp = StiOptions.Services.Components.FirstOrDefault(c => c is IStiPivotTableElement)?.Clone() as StiComponent;
            if (comp != null)
            {
                comp.Page = report.GetCurrentPage();

                if (comp.Guid != null)
                    comp.NewGuid();
            }

            return comp as IStiPivotTableElement;
        }

        public static IStiTableElement CreateTableElement(StiReport report)
        {
            var comp = StiOptions.Services.Components.FirstOrDefault(c => c is IStiTableElement)?.Clone() as StiComponent;
            if (comp != null)
            {
                comp.Page = report.GetCurrentPage();

                if (comp.Guid != null)
                    comp.NewGuid();
            }

            return comp as IStiTableElement;
        }

        public static IStiTextElement CreateTextElement(StiReport report)
        {
            var comp = StiOptions.Services.Components.FirstOrDefault(c => c is IStiTextElement)?.Clone() as StiComponent;
            if (comp != null)
            {
                comp.Page = report.GetCurrentPage();

                if (comp.Guid != null)
                    comp.NewGuid();
            }

            return comp as IStiTextElement;
        }
    }
}
