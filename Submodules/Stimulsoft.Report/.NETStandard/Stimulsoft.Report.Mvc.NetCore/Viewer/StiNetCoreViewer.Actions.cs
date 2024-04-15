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

using Stimulsoft.System.Web.UI.WebControls;
using Stimulsoft.Report.Web;
using Stimulsoft.Report.Export;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Serialization;
using Stimulsoft.Base.Json.Converters;
using System.Collections;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using System;
using System.Threading.Tasks;
using System.Drawing;
using Stimulsoft.Base.Localization;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Stimulsoft.Report.Mvc
{
    public partial class StiNetCoreViewer : Panel
    {
        #region GetReportResult

        #region Async
        /// <summary>
        /// Get the action result required for show the specified report in the viewer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> GetReportResultAsync(Controller controller, StiReport report)
        {
            return await Task.Run(() => GetReportResult(controller, report));
        }

        /// <summary>
        /// Get the action result required for show the specified report in the viewer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> GetReportResultAsync(PageModel page, StiReport report)
        {
            return await Task.Run(() => GetReportResult(page, report));
        }

        /// <summary>
        /// Get the action result required for show the specified report in the viewer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> GetReportResultAsync(StiRequestParams requestParams, StiReport report)
        {
            return await Task.Run(() => GetReportResult(requestParams, report));
        }
        #endregion

        /// <summary>
        /// Get the action result required for show the specified report in the viewer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult GetReportResult(Controller controller, StiReport report)
        {
            var requestParams = GetRequestParams(controller);
            return GetReportResult(requestParams, report);
        }

        /// <summary>
        /// Get the action result required for show the specified report in the viewer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult GetReportResult(PageModel page, StiReport report)
        {
            var requestParams = GetRequestParams(page);
            return GetReportResult(requestParams, report);
        }

        /// <summary>
        /// Get the action result required for show the specified report in the viewer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult GetReportResult(StiRequestParams requestParams, StiReport report)
        {
            report = report ?? GetReportObject(requestParams);
            if (report != null) report.Key = StiKeyHelper.GenerateKey();
            StiReportHelper.ApplyQueryParameters(requestParams, report);

            StiWebActionResult result;
            try
            {
                requestParams.Cache.Helper.SaveReportInternal(requestParams, report);
                result = StiReportHelper.ViewerResult(requestParams, report);
            }
            catch (StiDashboardNotSupportedException e)
            {
                Console.Write(StiWebActionResult.GetErrorMessageText(e));
                result = StiWebActionResult.DashboardNotSupportedResult(requestParams);
            }
            catch (Exception e)
            {
                result = StiWebActionResult.ErrorResult(requestParams, StiWebActionResult.GetErrorMessageText(e));
            }

            return StiNetCoreActionResult.FromWebActionResult(result);
        }
        #endregion

        #region PrintReportResult

        #region Async
        /// <summary>
        /// Get the action result required for print the report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        public static async Task<IActionResult> PrintReportResultAsync(Controller controller)
        {
            return await Task.Run(() => PrintReportResult(controller));
        }

        /// <summary>
        /// Get the action result required for print the report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        public static async Task<IActionResult> PrintReportResultAsync(PageModel page)
        {
            return await Task.Run(() => PrintReportResult(page));
        }

        /// <summary>
        /// Get the action result required for print the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        public static async Task<IActionResult> PrintReportResultAsync(StiRequestParams requestParams)
        {
            return await Task.Run(() => PrintReportResult(requestParams));
        }

        /// <summary>
        /// Get the action result required for print the report. The specified report will be used.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> PrintReportResultAsync(Controller controller, StiReport report)
        {
            return await Task.Run(() => PrintReportResult(controller, report));
        }

        /// <summary>
        /// Get the action result required for print the report. The specified report will be used.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> PrintReportResultAsync(PageModel page, StiReport report)
        {
            return await Task.Run(() => PrintReportResult(page, report));
        }

        /// <summary>
        /// Get the action result required for print the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> PrintReportResultAsync(StiRequestParams requestParams, StiReport report)
        {
            return await Task.Run(() => PrintReportResult(requestParams, report));
        }

        /// <summary>
        /// Get the action result required for print the report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="settings">Export settings for the selected print format, can be PDF or HTML.</param>
        public static async Task<IActionResult> PrintReportResultAsync(Controller controller, StiExportSettings settings)
        {
            return await Task.Run(() => PrintReportResult(controller, settings));
        }


        /// <summary>
        /// Get the action result required for print the report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="settings">Export settings for the selected print format, can be PDF or HTML.</param>
        public static async Task<IActionResult> PrintReportResultAsync(PageModel page, StiExportSettings settings)
        {
            return await Task.Run(() => PrintReportResult(page, settings));
        }

        /// <summary>
        /// Get the action result required for print the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="settings">Export settings for the selected print format, can be PDF or HTML.</param>
        public static async Task<IActionResult> PrintReportResultAsync(StiRequestParams requestParams, StiExportSettings settings)
        {
            return await Task.Run(() => PrintReportResult(requestParams, settings));
        }

        /// <summary>
        /// Get the action result required for print the report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="settings">Export settings for the selected print format, can be PDF or HTML.</param>
        public static async Task<IActionResult> PrintReportResultAsync(Controller controller, StiReport report, StiExportSettings settings)
        {
            return await Task.Run(() => PrintReportResult(controller, report, settings));
        }

        /// <summary>
        /// Get the action result required for print the report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="settings">Export settings for the selected print format, can be PDF or HTML.</param>
        public static async Task<IActionResult> PrintReportResultAsync(PageModel page, StiReport report, StiExportSettings settings)
        {
            return await Task.Run(() => PrintReportResult(page, report, settings));
        }

        /// <summary>
        /// Get the action result required for print the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="settings">Export settings for the selected print format, can be PDF or HTML.</param>
        public static async Task<IActionResult> PrintReportResultAsync(StiRequestParams requestParams, StiReport report, StiExportSettings settings)
        {
            return await Task.Run(() => PrintReportResult(requestParams, report, settings));
        }
        #endregion

        /// <summary>
        /// Get the action result required for print the report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        public static ActionResult PrintReportResult(Controller controller)
        {
            var requestParams = GetRequestParams(controller);
            return PrintReportResult(requestParams, null, null);
        }

        /// <summary>
        /// Get the action result required for print the report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        public static ActionResult PrintReportResult(PageModel page)
        {
            var requestParams = GetRequestParams(page);
            return PrintReportResult(requestParams, null, null);
        }

        /// <summary>
        /// Get the action result required for print the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        public static ActionResult PrintReportResult(StiRequestParams requestParams)
        {
            return PrintReportResult(requestParams, null, null);
        }

        /// <summary>
        /// Get the action result required for print the report. The specified report will be used.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult PrintReportResult(Controller controller, StiReport report)
        {
            var requestParams = GetRequestParams(controller);
            return PrintReportResult(requestParams, report, null);
        }

        /// <summary>
        /// Get the action result required for print the report. The specified report will be used.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult PrintReportResult(PageModel page, StiReport report)
        {
            var requestParams = GetRequestParams(page);
            return PrintReportResult(requestParams, report, null);
        }

        /// <summary>
        /// Get the action result required for print the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult PrintReportResult(StiRequestParams requestParams, StiReport report)
        {
            return PrintReportResult(requestParams, report, null);
        }

        /// <summary>
        /// Get the action result required for print the report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="settings">Export settings for the selected print format, can be PDF or HTML.</param>
        public static ActionResult PrintReportResult(Controller controller, StiExportSettings settings)
        {
            var requestParams = GetRequestParams(controller);
            return PrintReportResult(requestParams, null, settings);
        }

        /// <summary>
        /// Get the action result required for print the report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="settings">Export settings for the selected print format, can be PDF or HTML.</param>
        public static ActionResult PrintReportResult(PageModel page, StiExportSettings settings)
        {
            var requestParams = GetRequestParams(page);
            return PrintReportResult(requestParams, null, settings);
        }

        /// <summary>
        /// Get the action result required for print the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="settings">Export settings for the selected print format, can be PDF or HTML.</param>
        public static ActionResult PrintReportResult(StiRequestParams requestParams, StiExportSettings settings)
        {
            return PrintReportResult(requestParams, null, settings);
        }

        /// <summary>
        /// Get the action result required for print the report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="settings">Export settings for the selected print format, can be PDF or HTML.</param>
        public static ActionResult PrintReportResult(Controller controller, StiReport report, StiExportSettings settings)
        {
            var requestParams = GetRequestParams(controller);
            return PrintReportResult(requestParams, report, settings);
        }

        /// <summary>
        /// Get the action result required for print the report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="settings">Export settings for the selected print format, can be PDF or HTML.</param>
        public static ActionResult PrintReportResult(PageModel page, StiReport report, StiExportSettings settings)
        {
            var requestParams = GetRequestParams(page);
            return PrintReportResult(requestParams, report, settings);
        }

        /// <summary>
        /// Get the action result required for print the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="settings">Export settings for the selected print format, can be PDF or HTML.</param>
        public static ActionResult PrintReportResult(StiRequestParams requestParams, StiReport report, StiExportSettings settings)
        {
            report = report ?? GetReportObject(requestParams);
            settings = settings ?? GetExportSettings(requestParams);
            var result = StiExportsHelper.PrintReportResult(requestParams, report, settings);
            return StiNetCoreActionResult.FromWebActionResult(result);
        }
        #endregion

        #region ExportReportResult

        #region Async
        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        public static async Task<IActionResult> ExportReportResultAsync(Controller controller)
        {
            return await Task.Run(() => ExportReportResult(controller));
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        public static async Task<IActionResult> ExportReportResultAsync(PageModel page)
        {
            return await Task.Run(() => ExportReportResult(page));
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        public static async Task<IActionResult> ExportReportResultAsync(StiRequestParams requestParams)
        {
            return await Task.Run(() => ExportReportResult(requestParams));
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> ExportReportResultAsync(Controller controller, StiReport report)
        {
            return await Task.Run(() => ExportReportResult(controller, report));
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> ExportReportResultAsync(PageModel page, StiReport report)
        {
            return await Task.Run(() => ExportReportResult(page, report));
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> ExportReportResultAsync(StiRequestParams requestParams, StiReport report)
        {
            return await Task.Run(() => ExportReportResult(requestParams, report));
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static async Task<IActionResult> ExportReportResultAsync(Controller controller, StiExportSettings settings)
        {
            return await Task.Run(() => ExportReportResult(controller, settings));
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static async Task<IActionResult> ExportReportResultAsync(PageModel page, StiExportSettings settings)
        {
            return await Task.Run(() => ExportReportResult(page, settings));
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static async Task<IActionResult> ExportReportResultAsync(StiRequestParams requestParams, StiExportSettings settings)
        {
            return await Task.Run(() => ExportReportResult(requestParams, settings));
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static async Task<IActionResult> ExportReportResultAsync(Controller controller, StiReport report, StiExportSettings settings)
        {
            return await Task.Run(() => ExportReportResult(controller, report, settings));
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static async Task<IActionResult> ExportReportResultAsync(PageModel page, StiReport report, StiExportSettings settings)
        {
            return await Task.Run(() => ExportReportResult(page, report, settings));
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static async Task<IActionResult> ExportReportResultAsync(StiRequestParams requestParams, StiReport report, StiExportSettings settings)
        {
            return await Task.Run(() => ExportReportResult(requestParams, report, settings));
        }
        #endregion

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        public static ActionResult ExportReportResult(Controller controller)
        {
            var requestParams = GetRequestParams(controller);
            return ExportReportResult(requestParams, null, null);
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        public static ActionResult ExportReportResult(PageModel page)
        {
            var requestParams = GetRequestParams(page);
            return ExportReportResult(requestParams, null, null);
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        public static ActionResult ExportReportResult(StiRequestParams requestParams)
        {
            return ExportReportResult(requestParams, null, null);
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult ExportReportResult(Controller controller, StiReport report)
        {
            var requestParams = GetRequestParams(controller);
            return ExportReportResult(requestParams, report, null);
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult ExportReportResult(PageModel page, StiReport report)
        {
            var requestParams = GetRequestParams(page);
            return ExportReportResult(requestParams, report, null);
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult ExportReportResult(StiRequestParams requestParams, StiReport report)
        {
            return ExportReportResult(requestParams, report, null);
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static ActionResult ExportReportResult(Controller controller, StiExportSettings settings)
        {
            var requestParams = GetRequestParams(controller);
            return ExportReportResult(requestParams, null, settings);
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static ActionResult ExportReportResult(PageModel page, StiExportSettings settings)
        {
            var requestParams = GetRequestParams(page);
            return ExportReportResult(requestParams, null, settings);
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static ActionResult ExportReportResult(StiRequestParams requestParams, StiExportSettings settings)
        {
            return ExportReportResult(requestParams, null, settings);
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static ActionResult ExportReportResult(Controller controller, StiReport report, StiExportSettings settings)
        {
            var requestParams = GetRequestParams(controller);
            return ExportReportResult(requestParams, report, settings);
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static ActionResult ExportReportResult(PageModel page, StiReport report, StiExportSettings settings)
        {
            var requestParams = GetRequestParams(page);
            return ExportReportResult(requestParams, report, settings);
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static ActionResult ExportReportResult(StiRequestParams requestParams, StiReport report, StiExportSettings settings)
        {
            StiWebActionResult result = null;
            report = report ?? GetReportObject(requestParams);
            if (requestParams.Action == StiAction.ExportDashboard)
            {
                var dashboardSettings = GetDashboardExportSettings(requestParams);
                result = StiExportsHelper.ExportDashboardResult(requestParams, report, dashboardSettings);
            }
            else
            {
                settings = settings ?? GetExportSettings(requestParams);
                        result = StiExportsHelper.ExportReportResult(requestParams, report, settings);
            }
            return StiNetCoreActionResult.FromWebActionResult(result);
        }
        #endregion

        #region EmailReportResult

        #region Async
        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        public static async Task<IActionResult> EmailReportResultAsync(Controller controller, StiEmailOptions options)
        {
            return await Task.Run(() => EmailReportResult(controller, options));
        }

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        public static async Task<IActionResult> EmailReportResultAsync(PageModel page, StiEmailOptions options)
        {
            return await Task.Run(() => EmailReportResult(page, options));
        }

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        public static async Task<IActionResult> EmailReportResultAsync(StiRequestParams requestParams, StiEmailOptions options)
        {
            return await Task.Run(() => EmailReportResult(requestParams, options));
        }

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static async Task<IActionResult> EmailReportResultAsync(Controller controller, StiEmailOptions options, StiExportSettings settings)
        {
            return await Task.Run(() => EmailReportResult(controller, options, settings));
        }

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static async Task<IActionResult> EmailReportResultAsync(PageModel page, StiEmailOptions options, StiExportSettings settings)
        {
            return await Task.Run(() => EmailReportResult(page, options, settings));
        }

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static async Task<IActionResult> EmailReportResultAsync(StiRequestParams requestParams, StiEmailOptions options, StiExportSettings settings)
        {
            return await Task.Run(() => EmailReportResult(requestParams, options, settings));
        }

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        public static async Task<IActionResult> EmailReportResultAsync(Controller controller, StiReport report, StiEmailOptions options)
        {
            return await Task.Run(() => EmailReportResult(controller, report, options));
        }

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        public static async Task<IActionResult> EmailReportResultAsync(PageModel page, StiReport report, StiEmailOptions options)
        {
            return await Task.Run(() => EmailReportResult(page, report, options));
        }

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        public static async Task<IActionResult> EmailReportResultAsync(StiRequestParams requestParams, StiReport report, StiEmailOptions options)
        {
            return await Task.Run(() => EmailReportResult(requestParams, report, options));
        }

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static async Task<IActionResult> EmailReportResultAsync(Controller controller, StiReport report, StiEmailOptions options, StiExportSettings settings)
        {
            return await Task.Run(() => EmailReportResult(controller, report, options, settings));
        }

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static async Task<IActionResult> EmailReportResultAsync(PageModel page, StiReport report, StiEmailOptions options, StiExportSettings settings)
        {
            return await Task.Run(() => EmailReportResult(page, report, options, settings));
        }

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static async Task<IActionResult> EmailReportResultAsync(StiRequestParams requestParams, StiReport report, StiEmailOptions options, StiExportSettings settings)
        {
            return await Task.Run(() => EmailReportResult(requestParams, report, options, settings));
        }
        #endregion

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        public static ActionResult EmailReportResult(Controller controller, StiEmailOptions options)
        {
            var requestParams = GetRequestParams(controller);
            return EmailReportResult(requestParams, null, options, null);
        }

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        public static ActionResult EmailReportResult(PageModel page, StiEmailOptions options)
        {
            var requestParams = GetRequestParams(page);
            return EmailReportResult(requestParams, null, options, null);
        }

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        public static ActionResult EmailReportResult(StiRequestParams requestParams, StiEmailOptions options)
        {
            return EmailReportResult(requestParams, null, options, null);
        }

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static ActionResult EmailReportResult(Controller controller, StiEmailOptions options, StiExportSettings settings)
        {
            var requestParams = GetRequestParams(controller);
            return EmailReportResult(requestParams, null, options, settings);
        }

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static ActionResult EmailReportResult(PageModel page, StiEmailOptions options, StiExportSettings settings)
        {
            var requestParams = GetRequestParams(page);
            return EmailReportResult(requestParams, null, options, settings);
        }

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static ActionResult EmailReportResult(StiRequestParams requestParams, StiEmailOptions options, StiExportSettings settings)
        {
            return EmailReportResult(requestParams, null, options, settings);
        }

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        public static ActionResult EmailReportResult(Controller controller, StiReport report, StiEmailOptions options)
        {
            var requestParams = GetRequestParams(controller);
            return EmailReportResult(requestParams, report, options, null);
        }

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        public static ActionResult EmailReportResult(PageModel page, StiReport report, StiEmailOptions options)
        {
            var requestParams = GetRequestParams(page);
            return EmailReportResult(requestParams, report, options, null);
        }

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        public static ActionResult EmailReportResult(StiRequestParams requestParams, StiReport report, StiEmailOptions options)
        {
            return EmailReportResult(requestParams, report, options, null);
        }

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static ActionResult EmailReportResult(Controller controller, StiReport report, StiEmailOptions options, StiExportSettings settings)
        {
            var requestParams = GetRequestParams(controller);
            return EmailReportResult(requestParams, report, options, settings);
        }

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static ActionResult EmailReportResult(PageModel page, StiReport report, StiEmailOptions options, StiExportSettings settings)
        {
            var requestParams = GetRequestParams(page);
            return EmailReportResult(requestParams, report, options, settings);
        }

        /// <summary>
        /// Get the action result required for send the report by Email.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="options">Options to send an Email that contains all the necessary parameters, including account details.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static ActionResult EmailReportResult(StiRequestParams requestParams, StiReport report, StiEmailOptions options, StiExportSettings settings)
        {
            report = report ?? GetReportObject(requestParams);
            settings = settings ?? GetExportSettings(requestParams);
            var result = StiExportsHelper.EmailReportResult(requestParams, report, settings, options);
            return StiNetCoreActionResult.FromWebActionResult(result);
        }
        #endregion

        #region InteractionResult

        #region Async
        /// <summary>
        /// Get the action result required for interactive operations with a report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        public static async Task<IActionResult> InteractionResultAsync(Controller controller)
        {
            return await Task.Run(() => InteractionResult(controller));
        }

        /// <summary>
        /// Get the action result required for interactive operations with a report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        public static async Task<IActionResult> InteractionResultAsync(PageModel page)
        {
            return await Task.Run(() => InteractionResult(page));
        }

        /// <summary>
        /// Get the action result required for interactive operations with a report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        public static async Task<IActionResult> InteractionResultAsync(StiRequestParams requestParams)
        {
            return await Task.Run(() => InteractionResult(requestParams));
        }

        /// <summary>
        /// Get the action result required for interactive operations with a report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> InteractionResultAsync(Controller controller, StiReport report)
        {
            return await Task.Run(() => InteractionResult(controller, report));
        }

        /// <summary>
        /// Get the action result required for interactive operations with a report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> InteractionResultAsync(PageModel page, StiReport report)
        {
            return await Task.Run(() => InteractionResult(page, report));
        }

        /// <summary>
        /// Get the action result required for interactive operations with a report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> InteractionResultAsync(StiRequestParams requestParams, StiReport report)
        {
            return await Task.Run(() => InteractionResult(requestParams, report));
        }
        #endregion

        /// <summary>
        /// Get the action result required for interactive operations with a report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        public static ActionResult InteractionResult(Controller controller)
        {
            var requestParams = GetRequestParams(controller);
            return InteractionResult(requestParams, null);
        }

        /// <summary>
        /// Get the action result required for interactive operations with a report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        public static ActionResult InteractionResult(PageModel page)
        {
            var requestParams = GetRequestParams(page);
            return InteractionResult(requestParams, null);
        }

        /// <summary>
        /// Get the action result required for interactive operations with a report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        public static ActionResult InteractionResult(StiRequestParams requestParams)
        {
            return InteractionResult(requestParams, null);
        }

        /// <summary>
        /// Get the action result required for interactive operations with a report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult InteractionResult(Controller controller, StiReport report)
        {
            var requestParams = GetRequestParams(controller);
            return InteractionResult(requestParams, report);
        }

        /// <summary>
        /// Get the action result required for interactive operations with a report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult InteractionResult(PageModel page, StiReport report)
        {
            var requestParams = GetRequestParams(page);
            return InteractionResult(requestParams, report);
        }

        /// <summary>
        /// Get the action result required for interactive operations with a report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult InteractionResult(StiRequestParams requestParams, StiReport report)
        {
            report = report ?? GetReportObject(requestParams);
            var result = StiReportHelper.InteractionResult(requestParams, report);
            return StiNetCoreActionResult.FromWebActionResult(result);
        }
        #endregion

        #region ViewerEventResult

        #region Async
        /// <summary>
        /// Get the action result required for the service requests of the viewer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        public static async Task<IActionResult> ViewerEventResultAsync(Controller controller)
        {
            return await Task.Run(() => ViewerEventResult(controller));
        }

        /// <summary>
        /// Get the action result required for the service requests of the viewer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        public static async Task<IActionResult> ViewerEventResultAsync(PageModel page)
        {
            return await Task.Run(() => ViewerEventResult(page));
        }

        /// <summary>
        /// Get the action result required for the service requests of the viewer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        public static async Task<IActionResult> ViewerEventResultAsync(StiRequestParams requestParams)
        {
            return await Task.Run(() => ViewerEventResult(requestParams));
        }

        /// <summary>
        /// Get the action result required for the service requests of the viewer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> ViewerEventResultAsync(Controller controller, StiReport report)
        {
            return await Task.Run(() => ViewerEventResult(controller, report));
        }

        /// <summary>
        /// Get the action result required for the service requests of the viewer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> ViewerEventResultAsync(PageModel page, StiReport report)
        {
            return await Task.Run(() => ViewerEventResult(page, report));
        }

        /// <summary>
        /// Get the action result required for the service requests of the viewer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> ViewerEventResultAsync(StiRequestParams requestParams, StiReport report)
        {
            return await Task.Run(() => ViewerEventResult(requestParams, report));
        }
        #endregion

        /// <summary>
        /// Get the action result required for the service requests of the viewer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        public static ActionResult ViewerEventResult(Controller controller)
        {
            var requestParams = GetRequestParams(controller);
            return ViewerEventResult(requestParams, null);
        }

        /// <summary>
        /// Get the action result required for the service requests of the viewer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        public static ActionResult ViewerEventResult(PageModel page)
        {
            var requestParams = GetRequestParams(page);
            return ViewerEventResult(requestParams, null);
        }

        /// <summary>
        /// Get the action result required for the service requests of the viewer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        public static ActionResult ViewerEventResult(StiRequestParams requestParams)
        {
            return ViewerEventResult(requestParams, null);
        }

        /// <summary>
        /// Get the action result required for the service requests of the viewer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult ViewerEventResult(Controller controller, StiReport report)
        {
            var requestParams = GetRequestParams(controller);
            return ViewerEventResult(requestParams, report);
        }

        /// <summary>
        /// Get the action result required for the service requests of the viewer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult ViewerEventResult(PageModel page, StiReport report)
        {
            var requestParams = GetRequestParams(page);
            return ViewerEventResult(requestParams, report);
        }

        /// <summary>
        /// Get the action result required for the service requests of the viewer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult ViewerEventResult(StiRequestParams requestParams, StiReport report)
        {
            if (requestParams.ComponentType != StiComponentType.Viewer)
                return ProcessRequestResult(requestParams, report);

            StiWebActionResult result = null;
            switch (requestParams.Action)
            {
                case StiAction.Resource:
                    result = StiViewerResourcesHelper.Get(requestParams);
                    return new StiNetCoreActionResult(result.Data, result.ContentType, result.FileName, false, true);

                case StiAction.OpenReport:
                    result = StiReportHelper.OpenReportResult(requestParams);
                    return StiNetCoreActionResult.FromWebActionResult(result);

                case StiAction.UpdateCache:
                    requestParams.Cache.Helper.UpdateObjectCacheInternal(requestParams, null);
                    return StiNetCoreActionResult.EmptyResult();

                case StiAction.ReportResource:
                    if (report == null) report = GetReportObject(requestParams);
                    result = StiReportHelper.ReportResourceResult(requestParams, report);
                    return StiNetCoreActionResult.FromWebActionResult(result);

                default:
                    // Need for backward compatibility - previously, ViewerEvent handled all actions.
                    return ProcessRequestResult(requestParams, report);
            }
        }
        #endregion

        #region GetScriptsResult

        #region Async
        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the viewer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        public static async Task<IActionResult> GetScriptsResultAsync(Controller controller)
        {
            return await Task.Run(() => GetScriptsResult(controller));
        }

        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the viewer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        public static async Task<IActionResult> GetScriptsResultAsync(PageModel page)
        {
            return await Task.Run(() => GetScriptsResult(page));
        }

        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the viewer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        public static async Task<IActionResult> GetScriptsResultAsync(StiRequestParams requestParams)
        {
            return await Task.Run(() => GetScriptsResult(requestParams));
        }

        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the viewer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="options">A set of options that will be used for the viewer.</param>
        public static async Task<IActionResult> GetScriptsResultAsync(Controller controller, StiNetCoreViewerOptions options)
        {
            return await Task.Run(() => GetScriptsResult(controller, options));
        }

        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the viewer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="options">A set of options that will be used for the viewer.</param>
        public static async Task<IActionResult> GetScriptsResultAsync(PageModel page, StiNetCoreViewerOptions options)
        {
            return await Task.Run(() => GetScriptsResult(page, options));
        }

        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the viewer.
        /// </summary>
        /// <param name="options">A set of options that will be used for the viewer.</param>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        public static async Task<IActionResult> GetScriptsResultAsync(StiRequestParams requestParams, StiNetCoreViewerOptions options)
        {
            return await Task.Run(() => GetScriptsResult(requestParams, options));
        }
        #endregion

        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the viewer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        public static ActionResult GetScriptsResult(Controller controller)
        {
            var requestParams = GetRequestParams(controller);
            var options = new StiNetCoreViewerOptions();
            return GetScriptsResult(requestParams, options);
        }

        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the viewer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        public static ActionResult GetScriptsResult(PageModel page)
        {
            var requestParams = GetRequestParams(page);
            var options = new StiNetCoreViewerOptions();
            return GetScriptsResult(requestParams, options);
        }

        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the viewer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        public static ActionResult GetScriptsResult(StiRequestParams requestParams)
        {
            var options = new StiNetCoreViewerOptions();
            return GetScriptsResult(requestParams, options);
        }

        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the viewer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        /// <param name="options">A set of options that will be used for the viewer.</param>
        public static ActionResult GetScriptsResult(Controller controller, StiNetCoreViewerOptions options)
        {
            var requestParams = GetRequestParams(controller);
            return GetScriptsResult(requestParams, options);
        }

        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the viewer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        /// <param name="options">A set of options that will be used for the viewer.</param>
        public static ActionResult GetScriptsResult(PageModel page, StiNetCoreViewerOptions options)
        {
            var requestParams = GetRequestParams(page);
            return GetScriptsResult(requestParams, options);
        }

        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the viewer.
        /// </summary>
        /// <param name="options">A set of options that will be used for the viewer.</param>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="useAbsoluteUrl">Value that indicate using absolute url.</param>
        public static ActionResult GetScriptsResult(StiRequestParams requestParams, StiNetCoreViewerOptions options, bool useAbsoluteUrl = false)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            requestParams.ComponentType = StiComponentType.Viewer;
            requestParams.Action = StiAction.Resource;
            requestParams.Resource = "scripts";
            requestParams.Scripts.IncludeClient = true;
            requestParams.Localization = options.Localization;

            var scripts = StiViewerResourcesHelper.Get(requestParams);
            var jsScripts = Encoding.UTF8.GetString(scripts.Data);
            var jsOptions = JsonConvert.SerializeObject(options, Formatting.None,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = { new StringEnumConverter() }
                });

            var jsExportSettings = JsonConvert.SerializeObject(
                StiExportsHelper.GetDefaultExportSettings(options.Exports.DefaultSettings),
                Formatting.None, new StringEnumConverter());

            var parameters = new Hashtable();
            parameters["shortProductVersion"] = StiVersionHelper.AssemblyVersion;
            parameters["productVersion"] = StiVersionHelper.ProductVersion.Trim();
            parameters["frameworkType"] = StiNetCoreHelper.GetFrameworkVersion();
            parameters["requestUrl"] = !useAbsoluteUrl ? requestParams.HttpContext.Request.Url.LocalPath : requestParams.HttpContext.Request.Url.AbsoluteUri;
            parameters["requestAbsoluteUrl"] = requestParams.HttpContext.Request.Url.AbsoluteUri;
            parameters["stimulsoftFontContent"] = StiReportResourceHelper.GetStimulsoftFontBase64Data();
            parameters["isAngular"] = useAbsoluteUrl;

            var jsParameters = JsonConvert.SerializeObject(parameters, Formatting.None);

            var jsData =
                $"{jsScripts}\r\n" +
                $"Stimulsoft.Viewer.defaultOptions = {jsOptions};\r\n" +
                $"Stimulsoft.Viewer.defaultExportSettings = {jsExportSettings};\r\n" +
                $"Stimulsoft.Viewer.parameters = {jsParameters};";

            var data = Encoding.UTF8.GetBytes(jsData);
            return new StiNetCoreActionResult(data, scripts.ContentType, scripts.FileName, false, true);
        }
        #endregion

        #region ProcessRequestResult

        #region Async
        /// <summary>
        /// Get the action result required for the all requests of the viewer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        public static async Task<IActionResult> ProcessRequestResultAsync(Controller controller)
        {
            return await Task.Run(() => ProcessRequestResult(controller));
        }

        /// <summary>
        /// Get the action result required for the all requests of the viewer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        public static async Task<IActionResult> ProcessRequestResultAsync(PageModel page)
        {
            return await Task.Run(() => ProcessRequestResult(page));
        }

        /// <summary>
        /// Get the action result required for the all requests of the viewer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        public static async Task<IActionResult> ProcessRequestResultAsync(StiRequestParams requestParams)
        {
            return await Task.Run(() => ProcessRequestResult(requestParams));
        }

        /// <summary>
        /// Get the action result required for the all requests of the viewer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> ProcessRequestResultAsync(StiRequestParams requestParams, StiReport report)
        {
            return await Task.Run(() => ProcessRequestResult(requestParams, report));
        }
        #endregion

        /// <summary>
        /// Get the action result required for the all requests of the viewer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the viewer.</param>
        public static ActionResult ProcessRequestResult(Controller controller)
        {
            var requestParams = GetRequestParams(controller);
            return ProcessRequestResult(requestParams, null);
        }

        /// <summary>
        /// Get the action result required for the all requests of the viewer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the viewer.</param>
        public static ActionResult ProcessRequestResult(PageModel page)
        {
            var requestParams = GetRequestParams(page);
            return ProcessRequestResult(requestParams, null);
        }

        /// <summary>
        /// Get the action result required for the all requests of the viewer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        public static ActionResult ProcessRequestResult(StiRequestParams requestParams)
        {
            return ProcessRequestResult(requestParams, null);
        }

        /// <summary>
        /// Get the action result required for the all requests of the viewer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult ProcessRequestResult(StiRequestParams requestParams, StiReport report)
        {
            if (requestParams.Action == StiAction.Undefined && requestParams.HttpMethod == "GET")
                return GetScriptsResult(requestParams);

            if (requestParams.ComponentType == StiComponentType.Viewer && requestParams.Action != StiAction.Undefined)
            {
                switch (requestParams.Action)
                {
                    case StiAction.Resource:
                    case StiAction.OpenReport:
                    case StiAction.UpdateCache:
                    case StiAction.ReportResource:
                        return ViewerEventResult(requestParams, report);

                    case StiAction.GetReport:
                        return GetReportResult(requestParams, report);

                    case StiAction.PrintReport:
                        return PrintReportResult(requestParams, report);

                    case StiAction.ExportReport:
                    case StiAction.ExportDashboard:
                        return ExportReportResult(requestParams, report);

                    case StiAction.RefreshReport:
                        if (requestParams.Viewer.ReportType == StiReportType.Dashboard) StiCacheCleaner.Clean(report);
                        return InteractionResult(requestParams, report);

                    case StiAction.InitVars:
                    case StiAction.Variables:
                    case StiAction.Sorting:
                    case StiAction.DrillDown:
                    case StiAction.Collapsing:
                    case StiAction.DashboardGettingFilterItems:                    
                    case StiAction.DashboardFiltering:
                    case StiAction.DashboardResetAllFilters:
                    case StiAction.Signatures:
                    case StiAction.DashboardSorting:
                    case StiAction.DashboardDrillDown:
                    case StiAction.DashboardElementDrillDown:
                    case StiAction.DashboardElementDrillUp:
                    case StiAction.DashboardButtonElementApplyEvents:
                        return InteractionResult(requestParams, report);

                    default:
                        if (report == null) report = GetReportObject(requestParams);
                        var result = StiReportHelper.ViewerResult(requestParams, report);
                        return StiNetCoreActionResult.FromWebActionResult(result);
                }
            }

            return StiNetCoreActionResult.EmptyResult();
        }
        #endregion
    }
}
