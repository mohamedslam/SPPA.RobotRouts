#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	Stimulsoft.Report Library										}
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

namespace Stimulsoft.Report.Dashboard.Styles
{
    public class StiAliceBlueProgressElementStyle : StiProgressElementStyle
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiAliceBlueDashboardProgressStyle;
        #endregion

        #region Properties
        public override string LocalizedName => StiLocalization.Get("PropertyColor", "AliceBlue");

        public override StiElementStyleIdent Ident => StiElementStyleIdent.AliceBlue;

        public override Color TrackColor { get; set; } = StiColor.Get("3a5263");

        public override Color BandColor { get; set; } = StiColor.Get("e9f5fc");

        public override Color[] SeriesColors { get; set; } =
        {
            StiColor.Get("4569bb"),
            StiColor.Get("e47334"),
            StiColor.Get("9d9c9c"),
            StiColor.Get("f8b92d"),
            StiColor.Get("5e93cc"),
            StiColor.Get("6ea548")
        };
        #endregion
    }
}