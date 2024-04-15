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

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
#endif

namespace Stimulsoft.Base.Drawing
{
    public static class StiRoundedBackgroundGdiPainter
    {
        #region Methods
        public static void PaintBackground(Graphics g, RectangleF rect, StiSimpleBorder border, StiCornerRadius cornerRadius, Color? backColor,
            StiButtonShapeType shapeType, bool isEnabled, float scale)
        {
            var brush = new StiSolidBrush(backColor.GetValueOrDefault(Color.Transparent));
            PaintBackground(g, rect, border, cornerRadius, brush, shapeType, isEnabled, scale);
        }

        public static void PaintBackground(Graphics g, RectangleF rect, StiSimpleBorder border, StiCornerRadius cornerRadius, StiBrush brush, 
            StiButtonShapeType shapeType, bool isEnabled, float scale)
        {
            if (shapeType == StiButtonShapeType.Circle)
                PaintCircledBackground(g, rect, brush, isEnabled);

            else if (cornerRadius == null || cornerRadius.IsEmpty)
                PaintBackground(g, rect, border, brush, isEnabled);

            else
                PaintRoundedBackground(g, rect, border, cornerRadius, brush, isEnabled, scale);            
        }

        public static void PaintRoundedBackground(Graphics g, RectangleF rect, StiSimpleBorder border, StiCornerRadius cornerRadius, 
            StiBrush brush, bool isEnabled, float scale)
        {
            var storedMode = g.SmoothingMode;

            try
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                rect = ShiftRectangleFromBorder(rect, border);

                if (!isEnabled)
                    brush = new StiSolidBrush(Color.FromArgb(230, 230, 230));

                using (var gdiBrush = StiBrush.GetBrush(brush, rect))
                using (var path = StiRoundedRectangleCreator.Create(rect, cornerRadius, scale))
                {
                    g.FillPath(gdiBrush, path);

                    #region Draw sides rectangle (fix fill path #5888)
                    var rad = Math.Min(rect.Width / 2, rect.Height / 2);
                    var radiusTopLeft = StiRoundedRectangleCreator.GetRadiusTopLeft(cornerRadius, scale, rad);
                    var radiusTopRight = StiRoundedRectangleCreator.GetRadiusTopRight(cornerRadius, scale, rad);
                    var radiusBottomLeft = StiRoundedRectangleCreator.GetRadiusBottomLeft(cornerRadius, scale, rad);
                    var radiusBottomRight = StiRoundedRectangleCreator.GetRadiusBottomRight(cornerRadius, scale, rad);

                    using (var pen = new Pen(gdiBrush))
                    {
                        PointF point1;
                        PointF point2;

                        #region Left Side
                        point1 = radiusTopLeft > 0
                                            ? new PointF(rect.X, rect.Y + radiusTopLeft + 2)
                                            : new PointF(rect.X, rect.Y);

                        point2 = radiusBottomLeft > 0
                            ? new PointF(rect.X, rect.Bottom - radiusBottomLeft - 2)
                            : new PointF(rect.X, rect.Bottom); 

                        if (point2.Y > point1.Y)
                            g.DrawLine(pen, point1, point2);
                        #endregion

                        #region Top Side
                        point1 = radiusTopLeft > 0
                            ? new PointF(rect.X + radiusTopLeft + 2, rect.Y)
                            : new PointF(rect.X, rect.Y);

                        point2 = radiusTopRight > 0
                            ? new PointF(rect.Right - radiusTopRight - 2, rect.Y)
                            : new PointF(rect.Right, rect.Y);

                        if (point2.X > point1.X)
                            g.DrawLine(pen, point1, point2);
                        #endregion

                        #region Right Side
                        point1 = radiusTopRight > 0
                            ? new PointF(rect.Right, rect.Y + radiusTopRight + 2)
                            : new PointF(rect.Right, rect.Y);

                        point2 = radiusBottomRight > 0
                            ? new PointF(rect.Right, rect.Bottom - radiusBottomRight - 2)
                            : new PointF(rect.Right, rect.Bottom);

                        if (point2.Y > point1.Y)
                            g.DrawLine(pen, point1, point2);
                        #endregion

                        #region Bottom Side
                        point1 = radiusBottomLeft > 0
                            ? new PointF(rect.X + radiusBottomLeft + 2, rect.Bottom)
                            : new PointF(rect.X, rect.Bottom);

                        point2 = radiusBottomRight > 0
                            ? new PointF(rect.Right - radiusBottomRight - 2, rect.Bottom)
                            : new PointF(rect.Right, rect.Bottom);

                        if (point2.X > point1.X)
                            g.DrawLine(pen, point1, point2);
                        #endregion
                    }
                    #endregion
                }
            }
            finally
            {
                g.SmoothingMode = storedMode;
            }
        }

        public static void PaintBackground(Graphics g, RectangleF rect, StiSimpleBorder border, StiBrush brush, bool isEnabled)
        {
            rect = ShiftRectangleFromBorder(rect, border);

            if (!isEnabled)
                brush = new StiSolidBrush(Color.FromArgb(230, 230, 230));

            StiDrawing.FillRectangle(g, brush, rect);
        }

        public static void PaintCircledBackground(Graphics g, RectangleF rect, StiBrush brush, bool isEnabled)
        {
            var storedMode = g.SmoothingMode;

            try
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                rect = StiRoundedBorderGdiPainter.FitRectangleInCircle(rect);

                if (!isEnabled)
                    brush = new StiSolidBrush(Color.FromArgb(230, 230, 230));

                StiDrawing.FillEllipse(g, brush, rect);
            }
            finally
            {
                g.SmoothingMode = storedMode;
            }
        }

        private static RectangleF ShiftRectangleFromBorder(RectangleF rect, StiSimpleBorder border)
        {
            if (border == null || border.Size <= 1)
                return rect;

            var size = (float)border.Size / 2;
            var side = border.Side;

            if ((side & StiBorderSides.Left) > 0)
            {
                rect.X += size;
                rect.Width -= size;
            }

            if ((side & StiBorderSides.Top) > 0)
            {
                rect.Y += size;
                rect.Height -= size;
            }

            if ((side & StiBorderSides.Right) > 0)
                rect.Width -= size;

            if ((side & StiBorderSides.Bottom) > 0)
                rect.Height -= size;

            return rect;
        }
        #endregion
    }
}