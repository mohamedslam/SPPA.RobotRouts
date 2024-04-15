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
using System;

namespace Stimulsoft.Report.Chart
{
    internal class StiClusteredColumnAreaCoreXF3D : StiAxisAreaCoreXF3D
    {
        #region Properties.Localization
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string LocalizedName
        {
            get
            {
                return $"3D {StiLocalization.Get("Chart", "ClusteredColumn")}";
            }
        }
        #endregion

        #region Properties.Settings
        public override int Position => (int)StiChartAreaPosition.ClusteredColumn3d;
        #endregion

        #region Methods
        protected override void PrepareRange(IStiAxis3D specXAxis, IStiAxis3D specYAxis, IStiAxis3D specZAxis)
        {
            bool first = true;

            specYAxis.Info.Minimum = 0;
            specYAxis.Info.Maximum = 0;

            var seriesCollection = GetSeries();

            foreach(var series in seriesCollection)
            {
                foreach (double? value in series.Values)
                {
                    if (value != null)
                    {
                        if (first)
                        {
                            specYAxis.Info.Maximum = value.Value;
                            specYAxis.Info.Minimum = value.Value;
                            first = false;
                        }

                        else
                        {
                            specYAxis.Info.Maximum = Math.Max(value.Value, specYAxis.Info.Maximum);
                            specYAxis.Info.Minimum = Math.Min(value.Value, specYAxis.Info.Minimum);
                        }
                    }
                }
            }
        }
        #endregion

        public StiClusteredColumnAreaCoreXF3D(IStiArea area)
            : base(area)
        {
        }
    }
}
