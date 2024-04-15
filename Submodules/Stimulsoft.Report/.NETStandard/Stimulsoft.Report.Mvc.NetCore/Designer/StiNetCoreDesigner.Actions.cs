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
using Stimulsoft.System.Web.UI.WebControls;
using Stimulsoft.Report.Web;
using Stimulsoft.Report.Export;
using Microsoft.AspNetCore.Mvc;
using Stimulsoft.Base.Serializing;
using System.Text;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;
using Stimulsoft.Base.Json.Serialization;
using System.Collections;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Base.Localization;
using System.Globalization;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Stimulsoft.Report.Mvc
{
    public partial class StiNetCoreDesigner : Panel
    {
        #region GetReportResult

        #region Async
        /// <summary>
        /// Get the action result required for edit a new report template in the designer or edit a report template opened using the file menu.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        public static async Task<IActionResult> GetReportResultAsync(Controller controller)
        {
            return await Task.Run(() => GetReportResult(controller));
        }

        /// <summary>
        /// Get the action result required for edit a new report template in the designer or edit a report template opened using the file menu.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        public static async Task<IActionResult> GetReportResultAsync(PageModel page)
        {
            return await Task.Run(() => GetReportResult(page));
        }

        /// <summary>
        /// Get the action result required for edit a new report template in the designer or edit a report template opened using the file menu.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        public static async Task<IActionResult> GetReportResultAsync(StiRequestParams requestParams)
        {
            return await Task.Run(() => GetReportResult(requestParams));
        }

        /// <summary>
        /// Get the action result required for edit the specified report template in the designer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> GetReportResultAsync(Controller controller, StiReport report)
        {
            return await Task.Run(() => GetReportResult(controller, report));
        }

        /// <summary>
        /// Get the action result required for edit the specified report template in the designer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> GetReportResultAsync(PageModel page, StiReport report)
        {
            return await Task.Run(() => GetReportResult(page, report));
        }

        /// <summary>
        /// Get the action result required for edit the specified report template in the designer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> GetReportResultAsync(StiRequestParams requestParams, StiReport report)
        {
            return await Task.Run(() => GetReportResult(requestParams, report));
        }
        #endregion

        /// <summary>
        /// Get the action result required for edit a new report template in the designer or edit a report template opened using the file menu.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        public static ActionResult GetReportResult(Controller controller)
        {
            var requestParams = GetRequestParams(controller);
            return GetReportResult(requestParams, null);
        }

        /// <summary>
        /// Get the action result required for edit a new report template in the designer or edit a report template opened using the file menu.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        public static ActionResult GetReportResult(PageModel page)
        {
            var requestParams = GetRequestParams(page);
            return GetReportResult(requestParams, null);
        }

        /// <summary>
        /// Get the action result required for edit a new report template in the designer or edit a report template opened using the file menu.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        public static ActionResult GetReportResult(StiRequestParams requestParams)
        {
            return GetReportResult(requestParams, null);
        }

        /// <summary>
        /// Get the action result required for edit the specified report template in the designer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult GetReportResult(Controller controller, StiReport report)
        {
            var requestParams = GetRequestParams(controller);
            return GetReportResult(requestParams, report);
        }

        /// <summary>
        /// Get the action result required for edit the specified report template in the designer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult GetReportResult(PageModel page, StiReport report)
        {
            var requestParams = GetRequestParams(page);
            return GetReportResult(requestParams, report);
        }

        /// <summary>
        /// Get the action result required for edit the specified report template in the designer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult GetReportResult(StiRequestParams requestParams, StiReport report)
        {
            try
            {
                switch (requestParams.Action)
                {
                    case StiAction.CreateReport:
                        if (report == null)
                            report = requestParams.GetBoolean("isDashboard") ? StiWebDesigner.GetNewDashboard(requestParams) : StiWebDesigner.GetNewReport(requestParams);

                        var currentReport = GetReportObject(requestParams);
                        if (currentReport != null && (requestParams.Designer.Command == StiDesignerCommand.WizardResult || requestParams.Designer.NewReportDictionary == StiNewReportDictionary.DictionaryMerge))
                            StiReportCopier.CopyReportDictionary(currentReport, report);
                        break;

                    case StiAction.OpenReport:
                        if (report == null)
                            report = StiWebDesigner.LoadReportFromContent(requestParams);
                        break;

                    default:
                        report = StiWebDesigner.GetReportForDesigner(requestParams, report);
                        break;
                }

                var result = StiWebDesigner.CommandResult(requestParams, report);
                return StiNetCoreActionResult.FromWebActionResult(result);
            }
            catch (StiDashboardNotSupportedException e)
            {
                Console.Write(e.Message);
                var result = StiWebActionResult.DashboardNotSupportedResult(requestParams);
                return StiNetCoreActionResult.FromWebActionResult(result);
            }
            catch (Exception e)
            {
                var result = StiWebActionResult.ErrorResult(requestParams, e.Message);
                return StiNetCoreActionResult.FromWebActionResult(result);
            }
        }
        #endregion

        #region SaveReportResult

        #region Async
        /// <summary>
        /// Get the action result required for saving the report template.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        public static async Task<IActionResult> SaveReportResultAsync(Controller controller)
        {
            return await Task.Run(() => SaveReportResult(controller));
        }

        /// <summary>
        /// Get the action result required for saving the report template.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        public static async Task<IActionResult> SaveReportResultAsync(PageModel page)
        {
            return await Task.Run(() => SaveReportResult(page));
        }

        /// <summary>
        /// Get the action result required for saving the report template.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        public static async Task<IActionResult> SaveReportResultAsync(StiRequestParams requestParams)
        {
            return await Task.Run(() => SaveReportResult(requestParams));
        }

        /// <summary>
        /// Get the action result required for saving the report template.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        /// <param name="errorMessage">Save error message to be displayed in the designer.</param>
        public static async Task<IActionResult> SaveReportResultAsync(Controller controller, string errorMessage)
        {
            return await Task.Run(() => SaveReportResult(controller, errorMessage));
        }


        /// <summary>
        /// Get the action result required for saving the report template.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        /// <param name="errorMessage">Save error message to be displayed in the designer.</param>
        public static async Task<IActionResult> SaveReportResultAsync(PageModel page, string errorMessage)
        {
            return await Task.Run(() => SaveReportResult(page, errorMessage));
        }

        /// <summary>
        /// Get the action result required for saving the report template.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        /// <param name="errorMessage">Save error message to be displayed in the designer.</param>
        public static async Task<IActionResult> SaveReportResultAsync(StiRequestParams requestParams, string errorMessage)
        {
            return await Task.Run(() => SaveReportResult(requestParams, errorMessage));
        }

        /// <summary>
        /// Get the action result required for saving the report template.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        /// <param name="report">The report object to be sent back to the designer.</param>
        public static async Task<IActionResult> SaveReportResultAsync(Controller controller, StiReport report)
        {
            return await Task.Run(() => SaveReportResult(controller, report));
        }

        /// <summary>
        /// Get the action result required for saving the report template.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        /// <param name="report">The report object to be sent back to the designer.</param>
        public static async Task<IActionResult> SaveReportResultAsync(PageModel page, StiReport report)
        {
            return await Task.Run(() => SaveReportResult(page, report));
        }

        /// <summary>
        /// Get the action result required for saving the report template.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        /// <param name="report">The report object to be sent back to the designer.</param>
        public static async Task<IActionResult> SaveReportResultAsync(StiRequestParams requestParams, StiReport report)
        {
            return await Task.Run(() => SaveReportResult(requestParams, report));
        }
        #endregion

        /// <summary>
        /// Get the action result required for saving the report template.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        public static ActionResult SaveReportResult(Controller controller)
        {
            var requestParams = GetRequestParams(controller);
            var report = GetActionReportObject(requestParams);
            return SaveReportResult(requestParams, report);
        }

        /// <summary>
        /// Get the action result required for saving the report template.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        public static ActionResult SaveReportResult(PageModel page)
        {
            var requestParams = GetRequestParams(page);
            var report = GetActionReportObject(requestParams);
            return SaveReportResult(requestParams, report);
        }

        /// <summary>
        /// Get the action result required for saving the report template.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        public static ActionResult SaveReportResult(StiRequestParams requestParams)
        {
            return SaveReportResult(requestParams);
        }

        /// <summary>
        /// Get the action result required for saving the report template.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        /// <param name="errorMessage">Save error message to be displayed in the designer.</param>
        public static ActionResult SaveReportResult(Controller controller, string errorMessage)
        {
            var requestParams = GetRequestParams(controller);
            return SaveReportResult(requestParams, errorMessage);
        }

        /// <summary>
        /// Get the action result required for saving the report template.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        /// <param name="errorMessage">Save error message to be displayed in the designer.</param>
        public static ActionResult SaveReportResult(PageModel page, string errorMessage)
        {
            var requestParams = GetRequestParams(page);
            return SaveReportResult(requestParams, errorMessage);
        }

        /// <summary>
        /// Get the action result required for saving the report template.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        /// <param name="errorMessage">Save error message to be displayed in the designer.</param>
        public static ActionResult SaveReportResult(StiRequestParams requestParams, string errorMessage)
        {
            var result = StiWebActionResult.ErrorResult(requestParams, errorMessage);
            return StiNetCoreActionResult.FromWebActionResult(result);
        }

        /// <summary>
        /// Get the action result required for saving the report template.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        /// <param name="report">The report object to be sent back to the designer.</param>
        public static ActionResult SaveReportResult(Controller controller, StiReport report)
        {
            var requestParams = GetRequestParams(controller);
            return SaveReportResult(requestParams, report);
        }

        /// <summary>
        /// Get the action result required for saving the report template.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        /// <param name="report">The report object to be sent back to the designer.</param>
        public static ActionResult SaveReportResult(PageModel page, StiReport report)
        {
            var requestParams = GetRequestParams(page);
            return SaveReportResult(requestParams, report);
        }

        /// <summary>
        /// Get the action result required for saving the report template.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        /// <param name="report">The report object to be sent back to the designer.</param>
        public static ActionResult SaveReportResult(StiRequestParams requestParams, StiReport report)
        {
            report = report ?? GetActionReportObject(requestParams);
            var result = StiWebDesigner.CommandResult(requestParams, report);
            return StiNetCoreActionResult.FromWebActionResult(result);
        }
        #endregion

        #region PreviewReportResult

        #region Async
        /// <summary>
        /// Get the action result required for preview the report in the designer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        public static async Task<IActionResult> PreviewReportResultAsync(Controller controller)
        {
            return await Task.Run(() => PreviewReportResult(controller));
        }

        /// <summary>
        /// Get the action result required for preview the report in the designer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        public static async Task<IActionResult> PreviewReportResultAsync(PageModel page)
        {
            return await Task.Run(() => PreviewReportResult(page));
        }

        /// <summary>
        /// Get the action result required for preview the report in the designer. The specified request parameters will be used.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        public static async Task<IActionResult> PreviewReportResultAsync(StiRequestParams requestParams)
        {
            return await Task.Run(() => PreviewReportResult(requestParams));
        }

        /// <summary>
        /// Get the action result required for preview the report in the designer. The specified report template or snapshot will be used.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> PreviewReportResultAsync(Controller controller, StiReport report)
        {
            return await Task.Run(() => PreviewReportResult(controller, report));
        }

        /// <summary>
        /// Get the action result required for preview the report in the designer. The specified report template or snapshot will be used.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> PreviewReportResultAsync(PageModel page, StiReport report)
        {
            return await Task.Run(() => PreviewReportResult(page, report));
        }

        /// <summary>
        /// Get the action result required for preview the report in the designer. The specified request parameters and report template or snapshot will be used.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> PreviewReportResultAsync(StiRequestParams requestParams, StiReport report)
        {
            return await Task.Run(() => PreviewReportResult(requestParams, report));
        }
        #endregion

        /// <summary>
        /// Get the action result required for preview the report in the designer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        public static ActionResult PreviewReportResult(Controller controller)
        {
            var requestParams = GetRequestParams(controller);
            return PreviewReportResult(requestParams, null);
        }

        /// <summary>
        /// Get the action result required for preview the report in the designer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        public static ActionResult PreviewReportResult(PageModel page)
        {
            var requestParams = GetRequestParams(page);
            return PreviewReportResult(requestParams, null);
        }

        /// <summary>
        /// Get the action result required for preview the report in the designer. The specified request parameters will be used.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        public static ActionResult PreviewReportResult(StiRequestParams requestParams)
        {
            return PreviewReportResult(requestParams, null);
        }

        /// <summary>
        /// Get the action result required for preview the report in the designer. The specified report template or snapshot will be used.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult PreviewReportResult(Controller controller, StiReport report)
        {
            var requestParams = GetRequestParams(controller);
            return PreviewReportResult(requestParams, report);
        }

        /// <summary>
        /// Get the action result required for preview the report in the designer. The specified report template or snapshot will be used.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult PreviewReportResult(PageModel page, StiReport report)
        {
            var requestParams = GetRequestParams(page);
            return PreviewReportResult(requestParams, report);
        }

        /// <summary>
        /// Get the action result required for preview the report in the designer. The specified request parameters and report template or snapshot will be used.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult PreviewReportResult(StiRequestParams requestParams, StiReport report)
        {
            if (requestParams.Action == StiAction.PreviewReport)
            {
                // Set report for LoadReportToViewer command
                report = report ?? GetActionReportObject(requestParams);
                requestParams.Report = report;

                var result = StiWebDesigner.CommandResult(requestParams, report);
                return StiNetCoreActionResult.FromWebActionResult(result);
            }

            return ProcessRequestResult(requestParams, report);
        }
        #endregion

        #region ExportReportResult

        #region Async
        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        public static async Task<IActionResult> ExportReportResultAsync(Controller controller)
        {
            return await Task.Run(() => ExportReportResult(controller));
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        public static async Task<IActionResult> ExportReportResultAsync(PageModel page)
        {
            return await Task.Run(() => ExportReportResult(page));
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        public static async Task<IActionResult> ExportReportResultAsync(StiRequestParams requestParams)
        {
            return await Task.Run(() => ExportReportResult(requestParams));
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> ExportReportResultAsync(Controller controller, StiReport report)
        {
            return await Task.Run(() => ExportReportResult(controller, report));
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> ExportReportResultAsync(PageModel page, StiReport report)
        {
            return await Task.Run(() => ExportReportResult(page, report));
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> ExportReportResultAsync(StiRequestParams requestParams, StiReport report)
        {
            return await Task.Run(() => ExportReportResult(requestParams, report));
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static async Task<IActionResult> ExportReportResultAsync(Controller controller, StiExportSettings settings)
        {
            return await Task.Run(() => ExportReportResult(controller, settings));
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static async Task<IActionResult> ExportReportResultAsync(PageModel page, StiExportSettings settings)
        {
            return await Task.Run(() => ExportReportResult(page, settings));
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static async Task<IActionResult> ExportReportResultAsync(StiRequestParams requestParams, StiExportSettings settings)
        {
            return await Task.Run(() => ExportReportResult(requestParams, settings));
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static async Task<IActionResult> ExportReportResultAsync(Controller controller, StiReport report, StiExportSettings settings)
        {
            return await Task.Run(() => ExportReportResult(controller, report, settings));
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static async Task<IActionResult> ExportReportResultAsync(PageModel page, StiReport report, StiExportSettings settings)
        {
            return await Task.Run(() => ExportReportResult(page, report, settings));
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
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
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        public static ActionResult ExportReportResult(Controller controller)
        {
            var requestParams = GetRequestParams(controller);
            return ExportReportResult(requestParams, null, null);
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        public static ActionResult ExportReportResult(PageModel page)
        {
            var requestParams = GetRequestParams(page);
            return ExportReportResult(requestParams, null, null);
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        public static ActionResult ExportReportResult(StiRequestParams requestParams)
        {
            return ExportReportResult(requestParams, null, null);
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult ExportReportResult(Controller controller, StiReport report)
        {
            var requestParams = GetRequestParams(controller);
            return ExportReportResult(requestParams, report, null);
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult ExportReportResult(PageModel page, StiReport report)
        {
            var requestParams = GetRequestParams(page);
            return ExportReportResult(requestParams, report, null);
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult ExportReportResult(StiRequestParams requestParams, StiReport report)
        {
            return ExportReportResult(requestParams, report, null);
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static ActionResult ExportReportResult(Controller controller, StiExportSettings settings)
        {
            var requestParams = GetRequestParams(controller);
            return ExportReportResult(requestParams, null, settings);
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static ActionResult ExportReportResult(PageModel page, StiExportSettings settings)
        {
            var requestParams = GetRequestParams(page);
            return ExportReportResult(requestParams, null, settings);
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static ActionResult ExportReportResult(StiRequestParams requestParams, StiExportSettings settings)
        {
            return ExportReportResult(requestParams, null, settings);
        }

        /// <summary>
        /// Get the action result required for export the report.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
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
        /// <param name="page">The current page that processes the actions of the designer.</param>
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
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        /// <param name="settings">Export settings for the selected export format.</param>
        public static ActionResult ExportReportResult(StiRequestParams requestParams, StiReport report, StiExportSettings settings)
        {
            return StiNetCoreViewer.ExportReportResult(requestParams, report, settings);
        }
        #endregion

        #region DesignerEventResult

        #region Async
        /// <summary>
        /// Get the action result required for the various requests of the designer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        public static async Task<IActionResult> DesignerEventResultAsync(Controller controller)
        {
            return await Task.Run(() => DesignerEventResult(controller));
        }

        /// <summary>
        /// Get the action result required for the various requests of the designer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        public static async Task<IActionResult> DesignerEventResultAsync(PageModel page)
        {
            return await Task.Run(() => DesignerEventResult(page));
        }

        /// <summary>
        /// Get the action result required for the various requests of the designer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        public static async Task<IActionResult> DesignerEventResultAsync(StiRequestParams requestParams)
        {
            return await Task.Run(() => DesignerEventResult(requestParams));
        }

        /// <summary>
        /// Get the action result required for the various requests of the designer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> DesignerEventResultAsync(Controller controller, StiReport report)
        {
            return await Task.Run(() => DesignerEventResult(controller, report));
        }

        /// <summary>
        /// Get the action result required for the various requests of the designer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> DesignerEventResultAsync(PageModel page, StiReport report)
        {
            return await Task.Run(() => DesignerEventResult(page, report));
        }

        /// <summary>
        /// Get the action result required for the various requests of the designer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> DesignerEventResultAsync(StiRequestParams requestParams, StiReport report)
        {
            return await Task.Run(() => DesignerEventResult(requestParams, report));
        }
        #endregion

        /// <summary>
        /// Get the action result required for the various requests of the designer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        public static ActionResult DesignerEventResult(Controller controller)
        {
            var requestParams = GetRequestParams(controller);
            return DesignerEventResult(requestParams, null);
        }

        /// <summary>
        /// Get the action result required for the various requests of the designer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        public static ActionResult DesignerEventResult(PageModel page)
        {
            var requestParams = GetRequestParams(page);
            return DesignerEventResult(requestParams, null);
        }

        /// <summary>
        /// Get the action result required for the various requests of the designer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        public static ActionResult DesignerEventResult(StiRequestParams requestParams)
        {
            return DesignerEventResult(requestParams, null);
        }

        /// <summary>
        /// Get the action result required for the various requests of the designer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult DesignerEventResult(Controller controller, StiReport report)
        {
            var requestParams = GetRequestParams(controller);
            return DesignerEventResult(requestParams, report);
        }

        /// <summary>
        /// Get the action result required for the various requests of the designer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult DesignerEventResult(PageModel page, StiReport report)
        {
            var requestParams = GetRequestParams(page);
            return DesignerEventResult(requestParams, report);
        }

        /// <summary>
        /// Get the action result required for the various requests of the designer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult DesignerEventResult(StiRequestParams requestParams, StiReport report)
        {
            if (requestParams.ComponentType != StiComponentType.Designer)
                return ProcessRequestResult(requestParams, report);

            StiWebActionResult result = null;
            switch (requestParams.Action)
            {
                case StiAction.Resource:
                    result = StiDesignerResourcesHelper.Get(requestParams);
                    return new StiNetCoreActionResult(result.Data, result.ContentType, result.FileName, false, true);

                case StiAction.Exit:
                    return StiNetCoreActionResult.EmptyResult();

                default:
                    // Need for backward compatibility - previously, DesignerEvent handled all actions.
                    return ProcessRequestResult(requestParams, report);
            }
        }
        #endregion

        #region GetScriptsResult

        #region Async
        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the designer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        public static async Task<IActionResult> GetScriptsResultAsync(Controller controller)
        {
            return await Task.Run(() => GetScriptsResult(controller));
        }

        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the designer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        public static async Task<IActionResult> GetScriptsResultAsync(PageModel page)
        {
            return await Task.Run(() => GetScriptsResult(page));
        }

        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the designer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        public static async Task<IActionResult> GetScriptsResultAsync(StiRequestParams requestParams)
        {
            return await Task.Run(() => GetScriptsResult(requestParams));
        }

        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the designer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        /// <param name="options">A set of options that will be used for the designer.</param>
        public static async Task<IActionResult> GetScriptsResultAsync(Controller controller, StiNetCoreDesignerOptions options)
        {
            return await Task.Run(() => GetScriptsResult(controller, options));
        }

        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the designer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        /// <param name="options">A set of options that will be used for the designer.</param>
        public static async Task<IActionResult> GetScriptsResultAsync(PageModel page, StiNetCoreDesignerOptions options)
        {
            return await Task.Run(() => GetScriptsResult(page, options));
        }

        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the designer.
        /// </summary>
        /// <param name="options">A set of options that will be used for the designer.</param>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        public static async Task<IActionResult> GetScriptsResultAsync(StiRequestParams requestParams, StiNetCoreDesignerOptions options)
        {
            return await Task.Run(() => GetScriptsResult(requestParams, options));
        }
        #endregion

        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the designer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        public static ActionResult GetScriptsResult(Controller controller)
        {
            var requestParams = GetRequestParams(controller);
            var options = new StiNetCoreDesignerOptions();
            return GetScriptsResult(requestParams, options);
        }

        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the designer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        public static ActionResult GetScriptsResult(PageModel page)
        {
            var requestParams = GetRequestParams(page);
            var options = new StiNetCoreDesignerOptions();
            return GetScriptsResult(requestParams, options);
        }

        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the designer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        public static ActionResult GetScriptsResult(StiRequestParams requestParams)
        {
            var options = new StiNetCoreDesignerOptions();
            return GetScriptsResult(requestParams, options);
        }

        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the designer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        /// <param name="options">A set of options that will be used for the designer.</param>
        public static ActionResult GetScriptsResult(Controller controller, StiNetCoreDesignerOptions options)
        {
            var requestParams = GetRequestParams(controller);
            return GetScriptsResult(requestParams, options);
        }

        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the designer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        /// <param name="options">A set of options that will be used for the designer.</param>
        public static ActionResult GetScriptsResult(PageModel page, StiNetCoreDesignerOptions options)
        {
            var requestParams = GetRequestParams(page);
            return GetScriptsResult(requestParams, options);
        }

        /// <summary>
        /// Get the action result which returns a set of client scripts necessary for the work of the designer.
        /// </summary>
        /// <param name="options">A set of options that will be used for the designer.</param>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        /// <param name="forAngular">Value that indicate using in Angular version.</param>
        public static ActionResult GetScriptsResult(StiRequestParams requestParams, StiNetCoreDesignerOptions options, bool forAngular = false)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            requestParams.ComponentType = StiComponentType.Designer;
            requestParams.Action = StiAction.Resource;
            requestParams.Resource = "scripts";
            requestParams.Scripts.IncludeClient = true;
            requestParams.Localization = options.Localization;

            var scripts = StiDesignerResourcesHelper.Get(requestParams);
            var jsScripts = Encoding.UTF8.GetString(scripts.Data);
            var jsOptions = JsonConvert.SerializeObject(options, Formatting.None,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = { new StringEnumConverter() }
                });

            var requestUrl = !forAngular ? requestParams.HttpContext.Request.Url.LocalPath : requestParams.HttpContext.Request.Url.AbsoluteUri;

            if (!string.IsNullOrEmpty(options.Server.RouteTemplate))
                requestUrl = options.Server.RouteTemplate;

            var resourcesUrl = requestUrl;
            resourcesUrl += resourcesUrl.IndexOf("?") < 0 ? "?" : "&";
            resourcesUrl += "stiweb_component=Designer&stiweb_action=Resource&stiweb_cachemode=" + (options.Server.UseCacheForResources
                ? options.Server.CacheMode == StiServerCacheMode.ObjectSession || options.Server.CacheMode == StiServerCacheMode.StringSession
                    ? "session"
                    : "cache"
                : "none");
            resourcesUrl += "&stiweb_version=" + StiVersionHelper.AssemblyVersion + "&stiweb_data=";

            var parameters = new Hashtable();
            parameters["shortProductVersion"] = StiVersionHelper.AssemblyVersion;
            parameters["productVersion"] = StiVersionHelper.ProductVersion.Trim();
            parameters["frameworkType"] = StiNetCoreHelper.GetFrameworkVersion();
            parameters["requestUrl"] = requestUrl;
            parameters["requestAbsoluteUrl"] = requestParams.HttpContext.Request.Url.AbsoluteUri;
            parameters["stylesUrl"] = string.IsNullOrEmpty(options.Appearance.CustomCss) ? resourcesUrl.Replace("&stiweb_data=", "&stiweb_theme=") + options.Theme.ToString() : options.Appearance.CustomCss;
            parameters["scriptsUrl"] = resourcesUrl;
            parameters["dashboardAssemblyLoaded"] = StiDashboardAssembly.IsAssemblyLoaded && StiDashboardExportAssembly.IsAssemblyLoaded && StiDashboardDrawingAssembly.IsAssemblyLoaded;
            parameters["fullScreenMode"] = options.Width == Unit.Empty && options.Height == Unit.Empty;
            parameters["haveExitDesignerEvent"] = true;
            parameters["haveSaveEvent"] = true;
            parameters["haveSaveAsEvent"] = false;
            parameters["resourceIdent"] = StiHyperlinkProcessor.ResourceIdent;
            parameters["variableIdent"] = StiHyperlinkProcessor.VariableIdent;
            parameters["defaultDesignerOptions"] = StiDesignerOptionsHelper.GetDefaultDesignerOptions();
            parameters["reportResourcesMaximumSize"] = StiOptions.Engine.ReportResources.MaximumSize;
            parameters["isAngular"] = forAngular;

            //-------------- obsolete, for suporting old versions --------------
            parameters["runWizardAfterLoad"] = StiOptions.Designer.RunWizardAfterLoad;
            parameters["reportResourcesMaximumSize"] = StiOptions.Engine.ReportResources.MaximumSize;
            //------------------------------------------------------------------

            // Collections
            string[] collections = new string[] {
                "images", "loc", "locFiles", "paperSizes", "hatchStyles", "summaryTypes", "aggrigateFunctions", "fontNames", "conditions", "iconSetArrays",
                "dBaseCodePages", "csvCodePages", "textFormats", "currencySymbols", "dateFormats", "timeFormats", "customFormats", "cultures", "fontIcons",
                "fontIconSets", "predefinedColors"
            };

            for (int i = 0; i < collections.Length; i++)
            {
                object collectionObject = null;
                switch (collections[i])
                {
                    case "loc": collectionObject = StiLocalization.GetLocalization(false); break;
                    case "locFiles": collectionObject = StiCollectionsHelper.GetLocalizationsList(requestParams.HttpContext, options.LocalizationDirectory); break;
                    case "images": collectionObject = StiDesignerResourcesHelper.GetImagesArray(requestParams, resourcesUrl); break;
                    case "paperSizes": collectionObject = StiPaperSizes.GetItems(); break;
                    case "hatchStyles": collectionObject = StiHatchStyles.GetItems(); break;
                    case "summaryTypes": collectionObject = StiSummaryTypes.GetItems(); break;
                    case "aggrigateFunctions": collectionObject = StiAggrigateFunctions.GetItems(); break;
                    case "fontNames": collectionObject = StiFontNames.GetItems(options.Server.AllowLoadingCustomFontsToClientSide); break;
                    case "conditions": collectionObject = StiDefaultConditions.GetItems(); break;
                    case "iconSetArrays": collectionObject = StiIconSetArrays.GetItems(); break;
                    case "dBaseCodePages": collectionObject = StiCodePageHelper.GetDBaseCodePageItems(); break;
                    case "csvCodePages": collectionObject = StiCodePageHelper.GetCsvCodePageItems(); break;
                    case "textFormats": collectionObject = StiTextFormatHelper.GetTextFormatItems(); break;
                    case "currencySymbols": collectionObject = StiTextFormatHelper.GetCurrencySymbols(); break;
                    case "dateFormats": collectionObject = StiTextFormatHelper.GetDateAndTimeFormats("date", new StiDateFormatService()); break;
                    case "timeFormats": collectionObject = StiTextFormatHelper.GetDateAndTimeFormats("time", new StiTimeFormatService()); break;
                    case "customFormats": collectionObject = StiTextFormatHelper.GetDateAndTimeFormats("custom", new StiCustomFormatService()); break;
                    case "cultures": collectionObject = StiCultureHelper.GetItems(CultureTypes.SpecificCultures); break;
                    case "fontIcons": collectionObject = StiWebFontIconsHelper.GetIconItems(); break;
                    case "fontIconSets": collectionObject = StiWebFontIconsHelper.GetIconSetItems(); break;
                    case "predefinedColors": collectionObject = StiColorHelper.GetPredefinedColors(); break;
                }

                parameters[collections[i]] = collectionObject;
            }

            parameters["stimulsoftFontContent"] = StiReportResourceHelper.GetStimulsoftFontBase64Data();

            if (!options.Appearance.ShowSystemFonts)
            {
                var fontItems = StiFontNames.GetOpenTypeFontItems();
                if (fontItems.Count > 0)
                    parameters["opentypeFonts"] = fontItems;
            }

            parameters["showOnlyAliasForComponents"] = StiOptions.Dictionary.ShowOnlyAliasForComponents;
            parameters["showOnlyAliasForPages"] = StiOptions.Dictionary.ShowOnlyAliasForPages;
            parameters["showOnlyAliasForDatabase"] = StiOptions.Dictionary.ShowOnlyAliasForDatabase;
            parameters["showOnlyAliasForData"] = StiOptions.Dictionary.ShowOnlyAliasForData;
            parameters["showOnlyAliasForVariable"] = StiOptions.Dictionary.ShowOnlyAliasForVariable;
            parameters["showOnlyAliasForResource"] = StiOptions.Dictionary.ShowOnlyAliasForResource;
            parameters["showOnlyAliasForDataSource"] = StiOptions.Dictionary.ShowOnlyAliasForDataSource;
            parameters["showOnlyAliasForBusinessObject"] = StiOptions.Dictionary.ShowOnlyAliasForBusinessObject;
            parameters["showOnlyAliasForDataColumn"] = StiOptions.Dictionary.ShowOnlyAliasForDataColumn;
            parameters["showOnlyAliasForDataRelation"] = StiOptions.Dictionary.ShowOnlyAliasForDataRelation;

            parameters["urlCursorStyleSet"] = GetImageUrl(requestParams, resourcesUrl + "Cursors.StyleSet.cur");
            parameters["urlCursorPen"] = GetImageUrl(requestParams, resourcesUrl + "Cursors.Pen.cur");

            var jsParameters = JsonConvert.SerializeObject(parameters, Formatting.None);

            var jsData =
                $"{jsScripts}\r\n" +
                $"Stimulsoft.Designer.defaultOptions = {jsOptions};\r\n" +
                $"Stimulsoft.Designer.parameters = {jsParameters};";

            var viewerOptions = GetPreviewControlOptions(options);
            var viewerScripts = StiNetCoreViewer.GetScriptsResult(requestParams, viewerOptions, forAngular);
            var viewerData = ((StiNetCoreActionResult)viewerScripts).Data;
            var jsViewerData = Encoding.UTF8.GetString(viewerData);

            var data = Encoding.UTF8.GetBytes(jsData + jsViewerData);
            return new StiNetCoreActionResult(data, scripts.ContentType, scripts.FileName, false, true);
        }
        #endregion

        #region ProcessRequestResult

        #region Async
        /// <summary>
        /// Get the action result required for the all requests of the designer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        public static async Task<IActionResult> ProcessRequestResultAsync(Controller controller)
        {
            return await Task.Run(() => ProcessRequestResult(controller));
        }

        /// <summary>
        /// Get the action result required for the all requests of the designer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        public static async Task<IActionResult> ProcessRequestResultAsync(PageModel page)
        {
            return await Task.Run(() => ProcessRequestResult(page));
        }

        /// <summary>
        /// Get the action result required for the all requests of the designer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        public static async Task<IActionResult> ProcessRequestResultAsync(StiRequestParams requestParams)
        {
            return await Task.Run(() => ProcessRequestResult(requestParams));
        }

        /// <summary>
        /// Get the action result required for the all requests of the designer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static async Task<IActionResult> ProcessRequestResultAsync(StiRequestParams requestParams, StiReport report)
        {
            return await Task.Run(() => ProcessRequestResult(requestParams, report));
        }
        #endregion

        /// <summary>
        /// Get the action result required for the all requests of the designer.
        /// </summary>
        /// <param name="controller">The current controller that processes the actions of the designer.</param>
        public static ActionResult ProcessRequestResult(Controller controller)
        {
            var requestParams = GetRequestParams(controller);
            return ProcessRequestResult(requestParams, null);
        }

        /// <summary>
        /// Get the action result required for the all requests of the designer.
        /// </summary>
        /// <param name="page">The current page that processes the actions of the designer.</param>
        public static ActionResult ProcessRequestResult(PageModel page)
        {
            var requestParams = GetRequestParams(page);
            return ProcessRequestResult(requestParams, null);
        }

        /// <summary>
        /// Get the action result required for the all requests of the designer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        public static ActionResult ProcessRequestResult(StiRequestParams requestParams)
        {
            return ProcessRequestResult(requestParams, null);
        }

        /// <summary>
        /// Get the action result required for the all requests of the designer.
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the designer.</param>
        /// <param name="report">The report object to be used when processing the request.</param>
        public static ActionResult ProcessRequestResult(StiRequestParams requestParams, StiReport report)
        {
            if (requestParams.Action == StiAction.Undefined && requestParams.HttpMethod == "GET")
                return GetScriptsResult(requestParams);

            // Process preview tab requests
            if (requestParams.ComponentType == StiComponentType.Viewer)
                return StiNetCoreViewer.ProcessRequestResult(requestParams, report);

            // Process designer requests
            if (requestParams.ComponentType == StiComponentType.Designer)
            {
                switch (requestParams.Action)
                {
                    case StiAction.Resource:
                    case StiAction.Exit:
                        return DesignerEventResult(requestParams, report);

                    case StiAction.GetReport:
                    case StiAction.OpenReport:
                    case StiAction.CreateReport:
                        return GetReportResult(requestParams, report);

                    case StiAction.SaveReport:
                        return SaveReportResult(requestParams);

                    case StiAction.PreviewReport:
                        return PreviewReportResult(requestParams, report);

                    case StiAction.ExportReport:
                    case StiAction.ExportDashboard:
                        return ExportReportResult(requestParams, report);

                    default:
                        report = report ?? GetActionReportObject(requestParams);
                        var result = StiWebDesigner.CommandResult(requestParams, report);
                        return StiNetCoreActionResult.FromWebActionResult(result);
                }
            }

            return StiNetCoreActionResult.EmptyResult();
        }
        #endregion
    }
}
