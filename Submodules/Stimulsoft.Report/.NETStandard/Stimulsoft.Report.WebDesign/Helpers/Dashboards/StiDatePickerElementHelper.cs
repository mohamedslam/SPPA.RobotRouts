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
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Meters;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dictionary;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiDatePickerElementHelper
    {
        #region Fields
        private IStiDatePickerElement datePickerElement;
        #endregion

        #region Helper Methods
        private Hashtable GetDatePickerElementJSProperties(Hashtable parameters)
        {
            Hashtable properties = StiReportEdit.GetAllProperties(datePickerElement as StiComponent);
            properties["meters"] = GetMetersHash();

            if ((string)parameters["command"] != "GetDatePickerElementProperties")
            {
                properties["svgContent"] = StiEncodingHelper.Encode(StiDashboardsSvgHelper.SaveElementToString(datePickerElement, 1, 1, true));
            }

            return properties;
        }

        private Hashtable GetMeterHashItem(IStiMeter meter)
        {
            Hashtable meterItem = new Hashtable();
            meterItem["typeItem"] = "Meter";
            meterItem["type"] = StiTableElementHelper.GetMeterType(meter);
            meterItem["typeIcon"] = StiTableElementHelper.GetMeterTypeIcon(meter);
            meterItem["label"] = StiTableElementHelper.GetMeterLabel(meter);
            meterItem["expression"] = meter.Expression;

            return meterItem;
        }

        private Hashtable GetMetersHash()
        {
            var meters = datePickerElement.FetchAllMeters();
            var metersItems = new Hashtable();

            foreach (IStiMeter meter in meters)
            {
                switch (meter.GetType().Name)
                {
                    case "StiValueDatePickerMeter":
                        {
                            metersItems["value"] = GetMeterHashItem(meter);
                            break;
                        }
                }
            }

            return metersItems;
        }

        public static bool IsVariablePresent(IStiDatePickerElement datePickerElement)
        {
            var valueMeter = datePickerElement.GetValueMeter();
            return valueMeter != null && StiVariableExpressionHelper.IsVariableSpecifiedAsExpression(datePickerElement, valueMeter?.Expression);
        }

        public static bool IsRangeVariablePresent(IStiDatePickerElement datePickerElement)
        {
            var valueMeter = datePickerElement.GetValueMeter();
            return valueMeter != null && IsVariablePresent(datePickerElement) &&
                Range.IsRangeType((StiVariableExpressionHelper.GetVariableSpecifiedAsExpression(datePickerElement, valueMeter?.Expression) as StiVariable)?.Type);
        }
        #endregion

        #region Methods
        public void ExecuteJSCommand(Hashtable parameters, Hashtable callbackResult)
        {
            switch ((string)parameters["command"])
            {                
                case "SetDataColumn":
                    {
                        SetDataColumn(parameters, callbackResult);
                        break;
                    }
                case "MoveMeter":
                case "MoveAndDuplicateMeter":
                    {
                        MoveMeter(parameters, callbackResult);
                        break;
                    }
                case "RenameMeter":
                    {
                        RenameMeter(parameters, callbackResult);
                        break;
                    }
                case "SetPropertyValue":
                    {
                        SetPropertyValue(parameters, callbackResult);
                        break;
                    }
                case "NewItem":
                    {
                        CreateNewItem(parameters, callbackResult);
                        break;
                    }
                case "EditField":
                    {
                        EditField(parameters, callbackResult);
                        break;
                    }
                case "GetDatePickerElementProperties":
                    {
                        break;
                    }
            }

            callbackResult["elementProperties"] = GetDatePickerElementJSProperties(parameters);
        }       

        private void MoveMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var toContainerName = parameters["toContainerName"] as string;
            var fromContainerName = parameters["fromContainerName"] as string;
            IStiMeter movingMeter = null;

            #region Get and Remove meter
            switch (fromContainerName)
            {
                case "Value":
                    {
                        movingMeter = datePickerElement.GetValueMeter();
                        datePickerElement.RemoveValueMeter();
                        break;
                    }
            }
            #endregion

            #region Insert meter
            if (movingMeter != null)
            {
                switch (toContainerName)
                {
                    case "Value":
                        {
                            datePickerElement.AddValueMeter(movingMeter);
                            break;
                        }
                }
            }
            #endregion
        }

        private void RenameMeter(Hashtable parameters, Hashtable callbackResult)
        {
            if (parameters["newLabel"] != null)
            {
                switch (parameters["containerName"] as string)
                {
                    case "Value":
                        {
                            var meter = datePickerElement.GetValueMeter();
                            if (meter != null)
                            {
                                StiElementChangedProcessor.ProcessElementChanging(datePickerElement, StiElementChangedArgs.CreateRenamingArgs(meter.Label, parameters["newLabel"] as string));
                                meter.Label = parameters["newLabel"] as string;
                            }
                            break;
                        }
                }
            }
        }

        private void CreateNewItem(Hashtable parameters, Hashtable callbackResult)
        {
            switch (parameters["containerName"] as string)
            {
                case "Value":
                    {
                        datePickerElement.CreateNewValueMeter();
                        datePickerElement.GetValueMeter().Expression = parameters["expression"] as string;
                        break;
                    }
            }
        }

        private void EditField(Hashtable parameters, Hashtable callbackResult)
        {
            switch (parameters["containerName"] as string)
            {
                case "Value":
                    {
                        var valueMeter = datePickerElement.GetValueMeter();
                        if (valueMeter != null) valueMeter.Expression = parameters["expression"] as string;
                        break;
                    }
            }
        }

        private void SetDataColumn(Hashtable parameters, Hashtable callbackResult)
        {
            var dataColumnObject = parameters["dataColumnObject"] as Hashtable;
            IStiAppDataCell dataCell = null;

            if (dataColumnObject != null)
            {
                if (dataColumnObject["typeItem"] as string == "Variable")
                {
                    dataCell = datePickerElement.Report.Dictionary.Variables[dataColumnObject["name"] as string] as IStiAppDataCell;
                }
                else
                {
                    var allColumns = dataColumnObject != null ? StiDictionaryHelper.GetColumnsByTypeAndNameOfObject(datePickerElement.Report, dataColumnObject) : null;
                    if (allColumns != null) dataCell = allColumns[dataColumnObject["name"] as string];
                }
            }

            switch (parameters["containerName"] as string)
            {
                case "Value":
                    {
                        if (dataCell == null && !String.IsNullOrEmpty(datePickerElement.GetValueMeter()?.Label))
                            StiElementChangedProcessor.ProcessElementChanging(datePickerElement, StiElementChangedArgs.CreateDeletingArgs(datePickerElement.GetValueMeter()?.Label));

                        datePickerElement.AddValueMeter(dataCell);
                        break;
                    }
            }

            if (IsRangeVariablePresent(datePickerElement))
                datePickerElement.SelectionMode = StiDateSelectionMode.Range;
            else if (IsVariablePresent(datePickerElement))
                datePickerElement.SelectionMode = StiDateSelectionMode.Single;
        }

        private void SetPropertyValue(Hashtable parameters, Hashtable callbackResult)
        {
            var propertyName = parameters["propertyName"] as string;

            switch (propertyName)
            {
                case "SelectionMode":
                    {
                        datePickerElement.SelectionMode = (StiDateSelectionMode)Enum.Parse(typeof(StiDateSelectionMode), parameters["propertyValue"] as string);
                        break;
                    }
                case "InitialRangeSelection":
                    {
                        datePickerElement.InitialRangeSelection = (StiInitialDateRangeSelection)Enum.Parse(typeof(StiInitialDateRangeSelection), parameters["propertyValue"] as string);
                        break;
                    }
                case "InitialRangeSelectionSource":
                    {
                        datePickerElement.InitialRangeSelectionSource = (StiInitialDateRangeSelectionSource)Enum.Parse(typeof(StiInitialDateRangeSelectionSource), parameters["propertyValue"] as string);
                        break;
                    }
                case "Condition":
                    {
                        datePickerElement.Condition = (StiDateCondition)Enum.Parse(typeof(StiDateCondition), parameters["propertyValue"] as string);
                        break;
                    }
            }
        }

        public static void CreateDatePickerElementFromDictionary(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.IsModified = true;
            var point = param["point"] as Hashtable;
            var itemObject = param["itemObject"] as Hashtable;
            var typeItem = itemObject["typeItem"] as string;
            var currentPage = report.Pages[param["pageName"] as string];

            var newComponent = StiDashboardHelper.CreateDashboardElement(report, "StiDatePickerElement");
            var compWidth = newComponent.DefaultClientRectangle.Size.Width;
            var compXPos = StiReportEdit.StrToDouble((string)point["x"]);
            var compYPos = StiReportEdit.StrToDouble((string)point["y"]);

            if (compXPos + currentPage.Margins.Left + compWidth * 2 > currentPage.Width)
                compXPos = currentPage.Width - currentPage.Margins.Left - compWidth * 2;

            StiReportEdit.AddComponentToPage(newComponent, currentPage);
            RectangleD compRect = new RectangleD(new PointD(compXPos, compYPos), new SizeD(compWidth, newComponent.DefaultClientRectangle.Size.Height));
            StiReportEdit.SetComponentRect(newComponent, compRect);

            var datePickerElement = newComponent as IStiDatePickerElement;

            if (typeItem == "Column")
            {
                var allColumns = StiDictionaryHelper.GetColumnsByTypeAndNameOfObject(datePickerElement.Report, itemObject);
                if (allColumns != null)
                {
                    var dataColumn = allColumns[itemObject["name"] as string];
                    if (dataColumn != null) datePickerElement.AddValueMeter(dataColumn);
                }
            }
            else
            {
                var variable = report.Dictionary.Variables[itemObject["name"] as string];
                if (variable != null)
                {
                    datePickerElement.AddValueMeter(variable);

                    if (IsRangeVariablePresent(datePickerElement))
                        datePickerElement.SelectionMode = StiDateSelectionMode.Range;
                    else if (IsVariablePresent(datePickerElement))
                        datePickerElement.SelectionMode = StiDateSelectionMode.Single;
                }
            }

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
        public StiDatePickerElementHelper(IStiDatePickerElement datePickerElement)
        {
            this.datePickerElement = datePickerElement;
        }
        #endregion   
    }
}
