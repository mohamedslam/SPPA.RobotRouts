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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;

namespace Stimulsoft.Report.Chart
{
    public class StiBaseLineSeriesGeom : StiSeriesGeom
    {
        #region Fields
        private string additionalSeriesId;
        #endregion

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

        public PointF?[] PointsFrom { get; }

        public PointF?[] Points { get; }

        public string[] PointsIds { get; set; }
        #endregion

        #region Methods
        internal static RectangleF GetClientRectangle(PointF?[] points, float lineWidth)
        {
            if (points == null || points.Length == 0)
                return RectangleF.Empty;

            PointF minPoint = PointF.Empty;
            PointF maxPoint = PointF.Empty;
            foreach (PointF? point in points)
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
            //add line width to client rect
            return new RectangleF(minPoint.X - lineWidth / 2, minPoint.Y - lineWidth / 2, maxPoint.X - minPoint.X + lineWidth, maxPoint.Y - minPoint.Y + lineWidth);
        }

        protected StiAnimation GetAnimation()
        {
            return GetAnimation(Points);
        }

        protected StiAnimation GetAnimation(PointF?[] points)
        {
            if (!((StiChart)Series.Chart).IsAnimation) return null;

            if (PointsFrom == null || PointsIds == null) return null;
            var pointsFrom = new List<PointF?>();
            var points2 = new List<PointF?>();
            var pointsIds = new List<string>();

            for (int index = 0; index < PointsFrom.Length; index++)
            {
                if (index >= Points.Length || index >= PointsIds.Length)
                    break;

                if (PointsFrom[index] != null && Points[index] != null && PointsIds[index] != null)
                {
                    pointsFrom.Add((PointF)PointsFrom[index]);
                    points2.Add((PointF)Points[index]);
                    pointsIds.Add(PointsIds[index]);
                }
            }

            return GetAnimation(pointsFrom.ToArray(), points2.ToArray(), pointsIds.ToArray());
        }

        protected StiAnimation GetAnimationConnect(PointF?[] points)
        {
            if (!((StiChart)Series.Chart).IsAnimation) return null;

            if (PointsFrom == null || PointsIds == null) return null;
            var pointsFrom = new List<PointF?>();
            var points2 = new List<PointF?>();
            var pointsIds = new List<string>();

            var index = 0;
            foreach (var point in points)
            {
                if (point != null && (!points2.Exists(p => p.Equals(point.Value))))
                {
                    var pointFrom = PointsFrom[index];
                    if (pointFrom == null) pointFrom = point;

                    var pointId = PointsIds[index];
                    if (string.IsNullOrEmpty(pointId)) pointId = index.ToString() + "_";

                    pointsFrom.Add(pointFrom);
                    points2.Add((PointF)point);
                    pointsIds.Add(pointId);

                    index++;
                }
            }

            return GetAnimation(pointsFrom.ToArray(), points2.ToArray(), pointsIds.ToArray());
        }

        protected StiAnimation GetAnimation(PointF?[] pointsFrom, PointF?[] points, string[] pointsIds)
        {
            if (!((StiChart)Series.Chart).IsAnimation) return null;

            if (pointsFrom.Length == 0)
                return null;

            StiAnimation animation = null;
            StiPointsAnimation pointsAnimation = null;
            if (((StiChart)Series.Chart).IsAnimationChangingValues && (pointsFrom?.First() != null && pointsFrom?.Last() != null) && pointsIds != null)
            {
                pointsAnimation = new StiPointsAnimation(
                    pointsFrom.Cast<PointF>().ToArray(),
                    points.Cast<PointF>().ToArray(),
                    pointsIds,
                    StiChartHelper.GlobalDurationElement, TimeSpan.Zero);

                pointsAnimation.Id = $"series_{Series.Chart.Series.IndexOf(Series)}" + additionalSeriesId;

                if (StiPointsAnimation.IsAnimationChangingValues(Series, pointsIds, pointsAnimation.Id))
                {
                    pointsAnimation.ApplyPreviousAnimation(Series.Chart.PreviousAnimations);
                    animation = pointsAnimation;
                }
            }

            if (animation == null)
            {
                var translationAnimation = new StiTranslationAnimation(StiChartHelper.GlobalDurationElement, TimeSpan.Zero);
                translationAnimation.AnotherAnimation = pointsAnimation;
                translationAnimation.Id = $"series_{Series.Chart.Series.IndexOf(Series)}" + additionalSeriesId;

                animation = translationAnimation;
            }

            return animation;
        }
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
        }
        #endregion

        public StiBaseLineSeriesGeom(StiAreaGeom areaGeom, StiSeriesPointsInfo pointsInfo, IStiSeries series)
            : this(areaGeom, pointsInfo.PointsFrom, pointsInfo.Points, series)
        {
            PointsIds = pointsInfo.PointsIds;
            additionalSeriesId = pointsInfo.AdditionalSeriesId;
        }

        public StiBaseLineSeriesGeom(StiAreaGeom areaGeom, PointF?[] pointsFrom, PointF?[] points, IStiSeries series)
        : base(areaGeom, series, GetClientRectangle(points, series is StiBaseLineSeries ? ((StiBaseLineSeries)series).LineWidth : 0f))
        {
            this.Points = points;
            if (pointsFrom == null || pointsFrom.FirstOrDefault() == null) pointsFrom = (PointF?[])points.Clone();
            this.PointsFrom = pointsFrom;
        }
    }
}
