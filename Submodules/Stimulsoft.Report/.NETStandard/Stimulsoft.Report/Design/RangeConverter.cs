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
using System.Xml;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace Stimulsoft.Report.Design
{
	/// <summary>
	/// Converts Range from one data type to another. 
	/// </summary>
	public class RangeConverter : TypeConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return false; 
		}

        public static string RangeToString(Range range)
        {
            var converter = new RangeConverter();
            return converter.ConvertToString(range);
        }

        public static Range StringToRange(string str)
        {
            var converter = new RangeConverter();
            return converter.ConvertFromString(str) as Range;
        }


		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, 
			object value, Type destinationType) 
		{
			#region Convert to InstanceDescriptor
			if (destinationType == typeof(InstanceDescriptor) && value != null)
            {
                if (value is Range)
                {
                    var range = value as Range;
                    var types = new[]
                    {
                        range.RangeType,
						range.RangeType
					};

                    var info = typeof(Range).GetConstructor(types);
                    if (info != null)
                    {
                        var objs = new[]	
						{	
                            range.FromObject,
							range.ToObject
						};

                        return CreateNewInstanceDescriptor(info, objs);
                    }
                }
            }
			#endregion
			
			#region Convert to string
			else if (destinationType == typeof(string))
            {
                #region CharRange
                var charRange = value as CharRange;
                if (charRange != null)
                {
                    return $"Char,{XmlConvert.EncodeName(charRange.From.ToString())},{XmlConvert.EncodeName(charRange.To.ToString())}";
                }
                #endregion

                #region DateTimeRange
                string str1;
                string str2;

                var dateTimeRange = value as DateTimeRange;
                if (dateTimeRange != null)
                {
                    var currentCulture = Thread.CurrentThread.CurrentCulture;

                    try
                    {
                        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                        str1 = dateTimeRange.From.ToString();
                        str2 = dateTimeRange.To.ToString();
                    }
                    finally
                    {
                        Thread.CurrentThread.CurrentCulture = currentCulture;
                    }

                    return $"DateTime,{XmlConvert.EncodeName(str1)},{XmlConvert.EncodeName(str2)}";
                }
                #endregion

                #region TimeSpanRange
                var timeSpanRange = value as TimeSpanRange;
                if (timeSpanRange != null)
                {
                    var currentCulture = Thread.CurrentThread.CurrentCulture;

                    try
                    {
                        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                        str1 = timeSpanRange.From.ToString();
                        str2 = timeSpanRange.To.ToString();
                    }
                    finally
                    {
                        Thread.CurrentThread.CurrentCulture = currentCulture;
                    }

                    return $"TimeSpan,{XmlConvert.EncodeName(str1)},{XmlConvert.EncodeName(str2)}"; 
                }
                #endregion

                #region DecimalRange
                var decimalRange = value as DecimalRange;
                if (decimalRange != null)
                {
                    return string.Format("Decimal,{0},{1}", 
                        XmlConvert.EncodeName(decimalRange.From.ToString().Replace(",", ".")), 
                        XmlConvert.EncodeName(decimalRange.To.ToString().Replace(",", ".")));
                }
                #endregion

                #region FloatRange
                var floatRange = value as FloatRange;
                if (floatRange != null)
                {
                    return string.Format("Float,{0},{1}",
                        XmlConvert.EncodeName(floatRange.From.ToString().Replace(",", ".")),
                        XmlConvert.EncodeName(floatRange.To.ToString().Replace(",", ".")));
                }
                #endregion

                #region DoubleRange
                var doubleRange = value as DoubleRange;
                if (doubleRange != null)
                {
                    return string.Format("Double,{0},{1}",
                        XmlConvert.EncodeName(doubleRange.From.ToString().Replace(",", ".")),
                        XmlConvert.EncodeName(doubleRange.To.ToString().Replace(",", ".")));
                }
                #endregion

                #region ByteRange
                var byteRange = value as ByteRange;
                if (byteRange != null)
                {
                    return string.Format("Byte,{0},{1}", byteRange.From, byteRange.To);
                }
                #endregion

                #region ShortRange
                var shortRange = value as ShortRange;
                if (shortRange != null)
                {
                    return string.Format("Short,{0},{1}", shortRange.From, shortRange.To);
                }
                #endregion

                #region IntRange
                var intRange = value as IntRange;
                if (intRange != null)
                {
                    return string.Format("Int,{0},{1}", intRange.From, intRange.To);
                }
                #endregion

                #region LongRange
                var longRange = value as LongRange;
                if (longRange != null)
                {
                    return string.Format("Long,{0},{1}", longRange.From, longRange.To);
                }
                #endregion

                #region GuidRange
                var guidRange = value as GuidRange;
                if (guidRange != null)
                {
                    return string.Format("Guid,{0},{1}", XmlConvert.EncodeName(guidRange.From.ToString()), XmlConvert.EncodeName(guidRange.To.ToString()));
                }
                #endregion

                #region StringRange
                var stringRange = value as StringRange;
                if (stringRange != null)
                {
                    return string.Format("String,{0},{1}", XmlConvert.EncodeName(stringRange.From), XmlConvert.EncodeName(stringRange.To));
                }
                #endregion                
            }
			#endregion

			return base.ConvertTo(context, culture, value, destinationType);
		}

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

			if (destinationType == typeof(string))
                return true;

			return base.CanConvertTo(context, destinationType); 
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				var text = value as string;
				var splits = new[]{ ',' };
				var words = text.Split(splits);

                #region CharRange
                if (words[0] == "Char")
                {                    
                    return new CharRange(XmlConvert.DecodeName(words[1])[0], XmlConvert.DecodeName(words[2])[0]);
                }
                #endregion

                #region DateTimeRange
                if (words[0] == "DateTime")
                {
                    DateTime? date1;
                    DateTime? date2;

                    var currentCulture = Thread.CurrentThread.CurrentCulture;

                    try
                    {
                        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                        var str1 = XmlConvert.DecodeName(words[1]);
                        var str2 = XmlConvert.DecodeName(words[2]);

                        if (string.IsNullOrEmpty(str1.Trim()))
                            date1 = null;
                        else
                            date1 = DateTime.Parse(str1);

                        if (string.IsNullOrEmpty(str2.Trim()))
                            date2 = null;
                        else
                            date2 = DateTime.Parse(str2);
                    }
                    finally
                    {
                        Thread.CurrentThread.CurrentCulture = currentCulture;
                    }

                    return new DateTimeRange(date1, date2);
                }
                #endregion

                #region TimeSpanRange
                if (words[0] == "TimeSpan")
                {
                    TimeSpan? time1;
                    TimeSpan? time2;

                    var currentCulture = Thread.CurrentThread.CurrentCulture;

                    try
                    {
                        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                        var str1 = XmlConvert.DecodeName(words[1]);
                        var str2 = XmlConvert.DecodeName(words[2]);

                        if (string.IsNullOrEmpty(str1.Trim()))
                            time1 = null;
                        else
                            time1 = TimeSpan.Parse(str1);

                        if (string.IsNullOrEmpty(str2.Trim()))
                            time2 = null;
                        else
                            time2 = TimeSpan.Parse(str2);
                    }
                    finally
                    {
                        Thread.CurrentThread.CurrentCulture = currentCulture;
                    }

                    return new TimeSpanRange(time1, time2);
                }
                #endregion

                #region DecimalRange
                if (words[0] == "Decimal")
                {
                    var sep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    var num1 = decimal.Parse(XmlConvert.DecodeName(words[1]).Replace(".", ",").Replace(",", sep));
                    var num2 = decimal.Parse(XmlConvert.DecodeName(words[2]).Replace(".", ",").Replace(",", sep));
                    return new DecimalRange(num1, num2);
                }
                #endregion

                #region FloatRange
                if (words[0] == "Float")
                {
                    var sep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    var num1 = float.Parse(XmlConvert.DecodeName(words[1]).Replace(".", ",").Replace(",", sep));
                    var num2 = float.Parse(XmlConvert.DecodeName(words[2]).Replace(".", ",").Replace(",", sep));
                    return new FloatRange(num1, num2);
                }
                #endregion

                #region DoubleRange
                if (words[0] == "Double")
                {
                    var sep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    var num1 = double.Parse(XmlConvert.DecodeName(words[1]).Replace(".", ",").Replace(",", sep));
                    var num2 = double.Parse(XmlConvert.DecodeName(words[2]).Replace(".", ",").Replace(",", sep));
                    return new DoubleRange(num1, num2);
                }
                #endregion

                #region ByteRange
                if (words[0] == "Byte")
                {
                    var num1 = byte.Parse(words[1]);
                    var num2 = byte.Parse(words[2]);
                    return new ByteRange(num1, num2);
                }
                #endregion

                #region ShortRange
                if (words[0] == "Short")
                {
                    var num1 = short.Parse(words[1]);
                    var num2 = short.Parse(words[2]);
                    return new ShortRange(num1, num2);
                }
                #endregion

                #region IntRange
                if (words[0] == "Int")
                {
                    var num1 = int.Parse(words[1]);
                    var num2 = int.Parse(words[2]);
                    return new IntRange(num1, num2);
                }
                #endregion

                #region LongRange
                if (words[0] == "Long")
                {
                    var num1 = long.Parse(words[1]);
                    var num2 = long.Parse(words[2]);
                    return new LongRange(num1, num2);
                }
                #endregion

                #region GuidRange
                if (words[0] == "Guid")
                {
                    var str1 = XmlConvert.DecodeName(words[1]);
                    var str2 = XmlConvert.DecodeName(words[2]);

                    var guid1 = string.IsNullOrEmpty(str1) ? new Guid() : new Guid(str1);
                    var guid2 = string.IsNullOrEmpty(str2) ? new Guid() : new Guid(str2);

                    return new GuidRange(guid1, guid2);
                }
                #endregion

                #region StringRange
                if (words[0] == "String")
                {
                    var str1 = XmlConvert.DecodeName(words[1]);
                    var str2 = XmlConvert.DecodeName(words[2]);

                    return new StringRange(str1, str2);
                }
                #endregion
            }

			return base.ConvertFrom(context, culture, value); 
		}

        private object CreateNewInstanceDescriptor(ConstructorInfo info, object[] objs)
        {
            return new InstanceDescriptor(info, objs);
        }
	}
}
