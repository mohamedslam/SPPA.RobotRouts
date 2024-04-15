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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Components.Table;
using Stimulsoft.Base;
using System.Drawing;
using System.Drawing.Imaging;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
using Brushes = Stimulsoft.Drawing.Brushes;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiTableCellTextGdiPainter : StiComponentGdiPainter
    {
        #region Methods
        public virtual void PaintText(StiTableCell textComp, Graphics g, RectangleD rect)
        {
            string text = textComp.GetTextForPaint();
            if (textComp.IsDesigning && (!textComp.Report.Info.QuickInfoOverlay) && textComp.Report.Info.QuickInfoType != StiQuickInfoType.None)
                text = textComp.GetQuickInfo();

            if (!string.IsNullOrEmpty(text))
            {
                if (textComp.TextQuality == StiTextQuality.Wysiwyg && !StiTextGdiPainter.IsThumbnailsMode)
                {
                    StiWysiwygTextRender.DrawString(g, rect, text, textComp);
                }
                else if (textComp.TextQuality == StiTextQuality.Typographic && !StiTextGdiPainter.IsThumbnailsMode)
                {
                    StiTypographicTextRender.DrawString(g, rect, text, textComp);
                }
                else StiStandardTextRenderer.DrawString(g, rect, text, textComp);
            }
        }

        public virtual void PaintBackground(StiTableCell text, Graphics g, RectangleD rect)
        {
            if (text.Brush is StiSolidBrush &&
                ((StiSolidBrush)text.Brush).Color == Color.Transparent &&
                text.Report.Info.FillComponent &&
                text.IsDesigning)
            {
                Color color = Color.FromArgb(150, Color.White);

                StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
            }
            else StiDrawing.FillRectangle(g, text.Brush, rect);

            if (text.IsSelected)
            {
                Color selectColor = Color.FromArgb(80, Color.FromArgb(168, 205, 241));
                StiDrawing.FillRectangle(g, selectColor, rect);
            }
        }

        public virtual void PaintBorder(StiTableCell text, Graphics g, RectangleD rect, bool drawBorderFormatting, bool drawTopmostBorderSides)
        {
            if (text.HighlightState == StiHighlightState.Hide)
                base.PaintBorder(text, g, rect, text.Page.Zoom, drawBorderFormatting, drawTopmostBorderSides);
        }


        public virtual void PaintLinesOfUnderlining(StiTableCell text, Graphics g, RectangleD rect)
        {
            double heightFont = text.Font.GetHeight() * text.Page.Zoom;
            if (text.LinesOfUnderline != StiPenStyle.None)
            {
                using (var pen = new Pen(text.Border.Color))
                {
                    pen.DashStyle = StiPenUtils.GetPenStyle(text.LinesOfUnderline);
                    pen.Width = (int)(text.Border.Size * text.Page.Zoom);

                    if (text.VertAlignment == StiVertAlignment.Top)
                    {
                        double posy = heightFont + rect.Top;
                        while (posy < rect.Bottom)
                        {
                            PaintOneLineOfUnderline(text, g, pen, rect.X, rect.Right, posy);
                            posy += heightFont;
                        }
                    }
                    else if (text.VertAlignment == StiVertAlignment.Center)
                    {
                        double posy = rect.Top + rect.Height / 2 + heightFont * 0.5 - heightFont * 0.1;
                        while (posy > rect.Top && posy < rect.Bottom)
                        {
                            PaintOneLineOfUnderline(text, g, pen, rect.X, rect.Right, posy);
                            posy -= heightFont;
                        }

                        posy = rect.Top + rect.Height / 2 + heightFont * 0.5 - heightFont * 0.1;
                        while (posy > rect.Top && posy < rect.Bottom)
                        {
                            PaintOneLineOfUnderline(text, g, pen, rect.X, rect.Right, posy);
                            posy += heightFont;
                        }
                    }
                    else if (text.VertAlignment == StiVertAlignment.Bottom)
                    {
                        double posy = rect.Bottom - heightFont - heightFont * 0.1;
                        while (posy > rect.Top)
                        {
                            PaintOneLineOfUnderline(text, g, pen, rect.X, rect.Right, posy);
                            posy -= heightFont;
                        }
                    }
                }
            }
        }

        public virtual void PaintOneLineOfUnderline(StiTableCell text, Graphics g, Pen pen, double x1, double x2, double y)
        {
            if (text.LinesOfUnderline == StiPenStyle.Double)
            {
                g.DrawLine(pen,
                    (float)x1,
                    (float)(y - 1),
                    (float)x2,
                    (float)(y - 1));

                g.DrawLine(pen,
                    (float)x1,
                    (float)(y + 1),
                    (float)x2,
                    (float)(y + 1));
            }
            else
            {
                g.DrawLine(pen,
                    (float)x1,
                    (float)y,
                    (float)x2,
                    (float)y);
            }
        }

        private void PaintMarkerFields(StiTableCell text, Graphics g, RectangleD rect)
        {
            if (text.GetMarkerFieldResult())
            {
                double zoom = text.Report.Info.Zoom;
                var rectMarker = new RectangleD(
                    rect.Right - 8 * zoom,
                    rect.Bottom - 8 * zoom,
                    6 * zoom,
                    6 * zoom);

                g.FillRectangle(Brushes.Red, rectMarker.ToRectangleF());
            }
        }
        #endregion

        #region Methods.Painter
        public override Image GetImage(StiComponent component, ref float zoom, StiExportFormat format)
        {
            var text = (StiTableCell)component;

            double resZoom = text.Report.Info.Zoom;
            text.Report.Info.Zoom = zoom;
            var rect = text.GetPaintRectangle();
            rect.X = 0;
            rect.Y = 0;

            int imageWidth = (int)rect.Width;
            int imageHeight = (int)rect.Height;

            Image bmp = null;
            if ((format == StiExportFormat.Pdf) && (text.CheckAllowHtmlTags()) && (!text.IsExportAsImage(format)))
            {
                using (var bmpTemp = new Bitmap(1, 1))
                using (var grfx = Graphics.FromImage(bmpTemp))
                {
                    var ipHdc = grfx.GetHdc();
                    bmp = new Metafile(ipHdc, EmfType.EmfOnly);
                    grfx.ReleaseHdc(ipHdc);
                }
            }
            else bmp = new Bitmap(imageWidth, imageHeight);

            using (var g = Graphics.FromImage(bmp))
            {
                g.PageUnit = GraphicsUnit.Pixel;
                if (format != StiExportFormat.Pdf)
                {
                    g.FillRectangle(Brushes.White, new Rectangle(0, 0, imageWidth, imageHeight));
                }
                else
                {
                    if ((text.Brush != null) && (text.Brush is StiSolidBrush) && ((text.Brush as StiSolidBrush).Color == Color.Transparent))
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(1, 255, 255, 255)), new Rectangle(0, 0, imageWidth, imageHeight));
                    }
                }
                StiDrawing.FillRectangle(g, text.Brush, rect);

                PaintText(text, g, rect);
                PaintLinesOfUnderlining(text, g, rect);
            }
            text.Report.Info.Zoom = resZoom;
            return bmp;
        }

        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var text = (StiTableCell)component;
            if (!text.Enabled)
            {
                e.Cancel = false;
                component.InvokePainted(component, e);
                return;
            }
            component.InvokePainting(component, e);

            if (!e.Cancel && (!(component.Enabled == false && component.IsDesigning == false)))
            {
                var g = e.Graphics;

                var rect = component.GetPaintRectangle();
                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    if (component.Report != null && component.Report.IsExporting && (text.TextQuality == StiTextQuality.Wysiwyg))
                        Export.StiExportUtils.DisableFontSmoothing(component.Report);

                    #region Fill rectangle
                    if (e.DrawBorderFormatting)
                        PaintBackground(text, g, rect);
                    #endregion

                    #region Markers
                    if (e.DrawBorderFormatting)
                    {
                        if (text.HighlightState == StiHighlightState.Hide && text.Border.Side != StiBorderSides.All)
                            PaintMarkers(text, g, rect);
                    }
                    #endregion

                    var borderRect = rect;

                    #region Text margins
                    rect = text.ConvertTextMargins(rect, true);
                    rect = text.ConvertTextBorders(rect, true);
                    #endregion

                    #region Paint text
                    if (e.DrawBorderFormatting)
                        PaintText(text, g, rect);
                    #endregion

                    #region Lines of underlining
                    if (e.DrawBorderFormatting)
                        PaintLinesOfUnderlining(text, g, rect);
                    #endregion

                    #region Border
                    PaintBorder(text, g, borderRect, e.DrawBorderFormatting, e.DrawTopmostBorderSides || (!text.Border.Topmost));
                    #endregion

                    PaintMarkerFields(text, e.Graphics, rect);
                }

                PaintEvents(component, e.Graphics, rect);
                PaintConditions(component, e.Graphics, rect);
                PaintQuickButtons(component, e.Graphics);
                PaintInteraction(component, e.Graphics);
            }
            e.Cancel = false;
            component.InvokePainted(component, e);
        }

        public override void PaintSelection(StiComponent component, StiPaintEventArgs e)
        {
            var g = e.Graphics;
            if (component.IsDesigning && component.IsSelected && !component.Report.Info.IsComponentsMoving)
            {
                var rect = component.GetPaintRectangle();

                var size = StiScale.I2;

                using (var brush = new SolidBrush(Color.DimGray))
                {
                    StiDrawing.DrawSelectedRectangle(g, size, brush, rect);
                }
            }
        }
        #endregion
    }
}