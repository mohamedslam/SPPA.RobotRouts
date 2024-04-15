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
    public class StiStyleCoreXF21 : StiStyleCoreXF
    {
        #region Properties.Localization
        /// <summary>
        /// Gets a localized name of style.
        /// </summary>
        public override string LocalizedName => $"{Loc.Get("Chart", "Style")}21";
        #endregion       

        #region Properties
        #region Chart
        public override StiBrush ChartBrush => new StiSolidBrush(StiColorUtils.Light(BasicStyleColor, 100));

        public override StiBrush ChartAreaBrush => new StiSolidBrush(BasicStyleColor);

        public override Color ChartAreaBorderColor => StiColor.Get("#6a6a6a");
        #endregion

        #region SeriesLabels
        public override StiBrush SeriesLabelsBrush => new StiSolidBrush(StiColorUtils.Light(BasicStyleColor, 100));

        public override Color SeriesLabelsColor => StiColor.Get("#6a6a6a");

        public override Color SeriesLabelsBorderColor => Color.White;
        #endregion

        #region Legend
        public override StiBrush LegendBrush => new StiSolidBrush(StiColorUtils.Light(BasicStyleColor, 100));

        public override Color LegendLabelsColor => StiColor.Get("#6a6a6a");
        #endregion

        #region Axis
        public override Color AxisTitleColor => StiColor.Get("#6a6a6a");

        public override Color AxisLineColor => StiColor.Get("#6a6a6a");

        public override Color AxisLabelsColor => StiColor.Get("#6a6a6a");
        #endregion

        public override Color[] StyleColors { get; } = 
        {
            StiColor.Get("#239fd9"),
            StiColor.Get("#b2b2b2")
        };

        public override Color BasicStyleColor => StiColor.Get("#666666");

        public override StiChartStyleId StyleId => StiChartStyleId.StiStyle21;
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