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

using Stimulsoft.Base;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Export;
using Stimulsoft.Report.Print;
using Stimulsoft.Report.Viewer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Printing;

namespace Stimulsoft.Report
{
    public partial class StiReport
    {
        #region Methods.WinForms
        /// <summary>
        /// Prints the rendered report. If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="printerSettings">Specifies information about how a document is printed, including the printer that prints it.</param>
        public async Task<StiReport> PrintAsync(PrinterSettings printerSettings)
        {
            return await Task.Run(() => Print(printerSettings));
        }

        /// <summary>
        /// Prints the rendered report without print dialog. If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="printerSettings">Specifies information about how a document is printed, including the printer that prints it.</param>
        public StiReport Print(PrinterSettings printerSettings)
        {
            return Print(this.PrinterSettings.ShowDialog, printerSettings);
        }

        /// <summary>
        /// Prints the rendered report. If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="showPrintDialog">If a parameter is true then the print dialog will be shown.</param>
        /// <param name="printerSettings">Specifies information about how a document is printed, including the printer that prints it.</param>
        public StiReport Print(bool showPrintDialog, PrinterSettings printerSettings)
        {
            return Print(showPrintDialog, printerSettings.FromPage, printerSettings.ToPage, printerSettings.Copies, printerSettings);
        }

        /// <summary>
        /// Prints the rendered report. If the report is not rendered then its rendering starts.
        /// </summary>
        public StiReport Print()
        {
            return Print(this.PrinterSettings.ShowDialog, -1, -1, (short)PrinterSettings.Copies);
        }

        /// <summary>
        /// Prints the rendered report with the print dialog. If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="fromPage">A number of the first page to print.</param>
        /// <param name="toPage">A number of the last page to print.</param>
        /// <param name="copies">A number of copies to print.</param>
		public StiReport Print(int fromPage, int toPage, short copies)
        {
            return Print(this.PrinterSettings.ShowDialog, fromPage, toPage, copies);
        }

        /// <summary>
        /// Prints the rendered report. If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="copies">A number of copies to print.</param>
        public async Task<StiReport> PrintAsync(short copies)
        {
            return await Task.Run(() => Print(false, copies));
        }

        /// <summary>
        /// Prints a rendered report. If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="showPrintDialog">If a parameter is true then the print dialog will be shown.</param>
        /// <param name="copies">A number of copies to print.</param>
		public StiReport Print(bool showPrintDialog, short copies)
        {
            return Print(showPrintDialog, -1, -1, copies);
        }

        /// <summary>
        /// Prints the rendered report. If the report is not rendered then its rendering starts.
        /// </summary>
        public async Task<StiReport> PrintAsync()
        {
            return await Task.Run(() => Print(false));
        }

        /// <summary>
        /// Prints the rendered report. If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="showPrintDialog">If a parameter is true then the print dialog will be shown.</param>
		public StiReport Print(bool showPrintDialog)
        {
            return Print(showPrintDialog, (short)PrinterSettings.Copies);
        }

        /// <summary>
        /// Prints the rendered report. If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="fromPage">A number of the first page to print.</param>
        /// <param name="toPage">A number of the last page to print.</param>
        /// <param name="copies">A number of copies to print.</param>
        public async Task<StiReport> PrintAsync(int fromPage, int toPage, short copies)
        {
            return await Task.Run(() => Print(false, fromPage, toPage, copies));
        }

        /// <summary>
        /// Prints the rendered report. If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="showPrintDialog">If a parameter is true then the print dialog will be shown.</param>
        /// <param name="fromPage">A number of the first page to print.</param>
        /// <param name="toPage">A number of the last page to print.</param>
        /// <param name="copies">A number of copies to print.</param>
		public StiReport Print(bool showPrintDialog, int fromPage, int toPage, short copies)
        {
            return Print(showPrintDialog, fromPage, toPage, copies, null);
        }

        /// <summary>
        /// Prints the rendered report. If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="fromPage">A number of the first page to print.</param>
        /// <param name="toPage">A number of the last page to print.</param>
        /// <param name="copies">A number of copies to print.</param>
        /// <param name="printerSettings">Specifies information about how a document is printed, including the printer that prints it.</param>
        public async Task<StiReport> PrintAsync(int fromPage, int toPage, short copies, PrinterSettings printerSettings)
        {
            return await Task.Run(() => Print(false, fromPage, toPage, copies, printerSettings));
        }

        /// <summary>
        /// Prints the rendered report. If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="showPrintDialog">If a parameter is true then the print dialog will be shown.</param>
        /// <param name="fromPage">A number of the first page to print.</param>
        /// <param name="toPage">A number of the last page to print.</param>
        /// <param name="copies">A number of copies to print.</param>
        /// <param name="printerSettings">Specifies information about how a document is printed, including the printer that prints it.</param>
		public StiReport Print(bool showPrintDialog, int fromPage, int toPage, short copies, PrinterSettings printerSettings, StiPrintProvider printProvider = null)
        {
            InvokePrinting();
            RegReportDataSources();

            if (!IsRendered)
                this.Render(showPrintDialog);

            StiLogService.Write(this.GetType(), "Printing report");

            try
            {
                if (printProvider == null)
                    printProvider = new StiPrintProvider();

                var report = NeedsCompiling && CompiledReport != null ? CompiledReport : this;
                printProvider.Print(report, showPrintDialog, fromPage, toPage, copies, printerSettings);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Printing report...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
            InvokePrinted();

            return this;
        }

        /// <summary>
        /// Prints the collection of reports. If the report from collection is not rendered then its rendering starts.
        /// </summary>
        /// <param name="reports">Collection of reports to print.</param>
        public async Task<StiReport> PrintReportsAsync(List<StiReport> reports)
        {
            await Task.Run(() => PrintReports(reports));

            return this;
        }

        /// <summary>
        /// Prints the collection of reports. If the report from collection is not rendered then its rendering starts.
        /// </summary>
        /// <param name="reports">Collection of reports to print.</param>
		public static void PrintReports(List<StiReport> reports)
        {
            PrintReports(reports, true, 1);
        }

        /// <summary>
        /// Prints the collection of reports. If the report from collection is not rendered then its rendering starts.
        /// </summary>
        /// <param name="reports">Collection of reports to print.</param>
        /// <param name="copies">A number of copies to print.</param>
        public async Task<StiReport> PrintReportsAsync(List<StiReport> reports, short copies)
        {
            await Task.Run(() => PrintReports(reports, false, copies));

            return this;
        }

        /// <summary>
        /// Prints the collection of reports. If the report from collection is not rendered then its rendering starts.
        /// </summary>
        /// <param name="reports">Collection of reports to print.</param>
        /// <param name="showPrintDialog">If a parameter is true then the print dialog will be shown.</param>
        /// <param name="copies">A number of copies to print.</param>
		public static void PrintReports(List<StiReport> reports, bool showPrintDialog, short copies)
        {
            PrintReports(reports, showPrintDialog, copies, null);
        }

        /// <summary>
        /// Prints the collection of reports. If the report from collection is not rendered then its rendering starts.
        /// </summary>
        /// <param name="reports">Collection of reports to print.</param>
        /// <param name="copies">A number of copies to print.</param>
        /// <param name="printerSettings">Specifies information about how a document is printed, including the printer that prints it.</param>        
        public async Task<StiReport> PrintReportsAsync(List<StiReport> reports, short copies, PrinterSettings printerSettings)
        {
            await Task.Run(() => PrintReports(reports, false, copies, printerSettings));

            return this;
        }

        /// <summary>
        /// Prints the collection of reports. If the report from collection is not rendered then its rendering starts.
        /// </summary>
        /// <param name="reports">Collection of reports to print.</param>
        /// <param name="showPrintDialog">If a parameter is true then the print dialog will be shown.</param>
        /// <param name="copies">A number of copies to print.</param>
        /// <param name="printerSettings">Specifies information about how a document is printed, including the printer that prints it.</param>        
		public static void PrintReports(List<StiReport> reports, bool showPrintDialog, short copies, PrinterSettings printerSettings)
        {
            var tempReport = StiActivator.CreateObject(StiOptions.Engine.BaseReportType) as StiReport;
            tempReport.IsRendered = true;
            tempReport.NeedsCompiling = false;
            tempReport.RenderedPages.Clear();

            foreach (StiReport report in reports)
            {
                if (!report.IsRendered)
                    report.Render(showPrintDialog);

                foreach (StiPage page in report.RenderedPages)
                {
                    tempReport.RenderedPages.Add(page);
                }
            }
            tempReport.Print(showPrintDialog, copies, -1, -1, printerSettings);

            foreach (StiReport report in reports)
            {
                foreach (StiPage page in report.RenderedPages)
                {
                    page.Report = report;
                }
            }
        }
        #endregion

        #region Methods.WPF
        /// <summary>
        /// Prints the rendered report with using WPF technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Wpf assembly.
        /// </summary>
        public async Task<StiReport> PrintWithWpfAsync()
        {
            return await Task.Run(() => PrintWithWpf(false));
        }

        /// <summary>
        /// Prints the rendered report with using WPF technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Wpf assembly.
        /// </summary>
		public StiReport PrintWithWpf()
        {
            return PrintWithWpf(true);
        }

        /// <summary>
        /// Prints the rendered report with using WPF technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Wpf assembly.
        /// </summary>
        /// <param name="printerName">A name of a printer that will be used for printing.</param>
        public async Task<StiReport> PrintWithWpfAsync(string printerName)
        {
            return await Task.Run(() => PrintWithWpf(false, -1, printerName));
        }

        /// <summary>
        /// Prints the rendered report with using WPF technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Wpf assembly.
        /// </summary>
        /// <param name="printerName">A name of a printer that will be used for printing.</param>
		public StiReport PrintWithWpf(string printerName)
        {
            return PrintWithWpf(true, -1, printerName);
        }

        /// <summary>
        /// Prints the rendered report with using WPF technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Wpf assembly.
        /// </summary>
        /// <param name="printTicket">Specifies information about how a document is printed.</param>
        public async Task<StiReport> PrintWithWpfAsync(object printTicket)
        {
            return await Task.Run(() => PrintWithWpf(printTicket, false));
        }

        /// <summary>
        /// Prints the rendered report with using WPF technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Wpf assembly.
        /// </summary>
        /// <param name="printTicket">Specifies information about how a document is printed.</param>
		public StiReport PrintWithWpf(object printTicket)
        {
            return PrintWithWpf(printTicket, true);
        }

        /// <summary>
        /// Prints the rendered report with using WPF technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Wpf assembly.
        /// </summary>
        /// <param name="showPrintDialog">Show the print dialog or no.</param>
		public StiReport PrintWithWpf(bool showPrintDialog)
        {
            return PrintWithWpf(null, showPrintDialog);
        }

        /// <summary>
        /// Prints the rendered report with using WPF technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Wpf assembly.
        /// </summary>
        /// <param name="printTicket">Specifies information about how a document is printed.</param>
        /// <param name="showPrintDialog">Show the print dialog or no.</param>
		public StiReport PrintWithWpf(object printTicket, bool showPrintDialog)
        {
            return PrintWithWpf(printTicket, showPrintDialog, -1, -1, -1, null);
        }

        /// <summary>
        /// Prints the rendered report with using WPF technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Wpf assembly.
        /// </summary>
        /// <param name="copies">Number of copies to print.</param>
        public async Task<StiReport> PrintWithWpfAsync(int copies)
        {
            return await Task.Run(() => PrintWithWpf(false, copies));
        }

        /// <summary>
        /// Prints the rendered report with using WPF technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Wpf assembly.
        /// </summary>
        /// <param name="copies">Number of copies to print.</param>
		public StiReport PrintWithWpf(int copies)
        {
            return PrintWithWpf(true, copies);
        }

        /// <summary>
        /// Prints the rendered report with using WPF technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Wpf assembly.
        /// </summary>
        /// <param name="showPrintDialog">Show the print dialog or no.</param>
        /// <param name="copies">Number of copies to print.</param>
		public StiReport PrintWithWpf(bool showPrintDialog, int copies)
        {
            return PrintWithWpf(showPrintDialog, copies, null);
        }

        /// <summary>
        /// Prints the rendered report with using WPF technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Wpf assembly.
        /// </summary>
        /// <param name="copies">Number of copies to print.</param>
        /// <param name="printerName">A name of a printer that will be used for printing.</param>
        public async Task<StiReport> PrintWithWpfAsync(int copies, string printerName)
        {
            return await Task.Run(() => PrintWithWpf(false, -1, -1, copies, printerName));
        }

        /// <summary>
        /// Prints the rendered report with using WPF technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Wpf assembly.
        /// </summary>
        /// <param name="showPrintDialog">Show the print dialog or no.</param>
        /// <param name="copies">Number of copies to print.</param>
        /// <param name="printerName">A name of a printer that will be used for printing.</param>
		public StiReport PrintWithWpf(bool showPrintDialog, int copies, string printerName)
        {
            return PrintWithWpf(showPrintDialog, -1, -1, copies, printerName);
        }

        /// <summary>
        /// Prints the rendered report with using WPF technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Wpf assembly.
        /// </summary>
        /// <param name="fromPage">Number of the first page to print. Starts from 1.</param>
        /// <param name="toPage">Number of the last page to print. Starts from 1.</param>
        /// <param name="copies">Number of copies to print.</param>
        public async Task<StiReport> PrintWithWpfAsync(int fromPage, int toPage, int copies)
        {
            return await Task.Run(() => PrintWithWpf(false, fromPage, toPage, copies, null));
        }

        /// <summary>
        /// Prints the rendered report with using WPF technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Wpf assembly.
        /// </summary>
        /// <param name="showPrintDialog">Show the print dialog or no.</param>
        /// <param name="fromPage">Number of the first page to print. Starts from 1.</param>
        /// <param name="toPage">Number of the last page to print. Starts from 1.</param>
        /// <param name="copies">Number of copies to print.</param>
		public StiReport PrintWithWpf(bool showPrintDialog, int fromPage, int toPage, int copies)
        {
            return PrintWithWpf(showPrintDialog, fromPage, toPage, copies, null);
        }

        /// <summary>
        /// Prints the rendered report with using WPF technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Wpf assembly.
        /// </summary>
        /// <param name="fromPage">Number of the first page to print. Starts from 1.</param>
        /// <param name="toPage">Number of the last page to print. Starts from 1.</param>
        /// <param name="copies">Number of copies to print.</param>
        /// <param name="printerName">A name of a printer that will be used for printing.</param>
        public async Task<StiReport> PrintWithWpfAsync(int fromPage, int toPage, int copies, string printerName)
        {
            return await Task.Run(() => PrintWithWpf(false, fromPage, toPage, copies, printerName));
        }

        /// <summary>
        /// Prints the rendered report with using WPF technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Wpf assembly.
        /// </summary>
        /// <param name="showPrintDialog">Show the print dialog or no.</param>
        /// <param name="fromPage">Number of the first page to print. Starts from 1.</param>
        /// <param name="toPage">Number of the last page to print. Starts from 1.</param>
        /// <param name="copies">Number of copies to print.</param>
        /// <param name="printerName">A name of a printer that will be used for printing.</param>
		public StiReport PrintWithWpf(bool showPrintDialog, int fromPage, int toPage, int copies, string printerName)
        {
            return PrintWithWpf(null, showPrintDialog, fromPage, toPage, copies, printerName);
        }

        /// <summary>
        /// Prints the rendered report with using WPF technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Wpf assembly.
        /// </summary>
        /// <param name="printTicket">Specifies information about how a document is printed.</param>
        /// <param name="fromPage">Number of the first page to print. Starts from 1.</param>
        /// <param name="toPage">Number of the last page to print. Starts from 1.</param>
        /// <param name="copies">Number of copies to print.</param>
        /// <param name="printerName">A name of a printer that will be used for printing.</param>
        public async Task<StiReport> PrintWithWpfAsync(object printTicket, int fromPage, int toPage, int copies, string printerName)
        {
            return await Task.Run(() => PrintWithWpf(printTicket, false, fromPage, toPage, copies, printerName));
        }

        /// <summary>
        /// Prints the rendered report with using WPF technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Wpf assembly.
        /// </summary>
        /// <param name="printTicket">Specifies information about how a document is printed.</param>
        /// <param name="showPrintDialog">Show the print dialog or no.</param>
        /// <param name="fromPage">Number of the first page to print. Starts from 1.</param>
        /// <param name="toPage">Number of the last page to print. Starts from 1.</param>
        /// <param name="copies">Number of copies to print.</param>
        /// <param name="printerName">A name of a printer that will be used for printing.</param>
		public StiReport PrintWithWpf(object printTicket, bool showPrintDialog, int fromPage, int toPage, int copies, string printerName)
        {
            InvokePrinting();
            RegReportDataSources();
            if (!IsRendered) this.RenderWithWpf(true);

            StiLogService.Write(this.GetType(), "Printing report");
            try
            {
                var reportForPrint = CheckNeedsCompiling() && CompiledReport != null ? CompiledReport : this;
                reportForPrint.PrintWithWpfInternal(printTicket, showPrintDialog, fromPage, toPage, copies, printerName);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Printing report...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
            InvokePrinted();

            return this;
        }

        /// <summary>
        /// Prints the rendered report with using WPF technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Wpf assembly.
        /// </summary>
        /// <param name="printTicket">Specifies information about how a document is printed.</param>
        /// <param name="showPrintDialog">Show the print dialog or no.</param>
        /// <param name="fromPage">Number of the first page to print. Starts from 1.</param>
        /// <param name="toPage">Number of the last page to print. Starts from 1.</param>
        /// <param name="copies">Number of copies to print.</param>
        /// <param name="printerName">A name of a printer that will be used for printing.</param>
        private StiReport PrintWithWpfInternal(object printTicket, bool showPrintDialog, int fromPage, int toPage, int copies, string printerName)
        {
            var type = Type.GetType("Stimulsoft.Report.Print.StiWpfPrintProvider, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo);
            if (type == null)
                throw new Exception("Assembly 'Stimulsoft.Report.Wpf' is not found");

            var wpfPrintProvider = StiActivator.CreateObject(type, new object[] { this }) as IStiWpfPrintProvider;
            wpfPrintProvider.Print(printTicket, showPrintDialog, fromPage, toPage, copies, printerName);

            return this;
        }
        #endregion

        #region Methods.XBAP
        /// <summary>
        /// Prints the rendered report with using Xbap technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Xbap assembly.
        /// </summary>
        [Obsolete("The 'PrintWithXbap' method is deprecated!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public StiReport PrintWithXbap()
        {
            return PrintWithXbap(true);
        }

        /// <summary>
        /// Prints the rendered report with using Xbap technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Xbap assembly.
        /// </summary>
        /// <param name="printerName">A name of a printer that will be used for printing.</param>
        [Obsolete("The 'PrintWithXbap' method is deprecated!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public StiReport PrintWithXbap(string printerName)
        {
            return PrintWithXbap(true, -1, printerName);
        }

        /// <summary>
        /// Prints the rendered report with using Xbap technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Xbap assembly.
        /// </summary>
        /// <param name="printTicket">Specifies information about how a document is printed.</param>
        [Obsolete("The 'PrintWithXbap' method is deprecated!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public StiReport PrintWithXbap(object printTicket)
        {
            return PrintWithXbap(printTicket, true);
        }

        /// <summary>
        /// Prints the rendered report with using Xbap technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Xbap assembly.
        /// </summary>
        /// <param name="showPrintDialog">Show the print dialog or no.</param>
        [Obsolete("The 'PrintWithXbap' method is deprecated!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public StiReport PrintWithXbap(bool showPrintDialog)
        {
            return PrintWithXbap(null, showPrintDialog);
        }

        /// <summary>
        /// Prints the rendered report with using Xbap technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Xbap assembly.
        /// </summary>
        /// <param name="printTicket">Specifies information about how a document is printed.</param>
        /// <param name="showPrintDialog">Show the print dialog or no.</param>
        [Obsolete("The 'PrintWithXbap' method is deprecated!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public StiReport PrintWithXbap(object printTicket, bool showPrintDialog)
        {
            return PrintWithXbap(printTicket, showPrintDialog, -1, -1, -1, null);
        }

        /// <summary>
        /// Prints the rendered report with using Xbap technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Xbap assembly.
        /// </summary>
        /// <param name="copies">Number of copies to print.</param>
        [Obsolete("The 'PrintWithXbap' method is deprecated!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public StiReport PrintWithXbap(int copies)
        {
            return PrintWithXbap(true, copies);
        }

        /// <summary>
        /// Prints the rendered report with using Xbap technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Xbap assembly.
        /// </summary>
        /// <param name="showPrintDialog">Show the print dialog or no.</param>
        /// <param name="copies">Number of copies to print.</param>
        [Obsolete("The 'PrintWithXbap' method is deprecated!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public StiReport PrintWithXbap(bool showPrintDialog, int copies)
        {
            return PrintWithXbap(showPrintDialog, copies, null);
        }

        /// <summary>
        /// Prints the rendered report with using Xbap technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Xbap assembly.
        /// </summary>
        /// <param name="showPrintDialog">Show the print dialog or no.</param>
        /// <param name="copies">Number of copies to print.</param>
        /// <param name="printerName">A name of a printer that will be used for printing.</param>
        [Obsolete("The 'PrintWithXbap' method is deprecated!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public StiReport PrintWithXbap(bool showPrintDialog, int copies, string printerName)
        {
            return PrintWithXbap(showPrintDialog, -1, -1, copies, printerName);
        }

        /// <summary>
        /// Prints the rendered report with using Xbap technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Xbap assembly.
        /// </summary>
        /// <param name="showPrintDialog">Show the print dialog or no.</param>
        /// <param name="fromPage">Number of the first page to print. Starts from 1.</param>
        /// <param name="toPage">Number of the last page to print. Starts from 1.</param>
        /// <param name="copies">Number of copies to print.</param>
        [Obsolete("The 'PrintWithXbap' method is deprecated!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public StiReport PrintWithXbap(bool showPrintDialog, int fromPage, int toPage, int copies)
        {
            return PrintWithXbap(showPrintDialog, fromPage, toPage, copies, null);
        }

        /// <summary>
        /// Prints the rendered report with using Xbap technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Xbap assembly.
        /// </summary>
        /// <param name="showPrintDialog">Show the print dialog or no.</param>
        /// <param name="fromPage">Number of the first page to print. Starts from 1.</param>
        /// <param name="toPage">Number of the last page to print. Starts from 1.</param>
        /// <param name="copies">Number of copies to print.</param>
        /// <param name="printerName">A name of a printer that will be used for printing.</param>
        [Obsolete("The 'PrintWithXbap' method is deprecated!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public StiReport PrintWithXbap(bool showPrintDialog, int fromPage, int toPage, int copies, string printerName)
        {
            return PrintWithXbap(null, showPrintDialog, fromPage, toPage, copies, printerName);
        }

        /// <summary>
        /// Prints the rendered report with using Xbap technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Xbap assembly.
        /// </summary>
        /// <param name="printTicket">Specifies information about how a document is printed.</param>
        /// <param name="showPrintDialog">Show the print dialog or no.</param>
        /// <param name="fromPage">Number of the first page to print. Starts from 1.</param>
        /// <param name="toPage">Number of the last page to print. Starts from 1.</param>
        /// <param name="copies">Number of copies to print.</param>
        /// <param name="printerName">A name of a printer that will be used for printing.</param>
        [Obsolete("The 'PrintWithXbap' method is deprecated!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public StiReport PrintWithXbap(object printTicket, bool showPrintDialog, int fromPage, int toPage, int copies, string printerName)
        {
            InvokePrinting();
            RegReportDataSources();
            if (!IsRendered) this.RenderWithWpf(true);

            StiLogService.Write(this.GetType(), "Printing report");
            try
            {
                if (CheckNeedsCompiling())
                    this.CompiledReport.PrintWithXbapInternal(printTicket, showPrintDialog, fromPage, toPage, copies, printerName);
                else
                    this.PrintWithXbapInternal(printTicket, showPrintDialog, fromPage, toPage, copies, printerName);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Printing report...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
            InvokePrinted();
            return this;
        }

        /// <summary>
        /// Prints the rendered report with using Xbap technology. If the report is not rendered then its rendering starts. This method require Stimulsoft.Report.Xbap assembly.
        /// </summary>
        /// <param name="printTicket">Specifies information about how a document is printed.</param>
        /// <param name="showPrintDialog">Show the print dialog or no.</param>
        /// <param name="fromPage">Number of the first page to print. Starts from 1.</param>
        /// <param name="toPage">Number of the last page to print. Starts from 1.</param>
        /// <param name="copies">Number of copies to print.</param>
        /// <param name="printerName">A name of a printer that will be used for printing.</param>
        private void PrintWithXbapInternal(object printTicket, bool showPrintDialog, int fromPage, int toPage, int copies, string printerName)
        {
            var type = Type.GetType("Stimulsoft.Report.Print.StiWpfPrintProvider, Stimulsoft.Report.Xbap, " + StiVersion.VersionInfo);
            if (type == null)
                throw new Exception("Assembly 'Stimulsoft.Report.Xbap' is not found");

            var wpfPrintProvider = StiActivator.CreateObject(type, new object[] { this }) as IStiWpfPrintProvider;
            wpfPrintProvider.Print(printTicket, showPrintDialog, fromPage, toPage, copies, printerName);
        }
        #endregion

        #region Methods.DotMatrixPrinter
        /// <summary>
        /// Prints a report to default dot-matrix printer in the RAW mode in ASCII encoding.
        /// </summary>
        public StiReport PrintToDotMatrixPrinter()
        {
            var settings = new PrinterSettings();
            return PrintToDotMatrixPrinter(settings.PrinterName, Encoding.ASCII);
        }

        /// <summary>
        /// Prints a report to dot-matrix printer in the RAW mode in ASCII encoding.
        /// </summary>
        /// <param name="printerName">A name of a printer.</param>
		public StiReport PrintToDotMatrixPrinter(string printerName)
        {
            return PrintToDotMatrixPrinter(printerName, Encoding.ASCII);
        }

        /// <summary>
        /// Prints report to dot-matrix printer in the RAW mode.
        /// </summary>
        /// <param name="printerName">A name of a printer.</param>
        /// <param name="encoding">A parameter which sets text encoding.</param>
		public StiReport PrintToDotMatrixPrinter(string printerName, Encoding encoding)
        {
            return PrintToDotMatrixPrinter(printerName, encoding, -1, -1);
        }

        /// <summary>
        /// Prints a report to dot-matrix printer in the RAW mode.
        /// </summary>
        /// <param name="printerName">A name of a printer.</param>
        /// <param name="encoding">A parameter which sets text encoding.</param>
        /// <param name="fromPage">A number of the first page to print.</param>
        /// <param name="toPage">A number of the last page to print.</param>
		public StiReport PrintToDotMatrixPrinter(string printerName, Encoding encoding, int fromPage, int toPage)
        {
            StiDotMatrixPrintProvider.PrintToDotMatrixPrinter(
                this, printerName, encoding,
                fromPage, toPage);

            return this;
        }

        /// <summary>
        /// Prints a report to dot-matrix printer in RAW mode.
        /// </summary>
        /// <param name="printerName">A name of the printer.</param>
        /// <param name="encoding">A parameter which sets text encoding.</param>
        /// <param name="drawBorder">If true then borders are exported to a text.</param>
        /// <param name="borderType">Type of borders (StiTxtBorderType).</param>
        /// <param name="putFeedPageCode">If true then the EOF character will be added to the end of each page.</param>
        /// <param name="cutLongLines">If true then all long lines will be cut.</param>
        /// <param name="fromPage">A number of the first page to print.</param>
        /// <param name="toPage">A number of the last page to print.</param>
		public StiReport PrintToDotMatrixPrinter(string printerName, Encoding encoding,
			bool drawBorder, StiTxtBorderType borderType, 
			bool putFeedPageCode, bool cutLongLines, int fromPage, int toPage)
        {
            StiDotMatrixPrintProvider.PrintToDotMatrixPrinter(
                this, printerName, encoding,
                drawBorder, borderType, putFeedPageCode, cutLongLines,
                fromPage, toPage);

            return this;
        }

        /// <summary>
        /// Prints report to dot-matrix printer in the RAW mode.
        /// </summary>
        /// <param name="printerName">A name of the printer.</param>
        /// <param name="encoding">A parameter which sets text encoding.</param>
        /// <param name="drawBorder">If true then borders are exported to the text.</param>
        /// <param name="borderType">Type of borders (StiTxtBorderType).</param>
        /// <param name="killSpaceLines">If true then empty lines will be removed from the result text.</param>
        /// <param name="killSpaceGraphLines">If true then empty lines with vertical borders will be removed from the result text.</param>
        /// <param name="putFeedPageCode">If true then the EOF character will be added to the end of each page.</param>
        /// <param name="cutLongLines">If true then all long lines will be cut.</param>
        /// <param name="zoomX">Horizontal zoom factor by X axis. By default a value is 1.0f what is equal 100% in export settings window.</param>
        /// <param name="zoomY">Vertical zoom factor by Y axis. By default a value is 1.0f what is equal 100% in export settings window.</param>
        /// <param name="fromPage">A number of the first page to print.</param>
        /// <param name="toPage">A number of the last page to print.</param>
		public StiReport PrintToDotMatrixPrinter(string printerName, Encoding encoding,
			bool drawBorder, StiTxtBorderType borderType, bool killSpaceLines, bool killSpaceGraphLines,
			bool putFeedPageCode, bool cutLongLines, float zoomX, float zoomY, int fromPage, int toPage,
            bool useEscapeCodes, string escapeCodesCollectionName)
       {
            StiDotMatrixPrintProvider.PrintToDotMatrixPrinter(
                this, printerName, encoding,
                drawBorder, borderType, killSpaceLines,
                killSpaceGraphLines, putFeedPageCode, cutLongLines,
                zoomX, zoomY, fromPage, toPage, useEscapeCodes, escapeCodesCollectionName);

            return this;
        }
        #endregion
    }
}