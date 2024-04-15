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
    public class StiNeedleIndicator1Skin : StiGaugeElementSkin
    {
        public override void Draw(StiGaugeContextPainter context, StiGaugeElement element, RectangleF rect, float? angle, PointF? centerPoint)
        {
            var indicator = element as StiNeedle;

            float minumum = Math.Min(rect.Width, rect.Height);
            float value = minumum / 4;
            float height = rect.Height * 0.6f;

            #region BottomPath
            var rectBottomArrow = new RectangleF(rect.Left + value, rect.Top + rect.Height * 0.2f, rect.Width - value, height);

            var points = new PointF[3];
            points[0] = rectBottomArrow.Location;
            points[1] = new PointF(rectBottomArrow.Right, rectBottomArrow.Top + rectBottomArrow.Height / 2);
            points[2] = new PointF(rectBottomArrow.Left, rectBottomArrow.Bottom);

            var pathGeom = new StiGraphicsPathGaugeGeom(rectBottomArrow, points[0], new StiGradientBrush(Color.FromArgb(255, 248, 210), Color.FromArgb(0, 255, 248, 210), 0f), new StiEmptyBrush(), 0f);
            pathGeom.AddGraphicsPathLinesGaugeGeom(points);
            pathGeom.AddGraphicsPathCloseFigureGaugeGeom();
            #endregion

            #region TopPath
            height = rect.Height * 0.4f;
            float offsetHeight = (height * 0.4f);

            var rectTopArrow = new RectangleF(rect.Left + value, rect.Top + rect.Height * 0.3f, rect.Width - value, height);

            var points1 = new PointF[4];
            points1[0] = rectTopArrow.Location;
            points1[1] = new PointF(rectTopArrow.Right, rectTopArrow.Top + offsetHeight);
            points1[2] = new PointF(rectTopArrow.Right, rectTopArrow.Bottom - offsetHeight);
            points1[3] = new PointF(rectTopArrow.Left, rectTopArrow.Bottom);

            var path1Geom = new StiGraphicsPathGaugeGeom(rectTopArrow, points1[0],
                new StiGradientBrush(Color.FromArgb(34, 34, 34), Color.FromArgb(255, 210, 90), 0f), new StiEmptyBrush(), 0f);
            path1Geom.AddGraphicsPathLinesGaugeGeom(points1);
            path1Geom.AddGraphicsPathCloseFigureGaugeGeom();
            #endregion

            bool rotate = (angle != null && centerPoint != null);
            var ellipseRect = new RectangleF(rect.Location, new SizeF(minumum, minumum));

            if (rotate)
            {
                context.AddPushMatrixGaugeGeom(angle.Value, centerPoint.Value);
            }

            if (element.Animation != null)
            {
                pathGeom.Animation = path1Geom.Animation = element.Animation;
            }

            context.AddGraphicsPathGaugeGeom(pathGeom);
            context.AddGraphicsPathGaugeGeom(path1Geom);
            context.AddEllipseGaugeGeom(ellipseRect, new StiGradientBrush(Color.FromArgb(120, 120, 120), Color.FromArgb(132, 117, 77), 0f), new StiSolidBrush(Color.FromArgb(228, 188, 83)), 1f);

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