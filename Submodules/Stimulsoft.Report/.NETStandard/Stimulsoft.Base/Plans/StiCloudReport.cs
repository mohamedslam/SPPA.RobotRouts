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

using System;
using System.Collections;

namespace Stimulsoft.Base.Plans
{
    public static class StiCloudReport
    {
        #region Fields.Static
        private static object lockObject = new object();
        #endregion

        #region Properties
        private static Hashtable rowsFetched;
        private static Hashtable RowsFetched
        {
            get
            {
                return rowsFetched ?? (rowsFetched = new Hashtable());
            }
        }
        #endregion

        #region Methods
        public static int GetMaxReportPages(string reportGuid)
        {
            var planIdent = StiCloudPlan.GetPlanIdent(reportGuid);

            if ((planIdent & StiCloudPlanIdent.DSingle) > 0)
                planIdent -= StiCloudPlanIdent.DSingle;

            if ((planIdent & StiCloudPlanIdent.DTeam) > 0)
                planIdent -= StiCloudPlanIdent.DTeam;

            if ((planIdent & StiCloudPlanIdent.DEnterprise) > 0)
                planIdent -= StiCloudPlanIdent.DEnterprise;

            return StiCloudPlan.GetPlan(planIdent).MaxReportPages;
        }

        public static int GetMaxDataRows(string reportGuid)
        {
            return StiCloudPlan.GetPlan(reportGuid).MaxDataRows;
        }

        public static int GetUsedDataRows(string reportGuid)
        {
            lock (lockObject)
            {
                var value = RowsFetched[reportGuid];
                return value is int ? (int)value : 0;
            }
        }

        public static int AddDataRows(string reportGuid, int rowsCount)
        {
            lock (lockObject)
            {
                var value = RowsFetched[reportGuid];
                var intValue = value is int ? (int)value : 0;
                intValue += rowsCount;

                RowsFetched[reportGuid] = intValue;
                return intValue;
            }
        }

        public static void ResetDataRows(string reportGuid)
        {
            lock (lockObject)
            {
                if (!String.IsNullOrEmpty(reportGuid))
                    RowsFetched[reportGuid] = 0;                
            }
        }
        #endregion
    }
}