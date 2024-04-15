#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
#endif

namespace Stimulsoft.Base.Drawing
{
    public static class StiRoundedRectangleCreator
    {
        public static GraphicsPath Create(RectangleF rect, float radius)
        {
            return Create(rect, new StiCornerRadius(radius));
        }

        public static GraphicsPath Create(RectangleF rect, StiCornerRadius radius, float scale = 1)
        {
            var path = new GraphicsPath();

            PointF point1;
            PointF point2;

            var rad = Math.Min(rect.Width / 2, rect.Height / 2);
            var radiusTopLeft = GetRadiusTopLeft(radius, scale, rad);
            var radiusTopRight = GetRadiusTopRight(radius, scale, rad);
            var radiusBottomLeft = GetRadiusBottomLeft(radius, scale, rad);
            var radiusBottomRight = GetRadiusBottomRight(radius, scale, rad);

            #region Top-Left-Corner
            if (radiusTopLeft != 0f)
            {
                var corner = new RectangleF(rect.X, rect.Y, 2 * radiusTopLeft, 2 * radiusTopLeft);
                path.AddArc(corner, 180, 90);
                point1 = new PointF(rect.X + radiusTopLeft, rect.Y);
            }
            else
            {
                point1 = new PointF(rect.X, rect.Y);
            }
            #endregion

            #region Top-Side
            if (radiusTopRight != 0f)
                point2 = new PointF(rect.Right - radiusTopRight, rect.Y);
            else
                point2 = new PointF(rect.Right, rect.Y);

            path.AddLine(point1, point2);
            #endregion

            #region Top-Right-Corner
            if (radiusTopRight != 0f)
            {
                var corner = new RectangleF(rect.Right - 2 * radiusTopRight, rect.Y, 2 * radiusTopRight, 2 * radiusTopRight);
                path.AddArc(corner, 270, 90);
                point1 = new PointF(rect.Right, rect.Y + radiusTopRight);
            }
            else
            {
                point1 = new PointF(rect.Right, rect.Y);
            }
            #endregion

            #region Right-Side
            if (radiusBottomRight != 0f)
                point2 = new PointF(rect.Right, rect.Bottom - radiusBottomRight);
            else
                point2 = new PointF(rect.Right, rect.Bottom);

            path.AddLine(point1, point2);
            #endregion

            #region Lower-Right-Corner
            if (radiusBottomRight != 0f)
            {
                var corner = new RectangleF(rect.Right - 2 * radiusBottomRight, rect.Bottom - 2 * radiusBottomRight, 2 * radiusBottomRight, 2 * radiusBottomRight);
                path.AddArc(corner, 0, 90);
                point1 = new PointF(rect.Right - radiusBottomRight, rect.Bottom);
            }
            else
            {
                point1 = new PointF(rect.Right, rect.Bottom);
            }
            #endregion

            #region Bottom-Side
            if (radiusBottomLeft != 0f)
                point2 = new PointF(rect.X + radiusBottomLeft, rect.Bottom);
            else
                point2 = new PointF(rect.X, rect.Bottom);

            path.AddLine(point1, point2);
            #endregion

            #region Bottom-Left-Corner
            if (radiusBottomLeft != 0f)
            {
                var corner = new RectangleF(rect.X, rect.Bottom - 2 * radiusBottomLeft, 2 * radiusBottomLeft, 2 * radiusBottomLeft);
                path.AddArc(corner, 90, 90);
                point1 = new PointF(rect.X, rect.Bottom - radiusBottomLeft);
            }
            else
            {
                point1 = new PointF(rect.X, rect.Bottom);
            }
            #endregion

            #region Left-Side
            if (radiusTopLeft != 0f)
                point2 = new PointF(rect.X, rect.Y + radiusTopLeft);
            else
                point2 = new PointF(rect.X, rect.Y);

            path.AddLine(point1, point2);
            #endregion

            path.CloseFigure();

            return path;
        }
        
        public static void DrawRoundedRect(Graphics g, Pen pen, RectangleF rect, StiCornerRadius radius, StiBorderSides sides, float scale)
        {
            scale = (float)(scale * StiScale.Factor);

            rect.Inflate(-pen.Width / 2, -pen.Width / 2);

            var isTopSide = (sides & StiBorderSides.Top) > 0;
            var isRightSide = (sides & StiBorderSides.Right) > 0;
            var isBottomSide = (sides & StiBorderSides.Bottom) > 0;
            var isLeftSide = (sides & StiBorderSides.Left) > 0;

            var rad = Math.Min(rect.Width / 2, rect.Height / 2);
            var radiusTopLeft = GetRadiusTopLeft(radius, scale, rad);
            var radiusTopRight = GetRadiusTopRight(radius, scale, rad);
            var radiusBottomLeft = GetRadiusBottomLeft(radius, scale, rad);
            var radiusBottomRight = GetRadiusBottomRight(radius, scale, rad);

            #region Top-Left-Corner
            if (radiusTopLeft > 0 && isLeftSide && isTopSide)
            {
                var corner = new RectangleF(rect.X, rect.Y, 2 * radiusTopLeft, 2 * radiusTopLeft);
                g.DrawArc(pen, corner, 180, 90);
            }
            #endregion

            #region Top-Side
            if (isTopSide)
            {
                var pointTop1 = rect.Location;
                var pointTop2 = new PointF(rect.Right, rect.Y);

                if (radiusTopLeft > 0)
                    pointTop1.X += radiusTopLeft;

                if (radiusTopRight > 0)
                    pointTop2.X -= radiusTopRight;

                g.DrawLine(pen, pointTop1, pointTop2);
            }
            #endregion

            #region Top-Right-Corner
            if (radiusTopRight > 0f && isTopSide && isRightSide)
            {
                var corner = new RectangleF(rect.Right - 2 * radiusTopRight, rect.Y, 2 * radiusTopRight, 2 * radiusTopRight);
                g.DrawArc(pen, corner, 270, 90);
            }
            #endregion

            #region Right-Side
            if (isRightSide)
            {
                var pointRight1 = new PointF(rect.Right, rect.Y);
                var pointRight2 = new PointF(rect.Right, rect.Bottom);

                if (radiusTopRight > 0)
                    pointRight1.Y += radiusTopRight;

                if (radiusBottomRight > 0)
                    pointRight2.Y -= radiusBottomRight;

                g.DrawLine(pen, pointRight1, pointRight2);
            }
            #endregion

            #region Lower-Right-Corner
            if (radiusBottomRight > 0f && isRightSide && isBottomSide)
            {
                var corner = new RectangleF(rect.Right - 2 * radiusBottomRight, rect.Bottom - 2 * radiusBottomRight, 2 * radiusBottomRight, 2 * radiusBottomRight);
                g.DrawArc(pen, corner, 0, 90);
            }
            #endregion

            #region Bottom-Side
            if (isBottomSide)
            {
                var pointBottom1 = new PointF(rect.Right, rect.Bottom);
                var pointBottom2 = new PointF(rect.X, rect.Bottom);

                if (radiusBottomRight > 0)
                    pointBottom1.X -= radiusBottomRight;

                if (radiusBottomLeft > 0)
                    pointBottom2.X += radiusBottomLeft;

                g.DrawLine(pen, pointBottom1, pointBottom2);
            }
            #endregion

            #region Bottom-Left-Corner
            if (radiusBottomLeft > 0f && isBottomSide && isLeftSide)
            {
                var corner = new RectangleF(rect.X, rect.Bottom - 2 * radiusBottomLeft, 2 * radiusBottomLeft, 2 * radiusBottomLeft);
                g.DrawArc(pen, corner, 90, 90);
            }
            #endregion

            #region Left-Side
            if (isLeftSide)
            {
                var pointLeft1 = new PointF(rect.X, rect.Y);
                var pointLeft2 = new PointF(rect.X, rect.Bottom);

                if (radiusTopLeft > 0)
                    pointLeft1.Y += radiusTopLeft;

                if (radiusBottomLeft > 0)
                    pointLeft2.Y -= radiusBottomLeft;

                g.DrawLine(pen, pointLeft1, pointLeft2);
            }
            #endregion
        }

        internal static float GetRadiusTopLeft(StiCornerRadius radius, float scale, float rad)
        {
            var radiusTopLeft = radius != null ? radius.TopLeft * scale : 0;
            radiusTopLeft = Math.Max(radiusTopLeft, 0);
            return Math.Min(radiusTopLeft, rad);
        }

        internal static float GetRadiusTopRight(StiCornerRadius radius, float scale, float rad)
        {
            var radiusTopRight = radius != null ? radius.TopRight * scale : 0;
            radiusTopRight = Math.Max(radiusTopRight, 0);
            return Math.Min(radiusTopRight, rad);
        }

        internal static float GetRadiusBottomLeft(StiCornerRadius radius, float scale, float rad)
        {
            var radiusBottomLeft = radius != null ? radius.BottomLeft * scale : 0;
            radiusBottomLeft = Math.Max(radiusBottomLeft, 0);
            return Math.Min(radiusBottomLeft, rad);
        }

        internal static float GetRadiusBottomRight(StiCornerRadius radius, float scale, float rad)
        {
            var radiusBottomRight = radius != null ? radius.BottomRight * scale : 0;
            radiusBottomRight = Math.Max(radiusBottomRight, 0);
            return Math.Min(radiusBottomRight, rad);
        }
    }
}