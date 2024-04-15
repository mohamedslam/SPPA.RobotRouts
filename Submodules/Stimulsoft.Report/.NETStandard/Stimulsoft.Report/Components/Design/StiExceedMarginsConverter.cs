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
using System.Reflection;
using System.ComponentModel.Design.Serialization;
using System.Collections;
using System.Globalization;
using System.ComponentModel;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Components.Design
{
    /// <summary>
    /// Provides a type converter to convert Enum objects to and from various other representations.
    /// </summary>
    public class StiExceedMarginsConverter : TypeConverter
    {
        #region Properties
        private string LocAll => Loc.GetEnum("StiBorderSidesAll");

        private string LocNone => Loc.GetEnum("StiBorderSidesNone");
        
        private string LocLeft => Loc.GetEnum("StiBorderSidesLeft");
        
        private string LocRight => Loc.GetEnum("StiBorderSidesRight");
        
        private string LocTop => Loc.GetEnum("StiBorderSidesTop");
        
        private string LocBottom => Loc.GetEnum("StiBorderSidesBottom");
        #endregion

        #region Methods
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                try
                {
                    var strValue = (string)value;
                    if (StiPropertyGridOptions.Localizable)
                    {
                        var strs = Enum.GetNames(Type);
                        foreach (string str in strs)
                        {
                            var locName = GetLocName(str);
                            if (locName != null && locName == strValue)
                                strValue = str;
                        }
                    }

                    if (strValue.IndexOf(',') != -1)
                    {
                        long num = 0;
                        var strings = strValue.Split(',');
                        for (var index = 0; (index < strings.Length); index++)
                        {
                            num = num | Convert.ToInt64((Enum)Enum.Parse(this.Type, strings[index], true));
                        }
                        return Enum.ToObject(this.Type, num);
                    }

                    return Enum.Parse(this.Type, strValue, true);

                }
                catch
                {
                    throw new Exception("ConvertInvalidPrimitive");
                }

            }
            return base.ConvertFrom(context, culture, value);
        }

        private string GetLocName(string str)
        {
            if (str == "StiExceedMarginsAll")
                return Loc.GetEnum("StiBorderSidesAll");

            if (str == "StiExceedMarginsNone")
                return Loc.GetEnum("StiBorderSidesNone");

            if (str == "StiExceedMarginsLeft")
                return Loc.GetEnum("StiBorderSidesLeft");

            if (str == "StiExceedMarginsRight")
                return Loc.GetEnum("StiBorderSidesRight");

            if (str == "StiExceedMarginsTop")
                return Loc.GetEnum("StiBorderSidesTop");

            if (str == "StiExceedMarginsBottom")
                return Loc.GetEnum("StiBorderSidesBottom");

            return StiLocalization.Get("PropertyEnum", Type.Name + str, false);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
            object value, Type destinationType)
        {
            if (destinationType == null)
                throw new ArgumentNullException("destinationType");

            if ((destinationType == typeof(string)) && (value != null))
            {
                var underlyingType = Enum.GetUnderlyingType(this.Type);

                if (value is IConvertible && value.GetType() != underlyingType)
                    value = ((IConvertible)value).ToType(underlyingType, culture);

                if (!this.Type.IsDefined(typeof(FlagsAttribute), false) && !Enum.IsDefined(this.Type, value))
                    throw new Exception("EnumConverterInvalidValue");

                var name = Enum.Format(this.Type, value, "G");
                if (StiPropertyGridOptions.Localizable)
                {
                    if (name.IndexOf(',') == -1)
                    {
                        var locName = GetLocName(Type.Name + name);
                        return locName ?? name;
                    }

                    var names = name.Split(',');

                    var itog = "";

                    foreach (string nm in names)
                    {
                        var str = "";
                        if (itog != "")
                            str += ", ";

                        var locName = GetLocName(Type.Name + nm.Trim());

                        if (locName != null)
                            itog += str + locName;

                        else
                            itog += str + nm.Trim();
                    }
                    return itog;
                }
                return name;
            }

            if ((destinationType == typeof(InstanceDescriptor)) && (value != null))
            {
                string text1 = base.ConvertToInvariantString(context, value);
                if (this.Type.IsDefined(typeof(FlagsAttribute), false) && (text1.IndexOf(',') != -1))
                {
                    var underlyingType = Enum.GetUnderlyingType(this.Type);

                    if (!(value is IConvertible))
                        return base.ConvertTo(context, culture, value, destinationType);

                    var obj = ((IConvertible)value).ToType(underlyingType, culture);
                    var types = new[]
                    {
                        typeof(Type),
                        underlyingType
                    };

                    var methodInfo = typeof(Enum).GetMethod("ToObject", types);
                    if (methodInfo == null)
                        return base.ConvertTo(context, culture, value, destinationType);

                    return new InstanceDescriptor(methodInfo, new[] { Type, obj });
                }
                var fieldInfo = this.Type.GetField(text1);
                if (fieldInfo != null)
                    return new InstanceDescriptor(fieldInfo, null);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (this.Values == null)
            {
                var values = Enum.GetValues(this.Type);
                var comparer = this.Comparer;
                if (comparer != null)
                    Array.Sort(values, 0, values.Length, comparer);

                this.Values = new StandardValuesCollection(values);
            }
            return this.Values;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return !this.Type.IsDefined(typeof(FlagsAttribute), false);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return Enum.IsDefined(this.Type, value);
        }
        #endregion

        #region Properties
        protected virtual IComparer Comparer => InvariantComparer.Default;

        protected StandardValuesCollection Values { get; set; }

        protected Type Type => typeof(StiExceedMargins);
        #endregion
    }
}