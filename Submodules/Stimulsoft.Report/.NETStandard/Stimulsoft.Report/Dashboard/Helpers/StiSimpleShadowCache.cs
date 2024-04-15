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
using Stimulsoft.Base.Drawing;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Drawing;

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Dashboard.Helpers
{
    internal static class StiSimpleShadowCache
	{
	    #region Fields
	    private static object lockObject = new object();
	    private static Dictionary<string, Bitmap> cache = new Dictionary<string, Bitmap>();
	    #endregion

	    #region Methods.Cache
	    private static string GetCacheKey(IStiApp app, StiSimpleShadow shadow, StiCornerRadius cornerRadius,
			StiBorderSides borderSide, bool isCircle, Size size, float scale)
	    {
			unchecked
			{
				app?.SetKey(StiKeyHelper.GetOrGeneratedKey(app.GetKey()));
				var appKey = app?.GetKey() ?? string.Empty;

				var hashCode = 0;

				if (shadow != null)
					hashCode = (hashCode * 397) ^ shadow.GetUniqueCode();

				if (cornerRadius != null)
					hashCode = (hashCode * 397) ^ cornerRadius.GetUniqueCode();

				hashCode = (hashCode * 397) ^ borderSide.GetHashCode();
				hashCode = (hashCode * 397) ^ isCircle.GetHashCode();
				hashCode = (hashCode * 397) ^ size.Width.GetHashCode();
				hashCode = (hashCode * 397) ^ size.Height.GetHashCode();
				hashCode = (hashCode * 397) ^ scale.GetHashCode();				

				return $"{appKey}.{hashCode}";
			}
	    }

	    public static Bitmap GetFromCache(IStiApp app, StiSimpleShadow shadow, StiCornerRadius cornerRadius,
			StiBorderSides borderSide, bool isCircle, Size size, float scale)
	    {
	        lock (lockObject)
	        {
	            var key = GetCacheKey(app, shadow, cornerRadius, borderSide, isCircle, size, scale);
	            return cache.ContainsKey(key) ? cache[key] : null;
	        }
	    }

        public static void AddToCache(IStiApp app, StiSimpleShadow shadow, Bitmap shadowBitmap, StiCornerRadius cornerRadius,
			StiBorderSides borderSide, bool isCircle, Size size, float scale)
	    {
			if (shadow == null || shadowBitmap == null) return;

	        lock (lockObject)
	        {
	            var key = GetCacheKey(app, shadow, cornerRadius, borderSide, isCircle, size, scale);
	            cache[key] = shadowBitmap;
	        }
	    }

	    public static void Clean(string appKey)
	    {
	        lock (cache)
	        {
				var keys = appKey == null 
					? cache.Keys
					: cache.Keys.Where(k => k.StartsWith(appKey));

                keys.ToList().ForEach(k =>
				{
					var bitmap = cache[k];
					cache.Remove(k);
					bitmap.Dispose();
				});
			}
	    }
	    #endregion
    }
}