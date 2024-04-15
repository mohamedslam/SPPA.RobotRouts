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
using Stimulsoft.Base.Serializing;
using System;

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
            public sealed class MainMenu
            {
                #region Import
                public delegate StiImportResult ImportActiveReportsDelegate(byte[] bytes);
                public static ImportActiveReportsDelegate ImportActiveReports;
                
                public delegate StiImportResult ImportComponentOneReportsDelegate(byte[] bytes);
                public static ImportComponentOneReportsDelegate ImportComponentOneReports;

                public delegate StiImportResult ImportCrystalReportDelegate(byte[] bytes);
                public static ImportCrystalReportDelegate ImportCrystalReport;

                public delegate StiImportResult ImportFastReportsDelegate(byte[] bytes);
                public static ImportFastReportsDelegate ImportFastReports;

                public delegate StiImportResult ImportReportSharpShooterDelegate(byte[] bytes);
                public static ImportReportSharpShooterDelegate ImportReportSharpShooter;

                public delegate StiImportResult ImportRtfDelegate(byte[] bytes);
                public static ImportRtfDelegate ImportRtf;

                public delegate StiImportResult ImportTelerikReportsDelegate(byte[] bytes);
                public static ImportTelerikReportsDelegate ImportTelerikReports;

                public delegate StiImportResult ImportVisualFoxProDelegate(byte[] dataBytes, byte[] memoBytes);
                public static ImportVisualFoxProDelegate ImportVisualFoxPro;

                public delegate StiImportResult ImportMicrosoftReportingServicesDelegate(byte[] bytes);
                public static ImportMicrosoftReportingServicesDelegate ImportMicrosoftReportingServices;
                #endregion

                #region File
                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowFile { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowFileReportNew { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowFilePageNew { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowFileFormNew { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowFileDashboardNew { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowFileScreenNew { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowFileReportOpen { get; set; } = true;

                [Obsolete("Field 'ShowFileReportOpenFromGoogleDocs' is not used more!")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool ShowFileReportOpenFromGoogleDocs { get; set; }
                
                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowFilePageOpen { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowFileReportSave { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowFileReportSaveAs { get; set; } = true;


                [Obsolete("Field 'ShowFileReportSaveAsToGoogleDocs' is not used more!")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool ShowFileReportSaveAsToGoogleDocs { get; set; }

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowFilePageSaveAs { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowFilePageDelete { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowFileReportPreview { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowFilePageSetup { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowFileReportSetup { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowSelectLanguage { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowFileRecentFiles { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowOnlyExistingRecentFiles { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowFileExit { get; set; } = true;
                #endregion

                #region Edit
                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowEdit { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowEditUndo { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowEditRedo { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowEditCut { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowEditCopy { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowEditPaste { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowEditDelete { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowEditSelectAll { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowEditClearContents { get; set; } = true;
                #endregion

                #region View
                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowView { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowViewNormal { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowViewPageBreakPreview { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowViewShowGrid { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowViewAlignToGrid { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowViewShowHeaders { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowViewShowRulers { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowViewShowOrder { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowViewToolbars { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowViewPanels { get; set; } = true;
                #endregion

                #region Tools
                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowTools { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowToolsDataStore { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowToolsPagesManager { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowToolsServicesConfigurator { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowToolsOptions { get; set; } = true;
                #endregion

                #region Help
                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowHelp { get; set; } = true;

                [DefaultValue("https://stimulsoft.com/")]
                [StiSerializable]
                public static string ProductHomePage { get; set; } = "https://stimulsoft.com/";

                [DefaultValue("https://www.stimulsoft.com/")]
                [StiSerializable]
                public static string ProductHomePageWPF { get; set; } = "https://www.stimulsoft.com/";

                [DefaultValue("https://www.stimulsoft.com/en/faq/reports-net")]
                [StiSerializable]
                public static string FaqPage { get; set; } = "https://www.stimulsoft.com/en/faq/reports-net";

                [DefaultValue("https://www.stimulsoft.com/en/faq/reports-net")]
                public static string FaqPageWPF { get; set; } = "https://www.stimulsoft.com/en/faq/reports-net";

                [DefaultValue("mailto:support@stimulsoft.com")]
                [StiSerializable]
                public static string Support { get; set; } = "mailto:support@stimulsoft.com";

                [DefaultValue("https://www.stimulsoft.com/en/support")]
                [StiSerializable]
                public static string SupportPage { get; set; } = "https://www.stimulsoft.com/en/support";

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowHelpProductHomePage { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowHelpFaqPage { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowHelpSupport { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowHelpAboutProgramm = true;
                #endregion
            }
		}
    }
}