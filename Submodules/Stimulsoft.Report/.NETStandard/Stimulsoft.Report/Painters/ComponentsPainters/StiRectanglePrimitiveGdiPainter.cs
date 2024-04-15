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
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
using Pens = Stimulsoft.Drawing.Pens;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiRectanglePrimitiveGdiPainter : StiLinePrimitiveGdiPainter
    {
        #region Methods.Painter
        protected virtual void PaintPrimitive(StiRectanglePrimitive primitive, Graphics g, RectangleD rect)
        {
            #region HighlightState
            if ((primitive.HighlightState == StiHighlightState.Show || primitive.HighlightState == StiHighlightState.Active) &&
                (!primitive.Report.Info.IsComponentsMoving))
            {
                var oldSmoothingMode = g.SmoothingMode;
                if (!StiOptions.Engine.DisableAntialiasingInPainters) g.SmoothingMode = SmoothingMode.AntiAlias;

                using (var pen2 = new Pen(Color.FromArgb(150, Color.Gold), StiScale.I3))
                using (var path = new GraphicsPath())
                {
                    pen2.Alignment = PenAlignment.Outset;
                    pen2.StartCap = LineCap.Round;
                    pen2.EndCap = LineCap.Round;

                    path.AddRectangle(rect.ToRectangleF());

                    if (primitive.IsSelected)
                    {
                        float size = StiScale.I3;
                        AddDotToPath(path, rect.X, rect.Y, size);
                        AddDotToPath(path, rect.Right, rect.Y, size);
                        AddDotToPath(path, rect.X, rect.Bottom, size);
                        AddDotToPath(path, rect.Right, rect.Bottom, size);

                        AddDotToPath(path, rect.X + rect.Width / 2, rect.Y, size);
                        AddDotToPath(path, rect.Right, rect.Y + rect.Height / 2, size);
                        AddDotToPath(path, rect.X + rect.Width / 2, rect.Bottom, size);
                        AddDotToPath(path, rect.X, rect.Y + rect.Height / 2, size);
                    }

                    g.DrawPath(pen2, path);
                }

                g.SmoothingMode = oldSmoothingMode;
            }
            #endregion

            using (var pen = new Pen(primitive.Color, (float)(primitive.Size * primitive.Page.Zoom * StiScale.Factor)))
            {
                pen.DashStyle = StiPenUtils.GetPenStyle(primitive.Style);

                if (primitive.Style == StiPenStyle.Double)
                {
                    var border = new StiBorder
                    {
                        Color = primitive.Color,
                        Style = StiPenStyle.Double,
                        Side = StiBorderSides.None
                    };

                    if (primitive.TopSide) border.Side |= StiBorderSides.Top;
                    if (primitive.LeftSide) border.Side |= StiBorderSides.Left;
                    if (primitive.BottomSide) border.Side |= StiBorderSides.Bottom;
                    if (primitive.RightSide) border.Side |= StiBorderSides.Right;

                    border.Draw(g, rect, primitive.Page.Zoom * StiScale.Factor);
                }
                else
                {
                    if (primitive.Style != StiPenStyle.None || StiOptions.Engine.PrimitivesStyleNoneBackCompatibility)
                    {
                        if (primitive.TopSide && primitive.LeftSide && primitive.BottomSide && primitive.RightSide)
                            g.DrawRectangle(pen, (float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
                        else
                        {
                            if (primitive.TopSide)
                                g.DrawLine(pen, (float)rect.X, (float)rect.Y, (float)rect.Right, (float)rect.Y);

                            if (primitive.BottomSide)
                                g.DrawLine(pen, (float)rect.X, (float)rect.Bottom, (float)rect.Right, (float)rect.Bottom);

                            if (primitive.LeftSide)
                                g.DrawLine(pen, (float)rect.X, (float)rect.Y, (float)rect.X, (float)rect.Bottom);

                            if (primitive.RightSide)
                                g.DrawLine(pen, (float)rect.Right, (float)rect.Y, (float)rect.Right, (float)rect.Bottom);
                        }
                    }
                }
            }
        }

        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var primitive = (StiRectanglePrimitive)component;
            primitive.InvokePainting(primitive, e);

            if (!e.Cancel && (!(primitive.Enabled == false && primitive.IsDesigning == false)))
            {
                var g = e.Graphics;

                var rect = primitive.GetPaintRectangle();
                CheckRectForOverflow(ref rect);
                if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    PaintPrimitive(primitive, g, rect);
                    PaintEvents(primitive, e.Graphics, rect);
                    PaintConditions(primitive, e.Graphics, rect);
                }
            }
            e.Cancel = false;
            primitive.InvokePainted(primitive, e);

        }

        /// <summary>
        /// Paints the selection.
        /// </summary>
        public override void PaintSelection(StiComponent component, StiPaintEventArgs e)
        {
            var g = e.Graphics;
            if (component.IsDesigning && component.IsSelected && (!component.Report.Info.IsComponentsMoving))
            {
                var rect = component.GetPaintRectangle();

                var pen = Pens.DimGray;
                if (component.Locked) pen = Pens.Red;

                float size = 2.5f * (float)StiScale.Factor;
                if (component.Linked) size = 3.5f * (float)StiScale.Factor;

                DrawDot(g, pen, rect.X, rect.Y, size);
                DrawDot(g, pen, rect.Right, rect.Y, size);
                DrawDot(g, pen, rect.X, rect.Bottom, size);
                DrawDot(g, pen, rect.Right, rect.Bottom, size);

                DrawDot(g, pen, rect.X + rect.Width / 2, rect.Y, size);
                DrawDot(g, pen, rect.Right, rect.Y + rect.Height / 2, size);
                DrawDot(g, pen, rect.X + rect.Width / 2, rect.Bottom, size);
                DrawDot(g, pen, rect.X, rect.Y + rect.Height / 2, size);
            }
        }
        #endregion
    }
}
