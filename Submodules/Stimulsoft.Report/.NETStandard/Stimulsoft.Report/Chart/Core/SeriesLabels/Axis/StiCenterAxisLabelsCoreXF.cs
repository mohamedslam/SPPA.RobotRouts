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

using System.Drawing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;

namespace Stimulsoft.Report.Chart
{
    public class StiCenterAxisLabelsCoreXF : StiAxisSeriesLabelsCoreXF
    {
        #region Methods
        public override StiSeriesLabelsGeom RenderLabel(IStiSeries series, StiContext context,
            PointF endPoint, PointF startPoint,
            int pointIndex, double? value,
            double? labelValue, string argumentText, string tag,
            int colorIndex, int colorCount, RectangleF rect, StiAnimation animation = null)
        {
            this.currentIndex = pointIndex;
            return RenderLabel(series, context, endPoint, startPoint, pointIndex, value, labelValue, argumentText, tag, 0, colorIndex, colorCount, rect, animation);
        }
        
        public override StiSeriesLabelsGeom RenderLabel(IStiSeries series, StiContext context,
            PointF endPoint, PointF startPoint,
            int pointIndex, double? value, 
            double? labelValue, string argumentText, string tag, double weight,
            int colorIndex, int colorCount, RectangleF rect, StiAnimation animation = null)
        {
            if (value == null && (!this.SeriesLabels.ShowNulls)) return null;
            if (value == 0 && (!this.SeriesLabels.ShowZeros)) return null;

            if (value == null) value = 0d;
            if (labelValue == null) labelValue = 0d;

            string labelText = GetLabelText(series, labelValue, argumentText, tag, series.CoreTitle, weight, false);

            var labelColor = GetLabelColor(series, colorIndex, colorCount);
            var labelBorderColor = GetBorderColor(series, colorIndex, colorCount);
            var sf = this.GetStringFormatGeom(context);
            var font = StiFontGeom.ChangeFontSize(this.SeriesLabels.Font, this.SeriesLabels.Font.Size * context.Options.Zoom);

            var seriesBorderColor = (Color)series.Core.GetSeriesBorderColor(colorIndex, colorCount);
            var seriesBrush = series.Core.GetSeriesBrush(colorIndex, colorCount);
            var seriesLabelsBrush = ProcessSeriesColors(pointIndex, SeriesLabels.Brush, series);

            var labelRect = GetLabelRect(context, endPoint, startPoint, value, labelText, true, font, sf);
            var animationLabel = animation as StiLabelAnimation;
            if (animationLabel != null)
            {
                animationLabel.LabelRect = GetLabelRect(context, animationLabel.PointFrom, startPoint, animationLabel.ValueFrom, animationLabel.ValueFrom.ToString(), true, font, sf);
            }                

            return new StiCenterAxisLabelsGeom(this.SeriesLabels, series, pointIndex, value.Value, labelRect, labelText, labelColor, labelBorderColor,
                seriesBrush, seriesLabelsBrush, seriesBorderColor, font, animation);
        }

        protected virtual RectangleF GetLabelRect(StiContext context, PointF endPoint, PointF startPoint,
            double? value, string labelText, bool checkHeight, StiFontGeom font, StiStringFormatGeom sf)
        {
            if (this.SeriesLabels.Chart.Area.Core.SeriesOrientation == StiChartSeriesOrientation.Vertical)
            {
                return context.MeasureRotatedString(labelText, font,
                    new PointF(endPoint.X, (endPoint.Y + startPoint.Y) / 2),
                    sf, StiRotationMode.CenterCenter, 0f, (int)(this.SeriesLabels.Width * context.Options.Zoom));
            }
            else
            {
                return context.MeasureRotatedString(labelText, font,
                    new PointF((endPoint.X + startPoint.X) / 2, endPoint.Y),
                    sf, StiRotationMode.CenterCenter, 0f, (int)(this.SeriesLabels.Width * context.Options.Zoom));
            }
        }
        #endregion        

        #region Properties
        public override int Position
        {
            get
            {
                return (int)StiSeriesLabelsPosition.CenterAxis;
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

        public StiCenterAxisLabelsCoreXF(IStiSeriesLabels seriesLabels)
            : base(seriesLabels)
        {            
        }
    }
}
