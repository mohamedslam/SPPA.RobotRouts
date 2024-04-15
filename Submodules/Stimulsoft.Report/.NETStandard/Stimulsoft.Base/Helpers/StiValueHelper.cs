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
using System.Text;
using System.Threading;

namespace Stimulsoft.Base.Helpers
{
    public static class StiValueHelper
    {
        #region Methods
        public static bool IsZero(object value)
        {
            if (value == null)
                return true;

            var type = Type.GetTypeCode(value.GetType());
            switch (type)
            {
                case TypeCode.SByte:
                    return (sbyte)value == 0;

                case TypeCode.Int16:
                    return (short)value == 0;

                case TypeCode.UInt16:
                    return (ushort)value == 0;

                case TypeCode.Int32:
                    return (int)value == 0;

                case TypeCode.UInt32:
                    return (uint)value == 0;

                case TypeCode.Int64:
                    return (long)value == 0;

                case TypeCode.UInt64:
                    return (ulong)value == 0;

                case TypeCode.Single:
                    return (float)value == 0;

                case TypeCode.Double:
                    return (double)value == 0;

                case TypeCode.Decimal:
                    return (decimal)value == 0;
            }
            return false;
        }

        public static bool EqualDecimal(object value1, object value2)
        {
            if (value1 == null || value2 == null)
                return false;

            return TryToDecimal(value1) == TryToDecimal(value2);
        }

        public static bool ContainsNumeric(object value)
        {
            if (value == null)
                return false;

            if (value.GetType().IsNumericType())
                return true;

            if (value is string)
            {
                var str = NormalizeFloatingPointValue(value);

                double result;
                return double.TryParse(str, out result);
            }

            return false;
        }
        #endregion

        #region Methods.TryTo
        public static char TryToChar(object value)
        {
            if (value == null)
                return ' ';

            var str = value.ToString();

            return str.Length > 0 ? str[0] : ' ';
        }

        public static string TryToString(object value)
        {
            if (value is string)
                return value as string;

            if (value == null)
                return null;

            return value.ToString();
        }

        public static float TryToFloat(object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                    return 0;

                if (value is bool)
                    return (bool)value ? 1 : 0;

                if (value is float)
                    return (float)value;

                var str = value as string;
                if (str != null)
                {
                    str = NormalizeFloatingPointValue(value);

                    float result;
                    float.TryParse(str, out result);
                    return result;
                }

                if (!value.GetType().IsNumericType())
                    return 0;

                return Convert.ToSingle(value);
            }
            catch
            {
                return 0f;
            }
        }

        public static double TryToDouble(object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                    return 0;

                if (value is bool)
                    return (bool)value ? 1 : 0;

                if (value is double)
                    return (double)value;

                var str = value as string;
                if (str != null)
                {
                    str = NormalizeFloatingPointValue(value);

                    double result;
                    double.TryParse(str, out result);
                    return result;
                }

                if (!value.GetType().IsNumericType())
                    return 0;

                return Convert.ToDouble(value);
            }
            catch
            {
                return 0d;
            }
        }

        public static decimal TryToDecimal(object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                    return 0;

                if (value is bool)
                    return (bool)value ? 1 : 0;

                if (value is decimal)
                    return (decimal)value;

                var str = value as string;
                if (str != null)
                {
                    str = NormalizeFloatingPointValue(value);

                    decimal decimalResult;
                    if (decimal.TryParse(str, out decimalResult))
                        return decimalResult;

                    double doubleResult;
                    if (double.TryParse(str, out doubleResult))
                        return (decimal)doubleResult;

                    return 0;
                }

                if (!value.GetType().IsNumericType())
                    return 0;

                return Convert.ToDecimal(value);
            }
            catch
            {
                return 0m;
            }
        }

        public static bool TryToBool(object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                    return false;

                if (value is bool)
                    return (bool)value;

                var decimalValue = TryToNullableDecimal(value);
                if (decimalValue != null)
                    return decimalValue.Value == 1;

                var str = value as string;
                if (str != null)
                {
                    str = str.ToLowerInvariant();
                    return str == "true" || str == "on" || str == "yes";
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static int TryToInt(object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                    return 0;

                if (value is bool)
                    return (bool)value ? 1 : 0;

                if (value is int)
                    return (int)value;

                if (value is string)
                {
                    int result;
                    int.TryParse((string)value, out result);
                    return result;
                }

                if (!value.GetType().IsNumericType())
                    return 0;

                return Convert.ToInt16(value);
            }
            catch
            {
                return 0;
            }
        }

        internal static uint TryToUInt(object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                    return 0;

                if (value is bool)
                    return (uint)((bool)value ? 1 : 0);

                if (value is uint)
                    return (uint)value;

                if (value is string)
                {
                    uint result;
                    uint.TryParse((string)value, out result);
                    return result;
                }

                if (!value.GetType().IsNumericType())
                    return 0;

                return Convert.ToUInt16(value);
            }
            catch
            {
                return 0;
            }
        }

        public static long TryToLong(object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                    return 0;

                if (value is bool)
                    return (bool)value ? 1 : 0;

                if (value is long)
                    return (long)value;

                if (value is string)
                {
                    long result;
                    long.TryParse((string)value, out result);
                    return result;
                }

                if (!value.GetType().IsNumericType())
                    return 0;

                return Convert.ToInt64(value);
            }
            catch
            {
                return 0;
            }
        }

        internal static ulong TryToULong(object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                    return 0;

                if (value is bool)
                    return (ulong)((bool)value ? 1 : 0);

                if (value is ulong)
                    return (ulong)value;

                if (value is string)
                {
                    ulong result;
                    ulong.TryParse((string)value, out result);
                    return result;
                }

                if (!value.GetType().IsNumericType())
                    return 0;

                return Convert.ToUInt64(value);
            }
            catch
            {
                return 0;
            }
        }

        public static DateTime TryToDateTime(object value)
        {
            try
            {
                if (value is DateTime dateTime)
                    return dateTime;

                if (value is DateTimeOffset dateTimeOffset)
                    return dateTimeOffset.DateTime;

                if (value == null || value == DBNull.Value)
                    return DateTime.Now;

                if (value is string && string.IsNullOrWhiteSpace(value as string))
                    return DateTime.Now;

                if (value.GetType().IsNumericType())
                    return DateTime.Now;

                if (value.GetType().IsEnum)
                    return DateTime.Now;

                if (value is string && DateTime.TryParse((string)value, out var res))
                {
                    return res;
                }

                return (DateTime)StiConvert.ChangeType(value, typeof(DateTime), false);
            }
            catch
            {
                return DateTime.Now;
            }
        }

        public static DateTimeOffset TryToDateTimeOffset(object value)
        {
            try
            {
                if (value is DateTimeOffset)
                    return (DateTimeOffset)value;

                if (value is DateTime)
                    return new DateTimeOffset((DateTime)value);

                if (value == null || value == DBNull.Value)
                    return DateTimeOffset.Now;

                if (value is string && string.IsNullOrWhiteSpace(value as string))
                    return DateTimeOffset.Now;

                if (value.GetType().IsNumericType())
                    return DateTimeOffset.Now;

                if (value.GetType().IsEnum)
                    return DateTimeOffset.Now;

                return (DateTimeOffset)StiConvert.ChangeType(value, typeof(DateTimeOffset), false);
            }
            catch
            {
                return DateTimeOffset.Now;
            }
        }

        public static TimeSpan TryToTimeSpan(object value)
        {
            try
            {
                if (value is TimeSpan)
                    return (TimeSpan)value;

                if (value == null || value == DBNull.Value)
                    return new TimeSpan(0);

                return (TimeSpan)StiConvert.ChangeType(value, typeof(TimeSpan), false);
            }
            catch
            {
                return new TimeSpan(0);
            }
        }
        #endregion

        #region Methods.TryToNullable
        public static float? TryToNullableFloat(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            try
            {
                var str = value as string;
                if (str != null)
                {
                    str = NormalizeFloatingPointValue(value);

                    float result;
                    if (float.TryParse(str, out result))
                        return result;
                    else
                        return null;
                }

                if (value.GetType().IsEnum)
                    return (float)((int)value);

                if (!value.GetType().IsNumericType())
                    return 0;

                return Convert.ToSingle(value);
            }
            catch
            {
                return null;
            }
        }

        public static double? TryToNullableDouble(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            try
            {
                if (value is DateTime dateTime)
                    return dateTime.ToOADate();

                if (value is DateTimeOffset dateTimeOffset)
                    return dateTimeOffset.DateTime.ToOADate();

                var str = value as string;
                if (str != null)
                {
                    str = NormalizeFloatingPointValue(value);

                    double result;
                    if (double.TryParse(str, out result))
                        return result;
                    else
                        return null;
                }

                if (value.GetType().IsEnum)
                    return (double)((int)value);

                if (!value.GetType().IsNumericType()) return 0;
                return Convert.ToDouble(value);
            }
            catch
            {
                return null;
            }
        }

        public static decimal? TryToNullableDecimal(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            try
            {
                var str = value as string;
                if (str != null)
                {
                    str = NormalizeFloatingPointValue(value);

                    decimal result;
                    if (decimal.TryParse(str, out result))
                        return result;

                    return null;
                }

                if (value.GetType().IsEnum)
                    return (decimal)((int)value);

                if (!value.GetType().IsNumericType())
                    return null;

                return Convert.ToDecimal(value);
            }
            catch
            {
                return null;
            }
        }

        public static long? TryToNullableLong(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            try
            {
                if (value is string)
                {
                    long result;
                    if (long.TryParse((string)value, out result))
                        return result;
                    else
                        return null;
                }

                if (value.GetType().IsEnum)
                    return (long)((int)value);

                if (!value.GetType().IsNumericType())
                    return null;

                return Convert.ToInt64(value);
            }
            catch
            {
                return null;
            }
        }

        public static int? TryToNullableInt(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            try
            {
                if (value is string)
                {
                    int result;
                    if (int.TryParse((string)value, out result))
                        return result;
                    else
                        return null;
                }

                if (value.GetType().IsEnum)
                    return (int)value;

                if (!value.GetType().IsNumericType())
                    return null;

                return Convert.ToInt16(value);
            }
            catch
            {
                return null;
            }
        }

        internal static ulong? TryToNullableULong(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            try
            {
                if (value is string)
                {
                    ulong result;
                    if (ulong.TryParse((string)value, out result))
                        return result;
                    else
                        return null;
                }

                if (value.GetType().IsEnum)
                    return (ulong)((int)value);

                if (!value.GetType().IsNumericType())
                    return null;

                return Convert.ToUInt64(value);
            }
            catch
            {
                return null;
            }
        }

        internal static uint? TryToNullableUInt(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            try
            {
                if (value is string)
                {
                    uint result;
                    if (uint.TryParse((string)value, out result))
                        return result;
                    else
                        return null;
                }

                if (value.GetType().IsEnum)
                    return (uint)((int)value);

                if (!value.GetType().IsNumericType())
                    return null;

                return Convert.ToUInt16(value);
            }
            catch
            {
                return null;
            }
        }

        public static DateTime? TryToNullableDateTime(object value)
        {
            if (value is DateTime dateTime)
                return dateTime;

            if (value is DateTimeOffset dateTimeOffset)
                return dateTimeOffset.DateTime;

            if (value == null)
                return null;

            try
            {
                return (DateTime?)StiConvert.ChangeType(value, typeof(DateTime?), false);
            }
            catch
            {
                return null;
            }
        }

        public static TimeSpan? TryToNullableTimeSpan(object value)
        {
            if (value is TimeSpan)
                return (TimeSpan)value;

            if (value == null)
                return null;

            try
            {
                return (TimeSpan?)StiConvert.ChangeType(value, typeof(TimeSpan?), false);
            }
            catch
            {
                return null;
            }
        }

        public static Guid TryToGuid(object value)
        {
            if (value is Guid)
                return (Guid)value;

            if (value == null || string.IsNullOrEmpty(value as string))
                return Guid.Empty;

            try
            {
                return (Guid)StiConvert.ChangeType(value, typeof(Guid), false);
            }
            catch
            {
                return Guid.Empty;
            }
        }
        #endregion

        #region Methods.Parse
        public static decimal ParseDecimal(string value)
        {
            if (value == "0" || string.IsNullOrWhiteSpace(value))
                return 0m;

            var before = new StringBuilder();
            var after = new StringBuilder();
            var dec = 1;

            var isBefore = true;
            foreach (var chr in value)
            {
                if (chr == ',' || chr == '.')
                {
                    isBefore = false;
                }
                else
                {
                    if (isBefore)
                        before = before.Append(chr);
                    else
                    {
                        after = after.Append(chr);
                        dec *= 10;
                    }
                }
            }
            if (before.Length == 0 && after.Length != 0)
                return (decimal)long.Parse(after.ToString()) / dec;

            if (before.Length != 0 && after.Length != 0)
                return long.Parse(before.ToString()) + (decimal)long.Parse(after.ToString()) / dec;

            if (before.Length != 0 && after.Length == 0)
                return long.Parse(before.ToString());

            return 0m;
        }

        public static double ParseDouble(string value)
        {
            if (value == "0" || string.IsNullOrWhiteSpace(value))
                return 0d;

            var before = new StringBuilder();
            var after = new StringBuilder();
            var dec = 1;

            var isBefore = true;
            foreach (var chr in value)
            {
                if (chr == ',' || chr == '.')
                {
                    isBefore = false;
                }
                else
                {
                    if (isBefore)
                        before = before.Append(chr);
                    else
                    {
                        after = after.Append(chr);
                        dec *= 10;
                    }
                }
            }
            if (before.Length == 0 && after.Length != 0)
                return (double)long.Parse(after.ToString()) / dec;

            if (before.Length != 0 && after.Length != 0)
                return long.Parse(before.ToString()) + (double)long.Parse(after.ToString()) / dec;

            if (before.Length != 0 && after.Length == 0)
                return long.Parse(before.ToString());

            return 0d;
        }

        public static float ParseFloat(string value)
        {
            if (value == "0" || string.IsNullOrWhiteSpace(value))
                return 0f;

            var before = new StringBuilder();
            var after = new StringBuilder();
            var dec = 1;

            var isBefore = true;
            foreach (var chr in value)
            {
                if (chr == ',' || chr == '.')
                {
                    isBefore = false;
                }
                else
                {
                    if (isBefore)
                        before = before.Append(chr);
                    else
                    {
                        after = after.Append(chr);
                        dec *= 10;
                    }
                }
            }
            if (before.Length == 0 && after.Length != 0)
                return (float)long.Parse(after.ToString()) / dec;

            if (before.Length != 0 && after.Length != 0)
                return long.Parse(before.ToString()) + (float)long.Parse(after.ToString()) / dec;

            if (before.Length != 0 && after.Length == 0)
                return long.Parse(before.ToString());

            return 0f;
        }

        private static string NormalizeFloatingPointValue(object value)
        {
            return ((string)value).Replace(".", ",").Replace(",", Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        }
        #endregion
    }
}
