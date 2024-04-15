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
    public class StiCenterPictorialStackedLabelsCoreXF : StiPictorialStackedLabelsCoreXF
    {
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

            var point = new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

            var labelRect = context.MeasureRotatedString(labelText, font, point, sf, StiRotationMode.CenterCenter, 0f, (int)(this.SeriesLabels.Width * context.Options.Zoom));

            return new StiCenterFunnelLabelsGeom(this.SeriesLabels, series, pointIndex, value, labelRect, labelText,
                seriesBrush, labelBrush, labelBorderColor, seriesBorderColor, labelRect, null);
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
                return (int)StiSeriesLabelsPosition.CenterPictorialStacked;
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

        public StiCenterPictorialStackedLabelsCoreXF(IStiSeriesLabels seriesLabels) 
            : base(seriesLabels)
        {
        }
    }
}
