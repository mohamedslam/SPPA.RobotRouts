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
using Stimulsoft.Report.Components;
using Stimulsoft.Base;
using Stimulsoft.Report.Dashboard;
using System.IO;
using Stimulsoft.Report.Units;
using System.Drawing.Printing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
using PrintDialog = Stimulsoft.System.Drawing.Printing.PrintDialog;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Print
{
	/// <summary>
	/// This class provide print service.
	/// </summary>
	public class StiPrintProvider
	{
		#region Fields
		private int resCurrentPrintPage;
		private int resPageNumber;
		private int totalRenderedPageCount;
		private double zoom;
		private StiReport printedReport;
		private int fromPage;
		private int toPage;
		private PaperSource paperSource;
        private PrinterSettings.PaperSizeCollection paperSizesStore;
        private PrinterSettings.PaperSourceCollection paperSourcesStore;
        private PaperSource defaultPaperSource;
        private PaperSize allPagesPaperSize;
        private PaperSource allPagesPaperSource;
        #endregion

		#region Properties
	    public static bool SetPageSettings { get; set; } = true;

	    public static bool SetPaperSource { get; set; } = true;

	    public static PaperSize PaperSizeForUsing { get; set; }

	    public static object LandscapeForUsing { get; set; }

	    public static bool UseEXDialog { get; set; }

        public bool KeepCachedPaperSizeAndSource { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Prints the rendered report. If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="report">Rendered report for printing.</param>
        /// <param name="showPrintDialog">Show the print dialog or no.</param>
        /// <param name="fromPage">Number of the first page to print. Starts from 1.</param>
        /// <param name="toPage">Number of the last page to print. Starts from 1.</param>
        /// <param name="copies">Number of copies to print.</param>
        /// <param name="printerSettings">Specifies information about how a document is printed, including the printer that prints it.</param>
        public virtual void Print(StiReport report, bool showPrintDialog, 
			int fromPage, int toPage, short copies, PrinterSettings printerSettings)
		{
            #region Print Dashboard
            if (!report.IsDocument && report.GetCurrentPage() is IStiDashboard)
            {
                using (var stream = new MemoryStream())
                using (var document = new StiReport())
                {
                    StiDashboardExport.ExportDocument(report, stream);
                    stream.Position = 0;
                    document.LoadDocument(stream);
                    report.RenderedPages.Clear();
                    document.RenderedPages.ToList().ForEach(p =>
                    {
                        p.Convert(StiUnit.GetUnitFromReportUnit(document.ReportUnit), StiUnit.GetUnitFromReportUnit(report.ReportUnit), true);
                        p.Report = report;
                        report.RenderedPages.Add(p);
                    });
                }
            }
            #endregion

            try
            {
				printedReport = report;
			    if (printerSettings != null)
			        paperSource = printerSettings.DefaultPageSettings.PaperSource;

			    if (report.RenderedPages.Count <= 0) return;

			    #region Check Range
			    totalRenderedPageCount = report.GetTotalRenderedPageCount();
			    if (totalRenderedPageCount < 1)
			        totalRenderedPageCount = 1;
					
			    if (toPage == -1 || toPage == 0)
			        toPage = totalRenderedPageCount;

			    if (fromPage < 1)
			        fromPage = 1;

			    if (toPage < 1)
			        toPage = 1;

			    fromPage =	Math.Min(fromPage, totalRenderedPageCount);
			    toPage =	Math.Min(toPage, totalRenderedPageCount);
                #endregion

                if (!KeepCachedPaperSizeAndSource)
                {
                    paperSizesStore = null;
                    paperSourcesStore = null;
                }
			    defaultPaperSource = null;
                    
			    using (var printDocument = new PrintDocument())
			    {	
			        printDocument.QueryPageSettings += this.OnQueryPageSettings;
			        printDocument.PrintPage += this.OnPrintPage;

			        if (printerSettings == null)
			        {
			            printerSettings = printDocument.PrinterSettings;
			            printerSettings.Collate = report.PrinterSettings.Collate;
			            printerSettings.Duplex = report.PrinterSettings.Duplex;

			            if (report.PrinterSettings.PrinterName != null)
			            {
			                foreach (string printerName in PrinterSettings.InstalledPrinters)
			                {
			                    if (printerName == report.PrinterSettings.PrinterName && report.PrinterSettings.PrinterName != null)
			                    {
			                        printerSettings.PrinterName = report.PrinterSettings.PrinterName;
			                        break;
			                    }
			                }
			            }
			        }
			        else
			        {
			            if (StiOptions.Print.UsePrinterSettingsEntirely)
			                printDocument.PrinterSettings = printerSettings;

			            else
			            {
			                printDocument.PrinterSettings.PrinterName = printerSettings.PrinterName;
			                printDocument.PrinterSettings.Collate = printerSettings.Collate;
			                printDocument.PrinterSettings.Copies = printerSettings.Copies;
			                printDocument.PrinterSettings.Duplex = printerSettings.Duplex;
			                printDocument.PrinterSettings.FromPage = printerSettings.FromPage;
			                printDocument.PrinterSettings.ToPage = printerSettings.ToPage;
			                printDocument.PrinterSettings.PrintRange = printerSettings.PrintRange;
			                printDocument.PrinterSettings.PrintToFile = printerSettings.PrintToFile;
			                printDocument.PrinterSettings.ToPage = printerSettings.ToPage;
			                printDocument.PrinterSettings.DefaultPageSettings.Color = printerSettings.DefaultPageSettings.Color;
			                printDocument.PrinterSettings.DefaultPageSettings.PrinterResolution = printerSettings.DefaultPageSettings.PrinterResolution;

			                try
			                {
			                    if (!string.IsNullOrEmpty(printerSettings.PrintFileName))
			                        printDocument.PrinterSettings.PrintFileName = printerSettings.PrintFileName;
			                }
			                catch
			                {
			                }
			            }
			        }

			        if (PaperSizeForUsing != null)
			            printDocument.DefaultPageSettings.PaperSize = PaperSizeForUsing;

			        if (LandscapeForUsing is bool)
			            printDocument.DefaultPageSettings.Landscape = (bool)LandscapeForUsing;

			        var documentName = report.ReportAlias;
			        if (string.IsNullOrEmpty(documentName))
			            documentName = report.ReportName;

			        if (string.IsNullOrEmpty(documentName))
			            documentName = "Stimulsoft Document";

			        printDocument.PrinterSettings.Copies = Math.Max(copies, (short)1);
			        printDocument.PrinterSettings.MaximumPage = totalRenderedPageCount;
			        printDocument.PrinterSettings.FromPage = fromPage;
			        printDocument.PrinterSettings.ToPage = toPage;
			        printDocument.DocumentName = documentName;
			        printDocument.PrintController = new StandardPrintController();

			        if (showPrintDialog)
			        {
			            report.IsStopped = false;
							
			            using (var printDialog = new PrintDialog())
			            {
			                printDialog.UseEXDialog = UseEXDialog;
			                printDialog.AllowPrintToFile = true;
			                printDialog.AllowSomePages = true;
			                printDialog.Document = printDocument;

			                allPagesPaperSize = null;
			                allPagesPaperSource = null;
			                PaperSize paperSizeStored = null;
			                PaperSource paperSourceStored = null;

			                if (StiOptions.Print.UsePaperSizeAndSourceFromPrintDialogIfTheyChanged ||
			                    StiOptions.Print.UsePaperSizeAndSourceFromPrintDialogAlways)
			                {
			                    printDocument.DefaultPageSettings.PaperSize = printerSettings.DefaultPageSettings.PaperSize;
			                    printDocument.DefaultPageSettings.PaperSource = printerSettings.DefaultPageSettings.PaperSource;
			                    paperSizeStored = printDocument.DefaultPageSettings.PaperSize;
			                    paperSourceStored = printDocument.DefaultPageSettings.PaperSource;
			                }

			                report.PrinterSettings.PrintDialogResult = printDialog.ShowDialog();

			                if (copies > 1 && printDialog.PrinterSettings.Copies == 1 && printDialog.PrinterSettings.MaximumCopies == 1)
			                    printDialog.PrinterSettings.Copies = Math.Max(copies, (short) 1);

			                if (report.PrinterSettings.PrintDialogResult == DialogResult.OK)
			                {
			                    printDialog.Document.DocumentName = documentName;

			                    fromPage = printDialog.PrinterSettings.FromPage;
			                    toPage = printDialog.PrinterSettings.ToPage;

			                    if (StiOptions.Print.UsePaperSizeAndSourceFromPrintDialogIfTheyChanged &&
			                        printDocument.DefaultPageSettings.PaperSize != paperSizeStored ||
			                        StiOptions.Print.UsePaperSizeAndSourceFromPrintDialogAlways)
			                    {
			                        allPagesPaperSize = printDocument.DefaultPageSettings.PaperSize;
			                    }

			                    if (StiOptions.Print.UsePaperSizeAndSourceFromPrintDialogIfTheyChanged &&
			                        printDocument.DefaultPageSettings.PaperSource != paperSourceStored ||
			                        StiOptions.Print.UsePaperSizeAndSourceFromPrintDialogAlways)
			                    {
			                        allPagesPaperSource = printDocument.DefaultPageSettings.PaperSource;
			                    }

			                }
			                else
			                    report.PrinterSettings.PrintDialogResult = DialogResult.Cancel;
			            }
			        }
			        else
			            report.PrinterSettings.PrintDialogResult = DialogResult.None;
						
			        if (report.PrinterSettings.PrintDialogResult == DialogResult.OK ||
			            report.PrinterSettings.PrintDialogResult == DialogResult.None)
			        {
			            #region StorePrinterSettingsInReport
			            if (StiOptions.Print.StorePrinterSettingsInReportAfterPrintDialog)
			            {
			                report.PrinterSettings.Collate = printDocument.PrinterSettings.Collate;
			                report.PrinterSettings.Copies = printDocument.PrinterSettings.Copies;
			                report.PrinterSettings.Duplex = printDocument.PrinterSettings.Duplex;
			                report.PrinterSettings.PrinterName = printDocument.PrinterSettings.PrinterName;
			            }
			            #endregion

			            this.toPage = toPage;
			            this.fromPage = fromPage;

			            var needCopies = 1;
			            int printerSettingsCopies = printDocument.PrinterSettings.Copies;
			            if (printerSettingsCopies > 1 && printDocument.PrinterSettings.MaximumCopies < printerSettingsCopies)
			            {
			                needCopies = printerSettingsCopies;
			                printDocument.PrinterSettings.Copies = 1;
			            }

			            for (var index = 0; index < needCopies; index++)
			            {
			                BeginPrint();

			                try
			                {
			                    printDocument.Print();
			                }
			                catch (Exception e)
			                {
			                    if (StiExceptionProvider.Show(e, true)) throw;
			                }
			                finally
			                {
			                    StopPrint();
			                }
			            }
			        }
			        else report.IsStopped = true;
						
			    }

                if (!KeepCachedPaperSizeAndSource)
                {
                    paperSizesStore = null;
                    paperSourcesStore = null;
                }
			    defaultPaperSource = null;
			}
			catch (Exception e)
			{
                if (StiExceptionProvider.Show(e, true)) throw;
			}
		}

		private void BeginPrint()
		{
			if (!printedReport.RenderedPages.CacheMode)
			{
				foreach (StiPage page in printedReport.RenderedPages)
				{
					page.IsPrinted = false;
				}
			}

			printedReport.PageNumber = fromPage;
			resCurrentPrintPage = printedReport.CurrentPrintPage;
			resPageNumber = printedReport.PageNumber;
			printedReport.CurrentPrintPage = 0;
		}
		
		private void StopPrint()
		{
			printedReport.CurrentPrintPage = resCurrentPrintPage;
			printedReport.PageNumber = resPageNumber;
		}

		private object GetPaperSize(StiPage page, QueryPageSettingsEventArgs e)
		{
			if (PaperSizeForUsing != null)
			    return PaperSizeForUsing;

            if (paperSizesStore == null)
                paperSizesStore = e.PageSettings.PrinterSettings.PaperSizes;

			if (page.PaperSize != PaperKind.Custom && paperSizesStore != null)
			{
                foreach (PaperSize ps in paperSizesStore)
				{
					if (ps.Kind == page.PaperSize)
					    return ps;
				}
			}
			
			return GetPaperSize(e, page);
		}
	
		private object GetPaperSize(QueryPageSettingsEventArgs e, StiPage page)
		{
            var pageWidth = (int)Math.Round(page.Unit.ConvertToHInches(page.PageWidth), 0);
            var pageHeight = (int)Math.Round(page.Unit.ConvertToHInches(page.PageHeight), 0);

            var originPageWidth = pageWidth;
            var originPageHeight = pageHeight;

            if (page.Orientation == StiPageOrientation.Landscape)
            {
                originPageWidth = pageHeight;
                originPageHeight = pageWidth;
            }

            if (StiOptions.Print.FindCustomPaperSizeInStandardPaperSizes && paperSizesStore != null)
            {
                foreach (PaperSize ps in paperSizesStore)
                {
                    if (ps.Width == pageWidth && ps.Height == pageHeight ||
                        ps.Height == pageWidth && ps.Width == pageHeight) return ps;
                }
            }
            return new PaperSize("Custom", originPageWidth, originPageHeight);
		}
		#endregion

		#region Events
		public static event StiQueryPageSettingsEventHandler QueryPageSettings;

		private static void InvokeQueryPageSettings(StiReport report, StiQueryPageSettingsEventArgs e)
		{
            QueryPageSettings?.Invoke(report, e);
        }
        #endregion

		#region Handlers
		private void OnPrintPage(object sender, PrintPageEventArgs e)
		{
            if (printedReport.IsStopped)
            {
                e.Cancel = true;
                return;
            }

			int x;
			int y;

			printedReport.CurrentPrintPage = printedReport.GetPageIndex(printedReport.PageNumber, out x, out y);

            var index = printedReport.CurrentPrintPage;

            if (index < 0)
                index = 0;

            if (index >= printedReport.RenderedPages.Count)
                index = printedReport.RenderedPages.Count - 1;

            var ee = new StiPrintPageEventArgs(index + 1, fromPage, toPage);
            StiOptions.Engine.GlobalEvents.InvokeReportPrintingPage(this, ee);

            var page = printedReport.RenderedPages[index];
			printedReport.RenderedPages.GetPage(page);
			page.CurrentWidthSegment = x;
			page.CurrentHeightSegment = y;
			page.IsPrinted = true;

			zoom = printedReport.Info.Zoom;

            if (page.Report != null)
                page.Report.Info.Zoom = 1;

            else
                printedReport.Info.Zoom = 1;

			printedReport.IsPrinting = true;
			page.Paint(e.Graphics);
			
			printedReport.Info.Zoom = zoom;
			printedReport.IsPrinting = false;

            if (toPage >= fromPage)
            {
                printedReport.PageNumber++;

                if (printedReport.PageNumber > toPage ||
                    printedReport.PageNumber > totalRenderedPageCount) e.HasMorePages = false;

                else
                    e.HasMorePages = true;
            }
            else
            {
                printedReport.PageNumber--;

                if (printedReport.PageNumber < toPage ||
                    printedReport.PageNumber < 1) e.HasMorePages = false;

                else
                    e.HasMorePages = true;
            }

            StiOptions.Engine.GlobalEvents.InvokeReportPrintedPage(this, ee);
		}
								
		private void OnQueryPageSettings(object sender, QueryPageSettingsEventArgs e)
		{
            var x = 0;
            var y = 0;
            var pageIndex = printedReport.GetPageIndex(printedReport.PageNumber, out x, out y);
            if (pageIndex < 0)
                pageIndex = 0;

            printedReport.CurrentPrintPage = pageIndex;

            var page = printedReport.RenderedPages[printedReport.CurrentPrintPage];
            printedReport.RenderedPages.GetPage(page);

            #region Orientation
            e.PageSettings.Landscape = page.Orientation == StiPageOrientation.Landscape;
            #endregion

            var usePaperSourceFirstPage = false;
            var paperSourceSelected = false;

            if (paperSourcesStore == null)
                paperSourcesStore = e.PageSettings.PrinterSettings.PaperSources;

            #region PaperSourceOfFirstPage
            if (page.PaperSourceOfFirstPage != null &&
                page.PaperSourceOfFirstPage.Trim().Length != 0 &&
                (printedReport.CurrentPrintPage == 0 ||
                printedReport.RenderedPages[printedReport.CurrentPrintPage].Name !=
                printedReport.RenderedPages[printedReport.CurrentPrintPage - 1].Name))
            {
                foreach (PaperSource paperSource3 in paperSourcesStore)
                {
                    if (paperSource3.SourceName == page.PaperSourceOfFirstPage)
                    {
                        e.PageSettings.PaperSource = paperSource3;
                        usePaperSourceFirstPage = true;
                        paperSourceSelected = true;
                        break;
                    }
                }
            }
            #endregion

            #region PaperSourceOfOtherPages
            if (page.PaperSourceOfOtherPages != null &&
                page.PaperSourceOfOtherPages.Trim().Length != 0 &&
                e.PageSettings.PaperSource != null && (!usePaperSourceFirstPage))
            {
                foreach (PaperSource paperSource3 in paperSourcesStore)
                {
                    if (paperSource3.SourceName == page.PaperSourceOfOtherPages)
                    {
                        e.PageSettings.PaperSource = paperSource3;
                        paperSourceSelected = true;
                        break;
                    }
                }
            }
            #endregion

            #region Try to find default paper source if paperSourceSelected = false
            if (defaultPaperSource == null)
                defaultPaperSource = e.PageSettings.PrinterSettings.DefaultPageSettings.PaperSource;

		    if (!paperSourceSelected)
		        e.PageSettings.PaperSource = defaultPaperSource;
		    #endregion

            var paperSize = GetPaperSize(page, e);

            #region StiQueryPageSettingsEvent
            var ee = new StiQueryPageSettingsEventArgs(paperSize, paperSource, e.PageSettings);

            InvokeQueryPageSettings(printedReport, ee);
            paperSize = ee.PaperSize;
            paperSource = ee.PaperSource;
            #endregion

		    if (SetPaperSource && paperSource != null)
		        e.PageSettings.PaperSource = paperSource;

		    if (SetPageSettings && paperSize != null)
		        e.PageSettings.PaperSize = (PaperSize) paperSize;

		    if (allPagesPaperSize != null)
		        e.PageSettings.PaperSize = allPagesPaperSize;

            if (allPagesPaperSource != null)
                e.PageSettings.PaperSource = allPagesPaperSource;
        }
		#endregion

		static StiPrintProvider()
		{
            //We need use UseEXDialog property in Windows 7 64 bit
            //Running on Windows 7 x64, I also had the problem of the print dialog not showing up and immediately returning Canceled. Setting UseEXDialog = true fixed this.
			var is64Bit = IntPtr.Size == 8;
            UseEXDialog |= is64Bit;
        }
	}
}
