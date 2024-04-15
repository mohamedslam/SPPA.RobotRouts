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

using System.Collections;

namespace Stimulsoft.Base.Plans
{
    public static class StiCloudReportResults
    {
        #region Fields.Static
        private static object lockObject = new object();
        #endregion

        #region Properties
        private static Hashtable limits = new Hashtable();
        private static Hashtable Limits
        {
            get
            {
                return limits ?? (limits = new Hashtable());
            }
        }
        #endregion

        #region Methods
        public static void InitMaxReportPages(string reportGuid, int max)
        {
            lock (lockObject)
            {
                var limit = GetLimits(reportGuid);

                limit.MaxReportPages = max;
            }
        }

        public static void InitMaxDataRows(string reportGuid, int max)
        {
            lock (lockObject)
            {
                var limit = GetLimits(reportGuid);
                limit.MaxDataRows = max;
            }
        }

        private static StiCloudReportLimits GetLimits(string reportGuid)
        {
            lock (lockObject)
            {
                var limits = Limits[reportGuid] as StiCloudReportLimits;
                if (limits == null)
                {
                    limits = new StiCloudReportLimits();
                    Limits[reportGuid] = limits;
                }
                return limits;
            }
        }

        public static void ClearLimits(string reportGuid)
        {
            lock (lockObject)
            {
                if (Limits.ContainsKey(reportGuid))
                    Limits.Remove(reportGuid);
            }
        }


        public static StiCloudReportLimits GetAndRemoveLimits(string reportGuid)
        {
            var limits = GetLimits(reportGuid);
            if (limits != null)
            {
                lock (lockObject)
                    Limits.Remove(reportGuid);
            }
            return limits;
        }
        #endregion
    }
}