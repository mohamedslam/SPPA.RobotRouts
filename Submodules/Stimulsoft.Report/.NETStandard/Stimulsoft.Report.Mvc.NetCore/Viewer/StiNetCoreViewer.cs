#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{																	}
{	Copyright (C) 2003-2022 Stimulsoft     							}
{	ALL RIGHTS RESERVED												}
{																	}
{	The entire contents of this file is protected by U.S. and		}
{	International Copyright Laws. Unauthorized reproduction,		}
{	reverse-engineering, and distribution of all or any portion of	}
{	the code contained in this file is strictly prohibited and may	}
{	result in severe civil and criminal penalties and will be		}
{	prosecuted to the maximum extent possible under the law.		}
{																	}
{	RESTRICTIONS													}
{																	}
{	THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES			}
{	ARE CONFIDENTIAL AND PROPRIETARY								}
{	TRADE SECRETS OF Stimulsoft										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using Stimulsoft.System.Web.UI.WebControls;
using System.Linq;
using Stimulsoft.Report.Web;
using Stimulsoft.Base;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Stimulsoft.Report.Mvc
{
    public partial class StiNetCoreViewer : Panel
    {
        #region Fields

        private IHtmlHelper htmlHelper;
        private StiNetCoreViewerOptions options;

        #endregion

        #region Properties

        private static StiCacheHelper cacheHelper = null;
        /// <summary>
        /// Gets or sets an instance of the StiCacheHelper class that will be used for report caching on the server side.
        /// </summary>
        public static StiCacheHelper CacheHelper
        {
            get
            {
                if (cacheHelper == null) cacheHelper = new StiCacheHelper();
                return cacheHelper;
            }
            set
            {
                cacheHelper = value;
            }
        }

        private string clientGuid = null;
        internal string ClientGuid
        {
            get
            {
                if (clientGuid == null) clientGuid = StiGuidUtils.NewGuid();
                return clientGuid;
            }
            set
            {
                clientGuid = value;
            }
        }

        private bool reportDesignerMode = false;
        internal bool ReportDesignerMode
        {
            get
            {
                return reportDesignerMode;
            }
            set
            {
                reportDesignerMode = value;
            }
        }

        #endregion

        #region Internal

        /// <summary>
        /// Get the URL for the viewer requests.
        /// </summary>
        internal static string GetRequestUrl(IHtmlHelper htmlHelper, string template, string controller, bool useRelativeUrls, bool passQueryParameters, int portNumber)
        {
            var viewContext = htmlHelper.ViewContext;
            if (viewContext == null || viewContext.HttpContext == null) return null;
            var request = viewContext.HttpContext.Request;

            // Get base URL
            var uri = request.PathBase.ToUriComponent();
            if (!useRelativeUrls)
            {
                var host = portNumber < 0 || portNumber == 0 && request.Host.Port == null
                    ? new HostString(request.Host.Host)
                    : new HostString(request.Host.Host, portNumber > 0 ? portNumber : (int)request.Host.Port);
                uri = string.Concat(request.Scheme, "://", host.ToUriComponent(), uri);
            }
            if (!uri.EndsWith("/")) uri = string.Concat(uri, "/");

            if (!string.IsNullOrEmpty(template))
            {
                var isRazorPage = viewContext.ActionDescriptor is PageActionDescriptor;
                if (isRazorPage)
                    template = template.Replace("{handler}", "{action}");

                if (template.IndexOf("{controller}") >= 0)
                {
                    if (string.IsNullOrEmpty(controller)) controller = viewContext.RouteData.Values["controller"] as string;
                    if (!string.IsNullOrEmpty(controller)) template = template.Replace("{controller}", controller);
                }

                if (template.StartsWith("/")) template = template.Substring(1);
                while (template.EndsWith("/")) template = template.Substring(0, template.Length - 1);
                if (template.IndexOf("{action}") < 0) template = string.Concat(template, "/{action}");
                
                uri = template.StartsWith("http://") || template.StartsWith("https://") ? template : string.Concat(uri, template);
            }
            else
            {
                try
                {
                    var mvcRoute = viewContext.RouteData.Routers.OfType<RouteBase>().FirstOrDefault();
                    var endpointRoute = viewContext.HttpContext.Features.Get<IEndpointFeature>()?.Endpoint as RouteEndpoint;
                    if (endpointRoute != null)
                    {
                        foreach (var segment in endpointRoute.RoutePattern.PathSegments)
                        {
                            foreach (var part in segment.Parts)
                            {
                                if (part is RoutePatternParameterPart)
                                {
                                    var parameterPart = part as RoutePatternParameterPart;
                                    if (parameterPart.Name == "controller" && !string.IsNullOrEmpty(controller)) uri = string.Concat(uri, controller);
                                    else if (parameterPart.Name == "action") uri = string.Concat(uri, "{action}");
                                    else if (parameterPart.IsParameter) uri = string.Concat(uri, viewContext.RouteData.Values[parameterPart.Name]);
                                }
                                else if (part is RoutePatternLiteralPart)
                                {
                                    var literalPart = part as RoutePatternLiteralPart;
                                    var actionValue = uri.IndexOf("{action}") < 0 ? viewContext.RouteData.Values["action"] : null;
                                    uri = string.Concat(uri, actionValue is string && (string)actionValue == literalPart.Content ? "{action}" : literalPart.Content);
                                }
                            }

                            if (!uri.EndsWith("/")) uri = string.Concat(uri, "/");
                        }
                    }
                    else if (mvcRoute != null)
                    {
                        foreach (var segment in mvcRoute.ParsedTemplate.Segments)
                        {
                            foreach (var part in segment.Parts)
                            {
                                if (part.Name == "controller" && !string.IsNullOrEmpty(controller)) uri = string.Concat(uri, controller);
                                else if (part.Name == "action") uri = string.Concat(uri, "{action}");
                                else if (part.IsParameter) uri = string.Concat(uri, viewContext.RouteData.Values[part.Name]);
                                else uri = string.Concat(uri, part.Text);
                            }

                            if (!uri.EndsWith("/")) uri = string.Concat(uri, "/");
                        }
                    }
                    else
                    {
                        foreach (var value in viewContext.RouteData.Values)
                        {
                            if (value.Key == "controller" && !string.IsNullOrEmpty(controller)) uri = string.Concat(uri, controller);
                            else if (value.Key == "action") uri = string.Concat(uri, "{action}");
                            else uri = string.Concat(uri, value.Value);

                            if (!uri.EndsWith("/")) uri = string.Concat(uri, "/");
                        }
                    }
                }
                catch
                {   
                }
            }

            if (uri.IndexOf("{action}") < 0)
            {
                var isRazorPage = viewContext.ActionDescriptor is PageActionDescriptor;
                uri = isRazorPage ? string.Concat(uri, "?handler={action}") : string.Concat(uri, "{action}");
            }

            while (uri.EndsWith("/"))
                uri = uri.Substring(0, uri.Length - 1);

            if (passQueryParameters && request.QueryString.Value?.Length > 0)
            {
                var queryString = uri.IndexOf("?") >= 0 ? request.QueryString.Value.Replace("?", "&") : request.QueryString.Value;
                return string.Concat(uri, queryString);
            }

            return uri;
        }
        #endregion

        public StiNetCoreViewer(IHtmlHelper htmlHelper, string viewerId, StiNetCoreViewerOptions options)
        {
            StiOptions.Configuration.IsWeb = true;

            this.htmlHelper = htmlHelper;
            this.ID = viewerId;
            this.options = options;
        }
    }
}
