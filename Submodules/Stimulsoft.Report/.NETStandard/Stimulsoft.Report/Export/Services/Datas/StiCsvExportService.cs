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
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Helpers;
using System;
using System.IO;
using System.Text;

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// A class for the Csv export.
    /// </summary>
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourceCsv.png")]
	public class StiCsvExportService : StiExportService
    {
        #region Fields
        private StiReport report;
        private string fileName;
        private bool sendEMail;
        private StiGuiMode guiMode;
        private StreamWriter writer;
        #endregion

        #region StiExportService override
        /// <summary>
        /// Gets or sets a default extension of export. 
        /// </summary>
        public override string DefaultExtension => "csv";

		public override StiExportFormat ExportFormat => StiExportFormat.Csv;

		/// <summary>
        /// Gets a group of the export in the context menu.
		/// </summary>
		public override string GroupCategory => "Data";

		/// <summary>
        /// Gets a position of the export in the context menu.
		/// </summary>
		public override int Position => (int)StiExportPosition.Data;

        /// <summary>
        /// Gets a name of the export in the context menu.
        /// </summary>
		public override string ExportNameInMenu => StiLocalization.Get("Export", "ExportTypeCsvFile");

        /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public override void ExportTo(StiReport report, Stream stream, StiExportSettings settings)
        {
            ExportCsv(report, stream, settings as StiDataExportSettings);
            InvokeExporting(100, 100, 1, 1);
        }

		/// <summary>
		/// Exports a rendered report to a csv file.
		/// </summary>
		/// <param name="report">A report which is to be exported.</param>
		/// <param name="fileName">A name of the file for exporting a rendered report.</param>
		/// <param name="sendEMail">A parameter indicating whether the exported report will be sent via e-mail.</param>
        public override void Export(StiReport report, string fileName, bool sendEMail, StiGuiMode guiMode)
		{
            using (var form = StiGuiOptions.GetExportFormRunner("StiDataSetupForm", guiMode, this.OwnerWindow))
			{
                form["CurrentPage"] = report.CurrentPrintPage;
                form["OpenAfterExportEnabled"] = !sendEMail;

                this.report = report;
                this.fileName = fileName;
                this.sendEMail = sendEMail;
                this.guiMode = guiMode;
                form.Complete += Form_Complete;
                form.ShowDialog();
			}
		}

        /// <summary>
        /// Gets a value indicating a number of files in exported document as a result of export
        /// of one page of the rendered report.
        /// </summary>        
        public override bool MultipleFiles => false;

		/// <summary>
		/// Returns the filter of all available services which serves for saving, loading a document.
		/// </summary>
		/// <returns>Filter.</returns>
		public override string GetFilter() => StiLocalization.Get("FileFilters", "CsvFiles");
        #endregion

        #region Handlers
        private void Form_Complete(IStiFormRunner form, StiShowDialogCompleteEvetArgs e)
        {
            if (e.DialogResult)
            {
                var exportFormat = (StiExportFormat)form["ExportFormat"];

                #region Dbf
                if (exportFormat == StiExportFormat.Dbf)
                {
                    var dbfExportService = new StiDbfExportService();

                    if (string.IsNullOrEmpty(fileName))
                        fileName = dbfExportService.GetFileName(report, sendEMail);

                    if (fileName != null)
                    {
                        var codePage = (StiDbfCodePages)Enum.Parse(typeof(StiDbfCodePages),
                            StiDbfExportService.codePageCodes[(int)form["EncodingSelectedIndex"], 1].ToString());

                        StiFileUtils.ProcessReadOnly(fileName);
                        try
                        {
                            using (var stream = new FileStream(fileName, FileMode.Create))
                            {
                                dbfExportService.StartProgress(guiMode);

                                var settings = new StiDbfExportSettings
                                {
                                    PageRange = form["PagesRange"] as StiPagesRange,
                                    CodePage = codePage
                                };

                                dbfExportService.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], fileName, guiMode);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                #endregion

                #region Xml
                else if (exportFormat == StiExportFormat.Xml)
                {
                    var xmlExportService = new StiXmlExportService();

                    if (string.IsNullOrEmpty(fileName))
                        fileName = xmlExportService.GetFileName(report, sendEMail);

                    if (fileName != null)
                    {
                        StiFileUtils.ProcessReadOnly(fileName);

                        try
                        {
                            using (var stream = new FileStream(fileName, FileMode.Create))
                            {
                                xmlExportService.StartProgress(guiMode);

                                var settings = new StiXmlExportSettings();

                                int mode = (int)form["DataExportMode"];
                                if (mode == 1) settings.DataExportMode = StiDataExportMode.DataAndHeadersFooters;
                                if (mode == 2) settings.DataExportMode = StiDataExportMode.AllBands;

                                xmlExportService.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], fileName, guiMode);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                #endregion

                #region Json
                else if (exportFormat == StiExportFormat.Json)
                {
                    var jsonExportService = new StiJsonExportService();

                    if (string.IsNullOrEmpty(fileName))
                        fileName = jsonExportService.GetFileName(report, sendEMail);

                    if (fileName != null)
                    {
                        StiFileUtils.ProcessReadOnly(fileName);

                        try
                        {
                            using (var stream = new FileStream(fileName, FileMode.Create))
                            {
                                jsonExportService.StartProgress(guiMode);

                                var settings = new StiJsonExportSettings();

                                int mode = (int)form["DataExportMode"];
                                if (mode == 1) settings.DataExportMode = StiDataExportMode.DataAndHeadersFooters;
                                if (mode == 2) settings.DataExportMode = StiDataExportMode.AllBands;

                                jsonExportService.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], fileName, guiMode);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                #endregion

                #region Dif
                else if (exportFormat == StiExportFormat.Dif)
                {
                    var difExportService = new StiDifExportService();

                    if (string.IsNullOrEmpty(fileName))
                        fileName = difExportService.GetFileName(report, sendEMail);

                    if (fileName != null)
                    {
                        StiFileUtils.ProcessReadOnly(fileName);
                        try
                        {
                            using (var stream = new FileStream(fileName, FileMode.Create))
                            {
                                difExportService.StartProgress(guiMode);

                                var settings = new StiDifExportSettings
                                {
                                    PageRange = form["PagesRange"] as StiPagesRange,
                                    ExportDataOnly = (bool)form["ExportDataOnly"],
                                    Encoding = form["Encoding"] as Encoding,
                                    UseDefaultSystemEncoding = (bool)form["UseDefaultSystemEncoding"]
                                };

                                difExportService.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], fileName, guiMode);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                #endregion

                #region Sylk
                else if (exportFormat == StiExportFormat.Sylk)
                {
                    var sylkExportService = new StiSylkExportService();

                    if (string.IsNullOrEmpty(fileName))
                        fileName = sylkExportService.GetFileName(report, sendEMail);

                    if (fileName != null)
                    {
                        StiFileUtils.ProcessReadOnly(fileName);
                        try
                        {
                            using (var stream = new FileStream(fileName, FileMode.Create))
                            {
                                sylkExportService.StartProgress(guiMode);

                                var settings = new StiSylkExportSettings
                                {
                                    PageRange = form["PagesRange"] as StiPagesRange,
                                    ExportDataOnly = (bool)form["ExportDataOnly"],
                                    Encoding = form["Encoding"] as Encoding,
                                    UseDefaultSystemEncoding = (bool)form["UseDefaultSystemEncoding"]
                                };

                                sylkExportService.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], fileName, guiMode);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                #endregion

                #region Csv
                else
                {

                    if (string.IsNullOrEmpty(fileName))
                        fileName = base.GetFileName(report, sendEMail);

                    if (fileName != null)
                    {
                        StiFileUtils.ProcessReadOnly(fileName);
                        try
                        {
                            using (var stream = new FileStream(fileName, FileMode.Create))
                            {
                                StartProgress(guiMode);

                                var settings = new StiDataExportSettings
                                {
                                    PageRange = form["PagesRange"] as StiPagesRange,
                                    Separator = form["Separator"] as string,
                                    Encoding = form["Encoding"] as Encoding,
                                    SkipColumnHeaders = (bool)form["SkipColumnHeaders"]
                                };

                                int mode = (int)form["DataExportMode"];
                                if (mode == 1) settings.DataExportMode = StiDataExportMode.DataAndHeadersFooters;
                                if (mode == 2) settings.DataExportMode = StiDataExportMode.AllBands;

                                base.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], fileName, guiMode);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                #endregion
            }
            else IsStopped = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Exports a rendered report to a csv file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        public void ExportCsv(StiReport report, string fileName)
		{
			StiFileUtils.ProcessReadOnly(fileName);
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                ExportCsv(report, stream);
                stream.Flush();
                stream.Close();
            }
		}

       
		/// <summary>
		/// Exports a rendered report to a csv file.
		/// </summary>
		/// <param name="report">A report which is to be exported.</param>
		/// <param name="stream">A stream for export of a document.</param>
		public void ExportCsv(StiReport report, Stream stream)
		{
            var settings = new StiDataExportSettings();
			ExportCsv(report, stream, settings);
		}
        
    
		/// <summary>
		/// Exports a rendered report to a csv file.
		/// </summary>
		/// <param name="report">A report which is to be exported.</param>
		/// <param name="stream">A stream for export of a document.</param>
		/// <param name="pageRange">Describes range of  pages of the document for the export.</param>
		/// <param name="separator">A separator for the resulted csv file.</param>
		/// <param name="encoding">Encoding for the result file.</param>
		public void ExportCsv(StiReport report, Stream stream, StiPagesRange pageRange, string separator, 
			Encoding encoding)
		{
            var settings = new StiDataExportSettings
            {
                PageRange = pageRange,
                Separator = separator,
                Encoding = encoding
            };

            ExportCsv(report, stream, settings);
		}

		/// <summary>
		/// Exports a rendered report to a csv file.
		/// </summary>
		/// <param name="report">A report which is to be exported.</param>
		/// <param name="stream">A stream for export of a document.</param>
        public void ExportCsv(StiReport report, Stream stream, StiDataExportSettings settings)
		{
			StiLogService.Write(this.GetType(), "Export report to Csv format");

            #region Export Dashboard
            if (!report.IsDocument && report.GetCurrentPage() is IStiDashboard)
            {
                StiDashboardExport.Export(report, stream, settings);
                return;
            }
            #endregion

            #region Read settings
            if (settings == null)
                throw new ArgumentNullException("The 'settings' argument cannot be equal in null.");

            var pageRange = settings.PageRange;
			var separator = settings.Separator;
			var encoding = settings.Encoding;
            var skipColumnHeaders = settings.SkipColumnHeaders;
            var mode = settings.DataExportMode;
			#endregion

			if (separator == "\\t") separator = "\t";

			if (StiOptions.Export.Csv.ForcedSeparator != null && StiOptions.Export.Csv.ForcedSeparator.Trim().Length > 0)
				separator = StiOptions.Export.Csv.ForcedSeparator.Trim();

		    var pages = pageRange.GetSelectedPages(report.RenderedPages);

            CurrentPassNumber = 0;
		    MaximumPassNumber = 3;

 			var matrix = new StiMatrix(pages, false, this, null, mode);
			matrix.ScanComponentsPlacement(false);
			if (IsStopped)return;

			matrix.PrepareDocument(this, mode);

			writer = new StreamWriter(stream, encoding);
			StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");

			#region Render document
			if (!skipColumnHeaders)
			{
				for (int columnIndex = 0; columnIndex < matrix.Fields.Length; columnIndex ++)
				{
                    var text = matrix.Fields[columnIndex].Name.Replace("\"", "\"\"");
                    if ((text.IndexOf(separator, StringComparison.InvariantCulture) != -1) || (text.IndexOf('\"') != -1) || (text.IndexOf('\n') != -1)) text = "\"" + text + "\"";
					writer.Write(text);
					if (columnIndex < matrix.Fields.Length - 1)
					{
						writer.Write(separator);
					}
				}
				writer.WriteLine();
			}

            CurrentPassNumber = 2;

            for (int rowIndex = 0; rowIndex < matrix.DataArrayLength; rowIndex++)
			{
                InvokeExporting(rowIndex, matrix.DataArrayLength - 1, CurrentPassNumber, MaximumPassNumber);
				if (IsStopped)return;
				for (int columnIndex = 0; columnIndex < matrix.Fields.Length; columnIndex ++)
				{
                    var text = matrix.Fields[columnIndex].DataArray[rowIndex];
					if (text == null) text = string.Empty;
					if (text != string.Empty)
					{
                        if ((text.IndexOf(separator, StringComparison.InvariantCulture) != -1) ||
                            (text.IndexOf('\"') != -1) || (text.IndexOf('\n') != -1))
						{
							text = text.Replace("\"", "\"\"");
							text = "\"" + text + "\"";
						}
						writer.Write(text);
					}
					if (columnIndex < matrix.Fields.Length - 1)
					{
						writer.Write(separator);
					}
				}
				writer.WriteLine();
			}
			#endregion

			writer.Flush();

			if (matrix != null)
			{
				matrix.Clear();
				matrix = null;
			}
		}
		#endregion
	}
}