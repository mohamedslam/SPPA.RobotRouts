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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Export;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// Class for adjustment of the report Export.
        /// </summary>
        public sealed partial class Export
		{	
            /// <summary>
            /// Class for adjustment of the text export of a report.
            /// </summary>
            public class Text
            {
                /// <summary>
                /// Set the columns width in format "a;b;c;d;e;f; ... "
                /// </summary>
                [Obsolete("ColumnWidths property is obsolete.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                [DefaultValue("")]
                [Description("Set the columns width in format 'a;b;c;d;e;f; ... '.")]
                [StiSerializable]
                public static string ColumnWidths { get; set; } = string.Empty;

                /// <summary>
                /// Gets or sets a value which indicates whether it is necessary to use full width of TextBox to put text.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value which indicates whether it is necessary to use full width of TextBox to put text.")]
                [StiSerializable]
                public static bool UseFullTextBoxWidth { get; set; }

                /// <summary>
                /// Gets or sets a value which indicates whether it is necessary to use old export mode.
                /// </summary>
                [Obsolete("UseOldMode property is obsolete.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                [DefaultValue(false)]
                [Description("Gets or sets a value which indicates whether it is necessary to use old export mode.")]
                [StiSerializable]
                public static bool UseOldMode { get; set; }

                /// <summary>
                /// Gets or sets a value which indicates whether it is necessary to use full  vertical border.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value which indicates whether it is necessary to use full  vertical border.")]
                [StiSerializable]
                public static bool UseFullVerticalBorder { get; set; } = true;

                /// <summary>
				/// Gets or sets a value which indicates whether it is necessary to use full horizontal border.
				/// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value which indicates whether it is necessary to use full horizontal border.")]
                [StiSerializable]
                public static bool UseFullHorizontalBorder { get; set; } = true;

                /// <summary>
                /// Gets or sets a text of checkbox for true
                /// </summary>
                [DefaultValue("+")]
                [Description("Gets or sets a text of checkbox for true.")]
                [StiSerializable]
                public static string CheckBoxTextForTrue { get; set; } = "+";

                /// <summary>
				/// Gets or sets a text of checkbox for true
				/// </summary>
                [DefaultValue("-")]
                [Description("Gets or sets a text of checkbox for true.")]
                [StiSerializable]
                public static string CheckBoxTextForFalse { get; set; } = "-";

                /// <summary>
                /// Gets or sets a value which indicates whether it is necessary to trim trailing spaces.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value which indicates whether it is necessary to trim trailing spaces.")]
                [StiSerializable]
                public static bool TrimTrailingSpaces { get; set; } = true;

                /// <summary>
                /// Gets or sets a value which indicates whether it is necessary to remove last NewLine marker.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value which indicates whether it is necessary to remove last NewLine marker.")]
                [StiSerializable]
                public static bool RemoveLastNewLineMarker { get; set; }


                private static List<StiEscapeCodesCollection> escapeCodesCollectionList;
                /// <summary>
                /// Gets or sets a list of escape codes on their behalf.
                /// </summary>
                public static List<StiEscapeCodesCollection> EscapeCodesCollectionList
                {
                    get
                    {
                        if (escapeCodesCollectionList == null)
                        {
                            escapeCodesCollectionList = new List<StiEscapeCodesCollection>();

                            var epsonFX = new StiEscapeCodesCollection();
                            epsonFX.Name = "EpsonFX";
                            epsonFX["b"] = "\x1b" + "E";
                            epsonFX["/b"] = "\x1b" + "F";
                            epsonFX["i"] = "\x1b" + "4";
                            epsonFX["/i"] = "\x1b" + "5";
                            epsonFX["u"] = "\x1b" + "-1";
                            epsonFX["/u"] = "\x1b" + "-0";
                            epsonFX["sup"] = "\x1b" + "S0";
                            epsonFX["/sup"] = "\x1b" + "T";
                            epsonFX["sub"] = "\x1b" + "S1";
                            epsonFX["/sub"] = "\x1b" + "T";
                            epsonFX["condensed"] = "\x0F";
                            epsonFX["/condensed"] = "\x12";
                            epsonFX["elite"] = "\x1b" + "M";
                            epsonFX["pica"] = "\x1b" + "P";
                            epsonFX["doublewidth"] = "\x1b" + "W1";
                            epsonFX["/doublewidth"] = "\x1b" + "W0";
                            epsonFX["ff"] = "\x0c";

                            var oki = new StiEscapeCodesCollection();
                            oki.Name = "Oki ML92/93";
                            oki["b"] = "\x1b" + "T";
                            oki["/b"] = "\x1b" + "I";
                            //oki["i"] = "";
                            //oki["/i"] = "";
                            oki["u"] = "\x1b" + "H";
                            oki["/u"] = "\x1b" + "D";
                            oki["sup"] = "\x1b" + "J";
                            oki["/sup"] = "\x1b" + "K";
                            oki["sub"] = "\x1b" + "L";
                            oki["/sub"] = "\x1b" + "M";
                            oki["condensed"] = "\x1d";
                            oki["/condensed"] = "\x1e";     //use the PICA mode
                            oki["elite"] = "\x1c";
                            oki["pica"] = "\x1e";
                            oki["doublewidth"] = "\x1f";
                            oki["/doublewidth"] = "\x1e";   //use the PICA mode
                            oki["ff"] = "\x0c";

                            var pos = new StiEscapeCodesCollection();
                            pos.Name = "ESC/POS";
                            pos["b"] = "\x1b" + "E" + "\x01";
                            pos["/b"] = "\x1b" + "E" + "\x00";
                            pos["u"] = "\x1b" + "-" + "\x01"; ;
                            pos["/u"] = "\x1b" + "-" + "\x00"; ;
                            pos["doublewidth"] = "\x1b" + "!" + "\x20";
                            pos["/doublewidth"] = "\x1b" + "!" + "\x00";
                            pos["ff"] = "\x1b" + "i";

                            var none = new StiEscapeCodesCollection();
                            none.Name = "None";
                            none["ff"] = "\x0c";

                            escapeCodesCollectionList.Add(none);
                            escapeCodesCollectionList.Add(epsonFX);
                            escapeCodesCollectionList.Add(oki);
                            escapeCodesCollectionList.Add(pos);
                        }
                        return escapeCodesCollectionList;
                    }
                    set
                    {
                        escapeCodesCollectionList = value;
                    }
                }
			}

            #region Obsoleted
            /// <summary>
            /// Class for adjustment of the Text export of a report.
            /// </summary>
            [Obsolete("The class StiOptions.Txt is obsolete! Please use class StiOptions.Text instead it.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public sealed class Txt : Text
            {

            }
            #endregion
		}
    }
}
