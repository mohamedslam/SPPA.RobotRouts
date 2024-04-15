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
    public class StiGanttAreaCoreXF : StiClusteredBarAreaCoreXF
    {
        #region Methods
        protected override void CreateStripLinesXAxis(IStiAxis axis)
        {
            axis.Info.StripLines.Clear();
            axis.Info.StripLines.Add(null, 0);

            List<IStiSeries> seriesCollection = GetSeries();
            foreach (IStiSeries series in seriesCollection)
            {
                foreach (object argument in series.Arguments)
                {
                    bool notExist = true;
                    foreach (StiStripLineXF line in axis.Info.StripLines)
                    {
                        if (line.ValueObject != null && line.ValueObject.ToString() == argument.ToString()) notExist = false;
                    }

                    if (notExist) axis.Info.StripLines.Add(argument, axis.Info.StripLines.Count);
                }
            }

            axis.Info.StripLines.Add(string.Empty, axis.Info.StripLines.Count);
        }

        protected override void PrepareRange(IStiAxis specXAxis, IStiAxis specXTopAxis, IStiAxis specYAxis, IStiAxis specYRightAxis)
        {
            specXAxis.Info.Maximum = 0;
            specXAxis.Info.Minimum = 0;

            specYAxis.Info.Maximum = 0;
            specYAxis.Info.Minimum = 0;

            List<IStiSeries> seriesCollection = GetSeries();
            bool first = true;
            foreach (IStiGanttSeries series in seriesCollection)
            {
                foreach (double? value in series.Values)
                {
                    if (value != null)
                    {
                        if (first)
                        {
                            first = false;
                            specYAxis.Info.Maximum = value.Value;
                            specYAxis.Info.Minimum = value.Value;
                        }
                        else
                        {
                            specYAxis.Info.Maximum = Math.Max(specYAxis.Info.Maximum, value.Value);
                            specYAxis.Info.Minimum = Math.Min(specYAxis.Info.Minimum, value.Value);
                        }
                    }
                }

                foreach (double? value in series.ValuesEnd)
                {
                    if (value != null)
                    {
                        specYAxis.Info.Maximum = Math.Max(specYAxis.Info.Maximum, value.Value);
                        specYAxis.Info.Minimum = Math.Min(specYAxis.Info.Minimum, value.Value);
                    }
                }
            }

            specYRightAxis.Info.Maximum = specYAxis.Info.Maximum;
            specYRightAxis.Info.Minimum = specYAxis.Info.Minimum;
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
                return StiLocalization.Get("Chart", "Gantt");
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
                return (int)StiChartAreaPosition.Gantt;
            }
        }
        #endregion

        public StiGanttAreaCoreXF(IStiArea area)
            : base(area)
        {            
        }
	}
}
