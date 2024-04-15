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
using System.Collections.Generic;
using System.Drawing;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiChartCoreXF :
        ICloneable,
        IStiApplyStyle
    {
        #region ICloneable
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region IStiApplyStyle
        public void ApplyStyle(IStiChartStyle style)
        {
            this.Chart.Style = style;
            if (style == null) return;

            if (((StiChart)Chart).AllowApplyStyle)
                this.Chart.Brush = style.Core.ChartBrush;

            if (this.Chart.Title != null)
                this.Chart.Title.Core.ApplyStyle(style);

            if (this.Chart.Area != null)
                this.Chart.Area.Core.ApplyStyle(style);

            if (this.Chart.Legend != null)
                this.Chart.Legend.Core.ApplyStyle(style);

            if (this.Chart.SeriesLabels != null)
                this.Chart.SeriesLabels.Core.ApplyStyle(style);

            if (this.Chart.Series != null)
                this.Chart.Series.ApplyStyle(style);

            if (this.Chart.ConstantLines != null)
                this.Chart.ConstantLines.ApplyStyle(style);

            if (this.Chart.Strips != null)
                this.Chart.Strips.ApplyStyle(style);

            if (this.Chart.Table != null)
                this.Chart.Table.Core.ApplyStyle(style);
        }
        #endregion

        #region Methods
        public StiCellGeom Render(StiContext context, RectangleF rect, bool useMargins)
        {
            var chartGeom = new StiChartGeom(rect);
            chartGeom.CreateChildGeoms();

            RectangleF fullRectangle;

            #region useMargins
            if (useMargins)
            {
                float zoom = context.Options.Zoom;
                fullRectangle = new RectangleF(
                    rect.X + chart.HorSpacing * zoom,
                    rect.Y + chart.VertSpacing * zoom,
                    rect.Width - chart.HorSpacing * 2 * zoom,
                    rect.Height - chart.VertSpacing * 2 * zoom);
            }
            else
            {
                fullRectangle = rect;
            }
            #endregion

            this.FullRectangle = fullRectangle;

            #region ChartTitle
            StiChartTitleGeom titleGeom = chart.Title.Core.Render(context, chart.Title, fullRectangle) as StiChartTitleGeom;
            if (titleGeom != null)
            {
                switch (chart.Title.Dock)
                {
                    case StiChartTitleDock.Top:
                        fullRectangle.Y += titleGeom.ClientRectangle.Height;
                        fullRectangle.Height -= titleGeom.ClientRectangle.Height;
                        break;

                    case StiChartTitleDock.Right:
                        fullRectangle.Width -= titleGeom.ClientRectangle.Width;
                        break;

                    case StiChartTitleDock.Bottom:
                        fullRectangle.Height -= titleGeom.ClientRectangle.Height;
                        break;

                    case StiChartTitleDock.Left:
                        fullRectangle.X += titleGeom.ClientRectangle.Width;
                        fullRectangle.Width -= titleGeom.ClientRectangle.Width;
                        break;
                }
                chartGeom.ChildGeoms.Add(titleGeom);
            }
            #endregion

            StiAreaGeom areaGeom = null;

            #region Render Legend - First Path
            StiLegendGeom legendGeom = null;
            RectangleF areaRect = fullRectangle;
            if (chart.Legend != null)
            {
                areaGeom = chart.Area.Core.Render(context, areaRect) as StiAreaGeom;

                legendGeom = chart.Legend.Core.Render(context, areaGeom.ClientRectangle) as StiLegendGeom;
                if (legendGeom != null)
                {
                    RectangleF legendRect = legendGeom.ClientRectangle;

                    SetLegendRect(context, chart, ref fullRectangle, ref areaRect, ref legendRect);
                }
            }
            #endregion
            
            #region ChartTable - First Path
            float defaultWidth = 0;
            float defaulHeight = 0;
            if (chart.Table.Core.ShowTable())
            {
                areaGeom = chart.Area.Core.Render(context, areaRect) as StiAreaGeom;

                defaultWidth = chart.Table.Core.GetWidthCellLegend(context) + areaGeom.ClientRectangle.Width;
                defaulHeight = chart.Table.Core.GetHeightTable(context, defaultWidth);

                areaRect.Height -= defaulHeight;
            }
            #endregion

            #region Area
            if (chart.Area != null)
            {
                areaGeom = chart.Area.Core.Render(context, areaRect) as StiAreaGeom;

                if (areaGeom != null)
                {
                    chartGeom.ChildGeoms.Add(areaGeom);
                }
            }
            #endregion

            #region Fill Series Element Index
            var seriesGeoms = chartGeom.GetSeriesElementGeoms();

            for (var index = 0; index < seriesGeoms.Count; index++)
            {
                var seriesElementGeom = ((IStiSeriesElement)seriesGeoms[index]);
                if (seriesElementGeom != null)
                    seriesElementGeom.ElementIndex = index.ToString();
            }
            #endregion

            #region Render Chart Table
            if (chart.Table.Core.ShowTable())
            {
                StiSeriesCollection series = chart.Series;
                StiAxisArea area = this.chart.Area as StiAxisArea;
                if (series != null && series.Count > 0)
                {
                    float x = areaRect.X;
                    //if YAxis width more width Cell Legend
                    if (chart.Table.Core.GetWidthCellLegend(context) < (areaGeom.ClientRectangle.X - areaRect.X))
                        x = areaGeom.ClientRectangle.X - chart.Table.Core.GetWidthCellLegend(context);

                    if (area.ReverseHor && area.YRightAxis.Visible)
                        x = areaGeom.ClientRectangle.Left;

                    float y = areaGeom.ClientRectangle.Bottom;
                    if (chart.Table.Chart.Area is IStiClusteredBarArea)
                        y = areaRect.Bottom;

                    RectangleF rectTable = new RectangleF(x, y, defaultWidth, defaulHeight);
                    StiChartTableGeom tableGeom = chart.Table.Core.Render(context, rectTable) as StiChartTableGeom;

                    chartGeom.ChildGeoms.Add(tableGeom);
                }
            }
            #endregion

            #region Render Legend - Second Path
            chartGeom.CreateChildGeoms();
            if (legendGeom != null)
            {
                areaRect = areaGeom.ClientRectangle;

                RectangleF legendRect = legendGeom.ClientRectangle;
                SetLegendRect(context, chart, ref fullRectangle, ref areaRect, ref legendRect);
                legendGeom.ClientRectangle = legendRect;
                chartGeom.ChildGeoms.Add(legendGeom);
            }
            #endregion

            return chartGeom;
        }
        
        private void SetLegendRect(StiContext context, IStiChart chart, ref RectangleF fullRectangle, ref RectangleF areaRect, ref RectangleF legendRect)
        {
            float scaledChartHorSpacing = chart.HorSpacing * context.Options.Zoom;
            float scaledChartVertSpacing = chart.VertSpacing * context.Options.Zoom;

            #region HorAlignment
            switch (chart.Legend.HorAlignment)
            {
                case StiLegendHorAlignment.LeftOutside:
                    legendRect.X = fullRectangle.X;
                    areaRect.Width -= legendRect.Width + scaledChartHorSpacing;
                    areaRect.X += legendRect.Width + scaledChartHorSpacing;
                    break;

                case StiLegendHorAlignment.Left:
                    legendRect.X = areaRect.X + scaledChartHorSpacing;
                    break;

                case StiLegendHorAlignment.Center:
                    legendRect.X = areaRect.X + (areaRect.Width - legendRect.Width) / 2;
                    break;

                case StiLegendHorAlignment.Right:
                    legendRect.X = areaRect.Right - scaledChartHorSpacing - legendRect.Width;

                    #region Special correct for legend shadow
                    if (chart.Legend.ShowShadow &&
                        (chart.Legend.VertAlignment == StiLegendVertAlignment.BottomOutside))
                    {
                        legendRect.X -= 5;
                        areaRect.Width -= 5;
                    }
                    #endregion
                    break;

                case StiLegendHorAlignment.RightOutside:
                    legendRect.X = fullRectangle.Right - legendRect.Width;
                    areaRect.Width -= legendRect.Width + scaledChartHorSpacing;

                    #region Special correct for legend shadow
                    if (chart.Legend.ShowShadow)
                    {
                        legendRect.X -= 5;
                        areaRect.Width -= 5;
                    }
                    #endregion

                    break;
            }
            #endregion

            #region VertAlignment
            switch (chart.Legend.VertAlignment)
            {
                case StiLegendVertAlignment.TopOutside:
                    legendRect.Y = fullRectangle.Y;
                    areaRect.Height -= legendRect.Height + scaledChartVertSpacing;
                    areaRect.Y += legendRect.Height + scaledChartVertSpacing;
                    break;

                case StiLegendVertAlignment.Top:
                    legendRect.Y = areaRect.Y + scaledChartVertSpacing;
                    break;

                case StiLegendVertAlignment.Center:
                    legendRect.Y = areaRect.Y + (areaRect.Height - legendRect.Height) / 2;
                    break;

                case StiLegendVertAlignment.Bottom:
                    legendRect.Y = areaRect.Bottom - scaledChartVertSpacing - legendRect.Height;

                    #region Special correct for legend shadow
                    if (chart.Legend.ShowShadow &&
                        (chart.Legend.HorAlignment == StiLegendHorAlignment.LeftOutside ||
                        chart.Legend.HorAlignment == StiLegendHorAlignment.RightOutside))
                    {
                        legendRect.Y -= 5;
                        areaRect.Height -= 5;
                    }
                    #endregion

                    break;

                case StiLegendVertAlignment.BottomOutside:
                    legendRect.Y = fullRectangle.Bottom - legendRect.Height;
                    areaRect.Height -= legendRect.Height + scaledChartVertSpacing;

                    #region Special correct for legend shadow
                    if (chart.Legend.ShowShadow)
                    {
                        legendRect.Y -= 5;
                        areaRect.Height -= 5;
                    }
                    #endregion

                    break;
            }
            #endregion

            #region Alignment Correction
            if (chart.Legend.VertAlignment == StiLegendVertAlignment.BottomOutside ||
                chart.Legend.VertAlignment == StiLegendVertAlignment.TopOutside)
            {
                if (chart.Legend.HorAlignment == StiLegendHorAlignment.Left) legendRect.X -= scaledChartHorSpacing;
                if (chart.Legend.HorAlignment == StiLegendHorAlignment.Right) legendRect.X += scaledChartHorSpacing;
            }
            else if (chart.Legend.HorAlignment == StiLegendHorAlignment.RightOutside ||
                chart.Legend.HorAlignment == StiLegendHorAlignment.LeftOutside)
            {
                if (chart.Legend.VertAlignment == StiLegendVertAlignment.Top) legendRect.Y -= scaledChartVertSpacing;
                if (chart.Legend.VertAlignment == StiLegendVertAlignment.Bottom) legendRect.Y += scaledChartVertSpacing;
            }
            #endregion
        }
        #endregion

        #region Properties
        private IStiChart chart;
        public IStiChart Chart
        {
            get
            {
                return chart;
            }
            set
            {
                chart = value;
            }
        }

        internal RectangleF FullRectangle { get; private set; }
        #endregion

        #region Selected
        internal Dictionary<string, bool> SelectedSeriesElementHashtable = null;

        public bool GetIsSelectedSeriesElement(string key)
        {
            if (SelectedSeriesElementHashtable == null || !SelectedSeriesElementHashtable.ContainsKey(key))
                return false;

            return SelectedSeriesElementHashtable[key];
        }

        public void SetIsSelectedSeriesElement(string key, bool value)
        {
            if (SelectedSeriesElementHashtable == null)
                SelectedSeriesElementHashtable = new Dictionary<string, bool>();

            SelectedSeriesElementHashtable[key] = value;
        }
        #endregion

        public StiChartCoreXF(IStiChart chart)
        {
            this.chart = chart;
        }
    }
}
