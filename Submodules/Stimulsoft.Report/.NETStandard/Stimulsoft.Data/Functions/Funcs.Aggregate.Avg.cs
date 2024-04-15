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
        public static decimal Avg(object value)
        {
            if (!ListExt.IsList(value))
                return StiValueHelper.TryToDecimal(value);

            var values = SkipNulls(ListExt.ToList(value));
            if (Count(values) == 0) return 0;

            return Sum(values) / Count(values);
        }

        public static decimal? AvgNulls(object value)
        {
            if (!ListExt.IsList(value))
                return IsNull(value) ? (decimal?)null : StiValueHelper.TryToDecimal(value);

            var values = ListExt.ToList(value);
            if (values == null || values.Count() == 0)
                return null;

            return Sum(values) / values.Count();
        }

        public static double AvgD(object value)
        {
            if (!ListExt.IsList(value))
                return StiValueHelper.TryToDouble(value);

            var values = SkipNulls(ListExt.ToList(value));
            if (Count(values) == 0) return 0;

            return SumD(values) / Count(values);
        }

        public static long AvgI(object value)
        {
            if (!ListExt.IsList(value))
                return StiValueHelper.TryToLong(value);

            var values = SkipNulls(ListExt.ToList(value));
            if (Count(values) == 0) return 0;

            return SumI(values) / Count(values);
        }

        public static DateTime? AvgDate(object value)
        {
            if (!ListExt.IsList(value))
                return StiValueHelper.TryToNullableDateTime(value);

            var dates = SkipNulls(ListExt.ToList(value))
                .Where(d => d != null && d is DateTime)
                .TryCastToDateTime();

            if (Count(dates) == 0)return null;

            return new DateTime(dates.Sum(d => d.Ticks) / Count(dates));
        }

        public static TimeSpan? AvgTime(object value)
        {
            if (!ListExt.IsList(value))
                return StiValueHelper.TryToNullableTimeSpan(value);

            var times = SkipNulls(ListExt.ToList(value))
                .Select(d => GetTimeSpan(d))
                .Where(d => d != null)
                .TryCastToTimeSpan();

            if (Count(times) == 0)
                return null;
            
            return TimeSpan.FromTicks(Convert.ToInt64(times.Average(d => d.Ticks)));
        }

        private static TimeSpan? GetTimeSpan(object value)
        {
            if (value is DateTime date)
                return date.TimeOfDay;

            else if (value is TimeSpan time)
                return time;

            else
                return null;
        }
    }
}