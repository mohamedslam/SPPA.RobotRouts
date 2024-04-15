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
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Stimulsoft.Base.Helpers
{
    public static class StiColorDialogHelper
    {
        #region Fields
        private static readonly object sync = new object();
        #endregion

        #region Methods

        private static string GetDefaultReportSettingsPath()
        {
            string path = "Stimulsoft\\ColorDialog_CustomColors.json";
            string folder = null;
            if (StiBaseOptions.FullTrust)
            {
                folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                if (folder.Length != 0) path = Path.Combine(folder, path);
            }

            folder = Path.GetDirectoryName(path);
            if (StiBaseOptions.FullTrust && !Directory.Exists(folder)) Directory.CreateDirectory(folder);

            return path;
        }

        public static int[] Load()
        {
            Monitor.Enter(sync);
            try
            {
                string fileName = GetDefaultReportSettingsPath();
                if (File.Exists(fileName))
                {
                    var colors = new List<int>();
                    string json = File.ReadAllText(fileName);

                    if (!string.IsNullOrEmpty(json))
                    {
                        StiJsonHelper.LoadFromJsonString(json, colors);
                        if (colors.Count == 0)
                            return null;

                        return colors.ToArray();
                    }
                }
            }
            catch
            {

            }
            finally
            {
                Monitor.Exit(sync);
            }

            return null;
        }

        public static void Save(int[] colors)
        {
            int index = 0;

            string file = GetDefaultReportSettingsPath();
            while (true)
            {
                bool final = false;
                Monitor.Enter(sync);
                try
                {
                    StiFileUtils.ProcessReadOnly(file);
                    if (File.Exists(file))
                        File.Delete(file);

                    if (colors == null || colors.Length == 0)
                        return;

                    var directory = Path.GetDirectoryName(file);
                    if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                    var json = StiJsonHelper.SaveToJsonString(new List<int>(colors));
                    File.WriteAllText(file, json);
                }
                catch
                {
                    final = true;
                }
                finally
                {
                    Monitor.Exit(sync);
                }

                if (!final || index > 4)
                    break;
                index++;
            }
        }

        #endregion
    }
}
