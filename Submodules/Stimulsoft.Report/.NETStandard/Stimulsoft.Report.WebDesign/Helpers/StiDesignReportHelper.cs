#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
{	                         										}
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

using Stimulsoft.Base;
using Stimulsoft.Base.Licenses;
using Stimulsoft.Base.Plans;
using Stimulsoft.Report.Components;
using System;
using System.Collections;
using System.Globalization;
using System.Threading;
using System.Linq;

#if NETSTANDARD
using Stimulsoft.System.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiDesignReportHelper
    {
        private StiReport report = null;
        
        public Hashtable GetReportJsObject()
        {            
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

            Hashtable reportObject = new Hashtable();
            reportObject["zoom"] = StiReportEdit.DoubleToStr(report != null && report.Info != null ? report.Info.Zoom : 1d);
            reportObject["gridSize"] = StiReportEdit.DoubleToStr(report != null && report.Unit != null ? report.Unit.ConvertToHInches(report.Info.GridSize) : 1d);
            reportObject["dictionary"] = StiDictionaryHelper.GetDictionaryTree(report);
            reportObject["stylesCollection"] = StiStylesHelper.GetStyles(report);
            reportObject["pages"] = GetPages();
            reportObject["properties"] = StiReportEdit.GetReportProperties(report);
            reportObject["info"] = GetReportInfo();
            reportObject["isJsonReport"] = report.IsJsonReport;
            reportObject["containsDashboard"] = report.ContainsDashboard;
            reportObject["containsForm"] = report.ContainsForm;

            Thread.CurrentThread.CurrentCulture = currentCulture;

            return reportObject;
        }
        
        public ArrayList GetPages()
        {
            ArrayList pages = new ArrayList();
            for (int pageIndex = 0; pageIndex < report.Pages.Count; pageIndex++) {
                Hashtable page = GetPage(pageIndex);
                if (page != null) pages.Add(page);
            }
            return pages;
        }

        public Hashtable GetPage(int pageIndex)
        {
            StiPage page = report.Pages[pageIndex];
            page.Correct(true);

            Hashtable pageObject = new Hashtable();
            pageObject["name"] = page.Name;
            pageObject["pageIndex"] = pageIndex.ToString();
            pageObject["properties"] = StiReportEdit.GetAllProperties(page);
            if (((Hashtable)pageObject["properties"]).Count == 0) return null;
            pageObject["components"] = GetComponents(page);

            #region LicenseKey
#if CLOUD
            var isValidWeb = StiCloudPlan.IsReportsAvailable(report.ReportGuid);
            var isValidDbsWeb = StiCloudPlan.IsDashboardsAvailable(report.ReportGuid);
#elif SERVER
            var isValidWeb = !StiVersionX.IsSvr;
            var isValidDbsWeb = !StiVersionX.IsSvr;
#else
            var isValidKey = false;
            var licenseKey = StiLicenseKeyValidator.GetLicenseKey();
            if (licenseKey != null)
            {
                try
                {
                    using (var rsa = new RSACryptoServiceProvider(512))
                    using (var sha = new SHA1CryptoServiceProvider())
                    {
                        rsa.FromXmlString("<RSAKeyValue><Modulus>iyWINuM1TmfC9bdSA3uVpBG6cAoOakVOt+juHTCw/gxz/wQ9YZ+Dd9vzlMTFde6HAWD9DC1IvshHeyJSp8p4H3qXUKSC8n4oIn4KbrcxyLTy17l8Qpi0E3M+CI9zQEPXA6Y1Tg+8GVtJNVziSmitzZddpMFVr+6q8CRi5sQTiTs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                        isValidKey = rsa.VerifyData(licenseKey.GetCheckBytes(), sha, licenseKey.GetSignatureBytes());
                    }
                }
                catch (Exception)
                {
                    isValidKey = false;
                }
            }

            var isValidWeb = isValidKey && StiLicenseKeyValidator.IsValid(StiProductIdent.Web, licenseKey) && typeof(StiLicense).AssemblyQualifiedName.Contains(StiPublicKeyToken.Key);
            var isValidDbsWeb = isValidKey && StiLicenseKeyValidator.IsValid(StiProductIdent.DbsWeb, licenseKey) && typeof(StiLicense).AssemblyQualifiedName.Contains(StiPublicKeyToken.Key);
#endif
            if (page.GetType().Name == "StiPage" && isValidWeb ||
                page.GetType().Name == "StiDashboard" && isValidDbsWeb)
                pageObject["valid"] = true;
#endregion

            return pageObject;
        }

        private ArrayList GetComponents(StiPage page)
        {
            ArrayList components = new ArrayList();

            foreach (StiComponent comp in page.GetComponents())
            {
                components.Add(GetComponent(comp));
            }

            return components;
        }

        private Hashtable GetComponent(StiComponent component)
        {
            Hashtable compObject = new Hashtable();

            compObject["name"] = component.Name;
            compObject["typeComponent"] = component.GetType().Name;
            compObject["componentRect"] = StiReportEdit.GetComponentRect(component);
            compObject["parentName"] = StiReportEdit.GetParentName(component);
            compObject["parentIndex"] = StiReportEdit.GetParentIndex(component).ToString();
            compObject["componentIndex"] = StiReportEdit.GetComponentIndex(component).ToString();
            compObject["childs"] = StiReportEdit.GetAllChildComponents(component);
            compObject["svgContent"] = StiReportEdit.GetSvgContent(component, 1);
            compObject["pageName"] = component.Page.Name;
            compObject["properties"] = StiReportEdit.GetAllProperties(component);

            return compObject;
        }

        private Hashtable GetReportInfo()
        {
            Hashtable properties = new Hashtable();
            string[] propNames = { "ShowHeaders", "ShowRulers", "ShowOrder", "RunDesignerAfterInsert", "UseLastFormat", "ShowDimensionLines", "GenerateLocalizedName", "AlignToGrid",
                "ShowGrid", "GridMode", "GridSizeInch", "GridSizeHundredthsOfInch", "GridSizeCentimetres", "GridSizeMillimeters", "GridSizePixels", "QuickInfoType", "QuickInfoOverlay",
                "AutoSaveInterval", "EnableAutoSaveMode" };
            foreach (string propName in propNames)
            {
                var value = StiReportEdit.GetPropertyValue(propName, report.Info);
                if (value != null) { properties[StiReportEdit.LowerFirstChar(propName)] = value; }
            }

            properties["startScreen"] = StiSettings.GetStr("StiDesigner", "StartScreen", "Welcome");

            return properties;
        }

        public static void ApplyParamsToReport(StiReport report, StiRequestParams requestParams)
        {
            if (report != null)
            {
                report.Info.ForceDesigningMode = true;
                report.Info.Zoom = 1;

                if (requestParams != null)
                {
                    if (report.Designer == null)
                        report.Designer = new StiWebDesignerBase();

                    if (requestParams.All["useAliases"] != null)
                        report.Designer.UseAliases = (bool)requestParams.All["useAliases"];
                }
            }
        }

        public static StiCloudPlanIdent ParseCloudPlanIdent(object ident)
        {
            int identNum = 0;
            int.TryParse(ident?.ToString(), out identNum);
            return (StiCloudPlanIdent)identNum;
        }

        internal static void CheckAndCorrectDuplicatePageNames(StiReport report)
        {
            if (report != null)
            {
                report.Pages.ToList().ForEach(p =>
                {
                    var duplicatePage = report.Pages.ToList().Find(p2 => p2.Name == p.Name && p2 != p);
                    if (duplicatePage != null)
                    {
                        var i = 1;
                        while (report.Pages[$"{duplicatePage.Name}_{i}"] != null)
                        {
                            i++;
                        }
                        duplicatePage.Name = $"{duplicatePage.Name}_{i}";
                    }
                });
            }
        }

        public StiDesignReportHelper(StiReport report)
        {
            this.report = report;
        }
    }
}