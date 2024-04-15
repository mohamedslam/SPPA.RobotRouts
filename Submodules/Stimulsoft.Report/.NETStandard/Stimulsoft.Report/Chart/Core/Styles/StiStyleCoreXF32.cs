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
    public class StiStyleCoreXF32 : StiStyleCoreXF
    {
        #region Properties.Localization
        public override string LocalizedName => $"{Loc.Get("Chart", "Style")}32";
        #endregion

        #region Properties
        public override Color[] StyleColors => new[]
        {
            StiColor.Get("3a5263"),
            StiColor.Get("90a1ab"),
            StiColor.Get("c9d5dc")
        };

        public override Color BasicStyleColor => StiColor.Get("e9f4fc");

        public override StiBrush ChartBrush => new StiSolidBrush("6d7e8b");

        public override StiBrush ChartAreaBrush => new StiSolidBrush("6d7e8b");

        #region Legend
        public override StiBrush LegendBrush => new StiSolidBrush(Color.Transparent);

        public override Color LegendLabelsColor => StiColor.Get("e9f4fc");

        public override Color LegendBorderColor => Color.Transparent;

        public override Color LegendTitleColor => StiColor.Get("e9f4fc");

        public override bool LegendShowShadow => false;

        public override Font LegendFont => new Font("Arial", 9);
        #endregion

        #region SeriesLabels
        public override Color SeriesLabelsBorderColor => Color.Transparent;

        public override StiBrush SeriesLabelsBrush => new StiSolidBrush(Color.FromArgb(0x77, 0xff, 0xff, 0xff));

        public override Color SeriesLabelsColor => StiColor.Get("8c8c8c");
        
        public override Color SeriesLabelsLineColor => StiColor.Get("e9f4fc");

        public override Font SeriesLabelsFont => new Font("Arial", 10);
        #endregion

        #region Axis
        public override Color AxisTitleColor => StiColor.Get("e9f4fc");

        public override Color AxisLineColor => StiColor.Get("e9f4fc");

        public override Color AxisLabelsColor => StiColor.Get("e9f4fc");
        #endregion

        #region GridLines
        public override Color GridLinesHorColor => StiColor.Get("e9f4fc");

        public override Color GridLinesVertColor => StiColor.Get("e9f4fc");
        #endregion

        #region ToolTip
        public override StiBrush ToolTipBrush => new StiSolidBrush(Color.FromArgb(150, 67, 84, 94));
        #endregion

        public override bool SeriesLighting => false;

        public override StiChartStyleId StyleId => StiChartStyleId.StiStyle32;
        #endregion

        #region Methods
        public override Color GetColumnBorder(Color color)
        {
            return Color.Transparent;
        }

        public override StiBrush GetColumnBrush(Color color)
        {
            return new StiSolidBrush(color);
        }
        #endregion
    }
}