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
using System.Collections;
using System.IO;
using Stimulsoft.Report.Components;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Zip;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using Stimulsoft.Report.Helpers;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
#endif

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// A class for the Ods Export.
    /// </summary>
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourceOds.png")]
    public class StiOdsExportService : StiExportService
    {
        #region class CellStyleData
        private class CellStyleData
        {
            public string BorderLeft;
            public string BorderRight;
            public string BorderTop;
            public string BorderBottom;
            public Color BackColor;
            public StiVertAlignment VertAlign;
            public string FontName;
            public float FontSize;
            public bool Bold;
            public bool Italic;
            public bool Underline;
            public Color FontColor;
            public StiTextHorAlignment HorAlign;
            public int Angle;
            public bool Wordwrap;
            public int DataStyle;

            public CellStyleData()
            {
                BorderLeft = "none";
                BorderRight = "none";
                BorderTop = "none";
                BorderBottom = "none";
                BackColor = Color.Transparent;
                VertAlign = StiVertAlignment.Bottom;
                FontName = "Arial";
                FontSize = 6;
                Bold = false;
                Italic = false;
                Underline = false;
                FontColor = Color.Black;
                HorAlign = StiTextHorAlignment.Left;
                Angle = 0;
                Wordwrap = false;
                DataStyle = -1;
            }
        }
        #endregion

        #region class DataStyleData
        private class DataStyleData
        {
            public bool isNumeric;
            public bool isCurrency;
            public bool isPercent;
            public bool isDate;
            public bool isTime;
            public bool isDefaultFormat;
            public int DecimalDigits;
            public char DecimalComma;
            public int GroupDigits;
            public string CurrencySymbol;
            public bool CurrencyPositionBefore;
            public bool NegativeBraces;
            public string DateTimeFormatString;
            public string CurrencyPositivePattern;
            public string CurrencyNegativePattern;

            public DataStyleData()
            {
                isNumeric = false;
                isCurrency = false;
                isPercent = false;
                isDate = false;
                isTime = false;
                isDefaultFormat = false;
                DecimalDigits = 2;
                DecimalComma = ' ';
                GroupDigits = 0;
                CurrencySymbol = CultureInfo.InstalledUICulture.NumberFormat.CurrencySymbol;
                CurrencyPositionBefore = false;
                NegativeBraces = false;
                DateTimeFormatString = null;
                CurrencyPositivePattern = null;
                CurrencyNegativePattern = null;
            }
        }
        #endregion

        #region StiExportService override
        /// <summary>
        /// Gets or sets a default extension of export. 
        /// </summary>
        public override string DefaultExtension => "ods";

        public override StiExportFormat ExportFormat => StiExportFormat.Ods;

        /// <summary>
        /// Gets a group of the export in the context menu.
        /// </summary>
        public override string GroupCategory => "Excel";

        /// <summary>
        /// Gets a position of the export in the context menu.
        /// </summary>
        public override int Position => (int) StiExportPosition.Ods;

        /// <summary>
        /// Gets an export name in the context menu.
        /// </summary>
        public override string ExportNameInMenu => StiLocalization.Get("Export", "ExportTypeCalcFile");

        /// <summary>
        /// Gets a value indicating a number of files in exported document as a result of export
        /// of one page of the rendered report.
        /// </summary>
        public override bool MultipleFiles => false;

        /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public override void ExportTo(StiReport report, Stream stream, StiExportSettings settings)
        {
            ExportOds(report, stream, settings as StiOdsExportSettings);
            InvokeExporting(100, 100, 1, 1);
        }

        /// <summary>
        /// Exports a rendered report to the Ods file.
        /// Also exported document can be sent via e-mail.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        /// <param name="sendEMail">A parameter indicating whether the exported report will be sent via e-mail.</param>
        public override void Export(StiReport report, string fileName, bool sendEMail, StiGuiMode guiMode)
        {
            using (var form = StiGuiOptions.GetExportFormRunner("StiOdsExportSetupForm", guiMode, this.OwnerWindow))
            {
                form["CurrentPage"] = report.CurrentPrintPage;
                form["OpenAfterExportEnabled"] = !sendEMail;

                this.report = report;
                this.fileName = fileName;
                this.sendEMail = sendEMail;
                this.guiMode = guiMode;
                form.Complete += FormComplete;
                form.ShowDialog();
            }
        }

        /// <summary>
        /// Returns a filter for the Ods files.
        /// </summary>
        /// <returns>Returns a filter for the Ods files.</returns>
        public override string GetFilter()
        {
            return StiLocalization.Get("FileFilters", "CalcFiles");
        }
        #endregion

        #region Fields
        private StiReport report;
        private string fileName;
        private bool sendEMail;
        private StiGuiMode guiMode;
        private StiMatrix matrix;
        private StiImageCache imageCache;
        private ArrayList cellStyleList;
        private ArrayList dataStyleList;
        private ArrayList sheetNameList;
        private ArrayList matrixList;
        private ArrayList firstPageIndexList;
        private ArrayList minRowList;
        private ArrayList maxRowList;
        private ArrayList cellStyleTableList;
        private int xmlIndentation = 1;
        private float imageQuality = 0.75f;
        private float imageResolution = 96; //dpi
        private ImageFormat imageFormat = ImageFormat.Jpeg;
        private CultureInfo currentCulture;
        #endregion

        #region Methods
        /// <summary>
        /// Convert value from hinch double to inch string
        /// </summary>
        /// <param name="number">input value in hinch, double</param>
        /// <returns>output value in inch, string</returns>
        private static string DoubleToString(double number)
        {
            return Math.Round(number / 100d, 4).ToString().Replace(",", ".") + "in";
        }

        private void FormComplete(IStiFormRunner form, StiShowDialogCompleteEvetArgs e)
        {
            if (!e.DialogResult)
            {
                IsStopped = true;
                return;
            }

            if (string.IsNullOrEmpty(fileName))
                fileName = base.GetFileName(report, sendEMail);

            if (fileName == null) return;

            StiFileUtils.ProcessReadOnly(fileName);
            try
            {
                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    StartProgress(guiMode);

                    var settings = new StiOdsExportSettings
                    {
                        PageRange = form["PagesRange"] as StiPagesRange,
                        ImageQuality = (float)form["ImageQuality"],
                        ImageResolution = (float)form["Resolution"]
                    };

                    base.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], fileName, guiMode);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string GetColumnName(int column)
        {
            var columnHigh = column / 26;
            var columnLow = column % 26;
            var output = new StringBuilder();

            if (columnHigh > 0)
                output.Append((char) ((byte) 'A' + columnHigh - 1));

            output.Append((char) ((byte) 'A' + columnLow));
            return output.ToString();
        }

        private static string GetColorString(Color color)
        {
            return "#" + color.ToArgb().ToString("X6").Substring(2);
        }

        private int GetCellStyleNumber(int indexRow, int indexColumn, int height, int width)
        {
            var style = new CellStyleData();

            var needBorderLeft = true;
            var needBorderRight = true;

            for (var index = 0; index < height; index++)
            {
                if (matrix.BordersY[indexRow + index, indexColumn] == null)
                    needBorderLeft = false;

                if (matrix.BordersY[indexRow + index, indexColumn + width] == null)
                    needBorderRight = false;
            }

            var needBorderTop = true;
            var needBorderBottom = true;

            for (var index = 0; index < width; index++)
            {
                if (matrix.BordersX[indexRow, indexColumn + index] == null)
                    needBorderTop = false;

                if (matrix.BordersX[indexRow + height, indexColumn + index] == null)
                    needBorderBottom = false;
            }

            if (needBorderTop)
                style.BorderTop = GetStringFromBorder(matrix.BordersX[indexRow, indexColumn]);

            if (needBorderLeft)
                style.BorderLeft = GetStringFromBorder(matrix.BordersY[indexRow, indexColumn]);

            if (needBorderBottom)
                style.BorderBottom = GetStringFromBorder(matrix.BordersX[indexRow + height, indexColumn]);

            if (needBorderRight)
                style.BorderRight = GetStringFromBorder(matrix.BordersY[indexRow, indexColumn + width]);

            if (matrix.Cells[indexRow, indexColumn] != null)
            {
                var cell = matrix.Cells[indexRow, indexColumn];
                var cellStyle = cell.CellStyle;

                style.FontName = cellStyle.Font.Name;
                style.FontSize = cellStyle.Font.SizeInPoints;
                style.Bold = cellStyle.Font.Bold;
                style.Italic = cellStyle.Font.Italic;
                style.Underline = cellStyle.Font.Underline;
                style.FontColor = cellStyle.TextColor;
                style.BackColor = cellStyle.Color;
                style.VertAlign = cellStyle.VertAlignment;
                style.HorAlign = cellStyle.HorAlignment;

                double textAngle = 0;
                if (cellStyle.TextOptions != null)
                {
                    textAngle = cellStyle.TextOptions.Angle;
                    style.Wordwrap = cellStyle.TextOptions.WordWrap;
                }

                style.Angle = (int) Math.Round(textAngle);

                if (cell.Component != null)
                {
                    var textComp = cell.Component as StiText;
                    if (textComp != null)
                        style.DataStyle = GetDataStyleNumber(textComp);

                    var checkComp = cell.Component as StiCheckBox;
                    if (checkComp != null && !string.IsNullOrEmpty(checkComp.ExcelDataValue))
                    {
                        style.FontName = StiOptions.Export.CheckBoxReplacementForExcelValue.Font.Name;
                        style.FontSize = StiOptions.Export.CheckBoxReplacementForExcelValue.Font.SizeInPoints;
                        style.Bold = StiOptions.Export.CheckBoxReplacementForExcelValue.Font.Bold;
                        style.Italic = StiOptions.Export.CheckBoxReplacementForExcelValue.Font.Italic;
                        style.Underline = StiOptions.Export.CheckBoxReplacementForExcelValue.Font.Underline;
                        style.VertAlign = StiOptions.Export.CheckBoxReplacementForExcelValue.VertAlignment;
                        style.HorAlign = StiOptions.Export.CheckBoxReplacementForExcelValue.HorAlignment;
                    }
                }
            }

            if (cellStyleList.Count > 0)
            {
                for (var index = 0; index < cellStyleList.Count; index++)
                {
                    var tempStyle = (CellStyleData) cellStyleList[index];
                    if (tempStyle.BorderLeft == style.BorderLeft &&
                        tempStyle.BorderRight == style.BorderRight &&
                        tempStyle.BorderTop == style.BorderTop &&
                        tempStyle.BorderBottom == style.BorderBottom &&
                        tempStyle.BackColor == style.BackColor &&
                        tempStyle.VertAlign == style.VertAlign &&
                        tempStyle.FontName == style.FontName &&
                        tempStyle.FontSize == style.FontSize &&
                        tempStyle.Bold == style.Bold &&
                        tempStyle.Italic == style.Italic &&
                        tempStyle.Underline == style.Underline &&
                        tempStyle.FontColor == style.FontColor &&
                        tempStyle.HorAlign == style.HorAlign &&
                        tempStyle.Angle == style.Angle &&
                        tempStyle.Wordwrap == style.Wordwrap &&
                        tempStyle.DataStyle == style.DataStyle)
                    {
                        //is already in table, return number 
                        return index;
                    }
                }
            }

            //add to table, return number 
            cellStyleList.Add(style);
            return cellStyleList.Count - 1;
        }

        private static string GetStringFromBorder(StiBorderSide border)
        {
            return $"{DoubleToString(border.Size)} solid {GetColorString(border.Color)}";
        }

        private int GetDataStyleNumber(StiText textComp)
        {
            var style = new DataStyleData();
            var inputFormat = textComp.Format;
            bool hideZeros = (textComp != null) && textComp.HideZeros;

            int posPatternDelimiter = (inputFormat != null ? inputFormat.IndexOf("|") : -1);
            if (posPatternDelimiter != -1)
            {
                style.CurrencyPositivePattern = StiExportUtils.GetPositivePattern((int)inputFormat[posPatternDelimiter + 1] - (int)'A');
                style.CurrencyNegativePattern = StiExportUtils.GetNegativePattern((int)inputFormat[posPatternDelimiter + 2] - (int)'A');
                inputFormat = inputFormat.Substring(0, posPatternDelimiter);
            }

            #region Get value format
            if (!string.IsNullOrEmpty(inputFormat))
            {
                if (inputFormat[0] == 'C')
                    style.isCurrency = true;

                if (inputFormat[0] == 'N')
                    style.isNumeric = true;

                if (inputFormat[0] == 'P')
                    style.isPercent = true;

                if (inputFormat[0] == 'D')
                {
                    style.isDate = true;
                    if (inputFormat.Length > 1)
                        style.DateTimeFormatString = inputFormat.Substring(1);
                }

                if (inputFormat[0] == 'T')
                {
                    style.isTime = true;
                    if (inputFormat.Length > 1)
                        style.DateTimeFormatString = inputFormat.Substring(1);
                }

                if (inputFormat.Length == 1)
                    style.isDefaultFormat = true;

                else
                {
                    if (style.isCurrency || style.isNumeric || style.isPercent)
                    {
                        #region scan parameters
                        var indexPos = 1;
                        if (char.IsDigit(inputFormat[indexPos]))
                        {
                            var decimalSB = new StringBuilder();
                            while (indexPos < inputFormat.Length && char.IsDigit(inputFormat[indexPos]))
                            {
                                decimalSB.Append(inputFormat[indexPos]);
                                indexPos++;
                            }

                            style.DecimalDigits = int.Parse(decimalSB.ToString());
                        }

                        if (indexPos < inputFormat.Length && inputFormat[indexPos] == 'G')
                        {
                            indexPos++;
                            style.GroupDigits = 3;
                        }

                        if (indexPos < inputFormat.Length && inputFormat[indexPos] == '(')
                        {
                            indexPos++;
                            style.NegativeBraces = true;
                        }

                        if (indexPos < inputFormat.Length && (inputFormat[indexPos] == '.' || inputFormat[indexPos] == ','))
                        {
                            style.DecimalComma = inputFormat[indexPos];
                            indexPos++;
                        }

                        if (indexPos < inputFormat.Length &&
                            (inputFormat[indexPos] == '+' || inputFormat[indexPos] == '-'))
                        {
                            if (inputFormat[indexPos] == '+') style.CurrencyPositionBefore = true;
                            indexPos++;
                            if (indexPos < inputFormat.Length)
                            {
                                style.CurrencySymbol = inputFormat.Substring(indexPos);
                            }
                        }
                        #endregion
                    }
                }
            }
            else
            {
                //general format
                return -1;
            }
            #endregion

            if (textComp.HideZeros && !string.IsNullOrEmpty(inputFormat))
                style.isDefaultFormat = false;

            if (dataStyleList.Count > 0)
            {
                for (var index = 0; index < dataStyleList.Count; index++)
                {
                    var tempStyle = (DataStyleData) dataStyleList[index];
                    if (tempStyle.isNumeric == style.isNumeric &&
                        tempStyle.isCurrency == style.isCurrency &&
                        tempStyle.isPercent == style.isPercent &&
                        tempStyle.isDate == style.isDate &&
                        tempStyle.isTime == style.isTime &&
                        tempStyle.isDefaultFormat == style.isDefaultFormat &&
                        tempStyle.DecimalDigits == style.DecimalDigits &&
                        tempStyle.DecimalComma == style.DecimalComma &&
                        tempStyle.GroupDigits == style.GroupDigits &&
                        tempStyle.CurrencySymbol == style.CurrencySymbol &&
                        tempStyle.CurrencyPositionBefore == style.CurrencyPositionBefore &&
                        tempStyle.DateTimeFormatString == style.DateTimeFormatString &&
                        tempStyle.CurrencyPositivePattern == style.CurrencyPositivePattern &&
                        tempStyle.CurrencyNegativePattern == style.CurrencyNegativePattern)
                    {
                        //is already in table, return number 
                        return index;
                    }
                }
            }

            //add to table, return number 
            dataStyleList.Add(style);
            var temp = dataStyleList.Count - 1;
            return temp;
        }

        private string GetImageFormatExtension(ImageFormat currImageFormat)
        {
            if (currImageFormat == ImageFormat.Png) return "png";
            return "jpeg";
        }

        private MemoryStream WriteMimetype()
        {
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.ASCII);
            writer.Write("application/vnd.oasis.opendocument.spreadsheet");
            writer.Flush();
            return ms;
        }

        private MemoryStream WriteMeta()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8)
            {
                Indentation = xmlIndentation < 0 ? 0 : xmlIndentation,
                Formatting = xmlIndentation < 0 ? Formatting.None : Formatting.Indented
            };
            writer.WriteStartDocument();

            writer.WriteStartElement("office:document-meta");
            writer.WriteAttributeString("xmlns:office", "urn:oasis:names:tc:opendocument:xmlns:office:1.0");
            writer.WriteAttributeString("xmlns:xlink", "http://www.w3.org/1999/xlink");
            writer.WriteAttributeString("xmlns:dc", "http://purl.org/dc/elements/1.1/");
            writer.WriteAttributeString("xmlns:meta", "urn:oasis:names:tc:opendocument:xmlns:meta:1.0");
            writer.WriteAttributeString("xmlns:ooo", "http://openoffice.org/2004/office");
            writer.WriteAttributeString("office:version", "1.1");
            writer.WriteStartElement("office:meta");

            var dateTime = $"{DateTime.Now:yyyy-MM-ddTHH:mm:ss}";
            var creator = StiExportUtils.GetReportVersion(report);

            writer.WriteElementString("meta:generator", creator);
            writer.WriteElementString("meta:creation-date", dateTime);
            writer.WriteElementString("dc:date", dateTime);
            writer.WriteElementString("meta:editing-cycles", "1");
            writer.WriteElementString("meta:editing-duration", "PT0M0S");
            writer.WriteStartElement("meta:user-defined");
            writer.WriteAttributeString("meta:name", "Info 1");
            writer.WriteEndElement();
            writer.WriteStartElement("meta:user-defined");
            writer.WriteAttributeString("meta:name", "Info 2");
            writer.WriteEndElement();
            writer.WriteStartElement("meta:user-defined");
            writer.WriteAttributeString("meta:name", "Info 3");
            writer.WriteEndElement();
            writer.WriteStartElement("meta:user-defined");
            writer.WriteAttributeString("meta:name", "Info 4");
            writer.WriteEndElement();

            writer.WriteStartElement("meta:document-statistic");
            writer.WriteAttributeString("meta:table-count", "1");
            writer.WriteAttributeString("meta:cell-count", "1");
            writer.WriteEndElement();

            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }

        private MemoryStream WriteManifest()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8)
            {
                Indentation = xmlIndentation < 0 ? 0 : xmlIndentation,
                Formatting = xmlIndentation < 0 ? Formatting.None : Formatting.Indented
            };
            writer.WriteStartDocument();
            writer.WriteStartElement("manifest:manifest");
            writer.WriteAttributeString("xmlns:manifest", "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0");

            writer.WriteStartElement("manifest:file-entry");
            writer.WriteAttributeString("manifest:media-type", "application/vnd.oasis.opendocument.spreadsheet");
            writer.WriteAttributeString("manifest:full-path", "/");
            writer.WriteEndElement();

            for (var index = 0; index < imageCache.ImagePackedStore.Count; index++)
            {
                string ext = GetImageFormatExtension(imageCache.ImageFormatStore[index]);
                writer.WriteStartElement("manifest:file-entry");
                writer.WriteAttributeString("manifest:media-type", $"image/{ext}");
                writer.WriteAttributeString("manifest:full-path", $"Pictures/{index + 1:D5}.{ext}");
                writer.WriteEndElement();
            }

            writer.WriteStartElement("manifest:file-entry");
            writer.WriteAttributeString("manifest:media-type", "text/xml");
            writer.WriteAttributeString("manifest:full-path", "content.xml");
            writer.WriteEndElement();

            writer.WriteStartElement("manifest:file-entry");
            writer.WriteAttributeString("manifest:media-type", "text/xml");
            writer.WriteAttributeString("manifest:full-path", "styles.xml");
            writer.WriteEndElement();

            writer.WriteStartElement("manifest:file-entry");
            writer.WriteAttributeString("manifest:media-type", "text/xml");
            writer.WriteAttributeString("manifest:full-path", "meta.xml");
            writer.WriteEndElement();

            writer.WriteStartElement("manifest:file-entry");
            writer.WriteAttributeString("manifest:media-type", "text/xml");
            writer.WriteAttributeString("manifest:full-path", "settings.xml");
            writer.WriteEndElement();

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }

        private MemoryStream WriteImage(int number)
        {
            var ms = new MemoryStream();
            var buf = (byte[]) imageCache.ImagePackedStore[number];
            ms.Write(buf, 0, buf.Length);
            return ms;
        }

        private MemoryStream WriteSettings()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8)
            {
                Indentation = xmlIndentation < 0 ? 0 : xmlIndentation,
                Formatting = xmlIndentation < 0 ? Formatting.None : Formatting.Indented
            };
            writer.WriteStartDocument();

            writer.WriteStartElement("office:document-settings");
            writer.WriteAttributeString("xmlns:office", "urn:oasis:names:tc:opendocument:xmlns:office:1.0");
            writer.WriteAttributeString("xmlns:xlink", "http://www.w3.org/1999/xlink");
            writer.WriteAttributeString("xmlns:config", "urn:oasis:names:tc:opendocument:xmlns:config:1.0");
            writer.WriteAttributeString("xmlns:ooo", "http://openoffice.org/2004/office");
            writer.WriteAttributeString("office:version", "1.1");
            writer.WriteStartElement("office:settings");

            writer.WriteStartElement("config:config-item-set");
            writer.WriteAttributeString("config:name", "ooo:view-settings");

            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "VisibleAreaTop");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("0");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "VisibleAreaLeft");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("-10107");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "VisibleAreaWidth");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("43208");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "VisibleAreaHeight");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("22174");
            writer.WriteEndElement();
            //config:config-item-map-indexed
            writer.WriteStartElement("config:config-item-map-indexed");
            writer.WriteAttributeString("config:name", "Views");
            writer.WriteStartElement("config:config-item-map-entry");

            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "ViewId");
            writer.WriteAttributeString("config:type", "string");
            writer.WriteString("View1");
            writer.WriteEndElement();

            writer.WriteStartElement("config:config-item-map-named");
            writer.WriteAttributeString("config:name", "Tables");
            writer.WriteStartElement("config:config-item-map-entry");
            writer.WriteAttributeString("config:name", "Sheet1");
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "CursorPositionX");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("1");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "CursorPositionY");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("1");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "HorizontalSplitMode");
            writer.WriteAttributeString("config:type", "short");
            writer.WriteString("0");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "VerticalSplitMode");
            writer.WriteAttributeString("config:type", "short");
            writer.WriteString("0");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "HorizontalSplitPosition");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("0");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "VerticalSplitPosition");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("0");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "ActiveSplitRange");
            writer.WriteAttributeString("config:type", "short");
            writer.WriteString("2");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "PositionLeft");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("0");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "PositionRight");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("0");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "PositionTop");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("0");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "PositionBottom");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("0");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "ActiveTable");
            writer.WriteAttributeString("config:type", "string");
            writer.WriteString("Sheet1");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "HorizontalScrollbarWidth");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("270");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "ZoomType");
            writer.WriteAttributeString("config:type", "short");
            writer.WriteString("0");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "ZoomValue");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("100");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "PageViewZoomValue");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("60");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "ShowPageBreakPreview");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("false");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "ShowZeroValues");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("true");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "ShowNotes");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("true");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "ShowGrid");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("true");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "GridColor");
            writer.WriteAttributeString("config:type", "long");
            writer.WriteString("12632256");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "ShowPageBreaks");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("true");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "HasColumnRowHeaders");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("true");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "HasSheetTabs");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("true");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "IsOutlineSymbolsSet");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("true");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "IsSnapToRaster");
            writer.WriteAttributeString("config:type", "long");
            writer.WriteString("false");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "RasterIsVisible");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("false");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "RasterResolutionX");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("1000");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "RasterResolutionY");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("1000");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "RasterSubdivisionX");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("1");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "RasterSubdivisionY");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("1");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "IsRasterAxisSynchronized");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("true");
            writer.WriteEndElement();

            writer.WriteEndElement(); //config:config-item-map-entry
            writer.WriteEndElement(); //config:config-item-map-indexed
            writer.WriteEndElement(); //config:config-item-set


            writer.WriteStartElement("config:config-item-set");
            writer.WriteAttributeString("config:name", "ooo:configuration-settings");

            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "ShowZeroValues");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("true");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "ShowNotes");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("true");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "ShowGrid");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("true");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "GridColor");
            writer.WriteAttributeString("config:type", "long");
            writer.WriteString("12632256");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "ShowPageBreaks");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("true");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "LinkUpdateMode");
            writer.WriteAttributeString("config:type", "short");
            writer.WriteString("3");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "HasColumnRowHeaders");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("true");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "HasSheetTabs");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("true");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "IsOutlineSymbolsSet");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("true");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "IsSnapToRaster");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("false");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "RasterIsVisible");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("false");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "RasterResolutionX");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("1000");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "RasterResolutionY");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("1000");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "RasterSubdivisionX");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("1");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "RasterSubdivisionY");
            writer.WriteAttributeString("config:type", "int");
            writer.WriteString("1");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "IsRasterAxisSynchronized");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("true");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "AutoCalculate");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("true");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "PrinterName");
            writer.WriteAttributeString("config:type", "string");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "PrinterSetup");
            writer.WriteAttributeString("config:type", "base64Binary");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "ApplyUserData");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("true");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "CharacterCompressionType");
            writer.WriteAttributeString("config:type", "short");
            writer.WriteString("0");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "IsKernAsianPunctuation");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("false");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "SaveVersionOnClose");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("false");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "UpdateFromTemplate");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("true");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "AllowPrintJobCancel");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("true");
            writer.WriteEndElement();
            writer.WriteStartElement("config:config-item");
            writer.WriteAttributeString("config:name", "LoadReadonly");
            writer.WriteAttributeString("config:type", "boolean");
            writer.WriteString("false");
            writer.WriteEndElement();

            writer.WriteEndElement(); //config:config-item-set

            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }

        private MemoryStream WriteStyles(StiPagesCollection pages)
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8)
            {
                Indentation = xmlIndentation < 0 ? 0 : xmlIndentation,
                Formatting = xmlIndentation < 0 ? Formatting.None : Formatting.Indented
            };
            writer.WriteStartDocument();

            writer.WriteStartElement("office:document-styles");
            writer.WriteAttributeString("xmlns:office", "urn:oasis:names:tc:opendocument:xmlns:office:1.0");
            writer.WriteAttributeString("xmlns:style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
            writer.WriteAttributeString("xmlns:text", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
            writer.WriteAttributeString("xmlns:table", "urn:oasis:names:tc:opendocument:xmlns:table:1.0");
            writer.WriteAttributeString("xmlns:draw", "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0");
            writer.WriteAttributeString("xmlns:fo", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0");
            writer.WriteAttributeString("xmlns:xlink", "http://www.w3.org/1999/xlink");
            writer.WriteAttributeString("xmlns:dc", "http://purl.org/dc/elements/1.1/");
            writer.WriteAttributeString("xmlns:meta", "urn:oasis:names:tc:opendocument:xmlns:meta:1.0");
            writer.WriteAttributeString("xmlns:number", "urn:oasis:names:tc:opendocument:xmlns:datastyle:1.0");
            writer.WriteAttributeString("xmlns:presentation", "urn:oasis:names:tc:opendocument:xmlns:presentation:1.0");
            writer.WriteAttributeString("xmlns:svg", "urn:oasis:names:tc:opendocument:xmlns:svg-compatible:1.0");
            writer.WriteAttributeString("xmlns:chart", "urn:oasis:names:tc:opendocument:xmlns:chart:1.0");
            writer.WriteAttributeString("xmlns:dr3d", "urn:oasis:names:tc:opendocument:xmlns:dr3d:1.0");
            writer.WriteAttributeString("xmlns:math", "http://www.w3.org/1998/Math/MathML");
            writer.WriteAttributeString("xmlns:form", "urn:oasis:names:tc:opendocument:xmlns:form:1.0");
            writer.WriteAttributeString("xmlns:script", "urn:oasis:names:tc:opendocument:xmlns:script:1.0");
            writer.WriteAttributeString("xmlns:ooo", "http://openoffice.org/2004/office");
            writer.WriteAttributeString("xmlns:ooow", "http://openoffice.org/2004/writer");
            writer.WriteAttributeString("xmlns:oooc", "http://openoffice.org/2004/calc");
            writer.WriteAttributeString("xmlns:dom", "http://www.w3.org/2001/xml-events");
            writer.WriteAttributeString("office:version", "1.1");

            writer.WriteStartElement("office:font-face-decls");
            writer.WriteStartElement("style:font-face");
            writer.WriteAttributeString("style:name", "Arial");
            writer.WriteAttributeString("svg:font-family", "Arial");
            writer.WriteEndElement();
            writer.WriteStartElement("style:font-face");
            writer.WriteAttributeString("style:name", "Tahoma");
            writer.WriteAttributeString("svg:font-family", "Tahoma");
            writer.WriteAttributeString("style:font-family-generic", "system");
            writer.WriteAttributeString("style:font-pitch", "variable");
            writer.WriteEndElement();
            writer.WriteEndElement();

            //
            writer.WriteStartElement("office:styles");

            //style:family table-cell
            writer.WriteStartElement("style:default-style");
            writer.WriteAttributeString("style:family", "table-cell");
            writer.WriteStartElement("style:table-cell-properties");
            writer.WriteAttributeString("style:decimal-places", "2");
            writer.WriteEndElement();
            writer.WriteStartElement("style:paragraph-properties");
            writer.WriteAttributeString("style:tab-stop-distance", "1.25cm");
            writer.WriteEndElement();
            writer.WriteStartElement("style:text-properties");
            writer.WriteAttributeString("style:font-name", "Arial");
            writer.WriteAttributeString("fo:language", "ru");
            writer.WriteAttributeString("fo:country", "RU");
            writer.WriteAttributeString("style:font-name-asian", "Lucida Sans Unicode");
            writer.WriteAttributeString("style:language-asian", "zxx");
            writer.WriteAttributeString("style:country-asian", "none");
            writer.WriteAttributeString("style:font-name-complex", "Tahoma");
            writer.WriteAttributeString("style:language-complex", "zxx");
            writer.WriteAttributeString("style:country-complex", "none");
            writer.WriteEndElement();
            writer.WriteEndElement(); //---

            //style:name N0
            writer.WriteStartElement("number:number-style");
            writer.WriteAttributeString("style:name", "N0");
            writer.WriteStartElement("number:number");
            writer.WriteAttributeString("number:min-integer-digits", "1");
            writer.WriteEndElement();
            writer.WriteEndElement(); //---

            //style:name Default
            writer.WriteStartElement("style:style");
            writer.WriteAttributeString("style:name", "Default");
            writer.WriteAttributeString("style:family", "table-cell");
            writer.WriteEndElement(); //---

            //style:name Result
            writer.WriteStartElement("style:style");
            writer.WriteAttributeString("style:name", "Result");
            writer.WriteAttributeString("style:family", "table-cell");
            writer.WriteAttributeString("style:parent-style-name", "Default");
            writer.WriteStartElement("style:text-properties");
            writer.WriteAttributeString("fo:font-style", "italic");
            writer.WriteAttributeString("style:text-underline-style", "solid");
            writer.WriteAttributeString("style:text-underline-width", "auto");
            writer.WriteAttributeString("style:text-underline-color", "font-color");
            writer.WriteAttributeString("fo:font-weight", "bold");
            writer.WriteEndElement();
            writer.WriteEndElement(); //---

            //style:name Heading
            writer.WriteStartElement("style:style");
            writer.WriteAttributeString("style:name", "Heading");
            writer.WriteAttributeString("style:family", "table-cell");
            writer.WriteAttributeString("style:parent-style-name", "Default");
            writer.WriteStartElement("style:table-cell-properties");
            writer.WriteAttributeString("style:text-align-source", "fix");
            writer.WriteAttributeString("style:repeat-content", "false");
            writer.WriteEndElement();
            writer.WriteStartElement("style:paragraph-properties");
            writer.WriteAttributeString("fo:text-align", "center");
            writer.WriteEndElement();
            writer.WriteStartElement("style:text-properties");
            writer.WriteAttributeString("fo:font-size", "16pt");
            writer.WriteAttributeString("fo:font-style", "italic");
            writer.WriteAttributeString("fo:font-weight", "bold");
            writer.WriteEndElement();
            writer.WriteEndElement(); //---

            //style:name Heading1
            writer.WriteStartElement("style:style");
            writer.WriteAttributeString("style:name", "Heading1");
            writer.WriteAttributeString("style:family", "table-cell");
            writer.WriteAttributeString("style:parent-style-name", "Heading");
            writer.WriteStartElement("style:table-cell-properties");
            writer.WriteAttributeString("style:rotation-angle", "90");
            writer.WriteEndElement();
            writer.WriteEndElement(); //---

            writer.WriteFullEndElement(); //office:styles

            writer.WriteStartElement("office:automatic-styles");

            var page = pages[0];
            var pageHeight = page.Unit.ConvertToHInches(page.PageHeight * page.SegmentPerHeight);
            var pageWidth = page.Unit.ConvertToHInches(page.PageWidth * page.SegmentPerWidth);
            var mgLeft = page.Unit.ConvertToHInches(page.Margins.Left);
            var mgRight = page.Unit.ConvertToHInches(page.Margins.Right);
            var mgTop = page.Unit.ConvertToHInches(page.Margins.Top);
            var mgBottom = page.Unit.ConvertToHInches(page.Margins.Bottom) - 4; //correction
            if (mgBottom < 0) mgBottom = 0;

            writer.WriteStartElement("style:page-layout");
            writer.WriteAttributeString("style:name", "pm1");
            writer.WriteStartElement("style:page-layout-properties");
            writer.WriteAttributeString("fo:page-width", DoubleToString(pageWidth));
            writer.WriteAttributeString("fo:page-height", DoubleToString(pageHeight));
            writer.WriteAttributeString("style:num-format", "1");
            writer.WriteAttributeString("style:print-orientation", page.Orientation == StiPageOrientation.Portrait ? "portrait" : "landscape");
            writer.WriteAttributeString("fo:margin-top", DoubleToString(mgTop));
            writer.WriteAttributeString("fo:margin-bottom", DoubleToString(mgBottom));
            writer.WriteAttributeString("fo:margin-left", DoubleToString(mgLeft));
            writer.WriteAttributeString("fo:margin-right", DoubleToString(mgRight));
            writer.WriteAttributeString("style:writing-mode", "lr-tb");
            writer.WriteEndElement();
            writer.WriteStartElement("style:header-style");
            writer.WriteStartElement("style:header-footer-properties");
            writer.WriteAttributeString("fo:min-height", "0.101cm");
            writer.WriteAttributeString("fo:margin-left", "0cm");
            writer.WriteAttributeString("fo:margin-right", "0cm");
            writer.WriteAttributeString("fo:margin-bottom", "0cm");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("style:footer-style");
            writer.WriteStartElement("style:header-footer-properties");
            writer.WriteAttributeString("fo:min-height", "0.101cm");
            writer.WriteAttributeString("fo:margin-left", "0cm");
            writer.WriteAttributeString("fo:margin-right", "0cm");
            writer.WriteAttributeString("fo:margin-top", "0cm");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteFullEndElement(); //office:automatic-styles

            writer.WriteStartElement("office:master-styles");

            writer.WriteStartElement("style:master-page");
            writer.WriteAttributeString("style:name", "Default");
            writer.WriteAttributeString("style:page-layout-name", "pm1");
            writer.WriteStartElement("style:header");
            writer.WriteAttributeString("style:display", "false");
            writer.WriteStartElement("text:p");
            writer.WriteStartElement("text:sheet-name");
            writer.WriteString("Sheet1");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("style:header-left");
            writer.WriteAttributeString("style:display", "false");
            writer.WriteEndElement();
            writer.WriteStartElement("style:footer");
            writer.WriteAttributeString("style:display", "false");
            writer.WriteStartElement("text:p");
            writer.WriteString("Page ");
            writer.WriteStartElement("text:page-number");
            writer.WriteString("1");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("style:footer-left");
            writer.WriteAttributeString("style:display", "false");
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteEndElement(); //office:master-styles

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }

        private MemoryStream WriteContent(StiReport report, StiPagesCollection allPages)
        {
            MemoryStream ms = new Tools.StiCachedStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8)
            {
                Indentation = xmlIndentation < 0 ? 0 : xmlIndentation,
                Formatting = xmlIndentation < 0 ? Formatting.None : Formatting.Indented
            };
            writer.WriteStartDocument();

            #region Write begin of section
            writer.WriteStartElement("office:document-content");
            writer.WriteAttributeString("xmlns:office", "urn:oasis:names:tc:opendocument:xmlns:office:1.0");
            writer.WriteAttributeString("xmlns:style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
            writer.WriteAttributeString("xmlns:text", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
            writer.WriteAttributeString("xmlns:table", "urn:oasis:names:tc:opendocument:xmlns:table:1.0");
            writer.WriteAttributeString("xmlns:draw", "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0");
            writer.WriteAttributeString("xmlns:fo", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0");
            writer.WriteAttributeString("xmlns:xlink", "http://www.w3.org/1999/xlink");
            writer.WriteAttributeString("xmlns:dc", "http://purl.org/dc/elements/1.1/");
            writer.WriteAttributeString("xmlns:meta", "urn:oasis:names:tc:opendocument:xmlns:meta:1.0");
            writer.WriteAttributeString("xmlns:number", "urn:oasis:names:tc:opendocument:xmlns:datastyle:1.0");
            writer.WriteAttributeString("xmlns:presentation", "urn:oasis:names:tc:opendocument:xmlns:presentation:1.0");
            writer.WriteAttributeString("xmlns:svg", "urn:oasis:names:tc:opendocument:xmlns:svg-compatible:1.0");
            writer.WriteAttributeString("xmlns:chart", "urn:oasis:names:tc:opendocument:xmlns:chart:1.0");
            writer.WriteAttributeString("xmlns:dr3d", "urn:oasis:names:tc:opendocument:xmlns:dr3d:1.0");
            writer.WriteAttributeString("xmlns:math", "http://www.w3.org/1998/Math/MathML");
            writer.WriteAttributeString("xmlns:form", "urn:oasis:names:tc:opendocument:xmlns:form:1.0");
            writer.WriteAttributeString("xmlns:script", "urn:oasis:names:tc:opendocument:xmlns:script:1.0");
            writer.WriteAttributeString("xmlns:ooo", "http://openoffice.org/2004/office");
            writer.WriteAttributeString("xmlns:ooow", "http://openoffice.org/2004/writer");
            writer.WriteAttributeString("xmlns:oooc", "http://openoffice.org/2004/calc");
            writer.WriteAttributeString("xmlns:dom", "http://www.w3.org/2001/xml-events");
            writer.WriteAttributeString("xmlns:xforms", "http://www.w3.org/2002/xforms");
            writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
            writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            writer.WriteAttributeString("office:version", "1.1");
            #endregion

            #region Prepare matrixList
            var indexPage = 0;
            while (indexPage < allPages.Count)
            {
                var pages = new StiPagesCollection(report, allPages);
                pages.AddV2Internal(allPages[indexPage]);
                var firstPageIndex = indexPage;
                var pageName = allPages[indexPage].ExcelSheetValue;

                while (indexPage < allPages.Count - 1 && allPages[indexPage + 1].ExcelSheetValue == pageName)
                {
                    indexPage++;
                    pages.AddV2Internal(allPages[indexPage]);
                }

                pages.CacheMode = report.RenderedPages.CacheMode;

                var sheetName = pages[0].ExcelSheetValue;
                if (string.IsNullOrEmpty(sheetName))
                    sheetName = $"Page {sheetNameList.Count + 1}";

                var sheetSuffix = string.Empty;
                var sheetIndex = 1;

                matrix = new StiMatrix(pages, StiOptions.Export.OpenDocumentCalc.DivideBigCells, this);
                if (IsStopped) return null;

                var minRowIndex = 0;

                do
                {
                    firstPageIndexList.Add(firstPageIndex);
                    matrixList.Add(matrix);
                    var maxRowIndex = matrix.CoordY.Count - 1;

                    if (maxRowIndex - minRowIndex > StiOptions.Export.OpenDocumentCalc.MaximumSheetHeight)
                        maxRowIndex = minRowIndex + StiOptions.Export.OpenDocumentCalc.MaximumSheetHeight;

                    else
                        matrix = null;

                    minRowList.Add(minRowIndex);
                    maxRowList.Add(maxRowIndex);
                    minRowIndex = maxRowIndex;

                    if (matrix != null || sheetSuffix.Length > 0)
                        sheetSuffix = $" part{sheetIndex++}";

                    sheetNameList.Add(sheetName + sheetSuffix);
                    if (IsStopped) return null;
                }
                while (matrix != null);

                indexPage++;
            }
            #endregion

            var rowHeightList = new Hashtable();
            var rowHeightList2 = new Hashtable();
            var colWidthList = new Hashtable();
            var colWidthList2 = new Hashtable();

            for (var indexSheet = 0; indexSheet < matrixList.Count; indexSheet++)
            {
                matrix = (StiMatrix) matrixList[indexSheet];
                var minRowIndex = (int) minRowList[indexSheet];
                var maxRowIndex = (int) maxRowList[indexSheet];

                #region First pass - collect styles table
                var readyCells = new bool[matrix.CoordY.Count, matrix.CoordX.Count];
                var cellStyleTable = new int[matrix.CoordY.Count, matrix.CoordX.Count];

                for (var indexColumn = 0; indexColumn < matrix.CoordX.Count - 1; indexColumn++)
                {
                    var columnWidth = (double) matrix.CoordX.GetByIndex(indexColumn + 1) - (double) matrix.CoordX.GetByIndex(indexColumn);
                    if (!colWidthList.ContainsKey(columnWidth))
                    {
                        var listPos = colWidthList.Count;
                        colWidthList[columnWidth] = listPos;
                        colWidthList2[listPos] = columnWidth;
                    }
                }

                for (var indexRow = minRowIndex + 1; indexRow < maxRowIndex + 1; indexRow++)
                {
                    var rowHeight = (double) matrix.CoordY.GetByIndex(indexRow) - (double) matrix.CoordY.GetByIndex(indexRow - 1);
                    if (!rowHeightList.ContainsKey(rowHeight))
                    {
                        var listPos = rowHeightList.Count;
                        rowHeightList[rowHeight] = listPos;
                        rowHeightList2[listPos] = rowHeight;
                    }

                    for (var indexColumn = 1; indexColumn < matrix.CoordX.Count; indexColumn++)
                    {
                        var cell = matrix.Cells[indexRow - 1, indexColumn - 1];

                        if (!readyCells[indexRow, indexColumn])
                        {
                            if (cell != null)
                            {
                                #region Range
                                for (var yy = 0; yy <= cell.Height; yy++)
                                {
                                    for (var xx = 0; xx <= cell.Width; xx++)
                                    {
                                        readyCells[indexRow + yy, indexColumn + xx] = true;
                                    }
                                }
                                #endregion

                                cellStyleTable[indexRow - 1, indexColumn - 1] = GetCellStyleNumber(indexRow - 1, indexColumn - 1, cell.Height + 1, cell.Width + 1);
                            }
                            else
                            {
                                cellStyleTable[indexRow - 1, indexColumn - 1] = GetCellStyleNumber(indexRow - 1, indexColumn - 1, 1, 1);
                            }
                        }
                    }
                }
                #endregion

                cellStyleTableList.Add(cellStyleTable);
            }

            writer.WriteStartElement("office:scripts");
            writer.WriteEndElement();

            #region Write fonts info
            var fonts = new Hashtable();
            foreach (CellStyleData tempStyle in cellStyleList)
            {
                fonts[tempStyle.FontName] = tempStyle.FontName;
            }

            writer.WriteStartElement("office:font-face-decls");
            foreach (DictionaryEntry de in fonts)
            {
                var fontName = (string) de.Value;
                writer.WriteStartElement("style:font-face");
                writer.WriteAttributeString("style:name", fontName);
                writer.WriteAttributeString("svg:font-family", fontName);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            #endregion

            #region Write automatic styles
            writer.WriteStartElement("office:automatic-styles");

            for (var indexColumn = 0; indexColumn < colWidthList.Count; indexColumn++)
            {
                writer.WriteStartElement("style:style");
                writer.WriteAttributeString("style:name", $"co{indexColumn + 1}");
                writer.WriteAttributeString("style:family", "table-column");
                writer.WriteStartElement("style:table-column-properties");
                writer.WriteAttributeString("fo:break-before", "auto");
                var columnWidth = (double) colWidthList2[indexColumn];
                writer.WriteAttributeString("style:column-width", DoubleToString(columnWidth));
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            for (var indexRow = 0; indexRow < rowHeightList.Count; indexRow++)
            {
                writer.WriteStartElement("style:style");
                writer.WriteAttributeString("style:name", $"ro{indexRow + 1}");
                writer.WriteAttributeString("style:family", "table-row");
                writer.WriteStartElement("style:table-row-properties");
                var rowHeight = (double) rowHeightList2[indexRow];
                writer.WriteAttributeString("style:row-height", DoubleToString(rowHeight));
                writer.WriteAttributeString("fo:break-before", "auto");
                writer.WriteAttributeString("style:use-optimal-row-height", "false"); //exact height
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            writer.WriteStartElement("style:style");
            writer.WriteAttributeString("style:name", "ta1");
            writer.WriteAttributeString("style:family", "table");
            writer.WriteAttributeString("style:master-page-name", "Default");
            writer.WriteStartElement("style:table-properties");
            writer.WriteAttributeString("table:display", "true");
            writer.WriteAttributeString("style:writing-mode", "lr-tb");
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("style:style");
            writer.WriteAttributeString("style:name", "gr1");
            writer.WriteAttributeString("style:family", "graphic");
            writer.WriteStartElement("style:graphic-properties");
            writer.WriteAttributeString("draw:stroke", "none");
            writer.WriteAttributeString("draw:fill", "none");
            writer.WriteAttributeString("draw:textarea-horizontal-align", "center");
            writer.WriteAttributeString("draw:textarea-vertical-align", "middle");
            writer.WriteAttributeString("draw:color-mode", "standard");
            writer.WriteAttributeString("draw:luminance", "0%");
            writer.WriteAttributeString("draw:contrast", "0%");
            writer.WriteAttributeString("draw:gamma", "100%");
            writer.WriteAttributeString("draw:red", "0%");
            writer.WriteAttributeString("draw:green", "0%");
            writer.WriteAttributeString("draw:blue", "0%");
            writer.WriteAttributeString("fo:clip", "rect(0cm 0cm 0cm 0cm)");
            writer.WriteAttributeString("draw:image-opacity", "100%");
            writer.WriteAttributeString("style:mirror", "none");
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("style:style");
            writer.WriteAttributeString("style:name", "P1");
            writer.WriteAttributeString("style:family", "paragraph");
            writer.WriteStartElement("style:paragraph-properties");
            writer.WriteAttributeString("fo:text-align", "center");
            writer.WriteEndElement();
            writer.WriteEndElement();

            for (var indexStyle = 0; indexStyle < dataStyleList.Count; indexStyle++)
            {
                var style = (DataStyleData) dataStyleList[indexStyle];

                #region Style.isNumeric
                if (style.isNumeric)
                {
                    writer.WriteStartElement("number:number-style");
                    if (style.NegativeBraces)
                    {
                        writer.WriteAttributeString("style:name", $"N{indexStyle + 1}P0");
                        writer.WriteAttributeString("style:volatile", "true");
                    }
                    else
                    {
                        writer.WriteAttributeString("style:name", $"N{indexStyle + 1}");
                    }

                    if (style.DecimalComma != ' ')
                    {
                        writer.WriteAttributeString("number:language", style.DecimalComma == ',' ? "ru" : "en");
                        writer.WriteAttributeString("number:country", style.DecimalComma == ',' ? "RU" : "US");
                    }

                    writer.WriteStartElement("number:number");
                    writer.WriteAttributeString("number:decimal-places", style.DecimalDigits.ToString());
                    writer.WriteAttributeString("number:min-integer-digits", "1");
                    writer.WriteAttributeString("number:grouping", style.GroupDigits == 3 ? "true" : "false");
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    if (style.NegativeBraces)
                    {
                        writer.WriteStartElement("number:number-style");
                        writer.WriteAttributeString("style:name", $"N{indexStyle + 1}");

                        if (style.DecimalComma != ' ')
                        {
                            writer.WriteAttributeString("number:language", style.DecimalComma == ',' ? "ru" : "en");
                            writer.WriteAttributeString("number:country", style.DecimalComma == ',' ? "RU" : "US");
                        }

                        writer.WriteElementString("number:text", "(");
                        writer.WriteStartElement("number:number");
                        writer.WriteAttributeString("number:decimal-places", style.DecimalDigits.ToString());
                        writer.WriteAttributeString("number:min-integer-digits", "1");
                        writer.WriteAttributeString("number:grouping", style.GroupDigits == 3 ? "true" : "false");
                        writer.WriteEndElement();
                        writer.WriteElementString("number:text", ")");
                        writer.WriteStartElement("style:map");
                        writer.WriteAttributeString("style:condition", "value()>=0");
                        writer.WriteAttributeString("style:apply-style-name", $"N{indexStyle + 1}P0");
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }
                }
                #endregion

                #region Style.isPercent
                if (style.isPercent)
                {
                    writer.WriteStartElement("number:percentage-style");
                    writer.WriteAttributeString("style:name", $"N{indexStyle + 1}");

                    if (style.DecimalComma != ' ')
                    {
                        writer.WriteAttributeString("number:language", style.DecimalComma == ',' ? "ru" : "en");
                        writer.WriteAttributeString("number:country", style.DecimalComma == ',' ? "RU" : "US");
                    }

                    writer.WriteStartElement("number:number");
                    writer.WriteAttributeString("number:decimal-places", style.DecimalDigits.ToString());
                    writer.WriteAttributeString("number:min-integer-digits", "1");

                    if (style.GroupDigits == 3)
                        writer.WriteAttributeString("number:grouping", "true");

                    writer.WriteEndElement();
                    writer.WriteElementString("number:text", "%");
                    writer.WriteEndElement();
                }
                #endregion

                #region Style.isCurrency
                if (style.isCurrency)
                {
                    writer.WriteStartElement("number:currency-style");
                    writer.WriteAttributeString("style:name", $"N{indexStyle + 1}");

                    if (style.DecimalComma != ' ')
                    {
                        writer.WriteAttributeString("number:language", style.DecimalComma == ',' ? "ru" : "en");
                        writer.WriteAttributeString("number:country", style.DecimalComma == ',' ? "RU" : "US");
                    }

                    if (style.CurrencyPositionBefore)
                        writer.WriteElementString("number:currency-symbol", style.CurrencySymbol);

                    writer.WriteStartElement("number:number");
                    writer.WriteAttributeString("number:decimal-places", style.DecimalDigits.ToString());
                    writer.WriteAttributeString("number:min-integer-digits", "1");
                    writer.WriteAttributeString("number:grouping", style.GroupDigits == 3 ? "true" : "false");
                    // negative bracers?
                    writer.WriteEndElement();

                    if (!style.CurrencyPositionBefore)
                        writer.WriteElementString("number:currency-symbol", style.CurrencySymbol);

                    writer.WriteEndElement();
                }
                #endregion

                #region Style.isDate
                if (style.isDate)
                {
                    writer.WriteStartElement("number:date-style");
                    writer.WriteAttributeString("style:name", $"N{indexStyle + 1}");
                    WriteDateTimeFormatString(writer, style.DateTimeFormatString, "d");
                    writer.WriteEndElement();
                }
                #endregion

                #region Style.isTime
                if (style.isTime)
                {
                    writer.WriteStartElement("number:time-style");
                    writer.WriteAttributeString("style:name", $"N{indexStyle + 1}");

                    if (!string.IsNullOrEmpty(style.DateTimeFormatString) && style.DateTimeFormatString.IndexOf("h", StringComparison.Ordinal) != -1)
                    {
                        writer.WriteAttributeString("number:language", "en");
                        writer.WriteAttributeString("number:country", "US");
                    }

                    WriteDateTimeFormatString(writer, style.DateTimeFormatString, "t");
                    writer.WriteEndElement();
                }
                #endregion
            }

            #region Cell styles
            for (var indexStyle = 0; indexStyle < cellStyleList.Count; indexStyle++)
            {
                var style = (CellStyleData) cellStyleList[indexStyle];

                writer.WriteStartElement("style:style");
                writer.WriteAttributeString("style:name", $"ce{indexStyle + 1}");
                writer.WriteAttributeString("style:family", "table-cell");
                writer.WriteAttributeString("style:parent-style-name", "Default");

                if (style.DataStyle != -1)
                    writer.WriteAttributeString("style:data-style-name", $"N{style.DataStyle + 1}");

                writer.WriteStartElement("style:table-cell-properties");

                if (style.BackColor != Color.Transparent)
                    writer.WriteAttributeString("fo:background-color", GetColorString(style.BackColor));

                else
                    writer.WriteAttributeString("fo:background-color", "transparent");

                if (style.VertAlign == StiVertAlignment.Center)
                    writer.WriteAttributeString("style:vertical-align", "middle");

                if (style.VertAlign == StiVertAlignment.Top)
                    writer.WriteAttributeString("style:vertical-align", "top");

                writer.WriteAttributeString("style:text-align-source", "fix");
                writer.WriteAttributeString("style:repeat-content", "false");

                if (style.Wordwrap)
                    writer.WriteAttributeString("fo:wrap-option", "wrap");

                writer.WriteAttributeString("fo:padding", "0in");
                writer.WriteAttributeString("fo:border-left", style.BorderLeft);
                writer.WriteAttributeString("fo:border-right", style.BorderRight);
                writer.WriteAttributeString("fo:border-top", style.BorderTop);
                writer.WriteAttributeString("fo:border-bottom", style.BorderBottom);

                if (style.Angle != 0)
                {
                    writer.WriteAttributeString("style:rotation-angle", $"{style.Angle}");
                    writer.WriteAttributeString("style:rotation-align", "none");
                }

                writer.WriteEndElement();
                writer.WriteStartElement("style:paragraph-properties");

                if (style.HorAlign == StiTextHorAlignment.Left)
                    writer.WriteAttributeString("fo:text-align", "start");

                if (style.HorAlign == StiTextHorAlignment.Center)
                    writer.WriteAttributeString("fo:text-align", "center");

                if (style.HorAlign == StiTextHorAlignment.Right)
                    writer.WriteAttributeString("fo:text-align", "end");

                if (style.HorAlign == StiTextHorAlignment.Width)
                    writer.WriteAttributeString("fo:text-align", "justify");

                writer.WriteAttributeString("fo:margin-left", "0cm");
                writer.WriteEndElement();
                writer.WriteStartElement("style:text-properties");
                writer.WriteAttributeString("fo:color", GetColorString(style.FontColor));
                writer.WriteAttributeString("style:font-name", style.FontName);

                var fontSizeSt = $"{style.FontSize}pt".Replace(",", ".");
                writer.WriteAttributeString("fo:font-size", fontSizeSt);
                writer.WriteAttributeString("fo:font-size-asian", fontSizeSt);
                writer.WriteAttributeString("fo:font-size-complex", fontSizeSt);

                if (style.Italic)
                {
                    writer.WriteAttributeString("fo:font-style", "italic");
                    writer.WriteAttributeString("fo:font-style-asian", "italic");
                    writer.WriteAttributeString("fo:font-style-complex", "italic");
                }

                if (style.Underline)
                {
                    writer.WriteAttributeString("style:text-underline-style", "solid");
                    writer.WriteAttributeString("style:text-underline-width", "auto");
                    writer.WriteAttributeString("style:text-underline-color", "font-color");
                }

                if (style.Bold)
                {
                    writer.WriteAttributeString("fo:font-weight", "bold");
                    writer.WriteAttributeString("fo:font-weight-asian", "bold");
                    writer.WriteAttributeString("fo:font-weight-complex", "bold");
                }

                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            #endregion

            writer.WriteFullEndElement(); //office:automatic-styles
            #endregion

            writer.WriteStartElement("office:body");
            writer.WriteStartElement("office:spreadsheet");

            writer.WriteStartElement("table:calculation-settings");
            writer.WriteAttributeString("table:use-regular-expressions", "false");
            writer.WriteEndElement();

            CurrentPassNumber = StiOptions.Export.OpenDocumentCalc.DivideSegmentPages ? 3 : 2;

            for (var indexSheet = 0; indexSheet < matrixList.Count; indexSheet++)
            {
                WriteTableFromMatrix(writer, indexSheet, rowHeightList, colWidthList);
            }

            writer.WriteEndElement(); //office:spreadsheet
            writer.WriteEndElement(); //office:body

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }

        private void WriteDateTimeFormatString(XmlTextWriter writer, string formatString, string defaultFormatString)
        {
            if (string.IsNullOrEmpty(formatString))
                formatString = defaultFormatString;

            //convert standard format strings to custom format strings
            if (formatString.Length == 1)
            {
                var dtfi = currentCulture.DateTimeFormat;

                switch (formatString)
                {
                    case "d":
                        formatString = dtfi.ShortDatePattern;
                        break;

                    case "D":
                        formatString = dtfi.LongDatePattern;
                        break;

                    case "f":
                        formatString = dtfi.ShortDatePattern + " " + dtfi.ShortTimePattern;
                        break;

                    case "F":
                        formatString = dtfi.FullDateTimePattern;
                        break;

                    case "g":
                        formatString = dtfi.ShortDatePattern + " " + dtfi.ShortTimePattern;
                        break;

                    case "G":
                        formatString = dtfi.ShortDatePattern + " " + dtfi.LongTimePattern;
                        break;

                    case "m":
                    case "M":
                        formatString = dtfi.MonthDayPattern;
                        break;

                    case "r":
                    case "R":
                        formatString = dtfi.RFC1123Pattern;
                        break;

                    case "s":
                        formatString = dtfi.SortableDateTimePattern;
                        break;

                    case "t":
                        formatString = dtfi.ShortTimePattern;
                        break;

                    case "T":
                        formatString = dtfi.LongTimePattern;
                        break;

                    case "u":
                        formatString = dtfi.UniversalSortableDateTimePattern;
                        break;

                    case "U":
                        formatString = dtfi.FullDateTimePattern;
                        break;

                    case "y":
                    case "Y":
                        formatString = dtfi.YearMonthPattern;
                        break;
                }

                formatString = formatString.Replace("\"", "").Replace("'", "");
            }

            //parse custom format strings
            var index = 0;
            while (index < formatString.Length)
            {
                var ch = formatString[index];
                var counter = 1;
                while (index + 1 < formatString.Length && formatString[index + 1] == ch)
                {
                    index++;
                    counter++;
                }

                index++;

                switch (ch)
                {
                    case 'd':
                        if (counter == 1)
                        {
                            writer.WriteStartElement("number:day");
                            writer.WriteEndElement();
                        }

                        if (counter == 2)
                        {
                            writer.WriteStartElement("number:day");
                            writer.WriteAttributeString("number:style", "long");
                            writer.WriteEndElement();
                        }

                        if (counter == 3)
                        {
                            writer.WriteStartElement("number:day-of-week");
                            writer.WriteEndElement();
                        }

                        if (counter == 4)
                        {
                            writer.WriteStartElement("number:day-of-week");
                            writer.WriteAttributeString("number:style", "long");
                            writer.WriteEndElement();
                        }
                        break;

                    case 'M':
                        if (counter == 1)
                        {
                            writer.WriteStartElement("number:month");
                            writer.WriteEndElement();
                        }

                        if (counter == 2)
                        {
                            writer.WriteStartElement("number:month");
                            writer.WriteAttributeString("number:style", "long");
                            writer.WriteEndElement();
                        }

                        if (counter == 3)
                        {
                            writer.WriteStartElement("number:month");
                            writer.WriteAttributeString("number:textual", "true");
                            writer.WriteEndElement();
                        }

                        if (counter == 4)
                        {
                            writer.WriteStartElement("number:month");
                            writer.WriteAttributeString("number:style", "long");
                            writer.WriteAttributeString("number:textual", "true");
                            writer.WriteEndElement();
                        }
                        break;

                    case 'y':
                        if (counter == 1 || counter == 2)
                        {
                            writer.WriteStartElement("number:year");
                            writer.WriteEndElement();
                        }

                        if (counter == 3 || counter == 4)
                        {
                            writer.WriteStartElement("number:year");
                            writer.WriteAttributeString("number:style", "long");
                            writer.WriteEndElement();
                        }
                        break;

                    case 's':
                        if (counter == 1)
                        {
                            writer.WriteStartElement("number:seconds");
                            writer.WriteEndElement();
                        }

                        if (counter == 2)
                        {
                            writer.WriteStartElement("number:seconds");
                            writer.WriteAttributeString("number:style", "long");
                            writer.WriteEndElement();
                        }
                        break;

                    case 'm':
                        if (counter == 1)
                        {
                            writer.WriteStartElement("number:minutes");
                            writer.WriteEndElement();
                        }

                        if (counter == 2)
                        {
                            writer.WriteStartElement("number:minutes");
                            writer.WriteAttributeString("number:style", "long");
                            writer.WriteEndElement();
                        }
                        break;

                    case 'h':
                    case 'H':
                        if (counter == 1)
                        {
                            writer.WriteStartElement("number:hours");
                            writer.WriteEndElement();
                        }

                        if (counter == 2)
                        {
                            writer.WriteStartElement("number:hours");
                            writer.WriteAttributeString("number:style", "long");
                            writer.WriteEndElement();
                        }
                        break;

                    default:
                        writer.WriteElementString("number:text", ch.ToString());
                        break;
                }
            }
        }

        private void WriteTableFromMatrix(XmlTextWriter writer, int indexSheet, Hashtable rowHeightList, Hashtable colWidthList)
        {
            var matrix = (StiMatrix) matrixList[indexSheet];
            var minRowIndex = (int) minRowList[indexSheet];
            var maxRowIndex = (int) maxRowList[indexSheet];
            var cellStyleTable = (int[,]) cellStyleTableList[indexSheet];
            var sheetName = (string) sheetNameList[indexSheet];

            var readyCells = new bool[matrix.CoordY.Count, matrix.CoordX.Count];
            RichTextBox richtextForConvert = null;

            writer.WriteStartElement("table:table");
            writer.WriteAttributeString("table:name", sheetName);
            writer.WriteAttributeString("table:style-name", "ta1");
            writer.WriteAttributeString("table:print", "false");

            //columns
            for (var indexColumn = 0; indexColumn < matrix.CoordX.Count - 1; indexColumn++)
            {
                var columnWidth = (double) matrix.CoordX.GetByIndex(indexColumn + 1) - (double) matrix.CoordX.GetByIndex(indexColumn);
                var columnStyle = (int) colWidthList[columnWidth];
                writer.WriteStartElement("table:table-column");
                writer.WriteAttributeString("table:style-name", $"co{columnStyle + 1}");
                writer.WriteAttributeString("table:default-cell-style-name", "Default");
                writer.WriteEndElement();
            }

            double progressScale = Math.Max(matrix.CoordY.Count / 200f, 1f);
            int progressValue = 0;

            for (var indexRow = minRowIndex + 1; indexRow < maxRowIndex + 1; indexRow++)
            {
                int currentProgress = (int)(indexRow / progressScale);
                if (currentProgress > progressValue)
                {
                    progressValue = currentProgress;
                    InvokeExporting(indexRow, matrix.CoordY.Count, CurrentPassNumber, MaximumPassNumber);
                }


                var rowHeightDouble = (double)matrix.CoordY.GetByIndex(indexRow) - (double)matrix.CoordY.GetByIndex(indexRow - 1);
                var rowStyle = (int)rowHeightList[rowHeightDouble];

                writer.WriteStartElement("table:table-row");
                writer.WriteAttributeString("table:style-name", $"ro{rowStyle + 1}");

                for (var indexColumn = 1; indexColumn < matrix.CoordX.Count; indexColumn++)
                {
                    var cell = matrix.Cells[indexRow - 1, indexColumn - 1];

                    if (!readyCells[indexRow, indexColumn])
                    {
                        if (cell != null)
                        {
                            #region Range
                            for (var yy = 0; yy <= cell.Height; yy++)
                            {
                                for (var xx = 0; xx <= cell.Width; xx++)
                                {
                                    readyCells[indexRow + yy, indexColumn + xx] = true;
                                }
                            }
                            #endregion

                            var cellStyleIndex = cellStyleTable[indexRow - 1, indexColumn - 1];
                            var dataStyleIndex = ((CellStyleData) cellStyleList[cellStyleIndex]).DataStyle;

                            writer.WriteStartElement("table:table-cell");
                            writer.WriteAttributeString("table:style-name", $"ce{cellStyleIndex + 1}");

                            if (cell.Width > 0 || cell.Height > 0)
                            {
                                writer.WriteAttributeString("table:number-columns-spanned", $"{cell.Width + 1}"); //merged
                                writer.WriteAttributeString("table:number-rows-spanned", $"{cell.Height + 1}"); //merged
                            }

                            var rtf = cell.Component as StiRichText;
                            var textComp = cell.Component as StiText;

                            var str = cell.Text;
                            if (rtf != null && rtf.RtfText != string.Empty)
                            {
                                if (richtextForConvert == null) richtextForConvert = new Controls.StiRichTextBox(false);
                                rtf.GetPreparedText(richtextForConvert);
                                str = richtextForConvert.Text;
                            }

                            var checkComp = cell.Component as StiCheckBox;
                            if (checkComp != null && !string.IsNullOrEmpty(checkComp.ExcelDataValue))
                                str = checkComp.ExcelDataValue;

                            if (!cell.Component.IsExportAsImage(StiExportFormat.Ods) &&
                                (!string.IsNullOrEmpty(str) || textComp != null && textComp.ExcelDataValue != null))
                            {
                                #region Text
                                var isNumber = false;
                                double Number = 0;
                                var dt = new DateTime();
                                DataStyleData dataStyle = null;

                                if (dataStyleIndex != -1 &&
                                    textComp != null &&
                                    textComp.ExcelDataValue != null &&
                                    textComp.ExcelDataValue != "-")
                                {
                                    var value = textComp.ExcelDataValue;
                                    var sep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                                    var value2 = value.Replace(".", ",").Replace(",", sep);
                                    dataStyle = (DataStyleData) dataStyleList[dataStyleIndex];

                                    isNumber = true;
                                    try
                                    {
                                        if (dataStyle.isDate || dataStyle.isTime)
                                        {
                                            var result = DateTime.TryParse(value, currentCulture, DateTimeStyles.None, out dt);
                                            if (!result) isNumber = false;
                                        }
                                        else
                                        {
                                            //Number = double.Parse(value2, NumberStyles.Any);
                                            if (!double.TryParse(value2, NumberStyles.Any, CultureInfo.CurrentCulture, out Number))
                                                isNumber = false;
                                        }
                                    }
                                    catch (FormatException)
                                    {
                                        isNumber = false;
                                    }

                                    if (!isNumber && str == null)
                                        str = value;
                                }

                                if (isNumber)
                                {
                                    if (dataStyle.isDate)
                                    {
                                        writer.WriteAttributeString("office:value-type", "date");
                                        writer.WriteAttributeString("office:date-value", dt.ToString("yyyy-MM-dd"));
                                    }
                                    else if (dataStyle.isTime)
                                    {
                                        writer.WriteAttributeString("office:value-type", "time");
                                        writer.WriteAttributeString("office:time-value", dt.ToString(@"PTHH\Hmm\Mss\S"));
                                    }
                                    else
                                    {
                                        var value = Number.ToString().Replace(',', '.');

                                        if (dataStyle.isNumeric)
                                            writer.WriteAttributeString("office:value-type", "float");

                                        if (dataStyle.isPercent)
                                            writer.WriteAttributeString("office:value-type", "percentage");

                                        if (dataStyle.isCurrency)
                                            writer.WriteAttributeString("office:value-type", "currency");

                                        writer.WriteAttributeString("office:value", value);
                                    }
                                }
                                else
                                {
                                    writer.WriteAttributeString("office:value-type", "string");
                                }

                                var stringList = StiExportUtils.SplitString(str, true);
                                for (var indexLine = 0; indexLine < stringList.Count; indexLine++)
                                {
                                    var textLine = stringList[indexLine];

                                    writer.WriteStartElement("text:p");
                                    writer.WriteString(textLine);
                                    writer.WriteEndElement();
                                }
                                #endregion
                            }

                            if (cell.Component.IsExportAsImage(StiExportFormat.Ods))
                            {
                                #region Image
                                writer.WriteAttributeString("office:value-type", "string");

                                var exportImageExtended = cell.Component as IStiExportImageExtended;
                                if (exportImageExtended != null)
                                {
                                    var rsImageResolution = imageResolution;
                                    using (var image = exportImageExtended.GetImage(ref rsImageResolution, imageFormat == null || imageFormat == ImageFormat.Png ? StiExportFormat.ImagePng : StiExportFormat.Excel))
                                    {
                                        if (image != null)
                                        {
                                            var img = matrix.GetRealImageData(cell, image) ?? image;

                                            var indexImage = imageCache.AddImageInt(img);

                                            var imageWidth = (double) matrix.CoordX.GetByIndex(indexColumn + cell.Width) - (double) matrix.CoordX.GetByIndex(indexColumn - 1);
                                            var imageHeight = (double) matrix.CoordY.GetByIndex(indexRow + cell.Height) - (double) matrix.CoordY.GetByIndex(indexRow - 1);
                                            var endCellAddress = $"'{sheetName}'.{GetColumnName(cell.Left + cell.Width + 1)}{cell.Top + cell.Height + 1 + 1}";

                                            #region Write image info
                                            writer.WriteStartElement("draw:frame");
                                            writer.WriteAttributeString("table:end-cell-address", endCellAddress);
                                            writer.WriteAttributeString("table:end-x", "0in");
                                            writer.WriteAttributeString("table:end-y", "0in");
                                            writer.WriteAttributeString("draw:z-index", "0");
                                            writer.WriteAttributeString("draw:name", $"Picture{indexImage + 1}");
                                            writer.WriteAttributeString("draw:style-name", "gr1");
                                            writer.WriteAttributeString("draw:text-style-name", "P1");
                                            writer.WriteAttributeString("text:anchor-type", "paragraph");
                                            writer.WriteAttributeString("svg:x", DoubleToString(0));
                                            writer.WriteAttributeString("svg:y", DoubleToString(0));
                                            writer.WriteAttributeString("svg:width", DoubleToString(imageWidth));
                                            writer.WriteAttributeString("svg:height", DoubleToString(imageHeight));

                                            writer.WriteStartElement("draw:image");
                                            writer.WriteAttributeString("xlink:href", $"Pictures/{indexImage + 1:D5}.{GetImageFormatExtension(imageCache.ImageFormatStore[indexImage])}");
                                            writer.WriteAttributeString("xlink:type", "simple");
                                            writer.WriteAttributeString("xlink:show", "embed");
                                            writer.WriteAttributeString("xlink:actuate", "onLoad");
                                            writer.WriteStartElement("text:p");
                                            writer.WriteEndElement();
                                            writer.WriteEndElement();

                                            writer.WriteEndElement(); //draw:frame
                                            #endregion
                                        }
                                    }
                                }
                                #endregion
                            }

                            writer.WriteEndElement();
                        }
                        else
                        {
                            var cellStyleIndex = cellStyleTable[indexRow - 1, indexColumn - 1];
                            writer.WriteStartElement("table:table-cell");
                            writer.WriteAttributeString("table:style-name", $"ce{cellStyleIndex + 1}");
                            writer.WriteEndElement();
                        }
                    }
                    else
                    {
                        writer.WriteStartElement("table:covered-table-cell"); //merged
                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement(); //table-row
            }

            writer.WriteEndElement(); //table:table
            if (richtextForConvert != null)
                richtextForConvert.Dispose();
        }

        /// <summary>
        /// Exports rendered report to an Ods file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        public void ExportOds(StiReport report, string fileName)
        {
            FileStream stream = null;
            try
            {
                StiFileUtils.ProcessReadOnly(fileName);
                stream = new FileStream(fileName, FileMode.Create);
                ExportOds(report, stream);
            }
            finally
            {
                stream.Flush();
                stream.Close();
            }
        }

        /// <summary>
        /// Exports rendered report to an Ods file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        public void ExportOds(StiReport report, Stream stream)
        {
            ExportOds(report, stream, new StiOdsExportSettings());
        }

        /// <summary>
        /// Exports rendered report to an Ods file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        public void ExportOds(StiReport report, Stream stream, StiPagesRange pageRange)
        {
            ExportOds(report, stream, new StiOdsExportSettings
            {
                PageRange = pageRange
            });
        }

        public void ExportOds(StiReport report, Stream stream, StiOdsExportSettings settings)
        {
            StiLogService.Write(this.GetType(), "Export report to Ods format");

#if NETSTANDARD || NETCOREAPP
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif

            #region Read settings
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var pageRange = settings.PageRange;
            imageResolution = settings.ImageResolution;
            imageQuality = settings.ImageQuality;
            this.imageFormat = settings.ImageFormat;
            #endregion

            xmlIndentation = 1;

            if (imageQuality < 0)
                imageQuality = 0;

            if (imageQuality > 1)
                imageQuality = 1;

            if (imageResolution < 10)
                imageResolution = 10;

            imageResolution = imageResolution / 100;
            if (imageFormat != null && imageFormat != ImageFormat.Png) imageFormat = ImageFormat.Jpeg;

            currentCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                imageCache = new StiImageCache(StiOptions.Export.OpenDocumentCalc.AllowImageComparer, imageFormat, imageQuality);
                cellStyleList = new ArrayList();
                dataStyleList = new ArrayList();
                sheetNameList = new ArrayList();
                matrixList = new ArrayList();
                firstPageIndexList = new ArrayList();
                minRowList = new ArrayList();
                maxRowList = new ArrayList();
                cellStyleTableList = new ArrayList();

                CurrentPassNumber = 0;
                MaximumPassNumber = StiOptions.Export.OpenDocumentCalc.DivideSegmentPages ? 4 : 3;

                var pages = pageRange.GetSelectedPages(report.RenderedPages);
                if (StiOptions.Export.OpenDocumentCalc.DivideSegmentPages)
                {
                    pages = StiSegmentPagesDivider.Divide(pages, this);
                    CurrentPassNumber = 1;
                }

                if (IsStopped) return;

                StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");
                var zip = new StiZipWriter20();
                zip.Begin(stream, true);
                zip.AddFile("content.xml", WriteContent(report, pages));
                StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument") + " 2";
                zip.AddFile("mimetype", WriteMimetype());
                zip.AddFile("meta.xml", WriteMeta());
                zip.AddFile("META-INF/manifest.xml", WriteManifest());
                zip.AddFile("settings.xml", WriteSettings());
                zip.AddFile("styles.xml", WriteStyles(pages));

                if (imageCache.ImagePackedStore.Count > 0)
                {
                    for (var index = 0; index < imageCache.ImagePackedStore.Count; index++)
                    {
                        zip.AddFile($"Pictures/{index + 1:D5}.{GetImageFormatExtension(imageCache.ImageFormatStore[index])}", WriteImage(index));
                    }
                }

                zip.End();
            }
            finally
            {
                StiExportUtils.EnableFontSmoothing(report);
                Thread.CurrentThread.CurrentCulture = currentCulture;
                sheetNameList.Clear();
                sheetNameList = null;
                firstPageIndexList.Clear();
                firstPageIndexList = null;
                minRowList.Clear();
                minRowList = null;
                maxRowList.Clear();
                maxRowList = null;

                foreach (StiMatrix matrix1 in matrixList)
                {
                    if (matrix1 != null)
                        matrix1.Clear();
                }

                matrixList.Clear();
                matrixList = null;
                cellStyleTableList.Clear();
                cellStyleTableList = null;
                dataStyleList.Clear();
                dataStyleList = null;
                cellStyleList.Clear();
                cellStyleList = null;
                imageCache.Clear();
                imageCache = null;

                if (report.RenderedPages.CacheMode)
                    StiMatrix.GCCollect();
            }
        }
        #endregion
    }
}