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

using System.ComponentModel;
using System.Drawing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using System.Collections.Generic;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// Class for adjustment of the preview of a report.
        /// </summary>
        public partial class Viewer
		{
            [DefaultValue(StiViewerRefreshingMode.DataOnly)]
            [Description("This property defines how the dashboard viewer refreshes its content during refreshing via the Report.RefreshTime property.")]
            [StiSerializable]
            public static StiViewerRefreshingMode DashboardViewerRefreshing { get; set; } = StiViewerRefreshingMode.DataOnly;

            [DefaultValue(true)]
            [Description("This property disables the ability to zoom  using a combination of Ctrl + mouse wheel.")]
            [StiSerializable]
            public static bool MouseWheelZoomingEnabled { get; set; } = true;

            [DefaultValue(null)]
            [StiSerializable]
            public static int? RequestFromUserElementsPerColumn { get; set; }

            [DefaultValue(null)]
            [StiSerializable]
            public static int? RequestFromUserPanelMaxHeight { get; set; }

            /// <summary>
            /// Gets or sets color which used to mark component with Highlight Show State.
            /// </summary>
            public static Color HighlightShowStateColor { get; set; } = Color.Red;

            /// <summary>
            /// Gets or sets color which used to mark component with Highlight Active State.
            /// </summary>
            public static Color HighlightActiveStateColor { get; set; } = Color.Red;

            [DefaultValue(false)]
            [StiSerializable]
            public static bool DisableShortcutInViewer { get; set; }

            [DefaultValue(true)]
            [StiSerializable]
            public static bool HideStatusBarInDesigner { get; set; } = true;

            /// <summary>
            /// Gets or sets value which indicates that print dialog can use async mode.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets value which indicates that print dialog can use async mode.")]
            [StiSerializable]
            public static bool AllowAsyncPrinting { get; set; } = true;

            [DefaultValue(true)]
            [StiSerializable]
            public static bool AllowDrawSegmentMode { get; set; } = true;

            /// <summary>
            /// Gets or sets value which indicates that Tab control should be processed for tabulation in textbox.
            /// </summary>
            [DefaultValue(false)]
            [StiSerializable]
            public static bool AllowAcceptTabsInEditableMode { get; set; }

            [DefaultValue(false)]
            [StiSerializable]
            public static bool DisableCloseButtonInFullScreenMode { get; set; }

            [DefaultValue(true)]
            [StiSerializable]
            public static bool AllowUseDragDrop { get; set; } = true;

            [DefaultValue(null)]
            [StiSerializable]
            public static string ViewerTitle { get; set; }

            [DefaultValue(true)]
            [StiSerializable]
            public static bool ShowSuperToolTip { get; set; } = true;

            [DefaultValue(true)]
            [StiSerializable]
            public static bool ShowHyperlinkToolTip { get; set; } = true;

            [DefaultValue(null)]
            [StiSerializable]
            public static string ViewerTitleText { get; set; }

            [DefaultValue("{0} - {1}")]
            [StiSerializable]
            public static string ViewerTitleMask { get; set; } = "{0} - {1}";

            /// <summary>
            /// Gets or sets a value which controls of output of objects in the right to left mode.
            /// </summary>
            [DefaultValue(StiRightToLeftType.No)]
            [Description("Gets or sets a value which controls of output of objects in the right to left mode.")]
            [StiSerializable]
            public static StiRightToLeftType RightToLeft { get; set; } = StiRightToLeftType.No;

            [DefaultValue(true)]
            [StiSerializable]
            public static bool InteractionChartOnFirstLook { get; set; } = true;

            /// <summary>
            /// Internal use only.
            /// </summary>
            public static bool IsRightToLeft => RightToLeft == StiRightToLeftType.Yes;

		    [DefaultValue(null)]
            [StiSerializable]
            public static int? RequestFromUserDropDownWidth { get; set; }

            [DefaultValue(false)]
            [StiSerializable]
            [Description("This property is for opening sub-reports in the one tab.")]
            public static bool DrillDownSingleTab { get; set; }

            private static StiStyle interactionMouseOverStyle;
            public static StiStyle InteractionMouseOverStyle
            {
                get
                {
                    return interactionMouseOverStyle ?? (interactionMouseOverStyle = new StiStyle
                    {
                        Brush = new StiSolidBrush(Color.FromArgb(100, Color.Red))
                    });
                }
            }

            private static StiStyle interactionSelectedStyle;
            public static StiStyle InteractionSelectedStyle
            {
                get
                {
                    return interactionSelectedStyle ?? (interactionSelectedStyle = new StiStyle
                    {
                        Brush = new StiSolidBrush(Color.Red)
                    });
                }
            }

            private static StiStyle interactionMouseOverSelectedStyle;
            public static StiStyle InteractionMouseOverSelectedStyle
            {
                get
                {
                    return interactionMouseOverSelectedStyle ?? (interactionMouseOverSelectedStyle = new StiStyle
                    {
                        Brush = new StiSolidBrush(StiColorUtils.Light(Color.Red, 150))
                    });
                }
            }

            public static List<StiDateRangeKind> RequestFromUserDateRanges = new List<StiDateRangeKind>(
                new[] 
                {
                    StiDateRangeKind.Today,
                    StiDateRangeKind.Tomorrow,
                    StiDateRangeKind.Yesterday,

                    StiDateRangeKind.Last7Days,
                    StiDateRangeKind.Last14Days,
                    StiDateRangeKind.Last30Days,

                    StiDateRangeKind.NextWeek,
                    StiDateRangeKind.CurrentWeek,
                    StiDateRangeKind.PreviousWeek,

                    StiDateRangeKind.NextMonth,
                    StiDateRangeKind.CurrentMonth,
                    StiDateRangeKind.PreviousMonth,

                    StiDateRangeKind.NextQuarter,
                    StiDateRangeKind.CurrentQuarter,
                    StiDateRangeKind.PreviousQuarter,

                    StiDateRangeKind.NextYear,
                    StiDateRangeKind.CurrentYear,
                    StiDateRangeKind.PreviousYear,

                    StiDateRangeKind.FirstQuarter,
                    StiDateRangeKind.SecondQuarter,
                    StiDateRangeKind.ThirdQuarter,
                    StiDateRangeKind.FourthQuarter,

                    StiDateRangeKind.WeekToDate,
                    StiDateRangeKind.MonthToDate,
                    StiDateRangeKind.QuarterToDate,
                    StiDateRangeKind.YearToDate
                });

            #region Methods
            /// <summary>
            /// Internal use only.
            /// </summary>
            public static string GetViewerTitle()
            {
                return ViewerTitle ?? StiLocalization.Get("FormViewer", "title");
            }

            /// <summary>
            /// Internal use only.
            /// </summary>
            public static string GetViewerTitleWithMask(string file)
            {
                if (ViewerTitleText != null)
                    return ViewerTitleText;

                return file != null ? string.Format(ViewerTitleMask, file, GetViewerTitle()) : GetViewerTitle();
            }
            #endregion
        }
  
    }
}