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

using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Report.Chart;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Stimulsoft.Report.Painters
{
    public class StiLineSparklinesCellPainter
    {
        internal static void Draw(StiContext context, RectangleD rect, object[] array, float zoom,
            Color sparklineColor, bool showArea, bool showFirstLastMarker = true, bool showHighLowMarker = false)
        {
            if (array == null || array.Length < 2) return;
            rect.Inflate(-4 * zoom, -rect.Height / 4);

            var values = array.Select(StiValueHelper.TryToDouble).ToArray();
            if (values.All(v => v == 0)) return;

            var min = values.Min(v => v);
            var max = values.Max(v => v);
            if (min == 0 && max == 0) return;

            if (min > 0)
            {
                values = values.Select(v => v - min).ToArray();
                max -= min;
                min = 0;
            }

            if (max < 0)
            {
                values = values.Select(v => v + -max).ToArray();
                min += -max;
                max = 0;
            }

            var stepWidth = (float)(rect.Width / (values.Length - 1));//steps
            var totalHeight = max + Math.Abs(min);
            if (totalHeight == 0d) return;

            var maxPart = rect.Height * max / totalHeight;

            #region Calculate Rects
            var x = (float)rect.X;
            var points = values.Select(value =>
            {
                var valuePart = rect.Height * Math.Abs(value) / totalHeight;
                var y = (float)(rect.Y + (value > 0 ? maxPart - valuePart : maxPart));
                var point = new PointF(x, value > 0 ? y : y + (float)valuePart);

                x += stepWidth;

                return point;
            }).ToArray();
            #endregion

            if (points.Length > 1)
            {
                points = SimplifyPoints(points);

                if (showArea)
                    DrawArea(context, rect, points, zoom, sparklineColor);

                DrawLines(context, points, zoom, sparklineColor);

                if (showFirstLastMarker)
                    DrawFirstLastMarkers(context, points, zoom);

                if (showHighLowMarker)
                    DrawHighLowMarkers(context, points, zoom);
            }
        }

        private static void DrawArea(StiContext context, RectangleD rect, PointF[] points, float zoom, Color sparklineColor)
        {
            var path = new List<StiSegmentGeom>();

            var pointsPath = points.ToList();
            pointsPath.Insert(0, new PointF(points.FirstOrDefault().X, (float)(rect.Bottom + 2 * zoom)));
            pointsPath.Add(new PointF(points.LastOrDefault().X, (float)(rect.Bottom + 2 * zoom)));

            path.Add(new StiLinesSegmentGeom(pointsPath.ToArray()));
            context.FillPath(Color.FromArgb(60, sparklineColor), path, rect);
        }

        private static void DrawLines(StiContext context, PointF[] points, float zoom, Color sparklineColor)
        {
            var minY = points.Min(p => p.Y);
            var maxY = points.Max(p => p.Y);
            var firstPoint = points.FirstOrDefault();
            var lastPoint = points.LastOrDefault();

            var clipRect = new RectangleF(firstPoint.X, minY, lastPoint.X - firstPoint.X, maxY - minY);
            clipRect.Inflate(1, 1);

            context.PushSmoothingModeToAntiAlias();
            
            var penWidth = 2 * zoom;
            context.DrawLines(new StiPenGeom(sparklineColor, penWidth), points);
            
            context.PopSmoothingMode();            
        }

        private static void DrawFirstLastMarkers(StiContext context, PointF[] points, float zoom)
        {
            DrawMarker(context, points.FirstOrDefault(), Color.DimGray, zoom);
            DrawMarker(context, points.LastOrDefault(), Color.DimGray, zoom);
        }

        private static void DrawHighLowMarkers(StiContext context, PointF[] points, float zoom)
        {
            if (points.Length < 2) return;

            var maxPoint = points.FirstOrDefault();
            var minPoint = points.FirstOrDefault();

            foreach (var p in points)
            {
                if (p.Y > minPoint.Y) 
                    minPoint = p;
                
                if (p.Y < maxPoint.Y) 
                    maxPoint = p;
            }

            DrawMarker(context, maxPoint, Color.Green, zoom);
            DrawMarker(context, minPoint, Color.Red, zoom);
        }

        private static void DrawMarker(StiContext context, PointF p, Color color, float zoom)
        {
            context.PushSmoothingModeToAntiAlias();
            
            var w = 3.5f * zoom;
            context.FillEllipse(Color.White, new RectangleF(p.X - w, p.Y - w, w * 2, w * 2));
            w--;
            context.FillEllipse(color, new RectangleF(p.X - w, p.Y - w, w * 2, w * 2));
            
            context.PopSmoothingMode();
        }

        private static PointF[] SimplifyPoints(PointF[] points)
        {
            if (points.Length <= 30)
                return points;

            var pointsD = StiSimplifyHelper.Simplify(points.Select(p => new PointD(p.X, p.Y)).ToArray());
            return pointsD.Select(p => p.ToPointF()).ToArray();
        }
    }
}
