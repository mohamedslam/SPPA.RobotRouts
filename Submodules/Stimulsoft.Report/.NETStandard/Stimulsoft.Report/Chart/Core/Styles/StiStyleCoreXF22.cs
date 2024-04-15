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

namespace Stimulsoft.Report.Chart
{
    public class StiStyleCoreXF22 : StiStyleCoreXF
    {
        #region Properties.Localization
        public override string LocalizedName => $"{Loc.Get("Chart", "Style")}22";
        #endregion

        #region Properties
        #region Chart
        public override StiBrush ChartBrush => new StiSolidBrush(StiColorUtils.Light(BasicStyleColor, 100));

        public override StiBrush ChartAreaBrush => new StiSolidBrush(BasicStyleColor);

        public override Color ChartAreaBorderColor => StiColor.Get("#abacad");
        #endregion

        #region SeriesLabels
        public override StiBrush SeriesLabelsBrush => new StiSolidBrush(StiColorUtils.Light(BasicStyleColor, 100));

        public override Color SeriesLabelsColor => StiColor.Get("#5a5a5a");

        public override Color SeriesLabelsBorderColor => StiColor.Get("#8c8c8c");
        #endregion

        #region Legend
        public override StiBrush LegendBrush => new StiSolidBrush(StiColorUtils.Light(BasicStyleColor, 100));

        public override Color LegendLabelsColor => StiColor.Get("#8c8c8c");
        #endregion

        #region Axis
        public override Color AxisTitleColor => StiColor.Get("#8c8c8c");

        public override Color AxisLineColor => StiColor.Get("#8c8c8c");

        public override Color AxisLabelsColor => StiColor.Get("#8c8c8c");
        #endregion

        #region GridLines
        public override Color GridLinesHorColor => Color.FromArgb(50, StiColorUtils.Dark(BasicStyleColor, 150));

        public override Color GridLinesVertColor => Color.FromArgb(50, StiColorUtils.Dark(BasicStyleColor, 150));
        #endregion

        #region ToolTip
        public override StiBrush ToolTipBrush => new StiSolidBrush(Color.White);

        public override StiBrush ToolTipTextBrush => new StiSolidBrush(Color.DimGray);

        public override StiSimpleBorder ToolTipBorder => new StiSimpleBorder(StiBorderSides.All, StiColor.Get("#8c8c8c"), 1, StiPenStyle.Solid);
        #endregion

        public override Color[] StyleColors { get; } = 
        {
            StiColor.Get("#5b9bd5"),
            StiColor.Get("#ed7d31"),
            StiColor.Get("#9f9f9f"),
            StiColor.Get("#ffc000"),
            StiColor.Get("#4472c4"),
            StiColor.Get("#70ad47")
        };

        public override Color BasicStyleColor => Color.White;

        public override StiChartStyleId StyleId => StiChartStyleId.StiStyle22;
        #endregion

        #region Methods
        public override StiBrush GetColumnBrush(Color color)
        {
            return new StiSolidBrush(color);
        }

        public override Color GetColumnBorder(Color color)
        {
            return StiColorUtils.Light(color, 255);
        }
        #endregion
    }
}