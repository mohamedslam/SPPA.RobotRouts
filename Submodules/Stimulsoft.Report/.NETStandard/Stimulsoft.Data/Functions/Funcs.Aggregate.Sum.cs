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
        public static decimal Sum(object value)
        {
            if (!ListExt.IsList(value))
                return StiValueHelper.TryToDecimal(value);
            
            return SkipNulls(ListExt.ToList(value))
                .TryCastToDecimal()
                .Sum();
        }

        public static decimal? SumNulls(object value)
        {
            if (!ListExt.IsList(value))
                return IsNull(value) ? (decimal?)null : StiValueHelper.TryToDecimal(value);

            var list = ListExt.ToList(value);
            
            if (list.All(v => IsNull(v)))
                return null;
            
            return Sum(list);
        }

        private static bool IsNull(object value)
        {
            return value == null || value == DBNull.Value;
        }

        public static double SumD(object value)
        {
            if (!ListExt.IsList(value))
                return StiValueHelper.TryToDouble(value);

            return SkipNulls(ListExt.ToList(value))
                .TryCastToDouble()
                .Sum();
        }

        public static long SumI(object value)
        {
            if (!ListExt.IsList(value))
                return StiValueHelper.TryToLong(value);

            return SkipNulls(ListExt.ToList(value))
                .TryCastToLong()
                .Sum();
        }

        public static TimeSpan SumTime(object value)
        {
            if (!ListExt.IsList(value))
                return StiValueHelper.TryToTimeSpan(value);

            var timeSpan = SkipNulls(ListExt.ToList(value))
                .TryCastToTimeSpan()
                .Sum(t => t.Ticks);

            return new TimeSpan(timeSpan);
        }

        public static decimal SumDistinct(object value)
        {
            if (!ListExt.IsList(value))
                return StiValueHelper.TryToDecimal(value);

            return SkipNulls(ListExt.ToList(value))
                .TryCastToDecimal()
                .Distinct()
                .Sum();
        }
    }
}