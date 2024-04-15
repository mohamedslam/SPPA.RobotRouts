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

using Stimulsoft.Data.Functions;
using System;
using System.Globalization;
using System.Threading;

namespace Stimulsoft.Report.Dictionary
{
    public class StiFunctionsDate
    {
        #region DateDiff
        /// <summary>
        /// Returns a number of time intervals between two specified dates.
        /// </summary>
        public static TimeSpan DateDiff(DateTime date1, DateTime date2)
        {
			return date1.Subtract(date2);
        }

        /// <summary>
        /// Returns a number of time intervals between two specified dates.
        /// </summary>
        public static TimeSpan DateDiff(DateTimeOffset date1, DateTimeOffset date2)
        {
            return date1.Subtract(date2);
        }

        /// <summary>
        /// Returns a number of time intervals between two specified dates.
        /// </summary>
        public static TimeSpan? DateDiff(DateTime? date1, DateTime? date2)
		{
			if (date1.HasValue && date2.HasValue)
				return date1.Value.Subtract(date2.Value);
			else
				return null;
		}        

        /// <summary>
        /// Returns a number of time intervals between two specified dates.
        /// </summary>
        public static TimeSpan? DateDiff(DateTimeOffset? date1, DateTimeOffset? date2)
        {
            if (date1.HasValue && date2.HasValue)
                return date1.Value.Subtract(date2.Value);
            else
                return null;
        }
        #endregion

        #region Year
        /// <summary>
        /// Returns the year from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Year(DateTime date)
		{
			return date.Year;
		}

        /// <summary>
		/// Returns the year from a date and returns it as a integer value.
		/// </summary>
		public static Int64 Year(DateTimeOffset date)
        {
            return date.Year;
        }

        /// <summary>
        /// Returns the year from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Year(DateTime? date)
		{
            return date.HasValue ? date.Value.Year : 0;
        }        

        /// <summary>
        /// Returns the year from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Year(DateTimeOffset? date)
        {
            return date.HasValue ? date.Value.Year : 0;
        }
        #endregion

        #region Month
        /// <summary>
        /// Returns the month from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Month(DateTime date)
		{
			return date.Month;
		}

        /// <summary>
        /// Returns the month from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Month(DateTimeOffset date)
        {
            return date.Month;
        }

        /// <summary>
        /// Returns the month from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Month(DateTime? date)
		{
            return date.HasValue ? date.Value.Month : 0;
        }        

        /// <summary>
        /// Returns the month from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Month(DateTimeOffset? date)
        {
            return date.HasValue ? date.Value.Month : 0;
        }

        /// <summary>
        /// Returns the month identificator from the specified date-time value.
        /// </summary>
        public static object MonthIdent(object value)
        {
            return Funcs.MonthIdentObject(value);
        }
        #endregion

        #region Hour
        /// <summary>
        /// Returns the hour portion from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Hour(DateTime date)
		{
			return date.Hour;
		}

        /// <summary>
        /// Returns the hour portion from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Hour(DateTimeOffset date)
        {
            return date.Hour;
        }

        /// <summary>
        /// Returns the hour portion from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Hour(DateTime? date)
		{
            return date.HasValue ? date.Value.Hour : 0;
        }        

        /// <summary>
        /// Returns the hour portion from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Hour(DateTimeOffset? date)
        {
            return date.HasValue ? date.Value.Hour : 0;
        }
        #endregion

        #region Minute
        /// <summary>
        /// Returns the minutes portion from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Minute(DateTime date)
		{
			return date.Minute;
		}

        /// <summary>
        /// Returns the minutes portion from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Minute(DateTimeOffset date)
        {
            return date.Minute;
        }

        /// <summary>
        /// Returns the minutes portion from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Minute(DateTime? date)
		{
            return date.HasValue ? date.Value.Minute : 0;
        }        

        /// <summary>
        /// Returns the minutes portion from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Minute(DateTimeOffset? date)
        {
            return date.HasValue ? date.Value.Minute : 0;
        }
        #endregion

        #region Second
        /// <summary>
        /// Returns the seconds portion from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Second(DateTime date)
		{
			return date.Second;
		}

        /// <summary>
        /// Returns the seconds portion from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Second(DateTimeOffset date)
        {
            return date.Second;
        }

        /// <summary>
        /// Returns the seconds portion from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Second(DateTime? date)
		{
            return date.HasValue ? date.Value.Second : 0;
        }        

        /// <summary>
        /// Returns the seconds portion from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Second(DateTimeOffset? date)
        {
            return date.HasValue ? date.Value.Second : 0;
        }
        #endregion

        #region Day
        /// <summary>
        /// Returns the day from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Day(DateTime date)
		{
			return date.Day;
		}

        /// <summary>
        /// Returns the day from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Day(DateTimeOffset date)
        {
            return date.Day;
        }

        /// <summary>
        /// Returns the day from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Day(DateTime? date)
		{
            return date.HasValue ? date.Value.Day : 0;
        }        

        /// <summary>
        /// Returns the day from a date and returns it as a integer value.
        /// </summary>
        public static Int64 Day(DateTimeOffset? date)
        {
            return date.HasValue ? date.Value.Day : 0;
        }
        #endregion

        #region DayOfWeek
        /// <summary>
        /// Returns the day of the week.
        /// </summary>
        public static string DayOfWeek(DateTime date)
		{
            return Func.DayOfWeekToStr.DayOfWeek(date);
		}

        /// <summary>
        /// Returns the day of the week.
        /// </summary>
        public static string DayOfWeek(DateTimeOffset date)
        {
            return Func.DayOfWeekToStr.DayOfWeek(date);
        }

        /// <summary>
        /// Returns the day of the week.
        /// </summary>
        public static string DayOfWeek(DateTime? date)
		{
			if (date.HasValue)
				return Func.DayOfWeekToStr.DayOfWeek(date.Value);
			else
				return string.Empty;
		}

        /// <summary>
        /// Returns the day of the week.
        /// </summary>
        public static string DayOfWeek(DateTimeOffset? date)
        {
            if (date.HasValue)
                return Func.DayOfWeekToStr.DayOfWeek(date.Value);
            else
                return string.Empty;
        }

        /// <summary>
        /// Returns the day of the week.
        /// </summary>
        public static string DayOfWeek(DateTime date, bool localized)
        {
            return Func.DayOfWeekToStr.DayOfWeek(date, localized);
        }

        /// <summary>
        /// Returns the day of the week.
        /// </summary>
        public static string DayOfWeek(DateTimeOffset date, bool localized)
        {
            return Func.DayOfWeekToStr.DayOfWeek(date, localized);
        }

        /// <summary>
        /// Returns the day of the week.
        /// </summary>
        public static string DayOfWeek(DateTime? date, bool localized)
        {
            if (date.HasValue)
                return Func.DayOfWeekToStr.DayOfWeek(date.Value, localized);
            else
                return string.Empty;
        }

        /// <summary>
        /// Returns the day of the week.
        /// </summary>
        public static string DayOfWeek(DateTimeOffset? date, bool localized)
        {
            if (date.HasValue)
                return Func.DayOfWeekToStr.DayOfWeek(date.Value, localized);
            else
                return string.Empty;
        }

        /// <summary>
        /// Returns the day of the week.
        /// </summary>
        public static string DayOfWeek(DateTime date, string culture)
        {
            return Func.DayOfWeekToStr.DayOfWeek(date, culture);
        }

        /// <summary>
        /// Returns the day of the week.
        /// </summary>
        public static string DayOfWeek(DateTimeOffset date, string culture)
        {
            return Func.DayOfWeekToStr.DayOfWeek(date, culture);
        }

        /// <summary>
        /// Returns the day of the week.
        /// </summary>
        public static string DayOfWeek(DateTime? date, string culture)
        {
            if (date.HasValue)
                return Func.DayOfWeekToStr.DayOfWeek(date.Value, culture);
            else
                return string.Empty;
        }

        /// <summary>
        /// Returns the day of the week.
        /// </summary>
        public static string DayOfWeek(DateTimeOffset? date, string culture)
        {
            if (date.HasValue)
                return Func.DayOfWeekToStr.DayOfWeek(date.Value, culture);
            else
                return string.Empty;
        }

        /// <summary>
        /// Returns the day of the week.
        /// </summary>
        public static string DayOfWeek(DateTime date, string culture, bool upperCase)
        {
            return Func.DayOfWeekToStr.DayOfWeek(date, culture, upperCase);
        }

        /// <summary>
        /// Returns the day of the week.
        /// </summary>
        public static string DayOfWeek(DateTimeOffset date, string culture, bool upperCase)
        {
            return Func.DayOfWeekToStr.DayOfWeek(date, culture, upperCase);
        }

        /// <summary>
        /// Returns the day of the week.
        /// </summary>
        public static string DayOfWeek(DateTime? date, string culture, bool upperCase)
        {
            if (date.HasValue)
                return Func.DayOfWeekToStr.DayOfWeek(date.Value, culture, upperCase);
            else
                return string.Empty;
        } 

        /// <summary>
        /// Returns the day of the week.
        /// </summary>
        public static string DayOfWeek(DateTimeOffset? date, string culture, bool upperCase)
        {
            if (date.HasValue)
                return Func.DayOfWeekToStr.DayOfWeek(date.Value, culture, upperCase);
            else
                return string.Empty;
        }
        #endregion

        #region MonthName
        /// <summary>
        /// Returns the name of the month.
        /// </summary>
        public static string MonthName(DateTime date)
        {
            return Func.MonthToStr.MonthName(date);
        }

        /// <summary>
        /// Returns the name of the month.
        /// </summary>
        public static string MonthName(DateTime? date)
        {
            if (date.HasValue)
                return Func.MonthToStr.MonthName(date.Value);
            else
                return string.Empty;
        }

        /// <summary>
        /// Returns the name of the month.
        /// </summary>
        public static string MonthName(DateTime date, bool localized)
        {
            return Func.MonthToStr.MonthName(date, localized);
        }

        /// <summary>
        /// Returns the name of the month.
        /// </summary>
        public static string MonthName(DateTime? date, bool localized)
        {
            if (date.HasValue)
                return Func.MonthToStr.MonthName(date.Value, localized);
            else
                return string.Empty;
        }

        /// <summary>
        /// Returns the name of the month.
        /// </summary>
        public static string MonthName(DateTime date, string culture)
        {
            return Func.MonthToStr.MonthName(date, culture);
        }

        /// <summary>
        /// Returns the name of the month.
        /// </summary>
        public static string MonthName(DateTime? date, string culture)
        {
            if (date.HasValue)
                return Func.MonthToStr.MonthName(date.Value, culture);
            else
                return string.Empty;
        }

        /// <summary>
        /// Returns the name of the month.
        /// </summary>
        public static string MonthName(DateTime date, string culture, bool upperCase)
        {
            return Func.MonthToStr.MonthName(date, culture, upperCase);
        }

        /// <summary>
        /// Returns the name of the month.
        /// </summary>
        public static string MonthName(DateTime? date, string culture, bool upperCase)
        {
            if (date.HasValue)
                return Func.MonthToStr.MonthName(date.Value, culture, upperCase);
            else
                return string.Empty;
        }
        #endregion

        #region DayOfYear
        /// <summary>
		/// Returns the day of the year.
		/// </summary>
		public static long DayOfYear(DateTime date)
		{
			return date.DayOfYear;
		}

        /// <summary>
		/// Returns the day of the year.
		/// </summary>
		public static long DayOfYear(DateTimeOffset date)
        {
            return date.DayOfYear;
        }

        /// <summary>
        /// Returns the day of the year.
        /// </summary>
        public static long DayOfYear(DateTime? date)
		{
            return date.HasValue ? date.Value.DayOfYear : 0;
        }

        /// <summary>
        /// Returns the day of the year.
        /// </summary>
        public static long DayOfYear(DateTimeOffset? date)
        {
            return date.HasValue ? date.Value.DayOfYear : 0;
        }
        #endregion

        #region DateSerial
        /// <summary>
        /// Returns the DateTime value for the specified year, month, and day.
        /// </summary>
        /// <param name="year">The year (1 through 9999).</param>
        /// <param name="month">The month (1 through 12).</param>
        /// <param name="day">The day (1 through the number of days in month).</param>
        /// <returns>DateTime value.</returns>
        public static DateTime DateSerial(long year, long month, long day)
		{
            return new DateTime((int)year, (int)month, (int)day);
		}

        /// <summary>
        /// Returns the DateTime value for the specified number of ticks.
        /// </summary>
        /// <param name="ticks">A number of ticks that have elapsed since January 1, 0001.</param>
        /// <returns>DateTime value.</returns>
        public static DateTime DateSerial(long ticks)
        {
            return new DateTime(ticks);
        }
        #endregion

        #region TimeSerial
        /// <summary>
        /// Returns the TimeValue value for a specified number of hours, minutes, and seconds.
        /// </summary>
        public static TimeSpan TimeSerial(long hours, long minutes, long seconds)
		{
            return new TimeSpan((int)hours, (int)minutes, (int)seconds);
		}
        #endregion

		#region DaysInMonth
		/// <summary>
		/// Returns the number of days in the specified month and year.
		/// </summary>
		public static long DaysInMonth(long year, long month)
		{
			return DateTime.DaysInMonth((int)year, (int)month);
		}

		/// <summary>
		/// Returns the number of days in the specified month and year.
		/// </summary>
		public static long DaysInMonth(DateTime date)
		{
			return DateTime.DaysInMonth(date.Year, date.Month);
		}

        /// <summary>
		/// Returns the number of days in the specified month and year.
		/// </summary>
		public static long DaysInMonth(DateTimeOffset date)
        {
            return DateTime.DaysInMonth(date.Year, date.Month);
        }

        /// <summary>
        /// Returns the number of days in the specified month and year.
        /// </summary>
        public static long DaysInMonth(DateTime? date)
		{
            return date.HasValue ? DateTime.DaysInMonth(date.Value.Year, date.Value.Month) : 0;
        }

        /// <summary>
        /// Returns the number of days in the specified month and year.
        /// </summary>
        public static long DaysInMonth(DateTimeOffset? date)
        {
            return date.HasValue ? DateTime.DaysInMonth(date.Value.Year, date.Value.Month) : 0;
        }
        #endregion

        #region DaysInYear
        /// <summary>
        /// Returns the number of days in the specified year.
        /// </summary>
        public static long DaysInYear(long year)
		{
			return DateTime.IsLeapYear((int)year) ? 366 : 365;
		}

		/// <summary>
		/// Returns the number of days in the specified year.
		/// </summary>
		public static long DaysInYear(DateTime date)
		{
			return DateTime.IsLeapYear(date.Year) ? 366 : 365;
		}

        /// <summary>
		/// Returns the number of days in the specified year.
		/// </summary>
		public static long DaysInYear(DateTimeOffset date)
        {
            return DateTime.IsLeapYear(date.Year) ? 366 : 365;
        }

        /// <summary>
        /// Returns the number of days in the specified year.
        /// </summary>
        public static long DaysInYear(DateTime? date)
		{
            return date.HasValue ? DateTime.IsLeapYear(date.Value.Year) ? 366 : 365 : 0;
        }

        /// <summary>
        /// Returns the number of days in the specified year.
        /// </summary>
        public static long DaysInYear(DateTimeOffset? date)
        {
            return date.HasValue ? DateTime.IsLeapYear(date.Value.Year) ? 366 : 365 : 0;
        }
        #endregion

        #region WeekOfYear
        /// <summary>
        /// Returns the week of the year that includes the date in the specified DateTime value.
        /// </summary>
        public static long WeekOfYear(DateTime date)
        {
            var format = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
            return format.Calendar.GetWeekOfYear(date, format.CalendarWeekRule, format.FirstDayOfWeek);
        }
        /// <summary>
        /// Returns the week of the year that includes the date in the specified DateTime value.
        /// </summary>
        public static long WeekOfYear(DateTime? date)
        {
            if (date == null) 
                return 0;

            var format = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
            return format.Calendar.GetWeekOfYear(date.Value, format.CalendarWeekRule, format.FirstDayOfWeek);
        }

        /// <summary>
        /// Returns the week of the year that includes the date in the specified DateTime value.
        /// </summary>
        public static long WeekOfYear(DateTime date, DayOfWeek firstDayOfWeek)
        {
            var format = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
            return format.Calendar.GetWeekOfYear(date, format.CalendarWeekRule, firstDayOfWeek);
        }
        /// <summary>
        /// Returns the week of the year that includes the date in the specified DateTime value.
        /// </summary>
        public static long WeekOfYear(DateTime? date, DayOfWeek firstDayOfWeek)
        {
            if (date == null) 
                return 0;

            var format = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
            return format.Calendar.GetWeekOfYear(date.Value, format.CalendarWeekRule, firstDayOfWeek);
        }

        /// <summary>
        /// Returns the week of the year that includes the date in the specified DateTime value.
        /// </summary>
        public static long WeekOfYear(DateTime date, DayOfWeek firstDayOfWeek, CalendarWeekRule calendarWeekRule)
        {
            var format = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
            return format.Calendar.GetWeekOfYear(date, calendarWeekRule, firstDayOfWeek);
        }
        /// <summary>
        /// Returns the week of the year that includes the date in the specified DateTime value.
        /// </summary>
        public static long WeekOfYear(DateTime? date, DayOfWeek firstDayOfWeek, CalendarWeekRule calendarWeekRule)
        {
            if (date == null) 
                return 0;

            var format = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
            return format.Calendar.GetWeekOfYear(date.Value, calendarWeekRule, firstDayOfWeek);
        }
        #endregion

        #region WeekOfMonth
        /// <summary>
        /// Returns the week of the month that includes the date in the specified DateTime value.
        /// </summary>
        public static long WeekOfMonth(DateTime date)
        {
            return WeekOfYear(date) - WeekOfYear(new DateTime(date.Year, date.Month, 1)) + 1;
        }
        /// <summary>
        /// Returns the week of the month that includes the date in the specified DateTime value.
        /// </summary>
        public static long WeekOfMonth(DateTime? date)
        {
            return date != null ? WeekOfMonth(date.Value) : 0;
        }

        /// <summary>
        /// Returns the week of the month that includes the date in the specified DateTime value.
        /// </summary>
        public static long WeekOfMonth(DateTime date, DayOfWeek firstDayOfWeek)
        {
            return WeekOfYear(date, firstDayOfWeek) - WeekOfYear(new DateTime(date.Year, date.Month, 1), firstDayOfWeek) + 1;
        }
        /// <summary>
        /// Returns the week of the month that includes the date in the specified DateTime value.
        /// </summary>
        public static long WeekOfMonth(DateTime? date, DayOfWeek firstDayOfWeek)
        {
            return date != null ? WeekOfMonth(date.Value, firstDayOfWeek) : 0;
        }

        /// <summary>
        /// Returns the week of the month that includes the date in the specified DateTime value.
        /// </summary>
        public static long WeekOfMonth(DateTime date, DayOfWeek firstDayOfWeek, CalendarWeekRule calendarWeekRule)
        {
            var weekBegin = WeekOfYear(new DateTime(date.Year, date.Month, 1), firstDayOfWeek, calendarWeekRule);
            var weekEnd = WeekOfYear(date, firstDayOfWeek, calendarWeekRule);
            if (date.Month == 1 && weekBegin > 10)
            {
                if (weekBegin <= weekEnd)
                    weekBegin = WeekOfYear(new DateTime(date.Year - 1, 12, 1), firstDayOfWeek, calendarWeekRule);
                
                else
                    weekBegin = 1;
            }
            return weekEnd - weekBegin + 1;
        }
        /// <summary>
        /// Returns the week of the month that includes the date in the specified DateTime value.
        /// </summary>
        public static long WeekOfMonth(DateTime? date, DayOfWeek firstDayOfWeek, CalendarWeekRule calendarWeekRule)
        {
            return date != null ? WeekOfMonth(date.Value, firstDayOfWeek, calendarWeekRule) : 0;
        }
        #endregion

        #region OLEDate
        /// <summary>
        /// Returns a DateTime equivalent to the specified OLE Automation Date.
        /// </summary>
        public static DateTime FromOADate(double value)
        {
            return DateTime.FromOADate(value);
        }

        /// <summary>
        /// Returns the equivalent OLE Automation date of specified datetime.
        /// </summary>
        public static double ToOADate(DateTime value)
        {
            return value.ToOADate();
        }
        #endregion

    }
}
