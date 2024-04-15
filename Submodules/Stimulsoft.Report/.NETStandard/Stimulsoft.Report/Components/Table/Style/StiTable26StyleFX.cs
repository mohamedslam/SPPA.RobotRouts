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

using System.Drawing;

namespace Stimulsoft.Report.Components.Table
{
    public class StiTable26StyleFX : StiTableStyleFX
    {
        #region Properties
        public override StiTableStyleIdent StyleId => StiTableStyleIdent.Style26;

        public override Color HeaderColor
        {
            get
            {
                return ColorTranslator.FromHtml("#b42ec6c8");
            }
            set
            {
            }
        }

        public override Color FooterColor
        {
            get
            {
                return ColorTranslator.FromHtml("#b4d04456");
            }
            set
            {
            }
        }

        public override Color DataColor
        {
            get
            {
                return ColorTranslator.FromHtml("#ffffff");
            }
            set
            {
            }
        }

        public override Color DataForeground
        {
            get
            {
                return ColorTranslator.FromHtml("#000000");
            }
            set
            {
            }
        }

        public override Color HeaderForeground
        {
            get
            {
                return ColorTranslator.FromHtml("#ffffff");
            }
            set
            {
            }
        }

        public override Color FooterForeground
        {
            get
            {
                return ColorTranslator.FromHtml("#b45ab0ee");
            }
            set
            {
            }
        }

        public override Color GridColor
        {
            get
            {
                return ColorTranslator.FromHtml("#b4264478");
            }
            set
            {
            }
        }
        #endregion
    }
}