#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports.Net											}
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
using System;
using System.Linq;

namespace Stimulsoft.Report.Dictionary.Helpers
{
    public static class StiVariableBindingHelper
    {
        public static object[] GetArray(object value)
        {
            if (value == null || value is object[])
                return value as object[];

            else
                return new object[] { value };
        }

        public static bool IsEqualBinding(object valueBinding, object[] selectedValues)
        {
            if (valueBinding == null)
                return false;
            
            var valueBindingType = valueBinding.GetType();

            var v1 = ChangeTypeValueBinding(valueBinding);
            
            if (selectedValues == null)
                return true;

            return selectedValues.Any(v2 => v1.Equals(ChangeTypeValueBinding(v2, valueBindingType)));
        }

        public static object ChangeTypeValueBinding(object valueBinding, Type type = null)
        {
            var result = valueBinding;
            type = type ?? valueBinding.GetType();

            if (type == typeof(sbyte) ||
                type == typeof(byte) ||
                type == typeof(short) ||
                type == typeof(ushort) ||
                type == typeof(int) ||
                type == typeof(uint) ||
                type == typeof(long) ||
                type == typeof(ulong) ||
                type == typeof(sbyte?) ||
                type == typeof(byte?) ||
                type == typeof(short?) ||
                type == typeof(ushort?) ||
                type == typeof(int?) ||
                type == typeof(uint?) ||
                type == typeof(long?) ||
                type == typeof(ulong?) ||
                type == typeof(ByteList) ||
                type == typeof(ShortList) ||
                type == typeof(IntList) ||
                type == typeof(LongList))
            {
                result = StiValueHelper.TryToLong(valueBinding);
            }
            else if (
                type == typeof(double) ||
                type == typeof(float) ||
                type == typeof(double?) ||
                type == typeof(float?) ||
                type == typeof(DoubleList) ||
                type == typeof(FloatList))
            {
                result = StiValueHelper.TryToDouble(valueBinding);
            }
            else if (
                type == typeof(decimal) ||
                type == typeof(decimal?) ||
                type == typeof(DecimalList))
            {
                result = StiValueHelper.TryToDecimal(valueBinding);
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?) || type == typeof(DateTimeList))
            {
                result = StiValueHelper.TryToDateTime(valueBinding);
            }
            else if (type == typeof(TimeSpan))
            {
                result = (TimeSpan)valueBinding;
            }
            else if (type == typeof(TimeSpan?))
            {
                result = (TimeSpan?)valueBinding;
            }
            else if (type == typeof(TimeSpanList))
            {
                result = valueBinding;
            }
            else if (type == typeof(bool) || type == typeof(bool?) || type == typeof(BoolList))
            {
                result = StiValueHelper.TryToBool(valueBinding);
            }
            else if (type == typeof(char) || type == typeof(char?) || type == typeof(CharList))
            {
                result = StiValueHelper.TryToChar(valueBinding);
            }
            else if (type == typeof(Guid))
            {
                result = (Guid)valueBinding;
            }
            else if (type == typeof(Guid?))
            {
                result = (Guid?)valueBinding;
            }
            else if (type == typeof(GuidList))
            {
                result = valueBinding;
            }

            return result;
        }
    }
}
