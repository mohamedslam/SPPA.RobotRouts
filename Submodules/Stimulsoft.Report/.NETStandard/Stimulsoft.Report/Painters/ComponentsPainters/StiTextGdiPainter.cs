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
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Export;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
using Brushes = Stimulsoft.Drawing.Brushes;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiTextGdiPainter : StiComponentGdiPainter
    {
        #region Properties
        public static bool IsThumbnailsMode { get; set; }
        #endregion

        #region Methods
        public virtual RectangleD PaintIndicator(StiText textComp, Graphics g, RectangleD rect)
        {
            if (textComp.Indicator == null) 
                return rect;

            var painter = StiIndicatorTypePainter.GetPainter(textComp.Indicator.GetType(), StiGuiMode.Gdi);
            return painter.Paint(g, textComp, rect);
        }

        public virtual void PaintText(StiText textComp, Graphics g, RectangleD rect)
        {
            var text = textComp.GetTextForPaint();

            if (textComp.IsDesigning && !textComp.Report.Info.QuickInfoOverlay && textComp.Report.Info.QuickInfoType != StiQuickInfoType.None)
                text = textComp.GetQuickInfo();

            if (string.IsNullOrEmpty(text)) return;

            //speed optimization
            var fontSize = textComp.Font.Size * textComp.Page.Zoom * StiScale.Factor;
            if (fontSize < 1 && !textComp.IsPrinting && StiOptions.Engine.TinyTextOptimization)
            {
                var brush = new StiSolidBrush(Color.FromArgb(128, StiBrush.ToColor(textComp.TextBrush)));
                StiDrawing.FillRectangle(g, brush, rect);
                return;
            }

            if (StiOptions.Engine.UseNewHtmlEngine && textComp.AllowHtmlTags)
                StiHtmlTextRender.DrawString(g, rect, text, textComp);

            else if (textComp.TextQuality == StiTextQuality.Wysiwyg && !IsThumbnailsMode || textComp.AllowHtmlTags)
                StiWysiwygTextRender.DrawString(g, rect, text, textComp);

            else if (textComp.TextQuality == StiTextQuality.Typographic && !IsThumbnailsMode)
                StiTypographicTextRender.DrawString(g, rect, text, textComp);

            else
                StiStandardTextRenderer.DrawString(g, rect, text, textComp);
        }

        public virtual void PaintBackground(StiText text, Graphics g, RectangleD rect)
        {
            PaintBackground(text, g, rect, false);
        }

        public virtual void PaintBackground(StiText text, Graphics g, RectangleD rect, bool paintExtendedMargins)
        {
            if (text.ExceedMargins != StiExceedMargins.None && !paintExtendedMargins) return;            
            if (text.ExceedMargins == StiExceedMargins.None && paintExtendedMargins) return;

            if (text.ExceedMargins != StiExceedMargins.None && paintExtendedMargins)
                rect = ApplyExceedMargins(text, rect);

            if (text.Brush is StiSolidBrush &&
                ((StiSolidBrush)text.Brush).Color.A == 0 &&
                text.Report.Info.FillComponent &&
                text.IsDesigning)
            {
                var color = Color.FromArgb(150, Color.White);

                StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
            }
            else
            {
                StiDrawing.FillRectangle(g, text.Brush, rect);
            }
        }

        private static RectangleD ApplyExceedMargins(StiText text, RectangleD rect)
        {
            var paintRect = text.GetPaintRectangle(true, true);

            if ((text.ExceedMargins & StiExceedMargins.Left) > 0)
            {
                var leftPageMargin = text.Page.Unit.ConvertToHInches(text.Page.Margins.Left) * text.Page.Zoom * StiScale.Factor;
                rect.Width += paintRect.Left + leftPageMargin;
                rect.X = -leftPageMargin;
            }

            if ((text.ExceedMargins & StiExceedMargins.Top) > 0)
            {
                var topPageMargin = text.Page.Unit.ConvertToHInches(text.Page.Margins.Top) * text.Page.Zoom * StiScale.Factor;
                rect.Height += paintRect.Top + topPageMargin;
                rect.Y = - topPageMargin;
            }

            if ((text.ExceedMargins & StiExceedMargins.Right) > 0)
            {
                var pageWidthAndRightMargin = text.Page.Unit.ConvertToHInches(text.Page.Width + text.Page.Margins.Right) * text.Page.Zoom * StiScale.Factor;
                rect.Width += pageWidthAndRightMargin - paintRect.Right;
            }

            if ((text.ExceedMargins & StiExceedMargins.Bottom) > 0)
            {
                var pageHeightAndBottomMargin = text.Page.Unit.ConvertToHInches(text.Page.Height + text.Page.Margins.Bottom) * text.Page.Zoom * StiScale.Factor;
                rect.Height += pageHeightAndBottomMargin - paintRect.Bottom;
            }

            return rect;
        }

        public virtual void PaintBorder(StiText text, Graphics g, RectangleD rect, bool drawBorderFormatting, bool drawTopmostBorderSides)
        {
            if (text.HighlightState == StiHighlightState.Hide)
                base.PaintBorder(text, g, rect, text.Page.Zoom * StiScale.Factor, drawBorderFormatting, drawTopmostBorderSides);
        }


        public virtual void PaintLinesOfUnderlining(StiText text, Graphics g, RectangleD rect)
        {
            if (text.LinesOfUnderline == StiPenStyle.None) return;

            var heightFont = text.Font.GetHeight() * text.Page.Zoom * StiScale.Factor;
            using (var pen = new Pen(text.Border.Color))
            {
                pen.DashStyle = StiPenUtils.GetPenStyle(text.LinesOfUnderline);
                pen.Width = (int)(text.Border.Size * text.Page.Zoom * StiScale.Factor);

                if (text.VertAlignment == StiVertAlignment.Top)
                {
                    var posy = heightFont + rect.Top;
                    while (posy < rect.Bottom)
                    {
                        PaintOneLineOfUnderline(text, g, pen, rect.X, rect.Right, posy);
                        posy += heightFont;
                    }
                }
                else if (text.VertAlignment == StiVertAlignment.Center)
                {
                    var posy = rect.Top + rect.Height / 2 + heightFont * 0.5 - heightFont * 0.1;
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
                    var posy = rect.Bottom - heightFont - heightFont * 0.1;
                    while (posy > rect.Top)
                    {
                        PaintOneLineOfUnderline(text, g, pen, rect.X, rect.Right, posy);
                        posy -= heightFont;
                    }
                }
            }
        }

        public virtual void PaintOneLineOfUnderline(StiText text, Graphics g, Pen pen, double x1, double x2, double y)
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

        private void PaintMarkerFields(StiText text, Graphics g, RectangleD rect)
        {
            if (!text.GetMarkerFieldResult()) return;

            var zoom = text.Report.Info.Zoom;
            var rectMarker = new RectangleD(
                rect.Right - 8 * zoom,
                rect.Bottom - 8 * zoom,
                6 * zoom,
                6 * zoom);

            g.FillRectangle(Brushes.Red, rectMarker.ToRectangleF());
        }
        #endregion

        #region Methods.Painter
        public override Image GetImage(StiComponent component, ref float zoom, StiExportFormat format)
        {
            var text = (StiText)component;
            if (text.Indicator != null)
                zoom *= 2;

            var resZoom = text.Report.Info.Zoom;
            text.Report.Info.Zoom = zoom;
            StiScale.Lock();

            var rect = text.GetPaintRectangle();
            rect.X = 0;
            rect.Y = 0;

            //fix for border width in html
            if ((format == StiExportFormat.HtmlTable || format == StiExportFormat.ImagePng) && StiOptions.Export.Html.PrintLayoutOptimization &&
                (text.Border != null) && (text.Border.Style != StiPenStyle.None) && (text.Border.Side != StiBorderSides.None))
            {
                rect.Height -= text.Border.Size;
                rect.Width -= text.Border.Size;

                if (rect.Height < 0) 
                    rect.Height = 0;

                if (rect.Width < 0) 
                    rect.Width = 0;
            }

            var imageWidth = (int)rect.Width;
            var imageHeight = (int)rect.Height;

            Image bmp;
            if (format == StiExportFormat.Pdf && text.CheckAllowHtmlTags() && !text.IsExportAsImage(format) && StiOptions.Engine.FullTrust)
                bmp = CreateMetafileImage();
            else
                bmp = new Bitmap(imageWidth, imageHeight);

            if (text.Brush != null && text.TextQuality == StiTextQuality.Wysiwyg)
                StiExportUtils.DisableFontSmoothing(component.Report);

            using (var g = Graphics.FromImage(bmp))
            {
                g.PageUnit = GraphicsUnit.Pixel;

                var isTransparentBrush = (text.Brush != null) && ((text.Brush is StiSolidBrush) 
                    && ((text.Brush as StiSolidBrush).Color.A == 0) || (text.Brush is StiEmptyBrush));

                if (isTransparentBrush) 
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                if ((format == StiExportFormat.Pdf) || (format == StiExportFormat.ImagePng))
                {
                    if (isTransparentBrush)
                        g.FillRectangle(new SolidBrush(Color.FromArgb(1, 255, 255, 255)), new Rectangle(0, 0, imageWidth, imageHeight));
                }
                else
                {
                    g.FillRectangle(Brushes.White, new Rectangle(0, 0, imageWidth, imageHeight));
                }

                StiDrawing.FillRectangle(g, text.Brush, rect);
                rect = PaintIndicator(text, g, rect);

                rect = text.ConvertTextMargins(rect, true);
                rect = text.ConvertTextBorders(rect, true);

                PaintText(text, g, rect);
                PaintLinesOfUnderlining(text, g, rect);
            }
            text.Report.Info.Zoom = resZoom;

            StiScale.Unlock();
            
            return bmp;
        }

        private static Image CreateMetafileImage()
        {
            Image bmp = null; 
            using (var bmpTemp = new Bitmap(1, 1))
            using (var grfx = Graphics.FromImage(bmpTemp))
            {
                var ipHdc = grfx.GetHdc();
                bmp = new Metafile(ipHdc, EmfType.EmfOnly);
                grfx.ReleaseHdc(ipHdc);
            }
            return bmp;
        }

        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            var text = (StiText)component;

            if ((!e.DrawBorderFormatting) && e.DrawTopmostBorderSides && (!text.Border.Topmost))
                return;

            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            if (e.DrawBorderFormatting)
                component.InvokePainting(component, e);

            if (!e.Cancel && (!(component.Enabled == false && component.IsDesigning == false)))
            {
                var g = e.Graphics;
                var rect = component.GetPaintRectangle();
                
                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    if (component.Report != null && component.Report.IsExporting && (text.TextQuality == StiTextQuality.Wysiwyg))
                        StiExportUtils.DisableFontSmoothing(component.Report);

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

                    rect = PaintIndicator(text, g, rect);

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

                    if (e.DrawBorderFormatting)
                        PaintMarkerFields(text, e.Graphics, rect);
                }

                if (e.DrawBorderFormatting)
                {
                    PaintEvents(component, e.Graphics, rect);
                    PaintConditions(component, e.Graphics, rect);
                    PaintQuickButtons(component, e.Graphics);
                    PaintInteraction(component, e.Graphics);
                }
            }
            e.Cancel = false;
            
            if (e.DrawBorderFormatting)
                component.InvokePainted(component, e);

        }
        #endregion
    }
}
