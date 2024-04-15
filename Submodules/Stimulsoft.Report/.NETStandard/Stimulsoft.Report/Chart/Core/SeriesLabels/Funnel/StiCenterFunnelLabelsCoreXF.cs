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
using System.Drawing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;
using System.Collections.Generic;

namespace Stimulsoft.Report.Chart
{
    public class StiCenterFunnelLabelsCoreXF : StiFunnelSeriesLabelsCoreXF
    {
        #region Methods
        public override StiSeriesLabelsGeom RenderLabel(IStiSeries series, StiContext context,
            int pointIndex, double value, double valueNext,
            string argumentText, string tag,
            int colorIndex, int colorCount, RectangleF rect, float singleValueHeight, float singleValueWidth, float centerAxis, out RectangleF measureRect)
        {
            measureRect = rect;

            string labelText = GetLabelText(series, value, argumentText, tag, series.CoreTitle);

            StiBrush labelBrush = new StiSolidBrush(GetLabelColor(series, colorIndex, colorCount));
            Color labelBorderColor = GetBorderColor(series, colorIndex, colorCount);
            StiStringFormatGeom sf = this.GetStringFormatGeom(context);
            StiFontGeom font = StiFontGeom.ChangeFontSize(this.SeriesLabels.Font, this.SeriesLabels.Font.Size * context.Options.Zoom);

            Color seriesBorderColor = (Color)series.Core.GetSeriesBorderColor(colorIndex, colorCount);
            StiBrush seriesBrush = series.Core.GetSeriesBrush(colorIndex, colorCount);

            PointF point;
            if (series is StiFunnelWeightedSlicesSeries)
                point = new PointF(rect.Width / 2, rect.Height * 0.05f + singleValueHeight * GetSumLastValues(series, pointIndex) + singleValueHeight * Math.Abs((float)value / 2));
            else
                point = new PointF(rect.Width / 2, singleValueHeight * pointIndex + singleValueHeight / 2 + rect.Height * 0.05f);

            RectangleF labelRect = context.MeasureRotatedString(labelText, font, point, sf, StiRotationMode.CenterCenter, 0f, (int)(this.SeriesLabels.Width * context.Options.Zoom));

            return new StiCenterFunnelLabelsGeom(this.SeriesLabels, series, pointIndex, value, labelRect, labelText,
                seriesBrush, labelBrush, labelBorderColor, seriesBorderColor, labelRect, null);
        }

        private float GetSumLastValues(IStiSeries series, int indexCurrent)
        {
            var values = new List<double>();

            foreach (IStiFunnelSeries ser in series.Chart.Series)
            {
                foreach (double? value in ser.Values)
                {
                    if (!ser.ShowZeros && value.GetValueOrDefault() == 0)
                        continue;
                    values.Add(value.GetValueOrDefault());
                }
            }
            
            float sumLastValues = 0;
            for (int index = 0; index < indexCurrent; index++)
            {
                if (index >= values.Count)
                    break;
                sumLastValues += (float)Math.Abs(values[index]);
            }
            return sumLastValues;
        }
        #endregion

        #region Properties
        public override StiSeriesLabelsType SeriesLabelsType
        {
            get
            {
                return StiSeriesLabelsType.Funnel;
            }
        }

        public override int Position
        {
            get
            {
                return (int)StiSeriesLabelsPosition.CenterFunnel;
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
                return StiLocalization.Get("Chart", "LabelsCenter");
            }
        }
        #endregion

        public StiCenterFunnelLabelsCoreXF(IStiSeriesLabels seriesLabels)
            : base(seriesLabels)
        {
        }
    }
}
