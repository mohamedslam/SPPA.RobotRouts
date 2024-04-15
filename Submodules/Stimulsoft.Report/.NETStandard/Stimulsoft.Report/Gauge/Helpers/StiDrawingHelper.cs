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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
#endif

namespace Stimulsoft.Report.Gauge.Helpers
{
    public static class StiDrawingHelper
    {
        #region Methods
        public static GraphicsPath GetRoundedPath(RectangleF rect, int offset, int leftTop, int rightTop, int rightBottom, int leftBottom)
        {
            var gp = new GraphicsPath();

            float right = rect.X + rect.Width - offset;
            float bottom = rect.Y + rect.Height - offset;

            if (rightTop != 0)
            {
                gp.AddLine(rect.X + leftTop, rect.Y, right - leftTop * 2, rect.Y);
                gp.AddArc(right - leftTop * 2, rect.Y, leftTop * 2, leftTop * 2, 270, 90);
            }
            else
            {
                gp.AddLine(rect.X + leftTop, rect.Y, right, rect.Y);
            }

            if (rightBottom != 0)
            {
                gp.AddLine(right, rect.Y + rightTop, right, bottom - rightTop * 2);
                gp.AddArc(right - rightTop * 2, bottom - rightTop * 2, rightTop * 2, rightTop * 2, 0, 90);
            }
            else
            {
                gp.AddLine(right, rect.Y + rightTop, right, bottom);
            }

            if (leftBottom != 0)
            {
                gp.AddLine(right - rightBottom * 2, bottom, rect.X + rightBottom, bottom);
                gp.AddArc(rect.X, bottom - rightBottom * 2, rightBottom * 2, rightBottom * 2, 90, 90);
            }
            else
            {
                gp.AddLine(right - rightBottom, bottom, rect.X, bottom);
            }

            if (leftTop != 0)
            {
                gp.AddLine(rect.X, bottom - leftTop * 2, rect.X, rect.Y + leftTop);
                gp.AddArc(rect.X, rect.Y, leftTop * 2, leftTop * 2, 180, 90);
            }
            else
            {
                gp.AddLine(rect.X, bottom - leftTop, rect.X, rect.Y);
            }

            gp.CloseFigure();

            return gp;
        }
        #endregion

        #region Methods.ArcHelper
        internal static readonly float PiDiv180 = (float)(Math.PI / 180f);
        private static readonly float FourDivThree = 4f / 3f;

        public static GraphicsPath GetArcGeometry(RectangleF rect, float startAngle, float sweepAngle, float startWidth, float endWidth)
        {
            var path = new GraphicsPath();
            var centerPoint = new PointF(StiRectangleHelper.CenterX(rect), StiRectangleHelper.CenterY(rect));
            float radius = Math.Min(rect.Width / 2, rect.Height / 2);

            var lastPoint = PointF.Empty;
            var firstPoint = PointF.Empty;
            float steps = Round(Math.Abs(sweepAngle / 90));
            float stepAngle = sweepAngle / steps;
            float currentStartAngle = startAngle;

            #region First Arc
            var isFirst = true;
            for (int indexStep = 0; indexStep < steps; indexStep++)
            {
                var points = ConvertArcToCubicBezier(centerPoint, radius, currentStartAngle, stepAngle);
                if (isFirst)
                {
                    firstPoint = points[0];
                    lastPoint = firstPoint;
                    isFirst = false;
                }
                path.AddBezier(lastPoint, points[1], points[2], points[3]);
                lastPoint = points[3];

                currentStartAngle += stepAngle;
            }
            #endregion

            #region Second Arc
            float secondStartRadius = radius - (rect.Width * startWidth);
            float secondEndRadius = radius - (rect.Width * endWidth);

            if (secondStartRadius <= 0 || secondEndRadius <= 0) return null;

            float offsetSecondRadius = secondStartRadius - secondEndRadius;

            float offsetStep = 1 / steps;
            float offset = 0;
            isFirst = true;

            currentStartAngle = startAngle + sweepAngle;
            for (int indexStep = 0; indexStep < steps; indexStep++)
            {
                float startRadius = secondStartRadius - (offsetSecondRadius * offset);
                float endRadius = secondStartRadius - (offsetSecondRadius * (offset + offsetStep));

                var points = ConvertArcToCubicBezier(centerPoint, startRadius, endRadius, currentStartAngle, -stepAngle);
                if (isFirst)
                {
                    path.AddLine(lastPoint, points[0]);
                    lastPoint = points[0];
                    isFirst = false;
                }
                path.AddBezier(lastPoint, points[1], points[2], points[3]);

                lastPoint = points[3];
                currentStartAngle -= stepAngle;
                offset += offsetStep;
            }

            path.AddLine(lastPoint, firstPoint);
            #endregion

            return path;
        }

        public static GraphicsPath GetRadialRangeGeometry(PointF centerPoint, float startAngle, float sweepAngle,
            float radius1, float radius2, float radius3, float radius4)
        {
            var path = new GraphicsPath();
            var lastPoint = PointF.Empty;
            var firstPoint = PointF.Empty;
            float steps = Round(Math.Abs(sweepAngle / 90));
            float stepAngle = sweepAngle / steps;

            #region First Arc
            float restRadius = radius1 - radius2;
            float offsetStep = 1 / (steps);
            float offset = 0;
            var isFirst = true;
            var currentStartAngle = startAngle + sweepAngle;
            for (int indexStep = 0; indexStep < steps; indexStep++)
            {
                float startRadius = radius1 - (restRadius * offset);
                float endRadius = radius1 - (restRadius * (offset + offsetStep));

                var points = ConvertArcToCubicBezier(centerPoint, startRadius, endRadius, currentStartAngle, -stepAngle);
                if (isFirst)
                {
                    lastPoint = points[0];
                    firstPoint = lastPoint;
                    isFirst = false;
                }
                path.AddBezier(lastPoint, points[1], points[2], points[3]);
                lastPoint = points[3];

                currentStartAngle -= stepAngle;
                offset += offsetStep;
            }
            #endregion

            #region Second Arc
            stepAngle = sweepAngle / steps;
            restRadius = radius3 - radius4;

            offset = 0;
            isFirst = true;

            currentStartAngle = startAngle;
            for (int indexStep = 0; indexStep < steps; indexStep++)
            {
                float startRadius = radius3 - (restRadius * offset);
                float endRadius = radius3 - (restRadius * (offset + offsetStep));

                var points = ConvertArcToCubicBezier(centerPoint, startRadius, endRadius, currentStartAngle, stepAngle);
                if (isFirst)
                {
                    path.AddLine(lastPoint, points[0]);
                    lastPoint = points[0];
                    isFirst = false;
                }
                path.AddBezier(lastPoint, points[1], points[2], points[3]);
                lastPoint = points[3];

                currentStartAngle += stepAngle;
                offset += offsetStep;
            }

            path.AddLine(lastPoint, firstPoint);
            #endregion

            return path;
        }

        private static float Round(float value)
        {
            int value1 = (int)value;
            float rest = value - value1;
            return (rest > 0) ? (float)(value1 + 1) : (float)(value1);
        }

        internal static List<PointF> ConvertArcToCubicBezier(PointF centerPoint, float radius, float startAngle, float sweepAngle)
        {
            float startAngle1 = startAngle * PiDiv180;
            float sweepAngle1 = sweepAngle * PiDiv180;
            float endAngle1 = startAngle1 + sweepAngle1;

            float x1 = centerPoint.X + radius * (float)Math.Cos(startAngle1);
            float y1 = centerPoint.Y + radius * (float)Math.Sin(startAngle1);

            float x2 = centerPoint.X + radius * (float)Math.Cos(endAngle1);
            float y2 = centerPoint.Y + radius * (float)Math.Sin(endAngle1);

            float l = radius * FourDivThree * (float)Math.Tan(0.25 * sweepAngle1);
            float aL = (float)Math.Atan(l / radius);
            float radL = radius / (float)Math.Cos(aL);

            aL += startAngle1;
            float ax1 = centerPoint.X + radL * (float)Math.Cos(aL);
            float ay1 = centerPoint.Y + radL * (float)Math.Sin(aL);

            aL = (float)Math.Atan(-l / radius);
            aL += endAngle1;
            float ax2 = centerPoint.X + radL * (float)Math.Cos(aL);
            float ay2 = centerPoint.Y + radL * (float)Math.Sin(aL);

            var points = new List<PointF>(4);
            points.Add(new PointF(x1, y1));
            points.Add(new PointF(ax1, ay1));
            points.Add(new PointF(ax2, ay2));
            points.Add(new PointF(x2, y2));

            return points;
        }

        internal static List<PointF> ConvertArcToCubicBezier(PointF centerPoint, float radius1, float radius2, float startAngle, float sweepAngle)
        {
            float startAngle1 = startAngle * PiDiv180;
            float sweepAngle1 = sweepAngle * PiDiv180;
            float endAngle = startAngle1 + sweepAngle1;

            float x1 = centerPoint.X + radius1 * (float)Math.Cos(startAngle1);
            float y1 = centerPoint.Y + radius1 * (float)Math.Sin(startAngle1);

            float x2 = centerPoint.X + radius2 * (float)Math.Cos(endAngle);
            float y2 = centerPoint.Y + radius2 * (float)Math.Sin(endAngle);

            float rest = (radius1 - radius2) / 3;
            radius1 -= rest;
            radius2 += rest;

            float l = radius1 * FourDivThree * (float)Math.Tan(0.25 * sweepAngle1);
            float aL = (float)Math.Atan(l / radius1);
            float radL = radius1 / (float)Math.Cos(aL);

            aL += startAngle1;
            float ax1 = centerPoint.X + radL * (float)Math.Cos(aL);
            float ay1 = centerPoint.Y + radL * (float)Math.Sin(aL);

            aL = (float)Math.Atan(-l / radius1);
            aL += endAngle;
            float ax2 = centerPoint.X + radL * (float)Math.Cos(aL);
            float ay2 = centerPoint.Y + radL * (float)Math.Sin(aL);

            var points = new List<PointF>(4)
            {
                new PointF(x1, y1),
                new PointF(ax1, ay1),
                new PointF(ax2, ay2),
                new PointF(x2, y2)
            };

            return points;
        }
        #endregion
    }
}