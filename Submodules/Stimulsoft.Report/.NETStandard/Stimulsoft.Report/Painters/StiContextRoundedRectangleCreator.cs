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

using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Stimulsoft.Report.Painters
{
    internal class StiContextRoundedRectangleCreator
    {
        public static List<StiSegmentGeom> CreateWithoutTopSide(RectangleF rect, StiCornerRadius radius, float scale)
        {
            var path = new List<StiSegmentGeom>();

            PointF point1;
            PointF point2;

            var rad = Math.Min(rect.Width / 2, rect.Height / 2);
            var radiusTopLeft = StiRoundedRectangleCreator.GetRadiusTopLeft(radius, scale, rad);
            var radiusTopRight = StiRoundedRectangleCreator.GetRadiusTopRight(radius, scale, rad);
            var radiusBottomLeft = StiRoundedRectangleCreator.GetRadiusBottomLeft(radius, scale, rad);
            var radiusBottomRight = StiRoundedRectangleCreator.GetRadiusBottomRight(radius, scale, rad);

            #region Top-Right-Corner
            if (radiusTopRight != 0f)
            {
                var corner = new RectangleF(rect.Right - 2 * radiusTopRight, rect.Y, 2 * radiusTopRight, 2 * radiusTopRight);
                path.Add(new StiArcSegmentGeom(corner, 270, 90));
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

            path.Add(new StiLineSegmentGeom(point1, point2));
            #endregion

            #region Lower-Right-Corner
            if (radiusBottomRight != 0f)
            {
                var corner = new RectangleF(rect.Right - 2 * radiusBottomRight, rect.Bottom - 2 * radiusBottomRight, 2 * radiusBottomRight, 2 * radiusBottomRight);
                path.Add(new StiArcSegmentGeom(corner, 0, 90));
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

            path.Add(new StiLineSegmentGeom(point1, point2));
            #endregion

            #region Bottom-Left-Corner
            if (radiusBottomLeft != 0f)
            {
                var corner = new RectangleF(rect.X, rect.Bottom - 2 * radiusBottomLeft, 2 * radiusBottomLeft, 2 * radiusBottomLeft);
                path.Add(new StiArcSegmentGeom(corner, 90, 90));
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

            path.Add(new StiLineSegmentGeom(point1, point2));
            #endregion

            #region Top-Left-Corner
            if (radiusTopLeft != 0f)
            {
                var corner = new RectangleF(rect.X, rect.Y, 2 * radiusTopLeft, 2 * radiusTopLeft);
                path.Add(new StiArcSegmentGeom(corner, 180, 90));
            }
            #endregion

            return path;
        }

        public static List<StiSegmentGeom> CreateWithoutBottomSide(RectangleF rect, StiCornerRadius radius, float scale)
        {
            var path = new List<StiSegmentGeom>();

            PointF point1;
            PointF point2;

            var rad = Math.Min(rect.Width / 2, rect.Height / 2);
            var radiusTopLeft = StiRoundedRectangleCreator.GetRadiusTopLeft(radius, scale, rad);
            var radiusTopRight = StiRoundedRectangleCreator.GetRadiusTopRight(radius, scale, rad);
            var radiusBottomLeft = StiRoundedRectangleCreator.GetRadiusBottomLeft(radius, scale, rad);
            var radiusBottomRight = StiRoundedRectangleCreator.GetRadiusBottomRight(radius, scale, rad);

            #region Bottom-Left-Corner
            if (radiusBottomLeft != 0f)
            {
                var corner = new RectangleF(rect.X, rect.Bottom - 2 * radiusBottomLeft, 2 * radiusBottomLeft, 2 * radiusBottomLeft);
                path.Add(new StiArcSegmentGeom(corner, 90, 90));
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

            path.Add(new StiLineSegmentGeom(point1, point2));
            #endregion

            #region Top-Left-Corner
            if (radiusTopLeft != 0f)
            {
                var corner = new RectangleF(rect.X, rect.Y, 2 * radiusTopLeft, 2 * radiusTopLeft);
                path.Add(new StiArcSegmentGeom(corner, 180, 90));
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

            path.Add(new StiLineSegmentGeom(point1, point2));
            #endregion

            #region Top-Right-Corner
            if (radiusTopRight != 0f)
            {
                var corner = new RectangleF(rect.Right - 2 * radiusTopRight, rect.Y, 2 * radiusTopRight, 2 * radiusTopRight);
                path.Add(new StiArcSegmentGeom(corner, 270, 90));
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

            path.Add(new StiLineSegmentGeom(point1, point2));
            #endregion

            #region Lower-Right-Corner
            if (radiusBottomRight != 0f)
            {
                var corner = new RectangleF(rect.Right - 2 * radiusBottomRight, rect.Bottom - 2 * radiusBottomRight, 2 * radiusBottomRight, 2 * radiusBottomRight);
                path.Add(new StiArcSegmentGeom(corner, 0, 90));
            }
            #endregion

            return path;
        }

        internal static List<StiSegmentGeom> CreateWithoutLeftSide(RectangleF rect, StiCornerRadius radius, float scale)
        {
            var path = new List<StiSegmentGeom>();

            PointF point1;
            PointF point2;

            var rad = Math.Min(rect.Width / 2, rect.Height / 2);
            var radiusTopLeft = StiRoundedRectangleCreator.GetRadiusTopLeft(radius, scale, rad);
            var radiusTopRight = StiRoundedRectangleCreator.GetRadiusTopRight(radius, scale, rad);
            var radiusBottomLeft = StiRoundedRectangleCreator.GetRadiusBottomLeft(radius, scale, rad);
            var radiusBottomRight = StiRoundedRectangleCreator.GetRadiusBottomRight(radius, scale, rad);

            #region Top-Left-Corner
            if (radiusTopLeft != 0f)
            {
                var corner = new RectangleF(rect.X, rect.Y, 2 * radiusTopLeft, 2 * radiusTopLeft);
                path.Add(new StiArcSegmentGeom(corner, 180, 90));
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

            path.Add(new StiLineSegmentGeom(point1, point2));
            #endregion

            #region Top-Right-Corner
            if (radiusTopRight != 0f)
            {
                var corner = new RectangleF(rect.Right - 2 * radiusTopRight, rect.Y, 2 * radiusTopRight, 2 * radiusTopRight);
                path.Add(new StiArcSegmentGeom(corner, 270, 90));
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

            path.Add(new StiLineSegmentGeom(point1, point2));
            #endregion

            #region Lower-Right-Corner
            if (radiusBottomRight != 0f)
            {
                var corner = new RectangleF(rect.Right - 2 * radiusBottomRight, rect.Bottom - 2 * radiusBottomRight, 2 * radiusBottomRight, 2 * radiusBottomRight);
                path.Add(new StiArcSegmentGeom(corner, 0, 90));
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

            path.Add(new StiLineSegmentGeom(point1, point2));
            #endregion

            #region Bottom-Left-Corner
            if (radiusBottomLeft != 0f)
            {
                var corner = new RectangleF(rect.X, rect.Bottom - 2 * radiusBottomLeft, 2 * radiusBottomLeft, 2 * radiusBottomLeft);
                path.Add(new StiArcSegmentGeom(corner, 90, 90));
            }
            #endregion

            return path;
        }

        internal static List<StiSegmentGeom> CreateWithoutRightSide(RectangleF rect, StiCornerRadius radius, float scale)
        {
            var path = new List<StiSegmentGeom>();

            PointF point1;
            PointF point2;

            var rad = Math.Min(rect.Width / 2, rect.Height / 2);
            var radiusTopLeft = StiRoundedRectangleCreator.GetRadiusTopLeft(radius, scale, rad);
            var radiusTopRight = StiRoundedRectangleCreator.GetRadiusTopRight(radius, scale, rad);
            var radiusBottomLeft = StiRoundedRectangleCreator.GetRadiusBottomLeft(radius, scale, rad);
            var radiusBottomRight = StiRoundedRectangleCreator.GetRadiusBottomRight(radius, scale, rad);

            #region Lower-Right-Corner
            if (radiusBottomRight != 0f)
            {
                var corner = new RectangleF(rect.Right - 2 * radiusBottomRight, rect.Bottom - 2 * radiusBottomRight, 2 * radiusBottomRight, 2 * radiusBottomRight);
                path.Add(new StiArcSegmentGeom(corner, 0, 90));
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

            path.Add(new StiLineSegmentGeom(point1, point2));
            #endregion

            #region Bottom-Left-Corner
            if (radiusBottomLeft != 0f)
            {
                var corner = new RectangleF(rect.X, rect.Bottom - 2 * radiusBottomLeft, 2 * radiusBottomLeft, 2 * radiusBottomLeft);
                path.Add(new StiArcSegmentGeom(corner, 90, 90));
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

            path.Add(new StiLineSegmentGeom(point1, point2));
            #endregion

            #region Top-Left-Corner
            if (radiusTopLeft != 0f)
            {
                var corner = new RectangleF(rect.X, rect.Y, 2 * radiusTopLeft, 2 * radiusTopLeft);
                path.Add(new StiArcSegmentGeom(corner, 180, 90));
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

            path.Add(new StiLineSegmentGeom(point1, point2));
            #endregion

            #region Top-Right-Corner
            if (radiusTopRight != 0f)
            {
                var corner = new RectangleF(rect.Right - 2 * radiusTopRight, rect.Y, 2 * radiusTopRight, 2 * radiusTopRight);
                path.Add(new StiArcSegmentGeom(corner, 270, 90));
            }
            #endregion

            return path;
        }

        public static List<StiSegmentGeom> Create(RectangleF rect, StiCornerRadius radius, float scale)
        {
            var path = new List<StiSegmentGeom>();
            
            PointF point1;
            PointF point2;

            var rad = Math.Min(rect.Width / 2, rect.Height / 2);
            var radiusTopLeft = StiRoundedRectangleCreator.GetRadiusTopLeft(radius, scale, rad);
            var radiusTopRight = StiRoundedRectangleCreator.GetRadiusTopRight(radius, scale, rad);
            var radiusBottomLeft = StiRoundedRectangleCreator.GetRadiusBottomLeft(radius, scale, rad);
            var radiusBottomRight = StiRoundedRectangleCreator.GetRadiusBottomRight(radius, scale, rad);

            #region Top-Left-Corner
            if (radiusTopLeft != 0f)
            {
                var corner = new RectangleF(rect.X, rect.Y, 2 * radiusTopLeft, 2 * radiusTopLeft);
                path.Add(new StiArcSegmentGeom(corner, 180, 90));
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

            path.Add(new StiLineSegmentGeom(point1, point2));
            #endregion

            #region Top-Right-Corner
            if (radiusTopRight != 0f)
            {
                var corner = new RectangleF(rect.Right - 2 * radiusTopRight, rect.Y, 2 * radiusTopRight, 2 * radiusTopRight);
                path.Add(new StiArcSegmentGeom(corner, 270, 90));
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

            path.Add(new StiLineSegmentGeom(point1, point2));
            #endregion

            #region Lower-Right-Corner
            if (radiusBottomRight != 0f)
            {
                var corner = new RectangleF(rect.Right - 2 * radiusBottomRight, rect.Bottom - 2 * radiusBottomRight, 2 * radiusBottomRight, 2 * radiusBottomRight);
                path.Add(new StiArcSegmentGeom(corner, 0, 90));
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

            path.Add(new StiLineSegmentGeom(point1, point2));
            #endregion

            #region Bottom-Left-Corner
            if (radiusBottomLeft != 0f)
            {
                var corner = new RectangleF(rect.X, rect.Bottom - 2 * radiusBottomLeft, 2 * radiusBottomLeft, 2 * radiusBottomLeft);
                path.Add(new StiArcSegmentGeom(corner, 90, 90));
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

            path.Add(new StiLineSegmentGeom(point1, point2));
            #endregion

            return path;
        }
    }
}
