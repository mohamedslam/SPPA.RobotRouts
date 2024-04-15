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
using System.Drawing;

namespace Stimulsoft.Report.Maps
{
    public class StiMap21StyleFX : StiMapStyleFX
    {
        public StiMap21StyleFX()
        {
            this.HeatmapWithGroup.Colors = new[]
            {
                StiColor.Get("#239fd9"),
                StiColor.Get("#b2b2b2")
            };
            this.Heatmap.Color = StiColor.Get("#239fd9");
        }

        #region Properties
        public override StiMapStyleIdent StyleId => StiMapStyleIdent.Style21;

        public override string LocalizeName => $"{Loc.Get("Chart", "Style")}21";

        public override Color IndividualColor
        {
            get
            {
                return StiColor.Get("#239fd9");
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
                    StiColor.Get("#239fd9"),
                    StiColor.Get("#b2b2b2"),
                    StiColor.Get("#55d1ff"),
                    StiColor.Get("#e4e4e4"),
                    StiColor.Get("#55d1ff"),
                    StiColor.Get("#e4e4e4")
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
                return StiColor.Get("#666666");
            }
            set 
            { 
            }
        }

        public override Color DefaultColor
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
    }
}