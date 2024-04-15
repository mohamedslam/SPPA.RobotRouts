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
    public class StiMap31StyleFX : StiMapStyleFX
    {
        #region Properties
        public override bool AllowDashboard => true;

        public override string DashboardName => StiLocalization.Get("PropertyColor", "DarkTurquoise");

        public override StiElementStyleIdent StyleIdent => StiElementStyleIdent.DarkTurquoise;

        public override StiMapStyleIdent StyleId => StiMapStyleIdent.Style31;

        public override string LocalizeName => StiLocalization.Get("Chart", "Style") + "31";

        public override Color BorderColor
        {
            get
            {
                return StiColor.Get("aa1c4458");
            }
            set
            {
            }
        }

        public override Color IndividualColor
        {
            get
            {
                return StiColor.Get("#5ea8bf");
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
                    StiColor.Get("#fefef9"),
                    StiColor.Get("#a8d7e2"),
                    StiColor.Get("#5ea8bf"),
                    StiColor.Get("#2b7f9e"),
                    StiColor.Get("#1c4458")
                };
            }
            set
            {
            }
        }

        public override Color DefaultColor
        {
            get
            {
                return StiColor.Get("#fefef9");
            }
            set
            {
            }
        }

        public override Color BackColor
        {
            get
            {
                return StiColor.Get("#235e6d");
            }
            set
            {
            }
        }
        public override StiBrush ToolTipBrush { get; set; } = new StiSolidBrush(Color.FromArgb(150, 30, 74, 97));

        public override StiBrush ToolTipTextBrush { get; set; } = new StiSolidBrush(Color.White);

        public override StiSimpleBorder ToolTipBorder { get; set; } = new StiSimpleBorder(StiBorderSides.All, Color.White, 1, StiPenStyle.Solid);
        #endregion

        public StiMap31StyleFX()
        {
            this.HeatmapWithGroup.Colors = new[]
            {
                StiColor.Get("#a8d7e2"),
                StiColor.Get("#1c4458")
            };
            this.Heatmap.Color = StiColor.Get("#a8d7e2");
        }
    }
}