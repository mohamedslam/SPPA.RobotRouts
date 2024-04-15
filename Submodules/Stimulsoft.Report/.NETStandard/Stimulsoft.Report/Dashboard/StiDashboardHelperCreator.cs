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

using Stimulsoft.Base;
using Stimulsoft.Report.Dashboard.Visuals;

namespace Stimulsoft.Report.Dashboard
{
    public static class StiDashboardHelperCreator
	{
        #region Methods
        public static IStiTableElementAutoSizer CreateTableElementAutoSizer(IStiTableElement tableElement)
	    {
	        try
	        {
	            if (StiDashboardDrawingAssembly.Assembly == null)
	                return null;

	            var type = StiDashboardDrawingAssembly.Assembly.GetType("Stimulsoft.Dashboard.Drawing.Helpers.StiTableElementAutoSizer");
	            if (type == null)
	                return null;

	            return StiActivator.CreateObject(type) as IStiTableElementAutoSizer;
	        }
	        catch
	        {
	        }

	        return null;
	    }

        public static IStiProgressVisualSvgHelper CreateProgressVisualSvgHelper()
        {
            try
            {
                if (StiDashboardAssembly.Assembly == null)
                    return null;

                var type = StiDashboardAssembly.Assembly.GetType("Stimulsoft.Dashboard.Helpers.StiProgressVisualSvgHelper");
                if (type == null)
                    return null;

                return StiActivator.CreateObject(type) as IStiProgressVisualSvgHelper;
            }
            catch
            {
            }

            return null;
        }

        public static IStiIndicatorVisualSvgHelper CreateIndicatorVisualSvgHelper()
        {
            try
            {
                if (StiDashboardAssembly.Assembly == null)
                    return null;

                var type = StiDashboardAssembly.Assembly.GetType("Stimulsoft.Dashboard.Helpers.StiIndicatorVisualSvgHelper");
                if (type == null)
                    return null;

                return StiActivator.CreateObject(type) as IStiIndicatorVisualSvgHelper;
            }
            catch
            {
            }

            return null;
        }

        public static IStiGaugeVisualSvgHelper CreateGaugeVisualSvgHelper()
        {
            try
            {
                if (StiDashboardAssembly.Assembly == null)
                    return null;

                var type = StiDashboardAssembly.Assembly.GetType("Stimulsoft.Dashboard.Helpers.StiGaugeVisualSvgHelper");
                if (type == null)
                    return null;

                return StiActivator.CreateObject(type) as IStiGaugeVisualSvgHelper;
            }
            catch
            {
            }

            return null;
        }

        internal static IStiCardsVisualSvgHelper CreateCardsVisualSvgHelper()
        {
            try
            {
                if (StiDashboardAssembly.Assembly == null)
                    return null;

                var type = StiDashboardAssembly.Assembly.GetType("Stimulsoft.Dashboard.Helpers.StiCardsVisualSvgHelper");
                if (type == null)
                    return null;

                return StiActivator.CreateObject(type) as IStiCardsVisualSvgHelper;
            }
            catch
            {
            }

            return null;
        }
        #endregion
    }
}
