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

using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using System;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiCandlestickSeriesCoreXF :
        StiSeriesCoreXF
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            if (Series.AllowApplyStyle)
            {
                var brush = style.Core.GetColumnBrush(color);

                ((IStiCandlestickSeries)Series).Brush = brush;

                var borderColor = style.Core.GetColumnBorder(color);

                if (borderColor == Color.Transparent)
                {
                    borderColor = StiBrush.ToColor(brush);
                }

                ((IStiCandlestickSeries)Series).BorderColor = borderColor;
            }
        }
        #endregion

        #region Methods
        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] series)
        {
            if (series == null || series.Length == 0 || this.Series.Chart == null) return;

            var candlestickArea = geom.Area as IStiCandlestickArea;
            var posY = candlestickArea.AxisCore.GetDividerY();
            var time = StiChartHelper.GlobalBeginTimeElement;
            
            foreach (IStiCandlestickSeries currentSeries in series)
            {
                double?[] values = currentSeries.ValuesOpen;
                double?[] valuesEnd = currentSeries.ValuesClose;
                double?[] valuesHigh = currentSeries.ValuesHigh;
                double?[] valuesLow = currentSeries.ValuesLow;

                int valueCount = Math.Min(Math.Min(values.Length, valuesEnd.Length), Math.Min(valuesHigh.Length, valuesLow.Length));
                int argumentCount = currentSeries.Arguments.Length;

                int count = Math.Min(valueCount, argumentCount);

                for (int index = 0; index < count; index++)
                {
                    var value = values[index];
                    var valueEnd = valuesEnd[index];
                    var valueHigh = valuesHigh[index];
                    var valueLow = valuesLow[index];
                    if (value == null || valueEnd == null || valueHigh == null || valueLow == null) continue;

                    int argumentIndex = 0;
                    foreach (StiStripLineXF line in candlestickArea.XAxis.Info.StripLines)
                    {
                        if (currentSeries.Arguments[index]?.ToString() == line.ValueObject?.ToString()) break;
                        argumentIndex++;
                    }

                    float singleX = (float)candlestickArea.XAxis.Info.Dpi;
                    StiBrush candlestickBrush;
                    Color candlestickBorderColor;

                    #region Correction Values
                    if (value > valueEnd)
                    {
                        candlestickBrush = currentSeries.Brush;
                        candlestickBorderColor = currentSeries.BorderColor;

                        if (valueHigh < value) valueHigh = value;
                        if (valueEnd < valueLow) valueLow = valueEnd;
                    }
                    else
                    {
                        candlestickBrush = currentSeries.BrushNegative;
                        candlestickBorderColor = currentSeries.BorderColorNegative;

                        if (valueLow > value) valueLow = value;
                        if (valueEnd > valueHigh) valueHigh = valueEnd;

                        value = valuesEnd[index];
                        valueEnd = values[index];
                    }

                    if (candlestickArea.ReverseVert && value > valueEnd)
                    {
                        double? temp = value;

                        value = valueEnd;
                        valueEnd = temp;

                        temp = valueLow;

                        valueLow = valueHigh;
                        valueHigh = temp;
                    }
                    #endregion

                    var singleY = (float)candlestickArea.YAxis.Info.Dpi;
                    var bodyStart = Math.Abs(posY - (float)value * singleY);
                    var bodyEnd = Math.Abs(posY - (float)valueEnd * singleY);
                    var highCandle = Math.Abs(posY - (float)valueHigh * singleY);
                    var lowCandle = Math.Abs(posY - (float)valueLow * singleY);
                    
                    var positionX = argumentIndex * singleX;

                    var clientRect = new RectangleF(positionX - singleX / 4, bodyStart, singleX / 2, bodyEnd - bodyStart);

                    candlestickBrush = currentSeries.ProcessSeriesBrushes(index, candlestickBrush);
                    
                    var candlestickGeom = new StiCandlestickSeriesElementGeom(geom, currentSeries, clientRect,bodyStart, bodyEnd, highCandle, lowCandle, positionX,
                        index, candlestickBrush, candlestickBorderColor, new TimeSpan(time.Ticks / valueCount * index));

                    if (candlestickGeom != null && currentSeries.Core.Interaction != null)
                        candlestickGeom.Interaction = new StiSeriesInteractionData(geom.Area, currentSeries, index);

                    geom.CreateChildGeoms();
                    geom.ChildGeoms.Add(candlestickGeom);
                }
            }
        }
        #endregion

        #region Properties.Localization
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string LocalizedName
        {
            get
            {
                return StiLocalization.Get("Chart", "Candlestick");
            }
        }
        #endregion

        public StiCandlestickSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
