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

using Stimulsoft.Base.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Stimulsoft.Base.Helpers
{
    public static class StiWindowsSettings
    {
        #region class StiWindowState
        public class StiWindowState
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            [DefaultValue(0)]
            public int WindowState { get; set; }
        }
        #endregion

        #region class StiContainerInfo
        private class StiContainerInfo
        {
            public Dictionary<string, StiWindowState> Data = new Dictionary<string, StiWindowState>();
        }
        #endregion

        #region Fields
        private static Dictionary<string, StiWindowState> Data;
        private static bool isChanged;
        #endregion

        #region Methods
        public static StiWindowState Get(string key)
        {
            if (Data == null)
                Load();

            Data.TryGetValue(key, out StiWindowState state);
            return state;
        }

        public static void Set(string key, StiWindowState state)
        {
            if (Data == null)
                Load();

            if (Data.ContainsKey(key))
                Data.Remove(key);

            Data.Add(key, state);
            isChanged = true;
        }

        public static string GetSettingsPath()
        {
            string path = "Stimulsoft" + Path.DirectorySeparatorChar + "WindowsSettings.json";
            string folder;

            try
            {
                if (StiBaseOptions.FullTrust)
                {
                    folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    if (folder.Length != 0)
                        path = Path.Combine(folder, path);
                }

                folder = Path.GetDirectoryName(path);

                if (StiBaseOptions.FullTrust && !string.IsNullOrWhiteSpace(folder) && !Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
            }
            catch
            {
                try
                {
                    path = "Stimulsoft" + Path.DirectorySeparatorChar + "WindowsSettings.json";
                    folder = "Stimulsoft";

                    if (StiBaseOptions.FullTrust && !Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                }
                catch
                {
                    path = "WindowsSettings.json";
                }
            }

            return path;
        }

        public static void Load()
        {
            try
            {
                var path = GetSettingsPath();
                if (File.Exists(path))
                {
                    var text = File.ReadAllText(path);
                    var container = JsonConvert.DeserializeObject<StiContainerInfo>(text);

                    Data = container.Data;
                }
            }
            catch
            {
            }

            if (Data == null)
                Data = new Dictionary<string, StiWindowState>();
        }

        public static void Save()
        {
            var path = GetSettingsPath();
            if (!isChanged && File.Exists(path)) return;

            try
            {
                var container = new StiContainerInfo
                {
                    Data = Data
                };

                var dir = Path.GetDirectoryName(path);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var json = JsonConvert.SerializeObject(container, Formatting.Indented);

                StiFileUtils.ProcessReadOnly(path);
                File.WriteAllText(path, json);
            }
            catch
            {

            }
        }
        #endregion
    }
}