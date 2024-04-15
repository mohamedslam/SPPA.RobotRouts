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
using System.Drawing;

namespace Stimulsoft.Report.Maps
{
    public class StiMap35StyleFX : StiMapStyleFX
    {
        #region Properties
        public override bool AllowDashboard => true;

        public override string DashboardName => StiLocalization.Get("PropertyColor", "Sienna");

        public override StiElementStyleIdent StyleIdent => StiElementStyleIdent.Sienna;

        public override StiMapStyleIdent StyleId => StiMapStyleIdent.Style35;

        public override string LocalizeName => $"{Loc.Get("Chart", "Style")}35";

        public override Color DefaultColor
        {
            get
            {
                return StiColor.Get("#c2b39c");
            }
            set
            {
            }
        }

        public override Color BorderColor
        {
            get
            {
                return StiColor.Get("#442513");
            }
            set
            {
            }
        }

        public override Color IndividualColor
        {
            get
            {
                return StiColor.Get("#c2b39c");
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
                    StiColor.Get("#794d26"),
                    StiColor.Get("#c7986a"),
                    StiColor.Get("#c4b49a"),
                    StiColor.Get("#894d29"),
                    StiColor.Get("#422515"),
                    StiColor.Get("#564438"),
                    StiColor.Get("#876c57"),
                    StiColor.Get("#d8814b"),
                    StiColor.Get("#532525"),
                    StiColor.Get("#59413f"),
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
                return StiColor.Get("#fefefe");
            }
            set
            {
            }
        }

        public override StiBrush ToolTipBrush { get; set; } = new StiSolidBrush(Color.FromArgb(150, 66, 37, 21));

        public override StiBrush ToolTipTextBrush { get; set; } = new StiSolidBrush(Color.White);

        public override StiSimpleBorder ToolTipBorder { get; set; } = new StiSimpleBorder(StiBorderSides.None, Color.White, 1, StiPenStyle.Solid);
        #endregion

        public StiMap35StyleFX()
        {
            this.HeatmapWithGroup.Colors = new[]
            {
                StiColor.Get("#c7986a"),
                StiColor.Get("#260d09")
            };
            this.Heatmap.Color = StiColor.Get("#c7986a");
        }
    }
}