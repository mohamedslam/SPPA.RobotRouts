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
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Chart
{
    public class StiStockSeriesCoreXF :
        StiSeriesCoreXF
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            if (Series.AllowApplyStyle)
            {
                ((IStiStockSeries)Series).LineColor = color;
            }
        }
        #endregion

        #region Methods
        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] series)
        {
            if (series == null || series.Length == 0 || this.Series.Chart == null) return;

            IStiStockArea stockArea = geom.Area as IStiStockArea;

            float posY = stockArea.AxisCore.GetDividerY();

            var time = StiChartHelper.GlobalBeginTimeElement;
            
            foreach (IStiStockSeries currentSeries in series)
            {
                double?[] valuesOpen = currentSeries.ValuesOpen;
                double?[] valuesClose = currentSeries.ValuesClose;
                double?[] valuesHigh = currentSeries.ValuesHigh;
                double?[] valuesLow = currentSeries.ValuesLow;

                int valueCount = Math.Min(Math.Min(valuesOpen.Length, valuesClose.Length), Math.Min(valuesHigh.Length, valuesLow.Length));
                int argumentCount = currentSeries.Arguments.Length;

                int count = Math.Min(valueCount, argumentCount);

                for (int index = 0; index < count; index++)
                {
                    double? valueOpen = valuesOpen[index];
                    double? valueClose = valuesClose[index];
                    double? valueHigh = valuesHigh[index];
                    double? valueLow = valuesLow[index];

                    int argumentIndex = 0;
                    foreach (StiStripLineXF line in stockArea.XAxis.Info.StripLines)
                    {
                        if (currentSeries.Arguments[index].ToString() == line.ValueObject?.ToString()) break;
                        argumentIndex++;
                    }

                    float singleX = (float)stockArea.XAxis.Info.Dpi;

                    Color color = currentSeries.LineColor;
                    if (!currentSeries.AllowApplyColorNegative && valueOpen < valueClose)
                    {
                        color = currentSeries.LineColorNegative;
                    }

                    if (valueOpen == null || valueClose == null || valueHigh == null || valueLow == null) continue;                    

                    float singleY = (float)stockArea.YAxis.Info.Dpi;
                    float open = Math.Abs(posY - (float)valueOpen * singleY);
                    float close = Math.Abs(posY - (float)valueClose * singleY);
                    float high = Math.Abs(posY - (float)valueHigh * singleY);
                    float low = Math.Abs(posY - (float)valueLow * singleY);

                    float positionX = argumentIndex * singleX;

                    if (stockArea.ReverseHor)
                    {
                        float temp = open;

                        open = close;
                        close = temp;
                    }

                    var clientRect = new RectangleF(positionX - singleX / 3, Math.Min(high, low), singleX * 2 / 3, Math.Abs(low - high));

                    color = currentSeries.ProcessSeriesColors(index, color);

                    var stockGeom = new StiStockSeriesElementGeom(geom, currentSeries, clientRect, open, close, high, low, positionX, 
                        index, color, new TimeSpan(time.Ticks / valueCount * index));

                    if (stockGeom != null && currentSeries.Core.Interaction != null)
                        stockGeom.Interaction = new StiSeriesInteractionData(geom.Area, currentSeries, index);

                    geom.CreateChildGeoms();
                    geom.ChildGeoms.Add(stockGeom);
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
                return StiLocalization.Get("Chart", "Stock");
            }
        }
        #endregion

        public StiStockSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
