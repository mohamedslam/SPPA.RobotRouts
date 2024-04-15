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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Data.Extensions
{
    public static class ListExt
    {
        public static object Compare(object a, object b)
        {
            if (!IsList(a) && !IsList(b)) return -1;

            if (IsList(a) && !IsList(b))
                return ToList(a).Select(aa => CompareValues(aa, b));

            if (!IsList(a) && IsList(b))
                return ToList(b).Select(bb => CompareValues(a, bb));
            
            return ToList(a).SequenceEqual(ToList(b)) ? 0 : 1;
        }

        private static object CompareValues(object a, object b)
        {
            if (a is bool && b is bool)
                return (bool) a == (bool) b;

            if (a is string)
                return b != null && (string)a == (string)b;

            return Comparer.Default.Compare(a, b);

        }

        public static bool IsList(object value) => !(value is string) && value is IEnumerable;

        public static bool IsBoolList(object value) => ToList(value)?.FirstOrDefault() is bool;

        public static IEnumerable<object> ToList(object value) => (value as IEnumerable)?.Cast<object>();

        public static IEnumerable<string> ToStringList(object value) => ToList(value)?.TryCastToString();

        public static IEnumerable<int> ToIntList(object value) => ToList(value)?.TryCastToInt();

        public static IEnumerable<double> ToDoubleList(object value) => ToList(value)?.TryCastToDouble();

        public static IEnumerable<decimal> ToDecimalList(object value) => ToList(value)?.TryCastToDecimal();

        public static IEnumerable<DateTime?> ToNullableDateTimeList(object value) => ToList(value)?.TryCastToNullableDateTime();

        public static object[] ToArray(object value) => ToList(value)?.ToArray();

        public static string[] ToStringArray(object value) => ToStringList(value)?.ToArray();

        public static int[] ToIntArray(object value) => ToIntList(value)?.ToArray();

        public static double[] ToDoubleArray(object value) => ToDoubleList(value)?.ToArray();

        public static decimal[] ToDecimalArray(object value) => ToDecimalList(value)?.ToArray();

        public static object Add(object a, object b)
        {
            var array = ToArray(a);
            if (array == null)
                return a;

            for (var index = 0; index < array.Length; index++)
            {
                var valueA = array[index];
                
                if (valueA is Byte)
                    array[index] = (Byte)valueA + StiValueHelper.TryToLong(b);

                else if (valueA is SByte)
                    array[index] = (SByte)valueA + StiValueHelper.TryToLong(b);

                else if (valueA is Int16)
                    array[index] = (Int16)valueA + StiValueHelper.TryToLong(b);

                else if (valueA is UInt16)
                    array[index] = (UInt16)valueA + StiValueHelper.TryToLong(b);

                else if (valueA is Int32)
                    array[index] = (Int32)valueA + StiValueHelper.TryToLong(b);

                else if (valueA is UInt32)
                    array[index] = (UInt32)valueA + StiValueHelper.TryToLong(b);

                else if (valueA is Int64)
                    array[index] = (Int64)valueA + StiValueHelper.TryToLong(b);

                else if (valueA is UInt64)
                    array[index] = (UInt64)valueA + StiValueHelper.TryToULong(b);

                else if (valueA is Single)
                    array[index] = (Single)valueA + StiValueHelper.TryToFloat(b);

                else if (valueA is Double)
                    array[index] = (Double)valueA + StiValueHelper.TryToDouble(b);

                else if (valueA is Decimal)
                    array[index] = (Decimal)valueA + StiValueHelper.TryToDecimal(b);
            }

            return array.ToList();
        }
    }
}