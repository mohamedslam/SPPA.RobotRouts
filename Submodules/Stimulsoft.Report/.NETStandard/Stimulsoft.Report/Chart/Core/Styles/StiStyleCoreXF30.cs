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
    public class StiStyleCoreXF30 : StiStyleCoreXF22
    {
        #region Properties.Localization
        public override string LocalizedName => $"{Loc.Get("Chart", "Style")}30";
        #endregion

        #region Properties
        public override Color[] StyleColors => new[]
        {
            StiColor.Get("db7b46"),
            StiColor.Get("d3d3d5"),
            StiColor.Get("6b6e75"),
            StiColor.Get("4a4c55"),
        };

        public override StiBrush ChartBrush => new StiSolidBrush("595b65");

        public override StiBrush ChartAreaBrush => new StiSolidBrush("595b65");

        #region Chart
        public override Color ChartAreaBorderColor => Color.Transparent;
        #endregion

        #region SeriesLabels
        public override Color SeriesLabelsBorderColor => Color.Transparent;

        public override StiBrush SeriesLabelsBrush => new StiSolidBrush(Color.FromArgb(0x77, 0xff, 0xff, 0xff));

        public override Color SeriesLabelsColor => Color.White;

        public override Font SeriesLabelsFont => new Font("Arial", 10);
        #endregion

        #region Legend
        public override StiBrush LegendBrush => new StiSolidBrush(Color.Transparent);

        public override Color LegendLabelsColor => Color.White;

        public override Color LegendBorderColor => Color.Transparent;

        public override Color LegendTitleColor => Color.White;

        public override bool LegendShowShadow => false;

        public override Font LegendFont => new Font("Arial", 9);
        #endregion

        public override bool SeriesLighting => false;

        public override StiChartStyleId StyleId => StiChartStyleId.StiStyle30;
        #endregion

        #region Methods
        public override Color GetColumnBorder(Color color)
        {
            return Color.Transparent;
        }
        #endregion
    }
}