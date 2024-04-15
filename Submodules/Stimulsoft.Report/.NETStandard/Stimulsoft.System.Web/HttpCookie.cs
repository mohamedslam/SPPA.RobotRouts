using System;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Web
{
    public sealed class HttpCookie
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public DateTime Expires { get; set; }

        public HttpCookie()
        {
        }
        
        public HttpCookie(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
