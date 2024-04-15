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
    public class StiLegendCandelstickMarker : IStiLegendMarker
    {
        public void Draw(StiContext context, IStiSeries serie, RectangleF rect, int colorIndex, int colorCount, int index)
        {
            IStiCandlestickSeries series = serie as IStiCandlestickSeries;

            StiBrush seriesBrush = series.Brush;

            Color borderColor = series.BorderColor;
            StiPenGeom pen = new StiPenGeom(borderColor);

            context.PushSmoothingModeToAntiAlias();

            PointF pointLeftTop = new PointF(rect.Left + rect.Width / 4, rect.Top + rect.Height / 4);
            float width = rect.Width / 2;
            float height = rect.Height / 2;

            context.FillRectangle(seriesBrush, pointLeftTop.X, pointLeftTop.Y, width, height, null, index);
            context.DrawRectangle(pen, pointLeftTop.X, pointLeftTop.Y, width, height);
            context.DrawLine(pen, rect.X + rect.Width / 2, rect.Y, rect.X + rect.Width / 2, rect.Top + rect.Height / 4);
            context.DrawLine(pen, rect.X + rect.Width / 2, rect.Bottom, rect.X + rect.Width / 2, rect.Bottom - rect.Height / 4);

            context.PopSmoothingMode();  
        }
    }
}
