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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiStyleCoreXF26 : StiStyleCoreXF22
    {
        #region Properties.Localization
        public override string LocalizedName => $"{Loc.Get("Chart", "Style")}26";
        #endregion

        #region Properties
        public override Color[] StyleColors => new[]
        {
            StiColor.Get("#2ec6c8"),
            StiColor.Get("#b5a1dd"),
            StiColor.Get("#5ab0ee"),
            StiColor.Get("#f4984e"),
            StiColor.Get("#d77a80"),
            StiColor.Get("#d04456"),
        };

        public override StiBrush ChartAreaBrush => new StiSolidBrush("#ffffff");

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

        #region Marker
        public override bool MarkerVisible => false;
        #endregion

        public override StiChartStyleId StyleId => StiChartStyleId.StiStyle26;
        #endregion

        #region Methods
        public override Color GetColumnBorder(Color color)
        {
            return Color.Transparent;
        }
        #endregion
    }
}