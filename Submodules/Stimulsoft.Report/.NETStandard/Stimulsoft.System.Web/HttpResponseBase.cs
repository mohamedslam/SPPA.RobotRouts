using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Stimulsoft.System.Web
{
    public class HttpResponseBase
    {
        /// <summary>
        /// Gets the Microsoft.AspNetCore.Http.HttpResponse object for this request.
        /// </summary>
        public Microsoft.AspNetCore.Http.HttpResponse CoreResponse { get; set; }

        public bool Buffer { get; set; }

        public bool SuppressContent { get; set; }

        /// <summary>
        /// Gets the caching policy (such as expiration time, privacy settings, and vary clauses) of a Web page.
        /// </summary>
        public HttpCachePolicy Cache { get; }

        /// <summary>
        /// Gets the response cookie collection.
        /// </summary>
        public HttpCookieCollection Cookies { get; }

        /// <summary>
        /// Gets or sets the HTTP status code of the output returned to the client.
        /// </summary>
        public int StatusCode
        {
            get
            {
                return CoreResponse.StatusCode;
            }
            set
            {
                CoreResponse.StatusCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the HTTP MIME type of the output stream.
        /// </summary>
        public string ContentType
        {
            get
            {
                return CoreResponse.ContentType;
            }
            set
            {
                CoreResponse.ContentType = value;
            }
        }

        /// <summary>
        /// Gets or sets the HTTP character set of the output stream.
        /// </summary>
        public Encoding ContentEncoding
        {
            get
            {
                return Encoding.UTF8;
            }
            set
            {
                AppendHeader("Content-Type", string.Format("{0}; charset={1}", ContentType, value.WebName));
            }
        }

        /// <summary>
        /// Adds an HTTP header to the output stream.
        /// </summary>
        public void AddHeader(string name, string value)
        {
            AppendHeader(name, value);
        }

        /// <summary>
        /// Adds an HTTP header to the output stream.
        /// </summary>
        public void AppendHeader(string name, string value)
        {
            value = Regex.Replace(value, @"[^\u0000-\u007F]+", "_");
            Microsoft.AspNetCore.Http.HeaderDictionaryExtensions.Append(CoreResponse.Headers, name, value);
        }

        /// <summary>
        /// Writes a string of binary characters to the HTTP output stream.
        /// </summary>
        public void BinaryWrite(byte[] buffer)
        {
            if (buffer != null)
                CoreResponse.Body.WriteAsync(buffer, 0, buffer.Length).Wait();
        }

        /// <summary>
        /// Clears all content output from the buffer stream.
        /// </summary>
        public void ClearContent()
        {
            CoreResponse.Body = new MemoryStream();
        }

        /// <summary>
        /// Clears all headers from the buffer stream.
        /// </summary>
        public void ClearHeaders()
        {
            CoreResponse.Headers.Clear();
        }

        public void End()
        {
        }

        public void Flush()
        {
        }

        public HttpResponseBase(Microsoft.AspNetCore.Http.HttpResponse response)
        {
            this.CoreResponse = response;
            this.Cache = new HttpCachePolicy(this);
        }
    }
}
