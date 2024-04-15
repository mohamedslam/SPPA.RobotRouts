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
using System.Xml;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Text;
using System.Reflection;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.Components;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Zip;
using Stimulsoft.Base.Licenses;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Maps;
using System.Threading;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Helpers;
using System.Security;
using Stimulsoft.Report.Export.Services.Helpers;
using Stimulsoft.Report.Dashboard;
using System.Drawing;
using System.Drawing.Imaging;

#if CLOUD
using Stimulsoft.Base.Plans;
#endif

#if NETSTANDARD
using Stimulsoft.System.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
using Font = Stimulsoft.Drawing.Font;
using ImageAttributes = Stimulsoft.Drawing.Imaging.ImageAttributes;
#endif

namespace Stimulsoft.Report.Export
{

    /// <summary>
    /// A class for the HTML export.
    /// </summary>
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourceHtml.png")]
    public class StiHtmlExportService : StiExportService
    {
        #region StiExportService override
        /// <summary>
		/// Gets or sets a default extension of export. 
		/// </summary>
		public override string DefaultExtension
        {
            get
            {
                if (compressToArchive)
                    return "zip";

                if (exportSettings is StiMhtExportSettings) return "mht";
                if (exportSettings is StiHtmlExportSettings && (exportSettings as StiHtmlExportSettings).HtmlType == StiHtmlType.Mht) return "mht";

                return "html";
            }
        }

        public override StiExportFormat ExportFormat
        {
            get
            {
                if (exportSettings is StiHtml5ExportSettings) return StiExportFormat.Html5;
                if (exportSettings is StiMhtExportSettings) return StiExportFormat.Mht;
                if (exportSettings is StiHtmlExportSettings)
                {
                    StiHtmlType htmlType = (exportSettings as StiHtmlExportSettings).HtmlType;
                    if (htmlType == StiHtmlType.Html5) return StiExportFormat.Html5;
                    if (htmlType == StiHtmlType.Mht) return StiExportFormat.Mht;
                }
                return StiExportFormat.Html;
            }
        }

        /// <summary>
        /// Gets a group of the export in the context menu.
        /// </summary>
        public override string GroupCategory => "Web";

        /// <summary>
        /// Gets a position of the export in the context menu.
        /// </summary>
        public override int Position => (int)StiExportPosition.Html;

        /// <summary>
        /// Gets a export name in the context menu.
        /// </summary>
		public override string ExportNameInMenu => StiLocalization.Get("Export", "ExportTypeHtmlFile");

        /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public override void ExportTo(StiReport report, Stream stream, StiExportSettings settings)
        {
            exportSettings = settings as StiHtmlExportSettings;

            var htmlSettings = settings as StiHtmlExportSettings;
            var htmlType = htmlSettings != null ? htmlSettings.HtmlType : StiHtmlType.Html;

            if (htmlType == StiHtmlType.Html5 || htmlSettings is StiHtml5ExportSettings)
            {
                var html5ExportService = new StiHtml5ExportService();
                html5ExportService.ExportHtml(report, stream, htmlSettings as StiHtml5ExportSettings);
            }
            else if (htmlType == StiHtmlType.Mht || htmlSettings is StiMhtExportSettings)
            {
                var mhtExportService = new StiMhtExportService();
                mhtExportService.ExportMht(report, stream, htmlSettings as StiMhtExportSettings);
            }
            else
            {
                ExportHtml(report, stream, htmlSettings);
            }
            InvokeExporting(100, 100, 1, 1);
        }

        /// <summary>
        /// Exports a rendered report to the HTML file.
        /// Also rendered report can be sent via e-mail.
        /// </summary>
        /// <param name="report">A rendered report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        /// <param name="sendEMail">A parameter indicating whether the exported report will be sent via e-mail.</param>
        public override void Export(StiReport report, string fileName, bool sendEMail, StiGuiMode guiMode)
        {
            using (var form = StiGuiOptions.GetExportFormRunner("StiHtmlSetupForm", guiMode, this.OwnerWindow))
            {
                form["CurrentPage"] = report.CurrentPrintPage;
                form["OpenAfterExportEnabled"] = !sendEMail;

                this.reportTmp = report;
                this.documentFileName = fileName;
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
        /// Returns a filter for Html files.
        /// </summary>
        /// <returns>Returns a filter for Html files.</returns>
        public override string GetFilter()
        {
            if (compressToArchive)
                return StiLocalization.Get("FileFilters", "ZipArchives");
            return StiLocalization.Get("FileFilters", "HtmlFiles");
        }
        #endregion

        #region Handlers
        private void Form_Complete(IStiFormRunner form, StiShowDialogCompleteEvetArgs e)
        {
            if (e.DialogResult)
            {
                var exportFormat = (StiExportFormat)form["ExportFormat"];

                #region Export Html5

                if (exportFormat == StiExportFormat.Html5)
                {
                    var html5ExportService = new StiHtml5ExportService();
                    html5ExportService.compressToArchive = (bool)form["CompressToArchive"];

                    if (string.IsNullOrEmpty(documentFileName))
                        documentFileName = html5ExportService.GetFileName(reportTmp, sendEMail);

                    if (documentFileName != null)
                    {
                        StiFileUtils.ProcessReadOnly(documentFileName);
                        try
                        {
                            using (var stream = new FileStream(documentFileName, FileMode.Create))
                            {
                                html5ExportService.StartProgress(guiMode);

                                var settings = new StiHtml5ExportSettings
                                {
                                    PageRange = form["PagesRange"] as StiPagesRange,
                                    ImageFormat = (ImageFormat)form["ImageFormat"],
                                    ImageResolution = (float)form["Resolution"],
                                    ImageQuality = (float)form["ImageQuality"],
                                    ContinuousPages = (bool)form["ContinuousPages"],
                                    CompressToArchive = (bool)form["CompressToArchive"]
                                };

                                exportSettings = settings;

                                html5ExportService.StartExport(reportTmp, stream, settings, sendEMail, (bool)form["OpenAfterExport"], documentFileName, guiMode);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }

                #endregion

                #region Mht

                if (exportFormat == StiExportFormat.Mht)
                {
                    var mhtExportService = new StiMhtExportService();
                    mhtExportService.compressToArchive = (bool)form["CompressToArchive"];

                    if (string.IsNullOrEmpty(documentFileName))
                        documentFileName = mhtExportService.GetFileName(reportTmp, sendEMail);

                    if (documentFileName != null)
                    {
                        StiFileUtils.ProcessReadOnly(documentFileName);

                        try
                        {
                            using (var stream = new FileStream(documentFileName, FileMode.Create))
                            {
                                mhtExportService.StartProgress(guiMode);

                                var settings = new StiMhtExportSettings
                                {
                                    PageRange = form["PagesRange"] as StiPagesRange,
                                    Zoom = (float)form["Zoom"],
                                    ImageFormat = (ImageFormat)form["ImageFormat"],
                                    ExportMode = (StiHtmlExportMode)form["ExportMode"],
                                    ExportQuality = StiHtmlExportQuality.High,
                                    AddPageBreaks = (bool)form["AddPageBreaks"],
                                    CompressToArchive = (bool)form["CompressToArchive"],
                                    Encoding = Encoding.UTF8
                                };

                                mhtExportService.fileName = documentFileName;

                                exportSettings = settings;

                                mhtExportService.StartExport(reportTmp, stream, settings, sendEMail, (bool)form["OpenAfterExport"], documentFileName, guiMode);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }

                #endregion

                #region Html

                if (exportFormat == StiExportFormat.Html)
                {
                    compressToArchive = (bool)form["CompressToArchive"];

                    if (string.IsNullOrEmpty(documentFileName))
                        documentFileName = base.GetFileName(reportTmp, sendEMail);

                    if (documentFileName != null)
                    {
                        StiFileUtils.ProcessReadOnly(documentFileName);
                        try
                        {
                            using (var stream = new FileStream(documentFileName, FileMode.Create))
                            {
                                StartProgress(guiMode);

                                var settings = new StiHtmlExportSettings
                                {
                                    PageRange = form["PagesRange"] as StiPagesRange,
                                    Zoom = (float)form["Zoom"],
                                    ImageFormat = (ImageFormat)form["ImageFormat"],
                                    ExportMode = (StiHtmlExportMode)form["ExportMode"],
                                    ExportQuality = StiHtmlExportQuality.High,
                                    AddPageBreaks = (bool)form["AddPageBreaks"],
                                    CompressToArchive = (bool)form["CompressToArchive"],
                                    UseEmbeddedImages = (bool)form["UseEmbeddedImages"]
                                };

                                exportSettings = settings;

                                base.StartExport(reportTmp, stream, settings, sendEMail, (bool)form["OpenAfterExport"],
                                    documentFileName, guiMode);
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

        #region Fields
        private StiHtmlExportSettings exportSettings;
        private StiReport reportTmp;
        private string documentFileName;
        private bool sendEMail;
        private StiGuiMode guiMode;

        public StiHtmlTableRender TableRender;
        public StiHtmlTextWriter HtmlWriter;
        internal StiZipWriter20 zip;
        internal StiReport report;
        internal string fileName = string.Empty;
        private double startPage = 0;
        internal int imageNumber = 1;
        internal double zoom = 0.75;
        internal ImageFormat imageFormat;
        private NumberFormatInfo numberFormat = new NumberFormatInfo();
        internal StiHtmlExportQuality exportQuality;
        internal bool useStylesTable = true;
        internal bool isFileStreamMode = true;
        internal float imageQuality = 0.75f;
        internal float imageResolution = 96;
        internal bool compressToArchive;
        internal bool useEmbeddedImages;
        internal string openLinksTarget = null;
        internal StiHtmlChartType chartType = StiHtmlChartType.Image;
        internal StiHtmlExportMode exportMode = StiHtmlExportMode.Table;

        private SortedList coordX;
        private SortedList coordY;
        private string strSpanDiv = "span";
        private Hashtable hyperlinksToTag;
        private Hashtable pointerToBookmark;    //convert pointer link to bookmark
        private Hashtable pointerToTag;    //convert pointer link to bookmark

        internal Hashtable chartData = new Hashtable();

        internal bool useFullExceedMargins = false;

        private CultureInfo currentCulture;
        private CultureInfo storedCulture;

        internal Hashtable hashBookmarkGuid;

        private const double hiToPt = 0.716;   //0.716, must be 0.72, but it's correction of browsers calculation
        private const double pxToPt = 0.75;
        private const double dpi96 = 0.96;

        private int defaultCoordinatesPrecision = StiOptions.Export.Html.DefaultCoordinatesPrecision;

        private static Hashtable fontsToCorrectHeight = new Hashtable() {
            { "\u5FAE\u8F6F\u96C5\u9ED1", 1.09 },   // 微软雅黑, Microsoft YaHei
            {"Agency FB", 0.98},
            {"Algerian", 1.08},
            {"Arial Black", 1.17},
            {"Arial Narrow", 0.96},
            {"Arial Rounded MT Bold", 0.97},
            {"Arial", 0.96},
            {"Arial Unicode MS", 1.09},
            {"Baskerville Old Face",0.96},
            {"Bauhaus 93", 1.21},
            {"Bell MT", 0.94},
            {"Berlin Sans FB Demi", 0.94},
            {"Berlin Sans FB", 0.94},
            {"Bernard MT Condensed", 0.99},
            {"Bodoni MT Black", 0.97},
            {"Bodoni MT Condensed", 0.98},
            {"Bodoni MT Poster Compressed", 0.96},
            {"Bodoni MT", 1.01},
            {"Calibri Light", 1.01},
            {"Calibri", 1.01},
            {"Californian FB", 0.94},
            {"Cambria Math", 0.98},
            {"Cambria", 0.98},
            {"Candara", 1.01},
            {"Courier New", 0.96},
            {"Franklin Gothic Book", 0.94},
            {"Franklin Gothic Demi Cond", 0.93},
            {"Franklin Gothic Demi", 0.93},
            {"Franklin Gothic Heavy", 0.93},
            {"Franklin Gothic Medium Cond", 0.93},
            {"Franklin Gothic Medium", 0.93},
            {"Futura CondensedLight", 0.98},
            {"Futura Lt BT", 1.0},
            {"Futura Md BT", 1.0},
            {"Garamond", 0.94},
            {"Georgia", 0.94},
            {"Gill Sans MT Condensed", 1.0},
            {"Gill Sans MT Ext Condensed Bold", 0.99},
            {"Gill Sans MT", 0.96},
            {"Gill Sans Ultra Bold Condensed", 1.03},
            {"Gill Sans Ultra Bold", 1.03},
            {"HelveticaNeueCyr", 0.94},
            {"Lucida Bright", 0.98},
            {"Lucida Calligraphy", 1.16},
            {"Lucida Console", 0.84},
            {"Lucida Fax", 0.98},
            {"Lucida Handwriting", 1.15},
            {"Lucida Sans Typewriter", 0.98},
            {"Lucida Sans Unicode", 1.28},
            {"Lucida Sans", 0.98},
            {"Microsoft JhengHei Light", 1.11},
            {"Microsoft JhengHei UI Light", 1.04},
            {"Microsoft JhengHei UI", 1.04},
            {"Microsoft JhengHei", 1.12},
            {"Microsoft New Tai Lue", 1.1},
            {"Microsoft PhagsPa", 1.06},
            {"Microsoft Sans Serif", 0.94},
            {"Microsoft Tai Le", 1.07},
            {"Microsoft Uighur", 0.92},
            {"Microsoft YaHei", 1.09},
            {"Microsoft YaHei Light", 1.12},
            {"Microsoft YaHei UI Light", 1.1},
            {"Microsoft YaHei UI", 1.04},
            {"Microsoft Yi Baiti", 0.86},
            {"MS Gothic", 0.84},
            {"MS Outlook", 0.86},
            {"MS PGothic", 0.84},
            {"MS Reference Specialty", 1.0},
            {"MS UI Gothic", 0.84},
            {"Rockwell Condensed", 0.98},
            {"Rockwell Extra Bold", 0.97},
            {"Rockwell", 0.97},
            {"Segoe MDL2 Assets", 0.82},
            {"Segoe Print", 1.47},
            {"Segoe Script", 1.31},
            {"Segoe UI", 1.09},
            {"Segoe UI Black", 1.12},
            {"Segoe UI Emoji", 1.1},
            {"Segoe UI Historic", 1.1},
            {"Segoe UI Light", 1.1},
            {"Segoe UI Semibold", 1.1},
            {"Segoe UI Semilight", 1.1},
            {"Segoe UI Symbol", 1.1},
            {"Times New Roman", 0.95},
            {"Yu Gothic Light", 1.32},
            {"Yu Gothic Medium", 1.32},
            {"Yu Gothic UI Light", 1.09},
            {"Yu Gothic UI Semibold", 1.09},
            {"Yu Gothic UI Semilight", 1.09},
            {"Yu Gothic UI", 1.09},
            {"Yu Gothic", 1.32}
        };
        #endregion

        #region Properties
        public bool ClearOnFinish { get; set; } = true;

        public bool RenderStyles { get; set; } = true;

        public ArrayList Styles { get; set; }

        /* Use export service for HTML viewer */
        public bool RenderWebViewer { get; set; }

        /* Render interaction parameters for HTML viewer */
        public bool RenderWebInteractions { get; set; }

        /// <summary>
        /// Internal use only.
        /// </summary>
        public StiHtmlImageHost HtmlImageHost { get; set; }

        /// <summary>
        /// Internal use only.
        /// </summary>
        public double TotalPageWidth { get; set; }

        /// <summary>
        /// Internal use only.
        /// </summary>
        public double TotalPageHeight { get; set; }

        /// <summary>
        /// Internal use only.
        /// </summary>
        public bool RenderAsDocument { get; set; } = true;

        internal bool RemoveEmptySpaceAtBottom { get; private set; } = StiOptions.Export.Html.RemoveEmptySpaceAtBottom;

        internal StiHorAlignment PageHorAlignment { get; private set; } = StiHorAlignment.Center;
        #endregion

        #region Methods
        /// <summary>
        /// Internal use only.
        /// </summary>
        /// <param name="rect">A rectangle.</param>
        public void AddCoord(RectangleD rect)
        {
            AddCoord(rect.Left, rect.Top);
            AddCoord(rect.Right, rect.Bottom);
        }

        private void AddCoord(double x, double y)
        {
            coordX[x] = x;
            coordY[y] = y;
        }

        private void FormatCoords(StiReport report)
        {
            var newCoordX = new SortedList();
            foreach (double key in coordX.Keys)
            {
                newCoordX[key] = (double)Math.Round((decimal)(report.Unit.ConvertToHInches((double)coordX[key]) * zoom * hiToPt), 2);
            }
            coordX = newCoordX;

            var newCoordY = new SortedList();
            foreach (double key in coordY.Keys)
            {
                newCoordY[key] = (double)Math.Round((decimal)(report.Unit.ConvertToHInches((double)coordY[key]) * zoom * hiToPt), 2);
            }
            coordY = newCoordY;
        }

        private string FormatCoord(double value)
        {
            return Math.Round((decimal)value, defaultCoordinatesPrecision).ToString(numberFormat) + "pt";
        }

        internal static string FormatColor(Color color)
        {
            if (color.A < 255 && color.A > 0)
            {
                return string.Format("rgba({0},{1},{2},{3})",
                    color.R,
                    color.G,
                    color.B,
                    Math.Round(color.A / 255f, 3));
            }
            return ColorTranslator.ToHtml(color);
        }

        internal static double GetFontHeightCorrection(string font)
        {
            object cf = fontsToCorrectHeight[font];
            return cf == null ? 1 : (double)cf;
        }

        internal string GetBorderStyle(StiPenStyle style)
        {
            switch (style)
            {
                case StiPenStyle.Dot:
                    return " dotted";

                case StiPenStyle.Dash:
                case StiPenStyle.DashDot:
                case StiPenStyle.DashDotDot:
                    return " dashed";

                case StiPenStyle.Double:
                    return " double";

                default:
                    return " solid";
            }
        }

        internal void SetCurrentCulture()
        {
            storedCulture = Thread.CurrentThread.CurrentCulture;
            if (StiOptions.Export.Html.ForcedCultureForCharts != null)
            {
                Thread.CurrentThread.CurrentCulture = StiOptions.Export.Html.ForcedCultureForCharts;
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        internal void RestoreCulture()
        {
            Thread.CurrentThread.CurrentCulture = storedCulture;
        }

        #region Renders
        internal void RenderFont(StiHtmlTableCell cell, Font font)
        {
            string fontStr = "";
            if (font.Bold) fontStr += "bold ";
            if (font.Italic) fontStr += "italic ";
            fontStr += Math.Round(font.SizeInPoints * zoom, 2).ToString().Replace(",", ".");
            var fontName = font.FontFamily.GetName(0);
            if (fontName.Contains(" ")) fontName = "'" + fontName + "'";
            fontStr += "pt " + fontName;

            string decoration = (font.Underline ? (font.Strikeout ? "underline line-through" : "underline") : (font.Strikeout ? "line-through" : null));

            if (cell == null)
            {
                HtmlWriter.WriteStyleAttribute("Font", fontStr);
                if (font.Underline || font.Strikeout) HtmlWriter.WriteStyleAttribute("text-decoration", decoration);
            }
            else
            {
                cell.Style["Font"] = fontStr;
                if (font.Underline || font.Strikeout) cell.Style["text-decoration"] = decoration;
            }
        }


        //internal void RenderTextHorAlignment(StiHtmlTableCell cell, StiCellStyle style)
        internal void RenderTextHorAlignment(StiHtmlTableCell cell, StiTextOptions textOptions, StiTextHorAlignment textHorAlignment)
        {
            bool rightToLeft = textOptions != null && textOptions.RightToLeft;
            string align = string.Empty;
            if (textHorAlignment == StiTextHorAlignment.Left)
            {
                align = (!rightToLeft ? "left" : "right");
            }
            if (textHorAlignment == StiTextHorAlignment.Right)
            {
                align = (rightToLeft ? "left" : "right");
            }
            if (textHorAlignment == StiTextHorAlignment.Center) align = "center";
            if (textHorAlignment == StiTextHorAlignment.Width) align = "justify";

            if (align != string.Empty)
            {
                if (cell == null)
                {
                    HtmlWriter.WriteStyleAttribute("text-align", align);
                    if (exportMode != StiHtmlExportMode.Table)
                    {
                        if (align == "left") HtmlWriter.WriteStyleAttribute("justify-content", "flex-start");
                        if (align == "right") HtmlWriter.WriteStyleAttribute("justify-content", "flex-end");
                        if (align == "center") HtmlWriter.WriteStyleAttribute("justify-content", "center");
                        if (align == "justify") HtmlWriter.WriteStyleAttribute("justify-content", "space-between");
                    }
                }
                else
                {
                    cell.Style["text-align"] = align;
                }
            }
        }

        //internal void RenderVertAlignment(StiHtmlTableCell cell, StiCellStyle style)
        internal void RenderVertAlignment(StiHtmlTableCell cell, StiVertAlignment textVertAlignment, StiTextOptions textOptions = null, bool allowHtmlTags = false)
        {
            bool textEllipsis = (textOptions != null) && (textOptions.Trimming != StringTrimming.None) && !textOptions.WordWrap;

            string align = null;
            if (textVertAlignment == StiVertAlignment.Top) align = "top";
            if (textVertAlignment == StiVertAlignment.Center) align = "middle";
            if (textVertAlignment == StiVertAlignment.Bottom) align = "bottom";
            if (!string.IsNullOrEmpty(align))
            {
                if (cell == null)
                {
                    if (exportMode == StiHtmlExportMode.Table || allowHtmlTags)
                    {
                        HtmlWriter.WriteStyleAttribute("vertical-align", align);
                    }
                    else
                    {
                        if (textVertAlignment == StiVertAlignment.Top) align = "flex-start";
                        if (textVertAlignment == StiVertAlignment.Center) align = "center";
                        if (textVertAlignment == StiVertAlignment.Bottom) align = "flex-end";

                        if (textEllipsis)
                        {
                            HtmlWriter.WriteStyleAttribute("display", "block");
                            HtmlWriter.WriteStyleAttribute("white-space", "nowrap");
                        }
                        else
                        {
                            HtmlWriter.WriteStyleAttribute("display", "flex");
                        }
                        HtmlWriter.WriteStyleAttribute("align-items", align);
                    }
                }
                else
                {
                    cell.Style["vertical-align"] = align;
                }
            }
        }

        //		private void RenderTextHorAlignment(StiComponent comp)
        //		{
        //			if (comp is IStiTextHorAlignment)
        //			{
        //				IStiTextHorAlignment align = comp as IStiTextHorAlignment;
        //				RenderTextHorAlignment(align.HorAlignment);
        //			}
        //		}

        //internal void RenderTextAngle(StiTextOptions textOptions)
        //{
        //    if (textOptions != null &&
        //        (textOptions.Angle == 90f ||
        //        textOptions.Angle == 270f)) HtmlWriter.WriteStyleAttribute("writing-mode", "tb-rl");
        //}

        internal void RenderTextDirection(StiHtmlTableCell cell, StiTextOptions textOptions)
        {
            if (textOptions != null &&
                (textOptions.RightToLeft))
            {
                if (cell == null)
                {
                    HtmlWriter.WriteStyleAttribute("direction", "rtl");
                }
                else
                {
                    cell.Style["direction"] = "rtl";
                }
            }
        }


        //private void RenderBackColor(StiComponent comp)
        //{
        //    if (comp is IStiBrush)
        //    {
        //        IStiBrush brush = comp as IStiBrush;
        //        Color color = StiBrush.ToColor(brush.Brush);
        //        RenderBackColor(null, color);
        //    }
        //}


        internal void RenderBackColor(StiHtmlTableCell cell, Color color)
        {
            if (color.A > 0)
            {
                if (cell == null)
                {
                    HtmlWriter.WriteStyleAttribute("background-color", FormatColor(color));
                }
                else
                {
                    cell.Style["background-color"] = FormatColor(color);
                }
            }
        }


        //private void RenderTextColor(StiComponent comp)
        //{
        //    if (comp is IStiTextBrush)
        //    {
        //        IStiTextBrush textBrush = comp as IStiTextBrush;
        //        Color color = StiBrush.ToColor(textBrush.TextBrush);
        //        RenderTextColor(null, color);
        //    }
        //}


        internal void RenderTextColor(StiHtmlTableCell cell, Color color, bool forceAnyColor = false)
        {
            if ((color != Color.Black) || forceAnyColor)
            {
                if (cell == null)
                {
                    HtmlWriter.WriteStyleAttribute("color", FormatColor(color));
                }
                else
                {
                    cell.Style["color"] = FormatColor(color);
                }
            }
        }


        private void RenderBorder(StiComponent comp)
        {
            if (comp is IStiBorder && (!(comp is IStiIgnoreBorderWhenExport)))
            {
                var border = comp as IStiBorder;
                RenderBorder(border.Border);
            }
        }


        internal void RenderBorder(StiBorder border)
        {
            if (border != null)
            {
                StiBorderSide borderL = null;
                StiBorderSide borderR = null;
                StiBorderSide borderT = null;
                StiBorderSide borderB = null;
                StiAdvancedBorder advBorder = border as StiAdvancedBorder;
                if (advBorder != null)
                {
                    borderL = advBorder.LeftSide;
                    borderR = advBorder.RightSide;
                    borderT = advBorder.TopSide;
                    borderB = advBorder.BottomSide;
                }
                else
                {
                    borderL = new StiBorderSide(border.Color, border.Size, border.Style);
                    if (border.IsRightBorderSidePresent) borderR = borderL;
                    if (border.IsTopBorderSidePresent) borderT = borderL;
                    if (border.IsBottomBorderSidePresent) borderB = borderL;
                    if (!border.IsLeftBorderSidePresent) borderL = null;
                }
                RenderBorder(null, borderL, "left");
                RenderBorder(null, borderR, "right");
                RenderBorder(null, borderT, "top");
                RenderBorder(null, borderB, "bottom");
            }
        }


        internal void RenderBorder(StiHtmlTableCell cell, StiBorderSide border, string side)
        {
            if ((border != null) && (border.Style != StiPenStyle.None))
            {
                string color = FormatColor(border.Color);
                string style = GetBorderStyle(border.Style);
                double sizeD = border.Size;
                if (sizeD > 0 && sizeD < 1) sizeD = 1;
                if (sizeD < 1 &&
                    (border.Style == StiPenStyle.Dash ||
                    border.Style == StiPenStyle.DashDot ||
                    border.Style == StiPenStyle.DashDotDot ||
                    border.Style == StiPenStyle.Dot))
                {
                    sizeD = 1;
                }
                if (border.Style == StiPenStyle.Double)
                {
                    sizeD = 2.5;
                }
                string size = Math.Round(sizeD, MidpointRounding.AwayFromZero).ToString() + "px";

                if (cell == null)
                {
                    HtmlWriter.WriteStyleAttribute(string.Format("border-{0}-color", side), color);
                    HtmlWriter.WriteStyleAttribute(string.Format("border-{0}-style", side), style);
                    HtmlWriter.WriteStyleAttribute(string.Format("border-{0}-width", side), size);
                }
                else
                {
                    cell.Style[string.Format("border-{0}-color", side)] = color;
                    cell.Style[string.Format("border-{0}-style", side)] = style;
                    cell.Style[string.Format("border-{0}-width", side)] = size;
                }
            }
            else
            {
                if (StiOptions.Export.Html.UseStrictTableCellSize && (cell == null))
                {
                    HtmlWriter.WriteStyleAttribute(string.Format("border-{0}-color", side), "transparent");
                    HtmlWriter.WriteStyleAttribute(string.Format("border-{0}-style", side), "solid");
                    HtmlWriter.WriteStyleAttribute(string.Format("border-{0}-width", side), "1px");
                }
            }
        }


        private string RenderPosition(StiComponent comp, bool returnSize = false)
        {
            var rect = comp.ComponentToPage(comp.ClientRectangle);

            var leftD = (double)coordX[rect.Left];
            var topD = (double)coordY[rect.Top] + startPage * zoom * hiToPt;
            var widthD = (double)coordX[rect.Right] - (double)coordX[rect.Left];
            var heightD = (double)coordY[rect.Bottom] - (double)coordY[rect.Top];

            //correction for border width
            var bord = comp as IStiBorder;
            if (bord != null && bord.Border != null)
            {
                var advBorder = bord.Border as StiAdvancedBorder;
                if (advBorder != null)
                {
                    if (advBorder.IsLeftBorderSidePresent)
                    {
                        double offset = advBorder.LeftSide.GetSizeOffset() * pxToPt;
                        leftD -= offset;
                        widthD -= offset;
                    }
                    if (advBorder.IsRightBorderSidePresent) widthD -= advBorder.RightSide.GetSizeOffset() * pxToPt;
                    if (advBorder.IsTopBorderSidePresent)
                    {
                        double offset = advBorder.TopSide.GetSizeOffset() * pxToPt;
                        topD -= offset;
                        heightD -= offset;
                    }
                    if (advBorder.IsBottomBorderSidePresent) heightD -= advBorder.BottomSide.GetSizeOffset() * pxToPt;
                }
                else
                {
                    if ((bord.Border.Style != StiPenStyle.None) && (bord.Border.Size > 0) && (bord.Border.Side != StiBorderSides.None))
                    {
                        double offset = bord.Border.GetSizeOffset() * pxToPt;
                        if (bord.Border.IsLeftBorderSidePresent)
                        {
                            leftD -= offset;
                            widthD -= offset;
                        }
                        if (bord.Border.IsRightBorderSidePresent) widthD -= offset;
                        if (bord.Border.IsTopBorderSidePresent)
                        {
                            topD -= offset;
                            heightD -= offset;
                        }
                        if (bord.Border.IsBottomBorderSidePresent) heightD -= offset;
                    }
                }
            }

            //correction for padding
            var text = comp as StiText;
            if (text != null && (!text.Margins.IsEmpty))
            {
                widthD -= (Math.Truncate(text.Margins.Left * zoom) + Math.Truncate(text.Margins.Right * zoom)) * pxToPt;
                heightD -= (Math.Truncate(text.Margins.Top * zoom) + Math.Truncate(text.Margins.Bottom * zoom)) * pxToPt;
            }

            if (widthD < 0) widthD = 0;
            if (heightD < 0) heightD = 0;

            var left = FormatCoord(leftD);
            var top = FormatCoord(topD);
            var width = FormatCoord(widthD);
            var height = FormatCoord(heightD);

            HtmlWriter.WriteStyleAttribute("left", left);
            HtmlWriter.WriteStyleAttribute("top", top);
            HtmlWriter.WriteStyleAttribute("width", width);
            HtmlWriter.WriteStyleAttribute("height", height);

            if (text != null && (!text.Margins.IsEmpty))
            {
                HtmlWriter.WriteStyleAttribute("padding", string.Format("{0} {1} {2} {3}",
                   StiHtmlUnit.NewUnit(Math.Truncate(text.Margins.Top * zoom)).ToString(),
                   StiHtmlUnit.NewUnit(Math.Truncate(text.Margins.Right * zoom)).ToString(),
                   StiHtmlUnit.NewUnit(Math.Truncate(text.Margins.Bottom * zoom)).ToString(),
                   StiHtmlUnit.NewUnit(Math.Truncate(text.Margins.Left * zoom)).ToString()));
            }
            if (comp is StiShape)
            {
                HtmlWriter.WriteStyleAttribute("overflow", "visible");
            }

            if (returnSize) return $"{width};{height}";
            return null;
        }

        private struct SizeStrings
        {
            public string Width;
            public string Height;
            public double DWidth;
            public double DHeight;
        }

        private SizeStrings GetSizeStrings(StiComponent comp)
        {
            var rect = comp.ComponentToPage(comp.ClientRectangle);

            var widthD = (double)coordX[rect.Right] - (double)coordX[rect.Left];
            var heightD = (double)coordY[rect.Bottom] - (double)coordY[rect.Top];

            //correction for border width
            var bord = comp as IStiBorder;
            if (bord != null && bord.Border != null)
            {
                var advBorder = bord.Border as StiAdvancedBorder;
                if (advBorder != null)
                {
                    if (advBorder.IsLeftBorderSidePresent) widthD -= advBorder.LeftSide.GetSizeOffset() * pxToPt;
                    if (advBorder.IsRightBorderSidePresent) widthD -= advBorder.RightSide.GetSizeOffset() * pxToPt;
                    if (advBorder.IsTopBorderSidePresent) heightD -= advBorder.TopSide.GetSizeOffset() * pxToPt;
                    if (advBorder.IsBottomBorderSidePresent) heightD -= advBorder.BottomSide.GetSizeOffset() * pxToPt;
                }
                else
                {
                    if ((bord.Border.Style != StiPenStyle.None) && (bord.Border.Size > 0) && (bord.Border.Side != StiBorderSides.None))
                    {
                        double offset = bord.Border.GetSizeOffset() * pxToPt;
                        if (bord.Border.IsLeftBorderSidePresent) widthD -= offset;
                        if (bord.Border.IsRightBorderSidePresent) widthD -= offset;
                        if (bord.Border.IsTopBorderSidePresent) heightD -= offset;
                        if (bord.Border.IsBottomBorderSidePresent) heightD -= offset;
                    }
                }
            }

            //correction for padding
            var text = comp as StiText;
            if (text != null && (!text.Margins.IsEmpty))
            {
                widthD -= (Math.Truncate(text.Margins.Left * zoom) + Math.Truncate(text.Margins.Right * zoom)) * pxToPt;
                heightD -= (Math.Truncate(text.Margins.Top * zoom) + Math.Truncate(text.Margins.Bottom * zoom)) * pxToPt;
            }

            if (widthD < 0) widthD = 0;
            if (heightD < 0) heightD = 0;

            var result = new SizeStrings();
            result.Width = FormatCoord(widthD);
            result.Height = FormatCoord(heightD);
            result.DWidth = widthD;
            result.DHeight = heightD;

            return result;
        }

        private void RenderImage(StiComponent comp)
        {
            string imageURL = null;
            var image = comp as StiImage;
            if (image != null)
            {
                imageURL = image.ImageURLValue as string;
            }
            if (!string.IsNullOrEmpty(imageURL) && !(imageURL.StartsWith("http") || imageURL.StartsWith("ftp")))
            {
                imageURL = null;
            }

            if (StiSvgHelper.CheckShape(comp))
            {
                RenderShape(comp, (float)zoom);
            }
            else if (image != null && image.Icon != null)
            {
                RenderIcon(image, (float)zoom);
            }
            else
            {
                SetCurrentCulture();
                RenderImage(comp, imageURL);
                RestoreCulture();
            }
        }

        private void RenderImage(StiComponent component, string imageURL)
        {
            IStiExportImage exportImage = component as IStiExportImage;
            if (exportImage != null)
            {
                IStiExportImageExtended exportImageExtended = exportImage as IStiExportImageExtended;

                var zoom = (float)this.zoom;

                float resolution = imageResolution;
                var imageComp = exportImage as StiImage;
                if (StiOptions.Export.Html.UseImageResolution && imageComp != null && imageComp.ExistImageToDraw())
                {
                    using (var gdiImage = imageComp.TryTakeGdiImageToDraw())
                    {
                        var dpix = gdiImage.HorizontalResolution;
                        if (dpix >= 50 && dpix <= 1250) resolution = dpix;
                    }
                }
                if (resolution != 100) zoom *= resolution / 100f;

                Image image;

                if (exportImageExtended != null)
                {
                    if (exportImageExtended.IsExportAsImage(StiExportFormat.Html))
                    {
                        // For StiImage use StiExportFormat.HtmlTable (to get raw "unmargined" image - it will be transformed via html styles)
                        if (imageComp != null)
                            image = exportImageExtended.GetImage(ref zoom, StiExportFormat.HtmlTable);
                        else if (imageFormat == null || imageFormat == ImageFormat.Png)
                            image = exportImageExtended.GetImage(ref zoom, StiExportFormat.ImagePng);
                        else if (strSpanDiv == "span")
                            image = exportImageExtended.GetImage(ref zoom, StiExportFormat.HtmlSpan);
                        else
                            image = exportImageExtended.GetImage(ref zoom, StiExportFormat.HtmlDiv);
                    }
                    else return;
                }
                else image = exportImage.GetImage(ref zoom);

                RestoreCulture();
                var shape = exportImage as StiShape;
                RenderImage(image, component, imageURL, zoom, shape != null ? -(int)Math.Round(shape.Size * zoom / 2) : 0);
                if (image != null) image.Dispose();
            }
        }

        //private bool ForceExportAsImage(object exportImage)
        //{
        //    var textOptions = exportImage as IStiTextOptions;
        //    return textOptions != null && textOptions.TextOptions.Angle != 0;
        //}

        private void RenderImage(Image image, StiComponent component, string imageURL, float zoom, int margin)
        {
            if (image != null)
            {
                string imageString = imageURL;

                if (string.IsNullOrEmpty(imageURL))
                {
                    if (HtmlImageHost != null) imageString = HtmlImageHost.GetImageString(image as Bitmap);
                    if (imageString == null) imageString = string.Empty;
                }

                double scale = StiOptions.Export.Html.PrintLayoutOptimization ? 0.96 : 1;

                var stiImage = component as StiImage;
                if (stiImage != null)
                {
                    var resSize = GetSizeStrings(stiImage);
                    bool hasMargins = !stiImage.Margins.IsEmpty;
                    string marginsText = null;
                    if (hasMargins)
                    {
                        // Assume that sizes are in pt and margins are in hundredths of an inch (which are assumed to be equal to px)
                        int marginLeft = (int)Math.Truncate(stiImage.Margins.Left * zoom * pxToPt);
                        int marginRight = (int)Math.Truncate(stiImage.Margins.Right * zoom * pxToPt);
                        int marginTop = (int)Math.Truncate(stiImage.Margins.Top * zoom * pxToPt);
                        int marginBottom = (int)Math.Truncate(stiImage.Margins.Bottom * zoom * pxToPt);
                        resSize.DWidth -= (marginLeft + marginRight);
                        resSize.Width = FormatCoord(resSize.DWidth);
                        resSize.DHeight -= (marginTop + marginBottom);
                        resSize.Height = FormatCoord(resSize.DHeight);
                        marginsText = $"{marginTop}pt {marginRight}pt {marginBottom}pt {marginLeft}pt";
                    }

                    var imageRotation = stiImage.ImageRotation;
                    if (string.IsNullOrEmpty(imageURL) && Base.Helpers.StiSvgHelper.IsSvg(stiImage.ImageBytesToDraw)) imageRotation = StiImageRotation.None;   //fix for painted svg

                    if (stiImage.AspectRatio && stiImage.Stretch || imageRotation != StiImageRotation.None)
                    {
                        HtmlWriter.Write("<div style=\"");
                        HtmlWriter.WriteStyleAttribute("width", resSize.Width);
                        HtmlWriter.WriteStyleAttribute("height", resSize.Height);
                        if (hasMargins) HtmlWriter.WriteStyleAttribute("margin", marginsText);

                        var horAlignment = stiImage.HorAlignment;
                        var vertAlignment = stiImage.VertAlignment;
                        switch (imageRotation)
                        {
                            case StiImageRotation.Rotate90CW:
                                if (stiImage.HorAlignment == StiHorAlignment.Left) vertAlignment = StiVertAlignment.Bottom;
                                else if (stiImage.HorAlignment == StiHorAlignment.Right) vertAlignment = StiVertAlignment.Top;
                                else vertAlignment = StiVertAlignment.Center;
                                if (stiImage.VertAlignment == StiVertAlignment.Top) horAlignment = StiHorAlignment.Left;
                                else if (stiImage.VertAlignment == StiVertAlignment.Bottom) horAlignment = StiHorAlignment.Right;
                                else horAlignment = StiHorAlignment.Center;
                                break;

                            case StiImageRotation.Rotate90CCW:
                                if (stiImage.HorAlignment == StiHorAlignment.Left) vertAlignment = StiVertAlignment.Top;
                                else if (stiImage.HorAlignment == StiHorAlignment.Right) vertAlignment = StiVertAlignment.Bottom;
                                else vertAlignment = StiVertAlignment.Center;
                                if (stiImage.VertAlignment == StiVertAlignment.Top) horAlignment = StiHorAlignment.Right;
                                else if (stiImage.VertAlignment == StiVertAlignment.Bottom) horAlignment = StiHorAlignment.Left;
                                else horAlignment = StiHorAlignment.Center;
                                break;

                            case StiImageRotation.Rotate180:
                                if (stiImage.HorAlignment == StiHorAlignment.Left) horAlignment = StiHorAlignment.Right;
                                else if (stiImage.HorAlignment == StiHorAlignment.Right) horAlignment = StiHorAlignment.Left;
                                if (stiImage.VertAlignment == StiVertAlignment.Top) vertAlignment = StiVertAlignment.Bottom;
                                else if (stiImage.VertAlignment == StiVertAlignment.Bottom) vertAlignment = StiVertAlignment.Top;
                                break;

                            case StiImageRotation.FlipHorizontal:
                                if (stiImage.HorAlignment == StiHorAlignment.Left) horAlignment = StiHorAlignment.Right;
                                else if (stiImage.HorAlignment == StiHorAlignment.Right) horAlignment = StiHorAlignment.Left;
                                break;

                            case StiImageRotation.FlipVertical:
                                if (stiImage.VertAlignment == StiVertAlignment.Top) vertAlignment = StiVertAlignment.Bottom;
                                else if (stiImage.VertAlignment == StiVertAlignment.Bottom) vertAlignment = StiVertAlignment.Top;
                                break;
                        }

                        if (imageRotation == StiImageRotation.Rotate90CCW || imageRotation == StiImageRotation.Rotate90CW)
                        {
                            HtmlWriter.Write("\"><div style=\"");
                            HtmlWriter.WriteStyleAttribute("width", resSize.Height);
                            HtmlWriter.WriteStyleAttribute("height", resSize.Width);
                            var offs = (resSize.DWidth - resSize.DHeight) / 2;
                            HtmlWriter.WriteStyleAttribute("position", "relative");
                            HtmlWriter.WriteStyleAttribute("left", $"{offs}pt");
                            HtmlWriter.WriteStyleAttribute("top", $"{-offs}pt");

                            HtmlWriter.WriteStyleAttribute("transform", "rotate(" + (imageRotation == StiImageRotation.Rotate90CCW ? "-" : "") + "90deg)");
                        }

                        HtmlWriter.WriteStyleAttribute("background-repeat", "no-repeat");
                        HtmlWriter.WriteStyleAttribute("background-position", horAlignment.ToString().ToLower() + " " + vertAlignment.ToString().ToLower());
                        HtmlWriter.WriteStyleAttribute("background-image", "url(" + StiHtmlTable.StringToUrl(imageString) + ")");

                        var imgWidth = image.Width * stiImage.MultipleFactor * this.zoom * dpi96;
                        var imgHeight = image.Height * stiImage.MultipleFactor * this.zoom * dpi96;
                        HtmlWriter.WriteStyleAttribute("background-size", stiImage.Stretch ? (stiImage.AspectRatio ? "contain" : "100% 100%") : $"{imgWidth}px {imgHeight}px");
                        HtmlWriter.WriteStyleAttribute("-webkit-print-color-adjust", "exact");
                        HtmlWriter.WriteStyleAttribute("color-adjust", "exact");

                        if (imageRotation == StiImageRotation.FlipHorizontal) HtmlWriter.WriteStyleAttribute("transform", "scaleX(-1)");
                        else if (imageRotation == StiImageRotation.FlipVertical) HtmlWriter.WriteStyleAttribute("transform", "scaleY(-1)");
                        else if (imageRotation == StiImageRotation.Rotate180) HtmlWriter.WriteStyleAttribute("transform", "scale(-1)");

                        if (imageRotation == StiImageRotation.Rotate90CCW || imageRotation == StiImageRotation.Rotate90CW) HtmlWriter.Write("\"></div>");
                        else HtmlWriter.Write("\">");

                        HtmlWriter.Write("</div>");
                    }
                    else
                    {
                        HtmlWriter.WriteBeginTag("div style=\"");
                        HtmlWriter.WriteStyleAttribute("display", "table-cell");
                        HtmlWriter.WriteStyleAttribute("max-width", resSize.Width);
                        HtmlWriter.WriteStyleAttribute("width", resSize.Width);
                        HtmlWriter.WriteStyleAttribute("max-height", resSize.Height);
                        HtmlWriter.WriteStyleAttribute("text-align", stiImage.HorAlignment.ToString().ToLower());
                        HtmlWriter.WriteStyleAttribute("vertical-align", stiImage.VertAlignment.ToString().ToLower().Replace("center", "middle"));
                        HtmlWriter.WriteStyleAttribute("line-height", "0");
                        if (hasMargins) HtmlWriter.WriteStyleAttribute("padding", marginsText);
                        HtmlWriter.Write("\">");

                        int imgX = 0;
                        int imgY = 0;
                        int imgWidth = (int)(image.Width * stiImage.MultipleFactor * this.zoom * dpi96);
                        int imgHeight = (int)(image.Height * stiImage.MultipleFactor * this.zoom * dpi96);
                        double resWidth = resSize.DWidth / hiToPt * dpi96;
                        double resHeight = resSize.DHeight / hiToPt * dpi96;
                        if (imgWidth > resWidth)
                        {
                            switch (stiImage.HorAlignment)
                            {
                                case StiHorAlignment.Center:
                                    imgX = (int)(resWidth / 2 - imgWidth / 2);
                                    break;
                                case StiHorAlignment.Right:
                                    imgX = (int)(resWidth - imgWidth);
                                    break;
                            }
                        }
                        if (imgHeight > resHeight)
                        {
                            switch (stiImage.VertAlignment)
                            {
                                case StiVertAlignment.Center:
                                    imgY = (int)(resHeight / 2 - imgHeight / 2);
                                    break;
                                case StiVertAlignment.Bottom:
                                    imgY = (int)(resHeight - imgHeight);
                                    break;
                            }
                        }

                        HtmlWriter.WriteBeginTag("img style=\"");
                        if (stiImage.Stretch)
                        {
                            HtmlWriter.WriteStyleAttribute("width", resSize.Width);
                            HtmlWriter.WriteStyleAttribute("height", resSize.Height);
                        }
                        else
                        {
                            HtmlWriter.WriteStyleAttribute("width", $"{imgWidth}px");
                            HtmlWriter.WriteStyleAttribute("height", $"{imgHeight}px");
                            if (imgX != 0)
                                HtmlWriter.WriteStyleAttribute("margin-left", $"{imgX}px");
                            if (imgY != 0)
                                HtmlWriter.WriteStyleAttribute("margin-top", $"{imgY}px");
                        }
                        HtmlWriter.Write("\"");

                        HtmlWriter.WriteAttribute("src", imageString);

                        HtmlWriter.Write(">");
                        HtmlWriter.WriteEndTag("img");
                        HtmlWriter.WriteEndTag("div");
                    }
                }
                else
                {
                    HtmlWriter.WriteBeginTag("img ");
                    HtmlWriter.Write("style=\"width:" + ((int)(image.Width / zoom * this.zoom * scale)).ToString() + "px;");
                    if (margin != 0)
                    {
                        HtmlWriter.Write("margin:" + margin + "px;");
                    }
                    HtmlWriter.Write("height:" + ((int)(image.Height / zoom * this.zoom * scale)).ToString() + "px;border:0px\"");
                    HtmlWriter.WriteAttribute("src", imageString);
                    HtmlWriter.Write(">");
                    HtmlWriter.WriteEndTag("img");
                }
            }
        }

        private void RenderIcon(StiImage image, float zoom)
        {
            if (image == null) return;

            var resSize = GetSizeStrings(image);
            double baseWidth = Math.Round(resSize.DWidth / pxToPt, 2);
            double baseHeight = Math.Round(resSize.DHeight / pxToPt, 2);

            string svg = StiSvgHelper.RenderIcon(image, zoom, baseWidth, baseHeight);

            HtmlWriter.Write(svg);
        }
        
        private void RenderShape(StiComponent component, float zoom)
        {
            var shape = component as StiShape;
            if (shape == null) return;

            double scale = StiOptions.Export.Html.PrintLayoutOptimization ? 0.96 : 1;

            var rect = component.ComponentToPage(component.ClientRectangle).Normalize();
            rect = shape.Report.Unit.ConvertToHInches(rect).Multiply(zoom);
            rect.X = 0;
            rect.Y = 0;
            int imageWidth = (int)(rect.Width + 0.5) + 1 + (int)(shape.Size * zoom);
            int imageHeight = (int)(rect.Height + 0.5) + 1 + (int)(shape.Size * zoom);
            int shift = (int)Math.Round(shape.Size * zoom / 2);

            HtmlWriter.WriteBeginTag("img ");
            HtmlWriter.Write("style=\"width:" + ((int)(imageWidth * scale)).ToString() + "px;");
            HtmlWriter.Write("height:" + ((int)(imageHeight * scale)).ToString() + "px;border:0px\"");
            if (shift != 0)
            {
                HtmlWriter.Write("margin:" + shift + "px;");
            }
            HtmlWriter.WriteAttribute("src", StiSvgHelper.RenderShapeAsBase64(component));
            HtmlWriter.Write(">");
            HtmlWriter.WriteEndTag("img");
        }

        private bool RenderHyperlink(StiComponent comp)
        {
            string hyperlinkValue = comp.HyperlinkValue as string;
            if (!string.IsNullOrEmpty(hyperlinkValue))
            {
                hyperlinkValue = hyperlinkValue.Trim();
                if (StiOptions.Export.Html.DisableJavascriptInHyperlinks && hyperlinkValue.StartsWith("javascript:")) hyperlinkValue = null;
            }
            if (!string.IsNullOrWhiteSpace(hyperlinkValue) && hyperlinkValue.StartsWith("##"))
            {
                if ((hyperlinkValue.Length > 2) && (hyperlinkValue[2] == '#'))
                {
                    hyperlinkValue = hyperlinkValue.Substring(2);
                    string stt = (string)pointerToBookmark[hyperlinkValue.Substring(1)];
                    if (stt != null) hyperlinkValue = "#" + stt;
                }
                else
                {
                    hyperlinkValue = hyperlinkValue.Substring(1);
                }
            }

            string bookmarkValue = comp.BookmarkValue as string;
            string tag = comp.TagValue as string;
            if (bookmarkValue == null)
            {
                if (!string.IsNullOrEmpty(tag) && hyperlinksToTag.ContainsKey(tag))
                {
                    bookmarkValue = tag;
                }
                else if (!string.IsNullOrWhiteSpace((string)comp.PointerValue) && !string.IsNullOrEmpty(comp.Guid))
                {
                    bookmarkValue = $"{comp.PointerValue}#GUID#{comp.Guid}";
                }
            }

            string bookmarkGuid = null;
            if (!string.IsNullOrWhiteSpace(comp.Guid) && hashBookmarkGuid.ContainsKey(comp.Guid))
            {
                bookmarkGuid = comp.Guid;
            }

            if (!string.IsNullOrWhiteSpace(hyperlinkValue))
            {
                var rect = comp.ComponentToPage(comp.ClientRectangle);
                var style = new StringBuilder();
                
                if (comp is IStiTextBrush)
                {
                    var textBrush = comp as IStiTextBrush;
                    var color = StiBrush.ToColor(textBrush.TextBrush);
                    style.Append("color:" + FormatColor(color) + ";");
                }
                if (comp is IStiFont)
                {
                    var font = comp as IStiFont;
                    if (font.Font.Underline)
                    {
                        style.Append("text-decoration:underline;");
                    }
                    else
                    {
                        style.Append("text-decoration:none;");
                    }
                }
                else
                {
                    string height = FormatCoord((double)coordY[rect.Bottom] - (double)coordY[rect.Top]);
                    style.Append("display:block;height:" + height + ";text-decoration:none;");
                }


                HtmlWriter.WriteBeginTag("a");
                if (!string.IsNullOrWhiteSpace(openLinksTarget)) HtmlWriter.WriteAttribute("target", openLinksTarget);
                if (!string.IsNullOrWhiteSpace(bookmarkValue))
                {
                    HtmlWriter.WriteAttribute("name", bookmarkValue.Replace("'", ""));
                }
                if (!string.IsNullOrWhiteSpace(bookmarkGuid))
                {
                    HtmlWriter.WriteAttribute("guid", bookmarkGuid);
                }
                HtmlWriter.WriteAttribute("style", style.ToString());
                HtmlWriter.WriteAttribute("href", hyperlinkValue);
                HtmlWriter.Write(">");
                return true;
            }
            if (!string.IsNullOrWhiteSpace(bookmarkValue) || !string.IsNullOrWhiteSpace(bookmarkGuid))
            {
                HtmlWriter.WriteBeginTag("a");
                if (!string.IsNullOrWhiteSpace(openLinksTarget)) HtmlWriter.WriteAttribute("target", openLinksTarget);
                if (!string.IsNullOrWhiteSpace(bookmarkValue))
                {
                    HtmlWriter.WriteAttribute("name", bookmarkValue.Replace("'", ""));
                }
                if (!string.IsNullOrWhiteSpace(bookmarkGuid))
                {
                    HtmlWriter.WriteAttribute("guid", bookmarkGuid);
                }
                if (StiOptions.Export.Html.UseExtendedStyle)
                {
                    HtmlWriter.WriteAttribute("class", "sBaseStyleFix");
                }
                HtmlWriter.Write(">");
                return true;
            }
            return false;
        }


        private void RenderPage(StiPagesCollection pages, bool useBookmarksTree, int bookmarkWidth, StiPage page = null, bool writePageBreak = false)
        {
            if (exportSettings.AddPageBreaks)
            {
                HtmlWriter.WriteBeginTag("div");
                HtmlWriter.WriteAttribute("class", "pagemargins");
                if (writePageBreak)
                {
                    HtmlWriter.Write(" style=\"");
                    HtmlWriter.WriteStyleAttribute("page-break-before", "always");
                    HtmlWriter.Write("\"");
                }
                HtmlWriter.WriteLine(">");
                //HtmlWriter.Indent++;
            }

            if ((PageHorAlignment != StiHorAlignment.Left) && (!exportSettings.AddPageBreaks))
            {
                HtmlWriter.WriteBeginTag(strSpanDiv + " style=\"");
                HtmlWriter.WriteStyleAttribute("text-align", (PageHorAlignment == StiHorAlignment.Center ? "center" : "right"));
                if (useBookmarksTree)
                {
                    HtmlWriter.WriteStyleAttribute("margin-left", string.Format("{0}px", bookmarkWidth + 4));
                }
                HtmlWriter.Write("\">");
                HtmlWriter.Indent++;
                HtmlWriter.WriteLine();
            }

            HtmlWriter.WriteBeginTag(strSpanDiv + " class=\"StiPageContainer\" style=\"");
            if (RenderAsDocument)
            {
                if (PageHorAlignment != StiHorAlignment.Left)
                {
                    HtmlWriter.WriteStyleAttribute("display", "inline-block");
                }
                else
                {
                    if (useBookmarksTree) HtmlWriter.WriteStyleAttribute("left", string.Format("{0}px", bookmarkWidth + 4));
                }

                if (page != null)
                {
                    double pageWidth = report.Unit.ConvertToHInches(page.PageWidth - page.Margins.Left - page.Margins.Right);
                    double pageHeight = report.Unit.ConvertToHInches(page.PageHeight - page.Margins.Top - page.Margins.Bottom);

                    HtmlWriter.WriteStyleAttribute("width", FormatCoord(pageWidth * zoom * hiToPt));
                    HtmlWriter.WriteStyleAttribute("height", FormatCoord(pageHeight * zoom * hiToPt));
                }
                else
                {
                    HtmlWriter.WriteStyleAttribute("width", FormatCoord(TotalPageWidth * zoom * hiToPt));
                    HtmlWriter.WriteStyleAttribute("height", FormatCoord(TotalPageHeight * zoom * hiToPt));
                }
                HtmlWriter.WriteStyleAttribute("position", "relative");
                HtmlWriter.WriteStyleAttribute("white-space", "normal");

                var backColor = Color.Transparent;
                if ((pages != null) && (pages.Count > 0))
                {
                    if (pages[0].Brush != null)
                    {
                        backColor = StiBrush.ToColor(pages[0].Brush);
                    }
                    if (backColor.A == 0 && !(pages[0].Brush is StiEmptyBrush))
                    {
                        backColor = Color.White;
                    }
                    if (pages[0].Border != null)
                    {
                        RenderBorder(pages[0]);
                    }
                }
                HtmlWriter.WriteStyleAttribute("background-color", FormatColor(backColor));

                string imagePath = GetBackgroundImagePath(pages, zoom, exportSettings.UseWatermarkMargins);
                if (!string.IsNullOrWhiteSpace(imagePath))
                {
                    HtmlWriter.WriteStyleAttribute("background-image", string.Format("url('{0}')", imagePath));
                }
            }
            HtmlWriter.Write("\">");
            HtmlWriter.Indent++;
            HtmlWriter.WriteLine();
        }


        private void RenderEndPage()
        {
            HtmlWriter.Indent--;
            HtmlWriter.WriteEndTag(strSpanDiv);
            HtmlWriter.WriteLine();

            if ((PageHorAlignment != StiHorAlignment.Left) && (!exportSettings.AddPageBreaks))
            {
                HtmlWriter.Indent--;
                HtmlWriter.WriteEndTag(strSpanDiv);
                HtmlWriter.WriteLine();
            }

            if (exportSettings.AddPageBreaks)
            {
                HtmlWriter.WriteLine();
                //HtmlWriter.Indent--;
                HtmlWriter.WriteFullEndTag("div");
            }
        }


        private void RenderStartDoc(StiHtmlTableRender render, bool asTable, bool useBookmarks, bool exportBookmarksOnly, bool useStimulsoftFont, Hashtable cssStyles, StiPagesCollection pages, Encoding encoding)
        {
            HtmlWriter.WriteLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">");
            HtmlWriter.Write("<html xmlns=\"http://www.w3.org/1999/xhtml\">");

            HtmlWriter.WriteLine();
            HtmlWriter.Indent++;

            HtmlWriter.WriteFullBeginTag("head");
            HtmlWriter.WriteLine();
            HtmlWriter.Indent++;

            if (HtmlImageHost is StiMhtImageHost)
            {
                HtmlWriter.WriteBeginTag("meta");
                HtmlWriter.WriteAttribute("http-equiv", "X-UA-Compatible");
                HtmlWriter.WriteAttribute("content", "IE=11.0000");
                HtmlWriter.WriteEndTag("meta");
            }

            HtmlWriter.WriteFullBeginTag("title");
            HtmlWriter.Write(report.ReportAlias);
            HtmlWriter.WriteEndTag("title");
            HtmlWriter.WriteLine();

            HtmlWriter.WriteBeginTag("meta");
            HtmlWriter.WriteAttribute("http-equiv", "Content-Type");
            HtmlWriter.WriteAttribute("content", string.Format("text/html; charset={0}", encoding.WebName));
            HtmlWriter.WriteEndTag("meta");
            HtmlWriter.WriteLine();

            if (useStimulsoftFont)
            {
                using (var fontStream = typeof(StiFontIconsHelper).Assembly.GetManifestResourceStream("Stimulsoft.Base.FontIcons.Stimulsoft.ttf"))
                {
                    var buffer = new byte[fontStream.Length];
                    fontStream.Read(buffer, 0, buffer.Length);

                    var base64 = Convert.ToBase64String(buffer);

                    var sb = new StringBuilder();
                    sb.Append(@"<style>@font-face {font-family: 'Stimulsoft';src: url(data:font/ttf;base64,");
                    sb.Append(base64);
                    sb.Append(") format('truetype');font-weight: normal;font-style: normal;}</style>");

                    HtmlWriter.Write(sb.ToString());
                }
            }

            if (render != null)
            {
                if (asTable)
                {
                    render.RenderStylesTable(useBookmarks, exportBookmarksOnly, cssStyles);
                }
                else
                {
                    render.RenderStyles(useBookmarks, exportBookmarksOnly, cssStyles);
                }
            }

            if (useBookmarks)
            {
                RenderBookmarkScript();
            }

            HtmlWriter.Indent--;
            HtmlWriter.WriteLine();

            HtmlWriter.WriteEndTag("head");
            HtmlWriter.WriteLine();

            HtmlWriter.WriteBeginTag("body");
            if ((pages != null) && (pages.Count > 0) && (pages[0].Brush != null))
            {
                var backColor = StiBrush.ToColor(pages[0].Brush);
                if (backColor.A > 0)
                {
                    HtmlWriter.WriteAttribute("bgcolor", FormatColor(backColor));
                }
            }
            if (StiOptions.Export.Html.UseExtendedStyle)
            {
                HtmlWriter.WriteAttribute("class", "sBaseStyleFix");
            }
            if (StiOptions.Export.Html.PrintLayoutOptimization)
            {
                HtmlWriter.WriteAttribute("style", "margin:0;");
            }
            HtmlWriter.Write(">");
            HtmlWriter.Indent++;
            HtmlWriter.WriteLine();
            //            HtmlWriter.Indent++;
        }

        private void FillBitmapBackground(ref Bitmap bmp, Color fillColor)
        {
            var transparentColor = Color.LightGray;
            if ((bmp.Height > 0) && (bmp.Width > 0))
            {
                transparentColor = bmp.GetPixel(0, bmp.Size.Height - 1);
            }
            if (transparentColor.A != 0xff)
            {
                var size = bmp.Size;
                var pixelFormat = bmp.PixelFormat;
                var image = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
                var graphics = Graphics.FromImage(image);
                graphics.Clear(fillColor);
                var destRect = new Rectangle(0, 0, size.Width, size.Height);
                var imageAttrs = new ImageAttributes();
                imageAttrs.SetColorKey(transparentColor, transparentColor);
                graphics.DrawImage(bmp, destRect, 0, 0, size.Width, size.Height, GraphicsUnit.Pixel, imageAttrs, null, IntPtr.Zero);
                imageAttrs.Dispose();
                graphics.Dispose();
                bmp.Dispose();
                bmp = image;
            }
        }

        private void RenderBookmarkScript()
        {
            HtmlWriter.WriteBeginTag("script");
            HtmlWriter.WriteAttribute("type", "text/javascript");
            HtmlWriter.WriteLine(">");

            //load script from resources
            var buf = GetFile("Stimulsoft.Report", "Stimulsoft.Report.Export.Dtree.DtreeScript.js");
            var sb = new StringBuilder();
            for (int index = 0; index < buf.Length; index++) sb.Append((char)buf[index]);

            var fileList = new string[,]
            {
                {"img/base.gif",        "DtreeBase.png"},
                {"img/page.gif",        "DtreePage.png"},
                {"img/folder.gif",      "DtreeFolder.png"},
                {"img/folderopen.gif",  "DtreeFolderopen.png"},
                {"img/empty.gif",       "DtreeEmpty.png"},
                {"img/line.gif",        "DtreeLine.png"},
                {"img/join.gif",        "DtreeJoin.png"},
                {"img/joinbottom.gif",  "DtreeJoinbottom.png"},
                {"img/plus.gif",        "DtreePlus.png"},
                {"img/plusbottom.gif",  "DtreePlusbottom.png"},
                {"img/minus.gif",       "DtreeMinus.png"},
                {"img/minusbottom.gif", "DtreeMinusbottom.png"}
            };

            //replace images path
            HtmlImageHost.ForcePng = true;
            for (int index = 0; index < fileList.GetLength(0); index++)
            {
                var bmp = GetImage("Stimulsoft.Report", "Stimulsoft.Report.Export.Dtree." + fileList[index, 1], false);
                FillBitmapBackground(ref bmp, Color.FromArgb(0xf0, 0xf0, 0xf0));
                var bmpPath = HtmlImageHost.GetImageString(bmp).Replace("\\", "/");
                sb.Replace(fileList[index, 0], bmpPath);
            }
            HtmlImageHost.ForcePng = false;

            //write script
            var lineList = sb.ToString().Split(new char[] { '\r' });
            for (int index = 0; index < lineList.Length; index++)
            {
                HtmlWriter.WriteLine(lineList[index]);
            }

            HtmlWriter.WriteEndTag("script");
            HtmlWriter.WriteLine();
        }

        private void RenderChartScripts(bool writeScriptTag = true)
        {
            if (chartData.Count == 0) return;

            if (writeScriptTag)
            {
                HtmlWriter.WriteBeginTag("script");
                HtmlWriter.WriteAttribute("type", "text/javascript");
                HtmlWriter.WriteLine(">");
            }
            var guid = Guid.NewGuid().ToString().Replace("-", "");
            var fileList = new string[]
            {
                "stianimation.js"
            };

            //load scripts from resources
            for (int index = 0; index < fileList.Length; index++)
            {
                var buf = GetFile(this.GetType().Assembly, "Stimulsoft.Report.Export.Services.Htmls.ChartScripts." + fileList[index]);
                var st = Encoding.ASCII.GetString(buf);
                HtmlWriter.WriteLine(st.Replace("animateSti", "animateSti" + guid) + ";");
            }

            HtmlWriter.WriteLine("setTimeout(function() {");

            foreach (var key in chartData.Keys)
            {
                HtmlWriter.WriteLine(
                    string.Format("animateSti{0}(\"{1}\");", guid,
                    chartData[key]));
            }
            HtmlWriter.WriteLine("}, 300);");
            if (writeScriptTag)
            {
                HtmlWriter.WriteEndTag("script");
                HtmlWriter.WriteLine();
            }
        }

        internal string GetGuid(StiComponent comp)
        {
            if (!chartData.ContainsKey(comp))
            {
                chartData.Add(comp, Guid.NewGuid().ToString().Replace("-", ""));
            }
            return chartData[comp] as string;
        }


        private void RenderEndDoc()
        {
            HtmlWriter.Indent--;
            HtmlWriter.WriteLine();
            HtmlWriter.WriteEndTag("body");
            HtmlWriter.Indent--;
            HtmlWriter.WriteLine();
            HtmlWriter.WriteEndTag("html");
        }

        private void RenderBookmarkTree(StiBookmark root, int bookmarkWidth, Hashtable bookmarksPageIndex)
        {
            var bookmarksTree = new List<StiBookmarkTreeNode>();
            AddBookmarkNode(root, -1, bookmarksTree);

            //			HtmlWriter.WriteBeginTag(strSpanDiv + " class=\"dtree\" style=\"");
            //			HtmlWriter.WriteStyleAttribute("position", "absolute");
            HtmlWriter.WriteStyleAttribute("width", string.Format("{0}px", bookmarkWidth));
            //			HtmlWriter.WriteStyleAttribute("border-right", "1px");
            //			HtmlWriter.WriteStyleAttribute("border-right-style", "solid");
            //			HtmlWriter.WriteStyleAttribute("border-right-color", "Gray");
            HtmlWriter.WriteStyleAttribute("background-color", "#f0f0f0");
            HtmlWriter.Write("\">");
            HtmlWriter.Indent++;
            HtmlWriter.WriteLine();

            HtmlWriter.Indent++;
            HtmlWriter.WriteFullBeginTag("frame");
            HtmlWriter.WriteLine();
            HtmlWriter.WriteBeginTag("script");
            HtmlWriter.WriteAttribute("type", "text/javascript");
            HtmlWriter.WriteLine(">");
            HtmlWriter.Indent++;
            HtmlWriter.WriteLine("<!--");
            HtmlWriter.WriteLine("bmrk = new dTree('bmrk');");
            for (int index = 0; index < bookmarksTree.Count; index++)
            {
                var node = bookmarksTree[index];
                string pageIndex;
                string url = node.Url;
                if (bookmarksPageIndex.ContainsKey(node.Title))
                {
                    pageIndex = string.Format("Page {0}", (int)bookmarksPageIndex[node.Title] + 1);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(node.FullUrl) && bookmarksPageIndex.ContainsKey(node.FullUrl))
                    {
                        pageIndex = string.Format("Page {0}", (int)bookmarksPageIndex[node.FullUrl] + 1);
                        url = "#" + node.FullUrl.Replace("\\", "\\\\");
                    }
                    else
                    {
                        pageIndex = "Page 0";
                    }
                }

                HtmlWriter.WriteLine(string.Format("bmrk.add({0}, {1}, '{2}', '{3}', '{4}');", index, node.Parent, SecurityElement.Escape(node.Title), url, pageIndex));
            }
            HtmlWriter.WriteLine("document.write(bmrk);");
            HtmlWriter.WriteLine("//-->");
            HtmlWriter.Indent--;
            HtmlWriter.WriteEndTag("script");
            HtmlWriter.Indent--;
            HtmlWriter.WriteLine();

            HtmlWriter.Indent--;
            //			HtmlWriter.WriteEndTag(strSpanDiv);
            //			HtmlWriter.WriteLine();
        }

        private class StiBookmarkTreeNode
        {
            public int Parent;
            public string Title;
            public string Url;
            public string FullUrl;
            public bool Used;
        }

        private void AddBookmarkNode(StiBookmark bkm, int parentNode, List<StiBookmarkTreeNode> bookmarksTree, string path = null)
        {
            var tn = new StiBookmarkTreeNode();
            tn.Parent = parentNode;
            string st = bkm.Text.Replace("'", "\\\'").Replace("\r", "").Replace("\n", "");
            tn.Title = st;
            tn.Url = "#" + st;
            if (!string.IsNullOrWhiteSpace(path)) path += "\\" + st;
            tn.FullUrl = path;
            tn.Used = true;
            bookmarksTree.Add(tn);
            int currentNode = bookmarksTree.Count - 1;
            if (bkm.Bookmarks.Count != 0)
            {
                if (string.IsNullOrWhiteSpace(path)) path = "%";
                for (int tempCount = 0; tempCount < bkm.Bookmarks.Count; tempCount++)
                {
                    AddBookmarkNode(bkm.Bookmarks[tempCount], currentNode, bookmarksTree, path);
                }
            }
        }
        #endregion

        internal string PrepareTextForHtml(string text, bool processWhiteSpaces, bool isJustify)
        {
            if (text == null) return null;
            text = text.Replace("\r", "");
            var sbFull = new StringBuilder();

            if (isJustify)
            {
                text = text.TrimEnd(' ');
                if (text.EndsWithInvariant("\n"))
                    text = text.Substring(0, text.Length - 1);
            }

            if (processWhiteSpaces)
            {
                var txt = text.Split(new char[] { '\n' });
                for (int index = 0; index < txt.Length; index++)
                {
                    string st = txt[index];
                    int pos = 0;
                    while ((pos < st.Length) && (st[pos] == ' ')) pos++;
                    if (pos > 0)
                    {
                        for (int indexSp = 0; indexSp < pos; indexSp++)
                        {
                            sbFull.Append("&nbsp;");
                        }
                        sbFull.Append(st.Substring(pos));
                    }
                    else
                    {
                        sbFull.Append(st);
                    }
                    if (index < txt.Length - 1)
                    {
                        sbFull.Append("<br>");
                    }
                }
            }
            else
            {
                sbFull.Append(text.Replace("\n", "<br>"));
            }
            return sbFull.ToString();
        }


        internal static string ConvertTextWithHtmlTagsToHtmlText(StiText stiText, string text, double zoom)
        {
            string inputText = text;
            var baseTagsState = new StiTextRenderer.StiHtmlTagsState(
                stiText.Font.Bold,
                stiText.Font.Italic,
                stiText.Font.Underline,
                stiText.Font.Strikeout,
                stiText.Font.SizeInPoints,
                stiText.Font.Name,
                StiBrush.ToColor(stiText.TextBrush),
                StiBrush.ToColor(stiText.Brush),
                false,
                false,
                0,
                0,
                stiText.LineSpacing,
                stiText.HorAlignment);
            var baseState = new StiTextRenderer.StiHtmlState(
            baseTagsState,
            0);
            var statesList = StiTextRenderer.ParseHtmlToStates(inputText, baseState);

            var finalText = new StringBuilder();
            var textAlign = StiTextHorAlignment.Left;
            double lineHeight = stiText.LineSpacing;
            double lastLineHeight;
            bool needNbsp = true;
            bool needCloseFont = false;
            var outputText = new StringBuilder();
            var prevState = baseTagsState;
            for (int index = 0; index < statesList.Count; index++)
            {
                var htmlState = statesList[index];
                var state = htmlState.TS;

                if (state.Bold != prevState.Bold && !state.Bold)
                {
                    outputText.Append("</b>");
                }
                if (state.Italic != prevState.Italic && !state.Italic)
                {
                    outputText.Append("</i>");
                }
                if (state.Underline != prevState.Underline && !state.Underline)
                {
                    outputText.Append("</u>");
                }
                if (state.Strikeout != prevState.Strikeout && !state.Strikeout)
                {
                    outputText.Append("</s>");
                }
                if (state.Superscript != prevState.Superscript && !state.Superscript)
                {
                    outputText.Append("</sup>");
                }
                if (state.Subscript != prevState.Subscript && !state.Subscript)
                {
                    outputText.Append("</sub>");
                }

                if (state.Bold != prevState.Bold && state.Bold)
                {
                    outputText.Append("<b>");
                }
                if (state.Italic != prevState.Italic && state.Italic)
                {
                    outputText.Append("<i>");
                }
                if (state.Underline != prevState.Underline && state.Underline)
                {
                    outputText.Append("<u>");
                }
                if (state.Strikeout != prevState.Strikeout && state.Strikeout)
                {
                    outputText.Append("<s>");
                }
                if (state.Superscript != prevState.Superscript && state.Superscript)
                {
                    outputText.Append("<sup>");
                }
                if (state.Subscript != prevState.Subscript && state.Subscript)
                {
                    outputText.Append("<sub>");
                }

                if ((state.FontColor != prevState.FontColor) ||
                    (state.BackColor != prevState.BackColor) ||
                    (state.FontName != prevState.FontName) ||
                    (state.FontSize != prevState.FontSize) ||
                    (state.LetterSpacing != prevState.LetterSpacing) ||
                    (state.WordSpacing != prevState.WordSpacing) ||
                    (state.HtmlStyle != prevState.HtmlStyle))
                {
                    string fontStyle = "";
                    if (state.HtmlStyle != baseTagsState.HtmlStyle)
                    {
                        fontStyle += state.HtmlStyle + ";";
                    }
                    if (state.FontColor != baseTagsState.FontColor)
                    {
                        if (!fontStyle.Contains("color:") && (state.FontColor.A > 0))
                            fontStyle += string.Format("color:#{0:X6};", state.FontColor.ToArgb() & 0xFFFFFF);
                    }
                    if (state.BackColor != baseTagsState.BackColor)
                    {
                        if (!fontStyle.Contains("background-color:") && (state.BackColor.A > 0))
                            fontStyle += string.Format("background-color:#{0:X6};", state.BackColor.ToArgb() & 0xFFFFFF);
                    }
                    if (state.FontName != baseTagsState.FontName)
                    {
                        if (!fontStyle.Contains("font-family:"))
                            fontStyle += string.Format("font-family:{0};", state.FontName);
                    }
                    if (state.FontSize != baseTagsState.FontSize)
                    {
                        if (!fontStyle.Contains("font-size:"))
                        {
                            double fontSize = state.FontSize * zoom;
                            fontStyle += string.Format("font-size:{0}pt;", fontSize).Replace(",", ".");
                            fontStyle += string.Format("line-height:{0}em;", Math.Round(state.LineHeight * 1.12, 2)).Replace(",", ".");
                        }
                    }
                    if (state.LetterSpacing != baseTagsState.LetterSpacing)
                    {
                        if (!fontStyle.Contains("letter-spacing:"))
                            fontStyle += string.Format("letter-spacing:{0}em;", state.LetterSpacing).Replace(",", ".");
                    }
                    if (state.WordSpacing != baseTagsState.WordSpacing)
                    {
                        if (!fontStyle.Contains("word-spacing:"))
                            fontStyle += string.Format("word-spacing:{0}em;", state.WordSpacing).Replace(",", ".");
                    }

                    if (needCloseFont) outputText.Append("</font>");
                    needCloseFont = false;
                    if (fontStyle.Length > 0)
                    {
                        needCloseFont = true;
                        outputText.Append(string.Format("<font style=\"{0}\">", fontStyle));
                    }
                }

                lastLineHeight = state.LineHeight;

                if (htmlState.Text.ToString() == "\x0A")
                {
                    if (needNbsp)
                    {
                        outputText.Append("&nbsp;");
                    }
                    int listIndent = (htmlState.TS.Tag != null && htmlState.TS.Tag.Tag == StiTextRenderer.StiHtmlTag.ListItem) ? htmlState.TS.Indent : 0;
                    finalText.Append(GetParagraphString(outputText, textAlign, lastLineHeight, stiText.LineSpacing, state.FontName, stiText.RightToLeft, listIndent));
                    outputText = new StringBuilder();
                    needNbsp = true;
                    lineHeight = lastLineHeight;
                }
                else
                {
                    string href = state.Href;
                    if (!string.IsNullOrEmpty(href))
                    {
                        href = href.Trim();
                        if (StiOptions.Export.Html.DisableJavascriptInHyperlinks && href.StartsWith("javascript:")) href = null;
                    }
                    if (!string.IsNullOrEmpty(href))
                    {
                        outputText.AppendFormat("<a style=\"text-decoration:none;\" href=\"{0}\">", href);
                    }
                    outputText.Append(htmlState.Text);
                    if (!string.IsNullOrEmpty(href))
                    {
                        outputText.Append("</a>");
                    }
                    if (htmlState.Text.ToString().Trim().Length > 0) needNbsp = false;
                }

                textAlign = state.TextAlign;

                prevState = state;
            }

            if (outputText.Length > 0)
            {
                int listIndent = (prevState.Tag != null && prevState.Tag.Tag == StiTextRenderer.StiHtmlTag.ListItem) ? prevState.Indent : 0;
                finalText.Append(GetParagraphString(outputText, textAlign, lineHeight, stiText.LineSpacing, prevState.FontName, stiText.RightToLeft, listIndent));
            }
            if (needCloseFont) finalText.Append("</font>");
            if (prevState.Bold != baseTagsState.Bold)
            {
                finalText.Append(baseTagsState.Bold ? "<b>" : "</b>");
            }
            if (prevState.Italic != baseTagsState.Italic)
            {
                finalText.Append(baseTagsState.Italic ? "<i>" : "</i>");
            }
            if (prevState.Underline != baseTagsState.Underline)
            {
                finalText.Append(baseTagsState.Underline ? "<u>" : "</u>");
            }
            if (prevState.Strikeout != baseTagsState.Strikeout)
            {
                finalText.Append(baseTagsState.Strikeout ? "<s>" : "</s>");
            }
            if (prevState.Superscript != baseTagsState.Superscript)
            {
                finalText.Append(baseTagsState.Superscript ? "<sup>" : "</sup>");
            }
            if (prevState.Subscript != baseTagsState.Subscript)
            {
                finalText.Append(baseTagsState.Subscript ? "<sub>" : "</sub>");
            }

            return finalText.ToString();
        }

        private static string GetParagraphString(StringBuilder text, StiTextHorAlignment textAlign, double lineHeight, double baseLineSpacing, string fontName, bool rtl, int indent)
        {
            var outputText = new StringBuilder();
            outputText.Append("<p ");
            outputText.Append("style=\"margin:0px;");
            string align = rtl ? "right" : "left";
            if (textAlign == StiTextHorAlignment.Center) align = "center";
            if (textAlign == StiTextHorAlignment.Right) align = rtl ? "left" : "right";
            if (textAlign == StiTextHorAlignment.Width) align = "justify";
            outputText.Append(string.Format("text-align:{0};", align));
            if (lineHeight != baseLineSpacing)
            {
                outputText.Append(string.Format("line-height:{0}em;", Math.Round(lineHeight * 1.12 * GetFontHeightCorrection(fontName), 2)).Replace(",", "."));
            }
            if (indent > 0 && StiOptions.Engine.Html.AllowListItemSecondLineIndent)
            {
                outputText.Append(string.Format("padding-left:{0}em;text-indent:-{0}em;", (indent * 2.8).ToString().Replace(",", ".")));
            }
            if (indent > 0)
            {
                //replace "&nbsp;" with "&thinsp;"
                text.Replace("\xA0\xA0\xA0", "\u2009\u2009\u2009\u2009");
            }
            outputText.Append("\">");
            outputText.Append(text);
            outputText.Append("</p>");
            return outputText.ToString();
        }

        private string GetBackgroundImagePath(StiPagesCollection pages, double zoomBase, bool useMargins)
        {
            string backGroundImageString = string.Empty;
            if (pages.Count > 0)
            {
                #region watermark
                var page = pages[0];
                var watermark = page.Watermark;
                if (watermark != null && watermark.Enabled &&
                    (watermark.ExistImage() || !string.IsNullOrWhiteSpace(watermark.ImageHyperlink) || !string.IsNullOrEmpty(watermark.Text)))
                {
                    var painter = StiPainter.GetPainter(typeof(StiPage), StiGuiMode.Gdi) as IStiPagePainter;
                    var zoom = StiOptions.Export.Html.PrintLayoutOptimization ? zoomBase * 0.96 : zoomBase;
                    var image = painter.GetWatermarkImage(page, zoom, useMargins) as Bitmap;

                    if (HtmlImageHost != null) backGroundImageString = HtmlImageHost.GetImageString(image as Bitmap).Replace('\\', '/');
                    if (backGroundImageString == null) backGroundImageString = string.Empty;
                    if (image != null) image.Dispose();
                }
                #endregion
            }
            return backGroundImageString.Replace('\\', '/');
        }

        private static string ImageBytesToBase64String(byte[] image, double pageWidth, double pageHeight)
        {
            string mimeType = "data:image/png;base64,";

            if (Helpers.StiImageHelper.IsMetafile(image))
            {
                image = StiMetafileConverter.MetafileToPngBytes(image, (int)pageWidth, (int)pageHeight);
                //mimeType = "data:image/x-wmf;base64,";
            }
            else if (Helpers.StiImageHelper.IsBmp(image))
            {
                mimeType = "data:image/bmp;base64,";
            }
            else if (Helpers.StiImageHelper.IsJpeg(image))
            {
                mimeType = "data:image/jpeg;base64,";
            }
            else if (Helpers.StiImageHelper.IsGif(image))
            {
                mimeType = "data:image/gif;base64,";
            }
            else if (Base.Helpers.StiSvgHelper.IsSvg(image))
            {
                mimeType = "data:image/svg+xml;base64,";
            }

            return mimeType + Convert.ToBase64String(image);
        }

        private void RenderWatermarkImage(StiPage page, double topPos = 0)
        {
            if (page != null && page.Watermark != null && page.Watermark.Enabled && (page.Watermark.ExistImage() || !string.IsNullOrWhiteSpace(page.Watermark.ImageHyperlink)))
            {
                var rectPage = page.Unit.ConvertToHInches(page.ClientRectangle);
                var pageWidth = rectPage.Width * this.zoom;
                var pageHeight = rectPage.Height * this.zoom * (StiOptions.Export.Html.PrintLayoutOptimization ? StiMatrix.htmlScaleY : 1);
                var zIndex = page.Watermark.ShowImageBehind ? 0 : 1;
                var imageWidth = 0;
                var imageBase64Data = string.Empty;

                if (page.Watermark.TakeImage() != null)
                {
                    if (page.Watermark.TakeGdiImage() != null)
                    {
                        using (var gdiImage = page.Watermark.TakeGdiImage())
                        {
                            imageBase64Data = ImageBytesToBase64String(page.Watermark.TakeImage(), pageWidth, pageHeight);
                            imageWidth = gdiImage.Width;
                        }
                    }                    
                }
                else if (!string.IsNullOrWhiteSpace(page.Watermark.ImageHyperlink))
                {
                    if (StiHyperlinkProcessor.IsResourceHyperlink(page.Watermark.ImageHyperlink))
                    {
                        var resource = page.Report.Dictionary.Resources[StiHyperlinkProcessor.GetResourceNameFromHyperlink(page.Watermark.ImageHyperlink)];
                        if (resource != null)
                        {
                            using (var gdiImage = StiImageConverter.BytesToImage(resource.Content))
                            {
                                imageBase64Data = ImageBytesToBase64String(resource.Content, pageWidth, pageHeight);
                                imageWidth = gdiImage.Width;
                            }
                        }
                    }
                    else if (StiHyperlinkProcessor.IsVariableHyperlink(page.Watermark.ImageHyperlink))
                    {
                        var variable = page.Report.Dictionary.Variables[StiHyperlinkProcessor.GetVariableNameFromHyperlink(page.Watermark.ImageHyperlink)];
                        if (variable != null && variable.ValueObject != null)
                        {
                            var image = variable.ValueObject as Image;
                            imageBase64Data = ImageBytesToBase64String(Helpers.StiImageHelper.GetImageBytesFromObject(image), pageWidth, pageHeight);
                            imageWidth = image.Width;
                        }
                    }
                }

                var watermarkWidth = imageWidth * page.Watermark.ImageMultipleFactor * this.zoom;

                var imageStyles = "";
                var horAlign = "center";
                var vertAlign = "center";

                switch (page.Watermark.ImageAlignment)
                {
                    case ContentAlignment.TopLeft:
                        horAlign = "left";
                        vertAlign = "top";
                        break;

                    case ContentAlignment.TopCenter:
                        horAlign = "center";
                        vertAlign = "top";
                        break;

                    case ContentAlignment.TopRight:
                        horAlign = "right";
                        vertAlign = "top";
                        break;

                    case ContentAlignment.MiddleLeft:
                        horAlign = "left";
                        vertAlign = "center";
                        break;

                    case ContentAlignment.MiddleCenter:
                        horAlign = "center";
                        vertAlign = "center";
                        break;

                    case ContentAlignment.MiddleRight:
                        horAlign = "right";
                        vertAlign = "center";
                        break;

                    case ContentAlignment.BottomLeft:
                        horAlign = "left";
                        vertAlign = "bottom";
                        break;

                    case ContentAlignment.BottomCenter:
                        horAlign = "center";
                        vertAlign = "bottom";
                        break;

                    case ContentAlignment.BottomRight:
                        horAlign = "right";
                        vertAlign = "bottom";
                        break;
                }

                imageStyles += string.Format("background-position-x: {0};", horAlign);
                imageStyles += string.Format("background-position-y: {0};", vertAlign);

                if (page.Watermark.ImageStretch)
                {
                    if (page.Watermark.AspectRatio)
                        imageStyles += "background-size: contain;";
                    else
                        imageStyles += "background-size: 100% 100%;";
                }
                else
                {
                    imageStyles += string.Format("background-size: {0}%;", watermarkWidth != 0 ? Math.Round(watermarkWidth / pageWidth * 100) : 100);
                }

                imageStyles += string.Format("background-repeat: {0};", page.Watermark.ImageTiling ? "repeat" : "no-repeat");

                if (page.Watermark.ImageTransparency != 0)
                {
                    var opacity = Math.Round((255 - page.Watermark.ImageTransparency) / 255d, 4);
                    imageStyles += $" filter: alpha(Opacity={opacity * 10d}); opacity: {opacity}; -moz-opacity: {opacity}; -khtml-opacity: {opacity};";
                }

                HtmlWriter.WriteLine(string.Format("<div class='stiWatermarkImage' style=\"position: absolute; pointer-events: none; width: {0}px; height: {1}px; left: 0px; top: {5}px; right: 0px;" +
                    " bottom: 0px; z-index: {2}; background-image: url({3}); {4};\"></div>",
                    pageWidth,
                    pageHeight,
                    zIndex,
                    imageBase64Data,
                    imageStyles,
                    topPos
                ));
            }
        }

        private void RenderWatermarkText(StiPage page, double topPos = 0)
        {
            if (page != null && !string.IsNullOrEmpty(page.Watermark.Text) && page.Watermark.Enabled)
            {
                var rectPage = page.Unit.ConvertToHInches(page.DisplayRectangle);
                if (StiOptions.Export.Html.PrintLayoutOptimization) rectPage.Height *= StiMatrix.htmlScaleY;
                var fontSize = Math.Round(page.Watermark.Font.Size * this.zoom);
                var fontStyle = string.Format(" font-size: {0}pt; font-family: {1};", fontSize, page.Watermark.Font.Name);
                if (page.Watermark.Font.Bold) fontStyle += "font-weight:bold;";
                if (page.Watermark.Font.Italic) fontStyle += "font-style:italic;";
                if (page.Watermark.Font.Underline) fontStyle += ("text-decoration:underline" + (page.Watermark.Font.Strikeout ? " " : ";"));
                if (page.Watermark.Font.Strikeout) fontStyle += (page.Watermark.Font.Underline ? "line-through;" : "text-decoration:line-through;");
                var color = StiBrush.ToColor(page.Watermark.TextBrush);
                var htmlColor = string.Format("rgb({0},{1},{2})", color.R, color.G, color.B);
                var opacity = color.A / 255d;
                var zIndex = page.Watermark.ShowBehind ? 0 : 1;

                HtmlWriter.WriteLine(string.Format("<div class='stiWatermarkText' style=\"{0} position: absolute; pointer-events: none; filter: alpha(Opacity={2}); opacity: {1};" +
                    " -moz-opacity: {1}; -khtml-opacity: {1}; color: {5}; min-width: 100%; text-align: center; z-index: {8}; margin-left: {3}; margin-top: {4};" +
                    " transform: translate(-50%,-50%) rotate(-{6}deg); top:{9}px;\">{7}</div>",
                    fontStyle,
                    opacity,
                    opacity * 10d,
                    StiHtmlUnit.NewUnit(Math.Round((-page.Unit.ConvertToHInches(page.Margins.Left) + rectPage.Width / 2) * this.zoom), StiOptions.Export.Html.PrintLayoutOptimization),
                    StiHtmlUnit.NewUnit(Math.Round((-page.Unit.ConvertToHInches(page.Margins.Top) + rectPage.Height / 2) * this.zoom), StiOptions.Export.Html.PrintLayoutOptimization),
                    htmlColor,
                    page.Watermark.Angle,
                    page.Watermark.Text,
                    zIndex,
                    topPos
                ));
            }
        }

        public static Bitmap GetImage(string assemblyName, string imageName, bool makeTransparent)
        {
            var assemblys = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in assemblys)
            {
                if (StiOptions.Engine.FullTrust)
                {
                    var str = a.GetName().Name;
                    if (str == assemblyName) return StiImageUtils.GetImage(a, imageName, makeTransparent);
                }
                else
                {
                    var str = a.FullName;
                    if (str.StartsWith(assemblyName + ", ")) return StiImageUtils.GetImage(a, imageName, makeTransparent);
                }
            }
            throw new Exception(string.Format("Can't find assembly '{0}'", assemblyName));
        }

        /// <summary>
        /// Gets the object placed in assembly.
        /// </summary>
        /// <param name="assemblyName">The name of assembly in which the object is placed.</param>
        /// <param name="fileName">The name of the file to look for.</param>
        /// <returns>The object.</returns>
        public static byte[] GetFile(string assemblyName, string fileName)
        {
            var assemblys = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in assemblys)
            {
                if (StiOptions.Engine.FullTrust)
                {
                    var str = a.GetName().Name;
                    if (str == assemblyName) return GetFile(a, fileName);
                }
                else
                {
                    var str = a.FullName;
                    if (str.StartsWith(assemblyName + ", ")) return GetFile(a, fileName);
                }
            }
            throw new Exception(string.Format("Can't find assembly '{0}'", assemblyName));
        }

        /// <summary>
        /// Gets the Image object placed in assembly.
        /// </summary>
        /// <param name="imageAssembly">Assembly in which is the Image object is placed.</param>
        /// <param name="fileName">The name of the image file to look for.</param>
        /// <returns>The Image object.</returns>
        public static byte[] GetFile(Assembly imageAssembly, string fileName)
        {
            var stream = imageAssembly.GetManifestResourceStream(fileName);
            if (stream != null)
            {
                var buf = new byte[stream.Length];
                stream.Read(buf, 0, buf.Length);
                return buf;
            }
            else 
                throw new Exception(string.Format("Can't find file '{0}' in resources", fileName));
        }

        private void AssembleGuidUsedInBookmark(StiBookmark node, Hashtable hash)
        {
            if (!string.IsNullOrWhiteSpace(node.ComponentGuid))
            {
                hash[node.ComponentGuid] = node.Text;
            }
            if (node.Bookmarks != null && node.Bookmarks.Count > 0)
            {
                foreach (StiBookmark bookmark in node.Bookmarks)
                {
                    AssembleGuidUsedInBookmark(bookmark, hash);
                }
            }
        }

        private void ProcessGradientBrushes(StiComponent comp)
        {
            StiGradientBrush grb = null;
            StiGlareBrush glb = null;
            StiGlassBrush gsb = null;

            var compTxt = comp as StiText;
            if (compTxt != null)
            {
                if (compTxt.Brush is StiGradientBrush) grb = compTxt.Brush as StiGradientBrush;
                if (compTxt.Brush is StiGlareBrush) glb = compTxt.Brush as StiGlareBrush;
                if (compTxt.Brush is StiGlassBrush) gsb = compTxt.Brush as StiGlassBrush;
            }
            var cont = comp as StiContainer;
            if (cont != null)
            {
                if (cont.Brush is StiGradientBrush) grb = cont.Brush as StiGradientBrush;
                if (cont.Brush is StiGlareBrush) glb = cont.Brush as StiGlareBrush;
                if (cont.Brush is StiGlassBrush) gsb = cont.Brush as StiGlassBrush;
            }

            if (grb != null)
            {
                var stAngle = $"{Math.Round(grb.Angle) + 90}deg";
                HtmlWriter.Write($"background: linear-gradient({stAngle}, {FormatColor(grb.StartColor)}, {FormatColor(grb.EndColor)});");
            }
            if (glb != null)
            {
                var stAngle = $"{Math.Round(glb.Angle) + 90}deg";
                HtmlWriter.Write($"background: linear-gradient({stAngle}, {FormatColor(glb.StartColor)}, {FormatColor(glb.EndColor)} {Math.Round(glb.Focus * 100)}%, {FormatColor(glb.StartColor)});");
            }
            if (gsb != null)
            {
                HtmlWriter.Write($"background: linear-gradient({FormatColor(gsb.GetTopColor())}, {FormatColor(gsb.GetTopColor())} 49%, {FormatColor(gsb.GetBottomColor())} 50%, {FormatColor(gsb.GetBottomColor())});");
            }
        }

        internal static void PrepareSvgData(StiHtmlTextWriter sWriter, double height, double width)
        {
            sWriter.WriteBeginTag("svg");
            sWriter.WriteAttribute("version", "1.1");
            sWriter.WriteAttribute("baseProfile", "full");
            sWriter.WriteAttribute("xmlns", "http://www.w3.org/2000/svg");
            sWriter.WriteAttribute("xmlns:xlink", "http://www.w3.org/1999/xlink");
            sWriter.WriteAttribute("xmlns:ev", "http://www.w3.org/2001/xml-events");
            sWriter.WriteAttribute("height", height.ToString().Replace(",", "."));
            sWriter.WriteAttribute("width", width.ToString().Replace(",", "."));
            sWriter.Write(">");
        }

        internal void PrepareSvg(StiHtmlTextWriter sWriter, double height, double width)
        {
            StiHtmlExportService.PrepareSvgData(sWriter, height, width);
        }

        internal string PrepareChartData(StiHtmlTextWriter writer, StiChart chart, double width, double height)
        {
            using (var img = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(img))
                {
                    var painter = new StiGdiContextPainter(g);
                    var context = new Stimulsoft.Base.Context.StiContext(painter, true, false, false, (float)zoom);
                    var sb = new StringBuilder();
                    StiHtmlTextWriter sWriter = null;
                    if (writer == null)
                    {
                        var sw = new StringWriter(sb);
                        sWriter = new StiHtmlTextWriter(sw);
                    }
                    else
                    {
                        sWriter = writer;
                    }
                    float scale = (exportMode == StiHtmlExportMode.Table) ? 1f : 0.96f;
                    PrepareSvg(sWriter, Math.Round(height * scale, 2), Math.Round(width * scale, 2));
                    var pp = new StiSvgData();

                    pp.X = 0;
                    pp.Y = 0;
                    pp.Width = (float)(width * scale);
                    pp.Height = (float)(height * scale);
                    pp.Component = chart;

                    var ms = new MemoryStream();
                    var xmlsWriter = new XmlTextWriter(ms, Encoding.UTF8);
                    StiChartSvgHelper.WriteChart(xmlsWriter, pp, (float)this.zoom, chartType == StiHtmlChartType.AnimatedVector);
                    xmlsWriter.Flush();
                    sWriter.Write(Encoding.UTF8.GetString(ms.ToArray()));
                    sWriter.WriteEndTag("svg");
                    GetGuid(chart);
                    if (writer == null) return sb.ToString();
                    else return null;
                }
            }
        }

        internal static string GetSparklineData(StiHtmlTextWriter writer, StiSparkline sparkline, double width, double height, StiHtmlExportMode exportMode = StiHtmlExportMode.Div)
        {
            using (var img = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(img))
                {
                    var painter = new StiGdiContextPainter(g);
                    var context = new Stimulsoft.Base.Context.StiContext(painter, true, false, false, 1);
                    var sb = new StringBuilder();
                    StiHtmlTextWriter sWriter = null;
                    if (writer == null)
                    {
                        var sw = new StringWriter(sb);
                        sWriter = new StiHtmlTextWriter(sw);
                    }
                    else
                    {
                        sWriter = writer;
                    }
                    float scale = (exportMode == StiHtmlExportMode.Table) ? 1f : 0.96f;
                    PrepareSvgData(sWriter, Math.Round(height * scale, 2), Math.Round(width * scale, 2));
                    var pp = new StiSvgData();

                    pp.X = 0;
                    pp.Y = 0;
                    pp.Width = (float)(width * scale);
                    pp.Height = (float)(height * scale);
                    pp.Component = sparkline;

                    var ms = new MemoryStream();
                    var xmlsWriter = new XmlTextWriter(ms, Encoding.UTF8);
                    StiSparklineSvgHelper.WriteSparkline(xmlsWriter, pp);
                    xmlsWriter.Flush();
                    sWriter.Write(Encoding.UTF8.GetString(ms.ToArray()));
                    sWriter.WriteEndTag("svg");
                    if (writer == null) return sb.ToString();
                    else return null;
                }
            }
        }

        internal string PrepareSparklineData(StiHtmlTextWriter writer, StiSparkline sparkline, double width, double height)
        {
            return GetSparklineData(writer, sparkline, width, height, exportMode);
        }

        internal string PrepareGaugeData(StiHtmlTextWriter writer, StiGauge gauge, double width, double height)
        {
            using (var img = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(img))
                {
                    var painter = new StiGdiContextPainter(g);
                    var context = new Stimulsoft.Base.Context.StiContext(painter, true, false, false, (float)zoom);
                    var sb = new StringBuilder();
                    StiHtmlTextWriter sWriter = null;
                    if (writer == null)
                    {
                        var sw = new StringWriter(sb);
                        sWriter = new StiHtmlTextWriter(sw);
                    }
                    else
                    {
                        sWriter = writer;
                    }
                    float scale = (exportMode == StiHtmlExportMode.Table) ? 1f : 0.96f;
                    PrepareSvg(sWriter, Math.Round(height * scale, 2), Math.Round(width * scale, 2));
                    var pp = new StiSvgData();

                    pp.X = 0;
                    pp.Y = 0;
                    pp.Width = (float)(width * scale);
                    pp.Height = (float)(height * scale);
                    pp.Component = gauge;

                    var ms = new MemoryStream();
                    var xmlsWriter = new XmlTextWriter(ms, Encoding.UTF8);
                    StiGaugeSvgHelper.WriteGauge(xmlsWriter, pp, (float)this.zoom, (chartType == StiHtmlChartType.AnimatedVector));
                    xmlsWriter.Flush();
                    sWriter.Write(Encoding.UTF8.GetString(ms.ToArray()));
                    sWriter.WriteEndTag("svg");
                    GetGuid(gauge);
                    if (writer == null) return sb.ToString();
                    else return null;
                }
            }
        }

        internal string PrepareMapData(StiHtmlTextWriter writer, StiMap map, double width, double height)
        {

            var sb = new StringBuilder();
            StiHtmlTextWriter sWriter;
            if (writer == null)
            {
                var sw = new StringWriter(sb);
                sWriter = new StiHtmlTextWriter(sw);
            }
            else
            {
                sWriter = writer;
            }
            float scale = (exportMode == StiHtmlExportMode.Table) ? 1f : 0.96f;
            PrepareSvg(sWriter, Math.Round(height * scale, 2), Math.Round(width * scale, 2));
            var ms = new MemoryStream();
            var xmlsWriter = new XmlTextWriter(ms, Encoding.UTF8);
            StiMapSvgHelper.DrawMap(xmlsWriter, map, 0, 0, width * scale, height * scale, chartType == StiHtmlChartType.AnimatedVector);
            xmlsWriter.Flush();
            sWriter.Write(Encoding.UTF8.GetString(ms.ToArray()));
            sWriter.WriteEndTag("svg");
            GetGuid(map);
            if (writer == null) return sb.ToString();
            else return null;
        }

        public string GetChartScript()
        {
            var tempHtmlWriter = HtmlWriter;
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            HtmlWriter = new StiHtmlTextWriter(sw);

            RenderChartScripts(false);

            HtmlWriter.Flush();
            HtmlWriter = tempHtmlWriter;
            sw.Flush();
            sw.Close();

            return sb.ToString();
        }

        public void Clear()
        {
            if ((TableRender != null) && (TableRender.Matrix != null))
            {
                TableRender.Matrix.Clear();
                TableRender.Matrix = null;
            }
            TableRender = null;
            coordX = null;
            coordY = null;
            Styles = null;
            chartData.Clear();
        }

        private bool IsComponentHasInteraction(StiComponent comp)
        {
            if (this.RenderWebInteractions && comp.Interaction != null)
            {
                if (comp.Interaction.SortingEnabled && !string.IsNullOrWhiteSpace(comp.Interaction.SortingColumn)) return true;
                if (comp.Interaction.DrillDownEnabled && (!string.IsNullOrEmpty(comp.Interaction.DrillDownPageGuid) || !string.IsNullOrEmpty(comp.Interaction.DrillDownReport))) return true;
                if (comp.Interaction is StiBandInteraction && ((StiBandInteraction)comp.Interaction).CollapsingEnabled) return true;
            }
            return false;
        }
        #endregion

        #region this
        /// <summary>
		/// Exports a document to the HTML.
		/// </summary>
        /// <param name="report">A rendered report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        public void ExportHtml(StiReport report, string fileName)
        {
            StiFileUtils.ProcessReadOnly(fileName);
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                ExportHtml(report, stream);
                stream.Flush();
                stream.Close();
            }
        }


        /// <summary>
        /// Exports a document to the HTML.
        /// </summary>
        /// <param name="report">A rendered report which is to be exported.</param>
        /// <param name="stream">A stream for the export of a document.</param>
        public void ExportHtml(StiReport report, Stream stream)
        {
            ExportHtml(report, stream, 1, ImageFormat.Gif);
        }


        /// <summary>
        /// Exports a document to the HTML.
        /// </summary>
        /// <param name="report">A rendered report which is to be exported.</param>
        /// <param name="stream">A stream for the export of a document.</param>
        /// <param name="zoom">Sets zoom of the exported images.</param>
        /// <param name="imageFormat">Specifies a format of the images in the resulted HTML document.</param>
        public void ExportHtml(StiReport report, Stream stream, double zoom, ImageFormat imageFormat)
        {
            ExportHtml(report, stream, zoom, imageFormat, StiPagesRange.All);
        }


        /// <summary>
        /// Exports a document to the HTML.
        /// </summary>
        /// <param name="report">A rendered report which is to be exported.</param>
        /// <param name="stream">A stream for the export of a document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        public void ExportHtml(StiReport report, Stream stream, StiPagesRange pageRange)
        {
            ExportHtml(report, stream, 1, ImageFormat.Gif, pageRange, StiHtmlExportMode.Table, StiHtmlExportQuality.High);
        }


        /// <summary>
        /// Exports a document to the HTML.
        /// </summary>
        /// <param name="report">A rendered report which is to be exported.</param>
        /// <param name="stream">A stream for the export of a document.</param>
        /// <param name="zoom">A zoom of the exported document. Default value is 1.</param>
        /// <param name="imageFormat">Specifies a format of the images in the resulted HTML document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        public void ExportHtml(StiReport report, Stream stream, double zoom, ImageFormat imageFormat, StiPagesRange pageRange)
        {
            ExportHtml(report, stream, zoom, imageFormat, pageRange, StiHtmlExportMode.Table, StiHtmlExportQuality.High);
        }


        /// <summary>
        /// Exports a document to the HTML.
        /// </summary>
        /// <param name="report">A rendered report which is to be exported.</param>
        /// <param name="stream">A stream for the export of a document.</param>
        /// <param name="zoom">A zoom of the exported document. Default value is 1.</param>
        /// <param name="imageFormat">Specifies format of the image.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        /// <param name="exportMode">A parameter that sets modes for the HTML export.</param>
        /// <param name="exportQuality">A parameter which specifies a quality of the images in the resulted HTML document.</param>
        public void ExportHtml(StiReport report, Stream stream, double zoom, ImageFormat imageFormat,
            StiPagesRange pageRange, StiHtmlExportMode exportMode, StiHtmlExportQuality exportQuality)
        {
            ExportHtml(report, stream, zoom, imageFormat, pageRange, exportMode, exportQuality, Encoding.UTF8);
        }


        /// <summary>
        /// Exports a document to the HTML.
        /// </summary>
        /// <param name="report">A rendered report which is to be exported.</param>
        /// <param name="stream">A stream for the export of a document.</param>
        /// <param name="zoom">A zoom of the exported document. Default value is 1.</param>
        /// <param name="imageFormat">Specifies a format of the images in the resulted HTML document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        /// <param name="exportMode">A parameter that sets modes for the HTML export.</param>
        /// <param name="exportQuality">A parameter that specifies a quality of images in the resulted HTML document.</param>
        /// <param name="encoding">A parameter that controls the character encoding of the resulted document.</param>
        public void ExportHtml(StiReport report, Stream stream, double zoom, ImageFormat imageFormat,
            StiPagesRange pageRange, StiHtmlExportMode exportMode, StiHtmlExportQuality exportQuality, Encoding encoding)
        {
            var fileStream = stream as FileStream;
            if (fileStream != null) fileName = fileStream.Name;
            var streamWriter = new StreamWriter(stream, encoding);
            HtmlWriter = new StiHtmlTextWriter(streamWriter);
            var pages = pageRange.GetSelectedPages(report.RenderedPages);

            ExportHtml(report, HtmlWriter, zoom, imageFormat, pages, exportMode, exportQuality);

            streamWriter.Flush();
        }


        /// <summary>
        /// Exports a document to the HTML.
        /// </summary>
        /// <param name="report">A rendered report which is to be exported.</param>
        /// <param name="writer">A writer that can write a sequential series of characters to the HTML format.</param>
        /// <param name="zoom">A zoom of the exported document. Default value is 1.</param>
        /// <param name="imageFormat">Specifies a format of images in the resulted HTML document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        public void ExportHtml(StiReport report, StiHtmlTextWriter writer, double zoom, ImageFormat imageFormat,
            StiPagesRange pageRange)
        {
            var pages = pageRange.GetSelectedPages(report.RenderedPages);

            var settings = new StiHtmlExportSettings
            {
                PageRange = pageRange,
                Zoom = zoom,
                ImageFormat = imageFormat
            };

            ExportHtml(report, writer, settings, pages);
        }


        /// <summary>
        /// Exports a document to the HTML.
        /// </summary>
        /// <param name="report">A rendered report which is to be exported.</param>
        /// <param name="writer">A writer that can write a sequential series of characters to the HTML format.</param>
        /// <param name="zoom">A zoom of the exported document. Default value is 1.</param>
        /// <param name="imageFormat">Specifies a format of images in the resulted HTML document.</param>
        /// <param name="pageIndex">An index of the exported page.</param>
        public void ExportHtml(StiReport report, StiHtmlTextWriter writer, double zoom, ImageFormat imageFormat,
            int pageIndex)
        {
            var pages = new StiPagesCollection(report, report.RenderedPages);
            pages.CacheMode = report.RenderedPages.CacheMode;
            pages.AddV2Internal(report.RenderedPages[pageIndex]);

            var settings = new StiHtmlExportSettings
            {
                Zoom = zoom,
                ImageFormat = imageFormat
            };

            ExportHtml(report, writer, settings, pages);
        }

        /// <summary>
        /// Exports a document to the HTML.
        /// </summary>
        /// <param name="report">A rendered report which is to be exported.</param>
        /// <param name="writer">A writer that can write a sequential series of characters to the HTML format.</param>
        /// <param name="zoom">A zoom of the exported document. Default value is 1.</param>
        /// <param name="imageFormat">Specifies a format of images in the resulted HTML document.</param>
        /// <param name="pageIndex">An index of the exported page.</param>
        /// <param name="exportMode">A parameter that sets modes for the HTML export.</param>
        /// <param name="exportQuality">A parameter that specifies a quality of images in the resulted HTML document.</param>
        public void ExportHtml(StiReport report, StiHtmlTextWriter writer, double zoom, ImageFormat imageFormat,
            int pageIndex, StiHtmlExportMode exportMode, StiHtmlExportQuality exportQuality)
        {
            ExportHtml(report, writer, zoom, imageFormat,
                pageIndex, exportMode, exportQuality, StiHtmlExportBookmarksMode.All, 150);
        }


        /// <summary>
        /// Exports a document to the HTML.
        /// </summary>
        /// <param name="report">A rendered report which is to be exported.</param>
        /// <param name="writer">A writer that can write a sequential series of characters to the HTML format.</param>
        /// <param name="zoom">A zoom of the exported document. Default value is 1.</param>
        /// <param name="imageFormat">Specifies a format of images in the resulted HTML document.</param>
        /// <param name="pageIndex">An index of the exported page.</param>
        /// <param name="exportMode">A parameter that sets modes for the HTML export.</param>
        /// <param name="exportQuality">A parameter that specifies a quality of images in the resulted HTML document.</param>
        public void ExportHtml(StiReport report, StiHtmlTextWriter writer, double zoom, ImageFormat imageFormat,
            int pageIndex, StiHtmlExportMode exportMode, StiHtmlExportQuality exportQuality,
            StiHtmlExportBookmarksMode exportBookmarksMode, int bookmarksTreeWidth)
        {
            var pages = new StiPagesCollection(report, report.RenderedPages)
            {
                CacheMode = report.RenderedPages.CacheMode
            };
            pages.AddV2Internal(report.RenderedPages[pageIndex]);

            var settings = new StiHtmlExportSettings
            {
                Zoom = zoom,
                ImageFormat = imageFormat,
                ExportMode = exportMode,
                ExportQuality = exportQuality,
                ExportBookmarksMode = exportBookmarksMode,
                BookmarksTreeWidth = bookmarksTreeWidth
            };

            ExportHtml(report, writer, settings, pages);
        }


        /// <summary>
        /// Exports a document to the HTML.
        /// </summary>
        /// <param name="report">A rendered report which is to be exported.</param>
        /// <param name="writer">A writer that can write a sequential series of characters to the HTML format.</param>
        /// <param name="zoom">A zoom of the exported document. Default value is 1.</param>
        /// <param name="imageFormat">Specifies a format of the images in the resulted HTML document.</param>
        /// <param name="pages">A collection of pages for the export.</param>
        public void ExportHtml(StiReport report, StiHtmlTextWriter writer, double zoom, ImageFormat imageFormat,
            StiPagesCollection pages)
        {
            var settings = new StiHtmlExportSettings
            {
                Zoom = zoom,
                ImageFormat = imageFormat
            };

            ExportHtml(report, writer, settings, pages);
        }


        /// <summary>
        /// Exports a document to the HTML.
        /// </summary>
        /// <param name="report">A rendered report which is to be exported.</param>
        /// <param name="writer">A writer that can write a sequential series of characters to the HTML format.</param>
        /// <param name="zoom">A zoom of the exported document. Default value is 1.</param>
        /// <param name="imageFormat">Specifies a format of images in the resulted HTML document.</param>
        /// <param name="pages">A collection of pages for the export.</param>
        /// <param name="exportMode">A parameter that sets modes for the HTML export.</param>
        /// <param name="exportQuality">A parameter which specifies a quality of images in the resulted HTML document.</param>
        public void ExportHtml(StiReport report, StiHtmlTextWriter writer, double zoom, ImageFormat imageFormat,
            StiPagesCollection pages, StiHtmlExportMode exportMode, StiHtmlExportQuality exportQuality)
        {
            var settings = new StiHtmlExportSettings
            {
                Zoom = zoom,
                ImageFormat = imageFormat,
                ExportMode = exportMode,
                ExportQuality = exportQuality
            };

            ExportHtml(report, writer, settings, pages);
        }


        /// <summary>
        /// Exports a document to the HTML.
        /// </summary>
        /// <param name="report">A rendered report which is to be exported.</param>
        /// <param name="stream">A stream for the export of a document.</param>
        public void ExportHtml(StiReport report, Stream stream, StiHtmlExportSettings settings)
        {
            #region Export Dashboard
            if (!report.IsDocument && report.GetCurrentPage() is IStiDashboard)
            {
                StiDashboardExport.Export(report, stream, settings);
                return;
            }
            #endregion

            var fileStream = stream as FileStream;
            fileName = string.Empty;
            if (fileStream != null) fileName = fileStream.Name;

            MemoryStream ms = null;
            if (settings.CompressToArchive)
            {
                zip = new StiZipWriter20();
                zip.Begin(stream, true);
                ms = new MemoryStream();
                stream = ms;

                try
                {
                    if (!string.IsNullOrEmpty(fileName)) fileName = Path.GetFileNameWithoutExtension(fileName);
                }
                catch
                {
                }
                if (string.IsNullOrEmpty(fileName)) fileName = report.ReportName;
                if (string.IsNullOrEmpty(fileName)) fileName = "report";
            }

            var streamWriter = new StreamWriter(stream, settings.Encoding);
            HtmlWriter = new StiHtmlTextWriter(streamWriter);
            var pages = settings.PageRange.GetSelectedPages(report.RenderedPages);

            ExportHtml(report, HtmlWriter, settings, pages);

            streamWriter.Flush();

            if (settings.CompressToArchive)
            {
                zip.AddFile(fileName + "." + "html", ms);
                zip.End();
                zip = null;
            }
        }


        /// <summary>
        /// Exports a document to the HTML.
        /// </summary>
        public void ExportHtml(StiReport report, StiHtmlTextWriter writer, StiHtmlExportSettings settings, StiPagesCollection pages)
        {
            StiLogService.Write(this.GetType(), "Export report to Html format");

            this.exportSettings = settings;

            #region Read settings
            if (settings == null)
                throw new ArgumentNullException("The 'settings' argument cannot be equal in null.");

            var pageRange = settings.PageRange;
            this.zoom = settings.Zoom;
            this.imageFormat = settings.ImageFormat;
            this.exportQuality = settings.ExportQuality;
            this.exportMode = settings.ExportMode;
            bool useBookmarks = settings.ExportBookmarksMode != StiHtmlExportBookmarksMode.ReportOnly;
            int bookmarksWidth = settings.BookmarksTreeWidth;
            bool exportBookmarksOnly = settings.ExportBookmarksMode == StiHtmlExportBookmarksMode.BookmarksOnly;
            this.useStylesTable = settings.UseStylesTable;
            this.imageResolution = settings.ImageResolution;
            this.imageQuality = settings.ImageQuality;
            this.RemoveEmptySpaceAtBottom = settings.RemoveEmptySpaceAtBottom;
            this.PageHorAlignment = settings.PageHorAlignment;
            this.compressToArchive = settings.CompressToArchive;
            this.useEmbeddedImages = settings.UseEmbeddedImages;
            this.openLinksTarget = settings.OpenLinksTarget;
            this.chartType = settings.ChartType;
            #endregion

            bool modifyGradient = true;

            useBookmarks &= (report.Bookmark != null) && (report.Bookmark.Bookmarks.Count != 0);

            if (exportMode == StiHtmlExportMode.Span) strSpanDiv = "span";
            else if (exportMode == StiHtmlExportMode.Div) strSpanDiv = "div";

            isFileStreamMode = !string.IsNullOrEmpty(fileName);
            if (useEmbeddedImages) isFileStreamMode = false;

            currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                if (HtmlImageHost == null)
                {
                    HtmlImageHost = new StiHtmlImageHost(this);                     // !!!!!
                }

                HtmlImageHost.ImageCache = new StiImageCache(StiOptions.Export.Html.AllowImageComparer, imageFormat, imageQuality);

                var reportCulture = currentCulture;
                if (!string.IsNullOrEmpty(report.Culture))
                {
                    try
                    {
                        reportCulture = new CultureInfo(report.Culture);
                    }
                    catch
                    {
                    }
                }

                Thread.CurrentThread.CurrentCulture = reportCulture;

                this.HtmlWriter = writer;
                this.report = report;

                hashBookmarkGuid = new Hashtable();
                AssembleGuidUsedInBookmark(report.Bookmark, hashBookmarkGuid);

                TotalPageWidth = 0;
                TotalPageHeight = 0;
                startPage = 0;
                imageNumber = 1;

                #region Prepare bookmarksPageIndex
                var bookmarksPageIndex = new Hashtable();
                if (useBookmarks)
                {
                    int tempPageNumber = 0;
                    foreach (StiPage page in pages)
                    {
                        pages.GetPage(page);
                        var components = page.GetComponents();
                        components.Add(page);
                        foreach (StiComponent comp in components)
                        {
                            if (comp.Enabled)
                            {
                                string bookmarkValue = comp.BookmarkValue as string;
                                if (!string.IsNullOrWhiteSpace(bookmarkValue))
                                {
                                    bookmarkValue = bookmarkValue.Replace("'", "");
                                    if (bookmarkValue.Length > 0)
                                    {
                                        if (!bookmarksPageIndex.ContainsKey(bookmarkValue)) bookmarksPageIndex.Add(bookmarkValue, tempPageNumber);
                                    }
                                }
                            }
                        }
                        tempPageNumber++;
                    }
                }
                #endregion

                bool useStimulsoftFont = StiFontIconsExHelper.NeedToUseStimulsoftFont(report);

                #region StiHtmlExportMode.Span || StiHtmlExportMode.Div
                if (exportMode == StiHtmlExportMode.Span || exportMode == StiHtmlExportMode.Div)
                {
                    var tableRender = new StiHtmlTableRender(this, settings, new StiPagesCollection(report, pages));
                    if (IsStopped) return;

                    var isPrinting = pages.Report != null ? pages.Report.IsPrinting : false;

                    #region Prepare coordinates
                    coordX = new SortedList();
                    coordY = new SortedList();
                    hyperlinksToTag = new Hashtable();
                    pointerToBookmark = new Hashtable();
                    pointerToTag = new Hashtable();

                    foreach (StiPage page in pages)
                    {
                        TotalPageWidth = Math.Max(TotalPageWidth, page.Unit.ConvertToHInches(page.Width));
                        TotalPageHeight += page.Unit.ConvertToHInches(page.Height);
                    }

                    foreach (StiPage page in pages)
                    {
                        pages.GetPage(page);

                        this.InvokeExporting(page, pages, 0, 2);

                        var components = page.GetComponents();
                        var pageRect = page.Unit.ConvertToHInches(page.ClientRectangle);
                        if (StiOptions.Export.Html.ExportComponentsFromPageMargins)
                        {
                            var marginLeft = page.Unit.ConvertToHInches(page.Margins.Left);
                            var marginRight = page.Unit.ConvertToHInches(page.Margins.Right);
                            var marginTop = page.Unit.ConvertToHInches(page.Margins.Top);
                            var marginBottom = page.Unit.ConvertToHInches(page.Margins.Bottom);
                            pageRect = new RectangleD(-marginLeft, -marginTop, pageRect.Width + marginLeft + marginRight, pageRect.Height + marginTop + marginBottom);
                        }

                        foreach (StiComponent comp in components)
                        {
                            var rect = page.Unit.ConvertToHInches(comp.DisplayRectangle);
                            bool needAdd = !(isPrinting && !comp.Printable); ;
                            if (comp is StiPointPrimitive) needAdd = false;
                            if ((rect.Right < pageRect.Left) ||
                                (rect.Left > pageRect.Right) ||
                                (rect.Bottom < pageRect.Top) ||
                                (rect.Top > pageRect.Bottom)) needAdd = false;
                            if (comp.Enabled && needAdd)
                            {
                                AddCoord(comp.DisplayRectangle);

                                #region ExceedMargins
                                StiText stiText2 = comp as StiText;
                                if ((stiText2 != null) && (stiText2.ExceedMargins != StiExceedMargins.None) && !(stiText2.Brush is StiEmptyBrush))
                                {
                                    rect = comp.DisplayRectangle;
                                    if ((stiText2.ExceedMargins & StiExceedMargins.Left) > 0)
                                    {
                                        rect.Width += rect.Left + page.Margins.Left;
                                        rect.X = -page.Margins.Left;
                                    }
                                    if ((stiText2.ExceedMargins & StiExceedMargins.Top) > 0)
                                    {
                                        rect.Height += rect.Top + page.Margins.Top;
                                        rect.Y = -page.Margins.Top;
                                    }
                                    if ((stiText2.ExceedMargins & StiExceedMargins.Right) > 0)
                                    {
                                        rect.Width += (page.Width + page.Margins.Right) * 1.006 - rect.Right;
                                    }
                                    if ((stiText2.ExceedMargins & StiExceedMargins.Bottom) > 0)
                                    {
                                        rect.Height += (page.Height + page.Margins.Bottom) * 1.006 - rect.Bottom;
                                    }
                                    AddCoord(rect);
                                }
                                #endregion

                                if (comp.HyperlinkValue != null)
                                {
                                    string hyperlink = comp.HyperlinkValue as string;
                                    if (!string.IsNullOrEmpty(hyperlink) && (hyperlink.Length > 2) && hyperlink.StartsWith("##"))
                                    {
                                        if (hyperlink[2] != '#')
                                            hyperlinksToTag[hyperlink.Substring(2)] = null;
                                    }
                                }

                                if ((comp.PointerValue != null) && !string.IsNullOrWhiteSpace((string)comp.PointerValue) && !string.IsNullOrEmpty(comp.Guid))
                                {
                                    string pointerValue = $"{comp.PointerValue}#GUID#{comp.Guid}";

                                    string bookmarkValue = comp.BookmarkValue as string;
                                    if (!string.IsNullOrWhiteSpace(bookmarkValue))
                                    {
                                        pointerToBookmark[pointerValue] = bookmarkValue;
                                    }

                                    string tag = comp.TagValue as string;
                                    if ((bookmarkValue == null) && !string.IsNullOrEmpty(tag))
                                    {
                                        pointerToTag[pointerValue] = tag;
                                    }
                                }
                            }
                        }
                    }

                    //add only existing tags to bookmark list
                    foreach (DictionaryEntry de in pointerToTag)
                    {
                        if (hyperlinksToTag.Contains(de.Value))
                        {
                            pointerToBookmark[de.Key] = de.Value;
                        }
                    }
                    pointerToTag.Clear();

                    FormatCoords(report);
                    #endregion

                    #region Prepare styles
                    var hashStyles = new Hashtable();
                    var cssStyles = new Hashtable();

                    foreach (StiPage page in pages)
                    {
                        pages.GetPage(page);
                        var components = page.GetComponents();

                        foreach (StiComponent compp in components)
                        {
                            StiComponent comp = compp;
                            if (comp.Enabled)
                            {
                                if (modifyGradient)
                                {
                                    var txt = comp as StiText;
                                    if ((txt != null) && (txt.Brush != null) && !(txt.Brush is StiSolidBrush) && !(txt.Brush is StiEmptyBrush))
                                    {
                                        comp = (StiComponent)compp.Clone();

                                        var backColor = Color.Transparent;
                                        if (txt.Brush is StiGradientBrush) backColor = ((StiGradientBrush)txt.Brush).StartColor;
                                        if (txt.Brush is StiGlareBrush) backColor = ((StiGlareBrush)txt.Brush).StartColor;
                                        if (txt.Brush is StiGlassBrush) backColor = ((StiGlassBrush)txt.Brush).Color;
                                        (comp as StiText).Brush = new StiSolidBrush(backColor);
                                    }
                                }

                                var cellStyle = tableRender.Matrix.GetStyleFromComponent(comp, -1, -1);
                                cellStyle.AbsolutePosition = true;
                                if (!pages.CacheMode)
                                {
                                    hashStyles[compp] = cellStyle;
                                }

                                #region scan tag for css-style
                                string sTag = comp.TagValue as string;
                                if (!string.IsNullOrEmpty(sTag))
                                {
                                    var sTagArray = StiMatrix.SplitTag(sTag);
                                    for (int index = 0; index < sTagArray.Length; index++)
                                    {
                                        if (sTagArray[index].ToLower().StartsWith("css", StringComparison.InvariantCulture))
                                        {
                                            var stArr = StiMatrix.GetStringsFromTag(sTagArray[index], 3);
                                            if (stArr.Length > 1)
                                            {
                                                string styleName = stArr[0].Trim();
                                                cssStyles[styleName] = stArr[1].Trim() + ";position:absolute;";
                                                hashStyles[compp] = styleName;
                                                break;
                                            }
                                        }
                                    }
                                }
                                #endregion
                               
                            }
                        }
                    }
                    tableRender.Matrix.CheckStylesNames();
                    #endregion

                    if (RenderAsDocument) RenderStartDoc(tableRender, false, useBookmarks, exportBookmarksOnly, useStimulsoftFont, cssStyles, pages, settings.Encoding);

                    if (!RenderAsDocument && useBookmarks) RenderBookmarkScript();

                    var topPos = 0d;
                    for (var pageIndex = 0; pageIndex < pages.Count; pageIndex++)
                    {
                        var page = pages[pageIndex];
                        RenderWatermarkImage(page, topPos);
                        RenderWatermarkText(page, topPos);
                        topPos += page.Unit.ConvertToHInches(page.ClientRectangle).Height * (StiOptions.Export.Html.PrintLayoutOptimization ? StiMatrix.htmlScaleY : 1);
                    }
					
                    if (useBookmarks)
                    {
                        HtmlWriter.WriteBeginTag(strSpanDiv + " class=\"dtreeframe\" style=\"");
                        HtmlWriter.WriteStyleAttribute("position", "absolute");
                        if (!exportBookmarksOnly) HtmlWriter.WriteStyleAttribute("height", FormatCoord(TotalPageHeight * zoom * hiToPt));

                        RenderBookmarkTree(report.Bookmark, bookmarksWidth, bookmarksPageIndex);

                        HtmlWriter.WriteEndTag(strSpanDiv);
                        HtmlWriter.WriteLine();
                    }

                    if (!exportBookmarksOnly && !exportSettings.AddPageBreaks) RenderPage(pages, useBookmarks, bookmarksWidth);

                    if (!RenderAsDocument)
                    {
                        tableRender.RenderStyles(useBookmarks, exportBookmarksOnly, cssStyles);
                    }

                    StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");

                    if (!exportBookmarksOnly)
                    {
                        for (int indexPage = 0; indexPage < pages.Count; indexPage++)
                        {
                            var page = pages[indexPage];
                            pages.GetPage(page);

                            this.InvokeExporting(page, pages, 1, 2);
                            if (IsStopped) return;

                            if (exportSettings.AddPageBreaks) RenderPage(pages, useBookmarks, bookmarksWidth, page, indexPage > 0);

                            var components = page.GetComponents();
                            var pageRect = page.Unit.ConvertToHInches(page.ClientRectangle);
                            if (StiOptions.Export.Html.ExportComponentsFromPageMargins)
                            {
                                var pageMarginLeft = page.Unit.ConvertToHInches(page.Margins.Left);
                                var pageMarginRight = page.Unit.ConvertToHInches(page.Margins.Right);
                                var pageMarginTop = page.Unit.ConvertToHInches(page.Margins.Top);
                                var pageMarginBottom = page.Unit.ConvertToHInches(page.Margins.Bottom);
                                pageRect = new RectangleD(-pageMarginLeft, -pageMarginTop, pageRect.Width + pageMarginLeft + pageMarginRight, pageRect.Height + pageMarginTop + pageMarginBottom);
                            }

                            #region ExceedMargins
                            foreach (StiComponent compp in components)
                            {
                                StiText stiText2 = compp as StiText;
                                if (!compp.Enabled || (stiText2 == null) || (stiText2.ExceedMargins == StiExceedMargins.None) || (stiText2.Brush == null) || (stiText2.Brush is StiEmptyBrush)) continue;
                                if ((stiText2.Brush is StiSolidBrush) && (stiText2.Brush as StiSolidBrush).Color.Equals(Color.Transparent)) continue;

                                var rect = page.Unit.ConvertToHInches(compp.DisplayRectangle);
                                bool needAdd = !(isPrinting && !compp.Printable);
                                if ((rect.Right < pageRect.Left) ||
                                    (rect.Left > pageRect.Right) ||
                                    (rect.Bottom < pageRect.Top) ||
                                    (rect.Top > pageRect.Bottom)) needAdd = false;
                                if (rect.Width == 0 || rect.Height == 0) needAdd = false;
                                if (!needAdd) continue;

                                #region Calculate rect
                                rect = compp.DisplayRectangle;
                                if ((stiText2.ExceedMargins & StiExceedMargins.Left) > 0)
                                {
                                    rect.Width += rect.Left + page.Margins.Left;
                                    rect.X = -page.Margins.Left;
                                }
                                if ((stiText2.ExceedMargins & StiExceedMargins.Top) > 0)
                                {
                                    rect.Height += rect.Top + page.Margins.Top;
                                    rect.Y = -page.Margins.Top;
                                }
                                if ((stiText2.ExceedMargins & StiExceedMargins.Right) > 0)
                                {
                                    rect.Width += (page.Width + page.Margins.Right) * 1.006 - rect.Right;
                                }
                                if ((stiText2.ExceedMargins & StiExceedMargins.Bottom) > 0)
                                {
                                    rect.Height += (page.Height + page.Margins.Bottom) * 1.006 - rect.Bottom;
                                }
                                #endregion

                                writer.WriteBeginTag(strSpanDiv);
                                writer.Write(" style=\"");

                                #region Render position
                                var leftD = (double)coordX[rect.Left];
                                var topD = (double)coordY[rect.Top] + startPage * zoom * hiToPt;
                                var widthD = (double)coordX[rect.Right] - (double)coordX[rect.Left];
                                var heightD = (double)coordY[rect.Bottom] - (double)coordY[rect.Top];

                                if (widthD < 0) widthD = 0;
                                if (heightD < 0) heightD = 0;

                                HtmlWriter.WriteStyleAttribute("left", FormatCoord(leftD));
                                HtmlWriter.WriteStyleAttribute("top", FormatCoord(topD));
                                HtmlWriter.WriteStyleAttribute("width", FormatCoord(widthD));
                                HtmlWriter.WriteStyleAttribute("height", FormatCoord(heightD));
                                #endregion

                                HtmlWriter.WriteStyleAttribute("border", "0");
                                HtmlWriter.WriteStyleAttribute("position", "absolute");

                                if ((stiText2.Brush is StiSolidBrush) || (stiText2.Brush is StiGradientBrush) || (stiText2.Brush is StiGlareBrush) || (stiText2.Brush is StiGlassBrush))
                                {
                                    if (stiText2.Brush is StiSolidBrush)
                                    {
                                        RenderBackColor(null, (stiText2.Brush as StiSolidBrush).Color);
                                    }
                                    else
                                    {
                                        ProcessGradientBrushes(stiText2);
                                    }
                                    writer.Write("\">");
                                }
                                else
                                {
                                    writer.Write("\">");

                                    var cont = new StiText();
                                    cont.Page = page;
                                    cont.ClientRectangle = rect;
                                    cont.Brush = stiText2.Brush;
                                    RenderImage(cont);
                                }

                                writer.WriteEndTag(strSpanDiv);
                                writer.WriteLine("");
                            }
                            #endregion

                            List<StiComponent> compsInteraction = new List<StiComponent>();
                            int compIndex = 0;
                            foreach (StiComponent compp in components)
                            {
                                StiComponent comp = compp;

                                var rect = page.Unit.ConvertToHInches(comp.DisplayRectangle);
                                var scaledRect = rect.Multiply(zoom);
                                bool needAdd = !isPrinting || comp.Printable;
                                if (comp is StiPointPrimitive) needAdd = false;
                                if ((rect.Right < pageRect.Left) ||
                                    (rect.Left > pageRect.Right) ||
                                    (rect.Bottom < pageRect.Top) ||
                                    (rect.Top > pageRect.Bottom)) needAdd = false;
                                if (rect.Width == 0 || rect.Height == 0) needAdd = false;

                                if (comp.Enabled && needAdd)
                                {
                                    #region Prepare Class name
                                    string className = null;
                                    var cellStyle = hashStyles[compp] as StiCellStyle;
                                    if (cellStyle == null && pages.CacheMode)
                                    {
                                        cellStyle = tableRender.Matrix.GetStyleFromComponent(compp, -1, -1);
                                        cellStyle.AbsolutePosition = true;
                                    }
                                    int styleIndex = tableRender.Matrix.Styles.IndexOf(cellStyle);
                                    if ((styleIndex != -1) && (useStylesTable))
                                    {
                                        className = "s" + cellStyle.StyleName;
                                    }
                                    string styleName = hashStyles[compp] as string;
                                    if ((!string.IsNullOrEmpty(styleName)) && (useStylesTable))
                                    {
                                        className = styleName;
                                    }
                                    #endregion

                                    var stiText = comp as StiText;

                                    if (modifyGradient)
                                    {
                                        #region modifyGradient
                                        if ((stiText != null) && (stiText.Brush != null) && !(stiText.Brush is StiSolidBrush) && !(stiText.Brush is StiEmptyBrush) && !(stiText.Brush is StiGradientBrush) && !(stiText.Brush is StiGlareBrush) && !(stiText.Brush is StiGlassBrush))
                                        {
                                            writer.WriteBeginTag(strSpanDiv);

                                            if (!string.IsNullOrEmpty(className))
                                            {
                                                writer.WriteAttribute("class", className);
                                            }

                                            writer.Write(" style=\"");
                                            writer.Write("");
                                            RenderPosition(comp);
                                            writer.Write("position:absolute;\">");

                                            var cont = new StiText();
                                            cont.Page = page;
                                            cont.ClientRectangle = comp.ClientRectangle;
                                            cont.Brush = (comp as StiText).Brush;
                                            RenderImage(cont);

                                            writer.WriteEndTag(strSpanDiv);
                                            writer.WriteLine("");

                                            comp = (StiComponent)compp.Clone();
                                            (comp as StiText).Brush = new StiSolidBrush(Color.Transparent);
                                        }
                                        #endregion
                                    }

                                    #region render component
                                    bool isExportAsImage = comp.IsExportAsImage(StiExportFormat.Html);
                                    bool needHyperlink = false;

                                    //if (!isExportAsImage) needHyperlink = RenderHyperlink(comp);
                                    writer.WriteBeginTag(strSpanDiv);

                                    if (!string.IsNullOrEmpty(className))
                                    {
                                        writer.WriteAttribute("class", className);
                                    }

                                    if (exportMode == StiHtmlExportMode.Div)
                                    {
                                        if (comp.ToolTipValue != null)
                                        {
                                            writer.WriteAttribute("title", comp.ToolTipValue.ToString());
                                        }
                                    }

                                    #region HTML5 Viewer Interactions
                                    if (this.RenderWebInteractions)
                                    {
                                        StiComponent compp2 = comp;
                                        StiInteraction interaction = null;
                                        if (compp.Report != null && this.IsComponentHasInteraction(compp))
                                        {
                                            interaction = compp.Interaction;
                                            if (compp is StiContainer)
                                            {
                                                compsInteraction.Add(compp);
                                            }
                                        }

                                        if (interaction == null)
                                        {
                                            double centerX = compp.Left + compp.Width / 2;
                                            double centerY = compp.Top + compp.Height / 2;
                                            foreach (StiComponent comp2 in compsInteraction)
                                            {
                                                if ((centerX > comp2.Left) && (centerX < comp2.Right) && (centerY > comp2.Top) && (centerY < comp2.Bottom))
                                                {
                                                    interaction = comp2.Interaction;
                                                    compp2 = comp2;
                                                }
                                            }
                                        }

                                        if (interaction != null)
                                        {
                                            writer.Write(string.Format(" interaction=\"{0}\"", compp2.Name));

                                            #region Sorting
                                            if (interaction.SortingEnabled)
                                            {
                                                string dataBandName = interaction.GetSortDataBandName();
                                                var dataBand = compp2.Report.GetComponentByName(dataBandName) as StiDataBand;
                                                if (dataBand != null)
                                                {
                                                    writer.Write(string.Format(" databandsort=\"{0};{1}\"", dataBandName, string.Join(";", dataBand.Sort)));

                                                    for (int i = 0; i < dataBand.Sort.Length; i += 2)
                                                    {
                                                        if (dataBand.Sort[i + 1] == interaction.GetSortColumnsString()) writer.Write(string.Format(" sort=\"{0}\"", dataBand.Sort[i].ToLower()));
                                                    }
                                                }
                                            }
                                            #endregion

                                            #region Drill-down
                                            if (interaction.DrillDownEnabled && (!string.IsNullOrEmpty(interaction.DrillDownPageGuid) || !string.IsNullOrEmpty(interaction.DrillDownReport)))
                                            {
                                                if (!string.IsNullOrEmpty(interaction.DrillDownPageGuid)) writer.Write(string.Format(" pageguid=\"{0}\"", interaction.DrillDownPageGuid));
                                                if (!string.IsNullOrEmpty(interaction.DrillDownReport)) writer.Write(string.Format(" reportfile=\"{0}\"", interaction.DrillDownReport));
                                                writer.Write(string.Format(" pageindex=\"{0}\"", compp2.Page.Report.RenderedPages.IndexOf(compp2.Page).ToString()));
                                                writer.Write(string.Format(" compindex=\"{0}\"", compp2.Page.Components.IndexOf(compp2).ToString()));
                                            }
                                            #endregion

                                            #region Collapsing
                                            var bandInteraction = interaction as StiBandInteraction;
                                            if (bandInteraction != null && bandInteraction.CollapsingEnabled && compp2 is StiContainer)
                                            {
                                                var cont = compp2 as StiContainer;
                                                writer.Write(string.Format(" collapsed=\"{0}\"", StiDataBandV2Builder.IsCollapsed(cont, false).ToString().ToLower()));
                                                writer.Write(string.Format(" compindex=\"{0}\"", cont.CollapsingIndex.ToString()));
                                            }
                                            #endregion
                                        }

                                        if (compp.Page != null && compp is IStiEditable && ((IStiEditable)compp).Editable)
                                        {
                                            #region Editable
                                            var attr = new StringBuilder();
                                            int editableIndex = compp.Page.Components.IndexOf(compp);
                                            attr.AppendFormat("{0};", editableIndex);

                                            var checkBox = compp as StiCheckBox;
                                            if (checkBox != null)
                                            {
                                                var backColor = Color.Transparent;
                                                if (checkBox.TextBrush is StiSolidBrush) backColor = ((StiSolidBrush)checkBox.TextBrush).Color;
                                                else if (checkBox.TextBrush is StiGradientBrush) backColor = ((StiGradientBrush)checkBox.TextBrush).StartColor;
                                                else if (checkBox.TextBrush is StiGlareBrush) backColor = ((StiGlareBrush)checkBox.TextBrush).StartColor;
                                                else if (checkBox.TextBrush is StiGlassBrush) backColor = ((StiGlassBrush)checkBox.TextBrush).Color;
                                                else if (checkBox.TextBrush is StiHatchBrush) backColor = ((StiHatchBrush)checkBox.TextBrush).ForeColor;

                                                attr.AppendFormat("CheckBox;{0};{1};{2};#{3:X2}{4:X2}{5:X2};{6};#{7:X2}{8:X2}{9:X2}",
                                                    checkBox.CheckedValue,
                                                    checkBox.CheckStyleForFalse.ToString(),
                                                    checkBox.CheckStyleForTrue.ToString(),
                                                    checkBox.ContourColor.R, checkBox.ContourColor.G, checkBox.ContourColor.B,
                                                    checkBox.Size,
                                                    backColor.R, backColor.G, backColor.B);
                                            }

                                            var textBox = compp as StiText;
                                            if (textBox != null)
                                            {
                                                attr.AppendFormat("Text");
                                                if (textBox.CheckAllowHtmlTags())
                                                {
                                                    attr.AppendFormat(";html");
                                                }
                                            }

                                            var richTextBox = compp as StiRichText;
                                            if (richTextBox != null)
                                            {
                                                attr.AppendFormat("RichText");
                                            }

                                            writer.Write(string.Format(" editable=\"{0}\"", attr.ToString()));
                                            #endregion
                                        }
                                    }
                                    #endregion

                                    writer.Write(" style=\"");

                                    //if (compIndex == components.Count - 1)
                                    //	writer.Write("page-break-after:always;");

                                    if (!isExportAsImage)
                                    {
                                        ProcessGradientBrushes(comp);
                                    }

                                    string stSize = RenderPosition(comp, true);

                                    var bord = comp as IStiBorder;
                                    if ((bord != null) && (bord.Border != null) && bord.Border.DropShadow)
                                    {
                                        Color shadowColor = StiBrush.ToColor(bord.Border.ShadowBrush);
                                        if (!shadowColor.Equals(Color.Transparent))
                                        {
                                            string shadowSt = string.Format("box-shadow: {0}px {0}px 1px 0 rgba({1},{2},{3},{4});",
                                                (int)bord.Border.ShadowSize,
                                                shadowColor.R,
                                                shadowColor.G,
                                                shadowColor.B,
                                                Math.Round(shadowColor.A / 255f, 3));
                                            writer.Write(shadowSt);
                                        }
                                    }

                                    if (!useStylesTable)
                                    {
                                        if (styleIndex != -1)
                                        {
                                            tableRender.RenderStyle(cellStyle);
                                        }
                                        if (!string.IsNullOrEmpty(styleName))
                                        {
                                            writer.WriteLine(cssStyles[styleName] + ";overflow:hidden;");
                                        }
                                    }

                                    var chart = comp as StiChart;
                                    var gauge = comp as StiGauge;
                                    var map = comp as StiMap;
                                    var sparkline = comp as StiSparkline;
                                    if ((chart != null) && (chartType != StiHtmlChartType.Image))
                                    {
                                        #region Chart
                                        writer.Write("\" ");
                                        writer.WriteAttribute("id", GetGuid(chart));
                                        writer.Write(">");
                                        needHyperlink = RenderHyperlink(comp);
                                        PrepareChartData(writer, chart, scaledRect.Width, scaledRect.Height);
                                        if (needHyperlink) writer.WriteEndTag("a");
                                        #endregion
                                    }
                                    else if (gauge != null)
                                    {
                                        writer.Write("\" ");
                                        writer.WriteAttribute("id", GetGuid(gauge));
                                        writer.Write(">");
                                        needHyperlink = RenderHyperlink(comp);
                                        PrepareGaugeData(writer, gauge, scaledRect.Width, scaledRect.Height);
                                        if (needHyperlink) writer.WriteEndTag("a");
                                    }
                                    else if (map != null && map.MapMode == StiMapMode.Choropleth)
                                    {
                                        writer.Write("background-color:transparent;");
                                        writer.Write("\" ");
                                        writer.WriteAttribute("id", GetGuid(map));
                                        writer.WriteAttribute("isRegionMap", "true");
                                        writer.Write(">");
                                        needHyperlink = RenderHyperlink(comp);
                                        PrepareMapData(writer, map, scaledRect.Width, scaledRect.Height);
                                        if (needHyperlink) writer.WriteEndTag("a");
                                    }
                                    else if (sparkline != null && (chartType != StiHtmlChartType.Image))
                                    {
                                        writer.Write("\" ");
                                        writer.WriteAttribute("id", GetGuid(sparkline));
                                        writer.Write(">");
                                        needHyperlink = RenderHyperlink(comp);
                                        PrepareSparklineData(writer, sparkline, scaledRect.Width, scaledRect.Height);
                                        if (needHyperlink) writer.WriteEndTag("a");
                                    }
                                    else if (comp is Stimulsoft.Report.BarCodes.StiBarCode ||
                                        comp is Stimulsoft.Report.Components.StiCheckBox ||
                                        comp is Stimulsoft.Report.Components.StiShape)
                                    {
                                        writer.Write("\">");
                                        var contentSvg = StiSvgHelper.SaveComponentToString(comp);

                                        var rectComp = comp.ComponentToPage(comp.ClientRectangle);
                                        var widthD = (double)coordX[rectComp.Right] - (double)coordX[rectComp.Left];
                                        var heightD = (double)coordY[rectComp.Bottom] - (double)coordY[rectComp.Top];

                                        var svg = String.Format(
                                            "<svg width='{0}' height='{1}'><g transform='translate(1 1) scale({2} {3})'>{4}</g></svg>",
                                            FormatCoord(widthD),
                                            FormatCoord(heightD),
                                            this.zoom * 0.992, //0.992 = 0.716 / 0.72 - 0.002; (0.002 - correction of translate)
                                            this.zoom * 0.992,
                                            contentSvg);
                                        writer.Write(svg);
                                    }
                                    else if (isExportAsImage)
                                    {
                                        #region Image
                                        writer.Write("\">");
                                        needHyperlink = RenderHyperlink(comp);
                                        RenderImage(comp);
                                        if (needHyperlink) writer.WriteEndTag("a");
                                        #endregion
                                    }
                                    else
                                    {                                        
                                        if ((stiText != null) && (stiText.TextOptions != null && stiText.Angle != 0 || stiText.Indicator != null))
                                        {
                                            var contentSvg = StiSvgHelper.SaveComponentToString(comp);

                                            var scaleX = StiOptions.Export.Html.PrintLayoutOptimization ? StiMatrix.htmlScaleX : 1;
                                            var scaleY = StiOptions.Export.Html.PrintLayoutOptimization ? StiMatrix.htmlScaleY : 1;

                                            var svg = string.Format("<svg width=\"{0}\" height=\"{1}\"><g transform=\"scale({2})\">{3}</g></svg>", Math.Ceiling(scaledRect.Width * scaleX), Math.Ceiling(scaledRect.Height * scaleY), this.zoom, contentSvg);
                                            writer.Write("\">");
                                            writer.Write(svg);
                                        }
                                        else
                                        {
                                            #region Text
                                            var textOptions = comp as IStiTextOptions;
                                            if (textOptions != null) RenderTextDirection(null, textOptions.TextOptions);

                                            bool isText = comp is IStiText && !(comp is StiRichText);
                                            string text = null;

                                            #region WordWrap/PreserveWhiteSpaces
                                            bool needProcessPreserveWhiteSpaces = true;
                                            bool needWordwrap = (cellStyle != null) && ((cellStyle.TextOptions != null) && cellStyle.TextOptions.WordWrap || (cellStyle.HorAlignment == StiTextHorAlignment.Width));
                                            if (isText)
                                            {
                                                text = ((IStiText)comp).Text;
                                                if (StiOptions.Export.Html.PreserveWhiteSpaces && !string.IsNullOrWhiteSpace(text) && text.Contains("  "))
                                                {
                                                    if (needWordwrap)
                                                        writer.Write("white-space:pre-wrap;");
                                                    else
                                                        writer.Write("white-space:pre;");
                                                    needProcessPreserveWhiteSpaces = false;
                                                }
                                                if (needProcessPreserveWhiteSpaces && !needWordwrap)
                                                {
                                                    writer.Write("white-space:nowrap");
                                                }
                                            }
                                            #endregion

                                            writer.Write("\">");

                                            if (needWordwrap && StiOptions.Export.Html.UseWordWrapBreakWordMode)
                                            {
                                                writer.WriteBeginTag(strSpanDiv);
                                                writer.Write(string.Format(" style=\"word-wrap:break-word;width:{0};", stSize.Substring(0, stSize.IndexOf(";"))));

                                                if (stiText.VertAlignment != StiVertAlignment.Top)
                                                {
                                                    var sizest = GetSizeStrings(stiText);
                                                    if (stiText.VertAlignment == StiVertAlignment.Center) writer.WriteStyleAttribute("align-items", "center");
                                                    else writer.WriteStyleAttribute("align-items", "baseline");
                                                    //writer.WriteStyleAttribute("max-width", sizest.Width);
                                                    writer.WriteStyleAttribute("height", sizest.Height);
                                                    //writer.WriteStyleAttribute("display", "flex");
                                                    this.RenderVertAlignment(null, stiText.VertAlignment, null, stiText.AllowHtmlTags); //null specially, for emulate winforms (no trimming if wordwrap)
                                                }

                                                writer.Write("\">");
                                            }

                                            needHyperlink = RenderHyperlink(comp);
                                            bool needBr = true;

                                            #region Text content
                                            if (textOptions != null && (!textOptions.TextOptions.WordWrap))
                                            {
                                                if (isText)
                                                {
                                                    if (text != null)
                                                    {
                                                        if (stiText != null && stiText.TextQuality == StiTextQuality.Wysiwyg && !string.IsNullOrEmpty(text) && text.EndsWith(StiTextRenderer.StiForceWidthAlignTag))
                                                        {
                                                            text = text.Substring(0, text.Length - StiTextRenderer.StiForceWidthAlignTag.Length);
                                                        }

                                                        if ((stiText != null) && (stiText.CheckAllowHtmlTags()))
                                                        {
                                                            text = ConvertTextWithHtmlTagsToHtmlText(stiText, text, this.zoom);
                                                        }
                                                        else
                                                        {
                                                            if (StiOptions.Export.Html.ReplaceSpecialCharacters)
                                                            {
                                                                text = text.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\xA0", "&nbsp;");
                                                            }
                                                        }

                                                        if (StiOptions.Export.Html.ConvertDigitsToArabic && textOptions.TextOptions.RightToLeft)
                                                        {
                                                            text = StiExportUtils.ConvertDigitsToArabic(text, StiOptions.Export.Html.ArabicDigitsType);
                                                        }

                                                        bool hasTriming = textOptions != null && textOptions.TextOptions.Trimming != StringTrimming.None;

                                                        if (stiText.VertAlignment == StiVertAlignment.Top)
                                                        {
                                                            writer.Write(PrepareTextForHtml(text, needProcessPreserveWhiteSpaces, stiText.HorAlignment == StiTextHorAlignment.Width));
                                                        }
                                                        else
                                                        {
                                                            var sizest = GetSizeStrings(stiText);
                                                            writer.WriteBeginTag(this.strSpanDiv + " style=\"");
                                                            //writer.WriteStyleAttribute("display", "table-cell"); 
                                                            writer.WriteStyleAttribute("max-width", sizest.Width);
                                                            writer.WriteStyleAttribute("height", sizest.Height);
                                                            this.RenderVertAlignment(null, stiText.VertAlignment);
                                                            if (hasTriming)
                                                            {
                                                                this.RenderTextHorAlignment(null, textOptions?.TextOptions, stiText.HorAlignment);
                                                                writer.Write("\">");
                                                                writer.WriteBeginTag(this.strSpanDiv + " style=\"");
                                                                writer.WriteStyleAttribute("text-overflow", "ellipsis");
                                                                writer.WriteStyleAttribute("overflow", "hidden");
                                                                writer.WriteStyleAttribute("white-space", "nowrap");
                                                            }
                                                            this.RenderTextHorAlignment(null, textOptions?.TextOptions, stiText.HorAlignment);
                                                            writer.Write("\">");

                                                            writer.Write(PrepareTextForHtml(text, needProcessPreserveWhiteSpaces, stiText.HorAlignment == StiTextHorAlignment.Width));

                                                            if (hasTriming)
                                                            {
                                                                writer.WriteEndTag(this.strSpanDiv);
                                                            }

                                                            writer.WriteEndTag(this.strSpanDiv);
                                                        }
                                                    }
                                                    needBr = false;
                                                }
                                            }
                                            else
                                            {
                                                if (isText)
                                                {
                                                    if (text != null)
                                                    {
                                                        if (stiText != null && stiText.TextQuality == StiTextQuality.Wysiwyg && !string.IsNullOrEmpty(text) && text.EndsWith(StiTextRenderer.StiForceWidthAlignTag))
                                                        {
                                                            text = text.Substring(0, text.Length - StiTextRenderer.StiForceWidthAlignTag.Length);
                                                        }

                                                        if ((stiText != null) && (stiText.CheckAllowHtmlTags()))
                                                        {
                                                            text = ConvertTextWithHtmlTagsToHtmlText(stiText, text, this.zoom);
                                                        }
                                                        else
                                                        {
                                                            if ((stiText != null) &&
                                                                (StiOptions.Export.Html.ForceWysiwygWordwrap) &&
                                                                (!stiText.CheckAllowHtmlTags()) &&
                                                                (stiText.TextQuality == StiTextQuality.Wysiwyg) &&
                                                                (textOptions != null) &&
                                                                (textOptions.TextOptions.WordWrap))
                                                            {
                                                                var newTextLines = StiTextRenderer.GetTextLines(
                                                                    report.ReportMeasureGraphics,
                                                                    ref text,
                                                                    stiText.Font,
                                                                    page.Unit.ConvertToHInches(comp.ComponentToPage(comp.ClientRectangle)),
                                                                    1,
                                                                    true,
                                                                    textOptions.TextOptions.RightToLeft,
                                                                    1,
                                                                    textOptions.TextOptions.Angle,
                                                                    textOptions.TextOptions.Trimming,
                                                                    stiText.CheckAllowHtmlTags(),
                                                                    textOptions.TextOptions,
                                                                    1);
                                                                string delimiter = "\n";
                                                                var sb = new StringBuilder();
                                                                for (int index = 0; index < newTextLines.Count; index++)
                                                                {
                                                                    string st = newTextLines[index];
                                                                    sb.Append(st);
                                                                    if (index < newTextLines.Count - 1) sb.Append(delimiter);
                                                                }
                                                                text = sb.ToString();
                                                            }

                                                            if (StiOptions.Export.Html.ReplaceSpecialCharacters)
                                                            {
                                                                text = text.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\xA0", "&nbsp;");
                                                            }
                                                        }

                                                        if (stiText.VertAlignment == StiVertAlignment.Top)
                                                        {
                                                            writer.Write(PrepareTextForHtml(text, needProcessPreserveWhiteSpaces, stiText.HorAlignment == StiTextHorAlignment.Width));
                                                        }
                                                        else
                                                        {
                                                            var sizest = GetSizeStrings(stiText);
                                                            writer.WriteBeginTag(this.strSpanDiv + " style=\"");
                                                            writer.WriteStyleAttribute("overflow", "hidden");
                                                            writer.WriteStyleAttribute("max-width", sizest.Width);
                                                            writer.WriteStyleAttribute("width", stSize.Substring(0, stSize.IndexOf(";")));

                                                            if (needWordwrap && StiOptions.Export.Html.UseWordWrapBreakWordMode)
                                                            {
                                                                writer.WriteStyleAttribute("display", "table-cell");
                                                            }
                                                            else
                                                            {
                                                                this.RenderVertAlignment(null, stiText.VertAlignment, textOptions?.TextOptions, stiText.AllowHtmlTags);
                                                            }
                                                            this.RenderTextHorAlignment(null, textOptions?.TextOptions, stiText.HorAlignment);
                                                            writer.Write("\">");

                                                            writer.Write(PrepareTextForHtml(text, needProcessPreserveWhiteSpaces, stiText.HorAlignment == StiTextHorAlignment.Width));

                                                            writer.WriteEndTag(this.strSpanDiv);
                                                        }
                                                    }
                                                    needBr = false;
                                                }
                                            }
                                            #endregion

                                            if (needHyperlink)
                                            {
                                                if (needBr) writer.Write("<br>");
                                                writer.WriteEndTag("a");
                                            }
                                            if (needWordwrap && StiOptions.Export.Html.UseWordWrapBreakWordMode)
                                            {
                                                writer.WriteEndTag(strSpanDiv);
                                            }
                                            #endregion
                                        }
                                    }

                                    writer.WriteEndTag(strSpanDiv);

                                    //if (!isExportAsImage && needHyperlink) writer.WriteEndTag("a");

                                    writer.WriteLine("");
                                    compIndex++;
                                    #endregion
                                }
                            }

                            if (exportSettings.AddPageBreaks)
                            {
                                #region Trial
#if CLOUD
                                var isTrial2 = !StiCloudPlan.IsReportsAvailable(report != null ? report.ReportGuid : null);
#elif SERVER
                                var isTrial2 = StiVersionX.IsSvr;
#else
                                var key2 = StiLicenseKeyValidator.GetLicenseKey();
                                var isValidInDesigner2 = StiLicenseKeyValidator.IsValidInReportsDesignerOrOnPlatform(StiProductIdent.Net, key2) && Base.Design.StiDesignerAppStatus.IsRunning;
                                var isTrial2 = !(isValidInDesigner2 || (RenderWebViewer
                                    ? StiLicenseKeyValidator.IsValidOnWebFramework(key2)
                                    : StiLicenseKeyValidator.IsValidOnNetFramework(key2)));

                                if (!typeof(StiLicense).AssemblyQualifiedName.Contains(StiPublicKeyToken.Key))
                                    isTrial2 = true;

                                #region IsValidLicenseKey
                                if (!isTrial2)
                                {
                                    try
                                    {
                                        using (var rsa = new RSACryptoServiceProvider(512))
                                        using (var sha = new SHA1CryptoServiceProvider())
                                        {
                                            rsa.FromXmlString("<RSAKeyValue><Modulus>iyWINuM1TmfC9bdSA3uVpBG6cAoOakVOt+juHTCw/gxz/wQ9YZ+Dd9vzlMTFde6HAWD9DC1IvshHeyJSp8p4H3qXUKSC8n4oIn4KbrcxyLTy17l8Qpi0E3M+CI9zQEPXA6Y1Tg+8GVtJNVziSmitzZddpMFVr+6q8CRi5sQTiTs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                                            isTrial2 = !rsa.VerifyData(key2.GetCheckBytes(), sha, key2.GetSignatureBytes());
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        isTrial2 = true;
                                    }
                                }
                                #endregion
#endif
                                if (page != null && isTrial2)
                                {
                                    var rectPage = page.Unit.ConvertToHInches(page.ClientRectangle);
                                    var fontSize = (int)(100 * zoom);
                                    writer.WriteLine(string.Format("<div style=\"position: absolute; pointer-events: none; filter: alpha(Opacity=30); opacity: 0.3;" +
                                        " -moz-opacity: 0.3; -khtml-opacity: 0.3; font-size: {1}px; font-weight: bold; width: {2}; margin-top: {3}; text-align: center;" +
                                        " font-family: Arial; color: black; z-index: 9999; -ms-transform: rotate(-45deg); -webkit-transform: rotate(-45deg); transform: rotate(-45deg);\">{4}</div>",
                                        strSpanDiv,
                                        fontSize,
                                        StiHtmlUnit.NewUnit(rectPage.Width * zoom, StiOptions.Export.Html.PrintLayoutOptimization),
                                        StiHtmlUnit.NewUnit(rectPage.Height * zoom / 2 - fontSize, StiOptions.Export.Html.PrintLayoutOptimization),
                                        "Trial"));
                                }
                                #endregion

                                RenderEndPage();
                            }
                            else
                            {
                                startPage += (int)page.Unit.ConvertToHInches(page.Height);
                            }

                            writer.WriteLine("<!-- end page -->");
                        }
                    }

                    #region Trial
#if CLOUD
                    var isTrial = !StiCloudPlan.IsReportsAvailable(report != null ? report.ReportGuid : null);
#elif SERVER
                    var isTrial = StiVersionX.IsSvr;
#else
				    var key = StiLicenseKeyValidator.GetLicenseKey();
                    var isValidInDesigner = StiLicenseKeyValidator.IsValidInReportsDesignerOrOnPlatform(StiProductIdent.Net, key) && Base.Design.StiDesignerAppStatus.IsRunning;
                    var isTrial = !(isValidInDesigner || (RenderWebViewer
                        ? StiLicenseKeyValidator.IsValidOnWebFramework(key) 
                        : StiLicenseKeyValidator.IsValidOnNetFramework(key)));

                    if (!typeof(StiLicense).AssemblyQualifiedName.Contains(StiPublicKeyToken.Key))
                        isTrial = true;

                    #region IsValidLicenseKey
				    if (!isTrial)
				    {
				        try
				        {
				            using (var rsa = new RSACryptoServiceProvider(512))
				            using (var sha = new SHA1CryptoServiceProvider())
				            {
				                rsa.FromXmlString("<RSAKeyValue><Modulus>iyWINuM1TmfC9bdSA3uVpBG6cAoOakVOt+juHTCw/gxz/wQ9YZ+Dd9vzlMTFde6HAWD9DC1IvshHeyJSp8p4H3qXUKSC8n4oIn4KbrcxyLTy17l8Qpi0E3M+CI9zQEPXA6Y1Tg+8GVtJNVziSmitzZddpMFVr+6q8CRi5sQTiTs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
				                isTrial = !rsa.VerifyData(key.GetCheckBytes(), sha, key.GetSignatureBytes());
				            }
				        }
				        catch (Exception)
				        {
				            isTrial = true;
				        }
				    }
                    #endregion
#endif
                    if (pages.Count > 0 && isTrial && !exportSettings.AddPageBreaks)
                    {
                        var rectPage = pages[0].Unit.ConvertToHInches(pages[0].ClientRectangle);
                        var fontSize = (int)(100 * zoom);
                        writer.WriteLine(string.Format("<div style=\"position: absolute; pointer-events: none; filter: alpha(Opacity=30); opacity: 0.3;" +
                            " -moz-opacity: 0.3; -khtml-opacity: 0.3; font-size: {1}px; font-weight: bold; width: {2}; margin-top: {3}; text-align: center;" +
                            " font-family: Arial; color: black; z-index: 9999; -ms-transform: rotate(-45deg); -webkit-transform: rotate(-45deg); transform: rotate(-45deg);\">{4}</div>",
                            strSpanDiv,
                            fontSize,
                            StiHtmlUnit.NewUnit(rectPage.Width * zoom, StiOptions.Export.Html.PrintLayoutOptimization),
                            StiHtmlUnit.NewUnit(rectPage.Height * zoom / 2 - fontSize, StiOptions.Export.Html.PrintLayoutOptimization),
                            "Trial"));
                    }
                    #endregion

                    if (!exportSettings.AddPageBreaks) RenderEndPage();

                    if (RenderAsDocument) RenderChartScripts();

                    if (RenderAsDocument) RenderEndDoc();
                }
                #endregion

                #region StiHtmlExportMode.Table
                if (exportMode == StiHtmlExportMode.Table)
                {
                    CurrentPassNumber = 0;
                    MaximumPassNumber = 3;
                    TableRender = new StiHtmlTableRender(this, settings, pages);
                    if (IsStopped) return;

                    #region check for css-styles
                    var cssStyles = new Hashtable();
                    bool[,] readyCells = new bool[TableRender.Matrix.CoordY.Count, TableRender.Matrix.CoordX.Count];
                    for (int rowIndex = 1; rowIndex < TableRender.Matrix.CoordY.Count; rowIndex++)
                    {
                        for (int columnIndex = 1; columnIndex < TableRender.Matrix.CoordX.Count; columnIndex++)
                        {
                            if (!readyCells[rowIndex - 1, columnIndex - 1])
                            {
                                var cell = TableRender.Matrix.Cells[rowIndex - 1, columnIndex - 1];
                                if (cell != null)
                                {
                                    #region range
                                    for (int yy = 0; yy <= cell.Height; yy++)
                                    {
                                        for (int xx = 0; xx <= cell.Width; xx++)
                                        {
                                            readyCells[rowIndex - 1 + yy, columnIndex - 1 + xx] = true;
                                        }
                                    }
                                    #endregion

                                    if (cell.Component != null)
                                    {
                                        #region scan tag for css-style
                                        var sTag = cell.Component.TagValue as string;
                                        if (!string.IsNullOrEmpty(sTag))
                                        {
                                            var sTagArray = StiMatrix.SplitTag(sTag);
                                            for (int index = 0; index < sTagArray.Length; index++)
                                            {
                                                if (sTagArray[index].ToLower().StartsWith("css", StringComparison.InvariantCulture))
                                                {
                                                    var stArr = StiMatrix.GetStringsFromTag(sTagArray[index], 3);
                                                    if (stArr.Length > 1)
                                                    {
                                                        var styleName = stArr[0].Trim();
                                                        cssStyles[styleName] = stArr[1].Trim();
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    if (RenderAsDocument) RenderStartDoc(TableRender, true, useBookmarks, exportBookmarksOnly, useStimulsoftFont, cssStyles, pages, settings.Encoding);

                    if (!RenderAsDocument && useBookmarks) RenderBookmarkScript();

                    if (useBookmarks)
                    {
                        HtmlWriter.WriteBeginTag("table");
                        if (PageHorAlignment != StiHorAlignment.Left)
                        {
                            HtmlWriter.WriteAttribute("width", "100%");
                        }
                        if (StiOptions.Export.Html.UseExtendedStyle)
                        {
                            HtmlWriter.WriteAttribute("class", "sBaseStyleFix");
                        }
                        HtmlWriter.Write(">");

                        if (StiOptions.Export.Html.UseExtendedStyle)
                        {
                            writer.WriteBeginTag("tbody");
                            writer.WriteAttribute("class", "sBaseStyleFix");
                            writer.WriteLine(">");
                        }

                        HtmlWriter.WriteBeginTag("tr");
                        if (StiOptions.Export.Html.UseExtendedStyle)
                        {
                            HtmlWriter.WriteAttribute("class", "sBaseStyleFix");
                        }
                        HtmlWriter.Write(">");
                        HtmlWriter.WriteBeginTag("td class=\"dtreeframe\" style=\"");
                        HtmlWriter.WriteStyleAttribute("vertical-align", "top");

                        RenderBookmarkTree(report.Bookmark, bookmarksWidth, bookmarksPageIndex);

                        HtmlWriter.WriteEndTag("td");
                        HtmlWriter.WriteLine();
                        HtmlWriter.WriteBeginTag("td");
                        if (StiOptions.Export.Html.UseExtendedStyle)
                        {
                            HtmlWriter.WriteAttribute("class", "sBaseStyleFix");
                        }
                        HtmlWriter.WriteLine(">");
                        HtmlWriter.Indent++;
                    }

                    var topPos = 0d;
                    for (var pageIndex = 0; pageIndex < pages.Count; pageIndex++)
                    {
                        var page = pages[pageIndex];
                        RenderWatermarkImage(page, topPos);
                        RenderWatermarkText(page, topPos);
                        topPos += page.Unit.ConvertToHInches(page.ClientRectangle).Height * (StiOptions.Export.Html.PrintLayoutOptimization ? StiMatrix.htmlScaleY : 1);
                    }

                    #region Trial
#if CLOUD
                    var isTrial = !StiCloudPlan.IsReportsAvailable(report != null ? report.ReportGuid : null);
#elif SERVER
                    var isTrial = StiVersionX.IsSvr;
#else
                    var key = StiLicenseKeyValidator.GetLicenseKey();
                    var isValidInDesigner = StiLicenseKeyValidator.IsValidInReportsDesignerOrOnPlatform(StiProductIdent.Net, key) && Base.Design.StiDesignerAppStatus.IsRunning;
                    var isTrial = !(isValidInDesigner || (RenderWebViewer
                        ? StiLicenseKeyValidator.IsValidOnWebFramework(key)
                        : StiLicenseKeyValidator.IsValidOnNetFramework(key)));

                    if (!typeof(StiLicense).AssemblyQualifiedName.Contains(StiPublicKeyToken.Key))
                        isTrial = true;

                    #region IsValidLicenseKey
				    if (!isTrial)
				    {
				        try
				        {
				            using (var rsa = new RSACryptoServiceProvider(512))
				            using (var sha = new SHA1CryptoServiceProvider())
				            {
				                rsa.FromXmlString("<RSAKeyValue><Modulus>iyWINuM1TmfC9bdSA3uVpBG6cAoOakVOt+juHTCw/gxz/wQ9YZ+Dd9vzlMTFde6HAWD9DC1IvshHeyJSp8p4H3qXUKSC8n4oIn4KbrcxyLTy17l8Qpi0E3M+CI9zQEPXA6Y1Tg+8GVtJNVziSmitzZddpMFVr+6q8CRi5sQTiTs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
				                isTrial = !rsa.VerifyData(key.GetCheckBytes(), sha, key.GetSignatureBytes());
				            }
				        }
				        catch (Exception)
				        {
				            isTrial = true;
				        }
				    }
                    #endregion
#endif
                    if (pages.Count > 0 && isTrial)
                    {
                        var rectPage = pages[0].Unit.ConvertToHInches(pages[0].ClientRectangle);
                        var fontSize = (int)(100 * zoom);
                        writer.WriteLine(string.Format("<div style=\"position: absolute; pointer-events: none; filter: alpha(Opacity=30); opacity: 0.3; -moz-opacity: 0.3;" +
                            " -khtml-opacity: 0.3; font-size: {1}px; font-weight: bold; width: {2}; margin-top: {3}; text-align: center; font-family: Arial; color: black;" +
                            " z-index: 9999; -ms-transform: rotate(-45deg); -webkit-transform: rotate(-45deg); transform: rotate(-45deg);\">{4}</div>",
                            strSpanDiv,
                            fontSize,
                            StiHtmlUnit.NewUnit(rectPage.Width * zoom, StiOptions.Export.Html.PrintLayoutOptimization),
                            StiHtmlUnit.NewUnit(rectPage.Height * zoom / 2 - fontSize, StiOptions.Export.Html.PrintLayoutOptimization),
                            "Trial"));
                    }
                    #endregion

                    if (!exportBookmarksOnly) {
                        var page = pages.Count > 0 ? pages[0] : null;
                        var watermarkShowBehind = page != null && ((page.Watermark.Image != null || !String.IsNullOrEmpty(page.Watermark.ImageHyperlink)) &&
                            page.Watermark.ShowImageBehind || !String.IsNullOrEmpty(page.Watermark.Text) && page.Watermark.ShowBehind);

                        TableRender.RenderTable((!RenderAsDocument) && RenderStyles, null, useBookmarks, exportBookmarksOnly, cssStyles, watermarkShowBehind, page?.Border);
                    }

                    if (IsStopped) return;

                    if (useBookmarks)
                    {
                        HtmlWriter.Indent--;
                        HtmlWriter.WriteEndTag("td");
                        HtmlWriter.WriteEndTag("tr");
                        HtmlWriter.WriteEndTag("tbody");
                        HtmlWriter.WriteEndTag("table");
                        HtmlWriter.WriteLine();
                    }

                    if (RenderAsDocument) RenderChartScripts();

                    if (RenderAsDocument) RenderEndDoc();
                }
                #endregion

                writer.Flush();
            }
            finally
            {
                StiExportUtils.EnableFontSmoothing(report);
                Thread.CurrentThread.CurrentCulture = currentCulture;

                if (!HtmlImageHost.IsMhtExport) HtmlImageHost.ImageCache.Clear();

                report = null;
                if (ClearOnFinish) Clear();
                //htmlImageHost = null;
            }
        }
        #endregion

        /// <summary>
        /// Creates an instance of the class for the HTML export.
        /// </summary>
        public StiHtmlExportService()
        {
            numberFormat.NumberDecimalSeparator = ".";
        }
    }
}