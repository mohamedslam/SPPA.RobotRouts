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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Export;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiComboBoxElementHelper
    {
        #region Fields
        private IStiComboBoxElement comboBoxElement;
        #endregion

        #region Helper Methods
        private Hashtable GetComboBoxElementJSProperties(Hashtable parameters)
        {
            Hashtable properties = StiReportEdit.GetAllProperties(comboBoxElement as StiComponent);
            properties["meters"] = GetMetersHash();

            if ((string)parameters["command"] != "GetComboBoxElementProperties")
            {
                properties["svgContent"] = StiEncodingHelper.Encode(StiDashboardsSvgHelper.SaveElementToString(comboBoxElement, 1, 1, true));
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

        public Hashtable GetMetersHash()
        {
            var meters = comboBoxElement.FetchAllMeters();
            var metersItems = new Hashtable();

            foreach (IStiMeter meter in meters)
            {
                switch (meter.GetType().Name)
                {
                    case "StiKeyComboBoxMeter":
                        {
                            metersItems["key"] = GetMeterHashItem(meter);
                            break;
                        }
                    case "StiNameComboBoxMeter":
                        {
                            metersItems["name"] = GetMeterHashItem(meter);
                            break;
                        }
                }
            }

            return metersItems;
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
                case "RenameMeter":
                    {
                        RenameMeter(parameters, callbackResult);
                        break;
                    }
                case "MoveMeter":
                case "MoveAndDuplicateMeter":
                    {
                        MoveMeter(parameters, callbackResult);
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
                case "GetComboBoxElementProperties":
                    {
                        break;
                    }
            }

            callbackResult["elementProperties"] = GetComboBoxElementJSProperties(parameters);
        }       

        private void MoveMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var duplicateMeter = (string)parameters["command"] == "MoveAndDuplicateMeter";
            var toContainerName = parameters["toContainerName"] as string;
            var fromContainerName = parameters["fromContainerName"] as string;
            IStiMeter movingMeter = null;

            #region Get and Remove meter
            switch (fromContainerName)
            {
                case "Key":
                    {
                        movingMeter = comboBoxElement.GetKeyMeter();
                        if (!duplicateMeter)
                            comboBoxElement.RemoveKeyMeter();
                        break;
                    }
                case "Name":
                    {
                        movingMeter = comboBoxElement.GetNameMeter();
                        if (!duplicateMeter)
                            comboBoxElement.RemoveNameMeter();
                        break;
                    }
            }
            #endregion

            #region Insert meter
            if (movingMeter != null)
            {
                if (duplicateMeter)
                {
                    var cloneMeter = (movingMeter as ICloneable).Clone();
                    movingMeter = cloneMeter as IStiMeter;
                }

                switch (toContainerName)
                {
                    case "Key":
                        {
                            comboBoxElement.AddKeyMeter(movingMeter);
                            break;
                        }
                    case "Name":
                        {
                            comboBoxElement.AddNameMeter(movingMeter);
                            break;
                        }
                }
            }
            #endregion
        }

        private void CreateNewItem(Hashtable parameters, Hashtable callbackResult)
        {
            switch (parameters["containerName"] as string)
            {
                case "Key":
                    {
                        comboBoxElement.CreateNewKeyMeter();
                        comboBoxElement.GetKeyMeter().Expression = parameters["expression"] as string;
                        break;
                    }
                case "Name":
                    {
                        comboBoxElement.CreateNewNameMeter();
                        comboBoxElement.GetNameMeter().Expression = parameters["expression"] as string;
                        break;
                    }
            }
        }

        private void EditField(Hashtable parameters, Hashtable callbackResult)
        {
            switch (parameters["containerName"] as string)
            {
                case "Key":
                    {
                        var keyMeter = comboBoxElement.GetKeyMeter();
                        if (keyMeter != null) keyMeter.Expression = parameters["expression"] as string;
                        break;
                    }
                case "Name":
                    {
                        var nameMeter = comboBoxElement.GetNameMeter();
                        if (nameMeter != null) nameMeter.Expression = parameters["expression"] as string;
                        break;
                    }
            }
        }

        private void RenameMeter(Hashtable parameters, Hashtable callbackResult)
        {
            if (parameters["newLabel"] != null)
            {
                IStiMeter meter = null;

                switch (parameters["containerName"] as string)
                {
                    case "Key":
                        {
                            meter = comboBoxElement.GetKeyMeter();
                            break;
                        }
                    case "Name":
                        {
                            meter = comboBoxElement.GetNameMeter();
                            break;
                        }
                }

                if (meter != null)
                {
                    StiElementChangedProcessor.ProcessElementChanging(comboBoxElement, StiElementChangedArgs.CreateRenamingArgs(meter.Label, parameters["newLabel"] as string));
                    meter.Label = parameters["newLabel"] as string;
                }
            }
        }

        private void SetDataColumn(Hashtable parameters, Hashtable callbackResult)
        {
            var dataColumnObject = parameters["dataColumnObject"] as Hashtable;
            IStiAppDataCell dataCell = null;
            var oldLabel = string.Empty;

            if (dataColumnObject != null)
            {
                if (dataColumnObject["typeItem"] as string == "Variable")
                {
                    dataCell = comboBoxElement.Report.Dictionary.Variables[dataColumnObject["name"] as string] as IStiAppDataCell;
                }
                else
                {
                    var allColumns = dataColumnObject != null ? StiDictionaryHelper.GetColumnsByTypeAndNameOfObject(comboBoxElement.Report, dataColumnObject) : null;
                    if (allColumns != null) dataCell = allColumns[dataColumnObject["name"] as string];
                }
            }
            
            switch (parameters["containerName"] as string)
            {
                case "Key":
                    {
                        oldLabel = comboBoxElement.GetKeyMeter()?.Label;
                        comboBoxElement.AddKeyMeter(dataCell);
                        break;
                    }
                case "Name":
                    {
                        oldLabel = comboBoxElement.GetNameMeter()?.Label;
                        comboBoxElement.AddNameMeter(dataCell);
                        break;
                    }
            }

            if (dataCell == null && !String.IsNullOrEmpty(oldLabel))
                StiElementChangedProcessor.ProcessElementChanging(comboBoxElement, StiElementChangedArgs.CreateDeletingArgs(oldLabel));
        }

        private void SetPropertyValue(Hashtable parameters, Hashtable callbackResult)
        {
            var propertyName = parameters["propertyName"] as string;

            switch (propertyName)
            {
                case "SelectionMode":
                    {
                        comboBoxElement.SelectionMode = (StiItemSelectionMode)Enum.Parse(typeof(StiItemSelectionMode), parameters["propertyValue"] as string);
                        break;
                    }
                case "ShowAllValue":
                    {
                        comboBoxElement.ShowAllValue = Convert.ToBoolean(parameters["propertyValue"]);
                        break;
                    }
                case "ShowBlanks":
                    {
                        comboBoxElement.ShowBlanks = Convert.ToBoolean(parameters["propertyValue"]);
                        break;
                    }
                case "ParentKey":
                    {
                        comboBoxElement.SetParentKey(!String.IsNullOrEmpty(parameters["propertyValue"] as string) ? parameters["propertyValue"] as string : null);
                        break;
                    }
            }
        }

        public static void CreateComboBoxElementFromDictionary(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.IsModified = true;
            Hashtable point = (Hashtable)param["point"];
            Hashtable itemObject = (Hashtable)param["itemObject"];
            StiPage currentPage = report.Pages[param["pageName"] as string];

            var newComponent = StiDashboardHelper.CreateDashboardElement(report, "StiComboBoxElement");
            var compWidth = newComponent.DefaultClientRectangle.Size.Width;
            var compXPos = StiReportEdit.StrToDouble((string)point["x"]);
            var compYPos = StiReportEdit.StrToDouble((string)point["y"]);

            if (compXPos + currentPage.Margins.Left + compWidth * 2 > currentPage.Width)
                compXPos = currentPage.Width - currentPage.Margins.Left - compWidth * 2;

            StiReportEdit.AddComponentToPage(newComponent, currentPage);
            RectangleD compRect = new RectangleD(new PointD(compXPos, compYPos), new SizeD(compWidth, newComponent.DefaultClientRectangle.Size.Height));
            StiReportEdit.SetComponentRect(newComponent, compRect);

            var variable = report.Dictionary.Variables[itemObject["name"] as string];
            if (variable != null)
                ((IStiComboBoxElement)newComponent).AddKeyMeter(variable);

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
        public StiComboBoxElementHelper(IStiComboBoxElement comboBoxElement)
        {
            this.comboBoxElement = comboBoxElement;
        }
        #endregion   
    }
}
