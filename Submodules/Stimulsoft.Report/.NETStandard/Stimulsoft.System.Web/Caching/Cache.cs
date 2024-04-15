using Microsoft.Extensions.Caching.Memory;
using System;

namespace Stimulsoft.System.Web.Caching
{
    public class Cache
    {
        #region Fields
        public static readonly DateTime NoAbsoluteExpiration = DateTime.MaxValue;
        public static readonly TimeSpan NoSlidingExpiration = TimeSpan.Zero;
        private Microsoft.AspNetCore.Http.HttpContext httpContext;
        private MemoryCache cache = null;
        #endregion

        private static Microsoft.Extensions.Caching.Memory.CacheItemPriority GetCacheItemPriority(CacheItemPriority priority)
        {
            switch (priority)
            {
                case CacheItemPriority.Low:
                    return Microsoft.Extensions.Caching.Memory.CacheItemPriority.Low;

                case CacheItemPriority.High:
                    return Microsoft.Extensions.Caching.Memory.CacheItemPriority.High;

                case CacheItemPriority.NotRemovable:
                    return Microsoft.Extensions.Caching.Memory.CacheItemPriority.NeverRemove;

                default:
                    return Microsoft.Extensions.Caching.Memory.CacheItemPriority.Normal;
            }
        }

        public object this[string key]
        {
            get
            {
                return Get(key);
            }

            set
            {
                Insert(key, value);
            }
        }

        public object Get(string key)
        {
            if (cache != null) return cache.Get(key);
            return null;
        }

        public object Add(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback)
        {
            if (cache != null)
            {
                var options = new MemoryCacheEntryOptions();
                options.SetPriority(GetCacheItemPriority(priority));
                if (absoluteExpiration == Cache.NoAbsoluteExpiration) options.SetSlidingExpiration(slidingExpiration);
                else if (slidingExpiration == Cache.NoSlidingExpiration) options.SetAbsoluteExpiration(absoluteExpiration);

                return cache.Set(key, value, options);
            }
            return null;
        }

        public void Insert(string key, object value)
        {
            Add(key, value, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
        }

        public object Remove(string key)
        {
            if (cache != null) cache.Remove(key);
            return null;
        }

        public Cache(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            this.httpContext = httpContext;
            this.cache = httpContext.RequestServices.GetService(typeof(IMemoryCache)) as MemoryCache;
        }

        public bool HasCache()
        {
            return cache != null;
        }
    }
}
