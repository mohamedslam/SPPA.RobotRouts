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
using Stimulsoft.Report.Export;
using System.ComponentModel;

namespace Stimulsoft.Report.Web
{
    #region StiWebEventArgs

    public delegate void StiWebEventHandler(object sender, StiWebEventArgs e);

    public class StiWebEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the current action of the web component.
        /// </summary>
        public StiAction Action
        {
            get
            {
                if (RequestParams != null) return RequestParams.Action;
                return StiAction.Undefined;
            }
        }

        /// <summary>
        /// All of the parameters passed to the server from the client side of the web component.
        /// </summary>
        public StiRequestParams RequestParams { get; private set; }

        public StiWebEventArgs(StiRequestParams requestParams)
        {
            this.RequestParams = requestParams;
        }
    }

    #endregion

    #region StiViewerEventArgs

    public delegate void StiViewerEventHandler(object sender, StiViewerEventArgs e);

    public class StiViewerEventArgs : StiWebEventArgs
    {
        public StiViewerEventArgs(StiRequestParams requestParams) : base(requestParams)
        {
        }
    }

    #endregion

    #region StiDesignerEventArgs

    public delegate void StiDesignerEventHandler(object sender, StiDesignerEventArgs e);

    public class StiDesignerEventArgs : StiWebEventArgs
    {
        /// <summary>
        /// Gets the current command of the report designer.
        /// </summary>
        public StiDesignerCommand Command
        {
            get
            {
                if (RequestParams != null) return RequestParams.Designer.Command;
                return StiDesignerCommand.Undefined;
            }
        }

        public StiDesignerEventArgs(StiRequestParams requestParams) : base(requestParams)
        {
        }
    }

    #endregion

    #region StiReportDataEventArgs

    public delegate void StiReportDataEventHandler(object sender, StiReportDataEventArgs e);

    public class StiReportDataEventArgs : StiWebEventArgs
    {
        /// <summary>
        /// Gets or sets the current report object.
        /// </summary>
        public StiReport Report { get; set; }

        /// <summary>
        /// Indicates the presence of data in the report data storage.
        /// </summary>
        public virtual bool IsDataStoreEmpty
        {
            get
            {
                if (Report == null || Report.DataStore == null) return true;
                return Report.DataStore.Count == 0;
            }
        }

        /// <summary>
        /// Indicates whether the report is rendered or not.
        /// </summary>
        public virtual bool IsReportRendered
        {
            get
            {
                if (Report != null) return Report.IsRendered;
                return false;
            }
        }

        /// <summary>
        /// Indicates that the report object contain only rendered document which loaded from file or other source.
        /// </summary>
        public virtual bool IsReportDocument
        {
            get
            {
                if (Report != null) return Report.IsDocument;
                return false;
            }
        }

        public StiReportDataEventArgs(StiRequestParams requestParams, StiReport report) : base(requestParams)
        {
            this.Report = report;
        }
    }

    #endregion

    #region StiSaveReportEventArgs

    public delegate void StiSaveReportEventHandler(object sender, StiSaveReportEventArgs e);

    public class StiSaveReportEventArgs : StiReportDataEventArgs
    {
        /// <summary>
        /// Indicates the occurrence of an event of the automatic saving of the report.
        /// Only for Flash designer.
        /// </summary>
        public bool IsAutoSave
        {
            get
            {
                return this.RequestParams.Designer.IsAutoSave;
            }
        }

        /// <summary>
        /// Indicates saving a new report, that means the report is saved the first time after creation.
        /// Only for Flash designer.
        /// </summary>
        public bool IsNewReport
        {
            get
            {
                return this.RequestParams.Designer.IsNewReport;
            }
        }

        /// <summary>
        /// Sets the value that allows sending the saved report back to the client side, for example if it has been modified in the current event.
        /// Only for Flash designer.
        /// </summary>
        public bool SendReportToClient { get; set; }
        
        /// <summary>
        /// File name of the saving report template.
        /// </summary>
        public string FileName
        {
            get
            {
                return this.RequestParams.Designer.FileName;
            }
        }

        /// <summary>
        /// Password for save the encrypted report.
        /// Only for HTML5 designer.
        /// </summary>
        public string Password
        {
            get
            {
                return this.RequestParams.Designer.Password;
            }
        }

        /// <summary>
        /// Sets the error code saving the report, which will appear in the designer.
        /// If the code is -1 the message will not be shown. If the code is 0 then shows the message of successful saving of the report.
        /// Only for Flash designer.
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Sets the error message saving the report, which will appear in the designer. If the message is empty, it will use the ErrorCode as a result.
        /// Only for Flash designer.
        /// </summary>
        public string ErrorString { get; set; }

        #region Hide properties

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool IsDataStoreEmpty
        {
            get
            {
                return base.IsDataStoreEmpty;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool IsReportDocument
        {
            get
            {
                return base.IsReportDocument;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool IsReportRendered
        {
            get
            {
                return base.IsReportRendered;
            }
        }

        #endregion

        public StiSaveReportEventArgs(StiRequestParams requestParams, StiReport report)
            : base(requestParams, report)
        {
            this.SendReportToClient = false;
            this.ErrorCode = -1;
            this.ErrorString = string.Empty;
        }
    }

    #endregion

    #region StiPrintReportEventArgs

    public delegate void StiPrintReportEventHandler(object sender, StiPrintReportEventArgs e);

    public class StiPrintReportEventArgs : StiReportDataEventArgs
    {
        /// <summary>
        /// Gets a print action.
        /// </summary>
        public StiPrintAction PrintAction { get; private set; }

        /// <summary>
        /// Gets an export settings (PDF or HTML) that will be used to prepare the report for printing. Allowed to change the properties of the settings object.
        /// </summary>
        public StiExportSettings Settings { get; private set; }

        public StiPrintReportEventArgs(StiRequestParams requestParams, StiReport report, StiExportSettings settings)
            : base(requestParams, report)
        {
            this.Settings = settings;
            this.PrintAction = requestParams.Viewer.PrintAction;
        }
    }

    #endregion

    #region StiExportReportEventArgs

    public delegate void StiExportReportEventHandler(object sender, StiExportReportEventArgs e);

    public class StiExportReportEventArgs : StiReportDataEventArgs
    {
        /// <summary>
        /// Gets an export settings that will be used to export the report. Allowed to change the properties of the settings object.
        /// </summary>
        public StiExportSettings Settings { get; private set; }

        /// <summary>
        /// Gets an export format, based on the export settings.
        /// </summary>
        public StiExportFormat Format
        {
            get
            {
                if (Settings is StiExportSettings) return ((StiExportSettings)Settings).GetExportFormat();
                return StiExportFormat.None;
            }
        }

        public StiExportReportEventArgs(StiRequestParams requestParams, StiReport report, StiExportSettings settings)
            : base(requestParams, report)
        {
            this.Settings = settings;
        }
    }

    #endregion

    #region StiExportReportResponseEventArgs

    public delegate void StiExportReportResponseEventHandler(object sender, StiExportReportResponseEventArgs e);

    public class StiExportReportResponseEventArgs : StiExportReportEventArgs
    {
        /// <summary>
        /// Gets or sets a exported report stream.
        /// </summary>
        public Stream Stream { get; set; }
        
        /// <summary>
        /// Gets or sets a file name of the exported report.
        /// </summary>
        public string FileName { get; set; }
        
        /// <summary>
        /// Gets or sets a content type of the exported report.
        /// </summary>
        public string ContentType { get; set; }

        public StiExportReportResponseEventArgs(StiRequestParams requestParams, StiReport report, StiExportSettings settings, Stream stream, string fileName, string contentType)
            : base(requestParams, report, settings)
        {
            this.Stream = stream;
            this.FileName = fileName;
            this.ContentType = contentType;
        }
    }

    #endregion

    #region StiEmailReportEventArgs

    public delegate void StiEmailReportEventHandler(object sender, StiEmailReportEventArgs e);

    public class StiEmailReportEventArgs : StiExportReportEventArgs
    {
        /// <summary>
        /// Gets an Email options.
        /// </summary>
        public StiEmailOptions Options { get; private set; }

        public StiEmailReportEventArgs(StiRequestParams requestParams, StiReport report, StiExportSettings settings, StiEmailOptions options)
            : base(requestParams, report, settings)
        {
            this.Options = options;
        }
    }

    #endregion


    #region Obsolete

    [Obsolete("This delegate is obsolete. It will be removed in next versions. Please use the StiReportDataEventHandler delegate instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate void StiExitEventHandler(object sender, StiExitEventArgs e);

    [Obsolete("This delegate is obsolete. It will be removed in next versions. Please use the StiReportDataEventArgs delegate instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class StiExitEventArgs : StiReportDataEventArgs
    {
        public StiExitEventArgs(StiRequestParams reqestParams, StiReport report) : base(reqestParams, report)
        {
            this.Report = report;
        }
    }

    #endregion
}
