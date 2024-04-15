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
using System.IO;
using System.Text;
using Stimulsoft.Report.Export;
using System.Drawing.Printing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
using PrintDialog = Stimulsoft.System.Drawing.Printing.PrintDialog;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Viewer
{    
	/// <summary>
	/// This class provide print service.
	/// </summary>
	public class StiDotMatrixPrintProvider
	{		
		/// <summary>
		/// Prints a report to default dot-matrix printer in the RAW mode in ASCII encoding.
		/// </summary>
		public static void PrintToDotMatrixPrinter(StiReport report)
		{
			var settings = new PrinterSettings();
			PrintToDotMatrixPrinter(report, settings.PrinterName, Encoding.ASCII);			
		}

		/// <summary>
		/// Prints a report to dot-matrix printer in the RAW mode in ASCII encoding.
		/// </summary>
		/// <param name="printerName">A name of a printer.</param>
		public static void PrintToDotMatrixPrinter(StiReport report, string printerName)
		{
			PrintToDotMatrixPrinter(report, printerName, Encoding.ASCII);
		}

		/// <summary>
		/// Prints report to dot-matrix printer in the RAW mode.
		/// </summary>
		/// <param name="printerName">A name of a printer.</param>
		/// <param name="encoding">A parameter which sets text encoding.</param>
		public static void PrintToDotMatrixPrinter(StiReport report, string printerName, Encoding encoding)
		{
			PrintToDotMatrixPrinter(report, printerName, encoding, -1, -1);
		}

		/// <summary>
		/// Prints a report to dot-matrix printer in the RAW mode.
		/// </summary>
		/// <param name="printerName">A name of a printer.</param>
		/// <param name="encoding">A parameter which sets text encoding.</param>
		/// <param name="fromPage">A number of the first page to print.</param>
		/// <param name="toPage">A number of the last page to print.</param>
		public static void PrintToDotMatrixPrinter(StiReport report, string printerName, Encoding encoding, int fromPage, int toPage)
		{
			PrintToDotMatrixPrinter(report, printerName, encoding, true, StiTxtBorderType.UnicodeSingle,
				true, false, true, false, 1.25f, 1.25f, fromPage, toPage, false, null);
		}

		/// <summary>
		/// Prints a report to dot-matrix printer in RAW mode.
		/// </summary>
		/// <param name="printerName">A name of the printer.</param>
		/// <param name="encoding">A parameter which sets text encoding.</param>
		/// <param name="drawBorder">If true then borders are exported to a text.</param>
		/// <param name="borderType">Type of borders.</param>
		/// <param name="putFeedPageCode">If true then the EOF character will be added to the end of each page.</param>
		/// <param name="cutLongLines">If true then all long lines will be cut.</param>
		/// <param name="fromPage">A number of the first page to print.</param>
		/// <param name="toPage">A number of the last page to print.</param>
		public static void PrintToDotMatrixPrinter(StiReport report, string printerName, Encoding encoding, 
			bool drawBorder, StiTxtBorderType borderType, 
			bool putFeedPageCode, bool cutLongLines, int fromPage, int toPage)
		{
			PrintToDotMatrixPrinter(report, printerName, encoding, drawBorder, borderType,
                true, false, putFeedPageCode, cutLongLines, 1.25f, 1.25f, fromPage, toPage, false, null);
		}

		/// <summary>
		/// Prints report to dot-matrix printer in the RAW mode.
		/// </summary>
		/// <param name="printerName">A name of the printer.</param>
		/// <param name="encoding">A parameter which sets text encoding.</param>
		/// <param name="drawBorder">If true then borders are exported to the text.</param>
		/// <param name="borderType">Type of borders.</param>
		/// <param name="killSpaceLines">If true then empty lines will be removed from the result text.</param>
		/// <param name="killSpaceGraphLines">If true then empty lines with vertical borders will be removed from the result text.</param>
		/// <param name="putFeedPageCode">If true then the EOF character will be added to the end of each page.</param>
		/// <param name="cutLongLines">If true then all long lines will be cut.</param>
        /// <param name="zoomX">Horizontal zoom factor by X axis. By default a value is 1.0f what is equal 100% in export settings window.</param>
        /// <param name="zoomY">Vertical zoom factor by Y axis. By default a value is 1.0f what is equal 100% in export settings window.</param>
        /// <param name="fromPage">A number of the first page to print.</param>
		/// <param name="toPage">A number of the last page to print.</param>
        /// <param name="useEscapeCodes">Use Escape codes in text.</param>
        /// <param name="escapeCodesCollectionName">Name of the Escape codes collection to use.</param>
        public static void PrintToDotMatrixPrinter(StiReport report, string printerName, Encoding encoding, 
			bool drawBorder, StiTxtBorderType borderType, bool killSpaceLines, bool killSpaceGraphLines,
			bool putFeedPageCode, bool cutLongLines, float zoomX, float zoomY, int fromPage, int toPage,
            bool useEscapeCodes, string escapeCodesCollectionName)
		{
			report.InvokePrinting();
			report.RegReportDataSources();

		    if (!report.IsRendered)report.Render(true);

			StiLogService.Write(report.GetType(), "Printing report to dot-matrix printer");

			try
			{
			    Print(report.CheckNeedsCompiling() ? report.CompiledReport : report,
			        printerName, encoding, drawBorder, borderType, killSpaceLines, killSpaceGraphLines,
			        putFeedPageCode, cutLongLines, zoomX, zoomY, fromPage, toPage, useEscapeCodes, escapeCodesCollectionName);
			}
			catch (Exception e)
			{
				StiLogService.Write(report.GetType(), "Printing report to dot-matrix printer...ERROR");
				StiLogService.Write(report.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
			}
			report.InvokePrinted();
		}
        
		/// <summary>
		/// Returns string representation of a report in the dot-matrix mode.
		/// </summary>
		/// <param name="encoding">A parameter which sets text encoding.</param>
		/// <param name="drawBorder">If true then borders are exported to the text.</param>
		/// <param name="borderType">Type of borders.</param>
		/// <param name="killSpaceLines">If true then empty lines will be removed from the result text.</param>
		/// <param name="killSpaceGraphLines">If true then empty lines with vertical borders will be removed from the result text.</param>
		/// <param name="putFeedPageCode">If true then the EOF character will be added to the end of each page.</param>
		/// <param name="cutLongLines">If true then all long lines will be cut.</param>
        /// <param name="zoomX">Horizontal zoom factor by X axis. By default a value is 1.0f what is equal 100% in export settings window.</param>
        /// <param name="zoomY">Vertical zoom factor by Y axis. By default a value is 1.0f what is equal 100% in export settings window.</param>
        /// <param name="pageRange">A range of pages for exporting.</param>
        /// <param name="useEscapeCodes">Use Escape codes in text.</param>
        /// <param name="escapeCodesCollectionName">Name of the Escape codes collection to use.</param>
        /// <returns>A string which contains a string representation of a report in the dot-matrix mode.</returns>
		public static string GetReportForDotMatrixReport(StiReport report, Encoding encoding, 
			bool drawBorder, StiTxtBorderType borderType, bool killSpaceLines, bool killSpaceGraphLines,
			bool putFeedPageCode, bool cutLongLines, float zoomX, float zoomY, StiPagesRange pageRange,
            bool useEscapeCodes, string escapeCodesCollectionName)
		{
			if (report.CheckNeedsCompiling() && report.CompiledReport == null)return string.Empty;
			
			var ms = new MemoryStream();
			var sr = new StreamReader(ms);
				
			var export = new StiTxtExportService();

			export.ExportTxt(report.CheckNeedsCompiling() ? report.CompiledReport : report, ms, encoding, 
				drawBorder, borderType, killSpaceLines, killSpaceGraphLines, putFeedPageCode,
				cutLongLines, zoomX, zoomY, pageRange, useEscapeCodes, escapeCodesCollectionName);

			ms.Flush();
			ms.Seek(0, SeekOrigin.Begin);

			var reportStr = sr.ReadToEnd();
			sr.Close();
			ms.Close();

			return reportStr;
		}

        /// <summary>
        /// Prints the rendered report. If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="report">Rendered report for printing.</param>
        /// <param name="printerName">A name of the printer.</param>
        /// <param name="encoding">A parameter which sets text encoding.</param>
        /// <param name="drawBorder">If true then borders are exported to the text.</param>
        /// <param name="borderType">Type of borders.</param>
        /// <param name="killSpaceLines">If true then empty lines will be removed from the result text.</param>
        /// <param name="killSpaceGraphLines">If true then empty lines with vertical borders will be removed from the result text.</param>
        /// <param name="putFeedPageCode">If true then the EOF character will be added to the end of each page.</param>
        /// <param name="cutLongLines">If true then all long lines will be cut.</param>
        /// <param name="zoomX">Horizontal zoom factor by X axis. By default a value is 1.0f what is equal 100% in export settings window.</param>
        /// <param name="zoomY">Vertical zoom factor by Y axis. By default a value is 1.0f what is equal 100% in export settings window.</param>
        /// <param name="fromPage">A number of the first page to print.</param>
        /// <param name="toPage">A number of the last page to print.</param>
        /// <param name="useEscapeCodes">Use Escape codes in text.</param>
        /// <param name="escapeCodesCollectionName">Name of the Escape codes collection to use.</param>
        public static void Print(StiReport report, 
			string printerName, Encoding encoding, 
			bool drawBorder, StiTxtBorderType borderType, bool killSpaceLines, bool killSpaceGraphLines,
			bool putFeedPageCode, bool cutLongLines, float zoomX, float zoomY, int fromPage, int toPage,
            bool useEscapeCodes, string escapeCodesCollectionName)
		{
			using (var printDocument = new PrintDocument())
			{
				if (!string.IsNullOrEmpty(printerName))
				{
					printDocument.PrinterSettings.PrinterName = printerName;
				}

				#region Check Range
				int totalRenderedPageCount = report.GetTotalRenderedPageCount();
				if (totalRenderedPageCount < 1)totalRenderedPageCount = 1;
					
				if (toPage == -1 || toPage == 0)toPage = totalRenderedPageCount;
				if (fromPage < 1)fromPage = 1;
				if (toPage < 1)toPage = 1;
				fromPage =	Math.Min(fromPage, totalRenderedPageCount);
				toPage =	Math.Min(toPage, totalRenderedPageCount);
				#endregion

				printDocument.PrinterSettings.FromPage = fromPage;
				printDocument.PrinterSettings.ToPage = toPage;

                var result = DialogResult.OK;

				if (report.PrinterSettings.ShowDialog)
				{
                    using (var printDialog = new PrintDialog())
					{
                        printDialog.UseEXDialog = Stimulsoft.Report.Print.StiPrintProvider.UseEXDialog;
						printDialog.AllowPrintToFile = false;
						printDialog.AllowSomePages = true;
						printDialog.AllowSelection = true;
						printDialog.Document = printDocument;

						printDialog.Document.DocumentName = report.ReportAlias;

						result = printDialog.ShowDialog();
					}
				}

				if (result == DialogResult.OK)
				{
					var reportStr = GetReportForDotMatrixReport(report, null, 
						drawBorder, borderType, killSpaceLines, killSpaceGraphLines,
						putFeedPageCode, cutLongLines, zoomX, zoomY, 
						new StiPagesRange(StiRangeType.Pages, $"{printDocument.PrinterSettings.FromPage}-{printDocument.PrinterSettings.ToPage}", 0),
                        useEscapeCodes, escapeCodesCollectionName);

					StiRawPrinterHelper.SendStringToPrinter(printDocument.PrinterSettings.PrinterName, report.ReportAlias, reportStr, encoding);
				}
			}
		}
	}
}

