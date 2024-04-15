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
using Stimulsoft.Base;

namespace Stimulsoft.Report
{
    internal static class StiFileImageCacheV2
    {
        #region Properties.Static
        private static List<string> cachedDirectories;
        internal static List<string> CachedDirectories
        {
            get
            {
                return cachedDirectories ?? (cachedDirectories = new List<string>());
            }
        }
        #endregion

        #region Methods
        internal static void CreateNewCacheIfNeeded(StiReport report)
        {
            if (!string.IsNullOrEmpty(report.ImageCachePath)) return;

            var temp = string.IsNullOrEmpty(StiOptions.Engine.ImageCache.CachePath) ?
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) :
                StiOptions.Engine.ImageCache.CachePath;

            temp = Path.Combine(temp, "StimulsoftImageCache", StiGuidUtils.NewGuid());

            if (!Directory.Exists(temp))
                Directory.CreateDirectory(temp);

            CachedDirectories.Add(temp);

            report.ImageCachePath = temp;
        }


        internal static void DeleteCache(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }


        internal static void ClearCache()
        {
            if (cachedDirectories == null) return;

            foreach (var path in CachedDirectories)
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
            }
        }


        internal static string GetImageCacheName(string cache, string cacheImageGuid)
        {
            return Path.Combine(cache, $"{cacheImageGuid}.ich");
        }


        internal static void SaveImage(StiReport report, string guid, byte[] bytes)
        {
            CreateNewCacheIfNeeded(report);

            var path = GetImageCacheName(report.ImageCachePath, guid);
            StiFileUtils.ProcessReadOnly(path);
            File.WriteAllBytes(path, bytes);
        }

        internal static byte[] LoadImage(StiReport report, string guid)
        {
            if (guid == null || report == null || report.ImageCachePath == null) return null;

            var path = GetImageCacheName(report.ImageCachePath, guid);
            return File.Exists(path) ? File.ReadAllBytes(path) : null;
        }
        #endregion
    }
}
