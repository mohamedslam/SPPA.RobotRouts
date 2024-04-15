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
using Stimulsoft.Report.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Stimulsoft.Report.Chart
{
    public class StiPictorialSeriesCoreXF : StiSeriesCoreXF
    {
        #region DataPictorial
        private class DataPictorial
        {
            #region Properties
            public double Value { get; set; }
            
            public StiPictorialSeries Series { get; set; }

            public int Index { get; set; }
            #endregion

            public DataPictorial(double value, StiPictorialSeries series, int index)
            {
                this.Value = value;
                this.Index = index;
                this.Series = series;
            }
        }
        #endregion

        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            var pictorialSeries = this.Series as IStiPictorialSeries;

            if (pictorialSeries.AllowApplyStyle)
            {
                pictorialSeries.Brush = style.Core.GetColumnBrush(color);
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
                return StiLocalization.Get("Chart", "Pictorial");
            }
        }
        #endregion

        #region Fields
        private SizeF singleSizeConst = new Size(25, 25);
        #endregion

        #region Methods
        internal SizeF GetSingleSize(StiContext context)
        {
            return new SizeF(singleSizeConst.Width * context.Options.Zoom, singleSizeConst.Height * context.Options.Zoom);
        }

        private bool ShowEmptyGeom(IStiSeries[] seriesArray)
        {
            foreach (var series in seriesArray)
            {
                if (series.Values.Length > 0)
                    return false;
            }

            return true;
        }

        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] seriesArray)
        {
            var singleSize = GetSingleSize(context);
            var countElementsWidth = (int)Math.Floor(rect.Width / singleSize.Width);
            var countElementsHeight = (int)Math.Floor(rect.Height / singleSize.Height);

            var rectWidth = countElementsWidth * singleSize.Width;
            var rectHeight = countElementsHeight * singleSize.Height;
            var rectX = rect.X + (rect.Width - rectWidth) / 2;
            var rectY = rect.Y + (rect.Height - rectHeight) / 2;
            var rectSeries = new RectangleF(rectX, rectY, rectWidth, rectHeight);
            
            int countElemets = countElementsWidth * countElementsHeight;

            if (ShowEmptyGeom(seriesArray) && seriesArray.Length > 0)
            {
                var seriesPictorialElementGeom = new StiPictorialEmptySeriesElementGeom(geom, 1, 0, new StiSolidBrush(Color.LightGray),
                    seriesArray[0], StiFontIcons.Circle, new List<RectangleF> { rectSeries }, new List<RectangleF> { rectSeries }, rectSeries, null);

                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(seriesPictorialElementGeom);
                return;
            }

            var area = ((StiPictorialArea)this.Series.Chart.Area);
            var datas = new List<DataPictorial>();            
            foreach (var series in seriesArray)
            {
                var ser = series as StiPictorialSeries;
                var indexSer = 0;
                foreach (var value in series.Values)
                {
                    var val = area.RoundValues ? Math.Round(value != null ? value.Value: 0) : value;
                    datas.Add(new DataPictorial(Math.Abs(val.GetValueOrDefault()), ser, indexSer));
                    indexSer++;
                }
            }
            

            var sumValues = datas.Sum(x => x.Value);
            var factor = countElemets / sumValues;

            if (factor > 1 && area.Actual)
                factor = 1;

            var startPointValueRect = new PointF(rectSeries.X, rectSeries.Y);

            var deltaValue = 0d;

            for (var index = 0; index < datas.Count; index++)
            {
                var value = Math.Abs(datas[index].Value);
                if (value == 0) continue;

                var currentFactorValue = factor * value;
                var elementValue = area.RoundValues ? RoundPictorialValue(currentFactorValue, deltaValue) : currentFactorValue;
                deltaValue += currentFactorValue - elementValue;

                var squareElement = elementValue * singleSize.Width * singleSize.Height;

                var sumWidthSquareElement = (float)(squareElement / singleSize.Height);
                var restWidthSquareElement = sumWidthSquareElement;

                var drawRectangles = new List<RectangleF>();
                var clipRectangles = new List<RectangleF>();

                do
                {
                    var curWidth = 0f;
                    var curStartPoint = startPointValueRect;
                    var currentRowWidth = rectSeries.Right - startPointValueRect.X;

                    if (currentRowWidth > restWidthSquareElement)
                    {
                        curWidth = restWidthSquareElement;
                        startPointValueRect = new PointF(startPointValueRect.X + restWidthSquareElement, startPointValueRect.Y);
                    }
                    else
                    {
                        curWidth = currentRowWidth;
                        startPointValueRect = new PointF(rectSeries.X, startPointValueRect.Y + singleSize.Height);
                    }

                    restWidthSquareElement -= curWidth;

                    var mod = (curStartPoint.X - rectSeries.X) / singleSize.Width;
                    var delta = (float)(mod - Math.Floor(mod)) * singleSize.Width;

                    var curRect = new RectangleF(curStartPoint.X - delta, curStartPoint.Y, curWidth + delta, singleSize.Height);
                    var clipRect = new RectangleF(curStartPoint.X, curStartPoint.Y, curWidth, singleSize.Height);
                    drawRectangles.Add(curRect);
                    clipRectangles.Add(clipRect);

                } while (restWidthSquareElement > 0 && startPointValueRect.Y < rectSeries.Bottom);
                
                var seriesPictorialElementGeom = new StiPictorialSeriesElementGeom(geom, value, datas[index].Index, GetSeriesBrush(index, datas.Count), datas[index].Series, datas[index].Series.Icon.GetValueOrDefault(), drawRectangles, clipRectangles, rect, null);

                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(seriesPictorialElementGeom);
            }            
        }

        private double RoundPictorialValue(double currentFactorValue, double deltaValue)
        {
            if (deltaValue > 0)
                return Math.Ceiling(currentFactorValue);

            else if (deltaValue < 0)
                return Math.Floor(currentFactorValue);

            else
                return Math.Round(currentFactorValue);
        }

        public override StiBrush GetSeriesBrush(int colorIndex, int colorCount)
        {
            StiBrush brush = base.GetSeriesBrush(colorIndex, colorCount);
            if (brush == null) return ((IStiPictorialSeries)this.Series).Brush;
            return brush;
        }
        #endregion

        public StiPictorialSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
