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

using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Stimulsoft.Report.Engine
{
    public class StiProgressHelperV2
    {
        #region Fields
        private List<StiDataBand> cachedDataBands;
        private Hashtable cachedDataBandsValues;
        private Hashtable cachedPages;
        private decimal latestProgress;
        private decimal latestProgressValue;
        #endregion

        #region Properties
        internal StiEngine Engine { get; }

        private string FinishingReportString
        {
            get
            {
                return StiLocalization.CultureName == "en" 
                    ? "Finishing Report" 
                    : StiLocalization.Get("Report", "FinishingReport");
            }
        }

        private string FirstPassString
        {
            get
            {
                return StiLocalization.CultureName == "en" 
                    ? "First Pass" 
                    : StiLocalization.Get("Report", "FirstPass");
            }
        }

        private string SecondPassString
        {
            get
            {
                return StiLocalization.CultureName == "en" 
                    ? "Second Pass" 
                    : StiLocalization.Get("Report", "SecondPass");
            }
        }

        public bool AllowCachedPagesCache { get; set; }
        #endregion

        #region Methods
        internal void Dispose()
        {
            if (cachedDataBands != null)
                cachedDataBands.Clear();
            
            cachedDataBands = null;

            if (cachedDataBandsValues != null)
                cachedDataBandsValues.Clear();

            cachedDataBandsValues = null;

            if (cachedPages != null)
                cachedPages.Clear();

            cachedPages = null;
            
        }

        private void ScanReport()
        {
            if (Engine == null || cachedDataBands != null) return;

            var report = Engine.Report;

            if (report == null) return;

            this.cachedDataBands = new List<StiDataBand>();
            this.cachedDataBandsValues = new Hashtable();

            foreach (StiPage page in report.Pages)
            {
                if (page.Skip) continue;
                var comps = page.GetComponents();
                foreach (StiComponent comp in comps)
                {
                    var dataBand = comp as StiDataBand;
                    if (dataBand != null && dataBand.MasterComponent == null)
                    {
                        cachedDataBands.Add(dataBand);
                        cachedDataBandsValues[dataBand] = 0;
                    }
                }
            }
        }

        internal void ProcessInCache(StiPage page)
        {
            if (!AllowCachedPagesCache)
                return;

            if (page == null || page.Report == null || page.Report.RenderedPages == null)
                return;

            if (cachedPages == null)
                cachedPages = new Hashtable();

            var report = Engine.Report;

            cachedPages[page] = page;

            var progressValue = -1;

            decimal count = page.Report.RenderedPages.Count;
            decimal value = cachedPages.Count;
            value = value / count;
            value = value * (100 - latestProgress) + latestProgress;
            report.ProgressOfRendering = (int)value;
            progressValue = (int)value;

            #region Update Status String
            report.StatusString = $"{FinishingReportString}: {100 * cachedPages.Count / page.Report.RenderedPages.Count}%";
            #endregion

            UpdateReportProgress(report, progressValue);
            CheckProgressBreak(report);
            UpdateParentReportProgress(report);
        }

        internal void Process()
        {
            if (AllowCachedPagesCache)
                return;

            var report = Engine.Report;
            if (report.CompiledReport != null)report = report.CompiledReport;

            var progressValue = -1;

            #region Report passes
            var pass = string.Empty;
            if (report.ReportPass == StiReportPass.First)
                pass = this.FirstPassString;

            if (report.ReportPass == StiReportPass.Second)
                pass = this.SecondPassString;
            #endregion

            #region Calculate progress of report rendering
            if (StiOptions.Engine.UsePercentageProgress)
            {                
                decimal count = 0;
                decimal value = 0;

                ScanReport();

                if (this.cachedDataBands != null)
                {
                    foreach (var dataBand in this.cachedDataBands)
                    {
                        count += dataBand.Count;
                        var addValue = dataBand.Position;
                        if (dataBand.DataSource != null)
                        {
                            if (dataBand.DataSource.NameOfDataBandWhichInitDataSource == dataBand.Name)
                                cachedDataBandsValues[dataBand] = addValue;

                            else
                            {
                                var obj = cachedDataBandsValues[dataBand];
                                if (obj is int)
                                    addValue = (int) obj;
                            }
                        }

                        if (addValue <= dataBand.Count)
                            value += addValue;
                        else
                            value += dataBand.Count;
                    }
                }

                if (count > 0)
                {
                    //Добавляем дополнительное место на прогрессбаре для обработки кэша
                    if (report.ReportCacheMode == StiReportCacheMode.On)
                        count = (int)((float)count * 1.3);

                    if (report.NumberOfPass == StiNumberOfPass.DoublePass)
                        count *= 2;

                    value = value / count * 100;                    
                }
                else
                    value = 100;

                value = Math.Max(latestProgressValue, value);
                latestProgressValue = value;

                if (report.ReportPass != StiReportPass.Second)
                    Engine.LatestProgressValue = value;
                else
                    value += Engine.LatestProgressValue;

                this.latestProgress = value;

                progressValue = (int)value;
                report.ProgressOfRendering = progressValue;
            }
            #endregion

            #region Status String & PageNofM
            var statusStringBase = 
                StiLocalization.CultureName == "en" 
                    ? $"Page - {report.CurrentPrintPage + 1}"
                    : $"{StiLocalization.Get("Components", "StiPage")} - {report.CurrentPrintPage + 1}";

            report.StatusString = report.ReportPass != StiReportPass.None 
                ? $"{pass}: {statusStringBase}" 
                : statusStringBase;
            #endregion
            
            UpdateReportProgress(report, progressValue);
            CheckProgressBreak(report);
            UpdateParentReportProgress(report);
        }

        /// <summary>
        /// Throwes StiReportRenderingStopException exception if the user press break button in report progress form
        /// </summary>
        private static void CheckProgressBreak(StiReport report)
        {
            if (report.Progress == null || !report.Progress.IsBreaked) return;

            report.IsStopped = true;
            throw new StiReportRenderingStopException();
        }
        
        private static void UpdateReportProgress(StiReport report, int progressValue)
        {
            if (report.Progress == null) return;

            if (progressValue != -1)
            {
                //fix
                if (progressValue > 100)
                    progressValue = 100;

                report.Progress.IsMarquee = false;
                report.Progress.Update(report.StatusString, progressValue);
            }
            else
                report.Progress.Update(report.StatusString);
        }

        private static void UpdateParentReportProgress(StiReport report)
        {
            if (report.ParentReport == null || report.ParentReport.Progress == null) return;

            report.ParentReport.Progress.Update(report.ParentReport.StatusString);

            if (report.ParentReport.Progress.IsBreaked)
            {
                report.ParentReport.IsStopped = true;
                throw new StiReportRenderingStopException();
            }
        }
        #endregion

        internal StiProgressHelperV2(StiEngine engine)
        {
            this.Engine = engine;
        }
    }
}
