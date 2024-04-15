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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Zip;
using Stimulsoft.Report.Components;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Drawing;
using System.Drawing.Imaging;
using Stimulsoft.Report.Helpers;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
#endif

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// A class for the Odt Export.
    /// </summary>
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourceOdt.png")]
	public class StiOdtExportService : StiExportService
    {
        #region StiExportService override
        /// <summary>
		/// Gets or sets a default extension of export. 
		/// </summary>
		public override string DefaultExtension
		{
			get
			{
				return "odt";
			}
		}

		public override StiExportFormat ExportFormat
		{
			get
			{
				return StiExportFormat.Odt;
			}
		}


		/// <summary>
		/// Gets a group of the export in the context menu.
		/// </summary>
		public override string GroupCategory
		{
			get
			{
				return "Word";
			}
		}


		/// <summary>
		/// Gets a position of the export in the context menu.
		/// </summary>
		public override int Position
		{
			get
			{
				return (int)StiExportPosition.Odt;
			}
		}


		/// <summary>
		/// Gets an export name in the context menu.
		/// </summary>
		public override string ExportNameInMenu
		{
			get
			{
				return StiLocalization.Get("Export", "ExportTypeWriterFile");
			}
		}

        /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public override void ExportTo(StiReport report, Stream stream, StiExportSettings settings)
        {
            ExportOdt(report, stream, settings as StiOdtExportSettings);
			InvokeExporting(100, 100, 1, 1);
		}

	    /// <summary>
		/// Exports a rendered report to the Odt file.
		/// Also exported document can be sent via e-mail.
		/// </summary>
		/// <param name="report">A report which is to be exported.</param>
		/// <param name="fileName">A name of the file for exporting a rendered report.</param>
		/// <param name="sendEMail">A parameter indicating whether the exported report will be sent via e-mail.</param>
        public override void Export(StiReport report, string fileName, bool sendEMail, StiGuiMode guiMode)
		{
            using (IStiFormRunner form = StiGuiOptions.GetExportFormRunner("StiOdtExportSetupForm", guiMode, this.OwnerWindow))
            {
                form["CurrentPage"] = report.CurrentPrintPage;
                form["OpenAfterExportEnabled"] = !sendEMail;

                this.report = report;
                this.fileName = fileName;
                this.sendEMail = sendEMail;
                this.guiMode = guiMode;
                form.Complete += form_Complete;
                form.ShowDialog();
			}
		}

        private StiReport report;
        private string fileName;
        private bool sendEMail;
        private StiGuiMode guiMode;
        private void form_Complete(IStiFormRunner form, StiShowDialogCompleteEvetArgs e)
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

							var settings = new StiOdtExportSettings
							{
								PageRange = form["PagesRange"] as StiPagesRange,
								UsePageHeadersAndFooters = (bool)form["UsePageHeadersAndFooters"],
								ImageQuality = (float)form["ImageQuality"],
								ImageResolution = (float)form["Resolution"],
								RemoveEmptySpaceAtBottom = (bool)form["RemoveEmptySpaceAtBottom"]
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

		/// <summary>
		/// Gets a value indicating a number of files in exported document as a result of export
		/// of one page of the rendered report.
		/// </summary>
		public override bool MultipleFiles
		{
			get
			{
				return false;
			}
		}


		/// <summary>
		/// Returns a filter for the Odt files.
		/// </summary>
		/// <returns>Returns a filter for the Odt files.</returns>
		public override string GetFilter()
		{
			return StiLocalization.Get("FileFilters", "WriterFiles");
		}	

		#endregion

		#region this
		private StiMatrix matrix = null;
		internal StiMatrix Matrix
		{
			get
			{
				return matrix;
			}
		}

        private bool removeEmptySpaceAtBottom = StiOptions.Export.OpenDocumentWriter.RemoveEmptySpaceAtBottom;
        internal bool RemoveEmptySpaceAtBottom
        {
            get
            {
                return removeEmptySpaceAtBottom;
            }
        }

		private StiImageCache imageCache = null;
		private ArrayList cellStyleList = null;
		private ArrayList paragraphStyleList = null;

		private int xmlIndentation = 1;

		private float imageQuality = 0.75f;
		private float imageResolution = 96; //dpi
        private ImageFormat imageFormat = ImageFormat.Jpeg;
        private bool usePageHeadersAndFooters = false;

        private string GetImageFormatExtension(ImageFormat currImageFormat)
        {
            if (currImageFormat == ImageFormat.Png) return "png";
            return "jpeg";
        }

        #region class CellStyleData
        private class CellStyleData
		{
			public string BorderLeft;
			public string BorderRight;
			public string BorderTop;
			public string BorderBottom;
			public Color BackColor;
			public StiVertAlignment VertAlign;

			public CellStyleData()
			{
				BorderLeft = "none";
				BorderRight = "none";
				BorderTop = "none";
				BorderBottom = "none";
				BackColor = Color.Transparent;
				VertAlign = StiVertAlignment.Bottom;
			}
		}
		#endregion

		#region class ParagraphStyleData
		private class ParagraphStyleData
		{
			public string FontName;
			public float FontSize;
			public bool Bold;
			public bool Italic;
			public bool Underline;
			public Color FontColor;
			public StiTextHorAlignment HorAlign;
			public int Angle;

			public ParagraphStyleData()
			{
				FontName = "Arial";
				FontSize = 6;
				Bold = false;
				Italic = false;
				Underline = false;
				FontColor = Color.Black;
				HorAlign = StiTextHorAlignment.Left;
				Angle = 0;
			}
		}
		#endregion

		#region DoubleToString
		/// <summary>
		/// Convert value from hinch double to inch string
		/// </summary>
		/// <param name="number">input value in hinch, double</param>
		/// <returns>output value in inch, string</returns>
		private static string DoubleToString(double number)
		{
			//return Math.Round(number / 100d * 2.54d, 4).ToString().Replace(",", ".") + "cm";
			return Math.Round(number / 100d, 4).ToString().Replace(",", ".") + "in";
		}
		#endregion

		#region GetColumnName
		private static string GetColumnName(int column)
		{
			int columnHigh = column / 26;
			int columnLow = column % 26;
			StringBuilder output = new StringBuilder();
			if (columnHigh > 0) 
			{
				output.Append((char)((byte)'A' + columnHigh - 1));
			}
			output.Append((char)((byte)'A' + columnLow));
			return output.ToString();
		}
		#endregion

		#region GetColorString
		private static string GetColorString(Color color)
		{
			return "#" + color.ToArgb().ToString("X8").Substring(2);
		}
		#endregion

		#region GetCellStyleNumber
		private int GetCellStyleNumber(int indexRow, int indexColumn, int height, int width)
		{
			CellStyleData style = new CellStyleData();

			bool needBorderLeft = true;
			bool needBorderRight = true;
			for (int index = 0; index < height; index ++)
			{
				if (matrix.BordersY[indexRow + index, indexColumn] == null) needBorderLeft = false;
				if (matrix.BordersY[indexRow + index, indexColumn + width] == null) needBorderRight = false;
			}
			bool needBorderTop = true;
			bool needBorderBottom = true;
			for (int index = 0; index < width; index ++)
			{
				if (matrix.BordersX[indexRow, indexColumn + index] == null) needBorderTop = false;
				if (matrix.BordersX[indexRow + height, indexColumn + index] == null) needBorderBottom = false;
			}
			if (needBorderTop) style.BorderTop = GetStringFromBorder(Matrix.BordersX[indexRow, indexColumn]);
			if (needBorderLeft) style.BorderLeft = GetStringFromBorder(Matrix.BordersY[indexRow, indexColumn]);
			if (needBorderBottom) style.BorderBottom = GetStringFromBorder(Matrix.BordersX[indexRow + height, indexColumn]);
			if (needBorderRight) style.BorderRight = GetStringFromBorder(Matrix.BordersY[indexRow, indexColumn + width]);

			if (Matrix.Cells[indexRow, indexColumn] != null)
			{
				StiCell cell = Matrix.Cells[indexRow, indexColumn];
				style.BackColor = cell.CellStyle.Color;
				style.VertAlign = cell.CellStyle.VertAlignment;

                //rotate text alignment
                if (cell.CellStyle.TextOptions != null)
                {
                    var textAngle = cell.CellStyle.TextOptions.Angle;
                    if ((textAngle > 45) && (textAngle < 135) || (textAngle > 225) && (textAngle < 315))
                    {
                        if (cell.CellStyle.HorAlignment == StiTextHorAlignment.Left) style.VertAlign = StiVertAlignment.Bottom;
                        if (cell.CellStyle.HorAlignment == StiTextHorAlignment.Center) style.VertAlign = StiVertAlignment.Center;
                        if (cell.CellStyle.HorAlignment == StiTextHorAlignment.Right) style.VertAlign = StiVertAlignment.Top;
                        if (cell.CellStyle.HorAlignment == StiTextHorAlignment.Width) style.VertAlign = StiVertAlignment.Bottom;
                    }
                }
			}

			if (cellStyleList.Count > 0)
			{
				for (int index = 0; index < cellStyleList.Count; index++)
				{
					CellStyleData tempStyle = (CellStyleData)cellStyleList[index];
					if ((tempStyle.BorderLeft == style.BorderLeft) &&
						(tempStyle.BorderRight == style.BorderRight) &&
						(tempStyle.BorderTop == style.BorderTop) &&
						(tempStyle.BorderBottom == style.BorderBottom) &&
						(tempStyle.BackColor == style.BackColor) &&
						(tempStyle.VertAlign == style.VertAlign))
					{
						//is already in table, return number 
						return index;
					}
				}
			}
			//add to table, return number 
			cellStyleList.Add(style);
			int temp = cellStyleList.Count - 1;
			return temp;
		}
		private static string GetStringFromBorder(StiBorderSide border)
		{
			return string.Format("{0} solid {1}", DoubleToString(border.Size), GetColorString(border.Color));
		}
		#endregion

		#region GetParagraphStyleNumber
		private int GetParagraphStyleNumber(int indexRow, int indexColumn)
		{
			StiCellStyle cellStyle = Matrix.Cells[indexRow, indexColumn].CellStyle;
			ParagraphStyleData style = new ParagraphStyleData();

			style.FontName = cellStyle.Font.Name;
			style.FontSize = cellStyle.Font.SizeInPoints;
			style.Bold = cellStyle.Font.Bold;
			style.Italic = cellStyle.Font.Italic;
			style.Underline = cellStyle.Font.Underline;
			style.FontColor = cellStyle.TextColor;
			style.HorAlign = cellStyle.HorAlignment;
			float textAngle = 0;
			if (cellStyle.TextOptions != null) textAngle = cellStyle.TextOptions.Angle;
			if ((textAngle > 45) && (textAngle < 135)) style.Angle = 90;
			if ((textAngle > 225) && (textAngle < 315)) style.Angle = 270;

            //rotate text alignment
            if (style.Angle != 0)
            {
                if (cellStyle.VertAlignment == StiVertAlignment.Top) style.HorAlign = StiTextHorAlignment.Left;
                if (cellStyle.VertAlignment == StiVertAlignment.Center) style.HorAlign = StiTextHorAlignment.Center;
                if (cellStyle.VertAlignment == StiVertAlignment.Bottom) style.HorAlign = StiTextHorAlignment.Right;
            }

			if (paragraphStyleList.Count > 0)
			{
				for (int index = 0; index < paragraphStyleList.Count; index++)
				{
					ParagraphStyleData tempStyle = (ParagraphStyleData)paragraphStyleList[index];
					if ((tempStyle.FontName == style.FontName) &&
						(tempStyle.FontSize == style.FontSize) &&
						(tempStyle.Bold == style.Bold) &&
						(tempStyle.Italic == style.Italic) &&
						(tempStyle.Underline == style.Underline) &&
						(tempStyle.FontColor == style.FontColor) &&
						(tempStyle.HorAlign == style.HorAlign) &&
						(tempStyle.Angle == style.Angle))
					{
						//is already in table, return number 
						return index;
					}
				}
			}
			//add to table, return number 
			paragraphStyleList.Add(style);
			int temp = paragraphStyleList.Count - 1;
			return temp;
		}
		#endregion
        
		#region WriteMimetype
		private MemoryStream WriteMimetype()
		{
			MemoryStream ms = new MemoryStream();
			StreamWriter writer = new StreamWriter(ms, Encoding.ASCII);
			writer.Write("application/vnd.oasis.opendocument.text");
			writer.Flush();
			return ms;
		}
		#endregion

		#region WriteMeta
		private MemoryStream WriteMeta()
		{
			MemoryStream ms = new MemoryStream();
			XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
			writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
			writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
			writer.WriteStartDocument();

			writer.WriteStartElement("office:document-meta");
			writer.WriteAttributeString("xmlns:office", "urn:oasis:names:tc:opendocument:xmlns:office:1.0");
			writer.WriteAttributeString("xmlns:xlink", "http://www.w3.org/1999/xlink");
			writer.WriteAttributeString("xmlns:dc", "http://purl.org/dc/elements/1.1/");
			writer.WriteAttributeString("xmlns:meta", "urn:oasis:names:tc:opendocument:xmlns:meta:1.0");
			writer.WriteAttributeString("xmlns:ooo", "http://openoffice.org/2004/office");
			writer.WriteAttributeString("office:version", "1.1");
			writer.WriteStartElement("office:meta");

			string dateTime = string.Format("{0}", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));

			writer.WriteElementString("meta:generator", "Stimulsoft Reports");
			writer.WriteElementString("meta:creation-date", dateTime);
			writer.WriteElementString("dc:date", dateTime);
			writer.WriteElementString("meta:editing-cycles", "1");
			writer.WriteElementString("meta:editing-duration", "PT0M01S");
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
			writer.WriteAttributeString("meta:image-count", "0");
			writer.WriteAttributeString("meta:object-count", "0");
			writer.WriteAttributeString("meta:page-count", "1");
			writer.WriteAttributeString("meta:paragraph-count", "1");
			writer.WriteAttributeString("meta:word-count", "1");
			writer.WriteAttributeString("meta:character-count", "1");
			writer.WriteEndElement();

			writer.WriteFullEndElement();
			writer.WriteFullEndElement();
			writer.WriteEndDocument();
			writer.Flush();
			return ms;
		}
		#endregion

		#region WriteManifest
		private MemoryStream WriteManifest()
		{
			MemoryStream ms = new MemoryStream();
			XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
			writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
			writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
			writer.WriteStartDocument();
			writer.WriteStartElement("manifest:manifest");
			writer.WriteAttributeString("xmlns:manifest", "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0");

			writer.WriteStartElement("manifest:file-entry");
			writer.WriteAttributeString("manifest:media-type", "application/vnd.oasis.opendocument.text");
			writer.WriteAttributeString("manifest:full-path", "/");
			writer.WriteEndElement();
			for (int index = 0; index < imageCache.ImagePackedStore.Count; index ++)
			{
                string ext = GetImageFormatExtension(imageCache.ImageFormatStore[index]);
                writer.WriteStartElement("manifest:file-entry");
				writer.WriteAttributeString("manifest:media-type", $"image/{ext}");
				writer.WriteAttributeString("manifest:full-path", string.Format("Pictures/{0:D5}.{1}", index + 1, ext));
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
		#endregion

		#region WriteImage
		private MemoryStream WriteImage(int number)
		{
			MemoryStream ms = new MemoryStream();
			byte[] buf = (byte[])imageCache.ImagePackedStore[number];
			ms.Write(buf, 0, buf.Length);
			return ms;

		}
		#endregion

		#region WriteSettings
		private MemoryStream WriteSettings()
		{
			MemoryStream ms = new MemoryStream();
			XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
			writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
			writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
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
			writer.WriteAttributeString("config:name", "ViewAreaTop");
			writer.WriteAttributeString("config:type", "int");
			writer.WriteString("0");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "ViewAreaLeft");
			writer.WriteAttributeString("config:type", "int");
			writer.WriteString("-10107");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "ViewAreaWidth");
			writer.WriteAttributeString("config:type", "int");
			writer.WriteString("43208");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "ViewAreaHeight");
			writer.WriteAttributeString("config:type", "int");
			writer.WriteString("22174");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "ShowRedlineChanges");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("true");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "InBrowseMode");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			//config:config-item-map-indexed
			writer.WriteStartElement("config:config-item-map-indexed");
			writer.WriteAttributeString("config:name", "Views");
			writer.WriteStartElement("config:config-item-map-entry");
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "ViewId");
			writer.WriteAttributeString("config:type", "string");
			writer.WriteString("view2");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "ViewLeft");
			writer.WriteAttributeString("config:type", "int");
			writer.WriteString("3002");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "ViewTop");
			writer.WriteAttributeString("config:type", "int");
			writer.WriteString("10435");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "VisibleLeft");
			writer.WriteAttributeString("config:type", "int");
			writer.WriteString("-10107");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "VisibleTop");
			writer.WriteAttributeString("config:type", "int");
			writer.WriteString("0");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "VisibleRight");
			writer.WriteAttributeString("config:type", "int");
			writer.WriteString("33099");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "VisibleBottom");
			writer.WriteAttributeString("config:type", "int");
			writer.WriteString("22172");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "ZoomType");
			writer.WriteAttributeString("config:type", "short");
			writer.WriteString("0");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "ZoomFactor");
			writer.WriteAttributeString("config:type", "short");
			writer.WriteString("100");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "IsSelectedFrame");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteEndElement();	//config:config-item-map-indexed

			writer.WriteEndElement();	//config:config-item-set

			writer.WriteStartElement("config:config-item-set");
			writer.WriteAttributeString("config:name", "ooo:configuration-settings");

			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "AddParaTableSpacing");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("true");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "PrintReversed");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "OutlineLevelYieldsNumbering");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "LinkUpdateMode");
			writer.WriteAttributeString("config:type", "short");
			writer.WriteString("1");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "PrintEmptyPages");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("true");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "IgnoreFirstLineIndentInNumbering");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "CharacterCompressionType");
			writer.WriteAttributeString("config:type", "short");
			writer.WriteString("0");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "PrintSingleJobs");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "UpdateFromTemplate");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("true");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "PrintPaperFromSetup");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "AddFrameOffsets");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "PrintLeftPages");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("true");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "RedlineProtectionKey");
			writer.WriteAttributeString("config:type", "base64Binary");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "PrintTables");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("true");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "ChartAutoUpdate");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("true");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "PrintControls");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("true");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "PrinterSetup");
			writer.WriteAttributeString("config:type", "base64Binary");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "IgnoreTabsAndBlanksForLineCalculation");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "PrintAnnotationMode");
			writer.WriteAttributeString("config:type", "short");
			writer.WriteString("0");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "LoadReadonly");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "AddParaSpacingToTableCells");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("true");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "AddExternalLeading");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("true");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "ApplyUserData");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("true");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "FieldAutoUpdate");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("true");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "SaveVersionOnClose");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "SaveGlobalDocumentLinks");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "IsKernAsianPunctuation");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "AlignTabStopPosition");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("true");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "ClipAsCharacterAnchoredWriterFlyFrames");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "CurrentDatabaseDataSource");
			writer.WriteAttributeString("config:type", "string");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "DoNotCaptureDrawObjsOnPage");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "TableRowKeep");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "PrinterName");
			writer.WriteAttributeString("config:type", "string");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "PrintFaxName");
			writer.WriteAttributeString("config:type", "string");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "ConsiderTextWrapOnObjPos");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "UseOldPrinterMetrics");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "PrintRightPages");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("true");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "IsLabelDocument");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "UseFormerLineSpacing");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "AddParaTableSpacingAtStart");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("true");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "UseFormerTextWrapping");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "DoNotResetParaAttrsForNumFont");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "PrintProspect");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "PrintGraphics");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("true");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "AllowPrintJobCancel");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("true");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "CurrentDatabaseCommandType");
			writer.WriteAttributeString("config:type", "int");
			writer.WriteString("0");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "DoNotJustifyLinesWithManualBreak");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "UseFormerObjectPositioning");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "PrinterIndependentLayout");
			writer.WriteAttributeString("config:type", "string");
			writer.WriteString("high-resolution");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "UseOldNumbering");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "PrintPageBackground");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("true");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "CurrentDatabaseCommand");
			writer.WriteAttributeString("config:type", "string");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "PrintDrawings");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("true");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "PrintBlackFonts");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();
			writer.WriteStartElement("config:config-item");
			writer.WriteAttributeString("config:name", "UnxForceZeroExtLeading");
			writer.WriteAttributeString("config:type", "boolean");
			writer.WriteString("false");
			writer.WriteEndElement();

			writer.WriteEndElement();	//config:config-item-set

			writer.WriteFullEndElement();
			writer.WriteFullEndElement();
			writer.WriteEndDocument();
			writer.Flush();
			return ms;
		}
		#endregion

		#region WriteStyles
		private MemoryStream WriteStyles(StiPagesCollection pages)
		{
			MemoryStream ms = new MemoryStream();
			XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
			writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
			writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
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

			//style:family graphic
			writer.WriteStartElement("style:default-style");
			writer.WriteAttributeString("style:family", "graphic");
			writer.WriteStartElement("style:graphic-properties");
			writer.WriteAttributeString("draw:shadow-offset-x", "0.3cm");
			writer.WriteAttributeString("draw:shadow-offset-y", "0.3cm");
			writer.WriteAttributeString("draw:start-line-spacing-horizontal", "0.283cm");
			writer.WriteAttributeString("draw:start-line-spacing-vertical", "0.283cm");
			writer.WriteAttributeString("draw:end-line-spacing-horizontal", "0.283cm");
			writer.WriteAttributeString("draw:end-line-spacing-vertical", "0.283cm");
			writer.WriteAttributeString("style:flow-with-text", "false");
			writer.WriteEndElement();
			writer.WriteStartElement("style:paragraph-properties");
			writer.WriteAttributeString("style:text-autospace", "ideograph-alpha");
			writer.WriteAttributeString("style:line-break", "strict");
			writer.WriteAttributeString("style:writing-mode", "lr-tb");
			writer.WriteAttributeString("style:font-independent-line-spacing", "false");
			writer.WriteStartElement("style:tab-stops");
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteStartElement("style:text-properties");
			writer.WriteAttributeString("style:use-window-font-color", "true");
			writer.WriteAttributeString("fo:font-size", "10pt");
            //writer.WriteAttributeString("fo:language", "ru");
            //writer.WriteAttributeString("fo:country", "RU");
			writer.WriteAttributeString("style:letter-kerning", "true");
			writer.WriteAttributeString("style:font-size-asian", "12pt");
			writer.WriteAttributeString("style:language-asian", "zxx");
			writer.WriteAttributeString("style:country-asian", "none");
			writer.WriteAttributeString("style:font-size-complex", "12pt");
			writer.WriteAttributeString("style:language-complex", "zxx");
			writer.WriteAttributeString("style:country-complex", "none");
			writer.WriteEndElement();
			writer.WriteEndElement();	//style:default-style

			//style:family paragraph
			writer.WriteStartElement("style:default-style");
			writer.WriteAttributeString("style:family", "paragraph");
			writer.WriteStartElement("style:paragraph-properties");
			writer.WriteAttributeString("fo:hyphenation-ladder-count", "no-limit");
			writer.WriteAttributeString("style:text-autospace", "ideograph-alpha");
			writer.WriteAttributeString("style:punctuation-wrap", "hanging");
			writer.WriteAttributeString("style:line-break", "strict");
			writer.WriteAttributeString("style:tab-stop-distance", "1.251cm");
			writer.WriteAttributeString("style:writing-mode", "page");
			writer.WriteEndElement();
			writer.WriteStartElement("style:text-properties");
			writer.WriteAttributeString("style:use-window-font-color", "true");
			writer.WriteAttributeString("style:font-name", "Arial");
			writer.WriteAttributeString("fo:font-size", "10pt");
            //writer.WriteAttributeString("fo:language", "ru");
            //writer.WriteAttributeString("fo:country", "RU");
			writer.WriteAttributeString("style:letter-kerning", "true");
			writer.WriteAttributeString("style:font-name-asian", "Lucida Sans Unicode");
			writer.WriteAttributeString("style:font-size-asian", "12pt");
			writer.WriteAttributeString("style:language-asian", "zxx");
			writer.WriteAttributeString("style:country-asian", "none");
			writer.WriteAttributeString("style:font-name-complex", "Tahoma");
			writer.WriteAttributeString("style:font-size-complex", "12pt");
			writer.WriteAttributeString("style:language-complex", "zxx");
			writer.WriteAttributeString("style:country-complex", "none");
			writer.WriteAttributeString("fo:hyphenate", "false");
			writer.WriteAttributeString("fo:hyphenation-remain-char-count", "2");
			writer.WriteAttributeString("fo:hyphenation-push-char-count", "2");
			writer.WriteEndElement();
			writer.WriteEndElement();	//style:default-style

			//style:family table
			writer.WriteStartElement("style:default-style");
			writer.WriteAttributeString("style:family", "table");
			writer.WriteStartElement("style:table-properties");
			writer.WriteAttributeString("table:border-model", "collapsing");
			writer.WriteEndElement();
			writer.WriteEndElement();	//style:default-style

			//style:family table-row
			writer.WriteStartElement("style:default-style");
			writer.WriteAttributeString("style:family", "table-row");
			writer.WriteStartElement("style:table-row-properties");
			writer.WriteAttributeString("fo:keep-together", "auto");
			writer.WriteEndElement();
			writer.WriteEndElement();	//style:default-style

			//style:name Standard
			writer.WriteStartElement("style:style");
			writer.WriteAttributeString("style:name", "Standard");
			writer.WriteAttributeString("style:family", "paragraph");
			writer.WriteAttributeString("style:class", "text");
			writer.WriteEndElement();	//style:style

			//style:name Title
			writer.WriteStartElement("style:style");
			writer.WriteAttributeString("style:name", "Title");
			writer.WriteAttributeString("style:family", "paragraph");
			writer.WriteAttributeString("style:parent-style-name", "Standard");
			writer.WriteAttributeString("style:next-style-name", "Text_20_body");
			writer.WriteAttributeString("style:class", "text");
			writer.WriteStartElement("style:paragraph-properties");
			writer.WriteAttributeString("fo:margin-top", "0.423cm");
			writer.WriteAttributeString("fo:margin-bottom", "0.212cm");
			writer.WriteAttributeString("fo:keep-with-next", "always");
			writer.WriteEndElement();
			writer.WriteStartElement("style:text-properties");
			writer.WriteAttributeString("style:font-name", "Arial1");
			writer.WriteAttributeString("fo:font-size", "14pt");
			writer.WriteAttributeString("style:font-name-asian", "Lucida Sans Unicode");
			writer.WriteAttributeString("style:font-size-asian", "14pt");
			writer.WriteAttributeString("style:font-name-complex", "Tahoma");
			writer.WriteAttributeString("style:font-size-complex", "14pt");
			writer.WriteEndElement();
			writer.WriteEndElement();	//style:style

			//style:name Text_20_body
			writer.WriteStartElement("style:style");
			writer.WriteAttributeString("style:name", "Text_20_body");
			writer.WriteAttributeString("style:display-name", "Text body");
			writer.WriteAttributeString("style:family", "paragraph");
			writer.WriteAttributeString("style:parent-style-name", "Standard");
			writer.WriteAttributeString("style:class", "text");
			writer.WriteStartElement("style:paragraph-properties");
			writer.WriteAttributeString("fo:margin-top", "0cm");
			writer.WriteAttributeString("fo:margin-bottom", "0.212cm");
			writer.WriteEndElement();
			writer.WriteEndElement();	//style:style

			//style:name Subtitle
			writer.WriteStartElement("style:style");
			writer.WriteAttributeString("style:name", "Subtitle");
			writer.WriteAttributeString("style:family", "paragraph");
			writer.WriteAttributeString("style:parent-style-name", "Title");
			writer.WriteAttributeString("style:next-style-name", "Text_20_body");
			writer.WriteAttributeString("style:class", "chapter");
			writer.WriteStartElement("style:paragraph-properties");
			writer.WriteAttributeString("fo:text-align", "center");
			writer.WriteAttributeString("style:justify-single-word", "false");
			writer.WriteEndElement();
			writer.WriteStartElement("style:text-properties");
			writer.WriteAttributeString("fo:font-size", "14pt");
			writer.WriteAttributeString("fo:font-style", "italic");
			writer.WriteAttributeString("style:font-size-asian", "14pt");
			writer.WriteAttributeString("style:font-style-asian", "italic");
			writer.WriteAttributeString("style:font-size-complex", "14pt");
			writer.WriteAttributeString("style:font-style-complex", "italic");
			writer.WriteEndElement();
			writer.WriteEndElement();	//style:style

			//style:name List
			writer.WriteStartElement("style:style");
			writer.WriteAttributeString("style:name", "List");
			writer.WriteAttributeString("style:family", "paragraph");
			writer.WriteAttributeString("style:parent-style-name", "Text_20_body");
			writer.WriteAttributeString("style:class", "list");
			writer.WriteStartElement("style:text-properties");
			writer.WriteAttributeString("style:font-name", "Arial");
			writer.WriteAttributeString("style:font-name-complex", "Tahoma1");
			writer.WriteEndElement();
			writer.WriteEndElement();	//style:style

			//style:name Caption
			writer.WriteStartElement("style:style");
			writer.WriteAttributeString("style:name", "Caption");
			writer.WriteAttributeString("style:family", "paragraph");
			writer.WriteAttributeString("style:parent-style-name", "Standard");
			writer.WriteAttributeString("style:class", "extra");
			writer.WriteStartElement("style:paragraph-properties");
			writer.WriteAttributeString("fo:margin-top", "0.212cm");
			writer.WriteAttributeString("fo:margin-bottom", "0.212cm");
			writer.WriteAttributeString("text:number-lines", "false");
			writer.WriteAttributeString("text:line-number", "0");
			writer.WriteEndElement();
			writer.WriteStartElement("style:text-properties");
			writer.WriteAttributeString("style:font-name", "Arial");
			writer.WriteAttributeString("fo:font-size", "10pt");
			writer.WriteAttributeString("fo:font-style", "italic");
			writer.WriteAttributeString("style:font-size-asian", "12pt");
			writer.WriteAttributeString("style:font-style-asian", "italic");
			writer.WriteAttributeString("style:font-name-complex", "Tahoma1");
			writer.WriteAttributeString("style:font-size-complex", "12pt");
			writer.WriteAttributeString("style:font-style-complex", "italic");
			writer.WriteEndElement();
			writer.WriteEndElement();	//style:style

			//style:name Index
			writer.WriteStartElement("style:style");
			writer.WriteAttributeString("style:name", "Index");
			writer.WriteAttributeString("style:family", "paragraph");
			writer.WriteAttributeString("style:parent-style-name", "Standard");
			writer.WriteAttributeString("style:class", "index");
			writer.WriteStartElement("style:paragraph-properties");
			writer.WriteAttributeString("text:number-lines", "false");
			writer.WriteAttributeString("text:line-number", "0");
			writer.WriteEndElement();
			writer.WriteStartElement("style:text-properties");
			writer.WriteAttributeString("style:font-name", "Arial");
			writer.WriteAttributeString("style:font-name-complex", "Tahoma1");
			writer.WriteEndElement();
			writer.WriteEndElement();	//style:style

			//style:name Table_20_Contents
			writer.WriteStartElement("style:style");
			writer.WriteAttributeString("style:name", "Table_20_Contents");
			writer.WriteAttributeString("style:display-name", "Table Contents");
			writer.WriteAttributeString("style:family", "paragraph");
			writer.WriteAttributeString("style:parent-style-name", "Standard");
			writer.WriteAttributeString("style:class", "extra");
			writer.WriteStartElement("style:paragraph-properties");
			writer.WriteAttributeString("text:number-lines", "false");
			writer.WriteAttributeString("text:line-number", "0");
			writer.WriteEndElement();
			writer.WriteEndElement();	//style:style

			//style:name Graphics
			writer.WriteStartElement("style:style");
			writer.WriteAttributeString("style:name", "Graphics");
			writer.WriteAttributeString("style:family", "graphic");
			writer.WriteStartElement("style:graphic-properties");
			writer.WriteAttributeString("text:anchor-type", "paragraph");
			writer.WriteAttributeString("svg:x", "0cm");
			writer.WriteAttributeString("svg:y", "0cm");
			writer.WriteAttributeString("style:wrap", "dynamic");
			writer.WriteAttributeString("style:number-wrapped-paragraphs", "no-limit");
			writer.WriteAttributeString("style:wrap-contour", "false");
			writer.WriteAttributeString("style:vertical-pos", "top");
			writer.WriteAttributeString("style:vertical-rel", "paragraph");
			writer.WriteAttributeString("style:horizontal-pos", "center");
			writer.WriteAttributeString("style:horizontal-rel", "paragraph");
			writer.WriteEndElement();
			writer.WriteEndElement();	//style:style

			writer.WriteStartElement("text:outline-style");
			writer.WriteStartElement("text:outline-level-style");
			writer.WriteAttributeString("text:level", "1");
			writer.WriteAttributeString("style:num-format", "");
			writer.WriteStartElement("style:list-level-properties");
			writer.WriteAttributeString("text:min-label-distance", "0.381cm");
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteStartElement("text:outline-level-style");
			writer.WriteAttributeString("text:level", "2");
			writer.WriteAttributeString("style:num-format", "");
			writer.WriteStartElement("style:list-level-properties");
			writer.WriteAttributeString("text:min-label-distance", "0.381cm");
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteStartElement("text:outline-level-style");
			writer.WriteAttributeString("text:level", "3");
			writer.WriteAttributeString("style:num-format", "");
			writer.WriteStartElement("style:list-level-properties");
			writer.WriteAttributeString("text:min-label-distance", "0.381cm");
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteStartElement("text:outline-level-style");
			writer.WriteAttributeString("text:level", "4");
			writer.WriteAttributeString("style:num-format", "");
			writer.WriteStartElement("style:list-level-properties");
			writer.WriteAttributeString("text:min-label-distance", "0.381cm");
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteStartElement("text:outline-level-style");
			writer.WriteAttributeString("text:level", "5");
			writer.WriteAttributeString("style:num-format", "");
			writer.WriteStartElement("style:list-level-properties");
			writer.WriteAttributeString("text:min-label-distance", "0.381cm");
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteStartElement("text:outline-level-style");
			writer.WriteAttributeString("text:level", "6");
			writer.WriteAttributeString("style:num-format", "");
			writer.WriteStartElement("style:list-level-properties");
			writer.WriteAttributeString("text:min-label-distance", "0.381cm");
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteStartElement("text:outline-level-style");
			writer.WriteAttributeString("text:level", "7");
			writer.WriteAttributeString("style:num-format", "");
			writer.WriteStartElement("style:list-level-properties");
			writer.WriteAttributeString("text:min-label-distance", "0.381cm");
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteStartElement("text:outline-level-style");
			writer.WriteAttributeString("text:level", "8");
			writer.WriteAttributeString("style:num-format", "");
			writer.WriteStartElement("style:list-level-properties");
			writer.WriteAttributeString("text:min-label-distance", "0.381cm");
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteStartElement("text:outline-level-style");
			writer.WriteAttributeString("text:level", "9");
			writer.WriteAttributeString("style:num-format", "");
			writer.WriteStartElement("style:list-level-properties");
			writer.WriteAttributeString("text:min-label-distance", "0.381cm");
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteStartElement("text:outline-level-style");
			writer.WriteAttributeString("text:level", "10");
			writer.WriteAttributeString("style:num-format", "");
			writer.WriteStartElement("style:list-level-properties");
			writer.WriteAttributeString("text:min-label-distance", "0.381cm");
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteFullEndElement();	//text:outline-style

			writer.WriteStartElement("text:notes-configuration");
			writer.WriteAttributeString("text:note-class", "footnote");
			writer.WriteAttributeString("style:num-format", "1");
			writer.WriteAttributeString("text:start-value", "0");
			writer.WriteAttributeString("text:footnotes-position", "page");
			writer.WriteAttributeString("text:start-numbering-at", "document");
			writer.WriteEndElement();

			writer.WriteStartElement("text:notes-configuration");
			writer.WriteAttributeString("text:note-class", "endnote");
			writer.WriteAttributeString("style:num-format", "i");
			writer.WriteAttributeString("text:start-value", "0");
			writer.WriteEndElement();

			writer.WriteStartElement("text:linenumbering-configuration");
			writer.WriteAttributeString("text:number-lines", "false");
			writer.WriteAttributeString("text:offset", "0.499cm");
			writer.WriteAttributeString("style:num-format", "1");
			writer.WriteAttributeString("text:number-position", "left");
			writer.WriteAttributeString("text:increment", "5");
			writer.WriteEndElement();

			writer.WriteEndElement();	//office:styles

			StiPage page = pages[0];
			double pageHeight = page.Unit.ConvertToHInches(page.PageHeight * page.SegmentPerHeight);
			double pageWidth = page.Unit.ConvertToHInches(page.PageWidth * page.SegmentPerWidth);
			double mgLeft = page.Unit.ConvertToHInches(page.Margins.Left);
			double mgRight = page.Unit.ConvertToHInches(page.Margins.Right);
			double mgTop = page.Unit.ConvertToHInches(page.Margins.Top);
			double mgBottom = page.Unit.ConvertToHInches(page.Margins.Bottom) - 4;	//correction
			if (mgBottom < 0) mgBottom = 0;

			writer.WriteStartElement("office:automatic-styles");
			writer.WriteStartElement("style:page-layout");
			writer.WriteAttributeString("style:name", "pm1");
			writer.WriteStartElement("style:page-layout-properties");
			writer.WriteAttributeString("fo:page-width", DoubleToString(pageWidth));
			writer.WriteAttributeString("fo:page-height", DoubleToString(pageHeight));
			writer.WriteAttributeString("style:num-format", "1");
			writer.WriteAttributeString("style:print-orientation", (page.Orientation == StiPageOrientation.Portrait ? "portrait" : "landscape"));
			writer.WriteAttributeString("fo:margin-top", DoubleToString(mgTop));
			writer.WriteAttributeString("fo:margin-bottom", DoubleToString(mgBottom));
			writer.WriteAttributeString("fo:margin-left", DoubleToString(mgLeft));
			writer.WriteAttributeString("fo:margin-right", DoubleToString(mgRight));
			writer.WriteAttributeString("style:writing-mode", "lr-tb");
			writer.WriteAttributeString("style:footnote-max-height", "0cm");
			writer.WriteStartElement("style:footnote-sep");
			writer.WriteAttributeString("style:width", "0.018cm");
			writer.WriteAttributeString("style:distance-before-sep", "0.101cm");
			writer.WriteAttributeString("style:distance-after-sep", "0.101cm");
			writer.WriteAttributeString("style:adjustment", "left");
			writer.WriteAttributeString("style:rel-width", "25%");
			writer.WriteAttributeString("style:color", "#000000");
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteStartElement("style:header-style");
			writer.WriteEndElement();
			writer.WriteStartElement("style:footer-style");
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteFullEndElement();	//office:automatic-styles

			writer.WriteStartElement("office:master-styles");
			writer.WriteStartElement("style:master-page");
			writer.WriteAttributeString("style:name", "Standard");
			writer.WriteAttributeString("style:page-layout-name", "pm1");
			writer.WriteEndElement();
			writer.WriteEndElement();

			writer.WriteFullEndElement();
			writer.WriteEndDocument();
			writer.Flush();
			return ms;
		}
		#endregion

		#region WriteContent
		private MemoryStream WriteContent()
		{
			MemoryStream ms = new Tools.StiCachedStream();
			XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
			writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
			writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
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

			#region Make first pass
			bool[,] readyCells = new bool[Matrix.CoordY.Count, Matrix.CoordX.Count];
			int[,] cellStyleTable = new int[Matrix.CoordY.Count, Matrix.CoordX.Count];
			Hashtable rowHeightList = new Hashtable();
			Hashtable rowHeightList2 = new Hashtable();
			int[] rowPosList = new int[Matrix.CoordY.Count];

			for (int indexRow = 1; indexRow < Matrix.CoordY.Count; indexRow ++)
			{
				double rowHeight = (double)Matrix.CoordY.GetByIndex(indexRow) - (double)Matrix.CoordY.GetByIndex(indexRow - 1);
				if (!rowHeightList.ContainsKey(rowHeight))
				{
					int listPos = rowHeightList.Count;
					rowHeightList[rowHeight] = listPos;
					rowHeightList2[listPos] = rowHeight;
				}
				rowPosList[indexRow - 1] = (int)rowHeightList[rowHeight];

				for (int indexColumn = 1; indexColumn < Matrix.CoordX.Count; indexColumn ++)
				{
					StiCell cell = Matrix.Cells[indexRow - 1, indexColumn - 1];

					if (!readyCells[indexRow, indexColumn])
					{
						if (cell != null)
						{
							#region Range
							for (int yy = 0; yy <= cell.Height; yy++)
							{
								for (int xx = 0; xx <= cell.Width; xx++)
								{
									readyCells[indexRow + yy, indexColumn + xx] = true;
								}
							}
							#endregion

							cellStyleTable[indexRow - 1, indexColumn - 1] = GetCellStyleNumber(indexRow - 1, indexColumn - 1, cell.Height + 1, cell.Width + 1);
							int tempInt2 = GetParagraphStyleNumber(indexRow - 1, indexColumn - 1);
						}
						else
						{
							cellStyleTable[indexRow - 1, indexColumn - 1] = GetCellStyleNumber(indexRow - 1, indexColumn - 1, 1, 1);
						}
					}
				}
			}
			#endregion

			writer.WriteStartElement("office:scripts");
			writer.WriteEndElement();

			#region Write fonts info
            Hashtable fonts = new Hashtable();
            foreach (ParagraphStyleData par in paragraphStyleList)
            {
                fonts[par.FontName] = par.FontName;
            }

            writer.WriteStartElement("office:font-face-decls");
            foreach (DictionaryEntry de in fonts)
            {
                string fontName = (string)de.Value;
                writer.WriteStartElement("style:font-face");
                writer.WriteAttributeString("style:name", fontName);
                writer.WriteAttributeString("svg:font-family", fontName);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
			#endregion

			#region Write automatic styles
			writer.WriteStartElement("office:automatic-styles");

			writer.WriteStartElement("style:style");
			writer.WriteAttributeString("style:name", "Table1");
			writer.WriteAttributeString("style:family", "table");
			writer.WriteStartElement("style:table-properties");
			double tableWidth = (double)Matrix.CoordX.GetByIndex(Matrix.CoordX.Count - 1) - (double)Matrix.CoordX.GetByIndex(0);
			writer.WriteAttributeString("style:width", DoubleToString(tableWidth));
			writer.WriteAttributeString("table:align", "left");
			writer.WriteEndElement();
			writer.WriteEndElement();

			for (int indexColumn = 0; indexColumn < Matrix.CoordX.Count - 1; indexColumn++)
			{
				writer.WriteStartElement("style:style");
				writer.WriteAttributeString("style:name", string.Format("Table1.{0}", GetColumnName(indexColumn)));
				writer.WriteAttributeString("style:family", "table-column");
				writer.WriteStartElement("style:table-column-properties");
				double columnWidth = (double)Matrix.CoordX.GetByIndex(indexColumn + 1) - (double)Matrix.CoordX.GetByIndex(indexColumn);
				writer.WriteAttributeString("style:column-width", DoubleToString(columnWidth));
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
			//for (int indexRow = 0; indexRow < Matrix.CoordY.Count - 1; indexRow++)
			for (int indexRow = 0; indexRow < rowHeightList.Count; indexRow++)
			{
				writer.WriteStartElement("style:style");
				writer.WriteAttributeString("style:name", string.Format("Table1.{0}", indexRow + 1));
				writer.WriteAttributeString("style:family", "table-row");
				writer.WriteStartElement("style:table-row-properties");
				double rowHeight = (double)rowHeightList2[indexRow];
				writer.WriteAttributeString("style:row-height", DoubleToString(rowHeight));
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
			for (int indexStyle = 0; indexStyle < cellStyleList.Count; indexStyle++)
			{
				CellStyleData style = (CellStyleData)cellStyleList[indexStyle];
				writer.WriteStartElement("style:style");
				writer.WriteAttributeString("style:name", string.Format("cell{0}", indexStyle + 1));
				writer.WriteAttributeString("style:family", "table-cell");
				writer.WriteStartElement("style:table-cell-properties");
				if (style.VertAlign == StiVertAlignment.Center) writer.WriteAttributeString("style:vertical-align", "middle");
				if (style.VertAlign == StiVertAlignment.Bottom) writer.WriteAttributeString("style:vertical-align", "bottom");
				if (style.BackColor != Color.Transparent)
				{
					writer.WriteAttributeString("fo:background-color", GetColorString(style.BackColor));
				}
				else
				{
					writer.WriteAttributeString("fo:background-color", "transparent");
				}
				writer.WriteAttributeString("fo:padding", "0in");
				writer.WriteAttributeString("fo:border-left", style.BorderLeft);
				writer.WriteAttributeString("fo:border-right", style.BorderRight);
				writer.WriteAttributeString("fo:border-top", style.BorderTop);
				writer.WriteAttributeString("fo:border-bottom", style.BorderBottom);
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
			#endregion

			#region Write styles
			for (int indexStyle = 0; indexStyle < paragraphStyleList.Count; indexStyle++)
			{
				ParagraphStyleData style = (ParagraphStyleData)paragraphStyleList[indexStyle];
				writer.WriteStartElement("style:style");
				writer.WriteAttributeString("style:name", string.Format("par{0}", indexStyle + 1));
				writer.WriteAttributeString("style:family", "paragraph");
				writer.WriteAttributeString("style:parent-style-name", "Table_20_Contents");
				if (style.HorAlign != StiTextHorAlignment.Left)
				{
					writer.WriteStartElement("style:paragraph-properties");
					if (style.HorAlign == StiTextHorAlignment.Center) writer.WriteAttributeString("fo:text-align", "center");
					if (style.HorAlign == StiTextHorAlignment.Right) writer.WriteAttributeString("fo:text-align", "right");
					if (style.HorAlign == StiTextHorAlignment.Width) writer.WriteAttributeString("fo:text-align", "justify");
					writer.WriteEndElement();
				}
				writer.WriteStartElement("style:text-properties");
				writer.WriteAttributeString("fo:color", GetColorString(style.FontColor));
				writer.WriteAttributeString("style:font-name", style.FontName);
				string fontSizeSt = string.Format("{0}pt", style.FontSize).Replace(",", ".");
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
				if (style.Angle != 0)
				{
					writer.WriteAttributeString("style:text-rotation-angle", string.Format("{0}", style.Angle));
				}
				writer.WriteEndElement();
				writer.WriteEndElement();
			}

			writer.WriteStartElement("style:style");
			writer.WriteAttributeString("style:name", "parEnd");
			writer.WriteAttributeString("style:family", "paragraph");
			writer.WriteAttributeString("style:parent-style-name", "Table_20_Contents");
			writer.WriteStartElement("style:text-properties");
			string fontSizeStr = string.Format("{0}pt", 1).Replace(",", ".");
			writer.WriteAttributeString("fo:font-size", fontSizeStr);
			writer.WriteAttributeString("fo:font-size-asian", fontSizeStr);
			writer.WriteAttributeString("fo:font-size-complex", fontSizeStr);
			writer.WriteEndElement();
			writer.WriteEndElement();

			writer.WriteStartElement("style:style");
			writer.WriteAttributeString("style:name", "fr1");
			writer.WriteAttributeString("style:family", "graphic");
			writer.WriteAttributeString("style:parent-style-name", "Graphics");
			writer.WriteStartElement("style:graphic-properties");
			writer.WriteAttributeString("style:vertical-pos", "middle");
			writer.WriteAttributeString("style:vertical-rel", "paragraph");
			writer.WriteAttributeString("style:horizontal-pos", "center");
			writer.WriteAttributeString("style:horizontal-rel", "paragraph");
			writer.WriteAttributeString("fo:background-color", "transparent");
			writer.WriteAttributeString("style:background-transparency", "100%");
			writer.WriteAttributeString("style:shadow", "none");
			writer.WriteAttributeString("style:mirror", "none");
			writer.WriteAttributeString("fo:clip", "rect(0cm 0cm 0cm 0cm)");
			writer.WriteAttributeString("draw:luminance", "0%");
			writer.WriteAttributeString("draw:contrast", "0%");
			writer.WriteAttributeString("draw:red", "0%");
			writer.WriteAttributeString("draw:green", "0%");
			writer.WriteAttributeString("draw:blue", "0%");
			writer.WriteAttributeString("draw:gamma", "100%");
			writer.WriteAttributeString("draw:color-inversion", "false");
			writer.WriteAttributeString("draw:image-opacity", "100%");
			writer.WriteAttributeString("draw:color-mode", "standard");
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteEndElement();
			#endregion

			writer.WriteStartElement("office:body");
			writer.WriteStartElement("office:text");

			writer.WriteStartElement("office:forms");
			writer.WriteAttributeString("form:automatic-focus", "false");
			writer.WriteAttributeString("form:apply-design-mode", "false");
			writer.WriteEndElement();

			#region Write sequence-decls
			writer.WriteStartElement("text:sequence-decls");
			writer.WriteStartElement("text:sequence-decl");
			writer.WriteAttributeString("text:display-outline-level", "0");
			writer.WriteAttributeString("text:name", "Illustration");
			writer.WriteEndElement();
			writer.WriteStartElement("text:sequence-decl");
			writer.WriteAttributeString("text:display-outline-level", "0");
			writer.WriteAttributeString("text:name", "Table");
			writer.WriteEndElement();
			writer.WriteStartElement("text:sequence-decl");
			writer.WriteAttributeString("text:display-outline-level", "0");
			writer.WriteAttributeString("text:name", "Text");
			writer.WriteEndElement();
			writer.WriteStartElement("text:sequence-decl");
			writer.WriteAttributeString("text:display-outline-level", "0");
			writer.WriteAttributeString("text:name", "Drawing");
			writer.WriteEndElement();
			writer.WriteEndElement();
			#endregion

			#region Write table
			readyCells = new bool[Matrix.CoordY.Count, Matrix.CoordX.Count];

			writer.WriteStartElement("table:table");
			writer.WriteAttributeString("table:name", "Table1");
			writer.WriteAttributeString("table:style-name", "Table1");
			//columns
			for (int indexColumn = 0; indexColumn < Matrix.CoordX.Count - 1; indexColumn++)
			{
				writer.WriteStartElement("table:table-column");
				writer.WriteAttributeString("table:style-name", string.Format("Table1.{0}", GetColumnName(indexColumn)));
				writer.WriteEndElement();
			}

            double progressScale = Math.Max(matrix.CoordY.Count / 200f, 1f);
            int progressValue = 0;

            for (int indexRow = 1; indexRow < Matrix.CoordY.Count; indexRow ++)
			{
                int currentProgress = (int)(indexRow / progressScale);
                if (currentProgress > progressValue)
                {
                    progressValue = currentProgress;
                    InvokeExporting(indexRow, matrix.CoordY.Count, CurrentPassNumber, MaximumPassNumber);
                }

                if (matrix.isPaginationMode && matrix.HorizontalPageBreaksHash.ContainsKey(indexRow)) continue; //skip pagination space

                writer.WriteStartElement("table:table-row");
				writer.WriteAttributeString("table:style-name", string.Format("Table1.{0}", rowPosList[indexRow - 1] + 1));

				for (int indexColumn = 1; indexColumn < Matrix.CoordX.Count; indexColumn ++)
				{
					StiCell cell = Matrix.Cells[indexRow - 1, indexColumn - 1];

					if (!readyCells[indexRow, indexColumn])
					{
						if (cell != null)
						{
							#region Range
							for (int yy = 0; yy <= cell.Height; yy++)
							{
								for (int xx = 0; xx <= cell.Width; xx++)
								{
									readyCells[indexRow + yy, indexColumn + xx] = true;
								}
							}
							#endregion

							//int cellStyleIndex = GetCellStyleNumber(indexRow - 1, indexColumn - 1, cell.Height + 1, cell.Width + 1);
							int cellStyleIndex = cellStyleTable[indexRow - 1, indexColumn - 1];
							int parStyleIndex = GetParagraphStyleNumber(indexRow - 1, indexColumn - 1);

							writer.WriteStartElement("table:table-cell");
							writer.WriteAttributeString("table:style-name", string.Format("cell{0}", cellStyleIndex + 1));
							if (cell.Width > 0)
							{
								writer.WriteAttributeString("table:number-columns-spanned", string.Format("{0}", cell.Width + 1));	//merged
							}
							if (cell.Height > 0)
							{
								writer.WriteAttributeString("table:number-rows-spanned", string.Format("{0}", cell.Height + 1));	//merged
							}
							writer.WriteAttributeString("office:value-type", "string");

							if ((cell.Component is StiText) && (!cell.Component.IsExportAsImage(StiExportFormat.Odt)) && (cell.Text != null))
							{
								#region Text
                                string cellText = cell.Text;

                                //fix for rotated text
                                if ((paragraphStyleList[parStyleIndex] as ParagraphStyleData).Angle != 0)
                                {
                                    cellText = cellText.Replace('\n', ' ');
                                }

								#region count lines and make stringList
								ArrayList stringList = new ArrayList();
								string st = string.Empty;
                                foreach (char ch in cellText)
								{
									if (char.IsControl(ch) && (ch != '\t'))
									{
										if (ch == '\n')
										{
											stringList.Add(st);
											st = string.Empty;
										}
									}
									else
									{
										st += ch;
									}
								}
								if (st != string.Empty)	stringList.Add(st);
								if (stringList.Count == 0) stringList.Add(st);
								#endregion

								for (int indexLine = 0; indexLine < stringList.Count; indexLine++)
								{
									string textLine = (string)stringList[indexLine];

									writer.WriteStartElement("text:p");
									writer.WriteAttributeString("text:style-name", string.Format("par{0}", parStyleIndex + 1));
									writer.WriteString(textLine);
									writer.WriteEndElement();
								}
								#endregion
							}
							if (cell.Component.IsExportAsImage(StiExportFormat.Odt))
							{
                                #region Image
                                var exportImageExtended = cell.Component as IStiExportImageExtended;
                                if (exportImageExtended != null)
								{
									float rsImageResolution = imageResolution;
									using (Image image = exportImageExtended.GetImage(ref rsImageResolution, imageFormat == null || imageFormat == ImageFormat.Png ? StiExportFormat.ImagePng : StiExportFormat.Excel))
									{
										if (image != null)
										{
											int indexImage = imageCache.AddImageInt(image);

											double imageWidth = (double)Matrix.CoordX.GetByIndex(indexColumn + cell.Width) - (double)Matrix.CoordX.GetByIndex(indexColumn - 1);
											double imageHeight = (double)Matrix.CoordY.GetByIndex(indexRow + cell.Height) - (double)Matrix.CoordY.GetByIndex(indexRow - 1);

											#region Write image info
											writer.WriteStartElement("text:p");
											writer.WriteAttributeString("text:style-name", "Table_20_Contents");

											writer.WriteStartElement("draw:frame");
											writer.WriteAttributeString("draw:style-name", "fr1");
											writer.WriteAttributeString("draw:name", string.Format("Picture{0}", indexImage + 1));
											writer.WriteAttributeString("text:anchor-type", "paragraph");
											writer.WriteAttributeString("svg:width", DoubleToString(imageWidth));
											writer.WriteAttributeString("svg:height", DoubleToString(imageHeight));
											writer.WriteAttributeString("draw:z-index", "0");

											writer.WriteStartElement("draw:image");
											writer.WriteAttributeString("xlink:href", string.Format("Pictures/{0:D5}.{1}", indexImage + 1, GetImageFormatExtension(imageCache.ImageFormatStore[indexImage])));
											writer.WriteAttributeString("xlink:type", "simple");
											writer.WriteAttributeString("xlink:show", "embed");
											writer.WriteAttributeString("xlink:actuate", "onLoad");
											writer.WriteEndElement();

											writer.WriteEndElement();	//draw:frame
											writer.WriteEndElement();	//text:p
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
							int cellStyleIndex = cellStyleTable[indexRow - 1, indexColumn - 1];
							writer.WriteStartElement("table:table-cell");
							writer.WriteAttributeString("table:style-name", string.Format("cell{0}", cellStyleIndex + 1));
							writer.WriteEndElement();
						}
					}
					else
					{
						writer.WriteStartElement("table:covered-table-cell");	//merged
						writer.WriteEndElement();
					}
				}
				writer.WriteEndElement();	//table-row
			}
			writer.WriteEndElement();	//table:table
			#endregion

			//row
			writer.WriteStartElement("text:p");
			writer.WriteAttributeString("text:style-name", "parEnd");
			writer.WriteEndElement();

			writer.WriteEndElement();	//office:text
			writer.WriteEndElement();	//office:body

			writer.WriteFullEndElement();
			writer.WriteEndDocument();
			writer.Flush();
			return ms;
		}
		#endregion
        
		/// <summary>
		/// Exports rendered report to an Odt file.
		/// </summary>
		/// <param name="report">A report which is to be exported.</param>
		/// <param name="fileName">A name of the file for exporting a rendered report.</param>
		public void ExportOdt(StiReport report, string fileName)
		{
			FileStream stream = null;
			try
			{
				StiFileUtils.ProcessReadOnly(fileName);
				stream = new FileStream(fileName, FileMode.Create);
				ExportOdt(report, stream);
			}
			finally
			{
				stream.Flush();
				stream.Close();
			}			
		}

        
		/// <summary>
		/// Exports rendered report to an Odt file.
		/// </summary>
		/// <param name="report">A report which is to be exported.</param>
		/// <param name="stream">A stream for export of a document.</param>
		public void ExportOdt(StiReport report, Stream stream)
		{
			StiOdtExportSettings settings = new StiOdtExportSettings();

			ExportOdt(report, stream, settings);
		}

        
		/// <summary>
		/// Exports rendered report to an Odt file.
		/// </summary>
		/// <param name="report">A report which is to be exported.</param>
		/// <param name="stream">A stream for export of a document.</param>
		/// <param name="pageRange">Describes range of pages of the document for the export.</param>
		public void ExportOdt(StiReport report, Stream stream, StiPagesRange pageRange)
		{
			StiOdtExportSettings settings = new StiOdtExportSettings();

			settings.PageRange = pageRange;

			ExportOdt(report, stream, settings);
		}

        

		public void ExportOdt(StiReport report, Stream stream, StiOdtExportSettings settings)
		{
			StiLogService.Write(this.GetType(), "Export report to Odt format");

#if NETSTANDARD || NETCOREAPP
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif

            #region Read settings
            if (settings == null)
				throw new ArgumentNullException("The 'settings' argument cannot be equal in null.");

			StiPagesRange pageRange =	settings.PageRange;
			usePageHeadersAndFooters =	settings.UsePageHeadersAndFooters;
            removeEmptySpaceAtBottom =  settings.RemoveEmptySpaceAtBottom;
            imageResolution         =   settings.ImageResolution;
			imageQuality			=	settings.ImageQuality;
            this.imageFormat = settings.ImageFormat;
            #endregion

            xmlIndentation = -1;

			if (imageQuality < 0)imageQuality = 0;
			if (imageQuality > 1)imageQuality = 1;
			if (imageResolution < 10) imageResolution = 10;
			imageResolution = imageResolution / 100;
            if (imageFormat != null && imageFormat != ImageFormat.Png) imageFormat = ImageFormat.Jpeg;

            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
                //StiExportUtils.DisableFontSmoothing();
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                imageCache = new StiImageCache(StiOptions.Export.OpenDocumentWriter.AllowImageComparer, imageFormat, imageQuality);
				cellStyleList = new ArrayList();
				paragraphStyleList = new ArrayList();

                CurrentPassNumber = 0;
                MaximumPassNumber = 3 + (StiOptions.Export.OpenDocumentWriter.DivideSegmentPages ? 1 : 0);

				StiPagesCollection pages = pageRange.GetSelectedPages(report.RenderedPages);
                if (StiOptions.Export.OpenDocumentWriter.DivideSegmentPages)
			    {
			        pages = StiSegmentPagesDivider.Divide(pages, this);
			        CurrentPassNumber++;
			    }
			    if (IsStopped) return;


				StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");
				StiZipWriter20 zip = new StiZipWriter20();
				zip.Begin(stream, true);

				matrix = new StiMatrix(pages, true, this);
				if (IsStopped) return;
                StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");
                CurrentPassNumber = 2 + (StiOptions.Export.OpenDocumentWriter.DivideSegmentPages ? 1 : 0);

				zip.AddFile("content.xml", WriteContent());
				zip.AddFile("mimetype", WriteMimetype());
				zip.AddFile("meta.xml", WriteMeta());
				zip.AddFile("META-INF/manifest.xml", WriteManifest());
				zip.AddFile("settings.xml", WriteSettings());
				zip.AddFile("styles.xml", WriteStyles(pages));

				if (imageCache.ImagePackedStore.Count > 0)
				{
					for (int index = 0; index < imageCache.ImagePackedStore.Count; index ++)
					{
						zip.AddFile(string.Format("Pictures/{0:D5}.{1}", index + 1, GetImageFormatExtension(imageCache.ImageFormatStore[index])), WriteImage(index));
					}
				}

				zip.End();
			}
			finally
			{
                StiExportUtils.EnableFontSmoothing(report);
                Thread.CurrentThread.CurrentCulture = currentCulture;
				if (matrix != null)
				{
					matrix.Clear();
					matrix = null;
				}
				cellStyleList.Clear();
				cellStyleList = null;
				paragraphStyleList.Clear();
				paragraphStyleList = null;
				imageCache.Clear();
				imageCache = null;

                if (report.RenderedPages.CacheMode) StiMatrix.GCCollect();
			}
		}
		#endregion
	}
}
