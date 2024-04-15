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
    public class StiStyleCoreXF28 : StiStyleCoreXF26
    {
        #region Properties.Localization
        public override string LocalizedName => $"{Loc.Get("Chart", "Style")}28";
        #endregion

        #region Properties
        public override Color[] StyleColors => new[]
        {
            StiColor.Get("#165d9e"),
            StiColor.Get("#577eb6"),
            StiColor.Get("#569436"),
            StiColor.Get("#225056"),
            StiColor.Get("#d4dae0")
        };

        public override StiBrush ChartBrush => new StiSolidBrush("#0a325a");

        public override StiBrush ChartAreaBrush => new StiSolidBrush("#0a325a");

        #region Axis
        public override Color AxisTitleColor => StiColorUtils.Dark(BasicStyleColor, 50);

        public override Color AxisLineColor => StiColorUtils.Dark(BasicStyleColor, 50);

        public override Color AxisLabelsColor => StiColorUtils.Dark(BasicStyleColor, 50);
        #endregion
        
        #region Legend
        public override StiBrush LegendBrush => new StiSolidBrush(Color.Transparent);

        public override Color LegendLabelsColor => Color.White;

        public override Color LegendBorderColor => Color.Transparent;

        public override Color LegendTitleColor => Color.White;

        public override bool LegendShowShadow => false;

        public override Font LegendFont => new Font("Arial", 9);
        #endregion

        #region ToolTip
        public override StiBrush ToolTipBrush => new StiSolidBrush(Color.FromArgb(150, 6, 32, 59));

        public override StiBrush ToolTipTextBrush => new StiSolidBrush(Color.White);

        public override StiSimpleBorder ToolTipBorder => new StiSimpleBorder(StiBorderSides.All, Color.White, 1, StiPenStyle.Solid);
        #endregion

        public override StiChartStyleId StyleId => StiChartStyleId.StiStyle28;
        #endregion
    }
}