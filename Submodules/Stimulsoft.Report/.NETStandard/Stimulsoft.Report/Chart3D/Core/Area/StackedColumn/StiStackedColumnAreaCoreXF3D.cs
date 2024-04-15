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

using Stimulsoft.Base.Localization;
using System.Collections.Generic;
using System.Collections;
using System;

namespace Stimulsoft.Report.Chart
{
    public class StiStackedColumnAreaCoreXF3D : StiAxisAreaCoreXF3D
    {
        #region Methods
        private List<List<double>> PrepareSeriesRange(Type seriesType)
        {
            var minValues = new List<double>();
            var maxValues = new List<double>();
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
                    }
                }
            }

            var values = new List<List<double>>
            {
                minValues,
                maxValues
            };

            return values;
        }

        protected override void PrepareRange(IStiAxis3D specXAxis, IStiAxis3D yAxis, IStiAxis3D zAxis)
        {
            var seriesCollection = GetSeries();
            var seriesTypes = new Hashtable();

            foreach (var series in seriesCollection)
            {
                seriesTypes[series.GetType()] = series.GetType();
            }

            var minValues = new List<double>();
            var maxValues = new List<double>();

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
            }

            #region MinimumY
            bool firstValue = true;
            foreach (double value in minValues)
            {
                if (firstValue)
                {
                    yAxis.Info.Minimum = value;
                    firstValue = false;
                }
                else yAxis.Info.Minimum = Math.Min(value, yAxis.Info.Minimum);
            }
            #endregion

            #region MaximumY
            firstValue = true;
            foreach (double value in maxValues)
            {
                if (firstValue)
                {
                    yAxis.Info.Maximum = value;
                    firstValue = false;
                }
                else yAxis.Info.Maximum = Math.Max(value, yAxis.Info.Maximum);
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
                return $"3D {StiLocalization.Get("Chart", "StackedColumn")}";
            }
        }
        #endregion

        #region Properties.Settings
        public override int Position
        {
            get
            {
                return (int)StiChartAreaPosition.StackedColumn3d;
            }
        }
        #endregion

        public StiStackedColumnAreaCoreXF3D(IStiArea area)
            : base(area)
        {
        }
    }
}
