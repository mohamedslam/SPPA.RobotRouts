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
    public class StiStyleCoreXF25 : StiStyleCoreXF22
    {
        #region Properties.Localization
        public override string LocalizedName => $"{Loc.Get("Chart", "Style")}25";
        #endregion

        #region Properties
        public override Color[] StyleColors { get; } = 
        {
            StiColor.Get("#70ad47"),
            StiColor.Get("#4472c4"),
            StiColor.Get("#ffc000"),
            StiColor.Get("#43682b"),
            StiColor.Get("#fd6a37"),
            StiColor.Get("#997300")
        };

        public override StiChartStyleId StyleId => StiChartStyleId.StiStyle25;

        #region Chart
        public override Color ChartAreaBorderColor => Color.Transparent;
        #endregion

        #region Legend
        public override bool LegendShowShadow => false;

        public override Color LegendBorderColor => Color.Transparent;
        #endregion

        #region SeriesLabels
        public override Color SeriesLabelsBorderColor => Color.Transparent;

        public override StiBrush SeriesLabelsBrush => new StiSolidBrush(Color.FromArgb(0x77, 0xff, 0xff, 0xff));

        public override Color SeriesLabelsColor => StiColor.Get("#33475B");

        public override Font SeriesLabelsFont => new Font("Arial", 10);
        #endregion

        #region Series
        public override bool SeriesLighting => false;

        public override bool SeriesShowShadow => false;
        #endregion
        #endregion
    }
}
