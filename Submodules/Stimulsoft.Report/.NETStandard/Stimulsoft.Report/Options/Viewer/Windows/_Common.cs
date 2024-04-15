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
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

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
            /// <summary>
            /// Class for adjustment of the viewer window.
            /// </summary>
            public partial class Windows
            {
                #region Obsolete
                [Obsolete("Please use property 'ShowInTaskbar' instead property 'ForceShowInTaskbar'.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool ForceShowInTaskbar { get; set; } = true;

                /// <summary>
                /// Gets or sets a value indicating whether the Export button is visible in the viewer window.
                /// </summary>
                [Obsolete("Please use ShowSaveButton property instead ShowExportButton property.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool? ShowExportButton
                {
                    get
                    {
                        return ShowSaveButton;
                    }
                    set
                    {
                        ShowSaveButton = value;
                    }
                }

                [Obsolete("Please use property 'ViewerIcon' instead 'PreviewIcon' property.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static Icon PreviewIcon
                {
                    get
                    {
                        return ViewerIcon;
                    }
                    set
                    {
                        ViewerIcon = value;
                    }
                }

                /// <summary>
                /// Gets or sets window state of the viewer window.
                /// </summary>
                [Obsolete("Please use property 'ViewerWindowState' instead 'PreviewWindowState' property.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static FormWindowState PreviewWindowState
                {
                    get
                    {
                        return ViewerWindowState;
                    }
                    set
                    {
                        ViewerWindowState = value;
                    }
                }
                #endregion


                /// <summary>
				/// Gets or sets zoom of the drawing of the pages in viewer window.
				/// </summary>
                [Description("Gets or sets zoom of the drawing of the pages in viewer window.")]
                [StiSerializable]
                [DefaultValue(1d)]
                public static double Zoom
                {
                    get
                    {
                        StiSettings.Load();
                        return StiSettings.GetDouble("Viewer", "Zoom", 1d);
                    }
                    set
                    {
                        StiSettings.Load();
                        StiSettings.Set("Viewer", "Zoom", value);
                        StiSettings.Save();
                    }
                }

                /// <summary>
                /// Gets or sets a value indicating whether to show a Toolbar in the viewer control.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether to show a Toolbar in the viewer control.")]
                [StiSerializable]
                public static bool? ShowToolbar { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether to show all buttons for the Page View Mode. 
                /// </summary>
                [Description("Gets or sets a value indicating whether to show all buttons for the Page View Mode. ")]
                [StiSerializable]
                [DefaultValue(null)]
                public static bool? ShowPageViewMode
                {
                    get
                    {
                        if (ShowPageViewSingleMode == null && ShowPageViewContinuousMode == null && ShowPageViewMultipleMode == null) return null;

                        return
                            ShowPageViewSingleMode.GetValueOrDefault(true) &&
                            ShowPageViewContinuousMode.GetValueOrDefault(true) &&
                            ShowPageViewMultipleMode.GetValueOrDefault(true);
                    }
                    set
                    {
                        ShowPageViewSingleMode = value;
                        ShowPageViewContinuousMode = value;
                        ShowPageViewMultipleMode = value;
                    }
                }

                /// <summary>
                /// Gets or sets a value indicating whether to show Single Page Mode. 
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether to show Single Page Mode. ")]
                [StiSerializable]
                public static bool? ShowPageViewSingleMode { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether to show Continues Page Mode.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether to show Continues Page Mode.")]
                [StiSerializable]
                public static bool? ShowPageViewContinuousMode { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether to show MultiplePages Page Mode.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether to show MultiplePages Page Mode.")]
                [StiSerializable]
                public static bool? ShowPageViewMultipleMode { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether to show "FullScreen" button.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether to show 'FullScreen' button.")]
                [StiSerializable]
                public static bool? ShowFullScreen { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether to show the Vertical Scroll Bar.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether to show the Vertical Scroll Bar.")]
                [StiSerializable]
                public static bool? ShowVertScrollBar { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the Status Bar is shown in the viewer window.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the Status Bar is shown in the viewer window.")]
                [StiSerializable]
                public static bool? ShowStatusBar { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the Print Button is shown.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the Print Button is shown.")]
                [StiSerializable]
                public static bool? ShowPrintButton { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the Open button is visible in the viewer window.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the Open button is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowOpenButton { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the ResetAllFilters button is visible in the viewer window.
                /// This property works only for the dashboard viewer.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the ResetAllFilters button is visible in the viewer window." +
                    "This property works only for the dashboard viewer.")]
                [StiSerializable]
                public static bool? ShowResetAllFiltersButton { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the Refreh button is visible in the viewer window.
                /// This property works only for the dashboard viewer.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the Refreh button is visible in the viewer window." +
                    "This property works only for the dashboard viewer.")]
                [StiSerializable]
                public static bool? ShowRefreshButton { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the Save button is visible in the viewer window.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the Save button is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowSaveButton { get; set; }

                /// <summary>
				/// Gets or sets a value indicating whether the Save Document File button is visible in the viewer window.
				/// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the Save Document File button is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowSaveDocumentFileButton { get; set; }

                /// <summary>
				/// Gets or sets a value indicating whether the SendEMail Document File button is visible in the viewer window.
				/// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the SendEMail Document File button is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowSendEMailDocumentFileButton { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the Send EMail button is visible in the viewer window.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the Send EMail button is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowSendEMailButton { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the Show New Page button is visible in the viewer window.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the Show New Page button is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowPageNewButton { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the Delete Page button is visible in the viewer window.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the Delete Page button is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowPageDeleteButton { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the Page Design button is visible in the viewer window.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the Page Design button is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowPageDesignButton { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the Page Size button is visible in the viewer window.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the Page Size button is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowPageSizeButton { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether to show button for select mode of viewing pages in viewer window.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether to show button for select mode of viewing pages in viewer window.")]
                [StiSerializable]
                public static bool? ShowPageViewModeButton { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the Select Tool button is visible in the viewer window.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the Select Tool button is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowSelectTool { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the Hand Tool button is visible in the viewer window.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the Hand Tool button is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowHandTool { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the Editor Tool button is visible in the viewer window.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether the Editor Tool button is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowEditorTool { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the Editor Tool button is visible in the viewer window.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether the Signature Tool button is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowSignatureTool { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the Find Tool button is visible in the viewer window.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the Find Tool button is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowFindTool { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the Zoom panel is visible in the viewer window.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the Zoom panel is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowZoom { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the Zoom panel is visible in the viewer window.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the Zoom panel is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowPageControl { get; set; }

                /// <summary>
				/// Gets or sets a value indicating whether the "First Page" button is visible in the viewer window.
				/// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the 'First Page' button is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowFirstPage { get; set; }

                /// <summary>
				/// Gets or sets a value indicating whether the "Previous Page" button is visible in the viewer window.
				/// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the 'Previous Page' button is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowPreviousPage { get; set; }

                /// <summary>
				/// Gets or sets a value indicating whether the "Next Page" button is visible in the viewer window.
				/// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the 'Next Page' button is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowNextPage { get; set; }

                /// <summary>
				/// Gets or sets a value indicating whether the "Last Page" button is visible in the viewer window.
				/// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the 'Last Page' button is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowLastPage { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the BookMarks panel is visible in the viewer window.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the BookMarks panel is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowBookmarksPanel { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the Thumbnails panel is visible in the viewer window.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the Thumbnails panel is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowThumbsPanel { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the Thumbnails button is visible in the viewer window.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the Thumbnails button is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowThumbsButton { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the Parameters button is visible in the viewer window.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the Parameters button is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowParametersButton { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the Parameters button is visible in the viewer window.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether the Resources button is visible in the viewer window.")]
                [StiSerializable]
                public static bool? ShowResourcesButton { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether it is will be show in context menu of viewer window.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether it is will be show in context menu of viewer window.")]
                [StiSerializable]
                public static bool? ShowContextMenu { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether it is will be show Close Button in the viewer window.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether it is will be show Close Button in the viewer window.")]
                [StiSerializable]
                public static bool? ShowCloseButton { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether it is will be show Help Button in the viewer window.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating whether it is will be show Help Button in the viewer window.")]
                [StiSerializable]
                public static bool? ShowHelpButton { get; set; }

                /// <summary>
                /// Gets or sets path for saving and loading a report template.
                /// </summary>
                [DefaultValue("")]
                [Description("Gets or sets path for saving and loading a report template.")]
                [StiSerializable]
                public static string ReportSaveLoadPath { get; set; } = string.Empty;

                /// <summary>
                /// Gets or sets path for saving exported report.
                /// </summary>
                [DefaultValue("")]
                [Description("Gets or sets path for saving exported report.")]
                [StiSerializable]
                public static string ExportSaveLoadPath { get; set; } = string.Empty;

                /// <summary>
                /// Gets or sets value which that current page selection will be shown.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets value which that current page selection will be shown.")]
                [StiSerializable]
                public static bool ShowPageSelection { get; set; } = true;

                /// <summary>
                /// Gets or sets window state of the viewer window.
                /// </summary>
                [DefaultValue(FormWindowState.Maximized)]
                [Description("Gets or sets window state of the viewer window.")]
                [StiSerializable]
                public static FormWindowState ViewerWindowState { get; set; } = FormWindowState.Maximized;

                [DefaultValue(true)]
                [Description("")]
                [StiSerializable]
                public static bool ScrollToEditableComponent { get; set; } = true;

                [DefaultValue(false)]
                [StiSerializable]
                public static bool LimitLengthOfEditableRichText { get; set; }

                public static object ActivePageBorderColor { get; set; }

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowInTaskbar { get; set; } = true;

                [StiSerializable]
                public static Size MinimumSize { get; set; } = Size.Empty;


                public static Color BackgroundColor { get; set; } = Color.Empty;


                public static Icon ViewerIcon { get; set; }


                public static object ViewerWpfIcon { get; set; }

                [DefaultValue(true)]
                [StiSerializable]
                [Description("Allow the Animation component in the viewer.")]
                public static bool AllowAnimation
                {
                    get
                    {
                        StiSettings.Load();
                        return StiSettings.GetBool("Viewer", "AllowAnimation", true);
                    }
                    set
                    {
                        StiSettings.Load();
                        StiSettings.Set("Viewer", "AllowAnimation", value);
                        StiSettings.Set("Viewer", "AnimationPlaybackType",
                            value ?
                            StiAnimationPlaybackType.OnStart | StiAnimationPlaybackType.OnPreview :
                            StiAnimationPlaybackType.None);
                        StiSettings.Save();
                    }
                }

                [DefaultValue(StiAnimationPlaybackType.OnStart | StiAnimationPlaybackType.OnPreview)]
                [StiSerializable]
                public static StiAnimationPlaybackType AnimationPlaybackType
                {
                    get
                    {
                        StiSettings.Load();
                        return (StiAnimationPlaybackType)StiSettings.Get("Viewer", "AnimationPlaybackType",
                            StiSettings.GetBool("Viewer", "AllowAnimation", true) ?
                            StiAnimationPlaybackType.OnStart | StiAnimationPlaybackType.OnPreview :
                            StiAnimationPlaybackType.None);
                    }
                    set
                    {
                        StiSettings.Load();
                        StiSettings.Set("Viewer", "AnimationPlaybackType", value);
                        StiSettings.Set("Viewer", "AllowAnimation", value != StiAnimationPlaybackType.None);
                        StiSettings.Save();
                    }
                }
            }
        }
    }
}