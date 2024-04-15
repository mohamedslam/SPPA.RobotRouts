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
using Stimulsoft.Report.Dashboard.Styles;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Export;
using Stimulsoft.Report.Export.Services.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiCardsElementHelper
    {
        #region Fields
        private IStiCardsElement cardsElement;
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
            meterItem["functions"] = GetMeterFunctions(meter, cardsElement.Page as IStiDashboard);
            meterItem["currentFunction"] = StiExpressionHelper.GetFunction(meter.Expression);
            meterItem["visibility"] = (meter as IStiCardsColumn).Visibility;
            meterItem["visibilityExpression"] = (meter as IStiCardsColumn).VisibilityExpression != null ? StiEncodingHelper.Encode((meter as IStiCardsColumn).VisibilityExpression) : "";
            meterItem["textFormat"] = StiTextFormatHelper.GetTextFormatItem((meter as IStiTextFormat).TextFormat);
            meterItem["horAlignment"] = (meter as IStiHorAlignment).HorAlignment;
            meterItem["foreColor"] = StiReportEdit.GetStringFromColor((meter as IStiForeColor).ForeColor);
            meterItem["vertAlignment"] = (meter as IStiVertAlignment).VertAlignment;
            meterItem["wrapLine"] = (meter as IStiCardsColumn).WrapLine;
            meterItem["height"] = (meter as IStiCardsColumn).Height;
            meterItem["font"] = StiReportEdit.FontToStr((meter as IStiFont).Font);

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

            return meterItem;
        }

        private ArrayList GetMetersItems()
        {
            var metersItems = new ArrayList();
            var meters = cardsElement.FetchAllMeters();

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

        private Hashtable GetCardsElementJSProperties(Hashtable parameters)
        {
            Hashtable properties = StiReportEdit.GetAllProperties(cardsElement as StiComponent);
            properties["meters"] = GetMetersItems();

            if ((string)parameters["command"] != "GetCardsElementProperties")
            {
                properties["svgContent"] = StiEncodingHelper.Encode(StiDashboardsSvgHelper.SaveElementToString(cardsElement, 1, 1, true));
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
            
            if (meter is IStiDimensionCardsColumn) return "Dimension";
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

        private static string GetCardsTitleByTypeAndNameOfObject(StiReport report, Hashtable props)
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
                case "GetCardsElementProperties":
                    {
                        break;
                    }
            }

            callbackResult["elementProperties"] = GetCardsElementJSProperties(parameters);
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
                var variableDataCell = cardsElement.Report.Dictionary.Variables[draggedItemObject["name"] as string] as IStiAppDataCell;
                if (variableDataCell != null) draggedColumns.Add(variableDataCell);
            }
            else
            {
                var allColumns = StiDictionaryHelper.GetColumnsByTypeAndNameOfObject(cardsElement.Report, draggedItem);
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
                        ? cardsElement.GetMeasure(dataColumn)
                        : cardsElement.GetDimension(dataColumn);

                cardsElement.InsertMeter(insertIndex, newMeter);
            }
        }

        private void RemoveMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
            var meters = cardsElement.FetchAllMeters();
            if (itemIndex >= 0 && itemIndex < meters.Count)
            {
                StiElementChangedProcessor.ProcessElementChanging(cardsElement, StiElementChangedArgs.CreateDeletingArgs(meters[itemIndex].Label));
                cardsElement.RemoveMeter(itemIndex);
            }
        }

        private void RenameMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
            var meters = cardsElement.FetchAllMeters();
            if (itemIndex >= 0 && itemIndex < meters.Count && parameters["newLabel"] != null)
            {
                StiElementChangedProcessor.ProcessElementChanging(cardsElement, StiElementChangedArgs.CreateRenamingArgs(meters[itemIndex].Label, parameters["newLabel"] as string));
                meters[itemIndex].Label = parameters["newLabel"] as string;
            }
        }

        private void RemoveAllMeters()
        {
            cardsElement.RemoveAllMeters();
            StiElementChangedProcessor.ProcessElementChanging(cardsElement, StiElementChangedArgs.CreateClearingAllArgs());
        }

        private void ConvertMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
            var meterType = parameters["meterType"] as string;
            var meters = cardsElement.FetchAllMeters();

            if (itemIndex < meters.Count)
            {                
                var currentMeter = meters[itemIndex];
                var dashboard = cardsElement.Page as IStiDashboard;
                object newMeter = null;

                if (meterType == "Dimension")
                    newMeter = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard", "Helpers.StiMeterHelper+Cards", "GetDimension",
                        new object[] { currentMeter }, new Type[] { currentMeter.GetType() });

                else if (meterType == "Measure")
                {
                    newMeter = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard", "Helpers.StiMeterHelper+Cards", "GetMeasure",
                        new object[] { currentMeter, dashboard }, new Type[] { currentMeter.GetType(), dashboard.GetType() });
                    CheckMeasureMeterTextFormat(newMeter as IStiMeter);
                }

                else if (meterType == "DataBars")
                    newMeter = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard", "Helpers.StiMeterHelper+Cards", "GetDataBars",
                        new object[] { currentMeter, dashboard }, new Type[] { currentMeter.GetType(), dashboard.GetType() });

                else if (meterType == "ColorScale")
                    newMeter = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard", "Helpers.StiMeterHelper+Cards", "GetColorScale",
                        new object[] { currentMeter, dashboard }, new Type[] { currentMeter.GetType(), dashboard.GetType() });

                else if (meterType == "Indicator")
                    newMeter = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard", "Helpers.StiMeterHelper+Cards", "GetIndicator",
                        new object[] { currentMeter, dashboard }, new Type[] { currentMeter.GetType(), dashboard.GetType() });

                else if (meterType == "Sparklines")
                    newMeter = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard", "Helpers.StiMeterHelper+Cards", "GetSparklines",
                        new object[] { currentMeter }, new Type[] { currentMeter.GetType() });

                else if (meterType == "Bubble")
                    newMeter = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard", "Helpers.StiMeterHelper+Cards", "GetBubble",
                        new object[] { currentMeter, dashboard }, new Type[] { currentMeter.GetType(), dashboard.GetType() });

                CheckMeterTextFormatForPercentOfGrandTotal(currentMeter, newMeter as IStiMeter, meterType);

                if (newMeter != null) {
                    cardsElement.RemoveMeter(itemIndex);
                    cardsElement.InsertMeter(itemIndex, newMeter as IStiMeter);
                }
            }
        }

        private void MoveMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var duplicateMeter = (string)parameters["command"] == "MoveAndDuplicateMeter";
            var fromIndex = Convert.ToInt32(parameters["fromIndex"]);
            var toIndex = Convert.ToInt32(parameters["toIndex"]);
            var meters = cardsElement.FetchAllMeters();

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
                    cardsElement.RemoveMeter(fromIndex);
                }
                cardsElement.InsertMeter(toIndex, movingMeter);
            }
        }

        private void NewMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var insertIndex = Convert.ToInt32(parameters["insertIndex"]);

            switch (parameters["itemType"] as string)
            {
                case "newDimension":
                    {
                        cardsElement.InsertNewDimension(insertIndex);
                        break;
                    }
                case "newMeasure":
                    {
                        cardsElement.InsertNewMeasure(insertIndex);
                        break;
                    }
            }
        }

        private void DuplicateMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
            var meters = cardsElement.FetchAllMeters();
            if (itemIndex < meters.Count)
            {
                var cloneMeter = (meters[itemIndex] as ICloneable).Clone();
                var insertIndex = itemIndex < meters.Count - 1 ? itemIndex + 1 : -1;
                cardsElement.InsertMeter(insertIndex, cloneMeter as IStiMeter);
                callbackResult["insertIndex"] = insertIndex;
            }
        }

        private void SetFunction(Hashtable parameters, Hashtable callbackResult)
        {
            var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
            var meters = cardsElement.FetchAllMeters();
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
                var dashboard = cardsElement.Page as IStiDashboard;
                newMeter = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard", "Helpers.StiMeterHelper+Cards", "GetMeasure",
                    new object[] { meter, dashboard }, new Type[] { meter.GetType(), dashboard.GetType() }) as IStiMeter;
                cardsElement.RemoveMeter(itemIndex);
                cardsElement.InsertMeter(itemIndex, newMeter);
            }

            var textFormatMeter = newMeter as IStiTextFormat;
            if (textFormatMeter?.TextFormat != null && !(textFormatMeter is StiPercentageFormatService))
                textFormatMeter.TextFormat = new StiPercentageFormatService();

            return newMeter;
        }

        private void ChangeSparklinesType(Hashtable parameters, Hashtable callbackResult)
        {
            var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
            var meters = cardsElement.FetchAllMeters();

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
            var meters = cardsElement.FetchAllMeters();

            if (itemIndex < meters.Count)
            {
                var propertyName = parameters["propertyName"] as string;
                var propertyValue = parameters["propertyValue"];

                if (propertyName == "Expression" || propertyName == "VisibilityExpression")
                {
                    propertyValue = !string.IsNullOrEmpty(propertyValue as string) ? StiEncodingHelper.DecodeString(propertyValue as string) : null;
                }
                
                StiReportEdit.SetPropertyValue(cardsElement.Report, propertyName, meters[itemIndex], propertyValue);
            }
        }

        public static void CreateCardsElementFromDictionary(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.IsModified = true;
            string currentPageName = (string)param["pageName"];
            Hashtable point = param["point"] as Hashtable;            
            StiPage currentPage = report.Pages[currentPageName];

            var newComponent = StiDashboardHelper.CreateDashboardElement(report, "StiCardsElement");
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

            #region Add Data Columns to Cards Element
            var draggedItem = param["draggedItem"] as Hashtable;
            var allColumns = StiDictionaryHelper.GetColumnsByTypeAndNameOfObject(report, draggedItem);
            var cardsElement = newComponent as IStiTableElement;
            cardsElement.Name = StiNameCreation.CreateName(report, report.Info.GenerateLocalizedName ? $"{newComponent.LocalizedName}{draggedItem["currentParentName"]}" : $"Cards{draggedItem["currentParentName"]}", false, false, true);
            (cardsElement as IStiTitleElement).Title.Text = GetCardsTitleByTypeAndNameOfObject(report, draggedItem);

            if (allColumns != null && cardsElement != null) 
            {   
                foreach (StiDataColumn dataColumn in allColumns)
                {
                    cardsElement.CreateMeter(dataColumn);
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

        private static List<StiBaseStyle> GetCardsElementStyles(StiReport report, bool withReportStyles = true)
        {
            var styles = new List<StiBaseStyle>();

            if (withReportStyles)
            {
                foreach (StiBaseStyle style in report.Styles)
                {
                    if (style is StiCardsStyle)
                        styles.Add(style);
                }
            }

            styles.AddRange(StiOptions.Services.Dashboards.CardsStyles);

            return styles;
        }

        private static string GetStyleSampleImage(IStiCardsElement cardsElement, StiSparkline sparkLine, StiBaseStyle style, int width, int height)
        {
            var backColor = StiDashboardStyleHelper.GetBackColor(cardsElement);
            Color lineColor = Color.Empty;
            Color cellBackColor = backColor;
            Color cellForeColor = Color.Empty;

            if (style is StiCardsStyle)
            {
                var cardsStyle = style as StiCardsStyle;
                lineColor = cardsStyle.LineColor;
                cellForeColor = cardsStyle.ForeColor;

            }
            else if (style is StiCardsElementStyle)
            {
                var cardsElementStyle = style as StiCardsElementStyle;
                lineColor = cardsElementStyle.LineColor;
                cellBackColor = cardsElementStyle.CellBackColor;
                cellForeColor = cardsElementStyle.CellForeColor;
                sparkLine.PositiveColor = cardsElementStyle.CellSparkline;
            }

            var svgData = new StiSvgData()
            {
                X = 0,
                Y = 0,
                Width = width - 12,
                Height = height - 12,
                Component = sparkLine
            };

            var sb = new StringBuilder();

            using (var ms = new StringWriter(sb))
            {
                var writer = new XmlTextWriter(ms);

                writer.WriteStartElement("div");
                writer.WriteAttributeString("style", $"background: {StiReportHelper.GetHtmlColor(backColor)}; width: {width}px; height: {height}px;");

                writer.WriteStartElement("svg");
                writer.WriteAttributeString("version", "1.1");
                writer.WriteAttributeString("baseProfile", "full");

                writer.WriteAttributeString("xmlns", "http://www.w3.org/2000/svg");
                writer.WriteAttributeString("xmlns:xlink", "http://www.w3.org/1999/xlink");
                writer.WriteAttributeString("xmlns:ev", "http://www.w3.org/2001/xml-events");

                writer.WriteAttributeString("height", svgData.Height.ToString());
                writer.WriteAttributeString("width", svgData.Width.ToString());
                writer.WriteAttributeString("style", $"margin: 5px; background: {StiReportHelper.GetHtmlColor(cellBackColor)};  border: 1px solid {StiReportHelper.GetHtmlColor(lineColor)};");

                writer.WriteStartElement("text");
                writer.WriteAttributeString("dx", "78");
                writer.WriteAttributeString("dy", "13");
                writer.WriteAttributeString("style", $"fill: {StiReportHelper.GetHtmlColor(cellForeColor)}; font-size: 12px; font-family: 'Arial';");
                writer.WriteValue("1000");
                writer.WriteEndElement();

                StiSparklineSvgHelper.WriteSparkline(writer, svgData);

                writer.WriteEndElement();
                writer.WriteEndElement();
                
                writer.Flush();
                ms.Flush();
                writer.Close();
                ms.Close();
            }

            return sb.ToString();
        }

        public static ArrayList GetStylesContent(StiReport report, Hashtable param, bool withReportStyles = true)
        {
            ArrayList stylesContent = new ArrayList();
            var component = report.Pages.GetComponentByName(param["componentName"] as string);
            var currentPage = component != null ? component.Page : new StiPage(report);

            int width = 120;
            int height = 55;

            var tempCardsElement = StiDashboardHelper.CreateDashboardElement(report, "StiCardsElement") as IStiCardsElement;
            var tempCardsComponent = tempCardsElement as StiComponent;

            var tempSparkLine = new StiSparkline();
            tempSparkLine.Values = new decimal[] { 2, 3, 1, 3, 7, 6, 2, 3 };

            currentPage.Components.Add(tempCardsComponent);
            currentPage.Components.Add(tempSparkLine);

            foreach (var style in GetCardsElementStyles(report, withReportStyles))
            {
                Hashtable content = new Hashtable();

                if (style is StiCardsStyle)
                {
                    var cardsStyle = style as StiCardsStyle;
                    tempCardsElement.Style = StiElementStyleIdent.Custom;
                    tempCardsElement.CustomStyleName = cardsStyle.Name;

                    content["name"] = cardsStyle.Name;
                    content["ident"] = StiElementStyleIdent.Custom;
                }
                else if (style is StiCardsElementStyle)
                {
                    var cardsElementStyle = style as StiCardsElementStyle;
                    tempCardsElement.Style = cardsElementStyle.Ident;
                    tempCardsElement.CustomStyleName = string.Empty;

                    content["ident"] = cardsElementStyle.Ident;
                    content["localizedName"] = cardsElementStyle.LocalizedName;
                }

                content["image"] = GetStyleSampleImage(tempCardsElement, tempSparkLine, style, width, height);
                content["width"] = width;
                content["height"] = height;
                stylesContent.Add(content);
            }

            //Remove temp elements
            if (currentPage.Components.Contains(tempSparkLine))
                currentPage.Components.Remove(tempSparkLine);

            if (currentPage.Components.Contains(tempCardsComponent))
                currentPage.Components.Remove(tempCardsComponent);

            return stylesContent;
        }
        #endregion

        #region Constructor
        public StiCardsElementHelper(IStiCardsElement cardsElement)
        {
            this.cardsElement = cardsElement;
        }
        #endregion   
    }
}
