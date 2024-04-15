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
    public class StiTranslationAnimation : StiAnimation
    {
        #region Properties
        public PointF StartPoint { get; set; }

        public PointF EndPoint { get; set; }
        #endregion

        #region Methods
        private static StiTranslationAnimation GetPreviousAnimation(List<StiAnimation> previousAnimations, object id)
        {
            return id != null ? previousAnimations
                                    .Where(a => a is StiTranslationAnimation)
                                    .Cast<StiTranslationAnimation>()
                                    .FirstOrDefault(a => id.Equals(a.Id)) : null;
        }

        public void ApplyPreviousAnimation(List<StiAnimation> previousAnimations)
        {
            var prevAnimation = GetPreviousAnimation(previousAnimations, Id);

            if (prevAnimation != null)
            {
                this.StartPoint = prevAnimation.EndPoint;
                this.Duration = TimeSpan.FromMilliseconds(300);
            }
        }
        #endregion

        #region Properties.Override
        public override StiAnimationType Type => StiAnimationType.Translation;
        #endregion

        public StiTranslationAnimation(TimeSpan duration, TimeSpan beginTime) : this(PointF.Empty, PointF.Empty, duration, beginTime)
        {
        }

        public StiTranslationAnimation(PointF startPoint, PointF endPoint, TimeSpan duration, TimeSpan beginTime) : base(duration, beginTime)
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
        }
    }
}