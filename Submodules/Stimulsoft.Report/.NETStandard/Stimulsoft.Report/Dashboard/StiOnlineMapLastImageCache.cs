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
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Dashboard
{
    public static class StiOnlineMapLastImageCache
    {
        #region Fields.Static
        private static Dictionary<string, Image> cache = new Dictionary<string, Image>();
        #endregion

        #region Methods
        private static string GetKey(IStiOnlineMapElement element)
        {
            var appKey = StiAppKey.GetOrGeneratedKey(element) ?? string.Empty;
            var elementKey = element?.GetKey() ?? string.Empty;

            return $"{appKey}.{elementKey}";
        }
        
        public static Image GetLastImage(IStiOnlineMapElement element)
        {
            lock (cache)
            {
                var key = GetKey(element);
                return cache.ContainsKey(key) ? cache[key] : null;
            }
        }

        public static bool ExistsLastImage(IStiOnlineMapElement element)
        {
            lock (cache)
            {
                var key = GetKey(element);
                return cache.ContainsKey(key);
            }
        }

        public static void StoreLastImage(IStiOnlineMapElement element, Image image)
        {
            lock (cache)
            {
                var key = GetKey(element);
                cache[key] = image;
            }
        }

        public static void Clean(string reportKey)
        {
            lock (cache)
            {
                if (reportKey == null)
                {
                    cache.Clear();
                }
                else
                {
                    cache.Keys
                        .Where(k => k.StartsWith(reportKey))
                        .ToList()
                        .ForEach(k => cache.Remove(k));
                }
            }
        }
        #endregion
    }
}