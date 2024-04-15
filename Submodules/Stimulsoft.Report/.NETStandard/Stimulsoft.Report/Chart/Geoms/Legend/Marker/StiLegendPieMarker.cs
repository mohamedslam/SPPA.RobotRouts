#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using System.Collections.Generic;
using System.Drawing;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiLegendPieMarker : IStiLegendMarker
    {
        public void Draw(StiContext context, IStiSeries serie, RectangleF rect, int colorIndex, int colorCount, int index)
        {
            var series = serie as IStiPieSeries;
            var seriesBrush = series.Brush;
            if (series.AllowApplyBrush)
            {
                seriesBrush = series.Core.GetSeriesBrush(colorIndex, colorCount);
                seriesBrush = series.ProcessSeriesBrushes(index, seriesBrush);
            }

            var borderColor = series.BorderColor;
            if (series.AllowApplyBorderColor)
            {
                borderColor = (Color)serie.Core.GetSeriesBorderColor(colorIndex, colorCount);
            }

            var path = new List<StiSegmentGeom>();
            var pen = new StiPenGeom(borderColor);

            context.PushSmoothingModeToAntiAlias();

            path.Add(new StiArcSegmentGeom(
                new RectangleF(
                rect.X - rect.Width, rect.Y,
                rect.Width * 2, rect.Height * 2),
                270, 90));
            path.Add(new StiLineSegmentGeom(rect.Right, rect.Bottom, rect.X, rect.Bottom));
            path.Add(new StiLineSegmentGeom(rect.X, rect.Bottom, rect.X, rect.Y));

            context.FillPath(seriesBrush, path, rect, null, index);
            context.DrawPath(pen, path, null);

            context.PopSmoothingMode();
        }
    }
}
