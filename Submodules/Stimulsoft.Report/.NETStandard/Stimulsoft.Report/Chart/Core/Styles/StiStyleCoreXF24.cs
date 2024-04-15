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
    public class StiStyleCoreXF24 : StiStyleCoreXF22
    {
        #region Properties.Localization
        public override string LocalizedName => $"{Loc.Get("Chart", "Style")}24";
        #endregion

        #region Properties
        public override Color[] StyleColors { get; } = 
        {
            StiColor.Get("#ed7d31"),
            StiColor.Get("#ffc000"),
            StiColor.Get("#70ad47"),
            StiColor.Get("#9e480e"),
            StiColor.Get("#997300"),
            StiColor.Get("#43682b")
        };

        public override StiChartStyleId StyleId => StiChartStyleId.StiStyle24;

        #region Chart
        public override Color ChartAreaBorderColor => Color.Transparent;
        #endregion

        #region Legend
        public override Color LegendBorderColor => Color.Transparent;
        #endregion

        #region SeriesLabels
        public override Color SeriesLabelsBorderColor => Color.Transparent;

        public override StiBrush SeriesLabelsBrush => new StiSolidBrush(Color.FromArgb(0x77, 0xff, 0xff, 0xff));
        #endregion
        #endregion
    }
}