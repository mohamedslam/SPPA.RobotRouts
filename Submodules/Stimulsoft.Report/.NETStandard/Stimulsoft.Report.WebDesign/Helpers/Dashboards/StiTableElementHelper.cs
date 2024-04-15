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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Engine;
using Stimulsoft.Data.Functions;
using Stimulsoft.Data.Helpers;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiTableElementHelper
    {
        #region Fields
        private IStiTableElement tableElement;
        #endregion

        #region Helper Methods
        private Hashtable GetMeterItem(IStiMeter meter)
        {   
            Hashtable meterItem = new Hashtable();
            meterItem["typeItem"] = "Meter";
            meterItem["type"] = GetMeterType(meter);
            meterItem["typeIcon"] = GetMeterTypeIcon(meter);
            meterItem["label"] = GetMeterLabel(meter);
            meterItem["labelCorrect"] = StiNameValidator.CorrectName(GetMeterLabel(meter));
            meterItem["expression"] = meter.Expression != null ? StiEncodingHelper.Encode(meter.Expression) : string.Empty;
            meterItem["functions"] = GetMeterFunctions(meter, tableElement.Page as IStiDashboard);
            meterItem["currentFunction"] = StiExpressionHelper.GetFunction(meter.Expression);
            meterItem["visible"] = (meter as IStiTableColumn).Visible;
            meterItem["visibility"] = (meter as IStiTableColumn).Visibility;
            meterItem["visibilityExpression"] = (meter as IStiTableColumn).VisibilityExpression != null ? StiEncodingHelper.Encode((meter as IStiTableColumn).VisibilityExpression) : "";
            meterItem["textFormat"] = StiTextFormatHelper.GetTextFormatItem((meter as IStiTextFormat).TextFormat);
            meterItem["horAlignment"] = (meter as IStiHorAlignment).HorAlignment;
            meterItem["headerAlignment"] = (meter as IStiTableColumn).HeaderAlignment;
            meterItem["foreColor"] = StiReportEdit.GetStringFromColor((meter as IStiForeColor).ForeColor);
            meterItem["interaction"] = StiReportEdit.GetDashboardInteractionProperty((meter as IStiElementInteraction)?.DashboardInteraction);
            meterItem["showTotalSummary"] = (meter as IStiTableColumn).ShowTotalSummary;
            meterItem["summaryType"] = (meter as IStiTableColumn).SummaryType;
            meterItem["summaryAlignment"] = (meter as IStiTableColumn).SummaryAlignment;

            if (meter is IStiDimensionColumn)
            {
                meterItem["showHyperlink"] = ((IStiDimensionColumn)meter).ShowHyperlink;
                meterItem["hyperlinkPattern"] = StiEncodingHelper.Encode(((IStiDimensionColumn)meter).HyperlinkPattern);
            }

            if (meter is IStiSparklinesColumn)
            {
                var sparklinesType = GetSparklinesType(meter);
                var sparklineMeter = meter as IStiSparklinesColumn;
                
                meterItem["sparklinesType"] = sparklinesType;
                meterItem["allowCustomColors"] = sparklineMeter.AllowCustomColors;
                meterItem["positiveColor"] = StiReportEdit.GetStringFromColor(sparklineMeter.PositiveColor);
                meterItem["negativeColor"] = StiReportEdit.GetStringFromColor(sparklineMeter.NegativeColor);

                if (sparklinesType == "Line" || sparklinesType == "Area")
                {
                    meterItem["showHighLowPoints"] = sparklineMeter.ShowHighLowPoints;
                    meterItem["showFirstLastPoints"] = sparklineMeter.ShowFirstLastPoints;
                }
            }

            if (meter is IStiBubbleColumn)
            {
                var bubbleMeter = meter as IStiBubbleColumn;

                meterItem["allowCustomColors"] = bubbleMeter.AllowCustomColors;
                meterItem["positiveColor"] = StiReportEdit.GetStringFromColor(bubbleMeter.PositiveColor);
                meterItem["negativeColor"] = StiReportEdit.GetStringFromColor(bubbleMeter.NegativeColor);
            }

            if (meter is IStiColorScaleColumn)
            {
                meterItem["minimumColor"] = StiReportEdit.GetStringFromColor(((IStiColorScaleColumn)meter).MinimumColor);
                meterItem["maximumColor"] = StiReportEdit.GetStringFromColor(((IStiColorScaleColumn)meter).MaximumColor);
            }

            if (meter is IStiTableColumnSize)
            {
                var size = ((IStiTableColumnSize)meter).Size;
                meterItem["size.Width"] = size.Width;
                meterItem["size.MaxWidth"] = size.MaxWidth;
                meterItem["size.MinWidth"] = size.MinWidth;
                meterItem["size.WordWrap"] = size.WordWrap;
            }

            return meterItem;
        }

        private ArrayList GetMetersItems()
        {
            var metersItems = new ArrayList();
            var meters = tableElement.FetchAllMeters();

            foreach (IStiMeter meter in meters)
            {
                metersItems.Add(GetMeterItem(meter));
            }

            return metersItems;
        }

        internal static List<string> GetMeterFunctions(IStiMeter meter, IStiDashboard dashboard)
        {
            var funcs = new List<string>();
            if (meter == null) return null;

            if (meter is IStiDimensionMeter)
            {
                if (StiDataExpressionHelper.IsDateDataColumnInExpression(dashboard, meter.Expression))
                    funcs = funcs.Union(Funcs.GetDateDimensionFunctions()).ToList();
            }

            if (!(meter is IStiSparklinesColumn))
            {
                if (StiDataExpressionHelper.IsNumericDataColumnInExpression(dashboard, meter.Expression))
                {
                    funcs = funcs.Union(Funcs.GetAggregateMeasureFunctions()).ToList();
                    funcs.Add("PercentOfGrandTotal");
                }
                else
                    funcs = funcs.Union(Funcs.GetCommonMeasureFunctions()).ToList();
            }

            return funcs.Any() ? funcs : null;
        }

        private Hashtable GetTableElementJSProperties(Hashtable parameters)
        {
            Hashtable properties = StiReportEdit.GetAllProperties(tableElement as StiComponent);
            properties["meters"] = GetMetersItems();

            if ((string)parameters["command"] != "GetTableElementProperties")
            {
                properties["svgContent"] = StiEncodingHelper.Encode(StiDashboardsSvgHelper.SaveElementToString(tableElement, 1, 1, true));
            }

            return properties;
        }

        internal static string GetMeterLabel(IStiMeter meter)
        {
            return StiLabelHelper.GetLabel(meter);
        }

        private static string GetSparklinesType(IStiMeter meter)
        {
            return StiInvokeMethodsHelper.GetPropertyValue(meter, "Type").ToString();
        }

        internal static string GetMeterType(IStiMeter meter)
        {
            
            if (meter is IStiDimensionColumn) return "Dimension";
            if (meter is IStiBubbleColumn) return "Bubble";
            if (meter is IStiDataBarsColumn) return "DataBars";
            if (meter is IStiColorScaleColumn) return "ColorScale";
            if (meter is IStiIndicatorColumn) return "Indicator";
            if (meter is IStiSparklinesColumn) return "Sparklines";

            return "Measure";
        }

        internal static string GetMeterTypeIcon(IStiMeter meter)
        {
            var meterType = GetMeterType(meter);

            if (meter is IStiSparklinesColumn)
            {
                string sparklinesType = GetSparklinesType(meter);

                if (sparklinesType == "Line") return "SparklinesLine";
                if (sparklinesType == "Area") return "SparklinesArea";
                if (sparklinesType == "Column") return "SparklinesColumn";
                if (sparklinesType == "WinLoss") return "SparklinesWinLoss";
            }

            return meterType;
        }

        private static void CheckMeasureMeterTextFormat(IStiMeter meter)
        {
            if (meter is IStiTextFormat)
            {
                var textFormat = (meter as IStiTextFormat).TextFormat;
                var functionName = Funcs.ToLowerCase(StiExpressionHelper.GetFunction(meter.Expression));

                if (!(textFormat is StiGeneralFormatService ||
                     textFormat is StiCurrencyFormatService ||
                      textFormat is StiNumberFormatService ||
                      textFormat is StiPercentageFormatService))
                {   
                    var format = new StiNumberFormatService();

                    if (functionName == "count" || functionName == "countdiscount")
                    {
                        format.State |= Stimulsoft.Report.Components.StiTextFormatState.DecimalDigits;
                        format.DecimalDigits = 0;
                    }

                     (meter as IStiTextFormat).TextFormat = format;
                }
            }
        }

        private static void CheckMeterTextFormatForPercentOfGrandTotal(IStiMeter oldMeter, IStiMeter newMeter, string meterType)
        {
            if (newMeter is IStiTextFormat)
            {
                var oldFunction = Funcs.ToLowerCase(StiExpressionHelper.GetFunction(oldMeter.Expression));
                var newFunction = Funcs.ToLowerCase(StiExpressionHelper.GetFunction(newMeter.Expression));
                var meterTextFormat = newMeter as IStiTextFormat;

                if (meterTextFormat != null)
                {
                    if (newFunction == "percentofgrandtotal")
                    {
                        if (!(meterTextFormat.TextFormat is StiPercentageFormatService))
                            meterTextFormat.TextFormat = new StiPercentageFormatService();
                    }
                    else if (oldFunction == "percentofgrandtotal" && meterTextFormat.TextFormat is StiPercentageFormatService)
                    {
                        meterTextFormat.TextFormat = new StiNumberFormatService();
                    }
                }
            }
        }

        private static string GetTableTitleByTypeAndNameOfObject(StiReport report, Hashtable props)
        {
            string currentParentType = (string)props["currentParentType"];
            var title = string.Empty;

            if (currentParentType == "DataSource")
            {
                var dataSource = report.Dictionary.DataSources[(string)props["currentParentName"]];
                if (dataSource != null) title = !string.IsNullOrEmpty(dataSource.Alias) ? dataSource.Alias : dataSource.Name;
            }
            if (currentParentType == "BusinessObject")
            {
                var businessObject = StiDictionaryHelper.GetBusinessObjectByFullName(report, props["currentParentName"]);
                if (businessObject != null) title = !string.IsNullOrEmpty(businessObject.Alias) ? businessObject.Alias : businessObject.Name;
            }

            return title;
        }
        #endregion

        #region Methods
        public void ExecuteJSCommand(Hashtable parameters, Hashtable callbackResult)
        {
            switch ((string)parameters["command"])
            {
                case "InsertMeters":
                    {
                        InsertMeters(parameters, callbackResult);
                        break;
                    }
                case "RemoveMeter":
                    {
                        RemoveMeter(parameters, callbackResult);
                        break;
                    }
                case "RenameMeter":
                    {
                        RenameMeter(parameters, callbackResult);
                        break;
                    }
                case "RemoveAllMeters":
                    {
                        RemoveAllMeters();
                        break;
                    }
                case "ConvertMeter":
                    {
                        ConvertMeter(parameters, callbackResult);
                        break;
                    }
                case "MoveMeter":
                case "MoveAndDuplicateMeter":
                    {
                        MoveMeter(parameters, callbackResult);
                        break;
                    }
                case "NewMeter":
                    {
                        NewMeter(parameters, callbackResult);
                        break;
                    }
                case "DuplicateMeter":
                    {
                        DuplicateMeter(parameters, callbackResult);
                        break;
                    }
                case "SetFunction":
                    {
                        SetFunction(parameters, callbackResult);
                        break;
                    }
                case "ChangeSparklinesType":
                    {
                        ChangeSparklinesType(parameters, callbackResult);
                        break;
                    }
                case "SetPropertyValue":
                    {
                        SetPropertyValue(parameters, callbackResult);
                        break;
                    }
                case "GetTableElementProperties":
                    {
                        break;
                    }
            }

            callbackResult["elementProperties"] = GetTableElementJSProperties(parameters);
        }

        private void InsertMeters(Hashtable parameters, Hashtable callbackResult)
        {
            var draggedItem = parameters["draggedItem"] as Hashtable;
            var draggedItemObject = draggedItem["itemObject"] as Hashtable;
            var typeDraggedItem = draggedItemObject["typeItem"] as string;
            var draggedColumns = new List<IStiAppDataCell>();
            var insertIndex = parameters["insertIndex"] != null ? Convert.ToInt32(parameters["insertIndex"]) : -1;

            if (typeDraggedItem == "Variable")
            {
                var variableDataCell = tableElement.Report.Dictionary.Variables[draggedItemObject["name"] as string] as IStiAppDataCell;
                if (variableDataCell != null) draggedColumns.Add(variableDataCell);
            }
            else
            {
                var allColumns = StiDictionaryHelper.GetColumnsByTypeAndNameOfObject(tableElement.Report, draggedItem);
                if (allColumns != null)
                {
                    if (typeDraggedItem == "Column")
                    {
                        var draggedColumn = allColumns[draggedItemObject["name"] as string];
                        if (draggedColumn != null) draggedColumns.Add(draggedColumn);
                    }
                    else
                    {
                        draggedColumns.AddRange(allColumns.Cast<IStiAppDataCell>());
                    }
                }
            }

            foreach (IStiAppDataCell dataColumn in draggedColumns)
            {
                var newMeter = dataColumn is StiDataColumn && StiDataColumnExt.IsNumericType(dataColumn as StiDataColumn)
                        ? tableElement.GetMeasure(dataColumn)
                        : tableElement.GetDimension(dataColumn);

                if (dataColumn is StiDataColumn && StiDataColumnExt.IsNumericType(dataColumn as StiDataColumn) && newMeter is IStiTableColumn)
                    (newMeter as IStiTableColumn).ShowTotalSummary = true;

                tableElement.InsertMeter(insertIndex, newMeter);
            }
        }

        private void RemoveMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
            var meters = tableElement.FetchAllMeters();
            if (itemIndex >= 0 && itemIndex < meters.Count)
            {
                StiElementChangedProcessor.ProcessElementChanging(tableElement, StiElementChangedArgs.CreateDeletingArgs(meters[itemIndex].Label));
                tableElement.RemoveMeter(itemIndex);
            }
        }

        private void RenameMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
            var meters = tableElement.FetchAllMeters();
            if (itemIndex >= 0 && itemIndex < meters.Count && parameters["newLabel"] != null)
            {
                StiElementChangedProcessor.ProcessElementChanging(tableElement, StiElementChangedArgs.CreateRenamingArgs(meters[itemIndex].Label, parameters["newLabel"] as string));
                meters[itemIndex].Label = parameters["newLabel"] as string;
            }
        }

        private void RemoveAllMeters()
        {
            tableElement.RemoveAllMeters();
            StiElementChangedProcessor.ProcessElementChanging(tableElement, StiElementChangedArgs.CreateClearingAllArgs());
        }

        private void ConvertMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
            var meterType = parameters["meterType"] as string;
            var meters = tableElement.FetchAllMeters();

            if (itemIndex < meters.Count)
            {                
                var currentMeter = meters[itemIndex];
                var dashboard = tableElement.Page as IStiDashboard;
                object newMeter = null;

                if (meterType == "Dimension")
                    newMeter = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard", "Helpers.StiMeterHelper+Table", "GetDimension",
                        new object[] { currentMeter }, new Type[] { currentMeter.GetType() });

                else if (meterType == "Measure")
                {
                    newMeter = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard", "Helpers.StiMeterHelper+Table", "GetMeasure",
                        new object[] { currentMeter, dashboard }, new Type[] { currentMeter.GetType(), dashboard.GetType() });
                    CheckMeasureMeterTextFormat(newMeter as IStiMeter);
                }

                else if (meterType == "DataBars")
                    newMeter = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard", "Helpers.StiMeterHelper+Table", "GetDataBars",
                        new object[] { currentMeter, dashboard }, new Type[] { currentMeter.GetType(), dashboard.GetType() });

                else if (meterType == "ColorScale")
                    newMeter = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard", "Helpers.StiMeterHelper+Table", "GetColorScale",
                        new object[] { currentMeter, dashboard }, new Type[] { currentMeter.GetType(), dashboard.GetType() });

                else if (meterType == "Indicator")
                    newMeter = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard", "Helpers.StiMeterHelper+Table", "GetIndicator",
                        new object[] { currentMeter, dashboard }, new Type[] { currentMeter.GetType(), dashboard.GetType() });

                else if (meterType == "Sparklines")
                    newMeter = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard", "Helpers.StiMeterHelper+Table", "GetSparklines",
                        new object[] { currentMeter }, new Type[] { currentMeter.GetType() });

                else if (meterType == "Bubble")
                    newMeter = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard", "Helpers.StiMeterHelper+Table", "GetBubble",
                        new object[] { currentMeter, dashboard }, new Type[] { currentMeter.GetType(), dashboard.GetType() });

                CheckMeterTextFormatForPercentOfGrandTotal(currentMeter, newMeter as IStiMeter, meterType);

                if (newMeter != null) {
                    tableElement.RemoveMeter(itemIndex);
                    tableElement.InsertMeter(itemIndex, newMeter as IStiMeter);
                }
            }
        }

        private void MoveMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var duplicateMeter = (string)parameters["command"] == "MoveAndDuplicateMeter";
            var fromIndex = Convert.ToInt32(parameters["fromIndex"]);
            var toIndex = Convert.ToInt32(parameters["toIndex"]);
            var meters = tableElement.FetchAllMeters();

            if (fromIndex < meters.Count && toIndex < meters.Count)
            {
                var movingMeter = meters[fromIndex];
                if (duplicateMeter)
                {
                    var cloneMeter = (movingMeter as ICloneable).Clone();
                    movingMeter = cloneMeter as IStiMeter;
                }
                else
                {
                    tableElement.RemoveMeter(fromIndex);
                }
                tableElement.InsertMeter(toIndex, movingMeter);
            }
        }

        private void NewMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var insertIndex = Convert.ToInt32(parameters["insertIndex"]);

            switch (parameters["itemType"] as string)
            {
                case "newDimension":
                    {
                        tableElement.InsertNewDimension(insertIndex);
                        break;
                    }
                case "newMeasure":
                    {
                        tableElement.InsertNewMeasure(insertIndex);
                        break;
                    }
            }
        }

        private void DuplicateMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
            var meters = tableElement.FetchAllMeters();
            if (itemIndex < meters.Count)
            {
                var cloneMeter = (meters[itemIndex] as ICloneable).Clone();
                var insertIndex = itemIndex < meters.Count - 1 ? itemIndex + 1 : -1;
                tableElement.InsertMeter(insertIndex, cloneMeter as IStiMeter);
                callbackResult["insertIndex"] = insertIndex;
            }
        }

        private void SetFunction(Hashtable parameters, Hashtable callbackResult)
        {
            var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
            var meters = tableElement.FetchAllMeters();
            var function = parameters["function"] as string;
            if (itemIndex < meters.Count)
            {
                var currentMeter = meters[itemIndex];

                if (function.ToLowerInvariant() == "percentofgrandtotal")
                    currentMeter = SwitchToPercentOfGrandTotal(currentMeter, itemIndex);

                currentMeter.Expression = StiExpressionHelper.ReplaceFunction(currentMeter.Expression, function);
            }
        }

        private IStiMeter SwitchToPercentOfGrandTotal(IStiMeter meter, int itemIndex)
        {
            var newMeter = meter;

            if (!(meter is IStiMeasureMeter))
            {
                var dashboard = tableElement.Page as IStiDashboard;
                newMeter = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard", "Helpers.StiMeterHelper+Table", "GetMeasure",
                    new object[] { meter, dashboard }, new Type[] { meter.GetType(), dashboard.GetType() }) as IStiMeter;
                tableElement.RemoveMeter(itemIndex);
                tableElement.InsertMeter(itemIndex, newMeter);
            }

            var textFormatMeter = newMeter as IStiTextFormat;
            if (textFormatMeter?.TextFormat != null && !(textFormatMeter is StiPercentageFormatService))
                textFormatMeter.TextFormat = new StiPercentageFormatService();

            return newMeter;
        }

        private void ChangeSparklinesType(Hashtable parameters, Hashtable callbackResult)
        {
            var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
            var meters = tableElement.FetchAllMeters();

            if (itemIndex < meters.Count)
            {
                var property = meters[itemIndex].GetType().GetProperty("Type");
                if (property != null)
                    property.SetValue(meters[itemIndex], Enum.Parse(property.PropertyType, parameters["sparklinesType"] as string), null);
            }
        }

        private void SetPropertyValue(Hashtable parameters, Hashtable callbackResult)
        {
            var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
            var meters = tableElement.FetchAllMeters();

            if (itemIndex < meters.Count)
            {
                var propertyName = parameters["propertyName"] as string;
                var propertyValue = parameters["propertyValue"];

                if (propertyName == "Expression" || propertyName == "VisibilityExpression" || propertyName == "HyperlinkPattern")
                {
                    propertyValue = !string.IsNullOrEmpty(propertyValue as string) ? StiEncodingHelper.DecodeString(propertyValue as string) : null;
                }

                if (propertyName == "Interaction")
                {
                    StiReportEdit.SetDashboardInteractionProperty((meters[itemIndex] as IStiElementInteraction)?.DashboardInteraction, propertyValue);
                }
                else if (propertyName == "Size.Width")
                {
                    var size = (meters[itemIndex] as IStiTableColumnSize).Size;
                    var newWidth = StiReportEdit.StrToInt(propertyValue as string);
                    size.Width = Math.Max(Math.Min(size.MaxWidth, newWidth), size.MinWidth);
                }
                else
                    StiReportEdit.SetPropertyValue(tableElement.Report, propertyName, meters[itemIndex], propertyValue);
            }
        }

        public static void CreateTableElementFromDictionary(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.IsModified = true;
            string currentPageName = (string)param["pageName"];
            Hashtable point = param["point"] as Hashtable;            
            StiPage currentPage = report.Pages[currentPageName];

            var newComponent = StiDashboardHelper.CreateDashboardElement(report, "StiTableElement");
            double compWidth = newComponent.DefaultClientRectangle.Size.Width;
            double compHeight = newComponent.DefaultClientRectangle.Size.Height;
            double compXPos = StiReportEdit.StrToDouble((string)point["x"]);
            double compYPos = StiReportEdit.StrToDouble((string)point["y"]);

            if (compXPos + currentPage.Margins.Left + compWidth * 2 > currentPage.Width)
            {
                compXPos = currentPage.Width - currentPage.Margins.Left - compWidth * 2;
            }

            StiReportEdit.AddComponentToPage(newComponent, currentPage);
            RectangleD compRect = new RectangleD(new PointD(compXPos, compYPos), new SizeD(compWidth, compHeight));
            StiReportEdit.SetComponentRect(newComponent, compRect);

            #region Add Data Columns to Table Element
            var draggedItem = param["draggedItem"] as Hashtable;
            var allColumns = StiDictionaryHelper.GetColumnsByTypeAndNameOfObject(report, draggedItem);
            var tableElement = newComponent as IStiTableElement;
            tableElement.Name = StiNameCreation.CreateName(report, report.Info.GenerateLocalizedName ? $"{newComponent.LocalizedName}{draggedItem["currentParentName"]}" : $"Table{draggedItem["currentParentName"]}", false, false, true);
            (tableElement as IStiTitleElement).Title.Text = GetTableTitleByTypeAndNameOfObject(report, draggedItem);

            if (allColumns != null && tableElement != null) 
            {   
                foreach (StiDataColumn dataColumn in allColumns)
                {   
                    tableElement.CreateMeter(dataColumn);
                }
            }
            #endregion

            Hashtable componentProps = new Hashtable();
            componentProps["name"] = newComponent.Name;
            componentProps["typeComponent"] = newComponent.GetType().Name;
            componentProps["componentRect"] = StiReportEdit.GetComponentRect(newComponent);
            componentProps["parentName"] = StiReportEdit.GetParentName(newComponent);
            componentProps["parentIndex"] = StiReportEdit.GetParentIndex(newComponent).ToString();
            componentProps["componentIndex"] = StiReportEdit.GetComponentIndex(newComponent).ToString();
            componentProps["childs"] = StiReportEdit.GetAllChildComponents(newComponent);
            componentProps["svgContent"] = StiReportEdit.GetSvgContent(newComponent);
            componentProps["pageName"] = currentPage.Name;
            componentProps["properties"] = StiReportEdit.GetAllProperties(newComponent);
            componentProps["rebuildProps"] = StiReportEdit.GetPropsRebuildPage(report, currentPage);

            callbackResult["newComponent"] = componentProps;
        }
        #endregion

        #region Constructor
        public StiTableElementHelper(IStiTableElement tableElement)
        {
            this.tableElement = tableElement;
        }
        #endregion   
    }
}
