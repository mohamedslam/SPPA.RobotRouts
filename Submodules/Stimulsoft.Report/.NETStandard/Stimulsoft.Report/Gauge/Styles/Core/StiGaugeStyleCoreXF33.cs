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
    public class StiGaugeStyleCoreXF33 : StiGaugeStyleCoreXF30
    {
        #region Properties.Localization
        /// <summary>
        /// Gets a localized name of style.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Chart", "Style") + "33";
        #endregion

        #region Properties
        public override StiBrush Brush => new StiSolidBrush("#f2f5fc");

        public override Color ForeColor { get; } = StiDashboardStyleHelper.GetForeColor(StiElementStyleIdent.AliceBlue);

        public override Color TargetColor => ColorTranslator.FromHtml("#e47334");
        #endregion

        #region Properties.TickMarkMajor
        public override StiBrush TickMarkMajorBrush => new StiSolidBrush("#3a3a3a");

        public override StiBrush TickMarkMajorBorder => new StiEmptyBrush();
        #endregion

        #region Properties.TickMarkMinor
        public override StiBrush TickMarkMinorBrush => new StiEmptyBrush();

        public override StiBrush TickMarkMinorBorder => new StiEmptyBrush();
        #endregion

        #region Properties.TickLabelMajor
        public override StiBrush TickLabelMajorTextBrush => new StiSolidBrush("#3a3a3a");

        public override Font TickLabelMajorFont => new Font("Arial", 8);
        #endregion

        #region Properties.TickLabelMinor
        public override StiBrush TickLabelMinorTextBrush => new StiSolidBrush("#3a3a3a");

        public override Font TickLabelMinorFont => new Font("Arial", 7);
        #endregion

        #region Properties.Marker
        public override StiBrush MarkerBrush => new StiSolidBrush("#d41c2a");

        public override StiBrush LinearMarkerBorder => new StiSolidBrush(Color.Transparent);
        #endregion

        #region Properties.Needle
        public override StiBrush NeedleBrush => new StiSolidBrush("#d41c2a");

        public override StiBrush NeedleBorderBrush => new StiSolidBrush("#d41c2a");

        public override StiBrush NeedleCapBrush => new StiSolidBrush("#d41c2a");

        public override StiBrush NeedleCapBorderBrush => new StiSolidBrush("#d3d3d5");

        public override float NeedleCapBorderWidth => 0;
        #endregion

        #region Properties.LinearScale
        public override StiBrush LinearScaleBrush => new StiSolidBrush("#ccd5f0");

        public override StiBrush LinearBarBrush => new StiSolidBrush("#5d6b99");
        #endregion

        #region Properties.PropertiesRadial
        public override StiBrush RadialBarBrush => new StiSolidBrush("#ccd5f0");

        public override StiBrush RadialBarEmptyBrush => new StiSolidBrush("#5d6b99");
        #endregion
    }
}