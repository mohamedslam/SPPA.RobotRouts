#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Base;
using System.Globalization;
using Stimulsoft.Report.Engine;
using System.Text;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;
using Stimulsoft.Report.Design;
using System.Drawing;

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiVariablesHelper
    {
        #region Fields

        private static CultureInfo en_us_culture = new CultureInfo("en-US", false);

        #endregion

        /// <summary>
        /// Filling the list of variables initialized from the database columns
        /// </summary>
        public static void FillDialogInfoItems(StiReport report)
        {
            var isColumnsInitializationTypeItems = false;
            foreach (StiVariable variable in report.Dictionary.Variables)
            {
                if (variable.RequestFromUser && variable.DialogInfo.ItemsInitializationType == StiItemsInitializationType.Columns &&
                    (variable.DialogInfo.Keys == null || variable.DialogInfo.Keys.Length == 0 || variable.DialogInfo.Values == null || variable.DialogInfo.Values.Length == 0))
                {
                    isColumnsInitializationTypeItems = true;
                    break;
                }
            }

            if (isColumnsInitializationTypeItems)
            {
                report.Dictionary.Connect();
                StiVariableHelper.FillItemsOfVariables(report);
                report.Dictionary.Disconnect();
            }
        }

        private static string GetVariableAlias(StiVariable variable)
        {
            if (string.IsNullOrEmpty(variable.Alias)) return variable.Name;
            return variable.Alias;
        }

        private class StiVariableItemsComparer : IComparer
        {
            int IComparer.Compare(Object x, Object y)
            {
                Hashtable item1 = x as Hashtable;
                Hashtable item2 = y as Hashtable;
                if (item1 != null && item2 != null)
                {
                    string value1 = !string.IsNullOrEmpty(item1["value"] as string) ? item1["value"] as string : item1["key"] as string;
                    string value2 = !string.IsNullOrEmpty(item2["value"] as string) ? item2["value"] as string : item2["key"] as string;

                    if (value1 != null && value2 != null)
                        return value1.CompareTo(value2);
                    else
                        return 0;
                }
                return 0;
            }
        }

        private static ArrayList GetItems(StiVariable variable, Hashtable values = null)
        {
            var items = new ArrayList();
            var index = 0;

            var isBindingValue = variable.DialogInfo.BindingValue && variable.DialogInfo.BindingVariable != null;
            var valueBinding = isBindingValue ? variable.DialogInfo.BindingVariable.Value : null;

            if (isBindingValue && variable.DialogInfo.BindingVariable.Selection == StiSelectionMode.First && values == null && !string.IsNullOrEmpty(valueBinding))
                values = new Hashtable() { [variable.DialogInfo.BindingVariable.Name] = valueBinding };

            if (variable.DialogInfo.Keys != null && variable.DialogInfo.Keys.Length != 0 &&
                (variable.DialogInfo.ItemsInitializationType == StiItemsInitializationType.Items ||
                (variable.DialogInfo.ItemsInitializationType == StiItemsInitializationType.Columns &&
                (!string.IsNullOrEmpty(variable.DialogInfo.KeysColumn) || !string.IsNullOrEmpty(variable.DialogInfo.ValuesColumn)))))
            {
                List<StiDialogInfoItem> itemsVariable = variable.DialogInfo.GetDialogInfoItems(variable.Type);
                Hashtable bindingValues = new Hashtable();
                var listKeys = new HashSet<object>();
                var listValues = new HashSet<object>();

                foreach (StiDialogInfoItem itemVariable in itemsVariable)
                {
                    var itemsVariableValueBinding = itemVariable.ValueBinding;
                    if (itemsVariableValueBinding == null || itemsVariableValueBinding.Count == 0)
                        itemsVariableValueBinding = new List<object>() { itemVariable.ValueBinding };

                    foreach (var itemValueBinding in itemsVariableValueBinding)
                    {
                        if (values == null || !isBindingValue || (valueBinding != null && valueBinding == Convert.ToString(itemValueBinding)) || ContainsBindingVariableValue(variable, Convert.ToString(itemValueBinding), values))
                        {
                            var item = new Hashtable();
                            item["value"] = itemVariable.Value;
                            item["key"] = itemVariable.KeyObject;
                            item["keyTo"] = itemVariable.KeyObjectTo;

                            var keyStr = itemVariable.KeyObject?.ToString();

                            if (variable.Type == typeof(DateTime) || variable.Type == typeof(DateTime?) || variable.Type == typeof(DateTimeRange) || variable.Type == typeof(DateTimeList))
                            {
                                if (itemVariable.KeyObject != null) item["key"] = GetDateTimeObject(itemVariable.KeyObject);
                                if (itemVariable.KeyObjectTo != null) item["keyTo"] = GetDateTimeObject(itemVariable.KeyObjectTo);
                            }

                            if ((string.IsNullOrEmpty(valueBinding) || item["key"] == null || bindingValues[item["key"]] == null) && 
                                (!listKeys.Contains(keyStr) || (!string.IsNullOrEmpty(item["value"] as string) && !listValues.Contains(item["value"]))))
                            {                                
                                items.Add(item);
                                listKeys.Add(keyStr);
                                listValues.Add(item["value"]);
                            }

                            if (!string.IsNullOrEmpty(valueBinding))
                            {
                                bindingValues[item["key"]] = true;
                            }
                        }
                    }

                    index++;
                }
            }

            //Check checked states
            if (isBindingValue && values == null && StiTypeFinder.FindInterface(variable.Type, typeof(IStiList)))
            {
                var bindingVariable = variable.DialogInfo.BindingVariable;
                var checkedStates = bindingVariable.DialogInfo.CheckedStates;

                if (!bindingVariable.DialogInfo.AllowUserValues && checkedStates != null && checkedStates.Length > 0 && checkedStates.All(s => !s))
                    return new ArrayList();
            }

            return index > 0 ? items : null;
        }

        private static bool ContainsBindingVariableValue(StiVariable variable, string value, Hashtable values)
        {
            if (variable.DialogInfo.BindingValue && variable.DialogInfo.BindingVariable != null && StiTypeFinder.FindInterface(variable.DialogInfo.BindingVariable.Type, typeof(IStiList)))
            {
                try
                {
                    if (!string.IsNullOrEmpty(variable.DialogInfo.BindingVariable.Value))
                    {
                        var valuesArray = JsonConvert.DeserializeObject<List<object>>(variable.DialogInfo.BindingVariable.Value);
                        if (valuesArray != null && value != null && valuesArray.Any(v => v != null && Convert.ToString(v) == Convert.ToString(value)))
                            return true;
                    }
                    else if (variable.DialogInfo.BindingVariable.DialogInfo.ValuesBindingList != null)
                    {
                        var bindingVariableItems = values != null ? values[variable.DialogInfo.BindingVariable.Name] as ArrayList : null;
                        if (bindingVariableItems != null && bindingVariableItems.Count > 0)
                        {
                            return bindingVariableItems.Cast<object>().ToList().Any(v => v != null && Convert.ToString(v) == Convert.ToString(value));
                        }
                        else
                        {
                            foreach (var bindingValues in variable.DialogInfo.BindingVariable.DialogInfo.ValuesBindingList)
                            {
                                if (bindingValues != null && value != null && bindingValues.Any(v => v != null && Convert.ToString(v) == Convert.ToString(value)))
                                {
                                    var currentItems = values != null ? values[variable.Name] as ArrayList : null;
                                    if (currentItems != null && currentItems.Count > 0)
                                        return currentItems.Cast<object>().ToList().Any(v => v != null && Convert.ToString(v) == Convert.ToString(value));
                                    else
                                        return false;
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            return false;
        }

        private static TimeSpan ParseTimeSpanValue(string value)
        {
            TimeSpan resultValue = TimeSpan.Zero;

            if (!string.IsNullOrEmpty(value) && !TimeSpan.TryParse(value, out resultValue))
            {
                var days = 0;
                var hours = 0;
                var mins = 0;
                var secs = 0;
                var msecs = 0;

                try
                {
                    var valueArray = value.Split(':');

                    if (valueArray[0].Contains("."))
                    {
                        var hourArray = valueArray[0].Split('.');
                        days = Convert.ToInt32(hourArray[0]);
                        hours = Convert.ToInt32(hourArray[1]);
                    }
                    else
                    {
                        hours = Convert.ToInt32(valueArray[0]);
                    }
                    
                    if (hours > 23)
                    {
                        days += (int)Math.Truncate(hours / 24d);
                        hours = hours % 24;
                    }

                    if (valueArray.Length > 1)
                        mins = Math.Min(59, Convert.ToInt32(valueArray[1]));

                    if (valueArray.Length > 2)
                    {
                        if (valueArray[2].Contains("."))
                        {
                            var secArray = valueArray[2].Split('.');
                            secs = Math.Min(59, Convert.ToInt32(secArray[0]));
                            msecs = Convert.ToInt32(secArray[1]);
                        }
                        else
                        {
                            secs = Math.Min(59, Convert.ToInt32(valueArray[2]));
                        }
                    }
                }
                catch { }

                return new TimeSpan(days, hours, mins, secs, msecs);
            };

            return resultValue;
        }

        private static Hashtable GetDateTimeObject(object value)
        {
            if (value is Hashtable) return (Hashtable)value;

            var dateValue = DateTime.Now;

            if (value != null && value is DateTime)
                dateValue = (DateTime)value;

            if (value != null && value is DateTimeOffset)
                dateValue = new DateTime(((DateTimeOffset)value).Ticks);

            if (value != null && value is string)
                DateTime.TryParse(Convert.ToString(value), en_us_culture, DateTimeStyles.None, out dateValue);

            var dateTime = new Hashtable();
            dateTime["year"] = dateValue.Year;
            dateTime["month"] = dateValue.Month;
            dateTime["day"] = dateValue.Day;
            dateTime["hours"] = dateValue.Hour;
            dateTime["minutes"] = dateValue.Minute;
            dateTime["seconds"] = dateValue.Second;
            if (value == null) dateTime["isNull"] = true;

            return dateTime;
        }

        private static string GetTimeSpanStringValue(object value)
        {
            if (value != null && value is TimeSpan)
            {
                var time = (TimeSpan)value;
                return new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds).ToString(); //without milliseconds
            }

            return string.Empty;
        }

        private static string GetBasicType(StiVariable variable)
        {
            if (StiTypeFinder.FindType(variable.Type, typeof(Range))) return "Range";
            if (StiTypeFinder.FindInterface(variable.Type, typeof(IStiList))) return "List";
            if (variable.Type.Name.StartsWith("Nullable")) return "NullableValue";
            return "Value";
        }

        private static string GetType(StiVariable variable)
        {
            if (variable.Type == typeof(string) || variable.Type == typeof(StringList) || variable.Type == typeof(StringRange)) return "String";
            if (variable.Type == typeof(char) || variable.Type == typeof(char?) || variable.Type == typeof(CharRange) || variable.Type == typeof(CharList)) return "Char";
            if (variable.Type == typeof(bool) || variable.Type == typeof(bool?) || variable.Type == typeof(BoolList)) return "Bool";
            if (variable.Type == typeof(DateTime) || variable.Type == typeof(DateTime?) || variable.Type == typeof(DateTimeList) || variable.Type == typeof(DateTimeRange)) return "DateTime";
            if (variable.Type == typeof(DateTimeOffset) || variable.Type == typeof(DateTimeOffset?)) return "DateTimeOffset";
            if (variable.Type == typeof(TimeSpan) || variable.Type == typeof(TimeSpan?) || variable.Type == typeof(TimeSpanList) || variable.Type == typeof(TimeSpanRange)) return "TimeSpan";
            if (variable.Type == typeof(Guid) || variable.Type == typeof(Guid?) || variable.Type == typeof(GuidList) || variable.Type == typeof(GuidRange)) return "Guid";
            if (variable.Type == typeof(Image) || variable.Type == typeof(Bitmap)) return "Image";
            if (variable.Type == typeof(float) || variable.Type == typeof(float?) || variable.Type == typeof(FloatList) || variable.Type == typeof(FloatRange)) return "Float";
            if (variable.Type == typeof(double) || variable.Type == typeof(double?) || variable.Type == typeof(DoubleList) || variable.Type == typeof(DoubleRange)) return "Double";
            if (variable.Type == typeof(Decimal) || variable.Type == typeof(Decimal?) || variable.Type == typeof(DecimalList) || variable.Type == typeof(DecimalRange)) return "Decimal";
            if (variable.Type == typeof(int) || variable.Type == typeof(int?) || variable.Type == typeof(IntList) || variable.Type == typeof(IntRange)) return "Int";
            if (variable.Type == typeof(uint) || variable.Type == typeof(uint?)) return "Uint";
            if (variable.Type == typeof(short) || variable.Type == typeof(short?) || variable.Type == typeof(ShortList) || variable.Type == typeof(ShortRange)) return "Short";
            if (variable.Type == typeof(ushort) || variable.Type == typeof(ushort?)) return "Ushort";
            if (variable.Type == typeof(long) || variable.Type == typeof(long?) || variable.Type == typeof(LongList) || variable.Type == typeof(LongRange)) return "Long";
            if (variable.Type == typeof(ulong) || variable.Type == typeof(ulong?)) return "Ulong";
            if (variable.Type == typeof(byte) || variable.Type == typeof(byte?) || variable.Type == typeof(ByteList) || variable.Type == typeof(ByteRange)) return "Byte";
            if (variable.Type == typeof(sbyte) || variable.Type == typeof(sbyte?)) return "Sbyte";

            return string.Empty;
        }

        /// <summary>
        /// Assigning sent by the client parameters to the report
        /// </summary>
        public static void ApplyReportParameters(StiReport report, Hashtable values)
        {
            if (values != null && values.Count > 0)
            {
                if (report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    foreach (StiVariable variable in report.Dictionary.Variables)
                    {
                        report[variable.Name] = variable.ValueObject;
                    }
                }

                StiParser.PrepareReportVariables(report);
                FillDialogInfoItems(report);

                if (values != null)
                {
                    foreach (string key in values.Keys)
                    {
                        var variable = report.Dictionary.Variables[key];
                        if (variable != null)
                        {
                            SetVariableValue(report, key, values[key], variable);
                            SetVariableLabel(report, variable, values[key]);
                        }
                    }
                }

                report.IsRendered = false;
            }
        }

        internal static void TransferParametersValuesToReport(StiReport report, Hashtable values)
        {
            report.Dictionary.Variables.ToList().ForEach(v =>
            {
                if (values.Contains(v.Name))
                {
                    SetVariableValue(report, v.Name, values[v.Name], v);
                    SetVariableLabel(report, v, values[v.Name]);
                    report.IsRendered = false;
                }
            });
        }

        /// <summary>
        /// Assigning sent by the client parameters to the report
        /// </summary>
        public static void ApplyReportBindingVariables(StiReport report, Hashtable values)
        {
            if (values != null)
            {
                foreach (string key in values.Keys)
                {
                    foreach (StiVariable variable in report.Dictionary.Variables)
                    {
                        if (variable.Name == key)
                            variable.Value = values[key] is ArrayList ? "" : (StiTypeFinder.FindType(variable.Type, typeof(Range)) ? GetRangeStringValue(variable, values[key] as Hashtable) : Convert.ToString(values[key]));

                        if (variable.DialogInfo.BindingVariable != null && variable.DialogInfo.BindingVariable.Name == key)
                            variable.DialogInfo.BindingVariable.Value = values[key] is ArrayList ? "" : Convert.ToString(values[key]);
                    }
                }
            }
        }

        private static string GetLabelValue(StiVariable variable, object value)
        {
            var labelIndex = variable.DialogInfo.Keys.ToList().IndexOf(value?.ToString());
            if (labelIndex >= 0 && labelIndex < variable.DialogInfo.Values.Length)
            {
                return variable.DialogInfo.Values[labelIndex];
            }

            return null;
        }

        private static void SetVariableLabel(StiReport report, StiVariable variable, object value)
        {
            if (variable != null && variable.DialogInfo.Keys?.Length > 0 && variable.DialogInfo.Values?.Length > 0)
            {
                if (value is ArrayList values)
                {
                    var labels = new List<string>();
                    foreach (object val in values)
                    {
                        var label = GetLabelValue(variable, val);
                        if (label != null) labels.Add(label);
                    }
                    StiVariableHelper.SetVariableLabel(report, variable, string.Join("; ", labels.ToArray()));
                }
                else
                {
                    var label = GetLabelValue(variable, value);
                    if (label != null) StiVariableHelper.SetVariableLabel(report, variable, label);
                }
            }
        }

        private static string GetRangeStringValue(StiVariable variable, Hashtable value)
        {
            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            object rangeObject = null;

            if (variable.Type == typeof(StringRange))
            {
                rangeObject = new StringRange(Convert.ToString(value["from"]), Convert.ToString(value["to"]));
            }
            else if (variable.Type == typeof(FloatRange))
            {
                float valueFrom = 0;
                float valueTo = 0;
                float.TryParse(Convert.ToString(value["from"]).Replace(",", decimalSeparator), out valueFrom);
                float.TryParse(Convert.ToString(value["to"]).Replace(".", ",").Replace(",", decimalSeparator), out valueTo);
                rangeObject = new FloatRange(valueFrom, valueTo);
            }
            else if (variable.Type == typeof(DoubleRange))
            {
                double valueFrom = 0;
                double valueTo = 0;
                double.TryParse(Convert.ToString(value["from"]).Replace(".", ",").Replace(",", decimalSeparator), out valueFrom);
                double.TryParse(Convert.ToString(value["to"]).Replace(".", ",").Replace(",", decimalSeparator), out valueTo);
                rangeObject = new DoubleRange(valueFrom, valueTo);
            }
            else if (variable.Type == typeof(DecimalRange))
            {
                decimal valueFrom = 0;
                decimal valueTo = 0;
                decimal.TryParse(Convert.ToString(value["from"]).Replace(".", ",").Replace(",", decimalSeparator), out valueFrom);
                decimal.TryParse(Convert.ToString(value["to"]).Replace(".", ",").Replace(",", decimalSeparator), out valueTo);
                rangeObject = new DecimalRange(valueFrom, valueTo);
            }
            else if (variable.Type == typeof(IntRange))
            {
                int valueFrom = 0;
                int valueTo = 0;
                int.TryParse(Convert.ToString(value["from"]), out valueFrom);
                int.TryParse(Convert.ToString(value["to"]), out valueTo);
                rangeObject = new IntRange(valueFrom, valueTo);
            }
            else if (variable.Type == typeof(ShortRange))
            {
                short valueFrom = 0;
                short valueTo = 0;
                short.TryParse(Convert.ToString(value["from"]), out valueFrom);
                short.TryParse(Convert.ToString(value["to"]), out valueTo);
                rangeObject = new ShortRange(valueFrom, valueTo);
            }
            else if (variable.Type == typeof(LongRange))
            {
                long valueFrom = 0;
                long valueTo = 0;
                long.TryParse(Convert.ToString(value["from"]), out valueFrom);
                long.TryParse(Convert.ToString(value["to"]), out valueTo);
                rangeObject = new LongRange(valueFrom, valueTo);
            }
            else if (variable.Type == typeof(ByteRange))
            {
                byte valueFrom = 0;
                byte valueTo = 0;
                byte.TryParse(Convert.ToString(value["from"]), out valueFrom);
                byte.TryParse(Convert.ToString(value["to"]), out valueTo);
                rangeObject = new ByteRange(valueFrom, valueTo);
            }
            else if (variable.Type == typeof(CharRange))
            {
                char valueFrom = ' ';
                char valueTo = ' ';
                char.TryParse(Convert.ToString(value["from"]), out valueFrom);
                char.TryParse(Convert.ToString(value["to"]), out valueTo);
                rangeObject = new CharRange(valueFrom, valueTo);
            }
            else if (variable.Type == typeof(DateTimeRange))
            {
                DateTime valueFrom = DateTime.Now;
                DateTime valueTo = DateTime.Now;
                DateTime.TryParse(Convert.ToString(value["from"]), en_us_culture, DateTimeStyles.None, out valueFrom);
                DateTime.TryParse(Convert.ToString(value["to"]), en_us_culture, DateTimeStyles.None, out valueTo);
                rangeObject = new DateTimeRange(valueFrom, valueTo);
            }
            else if (variable.Type == typeof(TimeSpanRange))
            {
                TimeSpan valueFrom = TimeSpan.Zero;
                TimeSpan valueTo = TimeSpan.Zero;
                TimeSpan.TryParse(Convert.ToString(value["from"]), out valueFrom);
                TimeSpan.TryParse(Convert.ToString(value["to"]), out valueTo);
                rangeObject = new TimeSpanRange(valueFrom, valueTo);
            }
            else if (variable.Type == typeof(GuidRange))
            {
                Guid valueFrom = Guid.Empty;
                Guid valueTo = Guid.Empty;
                try
                {
                    valueFrom = new Guid(Convert.ToString(value["from"]));
                    valueTo = new Guid(Convert.ToString(value["to"]));
                }
                catch
                {
                }
                return new GuidRange(valueFrom, valueTo).ToString();
            }

            return RangeConverter.RangeToString(rangeObject as Range);
        }

        /// <summary>
        /// Assigning the specified parameter to the report
        /// </summary>
        internal static void SetVariableValue(StiReport report, string paramName, object paramValue, StiVariable variable)
        {
            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            #region Restore values

            string stringValue = null;
            Hashtable values = null;
            ArrayList array = null;

            if (paramValue != null)
            {
                if (paramValue is Hashtable) values = (Hashtable)paramValue;
                else if (paramValue is ArrayList) array = (ArrayList)paramValue;
                else stringValue = Convert.ToString(paramValue);
            }
            #endregion

            #region System types

            if (stringValue == null &&
                (variable.Type == typeof(float?) || variable.Type == typeof(double?) || variable.Type == typeof(decimal?) || variable.Type == typeof(int?) ||
                variable.Type == typeof(uint?) || variable.Type == typeof(short?) || variable.Type == typeof(ushort?) || variable.Type == typeof(long?) ||
                variable.Type == typeof(ulong?) || variable.Type == typeof(byte?) || variable.Type == typeof(sbyte?) || variable.Type == typeof(char?) ||
                variable.Type == typeof(bool?) || variable.Type == typeof(DateTime?) || variable.Type == typeof(TimeSpan?) || variable.Type == typeof(Guid?)))
            {
                report[paramName] = null;
            }
            else if (variable.Type == typeof(string))
            {
                report[paramName] = paramValue;
            }
            else if (variable.Type == typeof(float) || variable.Type == typeof(float?))
            {
                float value = 0;
                float.TryParse(stringValue.Replace(".", ",").Replace(",", decimalSeparator), out value);
                report[paramName] = value;
            }
            else if (variable.Type == typeof(double) || variable.Type == typeof(double?))
            {
                double value = 0;
                double.TryParse(stringValue.Replace(".", ",").Replace(",", decimalSeparator), out value);
                report[paramName] = value;
            }
            else if (variable.Type == typeof(decimal) || variable.Type == typeof(decimal?))
            {
                decimal value = 0;
                decimal.TryParse(stringValue.Replace(".", ",").Replace(",", decimalSeparator), out value);
                report[paramName] = value;
            }
            else if (variable.Type == typeof(int) || variable.Type == typeof(int?))
            {
                int value = 0;
                int.TryParse(stringValue, out value);
                report[paramName] = value;
            }
            else if (variable.Type == typeof(uint) || variable.Type == typeof(uint?))
            {
                uint value = 0;
                uint.TryParse(stringValue, out value);
                report[paramName] = value;
            }
            else if (variable.Type == typeof(short) || variable.Type == typeof(short?))
            {
                short value = 0;
                short.TryParse(stringValue, out value);
                report[paramName] = value;
            }
            else if (variable.Type == typeof(ushort) || variable.Type == typeof(ushort?))
            {
                ushort value = 0;
                ushort.TryParse(stringValue, out value);
                report[paramName] = value;
            }
            else if (variable.Type == typeof(long) || variable.Type == typeof(long?))
            {
                long value = 0;
                long.TryParse(stringValue, out value);
                report[paramName] = value;
            }
            else if (variable.Type == typeof(ulong) || variable.Type == typeof(ulong?))
            {
                ulong value = 0;
                ulong.TryParse(stringValue, out value);
                report[paramName] = value;
            }
            else if (variable.Type == typeof(byte) || variable.Type == typeof(byte?))
            {
                byte value = 0;
                byte.TryParse(stringValue, out value);
                report[paramName] = value;
            }
            else if (variable.Type == typeof(sbyte) || variable.Type == typeof(sbyte?))
            {
                sbyte value = 0;
                sbyte.TryParse(stringValue, out value);
                report[paramName] = value;
            }
            else if (variable.Type == typeof(char) || variable.Type == typeof(char?))
            {
                char value = ' ';
                char.TryParse(stringValue, out value);
                report[paramName] = value;
            }
            else if (variable.Type == typeof(bool) || variable.Type == typeof(bool?))
            {
                bool value = false;
                bool.TryParse(stringValue, out value);
                report[paramName] = value;
            }
            else if (variable.Type == typeof(DateTime) || variable.Type == typeof(DateTime?))
            {
                DateTime value = DateTime.Now;
                DateTime.TryParse(stringValue, en_us_culture, DateTimeStyles.None, out value);
                report[paramName] = value;
            }
            else if (variable.Type == typeof(DateTimeOffset) || variable.Type == typeof(DateTimeOffset?))
            {
                DateTimeOffset value = DateTimeOffset.Now;
                DateTimeOffset.TryParse(stringValue, en_us_culture, DateTimeStyles.None, out value);
                report[paramName] = value;
            }
            else if (variable.Type == typeof(TimeSpan) || variable.Type == typeof(TimeSpan?))
            {
                report[paramName] = ParseTimeSpanValue(stringValue);
            }
            else if (variable.Type == typeof(Guid) || variable.Type == typeof(Guid?))
            {
                Guid variableGuid;
                try
                {
                    variableGuid = new Guid(stringValue);
                }
                catch
                {
                    variableGuid = Guid.Empty;
                }
                report[paramName] = variableGuid;
            }

            #endregion

            #region Ranges

            else if (variable.Type == typeof(StringRange))
            {
                report[paramName] = new StringRange(Convert.ToString(values["from"]), Convert.ToString(values["to"]));
            }
            else if (variable.Type == typeof(FloatRange))
            {
                float valueFrom = 0;
                float valueTo = 0;
                float.TryParse(Convert.ToString(values["from"]).Replace(",", decimalSeparator), out valueFrom);
                float.TryParse(Convert.ToString(values["to"]).Replace(".", ",").Replace(",", decimalSeparator), out valueTo);
                report[paramName] = new FloatRange(valueFrom, valueTo);
            }
            else if (variable.Type == typeof(DoubleRange))
            {
                double valueFrom = 0;
                double valueTo = 0;
                double.TryParse(Convert.ToString(values["from"]).Replace(".", ",").Replace(",", decimalSeparator), out valueFrom);
                double.TryParse(Convert.ToString(values["to"]).Replace(".", ",").Replace(",", decimalSeparator), out valueTo);
                report[paramName] = new DoubleRange(valueFrom, valueTo);
            }
            else if (variable.Type == typeof(DecimalRange))
            {
                decimal valueFrom = 0;
                decimal valueTo = 0;
                decimal.TryParse(Convert.ToString(values["from"]).Replace(".", ",").Replace(",", decimalSeparator), out valueFrom);
                decimal.TryParse(Convert.ToString(values["to"]).Replace(".", ",").Replace(",", decimalSeparator), out valueTo);
                report[paramName] = new DecimalRange(valueFrom, valueTo);
            }
            else if (variable.Type == typeof(IntRange))
            {
                int valueFrom = 0;
                int valueTo = 0;
                int.TryParse(Convert.ToString(values["from"]), out valueFrom);
                int.TryParse(Convert.ToString(values["to"]), out valueTo);
                report[paramName] = new IntRange(valueFrom, valueTo);
            }
            else if (variable.Type == typeof(ShortRange))
            {
                short valueFrom = 0;
                short valueTo = 0;
                short.TryParse(Convert.ToString(values["from"]), out valueFrom);
                short.TryParse(Convert.ToString(values["to"]), out valueTo);
                report[paramName] = new ShortRange(valueFrom, valueTo);
            }
            else if (variable.Type == typeof(LongRange))
            {
                long valueFrom = 0;
                long valueTo = 0;
                long.TryParse(Convert.ToString(values["from"]), out valueFrom);
                long.TryParse(Convert.ToString(values["to"]), out valueTo);
                report[paramName] = new LongRange(valueFrom, valueTo);
            }
            else if (variable.Type == typeof(ByteRange))
            {
                byte valueFrom = 0;
                byte valueTo = 0;
                byte.TryParse(Convert.ToString(values["from"]), out valueFrom);
                byte.TryParse(Convert.ToString(values["to"]), out valueTo);
                report[paramName] = new ByteRange(valueFrom, valueTo);
            }
            else if (variable.Type == typeof(CharRange))
            {
                char valueFrom = ' ';
                char valueTo = ' ';
                char.TryParse(Convert.ToString(values["from"]), out valueFrom);
                char.TryParse(Convert.ToString(values["to"]), out valueTo);
                report[paramName] = new CharRange(valueFrom, valueTo);
            }
            else if (variable.Type == typeof(DateTimeRange))
            {
                DateTime valueFrom = DateTime.Now;
                DateTime valueTo = DateTime.Now;
                DateTime.TryParse(Convert.ToString(values["from"]), en_us_culture, DateTimeStyles.None, out valueFrom);
                DateTime.TryParse(Convert.ToString(values["to"]), en_us_culture, DateTimeStyles.None, out valueTo);
                report[paramName] = new DateTimeRange(valueFrom, valueTo);
            }
            else if (variable.Type == typeof(TimeSpanRange))
            {
                TimeSpan valueFrom = TimeSpan.Zero;
                TimeSpan valueTo = TimeSpan.Zero;
                TimeSpan.TryParse(Convert.ToString(values["from"]), out valueFrom);
                TimeSpan.TryParse(Convert.ToString(values["to"]), out valueTo);
                report[paramName] = new TimeSpanRange(valueFrom, valueTo);
            }
            else if (variable.Type == typeof(GuidRange))
            {
                Guid valueFrom = Guid.Empty;
                Guid valueTo = Guid.Empty;
                try
                {
                    valueFrom = new Guid(Convert.ToString(values["from"]));
                    valueTo = new Guid(Convert.ToString(values["to"]));
                }
                catch
                {
                }
                report[paramName] = new GuidRange(valueFrom, valueTo);
            }

            #endregion

            #region Lists

            else if (variable.Type == typeof(StringList))
            {
                var list = new StringList();
                foreach (object value in array) list.Add(Convert.ToString(value));
                report[paramName] = list;
                if (variable.DialogInfo.Keys == null || variable.DialogInfo.Keys.Length == 0) variable.DialogInfo.Keys = list.ToArray();
            }
            else if (variable.Type == typeof(FloatList))
            {
                var list = new FloatList();
                foreach (object value in array)
                {
                    float listValue = 0;
                    float.TryParse(Convert.ToString(value).Replace(".", ",").Replace(",", decimalSeparator), out listValue);
                    list.Add(listValue);
                }
                report[paramName] = list;
                if (variable.DialogInfo.Keys == null || variable.DialogInfo.Keys.Length == 0) variable.DialogInfo.Keys = list.ToArray().Select(i => Convert.ToString(i)).ToArray();
            }
            else if (variable.Type == typeof(DoubleList))
            {
                var list = new DoubleList();
                foreach (object value in array)
                {
                    double listValue = 0;
                    double.TryParse(Convert.ToString(value).Replace(".", ",").Replace(",", decimalSeparator), out listValue);
                    list.Add(listValue);
                }
                report[paramName] = list;
                if (variable.DialogInfo.Keys == null || variable.DialogInfo.Keys.Length == 0) variable.DialogInfo.Keys = list.ToArray().Select(i => Convert.ToString(i)).ToArray();
            }
            else if (variable.Type == typeof(DecimalList))
            {
                var list = new DecimalList();
                foreach (object value in array)
                {
                    decimal listValue = 0;
                    decimal.TryParse(Convert.ToString(value).Replace(".", ",").Replace(",", decimalSeparator), out listValue);
                    list.Add(listValue);
                }
                report[paramName] = list;
                if (variable.DialogInfo.Keys == null || variable.DialogInfo.Keys.Length == 0) variable.DialogInfo.Keys = list.ToArray().Select(i => Convert.ToString(i)).ToArray();
            }
            else if (variable.Type == typeof(ByteList))
            {
                var list = new ByteList();
                foreach (object value in array)
                {
                    byte listValue = 0;
                    byte.TryParse(Convert.ToString(value), out listValue);
                    list.Add(listValue);
                }
                report[paramName] = list;
                if (variable.DialogInfo.Keys == null || variable.DialogInfo.Keys.Length == 0) variable.DialogInfo.Keys = list.ToArray().Select(i => Convert.ToString(i)).ToArray();
            }
            else if (variable.Type == typeof(ShortList))
            {
                var list = new ShortList();
                foreach (object value in array)
                {
                    short listValue = 0;
                    short.TryParse(Convert.ToString(value), out listValue);
                    list.Add(listValue);
                }
                report[paramName] = list;
                if (variable.DialogInfo.Keys == null || variable.DialogInfo.Keys.Length == 0) variable.DialogInfo.Keys = list.ToArray().Select(i => Convert.ToString(i)).ToArray();
            }
            else if (variable.Type == typeof(IntList))
            {
                var list = new IntList();
                foreach (object value in array)
                {
                    int listValue = 0;
                    int.TryParse(Convert.ToString(value), out listValue);
                    list.Add(listValue);
                }
                report[paramName] = list;
                if (variable.DialogInfo.Keys == null || variable.DialogInfo.Keys.Length == 0) variable.DialogInfo.Keys = list.ToArray().Select(i => Convert.ToString(i)).ToArray();
            }
            else if (variable.Type == typeof(LongList))
            {
                var list = new LongList();
                foreach (object value in array)
                {
                    long listValue = 0;
                    long.TryParse(Convert.ToString(value), out listValue);
                    list.Add(listValue);
                }
                report[paramName] = list;
                if (variable.DialogInfo.Keys == null || variable.DialogInfo.Keys.Length == 0) variable.DialogInfo.Keys = list.ToArray().Select(i => Convert.ToString(i)).ToArray();
            }
            else if (variable.Type == typeof(CharList))
            {
                var list = new CharList();
                foreach (object value in array)
                {
                    char listValue = ' ';
                    char.TryParse(Convert.ToString(value), out listValue);
                    list.Add(listValue);
                }
                report[paramName] = list;
                if (variable.DialogInfo.Keys == null || variable.DialogInfo.Keys.Length == 0) variable.DialogInfo.Keys = list.ToArray().Select(i => Convert.ToString(i)).ToArray();
            }
            else if (variable.Type == typeof(DateTimeList))
            {
                var list = new DateTimeList();
                foreach (object value in array)
                {
                    DateTime listValue;
                    DateTime.TryParse(Convert.ToString(value), en_us_culture, DateTimeStyles.None, out listValue);
                    list.Add(listValue);
                }
                report[paramName] = list;
                if (variable.DialogInfo.Keys == null || variable.DialogInfo.Keys.Length == 0) variable.DialogInfo.Keys = list.ToArray().Select(i => Convert.ToString(i)).ToArray();
            }
            else if (variable.Type == typeof(TimeSpanList))
            {
                var list = new TimeSpanList();
                foreach (object value in array)
                {
                    TimeSpan listValue = TimeSpan.Zero;
                    TimeSpan.TryParse(Convert.ToString(value), out listValue);
                    list.Add(listValue);
                }
                report[paramName] = list;
                if (variable.DialogInfo.Keys == null || variable.DialogInfo.Keys.Length == 0) variable.DialogInfo.Keys = list.ToArray().Select(i => Convert.ToString(i)).ToArray();
            }
            else if (variable.Type == typeof(BoolList))
            {
                var list = new BoolList();
                foreach (object value in array)
                {
                    bool listValue;
                    bool.TryParse(Convert.ToString(value), out listValue);
                    list.Add(listValue);
                }
                report[paramName] = list;
                if (variable.DialogInfo.Keys == null || variable.DialogInfo.Keys.Length == 0) variable.DialogInfo.Keys = list.ToArray().Select(i => Convert.ToString(i)).ToArray();
            }
            else if (variable.Type == typeof(GuidList))
            {
                var list = new GuidList();
                foreach (object value in array)
                {
                    Guid listValue;
                    try
                    {
                        listValue = new Guid(Convert.ToString(value));
                    }
                    catch
                    {
                        listValue = Guid.Empty;
                    }

                    list.Add(listValue);
                }

                report[paramName] = list;
            }

            #endregion
        }

        public static Hashtable GetVariables(StiReport report, Hashtable values, bool sortDataItems)
        {
            FillDialogInfoItems(report);

            var variables = new Hashtable();
            var binding = new Hashtable();
            var index = 0;

            foreach (StiVariable variable in report.Dictionary.Variables)
            {
                if (variable.RequestFromUser)
                {
                    if (variable.DialogInfo.BindingValue && variable.DialogInfo.BindingVariable != null) binding[variable.DialogInfo.BindingVariable.Name] = true;

                    var reportVariableName = StiNameValidator.CorrectName(variable.Name);
                    var var = new Hashtable();
                    var["name"] = variable.Name;
                    var["alias"] = GetVariableAlias(variable);
                    var["category"] = variable.Category;
                    var["isCategory"] = variable.IsCategory;
                    var["readOnly"] = variable.ReadOnly;
                    var["description"] = variable.Description;
                    var["basicType"] = GetBasicType(variable);
                    var["type"] = GetType(variable);
                    var["allowUserValues"] = variable.DialogInfo.AllowUserValues;
                    var["dateTimeType"] = variable.DialogInfo.DateTimeType.ToString();
                    var["sortDirection"] = variable.DialogInfo.SortDirection.ToString();
                    var["sortField"] = variable.DialogInfo.SortField.ToString();
                    var["formatMask"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(variable.DialogInfo.Mask));
                    var["checkedStates"] = variable.DialogInfo.CheckedStates != null ? new ArrayList(variable.DialogInfo.CheckedStates) : null;
                    var["isFirstInitialization"] = values == null;

                    var items = GetItems(variable, values);

                    // The 'values' collection is empty for first variables init, and not empty for get dependent variables
                    if (values != null && values.Contains(variable.Name))
                    {
                        if (values[variable.Name] is ArrayList && (items == null || items.Count == 0))
                        {
                            values.Remove(variable.Name);
                        }
                        else
                        {
                            var["value"] = values[variable.Name];

                            //Reset binding variable values
                            if (variable.DialogInfo.BindingValue && var["value"] is ArrayList && items != null && !StiTypeFinder.FindInterface(variable.DialogInfo.BindingVariable?.Type, typeof(IStiList)))
                            {
                                var["value"] = new ArrayList();
                                foreach (Hashtable item in items)
                                    ((ArrayList)var["value"]).Add(item["key"] != null ? item["key"] : item["value"]);
                            }
                        }
                    }
                    else if (variable.Selection == StiSelectionMode.Nothing)
                        var["value"] = string.Empty;
                    else if (variable.Selection == StiSelectionMode.First)
                        var["value"] = (items != null && items.Count > 0) ? ((Hashtable)items[0])["key"] : string.Empty;
                    else
                        var["value"] = (variable.InitBy == StiVariableInitBy.Value) ? variable.Value : report[reportVariableName];

                    var["key"] = (variable.InitBy == StiVariableInitBy.Value)
                        ? variable.ValueObject
                        : (values != null && values[variable.Name] != null && (variable.Type == typeof(DateTime) || variable.Type == typeof(DateTimeOffset)))
                            ? GetDateTimeObject(values[variable.Name]) 
                            : report[reportVariableName];
                    
                    var["keyTo"] = string.Empty;

                    // Find selected item by key
                    Hashtable selectedItem = null;

                    if (items != null && items.Count > 0)
                    {
                        if (variable.Selection == StiSelectionMode.First)
                        {
                            selectedItem = (Hashtable)items[0];
                        }
                        else if (variable.Selection == StiSelectionMode.Nothing)
                        {
                            selectedItem = new Hashtable();
                            selectedItem["value"] = "";
                            selectedItem["key"] = variable.Type == typeof(DateTime) ? GetDateTimeObject(DateTime.Now) : null;
                            selectedItem["keyTo"] = variable.Type == typeof(DateTime) ? GetDateTimeObject(DateTime.Now) : null;
                        }

                        string stringValue = Convert.ToString(var["value"]);
                        foreach (Hashtable item in items)
                        {
                            if (Convert.ToString(item["key"]) == stringValue)
                            {
                                selectedItem = item;
                                break;
                            }
                        }

                        if (selectedItem == null && variable.DialogInfo.BindingValue && variable.DialogInfo.BindingVariable != null)
                        {
                            selectedItem = (Hashtable)items[0];
                        }

                        if (variable.DialogInfo.ItemsInitializationType == StiItemsInitializationType.Columns && sortDataItems)
                        {
                            items.Sort(new StiVariableItemsComparer());
                        }
                    }
                    else if (variable.DialogInfo.BindingValue && variable.DialogInfo.BindingVariable != null && !Convert.ToBoolean(binding[variable.Name]) && !(variable.InitBy == StiVariableInitBy.Value && !string.IsNullOrEmpty(variable.Value)))
                    {
                        var["value"] = "";
                        var["key"] = null;
                        var["keyTo"] = null;
                    }

                    var["items"] = items;

                    // Value
                    if ((string)var["basicType"] == "Value" || (string)var["basicType"] == "NullableValue")
                    {
                        if (selectedItem != null)
                        {
                            var["key"] = selectedItem["key"];
                            var["value"] = selectedItem["value"];
                            if (variable.DialogInfo.AllowUserValues || var["value"] == null || (var["value"] is string && (string)var["value"] == "")) var["value"] = var["key"];

                            // Update binding variables for selected value
                            foreach (StiVariable bindingVariable in report.Dictionary.Variables)
                            {
                                if (bindingVariable.DialogInfo.BindingVariable != null && bindingVariable.DialogInfo.BindingVariable.Name == variable.Name)
                                {
                                    bindingVariable.DialogInfo.BindingVariable.ValueObject = var["key"];
                                }
                            }
                        }

                        if ((string)var["type"] == "DateTime" || (string)var["type"] == "DateTimeOffset") 
                            var["key"] = GetDateTimeObject(var["key"]);
                        
                        if ((string)var["type"] == "TimeSpan") 
                            var["key"] = GetTimeSpanStringValue(var["key"]);
                    }

                    // Range
                    if ((string)var["basicType"] == "Range")
                    {
                        var fromObject = variable.InitBy == StiVariableInitBy.Value ? ((Range)variable.ValueObject)?.FromObject : ((Range)report[reportVariableName]).FromObject;
                        var toObject = variable.InitBy == StiVariableInitBy.Value ? ((Range)variable.ValueObject)?.ToObject : ((Range)report[reportVariableName])?.ToObject;

                        if ((string)var["type"] == "DateTime" || (string)var["type"] == "DateTimeOffset")
                        {
                            var["key"] = GetDateTimeObject(fromObject);
                            var["keyTo"] = GetDateTimeObject(toObject);
                        }
                        else if ((string)var["type"] == "TimeSpan")
                        {
                            var["key"] = GetTimeSpanStringValue(fromObject);
                            var["keyTo"] = GetTimeSpanStringValue(toObject);
                        }
                        else
                        {
                            var["key"] = fromObject?.ToString();
                            var["keyTo"] = toObject?.ToString();
                        }
                    }

                    variables[index.ToString()] = var;
                    index++;
                }
            }

            if (variables.Count > 0)
            {
                foreach (string name in binding.Keys)
                    foreach (Hashtable var in variables.Values)
                        if ((string)var["name"] == name) var["binding"] = true;

                return variables;
            }

            return null;
        }

        public static Hashtable GetVariablesValues(StiReport report)
        {
            var variables = new Hashtable();

            foreach (StiVariable variable in report.Dictionary.Variables)
            {
                if (!variable.ReadOnly)
                {
                    ArrayList items = GetItems(variable);
                    var reportVariableName = StiNameValidator.CorrectName(variable.Name);
                    object value = null;

                    if (variable.Selection == StiSelectionMode.First)
                        value = (items != null && items.Count > 0) ? ((Hashtable)items[0])["key"] : string.Empty;
                    else
                        value = (variable.InitBy == StiVariableInitBy.Value) ? variable.Value : report[reportVariableName];

                    variables[variable.Name] = value != null ? value.ToString() : string.Empty;
                }
            }

            return variables;
        }
    }
}
