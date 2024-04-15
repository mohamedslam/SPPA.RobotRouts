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
    internal class StiListBoxElementHelper
    {
        #region Fields
        private IStiListBoxElement listBoxElement;
        #endregion

        #region Helper Methods
        private Hashtable GetListBoxElementJSProperties(Hashtable parameters)
        {
            Hashtable properties = StiReportEdit.GetAllProperties(listBoxElement as StiComponent);
            properties["meters"] = GetMetersHash();

            if ((string)parameters["command"] != "GetListBoxElementProperties")
            {
                properties["svgContent"] = StiEncodingHelper.Encode(StiDashboardsSvgHelper.SaveElementToString(listBoxElement, 1, 1, true));
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
            var meters = listBoxElement.FetchAllMeters();
            var metersItems = new Hashtable();

            foreach (IStiMeter meter in meters)
            {
                switch (meter.GetType().Name)
                {
                    case "StiKeyListBoxMeter":
                        {
                            metersItems["key"] = GetMeterHashItem(meter);
                            break;
                        }
                    case "StiNameListBoxMeter":
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
                case "GetListBoxElementProperties":
                    {
                        break;
                    }
            }

            callbackResult["elementProperties"] = GetListBoxElementJSProperties(parameters);
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
                        movingMeter = listBoxElement.GetKeyMeter();
                        if (!duplicateMeter)
                            listBoxElement.RemoveKeyMeter();
                        break;
                    }
                case "Name":
                    {
                        movingMeter = listBoxElement.GetNameMeter();
                        if (!duplicateMeter)
                            listBoxElement.RemoveNameMeter();
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
                            listBoxElement.AddKeyMeter(movingMeter);
                            break;
                        }
                    case "Name":
                        {
                            listBoxElement.AddNameMeter(movingMeter);
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
                        listBoxElement.CreateNewKeyMeter();
                        listBoxElement.GetKeyMeter().Expression = parameters["expression"] as string;
                        break;
                    }
                case "Name":
                    {
                        listBoxElement.CreateNewNameMeter();
                        listBoxElement.GetNameMeter().Expression = parameters["expression"] as string;
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
                        var keyMeter = listBoxElement.GetKeyMeter();
                        if (keyMeter != null) keyMeter.Expression = parameters["expression"] as string;
                        break;
                    }
                case "Name":
                    {
                        var nameMeter = listBoxElement.GetNameMeter();
                        if (nameMeter != null) nameMeter.Expression = parameters["expression"] as string;
                        break;
                    }
            }
        }

        private void RenameMeter(Hashtable parameters, Hashtable callbackResult)
        {
            if (parameters["newLabel"] != null)
            {
                switch (parameters["containerName"] as string)
                {
                    case "Key":
                        {
                            var meter = listBoxElement.GetKeyMeter();
                            if (meter != null) meter.Label = parameters["newLabel"] as string;
                            break;
                        }
                    case "Name":
                        {
                            var meter = listBoxElement.GetNameMeter();
                            if (meter != null) meter.Label = parameters["newLabel"] as string;
                            break;
                        }
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
                    dataCell = listBoxElement.Report.Dictionary.Variables[dataColumnObject["name"] as string] as IStiAppDataCell;
                }
                else
                {
                    var allColumns = dataColumnObject != null ? StiDictionaryHelper.GetColumnsByTypeAndNameOfObject(listBoxElement.Report, dataColumnObject) : null;
                    if (allColumns != null) dataCell = allColumns[dataColumnObject["name"] as string];
                }
            }

            switch (parameters["containerName"] as string)
            {
                case "Key":
                    {
                        oldLabel = listBoxElement.GetKeyMeter()?.Label;
                        listBoxElement.AddKeyMeter(dataCell);
                        break;
                    }
                case "Name":
                    {
                        oldLabel = listBoxElement.GetNameMeter()?.Label;
                        listBoxElement.AddNameMeter(dataCell);
                        break;
                    }
            }

            if (dataCell == null && !String.IsNullOrEmpty(oldLabel))
                StiElementChangedProcessor.ProcessElementChanging(listBoxElement, StiElementChangedArgs.CreateDeletingArgs(oldLabel));
        }

        private void SetPropertyValue(Hashtable parameters, Hashtable callbackResult)
        {
            var propertyName = parameters["propertyName"] as string;

            switch (propertyName)
            {
                case "SelectionMode":
                    {
                        listBoxElement.SelectionMode = (StiItemSelectionMode)Enum.Parse(typeof(StiItemSelectionMode), parameters["propertyValue"] as string);
                        break;
                    }
                case "ShowAllValue":
                    {
                        listBoxElement.ShowAllValue = Convert.ToBoolean(parameters["propertyValue"]);
                        break;
                    }
                case "ShowBlanks":
                    {
                        listBoxElement.ShowBlanks = Convert.ToBoolean(parameters["propertyValue"]);
                        break;
                    }
                case "ParentKey":
                    {
                        listBoxElement.SetParentKey(!String.IsNullOrEmpty(parameters["propertyValue"] as string) ? parameters["propertyValue"] as string : null);
                        break;
                    }
                case "Orientation":
                    {
                        listBoxElement.Orientation = (StiItemOrientation)Enum.Parse(typeof(StiItemOrientation), parameters["propertyValue"] as string);
                        break;
                    }
            }
        }        
        #endregion

        #region Constructor
        public StiListBoxElementHelper(IStiListBoxElement listBoxElement)
        {
            this.listBoxElement = listBoxElement;
        }
        #endregion   
    }
}
