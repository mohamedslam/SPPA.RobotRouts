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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiShapeGdiPainter : 
        StiComponentGdiPainter
    {
        #region Methods.Painter
        public override Image GetImage(StiComponent component, ref float zoom, StiExportFormat format)
        {
            var shape = (StiShape)component;

            double resZoom = shape.Report.Info.Zoom;
            shape.Report.Info.Zoom = zoom;
            StiScale.Lock();

            int shift = (int)Math.Round(shape.Size * zoom / 2);
            var rect = component.ComponentToPage(component.ClientRectangle).Normalize();
            rect = shape.Report.Unit.ConvertToHInches(rect).Multiply(zoom);

            rect.X = 0;
            rect.Y = 0;

            int imageWidth = (int)(rect.Width + 0.5) + 1 + (int)(shape.Size * zoom);
            int imageHeight = (int)(rect.Height + 0.5) + 1 + (int)(shape.Size * zoom);

            var bmp = new Bitmap(imageWidth, imageHeight);
            using (var g = Graphics.FromImage(bmp))
            {
                g.PageUnit = GraphicsUnit.Pixel;
                if (format == StiExportFormat.ImagePng)
                {
                    g.Clear(Color.Transparent);
                }
                else
                {
                    var color = Color.White;
                    if (component.Page != null) color = StiBrush.ToColor(component.Page.Brush);
                    if (color.Equals(Color.Transparent) || color.Equals(Color.Empty)) color = Color.White;
                    g.Clear(color);
                }
                g.TranslateTransform(shift, shift);

                var state = g.Save();
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;

                shape.ShapeType.Paint(g, shape, rect.ToRectangleF(), (float)zoom);
                DrawText(g, shape, rect);

                g.Restore(state);
            }

            shape.Report.Info.Zoom = resZoom;

            StiScale.Unlock();

            return bmp;
        }

        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            if ((!e.DrawBorderFormatting) && e.DrawTopmostBorderSides) return;

            var shape = (StiShape)component;

            shape.InvokePainting(shape, e);

            if (!e.Cancel && (!(shape.Enabled == false && shape.IsDesigning == false)))
            {
                var g = e.Graphics;

                var state = g.Save();
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;

                var rect = shape.GetPaintRectangle();
                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    shape.ShapeType.Paint(g, shape, rect.ToRectangleF(), (float)(shape.Page.Zoom * StiScale.Factor));

                    DrawText(g, shape, rect);

                    #region Markers
                    PaintMarkers(shape, g, rect);
                    #endregion

                    PaintEvents(shape, e.Graphics, rect);
                    PaintConditions(shape, e.Graphics, rect);
                }

                g.Restore(state);
            }

            e.Cancel = false;
            shape.InvokePainted(shape, e);
        }

        public void DrawText(Graphics g, StiShape shape, RectangleD rect)
        {
            var parserText = shape.GetParsedText();
            if (string.IsNullOrEmpty(parserText)) return;

            g.SetClip(rect.ToRectangle());

            var zoom = shape.Page.Zoom * StiScale.Factor;

            if (rect.Width - shape.Margins.Left * zoom - shape.Margins.Right * zoom <= 0) return;
            if (rect.Height - shape.Margins.Top * zoom - shape.Margins.Bottom * zoom <= 0) return;

            rect = new RectangleD(rect.X + shape.Margins.Left * zoom, rect.Y + shape.Margins.Top * zoom,
                rect.Width - (shape.Margins.Left + shape.Margins.Right) * zoom, rect.Height - (shape.Margins.Top + shape.Margins.Bottom) * zoom);

            var fontSize = shape.Font.Size * shape.Page.Zoom;
            if (fontSize < 1 && !shape.IsPrinting && StiOptions.Engine.TinyTextOptimization)
            {
                var brush = new StiSolidBrush(Color.FromArgb(128, shape.ForeColor));
                StiDrawing.FillRectangle(g, brush, rect);
                return;
            }

            zoom = shape.Page.Zoom;

            if (StiScale.IsNoScaling)
            {
                if (!shape.IsPrinting && StiDpiHelper.NeedDeviceCapsScale)
                    zoom *= (float)StiDpiHelper.DeviceCapsScale;
            }

            fontSize = (float)(shape.Font.Size * zoom);

            if (shape.Report.IsPrinting)
            {
                if (shape.Font.Unit != GraphicsUnit.Pixel && shape.Font.Unit != GraphicsUnit.World)
                    fontSize *= StiOptions.Engine.TypographicTextQualityScale / 100f;
            }
            else
            {
                if (shape.Font.Unit != GraphicsUnit.Pixel && shape.Font.Unit != GraphicsUnit.World)
                    fontSize *= StiOptions.Engine.StandardTextQualityScale / 96f;
            }

            using (var font = StiFontUtils.ChangeFontSize(shape.Font, (float)fontSize))
            using (var brush = new SolidBrush(shape.ForeColor))
            {
                #region Get TextBackground
                if (shape.BackgroundColor != Color.Transparent)
                {
                    var textSize = g.MeasureString(parserText, font, (int)rect.Width);
                    double leftFillRect = rect.X;
                    double topFillRect = rect.Y;
                    switch (shape.HorAlignment)
                    {
                        case StiTextHorAlignment.Center:
                            leftFillRect = rect.X + (rect.Width / 2) - textSize.Width / 2;
                            break;

                        case StiTextHorAlignment.Right:
                            leftFillRect = rect.Right - textSize.Width;
                            break;
                    }

                    switch (shape.VertAlignment)
                    {
                        case StiVertAlignment.Center:
                            topFillRect = rect.Y + (rect.Height / 2) - textSize.Height / 2;
                            break;

                        case StiVertAlignment.Bottom:
                            topFillRect = rect.Bottom - textSize.Height;
                            break;
                    }

                    var fillRect = new RectangleF((float)leftFillRect, (float)topFillRect, textSize.Width, textSize.Height);
                    using (var fill = new SolidBrush(shape.BackgroundColor))
                    {
                        g.FillRectangle(fill, fillRect);
                    }
                }
                #endregion

                var defaultHint = g.TextRenderingHint;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                StiTextDrawing.DrawString(g, parserText, font, brush, rect, new StiTextOptions() { WordWrap = true },
                    shape.HorAlignment, shape.VertAlignment, true, (float)shape.Page.Zoom, StiOptions.Engine.TextLineSpacingScale);

                g.TextRenderingHint = defaultHint;
            }

            g.ResetClip();
        }
        #endregion
    }
}
