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
using System.Globalization;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Chart
{
    public class StiScatterAreaCoreXF : StiClusteredColumnAreaCoreXF
	{
        #region Fields
        // For calcutate XStripLines if argument is DateTime
        private bool isArgumentDateTime = false;
        #endregion

        #region Methods
        private bool IsXAxisAutoRange(IStiAxis xAxis)
        {
            var axisArea = this.Area as IStiAxisArea;
            return xAxis.Range.Auto || axisArea.XAxis.Range.Maximum == axisArea.XAxis.Range.Minimum || xAxis.LogarithmicScale;
        }

        protected override void PrepareRange(IStiAxis specXAxis, IStiAxis specXTopAxis, IStiAxis specYAxis, IStiAxis specYRightAxis)
        {
            base.PrepareRange(specXAxis, specXTopAxis, specYAxis, specYRightAxis);

            bool first = true;
            specXAxis.Info.Maximum = 0;
            specXAxis.Info.Minimum = 0;

            isArgumentDateTime = false;

            var seriesCollection = GetSeries();
            var cultureInfo = CultureInfo.GetCultureInfo("en-Us");
            foreach (IStiScatterSeries series in seriesCollection)
            {
                if (series.Arguments.Length > 0 && series.Arguments[0] is string)
                {
                    var st = series.Arguments[0] as string;
                    if (st.EndsWith("AM") || st.EndsWith("PM"))
                    {
                        var newArray = new object[series.Arguments.Length];
                                                
                        for (int index = 0; index < series.Arguments.Length; index++)
                        {
                            DateTime result;
                            if (DateTime.TryParse(series.Arguments[index] as string, cultureInfo, DateTimeStyles.None, out result))
                            {
                                newArray[index] = result;
                            }
                            else
                            {
                                newArray[index] = series.Arguments[index];
                            }
                        }

                        series.Arguments = newArray;
                    }
                }

                var culture = ((StiChart)series.Chart).Report.GetParsedCulture();
                foreach (object argument in series.Arguments)
                {
                    double value = 0;
                    if (argument is DateTime)
                    {
                        isArgumentDateTime = true;
                        value = ((DateTime)argument).ToOADate();
                    }
                    else if (argument is string)
                    {
                        double result;
                        if (StiSeries.TryParseValue((string)argument, culture, out result))
                        {
                            value = result;
                        }
                    }
                    else
                    {
                        try
                        {
                            value = (double)StiConvert.ChangeType(argument, typeof(double));
                        }
                        catch
                        {
                            value = 0;
                        }
                    }

                    if (first)
                    {
                        first = false;
                        specXAxis.Info.Maximum = value;
                        specXAxis.Info.Minimum = value;
                    }
                    else
                    {
                        specXAxis.Info.Maximum = Math.Max(specXAxis.Info.Maximum, value);
                        if (value == 0 && specXAxis.LogarithmicScale) continue;
                        specXAxis.Info.Minimum = Math.Min(specXAxis.Info.Minimum, value);
                    }
                }
            }
        }

        protected override void CheckStripLinesAndMaximumMinimumXAxis(IStiAxis axis)
        {
            if (axis.Info.StripLines.Count > 0)
            {
                axis.Info.Minimum = axis.Info.StripLines[0].Value;
                axis.Info.Maximum = axis.Info.StripLines[axis.Info.StripLines.Count - 1].Value;
            }
            else
            {
                axis.Info.Minimum = 0;
                axis.Info.Maximum = 1;
            }
        }

        protected override void CreateStripLinesXAxis(IStiAxis axis)
        {
            if (!(isArgumentDateTime) && !axis.LogarithmicScale)
            {
                if (IsXAxisAutoRange(axis))
                {
                    double range = Math.Abs(axis.Info.Maximum - axis.Info.Minimum) * 0.05;

                    if (range != 0)
                    {
                        axis.Info.Maximum += range;
                        axis.Info.Minimum -= range;

                        if (axis.Info.Minimum < 0 && axis.Info.Minimum + range >= 0) axis.Info.Minimum = 0;
                    }
                    else
                    {
                        axis.Info.Maximum *= 1.05;
                        if (axis.Info.Minimum < 0) axis.Info.Minimum *= 1.05;
                        else axis.Info.Minimum *= 0.95;
                    }

                    if (axis.Core.GetStartFromZero()) axis.Info.Minimum = 0;

                    if (axis.Info.Minimum == axis.Info.Maximum)
                    {
                        if (axis.Info.Maximum == 0) axis.Info.Maximum = 100;
                        else
                        {
                            axis.Info.Minimum -= axis.Info.Minimum * 0.1;
                            axis.Info.Maximum -= axis.Info.Maximum * 0.1;
                        }
                    }
                }
            }

            if (isArgumentDateTime)
            {
                if (IsXAxisAutoRange(axis))
                {
                    double range = Math.Abs(axis.Info.Maximum - axis.Info.Minimum);

                    if (range == 0)
                    {
                        axis.Info.Maximum++;
                        axis.Info.Minimum--;
                    }
                }
            }

            bool isDateTimeValues = false;
            List<IStiSeries> seriesCollection = GetSeries();
            for (int index = 0; index < seriesCollection.Count; index++)
            {
                if (seriesCollection[index].Core.IsDateTimeValues)
                {
                    isDateTimeValues = true;
                    break;
                }
            }

            var dec = new decimal(axis.Labels.Step);
            var step = (double)dec;

            if (step == 0) step = StiStripLineCalculatorXF.GetInterval(axis.Info.Minimum, axis.Info.Maximum, 6);
            StiStripLinesXF xStripLines;
            if (axis.LogarithmicScale)
            {
                xStripLines = StiStripLineCalculatorXF.GetStripLinesLogScale(axis.Info.Minimum, axis.Info.Maximum);
            }
            else
            {
                xStripLines = StiStripLineCalculatorXF.GetStripLines(axis.Info.Minimum, axis.Info.Maximum, step, isDateTimeValues, true);
            }


            axis.Info.StripLines.Clear();
            for (int id = xStripLines.Count - 1; id >= 0; id--)
            {
                double value = xStripLines[id].Value;
                object valueObject = xStripLines[id].ValueObject;
                if (isArgumentDateTime) valueObject = DateTime.FromOADate(Convert.ToDouble(value));

                axis.Info.StripLines.Add(valueObject, value);
            }
        }

        protected override void CreateStripLinesYAxis(IStiAxis axis, bool isDateTimeValues)
        {
            if (Area.IsDefaultSeriesTypeFullStackedColumnSeries ||
                Area.IsDefaultSeriesTypeFullStackedBarSeries)
            {
                #region Check Positive & Negative values
                bool positivePresent = false;
                bool negativePresent = false;

                List<IStiSeries> seriesCollection = this.GetSeries();
                foreach (IStiSeries series in seriesCollection)
                {
                    foreach (double? value in series.Values)
                    {
                        if (value > 0)
                            positivePresent = true;
                        if (value < 0)
                            negativePresent = true;
                    }
                }
                #endregion

                double minimum = negativePresent ? -100 : 0;
                double maximum = positivePresent ? 100 : 0;

                double step = axis.Labels.Step;
                if (step == 0) step = StiStripLineCalculatorXF.GetInterval(minimum, maximum, 6);
                axis.Info.StripLines = StiStripLineCalculatorXF.GetStripLines(minimum, maximum, step, false);

                foreach (StiStripLineXF stripLine in axis.Info.StripLines)
                {
                    stripLine.ValueObject = string.Format("{0}%", stripLine.ValueObject);
                }
            }
            else
            {
                float step = axis.Labels.Step;

                //Special code for preventing very small step
                if (step > 0 && axis.Info.Range > 0 && (axis.Info.Range / step) > 500)
                    step = 0;

                if (step == 0) step = (float)StiStripLineCalculatorXF.GetInterval(axis.Info.Minimum, axis.Info.Maximum, 6);

                if (axis.LogarithmicScale)
                {
                    axis.Info.StripLines = StiStripLineCalculatorXF.GetStripLinesLogScale(axis.Info.Minimum, axis.Info.Maximum);
                }
                else
                {
                    axis.Info.StripLines = StiStripLineCalculatorXF.GetStripLines(axis.Info.Minimum, axis.Info.Maximum, (double)Convert.ToDecimal(step), isDateTimeValues);
                }
            }
        }
        #endregion        

        #region Properties.Localization
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string LocalizedName
        {
            get
            {
                return StiLocalization.Get("Chart", "Scatter");
            }
        }
        #endregion

        #region Properties.Settings
        public override int Position
        {
            get
            {
                return (int)StiChartAreaPosition.Scatter;
            }
        }
        #endregion

        public StiScatterAreaCoreXF(IStiArea area)
            : base(area)
        {            
        }
	}
}
