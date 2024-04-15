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
using System.Collections.Generic;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;
using System.Linq;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Chart
{
    public class StiLegendCoreXF : 
        ICloneable,
        IStiApplyStyle
    {
        #region IStiApplyStyle
        public virtual void ApplyStyle(IStiChartStyle style)
        {
            if (this.Legend.AllowApplyStyle)
            {
                this.Legend.Brush = style.Core.LegendBrush.Clone() as StiBrush;
                this.Legend.LabelsColor = style.Core.LegendLabelsColor;
                this.Legend.TitleColor = style.Core.LegendTitleColor;
                this.Legend.BorderColor = style.Core.LegendBorderColor;
                this.Legend.ShowShadow = style.Core.LegendShowShadow;
                this.Legend.Font = style.Core.LegendFont;
            }
        }
        #endregion		

        #region ICloneable
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region Methods
        public StiCellGeom Render(StiContext context, RectangleF rect)
        {
            int realCountItems;
            var seriesItems = GetLegendItems(out realCountItems);
            if ((!legend.Visible) || seriesItems.Count == 0) return null;

            var countColumns = 0;
            var countRows = 0;
            var legendSize = GetLegendSize(context, rect, out countColumns, out countRows);

            var legendRect = new RectangleF(rect.X, rect.Y, legendSize.Width, legendSize.Height);
            var legendClientRect = legendRect;
            legendClientRect.Location = new PointF(0, 0);

            #region Legend Title
            StiLegendTitleGeom titleGeom = null;
            if (!string.IsNullOrEmpty(legend.Title))
            {
                var titleSize = GetTitleSize(context);
                var titleRect = legendRect;
                titleRect.Location = new PointF(0, 0);
                titleRect.Height = titleSize.Height;

                titleGeom = new StiLegendTitleGeom(legend, titleRect);

                legendClientRect.Y += titleRect.Height;
                legendClientRect.Height -= titleRect.Height;
            }
            #endregion

            #region Legend Area
            var legendGeom = new StiLegendGeom(legend, legendRect, seriesItems);
            if (titleGeom != null)
            {
                legendGeom.CreateChildGeoms();
                legendGeom.ChildGeoms.Add(titleGeom);
                legendGeom.LegendTitleGeom = titleGeom;
            }
            #endregion

            float scaledHorSpacing = legend.HorSpacing * context.Options.Zoom;
            float scaledVertSpacing = legend.VertSpacing * context.Options.Zoom;

            if (legendRect.Width > 0 && legendRect.Height > 0)
            {
                #region Render Series   
                if (legend.Columns == 0)
                {
                    #region Auto Count Columns Mode
                    var startX = scaledHorSpacing + legendClientRect.X;
                    var startY = scaledVertSpacing + legendClientRect.Y;

                    var posX = startX;
                    var posY = startY;

                    var matrixIndexItem = GetMatrixIndexItem(countColumns, countRows, seriesItems.Count);

                    #region Calculate Item Height and Item Real Size #5952
                    float itemHeighthMax = 0;
                    var sizeItemList = new List<SizeF>();

                    for (var indexColumn = 0; indexColumn < matrixIndexItem.GetLength(0); indexColumn++)
                    {
                        for (var indexRow = 0; indexRow < matrixIndexItem.GetLength(1); indexRow++)
                        {
                            var indexItem = matrixIndexItem[indexColumn, indexRow];
                            if (indexItem >= 0)
                            {
                                var seriesItem = seriesItems[indexItem];

                                var itemSize = GetItemRealSize(context, seriesItem);

                                itemHeighthMax = Math.Max(itemHeighthMax, itemSize.Height);

                                sizeItemList.Add(itemSize);
                            }
                        }
                    } 
                    #endregion

                    float itemWidthMax = 0;
                    var index = 0;

                    for (var indexColumn = 0; indexColumn < matrixIndexItem.GetLength(0); indexColumn++)
                    {
                        for (var indexRow = 0; indexRow < matrixIndexItem.GetLength(1); indexRow++)
                        {
                            var indexItem = matrixIndexItem[indexColumn, indexRow];
                            if (indexItem >= 0)
                            {
                                var seriesItem = seriesItems[indexItem];

                                var itemSize = sizeItemList[index];
                                index++;

                                itemWidthMax = Math.Max(itemWidthMax, itemSize.Width);

                                var itemRect = new RectangleF(posX, posY, itemSize.Width, itemHeighthMax);

                                var itemGeom = new StiLegendItemGeom(legend, seriesItem, itemRect, seriesItem.ColorIndex, realCountItems, seriesItem.Index);
                                legendGeom.CreateChildGeoms();
                                legendGeom.ChildGeoms.Add(itemGeom);
                            }
                            posY += itemHeighthMax + scaledVertSpacing;
                        }
                        posY = startY;
                        posX += itemWidthMax + scaledHorSpacing;
                        itemWidthMax = 0;
                    }
                    #endregion
                }
                else
                {
                    #region Custom Count Columns Mode
                    float startX = 0;
                    float startY = 0;
                    SizeF itemSize = GetItemSize(context, seriesItems, 0);

                    #region Get start position
                    if (legend.Direction == StiLegendDirection.LeftToRight || legend.Direction == StiLegendDirection.TopToBottom)
                    {
                        startX = scaledHorSpacing + legendClientRect.X;
                        startY = scaledVertSpacing + legendClientRect.Y;
                    }
                    else if (legend.Direction == StiLegendDirection.RightToLeft)
                    {
                        startX = legendClientRect.Right - scaledHorSpacing - itemSize.Width;
                        startY = scaledVertSpacing + legendClientRect.Y;
                    }
                    else if (legend.Direction == StiLegendDirection.BottomToTop)
                    {
                        startX = scaledHorSpacing + legendClientRect.X;
                        startY = legendClientRect.Bottom - scaledVertSpacing - itemSize.Height;
                    }
                    #endregion

                    float posX = startX;
                    float posY = startY;
                    int columnIndex = 0;
                    int colorIndex = 0;

                    foreach (StiLegendItemCoreXF seriesItem in seriesItems)
                    {
                        itemSize = GetItemSize(context, seriesItems, seriesItem);
                        var itemRect = new RectangleF(posX, posY, itemSize.Width, itemSize.Height);

                        var itemGeom = new StiLegendItemGeom(legend, seriesItem, itemRect, seriesItem.ColorIndex, realCountItems, seriesItem.Index);
                        legendGeom.CreateChildGeoms();
                        legendGeom.ChildGeoms.Add(itemGeom);

                        #region Update position
                        columnIndex++;

                        if (legend.Direction == StiLegendDirection.TopToBottom)
                        {
                            posY += scaledVertSpacing + itemRect.Height;
                            if (columnIndex == countColumns)
                            {
                                posY = startY;
                                posX += itemSize.Width + scaledHorSpacing;
                                columnIndex = 0;
                            }
                        }
                        else if (legend.Direction == StiLegendDirection.LeftToRight)
                        {
                            posX += scaledHorSpacing + itemRect.Width;
                            if (columnIndex == countColumns)
                            {
                                posX = startX;
                                posY += itemSize.Height + scaledVertSpacing;
                                columnIndex = 0;
                            }
                        }
                        else if (legend.Direction == StiLegendDirection.RightToLeft)
                        {
                            itemSize = GetItemSize(context, seriesItems, colorIndex + 1);
                            posX -= scaledHorSpacing + itemSize.Width;
                            if (columnIndex == countColumns)
                            {
                                posX = startX;
                                posY += itemSize.Height + scaledVertSpacing;
                                columnIndex = 0;
                            }
                        }
                        else if (legend.Direction == StiLegendDirection.BottomToTop)
                        {
                            posY -= scaledVertSpacing + itemRect.Height;
                            if (columnIndex == countColumns)
                            {
                                posY = startY;
                                posX += itemSize.Width + scaledHorSpacing;
                                columnIndex = 0;
                            }
                        }
                        #endregion

                        colorIndex++;
                    }
                    #endregion
                }
                #endregion
            }

            return legendGeom;
        }

        private int[,] GetMatrixIndexItem(int countColumns, int countRows, int countItems)
        {
            var matrixIndexItem = new int[countColumns, countRows];
            
            if (legend.Direction == StiLegendDirection.TopToBottom)
            {
                var index = 0;
                for (var indexRow = 0; indexRow < countRows; indexRow++)
                {
                    for (var indexColumn = 0; indexColumn < countColumns; indexColumn++)
                    {
                        matrixIndexItem[indexColumn, indexRow] = index < countItems ? index : -1;
                        index++;
                    }
                }
            }
            else if (legend.Direction == StiLegendDirection.BottomToTop)
            {
                var index = 0;
                for (var indexRow = countRows - 1; indexRow >= 0; indexRow--)
                {
                    for (var indexColumn = 0; indexColumn < countColumns; indexColumn++)
                    {
                        matrixIndexItem[indexColumn, indexRow] = index < countItems ? index : -1;
                        index++;
                    }
                }
            }
            else if (legend.Direction == StiLegendDirection.LeftToRight)
            {
                var index = 0;
                for (var indexColumn = 0; indexColumn < countColumns; indexColumn++)
                {
                    for (var indexRow = 0; indexRow < countRows; indexRow++)
                    {
                        matrixIndexItem[indexColumn, indexRow] = index < countItems ? index : -1;
                        index++;
                    }
                }
            }
            else if (legend.Direction == StiLegendDirection.RightToLeft)
            {
                var index = 0;
                for (var indexColumn = countColumns - 1; indexColumn >= 0; indexColumn--)
                {
                    for (var indexRow = 0; indexRow < countRows; indexRow++)
                    {
                        matrixIndexItem[indexColumn, indexRow] = index < countItems ? index : -1;
                        index++;
                    }
                }
            }

            return matrixIndexItem;
        }

        internal string GetArgumentText(IStiSeries series, int index)
        {
            if (series.Arguments.Length > index && series.Arguments[index] != null)
            {
                return series.Arguments[index].ToString();
            }
            return string.Empty;
        }


        internal int GetLegendItemColumn(List<StiLegendItemCoreXF> seriesItems, StiLegendItemCoreXF seriesItem)
        {
            int seriesIndex = 0;
            int columnIndex = 0;
            foreach (StiLegendItemCoreXF item in seriesItems)
            {
                if (item == seriesItem) break;
                seriesIndex++;
                if (seriesIndex >= legend.Columns)
                {
                    seriesIndex = 0;
                    columnIndex++;
                }
            }

            if (legend.Direction == StiLegendDirection.TopToBottom || legend.Direction == StiLegendDirection.BottomToTop) return columnIndex;
            return seriesIndex;
        }


        internal SizeF GetTitleSize(StiContext context)
        {
            if (string.IsNullOrEmpty(legend.Title))
                return new SizeF(0, 0);
            var newFont = StiFontGeom.ChangeFontSize(legend.TitleFont, legend.TitleFont.Size * context.Options.Zoom);
            var sf = context.GetDefaultStringFormat();

            sf.FormatFlags = 0;
            SizeF size = context.MeasureString(legend.Title, newFont, 1000000000, sf);
            size.Width++;
            return size;
        }


        internal SizeF GetItemSize(StiContext context, List<StiLegendItemCoreXF> seriesItems, int seriesIndex)
        {
            if (seriesIndex >= 0 && seriesIndex < seriesItems.Count)
            {
                var seriesItem = seriesItems[seriesIndex];
                return GetItemSize(context, seriesItems, seriesItem);
            }

            return new SizeF();
        }

        internal SizeF GetItemRealSize(StiContext context, StiLegendItemCoreXF seriesItem)
        {
            var newFont = StiFontGeom.ChangeFontSize(legend.Font, legend.Font.Size * context.Options.Zoom);

            var sizeStr = seriesItem.MeasureString(context, newFont);
            var itemWidth = sizeStr.Width;
            var itemHeight = sizeStr.Height;

            if (legend.MarkerVisible)
            {
                itemWidth += legend.MarkerSize.Width * context.Options.Zoom;
                itemHeight = Math.Max(legend.MarkerSize.Height * context.Options.Zoom, itemHeight);
            }

            return new SizeF(itemWidth, itemHeight);
        }

        internal SizeF GetItemSize(StiContext context, List<StiLegendItemCoreXF> seriesItems, StiLegendItemCoreXF seriesItem)
        {
            float itemWidth = 0;
            float itemHeight = 0;
            int seriesItemColumn = GetLegendItemColumn(seriesItems, seriesItem);

            var newFont = StiFontGeom.ChangeFontSize(legend.Font, legend.Font.Size * context.Options.Zoom);

            foreach (var item in seriesItems)
            {
                if (GetLegendItemColumn(seriesItems, item) == seriesItemColumn)
                {
                    var sizeStr = item.MeasureString(context, newFont);
                    itemWidth = Math.Max(sizeStr.Width, itemWidth);
                    itemHeight = Math.Max(sizeStr.Height, itemHeight);
                }
            }

            if (legend.MarkerVisible)
            {
                itemWidth = Math.Max(legend.MarkerSize.Width * context.Options.Zoom, itemWidth) + legend.MarkerSize.Width * context.Options.Zoom;
                itemHeight = Math.Max(legend.MarkerSize.Height * context.Options.Zoom, itemHeight);
            }

            return new SizeF(itemWidth, itemHeight);
        }

        private SizeF GetItemsAutoSize(StiContext context, List<StiLegendItemCoreXF> seriesItems, RectangleF rect, out int countColumns, out int countRows)
        {
            bool isVertOrientaion = legend.Direction == StiLegendDirection.TopToBottom || legend.Direction == StiLegendDirection.BottomToTop;

            float scaledHorSpacing = legend.HorSpacing * context.Options.Zoom;
            float scaledVertSpacing = legend.VertSpacing * context.Options.Zoom;
            
            var newFont = StiFontGeom.ChangeFontSize(legend.Font, legend.Font.Size * context.Options.Zoom);

            var indexColumn = 0;
            var indexRow = 0;

            var widthColumns = new List<float>();
            var heightRows = new List<float>();

            var itemHeightMax = 0f;

            int? maxColumns = null;

            for (var index = 0; index < seriesItems.Count; index++)
            {
                #region Current Item Width and Height
                var itemCurrent = seriesItems[index];

                var sizeStr = itemCurrent.MeasureString(context, newFont);

                var sizeStrWidth = legend.ColumnWidth == 0 
                    ? sizeStr.Width
                    : legend.ColumnWidth * context.Options.Zoom;

                var itemCurrentWidth = sizeStrWidth + scaledHorSpacing;
                var itemCurrentHeight = sizeStr.Height + scaledVertSpacing;

                if (legend.MarkerVisible)
                {
                    itemCurrentWidth += legend.MarkerSize.Width * context.Options.Zoom;
                    itemCurrentHeight = Math.Max(legend.MarkerSize.Height * context.Options.Zoom + scaledVertSpacing, itemCurrentHeight);
                }

                itemHeightMax = Math.Max(itemCurrentHeight, itemHeightMax);
                #endregion                

                if (isVertOrientaion)
                {
                    if (widthColumns.Count - 1 < indexColumn && maxColumns == null  ||
                        widthColumns.Count - 1 < indexColumn && maxColumns != null && indexColumn <= maxColumns)
                    {
                        widthColumns.Add(itemCurrentWidth);
                    }
                    else if (widthColumns[indexColumn] < itemCurrentWidth)
                    {
                        widthColumns[indexColumn] = itemCurrentWidth;
                        widthColumns.RemoveRange(indexColumn + 1, widthColumns.Count() - (indexColumn + 1));
                        
                        index = -1;
                        indexColumn = 0;
                        indexRow = 0;
                        continue;
                    }

                    indexColumn++;
                    if (maxColumns != null && maxColumns <= indexColumn)
                    {
                        indexColumn = 0;
                        indexRow++;
                    }

                    if (widthColumns.Sum() + scaledHorSpacing > rect.Width && widthColumns.Count > 1)
                    {
                        indexColumn = 0;
                        indexRow++;

                        if (index > 0) index--;
                        if (widthColumns.Count > 0)
                        {
                            widthColumns.RemoveAt(widthColumns.Count - 1);
                            maxColumns = widthColumns.Count();
                        }
                    }
                }
                else
                {
                    if (heightRows.Count - 1 < indexRow)
                        heightRows.Add(itemCurrentHeight);

                    indexRow++;

                    if (widthColumns.Count - 1 < indexColumn)
                    {
                        widthColumns.Add(itemCurrentWidth);
                    }
                    else
                    {
                        widthColumns[indexColumn] = Math.Max(widthColumns[indexColumn], itemCurrentWidth);
                    }

                    if (heightRows.Sum() + scaledVertSpacing > rect.Height && heightRows.Count() > 1)
                    {
                        indexColumn++;
                        indexRow = 0;

                        if (index > 0) index--;
                        if (heightRows.Count > 0) heightRows.RemoveAt(heightRows.Count - 1);
                    }
                }   
            }

            countColumns = widthColumns.Count;

            if (isVertOrientaion)
                countRows = (int)Math.Ceiling((float)seriesItems.Count / (float)countColumns);
            else
                countRows = heightRows.Count();

            return new SizeF(widthColumns.Sum() + scaledHorSpacing, countRows * itemHeightMax + scaledVertSpacing);
        }

        internal SizeF GetItemsSize(StiContext context, List<StiLegendItemCoreXF> seriesItems)
        {
            float scaledHorSpacing = legend.HorSpacing * context.Options.Zoom;

            float itemWidth = 0;
            float itemHeight = 0;

            int columnsCount = legend.Columns;
            if (legend.Direction == StiLegendDirection.TopToBottom || legend.Direction == StiLegendDirection.BottomToTop)
            {
                columnsCount = (int)((seriesItems.Count - 1) / legend.Columns) + 1;
            }

            for (int columnIndex = 0; columnIndex < columnsCount; columnIndex++)
            {
                foreach (var item in seriesItems)
                {
                    if (GetLegendItemColumn(seriesItems, item) == columnIndex)
                    {
                        var itemSize = GetItemSize(context, seriesItems, item);
                        itemWidth += itemSize.Width + scaledHorSpacing;
                        itemHeight = Math.Max(itemSize.Height, itemHeight);
                        break;
                    }
                }
            }
            return new SizeF(itemWidth, itemHeight);
        }
        
        internal SizeF GetSeriesSize(StiContext context, RectangleF rect, out int countColumns, out int countRows)
        {
            float scaledHorSpacing = legend.HorSpacing * context.Options.Zoom;
            float scaledVertSpacing = legend.VertSpacing * context.Options.Zoom;

            var seriesCollection = legend.Chart.Area.Core.GetSeries();

            int realItemsCount;
            var seriesItems = GetLegendItems(out realItemsCount);
            realItemsCount = seriesItems.Count;

            float width = 0;
            float height = 0;

            if (legend.Columns == 0)
            {
                var itemSize = GetItemsAutoSize(context, seriesItems, rect, out countColumns, out countRows);
                width = itemSize.Width;
                height = itemSize.Height;
            }
            else
            {
                countRows = 0;
                countColumns = legend.Columns;
                var itemSize = GetItemsSize(context, seriesItems);

                switch (legend.Direction)
                {
                    case StiLegendDirection.TopToBottom:
                    case StiLegendDirection.BottomToTop:
                        width = itemSize.Width + scaledHorSpacing;
                        if (countColumns > realItemsCount) countColumns = realItemsCount;
                        height = (itemSize.Height + scaledVertSpacing) * countColumns + scaledVertSpacing;
                        break;

                    case StiLegendDirection.LeftToRight:
                    case StiLegendDirection.RightToLeft:
                        int count = realItemsCount / countColumns;
                        if (count * countColumns < realItemsCount) count++;
                        width = itemSize.Width + scaledHorSpacing;
                        height = (itemSize.Height + scaledVertSpacing) * count + scaledVertSpacing;
                        break;
                }
            }

            return new SizeF(width, height);
        }

        internal SizeF GetLegendSize(StiContext context, RectangleF rect, out int countColumns, out int countRows)
        {
            SizeF titleSize = GetTitleSize(context);
            SizeF seriesSize = GetSeriesSize(context, rect, out countColumns, out countRows);
            SizeF size = new SizeF(
                Math.Max(titleSize.Width, seriesSize.Width),
                titleSize.Height + seriesSize.Height);

            float width = 0;
            float height = 0;

            if (legend.Chart != null)
            {
                width = (float)legend.Chart.ConvertToHInches(legend.Size.Width * context.Options.Zoom);
                height = (float)legend.Chart.ConvertToHInches(legend.Size.Height * context.Options.Zoom);
            }
            else
            {
                width = (float)(legend.Size.Width * context.Options.Zoom);
                height = (float)(legend.Size.Height * context.Options.Zoom);
            }

            if (width != 0) size.Width = width;
            if (height != 0) size.Height = height;

            size.Width = (int)Math.Round(size.Width);
            size.Height = (int)Math.Round(size.Height);

            return size;
        }
        
        internal List<StiLegendItemCoreXF> GetLegendItems(out int count)
        {
            count = 0;
            var seriesCollection = legend.Chart.Area.Core.GetSeries();
            var axisArea = legend.Chart.Area as IStiAxisArea;

            var list = new List<StiLegendItemCoreXF>();
            if (legend.Chart.Area.ColorEach ||
                seriesCollection.Any(s => s is IStiFunnelSeries) ||
                seriesCollection.Any(s => s is IStiPictorialSeries) ||
                seriesCollection.Any(s => s is IStiDoughnutSeries))
            {                
                int valuesCount = 0;
                foreach (IStiSeries series in seriesCollection)
                {
                    valuesCount = Math.Max(series.Values.Length, valuesCount);
                }

                int colorIndex = 0;
                bool revert = axisArea != null && (axisArea.ReverseHor || axisArea.AxisCore.SeriesOrientation == StiChartSeriesOrientation.Horizontal);
                
                foreach (IStiSeries series in seriesCollection)
                {
                    if (legend.Chart.Area.Core.IsAcceptableSeries(series.GetType()))
                    {
                        for (int pointIndex = 0; pointIndex < valuesCount; pointIndex++)
                        {
                            if (series.ShowInLegend && series.Values != null)
                            {
                                if (pointIndex >= series.Values.Length) continue;

                                var currentPointIndex = revert ? series.Values.Length - pointIndex - 1 : pointIndex;

                                var value = series.Values[currentPointIndex];

                                if (value != null)
                                {
                                    var pieSeries = series as IStiPieSeries;
                                    var funnelSeries = series as IStiFunnelSeries;
                                    var doughnutSeries = series as IStiDoughnutSeries;

                                    if (!((pieSeries != null && !pieSeries.ShowZeros
                                        || doughnutSeries != null && !doughnutSeries.ShowZeros
                                        || funnelSeries != null && !funnelSeries.ShowZeros)
                                        && value == 0))
                                    {
                                        list.Add(new StiLegendItemCoreXF(
                                        legend.Chart.SeriesLabels.Core.GetLabelText(series, value,
                                        GetArgumentText(series, currentPointIndex), series.Core.GetTag(currentPointIndex), series.CoreTitle, true), series, currentPointIndex, colorIndex));
                                    }
                                }
                            }
                            count++;
                            colorIndex++;
                        }
                    }
                }
            }
            else
            {
                if (legend.Chart.Area is IStiSunburstArea)
                {
                    if (seriesCollection.Count >= 1)
                    {
                        var mainseries = seriesCollection[0];
                        var argumentSeries = mainseries.Arguments.GroupBy(v => v).Select(x => x.Key).ToList();

                        var index = 0;
                        foreach (var argument in argumentSeries)
                        {
                            var argumentText = argument?.ToString();

                            if (!string.IsNullOrEmpty(argumentText))
                                list.Add(new StiLegendItemCoreXF(argumentText, mainseries, index, index));

                            index++;
                            count++;
                        }
                    }
                }
                else if (legend.Chart.Area is IStiBoxAndWhiskerArea)
                {
                    if (seriesCollection.Count >= 1)
                    {
                        var mainseries = seriesCollection[0];
                        var index = 0;
                        foreach (var series in seriesCollection)
                        {
                            list.Add(new StiLegendItemCoreXF(series.CoreTitle, mainseries, index, index));
                            index++;
                            count++;
                        }
                    }
                }
                else if (legend.Chart.Area is IStiWaterfallArea)
                {
                    if (seriesCollection.Count >= 1)
                    {
                        var mainseries = seriesCollection[0] as IStiWaterfallSeries;
                        list.Add(new StiLegendItemCoreXF(StiLocalization.Get("PropertyMain", "Positive"), mainseries, 0, 0));
                        list.Add(new StiLegendItemCoreXF(StiLocalization.Get("PropertyMain", "Negative"), mainseries, 0, 1));
                        list.Add(new StiLegendItemCoreXF(mainseries.Total.Text, mainseries, 0, 2));

                        count = 3;
                    }
                }
                else
                {
                    foreach (IStiSeries series in seriesCollection)
                    {
                        if (legend.Chart.Area.Core.IsAcceptableSeries(series.GetType()))
                        {
                            if (series.ShowInLegend && !(legend.HideSeriesWithEmptyTitle && string.IsNullOrEmpty(series.CoreTitle) && !legend.Chart.IsDesigning))
                            {
                                list.Add(new StiLegendItemCoreXF(series.CoreTitle, series, -1, 0));
                                count++;
                            }
                        }
                    }
                }
            }

            return list;
        }
        #endregion

        #region Properties
        private IStiLegend legend;
        public IStiLegend Legend
        {
            get
            {
                return legend;
            }
            set
            {
                legend = value;
            }
        }
        #endregion

        public StiLegendCoreXF(IStiLegend legend)
        {
            this.legend = legend;
        }
    }
}
