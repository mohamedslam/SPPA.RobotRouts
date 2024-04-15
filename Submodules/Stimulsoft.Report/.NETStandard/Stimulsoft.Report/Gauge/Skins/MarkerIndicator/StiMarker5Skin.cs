﻿#region Copyright (C) 2003-2022 Stimulsoft
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

#if STIDRAWING
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report.Gauge.Skins
{
    public class StiMarker5Skin : StiMarkerBaseSkin
    {
        // стрелка, влево-вверх
        public override void Draw(StiGaugeContextPainter context, StiGaugeElement element, RectangleF rect, float? angle, PointF? centerPoint)
        {
            // Top
            var marker = element as StiMarkerBase;

            float btm = rect.Bottom;
            float rgt = rect.Right;
            float offsetY = rect.Top + rect.Height / 3;
            float offsetX = rect.Left + rect.Width / 2;

            var points = new PointF[5];
            points[0] = new PointF(rect.Left, btm);
            points[1] = new PointF(rect.Left, offsetY);
            points[2] = new PointF(offsetX, rect.Top);
            points[3] = new PointF(rgt, offsetY);
            points[4] = new PointF(rgt, btm);

            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Far
            };

            AddLines(context, marker, points, rect, angle, centerPoint, sf, element.Animation);
        }
    }
}