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
    public class StiSiennaProgressElementStyle : StiProgressElementStyle
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiSiennaDashboardProgressStyle;
        #endregion

        #region Properties
        public override string LocalizedName => StiLocalization.Get("PropertyColor", "Sienna");

        public override StiElementStyleIdent Ident => StiElementStyleIdent.Sienna;

        public override Color TrackColor { get; set; } = StiColor.Get("e5ddd0");

        public override Color BandColor { get; set; } = StiColor.Get("3d2211");

        public override Color[] SeriesColors { get; set; } =
        {
            StiColor.Get("794d26"),
            StiColor.Get("c7986a"),
            StiColor.Get("c4b49a"),
            StiColor.Get("894d29"),
            StiColor.Get("422515"),
            StiColor.Get("564438"),
            StiColor.Get("876c57"),
            StiColor.Get("d8814b"),
            StiColor.Get("532525"),
            StiColor.Get("59413f"),
        };
        #endregion
    }
}