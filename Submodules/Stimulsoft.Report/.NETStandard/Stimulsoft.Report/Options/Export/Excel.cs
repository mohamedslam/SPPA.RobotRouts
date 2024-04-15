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
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Export;
using System.Drawing.Imaging;

#if STIDRAWING
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
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
        public partial class Export
		{	
			/// <summary>
			/// Class for adjustment of the Excel export of a report.
			/// </summary>
            public class Excel
			{
			    /// <summary>
			    /// Gets or sets a value indicating whether it is necessary to use image comparer.
			    /// </summary>
			    [DefaultValue(false)]
			    [Description("Gets or sets a value indicating whether it is necessary to use image comparer.")]
			    [StiSerializable]
			    public static bool AllowExportDateTime { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to set columns right to left.
                /// </summary>
                [DefaultValue(false)]
			    [Description("Gets or sets a value indicating whether it is necessary to set columns right to left.")]
			    [StiSerializable]
			    public static bool ColumnsRightToLeft { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to set flag "Show Grid Lines".
                /// </summary>
                [DefaultValue(true)]
			    [Description("Gets or sets a value indicating whether it is necessary to set flag 'Show Grid Lines'.")]
			    [StiSerializable]
			    public static bool ShowGridLines { get; set; } = true;

			    /// <summary>
			    /// Gets or sets a value indicating maximum sheet height in rows.
			    /// </summary>
			    [DefaultValue(65534)]
			    [Description("Gets or sets a value indicating maximum sheet height in rows.")]
			    [StiSerializable]
			    public static int MaximumSheetHeight { get; set; } = 65534;

			    /// <summary>
			    /// Gets or sets a value indicating whether it is necessary to remove empty space at the bottom of page
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
			    /// Gets or sets a value indicating whether it is necessary to use image resolution
			    /// </summary>
			    [DefaultValue(false)]
			    [Description("Gets or sets a value indicating whether it is necessary to use image resolution.")]
			    [StiSerializable]
			    public static bool UseImageResolution { get; set; }

                /// <summary>
                ///  Gets or sets a value indicating whether it is necessary to trim trailing spaces.
                /// </summary>
                [DefaultValue(true)]
			    [Description("Gets or sets a value indicating whether it is necessary to trim trailing spaces.")]
			    [StiSerializable]
			    public static bool TrimTrailingSpaces { get; set; } = true;

			    /// <summary>
			    ///  Gets or sets a value indicating whether it is necessary to use Footers and GropuFooters in DataOnly mode.
			    /// </summary>
			    [DefaultValue(false)]
			    [Description("Gets or sets a value indicating whether it is necessary to allow export Footers and GropuFooters in DataOnly mode.")]
			    [StiSerializable]
                [Obsolete("Please use the 'StiExcelExportSettings.DataExportMode' property.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool AllowExportFootersInDataOnlyMode { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to use image comparer.
                /// </summary>
                [DefaultValue(true)]
			    [Description("Gets or sets a value indicating whether it is necessary to use image comparer.")]
			    [StiSerializable]
			    public static bool AllowImageComparer { get; set; } = true;

			    /// <summary>
			    /// Gets or sets a value indicating whether it is necessary to use the "Freeze panes" feature of Excel.
			    /// </summary>
			    [DefaultValue(false)]
			    [Description("Gets or sets a value indicating whether it is necessary to use the Freeze Panes feature of Excel.")]
			    [StiSerializable]
			    public static bool AllowFreezePanes { get; set; }

                /// <summary>
                /// Gets or sets a value indicating a Html tags rendering mode.
                /// </summary>
                [DefaultValue(false)]
			    [Description("Gets or sets a value indicating a Html tags rendering mode.")]
			    [StiSerializable]
			    public static bool RenderHtmlTagsAsImage { get; set; }

                /// <summary>
                /// Gets or sets a value indicating a checkbox rendering mode.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value indicating a checkbox rendering mode.")]
                [StiSerializable]
                public static bool RenderCheckBoxAsImage { get; set; }

                [DefaultValue(StiExcel2007RestrictEditing.No)]
			    [StiSerializable]
			    public static StiExcel2007RestrictEditing RestrictEditing { get; set; } = StiExcel2007RestrictEditing.No;

                /// <summary>
                /// Gets or sets image format for exported images. Only for Excel2007+
                /// </summary>
                [Description("Gets or sets image format for exported images. Only for Excel2007+.")]
                [StiSerializable]
                public static ImageFormat ImageFormat { get; set; } = null; // null for auto, or ImageFormat.Jpeg or ImageFormat.Png

                /// <summary>
                /// Shrink the width of printout to fit a certain number of pages. 0 - Auto.
                /// </summary>
                [DefaultValue(1)]
                [Description("Shrink the width of printout to fit a certain number of pages. 0 - Auto.")]
                [StiSerializable]
                public static int NumberOfPagesInWideToFit { get; set; } = 1;

                /// <summary>
                /// Shrink the height of printout to fit a certain number of pages. 0 - Auto.
                /// </summary>
                [DefaultValue(0)]
                [Description("Shrink the height of printout to fit a certain number of pages. 0 - Auto.")]
                [StiSerializable]
                public static int NumberOfPagesInHeightToFit { get; set; }

                /// <summary>
                /// Gets or sets a value forcing fit to one page wide.
                /// </summary>
                [Obsolete("The property FitToOnePageWide is obsolete! Please use NumberOfPagesInWideToFit instead it.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool FitToOnePageWide
                {
                    get => NumberOfPagesInWideToFit == 1;
                    set => NumberOfPagesInWideToFit = 1;
                }


                /// <summary>
                /// Gets or sets a value of Excel sheet view mode.
                /// </summary>
                [DefaultValue(StiExcelSheetViewMode.Normal)]
                [Description("Gets or sets a value of Excel sheet view mode.")]
                [StiSerializable]
                public static StiExcelSheetViewMode SheetViewMode { get; set; } = StiExcelSheetViewMode.Normal;

                /// <summary>
                /// Gets or sets a value forcing to move and size images with cells.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value forcing to move and size images with cells.")]
                [StiSerializable]
                public static bool ImageMoveAndSizeWithCells { get; set; }

            }

            #region Excel2007
            /// <summary>
            /// Class for adjustment of the Excel export of a report.
            /// </summary>
            [Obsolete("The class StiOptions.Excel2007 is obsolete! Please use class StiOptions.Excel instead it.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public sealed class Excel2007 : Excel
			{
			}
			#endregion

            #region ExcelXml
            /// <summary>
            /// Class for adjustment of the Excel export of a report.
            /// </summary>
            [Obsolete("The class StiOptions.ExcelXml is obsolete! Please use class StiOptions.Excel instead it.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public sealed class ExcelXml : Excel
            {
            }
            #endregion
		}
    }
}
