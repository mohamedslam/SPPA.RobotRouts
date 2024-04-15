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
    public class StiMap28StyleFX : StiMapStyleFX
    {
        #region Properties
        public override bool AllowDashboard => true;

        public override string DashboardName => StiLocalization.Get("PropertyColor", "DarkBlue");

        public override StiMapStyleIdent StyleId => StiMapStyleIdent.Style28;

        public override StiElementStyleIdent StyleIdent => StiElementStyleIdent.DarkBlue;

        public override string LocalizeName => StiLocalization.Get("Chart", "Style") + 28;

        public override Color IndividualColor
        {
            get
            {
                return StiColor.Get("#165d9e");
            }
            set { }
        }

        public override Color[] Colors
        {
            get
            {
                return new[]
                {
                    StiColor.Get("#165d9e"),
                    StiColor.Get("#577eb6"),
                    StiColor.Get("#569436"),
                    StiColor.Get("#225056"),
                    StiColor.Get("#d4dae0"),
                };
            }
            set { }
        }

        public override Color DefaultColor
        {
            get
            {
                return StiColor.Get("#ffffff");
            }
            set { }
        }


        public override Color BackColor
        {
            get
            {
                return StiColor.Get("#0a325a");
            }
            set { }
        }

        public override Color BorderColor
        {
            get
            {
                return StiColor.Get("#99bbbbbb");
            }
            set { }
        }

        public override StiBrush ToolTipBrush { get; set; } = new StiSolidBrush(Color.FromArgb(150, 6, 32, 59));

        public override StiBrush ToolTipTextBrush { get; set; } = new StiSolidBrush(Color.White);

        public override StiSimpleBorder ToolTipBorder { get; set; } = new StiSimpleBorder(StiBorderSides.All, Color.White, 1, StiPenStyle.Solid);
        #endregion

        public StiMap28StyleFX()
        {
            this.HeatmapWithGroup.Colors = new[]
            {
                StiColor.Get("#165d9e"),
                StiColor.Get("#569436")
            };
            this.Heatmap.Color = StiColor.Get("#165d9e");
        }
    }
}