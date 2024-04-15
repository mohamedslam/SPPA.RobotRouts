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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;

namespace Stimulsoft.Base.Wpf.RecentFiles
{
    public sealed class StiRecentFileStructure
    {
        public StiRecentFileStructure()
        {
            var desktop = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            var documents = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            var dowloads = new DirectoryInfo(GetPath("{374DE290-123F-4565-9164-39C4925E467B}"));

            RootLevel = new StiRecentFileStructureLevel
            {
                Name = "This PC",
                IsRoot = true,
            };

            var desktopItem = new StiRecentFileStructureItem
            {
                Name = desktop.Name,
                Path = desktop.FullName,
                IsFolder = true
            };
            var documentsItem = new StiRecentFileStructureItem
            {
                Name = documents.Name,
                Path = documents.FullName,
                IsFolder = true
            };
            var downloadsItem = new StiRecentFileStructureItem
            {
                Name = dowloads.Name,
                Path = dowloads.FullName,
                IsFolder = true
            };
            RootLevel.Folders = new List<StiRecentFileStructureItem>
            {
                desktopItem,
                documentsItem,
                downloadsItem,
            };

            var drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                if (drive.IsReady)
                {
                    RootLevel.Folders.Add(new StiRecentFileStructureItem
                    {
                        Name = drive.Name,
                        Path = drive.RootDirectory.FullName,
                        VolumeLabel = drive.VolumeLabel,
                        DriveType = drive.DriveType,
                        IsFolder = true
                    });
                }
            }

            Levels.Add(RootLevel);
            MoveTo(documentsItem, out var errorMessage);
        }

        #region DllImport
        [DllImport("Shell32.dll")]
        private static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr ppszPath);
        #endregion

        #region Properties
        public StiRecentFileStructureLevel RootLevel { get; }

        public StiRecentFileStructureLevel CurrentLevel => Levels[Levels.Count - 1];

        public List<StiRecentFileStructureLevel> Levels { get; private set; } = new List<StiRecentFileStructureLevel>();

        public bool AllowLevelUp => Levels.Count > 1;

        public StiRecentFileStructureSort Sort { get; set; } = StiRecentFileStructureSort.NameAsc;
        #endregion

        #region Methods
        [SecuritySafeCritical]
        internal static string GetPath(string guidFolder)
        {
            int result = SHGetKnownFolderPath(new Guid(guidFolder), (uint)0x00004000, new IntPtr(0), out IntPtr outPath);
            if (result >= 0)
            {
                string path = Marshal.PtrToStringUni(outPath);
                Marshal.FreeCoTaskMem(outPath);
                return path;
            }
            else
            {
                throw new ExternalException("Unable to retrieve the known folder path. It may not "
                    + "be available on this system.", result);
            }
        }

        public bool MoveTo(StiRecentFileStructureItem item, out string errorMessage)
        {
            try
            {
                var currentDir = new DirectoryInfo(item.Path);
                var newLevel = new StiRecentFileStructureLevel
                {
                    Name = currentDir.Name,
                    Path = currentDir.FullName,
                    Folders = new List<StiRecentFileStructureItem>(),
                    Files = new List<StiRecentFileStructureItem>()
                };

                var dirs = currentDir.GetDirectories();
                foreach (var dir in dirs)
                {
                    if (dir.Name.StartsWith("$"))
                        continue;

                    newLevel.Folders.Add(new StiRecentFileStructureItem
                    {
                        Name = dir.Name,
                        Path = dir.FullName,
                        IsFolder = true,
                        DateModified = dir.LastWriteTime
                    });
                }

                var files = currentDir.GetFiles().Where(f => f.Extension == ".mrt" || f.Extension == ".mrz" || f.Extension == ".mrx").ToArray();
                foreach (var file in files)
                {
                    newLevel.Files.Add(new StiRecentFileStructureItem
                    {
                        Name = file.Name,
                        Path = file.FullName,
                        IsFolder = false,
                        DateModified = file.LastWriteTime
                    });
                }

                Levels.Add(newLevel);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }

            errorMessage = null;
            return true;
        }

        public bool GotoFolder(string path, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                var currentDir = new DirectoryInfo(path);

                var newLevels = new List<StiRecentFileStructureLevel>
                {
                    RootLevel
                };

                while (true)
                {
                    var newLevel = new StiRecentFileStructureLevel
                    {
                        Name = currentDir.Name,
                        Path = currentDir.FullName,
                        Folders = new List<StiRecentFileStructureItem>(),
                        Files = new List<StiRecentFileStructureItem>(),
                    };

                    newLevels.Insert(1, newLevel);

                    if (currentDir.Parent == null)
                        break;

                    currentDir = currentDir.Parent;
                }

                var lastLevel = newLevels[newLevels.Count - 1];
                currentDir = new DirectoryInfo(lastLevel.Path);

                var dirs = currentDir.GetDirectories();
                foreach (var dir in dirs)
                {
                    if (dir.Name.StartsWith("$"))
                        continue;

                    lastLevel.Folders.Add(new StiRecentFileStructureItem
                    {
                        Name = dir.Name,
                        Path = dir.FullName,
                        IsFolder = true,
                        DateModified = dir.LastWriteTime
                    });
                }

                var files = currentDir.GetFiles().Where(f => f.Extension == ".mrt" || f.Extension == ".mrz" || f.Extension == ".mrx").ToArray();
                foreach (var file in files)
                {
                    lastLevel.Files.Add(new StiRecentFileStructureItem
                    {
                        Name = file.Name,
                        Path = file.FullName,
                        IsFolder = false,
                        DateModified = file.LastWriteTime
                    });
                }

                Levels = newLevels;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }

            return true;
        }

        public void MoveBack()
        {
            if (Levels.Count > 1)
            {
                Levels.RemoveAt(Levels.Count - 1);

                var level = CurrentLevel;
                if (level == RootLevel) return;

                var currentDir = new DirectoryInfo(level.Path);
                level.Files.Clear();
                level.Folders.Clear();

                var dirs = currentDir.GetDirectories();
                foreach (var dir in dirs)
                {
                    if (dir.Name.StartsWith("$"))
                        continue;

                    level.Folders.Add(new StiRecentFileStructureItem
                    {
                        Name = dir.Name,
                        Path = dir.FullName,
                        IsFolder = true,
                        DateModified = dir.LastWriteTime
                    });
                }

                var files = currentDir.GetFiles().Where(f => f.Extension == ".mrt" || f.Extension == ".mrz" || f.Extension == ".mrx").ToArray();
                foreach (var file in files)
                {
                    level.Files.Add(new StiRecentFileStructureItem
                    {
                        Name = file.Name,
                        Path = file.FullName,
                        IsFolder = false,
                        DateModified = file.LastWriteTime
                    });
                }
            }
        }

        public bool NewFolder(string name, out string errorMessage)
        {
            errorMessage = null;

            var level = CurrentLevel;
            if (level == RootLevel)
                return false;

            try
            {
                var folderName = Path.Combine(level.Path, name);
                var dir = Directory.CreateDirectory(folderName);

                level.Folders.Add(new StiRecentFileStructureItem
                {
                    Name = dir.Name,
                    Path = dir.FullName,
                    IsFolder = true,
                    DateModified = dir.LastWriteTime
                });
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }

            return true;
        }
        #endregion
    }
}