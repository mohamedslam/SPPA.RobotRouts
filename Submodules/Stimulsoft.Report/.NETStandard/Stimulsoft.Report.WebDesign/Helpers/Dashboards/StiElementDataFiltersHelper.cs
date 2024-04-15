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

using Stimulsoft.Data.Engine;
using Stimulsoft.Data.Helpers;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dictionary;
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiElementDataFiltersHelper
    {
        #region Fields
        private IStiElement element;
        private IStiDataFilters dataFilterElement => element as IStiDataFilters;
        #endregion

        #region Helper Methods
        private Hashtable GetElementDataFiltersJSProperties(Hashtable parameters)
        {
            Hashtable properties = new Hashtable();
            properties["filters"] = GetFilters();
            properties["operation"] = GetCurrentOperation();
            properties["tableFiltersGroupsType"] = StiDataFilterRuleHelper.GetTableFiltersGroupsType(dataFilterElement.DataFilters);

            if ((string)parameters["command"] != "GetElementDataFiltersProperties")
            {
                properties["svgContent"] = StiEncodingHelper.Encode(StiDashboardsSvgHelper.SaveElementToString(element, 1, 1, true));
            }

            return properties;
        }

        private string GetDataFilterType(StiDataFilterRule dataFilter)
        {
            var dataColumn = StiDataExpressionHelper.GetDataColumnFromExpression(element.Page as IStiDashboard, dataFilter?.Path);
            var dataType = dataColumn == null ? typeof(object) : dataColumn.GetDataType();

            return StiDataFiltersHelper.TypeToString(dataType);
        }

        private ArrayList GetFilters()
        {
            ArrayList filters = new ArrayList();
            var filterElement = element as IStiDataFilters;

            if (filterElement != null)
            {
                foreach (var filterRule in filterElement.DataFilters)
                {
                    var filterItem = StiDataFiltersHelper.FilterRuleItem(filterRule);
                    filterItem["label"] = filterRule.GetStringRepresentation();
                    filterItem["type"] = GetDataFilterType(filterRule);
                    filterItem["values"] = GetDataValuesFromDataPath(filterRule.Path);

                    filters.Add(filterItem);
                }
            }

            return filters;
        }

        private string[] GetDataValuesFromDataPath(string path)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                if (string.IsNullOrWhiteSpace(path))
                    return null;

                path = StiExpressionHelper.RemoveFunction(path);

                if (path.StartsWith("[") && path.EndsWith("]"))
                    path = path.Substring(1, path.Length - 2);

                var dataSource = StiDataColumn.GetDataSourceFromDataColumn(element.Page.Report.Dictionary, path);

                if (dataSource == null)
                    return null;

                else if (dataSource is StiVirtualSource)
                    dataSource.Dictionary.ConnectVirtualDataSources();

                else if (dataSource is StiDataTransformation)
                    dataSource.Dictionary.ConnectDataTransformations();
                else
                    dataSource.Dictionary.Connect();

                var values = StiDataColumn.GetDataListFromDataColumn(element.Page.Report.Dictionary, path, 100)
                    .Where(v => v != null)
                    .Select(v => StiDataFiltersHelper.ToFilterString(v))
                    .Distinct()
                    .OrderBy(v => v)
                    .Take(100)
                    .ToArray();

                dataSource.Connect(false);

                return values;
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
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
                case "InsertFilters":
                    {
                        InsertFilters(parameters, callbackResult);
                        break;
                    }                
                case "RemoveFilter":
                    {
                        RemoveFilter(parameters, callbackResult);
                        break;
                    }
                case "MoveFilter":
                case "MoveAndDuplicateFilter":
                    {
                        MoveFilter(parameters, callbackResult);
                        break;
                    }
                case "SetPropertyValue":
                    {
                        SetPropertyValue(parameters, callbackResult);
                        break;
                    }
                case "GetElementDataFiltersProperties":
                    {
                        break;
                    }
            }

            callbackResult["elementProperties"] = GetElementDataFiltersJSProperties(parameters);
        }

        private void CreateNewItem(Hashtable parameters, Hashtable callbackResult)
        {
            var newFilter = new StiDataFilterRule();
            newFilter.Operation = GetCurrentOperation();
            dataFilterElement.DataFilters.Add(newFilter);
        }

        private StiDataFilterOperation GetCurrentOperation()
        {
            var isFilterOperationOr = dataFilterElement?.DataFilters?.Any(f => f.Operation == StiDataFilterOperation.OR);
            if (isFilterOperationOr.GetValueOrDefault())
                return StiDataFilterOperation.OR;
            else
                return StiDataFilterOperation.AND;
        }

        private void EditField(Hashtable parameters, Hashtable callbackResult)
        {
            var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
            if (itemIndex < dataFilterElement.DataFilters.Count)
                dataFilterElement.DataFilters[itemIndex].Path = parameters["expression"] as string;
        }

        private void MoveFilter(Hashtable parameters, Hashtable callbackResult)
        {
            var duplicateFilter = (string)parameters["command"] == "MoveAndDuplicateFilter";
            var fromIndex = Convert.ToInt32(parameters["fromIndex"]);
            var toIndex = Convert.ToInt32(parameters["toIndex"]);

            if (dataFilterElement != null && fromIndex < dataFilterElement.DataFilters.Count && toIndex < dataFilterElement.DataFilters.Count)
            {
                var movingFilter = dataFilterElement.DataFilters[fromIndex];
                if (duplicateFilter)
                {
                    var cloneFilter = (movingFilter as ICloneable).Clone();
                    movingFilter = cloneFilter as StiDataFilterRule;
                }
                else
                {
                    dataFilterElement.DataFilters.RemoveAt(fromIndex);
                }

                if (toIndex >= 0 && toIndex < dataFilterElement.DataFilters.Count)
                    dataFilterElement.DataFilters.Insert(toIndex, movingFilter);
                else
                    dataFilterElement.DataFilters.Add(movingFilter);
            }
        }

        private void RemoveFilter(Hashtable parameters, Hashtable callbackResult)
        {
            var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
            if (dataFilterElement != null && itemIndex < dataFilterElement.DataFilters.Count)
                dataFilterElement.DataFilters.RemoveAt(itemIndex);
        }

        private void InsertFilters(Hashtable parameters, Hashtable callbackResult)
        {
            var draggedItem = parameters["draggedItem"] as Hashtable;
            var draggedItemObject = draggedItem["itemObject"] as Hashtable;
            var draggedColumns = new StiDataColumnsCollection();

            var allColumns = StiDictionaryHelper.GetColumnsByTypeAndNameOfObject(element.Report, draggedItem);
            if (allColumns != null)
            {
                if (draggedItemObject["typeItem"] as string == "Column")
                {
                    var draggedColumn = allColumns[draggedItemObject["name"] as string];
                    if (draggedColumn != null) draggedColumns.Add(draggedColumn);
                }
                else
                {
                    draggedColumns = allColumns;
                }
            }

            foreach (StiDataColumn dataColumn in draggedColumns)
            {
                var newFilter = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard", "Helpers.StiMeterHelper+DataFilter", "GetFilter",
                        new object[] { dataColumn }, new Type[] { dataColumn.GetType() }) as StiDataFilterRule;

                newFilter.Operation = GetCurrentOperation();

                if (dataFilterElement != null && newFilter != null)
                {
                    var index = parameters["insertIndex"] != null ? Convert.ToInt32(parameters["insertIndex"]) : -1;
                    if (index >= 0 && index < dataFilterElement.DataFilters.Count)
                        dataFilterElement.DataFilters.Insert(index, newFilter);
                    else
                        dataFilterElement.DataFilters.Add(newFilter);
                }
            }
        }

        private void SetPropertyValue(Hashtable parameters, Hashtable callbackResult)
        {
            var propertyName = parameters["propertyName"] as string;

            if (propertyName == "Operation")
            {
                dataFilterElement.DataFilters.ForEach(f =>
                {
                    f.Operation = (StiDataFilterOperation)Enum.Parse(typeof(StiDataFilterOperation), parameters["propertyValue"] as string);
                });
            }
            else if (parameters["itemIndex"] != null)
            {
                var itemIndex = Convert.ToInt32(parameters["itemIndex"]);

                if (dataFilterElement != null && itemIndex >= 0 && itemIndex < dataFilterElement.DataFilters.Count)
                {
                    var filter = dataFilterElement.DataFilters[itemIndex];

                    switch (propertyName)
                    {
                        case "Condition":
                            {
                                filter.Condition = (StiDataFilterCondition)Enum.Parse(typeof(StiDataFilterCondition), parameters["propertyValue"] as string);
                                break;
                            }
                        case "Value":
                        case "ValueDate":
                        case "ValueExp":
                            {
                                filter.Value = parameters["propertyValue"] as string;
                                break;
                            }
                        case "Value2":
                        case "Value2Date":
                        case "Value2Exp":
                            {
                                filter.Value2 = parameters["propertyValue"] as string;
                                break;
                            }
                        case "Expression":
                            {
                                filter.IsExpression = Convert.ToBoolean(parameters["propertyValue"]);
                                break;
                            }
                        case "IsEnabled":
                            {
                                filter.IsEnabled = Convert.ToBoolean(parameters["propertyValue"]);
                                break;
                            }
                    }
                }
            }
        }
        #endregion

        #region Constructor
        public StiElementDataFiltersHelper(IStiElement element)
        {
            this.element = element;
        }
        #endregion   
    }
}
