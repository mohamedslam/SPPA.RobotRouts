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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.CrossTab.Core;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Helpers;
using System;
using System.Collections;
using System.Data;
using System.Linq;

using Stimulsoft.Base.Helpers;
using System.Collections.Generic;
using static Stimulsoft.Report.Engine.StiParser;

namespace Stimulsoft.Report.CrossTab
{
    public static class StiCrossTabHelper
    {
        public static void RenderCells(StiCrossTab masterCrossTab, StiContainer outContainer, int startCol, int startRow, int endCol, int endRow, RectangleM rect, int colCorrection = 0)
        {
            var info = masterCrossTab.CrossTabInfo;
            var cross = info.Cross;

            if (colCorrection == 0)
            {
                info.HidedCells.Clear();
            }            

            var startX = rect.Left - cross.CoordX[startCol];
            var startY = rect.Top - cross.CoordY[startRow];

            for (var rowIndex = startRow; rowIndex < endRow; rowIndex++)
            {
                for (var colIndex = startCol; colIndex < endCol; colIndex++)
                {
                    var cell = cross.Cells[colIndex][rowIndex];

                    if (info.HidedCells[cell] == null)
                    {
                        var cellRight = colIndex + cell.Width;
                        var cellBottom = rowIndex + cell.Height;

                        #region Check borders for big cell
                        if (cellRight > endCol + colCorrection) cellRight = endCol;
                        if (cellBottom > endRow) cellBottom = endRow;
                        #endregion

                        #region Get cell width and height
                        for (var widthIndex = colIndex; widthIndex < cellRight; widthIndex++)
                        {
                            for (var heightIndex = rowIndex; heightIndex < cellBottom; heightIndex++)
                            {
                                var curCell = cross.Cells[widthIndex][heightIndex];
                                info.HidedCells[curCell] = curCell;
                            }
                        }
                        #endregion

                        var cellRect = GetCellSize(colIndex, cellRight, cross, rowIndex, cellBottom, startX, startY);

                        var componentCell = CreateComponentFromCell(cell);
                        if (componentCell != null)
                        {
                            outContainer.Components.Add(componentCell);
                            componentCell.ClientRectangle = cellRect;
                        }

                        SetBottomLine(masterCrossTab, cell, componentCell, rowIndex, endRow);
                    }
                }
            }
        }

        public static RectangleM GetCellRect(StiCrossTab masterCrossTab, int colIndex, int rowIndex)
        {
            var cross = masterCrossTab.CrossTabInfo.Cross;

            var cellWidth = 0m;
            var cellHeight = 0m;

            var cell = cross.Cells[colIndex][rowIndex];

            for (var widthIndex = colIndex; widthIndex < colIndex + cell.Width; widthIndex++)
            {
                cellWidth += cross.Widths[widthIndex];
            }

            for (var heightIndex = rowIndex; heightIndex < rowIndex + cell.Height; heightIndex++)
            {
                cellHeight += cross.Heights[heightIndex];
            }

            return new RectangleM(cross.CoordX[colIndex], cross.CoordY[rowIndex], cellWidth, cellHeight);
        }

        public static SizeM GetCellsRect(StiCrossTab masterCrossTab, int startCol, int startRow, int endCol, int endRow)
        {
            var cellsWidth = 0m;
            var cellsHeight = 0m;

            for (var indexCol = startCol; indexCol < endCol; indexCol++)
            {
                cellsWidth += masterCrossTab.CrossTabInfo.Cross.Widths[indexCol];
            }

            for (var indexRow = startRow; indexRow < endRow; indexRow++)
            {
                cellsHeight += masterCrossTab.CrossTabInfo.Cross.Heights[indexRow];
            }

            return new SizeM(cellsWidth, cellsHeight);
        }

        private static RectangleD GetCellSize(int colIndex, int cellRight, StiCross cross, int rowIndex, int cellBottom, decimal startX, decimal startY)
        {
            var cellWidth = 0m;
            var cellHeight = 0m;

            for (var widthIndex = colIndex; widthIndex < cellRight; widthIndex++)
            {
                cellWidth += cross.Widths[widthIndex];
            }

            for (var heightIndex = rowIndex; heightIndex < cellBottom; heightIndex++)
            {
                cellHeight += cross.Heights[heightIndex];
            }

            return new RectangleD(
                (double)(cross.CoordX[colIndex] + startX),
                (double)(cross.CoordY[rowIndex] + startY),
                (double)cellWidth,
                (double)cellHeight);
        }

        private static StiComponent CreateComponentFromCell(StiCell cell)
        {
            var component = cell.IsImage ? CreateImageFromCell(cell) : CreateTextFromCell(cell);

            if (component != null)
            {
                component.HyperlinkValue = cell.HyperlinkValue;
                component.ToolTipValue = cell.ToolTipValue;
                component.TagValue = cell.TagValue;
                component.DrillDownParameters = cell.DrillDownParameters;
                component.ComponentPlacement = cell.GetComponentPlacement();
            }

            return component;
        }

        private static StiComponent CreateTextFromCell(StiCell cell)
        {
            StiText text = null;

            if (cell.Field != null)
            {
                if (cell.Width > 1 || cell.Height > 1 || (!cell.IsNumeric) || (cell.Field is StiCrossHeader))
                    text = cell.Field.Clone() as StiCrossField;

                else
                    text = cell.Field;

                var header = text as StiCrossHeader;
                if (header != null)
                {
                    header.HeaderLevel = cell.Level;
                    header.HeaderValue = cell.Value != null ? cell.Value.ToString() : null;
                }

                text.Border = cell.Field.Border.Clone() as StiBorder;
                text.MaxSize = new SizeD();
                text.MinSize = new SizeD();
                text.Guid = StiGuidUtils.NewGuid();

                //Components with big height will be breaked automatically
                if (cell.Height > 1)
                    text.CanBreak = true;
            }
            else
            {
                if (!(cell.Width <= 1 && cell.Height <= 1))
                {
                    text = new StiText();
                    text.Border.Side = StiBorderSides.All;
                }
            }

            if (cell.IsNegativeColor)
                text.TextBrush = new StiSolidBrush(StiOptions.Engine.NegativeColor);

            if (text != null)
            {
                text.SetTextInternal(cell.Text);
                return text;
            }
            else
                return null;
        }

        private static StiComponent CreateImageFromCell(StiCell cell)
        {
            StiImage image;

            if (cell.Field != null)
            {
                image = new StiImage();
                image.Border = cell.Field.Border.Clone() as StiBorder;
            }
            else
            {
                image = new StiImage();
                image.Border.Side = StiBorderSides.All;
            }

            if (cell.Value != null)
                image.PutImage(StiImageHelper.GetImageBytesFromObject(cell.Value));

            var crossSummary = cell.Field as StiCrossSummary;
            if (crossSummary != null)
            {
                image.Stretch = crossSummary.Stretch;
                image.AspectRatio = crossSummary.AspectRatio;
                image.HorAlignment = crossSummary.ImageHorAlignment;
                image.VertAlignment = crossSummary.ImageVertAlignment;
            }

            return image;
        }

        private static void SetBottomLine(StiCrossTab crossTab, StiCell cell, StiComponent component, int rowIndex, int endRow)
        {
            var cross = crossTab.CrossTabInfo.Cross;

            var sumCount = cross.SumFields.Count;
            if (sumCount < 2) return;//Work only for the crosstab with multiple summaries

            if (cell.SummaryIndex < 0 || cell.SummaryIndex >= sumCount - 1) return;//Accepted any cells except last
            if (rowIndex != endRow - 1) return;//Only last line in crosstab accepted

            var lastBorder = cross.SumFields.ToList().LastOrDefault() as IStiBorder;
            if (lastBorder == null || !lastBorder.Border.IsBottomBorderSidePresent) return;

            var border = component as IStiBorder;
            if (border == null || border.Border.IsBottomBorderSidePresent) return;

            border.Border.Side += (int)StiBorderSides.Bottom;
        }

        public static DataTable CreateCrossForCrossTabDataSource(StiCrossTab masterCrossTab)
        {
            masterCrossTab.CrossTabInfoV1.StartRow = 0;
            masterCrossTab.CrossTabInfoV1.StartCol = 0;

            StiDataHelper.SetData(masterCrossTab, true, masterCrossTab.Parent as StiDataBand);

            return BuildCrossForCrossTabDataSource(masterCrossTab, false);
        }

        public static DataTable BuildCrossForCrossTabDataSource(StiCrossTab masterCrossTab, bool designTime)
        {
            var colFields = new StiComponentsCollection(null);
            var rowFields = new StiComponentsCollection(null);
            var sumFields = new StiComponentsCollection(null);
            var sumHeaderFields = new StiComponentsCollection(null);
            var rowTitleFields = new StiComponentsCollection(null);
            var colTitleFields = new StiComponentsCollection(null);

            var table = new DataTable();
            foreach (StiComponent component in masterCrossTab.Components)
            {
                if (component is StiCrossCell || component is StiCrossTitle || component is StiCrossSummaryHeader)
                {
                    var row = component as StiCrossRow;
                    var col = component as StiCrossColumn;
                    var sum = component as StiCrossSummary;
                    var sumHeader = component as StiCrossSummaryHeader;
                    var title = component as StiCrossTitle;

                    #region title
                    if (title != null)
                    {
                        if (title.TypeOfComponent.StartsWithInvariant("Row:"))
                            rowTitleFields.Add(title);

                        if (title.TypeOfComponent.StartsWithInvariant("Col:"))
                            colTitleFields.Add(title);

                        continue;
                    }
                    #endregion

                    #region row
                    if (row != null)
                    {
                        rowFields.Add(row);
                        if (!row.IsTotalVisible)
                            row.Total.Height = 0;
                    }
                    #endregion

                    #region col
                    if (col != null)
                    {
                        colFields.Add(col);
                        if (!col.IsTotalVisible)
                            col.Total.Width = 0;
                    }
                    #endregion

                    if (sum != null && !sum.DisabledByCondition && sum.Enabled)
                        sumFields.Add(sum);

                    if (sumHeader != null && !sumHeader.DisabledByCondition && sumHeader.Enabled)
                        sumHeaderFields.Add(sumHeader);

                    var column = new DataColumn(component.Name, typeof(object));
                    table.Columns.Add(column);

                    if (component is StiCrossHeader)
                    {
                        column = new DataColumn($"Display__{component.Name}", typeof(object));
                        table.Columns.Add(column);
                    }
                }
            }

            if (!designTime)
            {
                masterCrossTab.First();
                while (!masterCrossTab.IsEof)
                {
                    var row = table.NewRow();
                    foreach (StiCrossField component in masterCrossTab.Components)
                    {
                        if (component is StiCrossSummaryHeader) continue;

                        var cell = component as StiCrossCell;
                        if (cell != null)
                        {
                            var arg = new StiGetCrossValueEventArgs();
                            cell.InvokeGetCrossValue(arg);
                            row[cell.Name] = arg.Value;

                            if (cell is StiCrossHeader)
                            {
                                arg = new StiGetCrossValueEventArgs();
                                (cell as StiCrossHeader).InvokeGetDisplayCrossValue(arg);

                                row[$"Display__{component.Name}"] = arg.Value;
                            }
                        }
                        else if (component is StiCrossTitle)
                        {
                            var args = new StiGetValueEventArgs();
                            ((StiCrossTitle)component).InvokeGetValue(component, args);
                            ((StiCrossTitle)component).SetTextInternal(args.Value);
                        }
                    }
                    table.Rows.Add(row);

                    masterCrossTab.Next();
                }
            }

            return table;
        }

        public static void BuildCross(StiCrossTab masterCrossTab, bool designTime)
        {
            var colFields = new StiComponentsCollection(null);
            var rowFields = new StiComponentsCollection(null);
            var sumFields = new StiComponentsCollection(null);
            var sumHeaderFields = new StiComponentsCollection(null);
            var rowTitleFields = new StiComponentsCollection(null);
            var colTitleFields = new StiComponentsCollection(null);

            StiCrossTitle crossLeftTitle = null;
            StiCrossTitle crossRightTitle = null;
            StiCrossTitle crossSummaryTitle = null;

            var table = new DataTable();
            foreach (StiComponent component in masterCrossTab.Components)
            {
                if (component is StiCrossCell || component is StiCrossTitle || component is StiCrossSummaryHeader)
                {
                    var row = component as StiCrossRow;
                    var col = component as StiCrossColumn;
                    var title = component as StiCrossTitle;
                    var sum = component as StiCrossSummary;
                    var sumHeader = component as StiCrossSummaryHeader;

                    #region title
                    if (title != null)
                    {
                        if (title.TypeOfComponent.StartsWithInvariant("Row:"))
                            rowTitleFields.Add(title);

                        if (title.TypeOfComponent.StartsWithInvariant("Col:"))
                            colTitleFields.Add(title);

                        if (title.TypeOfComponent.StartsWithInvariant("LeftTitle"))
                            crossLeftTitle = title;

                        if (title.TypeOfComponent.StartsWithInvariant("RightTitle"))
                            crossRightTitle = title;

                        if (title.TypeOfComponent.StartsWithInvariant("SummaryTitle"))
                            crossSummaryTitle = title;
                        continue;
                    }
                    #endregion

                    #region row
                    if (row != null)
                    {
                        rowFields.Add(row);
                        if (!row.IsTotalVisible)
                            row.Total.Height = 0;
                    }
                    #endregion

                    #region col
                    if (col != null)
                    {
                        colFields.Add(col);
                        if (!col.IsTotalVisible)
                            col.Total.Width = 0;
                    }
                    #endregion

                    if (sum != null && !sum.DisabledByCondition && sum.Enabled) {
                        sumFields.Add(sum);

                        sum.Arguments.Clear();
                        try
                        {
                            StiParserParameters parameters = new StiParserParameters
                            {
                                ReturnAsmList = true
                            };
                            List<StiAsmCommand> commands = StiParser.ParseTextValue(sum.Value, sum, null, parameters) as List<StiAsmCommand>;
                            List<StiAsmCommand> pushCommands = commands.Where(c => c.Type == StiAsmCommandType.PushDataSourceField).ToList();
                            if (pushCommands.Count > 1)
                            {
                                pushCommands.ForEach(c =>
                                {
                                    try
                                    {
                                        var colName = c.Parameter1 as string;
                                        sum.Arguments.Add(colName, colName);
                                        var argColumn = new DataColumn(colName, typeof(object));
                                        table.Columns.Add(argColumn);
                                    }
                                    catch { }
                                    
                                });
                            }
                        }
                        catch { }                        
                    }                        

                    if (sumHeader != null && !sumHeader.DisabledByCondition && sumHeader.Enabled)
                        sumHeaderFields.Add(sumHeader);

                    var column = new DataColumn(component.Name, typeof(object));
                    table.Columns.Add(column);

                    if (component is StiCrossHeader)
                    {
                        column = new DataColumn($"Display__{component.Name}", typeof(object));
                        table.Columns.Add(column);
                    }
                }
            }

            if (!designTime)
            {
                #region Calculate all values required for cross-tab rendering
                masterCrossTab.First();
                while (!masterCrossTab.IsEof)
                {
                    var row = table.NewRow();
                    foreach (StiCrossField component in masterCrossTab.Components)
                    {
                        if (component is StiCrossSummaryHeader) continue;
                        var cell = component as StiCrossCell;
                        if (cell != null)
                        {
                            var arg = new StiGetCrossValueEventArgs();
                            cell.InvokeGetCrossValue(arg);

                            object result = arg.Value;
                            if (!(result is string))
                            {
                                result = StiValueHelper.TryToNullableDecimal(arg.Value);
                                if (result == null)
                                    result = arg.Value;
                            }
                            row[cell.Name] = result;

                            if (cell is StiCrossHeader)
                            {
                                arg = new StiGetCrossValueEventArgs();
                                (cell as StiCrossHeader).InvokeGetDisplayCrossValue(arg);

                                row["Display__" + component.Name] = arg.Value;
                            }

                            if (cell is StiCrossSummary)
                            {
                                var summ = cell as StiCrossSummary;
                                foreach (string a in summ.Arguments.Keys)
                                {
                                    var parserResult = StiParser.ParseTextValue($"{{{a}}}", masterCrossTab);
                                    if (parserResult != null)
                                    {
                                        if (!(parserResult is string))
                                        {
                                            parserResult = StiValueHelper.TryToNullableDecimal(parserResult) ?? parserResult;
                                        }
                                        row[a] = parserResult;
                                    }
                                };
                            }
                        }
                        else if (component is StiCrossTitle)
                        {
                            var args = new StiGetValueEventArgs();
                            ((StiCrossTitle)component).InvokeGetValue(component, args);
                            ((StiCrossTitle)component).TextValue = args.Value;
                        }
                    }                                       
                    table.Rows.Add(row);

                    masterCrossTab.Next();
                }
                #endregion
            }

            var cross = masterCrossTab.CrossTabInfo.Cross = new StiCross();
            cross.DesignTime = designTime;

            cross.ColFields = colFields;
            cross.RowFields = rowFields;
            cross.SumFields = sumFields;
            cross.SumHeaderFields = sumHeaderFields;

            cross.ColTitleFields = colTitleFields;
            cross.RowTitleFields = rowTitleFields;

            cross.LeftCrossTitle = crossLeftTitle;
            cross.RightCrossTitle = crossRightTitle;
            cross.SummaryCrossTitle = crossSummaryTitle;

            cross.CrossTab = masterCrossTab;
            cross.Create(table, masterCrossTab.Report, masterCrossTab.SummaryDirection, masterCrossTab.EmptyValue);
            cross.CrossTab = null;
        }

        public static int GetEndCol(StiCrossTab masterCrossTab, int startCol, RectangleM rect, bool forceNoBreak = false)
        {
            var cross = masterCrossTab.CrossTabInfo.Cross;
            var width = 0m;
            var endCol = startCol;
            while (endCol < cross.ColCount)
            {
                width += cross.Widths[endCol];
                if (width > rect.Width && masterCrossTab.HorAlignment != StiCrossHorAlignment.Width) break;
                endCol++;
            }

            if ((endCol == startCol || forceNoBreak) && endCol + 1 <= cross.ColCount)
                endCol++;

            return endCol;
        }

        public static int GetEndRow(StiCrossTab masterCrossTab, int startRow, RectangleM rect, bool forceNoBreak = false)
        {
            var cross = masterCrossTab.CrossTabInfo.Cross;
            var height = 0m;
            var endRow = startRow;
            while (endRow < cross.RowCount)
            {
                height += cross.Heights[endRow];
                if (height > rect.Height) break;
                endRow++;
            }

            if ((endRow == startRow || forceNoBreak) && endRow + 1 <= cross.RowCount) 
                endRow++;

            return endRow;
        }

        public static int CheckMergedRowCells(StiCrossTab masterCrossTab, int startRow, int endRow, int startCol, int endCol)
        {
            var cells = masterCrossTab.CrossTabInfo.Cross.Cells;
            var mHeight = 0;
            for (int x = startCol; x < endCol; x++)
            {
                var cell = cells[x][endRow - 1];
                var parent = cell.ParentCell;

                if (cell.Field != null && cell.Height > 1 && parent != null && parent.KeepMergedCellsTogether)
                    mHeight = Math.Max(mHeight, parent.Height - cell.Height + 1);
            }
            return Math.Max(endRow - mHeight, startRow + 1);
        }

        public static bool IsColFieldsEmpty(StiCrossTab masterCrossTab)
        {
            return masterCrossTab.CrossTabInfo.Cross.ColFields.Count == 1 &&
                   masterCrossTab.CrossTabInfo.Cross.ColFields[0].Name == StiCross.EmptyField;
        }

        public static bool IsRowFieldsEmpty(StiCrossTab masterCrossTab)
        {
            var cross = masterCrossTab.CrossTabInfo.Cross;
            if (cross.IsRowsEmpty && !cross.IsColsEmpty && cross.IsSummariesEmpty) return false;
            return masterCrossTab.CrossTabInfo.Cross.RowFields.Count == 1 &&
                   masterCrossTab.CrossTabInfo.Cross.RowFields[0].Name == StiCross.EmptyField;
        }

        public static void CreateCross(StiCrossTab masterCrossTab)
        {
            masterCrossTab.CrossTabInfoV1.StartRow = 0;
            masterCrossTab.CrossTabInfoV1.StartCol = 0;

            StiDataHelper.SetData(masterCrossTab, true, masterCrossTab.Parent as StiDataBand);

            BuildCross(masterCrossTab, false);

            masterCrossTab.CrossTabInfo.Cross.MaxWidth = (decimal)masterCrossTab.Page.Width;
            masterCrossTab.CrossTabInfo.Cross.MaxHeight = (decimal)masterCrossTab.Page.Height;

            if (masterCrossTab.RightToLeft && (!masterCrossTab.Page.UnlimitedBreakable || StiSubReportsHelper.GetParentBand(masterCrossTab) == null))
                MakeRightToLeft(masterCrossTab);

            masterCrossTab.CrossTabInfo.Cross.DoAutoSize();
        }

        public static void MakeRightToLeft(StiCrossTab masterCrossTab)
        {
            var cross = masterCrossTab.CrossTabInfo.Cross;
            var newCells = new StiCell[cross.ColCount][];

            #region Rotate cells
            for (var indexCol = 0; indexCol < cross.ColCount; indexCol++)
            {
                newCells[indexCol] = new StiCell[cross.RowCount];

                for (var indexRow = 0; indexRow < cross.RowCount; indexRow++)
                {
                    var cell = cross.Cells[cross.ColCount - indexCol - 1][indexRow];
                    newCells[indexCol][indexRow] = cell;
                }
            }

            var hideCells = new Hashtable();
            for (var indexCol = 0; indexCol < cross.ColCount; indexCol++)
            {
                for (var indexRow = 0; indexRow < cross.RowCount; indexRow++)
                {
                    if (indexRow < 2)
                    {
                        var cell = newCells[cross.ColCount - indexCol - 1][indexRow];
                        if (cell != null && cell.Width > 1 && !cell.IsChangeWidthForRightToLeft)
                        {
                            var number = 0;

                            if (hideCells.Contains(cell.Text))
                            {
                                number = (int)hideCells[cell.Text];
                                if (cell.Width == 2)
                                    hideCells.Remove(cell.Text);
                            }
                            else
                            {
                                number = indexCol;
                                hideCells[cell.Text] = indexCol;
                            }
                            var oldCell = newCells[cross.ColCount - number - cell.Width][indexRow];
                            cell.IsChangeWidthForRightToLeft = true;
                            oldCell.IsChangeWidthForRightToLeft = true;
                            newCells[cross.ColCount - number - cell.Width][indexRow] = cell;
                            newCells[cross.ColCount - indexCol - 1][indexRow] = oldCell;
                        }
                    }
                    else
                    {
                        var cell = newCells[indexCol][indexRow];
                        if (cell != null && cell.Width > 1)
                        {
                            var oldCell = newCells[indexCol - cell.Width + 1][indexRow];
                            newCells[indexCol - cell.Width + 1][indexRow] = cell;
                            newCells[indexCol][indexRow] = oldCell;
                        }
                    }
                }
            }

            hideCells.Clear();
            cross.Cells = newCells;
            #endregion

            #region Rotate ColFields
            var comps = new StiComponentsCollection();
            for (var indexCol = cross.ColFields.Count - 1; indexCol >= 0; indexCol--)
            {
                comps.Add(cross.ColFields[indexCol]);
            }
            #endregion
        }

        public static void CalculateMaxAndMin(StiContainer outContainer, ref double maxLeft, ref double maxRight, int startIndex)
        {
            maxLeft = 0;
            maxRight = 0;

            for (var index = startIndex; index < outContainer.Components.Count; index++)
            {
                var comp = outContainer.Components[index];
                if (index == startIndex)
                {
                    maxLeft = comp.Left;
                    maxRight = comp.Right;
                }
                else
                {
                    maxLeft = Math.Min(maxLeft, comp.Left);
                    maxRight = Math.Max(maxRight, comp.Right);
                }
            }
        }

        public static void MakeHorAlignmentByWidth(StiContainer outContainer, int startIndex)
        {
            if (outContainer.Components.Count == 0) return;

            double maxLeft = 0;
            double maxRight = 0;
            CalculateMaxAndMin(outContainer, ref maxLeft, ref maxRight, startIndex);

            var coordForAlignWidth = new Hashtable();

            for (var index = startIndex; index < outContainer.Components.Count; index++)
            {
                var comp = outContainer.Components[index];
                coordForAlignWidth[comp.Left] = comp.Left;
                coordForAlignWidth[comp.Right] = comp.Right;
            }

            var scale = (decimal)outContainer.Width / ((decimal)maxRight - (decimal)maxLeft);

            var keys = new double[coordForAlignWidth.Count];
            coordForAlignWidth.Keys.CopyTo(keys, 0);

            foreach (var value in keys)
            {
                var newValue = value - maxLeft;
                newValue = StiAlignValue.AlignToGrid((double)((decimal)newValue * scale), 0.01, true);

                coordForAlignWidth[value] = newValue;
            }

            for (var index = startIndex; index < outContainer.Components.Count; index++)
            {
                var comp = outContainer.Components[index];

                double valueLeft = 0;
                if (coordForAlignWidth.ContainsKey(comp.Left))
                    valueLeft = (double)coordForAlignWidth[comp.Left];

                double valueRight = 0;
                if (coordForAlignWidth.ContainsKey(comp.Right))
                    valueRight = (double)coordForAlignWidth[comp.Right];

                comp.Left = valueLeft;
                comp.Width = valueRight - valueLeft;

                if (scale < 1)
                {
                    var text = comp as StiText;
                    if (text != null)
                        text.Font = StiFontUtils.ChangeFontSize(text.Font, text.Font.Size * (float)scale);
                }
            }
        }

        public static void ClearCross(StiCrossTab masterCrossTab)
        {
            if (masterCrossTab?.CrossTabInfo?.Cross == null)return;

            var needGCCollect = masterCrossTab.CrossTabInfo.Cross.Clear();
            masterCrossTab.CrossTabInfo.Cross = null;
            if (needGCCollect && StiOptions.Engine.ReportCache.AllowGCCollect)
            {
                GC.Collect();

                if (StiOptions.Engine.AllowWaitForPendingFinalizers)
                    GC.WaitForPendingFinalizers();

                GC.Collect();
            }
        }

    }
}
