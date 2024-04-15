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
using System.Collections.Generic;
using Stimulsoft.Base.Helpers;

namespace Stimulsoft.Data.Engine
{
    internal class StiDataFilterComparer : IComparer<object>
    {
        public bool ConvertStrings { get; set; }

        public int Compare(object x, object y)
        {
            if (x == null && y == null)
                return 0;

            if (x == null)
                return 1;

            if (y == null)
                return -1;

            if (x.GetType().IsNumericType() && y.GetType().IsNumericType())
                return StiValueHelper.TryToDecimal(x).CompareTo(StiValueHelper.TryToDecimal(y));

            if (x is string && y is string)
                return string.Compare((string)x, (string)y, StringComparison.Ordinal);

            if (x is DateTime && y is DateTime)
                return -((DateTime)x).CompareTo((DateTime)y);

            if (x is string && y.GetType().IsNumericType())
                return ConvertStrings ? StiValueHelper.TryToDecimal(x).CompareTo(StiValueHelper.TryToDecimal(y)) : -1;

            if (y is string && x.GetType().IsNumericType())
                return ConvertStrings ? StiValueHelper.TryToDecimal(x).CompareTo(StiValueHelper.TryToDecimal(y)) : 1;

            return 0;
        }

        public bool Similar(object x, object y)
        {
            if (x == null && y == null)
                return true;

            if (x == null || y == null)
                return false;

            if (x is string && y is string)
                return (string)x == (string)y;

            if (x.GetType().IsNumericType() && y.GetType().IsNumericType())
                return StiValueHelper.TryToDecimal(x) == StiValueHelper.TryToDecimal(y);

            if (x is DateTime && y is DateTime)
                return ((DateTime)x).Ticks == ((DateTime)y).Ticks;

            if (ConvertStrings && (x is string && y.GetType().IsNumericType() || y is string && x.GetType().IsNumericType()))
                return StiValueHelper.TryToDecimal(x) == StiValueHelper.TryToDecimal(y);

            return false;
        }

        public StiDataFilterComparer()
        {
        }

        public StiDataFilterComparer(bool convertStrings) : this()
        {
            ConvertStrings = convertStrings;
        }
    }
}
