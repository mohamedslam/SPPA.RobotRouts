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
    public class StiGaugeStyleCoreXF31 : StiGaugeStyleCoreXF30
    {
        #region Properties.Localization
        /// <summary>
        /// Gets a localized name of style.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Chart", "Style") + "31";
        #endregion

        #region Properties
        public override StiBrush Brush => new StiSolidBrush("#235e6d");

        public override Color ForeColor { get; } = StiDashboardStyleHelper.GetForeColor(StiElementStyleIdent.DarkTurquoise);

        public override Color TargetColor => ColorTranslator.FromHtml("#fefef9");
        #endregion

        #region Properties.Marker
        public override StiBrush MarkerBrush => new StiSolidBrush("#f2ffff");

        public override StiBrush LinearMarkerBorder => new StiSolidBrush("#f2ffff");
        #endregion

        #region Properties.LinearScale
        public override StiBrush LinearScaleBrush => new StiSolidBrush("#f0621e");

        public override StiBrush LinearBarBrush => new StiSolidBrush("#f0621e");
        #endregion

        #region Properties.PropertiesRadial
        public override StiBrush RadialBarBrush => new StiSolidBrush("#f0621e");

        public override StiBrush RadialBarEmptyBrush => new StiSolidBrush("#1c445b");
        #endregion
    }
}