using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Stimulsoft.System.Web
{
    public sealed class HttpServerUtility
    {
        private Microsoft.AspNetCore.Http.HttpContext httpContext;

        private static string MapPath(string rootPath, string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return rootPath;
            while (relativePath.StartsWith("~") || relativePath.StartsWith("/") || relativePath.StartsWith("\\")) relativePath = relativePath.Substring(1);
            return Path.Combine(rootPath, relativePath);
        }

        /// <summary>
        /// Returns the physical file path in the application content folder that corresponds to the specified virtual path on the Web server.
        /// </summary>
        public string MapPath(string path)
        {
            var rootPath = ((IHostingEnvironment)httpContext.RequestServices.GetService(typeof(IHostingEnvironment))).ContentRootPath;
            return MapPath(rootPath, path);
        }

        /// <summary>
        /// Returns the physical file path in the application wwwroot folder that corresponds to the specified virtual path on the Web server.
        /// </summary>
        public string MapRootPath(string path)
        {
            var rootPath = ((IHostingEnvironment)httpContext.RequestServices.GetService(typeof(IHostingEnvironment))).WebRootPath;
            return MapPath(rootPath, path);
        }

        public HttpServerUtility(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            this.httpContext = httpContext;
        }
    }
}
