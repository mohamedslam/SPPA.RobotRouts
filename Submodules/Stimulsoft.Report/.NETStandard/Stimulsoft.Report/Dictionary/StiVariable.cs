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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Design;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using Stimulsoft.Base.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// This class describes variable.
    /// </summary>
    [TypeConverter(typeof(Design.StiVariableConverter))]
    [StiSerializable]
    public class StiVariable :
        StiExpression,
        IStiName,
        IStiAlias,
        IStiInherited,
        IStiPropertyGridObject,
        IStiAppVariable,
        IStiAppAlias
    {
        #region enum Order
        public enum Order
        {
            Name = 100,
            Alias = 200,
            Category = 300
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            var variable = base.Clone() as StiVariable;
            
            variable.DialogInfo = this.DialogInfo?.Clone() as StiDialogInfo;
            
            return variable;
        }
        #endregion

        #region IStiJsonReportObject.override
        private string ConvertTypeToJsonString(Type type)
        {
            var typeName = type.FullName;

#if STIDRAWING
            typeName = typeName.Replace("Stimulsoft.Drawing", "System.Drawing");
#endif

            if (typeName.StartsWith("System.Nullable`1"))
            {
                var index = typeName.IndexOf(",");
                typeName = typeName.Substring(0, index).Replace("[[", "[") + "]";
            }

            return typeName;
        }

        private Type ConvertJsonStringToType(string text)
        {
            if (text.StartsWith("System.Nullable`1") && !text.Contains(", mscorlib,"))
            {
                text = text.Substring(0, text.Length - 1) + ", mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]";
                text = text.Replace("[", "[[");
            }

            return StiTypeFinder.GetType(text);
        }

        public JObject SaveToJsonObjectEx()
        {
            var jObject = new JObject();

            // StiExpression
            jObject.AddPropertyStringNullOrEmpty("Value", Value);

            // StiVariable
            jObject.AddPropertyStringNullOrEmpty("Name", Name);
            if (DialogInfo != null) jObject.AddPropertyJObject("DialogInfo", DialogInfo.SaveToJsonObject());
            jObject.AddPropertyStringNullOrEmpty("Alias", Alias);
            jObject.AddPropertyStringNullOrEmpty("Type", ConvertTypeToJsonString(Type));
            jObject.AddPropertyBool("ReadOnly", ReadOnly);
            jObject.AddPropertyBool("RequestFromUser", RequestFromUser);
            jObject.AddPropertyStringNullOrEmpty("Category", Category);
            jObject.AddPropertyStringNullOrEmpty("Description", Description);
            jObject.AddPropertyEnum("InitBy", InitBy, StiVariableInitBy.Value);
            jObject.AddPropertyStringNullOrEmpty("Key", Key);
            jObject.AddPropertyBool("AllowUseAsSqlParameter", AllowUseAsSqlParameter);
            jObject.AddPropertyEnum("SelectionMode", Selection, StiSelectionMode.FromVariable);

            return jObject;
        }

        public void LoadFromJsonObjectEx(JObject jObject, StiReport report)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Name":
                        Name = property.DeserializeString();
                        break;

                    case "DialogInfo":
                        DialogInfo.LoadFromJsonObject((JObject)property.Value, report);
                        break;

                    case "Alias":
                        Alias = property.DeserializeString();
                        break;

                    case "Type":
                        Type = ConvertJsonStringToType(property.DeserializeString());
                        break;

                    case "ReadOnly":
                        readOnly = property.DeserializeBool();
                        break;

                    case "RequestFromUser":
                        RequestFromUser = property.DeserializeBool();
                        break;

                    case "Category":
                        Category = property.DeserializeString();
                        break;

                    case "Description":
                        Description = property.DeserializeString();
                        break;

                    case "InitBy":
                        InitBy = property.DeserializeEnum<StiVariableInitBy>();
                        break;

                    case "Key":
                        Key = property.DeserializeString();
                        break;

                    case "AllowUseAsSqlParameter":
                        AllowUseAsSqlParameter = property.DeserializeBool();
                        break;

                    case "SelectionMode":
                        Selection = property.DeserializeEnum<StiSelectionMode>();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public StiComponentId ComponentId => StiComponentId.StiVariable;

        [Browsable(false)]
        public string PropName => Name;

        public StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // DataCategory
            var list = new[]
            {
                propHelper.Name(),
                propHelper.Alias(),
                propHelper.Category(),
                propHelper.Description()
            };
            objHelper.Add(StiPropertyCategories.Data, list);

            return objHelper;
        }

        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return null;
        }
        #endregion

        #region IStiAppVariable
        object IStiAppVariable.GetValue()
        {
            return ValueObject;
        }
        #endregion

        #region IStiAppDataCell
        string IStiAppDataCell.GetName()
        {
            return Name;
        }
        #endregion

        #region IStiAppCell
        string IStiAppCell.GetKey()
        {
            Key = StiKeyHelper.GetOrGeneratedKey(Key);

            return Key;
        }

        void IStiAppCell.SetKey(string key)
        {
            Key = key;
        }
        #endregion

        #region IStiAppAlias
        string IStiAppAlias.GetAlias()
        {
            return Alias;
        }
        #endregion

        #region IStiInherited
        [Browsable(false)]
        [DefaultValue(false)]
        public bool Inherited { get; set; }
        #endregion

        #region IStiName
        /// <summary>
        /// Gets or sets a name of the variable.
		/// </summary>
		[StiCategory("Data")]
        [StiSerializable]
        [ParenthesizePropertyName(true)]
        [StiOrder((int)Order.Name)]
        [Description("Gets or sets a name of the variable.")]
        public string Name { get; set; }
        #endregion

        #region IStiAlias
        /// <summary>
        /// Gets or sets an alias of the variable.
		/// </summary>
		[StiSerializable]
        [StiCategory("Data")]
        [ParenthesizePropertyName(true)]
        [StiOrder((int)Order.Alias)]
        [Description("Gets or sets an alias of the variable.")]
        public string Alias { get; set; }
        #endregion

        #region Properties
        [Browsable(false)]
        public override bool ApplyFormat => false;

        /// <summary>
        /// Gets or sets a dialog info of the variable.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [Browsable(false)]
        [Description("Gets or sets a dialog info of the variable.")]
        public StiDialogInfo DialogInfo { get; set; } = new StiDialogInfo();

        /// <summary>
        /// Gets or sets a type of the variable.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [TypeConverter(typeof(StiTypeConverter))]
        [Editor("Stimulsoft.Base.Design.StiTypeEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Browsable(false)]
        [Description("Gets or sets a type of the variable.")]
        public Type Type { get; set; }

        private bool readOnly;
        /// <summary>
        /// Gets or sets a value which indicates that this variable is read only.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Browsable(false)]
        [Description("Gets or sets a value which indicates that this variable is read only.")]
        public bool ReadOnly
        {
            get
            {
                return readOnly;
            }
            set
            {
                readOnly = value;
                if (value && !IsCategory)
                    RequestFromUser = false;
            }
        }

        /// <summary>
        /// Gets or sets a request from user value which indicates that this variable will be requested from user.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Browsable(false)]
        [Description("Gets or sets a request from user value which indicates that this variable will be requested from user.")]
        public bool RequestFromUser { get; set; }

        /// <summary>
        /// Gets or sets a value which allows using this variable as a SQL parameter in the query.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Browsable(false)]
        [Description("Gets or sets a value which allows using this variable as a SQL parameter in the query.")]
        public bool AllowUseAsSqlParameter { get; set; }

        /// <summary>
		/// Gets or sets a category of the variable.
		/// </summary>
		[StiSerializable]
        [StiCategory("Data")]
        [Description("Gets or sets a category of the variable.")]
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets a description of the variable.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [Description("Gets or sets a description of the variable.")]
        public string Description { get; set; }

        [Browsable(false)]
        public bool IsCategory => string.IsNullOrEmpty(Name);

        [Browsable(false)]
        public object ValueObject
        {
            get
            {
                return InitBy == StiVariableInitBy.Expression
                    ? Value
                    : GetValue(Value, Type);
            }
            set
            {
                if (InitBy == StiVariableInitBy.Expression)
                    Value = value as string;

                if (value == null)
                    Value = string.Empty;
                else
                    SetValue(value);
            }
        }

        /// <summary>
        /// Gets or sets init by expression for range values. This is expression for From part of range. Property can be used only for Range types!
        /// </summary>
        [Browsable(false)]
        public string InitByExpressionFrom
        {
            get
            {
                if (!StiTypeFinder.FindType(Type, typeof(Range)))
                    return null;

                var values = GetRangeValues();
                return values == null ? string.Empty : values[0];
            }
            set
            {
                if (!StiTypeFinder.FindType(Type, typeof(Range)))
                    return;

                var values = GetRangeValues();
                var rangeTo = string.Empty;
                if (values != null) rangeTo = values[1];

                Value = $"{value}<<|>>{rangeTo}";
            }
        }

        /// <summary>
        /// Gets or sets init by expression for range values. This is expression for To part of range. Property can be used only for Range types!
        /// </summary>
        [Browsable(false)]
        public string InitByExpressionTo
        {
            get
            {
                if (!StiTypeFinder.FindType(Type, typeof(Range)))
                    return null;

                var values = GetRangeValues();
                return values == null ? string.Empty : values[1];
            }
            set
            {
                if (!StiTypeFinder.FindType(Type, typeof(Range)))
                    return;

                var values = GetRangeValues();
                var rangeFrom = string.Empty;
                if (values != null)
                    rangeFrom = values[0];

                Value = $"{rangeFrom}<<|>>{value}";
            }
        }

        [Browsable(false)]
        public override string Value
        {
            get
            {
                if (InitBy == StiVariableInitBy.Expression)
                    return base.Value;

                if (!string.IsNullOrEmpty(base.Value))
                {
                    #region DateTime
                    if (Type == typeof(DateTime))
                    {
                        var date = GetDateTimeFromValue(base.Value);
                        var currentCulture = Thread.CurrentThread.CurrentCulture;

                        try
                        {
                            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                            return date.ToString();
                        }
                        finally
                        {
                            Thread.CurrentThread.CurrentCulture = currentCulture;
                        }
                    }
                    #endregion

                    #region DateTime?
                    if (Type == typeof(DateTime?))
                    {
                        var date = GetDateTimeFromValue(base.Value);
                        var currentCulture = Thread.CurrentThread.CurrentCulture;
                        try
                        {
                            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                            return date.ToString();
                        }
                        finally
                        {
                            Thread.CurrentThread.CurrentCulture = currentCulture;
                        }
                    }
                    #endregion

                    #region DateTimeOffset
                    if (Type == typeof(DateTimeOffset))
                    {
                        var date = GetDateTimeOffsetFromValue(base.Value);
                        var currentCulture = Thread.CurrentThread.CurrentCulture;

                        try
                        {
                            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                            return date.ToString();
                        }
                        finally
                        {
                            Thread.CurrentThread.CurrentCulture = currentCulture;
                        }
                    }
                    #endregion

                    #region DateTime?
                    if (Type == typeof(DateTimeOffset?))
                    {
                        var date = GetDateTimeOffsetFromValue(base.Value);
                        var currentCulture = Thread.CurrentThread.CurrentCulture;
                        try
                        {
                            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                            return date.ToString();
                        }
                        finally
                        {
                            Thread.CurrentThread.CurrentCulture = currentCulture;
                        }
                    }
                    #endregion
                }

                if (Type == typeof(DateTime))
                    return null;

                if (Type == typeof(DateTime?))
                    return null;

                if (Type == typeof(DateTimeOffset))
                    return null;

                if (Type == typeof(DateTimeOffset?))
                    return null;

                return base.Value;
            }
            set
            {
                if (InitBy == StiVariableInitBy.Expression)
                {
                    base.Value = value;
                }
                else
                {
                    if (value != null && value != "null" && value.Length > 0)
                    {
                        if (Type == typeof(DateTime))
                        {
                            try
                            {
                                var currentCulture = Thread.CurrentThread.CurrentCulture;
                                try
                                {
                                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                                    var date = DateTime.Parse(value);
                                    base.Value = GetValueFromDateTime(date);
                                }
                                finally
                                {
                                    Thread.CurrentThread.CurrentCulture = currentCulture;
                                }
                            }
                            catch
                            {
                                throw new ArgumentException($"Variable value '{value}' is wrong DateTime value");
                            }
                        }

                        else if (Type == typeof(DateTime?))
                        {
                            try
                            {
                                var currentCulture = Thread.CurrentThread.CurrentCulture;
                                try
                                {
                                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                                    var date = DateTime.Parse(value);
                                    base.Value = GetValueFromDateTime(date);
                                }
                                finally
                                {
                                    Thread.CurrentThread.CurrentCulture = currentCulture;
                                }

                            }
                            catch
                            {
                                throw new ArgumentException($"Variable value '{value}' is wrong DateTime value");
                            }
                        }
                        else if (Type == typeof(DateTimeOffset))
                        {
                            try
                            {
                                var currentCulture = Thread.CurrentThread.CurrentCulture;
                                try
                                {
                                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                                    var date = DateTimeOffset.Parse(value);
                                    base.Value = GetValueFromDateTimeOffset(date);
                                }
                                finally
                                {
                                    Thread.CurrentThread.CurrentCulture = currentCulture;
                                }
                            }
                            catch
                            {
                                throw new ArgumentException($"Variable value '{value}' is wrong DateTime value");
                            }
                        }

                        else if (Type == typeof(DateTimeOffset?))
                        {
                            try
                            {
                                var currentCulture = Thread.CurrentThread.CurrentCulture;
                                try
                                {
                                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                                    var date = DateTimeOffset.Parse(value);
                                    base.Value = GetValueFromDateTimeOffset(date);
                                }
                                finally
                                {
                                    Thread.CurrentThread.CurrentCulture = currentCulture;
                                }

                            }
                            catch
                            {
                                throw new ArgumentException($"Variable value '{value}' is wrong DateTime value");
                            }
                        }
                    }

                    base.Value = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value which indicates that this variable will be have defalt value based on calculation of expression which specified in Value property.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [ParenthesizePropertyName(true)]
        [DefaultValue(false)]
        [Description("Gets or sets a value which indicates that this variable will be have defalt value based on calculation of expression which specified in Value property.")]
        [Obsolete("'Function' property is obsolete. Please use 'InitBy' property instead it.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public bool Function
        {
            get
            {
                return InitBy == StiVariableInitBy.Expression;
            }
            set
            {
                InitBy = value
                    ? StiVariableInitBy.Expression
                    : StiVariableInitBy.Value;
            }
        }

        /// <summary>
        /// Gets or sets a value which indicates that this variable will be have defalt value based on calculation of expression which specified in Value property or based on default value.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [DefaultValue(StiVariableInitBy.Value)]
        [Browsable(false)]
        public StiVariableInitBy InitBy { get; set; } = StiVariableInitBy.Value;

        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [DefaultValue(StiSelectionMode.FromVariable)]
        [Browsable(false)]
        public StiSelectionMode Selection { get; set; } = StiSelectionMode.FromVariable;

        /// <summary>
        /// Gets or sets the key of the dictionary object.
        /// </summary>
        [DefaultValue(null)]
        [StiSerializable]
        [Browsable(false)]
        public string Key { get; set; }
        #endregion

        #region Methods
        private string[] GetRangeValues()
        {
            var value = Value;
            if (string.IsNullOrEmpty(value))
                return null;

            if (!value.Contains("<<|>>"))
                return null;

            var values = value.Split(new[] { "<<|>>" }, StringSplitOptions.None);
            if (values.Length != 2)
                return null;

            return values;
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        public static object GetValue(string str, Type type)
        {
            #region Default Values
            if (string.IsNullOrEmpty(str))
            {
                if (type == typeof(int) ||
                    type == typeof(uint) ||
                    type == typeof(long) ||
                    type == typeof(ulong) ||
                    type == typeof(byte) ||
                    type == typeof(sbyte) ||
                    type == typeof(short) ||
                    type == typeof(ushort))
                    return 0;

                if (type == typeof(string))
                    return string.Empty;

                if (type == typeof(bool))
                    return false;

                if (type == typeof(char))
                    return ' ';

                if (type == typeof(double))
                    return 0d;

                if (type == typeof(float))
                    return 0f;

                if (type == typeof(decimal))
                    return 0m;

                if (type == typeof(DateTime))
                    return null;

                if (type == typeof(DateTimeOffset))
                    return null;

                if (type == typeof(TimeSpan))
                    return null;

                if (StiTypeFinder.FindType(type, typeof(Range)))
                    return Activator.CreateInstance(type) as Range;

                if (StiTypeFinder.FindType(type, typeof(Image)))
                    return null;

                if (type == typeof(int?) ||
                    type == typeof(uint?) ||
                    type == typeof(long?) ||
                    type == typeof(ulong?) ||
                    type == typeof(byte?) ||
                    type == typeof(sbyte?) ||
                    type == typeof(short?) ||
                    type == typeof(ushort?))
                    return 0;

                if (type == typeof(bool?))
                    return false;

                if (type == typeof(char?))
                    return ' ';

                if (type == typeof(double?))
                    return 0d;

                if (type == typeof(float?))
                    return 0f;

                if (type == typeof(decimal?))
                    return 0m;

                if (type == typeof(DateTime?))
                    return null;

                if (type == typeof(DateTimeOffset?))
                    return null;

                if (type == typeof(TimeSpan?))
                    return null;
            }
            #endregion

            #region Parse values
            if (type == typeof(int))
                return StiValueHelper.TryToInt(str);

            if (type == typeof(uint))
                return StiValueHelper.TryToUInt(str);

            if (type == typeof(long))
                return StiValueHelper.TryToLong(str);

            if (type == typeof(ulong))
                return StiValueHelper.TryToULong(str);

            if (type == typeof(byte))
                return (byte)StiValueHelper.TryToInt(str);

            if (type == typeof(sbyte))
                return (sbyte)StiValueHelper.TryToInt(str);

            if (type == typeof(short))
                return (short)StiValueHelper.TryToInt(str);

            if (type == typeof(ushort))
                return (ushort)StiValueHelper.TryToInt(str);

            if (type == typeof(string))
                return str;

            if (type == typeof(bool))
                return str.ToLower(CultureInfo.InvariantCulture) == "true";

            if (type == typeof(char))
                return str[0];

            if (type == typeof(double))
                return StiValueHelper.TryToDouble(str);

            if (type == typeof(float))
                return StiValueHelper.TryToFloat(str);

            if (type == typeof(decimal))
                return StiValueHelper.TryToDecimal(str);

            if (type == typeof(Guid))
                return string.IsNullOrEmpty(str) ? new Guid() : new Guid(str);

            if (type == typeof(DateTime))
            {
                var currentCulture = Thread.CurrentThread.CurrentCulture;
                try
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                    return DateTime.Parse(str);
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                }
            }

            if (type == typeof(DateTimeOffset))
            {
                var currentCulture = Thread.CurrentThread.CurrentCulture;
                try
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                    return DateTimeOffset.Parse(str);
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                }
            }

            if (type == typeof(TimeSpan))
            {
                var currentCulture = Thread.CurrentThread.CurrentCulture;
                try
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                    return TimeSpan.Parse(str);
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                }
            }

            if (type == typeof(int?))
                return str.ToLowerInvariant() == "null" ? null : StiValueHelper.TryToNullableInt(str);

            if (type == typeof(uint?))
                return str.ToLowerInvariant() == "null" ? null : StiValueHelper.TryToNullableUInt(str);

            if (type == typeof(long?))
                return str.ToLowerInvariant() == "null" ? null : StiValueHelper.TryToNullableLong(str);

            if (type == typeof(ulong?))
                return str.ToLowerInvariant() == "null" ? null : StiValueHelper.TryToNullableULong(str);

            if (type == typeof(byte?))
                return str.ToLowerInvariant() == "null" ? null : (byte?)StiValueHelper.TryToNullableInt(str);

            if (type == typeof(sbyte?))
                return str.ToLowerInvariant() == "null" ? null : (sbyte?)StiValueHelper.TryToNullableInt(str);

            if (type == typeof(short?))
                return str.ToLowerInvariant() == "null" ? null : (short?)StiValueHelper.TryToNullableInt(str);

            if (type == typeof(ushort?))
                return str.ToLowerInvariant() == "null" ? null : (ushort?)StiValueHelper.TryToNullableInt(str);

            if (type == typeof(bool?))
                return str.ToLowerInvariant() == "null" ? null : (bool?)(str.ToLower(CultureInfo.InvariantCulture) == "true");

            if (type == typeof(char?))
                return str.ToLowerInvariant() == "null" ? null : (char?)str[0];

            if (type == typeof(double?))
                return str.ToLowerInvariant() == "null" ? null : StiValueHelper.TryToNullableDouble(str);

            if (type == typeof(float?))
                return str.ToLowerInvariant() == "null" ? null : StiValueHelper.TryToNullableFloat(str);

            if (type == typeof(decimal?))
                return str.ToLowerInvariant() == "null" ? null : StiValueHelper.TryToNullableDecimal(str);

            if (type == typeof(DateTime?))
            {
                if (str.ToLowerInvariant() == "null") return null;

                var currentCulture = Thread.CurrentThread.CurrentCulture;
                try
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                    return (DateTime?)DateTime.Parse(str);
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                }
            }

            if (type == typeof(DateTimeOffset?))
            {
                if (str.ToLowerInvariant() == "null") return null;

                var currentCulture = Thread.CurrentThread.CurrentCulture;
                try
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                    return (DateTimeOffset?)DateTimeOffset.Parse(str);
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                }
            }

            if (type == typeof(TimeSpan?))
            {
                if (str.ToLowerInvariant() == "null") return null;

                var currentCulture = Thread.CurrentThread.CurrentCulture;
                try
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                    return TimeSpan.Parse(str);
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                }
            }

            if (type == typeof(Metafile))
                return StiMetafileConverter.StringToMetafile(str);

            if (StiTypeFinder.FindType(type, typeof(Image)))
                return StiImageConverter.StringToImage(str);

            if (StiTypeFinder.FindType(type, typeof(Range)))
                return RangeConverter.StringToRange(str);
            #endregion

            return null;
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        private void SetValue(object value)
        {
            if (value is string)
                Value = value as string;

            else if (value is char)
                Value = value.ToString();

            else if (value is Guid)
                Value = value.ToString();

            else if (value is bool)
                Value = ((bool)value).ToString().ToLower(CultureInfo.InvariantCulture);

            else if (value is Metafile)
                Value = StiMetafileConverter.MetafileToString(value as Metafile);

            else if (value is Image)
                Value = StiImageConverter.ImageToString(value as Image);

            else if (value is byte[])
                Value = Convert.ToBase64String(value as byte[]);

            else if (value is Range)
                Value = RangeConverter.RangeToString(value as Range);

            else if (value is TimeSpan)
            {
                var currentCulture = Thread.CurrentThread.CurrentCulture;
                try
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                    Value = ((TimeSpan)value).ToString();
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                }
            }

            else if (value is DateTime)
            {
                var currentCulture = Thread.CurrentThread.CurrentCulture;
                try
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                    Value = ((DateTime)value).ToString();
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                }
            }

            else if (value is DateTimeOffset)
            {
                var currentCulture = Thread.CurrentThread.CurrentCulture;
                try
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                    Value = ((DateTimeOffset)value).ToString();
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                }
            }

            else
            {
                if (Type == typeof(int) ||
                    Type == typeof(uint) ||
                    Type == typeof(long) ||
                    Type == typeof(ulong) ||
                    Type == typeof(byte) ||
                    Type == typeof(sbyte) ||
                    Type == typeof(short) ||
                    Type == typeof(ushort) ||

                    Type == typeof(double) ||
                    Type == typeof(float) ||
                    Type == typeof(decimal) ||

                    Type == typeof(int?) ||
                    Type == typeof(uint?) ||
                    Type == typeof(long?) ||
                    Type == typeof(ulong?) ||
                    Type == typeof(byte?) ||
                    Type == typeof(sbyte?) ||
                    Type == typeof(short?) ||
                    Type == typeof(ushort?) ||

                    Type == typeof(double?) ||
                    Type == typeof(float?) ||
                    Type == typeof(decimal?))
                {
                    var currentCulture = Thread.CurrentThread.CurrentCulture;
                    try
                    {
                        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                        Value = value.ToString();
                    }
                    finally
                    {
                        Thread.CurrentThread.CurrentCulture = currentCulture;
                    }
                }
            }
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        public string GetNativeValue()
        {
            return base.Value;
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        public static DateTime GetDateTimeFromValue(string value)
        {
            if (value == "null")
                return DateTime.Now;

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                return DateTime.Parse(value);
            }
            catch
            {
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
            return DateTime.Now;
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        internal static string GetValueFromDateTime(DateTime value)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                return value.ToString();
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        /// <summary>
		/// Internal use only.
		/// </summary>
		public static DateTimeOffset GetDateTimeOffsetFromValue(string value)
        {
            if (value == "null")
                return DateTimeOffset.Now;

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                return DateTimeOffset.Parse(value);
            }
            catch
            {
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
            return DateTimeOffset.Now;
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        internal static string GetValueFromDateTimeOffset(DateTimeOffset value)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                return value.ToString();
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        private static string ConvertValue(object value)
        {
            if (value == null)
                return string.Empty;

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                if (value is DateTime)
                    return ((DateTime)value).ToString();

                else if (value is DateTimeOffset)
                    return ((DateTimeOffset)value).ToString();

                else if (value is Metafile)
                    return StiMetafileConverter.MetafileToString(value as Metafile);

                else if (value is Image)
                    return StiImageConverter.ImageToString(value as Image);

                else if (value is byte[])
                    return Convert.ToBase64String(value as byte[]);

                else if (value is Range)
                    return RangeConverter.RangeToString(value as Range);

                else if (value is bool)
                    return (bool)value ? "true" : "false";

                else
                    return value.ToString();
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        /// <summary>
        /// Returns variable in the form of the expression.
        /// </summary>
        /// <param name="variable">Variable for convertation.</param>
        /// <returns>Converted expression.</returns>
        public static string GetExpressionString(StiVariable variable)
        {
            return variable != null ? variable.Name : "";
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool onlyAlias)
        {
            if (IsCategory)
                return Category;

            if (onlyAlias && !string.IsNullOrWhiteSpace(Alias))
                return Alias;

            if (Name == Alias || string.IsNullOrWhiteSpace(Alias))
                return Name;

            return $"{Name} [{Alias}]";
        }

        public object Eval(StiReport report)
        {
            return Engine.StiParser.PrepareVariableValue(this, report, null, true);
        }
        #endregion

        /// <summary>
        /// Creates an object of the type StiVariable.
        /// </summary>
        public StiVariable()
            :
            this(string.Empty, string.Empty, typeof(string), string.Empty, false)
        {
        }

        /// <summary>
        /// Creates an object of the type StiVariable. In this case creates the category.
        /// </summary>
        /// <param name="category">Category of the variable.</param>
        public StiVariable(string category)
            :
            this(category, string.Empty, typeof(string), string.Empty, false)
        {
        }

        /// <summary>
        /// Creates an object of the type StiVariable. 
        /// </summary>
        /// <param name="name">Name of the variable.</param>
        /// <param name="type">Type of the variable.</param>
        public StiVariable(string name, Type type)
            :
            this(string.Empty, name, name, type, string.Empty, false)
        {
        }

        /// <summary>
        /// Creates an object of the type StiVariable. 
        /// </summary>
        /// <param name="category">Category of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="type">Type of the variable.</param>
        public StiVariable(string category, string name, Type type)
            :
            this(category, name, name, type, string.Empty, false)
        {
        }

        /// <summary>
        /// Creates an object of the type StiVariable. 
        /// </summary>
        /// <param name="name">Name of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        public StiVariable(string name, object value)
            :
            this(string.Empty, name, value)
        {
        }

        /// <summary>
        /// Creates an object of the type StiVariable. 
        /// </summary>
        /// <param name="category">Category of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        public StiVariable(string category, string name, object value)
            :
            this(category, name, name, value, false)
        {
        }

        /// <summary>
        /// Creates an object of the type StiVariable. 
        /// </summary>
        /// <param name="category">Category of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="alias">Alias of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        public StiVariable(string category, string name, string alias, object value)
            :
            this(category, name, alias, value, false)
        {
        }

        /// <summary>
        /// Creates an object of the type StiVariable. 
        /// </summary>
        /// <param name="category">Category of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="alias">Alias of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        /// <param name="readOnly">Value indicates which varibale is read only.</param>
        public StiVariable(string category, string name, string alias, object value, bool readOnly)
            :
            this(category, name, alias, value == null ? typeof(object) : value.GetType(), null, readOnly)
        {
            if ((Type == typeof(DateTime) && value is DateTime) || (Type == typeof(DateTimeOffset) && value is DateTimeOffset))
            {
                var currentCulture = Thread.CurrentThread.CurrentCulture;
                try
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                    if (value is DateTimeOffset)
                        Value = ((DateTimeOffset)value).ToString();
                    else
                        Value = ((DateTime)value).ToString();
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                }
            }
            else
            {
                Value = ConvertValue(value);
            }
        }

        /// <summary>
        /// Creates an object of the type StiVariable. 
        /// </summary>
        /// <param name="category">Category of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="type">Type of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        /// <param name="readOnly">Value indicates which varibale is read only.</param>
        public StiVariable(string category, string name, Type type, string value, bool readOnly)
            :
            this(category, name, name, type, value, readOnly)
        {
        }

        /// <summary>
        /// Creates an object of the type StiVariable. 
        /// </summary>
        /// <param name="category">Category of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="alias">Alias of the variable.</param>
        /// <param name="type">Type of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        /// <param name="readOnly">Value indicates which varibale is read only.</param>
        public StiVariable(string category, string name, string alias, Type type, string value, bool readOnly)
            :
            this(category, name, alias, type, value, readOnly, StiVariableInitBy.Value)
        {
        }

        /// <summary>
		/// Creates an object of the type StiVariable. 
		/// </summary>
		/// <param name="category">Category of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="alias">Alias of the variable.</param>
        /// <param name="description">Description of the variable.</param>
        /// <param name="type">Type of the variable.</param>
        /// <param name="value">Value of the variable.</param>
		/// <param name="readOnly">Value indicates which varibale is read only.</param>
        public StiVariable(string category, string name, string alias, string description, Type type, string value, bool readOnly)
            :
            this(category, name, alias, description, type, value, readOnly, StiVariableInitBy.Value, false)
        {
        }

        /// <summary>
        /// Creates an object of the type StiVariable. This constructor is obsolete! It use 'function' argument. Please use instead it constructor with 'initBy' argument!
        /// </summary>
        /// <param name="category">Category of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="alias">Alias of the variable.</param>
        /// <param name="type">Type of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        /// <param name="readOnly">Value indicates which varibale is read only.</param>
        /// <param name="function">Value which indicates that this variable will be have defalt value based on calculation of expression which specified in Value property.</param>
        [Obsolete("This constructor is obsolete! It use 'function' argument. Please use instead it constructor with 'initBy' argument!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public StiVariable(string category, string name, string alias, Type type, string value,
            bool readOnly, bool function)
            : this(category, name, alias, string.Empty, type, value, readOnly, function, false)
        {
        }

        /// <summary>
        /// Creates an object of the type StiVariable. This constructor is obsolete! It use 'function' argument. Please use instead it constructor with 'initBy' argument!
        /// </summary>
        /// <param name="category">Category of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="alias">Alias of the variable.</param>
        /// <param name="description">Description of the variable.</param>
        /// <param name="type">Type of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        /// <param name="readOnly">Value indicates which varibale is read only.</param>
        /// <param name="function">Value which indicates that this variable will be have defalt value based on calculation of expression which specified in Value property.</param>
        [Obsolete("This constructor is obsolete! It use 'function' argument. Please use instead it constructor with 'initBy' argument!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public StiVariable(string category, string name, string alias, string description, Type type, string value,
            bool readOnly, bool function)
            : this(category, name, alias, description, type, value, readOnly, function, false)
        {
        }

        /// <summary>
        /// Creates an object of the type StiVariable. This constructor is obsolete! It use 'function' argument. Please use instead it constructor with 'initBy' argument!
        /// </summary>
        /// <param name="category">Category of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="alias">Alias of the variable.</param>
        /// <param name="description">Description of the variable.</param>
        /// <param name="type">Type of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        /// <param name="readOnly">Value indicates which varibale is read only.</param>
        /// <param name="function">Value which indicates that this variable will be have defalt value based on calculation of expression which specified in Value property.</param>
        /// <param name="requestFromUser">RequestFromUser of the variable.</param>
        [Obsolete("This constructor is obsolete! It use 'function' argument. Please use instead it constructor with 'initBy' argument!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public StiVariable(string category, string name, string alias, string description, Type type, string value,
            bool readOnly, bool function, bool requestFromUser)
        {
            Category = category;
            Name = name;
            Alias = alias;
            Description = description;
            Type = type;
            this.readOnly = readOnly;
            Function = function;
            Value = value;
            RequestFromUser = requestFromUser;
        }

        /// <summary>
        /// Creates an object of the type StiVariable.
        /// </summary>
        /// <param name="category">Category of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="alias">Alias of the variable.</param>
        /// <param name="type">Type of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        /// <param name="readOnly">Value indicates which varibale is read only.</param>
        /// <param name="initBy">Value which indicates that this variable will be have defalt value based on calculation of expression which specified in Value property or based on default value.</param>
        public StiVariable(string category, string name, string alias, Type type, string value,
            bool readOnly, StiVariableInitBy initBy)
            : this(category, name, alias, string.Empty, type, value, readOnly, initBy, false)
        {
        }

        /// <summary>
        /// Creates an object of the type StiVariable.
        /// </summary>
        /// <param name="category">Category of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="alias">Alias of the variable.</param>
        /// <param name="description">Description of the variable.</param>
        /// <param name="type">Type of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        /// <param name="readOnly">Value indicates which varibale is read only.</param>
        /// <param name="initBy">Value which indicates that this variable will be have defalt value based on calculation of expression which specified in Value property or based on default value.</param>
        public StiVariable(string category, string name, string alias, string description, Type type, string value,
            bool readOnly, StiVariableInitBy initBy)
            : this(category, name, alias, description, type, value, readOnly, initBy, false)
        {
        }

        /// <summary>
        /// Creates an object of the type StiVariable. 
        /// </summary>
        /// <param name="category">Category of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="alias">Alias of the variable.</param>
        /// <param name="description">Description of the variable.</param>
        /// <param name="type">Type of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        /// <param name="readOnly">Value indicates which varibale is read only.</param>
        /// <param name="initBy">Value which indicates that this variable will be have defalt value based on calculation of expression which specified in Value property or based on default value.</param>
        /// <param name="requestFromUser">RequestFromUser of the variable.</param>
        public StiVariable(string category, string name, string alias, string description, Type type, string value,
            bool readOnly, StiVariableInitBy initBy, bool requestFromUser)
        {
            Category = category;
            Name = name;
            Alias = alias;
            Description = description;
            Type = type;
            this.readOnly = readOnly;
            InitBy = initBy;
            Value = value;
            RequestFromUser = requestFromUser;
        }

        /// <summary>
        /// Creates an object of the type StiVariable. 
        /// </summary>
        /// <param name="category">Category of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="alias">Alias of the variable.</param>
        /// <param name="description">Description of the variable.</param>
        /// <param name="type">Type of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        /// <param name="readOnly">Value indicates which varibale is read only.</param>
        /// <param name="initBy">Value which indicates that this variable will be have defalt value based on calculation of expression which specified in Value property or based on default value.</param>
        /// <param name="requestFromUser">RequestFromUser of the variable.</param>
        /// <param name="dialogInfo">StiDialogInfo contain dialog information for using RequestFromUser property of the variable.</param>
        public StiVariable(string category, string name, string alias, string description, Type type, string value,
            bool readOnly, StiVariableInitBy initBy, bool requestFromUser, StiDialogInfo dialogInfo)
        {
            Category = category;
            Name = name;
            Alias = alias;
            Description = description;
            Type = type;
            this.readOnly = readOnly;
            InitBy = initBy;
            Value = value;
            RequestFromUser = requestFromUser;
            DialogInfo = dialogInfo;
        }

        /// <summary>
        /// Creates an object of the type StiVariable. 
        /// </summary>
        /// <param name="category">Category of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="alias">Alias of the variable.</param>
        /// <param name="description">Description of the variable.</param>
        /// <param name="type">Type of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        /// <param name="readOnly">Value indicates which varibale is read only.</param>
        /// <param name="initBy">Value which indicates that this variable will be have defalt value based on calculation of expression which specified in Value property or based on default value.</param>
        /// <param name="requestFromUser">RequestFromUser of the variable.</param>
        /// <param name="dialogInfo">StiDialogInfo contain dialog information for using RequestFromUser property of the variable.</param>
        /// <param name="key">Key string.</param>
        public StiVariable(string category, string name, string alias, string description, Type type, string value,
            bool readOnly, StiVariableInitBy initBy, bool requestFromUser, StiDialogInfo dialogInfo, string key)
        {
            Category = category;
            Name = name;
            Alias = alias;
            Description = description;
            Type = type;
            this.readOnly = readOnly;
            InitBy = initBy;
            Value = value;
            RequestFromUser = requestFromUser;
            DialogInfo = dialogInfo;
            Key = key;
        }

        /// <summary>
        /// Creates an object of the type StiVariable. 
        /// </summary>
        /// <param name="category">Category of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="alias">Alias of the variable.</param>
        /// <param name="description">Description of the variable.</param>
        /// <param name="type">Type of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        /// <param name="readOnly">Value indicates which varibale is read only.</param>
        /// <param name="initBy">Value which indicates that this variable will be have defalt value based on calculation of expression which specified in Value property or based on default value.</param>
        /// <param name="requestFromUser">RequestFromUser of the variable.</param>
        /// <param name="dialogInfo">StiDialogInfo contain dialog information for using RequestFromUser property of the variable.</param>
        /// <param name="key">Key string.</param>
        /// <param name="allowUseAsSqlParameter">Value which allows using this variable as a SQL parameter in the query.</param>
        public StiVariable(string category, string name, string alias, string description, Type type, string value,
            bool readOnly, StiVariableInitBy initBy, bool requestFromUser, StiDialogInfo dialogInfo, string key, bool allowUseAsSqlParameter)
        {
            Category = category;
            Name = name;
            Alias = alias;
            Description = description;
            Type = type;
            this.readOnly = readOnly;
            InitBy = initBy;
            Value = value;
            RequestFromUser = requestFromUser;
            DialogInfo = dialogInfo;
            Key = key;
            AllowUseAsSqlParameter = allowUseAsSqlParameter;
        }

        /// <summary>
        /// Creates an object of the type StiVariable. 
        /// </summary>
        /// <param name="category">Category of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="alias">Alias of the variable.</param>
        /// <param name="description">Description of the variable.</param>
        /// <param name="type">Type of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        /// <param name="readOnly">Value indicates which varibale is read only.</param>
        /// <param name="initBy">Value which indicates that this variable will be have defalt value based on calculation of expression which specified in Value property or based on default value.</param>
        /// <param name="requestFromUser">RequestFromUser of the variable.</param>
        /// <param name="dialogInfo">StiDialogInfo contain dialog information for using RequestFromUser property of the variable.</param>
        /// <param name="key">Key string.</param>
        /// <param name="allowUseAsSqlParameter">Value which allows using this variable as a SQL parameter in the query.</param>
        public StiVariable(string category, string name, string alias, string description, Type type, string value,
            bool readOnly, StiVariableInitBy initBy, bool requestFromUser, StiDialogInfo dialogInfo, string key,
            bool allowUseAsSqlParameter, StiSelectionMode selection)
        {
            Category = category;
            Name = name;
            Alias = alias;
            Description = description;
            Type = type;
            this.readOnly = readOnly;
            InitBy = initBy;
            Value = value;
            RequestFromUser = requestFromUser;
            DialogInfo = dialogInfo;
            Key = key;
            AllowUseAsSqlParameter = allowUseAsSqlParameter;
            Selection = selection;
        }
    }
}