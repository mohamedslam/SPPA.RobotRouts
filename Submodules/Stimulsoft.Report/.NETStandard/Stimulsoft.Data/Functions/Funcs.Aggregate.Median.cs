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
using System.Linq;

namespace Stimulsoft.Data.Functions
{
    public partial class Funcs
    {
        public static decimal Median(object value)
        {
            if (!ListExt.IsList(value))
                return StiValueHelper.TryToDecimal(value);

            var values = ListExt.ToList(value)
                .TryCastToDecimal()
                .OrderBy(n => n)
                .ToArray();

            if (!values.Any())
                return 0;

            if (values.Length == 1)
                return values[0];

            if (values.Length % 2 == 0)
                return (values[values.Length / 2 - 1] + values[values.Length / 2]) / 2.0m;

            return values[values.Length / 2];
        }

        public static double MedianD(object value)
        {
            if (!ListExt.IsList(value))
                return StiValueHelper.TryToDouble(value);

            var values = ListExt.ToList(value)
                .TryCastToDouble()
                .OrderBy(n => n)
                .ToArray();

            if (!values.Any())
                return 0;

            if (values.Length == 1)
                return values[0];

            if (values.Length % 2 == 0)
                return (values[values.Length / 2 - 1] + values[values.Length / 2]) / 2.0d;

            return values[values.Length / 2];
        }

        public static long MedianI(object value)
        {
            if (!ListExt.IsList(value))
                return StiValueHelper.TryToLong(value);

            var values = ListExt.ToList(value)
                .TryCastToLong()
                .OrderBy(n => n)
                .ToArray();

            if (!values.Any())
                return 0;

            if (values.Length == 1)
                return values[0];

            if (values.Length % 2 == 0)
                return (values[values.Length / 2 - 1] + values[values.Length / 2]) / 2;

            return values[values.Length / 2];
        }
    }
}