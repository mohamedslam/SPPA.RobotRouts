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
using Stimulsoft.System.Web.UI.WebControls;
using Stimulsoft.System.Web.UI;
using System.Collections;
using Stimulsoft.Base.Localization;
using System.Text;
using Stimulsoft.Report.Web;
using System.Web;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;
using Stimulsoft.Base.Json.Serialization;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Base;
using System.Globalization;
using Microsoft.AspNetCore.Antiforgery;

namespace Stimulsoft.Report.Mvc
{
    public partial class StiNetCoreViewer : Panel
    {
        protected override void Render(HtmlTextWriter writer)
        {
            CreateChildControls();

            base.Render(writer);
        }

        private string RenderJsonParameters()
        {
            #region Routes

            var routes = new Hashtable();
            foreach (string key in htmlHelper.ViewContext.RouteData.Values.Keys) routes[key] = htmlHelper.ViewContext.RouteData.Values[key];
            string jsRoutes = JsonConvert.SerializeObject(routes, Formatting.None);

            #endregion

            #region Form Values

            var formValues = new Hashtable();
            if (options.Server.PassFormValues && htmlHelper.ViewContext.HttpContext.Request.Method == "POST")
            {
                foreach (string key in htmlHelper.ViewContext.HttpContext.Request.Form.Keys)
                {
                    formValues[key] = htmlHelper.ViewContext.HttpContext.Request.Form[key].ToString();
                }
            }

            #endregion

            if (ReportDesignerMode)
                options.Server.AllowAutoUpdateCache = false;

            var jsOptions = new Hashtable();
            jsOptions["viewerId"] = this.ID;
            jsOptions["theme"] = options.Theme;
            jsOptions["clientGuid"] = this.ClientGuid;
            jsOptions["requestAbsoluteUrl"] = GetRequestUrl(htmlHelper, options.Server.RouteTemplate, options.Server.Controller, false, true, options.Server.PortNumber);
            jsOptions["requestUrl"] = GetRequestUrl(htmlHelper, options.Server.RouteTemplate, options.Server.Controller, options.Server.UseRelativeUrls, true, options.Server.PortNumber);
            jsOptions["requestResourcesUrl"] = GetRequestUrl(htmlHelper, options.Server.RouteTemplate, options.Server.Controller, options.Server.UseRelativeUrls, options.Server.PassQueryParametersForResources, options.Server.PortNumber);
            jsOptions["routes"] = routes;
            jsOptions["formValues"] = formValues;
            jsOptions["cultureName"] = StiLocalization.CultureName;
            jsOptions["localization"] = options.Localization;
            jsOptions["shortProductVersion"] = StiVersionHelper.AssemblyVersion;
            jsOptions["productVersion"] = StiVersionHelper.ProductVersion.Trim();
            jsOptions["heightType"] = this.Height.Type.ToString();
            jsOptions["reportDesignerMode"] = this.ReportDesignerMode;
            jsOptions["frameworkType"] = StiNetCoreHelper.GetFrameworkVersion();
            jsOptions["stimulsoftFontContent"] = this.ReportDesignerMode ? null : StiReportResourceHelper.GetStimulsoftFontBase64Data();
            jsOptions["dashboardAssemblyLoaded"] = StiDashboardAssembly.IsAssemblyLoaded && StiDashboardExportAssembly.IsAssemblyLoaded && StiDashboardDrawingAssembly.IsAssemblyLoaded;
            jsOptions["alternateValid"] = StiLicenseHelper.CheckAnyLicense();
            jsOptions["licenseUserName"] = StiLicenseHelper.GetUserName();
            jsOptions["licenseIsValid"] = StiLicenseHelper.IsValidOnWeb();
            jsOptions["buildDate"] = StiVersion.Created.ToString(CultureInfo.CreateSpecificCulture("en-US"));
            jsOptions["listSeparator"] = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            jsOptions["customOpenTypeFonts"] = StiFontsHelper.GetCustomOpenTypeFontItems(options.Server.AllowLoadingCustomFontsToClientSide);
            jsOptions["fontNames"] = StiFontsHelper.GetFontNames();

            // Settings: Antiforgery Token
            if (options.Server.AllowAntiforgeryToken)
            {
                try
                {
                    var antiforgeryService = ((IAntiforgery)htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(IAntiforgery)));
                    if (antiforgeryService != null)
                    {
                        var tokens = antiforgeryService.GetAndStoreTokens(htmlHelper.ViewContext.HttpContext);
                        if (!string.IsNullOrEmpty(tokens.RequestToken))
                            jsOptions["requestToken"] = tokens.RequestToken;
                    }
                }
                catch
                {
                }
            }

            jsOptions["actions"] = options.Actions;
            jsOptions["server"] = options.Server;
            jsOptions["appearance"] = options.Appearance;
            jsOptions["toolbar"] = options.Toolbar;
            jsOptions["exports"] = options.Exports;
            jsOptions["email"] = options.Email;

            string jsonOptions = JsonConvert.SerializeObject(jsOptions, Formatting.None,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = { new StringEnumConverter() }
                });

            string jsonDefaultExportSettings = JsonConvert.SerializeObject(
                StiExportsHelper.GetDefaultExportSettings(options.Exports.DefaultSettings),
                Formatting.None, new StringEnumConverter());

            if (ReportDesignerMode)
            {
                var scriptUrl = GetScriptsUrl();
                var jsonScriptsUrl = JsonConvert.SerializeObject(scriptUrl);
                return $"{{options:{jsonOptions},defaultExportSettings:{jsonDefaultExportSettings},scriptsUrl:{jsonScriptsUrl}}}";
            }

            return $"{{options:{jsonOptions},defaultExportSettings:{jsonDefaultExportSettings}}}";
        }

        private string GetScriptsUrl()
        {
            string localizationBase64 = string.IsNullOrEmpty(options.Localization)
               ? null : Convert.ToBase64String(Encoding.UTF8.GetBytes(StiEncryption.Encrypt(options.Localization, StiRequestParamsHelper.EncryptingKey)));
            string requestUrl = GetRequestUrl(htmlHelper, options.Server.RouteTemplate, options.Server.Controller, options.Server.UseRelativeUrls, options.Server.PassQueryParametersForResources, options.Server.PortNumber);

            string scriptUrl = requestUrl.Replace("{action}", options.Actions.ViewerEvent) + (requestUrl.IndexOf("?") > 0 ? "&" : "?");
            scriptUrl += "stiweb_component=Viewer&stiweb_action=Resource&stiweb_data=scripts&stiweb_theme=" + options.Theme.ToString();
            if (!string.IsNullOrEmpty(localizationBase64)) scriptUrl += "&stiweb_loc=" + HttpUtility.UrlEncode(localizationBase64);
            scriptUrl += "&stiweb_cachemode=" + (options.Server.UseCacheForResources
                ? options.Server.CacheMode == StiServerCacheMode.ObjectSession || options.Server.CacheMode == StiServerCacheMode.StringSession
                    ? "session"
                    : "cache"
                : "none");
            scriptUrl += "&stiweb_version=" + StiVersionHelper.AssemblyVersion;
            
            return scriptUrl;
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            this.Width = options.Width;
            this.Height = options.Height.IsEmpty ? (options.Appearance.ScrollbarsMode ? Unit.Pixel(650) : Unit.Percentage(100)) : options.Height;
            this.BackColor = options.Appearance.BackgroundColor;

            #region Render Control

            Panel mainPanel = new Panel();
            mainPanel.CssClass = "stiJsViewerMainPanel";
            mainPanel.ID = this.ID + "_JsViewerMainPanel";
            this.Controls.Add(mainPanel);

            string jsParameters = RenderJsonParameters();
            if (ReportDesignerMode)
            {
                StiJavaScript scriptInit = new StiJavaScript();
                scriptInit.Text = string.Format("var js{0}Parameters = {1};", this.ID, jsParameters);
                mainPanel.Controls.Add(scriptInit);
            }
            else
            {
                StiJavaScript scriptEngine = new StiJavaScript();
                scriptEngine.ScriptUrl = GetScriptsUrl();
                mainPanel.Controls.Add(scriptEngine);

                StiJavaScript scriptInit = new StiJavaScript();
                scriptInit.Text = string.Format("var js{0} = new StiJsViewer({1});", this.ID, jsParameters);

                mainPanel.Controls.Add(scriptInit);
            }

            #endregion
        }
    }
}
