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
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Data.Functions
{
    public partial class Funcs
    {
        public static long DaysInMonth(long year, long month)
        {
            return global::System.DateTime.DaysInMonth((int)year, (int)month);
        }

        public static object DaysInMonthObject(object value1, object value2)
        {
            if (ListExt.IsList(value1) || ListExt.IsList(value2))
            {
                var list1 = ListExt.ToNullableDateTimeList(value1);
                var list2 = ListExt.ToNullableDateTimeList(value2);
                var list11 = list1 != null ? list1.ToList() : null;
                var list22 = list2 != null ? list2.ToList() : null;

                var count = list11 != null ? list11.Count : list22.Count;
                var resultList = new List<object>();

                for (var index = 0; index < count; index++)
                {
                    var v1 = list11 != null ? list11[index] : null;
                    var v2 = list22 != null ? list22[index] : null;
                    resultList.Add(DaysInMonthObject(v1, v2));
                }

                return resultList;
            }
            else
                return DaysInMonth(StiValueHelper.TryToInt(value1), StiValueHelper.TryToInt(value2));
        }

        public static long DaysInMonth(DateTime? date)
        {
            if (date.HasValue)
                return global::System.DateTime.DaysInMonth(date.Value.Year, date.Value.Month);
            else
                return 0;
        }

        public static object DaysInMonthObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(DaysInMonth);
            else
                return DaysInMonth(StiValueHelper.TryToNullableDateTime(value));
        }
    }
}