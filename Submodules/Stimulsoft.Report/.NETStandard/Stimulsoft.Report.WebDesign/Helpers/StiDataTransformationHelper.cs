#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using Stimulsoft.Base.Helpers;
using Stimulsoft.Data.Engine;
using Stimulsoft.Data.Exceptions;
using Stimulsoft.Data.Expressions.NCalc;
using Stimulsoft.Data.Extensions;
using Stimulsoft.Data.Functions;
using Stimulsoft.Data.Helpers;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Web.Helpers.Dashboards;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiDataTransformationHelper
    {
        #region Items
        #region Column Item
        public static Hashtable ColumnItem(StiDataTransformationColumn column, StiDictionary dictionary = null, string originalType = null)
        {
            Hashtable columnItem = new Hashtable();
            columnItem["typeItem"] = "Column";
            columnItem["typeIcon"] = StiDictionaryHelper.GetIconTypeForColumn(column).ToString();
            columnItem["isDataTransformationColumn"] = true;
            columnItem["type"] = StiDataFiltersHelper.TypeToString(column.Type);
            columnItem["originalType"] = originalType != null ? originalType : columnItem["type"];
            columnItem["key"] = column.Key;
            columnItem["name"] = column.Name;
            columnItem["correctName"] = StiNameValidator.CorrectName(column.Name);
            columnItem["alias"] = column.Alias;
            columnItem["nameInSource"] = column.NameInSource;
            columnItem["mode"] = column.Mode;
            columnItem["expression"] = !string.IsNullOrEmpty(column.Expression) ? StiEncodingHelper.Encode(column.Expression) : string.Empty;
            columnItem["functions"] = GetFuncs(column, dictionary);
            columnItem["currentFunction"] = StiExpressionHelper.GetFunction(column.Expression);

            return columnItem;
        }
        #endregion
                
        #region ActionRule Item
        private static Hashtable ActionRuleItem(StiDataActionRule actionRule)
        {
            Hashtable actionRuleItem = new Hashtable();
            actionRuleItem["typeItem"] = "ActionRule";
            actionRuleItem["key"] = actionRule.Key;
            actionRuleItem["type"] = actionRule.Type;
            actionRuleItem["path"] = actionRule.Path;
            actionRuleItem["startIndex"] = actionRule.StartIndex;
            actionRuleItem["rowsCount"] = actionRule.RowsCount;
            actionRuleItem["initialValue"] = actionRule.InitialValue;
            actionRuleItem["valueFrom"] = actionRule.ValueFrom;
            actionRuleItem["valueTo"] = actionRule.ValueTo;
            actionRuleItem["matchCase"] = actionRule.MatchCase;
            actionRuleItem["matchWholeWord"] = actionRule.MatchWholeWord;
            actionRuleItem["priority"] = actionRule.Priority;

            return actionRuleItem;
        }
        #endregion
        #endregion

        #region Helper Methods
        private static ArrayList GetFuncs(StiDataTransformationColumn column, StiDictionary dictionary)
        {
            if (column == null) return null;
            string[] funcs = null;

            var dataTransformation = new StiDataTransformation()
            {
                Dictionary = dictionary
            };

            if (column.Mode == StiDataTransformationMode.Dimension)
            {
                if (StiDataExpressionHelper.IsDateDataColumnInExpression(dataTransformation, column.Expression))
                    funcs = Funcs.GetDateDimensionFunctions().ToArray<string>();
            }
            else
            {
                if (StiDataExpressionHelper.IsNumericDataColumnInExpression(dataTransformation, column.Expression))
                    funcs = Funcs.GetAggregateMeasureFunctions().ToArray<string>();
                else
                    funcs = Funcs.GetCommonMeasureFunctions().ToArray<string>();
            }

            return funcs != null ? new ArrayList(funcs) : null;
        }

        public static StiDataTransformationColumn CreateTransformationColumnFromDataColumn(StiDataColumn dataColumn)
        {
            var tableColumn = new StiDataTransformationColumn
            {
                Key = StiKeyHelper.GenerateKey(),
                Name = dataColumn.Name,
                Alias = dataColumn.Alias,
                Mode = dataColumn.Type.IsNumericType() ? StiDataTransformationMode.Measure : StiDataTransformationMode.Dimension,
                Type = dataColumn.Type,
                Expression = ToExpression(dataColumn)
            };

            if (tableColumn.Mode == StiDataTransformationMode.Measure)
                tableColumn.Expression = ToSumExpression(dataColumn);

            return tableColumn;
        }

        private static string ToSumExpression(StiDataColumn dataColumn)
        {
            return $"Sum({ToExpression(dataColumn)})";
        }

        private static string ToExpression(StiDataColumn dataColumn)
        {
            return Funcs.ToExpression(dataColumn.DataSource.Name, dataColumn.Name);
        }

        private static StiDataTransformationColumn GetColumnFromJSColumnObject(Hashtable columnObject)
        {
            return new StiDataTransformationColumn()
            {
                Key = columnObject["key"] as string,
                Name = columnObject["name"] as string,
                Alias = columnObject["alias"] as string,
                Mode = (StiDataTransformationMode)Enum.Parse(typeof(StiDataTransformationMode), columnObject["mode"] as string),
                Type = StiDictionaryHelper.GetTypeFromString(columnObject["type"] as string),
                Expression = StiEncodingHelper.DecodeString(columnObject["expression"] as string)
            };
        }

        private static StiDataSortRule GetSortRuleFromJSSortRuleObject(Hashtable sortRuleObject)
        {
            return new StiDataSortRule()
            {
                Key = sortRuleObject["key"] as string,
                Direction = (StiDataSortDirection)Enum.Parse(typeof(StiDataSortDirection), sortRuleObject["direction"] as string)
            };
        }

        private static StiDataFilterRule GetFilterRuleFromJSFilterRuleObject(Hashtable filterRuleObject)
        {
            return new StiDataFilterRule()
            {
                Key = filterRuleObject["key"] as string,
                Path = filterRuleObject["path"] as string,
                Condition = (StiDataFilterCondition)Enum.Parse(typeof(StiDataFilterCondition), filterRuleObject["condition"] as string),
                Value = filterRuleObject["value"] as string,
                Value2 = filterRuleObject["value2"] as string,
                IsEnabled = Convert.ToBoolean(filterRuleObject["isEnabled"]),
                IsExpression = Convert.ToBoolean(filterRuleObject["isExpression"])
            };
        }

        private static StiDataActionRule GetActionRuleFromJSActionRuleObject(Hashtable actionRuleObject)
        {
            return new StiDataActionRule()
            {
                Key = actionRuleObject["key"] as string,
                Type = (StiDataActionType)Enum.Parse(typeof(StiDataActionType), actionRuleObject["type"] as string),
                Path = actionRuleObject["path"] as string,
                StartIndex = actionRuleObject["startIndex"] != null ? Convert.ToInt32(actionRuleObject["startIndex"]) : 0,
                RowsCount = actionRuleObject["rowsCount"] != null ? Convert.ToInt32(actionRuleObject["rowsCount"]) : -1,
                InitialValue = actionRuleObject["initialValue"] as string != string.Empty ? actionRuleObject["initialValue"] as string : null,
                ValueFrom = actionRuleObject["valueFrom"] as string != string.Empty ? actionRuleObject["valueFrom"] as string : null,
                ValueTo = actionRuleObject["valueTo"] as string != string.Empty ? actionRuleObject["valueTo"] as string : null,
                MatchCase = Convert.ToBoolean(actionRuleObject["matchCase"]),
                MatchWholeWord = Convert.ToBoolean(actionRuleObject["matchWholeWord"]),
                Priority = (StiDataActionPriority)Enum.Parse(typeof(StiDataActionPriority), actionRuleObject["priority"] as string)
            };
        }

        public static void GetViewQueryContent(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var dashboard = report.Pages[param["dashboardName"] as string] as IStiDashboard;
            ArrayList resultData = new ArrayList();

            if (dashboard != null)
            {
                #region Data
                var dataTable = StiElementDataCache.GetOrCreate(dashboard);
                if (dataTable != null)
                {
                    var netTable = dataTable.ToNetTable();

                    if (param.Contains("sort"))
                    {
                        var sortParams = param["sort"] as Hashtable;
                        DataRow[] foundRows = netTable.Select("", $"{sortParams["columnName"]} {sortParams["direction"]}");
                        netTable = foundRows.CopyToDataTable();

                        var sortLabels = new ArrayList();
                        var sortLabel = new Hashtable();
                        sortLabel["columnIndex"] = netTable.Columns.IndexOf(sortParams["columnName"] as string);
                        sortLabel["direction"] = sortParams["direction"] as string == "ASC" ? "Ascending" : "Descending";
                        sortLabels.Add(sortLabel);
                        callbackResult["sortLabels"] = sortLabels;
                    }

                    ArrayList captions = new ArrayList();
                    for (int k = 0; k < netTable.Columns.Count; k++)
                    {
                        captions.Add(netTable.Columns[k].Caption);
                    }
                    resultData.Add(captions);

                    for (int i = 0; i < netTable.Rows.Count; i++)
                    {
                        ArrayList rowArray = new ArrayList();
                        resultData.Add(rowArray);
                        for (int k = 0; k < netTable.Columns.Count; k++)
                        {
                            rowArray.Add(GetDataValue(netTable.Rows[i][k], netTable.Columns[k].DataType));
                        }
                    }
                }
                #endregion
            }

            callbackResult["data"] = resultData;
        }

        private static ArrayList GetDataTableContent(DataTable dataTable, StiDataTransformation dataTransformation)
        {
            ArrayList resultData = new ArrayList();
            List<StiDataColumn> dataTransformationColumns = new List<StiDataColumn>();

            if (dataTable != null)
            {
                ArrayList captions = new ArrayList();
                for (int k = 0; k < dataTable.Columns.Count; k++)
                {
                    captions.Add(dataTable.Columns[k].Caption);
                    dataTransformationColumns.Add(dataTransformation.Columns[k]);
                }
                resultData.Add(captions);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    ArrayList rowArray = new ArrayList();
                    resultData.Add(rowArray);
                    for (int k = 0; k < dataTable.Columns.Count; k++)
                    {
                        rowArray.Add(GetDataValue(dataTable.Rows[i][k], dataTransformationColumns[k]?.Type));
                    }
                }
            }

            return resultData;
        }

        private static Hashtable GetDataValue(object value, Type dataColumnType)
        {
            Hashtable resultItem = new Hashtable();
            var type = dataColumnType != null ? dataColumnType : value.GetType();

            if (type == typeof(Byte[]) || type == typeof(Image))
            {
                resultItem["type"] = "Image";
                resultItem["displayString"] = (type == typeof(Image)) 
                    ? StiReportEdit.ImageToBase64(value as Image) 
                    : string.Format("data:image;base64,{0}", value as byte[] != null ? Convert.ToBase64String(value as byte[]) : string.Empty);
            }
            else
            {
                resultItem["type"] = type.Name;
                resultItem["displayString"] = StiDataFiltersHelper.ToDisplayString(value, type);
            }            

            return resultItem;
        }

        private static ArrayList GetSortLabels(StiDataTransformation dataTransformation)
        {
            var sortLabels = new ArrayList();

            foreach (var sortRule in dataTransformation.Sorts)
            {
                for (var columnIndex = 0; columnIndex < dataTransformation.Columns.Count; columnIndex++)
                {
                    var column = dataTransformation.Columns[columnIndex];

                    if (sortRule.Key == column.Key)
                    {
                        var sortLabel = new Hashtable();
                        sortLabel["columnIndex"] = columnIndex;
                        sortLabel["direction"] = sortRule.Direction;
                        sortLabels.Add(sortLabel);
                    }
                }
            }

            return sortLabels;
        }

        private static ArrayList GetFilterLabels(StiDataTransformation dataTransformation)
        {
            var filterLabels = new ArrayList();

            for (var columnIndex = 0; columnIndex < dataTransformation.Columns.Count; columnIndex++)
            {
                var column = dataTransformation.Columns[columnIndex];

                var rule = dataTransformation.Filters.FirstOrDefault(r => string.Equals(r.Key, column.Key, StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(r.Path, column.GetColumnPath(), StringComparison.InvariantCultureIgnoreCase));

                var action = dataTransformation.Actions.FirstOrDefault(r => string.Equals(r.Key, column.Key, StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(r.Path, column.GetColumnPath(), StringComparison.InvariantCultureIgnoreCase));

                if (rule != null || action != null)
                {
                    var filterLabel = new Hashtable();
                    filterLabel["columnIndex"] = columnIndex;
                    filterLabels.Add(filterLabel);
                }
            }

            return filterLabels;
        }

        private static IEnumerable<object> Distinct(IEnumerable<object> items)
        {
            if (!(items.FirstOrDefault() is DateTime))
                return items.Distinct();

            return items
                .Select(StiValueHelper.TryToNullableDateTime)
                .Where(d => d != null)
                .Cast<DateTime>()
                .GroupBy(x => x.Date)
                .Select(x => x.Key)
                .Cast<object>()
                .Distinct();
        }

        private static bool IsValueCanBeFiltered(object value)
        {
            if (value == null || value == DBNull.Value)
                return true;

            var type = value.GetType();
            return type.IsNumericType() || type.IsDateType() || type == typeof(string) || type == typeof(bool);
        }
        #endregion

        #region Methods
        public static ArrayList GetColumns(StiDataTransformation dataTransformation)
        {
            ArrayList columns = new ArrayList();
            if (dataTransformation.Columns != null)
            {
                foreach (StiDataTransformationColumn column in dataTransformation.Columns)
                    columns.Add(ColumnItem(column, dataTransformation.Dictionary));
            }

            return columns;
        }

        public static ArrayList GetSortRules(StiDataTransformation dataTransformation)
        {
            ArrayList sortRules = new ArrayList();
            foreach (StiDataSortRule sortRule in dataTransformation.Sorts)
                sortRules.Add(StiDataSortsHelper.SortRuleItem(sortRule));

            return sortRules;
        }

        public static ArrayList GetFilterRules(StiDataTransformation dataTransformation)
        {
            ArrayList filterRules = new ArrayList();
            foreach (StiDataFilterRule filterRule in dataTransformation.Filters)
                filterRules.Add(StiDataFiltersHelper.FilterRuleItem(filterRule));

            return filterRules;
        }

        public static ArrayList GetActionRules(StiDataTransformation dataTransformation)
        {
            ArrayList actionRules = new ArrayList();
            foreach (StiDataActionRule actionRule in dataTransformation.Actions)
                actionRules.Add(ActionRuleItem(actionRule));

            return actionRules;
        }

        public static void ApplyProperties(StiDataTransformation dataTransformation, Hashtable dataSourceProps, StiReport report)
        {
            if (dataTransformation == null) return;

            if (dataSourceProps["name"] != null)
                dataTransformation.Name = dataSourceProps["name"] as string;

            if (dataSourceProps["nameInSource"] != null)
                dataTransformation.NameInSource = dataSourceProps["nameInSource"] as string;

            if (dataSourceProps["alias"] != null)
                dataTransformation.Alias = dataSourceProps["alias"] as string;

            if (report != null)
                dataTransformation.Dictionary = report.Dictionary;

            #region Apply Columns
            if (dataSourceProps["columns"] != null)
            {
                var columns = dataSourceProps["columns"] as ArrayList;
                dataTransformation.Columns.Clear();
                foreach (Hashtable columnObject in columns)
                {
                    dataTransformation.Columns.Add(GetColumnFromJSColumnObject(columnObject));
                }
            }
            #endregion

            #region Apply SortRules
            if (dataSourceProps["sortRules"] != null)
            {
                var sortRules = dataSourceProps["sortRules"] as ArrayList;
                dataTransformation.Sorts.Clear();
                foreach (Hashtable sortRuleObject in sortRules)
                {
                    dataTransformation.Sorts.Add(GetSortRuleFromJSSortRuleObject(sortRuleObject));
                }
            }
            #endregion

            #region Apply FilterRules
            if (dataSourceProps["filterRules"] != null)
            {
                var filterRules = dataSourceProps["filterRules"] as ArrayList;
                dataTransformation.Filters.Clear();
                foreach (Hashtable filterRuleObject in filterRules)
                {
                    dataTransformation.Filters.Add(GetFilterRuleFromJSFilterRuleObject(filterRuleObject));
                }
            }
            #endregion

            #region Apply ActionRules
            if (dataSourceProps["actionRules"] != null)
            {
                var actionRules = dataSourceProps["actionRules"] as ArrayList;
                dataTransformation.Actions.Clear();
                foreach (Hashtable actionRuleObject in actionRules)
                {
                    dataTransformation.Actions.Add(GetActionRuleFromJSActionRuleObject(actionRuleObject));
                }
            }
            #endregion
        }

        public static Hashtable GetDataGridContent(StiDataTransformation dataTransformation = null)
        {
            var content = new Hashtable();

            try
            {
                if (dataTransformation == null)
                    dataTransformation = new StiDataTransformation();

                var dataTable = dataTransformation.RetrieveDataTable(StiDataRequestOption.All);

                content["data"] = GetDataTableContent(dataTable, dataTransformation);
                content["sortLabels"] = GetSortLabels(dataTransformation);
                content["filterLabels"] = GetFilterLabels(dataTransformation);
            }
            catch (StiFunctionNotFoundException ex)
            {
                content["errorMessage"] = ex.Message;
            }
            catch (StiColumnNotFoundException ex)
            {
                content["errorMessage"] = ex.Message;
            }
            catch (StiArgumentNotFoundException ex)
            {
                content["errorMessage"] = ex.Message;
            }
            catch (EvaluationException ex)
            {
                content["errorMessage"] = ex.Message;
            }
            catch (FormatException ex)
            {
                content["errorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                content["errorMessage"] = ex.Message;
            }           
            
            return content;
        }

        public static Hashtable GetFilterItemsHelper(StiDataTransformation dataTransformation, Hashtable parameters)
        {
            var columnIndex = Convert.ToInt32(parameters["columnIndex"]);
            var meters = dataTransformation?.GetMeters();

            return StiDataFiltersHelper.GetFilterItemsHelper(dataTransformation as IStiQueryObject, meters, columnIndex, dataTransformation.Sorts, dataTransformation.Filters);
        }

        public static Hashtable GetFilterItemsHelper(StiReport report, Hashtable parameters)
        {
            var dataTransformation = new StiDataTransformation();
            ApplyProperties(dataTransformation, parameters, report);

            return GetFilterItemsHelper(dataTransformation, parameters);
        }

        public static void ExecuteJSCommand(StiReport report, Hashtable param, Hashtable callbackResult, StiRequestParams requestParams)
        {
            var parameters = param["parameters"] as Hashtable;

            switch ((string)parameters["command"])
            {
                #region GetDataTransformationColumnFromColumns
                case "GetDataTransformationColumnsFromColumns":
                    {
                        var draggedItem = parameters["draggedItem"] as Hashtable;
                        var draggedItemObject = draggedItem["itemObject"] as Hashtable;
                        var draggedColumns = new StiDataColumnsCollection();
                        var resultColumns = new ArrayList();

                        var allColumns = StiDictionaryHelper.GetColumnsByTypeAndNameOfObject(report, draggedItem);
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
                            var newColumn = CreateTransformationColumnFromDataColumn(dataColumn);
                            resultColumns.Add(ColumnItem(newColumn, report.Dictionary));
                            callbackResult["newColumns"] = resultColumns;
                        }
                        break;
                    }
                #endregion

                #region CreateNewDataTransformationColumn
                case "CreateNewDataTransformationColumn":
                    {
                        callbackResult["newColumn"] = ColumnItem(new StiDataTransformationColumn
                        {
                            Name = parameters["columnName"] as string,
                            Mode = (StiDataTransformationMode)Enum.Parse(typeof(StiDataTransformationMode), parameters["columnMode"] as string),
                            Type = typeof(object)
                        }, report.Dictionary);
                        break;
                    }
                #endregion

                #region ChangeDataColumnMode
                case "ChangeDataColumnMode":
                    {
                        var columnObject = parameters["columnObject"] as Hashtable;
                        var column = GetColumnFromJSColumnObject(columnObject);
                        column.Mode = (StiDataTransformationMode)Enum.Parse(typeof(StiDataTransformationMode), parameters["newMode"] as string);
                        if (column.Mode == StiDataTransformationMode.Dimension)
                        {
                            column.Expression = StiExpressionHelper.RemoveFunction(column.Expression);
                            if (columnObject["originalType"] != null)
                            {
                                column.Type = StiDictionaryHelper.GetTypeFromString(columnObject["originalType"] as string);
                            }
                            else {
                                var dictColumn = column.GetDictionaryColumn();
                                column.Type = dictColumn != null ? dictColumn.Type : typeof(object);
                            }
                        }
                        else if (column.Mode == StiDataTransformationMode.Measure)
                        {
                            var isNumeric = StiDataExpressionHelper.IsNumericDataColumnInExpression(new StiDataTransformation() { Dictionary = report.Dictionary }, column.Expression);
                            column.Expression = StiExpressionHelper.ReplaceFunction(column.Expression, isNumeric ? "Sum" : "Count");
                            column.Type = isNumeric ? typeof(int) : typeof(decimal);
                        }
                        callbackResult["newColumn"] = ColumnItem(column, report.Dictionary, columnObject["originalType"] as string);
                        break;
                    }
                #endregion

                #region SetFunctionToColumn
                case "SetFunctionToColumn":
                    {
                        var columnObject = parameters["columnObject"] as Hashtable;
                        var column = GetColumnFromJSColumnObject(columnObject);
                        column.Expression = StiExpressionHelper.ReplaceFunction(column.Expression, parameters["function"] as string);
                        callbackResult["column"] = ColumnItem(column, report.Dictionary, columnObject["originalType"] as string);
                        break;
                    }
                    #endregion

                #region GetDataGridContent
                case "GetDataGridContent":
                    {
                        var dataTransformation = new StiDataTransformation();
                        ApplyProperties(dataTransformation, parameters, report);
                        callbackResult["dataGridContent"] = GetDataGridContent(dataTransformation);
                        break;
                    }
                #endregion

                #region GetDataGridContentForElementDataTransform
                case "GetDataGridContentForElementDataTransform":
                    {
                        StiDataTransformation dataTransformation = null;

                        if (parameters["dataTransformationCacheGuid"] == null)
                        {
                            dataTransformation = GetDataTransformationFromElement(report, parameters);
                            var dataTransformationCacheGuid = $"{StiGuidUtils.NewGuid()}_{StiCacheHelper.GUID_DataTransformation}";
                            requestParams.Cache.Helper.SaveObjectInternal(dataTransformation, requestParams, dataTransformationCacheGuid);

                            if (requestParams.Cache.Mode == StiServerCacheMode.StringCache || requestParams.Cache.Mode == StiServerCacheMode.StringSession)
                                requestParams.Cache.Helper.SaveObjectInternal(dataTransformation.Dictionary.Report, requestParams, $"{dataTransformationCacheGuid}_{StiCacheHelper.GUID_ReportTemplate}");

                            callbackResult["dataTransformation"] = StiDictionaryHelper.DataTransformationItem(dataTransformation);
                            callbackResult["dataTransformationCacheGuid"] = dataTransformationCacheGuid;
                        }
                        else
                        {
                            dataTransformation = requestParams.Cache.Helper.GetObjectInternal(requestParams, parameters["dataTransformationCacheGuid"] as string) as StiDataTransformation;

                            if (requestParams.Cache.Mode == StiServerCacheMode.StringCache || requestParams.Cache.Mode == StiServerCacheMode.StringSession)
                            {
                                var tempReport = requestParams.Cache.Helper.GetObjectInternal(requestParams, $"{parameters["dataTransformationCacheGuid"]}_{StiCacheHelper.GUID_ReportTemplate}") as StiReport;
                                if (tempReport != null) dataTransformation.Dictionary = tempReport.Dictionary;
                            }

                            parameters.Remove("columns");
                            ApplyProperties(dataTransformation, parameters, null);
                        }

                        callbackResult["dataGridContent"] = GetDataGridContent(dataTransformation);
                        break;
                    }
                #endregion

                #region GetFilteredItems
                case "GetFilteredItems":
                    {
                        if (parameters["elementName"] != null && parameters["dataTransformationCacheGuid"] != null)
                        {
                            var dataTransformation = requestParams.Cache.Helper.GetObjectInternal(requestParams, parameters["dataTransformationCacheGuid"] as string) as StiDataTransformation;

                            if (requestParams.Cache.Mode == StiServerCacheMode.StringCache || requestParams.Cache.Mode == StiServerCacheMode.StringSession)
                            {
                                var tempReport = requestParams.Cache.Helper.GetObjectInternal(requestParams, $"{parameters["dataTransformationCacheGuid"]}_{StiCacheHelper.GUID_ReportTemplate}") as StiReport;
                                if (tempReport!= null) dataTransformation.Dictionary = tempReport.Dictionary;
                            }

                            parameters.Remove("columns");
                            ApplyProperties(dataTransformation, parameters, null);
                            callbackResult["filterItemsHelper"] = GetFilterItemsHelper(dataTransformation, parameters);
                        }
                        else
                        {
                            callbackResult["filterItemsHelper"] = GetFilterItemsHelper(report, parameters);
                        }
                        break;
                    }
                #endregion

                #region SetElementDataTransformRules
                case "SetElementDataTransformRules":
                    {
                        var element = report.Pages.GetComponentByName(parameters["elementName"] as string) as IStiElement;
                        var dataTransformation = parameters["dataTransformation"] as Hashtable;
                        if (element != null && dataTransformation != null)
                        {
                            StiCacheCleaner.Clean(element.Report);

                            if (element is IStiTransformFilters)
                            {
                                (element as IStiTransformFilters).TransformFilters = new List<StiDataFilterRule>();
                                var filterRules = dataTransformation["filterRules"] as ArrayList;
                                if (filterRules != null)
                                {
                                    filterRules.Cast<Hashtable>().ToList().ForEach(filterRuleObject =>
                                    {
                                        var filterRule = GetFilterRuleFromJSFilterRuleObject(filterRuleObject);
                                        filterRule.Key = null;

                                        if (filterRule.Path.StartsWith("Source."))
                                            filterRule.Path = filterRule.Path.Replace("Source.", "");

                                        (element as IStiTransformFilters).TransformFilters.Add(filterRule);
                                    });
                                }
                            }

                            if (element is IStiTransformSorts)
                            {
                                (element as IStiTransformSorts).TransformSorts = new List<StiDataSortRule>();
                                var sortRules = dataTransformation["sortRules"] as ArrayList;
                                var columns = dataTransformation["columns"] as ArrayList;
                                if (sortRules != null && columns != null)
                                {
                                    sortRules.Cast<Hashtable>().ToList().ForEach(sortRuleObject =>
                                    {
                                        var sortRule = GetSortRuleFromJSSortRuleObject(sortRuleObject);
                                        sortRule.Key = columns.Cast<Hashtable>().ToList().FirstOrDefault(column => (string)column["key"] == sortRule.Key)?["name"] as string;
                                        (element as IStiTransformSorts).TransformSorts.Add(sortRule);
                                    });
                                }
                            }

                            if (element is IStiTransformActions)
                            {
                                (element as IStiTransformActions).TransformActions = new List<StiDataActionRule>();
                                var actionRules = dataTransformation["actionRules"] as ArrayList;
                                if (actionRules != null)
                                {
                                    actionRules.Cast<Hashtable>().ToList().ForEach(actionRuleObject =>
                                    {
                                        var actionRule = GetActionRuleFromJSActionRuleObject(actionRuleObject);
                                        actionRule.Key = null;

                                        if (actionRule.Path.StartsWith("Source."))
                                            actionRule.Path = actionRule.Path.Replace("Source.", "");

                                        (element as IStiTransformActions).TransformActions.Add(actionRule);
                                    });
                                }
                            }

                            callbackResult["svgContent"] = StiEncodingHelper.Encode(StiDashboardsSvgHelper.SaveElementToString(element, 1, 1, true));
                        }
                        break;
                    }
                #endregion

                #region ClearDataTransformCache
                case "ClearDataTransformCache":
                    {
                        if (parameters["dataTransformationCacheGuid"] != null)
                        {
                            var dataTransformation = requestParams.Cache.Helper.GetObjectInternal(requestParams, parameters["dataTransformationCacheGuid"] as string) as StiDataTransformation;
                            if (dataTransformation != null)
                            {
                                if (dataTransformation.Dictionary != null && dataTransformation.Dictionary.Report != null)
                                {
                                    dataTransformation.Dictionary.Report.Dispose();
                                    dataTransformation.Dictionary.Report = null;
                                }
                                requestParams.Cache.Helper.RemoveObject(parameters["dataTransformationCacheGuid"] as string);
                            }
                            if (requestParams.Cache.Mode == StiServerCacheMode.StringCache || requestParams.Cache.Mode == StiServerCacheMode.StringSession)
                            {
                                requestParams.Cache.Helper.RemoveObject($"{parameters["dataTransformationCacheGuid"]}_{StiCacheHelper.GUID_ReportTemplate}");
                            }
                        }
                        break;
                    }
                #endregion
            }
        }

        private static StiDataTransformation GetDataTransformationFromElement(StiReport report, Hashtable param)
        {
            var element = report.Pages.GetComponentByName(param["elementName"] as string) as IStiElement;
            if (element != null)
            {
                var temReport = new StiReport();
                var dataTable = StiElementDataCache.Create(element, StiDataRequestOption.DisallowTransform);
                var netTable = dataTable.ToNetTable();
                netTable.TableName = "Source";

                temReport.RegData("Source", netTable);
                temReport.Dictionary.Synchronize();
                report.Dictionary.Variables.ToList().ForEach(v => temReport.Dictionary.Variables.Add(v.Clone() as StiVariable));

                var dataSource = temReport.Dictionary.DataSources[0];
                var dataColumns = dataSource.Columns.ToList();
                dataColumns.ForEach(c => c.Key = StiKeyHelper.GetOrGeneratedKey(c.Key));

                var dataTransformation = new StiDataTransformation
                {
                    NameInSource = "Source",
                    Name = "Source"
                };

                var transformFilters = (element as IStiTransformFilters)?.TransformFilters;
                if (transformFilters != null)
                {
                    dataTransformation.Filters = new List<StiDataFilterRule>();

                    transformFilters.ForEach(f =>
                    {
                        var fClone = f.Clone() as StiDataFilterRule;
                        fClone.Key = dataColumns.FirstOrDefault(c => c.Name == fClone.Path)?.Key;
                        if (fClone.Path != null && !fClone.Path.StartsWith("Source."))
                            fClone.Path = $"Source.{fClone.Path}";

                        dataTransformation.Filters.Add(fClone);
                    });
                }

                var transformSorts = (element as IStiTransformSorts)?.TransformSorts;
                if (transformSorts != null)
                {
                    dataTransformation.Sorts = new List<StiDataSortRule>();

                    transformSorts.ForEach(s =>
                    {
                        var sClone = s.Clone() as StiDataSortRule;
                        sClone.Key = dataColumns.FirstOrDefault(c => c.Name == sClone.Key)?.Key;

                        dataTransformation.Sorts.Add(sClone);
                    });
                }

                var transformActions = (element as IStiTransformActions)?.TransformActions;
                if (transformActions != null)
                {
                    dataTransformation.Actions = new List<StiDataActionRule>();

                    transformActions.ForEach(a =>
                    {
                        var aClone = a.Clone() as StiDataActionRule;
                        aClone.Key = dataColumns.FirstOrDefault(c => c.Name == aClone.Path)?.Key;
                        if (aClone.Path != null && !aClone.Path.StartsWith("Source."))
                            aClone.Path = $"Source.{aClone.Path}";

                        dataTransformation.Actions.Add(aClone);
                    });
                }

                temReport.Dictionary.DataSources.Add(dataTransformation);

                var columnIndex = 0;
                var columns = dataColumns.Select(CreateTransformationColumnFromDataColumn).ToArray();
                columns.ToList().ForEach(column =>
                {
                    column.Name = StiNameCreation.CreateColumnName(dataTransformation, column.Name);
                    column.Expression = StiExpressionHelper.ReplaceFunction(column.Expression, "All");
                    column.Mode = StiDataTransformationMode.Dimension;
                    column.Key = dataSource.Columns[columnIndex].Key;

                    dataTransformation.Columns.Add(column);

                    columnIndex++;
                });

                return dataTransformation;
            }

            return null;
        }
        #endregion
    }
}