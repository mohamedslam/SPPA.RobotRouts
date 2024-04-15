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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Stimulsoft.Data.Extensions
{
    public static class IEnumerableExt
    {
        public static IEnumerable<object[]> WhereEqualsTo(this IEnumerable<object[]> source, object values1, object values2)
        {
            return source.Where(r => StiValueComparer.EqualValues(values1, values2));
        }

        public static IEnumerable<object[]> WhereArrayItemEqualsTo(this IEnumerable<object[]> source, int itemIndex, object value)
        {
            return itemIndex == -1  
                ? source 
                : source.Where(r => StiValueComparer.EqualValues(r[itemIndex], value));
        }

        public static IEnumerable<object[]> WhereArrayItemStringEqualsTo(this IEnumerable<object[]> source, int itemIndex, string value)
        {
            return itemIndex == -1
                ? source
                : source.Where(r => StiValueComparer.EqualValues(ToString(r[itemIndex]), value));
        }

        public static object[] WhereFirstOrDefaultArrayItemStringEqualsTo(this IEnumerable<object[]> source, int itemIndex, string value)
        {
            return itemIndex == -1
                ? source.FirstOrDefault()
                : source.FirstOrDefault(r => StiValueComparer.EqualValues(ToString(r[itemIndex]), value));
        }

        private static string ToString(object value)
        {
            return value == null ? string.Empty : value.ToString();
        }

        public static IEnumerable<object> GetArrayItem(this IEnumerable<object[]> source, int itemIndex)
        {
            return source.Select(r => r[itemIndex]);
        }

        public static IEnumerable<string> GetStringArrayItem(this IEnumerable<object[]> source, int itemIndex)
        {
            return source.Select(r => r[itemIndex] as string);
        }

        public static IEnumerable<double?> TryCastValueOrFirstDefaultToNullableDouble<TSource>(this IEnumerable<TSource> source)
        {
            return source.Select(c => StiValueHelper.TryToNullableDouble(GetValueOrFirstOrDefault(c)));
        }

        public static IEnumerable<decimal?> TryCastToNullableDecimal<TSource>(this IEnumerable<TSource> source)
        {
            return source.Select(c => StiValueHelper.TryToNullableDecimal(c));
        }

        public static IEnumerable<double?> TryCastToNullableDouble<TSource>(this IEnumerable<TSource> source)
        {
            return source.Select(c => StiValueHelper.TryToNullableDouble(c));
        }

        public static IEnumerable<decimal> TryCastToDecimal<TSource>(this IEnumerable<TSource> source)
        {
            return source.Select(c => StiValueHelper.TryToDecimal(c));
        }

        public static IEnumerable<double> TryCastToDouble<TSource>(this IEnumerable<TSource> source)
        {
            return source.Select(c => StiValueHelper.TryToDouble(c));
        }

        public static IEnumerable<bool> TryCastToBool<TSource>(this IEnumerable<TSource> source)
        {
            return source.Select(c => StiValueHelper.TryToBool(c));
        }

        public static IEnumerable<int> TryCastToInt<TSource>(this IEnumerable<TSource> source)
        {
            return source.Select(c => StiValueHelper.TryToInt(c));
        }

        public static IEnumerable<long> TryCastToLong<TSource>(this IEnumerable<TSource> source)
        {
            return source.Select(c => StiValueHelper.TryToLong(c));
        }

        public static IEnumerable<DateTime> TryCastToDateTime<TSource>(this IEnumerable<TSource> source)
        {
            return source.Select(c => StiValueHelper.TryToDateTime(c));
        }

        public static IEnumerable<DateTime?> TryCastToNullableDateTime<TSource>(this IEnumerable<TSource> source)
        {
            return source.Select(c => StiValueHelper.TryToNullableDateTime(c));
        }

        public static IEnumerable<TimeSpan?> TryCastToNullableTimeSpan<TSource>(this IEnumerable<TSource> source)
        {
            return source.Select(c => StiValueHelper.TryToNullableTimeSpan(c));
        }

        public static IEnumerable<TimeSpan> TryCastToTimeSpan<TSource>(this IEnumerable<TSource> source)
        {
            return source.Select(c => StiValueHelper.TryToTimeSpan(c));
        }

        public static IEnumerable<string> TryCastToString<TSource>(this IEnumerable<TSource> source)
        {
            return source.Select(c => StiValueHelper.TryToString(c));
        }

        public static decimal? FirstOrDefaultAsNullableDecimal<TSource>(this IEnumerable<TSource> source)
        {
            return StiValueHelper.TryToNullableDecimal(GetValueOrFirstOrDefault(source.FirstOrDefault()));
        }

        public static DateTime? FirstOrDefaultAsNullableDateTime<TSource>(this IEnumerable<TSource> source)
        {
            return StiValueHelper.TryToNullableDateTime(GetValueOrFirstOrDefault(source.FirstOrDefault()));
        }

        public static double? FirstOrDefaultAsNullableDouble<TSource>(this IEnumerable<TSource> source)
        {
            return StiValueHelper.TryToNullableDouble(GetValueOrFirstOrDefault(source.FirstOrDefault()));
        }

        public static decimal FirstOrDefaultAsDecimal<TSource>(this IEnumerable<TSource> source)
        {
            return StiValueHelper.TryToDecimal(GetValueOrFirstOrDefault(source.FirstOrDefault()));
        }

        public static object GetValueOrFirstOrDefault(object value)
        {
            return ListExt.IsList(value) 
                ? ListExt.ToList(value).FirstOrDefault() 
                : value;
        }

        public static IEnumerable<object> Add(object a, object b)
        {
            var ay = a as IEnumerable<object>;
            var by = b as IEnumerable<object>;

            if (ay == null && by == null)
                return null;

            if (ay != null && by != null)
                return ay.Zip(by, (v1, v2) => StiValueHelper.TryToDecimal(v1) + StiValueHelper.TryToDecimal(v2)).Cast<object>();

            if (ay != null)
            {
                var bb = StiValueHelper.TryToDecimal(b);
                return ay.Select(v => StiValueHelper.TryToDecimal(v) + bb).Cast<object>();
            }

            var aa = StiValueHelper.TryToDecimal(a);
            return by.Select(v => StiValueHelper.TryToDecimal(v) + aa).Cast<object>();
        }

        public static IEnumerable<object> Sub(object a, object b)
        {
            var ay = a as IEnumerable<object>;
            var by = b as IEnumerable<object>;

            if (ay == null && by == null)
                return null;

            if (ay != null && by != null)
                return ay.Zip(by, (v1, v2) => StiValueHelper.TryToDecimal(v1) - StiValueHelper.TryToDecimal(v2)).Cast<object>();

            if (ay != null)
            {
                var bb = StiValueHelper.TryToDecimal(b);
                return ay.Select(v => StiValueHelper.TryToDecimal(v) - bb).Cast<object>();
            }

            var aa = StiValueHelper.TryToDecimal(a);
            return by.Select(v => StiValueHelper.TryToDecimal(v) - aa).Cast<object>();
        }

        public static IEnumerable<object> Mult(object a, object b)
        {
            var ay = a as IEnumerable<object>;
            var by = b as IEnumerable<object>;

            if (ay == null && by == null)
                return null;

            if (ay != null && by != null)
                return ay.Zip(by, (v1, v2) => StiValueHelper.TryToDecimal(v1) * StiValueHelper.TryToDecimal(v2)).Cast<object>();

            if (ay != null)
            {
                var bb = StiValueHelper.TryToDecimal(b);
                return ay.Select(v => StiValueHelper.TryToDecimal(v) * bb).Cast<object>();
            }

            var aa = StiValueHelper.TryToDecimal(a);
            return by.Select(v => StiValueHelper.TryToDecimal(v) * aa).Cast<object>();
        }

        public static IEnumerable<object> BitwiseAnd(object a, object b)
        {
            var ay = a as IEnumerable<object>;
            var by = b as IEnumerable<object>;

            if (ay == null && by == null)
                return null;

            if (ay != null && by != null)
            {
                var result = ay.Zip(by, (v1, v2) => StiValueHelper.TryToInt(v1) & StiValueHelper.TryToInt(v2)).Cast<object>();
                if (ListExt.IsBoolList(ay))
                    return result.AsEnumerable().TryCastToInt().Select(c => c == 1).Cast<object>();
                else
                    return result;
            }

            if (ay != null)
            {
                var bb = StiValueHelper.TryToInt(b);
                var result = ay.Select(v => StiValueHelper.TryToInt(v) & bb).Cast<object>();
                if (ListExt.IsBoolList(ay))
                    return result.AsEnumerable().TryCastToInt().Select(c => c == 1).Cast<object>();
                else
                    return result;
            }

            var aa = StiValueHelper.TryToInt(a);
            var result2= by.Select(v => StiValueHelper.TryToInt(v) & aa).Cast<object>();
            if (ListExt.IsBoolList(by))
                return result2.AsEnumerable().TryCastToInt().Select(c => c == 1).Cast<object>();
            else
                return result2;
        }

        public static IEnumerable<object> BitwiseXOr(object a, object b)
        {
            var ay = a as IEnumerable<object>;
            var by = b as IEnumerable<object>;

            if (ay == null && by == null)
                return null;

            if (ay != null && by != null)
            {
                var result = ay.Zip(by, (v1, v2) => StiValueHelper.TryToInt(v1) ^ StiValueHelper.TryToInt(v2)).Cast<object>();
                if (ListExt.IsBoolList(ay))
                    return result.AsEnumerable().TryCastToInt().Select(c => c == 1).Cast<object>();
                else
                    return result;
            }

            if (ay != null)
            {
                var bb = StiValueHelper.TryToInt(b);
                var result = ay.Select(v => StiValueHelper.TryToInt(v) ^ bb).Cast<object>();
                if (ListExt.IsBoolList(ay))
                    return result.AsEnumerable().TryCastToInt().Select(c => c == 1).Cast<object>();
                else
                    return result;
            }

            var aa = StiValueHelper.TryToInt(a);
            var result2 = by.Select(v => StiValueHelper.TryToInt(v) ^ aa).Cast<object>();
            if (ListExt.IsBoolList(by))
                return result2.AsEnumerable().TryCastToInt().Select(c => c == 1).Cast<object>();
            else
                return result2;
        }

        public static IEnumerable<object> BitwiseOr(object a, object b)
        {
            var ay = a as IEnumerable<object>;
            var by = b as IEnumerable<object>;

            if (ay == null && by == null)
                return null;

            if (ay != null && by != null)
            {
                var result = ay.Zip(by, (v1, v2) => StiValueHelper.TryToInt(v1) | StiValueHelper.TryToInt(v2)).Cast<object>();
                if (ListExt.IsBoolList(ay))
                    return result.AsEnumerable().TryCastToInt().Select(c => c == 1).Cast<object>();
                else
                    return result;
            }

            if (ay != null)
            {
                var bb = StiValueHelper.TryToInt(b);
                var result = ay.Select(v => StiValueHelper.TryToInt(v) | bb).Cast<object>();
                if (ListExt.IsBoolList(ay))
                    return result.AsEnumerable().TryCastToInt().Select(c => c == 1).Cast<object>();
                else
                    return result;
            }

            var aa = StiValueHelper.TryToInt(a);
            var result2 = by.Select(v => StiValueHelper.TryToInt(v) | aa).Cast<object>();
            if (ListExt.IsBoolList(by))
                return result2.AsEnumerable().TryCastToInt().Select(c => c == 1).Cast<object>();
            else
                return result2;
        }

        public static IEnumerable<object> Div(object a, object b)
        {
            var ay = a as IEnumerable<object>;
            var by = b as IEnumerable<object>;

            if (ay == null && by == null)
                return null;

            if (ay != null && by != null)
                return ay.Zip(by, (v1, v2) => StiValueHelper.TryToDecimal(v2) != 0 ? StiValueHelper.TryToDecimal(v1) / StiValueHelper.TryToDecimal(v2) : 0).Cast<object>();

            if (ay != null)
            {
                var bb = StiValueHelper.TryToDecimal(b);
                return ay.Select(v => bb != 0 ? StiValueHelper.TryToDecimal(v) / bb : 0).Cast<object>();
            }

            var aa = StiValueHelper.TryToDecimal(a);
            return by.Select(v => aa != 0 ? StiValueHelper.TryToDecimal(v) / aa : 0).Cast<object>();
        }
    }
}