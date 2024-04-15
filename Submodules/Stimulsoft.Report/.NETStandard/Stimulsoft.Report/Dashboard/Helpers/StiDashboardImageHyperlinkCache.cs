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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using Stimulsoft.Base;
using Stimulsoft.Report.Helpers;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Stimulsoft.Report.Dashboard.Helpers
{
    internal static class StiDashboardImageHyperlinkCache
	{
	    #region Fields
	    private static object lockObject = new object();
	    private static Dictionary<string, byte[]> cache = new Dictionary<string, byte[]>();
	    #endregion

        #region Methods
        internal static byte[] Get(string hyperlink, IStiApp app)
	    {
            lock (cache)
            {
                var key = GetCacheKey(hyperlink, app);
                var bytes = cache.ContainsKey(key) ? GetFromCache(hyperlink, app) : null;
                if (bytes == null)
                {
                    bytes = StiHyperlinkProcessor.TryGetBytes(app as StiReport, hyperlink, true, true);

                    if (bytes == null)
                        bytes = new byte[0];

                    AddToCache(hyperlink, bytes, app);

                    return bytes;
                }

                if (bytes.Length == 0)
                    return null;

                else
                    return bytes;
            }
        }
        #endregion

	    #region Methods.Cache
	    private static string GetCacheKey(string hyperlink, IStiApp app)
	    {
	        app?.SetKey(StiKeyHelper.GetOrGeneratedKey(app.GetKey()));
	        var appKey = app?.GetKey() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(hyperlink))
                return $"{appKey}.0";

            else
                return $"{appKey}.{hyperlink}";
	    }

	    private static byte[] GetFromCache(string hyperlink, IStiApp app)
	    {
	        lock (lockObject)
	        {
	            var key = GetCacheKey(hyperlink, app);
	            return cache.ContainsKey(key) ? cache[key] : null;
	        }
	    }

        internal static void AddToCache(string hyperlink, byte[] bytes, IStiApp app)
	    {
	        lock (lockObject)
	        {
	            var key = GetCacheKey(hyperlink, app);

	            if (bytes == null)
	                bytes = new byte[0];

	            cache[key] = bytes;
	        }
	    }

	    internal static void Clean(string appKey)
	    {
	        lock (cache)
	        {
	            if (appKey == null)
	            {
	                cache.Clear();
	            }
	            else
	            {
	                cache.Keys
	                    .Where(k => k.StartsWith(appKey))
	                    .ToList()
	                    .ForEach(k => cache.Remove(k));
	            }
	        }
	    }
	    #endregion
    }
}