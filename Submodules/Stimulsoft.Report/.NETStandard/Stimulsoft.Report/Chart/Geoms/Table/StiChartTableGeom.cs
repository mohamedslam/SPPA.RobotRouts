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


using System;
using System.Collections.Generic;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiChartTableGeom : StiCellGeom
    {
        public StiChartTableGeom(RectangleF clientRectangle, string[,] table, float widthCellLegendTableChart, float heightCellHeader, float widthSpace, IStiChartTable chartTable)
            : base(clientRectangle)
        {
            this.widthCellLegendTableChart = widthCellLegendTableChart;
            this.heightCellHeader = heightCellHeader;
            this.widthSpace = widthSpace;
            this.table = table;
            this.chartTable = chartTable;

            this.pen = new StiPenGeom(chartTable.GridLineColor);
            this.labelBrush = new StiSolidBrush(chartTable.TextColor);
            this.labelHeaderBrush = new StiSolidBrush(chartTable.Header.TextColor);
        }

        #region Fields
        private string[,] table;
        private float widthCellLegendTableChart;
        private float heightCellHeader;
        private float widthSpace;
        private IStiChartTable chartTable;
        private StiPenGeom pen;
        private StiFontGeom font;
        private StiFontGeom fontHeader;
        private StiBrush labelBrush;
        private StiStringFormatGeom sf;
        private StiStringFormatGeom sfHeader;
        private StiBrush labelHeaderBrush;
        #endregion

        #region Methods
        /// <summary>
        /// Draws table geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            this.font = StiFontGeom.ChangeFontSize(chartTable.DataCells.Font, chartTable.DataCells.Font.Size * context.Options.Zoom);
            this.sf = context.GetGenericStringFormat();
            this.sf.Trimming = StringTrimming.None;
            this.sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
            this.sf.Alignment = StringAlignment.Center;
            this.sf.LineAlignment = StringAlignment.Center;

            this.fontHeader = StiFontGeom.ChangeFontSize(chartTable.Header.Font, chartTable.Header.Font.Size * context.Options.Zoom);
            this.sfHeader = context.GetGenericStringFormat();
            this.sfHeader.Trimming = StringTrimming.None;
            if (!this.chartTable.Header.WordWrap)
                this.sfHeader.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
            this.sfHeader.Alignment = StringAlignment.Center;
            this.sfHeader.LineAlignment = StringAlignment.Center;

            RectangleF rect = this.ClientRectangle;
            StiAxisArea area = chartTable.Chart.Area as StiAxisArea;
            
            #region Draw Header Argument
            float xTA = area.ReverseHor ? rect.X : rect.X + widthCellLegendTableChart;
            float yTA = rect.Y;
            float widthTA = rect.Width - widthCellLegendTableChart;
            float heightTA = heightCellHeader;

            RectangleF rectHeaderArgument = new RectangleF(xTA, yTA, widthTA, heightTA);

            List<string> listArgument = new List<string>();

            for (int index = 1; index < table.GetLength(1); index++)
            {
                listArgument.Add(table[0,index]);
            }

            DrawHeaderArgument(context, rectHeaderArgument, listArgument, area.XAxis.StartFromZero);
            #endregion

            #region Draw Title Legend
            float xTL = area.ReverseHor? rect.Right - widthCellLegendTableChart: rect.X;
            float yTL = rect.Y + heightCellHeader;
            float widthTL = widthCellLegendTableChart;
            float heightTL = rect.Height - heightCellHeader;

            RectangleF rectTitleLegend = new RectangleF(xTL, yTL, widthTL, heightTL);

            List<string> listTitleLegend = new List<string>();

            for (int index = 1; index < table.GetLength(0); index++)
            {
                listTitleLegend.Add(table[index, 0]);
            }

            DrawTitleLegend(context, rectTitleLegend, listTitleLegend);
            #endregion

            #region Draw Root Table

            float xRT = area.ReverseHor ? rect.X : rect.X + widthCellLegendTableChart;
            float yRT = rect.Y + heightCellHeader;
            float widthRT = rect.Width - widthCellLegendTableChart;
            float heightRT = rect.Height - heightCellHeader;

            RectangleF rectRootTable = new RectangleF(xRT, yRT, widthRT, heightRT);

            DrawRootTable(context, rectRootTable, area.XAxis.StartFromZero);

            #endregion
        }

        private void DrawHeaderArgument(StiContext context, RectangleF rect, List<string> listArgument, bool startFromZero)
        {
            context.FillRectangle(this.chartTable.Header.Brush, Rectangle.Ceiling(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height)), null);

            if (chartTable.GridOutline)
                context.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);

            int count = startFromZero ? listArgument.Count + 1 : listArgument.Count - 1;

            float cellWidth = rect.Width / count;

            for (int indexColumn = 0; indexColumn < listArgument.Count; indexColumn++)
            {
                float deltaWidth = 0;
                if (indexColumn == 0 || indexColumn == listArgument.Count - 1)
                {
                    deltaWidth = startFromZero ? cellWidth / 2 : -cellWidth / 2;
                }

                float deltaX = startFromZero ? cellWidth / 2 : -cellWidth / 2;
                if (indexColumn == 0)
                    deltaX = 0;

                var rectX = rect.X + indexColumn * cellWidth + deltaX + widthSpace;
                var rectWidth = cellWidth + deltaWidth - 2 * widthSpace;
                if (rectWidth < 0)
                    rectWidth = 0;

                var rectangleF = new RectangleF(rectX, rect.Y, rectWidth, rect.Height);

                if (this.chartTable.Header.WordWrap)
                    context.DrawRotatedString(listArgument[indexColumn], fontHeader, labelHeaderBrush, rectangleF, sfHeader, StiRotationMode.CenterCenter, 0f, true, (int)Math.Ceiling(rectWidth));
                else
                    context.DrawString(listArgument[indexColumn], fontHeader, labelHeaderBrush, Rectangle.Ceiling(rectangleF), sfHeader);
                
                if (chartTable.GridLinesVert && indexColumn != listArgument.Count - 1)
                {
                    float x = startFromZero ? rect.X + (indexColumn + 1) * cellWidth + cellWidth / 2 : rect.X + (indexColumn + 1) * cellWidth - cellWidth / 2;

                    context.DrawLine(pen, x, rect.Y, x, rect.Bottom);
                }
            }
        }

        private void DrawTitleLegend(StiContext context, RectangleF rect, List<string> list)
        {
            if (chartTable.GridOutline)
                context.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);

            float cellHeight = rect.Height / list.Count;

            for (int indexRow = 0; indexRow < list.Count; indexRow++)
            {
                float x = rect.X;
                float y = rect.Y;
                float width = rect.Width;
                if (chartTable.GridLinesHor && indexRow != 0)
                    context.DrawLine(pen, x, y + cellHeight * indexRow, x + width, y + cellHeight * indexRow);

                if (chartTable.MarkerVisible)
                {
                    RectangleF rectMarker = new RectangleF(x + 2, y + 2 + cellHeight * indexRow, cellHeight - 4, cellHeight - 4);

                    IStiLegendMarker legendMarker = StiMarkerLegendFactory.CreateMarker(chartTable.Chart.Series[indexRow]);
                    legendMarker.Draw(context, chartTable.Chart.Series[indexRow], rectMarker, indexRow, list.Count, -1);

                    x += cellHeight;
                    width -= cellHeight;
                }

                RectangleF rectangleF = new RectangleF(x, y + cellHeight * indexRow, width, cellHeight);
                context.DrawString(list[indexRow], font, labelBrush, rectangleF, sf);
            }
        }

        private void DrawRootTable(StiContext context, RectangleF rect, bool startFromZero)
        {
            if (chartTable.GridOutline)
                context.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);

            int countRow = table.GetLength(0);
            int countColumn = table.GetLength(1);

            int count = startFromZero ? table.GetLength(1) : table.GetLength(1) - 2;
            float cellWidth = rect.Width/count;
            float cellHeight = rect.Height / (countRow - 1);

            var defaultFont = this.font;

            #region Measure Font Size
            if (this.chartTable.DataCells.ShrinkFontToFit)
            {
                for (int indexRow = 1; indexRow < countRow; indexRow++)
                {
                    if (chartTable.GridLinesHor && indexRow != countRow - 1)
                        context.DrawLine(pen, rect.X, rect.Y + indexRow * cellHeight, rect.Right, rect.Y + indexRow * cellHeight);

                    for (int indexColumn = 1; indexColumn < countColumn; indexColumn++)
                    {
                        float deltaWidth = 0;
                        if (indexColumn == 1 || indexColumn == countColumn - 1)
                        {
                            deltaWidth = startFromZero ? cellWidth / 2 : -cellWidth / 2;
                        }

                        float deltaX = startFromZero ? cellWidth / 2 : -cellWidth / 2;
                        if (indexColumn == 1)
                            deltaX = 0;

                        RectangleF rectangleF = new RectangleF(rect.X + (indexColumn - 1) * cellWidth + deltaX, rect.Y + (indexRow - 1) * cellHeight, cellWidth + deltaWidth, cellHeight);

                        var isFontSize = CheckFontSize(context, table[indexRow, indexColumn], defaultFont, rectangleF);

                        while (!isFontSize)
                        {
                            defaultFont.FontSize -= 0.5f;

                            if (defaultFont.FontSize <= this.chartTable.DataCells.ShrinkFontToFitMinimumSize)
                            {
                                defaultFont.FontSize = this.chartTable.DataCells.ShrinkFontToFitMinimumSize;
                                break;
                            }

                            isFontSize = CheckFontSize(context, table[indexRow, indexColumn], defaultFont, rectangleF);                            
                        }
                    }
                }
            }
            #endregion

            for (int indexRow = 1; indexRow < countRow; indexRow++)
            {
                if (chartTable.GridLinesHor && indexRow != countRow - 1)
                    context.DrawLine(pen, rect.X, rect.Y + indexRow * cellHeight, rect.Right, rect.Y + indexRow * cellHeight);

                for (int indexColumn = 1; indexColumn < countColumn; indexColumn++)
                {
                    float deltaWidth = 0;
                    if (indexColumn == 1 || indexColumn == countColumn - 1)
                    {
                        deltaWidth = startFromZero ? cellWidth / 2 : -cellWidth / 2;
                    }

                    float deltaX = startFromZero ? cellWidth / 2 : -cellWidth / 2;
                    if (indexColumn == 1)
                        deltaX = 0;

                    RectangleF rectangleF = new RectangleF(rect.X + (indexColumn - 1) * cellWidth + deltaX, rect.Y + (indexRow - 1) * cellHeight, cellWidth + deltaWidth, cellHeight);
                    
                    context.DrawRotatedString(table[indexRow, indexColumn], defaultFont, labelBrush, rectangleF, sf, StiRotationMode.CenterCenter, 0, true);
                    if (chartTable.GridLinesVert && indexColumn < countColumn - 1)
                    {
                        float x = startFromZero ? rect.X + indexColumn * cellWidth + cellWidth / 2 : rect.X + indexColumn * cellWidth - cellWidth / 2;

                        context.DrawLine(pen, x, rect.Y, x, rect.Bottom);
                    }
                }
            }
        }

        private bool CheckFontSize(StiContext context, string text, StiFontGeom fontGeom, RectangleF rectangleF)
        {
            var measureRect = context.MeasureRotatedString(text, fontGeom, rectangleF, sf, 0);

            return measureRect.Width < rectangleF.Width;
        }

        #endregion
    }
}
