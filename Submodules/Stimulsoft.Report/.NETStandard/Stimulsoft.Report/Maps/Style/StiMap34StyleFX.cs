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
    public class StiMap34StyleFX : StiMapStyleFX
    {
        #region Properties
        public override bool AllowDashboard => true;

        public override string DashboardName => Loc.Get("PropertyColor", "DarkGreen");

        public override StiElementStyleIdent StyleIdent => StiElementStyleIdent.DarkGreen;

        public override StiMapStyleIdent StyleId => StiMapStyleIdent.Style34;

        public override string LocalizeName => $"{Loc.Get("Chart", "Style")}34";

        public override Color DefaultColor
        {
            get
            {
                return StiColor.Get("e3c08e");
            }
            set
            {
            }
        }

        public override Color BorderColor
        {
            get
            {
                return Color.DarkGray;
            }
            set
            {
            }
        }

        public override Color IndividualColor
        {
            get
            {
                return StiColor.Get("e3c08e");
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
                    StiColor.Get("144b45"),
                    StiColor.Get("ecb92f"),
                    StiColor.Get("d66153"),
                    StiColor.Get("25775b"),
                    StiColor.Get("319491"),
                    StiColor.Get("f7f7f7"),
                    StiColor.Get("dd7c21"),
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
                return StiColor.Get("3f745e");
            }
            set
            {
            }
        }

        public override StiBrush ToolTipBrush { get; set; } = new StiSolidBrush(Color.FromArgb(150, 20, 75, 69));

        public override StiBrush ToolTipTextBrush { get; set; } = new StiSolidBrush(Color.White);

        public override StiSimpleBorder ToolTipBorder { get; set; } = new StiSimpleBorder(StiBorderSides.None, Color.White, 1, StiPenStyle.Solid);
        #endregion

        public StiMap34StyleFX()
        {
            this.HeatmapWithGroup.Colors = new[]
            {
                StiColor.Get("#539790"),
                StiColor.Get("#264945")
            };
            this.Heatmap.Color = StiColor.Get("#539790");
        }
    }
}