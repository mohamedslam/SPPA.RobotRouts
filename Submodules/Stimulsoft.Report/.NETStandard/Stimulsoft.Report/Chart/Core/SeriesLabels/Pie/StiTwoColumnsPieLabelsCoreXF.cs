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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiTwoColumnsPieLabelsCoreXF : StiOutsidePieLabelsCoreXF
    {
        #region Methods
        public override StiSeriesLabelsGeom RenderLabel(IStiSeries series, StiContext context,
            PointF centerPie,
            float radius, float radius2, float pieAngle, 
            int pointIndex, double? value, 
            double? labelValue, string argumentText, string tag, bool measure,
            int colorIndex, int colorCount, float percentPerValue, out RectangleF measureRect, bool drawValue, float deltaY)
        {
            this.percentPerValue = percentPerValue;

            measureRect = RectangleF.Empty;

            if (labelValue == null && (!this.SeriesLabels.ShowNulls)) return null;
            if (labelValue == 0 && (!this.SeriesLabels.ShowZeros)) return null;

            if (value == null) value = 0d;
            if (labelValue == null) labelValue = 0d;

            if (float.IsNaN(radius) || float.IsNaN(radius2)) return null;
            
            if (drawValue)
            {
                return base.RenderLabel(series, context, centerPie, radius, radius2, pieAngle, pointIndex, value, labelValue,
                    GetFormattedValue(series, labelValue.Value), tag, measure, colorIndex, colorCount, percentPerValue, out measureRect, drawValue, deltaY);
            }

            var labelText = GetLabelText(series, labelValue, argumentText, tag, series.CoreTitle);
            var angleRad = (float)(Math.PI * pieAngle / 180);

            var arcPoint = GetLabelPoint(centerPie, radius, angleRad);
            var startPoint = GetLabelPoint(centerPie, radius + (((IStiTwoColumnsPieLabels)this.SeriesLabels).LineLength * context.Options.Zoom), angleRad);

            var dist = 0;
            if (arcPoint.X > startPoint.X) dist = -1;
            else dist = 1;

            var endPoint = new PointF(centerPie.X + radius * dist + ((IStiTwoColumnsPieLabels)this.SeriesLabels).LineLength * dist + 10 * dist, startPoint.Y);
            endPoint.Y += deltaY * Math.Min(context.Options.Zoom, 1);

            var font = StiFontGeom.ChangeFontSize(this.SeriesLabels.Font, this.SeriesLabels.Font.Size * context.Options.Zoom);

            var sf = SeriesLabels.Core.GetStringFormatGeom(context);
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            var labelRect = Rectangle.Round(GetLabelRect(context, endPoint, labelText, font, sf));

            var labelRectPosition = Rectangle.Empty;
            if (dist < 0)
            {
                labelRectPosition = Rectangle.Round(new RectangleF(
                    endPoint.X - labelRect.Width,
                    endPoint.Y - labelRect.Height / 2,
                    labelRect.Width, labelRect.Height));
            }
            else
            {
                labelRectPosition = Rectangle.Round(new RectangleF(
                    endPoint.X,
                    endPoint.Y - labelRect.Height / 2,
                    labelRect.Width, labelRect.Height));
            }

            if (measure)
            {
                measureRect = labelRectPosition;
                return null;
            }
            else
            {
                measureRect = RectangleF.Empty;

                var borderColor = GetBorderColor(series, colorIndex, colorCount);
                var seriesBorderColor = (Color)series.Core.GetSeriesBorderColor(colorIndex, colorCount);
                var seriesBrush = series.Core.GetSeriesBrush(colorIndex, colorCount);
                seriesBrush = ProcessSeriesColors(pointIndex, seriesBrush, series);
                var labelBrush = new StiSolidBrush(GetLabelColor(series, colorIndex, colorCount));
                var lineColor = this.GetLineColor(series, colorIndex, colorCount);
                var seriesLabelsBrush = ProcessSeriesColors(pointIndex, SeriesLabels.Brush, series);

                return new StiTwoColumnsPieLabelsGeom(this.SeriesLabels, series, pointIndex, value.Value, labelRectPosition, labelText, seriesBrush, labelBrush, seriesLabelsBrush, borderColor, seriesBorderColor,
                    labelRectPosition, lineColor, startPoint, endPoint, arcPoint, centerPie, null);
            }
        }
        #endregion        

        #region Properties
        public override StiSeriesLabelsType SeriesLabelsType
        {
            get
            {
                return StiSeriesLabelsType.Pie | StiSeriesLabelsType.Doughnut;
            }
        }

        public override int Position
        {
            get
            {
                return (int)StiSeriesLabelsPosition.TwoColumnsPie;
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
                return StiLocalization.Get("Chart", "LabelsTwoColumns");
            }
        }
        #endregion

        public StiTwoColumnsPieLabelsCoreXF(IStiSeriesLabels seriesLabels)
            : base(seriesLabels)
        {            
        }
	}
}
