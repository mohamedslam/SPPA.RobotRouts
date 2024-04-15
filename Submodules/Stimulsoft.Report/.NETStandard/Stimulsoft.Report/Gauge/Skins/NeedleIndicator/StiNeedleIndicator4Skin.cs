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

using Stimulsoft.Report.Components.Gauge;
using Stimulsoft.Report.Components.Gauge.Primitives;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Gauge.GaugeGeoms;
using Stimulsoft.Report.Painters;
using System;
using System.Drawing;

#if STIDRAWING
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report.Gauge.Skins
{
    public class StiNeedleIndicator4Skin : StiGaugeElementSkin
    {
        public override void Draw(StiGaugeContextPainter context, StiGaugeElement element, RectangleF rect, float? angle, PointF? centerPoint)
        {
            var indicator = element as StiNeedle;
            float minumum = Math.Min(rect.Width, rect.Height);

            #region Draw Needle
            var needlePointLocation = new PointF(rect.Location.X + minumum / 2 - indicator.OffsetNeedle * rect.Width, rect.Location.Y + rect.Height / 2 - minumum / 2);

            var needleRect = new RectangleF(needlePointLocation, new SizeF(rect.Width - minumum / 2, minumum));

            var points = new PointF[5];
            points[0] = new PointF(needleRect.X, needleRect.Y + needleRect.Height / 2 - (needleRect.Height * indicator.EndWidth) / 2);
            points[1] = new PointF(needleRect.Right, needleRect.Y + needleRect.Height / 2 - (needleRect.Height * indicator.StartWidth) / 2);
            points[2] = new PointF(needleRect.Right, needleRect.Y + needleRect.Height / 2 + (needleRect.Height * indicator.StartWidth) / 2);
            points[3] = new PointF(needleRect.X, needleRect.Y + needleRect.Height / 2 + (needleRect.Height * indicator.EndWidth) / 2);
            points[4] = points[0];

            var pathGeom = new StiGraphicsPathGaugeGeom(needleRect, points[0], indicator.Brush, indicator.BorderBrush, indicator.BorderWidth);
            pathGeom.AddGraphicsPathLinesGaugeGeom(points);

            bool rotate = (angle != null && centerPoint != null);

            if (rotate)
                context.AddPushMatrixGaugeGeom(angle.Value, centerPoint.Value);

            if (element.Animation != null)
            {
                pathGeom.Animation = element.Animation;
            }

            context.AddGraphicsPathGaugeGeom(pathGeom);
            #endregion

            #region Draw Needle Cap
            var capPointLocation = new PointF(rect.Location.X, rect.Location.Y + rect.Height / 2 - minumum / 2);

            var capRect = new RectangleF(capPointLocation, new SizeF(minumum, minumum));

            context.AddEllipseGaugeGeom(capRect, indicator.CapBrush, indicator.CapBorderBrush, indicator.CapBorderWidth);
            #endregion

            if (rotate)
                context.AddPopTranformGaugeGeom();

            var value = indicator.GetActualValue();
            if (indicator.ShowValue && value != null)
            {
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.NoWrap
                };

                var text = string.Format(indicator.Format, value.GetValueOrDefault());
                var zoomFont = StiGaugeContextPainter.ChangeFontSize(indicator.Font, context.Zoom);
                var textSize = context.MeasureString(text, zoomFont);
                context.AddTextGaugeGeom(text, zoomFont, indicator.TextBrush, new RectangleF(capRect.Location, textSize), sf);
            }
        }
    }
}
