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
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;

namespace Stimulsoft.Report.Chart
{
    public class StiOutsideAxisLabelsCoreXF : StiAxisSeriesLabelsCoreXF
    {
        #region IStiApplyStyle
        public override void ApplyStyle(IStiChartStyle style)
        {
            base.ApplyStyle(style);

            if (this.SeriesLabels.AllowApplyStyle)
            {
                ((IStiOutsideAxisLabels)this.SeriesLabels).LineColor = style.Core.SeriesLabelsLineColor;
            }
        }
        #endregion   

        #region Methods
        public override StiSeriesLabelsGeom RenderLabel(IStiSeries series, StiContext context,
            PointF endPoint, PointF startPoint,
            int pointIndex, double? value, 
            double? labelValue, string argumentText, string tag,
            int colorIndex, int colorCount, RectangleF rect, StiAnimation animation = null)
        {
            var labels = this.SeriesLabels as IStiOutsideAxisLabels;

            if (value == null && (!labels.ShowNulls)) return null;
            if (value == 0 && (!labels.ShowZeros)) return null;

            if (value == null) value = 0d;
            if (labelValue == null) labelValue = 0d;

            string labelText = GetLabelText(series, labelValue, argumentText, tag, series.CoreTitle);

            float infoAngle = 90;

            if (this.SeriesLabels.Chart.Area is IStiStackedBarArea)
            {
                if (value <= 0) infoAngle = -infoAngle;
                infoAngle += 90;
            }
            else
            {
                if (value < 0) infoAngle = -infoAngle;
            }

            infoAngle *= (float)Math.PI / 180;

            var newPoint = new Point(
                (int)Math.Round(endPoint.X + labels.LineLength * Math.Cos(infoAngle) * context.Options.Zoom),
                (int)Math.Round(endPoint.Y - labels.LineLength * Math.Sin(infoAngle) * context.Options.Zoom));

            var font = StiFontGeom.ChangeFontSize(labels.Font, labels.Font.Size * context.Options.Zoom);

            var sf = this.GetStringFormatGeom(context);

            var labelSize = Size.Round(context.MeasureString(labelText, font, (int)(this.SeriesLabels.Width * context.Options.Zoom), sf));
            var labelRect = new Rectangle(
                newPoint.X - labelSize.Width / 2,
                newPoint.Y - labelSize.Height / 2,
                labelSize.Width + 1,
                labelSize.Height + 1);

            var labelColor = GetLabelColor(series, colorIndex, colorCount);
            var labelBorderColor = GetBorderColor(series, colorIndex, colorCount);

            var seriesBorderColor = (Color)series.Core.GetSeriesBorderColor(colorIndex, colorCount);
            var seriesBrush = series.Core.GetSeriesBrush(colorIndex, colorCount);
            seriesBrush = ProcessSeriesColors(pointIndex, seriesBrush, series);

            return new StiOutsideAxisLabelsGeom(this.SeriesLabels, series, pointIndex, value.Value, labelRect, labelText,
                labelColor, labelBorderColor, labels.LineColor, seriesBrush, seriesBorderColor, font, newPoint, endPoint, null);
        }
        #endregion

        #region Properties
        public override int Position
        {
            get
            {
                return (int)StiSeriesLabelsPosition.OutsideAxis;
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
                return StiLocalization.Get("Chart", "LabelsOutside");
            }
        }
        #endregion

        public StiOutsideAxisLabelsCoreXF(IStiSeriesLabels seriesLabels)
            : base(seriesLabels)
        {            
        }
	}
}
