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
using System.Data;
using System.Linq;
using System.Threading;

namespace Stimulsoft.Data.Extensions
{
    public static class DataTimeExt
    {
        public static DateTime FirstDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        public static int DaysInMonth(this DateTime value)
        {
            return DateTime.DaysInMonth(value.Year, value.Month);
        }

        public static DateTime LastDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.DaysInMonth());
        }

        public static DateTime FirstDayOfQuarter(this DateTime value)
        {
            return AddQuarters(new DateTime(value.Year, 1, 1), GetQuarter(value) - 1);
        }

        public static DateTime LastDayOfQuarter(this DateTime value)
        {
            return AddQuarters(new DateTime(value.Year, 1, 1), GetQuarter(value)).AddDays(-1);
        }

        public static DateTime LastDateTimeOfDay(this DateTime dateTime)
        {
            return dateTime.Date.AddDays(1).AddTicks(-1);
        }

        public static DateTime AddQuarters(this DateTime value, int quarters)
        {
            return value.AddMonths(quarters * 3);
        }
        
        public static int GetQuarter(this DateTime value)
        {
            var month = value.Month - 1;
            return Math.Abs(month / 3) + 1;
        }

        public static DateTime FirstDayOfWeek(this DateTime value)
        {
            var culture = Thread.CurrentThread.CurrentCulture;
            var diff = value.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
            if (diff < 0)
                diff += 7;

            return value.AddDays(-diff).Date;
        }

        public static DateTime LastDayOfWeek(this DateTime value)
        {
            return value.FirstDayOfWeek().AddDays(6);
        }

        public static DateTime FirstDayOfYear(this DateTime value)
        {
            return new DateTime(value.Year, 1, 1);
        }

        public static DateTime LastDayOfYear(this DateTime value)
        {
            return new DateTime(value.Year, 12, 31);
        }

        public static DateTime FirstDayOfFirthQuarter(this DateTime value)
        {
            return FirstDayOfQuarter(new DateTime(value.Year, 1, 1));
        }

        public static DateTime LastDayOfFirthQuarter(this DateTime value)
        {
            return LastDayOfQuarter(new DateTime(value.Year, 3, 31));
        }

        public static DateTime FirstDayOfSecondQuarter(this DateTime value)
        {
            return FirstDayOfQuarter(new DateTime(value.Year, 4, 1));
        }

        public static DateTime LastDayOfSecondQuarter(this DateTime value)
        {
            return LastDayOfQuarter(new DateTime(value.Year, 6, 30));
        }

        public static DateTime FirstDayOfThirdQuarter(this DateTime value)
        {
            return FirstDayOfQuarter(new DateTime(value.Year, 7, 1));
        }

        public static DateTime LastDayOfThirdQuarter(this DateTime value)
        {
            return LastDayOfQuarter(new DateTime(value.Year, 9, 30));
        }

        public static DateTime FirstDayOfFourthQuarter(this DateTime value)
        {
            return FirstDayOfQuarter(new DateTime(value.Year, 10, 1));
        }

        public static DateTime LastDayOfFourthQuarter(this DateTime value)
        {
            return LastDayOfQuarter(new DateTime(value.Year, 12, 31));
        }
    }
}