//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        27 February 2007
//********************************************************************************************
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Stimulsoft.Base.Helpers
{
    /// <summary>
    /// Miscellaneous and parsing methods for DateTime
    /// </summary>
    public static class DateTimeRoutines
    {
        #region enum DateTimeFormat
        /// <summary>
        /// Defines default date-time format.
        /// </summary>
        public enum DateTimeFormat
        {
            /// <summary>
            /// Month number goes before day number.
            /// </summary>
            USA_DATE,
            /// <summary>
            /// Day number goes before month number.
            /// </summary>
            UK_DATE
        }
        #endregion

        #region class ParsedDateTime
        /// <summary>
        /// Defines a substring where date-time was found and result of conversion
        /// </summary>
        public class ParsedDateTime
        {
            /// <summary>
            /// Index of first char of a date substring found in the string.
            /// </summary>
            public readonly int IndexOfDate;

            /// <summary>
            /// Length a date substring found in the string.
            /// </summary>
            public readonly int LengthOfDate;

            /// <summary>
            /// Index of first char of a time substring found in the string.
            /// </summary>
            public readonly int IndexOfTime;

            /// <summary>
            /// Length of a time substring found in the string.
            /// </summary>
            public readonly int LengthOfTime;

            /// <summary>
            /// DateTime found in the string.
            /// </summary>
            public readonly DateTime DateTime;

            /// <summary>
            /// True if a date was found within the string.
            /// </summary>
            public readonly bool IsDateFound;

            /// <summary>
            /// True if a time was found within the string.
            /// </summary>
            public readonly bool IsTimeFound;

            /// <summary>
            /// UTC offset if it was found within the string.
            /// </summary>
            public readonly TimeSpan UtcOffset;

            /// <summary>
            /// True if UTC offset was found in the string.
            /// </summary>
            public readonly bool IsUtcOffsetFound;

            /// <summary>
            /// Utc gotten from DateTime if IsUtcOffsetFound is True.
            /// </summary>
            public DateTime UtcDateTime;

            internal ParsedDateTime(int indexOfDate, int lengthOfDate, int indexOfTime, int lengthOfTime, DateTime dateTime)
            {
                IndexOfDate = indexOfDate;
                LengthOfDate = lengthOfDate;
                IndexOfTime = indexOfTime;
                LengthOfTime = lengthOfTime;
                DateTime = dateTime;
                IsDateFound = indexOfDate > -1;
                IsTimeFound = indexOfTime > -1;
                UtcOffset = new TimeSpan(25, 0, 0);
                IsUtcOffsetFound = false;
                UtcDateTime = new DateTime(1, 1, 1);
            }

            internal ParsedDateTime(int indexOfDate, int lengthOfDate, int indexOfTime, int lengthOfTime, DateTime dateTime, TimeSpan utcOffset)
            {
                IndexOfDate = indexOfDate;
                LengthOfDate = lengthOfDate;
                IndexOfTime = indexOfTime;
                LengthOfTime = lengthOfTime;
                DateTime = dateTime;
                IsDateFound = indexOfDate > -1;
                IsTimeFound = indexOfTime > -1;
                UtcOffset = utcOffset;
                IsUtcOffsetFound = Math.Abs(utcOffset.TotalHours) < 12;

                if (!IsUtcOffsetFound)
                {
                    UtcDateTime = new DateTime(1, 1, 1);
                }
                else
                {
                    if (indexOfDate < 0)//To avoid negative date exception when date is undefined
                    {
                        var ts = dateTime.TimeOfDay + utcOffset;
                        UtcDateTime = ts < new TimeSpan(0) 
                            ? new DateTime(1, 1, 2) + ts 
                            : new DateTime(1, 1, 1) + ts;
                    }
                    else
                    {
                        UtcDateTime = dateTime + utcOffset;
                    }
                }
            }
        }
        #endregion

        #region Properties.Static
        private static DateTime defaultDate = DateTime.Now;
        /// <summary>
        /// Date that is accepted in the following cases:
        /// - no date was parsed by TryParseDateOrTime();
        /// - no year was found by TryParseDate();
        /// It is ignored if DefaultDateIsNow = true was set after DefaultDate 
        /// </summary>
        public static DateTime DefaultDate
        {
            set
            {
                defaultDate = value;
                DefaultDateIsNow = false;
            }
            get
            {
                return DefaultDateIsNow ? DateTime.Now : defaultDate;
            }
        }

        /// <summary>
        /// If true then DefaultDate property is ignored and DefaultDate is always DateTime.Now
        /// </summary>
        public static bool DefaultDateIsNow { get; set; } = true;
        #endregion

        #region Methods
        /// <summary>
        /// Tries to find date and time within the passed string and return it as DateTime structure. 
        /// </summary>
        /// <param name="str">string that contains date and/or time</param>
        /// <param name="defaultFormat">format to be used preferably in ambivalent instances</param>
        /// <param name="dateTime">parsed date-time output</param>
        /// <returns>true if both date and time were found, else false</returns>
        public static bool TryParseDateTime(string str, DateTimeFormat defaultFormat, out DateTime dateTime)
        {
            ParsedDateTime parsedDateTime;
            if (TryParseDateTime(str, defaultFormat, out parsedDateTime))
            {
                dateTime = parsedDateTime.DateTime;
                return true;
            }

            dateTime = new DateTime(1, 1, 1);
            return false;
        }

        /// <summary>
        /// Tries to find date and/or time within the passed string and return it as DateTime structure. 
        /// If only date was found, time in the returned DateTime is always 0:0:0.
        /// If only time was found, date in the returned DateTime is DefaultDate.
        /// </summary>
        /// <param name="str">string that contains date and(or) time</param>
        /// <param name="defaultFormat">format to be used preferably in ambivalent instances</param>
        /// <param name="dateTime">parsed date-time output</param>
        /// <returns>true if date and/or time was found, else false</returns>
        public static bool TryParseDateOrTime(string str, DateTimeFormat defaultFormat, out DateTime dateTime)
        {
            ParsedDateTime parsedDateTime;
            if (TryParseDateOrTime(str, defaultFormat, out parsedDateTime))
            {
                dateTime = parsedDateTime.DateTime;
                return true;
            }

            dateTime = new DateTime(1, 1, 1);
            return false;
        }

        /// <summary>
        /// Tries to find time within the passed string and return it as DateTime structure. 
        /// It recognizes only time while ignoring date, so date in the returned DateTime is always 1/1/1.
        /// </summary>
        /// <param name="str">string that contains time</param>
        /// <param name="defaultFormat">format to be used preferably in ambivalent instances</param>
        /// <param name="time">parsed time output</param>
        /// <returns>true if time was found, else false</returns>
        public static bool TryParseTime(string str, DateTimeFormat defaultFormat, out DateTime time)
        {
            ParsedDateTime parsedTime;
            if (TryParseTime(str, defaultFormat, out parsedTime, null))
            {
                time = parsedTime.DateTime;
                return true;
            }

            time = new DateTime(1, 1, 1);
            return false;
        }

        /// <summary>
        /// Tries to find date within the passed string and return it as DateTime structure. 
        /// It recognizes only date while ignoring time, so time in the returned DateTime is always 0:0:0.
        /// If year of the date was not found then it accepts the current year. 
        /// </summary>
        /// <param name="str">string that contains date</param>
        /// <param name="defaultFormat">format to be used preferably in ambivalent instances</param>
        /// <param name="date">parsed date output</param>
        /// <returns>true if date was found, else false</returns>
        public static bool TryParseDate(string str, DateTimeFormat defaultFormat, out DateTime date)
        {
            ParsedDateTime parsedDate;
            if (TryParseDate(str, defaultFormat, out parsedDate))
            {
                date = parsedDate.DateTime;
                return true;
            }

            date = new DateTime(1, 1, 1);
            return false;
        }

        /// <summary>
        /// Tries to find date and time within the passed string and return it as ParsedDateTime object. 
        /// </summary>
        /// <param name="str">string that contains date-time</param>
        /// <param name="defaultFormat">format to be used preferably in ambivalent instances</param>
        /// <param name="parsedDateTime">parsed date-time output</param>
        /// <returns>true if both date and time were found, else false</returns>
        public static bool TryParseDateTime(string str, DateTimeFormat defaultFormat, out ParsedDateTime parsedDateTime)
        {
            if (TryParseDateOrTime(str, defaultFormat, out parsedDateTime))
                return true;

            parsedDateTime = null;
            return false;
        }

        /// <summary>
        /// Tries to find time within the passed string and return it as ParsedDateTime object. 
        /// It recognizes only time while ignoring date, so date in the returned ParsedDateTime is always 1/1/1
        /// </summary>
        /// <param name="str">string that contains date-time</param>
        /// <param name="defaultFormat">format to be used preferably in ambivalent instances</param>
        /// <param name="parsedTime">parsed date-time output</param>
        /// <returns>true if time was found, else false</returns>
        public static bool TryParseTime(string str, DateTimeFormat defaultFormat, out ParsedDateTime parsedTime)
        {
            return TryParseTime(str, defaultFormat, out parsedTime, null);
        }

        /// <summary>
        /// Tries to find date and/or time within the passed string and return it as ParsedDateTime object. 
        /// If only date was found, time in the returned ParsedDateTime is always 0:0:0.
        /// If only time was found, date in the returned ParsedDateTime is DefaultDate.
        /// </summary>
        /// <param name="str">string that contains date-time</param>
        /// <param name="defaultFormat">format to be used preferably in ambivalent instances</param>
        /// <param name="parsedDateTime">parsed date-time output</param>
        /// <returns>true if date or time was found, else false</returns>
        public static bool TryParseDateOrTime(string str, DateTimeFormat defaultFormat, out ParsedDateTime parsedDateTime)
        {
            parsedDateTime = null;

            ParsedDateTime parsedDate;
            ParsedDateTime parsedTime;
            if (TryParseDate(str, defaultFormat, out parsedDate))
            {
                if (!TryParseTime(str, defaultFormat, out parsedTime, parsedDate))
                {
                    var dateTime = new DateTime(parsedDate.DateTime.Year, parsedDate.DateTime.Month, parsedDate.DateTime.Day, 0, 0, 0);
                    parsedDateTime = new ParsedDateTime(parsedDate.IndexOfDate, parsedDate.LengthOfDate, -1, -1, dateTime);
                }
                else
                {
                    var dateTime = new DateTime(parsedDate.DateTime.Year, parsedDate.DateTime.Month, parsedDate.DateTime.Day, parsedTime.DateTime.Hour, parsedTime.DateTime.Minute, parsedTime.DateTime.Second);
                    parsedDateTime = new ParsedDateTime(parsedDate.IndexOfDate, parsedDate.LengthOfDate, parsedTime.IndexOfTime, parsedTime.LengthOfTime, dateTime, parsedTime.UtcOffset);
                }
            }
            else
            {
                if (!TryParseTime(str, defaultFormat, out parsedTime, null))
                    return false;

                var dateTime = new DateTime(DefaultDate.Year, DefaultDate.Month, DefaultDate.Day, parsedTime.DateTime.Hour, parsedTime.DateTime.Minute, parsedTime.DateTime.Second);
                parsedDateTime = new ParsedDateTime(-1, -1, parsedTime.IndexOfTime, parsedTime.LengthOfTime, dateTime, parsedTime.UtcOffset);
            }

            return true;
        }

        /// <summary>
        /// Tries to find time within the passed string (relatively to the passed parsedDate if any) and return it as ParsedDateTime object.
        /// It recognizes only time while ignoring date, so date in the returned ParsedDateTime is always 1/1/1
        /// </summary>
        /// <param name="str">string that contains date</param>
        /// <param name="defaultFormat">format to be used preferably in ambivalent instances</param>
        /// <param name="parsedTime">parsed date-time output</param>
        /// <param name="parsedDate">ParsedDateTime object if the date was found within this string, else NULL</param>
        /// <returns>true if time was found, else false</returns>
        public static bool TryParseTime(string str, DateTimeFormat defaultFormat, out ParsedDateTime parsedTime, ParsedDateTime parsedDate)
        {
            parsedTime = null;

            var time_zone_r = defaultFormat == DateTimeFormat.USA_DATE
                ? @"(?:\s*(?'time_zone'UTC|GMT|CST|EST))?"
                : @"(?:\s*(?'time_zone'UTC|GMT))?";

            Match m;
            if (parsedDate != null && parsedDate.IndexOfDate > -1)
            {
                //look around the found date
                //look for <date> hh:mm:ss <UTC offset> 
                m = Regex.Match(str.Substring(parsedDate.IndexOfDate + parsedDate.LengthOfDate), @"(?<=^\s*,?\s+|^\s*at\s*|^\s*[T\-]\s*)(?'hour'\d{2})\s*:\s*(?'minute'\d{2})\s*:\s*(?'second'\d{2})\s+(?'offset_sign'[\+\-])(?'offset_hh'\d{2}):?(?'offset_mm'\d{2})(?=$|[^\d\w])", RegexOptions.Compiled);
                if (!m.Success)//look for <date> [h]h:mm[:ss] [PM/AM] [UTC/GMT] 
                    m = Regex.Match(str.Substring(parsedDate.IndexOfDate + parsedDate.LengthOfDate), @"(?<=^\s*,?\s+|^\s*at\s*|^\s*[T\-]\s*)(?'hour'\d{1,2})\s*:\s*(?'minute'\d{2})\s*(?::\s*(?'second'\d{2}))?(?:\s*(?'ampm'AM|am|PM|pm))?" + time_zone_r + @"(?=$|[^\d\w])", RegexOptions.Compiled);

                if (!m.Success)//look for [h]h:mm:ss [PM/AM] [UTC/GMT] <date>
                    m = Regex.Match(str.Substring(0, parsedDate.IndexOfDate), @"(?<=^|[^\d])(?'hour'\d{1,2})\s*:\s*(?'minute'\d{2})\s*(?::\s*(?'second'\d{2}))?(?:\s*(?'ampm'AM|am|PM|pm))?" + time_zone_r + @"(?=$|[\s,]+)", RegexOptions.Compiled);

                if (!m.Success)//look for [h]h:mm:ss [PM/AM] [UTC/GMT] within <date>
                    m = Regex.Match(str.Substring(parsedDate.IndexOfDate, parsedDate.LengthOfDate), @"(?<=^|[^\d])(?'hour'\d{1,2})\s*:\s*(?'minute'\d{2})\s*(?::\s*(?'second'\d{2}))?(?:\s*(?'ampm'AM|am|PM|pm))?" + time_zone_r + @"(?=$|[\s,]+)", RegexOptions.Compiled);
            }
            else//look anywhere within string
            {
                //look for hh:mm:ss <UTC offset> 
                m = Regex.Match(str, @"(?<=^|\s+|\s*T\s*)(?'hour'\d{2})\s*:\s*(?'minute'\d{2})\s*:\s*(?'second'\d{2})\s+(?'offset_sign'[\+\-])(?'offset_hh'\d{2}):?(?'offset_mm'\d{2})?(?=$|[^\d\w])", RegexOptions.Compiled);
                if (!m.Success)//look for [h]h:mm[:ss] [PM/AM] [UTC/GMT]
                    m = Regex.Match(str, @"(?<=^|\s+|\s*T\s*)(?'hour'\d{1,2})\s*:\s*(?'minute'\d{2})\s*(?::\s*(?'second'\d{2}))?(?:\s*(?'ampm'AM|am|PM|pm))?" + time_zone_r + @"(?=$|[^\d\w])", RegexOptions.Compiled);
            }

            if (!m.Success)
                return false;

            var hour = int.Parse(m.Groups["hour"].Value);
            if (hour < 0 || hour > 23)
                return false;

            var minute = int.Parse(m.Groups["minute"].Value);
            if (minute < 0 || minute > 59)
                return false;

            var second = 0;
            if (!string.IsNullOrEmpty(m.Groups["second"].Value))
            {
                second = int.Parse(m.Groups["second"].Value);
                if (second < 0 || second > 59)
                    return false;
            }

            if (string.Compare(m.Groups["ampm"].Value, "PM", StringComparison.OrdinalIgnoreCase) == 0 && hour < 12)
                hour += 12;

            else if (string.Compare(m.Groups["ampm"].Value, "AM", StringComparison.OrdinalIgnoreCase) == 0 && hour == 12)
                hour -= 12;

            var dateTime = new DateTime(1, 1, 1, hour, minute, second);
            if (m.Groups["offset_hh"].Success)
            {
                var offsetHh = int.Parse(m.Groups["offset_hh"].Value);
                var offsetMm = 0;
                if (m.Groups["offset_mm"].Success)
                    offsetMm = int.Parse(m.Groups["offset_mm"].Value);

                var utcOffset = new TimeSpan(offsetHh, offsetMm, 0);
                if (m.Groups["offset_sign"].Value == "-")
                    utcOffset = -utcOffset;

                parsedTime = new ParsedDateTime(-1, -1, m.Index, m.Length, dateTime, utcOffset);
                return true;
            }

            if (m.Groups["time_zone"].Success)
            {
                TimeSpan utcOffset;
                switch (m.Groups["time_zone"].Value)
                {
                    case "UTC":
                    case "GMT":
                        utcOffset = new TimeSpan(0, 0, 0);
                        break;

                    case "CST":
                        utcOffset = new TimeSpan(-6, 0, 0);
                        break;

                    case "EST":
                        utcOffset = new TimeSpan(-5, 0, 0);
                        break;

                    default:
                        throw new Exception("Time zone: " + m.Groups["time_zone"].Value + " is not defined.");
                }
                parsedTime = new ParsedDateTime(-1, -1, m.Index, m.Length, dateTime, utcOffset);
                return true;
            }

            parsedTime = new ParsedDateTime(-1, -1, m.Index, m.Length, dateTime);
            return true;
        }

        /// <summary>
        /// Tries to find date within the passed string and return it as ParsedDateTime object. 
        /// It recognizes only date while ignoring time, so time in the returned ParsedDateTime is always 0:0:0.
        /// If year of the date was not found then it accepts the current year. 
        /// </summary>
        /// <param name="str">string that contains date</param>
        /// <param name="defaultFormat">format to be used preferably in ambivalent instances</param>
        /// <param name="parsedDate">parsed date output</param>
        /// <returns>true if date was found, else false</returns>
        public static bool TryParseDate(string str, DateTimeFormat defaultFormat, out ParsedDateTime parsedDate)
        {
            parsedDate = null;

            if (string.IsNullOrEmpty(str))
                return false;

            //look for dd/mm/yy
            var m = Regex.Match(str, @"(?<=^|[^\d])(?'day'\d{1,2})\s*(?'separator'[\\/\.])+\s*(?'month'\d{1,2})\s*\'separator'+\s*(?'year'\d{2}|\d{4})(?=$|[^\d])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (m.Success)
            {
                DateTime date;
                if ((defaultFormat ^ DateTimeFormat.USA_DATE) == DateTimeFormat.USA_DATE)
                {
                    if (!ConvertToDate(int.Parse(m.Groups["year"].Value), int.Parse(m.Groups["day"].Value), int.Parse(m.Groups["month"].Value), out date))
                        return false;
                }
                else
                {
                    if (!ConvertToDate(int.Parse(m.Groups["year"].Value), int.Parse(m.Groups["month"].Value), int.Parse(m.Groups["day"].Value), out date))
                        return false;
                }
                parsedDate = new ParsedDateTime(m.Index, m.Length, -1, -1, date);
                return true;
            }

            //look for [yy]yy-mm-dd
            m = Regex.Match(str, @"(?<=^|[^\d])(?'year'\d{2}|\d{4})\s*(?'separator'[\-])\s*(?'month'\d{1,2})\s*\'separator'+\s*(?'day'\d{1,2})(?=$|[^\d])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (m.Success)
            {
                DateTime date;
                if (!ConvertToDate(int.Parse(m.Groups["year"].Value), int.Parse(m.Groups["month"].Value), int.Parse(m.Groups["day"].Value), out date))
                    return false;

                parsedDate = new ParsedDateTime(m.Index, m.Length, -1, -1, date);
                return true;
            }

            var monthList = Thread.CurrentThread.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames.ToList()
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n.Length > 3 ? n.Substring(0, 3) : n)
                .Select(n => n.Substring(0, 1).ToUpperInvariant() + n.Substring(1, n.Length - 1).ToLowerInvariant())
                .ToArray();

            if (TryParseDateInternal(str, ref parsedDate, monthList))
                return true;

            monthList = Thread.CurrentThread.CurrentCulture.DateTimeFormat.MonthNames.ToList()
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n.Substring(0, 1).ToUpperInvariant() + n.Substring(1, n.Length - 1).ToLowerInvariant())
                .ToArray();

            return TryParseDateInternal(str, ref parsedDate, monthList);
        }

        private static bool TryParseDateInternal(string str, ref ParsedDateTime parsedDate, string[] monthList)
        {
            var months = string.Join("|", monthList);

            //look for month dd yyyy
            var m = Regex.Match(str, @"(?:^|[^\d\w])(?'month'" + months + @")[uarychilestmbro]*\s+(?'day'\d{1,2})(?:-?st|-?th|-?rd|-?nd)?\s*,?\s*(?'year'\d{4})(?=$|[^\d\w])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (!m.Success) //look for dd month [yy]yy
                m = Regex.Match(str, @"(?:^|[^\d\w:])(?'day'\d{1,2})(?:-?st\s+|-?th\s+|-?rd\s+|-?nd\s+|-|\s+)(?'month'" + months + @")[uarychilestmbro]*(?:\s*,?\s*|-)'?(?'year'\d{2}|\d{4})(?=$|[^\d\w])", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (!m.Success) //look for yyyy month dd
                m = Regex.Match(str, @"(?:^|[^\d\w])(?'year'\d{4})\s+(?'month'" + months + @")[uarychilestmbro]*\s+(?'day'\d{1,2})(?:-?st|-?th|-?rd|-?nd)?(?=$|[^\d\w])", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (!m.Success) //look for month dd hh:mm:ss MDT|UTC yyyy
                m = Regex.Match(str, @"(?:^|[^\d\w])(?'month'" + months + @")[uarychilestmbro]*\s+(?'day'\d{1,2})\s+\d{2}\:\d{2}\:\d{2}\s+(?:MDT|UTC)\s+(?'year'\d{4})(?=$|[^\d\w])", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (!m.Success) //look for  month dd [yyyy]
                m = Regex.Match(str, @"(?:^|[^\d\w])(?'month'" + months + @")[uarychilestmbro]*\s+(?'day'\d{1,2})(?:-?st|-?th|-?rd|-?nd)?(?:\s*,?\s*(?'year'\d{4}))?(?=$|[^\d\w])", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (m.Success)
            {
                var indexOfDate = m.Index;
                var lengthOfDate = m.Length;
                var monthStr = m.Groups["month"].Value.ToLowerInvariant();
                var monthValue = monthList.FirstOrDefault(mm => mm.ToLowerInvariant() == monthStr);
                if (monthValue == null)
                    return false;

                var month = Array.IndexOf(monthList, monthValue) + 1;

                var year = !string.IsNullOrEmpty(m.Groups["year"].Value)
                    ? int.Parse(m.Groups["year"].Value)
                    : DefaultDate.Year;

                DateTime date;
                if (!ConvertToDate(year, month, int.Parse(m.Groups["day"].Value), out date))
                    return false;

                parsedDate = new ParsedDateTime(indexOfDate, lengthOfDate, -1, -1, date);
                return true;
            }

            return false;
        }

        private static bool ConvertToDate(int year, int month, int day, out DateTime date)
        {
            if (year >= 100)
            {
                if (year < 1000)
                {
                    date = new DateTime(1, 1, 1);
                    return false;
                }
            }
            else if (year > 30)
                year += 1900;

            else
                year += 2000;

            try
            {
                if (month < 1 || month > 12 || day < 1 || day > 31)
                {
                    date = new DateTime(1, 1, 1);
                    return false;
                }

                date = new DateTime(year, month, day);
            }
            catch
            {
                date = new DateTime(1, 1, 1);
                return false;
            }
            return true;
        }
        #endregion
    }
}