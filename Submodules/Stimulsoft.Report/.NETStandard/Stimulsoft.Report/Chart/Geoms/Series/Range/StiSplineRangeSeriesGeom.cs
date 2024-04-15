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

using System.Drawing;
using System.Collections.Generic;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using System.Linq;

namespace Stimulsoft.Report.Chart
{
    public class StiSplineRangeSeriesGeom : StiSplineSeriesGeom
    {
        #region Properties
        public PointF?[] PointsEnd { get; }

        #endregion

        #region Methods
        public override void Draw(StiContext context)
        {
            var list = StiNullableDrawing.GetPointsList(this.Points);
            var listEnd = StiNullableDrawing.GetPointsList(this.PointsEnd);

            if (list.Count < 1 || listEnd.Count < 1) return;

            var points = list[0];
            var pointsEnd = listEnd[0];
            var pointsEndIds = PointsIds.Select(id => id + "_e").ToArray();

            FillPath(context, points, pointsEnd, PointsIds, pointsEndIds);
        }

        private void FillPath(StiContext context, PointF[] points, PointF[] pointsEnd, string[] pointsIds, string[] pointsEndIds)
        {
            var areaSeries = this.Series as IStiSplineRangeSeries;
            var brush = areaSeries.Brush;

            var path = new List<StiSegmentGeom>();

            var reversePointsEnd = pointsEnd.Reverse<PointF>().ToArray();

            var lineSegment1 = new StiLineSegmentGeom(reversePointsEnd[reversePointsEnd.Length - 1], points[0]);
            var lineSegments = new StiCurveSegmentGeom(points, areaSeries.Tension);
            var lineSegment2 = new StiLineSegmentGeom(points[points.Length - 1], reversePointsEnd[0]);
            var lineEndSegments = new StiCurveSegmentGeom(reversePointsEnd, areaSeries.Tension);

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
                    var pointsAnimation = new StiPointsAnimation(points, points, pointsIds, StiChartHelper.GlobalDurationElement, StiChartHelper.GlobalBeginTimeElement);
                    pointsAnimation.Id = $"series_{Series.Chart.Series.IndexOf(Series)}_a";
                    var pointsAnimationEnd = new StiPointsAnimation(pointsEnd, pointsEnd, pointsEndIds, pointsAnimation.Duration, pointsAnimation.BeginTime);
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
        #endregion

        public StiSplineRangeSeriesGeom(StiAreaGeom areaGeom, StiSeriesPointsInfo pointsInfo, IStiSeries series)
            : base(areaGeom, pointsInfo, series)
        {
            PointsEnd = pointsInfo.PointsEnd;
        }
    }
}
