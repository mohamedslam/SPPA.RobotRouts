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

using System.Drawing;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Helpers;

namespace Stimulsoft.Report.Chart
{
    public class StiLegendLineMarker : IStiLegendMarker
    {
        public void Draw(StiContext context, IStiSeries serie, RectangleF rect, int colorIndex, int colorCount, int index)
        {
            var colorMarker = Color.Transparent;

            var drawLine = false;
            var drawMarker = false;
            StiFontIcons? icon = null;

            #region StiRadarSeries
            var radarSeries = serie as IStiRadarSeries;
            if (radarSeries != null)
            {
                colorMarker = StiBrush.ToColor(radarSeries.Marker.Brush);
                icon = radarSeries.Marker.Icon;
                drawMarker = true;
            }
            #endregion

            #region StiRadarLineSeries
            var radarLineSeries = serie as IStiRadarLineSeries;
            if (radarLineSeries != null)
            {
                colorMarker = radarLineSeries.LineColor;
                drawMarker = radarLineSeries.Marker.Visible;
                icon = radarLineSeries.Marker.Icon;
                drawLine = true;
            }
            #endregion

            #region StiBaseLineSeries
            var lineSeries = serie as IStiBaseLineSeries;
            if (lineSeries != null)
            {
                colorMarker = lineSeries.LineColor;
                drawMarker = lineSeries.Marker.Visible;
                icon = lineSeries.Marker.Icon;
                drawLine = true;
            }
            #endregion

            #region StiStackedBaseLineSeries
            var stackedLineSeries = serie as IStiStackedBaseLineSeries;
            if (stackedLineSeries != null)
            {
                colorMarker = stackedLineSeries.LineColor;
                drawMarker = stackedLineSeries.Marker.Visible;
                icon = stackedLineSeries.Marker.Icon;
                drawLine = true;
            }
            #endregion

            #region StiScatterSeries
            var scatterSeries = serie as IStiScatterSeries;
            if (scatterSeries != null)
            {
                colorMarker = StiBrush.ToColor(scatterSeries.Marker.Brush);
                icon = scatterSeries.Marker.Icon;
                drawMarker = true;
                drawLine = false;
            }
            #endregion

            #region StiScatterLineSeries
            var scatterLineSeries = serie as IStiScatterLineSeries;
            if (scatterLineSeries != null)
            {
                colorMarker = scatterLineSeries.LineColor;
                drawMarker = scatterLineSeries.Marker.Visible;
                icon = scatterLineSeries.Marker.Icon;
                drawLine = true;
            }
            #endregion

            #region IStiBubbleSeries
            var bubbleSeries = serie as IStiBubbleSeries;
            if (bubbleSeries != null)
            {
                var seriesBrush = bubbleSeries.Brush;
                if (bubbleSeries.AllowApplyStyle)
                {
                    seriesBrush = bubbleSeries.Core.GetSeriesBrush(colorIndex, colorCount);
                    seriesBrush = bubbleSeries.ProcessSeriesBrushes(colorIndex, seriesBrush);
                }
                colorMarker = StiBrush.ToColor(seriesBrush);

                drawMarker = true;
                drawLine = false;
            }
            #endregion

            if (icon != null)
            {
                StiFontIconsExHelper.DrawDirectionIcons(context, new StiSolidBrush(colorMarker), rect, new SizeF(rect.Height, rect.Height), icon.GetValueOrDefault(), null, false);
            }
            else
            {
                if (drawLine)
                {
                    var pen = new StiPenGeom(colorMarker);
                    context.DrawRectangle(pen, rect);
                    context.DrawLine(pen, rect.X, rect.Y + rect.Height / 2, rect.Right, rect.Y + rect.Height / 2);
                }
                if (drawMarker)
                {
                    if (drawLine)
                        rect = new RectangleF(rect.X + rect.Width / 4, rect.Y + rect.Height / 4, rect.Width / 2, rect.Height / 2);
                    context.PushSmoothingModeToAntiAlias();
                    context.FillEllipse(new StiSolidBrush(colorMarker), rect, null, index);
                    context.PopSmoothingMode();
                }
            }
        }
    }
}
