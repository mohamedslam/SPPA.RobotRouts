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

namespace Stimulsoft.Report.CrossTab.Core
{
	public static class DecimalHelper
    {
        public static bool CanConvertToDecimal(object value)
        {
            if (value == null || DBNull.Value.Equals(value)) return false;

            return value is int ||
                   value is uint ||
                   value is long ||
                   value is ulong ||
                   value is sbyte ||
                   value is byte ||
                   value is float ||
                   value is double ||
                   value is decimal;
        }

        public static decimal ConvertToDecimal(object value)
        {
            try
            {
                if (value is string && ((string)value == "-" || ((string)value) == "")) return 0;
                return Convert.ToDecimal(value);
            }
            catch
            {
                return 0;
            }
        }
    }
}
