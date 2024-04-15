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

namespace Stimulsoft.Report.Chart
{
    internal static class StiStripLineCalculatorXF
    {
        #region Methods
        /// <summary>
        /// Calculate interval between two ticks of axis.
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        private static double GetInterval(double interval)
        {
            double tick = Math.Abs(interval);
            int count = 0;
            if (interval > 1)
            {
                while (tick > 1)
                {
                    tick /= 10;
                    count++;
                }

                if (tick < 0.15) tick = 0.1;
                else if (tick < 0.3) tick = 0.2;
                else if (tick < 0.75) tick = 0.5;
                else tick = 1.0;

                while (count > 0)
                {
                    tick *= 10;
                    count--;
                }
            }
            else if (interval > 0)
            {
                while ((tick * 10) < 1)
                {
                    tick *= 10;
                    count++;
                }
                if (tick < 0.15) tick = 0.1;
                else if (tick < 0.3) tick = 0.2;
                else if (tick < 0.75) tick = 0.5;
                else tick = 1.0;

                while (count > 0)
                {
                    tick /= 10;
                    count--;
                }
            }
            return tick;
        }


		internal static double GetInterval(double minValue, double maxValue, int num)
		{
			if (maxValue == minValue)return 0;
			return GetInterval((maxValue - minValue) / num);
		}


        internal static StiStripLinesXF GetStripLines(double minValue, double maxValue, double step, bool asDateTimeValue, bool defaultMinValue = false)
        {
            var list = new StiStripLinesXF();
			if (minValue == maxValue) return list;

            decimal minValueD = (decimal)minValue;
            decimal maxValueD = (decimal)maxValue;
            decimal stepD = (decimal)step;
            decimal pos = defaultMinValue ? minValueD : (long)(minValueD / stepD) * stepD;
            
            while (minValueD < pos + stepD) pos -= stepD;

            while (pos < maxValueD)
            {
                pos += stepD;

                if (asDateTimeValue) list.Insert(0, new StiStripLineXF(DateTime.FromOADate((double)pos), (double)pos));
                else list.Insert(0, new StiStripLineXF(pos.ToString(), (double)pos));
            }

            return list;
        }

        internal static StiStripLinesXF GetStripLinesLogScale(double minValue, double maxValue)
        {
            StiStripLinesXF list = new StiStripLinesXF();
            if (minValue == maxValue) return list;

            decimal minValueD = (decimal)minValue;
            decimal maxValueD = (decimal)maxValue;

            decimal minValueScale = 1;
            decimal maxValueScale = 1;

            #region Min Value Scale
            if (0 < minValue && minValue < 1)
            {
                while (minValueD < minValueScale)
                {
                    minValueScale /= 10;
                }
            }

            if(minValue > 1)
            {
                while (minValueD > minValueScale)
                {
                    minValueScale *= 10;
                }
                minValueScale /= 10;
            }
            #endregion

            #region Max Value Scale
            if (0 < maxValueD && maxValueD < 1)
            {
                while (maxValueScale > maxValueD)
                {
                    maxValueScale /= 10;
                }
                maxValueScale *= 10;
            }

            if (maxValueD > 1)
            {
                while (maxValueD > maxValueScale)
                {
                    maxValueScale *= 10;
                }
            }
            #endregion
            
            decimal pos = minValueScale;

            decimal startStepValue = pos;

            int index = 1;
            while (pos <= maxValueScale)
            {
                list.Insert(0, new StiStripLineXF(((double)pos).ToString(), (double)pos));

                pos += startStepValue;
                index++;

                if (index == 10)
                {
                    index = 1;
                    startStepValue = pos;
                }
            }

            return list;
        }
        #endregion
    }
}
