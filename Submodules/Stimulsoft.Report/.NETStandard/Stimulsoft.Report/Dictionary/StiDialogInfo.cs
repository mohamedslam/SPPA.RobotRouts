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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
using Stimulsoft.Report.Design;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using System.Threading;
using Stimulsoft.Base.Helpers;
using System.Runtime.InteropServices.ComTypes;
using System.Linq;
using Stimulsoft.Report.Dashboard;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Dictionary
{
    #region StiDialogInfo
    [TypeConverter(typeof(Stimulsoft.Report.Dictionary.Design.StiDialogInfoConverter))]
    public class StiDialogInfo : ICloneable
    {
        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            var info = MemberwiseClone() as StiDialogInfo;

            info.Keys = this.Keys?.Clone() as string[];
            info.Values = this.Values?.Clone() as string[];
            info.CheckedStates = this.CheckedStates?.Clone() as bool[];
            info.ValuesBindingList = this.ValuesBindingList?.Clone() as List<object>[];

            return info;
        }
        #endregion

        #region IStiJsonReportObject.override
        internal string jsonLoadedBindingVariableName;
        public JObject SaveToJsonObject()
        {
            var jObject = new JObject();

            // StiDialogInfo
            jObject.AddPropertyEnum(nameof(DateTimeType), DateTimeType, StiDateTimeType.Date);            
            jObject.AddPropertyEnum(nameof(SortDirection), SortDirection, StiVariableSortDirection.Asc);
            jObject.AddPropertyEnum(nameof(SortField), SortField, StiVariableSortField.Label);
            jObject.AddPropertyEnum(nameof(ItemsInitializationType), ItemsInitializationType, StiItemsInitializationType.Items);
            jObject.AddPropertyStringNullOrEmpty(nameof(KeysColumn), KeysColumn);
            jObject.AddPropertyStringNullOrEmpty(nameof(ValuesColumn), ValuesColumn);
            jObject.AddPropertyStringNullOrEmpty(nameof(CheckedColumn), CheckedColumn);
            jObject.AddPropertyStringNullOrEmpty(nameof(BindingValuesColumn), BindingValuesColumn);
            jObject.AddPropertyStringNullOrEmpty(nameof(Mask), Mask);
            jObject.AddPropertyBool(nameof(AllowUserValues), AllowUserValues, true);
            jObject.AddPropertyBool(nameof(BindingValue), BindingValue);

            if (Keys != null)
                jObject.AddPropertyStringArray(nameof(Keys), Keys);

            if (Values != null)
                jObject.AddPropertyStringArray(nameof(Values), Values);

            if (CheckedStates != null)
                jObject.AddPropertyBoolArray(nameof(CheckedStates), CheckedStates, true);

            if (BindingVariable != null)
                jObject.AddPropertyStringNullOrEmpty(nameof(BindingVariable), BindingVariable.Name);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject, StiReport report)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(DateTimeType):
                        this.DateTimeType = property.DeserializeEnum<StiDateTimeType>();
                        break;                                            

                    case nameof(SortDirection):
                        this.SortDirection = property.DeserializeEnum<StiVariableSortDirection>(); 
                        break;

                    case nameof(SortField):
                        this.SortField = property.DeserializeEnum<StiVariableSortField>(); 
                        break;

                    case nameof(ItemsInitializationType):
                        this.ItemsInitializationType = property.DeserializeEnum<StiItemsInitializationType>(); 
                        break;

                    case nameof(KeysColumn):
                        this.KeysColumn = property.DeserializeString();
                        break;

                    case nameof(ValuesColumn):
                        this.ValuesColumn = property.DeserializeString();
                        break;

                    case nameof(CheckedColumn):
                        this.CheckedColumn = property.DeserializeString();
                        break;

                    case nameof(BindingValuesColumn):
                        this.BindingValuesColumn = property.DeserializeString();
                        break;

                    case nameof(Mask):
                        this.Mask = property.DeserializeString();
                        break;

                    case nameof(AllowUserValues):
                        this.AllowUserValues = property.DeserializeBool();
                        break;

                    case nameof(BindingValue):
                        this.BindingValue = property.DeserializeBool();
                        break;

                    case nameof(Keys):
                        this.Keys = property.DeserializeStringArray();
                        break;

                    case nameof(Values):
                        this.Values = property.DeserializeStringArray();
                        break;

                    case nameof(CheckedStates):
                        this.CheckedStates = property.DeserializeBoolArray();
                        break;

                    case nameof(BindingVariable):
                        this.jsonLoadedBindingVariableName = property.DeserializeString();
                        report.jsonLoaderHelper.DialogInfo.Add(this);
                        break;
                }
            }
        }
        #endregion

        #region Properties
        [StiSerializable]
        [DefaultValue(StiDateTimeType.Date)]
        public StiDateTimeType DateTimeType { get; set; } = StiDateTimeType.Date;
                
        [StiSerializable]
        [DefaultValue(StiVariableSortDirection.Asc)]
        public StiVariableSortDirection SortDirection { get; set; } = StiVariableSortDirection.Asc;

        [StiSerializable]
        [DefaultValue(StiVariableSortField.Label)]
        public StiVariableSortField SortField { get; set; } = StiVariableSortField.Label;

        [StiSerializable]
        [DefaultValue(StiItemsInitializationType.Items)]
        public StiItemsInitializationType ItemsInitializationType { get; set; } = StiItemsInitializationType.Items;

        [StiSerializable]
        [DefaultValue("")]
        public string KeysColumn { get; set; } = string.Empty;

        [StiSerializable]
        [DefaultValue("")]
        public string ValuesColumn { get; set; } = string.Empty;

        [StiSerializable]
        [DefaultValue("")]
        public string CheckedColumn { get; set; } = string.Empty;

        [StiSerializable]
        [DefaultValue("")]
        public StiVariable BindingVariable { get; set; }

        [StiSerializable]
        [DefaultValue("")]
        public string BindingValuesColumn { get; set; } = string.Empty;

        [StiSerializable]
        [DefaultValue("")]
        public string Mask { get; set; } = string.Empty;

        [StiSerializable]
        [DefaultValue(true)]
        public bool AllowUserValues { get; set; } = true;

        [StiSerializable]
        [DefaultValue(false)]
        public bool BindingValue { get; set; }

        [StiSerializable(StiSerializationVisibility.List)]
        public string[] Keys 
        { 
            get; 
            set; 
        } = new string[0];

        [StiSerializable(StiSerializationVisibility.List)]
        public string[] Values { get; set; } = new string[0];

        [StiSerializable(StiSerializationVisibility.List)]
        public bool[] CheckedStates { get; set; } = new bool[0];

        public List<object>[] ValuesBindingList { get; set; } = new List<object>[0];

        public bool IsDefault => AllowUserValues
                    && DateTimeType == StiDateTimeType.Date                    
                    && SortDirection == StiVariableSortDirection.Asc
                    && SortField == StiVariableSortField.Label
                    && BindingVariable == null
                    && (Keys == null || Keys.Length == 0)
                    && (Values == null || Values.Length == 0)
                    && (CheckedStates == null || CheckedStates.Length == 0)
                    && string.IsNullOrEmpty(Mask)
                    && string.IsNullOrEmpty(KeysColumn)
                    && string.IsNullOrEmpty(ValuesColumn)
                    && string.IsNullOrEmpty(CheckedColumn)
                    && string.IsNullOrEmpty(BindingValuesColumn);
        #endregion

        #region Methods
        public static string Convert(object value)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                return value == null ? string.Empty : value.ToString();
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        public List<StiDialogInfoItem> GetDialogInfoItems(Type type, CultureInfo culture = null, string[] keys = null)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = culture ?? new CultureInfo("en-US", false);

                var items = new List<StiDialogInfoItem>();

                if (keys == null)
                    keys = Keys;

                if (keys == null)
                    return items;

                int index = 0;
                foreach (string key in keys)
                {
                    StiDialogInfoItem item = null;

                    object keyObject = null;
                    object keyObjectTo = null;
                    try
                    {
                        #region StiExpressionDialogInfoItem
                        if (key.StartsWith("{") && key.EndsWith("}"))
                        {
                            if (StiTypeFinder.FindType(type, typeof(Range)))
                            {
                                if (key.Contains("<<|>>"))
                                {
                                    var str = key.Substring(1, key.Length - 2);

                                    var strs = str.Split(new string[] { "<<|>>" }, StringSplitOptions.None);
                                    if (strs.Length == 2)
                                    {
                                        keyObject = strs[0];
                                        keyObjectTo = strs[1];
                                    }
                                    else
                                    {
                                        keyObject = key.Substring(1, key.Length - 2);
                                    }
                                }
                                else
                                {
                                    keyObject = key.Substring(1, key.Length - 2);
                                }

                                item = new StiExpressionRangeDialogInfoItem();
                            }
                            else
                            {
                                keyObject = key.Substring(1, key.Length - 2);
                                item = new StiExpressionDialogInfoItem();
                            }
                        }
                        #endregion

                        #region StiLongDialogInfoItem
                        else if (
                            type == typeof(sbyte) ||
                            type == typeof(byte) ||
                            type == typeof(short) ||
                            type == typeof(ushort) ||
                            type == typeof(int) ||
                            type == typeof(uint) ||
                            type == typeof(long) ||
                            type == typeof(ulong) ||
                            type == typeof(sbyte?) ||
                            type == typeof(byte?) ||
                            type == typeof(short?) ||
                            type == typeof(ushort?) ||
                            type == typeof(int?) ||
                            type == typeof(uint?) ||
                            type == typeof(long?) ||
                            type == typeof(ulong?) ||
                            type == typeof(ByteList) ||
                            type == typeof(ShortList) ||
                            type == typeof(IntList) ||
                            type == typeof(LongList))
                        {
                            keyObject = StiValueHelper.TryToLong(key);
                            item = new StiLongDialogInfoItem();
                        }
                        #endregion

                        #region StiStringDialogInfoItem
                        else if (type == typeof(string) || type == typeof(StringList))
                        {
                            keyObject = key;
                            item = new StiStringDialogInfoItem();
                        }
                        #endregion

                        #region StiDoubleDialogInfoItem
                        else if (
                            type == typeof(double) ||
                            type == typeof(float) ||
                            type == typeof(double?) ||
                            type == typeof(float?) ||
                            type == typeof(DoubleList) ||
                            type == typeof(FloatList))
                        {
                            keyObject = StiValueHelper.TryToDouble(key);
                            item = new StiDoubleDialogInfoItem();
                        }
                        #endregion

                        #region StiDecimalDialogInfoItem
                        else if (
                            type == typeof(decimal) ||
                            type == typeof(decimal?) ||
                            type == typeof(DecimalList))
                        {
                            keyObject = StiValueHelper.TryToDecimal(key);
                            item = new StiDecimalDialogInfoItem();
                        }
                        #endregion

                        #region StiDateTimeDialogInfoItem
                        else if (type == typeof(DateTime) || type == typeof(DateTime?) || type == typeof(DateTimeList))
                        {
                            keyObject = StiValueHelper.TryToDateTime(key);
                            item = new StiDateTimeDialogInfoItem();
                        }
                        #endregion

                        #region StiTimeSpanDialogInfoItem
                        else if (type == typeof(TimeSpan) || type == typeof(TimeSpan?) || type == typeof(TimeSpanList))
                        {
                            keyObject = StiValueHelper.TryToTimeSpan(key);
                            item = new StiTimeSpanDialogInfoItem();
                        }
                        #endregion

                        #region StiBoolDialogInfoItem
                        else if (type == typeof(bool) || type == typeof(bool?) || type == typeof(BoolList))
                        {
                            keyObject = StiValueHelper.TryToBool(key);
                            item = new StiBoolDialogInfoItem();
                        }
                        #endregion

                        #region StiCharDialogInfoItem
                        else if (type == typeof(char) || type == typeof(char?) || type == typeof(CharList))
                        {
                            keyObject = string.IsNullOrEmpty(key) ? ' ' : (object)StiValueHelper.TryToChar(key);
                            item = new StiCharDialogInfoItem();
                        }
                        #endregion

                        #region StiGuidDialogInfoItem
                        else if (type == typeof(Guid) || type == typeof(Guid?) || type == typeof(GuidList))
                        {
                            keyObject = StiValueHelper.TryToGuid(key);
                            item = new StiGuidDialogInfoItem();
                        }
                        #endregion

                        #region StiImageDialogInfoItem
                        else if (type == typeof(Image) || type == typeof(ImageList) || type == typeof(Bitmap))
                        {
                            keyObject = StiImageConverter.StringToImage(key);
                            item = new StiImageDialogInfoItem();
                        }
                        #endregion

                        #region StiLongRangeDialogInfoItem
                        else if (type == typeof(ByteRange) || type == typeof(ShortRange) ||
                            type == typeof(IntRange) || type == typeof(LongRange))
                        {
                            var range = RangeConverter.StringToRange(key);
                            item = new StiLongRangeDialogInfoItem();
                            keyObject = StiConvert.ChangeType(range.FromObject, typeof(long));
                            keyObjectTo = StiConvert.ChangeType(range.ToObject, typeof(long));
                        }
                        #endregion

                        #region StiStringRangeDialogInfoItem
                        else if (type == typeof(StringRange))
                        {
                            var range = RangeConverter.StringToRange(key);
                            item = new StiStringRangeDialogInfoItem();
                            keyObject = range.FromObject.ToString();
                            keyObjectTo = range.ToObject.ToString();
                        }
                        #endregion

                        #region StiDoubleRangeDialogInfoItem
                        else if (type == typeof(DoubleRange) || type == typeof(FloatRange))
                        {
                            var range = RangeConverter.StringToRange(key);
                            item = new StiDoubleRangeDialogInfoItem();
                            keyObject = StiConvert.ChangeType(range.FromObject, typeof(double));
                            keyObjectTo = StiConvert.ChangeType(range.ToObject, typeof(double));
                        }
                        #endregion

                        #region StiDecimalRangeDialogInfoItem
                        else if (type == typeof(DecimalRange))
                        {
                            var range = RangeConverter.StringToRange(key);
                            item = new StiDecimalRangeDialogInfoItem();
                            keyObject = StiConvert.ChangeType(range.FromObject, typeof(decimal));
                            keyObjectTo = StiConvert.ChangeType(range.ToObject, typeof(decimal));
                        }
                        #endregion

                        #region StiDateTimeRangeDialogInfoItem
                        else if (type == typeof(DateTimeRange))
                        {
                            var range = RangeConverter.StringToRange(key);
                            item = new StiDateTimeRangeDialogInfoItem();
                            keyObject = range.FromObject;
                            keyObjectTo = range.ToObject;
                        }
                        #endregion

                        #region StiTimeSpanRangeDialogInfoItem
                        else if (type == typeof(TimeSpanRange))
                        {
                            var range = RangeConverter.StringToRange(key);
                            item = new StiTimeSpanRangeDialogInfoItem();
                            keyObject = range.FromObject;
                            keyObjectTo = range.ToObject;
                        }
                        #endregion

                        #region StiCharRangeDialogInfoItem
                        else if (type == typeof(CharRange))
                        {
                            var range = RangeConverter.StringToRange(key);
                            item = new StiCharRangeDialogInfoItem();
                            keyObject = range.FromObject;
                            keyObjectTo = range.ToObject;
                        }
                        #endregion

                        #region StiGuidRangeDialogInfoItem
                        else if (type == typeof(GuidRange))
                        {
                            var range = RangeConverter.StringToRange(key);
                            item = new StiGuidRangeDialogInfoItem();
                            keyObject = range.FromObject;
                            keyObjectTo = range.ToObject;
                        }
                        #endregion

                        item.KeyObject = keyObject;
                        item.KeyObjectTo = keyObjectTo;
                    }
                    catch
                    {
                    }

                    if (item != null)
                    {
                        item.Value = (Values.Length > index && !string.IsNullOrEmpty(Values[index])) ? Values[index] : string.Empty;
                        item.Checked = CheckedStates != null && CheckedStates.Length > index ? CheckedStates[index] : true;
                        item.ValueBinding = (ValuesBindingList.Length > index) ? ValuesBindingList[index] : null;
                        items.Add(item);
                    }
                    index++;
                }

                return OrderBy(items);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        public List<StiDialogInfoItem> GetAndParseDialogInfoItems(Type type, CultureInfo culture = null, string[] keys = null, StiReport report = null)
        {
            var list = GetDialogInfoItems(type, culture, keys);
            var parsedList = new List<StiDialogInfoItem>();

            foreach (StiDialogInfoItem item in list)
            {
                if (item is StiExpressionDialogInfoItem || item is StiExpressionRangeDialogInfoItem)
                {
                    var emptyItem = GetDialogInfoItems(type, culture, new string[1] { "" }).FirstOrDefault();
                    emptyItem.Checked = item.Checked;
                    emptyItem.Label = item.Label;
                    emptyItem.KeyObject = StiConvert.ChangeType(StiReportParser.Parse(item.KeyObject as string, report, false), type);

                    if (item is StiExpressionRangeDialogInfoItem)
                        emptyItem.KeyObjectTo = StiReportParser.Parse(item.KeyObjectTo as string, report, false);

                    parsedList.Add(emptyItem);
                }
                else
                {
                    parsedList.Add(item);
                }
            }
            return parsedList;
        }

        public List<StiDialogInfoItem> OrderBy(List<StiDialogInfoItem> items)
        {
            switch (SortField)
            {
                case StiVariableSortField.Key:
                    {
                        if (SortDirection == StiVariableSortDirection.None)
                            return items;

                        else if (SortDirection == StiVariableSortDirection.Asc)
                            return items.OrderBy(i => i.KeyObject).ToList();

                        else
                            return items.OrderByDescending(i => i.KeyObject).ToList();
                    }

                case StiVariableSortField.Label:
                    {
                        if (SortDirection == StiVariableSortDirection.None)
                            return items;

                        else if (SortDirection == StiVariableSortDirection.Asc)
                            return items.OrderBy(i => i.Value).ToList();

                        else
                            return items.OrderByDescending(i => i.Value).ToList();
                    }

                default:
                    return items;
            }
        }

        public void SetDialogInfoItems(List<StiDialogInfoItem> items, Type type)
        {
            if (items == null || items.Count == 0)
            {
                this.Keys = null;
                this.Values = null;
                this.CheckedStates = null;
                return;
            }

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                this.Keys = new string[items.Count];
                this.Values = new string[items.Count];
                this.CheckedStates = new bool[items.Count];
                this.ValuesBindingList = new List<object>[items.Count];

                int index = 0;
                foreach (StiDialogInfoItem item in items)
                {
                    object keyObject = null;

                    try
                    {
                        #region Expression
                        if (item is StiExpressionDialogInfoItem)
                        {
                            keyObject = string.Format("{{{0}}}", item.KeyObject);
                        }
                        #endregion

                        #region Expression Range
                        else if (item is StiExpressionRangeDialogInfoItem)
                        {
                            keyObject = string.Format("{{{0}<<|>>{1}}}", item.KeyObject, item.KeyObjectTo);
                        }
                        #endregion

                        #region Integers, Numeric
                        else if (item is StiLongDialogInfoItem || item is StiDoubleDialogInfoItem ||
                            item is StiDecimalDialogInfoItem || item is StiDateTimeDialogInfoItem ||
                            item is StiTimeSpanDialogInfoItem || item is StiBoolDialogInfoItem ||
                            item is StiCharDialogInfoItem || item is StiGuidDialogInfoItem ||
                            item is StiStringDialogInfoItem)
                        {
                            keyObject = item.KeyObject.ToString();
                        }
                        #endregion

                        #region Image
                        else if (item is StiImageDialogInfoItem)
                        {
                            keyObject = StiImageConverter.ImageToString(item.KeyObject as Image);
                        }
                        #endregion

                        #region Range
                        else if (
                            item is StiLongRangeDialogInfoItem || item is StiDoubleRangeDialogInfoItem ||
                            item is StiDecimalRangeDialogInfoItem || item is StiDateTimeRangeDialogInfoItem ||
                            item is StiTimeSpanRangeDialogInfoItem || item is StiCharRangeDialogInfoItem ||
                            item is StiGuidRangeDialogInfoItem || item is StiStringRangeDialogInfoItem)
                        {
                            var range = (Range)StiActivator.CreateObject(type);
                            range.Parse(item.KeyObject?.ToString(), item.KeyObjectTo?.ToString());
                            keyObject = RangeConverter.RangeToString(range);
                        }
                        #endregion
                    }
                    catch
                    {
                    }

                    this.Keys[index] = keyObject == null ? string.Empty : keyObject.ToString();
                    this.Values[index] = item.Value;
                    this.CheckedStates[index] = item.Checked;
                    this.ValuesBindingList[index] = item.ValueBinding;
                    index++;
                }
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }
        #endregion

        public StiDialogInfo()
        {
        }

        public StiDialogInfo(StiDateTimeType type, string mask, bool allowUserValues, string[] keys, string[] values)
        {
            this.DateTimeType = type;
            this.Mask = mask;
            this.AllowUserValues = allowUserValues;
            this.Keys = keys;
            this.Values = values;
            this.ItemsInitializationType = StiItemsInitializationType.Items;
        }

        public StiDialogInfo(StiDateTimeType type, string mask, bool allowUserValues, string[] keys, string[] values, bool[] checkedStates)
        {
            this.DateTimeType = type;
            this.Mask = mask;
            this.AllowUserValues = allowUserValues;
            this.Keys = keys;
            this.Values = values;
            this.CheckedStates = checkedStates;
            this.ItemsInitializationType = StiItemsInitializationType.Items;
        }

        public StiDialogInfo(StiDateTimeType type, string mask, bool allowUserValues, StiVariableSortDirection sortDirection,
            string[] keys, string[] values, bool[] checkedStates)
        {
            this.DateTimeType = type;
            this.Mask = mask;
            this.AllowUserValues = allowUserValues;
            this.Keys = keys;
            this.Values = values;
            this.CheckedStates = checkedStates;
            this.ItemsInitializationType = StiItemsInitializationType.Items;
            this.SortDirection = sortDirection;
        }

        public StiDialogInfo(StiDateTimeType type, string mask, bool allowUserValues, string keysColumn, string valuesColumn)
        {
            this.DateTimeType = type;
            this.Mask = mask;
            this.AllowUserValues = allowUserValues;
            this.ItemsInitializationType = StiItemsInitializationType.Columns;
            this.KeysColumn = keysColumn;
            this.ValuesColumn = valuesColumn;
        }

        public StiDialogInfo(StiDateTimeType type, string mask, bool allowUserValues, string keysColumn, string valuesColumn,
            StiVariableSortDirection sortDirection, StiVariableSortField sortField)
        {
            this.DateTimeType = type;
            this.Mask = mask;
            this.AllowUserValues = allowUserValues;
            this.ItemsInitializationType = StiItemsInitializationType.Columns;
            this.KeysColumn = keysColumn;
            this.ValuesColumn = valuesColumn;
            this.SortDirection = sortDirection;
            this.SortField = sortField;
        }

        public StiDialogInfo(StiDateTimeType type, string mask, bool allowUserValues, string keysColumn, string valuesColumn, string checkedColumn,
            StiVariableSortDirection sortDirection, StiVariableSortField sortField)
        {
            this.DateTimeType = type;
            this.Mask = mask;
            this.AllowUserValues = allowUserValues;
            this.ItemsInitializationType = StiItemsInitializationType.Columns;
            this.KeysColumn = keysColumn;
            this.ValuesColumn = valuesColumn;
            this.CheckedColumn = checkedColumn;
            this.SortDirection = sortDirection;
            this.SortField = sortField;
        }

        public StiDialogInfo(StiDateTimeType type, string mask, bool allowUserValues, string keysColumn, string valuesColumn,
            bool bindingValue, StiVariable bindingVariable, string bindingValuesColumn)
        {
            this.DateTimeType = type;
            this.Mask = mask;
            this.AllowUserValues = allowUserValues;
            this.ItemsInitializationType = StiItemsInitializationType.Columns;
            this.KeysColumn = keysColumn;
            this.ValuesColumn = valuesColumn;
            this.BindingValue = bindingValue;
            this.BindingVariable = bindingVariable;
            this.BindingValuesColumn = bindingValuesColumn;
        }

        public StiDialogInfo(StiDateTimeType type, string mask, bool allowUserValues, string keysColumn, string valuesColumn, string checkedColumn,
            bool bindingValue, StiVariable bindingVariable, string bindingValuesColumn)
        {
            this.DateTimeType = type;
            this.Mask = mask;
            this.AllowUserValues = allowUserValues;
            this.ItemsInitializationType = StiItemsInitializationType.Columns;
            this.KeysColumn = keysColumn;
            this.ValuesColumn = valuesColumn;
            this.CheckedColumn = checkedColumn;
            this.BindingValue = bindingValue;
            this.BindingVariable = bindingVariable;
            this.BindingValuesColumn = bindingValuesColumn;
        }

        public StiDialogInfo(StiDateTimeType type, string mask, bool allowUserValues, string keysColumn, string valuesColumn,
            bool bindingValue, StiVariable bindingVariable, string bindingValuesColumn,
            StiVariableSortDirection sortDirection, StiVariableSortField sortField)
        {
            this.DateTimeType = type;
            this.Mask = mask;
            this.AllowUserValues = allowUserValues;
            this.ItemsInitializationType = StiItemsInitializationType.Columns;
            this.KeysColumn = keysColumn;
            this.ValuesColumn = valuesColumn;
            this.BindingValue = bindingValue;
            this.BindingVariable = bindingVariable;
            this.BindingValuesColumn = bindingValuesColumn;
            this.SortDirection = sortDirection;
            this.SortField = sortField;            
        }

        public StiDialogInfo(StiDateTimeType type, string mask, bool allowUserValues, string keysColumn, string valuesColumn, string checkedColumn,
            bool bindingValue, StiVariable bindingVariable, string bindingValuesColumn,
            StiVariableSortDirection sortDirection, StiVariableSortField sortField)
        {
            this.DateTimeType = type;
            this.Mask = mask;
            this.AllowUserValues = allowUserValues;
            this.ItemsInitializationType = StiItemsInitializationType.Columns;
            this.KeysColumn = keysColumn;
            this.ValuesColumn = valuesColumn;
            this.CheckedColumn = checkedColumn;
            this.BindingValue = bindingValue;
            this.BindingVariable = bindingVariable;
            this.BindingValuesColumn = bindingValuesColumn;
            this.SortDirection = sortDirection;
            this.SortField = sortField;
        }
    }
    #endregion

    #region StiDialogInfoItem
    public abstract class StiDialogInfoItem : 
        IStiPropertyGridObject, 
        ICloneable
    {
        #region ICloneable
        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion

        #region IStiComponentId Members
        [Browsable(false)]
        public virtual StiComponentId ComponentId => StiComponentId.StiDialogInfoItem;

        [Browsable(false)]
        public string PropName => string.Empty;

        public virtual StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level) => null;

        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid) => null;
        #endregion

        #region Properties
        /// <summary>
        /// Internal use only.
        /// </summary>
        [Browsable(false)]
        public object KeyObject { get; set; }

        /// <summary>
        /// Internal use only.
        /// </summary>
        [Browsable(false)]
        public object KeyObjectTo { get; set; }

        [Browsable(false)]
        public List<object> ValueBinding { get; set; } = new List<object>();

        /// <summary>
        /// Gets or sets Value which will be displayed instead of the Key value in GUI.
        /// </summary>        
        [Browsable(false)]
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value which indicates a state of the value in GUI - checked or not.
        /// </summary>        
        [Browsable(false)]
        public bool Checked { get; set; } = true;

        /// <summary>
        /// This property is used only for customer viewing - for a better understanding of the names of properties.
        /// </summary>
        [StiOrder(300)]
        [Description("Gets or sets Value which will be displayed instead of the Key value in GUI.")]
        public string Label
        {
            get
            {
                return Value;
            }
            set
            {
                Value = value;
            }
        }
        #endregion

        #region Methods
        public string ToString(StiDateTimeType dateTimeType)
        {
            if (this is StiStringRangeDialogInfoItem ||
                this is StiGuidRangeDialogInfoItem ||
                this is StiCharRangeDialogInfoItem ||
                this is StiTimeSpanRangeDialogInfoItem ||
                this is StiDoubleRangeDialogInfoItem ||
                this is StiDecimalRangeDialogInfoItem ||
                this is StiLongRangeDialogInfoItem ||
                this is StiExpressionRangeDialogInfoItem)
            {
                var str1 = KeyObject == null ? StiLocalization.Get("Report", "NotAssigned") : KeyObject.ToString();
                var str2 = KeyObjectTo == null ? StiLocalization.Get("Report", "NotAssigned") : KeyObjectTo.ToString();

                var strKey = string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2) ? string.Empty : string.Format("{0}-{1}", str1, str2);

                return (this is StiExpressionRangeDialogInfoItem)
                    ? string.Format("{{{0}}}", string.IsNullOrEmpty(Value) ? strKey : Value)
                    : string.IsNullOrEmpty(Value) ? strKey : Value;
            }
            else if (this is StiDateTimeRangeDialogInfoItem)
            {
                var range = (StiDateTimeRangeDialogInfoItem)this;

                string str1 = null;
                string str2 = null;

                if (dateTimeType == StiDateTimeType.DateAndTime)
                {
                    str1 = range.From.ToString();
                    str2 = range.To.ToString();
                }
                else if (dateTimeType == StiDateTimeType.Date)
                {
                    str1 = range.From.ToShortDateString();
                    str2 = range.To.ToShortDateString();
                }
                else if (dateTimeType == StiDateTimeType.Time)
                {
                    str1 = range.From.ToShortTimeString();
                    str2 = range.To.ToShortTimeString();
                }

                string strKey = string.Format("{0}-{1}", str1, str2);

                return string.IsNullOrEmpty(Value) ? strKey : Value;
            }
            else if (this is StiImageDialogInfoItem)
            {
                var item = this as StiImageDialogInfoItem;
                if (item.Key == null)
                    return StiLocalization.Get("Report", "NotAssigned");
                else
                    return StiLocalization.Get("Components", "StiImage");
            }
            else if (this is StiDateTimeDialogInfoItem)
            {
                var item = (StiDateTimeDialogInfoItem)this;
                string str = null;

                if (dateTimeType == StiDateTimeType.DateAndTime)
                    str = item.Key.ToString();

                else if (dateTimeType == StiDateTimeType.Date)
                    str = item.Key.ToShortDateString();

                else if (dateTimeType == StiDateTimeType.Time)
                    str = item.Key.ToShortTimeString();

                return string.IsNullOrEmpty(Value) ? str : Value;
            }
            else
            {
                string str = KeyObject == null ? StiLocalization.Get("Report", "NotAssigned") : KeyObject.ToString();

                if (this is StiExpressionDialogInfoItem)
                    return string.Format("{{{0}}}", string.IsNullOrEmpty(Value) ? str : Value);
                else
                    return string.IsNullOrEmpty(Value) ? str : Value;
            }
        }
        #endregion
    }
    #endregion

    #region StiRangeDialogInfoItem
    public class StiRangeDialogInfoItem : StiDialogInfoItem
    {
    }
    #endregion

    #region StiStringDialogInfoItem
    public class StiStringDialogInfoItem : StiDialogInfoItem
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiStringDialogInfoItem;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Misc,
                    new StiPropertyObject[]
                    {
                        propHelper.StringDialogInfoKey(),
                        propHelper.DialogInfoValue()
                    }
                }
            };
        }
        #endregion


        /// <summary>
        /// Gets or sets Key which will be used in variables when user select associated with it Value in GUI.
        /// </summary>
        [StiOrder(100)]
        [Description("Gets or sets Key which will be used in variables when user select associated with it Value in GUI.")]
        public string Key
        {
            get
            {
                return KeyObject as string;
            }
            set
            {
                KeyObject = value;
            }
        }

        public StiStringDialogInfoItem()
        {
            this.KeyObject = string.Empty;
        }
    }
    #endregion

    #region StiGuidDialogInfoItem
    public class StiGuidDialogInfoItem : StiDialogInfoItem
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiGuidDialogInfoItem;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Misc,
                    new StiPropertyObject[]
                    {
                        propHelper.GuidDialogInfoKey(),
                        propHelper.DialogInfoValue()
                    }
                }
            };
        }
        #endregion

        [StiOrder(100)]
        public Guid Key
        {
            get
            {
                return (Guid)KeyObject;
            }
            set
            {
                KeyObject = value;
            }
        }

        public StiGuidDialogInfoItem()
        {
            this.KeyObject = Guid.NewGuid();
        }
    }
    #endregion

    #region StiCharDialogInfoItem
    public class StiCharDialogInfoItem : StiDialogInfoItem
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiCharDialogInfoItem;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Misc,
                    new StiPropertyObject[]
                    {
                        propHelper.CharDialogInfoKey(),
                        propHelper.DialogInfoValue()
                    }
                }
            };
        }
        #endregion

        [StiOrder(100)]
        public char Key
        {
            get
            {
                return (char)KeyObject;
            }
            set
            {
                KeyObject = value;
            }
        }

        public StiCharDialogInfoItem()
        {
            this.KeyObject = ' ';
        }
    }
    #endregion

    #region StiBoolDialogInfoItem
    public class StiBoolDialogInfoItem : StiDialogInfoItem
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiBoolDialogInfoItem;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Misc,
                    new StiPropertyObject[]
                    {
                        propHelper.BoolDialogInfoKey(),
                        propHelper.DialogInfoValue()
                    }
                }
            };
        }
        #endregion

        [StiOrder(100)]
        public bool Key
        {
            get
            {
                return (bool)KeyObject;
            }
            set
            {
                KeyObject = value;
            }
        }

        public StiBoolDialogInfoItem()
        {
            this.KeyObject = false;
        }
    }
    #endregion

    #region StiImageDialogInfoItem
    public class StiImageDialogInfoItem : StiDialogInfoItem
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiImageDialogInfoItem;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Misc,
                    new StiPropertyObject[]
                    {
                        propHelper.ImageDialogInfoKey(),
                        propHelper.DialogInfoValue()
                    }
                }
            };
        }
        #endregion

        [StiOrder(100)]
        public Image Key
        {
            get
            {
                return (Image)KeyObject;
            }
            set
            {
                KeyObject = value;
            }
        }

        public StiImageDialogInfoItem()
        {
            this.KeyObject = null;
        }
    }
    #endregion

    #region StiDateTimeDialogInfoItem
    public class StiDateTimeDialogInfoItem : StiDialogInfoItem
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiDateTimeDialogInfoItem;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Misc,
                    new StiPropertyObject[]
                    {
                        propHelper.DateDialogInfoKey(),
                        propHelper.DialogInfoValue()
                    }
                }
            };
        }
        #endregion

        /// <summary>
        /// Gets or sets value which will be used as key of item in GUI.
        /// </summary>
        [StiOrder(100)]
        [Description("Gets or sets value which will be used as key of item in GUI.")]
        public DateTime Key
        {
            get
            {
                return (DateTime)KeyObject;
            }
            set
            {
                KeyObject = value;
            }
        }

        public StiDateTimeDialogInfoItem()
        {
            this.KeyObject = DateTime.Now;
        }
    }
    #endregion

    #region StiTimeSpanDialogInfoItem
    public class StiTimeSpanDialogInfoItem : StiDialogInfoItem
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiTimeSpanDialogInfoItem;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Misc,
                    new[]
                    {
                        propHelper.TimeDialogInfoKey(),
                        propHelper.DialogInfoValue()
                    }
                }
            };
        }
        #endregion

        /// <summary>
        /// Gets or sets value which will be used as key of item in GUI.
        /// </summary>
        [StiOrder(100)]
        [Description("Gets or sets value which will be used as key of item in GUI.")]
        public TimeSpan Key
        {
            get
            {
                return (TimeSpan)KeyObject;
            }
            set
            {
                KeyObject = value;
            }
        }

        public StiTimeSpanDialogInfoItem()
        {
            this.KeyObject = TimeSpan.Zero;
        }
    }
    #endregion

    #region StiDoubleDialogInfoItem
    public class StiDoubleDialogInfoItem : StiDialogInfoItem
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiDoubleDialogInfoItem;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Misc,
                    new[]
                    {
                        propHelper.DoubleDialogInfoKey(),
                        propHelper.DialogInfoValue()
                    }
                }
            };
        }
        #endregion

        /// <summary>
        /// Gets or sets value which will be used as key of item in GUI.
        /// </summary>
        [StiOrder(100)]
        [DefaultValue(0d)]
        [Description("Gets or sets value which will be used as key of item in GUI.")]
        public double Key
        {
            get
            {
                return (double)KeyObject;
            }
            set
            {
                KeyObject = value;
            }
        }

        public StiDoubleDialogInfoItem()
        {
            this.KeyObject = 0d;
        }
    }
    #endregion

    #region StiDecimalDialogInfoItem
    public class StiDecimalDialogInfoItem : StiDialogInfoItem
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiDecimalDialogInfoItem;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Misc,
                    new[]
                    {
                        propHelper.DecimalDialogInfoKey(),
                        propHelper.DialogInfoValue()
                    }
                }
            };
        }
        #endregion

        /// <summary>
        /// Gets or sets value which will be used as key of item in GUI.
        /// </summary>
        [StiOrder(100)]
        [Description("Gets or sets value which will be used as key of item in GUI.")]
        public decimal Key
        {
            get
            {
                return (decimal)KeyObject;
            }
            set
            {
                KeyObject = value;
            }
        }

        public StiDecimalDialogInfoItem()
        {
            this.KeyObject = 0m;
        }
    }
    #endregion

    #region StiLongDialogInfoItem
    public class StiLongDialogInfoItem : StiDialogInfoItem
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiLongDialogInfoItem;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Misc,
                    new[]
                    {
                        propHelper.LongDialogInfoKey(),
                        propHelper.DialogInfoValue()
                    }
                }
            };
        }
        #endregion

        /// <summary>
        /// Gets or sets value which will be used as key of item in GUI.
        /// </summary>
        [StiOrder(100)]
        [DefaultValue(0)]
        [Description("Gets or sets value which will be used as key of item in GUI.")]
        public long Key
        {
            get
            {
                return (long)KeyObject;
            }
            set
            {
                KeyObject = value;
            }
        }

        public StiLongDialogInfoItem()
        {
            this.KeyObject = (long)0;
        }
    }
    #endregion

    #region StiExpressionDialogInfoItem
    public class StiExpressionDialogInfoItem : StiDialogInfoItem
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiExpressionDialogInfoItem;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Misc,
                    new[]
                    {
                        propHelper.StringDialogInfoKey(),
                        propHelper.DialogInfoValue()
                    }
                }
            };
        }
        #endregion

        /// <summary>
        /// Gets or sets expression which will be used as key of item in GUI."
        /// </summary>
        [StiOrder(100)]
        [Description("Gets or sets expression which will be used as key of item in GUI.")]
        public string Key
        {
            get
            {
                return (string)KeyObject;
            }
            set
            {
                KeyObject = value;
            }
        }

        public StiExpressionDialogInfoItem()
        {
            this.KeyObject = string.Empty;
        }
    }
    #endregion

    #region StiStringRangeDialogInfoItem
    public class StiStringRangeDialogInfoItem : StiRangeDialogInfoItem
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiStringRangeDialogInfoItem;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Misc,
                    new[]
                    {
                        propHelper.RangeStringFromDialogInfo(),
                        propHelper.RangeStringToDialogInfo(),
                        propHelper.DialogInfoValue()
                    }
                }
            };
        }
        #endregion

        /// <summary>
        /// Gets or sets value which will be used as From key of range item in GUI.
        /// </summary>
        [StiOrder(100)]
        [Description("Gets or sets value which will be used as From key of range item in GUI.")]
        public string From
        {
            get
            {
                return (string)KeyObject;
            }
            set
            {
                KeyObject = value;
            }
        }


        /// <summary>
        /// Gets or sets value which will be used as To key of range item in GUI.
        /// </summary>
        [StiOrder(200)]
        [Description("Gets or sets value which will be used as To key of range item in GUI.")]
        public string To
        {
            get
            {
                return (string)KeyObjectTo;
            }
            set
            {
                KeyObjectTo = value;
            }
        }

        public StiStringRangeDialogInfoItem()
        {
            this.KeyObject = string.Empty;
            this.KeyObjectTo = string.Empty;
        }
    }
    #endregion

    #region StiGuidRangeDialogInfoItem
    public class StiGuidRangeDialogInfoItem : StiRangeDialogInfoItem
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiGuidRangeDialogInfoItem;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Misc,
                    new[]
                    {
                        propHelper.RangeGuidFromDialogInfo(),
                        propHelper.RangeGuidToDialogInfo(),
                        propHelper.DialogInfoValue()
                    }
                }
            };
        }
        #endregion

        /// <summary>
        /// Gets or sets value which will be used as From key of range item in GUI.
        /// </summary>
        [StiOrder(100)]
        [Description("Gets or sets value which will be used as From key of range item in GUI.")]
        public Guid From
        {
            get
            {
                return (Guid)KeyObject;
            }
            set
            {
                KeyObject = value;
            }
        }

        /// <summary>
        /// Gets or sets value which will be used as To key of range item in GUI.
        /// </summary>
        [StiOrder(200)]
        [Description("Gets or sets value which will be used as To key of range item in GUI.")]
        public Guid To
        {
            get
            {
                return (Guid)KeyObjectTo;
            }
            set
            {
                KeyObjectTo = value;
            }
        }

        public StiGuidRangeDialogInfoItem()
        {
            this.KeyObject = Guid.NewGuid();
            this.KeyObjectTo = Guid.NewGuid();
        }
    }
    #endregion

    #region StiByteArrayRangeDialogInfoItem
    public class StiByteArrayRangeDialogInfoItem : StiRangeDialogInfoItem
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiByteArrayRangeDialogInfoItem;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Misc,
                    new[]
                    {
                        propHelper.RangeGuidFromDialogInfo(),
                        propHelper.RangeGuidToDialogInfo(),
                        propHelper.DialogInfoValue()
                    }
                }
            };
        }
        #endregion

        /// <summary>
        /// Gets or sets value which will be used as From key of range item in GUI.
        /// </summary>
        [StiOrder(100)]
        [Description("Gets or sets value which will be used as From key of range item in GUI.")]
        [Browsable(false)]
        public byte[] From
        {
            get
            {
                return (byte[])KeyObject;
            }
            set
            {
                KeyObject = value;
            }
        }

        /// <summary>
        /// Gets or sets value which will be used as To key of range item in GUI.
        /// </summary>
        [StiOrder(200)]
        [Description("Gets or sets value which will be used as To key of range item in GUI.")]
        [Browsable(false)]
        public byte[] To
        {
            get
            {
                return (byte[])KeyObjectTo;
            }
            set
            {
                KeyObjectTo = value;
            }
        }
    }
    #endregion

    #region StiCharRangeDialogInfoItem
    public class StiCharRangeDialogInfoItem : StiRangeDialogInfoItem
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiCharRangeDialogInfoItem;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Misc,
                    new[]
                    {
                        propHelper.RangeCharFromDialogInfo(),
                        propHelper.RangeCharToDialogInfo(),
                        propHelper.DialogInfoValue()
                    }
                }
            };
        }
        #endregion

        /// <summary>
        /// Gets or sets value which will be used as From key of range item in GUI.
        /// </summary>
        [StiOrder(100)]
        [Description("Gets or sets value which will be used as From key of range item in GUI.")]
        public char From
        {
            get
            {
                return (char)KeyObject;
            }
            set
            {
                KeyObject = value;
            }
        }

        /// <summary>
        /// Gets or sets value which will be used as To key of range item in GUI.
        /// </summary>
        [StiOrder(200)]
        [Description("Gets or sets value which will be used as To key of range item in GUI.")]
        public char To
        {
            get
            {
                return (char)KeyObjectTo;
            }
            set
            {
                KeyObjectTo = value;
            }
        }

        public StiCharRangeDialogInfoItem()
        {
            this.KeyObject = 'A';
            this.KeyObjectTo = 'Z';
        }
    }
    #endregion

    #region StiDateTimeRangeDialogInfoItem
    public class StiDateTimeRangeDialogInfoItem : StiRangeDialogInfoItem
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiDateTimeRangeDialogInfoItem;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Misc,
                    new[]
                    {
                        propHelper.RangeDateFromDialogInfo(),
                        propHelper.RangeDateToDialogInfo(),
                        propHelper.DialogInfoValue()
                    }
                }
            };
        }
        #endregion

        /// <summary>
        /// Gets or sets value which will be used as From key of range item in GUI.
        /// </summary>
        [StiOrder(100)]
        [Description("Gets or sets value which will be used as From key of range item in GUI.")]
        public DateTime From
        {
            get
            {
                return (DateTime)KeyObject;
            }
            set
            {
                KeyObject = value;
            }
        }

        /// <summary>
        /// Gets or sets value which will be used as To key of range item in GUI.
        /// </summary>
        [StiOrder(200)]
        [Description("Gets or sets value which will be used as To key of range item in GUI.")]
        public DateTime To
        {
            get
            {
                return (DateTime)KeyObjectTo;
            }
            set
            {
                KeyObjectTo = value;
            }
        }

        public StiDateTimeRangeDialogInfoItem()
        {
            this.KeyObject = DateTime.Now;
            this.KeyObjectTo = DateTime.Now;
        }
    }
    #endregion

    #region StiTimeSpanRangeDialogInfoItem
    public class StiTimeSpanRangeDialogInfoItem : StiRangeDialogInfoItem
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiTimeSpanRangeDialogInfoItem;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Misc,
                    new[]
                    {
                        propHelper.RangeTimeFromDialogInfo(),
                        propHelper.RangeTimeToDialogInfo(),
                        propHelper.DialogInfoValue()
                    }
                }
            };
        }
        #endregion

        /// <summary>
        /// Gets or sets value which will be used as From key of range item in GUI.
        /// </summary>
        [StiOrder(100)]
        [Description("Gets or sets value which will be used as From key of range item in GUI.")]
        public TimeSpan From
        {
            get
            {
                return (TimeSpan)KeyObject;
            }
            set
            {
                KeyObject = value;
            }
        }

        /// <summary>
        /// Gets or sets value which will be used as To key of range item in GUI.
        /// </summary>
        [StiOrder(200)]
        [Description("Gets or sets value which will be used as To key of range item in GUI.")]
        public TimeSpan? To
        {
            get
            {
                return (TimeSpan)KeyObjectTo;
            }
            set
            {
                KeyObjectTo = value;
            }
        }

        public StiTimeSpanRangeDialogInfoItem()
        {
            this.KeyObject = TimeSpan.Zero;
            this.KeyObjectTo = TimeSpan.Zero;
        }
    }
    #endregion

    #region StiDoubleRangeDialogInfoItem
    public class StiDoubleRangeDialogInfoItem : StiRangeDialogInfoItem
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiDoubleRangeDialogInfoItem;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Misc,
                    new[]
                    {
                        propHelper.RangeDoubleFromDialogInfo(),
                        propHelper.RangeDoubleToDialogInfo(),
                        propHelper.DialogInfoValue()
                    }
                }
            };
        }
        #endregion

        /// <summary>
        /// Gets or sets value which will be used as From key of range item in GUI.
        /// </summary>
        [StiOrder(100)]
        [DefaultValue(0d)]
        [Description("Gets or sets value which will be used as From key of range item in GUI.")]
        public double From
        {
            get
            {
                return (double)KeyObject;
            }
            set
            {
                KeyObject = value;
            }
        }

        /// <summary>
        /// Gets or sets value which will be used as To key of range item in GUI.
        /// </summary>
        [StiOrder(200)]
        [DefaultValue(0d)]
        [Description("Gets or sets value which will be used as To key of range item in GUI.")]
        public double To
        {
            get
            {
                return (double)KeyObjectTo;
            }
            set
            {
                KeyObjectTo = value;
            }
        }

        public StiDoubleRangeDialogInfoItem()
        {
            this.KeyObject = 0d;
            this.KeyObjectTo = 0d;
        }
    }
    #endregion

    #region StiDecimalRangeDialogInfoItem
    public class StiDecimalRangeDialogInfoItem : StiRangeDialogInfoItem
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiDecimalRangeDialogInfoItem;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Misc,
                    new[]
                    {
                        propHelper.RangeDecimalFromDialogInfo(),
                        propHelper.RangeDecimalToDialogInfo(),
                        propHelper.DialogInfoValue()
                    }
                }
            };
        }
        #endregion

        /// <summary>
        /// Gets or sets value which will be used as From key of range item in GUI.
        /// </summary>
        [StiOrder(100)]
        [Description("Gets or sets value which will be used as From key of range item in GUI.")]
        public decimal From
        {
            get
            {
                return (decimal)KeyObject;
            }
            set
            {
                KeyObject = value;
            }
        }

        /// <summary>
        /// Gets or sets value which will be used as To key of range item in GUI.
        /// </summary>
        [StiOrder(200)]
        [Description("Gets or sets value which will be used as To key of range item in GUI.")]
        public decimal To
        {
            get
            {
                return (decimal)KeyObjectTo;
            }
            set
            {
                KeyObjectTo = value;
            }
        }

        public StiDecimalRangeDialogInfoItem()
        {
            this.KeyObject = 0m;
            this.KeyObjectTo = 0m;
        }
    }
    #endregion

    #region StiLongRangeDialogInfoItem
    public class StiLongRangeDialogInfoItem : StiRangeDialogInfoItem
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiLongRangeDialogInfoItem;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Misc,
                    new[]
                    {
                        propHelper.RangeLongFromDialogInfo(),
                        propHelper.RangeLongToDialogInfo(),
                        propHelper.DialogInfoValue()
                    }
                }
            };
        }
        #endregion

        /// <summary>
        /// Gets or sets value which will be used as From key of range item in GUI.
        /// </summary>
        [StiOrder(100)]
        [DefaultValue(0)]
        [Description("Gets or sets value which will be used as From key of range item in GUI.")]
        public long From
        {
            get
            {
                return (long)KeyObject;
            }
            set
            {
                KeyObject = value;
            }
        }

        /// <summary>
        /// Gets or sets value which will be used as To key of range item in GUI.
        /// </summary>
        [StiOrder(200)]
        [DefaultValue(0)]
        [Description("Gets or sets value which will be used as To key of range item in GUI.")]
        public long To
        {
            get
            {
                return (long)KeyObjectTo;
            }
            set
            {
                KeyObjectTo = value;
            }
        }

        public StiLongRangeDialogInfoItem()
        {
            this.KeyObject = (long)0;
            this.KeyObjectTo = (long)0;
        }
    }
    #endregion

    #region StiExpressionRangeDialogInfoItem
    public class StiExpressionRangeDialogInfoItem : StiRangeDialogInfoItem
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiExpressionRangeDialogInfoItem;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Misc,
                    new[]
                    {
                        propHelper.RangeExpressionFromDialogInfo(),
                        propHelper.RangeExpressionToDialogInfo(),
                        propHelper.DialogInfoValue()
                    }
                }
            };
        }
        #endregion

        /// <summary>
        /// Gets or sets expression which will be used as From key of range item in GUI.
        /// </summary>
        [StiOrder(100)]
        [Description("Gets or sets expression which will be used as From key of range item in GUI.")]
        public string From
        {
            get
            {
                return (string)KeyObject;
            }
            set
            {
                KeyObject = value;
            }
        }

        /// <summary>
        /// Gets or sets expression which will be used as To key of range item in GUI.
        /// </summary>
        [StiOrder(200)]
        [Description("Gets or sets expression which will be used as To key of range item in GUI.")]
        public string To
        {
            get
            {
                return (string)KeyObjectTo;
            }
            set
            {
                KeyObjectTo = value;
            }
        }

        public StiExpressionRangeDialogInfoItem()
        {
            this.KeyObject = string.Empty;
            this.KeyObjectTo = string.Empty;
        }
    }
    #endregion
}