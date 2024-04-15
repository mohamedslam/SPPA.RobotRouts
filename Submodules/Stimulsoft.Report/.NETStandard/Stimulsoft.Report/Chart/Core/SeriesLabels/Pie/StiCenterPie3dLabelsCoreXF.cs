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

using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiCenterPie3dLabelsCoreXF : StiSeriesLabelsCoreXF
    {
        #region Fields
        [Browsable(false)]
        public float percentPerValue = 0.0f;
        #endregion

        #region Properties
        public override StiSeriesLabelsType SeriesLabelsType
        {
            get
            {
                return StiSeriesLabelsType.Pie3d;
            }
        }
        public override int Position
        {
            get
            {
                return (int)StiSeriesLabelsPosition.CenterPie3d;
            }
        }

        public override string LocalizedName
        {
            get
            {
                return $"3D {StiLocalization.Get("Chart", "LabelsCenter")}";
            }
        }
        #endregion

        #region Methods
        internal StiSeriesLabelsGeom RenderLabel(IStiSeries series, StiContext context, StiPie3dSlice pie3dSlice)
        {
            string labelText = GetLabelText(series, pie3dSlice.Value, pie3dSlice.ArgumentText, pie3dSlice.Tag, series.CoreTitle);
            
            var font = StiFontGeom.ChangeFontSize(this.SeriesLabels.Font, this.SeriesLabels.Font.Size * context.Options.Zoom);
            var sf = this.GetStringFormatGeom(context);

            var labelRect = Rectangle.Round(GetLabelRect(context, pie3dSlice.TextPosition, labelText, font, sf));

            var rect = labelRect; 
            rect.X = -rect.Width / 2;
            rect.Y = -rect.Height / 2;

            var borderColor = GetBorderColor(series, pie3dSlice.ColorIndex, pie3dSlice.ColorCount);
            var seriesBorderColor = (Color)series.Core.GetSeriesBorderColor(pie3dSlice.ColorIndex, pie3dSlice.ColorCount);
            var seriesBrush = series.Core.GetSeriesBrush(pie3dSlice.ColorIndex, pie3dSlice.ColorCount);
            var labelBrush = new StiSolidBrush(GetLabelColor(series, pie3dSlice.ColorIndex, pie3dSlice.ColorCount));
            seriesBrush = ProcessSeriesColors(pie3dSlice.Index, seriesBrush, series);
            var seriesLabelsBrush = ProcessSeriesColors(pie3dSlice.Index, SeriesLabels.Brush, series);

            return new StiCenterPieLabelsGeom(this.SeriesLabels, series, pie3dSlice.Index, pie3dSlice.Value, rect, labelText,
                    seriesBrush, labelBrush, seriesLabelsBrush, borderColor, seriesBorderColor, StiRotationMode.CenterCenter, labelRect, this.SeriesLabels.Angle, null);
        }

        public virtual RectangleF GetLabelRect(StiContext context, PointF labelPoint,
            string labelText, StiFontGeom font, StiStringFormatGeom sf)
        {
            return context.MeasureRotatedString(labelText, font,
                labelPoint, sf, StiRotationMode.CenterCenter, 0f, (int)(this.SeriesLabels.Width * context.Options.Zoom));
        }

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


        public StiCenterPie3dLabelsCoreXF(IStiSeriesLabels seriesLabels)
               : base(seriesLabels)
        {
        }
    }
}
