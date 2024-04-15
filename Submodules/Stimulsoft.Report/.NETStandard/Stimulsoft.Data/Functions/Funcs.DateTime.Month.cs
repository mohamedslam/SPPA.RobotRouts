#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
using System.Linq;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Data.Extensions;
using Stimulsoft.Data.Types;

namespace Stimulsoft.Data.Functions
{
    public partial class Funcs
    {
        public static StiMonth? MonthIdent(DateTime? dateTime)
        {
            if (dateTime == null) 
                return null;

            return (StiMonth)Enum.ToObject(typeof(StiMonth), Month(dateTime));
        }

        public static object MonthIdentObject(object value)
        {
            if (ListExt.IsList(value))
            {
                return ListExt.ToList(value).Select(MonthIdentObject);
            }
            else
            {
                if (value is string)
                    return StiMonthToStrHelper.Month(value as string);
                
                else if (value != null && value.GetType().IsNumericType())
                    return StiMonthToStrHelper.Month(StiValueHelper.TryToInt(value));
                
                else
                    return MonthIdent(StiValueHelper.TryToNullableDateTime(value));
            }
        }

        /// <summary>
        /// Returns StiFiscalMonth object which based on DateTime value and from an index or an english name of first month in the year (start from 1).
        /// </summary>
        /// <param name="dateTime">A base DateTime value to get month.</param>
        /// <param name="startMonth">An index or an english name of the first month in the year.</param>
        public static StiFiscalMonth FiscalMonthIdent(DateTime? dateTime, object startMonth)
        {
            if (dateTime == null)
                return null;

            var startMonthIdent = ObjectToMonthIdent(startMonth);
            return new StiFiscalMonth(MonthIdent(dateTime).Value, startMonthIdent);
        }
        
        public static object FiscalMonthIdentObject(object value, object startMonth)
        {
            if (ListExt.IsList(value))
            {
                return ListExt.ToList(value).Select(i => FiscalMonthIdentObject(i, startMonth));
            }
            else
            {
                var startMonthIdent = ObjectToMonthIdent(startMonth);
                if (value is string)
                    return new StiFiscalMonth(StiMonthToStrHelper.Month(value as string).Value, startMonthIdent);

                else if (value != null && value.GetType().IsNumericType())
                    return new StiFiscalMonth(StiMonthToStrHelper.Month(StiValueHelper.TryToInt(value)).Value, startMonthIdent);

                else
                    return FiscalMonthIdent(StiValueHelper.TryToNullableDateTime(value), startMonthIdent);
            }
        }

        private static StiMonth ObjectToMonthIdent(object value)
        {
            if (value != null && value.GetType().IsNumericType())
            {
                var intValue = StiValueHelper.TryToInt(value);
                return intValue >= 1 && intValue <= 12 ? (StiMonth)intValue : StiMonth.January;
            }

            if (value is string)
                return StiMonthToStrHelper.Month(value as string).GetValueOrDefault(StiMonth.January);

            if (value is StiMonth startMonth)
                return startMonth;

            return StiMonth.January;
        }

        public static int Month(DateTime? dateTime)
        {
            if (dateTime == null) 
                return -1;

            return dateTime.Value.Month;
        }

        public static object MonthObject(object value)
        {
            if (ListExt.IsList(value))
            {
                return ListExt.ToList(value).Select(MonthObject);
            }
            else
            {
                if (value is string)
                {
                    var month = StiMonthToStrHelper.Month(value as string);
                    if (month == null)
                        return -1;
                    else 
                        return (int)month;
                }
                else
                    return Month(StiValueHelper.TryToNullableDateTime(value));
            }
        }

        public static string MonthName(DateTime? date)
        {
            if (date.HasValue)
                return StiMonthToStrHelper.MonthName(date.Value);
            else
                return string.Empty;
        }

        public static object MonthNameObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(MonthName);
            else
                return MonthName(StiValueHelper.TryToNullableDateTime(value));
        }

        public static string MonthName(DateTime? date, bool localized)
        {
            if (date.HasValue)
                return StiMonthToStrHelper.MonthName(date.Value, localized);
            else
                return string.Empty;
        }

        public static object MonthNameObject(object value, bool localized)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(d => MonthName(d, localized));
            else
                return MonthName(StiValueHelper.TryToNullableDateTime(value), localized);
        }

        public static string MonthName(DateTime? date, string culture)
        {
            if (date.HasValue)
                return StiMonthToStrHelper.MonthName(date.Value, culture);
            else
                return string.Empty;
        }

        public static object MonthNameObject(object value, string culture)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(d => MonthName(d, culture));
            else
                return MonthName(StiValueHelper.TryToNullableDateTime(value), culture);
        }

        public static string MonthName(DateTime? date, string culture, bool upperCase)
        {
            if (date.HasValue)
                return StiMonthToStrHelper.MonthName(date.Value, culture, upperCase);
            else
                return string.Empty;
        }

        public static object MonthNameObject(object value, string culture, bool upperCase)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(d => MonthName(d, culture, upperCase));
            else
                return MonthName(StiValueHelper.TryToNullableDateTime(value), culture, upperCase);
        }
    }
}