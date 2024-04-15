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

namespace Stimulsoft.Report.Chart
{
    internal class StiPointHelper
    {
        #region enum PointClassify
        private enum PointClassify
        {
            Left,
            Right,
            Beyond,
            Behind,
            Between,
            Origin,
            Destination
        };
        #endregion

        #region Methods
        private static PointClassify GetPointClassify(PointF basePoint, PointF point1, PointF point2)
        {
            var a = new PointF(point2.X - point1.X, point2.Y - point1.Y);
            var b = new PointF(basePoint.X - point1.X, basePoint.Y - point1.Y);

            var sa = a.X * b.Y - b.X * a.Y;

            if (sa > 0.0)
                return PointClassify.Left;

            if (sa < 0.0)
                return PointClassify.Right;

            if (a.X * b.X < 0.0 || a.Y * b.Y < 0.0)
                return PointClassify.Behind;

            if (Math.Sqrt(a.X * a.X + a.Y * a.Y) < Math.Sqrt(b.X * b.X + b.Y * b.Y))
                return PointClassify.Beyond;

            if (point1.Equals(basePoint))
                return PointClassify.Origin;

            if (point2.Equals(basePoint))
                return PointClassify.Destination;

            return PointClassify.Between;
        }

        public static bool IsPointInTriangle(PointF p, PointF a, PointF b, PointF c)
        {
            return GetPointClassify(p, a, b) != PointClassify.Left &&
                   GetPointClassify(p, b, c) != PointClassify.Left &&
                   GetPointClassify(p, c, a) != PointClassify.Left;
        }

        public static bool IsPointInPolygon(PointF p, PointF[] points)
        {
            for (var index = 0; index < points.Length; index++)
            {
                if (GetPointClassify(p, points[index], index + 1 < points.Length ? points[index + 1] : points[0]) == PointClassify.Left) return false;
            }
            return true;
        }

        public static PointF[] GetLineOffsetRectangle(PointF point1, PointF point2, double offset)
        {
            var angle = Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            var halfDist = offset / 2;

            var points = new PointF[4];

            points[0] = new PointF((float)(point1.X + halfDist * Math.Cos(angle + 0.5 * Math.PI)), (float)(point1.Y + halfDist * Math.Sin(angle + 0.5 * Math.PI)));
            points[1] = new PointF((float)(point2.X + halfDist * Math.Cos(angle + 0.5 * Math.PI)), (float)(point2.Y + halfDist * Math.Sin(angle + 0.5 * Math.PI)));
            points[2] = new PointF((float)(point2.X + halfDist * Math.Cos(angle + 1.5 * Math.PI)), (float)(point2.Y + halfDist * Math.Sin(angle + 1.5 * Math.PI)));
            points[3] = new PointF((float)(point1.X + halfDist * Math.Cos(angle + 1.5 * Math.PI)), (float)(point1.Y + halfDist * Math.Sin(angle + 1.5 * Math.PI)));

            return points;
        }

        public static bool IsLineContainsPoint(PointF startPoint, PointF endPoint, float offset, PointF point)
        {
            var points = GetLineOffsetRectangle(startPoint, endPoint, offset);
            return IsPointInPolygon(point, points);
        }
        
        public static PointF?[] OptimizePoints(PointF?[] points)
        {
            if (points.Length < 800) return points;

            const float step = 1f;

            int index = 0;
            float minx = 0;
            float miny = 0;
            //float maxx = 0;
            float maxy = 0;

            var points2 = new List<PointF?>();

            while (index < points.Length)
            {
                if (points[index] == null)
                {
                    while ((index < points.Length) && (points[index] == null)) index++;
                    points2.Add(null);
                    continue;
                }

                PointF pf = points[index].Value;
                index++;
                if ((index < points.Length) && points[index] != null && (points[index] != null) && ((points[index].Value.X - pf.X) < step))
                {
                    minx = pf.X;
                    //maxx = pf.X;
                    miny = pf.Y;
                    maxy = pf.Y;
                    float firstY = pf.Y;
                    float lastY = pf.Y;

                    while ((index < points.Length) && points[index] != null && (points[index] != null) && (points[index].Value.X - minx) < step)
                    {
                        //maxx = Math.Max(maxx, points[index].Value.X);
                        miny = Math.Min(miny, points[index].Value.Y);
                        maxy = Math.Max(maxy, points[index].Value.Y);
                        lastY = points[index].Value.Y;
                        index++;
                    }

                    if (lastY > firstY)
                    {
                        points2.Add(new PointF?(new PointF(minx, miny)));
                        points2.Add(new PointF?(new PointF(minx + step / 2, maxy)));
                    }
                    else
                    {
                        points2.Add(new PointF?(new PointF(minx, maxy)));
                        points2.Add(new PointF?(new PointF(minx + step / 2, miny)));
                    }
                }
                else
                {
                    points2.Add(pf);
                }
            }

            return points2.ToArray();
        }
        #endregion
    }
}