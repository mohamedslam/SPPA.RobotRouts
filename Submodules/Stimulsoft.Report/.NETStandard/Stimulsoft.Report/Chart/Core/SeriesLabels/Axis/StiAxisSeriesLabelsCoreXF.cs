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
using System.ComponentModel;
using System.Drawing;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;

namespace Stimulsoft.Report.Chart
{
    public abstract class StiAxisSeriesLabelsCoreXF : StiSeriesLabelsCoreXF
    {
        #region Methods
        public abstract StiSeriesLabelsGeom RenderLabel(IStiSeries series, StiContext context,
            PointF endPoint, PointF startPoint,
            int pointIndex, double? value,
            double? labelValue, string argumentText, string tag,
            int colorIndex, int colorCount, RectangleF rect, StiAnimation animation = null);

        public virtual StiSeriesLabelsGeom RenderLabel(IStiSeries series, StiContext context,
            PointF endPoint, PointF startPoint,
            int pointIndex, double? value,
            double? labelValue, string argumentText, string tag, double weight,
            int colorIndex, int colorCount, RectangleF rect, StiAnimation animation = null)
        {
            return null;
        }

        internal override double RecalcValue(double value, int signs)
        {
            if (((IStiAxisSeriesLabels)this.SeriesLabels).ShowInPercent && (this.SeriesLabels.Chart.Area is StiFullStackedColumnArea) && (this.currentIndex != null))
            {
                List<IStiSeries> seriesCollection = this.SeriesLabels.Chart.Area.Core.GetSeries();

                double sumIndexValue = 0;

                foreach (IStiSeries series in seriesCollection)
                {
                    if (currentIndex < series.Values.Length)
                        sumIndexValue += series.Values[(int)this.currentIndex].GetValueOrDefault();
                }

                if (sumIndexValue != 0)
                    return Math.Round(100 * value / sumIndexValue, signs);
            }

            return value;
        }
        #endregion

        #region Properties
        public override StiSeriesLabelsType SeriesLabelsType
        {
            get
            {
                return StiSeriesLabelsType.Axis;
            }
        }
        #endregion

        #region Fields
        [Browsable(false)]
        internal int? currentIndex = null;
        #endregion

        public StiAxisSeriesLabelsCoreXF(IStiSeriesLabels seriesLabels)
            : base(seriesLabels)
        {            
        }
	}
}
