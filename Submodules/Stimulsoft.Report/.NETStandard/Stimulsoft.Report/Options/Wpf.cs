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

using Stimulsoft.Base.Design;
using Stimulsoft.Base.Serializing;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
    {
        public sealed class Wpf
        {
            #region WpfBrowserApplication
            public sealed class WpfBrowserApplication
            {
                [DefaultValue(false)]
                [StiSerializable]
                public static bool IsWpfBrowserApplication { get; set; }
            }
            #endregion

            #region Viewer
            public sealed class Viewer
            {
                [DefaultValue(false)]
                [StiSerializable]
                public static bool UsePageShadow { get; set; } = true;

                #region CollapsingImages
                public sealed class CollapsingImages
                {
                    [DefaultValue(false)]
                    [StiSerializable]
                    public static bool UseCustomCollapsingImages { get; set; }

                    [DefaultValue(null)]
                    [StiSerializable]
                    public static string CollapsedImagePath { get; set; }

                    [DefaultValue(null)]
                    [StiSerializable]
                    public static string ExpandedImagePath { get; set; }
                }
                #endregion
            }
            #endregion

            #region ThemeNames
            [Obsolete()]
            public static class Themes
            {
                internal static StiWpfThemeInfo BaseOffice2013Theme { get; set; }

                public static List<StiWpfThemeInfo> ThemeList { get; set; } = new List<StiWpfThemeInfo>();
                
                /// <summary>
                /// Gets name of current theme.
                /// </summary>
                public static string DefaultTheme => Office2013Theme;

                /// <summary>
                /// Gets name of Office 2013 theme.
                /// </summary>
                public static string Office2013Theme => "Office 2013";

                /// <summary>
                /// Gets name of Office 2010 White theme.
                /// </summary>
                public static string Office2010WhiteTheme => "Office 2010 White";

                /// <summary>
                /// Gets name of Office 2010 Blue theme.
                /// </summary>
                public static string Office2010BlueTheme => "Office 2010 Blue";
                
                /// <summary>
                /// Gets name of Office 2007 Blue theme.
                /// </summary>
                public static string Office2007BlueTheme => "Office 2007 Blue";

                /// <summary>
                /// Gets name of Office 2007 Silver theme.
                /// </summary>
                public static string Office2007SilverTheme => "Office 2007 Silver";

                /// <summary>
                /// Gets name of Office 2007 Black theme.
                /// </summary>
                public static string Office2007BlackTheme => "Office 2007 Black";

                /// <summary>
                /// Gets name of Office 2003 Blue theme.
                /// </summary>
                public static string Office2003BlueTheme => "Office 2003 Blue";

                /// <summary>
                /// Gets name of Office 2003 Olive Green theme.
                /// </summary>
                public static string Office2003OliveGreenTheme => "Office 2003 Olive Green";

                /// <summary>
                /// Gets name of Office 2003 Silver theme.
                /// </summary>
                public static string Office2003SilverTheme => "Office 2003 Silver";

                /// <summary>
                /// Gets name of Black theme.
                /// </summary>
                public static string BlackTheme => "Black";
            }
            #endregion

            #region Properties
            [Obsolete("This property is obsolete!")]
            [DefaultValue(StiLoadThemeMode.OnceForTheWholeApp)]
            [StiSerializable]
            public static StiLoadThemeMode LoadThemeMode = StiLoadThemeMode.OnceForTheWholeApp;

            [DefaultValue(true)]
            [StiSerializable]
            public static bool DisableAllAnimationsInViewer { get; set; } = true;

            [DefaultValue(true)]
            [StiSerializable]
            public static bool LoadTheme { get; set; } = true;

            [DefaultValue(false)]
            [StiSerializable]
            public static bool UseGDIPrintingInWPFViewer { get; set; }

            [DefaultValue(true)]
            [StiSerializable]
            public static bool UseNewWpfPrintingMethod { get; set; } = true;

            [DefaultValue(false)]
            [StiSerializable]
            public static bool AllowPrintingProgress { get; set; }

            /// <summary>
            /// This property let to show in WPF designer properties of components, which unavailable in WPF.
            /// </summary>
            [Description("This property let to show in WPF designer properties of components, which unavailable in WPF.")]
            public static bool ShowAllPropertiesInWpf
            {
                get
                {
                    return StiPropertiesTab.ShowAllPropertiesInWpf;
                }
                set
                {
                    StiPropertiesTab.ShowAllPropertiesInWpf = value;
                }
            }

            public static bool IsGuiTypeSelected
            {
                get
                {
                    StiSettings.Load();
                    return StiSettings.GetBool("GuiHelper", "IsGuiTypeSelected", false);
                }
                set
                {
                    StiSettings.Load();
                    StiSettings.Set("GuiHelper", "IsGuiTypeSelected", value);
                    StiSettings.Save();
                }
            }

            /// <summary>
            /// Gets or sets name of current theme for Reports.Wpf applications.
            /// </summary>
            public static string CurrentTheme => "Office 2013";
            #endregion

            #region Methods
            [Obsolete()]
            public static StiWpfThemeInfo GetThemeInfoForCurrentTheme() => null;
            #endregion
        }
    }
}