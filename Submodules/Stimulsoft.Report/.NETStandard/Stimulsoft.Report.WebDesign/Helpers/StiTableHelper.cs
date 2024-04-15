#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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
using System.Collections;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Table;

namespace Stimulsoft.Report.Web
{
    internal class StiTableHelper
    {
        #region Fields

        private StiTable table;
        private double zoom;

        #endregion

        #region Methods

        public void ExecuteJSCommand(Hashtable parameters, Hashtable callbackResult)
        {
            Hashtable result = new Hashtable();
            string command = (string)parameters["command"];
            result["command"] = command;
            ArrayList cellsNames = (ArrayList)parameters["cells"];
            SetSelectedCurrentCells(cellsNames);
            

            switch (command)
            {
                #region ChangedDataSource                
                case "changeColumnsOrRowsCount":
                    {
                        if ((string)parameters["propertyName"] == "columnCount") 
                        {
                            table.ColumnCount = Convert.ToInt32((string)parameters["countValue"]);
                        }
                        else if ((string)parameters["propertyName"] == "rowCount") 
                        {
                            table.RowCount = Convert.ToInt32((string)parameters["countValue"]);
                        }
                        else if ((string)parameters["propertyName"] == "headerRowsCount")
                        {
                            table.HeaderRowsCount = Convert.ToInt32((string)parameters["countValue"]);
                        }
                        else if ((string)parameters["propertyName"] == "footerRowsCount")
                        {
                            table.FooterRowsCount = Convert.ToInt32((string)parameters["countValue"]);
                        }
                        break;
                    }
                case "convertTo":
                    {
                        ConvertTableCell(parameters, result);
                        break;
                    }
                case "joinCells":
                    {
                        JoinCells(GetSelectedCellsByNames(cellsNames));
                        break;
                    }
                case "unJoinCells":
                    {
                        UnJoinCells(GetSelectedCellsByNames(cellsNames));
                        break;
                    }
                case "insertColumnToLeft":
                    {   
                        table.InsertColumnToLeft(GetFirstIndexX(GetSelectedCellsByNames(cellsNames)));
                        break;
                    }
                case "insertColumnToRight":
                    {
                        table.InsertColumnToRight(GetLastIndexX(GetSelectedCellsByNames(cellsNames)));
                        break;
                    }
                case "deleteColumn":
                    {
                        table.DeleteColumns(GetFirstIndexX(GetSelectedCellsByNames(cellsNames)), GetLastIndexX(GetSelectedCellsByNames(cellsNames)));                        
                        break;
                    }
                case "insertRowAbove":
                    {
                        table.InsertRowAbove(GetFirstIndexY(GetSelectedCellsByNames(cellsNames)));
                        break;
                    }
                case "insertRowBelow":
                    {
                        table.InsertRowBelow(GetLastIndexY(GetSelectedCellsByNames(cellsNames)) + 1);
                        break;
                    }
                case "deleteRow":
                    {
                        table.DeleteRows(GetFirstIndexY(GetSelectedCellsByNames(cellsNames)), GetLastIndexY(GetSelectedCellsByNames(cellsNames)));
                        break;
                    }
                case "applyStyle":
                    {
                        var styleName = parameters["styleName"] as string;
                        var styleId = parameters["styleId"] as string;

                        if (!string.IsNullOrEmpty(styleName) && styleName != "[None]")
                        {
                            table.ComponentStyle = styleName;
                            table.TableStyleFX = null;
                        }
                        else if (!string.IsNullOrEmpty(styleId) && styleId != "[None]")
                        {
                            foreach (var style in StiOptions.Services.TableStyles)
                            {
                                if (style.StyleId.ToString() == styleId)
                                {
                                    table.ComponentStyle = string.Empty;
                                    table.TableStyleFX = style;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            table.ComponentStyle = string.Empty;
                            table.TableStyleFX = null;
                        }

                        break;
                    }
                #endregion
            }
            if ((string)parameters["command"] != "convertTo")
            {
                result["cells"] = GetTableCellsForJS();
                result["tableProperties"] = StiReportEdit.GetComponentMainProperties(table, zoom);
                if (command != "changeColumnsOrRowsCount") result["selectedCells"] = GetSelectedCellNames();
                if (command == "changeColumnsOrRowsCount" || command == "insertRowAbove" || command == "insertRowBelow" || command == "deleteRow")
                    result["rebuildProps"] = StiReportEdit.GetPropsRebuildPage(table.Report, table.Page);
            }    
        
            result["pageName"] = table.Page.Name;
            result["tableName"] = table.Name;
            callbackResult["result"] = result;
        }

        #region Helper Methods

        private int GetFirstIndexX(StiComponentsCollection selectedComponents)
        {
            var allX = new Stack<int>();
            for (int index1 = 0; index1 < selectedComponents.Count; index1++)
            {
                var selCell = selectedComponents[index1];
                if (((IStiTableCell)selCell).Join)
                {
                    int index = table.Components.IndexOf(selCell);
                    int row = (int)(index / table.ColumnCount);
                    allX.Push(index - (row * table.ColumnCount));
                    index = table.Components.IndexOf(((IStiTableCell)selCell).GetJoinComponentByIndex(0));
                    row = (int)(index / table.ColumnCount);
                    allX.Push(index - (row * table.ColumnCount));
                }
                else
                {
                    int index = table.Components.IndexOf(selCell);
                    int row = (int)(index / table.ColumnCount);
                    allX.Push(index - (row * table.ColumnCount));
                }
            }

            int firstIndexX = allX.Pop();
            while (allX.Count > 0)
            {
                int index = allX.Pop();
                if (firstIndexX > index)
                    firstIndexX = index;
            }

            return firstIndexX;
        }

        private int GetLastIndexX(StiComponentsCollection selectedComponents)
        {
            var allX = new Stack<int>();
            for (int index1 = 0; index1 < selectedComponents.Count; index1++)
            {
                int index = table.Components.IndexOf(selectedComponents[index1]);
                int row = (int)(index / table.ColumnCount);
                allX.Push(index - (row * table.ColumnCount));
            }

            int lastIndexX = allX.Pop();
            while (allX.Count > 0)
            {
                int index = allX.Pop();
                if (lastIndexX < index)
                    lastIndexX = index;
            }

            return lastIndexX;
        }

        private int GetFirstIndexY(StiComponentsCollection selectedComponents)
        {
            var allY = new Stack<int>();
            for (int index1 = 0; index1 < selectedComponents.Count; index1++)
            {
                var selCell = (StiTableCell)selectedComponents[index1];
                if (selCell.Join)
                {
                    int index = table.Components.IndexOf(selCell.GetJoinComponentByIndex(0));
                    allY.Push((int)(index / table.ColumnCount));
                }
                else
                {
                    int index = table.Components.IndexOf(selCell);
                    allY.Push((int)(index / table.ColumnCount));
                }
            }

            int firstIndexY = allY.Pop();
            while (allY.Count > 0)
            {
                int index = allY.Pop();
                if (firstIndexY > index)
                    firstIndexY = index;
            }

            return firstIndexY;
        }

        private int GetLastIndexY(StiComponentsCollection selectedComponents)
        {
            var allY = new Stack<int>();
            for (int index1 = 0; index1 < selectedComponents.Count; index1++)
            {
                var selCell = selectedComponents[index1];
                int index = table.Components.IndexOf(selCell);
                allY.Push((int)(index / table.ColumnCount));
            }

            int lastIndexY = allY.Pop();
            while (allY.Count > 0)
            {
                int index = allY.Pop();
                if (lastIndexY < index)
                    lastIndexY = index;
            }

            return lastIndexY;
        }

        private void JoinCells(StiComponentsCollection selectedComponents)
        {
            int[] allX = new int[selectedComponents.Count];
            int[] allY = new int[selectedComponents.Count];
            for (int index1 = 0; index1 < selectedComponents.Count; index1++)
            {
                int index = table.Components.IndexOf(selectedComponents[index1]);
                allY[index1] = (int)(index / table.ColumnCount);
                allX[index1] = index - (allY[index1] * table.ColumnCount);
            }

            int lastIndexX = allX[0];
            int lastIndexY = allY[0];
            for (int index1 = 1; index1 < allX.Length; index1++)
            {
                if (lastIndexX < allX[index1])
                    lastIndexX = allX[index1];
                if (lastIndexY < allY[index1])
                    lastIndexY = allY[index1];
            }

            (table.Components[lastIndexY * table.ColumnCount + lastIndexX] as IStiTableCell).Join = true;
            table.Components[lastIndexY * table.ColumnCount + lastIndexX].IsSelected = true;
        }

        private void UnJoinCells(StiComponentsCollection selectedComponents)
        {
            foreach (IStiTableCell cell in selectedComponents)
            {
                if (cell.Join)
                    cell.Join = false;
            }
        }

        private void SetSelectedCurrentCells(ArrayList cellsNames)
        {
            foreach (StiComponent cell in table.Components) 
                cell.IsSelected = false;
            
            foreach (string cellName in cellsNames)
            {
                StiComponent cell = table.Components[cellName];
                if (cell != null) cell.IsSelected = true;
            }
        }

        private ArrayList GetSelectedCellNames()
        {
            ArrayList cellNames = new ArrayList();
            foreach (StiComponent cell in table.Components)
                if (cell.IsSelected) cellNames.Add(cell.Name);

            return cellNames;
        }

        private StiComponentsCollection GetSelectedCellsByNames(ArrayList cellsNames)
        {
            StiComponentsCollection cells = new StiComponentsCollection();
            for (int i = 0; i < cellsNames.Count; i++)
            {
                StiComponent cell = table.Components[(string)cellsNames[i]];
                if (cell != null && cell.Enabled) cells.Add(cell);
            }

            return cells;
        }

        #endregion

        public static ArrayList GetTableStyles(StiReport report, bool withReportStyles = true)
        {
            var styles = new ArrayList();

            if (withReportStyles)
            {
                foreach (StiBaseStyle style in report.Styles)
                {
                    if (style is StiTableStyle)
                    {
                        styles.Add(StiStylesHelper.StyleItem(style));
                    }
                }
            }

            foreach (var tableStyle in StiOptions.Services.TableStyles)
            {
                var styleItem = StiStylesHelper.StyleItem(tableStyle);
                styleItem["styleId"] = tableStyle.StyleId.ToString();
                styles.Add(styleItem);
            }

            return styles;
        }

        public static ArrayList GetTableCellsProperties(StiTable table, double zoom)
        {
            ArrayList tableCells = new ArrayList();
            foreach (StiComponent cell in table.Components)
            {
                tableCells.Add(StiReportEdit.GetComponentMainProperties(cell, zoom));
            }

            return tableCells;
        }

        private ArrayList GetTableCellsForJS()
        {
            ArrayList tableCells = new ArrayList();            
            foreach (StiComponent cell in table.Components)
            {
                tableCells.Add(StiReportEdit.GetComponentMainProperties(cell, zoom));
            }

            return tableCells;
        }

        private void ConvertTableCell(Hashtable parameters, Hashtable result)
        {
            StiComponentsCollection cells = GetSelectedCellsByNames((ArrayList)parameters["cells"]);
            StiTablceCellType cellType = (StiTablceCellType)Enum.Parse(typeof(StiTablceCellType), (string)parameters["cellType"]);
            ArrayList resultCells = new ArrayList();

            foreach (StiComponent component in cells)
            {
                var cell = component as IStiTableCell;
                if (cell != null)
                {
                    int indexCell = table.Components.IndexOf(component);
                    cell.CellType = cellType;
                    StiComponent newCell = table.Components[indexCell];
                    resultCells.Add(StiReportEdit.GetComponentMainProperties(newCell, zoom));
                }
            }

            result["cells"] = resultCells;
        }

        #endregion

        #region Constructor

        public StiTableHelper(StiTable table, double zoom)
        {
            this.table = table;
            this.zoom = zoom;
        }

        #endregion
    }
}