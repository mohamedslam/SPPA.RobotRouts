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
using System.Collections;
using System.Linq;

namespace Stimulsoft.Data.Functions
{
    public partial class Funcs
    {
        public static long Count(object value)
        {
            if (ListExt.IsList(value))
                return OptionalSkipNulls(ListExt.ToList(value)).Count();
            else
                return StiValueHelper.TryToLong(value);
        }

        public static long CountIf(object value, object condition)
        {
            if (!ListExt.IsList(value))
                return GetCondition(condition) ? 1L : 0L;

            var values = SkipNulls(ListExt.ToList(value));
            var conditions = GetConditions(condition);

            var count = 0L;
            var index = 0;
            foreach (var v in values)
            {
                var condValue = index < conditions.Count ? conditions[index] : conditions.LastOrDefault();
                if (condValue)
                    count += 1;

                index++;
            }
            return count;
        }

        public static object Distinct(object value)
        {
            if (ListExt.IsList(value))
                return OptionalSkipNulls(ListExt.ToList(value)).Distinct();
            else
                return value;
        }

        public static long DistinctCount(object value)
        {
            if (ListExt.IsList(value))
                return OptionalSkipNulls(ListExt.ToList(value)).Distinct().Count();
            else
                return 1;
        }

        public static long DistinctCountIf(object value, object condition)
        {
            if (!ListExt.IsList(value))
                return GetCondition(condition) ? 1L : 0L;

            var values = ListExt.ToList(value);
            var conditions = GetConditions(condition);

            var hash = new Hashtable();
            var count = 0L;
            var index = 0;
            foreach (var v in values)
            {
                var condValue = index < conditions.Count ? conditions[index] : conditions.LastOrDefault();
                if (condValue && hash[v] == null)
                    count += 1;

                hash[v] = v;
                index++;
            }
            return count;
        }
    }
}