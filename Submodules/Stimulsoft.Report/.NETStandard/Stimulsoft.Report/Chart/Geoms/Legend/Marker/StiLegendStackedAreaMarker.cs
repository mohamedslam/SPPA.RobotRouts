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
    public class StiLegendStackedAreaMarker : IStiLegendMarker
    {
        public void Draw(StiContext context, IStiSeries series, RectangleF rect, int colorIndex, int colorCount, int index)
        {
            var areaSeries = series as StiStackedAreaSeries;

            var marker = areaSeries.Marker;
            var lineStyle = areaSeries.LineStyle;
            var lineMarker = areaSeries.LineMarker;
            var lineWidth = areaSeries.LineWidth;
            var lineColor = areaSeries.LineColor;
            var lighting = areaSeries.Lighting;
            var seriesBrush = areaSeries.Brush;

            var path = StiLegendMarkerHelper.GetAreaMarkerPath(rect);

            StiPenGeom pen = new StiPenGeom(lineColor);

            context.PushSmoothingModeToAntiAlias();
            context.FillPath(seriesBrush, path, rect, null, index);

            var points = StiLegendMarkerHelper.GetAreaMarkerLinePoints(rect);

            context.DrawLines(pen, points);

            context.PopSmoothingMode();
        }
    }
}