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
        public static int DaysInYear(long year)
        {
            return global::System.DateTime.IsLeapYear((int)year) ? 366 : 365;
        }

        public static int DaysInYear(DateTime? date)
        {
            if (date.HasValue)
                return DaysInYear(date.Value.Year);
            else
                return 0;
        }

        public static object DaysInYearObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToNullableDateTimeList(value).Select(DaysInMonth);
            
            if (value is DateTime)
                return DaysInYear(StiValueHelper.TryToNullableDateTime(value));
            else
                return DaysInYear(StiValueHelper.TryToInt(value));
        }
    }
}