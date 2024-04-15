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
    public class StiNeedleIndicator3Skin : StiGaugeElementSkin
    {
        public override void Draw(StiGaugeContextPainter context, StiGaugeElement element, RectangleF rect, float? angle, PointF? centerPoint)
        {
            var indicator = element as StiNeedle;

            float minumum = Math.Min(rect.Width, rect.Height);
            float value = minumum * 0.6f;

            var arrayRect = new RectangleF(rect.Left + (minumum / 2), rect.Top + ((rect.Height - value) / 2), rect.Width - (minumum / 2), value);
            var pathGeom = new StiGraphicsPathGaugeGeom(arrayRect, arrayRect.Location, indicator.Brush, indicator.BorderBrush, indicator.BorderWidth);

            float offsetX = arrayRect.Width * 0.111f;
            float offsetY = arrayRect.Height * 0.15f;

            var points = new PointF[6];
            points[0] = arrayRect.Location;
            points[1] = new PointF(arrayRect.Right - offsetX, arrayRect.Top + offsetY);
            points[2] = new PointF(arrayRect.Right, arrayRect.Top + arrayRect.Height / 2);
            points[3] = new PointF(arrayRect.Right - offsetX, arrayRect.Bottom - offsetY);
            points[4] = new PointF(arrayRect.Left, arrayRect.Bottom);
            points[5] = points[0];

            pathGeom.AddGraphicsPathLinesGaugeGeom(points);
            bool rotate = (angle != null && centerPoint != null);

            if (rotate)
                context.AddPushMatrixGaugeGeom(angle.Value, centerPoint.Value);

            context.AddGraphicsPathGaugeGeom(pathGeom);

            #region Ellipse
            var ellipseRect = new RectangleF(rect.Location, new SizeF(minumum, minumum));
            var ellipseBrush = new StiGradientBrush(Color.FromArgb(230, 229, 229), Color.FromArgb(80, 80, 80), 45f);
            var borderBrush = new StiSolidBrush(Color.FromArgb(98, 94, 90));
            context.AddEllipseGaugeGeom(ellipseRect, ellipseBrush, borderBrush, 1.5f);
            #endregion

            if (rotate)
                context.AddPopTranformGaugeGeom();

            if (element.Animation != null)
            {
                pathGeom.Animation = element.Animation;
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
                context.AddTextGaugeGeom(text, zoomFont, indicator.TextBrush, new RectangleF(ellipseRect.Location, textSize), sf);
            }
        }
    }
}