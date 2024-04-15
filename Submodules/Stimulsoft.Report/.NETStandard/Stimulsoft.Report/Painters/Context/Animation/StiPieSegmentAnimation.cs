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
    public class StiPieSegmentAnimation : StiAnimation
    {
        #region Properies
        public RectangleF RectFrom { get; set; }

        public RectangleF RectTo { get; set; }

        public RectangleF RectDtFrom { get; set; }

        public RectangleF RectDtTo { get; set; }

        public float StartAngleFrom { get; set; }

        public float EndAngleFrom { get; set; }

        public float StartAngleTo { get; set; }

        public float EndAngleTo { get; set; }
        #endregion

        #region Properies.Override
        public override StiAnimationType Type => StiAnimationType.PieSegment;
        #endregion

        #region Methods
        public static bool IsAnimationChangingValues(IStiSeries series)
        {
            var id = $"series_{series.Chart.Series.IndexOf(series)}";
            return IsAnimationChangingValues(series, id);
        }

        public static bool IsAnimationChangingValues(IStiSeries series, object id)
        {
            return series.Chart.IsAnimationChangingValues && GetPreviousAnimation(series.Chart.PreviousAnimations, id) != null;
        }

        private static StiPieSegmentAnimation GetPreviousAnimation(List<StiAnimation> previousAnimations, object id)
        {
            var prevAnimation = id != null ? previousAnimations
                .Where(a => a is StiPieSegmentAnimation)
                .Cast<StiPieSegmentAnimation>()
                .FirstOrDefault(a => id.Equals(a.Id)) : null;

            if (prevAnimation == null && id != null)
            {
                prevAnimation = previousAnimations
                .Where(a => a != null)
                .Where(a => a.AnotherAnimation != null)
                .FirstOrDefault(a => id.Equals(a.Id))?.AnotherAnimation as StiPieSegmentAnimation;
            }
            return prevAnimation;
        }

        public void ApplyPreviousAnimation(List<StiAnimation> previousAnimations)
        {
            var prevAnimation = GetPreviousAnimation(previousAnimations, Id);

            if (prevAnimation != null)
            {
                this.StartAngleFrom = prevAnimation.StartAngleTo;
                this.EndAngleFrom = prevAnimation.EndAngleTo;
                this.RectFrom = prevAnimation.RectTo;
                this.RectDtFrom = prevAnimation.RectDtTo;

                this.Duration = TimeSpan.FromMilliseconds(300);
            }
            else if (previousAnimations.Count > 0)
            {
                this.BeginTimeCorrect = TimeSpan.FromMilliseconds(200);
            }
        }
        #endregion

        public StiPieSegmentAnimation(RectangleF rectFrom, RectangleF rectTo, float startAngleFrom, float endAngleFrom, float startAngleTo, float endAngleTo, TimeSpan duration, TimeSpan beginTime)
            : this(rectFrom, rectTo, RectangleF.Empty, RectangleF.Empty, startAngleFrom, endAngleFrom, startAngleTo, endAngleTo, duration, beginTime)
        {
        }

        public StiPieSegmentAnimation(RectangleF rectFrom, RectangleF rectTo, RectangleF rectDtFrom, RectangleF rectDtTo, float startAngleFrom, float endAngleFrom, float startAngleTo, float endAngleTo, TimeSpan duration, TimeSpan beginTime)
        : base(duration, beginTime)
        {
            this.RectFrom = rectFrom;
            this.RectTo = rectTo;
            this.RectDtFrom = rectDtFrom;
            this.RectDtTo = rectDtTo;
            this.StartAngleFrom = startAngleFrom;
            this.EndAngleFrom = endAngleFrom;
            this.StartAngleTo = startAngleTo;
            this.EndAngleTo = endAngleTo;
        }
    }
}