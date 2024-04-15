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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.Engine;
using System.Threading;
using Stimulsoft.Data.Functions;
using Stimulsoft.Data.Engine;
using Stimulsoft.Base.Drawing;
using static Stimulsoft.Report.Engine.StiParser;

namespace Stimulsoft.Report.CrossTab.Core
{
    public class StiCross : StiGrid
    {
        #region Consts
        private const string StrNull = "";
        public const string EmptyField = "EMPTY_FIELD";
        #endregion

        #region Fields
        private int oneCellSize;
        private int oneCellWidth;
        private int oneCellHeight;

        private StiSummaryDirection summaryDirection = StiSummaryDirection.LeftToRight;

        private int colsHeaderHeight;
        private int rowsHeaderWidth;

        private int widthCorrection;
        private int heightCorrection;

        private int colsWidth;
        private int rowsHeight;

        private Hashtable columnsCell = new Hashtable();
        private Hashtable rowsCell = new Hashtable();
        #endregion

        #region Methods
        private void InvokeEvents(StiComponent component, string displayValue)
        {
            var isCompilationMode = Report?.CalculationMode == StiCalculationMode.Compilation;
            if (isCompilationMode)
            {
                #region Process Hyperlink
                var e1 = new StiValueEventArgs { DisplayValue = displayValue };
                component.InvokeGetHyperlink(component, e1);
                component.HyperlinkValue = e1.Value;
                #endregion

                #region Process ToolTip
                var e2 = new StiValueEventArgs { DisplayValue = displayValue };
                component.InvokeGetToolTip(component, e2);
                component.ToolTipValue = e2.Value;
                #endregion

                #region Process Tag
                var e3 = new StiValueEventArgs { DisplayValue = displayValue };
                component.InvokeGetTag(component, e3);
                component.TagValue = e3.Value;
                #endregion
            }
            else
            {
                #region Process Hyperlink
                if (component.Hyperlink.Value.Length > 0)
                {
                    var parserResult = StiParser.ParseTextValue(component.Hyperlink.Value, component);
                    component.HyperlinkValue = Report.ToString(parserResult);
                }
                #endregion

                #region Process ToolTip
                if (component.ToolTip.Value.Length > 0)
                {
                    var parserResult = StiParser.ParseTextValue(component.ToolTip.Value, component);
                    component.ToolTipValue = Report.ToString(parserResult);
                }
                #endregion

                #region Process Tag
                if (component.Tag.Value.Length > 0)
                {
                    var parserResult = StiParser.ParseTextValue(component.Tag.Value, component);
                    component.TagValue = Report.ToString(parserResult);
                }
                #endregion
            }

            component.InvokeBeforePrint(component, EventArgs.Empty);
            component.InvokeAfterPrint(component, EventArgs.Empty);
        }

        private void InvokeEvents(StiComponent component)
        {
            var isCompilationMode = Report?.CalculationMode == StiCalculationMode.Compilation;

            if (isCompilationMode)
            {
                #region Process Hyperlink
                var e1 = new StiValueEventArgs();
                component.InvokeGetHyperlink(component, e1);
                component.HyperlinkValue = e1.Value;
                #endregion

                #region Process ToolTip
                var e2 = new StiValueEventArgs();
                component.InvokeGetToolTip(component, e2);
                component.ToolTipValue = e2.Value;
                #endregion

                #region Process Tag
                var e3 = new StiValueEventArgs();
                component.InvokeGetTag(component, e3);
                component.TagValue = e3.Value;
                #endregion
            }
            else
            {
                #region Process Hyperlink
                if (component.Hyperlink.Value.Length > 0)
                {
                    var parserResult = StiParser.ParseTextValue(component.Hyperlink.Value, component);
                    component.HyperlinkValue = Report.ToString(parserResult);
                }
                #endregion

                #region Process ToolTip
                if (component.ToolTip.Value.Length > 0)
                {
                    var parserResult = StiParser.ParseTextValue(component.ToolTip.Value, component);
                    component.ToolTipValue = Report.ToString(parserResult);
                }
                #endregion

                #region Process Tag
                if (component.Tag.Value.Length > 0)
                {
                    var parserResult = StiParser.ParseTextValue(component.Tag.Value, component);
                    component.TagValue = Report.ToString(parserResult);
                }
                #endregion
            }

            component.InvokeBeforePrint(component, EventArgs.Empty);
            component.InvokeAfterPrint(component, EventArgs.Empty);
        }

        private void AddRowTotal(StiRowCollection rows, int level, int maxLevel, int setLevel)
        {
            while (true)
            {
                var row = new StiRow("", "")
                {
                    IsTotal = true,
                    Level = setLevel
                };

                var e = new StiGetValueEventArgs();
                var total = ((StiCrossRow)RowFields[setLevel]).Total;
                if (total != null)
                    total.InvokeGetValue(total, e);

                row.DisplayValue = e.Value;

                rows.Add(row);

                level++;
                if (level >= maxLevel) return;

                rows = rows[rows.Count - 1].Rows;
            }
        }

        private void AddColTotal(StiColumnCollection cols, int level, int maxLevel, int setLevel)
        {
            while (true)
            {
                var col = new StiColumn("", "")
                {
                    IsTotal = true,
                    Level = setLevel
                };

                var e = new StiGetValueEventArgs();
                var total = ((StiCrossColumn)ColFields[setLevel]).Total;
                if (total != null)
                    total.InvokeGetValue(total, e);

                col.DisplayValue = e.Value;
                cols.Add(col);

                level++;
                if (level >= maxLevel) return;

                cols = cols[cols.Count - 1].Cols;
            }
        }
        
        private void SortRows()
        {
            SortRows(Rows, 0, GetRowsHeaderWidth());
        }

        private void SortRows(StiRowCollection rows, int level, int width)
        {
            foreach (StiRow row in rows)
            {
                SortRows(row.Rows, level + 1, width);
            }
            if (level < width)
            {
                var sortDirection = (RowFields[level] as StiCrossRow).SortDirection;
                if (sortDirection != StiSortDirection.None)
                    rows.Sort(sortDirection, (RowFields[level] as StiCrossRow).SortType);
            }
        }
        
        private void SortCols()
        {
            SortCols(Cols, 0, GetColsHeaderHeight());
        }

        private void SortCols(StiColumnCollection cols, int level, int height)
        {
            foreach (StiColumn col in cols)
            {
                SortCols(col.Cols, level + 1, height);
            }

            if (level < height)
            {
                var sortDirection = (ColFields[level] as StiCrossColumn).SortDirection;
                if (sortDirection != StiSortDirection.None)
                    cols.Sort(sortDirection, (ColFields[level] as StiCrossColumn).SortType);
            }
        }

        private void CreateRowTotals()
        {
            var maxLevel = GetRowsHeaderWidth();
            if (maxLevel > 0)
                CreateRowTotals(Rows, 0, maxLevel);
        }

        private void CreateRowTotals(StiRowCollection rows, int level, int maxLevel)
        {
            if (rows.Count > 0)
            {
                foreach (StiRow row in rows)
                {
                    CreateRowTotals(row.Rows, level + 1, maxLevel);
                }

                var curField = RowFields[level] as StiCrossRow;
                var prevField = level == 0 ? null : RowFields[level - 1] as StiCrossRow;

                if (AllowTotal(curField, prevField))
                    AddRowTotal(rows, level, maxLevel, level);
            }
        }
        
        private void CreateColTotals()
        {
            var maxLevel = GetColsHeaderHeight();
            if (maxLevel > 0)
                CreateColTotals(Cols, 0, maxLevel);
        }

        private void CreateColTotals(StiColumnCollection cols, int level, int maxLevel)
        {
            if (cols.Count > 0)
            {
                foreach (StiColumn col in cols)
                {
                    CreateColTotals(col.Cols, level + 1, maxLevel);
                }

                var curField = ColFields[level] as StiCrossColumn;
                var prevField = level == 0 ? null : ColFields[level - 1] as StiCrossColumn;

                if (AllowTotal(curField, prevField))
                    AddColTotal(cols, level, maxLevel, level);
            }
        }
        
        private object GetDataFromDataRow(DataRow dataRow, string field)
        {
            if (dataRow == null) return StrNull;
            return dataRow[field] ?? StrNull;
        }

        private object GetValueFromDataRow(DataRow dataRow, StiCrossSummary field)
        {
            return GetValueFromDataRow(dataRow, field.Name, field.Summary, field.HideZeros);
        }

        private object GetValueFromDataRow(DataRow dataRow, string fieldName, StiSummaryType fieldSummary, bool hideZeros)
        {
            if (dataRow == null) return null;

            try
            {
                var value = dataRow[fieldName];

                switch (fieldSummary)
                {
                    case StiSummaryType.Image:
                    case StiSummaryType.Count:
                    case StiSummaryType.CountDistinct:
                        return value;

                    case StiSummaryType.None:
                        if (value is DateTime) return value;

                        if (!hideZeros)
                            return value;

                        else
                        {
                            decimal? valueObj = null;

                            try
                            {
                                var text = value as string;
                                if (text != null)
                                {
                                    //Проверяем если хоть одна цифра в строке и если есть то только тогда предпринимаем попытку преобразовать строку в число
                                    if (!text.Any(char.IsDigit)) return value;
                                    text = text.Replace(".", ",").Replace(",", Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(" ", "");
                                    decimal result;

                                    if (decimal.TryParse(text, out result))
                                        valueObj = result;
                                }
                                else
                                {
                                    valueObj = (decimal)global::System.Convert.ChangeType(value, typeof(decimal));
                                }
                                if (valueObj == 0) return string.Empty;
                            }
                            catch
                            {
                            }
                            return value;
                        }

                    default:
                        if (value is string)
                        {
                            var text = value as string;
                            if (text.Length == 0) return null;

                            text = text.Replace(".", ",").Replace(",",
                                Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                            decimal result;
                            return decimal.TryParse(text, out result) ? result : 0m;
                        }

                        if (value is DateTime) return value;
                        if (value == null) return 0m;
                        if (value == DBNull.Value) return 0m;

                        return (decimal)global::System.Convert.ChangeType(value, typeof(decimal));
                }

            }
            catch
            {
                return 0;
            }
        }

        private bool AllowTotal(StiCrossHeader row, StiCrossHeader prevRow)
        {
            if (row == null) return false;

            if (prevRow != null)
            {
                var interaction = prevRow.Interaction as StiCrossHeaderInteraction;
                if (interaction != null && interaction.CollapsingEnabled) return false;
            }
            return row.IsTotalVisible;
        }

        private StiRow GetRow(DataRow dataRow)
        {
            var split = false;
            var curRows = Rows;
            StiRow selectedRow = null;

            var level = 0;
            foreach (StiCrossRow rowField in rowFields)
            {
                object value = StrNull;
                object displayValue = StrNull;
                if (rowField.Name != EmptyField)
                {
                    value = GetDataFromDataRow(dataRow, rowField.Name);
                    displayValue = GetDataFromDataRow(dataRow, "Display__" + rowField.Name);
                }

                if (split)
                {
                    value = "all";
                    displayValue = "all";
                }
                

                selectedRow = curRows[value];
                if (selectedRow == null)
                {
                    selectedRow = new StiRow(value, displayValue);

                    #region Process Hyperlink
                    var e1 = new StiValueEventArgs();
                    rowField.InvokeGetHyperlink(rowField, e1);
                    selectedRow.HyperlinkValue = e1.Value;
                    #endregion

                    #region Process ToolTip
                    var e2 = new StiValueEventArgs();
                    rowField.InvokeGetToolTip(rowField, e2);
                    selectedRow.ToolTipValue = e2.Value;
                    #endregion

                    #region Process Tag
                    var e3 = new StiValueEventArgs();
                    rowField.InvokeGetTag(rowField, e3);
                    selectedRow.TagValue = e3.Value;
                    #endregion

                    curRows.Add(selectedRow);
                }

                split = StiCrossTabV2Builder.IsCollapsed(rowField, level, value as string);
                curRows = selectedRow.Rows;
                level++;
            }

            return selectedRow;
        }

        private StiColumn GetColumn(DataRow dataRow)
        {
            var split = false;
            var curCols = Cols;
            StiColumn selectedCol = null;

            var level = 0;
            foreach (StiCrossColumn colField in colFields)
            {
                object value = string.Empty;
                object displayValue = string.Empty;
                if (colField.Name != EmptyField)
                {
                    value = GetDataFromDataRow(dataRow, colField.Name);
                    displayValue = GetDataFromDataRow(dataRow, "Display__" + colField.Name);
                }

                if (split)
                {
                    value = "all";
                    displayValue = "all";
                }

                selectedCol = curCols[value];
                if (selectedCol == null)
                {
                    selectedCol = new StiColumn(value, displayValue);
                  
                    #region Process Hyperlink
                    var e1 = new StiValueEventArgs();
                    colField.InvokeGetHyperlink(colField, e1);
                    selectedCol.HyperlinkValue = e1.Value;
                    #endregion

                    #region Process ToolTip
                    var e2 = new StiValueEventArgs();
                    colField.InvokeGetToolTip(colField, e2);
                    selectedCol.ToolTipValue = e2.Value;
                    #endregion

                    #region Process Tag
                    var e3 = new StiValueEventArgs();
                    colField.InvokeGetTag(colField, e3);
                    selectedCol.TagValue = e3.Value;
                    #endregion

                    curCols.Add(selectedCol);
                }

                split = StiCrossTabV2Builder.IsCollapsed(colField, level, value as string);
                curCols = selectedCol.Cols;
                level++;
            }
            return selectedCol;
        }

        private void CalculateTopN()
        {
            for (int level = colFields.Count - 1; level >= 0 ; level--)
            {
                StiCrossColumn colField = colFields[level] as StiCrossColumn;
                var sumIndex = GetSumFiledIndex(colField.TopN);
                if (sumIndex >= 0)
                    ProcessTopNColumns(level, 0, sumIndex, Cols, colField.TopN);
            }

            for (int level = rowFields.Count - 1; level >= 0; level--)
            {
                StiCrossRow rowField = rowFields[level] as StiCrossRow;
                var sumIndex = GetSumFiledIndex(rowField.TopN);
                if (sumIndex >= 0)
                    ProcessTopNRows(level, 0, sumIndex, Rows, rowField.TopN);
            }
        }

        private void ProcessTopNRows(int level, int curLevel, int sumIndex, StiRowCollection rows, StiDataTopN topN)
        {
            if (curLevel < level)
                foreach (StiRow row in rows)
                    ProcessTopNRows(level, curLevel + 1, sumIndex, row.Rows, topN);
            else
            {
                var values = new SortedList();
                foreach (StiRow row in rows)
                {
                    var array = new List<StiRow>();
                    GetRowsArray(row.Rows, array);
                    if (array.Count == 0)
                        array.Add(row);

                    var sums = new ArrayList();

                    foreach (StiRow srow in array)
                    {
                        foreach (StiColumn col in SummaryContainer.GetDataCol().Keys)
                        {
                            var summary = (SummaryContainer.GetDataCol()[col] as Hashtable)[srow] as StiSummary;
                            if (summary != null)
                                foreach (var val in summary.Sums[sumIndex])
                                    sums.Add(val);
                        }       
                    }

                    var value = GetSummaryResult(sums, sumIndex, false);
                    if (values[value] == null)
                        values[value] = new List<StiRow>();
                    (values[value] as List<StiRow>).Add(row);
                }

                var sortedRows = new List<StiRow>();
                foreach (var value in values.Keys)
                    sortedRows.AddRange(values[value] as List<StiRow>);

                var resultRows = new List<StiRow>();
                for (int i = (topN.Mode == StiDataTopNMode.Top ? Math.Max(0, sortedRows.Count - topN.Count) : 0); i < (topN.Mode == StiDataTopNMode.Top ? sortedRows.Count : Math.Min(sortedRows.Count, topN.Count)); i++)
                    resultRows.Add(sortedRows[i]);

                int j = 0;
                var otherRows = new List<StiRow>();
                while (j < rows.Count)
                    if (!resultRows.Contains(rows[j]))
                    {
                        otherRows.Add(rows[j]);
                        rows.RemoveAt(j);
                    }
                    else j++;

                if (topN.ShowOthers && otherRows.Count > 0)
                {
                    var otherRow = new StiRow(topN.OthersText, topN.OthersText)
                    {
                        OthersText = topN.OthersText
                    };
                    rows.Add(otherRow);

                    FillOtherRows(otherRow, otherRows);
                }
            }
        }

        private void FillOtherRows(StiRow otherRow, IList rows)
        {
            foreach (StiRow row in rows)
                if (row.Rows.Count == 0)
                {
                    foreach (StiColumn col in SummaryContainer.GetDataCol().Keys)
                    {
                        var summary = (SummaryContainer.GetDataCol()[col] as Hashtable)[row] as StiSummary;
                        if (summary != null)
                        {
                            var otherSummary = SummaryContainer.GetSummary(col, otherRow);
                            var index = 0;
                            foreach (StiCrossSummary field in sumFields)
                                if (field.Name != EmptyField)
                                {
                                    if (otherSummary.Sums[index].Count == 0)
                                    {
                                        otherSummary.HyperlinkValues = summary.HyperlinkValues;
                                        otherSummary.ToolTipValues = summary.ToolTipValues;
                                        otherSummary.TagValues = summary.TagValues;
                                        otherSummary.DrillDownParameters = summary.DrillDownParameters;
                                    }
                                    otherSummary.Sums[index].AddRange(summary.Sums[index]);
                                    index++;
                                }
                        }
                    }
                }
                else
                {
                    foreach (StiRow sRow in row.Rows)
                    {
                        if (otherRow.Rows[sRow.Value] == null) otherRow.Rows.Add(sRow);
                        else
                        {
                            var otherRows = new List<StiRow>();
                            otherRows.Add(sRow);
                            FillOtherRows(otherRow.Rows[sRow.Value], otherRows);
                        }
                    }
                }
        }

        private void ProcessTopNColumns(int level, int curLevel, int sumIndex, StiColumnCollection cols, StiDataTopN topN)
        {
            if (curLevel < level)
                foreach (StiColumn col in cols)
                    ProcessTopNColumns(level, curLevel + 1, sumIndex, col.Cols, topN);
            else
            {
                var values = new SortedList();
                foreach (StiColumn col in cols)
                {   
                    var array = new List<StiColumn>();
                    GetColsArray(col.Cols, array);
                    if (array.Count == 0)
                        array.Add(col);

                    var sums = new ArrayList();

                    foreach (StiColumn scol in array)
                    {
                        Hashtable dataRow = SummaryContainer.GetDataCol()[scol] as Hashtable;
                        if (dataRow != null)
                            foreach (StiSummary summary in dataRow.Values)
                                foreach (var val in summary.Sums[sumIndex])
                                    sums.Add(val);
                    }

                    var value = GetSummaryResult(sums, sumIndex, false);
                    if (values[value] == null)
                        values[value] = new List<StiColumn>();
                    (values[value] as List<StiColumn>).Add(col);
                }

                var sortedCols = new List<StiColumn>();
                foreach (var value in values.Keys)
                    sortedCols.AddRange(values[value] as List<StiColumn>);

                var resultCols = new List<StiColumn>();
                for (int i = (topN.Mode == StiDataTopNMode.Top ? Math.Max(0, sortedCols.Count - topN.Count) : 0); i < (topN.Mode == StiDataTopNMode.Top ? sortedCols.Count : Math.Min(sortedCols.Count, topN.Count)); i++)
                    resultCols.Add(sortedCols[i]);

                int j = 0;
                var otherCols = new List<StiColumn>();
                while (j < cols.Count)
                    if (!resultCols.Contains(cols[j]))
                    {
                        otherCols.Add(cols[j]);
                        cols.RemoveAt(j);
                    } else j++;

                if (topN.ShowOthers && otherCols.Count > 0)
                {
                    var otherCol = new StiColumn(topN.OthersText, topN.OthersText)
                    {
                        OthersText = topN.OthersText
                    };
                    cols.Add(otherCol);

                    FillOtherColumns(otherCol, otherCols);
                }
            }
        }

        private void FillOtherColumns(StiColumn otherCol, IList cols)
        {
            foreach (StiColumn col in cols)
                if (col.Cols.Count == 0)
                {
                    Hashtable dataRow = SummaryContainer.GetDataCol()[col] as Hashtable;
                    if (dataRow != null)
                        foreach (StiRow row in dataRow.Keys)
                        {
                            var otherSummary = SummaryContainer.GetSummary(otherCol, row);
                            var summary = dataRow[row] as StiSummary;

                            var index = 0;
                            foreach (StiCrossSummary field in sumFields)
                            {
                                if (field.Name != EmptyField)
                                {
                                    if (otherSummary.Sums[index].Count == 0)
                                    {
                                        otherSummary.HyperlinkValues = summary.HyperlinkValues;
                                        otherSummary.ToolTipValues = summary.ToolTipValues;
                                        otherSummary.TagValues = summary.TagValues;
                                        otherSummary.DrillDownParameters = summary.DrillDownParameters;
                                    }
                                    otherSummary.Sums[index].AddRange(summary.Sums[index]);
                                    index++;
                                }
                            }
                        }
                }
                else
                {
                    foreach (StiColumn sCol in col.Cols)
                    {
                        if (otherCol.Cols[sCol.Value] == null) otherCol.Cols.Add(sCol);
                        else
                        {
                            var otherCols = new List<StiColumn>();
                            otherCols.Add(sCol);
                            FillOtherColumns(otherCol.Cols[sCol.Value], otherCols);
                        }
                    }   
                }                
        }

        private int GetSumFiledIndex(StiDataTopN topN)
        {
            if (topN != null && topN.Mode != StiDataTopNMode.None && !string.IsNullOrEmpty(topN.MeasureField))
            {
                for (int i = 0; i < sumFields.Count; i++)
                    if (sumFields[i].Alias.EndsWith($"{topN.MeasureField})"))
                        return i;
            }
            return -1;
        }

        private void CalculateDataTable(DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
                CalculateDataRow(null);

            else
            {
                #region Process DataSource
                if (CrossTab.DataSource != null)
                {
                    var resPosition = CrossTab.DataSource.Position;
                    var index = 0;
                    foreach (DataRow dataRow in table.Rows)
                    {
                        CrossTab.DataSource.Position = index++;
                        CalculateDataRow(dataRow);
                    }
                    CrossTab.DataSource.Position = resPosition;
                }
                #endregion

                #region Process BusinessObject
                else if (CrossTab.BusinessObject != null)
                {
                    var resPosition = CrossTab.BusinessObject.Position;
                    var index = 0;
                    foreach (DataRow dataRow in table.Rows)
                    {
                        CrossTab.BusinessObject.Position = index++;
                        CalculateDataRow(dataRow);
                    }
                    CrossTab.BusinessObject.Position = resPosition;
                }
                #endregion
            }
        }

        private void CalculateDataRow(DataRow dataRow)
        {
            var row = GetRow(dataRow);
            var col = GetColumn(dataRow);

            #region Calc Summary Data
            var summary = SummaryContainer.GetSummary(col, row);

            var index = 0;
            foreach (StiCrossSummary field in sumFields)
            {
                if (field.Name != EmptyField)
                {
                    var summaryValue = GetValueFromDataRow(dataRow, field);

                    summary.Sums[index].Add(summaryValue);
                    InvokeEvents(field);
                    summary.HyperlinkValues[index] = field.HyperlinkValue;
                    summary.ToolTipValues[index] = field.ToolTipValue;
                    summary.TagValues[index] = field.TagValue;
                    if (field.DrillDownParameters != null)
                    {
                        if (summary.DrillDownParameters == null)
                            summary.DrillDownParameters = new Dictionary<string, object>[summary.TagValues.Length];

                        summary.DrillDownParameters[index] = field.DrillDownParameters;
                    }

                    foreach (string argument in field.Arguments.Keys)
                    {
                        var argumentValue = GetValueFromDataRow(dataRow, argument, field.Summary, field.HideZeros);
                        if (!summary.Arguments[index].ContainsKey(argument))
                        {
                            summary.Arguments[index].Add(argument, new ArrayList());
                        }

                        (summary.Arguments[index][argument] as ArrayList).Add(argumentValue);
                    };

                    index++;
                }
            }
            #endregion

            #region Calc Row Data
            foreach (StiCrossRow field in rowFields)
            {
                if (field.Name != EmptyField)
                {
                    InvokeEvents(field);
                    row.HyperlinkValue = field.HyperlinkValue;
                    row.ToolTipValue = field.ToolTipValue;
                    row.TagValue = field.TagValue;

                    if (field.DrillDownParameters != null)
                    {
                        if (row.DrillDownParameters == null)
                            row.DrillDownParameters = new Dictionary<string, object>();

                        row.DrillDownParameters = field.DrillDownParameters;
                    }
                }
            }
            #endregion

            #region Calc Col Data
            foreach (StiCrossColumn field in colFields)
            {
                if (field.Name != EmptyField)
                {
                    InvokeEvents(field);
                    col.HyperlinkValue = field.HyperlinkValue;
                    col.ToolTipValue = field.ToolTipValue;
                    col.TagValue = field.TagValue;

                    if (field.DrillDownParameters != null)
                    {
                        if (col.DrillDownParameters == null)
                            col.DrillDownParameters = new Dictionary<string, object>();

                        col.DrillDownParameters = field.DrillDownParameters;
                    }
                }
            }
            #endregion
        }
        
        private int CopyRows(StiRowCollection rows, int left, int top, int level, out string parentGuid)
        {
            var rowCount = 0;
            parentGuid = Guid.NewGuid().ToString();

            foreach (StiRow row in rows)
            {
                var childrenGuid = "";
                var curRowCount = CopyRows(row.Rows, left + 1, top, level + 1, out childrenGuid);
                var field = RowFields[level] as StiCrossField;                
                rowCount += curRowCount;

                #region Calculate Total Width
                var totalWidth = 1;
                if (StiCrossTabV2Builder.IsCollapsed(field as StiCrossHeader, level, row.Value as string))
                {
                    var selectedRow = row;

                    while (true)
                    {
                        if (selectedRow.Rows.Count == 0)
                            break;

                        totalWidth++;
                        selectedRow = selectedRow.Rows[0];
                    }
                }
                #endregion

                var rect = new Rectangle(left, top, totalWidth, curRowCount);                
                if (row.IsTotal)
                {
                    rect.Width = rowsHeaderWidth + widthCorrection - rect.X - ((IsSummarySubHeadersPresent && summaryDirection == StiSummaryDirection.UpToDown) ? 1 : 0);
                    field = ((StiCrossHeader)field).Total;
                }

                var str = field.TextFormat.Format(row.DisplayValue);

                InvokeEvents(field, str);

                var isNumeric = field is StiCrossRow && !(field.TextFormat is StiGeneralFormatService);

                #region SetCells
                if (field.MergeHeaders || Report.IsDesigning)
                {
                    SetCell(rect.X, rect.Y, rect.Width, rect.Height, Rows.ToList().IndexOf(row), str, str, field, isNumeric,
                        row.HyperlinkValue, row.ToolTipValue, row.TagValue, row.DrillDownParameters, level, parentGuid, childrenGuid, field is StiCrossRow ? (field as StiCrossRow).KeepMergedCellsTogether : false, row.IsTotal ? StiCellType.HeaderRowTotal : StiCellType.HeaderRow);
                }
                else
                {
                    for (var index = 0; index < rect.Height; index++)
                    {
                        SetCell(rect.X, rect.Y + index, rect.Width, 1, Rows.ToList().IndexOf(row), str, str, field, isNumeric,
                            row.HyperlinkValue, row.ToolTipValue, row.TagValue, row.DrillDownParameters, level, parentGuid, childrenGuid, field is StiCrossRow ? (field as StiCrossRow).KeepMergedCellsTogether : false, row.IsTotal ? StiCellType.HeaderRowTotal : StiCellType.HeaderRow);
                    }
                }
                #endregion

                top += rect.Height;
            }
            return Math.Max(rowCount, oneCellHeight);
        }

        private int CopyCols(StiColumnCollection cols, int left, int top, int level, out string parentGuid)
        {
            var colCount = 0;
            parentGuid = Guid.NewGuid().ToString();
            foreach (StiColumn col in cols)
            {
                var guid = "";
                var curColCount = CopyCols(col.Cols, left, top + 1, level + 1, out guid);
                var field = ColFields[level] as StiCrossField;
                colCount += curColCount;

                #region Calculate Total Height
                var totalHeight = 1;
                if (StiCrossTabV2Builder.IsCollapsed(field as StiCrossHeader, level, col.Value as string))
                {
                    var selectedCol = col;

                    while (true)
                    {
                        if (selectedCol.Cols.Count == 0)
                            break;

                        totalHeight++;
                        selectedCol = selectedCol.Cols[0];
                    }
                }
                #endregion

                var rect = new Rectangle(left, top, curColCount, totalHeight);
                if (col.IsTotal)
                {
                    rect.Height = colsHeaderHeight + heightCorrection - rect.Y - ((IsSummarySubHeadersPresent && this.summaryDirection == StiSummaryDirection.LeftToRight) ? 1 : 0);
                    field = ((StiCrossHeader)field).Total;
                }

                var str = field.TextFormat.Format(col.DisplayValue);

                InvokeEvents(field, str);

                var isNumeric = field is StiCrossColumn && !(field.TextFormat is StiGeneralFormatService);

                #region SetCells
                if (field.MergeHeaders || Report.IsDesigning)
                {
                    SetCell(rect.X, rect.Y, rect.Width, rect.Height, Cols.ToList().IndexOf(col), str, str, field, isNumeric,
                        col.HyperlinkValue, col.ToolTipValue, col.TagValue, col.DrillDownParameters, level, parentGuid, guid, false, col.IsTotal ? StiCellType.HeaderColTotal : StiCellType.HeaderCol);
                }
                else
                {
                    for (var index = 0; index < rect.Width; index++)
                    {
                        SetCell(rect.X + index, rect.Y, 1, rect.Height, Cols.ToList().IndexOf(col), str, str, field, isNumeric,
                        col.HyperlinkValue, col.ToolTipValue, col.TagValue, col.DrillDownParameters, level, parentGuid, guid, false, col.IsTotal ? StiCellType.HeaderColTotal : StiCellType.HeaderCol);
                    }
                }
                #endregion

                left += rect.Width;
            }

            return Math.Max(colCount, oneCellWidth);
        }
        
        private void CopySummaries(int left, int top, object emptyValue)
        {
            var rowsArray = GetRowsArray();
            var colsArray = GetColsArray();

            #region Prepare totals calculation
            var sumsOnCell = new ArrayList[colsHeaderHeight * oneCellSize];
            var argsOnCell = new ArrayList[colsHeaderHeight * oneCellSize];

            for (var sumIndex = 0; sumIndex < sumsOnCell.Length; sumIndex++)
            {
                sumsOnCell[sumIndex] = new ArrayList();
                argsOnCell[sumIndex] = new ArrayList();
            }
            #endregion

            var rowIndex = 0;
            var colIndex = 0;
            foreach (var row in rowsArray)
            {
                #region New Row - Clear totals calculatation
                foreach (var list in sumsOnCell)list.Clear();
                foreach (var table in argsOnCell) table.Clear();
                #endregion

                colIndex = 0;
                foreach (var col in colsArray)
                {
                    if (col.IsTotal)
                    {
                        #region IsTotal
                        for (var sumIndex = 0; sumIndex < oneCellSize; sumIndex++)
                        {
                            var sumField = SumFields[sumIndex] as StiCrossSummary;
                            var list = sumsOnCell[col.Level * oneCellSize + sumIndex];
                            var argList = argsOnCell[col.Level * oneCellSize + sumIndex];
                            Hashtable argValues = new Hashtable();

                            var value = GetSummary(list, argList, sumIndex, true, argValues);
                            list.Clear();
                            argList.Clear();

                            var x = summaryDirection == StiSummaryDirection.UpToDown 
                                ? left + colIndex 
                                : left + colIndex * oneCellSize + sumIndex;

                            var y = summaryDirection == StiSummaryDirection.UpToDown 
                                ? top + rowIndex * oneCellSize + sumIndex 
                                : top + rowIndex;

                            var summary = SummaryContainer.GetSummary(col, row, true);
                            
                            summary.Sums[sumIndex].Add(value);
                            summary.Arguments[sumIndex] = SummaryContainer.GetArguments(argValues);

                            InvokeEvents(sumField);
                            var cell = SetCellValue(x, y, value, sumIndex, col.Level, StiFieldType.Column,
                                sumField.HyperlinkValue,
                                sumField.ToolTipValue,
                                sumField.TagValue,
                                sumField.DrillDownParameters);

                            cell.SummaryIndex = sumIndex;
                            cell.IsCrossSummary = true;

                            if (sumField.Summary == StiSummaryType.Image || sumField.Summary == StiSummaryType.None)
                            {
                                cell.Value = null;
                                cell.Text = string.Empty;
                            }

                            CalculateColumnPercentages(cell, x, y);
                          
                            if (sumField.UseStyleOfSummaryInColumnTotal)
                                SetCellField(x, y, sumField.Clone() as StiCrossField);
                        }
                        #endregion
                    }
                    else
                    {
                        #region Create cell
                        var summary = SummaryContainer.GetSummary(col, row, false);

                        var sums = CopySummary(summary, left, top, colIndex, rowIndex, true, false, emptyValue);
                        var args = CopyArguments(summary);

                        #region Calc totals
                        for (var totalIndex = 0; totalIndex < colsHeaderHeight; totalIndex++)
                        for (var sumIndex = 0; sumIndex < oneCellSize; sumIndex++)
                        {
                            var list = sumsOnCell[totalIndex * oneCellSize + sumIndex];
                            list.Add(sums[sumIndex]);

                            argsOnCell[totalIndex * oneCellSize + sumIndex].Add(args[sumIndex]);
                        }
                        #endregion
                        #endregion
                    }
                    colIndex++;
                }

                rowIndex++;
            }

            #region Vert totals calculation
            #region Prepare totals calculation
            sumsOnCell = new ArrayList[rowsHeaderWidth * oneCellSize];
            argsOnCell = new ArrayList[rowsHeaderWidth * oneCellSize];
            for (var totalIndex = 0; totalIndex < sumsOnCell.Length; totalIndex++)
            {
                sumsOnCell[totalIndex] = new ArrayList();
                argsOnCell[totalIndex] = new ArrayList();
            }
            #endregion

            colIndex = 0;
            foreach (var col in colsArray)
            {
                #region New Col - Clear totals calculatation
                for (var totalIndex = 0; totalIndex < rowsHeaderWidth * oneCellSize; totalIndex++)
                {
                    sumsOnCell[totalIndex].Clear();
                    argsOnCell[totalIndex].Clear();
                }
                #endregion

                rowIndex = 0;
                foreach (var row in rowsArray)
                {
                    if (row.IsTotal)
                    {
                        #region IsTotal
                        for (var sumIndex = 0; sumIndex < oneCellSize; sumIndex++)
                        {
                            var sumField = SumFields[sumIndex] as StiCrossSummary;
                            var list = sumsOnCell[row.Level * oneCellSize + sumIndex];
                            var argList = argsOnCell[row.Level * oneCellSize + sumIndex];

                            var value = GetSummary(list, argList, sumIndex, true, new Hashtable());
                            list.Clear();
                            argList.Clear();

                            var x = summaryDirection == StiSummaryDirection.UpToDown
                                ? left + colIndex
                                : left + colIndex * oneCellSize + sumIndex;

                            var y = summaryDirection == StiSummaryDirection.UpToDown
                                ? top + rowIndex * oneCellSize + sumIndex
                                : top + rowIndex;

                            InvokeEvents(sumField);
                            var cell = SetCellValue(x, y, value, sumIndex, row.Level, StiFieldType.Row,
                                sumField.HyperlinkValue,
                                sumField.ToolTipValue,
                                sumField.TagValue,
                                sumField.DrillDownParameters);

                            cell.SummaryIndex = sumIndex;                     

                            if (sumField.Summary == StiSummaryType.Image || sumField.Summary == StiSummaryType.None)
                            {
                                cell.Value = null;
                                cell.Text = string.Empty;
                            }

                            CalculateRowPercentages(cell, x, y);

                            if (sumField.UseStyleOfSummaryInRowTotal)
                                SetCellField(x, y, sumField.Clone() as StiCrossField);
                        }
                        #endregion
                    }
                    else
                    {
                        #region Create cell
                        var summary = SummaryContainer.GetSummary(col, row, false);
                        var sums = CopySummary(summary, left, top, colIndex, rowIndex, false, col.IsTotal, emptyValue);
                        var args = CopyArguments(summary);

                        #region Calc totals
                        for (var totalIndex = 0; totalIndex < rowsHeaderWidth; totalIndex++)
                        for (var sumIndex = 0; sumIndex < oneCellSize; sumIndex++)
                        {
                            var list = sumsOnCell[totalIndex * oneCellSize + sumIndex];
                            list.Add(sums[sumIndex]);

                            argsOnCell[totalIndex * oneCellSize + sumIndex].Add(args[sumIndex]);
                        }
                        #endregion

                        #endregion
                    }
                    rowIndex++;
                }

                colIndex++;
            }
            #endregion
        }

        private void CalculateColumnPercentages(StiCell cell, int x, int y)
        {
            var showPercents = false;
            var valueTotal = cell.IsNumeric && cell.Value != null ? (decimal) cell.Value : 0m;

            for (var index = x - 1; index >= 0; index--)
            {
                var cellSummary = Cells[index][y];
                var crossSummary = cellSummary.Field as StiCrossSummary;
                if (crossSummary == null) break;

                var value = cellSummary.IsNumeric ? (decimal) cellSummary.Value : 0;
                value = valueTotal != 0 ? Math.Round(value / valueTotal * 100, 2) : 0;

                if (cellSummary.IsNumeric && crossSummary.ShowPercents)
                {
                    cellSummary.Value = value;
                    cellSummary.Text = $"{value}%";
                    showPercents = true;
                }
            }

            if (showPercents && cell.Field is StiCrossTotal) cell.Text = "100%";
        }

        private void CalculateRowPercentages(StiCell cell, int x, int y)
        {
            var showPercents = false;
            var valueTotal = cell.IsNumeric && cell.Value != null ? (decimal)cell.Value : 0m;

            for (var index = y - 1; index >= 0; index--)
            {
                var cellSummary = Cells[x][index];
                var crossSummary = cellSummary.Field as StiCrossSummary;
                if (crossSummary == null) break;

                var value = cellSummary.IsNumeric ? (decimal)cellSummary.Value : 0;
                value = valueTotal != 0 ? Math.Round(value / valueTotal * 100, 2) : 0;

                if (cellSummary.IsNumeric && crossSummary.ShowPercents)
                {
                    cellSummary.Value = value;
                    cellSummary.Text = $"{value}%";
                    showPercents = true;
                }
            }

            if (showPercents && cell.Field is StiCrossTotal) cell.Text = "100%";
        }

        private object[] CopySummary(StiSummary summary, int left, int top, int colIndex, int rowIndex, bool setValue, bool grandTotal, object emptyValue)
        {
            var sumsOnCell = new object[oneCellSize];

            for (var calcIndex = 0; calcIndex < oneCellSize; calcIndex++)
            {
                var value = emptyValue;

                if (summary != null)
                {
                    value = GetSummary(summary, calcIndex, grandTotal);
                    
                    sumsOnCell[calcIndex] = value;
                }

                var x = 0;
                var y = 0;

                if (summaryDirection == StiSummaryDirection.UpToDown)
                {
                    x = left + colIndex;
                    y = top + rowIndex * oneCellSize + calcIndex;
                }
                else
                {
                    x = left + colIndex * oneCellSize + calcIndex;
                    y = top + rowIndex;
                }

                var hyperlinkValue = summary?.HyperlinkValues[calcIndex];
                var toolTipValue = summary?.ToolTipValues[calcIndex];
                var tagValue = summary?.TagValues[calcIndex];

                var drillDownParameters = summary != null && summary.DrillDownParameters != null ? summary.DrillDownParameters[calcIndex] : null;

                if (!setValue || x >= Widths.Length || y >= Heights.Length) continue;

                var cell = SetCellValue(x, y, value, calcIndex, 0, StiFieldType.Cell, hyperlinkValue, toolTipValue, tagValue, drillDownParameters);
                cell.SummaryIndex = calcIndex;
            }
            return sumsOnCell;
        }

        private Hashtable[] CopyArguments(StiSummary summary)
        {
            var argOnCell = new Hashtable[oneCellSize];
            for (var calcIndex = 0; calcIndex < oneCellSize; calcIndex++)
            {
                if (summary != null)
                {
                    var arguments = summary.Arguments[calcIndex];
                    var values = new Hashtable();
                    foreach (string key in arguments.Keys)
                    {
                        var sums = arguments[key] as ArrayList;
                        values[key] = GetSummaryResult(sums, calcIndex, false);
                    }
                    argOnCell[calcIndex] = values;
                }                
            }
            return argOnCell;
        }

        private StiCell SetCellValue(int x, int y, object value, int calcIndex, int level,
            StiFieldType fieldType, object hyperlinkValue, object toolTipValue, object tagValue,
            Dictionary<string, object> drillDownParameters)
        {
            switch (fieldType)
            {
                case StiFieldType.Column:
                    var field = GetColumnTotalCell(level, calcIndex);
                    var str = ConvertValueToString(value, field);
                    
                    var cell = SetCell(x, y, 1, 1, str, value, field, value is decimal, hyperlinkValue, toolTipValue, tagValue, drillDownParameters);
                    CheckNegativeColor(value, field, cell);
                    return cell;

                case StiFieldType.Row:
                    var field2 = GetRowTotalCell(level, calcIndex);
                    var str2 = ConvertValueToString(value, field2);
                    
                    var cell2 = SetCell(x, y, 1, 1, str2, value, field2, value is decimal, hyperlinkValue, toolTipValue, tagValue, drillDownParameters);
                    CheckNegativeColor(value, field2, cell2);
                    return cell2;

                case StiFieldType.Cell:
                    var field3 = SumFields[calcIndex] as StiCrossField;
                    var str3 = ConvertValueToString(value, field3);
                    
                    var cell3 = SetCell(x, y, 1, 1, str3, value, field3, value is decimal, hyperlinkValue, toolTipValue, tagValue, drillDownParameters);
                    CheckNegativeColor(value, field3, cell3);
                    return cell3;
            }
            return null;
        }

        private static void CheckNegativeColor(object value, StiCrossField field3, StiCell cell)
        {
            if (StiNegativeColorChecker.IsNegativeInRed(field3.TextFormat))
            {
                var valueDecimal = StiValueHelper.TryToNullableDecimal(value);
                cell.IsNegativeColor = valueDecimal != null && valueDecimal.Value < 0;
            }
        }

        private object GetSummary(StiSummary summary, int sumIndex, bool grandTotal)
        {
            var sumField = SumFields[sumIndex] as StiCrossSummary;

            object value = null;

            if (summary.Arguments[sumIndex].Count == 0)
            {
                var sums = new ArrayList();
                foreach (var val in summary.Sums[sumIndex])
                {
                    sums.Add(val);
                }
                value = GetSummaryResult(sums, sumIndex, grandTotal);
            }
            else
            {
                try
                {
                    Hashtable argValues = new Hashtable();
                    Hashtable arguments = summary.Arguments[sumIndex];

                    foreach (var key in arguments.Keys)
                    {
                        argValues[key] = GetSummaryResult(arguments[key] as ArrayList, sumIndex, grandTotal);
                    }

                    StiParserParameters parameters = new StiParserParameters()
                    {
                        GetDataFieldValue = (object sender, StiParserGetDataFieldValueEventArgs e) =>
                        {
                            var path = String.Join(".", e.Path);
                            if (argValues.ContainsKey(path))
                            {
                                e.Processed = true;
                                e.Value = argValues[path];
                            }
                        }
                    };

                    value = StiParser.ParseTextValue(sumField.Value, sumField, sumField, parameters);
                }
                catch { }
            }            

            return IsHideZeros(value) && sumField.HideZeros ? CrossTab.EmptyValue : value;
        }

        private object GetSummary(ArrayList sums, ArrayList argList, int sumIndex, bool grandTotal, Hashtable argValues)
        {
            var sumField = SumFields[sumIndex] as StiCrossSummary;

            Hashtable argSums = new Hashtable();
            for (int i = 0; i < argList.Count; i++)
            {
                Hashtable table = (Hashtable)argList[i];
                if (table != null)
                {
                    foreach (string key in table.Keys)
                    {
                        if (!argSums.ContainsKey(key))
                        {
                            argSums[key] = new ArrayList();
                        }
                        (argSums[key] as ArrayList).Add(table[key]);
                    }
                }                
            }

            object value = null;
            if (argSums.Count > 0)// if expression cell
            {
                try
                {
                    
                    foreach (string key in argSums.Keys)
                    {
                        argValues[key] = GetSummaryResult(argSums[key] as ArrayList, sumIndex, grandTotal);
                    }

                    StiParserParameters parameters = new StiParserParameters()
                    {
                        GetDataFieldValue = (object sender, StiParserGetDataFieldValueEventArgs e) =>
                        {
                            var path = String.Join(".", e.Path);
                            if (argValues.ContainsKey(path))
                            {
                                e.Processed = true;
                                e.Value = argValues[path];
                            }
                        }
                    };

                    value = StiParser.ParseTextValue(sumField.Value, sumField, sumField, parameters);
                } catch { }               
            }
            else
            {
                value = GetSummaryResult(sums, sumIndex, grandTotal);
            }

            return IsHideZeros(value) && sumField.HideZeros ? CrossTab.EmptyValue : value;
        }

        private object GetSummaryResult(ArrayList sums, int totalIndex, bool grandTotal)
        {
            var crossSummary = SumFields[totalIndex] as StiCrossSummary;
            var summaryType = crossSummary.Summary;
            var summaryValues = crossSummary.SummaryValues;

            if (grandTotal && (summaryType == StiSummaryType.Count || summaryType == StiSummaryType.CountDistinct))summaryType = StiSummaryType.Sum;

            switch (summaryType)
            {
                case StiSummaryType.None:
                case StiSummaryType.Image:
                    return GetsSummaryResultImageAndNone(sums, totalIndex);

                case StiSummaryType.Sum:
                    return GetSummaryResultSum(sums);

                case StiSummaryType.Max:
                    return GetsSummaryResultMax(sums, crossSummary);

                case StiSummaryType.Min:
                    return GetsSummaryResultMin(sums, crossSummary);

                case StiSummaryType.Count:
                    return GetsSummaryResultCount(sums, summaryValues);

                case StiSummaryType.Median:
                    return Funcs.Median(sums); 

                case StiSummaryType.Average:
                    return GetSummaryResultAverage(sums, summaryValues);

                case StiSummaryType.CountDistinct:
                    return GetSummaryResultCountDistinct(sums, summaryValues);

                default:
                    return 0m;
            }
        }

        private object GetsSummaryResultImageAndNone(ArrayList sums, int totalIndex)
        {
            var indexToUse = 0;
            if (totalIndex != 0)
            {
                for (var index = totalIndex - 1; index >= 0; index--)
                {
                    var sumField = SumFields[index] as StiCrossSummary;
                    if (sumField.IndexOfSelectValue != -1)
                    {
                        indexToUse = sumField.IndexOfSelectValue;
                        if (indexToUse < sums.Count)
                            break;

                        indexToUse = 0;
                    }
                }
            }

            if (sums == null || sums.Count == 0) return string.Empty;
            return sums[indexToUse];
        }

        private object GetSummaryResultSum(ArrayList sums)
        {
            var value = 0m;

            var isNull = true;
            foreach (var val in sums)
            {
                if (val == null || DBNull.Value.Equals(val)) continue;
                isNull = false;
                if (DecimalHelper.CanConvertToDecimal(value)) value += DecimalHelper.ConvertToDecimal(val);
            }
            if (isNull) return this.CrossTab.EmptyValue;
            return value;
        }

        private object GetsSummaryResultMax(ArrayList sums, StiCrossSummary crossSummary)
        {
            var value = 0m;
            var isNull = true;

            #region MaxDate
            if (sums != null && sums.Count > 0 && IsDateTime(sums))
            {
                var valueProcessed = false;
                DateTime? maximum = null;
                var indexOfSelectedValue = -1;

                var index = 0;
                foreach (var val in sums)
                {
                    if (val == null || DBNull.Value.Equals(val)) continue;
                    isNull = false;

                    #region DateTime
                    if (val is DateTime)
                    {
                        var dateTimeValue = (DateTime)val;
                        if (valueProcessed)
                        {
                            if (maximum < dateTimeValue)
                            {
                                maximum = dateTimeValue;
                                indexOfSelectedValue = index;
                            }
                        }
                        else
                        {
                            maximum = dateTimeValue;
                            valueProcessed = true;
                            indexOfSelectedValue = 0;
                        }
                    }
                    #endregion

                    index++;
                }
                crossSummary.IndexOfSelectValue = indexOfSelectedValue;
                if (isNull) return this.CrossTab.EmptyValue;
                if (valueProcessed) return maximum;
                return null;
            }
            #endregion

            else
            {
                var indexOfSelectedValue = -1;

                var index = 0;
                foreach (var val in sums)
                {
                    if (val == null || DBNull.Value.Equals(val)) continue;
                    isNull = false;

                    if (index == 0)
                    {
                        if (DecimalHelper.CanConvertToDecimal(val))
                        {
                            value = DecimalHelper.ConvertToDecimal(val);
                            indexOfSelectedValue = index;
                        }
                    }
                    else
                    {
                        if (DecimalHelper.CanConvertToDecimal(val))
                        {
                            var val2 = DecimalHelper.ConvertToDecimal(val);
                            if (value < val2)
                            {
                                value = val2;
                                indexOfSelectedValue = index;
                            }
                        }
                    }
                    index++;
                }
                crossSummary.IndexOfSelectValue = indexOfSelectedValue;
                if (isNull) return this.CrossTab.EmptyValue;
                return value;
            }
        }

        private object GetsSummaryResultMin(ArrayList sums, StiCrossSummary crossSummary)
        {
            var value = 0m;
            var isNull = true;

            #region MinDate
            if (sums != null && sums.Count > 0 && IsDateTime(sums))
            {
                var valueProcessed = false;
                DateTime? minimum = null;
                var indexOfSelectedValue = -1;

                var index = 0;
                foreach (var val in sums)
                {
                    if (val == null || DBNull.Value.Equals(val)) continue;
                    isNull = false;

                    #region DateTime
                    if (val is DateTime)
                    {
                        var dateTimeValue = (DateTime)val;
                        if (valueProcessed)
                        {
                            if (minimum > dateTimeValue)
                            {
                                minimum = dateTimeValue;
                                indexOfSelectedValue = index;
                            }
                        }
                        else
                        {
                            minimum = dateTimeValue;
                            valueProcessed = true;
                            indexOfSelectedValue = 0;
                        }
                    }
                    #endregion

                    index++;
                }
                crossSummary.IndexOfSelectValue = indexOfSelectedValue;
                if (isNull) return this.CrossTab.EmptyValue;
                if (valueProcessed) return minimum;
                else return null;
            }
            #endregion

            else
            {
                var index2 = 0;
                var indexOfSelectedValue = -1;
                foreach (var val in sums)
                {
                    if (val == null || DBNull.Value.Equals(val)) continue;
                    isNull = false;

                    if (index2 == 0)
                    {
                        if (DecimalHelper.CanConvertToDecimal(val))
                        {
                            value = DecimalHelper.ConvertToDecimal(val);
                            indexOfSelectedValue = 0;
                        }
                    }
                    else
                    {
                        if (DecimalHelper.CanConvertToDecimal(val))
                        {
                            var val2 = DecimalHelper.ConvertToDecimal(val);
                            if (value > val2)
                            {
                                value = val2;
                                indexOfSelectedValue = index2;
                            }
                        }
                    }
                    index2++;
                }
                crossSummary.IndexOfSelectValue = indexOfSelectedValue;
                if (isNull) return this.CrossTab.EmptyValue;
                return value;
            }
        }

        private object GetsSummaryResultCount(ArrayList sums, StiSummaryValues summaryValues)
        {
            var isNull = true;

            if (sums == null || sums.Count == 0) return 0;

            decimal countOfValues = 0;
            foreach (var val in sums)
            {
                isNull = false;

                decimal valueDecimal = 0;
                if (DecimalHelper.CanConvertToDecimal(val))
                {
                    valueDecimal = DecimalHelper.ConvertToDecimal(val);
                }

                if (summaryValues == StiSummaryValues.AllValues) countOfValues++;
                if (summaryValues == StiSummaryValues.SkipNulls && val != null) countOfValues++;
                if (summaryValues == StiSummaryValues.SkipZerosAndNulls && val != null && valueDecimal != 0) countOfValues++;
            }
            if (isNull) return this.CrossTab.EmptyValue;
            return countOfValues;
        }
        
        private object GetSummaryResultAverage(ArrayList sums, StiSummaryValues summaryValues)
        {
            var value = 0m;
            var isNull = true;

            #region MinDate
            if (sums != null && sums.Count > 0 && IsDateTime(sums))
            {
                decimal avgValue = 0;
                long count2 = 0;

                foreach (var val in sums)
                {
                    if (val == null || DBNull.Value.Equals(val)) continue;
                    isNull = false;

                    #region DateTime
                    if (val is DateTime)
                    {
                        var dateTimeValue = (DateTime)val;
                        avgValue += dateTimeValue.Ticks;
                        count2++;
                    }
                    #endregion
                }
                if (count2 == 0) return null;
                if (isNull) return this.CrossTab.EmptyValue;
                return new DateTime((long)(avgValue / count2));
            }
            #endregion

            else
            {
                if (sums == null || sums.Count == 0) return 0;
                var countOfValues2 = 0;
                foreach (var val in sums)
                {
                    if (val == null || DBNull.Value.Equals(val)) continue;
                    isNull = false;

                    decimal valueDecimal = 0;
                    if (DecimalHelper.CanConvertToDecimal(val))
                    {
                        valueDecimal = DecimalHelper.ConvertToDecimal(val);
                        value += valueDecimal;
                    }

                    if (summaryValues == StiSummaryValues.AllValues) countOfValues2++;
                    if (summaryValues == StiSummaryValues.SkipNulls) countOfValues2++;
                    if (summaryValues == StiSummaryValues.SkipZerosAndNulls && valueDecimal != 0) countOfValues2++;
                }
                if (countOfValues2 == 0) return 0;
                if (isNull) return this.CrossTab.EmptyValue;
                return Math.Round(value / countOfValues2, 5);
            }
        }

        private object GetSummaryResultCountDistinct(ArrayList sums, StiSummaryValues summaryValues)
        {
            var isNull = true;

            if (sums == null || sums.Count == 0) return 0;
            var values = new Hashtable();
            decimal count = 0;
            var nullProcessed = false;

            foreach (var val in sums)
            {
                isNull = false;

                var valueDecimal = 0m;
                var converted = false;
                if (DecimalHelper.CanConvertToDecimal(val))
                {
                    valueDecimal = DecimalHelper.ConvertToDecimal(val);
                    converted = true;
                }

                if (val == null)
                {
                    if (summaryValues == StiSummaryValues.AllValues)
                    {
                        if (!nullProcessed)
                        {
                            nullProcessed = true;
                            count++;
                        }
                    }
                }
                else if (values[val] == null)
                {
                    if (valueDecimal == 0 && converted && summaryValues == StiSummaryValues.SkipZerosAndNulls) continue;

                    values[val] = val;
                    count++;
                }
            }
            if (isNull) return this.CrossTab.EmptyValue;
            return count;
        }
        
        private StiCrossField GetColumnTotalCell(int level, int calcIndex)
        {
            var column = columnsCell[level * oneCellSize + calcIndex] as StiCrossField;
            if (column == null)
            {
                var crossHeader = ColFields[level] as StiCrossHeader;

                column = crossHeader.Total.Clone(true) as StiCrossField;
                columnsCell[level * oneCellSize + calcIndex] = column;

                if (!string.IsNullOrEmpty(CrossTab?.CrossTabStyle) && Report?.Styles[CrossTab.CrossTabStyle] as StiCrossTabStyle != null && column.ComponentStyle != null)
                {
                    var style = Report.Styles[CrossTab.CrossTabStyle] as StiCrossTabStyle;
                    column.TextBrush = new StiSolidBrush(style.TotalCellColumnForeColor);
                    column.Brush = new StiSolidBrush(style.TotalCellColumnBackColor);
                }

                //Change in 2008.1.264 from 12 August 2008
                //Commented to allow right border
                //column.Border =			((StiCrossField)SumFields[sumIndex]).Border.Clone() as StiBorder;
                column.HorAlignment = ((StiCrossField)SumFields[calcIndex]).HorAlignment;
                column.VertAlignment = ((StiCrossField)SumFields[calcIndex]).VertAlignment;
                column.TextFormat = ((StiCrossField)SumFields[calcIndex]).TextFormat.Clone() as StiFormatService;
            }
            return column;
        }

        private StiCrossField GetRowTotalCell(int level, int calcIndex)
        {
            var row = rowsCell[level * oneCellSize + calcIndex] as StiCrossField;
            if (row == null)
            {
                row = ((StiCrossHeader)RowFields[level]).Total.Clone(true) as StiCrossField;
                rowsCell[level * oneCellSize + calcIndex] = row;

                if (!string.IsNullOrEmpty(CrossTab?.CrossTabStyle) && Report?.Styles[CrossTab.CrossTabStyle] as StiCrossTabStyle != null && row.ComponentStyle != null)
                {
                    var style = Report.Styles[CrossTab.CrossTabStyle] as StiCrossTabStyle;
                    row.TextBrush = new StiSolidBrush(style.TotalCellRowForeColor);
                    row.Brush = new StiSolidBrush(style.TotalCellRowBackColor);
                }

                //Change in 2008.1.264 from 12 August 2008
                //Commented to allow bottom border
                //row.Border =		((StiCrossField)SumFields[sumIndex]).Border.Clone() as StiBorder;
                row.HorAlignment = ((StiCrossField)SumFields[calcIndex]).HorAlignment;
                row.VertAlignment = ((StiCrossField)SumFields[calcIndex]).VertAlignment;
                row.TextFormat = ((StiCrossField)SumFields[calcIndex]).TextFormat.Clone() as StiFormatService;
            }
            return row;
        }
        
        private List<StiRow> GetRowsArray()
        {
            var array = new List<StiRow>();
            GetRowsArray(Rows, array);
            return array;
        }

        private void GetRowsArray(StiRowCollection rows, List<StiRow> array)
        {
            foreach (StiRow row in rows)
            {
                if (row.Rows.Count == 0) array.Add(row);
                GetRowsArray(row.Rows, array);
            }
        }
        
        private List<StiColumn> GetColsArray()
        {
            var array = new List<StiColumn>();
            GetColsArray(Cols, array);
            return array;
        }

        private void GetColsArray(StiColumnCollection cols, List<StiColumn> array)
        {
            foreach (StiColumn col in cols)
            {
                if (col.Cols.Count == 0) array.Add(col);
                GetColsArray(col.Cols, array);
            }
        }
        
        private int GetRowsHeaderWidth()
        {
            if (IsRowsEmpty) return 0;
            return GetRowsHeaderWidth(Rows);
        }

        private int GetRowsHeaderWidth(StiRowCollection rows)
        {
            var width = 0;
            while (rows.Count > 0)
            {
                width++;
                rows = rows[0].Rows;
            }
            return width;
        }
        
        private int GetColsHeaderHeight()
        {
            if (IsColsEmpty) return 0;
            return GetColsHeaderHeight(Cols);
        }

        private int GetColsHeaderHeight(StiColumnCollection cols)
        {
            var height = 0;
            while (cols.Count > 0)
            {
                height++;
                cols = cols[0].Cols;
            }
            return height;
        }
        
        private int GetRowsHeight()
        {
            return GetRowsHeight(Rows, GetRowsHeaderWidth(), 0);
        }

        private int GetRowsHeight(StiRowCollection rows, int maxLevel, int curLevel)
        {
            var rowHeight = 0;
            foreach (StiRow row in rows)
            {
                rowHeight += GetRowsHeight(row.Rows, maxLevel, curLevel + 1);

                if (curLevel == maxLevel - 1)
                {
                    rowHeight += oneCellHeight;
                }
            }
            return rowHeight;
        }
        
        private int GetColsWidth()
        {
            return GetColsWidth(Cols, GetColsHeaderHeight(), 0);
        }

        private int GetColsWidth(StiColumnCollection cols, int maxLevel, int curLevel)
        {
            var colWidth = 0;
            foreach (StiColumn col in cols)
            {
                colWidth += GetColsWidth(col.Cols, maxLevel, curLevel + 1);
                if (curLevel == maxLevel - 1)
                {
                    colWidth += oneCellWidth;
                }
            }
            return colWidth;
        }
        
        private void EnumerateRows(StiRowCollection rows, int level)
        {
            var crossRow = RowFields[level] as StiCrossRow;
            var separator = crossRow.EnumeratorSeparator;

            var index = 0;
            foreach (StiRow row in rows)
            {
                if (row.Rows != null && row.Rows.Count > 0) EnumerateRows(row.Rows, level + 1);

                if (crossRow.EnumeratorType != StiEnumeratorType.None)
                {
                    index++;
                    string number = null;

                    if (crossRow.EnumeratorType == StiEnumeratorType.Arabic) number = index.ToString();
                    else if (crossRow.EnumeratorType == StiEnumeratorType.ABC) number = Func.Convert.ToABC(index);
                    else number = Func.Convert.ToRoman(index);

                    row.DisplayValue = $"{number}{separator}{row.DisplayValue}";
                }
            }
        }

        private void EnumerateColumns(StiColumnCollection columns, int level)
        {
            var crossColumn = ColFields[level] as StiCrossColumn;
            var separator = crossColumn.EnumeratorSeparator;

            var index = 0;
            foreach (StiColumn column in columns)
            {
                if (column.Cols != null && column.Cols.Count > 0) EnumerateColumns(column.Cols, level + 1);

                if (crossColumn.EnumeratorType != StiEnumeratorType.None)
                {
                    index++;
                    string number = null;

                    if (crossColumn.EnumeratorType == StiEnumeratorType.Arabic) number = index.ToString();
                    else if (crossColumn.EnumeratorType == StiEnumeratorType.ABC) number = Func.Convert.ToABC(index);
                    else number = Func.Convert.ToRoman(index);

                    column.DisplayValue = $"{number}{separator}{column.DisplayValue}";
                }
            }
        }

        private void CheckSeparators()
        {
            var newColCount = 0;
            var findedCells = new Hashtable();
            var lengthOfColumn = new Hashtable();
            var needProcess = false;

            #region Check for separator |
            for (var colIndex = 0; colIndex < ColCount; colIndex++)
            {
                var separatorCount = 1;

                for (var rowIndex = 0; rowIndex < RowCount; rowIndex++)
                {
                    var cell = Cells[colIndex][rowIndex];
                    if (cell.ParentCell == cell && cell.Text.Contains("#|#") && cell.Width == 1)
                    {
                        var count = 1;
                        for (var index2 = 0; index2 < cell.Text.Length; index2++)
                        {
                            if (index2 < cell.Text.Length - 3 &&
                                cell.Text[index2] == '#' &&
                                cell.Text[index2 + 1] == '|' &&
                                cell.Text[index2 + 2] == '#')
                            {
                                findedCells[cell] = cell;
                                needProcess = true;
                                count++;
                            }
                        }
                        separatorCount = Math.Max(separatorCount, count);
                    }
                }

                lengthOfColumn[colIndex] = separatorCount;
                newColCount += separatorCount;
            }
            #endregion

            #region If we need to change amount of columns
            if (newColCount > ColCount || needProcess)
            {
                var oldColCount = ColCount;
                var oldCells = this.Cells;

                this.Init(newColCount, RowCount);
                var newColIndex = 0;
                for (var colIndex = 0; colIndex < oldColCount; colIndex++)
                {
                    var length = (int)lengthOfColumn[colIndex];

                    var separatorCount = 1;
                    for (var rowIndex = 0; rowIndex < RowCount; rowIndex++)
                    {
                        var oldCell = oldCells[colIndex][rowIndex];

                        #region If need process cell
                        if (findedCells[oldCell] != null && oldCell.ParentCell == oldCell)
                        {
                            var strs = oldCell.Text.Split(new[] { "#|#" }, StringSplitOptions.None);
                            separatorCount = Math.Max(separatorCount, strs.Length + 1);

                            #region oldCell.Width == 1
                            oldCell.Text = strs[0];
                            this.Cells[newColIndex][rowIndex] = oldCell;

                            for (var index2 = 1; index2 < strs.Length; index2++)
                            {
                                var copyCell = oldCell.Clone() as StiCell;
                                this.Cells[newColIndex + index2][rowIndex] = copyCell;
                                copyCell.Text = strs[index2];
                            }
                            #endregion
                        }
                        #endregion

                        else
                        {
                            oldCell.ParentCell.Width += length - 1;

                            this.Cells[newColIndex][rowIndex] = oldCell;
                        }
                    }
                    newColIndex += length;
                }
            }
            #endregion
        }

        public void Create(DataTable table, StiReport report, StiSummaryDirection direction, string emptyValue)
        {
            this.summaryDirection = direction;
            this.Report = report;

            oneCellSize = SumFields.Count;
            oneCellWidth = summaryDirection == StiSummaryDirection.LeftToRight ? oneCellSize : 1;
            oneCellHeight = summaryDirection == StiSummaryDirection.UpToDown ? oneCellSize : 1;
            SummaryContainer = new StiSummaryContainer(oneCellSize);

            Rows.Clear();
            Cols.Clear();

            CalculateDataTable(table);

            CalculateTopN();

            SortRows();
            SortCols();

            if (Rows.Count > 0) EnumerateRows(Rows, 0);
            if (Cols.Count > 0) EnumerateColumns(Cols, 0);

            CreateRowTotals();
            CreateColTotals();

            #region Init parameters
            colsHeaderHeight = GetColsHeaderHeight();
            rowsHeaderWidth = GetRowsHeaderWidth();

            if (IsSummarySubHeadersPresent)
            {
                if (direction == StiSummaryDirection.LeftToRight) colsHeaderHeight++;
                else rowsHeaderWidth++;
            }

            colsWidth = GetColsWidth();
            rowsHeight = GetRowsHeight();
            #endregion

            var totalCellsWidth = colsWidth + rowsHeaderWidth;
            var totalCellsHeight = rowsHeight + colsHeaderHeight;

            if (IsRowsEmpty) totalCellsHeight += oneCellHeight;
            if (IsColsEmpty) totalCellsWidth += oneCellWidth;

            widthCorrection = 0;
            heightCorrection = 0;

            if (IsColsEmpty && IsSummaryPresent) heightCorrection = 1;
            if (IsRowsEmpty && IsSummaryPresent) widthCorrection = 1;

            if (IsRowsEmpty && !IsColsEmpty && IsSummariesEmpty) widthCorrection = 1;

            if (IsTopLinePresent) heightCorrection++;

            base.Init(totalCellsWidth + widthCorrection, totalCellsHeight + heightCorrection);

            if (IsRowsEmpty && IsColsEmpty) return;

            #region Create Col Titles
            var index = 0;
            foreach (StiCrossColumn col in ColFields)
            {
                StiCrossTitle currTitle = null;
                foreach (StiCrossTitle title in ColTitleFields)
                {
                    if (title.TypeOfComponent == "Col:" + col.Name)
                    {
                        currTitle = title;
                        break;
                    }
                }

                if (currTitle != null && currTitle.IsEnabled)
                {
                    var cellHeight = 1;
                    var cellWidth = Math.Max(rowsHeaderWidth, 1);
                    InvokeEvents(currTitle);
                    SetCell(0, index++, cellWidth, cellHeight, currTitle.TextValue ?? currTitle.GetTextInternal(), emptyValue, currTitle, false,
                        currTitle.HyperlinkValue,
                        currTitle.ToolTipValue,
                        currTitle.TagValue,
                        currTitle.DrillDownParameters,
                        StiCellType.CornerCol);
                }
            }
            #endregion

            #region Create Row Titles
            var isRowTitlePresent = IsRowTitlePresent;
            index = 0;
            foreach (StiCrossRow row in RowFields)
            {
                StiCrossTitle currTitle = null;
                foreach (StiCrossTitle title in RowTitleFields)
                {
                    if (title.TypeOfComponent == "Row:" + row.Name)
                    {
                        currTitle = title;
                        break;
                    }
                }

                if (currTitle != null && isRowTitlePresent)
                {
                    var cellWidth = 1;
                    // 2014.05.29 Строка закомментирована для исправления ошибки "смещение заголовка столбца при нескольких суммари".
                    //if (IsSummarySubHeadersPresent && direction != StiSummaryDirection.LeftToRight) cellWidth++;

                    var cellHeight = Math.Max(colsHeaderHeight, 1);
                    var top = IsTopLinePresent ? 1 : 0;
                    InvokeEvents(currTitle);

					#region Fix-2016.07.25
					//Increase height when we have more than one sum fields and don't have columns
                    if (direction == StiSummaryDirection.LeftToRight && IsColsEmpty && SumFields.Count > 1)
                    {
                        cellHeight++;
                    }
					#endregion

                    SetCell(index++, top, cellWidth, cellHeight, currTitle.TextValue ?? currTitle.GetTextInternal(), emptyValue, currTitle, false,
                        currTitle.HyperlinkValue,
                        currTitle.ToolTipValue,
                        currTitle.TagValue,
                        currTitle.DrillDownParameters,
                        StiCellType.CornerRow);
                }
            }
            #endregion

            if (IsSummarySubHeadersPresent)
            {
                #region LeftToRight
                if (direction == StiSummaryDirection.LeftToRight)
                {
                    var top = colsHeaderHeight + heightCorrection - 1;
                    index = rowsHeaderWidth + widthCorrection;
                    
                    var summaryIndex = 0;
                    var columnCount = colsWidth == 0 ? this.SumHeaderFields.Count : colsWidth;

                    for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
                    {
                        var header = this.SumHeaderFields[summaryIndex] as StiCrossSummaryHeader;
                        if (!this.DesignTime)
                        {
                            if (header.Text != null && header.Text.Value != null)
                            {
                                if (header.OriginalValue == null)//store original value
                                    header.OriginalValue = header.Text.Value;
                                else header.Text.Value = (string)header.OriginalValue;
                            }

                            var args = new StiGetValueEventArgs();
                            header.InvokeGetValue(header, args);
                            header.SetTextInternal(args.Value);
                        }

                        InvokeEvents(header);
                        SetCell(index, top, 1, 1, header.TextValue ?? header.GetTextInternal(), emptyValue, header, false,
                            header.HyperlinkValue, header.ToolTipValue, header.TagValue, header.DrillDownParameters, StiCellType.HeaderColSummaryTotal);
                        
                        index++;
                        summaryIndex++;
                        if (summaryIndex == this.SumHeaderFields.Count) summaryIndex = 0;
                    }
                }
                #endregion

                #region UpToDown
                else
                {
                    var left = rowsHeaderWidth + widthCorrection - 1;
                    index = colsHeaderHeight + heightCorrection;
                    
                    var summaryIndex = 0;
                    var rowCount = rowsHeight == 0 ? this.SumHeaderFields.Count : rowsHeight;

                    for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
                    {
                        var header = this.SumHeaderFields[summaryIndex] as StiCrossSummaryHeader;
                        if (!this.DesignTime)
                        {
                            if (header.Text != null && header.Text.Value != null)
                            {
                                if (header.OriginalValue == null)//store original value
                                    header.OriginalValue = header.Text.Value;
                                else header.Text.Value = (string) header.OriginalValue;
                            }                   
                            
                            var args = new StiGetValueEventArgs();
                            header.InvokeGetValue(header, args);
                            header.SetTextInternal(args.Value);
                        }

                        InvokeEvents(header);
                        SetCell(left, index, 1, 1, header.TextValue ?? header.GetTextInternal(), emptyValue, header, false,
                            header.HyperlinkValue, header.ToolTipValue, header.TagValue, header.DrillDownParameters, StiCellType.HeaderRowSummaryTotal);

                        index++;
                        summaryIndex++;
                        if (summaryIndex == this.SumHeaderFields.Count) summaryIndex = 0;
                    }
                }
                #endregion
            }

            #region Create Top Line
            if (IsTopLinePresent)
            {
                if (IsLeftTopLinePresent)
                {
                    InvokeEvents(LeftCrossTitle);
                    SetCell(0, 0, rowsHeaderWidth, 1, LeftCrossTitle.TextValue ?? LeftCrossTitle.GetTextInternal(), emptyValue, LeftCrossTitle, false,
                        LeftCrossTitle.HyperlinkValue,
                        LeftCrossTitle.ToolTipValue,
                        LeftCrossTitle.TagValue,
                        LeftCrossTitle.DrillDownParameters,
                        StiCellType.LeftTopLine);
                }
                if (IsRightTopLinePresent && totalCellsWidth - rowsHeaderWidth > 0)
                {
                    InvokeEvents(RightCrossTitle);
                    SetCell(rowsHeaderWidth, 0, totalCellsWidth - rowsHeaderWidth, 1,
                        RightCrossTitle.TextValue ?? RightCrossTitle.GetTextInternal(), emptyValue, RightCrossTitle, false,
                        RightCrossTitle.HyperlinkValue,
                        RightCrossTitle.ToolTipValue,
                        RightCrossTitle.TagValue,
                        RightCrossTitle.DrillDownParameters,
                        StiCellType.RightTopLine);
                }
            }
            #endregion

            #region Create Summary Title
            if (SummaryCrossTitle != null)
            {
                if (IsColsEmpty)
                {
                    InvokeEvents(SummaryCrossTitle);
                    SetCell(rowsHeaderWidth, 0, oneCellWidth, 1, SummaryCrossTitle.TextValue ?? SummaryCrossTitle.GetTextInternal(), emptyValue,
                        SummaryCrossTitle, false,
                        SummaryCrossTitle.HyperlinkValue,
                        SummaryCrossTitle.ToolTipValue,
                        SummaryCrossTitle.TagValue,
                        SummaryCrossTitle.DrillDownParameters,
                        StiCellType.HeaderCol);
                }

                if (IsRowsEmpty)
                {
                    InvokeEvents(SummaryCrossTitle);


                    var y = direction == StiSummaryDirection.LeftToRight ? colsHeaderHeight - 1 : colsHeaderHeight;
                    var height = direction == StiSummaryDirection.LeftToRight ? oneCellHeight + 1 : oneCellHeight;
                    if (SummaryCrossTitle.Enabled)
                        SetCell(0, y, 1,
                        height, SummaryCrossTitle.TextValue ?? SummaryCrossTitle.GetTextInternal(), emptyValue, SummaryCrossTitle, false,
                        SummaryCrossTitle.HyperlinkValue,
                        SummaryCrossTitle.ToolTipValue,
                        SummaryCrossTitle.TagValue,
                        SummaryCrossTitle.DrillDownParameters,
                        StiCellType.HeaderRow);
                }
            }
            #endregion
            var guid = ""; 
            if (!IsRowsEmpty) CopyRows(Rows, 0, colsHeaderHeight + heightCorrection, 0, out guid);
            if (!IsColsEmpty) CopyCols(Cols, rowsHeaderWidth + widthCorrection, heightCorrection, 0, out guid);

            CopySummaries(rowsHeaderWidth + widthCorrection, colsHeaderHeight + heightCorrection, emptyValue);

            if (!DesignTime)
                CheckSeparators();

            #region Create list of cells
            Report.Cells.Clear();

            for (var colIndex = 0; colIndex < ColCount; colIndex++)
            {
                for (var rowIndex = 0; rowIndex < RowCount; rowIndex++)
                {
                    var cell = Cells[colIndex][rowIndex];
                    if (cell.IsNumeric && cell.Value is decimal)
                        Report.Cells.SetCell(colIndex, rowIndex, (decimal)cell.Value);
                    else
                        Report.Cells.SetCell(colIndex, rowIndex, 0m);
                }
            }
            #endregion

            #region Post process cells
            for (var colIndex = 0; colIndex < ColCount; colIndex++)
            {
                for (var rowIndex = 0; rowIndex < RowCount; rowIndex++)
                {
                    var cell = Cells[colIndex][rowIndex];
                    //cell.Text = cell.Text + "(" + cell.CellType.ToString() + ")";

                    if (cell.ParentCell == cell)
                    {

                        #region Image
                        if (cell.Field is StiCrossSummary && ((StiCrossSummary)cell.Field).Summary == StiSummaryType.Image)
                        {
                            cell.IsImage = true;
                        }
                        #endregion

                        var e = new StiProcessCellEventArgs
                        {
                            Row = rowIndex,
                            Column = colIndex,
                            Cell = cell
                        };

                        var val = 0m;
                        if (cell.Value is decimal)
                            val = (decimal)cell.Value;

                        e.Text = cell.Text;
                        e.Value = val;

                        report.Cells.DistX = colIndex;
                        report.Cells.DistY = rowIndex;

                        if (cell.Field is StiCrossTotal)
                            cell.Field = cell.Field;

                        if (cell.Field != null)
                            cell.Field.InvokeProcessCell(e);


                        if (e.Value != val)
                        {
                            Report.Cells.SetCell(colIndex, rowIndex, e.Value);
                            cell.Value = e.Value;
                            cell.Text = cell.Field.TextFormat.Format(e.Value);
                        }
                        else
                        {
                            cell.Text = e.Text;
                        }

                        #region Parse numeric values for excel export
                        if (cell.IsNumeric && cell.Field != null)
                        {
                            if (cell.Field.HideZeros && IsHideZeros(cell.Value)) continue;

                            if (cell.Value != null)
                                cell.Field.ExcelDataValue = cell.Value.ToString();

                            if (((cell.Field.TextFormat as StiNumberFormatService)?.State & StiTextFormatState.Abbreviation) > 0) cell.Field.ExcelDataValue = null;
                        }
                        #endregion

                    }
                }
            }

            if (IsRowsEmpty && !IsColsEmpty && IsSummariesEmpty)//remove last unuseful row if only cols present in crosstab
            {
                var heights = Heights;
                Array.Resize(ref heights, Heights.Length - 1);
                Heights = heights;
                for (var i = 0; i < Cells.Length; i++)
                {
                    var cells = Cells[i];
                    Array.Resize(ref cells, cells.Length - 1);
                    Cells[i] = cells;
                }
            }
                
            #endregion
        }

        public bool Clear()
        {
            var needGCCollect = false;
            if ((Cells != null) && (Cells.Length > 1) && (Cells[0] != null))
            {
                var size = Cells.Length * Cells[0].Length;
                if (size > 50000)
                {
                    needGCCollect = true;
                }
            }

            Cells = null;

            return needGCCollect;
        }

        internal int GetCorrectedColumnsHeaderHeight()
        {
            return colsHeaderHeight + heightCorrection;
        }
        #endregion

        #region Methods.Helpers
        private bool IsHideZeros(object value)
        {
            return value != null && value.GetType().IsNumericType() && StiValueHelper.TryToDecimal(value) == 0m;
        }

        private bool IsDateTime(ArrayList list)
        {
            return list.Cast<object>().Any(s => s is DateTime);
        }

        private static string ConvertValueToString(object value, StiCrossField field)
        {
            if (value == null)
                return string.Empty;

            if (value is decimal && (decimal)value == 0m && field.HideZeros)
                return string.Empty;

            if (value is decimal)
            {
                if ((field.TextFormat is StiCustomFormatService) && !string.IsNullOrWhiteSpace(field.TextFormat.StringFormat) && field.TextFormat.StringFormat.Contains("{"))
                {
                    var storeFormat = field.TextFormat.StringFormat;
                    try
                    {
                        field.TextFormat.StringFormat = global::System.Convert.ToString(StiParser.ParseTextValue(storeFormat, field));
                        return field.TextFormat.Format((decimal)value);
                    }
                    finally
                    {
                        field.TextFormat.StringFormat = storeFormat;
                    }
                }
                return field.TextFormat.Format((decimal)value);
            }

            if (value is DateTime)
            {
                if ((field.TextFormat is StiCustomFormatService) && !string.IsNullOrWhiteSpace(field.TextFormat.StringFormat) && field.TextFormat.StringFormat.Contains("{"))
                {
                    var storeFormat = field.TextFormat.StringFormat;
                    try
                    {
                        field.TextFormat.StringFormat = global::System.Convert.ToString(StiParser.ParseTextValue(storeFormat, field));
                        return field.TextFormat.Format((DateTime)value);
                    }
                    finally
                    {
                        field.TextFormat.StringFormat = storeFormat;
                    }
                }
                return field.TextFormat.Format((DateTime)value);
            }

            return value.ToString();
        }
        #endregion

        #region Properties
        internal StiCrossTab CrossTab { get; set; }

        private bool IsSummaryPresent
        {
            get
            {
                if (Report.IsDesigning) return true;
                return SummaryCrossTitle != null && SummaryCrossTitle.IsEnabled;
            }
        }
        
        private bool IsRowTitlePresent
        {
            get
            {
                if (Report.IsDesigning) return true;
                foreach (StiCrossRow row in RowFields)
                {
                    if (!row.IsEnabled) return false;
                }
                return true;
            }
        }
        
        private bool IsTopLinePresent
        {
            get
            {
                if (IsColsEmpty) return false;
                if (LeftCrossTitle == null && RightCrossTitle == null) return false;
                if (Report.IsDesigning) return true;
                if (LeftCrossTitle.IsEnabled && RightCrossTitle.IsEnabled) return true;
                return false;
            }
        }

        internal bool IsTopCrossTitleVisible
        {
            get
            {
                return !IsRowsEmpty && IsCrossTitleEnabled;
            }
        }

        internal bool IsLeftCrossTitleVisible
        {
            get
            {
                return !IsTopCrossTitleVisible && IsCrossTitleEnabled;
            }
        }

        internal bool IsCrossTitleEnabled
        {
            get
            {
                return (LeftCrossTitle != null && LeftCrossTitle.Enabled &&
                        RightCrossTitle != null && RightCrossTitle.Enabled) ||
                       (SummaryCrossTitle != null && SummaryCrossTitle.Enabled);
            }
        }

        internal bool IsCrossTitlePrintOnAllPages
        {
            get
            {
                return
                    (LeftCrossTitle != null && LeftCrossTitle.PrintOnAllPages &&
                    RightCrossTitle != null && RightCrossTitle.PrintOnAllPages) ||
                    (SummaryCrossTitle != null && SummaryCrossTitle.PrintOnAllPages);
            }
        }

        private bool IsShowSummarySubHeaders
        {
            get
            {
                if (DesignTime) return true;

                foreach (StiCrossSummaryHeader header in this.SumHeaderFields)
                {
                    if (header.Enabled) return true;
                }
                return false;
            }
        }

        private bool IsSummarySubHeadersPresent
        {
            get
            {
                return
                    this.SumFields.Count > 1 &&
                    IsShowSummarySubHeaders &&
                    this.SumHeaderFields.Count > 1;
            }
        }

        private bool IsLeftTopLinePresent
        {
            get
            {
                if (!IsTopLinePresent) return false;
                if (LeftCrossTitle == null) return false;
                if (Report.IsDesigning) return true;
                return LeftCrossTitle.IsEnabled;
            }
        }

        private bool IsRightTopLinePresent
        {
            get
            {
                if (!IsTopLinePresent) return false;
                if (RightCrossTitle == null) return false;
                if (Report.IsDesigning) return true;
                return RightCrossTitle.IsEnabled;
            }
        }

        public bool IsRowsEmpty
        {
            get
            {
                return RowFields.Count == 1 && RowFields[0].Name == EmptyField;
            }
        }

        public bool IsColsEmpty
        {
            get
            {
                return ColFields.Count == 1 && ColFields[0].Name == EmptyField;
            }
        }

        public bool IsSummariesEmpty
        {
            get
            {
                return SumFields.Count == 1 && SumFields[0].Name == EmptyField;
            }
        }

        public StiRowCollection Rows { get; } = new StiRowCollection();
        
        public StiColumnCollection Cols { get; } = new StiColumnCollection();

        public StiComponentsCollection ColTitleFields { get; set; }
        
        public StiComponentsCollection RowTitleFields { get; set; }
        
        private StiComponentsCollection rowFields;
        public StiComponentsCollection RowFields
        {
            get
            {
                return rowFields;
            }
            set
            {
                if (value == null || value.Count == 0)
                {
                    if (value == null) value = new StiComponentsCollection();
                    else value.Clear();

                    value.Add(new StiCrossRow
                    {
                        Name = EmptyField
                    });
                }
                rowFields = value;
            }
        }
        
        private StiComponentsCollection colFields;
        public StiComponentsCollection ColFields
        {
            get
            {
                return colFields;
            }
            set
            {
                if (value == null || value.Count == 0)
                {
                    if (value == null) value = new StiComponentsCollection();
                    else value.Clear();

                    value.Add(new StiCrossColumn
                    {
                        Name = EmptyField
                    });
                }
                colFields = value;
            }
        }

        private StiComponentsCollection sumFields;
        public StiComponentsCollection SumFields
        {
            get
            {
                return sumFields;
            }
            set
            {
                if (value == null || value.Count == 0)
                {
                    if (value == null) value = new StiComponentsCollection();
                    else value.Clear();

                    value.Add(new StiCrossSummary
                    {
                        Name = EmptyField
                    });
                }
                sumFields = value;
            }
        }

        private StiComponentsCollection sumHeaderFields;
        public StiComponentsCollection SumHeaderFields
        {
            get
            {
                return sumHeaderFields;
            }
            set
            {
                if (value == null || value.Count == 0)
                {
                    if (value == null) value = new StiComponentsCollection();
                    else value.Clear();

                    value.Add(new StiCrossSummaryHeader
                    {
                        Name = EmptyField
                    });
                }
                sumHeaderFields = value;
            }
        }

        public StiSummaryContainer SummaryContainer { get; private set; }
        
        public StiCrossTitle LeftCrossTitle { get; set; }

        public StiCrossTitle RightCrossTitle { get; set; }
        
        public StiCrossTitle SummaryCrossTitle { get; set; }
        #endregion
    }
}
