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

using Stimulsoft.Report.Components.Gauge.Primitives;
using Stimulsoft.Report.Gauge.GaugeGeoms;
using Stimulsoft.Report.Painters;
using System.Drawing;

namespace Stimulsoft.Report.Gauge.Skins
{
    public class StiMark3Skin : StiGaugeElementSkin
    {
        public override void Draw(StiGaugeContextPainter context, StiGaugeElement element, RectangleF rect, float? angle, PointF? centerPoint)
        {
            var tickMark = element as StiTickMarkBase;

            var points = new PointF[4];
            float offsetX = rect.Left + rect.Width / 2;
            float offsetY = rect.Top + rect.Height / 2;

            points[0] = new PointF(rect.Left, offsetY);
            points[1] = new PointF(offsetX, rect.Top);
            points[2] = new PointF(rect.Right, offsetY);
            points[3] = new PointF(offsetX, rect.Bottom);

            var pathGeom = new StiGraphicsPathGaugeGeom(rect, points[0], tickMark.Brush, tickMark.BorderBrush, tickMark.BorderWidth);

            pathGeom.AddGraphicsPathLinesGaugeGeom(points);
            pathGeom.AddGraphicsPathCloseFigureGaugeGeom();
            context.AddGraphicsPathGaugeGeom(pathGeom);
        }
    }
}