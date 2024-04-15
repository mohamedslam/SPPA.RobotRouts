using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;

namespace Stimulsoft.System.Web
{
    public sealed class HttpCachePolicy
    {
        private HttpResponseBase response { get; }

        public void SetExpires(DateTime date)
        {
            if (DateTime.Now >= date) response.AppendHeader("Expires", "0");
            else response.AppendHeader("Expires", date.ToUniversalTime().ToString("R", DateTimeFormatInfo.InvariantInfo));
        }

        public void SetCacheability(HttpCacheability cacheability)
        {
            switch (cacheability)
            {
                case HttpCacheability.NoCache:
                case HttpCacheability.ServerAndNoCache:
                    response.AppendHeader("Cache-Control", "no-cache");
                    break;

                case HttpCacheability.Private:
                case HttpCacheability.ServerAndPrivate:
                    response.AppendHeader("Cache-Control", "private");
                    break;

                default:
                    response.AppendHeader("Cache-Control", "public");
                    break;
            }
        }

        public HttpCachePolicy(HttpResponseBase response)
        {
            this.response = response;
        }
    }
}
