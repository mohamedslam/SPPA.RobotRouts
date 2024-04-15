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

using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Mvc;
using Stimulsoft.Report.Web;
using System;
using System.Collections;
using System.Globalization;

#if NETSTANDARD
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web.Mvc;
#endif

namespace Stimulsoft.Report.Angular
{
    public class StiAngularViewer :
#if NETSTANDARD
    StiNetCoreViewer
#else
    StiMvcViewer
#endif
    {
        #region Methods
        private static Hashtable GetJsOptions(StiAngularViewerOptions options)
        {
            var jsOptions = new Hashtable();
            jsOptions["theme"] = options.Theme;
            jsOptions["cultureName"] = StiLocalization.CultureName;
            jsOptions["localization"] = options.Localization;
            jsOptions["shortProductVersion"] = StiVersionHelper.AssemblyVersion;
            jsOptions["productVersion"] = StiVersionHelper.ProductVersion.Trim();
            jsOptions["licenseUserName"] = StiLicenseHelper.GetUserName();
            jsOptions["licenseIsValid"] = StiLicenseHelper.IsValidOnWeb();
            jsOptions["frameworkType"] =
#if NETSTANDARD
                ".NET Core";
#else
                "ASP.NET MVC";
#endif
            jsOptions["stimulsoftFontContent"] = StiReportResourceHelper.GetStimulsoftFontBase64Data();
            jsOptions["dashboardAssemblyLoaded"] = StiDashboardAssembly.IsAssemblyLoaded && StiDashboardExportAssembly.IsAssemblyLoaded && StiDashboardDrawingAssembly.IsAssemblyLoaded;
            jsOptions["alternateValid"] = StiLicenseHelper.CheckAnyLicense();
            jsOptions["buildDate"] = StiVersion.Created.ToString(CultureInfo.CreateSpecificCulture("en-US"));

            return jsOptions;
        }
        #endregion

        /// <summary>
        /// Get the initial data for Angular Viewer
        /// </summary>
        /// <param name="requestParams">Request parameters passed from the client side of the viewer.</param>
        /// <param name="options">The viewer options.</param>
        public static ActionResult ViewerDataResult(StiRequestParams requestParams, StiAngularViewerOptions options = null)
        {
            if (options == null)
            {
                options = new StiAngularViewerOptions();
            }

            requestParams.Theme = options.Theme.ToString();
            if (!string.IsNullOrEmpty(options.Localization))
                requestParams.Localization = options.Localization;

            var parameters = new Hashtable
            {
                ["action"] = StiAction.AngularViewerData,
                ["options"] = options,
                ["defaultSettings"] = StiExportsHelper.GetDefaultExportSettings(options.Exports.DefaultSettings),
                ["images"] = StiViewerResourcesHelper.GetImagesArray(requestParams, Math.Max(requestParams.GetDouble("stiweb_imagesScalingFactor"), 1)),
                ["styles"] = StiViewerResourcesHelper.GetStyles(requestParams),
                ["loc"] = StiCollectionsHelper.GetLocalizationItems(requestParams),
                ["encodingData"] = StiCollectionsHelper.GetEncodingDataItems(),
                ["dateRanges"] = StiCollectionsHelper.GetDateRangesItems(),
                ["months"] = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" },
                ["dayOfWeek"] = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" },
                ["paperSizes"] = StiCollectionsHelper.GetPaperSizes(),
                ["pdfSecurityCertificates"] = StiCollectionsHelper.GetPdfSecurityCertificatesItems(),
                ["jsOptions"] = GetJsOptions(options)
            };

            var viewerResult = StiWebActionResult.JsonResult(requestParams, parameters);
            return StiAngularActionResult.FromWebActionResult(viewerResult);
        }

        public StiAngularViewer(
#if NETSTANDARD
            IHtmlHelper htmlHelper,
#else
            HtmlHelper htmlHelper,
#endif
            string viewerId, StiAngularViewerOptions options) : base(htmlHelper, viewerId, options)
        {
        }
    }
}
