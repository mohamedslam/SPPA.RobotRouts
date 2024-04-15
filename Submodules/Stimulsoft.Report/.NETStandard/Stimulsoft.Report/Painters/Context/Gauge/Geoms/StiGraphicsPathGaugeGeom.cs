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
using System.Collections.Generic;
using System.Drawing;

namespace Stimulsoft.Report.Gauge.GaugeGeoms
{
    public class StiGraphicsPathGaugeGeom : StiGaugeGeom
    {
        #region Properties
        public RectangleF Rect { get; }

        public StiBrush Background { get; }

        public StiBrush BorderBrush { get; }

        public float BorderWidth { get; }

        public PointF StartPoint { get; }

        public List<StiGaugeGeom> Geoms { get; } = new List<StiGaugeGeom>();
        #endregion

        #region Properties.Override
        public override StiGaugeGeomType Type => StiGaugeGeomType.GraphicsPath;

        #endregion

        #region Methods.Add
        public void AddGraphicsPathArcGaugeGeom(float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            if (width > 0 && height > 0)
                this.Geoms.Add(new StiGraphicsPathArcGaugeGeom(x, y, width, height, startAngle, sweepAngle));
        }

        public void AddGraphicsPathCloseFigureGaugeGeom()
        {
            this.Geoms.Add(new StiGraphicsPathCloseFigureGaugeGeom());
        }

        public void AddGraphicsPathLinesGaugeGeom(PointF[] points)
        {
            this.Geoms.Add(new StiGraphicsPathLinesGaugeGeom(points));
        }

        public void AddGraphicsPathLineGaugeGeom(PointF p1, PointF p2)
        {
            this.Geoms.Add(new StiGraphicsPathLineGaugeGeom(p1, p2));
        }
        #endregion

        public StiGraphicsPathGaugeGeom(RectangleF rect, PointF startPoint, StiBrush background, StiBrush borderBrush, float borderWidth)
        {
            this.Rect = rect;
            this.StartPoint = startPoint;
            this.Background = background;
            this.BorderBrush = borderBrush;
            this.BorderWidth = borderWidth;
            this.StartPoint = startPoint;
        }
    }
}