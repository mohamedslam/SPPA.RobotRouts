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
using System.Text.RegularExpressions;
using System.Threading;

using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Json;

namespace Stimulsoft.Base
{
    public static class StringExt
    {
        public static bool TryParseDateTime(this string value, out DateTime dateTime)
        {
            dateTime = DateTime.Now;

            if (string.IsNullOrWhiteSpace(value))
                return false;

            if (value.TryParseJsonDateTime(out dateTime))
                return true;

            if (DateTime.TryParse(value, out dateTime))
                return true;

            var format = Thread.CurrentThread.CurrentCulture.Name == "en-US"
                ? DateTimeRoutines.DateTimeFormat.USA_DATE
                : DateTimeRoutines.DateTimeFormat.UK_DATE;

            if (DateTimeRoutines.TryParseDateTime(value, format, out dateTime))
                return true;

            return false;
        }

        private static bool TryParseJsonDateTime(this string value, out DateTime dateTime)
        {
            dateTime = DateTime.Now;

            try
            {
                if (string.IsNullOrWhiteSpace(value))
                    return false;

                if (!value.Replace(" ", "").Contains("Date("))
                    return false;

                if (value.TryParseJsonDateTimeInNewDate(out dateTime))
                    return true;

                if (value.StartsWithInvariant("Date("))
                    value = $"\"\\/{value}\\/\"";

                else if (value.StartsWithInvariant("/Date("))
                    value = $"\"\\{value}/\"";

                else if (value.StartsWithInvariant("\\/Date("))
                    value = $"\"{value}\"";

                var dateJson = JsonConvert.DeserializeObject<DateTime?>(value);
                if (dateJson == null)
                    return false;

                dateTime = dateJson.Value;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool TryParseJsonDateTimeInNewDate(this string value, out DateTime dateTime)
        {
            dateTime = DateTime.Now;

            try
            {
                var solidValue = value.Replace(" ", "").ToLowerInvariant();
                if (!solidValue.StartsWithInvariantIgnoreCase("newdate("))
                    return false;

                var startIndex = solidValue.IndexOf("(", StringComparison.Ordinal) + 1;
                if (startIndex == -1)
                    return false;

                var endIndex = solidValue.IndexOf(")", startIndex, StringComparison.Ordinal);
                if (endIndex == -1)
                    return false;

                solidValue = solidValue.Substring(startIndex, endIndex - startIndex);

                long ticks;
                if (!long.TryParse(solidValue, out ticks))
                    return false;

                dateTime = new DateTime(1970, 1, 1).AddTicks(ticks * 10000);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string ToLowerNull(string str)
        {
            return string.IsNullOrWhiteSpace(str) ? str : str.ToLowerInvariant();
        }

        public static bool IsBase64String(this string s)
        {
            s = s.Trim().Replace("\r", string.Empty).Replace("\n", string.Empty);
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        public static bool StartsWithInvariantIgnoreCase(this string str, string check)
        {
            return str.StartsWith(check, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool EndsWithInvariantIgnoreCase(this string str, string check)
	    {
            return str.EndsWith(check, StringComparison.InvariantCultureIgnoreCase);
	    }

        public static bool StartsWithInvariant(this string str, string check)
        {
            return str.StartsWith(check, StringComparison.InvariantCulture);
        }

        public static bool EndsWithInvariant(this string str, string check)
        {
            return str.EndsWith(check, StringComparison.InvariantCulture);
        }

        public static int IndexOfInvariant(this string str, string value)
        {
            return str.IndexOf(value, StringComparison.InvariantCulture);
        }

        public static int IndexOfInvariant(this string str, string value, int startIndex)
        {
            return str.IndexOf(value, startIndex, StringComparison.InvariantCulture);
        }

        public static string ToEmpty(this string str)
        {
            return str ?? string.Empty;
        }
    }
}
