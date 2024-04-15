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

namespace Stimulsoft.Report.Chart
{
    public class StiCenterPieLabelsCoreXF : StiPieSeriesLabelsCoreXF
    {
        #region Methods
        public override StiSeriesLabelsGeom RenderLabel(IStiSeries series, StiContext context,
            PointF centerPie,
            float radius, float radius2, float pieAngle,
            int pointIndex, double? value,
            double? labelValue, string argumentText, string tag, bool measure,
            int colorIndex, int colorCount, float percentPerValue, out RectangleF measureRect, bool drawValue, float deltaY)
        {
            measureRect = RectangleF.Empty;

            this.percentPerValue = percentPerValue;

            if (labelValue == null && (!this.SeriesLabels.ShowNulls)) return null;
            if (labelValue == 0 && (!this.SeriesLabels.ShowZeros)) return null;

            if (value == null) value = 0d;
            if (labelValue == null) labelValue = 0d;

            if (float.IsNaN(radius) || float.IsNaN(radius2)) return null;

            string labelText = drawValue
                ? GetFormattedValue(series, labelValue.Value)
                : GetLabelText(series, labelValue, argumentText, tag, series.CoreTitle);

            var angleRad = (float)(Math.PI * pieAngle / 180);

            var labelPoint = GetLabelPoint(centerPie, (radius - radius2) / 1.75f + radius2, angleRad);

            if (!drawValue)
            {
                if (this is StiOutsidePieLabelsCoreXF)
                    labelPoint = GetLabelPoint(centerPie, radius + (((IStiOutsidePieLabels)this.SeriesLabels).LineLength * context.Options.Zoom), angleRad);

                if (this is StiInsideEndPieLabelsCoreXF)
                    labelPoint = GetLabelPoint(centerPie, (radius - radius2) / 1.2f + radius2, angleRad);
            }

            if (float.IsNaN(labelPoint.X))
                labelPoint.X = 0;

            if (float.IsNaN(labelPoint.Y))
                labelPoint.Y = 0;

            var font = StiFontGeom.ChangeFontSize(this.SeriesLabels.Font, this.SeriesLabels.Font.Size * context.Options.Zoom);
            var sf = this.GetStringFormatGeom(context);

            var labelRect = Rectangle.Round(GetLabelRect(context, labelPoint, labelText, font, sf));

            var angleToUse = this.SeriesLabels.Angle;
            var mode = StiRotationMode.CenterCenter;

            var rect = labelRect;
            rect.X = -rect.Width / 2;
            rect.Y = -rect.Height / 2;

            #region AutoRotate
            if (((IStiCenterPieLabels)SeriesLabels).AutoRotate)
            {
                angleToUse = pieAngle;

                if ((angleToUse > 0 && angleToUse < 90) || (angleToUse > 270))
                {
                    if (this.SeriesLabels is IStiOutsidePieLabels) mode = StiRotationMode.LeftCenter;
                    rect.X = 0;
                }
                else
                {
                    if (this.SeriesLabels is IStiOutsidePieLabels) mode = StiRotationMode.RightCenter;
                    angleToUse += 180;
                    rect.X = -rect.Width;
                }
            }
            #endregion

            if (this is StiOutsidePieLabelsCoreXF)
            {
                #region Calculate position
                if (!((IStiCenterPieLabels)SeriesLabels).AutoRotate)
                {
                    if (pieAngle > 337.5 || pieAngle < 22.5)
                    {
                        rect.X = 0;
                        rect.Y = -rect.Height / 2;
                        mode = StiRotationMode.LeftCenter;
                    }
                    else if (pieAngle >= 22.5 && pieAngle < 67.5)
                    {
                        rect.X = 0;
                        rect.Y = 0;
                        mode = StiRotationMode.LeftTop;
                    }
                    else if (pieAngle >= 67.5 && pieAngle < 112.5)
                    {
                        rect.X = -rect.Width / 2;
                        rect.Y = 0;
                        mode = StiRotationMode.CenterTop;
                    }
                    else if (pieAngle >= 112.5 && pieAngle < 157.5)
                    {
                        rect.X = -rect.Width;
                        rect.Y = 0;
                        mode = StiRotationMode.RightTop;
                    }
                    else if (pieAngle >= 157.5 && pieAngle < 202.5)
                    {
                        rect.X = -rect.Width;
                        rect.Y = -rect.Height / 2;
                        mode = StiRotationMode.RightCenter;
                    }
                    else if (pieAngle >= 202.5 && pieAngle < 247.5)
                    {
                        rect.X = -rect.Width;
                        rect.Y = -rect.Height;
                        mode = StiRotationMode.RightBottom;
                    }
                    else if (pieAngle >= 247.5 && pieAngle < 292.5)
                    {
                        rect.X = -rect.Width / 2;
                        rect.Y = -rect.Height;
                        mode = StiRotationMode.CenterBottom;
                    }
                    else if (pieAngle >= 292.5)
                    {
                        rect.X = 0;
                        rect.Y = -rect.Height;
                        mode = StiRotationMode.LeftBottom;
                    }
                }
                #endregion
            }
            else
            {
                rect.X = -rect.Width / 2;
                rect.Y = -rect.Height / 2;
            }

            if (measure)
            {
                measureRect = context.MeasureRotatedString(labelText, font, labelRect, sf, mode, angleToUse);
                return null;
            }

            var borderColor = GetBorderColor(series, colorIndex, colorCount);
            var seriesBorderColor = (Color)series.Core.GetSeriesBorderColor(colorIndex, colorCount);
            var seriesBrush = series.Core.GetSeriesBrush(colorIndex, colorCount);
            var labelBrush = new StiSolidBrush(GetLabelColor(series, colorIndex, colorCount));
            seriesBrush = ProcessSeriesColors(pointIndex, seriesBrush, series);

            var seriesLabelsBrush = ProcessSeriesColors(pointIndex, SeriesLabels.Brush, series);

            measureRect = RectangleF.Empty;

            if (this.SeriesLabels is IStiOutsidePieLabels)
            {
                var outsidePieLabels = this as StiOutsidePieLabelsCoreXF;
                var lineColor = outsidePieLabels.GetLineColor(series, colorIndex, colorCount);

                labelPoint = GetLabelPoint(centerPie, radius + (float)(((IStiOutsidePieLabels)outsidePieLabels.SeriesLabels).LineLength * context.Options.Zoom), angleRad);
                var startPoint = GetLabelPoint(centerPie, radius, angleRad);

                return new StiOutsidePieLabelsGeom(this.SeriesLabels, series, pointIndex, value.Value, rect, labelText,
                    seriesBrush, labelBrush, seriesLabelsBrush, borderColor, seriesBorderColor, mode, labelRect, angleToUse, lineColor,
                    labelPoint, startPoint);
            }
            else
            {
                return new StiCenterPieLabelsGeom(this.SeriesLabels, series, pointIndex, value.Value, rect, labelText,
                    seriesBrush, labelBrush, seriesLabelsBrush, borderColor, seriesBorderColor, mode, labelRect, angleToUse, null);
            }
        }

        public PointF GetLabelPoint(PointF centerPie, float radius, float angleRad)
        {
            return new PointF(
                centerPie.X + (float)Math.Cos(angleRad) * radius,
                centerPie.Y + (float)Math.Sin(angleRad) * radius);
        }

        public virtual RectangleF GetLabelRect(StiContext context, PointF labelPoint,
            string labelText, StiFontGeom font, StiStringFormatGeom sf)
        {
            return context.MeasureRotatedString(labelText, font,
                labelPoint, sf, StiRotationMode.CenterCenter, 0f, (int)(this.SeriesLabels.Width * context.Options.Zoom));
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

        public override int Position
        {
            get
            {
                return (int)StiSeriesLabelsPosition.CenterPie;
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

        public StiCenterPieLabelsCoreXF(IStiSeriesLabels seriesLabels)
            : base(seriesLabels)
        {
        }
    }
}
