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
using System.ComponentModel;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Serializing;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

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
            /// Class for adjustment of the StiCheckBoxExcelValue parameters
            /// </summary>
            public class CheckBoxReplacementForExcelValue
            {
                /// <summary>
                ///  Gets or sets a default font of text
                /// </summary>
                [Description("Gets or sets a default font of text.")]
                [StiSerializable]
                [StiOptionsFontHelper(1)]
                public static Font Font { get; set; } = new Font("Arial", 10);

                /// <summary>
                ///  Gets or sets a default horizontal alignment of text
                /// </summary>
                [DefaultValue(StiTextHorAlignment.Center)]
                [Description("Gets or sets a default horizontal alignment of text.")]
                [StiSerializable]
                public static StiTextHorAlignment HorAlignment { get; set; } = StiTextHorAlignment.Center;

                /// <summary>
                ///  Gets or sets a default vertical alignment of text
                /// </summary>
                [DefaultValue(StiVertAlignment.Center)]
                [Description("Gets or sets a default vertical alignment of text.")]
                [StiSerializable]
                public static StiVertAlignment VertAlignment { get; set; } = StiVertAlignment.Center;
            }

            #region Obsoleted
            /// <summary>
            /// Class for adjustment of the RichText export of a report.
            /// </summary>
            [Obsolete("The class StiOptions.StiCheckBoxExcelValueParameters is obsolete! Please use class StiOptions.CheckBoxReplacementForExcelValue instead it.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public sealed class StiCheckBoxExcelValueParameters : CheckBoxReplacementForExcelValue
            {

            }
            #endregion
		}
    }
}