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

using Stimulsoft.Base.Helpers;
using System;
using System.Globalization;
using System.Threading;

namespace Stimulsoft.Base
{
    public static class StiConvert
    {
        #region Properties
        public static bool UseDefaultDateTimeForNullValues { get; set; }

        public static DateTime DefaultDateTimeForNullValues { get; set; } = DateTime.MinValue;

        public static bool UseDefaultDateTimeOffsetForNullValues { get; set; }

        public static DateTimeOffset DefaultDateTimeOffsetForNullValues { get; set; } = DateTimeOffset.MinValue;
        #endregion

        #region Methods
        /// <summary>
        /// Internal use only.
        /// </summary>
        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Changes a type of the specified value. 
        /// </summary>
        /// <param name="value">A parameter which type will be changed.</param>
        /// <param name="conversionType">A type to which the value parameter will be converted.</param>
        /// <returns>Returns a value of the converted type.</returns>
        public static object ChangeType(object value, Type conversionType)
        {
            return ChangeType(value, conversionType, true);
        }

        /// <summary>
        /// Changes a type of the specified value. 
        /// </summary>
        /// <param name="value">A parameter which type will be changed.</param>
        /// <param name="conversionType">A type to which the value parameter will be converted.</param>
        /// <param name="convertNulls">A parameter which, when converting zero values, instead of null, returns String.Empty, false for Boolean, ' ' for char or null for DateTime.</param>
        /// <returns>Returns a value of the converted type.</returns>
        public static object ChangeType(object value, Type conversionType, bool convertNulls)
        {
            try
            {
                #region From Null
                if ((value == null || value is DBNull) && convertNulls)
                    return ChangeTypeFromNullValue(conversionType);
                #endregion

                #region From String
                if (value is string)
                {
                    if (conversionType == typeof(string))
                        return value;

                    if (conversionType == typeof(decimal) ||
                        conversionType == typeof(double) ||
                        conversionType == typeof(float) ||
                        conversionType == typeof(decimal?) ||
                        conversionType == typeof(double?) ||
                        conversionType == typeof(float?))
                    {
                        var text = value as string;
                        var sep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                        text = text.Replace(".", ",").Replace(",", sep);
                        value = text.Trim();
                        if (text.Length == 0)
                            value = "0";
                    }
                    else
                    {
                        if (conversionType == typeof(int) ||
                             conversionType == typeof(long) ||
                             conversionType == typeof(byte) ||
                             conversionType == typeof(short) ||
                             conversionType == typeof(uint) ||
                             conversionType == typeof(ulong) ||
                             conversionType == typeof(sbyte) ||
                             conversionType == typeof(ushort) ||
                             conversionType == typeof(int?) ||
                             conversionType == typeof(long?) ||
                             conversionType == typeof(byte?) ||
                             conversionType == typeof(short?) ||
                             conversionType == typeof(uint?) ||
                             conversionType == typeof(ulong?) ||
                             conversionType == typeof(sbyte?) ||
                             conversionType == typeof(ushort?))
                        {
                            if (((string)value).Length == 0)
                                value = "0";
                        }
                    }
                }
                #endregion

                #region To Enum
                if (conversionType.IsEnum)
                {
                    if (value is int ||
                        value is uint ||
                        value is long ||
                        value is ulong ||
                        value is short ||
                        value is ushort ||
                        value is byte ||
                        value is sbyte)
                    {
                        return Enum.ToObject(conversionType, (int)value);
                    }
                    
                    if (value != null && value.GetType().IsEnum)
                        return Enum.ToObject(conversionType, value);
                }
                #endregion

                #region To Nullable
                if (IsNullableType(conversionType))
                {
                    var flag = value != null && Nullable.GetUnderlyingType(value.GetType()) != null;
                    if (flag)
                    {
                        if (value is int)
                            return (int)value;

                        if (value is uint)
                            return (uint)value;

                        if (value is long)
                            return (long)value;

                        if (value is ulong)
                            return (ulong)value;

                        if (value is byte)
                            return (byte)value;

                        if (value is sbyte)
                            return (sbyte)value;

                        if (value is short)
                            return (short)value;

                        if (value is ushort)
                            return (ushort)value;

                        if (value is bool)
                            return (bool)value;

                        if (value is char)
                            return (char)value;

                        if (value is double)
                            return (double)value;

                        if (value is float)
                            return (float)value;

                        if (value is decimal)
                            return (decimal)value;

                        if (value is DateTime)
                            return (DateTime)value;

                        if (value is DateTimeOffset)
                            return (DateTimeOffset)value;

                        if (value is TimeSpan)
                            return (TimeSpan)value;
                    }
                }
                #endregion

                #region From IConvertible
                if (value is IConvertible)
                    return ChangeTypeToConvertible(value, conversionType);
                #endregion

                return value;
            }
            catch
            {
                return value;
            }
        }

        private static object ChangeTypeToConvertible(object value, Type conversionType)
        {
            if (value is DBNull)
                return null;

            if (value is string && conversionType == typeof(Guid))
                return Guid.Parse(value as string);

            if (conversionType == typeof(DateTime) || conversionType == typeof(DateTime?))
                return ChangeTypeToDateTime(value, conversionType);

            if (conversionType == typeof(DateTimeOffset) || conversionType == typeof(DateTimeOffset?))
                return ChangeTypeToDateTimeOffset(value, conversionType);

            if (conversionType == typeof(TimeSpan))
                return ChangeTypeToTimeSpan(value, conversionType);

            if (conversionType.IsEnum)
            {
                var enumInt = StiValueHelper.TryToInt(value);
                if (Enum.IsDefined(conversionType, enumInt))
                    return Enum.ToObject(conversionType, enumInt);
                
                return null;
            }

            var type = Nullable.GetUnderlyingType(conversionType) ?? conversionType;
            return Convert.ChangeType(value, type);
        }

        private static object ChangeTypeToTimeSpan(object value, Type conversionType)
        {
            try
            {
                if (value is string)
                {
                    if (string.IsNullOrWhiteSpace(value as string)) return null;

                    TimeSpan result;

                    if (TimeSpan.TryParse(value as string, out result))
                        return result;
                    
                    return null;
                }
                else if (conversionType.IsEnum)
                {
                    return Enum.Parse(conversionType, value.ToString());
                }
                else
                {
                    return Convert.ChangeType(value, conversionType);
                }
            }
            catch
            {
                return null;
            }
        }

        private static object ChangeTypeToDateTimeOffset(object value, Type conversionType)
        {
            try
            {
                if (value is string)
                {
                    if (string.IsNullOrWhiteSpace(value as string))
                        return null;

                    var str = value as string;

                    DateTimeOffset result;

                    if (DateTimeOffset.TryParse(str, out result))
                        return result;

                    DateTime resultDateTime;
                    if (str.TryParseDateTime(out resultDateTime))
                        return new DateTimeOffset(resultDateTime);

                    return null;
                }
                else
                    return Convert.ChangeType(value, conversionType);
            }
            catch
            {
                return null;
            }
        }

        private static object ChangeTypeToDateTime(object value, Type conversionType)
        {
            try
            {
                if (value is string)
                {
                    if (string.IsNullOrWhiteSpace(value as string))
                        return null;

                    var str = value as string;
                    DateTime result;
                    if (str.TryParseDateTime(out result))
                        return result;

                    return null;
                }

                if (value is double)
                {
                    DateTime resultDateTime;
                    
                    if (DateTime.TryParse(value.ToString(), new CultureInfo("en-US", true), DateTimeStyles.None, out resultDateTime))
                        return resultDateTime;

                    else if (DateTime.TryParse(value.ToString(), out resultDateTime))
                        return resultDateTime;
                }

                if (value is DateTime)
                    return value;

                else if (value == null || value == DBNull.Value)
                    return null;

                else if (value.GetType().IsNumericType())
                    return null;

                else if (value.GetType().IsEnum)
                    return null;

                else
                    return Convert.ChangeType(value, conversionType);
            }
            catch
            {
                return null;
            }
        }

        private static object ChangeTypeFromNullValue(Type conversionType)
        {
            if (conversionType == typeof(string))
                return string.Empty;

            if (conversionType == typeof(bool))
                return false;

            if (conversionType.IsEnum)
                return Enum.ToObject(conversionType, 0);

            if (conversionType == typeof(Guid?) || conversionType == typeof(Guid))
                return Guid.Empty;

            if (conversionType == typeof(DateTime))
                return UseDefaultDateTimeForNullValues ? (object) DefaultDateTimeForNullValues : null;

            if (conversionType == typeof(DateTimeOffset))
                return UseDefaultDateTimeOffsetForNullValues ? (object)DefaultDateTimeOffsetForNullValues : null;

            if (conversionType == typeof(char))
                return ' ';

            if (!conversionType.IsValueType)
                return null;

            if (conversionType == typeof(int?))
                return null;

            if (conversionType == typeof(uint?))
                return null;

            if (conversionType == typeof(long?))
                return null;

            if (conversionType == typeof(ulong?))
                return null;

            if (conversionType == typeof(byte?))
                return null;

            if (conversionType == typeof(sbyte?))
                return null;

            if (conversionType == typeof(short?))
                return null;

            if (conversionType == typeof(ushort?))
                return null;

            if (conversionType == typeof(bool?))
                return null;

            if (conversionType == typeof(char?))
                return null;

            if (conversionType == typeof(double?))
                return null;

            if (conversionType == typeof(float?))
                return null;

            if (conversionType == typeof(decimal?))
                return null;

            if (conversionType == typeof(TimeSpan?))
                return null;

            try
            {

                if (conversionType == typeof(DateTime?))
                    return UseDefaultDateTimeForNullValues ? (object) DefaultDateTimeForNullValues : null;

                if (conversionType == typeof(DateTimeOffset?))
                    return UseDefaultDateTimeOffsetForNullValues ? (object)DefaultDateTimeOffsetForNullValues : null;

                return Convert.ChangeType(0, conversionType);
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion
    }
}
