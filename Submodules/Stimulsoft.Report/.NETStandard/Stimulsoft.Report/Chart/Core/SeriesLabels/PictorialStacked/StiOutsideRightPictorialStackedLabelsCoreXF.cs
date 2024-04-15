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
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Chart
{
    public class StiOutsideRightPictorialStackedLabelsCoreXF : StiPictorialStackedLabelsCoreXF
    {
        #region IStiApplyStyle
        public override void ApplyStyle(IStiChartStyle style)
        {
            base.ApplyStyle(style);

            if (this.SeriesLabels.AllowApplyStyle)
            {
                ((StiOutsideRightPictorialStackedLabels)this.SeriesLabels).LineColor = style.Core.SeriesLabelsLineColor;
            }
        }
        #endregion 

        #region Methods
        public override StiSeriesLabelsGeom RenderLabel(IStiSeries series, StiContext context,
            int pointIndex, double value, string argumentText, string tag,
            int colorIndex, int colorCount, float lineLength, RectangleF rect)
        {
            string labelText = GetLabelText(series, value, argumentText, tag, series.CoreTitle);

            var labelBrush = new StiSolidBrush(GetLabelColor(series, colorIndex, colorCount));
            var labelBorderColor = GetBorderColor(series, colorIndex, colorCount);
            var sf = this.GetStringFormatGeom(context);
            var font = StiFontGeom.ChangeFontSize(this.SeriesLabels.Font, this.SeriesLabels.Font.Size * context.Options.Zoom);

            var seriesBorderColor = (Color)series.Core.GetSeriesBorderColor(colorIndex, colorCount);
            var seriesBrush = series.Core.GetSeriesBrush(colorIndex, colorCount);

            if (seriesBrush != null)
                seriesBrush = new StiSolidBrush(StiBrush.ToColor(seriesBrush));

            var point = new PointF(rect.X + lineLength, rect.Y + rect.Height / 2);

            var lineColor = ((StiOutsideRightPictorialStackedLabels)this.SeriesLabels).LineColor;

            var labelRect = context.MeasureRotatedString(labelText, font, point, sf, StiRotationMode.LeftCenter, 0f, (int)(this.SeriesLabels.Width * context.Options.Zoom));
            var lineRect = new RectangleF(rect.X, rect.Y, lineLength, rect.Height);

            return new StiOutsidePictorialStackedLabelGeom(this.SeriesLabels, series, pointIndex, value, rect, labelText, seriesBrush, labelBrush, lineColor, labelBorderColor,
                seriesBorderColor, labelRect, lineRect, null);
        }
        #endregion

        #region Properties
        public override StiSeriesLabelsType SeriesLabelsType
        {
            get
            {
                return StiSeriesLabelsType.PictorialStacked;
            }
        }

        public override int Position
        {
            get
            {
                return (int)StiSeriesLabelsPosition.OutsideRightPictorialStacked;
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
                return StiLocalization.Get("PropertyMain", "Right");
            }
        }
        #endregion

        public StiOutsideRightPictorialStackedLabelsCoreXF(IStiSeriesLabels seriesLabels) : base(seriesLabels)
        {
        }
    }
}
