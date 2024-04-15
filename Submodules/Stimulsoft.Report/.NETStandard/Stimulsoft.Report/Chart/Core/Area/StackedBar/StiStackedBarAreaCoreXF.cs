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
using System.Collections;
using System.Collections.Generic;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Chart
{
    public class StiStackedBarAreaCoreXF : StiClusteredBarAreaCoreXF
    {
        #region Methods
        private List<List<double>> PrepareSeriesRange(Type seriesType)
        {
            var minValues = new List<double>();
            var maxValues = new List<double>();
            var minValuesRight = new List<double>();
            var maxValuesRight = new List<double>();
            var seriesCollection = GetSeries();

            for (int indexPoint = 0; indexPoint < base.ValuesCount; indexPoint++)
            {
                foreach (var series in seriesCollection)
                {
                    if (series.GetType() == seriesType)
                    {
                        if (series.YAxis == StiSeriesYAxis.LeftYAxis)
                        {
                            minValues.Add(0d);
                            maxValues.Add(0d);

                            if (indexPoint < series.Values.Length)
                            {
                                double? value = series.Values[indexPoint];

                                if (value > 0) 
                                    maxValues[indexPoint] = maxValues[indexPoint] + value.GetValueOrDefault();
                                else 
                                    minValues[indexPoint] = minValues[indexPoint] + value.GetValueOrDefault();
                            }
                        }
                        else
                        {
                            minValuesRight.Add(0d);
                            maxValuesRight.Add(0d);

                            if (indexPoint < series.Values.Length)
                            {
                                double? value = series.Values[indexPoint];

                                if (value > 0)
                                    maxValuesRight[indexPoint] = maxValuesRight[indexPoint] + value.GetValueOrDefault();
                                else 
                                    minValuesRight[indexPoint] = minValuesRight[indexPoint] + value.GetValueOrDefault();
                            }
                        }
                    }
                }
            }

            var values = new List<List<double>>
            {
                minValues, 
                maxValues, 
                minValuesRight, 
                maxValuesRight
            };

            return values;
        }

        protected override void PrepareRange(IStiAxis specXAxis, IStiAxis specXTopAxis, IStiAxis specYAxis, IStiAxis specYRightAxis)
        {
            var seriesCollection = GetSeries();
            var seriesTypes = new Hashtable();

            foreach (var series in seriesCollection)
            {
                seriesTypes[series.GetType()] = series.GetType();
            }

            var minValues = new List<double>();
            var maxValues = new List<double>();
            var minValuesRight = new List<double>();
            var maxValuesRight = new List<double>();

            foreach (Type seriesType in seriesTypes.Values)
            {
                var values = PrepareSeriesRange(seriesType);

                int index = 0;
                foreach (double value in values[0])
                {
                    if (minValues.Count <= index) 
                        minValues.Add(value);
                    else 
                        minValues[index] = Math.Min(minValues[index], value);

                    index++;
                }

                index = 0;
                foreach (double value in values[1])
                {
                    if (maxValues.Count <= index) 
                        maxValues.Add(value);
                    else
                        maxValues[index] = Math.Max(maxValues[index], value);

                    index++;
                }

                index = 0;
                foreach (double value in values[2])
                {
                    if (minValuesRight.Count <= index) 
                        minValuesRight.Add(value);
                    else 
                        minValuesRight[index] = Math.Min(minValuesRight[index], value);

                    index++;
                }

                index = 0;
                foreach (double value in values[3])
                {
                    if (maxValuesRight.Count <= index) 
                        maxValuesRight.Add(value);
                    else 
                        maxValuesRight[index] = Math.Max(maxValuesRight[index], value);

                    index++;
                }
            }

            if (minValues.Count == 0) minValues = minValuesRight;
            if (maxValues.Count == 0) maxValues = maxValuesRight;
            if (minValuesRight.Count == 0) minValuesRight = minValues;
            if (maxValuesRight.Count == 0) maxValuesRight = maxValues;

            #region MinimumY

            bool firstValue = true;
            foreach (double value in minValues)
            {
                if (firstValue)
                {
                    specYAxis.Info.Minimum = value;
                    firstValue = false;
                }
                else specYAxis.Info.Minimum = Math.Min(value, specYAxis.Info.Minimum);
            }

            #endregion

            #region MaximumY

            firstValue = true;
            foreach (double value in maxValues)
            {
                if (firstValue)
                {
                    specYAxis.Info.Maximum = value;
                    firstValue = false;
                }
                else specYAxis.Info.Maximum = Math.Max(value, specYAxis.Info.Maximum);
            }

            #endregion

            #region MinimumRightY

            firstValue = true;
            foreach (double value in minValuesRight)
            {
                if (firstValue)
                {
                    specYRightAxis.Info.Minimum = value;
                    firstValue = false;
                }
                else specYRightAxis.Info.Minimum = Math.Min(value, specYRightAxis.Info.Minimum);
            }

            #endregion

            #region MaximumRightY

            firstValue = true;
            foreach (double value in maxValuesRight)
            {
                if (firstValue)
                {
                    specYRightAxis.Info.Maximum = value;
                    firstValue = false;
                }
                else specYRightAxis.Info.Maximum = Math.Max(value, specYRightAxis.Info.Maximum);
            }

            #endregion
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
                return StiLocalization.Get("Chart", "StackedBar");
            }
        }
        #endregion

        #region Properties.Settings
        public override StiChartSeriesOrientation SeriesOrientation
        {
            get
            {
                return StiChartSeriesOrientation.Horizontal;
            }
        }

        public override int Position
        {
            get
            {
                return (int)StiChartAreaPosition.StackedBar;
            }
        }
        #endregion

        public StiStackedBarAreaCoreXF(IStiArea area)
            : base(area)
        {            
        }
	}
}
