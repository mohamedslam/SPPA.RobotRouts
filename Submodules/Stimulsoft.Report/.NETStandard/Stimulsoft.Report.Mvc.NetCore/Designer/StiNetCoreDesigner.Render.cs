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
using Stimulsoft.System.Web.UI;
using System.Collections;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Web;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.Components.TextFormats;
using System.Globalization;
using Stimulsoft.Report.Dashboard;
using Microsoft.AspNetCore.Antiforgery;
using Stimulsoft.Base.Blocks;

namespace Stimulsoft.Report.Mvc
{
    public partial class StiNetCoreDesigner : Panel
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
            /*foreach (string routeKey in htmlHelper.ViewContext?.RouteData.Values.Keys)
            {
                routes[routeKey] = htmlHelper.ViewContext.RouteData.Values[routeKey];
            }*/
            string jsRoutes = JsonConvert.SerializeObject(routes, Formatting.None);

            #endregion

            #region Form Values

            var formValues = new Hashtable();
            if (options.Server.PassFormValues && htmlHelper.ViewContext.HttpContext.Request.Method == "POST")
            {
                foreach (string formKey in htmlHelper.ViewContext.HttpContext.Request.Form.Keys)
                {
                    formValues[formKey] = htmlHelper.ViewContext.HttpContext.Request.Form[formKey];
                }
            }

            #endregion
            
            var parameters = new Hashtable();
            StiRequestParams requestParams = GetRequestParams(this.htmlHelper.ViewContext.HttpContext);
            if (requestParams.ComponentType != StiComponentType.Designer) requestParams = CreateRequestParams();
            
            if (requestParams.Action == StiAction.RunCommand && requestParams.Designer.Command == StiDesignerCommand.SetLocalization) this.options.Localization = requestParams.Localization;
            StiCollectionsHelper.LoadLocalizationFile(requestParams.HttpContext, this.options.LocalizationDirectory, this.options.Localization);

            // Settings: General
            parameters["mobileDesignerId"] = this.ID;
            parameters["viewerId"] = this.ID + "Viewer";
            parameters["cloudMode"] = false;
            parameters["demoMode"] = false;
            parameters["theme"] = options.Theme;
            parameters["cultureName"] = StiLocalization.CultureName;
            parameters["productVersion"] = StiVersionHelper.ProductVersion.Trim();
            parameters["routes"] = routes;
            parameters["formValues"] = formValues;
            parameters["frameworkType"] = StiNetCoreHelper.GetFrameworkVersion();
            parameters["dashboardAssemblyLoaded"] = StiDashboardAssembly.IsAssemblyLoaded && StiDashboardExportAssembly.IsAssemblyLoaded && StiDashboardDrawingAssembly.IsAssemblyLoaded;
            parameters["chartAssemblyLoaded"] = true;
            parameters["allowAutoUpdateCookies"] = options.Server.AllowAutoUpdateCookies;

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
                            parameters["requestToken"] = tokens.RequestToken;
                    }
                }
                catch
                {
                }
            }
            
            // Settings: Report cache
            parameters["clientGuid"] = this.ClientGuid;
            parameters["reportGuid"] = this.ClientGuid;
            parameters["cacheMode"] = options.Server.CacheMode;
            parameters["requestTimeout"] = options.Server.RequestTimeout;
            parameters["cacheTimeout"] = options.Server.CacheTimeout;
            parameters["cacheItemPriority"] = options.Server.CacheItemPriority;
            parameters["useRelativeUrls"] = options.Server.UseRelativeUrls;
            parameters["allowAutoUpdateCache"] = options.Server.AllowAutoUpdateCache;
            parameters["cacheHelper"] = CacheHelper != null;
            parameters["useCacheForResources"] = options.Server.UseCacheForResources;
            parameters["useCacheHelper"] = CacheHelper != null && CacheHelper.GetType() != typeof(StiCacheHelper);

            // Settings: URLs
            string resourcesUrl = GetResourcesUrl();
            parameters["useCompression"] = options.Server.UseCompression;
            parameters["passQueryParametersForResources"] = options.Server.PassQueryParametersForResources;
            parameters["cloudServerUrl"] = StiHyperlinkProcessor.ServerIdent;
            parameters["requestUrl"] = GetRequestUrl(htmlHelper, options.Server.RouteTemplate, options.Server.Controller, options.Server.UseRelativeUrls, true, options.Server.PortNumber);
            parameters["stylesUrl"] = string.IsNullOrEmpty(options.Appearance.CustomCss) ? resourcesUrl.Replace("&stiweb_data=", "&stiweb_theme=") + options.Theme.ToString() : options.Appearance.CustomCss;
            parameters["scriptsUrl"] = resourcesUrl;

            // Settings
            parameters["showAnimation"] = options.Appearance.ShowAnimation;
            parameters["defaultUnit"] = options.Appearance.DefaultUnit;
            parameters["focusingX"] = options.Behavior.FocusingX;
            parameters["focusingY"] = options.Behavior.FocusingY;
            parameters["helpLanguage"] = StiLocalization.CultureName == "ru" ? "ru" : "en";
            parameters["showSaveDialog"] = options.Behavior.ShowSaveDialog;
            parameters["fullScreenMode"] = options.Width == Unit.Empty && options.Height == Unit.Empty;
            parameters["haveExitDesignerEvent"] = !string.IsNullOrEmpty(options.Actions.Exit);
            parameters["haveSaveEvent"] = !string.IsNullOrEmpty(options.Actions.SaveReport);
            parameters["haveSaveAsEvent"] = !string.IsNullOrEmpty(options.Actions.SaveReportAs);
            parameters["showTooltips"] = options.Appearance.ShowTooltips;
            parameters["showTooltipsHelp"] = options.Appearance.ShowTooltipsHelp;
            parameters["showDialogHelp"] = options.Appearance.ShowDialogsHelp;
            parameters["showOpenDialog"] = options.Appearance.ShowOpenDialog;            
            parameters["interfaceType"] = options.Appearance.InterfaceType;
            parameters["undoMaxLevel"] = options.Behavior.UndoMaxLevel;
            parameters["resourceIdent"] = StiHyperlinkProcessor.ResourceIdent;
            parameters["variableIdent"] = StiHyperlinkProcessor.VariableIdent;
            parameters["blocklyIdent"] = StiBlocksConst.IdentXml;
            parameters["defaultDesignerOptions"] = StiDesignerOptionsHelper.GetDefaultDesignerOptions();
            parameters["showPropertiesWhichUsedFromStyles"] = StiOptions.Designer.PropertyGrid.ShowPropertiesWhichUsedFromStyles;
            parameters["wizardTypeRunningAfterLoad"] = options.Appearance.WizardTypeRunningAfterLoad;
            parameters["datePickerFirstDayOfWeek"] = options.Appearance.DatePickerFirstDayOfWeek;
            parameters["datePickerIncludeCurrentDayForRanges"] = options.Appearance.DatePickerIncludeCurrentDayForRanges;
            parameters["reportResourcesMaximumSize"] = StiOptions.Engine.ReportResources.MaximumSize;
            parameters["allowChangeWindowTitle"] = options.Behavior.AllowChangeWindowTitle;
            parameters["saveReportMode"] = options.Behavior.SaveReportMode;
            parameters["saveReportAsMode"] = options.Behavior.SaveReportAsMode;
            parameters["checkReportBeforePreview"] = options.Behavior.CheckReportBeforePreview;
            parameters["closeDesignerWithoutAsking"] = options.Appearance.CloseDesignerWithoutAsking;
            parameters["showSystemFonts"] = options.Appearance.ShowSystemFonts;
            parameters["alternateValid"] = StiLicenseHelper.CheckAnyLicense();
            parameters["licenseUserName"] = StiLicenseHelper.GetUserName();
            parameters["allowWordWrapTextEditors"] = options.Appearance.AllowWordWrapTextEditors;
            parameters["listSeparator"] = CultureInfo.CurrentCulture.TextInfo.ListSeparator;

            if (options.Appearance.Zoom == StiZoomMode.PageWidth || options.Appearance.Zoom == StiZoomMode.PageHeight)
            {
                parameters[options.Appearance.Zoom == StiZoomMode.PageWidth ? "setZoomToPageWidth" : "setZoomToPageHeight"] = true;
                options.Appearance.Zoom = 1;
            }
            parameters["zoom"] = options.Appearance.Zoom / 100d;

            //-------------- obsolete, for suporting old versions --------------
            parameters["runWizardAfterLoad"] = StiOptions.Designer.RunWizardAfterLoad;
            parameters["runSpecificWizardAfterLoad"] = StiOptions.Designer.RunSpecificWizardAfterLoad;
            //------------------------------------------------------------------

            if (options.Toolbar.ComponentsIntoInsertTab != null)
            {
                ArrayList componentsArray = new ArrayList();
                componentsArray.AddRange(options.Toolbar.ComponentsIntoInsertTab);
                parameters["componentsIntoInsertTab"] = componentsArray;
            }

            // Collections
            string[] collections = new string[] {
                "loc", "locFiles", "paperSizes", "hatchStyles", "summaryTypes", "aggrigateFunctions", "fontNames", "conditions", "iconSetArrays",
                "dBaseCodePages", "csvCodePages", "textFormats", "currencySymbols", "dateFormats", "timeFormats", "customFormats", "cultures", "fontIcons",
                "fontIconSets", "predefinedColors"
            };

            for (int i = 0; i < collections.Length; i++)
            {
                object collectionObject = null;
                switch (collections[i])
                {
                    case "loc": collectionObject = StiLocalization.GetLocalization(false); break;
                    case "locFiles": collectionObject = StiCollectionsHelper.GetLocalizationsList(requestParams.HttpContext, this.options.LocalizationDirectory); break;
                    case "paperSizes": collectionObject = StiPaperSizes.GetItems(); break;
                    case "hatchStyles": collectionObject = StiHatchStyles.GetItems(); break;
                    case "summaryTypes": collectionObject = StiSummaryTypes.GetItems(); break;
                    case "aggrigateFunctions": collectionObject = StiAggrigateFunctions.GetItems(); break;
                    case "fontNames": collectionObject = StiFontNames.GetItems(options.Server.AllowLoadingCustomFontsToClientSide); break;
                    case "conditions": collectionObject = StiDefaultConditions.GetItems(); break;
                    case "iconSetArrays": collectionObject = StiIconSetArrays.GetItems(); break;
                    case "dBaseCodePages": collectionObject = StiCodePageHelper.GetDBaseCodePageItems(); break;
                    case "csvCodePages": collectionObject = StiCodePageHelper.GetCsvCodePageItems(); break;
                    case "textFormats": collectionObject = StiTextFormatHelper.GetTextFormatItems(); break;
                    case "currencySymbols": collectionObject = StiTextFormatHelper.GetCurrencySymbols(); break;
                    case "dateFormats": collectionObject = StiTextFormatHelper.GetDateAndTimeFormats("date", new StiDateFormatService()); break;
                    case "timeFormats": collectionObject = StiTextFormatHelper.GetDateAndTimeFormats("time", new StiTimeFormatService()); break;
                    case "customFormats": collectionObject = StiTextFormatHelper.GetDateAndTimeFormats("custom", new StiCustomFormatService()); break;
                    case "cultures": collectionObject = StiCultureHelper.GetItems(CultureTypes.SpecificCultures); break;
                    case "fontIcons": collectionObject = StiWebFontIconsHelper.GetIconItems(); break;
                    case "fontIconSets": collectionObject = StiWebFontIconsHelper.GetIconSetItems(); break;
                    case "predefinedColors": collectionObject = StiColorHelper.GetPredefinedColors(); break;
                }

                parameters[collections[i]] = collectionObject;
            }

            parameters["stimulsoftFontContent"] = StiReportResourceHelper.GetStimulsoftFontBase64Data();

            if (!options.Appearance.ShowSystemFonts)
            {
                var fontItems = StiFontNames.GetOpenTypeFontItems();
                if (fontItems.Count > 0)
                    parameters["opentypeFonts"] = fontItems;
            }

            // ToolBar
            parameters["showToolbar"] = options.Toolbar.ShowToolbar;
            parameters["showInsertButton"] = options.Toolbar.ShowInsertButton;
            parameters["showLayoutButton"] = options.Toolbar.ShowLayoutButton;
            parameters["showPageButton"] = options.Toolbar.ShowPageButton;
            parameters["showPreviewButton"] = options.Toolbar.ShowPreviewButton;
            parameters["showSaveButton"] = options.Toolbar.ShowSaveButton;
            parameters["showAboutButton"] = options.Toolbar.ShowAboutButton;
            parameters["showSetupToolboxButton"] = options.Toolbar.ShowSetupToolboxButton;
            parameters["showFileMenu"] = options.FileMenu.Visible;
            parameters["showFileMenuNew"] = options.FileMenu.ShowNew;
            parameters["showFileMenuOpen"] = options.FileMenu.ShowOpen;
            parameters["showFileMenuSave"] = options.FileMenu.ShowSave;
            parameters["showFileMenuSaveAs"] = options.FileMenu.ShowSaveAs;
            parameters["showFileMenuClose"] = options.FileMenu.ShowClose;
            parameters["showFileMenuExit"] = options.FileMenu.ShowExit;
            parameters["showFileMenuReportSetup"] = options.FileMenu.ShowReportSetup;
            parameters["showFileMenuOptions"] = options.FileMenu.ShowOptions;
            parameters["showFileMenuInfo"] = options.FileMenu.ShowInfo;
            parameters["showFileMenuAbout"] = options.FileMenu.ShowAbout;
            parameters["showFileMenuHelp"] = options.FileMenu.ShowHelp;
            parameters["showFileMenuNewReport"] = options.FileMenu.ShowFileMenuNewReport;
            parameters["showFileMenuNewDashboard"] = options.FileMenu.ShowFileMenuNewDashboard;

            // Pages
            parameters["showNewPageButton"] = options.Pages.ShowNewPageButton;

            // Bands
            Hashtable visibilityBands = new Hashtable();
            parameters["visibilityBands"] = visibilityBands;
            visibilityBands["StiReportTitleBand"] = options.Bands.ShowReportTitleBand;
            visibilityBands["StiReportSummaryBand"] = options.Bands.ShowReportSummaryBand;
            visibilityBands["StiPageHeaderBand"] = options.Bands.ShowPageHeaderBand;
            visibilityBands["StiPageFooterBand"] = options.Bands.ShowPageFooterBand;
            visibilityBands["StiGroupHeaderBand"] = options.Bands.ShowGroupHeaderBand;
            visibilityBands["StiGroupFooterBand"] = options.Bands.ShowGroupFooterBand;
            visibilityBands["StiHeaderBand"] = options.Bands.ShowHeaderBand;
            visibilityBands["StiFooterBand"] = options.Bands.ShowFooterBand;
            visibilityBands["StiColumnHeaderBand"] = options.Bands.ShowColumnHeaderBand;
            visibilityBands["StiColumnFooterBand"] = options.Bands.ShowColumnFooterBand;
            visibilityBands["StiDataBand"] = options.Bands.ShowDataBand;
            visibilityBands["StiHierarchicalBand"] = options.Bands.ShowHierarchicalBand;
            visibilityBands["StiChildBand"] = options.Bands.ShowChildBand;
            visibilityBands["StiEmptyBand"] = options.Bands.ShowEmptyBand;
            visibilityBands["StiOverlayBand"] = options.Bands.ShowOverlayBand;
            visibilityBands["StiTable"] = options.Components.ShowTable;
            visibilityBands["StiTableOfContents"] = options.Bands.ShowTableOfContents;

            // Cross Bands
            Hashtable visibilityCrossBands = new Hashtable();
            parameters["visibilityCrossBands"] = visibilityCrossBands;
            visibilityCrossBands["StiCrossTab"] = options.Components.ShowCrossTab;
            visibilityCrossBands["StiCrossGroupHeaderBand"] = options.CrossBands.ShowCrossGroupHeaderBand;
            visibilityCrossBands["StiCrossGroupFooterBand"] = options.CrossBands.ShowCrossGroupFooterBand;
            visibilityCrossBands["StiCrossHeaderBand"] = options.CrossBands.ShowCrossHeaderBand;
            visibilityCrossBands["StiCrossFooterBand"] = options.CrossBands.ShowCrossFooterBand;
            visibilityCrossBands["StiCrossDataBand"] = options.CrossBands.ShowCrossDataBand;

            // Components
            Hashtable visibilityComponents = new Hashtable();
            parameters["visibilityComponents"] = visibilityComponents;
            visibilityComponents["StiText"] = options.Components.ShowText;
            visibilityComponents["StiTextInCells"] = options.Components.ShowTextInCells;
            visibilityComponents["StiRichText"] = options.Components.ShowRichText;
            visibilityComponents["StiImage"] = options.Components.ShowImage;
            visibilityComponents["StiBarCode"] = options.Components.ShowBarCode;
            visibilityComponents["StiShape"] = options.Components.ShowShape;
            visibilityComponents["StiPanel"] = options.Components.ShowPanel;
            visibilityComponents["StiClone"] = options.Components.ShowClone;
            visibilityComponents["StiCheckBox"] = options.Components.ShowCheckBox;
            visibilityComponents["StiSubReport"] = options.Components.ShowSubReport;
            visibilityComponents["StiZipCode"] = options.Components.ShowZipCode;
            visibilityComponents["StiChart"] = options.Components.ShowChart;
            visibilityComponents["StiMap"] = options.Components.ShowMap;
            visibilityComponents["StiGauge"] = options.Components.ShowGauge;
            visibilityComponents["StiHorizontalLinePrimitive"] = options.Components.ShowHorizontalLinePrimitive;
            visibilityComponents["StiVerticalLinePrimitive"] = options.Components.ShowVerticalLinePrimitive;
            visibilityComponents["StiRectanglePrimitive"] = options.Components.ShowRectanglePrimitive;
            visibilityComponents["StiRoundedRectanglePrimitive"] = options.Components.ShowRoundedRectanglePrimitive;
            visibilityComponents["StiSparkline"] = options.Components.ShowSparkline;
            visibilityComponents["StiMathFormula"] = options.Components.ShowMathFormula;
            visibilityComponents["StiElectronicSignature"] = options.Components.ShowElectronicSignature;
            visibilityComponents["StiPdfDigitalSignature"] = options.Components.ShowPdfDigitalSignature;

            //Dashboards
            parameters["showNewDashboardButton"] = options.Dashboards.ShowNewDashboardButton;

            // DashboardElements
            Hashtable visibilityDashboardElements = new Hashtable();
            parameters["visibilityDashboardElements"] = visibilityDashboardElements;
            visibilityDashboardElements["StiTableElement"] = options.DashboardElements.ShowTableElement;
            visibilityDashboardElements["StiChartElement"] = options.DashboardElements.ShowChartElement;
            visibilityDashboardElements["StiGaugeElement"] = options.DashboardElements.ShowGaugeElement;
            visibilityDashboardElements["StiPivotTableElement"] = options.DashboardElements.ShowPivotTableElement;
            visibilityDashboardElements["StiIndicatorElement"] = options.DashboardElements.ShowIndicatorElement;
            visibilityDashboardElements["StiProgressElement"] = options.DashboardElements.ShowProgressElement;
            visibilityDashboardElements["StiRegionMapElement"] = options.DashboardElements.ShowRegionMapElement;
            visibilityDashboardElements["StiOnlineMapElement"] = options.DashboardElements.ShowOnlineMapElement;
            visibilityDashboardElements["StiImageElement"] = options.DashboardElements.ShowImageElement;
            visibilityDashboardElements["StiTextElement"] = options.DashboardElements.ShowTextElement;
            visibilityDashboardElements["StiPanelElement"] = options.DashboardElements.ShowPanelElement;
            visibilityDashboardElements["StiShapeElement"] = options.DashboardElements.ShowShapeElement;
            visibilityDashboardElements["StiListBoxElement"] = options.DashboardElements.ShowListBoxElement;
            visibilityDashboardElements["StiComboBoxElement"] = options.DashboardElements.ShowComboBoxElement;
            visibilityDashboardElements["StiTreeViewElement"] = options.DashboardElements.ShowTreeViewElement;
            visibilityDashboardElements["StiTreeViewBoxElement"] = options.DashboardElements.ShowTreeViewBoxElement;
            visibilityDashboardElements["StiDatePickerElement"] = options.DashboardElements.ShowDatePickerElement;
            visibilityDashboardElements["StiCardsElement"] = options.DashboardElements.ShowCardsElement;
            visibilityDashboardElements["StiButtonElement"] = options.DashboardElements.ShowButtonElement;

            // Properties Grid
            parameters["showPropertiesGrid"] = options.PropertiesGrid.Visible;
            parameters["propertiesGridWidth"] = options.PropertiesGrid.Width;
            parameters["propertiesGridLabelWidth"] = options.PropertiesGrid.LabelWidth;
            parameters["propertiesGridPosition"] = options.PropertiesGrid.PropertiesGridPosition;
            parameters["showPropertiesWhichUsedFromStyles"] = options.PropertiesGrid.ShowPropertiesWhichUsedFromStyles;

            // Dictionary
            parameters["showDictionary"] = options.Dictionary.Visible;
            parameters["useAliasesDictionary"] = options.Dictionary.UseAliases;
            parameters["newReportDictionary"] = options.Dictionary.NewReportDictionary;
            parameters["showDictionaryContextMenuProperties"] = options.Dictionary.ShowDictionaryContextMenuProperties;
            parameters["permissionDataSources"] = options.Dictionary.PermissionDataSources.ToString();
            parameters["permissionDataColumns"] = options.Dictionary.PermissionDataColumns.ToString();
            parameters["permissionDataRelations"] = options.Dictionary.PermissionDataRelations.ToString();
            parameters["permissionDataConnections"] = options.Dictionary.PermissionDataConnections.ToString();
            parameters["permissionBusinessObjects"] = options.Dictionary.PermissionBusinessObjects.ToString();
            parameters["permissionVariables"] = options.Dictionary.PermissionVariables.ToString();
            parameters["permissionResources"] = options.Dictionary.PermissionResources.ToString();
            parameters["permissionSqlParameters"] = options.Dictionary.PermissionSqlParameters.ToString();

            parameters["showOnlyAliasForComponents"] = StiOptions.Dictionary.ShowOnlyAliasForComponents;
            parameters["showOnlyAliasForPages"] = StiOptions.Dictionary.ShowOnlyAliasForPages;
            parameters["showOnlyAliasForDatabase"] = StiOptions.Dictionary.ShowOnlyAliasForDatabase;
            parameters["showOnlyAliasForData"] = StiOptions.Dictionary.ShowOnlyAliasForData;
            parameters["showOnlyAliasForVariable"] = StiOptions.Dictionary.ShowOnlyAliasForVariable;
            parameters["showOnlyAliasForResource"] = StiOptions.Dictionary.ShowOnlyAliasForResource;
            parameters["showOnlyAliasForDataSource"] = StiOptions.Dictionary.ShowOnlyAliasForDataSource;
            parameters["showOnlyAliasForBusinessObject"] = StiOptions.Dictionary.ShowOnlyAliasForBusinessObject;
            parameters["showOnlyAliasForDataColumn"] = StiOptions.Dictionary.ShowOnlyAliasForDataColumn;
            parameters["showOnlyAliasForDataRelation"] = StiOptions.Dictionary.ShowOnlyAliasForDataRelation;

            // Report Tree
            parameters["showReportTree"] = options.Appearance.ShowReportTree;

            // Cursors
            parameters["urlCursorStyleSet"] = GetImageUrl(requestParams, resourcesUrl + "Cursors.StyleSet.cur");
            parameters["urlCursorPen"] = GetImageUrl(requestParams, resourcesUrl + "Cursors.Pen.cur");

            // MVC Actions
            Hashtable actions = new Hashtable();
            actions["getReport"] = options.Actions.GetReport;
            actions["openReport"] = options.Actions.OpenReport;
            actions["createReport"] = options.Actions.CreateReport;
            actions["saveReport"] = options.Actions.SaveReport;
            actions["saveReportAs"] = options.Actions.SaveReportAs;
            actions["previewReport"] = options.Actions.PreviewReport;
            actions["exportReport"] = options.Actions.ExportReport;
            actions["exit"] = options.Actions.Exit;
            actions["designerEvent"] = options.Actions.DesignerEvent;
            parameters["actions"] = actions;
            
            return JSON.Encode(parameters);
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            #region Render Control
            var jsParameters = RenderJsonParameters();

            var viewer = this.CreatePreviewControl();
            this.Controls.Add(viewer);

            var mainPanel = new Panel();
            mainPanel.CssClass = "stiDesignerMainPanel";
            mainPanel.ID = this.ID + "_MainPanel";
            this.Controls.Add(mainPanel);

            if (options.Width == Unit.Empty && options.Height == Unit.Empty)
            {
                this.Height = Unit.Empty;
                this.Style.Add("position", "absolute");
                this.Style.Add("z-index", "1000000");
                this.Style.Add("top", "0px");
                this.Style.Add("left", "0px");
                this.Style.Add("right", "0px");
                this.Style.Add("bottom", "0px");
            }
            else
            {
                mainPanel.Width = options.Width;
                mainPanel.Height = options.Height;
            }

            var scriptEngine = new StiJavaScript();
            scriptEngine.ScriptUrl = this.GetResourcesUrl() + "DesignerScripts";
            mainPanel.Controls.Add(scriptEngine);

            var scriptInit = new StiJavaScript();
            scriptInit.Text = string.Format("var js{0} = new StiMobileDesigner({1});", this.ID, jsParameters);
            mainPanel.Controls.Add(scriptInit);
            
            #endregion
        }
    }
}
