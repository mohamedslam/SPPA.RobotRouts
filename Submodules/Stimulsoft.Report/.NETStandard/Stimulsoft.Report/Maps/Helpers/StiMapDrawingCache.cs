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

using Stimulsoft.Base.Drawing;
using System.Collections.Generic;
using System.Drawing;

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Maps.Helpers
{
    public static class StiMapDrawingCache
    {
        #region class StiCacheItem
        private class StiCacheItem
        {
            public object Image { get; set; }

            public Size Size { get; set; }

            public string Latitude { get; set; }

            public bool IsDesigning { get; set; }

            public string Longitude { get; set; }
        }
        #endregion

        #region Fields.Static
        private static Dictionary<StiMap, StiCacheItem> lastImageCache = new Dictionary<StiMap, StiCacheItem>();
        #endregion

        #region Methods
        internal static void Clear()
        {
            lastImageCache.Clear();
        }

        internal static void RemoveImage(StiMap map)
        {
            lock (lastImageCache)
            {
                if (lastImageCache.ContainsKey(map))
                    lastImageCache.Remove(map);
            }
        }

        internal static object GetLastImage(StiMap map)
        {
            lock (lastImageCache)
            {
                if (!lastImageCache.ContainsKey(map))
                {
                    if (!map.IsDesigning && map.MapImage != null)
                    {
                        try
                        {
                            return StiImageConverter.StringToImage(map.MapImage);
                        }
                        catch { }                        
                    }
                    return null;
                }

                var item = lastImageCache[map];
                if (item.IsDesigning != map.IsDesigning ||
                    item.Latitude != map.Latitude ||
                    item.Longitude != map.Longitude || 
                    item.Size != map.ClientRectangle.Size.ToSize())
                {
                    lastImageCache.Remove(map);
                    return null;
                }

                return item.Image;
            }
        }

        internal static void StoreLastImage(StiMap map, object image)
        {
            lock (lastImageCache)
            {
                var item = new StiCacheItem
                {
                    Image = image,
                    Size = map.ClientRectangle.Size.ToSize(),
                    IsDesigning = map.IsDesigning,
                    Latitude = map.Latitude,
                    Longitude = map.Longitude
                };

                lastImageCache[map] = item;

                Bitmap bitmap = image as Bitmap;
                if (bitmap != null)
                {
                    try
                    {
                        map.MapImage = StiImageConverter.ImageToString(bitmap);
                    }catch
                    {

                    }                    
                }
            }
        }
        #endregion
    }
}