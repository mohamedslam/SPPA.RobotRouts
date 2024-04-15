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
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;

namespace Stimulsoft.Base.Helpers
{
    public static class StiArrayHelper
    {
        public static double?[] ToNullableDoubleArray(object[] values)
        {
            int index = 0;
            var result = new double?[values.Length];
            foreach (var obj in values)
            {
                try
                {
                    result[index] = (double) Convert.ToDecimal(obj);
                }
                catch
                {
                    result[index] = null;
                }
                index++;
            }

            return result;
        }

        public static double[] ToDoubleArray(object[] values)
        {
            int index = 0;
            var result = new double[values.Length];
            foreach (var obj in values)
            {
                if (obj == null || obj == DBNull.Value) continue;

                try
                {
                    if (obj is string)
                    {
                        double value;
                        if (double.TryParse(obj as string, out value))
                            result[index] = value;
                    }
                    else
                        result[index] = (double)Convert.ToDecimal(obj);
                }
                catch
                {
                }
                index++;
            }

            return result;
        }
    }
}