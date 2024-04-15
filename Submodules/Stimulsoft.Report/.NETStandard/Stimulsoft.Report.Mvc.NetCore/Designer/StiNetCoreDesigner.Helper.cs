﻿#region Copyright (C) 2003-2022 Stimulsoft
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

using Stimulsoft.System.Web.UI.WebControls;
using Stimulsoft.Report.Web;
using System.Collections.Specialized;
using Stimulsoft.Report.Export;
using Stimulsoft.System.Web;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Stimulsoft.Report.Mvc
{
    public partial class StiNetCoreDesigner : Panel
    {
        #region Request Params
        /// <summary>
        /// Get the all request parameters of the report designer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        public static StiRequestParams GetRequestParams(Controller controller)
        {
            return GetRequestParams(controller.HttpContext);
        }

        /// <summary>
        /// Get the all request parameters of the report designer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        public static StiRequestParams GetRequestParams(PageModel page)
        {
            return GetRequestParams(page.HttpContext);
        }

        /// <summary>
        /// Get the all request parameters of the report designer.
        /// </summary>
        /// <param name="httpContext">The current HttpContext that processes the actions of the designer.</param>
        public static StiRequestParams GetRequestParams(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            var httpContextBase = new HttpContext(httpContext);
            var requestParams = StiRequestParamsHelper.Get(httpContextBase);
            requestParams.Cache.Helper = CacheHelper;
            return requestParams;
        }
        #endregion

        #region Form and Route Values
        /// <summary>
        /// Get the POST form values for the start page of the report designer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        public static NameValueCollection GetFormValues(Controller controller)
        {
            var requestParams = GetRequestParams(controller);
            return GetFormValues(requestParams);
        }

        /// <summary>
        /// Get the POST form values for the start page of the report designer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        public static NameValueCollection GetFormValues(PageModel page)
        {
            var requestParams = GetRequestParams(page);
            return GetFormValues(requestParams);
        }

        /// <summary>
        /// Get the POST form values for the start page of the report designer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        public static NameValueCollection GetFormValues(StiRequestParams requestParams)
        {
            return requestParams.FormValues;
        }

        /// <summary>
        /// Get the route values for the start page of the report designer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        public static RouteValueDictionary GetRouteValues(Controller controller)
        {
            var requestParams = GetRequestParams(controller);
            return GetRouteValues(requestParams);
        }

        /// <summary>
        /// Get the route values for the start page of the report designer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        public static RouteValueDictionary GetRouteValues(PageModel page)
        {
            var requestParams = GetRequestParams(page);
            return GetRouteValues(requestParams);
        }

        /// <summary>
        /// Get the route values for the start page of the report designer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        public static RouteValueDictionary GetRouteValues(StiRequestParams requestParams)
        {
            var routeValues = new RouteValueDictionary();
            if (requestParams.Routes != null)
            {
                foreach (string key in requestParams.Routes.Keys)
                {
                    routeValues.Add(key, requestParams.Routes[key]);
                }
            }

            return routeValues;
        }
        #endregion

        #region Report
        /// <summary>
        /// Get the current report template for the current action of the component.
        /// Returns report template from the client for OpenReport action, new report template for CreateReport action, copy of the report template for PreviewReport action.
        /// Otherwise returns report template from the cache.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        public static StiReport GetActionReportObject(Controller controller)
        {
            var requestParams = GetRequestParams(controller);
            return GetActionReportObject(requestParams);
        }

        /// <summary>
        /// Get the current report template for the current action of the component.
        /// Returns report template from the client for OpenReport action, new report template for CreateReport action, copy of the report template for PreviewReport action.
        /// Otherwise returns report template from the cache.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        public static StiReport GetActionReportObject(PageModel page)
        {
            var requestParams = GetRequestParams(page);
            return GetActionReportObject(requestParams);
        }

        /// <summary>
        /// Get the current report template for the current action of the component.
        /// Returns report template from the client for OpenReport action, new report template for CreateReport action, copy of the report template for PreviewReport action.
        /// Otherwise returns report template from the cache.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        public static StiReport GetActionReportObject(StiRequestParams requestParams)
        {
            if (requestParams.Action == StiAction.OpenReport)
            {
                try
                {
                    // The try/catch block is needed for opening incorrect report file or wrong report password
                    return StiWebDesigner.LoadReportFromContent(requestParams);
                }
                catch
                {
                    return null;
                }
            }
            var currentReport = GetReportObject(requestParams);
            if (requestParams.Action == StiAction.CreateReport)
            {
                var newReport = requestParams.GetBoolean("isDashboard") ? StiWebDesigner.GetNewDashboard(requestParams) : StiWebDesigner.GetNewReport(requestParams);
                if (currentReport != null && (requestParams.Designer.Command == StiDesignerCommand.WizardResult || requestParams.Designer.NewReportDictionary == StiNewReportDictionary.DictionaryMerge))
                    StiReportCopier.CopyReportDictionary(currentReport, newReport);
                return newReport;
            }
            if (requestParams.Action == StiAction.PreviewReport) return StiReportCopier.CloneReport(currentReport);
            return currentReport;
        }

        /// <summary>
        /// Get the current report template from the cache.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        public static StiReport GetReportObject(Controller controller)
        {
            var requestParams = GetRequestParams(controller);
            return GetReportObject(requestParams);
        }

        /// <summary>
        /// Get the current report template from the cache.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        public static StiReport GetReportObject(PageModel page)
        {
            var requestParams = GetRequestParams(page);
            return GetReportObject(requestParams);
        }

        /// <summary>
        /// Get the current report template from the cache. The specified request parameters will be used.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        public static StiReport GetReportObject(StiRequestParams requestParams)
        {
            return requestParams.Cache.Helper.GetReportInternal(requestParams);
        }
        #endregion

        #region Export Settings
        /// <summary>
        /// Get the export settings from the dialog form of the report preview.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        public static StiExportSettings GetExportSettings(Controller controller)
        {
            var requestParams = GetRequestParams(controller);
            return GetExportSettings(requestParams);
        }

        /// <summary>
        /// Get the export settings from the dialog form of the report preview.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        public static StiExportSettings GetExportSettings(PageModel page)
        {
            var requestParams = GetRequestParams(page);
            return GetExportSettings(requestParams);
        }

        /// <summary>
        /// Get the export settings from the dialog form of the report preview.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        public static StiExportSettings GetExportSettings(StiRequestParams requestParams)
        {
            return StiExportsHelper.GetExportSettings(requestParams);
        }
        #endregion
    }
}
