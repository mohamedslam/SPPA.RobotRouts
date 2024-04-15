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
using Stimulsoft.Report.Dashboard;
using System.Drawing;
using Stimulsoft.Base.Drawing;

namespace Stimulsoft.Report.Maps
{
    public class StiMap33StyleFX : StiMapStyleFX
    {
        #region Properties
        public override bool AllowDashboard => true;

        public override string DashboardName => StiLocalization.Get("PropertyColor", "AliceBlue");

        public override StiElementStyleIdent StyleIdent => StiElementStyleIdent.AliceBlue;

        public override StiMapStyleIdent StyleId => StiMapStyleIdent.Style33;

        public override string LocalizeName => StiLocalization.Get("Chart", "Style") + "33";

        public override Color DefaultColor
        {
            get
            {
                return StiColor.Get("#40568d");
            }
            set
            {
            }
        }

        public override Color BorderColor
        {
            get
            {
                return StiColor.Get("647cb9");
            }
            set
            {
            }
        }

        public override Color IndividualColor
        {
            get
            {
                return StiColor.Get("40568d");
            }
            set
            {
            }
        }

        public override Color[] Colors
        {
            get
            {
                return new[]
                {
                    StiColor.Get("40568d"),
                    StiColor.Get("4569bb"),
                    StiColor.Get("e47334"),
                    StiColor.Get("9d9c9c"),
                    StiColor.Get("f8b92d"),
                    StiColor.Get("5e93cc"),
                    StiColor.Get("6ea548")
                };
            }
            set
            {
            }
        }

        public override Color BackColor
        {
            get
            {
                return StiColor.Get("#ffffff");
            }
            set
            {
            }
        }
        #endregion

        public StiMap33StyleFX()
        {
            this.HeatmapWithGroup.Colors = new[]
            {
                StiColor.Get("#40568d"),
                StiColor.Get("#ccd5f0")
            };
            this.Heatmap.Color = StiColor.Get("#40568d");
        }
    }
}