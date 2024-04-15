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
    public class StiMap29StyleFX : StiMapStyleFX
    {
        #region Properties
        public override bool AllowDashboard => true;

        public override string DashboardName => StiLocalization.Get("PropertyColor", "Blue");

        public override StiElementStyleIdent StyleIdent => StiElementStyleIdent.Blue;

        public override StiMapStyleIdent StyleId => StiMapStyleIdent.Style29;

        public override string LocalizeName => StiLocalization.Get("Chart", "Style") + 29;

        public override Color DefaultColor
        {
            get
            {
                return StiColor.Get("#d0d0d0");
            }
            set { }
        }

        public override Color BorderColor
        {
            get
            {
                return StiColor.Get("#b4b4b5");
            }
            set { }
        }

        public override Color IndividualColor
        {
            get
            {
                return StiColor.Get("#3498db");
            }
            set { }
        }

        public override Color[] Colors
        {
            get
            {
                return new[]
                {
                    StiColor.Get("#3498db"),
                    StiColor.Get("#ef717a"),
                    StiColor.Get("#6dcbb3"),
                    StiColor.Get("#f28161"),
                    StiColor.Get("#fccd1b"),
                };
            }
            set { }
        }

        public override Color BackColor
        {
            get
            {
                return StiColor.Get("#ffffff");
            }
            set { }
        }

        public override StiBrush ToolTipBrush { get; set; } = new StiSolidBrush(Color.White);

        public override StiBrush ToolTipTextBrush { get; set; } = new StiSolidBrush(Color.DimGray);

        public override StiSimpleBorder ToolTipBorder { get; set; } = new StiSimpleBorder(StiBorderSides.All, StiColor.Get("#8c8c8c"), 1, StiPenStyle.Solid);
        #endregion

        public StiMap29StyleFX()
        {
            this.HeatmapWithGroup.Colors = new[]
            {
                StiColor.Get("#3498db"),
                StiColor.Get("#ef717a")
            };
            this.Heatmap.Color = StiColor.Get("#3498db");
        }
    }
}