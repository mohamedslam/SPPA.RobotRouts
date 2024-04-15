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

using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;

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
        /// Class for adjustment of configuration of a report.
        /// </summary>
        public sealed class Configuration
        {
            #region Methods
            /// <summary>
            /// Disables logging and hides messages of the report engine.
            /// </summary>
            public static void InitWeb()
            {
                StiOptions.Engine.LogEnabled = false;
                StiOptions.Engine.HideMessages = true;
            }
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets a name of the file which is used for settings of a report.
            /// </summary>
            [DefaultValue("")]
            [Description("Gets or sets a name of the file which is used for settings of a report.")]
            [StiSerializable]
            public static string DefaultReportSettingsPath { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets a name of the file which is used for configuration of a report.
            /// </summary>
            [DefaultValue("")]
            [Description("Gets or sets a name of the file which is used for configuration of a report.")]
            [StiSerializable]
            public static string DefaultReportConfigPath { get; set; } = string.Empty;

            /// <summary>
			/// Gets or sets a value which indicates whether it is necessary to use the path from the register for localization files search.
			/// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value which indicates whether it is necessary to use the path from the register for localization files search.")]
            [StiSerializable]
            public static bool SearchLocalizationFromRegistry
			{
				get
				{
					return StiLocalization.SearchLocalizationFromRegistry;
				}
				set
				{
					StiLocalization.SearchLocalizationFromRegistry = value;
				}
			}

            /// <summary>
			/// Gets or sets string containing path to directory in which files with localized resource are located.
			/// </summary>
            [DefaultValue("Localization")]
            [Description("Gets or sets string containing path to directory in which files with localized resource are located.")]
            [StiSerializable]
            public static string DirectoryLocalization
			{
				get
				{
					return StiLocalization.DirectoryLocalization;
				}
				set
				{
					StiLocalization.DirectoryLocalization = value;
				}
			}
		
			/// <summary>
			/// Gets or sets name of file with localized resource.
			/// </summary>
            [DefaultValue("")]
            [Description("Gets or sets name of file with localized resource.")]
            [StiSerializable]
            public static string Localization
			{
				get
				{
					return StiLocalization.Localization;
				}
				set
				{
					StiLocalization.Localization = value;
				}
			}

            /// <summary>
            /// Gets or sets an application directory.
            /// </summary>
            public static string ApplicationDirectory { get; set; }
            
            private static bool isWeb;
            /// <summary>
            /// Gets a value which indicates whether configuration is used for the web report.
            /// </summary>
            public static bool IsWeb
            {
                get
                {
                    return isWeb;
                }
                set
                {
                    if (isWeb != value)
                    {
                        isWeb = value;
                        if (value)
                            InitWeb();
                    }
                }
            }

            /// <summary>
            /// Gets a value which indicates whether configuration is used for the WinForms report.
            /// </summary>
            public static bool IsWinForms => Application.OpenForms.Count > 0;

            /// <summary>
            /// Gets a value which indicates whether configuration is used for the WPF report.
            /// </summary>
            public static bool IsWPF { get; set; }

            public static bool IsWPFV2 { get; set; }

            public static bool IsForm { get; set; }

            /// <summary>
            /// Gets a value which indicates whether configuration is used for the WPF report.
            /// </summary>
            [Obsolete("The 'IsXbap' property is deprecated!")]
            public static bool IsXbap { get; set; }

            /// <summary>
            /// Gets a value which indicates whether configuration is used for the Server.
            /// </summary>
            public static bool IsServer { get; set; }
            #endregion

            static Configuration()
            {
                try
                {
                    var a = Assembly.GetEntryAssembly();
                    ApplicationDirectory = a != null ? Path.GetDirectoryName(a.Location) : AppDomain.CurrentDomain.BaseDirectory;
                }
                catch
                {
                }
            }
        }
    }
}