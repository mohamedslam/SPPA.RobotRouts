#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using Stimulsoft.Report.Chart;

namespace Stimulsoft.Report.Chart
{
    public class StiSeriesLabelsGeom3D : StiGeom3D
    {
        #region Properties
        public double Value { get; }

        public int Index { get; }

        public IStiSeries Series { get; }

        public IStiSeriesLabels SeriesLabels { get; }
        #endregion

        public StiSeriesLabelsGeom3D(IStiSeriesLabels seriesLabels, IStiSeries series, int index, double value, StiRender3D render3D)
            : base(render3D)
        {
            this.SeriesLabels = seriesLabels;
            this.Series = series;
            this.Index = index;
            this.Value = value;
        }
    }
}
