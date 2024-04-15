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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Stimulsoft.Base.Context.Animation
{
    public class StiRotationAnimation : StiAnimation
    {
        #region Properies
        public double StartAngle { get; set; }

        public double EndAngle { get; set; }

        public PointF CenterPoint { get; set; }
        #endregion

        #region Methods
        private static StiRotationAnimation GetPreviousAnimation(List<StiAnimation> previousAnimations, object id)
        {
            return id != null ? previousAnimations
                                    .Where(a => a is StiRotationAnimation)
                                    .Cast<StiRotationAnimation>()
                                    .FirstOrDefault(a => id.Equals(a.Id)) : null;
        }

        public void ApplyPreviousAnimation(List<StiAnimation> previousAnimations)
        {
            var prevAnimation = GetPreviousAnimation(previousAnimations, Id);

            if (prevAnimation != null)
            {
                this.EndAngle = prevAnimation.StartAngle;
                this.Duration = TimeSpan.FromMilliseconds(300);
            }
        }
        #endregion

        #region Properies.Override
        public override StiAnimationType Type => StiAnimationType.Rotation;
        #endregion

        public StiRotationAnimation(double startAngle, double endAngle, PointF centerPoint, TimeSpan duration, TimeSpan beginTime)
            : base(duration, beginTime)
        {
            this.StartAngle = startAngle;
            this.EndAngle = endAngle;
            this.CenterPoint = centerPoint;
        }
    }
}