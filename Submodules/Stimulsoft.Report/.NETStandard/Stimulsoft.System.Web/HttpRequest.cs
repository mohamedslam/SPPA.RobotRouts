using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace Stimulsoft.System.Web
{
    public sealed class HttpRequest
    {
        /// <summary>
        /// Gets the Microsoft.AspNetCore.Http.HttpRequest object for this request.
        /// </summary>
        public Microsoft.AspNetCore.Http.HttpRequest CoreRequest { get; set; }

        /// <summary>
        /// Gets or set the RequestBody Stream.
        /// </summary>
        public Stream Body
        {
            get
            {
                return CoreRequest.Body;
            }
            set
            {
                CoreRequest.Body = value;
            }
        }

        /// <summary>
        /// Gets the query value collection parsed from Request.QueryString.
        /// </summary>
        public Microsoft.AspNetCore.Http.IQueryCollection Query
        {
            get
            {
                return CoreRequest.Query;
            }
            set
            {
                CoreRequest.Query = value;
            }
        }

        /// <summary>
        /// Gets the collection of HTTP query string variables.
        /// </summary>
        public NameValueCollection QueryString
        {
            get
            {
                var collection = new NameValueCollection();
                foreach (string key in CoreRequest.Query.Keys)
                {
                    collection[key] = CoreRequest.Query[key].ToString();
                }

                return collection;
            }
        }

        /// <summary>
        ///  Gets a combined collection of System.Web.HttpRequest.QueryString and System.Web.HttpRequest.Body items.
        /// </summary>
        private NameValueCollection paramss = null;
        public NameValueCollection Params
        {
            get
            {
                if (paramss == null)
                {
                    paramss = this.QueryString;
                    paramss.Add(this.Form);
                }
                
                return paramss;
            }
        }

        /// <summary>
        /// Gets a collection of form variables.
        /// </summary>
        private NameValueCollection form = null;
        public NameValueCollection Form
        {
            get
            {
                if (form == null)
                {
                    form = new NameValueCollection();
                    if (this.Method == "POST")
                    {
                        try
                        {
                            foreach (var parameter in CoreRequest.Form)
                            {
                                form[parameter.Key] = parameter.Value;
                            }
                        }
                        catch
                        {
                        }

                        // Try to get parameters from request body
                        if (form.Count == 0 && this.ContentLength > 0)
                        {
                            var body = this.Body;
                            if (body.CanSeek)
                            {
                                body.Position = 0;
                            }
                            else
                            {
                                body = new MemoryStream();
                                this.Body.CopyToAsync(body).Wait();
                                body.Position = 0;
                            }

                            var reader = new StreamReader(body);
                            var data = reader.ReadToEnd();
                            var parameters = data.Split('&');
                            foreach (var parameter in parameters)
                            {
                                if (parameter.IndexOf("=") > 0)
                                {
                                    var key = parameter.Substring(0, parameter.IndexOf("="));
                                    var value = parameter.Substring(parameter.IndexOf("=") + 1);
                                    form[key] = Uri.UnescapeDataString(value);
                                }
                            }

                            if (!this.Body.CanSeek)
                            {
                                body.Position = 0;
                                this.Body = body;
                            }
                        }
                    }
                }
                
                return form;
            }
        }

        /// <summary>
        /// Gets or set the HTTP method.
        /// </summary>
        public string Method
        {
            get
            {
                return CoreRequest.Method;
            }
            set
            {
                CoreRequest.Method = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the Content-Length header
        /// </summary>
        public long? ContentLength
        {
            get
            {
                return CoreRequest.ContentLength;
            }
            set
            {
                CoreRequest.ContentLength = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the Content-Type header.
        /// </summary>
        public string ContentType
        {
            get
            {
                return CoreRequest.ContentType;
            }
            set
            {
                CoreRequest.ContentType = value;
            }
        }

        /// <summary>
        /// Gets a collection of cookies sent by the client.
        /// </summary>
        public HttpCookieCollection Cookies { get; }

        /// <summary>
        /// Gets information about the URL of the current request.
        /// </summary>
        public Uri Url
        {
            get
            {
                return new Uri(string.Format("{0}://{1}{2}{3}{4}", CoreRequest.Scheme, CoreRequest.Host, CoreRequest.PathBase, CoreRequest.Path, CoreRequest.QueryString));
            }
        }

        public HttpRequest(Microsoft.AspNetCore.Http.HttpRequest request)
        {
            this.CoreRequest = request;
            this.Cookies = new HttpCookieCollection(request.Cookies);
        }
    }
}
