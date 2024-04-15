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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Engine
{
    public class StiVariableHelper
    {
        #region Fields
        private static object lockObject = new object();
        private static Hashtable ReportToLabels = new Hashtable();
        #endregion

        #region Methods
        public static bool FillItemsOfVariables(StiReport compiledReport, bool? modeItems = null)
        {
            var modified = false;
            foreach (StiVariable variable in compiledReport.Dictionary.Variables)
            {
                if (FillItemsOfVariable(variable, compiledReport, ref modified, modeItems)) break;
            }
            return modified;
        }

        public static bool FillItemsOfVariable(StiVariable variable, StiReport compiledReport, ref bool modified, bool? modeItems = null)
        {

#if SERVER
            //If this variable initialized from the server side then we need skip variable initialization
            if (compiledReport.ServerParameters != null && compiledReport.ServerParameters[variable.Name] != null) return false;
#endif

            if (variable.DialogInfo == null)
                return false;

            bool processItems = modeItems == null || modeItems == true;
            bool processColumns = modeItems == null || modeItems == false;

            #region StiItemsInitializationType.Items
            if (variable.DialogInfo.ItemsInitializationType == StiItemsInitializationType.Items && processItems)
            {
                if (compiledReport.IsReportRenderingAfterSubmit)
                    return false;

                #region Fill List
                var items = variable.DialogInfo.GetDialogInfoItems(variable.Type);
                items = variable.DialogInfo.OrderBy(items);
                if (items != null && items.Count > 0)
                {
                    var list = compiledReport[variable.Name] as IStiList;
                    if (list == null)
                        return false;

                    if (list.Count > 0)
                        return true;

                    list.Clear();

                    foreach (var item in items)
                    {
                        try
                        {
                            if (item.Checked)
                            {
                                list.AddElement(item.KeyObject);
                                modified = true;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                #endregion
            }
            #endregion

            #region StiItemsInitializationType.Columns
            if (variable.DialogInfo.ItemsInitializationType == StiItemsInitializationType.Columns && processColumns)
            {
                if (variable.DialogInfo.Keys != null && variable.DialogInfo.Keys.Length > 0 &&
                    variable.DialogInfo.Values != null && variable.DialogInfo.Values.Length > 0 &&
                    variable.DialogInfo.CheckedStates != null && variable.DialogInfo.CheckedStates.Length > 0)
                    return false;

                var keys = !string.IsNullOrEmpty(variable.DialogInfo.KeysColumn) 
                    ? StiDataColumn.GetDatasFromDataColumn(compiledReport.Dictionary, variable.DialogInfo.KeysColumn, null, false, false) 
                    : null;

                var values = !string.IsNullOrEmpty(variable.DialogInfo.ValuesColumn)
                    ? StiDataColumn.GetDatasFromDataColumn(compiledReport.Dictionary, variable.DialogInfo.ValuesColumn, null, false, false)
                    : null;

                var checkedStates = !string.IsNullOrEmpty(variable.DialogInfo.CheckedColumn)
                    ? GetChecked(compiledReport.Dictionary, variable.DialogInfo.CheckedColumn)
                    : null;

                var valuesBinding = !string.IsNullOrEmpty(variable.DialogInfo.BindingValuesColumn) ?
                    StiDataColumn.GetDatasFromDataColumn(compiledReport.Dictionary, variable.DialogInfo.BindingValuesColumn, null, false, false) : null;

                if (keys == null)
                    keys = new object[0];

                if (values == null)
                    values = new object[0];

                if (checkedStates == null)
                    checkedStates = new bool[0];

                if (valuesBinding == null)
                    valuesBinding = new object[0];

                var length = Math.Max(keys.Length, values.Length);
                length = Math.Max(length, checkedStates.Length);

                var hashKeysValuesBinding = new Hashtable();

                //remove keys duplicates
                if (keys.Length > 0)
                {
                    var keys1 = new List<object>();
                    var values1 = new List<object>();                    
                    var valuesBinding1 = new List<object>();
                    var states1 = new List<bool>();

                    for (int index = 0; index < length; index++)
                    {
                        var key = index < keys.Length ? keys[index] : null;
                        var value = index < values.Length ? values[index].ToString() : string.Empty;
                        
                        var valueObject = index < values.Length ? values[index] : null;
                        var valueBinding = index < valuesBinding.Length ? valuesBinding[index] : null;

                        var state = checkedStates == null || index >= checkedStates.Length || StiValueHelper.TryToBool(checkedStates[index]);
						if (key is DateTime)
						{
                            var dateTimeValue = (DateTime)key;
                            switch (variable.DialogInfo.DateTimeType)
                            {
                                case StiDateTimeType.DateAndTime:
                                    break;

                                case StiDateTimeType.Date:
                                    key = dateTimeValue.Date;
                                    break;

                                case StiDateTimeType.Time:
                                    key = dateTimeValue.ToShortTimeString();
                                    break;
                            }
                        }

                        if (valueObject is DateTime)
                        {
                            var dateTimeValue = (DateTime)values[index];
                            switch (variable.DialogInfo.DateTimeType)
                            {
                                case StiDateTimeType.DateAndTime:
                                    value = dateTimeValue.ToString();
                                    break;

                                case StiDateTimeType.Date:
                                    value = dateTimeValue.ToShortDateString();
                                    break;

                                case StiDateTimeType.Time:
                                    value = dateTimeValue.ToShortTimeString();
                                    break;
                            }
                        }

                        if (!hashKeysValuesBinding.Contains(key))
                        {
                            keys1.Add(key);
                            values1.Add(value);
                            valuesBinding1.Add(valueBinding);
                            states1.Add(state);
                            hashKeysValuesBinding[key] = new List<object>() { valueBinding };
                        }
                        else
                        {
                            var listValueBinding = hashKeysValuesBinding[key] as List<object>;
                            listValueBinding.Add(valueBinding);
                        }
                    }

                    keys = keys1.ToArray();
                    values = values1.ToArray();
                    valuesBinding = valuesBinding1.ToArray();
                    checkedStates = states1.ToArray();

                    length = Math.Max(keys.Length, values.Length);
                    length = Math.Max(length, checkedStates.Length);
                }

                var items = new List<StiDialogInfoItem>();

                #region Create List<StiDialogInfoItem>
                var type = variable.Type;

                for (var index = 0; index < length; index++)
                {
                    var key = keys.Length <= index ? null : keys[index];
                    var value = values.Length <= index ? string.Empty : values[index].ToString();
                    var state = checkedStates == null || index >= checkedStates.Length || checkedStates[index];

                    StiDialogInfoItem item = null;

                    #region StiLongDialogInfoItem
                    if (type == typeof(ByteList) ||
                        type == typeof(ShortList) ||
                        type == typeof(IntList) ||
                        type == typeof(LongList) ||
                        type == typeof(byte) ||
                        type == typeof(short) ||
                        type == typeof(int) ||
                        type == typeof(long) ||
                        type == typeof(byte?) ||
                        type == typeof(short?) ||
                        type == typeof(int?) ||
                        type == typeof(long?))
                    {
                        item = new StiLongDialogInfoItem();
                    }
                    #endregion

                    #region StiStringDialogInfoItem
                    else if (type == typeof(StringList) || type == typeof(string))
                    {
                        item = new StiStringDialogInfoItem();
                    }
                    #endregion

                    #region StiDoubleDialogInfoItem
                    else if (
                        type == typeof(DoubleList) ||
                        type == typeof(FloatList) ||
                        type == typeof(double) ||
                        type == typeof(float) ||
                        type == typeof(double?) ||
                        type == typeof(float?))
                    {
                        item = new StiDoubleDialogInfoItem();
                    }
                    #endregion

                    #region StiDecimalDialogInfoItem
                    else if (type == typeof(DecimalList) || type == typeof(decimal) || type == typeof(decimal?))
                    {
                        item = new StiDecimalDialogInfoItem();
                    }
                    #endregion

                    #region StiDateTimeDialogInfoItem
                    else if (type == typeof(DateTimeList) || type == typeof(DateTime) || type == typeof(DateTime?))
                    {
                        item = new StiDateTimeDialogInfoItem();                        
                    }
                    #endregion

                    #region StiTimeSpanDialogInfoItem
                    else if (type == typeof(TimeSpanList) || type == typeof(TimeSpan) || type == typeof(TimeSpan?))
                    {
                        item = new StiTimeSpanDialogInfoItem();
                    }
                    #endregion

                    #region StiBoolDialogInfoItem
                    else if (type == typeof(BoolList) || type == typeof(bool) || type == typeof(bool?))
                    {
                        item = new StiBoolDialogInfoItem();
                    }
                    #endregion

                    #region StiCharDialogInfoItem
                    else if (type == typeof(CharList) || type == typeof(char) || type == typeof(char?))
                    {
                        item = new StiCharDialogInfoItem();
                    }
                    #endregion

                    #region StiGuidDialogInfoItem
                    else if (type == typeof(GuidList) || type == typeof(Guid))
                    {
                        item = new StiGuidDialogInfoItem();
                    }
                    #endregion

                    #region StiImageDialogInfoItem
                    else if (type == typeof(Image))
                    {
                        item = new StiImageDialogInfoItem();
                    }
                    #endregion

                    if (item != null)
                    {
                        if (key != null)
                            item.KeyObject = key;

                        item.Value = value;
                        item.ValueBinding = hashKeysValuesBinding[key] as List<object>;
                        item.Checked = state;

                        items.Add(item);
                    }
                }
                #endregion

                var itemsFiltered = new List<StiDialogInfoItem>();
                var hash = new Hashtable();
                foreach (var item in items)
                {
                    if (hash[item.KeyObject] == null || item is StiRangeDialogInfoItem)
                    {
                        hash[item.KeyObject] = item.KeyObject;
                        itemsFiltered.Add(item);
                    }
                }
                
                //fix Artem (17.09.2013) (RequestFromUser Do not use filtering for dependent variables because
                //this may cause lost of data for further calculations.)
                var resultItem = variable.DialogInfo.BindingValue ? items : itemsFiltered;
                resultItem = variable.DialogInfo.OrderBy(resultItem);
                variable.DialogInfo.SetDialogInfoItems(resultItem, variable.Type);

                #region Fill List
                keys = resultItem.Select(r => r.KeyObject).ToArray();
                var checks = resultItem.Select(r => r.Checked).Cast<bool>().ToArray();
                if (StiTypeFinder.FindInterface(variable.Type, typeof(IStiList)) && keys != null && keys.Length > 0)
                {
                    FillVariableList(variable, compiledReport, keys, checks);
                    modified = true;
                }
                #endregion
            }
            #endregion

            return false;
        }

        private static bool[] GetChecked(StiDictionary dictionary, string dataColumn)
        {
            if (string.IsNullOrEmpty(dataColumn))
                return new bool[0];

            var index = dataColumn.IndexOf(";");
            if (index == -1)
                return new bool[0];

            var dataSourceName = dataColumn.Substring(0, index);
            var expression = "{" + dataColumn.Substring(index + 1) + "}";

            var dataSource = dictionary.DataSources[dataSourceName];
            if (dataSource == null)
                return new bool[0];

            var array = StiDataColumn.GetDatasFromDataSourceWithExpression(dataSource, expression, null, true);
            if (array == null)
                return null;

            return array.Select(c => StiValueHelper.TryToBool(c)).ToArray();
            
        }

        private static void FillVariableList(StiVariable variable, StiReport compiledReport, object[] keys, bool[] checks)
        {
            var list = compiledReport[variable.Name] as IStiList;
            if (list == null)
            {
                var typeList = StiType.GetTypeFromTypeMode(variable.Type, StiTypeMode.List);
                list = StiActivator.CreateObject(typeList) as IStiList;
            }

            list.Clear();

            var index = 0;
            foreach (var key in keys)
            {
                var check = index >= checks.Length || checks[index];
                try
                {
                    if (check)
                        list.AddElement(key);
                }
                catch
                {
                }
                index++;
            }
        }

        public static void SetDefaultValueForRequestFromUserVariables(StiReport report, bool haveVars, bool allowParseQuery = false, bool isConnectToDataV2 = false)
        {
            if (report == null) return;

            var tempText = new StiText { Name = "**VariableRequestFromUser**" };
            if (report.Pages.Count > 0)
                tempText.Page = report.Pages[0];

            #region Calculate variables
            InitDateTimeVariables(report);

            if (!report.IsReportRenderingAfterSubmit)
            {
                foreach (StiVariable variable in report.Dictionary.Variables)
                {
                    if (!variable.RequestFromUser || !typeof(IStiList).IsAssignableFrom(variable.Type) 
                        || report.ModifiedVariables.ContainsKey(variable.Name) 
                        || variable.DialogInfo.Values == null) continue;

                    var separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
                    var str = string.Join(separator, variable.DialogInfo.Values.ToArray());
                    SetVariableLabel(report, variable, str);
                }
            }

            foreach (StiVariable variable in report.Dictionary.Variables)
            {
                if (!variable.RequestFromUser || typeof(IStiList).IsAssignableFrom(variable.Type) ||
                    variable.Type.IsSubclassOf(typeof(Range))) continue;

                //haveVars = true;

                if (report.ModifiedVariables.ContainsKey(variable.Name)) continue;

                try
                {                    
                    if (variable.Selection == StiSelectionMode.FromVariable && variable.InitBy == StiVariableInitBy.Expression)
                    {
                        var newValue = StiParser.ParseTextValue("{" + variable.Value + "}", tempText);
                        if (variable.DialogInfo.DateTimeType == StiDateTimeType.Date && newValue is DateTime)
                            newValue = ((DateTime)newValue).Date;

                        if (!report.IsReportRenderingAfterSubmit)
                        {
                            var index = (newValue != null && variable.DialogInfo.Keys != null) 
                                ? variable.DialogInfo.Keys.ToList().IndexOf(newValue?.ToString()) 
                                : -1;

                            if (index != -1)
                                SetVariableLabel(report, variable, variable.DialogInfo.Values.ElementAtOrDefault(index));
                        }

                        report[variable.Name] = newValue;
                        haveVars = true;
                    }

                    if (variable.Selection == StiSelectionMode.FromVariable && variable.InitBy == StiVariableInitBy.Value)
                    {
                        if (!report.IsReportRenderingAfterSubmit)
                        {
                            var index = (variable.ValueObject != null && variable.DialogInfo.Keys != null) 
                                ? variable.DialogInfo.Keys.ToList().IndexOf(variable.ValueObject?.ToString()) 
                                : -1;

                            if (index != -1)
                                SetVariableLabel(report, variable, variable.DialogInfo.Values.ElementAtOrDefault(index));
                        }
                    }

                    if (variable.Selection == StiSelectionMode.First && variable.DialogInfo.Keys != null && variable.DialogInfo.Keys.Length > 0)
                    {
                        var newValue = StiReport.ChangeType(variable.DialogInfo.Keys.FirstOrDefault(), variable.Type);
                        report[variable.Name] = newValue;
                        haveVars = true;

                        if (!report.IsReportRenderingAfterSubmit)
                            SetVariableLabel(report, variable, variable.DialogInfo.Values.FirstOrDefault());
                    }
                }
                catch
                {
                }
            }
            #endregion

            if (!haveVars || isConnectToDataV2) return;

            var dictionary = report.Dictionary;

            #region Reconnect datasources with RequestFromUser variables in the SqlCommand
            var dataSourcesToReconnect = dictionary.ReconnectListForRequestFromUserVariables;
            if (dataSourcesToReconnect == null)
                dataSourcesToReconnect = GetDataSourcesWithRequestFromUserVariablesInCommand(report).ToArray();

            if (dataSourcesToReconnect.Length <= 0) return;

            foreach (var dataSourceName in dataSourcesToReconnect)
            {
                var dataSource = dictionary.DataSources[dataSourceName];
                var lockObject = dictionary != null ? (object)dictionary : dataSource;

                lock (lockObject)
                {
                    var sqlSource = dataSource as StiSqlSource;
                    var resSqlCommand = sqlSource?.SqlCommand;

                    try
                    {
                        if (allowParseQuery)
                            StiDataSourceParserHelper.ConnectSqlSource(sqlSource);

                        StiDataLeader.Disconnect(dataSource);
                        StiDataLeader.Connect(dataSource);
                    }
                    finally
                    {
                        if (resSqlCommand != null)
                            sqlSource.SqlCommand = resSqlCommand;
                    }
                }
            }
            dictionary.RegRelations();
            dictionary.RegRelations(true);
            #endregion
        }

        public static void SetDefaultValueForRequestFromUserVariablesIfUserItems(StiReport report)
        {
            if (report == null) return;

            #region Calculate variables
            var tempText = new StiText { Name = "**VariableRequestFromUser**" };
            if (report.Pages.Count > 0)
                tempText.Page = report.Pages[0];

            foreach (StiVariable variable in report.Dictionary.Variables)
            {
                if (!(variable.RequestFromUser && variable.DialogInfo.ItemsInitializationType == StiItemsInitializationType.Items)) continue;
                if (report.ModifiedVariables.ContainsKey(variable.Name)) continue;

                try
                {
                    if (variable.Selection == StiSelectionMode.FromVariable && variable.InitBy == StiVariableInitBy.Expression)
                    {
                        object newValue = null;
                        try
                        {
                            if (variable.Type.IsSubclassOf(typeof(Range)) && (variable.Value != null) && variable.Value.Contains("<<|>>"))
                            {
                                int pos = variable.Value.IndexOf("<<|>>");
                                newValue = StiActivator.CreateObject(variable.Type);
                                (newValue as Range).FromObject = StiParser.ParseTextValue("{" + variable.Value.Substring(0, pos) + "}", tempText);
                                (newValue as Range).ToObject = StiParser.ParseTextValue("{" + variable.Value.Substring(pos + 5) + "}", tempText);
                            }
                            else
                            {
                                newValue = StiParser.ParseTextValue("{" + variable.Value + "}", tempText);
                            }
                        }
                        catch
                        {
                        }
                        if (variable.DialogInfo.DateTimeType == StiDateTimeType.Date && newValue is DateTime dateTimeValue)
                            newValue = dateTimeValue.Date;

                        if (!report.IsReportRenderingAfterSubmit)
                        {
                            var index = (newValue != null && variable.DialogInfo.Keys != null)
                                ? variable.DialogInfo.Keys.ToList().IndexOf(newValue?.ToString())
                                : -1;

                            if (index != -1)
                                SetVariableLabel(report, variable, variable.DialogInfo.Values.ElementAtOrDefault(index));
                        }

                        if (report.CalculationMode == StiCalculationMode.Interpretation)
                            report[variable.Name] = newValue;
                    }

                    if (variable.Selection == StiSelectionMode.FromVariable && variable.InitBy == StiVariableInitBy.Value)
                    {
                        if (!report.IsReportRenderingAfterSubmit)
                        {
                            var index = (variable.ValueObject != null && variable.DialogInfo.Keys != null)
                                ? variable.DialogInfo.Keys.ToList().IndexOf(variable.ValueObject?.ToString())
                                : -1;

                            if (index != -1)
                                SetVariableLabel(report, variable, variable.DialogInfo.Values.ElementAtOrDefault(index));
                        }
                    }

                    
                    if (variable.Selection == StiSelectionMode.First && variable.DialogInfo.Keys != null && variable.DialogInfo.Keys.Length > 0 &&
                        !variable.Type.BaseType.IsGenericType && typeof(Range) != variable.Type.BaseType)
                    {
                        var newValue = StiReport.ChangeType(variable.DialogInfo.Keys.FirstOrDefault(), variable.Type);
                        report[variable.Name] = newValue;

                        if (!report.IsReportRenderingAfterSubmit)
                            SetVariableLabel(report, variable, variable.DialogInfo.Values.FirstOrDefault());
                    }
                }
                catch
                {
                }
            }
            #endregion
        }

        private static void InitDateTimeVariables(StiReport report)
        {
            if (report.IsReportRenderingAfterSubmit) return;

            var variables = report.Dictionary.Variables.ToList()
                .Where(v => v.DialogInfo.DateTimeType == StiDateTimeType.Date 
                && v.RequestFromUser 
                && v.InitBy == StiVariableInitBy.Value 
                && (v.Type == typeof(DateTimeRange) || v.Type == typeof(DateTime))).ToList();

            variables.ForEach(v =>
            {
                if (v.Type == typeof(DateTime))
                {
                    if (v.Selection == StiSelectionMode.First && v.DialogInfo.Keys != null && v.DialogInfo.Keys.Length > 0)
                    {
                        var newValue = StiReport.ChangeType(v.DialogInfo.Keys.FirstOrDefault(), v.Type);
                        report[v.Name] = newValue;
                    }
                    else
                    {
                        report[v.Name] = StiVariable.GetDateTimeFromValue(v.GetNativeValue()).Date;
                    }
                }
                else
                {
                    var range = v.ValueObject as DateTimeRange;
                    if (range == null) return;

                    report[v.Name] = new DateTimeRange(range.FromDate.Date, range.ToDate != DateTime.MaxValue ? range.ToDate.Date.AddDays(1).AddTicks(-1) : range.ToDate.Date);
                }
            });
        }

        internal static List<string> GetDataSourcesWithRequestFromUserVariablesInCommand(StiReport report)
        {
            var list = new List<string>();
            var vars = new Hashtable();

            var tempText = new StiText
            {
                Name = "**VariableRequestFromUser**",
                Page = report.Pages[0]
            };

            foreach (StiVariable variable in report.Dictionary.Variables)
            {
                if (variable.RequestFromUser)
                    vars[variable.Name] = null;
            }

            foreach (StiDataSource ds in report.Dictionary.DataSources)
            {
                var sql = ds as StiSqlSource;
                if (sql == null) continue;

                try
                {
                    string sqlCommand = null;
                    if (report.CalculationMode == StiCalculationMode.Interpretation && (report.Variables != null))
                    {
                        var baseSqlCommand = report.Variables["**StoredDataSourceSqlCommandForInterpretationMode**" + ds.Name];
                        if (baseSqlCommand != null && (baseSqlCommand is string))
                            sqlCommand = baseSqlCommand as string;
                    }

                    if (string.IsNullOrWhiteSpace(sqlCommand))
                        sqlCommand = sql.SqlCommand;

                    var found = CheckExpressionForVariables(sqlCommand, tempText, vars);
                    if (!found)
                    {
                        foreach (StiDataParameter parameter in sql.Parameters)
                        {
                            if (!string.IsNullOrWhiteSpace(parameter.Expression) &&
                                CheckExpressionForVariables(parameter.Expression, tempText, vars))
                            {
                                found = true;
                                break;
                            }
                        }
                    }

                    if (found)
                        list.Add(ds.Name);
                }
                catch
                {
                }
            }
            return list;
        }

        private static bool CheckExpressionForVariables(string expression, StiComponent component, Hashtable vars)
        {
            try
            {
                var storeToPrint = false;
                var result = StiParser.ParseTextValue(expression, component, ref storeToPrint, false, true);
                if (result != null && result is List<StiParser.StiAsmCommand>)
                {
                    foreach (var asmCommand in result as List<StiParser.StiAsmCommand>)
                    {
                        if (asmCommand.Type == StiParser.StiAsmCommandType.PushVariable)
                        {
                            var varName = asmCommand.Parameter1.ToString();
                            if (vars.ContainsKey(varName))
                                return true;
                        }
                    }
                }
            }
            catch
            {
            }
            return false;
        }
        #endregion

        #region Methods.RequestFromUser related variables
        public static bool ApplyVariableValue(StiReport report, StiVariable variable, object variableValue)
        {
            if (!StiOptions.Engine.ReconnectDataSourcesIfRequestFromUserVariableChanged)
                return false;

            StiComponent comp = null;
            var dataSourcesToReconnect = GetDataSourcesToReconnectList(report, variable, ref comp);
            if (dataSourcesToReconnect.Count == 0)
                return false;

            SetVariableValue(report, variable, variableValue);

            //reconnect dataSources
            report.Dictionary.ConnectToDatabases(false);
            foreach (var ds in dataSourcesToReconnect)
            {
                StiDataLeader.Disconnect(ds);
                StiDataLeader.Connect(ds);
            }
            report.Dictionary.DataStore.ClearReportDatabase();

            return true;
        }

        public static void SetVariableValue(StiReport report, StiVariable variable, object variableValue)
        {
            var field = report.GetType().GetField(variable.Name);

            if (field != null)
                field.SetValue(report, variableValue);

            else
                report[variable.Name] = variableValue;
        }

        public static void RefreshDialogInfo(StiReport report, StiVariable var)
        {
            if (!StiOptions.Engine.ReconnectDataSourcesIfRequestFromUserVariableChanged) return;

            var.DialogInfo.Keys = null;
            var.DialogInfo.Values = null;
            var.DialogInfo.CheckedStates = null;
            var tempBool = false;

            FillItemsOfVariable(var, report, ref tempBool);
        }

        public static List<string> GetRelatedVariablesList(StiReport report, StiVariable var)
        {
            var relatedVariables = new List<string>();

            var dataSourceNames = new Hashtable();
            StiComponent comp = null;
            var dataSourcesToReconnect = GetDataSourcesToReconnectList(report, var, ref comp);

            //find the variables that are initialized from these reconnected dataSources
            foreach (StiVariable vr in report.Dictionary.Variables)
            {
                if (!vr.RequestFromUser || vr.DialogInfo == null)continue;
                dataSourceNames.Clear();

                if (vr.DialogInfo.ItemsInitializationType == StiItemsInitializationType.Columns)
                    StiDataSourceHelper.CheckExpression("{" + vr.DialogInfo.KeysColumn + "}", comp, dataSourceNames);

                if (vr.InitBy == StiVariableInitBy.Expression)
                {
                    StiDataSourceHelper.CheckExpression("{" + vr.Value + "}", comp, dataSourceNames);
                    if (vr.Type == typeof(Range))
                    {
                        StiDataSourceHelper.CheckExpression("{" + vr.InitByExpressionFrom + "}", comp, dataSourceNames);
                        StiDataSourceHelper.CheckExpression("{" + vr.InitByExpressionTo + "}", comp, dataSourceNames);
                    }
                }

                foreach (var ds in dataSourcesToReconnect)
                {
                    if (dataSourceNames.ContainsKey(ds.Name))
                    {
                        relatedVariables.Add(vr.Name);
                        break;
                    }
                }
            }

            return relatedVariables;
        }

        private static List<StiDataSource> GetDataSourcesToReconnectList(StiReport report, StiVariable var, ref StiComponent comp)
        {
            var vars = new Hashtable();
            vars[var.Name] = null;

            var dataSourcesToReconnect = new List<StiDataSource>();

            comp = new StiText
            {
                Name = "**DataSourceParameter**",
                Page = report.Pages[0]
            };

            //get list of all datasources, which used in the RequestFromUsers variables
            var dataSourceNames = StiDataSourceHelper.GetDataSourcesUsedInRequestFromUsersVariables(report);

            //leave dataSources with selected variable in parameters
            foreach (string dsName in dataSourceNames.Keys)
            {
                var ds = report.Dictionary.DataSources[dsName];
                foreach (StiDataParameter dp in ds.Parameters)
                {
                    if (CheckExpressionForVariables("{" + dp.Expression + "}", comp, vars))
                    {
                        dataSourcesToReconnect.Add(ds);
                        break;
                    }
                }
            }

            return dataSourcesToReconnect;
        }
        #endregion

        #region Methods.Variable labels
        public static void SetVariableLabel(StiReport report, StiVariable variable, string label)
        {
            if (report == null || label == null) return;

            lock (lockObject)
            {
                var variableName = variable?.Name;
                if (string.IsNullOrWhiteSpace(variableName)) return;

                report.Key = StiKeyHelper.GetOrGeneratedKey(report.Key);

                var labels = ReportToLabels[report.Key] as Hashtable;
                if (labels == null)
                {
                    labels = new Hashtable();
                    ReportToLabels[report.Key] = labels;
                }

                labels[variableName] = label;
            }
        }

        public static string GetVariableLabel(StiReport report, string variableName)
        {
            if (report == null)
                return string.Empty;

            if (string.IsNullOrWhiteSpace(variableName))
                return string.Empty;

            lock (lockObject)
            {
                report.Key = StiKeyHelper.GetOrGeneratedKey(report.Key);

                var labels = ReportToLabels[report.Key] as Hashtable;
                if (labels == null)
                    return string.Empty;

                var value = labels[variableName] as string;
                if (value == null)
                    return string.Empty;

                return value;
            }
        }
        #endregion
    }
}
