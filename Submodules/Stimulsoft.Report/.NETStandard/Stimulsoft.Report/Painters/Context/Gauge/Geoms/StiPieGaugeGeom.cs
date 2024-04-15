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
    public class StiPieGaugeGeom : StiGaugeGeom
    {
        #region Properties
        public RectangleF Rect { get; }

        public StiBrush Background { get; }

        public StiBrush BorderBrush { get; }

        public float BorderWidth { get; }

        public float StartAngle { get; }

        public float SweepAngle { get; }
        #endregion

        #region Properties.Override
        public override StiGaugeGeomType Type => StiGaugeGeomType.Pie;
        #endregion

        public StiPieGaugeGeom(RectangleF rect, StiBrush background, StiBrush borderBrush, float borderWidth, float startAngle, float sweepAngle)
        {
            this.Rect = rect;
            this.Background = background;
            this.BorderBrush = borderBrush;
            this.BorderWidth = borderWidth;
            this.StartAngle = startAngle;
            this.SweepAngle = sweepAngle;
        }
    }
}