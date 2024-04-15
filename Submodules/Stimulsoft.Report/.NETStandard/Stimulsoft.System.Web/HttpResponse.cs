using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Stimulsoft.System.Web
{
    public sealed class HttpResponse : HttpResponseBase
    {
        public HttpResponse(Microsoft.AspNetCore.Http.HttpResponse response) : base(response)
        {
        }
    }
}
