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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Data.Functions
{
    public partial class Funcs
    { 
        public static decimal SumIf(object value, object condition)
        {
            if (!ListExt.IsList(value))
                return GetCondition(condition) ? StiValueHelper.TryToDecimal(value) : 0m;

            var values = ListExt.ToList(value).TryCastToDecimal();
            var conditions = GetConditions(condition);

            var index = 0;
            var sum = 0m;
            foreach (var v in values)
            {
                var conditionValue = index < conditions.Count ? conditions[index] : conditions.LastOrDefault();
                if (conditionValue)
                    sum += v;

                index++;
            }
            return sum;
        }

        public static double SumDIf(object value, object condition)
        {
            if (!ListExt.IsList(value))
                return GetCondition(condition) ? StiValueHelper.TryToDouble(value) : 0d;

            var values = ListExt.ToList(value).TryCastToDouble();
            var conditions = GetConditions(condition);

            var index = 0;
            var sum = 0d;
            foreach (var v in values)
            {
                var conditionValue = index < conditions.Count ? conditions[index] : conditions.LastOrDefault();
                if (conditionValue)
                    sum += v;

                index++;
            }
            return sum;
        }

        public static long SumIIf(object value, object condition)
        {
            if (!ListExt.IsList(value))
                return GetCondition(condition) ? StiValueHelper.TryToLong(value) : 0L;

            var values = ListExt.ToList(value).TryCastToLong();
            var conditions = GetConditions(condition);

            var index = 0;
            var sum = 0L;
            foreach (var v in values)
            {
                var conditionValue = index < conditions.Count ? conditions[index] : conditions.LastOrDefault();
                if (conditionValue)
                    sum += v;

                index++;
            }
            return sum;
        }

        public static TimeSpan SumTimeIf(object value, object condition)
        {
            if (!ListExt.IsList(value))
                return GetCondition(condition) ? StiValueHelper.TryToTimeSpan(value) : TimeSpan.Zero;

            var values = ListExt.ToList(value).TryCastToTimeSpan();
            var conditions = GetConditions(condition);

            var index = 0;
            var sum = 0L;
            foreach (var v in values)
            {
                var conditionValue = index < conditions.Count ? conditions[index] : conditions.LastOrDefault();
                if (conditionValue)
                    sum += v.Ticks;

                index++;
            }
            return new TimeSpan(sum);
        }

        public static decimal SumDistinctIf(object value, object condition)
        {
            if (!ListExt.IsList(value))
                return GetCondition(condition) ? StiValueHelper.TryToDecimal(value) : 0m;

            var values = ListExt.ToList(value).TryCastToDecimal();
            var conditions = GetConditions(condition);

            var hash = new Hashtable();
            var index = 0;
            var sum = 0m;
            foreach (var v in values)
            {
                var conditionValue = index < conditions.Count ? conditions[index] : conditions.LastOrDefault();
                if (conditionValue && hash[conditionValue] != null)
                {
                    sum += v;
                    hash[conditionValue] = conditionValue;
                }

                index++;
            }
            return sum;
        }

        private static bool GetCondition(object value)
        {
            if (!ListExt.IsList(value))
                return StiValueHelper.TryToBool(value);

            return ListExt.ToList(value)
                .TryCastToBool()
                .FirstOrDefault();
        }

        private static List<bool> GetConditions(object value)
        {
            if (!ListExt.IsList(value))
                return new List<bool> { StiValueHelper.TryToBool(value) };

            return ListExt.ToList(value)
                .TryCastToBool()
                .ToList();
        }
    }
}