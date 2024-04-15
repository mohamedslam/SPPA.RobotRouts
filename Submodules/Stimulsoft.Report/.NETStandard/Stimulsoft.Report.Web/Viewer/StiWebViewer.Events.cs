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
using System.IO;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Stimulsoft.Report.Export;

namespace Stimulsoft.Report.Web
{
    public partial class StiWebViewer :
        WebControl,
        INamingContainer
    {
        #region ViewerEvent

        /// <summary>
        /// The event occurs for any action of the viewer before it is processed.
        /// </summary>
        [Category("Viewer")]
        [Description("The event occurs for any action of the viewer before it is processed.")]
        public event StiViewerEventHandler ViewerEvent;

        private void InvokeViewerEvent()
        {
            var e = new StiViewerEventArgs(this.RequestParams);
            OnViewerEvent(e);
        }

        protected virtual void OnViewerEvent(StiViewerEventArgs e)
        {
            ViewerEvent?.Invoke(this, e);
        }

        #endregion

        #region GetReport

        /// <summary>
        /// The event occurs when the report is requested after running the viewer.
        /// </summary>
        [Category("Viewer")]
        [Description("The event occurs when the report is requested after running the viewer.")]
        public event StiReportDataEventHandler GetReport;
        
        private void InvokeGetReport()
        {
            var requestParams = this.RequestParams;
            var report = requestParams.Report ?? this.Report;

            if (requestParams.Action == StiAction.GetReport || report == null)
            {
                var e = new StiReportDataEventArgs(requestParams, report);
                OnGetReport(e);
                this.report = e.Report;
                InvokeGetReportData();
                if (e.Report != null)
                {
                    StiReportHelper.ApplyQueryParameters(requestParams, e.Report);
                    var action = requestParams.Action;
                    requestParams.Action = StiAction.GetReport;
                    requestParams.Cache.Helper.SaveReportInternal(requestParams, e.Report);
                    requestParams.Action = action;
                }
            }
            else if (report != null && report.IsRendered == false)
            {
                InvokeGetReportData();
            }
        }

        protected virtual void OnGetReport(StiReportDataEventArgs e)
        {
            GetReport?.Invoke(this, e);
        }

        #endregion

        #region GetReportData

        /// <summary>
        /// The event occurs each time before rendering the report.
        /// </summary>
        [Category("Viewer")]
        [Description("The event occurs each time before rendering the report.")]
        public event StiReportDataEventHandler GetReportData;

        private void InvokeGetReportData()
        {
            if (this.Report != null && this.Report.IsDocument == false)
            {
                var e = new StiReportDataEventArgs(this.RequestParams, this.Report);
                OnGetReportData(e);
            }
        }

        protected virtual void OnGetReportData(StiReportDataEventArgs e)
        {
            GetReportData?.Invoke(this, e);
        }

        #endregion

        #region PrintReport

        /// <summary>
        /// The event occurs before the printing of the report from the viewer menu.
        /// </summary>
        [Category("Viewer")]
        [Description("The event occurs before the printing of the report from the viewer menu.")]
        public event StiPrintReportEventHandler PrintReport;

        private void InvokePrintReport(StiExportSettings settings)
        {
            InvokeGetReport();
            var e = new StiPrintReportEventArgs(this.RequestParams, this.Report, settings);
            OnPrintReport(e);
        }

        protected virtual void OnPrintReport(StiPrintReportEventArgs e)
        {
            PrintReport?.Invoke(this, e);
        }

        #endregion

        #region ExportReport

        /// <summary>
        /// The event occurs before the exporting of the report from the viewer menu.
        /// </summary>
        [Category("Viewer")]
        [Description("The event occurs before the exporting of the report from the viewer menu.")]
        public event StiExportReportEventHandler ExportReport;

        private void InvokeExportReport(StiExportSettings settings)
        {
            InvokeGetReport();
            var e = new StiExportReportEventArgs(this.RequestParams, this.Report, settings);
            OnExportReport(e);
        }

        protected virtual void OnExportReport(StiExportReportEventArgs e)
        {
            if (ExportReport != null) ExportReport(this, e);
        }

        /// <summary>
        /// The event occurs after the report is exported.
        /// </summary>
        [Category("Viewer")]
        [Description("The event occurs after the report is exported.")]
        public event StiExportReportResponseEventHandler ExportReportResponse;
        private void InvokeExportReportResponse(StiExportSettings settings, StiWebActionResult result)
        {
            var stream = new MemoryStream(result.Data);
            var e = new StiExportReportResponseEventArgs(RequestParams, Report, settings, stream, result.FileName, result.ContentType);
            OnExportReportResponse(e);

            result.FileName = e.FileName;
            result.ContentType = e.ContentType;
            if (e.Stream != stream && e.Stream is MemoryStream)
                result.Data = ((MemoryStream)e.Stream).ToArray();
        }

        protected virtual void OnExportReportResponse(StiExportReportResponseEventArgs e)
        {
            ExportReportResponse?.Invoke(this, e);
        }

        #endregion

        #region EmailReport

        /// <summary>
        /// The event occurs when sending a report via Email. In this event it is necessary to set settings for sending Email.
        /// </summary>
        [Category("Viewer")]
        [Description("The event occurs when sending a report via Email. In this event it is necessary to set settings for sending Email.")]
        public event StiEmailReportEventHandler EmailReport;

        private void InvokeEmailReport(StiExportSettings settings, StiEmailOptions options)
        {
            InvokeGetReport();
            var e = new StiEmailReportEventArgs(this.RequestParams, this.Report, settings, options);
            OnEmailReport(e);
        }

        protected virtual void OnEmailReport(StiEmailReportEventArgs e)
        {
            EmailReport?.Invoke(this, e);
        }

        #endregion

        #region Interaction

        /// <summary>
        /// The event occurs before the interactive action in the viewer, such as sorting, collapsing, drill-down, request from user variables.
        /// </summary>
        [Category("Viewer")]
        [Description("The event occurs before the interactive action in the viewer, such as sorting, collapsing, drill-down, request from user variables.")]
        public event StiReportDataEventHandler Interaction;

        private void InvokeInteraction()
        {
            InvokeGetReport();
            var e = new StiReportDataEventArgs(this.RequestParams, this.Report);
            OnInteraction(e);
        }

        protected virtual void OnInteraction(StiReportDataEventArgs e)
        {
            Interaction?.Invoke(this, e);
        }

        #endregion

        #region DesignReport

        /// <summary>
        /// The event occurs when clicking on the Design button in the viewer toolbar.
        /// </summary>
        [Category("Viewer")]
        [Description("The event occurs when clicking on the Design button in the viewer toolbar.")]
        public event StiReportDataEventHandler DesignReport;

        private void InvokeDesignReport()
        {
            InvokeGetReport();
            var e = new StiReportDataEventArgs(this.RequestParams, this.Report);
            OnDesignReport(e);
        }

        protected virtual void OnDesignReport(StiReportDataEventArgs e)
        {
            if (DesignReport != null) DesignReport(this, e);
        }

        #endregion
    }
}
