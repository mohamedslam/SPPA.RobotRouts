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

using Stimulsoft.Report.Export;
using System;
using System.Collections;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Stimulsoft.Report.Web
{
    public partial class StiWebDesigner :
        WebControl,
        INamingContainer
    {
        #region Request Params

        /// <summary>
        /// Get the all request parameters of the report designer.
        /// </summary>
        public static StiRequestParams GetRequestParams()
        {
            StiRequestParams requestParams = StiRequestParamsHelper.Get();
            requestParams.Cache.Helper = CacheHelper;
            return requestParams;
        }

        #endregion

        #region Report

        /// <summary>
        /// Get the current report template for the current action of the component.
        /// Returns report template from the client for OpenReport action, new report template for CreateReport action, copy of the report template for PreviewReport action.
        /// Otherwise returns report template from the cache.
        /// </summary>
        public static StiReport GetActionReportObject()
        {
            StiRequestParams requestParams = GetRequestParams();
            return GetActionReportObject(requestParams);
        }

        /// <summary>
        /// Get the current report template for the current action of the component.
        /// Returns report template from the client for OpenReport action, new report template for CreateReport action, copy of the report template for PreviewReport action.
        /// Otherwise returns report template from the cache. The specified request parameters will be used.
        /// </summary>
        public static StiReport GetActionReportObject(StiRequestParams requestParams)
        {
            if (requestParams.Action == StiAction.OpenReport)
            {
                try
                {
                    // The try/catch block is needed for opening incorrect report file or wrong report password
                    return LoadReportFromContent(requestParams);
                }
                catch
                {
                    return null;
                }
            }
            StiReport currentReport = GetReportObject(requestParams);
            if (requestParams.Action == StiAction.CreateReport)
            {
                StiReport newReport = GetNewReport(requestParams);

                if (currentReport != null && requestParams.Designer.NewReportDictionary == StiNewReportDictionary.DictionaryMerge)
                    StiReportCopier.CopyReportDictionary(currentReport, newReport);

                return newReport;
            }
            if (requestParams.Action == StiAction.PreviewReport) return StiReportCopier.CloneReport(currentReport);
            return currentReport;
        }

        /// <summary>
        /// Get the current report template from the cache.
        /// </summary>
        public static StiReport GetReportObject()
        {
            StiRequestParams requestParams = GetRequestParams();
            return GetReportObject(requestParams);
        }

        /// <summary>
        /// Get the current report template from the cache. The specified request parameters will be used.
        /// </summary>
        public static StiReport GetReportObject(StiRequestParams requestParams)
        {
            return requestParams.Cache.Helper.GetReportInternal(requestParams);
        }

        #endregion

        #region Export Settings

        /// <summary>
        /// Get the export settings from the dialog form of the report preview.
        /// </summary>
        public static StiExportSettings GetExportSettings()
        {
            var requestParams = GetRequestParams();
            return GetExportSettings(requestParams);
        }

        /// <summary>
        /// Get the export settings from the dialog form of the report preview. The specified request parameters will be used.
        /// </summary>
        public static StiExportSettings GetExportSettings(StiRequestParams requestParams)
        {
            return StiExportsHelper.GetExportSettings(requestParams);
        }

        #endregion
    }
}
