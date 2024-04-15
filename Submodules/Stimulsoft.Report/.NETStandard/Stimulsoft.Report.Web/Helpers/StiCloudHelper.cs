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
using Stimulsoft.Base.Licenses;
using Stimulsoft.Base.Plans;
using System;
using System.Collections;

namespace Stimulsoft.Report.Web
{
    internal class StiCloudHelper
    {
        #region Methods
        public static Hashtable GetPlansLimits()
        {
            var limits = new Hashtable();
            limits["trial"] = new StiCloudPlans.Trial();
            limits["base"] = new StiCloudPlans.Developer();
            limits["single"] = new StiCloudPlans.Single();
            limits["team"] = new StiCloudPlans.Team();
            limits["enterprise"] = new StiCloudPlans.Enterprise();

            return limits;
        }

        internal static Hashtable GetProductIdentKeys()
        {
            var keys = new Hashtable();
            for (int i = 0; i <= 30; i++)
            {
                var identName = ((StiProductIdent)i).ToString();
                if (identName != i.ToString())
                    keys[identName] = i;
            }
            return keys;
        }

#if CLOUD

        internal static StiCloudPlanIdent ParseCloudPlanIdent(object ident)
        {
            int identNum = 0;
            int.TryParse(ident?.ToString(), out identNum);
            return (StiCloudPlanIdent)identNum;
        }

        internal static void ClearCloudLimits(StiReport report)
        {
            if (report != null)
            {
                if (string.IsNullOrEmpty(report.ReportGuid))
                    report.ReportGuid = StiGuidUtils.NewGuid();

                StiCloudReportResults.ClearLimits(report.ReportGuid);
                StiCloudReport.ResetDataRows(report.ReportGuid);
            }
        }

        internal static void SetCloudPlan(StiReport report, StiCloudPlanIdent planIdent)
        {
            if (report != null)
            {
                if (string.IsNullOrEmpty(report.ReportGuid))
                    report.ReportGuid = StiGuidUtils.NewGuid();

                StiCloudPlan.SetPlan(report.ReportGuid, planIdent);
            }
        }
#endif
        #endregion
    }
}
