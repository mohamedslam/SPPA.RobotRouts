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
using Stimulsoft.Base.Localization;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiOutsideAxisLabelsCoreXF3D : StiCenterAxisLabelsCoreXF3D
    {
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

        #region Methods
        public override StiCellGeom RenderLabel3D(StiRender3D render3D, IStiSeries series, StiContext context, StiRectangle3D rect3D,
            int pointIndex, double? value, double? labelValue,
            string argumentText, string tag, double weight,
            int colorIndex, int colorCount)
        {

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

            var point3D = GetPointLabel(rect3D, value);

            return new StiOutsideAxisLabelsGeom3D(this.SeriesLabels, series, pointIndex, value.Value, labelText, labelColor, labelBorderColor,
                seriesBrush, seriesLabelsBrush, seriesBorderColor, font, point3D, render3D);
        }

        private StiPoint3D GetPointLabel(StiRectangle3D rect3D, double? value)
        {
            var height = value.GetValueOrDefault() > 0 ? rect3D.Height : -rect3D.Height;
            return new StiPoint3D(rect3D.X + rect3D.Length / 2, rect3D.Y + height, rect3D.Front);
        } 
        #endregion

        public StiOutsideAxisLabelsCoreXF3D(IStiSeriesLabels seriesLabels) 
            : base(seriesLabels)
        {
        }
    }
}
