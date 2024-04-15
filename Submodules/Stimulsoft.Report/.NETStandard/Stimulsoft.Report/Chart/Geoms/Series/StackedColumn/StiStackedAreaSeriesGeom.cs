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
using System.Collections.Generic;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Drawing;
using System.Linq;

namespace Stimulsoft.Report.Chart
{
    public class StiStackedAreaSeriesGeom : StiSeriesGeom
    {
        #region IStiGeomInteraction override
        public override void InvokeMouseEnter(StiInteractionOptions options)
        {
            if (!AllowMouseOver) return;

            if (!IsMouseOver)
            {
                IsMouseOver = true;
                options.UpdateContext = true;
            }
        }

        public override void InvokeMouseLeave(StiInteractionOptions options)
        {
            if (!AllowMouseOver) return;

            if (IsMouseOver)
            {
                IsMouseOver = false;
                options.UpdateContext = true;
            }
        }

        public virtual bool AllowMouseOver
        {
            get
            {
                return
                    (this.Series.Interaction.DrillDownEnabled && this.Series.Interaction.AllowSeries);
            }
        }

        public virtual bool IsMouseOver
        {
            get
            {
                if (this.Series == null)
                    return false;

                return this.Series.Core.IsMouseOver;
            }
            set
            {
                if (this.Series != null)
                    this.Series.Core.IsMouseOver = value;
            }
        }
        #endregion

        #region Properties
        public PointF?[] StartPoints { get; }

        public PointF?[] EndPoints { get; }

        public string[] PointsIds { get; }
        #endregion

        #region Methods
        public override bool Contains(float x, float y)
        {
            if (Invisible) return false;

            for (int pointIndex = 0; pointIndex < (this.StartPoints.Length - 1); pointIndex++)
            {
                var point1 = this.StartPoints[pointIndex];
                var point4 = this.StartPoints[pointIndex + 1];
                var point2 = this.EndPoints[pointIndex];
                var point3 = this.EndPoints[pointIndex + 1];

                if (point1 == null || point2 == null || point3 == null || point4 == null)
                    continue;

                bool result = StiPointHelper.IsPointInPolygon(new PointF(x, y), new PointF[] { point1.Value, point4.Value, point3.Value, point2.Value });
                if (result)
                    return true;
            }
            return false;
        }

        internal static RectangleF GetClientRectangle(PointF?[] startPoints, PointF?[] endPoints)
        {
            if (startPoints == null || startPoints.Length == 0 || endPoints == null || endPoints.Length == 0)
                return RectangleF.Empty;

            var minPoint = PointF.Empty;
            var maxPoint = PointF.Empty;
            foreach (PointF? point in startPoints)
            {
                if (point == null) continue;

                if (minPoint == PointF.Empty)
                {
                    minPoint = point.Value;
                    maxPoint = point.Value;
                }
                else
                {
                    minPoint.X = Math.Min(minPoint.X, point.Value.X);
                    minPoint.Y = Math.Min(minPoint.Y, point.Value.Y);

                    maxPoint.X = Math.Max(maxPoint.X, point.Value.X);
                    maxPoint.Y = Math.Max(maxPoint.Y, point.Value.Y);
                }
            }

            foreach (PointF? point in endPoints)
            {
                if (point == null) continue;

                if (minPoint == PointF.Empty)
                {
                    minPoint = point.Value;
                    maxPoint = point.Value;
                }
                else
                {
                    minPoint.X = Math.Min(minPoint.X, point.Value.X);
                    minPoint.Y = Math.Min(minPoint.Y, point.Value.Y);

                    maxPoint.X = Math.Max(maxPoint.X, point.Value.X);
                    maxPoint.Y = Math.Max(maxPoint.Y, point.Value.Y);
                }
            }

            return new RectangleF(minPoint.X, minPoint.Y, maxPoint.X - minPoint.X, maxPoint.Y - minPoint.Y);
        }

        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            var areaSeries = this.Series as IStiStackedAreaSeries;
            var isAnimation = ((StiChart)areaSeries.Chart).IsAnimation;

            List<PointF[]> startList;
            List<PointF[]> endList;
            StiNullableDrawing.GetPointsList(this.StartPoints, this.EndPoints, out startList, out endList);

            int listIndex = 0;
            foreach (PointF[] newStartPoints in startList)
            {
                var newEndPoints = endList[listIndex];

                // for correct fill in Wpf swap points
                var reverseStartPoint = newStartPoints.Reverse<PointF>().ToArray();

                var path = new List<StiSegmentGeom>();

                var lineSegment1 = new StiLineSegmentGeom(newStartPoints[0], newEndPoints[0]);
                var lineSegments = new StiLinesSegmentGeom(newEndPoints);
                var lineSegment2 = new StiLineSegmentGeom(newEndPoints[newEndPoints.Length - 1], newStartPoints[newStartPoints.Length - 1]);
                var reverseLineSegment = new StiLinesSegmentGeom(reverseStartPoint);

                path.Add(lineSegment1);
                path.Add(lineSegments);
                path.Add(lineSegment2);
                path.Add(reverseLineSegment);

                StiAnimation animation = null;
                if (isAnimation)
                {
                    if (((StiChart)areaSeries.Chart).IsAnimationChangingValues)
                    {
                        var pointsAnimation = new StiPointsAnimation(newEndPoints, newEndPoints, PointsIds, StiChartHelper.GlobalDurationElement, StiChartHelper.GlobalBeginTimeElement);
                        pointsAnimation.Id = $"series_{Series.Chart.Series.IndexOf(Series)}_a";
                        var pointsAnimationReverse = new StiPointsAnimation(newStartPoints, newStartPoints, PointsIds, pointsAnimation.Duration, pointsAnimation.BeginTime);
                        pointsAnimationReverse.Id = $"series_{Series.Chart.Series.IndexOf(Series)}_a_r";

                        context.Animations.Add(pointsAnimation);
                        context.Animations.Add(pointsAnimationReverse);

                        if (StiPointsAnimation.IsAnimationChangingValues(Series, PointsIds, pointsAnimation.Id))
                        {
                            pointsAnimation.ApplyPreviousAnimation(Series.Chart.PreviousAnimations);

                            pointsAnimationReverse.ApplyPreviousAnimation(Series.Chart.PreviousAnimations);
                            pointsAnimationReverse.Reverse();

                            lineSegment1.Animation = new StiPointsAnimation(new PointF[] { pointsAnimationReverse.PointsFrom.LastOrDefault(), pointsAnimation.PointsFrom.FirstOrDefault() }, pointsAnimation.Duration, pointsAnimation.BeginTime) { BeginTimeCorrect = pointsAnimation.BeginTimeCorrect };
                            lineSegments.Animation = pointsAnimation;
                            lineSegment2.Animation = new StiPointsAnimation(new PointF[] { pointsAnimation.PointsFrom.LastOrDefault(), pointsAnimationReverse.PointsFrom.FirstOrDefault() }, pointsAnimation.Duration, pointsAnimation.BeginTime) { BeginTimeCorrect = pointsAnimation.BeginTimeCorrect };
                            reverseLineSegment.Animation = pointsAnimationReverse;
                        }
                        else
                        {
                            animation = new StiOpacityAnimation(StiChartHelper.GlobalDurationElement, StiChartHelper.GlobalBeginTimeElement);
                        }
                    }
                }

                if (areaSeries.Brush != null)
                {
                    if (isAnimation)
                    {
                        context.FillDrawAnimationPath(areaSeries.Brush, null, path, StiPathGeom.GetBoundsState, null, animation, null);
                    }
                    else
                    {
                        context.FillPath(areaSeries.Brush, path, StiPathGeom.GetBoundsState, null);
                    }
                }

                if (areaSeries.AllowApplyBrushNegative && areaSeries.BrushNegative != null)
                {
                    var axisArea = this.Series.Chart.Area as IStiAxisArea;

                    var posY = axisArea.AxisCore.GetDividerY();

                    var width = (float)(axisArea.AxisCore.ScrollRangeX * axisArea.AxisCore.ScrollDpiX);
                    var height = (float)(axisArea.AxisCore.ScrollRangeY * axisArea.AxisCore.ScrollDpiY - posY);

                    var clipRect = new RectangleF(0, posY, width, height);
                    context.PushClip(clipRect);

                    if (isAnimation)
                    {
                        context.FillDrawAnimationPath(areaSeries.BrushNegative, null, path, StiPathGeom.GetBoundsState, null, animation, null);
                    }
                    else
                    {
                        context.FillPath(areaSeries.BrushNegative, path, StiPathGeom.GetBoundsState, null);
                    }

                    context.PopClip();
                }

                #region IsMouseOver
                if (IsMouseOver || Series.Core.IsMouseOver)
                {
                    context.FillPath(StiMouseOverHelper.GetMouseOverColor(), path, StiPathGeom.GetBoundsState, null);
                }
                #endregion

                listIndex++;
            }
        }
        #endregion

        public StiStackedAreaSeriesGeom(StiAreaGeom areaGeom, StiSeriesPointsInfo pointsInfo, IStiSeries series)
            : base(areaGeom, series, GetClientRectangle(pointsInfo.PointsStart, pointsInfo.PointsEnd))
        {
            StartPoints = pointsInfo.PointsStart;
            EndPoints = pointsInfo.PointsEnd;
            PointsIds = pointsInfo.PointsIds;
        }
    }
}
