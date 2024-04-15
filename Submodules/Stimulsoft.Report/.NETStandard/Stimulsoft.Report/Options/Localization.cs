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

using System.IO;

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
        public sealed class Localization
        {
            #region Fields
            private static bool isLoaded;
            #endregion

            #region Methods
            /// <summary>
            /// Loads localization from the xml file.
            /// </summary>
            /// <param name="file">A file from which localization will be loaded.</param>
            public static void Load(string file)
            {
                isLoaded = false;
                StiLocalization.Load(file);
                isLoaded = true;
            }

            /// <summary>
            /// Loads localization from the stream.
            /// </summary>
            /// <param name="stream">A stream from which localization will be loaded.</param>
            public static void Load(Stream stream)
            {
                isLoaded = false;
                StiLocalization.Load(stream);
                isLoaded = true;
            }

            public static string GetDirectory()
            {
                var dir = GetDirectoryLocalizationFromSettings();
                if (dir != null) return dir;

                dir = StiLocalization.GetDirectoryLocales();
                if (dir != null) return dir;

                dir = StiLocalization.GetDirectoryLocalizationFromRegistry();
                if (dir != null) return dir;

                return Configuration.DirectoryLocalization;
            }

            private static string GetDirectoryLocalizationFromSettings()
            {
                try
                {
                    StiSettings.Load();
                    return StiSettings.GetStr("Localization", "DirectoryLocalization", null);
                }
                catch
                {
                }

                return null;
            }

            public static bool TryLoadDefault()
            {
                try
                {
                    LoadDefault();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public static void LoadDefault()
            {
                // Если это Server, то ничего грузить не надо, мы там сами все делаем(иначе перетираются наши загруженные локализации)
                if (StiOptions.Configuration.IsServer) return;

                StiSettings.Load();
                if (isLoaded) return;

                var dir = GetDirectory();
                var file = StiSettings.GetStr("Localization", "FileName", null);
                if (file == null)
                    file = Configuration.Localization;

                StiLocalization.DirectoryLocalization = dir;
                StiLocalization.Localization = file;

                StiLocalization.LoadCurrentLocalization();

                isLoaded = true;
            }
            #endregion
        }
    }
}