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
    public class StiSteppedAreaSeriesGeom : StiSteppedLineSeriesGeom
    {
        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            var areaSeries = this.Series as IStiSteppedAreaSeries;

            var chart = this.Series.Chart as StiChart;

            var axisArea = this.Series.Chart.Area as IStiAxisArea;

            float posY = axisArea.AxisCore.GetDividerY();

            var list = StiNullableDrawing.GetNullablePointsList(this.Points);

            var listFrom = this.PointsFrom != null ? StiNullableDrawing.GetNullablePointsList(this.PointsFrom) : null;

            if (chart.IsAnimation)
            {
                for (var index = 0; index < list.Count; index++)
                {
                    var newPoints = GetSteppedPoints(list[index]).Cast<PointF>().ToArray();

                    var path = new List<StiSegmentGeom>();

                    var lineSegment1 = new StiLineSegmentGeom(new PointF(newPoints[0].X, posY), newPoints[0]);
                    var lineSegments = new StiLinesSegmentGeom(newPoints);
                    var lineSegment2 = new StiLineSegmentGeom(newPoints[newPoints.Length - 1], new PointF(newPoints[newPoints.Length - 1].X, posY));

                    path.Add(lineSegment1);
                    path.Add(lineSegments);
                    path.Add(lineSegment2);

                    var newPointsFrom = GetSteppedPoints(listFrom[index]).Cast<PointF>().ToArray();
                    var newPointsIds = GetSteppedPointsIds(PointsIds);

                    if (StiPointsAnimation.IsAnimationChangingValues(Series, newPointsIds))
                    {
                        var pointsAnimation = new StiPointsAnimation(newPointsFrom, newPoints, newPointsIds, StiChartHelper.GlobalDurationElement, StiChartHelper.GlobalBeginTimeElement);
                        pointsAnimation.Id = $"series_{Series.Chart.Series.IndexOf(Series)}";
                        pointsAnimation.ApplyPreviousAnimation(this.Series.Chart.PreviousAnimations);

                        lineSegment1.Animation = new StiPointsAnimation(new PointF[] { new PointF(pointsAnimation.PointsFrom.FirstOrDefault().X, posY), pointsAnimation.PointsFrom.FirstOrDefault() }, pointsAnimation.Duration, pointsAnimation.BeginTime) { BeginTimeCorrect = pointsAnimation.BeginTimeCorrect };
                        lineSegments.Animation = pointsAnimation;
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
                    else if (areaSeries.Brush != null)
                    {
                        var animation = new StiOpacityAnimation(StiChartHelper.GlobalDurationElement, StiChartHelper.GlobalBeginTimeElement);
                        context.FillDrawAnimationPath(areaSeries.Brush, null, path, StiPathGeom.GetBoundsState, null, animation, null);
                    }

                    index++;
                }
            }
            else
            {
                foreach (PointF?[] newPoints in list)
                {
                    var newPoints2 = GetSteppedPoints(newPoints).Cast<PointF>().ToArray();

                    var path = new List<StiSegmentGeom>();

                    path.Add(new StiLineSegmentGeom(new PointF(newPoints2[0].X, posY), newPoints2[0]));
                    path.Add(new StiLinesSegmentGeom(newPoints2));
                    path.Add(new StiLineSegmentGeom(newPoints2[newPoints2.Length - 1], new PointF(newPoints2[newPoints2.Length - 1].X, posY)));

                    if (areaSeries.Brush != null)
                    {
                        context.FillPath(areaSeries.Brush, path, StiPathGeom.GetBoundsState, null);
                    }

                    if (areaSeries.AllowApplyBrushNegative && areaSeries.BrushNegative != null)
                    {
                        float width = (float)(axisArea.AxisCore.ScrollRangeX * axisArea.AxisCore.ScrollDpiX);
                        float height = (float)(axisArea.AxisCore.ScrollRangeY * axisArea.AxisCore.ScrollDpiY - posY);

                        RectangleF clipRect = new RectangleF(0, posY, width, height);
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

        public StiSteppedAreaSeriesGeom(StiAreaGeom areaGeom, StiSeriesPointsInfo pointsInfo, IStiSeries series)
            : base(areaGeom, pointsInfo, series)
        {
        }
    }
}
