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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Stimulsoft.Report.Web
{
    public partial class StiWebDesigner :
        WebControl,
        INamingContainer
    {
        #region DesignerEvent

        /// <summary>
        /// The event occurs for any action of the designer before it is processed.
        /// </summary>
        [Category("Viewer")]
        [Description("The event occurs for any action of the designer before it is processed.")]
        public event StiDesignerEventHandler DesignerEvent;

        private void InvokeDesignerEvent()
        {
            var e = new StiDesignerEventArgs(this.RequestParams);
            OnDesignerEvent(e);
        }

        protected virtual void OnDesignerEvent(StiDesignerEventArgs e)
        {
            DesignerEvent?.Invoke(this, e);
        }

        #endregion

        #region GetReport

        /// <summary>
        /// The event occurs when the report template is requested after running the designer.
        /// </summary>
        [Category("Designer")]
        [Description("The event occurs when the report template is requested after running the designer.")]
        public event StiReportDataEventHandler GetReport;

        private void InvokeGetReport(StiReport currentReport)
        {
            StiReportDataEventArgs e = new StiReportDataEventArgs(this.RequestParams, currentReport);
            OnGetReport(e);
            this.report = GetReportForDesigner(this.RequestParams, e.Report);
        }

        protected virtual void OnGetReport(StiReportDataEventArgs e)
        {
            GetReport?.Invoke(this, e);
        }

        #endregion

        #region OpenReport

        /// <summary>
        /// The event occurs when the report template is opened using the file menu.
        /// </summary>
        [Category("Designer")]
        [Description("The event occurs when the report template is opened using the file menu.")]
        public event StiReportDataEventHandler OpenReport;

        private void InvokeOpenReport(StiReport currentReport)
        {
            var e = new StiReportDataEventArgs(this.RequestParams, currentReport);
            OnOpenReport(e);
            this.report = e.Report;
        }

        protected virtual void OnOpenReport(StiReportDataEventArgs e)
        {
            if (OpenReport != null) OpenReport(this, e);
        }

        #endregion

        #region CreateReport

        /// <summary>
        /// The event occurs when creating a new report template using the file menu.
        /// </summary>
        [Category("Designer")]
        [Description("The event occurs when creating a new report template using the file menu.")]
        public event StiReportDataEventHandler CreateReport;

        private void InvokeCreateReport(StiReport newReport)
        {
            var e = new StiReportDataEventArgs(this.RequestParams, newReport);
            OnCreateReport(e);
            this.report = e.Report;
        }

        protected virtual void OnCreateReport(StiReportDataEventArgs e)
        {
            if (CreateReport != null) CreateReport(this, e);
        }

        #endregion

        #region SaveReport / SaveReportAs

        /// <summary>
        /// The event occurs when saving the report template.
        /// </summary>
        [Category("Designer")]
        [Description("The event occurs when saving a report template.")]
        public event StiSaveReportEventHandler SaveReport;

        /// <summary>
        /// The event occurs when saving as file the report template.
        /// </summary>
        [Category("Designer")]
        [Description("The event occurs when 'saving as' a report template.")]
        public event StiSaveReportEventHandler SaveReportAs;

        private void InvokeSaveReport(StiReport currentReport)
        {
            var e = new StiSaveReportEventArgs(this.RequestParams, currentReport);

            if (this.RequestParams.Designer.IsSaveAs) OnSaveReportAs(e);
            else OnSaveReport(e);

            if (!string.IsNullOrEmpty(e.ErrorString)) this.SaveReportErrorString = e.ErrorString;
            else if (e.ErrorCode >= 0) this.SaveReportErrorString = "Error at saving. Error code: " + e.ErrorCode.ToString();
            else this.SaveReportErrorString = string.Empty;

            if (e.SendReportToClient) this.report = e.Report;
        }

        protected virtual void OnSaveReport(StiSaveReportEventArgs e)
        {
            if (SaveReport != null) SaveReport(this, e);
        }

        protected virtual void OnSaveReportAs(StiSaveReportEventArgs e)
        {
            if (SaveReportAs != null) SaveReportAs(this, e);
        }

        #endregion

        #region PreviewReport

        /// <summary>
        /// The event occurs before rendering the report for preview. Allowed to change the properties of the report, register the data.
        /// </summary>
        [Category("Designer")]
        [Description("The event occurs before rendering the report for preview. Allowed to change the properties of the report, register the data.")]
        public event StiReportDataEventHandler PreviewReport;

        private void InvokePreviewReport(StiReport report)
        {
            var e = new StiReportDataEventArgs(this.RequestParams, report);
            OnPreviewReport(e);
            this.RequestParams.Report = e.Report; // Set report for LoadReportToViewer command
        }

        protected virtual void OnPreviewReport(StiReportDataEventArgs e)
        {
            if (PreviewReport != null) PreviewReport(this, e);
        }

        #endregion

        #region GetPreviewReport

        /// <summary>
        /// The event occurs before displaying the rendered report in the preview.
        /// </summary>
        [Category("Viewer")]
        [Description("The event occurs before displaying the rendered report in the preview.")]
        public event StiReportDataEventHandler GetPreviewReport;

        private void InvokeGetPreviewReport(StiReport report)
        {
            var e = new StiReportDataEventArgs(this.RequestParams, report);
            OnGetPreviewReport(e);
        }

        protected virtual void OnGetPreviewReport(StiReportDataEventArgs e)
        {
            if (GetPreviewReport != null)
            {
                GetPreviewReport(this, e);
                this.RequestParams.Report = e.Report; // Set report for viewer
            }
        }
        #endregion

        #region ExportReport

        /// <summary>
        /// The event occurs before the exporting of the report from the preview tab.
        /// </summary>
        [Category("Viewer")]
        [Description("The event occurs before the exporting of the report from the preview tab.")]
        public event StiExportReportEventHandler ExportReport;

        private void InvokeExportReport(StiExportReportEventArgs e)
        {
            OnExportReport(e);
        }

        protected virtual void OnExportReport(StiExportReportEventArgs e)
        {
            if (ExportReport != null) ExportReport(this, e);
        }

        /// <summary>
        /// The event occurs after the report is exported from the preview tab.
        /// </summary>
        [Category("Viewer")]
        [Description("The event occurs after the report is exported from the preview tab.")]
        public event StiExportReportResponseEventHandler ExportReportResponse;

        private void InvokeExportReportResponse(StiExportReportResponseEventArgs e)
        {
            OnExportReportResponse(e);
        }

        protected virtual void OnExportReportResponse(StiExportReportResponseEventArgs e)
        {
            if (ExportReportResponse != null) ExportReportResponse(this, e);
        }

        #endregion

        #region Exit

        /// <summary>
        /// Fires when the user pressed Exit in the file menu.
        /// </summary>
        [Category("Designer")]
        [Description("Fires when the user pressed Exit in the file menu.")]
        public event StiReportDataEventHandler Exit;

        private void InvokeExit(StiReport currentReport)
        {
            var e = new StiReportDataEventArgs(this.RequestParams, currentReport);
            OnExit(e);
        }

        protected virtual void OnExit(StiReportDataEventArgs e)
        {
            if (Exit != null) Exit(this, e);
        }
        
        #endregion
    }
}
