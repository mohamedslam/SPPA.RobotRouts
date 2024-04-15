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

using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiStyleCoreXF35 : StiStyleCoreXF22
    {
        #region Properties.Localization
        public override string LocalizedName => $"{Loc.Get("Chart", "Style")}35";
        #endregion

        #region Properties
        public override Color[] StyleColors => new[]
        {
            StiColor.Get("#794d26"),
            StiColor.Get("#c7986a"),
            StiColor.Get("#c4b49a"),
            StiColor.Get("#894d29"),
            StiColor.Get("#422515"),
            StiColor.Get("#564438"),
            StiColor.Get("#876c57"),
            StiColor.Get("#d8814b"),
            StiColor.Get("#532525"),
            StiColor.Get("#59413f"),
        };

        public override StiBrush ChartBrush => new StiSolidBrush("#fefefe");

        public override StiBrush ChartAreaBrush => new StiSolidBrush("#fefefe");

        #region Chart
        public override Color ChartAreaBorderColor => Color.Transparent;
        #endregion

        #region SeriesLabels
        public override Color SeriesLabelsBorderColor => Color.Transparent;

        public override StiBrush SeriesLabelsBrush => new StiSolidBrush(StiColor.Get("#5a4941"));

        public override Color SeriesLabelsColor => StiColor.Get("#fefefe");

        public override Font SeriesLabelsFont => new Font("Arial", 10);
        #endregion

        #region Legend
        public override StiBrush LegendBrush => new StiSolidBrush(Color.Transparent);

        public override Color LegendLabelsColor => StiColor.Get("#222222");

        public override Color LegendBorderColor => Color.Transparent;

        public override Color LegendTitleColor => Color.White;

        public override bool LegendShowShadow => false;

        public override Font LegendFont => new Font("Arial", 9);
        #endregion

        #region ToolTip
        public override StiBrush ToolTipBrush => new StiSolidBrush(Color.FromArgb(150, 66, 37, 21));

        public override StiBrush ToolTipTextBrush => new StiSolidBrush(Color.White);

        public override StiSimpleBorder ToolTipBorder => new StiSimpleBorder(StiBorderSides.None, Color.White, 1, StiPenStyle.Solid);
        #endregion

        public override bool SeriesLighting => false;

        public override StiChartStyleId StyleId => StiChartStyleId.StiStyle35;
        #endregion

        #region Methods
        public override Color GetColumnBorder(Color color)
        {
            return Color.Transparent;
        }
        #endregion
    }
}