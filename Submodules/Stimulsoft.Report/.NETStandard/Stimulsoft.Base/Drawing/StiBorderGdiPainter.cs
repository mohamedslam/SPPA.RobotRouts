#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
#endif

namespace Stimulsoft.Base.Drawing
{
    public static class StiBorderGdiPainter
    {
        public static void PaintBorder(Graphics g, Rectangle rect, StiSimpleBorder border, Color backColor,
            bool isTooLightBorderAdoptation = false)
        {
            PaintBorder(g, rect, border, backColor, 1f, isTooLightBorderAdoptation);
        }

        public static void PaintBorder(Graphics g, RectangleF rect, StiSimpleBorder border, Color backColor)
        {
            PaintBorder(g, rect, border, backColor, 1f);
        }

        public static void PaintBorder(Graphics g, RectangleF rect, StiSimpleBorder border, Color backColor, 
            float zoom, bool isTooLightBorderAdoptation = false)
        {
            if (border == null || border.IsDefault) return;

            var emptyColor = Color.White;
            if (border.Style == StiPenStyle.Double)
            {
                emptyColor = backColor;

                if (emptyColor == Color.Transparent)
                    emptyColor = Color.White;

                if (isTooLightBorderAdoptation && StiColorUtils.IsItTooLight(backColor))
                    emptyColor = Color.Transparent;
            }

            Pen emptyPen = null;
            using (var pen = new Pen(border.Color))
            {
                pen.Alignment = PenAlignment.Inset;

                if (border.Style == StiPenStyle.Double)
                    emptyPen = new Pen(emptyColor);

                pen.DashStyle = StiPenUtils.GetPenStyle(border.Style);

                if (border.Style != StiPenStyle.None)
                {
                    pen.Width = (int) (border.Size * zoom);
                    pen.StartCap = LineCap.Square;
                    pen.EndCap = LineCap.Square;

                    if (pen.Width > 1 && border.Style != StiPenStyle.Double)//Special correction
                    {
                        rect.Width++;
                        rect.Height++;
                    }

                    var rectIn = rect;
                    var rectOut = rect;

                    if (border.Style == StiPenStyle.Double)
                    {
                        rect.Inflate(-1, -1);
                        rectIn.Inflate(-2, -2);
                        pen.Width = 1;
                    }

                    #region All border sides
                    if (border.IsAllBorderSidesPresent)
                    {
                        if (border.Style == StiPenStyle.Double)
                        {
                            g.DrawRectangle(emptyPen, rect.X, rect.Y, rect.Width, rect.Height);
                            g.DrawRectangle(pen, rectIn.X, rectIn.Y, rectIn.Width, rectIn.Height);
                            g.DrawRectangle(pen, rectOut.X, rectOut.Y, rectOut.Width, rectOut.Height);
                        }
                        else
                            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                    }
                    #endregion

                    else
                    {
                        var shift = pen.Width / 2;
                        if (shift < 1)
                            shift = 0;

                        #region Top border side
                        float left;
                        float right;
                        if (border.IsTopBorderSidePresent)
                        {
                            left = rectIn.Left;
                            right = rectIn.Right;

                            if (!border.IsLeftBorderSidePresent)
                                left = rectOut.Left;

                            if (!border.IsRightBorderSidePresent)
                                right = rectOut.Right;

                            if (border.Style == StiPenStyle.Double)
                            {
                                g.DrawLine(emptyPen, rect.Left, rect.Top, rect.Right, rect.Top);
                                g.DrawLine(pen, left, rectIn.Top, right, rectIn.Top);
                                g.DrawLine(pen, rectOut.Left, rectOut.Top, rectOut.Right, rectOut.Top);
                            }
                            else
                                g.DrawLine(pen, rect.Left + shift, rect.Top + shift, rect.Right - shift, rect.Top + shift);
                        }
                        #endregion

                        #region Left border side
                        float top;
                        float bottom;
                        if (border.IsLeftBorderSidePresent)
                        {
                            top = rectIn.Top;
                            bottom = rectIn.Bottom;

                            if (!border.IsTopBorderSidePresent)
                                top = rectOut.Top;

                            if (!border.IsBottomBorderSidePresent)
                                bottom = rectOut.Bottom;

                            if (border.Style == StiPenStyle.Double)
                            {
                                g.DrawLine(emptyPen, rect.Left, rect.Top, rect.Left, rect.Bottom);
                                g.DrawLine(pen, rectIn.Left, top, rectIn.Left, bottom);
                                g.DrawLine(pen, rectOut.Left, rectOut.Top, rectOut.Left, rectOut.Bottom);
                            }
                            else
                                g.DrawLine(pen, rect.Left + shift, rect.Top + shift, rect.Left + shift, rect.Bottom - shift);
                        }
                        #endregion

                        #region Bottom border side
                        if (border.IsBottomBorderSidePresent)
                        {
                            left = rectIn.Left;
                            right = rectIn.Right;

                            if (!border.IsLeftBorderSidePresent)
                                left = rectOut.Left;

                            if (!border.IsRightBorderSidePresent)
                                right = rectOut.Right;

                            if (border.Style == StiPenStyle.Double)
                            {
                                g.DrawLine(emptyPen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
                                g.DrawLine(pen, left, rectIn.Bottom, right, rectIn.Bottom);
                                g.DrawLine(pen, rectOut.Left, rectOut.Bottom, rectOut.Right, rectOut.Bottom);
                            }
                            else
                                g.DrawLine(pen, rect.Left + shift, rect.Bottom - shift, rect.Right - shift, rect.Bottom - shift);
                        }
                        #endregion

                        #region Right border side
                        if (border.IsRightBorderSidePresent)
                        {
                            top = rectIn.Top;
                            bottom = rectIn.Bottom;

                            if (!border.IsTopBorderSidePresent)
                                top = rectOut.Top;

                            if (!border.IsBottomBorderSidePresent)
                                bottom = rectOut.Bottom;

                            if (border.Style == StiPenStyle.Double)
                            {
                                g.DrawLine(emptyPen, rect.Right, rect.Top, rect.Right, rect.Bottom);
                                g.DrawLine(pen, rectIn.Right, top, rectIn.Right, bottom);
                                g.DrawLine(pen, rectOut.Right, rectOut.Top, rectOut.Right, rectOut.Bottom);
                            }
                            else
                                g.DrawLine(pen, rect.Right - shift, rect.Top + shift, rect.Right - shift, rect.Bottom - shift);
                        }
                        #endregion
                    }

                    if (emptyPen != null)
                        emptyPen.Dispose();
                }
            }
        }
    }
}