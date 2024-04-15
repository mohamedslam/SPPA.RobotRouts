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

using Stimulsoft.Report.Components;
using System.Collections;
using System.Drawing.Printing;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class helps in resizing existing report with specified parameters.
    /// </summary>
    public sealed class StiResizeReportHelper
	{
        /// <summary>
        /// Method resize page with specified parameters.
        /// </summary>
        /// <param name="page">Page which must be resized.</param>
        /// <param name="orientation">New value of page orientation.</param>
        /// <param name="paperSize">New value of paper size.</param>
        /// <param name="margins">New value of page margins.</param>
        /// <param name="pageWidth">New value of page width.</param>
        /// <param name="pageHeight">New value of page height.</param>        
        /// <param name="options">Options which specify how page will be resized.</param>
        private static void SetPageParameters(StiPage page, StiPageOrientation orientation, PaperKind paperSize, StiMargins margins,
            double pageWidth, double pageHeight, StiResizeReportOptions options)
        {
            bool isPageOrientationChanged = (options & StiResizeReportOptions.PageOrientationChanged) > 0;
            bool isRescaleContent = (options & StiResizeReportOptions.RescaleContent) > 0;
            bool isAllowPageMarginsRescaling = (options & StiResizeReportOptions.AllowPageMarginsRescaling) > 0;

            double oldWidth = page.Width;
            double oldHeight = page.Height;

            if (isPageOrientationChanged)
                page.Orientation = orientation;

            page.PaperSize = paperSize;

            #region Equal orientation
            if (orientation == page.Orientation)
            {
                page.PageWidth = pageWidth;
                page.PageHeight = pageHeight;
                page.Margins = margins.Clone() as StiMargins;
            }
            #endregion

            #region Not equal orientation
            else
            {
                page.PageWidth = pageHeight;
                page.PageHeight = pageWidth;
                page.Margins = new StiMargins(
                    margins.Top,
                    margins.Bottom,
                    margins.Left,
                    margins.Right);
            }
            #endregion

            if (page.Width <= 0) page.Width = 0.1;
            if (page.Height <= 0) page.Height = 0.1;

            #region isRescaleContent
            if (isRescaleContent)
            {
                double factorX = page.Width / oldWidth;
                double factorY = page.Height / oldHeight;

                page.ResizePage(factorX, factorY, isAllowPageMarginsRescaling);
            }
            #endregion

            foreach (StiComponent component in page.Components)
            {
                component.DockToContainer();
            }
        }

        /// <summary>
        /// Resize report with specified parameters.
        /// </summary>
        /// <param name="report">Report for resizing.</param>
        /// <param name="orientation">New value of page orientation.</param>
        /// <param name="paperSize">New value of paper size.</param>
        /// <param name="margins">New value of page margins.</param>
        /// <param name="pageWidth">New value of page width.</param>
        /// <param name="pageHeight">New value of page height.</param>
        /// <param name="options">Options which specify how page will be resized.</param>
        public static void ResizeReport(StiReport report,
            StiPageOrientation orientation, PaperKind paperSize, StiMargins margins,
            double pageWidth, double pageHeight, 
            StiResizeReportOptions options)
        {
            ResizeReport(report,
                orientation, paperSize, margins, pageWidth, pageHeight,
                options, -1);
        }

        /// <summary>
        /// Resize report with specified parameters.
        /// </summary>
        /// <param name="report">Report for resizing.</param>
        /// <param name="orientation">New value of page orientation.</param>
        /// <param name="paperSize">New value of paper size.</param>
        /// <param name="margins">New value of page margins.</param>
        /// <param name="pageWidth">New value of page width.</param>
        /// <param name="pageHeight">New value of page height.</param>
        /// <param name="options">Options which specify how page will be resized.</param>
		public static void ResizeReport(StiReport report,
            StiPageOrientation orientation, PaperKind paperSize, StiMargins margins, double pageWidth, double pageHeight,
            StiResizeReportOptions options, int indexOfRenderedPage)
		{
            bool isRebuildReport = (options & StiResizeReportOptions.RebuildReport) > 0;
            bool isProcessAllPages = (options & StiResizeReportOptions.ProcessAllPages) > 0;
            bool isShowProgressOnRebuildReport = (options & StiResizeReportOptions.ShowProgressOnRebuildReport) > 0;

            StiPage renderedPage = null;
            if (indexOfRenderedPage < report.RenderedPages.Count && indexOfRenderedPage >= 0)
                renderedPage = report.RenderedPages[indexOfRenderedPage];

            string renderedPageName = renderedPage != null ? renderedPage.Name : string.Empty;

            #region isRebuildReport
            if (isRebuildReport)
            {
                #region Process Master Report

                #region Prepare subreports list
                Hashtable guidToPage = new Hashtable();
                StiComponentsCollection comps = report.GetComponents();
                foreach (StiPage page in report.Pages)
                {
                    guidToPage[page.Guid] = page;
                }
                Hashtable pageToParentSubreportPage = new Hashtable();
                foreach (StiComponent comp in comps)
                {
                    StiSubReport subReport = comp as StiSubReport;
                    if (subReport != null && subReport.SubReportPageGuid != null)
                    {
                        StiPage pg = guidToPage[subReport.SubReportPageGuid] as StiPage;
                        if (pg != null)
                        {
                            pageToParentSubreportPage[pg] = comp.Width;
                        }
                    }
                }
                #endregion

                foreach (StiPage page in report.Pages)
                {
                    if (!(page is Dialogs.StiForm) && (page.Name == renderedPageName || isProcessAllPages))
                    {
                        object obj = pageToParentSubreportPage[page];
                        if (obj == null)
                        {
                            SetPageParameters(page, orientation, paperSize, margins, pageWidth, pageHeight, options);
                        }
                        if (!isProcessAllPages) break;
                    }
                }

                //second pass for subreports pages
                foreach (StiPage page in report.Pages)
                {
                    object obj = pageToParentSubreportPage[page];
                    if (obj != null)
                    {
                        double oldWidth = (double)obj;
                        if (page.Width != oldWidth)
                        {
                            double pageWidth2 = page.Width + page.Margins.Left + page.Margins.Right;
                            page.Width = oldWidth;
                            SetPageParameters(page, orientation, paperSize, margins, pageWidth2, pageHeight, options);
                        }
                    }
                }
                #endregion

                #region Process SubReports
                if (report.SubReports != null && report.SubReports.Count > 0)
                {
                    foreach (StiReport subReport in report.SubReports)
                    {
                        StiReport currentSubReport = subReport;
                        if (subReport.CompiledReport != null) currentSubReport = subReport.CompiledReport;

                        ResizeReport(currentSubReport, orientation, paperSize, margins, pageWidth, pageHeight, options);
                    }
                }
                #endregion

                report.IsRendered = false;
                report.Render(isShowProgressOnRebuildReport);
                report.InvokeRefreshViewer();
            }
            #endregion

            #region Process Rendered Pages
            else
            {
                int index = 0;
                foreach (StiPage page in report.RenderedPages)
                {
                    if (!(page is Dialogs.StiForm) && (index == indexOfRenderedPage || isProcessAllPages))
                    {
                        SetPageParameters(page, orientation, paperSize, margins, pageWidth, pageHeight, options);

                        if (!isProcessAllPages) return;
                    }
                    index++;
                }
            }
            #endregion
        }
	}
}
