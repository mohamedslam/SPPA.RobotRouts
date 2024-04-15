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
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Styles;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Gauge
{
    public class StiGaugeStyleCoreXF30 : StiGaugeStyleCoreXF
    {
        #region Properties.Localization
        /// <summary>
        /// Gets a localized name of style.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Chart", "Style") + "30";
        #endregion

        #region Properties
        public override StiBrush Brush => new StiSolidBrush("#595b65");

        public override Color BorderColor => Color.Transparent;

        public override Color ForeColor { get; } = StiDashboardStyleHelper.GetForeColor(StiElementStyleIdent.DarkGray);

        public override float BorderWidth => 1;

        public override Color TargetColor => ColorTranslator.FromHtml("#d3d3d5");
        #endregion

        #region Properties.TickMarkMajor
        public override StiBrush TickMarkMajorBrush => new StiSolidBrush("#ffffff");

        public override StiBrush TickMarkMajorBorder => new StiEmptyBrush();
        #endregion

        #region Properties.TickMarkMinor
        public override StiBrush TickMarkMinorBrush => new StiEmptyBrush();

        public override StiBrush TickMarkMinorBorder => new StiEmptyBrush();
        #endregion

        #region Properties.TickLabelMajor
        public override StiBrush TickLabelMajorTextBrush => new StiSolidBrush("#ffffff");

        public override Font TickLabelMajorFont => new Font("Arial", 10);
        #endregion

        #region Properties.TickLabelMinor
        public override StiBrush TickLabelMinorTextBrush => new StiSolidBrush("#ffffff");

        public override Font TickLabelMinorFont => new Font("Arial", 9);
        #endregion

        #region Properties.Marker
        public override StiBrush MarkerBrush => new StiSolidBrush("#d3d3d5");

        public override StiBrush LinearMarkerBorder => new StiSolidBrush("#d3d3d5");
        #endregion

        #region Properties.LinearScale
        public override StiBrush LinearScaleBrush => new StiSolidBrush("#db7b46");

        public override StiBrush LinearBarBrush => new StiSolidBrush("#db7b46");

        public override StiBrush LinearBarBorderBrush => new StiEmptyBrush();

        public override StiBrush LinearBarEmptyBrush => new StiEmptyBrush();

        public override StiBrush LinearBarEmptyBorderBrush => new StiEmptyBrush();

        public override float LinearBarStartWidth => 0.1f;

        public override float LinearBarEndWidth => 0.1f;
        #endregion

        #region Properties.PropertiesRadial
        public override StiBrush RadialBarBrush => new StiSolidBrush("#db7b46");

        public override StiBrush RadialBarBorderBrush => new StiEmptyBrush();

        public override StiBrush RadialBarEmptyBrush => new StiSolidBrush("#6b6e75");

        public override StiBrush RadialBarEmptyBorderBrush => new StiEmptyBrush();

        public override float RadialBarStartWidth => 0.1f;

        public override float RadialBarEndWidth => 0.1f;
        #endregion

        #region Properties.Needle
        public override StiBrush NeedleBrush => new StiSolidBrush("#d3d3d5");

        public override StiBrush NeedleBorderBrush => new StiEmptyBrush();

        public override StiBrush NeedleCapBrush => new StiSolidBrush("#ffffff");

        public override StiBrush NeedleCapBorderBrush => new StiSolidBrush("#d3d3d5");

        public override float NeedleBorderWidth => 0;

        public override float NeedleCapBorderWidth => 2;

        public override float NeedleStartWidth => 0.1f;

        public override float NeedleEndWidth => 1;

        public override float NeedleRelativeHeight => 0.06f;

        public override float NeedleRelativeWith => 0.45f;
        #endregion
    }
}