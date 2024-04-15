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

using System.Drawing;
using System.Collections.Generic;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using System.Linq;

namespace Stimulsoft.Report.Chart
{
    public class StiSplineAreaSeriesGeom : StiSplineSeriesGeom
    {
        #region Methods
        public override bool Contains(float x, float y)
        {
            IStiAxisArea axisArea = this.Series.Chart.Area as IStiAxisArea;
            float posY = axisArea.AxisCore.GetDividerY();

            if (Invisible) return false;

            for (int pointIndex = 0; pointIndex < (this.Points.Length - 1); pointIndex++)
            {
                var point1 = this.Points[pointIndex];
                var point2 = this.Points[pointIndex + 1];

                if (point1 == null || point2 == null) continue;

                var point3 = new PointF(point2.Value.X, posY);
                var point4 = new PointF(point1.Value.X, posY);

                bool result = StiPointHelper.IsPointInPolygon(new PointF(x, y), new PointF[] { point1.Value, point4, point3, point2.Value });
                if (result)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            var areaSeries = this.Series as IStiSplineAreaSeries;

            var chart = this.Series.Chart as StiChart;

            var axisArea = this.Series.Chart.Area as IStiAxisArea;

            float posY = axisArea.AxisCore.GetDividerY();

            var list = StiNullableDrawing.GetPointsList(this.Points);
            var listFrom = this.PointsFrom != null ? StiNullableDrawing.GetPointsList(this.PointsFrom) : null;

            if (chart.IsAnimation)
            {
                for (var index = 0; index < list.Count; index++)
                {
                    var newPoints = list[index];

                    var path = new List<StiSegmentGeom>();

                    var lineSegment1 = new StiLineSegmentGeom(new PointF(newPoints[0].X, posY), newPoints[0]);
                    var curveSegments = new StiCurveSegmentGeom(newPoints, areaSeries.Tension);
                    var lineSegment2 = new StiLineSegmentGeom(newPoints[newPoints.Length - 1], new PointF(newPoints[newPoints.Length - 1].X, posY));

                    path.Add(lineSegment1);
                    path.Add(curveSegments);
                    path.Add(lineSegment2);

                    if (StiPointsAnimation.IsAnimationChangingValues(Series, PointsIds))
                    {
                        var newPointsFrom = listFrom[index];

                        var pointsAnimation = new StiPointsAnimation(newPointsFrom, newPoints, this.PointsIds, StiChartHelper.GlobalDurationElement, StiChartHelper.GlobalBeginTimeElement);
                        pointsAnimation.Id = $"series_{Series.Chart.Series.IndexOf(Series)}";
                        pointsAnimation.ApplyPreviousAnimation(this.Series.Chart.PreviousAnimations);

                        lineSegment1.Animation = new StiPointsAnimation(new PointF[] { new PointF(pointsAnimation.PointsFrom.FirstOrDefault().X, posY), pointsAnimation.PointsFrom.FirstOrDefault() }, pointsAnimation.Duration, pointsAnimation.BeginTime) { BeginTimeCorrect = pointsAnimation.BeginTimeCorrect };
                        curveSegments.Animation = pointsAnimation;
                        lineSegment2.Animation = new StiPointsAnimation(new PointF[] { pointsAnimation.PointsFrom.LastOrDefault(), new PointF(pointsAnimation.PointsFrom.LastOrDefault().X, posY) }, pointsAnimation.Duration, pointsAnimation.BeginTime) { BeginTimeCorrect = pointsAnimation.BeginTimeCorrect };
                        context.Animations.Add(pointsAnimation);

                        context.FillDrawAnimationPath(areaSeries.Brush, null, path, StiPathGeom.GetBoundsState, null, null, null);

                        if (areaSeries.AllowApplyBrushNegative && areaSeries.BrushNegative != null)
                        {
                            float width = (float)(axisArea.AxisCore.ScrollRangeX * axisArea.AxisCore.ScrollDpiX);
                            float height = (float)(axisArea.AxisCore.ScrollRangeY * axisArea.AxisCore.ScrollDpiY - posY);

                            var clipRect = new RectangleF(0, posY, width, height);
                            context.PushClip(clipRect);

                            context.FillDrawAnimationPath(areaSeries.BrushNegative, null, path, StiPathGeom.GetBoundsState, null, null, null);

                            context.PopClip();
                        }
                    }
                    else
                    {
                        var animation = new StiOpacityAnimation(StiChartHelper.GlobalDurationElement, StiChartHelper.GlobalBeginTimeElement);
                        context.FillDrawAnimationPath(areaSeries.Brush, null, path, StiPathGeom.GetBoundsState, null, animation, null);
                    }

                    index++;
                }
            }
            else
            {
                foreach (PointF[] newPoints in list)
                {
                    var path = new List<StiSegmentGeom>();

                    path.Add(new StiLineSegmentGeom(new PointF(newPoints[0].X, posY), newPoints[0]));
                    path.Add(new StiCurveSegmentGeom(newPoints, areaSeries.Tension));
                    path.Add(new StiLineSegmentGeom(newPoints[newPoints.Length - 1], new PointF(newPoints[newPoints.Length - 1].X, posY)));

                    if (areaSeries.Brush != null)
                    {
                        context.FillPath(areaSeries.Brush, path, StiPathGeom.GetBoundsState, null);
                    }

                    if (areaSeries.AllowApplyBrushNegative && areaSeries.BrushNegative != null)
                    {
                        float width = (float)(axisArea.AxisCore.ScrollRangeX * axisArea.AxisCore.ScrollDpiX);
                        float height = (float)(axisArea.AxisCore.ScrollRangeY * axisArea.AxisCore.ScrollDpiY - posY);

                        var clipRect = new RectangleF(0, posY, width, height);
                        context.PushClip(clipRect);

                        context.FillPath(areaSeries.BrushNegative, path, StiPathGeom.GetBoundsState, null);

                        context.PopClip();
                    }

                    #region IsMouseOver
                    if (IsMouseOver || Series.Core.IsMouseOver)
                    {
                        context.FillPath(StiMouseOverHelper.GetMouseOverColor(), path, StiPathGeom.GetBoundsState, null);
                    }
                    #endregion
                }
            }
        }
        #endregion

        public StiSplineAreaSeriesGeom(StiAreaGeom areaGeom, StiSeriesPointsInfo pointsInfo, IStiSeries series)
            : base(areaGeom, pointsInfo, series)
        {
        }
    }
}
