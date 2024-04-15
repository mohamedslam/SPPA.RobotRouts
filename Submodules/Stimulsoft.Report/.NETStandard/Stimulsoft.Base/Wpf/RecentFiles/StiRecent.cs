#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Stimulsoft.Base.Wpf.RecentFiles
{
    public sealed class StiRecent
    {
        static StiRecent()
        {
            try
            {
                Connector = StiActivator.CreateObject(Type.GetType("Stimulsoft.Accounts.Wpf.Items.RecentFiles.StiRecentFilesCloudConnector, Stimulsoft.Accounts.Wpf")) as IStiRecent22CloudConnector;
            }
            catch { }

            var type = Type.GetType("Stimulsoft.Report.StiSettings, Stimulsoft.Report");
            SettingsClearKey = type.GetMethod("ClearKey", new Type[] { typeof(string), typeof(string) });
            SettingsSet = type.GetMethod("Set", new Type[] { typeof(string), typeof(string), typeof(object) });
            SettingsGetStr = type.GetMethod("GetStr", new Type[] { typeof(string), typeof(string) });
            SettingsSave = type.GetMethod("Save", new Type[] { });

            Current = new StiRecent();
            Current.Load();
        }

        private StiRecent()
        {
            //StiJumpListHelper.AddRecentFile(file);
        }

        #region Fields
        private const int MaxFilesCount = 10;
        private const int MaxFolderCount = 10;
        private static MethodInfo SettingsClearKey;
        private static MethodInfo SettingsSet;
        private static MethodInfo SettingsGetStr;
        private static MethodInfo SettingsSave;
        #endregion

        #region class SaveContainer
        private sealed class SaveContainer
        {
            public string SaveAsDefaultFolder { get; set; }

            public string LocalDefaultFolder { get; set; }
        }
        #endregion

        #region Properties
        public static StiRecent Current { get; }

        internal static IStiRecent22CloudConnector Connector { get; }

        public List<StiRecentFile> RecentFiles { get; set; } = new List<StiRecentFile>();

        public List<StiRecentFolder> RecentFolders { get; set; } = new List<StiRecentFolder>();

        public string SaveAsDefaultFolder { get; set; }

        public string LocalDefaultFolder { get; set; }
        #endregion

        #region Events
        public event EventHandler RecentFoldersChanged;
        internal void InvokeRecentFoldersChanged()
        {
            this.RecentFoldersChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler RecentFilesChanged;
        internal void InvokeRecentFilesChanged()
        {
            this.RecentFilesChanged?.Invoke(this, EventArgs.Empty);
        }

        public event StiRecentCloudFolderChangedEventHandler CloudFolderChanged;
        internal void InvokeCloudFolderChanged(string folderKey)
        {
            this.CloudFolderChanged?.Invoke(this, new StiRecentCloudFolderChangedEventArgs(folderKey));
        }
        #endregion

        #region Methods
        public void AddCloudFile(string name, string itemKey, string folderKey, string path, bool isPinned, bool addFirst, bool autoSave)
        {
            if (string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(itemKey) ||
                string.IsNullOrEmpty(path))
            {
                return;
            }

            #region Check File
            var findFile = this.RecentFiles.FirstOrDefault(x => x.Path.ToLowerInvariant() == path.ToLowerInvariant());
            if (findFile == null)
            {
                if (this.RecentFiles.Count >= MaxFilesCount)
                    this.RecentFiles.RemoveAt(this.RecentFiles.Count - 1);

                if (addFirst)
                {
                    this.RecentFiles.Insert(0, new StiRecentFile
                    {
                        Name = name,
                        CloudKey = itemKey,
                        Path = path,
                        DateModified = DateTime.Now,
                        IsPinned = isPinned,
                        IsCloud = true
                    });
                }
                else
                {
                    this.RecentFiles.Add(new StiRecentFile
                    {
                        Name = name,
                        CloudKey = itemKey,
                        Path = path,
                        DateModified = DateTime.Now,
                        IsPinned = isPinned,
                        IsCloud = true
                    });
                }
            }
            else
            {
                findFile.IsPinned = isPinned;
                findFile.DateModified = DateTime.Now;

                if (addFirst)
                {
                    this.RecentFiles.Remove(findFile);
                    this.RecentFiles.Insert(0, findFile);
                }
            }
            #endregion

            #region Check Folder
            var index = path.LastIndexOf("\\");
            if (index != -1)
            {
                var folderPath = path.Substring(0, index);
                index = folderPath.LastIndexOf("\\");
                string folderName = "";

                bool success = true;
                if (index == -1)
                {
                    index = folderPath.IndexOf("] ");
                    if (index == -1)
                    {
                        success = false;
                    }
                    else
                    {
                        folderName = folderPath.Substring(index + 2);
                    }
                }
                else
                {
                    folderName = folderPath.Substring(index + 1);
                }

                if (success)
                {
                    var findFolder = this.RecentFolders.FirstOrDefault(x => x.Path.ToLowerInvariant() == folderPath.ToLowerInvariant());
                    if (findFolder == null)
                    {
                        if (this.RecentFolders.Count >= MaxFolderCount)
                            this.RecentFolders.RemoveAt(this.RecentFolders.Count - 1);

                        this.RecentFolders.Insert(0, new StiRecentFolder
                        {
                            Name = folderName,
                            CloudKey = folderKey,
                            Path = folderPath,
                            DateModified = DateTime.Now,
                            IsCloud = true
                        });
                    }
                    else
                    {
                        findFolder.DateModified = DateTime.Now;

                        if (addFirst)
                        {
                            this.RecentFolders.Remove(findFolder);
                            this.RecentFolders.Insert(0, findFolder);
                        }
                    }
                }
            }
            #endregion

            if (autoSave)
                Save();

            InvokeRecentFilesChanged();
            InvokeRecentFoldersChanged();
        }

        public void AddCloudFolder(string name, string itemKey, string path, bool isPinned, bool addFirst, bool autoSave)
        {
            if (string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(itemKey) ||
                string.IsNullOrEmpty(path))
            {
                return;
            }

            #region Check Folder
            string folderName;
            var index = path.LastIndexOf("\\");
            if (index != -1)
            {
                folderName = path.Substring(index + 1);
            }
            else
            {
                folderName = path;
            }

            var findFolder = this.RecentFolders.FirstOrDefault(x => x.Path.ToLowerInvariant() == path.ToLowerInvariant());
            if (findFolder == null)
            {
                if (this.RecentFolders.Count >= MaxFolderCount)
                    this.RecentFolders.RemoveAt(this.RecentFolders.Count - 1);

                if (addFirst)
                {
                    this.RecentFolders.Insert(0, new StiRecentFolder
                    {
                        Name = folderName,
                        CloudKey = itemKey,
                        Path = path,
                        DateModified = DateTime.Now,
                        IsPinned = isPinned,
                        IsCloud = true
                    });
                }
                else
                {
                    this.RecentFolders.Add(new StiRecentFolder
                    {
                        Name = folderName,
                        CloudKey = itemKey,
                        Path = path,
                        DateModified = DateTime.Now,
                        IsPinned = isPinned,
                        IsCloud = true
                    });
                }
            }
            else
            {
                findFolder.IsPinned = isPinned;
                findFolder.DateModified = DateTime.Now;

                if (addFirst)
                {
                    this.RecentFolders.Remove(findFolder);
                    this.RecentFolders.Insert(0, findFolder);
                }
            }
            #endregion

            if (autoSave)
                Save();

            InvokeRecentFoldersChanged();
        }

        public void AddLocalFile(string filePath, bool isPinned, bool addFirst, bool autoSave)
        {
            if (string.IsNullOrEmpty(filePath))
                return;
            if (filePath.Length > 4)
            {
                string ext = filePath.Substring(filePath.Length - 4, 4);
                switch(ext)
                {
                    case ".mrt":
                    case ".mrz":
                    case ".mrx":
                        break;

                    default:
                        return;
                }
            }

            #region Check File
            var findFile = this.RecentFiles.FirstOrDefault(x => x.Path.ToLowerInvariant() == filePath.ToLowerInvariant());
            if (findFile == null)
            {
                if (this.RecentFiles.Count >= MaxFilesCount)
                    this.RecentFiles.RemoveAt(this.RecentFiles.Count - 1);

                var reportType = StiReportContentHelper.GetReportType(filePath);
                var fi = new FileInfo(filePath);
                if (addFirst)
                {
                    this.RecentFiles.Insert(0, new StiRecentFile
                    {
                        Name = fi.Name,
                        Path = fi.FullName,
                        IsPinned = isPinned,
                        DateModified = DateTime.Now,
                        ContentType = reportType
                    });
                }
                else
                {
                    this.RecentFiles.Add(new StiRecentFile
                    {
                        Name = fi.Name,
                        Path = fi.FullName,
                        IsPinned = isPinned,
                        DateModified = DateTime.Now,
                        ContentType = reportType
                    });
                }
            }
            else
            {
                if (isPinned)
                    findFile.IsPinned = isPinned;
                findFile.DateModified = DateTime.Now;

                if (addFirst)
                {
                    this.RecentFiles.Remove(findFile);
                    this.RecentFiles.Insert(0, findFile);
                }
            }
            #endregion

            #region Check Folder
            var folderPath = Path.GetDirectoryName(filePath);

            var findFolder = this.RecentFolders.FirstOrDefault(x => x.Path.ToLowerInvariant() == folderPath.ToLowerInvariant());
            if (findFolder == null)
            {
                if (this.RecentFolders.Count >= MaxFolderCount)
                    this.RecentFolders.RemoveAt(this.RecentFolders.Count - 1);

                var di = new DirectoryInfo(folderPath);
                this.RecentFolders.Insert(0, new StiRecentFolder
                {
                    Name = di.Name,
                    Path = folderPath,
                    DateModified = DateTime.Now
                });
            }
            else
            {
                findFolder.DateModified = DateTime.Now;
            }
            #endregion

            if (autoSave)
                Save();

            InvokeRecentFilesChanged();
            InvokeRecentFoldersChanged();
        }

        public void AddLocalFile(string filePath, bool isPinned, DateTime dateModified, bool addFirst, bool autoSave)
        {
            if (string.IsNullOrEmpty(filePath))
                return;
            if (filePath.Length > 4)
            {
                string ext = filePath.Substring(filePath.Length - 4, 4);
                switch (ext)
                {
                    case ".mrt":
                    case ".mrz":
                    case ".mrx":
                        break;

                    default:
                        return;
                }
            }

            #region Check File
            var findFile = this.RecentFiles.FirstOrDefault(x => x.Path.ToLowerInvariant() == filePath.ToLowerInvariant());
            if (findFile == null)
            {
                if (this.RecentFiles.Count >= MaxFilesCount)
                    this.RecentFiles.RemoveAt(this.RecentFiles.Count - 1);

                var reportType = StiReportContentHelper.GetReportType(filePath);
                var fi = new FileInfo(filePath);

                if (addFirst)
                {
                    this.RecentFiles.Insert(0, new StiRecentFile
                    {
                        Name = fi.Name,
                        Path = fi.FullName,
                        IsPinned = isPinned,
                        DateModified = dateModified,
                        ContentType = reportType
                    });
                }
                else
                {
                    this.RecentFiles.Add(new StiRecentFile
                    {
                        Name = fi.Name,
                        Path = fi.FullName,
                        IsPinned = isPinned,
                        DateModified = dateModified,
                        ContentType = reportType
                    });
                }
            }
            else
            {
                if (isPinned)
                    findFile.IsPinned = isPinned;
                findFile.DateModified = dateModified;

                if (addFirst)
                {
                    this.RecentFiles.Remove(findFile);
                    this.RecentFiles.Insert(0, findFile);
                }
            }
            #endregion

            #region Check Folder
            var folderPath = Path.GetDirectoryName(filePath);

            var findFolder = this.RecentFolders.FirstOrDefault(x => x.Path.ToLowerInvariant() == folderPath.ToLowerInvariant());
            if (findFolder == null)
            {
                if (this.RecentFolders.Count >= MaxFolderCount)
                    this.RecentFolders.RemoveAt(this.RecentFolders.Count - 1);

                var di = new DirectoryInfo(folderPath);
                this.RecentFolders.Insert(0, new StiRecentFolder
                {
                    Name = di.Name,
                    Path = folderPath,
                    DateModified = dateModified
                });
            }
            else
            {
                findFolder.DateModified = dateModified;

                if (addFirst)
                {
                    this.RecentFolders.Remove(findFolder);
                    this.RecentFolders.Insert(0, findFolder);
                }
            }
            #endregion

            if (autoSave)
                Save();

            InvokeRecentFilesChanged();
            InvokeRecentFoldersChanged();
        }

        public void AddLocalFolder(string folderPath, bool isPinned, bool addFirst, bool autoSave)
        {
            if (string.IsNullOrEmpty(folderPath))
                return;

            #region Check Folder
            var findFolder = this.RecentFolders.FirstOrDefault(x => x.Path.ToLowerInvariant() == folderPath.ToLowerInvariant());
            if (findFolder == null)
            {
                if (this.RecentFolders.Count >= MaxFolderCount)
                    this.RecentFolders.RemoveAt(this.RecentFolders.Count - 1);

                var di = new DirectoryInfo(folderPath);

                if (addFirst)
                {
                    this.RecentFolders.Insert(0, new StiRecentFolder
                    {
                        Name = di.Name,
                        Path = folderPath,
                        IsPinned = isPinned,
                        DateModified = DateTime.Now
                    });
                }
                else
                {
                    this.RecentFolders.Add(new StiRecentFolder
                    {
                        Name = di.Name,
                        Path = folderPath,
                        IsPinned = isPinned,
                        DateModified = DateTime.Now
                    });
                }
            }
            else
            {
                if (isPinned)
                    findFolder.IsPinned = isPinned;

                findFolder.DateModified = DateTime.Now;

                if (addFirst)
                {
                    this.RecentFolders.Remove(findFolder);
                    this.RecentFolders.Insert(0, findFolder);
                }
            }
            #endregion

            if (autoSave)
                Save();

            InvokeRecentFoldersChanged();
        }

        public void RemoveFolder(StiRecentFolder folder)
        {
            if (RecentFolders.Contains(folder))
                RecentFolders.Remove(folder);

            Save();
            this.RecentFoldersChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveFile(StiRecentFile file, bool runEvent = true)
        {
            if (RecentFiles.Contains(file))
                RecentFiles.Remove(file);

            Save();

            if (runEvent)
                this.RecentFilesChanged?.Invoke(this, EventArgs.Empty);
        }

        public void MoveToTop(StiRecentFile file)
        {
            if (!RecentFiles.Contains(file)) return;

            if (RecentFiles.Count > 1)
            {
                RecentFiles.Remove(file);
                RecentFiles.Insert(0, file);

                InvokeRecentFilesChanged();
            }
        }
        #endregion

        #region Methods.Save/Load
        private static string GetSettingsPath()
        {
            var path = $"Stimulsoft{Path.DirectorySeparatorChar}Stimulsoft.Designer.RecentFiles.json";
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
                    path = $"Stimulsoft{Path.DirectorySeparatorChar}Stimulsoft.Designer.RecentFiles.json";
                    folder = "Stimulsoft";

                    if (StiBaseOptions.FullTrust && !Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                }
                catch
                {
                    path = "Stimulsoft.Designer.RecentFiles.json";
                }
            }

            return path;
        }

        private void Load()
        {
            #region Load Settings
            try
            {
                var path = GetSettingsPath();
                if (File.Exists(path))
                {
                    var text = File.ReadAllText(path);
                    var container = JsonConvert.DeserializeObject<SaveContainer>(text);

                    this.SaveAsDefaultFolder = container.SaveAsDefaultFolder;
                    this.LocalDefaultFolder = container.LocalDefaultFolder;

                    //this.RecentFiles = container.RecentFiles;
                    //this.RecentFolders = container.RecentFolders;
                }
            }
            catch
            {
            }
            #endregion

            #region Load Files
            for (int index = 0; index < 10; index++)
            {
                var path = SettingsGetStr.Invoke(null, new object[] { "StiDesigner", "RecentFile" + index }) as string;
                if (!string.IsNullOrEmpty(path))
                {
                    try
                    {
                        if (path.StartsWith("cloud\\"))
                        {
                            if (StiAccountSettings.CurrentAccount != null && StiAccountSettings.CurrentAccount.User != null)
                            {
                                var str1 = path.Substring(6);
                                int index2 = str1.IndexOf("\\");
                                if (index2 == -1) continue;

                                var itemKey = str1.Substring(0, index2);
                                var path1 = str1.Substring(index2 + 1);

                                var itemName = path1;
                                if (path1.Contains("\\"))
                                {
                                    int index3 = itemName.LastIndexOf("\\");
                                    itemName = itemName.Substring(index3 + 1);
                                }

                                AddCloudFile(itemName, itemKey, null, $"[{StiAccountSettings.CurrentAccount.User.UserName}] {path1}", false, false, false);
                            }
                        }
                        else
                        {
                            var fi = new FileInfo(path);
                            if (!fi.Exists) continue;

                            AddLocalFile(fi.FullName, false, fi.LastWriteTime, false, false);
                        }
                    }
                    catch { }
                }
            }
            #endregion

            #region Cleaning 
            var files = this.RecentFiles.ToArray();
            foreach (var file in files)
            {
                if (file.IsCloud) continue;

                try
                {
                    if (!File.Exists(file.Path))
                        this.RecentFiles.Remove(file);
                }
                catch { }
            }

            var folders = this.RecentFolders.ToArray();
            foreach (var folder in folders)
            {
                if (folder.IsCloud) continue;

                try
                {
                    if (!Directory.Exists(folder.Path))
                        this.RecentFolders.Remove(folder);
                }
                catch { }
            }
            #endregion
        }

        public void Save()
        {
            #region Save Settings
            try
            {
                var container = new SaveContainer
                {
                    SaveAsDefaultFolder = SaveAsDefaultFolder,
                    LocalDefaultFolder = LocalDefaultFolder,
                };

                var path = GetSettingsPath();
                var dir = Path.GetDirectoryName(path);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var json = JsonConvert.SerializeObject(container, Formatting.Indented,
                    new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });

                StiFileUtils.ProcessReadOnly(path);
                File.WriteAllText(path, json);
            }
            catch
            {

            }
            #endregion

            #region Save Files
            var files = RecentFiles.ToArray();

            for (int index = 0; index < 10; index++)
            {
                SettingsClearKey.Invoke(null, new object[] { "StiDesigner", "RecentFile" + index });
            }

            Debug.WriteLine("=======RECENT FILES=======");
            int indexRecent = 0;
            foreach (var file in files)
            {
                string path;
                if (file.IsCloud)
                {
                    Debug.WriteLine(file + " CLOUD");
                    int index = file.Path.IndexOf("] ");
                    if (index == -1)
                        continue;

                    path = file.Path.Substring(index + 2);
                    path = $"cloud\\{file.CloudKey}\\{path}";
                }
                else
                {
                    Debug.WriteLine(file);
                    path = file.Path;
                }

                SettingsSet.Invoke(null, new object[] { "StiDesigner", "RecentFile" + indexRecent, path });
                indexRecent++;

                if (indexRecent >= 10)
                    break;
            }
            Debug.WriteLine("=====================");

            SettingsSave.Invoke(null, null);
            #endregion
        }
        #endregion
    }
}