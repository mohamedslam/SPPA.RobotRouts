namespace Stimulsoft.System.Web.Caching
{
    //
    // Summary:
    //     Specifies the relative priority of items stored in the System.Web.Caching.Cache
    //     object.
    public enum CacheItemPriority
    {
        //
        // Summary:
        //     Cache items with this priority level are the most likely to be deleted from the
        //     cache as the server frees system memory.
        Low = 1,
        //
        // Summary:
        //     Cache items with this priority level are more likely to be deleted from the cache
        //     as the server frees system memory than items assigned a System.Web.Caching.CacheItemPriority.Normal
        //     priority.
        BelowNormal = 2,
        //
        // Summary:
        //     Cache items with this priority level are likely to be deleted from the cache
        //     as the server frees system memory only after those items with System.Web.Caching.CacheItemPriority.Low
        //     or System.Web.Caching.CacheItemPriority.BelowNormal priority. This is the default.
        Normal = 3,
        //
        // Summary:
        //     The default value for a cached item's priority is System.Web.Caching.CacheItemPriority.Normal.
        Default = 3,
        //
        // Summary:
        //     Cache items with this priority level are less likely to be deleted as the server
        //     frees system memory than those assigned a System.Web.Caching.CacheItemPriority.Normal
        //     priority.
        AboveNormal = 4,
        //
        // Summary:
        //     Cache items with this priority level are the least likely to be deleted from
        //     the cache as the server frees system memory.
        High = 5,
        //
        // Summary:
        //     The cache items with this priority level will not be automatically deleted from
        //     the cache as the server frees system memory. However, items with this priority
        //     level are removed along with other items according to the item's absolute or
        //     sliding expiration time.
        NotRemovable = 6
    }
}