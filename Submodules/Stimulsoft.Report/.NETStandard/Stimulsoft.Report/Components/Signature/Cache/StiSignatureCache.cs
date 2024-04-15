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
using System.IO;
using System.Linq;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components
{
    public static class StiSignatureCache
    {
        #region Fields
        private static List<StiSignatureCacheItem> cacheList;
        #endregion

        #region Methods
        public static void AddAndSave(StiSignatureCacheItem item)
        {
            if (cacheList == null) Load();

            var first = cacheList.FirstOrDefault(x => x.AlreadyExists(item));
            if (first != null) return;

            cacheList.Insert(0, item);
            item.Save();
        }

        public static void DeleteAndSave(StiSignatureCacheItem item)
        {
            if (cacheList == null) Load();

            if (cacheList.Contains(item))
                cacheList.Remove(item);

            item.Delete();
        }

        internal static string GetFolderPath()
        {
            var folder = $"Stimulsoft{Path.DirectorySeparatorChar}ReportSignatures";

            try
            {
                if (StiOptions.Engine.FullTrust)
                {
                    var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    if (folder.Length != 0)
                        folder = Path.Combine(path, folder);
                }

                if (StiOptions.Engine.FullTrust && !string.IsNullOrWhiteSpace(folder) && !Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
            }
            catch
            {
                try
                {
                    folder = $"Stimulsoft{Path.DirectorySeparatorChar}ReportSignatures";

                    if (StiOptions.Engine.FullTrust && !Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                }
                catch
                {
                    
                }
            }

            return folder;
        }

        public static List<StiSignatureCacheItem> Load()
        {
            if (cacheList == null)
            {
                cacheList = new List<StiSignatureCacheItem>();

                try
                {
                    var folder = GetFolderPath();
                    if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder))
                    {
                        var files = new DirectoryInfo(folder).GetFiles("*.json");
                        if (files != null && files.Length > 0)
                        {
                            foreach (var file in files)
                            {
                                try
                                {
                                    var text = File.ReadAllText(file.FullName);

                                    var item = JsonConvert.DeserializeObject<StiSignatureCacheItem>(text);
                                    cacheList.Add(item);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            cacheList = cacheList.OrderByDescending(x => x.Created).ToList();
            return cacheList;
        }
        #endregion
    }
}