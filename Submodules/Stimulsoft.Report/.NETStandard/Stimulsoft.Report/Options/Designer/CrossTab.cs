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

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// Class which allows adjustment of the Designer of the report.
        /// </summary>
        public sealed partial class Designer
        {
            public sealed class CrossTab
            {
                public static Color[] StyleColors { get; set; } =
                {
                    Color.White,
                    Color.DarkGray,
                    Color.PeachPuff,
                    Color.Plum,
                    Color.LightCoral,
                    Color.SkyBlue,
                    Color.LightSeaGreen,
                    Color.LightGreen,
                    Color.YellowGreen,
                    Color.Wheat,
                    Color.Khaki,
                    ColorTranslator.FromHtml("#0bac45"),
                    ColorTranslator.FromHtml("#b5a1dd"),
                    ColorTranslator.FromHtml("#ffc000"),
                    ColorTranslator.FromHtml("#ed7d31"),
                    ColorTranslator.FromHtml("#239fd9"),
                };
            }
        }
    }
}