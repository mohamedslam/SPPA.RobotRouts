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
using System.Linq;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiTreeViewElementHelper
    {
        #region Fields
        private IStiTreeViewElement treeViewElement;
        #endregion

        #region Helper Methods
        private Hashtable GetTreeViewElementJSProperties(Hashtable parameters)
        {
            Hashtable properties = StiReportEdit.GetAllProperties(treeViewElement as StiComponent);
            properties["meters"] = GetMetersHash();

            if ((string)parameters["command"] != "GetTreeViewElementProperties")
            {
                properties["svgContent"] = StiEncodingHelper.Encode(StiDashboardsSvgHelper.SaveElementToString(treeViewElement, 1, 1, true));
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
            var meters = treeViewElement.FetchAllMeters();
            var metersItems = new Hashtable();
            metersItems["keys"] = new ArrayList();

            foreach (IStiMeter meter in meters)
            {
                switch (meter.GetType().Name)
                {
                    case "StiKeyTreeViewMeter":
                        {
                            ((ArrayList)metersItems["keys"]).Add(GetMeterHashItem(meter));
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
                        RemoveAllMeters(parameters, callbackResult);
                        break;
                    }
                case "MoveMeter":
                case "MoveAndDuplicateMeter":
                    {
                        MoveMeter(parameters, callbackResult);
                        break;
                    }
                case "DuplicateMeter":
                    {
                        DuplicateMeter(parameters, callbackResult);
                        break;
                    }
                case "SetPropertyValue":
                    {
                        SetPropertyValue(parameters, callbackResult);
                        break;
                    }
                case "GetTreeViewElementProperties":
                    {
                        break;
                    }
            }

            callbackResult["elementProperties"] = GetTreeViewElementJSProperties(parameters);
        }

        private void RemoveAllMeters(Hashtable parameters, Hashtable callbackResult)
        {
            treeViewElement.RemoveAllKeyMeters();
            StiElementChangedProcessor.ProcessElementChanging(treeViewElement, StiElementChangedArgs.CreateClearingAllArgs());
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
                var variableDataCell = treeViewElement.Report.Dictionary.Variables[draggedItemObject["name"] as string] as IStiAppDataCell;
                if (variableDataCell != null) draggedColumns.Add(variableDataCell);
            }
            else
            {
                var allColumns = StiDictionaryHelper.GetColumnsByTypeAndNameOfObject(treeViewElement.Report, draggedItem);
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
                var keyMeter = treeViewElement.GetKeyMeter(dataColumn);
                if (keyMeter != null)
                {
                    treeViewElement.InsertKeyMeter(insertIndex, keyMeter);
                }
            }
        }

        private void CreateNewItem(Hashtable parameters, Hashtable callbackResult)
        {
            treeViewElement.AddNewKeyMeter().Expression = parameters["expression"] as string;
        }

        private void EditField(Hashtable parameters, Hashtable callbackResult)
        {
            var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
            var keyMeter = treeViewElement.GetKeyMeterByIndex(itemIndex);
            if (keyMeter != null) keyMeter.Expression = parameters["expression"] as string;
        }

        private void RemoveMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var oldLabel = treeViewElement.GetKeyMeterByIndex(Convert.ToInt32(parameters["itemIndex"]))?.Label;
            if (!String.IsNullOrEmpty(oldLabel))
                StiElementChangedProcessor.ProcessElementChanging(treeViewElement, StiElementChangedArgs.CreateDeletingArgs(oldLabel));

            treeViewElement.RemoveKeyMeter(Convert.ToInt32(parameters["itemIndex"]));
        }

        private void RenameMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
            var keyMeter = treeViewElement.GetKeyMeterByIndex(itemIndex);
            if (keyMeter != null && parameters["newLabel"] != null)
            {
                StiElementChangedProcessor.ProcessElementChanging(treeViewElement, StiElementChangedArgs.CreateRenamingArgs(keyMeter.Label, parameters["newLabel"] as string));
                keyMeter.Label = parameters["newLabel"] as string;
            }
        }

        private void MoveMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var duplicateMeter = (string)parameters["command"] == "MoveAndDuplicateMeter";
            var fromIndex = Convert.ToInt32(parameters["fromIndex"]);
            var toIndex = Convert.ToInt32(parameters["toIndex"]);

            var movingMeter = treeViewElement.GetKeyMeterByIndex(fromIndex);
            if (movingMeter != null)
            {
                if (duplicateMeter)
                {
                    var cloneMeter = (movingMeter as ICloneable).Clone();
                    movingMeter = cloneMeter as IStiMeter;
                }
                else
                {
                    treeViewElement.RemoveKeyMeter(fromIndex);
                }
                treeViewElement.InsertKeyMeter(toIndex, movingMeter);
            }
        }

        private void DuplicateMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
            callbackResult["insertIndex"] = itemIndex + 1;

            var keyMeter = treeViewElement.GetKeyMeterByIndex(itemIndex);
            if (keyMeter != null)
            {
                var cloneKeyMeter = (keyMeter as ICloneable).Clone();
                treeViewElement.InsertKeyMeter(itemIndex + 1, cloneKeyMeter as IStiMeter);
            }
        }

        private void SetPropertyValue(Hashtable parameters, Hashtable callbackResult)
        {
            var propertyName = parameters["propertyName"] as string;

            switch (propertyName)
            {
                case "SelectionMode":
                    {
                        treeViewElement.SelectionMode = (StiItemSelectionMode)Enum.Parse(typeof(StiItemSelectionMode), parameters["propertyValue"] as string);
                        break;
                    }
                case "ShowAllValue":
                    {
                        treeViewElement.ShowAllValue = Convert.ToBoolean(parameters["propertyValue"]);
                        break;
                    }
                case "ShowBlanks":
                    {
                        treeViewElement.ShowBlanks = Convert.ToBoolean(parameters["propertyValue"]);
                        break;
                    }
                case "ParentKey":
                    {
                        treeViewElement.SetParentKey(!String.IsNullOrEmpty(parameters["propertyValue"] as string) ? parameters["propertyValue"] as string : null);
                        break;
                    }
            }
        }
        #endregion

        #region Constructor
        public StiTreeViewElementHelper(IStiTreeViewElement treeViewElement)
        {
            this.treeViewElement = treeViewElement;
        }
        #endregion   
    }
}
