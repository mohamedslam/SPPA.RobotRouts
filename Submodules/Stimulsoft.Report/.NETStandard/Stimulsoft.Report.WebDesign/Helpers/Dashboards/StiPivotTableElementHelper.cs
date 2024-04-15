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
using Stimulsoft.Data.Helpers;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Export;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiPivotTableElementHelper
    {
        #region Fields
        private IStiPivotTableElement pivotTableElement;
        #endregion

        #region Helper Methods
        private Hashtable GetPivotTableElementJSProperties(Hashtable parameters)
        {
            Hashtable properties = StiReportEdit.GetAllProperties(pivotTableElement as StiComponent);
            properties["meters"] = GetMetersHash();

            if ((string)parameters["command"] != "GetPivotTableElementProperties")
            {
                properties["svgContent"] = StiEncodingHelper.Encode(StiDashboardsSvgHelper.SaveElementToString(pivotTableElement, 1, 1, true));
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
            meterItem["expression"] = meter.Expression != null ? StiEncodingHelper.Encode(meter.Expression) : string.Empty;
            meterItem["functions"] = StiTableElementHelper.GetMeterFunctions(meter, pivotTableElement.Page as IStiDashboard);
            meterItem["currentFunction"] = StiExpressionHelper.GetFunction(meter.Expression);
            meterItem["textFormat"] = StiTextFormatHelper.GetTextFormatItem((meter as IStiTextFormat).TextFormat);
            

            if (meter is IStiHorAlignment)
                meterItem["horAlignment"] = (meter as IStiHorAlignment).HorAlignment;

            if (meter is IStiDataTopN)
                meterItem["topN"] = StiReportEdit.GetTopNProperty((meter as IStiDataTopN).TopN);

            if (meter is IStiPivotColumn) {
                meterItem["showTotal"] = ((IStiPivotColumn)meter).ShowTotal;
                meterItem["totalLabel"] = ((IStiPivotColumn)meter).TotalLabel;
                meterItem["sortDirection"] = ((IStiPivotColumn)meter).StrSortDirection;
                meterItem["expandExpression"] = ((IStiPivotColumn)meter).ExpandExpression;
            }

            if (meter is IStiPivotRow)
            {
                meterItem["showTotal"] = ((IStiPivotRow)meter).ShowTotal;
                meterItem["totalLabel"] = ((IStiPivotRow)meter).TotalLabel;
                meterItem["sortDirection"] = ((IStiPivotRow)meter).StrSortDirection;
                meterItem["expandExpression"] = ((IStiPivotRow)meter).ExpandExpression;
            }

            if (meter is IStiPivotSummary)
            {
                meterItem["hideZeros"] = ((IStiPivotSummary)meter).HideZeros;
            }

            if (meter is IStiTableColumnSize) {
                var size = ((IStiTableColumnSize)meter).Size;
                meterItem["size.Width"] = size.Width;
                meterItem["size.MaxWidth"] = size.MaxWidth;
                meterItem["size.MinWidth"] = size.MinWidth;
                meterItem["size.WordWrap"] = size.WordWrap;
            }   

            return meterItem;
        }

        private Hashtable GetMetersHash()
        {
            var meters = pivotTableElement.FetchAllMeters();
            var metersItems = new Hashtable();
            metersItems["columns"] = new ArrayList();
            metersItems["rows"] = new ArrayList();
            metersItems["summaries"] = new ArrayList();

            foreach (IStiMeter meter in meters)
            {
                if (meter is IStiPivotColumn)
                    ((ArrayList)metersItems["columns"]).Add(GetMeterHashItem(meter));

                else if (meter is IStiPivotRow)
                    ((ArrayList)metersItems["rows"]).Add(GetMeterHashItem(meter));

                else if (meter is IStiPivotSummary)
                    ((ArrayList)metersItems["summaries"]).Add(GetMeterHashItem(meter));
            }

            return metersItems;
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
                case "SetExpression":
                    {
                        SetExpression(parameters, callbackResult);
                        break;
                    }
                case "SetTopN":
                    {
                        SetTopN(parameters, callbackResult);
                        break;
                    }
                case "SetFunction":
                    {
                        SetFunction(parameters, callbackResult);
                        break;
                    }
                case "NewItem":
                    {
                        CreateNewItem(parameters, callbackResult);
                        break;
                    }
                case "SwapColumnsRows":
                    {
                        SwapColumnsRows(parameters, callbackResult);
                        break;
                    }
                case "SwapSummaryDirection":
                    {
                        SwapSummaryDirection(parameters, callbackResult);
                        break;
                    }
                case "SetPropertyValue":
                    {
                        SetPropertyValue(parameters, callbackResult);
                        break;
                    }
                case "GetPivotTableElementProperties":
                    {
                        break;
                    }
            }

            callbackResult["elementProperties"] = GetPivotTableElementJSProperties(parameters);
        }

        private void SetPropertyValue(Hashtable parameters, Hashtable callbackResult)
        {            
            var meter = GetMeterFromContainer(parameters["containerName"] as string, Convert.ToInt32(parameters["itemIndex"]));
            if (meter != null)
            {
                var propertyName = parameters["propertyName"] as string;
                if (propertyName == "Expression")
                    meter.Expression = StiEncodingHelper.DecodeString(parameters["propertyValue"] as string);
                else if (propertyName == "TextFormat")
                    (meter as IStiTextFormat).TextFormat = StiTextFormatHelper.GetFormatService(parameters["propertyValue"] as Hashtable);                
                else
                    StiReportEdit.SetPropertyValue(pivotTableElement.Report, propertyName, meter, parameters["propertyValue"]);

                (meter as IStiMeterRules)?.CheckRules();
            }
        }

        private IStiMeter GetMeterFromContainer(string containerName, int index)
        {
            switch (containerName)
            {
                case "columns": return pivotTableElement.GetColumnByIndex(index);
                case "rows": return pivotTableElement.GetRowByIndex(index);
                case "summaries": return pivotTableElement.GetSummaryByIndex(index);
            }

            return null;
        }

        private void SetExpression(Hashtable parameters, Hashtable callbackResult)
        {
            var meter = GetMeterFromContainer(parameters["containerName"] as string, Convert.ToInt32(parameters["itemIndex"]));
            if (meter != null)
            {
                meter.Expression = StiEncodingHelper.DecodeString(parameters["expressionValue"] as string);
            }
        }

        private void RenameMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var meter = GetMeterFromContainer(parameters["containerName"] as string, Convert.ToInt32(parameters["itemIndex"]));
            if (meter != null && parameters["newLabel"] != null)
            {
                StiElementChangedProcessor.ProcessElementChanging(pivotTableElement, StiElementChangedArgs.CreateRenamingArgs(meter.Label, parameters["newLabel"] as string));
                meter.Label = parameters["newLabel"] as string;
            }
        }

        private void SetTopN(Hashtable parameters, Hashtable callbackResult)
        {
            var meter = GetMeterFromContainer(parameters["containerName"] as string, Convert.ToInt32(parameters["itemIndex"]));
            if (meter != null)
            {
                StiReportEdit.SetTopNProperty(meter, parameters["topN"]);
            }
        }

        private void SetFunction(Hashtable parameters, Hashtable callbackResult)
        {
            var meter = GetMeterFromContainer(parameters["containerName"] as string, Convert.ToInt32(parameters["itemIndex"]));
            if (meter != null)
            {
                var function = parameters["function"] as string;
                if (function.ToLowerInvariant() == "percentofgrandtotal")
                {
                    var textFormatMeter = meter as IStiTextFormat;
                    if (textFormatMeter?.TextFormat != null && !(textFormatMeter is StiPercentageFormatService))
                        textFormatMeter.TextFormat = new StiPercentageFormatService();
                }               

                meter.Expression = StiExpressionHelper.ReplaceFunction(meter.Expression, function);
            }
        }

        private void RemoveAllMeters(Hashtable parameters, Hashtable callbackResult)
        {
            var containerName = parameters["containerName"] as string;
            switch (containerName)
            {
                case "columns":
                    {
                        pivotTableElement.RemoveAllColumns();
                        break;
                    }
                case "rows":
                    {
                        pivotTableElement.RemoveAllRows();
                        break;
                    }

                case "summaries":
                    {
                        pivotTableElement.RemoveAllSummaries();
                        break;
                    }
            }

            StiElementChangedProcessor.ProcessElementChanging(pivotTableElement, StiElementChangedArgs.CreateClearingAllArgs());
        }

        private void RemoveMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var containerName = parameters["containerName"] as string;
            var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
            var oldLabel = string.Empty;

            switch (containerName)
            {
                case "columns":
                    {
                        oldLabel = pivotTableElement.GetColumnByIndex(itemIndex)?.Label;
                        pivotTableElement.RemoveColumn(itemIndex);
                        break;
                    }
                case "rows":
                    {
                        oldLabel = pivotTableElement.GetRowByIndex(itemIndex)?.Label;
                        pivotTableElement.RemoveRow(itemIndex);
                        break;
                    }

                case "summaries":
                    {
                        oldLabel = pivotTableElement.GetSummaryByIndex(itemIndex)?.Label;
                        pivotTableElement.RemoveSummary(itemIndex);
                        break;
                    }
            }

            if (!String.IsNullOrEmpty(oldLabel))
                StiElementChangedProcessor.ProcessElementChanging(pivotTableElement, StiElementChangedArgs.CreateDeletingArgs(oldLabel));
        }

        private void MoveMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var duplicateMeter = (string)parameters["command"] == "MoveAndDuplicateMeter";
            var fromIndex = Convert.ToInt32(parameters["fromIndex"]);
            var toIndex = Convert.ToInt32(parameters["toIndex"]);
            var toContainerName = parameters["toContainerName"] as string;
            var fromContainerName = parameters["fromContainerName"] as string;
            IStiMeter movingMeter = null;

            #region Get and Remove meter
            switch (fromContainerName)
            {
                case "columns":
                    {
                        movingMeter = pivotTableElement.GetColumnByIndex(fromIndex);
                        if (movingMeter != null && !duplicateMeter)
                            pivotTableElement.RemoveColumn(fromIndex);
                        break;
                    }
                case "rows":
                    {
                        movingMeter = pivotTableElement.GetRowByIndex(fromIndex);
                        if (movingMeter != null && !duplicateMeter)
                            pivotTableElement.RemoveRow(fromIndex);
                        break;
                    }

                case "summaries":
                    {
                        movingMeter = pivotTableElement.GetSummaryByIndex(fromIndex);
                        if (movingMeter != null && !duplicateMeter)
                            pivotTableElement.RemoveSummary(fromIndex);
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
                    case "columns":
                        {
                            var columnMeter = pivotTableElement.GetColumn(movingMeter);
                            if (columnMeter != null)
                                pivotTableElement.InsertColumn(toIndex, columnMeter);
                            break;
                        }
                    case "rows":
                        {
                            var rowMeter = pivotTableElement.GetRow(movingMeter);
                            if (rowMeter != null)
                                pivotTableElement.InsertRow(toIndex, rowMeter);
                            break;
                        }

                    case "summaries":
                        {
                            var summaryMeter = pivotTableElement.GetSummary(movingMeter);
                            if (summaryMeter != null)
                                pivotTableElement.InsertSummary(toIndex, summaryMeter);
                            break;
                        }
                }
            }
            #endregion
        }

        private void DuplicateMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var containerName = parameters["containerName"] as string;
            var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
            callbackResult["insertIndex"] = itemIndex + 1;

            switch (containerName)
            {
                case "columns":
                    {
                        var pivotColumn = pivotTableElement.GetColumnByIndex(itemIndex);
                        if (pivotColumn != null)
                        {
                            var cloneColumn = (pivotColumn as ICloneable).Clone();
                            pivotTableElement.InsertColumn(itemIndex + 1, cloneColumn as IStiMeter);
                        }
                        break;
                    }
                case "rows":
                    {
                        var pivotRow = pivotTableElement.GetRowByIndex(itemIndex);
                        if (pivotRow != null)
                        {
                            var cloneRow = (pivotRow as ICloneable).Clone();
                            pivotTableElement.InsertRow(itemIndex + 1, cloneRow as IStiMeter);
                        }
                        break;
                    }

                case "summaries":
                    {
                        var pivotSummary = pivotTableElement.GetSummaryByIndex(itemIndex);
                        if (pivotSummary != null)
                        {
                            var cloneSummary = (pivotSummary as ICloneable).Clone();
                            pivotTableElement.InsertSummary(itemIndex + 1, cloneSummary as IStiMeter);
                        }
                        break;
                    }
            }
        }

        private void InsertMeters(Hashtable parameters, Hashtable callbackResult)
        {
            var containerName = parameters["containerName"] as string;
            var draggedItem = parameters["draggedItem"] as Hashtable;
            var draggedItemObject = draggedItem["itemObject"] as Hashtable;
            var typeDraggedItem = draggedItemObject["typeItem"] as string;
            var draggedColumns = new List<IStiAppDataCell>();
            var insertIndex = parameters["insertIndex"] != null ? Convert.ToInt32(parameters["insertIndex"]) : -1;

            if (typeDraggedItem == "Variable")
            {
                var variableDataCell = pivotTableElement.Report.Dictionary.Variables[draggedItemObject["name"] as string] as IStiAppDataCell;
                if (variableDataCell != null) draggedColumns.Add(variableDataCell);
            }
            else
            {
                var allColumns = StiDictionaryHelper.GetColumnsByTypeAndNameOfObject(pivotTableElement.Report, draggedItem);
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
                switch (containerName)
                {
                    case "columns":
                        {
                            var pivotColumn = pivotTableElement.GetColumn(dataColumn);
                            if (pivotColumn != null)
                                pivotTableElement.InsertColumn(insertIndex, pivotColumn);
                            break;
                        }
                    case "rows":
                        {
                            var pivotRow = pivotTableElement.GetRow(dataColumn);
                            if (pivotRow != null)
                                pivotTableElement.InsertRow(insertIndex, pivotRow);
                            break;
                        }

                    case "summaries":
                        {
                            var pivotSummary = pivotTableElement.GetSummary(dataColumn);
                            if (pivotSummary != null)
                                pivotTableElement.InsertSummary(insertIndex, pivotSummary);
                            break;
                        }
                }
            }
        }

        private void CreateNewItem(Hashtable parameters, Hashtable callbackResult)
        {
            switch (parameters["containerName"] as string)
            {
                case "columns":
                    {
                        pivotTableElement.CreateNewColumn();
                        break;
                    }
                case "rows":
                    {
                        pivotTableElement.CreateNewRow();
                        break;
                    }
                case "summaries":
                    {
                        pivotTableElement.CreateNewSummary();
                        break;
                    }
            }
        }

        private void SwapColumnsRows(Hashtable parameters, Hashtable callbackResult)
        {
            var meters = pivotTableElement.FetchAllMeters();
            var columns = new List<IStiMeter>();
            var rows = new List<IStiMeter>();
            var summaries = new List<IStiMeter>();

            foreach (var meter in meters)
            {
                if (meter is IStiPivotColumn)
                    columns.Add(meter);
                else if (meter is IStiPivotRow)
                    rows.Add(meter);
                else if (meter is IStiPivotSummary)
                    summaries.Add(meter);
            }

            pivotTableElement.RemoveAllColumns();
            pivotTableElement.RemoveAllRows();
            pivotTableElement.RemoveAllSummaries();

            foreach (var column in columns)
                pivotTableElement.InsertRow(-1, pivotTableElement.GetRow(column));

            foreach (var row in rows)
                pivotTableElement.InsertColumn(-1, pivotTableElement.GetColumn(row));

            foreach (var summary in summaries)
                pivotTableElement.InsertSummary(-1, summary);
        }

        private void SwapSummaryDirection(Hashtable parameters, Hashtable callbackResult)
        {
            pivotTableElement.SummaryDirection = pivotTableElement.SummaryDirection == CrossTab.Core.StiSummaryDirection.LeftToRight ? CrossTab.Core.StiSummaryDirection.UpToDown : CrossTab.Core.StiSummaryDirection.LeftToRight;
        }
        #endregion

        #region Constructor
        public StiPivotTableElementHelper(IStiPivotTableElement pivotTableElement)
        {
            this.pivotTableElement = pivotTableElement;
        }
        #endregion   
    }
}
