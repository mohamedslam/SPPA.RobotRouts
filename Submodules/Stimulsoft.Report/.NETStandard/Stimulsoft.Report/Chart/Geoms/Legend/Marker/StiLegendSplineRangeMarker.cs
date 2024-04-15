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
using System.Drawing;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using System.Collections.Generic;

namespace Stimulsoft.Report.Chart
{
    public class StiLegendSplineRangeMarker : IStiLegendMarker
    {
        public void Draw(StiContext context, IStiSeries series, RectangleF rect, int colorIndex, int colorCount, int index)
        {
            var splineSeries = series as StiSplineRangeSeries;

            var marker = splineSeries.Marker;
            var lineStyle = splineSeries.LineStyle;
            var lineMarker = splineSeries.LineMarker;
            var lineWidth = splineSeries.LineWidth;
            var lineColor = splineSeries.LineColor;
            var lighting = splineSeries.Lighting;
            var seriesBrush = splineSeries.Brush;

            List<StiSegmentGeom> path = new List<StiSegmentGeom>();
            var width1 = rect.Width / 5;
            var height1 = rect.Height / 5;

            path.Add(new StiLineSegmentGeom(rect.X, rect.Y + height1 * 5, rect.X, rect.Y + height1 * 3));

            PointF[] points1 = new PointF[] 
            {
                new PointF(rect.X, rect.Y + height1 * 3),
                new PointF(rect.X + width1 * 2, rect.Y),
                new PointF(rect.X + width1 * 4, rect.Y + height1 * 2),
                new PointF(rect.X + width1 * 5, rect.Y + height1 * 1)
            };
            path.Add(new StiCurveSegmentGeom(points1, splineSeries.Tension));

            path.Add(new StiLineSegmentGeom(rect.X + width1 * 5, rect.Y + height1 * 1, rect.X + width1 * 5, rect.Y + height1 * 3));

            PointF[] points2 = new PointF[]
            {
                new PointF(rect.X + width1 * 5, rect.Y + height1 * 3),
                new PointF(rect.X + width1 * 4, rect.Y + height1 * 4),
                new PointF(rect.X + width1 * 2, rect.Y + height1 * 4),
                new PointF(rect.X, rect.Y + height1 * 5)
            };
            path.Add(new StiCurveSegmentGeom(points2, splineSeries.Tension));
            

            StiPenGeom pen = new StiPenGeom(lineColor);

            context.PushSmoothingModeToAntiAlias();
            context.FillPath(seriesBrush, path, rect, null, index);
            context.DrawCurve(pen, points1, splineSeries.Tension);
            context.DrawCurve(pen, points2, splineSeries.Tension);

            context.PopSmoothingMode();
        }
    }
}
