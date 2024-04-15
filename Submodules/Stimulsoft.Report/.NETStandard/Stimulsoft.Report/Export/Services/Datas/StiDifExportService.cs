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
using System.Text;
using System.IO;
using Stimulsoft.Report.Components;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Helpers;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// A class for the DIF export.
    /// </summary>
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourceDif.png")]
	public class StiDifExportService : StiExportService
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
		public override string DefaultExtension => "dif";

		public override StiExportFormat ExportFormat => StiExportFormat.Dif;

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
		public override string ExportNameInMenu => StiLocalization.Get("Export", "ExportTypeDifFile");

        /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public override void ExportTo(StiReport report, Stream stream, StiExportSettings settings)
        {
            ExportDif(report, stream, settings as StiDataExportSettings);
			InvokeExporting(100, 100, 1, 1);
		}

	    /// <summary>
		/// Exports a rendered report to a dif file.
		/// </summary>
		/// <param name="report">A report which is to be exported.</param>
		/// <param name="fileName">A name of the file for exporting a rendered report.</param>
		/// <param name="sendEMail">A parameter indicating whether the exported report will be sent via e-mail.</param>
        public override void Export(StiReport report, string fileName, bool sendEMail, StiGuiMode guiMode)
		{
            using (var form = StiGuiOptions.GetExportFormRunner("StiDifExportSetupForm", guiMode, this.OwnerWindow))
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
		public override string GetFilter() => StiLocalization.Get("FileFilters", "DifFiles");
		#endregion

		#region Handlers
		private void Form_Complete(IStiFormRunner form, StiShowDialogCompleteEvetArgs e)
		{
			if (e.DialogResult)
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
								ExportDataOnly = (bool)form["ExportDataOnly"],
								Encoding = form["Encoding"] as Encoding,
								UseDefaultSystemEncoding = (bool)form["UseDefaultSystemEncoding"]
							};

							base.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], fileName, guiMode);
						}
					}
					catch (Exception ex)
					{
						throw ex;
					}
				}
			}
			else IsStopped = true;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Exports a rendered report to a Dif file.
		/// </summary>
		/// <param name="report">A report which is to be exported.</param>
		/// <param name="fileName">A name of the file for exporting a rendered report.</param>
		public void ExportDif(StiReport report, string fileName)
		{
			StiFileUtils.ProcessReadOnly(fileName);
			using (var stream = new FileStream(fileName, FileMode.Create))
			{
				ExportDif(report, stream);
				stream.Flush();
				stream.Close();
			}
		}

       
		/// <summary>
		/// Exports a rendered report to a Dif file.
		/// </summary>
		/// <param name="report">A report which is to be exported.</param>
		/// <param name="stream">A stream for export of a document.</param>
		public void ExportDif(StiReport report, Stream stream)
		{
			ExportDif(report, stream, new StiDataExportSettings());
		}
        
    
		/// <summary>
		/// Exports a rendered report to a Dif file.
		/// </summary>
		/// <param name="report">A report which is to be exported.</param>
		/// <param name="stream">A stream for export of a document.</param>
		/// <param name="pageRange">Describes range of  pages of the document for the export.</param>
		/// <param name="encoding">Encoding for the result file.</param>
		public void ExportDif(StiReport report, Stream stream, StiPagesRange pageRange, bool exportDataOnly, bool useDefaultSystemEncoding,
			Encoding encoding)
		{
            var settings = new StiDataExportSettings
            {
                PageRange = pageRange,
                ExportDataOnly = exportDataOnly,
                UseDefaultSystemEncoding = useDefaultSystemEncoding,
                Encoding = encoding
            };

            ExportDif(report, stream, settings);
		}

		/// <summary>
		/// Exports a rendered report to a Dif file.
		/// </summary>
        /// <param name="report">A report which is to be exported.</param>
		/// <param name="stream">A stream for export of a document.</param>
        public void ExportDif(StiReport report, Stream stream, StiDataExportSettings settings)
		{
			StiLogService.Write(this.GetType(), "Export report to Dif format");

            #region Export Dashboard
            if (!report.IsDocument && report.GetCurrentPage() is IStiDashboard)
            {
                StiDashboardExport.Export(report, stream, settings);
                return;
            }
            #endregion

#if NETSTANDARD || NETCOREAPP
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif

            #region Read settings
            if (settings == null)
                throw new ArgumentNullException("The 'settings' argument cannot be equal in null.");

            var pageRange = settings.PageRange;
			var exportDataOnly = settings.ExportDataOnly;
			var useDefaultSystemEncoding = settings.UseDefaultSystemEncoding;
			var encoding = settings.Encoding;
			#endregion

			if (useDefaultSystemEncoding)
			{
				encoding = Encoding.GetEncoding(CultureInfo.InstalledUICulture.TextInfo.OEMCodePage);
			}
			writer = new StreamWriter(stream, encoding);

			StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");

			var pages = pageRange.GetSelectedPages(report.RenderedPages);

            CurrentPassNumber = 0;
            MaximumPassNumber = 3;

			var dataMode = exportDataOnly ? StiDataExportMode.Data | StiDataExportMode.Headers : StiDataExportMode.AllBands;

			var matrix = new StiMatrix(pages, true, this, null, dataMode);
			matrix.ScanComponentsPlacement(false);
			if (IsStopped) return;

			#region Write header
			writer.WriteLine("TABLE");
			writer.WriteLine("0,1");
			writer.WriteLine("\"EXCEL\"");
			writer.WriteLine("VECTORS");
			writer.WriteLine(string.Format("0,{0}", matrix.CoordX.Count - 1));
			writer.WriteLine("\"\"");
			writer.WriteLine("TUPLES");
			writer.WriteLine(string.Format("0,{0}", matrix.CoordY.Count - 1));
			writer.WriteLine("\"\"");
			writer.WriteLine("DATA");
			writer.WriteLine("0,0");
			writer.WriteLine("\"\"");
			#endregion

		    CurrentPassNumber = 2;

			if (exportDataOnly)
			{
                matrix.PrepareDocument(this, dataMode);
				for (int rowIndex = 0; rowIndex < matrix.DataArrayLength; rowIndex++)
				{
					InvokeExporting(rowIndex, matrix.DataArrayLength - 1, CurrentPassNumber, MaximumPassNumber);
					if (IsStopped) return;
					writer.WriteLine("-1,0");
					writer.WriteLine("BOT");
					for (int columnIndex = 0; columnIndex < matrix.Fields.Length; columnIndex ++)
					{
						#region Export cell
						string text = matrix.Fields[columnIndex].DataArray[rowIndex];
						if (text == null) text = string.Empty;

						bool isValueFormat = (StiExportDataType)matrix.Fields[columnIndex].Info[0] != StiExportDataType.String;
						if (isValueFormat)
						{
							writer.WriteLine("0,{0}", text);
							writer.WriteLine("V");
						}
						else
						{
							writer.WriteLine("1,0");
							writer.WriteLine("\"{0}\"", text);
						}
						#endregion
					}
				}
			}
			else
			{
				RichTextBox richtextForConvert = null;
				for (int rowIndex = 0; rowIndex < matrix.CoordY.Count - 1; rowIndex++)
				{
                    InvokeExporting(rowIndex, matrix.CoordY.Count - 1, CurrentPassNumber, MaximumPassNumber);
					if (IsStopped) return;
					writer.WriteLine("-1,0");
					writer.WriteLine("BOT");
					for (int columnIndex = 0; columnIndex < matrix.CoordX.Count - 1; columnIndex ++)
					{
						#region Export cell
						string text = null;
						string inputFormat = string.Empty;
						var cell = matrix.Cells[rowIndex, columnIndex];
						if (cell != null)
						{
							text = cell.Text;
							var rtf = cell.Component as StiRichText;
							if ((rtf != null) && (rtf.RtfText != string.Empty))
							{
								if (richtextForConvert == null) richtextForConvert = new Controls.StiRichTextBox(false);
								rtf.GetPreparedText(richtextForConvert);
								text = richtextForConvert.Text;
							}
							if (text == null) text = string.Empty;

							var textComp = cell.Component as StiText;
							if (textComp != null) inputFormat = textComp.Format; 
						}
						if (text == null) text = string.Empty;

						bool isValueFormat = false;
						if (!string.IsNullOrEmpty(inputFormat))
						{
							if ((inputFormat[0] == 'C') ||
								(inputFormat[0] == 'N') ||
								(inputFormat[0] == 'P') ||
								(inputFormat[0] == 'D') ||
								(inputFormat[0] == 'T'))
							{
								isValueFormat = true;
							}
						}

						if (isValueFormat)
						{
							writer.WriteLine("0,{0}", text);
							writer.WriteLine("V");
						}
						else
						{
							writer.WriteLine("1,0");
							writer.WriteLine("\"{0}\"", text);
						}
						#endregion
					}
				}
			}

			writer.WriteLine("-1,0");
			writer.WriteLine("EOD");

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