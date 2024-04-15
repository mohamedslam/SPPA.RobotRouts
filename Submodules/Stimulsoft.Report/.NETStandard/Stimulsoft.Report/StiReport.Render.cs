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
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Viewer;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Print;
using System.Threading.Tasks;
using Stimulsoft.Base.Plans;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if NETSTANDARD || NETCOREAPP
using System.Text;
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report
{
    public partial class StiReport
    {
        #region Methods
        /// <summary>
        /// Renders a report.
        /// </summary>
        public async Task<StiReport> RenderAsync()
        {
            return await Task.Run(() => Render(false));
        }

        /// <summary>
        /// Renders a report.
        /// </summary>
        public StiReport Render()
        {
            return Render(true);
        }

        /// <summary>
        /// Renders a report.
        /// </summary>
        /// <param name="showProgress">Whether it is necessary to show the progress of report rendering or not.</param>
        public StiReport Render(bool showProgress)
        {
            return Render(showProgress, -1, -1);
        }

        /// <summary>
        /// Renders a report.
        /// </summary>
        /// <param name="fromPage">Specifies from which page the result of the report rendering should be presented into the rendered report.</param>
        /// <param name="toPage">Specifies to which page the result of the report rendering should be presented into the rendered report.</param>
        public async Task<StiReport> RenderAsync(int fromPage, int toPage)
        {
            return await Task.Run(() => Render(false, fromPage, toPage));
        }

        /// <summary>
        /// Renders a report.
        /// </summary>
        /// <param name="showProgress">Whether it is necessary to show the progress of report rendering or not.</param>
        /// <param name="fromPage">Specifies from which page the result of the report rendering should be presented into the rendered report.</param>
        /// <param name="toPage">Specifies to which page the result of the report rendering should be presented into the rendered report.</param>
        public StiReport Render(bool showProgress, int fromPage, int toPage)
        {
            return Render(new StiRenderState(fromPage, toPage, showProgress));
        }

        /// <summary>
        /// Renders report.
        /// </summary>
        /// <param name="renderState">Specifies a parameters for the report rendering.</param>
        public async Task<StiReport> RenderAsync(StiRenderState renderState)
        {
            return await Task.Run(() => Render(renderState));
        }

        /// <summary>
        /// Renders report.
        /// </summary>
        /// <param name="renderState">Specifies a parameters for the report rendering.</param>
        public StiReport Render(StiRenderState renderState)
        {
            return Render(renderState, this.IsWpf ? StiGuiMode.Wpf : StiGuiMode.Gdi);
        }

        /// <summary>
        /// Renders a report.
        /// </summary>
        /// <param name="renderState">Specifies a parameters for the report rendering.</param>
        private StiReport Render(StiRenderState renderState, StiGuiMode guiMode)
        {
#if NETSTANDARD || NETCOREAPP
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif

            this.RenderedWith = StiRenderedWith.Net;
            if (guiMode == StiGuiMode.Wpf)
            {
                this.IsWpf = true;
                if (this.CompiledReport != null)
                {
                    this.CompiledReport.IsWpf = true;
                    this.CompiledReport.RenderedWith = StiRenderedWith.Wpf;
                }
                StiOptions.Configuration.IsWPF = true;
                this.RenderedWith = StiRenderedWith.Wpf;
            }

            this.bookmarkValue.Engine = this.EngineVersion;
            this.Bookmark.Engine = this.EngineVersion;
            this.IsEditedInViewer = false;

            if (StiOptions.Engine.HideRenderingProgress) 
                renderState.ShowProgress = false;

            if (StiOptions.Engine.HideMessages) 
                renderState.ShowProgress = false;

            if (StiOptions.Configuration.IsWeb) 
                renderState.ShowProgress = false;

            #region Render Report with Progress
            if (renderState.ShowProgress && this.ReportPass != StiReportPass.First && (!renderState.IsSubReportMode))
            {
                BackgroundWorker worker = null;
                try
                {
                    if (StiOptions.Engine.AllowProgressInThread || UseProgressInThread)
                    {
                        worker = new BackgroundWorker();
                        worker.WorkerSupportsCancellation = true;
                        worker.DoWork += RenderWorker_DoWork;
                        worker.RunWorkerAsync(renderState);
                    }

                    try
                    {
                        if (!(StiOptions.Engine.AllowProgressExternalAccess && Progress != null))
                        {
                            Progress = this.Designer is Form
                                ? StiGuiOptions.GetProgressInformation(this.Designer as Form, guiMode)
                                : StiGuiOptions.GetProgressInformation(guiMode);
                        }
                    }
                    catch
                    {
                        Progress = null;
                    }

                    if (Progress != null)
                        Progress.IsMarquee = true;

                    if (StiOptions.Engine.AllowProgressInThread || UseProgressInThread)
                    {
                        if (Progress != null)
                            Progress.AllowUseDoEvents = false;
                    }

                    if (Progress != null)
                    {
                        Progress.HideProgressBar();
                        Progress.SetAllowClose(false);

                        Progress.Start(StiLocalization.CultureName == "en"
                            ? "Preparing Report"
                            : StiLocalization.Get("Report", "PreparingReport"));
                    }

                    if (worker != null)
                    {
                        while (worker.IsBusy)
                        {
                            Application.DoEvents();
                        }
                    }
                    else
                    {
                        RenderReport(renderState);
                    }

                }
                finally
                {
                    worker?.Dispose();

                    #region Close progress
                    if (Progress != null && this.ReportPass != StiReportPass.First && (!renderState.IsSubReportMode))
                    {
                        Progress.Close();

                        if (!StiOptions.Engine.AllowProgressExternalAccess)
                            Progress = null;

                        StatusString = "";
                        
                        if (CompiledReport != null) 
                            CompiledReport.Progress = null;
                    }
                    #endregion
                }

            }
            #endregion

            #region Render Report without Progress
            else
            {
                RenderReport(renderState);
            }
            #endregion

            if (this.ReportPass != StiReportPass.First)
            {
                this.IsWpf = false;

                if (this.CompiledReport != null)
                    this.CompiledReport.IsWpf = false;
            }

            return this;
        }

        private void RenderWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var renderState = e.Argument as StiRenderState;
            e.Result = renderState;

            RenderReport(renderState);
        }

        /// <summary>
        /// Renders report.
        /// </summary>
        /// <param name="renderState">Specifies a parameters for the report rendering.</param>
        private void RenderReport(StiRenderState renderState)
        {
            if (CalculationMode == StiCalculationMode.Interpretation)
                ProcessAutoLocalizeReportOnRun();

#if CLOUD
            StiCloudReport.ResetDataRows(this.ReportGuid); //reset cloud data rows counter
#endif

            var oldMode = CalculationMode;
            CheckNeedsCompiling();  //for CheckNeedForceInterpretationMode
            if (StiOptions.Engine.ForceInterpretationMode && (NeedsCompiling || IsRendered))
                CalculationMode = StiCalculationMode.Interpretation;

            if (NeedsCompiling && CalculationMode == StiCalculationMode.Compilation && !StiOptions.Engine.FullTrust)
                CalculationMode = StiCalculationMode.Interpretation;

            #region Set culture
            var storedCulture = Thread.CurrentThread.CurrentCulture;
            var culture = GetParsedCulture();
            if (!string.IsNullOrEmpty(culture))
            {
                try
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(culture, false);
                }
                catch
                {
                }
            }
            #endregion

            var storeInteraction = new Hashtable();

            try
            {
                if (CachedTotals != null)
                    CachedTotals.Clear();
                else
                    CachedTotals = new Hashtable();

                #region Fix problem with rerender report and page numbers
                if (Engine != null && Engine.PageNumbers != null)
                    Engine.PageNumbers.ClearNotFixed();
                #endregion

                if (CalculationMode == StiCalculationMode.Interpretation)
                {
                    UpdateReportVersion();
                    if (Variables == null) 
                        StiParser.PrepareReportVariables(this);
                }

                #region Store DataSources SqlCommand for Interpretation mode
                if (CalculationMode == StiCalculationMode.Interpretation && this.ReportPass != StiReportPass.First && !renderState.IsSubReportMode)
                {
                    foreach (StiDataSource ds in Dictionary.DataSources)
                    {
                        var sqlSource = ds as StiSqlSource;
                        if (sqlSource != null)                        
                            Variables[$"**StoredDataSourceSqlCommandForInterpretationMode**{ds.Name}"] = sqlSource.SqlCommand;
                    }
                }
                #endregion

                #region Store Interaction information and Process GlobalizedName of Images
                if (CalculationMode == StiCalculationMode.Interpretation)
                {
                    var comps = GetComponents();
                    foreach (StiComponent comp in comps)
                    {
                        if (comp.Interaction != null && comp.Interaction.IsDefault)
                        {
                            storeInteraction[comp] = comp.Interaction;
                            comp.Interaction = null;
                        }

                        if ((comp is StiImage) && (GlobalizationManager != null))
                        {
                            var globalizedNameComp = comp as IStiGlobalizedName;
                            if ((globalizedNameComp != null) && (!string.IsNullOrWhiteSpace(globalizedNameComp.GlobalizedName)))
                            {
                                var newImage = GlobalizationManager.GetObject(globalizedNameComp.GlobalizedName);
                                if (newImage is byte[]) 
                                    (comp as StiImage).PutImage(newImage as byte[]);
                                
                                if (newImage is Image)
                                    (comp as StiImage).PutImage(newImage as Image);
                            }
                        }
                    }
                }
                #endregion

                #region Render SubReports
                if (this.SubReports != null && this.SubReports.Count > 0)
                {
                    if (EngineVersion == StiEngineVersion.EngineV1)
                        StiReportV1Builder.RenderSubReports(this, renderState);
                    else
                        StiReportV2Builder.RenderSubReports(this, renderState);
                }
                #endregion

                #region Render Single Report
                else
                {
                    if (EngineVersion == StiEngineVersion.EngineV1)
                        StiReportV1Builder.RenderSingleReport(this, renderState);
                    else
                        StiReportV2Builder.RenderSingleReport(this, renderState);
                }
                #endregion
            }
            finally
            {
                #region Restore Interaction information
                if (CalculationMode == StiCalculationMode.Interpretation)
                {
                    foreach (DictionaryEntry de in storeInteraction)
                    {
                        var comp = (StiComponent)de.Key;
                        comp.Interaction = (StiInteraction)de.Value;
                    }
                    storeInteraction.Clear();
                }
                #endregion

                #region Restore DataSources SqlCommand for Interpretation mode
                if ((CalculationMode == StiCalculationMode.Interpretation) && (this.ReportPass != StiReportPass.First) && (!renderState.IsSubReportMode))
                {
                    foreach (StiDataSource ds in Dictionary.DataSources)
                    {
                        var sqlSource = ds as StiSqlSource;
                        if (sqlSource == null) continue;

                        var sqlCommandKey = $"**StoredDataSourceSqlCommandForInterpretationMode**{ds.Name}";
                        if (Variables.ContainsKey(sqlCommandKey))
                        {
                            var sqlCommand = Variables[sqlCommandKey];
                            if (sqlCommand is string)
                                sqlSource.SqlCommand = sqlCommand as string;

                            Variables.Remove(sqlCommandKey);
                        }
                    }
                }
                #endregion

                CalculationMode = oldMode;
                if (!string.IsNullOrEmpty(GetParsedCulture()))
                    Thread.CurrentThread.CurrentCulture = storedCulture;

                if (CachedTotals != null)
                {
                    CachedTotals.Clear();
                    CachedTotals = null;
                }

                if ((ReportPass == StiReportPass.First) && (Anchors != null))
                    Anchors.Clear();

                foreach (StiBusinessObject bo in Dictionary.BusinessObjects)
                {
                    bo.previousResetException = false;
                }

                if (WpfRichTextDomain != null)
                {
                    AppDomain.Unload(WpfRichTextDomain);
                    WpfRichTextDomain = null;
                }

                if (CompiledReport != null && CompiledReport.WpfRichTextDomain != null)
                {
                    AppDomain.Unload(CompiledReport.WpfRichTextDomain);
                    CompiledReport.WpfRichTextDomain = null;
                }

                RenderedPages.Flush(true);
            }

            IsDocument = false;
        }

        /// <summary>
        /// Resets a report to null state.
        /// </summary>
        internal void Clear()
        {
            Clear(true);
        }

        /// <summary>
        /// Resets a report to null state.
        /// </summary>
        protected internal virtual void Clear(bool generateNewScript)
        {
            this.Pages.Clear();
            this.RenderedPages.Clear();

            this.RenderingEvent = new StiRenderingEvent();
            this.BeginRenderEvent = new StiBeginRenderEvent();
            this.EndRenderEvent = new StiEndRenderEvent();

            this.PrintingEvent = new StiPrintingEvent();
            this.PrintedEvent = new StiPrintedEvent();
            this.ExportedEvent = new StiExportedEvent();
            this.ExportingEvent = new StiExportingEvent();

            this.ScriptLanguage = StiOptions.Engine.DefaultReportLanguage;

            if (StiLocalization.CultureName == "en")
            {
                this.ReportName = "Report";
                this.ReportAlias = "Report";
            }
            else
            {
                this.ReportName = StiLocalization.Get("Components", "StiReport");
                this.ReportAlias = StiLocalization.Get("Components", "StiReport");
            }

            this.ReportImage = null;
            this.ReportIcon = null;

            this.ReportDescription = string.Empty;
            this.ReportAuthor = string.Empty;

            if (StiOptions.Engine.DefaultUnit is StiReportUnitType)
            {
                this.ReportUnit = (StiReportUnitType)StiOptions.Engine.DefaultUnit;
            }
            else
            {
                this.ReportUnit = RegionInfo.CurrentRegion.IsMetric
                    ? StiReportUnitType.Centimeters
                    : StiReportUnitType.Inches;
            }

            ReferencedAssemblies = StiOptions.Engine.ReferencedAssemblies ?? new[]
            {
                "System.Dll",
                "System.Drawing.Dll",
                "System.Windows.Forms.Dll",
                "System.Data.Dll",
                "System.Xml.Dll",
                "Stimulsoft.Controls.Dll",
                "Stimulsoft.Base.Dll",
                "Stimulsoft.Report.Dll"
            };

            this.ScriptLanguage = StiOptions.Engine.DefaultReportLanguage;

            this.Styles.Clear();
            this.AutoLocalizeReportOnRun = false;
            this.CacheAllData = false;
            this.ConvertNulls = true;
            this.Collate = 1;
            this.Culture = string.Empty;
            this.GlobalizationStrings.Clear();
            this.NumberOfPass = StiNumberOfPass.SinglePass;
            this.PreviewMode = StiPreviewMode.Standard;
            this.DashboardViewerSettings = StiDashboardViewerSettings.All;
            this.PreviewSettings = (int)(StiPreviewSettings.Default);
            this.HtmlPreviewMode = StiHtmlPreviewMode.Div;
            this.PrinterSettings = new StiPrinterSettings();
            this.ReportCacheMode = StiReportCacheMode.Off;
            this.StopBeforePage = 0;
            this.StopBeforeTime = 0;
            this.IsEditedInViewer = false;
            this.MetaTags.Clear();

            this.CalculationMode = StiCalculationMode.Compilation;
            this.ParametersOrientation = StiOrientation.Horizontal;
            this.RequestParameters = false;
            this.SaveReportInResources = true;
            this.RefreshTime = 0;

            if (generateNewScript && StiOptions.Engine.FullTrust) 
                this.ScriptNew();

            ResetRenderedState();
            ResetAggregateFunctions();

            if (CompiledReport != null)
                CompiledReport.ResetAggregateFunctions();

            if (HashViewWpfPainter != null)
            {
                HashViewWpfPainter.Clear();
                HashViewWpfPainter = null;
            }
        }

        /// <summary>
        /// Resets states of aggregate functions in compiled report.
        /// </summary>
        public StiReport ResetAggregateFunctions()
        {
            if (this.AggregateFunctions == null)
                return this;

            foreach (StiAggregateFunctionService service in this.AggregateFunctions)
            {
                var isFirstInit = service.IsFirstInit;
                service.IsFirstInit = true;
                service.Init();
                service.IsFirstInit = isFirstInit;
            }

            return this;
        }

        /// <summary>
        /// Resets a rendered states.
        /// </summary>
        public StiReport ResetRenderedState()
        {
            IsRendered = false;
            CompiledReport = null;
            CompilerResults = null;
            RenderedPages.Clear();

            return this;
        }

        /// <summary>
        /// Recalculate segments in all rendered pages
        /// </summary>
        public StiReport RecalculateRenderedPagesSegments()
        {
            foreach (StiPage page in RenderedPages)
            {
                if (page.SegmentPerWidth > 1 || page.SegmentPerHeight > 1)
                {
                    RenderedPages.GetPage(page);
                    double maxX = 0;
                    double maxY = 0;
                    foreach (StiComponent comp in page.Components)
                    {
                        if (comp.Right > maxX) 
                            maxX = comp.Right;

                        if (comp.Bottom > maxY) 
                            maxY = comp.Bottom;
                    }
                    
                    var newSegmentPerWidth = 1;
                    while ((decimal)((page.PageWidth - page.Margins.Left - page.Margins.Right) * newSegmentPerWidth) < (decimal)maxX)
                    {
                        newSegmentPerWidth++;
                    }
                    
                    var newSegmentPerHeight = 1;
                    while ((decimal)((page.PageHeight - page.Margins.Top - page.Margins.Bottom) * newSegmentPerHeight) < (decimal)maxY)
                    {
                        newSegmentPerHeight++;
                    }

                    page.SegmentPerWidth = newSegmentPerWidth;
                    page.SegmentPerHeight = newSegmentPerHeight;
                }
            }

            return this;
        }
        #endregion

        #region Methods.WPF
        /// <summary>
        /// Renders a report with using WPF technology.
        /// </summary>
        public StiReport RenderWithWpf()
        {
            return RenderWithWpf(true);
        }
        
        /// <summary>
        /// Renders a report with using WPF technology.
        /// </summary>
        /// <param name="showProgress">Whether it is necessary to show the progress of report rendering or not.</param>
        public StiReport RenderWithWpf(bool showProgress)
        {
            return RenderWithWpf(showProgress, -1, -1);
        }

        /// <summary>
        /// Renders a report with using WPF technology.
        /// </summary>
        /// <param name="showProgress">Whether it is necessary to show the progress of report rendering or not.</param>
        public StiReport RenderWithWpf(bool showProgress, int fromPage, int toPage)
        {
            return RenderWithWpf(new StiRenderState(fromPage, toPage, showProgress));
        }

        /// <summary>
        /// Renders a report with using WPF technology.
        /// </summary>
        public StiReport RenderWithWpf(StiRenderState renderState)
        {
            return Render(renderState, StiGuiMode.Wpf);
        }
        #endregion
    }
}