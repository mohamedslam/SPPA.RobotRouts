#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

namespace Stimulsoft.Data.Functions
{
    /// <summary>
    /// MonthToStr helper.
    /// </summary>
    public sealed class StiExtValueConverter
    {
        public static bool IsEligable(object value, string culture)
        {
            if (string.IsNullOrWhiteSpace(culture))
                return false;

            return value is StiMonth || value is DayOfWeek;
        }

        public static string Convert(object value, string culture)
        {
            if (string.IsNullOrWhiteSpace(culture))
                return null;

            if (value is StiMonth)
                return Funcs.ToProperCase(StiMonthToStrHelper.MonthName((int)value, culture));

            else if (value is DayOfWeek)
                return Funcs.ToProperCase(StiDayOfWeekToStrHelper.DayOfWeek((int)value, culture));

            else
                return null;
        }
    }
}