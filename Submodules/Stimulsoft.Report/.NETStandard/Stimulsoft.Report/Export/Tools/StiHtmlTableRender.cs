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
using System.Collections;
using System.Text;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Helpers;
using System.Collections.Generic;
using Stimulsoft.Report.BarCodes;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;

#if STIDRAWING
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Export
{

    #region StiHtmlUnit
    public class StiHtmlUnit
    {
        public enum StiHtmlUnitType
        {
            Pixel,
            Point
        }

        private const double hiToPt = 0.716;   //must be 0.72, but it's correction of browsers calculation

        public double Value = 0;
        public StiHtmlUnitType UnitType = StiHtmlUnitType.Pixel;

        public override string ToString()
        {
            //if (UnitType == StiHtmlUnitType.Point)
            //{
            //    //return (Math.Round(Value, 1) * hiToPt).ToString() + "pt";
            //    return ((int)(Value * hiToPt * 10) / 10d).ToString() + "pt";
            //    //return (Value / 100d).ToString() + "in";
            //}
            return ((int)Value).ToString() + "px";
        }

        public static string ToPixelString(int value)
        {
            return ((int)value).ToString() + "px";
        }

        //public static StiHtmlUnit Pixel(int value)
        //{
        //    StiHtmlUnit unit = new StiHtmlUnit();
        //    unit.UnitType = StiHtmlUnitType.Pixel;
        //    unit.Value = value;
        //    return unit;
        //}

        public static StiHtmlUnit NewUnit(double value)
        {
            return NewUnit(value, StiHtmlUnitType.Pixel);
        }

        public static StiHtmlUnit NewUnit(double value, StiHtmlUnitType unitType)
        {
            StiHtmlUnit unit = new StiHtmlUnit();
            unit.UnitType = unitType;
            unit.Value = value;
            return unit;
        }

        public static StiHtmlUnit NewUnit(double value, bool usePoints)
        {
            return NewUnit(value, usePoints ? StiHtmlUnitType.Point : StiHtmlUnitType.Pixel);
        }

        public static bool IsNullOrZero(StiHtmlUnit unit)
        {
            return (unit == null) || (unit.Value == 0);
        }
    }
    #endregion

    #region StiHtmlHyperlink
    public class StiHtmlSvg
    {
        public string Text;
    }

    public class StiHtmlHyperlink
    {
        public string Text;
        public string ToolTip;
        public string NavigateUrl;
        public Hashtable Attributes;
        public Hashtable Style;
        public string ImageUrl;
        public string CssClass;
        public StiHtmlUnit Width;
        public StiHtmlUnit Height;
        public string OpenLinksTarget;
        public string Id;
        public StiHtmlUnit Margin;
        public StiImage ImageComp;
        public float Zoom;

        public StiHtmlHyperlink()
        {
            Attributes = new Hashtable();
            Style = new Hashtable();
        }
    }
    #endregion

    #region StiHtmlImage
    public class StiHtmlImage
    {
        public string ToolTip;
        public string ImageUrl;
        public StiHtmlUnit Width;
        public StiHtmlUnit Height;
        public StiHtmlUnit Margin;
        public StiImage ImageComp;
        public float Zoom;
        public bool UseImageSize;
    }
    #endregion

    #region StiHtmlRoundedRectangle
    public class StiHtmlRoundedRectangle
    {
        //public StiRoundedRectanglePrimitive Primitive;
        public double Width;
        public double Height;
        public bool FitToCell;
        public double Size;
        public string Style;
        public string Color;
        public string Round;
        //public float Zoom;
    }
    #endregion

    #region StiHtmlTableCell
    public class StiHtmlTableCell
    {
        public StiHtmlUnit Width;
        public StiHtmlUnit Height;
        public Hashtable Style;
        public int ColumnSpan;
        public int RowSpan;
        public string CssClass;
        public string Text;
        public string ToolTip;
        public ArrayList Controls;
        public string Id;

        // Fields for HTML viewer interactions
        public string Interaction;
        public string Collapsed;
        public string SortDirection;
        public string DataBandSort;
        public string PageGuid;
        public string PageIndex;
        public string ReportFile;
        public string ComponentIndex;
        public string Editable;
        public string DrillDownMode;

        public StiHtmlTableCell()
        {
            Style = new Hashtable();
            Controls = new ArrayList();
        }
    }
    #endregion

    #region StiHtmlTableRow
    public class StiHtmlTableRow
    {
        public Hashtable Style;
        public ArrayList Cells;
        public StiHtmlUnit Height;

        public StiHtmlTableRow()
        {
            Style = new Hashtable();
            Cells = new ArrayList();
        }
    }
    #endregion

    #region StiHtmlTable
    public class StiHtmlTable
    {
        public string BackImageUrl;
        public StiHtmlUnit Width;
        public StiBorder Border = null;
        public int BorderWidth;
        public int CellPadding;
        public int CellSpacing;
        public ArrayList Rows;
        public StiHorAlignment Align = StiHorAlignment.Left;
        public string Position = null;
        public StiHtmlExportService htmlExport;

        public const string MarginsKey = "padding";
        public const string PageBreakBeforeKey = "page-break-before";
        public const string VertAlignKey = "div:vertical-align";
        public const string HorAlignKey = "div:text-align";
        public const string WordwrapKey = "div:wordwrap";

        private const double dpi96 = 0.96;

        #region Methods.StringToUrl
        public static string StringToUrl(string input)
        {
            UTF8Encoding enc = new UTF8Encoding();
            byte[] buf = enc.GetBytes(input);
            StringBuilder output = new StringBuilder();
            foreach (byte byt in buf)
            {
                if ((byt <= 0x20) || (byt > 0x7f) || (wrongUrlSymbols.IndexOf((char)byt) != -1))
                {
                    if ((byt <= 0x20) || (byt > 0x7f)) output.Append(string.Format("%{0:x2}", byt));
                    if (byt == 0x22) output.Append("&quot;");
                    if (byt == 0x26) output.Append("&amp;");
                    if (byt == 0x3c) output.Append("&lt;");
                    //if (byt == 0x3e) output.Append("&gt;");
                }
                else
                {
                    output.Append((char)byt);
                }
            }
            return output.ToString();
        }
        //private static string wrongUrlSymbols   = "\x22\x26\x3c\x3e";
        private static string wrongUrlSymbols = "\x22\x26\x3c";
        #endregion

        #region Methods.RenderControl
        public void RenderControl(StiHtmlTextWriter writer, bool addPageBreaks)
        {
            WriteTableBegin(writer, addPageBreaks, false);

            foreach (StiHtmlTableRow row in Rows)
            {
                if (row.Style.ContainsKey(PageBreakBeforeKey))
                {
                    row.Style.Remove(PageBreakBeforeKey);
                    WriteTableEnd(writer, addPageBreaks);
                    writer.WriteLine();
                    WriteTableBegin(writer, addPageBreaks, true);
                }

                #region Write table row
                writer.WriteBeginTag("tr");
                if (StiOptions.Export.Html.UseExtendedStyle)
                {
                    writer.WriteAttribute("class", "sBaseStyleFix");
                }
                if (!StiHtmlUnit.IsNullOrZero(row.Height))
                {
                    row.Style["height"] = row.Height.ToString();
                }

                if (row.Style.Count > 0)
                {
                    writer.Write(" style=\"");
                    foreach (DictionaryEntry de in row.Style)
                    {
                        writer.WriteStyleAttribute((string)de.Key, (string)de.Value);
                    }
                    writer.Write("\"");
                }

                writer.WriteLine(">");
                writer.Indent++;

                foreach (StiHtmlTableCell cell in row.Cells)
                {
                    #region Write table cell
                    writer.WriteBeginTag("td");

                    if (!string.IsNullOrEmpty(cell.ToolTip)) writer.WriteAttribute("title", cell.ToolTip);
                    if (!string.IsNullOrEmpty(cell.CssClass)) writer.WriteAttribute("class", cell.CssClass);
                    if (!string.IsNullOrEmpty(cell.Id)) writer.WriteAttribute("id", cell.Id);
                    if (!string.IsNullOrEmpty(cell.Editable)) writer.WriteAttribute("editable", cell.Editable);
                    if (cell.ColumnSpan > 0) writer.WriteAttribute("colspan", cell.ColumnSpan.ToString());
                    if (cell.RowSpan > 0) writer.WriteAttribute("rowspan", cell.RowSpan.ToString());
                    if (!string.IsNullOrEmpty(cell.Interaction)) writer.WriteAttribute("interaction", cell.Interaction);
                    if (!string.IsNullOrEmpty(cell.Collapsed)) writer.WriteAttribute("collapsed", cell.Collapsed);
                    if (!string.IsNullOrEmpty(cell.SortDirection)) writer.WriteAttribute("sort", cell.SortDirection);
                    if (!string.IsNullOrEmpty(cell.DataBandSort)) writer.WriteAttribute("databandsort", cell.DataBandSort);
                    if (!string.IsNullOrEmpty(cell.PageGuid)) writer.WriteAttribute("pageguid", cell.PageGuid);
                    if (!string.IsNullOrEmpty(cell.PageIndex)) writer.WriteAttribute("pageindex", cell.PageIndex);
                    if (!string.IsNullOrEmpty(cell.ReportFile)) writer.WriteAttribute("reportfile", cell.ReportFile);
                    if (!string.IsNullOrEmpty(cell.ComponentIndex)) writer.WriteAttribute("compindex", cell.ComponentIndex);
                    if (!string.IsNullOrEmpty(cell.DrillDownMode)) writer.WriteAttribute("drilldownmode", cell.DrillDownMode);
                    if (!StiHtmlUnit.IsNullOrZero(cell.Height)) cell.Style["height"] = cell.Height.ToString();
                    if (!StiHtmlUnit.IsNullOrZero(cell.Width)) cell.Style["width"] = cell.Width.ToString();

                    object marginsEntry = null;
                    string cellVertAlign = null;
                    string cellHorAlign = null;
                    //if (StiOptions.Export.Html.UseStrictTableCellSize && (cell.Style.Count > 0))
                    if (cell.Style.Count > 0)
                    {
                        if (cell.Style.ContainsKey(MarginsKey))
                        {
                            marginsEntry = cell.Style[MarginsKey];
                            cell.Style.Remove(MarginsKey);
                        }
                        if (cell.Style.ContainsKey(VertAlignKey) && cell.Style.ContainsKey("height"))
                        {
                            cellVertAlign = (string)cell.Style[VertAlignKey];
                            cell.Style.Remove(VertAlignKey);
                        }
                        if (cell.Style.ContainsKey(HorAlignKey) && cell.Style.ContainsKey("width"))
                        {
                            cellHorAlign = (string)cell.Style[HorAlignKey];
                            cell.Style.Remove(HorAlignKey);
                        }
                    }

                    bool cellWordwrap = false;
                    if (cell.Style.ContainsKey(WordwrapKey) && cell.Style.ContainsKey("width"))
                    {
                        cellWordwrap = true;
                        cell.Style.Remove(WordwrapKey);
                    }

                    StiHtmlHyperlink hyperLink = null;
                    StiHtmlImage image = null;
                    StiHtmlSvg svg = null;
                    bool hasRoundedRectangles = false;
                    if (cell.Controls.Count > 0)
                    {
                        hyperLink = cell.Controls[0] as StiHtmlHyperlink;
                        image = cell.Controls[0] as StiHtmlImage;
                        svg = cell.Controls[0] as StiHtmlSvg;
                        foreach (var control in cell.Controls)
                        {
                            if (control is StiHtmlRoundedRectangle) hasRoundedRectangles = true;
                        }
                    }

                    if ((image != null) && !StiOptions.Export.Html.UseStrictTableCellSize)
                    {
                        cell.Style["line-height"] = "0";
                    }
                    if (hasRoundedRectangles)
                    {
                        cell.Style["position"] = "relative";
                        cell.Style["overflow"] = "visible";
                    }

                    if (cell.Style.Count > 0)
                    {
                        writer.Write(" style=\"");
                        foreach (DictionaryEntry de in cell.Style)
                        {
                            writer.WriteStyleAttribute((string)de.Key, (string)de.Value);
                        }

                        if (StiOptions.Export.Html.UseStrictTableCellSizeV2)
                        {
                            if (marginsEntry != null)
                            {
                                writer.WriteStyleAttribute("padding", (string)marginsEntry);
                            }
                        }

                        writer.Write("\"");
                    }

                    writer.Write(">");

                    if (hasRoundedRectangles)
                    {
                        foreach (var control in cell.Controls)
                        {
                            var roundrect = control as StiHtmlRoundedRectangle;
                            if (roundrect != null)
                            {
                                string st = null;
                                if (roundrect.FitToCell)
                                {
                                    st = string.Format("<div style=\"width:calc(100% - {0}px);height:calc(100% - {0}px);margin:-{1}px;position:absolute;border:{0}px{2} {3};border-radius:{4}px;\"></div>", 
                                        roundrect.Size, 
                                        roundrect.Size / 2,
                                        roundrect.Style,
                                        roundrect.Color,
                                        roundrect.Round);
                                }
                                else
                                {
                                    st = string.Format("<div style=\"width:{0}px;height:{1}px;margin:-{2}px;position:absolute;border:{3}px{4} {5};border-radius:{6}px;\"></div>",
                                        roundrect.Width - roundrect.Size,
                                        roundrect.Height - roundrect.Size,
                                        roundrect.Size / 2,
                                        roundrect.Size,
                                        roundrect.Style,
                                        roundrect.Color,
                                        roundrect.Round);
                                }
                                writer.Write(st);
                            }
                        }
                    }

                    string stFixStyle = null;

                    #region For UseStrictTableCellSize
                    bool isCellNotEmpty = (cell.Controls.Count > 0) || !string.IsNullOrEmpty(cell.Text);
                    if (isCellNotEmpty && StiOptions.Export.Html.UseStrictTableCellSize)
                    {
                        writer.Write("<div");
                        if (StiOptions.Export.Html.UseExtendedStyle)
                        {
                            writer.WriteAttribute("class", "sBaseStyleFix");

                            string stFont = cell.Style["Font"] as string;
                            string stDecor = cell.Style["text-decoration"] as string;
                            string stColor = cell.Style["color"] as string;
                            if (!string.IsNullOrEmpty(stFont)) stFixStyle = "Font:" + stFont + ";";
                            if (!string.IsNullOrEmpty(stDecor)) stFixStyle += "text-decoration:" + stDecor + ";";
                            if (!string.IsNullOrEmpty(stColor)) stFixStyle += "color:" + stColor + ";";
                        }
                        if (cell.Style.ContainsKey("width") || cell.Style.ContainsKey("height") || (stFixStyle != null))
                        {
                            writer.Write(" style=\"");
                            if (cell.Style.ContainsKey("width"))
                            {
                                writer.WriteStyleAttribute("width", (string)cell.Style["width"]);
                            }
                            if (cell.Style.ContainsKey("height"))
                            {
                                writer.WriteStyleAttribute("height", (string)cell.Style["height"]);
                            }
                            if (!string.IsNullOrEmpty(stFixStyle))
                            {
                                writer.Write(stFixStyle);
                            }
                            writer.Write("\"");
                        }
                        writer.Write(">");

                        if ((cellVertAlign != null) || (cellHorAlign != null))
                        {
                            writer.Write("<div ");
                            if (StiOptions.Export.Html.UseExtendedStyle)
                            {
                                writer.Write("class=\"sBaseStyleFix\" ");
                            }
                            writer.Write("style=\"");
                            writer.WriteStyleAttribute("display", "table-cell");
                            if (cellVertAlign != null)
                            {
                                writer.WriteStyleAttribute("height", (string)cell.Style["height"]);
                                writer.WriteStyleAttribute("vertical-align", cellVertAlign);
                            }
                            if (cellHorAlign != null)
                            {
                                writer.WriteStyleAttribute("width", (string)cell.Style["width"]);
                                writer.WriteStyleAttribute("text-align", cellHorAlign);
                            }
                            if (!string.IsNullOrEmpty(stFixStyle))
                            {
                                writer.Write(stFixStyle);
                            }
                            writer.Write("\">");
                        }
                        if ((marginsEntry != null) || cellWordwrap)
                        {
                            writer.Write("<div ");
                            if (StiOptions.Export.Html.UseExtendedStyle)
                            {
                                writer.Write("class=\"sBaseStyleFix\" ");
                            }
                            writer.Write("style=\"");
                            if (marginsEntry != null)
                            {
                                writer.WriteStyleAttribute("margin", (string)marginsEntry);
                            }
                            if (cellWordwrap)
                            {
                                if (marginsEntry != null)
                                {
                                    try
                                    {
                                        var margins = ((string)marginsEntry).Replace("px", "").Split(' ');
                                        cell.Width.Value -= (Double.Parse(margins[1]) + Double.Parse(margins[3]));
                                    }
                                    catch { }
                                }
                                writer.WriteStyleAttribute("width", cell.Width.ToString());
                            }
                            if (!string.IsNullOrEmpty(stFixStyle))
                            {
                                writer.Write(stFixStyle);
                            }
                            writer.Write("\">");
                        }
                    }
                    #endregion

                    if (image != null)
                    {
                        var cellWidth = cell.Width;
                        var cellHeight = cell.Height;
                        var hasMargins = image.ImageComp != null && !image.ImageComp.Margins.IsEmpty;
                        string marginsText = null;
                        if (hasMargins)
                        {
                            // Assume that cell sizes are in px and margins are in hundredths of an inch (which are assumed to be equal to px)
                            int marginLeft = (int)Math.Truncate(image.ImageComp.Margins.Left * htmlExport.zoom);
                            int marginRight = (int)Math.Truncate(image.ImageComp.Margins.Right * htmlExport.zoom);
                            int marginTop = (int)Math.Truncate(image.ImageComp.Margins.Top * htmlExport.zoom);
                            int marginBottom = (int)Math.Truncate(image.ImageComp.Margins.Bottom * htmlExport.zoom);
                            cellWidth.Value -= (marginLeft + marginRight);
                            cellHeight.Value -= (marginTop + marginBottom);
                            marginsText = $"{marginTop}pt {marginRight}pt {marginBottom}pt {marginLeft}pt";
                        }

                        var imageRotation = StiImageRotation.None;
                        if (image.ImageComp != null)
                        {
                            imageRotation = image.ImageComp.ImageRotation;
                            if (Base.Helpers.StiSvgHelper.IsSvg(image.ImageComp.ImageBytesToDraw)) imageRotation = StiImageRotation.None;   //fix for painted svg
                        }

                        if (image.ImageUrl != null && image.ImageUrl.StartsWith("<svg"))
                        {
                            writer.Write(image.ImageUrl);
                        }
                        else if (image.ImageComp != null && !image.ImageComp.Stretch && imageRotation == StiImageRotation.None)
                        {
                            #region !Stretch & Rotation.None
                            writer.Write("<div style=\"");
                            writer.WriteStyleAttribute("height", cell.Height.ToString());
                            writer.WriteStyleAttribute("width", cell.Width.ToString());
                            writer.WriteStyleAttribute("display", "table-cell");
                            writer.WriteStyleAttribute("text-align", image.ImageComp.HorAlignment.ToString().ToLower());
                            writer.WriteStyleAttribute("vertical-align", image.ImageComp.VertAlignment.ToString().ToLower().Replace("center", "middle"));
                            writer.Write("\">");
                            writer.Write("<div style=\"");
                            writer.WriteStyleAttribute("max-height", cellHeight.ToString());
                            writer.WriteStyleAttribute("max-width", cellWidth.ToString());
                            writer.WriteStyleAttribute("overflow", "hidden");
                            if (hasMargins) writer.WriteStyleAttribute("margin", marginsText);
                            writer.Write("\">");
                            writer.WriteBeginTag("img");
                            if (!string.IsNullOrEmpty(image.ToolTip)) writer.WriteAttribute("title", image.ToolTip);
                            if (!string.IsNullOrEmpty(image.ImageUrl)) writer.WriteAttribute("src", StringToUrl(image.ImageUrl));
                            
                            double imgX = 0;
                            double imgY = 0;
                            var imgWidth = image.Width.Value * image.ImageComp.MultipleFactor * image.Zoom * dpi96;
                            var imgHeight = image.Height.Value * image.ImageComp.MultipleFactor * image.Zoom * dpi96;
                            
                            if (imgWidth > cellWidth.Value)
                            {
                                switch (image.ImageComp.HorAlignment)
                                {
                                    case StiHorAlignment.Center:
                                        imgX = cellWidth.Value / 2 - imgWidth / 2;
                                        break;
                                    case StiHorAlignment.Right:
                                        imgX = cellWidth.Value - imgWidth;
                                        break;
                                }
                            }
                            if (imgHeight > cellHeight.Value)
                            {
                                switch (image.ImageComp.VertAlignment)
                                {
                                    case StiVertAlignment.Center:
                                        imgY = cellHeight.Value / 2 - imgHeight / 2;
                                        break;
                                    case StiVertAlignment.Bottom:
                                        imgY = cellHeight.Value - imgHeight;
                                        break;
                                }
                            }

                            writer.WriteAttribute("height", imgHeight.ToString() + "px");
                            writer.WriteAttribute("width", imgWidth.ToString() + "px");

                            writer.Write(" style=\"");
                            writer.WriteStyleAttribute("border-width", StiHtmlUnit.ToPixelString(0));
                            writer.WriteStyleAttribute("margin-top", StiHtmlUnit.ToPixelString((int)imgY));
                            writer.WriteStyleAttribute("margin-left", StiHtmlUnit.ToPixelString((int)imgX));
                            writer.Write("\" /></div></div>");
                            #endregion
                        }
                        else if (image.ImageComp != null && (image.ImageComp.AspectRatio || imageRotation != StiImageRotation.None))
                        {
                            #region AspectRatio || !Rotation.None
                            writer.Write("<div style=\"");
                            writer.WriteStyleAttribute("width", cellWidth.ToString());
                            writer.WriteStyleAttribute("height", cellHeight.ToString());
                            if (hasMargins) writer.WriteStyleAttribute("margin", marginsText);

                            var horAlignment = image.ImageComp.HorAlignment;
                            var vertAlignment= image.ImageComp.VertAlignment;
                            switch (imageRotation)
                            {
                                case StiImageRotation.Rotate90CW:
                                    if (image.ImageComp.HorAlignment == StiHorAlignment.Left) vertAlignment = StiVertAlignment.Bottom;
                                    else if (image.ImageComp.HorAlignment == StiHorAlignment.Right) vertAlignment = StiVertAlignment.Top;
                                    else vertAlignment = StiVertAlignment.Center;
                                    if (image.ImageComp.VertAlignment == StiVertAlignment.Top) horAlignment = StiHorAlignment.Left;
                                    else if (image.ImageComp.VertAlignment == StiVertAlignment.Bottom) horAlignment = StiHorAlignment.Right;
                                    else horAlignment = StiHorAlignment.Center;
                                    break;

                                case StiImageRotation.Rotate90CCW:
                                    if (image.ImageComp.HorAlignment == StiHorAlignment.Left) vertAlignment = StiVertAlignment.Top;
                                    else if (image.ImageComp.HorAlignment == StiHorAlignment.Right) vertAlignment = StiVertAlignment.Bottom;
                                    else vertAlignment = StiVertAlignment.Center;
                                    if (image.ImageComp.VertAlignment == StiVertAlignment.Top) horAlignment = StiHorAlignment.Right;
                                    else if (image.ImageComp.VertAlignment == StiVertAlignment.Bottom) horAlignment = StiHorAlignment.Left;
                                    else horAlignment = StiHorAlignment.Center;
                                    break;

                                case StiImageRotation.Rotate180:
                                    if (image.ImageComp.HorAlignment == StiHorAlignment.Left) horAlignment = StiHorAlignment.Right;
                                    else if (image.ImageComp.HorAlignment == StiHorAlignment.Right) horAlignment = StiHorAlignment.Left;
                                    if (image.ImageComp.VertAlignment == StiVertAlignment.Top) vertAlignment = StiVertAlignment.Bottom;
                                    else if (image.ImageComp.VertAlignment == StiVertAlignment.Bottom) vertAlignment = StiVertAlignment.Top;
                                    break;

                                case StiImageRotation.FlipHorizontal:
                                    if (image.ImageComp.HorAlignment == StiHorAlignment.Left) horAlignment = StiHorAlignment.Right;
                                    else if (image.ImageComp.HorAlignment == StiHorAlignment.Right) horAlignment = StiHorAlignment.Left;
                                    break;

                                case StiImageRotation.FlipVertical:
                                    if (image.ImageComp.VertAlignment == StiVertAlignment.Top) vertAlignment = StiVertAlignment.Bottom;
                                    else if (image.ImageComp.VertAlignment == StiVertAlignment.Bottom) vertAlignment = StiVertAlignment.Top;
                                    break;
                            }

                            if (imageRotation == StiImageRotation.Rotate90CCW || imageRotation == StiImageRotation.Rotate90CW)
                            {
                                writer.Write("\"><div style=\"");
                                writer.WriteStyleAttribute("width", cellHeight.ToString());
                                writer.WriteStyleAttribute("height", cellWidth.ToString());

                                var offs = (cellWidth.Value - cellHeight.Value) / 2;
                                writer.WriteStyleAttribute("position", "relative");
                                writer.WriteStyleAttribute("left", StiHtmlUnit.NewUnit(offs).ToString());
                                writer.WriteStyleAttribute("top", StiHtmlUnit.NewUnit(-offs).ToString());

                                writer.WriteStyleAttribute("transform", "rotate(" + (imageRotation == StiImageRotation.Rotate90CCW ? "-" : "") + "90deg)");
                            }

                            writer.WriteStyleAttribute("background-repeat", "no-repeat");
                            writer.WriteStyleAttribute("background-position", horAlignment.ToString().ToLower() + " " + vertAlignment.ToString().ToLower());
                            writer.WriteStyleAttribute("background-image", "url(" + StiHtmlTable.StringToUrl(image.ImageUrl) + ")");

                            var imgWidth = image.Width.Value * image.ImageComp.MultipleFactor * image.Zoom;
                            var imgHeight = image.Height.Value * image.ImageComp.MultipleFactor * image.Zoom;
                            writer.WriteStyleAttribute("background-size", image.ImageComp.Stretch ? (image.ImageComp.AspectRatio ? "contain" : "100% 100%") : $"{imgWidth}px {imgHeight}px");
                            writer.WriteStyleAttribute("-webkit-print-color-adjust", "exact");
                            writer.WriteStyleAttribute("color-adjust", "exact");

                            if (imageRotation == StiImageRotation.FlipHorizontal) writer.WriteStyleAttribute("transform", "scaleX(-1)");
                            else if (imageRotation == StiImageRotation.FlipVertical) writer.WriteStyleAttribute("transform", "scaleY(-1)");
                            else if (imageRotation == StiImageRotation.Rotate180) writer.WriteStyleAttribute("transform", "scale(-1)");

                            if (imageRotation == StiImageRotation.Rotate90CCW || imageRotation == StiImageRotation.Rotate90CW) writer.Write("\"></div>");
                            else writer.Write("\">");

                            writer.Write("</div>");
                            #endregion
                        }
                        else
                        {
                            #region Simple stretch
                            writer.WriteBeginTag("img");
                            if (!string.IsNullOrEmpty(image.ToolTip)) writer.WriteAttribute("title", image.ToolTip);
                            if (!string.IsNullOrEmpty(image.ImageUrl)) writer.WriteAttribute("src", StringToUrl(image.ImageUrl));
                            writer.Write(" style=\"");
                            var width = cellWidth;
                            var height = cellHeight;
                            if (image != null && image.UseImageSize && image.Width != null && image.Height != null)
                            {
                                var cf = Math.Abs(width.Value / image.Width.Value + height.Value / image.Height.Value);
                                if (cf > 1.8 && cf < 2.2)
                                {
                                    width = image.Width;
                                    height = image.Height;
                                }                                
                            }
                            if (image.Margin == null)
                            {
                                writer.WriteStyleAttribute("height", height.ToString());
                                writer.WriteStyleAttribute("width", width.ToString());
                                if (hasMargins) writer.WriteStyleAttribute("margin", marginsText);
                            }
                            else
                            {
                                writer.WriteStyleAttribute("height", StiHtmlUnit.NewUnit(height.Value + Math.Abs(image.Margin.Value * 2)).ToString());
                                writer.WriteStyleAttribute("width", StiHtmlUnit.NewUnit(width.Value + Math.Abs(image.Margin.Value * 2)).ToString());
                                writer.WriteStyleAttribute("margin", image.Margin.ToString());
                            }
                            writer.WriteStyleAttribute("border-width", StiHtmlUnit.ToPixelString(0));
                            writer.Write("\" />");
                            #endregion
                        }
                    }
                    else
                    {
                        if (hyperLink != null)
                        {
                            writer.WriteBeginTag("a");
                            if (!string.IsNullOrEmpty(hyperLink.OpenLinksTarget)) writer.WriteAttribute("target", hyperLink.OpenLinksTarget);
                            if (!string.IsNullOrEmpty(hyperLink.ToolTip)) writer.WriteAttribute("title", hyperLink.ToolTip);
                            if (!string.IsNullOrEmpty(hyperLink.CssClass)) writer.WriteAttribute("class", hyperLink.CssClass);
                            if (hyperLink.Attributes.ContainsKey("name")) writer.WriteAttribute("name", (string)hyperLink.Attributes["name"]);
                            if (hyperLink.Attributes.ContainsKey("guid")) writer.WriteAttribute("guid", (string)hyperLink.Attributes["guid"]);
                            if (!string.IsNullOrEmpty(hyperLink.NavigateUrl)) writer.WriteAttribute("href", StringToUrl(hyperLink.NavigateUrl));
                            if (!string.IsNullOrEmpty(hyperLink.ImageUrl))
                            {
                                hyperLink.Style["display"] = "block";    //"inline-block";
                                hyperLink.Style["height"] = cell.Height.ToString();
                                hyperLink.Style["width"] = cell.Width.ToString();
                            }
                            if (StiOptions.Export.Html.UseExtendedStyle)
                            {
                                hyperLink.Style["border"] = "0";
                            }
                            if (hyperLink.Style.Count > 0)
                            {
                                writer.Write(" style=\"");
                                foreach (DictionaryEntry de in hyperLink.Style)
                                {
                                    writer.WriteStyleAttribute((string)de.Key, (string)de.Value);
                                }
                                if (!string.IsNullOrEmpty(stFixStyle))
                                {
                                    writer.Write(stFixStyle);
                                }
                                writer.Write("\"");
                            }
                            writer.Write(">");
                            if (!string.IsNullOrEmpty(hyperLink.ImageUrl))
                            {
                                //writer.WriteBeginTag("img");
                                //if (!string.IsNullOrEmpty(hyperLink.ToolTip)) writer.WriteAttribute("title", hyperLink.ToolTip);
                                //writer.WriteAttribute("src", StringToUrl(hyperLink.ImageUrl));
                                //writer.Write(" style=\"");
                                //writer.WriteStyleAttribute("height", hyperLink.Height.ToString());
                                //writer.WriteStyleAttribute("width", hyperLink.Width.ToString());
                                //writer.WriteStyleAttribute("border", "0");
                                //writer.Write("\" />");

                                if (hyperLink.ImageComp != null && !hyperLink.ImageComp.Stretch && hyperLink.ImageComp.ImageRotation == StiImageRotation.None)
                                {
                                    #region !Stretch & Rotation.None
                                    writer.Write("<div style=\"");
                                    writer.WriteStyleAttribute("height", cell.Height.ToString());
                                    writer.WriteStyleAttribute("width", cell.Width.ToString());
                                    writer.WriteStyleAttribute("display", "table-cell");
                                    writer.WriteStyleAttribute("text-align", hyperLink.ImageComp.HorAlignment.ToString().ToLower());
                                    writer.WriteStyleAttribute("vertical-align", hyperLink.ImageComp.VertAlignment.ToString().ToLower().Replace("center", "middle"));
                                    writer.Write("\">");
                                    writer.Write("<div style=\"");
                                    writer.WriteStyleAttribute("max-height", cell.Height.ToString());
                                    writer.WriteStyleAttribute("max-width", cell.Width.ToString());
                                    writer.WriteStyleAttribute("overflow", "hidden");
                                    writer.Write("\">");
                                    writer.WriteBeginTag("img");
                                    if (!string.IsNullOrEmpty(hyperLink.ToolTip)) writer.WriteAttribute("title", hyperLink.ToolTip);
                                    if (!string.IsNullOrEmpty(hyperLink.ImageUrl)) writer.WriteAttribute("src", StringToUrl(hyperLink.ImageUrl));

                                    double imgX = 0;
                                    double imgY = 0;
                                    var imgWidth = hyperLink.Width.Value * hyperLink.ImageComp.MultipleFactor * hyperLink.Zoom;
                                    var imgHeight = hyperLink.Height.Value * hyperLink.ImageComp.MultipleFactor * hyperLink.Zoom;

                                    if (imgWidth > cell.Width.Value)
                                    {
                                        switch (hyperLink.ImageComp.HorAlignment)
                                        {
                                            case StiHorAlignment.Center:
                                                imgX = cell.Width.Value / 2 - imgWidth / 2;
                                                break;
                                            case StiHorAlignment.Right:
                                                imgX = cell.Width.Value - imgWidth;
                                                break;
                                        }
                                    }
                                    if (imgHeight > cell.Height.Value)
                                    {
                                        switch (hyperLink.ImageComp.VertAlignment)
                                        {
                                            case StiVertAlignment.Center:
                                                imgY = cell.Height.Value / 2 - imgHeight / 2;
                                                break;
                                            case StiVertAlignment.Bottom:
                                                imgY = cell.Height.Value - imgHeight;
                                                break;
                                        }
                                    }

                                    writer.WriteAttribute("height", imgHeight.ToString() + "px");
                                    writer.WriteAttribute("width", imgWidth.ToString() + "px");

                                    writer.Write(" style=\"");
                                    writer.WriteStyleAttribute("border-width", StiHtmlUnit.ToPixelString(0));
                                    writer.WriteStyleAttribute("margin-top", StiHtmlUnit.ToPixelString((int)imgY));
                                    writer.WriteStyleAttribute("margin-left", StiHtmlUnit.ToPixelString((int)imgX));
                                    writer.Write("\" /></div></div>");
                                    #endregion
                                }
                                else if (hyperLink.ImageComp != null && (hyperLink.ImageComp.AspectRatio || hyperLink.ImageComp.ImageRotation != StiImageRotation.None))
                                {
                                    #region AspectRatio || !Rotation.None
                                    writer.Write("<div style=\"");
                                    writer.WriteStyleAttribute("width", cell.Width.ToString());
                                    writer.WriteStyleAttribute("height", cell.Height.ToString());

                                    var horAlignment = hyperLink.ImageComp.HorAlignment;
                                    var vertAlignment = hyperLink.ImageComp.VertAlignment;
                                    switch (hyperLink.ImageComp.ImageRotation)
                                    {
                                        case StiImageRotation.Rotate90CW:
                                            if (hyperLink.ImageComp.HorAlignment == StiHorAlignment.Left) vertAlignment = StiVertAlignment.Bottom;
                                            else if (hyperLink.ImageComp.HorAlignment == StiHorAlignment.Right) vertAlignment = StiVertAlignment.Top;
                                            else vertAlignment = StiVertAlignment.Center;
                                            if (hyperLink.ImageComp.VertAlignment == StiVertAlignment.Top) horAlignment = StiHorAlignment.Left;
                                            else if (hyperLink.ImageComp.VertAlignment == StiVertAlignment.Bottom) horAlignment = StiHorAlignment.Right;
                                            else horAlignment = StiHorAlignment.Center;
                                            break;

                                        case StiImageRotation.Rotate90CCW:
                                            if (hyperLink.ImageComp.HorAlignment == StiHorAlignment.Left) vertAlignment = StiVertAlignment.Top;
                                            else if (hyperLink.ImageComp.HorAlignment == StiHorAlignment.Right) vertAlignment = StiVertAlignment.Bottom;
                                            else vertAlignment = StiVertAlignment.Center;
                                            if (hyperLink.ImageComp.VertAlignment == StiVertAlignment.Top) horAlignment = StiHorAlignment.Right;
                                            else if (hyperLink.ImageComp.VertAlignment == StiVertAlignment.Bottom) horAlignment = StiHorAlignment.Left;
                                            else horAlignment = StiHorAlignment.Center;
                                            break;

                                        case StiImageRotation.Rotate180:
                                            if (hyperLink.ImageComp.HorAlignment == StiHorAlignment.Left) horAlignment = StiHorAlignment.Right;
                                            else if (hyperLink.ImageComp.HorAlignment == StiHorAlignment.Right) horAlignment = StiHorAlignment.Left;
                                            if (hyperLink.ImageComp.VertAlignment == StiVertAlignment.Top) vertAlignment = StiVertAlignment.Bottom;
                                            else if (hyperLink.ImageComp.VertAlignment == StiVertAlignment.Bottom) vertAlignment = StiVertAlignment.Top;
                                            break;

                                        case StiImageRotation.FlipHorizontal:
                                            if (hyperLink.ImageComp.HorAlignment == StiHorAlignment.Left) horAlignment = StiHorAlignment.Right;
                                            else if (hyperLink.ImageComp.HorAlignment == StiHorAlignment.Right) horAlignment = StiHorAlignment.Left;
                                            break;

                                        case StiImageRotation.FlipVertical:
                                            if (hyperLink.ImageComp.VertAlignment == StiVertAlignment.Top) vertAlignment = StiVertAlignment.Bottom;
                                            else if (hyperLink.ImageComp.VertAlignment == StiVertAlignment.Bottom) vertAlignment = StiVertAlignment.Top;
                                            break;
                                    }

                                    if (hyperLink.ImageComp.ImageRotation == StiImageRotation.Rotate90CCW || hyperLink.ImageComp.ImageRotation == StiImageRotation.Rotate90CW)
                                    {
                                        writer.Write("\"><div style=\"");
                                        writer.WriteStyleAttribute("width", cell.Height.ToString());
                                        writer.WriteStyleAttribute("height", cell.Width.ToString());

                                        var offs = (cell.Width.Value - cell.Height.Value) / 2;
                                        writer.WriteStyleAttribute("position", "relative");
                                        writer.WriteStyleAttribute("left", StiHtmlUnit.NewUnit(offs).ToString());
                                        writer.WriteStyleAttribute("top", StiHtmlUnit.NewUnit(-offs).ToString());

                                        writer.WriteStyleAttribute("transform", "rotate(" + (hyperLink.ImageComp.ImageRotation == StiImageRotation.Rotate90CCW ? "-" : "") + "90deg)");
                                    }

                                    writer.WriteStyleAttribute("background-repeat", "no-repeat");
                                    writer.WriteStyleAttribute("background-position", horAlignment.ToString().ToLower() + " " + vertAlignment.ToString().ToLower());
                                    writer.WriteStyleAttribute("background-image", "url(" + StiHtmlTable.StringToUrl(hyperLink.ImageUrl) + ")");

                                    var imgWidth = hyperLink.Width.Value * hyperLink.ImageComp.MultipleFactor * hyperLink.Zoom;
                                    var imgHeight = hyperLink.Height.Value * hyperLink.ImageComp.MultipleFactor * hyperLink.Zoom;
                                    writer.WriteStyleAttribute("background-size", hyperLink.ImageComp.Stretch ? (hyperLink.ImageComp.AspectRatio ? "contain" : "100% 100%") : $"{imgWidth}px {imgHeight}px");

                                    if (hyperLink.ImageComp.ImageRotation == StiImageRotation.FlipHorizontal) writer.WriteStyleAttribute("transform", "scaleX(-1)");
                                    else if (hyperLink.ImageComp.ImageRotation == StiImageRotation.FlipVertical) writer.WriteStyleAttribute("transform", "scaleY(-1)");
                                    else if (hyperLink.ImageComp.ImageRotation == StiImageRotation.Rotate180) writer.WriteStyleAttribute("transform", "scale(-1)");

                                    if (hyperLink.ImageComp.ImageRotation == StiImageRotation.Rotate90CCW || hyperLink.ImageComp.ImageRotation == StiImageRotation.Rotate90CW) writer.Write("\"></div>");
                                    else writer.Write("\">");

                                    writer.Write("</div>");
                                    #endregion
                                }
                                else
                                {
                                    #region Simple stretch
                                    writer.WriteBeginTag("img");
                                    if (!string.IsNullOrEmpty(hyperLink.ToolTip)) writer.WriteAttribute("title", hyperLink.ToolTip);
                                    if (!string.IsNullOrEmpty(hyperLink.ImageUrl)) writer.WriteAttribute("src", StringToUrl(hyperLink.ImageUrl));
                                    writer.Write(" style=\"");
                                    var width = cell.Width;
                                    var height = cell.Height;
                                    if (image != null && image.UseImageSize && image.Width != null && image.Height != null)
                                    {
                                        var cf = Math.Abs(width.Value / image.Width.Value + height.Value / image.Height.Value);
                                        if (cf > 1.8 && cf < 2.2)
                                        {
                                            width = image.Width;
                                            height = image.Height;
                                        }
                                    }
                                    if (hyperLink.Margin == null)
                                    {
                                        writer.WriteStyleAttribute("height", height.ToString());
                                        writer.WriteStyleAttribute("width", width.ToString());
                                    }
                                    else
                                    {
                                        writer.WriteStyleAttribute("height", StiHtmlUnit.NewUnit(height.Value + Math.Abs(hyperLink.Margin.Value * 2)).ToString());
                                        writer.WriteStyleAttribute("width", StiHtmlUnit.NewUnit(width.Value + Math.Abs(hyperLink.Margin.Value * 2)).ToString());
                                        writer.WriteStyleAttribute("margin", hyperLink.Margin.ToString());
                                    }
                                    writer.WriteStyleAttribute("border-width", StiHtmlUnit.ToPixelString(0));
                                    writer.Write("\" />");
                                    #endregion
                                }
                            }
                            writer.Write(hyperLink.Text ?? cell.Text);
                            writer.WriteFullEndTag("a");
                        }
                        else if (svg != null)
                        {
                            writer.Write(svg.Text);
                        }
                        else
                        {
                            if (hasRoundedRectangles)
                            {
                                writer.Write("<div style=\"display:table-cell;");
                                if (cell.Style.ContainsKey("width"))
                                {
                                    writer.WriteStyleAttribute("width", (string)cell.Style["width"]);
                                }
                                if (cell.Style.ContainsKey("height"))
                                {
                                    writer.WriteStyleAttribute("height", (string)cell.Style["height"]);
                                }
                                if (cellVertAlign != null)
                                {
                                    writer.WriteStyleAttribute("vertical-align", cellVertAlign);
                                }
                                if (cellHorAlign != null)
                                {
                                    writer.WriteStyleAttribute("text-align", cellHorAlign);
                                }
                                writer.Write("\">");
                            }
                            writer.Write(cell.Text);
                            if (hasRoundedRectangles)
                            {
                                writer.Write("</div>");
                            }
                        }
                    }

                    if (isCellNotEmpty && StiOptions.Export.Html.UseStrictTableCellSize)
                    {
                        if ((marginsEntry != null) || cellWordwrap)
                        {
                            writer.Write("</div>");
                        }
                        if ((cellVertAlign != null) || (cellHorAlign != null))
                        {
                            writer.Write("</div>");
                        }
                        writer.Write("</div>");
                    }

                    writer.WriteFullEndTag("td");
                    #endregion
                }
                writer.WriteLine();
                writer.Indent--;
                writer.WriteFullEndTag("tr");
                #endregion
            }

            WriteTableEnd(writer, addPageBreaks);
        }

        private void WriteTableBegin(StiHtmlTextWriter writer, bool writePadding, bool writePageBreak)
        {
            if (writePadding)
            {
                writer.WriteBeginTag("div");
                writer.WriteAttribute("class", "pagemargins");
                if (writePageBreak)
                {
                    writer.Write(" style=\"");
                    writer.WriteStyleAttribute(PageBreakBeforeKey, "always");
                    writer.Write("\"");
                }
                writer.WriteLine(">");
                writer.Indent++;
            }

            writer.WriteBeginTag("table");
            writer.WriteAttribute("cellspacing", CellSpacing.ToString());
            writer.WriteAttribute("cellpadding", CellPadding.ToString());
            writer.WriteAttribute("border", "0");
            if (Align != StiHorAlignment.Left)
            {
                writer.WriteAttribute("align", (Align == StiHorAlignment.Center ? "center" : "right"));
            }
            if (StiOptions.Export.Html.UseExtendedStyle)
            {
                writer.WriteAttribute("class", "sBaseStyleFix");
            }

            writer.Write(" style=\"");
            writer.WriteStyleAttribute("border-width", StiHtmlUnit.ToPixelString(BorderWidth));
            writer.WriteStyleAttribute("width", Width.ToString());
            if (Border != null && Border.DropShadow)
            {
                var shadowColor = StiBrush.ToColor(Border.ShadowBrush);
                writer.WriteStyleAttribute("box-shadow", $"{Border.ShadowSize}px {Border.ShadowSize}px 1px rgb({shadowColor.R},{shadowColor.G},{shadowColor.B})");
            }
            if (!string.IsNullOrEmpty(Position))
            {
                writer.WriteStyleAttribute("Position", Position);
            }
            if (!string.IsNullOrEmpty(BackImageUrl))
            {
                writer.WriteStyleAttribute("background-image", string.Format("url('{0}')", StringToUrl(BackImageUrl)));
            }
            writer.WriteStyleAttribute("border-collapse", "collapse");
            writer.WriteStyleAttribute("white-space", "normal");
            writer.Write("\">");

            writer.WriteLine();
            writer.Indent++;

            if (StiOptions.Export.Html.UseExtendedStyle)
            {
                writer.WriteBeginTag("tbody");
                writer.WriteAttribute("class", "sBaseStyleFix");
                writer.WriteLine(">");
                writer.Indent++;
            }
        }

        private void WriteTableEnd(StiHtmlTextWriter writer, bool writePadding)
        {
            if (StiOptions.Export.Html.UseExtendedStyle)
            {
                writer.WriteLine();
                writer.Indent--;
                writer.WriteFullEndTag("tbody");
            }

            writer.WriteLine();
            writer.Indent--;
            writer.WriteFullEndTag("table");

            if (writePadding)
            {
                writer.WriteLine();
                writer.Indent--;
                writer.WriteFullEndTag("div");
            }
        }
        #endregion

        public StiHtmlTable()
        {
            Rows = new ArrayList();
        }
    }
    #endregion

    #region StiHtmlTextWriter
    public class StiHtmlTextWriter
    {
        #region Enums
        public enum WriterMode
        {
            None,
            BeginTag,
            Attribute,
            Data
        }
        #endregion

        #region Fields
        private TextWriter stream = null;
        private WriterMode mode = WriterMode.None;
        private int indent = 0;
        #endregion

        #region Properties
        public int Indent
        {
            get
            {
                return indent;
            }
            set
            {
                indent = value;
            }
        }
        #endregion

        #region Public Methods
        public void Write(string st)
        {
            CheckIndent();
            stream.Write(st);
            mode = WriterMode.Data;
        }
        public void WriteLine()
        {
            stream.WriteLine();
            mode = WriterMode.None;
        }
        public void WriteLine(string st)
        {
            if (string.IsNullOrEmpty(st))
            {
                stream.WriteLine();
            }
            else
            {
                CheckIndent();
                stream.WriteLine(st);
            }
            mode = WriterMode.None;
        }

        public void WriteBeginTag(string st)
        {
            CloseTag();
            CheckIndent();
            stream.Write("<" + st);
            mode = WriterMode.BeginTag;
        }
        public void WriteFullBeginTag(string st)
        {
            CloseTag();
            CheckIndent();
            stream.Write("<" + st + ">");
            mode = WriterMode.Data;
        }

        public void WriteEndTag(string st)
        {
            if (mode == WriterMode.BeginTag)
            {
                stream.Write("/>");
            }
            else
            {
                CloseTag();
                CheckIndent();
                stream.Write("</" + st + ">");
            }
            mode = WriterMode.Data;
        }
        public void WriteFullEndTag(string st)
        {
            CloseTag();
            CheckIndent();
            stream.Write("</" + st + ">");
            mode = WriterMode.Data;
        }

        public void WriteAttribute(string attr, string value)
        {
            stream.Write(" " + attr);
            if (value != null)
            {
                stream.Write("=\"" + value + "\"");
            }
            mode = WriterMode.Attribute;
        }
        public void WriteStyleAttribute(string attr, string value)
        {
            stream.Write(attr + ":" + value + ";");
            mode = WriterMode.Attribute;
        }

        public void Flush()
        {
            stream.Flush();
        }
        #endregion

        #region Private Methods
        private void CloseTag()
        {
            if (mode == WriterMode.Attribute ||
                mode == WriterMode.BeginTag)
            {
                stream.Write(">");
            }
        }

        private void CheckIndent()
        {
            if (mode == WriterMode.None)
            {
                for (int index = 0; index < indent; index++) stream.Write("\t");
            }
        }
        #endregion

        public StiHtmlTextWriter(TextWriter baseStream)
        {
            stream = baseStream;
            mode = WriterMode.None;
            indent = 0;
        }
    }
    #endregion

    public sealed class StiHtmlTableRender
    {
        #region Fields
        private StiHtmlExportService htmlExport = null;
		private StiHtmlExportSettings htmlExportSettings = null;
		#endregion

		#region Properties
		private StiMatrix matrix;
		public StiMatrix Matrix
		{
			get
			{
				return matrix;
			}
			set
			{
				matrix = value;
			}
		}
        #endregion

		#region Methods
		public void RenderStyle(StiCellStyle style)
		{	
			htmlExport.RenderBackColor(null, style.Color);
			htmlExport.RenderTextColor(null, style.TextColor);
			htmlExport.RenderFont(null, style.Font);
			
            htmlExport.RenderBorder(null, style.Border, "top");
            htmlExport.RenderBorder(null, style.BorderL, "left");
            htmlExport.RenderBorder(null, style.BorderR, "right");
            htmlExport.RenderBorder(null, style.BorderB, "bottom");

			htmlExport.RenderTextDirection(null, style.TextOptions);
			htmlExport.RenderTextHorAlignment(null, style.TextOptions, style.HorAlignment);
			htmlExport.RenderVertAlignment(null, style.VertAlignment, style.TextOptions, style.AllowHtmlTags);

			if (style.AbsolutePosition)htmlExport.HtmlWriter.WriteStyleAttribute("position", "absolute");
            htmlExport.HtmlWriter.Write("overflow:hidden;");
            htmlExport.HtmlWriter.Write(string.Format("line-height:{0}em;", Math.Round(1.15 * style.LineSpacing * StiHtmlExportService.GetFontHeightCorrection(style.Font.Name), 2)).Replace(",", "."));
            if (style.TextOptions != null && style.TextOptions.Trimming != StringTrimming.None)
            {
                htmlExport.HtmlWriter.Write("text-overflow:ellipsis;");
            }
        }

        public void RenderStyleTable(StiHtmlTableCell cell, StiCellStyle style)
		{	
			htmlExport.RenderBackColor(cell, style.Color);
			htmlExport.RenderTextColor(cell, style.TextColor);
			htmlExport.RenderFont(cell, style.Font);

            if (cell == null) htmlExport.HtmlWriter.Write("border:0px;");
			htmlExport.RenderBorder(cell, style.Border, "top");
			htmlExport.RenderBorder(cell, style.BorderL, "left");
			htmlExport.RenderBorder(cell, style.BorderR, "right");
			htmlExport.RenderBorder(cell, style.BorderB, "bottom");

			htmlExport.RenderTextDirection(cell, style.TextOptions);
			htmlExport.RenderTextHorAlignment(cell, style.TextOptions, style.HorAlignment);
			htmlExport.RenderVertAlignment(cell, style.VertAlignment, style.TextOptions, style.AllowHtmlTags);

            bool wordWrap = style.WordWrap;
            if (style.HorAlignment == StiTextHorAlignment.Width) wordWrap = true;

            if (cell == null)
            {
                if (style.AbsolutePosition) htmlExport.HtmlWriter.WriteStyleAttribute("position", "absolute");
                htmlExport.HtmlWriter.Write(string.Format("line-height:{0}em;", Math.Round(1.15 * style.LineSpacing * StiHtmlExportService.GetFontHeightCorrection(style.Font.Name), 2)).Replace(",", "."));
                if (StiOptions.Export.Html.UseStrictTableCellSize || StiOptions.Export.Html.UseStrictTableCellSizeV2)
                {
                    if (wordWrap)
                    {
                        htmlExport.HtmlWriter.Write("word-wrap:break-word;");
                    }
                    else
                    {
                        htmlExport.HtmlWriter.Write("white-space:nowrap;");
                    }
                    htmlExport.HtmlWriter.Write("overflow:hidden;");
                    if (style.TextOptions != null && style.TextOptions.Trimming != StringTrimming.None)
                    {
                        htmlExport.HtmlWriter.Write("text-overflow:ellipsis;");
                    }
                }
                else
                {
                    if (!wordWrap && StiOptions.Export.Html.UseWordWrapBreakWordMode) htmlExport.HtmlWriter.Write("word-wrap:break-word;");
                }
            }
            else
            {
                if (style.AbsolutePosition) cell.Style["position"] = "absolute";
                cell.Style["line-height"] = string.Format("{0}em", Math.Round(1.15 * style.LineSpacing * StiHtmlExportService.GetFontHeightCorrection(style.Font.Name), 2)).Replace(",", ".");
                if (StiOptions.Export.Html.UseStrictTableCellSize || StiOptions.Export.Html.UseStrictTableCellSizeV2)
                {
                    if (wordWrap)
                    {
                        cell.Style["word-wrap"] = "break-word";
                    }
                    else
                    {
                        cell.Style["white-space"] = "nowrap";
                    }
                    cell.Style["overflow"] = "hidden";
                    if (style.TextOptions != null && style.TextOptions.Trimming != StringTrimming.None)
                    {
                        cell.Style["text-overflow"] = "ellipsis";
                    }
                }
                else
                {
                    if (!wordWrap && StiOptions.Export.Html.UseWordWrapBreakWordMode) cell.Style["word-wrap"] = "break-word";
                }
            }
		}

        public void RenderStyles(bool useBookmarks, bool exportBookmarksOnly, Hashtable cssStyles)
		{			
			htmlExport.HtmlWriter.WriteLine("<style type=\"text/css\">");

            if (StiOptions.Export.Html.UseExtendedStyle)
            {
                htmlExport.HtmlWriter.WriteLine(".sBaseStyleFix { border: 0; }");
            }

			if ((!exportBookmarksOnly) && (htmlExport.useStylesTable))
			{
				for (int index = 0; index < matrix.Styles.Count; index++)
				{
					StiCellStyle style = matrix.Styles[index] as StiCellStyle;
					htmlExport.HtmlWriter.Write(".s" + style.StyleName);
					htmlExport.HtmlWriter.Write("{");
					RenderStyle(style);
					htmlExport.HtmlWriter.WriteLine("}");
				}
			}
			if ((cssStyles != null) && (cssStyles.Count > 0))
			{
				foreach (DictionaryEntry entry in cssStyles)
				{
					htmlExport.HtmlWriter.WriteLine("." + (string)entry.Key + " {" + (string)entry.Value + ";overflow:hidden;}");
				}
			}
			if (useBookmarks)
			{
                if (StiOptions.Export.Html.UseExtendedStyle)
                {
                    htmlExport.HtmlWriter.WriteLine(".dtree {border:0; font-family: Verdana, Geneva, Arial, Helvetica, sans-serif;font-size:11px;color:#666;white-space:nowrap;}");
                    htmlExport.HtmlWriter.WriteLine(".dTreeNode {border:0;}");
                    htmlExport.HtmlWriter.WriteLine(".dtreeStyleFix {border:0;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree img {border:0; vertical-align: middle;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree a {border:0;line-height:0; color:#333;text-decoration:none;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree a.node, .dtree a.nodeSel {border:0; white-space: nowrap;padding: 1px 2px 1px 2px;font-family: Verdana, Geneva, Arial, Helvetica, sans-serif;font-size:11px;color:#666;text-decoration: none;font-weight:normal;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree a.node:hover, .dtree a.nodeSel:hover {border:0; color: #333;text-decoration: underline;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree a.nodeSel {border:0; background-color: #c0d2ec;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree .clip {border:0; overflow: hidden;}");
                    htmlExport.HtmlWriter.WriteLine(".dtreeframe {border:0; border-right:1px;border-right-style:solid;border-right-color:Gray;}");
                }
                else
                {
                    htmlExport.HtmlWriter.WriteLine(".dtree {font-family: Verdana, Geneva, Arial, Helvetica, sans-serif;font-size:11px;color:#666;white-space:nowrap;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree img {border: 0px;vertical-align: middle;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree a {color: #333;text-decoration: none;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree a.node, .dtree a.nodeSel {white-space: nowrap;padding: 1px 2px 1px 2px;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree a.node:hover, .dtree a.nodeSel:hover {color: #333;text-decoration: underline;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree a.nodeSel {background-color: #c0d2ec;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree .clip {overflow: hidden;}");
                    htmlExport.HtmlWriter.WriteLine(".dtreeframe {border-right:1px;border-right-style:solid;border-right-color:Gray;}");
                }
			}
            if (htmlExportSettings.AddPageBreaks)
            {
                StiPage page = htmlExport.report.RenderedPages[0];
                htmlExport.report.RenderedPages.GetPage(page);
                double pageWidth = Math.Round(Units.StiMillimetersUnit.Millimeters.ConvertFromHInches(htmlExport.report.Unit.ConvertToHInches(page.PageWidth)), 2);
                double pageHeight = Math.Round(Units.StiMillimetersUnit.Millimeters.ConvertFromHInches(htmlExport.report.Unit.ConvertToHInches(page.PageHeight)), 2);
                double mgLeft = Math.Round(Units.StiMillimetersUnit.Millimeters.ConvertFromHInches(htmlExport.report.Unit.ConvertToHInches(page.Margins.Left)), 2);
                double mgRight = Math.Round(Units.StiMillimetersUnit.Millimeters.ConvertFromHInches(htmlExport.report.Unit.ConvertToHInches(page.Margins.Right)), 2);
                double mgTop = Math.Round(Units.StiMillimetersUnit.Millimeters.ConvertFromHInches(htmlExport.report.Unit.ConvertToHInches(page.Margins.Top)), 2);
                double mgBottom = Math.Round(Units.StiMillimetersUnit.Millimeters.ConvertFromHInches(htmlExport.report.Unit.ConvertToHInches(page.Margins.Bottom)), 2);

                htmlExport.HtmlWriter.WriteLine(".pagemargins { padding:0; border:0; }");
                htmlExport.HtmlWriter.Write("@page { size: ");
                if (page.PaperSize != PaperKind.Custom)
                {
                    htmlExport.HtmlWriter.Write(page.PaperSize.ToString());
                    if (page.Orientation == StiPageOrientation.Landscape) htmlExport.HtmlWriter.Write(" landscape");
                }
                else
                {
                    htmlExport.HtmlWriter.Write($"{pageWidth}mm {pageHeight}mm");
                }
                htmlExport.HtmlWriter.WriteLine("; margin: 0; }");
                htmlExport.HtmlWriter.Write($"@media print {{ html,body {{ width: {pageWidth - mgLeft - mgRight}mm; height: {pageHeight - mgTop - mgBottom}mm; }} ");
                htmlExport.HtmlWriter.WriteLine($".pagemargins {{ padding: {mgTop}mm {mgRight}mm {mgBottom / 2}mm {mgLeft}mm; border:0; }} }}");
            }
            htmlExport.HtmlWriter.WriteLine("</style>");
		}

		public void RenderStylesTable(bool useBookmarks, bool exportBookmarksOnly)
		{
			RenderStylesTable(useBookmarks, exportBookmarksOnly, null);
		}

        public void RenderStylesTable(bool useBookmarks, bool exportBookmarksOnly, bool addStyleTag)
        {
            RenderStylesTable(useBookmarks, exportBookmarksOnly, addStyleTag, null);
        }

        public void RenderStylesTable(bool useBookmarks, bool exportBookmarksOnly, Hashtable cssStyles)
        {
            RenderStylesTable(useBookmarks, exportBookmarksOnly, true, cssStyles);
        }

		public void RenderStylesTable(bool useBookmarks, bool exportBookmarksOnly, bool addStyleTag, Hashtable cssStyles)
		{
			if (addStyleTag) htmlExport.HtmlWriter.WriteLine("<style type=\"text/css\">");

            if (StiOptions.Export.Html.UseExtendedStyle)
            {
                htmlExport.HtmlWriter.WriteLine(".sBaseStyleFix { border: 0; }");
            }

            if ((!exportBookmarksOnly) && (htmlExport.useStylesTable))
			{
				for (int index = 0; index < matrix.Styles.Count; index++)
				{
					StiCellStyle style = matrix.Styles[index] as StiCellStyle;
					htmlExport.HtmlWriter.Write(".s" + style.StyleName);
					htmlExport.HtmlWriter.Write("{");
                    RenderStyleTable(null, style);
					htmlExport.HtmlWriter.WriteLine("}");
				}
			}
			if ((cssStyles != null) && (cssStyles.Count > 0))
			{
				foreach (DictionaryEntry entry in cssStyles)
				{
					htmlExport.HtmlWriter.WriteLine("." + (string)entry.Key + " {" + (string)entry.Value + ";}");
				}
			}
			if (useBookmarks)
			{
                if (StiOptions.Export.Html.UseExtendedStyle)
                {
                    htmlExport.HtmlWriter.WriteLine(".dtree {border:0; font-family: Verdana, Geneva, Arial, Helvetica, sans-serif;font-size:11px;color:#666;white-space:nowrap;}");
                    htmlExport.HtmlWriter.WriteLine(".dTreeNode {border:0;}");
                    htmlExport.HtmlWriter.WriteLine(".dtreeStyleFix {border:0;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree img {border:0; vertical-align: middle;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree a {border:0;line-height:0; color:#333;text-decoration:none;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree a.node, .dtree a.nodeSel {border:0; white-space: nowrap;padding: 1px 2px 1px 2px;font-family: Verdana, Geneva, Arial, Helvetica, sans-serif;font-size:11px;color:#666;text-decoration: none;font-weight:normal;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree a.node:hover, .dtree a.nodeSel:hover {border:0; color: #333;text-decoration: underline;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree a.nodeSel {border:0; background-color: #c0d2ec;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree .clip {border:0; overflow: hidden;}");
                    htmlExport.HtmlWriter.WriteLine(".dtreeframe {border:0; border-right:1px;border-right-style:solid;border-right-color:Gray;}");
                }
                else
                {
                    htmlExport.HtmlWriter.WriteLine(".dtree {font-family: Verdana, Geneva, Arial, Helvetica, sans-serif;font-size:11px;color:#666;white-space:nowrap;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree img {border: 0px;vertical-align: middle;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree a {color: #333;text-decoration: none;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree a.node, .dtree a.nodeSel {white-space: nowrap;padding: 1px 2px 1px 2px;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree a.node:hover, .dtree a.nodeSel:hover {color: #333;text-decoration: underline;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree a.nodeSel {background-color: #c0d2ec;}");
                    htmlExport.HtmlWriter.WriteLine(".dtree .clip {overflow: hidden;}");
                    htmlExport.HtmlWriter.WriteLine(".dtreeframe {border-right:1px;border-right-style:solid;border-right-color:Gray;}");
                }
			}
            if (htmlExportSettings.AddPageBreaks)
            {
                StiPage page = htmlExport.report.RenderedPages[0];
                htmlExport.report.RenderedPages.GetPage(page);
                double pageWidth = Math.Round(Units.StiMillimetersUnit.Millimeters.ConvertFromHInches(htmlExport.report.Unit.ConvertToHInches(page.PageWidth)), 2);
                double pageHeight = Math.Round(Units.StiMillimetersUnit.Millimeters.ConvertFromHInches(htmlExport.report.Unit.ConvertToHInches(page.PageHeight)), 2);
                double mgLeft = Math.Round(Units.StiMillimetersUnit.Millimeters.ConvertFromHInches(htmlExport.report.Unit.ConvertToHInches(page.Margins.Left)), 2);
                double mgRight = Math.Round(Units.StiMillimetersUnit.Millimeters.ConvertFromHInches(htmlExport.report.Unit.ConvertToHInches(page.Margins.Right)), 2);
                double mgTop = Math.Round(Units.StiMillimetersUnit.Millimeters.ConvertFromHInches(htmlExport.report.Unit.ConvertToHInches(page.Margins.Top)), 2);
                double mgBottom = Math.Round(Units.StiMillimetersUnit.Millimeters.ConvertFromHInches(htmlExport.report.Unit.ConvertToHInches(page.Margins.Bottom)), 2);

                htmlExport.HtmlWriter.WriteLine(".pagemargins { padding:0; border:0; }");
                htmlExport.HtmlWriter.Write("@page { size: ");
                if (page.PaperSize != PaperKind.Custom)
                {
                    htmlExport.HtmlWriter.Write(page.PaperSize.ToString());
                    if (page.Orientation == StiPageOrientation.Landscape) htmlExport.HtmlWriter.Write(" landscape");
                }
                else
                {
                    htmlExport.HtmlWriter.Write($"{pageWidth}mm {pageHeight}mm");
                }
                htmlExport.HtmlWriter.WriteLine("; margin: 0; }");
                if (htmlExport.useFullExceedMargins)
                {
                    htmlExport.HtmlWriter.Write($"@media print {{ html,body {{ width: {pageWidth}mm; height: {pageHeight}mm; }} ");
                    htmlExport.HtmlWriter.WriteLine($".pagemargins {{ padding: 0; border:0; }} }}");
                }
                else
                {
                    htmlExport.HtmlWriter.Write($"@media print {{ html,body {{ width: {pageWidth - mgLeft - mgRight}mm; height: {pageHeight - mgTop - mgBottom}mm; }} ");
                    htmlExport.HtmlWriter.WriteLine($".pagemargins {{ padding: {mgTop}mm {mgRight}mm {mgBottom / 2}mm {mgLeft}mm; border:0; }} }}");
                }
            }
            if (addStyleTag) htmlExport.HtmlWriter.WriteLine("</style>");
		}

		private double GetWidth(IList listX, int columnIndex, double zoom)
		{
            return ((double)listX[columnIndex + 1] - (double)listX[columnIndex]) * zoom;
		}

		private double GetHeight(IList listY, int rowIndex, double zoom)
		{
			return ((double)listY[rowIndex + 1] - (double)listY[rowIndex]) * zoom;
		}

        //private string PrepareTextForHtml(string text)
        //{
        //    if (text == null) return null;
        //    //return text.Replace("\n", "<br>");

        //    string[] txt = text.Split(new char[] {'\n'});
        //    StringBuilder sbFull = new StringBuilder();
        //    for (int index = 0; index < txt.Length; index++)
        //    {
        //        string st = txt[index];
        //        int pos = 0;
        //        while ((pos < st.Length) && (st[pos] == ' ')) pos++;
        //        if (pos > 0)
        //        {
        //            for (int indexSp = 0; indexSp < pos; indexSp++)
        //            {
        //                sbFull.Append("&nbsp;");
        //            }
        //            sbFull.Append(st.Substring(pos));
        //        }
        //        else
        //        {
        //            sbFull.Append(st);
        //        }
        //        if (index < txt.Length - 1)
        //        {
        //            sbFull.Append("<br>");
        //        }
        //    }
        //    return sbFull.ToString();
        //}

        public void RenderTable(bool renderStyles, string backGroundImageString, bool useBookmarks, bool exportBookmarksOnly, Hashtable cssStyles, bool watermarkShowBehind = false, StiBorder border = null)
        {
            if (renderStyles) RenderStylesTable(useBookmarks, exportBookmarksOnly, cssStyles);

            StiHtmlTable table = new StiHtmlTable();

            table.Align = htmlExport.PageHorAlignment;

            table.BackImageUrl = backGroundImageString;

            table.htmlExport = htmlExport;

            if (watermarkShowBehind) table.Position = "relative";

            table.Width = StiHtmlUnit.NewUnit(Math.Round(matrix.TotalWidth * htmlExport.zoom, 0), StiOptions.Export.Html.PrintLayoutOptimization);

            table.Border = border;
            table.BorderWidth = 0;
            table.CellPadding = 0;
            table.CellSpacing = 0;

            IList listX = matrix.CoordX.GetValueList();
            IList listY = matrix.CoordY.GetValueList();

            bool[,] lines = new bool[matrix.CoordX.Count, matrix.CoordY.Count];

            Hashtable hashStyles = new Hashtable();
            foreach (var hashStyle in matrix.Styles)
            {
                hashStyles[hashStyle] = matrix.Styles.IndexOf(hashStyle);
            }

            htmlExport.StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");
            double progressScale = Math.Max(matrix.CoordY.Count / 200f, 1f);
            int progressValue = 0;

            for (int rowIndex = 0; rowIndex < matrix.CoordY.Count - 1; rowIndex++)
            {
                int currentProgress = (int)(rowIndex / progressScale);
                if (currentProgress > progressValue)
                {
                    progressValue = currentProgress;
                    htmlExport.InvokeExporting(rowIndex, matrix.CoordY.Count, 2, 3);
                }
                if (htmlExport.IsStopped) return;

                double rowHeight = GetHeight(listY, rowIndex, htmlExport.zoom);

                #region Render Row
                StiHtmlTableRow row = new StiHtmlTableRow();
                row.Height = StiHtmlUnit.NewUnit(rowHeight, StiOptions.Export.Html.PrintLayoutOptimization);
                table.Rows.Add(row);

                #region AddPageBreakAfterCss
                if (htmlExportSettings.AddPageBreaks)
                {
                    if (matrix.HorizontalPageBreaks.Contains(rowIndex))
                    {
                        row.Style[StiHtmlTable.PageBreakBeforeKey] = "always";
                    }
                }
                #endregion

                Color[] cellsColors = new Color[matrix.CoordX.Count - 1];

                //get maximum horizontal border width in current row
                double maxBorderWidth = 0;
                for (int columnIndex = 0; columnIndex < matrix.CoordX.Count - 1; columnIndex++)
                {
                    var borderSide = matrix.BordersX[rowIndex, columnIndex];
                    if ((borderSide != null) && (borderSide.Style != StiPenStyle.None))
                    {
                        maxBorderWidth = Math.Max(maxBorderWidth, borderSide.GetSize());
                    }
                }

                List<StiMatrix.RoundedRectangleInfo> listPrimitivesInRow = null;
                matrix.RoundedRectanglesList.TryGetValue(rowIndex, out listPrimitivesInRow);

                for (int columnIndex = 0; columnIndex < matrix.CoordX.Count - 1; columnIndex++)
                {
                    if (!lines[columnIndex, rowIndex])
                    {
                        #region Render Column
                        StiHtmlTableCell cell = new StiHtmlTableCell();
                        row.Cells.Add(cell);

                        double width = GetWidth(listX, columnIndex, htmlExport.zoom);
                        double height = rowHeight;

                        StiCell matrixCell = matrix.Cells[rowIndex, columnIndex];

                        if (matrixCell != null)
                        {
                            string cellText = (matrixCell.Text != null ? matrixCell.Text : string.Empty);

                            IStiTextOptions textOptions = matrixCell.Component as IStiTextOptions;
                            if (textOptions != null &&
                                StiOptions.Export.Html.ConvertDigitsToArabic &&
                                textOptions.TextOptions.RightToLeft)
                            {
                                cellText = StiExportUtils.ConvertDigitsToArabic(cellText, StiOptions.Export.Html.ArabicDigitsType);
                            }

                            StiText stiText = matrixCell.Component as StiText;

                            bool flag = true;
                            if (matrixCell.Component != null)
                            {
                                #region Prepare cellText
                                if ((stiText != null) && (stiText.CheckAllowHtmlTags()))
                                {
                                    cellText = StiHtmlExportService.ConvertTextWithHtmlTagsToHtmlText(stiText, cellText, htmlExport.zoom);
                                    flag = false;
                                }

                                //added 15.04.2008
                                if (StiOptions.Export.Html.ForceWysiwygWordwrap &&
                                    (stiText != null) &&
                                    (!stiText.CheckAllowHtmlTags()) &&
                                    (stiText.TextQuality == StiTextQuality.Wysiwyg) &&
                                    (textOptions != null) &&
                                    (textOptions.TextOptions.WordWrap))
                                {
                                    var newTextLines = StiTextRenderer.GetTextLines(
                                        htmlExport.report.ReportMeasureGraphics,
                                        ref cellText,
                                        stiText.Font,
                                        htmlExport.report.Unit.ConvertToHInches(matrixCell.Component.ComponentToPage(matrixCell.Component.ClientRectangle)),
                                        1,
                                        true,
                                        textOptions.TextOptions.RightToLeft,
                                        1,
                                        textOptions.TextOptions.Angle,
                                        textOptions.TextOptions.Trimming,
                                        stiText.CheckAllowHtmlTags(),
                                        textOptions.TextOptions,
                                        1);
                                    var sb = new StringBuilder();
                                    for (int index = 0; index < newTextLines.Count; index++)
                                    {
                                        string st = newTextLines[index];
                                        sb.Append(st);
                                        if (index < newTextLines.Count - 1) sb.Append("\n");
                                    }
                                    cellText = sb.ToString();
                                }
                                #endregion
                            }

                            if (StiOptions.Export.Html.ReplaceSpecialCharacters && flag)
                            {
                                cellText = cellText.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\xA0", "&nbsp;");
                            }

                            #region Colspan & rowspan
                            int colSpan = matrixCell.Width + 1;
                            int rowSpan = matrixCell.Height + 1;

                            for (int cIndex = columnIndex; cIndex < (columnIndex + colSpan); cIndex++)
                            {
                                for (int rIndex = rowIndex; rIndex < (rowIndex + rowSpan); rIndex++)
                                {
                                    lines[cIndex, rIndex] = true;
                                }
                            }
                            if (colSpan > 1) cell.ColumnSpan = colSpan;
                            if (rowSpan > 1) cell.RowSpan = rowSpan;
                            #endregion

                            #region Render Style
                            StiCellStyle cellStyle = matrix.CellStyles[rowIndex, columnIndex];
                            if (cellStyle == null) cellStyle = matrixCell.CellStyle;
                            if (cellStyle != null)
                            {
                                object styleObj = hashStyles[cellStyle];
                                if ((styleObj != null) && (htmlExport.useStylesTable))
                                {
                                    //cell.CssClass = "s" + styleIndex.ToString();
                                    cell.CssClass = "s" + cellStyle.StyleName;
                                }

                                if (StiOptions.Export.Html.UseStrictTableCellSize)
                                {
                                    if (cellStyle.VertAlignment != StiVertAlignment.Top)
                                    {
                                        cell.Style[StiHtmlTable.VertAlignKey] = cellStyle.VertAlignment == StiVertAlignment.Center ? "middle" : "bottom";
                                    }

                                    bool rightToLeft = (cellStyle.TextOptions != null) && (cellStyle.TextOptions.RightToLeft);
                                    string textAlign = null;
                                    if (cellStyle.HorAlignment == StiTextHorAlignment.Left)
                                    {
                                        textAlign = (!rightToLeft ? null : "right");
                                    }
                                    if (cellStyle.HorAlignment == StiTextHorAlignment.Right)
                                    {
                                        textAlign = (rightToLeft ? null : "right");
                                    }
                                    if (cellStyle.HorAlignment == StiTextHorAlignment.Center) textAlign = "center";
                                    if (cellStyle.HorAlignment == StiTextHorAlignment.Width) textAlign = "justify";
                                    if (textAlign != null)
                                    {
                                        cell.Style[StiHtmlTable.HorAlignKey] = textAlign;
                                    }

                                    for (int index = 0; index < colSpan; index++)
                                    {
                                        cellsColors[columnIndex + index] = cellStyle.Color;
                                    }
                                }
                            }
                            if (matrixCell.Component != null)
                            {
                                string sTag = matrixCell.Component.TagValue as string;
                                if (!string.IsNullOrEmpty(sTag))
                                {
                                    string[] sTagArray = matrix.SplitTagWithCache(sTag);
                                    for (int index = 0; index < sTagArray.Length; index++)
                                    {
                                        if (sTagArray[index].ToLower().StartsWith("css", StringComparison.InvariantCulture))
                                        {
                                            string[] stArr = StiMatrix.GetStringsFromTag(sTagArray[index], 3);
                                            if ((stArr.Length > 1) && (htmlExport.useStylesTable))
                                            {
                                                cell.CssClass = stArr[0].Trim();
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            if (!htmlExport.useStylesTable)
                            {
                                RenderStyleTable(cell, cellStyle);
                            }
                            else
                            {
                                if (StiOptions.Export.Html.UseExtendedStyle)
                                {
                                    htmlExport.RenderTextColor(cell, cellStyle.TextColor, true);
                                    htmlExport.RenderFont(cell, cellStyle.Font);
                                    //htmlExport.RenderTextDirection(cell, cellStyle.TextOptions);
                                }
                            }
                            #endregion

                            if (StiOptions.Export.Html.PreserveWhiteSpaces && !string.IsNullOrWhiteSpace(cellText) && cellText.Contains("  ") && !(cellStyle != null && cellStyle.HorAlignment == StiTextHorAlignment.Width))
                            {
                                cell.Style["white-space"] = "pre-wrap";
                                cellText = htmlExport.PrepareTextForHtml(cellText, false, false);
                            }
                            else
                            {
                                cellText = htmlExport.PrepareTextForHtml(cellText, true, cellStyle != null && cellStyle.HorAlignment == StiTextHorAlignment.Width);
                            }

                            //fix for border width
                            if (StiOptions.Export.Html.PrintLayoutOptimization && (cellStyle != null) && (cellStyle.Border != null) && (cellStyle.Border.Style != StiPenStyle.None))
                            {
                                height -= cellStyle.Border.Size;
                                width -= cellStyle.Border.Size;
                                if (height < 0) height = 0;
                                if (width < 0) width = 0;
                            }

                            if (htmlExport.exportQuality == StiHtmlExportQuality.High)
                            {
                                if (StiOptions.Export.Html.ForceIE6Compatibility)
                                {
                                    if (colSpan == 1) cell.Width = StiHtmlUnit.NewUnit(width, StiOptions.Export.Html.PrintLayoutOptimization);
                                    if (rowSpan == 1) cell.Height = StiHtmlUnit.NewUnit(height, StiOptions.Export.Html.PrintLayoutOptimization);
                                }
                                else
                                {
                                    // following code work perfectly for FireFox, but not correctly for IE6
                                    if (colSpan > 1)
                                    {
                                        for (int indexColSpan = 1; indexColSpan < colSpan; indexColSpan++)
                                        {
                                            width += GetWidth(listX, columnIndex + indexColSpan, htmlExport.zoom);
                                        }
                                    }
                                    if (rowSpan > 1)
                                    {
                                        for (int indexRowSpan = 1; indexRowSpan < rowSpan; indexRowSpan++)
                                        {
                                            height += GetHeight(listY, rowIndex + indexRowSpan, htmlExport.zoom);
                                        }
                                    }
                                    cell.Width = StiHtmlUnit.NewUnit(width, StiOptions.Export.Html.PrintLayoutOptimization);
                                    cell.Height = StiHtmlUnit.NewUnit(height, StiOptions.Export.Html.PrintLayoutOptimization);

                                    if (StiOptions.Export.Html.UseStrictTableCellSizeV2 && cellText.Length > 3)
                                    {
                                        cell.Style["max-width"] = cell.Width.ToString();
                                        cell.Style["max-height"] = cell.Height.ToString();
                                    }
                                }
                            }

                            #region HTML Viewer Interactions
                            if (htmlExport.RenderWebInteractions && matrixCell.Component != null &&
                                (matrix.IsComponentHasInteraction(matrixCell.Component) || matrix.Interactions[rowIndex, columnIndex, 1] > 0))
                            {
                                StiComponent comp = matrixCell.Component;

                                if (!matrix.IsComponentHasInteraction(comp))
                                {
                                    StiPage page = htmlExport.report.RenderedPages[matrix.Interactions[rowIndex, columnIndex, 0] - 1];
                                    htmlExport.report.RenderedPages.GetPage(page);
                                    comp = page.Components[matrix.Interactions[rowIndex, columnIndex, 1] - 1];
                                }

                                cell.Interaction = comp.Name;

                                // Sorting
                                if (comp.Interaction.SortingEnabled)
                                {
                                    string dataBandName = comp.Interaction.GetSortDataBandName();
                                    StiDataBand dataBand = comp.Report.GetComponentByName(dataBandName) as StiDataBand;
                                    if (dataBand != null)
                                    {
                                        cell.DataBandSort = dataBandName + ";" + string.Join(";", dataBand.Sort);

                                        var index = 0;
                                        string sortDirection = string.Empty;
                                        while (index < dataBand.Sort.Length)
                                        {
                                            var columnName = string.Empty;
                                            sortDirection = dataBand.Sort[index++];

                                            while (
                                                index < dataBand.Sort.Length &&
                                                dataBand.Sort[index] != "ASC" &&
                                                dataBand.Sort[index] != "DESC")
                                            {
                                                if (columnName.Length == 0)
                                                    columnName = dataBand.Sort[index];
                                                else
                                                    columnName += '.' + dataBand.Sort[index];
                                                index++;
                                            }

                                            if (columnName == comp.Interaction.GetSortColumnsString())
                                            {
                                                cell.SortDirection = sortDirection.ToLower();
                                            }
                                        }
                                    }
                                }

                                // Drill-down
                                if (comp.Interaction.DrillDownEnabled && (!string.IsNullOrEmpty(comp.Interaction.DrillDownPageGuid) || !string.IsNullOrEmpty(comp.Interaction.DrillDownReport)))
                                {
                                    if (!string.IsNullOrEmpty(comp.Interaction.DrillDownPageGuid)) cell.PageGuid = comp.Interaction.DrillDownPageGuid;
                                    if (!string.IsNullOrEmpty(comp.Interaction.DrillDownReport)) cell.ReportFile = comp.Interaction.DrillDownReport;
                                    cell.PageIndex = comp.Page.Report.RenderedPages.IndexOf(comp.Page).ToString();
                                    cell.ComponentIndex = comp.Page.Components.IndexOf(comp).ToString();
                                    cell.DrillDownMode = comp.Interaction.DrillDownMode.ToString();
                                }

                                // Collapsing
                                StiBandInteraction bandInteraction = comp.Interaction as StiBandInteraction;
                                if (bandInteraction != null && bandInteraction.CollapsingEnabled && comp is StiContainer)
                                {
                                    StiContainer cont = comp as StiContainer;
                                    cell.Collapsed = StiDataBandV2Builder.IsCollapsed(cont, false).ToString().ToLower();
                                    cell.ComponentIndex = cont.CollapsingIndex.ToString();
                                }
                            }
                            #endregion

                            string hyperlinkValue = null;
                            string tooltipValue = null;
                            string bookmarkValue = null;
                            string bookmarkGuid = null;

                            bool isChart = false;
                            bool isSparkline = false;
                            bool isGauge = false;
                            bool isMap = false;

                            IStiExportImage exportImage = matrixCell.ExportImage;

                            if (matrixCell.Component != null)
                            {
                                #region Process component
                                hyperlinkValue = matrixCell.Component.HyperlinkValue as string;
                                tooltipValue = matrixCell.Component.ToolTipValue as string;
                                bookmarkValue = matrixCell.Component.BookmarkValue as string;

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
                                        string stt = (string)matrix.pointerToBookmark[hyperlinkValue.Substring(1)];
                                        if (stt != null) hyperlinkValue = "#" + stt;
                                    }
                                    else
                                    {
                                        hyperlinkValue = hyperlinkValue.Substring(1);
                                    }
                                }

                                if (string.IsNullOrEmpty(bookmarkValue))
                                {
                                    #region check range
                                    for (int yy = 0; yy <= matrixCell.Height; yy++)
                                    {
                                        bool breakLoop = false;
                                        for (int xx = 0; xx <= matrixCell.Width; xx++)
                                        {
                                            string bkm = matrix.Bookmarks[rowIndex + yy, columnIndex + xx];
                                            if (!string.IsNullOrEmpty(bkm))
                                            {
                                                bookmarkValue = bkm;
                                                breakLoop = true;
                                                break;
                                            }
                                        }
                                        if (breakLoop) break;
                                    }
                                    #endregion
                                }
                                if (!string.IsNullOrWhiteSpace(matrixCell.Component.Guid) && htmlExport.hashBookmarkGuid.ContainsKey(matrixCell.Component.Guid))
                                {
                                    bookmarkGuid = matrixCell.Component.Guid;
                                }

                                if (StiOptions.Export.Html.AllowStrippedImages && exportImage == null)
                                {
                                    IStiExportImage exp = matrixCell.Component as IStiExportImage;
                                    if (exp != null && exp is IStiExportImageExtended)
                                    {
                                        if (((IStiExportImageExtended)exp).IsExportAsImage(StiExportFormat.HtmlTable) && (textOptions == null || textOptions.TextOptions.Angle == 0))
                                            exportImage = exp;
                                    }
                                }

                                var chart = matrixCell.Component as Stimulsoft.Report.Chart.StiChart;
                                if ((chart != null) && (htmlExport.chartType != StiHtmlChartType.Image))
                                {
                                    isChart = true;
                                }

                                var sparkline = matrixCell.Component as StiSparkline;
                                if ((sparkline != null) && (htmlExport.chartType != StiHtmlChartType.Image))
                                {
                                    isSparkline = true;
                                }

                                var gauge = matrixCell.Component as Stimulsoft.Report.Gauge.StiGauge;
                                if ((gauge != null) && (htmlExport.chartType != StiHtmlChartType.Image))
                                {
                                    isGauge = true;
                                }

                                var map = matrixCell.Component as Stimulsoft.Report.Maps.StiMap;
                                if ((map != null) && (htmlExport.chartType != StiHtmlChartType.Image) && map.MapMode != Maps.StiMapMode.Online)
                                {
                                    isMap = true;
                                }

                                var editable = matrixCell.Component as IStiEditable;
                                if (editable != null && editable.Editable)
                                {
                                    StringBuilder attr = new StringBuilder();
                                    int editableIndex = matrixCell.Component.Page.Components.IndexOf(matrixCell.Component);
                                    attr.AppendFormat("{0};", editableIndex);

                                    var checkBox = matrixCell.Component as StiCheckBox;
                                    if (checkBox != null)
                                    {
                                        Color backColor = Color.Transparent;
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

                                    if (stiText != null)
                                    {
                                        attr.AppendFormat("Text");
                                        if (stiText.CheckAllowHtmlTags())
                                        {
                                            attr.AppendFormat(";html");
                                        }
                                    }

                                    var richTextBox = matrixCell.Component as StiRichText;
                                    if (richTextBox != null)
                                    {
                                        attr.AppendFormat("RichText");
                                    }

                                    cell.Editable = attr.ToString();
                                }

                                if (exportImage == null)
                                {
                                    StiGradientBrush grb = null;
                                    StiGlareBrush glb = null;
                                    StiGlassBrush gsb = null;

                                    if (stiText != null)
                                    {
                                        if (stiText.Brush is StiGradientBrush) grb = stiText.Brush as StiGradientBrush;
                                        if (stiText.Brush is StiGlareBrush) glb = stiText.Brush as StiGlareBrush;
                                        if (stiText.Brush is StiGlassBrush) gsb = stiText.Brush as StiGlassBrush;
                                    }
                                    var cont = matrixCell.Component as StiContainer;
                                    if (cont != null)
                                    {
                                        if (cont.Brush is StiGradientBrush) grb = cont.Brush as StiGradientBrush;
                                        if (cont.Brush is StiGlareBrush) glb = cont.Brush as StiGlareBrush;
                                        if (cont.Brush is StiGlassBrush) gsb = cont.Brush as StiGlassBrush;
                                    }

                                    if (grb != null || glb != null)
                                    {
                                        string backgroundStyle = "";
                                        if (grb != null)
                                        {
                                            string stAngle = $"{Math.Round(grb.Angle) + 90}deg";
                                            backgroundStyle = $"linear-gradient({stAngle}, {StiHtmlExportService.FormatColor(grb.StartColor)}, {StiHtmlExportService.FormatColor(grb.EndColor)})";
                                        }
                                        if (glb != null)
                                        {
                                            string stAngle = $"{Math.Round(glb.Angle) + 90}deg";
                                            backgroundStyle = $"linear-gradient({stAngle}, {StiHtmlExportService.FormatColor(glb.StartColor)}, {StiHtmlExportService.FormatColor(glb.EndColor)}  {Math.Round(glb.Focus * 100)}%, {StiHtmlExportService.FormatColor(glb.StartColor)})";
                                        }

                                        var rectt = matrix.GetRange(htmlExport.report.Unit.ConvertToHInches(matrixCell.Component.ClientRectangle));
                                        if (rectt.Top != -1)    //fix; the code below is not quite correct, ClientRectangle will work correctly only for the first page, for others you need to add an page offset. so for now I just add a check
                                        {
                                        bool hasLeftDiff = matrixCell.Left != rectt.Left;
                                        bool hasTopDiff = matrixCell.Top != rectt.Top;
                                        if (hasLeftDiff || hasTopDiff)
                                        {
                                            double leftDiff = ((double)listX[rectt.Left] - (double)listX[columnIndex]) * htmlExport.zoom;
                                            double topDiff = ((double)listY[rectt.Top] - (double)listY[rowIndex]) * htmlExport.zoom;
                                            backgroundStyle += $" {StiHtmlUnit.NewUnit(leftDiff)} {StiHtmlUnit.NewUnit(topDiff)}";
                                        }
                                        if (rectt.Width != matrixCell.Width + 1 || rectt.Height != matrixCell.Height + 1)
                                        {
                                            double componentWidth = ((double)listX[rectt.Right] - (double)listX[rectt.Left]) * htmlExport.zoom;
                                            double componentHeight = ((double)listY[rectt.Bottom] - (double)listY[rectt.Top]) * htmlExport.zoom;

                                            // background-size MUST follow background but due to usage of hashtable I cannot guarantee this by separate style
                                            backgroundStyle += $"; background-size: {StiHtmlUnit.NewUnit(componentWidth)} {StiHtmlUnit.NewUnit(componentHeight)}";
                                        }
                                        }

                                        cell.Style["background"] = backgroundStyle;
                                    }
                                    
                                    if (gsb != null)
                                    {
                                        cell.Style["background"] = $"linear-gradient({StiHtmlExportService.FormatColor(gsb.GetTopColor())}, {StiHtmlExportService.FormatColor(gsb.GetTopColor())} 49%, {StiHtmlExportService.FormatColor(gsb.GetBottomColor())} 50%, {StiHtmlExportService.FormatColor(gsb.GetBottomColor())});";
                                    }
                                }
                                #endregion
                            }

                            #region Render Bookmark
                            bool notImageProcessed = true;
                            if (!string.IsNullOrWhiteSpace(bookmarkValue) || !string.IsNullOrEmpty(bookmarkGuid))
                            {
                                StiHtmlHyperlink bookmark = new StiHtmlHyperlink();
                                if (!string.IsNullOrWhiteSpace(bookmarkValue))
                                {
                                    //bookmark.Attributes["name"] = "#" + bookmarkValue;
                                    bookmark.Attributes["name"] = bookmarkValue;
                                }
                                if (!string.IsNullOrEmpty(bookmarkGuid))
                                {
                                    bookmark.Attributes["guid"] = bookmarkGuid;
                                }
                                //bookmark.Text = cellText;
                                bookmark.ToolTip = tooltipValue;
                                bookmark.NavigateUrl = hyperlinkValue;
                                //bookmark.CssClass = cell.CssClass;
                                bookmark.OpenLinksTarget = htmlExport.openLinksTarget;
                                bookmark.ImageComp = matrixCell.Component as StiImage;
                                cell.ToolTip = tooltipValue;

                                if (isChart)
                                {
                                    cell.Id = htmlExport.GetGuid(matrixCell.Component);
                                    cell.Text = htmlExport.PrepareChartData(null, matrixCell.Component as Stimulsoft.Report.Chart.StiChart, width, height);
                                }
                                else if (isGauge)
                                {
                                    cell.Id = htmlExport.GetGuid(matrixCell.Component);
                                    cell.Text = htmlExport.PrepareGaugeData(null, matrixCell.Component as Stimulsoft.Report.Gauge.StiGauge, width, height);
                                }
                                else if (isMap)
                                {
                                    cell.Id = htmlExport.GetGuid(matrixCell.Component);
                                    cell.Text = htmlExport.PrepareMapData(null, matrixCell.Component as Stimulsoft.Report.Maps.StiMap, width, height);
                                }
                                else if (isSparkline)
                                {
                                    cell.Id = htmlExport.GetGuid(matrixCell.Component);
                                    cell.Text = htmlExport.PrepareSparklineData(null, matrixCell.Component as StiSparkline, width, height);
                                }
                                else if (exportImage != null)
                                {
                                    float zoom = (float)htmlExport.zoom;

                                    float resolution = htmlExport.imageResolution;
                                    var imageComp = exportImage as StiImage;
                                    if (StiOptions.Export.Html.UseImageResolution && imageComp != null && imageComp.ExistImageToDraw())
                                    {
                                        using (var gdiImage = imageComp.TakeGdiImageToDraw())
                                        {
                                            if (gdiImage != null)
                                            {
                                                var dpix = gdiImage.HorizontalResolution;
                                                if (dpix >= 50 && dpix <= 1250) resolution = dpix;
                                            }
                                        }
                                    }
                                    if (resolution != 100) zoom *= resolution / 100f;

                                    Image image = null;

                                    htmlExport.SetCurrentCulture();
                                    IStiExportImageExtended exportImageExtended = exportImage as IStiExportImageExtended;

                                    if (exportImageExtended != null && exportImageExtended.IsExportAsImage(StiExportFormat.HtmlTable))
                                    {
                                        if ((htmlExport.imageFormat == null || htmlExport.imageFormat == ImageFormat.Png) && (imageComp == null))
                                            image = exportImageExtended.GetImage(ref zoom, StiExportFormat.ImagePng);
                                        else
                                            image = exportImageExtended.GetImage(ref zoom, StiExportFormat.HtmlTable);
                                    }
                                    else
                                        image = exportImage.GetImage(ref zoom);

                                    htmlExport.RestoreCulture();

                                    if (image != null)
                                    {
                                        Image imgPart = matrix.GetRealImageData(matrixCell, image);
                                        if (imgPart != null) image = imgPart;

                                        if (htmlExport.HtmlImageHost != null)
                                        {
                                            bookmark.ImageUrl = htmlExport.HtmlImageHost.GetImageString(image as Bitmap);
                                        }
                                        //bookmark.CssClass = cell.CssClass;
                                        bookmark.Width = StiHtmlUnit.NewUnit(image.Width / zoom * htmlExport.zoom, StiOptions.Export.Html.PrintLayoutOptimization);
                                        bookmark.Height = StiHtmlUnit.NewUnit(image.Height / zoom * htmlExport.zoom, StiOptions.Export.Html.PrintLayoutOptimization);

                                        image.Dispose();

                                        notImageProcessed = false;
                                    }
                                }

                                if (notImageProcessed && !isChart)
                                {
                                    bookmark.Text = cellText;

                                    if ((matrixCell.Component != null) && (matrixCell.Component is IStiTextBrush))
                                    {
                                        IStiTextBrush textBrush = matrixCell.Component as IStiTextBrush;
                                        Color color = StiBrush.ToColor(textBrush.TextBrush);
                                        bookmark.Style["color"] = StiHtmlExportService.FormatColor(color);
                                    }
                                    if ((matrixCell.Component != null) && (matrixCell.Component is IStiFont))
                                    {
                                        IStiFont font = matrixCell.Component as IStiFont;
                                        if (font.Font.Underline)
                                        {
                                            bookmark.Style["text-decoration"] = "underline";
                                        }
                                        else
                                        {
                                            bookmark.Style["text-decoration"] = "none";
                                        }
                                    }
                                }

                                cell.Controls.Add(bookmark);
                            }
                            #endregion

                            #region Render Chart
                            else if (isChart)
                            {
                                if (!string.IsNullOrEmpty(hyperlinkValue))
                                {
                                    StiHtmlHyperlink hyperlink = new StiHtmlHyperlink();
                                    hyperlink.ToolTip = tooltipValue;
                                    hyperlink.NavigateUrl = hyperlinkValue;
                                    hyperlink.OpenLinksTarget = htmlExport.openLinksTarget;
                                    hyperlink.Style["display"] = "block";

                                    cell.Controls.Add(hyperlink);
                                }
                                else
                                {
                                    cell.ToolTip = tooltipValue;
                                }

                                cell.Id = htmlExport.GetGuid(matrixCell.Component);
                                cell.Text = htmlExport.PrepareChartData(null, matrixCell.Component as Stimulsoft.Report.Chart.StiChart, width, height);
                            }
                            #endregion
                            else if (isSparkline)
                            {
                                if (!string.IsNullOrEmpty(hyperlinkValue))
                                {
                                    StiHtmlHyperlink hyperlink = new StiHtmlHyperlink();
                                    hyperlink.ToolTip = tooltipValue;
                                    hyperlink.NavigateUrl = hyperlinkValue;
                                    hyperlink.OpenLinksTarget = htmlExport.openLinksTarget;
                                    hyperlink.Style["display"] = "block";
                                    cell.Controls.Add(hyperlink);
                                }
                                else
                                {
                                    cell.ToolTip = tooltipValue;
                                }
                                cell.Id = htmlExport.GetGuid(matrixCell.Component);
                                cell.Text = htmlExport.PrepareSparklineData(null, matrixCell.Component as StiSparkline, width, height);
                            }
                            else if (isGauge)
                            {
                                if (!string.IsNullOrEmpty(hyperlinkValue))
                                {
                                    StiHtmlHyperlink hyperlink = new StiHtmlHyperlink();
                                    hyperlink.ToolTip = tooltipValue;
                                    hyperlink.NavigateUrl = hyperlinkValue;
                                    hyperlink.OpenLinksTarget = htmlExport.openLinksTarget;
                                    hyperlink.Style["display"] = "block";
                                    cell.Controls.Add(hyperlink);
                                }
                                else
                                {
                                    cell.ToolTip = tooltipValue;
                                }
                                cell.Id = htmlExport.GetGuid(matrixCell.Component);
                                cell.Text = htmlExport.PrepareGaugeData(null, matrixCell.Component as Stimulsoft.Report.Gauge.StiGauge, width, height);
                            }
                            else if (isMap)
                            {
                                cell.Id = htmlExport.GetGuid(matrixCell.Component);
                                cell.Text = htmlExport.PrepareMapData(null, matrixCell.Component as Stimulsoft.Report.Maps.StiMap, width, height);
                            }

                            #region Render BarCode
                            else if (matrixCell.Component is StiBarCode ||
                                matrixCell.Component is StiCheckBox ||
                                matrixCell.Component is StiShape ||
                                (isChart && this.htmlExportSettings.ChartType != StiHtmlChartType.AnimatedVector))
                            {
                                var comp = matrixCell.Component as StiComponent;
                                var htmlSvg = new StiHtmlSvg();
                                var contentSvg = StiSvgHelper.SaveComponentToString(comp);
                                var unit = comp.Report.Unit;
                                if (comp.Page != null)
                                    unit = comp.Page.Unit;

                                double x1 = StiMatrix.htmlScaleX * unit.ConvertToHInches(comp.Left);
                                double y1 = StiMatrix.htmlScaleY * unit.ConvertToHInches(comp.Top);
                                double x2 = StiMatrix.htmlScaleX * unit.ConvertToHInches(comp.Right);
                                double y2 = StiMatrix.htmlScaleY * unit.ConvertToHInches(comp.Bottom);

                                var svg = String.Format("<svg width=\"{0}\" height=\"{1}\"><g transform=\"scale({2})\">{3}</g></svg>", Math.Round(x2 - x1, 0), Math.Round(y2 - y1, 0), this.htmlExport.zoom, contentSvg);

                                htmlSvg.Text = svg;
                                cell.Controls.Add(htmlSvg);
                            }
                            #endregion

                            #region Render Text
                            else if (exportImage == null)
                            {
                                if (hyperlinkValue != null && hyperlinkValue.Length > 0)
                                {
                                    StiHtmlHyperlink hyperlink = new StiHtmlHyperlink();
                                    hyperlink.Text = cellText;
                                    hyperlink.ToolTip = tooltipValue;
                                    hyperlink.NavigateUrl = hyperlinkValue;
                                    hyperlink.OpenLinksTarget = htmlExport.openLinksTarget;
                                    //hyperlink.CssClass = cell.CssClass;
                                    hyperlink.Style["display"] = "block";
                                    if ((matrixCell.Component != null) && (matrixCell.Component is IStiTextBrush))
                                    {
                                        IStiTextBrush textBrush = matrixCell.Component as IStiTextBrush;
                                        Color color = StiBrush.ToColor(textBrush.TextBrush);
                                        hyperlink.Style["color"] = StiHtmlExportService.FormatColor(color);
                                    }
                                    if ((matrixCell.Component != null) && (matrixCell.Component is IStiFont))
                                    {
                                        IStiFont font = matrixCell.Component as IStiFont;
                                        if (font.Font.Underline)
                                        {
                                            hyperlink.Style["text-decoration"] = "underline";
                                        }
                                        else
                                        {
                                            hyperlink.Style["text-decoration"] = "none";
                                        }
                                    }
                                    cell.Controls.Add(hyperlink);
                                }
                                else
                                {
                                    if ((stiText != null) && (stiText.TextOptions != null && stiText.Angle != 0 /* || stiText.Indicator != null */))
                                    {
                                        var contentSvg = StiSvgHelper.SaveComponentToString(stiText);

                                        var svg = string.Format("<svg width=\"{0}\" height=\"{1}\"><g transform=\"scale({2})\">{3}</g></svg>", Math.Ceiling(width), Math.Ceiling(height), htmlExport.zoom, contentSvg);
                                        cell.Text = svg;

                                        cell.Style["line-height"] = "0";
                                    }
                                    else
                                    {
                                        cell.Text = cellText;
                                    }
                                    cell.ToolTip = tooltipValue;
                                }
                            }
                            #endregion

                            #region Render Image
                            else
                            {
                                float zoom = (float)htmlExport.zoom;

                                var stiImage = matrixCell.Component as StiImage;
                                if ((stiImage != null) && (stiImage.Icon != null))
                                {
                                    StiHtmlImage img = new StiHtmlImage();
                                    img.Zoom = zoom;
                                    img.ToolTip = tooltipValue;
                                    img.Width = StiHtmlUnit.NewUnit(width, false);
                                    img.Height = StiHtmlUnit.NewUnit(height, false);
                                    img.ImageUrl = StiSvgHelper.RenderIcon(stiImage, zoom, width, height);
                                    cell.Controls.Add(img);
                                }
                                else if (StiSvgHelper.CheckShape(matrixCell.Component))
                                {
                                    StiShape shape = matrixCell.Component as StiShape;

                                    var rect = shape.ComponentToPage(shape.ClientRectangle).Normalize();
                                    rect = shape.Report.Unit.ConvertToHInches(rect).Multiply(zoom);
                                    rect.X = 0;
                                    rect.Y = 0;
                                    int imageWidth = (int)(rect.Width + 0.5) + 1 + (int)(shape.Size * zoom);
                                    int imageHeight = (int)(rect.Height + 0.5) + 1 + (int)(shape.Size * zoom);
                                    int shift = (int)Math.Round(shape.Size * zoom / 2);

                                    StiHtmlImage img = new StiHtmlImage();
                                    img.Zoom = zoom;
                                    img.ToolTip = tooltipValue;
                                    img.Margin = StiHtmlUnit.NewUnit(-shift);
                                    img.Width = StiHtmlUnit.NewUnit((imageWidth + shape.Size * zoom), false);
                                    img.Height = StiHtmlUnit.NewUnit((imageHeight + shape.Size * zoom), false);
                                    img.ImageUrl = StiSvgHelper.RenderShapeAsBase64(shape);
                                    cell.Style.Add("overflow", "visible");
                                    cell.Controls.Add(img);
                                }
                                else
                                {
                                    float resolution = htmlExport.imageResolution;
                                    var imageComp = exportImage as StiImage;
                                    if (StiOptions.Export.Html.UseImageResolution && imageComp != null && imageComp.ExistImageToDraw())
                                    {
                                        using (var gdiImage = imageComp.TakeGdiImageToDraw())
                                        {
                                            if (gdiImage != null)
                                            {
                                                var dpix = gdiImage.HorizontalResolution;
                                                if (dpix >= 50 && dpix <= 1250) resolution = dpix;
                                            }
                                        }
                                    }
                                    if (resolution != 100) zoom *= resolution / 100f;

                                    Image image = null;

                                    htmlExport.SetCurrentCulture();
                                    IStiExportImageExtended exportImageExtended = exportImage as IStiExportImageExtended;

                                    if (exportImageExtended != null && exportImageExtended.IsExportAsImage(StiExportFormat.HtmlTable))
                                    {
                                        if ((htmlExport.imageFormat == null || htmlExport.imageFormat == ImageFormat.Png) && (imageComp == null))
                                            image = exportImageExtended.GetImage(ref zoom, StiExportFormat.ImagePng);
                                        else
                                            image = exportImageExtended.GetImage(ref zoom, StiExportFormat.HtmlTable);
                                    }
                                    else
                                        image = exportImage.GetImage(ref zoom);

                                    htmlExport.RestoreCulture();

                                    if (image != null)
                                    {
                                        Image imgPart = null;// matrix.GetRealImageData(matrixCell, image);
                                        if (imgPart != null) image = imgPart;

                                        if (hyperlinkValue != null && hyperlinkValue.Length > 0)
                                        {
                                            StiHtmlHyperlink hyperlink = new StiHtmlHyperlink();
                                            hyperlink.NavigateUrl = hyperlinkValue;
                                            hyperlink.ToolTip = tooltipValue;
                                            if (htmlExport.HtmlImageHost != null)
                                            {
                                                hyperlink.ImageUrl = htmlExport.HtmlImageHost.GetImageString(image as Bitmap);
                                            }
                                            //hyperlink.CssClass = cell.CssClass;
                                            hyperlink.Width = StiHtmlUnit.NewUnit(image.Width / zoom * htmlExport.zoom, StiOptions.Export.Html.PrintLayoutOptimization);
                                            hyperlink.Height = StiHtmlUnit.NewUnit(image.Height / zoom * htmlExport.zoom, StiOptions.Export.Html.PrintLayoutOptimization);
                                            hyperlink.OpenLinksTarget = htmlExport.openLinksTarget;
                                            hyperlink.ImageComp = matrixCell.Component as StiImage;
                                            hyperlink.Zoom = zoom;
                                            cell.Controls.Add(hyperlink);
                                        }
                                        else
                                        {
                                            string imageURLStr = null;
                                            if (matrixCell.Component != null && matrixCell.Component is StiImage)
                                            {
                                                StiImage imageComponent = matrixCell.Component as StiImage;
                                                if (!(!imageComponent.Stretch || imageComponent.AspectRatio == true || imageComponent.ImageRotation != StiImageRotation.None))
                                                {
                                                    string stTemp = imageComponent.ImageURLValue as string;
                                                    if (!StiHyperlinkProcessor.IsResourceHyperlink(stTemp))
                                                    {
                                                        imageURLStr = stTemp;
                                                    }
                                                }
                                            }

                                            StiHtmlImage img = new StiHtmlImage();
                                            img.ImageComp = imageComp;
                                            img.Zoom = zoom;

                                            if (imageURLStr != null && imageURLStr.Length != 0) img.ImageUrl = imageURLStr;
                                            else
                                            {
                                                if (htmlExport.HtmlImageHost != null)
                                                {
                                                    img.ImageUrl = htmlExport.HtmlImageHost.GetImageString(image as Bitmap);
                                                }
                                            }
                                            img.ToolTip = tooltipValue;
                                            //img.Width = StiHtmlUnit.NewUnit(image.Width / zoom * htmlExport.zoom, StiOptions.Export.Html.PrintLayoutOptimization);
                                            //img.Height = StiHtmlUnit.NewUnit(image.Height / zoom * htmlExport.zoom, StiOptions.Export.Html.PrintLayoutOptimization);                                        
                                            StiShape shape = matrixCell.Component as StiShape;
                                            if (shape == null)
                                            {
                                                img.Width = StiHtmlUnit.NewUnit(image.Width / zoom * htmlExport.zoom, false);
                                                img.Height = StiHtmlUnit.NewUnit(image.Height / zoom * htmlExport.zoom, false);
                                                img.UseImageSize = StiOptions.Engine.BarcodeImproveQualityHtmlExport && exportImage is StiBarCode;
                                            }
                                            else
                                            {
                                                int shift = (int)Math.Round(shape.Size * zoom / 2);
                                                img.Margin = StiHtmlUnit.NewUnit(-shift);
                                                img.Width = StiHtmlUnit.NewUnit((image.Width + shape.Size * zoom) / zoom * htmlExport.zoom, false);
                                                img.Height = StiHtmlUnit.NewUnit((image.Height + shape.Size * zoom) / zoom * htmlExport.zoom, false);
                                                cell.Style.Add("overflow", "visible");
                                            }
                                            cell.Controls.Add(img);
                                        }

                                        image.Dispose();

                                        notImageProcessed = false;
                                    }
                                }
                            }
                            #endregion

                            #region Cell margins
                            if ((stiText != null) && (!stiText.Margins.IsEmpty) && notImageProcessed)
                            {
                                cell.Style[StiHtmlTable.MarginsKey] = string.Format("{0} {1} {2} {3}",
                                    StiHtmlUnit.NewUnit(Math.Truncate(stiText.Margins.Top * htmlExport.zoom)),
                                    StiHtmlUnit.NewUnit(Math.Truncate(stiText.Margins.Right * htmlExport.zoom)),
                                    StiHtmlUnit.NewUnit(Math.Truncate(stiText.Margins.Bottom * htmlExport.zoom)),
                                    StiHtmlUnit.NewUnit(Math.Truncate(stiText.Margins.Left * htmlExport.zoom)));

                                if ((cell.Width != null) && (stiText.Margins.Left + stiText.Margins.Right > 0)) cell.Width.Value -= Math.Truncate(stiText.Margins.Left * htmlExport.zoom) + Math.Truncate(stiText.Margins.Right * htmlExport.zoom);
                                if ((cell.Height != null) && (stiText.Margins.Top + stiText.Margins.Bottom > 0)) cell.Height.Value -= Math.Truncate(stiText.Margins.Top * htmlExport.zoom) + Math.Truncate(stiText.Margins.Bottom * htmlExport.zoom);
                            }
                            #endregion

                            if ((textOptions != null) && (textOptions.TextOptions.WordWrap) &&
                                (cellStyle != null) && (cellStyle.VertAlignment != StiVertAlignment.Top || cellStyle.HorAlignment != StiTextHorAlignment.Left))
                            {
                                cell.Style[StiHtmlTable.WordwrapKey] = null;
                            }

                            #region Process RoundedRectangle
                            if (listPrimitivesInRow != null)
                            {
                                foreach (var rrInfo in listPrimitivesInRow)
                                {
                                    if (rrInfo.Rect.Left == columnIndex)
                                    {
                                        double rWidth = width;
                                        double rHeight = height;
                                        bool fitToCell = (rrInfo.Rect.Width == matrixCell.Width + 1) && (rrInfo.Rect.Height == matrixCell.Height + 1);
                                        if (!fitToCell)
                                        {
                                            if (rrInfo.Rect.Width > 1)
                                            {
                                                for (int indexColSpan = 1; indexColSpan < rrInfo.Rect.Width; indexColSpan++)
                                                {
                                                    rWidth += GetWidth(listX, columnIndex + indexColSpan, htmlExport.zoom);
                                                }
                                            }
                                            if (rrInfo.Rect.Height > 1)
                                            {
                                                for (int indexRowSpan = 1; indexRowSpan < rrInfo.Rect.Height; indexRowSpan++)
                                                {
                                                    rHeight += GetHeight(listY, rowIndex + indexRowSpan, htmlExport.zoom);
                                                }
                                            }
                                        }
                                        int space = (int)Math.Round((double)(Math.Min((rHeight < rWidth ? rHeight : rWidth), 100) * rrInfo.Primitive.Round * 1.04));

                                        var htmlPrimitive = new StiHtmlRoundedRectangle();
                                        //htmlPrimitive.Primitive = rrInfo.Primitive;
                                        htmlPrimitive.Width = rWidth;
                                        htmlPrimitive.Height = rHeight;
                                        htmlPrimitive.FitToCell = fitToCell;
                                        htmlPrimitive.Style = htmlExport.GetBorderStyle(rrInfo.Primitive.Style);
                                        htmlPrimitive.Size = Math.Round((double)rrInfo.Primitive.Size, 2);
                                        htmlPrimitive.Color = StiHtmlExportService.FormatColor(rrInfo.Primitive.Color);
                                        htmlPrimitive.Round = space.ToString();
                                        cell.Controls.Add(htmlPrimitive);

                                        //stote vert and hor align
                                        if (cellStyle.VertAlignment != StiVertAlignment.Top)
                                        {
                                            cell.Style[StiHtmlTable.VertAlignKey] = cellStyle.VertAlignment == StiVertAlignment.Center ? "middle" : "bottom";
                                        }
                                        bool rightToLeft = (cellStyle.TextOptions != null) && cellStyle.TextOptions.RightToLeft;
                                        string textAlign = null;
                                        if (cellStyle.HorAlignment == StiTextHorAlignment.Left)
                                        {
                                            textAlign = (!rightToLeft ? null : "right");
                                        }
                                        if (cellStyle.HorAlignment == StiTextHorAlignment.Right)
                                        {
                                            textAlign = (rightToLeft ? null : "right");
                                        }
                                        if (cellStyle.HorAlignment == StiTextHorAlignment.Center) textAlign = "center";
                                        if (cellStyle.HorAlignment == StiTextHorAlignment.Width) textAlign = "justify";
                                        if (textAlign != null)
                                        {
                                            cell.Style[StiHtmlTable.HorAlignKey] = textAlign;
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            if (StiOptions.Export.Html.PrintLayoutOptimization && (maxBorderWidth > 0))
                            {
                                height -= maxBorderWidth;
                                if (height < 0) height = 0;
                            }

                            if (htmlExport.exportQuality == StiHtmlExportQuality.High)
                            {
                                cell.Width = StiHtmlUnit.NewUnit(width, StiOptions.Export.Html.PrintLayoutOptimization);
                                cell.Height = StiHtmlUnit.NewUnit(height, StiOptions.Export.Html.PrintLayoutOptimization);
                            }

                            #region Render Style
                            StiCellStyle cellStyle = matrix.CellStyles[rowIndex, columnIndex];
                            if (cellStyle != null)
                            {
                                object styleObj = hashStyles[cellStyle];
                                if ((styleObj != null) && (htmlExport.useStylesTable))
                                {
                                    //cell.CssClass = "s" + styleIndex.ToString();
                                    cell.CssClass = "s" + cellStyle.StyleName;
                                }
                            }
                            if (string.IsNullOrEmpty(cell.CssClass))
                            {
                                cell.Style["border"] = "0px";
                            }
                            #endregion

                            #region Process RoundedRectangle
                            if (listPrimitivesInRow != null)
                            {
                                foreach (var rrInfo in listPrimitivesInRow)
                                {
                                    if (rrInfo.Rect.Left == columnIndex)
                                    {
                                        if (rrInfo.Rect.Width > 1)
                                        {
                                            for (int indexColSpan = 1; indexColSpan < rrInfo.Rect.Width; indexColSpan++)
                                            {
                                                width += GetWidth(listX, columnIndex + indexColSpan, htmlExport.zoom);
                                            }
                                        }
                                        if (rrInfo.Rect.Height > 1)
                                        {
                                            for (int indexRowSpan = 1; indexRowSpan < rrInfo.Rect.Height; indexRowSpan++)
                                            {
                                                height += GetHeight(listY, rowIndex + indexRowSpan, htmlExport.zoom);
                                            }
                                        }

                                        int space = (int)Math.Round((double)(Math.Min((height < width ? height : width), 100) * rrInfo.Primitive.Round * 1.04));

                                        var htmlPrimitive = new StiHtmlRoundedRectangle();
                                        //htmlPrimitive.Primitive = rrInfo.Primitive;
                                        htmlPrimitive.Width = width;
                                        htmlPrimitive.Height = height;
                                        htmlPrimitive.Style = htmlExport.GetBorderStyle(rrInfo.Primitive.Style);
                                        htmlPrimitive.Size = Math.Round((double)rrInfo.Primitive.Size, 2);
                                        htmlPrimitive.Color = StiHtmlExportService.FormatColor(rrInfo.Primitive.Color);
                                        htmlPrimitive.Round = space.ToString();
                                        cell.Controls.Add(htmlPrimitive);
                                    }
                                }
                            }
                            #endregion
                        }
                        #endregion
                    }
                }

                #region UseStrictTableCellSize, background color check
                if (StiOptions.Export.Html.UseStrictTableCellSize)
                {
                    if (cellsColors[0].A != 0)
                    {
                        bool flag = true;
                        for (int index1 = 0; index1 < cellsColors.Length - 1; index1++)
                        {
                            if (!cellsColors[index1].Equals(cellsColors[index1 + 1]))
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            row.Style["background-color"] = StiHtmlExportService.FormatColor(cellsColors[0]);
                        }
                    }
                }
                #endregion

                #endregion
            }


            table.RenderControl(htmlExport.HtmlWriter, htmlExportSettings.AddPageBreaks);
		}
		#endregion
		
		internal StiHtmlTableRender(StiHtmlExportService htmlExport, StiHtmlExportSettings htmlExportSettings, 
			StiPagesCollection pages)
		{
            #region Check ExceedMargins
            bool hasExceedMargins = false;
            foreach (StiPage page in htmlExport.report.Pages)
            {
                foreach (StiComponent comp in page.GetComponents())
                {
                    var text = comp as StiText;
                    if (text != null && text.ExceedMargins != StiExceedMargins.None)
                    {
                        hasExceedMargins = true;
                        break;
                    }
                }
                if (hasExceedMargins) break;
            }
            //htmlExport.useFullExceedMargins = hasExceedMargins;   //commented temporarily for test
            #endregion

            matrix = new StiMatrix(pages, htmlExport, htmlExport.Styles);
			this.htmlExport = htmlExport;
			this.htmlExportSettings = htmlExportSettings;
		}
	}	
}
