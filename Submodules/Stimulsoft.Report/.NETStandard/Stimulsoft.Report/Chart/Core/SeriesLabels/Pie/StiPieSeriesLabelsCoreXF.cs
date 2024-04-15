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
using System.ComponentModel;
using System.Drawing;
using Stimulsoft.Base.Context;
using System.Collections.Generic;

namespace Stimulsoft.Report.Chart
{
    public abstract class StiPieSeriesLabelsCoreXF : StiSeriesLabelsCoreXF
    {
        #region Methods
        public abstract StiSeriesLabelsGeom RenderLabel(IStiSeries series, StiContext context,
            PointF centerPie, float radius, float radius2, float pieAngle,
            int pointIndex, double? value, double? labelValue, string argumentText, string tag, bool measure,
            int colorIndex, int colorCount, float percentPerValue, out RectangleF measureRect, bool drawValue, float deltaY);

        internal override double RecalcValue(double value, int signs)
        {
            if (((IStiPieSeriesLabels)this.SeriesLabels).ShowInPercent)
            {
                List<IStiSeries> seriesCollection = this.SeriesLabels.Chart.Area.Core.GetSeries();

                if (seriesCollection.Count > 0)
                {
                    IStiSeries[] seriesArray = new IStiSeries[seriesCollection.Count];
                    seriesCollection.CopyTo(seriesArray);

                    if (seriesArray.Length > 0 && seriesArray[0] is StiPieSeries)
                        percentPerValue = ((StiPieSeriesCoreXF)seriesArray[0].Core).GetPercentPerValue(seriesArray);
                }

                return Math.Round(value * percentPerValue, signs);
            }

            return value;
        }
        #endregion

        #region Properties
        public override StiSeriesLabelsType SeriesLabelsType
        {
            get
            {
                return StiSeriesLabelsType.Pie;
            }
        }
        #endregion

        #region Fields
        [Browsable(false)]
        public float percentPerValue = 0.0f;
        #endregion

        public StiPieSeriesLabelsCoreXF(IStiSeriesLabels seriesLabels)
            : base(seriesLabels)
        {            
        }
	}
}
