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
using System.Xml;
using System.Text;
using System.IO;
using Stimulsoft.Report.Components;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Services;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using Stimulsoft.Report.Helpers;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// A class for the export in the ExcelXml format.
    /// </summary>
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourceExcel.png")]
	public class StiExcelXmlExportService : StiExportService
    {
        #region StiExportService override
        /// <summary>
		/// Gets or sets a default extension of export. 
		/// </summary>
		public override string DefaultExtension => "xls";

		public override StiExportFormat ExportFormat => StiExportFormat.ExcelXml;

		/// <summary>
        /// Gets a group of the export in the context menu.
		/// </summary>
		public override string GroupCategory => "Excel";

		/// <summary>
        /// Gets a position of the export in the context menu.
		/// </summary>
		public override int Position => (int)StiExportPosition.ExcelXml;

        /// <summary>
        /// Gets an export name in the context menu.
        /// </summary>
		public override string ExportNameInMenu => StiLocalization.Get("Export", "ExportTypeExcelXmlFile");
		
        /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public override void ExportTo(StiReport report, Stream stream, StiExportSettings settings)
        {
            ExportExcel(report, stream, settings as StiExcelExportSettings);
			InvokeExporting(100, 100, 1, 1);
		}

        /// <summary>
        /// Exports a rendered report to the ExcelXml file.
        /// Also rendered report can be sent via e-mail.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        /// <param name="sendEMail">A parameter indicating whether the exported report will be sent via e-mail.</param>
        public override void Export(StiReport report, string fileName, bool sendEMail, StiGuiMode guiMode)
		{
            using (var form = StiGuiOptions.GetExportFormRunner("StiExcelSetupForm", guiMode, this.OwnerWindow))
            {
                form["CurrentPage"] = report.CurrentPrintPage;
                form["OpenAfterExportEnabled"] = !sendEMail;
                form["ExportFormat"] = StiExportFormat.ExcelXml;

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
        /// Returns a filter for files of the export format.
        /// </summary>
        /// <returns>String with filter.</returns>
		public override string GetFilter() => StiLocalization.Get("FileFilters", "ExcelXmlFiles");
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
                    this.title = Path.GetFileNameWithoutExtension(fileName);
                    StiFileUtils.ProcessReadOnly(fileName);
					try
					{
						using (var stream = new FileStream(fileName, FileMode.Create))
						{
							StartProgress(guiMode);

							var settings = new StiExcelExportSettings
							{
								ExcelType = StiExcelType.ExcelXml,
								PageRange = form["PagesRange"] as StiPagesRange
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

        #region Fields
        private StiReport report;
        private string fileName;
        private bool sendEMail;
        private StiGuiMode guiMode;
        #endregion

        #region this
        private XmlTextWriter writer;

        private string title;

		private StiMatrix matrix;
		internal StiMatrix Matrix
		{
			get
			{
				return matrix;
			}
		}

		
		private string ConvertColor(Color color)
		{
			int rgb = color.ToArgb() & 0xffffff;
			return ("#" + rgb.ToString("X").PadLeft(6, '0'));
		}
 
		
		private string Convert(double x)
		{
			x = (x / 300f) * 72f * 3.2f * 0.96;
			return (Math.Round((decimal)x, 3)).ToString(CultureInfo.InvariantCulture);
		}

		
		private string ConvertColumn(double x)
		{
			x = (x / 300f) * 72f * 3.2f * 0.864 * 1.015;
			return (Math.Round((decimal)x, 3)).ToString(CultureInfo.InvariantCulture);
		}

		
		private string ConvertPageMargins(double x)
		{
			x = (x / 100f);
			return (Math.Round((decimal)x, 3)).ToString(CultureInfo.InvariantCulture);
		}


		private string GetLineStyle(StiPenStyle penStyle)
		{
			switch (penStyle)
			{
				case StiPenStyle.Solid:
					return "Continuous";

				case StiPenStyle.Dot:
					return "Dot";

				case StiPenStyle.Dash:
					return "Dash";

				case StiPenStyle.DashDot:
					return "DashDot";
				
				case StiPenStyle.DashDotDot:
					return "DashDotDot";

				case StiPenStyle.Double:
					return "Double";

				default:
					return "None";
			}
		}


		private void RenderBeginDoc()
		{
			writer.WriteStartElement("Workbook");
			writer.WriteAttributeString("xmlns", "urn:schemas-microsoft-com:office:spreadsheet");
			writer.WriteAttributeString("xmlns", "o", null, "urn:schemas-microsoft-com:office:office");
			writer.WriteAttributeString("xmlns", "x", null, "urn:schemas-microsoft-com:office:excel");
			writer.WriteAttributeString("xmlns", "ss", null, "urn:schemas-microsoft-com:office:spreadsheet");
			writer.WriteAttributeString("xmlns", "html", null, "http://www.w3.org/TR/REC-html40");

			writer.WriteStartElement("DocumentProperties");
			writer.WriteAttributeString("xmlns", "urn:schemas-microsoft-com:office:office");
			writer.WriteEndElement();

			writer.WriteStartElement("ExcelWorkbook");
			writer.WriteAttributeString("xmlns", "urn:schemas-microsoft-com:office:excel");
			writer.WriteElementString("WindowTopX", "0");
			writer.WriteElementString("WindowTopY", "0");
			writer.WriteElementString("ProtectStructure", "False");
			writer.WriteElementString("ProtectWindows", "False");
			writer.WriteEndElement();

		}

		
		private void RenderStyles()
		{
			writer.WriteStartElement("Styles");
			for (int index = 0; index < Matrix.Styles.Count; index++)
			{
				var style = Matrix.Styles[index] as StiCellStyle;

				writer.WriteStartElement("Style");
				writer.WriteAttributeString("ss", "ID", null, "s" + index.ToString());
				RenderAlignment(style);
				RenderFont(style);
				RenderBorders(style);
				RenderInterior(style);
				RenderNumberFormat(style);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		
		private void RenderNumberFormat(StiCellStyle style)
		{
			writer.WriteStartElement("NumberFormat");
			if ((style.Format != null) && (style.Format != string.Empty))
			{
				writer.WriteAttributeString("ss", "Format", null, style.Format);
			}
			writer.WriteEndElement();
		}
	
		
		private void RenderAlignment(StiCellStyle style)
		{
			writer.WriteStartElement("Alignment");

			if (style.TextOptions != null && style.TextOptions.RightToLeft)
			{
				writer.WriteAttributeString("ss", "ReadingOrder", null, "RightToLeft");			
			}
			
			if (style.HorAlignment == StiTextHorAlignment.Width)
				writer.WriteAttributeString("ss", "Horizontal", null, "Justify");
			else writer.WriteAttributeString("ss", "Horizontal", null, style.HorAlignment.ToString());

			writer.WriteAttributeString("ss", "Vertical", null, style.VertAlignment.ToString());
			
			if (style.TextOptions != null)
			{
				if (style.TextOptions.WordWrap || style.WordWrap)
					writer.WriteAttributeString("ss", "WrapText", null, "1");

				writer.WriteAttributeString("ss", "Rotate", null, style.TextOptions.Angle.ToString());
			}
			writer.WriteEndElement();
			
		}

		
		private void RenderFont(StiCellStyle style)
		{
			writer.WriteStartElement("Font");
			writer.WriteAttributeString("ss", "FontName", null, style.Font.Name);
			writer.WriteAttributeString("x", "CharSet", null, style.Font.GdiCharSet.ToString());
			writer.WriteAttributeString("ss", "Size", null, 
				style.Font.SizeInPoints.ToString(CultureInfo.InvariantCulture));

			if (style.Font.Bold)		writer.WriteAttributeString("ss", "Bold", null, "1");
			if (style.Font.Italic)		writer.WriteAttributeString("ss", "Italic", null, "1");
			if (style.Font.Strikeout)	writer.WriteAttributeString("ss", "StrikeThrough", null, "1");
			if (style.Font.Underline)	writer.WriteAttributeString("ss", "Underline", null, "Single");

			writer.WriteAttributeString("ss", "Color", null, ConvertColor(style.TextColor));
			writer.WriteEndElement();
		}

		
		private void RenderBorder(StiBorderSide border, string position)
		{
			if ((border != null) && (border.Style != StiPenStyle.None))
			{
				writer.WriteStartElement("Border");
				writer.WriteAttributeString("ss", "Position",	null, position);
				writer.WriteAttributeString("ss", "LineStyle",	null, GetLineStyle(border.Style));
				double borderSize = border.Size;
				if (borderSize > 3) borderSize = 3;
				writer.WriteAttributeString("ss", "Weight",		null, borderSize.ToString());
				writer.WriteAttributeString("ss", "Color",		null, ConvertColor(border.Color));
				writer.WriteEndElement();
			}
		}

		
		private void RenderBorders(StiCellStyle style)
		{
//			if (style.Border != null && style.Border.Style != StiPenStyle.None)
			if ((style.Border != null) || (style.BorderL != null) || (style.BorderR != null) || (style.BorderB != null))
			{
				writer.WriteStartElement("Borders");
//				if ((style.Border.Side & StiBorderSides.Left) > 0)	RenderBorder(style.BorderL, "Left");
//				if ((style.Border.Side & StiBorderSides.Right) > 0)	RenderBorder(style.BorderR, "Right");
//				if ((style.Border.Side & StiBorderSides.Top) > 0)	RenderBorder(style.Border, "Top");
//				if ((style.Border.Side & StiBorderSides.Bottom) > 0)RenderBorder(style.BorderB, "Bottom");
				RenderBorder(style.BorderL, "Left");
				RenderBorder(style.BorderR, "Right");
				RenderBorder(style.Border, "Top");
				RenderBorder(style.BorderB, "Bottom");
				writer.WriteEndElement();
			}
		}

		
		private void RenderInterior(StiCellStyle style)
		{
			writer.WriteStartElement("Interior");
			if ((style.Color.A != 0) && (style.Color != Color.White))
			{
				writer.WriteAttributeString("ss", "Color", null, ConvertColor(style.Color));
				writer.WriteAttributeString("ss", "Pattern", null, "Solid");
			}
			writer.WriteEndElement();
		}
  
		
		private void RenderWorksheetOptions(StiPage page)
		{
            #region check for locked components for FreezePanes feature
            int paneX = 0;
            int paneY = 0;
            if (StiOptions.Export.Excel.AllowFreezePanes)
            {
                for (int indexRow = 0; indexRow < Matrix.CoordY.Count - 1; indexRow++)
                {
                    for (int indexColumn = 0; indexColumn < Matrix.CoordX.Count - 1; indexColumn++)
                    {
						var cell = Matrix.Cells[indexRow, indexColumn];
                        if (cell != null && cell.Component != null && (cell.Component.Locked || (cell.Component.TagValue != null && cell.Component.TagValue.ToString().Contains("excelfreezepanes"))))
                        {
                            string stPlacement = cell.Component.ComponentPlacement;
                            if (stPlacement != null && stPlacement.Length > 0 && (stPlacement.StartsWith("rt") || stPlacement.StartsWith("ph") || stPlacement.StartsWith("h")))
                            {
                                //use the bottom left corner of the component
                                paneX = cell.Left;
                                paneY = cell.Top + cell.Height + 1;
                                break;
                            }
                        }
                    }
                }
            }
            #endregion

			writer.WriteStartElement("WorksheetOptions");
			writer.WriteAttributeString("xmlns", "urn:schemas-microsoft-com:office:excel");
			writer.WriteStartElement("PageSetup");
			if (page.Orientation == StiPageOrientation.Landscape)
			{
				writer.WriteStartElement("Layout");
				writer.WriteAttributeString("x", "Orientation", null, "Landscape");
				writer.WriteEndElement();
			}
			writer.WriteStartElement("PageMargins");
			writer.WriteAttributeString("x", "Bottom", null, ConvertPageMargins(page.Unit.ConvertToHInches(page.Margins.Bottom)));
			writer.WriteAttributeString("x", "Left", null, ConvertPageMargins(page.Unit.ConvertToHInches(page.Margins.Left)));
			writer.WriteAttributeString("x", "Right", null, ConvertPageMargins(page.Unit.ConvertToHInches(page.Margins.Right)));
			writer.WriteAttributeString("x", "Top", null, ConvertPageMargins(page.Unit.ConvertToHInches(page.Margins.Top)));
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteStartElement("DoNotDisplayGridlines");
			writer.WriteEndElement();

            if (paneX != 0 || paneY != 0)
            {
                if (paneX == 0)
                {
                    writer.WriteElementString("SplitHorizontal", paneY.ToString());
                    writer.WriteElementString("TopRowBottomPane", paneY.ToString());
                    writer.WriteStartElement("FreezePanes");
                    writer.WriteEndElement();
                    writer.WriteStartElement("FrozenNoSplit");
                    writer.WriteEndElement();
                    writer.WriteElementString("ActivePane", "2");
                }
                else
                {
                    if (paneY == 0)
                    {
                        writer.WriteElementString("SplitVertical", paneX.ToString());
                        writer.WriteElementString("LeftColumnRightPane", paneX.ToString());
                        writer.WriteStartElement("FreezePanes");
                        writer.WriteEndElement();
                        writer.WriteStartElement("FrozenNoSplit");
                        writer.WriteEndElement();
                        writer.WriteElementString("ActivePane", "1");
                    }
                    else
                    {
                        writer.WriteElementString("SplitHorizontal", paneY.ToString());
                        writer.WriteElementString("TopRowBottomPane", paneY.ToString());
                        writer.WriteElementString("SplitVertical", paneX.ToString());
                        writer.WriteElementString("LeftColumnRightPane", paneX.ToString());
                        writer.WriteStartElement("FreezePanes");
                        writer.WriteEndElement();
                        writer.WriteStartElement("FrozenNoSplit");
                        writer.WriteEndElement();
                        writer.WriteElementString("ActivePane", "0");
                    }
                }
            }

			writer.WriteStartElement("ProtectObjects");
			writer.WriteString("False");
			writer.WriteEndElement();
			writer.WriteStartElement("ProtectScenarios");
			writer.WriteString("False");
			writer.WriteEndElement();

			if (page.PaperSize != PaperKind.Custom)
			{
				writer.WriteStartElement("Print");
				writer.WriteElementString("ValidPrinterInfo", "");
				writer.WriteElementString("PaperSizeIndex", string.Format("{0}", (int)page.PaperSize));
				writer.WriteElementString("HorizontalResolution", "600");
				writer.WriteElementString("VerticalResolution", "600");
				writer.WriteEndElement();
			}

			writer.WriteEndElement();
		}
 
		
		private void RenderColumns()
		{
			for (int columnIndex = 1; columnIndex < Matrix.CoordX.Count; columnIndex++)
			{
				writer.WriteStartElement("Column");
				writer.WriteAttributeString("ss", "AutoFitWidth", null, "0");

				var value2 = (double)Matrix.CoordX.GetByIndex(columnIndex);
				var value1 = (double)Matrix.CoordX.GetByIndex(columnIndex - 1);

				writer.WriteAttributeString("ss", "Width", null, ConvertColumn(value2 - value1));
				writer.WriteEndElement();
			}
		}

		
		private void RenderRows()
		{
			RichTextBox richtextForConvert = null;

			var readyCells = new bool[Matrix.CoordY.Count, Matrix.CoordX.Count];

			var progressScale = Math.Max(Matrix.CoordY.Count / 200f, 1f);
            int progressValue = 0;

            for (int rowIndex = 1; rowIndex < Matrix.CoordY.Count; rowIndex++)
			{
                int currentProgress = (int)(rowIndex / progressScale);
                if (currentProgress > progressValue)
                {
                    progressValue = currentProgress;
                    InvokeExporting(rowIndex, matrix.CoordY.Count, CurrentPassNumber, MaximumPassNumber);
                }
                if (IsStopped) return;

                writer.WriteStartElement("Row");
				writer.WriteAttributeString("ss", "AutoFitHeight", null, "0");
				var height = (double)Matrix.CoordY.GetByIndex(rowIndex) - (double)Matrix.CoordY.GetByIndex(rowIndex - 1);
				writer.WriteAttributeString("ss", "Height", null, Convert(height));
				
				for (int columnIndex = 1; columnIndex < Matrix.CoordX.Count; columnIndex++)
				{
					var cell = Matrix.Cells[rowIndex - 1, columnIndex - 1];
					
					if ((cell != null) && (!readyCells[rowIndex, columnIndex]))
					{
						readyCells[rowIndex, columnIndex] = true;
						var rtf = cell.Component as StiRichText;
						string str = cell.Text;
						if ((rtf != null) && (rtf.RtfText != string.Empty))
						{
							if (richtextForConvert == null)richtextForConvert = new Controls.StiRichTextBox(false);
							rtf.GetPreparedText(richtextForConvert);
							str = richtextForConvert.Text;
						}
                        var checkComp = cell.Component as StiCheckBox;
                        if ((checkComp != null) && (!string.IsNullOrEmpty(checkComp.ExcelDataValue)))
                        {
                            str = checkComp.ExcelDataValue;
                        }
						writer.WriteStartElement("Cell");
						writer.WriteAttributeString("ss", "Index", null, columnIndex.ToString());

						#region Style
						//StiCellStyle cellStyle = cell.CellStyle;
						var cellStyle = matrix.CellStyles[rowIndex - 1, columnIndex - 1];
						int indexStyle = Matrix.Styles.IndexOf(cellStyle);
						writer.WriteAttributeString("ss", "StyleID", null, "s" + indexStyle.ToString());
						#endregion

						#region Range
						if (cell.Width > 0)
							writer.WriteAttributeString("ss", "MergeAcross", null, cell.Width.ToString());

						if (cell.Height > 0)
							writer.WriteAttributeString("ss", "MergeDown", null, cell.Height.ToString());

						for (int xx = 0; xx <= cell.Width; xx++)
						{
							for (int yy = 0; yy <= cell.Height; yy++)
							{
								readyCells[rowIndex + yy, columnIndex + xx] = true;
							}
						}
						#endregion

						#region Text
						var textComp = cell.Component as StiText;
						if (!string.IsNullOrEmpty(str) || textComp != null &&
                            textComp.ExcelDataValue != null &&
                            textComp.ExcelDataValue != "-")
						{
							writer.WriteStartElement("Data");

							string textType = "String";
							if (textComp != null && textComp.ExcelDataValue != null)
							{								
								string value = textComp.ExcelDataValue;
								string sep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
								value = value.Replace(".", ",").Replace(",", sep);

								#region Format
								bool isFormatCurrency = false;
								bool isFormatPercent = false;
								bool isFormatDate = false;
								bool isFormatTime = false;
								string inputFormat = string.Empty;
								if (textComp != null) inputFormat = textComp.Format; 
								if ((inputFormat != null) && (inputFormat.Length > 0))
								{
									if (inputFormat[0] == 'C')
									{
										isFormatCurrency = true;
									}
									if (inputFormat[0] == 'P')
									{
										isFormatPercent = true;
									}
									if (inputFormat[0] == 'D')
									{
										isFormatDate = true;
									}
									if (inputFormat[0] == 'T')
									{
										isFormatTime = true;
									}
								}
								#endregion

								bool error = false;
								if (isFormatCurrency || isFormatPercent || isFormatDate || isFormatTime)
								{
									error = true;
								}
								else
								{
									double result;
									if (!double.TryParse(value, NumberStyles.Any, NumberFormatInfo.CurrentInfo, out result))
									{
                                        error = true;
									}
								}

								if (!error)
								{
									textType = "Number";
									str = value;
								}
							}
							
							writer.WriteAttributeString("ss", "Type", null, textType);

							var strs = str.Replace("\n", "").Replace('\t', ' ').Split(new char[1] { '\r' });
							for (int index = 0; index < strs.Length; index++)
							{
								writer.WriteString(strs[index].TrimEnd(' '));
								if (index < (strs.Length - 1))writer.WriteRaw("&#10;");
							}
							writer.WriteEndElement();
						}
						#endregion

						writer.WriteEndElement();
					}
					if ((cell == null) && (!readyCells[rowIndex, columnIndex]))
					{
						var cellStyle = matrix.CellStyles[rowIndex - 1, columnIndex - 1];
						if (cellStyle != null)
						{
							#region Style
							int indexStyle = Matrix.Styles.IndexOf(cellStyle);
							writer.WriteStartElement("Cell");
							if (columnIndex > 1)
							{
								writer.WriteAttributeString("ss", "Index", null, columnIndex.ToString());
							}
							writer.WriteAttributeString("ss", "StyleID", null, "s" + indexStyle.ToString());
							writer.WriteEndElement();
							#endregion
						}
					}
				}
				writer.WriteEndElement();
			}
			if (richtextForConvert != null) richtextForConvert.Dispose();
		}
				
		
		private void RenderCells(StiPage page)
		{	
			writer.WriteStartElement("Worksheet");
			writer.WriteAttributeString("ss", "Name", null, title);
			
			writer.WriteStartElement("Table");
			
			writer.WriteAttributeString("ss", "ExpandedColumnCount", null, (Matrix.CoordX.Count - 1).ToString());
			writer.WriteAttributeString("ss", "ExpandedRowCount", null, (Matrix.CoordY.Count - 1).ToString());
			writer.WriteAttributeString("x", "FullColumns", null, "1");
			writer.WriteAttributeString("x", "FullRows", null, "1");
			
			RenderColumns();
			RenderRows();
			
			writer.WriteEndElement();
			RenderWorksheetOptions(page);
			writer.WriteEndElement();
		}
        
		/// <summary>
        /// Exports a rendered report to the ExcelXml file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        public void ExportExcel(StiReport report, string fileName)
		{
			StiFileUtils.ProcessReadOnly(fileName);
			using (var stream = new FileStream(fileName, FileMode.Create))
			{
				ExportExcel(report, stream);
				stream.Flush();
				stream.Close();
			}
		}

        
		/// <summary>
        /// Exports a rendered report to the ExcelXml file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        public void ExportExcel(StiReport report, Stream stream)
		{
			ExportExcel(report, stream, new StiExcelXmlExportSettings());
		}

        
		/// <summary>
        /// Exports a rendered report to the ExcelXml file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
		public void ExportExcel(StiReport report, Stream stream, StiPagesRange pageRange)
		{
            var settings = new StiExcelXmlExportSettings
            {
                PageRange = pageRange
            };

            ExportExcel(report, stream, settings);
		}

		
		/// <summary>
		/// Exports a rendered report to the ExcelXml file.
		/// </summary>
        public void ExportExcel(StiReport report, Stream stream, StiExcelExportSettings settings)
		{
			StiLogService.Write(this.GetType(), "Export report to Excel Xml format");

			#region Read settings
			if (settings == null)
				throw new ArgumentNullException("The 'settings' argument cannot be equal in null.");

			var pageRange = settings.PageRange;
			#endregion

			var currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

				this.title = (report.ReportAlias == null || report.ReportAlias.Trim().Length == 0) 
					? report.ReportName : report.ReportAlias;
				var pages = pageRange.GetSelectedPages(report.RenderedPages);

                CurrentPassNumber = 0;
                MaximumPassNumber = 3;

                matrix = new StiMatrix(pages, StiOptions.Export.Excel.DivideBigCells, this);
				if (IsStopped) return;

				StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");
                CurrentPassNumber = 2;

                writer = new XmlTextWriter(stream, Encoding.UTF8)
                {
                    Indentation = 2,
                    Formatting = Formatting.Indented
                };
                writer.WriteStartDocument();

				RenderBeginDoc();
				RenderStyles();
				pages.GetPage(pages[0]);
				RenderCells(pages[0]);
				if (IsStopped) return;
			
				writer.WriteFullEndElement();
				writer.WriteEndDocument();
				writer.Flush();
			}
			finally
			{
                Thread.CurrentThread.CurrentCulture = currentCulture;
				if (matrix != null)
				{
					matrix.Clear();
					matrix = null;
				}
			}
		}
		#endregion
	}
}