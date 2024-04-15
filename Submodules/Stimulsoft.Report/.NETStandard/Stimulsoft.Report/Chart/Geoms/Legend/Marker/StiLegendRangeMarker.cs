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

using System;
using System.Drawing;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using System.Collections.Generic;

namespace Stimulsoft.Report.Chart
{
    public class StiLegendRangeMarker : IStiLegendMarker
    {
        public void Draw(StiContext context, IStiSeries series, RectangleF rect, int colorIndex, int colorCount, int index)
        {
            var rangeSeries = series as StiRangeSeries;

            var marker = rangeSeries.Marker;
            var lineStyle = rangeSeries.LineStyle;
            var lineMarker = rangeSeries.LineMarker;
            var lineWidth = rangeSeries.LineWidth;
            var lineColor = rangeSeries.LineColor;
            var lighting = rangeSeries.Lighting;
            var seriesBrush = rangeSeries.Brush;

            List<StiSegmentGeom> path = new List<StiSegmentGeom>();
            var width1 = rect.Width / 5;
            var height1 = rect.Height / 5;
            path.Add(new StiLineSegmentGeom(rect.X, rect.Y + height1 * 3, rect.X, rect.Y + height1 * 3));
            path.Add(new StiLineSegmentGeom(rect.X, rect.Y + height1 * 3, rect.X + width1 * 2, rect.Y));
            path.Add(new StiLineSegmentGeom(rect.X + width1 * 2, rect.Y, rect.X + width1 * 4, rect.Y + height1 * 2));
            path.Add(new StiLineSegmentGeom(rect.X + width1 * 4, rect.Y + height1 * 2, rect.X + width1 * 5, rect.Y + height1 * 1));
            path.Add(new StiLineSegmentGeom(rect.X + width1 * 5, rect.Y + height1 * 1, rect.X + width1 * 5, rect.Y + height1 * 3));

            path.Add(new StiLineSegmentGeom(rect.X + width1 * 5, rect.Y + height1 * 3, rect.X + width1 * 4, rect.Y + height1 * 4));
            path.Add(new StiLineSegmentGeom(rect.X + width1 * 4, rect.Y + height1 * 4, rect.X + width1 * 2, rect.Y + height1 * 3));
            path.Add(new StiLineSegmentGeom(rect.X + width1 * 2, rect.Y + height1 * 3, rect.X, rect.Y + height1 * 5));
            
            path.Add(new StiLineSegmentGeom(rect.X, rect.Y + height1 * 5, rect.X, rect.Y + height1 * 3));

            StiPenGeom pen = new StiPenGeom(lineColor);

            context.PushSmoothingModeToAntiAlias();
            context.FillPath(seriesBrush, path, rect, null, index);
            context.DrawPath(pen, path, rect);

            context.PopSmoothingMode();
        }
    }
}
