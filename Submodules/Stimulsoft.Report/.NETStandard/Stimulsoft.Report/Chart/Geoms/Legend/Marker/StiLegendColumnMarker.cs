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

using System.Drawing;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiLegendColumnMarker : IStiLegendMarker
    {
        public void Draw(StiContext context, IStiSeries series, RectangleF rect, int colorIndex, int colorCount, int index)
        {
            try
            {
                var colorLegend = ((StiSeries)series).ColorLegend;

                var br = colorLegend != null ? new StiSolidBrush(colorLegend.GetValueOrDefault()) : series.Core.GetSeriesBrush(colorIndex, colorCount);
                var pen2 = new StiPenGeom((Color)series.Core.GetSeriesBorderColor(colorIndex, colorCount));

                if (series.Chart != null && series.Chart.Style != null)
                    series.Chart.Style.Core.FillColumn(context, rect, null, br,  null, index);

                context.DrawRectangle(pen2, rect.X, rect.Y, rect.Width, rect.Height);
            }
            catch { }
        }
    }
}
