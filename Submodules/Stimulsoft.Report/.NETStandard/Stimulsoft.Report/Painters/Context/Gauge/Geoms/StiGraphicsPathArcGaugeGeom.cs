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

using System;

namespace Stimulsoft.Report.Gauge.GaugeGeoms
{
    public class StiGraphicsPathArcGaugeGeom : StiGaugeGeom
    {
        #region Properties
        public float X { get; }

        public float Y { get; }

        public float Width { get; }

        public float Height { get; }

        public float StartAngle { get; }

        public float SweepAngle { get; }
        #endregion

        #region Properties.Override
        public override StiGaugeGeomType Type => StiGaugeGeomType.GraphicsPathArc;
        #endregion

        public StiGraphicsPathArcGaugeGeom(float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.StartAngle = startAngle;
            this.SweepAngle = sweepAngle;
        }
    }
}