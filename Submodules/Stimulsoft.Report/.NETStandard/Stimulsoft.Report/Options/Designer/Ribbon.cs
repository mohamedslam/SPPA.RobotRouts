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
            public static class Ribbon
            {
                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowOfficeButton { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowLocalizationMenu { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowInfo { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowHelp { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowGetStarted { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowSchedule { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowClose { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowAbout { get; set; } = true;

                #region Properties.Show.TabHome
                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowTabHome { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowTabHomeBarClipboard { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowTabHomeBarFont { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowTabHomeBarAlignment { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowTabHomeBarBorders { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowTabHomeBarTextFormat { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowTabHomeBarStyle { get; set; } = true;
                #endregion

                #region Properties.Show.TabPage
                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowTabPage { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowTabPageBarPageSetup { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                [Obsolete("ShowTabPageBarWatermarkText property is not used more!")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool ShowTabPageBarWatermarkText { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                [Obsolete("ShowTabPageBarWatermarkImage property is not used more!")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool ShowTabPageBarWatermarkImage { get; set; } = true;
                #endregion

                #region Properties.Show.TabLayout
                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowTabLayout { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowTabLayoutBarArrange { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                [Obsolete("ShowTabLayoutBarDockStyle property is not used more!")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool ShowTabLayoutBarDockStyle { get; set; } = true;
                #endregion

                #region Properties.Show.TabView
                [DefaultValue(true)]
                [StiSerializable]
                [Obsolete("ShowMainMenuPageNew property is not used more!")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool ShowTabView { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowTabViewBarViewOptions { get; set; } = true;
                #endregion

                #region Properties.Show.TabPreview
                [DefaultValue(false)]
                [StiSerializable]
                public static bool ShowTabPreview { get; set; } = true;
                #endregion

                #region Properties.Show.Quick
                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowQuickReportOpen { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowQuickReportSave { get; set; } = true;
                #endregion

                #region Properties.Show.MainMenu
                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowMainMenuNew { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowMainMenuReportNew { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                [Obsolete("ShowMainMenuReportWizardNew property is not used more!")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool ShowMainMenuReportWizardNew { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                [Obsolete("ShowMainMenuPageNew property is not used more!")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool ShowMainMenuPageNew { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                [Obsolete("ShowMainMenuFormNew property is not used more!")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool ShowMainMenuFormNew { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowMainMenuReportOpen { get; set; } = true;

                [Obsolete("Field 'ShowMainMenuReportOpenFromGoogleDocs' is not used more!")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool ShowMainMenuReportOpenFromGoogleDocs { get; set; }

                [DefaultValue(true)]
                [StiSerializable]
                [Obsolete("ShowMainMenuPageOpen property is not used more!")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool ShowMainMenuPageOpen { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowMainMenuReportSave { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                [Obsolete("ShowMainMenuSaveAs property is not used more! Please use ShowMainMenuReportSaveAs property instead.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool ShowMainMenuSaveAs
                {
                    get
                    {
                        return ShowMainMenuReportSaveAs;
                    }
                    set
                    {
                        ShowMainMenuReportSaveAs = value;
                    }
                }

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowMainMenuReportSaveAs { get; set; } = true;

                [Obsolete("Field 'ShowMainMenuReportSaveAsToGoogleDocs' is not used more!")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool ShowMainMenuReportSaveAsToGoogleDocs { get; set; }

                [DefaultValue(true)]
                [StiSerializable]
                [Obsolete("ShowMainMenuPageSaveAs property is not used more!")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool ShowMainMenuPageSaveAs { get; set; } = true;
                
                [DefaultValue(true)]
                [StiSerializable]
                [Obsolete("ShowMainMenuPageDelete property is not used more!")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool ShowMainMenuPageDelete { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowMainMenuReportPreview { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowMainMenuCheckForIssues { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowMainMenuReportSetup { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowMainMenuRecentFiles { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowMainMenuClose { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                [Obsolete("ShowMainMenuDesigner property is not used more!")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool ShowMainMenuDesigner { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                [Obsolete("ShowMainMenuReport property is not used more!")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool ShowMainMenuReport { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                [Obsolete("ShowMainMenuPage property is not used more!")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool ShowMainMenuPage { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                [Obsolete("ShowMainMenuReportPrint property is not used more!")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool ShowMainMenuReportPrint { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowMainMenuOptions { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowMainMenuExit { get; set; } = true;

                [DefaultValue(false)]
                [StiSerializable]
                public static bool ShowMainMenuHelp { get; set; } = false;
                #endregion
            }            
		}
    }
}