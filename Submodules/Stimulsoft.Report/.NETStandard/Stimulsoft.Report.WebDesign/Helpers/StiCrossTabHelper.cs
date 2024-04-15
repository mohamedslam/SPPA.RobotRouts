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
using Stimulsoft.Report.CrossTab;
using System.Collections;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Components;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.CrossTab.Core;
using Stimulsoft.Base.Localization;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiCrossTabHelper
    {
        #region Fields
        private List<StiCrossTotal> rowTotals;
        private List<StiCrossTotal> colTotals;
        private List<StiCrossSummaryHeader> sumHeaders;
        private Hashtable createdTotals;
        private StiCrossTab crossTab;        
        private List<StiCrossField> columnsContainer;
        private List<StiCrossField> rowsContainer;
        private List<StiCrossField> summaryContainer;
        private StiDataSource selectedDataSource;
        private StiBusinessObject selectedBusinessObject;
        private double oldLeft;
        private double oldTop;
        #endregion

        #region Methods
        public void RestorePositions()
        {
            this.crossTab.Left = this.oldLeft;
            this.crossTab.Top = this.oldTop;
        }

        public void ExecuteJSCommand(Hashtable parameters, Hashtable callbackResult)
        {
            
            Hashtable updateResult = new Hashtable();
            updateResult["command"] = (string)parameters["command"];
            
            switch ((string)parameters["command"])
            {
                #region ChangedDataSource
                case "ChangedDataSource":
                    {
                        StiReportEdit.SetDataSourceProperty(this.crossTab, string.Empty);
                        StiReportEdit.SetBusinessObjectProperty(this.crossTab, string.Empty);

                        if ((string)parameters["dataSourceType"] == "DataSource")
                            StiReportEdit.SetDataSourceProperty(this.crossTab, (string)parameters["dataSourceName"]);
                        else if ((string)parameters["dataSourceType"] == "BusinessObject")
                            StiReportEdit.SetBusinessObjectProperty(this.crossTab, (string)parameters["dataSourceName"]);

                        this.crossTab.Components.Clear();
                        break;
                    }
                #endregion

                #region InsertItemToContainer
                case "InsertItemToContainer":
                    {
                        Hashtable itemObject = (Hashtable)parameters["itemObject"];
                        bool createNewField = false;
                        StiCrossField field = null;
                        List<StiCrossField> destinationContainer = GetContainerByName((string)parameters["destinationContainerName"]);

                        #region Get source item
                        if ((string)parameters["sourceContainerName"] != "datasources")
                        {
                            List<StiCrossField> sourceContainer = GetContainerByName((string)parameters["sourceContainerName"]);

                            int index = Convert.ToInt32(parameters["sourceIndex"]);
                            if (sourceContainer != null && index < sourceContainer.Count)
                            {
                                field = sourceContainer[index];
                            }

                            sourceContainer.Remove(field);
                        }
                        #endregion

                        #region Create new field if need
                        if (field == null)
                        {
                            createNewField = true;

                            if (destinationContainer == this.columnsContainer)
                            {
                                field = new StiCrossColumn();
                                field.Alias = (string)itemObject["alias"];
                                ((StiCrossHeader)field).DisplayValue.Value =
                                    ((StiCrossHeader)field).Value.Value = "{" + itemObject["columnFullName"] + "}";
                            }
                            else if (destinationContainer == this.rowsContainer)
                            {
                                field = new StiCrossRow();
                                field.Alias = (string)itemObject["alias"];
                                ((StiCrossHeader)field).DisplayValue.Value =
                                    ((StiCrossHeader)field).Value.Value = "{" + itemObject["columnFullName"] + "}";
                            }
                            else if (destinationContainer == this.summaryContainer)
                            {
                                field = new StiCrossSummary();
                                field.Alias = (string)itemObject["alias"];
                                ((StiCrossSummary)field).Value.Value = "{" + itemObject["columnFullName"] + "}";
                            }

                            this.crossTab.ApplyFieldStyle(field);
                        }
                        #endregion
                                                
                        #region Convert
                        if (!createNewField)
                        {
                            if (field is StiCrossSummary && destinationContainer == this.rowsContainer)
                            {
                                field = CopySummaryToRow(field as StiCrossSummary);
                            }
                            else if (field is StiCrossSummary && destinationContainer == this.columnsContainer)
                            {
                                field = CopySummaryToColumn(field as StiCrossSummary);
                            }
                            else if (field is StiCrossHeader)
                            {
                                if (destinationContainer == this.summaryContainer) field = CopyHeaderToSummary(field as StiCrossHeader);
                                if (destinationContainer == this.rowsContainer)
                                {
                                    var column = field as StiCrossColumn;
                                    var row = CopyColumnToRow(column);
                                    ConvertColumnTotal(column, row);

                                    field = row;
                                }
                                if (destinationContainer == this.columnsContainer)
                                {
                                    var row = field as StiCrossRow;
                                    var column = CopyRowToColumn(row);
                                    ConvertRowTotal(row, column);

                                    field = column;
                                }
                            }
                        }
                        #endregion

                        #region Insert field
                        if (parameters["destinationIndex"] != null)
                        {
                            destinationContainer.Insert(Convert.ToInt32(parameters["destinationIndex"]), field);
                        }
                        else
                        {
                            destinationContainer.Add(field);
                        }
                        #endregion

                        KillRightTitle();
                        UpdateCrossTab();

                        updateResult["fieldsProperties"] = GetFieldsPropertiesForJS();
                        updateResult["containerName"] = parameters["destinationContainerName"];
                        updateResult["selectedIndex"] = parameters["destinationIndex"] != null ? parameters["destinationIndex"] : destinationContainer.Count - 1;
                        break;
                    }
                #endregion

                #region RemoveItemFromContainer
                case "RemoveItemFromContainer":
                    {
                        List<StiCrossField> container = GetContainerByName((string)parameters["containerName"]);
                        int indexForRemove = Convert.ToInt32(parameters["indexForRemove"]);
                        if (container != null && indexForRemove < container.Count)
                        {
                            switch ((string)parameters["containerName"])
                            {
                                case "rows":
                                    {
                                        var row = container[indexForRemove] as StiCrossRow;
                                        container.RemoveAt(indexForRemove);
                                        var title = row.GetCrossRowTitle();
                                        var total = row.GetCrossRowTotal();

                                        if (crossTab.Components.Contains(row)) crossTab.Components.Remove(row);
                                        if (title != null && crossTab.Components.Contains(title)) crossTab.Components.Remove(title);
                                        if (total != null && crossTab.Components.Contains(total)) crossTab.Components.Remove(total);

                                        OrderRows();
                                        break;
                                    }
                                case "columns":
                                    {
                                        container.RemoveAt(indexForRemove);
                                        KillRightTitle();
                                        break;
                                    }
                                case "summary":
                                    {
                                        container.RemoveAt(indexForRemove);
                                        break;
                                    }
                            }
                        }

                        UpdateCrossTab();
                        updateResult["fieldsProperties"] = GetFieldsPropertiesForJS();
                        updateResult["containerName"] = parameters["containerName"];
                        updateResult["selectedIndex"] = parameters["selectIndexAfterRemoved"];
                        break;
                    }
                #endregion

                #region SwapColumnsAndRows
                case "SwapColumnsAndRows":
                    {
                        SwapColumnsAndRows();
                        updateResult["fieldsProperties"] = GetFieldsPropertiesForJS();
                        break;
                    }
                #endregion

                #region ItemMoveUp & ItemMoveDown
                case "ItemMoveUp":
                case "ItemMoveDown":
                    {
                        List<StiCrossField> container = GetContainerByName((string)parameters["containerName"]);
                        int indexForMoving = Convert.ToInt32(parameters["indexForMoving"]);
                        if (container != null)
                        {
                            if ((string)parameters["command"] == "ItemMoveUp")
                            {
                                if (indexForMoving > 0 && indexForMoving < container.Count)
                                {
                                    var selectedItem = container[indexForMoving];
                                    container.Remove(selectedItem);
                                    container.Insert(indexForMoving - 1, selectedItem);
                                }
                            }
                            else
                            {
                                if (indexForMoving < container.Count - 1)
                                {
                                    var selectedItem = container[indexForMoving];
                                    container.Remove(selectedItem);
                                    container.Insert(indexForMoving + 1, selectedItem);
                                }
                            }
                        }
                        UpdateCrossTab();
                        updateResult["fieldsProperties"] = GetFieldsPropertiesForJS();
                        updateResult["containerName"] = parameters["containerName"];
                        updateResult["selectedIndex"] = parameters["selectedIndexAfterMoving"];
                        break;
                    }
                #endregion

                #region UpdateProperty
                case "UpdateProperty":
                    {
                        StiComponent component = this.crossTab.Components[(string)parameters["componentName"]];
                        if (component != null) 
                        {
                            string propertyName = (string)parameters["propertyName"];
                            StiReportEdit.SetPropertyValue(component.Report, StiReportEdit.UpperFirstChar(propertyName), component, parameters["propertyValue"]);
                        }
                        UpdateCrossTab();
                        updateResult["fieldsProperties"] = GetFieldsPropertiesForJS();
                        updateResult["selectedComponentName"] = parameters["componentName"];
                        break;
                    }
                #endregion

                #region SetStyle
                case "SetStyle":
                    {
                        if (parameters["indexColorStyles"] != null)
                        {                            
                            this.crossTab.CrossTabStyle = string.Empty;
                            this.crossTab.CrossTabStyleIndex = Convert.ToInt32(parameters["indexColorStyles"]);
                        }
                        else 
                        {
                            this.crossTab.CrossTabStyleIndex = -1;
                            this.crossTab.CrossTabStyle = (string)parameters["styleName"];
                            this.crossTab.UpdateStyles();
                        }

                        this.crossTab.UpdateStyles();
                        UpdateCrossTab();
                        updateResult["fieldsProperties"] = GetFieldsPropertiesForJS();
                        updateResult["selectedComponentName"] = parameters["selectedComponentName"];
                        break;
                    }
                #endregion

                #region Change Summary Direction
                case "ChangeSummaryDirection":
                    {
                        ChangeSummaryDirection((string)parameters["summaryDirection"]);
                        updateResult["fieldsProperties"] = GetFieldsPropertiesForJS();
                        break;
                    }
                #endregion
            }

            callbackResult["updateResult"] = updateResult;
        }

        private List<StiCrossField> GetContainerByName(string containerName)
        {
            switch (containerName)
            {
                case "columns": return this.columnsContainer;
                case "rows": return this.rowsContainer;
                case "summary": return this.summaryContainer;
            }

            return null;
        }

        public ArrayList GetCrossTabResult()
        {
            var components = new ArrayList();

            foreach (StiComponent component in this.crossTab.Components)
            {
                if (component is StiCrossField)
                {
                    var mainProps = new Hashtable();
                    components.Add(mainProps);
                    mainProps["name"] = component.Name;
                    mainProps["typeComponent"] = "StiCrossField";
                    mainProps["svgContent"] = StiReportEdit.GetSvgContent(component, 1);
                    mainProps["componentRect"] = StiReportEdit.RectToStr(component.ClientRectangle);
                    mainProps["properties"] = GetCrossFieldJSProperies(component as StiCrossField);
                }
            }

            return components;
        }

        public Hashtable GetCrossFieldJSProperies(StiCrossField crossField)
        {                       
            Hashtable properties = new Hashtable();
            
            //Main Properties
            properties["parentCrossTabName"] = this.crossTab.Name;
            properties["typeCrossField"] = crossField.GetType().Name;
            properties["name"] = crossField.Name;
            properties["alias"] = crossField.Alias;            
            properties["brush"] = StiReportEdit.BrushToStr(crossField.Brush);
            properties["border"] = StiReportEdit.BorderToStr(crossField.Border);
            properties["componentStyle"] = crossField.ComponentStyle;
            properties["conditions"] = StiReportEdit.GetConditionsProperty(crossField);
            properties["horAlignment"] = crossField.HorAlignment.ToString();
            properties["vertAlignment"] = crossField.VertAlignment.ToString();
            properties["useParentStyles"] = crossField.UseParentStyles;
            properties["minSize"] = StiReportEdit.DoubleToStr(crossField.MinSize.Width) + ";" + StiReportEdit.DoubleToStr(crossField.MinSize.Height);
            properties["maxSize"] = StiReportEdit.DoubleToStr(crossField.MaxSize.Width) + ";" + StiReportEdit.DoubleToStr(crossField.MaxSize.Height);            
            properties["textOptions.trimming"] = crossField.TextOptions.Trimming.ToString();
            properties["textOptions.rightToLeft"] = crossField.TextOptions.RightToLeft;
            properties["textOptions.lineLimit"] = crossField.TextOptions.LineLimit;
            properties["textOptions.hotkeyPrefix"] = crossField.TextOptions.HotkeyPrefix.ToString();
            properties["textOptions.firstTabOffset"] = StiReportEdit.DoubleToStr(crossField.TextOptions.FirstTabOffset);
            properties["textOptions.distanceBetweenTabs"] = StiReportEdit.DoubleToStr(crossField.TextOptions.DistanceBetweenTabs);            
            properties["wordWrap"] = crossField.WordWrap;
            properties["margins"] = string.Format("{0};{1};{2};{3}", crossField.Margins.Left, crossField.Margins.Top, crossField.Margins.Right, crossField.Margins.Bottom);
            properties["font"] = StiReportEdit.FontToStr(crossField.Font);
            properties["angle"] = StiReportEdit.DoubleToStr(crossField.Angle);
            properties["textBrush"] = StiReportEdit.BrushToStr(crossField.TextBrush);
            properties["allowHtmlTags"] = crossField.AllowHtmlTags;
            properties["mergeHeaders"] = crossField.MergeHeaders;
            properties["interaction"] = StiReportEdit.GetInteractionProperty(crossField.Interaction);
            properties["enabled"] = crossField.Enabled;
            properties["textQuality"] = crossField.TextQuality.ToString();
            properties["pointerValue"] = crossField.Pointer != null && !string.IsNullOrEmpty(crossField.Pointer.Value) ? StiEncodingHelper.Encode(crossField.Pointer.Value) : string.Empty;

            if (crossField is StiCrossSummary || crossField is StiCrossSummaryHeader)
                properties["hideZeros"] = crossField.HideZeros;
            if (crossField is StiCrossSummary || crossField is StiCrossSummaryHeader || crossField is StiCrossColumn || crossField is StiCrossRow)
                properties["textFormat"] = StiTextFormatHelper.GetTextFormatItem(crossField.TextFormat);

            //Other Properties
            string[] propNames = { "text", "displayValue", "sortDirection", "sortType", "value", "enumeratorSeparator", "enumeratorType", "printOnAllPages", "showTotal",
                "showPercents", "summary", "summaryValues", "imageHorAlignment", "imageVertAlignment", "aspectRatio", "stretch", "useStyleOfSummaryInRowTotal", 
                "useStyleOfSummaryInColumnTotal"};
            
            foreach (string propName in propNames)
            {
                var value = StiReportEdit.GetPropertyValue(StiReportEdit.UpperFirstChar(propName), crossField, true);
                if (value != null) { properties[propName] = value; }
            }

            if (properties["value"] != null && properties["text"] != null)
                properties.Remove("text");

            return properties;
        }

        public Hashtable GetFieldsPropertiesForJS()
        {
            Hashtable properties = new Hashtable();
            ArrayList jsColumns = new ArrayList();
            ArrayList jsRows = new ArrayList();
            ArrayList jsSummary = new ArrayList();

            foreach (StiCrossField crossField in this.columnsContainer)
                jsColumns.Add(GetCrossFieldJSProperies(crossField));

            foreach (StiCrossField crossField in this.rowsContainer)
                jsRows.Add(GetCrossFieldJSProperies(crossField));

            foreach (StiCrossField crossField in this.summaryContainer)
                jsSummary.Add(GetCrossFieldJSProperies(crossField));

            properties["columns"] = jsColumns;
            properties["rows"] = jsRows;
            properties["summary"] = jsSummary;
            properties["components"] = GetCrossTabResult();
            properties["crossTabStyleIndex"] = this.crossTab.CrossTabStyleIndex;
            properties["crossTabStyle"] = this.crossTab.CrossTabStyle;

            return properties;
        }

        public static ArrayList GetColorStyles()
        {
            ArrayList styles = new ArrayList();
            int index = 0;

            foreach (var color in StiOptions.Designer.CrossTab.StyleColors)
            {
                var colorName = color.Name;
                switch (colorName)
                {
                    case "ff0bac45":
                        colorName = "Dark Pastel Green";
                        break;

                    case "ffb5a1dd":
                        colorName = "Perfume";
                        break;

                    case "ffffc000":
                        colorName = "Amber";
                        break;

                    case "ffed7d31":
                        colorName = "Sun";
                        break;

                    case "ff239fd9":
                        colorName = "Summer Sky";
                        break;
                }

                var style = new StiCrossTabStyle(colorName)
                {
                    Color = color
                };
                
                string locName = StiLocalization.Get("PropertyColor", colorName, false);
                if (locName != null) style.Name = locName;
                
                var styleItem = StiStylesHelper.StyleItem(style);
                styleItem["indexColorStyles"] = index;
                styles.Add(styleItem);
                
                index++;
            }

            return styles;
        }

        private void UpdateCrossTab()
        {
            rowTotals.Clear();
            colTotals.Clear();
            var rowComps = new List<StiCrossRow>();
            var colComps = new List<StiCrossColumn>();
            var usedComps = new Hashtable();

            #region Check Rows
            int rowIndex = 0;
            foreach (StiCrossRow row in this.rowsContainer)
            {
                row.Page = null;
                row.Name = this.crossTab.Name + "_Row" + (rowIndex + 1).ToString();
                row.Page = this.crossTab.Page;

                if (this.crossTab.Components.Contains(row)) this.crossTab.Components.Remove(row);
                this.crossTab.Components.Add(row);

                usedComps[row] = row;
                rowComps.Add(row);

                var total = row.Total;
                if (total == null)
                {
                    total = CreateRowTotal(row.Guid);
                    row.TotalGuid = total.Guid;
                    this.crossTab.Components.Add(total);
                }
                usedComps[total] = total;

                rowTotals.Add(total);

                rowIndex++;
            }
            #endregion

            #region Check Columns
            int columnIndex = 0;
            foreach (StiCrossColumn column in this.columnsContainer)
            {
                column.Page = null;
                column.Name = this.crossTab.Name + "_Column" + (columnIndex + 1).ToString();
                column.Page = this.crossTab.Page;

                if (this.crossTab.Components.Contains(column)) this.crossTab.Components.Remove(column);
                this.crossTab.Components.Add(column);

                usedComps[column] = column;
                colComps.Add(column);

                var total = column.Total;
                if (total == null)
                {
                    total = CreateColTotal(column.Guid);
                    column.TotalGuid = total.Guid;
                    this.crossTab.Components.Add(total);
                }
                usedComps[total] = total;

                colTotals.Add(total);

                columnIndex++;
            }
            #endregion

            #region Check Summary
            int summaryIndex = 1;
            foreach (StiCrossSummary summary in this.summaryContainer)
            {
                summary.Page = null;
                summary.Name = this.crossTab.Name + "_Sum" + summaryIndex.ToString();
                summary.Page = this.crossTab.Page;

                usedComps[summary] = summary;

                if (this.crossTab.Components.Contains(summary)) this.crossTab.Components.Remove(summary);
                this.crossTab.Components.Add(summary);

                summaryIndex++;
            }
            #endregion

            #region Check Summary Sub Headers
            summaryIndex = 1;

            #region Clear Sum Headers if Required
            if (this.summaryContainer.Count < 2)
            {
                foreach (var header in sumHeaders)
                {
                    if (this.crossTab.Components.Contains(header))
                    {
                        this.crossTab.Components.Remove(header);
                    }
                }
                sumHeaders.Clear();
            }
            #endregion

            #region Update Summary Sub Headers
            else
            {
                if (sumHeaders.Count > this.summaryContainer.Count)
                {
                    while (sumHeaders.Count > this.summaryContainer.Count)
                    {
                        var header = sumHeaders[sumHeaders.Count - 1];

                        if (this.crossTab.Components.Contains(header))
                        {
                            this.crossTab.Components.Remove(header);
                        }
                        sumHeaders.RemoveAt(sumHeaders.Count - 1);
                    }
                }
                else if (sumHeaders.Count < this.summaryContainer.Count)
                {
                    while (sumHeaders.Count < this.summaryContainer.Count)
                    {
                        var header = new StiCrossSummaryHeader
                        {
                            Text =
                            {
                                Value = (sumHeaders.Count + 1).ToString()
                            },
                            Page = null,
                            Name = this.crossTab.Name + "_SumHeader" + (sumHeaders.Count + 1).ToString()
                        };
                        header.Page = this.crossTab.Page;
                        this.crossTab.ApplyFieldStyle(header);

                        this.crossTab.Components.Add(header);
                        sumHeaders.Add(header);
                    }
                }
            }

            foreach (var header in sumHeaders)
            {
                usedComps[header] = header;
            }
            #endregion
            #endregion

            #region Check row totals
            int rowTotalIndex = 1;
            foreach (var rowTotal in rowTotals)
            {
                rowTotal.Page = null;
                rowTotal.Name = this.crossTab.Name + "_RowTotal" + (rowTotalIndex++).ToString();
                rowTotal.Page = this.crossTab.Page;
            }
            #endregion

            #region Check col totals
            int colTotalIndex = 1;
            foreach (var colTotal in colTotals)
            {
                colTotal.Page = null;
                colTotal.Name = this.crossTab.Name + "_ColTotal" + (colTotalIndex++).ToString();
                colTotal.Page = this.crossTab.Page;
            }
            #endregion

            #region Check Titles
            bool needTopTitle = this.columnsContainer.Count > 0 && this.rowsContainer.Count > 0;

            var titleCols = new List<StiCrossTitle>();
            var titleRows = new List<StiCrossTitle>();

            StiCrossTitle leftTitle = null;
            StiCrossTitle rightTitle = null;
            StiCrossTitle summaryTitle = null;

            foreach (StiCrossField field in this.crossTab.Components)
            {
                if (field is StiCrossTitle)
                {
                    var title = field as StiCrossTitle;

                    if (title.TypeOfComponent.StartsWith("Col:", StringComparison.InvariantCulture)) titleCols.Add(title);
                    if (title.TypeOfComponent.StartsWith("Row:", StringComparison.InvariantCulture)) titleRows.Add(title);
                    if (title.TypeOfComponent.StartsWith("LeftTitle", StringComparison.InvariantCulture)) leftTitle = title;
                    if (title.TypeOfComponent.StartsWith("RightTitle", StringComparison.InvariantCulture)) rightTitle = title;
                    if (title.TypeOfComponent.StartsWith("SummaryTitle", StringComparison.InvariantCulture)) summaryTitle = title;
                }
            }

            #region Check new title cols
            if (this.rowsContainer.Count == 0)
            {
                foreach (StiCrossColumn col in colComps)
                {
                    string type = "Col:" + col.Name;
                    bool finded = false;

                    foreach (StiCrossTitle title in titleCols)
                    {
                        if (title.TypeOfComponent == type)
                        {
                            titleCols.Remove(title);
                            finded = true;
                            usedComps[title] = title;
                            break;
                        }
                    }

                    if (!finded)
                    {
                        string name = string.Empty;
                        foreach (StiCrossField col_ in columnsContainer)
                        {
                            if (col_ == col) name = col_.Alias;
                        }

                        var title = new StiCrossTitle
                        {
                            Name = col.Name + "_Title",
                            Text = { Value = name },
                            TypeOfComponent = "Col:" + col.Name
                        };
                        this.crossTab.ApplyFieldStyle(title);
                        this.crossTab.Components.Add(title);
                        usedComps[title] = title;
                    }
                }
            }
            #endregion

            #region Check new title rows
            foreach (StiCrossRow row in rowComps)
            {
                string type = "Row:" + row.Name;
                bool finded = false;

                foreach (StiCrossTitle title in titleRows)
                {
                    if (title.TypeOfComponent != type) continue;

                    titleRows.Remove(title);
                    finded = true;
                    usedComps[title] = title;
                    break;
                }

                if (!finded)
                {
                    string name = string.Empty;
                    foreach (StiCrossField row_ in rowsContainer)
                    {
                        if (row_ == row) name = row_.Alias;
                    }

                    var title = new StiCrossTitle
                    {
                        Name = row.Name + "_Title",
                        Text = { Value = name },
                        TypeOfComponent = "Row:" + row.Name
                    };
                    this.crossTab.ApplyFieldStyle(title);
                    this.crossTab.Components.Add(title);
                    usedComps[title] = title;
                }
            }
            #endregion

            #region Left Title
            if (leftTitle == null)
            {
                if (needTopTitle)
                {
                    leftTitle = new StiCrossTitle
                    {
                        Name = this.crossTab.Name + "_LeftTitle"
                    };

                    if (this.selectedDataSource != null)
                        leftTitle.Text.Value = this.selectedDataSource.Name;
                    else if (this.selectedBusinessObject != null)
                        leftTitle.Text.Value = this.selectedBusinessObject.Name;
                    else
                        leftTitle.Text.Value = string.Empty;

                    leftTitle.Text.Value = this.selectedDataSource != null ? this.selectedDataSource.Name : string.Empty;
                    leftTitle.TypeOfComponent = "LeftTitle";
                    this.crossTab.ApplyFieldStyle(leftTitle);
                    this.crossTab.Components.Add(leftTitle);
                    usedComps[leftTitle] = leftTitle;
                }
            }
            else if (needTopTitle) usedComps[leftTitle] = leftTitle;
            #endregion

            #region Right Title
            if (rightTitle == null)
            {
                if (needTopTitle)
                {
                    string name = string.Empty;
                    bool first = true;
                    foreach (StiCrossField col_ in columnsContainer)
                    {
                        if (!first) name += ", ";
                        name += col_.Alias;
                        first = false;
                    }

                    rightTitle = new StiCrossTitle
                    {
                        Name = this.crossTab.Name + "_RightTitle",
                        Text = { Value = name },
                        TypeOfComponent = "RightTitle"
                    };
                    this.crossTab.ApplyFieldStyle(rightTitle);
                    this.crossTab.Components.Add(rightTitle);
                    usedComps[rightTitle] = rightTitle;
                }
            }
            else if (needTopTitle) usedComps[rightTitle] = rightTitle;
            #endregion

            #region Summary Title
            bool needSummaryTitle = (columnsContainer.Count == 0 || rowsContainer.Count == 0) &&
                summaryContainer.Count > 0;

            if (summaryTitle == null)
            {
                if (needSummaryTitle)
                {
                    string name = string.Empty;
                    bool first = true;
                    foreach (StiCrossField summary_ in summaryContainer)
                    {
                        if (!first) name += ", ";
                        name += summary_.Alias;
                        first = false;
                    }

                    summaryTitle = new StiCrossTitle
                    {
                        Name = this.crossTab.Name + "_SummaryTitle",
                        Text = { Value = name },
                        TypeOfComponent = "SummaryTitle"
                    };
                    this.crossTab.ApplyFieldStyle(summaryTitle);
                    this.crossTab.Components.Add(summaryTitle);
                    usedComps[summaryTitle] = summaryTitle;
                }
            }
            else if (needSummaryTitle) usedComps[summaryTitle] = summaryTitle;

            #endregion
            #endregion

            #region Remove unused
            int index = 0;
            while (index < this.crossTab.Components.Count)
            {
                StiComponent comp = this.crossTab.Components[index];
                if (usedComps[comp] == null)
                {
                    this.crossTab.Components.RemoveAt(index);
                }
                else index++;
            }
            #endregion

            #region Prepare Cross
            var rect = RectangleM.Empty;

            Stimulsoft.Report.CrossTab.StiCrossTabHelper.BuildCross(this.crossTab, true);

            foreach (StiCrossField field in this.crossTab.Components)
            {
                if (this.crossTab.CrossTabInfo.Cross.Fields[field] is Point)
                {
                    var pos = (Point)this.crossTab.CrossTabInfo.Cross.Fields[field];
                    this.crossTab.CrossTabInfo.Cross.SetTextOfCell(pos.X, pos.Y, field.CellText);
                }
            }

            this.crossTab.CrossTabInfo.Cross.MaxWidth = (decimal)this.crossTab.Page.Width;
            this.crossTab.CrossTabInfo.Cross.MaxHeight = (decimal)this.crossTab.Page.Height;

            this.crossTab.CrossTabInfo.Cross.DoAutoSize();

            var hidedTotals = new Hashtable();
            foreach (StiCrossField field in this.crossTab.Components)
            {
                var header = field as StiCrossHeader;
                if (header != null && !header.IsTotalVisible && header.Total != null)
                {
                    hidedTotals[header.Total] = header.Total;
                }
            }

            var distXX = this.crossTab.Report.Unit.ConvertFromHInches(2m);
            var distYY = this.crossTab.Report.Unit.ConvertFromHInches(2m);

            foreach (StiCrossField field in this.crossTab.Components)
            {
                if (this.crossTab.CrossTabInfo.Cross.Fields[field] is Point)
                {
                    var pos = (Point)this.crossTab.CrossTabInfo.Cross.Fields[field];

                    var fieldRect = this.crossTab.Report.Unit.ConvertToHInches(CrossTab.StiCrossTabHelper.GetCellRect(this.crossTab, pos.X, pos.Y));

                    rect.Width = Math.Max(rect.Width, fieldRect.Right);
                    rect.Height = Math.Max(rect.Height, fieldRect.Bottom);

                    fieldRect = this.crossTab.Report.Unit.ConvertFromHInches(fieldRect);
                    if (hidedTotals[field] != null)
                    {
                        fieldRect.Width = 0;
                        fieldRect.Height = 0;
                    }

                    #region Change position
                    var distX = 0m;
                    var distY = 0m;

                    #region StiCrossTitle
                    var title = field as StiCrossTitle;
                    if (title != null)
                    {
                        #region LeftTitle
                        if (title.TypeOfComponent.StartsWith("LeftTitle", StringComparison.InvariantCulture))
                        {
                            distX = 0;
                            distY = 0;
                        }
                        #endregion

                        #region RightTitle
                        if (title.TypeOfComponent.StartsWith("RightTitle", StringComparison.InvariantCulture))
                        {
                            distX = distXX;
                            distY = 0;
                        }
                        #endregion

                        #region SummaryTitle
                        if (title.TypeOfComponent.StartsWith("SummaryTitle", StringComparison.InvariantCulture))
                        {
                            if (rowsContainer.Count == 0)
                            {
                                distX = 0;
                                distY = distYY * 2;
                            }

                            if (columnsContainer.Count == 0)
                            {
                                distX = distXX;
                                distY = distYY;
                            }
                        }
                        #endregion

                        #region Row
                        if (title.TypeOfComponent.StartsWith("Row:", StringComparison.InvariantCulture))
                        {
                            distX = 0;
                            distY = distYY;
                        }
                        #endregion

                        #region Col
                        if (title.TypeOfComponent.StartsWith("Col:", StringComparison.InvariantCulture))
                        {
                            distX = 0;
                            distY = distYY;
                        }
                        #endregion
                    }
                    #endregion

                    #region StiCrossRow
                    var row = field as StiCrossRow;
                    if (row != null)
                    {
                        distX = 0;
                        distY = distYY * 2;
                    }
                    #endregion

                    #region StiCrossRowTotal
                    var rowTotal = field as StiCrossRowTotal;
                    if (rowTotal != null)
                    {
                        distX = 0;
                        distY = distYY * 2;
                    }
                    #endregion

                    #region StiCrossColumn
                    var col = field as StiCrossColumn;
                    if (col != null)
                    {
                        distX = distXX;
                        distY = distYY;
                    }
                    #endregion

                    #region StiCrossColumnTotal
                    var colTotal = field as StiCrossColumnTotal;
                    if (colTotal != null)
                    {
                        distX = distXX;
                        distY = distYY;
                    }
                    #endregion

                    #region StiCrossSummary
                    var summary = field as StiCrossSummary;
                    if (summary != null)
                    {
                        distX = distXX;
                        distY = distYY * 2;
                    }
                    #endregion

                    #region StiCrossSummaryHeader
                    var summaryHeader = field as StiCrossSummaryHeader;
                    if (summaryHeader != null)
                    {
                        if (this.crossTab.SummaryDirection == StiSummaryDirection.LeftToRight)
                        {
                            distX = distXX;
                            distY = distYY;
                        }
                        else
                        {
                            distX = 0;//distXX;
                            distY = distYY * 2;
                        }
                    }
                    #endregion

                    fieldRect.X += distX;
                    fieldRect.Y += distY;
                    #endregion

                    field.ClientRectangle = fieldRect.ToRectangleD();
                }

                field.SetTextInternal(field.CellText);
            }
            #endregion
        }

        private StiCrossRowTotal CreateRowTotal(string rowGuid)
        {
            var rowTotal = createdTotals[rowGuid] as StiCrossRowTotal;
            if (rowTotal == null)
            {
                rowTotal = new StiCrossRowTotal();
                this.crossTab.ApplyFieldStyle(rowTotal);
            }
            createdTotals[rowGuid] = rowTotal;

            return rowTotal;
        }

        private StiCrossColumnTotal CreateColTotal(string colGuid)
        {
            var colTotal = createdTotals[colGuid] as StiCrossColumnTotal;
            if (colTotal == null)
            {
                colTotal = new StiCrossColumnTotal();
                this.crossTab.ApplyFieldStyle(colTotal);
            }
            createdTotals[colGuid] = colTotal;

            colTotals.Add(colTotal);
            return colTotal;
        }                

        private void SwapColumnsAndRows()
        {
            var rows = new List<StiCrossRow>();

            foreach (StiCrossColumn column in this.columnsContainer)
            {
                var row = CopyColumnToRow(column);
                rows.Add(row);

                ConvertColumnTotal(column, row);
            }
            this.columnsContainer.Clear();

            foreach (StiCrossRow row in this.rowsContainer)
            {
                var column = CopyRowToColumn(row);

                this.columnsContainer.Add(column);

                ConvertRowTotal(row, column);
            }
            this.rowsContainer.Clear();

            foreach (StiCrossRow row in rows)
            {
                this.rowsContainer.Add(row);
            }
            UpdateCrossTab();
        }

        private void ChangeSummaryDirection(string summaryDirection)
        {
            this.crossTab.SummaryDirection = (StiSummaryDirection)Enum.Parse(typeof(StiSummaryDirection), summaryDirection);
            UpdateCrossTab();
        }

        private StiCrossHeader CopySummaryToRow(StiCrossSummary field)
        {
            var header = new StiCrossRow();
            CopyFieldToField(header, field);
            header.DisplayValue.Value = header.Value.Value = field.Value.Value;
            return header;
        }

        private StiCrossHeader CopySummaryToColumn(StiCrossSummary field)
        {
            var header = new StiCrossColumn();
            CopyFieldToField(header, field);
            header.DisplayValue.Value = header.Value.Value = field.Value.Value;
            return header;
        }

        private StiCrossSummary CopyHeaderToSummary(StiCrossHeader header)
        {
            var field = new StiCrossSummary();
            CopyFieldToField(field, header);
            field.Value.Value = header.Value.Value;
            return field;
        }

        private StiCrossColumn CopyRowToColumn(StiCrossRow row)
        {
            var field = new StiCrossColumn();
            CopyFieldToField(field, row);
            field.Value.Value = row.Value.Value;
            field.DisplayValue.Value = row.DisplayValue.Value;
            return field;
        }

        private StiCrossRow CopyColumnToRow(StiCrossColumn column)
        {
            var field = new StiCrossRow();
            CopyFieldToField(field, column);
            field.Value.Value = column.Value.Value;
            field.DisplayValue.Value = column.DisplayValue.Value;
            return field;
        }

        private StiCrossColumnTotal CopyRowTotalToColumnTotal(StiCrossRowTotal row)
        {
            var field = new StiCrossColumnTotal();
            CopyFieldToField(field, row);
            field.Text.Value = row.Text.Value;
            return field;
        }

        private StiCrossRowTotal CopyColumnTotalToRowTotal(StiCrossColumnTotal column)
        {
            var field = new StiCrossRowTotal();
            CopyFieldToField(field, column);
            field.Text.Value = column.Text.Value;
            return field;
        }

        private void CopyFieldToField(StiCrossField dest, StiCrossField source)
        {
            dest.Name = source.Name;
            dest.Alias = source.Alias;

            dest.Guid = source.Guid;
            dest.Border = (StiBorder)source.Border.Clone();
            dest.Brush = (StiBrush)source.Brush.Clone();
            dest.Font = (Font)source.Font.Clone();
            dest.TextFormat = (Stimulsoft.Report.Components.TextFormats.StiFormatService)source.TextFormat.Clone();
            dest.TextBrush = (StiBrush)source.TextBrush.Clone();
            dest.TextOptions = (StiTextOptions)source.TextOptions.Clone();

            if (source is StiCrossHeader && dest is StiCrossHeader)
            {
                ((StiCrossHeader)dest).ShowTotal = ((StiCrossHeader)source).ShowTotal;
                ((StiCrossHeader)dest).SortDirection = ((StiCrossHeader)source).SortDirection;
                ((StiCrossHeader)dest).SortType = ((StiCrossHeader)source).SortType;
                ((StiCrossHeader)dest).PrintOnAllPages = ((StiCrossHeader)source).PrintOnAllPages;
            }


            dest.HorAlignment = source.HorAlignment;
            dest.VertAlignment = source.VertAlignment;
        }

        private void ConvertColumnTotal(StiCrossColumn sourceColumn, StiCrossRow destRow)
        {
            if (sourceColumn.TotalGuid != null)
            {
                var columnTotal = sourceColumn.Total as StiCrossColumnTotal;
                crossTab.Components.Remove(columnTotal);

                var rowTotal = CopyColumnTotalToRowTotal(columnTotal);
                crossTab.Components.Add(rowTotal);

                destRow.TotalGuid = rowTotal.Guid;
            }
        }

        private void ConvertRowTotal(StiCrossRow sourceRow, StiCrossColumn destColumn)
        {
            if (sourceRow.TotalGuid != null)
            {
                var rowTotal = sourceRow.Total as StiCrossRowTotal;
                crossTab.Components.Remove(rowTotal);

                var columnTotal = CopyRowTotalToColumnTotal(rowTotal);
                crossTab.Components.Add(columnTotal);

                destColumn.TotalGuid = columnTotal.Guid;
            }
        }
                
        private void KillRightTitle()
        {
            foreach (StiComponent comp in this.crossTab.Components)
            {
                var title = comp as StiCrossTitle;
                if (title != null && title.TypeOfComponent.StartsWith("RightTitle", StringComparison.InvariantCulture))
                {
                    this.crossTab.Components.Remove(comp);
                    break;
                }
            }
        }

        private void OrderRows()
        {
            var rowIndex = 1;
            foreach (StiCrossRow row in this.rowsContainer)
            {
                var title = row.GetCrossRowTitle();
                var total = row.GetCrossRowTotal();

                row.Name = string.Format("{0}_Row{1}", crossTab.Name, rowIndex);

                if (title != null)
                {
                    title.Name = string.Format("{0}_Title", row.Name);
                    title.TypeOfComponent = string.Format("Row:{0}", row.Name);
                }

                if (total != null)
                    total.Name = string.Format("{0}_RowTotal{1}", crossTab.Name, rowIndex);

                rowIndex++;
            }
        }
        #endregion

        #region Constructor
        public StiCrossTabHelper(StiCrossTab crossTab)
        {
            this.rowTotals = new List<StiCrossTotal>();
            this.colTotals = new List<StiCrossTotal>();
            this.sumHeaders = new List<StiCrossSummaryHeader>();
            this.columnsContainer = new List<StiCrossField>();
            this.rowsContainer = new List<StiCrossField>();
            this.summaryContainer = new List<StiCrossField>();
            this.createdTotals = new Hashtable();
            this.crossTab = crossTab;
            this.oldLeft = this.crossTab.Left;
            this.oldTop = this.crossTab.Top;
            this.crossTab.Left = this.crossTab.Top = 0;
            this.selectedDataSource = crossTab.DataSource;
            this.selectedBusinessObject = crossTab.BusinessObject;

            foreach (StiComponent comp in this.crossTab.Components)
            {
                if (comp is StiCrossSummary || comp is StiCrossColumn || comp is StiCrossRow)
                {
                    if (comp is StiCrossSummary) summaryContainer.Add(comp as StiCrossSummary);
                    if (comp is StiCrossColumn) columnsContainer.Add(comp as StiCrossColumn);
                    if (comp is StiCrossRow) rowsContainer.Add(comp as StiCrossRow);
                }

                var summary = comp as StiCrossSummaryHeader;
                if (summary != null)
                {
                    sumHeaders.Add(summary);
                }
            }
        }
        #endregion   
    }
}