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
    public class StiLabelAnimation : StiAnimation
    {
        #region Properies
        public PointF PointFrom { get; set; }

        public PointF Point { get; set; }

        public double? ValueFrom { get; set; }

        public double? Value { get; set; }

        public RectangleF LabelRect { get; set; }
        #endregion

        #region Properies.Override
        public override StiAnimationType Type => StiAnimationType.Translation;
        #endregion

        public StiLabelAnimation(TimeSpan duration, TimeSpan beginTime) : this(null, null, PointF.Empty, PointF.Empty, duration, beginTime)
        {
        }

        public StiLabelAnimation(double? valueFrom, double? value, PointF pointFrom, PointF point, TimeSpan duration, TimeSpan beginTime) : base(duration, beginTime)
        {
            this.PointFrom = pointFrom;
            this.Point = point;
            this.ValueFrom = valueFrom;
            this.Value = value;
        }
    }
}