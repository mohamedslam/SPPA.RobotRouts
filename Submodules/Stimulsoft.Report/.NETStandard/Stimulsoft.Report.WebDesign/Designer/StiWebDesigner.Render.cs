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
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Helpers;
using System.Globalization;
using Stimulsoft.Report.Components.TextFormats;
using System.Web;
using System.Text;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Base;
using Stimulsoft.Base.Plans;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;
using Stimulsoft.Base.Blocks;

#if SERVER
using Stimulsoft.Server.Objects;
#endif

namespace Stimulsoft.Report.Web
{
    public partial class StiWebDesigner :
        WebControl,
        INamingContainer
    {
        #region JSON parameters

        internal object RenderJsonParameters(bool notEncodeToJson = false)
        {
            // Create JS parameters table
            Hashtable parameters = new Hashtable();
            Hashtable designParams = null;

            //QuickBooks Auth
            if ((Page.Request.Params["code"] != null || Page.Request.Params["error"] != null) && Page.Request.Params["state"] != null)
            {
                parameters["oAuthCode"] = Page.Request.Params["code"];
                parameters["oAuthError"] = Page.Request.Params["error"];
                parameters["oAuthRealmId"] = Page.Request.Params["realmId"];

                StiQuickBooksHelper.QuickBooksAuthorizationProcess(parameters);
            }

            //Is Cloud or Server Mode
            if (CloudMode || ServerMode)
            {
                designParams = new Hashtable();                

                #region Cloud Mode
                if (CloudMode)
                {
#if CLOUD
                    parameters["googleAuthRedirectUrl"] = "https://designer.stimulsoft.com";

                    //Google Auth
                    if ((Page.Request.Params["code"] != null || Page.Request.Params["error"] != null) && Page.Request.Params["scope"] != null)
                    {
                        parameters["oAuthCode"] = Page.Request.Params["code"];
                        parameters["oAuthError"] = Page.Request.Params["error"];

                        StiGoogleAccountHelper.GoogleAuthorizationProcess(parameters);
                    }

                    //GitHub Auth
                    if ((Page.Request.Params["code"] != null || Page.Request.Params["error"] != null) && Page.Request.Params["state"] != null)
                    {
                        parameters["gitHubAuthCode"] = Page.Request.Params["code"];
                        parameters["gitHubAuthError"] = Page.Request.Params["error"];

                        StiGitHubAccountHelper.GitHubAuthorizationProcess(parameters);
                    }

                    //Facebook Auth
                    if (Page.Request.Params["code"] != null || Page.Request.Params["error"] != null)
                    {
                        parameters["facebookAuthCode"] = Page.Request.Params["code"];
                        parameters["facebookAuthError"] = Page.Request.Params["error"];

                        StiFacebookAccountHelper.FacebookAuthorizationProcess(parameters);
                    }
#endif

                    var stimulsoftRequestSource = Page.Session["StimulsoftRequestSource"] as string;
                    if (stimulsoftRequestSource == "website")
                    {
                        // Run from stimulsoft.com
                        var sessionKey = Page.Session["SessionKey"] as string;
                        var userKey = Page.Session["UserKey"] as string;

                        if (!string.IsNullOrEmpty(sessionKey) && !string.IsNullOrEmpty(userKey))
                        {
                            designParams["sessionKey"] = Page.Session["SessionKey"];
                            designParams["userKey"] = Page.Session["UserKey"];
                        }

                        var templateType = Page.Session["TemplateType"] as string;
                        var templateKey = Page.Session["TemplateKey"] as string;

                        if (templateType == "Embedded" && !string.IsNullOrEmpty(templateKey))
                        {
                            var startParameters = new Hashtable();
                            startParameters["action"] = $"dashboard{templateKey}Button";
                            designParams["startParameters"] = StiEncodingHelper.Encode(JSON.Encode(startParameters));
                        }
                    }
                    else if (Page.Session["designerParams"] != null)
                    {
                        var cloudParams = JsonConvert.DeserializeObject<Hashtable>(Encoding.UTF8.GetString(Convert.FromBase64String(Page.Session["designerParams"] as string)));

                        if (cloudParams.Contains("demomode"))
                        {
                            //Run from demo.stimulsoft.com
                            designParams["demomode"] = true;
                        }
                        else
                        {
                            designParams["sessionKey"] = cloudParams["sessionKey"];
                            designParams["userKey"] = cloudParams["userKey"];
                            designParams["reportTemplateItemKey"] = cloudParams["reportTemplateItemKey"];
                            designParams["versionKey"] = cloudParams["versionKey"];
                            designParams["isDashboard"] = cloudParams["reportType"] as string == "dbs";
                            designParams["isForm"] = cloudParams["reportType"] as string == "form";
                            designParams["startParameters"] = cloudParams["startParameters"];

                            if (cloudParams.Contains("reportName"))
                                designParams["reportName"] = Encoding.UTF8.GetString(Convert.FromBase64String(cloudParams["reportName"] as string));
                        }

                        designParams["themeName"] = cloudParams["themeName"];
                        designParams["localizationName"] = cloudParams["localizationName"];
                    }

                    //Additional parameters
                    designParams["restUrl"] = CloudServerAdress;
                    designParams["permissionDataSources"] = "All";
                    designParams["favIcon"] = "favicon.ico";
                }
                #endregion

                #region Server Mode
                else if (ServerMode && Page.Request.Form.AllKeys != null || Page.Request.Form.AllKeys.Length > 0)
                {
                    for (int i = 0; i < Page.Request.Form.AllKeys.Length; i++)
                    {
                        designParams[Page.Request.Form.AllKeys[i]] = HttpContext.Current.Request.Form[Page.Request.Form.AllKeys[i]];
                    }

                    if (designParams["reportName"] != null)
                        designParams["reportName"] = StiEncodingHelper.DecodeString(designParams["reportName"] as string);

                    if (designParams["attachedItems"] != null)
                        designParams["attachedItems"] = JSON.Decode(designParams["attachedItems"] as string) as ArrayList;

                    if (designParams["resourceItems"] != null)
                        designParams["resourceItems"] = JSON.Decode(designParams["resourceItems"] as string) as ArrayList;
                }
                #endregion

                if (designParams["themeName"] != null)
                {
                    this.Theme = (StiDesignerTheme)Enum.Parse(typeof(StiDesignerTheme), designParams["themeName"] as string);
                    this.Viewer.Theme = (StiViewerTheme)Enum.Parse(typeof(StiViewerTheme), designParams["themeName"] as string);
                }

                parameters["cloudParameters"] = designParams;
            }

            // Try to load localization file
            if (!IsDesignMode) 
                StiCollectionsHelper.LoadLocalizationFile(HttpContext.Current, this.LocalizationDirectory, this.Localization);

            //Plans Limits
            if (CloudMode || notEncodeToJson)
            {
                parameters["plansLimits"] = JsonConvert.SerializeObject(StiCloudHelper.GetPlansLimits(), Formatting.None, new StringEnumConverter());
                parameters["productIdentKeys"] = StiCloudHelper.GetProductIdentKeys();
            }

            // Settings: General
            parameters["mobileDesignerId"] = this.ClientID;
            parameters["viewerId"] = this.ID + "Viewer";
            parameters["cloudMode"] = this.CloudMode;
            parameters["serverMode"] = this.ServerMode;
            parameters["theme"] = this.Theme;
            parameters["cultureName"] = StiLocalization.CultureName;
            parameters["productVersion"] = StiVersionHelper.ProductVersion;
            parameters["shortProductVersion"] = StiVersionHelper.AssemblyVersion;
            parameters["frameworkType"] = "ASP.NET";
            parameters["dashboardAssemblyLoaded"] = StiDashboardAssembly.IsAssemblyLoaded && StiDashboardExportAssembly.IsAssemblyLoaded && StiDashboardDrawingAssembly.IsAssemblyLoaded;
            parameters["chartAssemblyLoaded"] = true;
            parameters["allowAutoUpdateCookies"] = this.AllowAutoUpdateCookies;

            // Settings: Report cache
            parameters["clientGuid"] = this.ClientGuid;
            parameters["reportGuid"] = this.ClientGuid;
            parameters["cacheMode"] = this.CacheMode;
            parameters["requestTimeout"] = this.RequestTimeout;
            parameters["cacheTimeout"] = this.CacheTimeout;
            parameters["cacheItemPriority"] = this.CacheItemPriority;
            parameters["useRelativeUrls"] = this.UseRelativeUrls;
            parameters["allowAutoUpdateCache"] = this.AllowAutoUpdateCache;
            parameters["useCacheHelper"] = CacheHelper != null && CacheHelper.GetType() != typeof(StiCacheHelper);
            parameters["useCacheForResources"] = this.UseCacheForResources;

            // Settings: URLs
            string resourcesUrl = this.GetResourcesUrl();
            parameters["useCompression"] = this.UseCompression;
            parameters["passQueryParametersForResources"] = this.PassQueryParametersForResources;
            parameters["cloudServerUrl"] = StiHyperlinkProcessor.ServerIdent;
            parameters["requestUrl"] = GetRequestUrl(this.UseRelativeUrls, true, this.PortNumber);
            parameters["stylesUrl"] = string.IsNullOrEmpty(this.CustomCss) ? resourcesUrl.Replace("&stiweb_data=", "&stiweb_theme=") + this.Theme.ToString() : this.CustomCss;
            parameters["scriptsUrl"] = resourcesUrl;
            parameters["imagesUrl"] = resourcesUrl;

            // Settings
            parameters["showAnimation"] = this.ShowAnimation;
            parameters["defaultUnit"] = this.DefaultUnit;
            parameters["focusingX"] = this.FocusingX;
            parameters["focusingY"] = this.FocusingY;
            parameters["showSaveDialog"] = this.ShowSaveDialog;
            parameters["fullScreenMode"] = this.Width == Unit.Empty && this.Height == Unit.Empty;
            parameters["haveExitDesignerEvent"] = this.Exit != null;
            parameters["haveSaveEvent"] = this.SaveReport != null;
            parameters["haveSaveAsEvent"] = this.SaveReportAs != null;
            parameters["showOpenDialog"] = this.ShowOpenDialog;
            parameters["showTooltips"] = this.ShowTooltips;
            parameters["showTooltipsHelp"] = this.ShowTooltipsHelp;
            parameters["showDialogsHelp"] = this.ShowDialogsHelp;
            parameters["interfaceType"] = this.InterfaceType;
            parameters["undoMaxLevel"] = this.UndoMaxLevel;
            parameters["resourceIdent"] = StiHyperlinkProcessor.ResourceIdent;
            parameters["variableIdent"] = StiHyperlinkProcessor.VariableIdent;
            parameters["blocklyIdent"] = StiBlocksConst.IdentXml;
            parameters["defaultDesignerOptions"] = StiDesignerOptionsHelper.GetDefaultDesignerOptions();
            parameters["wizardTypeRunningAfterLoad"] = this.WizardTypeRunningAfterLoad;
            parameters["datePickerFirstDayOfWeek"] = this.DatePickerFirstDayOfWeek;
            parameters["reportResourcesMaximumSize"] = StiOptions.Engine.ReportResources.MaximumSize;
            parameters["allowChangeWindowTitle"] = this.AllowChangeWindowTitle;
            parameters["saveReportMode"] = this.SaveReportMode;
            parameters["saveReportAsMode"] = this.SaveReportAsMode;
            parameters["checkReportBeforePreview"] = this.CheckReportBeforePreview;            
            parameters["closeDesignerWithoutAsking"] = this.CloseDesignerWithoutAsking;
            parameters["showSystemFonts"] = this.ShowSystemFonts;
            parameters["netCoreMode"] = false;
            parameters["alternateValid"] = StiLicenseHelper.CheckAnyLicense();
            parameters["buildDate"] = StiVersion.Created.ToString(CultureInfo.CreateSpecificCulture("en-US"));
            parameters["conditionsPredefinedStyles"] = StiConditionsStylesHelper.GetPredefinedStylesItems();
            parameters["allowWordWrapTextEditors"] = this.AllowWordWrapTextEditors;
            parameters["listSeparator"] = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            parameters["newReportDictionary"] = this.NewReportDictionary;
            parameters["showDictionaryContextMenuProperties"] = this.ShowDictionaryContextMenuProperties;

            if (this.Zoom == StiZoomMode.PageWidth || this.Zoom == StiZoomMode.PageHeight)
            {
                parameters[this.Zoom == StiZoomMode.PageWidth ? "setZoomToPageWidth" : "setZoomToPageHeight"] = true;
                this.Zoom = 1;
            }
            parameters["zoom"] = this.Zoom / 100d;

            //-------------- obsolete, for suporting old versions --------------
            parameters["runWizardAfterLoad"] = StiOptions.Designer.RunWizardAfterLoad; 
            parameters["runSpecificWizardAfterLoad"] = StiOptions.Designer.RunSpecificWizardAfterLoad;
            //------------------------------------------------------------------

            if (!notEncodeToJson)
            {
                parameters["licenseUserName"] = StiLicenseHelper.GetUserName();
#if DEBUG
                parameters["isDebugMode"] = true;
#endif
            }

            if (ComponentsIntoInsertTab != null)
            {
                ArrayList componentsArray = new ArrayList();
                componentsArray.AddRange(ComponentsIntoInsertTab);
                parameters["componentsIntoInsertTab"] = componentsArray;
            }

            // Collections
            string[] collections = new string[] {
                "loc", "locFiles", "paperSizes", "hatchStyles", "summaryTypes", "aggrigateFunctions", "fontNames", "conditions", "iconSetArrays",
                "dBaseCodePages", "csvCodePages", "textFormats", "currencySymbols", "dateFormats", "timeFormats", "customFormats", "cultures", "fontIcons",
                "fontIconSets", "predefinedColors", "productsIdents"
            };

            for (int i = 0; i < collections.Length; i++)
            {
                object collectionObject = null;
                switch (collections[i])
                {
                    case "loc":
                        if (ServerMode && designParams != null && designParams["localizationName"] != null && designParams["sessionKey"] != null)
                        {
#if SERVER
                            var getLocalizationCommand = new StiLocalizationCommands.Get()
                            {
                                Name = designParams["localizationName"] as string,
                                SessionKey = designParams["sessionKey"] as string,
                                Set = StiLocalizationSet.Reports,
                                Type = StiLocalizationFormatType.Json
                            };

                            var resultLocalizationCommand = RunCommand(getLocalizationCommand) as StiLocalizationCommands.Get;
                            if (resultLocalizationCommand.ResultSuccess && resultLocalizationCommand.ResultReportsJson != null)
                                collectionObject = StiEncodingHelper.DecodeString(resultLocalizationCommand.ResultReportsJson);

                            parameters["cultureName"] = designParams["localizationName"] as string;
#endif
                        }
                        else
                        {
                            collectionObject = StiLocalization.GetLocalization(false);
                        }
                        break;

                    case "locFiles": collectionObject = StiCollectionsHelper.GetLocalizationsList(HttpContext.Current, this.LocalizationDirectory); break;
                    case "paperSizes": collectionObject = StiPaperSizes.GetItems(); break;
                    case "hatchStyles": collectionObject = StiHatchStyles.GetItems(); break;
                    case "summaryTypes": collectionObject = StiSummaryTypes.GetItems(); break;
                    case "aggrigateFunctions": collectionObject = StiAggrigateFunctions.GetItems(); break;
                    case "fontNames": collectionObject = StiFontNames.GetItems(this.AllowLoadingCustomFontsToClientSide); break;
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
                    case "productsIdents": collectionObject = StiLicenseProductsHelper.GetProductsIdents(); break;
                }

                parameters[collections[i]] = collectionObject;
            }

            if (!notEncodeToJson)
                parameters["stimulsoftFontContent"] = StiReportResourceHelper.GetStimulsoftFontBase64Data();

            if (!ShowSystemFonts)
            {
                var fontItems = StiFontNames.GetOpenTypeFontItems();
                if (fontItems.Count > 0)
                    parameters["opentypeFonts"] = fontItems;
            }

            // ToolBar
            parameters["showToolbar"] = ShowToolbar;
            parameters["showInsertButton"] = ShowInsertButton;
            parameters["showLayoutButton"] = ShowLayoutButton;
            parameters["showPageButton"] = ShowPageButton;
            parameters["showPreviewButton"] = ShowPreviewButton;
            parameters["showFileMenu"] = ShowFileMenu;
            parameters["showSaveButton"] = ShowSaveButton;
            parameters["showAboutButton"] = ShowAboutButton;
            parameters["showSetupToolboxButton"] = ShowSetupToolboxButton;
            parameters["showFileMenuNew"] = ShowFileMenuNew;
            parameters["showFileMenuOpen"] = ShowFileMenuOpen;
            parameters["showFileMenuSave"] = ShowFileMenuSave;
            parameters["showFileMenuSaveAs"] = ShowFileMenuSaveAs;
            parameters["showFileMenuClose"] = ShowFileMenuClose;
            parameters["showFileMenuExit"] = ShowFileMenuExit;
            parameters["showFileMenuReportSetup"] = ShowFileMenuReportSetup;
            parameters["showFileMenuOptions"] = ShowFileMenuOptions;
            parameters["showFileMenuInfo"] = ShowFileMenuInfo;
            parameters["showFileMenuAbout"] = ShowFileMenuAbout;
            parameters["showFileMenuHelp"] = ShowFileMenuHelp;
            parameters["showFileMenuNewReport"] = ShowFileMenuNewReport;
            parameters["showFileMenuNewDashboard"] = ShowFileMenuNewDashboard;            

            // Pages
            parameters["showNewPageButton"] = ShowNewPageButton;

            // Bands
            Hashtable visibilityBands = new Hashtable();
            parameters["visibilityBands"] = visibilityBands;
            visibilityBands["StiReportTitleBand"] = ShowReportTitleBand;
            visibilityBands["StiReportSummaryBand"] = ShowReportSummaryBand;
            visibilityBands["StiPageHeaderBand"] = ShowPageHeaderBand;
            visibilityBands["StiPageFooterBand"] = ShowPageFooterBand;
            visibilityBands["StiGroupHeaderBand"] = ShowGroupHeaderBand;
            visibilityBands["StiGroupFooterBand"] = ShowGroupFooterBand;
            visibilityBands["StiHeaderBand"] = ShowHeaderBand;
            visibilityBands["StiFooterBand"] = ShowFooterBand;
            visibilityBands["StiColumnHeaderBand"] = ShowColumnHeaderBand;
            visibilityBands["StiColumnFooterBand"] = ShowColumnFooterBand;
            visibilityBands["StiDataBand"] = ShowDataBand;
            visibilityBands["StiHierarchicalBand"] = ShowHierarchicalBand;
            visibilityBands["StiChildBand"] = ShowChildBand;
            visibilityBands["StiEmptyBand"] = ShowEmptyBand;
            visibilityBands["StiOverlayBand"] = ShowOverlayBand;
            visibilityBands["StiTable"] = ShowTable;
            visibilityBands["StiTableOfContents"] = ShowTableOfContents;

            // Bands
            Hashtable visibilityCrossBands = new Hashtable();
            parameters["visibilityCrossBands"] = visibilityCrossBands;
            visibilityCrossBands["StiCrossTab"] = ShowCrossTab;
            visibilityCrossBands["StiCrossGroupHeaderBand"] = ShowCrossGroupHeaderBand;
            visibilityCrossBands["StiCrossGroupFooterBand"] = ShowCrossGroupFooterBand;
            visibilityCrossBands["StiCrossHeaderBand"] = ShowCrossHeaderBand;
            visibilityCrossBands["StiCrossFooterBand"] = ShowCrossFooterBand;
            visibilityCrossBands["StiCrossDataBand"] = ShowCrossDataBand;

            // Components
            Hashtable visibilityComponents = new Hashtable();
            parameters["visibilityComponents"] = visibilityComponents;
            visibilityComponents["StiText"] = ShowText;
            visibilityComponents["StiTextInCells"] = ShowTextInCells;
            visibilityComponents["StiRichText"] = ShowRichText;
            visibilityComponents["StiImage"] = ShowImage;
            visibilityComponents["StiBarCode"] = ShowBarCode;
            visibilityComponents["StiShape"] = ShowShape;
            visibilityComponents["StiPanel"] = ShowPanel;
            visibilityComponents["StiClone"] = ShowClone;
            visibilityComponents["StiCheckBox"] = ShowCheckBox;
            visibilityComponents["StiSubReport"] = ShowSubReport;
            visibilityComponents["StiZipCode"] = ShowZipCode;
            visibilityComponents["StiChart"] = ShowChart;
            visibilityComponents["StiMap"] = ShowMap;
            visibilityComponents["StiGauge"] = ShowGauge;
            visibilityComponents["StiHorizontalLinePrimitive"] = ShowHorizontalLinePrimitive;
            visibilityComponents["StiVerticalLinePrimitive"] = ShowVerticalLinePrimitive;
            visibilityComponents["StiRectanglePrimitive"] = ShowRectanglePrimitive;
            visibilityComponents["StiRoundedRectanglePrimitive"] = ShowRoundedRectanglePrimitive;
            visibilityComponents["StiSparkline"] = ShowSparkline;
            visibilityComponents["StiMathFormula"] = ShowMathFormula;
            visibilityComponents["StiElectronicSignature"] = ShowElectronicSignature;
            visibilityComponents["StiPdfDigitalSignature"] = ShowPdfDigitalSignature;


            //Dashboards
            parameters["showNewDashboardButton"] = ShowNewDashboardButton;

            // DashboardElements
            Hashtable visibilityDashboardElements = new Hashtable();
            parameters["visibilityDashboardElements"] = visibilityDashboardElements;
            visibilityDashboardElements["StiTableElement"] = ShowTableElement;
            visibilityDashboardElements["StiChartElement"] = ShowChartElement;
            visibilityDashboardElements["StiGaugeElement"] = ShowGaugeElement;
            visibilityDashboardElements["StiPivotTableElement"] = ShowPivotTableElement;
            visibilityDashboardElements["StiIndicatorElement"] = ShowIndicatorElement;
            visibilityDashboardElements["StiProgressElement"] = ShowProgressElement;
            visibilityDashboardElements["StiCardsElement"] = ShowCardsElement;
            visibilityDashboardElements["StiRegionMapElement"] = ShowRegionMapElement;
            visibilityDashboardElements["StiOnlineMapElement"] = ShowOnlineMapElement;
            visibilityDashboardElements["StiImageElement"] = ShowImageElement;
            visibilityDashboardElements["StiTextElement"] = ShowTextElement;
            visibilityDashboardElements["StiPanelElement"] = ShowPanelElement;
            visibilityDashboardElements["StiShapeElement"] = ShowShapeElement;
            visibilityDashboardElements["StiButtonElement"] = ShowButtonElement;
            visibilityDashboardElements["StiListBoxElement"] = ShowListBoxElement;
            visibilityDashboardElements["StiComboBoxElement"] = ShowComboBoxElement;
            visibilityDashboardElements["StiTreeViewElement"] = ShowTreeViewElement;
            visibilityDashboardElements["StiTreeViewBoxElement"] = ShowTreeViewBoxElement;
            visibilityDashboardElements["StiDatePickerElement"] = ShowDatePickerElement;

            // Properties Grid
            parameters["showPropertiesGrid"] = ShowPropertiesGrid;
            parameters["propertiesGridWidth"] = PropertiesGridWidth;
            parameters["propertiesGridLabelWidth"] = PropertiesGridLabelWidth;
            parameters["propertiesGridPosition"] = PropertiesGridPosition;
            parameters["showPropertiesWhichUsedFromStyles"] = ShowPropertiesWhichUsedFromStyles;

            // Dictionary
            parameters["useAliasesDictionary"] = UseAliases;
            parameters["showDictionary"] = ShowDictionary;
            parameters["permissionDataSources"] = (CloudMode || ServerMode) && designParams != null ? (string)designParams["permissionDataSources"] : PermissionDataSources.ToString();
            parameters["permissionDataColumns"] = (CloudMode || ServerMode) && designParams != null ? (string)designParams["permissionDataSources"] : PermissionDataColumns.ToString();
            parameters["permissionDataRelations"] = (CloudMode || ServerMode) && designParams != null ? (string)designParams["permissionDataSources"] : PermissionDataRelations.ToString();
            parameters["permissionDataConnections"] = (CloudMode || ServerMode) && designParams != null ? (string)designParams["permissionDataSources"] : PermissionDataConnections.ToString();
            parameters["permissionBusinessObjects"] = (CloudMode || ServerMode) && designParams != null ? "None" : PermissionBusinessObjects.ToString();
            parameters["permissionVariables"] = PermissionVariables.ToString();
            parameters["permissionResources"] = PermissionResources.ToString();
            parameters["permissionSqlParameters"] = PermissionSqlParameters.ToString();
            parameters["permissionDataTransformations"] = PermissionDataTransformations.ToString();

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
            parameters["showReportTree"] = ShowReportTree;

            // Cursors
            StiRequestParams requestParams = this.CreateRequestParams();
            parameters["urlCursorStyleSet"] = GetImageUrl(requestParams, resourcesUrl + "Cursors.StyleSet.cur");
            parameters["urlCursorPen"] = GetImageUrl(requestParams, resourcesUrl + "Cursors.Pen.cur");

            if (notEncodeToJson)
                return parameters;
            else
                return JSON.Encode(parameters);
        }

        #endregion

        #region Override Methods

        protected override void CreateChildControls()
        {
            this.Controls.Clear();

            Panel mainPanel = new Panel();
            mainPanel.ID = this.ID + "_MainPanel";
            mainPanel.CssClass = "stiDesignerMainPanel";
            this.Controls.Add(mainPanel);

            if (this.Width == Unit.Empty && this.Height == Unit.Empty)
            {
                this.Height = Unit.Empty;
                this.Style.Add("position", "fixed");
                this.Style.Add("z-index", "1000000");
                this.Style.Add("top", "0");
                this.Style.Add("right", "0");
                this.Style.Add("bottom", "0");
                this.Style.Add("left", "0");
            }
            else
            {
                mainPanel.Width = this.Width;
                mainPanel.Height = this.Height;
            }

            if (!IsDesignMode)
            {
                this.Controls.Add(this.Viewer);

                var jsParameters = RenderJsonParameters() as string;
                var initScript = new StiJavaScript();
                initScript.Text = string.Format("var js{0} = new StiMobileDesigner({1});", this.ID, jsParameters);
                mainPanel.Controls.Add(initScript);
            }

            base.CreateChildControls();
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            #region Design Mode
            if (IsDesignMode)
            {
                StiRequestParams requestParams = this.CreateRequestParams();

                Panel panel = new Panel();
                panel.Width = this.Width == Unit.Empty ? Unit.Percentage(100) : this.Width;
                panel.Height = this.Height == Unit.Empty ? Unit.Percentage(100) : this.Height;
                panel.Style.Add("overflow", "hidden");

                Table mainTable = new Table();
                mainTable.CellPadding = 0;
                mainTable.CellSpacing = 0;
                mainTable.Width = Unit.Percentage(100);
                mainTable.Height = Unit.Percentage(100);
                mainTable.BorderColor = this.BorderColor.IsEmpty ? Color.DarkGray : this.BorderColor;
                mainTable.BorderWidth = this.BorderWidth.IsEmpty ? 2 : this.BorderWidth;
                mainTable.BorderStyle = this.BorderStyle == BorderStyle.NotSet ? BorderStyle.Solid : this.BorderStyle;
                panel.Controls.Add(mainTable);

                TableRow rowToolbar = new TableRow();
                rowToolbar.VerticalAlign = VerticalAlign.Top;
                mainTable.Rows.Add(rowToolbar);

                TableCell cellToolbarLeft = new TableCell();
                cellToolbarLeft.Style.Add("width", "966px");
                cellToolbarLeft.Style.Add("height", "133px");
                cellToolbarLeft.Style.Add("background", string.Format("url('{0}')", GetImageUrl(requestParams, "DesignToolbarLeftHalf.png")));
                rowToolbar.Cells.Add(cellToolbarLeft);

                TableCell cellToolbarMiddle = new TableCell();
                cellToolbarMiddle.Style.Add("height", "133px");
                cellToolbarMiddle.Style.Add("background", string.Format("url('{0}')", GetImageUrl(requestParams, "DesignToolbarMiddleHalf.png")));
                rowToolbar.Cells.Add(cellToolbarMiddle);

                TableCell cellToolbarRight = new TableCell();
                cellToolbarRight.Style.Add("width", "24px");
                cellToolbarRight.Style.Add("height", "133px");
                rowToolbar.Cells.Add(cellToolbarRight);

                Panel rightPanel = new Panel();
                rightPanel.Style.Add("width", "24px");
                rightPanel.Style.Add("height", "133px");
                rightPanel.Style.Add("background", string.Format("url('{0}')", GetImageUrl(requestParams, "DesignToolbarRightHalf.png")));
                cellToolbarRight.Controls.Add(rightPanel);

                TableRow rowCaption = new TableRow();
                mainTable.Rows.Add(rowCaption);

                TableCell cellCaption = new TableCell();
                cellCaption.ColumnSpan = 3;
                cellCaption.Height = Unit.Percentage(100);
                cellCaption.Font.Name = "Arial";
                cellCaption.Text = "<strong>HTML5 Web Designer</strong><br />" + this.ID;
                cellCaption.HorizontalAlign = HorizontalAlign.Center;
                rowCaption.Cells.Add(cellCaption);

                panel.RenderControl(output);
            }
            #endregion

            #region Runtime Mode
            else
            {
                base.RenderContents(output);
            }
            #endregion
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!IsDesignMode)
            {
                this.EnsureChildControls();

                var url = StiUrlHelper.EscapeUrlQuotes(this.GetResourcesUrl());
                var script = string.Format("<script type=\"text/javascript\" src=\"{0}DesignerScripts\"></script>", url);
                RegisterClientScriptBlockIntoHeader(url, script);
            }

            base.OnPreRender(e);
        }

        #endregion
    }
}
