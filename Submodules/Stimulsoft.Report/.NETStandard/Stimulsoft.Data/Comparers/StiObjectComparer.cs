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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Data.Helpers;
using Stimulsoft.Data.Types;

namespace Stimulsoft.Data.Comparers
{
    public class StiObjectComparer : IEqualityComparer<object>
    {
        #region IEqualityComparer
        public new bool Equals(object x, object y)
        {
            return Compare(x, y) == 0;
        }

        public int GetHashCode(object x)
        {
            return ((IStructuralEquatable)x).GetHashCode(EqualityComparer<object>.Default);
        }
        #endregion

        #region Properties
        public static readonly StiObjectComparer Default = new StiObjectComparer();
        #endregion

        #region Methods
        public static int Compare(object x, object y)
        {
            if (x == DBNull.Value && y == DBNull.Value)
                return 0;

            if (x == null && y == null)
                return 0;

            if (x == DBNull.Value)
                return -1;

            if (y == DBNull.Value)
                return 1;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            var type = x.GetType();

            if (type == typeof(DateTime))
                return DateTimeCompare(x, y);

            if (type == typeof(string))
                return string.Compare(x.ToString(), y.ToString(), CultureInfo.CurrentCulture, CompareOptions.OrdinalIgnoreCase);

            if (type == typeof(bool))
                return DefaultCompare<bool>(x, y);

            if (type == typeof(short) || type == typeof(int) || 
                type == typeof(long) || type == typeof(ushort) ||
                type == typeof(uint) || type == typeof(ulong))
                return Comparer<long>.Default.Compare(StiValueHelper.TryToLong(x), StiValueHelper.TryToLong(y));

            if (type == typeof(float) || type == typeof(double))
                return Comparer<double>.Default.Compare(StiValueHelper.TryToDouble(x), StiValueHelper.TryToDouble(y));

            if (type == typeof(decimal))
                return Comparer<decimal>.Default.Compare(StiValueHelper.TryToDecimal(x), StiValueHelper.TryToDecimal(y));

            if (type == typeof(byte[]))
                return ArrayCompare(x as byte[], y as byte[]);

            if (x is SimpleValue || y is SimpleValue)
            {
                var xValue = x is SimpleValue simpleValue1 ? simpleValue1.Value : x;
                var yValue = y is SimpleValue simpleValue2 ? simpleValue2.Value : y;
                return Compare(xValue, yValue);
            }

            if (x is DateTimeValue || y is DateTimeValue)
            {
                var xValue = x is DateTimeValue dateTimeValue1 ? dateTimeValue1.Value : x;
                var yValue = y is DateTimeValue dateTimeValue2 ? dateTimeValue2.Value : y;
                return Compare(xValue, yValue);
            }

            if (x is StiFiscalMonth || y is StiFiscalMonth)
            {
                var xValue = x is StiFiscalMonth fiscalMonth1 ? fiscalMonth1.ActualMonthIndex : x;
                var yValue = y is StiFiscalMonth fiscalMonth2 ? fiscalMonth2.ActualMonthIndex : y;
                return Compare(xValue, yValue);
            }

            if (x is IComparable || y is IComparable)
                return Comparer.Default.Compare(x, y);

            return 0;
        }

        private static int DefaultCompare<T>(object a, object b)
        {
            return Comparer<T>.Default.Compare((T)a, (T)b);
        }

        private static int DateTimeCompare(object a, object b)
        {
            if (a is DateTime && b is DateTime)
                return Comparer<DateTime>.Default.Compare(
                    StiDateTimeCorrector.Correct((DateTime)a),
                    StiDateTimeCorrector.Correct((DateTime)b));

            if (a is DateTime && !(b is DateTime))
                return -1;

            return 1;
        }

        private static int ArrayCompare(byte[] a, byte[] b)
        {
            if (a.Length < b.Length)
                return -1;

            if (a.Length > b.Length)
                return 1;

            var result = a.AsQueryable().SequenceEqual(b);
            return result ? 0 : -1;
        }
        #endregion
    }
}