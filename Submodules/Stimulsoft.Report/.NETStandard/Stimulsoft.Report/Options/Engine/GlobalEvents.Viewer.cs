#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
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
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Viewer;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// A class which controls of settings of the report engine. 
        /// </summary>
		public sealed partial class Engine
        {
            public sealed partial class GlobalEvents
            {
                #region SendEMailProcess
                public static event StiSendEMailEventHandler SendEMailProcess;

                public static void InvokeSendEMailProcess(object sender, StiSendEMailEventArgs e)
                {
                    SendEMailProcess?.Invoke(sender, e);
                }
                #endregion

                #region SaveDocumentProcess
                public static event StiSaveDocumentEventHandler SaveDocumentProcess;

                public static void InvokeSaveDocumentProcess(object sender, StiSaveDocumentEventArgs e)
                {
                    SaveDocumentProcess?.Invoke(sender, e);
                }
                #endregion

                #region LoadViewerControl
                public static event EventHandler LoadViewerControl;
                public static void InvokeLoadViewerControl(object control)
                {
                    LoadViewerControl?.Invoke(control, EventArgs.Empty);
                }
                #endregion

                #region DropFileInViewer
                public static event StiDropFileInViewerEventHandler DropFileInViewer;
                public static bool AllowDropFileInViewer => DropFileInViewer != null;

                public static bool InvokeDropFileInViewer(StiReport report, StiDropFileInViewerEventArgs args)
                {
                    if (DropFileInViewer != null)
                    {
                        DropFileInViewer(report, args);
                        return true;
                    }
                    else return false;
                }
                #endregion

                #region PasteDataInViewer
                public static event StiPasteDataInViewerEventHandler PasteDataInViewer;
                public static bool AllowPasteDataInViewer => PasteDataInViewer != null;

                public static bool InvokePasteDataInViewer(StiReport report, StiPasteDataInViewerEventArgs args)
                {
                    if (PasteDataInViewer != null)
                    {
                        PasteDataInViewer(report, args);
                        return true;
                    }
                    else return false;
                }
                #endregion

                #region SavingDocumentInViewer
                public static event EventHandler SavingDocumentInViewer;
                public static bool InvokeSavingDocumentInViewer(StiReport report)
                {
                    if (SavingDocumentInViewer != null)
                    {
                        SavingDocumentInViewer(report, EventArgs.Empty);
                        return true;
                    }
                    else return false;
                }
                #endregion

                #region LoadingDocumentInViewer
                public static event EventHandler LoadingDocumentInViewer;
                public static bool InvokeLoadingDocumentInViewer(StiReport report)
                {
                    if (LoadingDocumentInViewer != null)
                    {
                        LoadingDocumentInViewer(report, EventArgs.Empty);
                        return true;
                    }
                    else return false;
                }
                #endregion

                #region DocumentLoadedInViewer
                public static event EventHandler DocumentLoadedInViewer;
                public static void InvokeDocumentLoadedInViewer(IStiViewerControl viewer)
                {
                    DocumentLoadedInViewer?.Invoke(viewer, EventArgs.Empty);
                }
                #endregion

                #region PrintingDocumentInViewer
                public static event EventHandler PrintingDocumentInViewer;
                public static bool InvokePrintingDocumentInViewer(StiReport report)
                {
                    if (PrintingDocumentInViewer != null)
                    {
                        PrintingDocumentInViewer(report, EventArgs.Empty);
                        return true;
                    }
                    else return false;
                }
                #endregion

				#region StartEditingInPreview
				public static event EventHandler StartEditingInPreview;

				public static void InvokeStartEditingInPreview(object sender, EventArgs e)
				{
                    StartEditingInPreview?.Invoke(sender, e);
                }
				#endregion

				#region EndEditingInPreview
				public static event EventHandler EndEditingInPreview;

				public static void InvokeEndEditingInPreview(object sender, EventArgs e)
				{
                    EndEditingInPreview?.Invoke(sender, e);
                }
				#endregion

				#region EnableEditorMode
				public static event EventHandler EnableEditorMode;

				public static void InvokeEnableEditorMode(object sender, EventArgs e)
				{
                    EnableEditorMode?.Invoke(sender, e);
                }
				#endregion

				#region DisableEditorMode
				public static event EventHandler DisableEditorMode;

				public static void InvokeDisableEditorMode(object sender, EventArgs e)
				{
                    DisableEditorMode?.Invoke(sender, e);
                }
				#endregion

                #region Click
                public static event EventHandler Click;

                public static void InvokeClick(object sender, EventArgs e)
                {
                    Click?.Invoke(sender, e);
                }
                #endregion

                #region MouseClick
                public static event MouseEventHandler MouseClick;

                public static void InvokeMouseClick(object sender, MouseEventArgs e)
                {
                    MouseClick?.Invoke(sender, e);
                }
                #endregion

                #region DoubleClick
                public static event EventHandler DoubleClick;

                public static void InvokeDoubleClick(object sender, EventArgs e)
                {
                    DoubleClick?.Invoke(sender, e);
                }
                #endregion

                #region HyperlinkClick
                public static event StiHyperlinkClickEventHandlers HyperlinkClick;
                public static bool InvokeHyperlinkClick(object viewer, string hyperlink)
                {
                    if (HyperlinkClick == null) return false;

                    var args = new StiHyperlinkClickEventArgs(hyperlink);
                    HyperlinkClick(viewer, args);
                    return args.Handled;
                }
                #endregion

                #region CheckDrillDownReport
                public static event StiCheckDrillDownReportEventHandler CheckDrillDownReport;

                public static void InvokeCheckDrillDownReport(object sender, StiCheckDrillDownReportEventArgs e)
                {
                    CheckDrillDownReport?.Invoke(sender, e);
                }
                #endregion

                #region GetDrillDownReport
                public static event StiGetDrillDownReportEventHandler GetDrillDownReport;
                public static void InvokeGetDrillDownReport(object sender, StiGetDrillDownReportEventArgs e)
                {
                    GetDrillDownReport?.Invoke(sender, e);
                }
                #endregion

                #region NewRequestFromUserDialogCreating
                public static event StiNewRequestFromUserDialogCreatingEventHandler NewRequestFromUserDialogCreating;

                public static void InvokeNewRequestFromUserDialogCreating(StiNewRequestFromUserDialogCreatingEventArgs e)
                {
                    NewRequestFromUserDialogCreating?.Invoke(null, e);
                }
                #endregion
            }
		}
    }
}