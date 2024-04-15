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

using Stimulsoft.Base.Licenses;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Base.Plans
{
    public abstract class StiCloudPlan : StiPlan
    {
        #region Fields.Static
        private static object lockObject = new object();
        #endregion

        #region Properties.Static
        private static Hashtable plans;
        private static Hashtable Plans
        {
            get
            {
                return plans ?? (plans = new Hashtable());
            }
        }
        #endregion

        #region Properties
        public abstract bool AllowDatabases { get; }

        public abstract bool AllowDataTransformation { get; }

        public abstract int MaxDataRows { get; }

        public abstract int MaxFileSize { get; }

        public abstract int? MaxItems { get; }

        public abstract int MaxRefreshes { get; }

        public abstract int MaxReportPages { get; }

        public abstract int MaxResources { get; }

        public abstract int MaxResourceSize { get; }    

        public abstract int? MaxViewers { get; }
        #endregion

        #region Methods
        public static bool IsTrial(string reportGuid)
        {
            return GetPlanIdent(reportGuid) == StiCloudPlanIdent.Trial;
        }

        public static bool IsDashboardsAvailable(string reportGuid)
        {
            var plan = GetPlanIdent(reportGuid);
            return
                (plan & StiCloudPlanIdent.DBase) > 0 ||
                (plan & StiCloudPlanIdent.DSingle) > 0 ||
                (plan & StiCloudPlanIdent.DTeam) > 0 ||
                (plan & StiCloudPlanIdent.DEnterprise) > 0;
        }

        public static bool IsReportsAvailable(string reportGuid)
        {
            var plan = GetPlanIdent(reportGuid);
            return
                (plan & StiCloudPlanIdent.RBase) > 0 ||
                (plan & StiCloudPlanIdent.RSingle) > 0 ||
                (plan & StiCloudPlanIdent.RTeam) > 0 ||
                (plan & StiCloudPlanIdent.REnterprise) > 0;
        }

        public static StiCloudPlanIdent GetPlanIdent(string reportGuid)
        {
            lock (lockObject)
            {
                if (string.IsNullOrWhiteSpace(reportGuid))
                    return StiCloudPlanIdent.Trial;

                var value = Plans[reportGuid];
                return value is StiCloudPlanIdent ? (StiCloudPlanIdent)value : StiCloudPlanIdent.Trial;
            }
        }

        public static StiCloudPlan GetPlan(string reportGuid)
        {
            return GetPlan(GetPlanIdent(reportGuid));
        }

        public static StiCloudPlan GetPlan(StiLicenseKey licenceKey)
        {
            if (licenceKey?.Products == null || !licenceKey.Products.Any())
                return new StiCloudPlans.Trial();

            return licenceKey.Products.Any(p => p.IsCloudAvailable()) 
                ? new StiCloudPlans.Single() 
                : new StiCloudPlans.Developer();
        }

        public static StiCloudPlan GetPlan(StiCloudPlanIdent ident)
        {
            if ((ident & StiCloudPlanIdent.REnterprise) > 0 || (ident & StiCloudPlanIdent.DEnterprise) > 0)
                return new StiCloudPlans.Enterprise();

            if ((ident & StiCloudPlanIdent.RTeam) > 0 || (ident & StiCloudPlanIdent.DTeam) > 0)
                return new StiCloudPlans.Team();

            if ((ident & StiCloudPlanIdent.RSingle) > 0 || (ident & StiCloudPlanIdent.DSingle) > 0)
                return new StiCloudPlans.Single();

            if ((ident & StiCloudPlanIdent.RBase) > 0 || (ident & StiCloudPlanIdent.DBase) > 0)
                return new StiCloudPlans.Developer();                                                
                        
            return new StiCloudPlans.Trial();
        }

        public static void SetPlan(string reportGuid, StiCloudPlanIdent plan)
        {
            lock (lockObject)
            {
                Plans[reportGuid] = plan;
            }
        }        
        #endregion
    }
}