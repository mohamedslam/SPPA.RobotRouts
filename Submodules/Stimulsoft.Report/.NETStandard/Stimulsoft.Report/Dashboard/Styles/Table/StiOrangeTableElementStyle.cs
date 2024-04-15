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
    public class StiOrangeTableElementStyle : StiTableElementStyle
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiOrangeDashboardTableStyle;
        #endregion

        #region Properties
        public override string LocalizedName => StiLocalization.Get("PropertyColor", "Orange");

        public override StiElementStyleIdent Ident => StiElementStyleIdent.Orange;

        public override Color CellBackColor { get; set; } = Color.White;

        public override Color AlternatingCellBackColor { get; set; } = StiColor.Get("eee");

        public override Color SelectedCellBackColor { get; set; } = StiColorUtils.Light("ed7d31", 30);

        public override Color HeaderBackColor { get; set; } = StiColor.Get("ed7d31");

        public override Color HotHeaderBackColor { get; set; } = StiColorUtils.Light("ed7d31", 30);

        public override Color FooterBackColor { get; set; } = StiColor.Get("ed7d31");

        public override Color FooterForeColor { get; set; } = Color.White;
        #endregion
    }
}