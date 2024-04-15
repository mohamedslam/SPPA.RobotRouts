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
using System.ComponentModel;
using System.Drawing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Serializing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Chart
{
	public abstract class StiStyleCoreXF
	{
        #region Properties.Localization
        /// <summary>
        /// Gets a localized name of style.
        /// </summary>
        public abstract string LocalizedName { get; }
        #endregion

        #region Properties
        [StiNonSerialized]
        public virtual StiChartStyleId StyleId => StiChartStyleId.StiStyle01;

        #region Chart
        public virtual StiBrush ChartBrush => new StiSolidBrush(StiColorUtils.Light(BasicStyleColor, 100));

        public virtual StiBrush ChartAreaBrush => new StiGradientBrush(
                    StiColorUtils.Light(BasicStyleColor, 80),
                    StiColorUtils.Light(BasicStyleColor, 40),
                    90f);

		public virtual Color ChartAreaBorderColor => StiColorUtils.Dark(BasicStyleColor, 150);

        public virtual int ChartAreaBorderThickness => 1;

        [Browsable(false)]
        public IStiChart Chart { get; set; }

        public virtual bool ChartAreaShowShadow => false;
        #endregion

        #region SeriesLabels
        public virtual StiBrush SeriesLabelsBrush => new StiSolidBrush(BasicStyleColor);

        public virtual Color SeriesLabelsColor => StiColorUtils.Dark(BasicStyleColor, 150);

        public virtual Color SeriesLabelsBorderColor => StiColorUtils.Dark(BasicStyleColor, 150);

        public virtual Color SeriesLabelsLineColor => StiColorUtils.Dark(BasicStyleColor, 150);

        public virtual Font SeriesLabelsFont => new Font("Arial", 7);
        #endregion

        #region ToolTip
        public virtual StiBrush ToolTipBrush => new StiSolidBrush(Color.FromArgb(180, 50, 50, 50));

        public virtual StiBrush ToolTipTextBrush => new StiSolidBrush(Color.White);

        public virtual StiCornerRadius ToolTipCornerRadius => new StiCornerRadius(8);

        public virtual StiSimpleBorder ToolTipBorder => new StiSimpleBorder();
        #endregion

        #region Trend Line
        public virtual Color TrendLineColor => StiColorUtils.Dark(BasicStyleColor, 150);

        public virtual bool TrendLineShowShadow => false;
        #endregion

        #region Legend
        public virtual StiBrush LegendBrush => new StiGradientBrush(
                    StiColorUtils.Light(BasicStyleColor, 80),
                    StiColorUtils.Light(BasicStyleColor, 20),
                    90f);

		public virtual Color LegendLabelsColor => StiColorUtils.Dark(BasicStyleColor, 150);

        public virtual Color LegendBorderColor => StiColorUtils.Dark(BasicStyleColor, 100);

        public virtual Color LegendTitleColor => StiColorUtils.Dark(BasicStyleColor, 150);

        public virtual bool LegendShowShadow { get; }

        public virtual Font LegendFont => new Font("Arial", 8);
        #endregion

        #region Axis
        public virtual Color AxisTitleColor => StiColorUtils.Dark(BasicStyleColor, 150);

        public virtual Color AxisLineColor => StiColorUtils.Dark(BasicStyleColor, 150);

        public virtual Color AxisLabelsColor => StiColorUtils.Dark(BasicStyleColor, 150);
        #endregion

		#region Interlacing
		public virtual StiBrush InterlacingHorBrush => new StiSolidBrush(Color.FromArgb(10, StiColorUtils.Dark(BasicStyleColor, 100)));

		public virtual StiBrush InterlacingVertBrush => new StiSolidBrush(Color.FromArgb(10, StiColorUtils.Dark(BasicStyleColor, 100)));
		#endregion	

		#region GridLines
		public virtual Color GridLinesHorColor => 
            Color.FromArgb(100, StiColorUtils.Dark(BasicStyleColor, 150));

        public virtual Color GridLinesVertColor => 
            Color.FromArgb(100, StiColorUtils.Dark(BasicStyleColor, 150));
        #endregion

        #region Series
        public virtual bool SeriesLighting => true;

        public virtual bool SeriesShowShadow { get; }

        public virtual bool SeriesShowBorder { get; } = true;

        public virtual int SeriesBorderThickness { get; } = 1;

        public virtual StiCornerRadius SeriesCornerRadius { get; } = new StiCornerRadius();
        #endregion

        #region Marker
        public virtual bool MarkerVisible { get; set; } = true;
        #endregion

        public virtual Color FirstStyleColor => StyleColors[0];

        public virtual Color LastStyleColor => StyleColors[StyleColors.Length - 1];

        public abstract Color[] StyleColors { get; }

		public abstract Color BasicStyleColor { get; }
        #endregion		

		#region Methods
        public virtual void FillColumn(StiContext context, RectangleF rect, StiCornerRadius cornerRadius, StiBrush brush, StiInteractionDataGeom interaction, int elementIndex = -1)
		{
            context.FillCicledRectangle(brush, rect, cornerRadius, interaction, elementIndex);
		}

		public virtual StiBrush GetAreaBrush(Color color)
		{
			return new StiSolidBrush(Color.FromArgb(200, color));
		}

		public virtual StiBrush GetColumnBrush(Color color)
		{
			return new StiGradientBrush(color, StiColorUtils.Dark(color, 50), 0); ;
		}

		public virtual Color GetColumnBorder(Color color)
		{
            if (!SeriesShowBorder)
                return Color.Transparent;

            return StiColorUtils.Dark(color, 100);
		}
        
		public virtual Color[] GetColors(int seriesCount, Color[] seriesColors = null)
		{
			var colors = new Color[seriesCount];
			var styleColors = seriesColors != null ? seriesColors : StyleColors;
            
			int styleColorIndex = 0;
			int dist = 0;

            var arrayColorHash = new Color[styleColors.Length];
            var passIndex = 0;

			for (int colorIndex = 0; colorIndex < seriesCount; colorIndex++)
			{
                if (styleColors.Length == 0 || styleColors.Length < styleColorIndex)
                {
                    colors[colorIndex] = Color.FromArgb(255, 112, 173, 71);
                }
                else
                {
                    if (passIndex > 1)
                    {
                        colors[colorIndex] = arrayColorHash[styleColorIndex];
                    }
                    else
                    {
                        if (dist != 0)
                        {
                            var color = styleColors[styleColorIndex];

                            int a = Math.Min(color.A + dist, 255);
                            int r = Math.Min(color.R + dist, 255);
                            int g = Math.Min(color.G + dist, 255);
                            int b = Math.Min(color.B + dist, 255);

                            var currentColor = Color.FromArgb(a, r, g, b);
                            colors[colorIndex] = currentColor;
                            arrayColorHash[styleColorIndex] = currentColor;
                        }
                        else
                        {
                            colors[colorIndex] = styleColors[styleColorIndex];
                        }
                    }
                }

				styleColorIndex++;
				if (styleColorIndex == styleColors.Length)
				{
                    passIndex++;
                    styleColorIndex = 0;
					dist = 50;
				}
			}
			return colors;
		}

		public Color GetColorByIndex(int index, int count, Color[] seriesColors)
		{
			return GetColors(count, seriesColors)[index];
		}

        public Color GetColorBySeries(IStiSeries series, Color[] seriesColors = null)
		{
			return GetColors(series.Chart.Series.Count, seriesColors)[series.Chart.Series.IndexOf(series)];
		}
        #endregion
	}
}
