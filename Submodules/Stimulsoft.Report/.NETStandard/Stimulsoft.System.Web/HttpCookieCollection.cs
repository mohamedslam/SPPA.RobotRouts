using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Stimulsoft.System.Web
{
    public sealed class HttpCookieCollection
    {
        /// <summary>
        /// Gets the collection of Cookies for this request.
        /// </summary>
        public IRequestCookieCollection CoreCookies { get; }

        /// <summary>
        /// Returns the cookie with the specified name from the cookie collection.
        /// </summary>
        public HttpCookie Get(string name)
        {
            var result = CoreCookies.TryGetValue(name, out string value);
            if (result) return new HttpCookie(name, value);
            return null;
        }

        /// <summary>
        /// Adds the specified cookie to the cookie collection.
        /// </summary>
        public void Add(HttpCookie cookie)
        {
            throw new NotImplementedException();
        }

        public HttpCookie this[string name]
        {
            get
            {
                return Get(name);
            }

        }

        /// <summary>
        /// Gets a string array containing all the keys (cookie names) in the cookie collection.
        /// </summary>
        public string[] AllKeys
        {
            get
            {
                return CoreCookies.Keys.ToArray();
            }
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the System.Collections.Specialized.NameObjectCollectionBase instance.
        /// </summary>
        public int Count
        {
            get
            {
                return CoreCookies.Count;
            }
        }

        public HttpCookieCollection(IRequestCookieCollection cookies)
        {
            this.CoreCookies = cookies;
        }
    }
}
