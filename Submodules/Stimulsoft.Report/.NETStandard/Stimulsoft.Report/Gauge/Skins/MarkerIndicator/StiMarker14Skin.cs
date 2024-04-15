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
using Stimulsoft.Report.Painters;
using System.Drawing;

namespace Stimulsoft.Report.Gauge.Skins
{
    public class StiMarker14Skin : StiMarkerBaseSkin
    {
        public override void Draw(StiGaugeContextPainter context, StiGaugeElement element, RectangleF rect, float? angle, PointF? centerPoint)
        {
            // Left
            var marker = element as StiMarkerBase;
           
            float btm = rect.Bottom;
            float rgt = rect.Right;
            float height = rect.Height / 3;
            float left = rect.Left + rect.Width / 5;

            var points = new PointF[7];
            points[0] = new PointF(left, btm);
            points[1] = new PointF(left, btm - height);
            points[2] = new PointF(rect.Left, rect.Top + rect.Height / 2);
            points[3] = new PointF(left, rect.Top + height);
            points[4] = new PointF(left, rect.Top);
            points[5] = new PointF(rgt, rect.Top);
            points[6] = new PointF(rgt, btm);

            AddLines(context, marker, points, rect, angle, centerPoint, null, element.Animation);
        }
    }
}