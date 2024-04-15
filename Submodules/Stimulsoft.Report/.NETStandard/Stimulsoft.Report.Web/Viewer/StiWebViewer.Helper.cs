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

using System.Web.UI;
using System.Web.UI.WebControls;
using Stimulsoft.Report.Export;
using System.Collections.Specialized;
using Stimulsoft.Report.Dashboard.Export;
using System;
using System.Collections;
using Stimulsoft.Base.Json.Linq;

namespace Stimulsoft.Report.Web
{
    public partial class StiWebViewer :
        WebControl,
        INamingContainer
    {
        /// <summary>
        /// Get the all request parameters of the report viewer.
        /// </summary>
        public static StiRequestParams GetRequestParams()
        {
            StiRequestParams requestParams = StiRequestParamsHelper.Get();
            requestParams.Cache.Helper = CacheHelper;
            return requestParams;
        }

        /// <summary>
        /// Get the POST form values for the start page of the report viewer.
        /// </summary>
        public static NameValueCollection GetFormValues()
        {
            StiRequestParams requestParams = GetRequestParams();
            return GetFormValues(requestParams);
        }

        /// <summary>
        /// Get the POST form values for the start page of the report viewer. The specified request parameters will be used.
        /// </summary>
        public static NameValueCollection GetFormValues(StiRequestParams requestParams)
        {
            return requestParams.FormValues;
        }

        /// <summary>
        /// Get the report template or report snapshot from the cache.
        /// </summary>
        public static StiReport GetReportObject()
        {
            StiRequestParams requestParams = GetRequestParams();
            return GetReportObject(requestParams);
        }

        /// <summary>
        /// Get the report template or report snapshot from the cache. The specified request parameters will be used.
        /// </summary>
        public static StiReport GetReportObject(StiRequestParams requestParams)
        {
            return requestParams.Cache.Helper.GetReportInternal(requestParams);
        }

        /// <summary>
        /// Get the export settings from the dialog form of the report viewer.
        /// </summary>
        public static StiExportSettings GetExportSettings()
        {
            StiRequestParams requestParams = GetRequestParams();
            return GetExportSettings(requestParams);
        }

        /// <summary>
        /// Get the export settings from the dialog form of the report viewer. The specified request parameters will be used.
        /// </summary>
        public static StiExportSettings GetExportSettings(StiRequestParams requestParams)
        {
            return StiExportsHelper.GetExportSettings(requestParams);
        }

        /// <summary>
        /// Get the export settings from the dialog form of the dashboard viewer.
        /// </summary>
        public static IStiDashboardExportSettings GetDashboardExportSettings()
        {
            StiRequestParams requestParams = GetRequestParams();
            return GetDashboardExportSettings(requestParams);
        }

        /// <summary>
        /// Get the export settings from the dialog form of the dashboard viewer. The specified request parameters will be used.
        /// </summary>
        public static IStiDashboardExportSettings GetDashboardExportSettings(StiRequestParams requestParams)
        {
            return StiExportsHelper.GetDashboardExportSettings(requestParams);
        }

        /// <summary>
        /// Get the Email options from the dialog, sent by the client.
        /// </summary>
        public static StiEmailOptions GetEmailOptions()
        {
            StiRequestParams requestParams = GetRequestParams();
            return GetEmailOptions(requestParams);
        }

        /// <summary>
        /// Get the Email options from the dialog, sent by the client. The specified request parameters will be used.
        /// </summary>
        public static StiEmailOptions GetEmailOptions(StiRequestParams requestParams)
        {
            var options = new StiEmailOptions();

            if (requestParams.ExportSettings.Contains("Email")) options.AddressTo = (string)requestParams.ExportSettings["Email"];
            if (requestParams.ExportSettings.Contains("Subject")) options.Subject = (string)requestParams.ExportSettings["Subject"];
            if (requestParams.ExportSettings.Contains("Message")) options.Body = (string)requestParams.ExportSettings["Message"];
            
            if (requestParams.ExportSettings.Contains("EmailCc"))
                options.CC = new ArrayList(((string)requestParams.ExportSettings["EmailCc"]).Split(new string[] { ",", ";" }, StringSplitOptions.None));

            if (requestParams.ExportSettings.Contains("EmailBcc"))
                options.BCC = new ArrayList(((string)requestParams.ExportSettings["EmailBcc"]).Split(new string[] { ",", ";" }, StringSplitOptions.None));

            return options;
        }
    }
}
