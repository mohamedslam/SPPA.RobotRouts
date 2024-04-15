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
using Stimulsoft.Base.Localization;
using Stimulsoft.Data.Extensions;
using Stimulsoft.Data.Types;

namespace Stimulsoft.Data.Functions
{
    public partial class Funcs
    {
        public static object AddMonthsObject(object date, int months)
        {
            var dateValue = StiValueHelper.TryToNullableDateTime(date);
            if (dateValue == null) return null;

            return dateValue.Value.AddMonths(months);
        }

        public static DateTime FromOADate(double value)
        {
            return global::System.DateTime.FromOADate(value);
        }

        public static object FromOADateObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToDoubleList(value).Select(FromOADate);
            else
                return global::System.DateTime.FromOADate(StiValueHelper.TryToDouble(value));
        }

        public static double ToOADate(DateTime value)
        {
            return value.ToOADate();
        }

        public static object ToOADateObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(c => c == null ? 0 : c.Value.ToOADate());
            else
                return StiValueHelper.TryToDateTime(value).ToOADate();
        }

        public static DateTime AddYears(DateTime date, int years)
        {
            return date.AddYears(years);
        }

        public static object AddYearsObject(object date, int years)
        {
            var dateValue = StiValueHelper.TryToNullableDateTime(date);
            if (dateValue == null) return null;

            return AddYears(dateValue.Value, years);
        }

        public static int Day(DateTime? dateTime)
        {
            if (dateTime == null) return -1;
            return dateTime.Value.Day;
        }

        public static object DayObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(Day);
            else
                return Day(StiValueHelper.TryToNullableDateTime(value));
        }

        public static TimeSpan? DateDiff(DateTime? date1, DateTime? date2)
        {
            if (date1 == null || date2 == null) return null;
            return date1.Value.Subtract(date2.Value);
        }

        public static object DateDiffObject(object value1, object value2)
        {
            if (ListExt.IsList(value1) || ListExt.IsList(value2))
            {
                var list1 = ListExt.ToNullableDateTimeList(value1);
                var list2 = ListExt.ToNullableDateTimeList(value2);
                var list11 = list1 != null ? list1.ToList() : null;
                var list22 = list2 != null ? list2.ToList() : null;

                var count = list11 != null ? list11.Count : list22.Count;
                var resultList = new List<TimeSpan?>();

                for (var index = 0; index < count; index++)
                {
                    var v1 = list11 != null ? list11[index] : null;
                    var v2 = list22 != null ? list22[index] : null;
                    resultList.Add(DateDiff(v1, v2));
                }

                return resultList;
            }
            else
                return DateDiff(StiValueHelper.TryToNullableDateTime(value1), StiValueHelper.TryToNullableDateTime(value2));
        }

        public static object DateTime(object value)
        {
            if (ListExt.IsList(value))
                return SkipNulls(ListExt.ToList(value)).Select(v => DateTime(v));
            else
                return value is DateTime ? new DateTimeValue(value) : null;
        }
        
        public static int DayOfYear(DateTime? dateTime)
        {
            if (dateTime == null) return -1;
            return dateTime.Value.DayOfYear;
        }

        public static object DayOfYearObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(DayOfYear);
            else
                return DayOfYear(StiValueHelper.TryToNullableDateTime(value));
        }

        public static StiQuarter? FinancialQuarter(DateTime? dateTime)
        {
            if (dateTime == null) return null;
            return (StiQuarter)Enum.ToObject(typeof(StiQuarter), FinancialQuarterIndex(dateTime));
        }

        public static object FinancialQuarterObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(FinancialQuarter);
            else
                return FinancialQuarter(StiValueHelper.TryToNullableDateTime(value));
        }

        public static int FinancialQuarterIndex(DateTime? dateTime)
        {
            if (dateTime == null) return -1;
            return (int)(Math.Ceiling(dateTime.Value.Month / 3.0 + 2) % 4 + 1);
        }

        public static object FinancialQuarterIndexObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(FinancialQuarterIndex);
            else
                return FinancialQuarterIndex(StiValueHelper.TryToNullableDateTime(value));
        }

        public static int Hour(DateTime? dateTime)
        {
            if (dateTime == null) return -1;
            return dateTime.Value.Hour;
        }

        public static object HourObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(Hour);
            else
                return Hour(StiValueHelper.TryToNullableDateTime(value));
        }

        public static DateTime MakeDate(int year, int month = 1, int day = 1)
        {
            return new DateTime(year, month, day);
        }

        public static object MakeDateObject(object year, object month = null, object day = null)
        {
            var intYear = StiValueHelper.TryToInt(year);
            var intMonth = StiValueHelper.TryToNullableInt(month) ?? 1;
            var intDay = StiValueHelper.TryToNullableInt(day) ?? 1;

            return MakeDate(intYear, intMonth, intDay);
        }

        public static DateTime MakeDateTime(int year, int month = 1, int day = 1, int hour = 0, int minute = 0, int second = 0)
        {
            return new DateTime(year, month, day, hour, minute, second);
        }

        public static object MakeDateTimeObject(object year, object month = null, object day = null, object hour = null, object minute = null, object second = null)
        {
            var intYear = StiValueHelper.TryToInt(year);
            var intMonth = StiValueHelper.TryToNullableInt(month) ?? 1;
            var intDay = StiValueHelper.TryToNullableInt(day) ?? 1;
            var intHour = StiValueHelper.TryToNullableInt(hour) ?? 0;
            var intMinute = StiValueHelper.TryToNullableInt(minute) ?? 0;
            var intSecond = StiValueHelper.TryToNullableInt(second) ?? 0;

            return MakeDateTime(intYear, intMonth, intDay, intHour, intMinute, intSecond);
        }

        public static DateTime MakeTime(int hour, int minute = 0, int second = 0)
        {
            var nowDate = Now();
            return new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, hour, minute, second);
        }

        public static object MakeTimeObject(object hour, object minute = null, object second = null)
        {
            var intHour = StiValueHelper.TryToInt(hour);
            var intMinute = StiValueHelper.TryToNullableInt(minute) ?? 0;
            var intSecond = StiValueHelper.TryToNullableInt(second) ?? 0;

            return MakeTime(intHour, intMinute, intSecond);
        }

        public static int Minute(DateTime? dateTime)
        {
            if (dateTime == null) return -1;
            return dateTime.Value.Minute;
        }

        public static object MinuteObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(Minute);
            else
                return Minute(StiValueHelper.TryToNullableDateTime(value));
        }

        public static DateTime Now()
        {
            return global::System.DateTime.Now;
        }

        public static string QuarterName(DateTime? dateTime, bool localized = true)
        {
            var quater = Quarter(dateTime);

            switch (quater)
            {
                case StiQuarter.Q1:
                    return localized 
                        ? Loc.Get("DatePickerRanges", "FirstQuarter")
                        : "First Quarter";

                case StiQuarter.Q2:
                    return localized 
                        ? Loc.Get("DatePickerRanges", "SecondQuarter")
                        : "Second Quarter";

                case StiQuarter.Q3:
                    return localized 
                        ? Loc.Get("DatePickerRanges", "ThirdQuarter")
                        : "Third Quarter";

                case StiQuarter.Q4:
                    return localized 
                        ? Loc.Get("DatePickerRanges", "FourthQuarter")
                        : "FourthQuarter";

                default:
                    return string.Empty;
            }
        }

        public static object QuarterNameObject(object value, bool localized = true)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(c => QuarterName(c, localized));
            else
                return QuarterName(StiValueHelper.TryToNullableDateTime(value), localized);
        }

        public static StiQuarter? Quarter(DateTime? dateTime)
        {
            if (dateTime == null) return null;
            return (StiQuarter)Enum.ToObject(typeof(StiQuarter), QuarterIndex(dateTime));
        }

        public static object QuarterObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(Quarter);
            else
                return Quarter(StiValueHelper.TryToNullableDateTime(value));
        }

        public static int QuarterIndex(DateTime? dateTime)
        {
            if (dateTime == null) return -1;
            return (dateTime.Value.Month + 2) / 3;
        }

        public static object QuarterIndexObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(QuarterIndex);
            else
                return QuarterIndex(StiValueHelper.TryToNullableDateTime(value));
        }

        public static int Second(DateTime? dateTime)
        {
            if (dateTime == null) return -1;
            return dateTime.Value.Second;
        }

        public static object SecondObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(Second);
            else
                return Second(StiValueHelper.TryToNullableDateTime(value));
        }

        public static object Time(object value)
        {
            if (ListExt.IsList(value))
                return SkipNulls(ListExt.ToList(value))
                    .Select(v => v is DateTime ? ((DateTime)v).TimeOfDay : (TimeSpan?)null);
            else
                return value is DateTime ? ((DateTime)value).TimeOfDay : (TimeSpan?)null;
        }

        public static int Year(DateTime? dateTime)
        {
            if (dateTime == null) return -1;
            return dateTime.Value.Year;
        }

        public static object YearObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(Year);
            else
                return Year(StiValueHelper.TryToNullableDateTime(value));
        }

        public static string YearMonth(DateTime? dateTime)
        {
            if (dateTime == null) return "";

            return $"{dateTime.Value.Year}-{dateTime.Value.Month:00}";
        }

        public static object YearMonthObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(YearMonth);
            else
                return YearMonth(StiValueHelper.TryToNullableDateTime(value));
        }
    }
}