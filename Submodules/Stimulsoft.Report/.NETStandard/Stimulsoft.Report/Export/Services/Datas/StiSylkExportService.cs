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
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Stimulsoft.Report.Components;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Services;
using System.Threading;
using Stimulsoft.Report.Dashboard;
using System.Drawing;
using Stimulsoft.Report.Helpers;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// A class for the SYLK export.
    /// </summary>
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourceData.png")]
	public class StiSylkExportService : StiExportService
    {
		#region Fields
		private StiReport report;
		private string fileName;
		private bool sendEMail;
		private StiGuiMode guiMode;
        #endregion

        #region StiExportService override
        /// <summary>
        /// Gets or sets a default extension of export. 
        /// </summary>
        public override string DefaultExtension => "slk";

		public override StiExportFormat ExportFormat => StiExportFormat.Sylk;

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
		public override string ExportNameInMenu => StiLocalization.Get("Export", "ExportTypeSylkFile");

        /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public override void ExportTo(StiReport report, Stream stream, StiExportSettings settings)
        {
            ExportSylk(report, stream, settings as StiDataExportSettings);
			InvokeExporting(100, 100, 1, 1);
		}

		/// <summary>
		/// Exports a rendered report to a Sylk file.
		/// </summary>
		/// <param name="report">A report which is to be exported.</param>
		/// <param name="fileName">A name of the file for exporting a rendered report.</param>
		/// <param name="sendEMail">A parameter indicating whether the exported report will be sent via e-mail.</param>
        public override void Export(StiReport report, string fileName, bool sendEMail, StiGuiMode guiMode)
		{
            using (var form = StiGuiOptions.GetExportFormRunner("StiSylkExportSetupForm", guiMode, this.OwnerWindow))
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
		public override string GetFilter() => StiLocalization.Get("FileFilters", "SylkFiles");
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

		#region struct DataFont
		private struct DataFont
		{
			public string Name;
			public bool Bold;
			public bool Italic;
			public bool Underlined;
			public int Height;
			public int Color;

			public DataFont(
				string Name,
				bool Bold,
				bool Italic,
				bool Underlined,
				int Height,
				int Color)
			{
				this.Name		= Name;
				this.Bold		= Bold;
				this.Italic		= Italic;
				this.Underlined	= Underlined;
				this.Height		= Height;
				this.Color		= Color;
			}
		} 
		#endregion

		#region GetFontNumber
		private int GetFontNumber(DataFont dataIn)
		{
			if (fontList.Count > 4)
			{
				for (int index = 4; index < fontList.Count; index++)
				{
					if (Equals(fontList[index], dataIn))
					{
						//is already in table, return number 
						return index;
					}
				}
			}
			//add to table, return number 
			fontList.Add(dataIn);
			int temp = fontList.Count - 1;
			return temp;
		}
		#endregion

		#region GetFormatNumber
		private int GetFormatNumber(string dataIn)
		{
			if (formatList.Count > 0)
			{
				for (int index = 0; index < formatList.Count; index++)
				{
					if (Equals(formatList[index], dataIn))
					{
						return index;
					}
				}
			}
			formatList.Add(dataIn);
			int temp = formatList.Count - 1;
			return temp;
		}
		#endregion

		#region Color table
		private int colorTableSize = 64;
		private int[] colorTable = 
		{
			0x000000,	//0
			0xFEFEFE,
			0xFF0000,
			0x00FF00,
			0x0000FF,
			0xFFFF00,
			0xFF00FF,
			0x00FFFF,
			0x000000,	//8
			0xFEFEFE,
			0xFF0000,
			0x00FF00,
			0x0000FF,
			0xFFFF00,
			0xFF00FF,
			0x00FFFF,
			0x800000,	//16
			0x008000,
			0x000080,
			0x808000,
			0x800080,
			0x008080,
			0xC0C0C0,
			0x808080,
			0x9999FF,	//24
			0x993366,
			0xFFFFCC,
			0xCCFFFF,
			0x660066,
			0xFF8080,
			0x0066CC,
			0xCCCCFF,
			0x000080,	//32
			0xFF00FF,
			0xFFFF00,
			0x00FFFF,
			0x800080,
			0x800000,
			0x008080,
			0x0000FF,
			0x00CCFF,	//40
			0xCCFFFF,
			0xCCFFCC,
			0xFFFF99,
			0x99CCFF,
			0xFF99CC,
			0xCC99FF,
			0xFFCC99,
			0x3366FF,	//48
			0x33CCCC,
			0x99CC00,
			0xFFCC00,
			0xFF9900,
			0xFF6600,
			0x666699,
			0x969696,
			0x003366,	//56
			0x339966,
			0x003300,
			0x333300,
			0x993300,
			0x993366,
			0x333399,
			0x333333
		};
		#endregion

		#region GetColorIndex
		private int GetColorIndex(Color color)
		{
			if (color == Color.Transparent) return 0;
			int colorValue = color.ToArgb();
			if (colorHash.ContainsKey(colorValue))
			{
				return (int)colorHash[colorValue];
			}
			int offset = 768;
			int colorIndex = 0;
			for (int index = 0; index < colorTableSize; index++)
			{
				int tempColor = colorTable[index];
				int tempOffset = Math.Abs(color.R - ((tempColor >> 16) & 0xFF)) + Math.Abs(color.G - ((tempColor >> 8) & 0xFF)) + Math.Abs(color.B - (tempColor & 0xFF));
				if (tempOffset < offset)
				{
					colorIndex = index;
					offset = tempOffset;
				}
			}
			colorIndex++;
			colorHash[colorValue] = colorIndex;
			return colorIndex;
		}
		#endregion

		#region this

        private StreamWriter writer;
		private List<string> formatList = null;
        private List<DataFont> fontList = null;
		private Hashtable colorHash = null;

		// 1 point = 1/72 inch
		// 20 twips = 1 point 
		// 75 point = 100 pixels
		// 75 pixels = 10 width of column
		// 100 pixels = 3657 colinfo width
		// 705 pixels = 100 symbols
		//
		// 1 twips = (1/20 point) * (100/75 pixels) * (3657/100 width) = 100*3657 / 20*75*100 = 
		// = 3657 / 1500 = 2.438 colinfo width
		// 1 twips = (1/20 point) * (100/75 pixels) * (100/705 symbols) = 100*100 / 20*75*705 = 
		// = 10000 / 1057500 = 0.0094562647754137 symbols
		
		private static double HiToTwips = 14.4 * 1.02;

		//conversion from twips to units of column
		const double TwipsToColinfo = 2.438 * 0.921;

		//conversion from twips to symbols of column
		const double TwipsToSymbols = 0.0094563 * 0.92;

		/// <summary>
		/// Exports a rendered report to a Sylk file.
		/// </summary>
		/// <param name="report">A report which is to be exported.</param>
		/// <param name="fileName">A name of the file for exporting a rendered report.</param>
		public void ExportSylk(StiReport report, string fileName)
		{
			StiFileUtils.ProcessReadOnly(fileName);
			using (var stream = new FileStream(fileName, FileMode.Create))
			{
				ExportSylk(report, stream);
				stream.Flush();
				stream.Close();
			}
		}

       
		/// <summary>
		/// Exports a rendered report to a Sylk file.
		/// </summary>
		/// <param name="report">A report which is to be exported.</param>
		/// <param name="stream">A stream for export of a document.</param>
		public void ExportSylk(StiReport report, Stream stream)
		{
			ExportSylk(report, stream, new StiDataExportSettings());
		}
        
    
		/// <summary>
		/// Exports a rendered report to a Sylk file.
		/// </summary>
		/// <param name="report">A report which is to be exported.</param>
		/// <param name="stream">A stream for export of a document.</param>
		/// <param name="pageRange">Describes range of  pages of the document for the export.</param>
		/// <param name="encoding">Encoding for the result file.</param>
		public void ExportSylk(StiReport report, Stream stream, StiPagesRange pageRange, bool exportDataOnly, bool useDefaultSystemEncoding,
			Encoding encoding)
		{
            var settings = new StiDataExportSettings
            {
                PageRange = pageRange,
                ExportDataOnly = exportDataOnly,
                UseDefaultSystemEncoding = useDefaultSystemEncoding,
                Encoding = encoding
            };

            ExportSylk(report, stream, settings);
		}

		/// <summary>
		/// Exports a rendered report to a Sylk file.
		/// </summary>
		/// <param name="report">A report which is to be exported.</param>
		/// <param name="stream">A stream for export of a document.</param>
        public void ExportSylk(StiReport report, Stream stream, StiDataExportSettings settings)
		{
			StiLogService.Write(this.GetType(), "Export report to Sylk format");

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

			#region Init
            fontList = new List<DataFont>();
			var standardFont = new DataFont("Arial", false, false, false, 200, 0);
			GetFontNumber(standardFont);
			GetFontNumber(standardFont);
			GetFontNumber(standardFont);
			GetFontNumber(standardFont);
			colorHash = new Hashtable();
			formatList = new List<string>();
			GetFormatNumber("General");

			if (useDefaultSystemEncoding)
			{
				encoding = Encoding.GetEncoding(CultureInfo.InstalledUICulture.TextInfo.ANSICodePage);
			}
			var memSt = new MemoryStream();
			writer = new StreamWriter(memSt, encoding);
			#endregion

			StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");

			var pages = pageRange.GetSelectedPages(report.RenderedPages);

            CurrentPassNumber = 0;
            MaximumPassNumber = 3;

			var dataMode = exportDataOnly ? StiDataExportMode.Data | StiDataExportMode.Headers : StiDataExportMode.AllBands;

			var matrix = new StiMatrix(pages, true, this, null, dataMode);
			//matrix.ScanComponentsPlacement(false);
			if (IsStopped) return;

			if (exportDataOnly)
			{
				matrix.ScanComponentsPlacement(true, false);

				#region remake matrix
				int linesCount = 0;
				var headerNames = new Hashtable();
				string lastParentBandName = null;
				for (int rowIndex = 0; rowIndex < matrix.CoordY.Count - 1; rowIndex++)
				{
					bool isHeader = false;
					if ((matrix.LinePlacement[rowIndex] == StiMatrix.StiTableLineInfo.HeaderD) || (matrix.LinePlacement[rowIndex] == StiMatrix.StiTableLineInfo.HeaderAP))
					{
						string tempSt = matrix.ParentBandName[rowIndex];
						//check for new header component 
                        int symPos = tempSt.IndexOf('\x1f');
						if (symPos != -1)
						{
							var parentBandName = tempSt.Substring(0, symPos);
							if (parentBandName != lastParentBandName)
							{
								lastParentBandName = parentBandName;
								headerNames.Clear();
							}
						}
						//check for repeated lines
						if (!headerNames.ContainsKey(tempSt))
						{
							isHeader = true;
							headerNames.Add(tempSt, tempSt);
						}
					}
					if ((matrix.LinePlacement[rowIndex] == StiMatrix.StiTableLineInfo.Data ) ||
						//	(Matrix.LinePlacement[rowIndex] == StiMatrix.StiTableLineInfo.FooterD) ||
						//	(Matrix.LinePlacement[rowIndex] == StiMatrix.StiTableLineInfo.FooterAP) ||
						isHeader)
					{
						//move line
						for (int columnIndex = 0; columnIndex < matrix.CoordX.Count - 1; columnIndex++)
						{
							//move cell
							matrix.Cells[linesCount, columnIndex] = matrix.Cells[rowIndex, columnIndex];
							if (matrix.Cells[linesCount, columnIndex] != null)
							{
								//correct coordinate
								matrix.Cells[linesCount, columnIndex].Top = linesCount;
							}
							//move border
							if ((linesCount == 0) || ((matrix.BordersX[rowIndex, columnIndex] != null)))
							{
								matrix.BordersX[linesCount, columnIndex] = matrix.BordersX[rowIndex, columnIndex];
							}
							matrix.BordersX[linesCount + 1, columnIndex] = matrix.BordersX[rowIndex + 1, columnIndex];
							matrix.BordersY[linesCount, columnIndex] = matrix.BordersY[rowIndex, columnIndex];
							//move bookmarks
							matrix.Bookmarks[linesCount, columnIndex] = matrix.Bookmarks[rowIndex, columnIndex];
						}
						//move border - right line
						matrix.BordersY[linesCount, matrix.CoordX.Count - 1] = matrix.BordersY[rowIndex, matrix.CoordX.Count - 1];

						//count line height
						double lineHeight = (double)matrix.CoordY.GetByIndex(rowIndex + 1) - (double)matrix.CoordY.GetByIndex(rowIndex);
						matrix.CoordY.SetByIndex(linesCount + 1, (double)matrix.CoordY.GetByIndex(linesCount) + lineHeight);
						linesCount ++;
					}
				}

				int numAbove = matrix.CoordY.Count - 1 - linesCount;
				if (numAbove > 0)
				{
					//remove lines at end of array
					for (int tempIndex = 0; tempIndex < numAbove; tempIndex ++)
					{
						matrix.CoordY.RemoveAt(linesCount + 1);
					}
				}
				#endregion
			}

            CurrentPassNumber = 2;

			#region Prepare rows stream
			RichTextBox richtextForConvert = null;
			for (int rowIndex = 0; rowIndex < matrix.CoordY.Count - 1; rowIndex++)
			{
				InvokeExporting(rowIndex, matrix.CoordY.Count - 1, CurrentPassNumber, MaximumPassNumber);
				if (IsStopped) return;
				int brushCount = 0;
				for (int columnIndex = 0; columnIndex < matrix.CoordX.Count - 1; columnIndex ++)
				{
					#region Export cell
					string text = null;
					string style = null;
					string format = null;
					string picture = null;
					Font font = null;
					int colorIndex = 0;
					bool isNumber = false;
					bool needBrush = false;
					if (brushCount > 0) 
					{
						needBrush = true;
						brushCount--;
					}

					var cell = matrix.Cells[rowIndex, columnIndex];
					if (cell != null)
					{
						#region Get text
						var textComp = cell.Component as StiText;
						var rtf = cell.Component as StiRichText;

						if (!string.IsNullOrEmpty(cell.Text))
						{
							text = cell.Text;
						}
					    if ((rtf != null) && (rtf.RtfText != string.Empty))
						{
							if (richtextForConvert == null) richtextForConvert = new Controls.StiRichTextBox(false);
							rtf.GetPreparedText(richtextForConvert);
							text = richtextForConvert.Text;
						}

						if (text == null) text = string.Empty;
						var sbTemp = new StringBuilder();
						for (int indexChar = 0; indexChar < text.Length; indexChar++)
						{
							int sym = (int)text[indexChar];
							if (sym < 32)
							{
								string st = string.Format("\x1B{0}{1}",
									(char)(0x20 + ((sym >> 4) & 0x0F)),
									(char)(0x30 + (sym & 0x0F)));
								sbTemp.Append(st);
							}
							else
							{
								sbTemp.Append((char)sym);
							}
						}
						text = StiExportUtils.TrimEndWhiteSpace(sbTemp.ToString());
						#endregion

						font = cell.CellStyle.Font;
						colorIndex = GetColorIndex(cell.CellStyle.TextColor);

						if (cell.CellStyle.Color != Color.Transparent)
						{
							if (cell.CellStyle.Color.R + cell.CellStyle.Color.G + cell.CellStyle.Color.B < 0x220)
							{
								needBrush = true;
								brushCount = cell.Width;
							}
						}

						#region Scan value format
						string inputFormat = string.Empty;
						if (textComp != null) inputFormat = textComp.Format; 

						bool isFormatCurrency = false;
						bool isFormatNumeric = false;
						bool isFormatPercent = false;
						bool isFormatDate = false;
						bool isFormatTime = false;
						//						bool isDefaultFormat = true;
						string outputFormat = string.Empty;
						int decimalDigits = 2;
						int groupDigits = 0;
						string currencySymbol = "$";
						bool currencyPositionBefore = false;
                        bool negativeBraces = false;
                        bool hideZeros = (textComp != null) && textComp.HideZeros;

                        string positivePatternString = null;
                        string negativePatternString = null;
                        int posPatternDelimiter = (inputFormat != null ? inputFormat.IndexOf("|") : -1);
                        if (posPatternDelimiter != -1)
                        {
                            positivePatternString = StiExportUtils.GetPositivePattern((int)inputFormat[posPatternDelimiter + 1] - (int)'A');
                            negativePatternString = StiExportUtils.GetNegativePattern((int)inputFormat[posPatternDelimiter + 2] - (int)'A');
                            inputFormat = inputFormat.Substring(0, posPatternDelimiter);
                        }

                        #region get value format
                        if (!string.IsNullOrEmpty(inputFormat))
						{
							if (inputFormat[0] == 'C')
							{
								isFormatCurrency = true;
							}
							if (inputFormat[0] == 'N')
							{
								isFormatNumeric = true;
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
							if (inputFormat.Length > 1)
							{
								if (isFormatCurrency || isFormatNumeric || isFormatPercent)
								{
									#region scan parameters
									int indexPos = 1;
									if (char.IsDigit(inputFormat[indexPos]))
									{
										var decimalSB = new StringBuilder();
										while ((indexPos < inputFormat.Length) && (char.IsDigit(inputFormat[indexPos])))
										{
											decimalSB.Append(inputFormat[indexPos]);
											indexPos ++;
										}
										decimalDigits = int.Parse(decimalSB.ToString());
									}
									if ((indexPos < inputFormat.Length) && (inputFormat[indexPos] == 'G'))
									{
										indexPos ++;
										groupDigits = 3;
									}
                                    if ((indexPos < inputFormat.Length) && (inputFormat[indexPos] == '('))
                                    {
                                        indexPos++;
                                        negativeBraces = true;
                                    }
                                    if ((indexPos < inputFormat.Length) && 
										((inputFormat[indexPos] == '+') || (inputFormat[indexPos] == '-')))
									{
										if (inputFormat[indexPos] == '+') currencyPositionBefore = true;
										indexPos ++;
										if (indexPos < inputFormat.Length)
										{
											currencySymbol = inputFormat.Substring(indexPos);
										}
									}
									#endregion
								}
							}
						}
						#endregion

						if (isFormatCurrency || isFormatNumeric || isFormatPercent)
						{
                            if (posPatternDelimiter != -1)
                            {
								var outputSB = new StringBuilder();
                                if (groupDigits > 1)
                                {
                                    outputSB.Append("#,");
                                    outputSB.Append('#', groupDigits - 1);
                                }
                                outputSB.Append('0');
                                if (decimalDigits > 0)
                                {
                                    outputSB.Append(".");
                                    outputSB.Append('0', decimalDigits);
                                }
                                string nn = outputSB.ToString();

                                string positivePattern = positivePatternString.Replace("n", nn).Replace("$", $"\"{currencySymbol}\"");
                                string negativePattern = negativePatternString.Replace("n", nn).Replace("$", $"\"{currencySymbol}\"");

                                outputFormat = positivePattern + ";" + negativePattern + (hideZeros ? ";" : "");
                            }
                            else
                            {
								#region make format string
								var outputSB = new StringBuilder();
                                if ((isFormatCurrency) && (currencyPositionBefore == true))
                                {
                                    outputSB.Append("\"");
                                    outputSB.Append(currencySymbol);
                                    outputSB.Append("\"");
                                }
                                if (groupDigits > 1)
                                {
                                    outputSB.Append("#,");
                                    outputSB.Append('#', groupDigits - 1);
                                }
                                outputSB.Append('0');
                                if (decimalDigits > 0)
                                {
                                    outputSB.Append(".");
                                    outputSB.Append('0', decimalDigits);
                                }
                                if ((isFormatCurrency) && (currencyPositionBefore == false))
                                {
                                    outputSB.Append("\"");
                                    outputSB.Append(currencySymbol);
                                    outputSB.Append("\"");
                                }
                                if (isFormatPercent)
                                {
                                    outputSB.Append("%");
                                }
                                outputFormat = outputSB.ToString();
                                string negativePattern = (negativeBraces ? "(" : "-") + outputFormat + (negativeBraces ? ")" : "");
                                if ((textComp != null) && textComp.HideZeros)   //add 25.11.2008
                                {
                                    outputFormat = outputFormat + ";" + negativePattern + ";";
                                }
                                else
                                {
                                    if (negativeBraces) outputFormat = outputFormat + ";" + negativePattern;
                                }
                                #endregion
                            }
						}
						if (isFormatDate)
						{
							outputFormat = "dd/mm/yyyy";
						}
						if (isFormatTime)
						{
							outputFormat = "[$-F400]h:mm:ss\\ AM/PM";
						}

						if (outputFormat != string.Empty)
						{
							picture = string.Format("P{0};", GetFormatNumber(outputFormat));
						}

						string formatInfo = "G0";
						if (isFormatNumeric)
						{
							formatInfo = string.Format("F{0}", decimalDigits);
						}
						if (isFormatCurrency)
						{
							formatInfo = string.Format("${0}", decimalDigits);
						}
						if (isFormatPercent)
						{
							formatInfo = string.Format("%{0}", decimalDigits);
						}
						#endregion

						string justify = "L";
						if (cell.CellStyle.HorAlignment == StiTextHorAlignment.Right) justify = "R";
						if (cell.CellStyle.HorAlignment == StiTextHorAlignment.Center) justify = "C";
						if (cell.CellStyle.HorAlignment == StiTextHorAlignment.Width) justify = "X";
						format = string.Format("F{0}{1};", formatInfo, justify);

						#region Text
						if ((textComp != null) && (textComp.ExcelDataValue != null && textComp.ExcelDataValue != "-"))
						{
							double Number = 0;
							string value = textComp.ExcelDataValue;

							string sep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
							string value2 = value.Replace(".", ",").Replace(",", sep);

							isNumber = true;
							try
							{
								if (isFormatDate || isFormatTime)
								{
									//DateTime dt = DateTime.Parse(value2, CultureInfo.InstalledUICulture);
                                    DateTime dt;
                                    if (DateTime.TryParse(value2, CultureInfo.InstalledUICulture, DateTimeStyles.None, out dt))
                                    {
                                        if (isFormatDate)
                                        {
                                            Number = dt.Subtract(new DateTime(1900, 1, 1)).Days + 1 + 1;
                                        }
                                        else
                                        {
                                            Number = (double)dt.TimeOfDay.TotalSeconds / 86400d;
                                        }
                                    }
                                    else
                                    {
                                        isNumber = false;
                                    }
                                }
								else
								{
                                    //Number = double.Parse(value2, NumberStyles.Any);
                                    if (!double.TryParse(value2, NumberStyles.Any, CultureInfo.CurrentCulture, out Number))
                                    {
                                        isNumber = false;
                                    }
                                }
							}
							catch (FormatException)
							{
								isNumber = false;
							}

							if (isNumber)
							{
								text = Number.ToString().Replace(",", ".");
							}
						}
						#endregion

					}

					#region Style
					bool needBorderLeft = (matrix.BordersY[rowIndex, columnIndex] != null);
					bool needBorderRight = (matrix.BordersY[rowIndex, columnIndex + 1] != null);
					bool needBorderTop = (matrix.BordersX[rowIndex, columnIndex] != null);
					bool needBorderBottom = (matrix.BordersX[rowIndex + 1, columnIndex] != null);
					if (needBorderLeft || needBorderRight || needBorderTop || needBorderBottom || (font != null) || needBrush)
					{
						int fontIndex = 0;
						if (font != null)
						{
							fontIndex = GetFontNumber(new DataFont(font.Name, font.Bold, font.Italic, font.Underline, (int)(font.SizeInPoints * 20), colorIndex));
						}

						style = string.Format("S{0}{1}{2}{3}{4}{5}{6}{7};",
							((font != null) && font.Bold ? "D" : ""),
							((font != null) && font.Italic ? "I" : ""),
							(needBorderLeft ? "L" : ""),
							(needBorderRight ? "R" : ""),
							(needBorderTop ? "T" : ""),
							(needBorderBottom ? "B" : ""),
							(needBrush ? "S" : ""),
							(font != null ? string.Format("M{0}", fontIndex + 1) : ""));
					}
					#endregion

					if ((format != null) || (style != null) || (text != null) || (picture != null))
					{
						writer.WriteLine("F;{0}{1}{2}Y{3};X{4}",
							picture,
							format,
							style,
							rowIndex + 1,
							columnIndex + 1);
						if (!string.IsNullOrEmpty(text))
						{
							if (isNumber)
							{
								writer.WriteLine("C;K{0}", text);
							}
							else
							{
								writer.WriteLine("C;K\"{0}\"", text);
							}
						}
					}
					#endregion
				}
			}
			#endregion

			writer.Flush();
			writer = new StreamWriter(stream, encoding);

			#region Write header
			writer.WriteLine("ID;PWXL;N;E");

			//write formats
			for (int index = 0; index < formatList.Count; index++)
			{
				writer.WriteLine("P;P{0}", formatList[index]);
			}

			//write fonts
			for (int indexFont = 0; indexFont < fontList.Count; indexFont++)
			{
				var tempFont = fontList[indexFont];
				var sbFont = new StringBuilder("P;");
				sbFont.Append(string.Format("{0}{1};M{2}",					
					indexFont < 4 ? "F" : "E",
					tempFont.Name,
					tempFont.Height));
				if (tempFont.Bold || tempFont.Italic || tempFont.Underlined)
				{
					sbFont.Append(";S");
					if (tempFont.Bold) sbFont.Append("B");
					if (tempFont.Italic) sbFont.Append("I");
					if (tempFont.Underlined) sbFont.Append("U");
				}
				if (tempFont.Color != 0)
				{
					sbFont.Append(string.Format(";L{0}", tempFont.Color));
				}
				writer.WriteLine(sbFont.ToString());
			}

			//default format
			writer.WriteLine("F;P0;DG0G8;M255");

			//Dimensions
			writer.WriteLine("B;Y{0};X{1};D0 0 {2} {3}",
				matrix.CoordY.Count - 1,
				matrix.CoordX.Count - 1,
				matrix.CoordY.Count - 2,
				matrix.CoordX.Count - 2);

			//Columns widths
			for (int columnIndex = 1; columnIndex < matrix.CoordX.Count; columnIndex++)
			{
				double value2 = (double)matrix.CoordX.GetByIndex(columnIndex);
				double value1 = (double)matrix.CoordX.GetByIndex(columnIndex - 1);
				int width = (int)Math.Round((value2 - value1) * HiToTwips * TwipsToSymbols);
				if (width == 0) width++;
				writer.WriteLine("F;W{0} {0} {1}", columnIndex, width);
			}

			//Row heights
			for (int rowIndex = 1; rowIndex < matrix.CoordY.Count; rowIndex++)
			{
				int height = (int)Math.Round(((double)matrix.CoordY.GetByIndex(rowIndex) 
					- (double)matrix.CoordY.GetByIndex(rowIndex - 1)) * HiToTwips );
				if (height == 0) height++;
				writer.WriteLine("F;M{0};R{1}", height, rowIndex);
			}
			#endregion

			writer.Flush();
			memSt.WriteTo(stream);
			writer.WriteLine("E");
			writer.Flush();

			#region Clear
			if (matrix != null)
			{
				matrix.Clear();
				matrix = null;
			}
			formatList = null;
			fontList = null;
			colorHash = null;
			#endregion
		}
		#endregion
	}
}