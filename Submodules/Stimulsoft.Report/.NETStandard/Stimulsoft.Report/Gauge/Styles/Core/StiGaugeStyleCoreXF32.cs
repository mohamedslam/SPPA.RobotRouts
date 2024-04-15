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
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Styles;

namespace Stimulsoft.Report.Gauge
{
    public class StiGaugeStyleCoreXF32 : StiGaugeStyleCoreXF30
    {
        #region Properties.Localization
        /// <summary>
        /// Gets a localized name of style.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Chart", "Style") + "32";
        #endregion

        #region Properties
        public override StiBrush Brush => new StiSolidBrush("#6d7e8b");

        public override Color ForeColor { get; } = StiDashboardStyleHelper.GetForeColor(StiElementStyleIdent.Silver);

        public override Color TargetColor => ColorTranslator.FromHtml("#90a1ab");
        #endregion

        #region Properties.Marker
        public override StiBrush MarkerBrush => new StiSolidBrush("#d41c2a");

        public override StiBrush LinearMarkerBorder => new StiSolidBrush("#d41c2a");
        #endregion

        #region Properties.Needle
        public override StiBrush NeedleBrush => new StiSolidBrush("#d41c2a");

        public override StiBrush NeedleBorderBrush => new StiSolidBrush("#d41c2a");
        
        public override StiBrush NeedleCapBrush => new StiSolidBrush("#d41c2a");

        public override StiBrush NeedleCapBorderBrush => new StiSolidBrush("#d3d3d5");

        public override float NeedleCapBorderWidth => 0;
        #endregion

        #region Properties.LinearScale
        public override StiBrush LinearScaleBrush => new StiSolidBrush("#e9f4fc");

        public override StiBrush LinearBarBrush => new StiSolidBrush("#3a5263");
        #endregion

        #region Properties.PropertiesRadial
        public override StiBrush RadialBarBrush => new StiSolidBrush("#e9f4fc");

        public override StiBrush RadialBarEmptyBrush => new StiSolidBrush("#3a5263");
        #endregion
    }
}