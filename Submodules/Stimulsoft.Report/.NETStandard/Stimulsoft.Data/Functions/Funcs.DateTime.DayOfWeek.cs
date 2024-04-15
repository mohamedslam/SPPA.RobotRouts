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

using Stimulsoft.Base.Helpers;
using Stimulsoft.Data.Extensions;
using System;
using System.Linq;

namespace Stimulsoft.Data.Functions
{
    public partial class Funcs
    {
        public static DayOfWeek? DayOfWeekIdent(DateTime? dateTime)
        {
            if (dateTime == null) 
                return null;

            return (DayOfWeek)Enum.ToObject(typeof(DayOfWeek), dateTime.Value.DayOfWeek);
        }

        public static object DayOfWeekIdentObject(object value)
        {
            if (ListExt.IsList(value))
            {
                return ListExt.ToList(value).Select(DayOfWeekIdentObject);
            }
            else
            {
                if (value is string)
                    return StiDayOfWeekToStrHelper.DayOfWeek(value as string);
                else
                    return DayOfWeekIdent(StiValueHelper.TryToNullableDateTime(value));
            }
        }

        public static int DayOfWeekIndex(DateTime? dateTime)
        {
            if (dateTime == null) 
                return -1;

            return (int)dateTime.Value.DayOfWeek;
        }

        public static object DayOfWeekIndexObject(object value)
        {
            if (ListExt.IsList(value))
            {
                return ListExt.ToList(value).Select(DayOfWeekIndexObject);
            }
            else
            {
                if (value is string)
                {
                    var day = StiDayOfWeekToStrHelper.DayOfWeek(value as string);
                    if (day == null)
                        return -1;
                    else
                        return (int)day;
                }
                else
                    return DayOfWeekIndex(StiValueHelper.TryToNullableDateTime(value));
            }
        }

        public static string DayOfWeek(DateTime? date)
        {
            return date.HasValue 
                ? StiDayOfWeekToStrHelper.DayOfWeek(date.Value) 
                : string.Empty;
        }

        public static object DayOfWeekObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(DayOfWeek);
            else
                return DayOfWeek(StiValueHelper.TryToNullableDateTime(value));
        }

        public static string DayOfWeek(DateTime? date, bool localized)
        {
            return date.HasValue 
                ? StiDayOfWeekToStrHelper.DayOfWeek(date.Value, localized) 
                : string.Empty;
        }

        public static object DayOfWeekObject(object value, bool localized)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(d => DayOfWeek(d, localized));
            else
                return DayOfWeek(StiValueHelper.TryToNullableDateTime(value), localized);
        }

        public static string DayOfWeek(DateTime? date, string culture)
        {
            return date.HasValue 
                ? StiDayOfWeekToStrHelper.DayOfWeek(date.Value, culture) 
                : string.Empty;
        }

        public static object DayOfWeekObject(object value, string culture)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(d => DayOfWeek(d, culture));
            else
                return DayOfWeek(StiValueHelper.TryToNullableDateTime(value), culture);
        }

        public static string DayOfWeek(DateTime? date, string culture, bool upperCase)
        {
            return date.HasValue 
                ? StiDayOfWeekToStrHelper.DayOfWeek(date.Value, culture, upperCase) 
                : string.Empty;
        }

        public static object DayOfWeekObject(object value, string culture, bool upperCase)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(d => DayOfWeek(d, culture, upperCase));
            else
                return DayOfWeek(StiValueHelper.TryToNullableDateTime(value), culture, upperCase);
        }
    }
}