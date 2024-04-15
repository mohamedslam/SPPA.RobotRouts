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

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
using Pens = Stimulsoft.Drawing.Pens;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Pen = Stimulsoft.Drawing.Pen;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiHorizontalLinePrimitiveGdiPainter : StiLinePrimitiveGdiPainter
    {
        #region Methods
        /// <summary>
        /// Paints the selection.
        /// </summary>
        public override void PaintSelection(StiComponent component, StiPaintEventArgs e)
        {
            var g = e.Graphics;
            if (component.IsDesigning && component.IsSelected && (!component.Report.Info.IsComponentsMoving))
            {
                var rect = component.GetPaintRectangle();

                float size = 2.5f * (float)StiScale.Factor;
                if (component.Linked) size = 3.5f * (float)StiScale.Factor;
                if (component.Locked)
                {
                    DrawDot(g, Pens.Red, rect.X, rect.Y, size);
                    DrawDot(g, Pens.Red, rect.Right, rect.Y, size);
                }
                else
                {
                    DrawDot(g, Pens.DimGray, rect.X, rect.Y, size);
                    DrawDot(g, Pens.DimGray, rect.Right, rect.Y, size);
                }
            }
        }

        private void DrawCap(Graphics g, StiHorizontalLinePrimitive primitive, RectangleD paintRect)
        {
            double zoom = primitive.Page.Zoom * StiScale.Factor;
            float x;
            float y;
            float vertOffset;
            float horOffset;
            float lineSize = (float)(zoom * primitive.Size);

            #region StartCap
            var startCap = primitive.StartCap;
            if (startCap.Style != StiCapStyle.None)
            {
                SmoothingMode mode = g.SmoothingMode;
                if (!StiOptions.Engine.DisableAntialiasingInPainters) g.SmoothingMode = SmoothingMode.AntiAlias;
                PointF[] points = null;
                var clRect = new RectangleF((float)paintRect.X, (float)(paintRect.Y - startCap.Height / 2),
                    startCap.Width, startCap.Height);

                x = clRect.X;
                y = (float)(clRect.Y + clRect.Height / 2);
                vertOffset = (float)(clRect.Height / 2 * zoom);
                horOffset = (float)(clRect.Width / 2 * zoom);

                switch (startCap.Style)
                {
                    #region Arrow
                    case StiCapStyle.Arrow:
                        points = new PointF[] { new PointF(x - lineSize, y), 
                            new PointF(x + 2 * horOffset, y + vertOffset),
                            new PointF(x + 2 * horOffset, y - vertOffset)};
                        if (startCap.Fill)
                        {
                            using (var brush = new SolidBrush(startCap.Color))
                            {
                                g.FillPolygon(brush, points);
                            }
                        }
                        else
                        {
                            using (var pen = new Pen(startCap.Color, lineSize))
                            {
                                g.DrawPolygon(pen, points);
                            }
                        }
                        break;
                    #endregion

                    #region Diamond
                    case StiCapStyle.Diamond:
                        points = new PointF[] { new PointF(x - horOffset, y), 
                                new PointF(x, y - vertOffset),
                                new PointF(x + horOffset, y), 
                                new PointF(x, y + vertOffset)};
                        if (startCap.Fill)
                        {
                            using (var brush = new SolidBrush(startCap.Color))
                            {
                                g.FillPolygon(brush, points);
                            }
                        }
                        else
                        {
                            using (var pen = new Pen(startCap.Color, lineSize))
                            {
                                g.DrawPolygon(pen, points);
                            }
                        }
                        break;
                    #endregion

                    #region Open
                    case StiCapStyle.Open:
                        using (var pen = new Pen(startCap.Color, lineSize))
                        {
                            points = new PointF[] {new PointF(x + 2 * horOffset, y - vertOffset),
                                new PointF(x, y), 
                                new PointF(x + 2 * horOffset, y + vertOffset)};
                            g.DrawLines(pen, points);
                        }
                        break;
                    #endregion

                    #region Oval
                    case StiCapStyle.Oval:
                        var ellipseRect = new RectangleF(x - horOffset, y - vertOffset, horOffset * 2, vertOffset * 2);

                        if (startCap.Fill)
                        {
                            using (var brush = new SolidBrush(startCap.Color))
                            {
                                g.FillEllipse(brush, ellipseRect);
                            }
                        }
                        else
                        {
                            using (var pen = new Pen(startCap.Color, lineSize))
                            {
                                g.DrawEllipse(pen, ellipseRect);
                            }
                        }
                        break;
                    #endregion

                    #region Square
                    case StiCapStyle.Square:
                        var squareRect = new RectangleF(x - horOffset, y - vertOffset, horOffset * 2, vertOffset * 2);

                        if (startCap.Fill)
                        {
                            using (var brush = new SolidBrush(startCap.Color))
                            {
                                g.FillRectangle(brush, squareRect);
                            }
                        }
                        else
                        {
                            using (var pen = new Pen(startCap.Color, lineSize))
                            {
                                g.DrawRectangle(pen, squareRect.X, squareRect.Y, squareRect.Width, squareRect.Height);
                            }
                        }
                        break;
                    #endregion

                    #region Stealth
                    case StiCapStyle.Stealth:
                        points = new PointF[] { new PointF(x - lineSize, y), 
                            new PointF(x + 2 * horOffset, y - vertOffset),
                            new PointF(x + horOffset * 1.3f, y), 
                            new PointF(x + 2 * horOffset, y + vertOffset)};
                        if (startCap.Fill)
                        {
                            using (var brush = new SolidBrush(startCap.Color))
                            {
                                g.FillPolygon(brush, points);
                            }
                        }
                        else
                        {
                            using (var pen = new Pen(startCap.Color, lineSize))
                            {
                                g.DrawPolygon(pen, points);
                            } 
                        }
                        break;
                    #endregion
                }
                g.SmoothingMode = mode;
            }
            #endregion

            #region EndCap
            var endCap = primitive.EndCap;
            if (endCap.Style != StiCapStyle.None)
            {
                SmoothingMode mode = g.SmoothingMode;
                if (!StiOptions.Engine.DisableAntialiasingInPainters) g.SmoothingMode = SmoothingMode.AntiAlias;
                PointF[] points = null;
                var clRect = new RectangleF((float)paintRect.Right - endCap.Width, (float)(paintRect.Y - endCap.Height / 2),
                    endCap.Width, endCap.Height);

                y = clRect.Y + (float)(clRect.Height / 2);
                x = clRect.Right;
                vertOffset = (float)((clRect.Height / 2) * zoom);
                horOffset = (float)((clRect.Width / 2) * zoom);

                switch (endCap.Style)
                {
                    #region Arrow
                    case StiCapStyle.Arrow:
                        points = new PointF[] { new PointF(x + lineSize, y), 
                            new PointF(x - 2 * horOffset, y - vertOffset),
                            new PointF(x - 2 * horOffset, y + vertOffset)};
                        if (endCap.Fill)
                        {
                            using (var brush = new SolidBrush(endCap.Color))
                            {
                                g.FillPolygon(brush, points);
                            }
                        }
                        else
                        {
                            using (var pen = new Pen(endCap.Color, lineSize))
                            {
                                g.DrawPolygon(pen, points);
                            }
                        }
                        break;
                    #endregion

                    #region Diamond
                    case StiCapStyle.Diamond:
                        points = new PointF[] { new PointF(x + horOffset, y), 
                                new PointF(x, y - vertOffset),
                                new PointF(x - horOffset, y), 
                                new PointF(x, y + vertOffset)};
                        if (endCap.Fill)
                        {
                            using (var brush = new SolidBrush(endCap.Color))
                            {
                                g.FillPolygon(brush, points);
                            }
                        }
                        else
                        {
                            using (var pen = new Pen(endCap.Color, lineSize))
                            {
                                g.DrawPolygon(pen, points);
                            }
                        }
                        break;
                    #endregion

                    #region Open
                    case StiCapStyle.Open:
                        using (var pen = new Pen(endCap.Color, lineSize))
                        {
                            points = new PointF[] {new PointF(x - 2 * horOffset, y - vertOffset),
                            new PointF(x, y),
                            new PointF(x - 2 * horOffset, y + vertOffset)};
                            g.DrawLines(pen, points);
                        }
                        break;
                    #endregion

                    #region Oval
                    case StiCapStyle.Oval:
                        var ellipseRect = new RectangleF(x - horOffset, y - vertOffset, horOffset * 2, vertOffset * 2);

                        if (endCap.Fill)
                        {
                            using (var brush = new SolidBrush(endCap.Color))
                            {
                                g.FillEllipse(brush, ellipseRect);
                            }
                        }
                        else
                        {
                            using (var pen = new Pen(endCap.Color, lineSize))
                            {
                                g.DrawEllipse(pen, ellipseRect);
                            }
                        }
                        break;
                    #endregion

                    #region Square
                    case StiCapStyle.Square:
                        var squareRect = new RectangleF(x - horOffset, y - vertOffset, horOffset * 2, vertOffset * 2);

                        if (endCap.Fill)
                        {
                            using (var brush = new SolidBrush(endCap.Color))
                            {
                                g.FillRectangle(brush, squareRect);
                            }
                        }
                        else
                        {
                            using (var pen = new Pen(endCap.Color, lineSize))
                            {
                                g.DrawRectangle(pen, squareRect.X, squareRect.Y, squareRect.Width, squareRect.Height);
                            }
                        }
                        break;
                    #endregion

                    #region Stealth
                    case StiCapStyle.Stealth:
                        points = new PointF[] { new PointF(x + lineSize, y), 
                                new PointF(x - horOffset * 2, y - vertOffset),
                                new PointF(x - horOffset * 1.3f, y), 
                                new PointF(x - 2 * horOffset, y + vertOffset)};
                        if (endCap.Fill)
                        {
                            using (var brush = new SolidBrush(endCap.Color))
                            {
                                g.FillPolygon(brush, points);
                            }
                        }
                        else
                        {
                            using (var pen = new Pen(endCap.Color, lineSize))
                            {
                                g.DrawPolygon(pen, points);
                            }
                        }
                        break;
                    #endregion
                }
                g.SmoothingMode = mode;
            }
            #endregion
        }
        #endregion

        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var primitive = component as StiHorizontalLinePrimitive;
            primitive.InvokePainting(primitive, e);

            if (!e.Cancel && (!(primitive.Enabled == false && primitive.IsDesigning == false)))
            {
                var g = e.Graphics;

                var rect = primitive.GetPaintRectangle();
                CheckRectForOverflow(ref rect);
                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    double y = rect.Y;
                    if (primitive.Page.OffsetRectangle.Height < 0) y = rect.Bottom;

                    #region HighlightState
                    if ((primitive.HighlightState == StiHighlightState.Show ||
                        primitive.HighlightState == StiHighlightState.Active) &&
                        (!primitive.Report.Info.IsComponentsMoving) &&
                        StiRestrictionsHelper.IsAllowSelect(primitive))
                    {
                        SmoothingMode oldSmoothingMode = g.SmoothingMode;
                        if (!StiOptions.Engine.DisableAntialiasingInPainters) g.SmoothingMode = SmoothingMode.AntiAlias;

                        using (var pen2 = new Pen(Color.FromArgb(150, Color.Gold), StiScale.I3))
                        using (var path = new GraphicsPath())
                        {
                            pen2.Alignment = PenAlignment.Outset;
                            pen2.StartCap = LineCap.Round;
                            pen2.EndCap = LineCap.Round;

                            path.AddLine((float)rect.X, (float)y, (float)rect.Right, (float)y);

                            if (primitive.IsSelected)
                            {
                                float size = StiScale.I3;
                                AddDotToPath(path, rect.X, y, size);
                                AddDotToPath(path, rect.Right, y, size);
                            }

                            g.DrawPath(pen2, path);
                        }

                        g.SmoothingMode = oldSmoothingMode;
                    }
                    #endregion

                    float offsetDouble = (float)(primitive.Page.Zoom * StiScale.Factor> StiScale.Factor ? primitive.Page.Zoom * StiScale.Factor : StiScale.Factor);
                    using (var pen = new Pen(primitive.Color, (primitive.Style == StiPenStyle.Double) ? offsetDouble : (float)(primitive.Size * primitive.Page.Zoom * StiScale.Factor)))
                    {
                        pen.DashStyle = StiPenUtils.GetPenStyle(primitive.Style);

                        if (primitive.Style == StiPenStyle.Double)
                        {
                            g.DrawLine(pen, (float)rect.X, (float)y - offsetDouble, (float)rect.Right, (float)y - offsetDouble);
                            g.DrawLine(pen, (float)rect.X, (float)y + offsetDouble, (float)rect.Right, (float)y + offsetDouble);
                        }
                        else
                        {
                            if (primitive.Style != StiPenStyle.None || StiOptions.Engine.PrimitivesStyleNoneBackCompatibility)
                                g.DrawLine(pen, (float)rect.X, (float)y, (float)rect.Right, (float)y);
                        }
                    }

                    DrawCap(g, primitive, rect);

                    PaintEvents(primitive, e.Graphics, rect);
                    PaintConditions(primitive, e.Graphics, rect);
                }
            }
            e.Cancel = false;
            primitive.InvokePainted(primitive, e);

        }
        #endregion
    }
}