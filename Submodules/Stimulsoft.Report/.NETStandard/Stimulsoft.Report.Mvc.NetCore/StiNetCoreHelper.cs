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

using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stimulsoft.System.Web.UI;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Html;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Stimulsoft.Report.Mvc
{
    public static class StiNetCoreHelper
    {
        #region Static Helpers
        public static string MapPath(Controller controller, string path)
        {
            var httpContext = new System.Web.HttpContext(controller.HttpContext);
            return httpContext.Server.MapPath(path);
        }

        public static string MapPath(PageModel page, string path)
        {
            var httpContext = new System.Web.HttpContext(page.HttpContext);
            return httpContext.Server.MapPath(path);
        }

        public static string MapWebRootPath(Controller controller, string path)
        {
            var httpContext = new System.Web.HttpContext(controller.HttpContext);
            return httpContext.Server.MapRootPath(path);
        }

        public static string MapWebRootPath(PageModel page, string path)
        {
            var httpContext = new System.Web.HttpContext(page.HttpContext);
            return httpContext.Server.MapRootPath(path);
        }

        internal static string GetFrameworkVersion()
        {
            var frameworkName = Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;
            if (!string.IsNullOrEmpty(frameworkName) && frameworkName.IndexOf("=v") > 0)
            {
                var version = frameworkName.Substring(frameworkName.IndexOf("=v") + 2);
                return $".NET{(frameworkName.StartsWith(".NETCoreApp") && (version.StartsWith("2") || version.StartsWith("3")) ? " Core" : "")} {version}";
            }

            return ".NET Core";
        }
        #endregion

        #region Controls

        #region StiNetCoreViewer

        public static HtmlString StiNetCoreViewer(this IHtmlHelper htmlHelper, StiNetCoreViewerOptions options)
        {
            return htmlHelper.StiNetCoreViewer("NetCoreViewer", options);
        }

        public static HtmlString StiNetCoreViewer(this IHtmlHelper htmlHelper, string ID, StiNetCoreViewerOptions options)
        {
            if (options == null)
                throw new Exception("Failed to initialize the StiNetCoreViewer component. Please define the StiNetCoreViewerOptions to work correctly with the viewer.");

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var viewer = new StiNetCoreViewer(htmlHelper, ID, options);
            var writer = new StringWriter();
            var htmlWriter = new HtmlTextWriter(writer, string.Empty);
            htmlWriter.NewLine = string.Empty;
            viewer.RenderControl(htmlWriter);
            htmlWriter.Dispose();

            return new HtmlString(writer.ToString());
        }

        #endregion

        #region StiNetCoreDesigner

        public static HtmlString StiNetCoreDesigner(this IHtmlHelper htmlHelper, StiNetCoreDesignerOptions options)
        {
            return htmlHelper.StiNetCoreDesigner("NetCoreDesigner", options);
        }

        public static HtmlString StiNetCoreDesigner(this IHtmlHelper htmlHelper, string ID, StiNetCoreDesignerOptions options)
        {
            if (options == null)
                throw new Exception("Failed to initialize the StiNetCoreDesigner component. Please define the StiNetCoreDesignerOptions to work correctly with the designer.");

            if (string.IsNullOrEmpty(options.Actions.DesignerEvent))
                throw new Exception("Failed to initialize the StiNetCoreDesigner component. Please define the DesignerEvent action to work correctly with the designer.");

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var designer = new StiNetCoreDesigner(htmlHelper, ID, options);
            var writer = new StringWriter();
            var htmlWriter = new HtmlTextWriter(writer, string.Empty);
            htmlWriter.NewLine = string.Empty;
            designer.RenderControl(htmlWriter);
            htmlWriter.Dispose();

            return new HtmlString(writer.ToString());
        }

        #endregion

        #endregion
    }
}
