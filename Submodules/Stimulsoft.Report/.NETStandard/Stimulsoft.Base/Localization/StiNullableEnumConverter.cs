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

namespace Stimulsoft.Base.Localization
{
	/// <summary>
	/// Provides a type converter to convert Enum objects to and from various other representations.
	/// </summary>
	public class StiNullableEnumConverter : TypeConverter
    {
        #region Methods
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))return true; 
			return base.CanConvertFrom(context, sourceType); 
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))return true; 
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{	
			if ((value as string) != null)
			{
                var underlyingType = Nullable.GetUnderlyingType(NullableType);

				try
				{
					var strValue = (string) value;
                    if (strValue.Length == 0)
                        return null;

					if (StiPropertyGridOptions.Localizable)
					{
                        var strs = Enum.GetNames(underlyingType);
						foreach (var str in strs)
						{
                            var locName = StiLocalization.Get("PropertyEnum", underlyingType.Name + str, false);
						    if (locName != null && locName == strValue)
						        strValue = str;
						}
					}

                    if (strValue.IndexOf(',') != -1)
					{
						long num = 0;
						var strings = strValue.Split(',');
						foreach (var str in strings)
						{
						    num = num | Convert.ToInt64((Enum)Enum.Parse(underlyingType, str, true));
						}
                        return Enum.ToObject(underlyingType, num);
 
					}

                    return Enum.Parse(underlyingType, strValue, true);
				}
				catch
				{
					throw new Exception("ConvertInvalidPrimitive");
				}
			}

			return base.ConvertFrom(context, culture, value); 
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, 
			object value, Type destinationType)
		{

			if (destinationType == null)
			    throw new ArgumentNullException("destinationType");

			if (value == null)
			    return "null";

			if (destinationType == typeof(string))
			{
                if (value is string && (string)value == "null") 
                    return null;

                var nullableType = Nullable.GetUnderlyingType(NullableType);
                var underlyingType = Enum.GetUnderlyingType(nullableType);

			    if (value is IConvertible && value.GetType() != underlyingType)
			        value = ((IConvertible) value).ToType(underlyingType, culture);

			    if (!this.NullableType.IsDefined(typeof(FlagsAttribute), false) && !Enum.IsDefined(nullableType, value))
					throw new Exception("EnumConverterInvalidValue");

                var name = Enum.Format(nullableType, value, "G");
				if (StiPropertyGridOptions.Localizable)
				{
                    if (name.IndexOf(',') == -1)
					{
                        var locName = StiLocalization.Get("PropertyEnum", nullableType.Name + name, false);
						return locName ?? name;
					}
					else
					{
						var names = name.Split(',');
                        var result = string.Empty;

						foreach (var nm in names)
						{
							var str = string.Empty;
                            if (result != string.Empty)
                                str += ", ";

                            var locName = StiLocalization.Get("PropertyEnum", nullableType.Name + nm.Trim(), false);

						    if (locName != null)
							    result += str + locName;
							else
							    result += str + nm.Trim();
						}
						return result;
					}
				}
				return name;
			}

			if (destinationType == typeof(InstanceDescriptor))
			{
                var nullableType = Nullable.GetUnderlyingType(NullableType);

				var text1 = base.ConvertToInvariantString(context, value);
                if (nullableType.IsDefined(typeof(FlagsAttribute), false) && text1.IndexOf(',') != -1)
				{
                    var underlyingType = Enum.GetUnderlyingType(nullableType);
					if (!(value is IConvertible))
					    return base.ConvertTo(context, culture, value, destinationType); 

					var obj = ((IConvertible) value).ToType(underlyingType, culture);
					var types = new[]
					{
						typeof(Type),
						underlyingType
					};

				    var methodInfo = typeof(Enum).GetMethod("ToObject", types);
					if (methodInfo == null)
					    return base.ConvertTo(context, culture, value, destinationType);

                    return new InstanceDescriptor(methodInfo, new[] { nullableType, obj }); 
				}

                var fieldInfo = nullableType.GetField(text1);
				if (fieldInfo != null)
				    return new InstanceDescriptor(fieldInfo, null); 
			}

			return base.ConvertTo(context, culture, value, destinationType); 
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if (this.Values == null)
			{
                var values = Enum.GetValues(Nullable.GetUnderlyingType(NullableType));
			    var list = new ArrayList(values) { "null" };
			    this.Values = new StandardValuesCollection(list);
 			}
			return this.Values;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return !NullableType.IsDefined(typeof(FlagsAttribute), false);
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true; 
		}

		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			return Enum.IsDefined(NullableType, value);
        }
        #endregion

        #region Properties
        protected virtual IComparer Comparer => InvariantComparer.Default;

        protected Type NullableType { get; set; }

        protected StandardValuesCollection Values { get; set; }
        #endregion

        public StiNullableEnumConverter(Type type)
        {
            this.NullableType = type;
        }

        public StiNullableEnumConverter()
        {
        }
    }
}