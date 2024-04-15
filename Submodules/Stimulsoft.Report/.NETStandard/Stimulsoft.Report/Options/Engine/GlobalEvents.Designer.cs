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
using System.ComponentModel;
using System.Data;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Design;
using Stimulsoft.Report.Events;

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
                #region AllowCutOperation
                public static event StiAllowClipboardOperationEventHandler AllowCutOperation;

                public static void InvokeAllowCutOperation(object sender, StiAllowClipboardOperationEventArgs e)
                {
                    if (AllowCutOperation != null) AllowCutOperation(sender, e);
                }
                #endregion

                #region AllowCopyOperation
                public static event StiAllowClipboardOperationEventHandler AllowCopyOperation;

                public static void InvokeAllowCopyOperation(object sender, StiAllowClipboardOperationEventArgs e)
                {
                    if (AllowCopyOperation != null) AllowCopyOperation(sender, e);
                }
                #endregion

                #region AllowPasteOperation
                public static event StiAllowClipboardOperationEventHandler AllowPasteOperation;

                public static void InvokeAllowPasteOperation(object sender, StiAllowClipboardOperationEventArgs e)
                {
                    if (AllowPasteOperation != null) AllowPasteOperation(sender, e);
                }
                #endregion

                #region Pasted
                public static event StiPastedEventHandler Pasted;

                public static void InvokePasted(object sender, StiPastedEventArgs e)
                {
                    if (Pasted != null) Pasted(sender, e);
                }
                #endregion

                #region ImportXmlSchema
                public class StiImportXmlSchemaEventArg : EventArgs
                {
                    public DataSet DataSet { get; }

                    public StiImportXmlSchemaEventArg(DataSet dataSet)
                    {
                        this.DataSet = dataSet;
                    }
                }

                public delegate void StiImportXmlSchemaEventHandler(object sender, StiImportXmlSchemaEventArg e);

                public static event StiImportXmlSchemaEventHandler ImportXmlSchema;

                public static void InvokeImportXmlSchema(object sender, StiImportXmlSchemaEventArg e)
                {
                    if (ImportXmlSchema != null) ImportXmlSchema(sender, e);
                }
                #endregion                

                #region SavingStylesInDesigner
                public static event StiSavingStylesEventHandler SavingStylesInDesigner;

                public static bool InvokeSavingStylesInDesigner(IStiDesignerBase designer, StiStylesCollection styles)
                {
                    if (SavingStylesInDesigner != null)
                    {
                        var e = new StiSavingStylesEventArgs(styles);
                        SavingStylesInDesigner(designer, e);
                        return e.Processed;
                    }
                    else return false;
                }
                #endregion

                #region LoadingStylesInDesigner
                public static event StiLoadingStylesEventHandler LoadingStylesInDesigner;

                public static bool InvokeLoadingStylesInDesigner(IStiDesignerBase designer, StiStylesCollection styles)
                {
                    if (LoadingStylesInDesigner != null)
                    {
                        var e = new StiLoadingStylesEventArgs(styles);
                        LoadingStylesInDesigner(designer, e);
                        return e.Processed;
                    }
                    else return false;
                }
                #endregion

                #region DragDropToPageInDesigner
                public static event DragEventHandler DragDropToPageInDesigner;

                public static void InvokeDragDropToPageInDesigner(IStiDesignerBase designer, DragEventArgs e)
                {
                    if (DragDropToPageInDesigner != null) DragDropToPageInDesigner(designer, e);
                }
                #endregion

                #region DragEnterToPageInDesigner
                public static event DragEventHandler DragEnterToPageInDesigner;

                public static void InvokeDragEnterToPageInDesigner(IStiDesignerBase designer, DragEventArgs e)
                {
                    if (DragEnterToPageInDesigner != null) DragEnterToPageInDesigner(designer, e);
                }
                #endregion

                #region DragLeaveFromPageInDesigner
                public static event EventHandler DragLeaveFromPageInDesigner;

                public static void InvokeDragLeaveFromPageInDesigner(IStiDesignerBase designer, EventArgs e)
                {
                    if (DragLeaveFromPageInDesigner != null) DragLeaveFromPageInDesigner(designer, e);
                }
                #endregion

                #region DragOverPageInDesigner
                public static event DragEventHandler DragOverPageInDesigner;

                public static void InvokeDragOverPageInDesigner(IStiDesignerBase designer, DragEventArgs e)
                {
                    if (DragOverPageInDesigner != null) DragOverPageInDesigner(designer, e);
                }
                #endregion

                #region QueryContinueDragOverPageInDesigner
                public static event QueryContinueDragEventHandler QueryContinueDragOverPageInDesigner;

                public static void InvokeQueryContinueDragOverPageInDesigner(IStiDesignerBase designer, QueryContinueDragEventArgs e)
                {
                    if (QueryContinueDragOverPageInDesigner != null) QueryContinueDragOverPageInDesigner(designer, e);
                }
                #endregion

                #region OpenRecentFileInDesigner
                public static event StiOpenRecentFileObjectEventHandler OpenRecentFileInDesigner;
                public static bool InvokeOpenRecentFileInDesigner(IStiDesignerBase designer, StiRecentFile recentFile)
                {
                    if (OpenRecentFileInDesigner != null)
                    {
                        var e = new StiOpenRecentFileObjectEventArgs(recentFile);
                        OpenRecentFileInDesigner(designer, e);
                        return e.Processed;
                    }
                    else return false;
                }
                #endregion
                
                #region DisplayRecentFileInDesigner
                public static event StiDisplayRecentFileObjectEventHandler DisplayRecentFileInDesigner;
                public static StiDisplayRecentFileObjectEventArgs InvokeDisplayRecentFileInDesigner(IStiDesignerBase designer, StiRecentFile recentFile)
                {
                    var e = new StiDisplayRecentFileObjectEventArgs(recentFile);
                    if (DisplayRecentFileInDesigner != null)
                    {                        
                        DisplayRecentFileInDesigner(designer, e);
                    }
                    return e;
                }
                #endregion

                #region LoadingDesigner
                public static event EventHandler LoadingDesigner;
                public static void InvokeLoadingDesigner(IStiDesignerBase designer, EventArgs e)
                {
                    if (LoadingDesigner != null) LoadingDesigner(designer, e);
                }
                #endregion

                #region LoadedDesigner
                public static event EventHandler LoadedDesigner;
                public static void InvokeLoadedDesigner(IStiDesignerBase designer, EventArgs e)
                {
                    if (LoadedDesigner != null) LoadedDesigner(designer, e);
                }
                #endregion

                #region SaveChangesInDialogBoxDesigner
                public static event StiSaveChangesInDialogBoxDesignerEventHandler SaveChangesInDialogBoxDesigner;
                public static void InvokeSaveChangesInDialogBoxDesigner(IStiDesignerBase designer, StiSaveChangesInDialogBoxDesignerEvenArgs e)
                {
                    if (SaveChangesInDialogBoxDesigner != null) SaveChangesInDialogBoxDesigner(designer, e);
                }
                #endregion

				#region LoadingConfigurationInDesigner
				public static event EventHandler LoadingConfigurationInDesigner;
				public static void InvokeLoadingConfigurationInDesigner(IStiDesignerBase designer, EventArgs e)
				{
					if (LoadingConfigurationInDesigner != null) LoadingConfigurationInDesigner(designer, e);
				}
				#endregion

				#region LoadedConfigurationInDesigner
				public static event EventHandler LoadedConfigurationInDesigner;
				public static void InvokeLoadedConfigurationInDesigner(IStiDesignerBase designer, EventArgs e)
				{
					if (LoadedConfigurationInDesigner != null) LoadedConfigurationInDesigner(designer, e);
				}
				#endregion

				#region SavingConfigurationInDesigner
				public static event EventHandler SavingConfigurationInDesigner;
				public static void InvokeSavingConfigurationInDesigner(IStiDesignerBase designer, EventArgs e)
				{
					if (SavingConfigurationInDesigner != null) SavingConfigurationInDesigner(designer, e);
				}
				#endregion

				#region SavedConfigurationInDesigner
				public static event EventHandler SavedConfigurationInDesigner;
				public static void InvokeSavedConfigurationInDesigner(IStiDesignerBase designer, EventArgs e)
				{
					if (SavedConfigurationInDesigner != null) SavedConfigurationInDesigner(designer, e);
				}
				#endregion

                #region ClosingDesigner
                public static event CancelEventHandler ClosingDesigner;
                public static void InvokeClosingDesigner(IStiDesignerBase designer, CancelEventArgs e)
                {
                    if (ClosingDesigner != null) ClosingDesigner(designer, e);
                }
                #endregion

                #region ClosedDesigner
                public static event EventHandler ClosedDesigner;
                public static void InvokeClosedDesigner(IStiDesignerBase designer, EventArgs e)
                {
                    if (ClosedDesigner != null) ClosedDesigner(designer, e);
                }
                #endregion
                
                #region ReportChangedInDesigner
                public static event EventHandler ReportChangedInDesigner;
                public static void InvokeReportChangedInDesigner(IStiDesignerBase designer, EventArgs e)
                {
                    if (ReportChangedInDesigner != null) ReportChangedInDesigner(designer, e);
                }
                #endregion

				#region DictionaryMenuActionsOpenInDesigner
				public static event EventHandler DictionaryMenuActionsOpenInDesigner;

				public static void InvokeDictionaryMenuActionsOpenInDesigner(object sender)
				{
					if (DictionaryMenuActionsOpenInDesigner != null) DictionaryMenuActionsOpenInDesigner(sender, EventArgs.Empty);
				}
				#endregion

				#region DictionaryMenuNewOpenInDesigner
				public static event EventHandler DictionaryMenuNewOpenInDesigner;

				public static void InvokeDictionaryMenuNewOpenInDesigner(object sender)
				{
					if (DictionaryMenuNewOpenInDesigner != null) DictionaryMenuNewOpenInDesigner(sender, EventArgs.Empty);
				}
				#endregion

				#region DictionaryMenuEditOpenInDesigner
				public static event EventHandler DictionaryMenuEditOpenInDesigner;

				public static void InvokeDictionaryMenuEditOpenInDesigner(object sender)
				{
					if (DictionaryMenuEditOpenInDesigner != null) DictionaryMenuEditOpenInDesigner(sender, EventArgs.Empty);
				}
				#endregion

                #region SavingReportInDesigner
                public static event StiSavingObjectEventHandler SavingReportInDesigner;

                public static bool InvokeSavingReportInDesigner(IStiDesignerBase designer, bool saveAs)
                {
                    return InvokeSavingReportInDesigner(designer, saveAs, StiSaveEventSource.Underfined);
                }

                public static bool InvokeSavingReportInDesigner(IStiDesignerBase designer, bool saveAs, StiSaveEventSource eventSource)
                {
                    if (SavingReportInDesigner != null)
                    {
                        var e = new StiSavingObjectEventArgs(saveAs, eventSource);
                        SavingReportInDesigner(designer, e);
                        return e.Processed;
                    }
                    else return false;
                }
                #endregion

                #region SavedReportInDesigner
                public static event EventHandler SavedReportInDesigner;

                public static void InvokeSavedReportInDesigner(IStiDesignerBase designer)
                {
                    if (SavedReportInDesigner != null) SavedReportInDesigner(designer, EventArgs.Empty);
                }
                #endregion

                #region LoadingReportInDesigner
                public static event StiLoadingObjectEventHandler LoadingReportInDesigner;
                public static bool InvokeLoadingReportInDesigner(IStiDesignerBase designer)
                {
                    if (LoadingReportInDesigner != null)
                    {
                        var e = new StiLoadingObjectEventArgs();
                        LoadingReportInDesigner(designer, e);
                        return e.Processed;                        
                    }
                    else return false;
                }
                #endregion

                #region LoadedReportInDesigner
                public static event EventHandler LoadedReportInDesigner;
                public static void InvokeLoadedReportInDesigner(IStiDesignerBase designer)
                {
                    if (LoadedReportInDesigner != null) LoadedReportInDesigner(designer, EventArgs.Empty);
                }
                #endregion

                #region CreatingReportInDesigner
                public static event StiCreatingObjectEventHandler CreatingReportInDesigner;
                public static bool InvokeCreatingReportInDesigner(IStiDesignerBase designer)
                {
                    if (CreatingReportInDesigner != null)
                    {
                        var e = new StiCreatingObjectEventArgs();
                        CreatingReportInDesigner(designer, e);
                        return e.Processed;
                    }
                    else return false;
                }
                #endregion

                #region CreatedReportInDesigner
                public static event EventHandler CreatedReportInDesigner;
                public static void InvokeCreatedReportInDesigner(IStiDesignerBase designer)
                {
                    if (CreatedReportInDesigner != null) CreatedReportInDesigner(designer, EventArgs.Empty);
                }
                #endregion

                #region SavingPageInDesigner
                public static event EventHandler SavingPageInDesigner;
                public static bool InvokeSavingPageInDesigner(IStiDesignerBase designer)
                {
                    if (SavingPageInDesigner != null)
                    {
                        SavingPageInDesigner(designer, EventArgs.Empty);
                        return true;
                    }
                    return false;
                }
                #endregion

                #region LoadingPageInDesigner
                public static event EventHandler LoadingPageInDesigner;
                public static bool InvokeLoadingPageInDesigner(IStiDesignerBase designer)
                {
                    if (LoadingPageInDesigner != null)
                    {
                        LoadingPageInDesigner(designer, EventArgs.Empty);
                        return true;
                    }
                    return false;
                }
                #endregion

                #region PreviewingReportInDesigner
                public static event EventHandler PreviewingReportInDesigner;
                public static bool InvokePreviewingReportInDesigner(IStiDesignerBase designer)
                {
                    if (PreviewingReportInDesigner != null)
                    {
                        PreviewingReportInDesigner(designer, EventArgs.Empty);
                        return true;
                    }
                    else return false;
                }
                #endregion

                #region DragDropFromDictionary
                public static event DragDropFromDictionaryEventHandler DragDropFromDictionary;

                public static void InvokeDragDropFromDictionary(object sender, StiDragDropFromDictionaryEventArgs e)
                {
                    if (DragDropFromDictionary != null) DragDropFromDictionary(sender, e);
                }
                #endregion

                #region WizardStart
                public static event EventHandler WizardStart;

                public static void InvokeWizardStart(object sender, EventArgs e)
                {
                    if (WizardStart != null) WizardStart(sender, e);
                }
                #endregion

                #region WizardFinish
                public static event EventHandler WizardFinish;

                public static void InvokeWizardFinish(object sender, EventArgs e)
                {
                    if (WizardFinish != null) WizardFinish(sender, e);
                }
                #endregion

                #region RunPageSetupInDesigner
                public static event EventHandler RunPageSetupInDesigner;
                public static bool InvokeRunPageSetupInDesigner(StiPage page)
                {
                    if (RunPageSetupInDesigner == null)return false;
                    else
                    {
                        RunPageSetupInDesigner(page, EventArgs.Empty);
                        return true;
                    }
                }
                #endregion

                #region CheckDataBandNameCreatedInWizard
                public static event StiCheckDataBandNameCreatedInWizardHandler CheckDataBandNameCreatedInWizard;
                public static void InvokeCheckDataBandNameCreatedInWizard(StiCheckDataBandNameCreatedInWizardArgs checkArgs)
                {
                    if (CheckDataBandNameCreatedInWizard != null) CheckDataBandNameCreatedInWizard(null, checkArgs);
                }
                #endregion
            }
		}
    }
}