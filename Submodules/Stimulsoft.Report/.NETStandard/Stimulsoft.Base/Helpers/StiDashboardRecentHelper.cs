#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Base;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Stimulsoft.Report.Dashboard.Helpers
{
    public static class StiDashboardRecentHelper
    {
        #region class FilesContainer
        private class FilesContainer
        {
            public List<string> DbsFiles { get; set; }

            public List<string> ReportFile { get; set; }
        }
        #endregion

        #region Fields
        private static List<string> dbsFiles;
        private static List<string> reportFiles;
        #endregion

        #region Methods
        private static string GetSettingsPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Stimulsoft\\DashboardRecent.json");
        }

        private static string GetNewSettingsPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Stimulsoft\\DbsRecent.json");
        }

        public static bool Save()
        {
            if (dbsFiles == null)
                return false;

            try
            {
                var container = new FilesContainer
                {
                    DbsFiles = dbsFiles,
                    ReportFile = reportFiles
                };

                var path = GetNewSettingsPath();
                var json = JsonConvert.SerializeObject(container, Formatting.Indented);
                StiFileUtils.ProcessReadOnly(path);
                File.WriteAllText(path, json);
            }
            catch
            {
                return true;
            }

            return true;
        }

        private static void Load()
        {
            if (dbsFiles != null) return;

            try
            {
                var path = GetNewSettingsPath();
                if (File.Exists(path))
                {
                    var text = File.ReadAllText(path);
                    var container = JsonConvert.DeserializeObject<FilesContainer>(text);

                    dbsFiles = container.DbsFiles;
                    reportFiles = container.ReportFile;
                }
            }
            catch
            {
            }

            if (dbsFiles == null || reportFiles == null)
            {
                try
                {
                    var path = GetSettingsPath();
                    if (File.Exists(path))
                    {
                        var text = File.ReadAllText(path);
                        dbsFiles = JsonConvert.DeserializeObject<List<string>>(text);
                        reportFiles = new List<string>();

                        // Save new file settings and delete old file settings
                        if (Save())
                            File.Delete(path);
                    }
                }
                catch
                {
                }
            }

            if (dbsFiles == null)
                dbsFiles = new List<string>();

            if (reportFiles == null)
                reportFiles = new List<string>();
        }

        public static void Add(bool containsDashboards, string path, bool autoSave = true)
        {
            if (path == null) return;

            Load();

            bool isChagned = false;
            var pathLower = path.ToLowerInvariant();
            if (containsDashboards)
            {
                if (!dbsFiles.Contains(pathLower))
                {
                    dbsFiles.Add(pathLower);
                    isChagned = true;
                }

                if (reportFiles.Contains(pathLower))
                {
                    reportFiles.Remove(pathLower);
                    isChagned = true;
                }
            }
            else
            {
                if (!reportFiles.Contains(pathLower))
                {
                    reportFiles.Add(pathLower);
                    isChagned = true;
                }

                if (dbsFiles.Contains(pathLower))
                {
                    dbsFiles.Remove(pathLower);
                    isChagned = true;
                }
            }

            if (isChagned && autoSave)
                Save();
        }

        public static void Add(string path, bool autoSave = true)
        {
            if (path == null) return;
            Load();

            bool isChagned = false;
            var pathLower = path.ToLowerInvariant();

            var containsDbs = StiReportContentHelper.ContainsDbs(path);
            if (containsDbs)
            {
                if (!dbsFiles.Contains(pathLower))
                {
                    dbsFiles.Add(pathLower);
                    isChagned = true;
                }

                if (reportFiles.Contains(pathLower))
                {
                    reportFiles.Remove(pathLower);
                    isChagned = true;
                }
            }
            else
            {
                if (!reportFiles.Contains(pathLower))
                {
                    reportFiles.Add(pathLower);
                    isChagned = true;
                }

                if (dbsFiles.Contains(pathLower))
                {
                    dbsFiles.Remove(pathLower);
                    isChagned = true;
                }
            }

            if (isChagned && autoSave)
                Save();
        }

        public static void Remove(string path)
        {
            if (path == null) return;

            Load();

            var pathLower = path.ToLowerInvariant();

            bool isChanged = false;
            if (dbsFiles.Contains(pathLower))
            {
                dbsFiles.Remove(pathLower);
                isChanged = true;
            }
            if (reportFiles.Contains(pathLower))
            {
                reportFiles.Remove(pathLower);
                isChanged = true;
            }

            if (isChanged)
                Save();
        }

        public static bool ContainsDbs(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            Load();

            return dbsFiles.Contains(path.ToLowerInvariant());
        }

        public static bool ContainsFile(string path)
        {
            if (path == null)
                return false;

            Load();

            var pathLower = path.ToLowerInvariant();

            if (dbsFiles.Contains(pathLower))
                return true;

            if (reportFiles.Contains(pathLower))
                return true;

            return false;
        }
        #endregion
    }
}