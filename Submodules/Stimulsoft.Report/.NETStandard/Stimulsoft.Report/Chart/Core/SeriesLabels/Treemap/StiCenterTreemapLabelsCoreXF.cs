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

using System.ComponentModel;
using System.Drawing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using System;

namespace Stimulsoft.Report.Chart
{
    public class StiCenterTreemapLabelsCoreXF : StiSeriesLabelsCoreXF
    {
        #region Methods
        public StiSeriesLabelsGeom RenderLabel(IStiSeries series, StiContext context,
            int pointIndex, double value, string argumentText, string tag,
            int colorIndex, int colorCount, RectangleF rect, StiAnimation animation = null)
        {
            string labelText = GetLabelText(series, value, argumentText, tag, series.CoreTitle);
            
            var labelColor = GetLabelColor(series, colorIndex, colorCount);
            var labelBorderColor = GetBorderColor(series, colorIndex, colorCount);
            var sf = this.GetStringFormatGeom(context);
            var font = StiFontGeom.ChangeFontSize(this.SeriesLabels.Font, this.SeriesLabels.Font.Size * context.Options.Zoom);

            var seriesBorderColor = (Color)series.Core.GetSeriesBorderColor(colorIndex, colorCount);
            var seriesBrush = series.Core.GetSeriesBrush(colorIndex, colorCount);
            var seriesLabelsBrush = ProcessSeriesColors(pointIndex, SeriesLabels.Brush, series);

            var labelRect = GetLabelRect(context, rect, value, labelText, true, font, sf);

            return new StiCenterTreemapLabelsGeom(this.SeriesLabels, series, pointIndex, value, labelRect, labelText, labelColor, labelBorderColor,
                seriesBrush, seriesLabelsBrush, seriesBorderColor, font, animation);            
        }

        protected virtual RectangleF GetLabelRect(StiContext context, RectangleF rect,
            double? value, string labelText, bool checkHeight, StiFontGeom font, StiStringFormatGeom sf)
        {
            return context.MeasureRotatedString(labelText, font,
                new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2),
                sf, StiRotationMode.CenterCenter, 0f, (int)(this.SeriesLabels.Width * context.Options.Zoom));
        }
        #endregion        

        #region Properties
        public override int Position
        {
            get
            {
                return (int)StiSeriesLabelsPosition.CenterTreemap;
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

        public override StiSeriesLabelsType SeriesLabelsType
        {
            get
            {
                return StiSeriesLabelsType.Treemap;
            }
        }
        #endregion

        public StiCenterTreemapLabelsCoreXF(IStiSeriesLabels seriesLabels)
            : base(seriesLabels)
        {
        }
    }
}
