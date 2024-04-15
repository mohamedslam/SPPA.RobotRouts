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
    public class StiRadarAreaSeriesGeom : StiCellGeom
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
        public IStiSeries Series { get; }

        public PointF?[] PointsFrom { get; }

        public PointF?[] Points { get; }

        public string[] PointsIds { get; }
        #endregion

        #region Methods
        public override bool Contains(float x, float y)
        {
            if (Invisible) return false;

            var radarArea = this.Series.Chart.Area as IStiRadarArea;
            var radarCore = radarArea.Core as StiRadarAreaCoreXF;

            for (int pointIndex = 0; pointIndex < this.Points.Length; pointIndex++)
            {
                var point1 = this.Points[pointIndex];
                var point2 = pointIndex == this.Points.Length - 1 ? this.Points[0] : this.Points[pointIndex + 1];
                var point3 = radarCore.CenterPoint;

                if (point1 == null || point2 == null)
                    continue;

                bool result = StiPointHelper.IsPointInTriangle(new PointF(x, y), point1.Value, point3, point2.Value);
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
            var radarSeries = this.Series as IStiRadarAreaSeries;

            if (radarSeries.Brush == null) return;

            var chart = this.Series.Chart as StiChart;
            var path = new List<StiSegmentGeom>();

            if (chart.IsAnimation)
            {
                StiAnimation animation = null;

                StiPointsAnimation pointsAnimation = null;
                if (StiPointsAnimation.IsAnimationChangingValues(Series, PointsIds))
                {
                    pointsAnimation = new StiPointsAnimation(PointsFrom.Cast<PointF>().ToArray(), Points.Cast<PointF>().ToArray(), PointsIds, StiChartHelper.GlobalDurationElement, StiChartHelper.GlobalBeginTimeElement);
                    pointsAnimation.Id = $"series_{Series.Chart.Series.IndexOf(Series)}";
                    pointsAnimation.ApplyPreviousAnimation(chart.PreviousAnimations);

                    context.Animations.Add(pointsAnimation);
                }
                else
                {
                    animation = new StiOpacityAnimation(StiChartHelper.GlobalDurationElement, StiChartHelper.GlobalBeginTimeElement);
                }

                path.Add(new StiLinesSegmentGeom(Points.Cast<PointF>().ToArray(), pointsAnimation));

                context.FillDrawAnimationPath(radarSeries.Brush, null, path, StiPathGeom.GetBoundsState, null, animation, null);
            }
            else
            {
                int index = 0;
                foreach (PointF? point in this.Points)
                {
                    var curPoint = point;
                    var nextPoint = index < Points.Length - 1 ? Points[index + 1] : Points[0];

                    path.Add(new StiLineSegmentGeom(curPoint.Value, nextPoint.Value));

                    index++;
                }

                context.PushSmoothingModeToAntiAlias();
                context.FillPath(radarSeries.Brush, path, RectangleF.Empty, null);

                #region IsMouseOver
                if (IsMouseOver || Series.Core.IsMouseOver)
                {
                    context.FillPath(StiMouseOverHelper.GetMouseOverColor(), path, RectangleF.Empty, null);
                }
                #endregion

                context.PopSmoothingMode();
            }

        }
        #endregion

        public StiRadarAreaSeriesGeom(IStiSeries series, StiSeriesPointsInfo pointsInfo)
            : base(RectangleF.Empty)
        {
            this.Series = series;
            this.PointsFrom = pointsInfo.PointsFrom;
            this.Points = pointsInfo.Points;
            this.PointsIds = pointsInfo.PointsIds;
        }
    }
}
