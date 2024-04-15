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
    public class StiColumnAnimation : StiAnimation
    {
        #region Properies
        public RectangleF RectFrom { get; set; }

        public RectangleF RectTo { get; set; }
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

        private static StiColumnAnimation GetPreviousAnimation(List<StiAnimation> previousAnimations, object id)
        {
            return id != null ? previousAnimations
                                    .Where(a => a is StiColumnAnimation)
                                    .Cast<StiColumnAnimation>()
                                    .FirstOrDefault(a => id.Equals(a.Id)) : null;
        }

        public void ApplyPreviousAnimation(List<StiAnimation> previousAnimations)
        {
            var prevAnimation = GetPreviousAnimation(previousAnimations, Id);

            if (prevAnimation != null)
            {
                this.RectFrom = prevAnimation.RectTo;
                this.Duration = TimeSpan.FromMilliseconds(300);
            }
            else if (previousAnimations.Count > 0)
            {
                this.BeginTimeCorrect = TimeSpan.FromMilliseconds(200);
                this.Duration = TimeSpan.FromMilliseconds(300);
            }
        }
        #endregion

        #region Properies.Override
        public override StiAnimationType Type => StiAnimationType.Column;
        #endregion

        public StiColumnAnimation(RectangleF rectFrom, RectangleF rectTo, TimeSpan duration, TimeSpan beginTime)
            : base(duration, beginTime)
        {
            this.RectFrom = rectFrom;
            this.RectTo = rectTo;
        }
    }
}