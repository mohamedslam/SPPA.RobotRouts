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

using Stimulsoft.Data.Engine;
using Stimulsoft.Report.Dashboard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiDashboardElementDrillDownHelper
    {
        #region Methods
        public static void ApplyDashboardElementDrillDown(StiReport report, Hashtable parameters)
        {
            if (parameters == null || parameters.Count == 0) return;

            var element = report.Pages.GetComponentByName(parameters["elementName"] as string) as IStiElement;
            var filters = parameters["filters"] as ArrayList;

            ApplyDrillDownToElement(element, filters);
        }

        public static void ApplyDrillDownToElement(IStiElement element, ArrayList filters)
        {
            if (element == null || filters == null) return;
            var drillDownElement = element as IStiDrillDownElement;

            if (drillDownElement != null)
            {
                var drillDownFilters = new List<StiDataFilterRule>();
                foreach (Hashtable filter in filters)
                {
                    var filterCondition = (StiDataFilterCondition)Enum.Parse(typeof(StiDataFilterCondition), filter["condition"] as string);

                    drillDownFilters.Add(new StiDataFilterRule(
                        filter["key"] as string,
                        filter["path"] as string,
                        filter["path2"] as string,
                        filterCondition,
                        filter["value"] as string,
                        filter["value2"] as string,
                        true,
                        false));
                }

                drillDownElement.DrillDownFiltersList.Add(drillDownElement.DrillDownFilters);

                var list = drillDownElement.DrillDownFilters?.ToList();
                if (list == null)
                    list = new List<StiDataFilterRule>();

                if (drillDownFilters != null && drillDownFilters.Count > 0)
                    list.AddRange(drillDownFilters);


                drillDownElement.DrillDownFilters = list;
                drillDownElement.DrillDownCurrentLevel++;
            }
        }

        public static void ApplyDashboardElementDrillUp(StiReport report, Hashtable parameters)
        {
            if (parameters == null || parameters.Count == 0) return;

            var element = report.Pages.GetComponentByName(parameters["elementName"] as string) as IStiElement;

            ApplyDrillUpToElement(element);
        }

        public static void ApplyDrillUpToElement(IStiElement element)
        {
            if (element == null) return;

            var drillDownElement = element as IStiDrillDownElement;
            if (drillDownElement != null)
            {
                drillDownElement.DrillDownCurrentLevel--;

                if (drillDownElement.DrillDownCurrentLevel < 0)
                    drillDownElement.DrillDownCurrentLevel = 0;

                if (drillDownElement.DrillDownFiltersList.Count > 0)
                {
                    drillDownElement.DrillDownFilters = drillDownElement.DrillDownFiltersList[drillDownElement.DrillDownCurrentLevel];
                    drillDownElement.DrillDownFiltersList.RemoveAt(drillDownElement.DrillDownFiltersList.Count - 1);
                }
            }
        }
        #endregion
    }
}
