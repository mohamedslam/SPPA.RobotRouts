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
using System.Drawing.Imaging;
using Stimulsoft.Base.Serializing;

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
			/// Class for adjustment of the OpenDocumentCalc export of a report.
			/// </summary>
            public class OpenDocumentCalc
			{
			    /// <summary>
                /// Gets or sets a value indicating behavior of the exporting segmented pages will be forcibly broken in order of chosen page format.
			    /// </summary>
			    [DefaultValue(true)]
                [Description("Gets or sets a value indicating behavior of the exporting segmented pages will be forcibly broken in order of chosen page format.")]
			    [StiSerializable]
			    public static bool DivideSegmentPages { get; set; } = true;

			    /// <summary>
			    /// Gets or sets a value indicating whether it is necessary to use image comparer.
			    /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to use image comparer.")]
                [StiSerializable]
			    public static bool AllowImageComparer { get; set; } = true;

			    /// <summary>
			    ///  Gets or sets a value indicating whether it is necessary to remove empty space at the bottom of page
			    /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to remove empty space at the bottom of page.")]
                [StiSerializable]
			    public static bool RemoveEmptySpaceAtBottom { get; set; } = true;

			    /// <summary>
                /// Gets or sets a value indicating behavior of the exporting big cells will be divided into small cells.
			    /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating behavior of the exporting big cells will be divided into small cells.")]
                [StiSerializable]
			    public static bool DivideBigCells { get; set; } = true;

			    /// <summary>
			    /// Gets or sets a value indicating maximum sheet height in rows.
			    /// </summary>
			    [DefaultValue(1048574)]
			    [Description("Gets or sets a value indicating maximum sheet height in rows.")]
			    [StiSerializable]
			    public static int MaximumSheetHeight { get; set; } = 1048574;

                /// <summary>
                /// Gets or sets image format for exported images.
                /// </summary>
                [Description("Gets or sets image format for exported images.")]
                [StiSerializable]
                public static ImageFormat ImageFormat { get; set; } = null; // null for auto, or ImageFormat.Jpeg or ImageFormat.Png
            }

            #region Obsoleted
            /// <summary>
            /// Class for adjustment of the PowerPoint2007 export of a report.
            /// </summary>
            [Obsolete("The class StiOptions.Ods is obsolete! Please use class StiOptions.OpenDocumentCalc instead it.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public sealed class Ods : OpenDocumentCalc
            {

            }
            #endregion
		}
    }
}