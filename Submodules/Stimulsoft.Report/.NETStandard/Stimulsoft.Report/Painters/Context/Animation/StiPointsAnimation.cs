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

using Stimulsoft.Report.Chart;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Stimulsoft.Base.Context.Animation
{
    public class StiPointsAnimation : StiAnimation
    {
        #region Field
        private bool isReverse = false;
        #endregion

        #region Properies
        public PointF[] PointsFrom { get; set; }

        public PointF[] PointsTo { get; set; }

        public string[] PointsIds { get; set; }
        #endregion

        #region Methods
        public static bool IsAnimationChangingValues(IStiSeries series, string[] pointsIds)
        {
            var id = $"series_{series.Chart.Series.IndexOf(series)}";
            return IsAnimationChangingValues(series, pointsIds, id);
        }

        public static bool IsAnimationChangingValues(IStiSeries series, string[] pointsIds, object id)
        {
            return series.Chart.IsAnimationChangingValues && GetPreviousAnimation(series.Chart.PreviousAnimations, pointsIds, id) != null;
        }

        private static StiPointsAnimation GetPreviousAnimation(List<StiAnimation> previousAnimations, string[] pointsIds, object id)
        {
            var prevAnimation = id != null ? previousAnimations
                .Where(a => a is StiPointsAnimation)
                .Cast<StiPointsAnimation>()
                .FirstOrDefault(a => id.Equals(a.Id)) : null;

            if (prevAnimation == null)
                prevAnimation = id != null ? previousAnimations
                .Where(a => a is StiPointsAnimation)
                .Cast<StiPointsAnimation>()
                .FirstOrDefault(a => id.Equals(a.Id + "_a")) : null;

            if (prevAnimation == null)
                prevAnimation = id != null ? previousAnimations
                .Where(a => a is StiPointsAnimation)
                .Cast<StiPointsAnimation>()
                .FirstOrDefault(a => (id + "_a").Equals(a.Id)) : null;

            if (prevAnimation == null && id != null)
            {
                prevAnimation = previousAnimations
                .Where(a => a != null)
                .Where(a => a.AnotherAnimation != null)
                .FirstOrDefault(a => id.Equals(a.Id))?.AnotherAnimation as StiPointsAnimation;
            }

            if (prevAnimation != null && pointsIds != null && !pointsIds.Any(p => GetPointIndex(prevAnimation.PointsIds.ToList(), p) >= 0)) return null;

            return prevAnimation;
        }

        private static int GetPointIndex(List<string> pointsIds, string pointId)
        {
            pointsIds = pointsIds.Where(id => id != null).ToList();
            var pointIndex = pointsIds.FindIndex(id => id.Equals(pointId));
            if (pointIndex == -1) pointIndex = pointsIds.FindIndex(id => (id + "_l").Equals(pointId));
            if (pointIndex == -1) pointIndex = pointsIds.FindIndex(id => (id + "_h").Equals(pointId));
            if (pointIndex == -1) pointIndex = pointsIds.FindIndex(id => id.Equals(pointId + "_h"));
            if (pointIndex == -1) pointIndex = pointsIds.FindIndex(id => id.Equals(pointId + "_l"));

            return pointIndex;
        }

        public void ApplyPreviousAnimation(List<StiAnimation> previousAnimations)
        {
            var prevAnimation = GetPreviousAnimation(previousAnimations, PointsIds, Id);

            if (prevAnimation != null)
            {
                if (prevAnimation.isReverse) prevAnimation.Reverse();
                for (var indexPoint = 0; indexPoint < this.PointsFrom.Length; indexPoint++)
                {
                    var pointId = this.PointsIds[indexPoint];
                    var prevPointIndex = GetPointIndex(prevAnimation.PointsIds.ToList(), pointId);
                    
                    if (prevPointIndex >= 0)
                    {
                        var prevPoint = prevAnimation.PointsTo[prevPointIndex];
                        this.PointsFrom[indexPoint] = prevPoint;
                    }
                    else if (indexPoint == 0 && this.PointsFrom.Length > 0)
                    {
                        this.PointsFrom[indexPoint] = prevAnimation.PointsTo.FirstOrDefault();
                    }
                    else if (indexPoint > 0 && pointId == this.PointsIds[0] + "_e")
                    {
                        this.PointsFrom[indexPoint] = this.PointsFrom[0];
                    }
                    else if (indexPoint > 0)
                    {
                        this.PointsFrom[indexPoint] = this.PointsFrom[indexPoint - 1];
                    }
                }

                if (prevAnimation.isReverse) prevAnimation.Reverse();
                this.Duration = TimeSpan.FromMilliseconds(300);
            }
            else if (previousAnimations.Count > 0)
            {
                this.BeginTimeCorrect = TimeSpan.FromMilliseconds(200);
            }
        }

        public void Reverse()
        {
            isReverse = true;
            PointsTo = PointsTo.Reverse<PointF>().ToArray();
            PointsFrom = PointsFrom.Reverse<PointF>().ToArray();
            PointsIds = PointsIds.Reverse<string>().ToArray();
        }
        #endregion

        #region Properies.Override
        public override StiAnimationType Type => StiAnimationType.Points;
        #endregion

        public StiPointsAnimation(PointF[] pointsFrom, PointF[] pointsTo, string[] pointsIds, TimeSpan duration, TimeSpan beginTime) : this(pointsFrom, duration, beginTime)
        {
            PointsTo = pointsTo?.Clone() as PointF[];
            PointsIds = pointsIds?.Clone() as string[];
        }

        public StiPointsAnimation(PointF[] pointsFrom, TimeSpan duration, TimeSpan beginTime)
            : base(duration, beginTime)
        {
            PointsFrom = pointsFrom.Clone() as PointF[];
        }
    }
}