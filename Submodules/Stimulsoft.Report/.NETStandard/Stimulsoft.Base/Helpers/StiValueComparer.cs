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

namespace Stimulsoft.Base.Helpers
{
    public static class StiValueComparer
    {
        public static bool EqualValues(object value1, object value2)
        {
            if (value1 == null || value2 == null)
                return false;

            if (value1 is Array && value2 is Array)
                return CompareArrays(value1 as Array, value2 as Array);
            
            if (value1.GetType().IsNumericType() && value2.GetType().IsNumericType())
                return StiValueHelper.TryToDecimal(value1) == StiValueHelper.TryToDecimal(value2);

            if (value1 is string)
                return (value1 as string).Equals(value2.ToString());

            return value1.Equals(value2);
        }

        static bool CompareArrays(Array a, Array b)
        {
            if (a.Length != b.Length)
                return false;

            for (var i = 0; i < a.Length; i++)
            {
                if (!EqualValues(a.GetValue(i), b.GetValue(i)))
                    return false;
            }

            return true;
        }
    }
}