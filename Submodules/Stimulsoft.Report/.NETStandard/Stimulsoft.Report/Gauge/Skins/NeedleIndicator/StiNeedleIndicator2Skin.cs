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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components.Gauge;
using Stimulsoft.Report.Components.Gauge.Primitives;
using Stimulsoft.Report.Gauge.GaugeGeoms;
using Stimulsoft.Report.Painters;
using System;
using System.Drawing;

#if STIDRAWING
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report.Gauge.Skins
{
    public class StiNeedleIndicator2Skin : StiGaugeElementSkin
    {
        public override void Draw(StiGaugeContextPainter context, StiGaugeElement element, RectangleF rect, float? angle, PointF? centerPoint)
        {
            var indicator = element as StiNeedle;

            float minumum = Math.Min(rect.Width, rect.Height);
            float value = minumum / 3;

            var arrayRect = new RectangleF(rect.Left + 1, rect.Top + (rect.Height - value) / 2, rect.Width - 1, value);
            var pathGeom = new StiGraphicsPathGaugeGeom(arrayRect, arrayRect.Location, indicator.Brush, indicator.BorderBrush, indicator.BorderWidth);
            value = minumum / 7;

            var points = new PointF[5];
            points[0] = arrayRect.Location;
            points[1] = new PointF(arrayRect.Right, arrayRect.Top + (arrayRect.Height - value) / 2);
            points[2] = new PointF(arrayRect.Right, arrayRect.Bottom - (arrayRect.Height - value) / 2);
            points[3] = new PointF(arrayRect.Left, arrayRect.Bottom);
            points[4] = points[0];

            pathGeom.AddGraphicsPathLinesGaugeGeom(points);

            bool rotate = (angle != null && centerPoint != null);

            if (rotate)
            {
                context.AddPushMatrixGaugeGeom(angle.Value, centerPoint.Value);
            }

            if (element.Animation != null)
            {
                pathGeom.Animation = element.Animation;
            }

            context.AddGraphicsPathGaugeGeom(pathGeom);

            #region Ellipse
            var ellipseRect = new RectangleF(rect.Location, new SizeF(minumum, minumum));
            var ellipseBrush = new StiSolidBrush(Color.FromArgb(175, 175, 175));
            context.AddEllipseGaugeGeom(ellipseRect, ellipseBrush, indicator.BorderBrush, 1.5f);

            float offset = minumum * 0.15f;

            var internalEllipseRect = new RectangleF(ellipseRect.X + offset, ellipseRect.Top + offset, minumum - (offset * 2), minumum - (offset * 2));
            context.AddEllipseGaugeGeom(internalEllipseRect, new StiEmptyBrush(), new StiSolidBrush(Color.White), 2f);
            #endregion

            if (rotate)
            {
                context.AddPopTranformGaugeGeom();
            }

            if (indicator.ShowValue)
            {
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.NoWrap
                };

                var text = string.Format(indicator.Format, indicator.ValueObj);
                var zoomFont = StiGaugeContextPainter.ChangeFontSize(indicator.Font, context.Zoom);
                var textSize = context.MeasureString(text, zoomFont);
                context.AddTextGaugeGeom(string.Format(indicator.Format, indicator.ValueObj), zoomFont, indicator.TextBrush, new RectangleF(ellipseRect.Location, textSize), sf);
            }
        }
    }
}