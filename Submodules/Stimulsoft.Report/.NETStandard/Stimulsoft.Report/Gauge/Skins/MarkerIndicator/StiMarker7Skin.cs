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

#if STIDRAWING
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report.Gauge.Skins
{
    public class StiMarker7Skin : StiGaugeElementSkin
    {
        public override void Draw(StiGaugeContextPainter context, StiGaugeElement element, RectangleF rect, float? angle, PointF? centerPoint)
        {
            var marker = element as StiMarkerBase;
            var value = marker.ValueObj;

            if (value >= marker.Scale.Minimum && value <= marker.Scale.Maximum)
            {
                if (angle != null && centerPoint != null)
                {
                    context.AddPushMatrixGaugeGeom(angle.Value, centerPoint.Value);
                    context.AddEllipseGaugeGeom(rect, marker.Brush, marker.BorderBrush, marker.BorderWidth);
                    context.AddPopTranformGaugeGeom();
                }
                else
                {
                    context.AddEllipseGaugeGeom(rect, marker.Brush, marker.BorderBrush, marker.BorderWidth);
                }

                if (marker.ShowValue)
                {
                    using (var sf = new StringFormat())
                    {
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;
                        sf.FormatFlags = StringFormatFlags.NoWrap;

                        var zoomFont = StiGaugeContextPainter.ChangeFontSize(marker.Font, context.Zoom);
                        context.AddTextGaugeGeom(string.Format(marker.Format, value), zoomFont, marker.TextBrush, rect, sf);
                    };
                }
            }
        }
    }
}