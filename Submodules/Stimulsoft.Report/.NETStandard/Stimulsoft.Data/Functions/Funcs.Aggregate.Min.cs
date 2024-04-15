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
        public static decimal Min(object value)
        {
            if (!ListExt.IsList(value))
                return StiValueHelper.TryToDecimal(value);

            var list = SkipNulls(ListExt.ToList(value)).TryCastToDecimal();
            return list.Any() ? list.Min() : 0;
        }

        public static decimal? MinNulls(object value)
        {
            if (!ListExt.IsList(value))
                return IsNull(value) ? (decimal?)null : StiValueHelper.TryToDecimal(value);

            var values = ListExt.ToList(value);
            if (values == null || values.Count() == 0 || values.All(v => IsNull(v)))
                return null;

            var list = SkipNulls(values).TryCastToDecimal();
            return list.Any() ? list.Min() : 0;
        }

        public static double MinD(object value)
        {
            if (!ListExt.IsList(value))
                return StiValueHelper.TryToDouble(value);

            var list = SkipNulls(ListExt.ToList(value)).TryCastToDouble();
            return list.Any() ? list.Min() : 0;
        }

        public static long MinI(object value)
        {
            if (!ListExt.IsList(value))
                return StiValueHelper.TryToLong(value);

            var list = SkipNulls(ListExt.ToList(value)).TryCastToLong();
            return list.Any() ? list.Min() : 0;
        }

        public static DateTime? MinDate(object value)
        {
            if (!ListExt.IsList(value))
                return StiValueHelper.TryToNullableDateTime(value);

            var list = SkipNulls(ListExt.ToList(value)).TryCastToNullableDateTime();
            return list.Any() ? list.Min() : null;
        }

        public static TimeSpan? MinTime(object value)
        {
            if (!ListExt.IsList(value))
                return StiValueHelper.TryToNullableTimeSpan(value);

            var list = SkipNulls(ListExt.ToList(value)).TryCastToNullableTimeSpan();
            return list.Any() ? list.Min() : null;
        }

        public static string MinMaxDateString(object value)
        {
            var minDate = MinDate(value);
            var maxDate = MaxDate(value);

            var minDateStr = minDate != null ? minDate.Value.ToString("MM'/'dd'/'yyyy") : "null";
            var maxDateStr = maxDate != null ? maxDate.Value.ToString("MM'/'dd'/'yyyy") : "null";

            return $"{minDateStr}-{maxDateStr}";
        }

        public static string MinStr(object value)
        {
            if (!ListExt.IsList(value))
                return ToString(value);

            return SkipNulls(ListExt.ToList(value))
                .OrderBy(ToString)
                .Cast<string>()
                .FirstOrDefault();
        }
    }
}