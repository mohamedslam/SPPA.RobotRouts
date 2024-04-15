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
using System.Collections.Generic;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using System.Linq;

namespace Stimulsoft.Report.Chart
{
    public class StiRangeSeriesGeom : StiLineSeriesGeom
    {
        #region Properties
        public PointF?[] PointsEnd { get; }
        #endregion

        #region Methods
        public override void Draw(StiContext context)
        {
            var areaSeries = this.Series as IStiLineRangeSeries;

            var list = StiNullableDrawing.GetPointsList(this.Points);
            var listEnd = StiNullableDrawing.GetPointsList(this.PointsEnd);

            if (list.Count < 1 || listEnd.Count < 1) return;

            var points = list[0];
            var pointsEnd = listEnd[0];

            int count = Math.Min(points.Length, pointsEnd.Length);

            var pointsLine = new List<PointF>();
            var pointsLineEnd = new List<PointF>();
            var pointsLineIds = new List<string>();
            var pointsLineEndIds = new List<string>();

            for (int index = 0; index < count; index++)
            {
                #region Point and Point Next
                PointF? point = points[index];
                PointF? pointEnd = pointsEnd[index];
                var pointId = PointsIds[index];
                var pointEndId = PointsIds[index] + "_e";

                PointF? pointNext = null;
                PointF? pointNextEnd = null;

                if (index != (count - 1))
                {
                    pointNext = points[index + 1];
                    pointNextEnd = pointsEnd[index + 1];
                }
                #endregion

                #region Cross point
                PointF? pointCross = null;

                if (pointNext != null || Intersection(point, pointEnd, pointNext, pointNextEnd))
                    pointCross = GetPointCross((PointF)point, (PointF)pointEnd, (PointF)pointNext, (PointF)pointNextEnd);
                #endregion

                pointsLine.Add((PointF)point);
                pointsLineEnd.Add((PointF)pointEnd);
                pointsLineIds.Add(pointId);
                pointsLineEndIds.Add(pointEndId);

                if (pointCross != null)
                {
                    pointsLine.Add((PointF)pointCross);
                    pointsLineEnd.Add((PointF)pointCross);
                    pointsLineIds.Add(pointId + "_c");
                    pointsLineEndIds.Add(pointId + "_c_e");

                    FillPath(context, GetBrush(areaSeries, point, pointEnd), pointsLine.ToArray(), pointsLineEnd.ToArray(), pointsLineIds.ToArray(), pointsLineEndIds.ToArray());

                    pointsLine.Clear();
                    pointsLineEnd.Clear();
                    pointsLineIds.Clear();
                    pointsLineEndIds.Clear();
                    pointsLine.Add((PointF)pointCross);
                    pointsLineEnd.Add((PointF)pointCross);
                    pointsLineIds.Add(pointId + "_c");
                    pointsLineEndIds.Add(pointId + "_c_e");
                }
                else if (pointNext == null)
                {
                    FillPath(context, GetBrush(areaSeries, point, pointEnd), pointsLine.ToArray(), pointsLineEnd.ToArray(), pointsLineIds.ToArray(), pointsLineEndIds.ToArray());
                }
            }
        }

        private bool IsPointsEqual(PointF[] pointsLine, PointF[] pointsLineEnd)
        {
            if (pointsLine.Length == pointsLineEnd.Length)
            {
                for (var index = 0; index < pointsLine.Length; index++)
                {
                    if (!pointsLine[index].Equals(pointsLineEnd[index]))
                        return false;
                }

                return true;
            }

            return false;
        }

        private StiBrush GetBrush(IStiLineRangeSeries areaSeries, PointF? point, PointF? pointEnd)
        {
            var brush = areaSeries.Brush;
            if (areaSeries.AllowApplyBrushNegative)
            {
                brush = point.Value.Y < pointEnd.Value.Y ? areaSeries.BrushNegative : areaSeries.Brush;
            }
            return brush;
        }

        private void FillPath(StiContext context, StiBrush brush, PointF[] pointsLine, PointF[] pointsLineEnd, string[] pointsIds, string[] pointsEndIds)
        {
            if (IsPointsEqual(pointsLine, pointsLineEnd)) return;

            var path = new List<StiSegmentGeom>();
            var reversePointsLineEnd = pointsLineEnd.Reverse<PointF>().ToArray();

            var lineSegment1 = new StiLineSegmentGeom(reversePointsLineEnd[reversePointsLineEnd.Length - 1], pointsLine[0]);
            var lineSegments = new StiLinesSegmentGeom(pointsLine);
            var lineSegment2 = new StiLineSegmentGeom(pointsLine[pointsLine.Length - 1], reversePointsLineEnd[0]);
            var lineEndSegments = new StiLinesSegmentGeom(reversePointsLineEnd);

            path.Add(lineSegment1);
            path.Add(lineSegments);
            path.Add(lineSegment2);
            path.Add(lineEndSegments);

            var chart = this.Series.Chart as StiChart;


            if (chart.IsAnimation)
            {
                StiAnimation animation = null;
                if (chart.IsAnimationChangingValues)
                {
                    var pointsAnimation = new StiPointsAnimation(pointsLine, pointsLine, pointsIds, StiChartHelper.GlobalDurationElement, StiChartHelper.GlobalBeginTimeElement);
                    pointsAnimation.Id = $"series_{Series.Chart.Series.IndexOf(Series)}_a";
                    var pointsAnimationEnd = new StiPointsAnimation(pointsLineEnd, pointsLineEnd, pointsEndIds, pointsAnimation.Duration, pointsAnimation.BeginTime);
                    pointsAnimationEnd.Id = $"series_{Series.Chart.Series.IndexOf(Series)}_a_e";

                    context.Animations.Add(pointsAnimation);
                    context.Animations.Add(pointsAnimationEnd);

                    if (StiPointsAnimation.IsAnimationChangingValues(Series, pointsIds, pointsAnimation.Id))
                    {
                        pointsAnimation.ApplyPreviousAnimation(Series.Chart.PreviousAnimations);

                        pointsAnimationEnd.ApplyPreviousAnimation(Series.Chart.PreviousAnimations);
                        pointsAnimationEnd.Reverse();

                        lineSegment1.Animation = new StiPointsAnimation(new PointF[] { pointsAnimationEnd.PointsFrom[pointsAnimationEnd.PointsFrom.Length - 1], pointsAnimation.PointsFrom[0] }, null, null, pointsAnimation.Duration, pointsAnimation.BeginTime);
                        lineSegment2.Animation = new StiPointsAnimation(new PointF[] { pointsAnimation.PointsFrom[pointsAnimation.PointsFrom.Length - 1], pointsAnimationEnd.PointsFrom[0] }, null, null, pointsAnimation.Duration, pointsAnimation.BeginTime);
                        lineSegment1.Animation.BeginTimeCorrect = pointsAnimation.BeginTimeCorrect;
                        lineSegment2.Animation.BeginTimeCorrect = pointsAnimation.BeginTimeCorrect;

                        lineSegments.Animation = pointsAnimation;
                        lineEndSegments.Animation = pointsAnimationEnd;
                    }
                    else
                    {
                        animation = new StiOpacityAnimation(StiChartHelper.GlobalDurationElement, StiChartHelper.GlobalBeginTimeElement);
                    }
                }

                context.FillDrawAnimationPath(brush, null, path, StiPathGeom.GetBoundsState, null, animation, null);
            }
            else
            {
                context.FillPath(brush, path, StiPathGeom.GetBoundsState, null);
            }

            #region IsMouseOver
            if (IsMouseOver || Series.Core.IsMouseOver)
            {
                context.FillPath(StiMouseOverHelper.GetMouseOverColor(), path, StiPathGeom.GetBoundsState, null);
            }
            #endregion
        }

        private bool Intersection(PointF? point, PointF? pointEnd, PointF? pointNext, PointF? pointNextEnd)
        {
            if (pointNext == null)
                return false;
            if (point.Value.Y > pointEnd.Value.Y && pointNext.Value.Y < pointNextEnd.Value.Y ||
                point.Value.Y < pointEnd.Value.Y && pointNext.Value.Y > pointNextEnd.Value.Y ||
                pointNext.Value.Y == pointNextEnd.Value.Y)
                return true;
            else
                return false;
        }

        private PointF? GetPointCross(PointF point, PointF pointEnd, PointF pointNext, PointF pointNextEnd)
        {
            if (pointNext == pointNextEnd)
                return pointNext;

            float x1 = point.X;
            float y1 = point.Y;

            float x2 = pointNext.X;
            float y2 = pointNext.Y;

            float x3 = pointEnd.X;
            float y3 = pointEnd.Y;

            float x4 = pointNextEnd.X;
            float y4 = pointNextEnd.Y;

            float x = -((x1 * y2 - x2 * y1) * (x4 - x3) - (x3 * y4 - x4 * y3) * (x2 - x1)) / ((y1 - y2) * (x4 - x3) - (y3 - y4) * (x2 - x1));
            float y = ((y3 - y4) * (-x) - (x3 * y4 - x4 * y3)) / (x4 - x3);

            if (x > x1 && x < x2)
            {
                return new PointF(x, y);
            }
            else
            {
                return null;
            }


        }
        #endregion

        public StiRangeSeriesGeom(StiAreaGeom areaGeom, StiSeriesPointsInfo pointsInfo, IStiSeries series)
            : base(areaGeom, pointsInfo, series)
        {
            PointsEnd = pointsInfo.PointsEnd;
        }
    }
}
