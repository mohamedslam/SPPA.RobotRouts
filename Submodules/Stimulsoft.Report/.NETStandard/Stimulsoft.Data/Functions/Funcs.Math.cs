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
using Stimulsoft.Data.Options;
using System;
using System.Linq;

namespace Stimulsoft.Data.Functions
{
    public partial class Funcs
    {
        public static decimal Abs(decimal value)
        {
            return Math.Abs(value);
        }

        public static object AbsObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToDecimalList(value).Select(Abs);
            else
                return Abs(StiValueHelper.TryToDecimal(value));
        }

        public static double Acos(double value)
        {
            return Math.Acos(value);
        }

        public static object AcosObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToDoubleList(value).Select(Acos);
            else
                return Acos(StiValueHelper.TryToDouble(value));
        }

        public static double Asin(double value)
        {
            return Math.Asin(value);
        }

        public static object AsinObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToDoubleList(value).Select(Asin);
            else
                return Asin(StiValueHelper.TryToDouble(value));
        }

        public static double Atan(double value)
        {
            return Math.Atan(value);
        }

        public static object AtanObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToDoubleList(value).Select(Atan);
            else
                return Atan(StiValueHelper.TryToDouble(value));
        }

        public static decimal Ceiling(decimal value)
        {
            return Math.Ceiling(value);
        }

        public static object CeilingObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToDecimalList(value).Select(Ceiling);
            else
                return Ceiling(StiValueHelper.TryToDecimal(value));
        }

        public static double Cos(double value)
        {
            return Math.Cos(value);
        }

        public static object CosObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToDoubleList(value).Select(Cos);
            else
                return Cos(StiValueHelper.TryToDouble(value));
        }

        public static decimal? Div(decimal value1, decimal value2, decimal? zeroResult = null)
        {
            if (value2 == 0) return zeroResult;
            return Math.Truncate(value1 / value2);
        }

        public static object DivObject(object value1, object value2, object zeroResult = null)
        {
            var decimalValue1 = StiValueHelper.TryToDecimal(value1);
            var decimalValue2 = StiValueHelper.TryToDecimal(value2);
            var decimalZeroResult = StiValueHelper.TryToNullableDecimal(zeroResult);

            return Div(decimalValue1, decimalValue2, decimalZeroResult);
        }

        public static double Exp(double value)
        {
            return Math.Exp(value);
        }

        public static object ExpObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToDoubleList(value).Select(Exp);
            else
                return Exp(StiValueHelper.TryToDouble(value));
        }

        public static decimal Floor(decimal value)
        {
            return Math.Floor(value);
        }

        public static object FloorObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToDecimalList(value).Select(Floor);
            else
                return Floor(StiValueHelper.TryToDecimal(value));
        }

        public static double Log(double value)
        {
            return Math.Log(value);
        }

        public static object LogObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToDoubleList(value).Select(Log);
            else
                return Log(StiValueHelper.TryToDouble(value));
        }

        public static decimal Round(decimal value, int decimals = 0)
        {
            return Math.Round(value, decimals, StiDataOptions.RoundType);
        }

        public static object RoundObject(object value, int decimals = 0)
        {
            if (ListExt.IsList(value))
                return ListExt.ToDecimalList(value).Select(v => Round(v, decimals));
            else
                return Round(StiValueHelper.TryToDecimal(value), decimals);
        }

        public static int Sign(decimal value)
        {
            return Math.Sign(value);
        }

        public static object SignObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToDecimalList(value).Select(Sign);
            else
                return Sign(StiValueHelper.TryToDecimal(value));
        }

        public static double Sin(double value)
        {
            return Math.Sin(value);
        }

        public static object SinObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToDoubleList(value).Select(Sin);
            else
                return Sin(StiValueHelper.TryToDouble(value));
        }

        public static double Sqrt(double value)
        {
            return Math.Sqrt(value);
        }

        public static object SqrtObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToDoubleList(value).Select(Sqrt);
            else
                return Sqrt(StiValueHelper.TryToDouble(value));
        }

        public static double Tan(double value)
        {
            return Math.Tan(value);
        }

        public static object TanObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToDoubleList(value).Select(Tan);
            else
                return Tan(StiValueHelper.TryToDouble(value));
        }

        public static decimal Truncate(decimal value)
        {
            return Math.Truncate(value);
        }

        public static object TruncateObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToDecimalList(value).Select(Truncate);
            else
                return Truncate(StiValueHelper.TryToDecimal(value));
        }
    }
}