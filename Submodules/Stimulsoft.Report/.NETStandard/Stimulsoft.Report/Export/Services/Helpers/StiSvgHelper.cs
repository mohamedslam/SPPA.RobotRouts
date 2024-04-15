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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.IO.Compression;
using System.Globalization;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.BarCodes;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.ShapeTypes;
using Stimulsoft.Report.Design;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Maps;
using Stimulsoft.Report.Export.Services.Helpers;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Painters;
using Stimulsoft.Base.Context;
using Stimulsoft.Report.Helpers;
using System.Drawing;
using System.Drawing.Imaging;

#if NETSTANDARD
using Stimulsoft.System.Drawing;
#endif

#if STIDRAWING
using FontFamily = Stimulsoft.Drawing.FontFamily;
using Font = Stimulsoft.Drawing.Font;
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
using ImageCodecInfo = Stimulsoft.Drawing.Imaging.ImageCodecInfo;
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
using ImageEncoder = Stimulsoft.Drawing.Imaging.Encoder;
using EncoderParameter = Stimulsoft.Drawing.Imaging.EncoderParameter;
using EncoderParameters = Stimulsoft.Drawing.Imaging.EncoderParameters;
#else
using ImageEncoder = System.Drawing.Imaging.Encoder;
#endif

namespace Stimulsoft.Report.Export
{
    public class StiSvgHelper
    {
        #region Constants
        const double correctFontSize = 1.35;
        const float pdfCKT = 0.56f;     //circle round koefficient for Bezier curve
        #endregion

        #region GetLineStyleDash
        public static string GetLineStyleDash(StiPenStyle penStyle, double width)
        {
            string dotWidth = Math.Round(1.2 * width, 1).ToString().Replace(",", ".");
            string lineWidth = Math.Round(3 * width, 1).ToString().Replace(",", ".");
            string dashArray = string.Empty;
            switch (penStyle)
            {
                case StiPenStyle.Dot:
                    dashArray = string.Format("{0},{0}", dotWidth);
                    break;

                case StiPenStyle.Dash:
                    dashArray = string.Format("{0},{1}", lineWidth, dotWidth);
                    break;

                case StiPenStyle.DashDot:
                    dashArray = string.Format("{0},{1},{1},{1}", lineWidth, dotWidth);
                    break;

                case StiPenStyle.DashDotDot:
                    dashArray = string.Format("{0},{1},{1},{1},{1},{1}", lineWidth, dotWidth);
                    break;
            }
            return dashArray;
        }
        #endregion

        #region ToUnits
        public static string ToUnits(double number, int digitsLimit = 2)
        {
            //at this moment in user units - px
            string output = Math.Round(number, digitsLimit).ToString().Replace(",", ".");
            return output;
        }
        #endregion

        #region  TODO
        private void GetTextLinesAndWidths()
        {
            //if (StiOptions.Export.Pdf.WinFormsHighAccuracyMode == StiPdfHighAccuracyMode.All)
            //{
            //RectangleD rect = textComp.GetPaintRectangle(true, false);
            //rect = textComp.ConvertTextMargins(rect, false);
            //rect = textComp.ConvertTextBorders(rect, false);

            //float zoom = 1f;
            //StringFormat sf = StiTextDrawing.GetStringFormat(
            //    textOpt == null ? new StiTextOptions() : textOpt.TextOptions,
            //    mTextHorAlign == null ? StiTextHorAlignment.Left : mTextHorAlign.HorAlignment,
            //    mVertAlign == null ? StiVertAlignment.Bottom : mVertAlign.VertAlignment,
            //    textComp.TextQuality == StiTextQuality.Typographic,
            //    zoom);

            //Image img = new Bitmap(1, 1);
            //Graphics g = Graphics.FromImage(img);

            //var newList = new List<string>();
            //for (int indexLine = 0; indexLine < stringList.Count; indexLine++)
            //{
            //    string stt = (string)stringList[indexLine];

            //    if (stt.Length == 0)
            //    {
            //        newList.Add(string.Empty);
            //        continue;
            //    }

            //    //prepare ranges
            //    Region[] ranges = new Region[stt.Length];
            //    int indexRange = 0;
            //    while (indexRange < stt.Length)
            //    {
            //        int count = stt.Length - indexRange;
            //        if (count > 32) count = 32;

            //        CharacterRange[] cr = new CharacterRange[count];
            //        for (int index2 = 0; index2 < count; index2++)
            //        {
            //            cr[index2].First = indexRange + index2;
            //            cr[index2].Length = 1;
            //        }
            //        sf.SetMeasurableCharacterRanges(cr);

            //        g.MeasureCharacterRanges(stt, textComp.Font, rect.ToRectangleF(), sf).CopyTo(ranges, indexRange);

            //        indexRange += count;
            //    }

            //    //RectangleF[] recta = new RectangleF[ranges.Length];
            //    //for (int index = 0; index < ranges.Length; index++)
            //    //{
            //    //    recta[index] = ranges[index].GetBounds(g);
            //    //}

            //    //calculate wordwrap points
            //    RectangleF rectf = ranges[0].GetBounds(g);
            //    double lastCenter = rectf.Top + rectf.Height / 2d;
            //    int pos = 0;
            //    int skip = 0;
            //    for (int index = 1; index < stt.Length; index++)
            //    {
            //        if (skip == 0)
            //        {
            //            int count = 15;
            //            if (index + count > stt.Length - 1) count = stt.Length - index - 1;
            //            if (count > 0)
            //            {
            //                rectf = ranges[index + count].GetBounds(g);
            //                if (rectf.Top < lastCenter)
            //                {
            //                    index += count;
            //                    continue;
            //                }
            //                else
            //                {
            //                    skip = count;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            skip--;
            //        }
            //        rectf = ranges[index].GetBounds(g);
            //        if (rectf.Top > lastCenter)
            //        {
            //            //new line
            //            newList.Add(stt.Substring(pos, index - pos).Replace("\r", "").Replace("\n", "") + ((index < stt.Length) && needWidthAlign ? "\a" : ""));
            //            pos = index;
            //            lastCenter = rectf.Top + rectf.Height / 2d;
            //            skip = 0;
            //        }
            //    }
            //    if (pos < stt.Length) newList.Add(stt.Substring(pos, stt.Length - pos).Replace("\r", "").Replace("\n", ""));
            //}
            //stringList = newList;
            //}
        }
        #endregion

        #region Write methods
        private static void WriteCoordinates(XmlTextWriter writer, StiSvgData svgData)
        {
            writer.WriteAttributeString("x", ToUnits(svgData.X));
            writer.WriteAttributeString("y", ToUnits(svgData.Y));
            writer.WriteAttributeString("width", ToUnits(svgData.Width));
            writer.WriteAttributeString("height", ToUnits(svgData.Height));
        }

        private static void WriteStrokeInfo(XmlTextWriter writer, Color color, double width, StiPenStyle style)
        {
            if (style == StiPenStyle.None)
            {
                writer.WriteAttributeString("stroke", "none");
            }
            else
            {
                writer.WriteAttributeString("stroke", string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B));
                if (color.A != 0xFF)
                {
                    writer.WriteAttributeString("stroke-opacity", Math.Round(color.A / 255f, 3).ToString().Replace(",", "."));
                }
                writer.WriteAttributeString("stroke-width", ToUnits(width));
                if (style != StiPenStyle.Solid)
                {
                    writer.WriteAttributeString("stroke-dasharray", GetLineStyleDash(style, width));
                }
            }
        }

        private static void WriteFillInfo(XmlTextWriter writer, Color color)
        {
            writer.WriteAttributeString("fill", string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B));
            if (color.A != 0xFF)
            {
                writer.WriteAttributeString("fill-opacity", Math.Round(color.A / 255f, 3).ToString().Replace(",", "."));
            }
        }
        #endregion

        #region CheckShape
        internal static bool CheckShape(StiComponent component)
        {
            if (!(component is StiShape)) return false;
            StiShape shape = component as StiShape;
            if (shape == null) return false;
            IStiBrush mBrush = component as IStiBrush;
            if ((mBrush != null) && (mBrush.Brush != null) && !((mBrush.Brush is StiSolidBrush) || (mBrush.Brush is StiEmptyBrush))) return false;
            if (
                (shape.ShapeType is StiVerticalLineShapeType) ||
                (shape.ShapeType is StiHorizontalLineShapeType) ||
                (shape.ShapeType is StiTopAndBottomLineShapeType) ||
                (shape.ShapeType is StiLeftAndRightLineShapeType) ||
                (shape.ShapeType is StiRectangleShapeType) ||
                (shape.ShapeType is StiRoundedRectangleShapeType) ||
                (shape.ShapeType is StiDiagonalDownLineShapeType) ||
                (shape.ShapeType is StiDiagonalUpLineShapeType) ||
                (shape.ShapeType is StiTriangleShapeType) ||
                (shape.ShapeType is StiOvalShapeType) ||
                (shape.ShapeType is StiArrowShapeType) ||
                (shape.ShapeType is StiOctagonShapeType) ||
                (shape.ShapeType is StiComplexArrowShapeType) ||
                (shape.ShapeType is StiBentArrowShapeType) ||
                (shape.ShapeType is StiChevronShapeType) ||
                (shape.ShapeType is StiDivisionShapeType) ||
                (shape.ShapeType is StiEqualShapeType) ||
                (shape.ShapeType is StiFlowchartCardShapeType) ||
                (shape.ShapeType is StiFlowchartCollateShapeType) ||
                (shape.ShapeType is StiFlowchartDecisionShapeType) ||
                (shape.ShapeType is StiFlowchartManualInputShapeType) ||
                (shape.ShapeType is StiFlowchartOffPageConnectorShapeType) ||
                (shape.ShapeType is StiFlowchartPreparationShapeType) ||
                (shape.ShapeType is StiFlowchartSortShapeType) ||
                (shape.ShapeType is StiFrameShapeType) ||
                (shape.ShapeType is StiMinusShapeType) ||
                (shape.ShapeType is StiMultiplyShapeType) ||
                (shape.ShapeType is StiParallelogramShapeType) ||
                (shape.ShapeType is StiPlusShapeType) ||
                (shape.ShapeType is StiRegularPentagonShapeType) ||
                (shape.ShapeType is StiTrapezoidShapeType) ||
                (shape.ShapeType is StiSnipSameSideCornerRectangleShapeType) ||
                (shape.ShapeType is StiSnipDiagonalSideCornerRectangleShapeType)
                )
            {
                return true;
            }
            return false;
        }
        #endregion

        #region WriteDocument
        private static MemoryStream WriteDocument(StiReport report, StiPage page, bool standalone, ImageFormat imageFormat, float imageQuality, float imageResolution)
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);

            //#if TESTSVG
            int xmlIndentation = -1;
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            //#else
            //                writer.Indentation = 0;
            //                writer.Formatting = Formatting.None;
            //#endif

            ImageCodecInfo imageCodec = StiImageCodecInfo.GetImageCodec("image/jpeg");
            imageResolution = imageResolution / 100f;

            Hashtable guids = new Hashtable();

            if (standalone) writer.WriteStartDocument();
            writer.WriteStartElement("svg");
            writer.WriteAttributeString("version", "1.1");
            writer.WriteAttributeString("baseProfile", "full");
            if (!standalone)
            {
                writer.WriteAttributeString("style", "margin:5px; border:1px solid DarkGrey;");
            }
            writer.WriteAttributeString("xmlns", "http://www.w3.org/2000/svg");
            writer.WriteAttributeString("xmlns:xlink", "http://www.w3.org/1999/xlink");
            writer.WriteAttributeString("xmlns:ev", "http://www.w3.org/2001/xml-events");

            double hiToTwips = 1;
            double pageHeight = hiToTwips * report.Unit.ConvertToHInches(page.PageHeight * page.SegmentPerHeight);
            double pageWidth = hiToTwips * report.Unit.ConvertToHInches(page.PageWidth * page.SegmentPerWidth);
            double mgLeft = hiToTwips * report.Unit.ConvertToHInches(page.Margins.Left);
            double mgRight = hiToTwips * report.Unit.ConvertToHInches(page.Margins.Right);
            double mgTop = hiToTwips * report.Unit.ConvertToHInches(page.Margins.Top);
            double mgBottom = hiToTwips * report.Unit.ConvertToHInches(page.Margins.Bottom);

            writer.WriteAttributeString("height", ToUnits(pageHeight));
            writer.WriteAttributeString("width", ToUnits(pageWidth));

            #region Write defs
            bool needStimulsoftFont = false;
            writer.WriteStartElement("defs");
            foreach (StiComponent component in page.Components)
            {
                bool needAdd = component.Enabled;
                if (component is StiPointPrimitive) needAdd = false;
                if (component.Width == 0 || component.Height == 0) needAdd = false;
                if (needAdd)
                {
                    bool needClip = false;

                    bool isImage = component.IsExportAsImage(StiExportFormat.ImageSvg);

                    var stiText = component as StiText;
                    if (stiText != null && !isImage && stiText.Text.ToString() != null && stiText.Text.ToString().Trim().Length > 0)
                    {
                        needClip = true;
                    }

                    var advWatermark = component.TagValue as StiAdvancedWatermark;
                    if (advWatermark != null)
                    {
                        needClip = true;
                        if (advWatermark.WeaveEnabled) needStimulsoftFont = true;
                    }

                    var stiImage = component as StiImage;
                    if (stiImage != null)
                    {
                        if (stiImage.Icon != null) needStimulsoftFont = true;
                        if (stiImage.ExistImageToDraw()) needClip = true;
                    }

                    if (needClip)
                    {
                        #region Prepare data
                        double x1 = hiToTwips * report.Unit.ConvertToHInches(component.Left);
                        double y1 = hiToTwips * report.Unit.ConvertToHInches(component.Top);
                        double x2 = hiToTwips * report.Unit.ConvertToHInches(component.Right);
                        double y2 = hiToTwips * report.Unit.ConvertToHInches(component.Bottom);

                        StiSvgData pp = new StiSvgData();
                        pp.X = x1 + mgLeft;
                        pp.Y = y1 + mgTop;
                        pp.Width = x2 - x1;
                        pp.Height = y2 - y1;
                        pp.Component = component;
                        #endregion

                        #region Write clip data
                        writer.WriteStartElement("clipPath");
                        writer.WriteAttributeString("id", GetClipPathName(component, guids));
                        writer.WriteStartElement("rect");
                        WriteCoordinates(writer, pp);
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        #endregion
                    }
                }
            }
            if (needStimulsoftFont)
            {
                using (var fontStream = typeof(StiFontIconsHelper).Assembly.GetManifestResourceStream("Stimulsoft.Base.FontIcons.Stimulsoft.ttf"))
                {
                    byte[] buffer = new byte[fontStream.Length];
                    fontStream.Read(buffer, 0, buffer.Length);

                    var sb = new StringBuilder();
                    sb.Append(@"<style>@font-face {font-family: 'Stimulsoft';src: url(data:font/ttf;base64,");
                    sb.Append(Convert.ToBase64String(buffer));
                    sb.Append(") format('truetype');font-weight: normal;font-style: normal;}</style>");

                    writer.WriteRaw(sb.ToString());
                }
            }
            writer.WriteEndElement();
            #endregion

            int gradientCounter = 1;

            #region Draw page background
            StiContainer pageCont = new StiContainer();
            pageCont.Border = (StiBorder)page.Border.Clone();
            pageCont.Brush = (StiBrush)page.Brush.Clone();

            if (pageCont.Brush is StiEmptyBrush)
                pageCont.Brush = new StiSolidBrush(Color.Transparent); //for support transparency dbs elements
            else if (StiBrush.ToColor(pageCont.Brush).A == 0)
                pageCont.Brush = new StiSolidBrush(Color.White);

            StiSvgData ppp = new StiSvgData();
            ppp.X = 0;
            ppp.Y = 0;
            ppp.Width = pageWidth;
            ppp.Height = pageHeight;
            ppp.Component = pageCont;
            WriteBorder1(writer, ppp, ref gradientCounter);
            #endregion

            WriteWatermark(writer, xmlIndentation, page, true, pageWidth, pageHeight, imageResolution);

            string pageBookmark = page.BookmarkValue as string;
            if (!string.IsNullOrEmpty(pageBookmark))
            {
                writer.WriteStartElement("view");
                writer.WriteAttributeString("id", pageBookmark);
                writer.WriteAttributeString("viewBox", "0 0 1 1");
                writer.WriteEndElement();
            }

            #region Process ExceedMargins
            foreach (StiComponent component in page.Components)
            {
                if (component.Enabled && (component.Width > 0) && (component.Height > 0))
                {
                    var stiText = component as StiText;
                    if ((stiText != null) && (stiText.ExceedMargins != StiExceedMargins.None) && (stiText.Page != null))
                    {
                        double x1 = hiToTwips * report.Unit.ConvertToHInches(component.Left);
                        double y1 = hiToTwips * report.Unit.ConvertToHInches(component.Top);
                        double x2 = hiToTwips * report.Unit.ConvertToHInches(component.Right);
                        double y2 = hiToTwips * report.Unit.ConvertToHInches(component.Bottom);

                        StiSvgData pp = new StiSvgData();
                        pp.X = x1 + mgLeft;
                        pp.Y = y1 + mgTop;
                        pp.Width = x2 - x1;
                        pp.Height = y2 - y1;
                        pp.Component = component;

                        WriteBorder1(writer, pp, ref gradientCounter);
                    }
                }
            }
            #endregion

            foreach (StiComponent component in page.Components)
            {
                bool needAdd = component.Enabled;
                if (component is StiPointPrimitive) needAdd = false;
                if (component.Width == 0 || component.Height == 0) needAdd = false;
                if (needAdd)
                {
                    #region Prepare data
                    double x1 = hiToTwips * report.Unit.ConvertToHInches(component.Left);
                    double y1 = hiToTwips * report.Unit.ConvertToHInches(component.Top);
                    double x2 = hiToTwips * report.Unit.ConvertToHInches(component.Right);
                    double y2 = hiToTwips * report.Unit.ConvertToHInches(component.Bottom);

                    StiSvgData pp = new StiSvgData();

                    pp.X = x1 + mgLeft;
                    pp.Y = y1 + mgTop;
                    pp.Width = x2 - x1;
                    pp.Height = y2 - y1;
                    pp.Component = component;
                    #endregion

                    bool isShape = CheckShape(component);
                    bool isImage = component.IsExportAsImage(StiExportFormat.ImageSvg);

                    var stiText = component as StiText;
                    bool isExceedMargins = (stiText != null) && (stiText.ExceedMargins != StiExceedMargins.None) && (stiText.Page != null);

                    if (!isShape && !isImage && !isExceedMargins)
                    {
                        WriteBorder1(writer, pp, ref gradientCounter);
                    }

                    var advWatermark = component.TagValue as StiAdvancedWatermark;
                    if (advWatermark != null)
                    {
                        RenderAdvancedWatermark(writer, xmlIndentation, advWatermark, pp, page, guids);
                    }

                    string hyperlink = component.HyperlinkValue as string;
                    if (!string.IsNullOrEmpty(hyperlink))
                    {
                        writer.WriteStartElement("a");
                        writer.WriteAttributeString("xlink:href", hyperlink);
                    }

                    if ((component is StiText) && !isImage)
                    {
                        WriteText(writer, pp, xmlIndentation, true, guids);
                    }
                    else
                    {
                        if (isShape)
                            WriteShape(writer, pp, xmlIndentation, true, guids);

                        else if (component is StiBarCode)
                            WriteBarCode(writer, pp);

                        else if (component is StiSparkline)
                            StiSparklineSvgHelper.WriteSparkline(writer, pp);

                        else if (component is StiCheckBox)
                            WriteCheckBox(writer, pp);

                        else if (component is StiChart)
                            StiChartSvgHelper.WriteChart(writer, pp, false);

                        else if (component is StiMap && ((StiMap)component).MapMode != StiMapMode.Online)
                            StiMapSvgHelper.DrawMap(writer, (StiMap)component, pp.X, pp.Y, pp.Width, pp.Height, false);

                        else
                        {
                            if (isImage)
                                WriteImage(writer, pp, imageResolution, imageFormat, imageQuality, imageCodec, guids);

                            else
                            {
                                string bookmark = pp.Component.BookmarkValue as string;
                                if (!string.IsNullOrEmpty(bookmark))
                                {
                                    writer.WriteStartElement("view");
                                    writer.WriteAttributeString("id", bookmark);
                                    writer.WriteAttributeString("viewBox", string.Format("{0} {1} {2} {3}", ToUnits(pp.X), ToUnits(pp.Y), ToUnits(pp.Width), ToUnits(pp.Height)));
                                    writer.WriteEndElement();
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(hyperlink))
                    {
                        writer.WriteEndElement();
                    }

                    if (!standalone)
                    {
                        pp.X = Math.Round(pp.X - 0.5) + 0.5;
                        pp.Y = Math.Round(pp.Y - 0.5) + 0.5;
                        pp.Width = Math.Round(x2 + mgLeft - 0.5) + 0.5 - pp.X;
                        pp.Height = Math.Round(y2 + mgTop - 0.5) + 0.5 - pp.Y;
                    }
                    if (component is StiRectanglePrimitive)
                    {
                        WriteRoundedRectanglePrimitive(writer, pp);
                    }
                    else
                    {
                        WriteBorder2(writer, pp);
                    }
                }
            }

            #region Draw page border
            ppp.X = mgLeft;
            ppp.Y = mgTop;
            ppp.Width -= mgLeft + mgRight;
            ppp.Height -= mgTop + mgBottom;
            WriteBorder2(writer, ppp);
            #endregion

            WriteWatermark(writer, xmlIndentation, page, false, pageWidth, pageHeight, imageResolution);

            writer.WriteFullEndElement();   //svg
            if (standalone) writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }

        #region WriteWatermark
        private static void WriteWatermark(XmlTextWriter writer, int xmlIndentation, StiPage page, bool behind, double pageWidth, double pageHeight, float imageResolution)
        {
            StiWatermark watermark = page.Watermark;
            if ((watermark != null) && (watermark.Enabled))
            {
                #region watermark image
                if (watermark.ExistImage() && watermark.ShowImageBehind == behind)
                {
                    WriteWatermarkImage(
                        writer,
                        new RectangleD(0, 0, pageWidth, pageHeight),
                        watermark.ImageBytes,
                        watermark.TryTakeGdiImage(),
                        watermark.ImageStretch,
                        watermark.ImageTiling,
                        watermark.AspectRatio,
                        watermark.ImageMultipleFactor,
                        watermark.ImageAlignment);
                }
                #endregion

                #region watermark text
                if ((!string.IsNullOrEmpty(watermark.Text)) && (watermark.ShowBehind == behind))
                {
                    StiSvgData pp = new StiSvgData();
                    pp.X = 0;
                    pp.Y = 0;
                    pp.Width = pageWidth;
                    pp.Height = pageHeight;
                    StiText stt = new StiText(new RectangleD(pp.X, pp.Y, pp.Width, pp.Height));
                    stt.Text = watermark.Text;
                    stt.TextBrush = watermark.TextBrush;
                    stt.Font = watermark.Font;
                    stt.TextOptions = new StiTextOptions();
                    stt.TextOptions.Angle = watermark.Angle;
                    stt.HorAlignment = StiTextHorAlignment.Center;
                    stt.VertAlignment = StiVertAlignment.Center;
                    stt.Page = page;
                    stt.TextQuality = StiTextQuality.Standard;
                    pp.Component = stt;
                    WriteText(writer, pp, xmlIndentation, false, null);
                }
                #endregion

                var advWatermark = page.TagValue as StiAdvancedWatermark;
                if (advWatermark != null && advWatermark.WeaveEnabled && behind)
                {
                    WriteWatermarkWeave(writer, advWatermark, new RectangleD(0, 0, pageWidth, pageHeight));
                }
            }
        }

        private static void WriteWatermarkImage(XmlTextWriter writer, RectangleD mainRect, byte[] imageBytes, Image image, bool imageStretch, bool isImageTiling, bool imageAspectRatio, double imageMultipleFactor, ContentAlignment imageAlignment)
        {
            double imageWidth = image.Width * imageMultipleFactor;
            double imageHeight = image.Height * imageMultipleFactor;
            double imageX = mainRect.Left;
            double imageY = mainRect.Top;
            int dupX = 1;
            int dupY = 1;

            if (imageStretch)
            {
                double aspectRatio = imageHeight / imageWidth;
                imageWidth = mainRect.Width;
                imageHeight = mainRect.Height;
                isImageTiling = false;
                if (imageAspectRatio)
                {
                    if (mainRect.Height / mainRect.Width > aspectRatio)
                    {
                        imageHeight = imageWidth * aspectRatio;
                    }
                    else
                    {
                        imageWidth = imageHeight / aspectRatio;
                    }
                }
            }
            if (isImageTiling)
            {
                imageAlignment = ContentAlignment.TopLeft;
                dupX = (int)(mainRect.Width / imageWidth) + 1;
                dupY = (int)(mainRect.Height / imageHeight) + 1;
            }

            switch (imageAlignment)
            {
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    imageX += (mainRect.Width - imageWidth) / 2f;
                    break;

                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    imageX += mainRect.Width - imageWidth;
                    break;
            }
            switch (imageAlignment)
            {
                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomRight:
                    imageY += mainRect.Height - imageHeight;
                    break;

                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleRight:
                    imageY += (mainRect.Height - imageHeight) / 2f;
                    break;
            }

            StiSvgData svgData = new StiSvgData();
            svgData.Width = imageWidth;
            svgData.Height = imageHeight;

            for (int indexY = 0; indexY < dupY; indexY++)
            {
                for (int indexX = 0; indexX < dupX; indexX++)
                {
                    svgData.X = imageX + imageWidth * indexX;
                    svgData.Y = imageY + imageHeight * indexY;
                    WriteImageBytes(writer, svgData, imageBytes);
                }
            }
        }

        internal static void WriteImageBytes(XmlTextWriter writer, StiSvgData svgData, byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0) return;

            string mimeType = "png";
            if (Helpers.StiImageHelper.IsMetafile(imageBytes))
            {
                imageBytes = StiMetafileConverter.MetafileToPngBytes(imageBytes, (int)svgData.Width, (int)svgData.Height);
            }
            else if (Helpers.StiImageHelper.IsBmp(imageBytes))
            {
                mimeType = "bmp";
            }
            else if (Helpers.StiImageHelper.IsJpeg(imageBytes))
            {
                mimeType = "jpeg";
            }
            else if (Helpers.StiImageHelper.IsGif(imageBytes))
            {
                mimeType = "gif";
            }
            else if (Base.Helpers.StiSvgHelper.IsSvg(imageBytes))
            {
                mimeType = "svg+xml";
            }

            writer.WriteStartElement("image");
            WriteCoordinates(writer, svgData);
            writer.WriteStartAttribute("href");
            writer.WriteString(string.Format("data:image/{0};base64,", mimeType));
            writer.WriteRaw("\r\n");
            writer.WriteBase64(imageBytes, 0, imageBytes.Length);
            writer.WriteEndAttribute();
            writer.WriteEndElement();
        }

        private static void WriteWatermarkWeave(XmlTextWriter writer, StiAdvancedWatermark advWatermark, RectangleD mainRect)
        {
            if (advWatermark == null) return;

            #region Prepare components
            var color1 = advWatermark.WeaveMajorColor;
            var color2 = advWatermark.WeaveMinorColor;

            FontFamily family = StiFontIconsHelper.GetFontFamilyIcons();
            if (family == null) family = StiFontCollection.GetFontFamily("Stimulsoft");
            var font1 = new Font(family, (float)(advWatermark.WeaveMajorSize * 5 * 0.57));
            var font2 = new Font(family, (float)(advWatermark.WeaveMinorSize * 5 * 0.57));

            string text1 = StiFontIconsHelper.GetContent(advWatermark.WeaveMajorIcon);
            string text2 = StiFontIconsHelper.GetContent(advWatermark.WeaveMinorIcon);

            double step = advWatermark.WeaveDistance;

            int range = (int)(Math.Max(mainRect.Width, mainRect.Height) / 2 * 1.4 / step) + 1;
            #endregion

            #region Render weave
            writer.WriteStartElement("g");
            writer.WriteAttributeString("transform", string.Format("translate({0},{1})",
                ToUnits(mainRect.X + mainRect.Width / 2),
                ToUnits(mainRect.Y + mainRect.Height / 2)));
            if (advWatermark.WeaveAngle != 0)
            {
                writer.WriteStartElement("g");
                writer.WriteAttributeString("transform", string.Format("rotate({0})", ToUnits(advWatermark.WeaveAngle)));
            }

            for (int dy = -range; dy <= range; dy++)
            {
                for (int dx = -range; dx <= range; dx++)
                {
                    int x = (int)(dx * step);
                    int y = (int)(dy * step);
                    bool isOdd = (Math.Abs(dx + dy) & 1) == 0;  //almost odd :)

                    var color = isOdd ? color1 : color2;
                    var font = isOdd ? font1 : font2;
                    var text = isOdd ? text1 : text2;

                    writer.WriteStartElement("text");
                    writer.WriteAttributeString("x", ToUnits(x));
                    writer.WriteAttributeString("y", ToUnits(y));

                    var baseStyle = new StringBuilder();
                    baseStyle.Append(GetStyleString(font, color));
                    baseStyle.Append("text-anchor:middle;alignment-baseline:middle;");
                    writer.WriteAttributeString("style", baseStyle.ToString());

                    writer.WriteString(text);
                    writer.WriteEndElement();
                }
            }

            if (advWatermark.WeaveAngle != 0)
            {
                writer.WriteFullEndElement();
            }
            writer.WriteFullEndElement();
            #endregion
        }

        private static void RenderAdvancedWatermark(XmlTextWriter writer, int xmlIndentation, StiAdvancedWatermark advWatermark, StiSvgData pp, StiPage page, Hashtable guids)
        {
            if (advWatermark == null) return;
            writer.WriteStartElement("g");
            writer.WriteAttributeString("clip-path", string.Format("url(#{0})", GetClipPathName(pp.Component, guids)));
            //clip-path: polygon(50% 0%, 100% 25%, 100% 75%, 50% 100%, 0% 75%, 0% 25%);
            //clip-path: inset(0 0 0 0);

            var advRect = new RectangleD(pp.X, pp.Y, pp.Width, pp.Height);
            if (advWatermark.ImageEnabled && advWatermark.ImageBytes != null)
            {
                WriteWatermarkImage(
                    writer,
                    advRect,
                    advWatermark.ImageBytes,
                    advWatermark.Image,
                    advWatermark.ImageStretch,
                    advWatermark.ImageTiling,
                    advWatermark.ImageAspectRatio,
                    advWatermark.ImageMultipleFactor,
                    advWatermark.ImageAlignment);
            }
            if (!string.IsNullOrEmpty(advWatermark.Text))
            {
                StiText stt = new StiText(advRect);
                stt.Text = advWatermark.Text;
                stt.TextBrush = new StiSolidBrush(advWatermark.TextColor);
                stt.Font = advWatermark.TextFont;
                stt.Angle = advWatermark.TextAngle;
                stt.HorAlignment = StiTextHorAlignment.Center;
                stt.VertAlignment = StiVertAlignment.Center;
                stt.Page = page;
                stt.TextQuality = StiTextQuality.Standard;

                var pp3 = new StiSvgData();
                pp3.X = pp.X;
                pp3.Y = pp.Y;
                pp3.Width = pp.Width;
                pp3.Height = pp.Height;
                pp3.Component = stt;
                WriteText(writer, pp3, xmlIndentation, false, null);
            }
            if (advWatermark.WeaveEnabled)
            {
                WriteWatermarkWeave(writer, advWatermark, advRect);
            }
            writer.WriteFullEndElement();
        }
        #endregion

        private static void WriteBorder1(XmlTextWriter writer, StiSvgData svgData, ref int gradientCounter)
        {
            IStiBrush mBrush = svgData.Component as IStiBrush;
            if ((mBrush != null) && (mBrush.Brush != null))
            {
                #region Process ExceedMargins
                var stiText = svgData.Component as StiText;
                if ((stiText != null) && (stiText.ExceedMargins != StiExceedMargins.None) && (stiText.Page != null))
                {
                    var pp2 = svgData.Clone();
                    double pageHeight = stiText.Report.Unit.ConvertToHInches(stiText.Page.PageHeight * stiText.Page.SegmentPerHeight);
                    double pageWidth = stiText.Report.Unit.ConvertToHInches(stiText.Page.PageWidth * stiText.Page.SegmentPerWidth);

                    if ((stiText.ExceedMargins & StiExceedMargins.Left) > 0)
                    {
                        pp2.X = 0;
                        pp2.Width = svgData.Right;
                    }
                    if ((stiText.ExceedMargins & StiExceedMargins.Right) > 0)
                    {
                        pp2.Width = pageWidth - pp2.X;
                    }
                    if ((stiText.ExceedMargins & StiExceedMargins.Top) > 0)
                    {
                        pp2.Y = 0;
                        pp2.Height = svgData.Bottom;
                    }
                    if ((stiText.ExceedMargins & StiExceedMargins.Bottom) > 0)
                    {
                        pp2.Height = pageHeight - pp2.Y;
                    }

                    svgData = pp2;
                }
                #endregion

                #region fill
                if (mBrush.Brush is StiGradientBrush)
                {
                    var gradientId = StiBrushSvgHelper.WriteGradientBrush(writer, mBrush.Brush,
                        new RectangleF((float)svgData.X, (float)svgData.Y, (float)svgData.Width, (float)svgData.Height));

                    writer.WriteStartElement("rect");
                    WriteCoordinates(writer, svgData);
                    writer.WriteAttributeString("fill", string.Format("url(#{0})", gradientId));
                    writer.WriteEndElement();
                }
                else if (mBrush.Brush is StiGlareBrush)
                {
                    var gradientId = StiBrushSvgHelper.WriteGlareBrush(writer, mBrush.Brush,
                        new RectangleF((float)svgData.X, (float)svgData.Y, (float)svgData.Width, (float)svgData.Height));

                    writer.WriteStartElement("rect");
                    WriteCoordinates(writer, svgData);
                    writer.WriteAttributeString("fill", string.Format("url(#{0})", gradientId));
                    writer.WriteEndElement();
                }
                else if (mBrush.Brush is StiGlassBrush)
                {
                    var gradientId = StiBrushSvgHelper.WriteGlassBrush(writer, mBrush.Brush,
                        new RectangleF((float)svgData.X, (float)svgData.Y, (float)svgData.Width, (float)svgData.Height));

                    writer.WriteStartElement("rect");
                    WriteCoordinates(writer, svgData);
                    writer.WriteAttributeString("fill", string.Format("url(#{0})", gradientId));
                    writer.WriteEndElement();
                }
                else if (mBrush.Brush is StiHatchBrush)
                {
                    var gradientId = StiBrushSvgHelper.WriteHatchBrush(writer, mBrush.Brush);

                    writer.WriteStartElement("rect");
                    WriteCoordinates(writer, svgData);
                    writer.WriteAttributeString("fill", string.Format("url(#{0})", gradientId));
                    writer.WriteEndElement();
                }
                else
                {
                    //if (mBrush.Brush is StiSolidBrush)
                    //{
                    #region Fill with solid color
                    Color color = StiBrush.ToColor(mBrush.Brush);
                    if (color != Color.Transparent)
                    {
                        writer.WriteStartElement("rect");
                        WriteCoordinates(writer, svgData);
                        WriteFillInfo(writer, color);
                        //writer.WriteAttributeString("stroke", "none");
                        writer.WriteEndElement();	//rect
                    }
                    #endregion
                    //}
                }
                #endregion
            }
        }

        private static void WriteBorder2(XmlTextWriter writer, StiSvgData svgData)
        {
            IStiBorder mBorder = svgData.Component as IStiBorder;
            if ((mBorder != null) && (mBorder.Border != null) && (mBorder.Border.Side != StiBorderSides.None) && (mBorder.Border.Style != StiPenStyle.None))
            {
                StiBorder border = mBorder.Border;
                if (border.Side == StiBorderSides.All)
                {
                    #region stroke all
                    writer.WriteStartElement("rect");
                    WriteCoordinates(writer, svgData);
                    writer.WriteAttributeString("fill", "none");
                    WriteStrokeInfo(writer, border.Color, border.Size, border.Style);
                    writer.WriteEndElement();	//rect
                    #endregion
                }
                else
                {
                    #region stroke by sides
                    writer.WriteStartElement("path");

                    #region Make path
                    string x1 = ToUnits(svgData.X);
                    string x2 = ToUnits(svgData.X + svgData.Width);
                    string y1 = ToUnits(svgData.Y + svgData.Height);
                    string y2 = ToUnits(svgData.Y);

                    StringBuilder stPath = new StringBuilder();
                    bool lastSidePresent = false;
                    if ((border.Side & StiBorderSides.Left) != 0)
                    {
                        stPath.Append(string.Format("M {0} {1} ", x1, y1));
                        stPath.Append(string.Format("L {0} {1} ", x1, y2));
                        lastSidePresent = true;
                    }
                    else lastSidePresent = false;
                    if ((border.Side & StiBorderSides.Top) != 0)
                    {
                        if (!lastSidePresent)
                        {
                            stPath.Append(string.Format("M {0} {1} ", x1, y2));
                        }
                        stPath.Append(string.Format("L {0} {1} ", x2, y2));
                        lastSidePresent = true;
                    }
                    else lastSidePresent = false;
                    if ((border.Side & StiBorderSides.Right) != 0)
                    {
                        if (!lastSidePresent)
                        {
                            stPath.Append(string.Format("M {0} {1} ", x2, y2));
                        }
                        stPath.Append(string.Format("L {0} {1} ", x2, y1));
                        lastSidePresent = true;
                    }
                    else lastSidePresent = false;
                    if ((border.Side & StiBorderSides.Bottom) != 0)
                    {
                        if (!lastSidePresent)
                        {
                            stPath.Append(string.Format("M {0} {1} ", x2, y1));
                        }
                        stPath.Append(string.Format("L {0} {1} ", x1, y1));
                    }
                    #endregion

                    writer.WriteAttributeString("d", stPath.ToString());
                    writer.WriteAttributeString("fill", "none");
                    WriteStrokeInfo(writer, border.Color, border.Size, border.Style);
                    writer.WriteEndElement();	//rect
                    #endregion
                }
            }
        }

        private static void WriteText(XmlTextWriter writer, StiSvgData svgData, int xmlIndentation, bool useClip, Hashtable guids, Color? textBackColor = null)
        {
            #region Write text
            var stiText = svgData.Component as StiText;
            bool needWidthAlign = stiText.HorAlignment == StiTextHorAlignment.Width;
            var textOptions = stiText.TextOptions ?? new StiTextOptions();
            if (needWidthAlign)
            {
                textOptions = textOptions.Clone() as StiTextOptions;
                textOptions.WordWrap = true;
            }
            float angle = textOptions.Angle;
            while (angle >= 360) angle -= 360;
            while (angle < 0) angle += 360;

            bool useAliases = stiText.Report != null && stiText.Report.Info.ForceDesigningMode && stiText.Report.Designer != null && stiText.Report.Designer.UseAliases;

            string textString = useAliases
                ? StiExpressionPacker.PackExpression(stiText.GetTextInternal(), stiText.Report, true)
                : stiText.Text.ToString();

            if (string.IsNullOrWhiteSpace(textString)) return;

            RectangleD rectComp = stiText.GetPaintRectangle(true, false);
            RectangleD rect = stiText.ConvertTextMargins(rectComp, false);
            rect = stiText.ConvertTextBorders(rect, false);
            RectangleD rectRotated = new RectangleD(rect.X, rect.Y, rect.Width, rect.Height);
            if (((angle > 45) && (angle < 135)) || ((angle > 225) && (angle < 315)))
            {
                double tempValue = rectRotated.Width;
                rectRotated.Width = rectRotated.Height;
                rectRotated.Height = tempValue;
            }

            var img = new Bitmap(1, 1);
            var g = Graphics.FromImage(img);

            if (!StiDpiHelper.IsWindows && stiText.CheckAllowHtmlTags() && !StiOptions.Engine.UseNewHtmlEngine)
            {
                textString = StiTextRenderer.GetPlainTextFromHtmlTags(textString);
            }

            if ((StiDpiHelper.IsWindows || StiOptions.Engine.UseNewHtmlEngine) && stiText.CheckAllowHtmlTags())
            {
                #region Html-tags

                #region Rotate text - part1
                double textX = svgData.X;
                double textY = svgData.Y;
                double textW = svgData.Width;
                double textH = svgData.Height;

                if (angle != 0)
                {
                    if (((angle > 45) && (angle < 135)) || ((angle > 225) && (angle < 315)))
                    {
                        double tempValue = textW;
                        textW = textH;
                        textH = tempValue;
                    }
                    textX = -textW / 2f;
                    textY = -textH / 2f;

                    if (useClip)
                    {
                        writer.WriteStartElement("g");
                        writer.WriteAttributeString("clip-path", string.Format("url(#{0})", GetClipPathName(stiText, guids)));
                    }
                    writer.WriteStartElement("g");
                    writer.WriteAttributeString("transform", string.Format("translate({0},{1})",
                        ToUnits(svgData.X + svgData.Width / 2f),
                        ToUnits(svgData.Y + svgData.Height / 2f)));
                    writer.WriteStartElement("g");
                    writer.WriteAttributeString("transform", string.Format("rotate({0})", ToUnits(-angle)));
                }
                #endregion

                #region Prepare RunInfo list
                var outRunsList = new List<StiTextRenderer.RunInfo>();
                var outFontsList = new List<StiTextRenderer.StiFontState>();

                bool wordWrap = textOptions.WordWrap || needWidthAlign;

                string baseFontName = stiText.Font.Name;
                var baseFontColor = StiBrush.ToColor(stiText.TextBrush);
                var baseBackColor = StiBrush.ToColor(stiText.Brush);

                if (StiOptions.Engine.UseNewHtmlEngine)
                {
                    StiHtmlTextRender.DrawTextForOutput(stiText, out outRunsList, out outFontsList);
                }
                else
                {
                    StiTextRenderer.DrawTextForOutput(
                    g,
                    textString,
                    stiText.Font,
                    rectRotated,
                    baseFontColor,
                    baseBackColor,
                    stiText.LineSpacing * StiOptions.Engine.TextLineSpacingScale,
                    stiText.HorAlignment,
                    stiText.VertAlignment,
                    wordWrap,
                    textOptions.RightToLeft,
                    1,
                    angle,
                    textOptions.Trimming,
                    textOptions.LineLimit,
                    stiText.CheckAllowHtmlTags(),
                    outRunsList,
                    outFontsList,
                    textOptions,
                    1);
                }
                #endregion

                //background
                foreach (StiTextRenderer.RunInfo runInfo in outRunsList)
                {
                    if (baseBackColor.ToArgb() != runInfo.BackColor.ToArgb())
                    {
                        StiTextRenderer.StiFontState fontState = outFontsList[runInfo.FontIndex];

                        double posX = textX + runInfo.XPos + 0.25;
                        double posY = textY + runInfo.YPos;

                        double widths = 0;
                        foreach (int width in runInfo.Widths) widths += width;

                        writer.WriteStartElement("rect");
                        writer.WriteAttributeString("x", ToUnits(posX));
                        writer.WriteAttributeString("y", ToUnits(posY));
                        writer.WriteAttributeString("width", ToUnits(widths));
                        writer.WriteAttributeString("height", ToUnits(fontState.FontBase.Height));
                        WriteFillInfo(writer, runInfo.BackColor);
                        writer.WriteEndElement();
                    }
                }

                writer.WriteStartElement("text");
                writer.WriteAttributeString("x", ToUnits(textX));
                writer.WriteAttributeString("y", ToUnits(textY));

                string bookmark = stiText.BookmarkValue as string;
                if (!string.IsNullOrEmpty(bookmark))
                {
                    writer.WriteAttributeString("id", bookmark);
                }

                var baseStyle = new StringBuilder();
                if (useClip && angle == 0)
                    baseStyle.Append(string.Format("clip-path: url(#{0});", GetClipPathName(stiText, guids)));
                baseStyle.Append(GetStyleString(stiText.Font, baseFontColor, false));
                baseStyle.Append("white-space:pre;");
                writer.WriteAttributeString("style", baseStyle.ToString());

                foreach (StiTextRenderer.RunInfo runInfo in outRunsList)
                {
                    StiTextRenderer.StiFontState fontState = outFontsList[runInfo.FontIndex];

                    double fontSize = fontState.FontBase.SizeInPoints;
                    if (StiDpiHelper.NeedGraphicsScale && StiDpiHelper.NeedFontScaling)
                        fontSize /= StiDpiHelper.GraphicsScale;

                    double posX = textX + (rect.Left + runInfo.XPos - rectComp.Left) + 1;
                    double posY = textY + runInfo.YPos + fontState.Ascend + 0.5; //stiText.Font.SizeInPoints * 0.9 * correctFontSize

                    #region RunStyle
                    StringBuilder style = new StringBuilder();
                    if (baseFontName != fontState.FontNameReal)
                    {
                        style.Append(string.Format("font-family:'{0}';", fontState.FontNameReal));
                    }
                    if (stiText.Font.SizeInPoints != fontSize)
                    {
                        style.Append(string.Format("font-size:{0}pt;", Math.Round(fontSize, 2).ToString().Replace(',', '.')));
                    }
                    if (stiText.Font.Bold != fontState.FontBase.Bold)
                    {
                        style.Append("font-weight:" + (fontState.FontBase.Bold ? "bold" : "normal") + ";");
                    }
                    if (stiText.Font.Italic != fontState.FontBase.Italic)
                    {
                        style.Append("font-style:" + (fontState.FontBase.Italic ? "italic" : "normal") + ";");
                    }
                    //if (stiText.Font.Underline != fontState.FontBase.Underline || stiText.Font.Strikeout != fontState.FontBase.Strikeout)     //for fix TextDecoration color
                    if (stiText.Font.Underline || stiText.Font.Strikeout)
                    {
                        string decoration = fontState.FontBase.Underline ? (fontState.FontBase.Strikeout ? "underline line-through" : "underline") : (fontState.FontBase.Strikeout ? "line-through" : "none");
                        style.Append(string.Format("text-decoration:{0};", decoration));
                    }
                    if (baseFontColor.ToArgb() != runInfo.TextColor.ToArgb())
                    {
                        style.Append(string.Format("fill:#{0:X2}{1:X2}{2:X2};", runInfo.TextColor.R, runInfo.TextColor.G, runInfo.TextColor.B));
                        if (runInfo.TextColor.A != 0xFF)
                        {
                            style.Append(string.Format("fill-opacity:{0};", Math.Round(runInfo.TextColor.A / 255f, 3).ToString().Replace(",", ".")));
                        }
                    }

                    double widths = 0;
                    foreach (int width in runInfo.Widths) widths += width;

                    if (runInfo.TextAlign != StiTextHorAlignment.Left)
                    {
                        if (runInfo.TextAlign == StiTextHorAlignment.Center)
                        {
                            posX += widths / 2;
                            style.Append("text-anchor:middle;");
                        }
                        if (runInfo.TextAlign == StiTextHorAlignment.Right)
                        {
                            posX += widths;
                            style.Append("text-anchor:end;");
                        }
                    }
                    #endregion

                    writer.WriteStartElement("tspan");
                    writer.WriteAttributeString("x", ToUnits(posX));
                    writer.WriteAttributeString("y", ToUnits(posY));
                    if (style.Length > 0)
                    {
                        writer.WriteAttributeString("style", style.ToString());
                    }

                    writer.WriteAttributeString("textLength", ToUnits(widths));

                    if (!string.IsNullOrWhiteSpace(runInfo.Href))
                    {
                        writer.WriteStartElement("a");
                        writer.WriteAttributeString("href", runInfo.Href);
                        writer.WriteAttributeString("target", "_blank");
                    }

                    string stt = runInfo.Text;
                    if ((stt != null) && (stt.Trim().Length == 0)) stt = "\xA0";
                    writer.WriteString(stt);

                    if (!string.IsNullOrWhiteSpace(runInfo.Href))
                    {
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();   //tspan
                }

                writer.WriteFullEndElement();   //text
                #endregion

                #region Rotate text - part2
                if (angle != 0)
                {
                    writer.WriteFullEndElement();
                    writer.WriteFullEndElement();
                    if (useClip) writer.WriteFullEndElement();
                }
                #endregion
            }
            else
            {
                #region WinForms HighAccuracy WordWrap
                var arrLinesInfo = StiTextDrawing.SplitTextWordwrap(
                    textString,
                    g,
                    StiFontUtils.ChangeFontSize(stiText.Font, stiText.Font.Size * (float)StiDpiHelper.GraphicsScale),
                    rectRotated,
                    textOptions,
                    stiText.HorAlignment,
                    stiText.TextQuality == StiTextQuality.Typographic);
                foreach (var lineInfo in arrLinesInfo)
                {
                    lineInfo.LineHeight *= stiText.LineSpacing;
                }

                bool needOutput = arrLinesInfo.Count > 0;
                if ((arrLinesInfo.Count == 1) && (arrLinesInfo[0].Text.Length == 0)) needOutput = false;
                if (needOutput)
                {
                    #region Make style attribute
                    double allTextHeight = 0;
                    foreach (var lineInfo in arrLinesInfo)
                    {
                        allTextHeight += lineInfo.LineHeight;
                    }
                    double borderSize = 0;
                    if (stiText.Border != null) borderSize = stiText.Border.Size / 2f;

                    StringBuilder style = new StringBuilder();
                    style.Append("white-space:pre;");

                    if (useClip && angle == 0)
                        style.Append(string.Format("clip-path: url(#{0});", GetClipPathName(stiText, guids)));

                    double svgDataX = svgData.X + stiText.Margins.Left;
                    double svgDataY = svgData.Y + stiText.Margins.Top;
                    double svgDataWidth = svgData.Width - stiText.Margins.Left - stiText.Margins.Right;
                    double svgDataHeight = svgData.Height - stiText.Margins.Top - stiText.Margins.Bottom;

                    double startX = svgDataX;
                    double startY = svgDataY + stiText.Font.SizeInPoints * 0.9 * correctFontSize;

                    if (angle % 90 == 0)
                    {
                        if (!textOptions.RightToLeft && stiText.HorAlignment == StiTextHorAlignment.Left ||
                            textOptions.RightToLeft && stiText.HorAlignment == StiTextHorAlignment.Right)
                        {
                            //‘text-anchor’ initial: start
                            if (textOptions.RightToLeft)
                            {
                                style.Append(string.Format("text-anchor:{0};", "end"));
                            }
                            startX += 2.5 + borderSize;
                        }
                        if (stiText.HorAlignment == StiTextHorAlignment.Center)
                        {
                            style.Append(string.Format("text-anchor:{0};", "middle"));
                            if (angle == 90 || angle == 270) startX += svgDataHeight / 2f;
                            else startX += svgDataWidth / 2f;
                        }
                        if (textOptions.RightToLeft && stiText.HorAlignment == StiTextHorAlignment.Left ||
                            !textOptions.RightToLeft && stiText.HorAlignment == StiTextHorAlignment.Right)
                        {
                            style.Append(string.Format("text-anchor:{0};", textOptions.RightToLeft ? "start" : "end"));
                            if (angle == 90 || angle == 270) startX += svgDataHeight - 2 - borderSize;
                            else startX += svgDataWidth - 2 - borderSize;
                        }
                        if (stiText.HorAlignment == StiTextHorAlignment.Width)
                        {
                            startX += 2 + borderSize;
                        }

                        if (stiText.VertAlignment == StiVertAlignment.Top) startY += borderSize;
                        if (stiText.VertAlignment == StiVertAlignment.Center)
                        {
                            if (angle == 90 || angle == 270) startY += (svgDataWidth - allTextHeight) / 2f;
                            else startY += (svgDataHeight - allTextHeight) / 2f;
                        }
                        if (stiText.VertAlignment == StiVertAlignment.Bottom)
                        {
                            if (angle == 90 || angle == 270) startY += svgDataWidth - allTextHeight - borderSize;
                            else startY += svgDataHeight - allTextHeight - borderSize;
                        }
                    }
                    else
                    {
                        // Center alignment
                        style.Append(string.Format("text-anchor:{0};", "middle"));
                        startX += svgDataWidth / 2f;
                        startY += (svgDataHeight - allTextHeight) / 2f;
                    }

                    style.Append(GetStyleString(
                        stiText.Font,
                        StiBrush.ToColor(stiText.TextBrush)));
                    #endregion

                    #region Rotate text - part1
                    if (angle != 0)
                    {
                        if (useClip)
                        {
                            writer.WriteStartElement("g");
                            writer.WriteAttributeString("clip-path", string.Format("url(#{0})", GetClipPathName(stiText, guids)));
                        }
                        writer.WriteStartElement("g");
                        writer.WriteAttributeString("transform", string.Format("translate({0},{1})",
                            ToUnits(svgDataX + svgDataWidth / 2f),
                            ToUnits(svgDataY + svgDataHeight / 2f)));
                        writer.WriteStartElement("g");
                        writer.WriteAttributeString("transform", string.Format("rotate({0})", ToUnits(-angle)));
                        if (angle == 90 || angle == 270)
                        {
                            startX -= svgDataX + svgDataHeight / 2f;
                            startY -= svgDataY + svgDataWidth / 2f;
                        }
                        else
                        {
                            startX -= svgDataX + svgDataWidth / 2f;
                            startY -= svgDataY + svgDataHeight / 2f;
                        }
                    }
                    #endregion

                    bool hasRtlSymbols = StiBidirectionalConvert.StringContainArabicOrHebrew(textString);

                    var paintBackColor = textBackColor != null && !textBackColor.Equals(Color.Transparent);
                    if (paintBackColor)
                    {
                        writer.WriteStartElement("filter");
                        writer.WriteAttributeString("x", "0");
                        writer.WriteAttributeString("y", "0");
                        writer.WriteAttributeString("width", "1");
                        writer.WriteAttributeString("height", "1");
                        writer.WriteAttributeString("id", "solid");

                        writer.WriteStartElement("feFlood");
                        writer.WriteAttributeString("flood-color", string.Format("rgb({0},{1},{2})", textBackColor?.R, textBackColor?.G, textBackColor?.B));
                        writer.WriteAttributeString("result", "bg");
                        writer.WriteEndElement(); //feFlood

                        writer.WriteStartElement("feMerge");
                        writer.WriteStartElement("feMergeNode");
                        writer.WriteAttributeString("in", "bg");
                        writer.WriteEndElement(); //feMergeNode

                        writer.WriteStartElement("feMergeNode");
                        writer.WriteAttributeString("in", "SourceGraphic");
                        writer.WriteEndElement(); //feMergeNode
                        writer.WriteEndElement(); //feMerge

                        writer.WriteEndElement(); //filter
                    }

                    writer.WriteStartElement("text");
                    writer.WriteAttributeString("x", ToUnits(startX));
                    writer.WriteAttributeString("y", ToUnits(startY));
                    writer.WriteAttributeString("style", style.ToString());
                    if (textOptions.RightToLeft) // && StiBidirectionalConvert.StringContainArabicOrHebrew(arrLinesInfo[0].Text))
                    {
                        writer.WriteAttributeString("direction", "rtl");
                        writer.WriteAttributeString("unicode-bidi", "embed");
                    }

                    if (paintBackColor)
                        writer.WriteAttributeString("filter", "url(#solid)");

                    string bookmark = stiText.BookmarkValue as string;
                    if (!string.IsNullOrEmpty(bookmark))
                    {
                        writer.WriteAttributeString("id", bookmark);
                    }

                    #region Write text lines
                    //#if TESTSVG
                    writer.Indentation = 0;
                    writer.Formatting = Formatting.None;
                    //#endif

                    for (int indexLine = 0; indexLine < arrLinesInfo.Count; indexLine++)
                    {
                        var lineInfo = arrLinesInfo[indexLine];
                        if (arrLinesInfo.Count > 1)
                        {
                            if (hasRtlSymbols)
                            {
                                if (indexLine > 0)
                                {
                                    writer.WriteEndElement();   //text
                                    writer.WriteStartElement("text");
                                    writer.WriteAttributeString("x", ToUnits(startX));
                                    writer.WriteAttributeString("y", ToUnits(startY + indexLine * lineInfo.LineHeight));
                                    writer.WriteAttributeString("style", style.ToString());
                                    if (textOptions.RightToLeft && StiBidirectionalConvert.StringContainArabicOrHebrew(arrLinesInfo[indexLine].Text))
                                    {
                                        writer.WriteAttributeString("direction", "rtl");
                                        writer.WriteAttributeString("unicode-bidi", "embed");
                                    }
                                }
                            }
                            else
                            {
                                writer.WriteStartElement("tspan");
                                if (indexLine > 0)
                                {
                                    writer.WriteAttributeString("x", ToUnits(startX));
                                    //writer.WriteAttributeString("dy", ToUnits(stiText.Font.SizeInPoints * correctFontSize));
                                    writer.WriteAttributeString("dy", ToUnits(lineInfo.LineHeight));
                                }
                            }
                        }
                        if (lineInfo.NeedWidthAlign)
                        {
                            float angle2 = angle;
                            while (angle2 < 0) angle2 += 360;
                            double maxTextWidth = svgDataWidth;
                            if ((angle2 > 45 && angle2 < 135) || (angle2 > 225 && angle2 < 315))
                            {
                                maxTextWidth = svgDataHeight;
                            }
                            double textLength = maxTextWidth - 4 - borderSize * 2;
                            writer.WriteAttributeString("textLength", ToUnits(textLength > 0 ? textLength : 1));
                        }

                        string stt = arrLinesInfo[indexLine].Text;
                        if ((stt != null) && (stt.Trim().Length == 0)) stt = "\xA0";
                        writer.WriteString(stt);

                        if (!hasRtlSymbols && arrLinesInfo.Count > 1)
                        {
                            writer.WriteEndElement();
                        }
                    }
                    writer.WriteFullEndElement();   //text

                    //#if TESTSVG
                    writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
                    writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
                    //#endif
                    #endregion

                    #region Rotate text - part2
                    if (angle != 0)
                    {
                        writer.WriteFullEndElement();
                        writer.WriteFullEndElement();
                        if (useClip) writer.WriteFullEndElement();
                    }
                    #endregion
                }
                #endregion
            }
            #endregion
        }

        public static string GetStyleString(Font font, Color textColor, bool useTextDecoration = true)
        {
            var style = new StringBuilder();

            style.Append(string.Format("font-size:{0}pt;", font.SizeInPoints.ToString().Replace(',', '.')));

            style.Append(string.Format("font-family:'{0}';", font.Name));   //!String.IsNullOrEmpty(font.OriginalFontName) ? font.OriginalFontName : font.Name));

            if (font.Bold)
            {
                style.Append("font-weight:bold;");
            }
            if (font.Italic)
            {
                style.Append("font-style:italic;");
            }
            if (useTextDecoration && (font.Underline || font.Strikeout))
            {
                string decoration = font.Underline ? (font.Strikeout ? "underline line-through" : "underline") : (font.Strikeout ? "line-through" : null);
                style.Append(string.Format("text-decoration:{0};", decoration));
            }
            style.Append(string.Format("fill:#{0:X2}{1:X2}{2:X2};", textColor.R, textColor.G, textColor.B));
            if (textColor.A != 0xFF)
            {
                style.Append(string.Format("fill-opacity:{0};", Math.Round(textColor.A / 255f, 3).ToString().Replace(",", ".")));
            }

            return style.ToString();
        }

        internal static void WriteImage(XmlTextWriter writer, StiSvgData svgData, float imageResolution, ImageFormat imageFormat, float imageQuality, ImageCodecInfo imageCodec, Hashtable guids)
        {
            var stiImage = svgData.Component as StiImage;
            if (stiImage != null) 
            {
                if (stiImage.Icon != null)
                {
                    RenderIconInternal(writer, false, stiImage, 1, new RectangleD(svgData.X, svgData.Y, svgData.Width, svgData.Height));
                    return;
                }
                var buf = stiImage.TakeImageToDraw();
                if ((buf != null) &&
                    (Stimulsoft.Report.Helpers.StiImageHelper.IsTiff(buf) ||
                    Stimulsoft.Report.Helpers.StiImageHelper.IsPng(buf) ||
                    Stimulsoft.Report.Helpers.StiImageHelper.IsGif(buf) ||
                    Stimulsoft.Report.Helpers.StiImageHelper.IsJpeg(buf) ||
                    Stimulsoft.Report.Helpers.StiImageHelper.IsBmp(buf) ||
                    Stimulsoft.Report.Helpers.StiImageHelper.IsSvg(buf)))
                {
                    RenderImageInternal(writer, svgData, guids, 1);
                    return;
                }
            }

            if (imageCodec == null) imageCodec = StiImageCodecInfo.GetImageCodec("image/jpeg");

            var exportImage = svgData.Component as IStiExportImageExtended;
            if (exportImage != null && svgData.Component.IsExportAsImage(StiExportFormat.ImagePng))
            {
                float rsImageResolution = imageResolution;
                using (Image image = exportImage.GetImage(ref rsImageResolution, StiExportFormat.ImagePng))
                {
                    WriteImage2(writer, svgData, image, imageResolution, imageFormat, imageQuality, imageCodec);
                }
            }
        }

        private static void RenderImageInternal(XmlTextWriter writer, StiSvgData svgData, Hashtable guids, float zoom)
        {
            #region Prepare data
            var image = svgData.Component as StiImage;

            var buf = image.TakeImageToDraw();
            string imageFormatMime = Stimulsoft.Report.Helpers.StiImageHelper.GetImageName(buf).ToLowerInvariant();
            if (imageFormatMime == "svg") imageFormatMime = "svg+xml";

            var gdiImage = image.TakeGdiImageToDraw();  //need check for SVG?
            double imageWidth = gdiImage.Width * image.MultipleFactor;
            double imageHeight = gdiImage.Height * image.MultipleFactor;

            double width = imageWidth;
            double height = imageHeight;
            double dw = imageWidth;
            double dh = imageHeight;

            if (image.ImageRotation == StiImageRotation.Rotate90CW || image.ImageRotation == StiImageRotation.Rotate90CCW)
            {
                var temp = width;
                width = height;
                height = temp;
            }

            if (image.Stretch)
            {
                if (image.AspectRatio)
                {
                    double sX = Math.Round(svgData.Width / width, 4);
                    double sY = Math.Round(svgData.Height / height, 4);
                    double sc = Math.Min(sX, sY);
                    width *= sc;
                    height *= sc;
                }
                else
                {
                    width = svgData.Width;
                    height = svgData.Height;
                }
                dw = width;
                dh = height;
                if (image.ImageRotation == StiImageRotation.Rotate90CW || image.ImageRotation == StiImageRotation.Rotate90CCW)
                {
                    dw = height;
                    dh = width;
                }
            }

            double x = svgData.X;
            double y = svgData.Y;
            if (image.HorAlignment == StiHorAlignment.Center)
                x += (svgData.Width - width) / 2;
            if (image.HorAlignment == StiHorAlignment.Right)
                x += svgData.Width - width;
            if (image.VertAlignment == StiVertAlignment.Center)
                y += (svgData.Height - height) / 2;
            if (image.VertAlignment == StiVertAlignment.Bottom)
                y += svgData.Height - height;

            double cx = x + width / 2;
            double cy = y + height / 2;
            double scaleX = 1;
            double scaleY = 1;

            string rotate = "";
            if (image.ImageRotation == StiImageRotation.Rotate90CW) rotate = " rotate(90)";
            if (image.ImageRotation == StiImageRotation.Rotate90CCW) rotate = " rotate(-90)";
            if (image.ImageRotation == StiImageRotation.Rotate180) rotate = " rotate(180)";
            if (image.ImageRotation == StiImageRotation.FlipHorizontal) scaleX = -scaleX;
            if (image.ImageRotation == StiImageRotation.FlipVertical) scaleY = -scaleY;
            #endregion

            #region Write data
            writer.WriteStartElement("g");
            writer.WriteAttributeString("clip-path", string.Format("url(#{0})", GetClipPathName(image, guids)));
            writer.WriteStartElement("g");
            writer.WriteAttributeString("transform", String.Format("translate({0}, {1}) scale({2}, {3}){4}", ToUnits(cx), ToUnits(cy), ToUnits(scaleX, 3), ToUnits(scaleY, 3), rotate));

            writer.WriteStartElement("image");

            string bookmark = svgData.Component.BookmarkValue as string;
            if (!string.IsNullOrEmpty(bookmark))
            {
                writer.WriteAttributeString("id", bookmark);
            }

            writer.WriteAttributeString("x", ToUnits(-dw/2));
            writer.WriteAttributeString("y", ToUnits(-dh/2));
            writer.WriteAttributeString("width", ToUnits(dw));
            writer.WriteAttributeString("height", ToUnits(dh));

            if (image.Stretch && !image.AspectRatio)
            {
                writer.WriteAttributeString("preserveAspectRatio", "none");
            }

            writer.WriteStartAttribute("href");
            writer.WriteString(string.Format("data:image/{0};base64,", imageFormatMime));
            writer.WriteRaw("\r\n");
            writer.WriteBase64(buf, 0, buf.Length);
            writer.WriteEndAttribute();
            writer.WriteEndElement();

            writer.WriteEndElement();   //g
            writer.WriteEndElement();   //g

            writer.Flush();
            #endregion
        }

        internal static void WriteImage2(XmlTextWriter writer, StiSvgData svgData, Image image, float imageResolution, ImageFormat imageFormat, float imageQuality, ImageCodecInfo imageCodec)
        {
            if (image == null) return;

            #region Write image
            MemoryStream ms = new MemoryStream();
            if (imageFormat == ImageFormat.Png || imageFormat == ImageFormat.Gif || imageFormat == ImageFormat.Bmp)
            {
                image.Save(ms, imageFormat);
            }
            else
            {
                if (imageCodec == null)
                {
                    image.Save(ms, ImageFormat.Jpeg);
                }
                else
                {
                    EncoderParameters imageEncoderParameters = new EncoderParameters(1);
                    imageEncoderParameters.Param[0] = new EncoderParameter(ImageEncoder.Quality, (long)(imageQuality * 100));
                    image.Save(ms, imageCodec, imageEncoderParameters);
                }
            }
            byte[] buf = ms.ToArray();

            writer.WriteStartElement("image");

            string bookmark = svgData.Component.BookmarkValue as string;
            if (!string.IsNullOrEmpty(bookmark))
            {
                writer.WriteAttributeString("id", bookmark);
            }
            if (svgData.Component is StiMap)
            {
                writer.WriteAttributeString("x", ToUnits(svgData.X));
                writer.WriteAttributeString("y", ToUnits(svgData.Y));
                writer.WriteAttributeString("width", ToUnits(image.Width));
                writer.WriteAttributeString("height", ToUnits(image.Height));
            }
            else
                WriteCoordinates(writer, svgData);
            writer.WriteStartAttribute("href");
            string imageFormatMime = "jpg";
            if (imageFormat == ImageFormat.Png) imageFormatMime = "png";
            if (imageFormat == ImageFormat.Gif) imageFormatMime = "gif";
            if (imageFormat == ImageFormat.Bmp) imageFormatMime = "bmp";
            writer.WriteString(string.Format("data:image/{0};base64,", imageFormatMime));
            writer.WriteRaw("\r\n");
            writer.WriteBase64(buf, 0, buf.Length);
            writer.WriteEndAttribute();
            writer.WriteEndElement();
            #endregion
        }

        private static void WriteBarCode(XmlTextWriter writer, StiSvgData svgData)
        {
            var barCode = svgData.Component as StiBarCode;

            var svgGeomWriter = new StiSvgGeomWriter(writer);
            var barCodePainter = new StiBarCodeExportPainter(svgGeomWriter);

            barCode.BarCodeType.Draw(barCodePainter, barCode, new RectangleF((float)svgData.X, (float)svgData.Y, (float)svgData.Width, (float)svgData.Height), 1);
        }

        private static void WriteShape(XmlTextWriter writer, StiSvgData svgData, int xmlIndentation, bool useClip, Hashtable guids)
        {
            var shape = svgData.Component as StiShape;
            var mBrush = svgData.Component as IStiBrush;
            var tempColor = Color.Transparent;
            if (mBrush != null) tempColor = StiBrush.ToColor(mBrush.Brush);
            StringBuilder fillPath = new StringBuilder();
            var strokePath = new StringBuilder();

            #region VerticalLine
            if (shape.ShapeType is StiVerticalLineShapeType)
            {
                fillPath.Append(string.Format("M {0},{1} V {3} H {2} V {1} Z",
                    ToUnits(svgData.X), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Bottom)));
                strokePath.Append(string.Format("M {0},{1} V {2}",
                    ToUnits(svgData.X + svgData.Width / 2), ToUnits(svgData.Y), ToUnits(svgData.Bottom)));
            }
            #endregion

            #region HorizontalLine
            if (shape.ShapeType is StiHorizontalLineShapeType)
            {
                fillPath.Append(string.Format("M {0},{1} V {3} H {2} V {1} Z",
                    ToUnits(svgData.X), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Bottom)));
                strokePath.Append(string.Format("M {0},{1} H {2}",
                    ToUnits(svgData.X), ToUnits(svgData.Y + svgData.Height / 2), ToUnits(svgData.Right)));
            }
            #endregion

            #region TopAndBottomLine
            if (shape.ShapeType is StiTopAndBottomLineShapeType)
            {
                fillPath.Append(string.Format("M {0},{1} V {3} H {2} V {1} Z",
                    ToUnits(svgData.X), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Bottom)));
                strokePath.Append(string.Format("M {0},{1} H {2} M {0} {3} H {2}",
                    ToUnits(svgData.X), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Bottom)));
            }
            #endregion

            #region LeftAndRightLine
            if (shape.ShapeType is StiLeftAndRightLineShapeType)
            {
                fillPath.Append(string.Format("M {0},{1} V {3} H {2} V {1} Z",
                    ToUnits(svgData.X), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Bottom)));
                strokePath.Append(string.Format("M {0},{1} V {3} M {2},{1} V {3}",
                    ToUnits(svgData.X), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Bottom)));
            }
            #endregion

            #region Rectangle
            if (shape.ShapeType is StiRectangleShapeType)
            {
                fillPath.Append(string.Format("M {0},{1} V {3} H {2} V {1} Z",
                    ToUnits(svgData.X), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Bottom)));
                strokePath = fillPath;
            }
            #endregion

            #region DiagonalDownLine
            if (shape.ShapeType is StiDiagonalDownLineShapeType)
            {
                fillPath.Append(string.Format("M {0},{1} V {3} H {2} V {1} Z",
                    ToUnits(svgData.X), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Bottom)));
                strokePath.Append(string.Format("M {0},{1} L {2} {3}",
                    ToUnits(svgData.X), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Bottom)));
            }
            #endregion

            #region DiagonalUpLine
            if (shape.ShapeType is StiDiagonalUpLineShapeType)
            {
                fillPath.Append(string.Format("M {0},{1} V {3} H {2} V {1} Z",
                    ToUnits(svgData.X), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Bottom)));
                strokePath.Append(string.Format("M {0},{3} L {2} {1}",
                    ToUnits(svgData.X), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Bottom)));
            }
            #endregion

            #region Triangle
            if (shape.ShapeType is StiTriangleShapeType)
            {
                StiShapeDirection ssd = (shape.ShapeType as StiTriangleShapeType).Direction;
                if (ssd == StiShapeDirection.Up)
                {
                    fillPath.Append(string.Format("M {0},{3} L {4},{1} {2},{3} Z",
                        ToUnits(svgData.X), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Bottom), ToUnits(svgData.X + svgData.Width / 2)));
                }
                if (ssd == StiShapeDirection.Down)
                {
                    fillPath.Append(string.Format("M {0},{1} L {4},{3} {2},{1} Z",
                        ToUnits(svgData.X), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Bottom), ToUnits(svgData.X + svgData.Width / 2)));
                }
                if (ssd == StiShapeDirection.Left)
                {
                    fillPath.Append(string.Format("M {2},{3} L {0},{4} {2},{1} Z",
                        ToUnits(svgData.X), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Bottom), ToUnits(svgData.Y + svgData.Height / 2)));
                }
                if (ssd == StiShapeDirection.Right)
                {
                    fillPath.Append(string.Format("M {0},{3} L {2},{4} {0},{1} Z",
                        ToUnits(svgData.X), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Bottom), ToUnits(svgData.Y + svgData.Height / 2)));
                }
                strokePath = fillPath;
            }
            #endregion

            #region Oval
            if (shape.ShapeType is StiOvalShapeType)
            {
                double tmpX = svgData.Width / 2 * (1 - pdfCKT);
                double tmpY = svgData.Height / 2 * (1 - pdfCKT);
                double x1 = svgData.X;
                double y1 = svgData.Y;
                double x2 = svgData.Right;
                double y2 = svgData.Bottom;
                double xc = svgData.X + svgData.Width / 2;
                double yc = svgData.Y + svgData.Height / 2;

                fillPath.Append(string.Format("M {0},{1} ",
                    ToUnits(xc), ToUnits(y2)));
                fillPath.Append(string.Format("C {0},{1} {2},{3} {4},{5} ",
                    ToUnits(x1 + tmpX), ToUnits(y2), ToUnits(x1), ToUnits(y2 - tmpY), ToUnits(x1), ToUnits(yc)));
                fillPath.Append(string.Format("C {0},{1} {2},{3} {4},{5} ",
                    ToUnits(x1), ToUnits(y1 + tmpY), ToUnits(x1 + tmpX), ToUnits(y1), ToUnits(xc), ToUnits(y1)));
                fillPath.Append(string.Format("C {0},{1} {2},{3} {4},{5} ",
                    ToUnits(x2 - tmpX), ToUnits(y1), ToUnits(x2), ToUnits(y1 + tmpY), ToUnits(x2), ToUnits(yc)));
                fillPath.Append(string.Format("C {0},{1} {2},{3} {4},{5} Z",
                    ToUnits(x2), ToUnits(y2 - tmpY), ToUnits(x2 - tmpX), ToUnits(y2), ToUnits(xc), ToUnits(y2)));
                strokePath = fillPath;
            }
            #endregion

            #region RoundedRectangle
            if (shape.ShapeType is StiRoundedRectangleShapeType)
            {
                float rnd = (shape.ShapeType as StiRoundedRectangleShapeType).Round;
                double side = svgData.Width;
                if (side > svgData.Height) side = svgData.Height;
                double offs = Math.Min(side, 100 * shape.Page.Zoom) * rnd;
                double tmp = offs * (1 - pdfCKT);
                double x1 = svgData.X;
                double y1 = svgData.Y;
                double x2 = svgData.Right;
                double y2 = svgData.Bottom;

                fillPath.Append(string.Format("M {0},{1} ",
                    ToUnits(x1 + offs), ToUnits(y2)));
                fillPath.Append(string.Format("C {0},{1} {2},{3} {4},{5} ",
                    ToUnits(x1 + tmp), ToUnits(y2), ToUnits(x1), ToUnits(y2 - tmp), ToUnits(x1), ToUnits(y2 - offs)));
                fillPath.Append(string.Format("V {0} ", ToUnits(y1 + offs)));
                fillPath.Append(string.Format("C {0},{1} {2},{3} {4},{5} ",
                    ToUnits(x1), ToUnits(y1 + tmp), ToUnits(x1 + tmp), ToUnits(y1), ToUnits(x1 + offs), ToUnits(y1)));
                fillPath.Append(string.Format("H {0} ", ToUnits(x2 - offs)));
                fillPath.Append(string.Format("C {0},{1} {2},{3} {4},{5} ",
                    ToUnits(x2 - tmp), ToUnits(y1), ToUnits(x2), ToUnits(y1 + tmp), ToUnits(x2), ToUnits(y1 + offs)));
                fillPath.Append(string.Format("V {0} ", ToUnits(y2 - offs)));
                fillPath.Append(string.Format("C {0},{1} {2},{3} {4},{5} Z",
                    ToUnits(x2), ToUnits(y2 - tmp), ToUnits(x2 - tmp), ToUnits(y2), ToUnits(x2 - offs), ToUnits(y2)));
                strokePath = fillPath;
            }
            #endregion

            #region Octagon
            if (shape.ShapeType is StiOctagonShapeType)
            {
                StiOctagonShapeType octagonShape = shape.ShapeType as StiOctagonShapeType;
                double bevelx = (shape.Report != null ? shape.Report.Unit.ConvertToHInches(octagonShape.Bevel) : octagonShape.Bevel) * shape.Page.Zoom;
                double bevely = bevelx;
                if (octagonShape.AutoSize)
                {
                    bevelx = svgData.Width / (2.414f * 1.414f);
                    bevely = svgData.Height / (2.414f * 1.414f);
                }
                if (bevelx > svgData.Width / 2) bevelx = svgData.Width / 2;
                if (bevely > svgData.Height / 2) bevely = svgData.Height / 2;

                fillPath.Append(string.Format("M {0},{1} ",
                    ToUnits(svgData.X + bevelx), ToUnits(svgData.Y)));
                fillPath.Append(string.Format("L {0},{1} {2},{3} ",
                    ToUnits(svgData.Right - bevelx), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Y + bevely)));
                fillPath.Append(string.Format("{0},{1} {2},{3} ",
                    ToUnits(svgData.Right), ToUnits(svgData.Bottom - bevely), ToUnits(svgData.Right - bevelx), ToUnits(svgData.Bottom)));
                fillPath.Append(string.Format("{0},{1} {2},{3} ",
                    ToUnits(svgData.X + bevelx), ToUnits(svgData.Bottom), ToUnits(svgData.X), ToUnits(svgData.Bottom - bevely)));
                fillPath.Append(string.Format("{0},{1} {2},{3} ",
                    ToUnits(svgData.X + bevelx), ToUnits(svgData.Bottom), ToUnits(svgData.X), ToUnits(svgData.Bottom - bevely)));
                fillPath.Append(string.Format("{0},{1} {2},{3} Z",
                    ToUnits(svgData.X), ToUnits(svgData.Y + bevely), ToUnits(svgData.X + bevelx), ToUnits(svgData.Y)));
                strokePath = fillPath;
            }
            #endregion

            #region Arrow
            if (shape.ShapeType is StiArrowShapeType)
            {
                StiShapeDirection ssd = (shape.ShapeType as StiArrowShapeType).Direction;
                float arrowW = (shape.ShapeType as StiArrowShapeType).ArrowWidth;
                float arrowH = (shape.ShapeType as StiArrowShapeType).ArrowHeight;
                double arw = svgData.Width * arrowW;
                double arh = svgData.Height * arrowH;
                if ((ssd == StiShapeDirection.Left) || (ssd == StiShapeDirection.Right))
                {
                    arw = svgData.Height * arrowW;
                    arh = svgData.Width * arrowH;
                }

                if (ssd == StiShapeDirection.Up)
                {
                    fillPath.Append(string.Format("M {0},{1} ", ToUnits(svgData.Right - arw), ToUnits(svgData.Bottom)));
                    fillPath.Append(string.Format("L {0},{1} ", ToUnits(svgData.Right - arw), ToUnits(svgData.Y + arh)));
                    fillPath.Append(string.Format("{0},{1} ", ToUnits(svgData.Right), ToUnits(svgData.Y + arh)));
                    fillPath.Append(string.Format("{0},{1} ", ToUnits(svgData.X + svgData.Width / 2), ToUnits(svgData.Y)));
                    fillPath.Append(string.Format("{0},{1} ", ToUnits(svgData.X), ToUnits(svgData.Y + arh)));
                    fillPath.Append(string.Format("{0},{1} ", ToUnits(svgData.X + arw), ToUnits(svgData.Y + arh)));
                    fillPath.Append(string.Format("{0},{1} ", ToUnits(svgData.X + arw), ToUnits(svgData.Bottom)));
                    fillPath.Append(string.Format("{0},{1} Z", ToUnits(svgData.Right - arw), ToUnits(svgData.Bottom)));
                }
                if (ssd == StiShapeDirection.Down)
                {
                    fillPath.Append(string.Format("M {0},{1} ", ToUnits(svgData.X + arw), ToUnits(svgData.Y)));
                    fillPath.Append(string.Format("L {0},{1} ", ToUnits(svgData.X + arw), ToUnits(svgData.Bottom - arh)));
                    fillPath.Append(string.Format("{0},{1} ", ToUnits(svgData.X), ToUnits(svgData.Bottom - arh)));
                    fillPath.Append(string.Format("{0},{1} ", ToUnits(svgData.X + svgData.Width / 2), ToUnits(svgData.Bottom)));
                    fillPath.Append(string.Format("{0},{1} ", ToUnits(svgData.Right), ToUnits(svgData.Bottom - arh)));
                    fillPath.Append(string.Format("{0},{1} ", ToUnits(svgData.Right - arw), ToUnits(svgData.Bottom - arh)));
                    fillPath.Append(string.Format("{0},{1} ", ToUnits(svgData.Right - arw), ToUnits(svgData.Y)));
                    fillPath.Append(string.Format("{0},{1} Z", ToUnits(svgData.X + arw), ToUnits(svgData.Y)));
                }
                if (ssd == StiShapeDirection.Left)
                {
                    fillPath.Append(string.Format("M {0},{1} ", ToUnits(svgData.Right), ToUnits(svgData.Y + arw)));
                    fillPath.Append(string.Format("L {0},{1} ", ToUnits(svgData.X + arh), ToUnits(svgData.Y + arw)));
                    fillPath.Append(string.Format("{0},{1} ", ToUnits(svgData.X + arh), ToUnits(svgData.Y)));
                    fillPath.Append(string.Format("{0},{1} ", ToUnits(svgData.X), ToUnits(svgData.Y + svgData.Height / 2)));
                    fillPath.Append(string.Format("{0},{1} ", ToUnits(svgData.X + arh), ToUnits(svgData.Bottom)));
                    fillPath.Append(string.Format("{0},{1} ", ToUnits(svgData.X + arh), ToUnits(svgData.Bottom - arw)));
                    fillPath.Append(string.Format("{0},{1} ", ToUnits(svgData.Right), ToUnits(svgData.Bottom - arw)));
                    fillPath.Append(string.Format("{0},{1} Z", ToUnits(svgData.Right), ToUnits(svgData.Y + arw)));
                }
                if (ssd == StiShapeDirection.Right)
                {
                    fillPath.Append(string.Format("M {0},{1} ", ToUnits(svgData.X), ToUnits(svgData.Bottom - arw)));
                    fillPath.Append(string.Format("L {0},{1} ", ToUnits(svgData.X + svgData.Width - arh), ToUnits(svgData.Bottom - arw)));
                    fillPath.Append(string.Format("{0},{1} ", ToUnits(svgData.Right - arh), ToUnits(svgData.Bottom)));
                    fillPath.Append(string.Format("{0},{1} ", ToUnits(svgData.Right), ToUnits(svgData.Y + svgData.Height / 2)));
                    fillPath.Append(string.Format("{0},{1} ", ToUnits(svgData.Right - arh), ToUnits(svgData.Y)));
                    fillPath.Append(string.Format("{0},{1} ", ToUnits(svgData.Right - arh), ToUnits(svgData.Y + arw)));
                    fillPath.Append(string.Format("{0},{1} ", ToUnits(svgData.X), ToUnits(svgData.Y + arw)));
                    fillPath.Append(string.Format("{0},{1} Z", ToUnits(svgData.X), ToUnits(svgData.Bottom - arw)));
                }

                strokePath = fillPath;
            }
            #endregion

            #region ComplexArrow

            if (shape.ShapeType is StiComplexArrowShapeType)
            {
                double restHeight = (svgData.Width < svgData.Height) ? svgData.Width / 2 : svgData.Height / 2;
                double topBottomSpace = (svgData.Height / 3.8f);
                double leftRightSpace = (svgData.Width / 3.8f);
                double restWidth = (svgData.Height < svgData.Width) ? svgData.Height / 2 : svgData.Width / 2;

                switch ((shape.ShapeType as StiComplexArrowShapeType).Direction)
                {
                    case StiShapeDirection.Left:
                    case StiShapeDirection.Right:
                        fillPath.Append(string.Format("M {0},{1} ",
                            ToUnits(svgData.X), ToUnits(svgData.Y + svgData.Height / 2)));
                        fillPath.Append(string.Format("L {0},{1} ",
                            ToUnits(svgData.X + restHeight), ToUnits(svgData.Y)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X + restHeight), ToUnits(svgData.Y + topBottomSpace)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right - restHeight), ToUnits(svgData.Y + topBottomSpace)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right - restHeight), ToUnits(svgData.Y)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right), ToUnits(svgData.Y + svgData.Height / 2)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right - restHeight), ToUnits(svgData.Bottom)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right - restHeight), ToUnits(svgData.Bottom - topBottomSpace)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X + restHeight), ToUnits(svgData.Bottom - topBottomSpace)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X + restHeight), ToUnits(svgData.Bottom)));
                        fillPath.Append(string.Format("{0},{1} Z",
                            ToUnits(svgData.X), ToUnits(svgData.Y + svgData.Height / 2)));
                        break;

                    case StiShapeDirection.Down:
                    case StiShapeDirection.Up:
                        fillPath.Append(string.Format("M {0},{1} ",
                            ToUnits(svgData.X), ToUnits(svgData.Y + restWidth)));
                        fillPath.Append(string.Format("L {0},{1} ",
                            ToUnits(svgData.X + svgData.Width / 2), ToUnits(svgData.Y)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right), ToUnits(svgData.Y + restWidth)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right - leftRightSpace), ToUnits(svgData.Y + restWidth)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right - leftRightSpace), ToUnits(svgData.Bottom - restWidth)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right), ToUnits(svgData.Bottom - restWidth)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X + svgData.Width / 2), ToUnits(svgData.Bottom)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X), ToUnits(svgData.Bottom - restWidth)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X + leftRightSpace), ToUnits(svgData.Bottom - restWidth)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X + leftRightSpace), ToUnits(svgData.Y + restWidth)));
                        fillPath.Append(string.Format("{0},{1} Z",
                            ToUnits(svgData.X), ToUnits(svgData.Y + restWidth)));
                        break;
                }

                strokePath = fillPath;
            }

            #endregion

            #region BentArrow

            if (shape.ShapeType is StiBentArrowShapeType)
            {
                double lineHeight = 0;
                double arrowWidth = 0;
                double space = 0;
                if (svgData.Height > svgData.Width)
                {
                    arrowWidth = svgData.Width / 4;
                    lineHeight = arrowWidth;
                    space = arrowWidth / 2;
                }
                else
                {
                    lineHeight = (int)(svgData.Height / 4);
                    arrowWidth = lineHeight;
                    space = arrowWidth / 2;
                }

                switch ((shape.ShapeType as StiBentArrowShapeType).Direction)
                {
                    #region Up

                    case StiShapeDirection.Up:
                        fillPath.Append(string.Format("M {0},{1} ",
                            ToUnits(svgData.X), ToUnits(svgData.Bottom)));
                        fillPath.Append(string.Format("L {0},{1} ",
                            ToUnits(svgData.X), ToUnits(svgData.Bottom - lineHeight)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right - (space + lineHeight)), ToUnits(svgData.Bottom - lineHeight)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right - (space + lineHeight)), ToUnits(svgData.Y + arrowWidth)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right - arrowWidth * 2), ToUnits(svgData.Y + arrowWidth)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right - arrowWidth), ToUnits(svgData.Y)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right), ToUnits(svgData.Y + arrowWidth)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right - space), ToUnits(svgData.Y + arrowWidth)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right - space), ToUnits(svgData.Bottom)));
                        fillPath.Append(string.Format("{0},{1} Z",
                            ToUnits(svgData.X), ToUnits(svgData.Bottom)));
                        break;

                    #endregion

                    #region Left

                    case StiShapeDirection.Left:
                        fillPath.Append(string.Format("M {0},{1} ",
                            ToUnits(svgData.Right), ToUnits(svgData.Bottom)));
                        fillPath.Append(string.Format("L {0},{1} ",
                            ToUnits(svgData.Right), ToUnits(svgData.Y + space)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X + arrowWidth), ToUnits(svgData.Y + space)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X + arrowWidth), ToUnits(svgData.Y)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X), ToUnits(svgData.Y + arrowWidth)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X + arrowWidth), ToUnits(svgData.Y + arrowWidth * 2)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X + arrowWidth), ToUnits(svgData.Y + arrowWidth + space)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right - lineHeight), ToUnits(svgData.Y + arrowWidth + space)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right - lineHeight), ToUnits(svgData.Bottom)));
                        fillPath.Append(string.Format("{0},{1} Z",
                            ToUnits(svgData.Right), ToUnits(svgData.Bottom)));
                        break;

                    #endregion

                    #region Down

                    case StiShapeDirection.Down:
                        fillPath.Append(string.Format("M {0},{1} ",
                            ToUnits(svgData.Right), ToUnits(svgData.Y)));
                        fillPath.Append(string.Format("L {0},{1} ",
                            ToUnits(svgData.X + space), ToUnits(svgData.Y)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X + space), ToUnits(svgData.Bottom - arrowWidth)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X), ToUnits(svgData.Bottom - arrowWidth)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X + arrowWidth), ToUnits(svgData.Bottom)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X + arrowWidth * 2), ToUnits(svgData.Bottom - arrowWidth)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X + arrowWidth + space), ToUnits(svgData.Bottom - arrowWidth)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X + arrowWidth + space), ToUnits(svgData.Y + lineHeight)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right), ToUnits(svgData.Y + lineHeight)));
                        fillPath.Append(string.Format("{0},{1} Z",
                            ToUnits(svgData.Right), ToUnits(svgData.Y)));
                        break;

                    #endregion

                    #region Right

                    case StiShapeDirection.Right:
                        fillPath.Append(string.Format("M {0},{1} ",
                            ToUnits(svgData.X), ToUnits(svgData.Y)));
                        fillPath.Append(string.Format("L {0},{1} ",
                            ToUnits(svgData.X), ToUnits(svgData.Bottom - space)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right - arrowWidth), ToUnits(svgData.Bottom - space)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right - arrowWidth), ToUnits(svgData.Bottom)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right), ToUnits(svgData.Bottom - arrowWidth)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right - arrowWidth), ToUnits(svgData.Bottom - arrowWidth * 2)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right - arrowWidth), ToUnits(svgData.Bottom - arrowWidth - space)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X + lineHeight), ToUnits(svgData.Bottom - arrowWidth - space)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X + lineHeight), ToUnits(svgData.Y)));
                        fillPath.Append(string.Format("{0},{1} Z",
                            ToUnits(svgData.X), ToUnits(svgData.Y)));
                        break;

                        #endregion
                }
                strokePath = fillPath;
            }

            #endregion

            #region Chevron

            if (shape.ShapeType is StiChevronShapeType)
            {
                double rest = (svgData.Width > svgData.Height) ? (svgData.Height / 2) : (svgData.Width / 2);
                switch ((shape.ShapeType as StiChevronShapeType).Direction)
                {
                    #region Right

                    case StiShapeDirection.Right:
                        fillPath.Append(string.Format("M {0},{1} ",
                            ToUnits(svgData.X), ToUnits(svgData.Y)));
                        fillPath.Append(string.Format("L {0},{1} ",
                            ToUnits(svgData.X + rest), ToUnits(svgData.Y + svgData.Height / 2)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X), ToUnits(svgData.Bottom)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right - rest), ToUnits(svgData.Bottom)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right), ToUnits(svgData.Y + svgData.Height / 2)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right - rest), ToUnits(svgData.Y)));
                        fillPath.Append(string.Format("{0},{1} Z",
                            ToUnits(svgData.X), ToUnits(svgData.Y)));
                        break;

                    #endregion

                    #region Left

                    case StiShapeDirection.Left:
                        fillPath.Append(string.Format("M {0},{1} ",
                            ToUnits(svgData.Right), ToUnits(svgData.Y)));
                        fillPath.Append(string.Format("L {0},{1} ",
                            ToUnits(svgData.X + rest), ToUnits(svgData.Y)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X), ToUnits(svgData.Y + svgData.Height / 2)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X + rest), ToUnits(svgData.Bottom)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right), ToUnits(svgData.Bottom)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right - rest), ToUnits(svgData.Y + svgData.Height / 2)));
                        fillPath.Append(string.Format("{0},{1} Z",
                            ToUnits(svgData.Right), ToUnits(svgData.Y)));
                        break;

                    #endregion

                    #region Up

                    case StiShapeDirection.Up:
                        fillPath.Append(string.Format("M {0},{1} ",
                            ToUnits(svgData.X), ToUnits(svgData.Y + rest)));
                        fillPath.Append(string.Format("L {0},{1} ",
                            ToUnits(svgData.X + svgData.Width / 2), ToUnits(svgData.Y)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right), ToUnits(svgData.Y + rest)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right), ToUnits(svgData.Bottom)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X + svgData.Width / 2), ToUnits(svgData.Bottom - rest)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X), ToUnits(svgData.Bottom)));
                        fillPath.Append(string.Format("{0},{1} Z",
                            ToUnits(svgData.X), ToUnits(svgData.Y + rest)));
                        break;

                    #endregion

                    #region Down

                    case StiShapeDirection.Down:
                        fillPath.Append(string.Format("M {0},{1} ",
                            ToUnits(svgData.X), ToUnits(svgData.Y)));
                        fillPath.Append(string.Format("L {0},{1} ",
                            ToUnits(svgData.X + svgData.Width / 2), ToUnits(svgData.Y + rest)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right), ToUnits(svgData.Y)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.Right), ToUnits(svgData.Bottom - rest)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X + svgData.Width / 2), ToUnits(svgData.Bottom)));
                        fillPath.Append(string.Format("{0},{1} ",
                            ToUnits(svgData.X), ToUnits(svgData.Bottom - rest)));
                        fillPath.Append(string.Format("{0},{1} Z",
                            ToUnits(svgData.X), ToUnits(svgData.Y)));
                        break;

                        #endregion
                }
                strokePath = fillPath;
            }

            #endregion

            #region Division

            if (shape.ShapeType is StiDivisionShapeType)
            {
                double restHeight = svgData.Height / 3;
                restHeight += 4;

                fillPath.Append(string.Format("M {0},{1} ",
                    ToUnits(svgData.X), ToUnits(svgData.Y + restHeight)));
                fillPath.Append(string.Format("L {0},{1} {2},{3} {4},{5} {6},{7} ",
                    ToUnits(svgData.Right), ToUnits(svgData.Y + restHeight), ToUnits(svgData.Right), ToUnits(svgData.Bottom - restHeight),
                    ToUnits(svgData.X), ToUnits(svgData.Bottom - restHeight), ToUnits(svgData.X), ToUnits(svgData.Y + restHeight)));

                restHeight -= 4;

                fillPath.Append(string.Format("M {0},{1} ",
                    ToUnits(svgData.Width / 2 - restHeight / 2 + svgData.X), ToUnits(svgData.Y + 1 + restHeight / 2)));
                fillPath.Append(string.Format("a {0},{0} 0 1,0 {1},0 a {0},{0} 0 1,0 -{1},0 ",
                    ToUnits(restHeight / 2), ToUnits(restHeight)));

                fillPath.Append(string.Format("M {0},{1} ",
                    ToUnits(svgData.Width / 2 - restHeight / 2 + svgData.X), ToUnits(svgData.Bottom - 2 - restHeight / 2)));
                fillPath.Append(string.Format("a {0},{0} 0 1,0 {1},0 a {0},{0} 0 1,0 -{1},0 Z",
                    ToUnits(restHeight / 2), ToUnits(restHeight)));

                strokePath = fillPath;
            }

            #endregion

            #region Equal

            if (shape.ShapeType is StiEqualShapeType)
            {
                double height = (svgData.Height - (svgData.Height / 6)) / 2;

                fillPath.Append(string.Format("M {0},{1} h {2} v {3} H {0} V {1} ",
                    ToUnits(svgData.X), ToUnits(svgData.Bottom - height), ToUnits(svgData.Width), ToUnits(height)));
                fillPath.Append(string.Format("M {0},{1} h {2} v {3} H {0} V {1} Z",
                    ToUnits(svgData.X), ToUnits(svgData.Y), ToUnits(svgData.Width), ToUnits(height)));

                strokePath = fillPath;
            }

            #endregion

            #region Flowchart: Card

            if (shape.ShapeType is StiFlowchartCardShapeType)
            {
                fillPath.Append(string.Format("M {0},{1} L {2},{3} ",
                    ToUnits(svgData.Right), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Bottom)));
                fillPath.Append(string.Format("{0},{1} {2},{3} {4},{5} {6},{7} Z",
                    ToUnits(svgData.X), ToUnits(svgData.Bottom), ToUnits(svgData.X), ToUnits(svgData.Y + svgData.Height / 5),
                    ToUnits(svgData.X + svgData.Width / 5), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Y)));

                strokePath = fillPath;
            }

            #endregion

            #region Flowchart: Collate

            if (shape.ShapeType is StiFlowchartCollateShapeType)
            {
                switch ((shape.ShapeType as StiFlowchartCollateShapeType).Direction)
                {
                    case StiShapeDirection.Down:
                    case StiShapeDirection.Up:
                        fillPath.Append(string.Format("M {0},{1} ",
                            ToUnits(svgData.X), ToUnits(svgData.Y)));
                        fillPath.Append(string.Format("L {0},{1} {2},{3} {4},{5} {6},{7} Z",
                            ToUnits(svgData.Right), ToUnits(svgData.Bottom), ToUnits(svgData.X), ToUnits(svgData.Bottom),
                            ToUnits(svgData.Right), ToUnits(svgData.Y), ToUnits(svgData.X), ToUnits(svgData.Y)));
                        break;

                    case StiShapeDirection.Left:
                    case StiShapeDirection.Right:
                        fillPath.Append(string.Format("M {0},{1} ",
                            ToUnits(svgData.X), ToUnits(svgData.Y)));
                        fillPath.Append(string.Format("L {0},{1} {2},{3} {4},{5} {6},{7} Z",
                            ToUnits(svgData.Right), ToUnits(svgData.Bottom), ToUnits(svgData.Right), ToUnits(svgData.Y),
                            ToUnits(svgData.X), ToUnits(svgData.Bottom), ToUnits(svgData.X), ToUnits(svgData.Y)));
                        break;
                }

                strokePath = fillPath;
            }

            #endregion

            #region Flowchart: Decision

            if (shape.ShapeType is StiFlowchartDecisionShapeType)
            {
                fillPath.Append(string.Format("M {0},{1} ",
                    ToUnits(svgData.X), ToUnits(svgData.Y + svgData.Height / 2)));
                fillPath.Append(string.Format("L {0},{1} {2},{3} {4},{5} {6},{7} Z",
                    ToUnits(svgData.X + svgData.Width / 2), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Y + svgData.Height / 2),
                    ToUnits(svgData.X + svgData.Width / 2), ToUnits(svgData.Bottom), ToUnits(svgData.X), ToUnits(svgData.Y + svgData.Height / 2)));

                strokePath = fillPath;
            }

            #endregion

            #region Flowchart: Manual Input

            if (shape.ShapeType is StiFlowchartManualInputShapeType)
            {
                fillPath.Append(string.Format("M {0},{1} ",
                    ToUnits(svgData.X), ToUnits(svgData.Y + svgData.Height / 5)));
                fillPath.Append(string.Format("L {0},{1} {2},{3} {4},{5} {6},{7} Z",
                    ToUnits(svgData.Right), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Bottom),
                    ToUnits(svgData.X), ToUnits(svgData.Bottom), ToUnits(svgData.X), ToUnits(svgData.Y + svgData.Height / 5)));

                strokePath = fillPath;
            }

            #endregion

            #region Flowchart: Off Page Connector

            if (shape.ShapeType is StiFlowchartOffPageConnectorShapeType)
            {
                double restHeight = svgData.Height / 5;
                double restWidth = svgData.Width / 5;
                switch ((shape.ShapeType as StiFlowchartOffPageConnectorShapeType).Direction)
                {
                    case StiShapeDirection.Down:
                        fillPath.Append(string.Format("M {0},{1} L {2},{3} ",
                            ToUnits(svgData.X), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Y)));
                        fillPath.Append(string.Format("{0},{1} {2},{3} {4},{5} {6},{7} Z",
                            ToUnits(svgData.Right), ToUnits(svgData.Bottom - restHeight), ToUnits(svgData.X + svgData.Width / 2), ToUnits(svgData.Bottom),
                            ToUnits(svgData.X), ToUnits(svgData.Bottom - restHeight), ToUnits(svgData.X), ToUnits(svgData.Y)));
                        break;

                    case StiShapeDirection.Up:
                        fillPath.Append(string.Format("M {0},{1} L {2},{3} ",
                            ToUnits(svgData.X), ToUnits(svgData.Bottom), ToUnits(svgData.X), ToUnits(svgData.Y + restHeight)));
                        fillPath.Append(string.Format("{0},{1} {2},{3} {4},{5} {6},{7} Z",
                            ToUnits(svgData.X + svgData.Width / 2), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Y + restHeight),
                            ToUnits(svgData.Right), ToUnits(svgData.Bottom), ToUnits(svgData.X), ToUnits(svgData.Bottom)));
                        break;

                    case StiShapeDirection.Left:
                        fillPath.Append(string.Format("M {0},{1} L {2},{3} ",
                            ToUnits(svgData.X + restWidth), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Y)));
                        fillPath.Append(string.Format("{0},{1} {2},{3} {4},{5} {6},{7} Z",
                            ToUnits(svgData.Right), ToUnits(svgData.Bottom), ToUnits(svgData.X + restWidth), ToUnits(svgData.Bottom),
                            ToUnits(svgData.X), ToUnits(svgData.Y + svgData.Height / 2), ToUnits(svgData.X + restWidth), ToUnits(svgData.Y)));
                        break;

                    case StiShapeDirection.Right:
                        fillPath.Append(string.Format("M {0},{1} L {2},{3} ",
                            ToUnits(svgData.X), ToUnits(svgData.Y), ToUnits(svgData.Right - restWidth), ToUnits(svgData.Y)));
                        fillPath.Append(string.Format("{0},{1} {2},{3} {4},{5} {6},{7} Z",
                            ToUnits(svgData.Right), ToUnits(svgData.Y + svgData.Height / 2), ToUnits(svgData.Right - restWidth), ToUnits(svgData.Bottom),
                            ToUnits(svgData.X), ToUnits(svgData.Bottom), ToUnits(svgData.X), ToUnits(svgData.Y)));
                        break;
                }

                strokePath = fillPath;
            }

            #endregion

            #region Flowchart: Preparation

            if (shape.ShapeType is StiFlowchartPreparationShapeType)
            {
                double restWidth = svgData.Width / 5;
                double restHeight = svgData.Height / 5;
                double xCenter = svgData.Width / 2;
                double yCenter = svgData.Height / 2;

                switch ((shape.ShapeType as StiFlowchartPreparationShapeType).Direction)
                {
                    case StiShapeDirection.Left:
                    case StiShapeDirection.Right:
                        fillPath.Append(string.Format("M {0},{1} L {2},{3} {4},{5} ",
                            ToUnits(svgData.X), ToUnits(svgData.Y + yCenter),
                            ToUnits(svgData.X + restWidth), ToUnits(svgData.Y), ToUnits(svgData.Right - restWidth), ToUnits(svgData.Y)));
                        fillPath.Append(string.Format("{0},{1} {2},{3} {4},{5} {6},{7} Z",
                            ToUnits(svgData.Right), ToUnits(svgData.Y + yCenter), ToUnits(svgData.Right - restWidth), ToUnits(svgData.Bottom),
                            ToUnits(svgData.X + restWidth), ToUnits(svgData.Bottom), ToUnits(svgData.X), ToUnits(svgData.Y + yCenter)));
                        break;

                    case StiShapeDirection.Down:
                    case StiShapeDirection.Up:
                        fillPath.Append(string.Format("M {0},{1} L {2},{3} {4},{5} ",
                            ToUnits(svgData.X + xCenter), ToUnits(svgData.Y),
                            ToUnits(svgData.Right), ToUnits(svgData.Y + restHeight), ToUnits(svgData.Right), ToUnits(svgData.Bottom - restHeight)));
                        fillPath.Append(string.Format("{0},{1} {2},{3} {4},{5} {6},{7} Z",
                            ToUnits(svgData.X + xCenter), ToUnits(svgData.Bottom), ToUnits(svgData.X), ToUnits(svgData.Bottom - restHeight),
                            ToUnits(svgData.X), ToUnits(svgData.Y + restHeight), ToUnits(svgData.X + xCenter), ToUnits(svgData.Y)));
                        break;
                }

                strokePath = fillPath;
            }

            #endregion

            #region Flowchart: Sort

            if (shape.ShapeType is StiFlowchartSortShapeType)
            {
                fillPath.Append(string.Format("M {0},{1} L {2},{3} ",
                    ToUnits(svgData.X), ToUnits(svgData.Y + svgData.Height / 2), ToUnits(svgData.X + svgData.Width / 2), ToUnits(svgData.Y)));
                fillPath.Append(string.Format("{0},{1} {2},{3} {4},{5} {6},{7} Z",
                    ToUnits(svgData.Right), ToUnits(svgData.Y + svgData.Height / 2), ToUnits(svgData.X + svgData.Width / 2), ToUnits(svgData.Bottom),
                    ToUnits(svgData.X), ToUnits(svgData.Y + svgData.Height / 2), ToUnits(svgData.Right), ToUnits(svgData.Y + svgData.Height / 2)));

                strokePath = fillPath;
            }

            #endregion

            #region Frame

            if (shape.ShapeType is StiFrameShapeType)
            {
                double restWidth = svgData.Width / 7;
                double restHeight = svgData.Height / 7;

                fillPath.Append(string.Format("M {0},{1} L {2},{3} {4},{5} {6},{7} {8},{9} ",
                    ToUnits(svgData.X), ToUnits(svgData.Y),
                    ToUnits(svgData.Right), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Bottom),
                    ToUnits(svgData.X), ToUnits(svgData.Bottom), ToUnits(svgData.X), ToUnits(svgData.Y)));
                fillPath.Append(string.Format("M {0},{1} L {2},{3} {4},{5} {6},{7} {8},{9} Z",
                    ToUnits(svgData.X + restWidth), ToUnits(svgData.Y + restHeight),
                    ToUnits(svgData.X + restWidth), ToUnits(svgData.Bottom - restHeight), ToUnits(svgData.Right - restWidth), ToUnits(svgData.Bottom - restHeight),
                    ToUnits(svgData.Right - restWidth), ToUnits(svgData.Y + restHeight), ToUnits(svgData.X + restWidth), ToUnits(svgData.Y + restHeight)));

                strokePath = fillPath;
            }

            #endregion

            #region Minus

            if (shape.ShapeType is StiMinusShapeType)
            {
                double restHeight = svgData.Height / 3;

                fillPath.Append(string.Format("M {0},{1} ",
                    ToUnits(svgData.X), ToUnits(svgData.Y + restHeight)));
                fillPath.Append(string.Format("L {0},{1} {2},{3} {4},{5} {6},{7} Z",
                    ToUnits(svgData.Right), ToUnits(svgData.Y + restHeight), ToUnits(svgData.Right), ToUnits(svgData.Bottom - restHeight),
                    ToUnits(svgData.X), ToUnits(svgData.Bottom - restHeight), ToUnits(svgData.X), ToUnits(svgData.Y + restHeight)));

                strokePath = fillPath;
            }

            #endregion

            #region Multiply

            if (shape.ShapeType is StiMultiplyShapeType)
            {
                double restWidth = svgData.Width / 4;
                double restHeight = svgData.Height / 4;

                fillPath.Append(string.Format("M {0},{1} ",
                    ToUnits(svgData.X), ToUnits(svgData.Y + restHeight)));
                fillPath.Append(string.Format("L {0},{1} {2},{3} {4},{5} {6},{7} ",
                    ToUnits(svgData.X + restWidth), ToUnits(svgData.Y), ToUnits(svgData.X + svgData.Width / 2), ToUnits(svgData.Y + restHeight),
                    ToUnits(svgData.Right - restWidth), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Y + restHeight)));
                fillPath.Append(string.Format("{0},{1} {2},{3} {4},{5} {6},{7} ",
                    ToUnits(svgData.Right - restWidth), ToUnits(svgData.Y + svgData.Height / 2), ToUnits(svgData.Right), ToUnits(svgData.Bottom - restHeight),
                    ToUnits(svgData.Right - restWidth), ToUnits(svgData.Bottom), ToUnits(svgData.X + svgData.Width / 2), ToUnits(svgData.Bottom - restHeight)));
                fillPath.Append(string.Format("{0},{1} {2},{3} {4},{5} {6},{7} Z",
                    ToUnits(svgData.X + restWidth), ToUnits(svgData.Bottom), ToUnits(svgData.X), ToUnits(svgData.Bottom - restHeight),
                    ToUnits(svgData.X + restWidth), ToUnits(svgData.Y + svgData.Height / 2), ToUnits(svgData.X), ToUnits(svgData.Y + restHeight)));

                strokePath = fillPath;
            }

            #endregion

            #region Parallelogram

            if (shape.ShapeType is StiParallelogramShapeType)
            {
                double restWidth = svgData.Width / 7;
                double restHeight = svgData.Height / 7;

                fillPath.Append(string.Format("M {0},{1} L {2},{3} {4},{5} {6},{7} {8},{9} Z",
                    ToUnits(svgData.X), ToUnits(svgData.Bottom),
                    ToUnits(svgData.X + svgData.Width / 5), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Y),
                    ToUnits(svgData.Right - svgData.Width / 5), ToUnits(svgData.Bottom), ToUnits(svgData.X), ToUnits(svgData.Bottom)));

                strokePath = fillPath;
            }

            #endregion

            #region Plus

            if (shape.ShapeType is StiPlusShapeType)
            {
                double restWidth = svgData.Width / 3;
                double restHeight = svgData.Height / 3;

                fillPath.Append(string.Format("M {0},{1} ",
                    ToUnits(svgData.X + restWidth), ToUnits(svgData.Y)));
                fillPath.Append(string.Format("L {0},{1} {2},{3} {4},{5} {6},{7} ",
                    ToUnits(svgData.Right - restWidth), ToUnits(svgData.Y), ToUnits(svgData.Right - restWidth), ToUnits(svgData.Y + restHeight),
                    ToUnits(svgData.Right), ToUnits(svgData.Y + restHeight), ToUnits(svgData.Right), ToUnits(svgData.Bottom - restHeight)));
                fillPath.Append(string.Format("{0},{1} {2},{3} {4},{5} {6},{7} ",
                    ToUnits(svgData.Right - restWidth), ToUnits(svgData.Bottom - restHeight), ToUnits(svgData.Right - restWidth), ToUnits(svgData.Bottom),
                    ToUnits(svgData.X + restWidth), ToUnits(svgData.Bottom), ToUnits(svgData.X + restWidth), ToUnits(svgData.Bottom - restHeight)));
                fillPath.Append(string.Format("{0},{1} {2},{3} {4},{5} {6},{7} Z",
                    ToUnits(svgData.X), ToUnits(svgData.Bottom - restHeight), ToUnits(svgData.X), ToUnits(svgData.Y + restHeight),
                    ToUnits(svgData.X + restWidth), ToUnits(svgData.Y + restHeight), ToUnits(svgData.X + restWidth), ToUnits(svgData.Y)));

                strokePath = fillPath;
            }

            #endregion

            #region Regular: Pentagon

            if (shape.ShapeType is StiRegularPentagonShapeType)
            {
                double restTop = svgData.Height / 2.6f;
                double restLeft = svgData.Width / 5.5f;

                fillPath.Append(string.Format("M {0},{1} L {2},{3} ",
                    ToUnits(svgData.X), ToUnits(svgData.Y + restTop), ToUnits(svgData.X + svgData.Width / 2), ToUnits(svgData.Y)));
                fillPath.Append(string.Format("{0},{1} {2},{3} {4},{5} {6},{7} Z",
                    ToUnits(svgData.Right), ToUnits(svgData.Y + restTop), ToUnits(svgData.Right - restLeft), ToUnits(svgData.Bottom),
                    ToUnits(svgData.X + restLeft), ToUnits(svgData.Bottom), ToUnits(svgData.X), ToUnits(svgData.Y + restTop)));

                strokePath = fillPath;
            }

            #endregion

            #region Trapezoid

            if (shape.ShapeType is StiTrapezoidShapeType)
            {
                double rest = svgData.Width / 4.75f;
                fillPath.Append(string.Format("M {0},{1} L {2},{3} {4},{5} {6},{7} {8},{9} Z",
                    ToUnits(svgData.X), ToUnits(svgData.Bottom),
                    ToUnits(svgData.X + rest), ToUnits(svgData.Y), ToUnits(svgData.Right - rest), ToUnits(svgData.Y),
                    ToUnits(svgData.Right), ToUnits(svgData.Bottom), ToUnits(svgData.X), ToUnits(svgData.Bottom)));

                strokePath = fillPath;
            }

            #endregion

            #region Snip Same Side Corner Rectangle

            if (shape.ShapeType is StiSnipSameSideCornerRectangleShapeType)
            {
                double restWidth = svgData.Width / 7.2f;
                double restHeight = svgData.Height / 4.6f;

                fillPath.Append(string.Format("M {0},{1} L {2},{3} {4},{5} ",
                    ToUnits(svgData.X), ToUnits(svgData.Y + restHeight),
                    ToUnits(svgData.X + restWidth), ToUnits(svgData.Y), ToUnits(svgData.Right - restWidth), ToUnits(svgData.Y)));
                fillPath.Append(string.Format("{0},{1} {2},{3} {4},{5} {6},{7} Z",
                    ToUnits(svgData.Right), ToUnits(svgData.Y + restHeight), ToUnits(svgData.Right), ToUnits(svgData.Bottom),
                    ToUnits(svgData.X), ToUnits(svgData.Bottom), ToUnits(svgData.X), ToUnits(svgData.Y + restHeight)));

                strokePath = fillPath;
            }

            #endregion

            #region Snip Diagonal Side Corner Rectangle

            if (shape.ShapeType is StiSnipDiagonalSideCornerRectangleShapeType)
            {
                double restWidth = svgData.Width / 7.2f;
                double restHeight = svgData.Height / 4.6f;

                fillPath.Append(string.Format("M {0},{1} L {2},{3} {4},{5} ",
                    ToUnits(svgData.X), ToUnits(svgData.Y),
                    ToUnits(svgData.Right - restWidth), ToUnits(svgData.Y), ToUnits(svgData.Right), ToUnits(svgData.Y + restHeight)));
                fillPath.Append(string.Format("{0},{1} {2},{3} {4},{5} {6},{7} Z",
                    ToUnits(svgData.Right), ToUnits(svgData.Bottom), ToUnits(svgData.X + restWidth), ToUnits(svgData.Bottom),
                    ToUnits(svgData.X), ToUnits(svgData.Bottom - restHeight), ToUnits(svgData.X), ToUnits(svgData.Y)));

                strokePath = fillPath;
            }

            #endregion

            if ((fillPath.Length > 0) || (strokePath.Length > 0))
            {
                #region Write path
                if (fillPath == strokePath)
                {
                    writer.WriteStartElement("path");
                    if (tempColor != Color.Transparent)
                    {
                        WriteFillInfo(writer, tempColor);
                    }
                    else
                    {
                        writer.WriteAttributeString("fill", "none");
                    }
                    if (shape.BorderColor != Color.Transparent)
                    {
                        WriteStrokeInfo(writer, shape.BorderColor, shape.Size, shape.Style);
                    }
                    else
                    {
                        writer.WriteAttributeString("stroke", "none");
                    }
                    writer.WriteAttributeString("d", fillPath.ToString());
                    writer.WriteEndElement();
                }
                else
                {
                    if ((fillPath.Length > 0) && (tempColor != Color.Transparent))
                    {
                        writer.WriteStartElement("path");
                        WriteFillInfo(writer, tempColor);
                        writer.WriteAttributeString("stroke", "none");
                        writer.WriteAttributeString("d", fillPath.ToString());
                        writer.WriteEndElement();
                    }
                    if ((strokePath.Length > 0) && (shape.BorderColor != Color.Transparent))
                    {
                        writer.WriteStartElement("path");
                        writer.WriteAttributeString("fill", "none");
                        WriteStrokeInfo(writer, shape.BorderColor, shape.Size, shape.Style);
                        writer.WriteAttributeString("d", strokePath.ToString());
                        writer.WriteEndElement();
                    }
                }
                #endregion
            }

            var parserText = shape.GetParsedText();
            if (!string.IsNullOrWhiteSpace(parserText))
            {
                var txt = new StiText(shape.ClientRectangle, parserText)
                {
                    Font = shape.Font,
                    TextBrush = new StiSolidBrush(shape.ForeColor),
                    HorAlignment = shape.HorAlignment,
                    VertAlignment = shape.VertAlignment,
                    Margins = shape.Margins,
                    WordWrap = true,
                    TextQuality = StiTextQuality.Typographic,
                    Page = shape.Page
                };
                StiSvgData svgData2 = new StiSvgData()
                {
                    X = svgData.X,
                    Y = svgData.Y,
                    Width = svgData.Width,
                    Height = svgData.Height,
                    Component = txt
                };
                WriteText(writer, svgData2, xmlIndentation, false, guids, shape.BackgroundColor);
            }
        }

        private static void WriteRoundedRectanglePrimitive(XmlTextWriter writer, StiSvgData svgData)
        {
            StiRoundedRectanglePrimitive rrp = svgData.Component as StiRoundedRectanglePrimitive;
            if (rrp.Color != Color.Transparent)
            {
                #region Write path
                StringBuilder strokePath = new StringBuilder();

                float rnd = rrp.Round;
                double side = svgData.Width;
                if (side > svgData.Height) side = svgData.Height;
                double offs = Math.Min(side, 100 * rrp.Page.Zoom) * rnd;
                double tmp = offs * (1 - pdfCKT);
                double x1 = svgData.X;
                double y1 = svgData.Y;
                double x2 = svgData.X + svgData.Width;
                double y2 = svgData.Y + svgData.Height;

                strokePath.Append(string.Format("M {0} {1} ",
                    ToUnits(x1 + offs), ToUnits(y2)));
                strokePath.Append(string.Format("C {0} {1} {2} {3} {4} {5} ",
                    ToUnits(x1 + tmp), ToUnits(y2), ToUnits(x1), ToUnits(y2 - tmp), ToUnits(x1), ToUnits(y2 - offs)));
                strokePath.Append(string.Format("V {0} ", ToUnits(y1 + offs)));
                strokePath.Append(string.Format("C {0} {1} {2} {3} {4} {5} ",
                    ToUnits(x1), ToUnits(y1 + tmp), ToUnits(x1 + tmp), ToUnits(y1), ToUnits(x1 + offs), ToUnits(y1)));
                strokePath.Append(string.Format("H {0} ", ToUnits(x2 - offs)));
                strokePath.Append(string.Format("C {0} {1} {2} {3} {4} {5} ",
                    ToUnits(x2 - tmp), ToUnits(y1), ToUnits(x2), ToUnits(y1 + tmp), ToUnits(x2), ToUnits(y1 + offs)));
                strokePath.Append(string.Format("V {0} ", ToUnits(y2 - offs)));
                strokePath.Append(string.Format("C {0} {1} {2} {3} {4} {5} Z",
                    ToUnits(x2), ToUnits(y2 - tmp), ToUnits(x2 - tmp), ToUnits(y2), ToUnits(x2 - offs), ToUnits(y2)));

                writer.WriteStartElement("path");
                writer.WriteAttributeString("fill", "none");
                WriteStrokeInfo(writer, rrp.Color, rrp.Size, rrp.Style);
                writer.WriteAttributeString("d", strokePath.ToString());
                writer.WriteEndElement();
                #endregion
            }
        }

        private static string GetClipPathName(StiComponent component, Hashtable guids)
        {
            if (guids != null)
            {
                string guid = (string)guids[component];
                if (guid == null)
                {
                    guid = global::System.Guid.NewGuid().ToString("N");
                    guids[component] = guid;
                }
                return string.Format("clipPath_{0}", guid);
            }
            if (component.Guid == null) component.NewGuid();
            return string.Format("clipPath_{0}", component.Guid);
        }
        #endregion

        #region SaveComponentToString
        public static string SaveComponentToString(StiComponent component)
        {
            return SaveComponentToString(component, 1);
        }

        public static string SaveComponentToString(StiComponent component, double zoom)
        {
            return SaveComponentToString(component, ImageFormat.Png, 0.75f, (float)zoom * 100);
        }

        internal static string SaveComponentToString(StiComponent component, ImageFormat imageFormat, float imageQuality, float imageResolution)
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            var xmlIndentation = -1;
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);

            if (component.Report == null)
                return string.Empty;

            var unit = component.Report.Unit;
            if (component.Page != null)
                unit = component.Page.Unit;

            #region Prepare data
            double htmlScaleX = StiMatrix.htmlScaleX;
            double htmlScaleY = StiMatrix.htmlScaleY;
            if (component.IsDesigning)
            {
                htmlScaleX = 1;
                htmlScaleY = 1;
            }

            double x1 = htmlScaleX * unit.ConvertToHInches(component.Left);
            double y1 = htmlScaleY * unit.ConvertToHInches(component.Top);
            double x2 = htmlScaleX * unit.ConvertToHInches(component.Right);
            double y2 = htmlScaleY * unit.ConvertToHInches(component.Bottom);

            var pp = new StiSvgData();
            pp.X = 0;
            pp.Y = 0;
            pp.Width = Math.Round(x2 - x1, 0);
            pp.Height = Math.Round(y2 - y1, 0);
            pp.Component = component;
            #endregion

            var isImage = component.IsExportAsImage(StiExportFormat.ImageSvg);
            var isShape = CheckShape(component);

            var gradientCounter = 1;

            Hashtable guids = new Hashtable();

            #region Write defs
            var stiText = component as StiText;
            if (stiText != null && !isImage && stiText.Text.ToString() != null && stiText.Text.ToString().Trim().Length > 0)
            {
                writer.WriteStartElement("defs");
                writer.WriteStartElement("clipPath");
                writer.WriteStartElement("rect");
                WriteCoordinates(writer, pp);
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            #endregion

            if (!isShape && !isImage)
                WriteBorder1(writer, pp, ref gradientCounter);

            if ((component is StiText) && !isImage)
                WriteText(writer, pp, xmlIndentation, true, guids);

            else if (component is StiBarCode)
                WriteBarCode(writer, pp);

            else if (component is StiSparkline)
                StiSparklineSvgHelper.WriteSparkline(writer, pp);

            else if (component is StiCheckBox)
                WriteCheckBox(writer, pp);

            else if (component is StiChart)
                StiChartSvgHelper.WriteChart(writer, pp, false);

            else if (component is StiGauge)
                StiGaugeSvgHelper.WriteGauge(writer, pp, false);

            else if (component is StiMap && ((StiMap)component).MapMode == StiMapMode.Choropleth)
                StiMapSvgHelper.DrawMap(writer, component as StiMap, 0, 0, pp.Width, pp.Height, false);

            else if (component is IStiProgressElement)
                StiProgressElementSvgHelper.WriteProgress(writer, pp, 1f);

            else if (component is IStiIndicatorElement)
                StiIndicatorElementSvgHelper.WriteIndicator(writer, pp, 1f);

            else if (component is IStiCardsElement)
                StiCardsElementSvgHelper.WriteCards(writer, pp, 1f);

            else if (component is StiMathFormula mathFormula && string.IsNullOrEmpty(mathFormula.LaTexExpression))
                StiMathFormulaSvgHelper.WriteEmptyDataMessage(writer, pp);

            else if (isShape)
                WriteShape(writer, pp, xmlIndentation, true, guids);

            else if (isImage)
            {
                WriteImage(writer, pp, imageResolution / 100f, imageFormat, imageQuality, null, guids);
            }

            //if (component is StiRectanglePrimitive)
            //{
            //    WriteRoundedRectanglePrimitive(writer, pp);
            //}
            //else
            //{
            //    WriteBorder2(writer, pp);
            //}

            writer.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            var sr = new StreamReader(ms);
            var st = sr.ReadToEnd();
            sr.Close();
            ms.Close();

            StiExportUtils.EnableFontSmoothing(component.Report);

            return st;
        }
        #endregion

        #region Method SaveToStream
        public static void SaveToStream(StiReport report, StiPage page, Stream stream, bool compressed)
        {
            int tempInt = 0;
            SaveToStream(report, page, stream, compressed, true, ref tempInt, ImageFormat.Png, 0.75f, 100);
        }

        internal static void SaveToStream(StiReport report, StiPage page, Stream stream, bool compressed, bool standalone, ref int clipCounter, ImageFormat imageFormat, float imageQuality, float imageResolution)
        {
            if (compressed)
            {
                GZipStream gzip = new GZipStream(stream, CompressionMode.Compress, true);
                WriteDocument(report, page, standalone, imageFormat, imageQuality, imageResolution).WriteTo(gzip);
                gzip.Close();
            }
            else
            {
                WriteDocument(report, page, standalone, imageFormat, imageQuality, imageResolution).WriteTo(stream);
            }
        }
        #endregion

        #region Render CheckBoxes
        public static void WriteCheckBox(XmlTextWriter writer, StiSvgData svgData)
        {
            StiCheckBox checkBox = svgData.Component as StiCheckBox;

            if (checkBox == null) return;

            var fill = StiContextSvgHelper.WriteFillBrush(writer, checkBox.TextBrush, new RectangleF((float)svgData.X, (float)svgData.Y, (float)svgData.Width, (float)svgData.Height));

            writer.WriteStartElement("g");

            int checkBoxSize = 200;
            var scaleCheckBox = 1 / (checkBoxSize / Math.Min(svgData.Width, svgData.Height));

            var offsetX = 0d;
            var offsetY = 0d;

            if (svgData.Width > svgData.Height)
            {
                offsetX = Math.Abs(svgData.Width / 2 - checkBoxSize * scaleCheckBox / 2);
            }
            else if (svgData.Width < svgData.Height)
            {
                offsetY = Math.Abs(svgData.Height / 2 - checkBoxSize * scaleCheckBox / 2);
            }

            writer.WriteAttributeString("transform", string.Format("translate({0},{1})",
                (svgData.X + offsetX).ToString().Replace(",", "."),
                (svgData.Y + offsetY).ToString().Replace(",", ".")));

            writer.WriteStartElement("path");

            writer.WriteAttributeString("d", GetCheckBoxData(checkBox));

            writer.WriteAttributeString("stroke", string.Format("#{0:X2}{1:X2}{2:X2}", checkBox.ContourColor.R, checkBox.ContourColor.G, checkBox.ContourColor.B));
            writer.WriteAttributeString("stroke-width", checkBox.Size.ToString());
            writer.WriteAttributeString("stroke-linecap", "round");
            writer.WriteAttributeString("stroke-linejoin", "round");
            writer.WriteAttributeString("transform", string.Format("scale({0})", scaleCheckBox).Replace(",", "."));
            writer.WriteAttributeString("style", fill);

            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        private static string GetCheckBoxData(StiCheckBox checkbox)
        {
            bool checkBoxValue = false;
            var value = checkbox.CheckedValue ?? checkbox.Checked.Value;

            string checkedValueStr = value.ToString().Trim().ToLower(CultureInfo.InvariantCulture);
            string[] strs = checkbox.Values.Split(new char[] { '/', ';', ',' });
            if (strs != null && strs.Length > 0)
            {
                string firstValue = strs[0].Trim().ToLower(CultureInfo.InvariantCulture);
                checkBoxValue = checkedValueStr == firstValue;
            }

            string shape = string.Empty;
            switch (checkBoxValue ? checkbox.CheckStyleForTrue : checkbox.CheckStyleForFalse)
            {
                case StiCheckStyle.Cross:
                    shape = "m 62.567796,147.97593 c -0.55,-0.14223 -2.162828,-0.5128 -3.584062,-0.82348 -3.647667,-0.79738 -9.670499,-5.83775 -14.242817,-11.91949 l " +
                        "-3.902341,-5.19058 5.080199,-1.13481 c 7.353071,-1.64253 13.640456,-5.71752 21.826811,-14.14646 l 7.208128,-7.42171 " +
                        "-6.410736,-7.513354 c -11.773129,-13.79803 -14.346726,-23.01954 -8.627769,-30.91434 2.894109,-3.9952 11.818482,-12.369333 " +
                        "13.182086,-12.369333 0.411356,0 1.063049,1.6875 1.448207,3.750003 0.980474,5.25038 6.456187,16.76587 10.936694,23 2.075266,2.8875 " +
                        "3.991125,5.25 4.257464,5.25 0.266339,0 3.775242,-3.4875 7.797566,-7.75 16.397034,-17.37615 29.674184,-19.76481 38.280564,-6.88699 " +
                        "4.15523,6.21753 4.18631,8.07093 0.14012,8.3552 -5.84833,0.41088 -17.16241,8.5342 -25.51465,18.319104 l -4.63153,5.42599 " +
                        "4.87803,4.31529 c 6.55108,5.79533 18.8991,11.89272 25.84076,12.76002 3.0455,0.38051 5.53727,1.10582 5.53727,1.6118 0,2.7809 " +
                        "-9.26611,14.41872 -13.03,16.36511 -7.96116,4.11687 -16.36991,0.71207 -32.764584,-13.26677 l -4.985957,-4.25125 -7.086791,8.97188 c " +
                        "-3.897736,4.93454 -8.82141,10.1198 -10.9415,11.52281 -3.906121,2.58495 -8.86588,4.41339 -10.691162,3.94136 z";
                    break;

                case StiCheckStyle.Check:
                    shape = "M 60.972125,162.49704 C 51.172676,136.72254 43.561975,123.37669 35.370344,117.6027 l -4.45827,-3.14248 2.75159,-2.89559 c 3.875121,-4.07793 " +
                        "10.034743,-7.49924 14.902472,-8.27747 3.859874,-0.61709 4.458306,-0.38024 8.535897,3.37835 2.660692,2.45254 6.265525,7.60856 9.167226,13.11196 " +
                        "2.630218,4.98849 4.910542,9.06999 5.067388,9.06999 0.156846,0 2.31372,-3.0375 4.793052,-6.75 C 96.259164,91.956015 129.68299,58.786374 157.56485,41.281603 l " +
                        "8.84913,-5.555656 2.2633,2.631238 2.26329,2.631237 -7.76266,6.294183 C 139.859,66.19023 108.01682,105.51363 89.042715,138.83563 c -6.680477,11.73214 " +
                        "-7.172359,12.31296 -15.090788,17.81963 -4.501873,3.13071 -9.044031,6.30443 -10.093684,7.05271 -1.708923,1.21826 -2.010678,1.09165 -2.886118,-1.21093 z";
                    break;

                case StiCheckStyle.CrossRectangle:
                    shape = "m 24.152542,102.04237 0,-72.499996 74.5,0 74.499998,0 0,72.499996 0,72.5 -74.499998,0 -74.5,0 0,-72.5 z m 133.758188,0.25 -0.25819,-57.249996 " +
                        "-58.999998,0 -59,0 -0.259695,55.999996 c -0.142833,30.8 -0.04446,56.5625 0.218615,57.25 0.375181,0.98048 13.207991,1.25 59.517885,1.25 l " +
                        "59.039573,0 -0.25819,-57.25 z m -90.574091,43.18692 c -1.823747,-0.3912 -4.926397,-1.85716 -6.894778,-3.25768 -3.319254,-2.36169 -12.289319,-12.40741 " +
                        "-12.289319,-13.76302 0,-0.32888 2.417494,-1.13897 5.372209,-1.80021 7.185193,-1.60797 13.747505,-5.93496 21.803114,-14.3763 l 6.675323,-6.99496 " +
                        "-6.379078,-7.31436 C 64.931387,85.71231 61.643682,76.29465 65.471903,68.89169 67.054097,65.83207 78.56175,54.542374 80.098251,54.542374 c 0.45744,0 " +
                        "1.146839,1.6875 1.531997,3.75 0.980474,5.250386 6.456187,16.765876 10.936694,22.999996 2.075266,2.8875 3.991125,5.25 4.257464,5.25 0.266339,0 " +
                        "3.775244,-3.4875 7.797564,-7.75 16.39704,-17.376139 29.67419,-19.764806 38.28057,-6.88698 4.15523,6.21752 4.18631,8.07092 0.14012,8.35519 -5.82996,0.40959 " +
                        "-18.23707,9.34942 -25.91566,18.67328 -3.90068,4.73647 -3.97203,4.95414 -2.2514,6.86861 3.19054,3.54997 13.7039,10.54321 18.97191,12.61967 2.83427,1.11716 " +
                        "7.43737,2.33421 10.22912,2.70455 2.79175,0.37034 5.07591,0.9956 5.07591,1.38947 0,2.11419 -8.37504,13.20895 -11.6517,15.4355 -8.39423,5.70403 " +
                        "-16.63203,2.77 -34.14289,-12.16054 l -4.985955,-4.25125 -7.086791,8.97188 c -9.722344,12.3085 -16.524852,16.55998 -23.948565,14.96754 z";
                    break;

                case StiCheckStyle.CheckRectangle:
                    shape = "m 19.915254,103.5 0,-72.5 71.942245,0 71.942241,0 6.55727,-4.11139 6.55726,-4.11139 1.96722,2.36139 c 1.08197,1.298765 1.98219,2.644166 2.00049,2.98978 " +
                        "0.0183,0.345615 -2.44173,2.53784 -5.46673,4.87161 l -5.5,4.243219 0,69.378391 0,69.37839 -74.999991,0 -75.000005,0 0,-72.5 z m 133.999996,3.87756 c " +
                        "0,-49.33933 -0.12953,-53.514947 -1.62169,-52.276568 -2.78014,2.307312 -15.68408,17.90053 -24.32871,29.399008 -10.4919,13.955575 -23.47926,33.53736 " +
                        "-29.514025,44.5 -4.457326,8.09707 -5.134776,8.80812 -14.291256,15 -5.28667,3.575 -9.903486,6.62471 -10.259592,6.77712 -0.356107,0.15242 -1.912439,-2.99758 " +
                        "-3.458515,-7 -1.546077,-4.00241 -5.258394,-12.41205 -8.249593,-18.68809 -4.285436,-8.99155 -6.676569,-12.64898 -11.27758,-17.25 C 47.70282,104.62757 " +
                        "44.364254,102 43.495254,102 c -2.798369,0 -1.704872,-1.66044 3.983717,-6.049158 5.593548,-4.31539 13.183139,-7.091307 16.801313,-6.145133 3.559412,0.930807 " +
                        "9.408491,8.154973 13.919775,17.192241 l 4.46286,8.94025 4.54378,-6.83321 C 95.518219,96.605618 108.21371,81.688517 125.80695,63.75 L 143.21531,46 l " +
                        "-53.650021,0 -53.650035,0 0,57.5 0,57.5 59.000005,0 58.999991,0 0,-53.62244 z";
                    break;

                case StiCheckStyle.CrossCircle:
                    shape = "M 83.347458,173.13597 C 61.069754,168.04956 42.193415,152.8724 32.202285,132.01368 23.4014,113.63986 23.679644,89.965903 32.91889,71.042373 " +
                        "41.881579,52.685283 60.867647,37.139882 80.847458,31.799452 c 10.235111,-2.735756 31.264662,-2.427393 40.964762,0.600679 26.18668,8.174684 " +
                        "46.06876,28.926852 51.62012,53.879155 2.43666,10.952327 1.56754,28.058524 -1.98036,38.977594 -6.65679,20.48707 -25.64801,38.95163 -47.32647,46.01402 " +
                        "-6.3909,2.08202 -10.18566,2.59644 -21.27805,2.88446 -9.033911,0.23456 -15.484931,-0.10267 -19.500002,-1.01939 z M 112.4138,158.45825 c 17.13137,-3.13002 " +
                        "33.71724,-15.96081 41.41353,-32.03742 14.8975,-31.119027 -1.10807,-67.659584 -34.40232,-78.540141 -6.71328,-2.193899 -9.93541,-2.643501 " +
                        "-19.07755,-2.661999 -9.354252,-0.01893 -12.16228,0.37753 -18.768532,2.649866 -17.155451,5.900919 -29.669426,17.531424 -36.438658,33.866137 " +
                        "-2.152301,5.193678 -2.694658,8.35455 -3.070923,17.89744 -0.518057,13.139047 0.741843,19.201887 6.111644,29.410237 4.106815,7.80733 15.431893,19.09359 " +
                        "23.36818,23.28808 12.061362,6.37467 27.138828,8.6356 40.864629,6.1278 z M 69.097458,133.41654 c -2.8875,-2.75881 -5.25,-5.35869 -5.25,-5.77751 " +
                        "0,-0.41882 5.658529,-6.30954 12.57451,-13.0905 l 12.57451,-12.329 L 76.198053,89.392633 63.399628,76.565738 68.335951,71.554056 c 2.714978,-2.756426 " +
                        "5.304859,-5.011683 5.75529,-5.011683 0.450432,0 6.574351,5.611554 13.608709,12.470121 l 12.78974,12.470119 4.42889,-4.553471 c 2.43588,-2.50441 " +
                        "8.39186,-8.187924 13.23551,-12.630032 l 8.80663,-8.076559 5.34744,5.281006 5.34743,5.281007 -12.96155,12.557899 -12.96154,12.557897 13.13318,13.16027 " +
                        "13.13319,13.16027 -5.18386,4.66074 c -2.85112,2.5634 -5.70472,4.66073 -6.34134,4.66073 -0.63661,0 -6.5434,-5.4 -13.12621,-12 -6.58281,-6.6 -12.3871,-12 " +
                        "-12.89844,-12 -0.511329,0 -6.593363,5.60029 -13.515627,12.44509 l -12.585935,12.44508 -5.25,-5.016 z";
                    break;

                case StiCheckStyle.DotCircle:
                    shape = "M 81.652542,170.5936 C 59.374838,165.50719 40.498499,150.33003 30.507369,129.47131 21.706484,111.09749 21.984728,87.42353 31.223974,68.5 " +
                        "40.186663,50.14291 59.172731,34.597509 79.152542,29.257079 89.387653,26.521323 110.4172,26.829686 120.1173,29.857758 c 26.18668,8.174684 " +
                        "46.06876,28.926852 51.62012,53.879152 2.43666,10.95233 1.56754,28.05853 -1.98036,38.9776 -6.65679,20.48707 -25.64801,38.95163 -47.32647,46.01402 " +
                        "-6.3909,2.08202 -10.18566,2.59644 -21.27805,2.88446 -9.033907,0.23456 -15.484927,-0.10267 -19.499998,-1.01939 z m 29.999998,-15.098 c 20.68862,-4.34363 " +
                        "38.01874,-20.45437 44.09844,-40.9956 2.36228,-7.9813 2.36228,-22.0187 0,-30 C 150.08927,65.371023 134.63549,50.297336 114.65254,44.412396 " +
                        "106.5531,42.027127 90.741304,42.026386 82.695253,44.4109 62.460276,50.407701 46.686742,66.039241 41.6053,85.13096 c -1.948821,7.32201 -1.86506,23.11641 " +
                        "0.158766,29.93754 8.730326,29.42481 38.97193,46.91812 69.888474,40.4271 z M 90.004747,122.6703 C 76.550209,117.63801 69.825047,101.82445 " +
                        "75.898143,89.5 c 2.136718,-4.33615 7.147144,-9.356192 11.754399,-11.776953 5.578622,-2.931141 16.413098,-2.927504 22.052908,0.0074 18.03,9.382663 " +
                        "19.07573,32.784373 1.91442,42.841563 -5.57282,3.26589 -15.830952,4.2617 -21.615123,2.09829 z";
                    break;

                case StiCheckStyle.DotRectangle:
                    shape = "m 23.847458,101.19491 0,-72.499995 74.5,0 74.499992,0 0,72.499995 0,72.5 -74.499992,0 -74.5,0 0,-72.5 z m 133.999992,-0.008 0,-57.507925 " +
                        "-59.249992,0.25793 -59.25,0.25793 -0.25819,57.249995 -0.258189,57.25 59.508189,0 59.508182,0 0,-57.50793 z m -94.320573,33.85402 c -0.37368,-0.37368 " +
                        "-0.679419,-15.67942 -0.679419,-34.01275 l 0,-33.333335 35.513302,0 35.51329,0 -0.2633,33.749995 -0.2633,33.75 -34.570573,0.26275 c -19.013819,0.14452 " +
                        "-34.876319,-0.043 -35.25,-0.41666 z";
                    break;

                case StiCheckStyle.NoneCircle:
                    shape = "M 83.5,170.5936 C 61.222296,165.50719 42.345957,150.33003 32.354827,129.47131 23.553942,111.09749 23.832186,87.423523 33.071432,68.5 " +
                        "42.034121,50.14291 61.020189,34.597509 81,29.257079 c 10.235111,-2.735756 31.26466,-2.427393 40.96476,0.600679 26.18668,8.174684 46.06876,28.926852 " +
                        "51.62012,53.879155 2.43666,10.95232 1.56754,28.058527 -1.98036,38.977597 -6.65679,20.48707 -25.64801,38.95163 -47.32647,46.01402 -6.3909,2.08202 " +
                        "-10.18566,2.59644 -21.27805,2.88446 -9.033909,0.23456 -15.484929,-0.10267 -19.5,-1.01939 z m 30,-15.098 c 20.68862,-4.34363 38.01874,-20.45437 " +
                        "44.09844,-40.9956 2.36228,-7.9813 2.36228,-22.018707 0,-29.999997 C 151.93673,65.371023 136.48295,50.297336 116.5,44.412396 108.40056,42.027127 " +
                        "92.588762,42.026386 84.542711,44.410896 64.307734,50.407697 48.5342,66.039237 43.452758,85.130959 c -1.948821,7.322 -1.86506,23.116411 " +
                        "0.158766,29.937541 8.730326,29.42481 38.97193,46.91812 69.888476,40.4271 z";
                    break;

                case StiCheckStyle.NoneRectangle:
                    shape = "m 24.152542,102.04237 0,-72.499997 74.5,0 74.500008,0 0,72.499997 0,72.5 -74.500008,0 -74.5,0 0,-72.5 z m 133.758198,0.25 " +
                        "-0.25819,-57.249997 -59.000008,0 -59,0 -0.259695,55.999997 c -0.142833,30.8 -0.04446,56.5625 0.218615,57.25 0.375181,0.98048 " +
                        "13.207991,1.25 59.517885,1.25 l 59.039583,0 -0.25819,-57.25 z";
                    break;
            }

            return shape;
        }

        public static string RenderCheckBox(StiCheckStyle style, Color contourColor, double contourSize, StiBrush textBrush, double width)
        {
            Color backColor = Color.Transparent;
            if (textBrush is StiSolidBrush) backColor = ((StiSolidBrush)textBrush).Color;
            else if (textBrush is StiGradientBrush) backColor = ((StiGradientBrush)textBrush).StartColor;
            else if (textBrush is StiGlareBrush) backColor = ((StiGlareBrush)textBrush).StartColor;
            else if (textBrush is StiGlassBrush) backColor = ((StiGlassBrush)textBrush).Color;
            else if (textBrush is StiHatchBrush) backColor = ((StiHatchBrush)textBrush).ForeColor;

            string head = string.Format(
                "<svg version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" x=\"0\" y=\"0\" width=\"{0}px\" height=\"{0}px\">",
                width);

            string path = string.Format(
                "<path stroke=\"#{0:X2}{1:X2}{2:X2}\" stroke-width=\"{3}\" fill=\"#{4:X2}{5:X2}{6:X2}\" stroke-linecap=\"round\" stroke-linejoin=\"round\" transform=\"scale({7})\" d=\"",
                contourColor.R, contourColor.G, contourColor.B,
                contourSize,
                backColor.R, backColor.G, backColor.B,
                1 / (200 / width));

            string shape = string.Empty;
            switch (style)
            {
                case StiCheckStyle.Cross:
                    shape = "m 62.567796,147.97593 c -0.55,-0.14223 -2.162828,-0.5128 -3.584062,-0.82348 -3.647667,-0.79738 -9.670499,-5.83775 -14.242817,-11.91949 l " +
                        "-3.902341,-5.19058 5.080199,-1.13481 c 7.353071,-1.64253 13.640456,-5.71752 21.826811,-14.14646 l 7.208128,-7.42171 " +
                        "-6.410736,-7.513354 c -11.773129,-13.79803 -14.346726,-23.01954 -8.627769,-30.91434 2.894109,-3.9952 11.818482,-12.369333 " +
                        "13.182086,-12.369333 0.411356,0 1.063049,1.6875 1.448207,3.750003 0.980474,5.25038 6.456187,16.76587 10.936694,23 2.075266,2.8875 " +
                        "3.991125,5.25 4.257464,5.25 0.266339,0 3.775242,-3.4875 7.797566,-7.75 16.397034,-17.37615 29.674184,-19.76481 38.280564,-6.88699 " +
                        "4.15523,6.21753 4.18631,8.07093 0.14012,8.3552 -5.84833,0.41088 -17.16241,8.5342 -25.51465,18.319104 l -4.63153,5.42599 " +
                        "4.87803,4.31529 c 6.55108,5.79533 18.8991,11.89272 25.84076,12.76002 3.0455,0.38051 5.53727,1.10582 5.53727,1.6118 0,2.7809 " +
                        "-9.26611,14.41872 -13.03,16.36511 -7.96116,4.11687 -16.36991,0.71207 -32.764584,-13.26677 l -4.985957,-4.25125 -7.086791,8.97188 c " +
                        "-3.897736,4.93454 -8.82141,10.1198 -10.9415,11.52281 -3.906121,2.58495 -8.86588,4.41339 -10.691162,3.94136 z";
                    break;

                case StiCheckStyle.Check:
                    shape = "M 60.972125,162.49704 C 51.172676,136.72254 43.561975,123.37669 35.370344,117.6027 l -4.45827,-3.14248 2.75159,-2.89559 c 3.875121,-4.07793 " +
                        "10.034743,-7.49924 14.902472,-8.27747 3.859874,-0.61709 4.458306,-0.38024 8.535897,3.37835 2.660692,2.45254 6.265525,7.60856 9.167226,13.11196 " +
                        "2.630218,4.98849 4.910542,9.06999 5.067388,9.06999 0.156846,0 2.31372,-3.0375 4.793052,-6.75 C 96.259164,91.956015 129.68299,58.786374 157.56485,41.281603 l " +
                        "8.84913,-5.555656 2.2633,2.631238 2.26329,2.631237 -7.76266,6.294183 C 139.859,66.19023 108.01682,105.51363 89.042715,138.83563 c -6.680477,11.73214 " +
                        "-7.172359,12.31296 -15.090788,17.81963 -4.501873,3.13071 -9.044031,6.30443 -10.093684,7.05271 -1.708923,1.21826 -2.010678,1.09165 -2.886118,-1.21093 z";
                    break;

                case StiCheckStyle.CrossRectangle:
                    shape = "m 24.152542,102.04237 0,-72.499996 74.5,0 74.499998,0 0,72.499996 0,72.5 -74.499998,0 -74.5,0 0,-72.5 z m 133.758188,0.25 -0.25819,-57.249996 " +
                        "-58.999998,0 -59,0 -0.259695,55.999996 c -0.142833,30.8 -0.04446,56.5625 0.218615,57.25 0.375181,0.98048 13.207991,1.25 59.517885,1.25 l " +
                        "59.039573,0 -0.25819,-57.25 z m -90.574091,43.18692 c -1.823747,-0.3912 -4.926397,-1.85716 -6.894778,-3.25768 -3.319254,-2.36169 -12.289319,-12.40741 " +
                        "-12.289319,-13.76302 0,-0.32888 2.417494,-1.13897 5.372209,-1.80021 7.185193,-1.60797 13.747505,-5.93496 21.803114,-14.3763 l 6.675323,-6.99496 " +
                        "-6.379078,-7.31436 C 64.931387,85.71231 61.643682,76.29465 65.471903,68.89169 67.054097,65.83207 78.56175,54.542374 80.098251,54.542374 c 0.45744,0 " +
                        "1.146839,1.6875 1.531997,3.75 0.980474,5.250386 6.456187,16.765876 10.936694,22.999996 2.075266,2.8875 3.991125,5.25 4.257464,5.25 0.266339,0 " +
                        "3.775244,-3.4875 7.797564,-7.75 16.39704,-17.376139 29.67419,-19.764806 38.28057,-6.88698 4.15523,6.21752 4.18631,8.07092 0.14012,8.35519 -5.82996,0.40959 " +
                        "-18.23707,9.34942 -25.91566,18.67328 -3.90068,4.73647 -3.97203,4.95414 -2.2514,6.86861 3.19054,3.54997 13.7039,10.54321 18.97191,12.61967 2.83427,1.11716 " +
                        "7.43737,2.33421 10.22912,2.70455 2.79175,0.37034 5.07591,0.9956 5.07591,1.38947 0,2.11419 -8.37504,13.20895 -11.6517,15.4355 -8.39423,5.70403 " +
                        "-16.63203,2.77 -34.14289,-12.16054 l -4.985955,-4.25125 -7.086791,8.97188 c -9.722344,12.3085 -16.524852,16.55998 -23.948565,14.96754 z";
                    break;

                case StiCheckStyle.CheckRectangle:
                    shape = "m 19.915254,103.5 0,-72.5 71.942245,0 71.942241,0 6.55727,-4.11139 6.55726,-4.11139 1.96722,2.36139 c 1.08197,1.298765 1.98219,2.644166 2.00049,2.98978 " +
                        "0.0183,0.345615 -2.44173,2.53784 -5.46673,4.87161 l -5.5,4.243219 0,69.378391 0,69.37839 -74.999991,0 -75.000005,0 0,-72.5 z m 133.999996,3.87756 c " +
                        "0,-49.33933 -0.12953,-53.514947 -1.62169,-52.276568 -2.78014,2.307312 -15.68408,17.90053 -24.32871,29.399008 -10.4919,13.955575 -23.47926,33.53736 " +
                        "-29.514025,44.5 -4.457326,8.09707 -5.134776,8.80812 -14.291256,15 -5.28667,3.575 -9.903486,6.62471 -10.259592,6.77712 -0.356107,0.15242 -1.912439,-2.99758 " +
                        "-3.458515,-7 -1.546077,-4.00241 -5.258394,-12.41205 -8.249593,-18.68809 -4.285436,-8.99155 -6.676569,-12.64898 -11.27758,-17.25 C 47.70282,104.62757 " +
                        "44.364254,102 43.495254,102 c -2.798369,0 -1.704872,-1.66044 3.983717,-6.049158 5.593548,-4.31539 13.183139,-7.091307 16.801313,-6.145133 3.559412,0.930807 " +
                        "9.408491,8.154973 13.919775,17.192241 l 4.46286,8.94025 4.54378,-6.83321 C 95.518219,96.605618 108.21371,81.688517 125.80695,63.75 L 143.21531,46 l " +
                        "-53.650021,0 -53.650035,0 0,57.5 0,57.5 59.000005,0 58.999991,0 0,-53.62244 z";
                    break;

                case StiCheckStyle.CrossCircle:
                    shape = "M 83.347458,173.13597 C 61.069754,168.04956 42.193415,152.8724 32.202285,132.01368 23.4014,113.63986 23.679644,89.965903 32.91889,71.042373 " +
                        "41.881579,52.685283 60.867647,37.139882 80.847458,31.799452 c 10.235111,-2.735756 31.264662,-2.427393 40.964762,0.600679 26.18668,8.174684 " +
                        "46.06876,28.926852 51.62012,53.879155 2.43666,10.952327 1.56754,28.058524 -1.98036,38.977594 -6.65679,20.48707 -25.64801,38.95163 -47.32647,46.01402 " +
                        "-6.3909,2.08202 -10.18566,2.59644 -21.27805,2.88446 -9.033911,0.23456 -15.484931,-0.10267 -19.500002,-1.01939 z M 112.4138,158.45825 c 17.13137,-3.13002 " +
                        "33.71724,-15.96081 41.41353,-32.03742 14.8975,-31.119027 -1.10807,-67.659584 -34.40232,-78.540141 -6.71328,-2.193899 -9.93541,-2.643501 " +
                        "-19.07755,-2.661999 -9.354252,-0.01893 -12.16228,0.37753 -18.768532,2.649866 -17.155451,5.900919 -29.669426,17.531424 -36.438658,33.866137 " +
                        "-2.152301,5.193678 -2.694658,8.35455 -3.070923,17.89744 -0.518057,13.139047 0.741843,19.201887 6.111644,29.410237 4.106815,7.80733 15.431893,19.09359 " +
                        "23.36818,23.28808 12.061362,6.37467 27.138828,8.6356 40.864629,6.1278 z M 69.097458,133.41654 c -2.8875,-2.75881 -5.25,-5.35869 -5.25,-5.77751 " +
                        "0,-0.41882 5.658529,-6.30954 12.57451,-13.0905 l 12.57451,-12.329 L 76.198053,89.392633 63.399628,76.565738 68.335951,71.554056 c 2.714978,-2.756426 " +
                        "5.304859,-5.011683 5.75529,-5.011683 0.450432,0 6.574351,5.611554 13.608709,12.470121 l 12.78974,12.470119 4.42889,-4.553471 c 2.43588,-2.50441 " +
                        "8.39186,-8.187924 13.23551,-12.630032 l 8.80663,-8.076559 5.34744,5.281006 5.34743,5.281007 -12.96155,12.557899 -12.96154,12.557897 13.13318,13.16027 " +
                        "13.13319,13.16027 -5.18386,4.66074 c -2.85112,2.5634 -5.70472,4.66073 -6.34134,4.66073 -0.63661,0 -6.5434,-5.4 -13.12621,-12 -6.58281,-6.6 -12.3871,-12 " +
                        "-12.89844,-12 -0.511329,0 -6.593363,5.60029 -13.515627,12.44509 l -12.585935,12.44508 -5.25,-5.016 z";
                    break;

                case StiCheckStyle.DotCircle:
                    shape = "M 81.652542,170.5936 C 59.374838,165.50719 40.498499,150.33003 30.507369,129.47131 21.706484,111.09749 21.984728,87.42353 31.223974,68.5 " +
                        "40.186663,50.14291 59.172731,34.597509 79.152542,29.257079 89.387653,26.521323 110.4172,26.829686 120.1173,29.857758 c 26.18668,8.174684 " +
                        "46.06876,28.926852 51.62012,53.879152 2.43666,10.95233 1.56754,28.05853 -1.98036,38.9776 -6.65679,20.48707 -25.64801,38.95163 -47.32647,46.01402 " +
                        "-6.3909,2.08202 -10.18566,2.59644 -21.27805,2.88446 -9.033907,0.23456 -15.484927,-0.10267 -19.499998,-1.01939 z m 29.999998,-15.098 c 20.68862,-4.34363 " +
                        "38.01874,-20.45437 44.09844,-40.9956 2.36228,-7.9813 2.36228,-22.0187 0,-30 C 150.08927,65.371023 134.63549,50.297336 114.65254,44.412396 " +
                        "106.5531,42.027127 90.741304,42.026386 82.695253,44.4109 62.460276,50.407701 46.686742,66.039241 41.6053,85.13096 c -1.948821,7.32201 -1.86506,23.11641 " +
                        "0.158766,29.93754 8.730326,29.42481 38.97193,46.91812 69.888474,40.4271 z M 90.004747,122.6703 C 76.550209,117.63801 69.825047,101.82445 " +
                        "75.898143,89.5 c 2.136718,-4.33615 7.147144,-9.356192 11.754399,-11.776953 5.578622,-2.931141 16.413098,-2.927504 22.052908,0.0074 18.03,9.382663 " +
                        "19.07573,32.784373 1.91442,42.841563 -5.57282,3.26589 -15.830952,4.2617 -21.615123,2.09829 z";
                    break;

                case StiCheckStyle.DotRectangle:
                    shape = "m 23.847458,101.19491 0,-72.499995 74.5,0 74.499992,0 0,72.499995 0,72.5 -74.499992,0 -74.5,0 0,-72.5 z m 133.999992,-0.008 0,-57.507925 " +
                        "-59.249992,0.25793 -59.25,0.25793 -0.25819,57.249995 -0.258189,57.25 59.508189,0 59.508182,0 0,-57.50793 z m -94.320573,33.85402 c -0.37368,-0.37368 " +
                        "-0.679419,-15.67942 -0.679419,-34.01275 l 0,-33.333335 35.513302,0 35.51329,0 -0.2633,33.749995 -0.2633,33.75 -34.570573,0.26275 c -19.013819,0.14452 " +
                        "-34.876319,-0.043 -35.25,-0.41666 z";
                    break;

                case StiCheckStyle.NoneCircle:
                    shape = "M 83.5,170.5936 C 61.222296,165.50719 42.345957,150.33003 32.354827,129.47131 23.553942,111.09749 23.832186,87.423523 33.071432,68.5 " +
                        "42.034121,50.14291 61.020189,34.597509 81,29.257079 c 10.235111,-2.735756 31.26466,-2.427393 40.96476,0.600679 26.18668,8.174684 46.06876,28.926852 " +
                        "51.62012,53.879155 2.43666,10.95232 1.56754,28.058527 -1.98036,38.977597 -6.65679,20.48707 -25.64801,38.95163 -47.32647,46.01402 -6.3909,2.08202 " +
                        "-10.18566,2.59644 -21.27805,2.88446 -9.033909,0.23456 -15.484929,-0.10267 -19.5,-1.01939 z m 30,-15.098 c 20.68862,-4.34363 38.01874,-20.45437 " +
                        "44.09844,-40.9956 2.36228,-7.9813 2.36228,-22.018707 0,-29.999997 C 151.93673,65.371023 136.48295,50.297336 116.5,44.412396 108.40056,42.027127 " +
                        "92.588762,42.026386 84.542711,44.410896 64.307734,50.407697 48.5342,66.039237 43.452758,85.130959 c -1.948821,7.322 -1.86506,23.116411 " +
                        "0.158766,29.937541 8.730326,29.42481 38.97193,46.91812 69.888476,40.4271 z";
                    break;

                case StiCheckStyle.NoneRectangle:
                    shape = "m 24.152542,102.04237 0,-72.499997 74.5,0 74.500008,0 0,72.499997 0,72.5 -74.500008,0 -74.5,0 0,-72.5 z m 133.758198,0.25 " +
                        "-0.25819,-57.249997 -59.000008,0 -59,0 -0.259695,55.999997 c -0.142833,30.8 -0.04446,56.5625 0.218615,57.25 0.375181,0.98048 " +
                        "13.207991,1.25 59.517885,1.25 l 59.039583,0 -0.25819,-57.25 z";
                    break;
            }

            return string.Format("{0}{1}{2}\" /></svg>", head, path, shape);
        }

        #endregion

        #region Render Shapes
        public static string RenderShapeAsBase64(StiComponent component)
        {
            var bytes = RenderShape(component).ToArray();
            var st = "data:image/svg+xml;base64," + Convert.ToBase64String(bytes);
            return st;
        }

        public static MemoryStream RenderShape(StiComponent component)
        {
            StiShape shape = component as StiShape;
            if (shape == null || !CheckShape(component)) return null;

            MemoryStream ms = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8) { Indentation = 0, Formatting = Formatting.None };

            writer.WriteStartDocument();
            writer.WriteStartElement("svg");
            writer.WriteAttributeString("xmlns", "http://www.w3.org/2000/svg");

            double hiToTwips = 1;
            double height = hiToTwips * component.Report.Unit.ConvertToHInches(component.Height);
            double width = hiToTwips * component.Report.Unit.ConvertToHInches(component.Width);
            int margin = (int)Math.Round(shape.Size / 2 + 0.5);

            writer.WriteAttributeString("height", ToUnits(height));
            writer.WriteAttributeString("width", ToUnits(width));
            writer.WriteAttributeString("viewBox", string.Format("{0} {1} {2} {3}", ToUnits(-margin), ToUnits(-margin), ToUnits(width + margin * 2), ToUnits(height + margin * 2)));

            StiSvgData pp = new StiSvgData();
            pp.X = 0;
            pp.Y = 0;
            pp.Width = width;
            pp.Height = height;
            pp.Component = component;

            WriteShape(writer, pp, -1, false, new Hashtable());

            writer.WriteFullEndElement();

            writer.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
        #endregion

        #region Render Icon
        public static string RenderIcon(StiImage image, float zoom, double baseWidth, double baseHeight)
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8) { Indentation = 0, Formatting = Formatting.None };

            RenderIconInternal(writer, true, image, zoom, new RectangleD(0, 0, baseWidth, baseHeight));

            var buf = ms.ToArray();
            string output = Encoding.UTF8.GetString(buf);
            int indexStart = output.IndexOf("<svg ");

            writer.Close();
            ms.Close();

            return output.Substring(indexStart);
        }

        private static void RenderIconInternal(XmlTextWriter writer, bool alone, StiImage image, float zoom, RectangleD baseRect)
        {
            #region Prepare data
            double width = baseRect.Width;
            double height = baseRect.Height;
            double x = baseRect.X;
            double y = baseRect.Y;

            if (!image.Margins.IsEmpty)
            {
                const double pxToPt = 0.75;
                int marginLeft = (int)Math.Truncate(image.Margins.Left * zoom * pxToPt);
                int marginRight = (int)Math.Truncate(image.Margins.Right * zoom * pxToPt);
                int marginTop = (int)Math.Truncate(image.Margins.Top * zoom * pxToPt);
                int marginBottom = (int)Math.Truncate(image.Margins.Bottom * zoom * pxToPt);
                width -= marginLeft + marginRight;
                height -= marginTop + marginBottom;
                x += marginLeft;
                y += marginTop;
            }

            double cx = x + width / 2;
            double cy = y + height / 2;
            double scaleX = Math.Round(width / 100, 4);
            double scaleY = Math.Round(height / 100, 4);
            if (image.AspectRatio)
            {
                scaleX = scaleY = Math.Min(scaleX, scaleY);
                double offsetMin = Math.Min(width / 2, height / 2);
                if (image.VertAlignment == StiVertAlignment.Top) cy = y + offsetMin;
                if (image.VertAlignment == StiVertAlignment.Bottom) cy = y + height - offsetMin;
                if (image.HorAlignment == StiHorAlignment.Left) cx = x + offsetMin;
                if (image.HorAlignment == StiHorAlignment.Right) cx = x + width - offsetMin;
            }

            string rotate = "";
            if (image.ImageRotation == StiImageRotation.Rotate90CW) rotate = " rotate(90)";
            if (image.ImageRotation == StiImageRotation.Rotate90CCW) rotate = " rotate(-90)";
            if (image.ImageRotation == StiImageRotation.Rotate180) rotate = " rotate(180)";
            if (image.ImageRotation == StiImageRotation.FlipHorizontal) scaleX = -scaleX;
            if (image.ImageRotation == StiImageRotation.FlipVertical) scaleY = -scaleY;

            string text = StiFontIconsHelper.GetContent(image.Icon);
            #endregion

            #region Write data
            if (alone)
            {
                //writer.WriteStartDocument();
                writer.WriteStartElement("svg");
                writer.WriteAttributeString("xmlns", "http://www.w3.org/2000/svg");
                writer.WriteAttributeString("width", ToUnits(baseRect.Width));
                writer.WriteAttributeString("height", ToUnits(baseRect.Height));
            }

            writer.WriteStartElement("g");
            writer.WriteAttributeString("transform", String.Format("translate({0}, {1}) scale({2}, {3}){4}", ToUnits(cx), ToUnits(cy), ToUnits(scaleX, 3), ToUnits(scaleY, 3), rotate));
            
            writer.WriteStartElement("text");
            writer.WriteAttributeString("font-size", "72");
            writer.WriteAttributeString("font-family", "Stimulsoft");
            writer.WriteAttributeString("text-anchor", "middle");
            writer.WriteAttributeString("style", "fill:" + StiHtmlExportService.FormatColor(image.IconColor) + ";dominant-baseline:middle;");
            writer.WriteString(text);
            writer.WriteEndElement();

            writer.WriteEndElement();   //g
            if (alone)
            {
                writer.WriteEndElement();   //svg
            }
            writer.Flush();
            #endregion
        }
        #endregion
    }
}
