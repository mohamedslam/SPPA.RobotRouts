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

namespace Stimulsoft.Report.Gauge.Helpers
{
    internal static class StiMathHelper
    {
        /// <summary>
        /// Вычисляет расстояние между значением value1 и value2. (Результат >= 0)
        /// </summary>
        /// <param name="value1">Начальное значение</param>
        /// <param name="value2">Конечное значение</param>
        public static float Length(float value1, float value2)
        {
            if (value1 < 0 && value2 < 0)
            {
                return Math.Abs(value1 - value2);
            }

            if (value1 > 0 && value2 > 0)
            {
                return value2 - value1;
            }

            return Math.Abs(value1) + value2;
        }

        public static float MaxMinusMin(float value1, float value2)
        {
            return (value1 > value2) ? (value1 - value2) : (value2 - value1);
        }

        public static float GetMax(params float[] list)
        {
            float max = 0f;

            if (list.Length > 0)
                max = list[0];

            int index = 1;
            while (index < list.Length)
            {
                if (max < list[index])
                    max = list[index];

                index++;
            }

            return max;
        }
    }
}