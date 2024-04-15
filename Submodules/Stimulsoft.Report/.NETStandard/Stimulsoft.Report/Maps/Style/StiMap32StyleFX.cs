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
    public class StiMap32StyleFX : StiMapStyleFX
    {
        #region Properties
        public override bool AllowDashboard => true;

        public override string DashboardName => StiLocalization.Get("PropertyColor", "Silver");

        public override StiElementStyleIdent StyleIdent => StiElementStyleIdent.Silver;

        public override StiMapStyleIdent StyleId => StiMapStyleIdent.Style32;

        public override string LocalizeName => StiLocalization.Get("Chart", "Style") + "32";

        public override Color BorderColor
        {
            get
            {
                return StiColor.Get("bb8698a2");
            }
            set
            {
            }
        }

        public override Color IndividualColor
        {
            get
            {
                return StiColor.Get("3a5263");
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
                    StiColor.Get("3a5263"),
                    StiColor.Get("90a1ab"),
                    StiColor.Get("c9d5dc")
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
                return StiColor.Get("#6d7e8b");
            }
            set
            {
            }
        }

        public override StiBrush ToolTipBrush { get; set; } = new StiSolidBrush(Color.FromArgb(150, 67, 84, 94));
        #endregion

        public StiMap32StyleFX()
        {
            this.HeatmapWithGroup.Colors = new[]
            {
                StiColor.Get("#3a5263"),
                StiColor.Get("#c9d5dc")
            };
            this.Heatmap.Color = StiColor.Get("#3a5263");
        }
    }
}