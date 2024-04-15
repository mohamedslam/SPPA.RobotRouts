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
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Chart
{
    public class StiClusteredColumnAreaCoreXF : StiAxisAreaCoreXF
    {
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
            var seriesCollection = GetSeries();

            for (int index = 0; index < seriesCollection.Count; index++)
            {
                IStiSeries series = (IStiSeries)seriesCollection[index];
                double?[] values = series.Values;
                foreach (double? value in values)
                {
                    if (series.YAxis == StiSeriesYAxis.LeftYAxis)
                    {
                        if (value != null)
                        {
                            if (firstLeft)
                            {
                                seriesLeftAxisY = true;
                                specYAxis.Info.Maximum = value.Value;
                                specYAxis.Info.Minimum = value.Value;
                                firstLeft = false;
                            }
                            else
                            {
                                seriesLeftAxisY = true;
                                specYAxis.Info.Maximum = Math.Max(value.Value, specYAxis.Info.Maximum);
                                specYAxis.Info.Minimum = Math.Min(value.Value, specYAxis.Info.Minimum);
                            }
                        }
                    }
                    else
                    {
                        if (value != null)
                        {
                            if (firstRight)
                            {
                                seriesRightAxisY = true;
                                specYRightAxis.Info.Maximum = value.Value;
                                specYRightAxis.Info.Minimum = value.Value;
                                firstRight = false;
                            }
                            else
                            {
                                seriesRightAxisY = true;
                                specYRightAxis.Info.Maximum = Math.Max(value.Value, specYRightAxis.Info.Maximum);
                                specYRightAxis.Info.Minimum = Math.Min(value.Value, specYRightAxis.Info.Minimum);
                            }
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

        #region Properties.Localization
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string LocalizedName
        {
            get
            {
                return StiLocalization.Get("Chart", "ClusteredColumn");
            }
        }
        #endregion

        #region Properties.Settings
        public override int Position
        {
            get
            {
                return (int)StiChartAreaPosition.ClusteredColumn;
            }
        }
        #endregion  

        public StiClusteredColumnAreaCoreXF(IStiArea area)
            : base(area)
        {            
        }
	}
}
