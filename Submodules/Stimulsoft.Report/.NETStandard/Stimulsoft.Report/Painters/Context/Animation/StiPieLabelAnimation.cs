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
using System.Drawing;

namespace Stimulsoft.Base.Context.Animation
{
    public class StiPieLabelAnimation : StiAnimation
    {
        #region Properies
        public double? ValueFrom { get; set; }

        public double? Value { get; set; }

        public RectangleF RectLabelFrom { get; set; }

        public RectangleF RectLabel { get; set; }

        public RectangleF ClientRect { get; set; }

        public float AngleFrom { get; set; }

        public float Angle { get; set; }
        #endregion

        #region Properies.Override
        public override StiAnimationType Type => StiAnimationType.Translation;
        #endregion

        public StiPieLabelAnimation(double? valueFrom, double? value, float angleFrom, float angle, RectangleF clientRect, RectangleF rectLabelFrom, RectangleF rectLabel, TimeSpan duration, TimeSpan beginTime) : base(duration, beginTime)
        {
            this.ValueFrom = valueFrom;
            this.Value = value;
            this.ClientRect = clientRect;
            this.RectLabelFrom = rectLabelFrom;
            this.RectLabel = rectLabel;
            this.AngleFrom = angleFrom;
            this.Angle = angle;
        }
    }
}