#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports.Net											}
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

using Stimulsoft.Base.Drawing;
using System.Drawing;

namespace Stimulsoft.Report.Gauge.GaugeGeoms
{
    public class StiRadialRangeGaugeGeom : StiGaugeGeom
    {
        #region Fields
        public RectangleF Rect { get; }

        public StiBrush Background { get; }

        public StiBrush BorderBrush { get; }

        public float BorderWidth { get; }

        public PointF CenterPoint { get; }

        public float StartAngle { get; }

        public float SweepAngle { get; }

        public float Radius1 { get; }

        public float Radius2 { get; }

        public float Radius3 { get; }

        public float Radius4 { get; }
        #endregion

        #region Properties.Override
        public override StiGaugeGeomType Type => StiGaugeGeomType.RadialRange;
        #endregion

        public StiRadialRangeGaugeGeom(RectangleF rect, StiBrush background, StiBrush borderBrush, float borderWidth, PointF centerPoint,
            float startAngle, float sweepAngle, float radius1, float radius2, float radius3, float radius4)
        {
            this.Rect = rect;
            this.Background = background;
            this.BorderBrush = borderBrush;
            this.BorderWidth = borderWidth;
            this.CenterPoint = centerPoint;
            this.StartAngle = startAngle;
            this.SweepAngle = sweepAngle;
            this.Radius1 = radius1;
            this.Radius2 = radius2;
            this.Radius3 = radius3;
            this.Radius4 = radius4;
        }
    }
}