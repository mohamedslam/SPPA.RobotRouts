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

using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Report.Components.Gauge.Primitives;
using Stimulsoft.Report.Gauge.GaugeGeoms;
using Stimulsoft.Report.Painters;
using System.Drawing;

#if STIDRAWING
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report.Gauge.Skins
{
    public abstract class StiMarkerBaseSkin : StiGaugeElementSkin
    {
        protected void AddLines(StiGaugeContextPainter context, StiIndicatorBase indicator, PointF[] points, RectangleF rect, float? angle, PointF? centerPoint, StringFormat sf, StiAnimation animation)
        {
            var value = indicator.ValueObj;
            if (value >= indicator.Scale.Minimum && value <= indicator.Scale.Maximum)
            {
                var pathGeom = new StiGraphicsPathGaugeGeom(rect, points[0], indicator.Brush, indicator.BorderBrush, indicator.BorderWidth);

                pathGeom.AddGraphicsPathLinesGaugeGeom(points);
                pathGeom.AddGraphicsPathCloseFigureGaugeGeom();

                if (animation != null)
                    pathGeom.Animation = animation;

                if (angle != null && centerPoint != null)
                {
                    context.AddPushMatrixGaugeGeom(angle.Value, centerPoint.Value);
                    context.AddGraphicsPathGaugeGeom(pathGeom);
                    context.AddPopTranformGaugeGeom();
                }
                else
                {
                    context.AddGraphicsPathGaugeGeom(pathGeom);
                }

                var marker = indicator as IStiGaugeMarker;
                if (marker != null && marker.ShowValue)
                {
                    if (sf == null)
                    {
                        sf = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center,
                            FormatFlags = StringFormatFlags.NoWrap
                        };
                    }


                    var zoomFont = StiGaugeContextPainter.ChangeFontSize(marker.Font, context.Zoom);
                    context.AddTextGaugeGeom(string.Format(marker.Format, value), zoomFont, marker.TextBrush, rect, sf);
                }
            }
        }
    }
}