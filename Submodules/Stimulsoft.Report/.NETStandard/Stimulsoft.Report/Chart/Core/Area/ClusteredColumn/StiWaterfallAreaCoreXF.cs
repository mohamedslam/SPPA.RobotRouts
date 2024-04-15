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

namespace Stimulsoft.Report.Chart
{
    public class StiWaterfallAreaCoreXF : StiAxisAreaCoreXF
    {
        #region Methods
        protected override void PrepareRange(IStiAxis specXAxis, IStiAxis specXTopAxis, IStiAxis specYAxis, IStiAxis specYRightAxis)
        {
            var firstLeft = true;
            var currentValue = 0d;
            specYAxis.Info.Maximum = 0;
            specYAxis.Info.Minimum = 0;
            var seriesCollection = GetSeries();

            for (int index = 0; index < seriesCollection.Count; index++)
            {
                var series = seriesCollection[index] as StiWaterfallSeries;
                var lastDeltaIndex = series.Total.Visible ? 1 : 0;
                for(var indexVallue = 0; indexVallue < series.Values.Length - lastDeltaIndex; indexVallue++)
                {
                    var value = series.Values[indexVallue];
                    if (series.YAxis == StiSeriesYAxis.LeftYAxis)
                    {
                        if (value != null)
                        {
                            if (firstLeft)
                            {
                                currentValue = value.Value;
                                specYAxis.Info.Maximum = Math.Max(0, value.Value);
                                specYAxis.Info.Minimum = Math.Min(0, value.Value);
                                firstLeft = false;
                            }
                            else
                            {
                                currentValue += value.Value;
                                specYAxis.Info.Maximum = Math.Max(currentValue, specYAxis.Info.Maximum);
                                specYAxis.Info.Minimum = Math.Min(currentValue, specYAxis.Info.Minimum);
                            }
                        }
                    }
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
                return StiLocalization.Get("Chart", "Waterfall");
            }
        }
        #endregion

        #region Properties.Settings
        public override int Position
        {
            get
            {
                return (int)StiChartAreaPosition.Waterfall;
            }
        }
        #endregion  

        public StiWaterfallAreaCoreXF(IStiArea area)
            : base(area)
        {
        }
    }
}
