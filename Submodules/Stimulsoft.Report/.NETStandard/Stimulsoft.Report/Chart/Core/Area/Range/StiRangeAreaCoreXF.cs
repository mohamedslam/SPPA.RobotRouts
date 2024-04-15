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
using Stimulsoft.Base.Localization;
using System.Collections.Generic;

namespace Stimulsoft.Report.Chart
{
    public class StiRangeAreaCoreXF : StiClusteredColumnAreaCoreXF
	{
        #region Properties.Localization
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string LocalizedName
        {
            get
            {
                return StiLocalization.Get("PropertyMain", "Range");
            }
        }
        #endregion

        #region Properties.Settings
        public override int Position
        {
            get
            {
                return (int)StiChartAreaPosition.Range;
            }
        }
        #endregion

        #region Methods
        protected override void PrepareRange(IStiAxis specXAxis, IStiAxis specXTopAxis, IStiAxis specYAxis, IStiAxis specYRightAxis)
        {
            bool firstLeft = true;
            bool firstRight = true;
            bool seriesLeftAxisY = false;
            bool seriesRightAxisY = false;
            
            specYAxis.Info.Maximum = 0;
            specYAxis.Info.Minimum = 0;
            specYRightAxis.Info.Maximum = 0;
            specYRightAxis.Info.Minimum = 0;
            List<IStiSeries> seriesCollection = GetSeries();

            for (int index = 0; index < seriesCollection.Count; index++)
            {
                IStiLineRangeSeries series = (IStiLineRangeSeries)seriesCollection[index];
                double?[] values = series.Values;
                double?[] valuesEnd = series.ValuesEnd;

                int count = Math.Min(values.Length, valuesEnd.Length);

                for (int indexValues = 0; indexValues < count; indexValues++)
                {
                    double value = values[indexValues] != null ? (double)values[indexValues] : 0;
                    double valueEnd = valuesEnd[indexValues] != null ? (double)valuesEnd[indexValues] : 0;

                    double? valueMax = Math.Max(value, valueEnd);
                    double? valueMin = Math.Min(value, valueEnd);
                    if (series.YAxis == StiSeriesYAxis.LeftYAxis)
                    {
                        if (firstLeft)
                        {
                            seriesLeftAxisY = true;
                            specYAxis.Info.Maximum = valueMax.Value;
                            specYAxis.Info.Minimum = valueMin.Value;
                            firstLeft = false;
                        }
                        else
                        {
                            seriesLeftAxisY = true;
                            specYAxis.Info.Maximum = Math.Max(valueMax.Value, specYAxis.Info.Maximum);
                            specYAxis.Info.Minimum = Math.Min(valueMin.Value, specYAxis.Info.Minimum);
                        }                        
                    }
                    else
                    {
                        if (firstRight)
                        {
                            seriesRightAxisY = true;
                            specYRightAxis.Info.Maximum = valueMax.Value;
                            specYRightAxis.Info.Minimum = valueMin.Value;
                            firstRight = false;
                        }
                        else
                        {
                            seriesRightAxisY = true;
                            specYRightAxis.Info.Maximum = Math.Max(valueMax.Value, specYRightAxis.Info.Maximum);
                            specYRightAxis.Info.Minimum = Math.Min(valueMin.Value, specYRightAxis.Info.Minimum);
                        }                        
                    }
                }

            }

            if (!seriesLeftAxisY)
            {
                specYAxis.Info.Maximum = specYRightAxis.Info.Maximum;
                specYAxis.Info.Minimum = specYRightAxis.Info.Minimum;
            }
            if (!seriesRightAxisY)
            {
                specYRightAxis.Info.Maximum = specYAxis.Info.Maximum;
                specYRightAxis.Info.Minimum = specYAxis.Info.Minimum;
            }
        }
        #endregion

        public StiRangeAreaCoreXF(IStiArea area)
            : base(area)
        {            
        }
	}
}
