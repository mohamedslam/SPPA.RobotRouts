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


namespace Stimulsoft.Report.Chart
{
	public class StiCustomStyleCoreXF : StiStyleCoreXF01
	{
        #region Properties.Localization
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string LocalizedName => "CustomStyle";
        #endregion

        #region Fields
        public Stimulsoft.Report.StiChartStyle ReportChartStyle = null;
        #endregion

        #region Properties
        public override StiBrush ChartBrush
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.Brush 
                    : base.ChartBrush;
            }
        }

        #region Area
        public override StiBrush ChartAreaBrush
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.ChartAreaBrush 
                    : base.ChartAreaBrush;
            }
        }

        public override Color ChartAreaBorderColor
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.ChartAreaBorderColor 
                    : base.ChartAreaBorderColor;
            }
        }

        public override int ChartAreaBorderThickness
        {
            get
            {
                return ReportStyle != null
                    ? ReportStyle.ChartAreaBorderThickness
                    : base.ChartAreaBorderThickness;
            }
        }

        public override bool ChartAreaShowShadow
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.ChartAreaShowShadow 
                    : base.ChartAreaShowShadow;
            }
        }
        #endregion

        #region Properties
        public override StiBrush ToolTipBrush
        {
            get
            {
                return ReportStyle != null
                    ? ReportStyle.ToolTipBrush
                    : base.ToolTipBrush;
            }
        }

        public override StiBrush ToolTipTextBrush
        {
            get
            {
                return ReportStyle != null
                    ? ReportStyle.ToolTipTextBrush
                    : base.ToolTipTextBrush;
            }
        }

        public override StiCornerRadius ToolTipCornerRadius
        {
            get
            {
                return ReportStyle != null
                    ? ReportStyle.ToolTipCornerRadius
                    : base.ToolTipCornerRadius;
            }
        }

        public override StiSimpleBorder ToolTipBorder
        {
            get
            {
                return ReportStyle != null
                    ? ReportStyle.ToolTipBorder
                    : base.ToolTipBorder;
            }
        }
        #endregion

        #region Series

        public override bool SeriesLighting
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.SeriesLighting 
                    : base.SeriesLighting;
            }
        }

        public override bool SeriesShowShadow
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.SeriesShowShadow 
                    : base.SeriesShowShadow;
            }
        }

        public override bool SeriesShowBorder
        {
            get
            {
                return ReportStyle != null
                    ? ReportStyle.SeriesShowBorder
                    : base.SeriesShowBorder;
            }
        }
        #endregion

        #region SeriesLabels
        public override StiBrush SeriesLabelsBrush
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.SeriesLabelsBrush 
                    : base.SeriesLabelsBrush;
            }
        }

        public override Color SeriesLabelsColor
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.SeriesLabelsColor 
                    : base.SeriesLabelsColor;
            }
        }

        public override Color SeriesLabelsBorderColor
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.SeriesLabelsBorderColor 
                    : base.SeriesLabelsBorderColor;
            }
        }

        public override Color SeriesLabelsLineColor
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.SeriesLabelsLineColor 
                    : base.SeriesLabelsLineColor;
            }
        }

        public override int SeriesBorderThickness
        {
            get
            {
                return ReportStyle != null
                    ? (int)ReportStyle.SeriesBorderThickness
                    : base.SeriesBorderThickness;
            }
        }

        public override StiCornerRadius SeriesCornerRadius
        {
            get
            {
                return ReportStyle != null
                    ? ReportStyle.SeriesCornerRadius
                    : base.SeriesCornerRadius;
            }
        }
        #endregion

        #region Trend Line
        public override Color TrendLineColor
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.TrendLineColor 
                    : base.TrendLineColor;
            }
        }

        public override bool TrendLineShowShadow
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.TrendLineShowShadow 
                    : base.TrendLineShowShadow;
            }
        }
        #endregion

        #region Legend
        public override StiBrush LegendBrush
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.LegendBrush 
                    : base.LegendBrush;
            }
        }

        public override Color LegendLabelsColor
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.LegendLabelsColor 
                    : base.LegendLabelsColor;
            }
        }

        public override Color LegendBorderColor
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.LegendBorderColor 
                    : base.LegendBorderColor;
            }
        }

        public override Color LegendTitleColor
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.LegendTitleColor 
                    : base.LegendTitleColor;
            }
        }
        #endregion

        #region Marker
        public override bool MarkerVisible
    {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.MarkerVisible 
                    : base.MarkerVisible;
            }
        }
        #endregion

        #region Axis
        public override Color AxisTitleColor
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.AxisTitleColor 
                    : base.AxisTitleColor;
            }
        }

        public override Color AxisLineColor
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.AxisLineColor 
                    : base.AxisLineColor;
            }
        }

        public override Color AxisLabelsColor
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.AxisLabelsColor 
                    : base.AxisLabelsColor;
            }
        }
	    #endregion

        #region Interlacing
        public override StiBrush InterlacingHorBrush
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.InterlacingHorBrush 
                    : base.InterlacingHorBrush;
            }
        }

        public override StiBrush InterlacingVertBrush
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.InterlacingVertBrush 
                    : base.InterlacingVertBrush;
            }
        }
        #endregion

        #region GridLines
        public override Color GridLinesHorColor
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.GridLinesHorColor 
                    : base.GridLinesHorColor;
            }
        }

        public override Color GridLinesVertColor
        {
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.GridLinesVertColor 
                    : base.GridLinesVertColor;
            }
        }
        #endregion

        public override Color[] StyleColors
		{
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.StyleColors 
                    : base.StyleColors;
            }
		}

		public override Color BasicStyleColor
		{
            get
            {
                return ReportStyle != null 
                    ? ReportStyle.BasicStyleColor 
                    : base.BasicStyleColor;
            }
		}

        public string ReportStyleName { get; set; }

        public Stimulsoft.Report.StiChartStyle ReportStyle
        {
            get
            {
				var styleName = this.ReportStyleName == null ? ((StiChart)this.Chart).CustomStyleName : this.ReportStyleName;

				if (Chart == null || ((StiChart)Chart).Report == null || styleName == null || styleName.Length == 0)
                    return null;

				return ((StiChart)Chart).Report.Styles[styleName] as Stimulsoft.Report.StiChartStyle;
            }
        }

        public StiCustomStyle CustomStyle { get; }
        #endregion

		#region Methods
		public override StiBrush GetColumnBrush(Color color)
		{
            if (this.ReportStyle == null)
                return new StiSolidBrush(color);

            switch (this.ReportStyle.BrushType)
            {
                case StiBrushType.Glare:
                    return new StiGlareBrush(StiColorUtils.Dark(color, 50), color, 0);

                case StiBrushType.Gradient0:
                    return new StiGradientBrush(StiColorUtils.Dark(color, 50), color, 0);

                case StiBrushType.Gradient90:
                    return new StiGradientBrush(StiColorUtils.Dark(color, 50), color, 90);

                case StiBrushType.Gradient180:
                    return new StiGradientBrush(StiColorUtils.Dark(color, 50), color, 180);

                case StiBrushType.Gradient270:
                    return new StiGradientBrush(StiColorUtils.Dark(color, 50), color, 270);

                case StiBrushType.Gradient45:
                    return new StiGradientBrush(StiColorUtils.Dark(color, 50), color, 45);

                case StiBrushType.Solid:
                    return new StiSolidBrush(color);
            }

            return new StiSolidBrush(color);
		}
		#endregion

        public StiCustomStyleCoreXF(StiCustomStyle customStyle)
        {
            this.CustomStyle = customStyle;
        }
	}
}