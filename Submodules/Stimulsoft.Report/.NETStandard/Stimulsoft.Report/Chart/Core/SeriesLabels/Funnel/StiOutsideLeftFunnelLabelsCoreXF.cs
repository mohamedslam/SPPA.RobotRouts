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

namespace Stimulsoft.Report.Chart
{
    public class StiOutsideLeftFunnelLabelsCoreXF : StiFunnelSeriesLabelsCoreXF
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

            Rectangle labelRect = Rectangle.Round(context.MeasureRotatedString(labelText, font, new PointF(0, 0), sf, StiRotationMode.CenterCenter, 0, (int)(this.SeriesLabels.Width * context.Options.Zoom)));

            measureRect.Width -= labelRect.Width;
            measureRect.X += labelRect.Width;

            PointF startPoint = new PointF(centerAxis - (float)(value + valueNext) * singleValueWidth/4, singleValueHeight * pointIndex + singleValueHeight / 2 + rect.Height * 0.05f);
            PointF endPoint = new PointF(rect.Width * 0.05f + labelRect.Width, singleValueHeight * pointIndex + singleValueHeight / 2 + rect.Height * 0.05f);

            RectangleF labelRectPosition = context.MeasureRotatedString(labelText, font,
                    endPoint, sf, StiRotationMode.RightCenter, 0f, (int)(this.SeriesLabels.Width * context.Options.Zoom));

            return new StiOutsideFunnelLabelsGeom(this.SeriesLabels, series, pointIndex, value, labelRectPosition, labelText,
                seriesBrush, labelBrush, labelBorderColor, seriesBorderColor, labelRectPosition, startPoint, endPoint);
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
                return (int)StiSeriesLabelsPosition.OutsideLeftFunnel;
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
                return StiLocalization.Get("PropertyMain", "Left");
            }
        }
        #endregion

        public StiOutsideLeftFunnelLabelsCoreXF(IStiSeriesLabels seriesLabels)
            : base(seriesLabels)
        {
        }
    }
}
