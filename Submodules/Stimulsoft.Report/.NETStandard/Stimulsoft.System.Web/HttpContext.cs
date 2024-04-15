using Stimulsoft.System.Web.Caching;
using Stimulsoft.System.Web.SessionState;

namespace Stimulsoft.System.Web
{
    public sealed class HttpContext
    {
        public static HttpContext Current { get; } = null;

        public HttpApplication ApplicationInstance { get; set; } = new HttpApplication();

        /// <summary>
        /// Gets the Microsoft.AspNetCore.Http.HttpContext for the executing action.
        /// </summary>
        public Microsoft.AspNetCore.Http.HttpContext CoreHttpContext { get; }

        /// <summary>
        /// Gets the System.Web.HttpRequest object for the current HTTP request.
        /// </summary>
        public HttpRequest Request { get; }

        /// <summary>
        /// Gets the System.Web.HttpResponse object for the current HTTP response.
        /// </summary>
        public HttpResponse Response { get; }

        /// <summary>
        /// Gets the System.Web.Caching.Cache object for the current application domain.
        /// </summary>
        public Cache Cache { get; }

        /// <summary>
        /// Gets the System.Web.SessionState.HttpSessionState object for the current HTTP request.
        /// </summary>
        public HttpSessionState Session { get; }

        /// <summary>
        /// Gets the System.Web.HttpServerUtility object that provides methods used in processing Web requests.
        /// </summary>
        public HttpServerUtility Server { get; }

        public HttpContext(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            this.CoreHttpContext = httpContext;
            this.Request = new HttpRequest(httpContext.Request);
            this.Response = new HttpResponse(httpContext.Response);
            this.Cache = new Cache(httpContext);
            this.Session = new HttpSessionState(httpContext);
            this.Server = new HttpServerUtility(httpContext);
        }
    }
}
