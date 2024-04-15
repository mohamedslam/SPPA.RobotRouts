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
using Stimulsoft.Base.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiLegendFunnelMarker : IStiLegendMarker
    {
        public void Draw(StiContext context, IStiSeries serie, RectangleF rect, int colorIndex, int colorCount, int index)
        {
            IStiFunnelSeries series = serie as IStiFunnelSeries;
            StiBrush seriesBrush = series.Brush;
            if (series.AllowApplyBrush)
            {
                seriesBrush = series.Core.GetSeriesBrush(colorIndex, colorCount);
                seriesBrush = series.ProcessSeriesBrushes(colorIndex, seriesBrush);
            }

            Color borderColor = series.BorderColor;
            if (series.AllowApplyBorderColor)
            {
                borderColor = (Color)series.Core.GetSeriesBorderColor(colorIndex, colorCount);
            }

            List<StiSegmentGeom> path = new List<StiSegmentGeom>();
            StiPenGeom pen = new StiPenGeom(borderColor);

            context.PushSmoothingModeToAntiAlias();

            PointF[] points = new PointF[] { new PointF(rect.X, rect.Y), new PointF(rect.Right, rect.Y), 
            new PointF(rect.X + rect.Width * 3 / 4, rect.Top + rect.Height / 2), new PointF(rect.X + rect.Width * 3 / 4, rect.Bottom),
            new PointF(rect.X + rect.Width * 1 / 4, rect.Bottom), new PointF(rect.X + rect.Width * 1 / 4, rect.Top + rect.Height / 2), new PointF(rect.X, rect.Y)};

            path.Add(new StiLinesSegmentGeom(points));
            path.Add(new StiCloseFigureSegmentGeom());

            context.FillPath(seriesBrush, path, rect, null, index);
            context.DrawPath(pen, path, null);

            context.PopSmoothingMode();
        }
    }
}
