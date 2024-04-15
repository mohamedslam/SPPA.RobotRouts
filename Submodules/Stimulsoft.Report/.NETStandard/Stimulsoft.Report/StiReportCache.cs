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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Stimulsoft.Base;

namespace Stimulsoft.Report
{
	internal class StiReportCache
    {
        #region Methods
        internal static string CreateNewCache()
		{
			var temp = string.IsNullOrEmpty(StiOptions.Engine.ReportCache.CachePath) 
			    ? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) 
			    : StiOptions.Engine.ReportCache.CachePath;

			temp = Path.Combine(temp, "Stimulsoft", "ReportsCache");
			
			if (!Directory.Exists(temp))
			    Directory.CreateDirectory(temp);

			var cache = StiGuidUtils.NewGuid();
            temp = Path.Combine(temp, cache);

			if (!Directory.Exists(temp))
			    Directory.CreateDirectory(temp);

		    if (CachedDirectories == null)
		        CachedDirectories = new List<string>();

			CachedDirectories.Add(temp);

			return temp;
		}

        internal static void DeleteCache(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }

        internal static void ClearCache()
        {
            if (CachedDirectories == null) return;

            CachedDirectories.ForEach(d =>
            {
                if (Directory.Exists(d))
                    Directory.Delete(d, true);
            });
        }

        internal static string GetPageCacheName(string cache, string cachePageGuid)
        {
            return Path.Combine(cache, $"{cachePageGuid}.mch");
        }
        #endregion

        #region Properties.Static
        internal static List<string> CachedDirectories { get; set; }
        #endregion

        private StiReportCache()
		{			
		}
	}
}
