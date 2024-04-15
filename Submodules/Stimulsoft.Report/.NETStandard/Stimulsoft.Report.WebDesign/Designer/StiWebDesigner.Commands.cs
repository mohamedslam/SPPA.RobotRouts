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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.BarCodes;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Table;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Check;
using Stimulsoft.Report.Maps;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Web.Helpers.Dashboards;
using Stimulsoft.Report.SaveLoad;
using StiDashboardHelper = Stimulsoft.Report.Web.Helpers.Dashboards.StiDashboardHelper;

#if SERVER
using Stimulsoft.Server.Objects;
using Stimulsoft.Server.Connect;
using Stimulsoft.Server;
using Stimulsoft.Client;
#endif

#if NETSTANDARD
using Stimulsoft.System.Web.UI;
using Stimulsoft.System.Web.UI.WebControls;
using System.Threading.Tasks;
#else
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using Stimulsoft.Base.Plans;
using Stimulsoft.Base.Exceptions;
using Stimulsoft.Report.Export;
using System.Threading.Tasks;
#endif

namespace Stimulsoft.Report.Web
{
    public partial class StiWebDesigner :
        WebControl,
        INamingContainer
    {
        #region Process Request
#if !NETSTANDARD
        internal void ProcessRequest(StiRequestParams requestParams)
        {
            // Process preview tab requests
            if (requestParams.ComponentType == StiComponentType.Viewer)
            {
                if (requestParams.Action == StiAction.GetReport && GetPreviewReport != null)
                    InvokeGetPreviewReport(this.Report);

                this.Viewer.ProcessRequest(requestParams);
            }

            // Process designer requests
            if (requestParams.ComponentType == StiComponentType.Designer &&
                requestParams.Action != StiAction.Undefined &&
                (requestParams.Id == this.ID || requestParams.Action == StiAction.Resource))
            {
                this.clientGuid = requestParams.Cache.ClientGuid;
                StiWebActionResult result = null;
                var currentReport = this.Report;
                InvokeDesignerEvent();

                switch (requestParams.Action)
                {
                    case StiAction.Resource:
                        result = StiDesignerResourcesHelper.Get(requestParams);
                        break;

                    case StiAction.GetReport:
                        try
                        {
                            InvokeGetReport(currentReport);
                        }
                        catch (StiDashboardNotSupportedException e)
                        {
                            Console.Write(GetErrorMessageText(e));
                            result = StiWebActionResult.DashboardNotSupportedResult(requestParams);
                        }
                        catch (Exception e)
                        {
                            result = StiWebActionResult.ErrorResult(requestParams, GetErrorMessageText(e));
                        }
                        break;

                    case StiAction.OpenReport:
                        try
                        {
                            #region Load big reports by blocks
                            if (requestParams.All["countBlocks"] != null)
                            {
                                var callbackResult = new Hashtable();
                                var cacheGuid = requestParams.All["cacheGuid"] as string;
                                var countBlocks = Convert.ToInt32(requestParams.All["countBlocks"]);

                                var blocks = requestParams.Cache.Helper.GetObjectInternal(requestParams, cacheGuid) as List<string>;
                                if (blocks == null) blocks = new List<string>();
                                blocks.Add(requestParams.All["blockContent"] as string);

                                callbackResult["progress"] = blocks.Count / (float)countBlocks;

                                if (countBlocks > blocks.Count)
                                {
                                    requestParams.Cache.Helper.SaveObjectInternal(blocks, requestParams, cacheGuid);
                                    callbackResult["loadingCompleted"] = false;
                                    callbackResult["command"] = "OpenReport";
                                    callbackResult["commandGuid"] = requestParams.All["commandGuid"];
                                    result = StiWebActionResult.JsonResult(requestParams, callbackResult);
                                    StiReportResponse.ResponseBuffer(result.Data, result.ContentType, requestParams.Action == StiAction.Resource, result.FileName);
                                    return;
                                }
                                else
                                {
                                    var resultStrings = new string[countBlocks];
                                    var resultString = string.Empty;
                                    blocks.ForEach(s => { resultStrings[Convert.ToInt32(s.Substring(0, 1))] = s.Substring(1); });
                                    resultStrings.ToList().ForEach(s => { resultString += s; });
                                    requestParams.Data = Convert.FromBase64String(resultString);
                                    requestParams.Cache.Helper.RemoveObject(cacheGuid);
                                }
                            }
                            #endregion

                            // The try/catch block is needed for opening incorrect report file or wrong report password
                            currentReport = LoadReportFromContent(requestParams);
                            
                            InvokeOpenReport(currentReport);

                            if (requestParams.Data != null)
                            {
                                #region CheckResourcesLimits
#if CLOUD
                                if (requestParams.All["cp"] != null && currentReport != null)
                                {
                                    var plan = StiCloudPlan.GetPlan(StiDesignReportHelper.ParseCloudPlanIdent(requestParams.All["cp"]));
                                    if (currentReport.Dictionary.Resources.Count > plan.MaxResources)
                                    {
                                        var callbackResult = new Hashtable();
                                        callbackResult["maxResourcesExceeded"] = plan.MaxResources;
                                        callbackResult["command"] = "OpenReport";
                                        result = StiWebActionResult.JsonResult(requestParams, callbackResult);
                                        StiReportResponse.ResponseBuffer(result.Data, result.ContentType, requestParams.Action == StiAction.Resource, result.FileName);
                                        return;
                                    }
                                    var maxResourceSizeExceeded = currentReport.Dictionary.Resources.ToList().Any(r => r.Content.Length > plan.MaxResourceSize);
                                    if (maxResourceSizeExceeded)
                                    {
                                        var callbackResult = new Hashtable();
                                        callbackResult["maxResourceSizeExceeded"] = plan.MaxResourceSize;
                                        callbackResult["command"] = "OpenReport";
                                        result = StiWebActionResult.JsonResult(requestParams, callbackResult);
                                        StiReportResponse.ResponseBuffer(result.Data, result.ContentType, requestParams.Action == StiAction.Resource, result.FileName);
                                        return;
                                    }
                                }
#endif
                                #endregion
                            }
                        }
                        catch (StiDashboardNotSupportedException e)
                        {
                            Console.Write(GetErrorMessageText(e));
                            result = StiWebActionResult.DashboardNotSupportedResult(requestParams);
                        }
                        catch (Exception e)
                        {
                            result = StiWebActionResult.ErrorResult(requestParams, GetErrorMessageText(e));
                        }
                        break;

                    case StiAction.CreateReport:
                        StiReport newReport = null;

                        if (requestParams.GetBoolean("isDashboard"))
                            newReport = GetNewDashboard(requestParams);
                        else if (requestParams.GetBoolean("isForm"))
                            newReport = GetNewForm(requestParams);
                        else
                            newReport = GetNewReport(requestParams);

                        if (currentReport != null && (requestParams.Designer.Command == StiDesignerCommand.WizardResult || requestParams.Designer.NewReportDictionary == StiNewReportDictionary.DictionaryMerge))
                            StiReportCopier.CopyReportDictionary(currentReport, newReport);

                        InvokeCreateReport(newReport);
                        break;

                    case StiAction.SaveReport:
                        InvokeSaveReport(currentReport);
                        if (!string.IsNullOrEmpty(this.SaveReportErrorString)) result = StiWebActionResult.ErrorResult(requestParams, SaveReportErrorString);
                        break;

                    case StiAction.PreviewReport:
                        var previewReport = StiReportCopier.CloneReport(currentReport);
                        InvokePreviewReport(previewReport);
                        break;

                    case StiAction.Exit:
                        InvokeExit(currentReport);
                        result = new StiWebActionResult();
                        break;

                    case StiAction.RunCommand:
                        if (requestParams.Designer.Command == StiDesignerCommand.SetLocalization)
                        {
                            this.Localization = requestParams.Localization;
                            return;
                        }
                        break;
                }

                if (!string.IsNullOrEmpty(requestParams.Designer.CurrentCultureName) && !string.IsNullOrEmpty(StiLocalization.CultureName) &&
                    requestParams.Designer.CurrentCultureName != StiLocalization.CultureName)
                {
                    StiCollectionsHelper.LoadLocalizationFile(requestParams.HttpContext, this.LocalizationDirectory, requestParams.Designer.CurrentCultureName);
                }

                if (result == null)
                {
                    // Get report from event handlers, if null - get report from cache
                    currentReport = this.report != null ? this.report : this.Report;

                    result = CommandResult(requestParams, currentReport);
                }

                bool useBrowserCache = requestParams.Action == StiAction.Resource;
                StiReportResponse.ResponseBuffer(result.Data, result.ContentType, useBrowserCache, result.FileName);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            StiRequestParams requestParams = this.RequestParams;
            this.ProcessRequest(requestParams);

            base.OnInit(e);
        }
#endif
        #endregion

        #region Commands processor

        internal static StiWebActionResult CommandResult(StiRequestParams requestParams, StiReport currentReport)
        {
            var callbackResult = new Hashtable();
            StiDesignReportHelper.ApplyParamsToReport(currentReport, requestParams);

            try
            {
                Hashtable param = requestParams.All;
                StiDesignerCommand command = requestParams.Designer.Command;
                callbackResult["command"] = command;
                if (param["callbackFunctionId"] != null) callbackResult["callbackFunctionId"] = param["callbackFunctionId"];
                if (param["commandGuid"] != null) callbackResult["commandGuid"] = param["commandGuid"];

                #region Session Completed
                if (currentReport == null && command != StiDesignerCommand.CloseReport &&
                command != StiDesignerCommand.CreateReport && command != StiDesignerCommand.CreateDashboard && command != StiDesignerCommand.CreateForm &&
                command != StiDesignerCommand.GetReportFromData && command != StiDesignerCommand.OpenReport &&
                command != StiDesignerCommand.WizardResult && command != StiDesignerCommand.LoadReportFromCloud &&
                command != StiDesignerCommand.UpdateCache && command != StiDesignerCommand.GetReportForDesigner &&
                command != StiDesignerCommand.UpdateImagesArray && command != StiDesignerCommand.Synchronization &&
                command != StiDesignerCommand.OpenWizardDashboard && command != StiDesignerCommand.OpenWizardReport &&
                command != StiDesignerCommand.GetImagesArray && command != StiDesignerCommand.PrepareReportBeforeGetData &&
                command != StiDesignerCommand.EncryptMachineName)
                {
                    param["command"] = "SessionCompleted";
                    callbackResult["command"] = "SessionCompleted";
                    callbackResult["warningMessage"] = "Your session has expired!";
                    try
                    {
                        var guid = StiGuidUtils.NewGuid();
                        var cache = requestParams.Cache.Helper.HttpContext.Cache;
                        cache.Insert(guid, new object());
                        var hasCache = cache.Get(guid) != null;
                        if (!hasCache && requestParams.Cache.Mode != StiServerCacheMode.None)
                        {
                            callbackResult["warningMessage"] = "Cache not specified!<br> Please see <a href='https://www.stimulsoft.com/en/documentation/online/programming-manual/index.html?reports_web_angular_using_web_viewer_cashing.htm'>documentation.</a>";
                        }
                    }
                    catch { };
                    return StiWebActionResult.JsonResult(requestParams, callbackResult);
                }
                #endregion

#if CLOUD
                StiCloudHelper.SetCloudPlan(currentReport, StiCloudHelper.ParseCloudPlanIdent(param["cp"]));
#endif

                #region Synchronization
                if (command == StiDesignerCommand.Synchronization)
                {
                    if (currentReport == null)
                        callbackResult["command"] = "SynchronizationError";
                    else
                        callbackResult["reportObject"] = StiReportEdit.WriteReportToJsObject(currentReport);
                }
                #endregion

                #region Update Cache
                else if (command == StiDesignerCommand.UpdateCache)
                {
                    callbackResult["command"] = "UpdateCache";

                    //update all timeouts for additional caches
                    var additionalGuids = new string[] { StiCacheHelper.GUID_Clipboard, StiCacheHelper.GUID_ComponentClone,
                        StiCacheHelper.GUID_ReportCheckers, StiCacheHelper.GUID_UndoArray, StiCacheHelper.GUID_DataTransformation };

                    foreach (var additionalGuid in additionalGuids)
                        requestParams.Cache.Helper.GetObjectInternal(requestParams, additionalGuid);
                }
                #endregion

                #region Get Report For Designer
                else if (command == StiDesignerCommand.GetReportForDesigner)
                {
                    Hashtable attachedItems = null;
#if SERVER
                    if (param["startParameters"] != null)
                    {
                        var startParameters = param["startParameters"] as Hashtable;
                        if (startParameters["itemKey"] != null)
                        {
                            var nvc = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection("appSettings");
                            var connection = new StiServerConnection(CloudServerAdress);
                            connection.Accounts.Users.LoginAnonymously(StiEncryption.Encrypt(nvc["ConnectionString"], "8pTP5X15uKADcSw7"));

                            var commandDownload = new StiReportCommands.Download
                            {
                                ItemKey = startParameters["itemKey"] as string,
                                SessionKey = connection.SessionKey
                            };

                            commandDownload = connection.RunCommand(commandDownload) as StiReportCommands.Download;

                            if (commandDownload.ResultSuccess && commandDownload.ResultResource != null && commandDownload.ResultResource.Length > 0)
                            {
                                currentReport.Load(commandDownload.ResultResource);
                            }
                        }
                        else if (startParameters["useDemoData"] != null && currentReport != null)
                        {
                            param["isDashboard"] = startParameters["isDashboard"];
                            StiWizardHelper.AddDemoDataToReport(currentReport, param, callbackResult, requestParams);
                        }
                    }

                    if (param["resourceItems"] != null)
                    {
                        AddResourcesToReport(currentReport, param["resourceItems"] as ArrayList, param["sessionKey"] as string);
                    }

                    if (param["attachedItems"] != null)
                    {
                        attachedItems = GetReportAttachedItems(param["attachedItems"] as ArrayList, param["sessionKey"] as string);

                        if (attachedItems["Data"] != null)
                        {
                            CreateDataSourcesFromAttachedItem(currentReport, attachedItems["Data"] as ArrayList);
                        }
                    }
#endif
                    if (currentReport != null && callbackResult["reportObject"] == null)
                    {
                        if (currentReport.ContainsForm)
                        {
                            callbackResult["formContent"] = currentReport.SaveToJsonString();
                            callbackResult["loadingCompleted"] = false;
                        }
                        else
                        {
                            var zoom = param["zoom"] != null ? Convert.ToDouble(param["zoom"]) : 1d;
                            StiDesignReportHelper.CheckAndCorrectDuplicatePageNames(currentReport);
                            callbackResult["reportObject"] = StiReportEdit.WriteReportToJsObject(currentReport, attachedItems, zoom);
                        }
                    }
#if CLOUD
                    if (!string.IsNullOrEmpty(currentReport?.ReportGuid))
                    {
                        var reportLimits = StiCloudReportResults.GetAndRemoveLimits(currentReport.ReportGuid);
                        if (reportLimits.MaxDataRows > 0)
                            callbackResult["cloudNotificationMaxDataRows"] = reportLimits.MaxDataRows;
                    }
#endif
                }
                #endregion

                #region Create Report
                else if (command == StiDesignerCommand.CreateReport || command == StiDesignerCommand.CreateDashboard)
                {
                    var zoom = param["zoom"] != null ? Convert.ToDouble(param["zoom"]) : 1d;
                    StiDesignerOptionsHelper.ApplyDesignerOptionsToReport(StiDesignerOptionsHelper.GetDesignerOptions(requestParams), currentReport);
                    callbackResult["reportGuid"] = requestParams.Cache.ClientGuid;
                    callbackResult["reportObject"] = StiReportEdit.WriteReportToJsObject(currentReport, zoom);
                    callbackResult["needClearAfterOldReport"] = param["needClearAfterOldReport"];
                    StiReportEdit.ClearUndoArray(requestParams);
                }
                #endregion

                #region Create Form
                else if (command == StiDesignerCommand.CreateForm)
                {                                        
                    //var zoom = param["zoom"] != null ? Convert.ToDouble(param["zoom"]) : 1d;
                    //StiDesignerOptionsHelper.ApplyDesignerOptionsToReport(StiDesignerOptionsHelper.GetDesignerOptions(requestParams), currentReport);
                    //callbackResult["reportGuid"] = requestParams.Cache.ClientGuid;
                    //callbackResult["reportObject"] = StiReportEdit.WriteReportToJsObject(currentReport, zoom);
                    //StiReportEdit.ClearUndoArray(requestParams);
                }
                #endregion

                #region Open Report
                else if (command == StiDesignerCommand.OpenReport)
                {
                    try
                    {
                        if (currentReport.ContainsForm)
                        {
                            callbackResult["formContent"] = currentReport.SaveToJsonString();
                            callbackResult["formName"] = currentReport.ReportFile;
                        }
                        else
                        {
                            currentReport.Info.ForceDesigningMode = true;
                            StiDesignerOptionsHelper.ApplyDesignerOptionsToReport(StiDesignerOptionsHelper.GetDesignerOptions(requestParams), currentReport);

                            var zoom = param["zoom"] != null ? Convert.ToDouble(param["zoom"]) : 1d;
                            StiDesignReportHelper.CheckAndCorrectDuplicatePageNames(currentReport);
                            string reportObjectStr = StiReportEdit.WriteReportToJsObject(currentReport, zoom);

                            if (reportObjectStr != null)
                            {
                                callbackResult["reportObject"] = reportObjectStr;
                                callbackResult["reportGuid"] = requestParams.Cache.ClientGuid;
                            }
                            else
                            {
                                callbackResult["errorMessage"] = "Loading report error: Json parser error!";
                            }

                            callbackResult["loadingCompleted"] = true;
                            callbackResult["fileSize"] = param["fileSize"];

                            if (requestParams.Designer.Password != null)
                                callbackResult["encryptedPassword"] = requestParams.Designer.Password;

#if CLOUD
                            if (!string.IsNullOrEmpty(currentReport?.ReportGuid))
                            {
                                var reportLimits = StiCloudReportResults.GetAndRemoveLimits(currentReport.ReportGuid);
                                if (reportLimits.MaxDataRows > 0)
                                    callbackResult["cloudNotificationMaxDataRows"] = reportLimits.MaxDataRows;
                            }
#endif
                        }
                    }
                    catch (Exception e)
                    {
                        callbackResult["reportGuid"] = null;
                        callbackResult["reportObject"] = null;
                        callbackResult["error"] = $"Loading report error. {GetErrorMessageText(e)}";
                    }
                    StiReportEdit.ClearUndoArray(requestParams);
                }
                #endregion

                #region Close Report
                else if (command == StiDesignerCommand.CloseReport)
                {
                    currentReport = null;
                }
                #endregion

                #region Save Report
                else if (command == StiDesignerCommand.SaveReport)
                {
                    currentReport.ReportFile = requestParams.Designer.FileName;
                }
                #endregion

                #region Save As Report
                else if (command == StiDesignerCommand.SaveAsReport)
                {
                    currentReport.ReportFile = requestParams.Designer.FileName;
                }
                #endregion

                #region Change Rect Component
                else if (command == StiDesignerCommand.MoveComponent || command == StiDesignerCommand.ResizeComponent)
                {
                    if (!Convert.ToBoolean(param["moveAfterCopyPaste"]))
                        StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    
                    StiReportEdit.ChangeRectComponent(currentReport, param, callbackResult);
                }
                #endregion

                #region Create Component
                else if (command == StiDesignerCommand.CreateComponent)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportEdit.CreateComponent(currentReport, param, callbackResult);
                }
                #endregion

                #region Remove Component
                else if (command == StiDesignerCommand.RemoveComponent)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportEdit.RemoveComponent(currentReport, param, callbackResult);
                }
                #endregion

                #region Add Page
                else if (command == StiDesignerCommand.AddPage)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportEdit.AddPage(currentReport, param, callbackResult);
                }
                #endregion

                #region Add Dashboard
                else if (command == StiDesignerCommand.AddDashboard)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDashboardHelper.AddDashboard(currentReport, param, callbackResult);
                }
                #endregion

                #region Remove Page
                else if (command == StiDesignerCommand.RemovePage)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportEdit.RemovePage(currentReport, param, callbackResult);
                }
                #endregion

                #region Set Properties
                else if (command == StiDesignerCommand.SendProperties)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportEdit.ReadAllPropertiesFromString(currentReport, param, callbackResult);
                }
                #endregion

                #region ChangeUnit
                else if (command == StiDesignerCommand.ChangeUnit)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportEdit.ChangeUnit(currentReport, (string)param["reportUnit"]);
                    callbackResult["reportUnit"] = param["reportUnit"];
                    callbackResult["gridSize"] = currentReport.Info.GridSize.ToString();
                    StiReportEdit.GetAllComponentsPositions(currentReport, callbackResult);
                }
                #endregion

                #region Rebuild Page
                else if (command == StiDesignerCommand.RebuildPage)
                {
                    callbackResult["pageName"] = (string)param["pageName"];
                    callbackResult["rebuildProps"] = StiReportEdit.GetPropsRebuildPage(currentReport, currentReport.Pages[(string)param["pageName"]]);
                }
                #endregion

                #region LoadReportToViewer
                else if (command == StiDesignerCommand.LoadReportToViewer)
                {
                    // Clear design mode
                    requestParams.Report.Info.ForceDesigningMode = false;
                    requestParams.Report.Designer = null;

                    if (requestParams.Designer.CheckReportBeforePreview)
                    {
                        try
                        {
                            List<StiCheck> checks = StiReportCheckHelper.CheckReport(requestParams, requestParams.Report, false);
                            callbackResult["checkItems"] = StiReportCheckHelper.GetChecksJSCollection(checks);
                        }
                        catch (Exception e)
                        {
                            Console.Write(GetErrorMessageText(e));
                        }
#if CLOUD
                        StiCloudHelper.ClearCloudLimits(requestParams.Report);
                        requestParams.Report.CalculationMode = StiCalculationMode.Interpretation;
#endif
                    }

                    string viewerReportGuid = string.Format("{0}_{1}", requestParams.Id + "Viewer", param["viewerClientGuid"]);
                    requestParams.Cache.Helper.SavePreviewReportInternal(requestParams, requestParams.Report, viewerReportGuid);
                    return StiWebActionResult.JsonResult(requestParams, callbackResult);
                }
                #endregion

                #region SetToClipboard
                else if (command == StiDesignerCommand.SetToClipboard)
                {
                    StiReportEdit.SetToClipboard(requestParams, currentReport, param, callbackResult);
                }
                #endregion

                #region GetFromClipboard
                else if (command == StiDesignerCommand.GetFromClipboard)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportEdit.GetFromClipboard(requestParams, currentReport, param, callbackResult);
                }
                #endregion

                #region Undo
                else if (command == StiDesignerCommand.Undo)
                {
                    currentReport = StiReportEdit.GetUndoStep(requestParams, currentReport, param, callbackResult);
                }
                #endregion

                #region Redo
                else if (command == StiDesignerCommand.Redo)
                {
                    currentReport = StiReportEdit.GetRedoStep(requestParams, currentReport, param, callbackResult);
                }
                #endregion

                #region RenameComponent
                else if (command == StiDesignerCommand.RenameComponent)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportEdit.RenameComponent(currentReport, param, callbackResult);
                }
                #endregion

                #region WizardResult
                else if (command == StiDesignerCommand.WizardResult)
                {
                    Hashtable wizardResult = (Hashtable)param["wizardResult"];
                    currentReport = StiWizardHelper.GetReportFromWizardOptions(currentReport, wizardResult["reportOptions"] as Hashtable, wizardResult["dataSources"] as Hashtable, requestParams);
                    StiDesignerOptionsHelper.ApplyDesignerOptionsToReport(StiDesignerOptionsHelper.GetDesignerOptions(requestParams), currentReport);
                    callbackResult["reportGuid"] = requestParams.Cache.ClientGuid;
                    Hashtable attachedItems = null;
#if SERVER
                    if (param["attachedItems"] != null)
                    {
                        attachedItems = GetReportAttachedItems(param["attachedItems"] as ArrayList, param["sessionKey"] as string);
                    }
#endif
                    var zoom = param["zoom"] != null ? Convert.ToDouble(param["zoom"]) : 1d;
                    callbackResult["reportObject"] = StiReportEdit.WriteReportToJsObject(currentReport, attachedItems, zoom);
                    StiReportEdit.ClearUndoArray(requestParams);
                }
                #endregion

                #region GetConnectionTypes
                else if (command == StiDesignerCommand.GetConnectionTypes)
                {
                    StiDictionaryHelper.GetConnectionTypes(currentReport, param, callbackResult);
                }
                #endregion

                #region CreateOrEditConnection
                else if (command == StiDesignerCommand.CreateOrEditConnection)
                {
                    if (StiDashboardAssembly.IsAssemblyLoaded)
                        StiCacheCleaner.Clean(currentReport);

                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.CreateOrEditConnection(currentReport, param, callbackResult);
                }
                #endregion

                #region DeleteConnection
                else if (command == StiDesignerCommand.DeleteConnection)
                {
                    if (StiDashboardAssembly.IsAssemblyLoaded)
                        StiCacheCleaner.Clean(currentReport);

                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.DeleteConnection(currentReport, param, callbackResult);
                }
                #endregion

                #region CreateOrEditRelation
                else if (command == StiDesignerCommand.CreateOrEditRelation)
                {
                    if (StiDashboardAssembly.IsAssemblyLoaded)
                        StiCacheCleaner.Clean(currentReport);

                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.CreateOrEditRelation(currentReport, param, callbackResult);
                }
                #endregion

                #region DeleteRelation
                else if (command == StiDesignerCommand.DeleteRelation)
                {
                    if (StiDashboardAssembly.IsAssemblyLoaded)
                        StiCacheCleaner.Clean(currentReport);

                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.DeleteRelation(currentReport, param, callbackResult);
                }
                #endregion

                #region CreateOrEditColumn
                else if (command == StiDesignerCommand.CreateOrEditColumn)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.CreateOrEditColumn(currentReport, param, callbackResult);
                }
                #endregion

                #region DeleteColumn
                else if (command == StiDesignerCommand.DeleteColumn)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.DeleteColumn(currentReport, param, callbackResult);
                }
                #endregion

                #region CreateOrEditParameter
                else if (command == StiDesignerCommand.CreateOrEditParameter)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.CreateOrEditParameter(currentReport, param, callbackResult);
                }
                #endregion

                #region DeleteParameter
                else if (command == StiDesignerCommand.DeleteParameter)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.DeleteParameter(currentReport, param, callbackResult);
                }
                #endregion

                #region CreateOrEditDataSource
                else if (command == StiDesignerCommand.CreateOrEditDataSource)
                {
#if CLOUD
                    StiCloudHelper.ClearCloudLimits(currentReport);
#endif
                    if (StiDashboardAssembly.IsAssemblyLoaded)
                        StiCacheCleaner.Clean(currentReport);

                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.CreateOrEditDataSource(currentReport, param, callbackResult);
                }
                #endregion

                #region DeleteDataSource
                else if (command == StiDesignerCommand.DeleteDataSource)
                {
                    if (StiDashboardAssembly.IsAssemblyLoaded)
                        StiCacheCleaner.Clean(currentReport);

                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.DeleteDataSource(currentReport, param, callbackResult);
                }
                #endregion

                #region CreateOrEditBusinessObject
                else if (command == StiDesignerCommand.CreateOrEditBusinessObject)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.CreateOrEditBusinessObject(currentReport, param, callbackResult);
                }
                #endregion

                #region DeleteBusinessObject
                else if (command == StiDesignerCommand.DeleteBusinessObject)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.DeleteBusinessObject(currentReport, param, callbackResult);
                }
                #endregion

                #region DeleteBusinessObjectCategory
                else if (command == StiDesignerCommand.DeleteBusinessObjectCategory)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.DeleteBusinessObjectCategory(currentReport, param, callbackResult);
                }
                #endregion

                #region EditBusinessObjectCategory
                else if (command == StiDesignerCommand.EditBusinessObjectCategory)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.EditBusinessObjectCategory(currentReport, param, callbackResult);
                }
                #endregion

                #region CreateOrEditVariable
                else if (command == StiDesignerCommand.CreateOrEditVariable)
                {
                    if (StiDashboardAssembly.IsAssemblyLoaded)
                        StiCacheCleaner.Clean(currentReport);

                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.CreateOrEditVariable(currentReport, param, callbackResult);
                }
                #endregion

                #region DeleteVariable
                else if (command == StiDesignerCommand.DeleteVariable)
                {
                    if (StiDashboardAssembly.IsAssemblyLoaded)
                        StiCacheCleaner.Clean(currentReport);

                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.DeleteVariable(currentReport, param, callbackResult);
                }
                #endregion

                #region DeleteVariablesCategory
                else if (command == StiDesignerCommand.DeleteVariablesCategory)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.DeleteVariablesCategory(currentReport, param, callbackResult);
                }
                #endregion

                #region EditVariablesCategory
                else if (command == StiDesignerCommand.EditVariablesCategory)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.EditVariablesCategory(currentReport, param, callbackResult);
                }
                #endregion

                #region CreateVariablesCategory
                else if (command == StiDesignerCommand.CreateVariablesCategory)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.CreateVariablesCategory(currentReport, param, callbackResult);
                }
                #endregion

                #region CreateOrEditResource
                else if (command == StiDesignerCommand.CreateOrEditResource)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport, true);
                    StiDictionaryHelper.CreateOrEditResource(currentReport, param, callbackResult, requestParams);
                }
                #endregion

                #region DeleteResource
                else if (command == StiDesignerCommand.DeleteResource)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport, true);
                    StiDictionaryHelper.DeleteResource(currentReport, param, callbackResult);
                }
                #endregion

                #region SynchronizeDictionary
                else if (command == StiDesignerCommand.SynchronizeDictionary)
                {
                    if (StiDashboardAssembly.IsAssemblyLoaded)
                        StiCacheCleaner.Clean(currentReport);

                    StiDictionaryHelper.SynchronizeDictionary(currentReport, param, callbackResult);
                }
                #endregion

                #region NewDictionary
                else if (command == StiDesignerCommand.NewDictionary)
                {
                    if (StiDashboardAssembly.IsAssemblyLoaded)
                        StiCacheCleaner.Clean(currentReport);

                    StiDictionaryHelper.NewDictionary(currentReport, param, callbackResult);
                }
                #endregion

                #region GetAllConnections
                else if (command == StiDesignerCommand.GetAllConnections)
                {
#if CLOUD
                    StiCloudHelper.ClearCloudLimits(currentReport);
#endif
                    StiDictionaryHelper.GetAllConnections(currentReport, param, callbackResult);
                }
                #endregion

                #region RetrieveColumns
                else if (command == StiDesignerCommand.RetrieveColumns)
                {
#if CLOUD
                    StiCloudHelper.ClearCloudLimits(currentReport);
#endif
                    StiDictionaryHelper.RetrieveColumns(currentReport, param, callbackResult);
                }
                #endregion

                #region UpdateStyles
                else if (command == StiDesignerCommand.UpdateStyles)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiStylesHelper.UpdateStyles(currentReport, param, callbackResult);
                }
                #endregion

                #region AddStyle
                else if (command == StiDesignerCommand.AddStyle)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiStylesHelper.AddStyle(currentReport, param, callbackResult);
                }
                #endregion

                #region CreateStyleCollection
                else if (command == StiDesignerCommand.CreateStyleCollection)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiStylesHelper.CreateStyleCollection(currentReport, param, callbackResult);
                }
                #endregion

                #region CloneChartComponent
                else if (command == StiDesignerCommand.StartEditChartComponent)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null)
                    {
                        StiReportEdit.SaveComponentClone(requestParams, component);
                        callbackResult["properties"] = StiChartHelper.GetChartProperties(component as StiChart);
                        callbackResult["formName"] = param["formName"];
                    }
                }
                #endregion

                #region CloneChartElement
                else if (command == StiDesignerCommand.StartEditChartElement)
                {
                    var chartElement = currentReport.Pages.GetComponentByName((string)param["componentName"]) as IStiChartElement;
                    if (chartElement != null)
                    {
                        StiReportEdit.SaveComponentClone(requestParams, chartElement as StiComponent);
                        callbackResult["properties"] = StiReportEdit.GetAllProperties(chartElement as StiComponent);
                        callbackResult["svgContent"] = StiEncodingHelper.Encode(StiDashboardsSvgHelper.SaveElementToString(chartElement, 1, 1, true));
                        callbackResult["formName"] = param["formName"];
                    }
                }
                #endregion

                #region CloneMapComponent
                else if (command == StiDesignerCommand.StartEditMapComponent)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null)
                    {
                        StiReportEdit.SaveComponentClone(requestParams, component);
                        callbackResult["properties"] = StiMapHelper.GetMapProperties(component as StiMap);
                        callbackResult["svgContent"] = StiReportEdit.GetSvgContent(component);
                        callbackResult["additionalParams"] = param["additionalParams"];
                    }
                }
                #endregion

                #region CanceledEditComponent
                else if (command == StiDesignerCommand.CanceledEditComponent)
                {
                    StiReportEdit.CanceledEditComponent(requestParams, currentReport, param, callbackResult);
                }
                #endregion

                #region Add Series
                else if (command == StiDesignerCommand.AddSeries)
                {
                    StiChartHelper.AddSeries(currentReport, param, callbackResult);
                }
                #endregion

                #region Remove Series
                else if (command == StiDesignerCommand.RemoveSeries)
                {
                    StiChartHelper.RemoveSeries(currentReport, param, callbackResult);
                }
                #endregion

                #region Series Move
                else if (command == StiDesignerCommand.SeriesMove)
                {
                    StiChartHelper.SeriesMove(currentReport, param, callbackResult);
                }
                #endregion

                #region Add ConstantLineOrStrip
                else if (command == StiDesignerCommand.AddConstantLineOrStrip)
                {
                    StiChartHelper.AddConstantLineOrStrip(currentReport, param, callbackResult);
                }
                #endregion

                #region Remove ConstantLineOrStrip
                else if (command == StiDesignerCommand.RemoveConstantLineOrStrip)
                {
                    StiChartHelper.RemoveConstantLineOrStrip(currentReport, param, callbackResult);
                }
                #endregion

                #region ConstantLineOrStrip Move
                else if (command == StiDesignerCommand.ConstantLineOrStripMove)
                {
                    StiChartHelper.ConstantLineOrStripMove(currentReport, param, callbackResult);
                }
                #endregion

                #region Get Labels Content
                else if (command == StiDesignerCommand.GetLabelsContent)
                {
                    StiChartHelper.GetLabelsContent(currentReport, param, callbackResult);
                }
                #endregion

                #region Get Styles Content
                else if (command == StiDesignerCommand.GetStylesContent)
                {
                    StiChartHelper.GetStylesContent(currentReport, param, callbackResult, false);
                }
                #endregion

                #region Set Labels Type
                else if (command == StiDesignerCommand.SetLabelsType)
                {
                    StiChartHelper.SetLabelsType(currentReport, param, callbackResult);
                }
                #endregion

                #region Set Chart Style
                else if (command == StiDesignerCommand.SetChartStyle)
                {
                    StiChartHelper.SetChartStyle(currentReport, param, callbackResult);
                }
                #endregion

                #region Set Chart Property Value
                else if (command == StiDesignerCommand.SetChartPropertyValue)
                {
                    StiChartHelper.SetChartPropertyValue(currentReport, param, callbackResult);
                }
                #endregion

                #region Send Container Value
                else if (command == StiDesignerCommand.SendContainerValue)
                {
                    StiChartHelper.SetContainerValue(currentReport, param, callbackResult);
                }
                #endregion

                #region ItemResourceSave
                else if (command == StiDesignerCommand.ItemResourceSave)
                {
                    if (currentReport != null)
                    {
#if SERVER
                        try
                        {
                            StiServerConnection connection = null;
                            var cloudMode = Convert.ToBoolean(param["cloudMode"]);

                            if (cloudMode)
                            {
                                connection = new StiServerConnection(CloudServerAdress);
                                connection.Accounts.Users.Login(param["sessionKey"] as string, false);
                            }

                            var resourceBytes = currentReport.SaveToByteArray();

                            if (param["formContent"] != null) {
                                resourceBytes = Convert.FromBase64String(param["formContent"] as string);
                            }

                            var itemResourceSaveCommand = new StiItemResourceCommands.Save()
                            {
                                ItemKey = (string)param["reportTemplateItemKey"],
                                SessionKey = (string)param["sessionKey"],
                                Resource = resourceBytes
                            };

                            string customMessage = param["customMessage"] as string;
                            if (!string.IsNullOrEmpty(customMessage))
                            {
                                itemResourceSaveCommand.VersionInfo = StiNotice.Create(StiEncodingHelper.DecodeString(customMessage));
                            }

                            var resultResourceSaveCommand = new StiItemResourceCommands.Save();

                            if (cloudMode)
                            {
                                resultResourceSaveCommand = connection.RunCommand(itemResourceSaveCommand) as StiItemResourceCommands.Save;
                            }
                            else
                            {
                                resultResourceSaveCommand = StiCommandToServer.RunCommand(itemResourceSaveCommand) as StiItemResourceCommands.Save;
                            }

                            if (resultResourceSaveCommand.ResultSuccess)
                            {
                                var identifyContentTypeCommand = new StiItemCommands.IdentifyContentType()
                                {
                                    ItemKeys = new List<string>() { itemResourceSaveCommand.ItemKey }
                                };

                                if (cloudMode)
                                {
                                    connection.RunCommand(identifyContentTypeCommand);
                                }
                                else
                                {
                                    StiCommandToServer.RunCommand(identifyContentTypeCommand);
                                }
                            }
                            else if (resultResourceSaveCommand.ResultNotice != null)
                            {
                                callbackResult["errorMessage"] = StiNoticeConverter.Convert(resultResourceSaveCommand.ResultNotice);
                            }
                        }
                        catch (StiServerException e)
                        {
                            string msg = e.Notice != null ? StiNoticeConverter.Convert(e.Notice) : GetErrorMessageText(e);
                            callbackResult["errorMessage"] = msg;

                            if (e.Notice != null && e.Notice.Ident == StiNoticeIdent.IsNotRecognized && e.Notice.Arguments[0] == "Item")
                            {
                                callbackResult["openSaveAsDialog"] = true;
                            }
                        }
#endif

                    }
                }
                #endregion

                #region CloneItemResourceSave
                else if (command == StiDesignerCommand.CloneItemResourceSave)
                {
                    if (currentReport != null)
                    {
#if SERVER
                        var reportTemplateKey = StiKeyHelper.GenerateKey();

                        var tempItem = new StiReportTemplateItem();
                        tempItem.GenerateNewStateKey();
                        tempItem.Visible = false;
                        tempItem.Expires = DateTime.UtcNow + StiClientConfiguration.Reports.ReportViewInDesignerLifeTime;
                        tempItem.Key = reportTemplateKey;
                        tempItem.Name = !string.IsNullOrEmpty(param["reportName"] as string) ? param["reportName"] as string : "Report";
                        if (param["attachedItems"] != null) tempItem.AttachedItems = (param["attachedItems"] as ArrayList).Cast<string>().ToList();

                        var itemSaveCommand = new StiItemCommands.Save()
                        {
                            Items = tempItem.ToItems(),
                            SessionKey = (string)param["sessionKey"]
                        };

                        var resultItemSaveCommand = new StiItemCommands.Save();

                        resultItemSaveCommand = StiCommandToServer.RunCommand(itemSaveCommand) as StiItemCommands.Save;

                        if (resultItemSaveCommand.ResultSuccess)
                        {
                            var resultResourceSaveCommand = new StiItemResourceCommands.Save()
                            {
                                ItemKey = reportTemplateKey,
                                SessionKey = (string)param["sessionKey"],
                                Resource = currentReport.SaveToByteArray()
                            };

                            resultResourceSaveCommand = StiCommandToServer.RunCommand(resultResourceSaveCommand) as StiItemResourceCommands.Save;

                            if (resultResourceSaveCommand.ResultSuccess)
                            {
                                callbackResult["resultItemKey"] = reportTemplateKey;
                                callbackResult["reportName"] = param["reportName"];
                            }
                            else if (resultResourceSaveCommand.ResultNotice != null)
                            {
                                callbackResult["errorMessage"] = resultResourceSaveCommand.ResultNotice.CustomMessage;
                            }
                        }
                        else if (resultItemSaveCommand.ResultNotice != null)
                        {
                            callbackResult["errorMessage"] =
                                resultItemSaveCommand.ResultNotice.CustomMessage != null ? resultItemSaveCommand.ResultNotice.CustomMessage : resultItemSaveCommand.ResultNotice.Ident.ToString();
                        }
#endif
                    }
                }
                #endregion

                #region GetDatabaseData
                else if (command == StiDesignerCommand.GetDatabaseData)
                {
                    StiDictionaryHelper.GetDatabaseData(currentReport, param, callbackResult);
                }
                #endregion

                #region ApplySelectedData
                else if (command == StiDesignerCommand.ApplySelectedData)
                {
                    if (StiDashboardAssembly.IsAssemblyLoaded)
                        StiCacheCleaner.Clean(currentReport);

                    StiDictionaryHelper.ApplySelectedData(currentReport, param, callbackResult);
                }
                #endregion

                #region CreateTextComponent
                else if (command == StiDesignerCommand.CreateTextComponent)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportEdit.CreateTextComponentFromDictionary(currentReport, param, callbackResult);
                }
                #endregion

                #region CreateDataComponent
                else if (command == StiDesignerCommand.CreateDataComponent)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportEdit.CreateDataComponentFromDictionary(currentReport, param, callbackResult);
                }
                #endregion

                #region Set Report Properties
                else if (command == StiDesignerCommand.SetReportProperties)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportEdit.SetReportProperties(currentReport, param, callbackResult);
                }
                #endregion

                #region Page Move
                else if (command == StiDesignerCommand.PageMove)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportEdit.PageMove(currentReport, param, callbackResult);
                }
                #endregion

                #region Test Connection
                else if (command == StiDesignerCommand.TestConnection)
                {
                    StiDictionaryHelper.TestConnection(currentReport, param, callbackResult);
                }
                #endregion

                #region Run Query Script
                else if (command == StiDesignerCommand.RunQueryScript)
                {
#if CLOUD
                    StiCloudHelper.ClearCloudLimits(currentReport);
#endif
                    StiDictionaryHelper.RunQueryScript(currentReport, param, callbackResult);
                }
                #endregion

                #region View Data
                else if (command == StiDesignerCommand.ViewData)
                {
#if CLOUD
                    StiCloudHelper.ClearCloudLimits(currentReport);
#endif
                    StiDictionaryHelper.ViewData(currentReport, param, callbackResult);
                }
                #endregion

                #region Apply Designer Options
                else if (command == StiDesignerCommand.ApplyDesignerOptions)
                {
                    var designerOptions = (Hashtable)param["designerOptions"];
                    currentReport.Info.Zoom = StiReportEdit.StrToDouble((string)param["zoom"]);
                    StiDesignerOptionsHelper.ApplyDesignerOptionsToReport(designerOptions, currentReport);
                    if (param["reportFile"] != null) currentReport.ReportFile = (string)param["reportFile"];
                    callbackResult["reportObject"] = StiReportEdit.WriteReportToJsObject(currentReport);
                    callbackResult["selectedObjectName"] = param["selectedObjectName"];
                }
                #endregion

                #region GetSqlParameterTypes
                else if (command == StiDesignerCommand.GetSqlParameterTypes)
                {
                    StiDictionaryHelper.GetSqlParameterTypes(currentReport, param, callbackResult);
                }
                #endregion

                #region Align To Grid
                else if (command == StiDesignerCommand.AlignToGridComponents)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportEdit.AlignToGridComponents(currentReport, param, callbackResult);
                }
                #endregion

                #region Change Arrange
                else if (command == StiDesignerCommand.ChangeArrangeComponents)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportEdit.ChangeArrangeComponents(currentReport, param, callbackResult);
                }
                #endregion

                #region Update Sample Text Format
                else if (command == StiDesignerCommand.UpdateSampleTextFormat)
                {
                    StiTextFormatHelper.UpdateSampleTextFormat(currentReport, param, callbackResult);
                }
                #endregion

                #region CloneCrossTabComponent
                else if (command == StiDesignerCommand.StartEditCrossTabComponent)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null)
                    {
                        StiReportEdit.SaveComponentClone(requestParams, component);
                    }
                }
                #endregion

                #region UpdateCrossTabComponent
                else if (command == StiDesignerCommand.UpdateCrossTabComponent)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null && component is StiCrossTab)
                    {
                        StiCrossTabHelper crossTabHelper = new StiCrossTabHelper(component as StiCrossTab);
                        crossTabHelper.ExecuteJSCommand(param["updateParameters"] as Hashtable, callbackResult);
                        crossTabHelper.RestorePositions();
                        crossTabHelper = null;
                    }
                }
                #endregion

                #region GetCrossTabColorStyles
                else if (command == StiDesignerCommand.GetCrossTabColorStyles)
                {
                    callbackResult["colorStyles"] = StiCrossTabHelper.GetColorStyles();
                }
                #endregion

                #region DuplicatePage
                else if (command == StiDesignerCommand.DuplicatePage)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportEdit.DuplicatePage(currentReport, param, callbackResult);
                }
                #endregion

                #region Set Event Value
                else if (command == StiDesignerCommand.SetEventValue)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportEdit.SetEventValue(currentReport, param, callbackResult);
                }
                #endregion

                #region Get Chart Styles Content
                else if (command == StiDesignerCommand.GetChartStylesContent)
                {
                    StiChartHelper.GetStylesContent(currentReport, param, callbackResult, true);
                }
                #endregion

                #region Get Gauge Styles Content
                else if (command == StiDesignerCommand.GetGaugeStylesContent)
                {
                    StiGaugeHelper.GetStylesContent(currentReport, param, callbackResult, true);
                }
                #endregion

                #region Get Map Styles Content
                else if (command == StiDesignerCommand.GetMapStylesContent)
                {
                    StiMapHelper.GetStylesContent(currentReport, param, callbackResult);
                }
                #endregion

                #region Get CrossTab Styles Content
                else if (command == StiDesignerCommand.GetCrossTabStylesContent)
                {
                    callbackResult["stylesContent"] = StiCrossTabHelper.GetColorStyles();
                }
                #endregion

                #region Get Table Styles Content
                else if (command == StiDesignerCommand.GetTableStylesContent)
                {
                    callbackResult["stylesContent"] = StiTableHelper.GetTableStyles(currentReport);
                }
                #endregion

                #region Get Sparkline Styles Content
                else if (command == StiDesignerCommand.GetSparklineStylesContent)
                {
                    StiSparklineHelper.GetStylesContent(currentReport, param, callbackResult);
                }
                #endregion

                #region Get Dashboard Styles Content
                else if (command == StiDesignerCommand.GetDashboardStylesContent)
                {
                    callbackResult["stylesContent"] = StiDashboardHelper.GetDashboardStyles(currentReport, param, callbackResult);
                }
                #endregion

                #region Change Table Component
                else if (command == StiDesignerCommand.ChangeTableComponent)
                {
                    var table = currentReport.Pages.GetComponentByName((string)param["tableName"]);
                    if (table != null && table is StiTable)
                    {
                        StiTableHelper tableHelper = new StiTableHelper(table as StiTable, StiReportEdit.StrToDouble((string)param["zoom"]));
                        tableHelper.ExecuteJSCommand(param["changeParameters"] as Hashtable, callbackResult);
                        tableHelper = null;
                    }
                }
                #endregion

                #region Get Images Array
                else if (command == StiDesignerCommand.GetImagesArray)
                {
                    var imagesScalingFactor = param["imagesScalingFactor"] != null ? StiReportEdit.StrToDouble(param["imagesScalingFactor"] as string) : 1;
                    callbackResult["images"] = StiDesignerResourcesHelper.GetImagesArray(requestParams, param["imagesUrl"] as string, Math.Round(imagesScalingFactor, 2));
                    return StiWebActionResult.JsonResult(requestParams, callbackResult);
                }
                #endregion

                #region Update Images Array
                else if (command == StiDesignerCommand.UpdateImagesArray)
                {
                    var imagesScalingFactor = param["imagesScalingFactor"] != null ? StiReportEdit.StrToDouble(param["imagesScalingFactor"] as string) : 1;
                    callbackResult["images"] = StiDesignerResourcesHelper.GetImagesArray(requestParams, null, Math.Round(imagesScalingFactor, 2), true);
                    return StiWebActionResult.JsonResult(requestParams, callbackResult);
                }
                #endregion

                #region Open Style
                else if (command == StiDesignerCommand.OpenStyle)
                {
                    StiStylesHelper.OpenStyle(requestParams, currentReport, callbackResult);
                }
                #endregion

                #region Get Style From Component
                else if (command == StiDesignerCommand.CreateStylesFromComponents)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiStylesHelper.CreateStylesFromComponents(currentReport, param, callbackResult);
                }
                #endregion

                #region Change Size Components
                else if (command == StiDesignerCommand.ChangeSizeComponents)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportEdit.ChangeSizeComponents(currentReport, param, callbackResult);
                }
                #endregion

                #region CreateFieldOnDblClick
                else if (command == StiDesignerCommand.CreateFieldOnDblClick)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.CreateFieldOnDblClick(currentReport, param, callbackResult);
                }
                #endregion

                #region Get Params From Query String
                else if (command == StiDesignerCommand.GetParamsFromQueryString)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.GetParamsFromQueryString(currentReport, param, callbackResult);
                }
                #endregion

                #region Create Moving Copy Component
                else if (command == StiDesignerCommand.CreateMovingCopyComponent)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportEdit.CreateMovingCopyComponent(requestParams, currentReport, param, callbackResult);
                }
                #endregion

                #region Get Report Check Items
                else if (command == StiDesignerCommand.GetReportCheckItems)
                {
                    List<StiCheck> checks = StiReportCheckHelper.CheckReport(requestParams, currentReport, true);
                    callbackResult["checkItems"] = StiReportCheckHelper.GetChecksJSCollection(checks);
                }
                #endregion

                #region Get Check Preview
                else if (command == StiDesignerCommand.GetCheckPreview)
                {
                    StiReportCheckHelper.GetCheckPreview(requestParams, currentReport, param, callbackResult);
                }
                #endregion

                #region Action Check
                else if (command == StiDesignerCommand.ActionCheck)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportCheckHelper.ActionCheck(requestParams, currentReport, param, callbackResult);
                }
                #endregion

                #region Check Expression
                else if (command == StiDesignerCommand.CheckExpression)
                {
                    StiReportCheckHelper.CheckExpression(currentReport, param, callbackResult);
                }
                #endregion

                #region Get Globalization Strings
                else if (command == StiDesignerCommand.GetGlobalizationStrings)
                {
                    callbackResult["globalizationStrings"] = StiCultureHelper.GetReportGlobalizationStrings(currentReport);
                    callbackResult["cultures"] = StiCultureHelper.GetItems(CultureTypes.SpecificCultures);
                }
                #endregion

                #region Get Wizard Data
                else if (command == StiDesignerCommand.GetWizardData)
                {
                    callbackResult["wizardData"] = StiWizardHelper.GetWizardData(param, requestParams);
                }
                #endregion

                #region Add Globalization Strings
                else if (command == StiDesignerCommand.AddGlobalizationStrings)
                {
                    StiCultureHelper.AddReportGlobalizationStrings(currentReport, param, callbackResult);
                }
                #endregion

                #region Edit Globalization Strings
                else if (command == StiDesignerCommand.EditGlobalizationStrings)
                {
                    StiCultureHelper.EditGlobalizationStrings(currentReport, param, callbackResult);
                }
                #endregion

                #region Remove Globalization Strings
                else if (command == StiDesignerCommand.RemoveGlobalizationStrings)
                {
                    StiCultureHelper.RemoveReportGlobalizationStrings(currentReport, param, callbackResult);
                }
                #endregion

                #region Get Culture Settings From Report
                else if (command == StiDesignerCommand.GetCultureSettingsFromReport)
                {
                    StiCultureHelper.GetCultureSettingsFromReport(currentReport, param, callbackResult);
                }
                #endregion

                #region Set Culture Settings To Report
                else if (command == StiDesignerCommand.SetCultureSettingsToReport)
                {
                    StiCultureHelper.SetCultureSettingsToReport(currentReport, param, callbackResult);
                }
                #endregion

                #region Apply Globalization Strings
                else if (command == StiDesignerCommand.ApplyGlobalizationStrings)
                {
                    StiCultureHelper.ApplyGlobalizationStrings(currentReport, param, callbackResult);
                }
                #endregion

                #region Remove Unlocalized Globalization Strings
                else if (command == StiDesignerCommand.RemoveUnlocalizedGlobalizationStrings)
                {
                    StiCultureHelper.RemoveUnlocalizedGlobalizationStrings(currentReport, param, callbackResult);
                }
                #endregion

                #region CloneGaugeComponent
                else if (command == StiDesignerCommand.StartEditGaugeComponent)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null)
                    {
                        StiReportEdit.SaveComponentClone(requestParams, component);
                        callbackResult["properties"] = StiGaugeHelper.GetGaugeProperties(component as StiGauge);
                        callbackResult["svgContent"] = StiReportEdit.GetSvgContent(component);
                    }
                }
                #endregion

                #region CloneSparklineComponent
                else if (command == StiDesignerCommand.StartEditSparklineComponent)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null)
                    {
                        StiReportEdit.SaveComponentClone(requestParams, component);
                        callbackResult["properties"] = StiSparklineHelper.GetSparklineProperties(component as StiSparkline);
                        callbackResult["svgContent"] = StiReportEdit.GetSvgContent(component);
                    }
                }
                #endregion

                #region Get Resource Content
                else if (command == StiDesignerCommand.GetResourceContent)
                {
                    StiReportResourcesHelper.GetResourceContent(currentReport, param, callbackResult);
                }
                #endregion

                #region Convert Resource Content
                else if (command == StiDesignerCommand.ConvertResourceContent)
                {
                    StiResourceType type = (StiResourceType)Enum.Parse(typeof(StiResourceType), param["type"] as string);
                    callbackResult["content"] = StiReportResourcesHelper.GetStringContentForJSFromResourceContent(type, requestParams.Data);
                }
                #endregion

                #region Get Rtf Resource Content
                else if (command == StiDesignerCommand.GetRtfResourceContentFromHtmlText)
                {
                    callbackResult["resourceContent"] = StiReportResourcesHelper.GetRtfResourceContentFromHtmlText(param);
                }
                #endregion

                #region Get Resource Text
                else if (command == StiDesignerCommand.GetResourceText)
                {
                    StiReportResourcesHelper.GetResourceText(currentReport, param, callbackResult);
                }
                #endregion

                #region Set Resource Text
                else if (command == StiDesignerCommand.SetResourceText)
                {
                    StiReportResourcesHelper.SetResourceText(currentReport, param, callbackResult);
                }
                #endregion

                #region Get Resource View Data
                else if (command == StiDesignerCommand.GetResourceViewData)
                {
                    StiReportResourcesHelper.GetResourceViewData(currentReport, param, callbackResult);
                }
                #endregion

                #region Get Images Gallery
                else if (command == StiDesignerCommand.GetImagesGallery)
                {
#if CLOUD
                    StiCloudHelper.ClearCloudLimits(currentReport);
#endif
                    StiDictionaryHelper.GetImagesGallery(currentReport, param, callbackResult);
                }
                #endregion

                #region CreateComponentFromResource
                else if (command == StiDesignerCommand.CreateComponentFromResource)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportEdit.CreateComponentFromResource(currentReport, param, callbackResult);
                }
                #endregion

                #region CreateElementFromResource
                else if (command == StiDesignerCommand.CreateElementFromResource)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportEdit.CreateElementFromResource(currentReport, param, callbackResult);
                }
                #endregion

                #region GetSampleConnectionString
                else if (command == StiDesignerCommand.GetSampleConnectionString)
                {
                    StiDictionaryHelper.GetSampleConnectionString(currentReport, param, callbackResult);
                }
                #endregion

                #region CreateDatabaseFromResource
                else if (command == StiDesignerCommand.CreateDatabaseFromResource)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.CreateDatabaseFromResource(currentReport, param, callbackResult, requestParams);
                }
                #endregion

                #region Get Rich Text Gallery
                else if (command == StiDesignerCommand.GetRichTextGallery)
                {
#if CLOUD
                    StiCloudHelper.ClearCloudLimits(currentReport);
#endif
                    StiDictionaryHelper.GetRichTextGallery(currentReport, param, callbackResult);
                }
                #endregion

                #region Get RichText Content
                else if (command == StiDesignerCommand.GetRichTextContent)
                {
                    callbackResult["content"] = StiGalleriesHelper.GetHtmlStringFromRichTextItem(currentReport, param["itemObject"] as Hashtable);
                }
                #endregion

                #region Delete All DataSources
                else if (command == StiDesignerCommand.DeleteAllDataSources)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.DeleteAllDataSources(currentReport, param, callbackResult);
                }
                #endregion

                #region Get Report Cloud Server Mode                
                else if (command == StiDesignerCommand.LoadReportFromCloud)
                {
#if SERVER
                    Hashtable itemObject = param["itemObject"] as Hashtable;
                    try
                    {
                        if (Convert.ToBoolean(param["cloudMode"]))
                        {
                            StiReport newReport = new StiReport();

                            var connection = new StiServerConnection(CloudServerAdress);
                            connection.Accounts.Users.Login(param["sessionKey"] as string, false);

                            byte[] resultResource = null;

                            if (Convert.ToBoolean(itemObject["isOnlineStoreItem"]))
                            {
                                var commandDownload = new StiReportCommands.Download
                                {
                                    ItemKey = itemObject["Key"] as string,
                                    SessionKey = param["sessionKey"] as string
                                };

                                commandDownload = connection.RunCommand(commandDownload) as StiReportCommands.Download;

                                if (commandDownload.ResultSuccess && commandDownload.ResultResource != null && commandDownload.ResultResource.Length > 0)
                                {
                                    resultResource = commandDownload.ResultResource;
                                }
                                else if (commandDownload.ResultNotice != null)
                                {
                                    callbackResult["errorMessage"] = StiNoticeConverter.Convert(commandDownload.ResultNotice);
                                }

                            }
                            else
                            {
                                var commandResourceGet = new StiItemResourceCommands.Get
                                {
                                    ItemKey = itemObject["Key"] as string,
                                    SessionKey = param["sessionKey"] as string
                                };

                                if (itemObject.ContainsKey("VersionKey"))
                                {
                                    commandResourceGet.VersionKey = itemObject["VersionKey"] as string;
                                }

                                commandResourceGet = connection.RunCommand(commandResourceGet) as StiItemResourceCommands.Get;

                                if (commandResourceGet.ResultSuccess && commandResourceGet.ResultResource != null && commandResourceGet.ResultResource.Length > 0)
                                {
                                    resultResource = commandResourceGet.ResultResource;
                                }
                                else if (commandResourceGet.ResultNotice != null)
                                {
                                    callbackResult["errorMessage"] = StiNoticeConverter.Convert(commandResourceGet.ResultNotice);
                                }
                            }

                            if (resultResource != null && resultResource.Length > 0)
                            {
                                try
                                {
                                    newReport.Load(resultResource);
                                    newReport.Info.Zoom = 1;
                                    StiDesignerOptionsHelper.ApplyDesignerOptionsToReport(StiDesignerOptionsHelper.GetDesignerOptions(requestParams), newReport);
                                }
                                catch (Exception e)
                                {
                                    callbackResult["error"] = GetErrorMessageText(e);
                                };
                            }

                            currentReport = newReport;

                            if (currentReport.ContainsForm)
                            {
                                callbackResult["formContent"] = currentReport.SaveToJsonString();
                            }
                            else
                            {
                                StiDesignReportHelper.CheckAndCorrectDuplicatePageNames(currentReport);
                                string reportObjectStr = StiReportEdit.WriteReportToJsObject(currentReport);

                                if (reportObjectStr != null)
                                {
                                    callbackResult["reportObject"] = reportObjectStr;
                                    callbackResult["reportGuid"] = requestParams.Cache.ClientGuid;
                                }
                            }
                        }
                        else
                        {
                            var getReportForDesignCommand = new StiReportCommands.Design()
                            {
                                ReportTemplateItemKey = itemObject["Key"] as string,
                                SessionKey = param["sessionKey"] as string
                            };

                            if (itemObject.ContainsKey("VersionKey"))
                            {
                                getReportForDesignCommand.VersionKey = itemObject["VersionKey"] as string;
                            }

                            getReportForDesignCommand = StiCommandToServer.RunCommand(getReportForDesignCommand) as StiReportCommands.Design;

                            if (getReportForDesignCommand.ResultSuccess && getReportForDesignCommand.ResultReport != null)
                            {
                                StiReport newReport = new StiReport();
                                try
                                {
                                    newReport.Load(getReportForDesignCommand.ResultReport);
                                    newReport.Info.Zoom = 1;
                                    StiDesignerOptionsHelper.ApplyDesignerOptionsToReport(StiDesignerOptionsHelper.GetDesignerOptions(requestParams), newReport);
                                }
                                catch (Exception e) { callbackResult["error"] = GetErrorMessageText(e); };

                                currentReport = newReport;

                                Hashtable attachedItems = GetReportAttachedItems(itemObject["AttachedItems"] as ArrayList, param["sessionKey"] as string);

                                if (attachedItems["Data"] != null)
                                {
                                    CreateDataSourcesFromAttachedItem(currentReport, attachedItems["Data"] as ArrayList);
                                }

                                StiDesignReportHelper.CheckAndCorrectDuplicatePageNames(currentReport);
                                string reportObjectStr = StiReportEdit.WriteReportToJsObject(currentReport, attachedItems);

                                if (reportObjectStr != null)
                                {
                                    callbackResult["reportObject"] = reportObjectStr;
                                    callbackResult["reportGuid"] = requestParams.Cache.ClientGuid;
                                }
                            }
                            else
                            {
                                if (getReportForDesignCommand.ResultNotice != null)
                                    callbackResult["errorMessage"] = getReportForDesignCommand.ResultNotice.CustomMessage;
                            }
                        }
                    }

                    catch (StiServerException e)
                    {
                        string msg = e.Notice != null ? StiNoticeConverter.Convert(e.Notice) : GetErrorMessageText(e);
                        callbackResult["errorMessage"] = msg;
                    }
#endif
                }
                #endregion

                #region CloneBarCodeComponent
                else if (command == StiDesignerCommand.StartEditBarCodeComponent)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null)
                    {
                        StiReportEdit.SaveComponentClone(requestParams, component);
                        callbackResult["barCode"] = StiBarCodeHelper.GetBarCodeJSObject(component as StiBarCode);
                    }
                }
                #endregion

                #region CloneShapeComponent
                else if (command == StiDesignerCommand.StartEditShapeComponent)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null)
                    {
                        StiReportEdit.SaveComponentClone(requestParams, component);
                        callbackResult["shape"] = StiShapeHelper.GetShapeJSObject(component as StiShape);
                    }
                }
                #endregion

                #region Apply BarCode Properties
                else if (command == StiDesignerCommand.ApplyBarCodeProperty)
                {
                    StiBarCodeHelper.ApplyBarCodeProperty(currentReport, param, callbackResult);
                }
                #endregion

                #region Apply Shape Properties
                else if (command == StiDesignerCommand.ApplyShapeProperty)
                {
                    StiShapeHelper.ApplyShapeProperty(currentReport, param, callbackResult);
                }
                #endregion

                #region Download Report
                else if (command == StiDesignerCommand.DownloadReport)
                {
                    string reportFile = !string.IsNullOrWhiteSpace(requestParams.Designer.FileName) ? requestParams.Designer.FileName : "Report.mrt";
                    string password = requestParams.Designer.Password;

                    if (currentReport != null)
                    {
                        using (var stream = new MemoryStream())
                        {
                            if (!String.IsNullOrEmpty(password))
                            {
                                currentReport.SaveEncryptedReport(stream, password);

                                if (reportFile.ToLower().EndsWith(".mrt")) reportFile = reportFile.Substring(0, reportFile.Length - 4) + ".mrx";
                                else if (!reportFile.ToLower().EndsWith(".mrx")) reportFile += ".mrx";
                            }
                            else
                            {
                                if (requestParams.Designer.SaveType == "json")
                                {
                                    currentReport.Save(new StiJsonReportSLService(), stream);
                                }
                                else if (requestParams.Designer.SaveType == "xml")
                                {
                                    currentReport.Save(new StiXmlReportSLService(), stream);
                                }
                                else
                                {
                                    currentReport.Save(stream);
                                }
                            }
                            return new StiWebActionResult(stream, "application/octet-stream", reportFile);
                        }
                    }
                }
                #endregion

                #region Download Styles
                else if (command == StiDesignerCommand.DownloadStyles)
                {
                    StiReport tempReport = new StiReport();
                    string styles = requestParams.GetString("stylesCollection");

                    if (styles != null)
                    {
                        ArrayList stylesCollection = JSON.Decode(styles) as ArrayList;
                        StiStylesHelper.WriteStylesToReport(tempReport, stylesCollection);

                        using (var stream = new MemoryStream())
                        {
                            tempReport.Styles.Save(stream);
                            return new StiWebActionResult(stream, "application/octet-stream", "Styles.sts");
                        }
                    }
                }
                #endregion

                #region Download Resource
                else if (command == StiDesignerCommand.DownloadResource)
                {
                    return StiReportResourcesHelper.DownloadResource(currentReport, param);
                }
                #endregion

                #region Download Image Content
                else if (command == StiDesignerCommand.DownloadImageContent)
                {
                    return StiReportResourcesHelper.DownloadImageContent(currentReport, param);
                }
                #endregion

                #region GetVariableItemsFromDataColumn
                else if (command == StiDesignerCommand.GetVariableItemsFromDataColumn)
                {
#if CLOUD
                    StiCloudHelper.ClearCloudLimits(currentReport);
#endif
                    StiDictionaryHelper.GetVariableItemsFromDataColumn(currentReport, param, callbackResult);
                }
                #endregion

                #region Move Dictionary Item
                else if (command == StiDesignerCommand.MoveDictionaryItem)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.MoveDictionaryItem(currentReport, param, callbackResult);
                }
                #endregion

                #region Convert MetaFile To Png
                else if (command == StiDesignerCommand.ConvertMetaFileToPng)
                {
                    callbackResult["fileContent"] = StiReportResourcesHelper.ConvertBase64MetaFileToBase64Png(param["fileContent"] as string);
                }
                #endregion

                #region GetReportString
                else if (command == StiDesignerCommand.GetReportString)
                {
                    if (currentReport != null)
                        callbackResult["reportString"] = StiEncodingHelper.Encode(currentReport.SaveToString());
                }
                #endregion

                #region UpdateReportAliases
                else if (command == StiDesignerCommand.UpdateReportAliases)
                {
                    StiReportEdit.UpdateReportAliases(requestParams, currentReport, param, callbackResult);
                }
                #endregion

                #region MoveConnectionDataToResource
                else if (command == StiDesignerCommand.MoveConnectionDataToResource)
                {
                    StiDictionaryHelper.MoveConnectionDataToResource(currentReport, param, callbackResult);
                }
                #endregion               

                #region SetMapProperties
                else if (command == StiDesignerCommand.SetMapProperties)
                {
                    StiMapHelper.SetMapProperties(currentReport, param, callbackResult);
                }
                #endregion

                #region UpdateMapData
                else if (command == StiDesignerCommand.UpdateMapData)
                {
                    StiMapHelper.UpdateMapData(currentReport, param, callbackResult);
                }
                #endregion

                #region Open Page
                else if (command == StiDesignerCommand.OpenPage)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportEdit.OpenPage(requestParams, currentReport, callbackResult);
                }
                #endregion

                #region Download Page
                else if (command == StiDesignerCommand.DownloadPage)
                {
                    var pageIndex = Convert.ToInt32(param["pageIndex"]);
                    if (currentReport.Pages.Count > pageIndex)
                    {
                        using (var stream = new MemoryStream())
                        {
                            currentReport.Pages[pageIndex].Save(stream);
                            return new StiWebActionResult(stream, "application/octet-stream", "Page.pg");
                        }
                    }
                }
                #endregion

                #region UpdateTableElement
                else if (command == StiDesignerCommand.UpdateTableElement)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null && component is IStiTableElement)
                    {
                        new StiTableElementHelper(component as IStiTableElement).ExecuteJSCommand(param["updateParameters"] as Hashtable, callbackResult);
                    }
                }
                #endregion

                #region UpdateCardsElement
                else if (command == StiDesignerCommand.UpdateCardsElement)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null && component is IStiCardsElement)
                    {
                        new StiCardsElementHelper(component as IStiCardsElement).ExecuteJSCommand(param["updateParameters"] as Hashtable, callbackResult);
                    }
                }
                #endregion

                #region UpdateImageElement
                else if (command == StiDesignerCommand.UpdateImageElement)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null && component is IStiImageElement)
                    {
                        new StiImageElementHelper(component as IStiImageElement).ExecuteJSCommand(param["updateParameters"] as Hashtable, callbackResult);
                    }
                }
                #endregion

                #region UpdateTextElement
                else if (command == StiDesignerCommand.UpdateTextElement)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null && component is IStiTextElement)
                    {
                        new StiTextElementHelper(component as IStiTextElement).ExecuteJSCommand(param["updateParameters"] as Hashtable, callbackResult);
                    }
                }
                #endregion

                #region UpdateRegionMapElement
                else if (command == StiDesignerCommand.UpdateRegionMapElement)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null && component is IStiRegionMapElement)
                    {
                        new StiRegionMapElementHelper(component as IStiRegionMapElement).ExecuteJSCommand(param["updateParameters"] as Hashtable, callbackResult);
                    }
                }
                #endregion

                #region UpdateOnlineMapElement
                else if (command == StiDesignerCommand.UpdateOnlineMapElement)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null && component is IStiOnlineMapElement)
                    {
                        new StiOnlineMapElementHelper(component as IStiOnlineMapElement).ExecuteJSCommand(param["updateParameters"] as Hashtable, callbackResult);
                    }
                }
                #endregion

                #region UpdateProgressElement
                else if (command == StiDesignerCommand.UpdateProgressElement)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null && component is IStiProgressElement)
                    {
                        new StiProgressElementHelper(component as IStiProgressElement).ExecuteJSCommand(param["updateParameters"] as Hashtable, callbackResult);
                    }
                }
                #endregion

                #region UpdateIndicatorElement
                else if (command == StiDesignerCommand.UpdateIndicatorElement)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null && component is IStiIndicatorElement)
                    {
                        new StiIndicatorElementHelper(component as IStiIndicatorElement).ExecuteJSCommand(param["updateParameters"] as Hashtable, callbackResult);
                    }
                }
                #endregion

                #region UpdateChartElement
                else if (command == StiDesignerCommand.UpdateChartElement)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null && component is IStiChartElement)
                    {
                        new StiChartElementHelper(component as IStiChartElement).ExecuteJSCommand(param["updateParameters"] as Hashtable, callbackResult);
                    }
                }
                #endregion

                #region UpdateGaugeElement
                else if (command == StiDesignerCommand.UpdateGaugeElement)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null && component is IStiGaugeElement)
                    {
                        new StiGaugeElementHelper(component as IStiGaugeElement).ExecuteJSCommand(param["updateParameters"] as Hashtable, callbackResult);
                    }
                }
                #endregion

                #region UpdateShapeElement
                else if (command == StiDesignerCommand.UpdateShapeElement)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null && component is IStiShapeElement)
                    {
                        new StiShapeElementHelper(component as IStiShapeElement).ExecuteJSCommand(param["updateParameters"] as Hashtable, callbackResult);
                    }
                }
                #endregion

                #region UpdatePivotTableElement
                else if (command == StiDesignerCommand.UpdatePivotTableElement)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null && component is IStiPivotTableElement)
                    {
                        new StiPivotTableElementHelper(component as IStiPivotTableElement).ExecuteJSCommand(param["updateParameters"] as Hashtable, callbackResult);
                    }
                }
                #endregion

                #region UpdateListBoxElement
                else if (command == StiDesignerCommand.UpdateListBoxElement)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null && component is IStiListBoxElement)
                    {
                        new StiListBoxElementHelper(component as IStiListBoxElement).ExecuteJSCommand(param["updateParameters"] as Hashtable, callbackResult);
                    }
                }
                #endregion

                #region UpdateComboBoxElement
                else if (command == StiDesignerCommand.UpdateComboBoxElement)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null && component is IStiComboBoxElement)
                    {
                        new StiComboBoxElementHelper(component as IStiComboBoxElement).ExecuteJSCommand(param["updateParameters"] as Hashtable, callbackResult);
                    }
                }
                #endregion

                #region UpdateTreeViewElement
                else if (command == StiDesignerCommand.UpdateTreeViewElement)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null && component is IStiTreeViewElement)
                    {
                        new StiTreeViewElementHelper(component as IStiTreeViewElement).ExecuteJSCommand(param["updateParameters"] as Hashtable, callbackResult);
                    }
                }
                #endregion

                #region UpdateTreeViewBoxElement
                else if (command == StiDesignerCommand.UpdateTreeViewBoxElement)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null && component is IStiTreeViewBoxElement)
                    {
                        new StiTreeViewBoxElementHelper(component as IStiTreeViewBoxElement).ExecuteJSCommand(param["updateParameters"] as Hashtable, callbackResult);
                    }
                }
                #endregion

                #region UpdateDatePickerElement
                else if (command == StiDesignerCommand.UpdateDatePickerElement)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null && component is IStiDatePickerElement)
                    {
                        new StiDatePickerElementHelper(component as IStiDatePickerElement).ExecuteJSCommand(param["updateParameters"] as Hashtable, callbackResult);
                    }
                }
                #endregion

                #region CreateTextElement
                else if (command == StiDesignerCommand.CreateTextElement)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiTextElementHelper.CreateTextElementFromDictionary(currentReport, param, callbackResult);
                }
                #endregion

                #region CreateTableElement
                else if (command == StiDesignerCommand.CreateTableElement)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiTableElementHelper.CreateTableElementFromDictionary(currentReport, param, callbackResult);
                }
                #endregion

                #region CreateDatePickerElement
                else if (command == StiDesignerCommand.CreateDatePickerElement)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDatePickerElementHelper.CreateDatePickerElementFromDictionary(currentReport, param, callbackResult);
                }
                #endregion

                #region CreateComboBoxElement
                else if (command == StiDesignerCommand.CreateComboBoxElement)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiComboBoxElementHelper.CreateComboBoxElementFromDictionary(currentReport, param, callbackResult);
                }
                #endregion

                #region ExecuteCommandForDataTransformation
                else if (command == StiDesignerCommand.ExecuteCommandForDataTransformation)
                {
                    StiDataTransformationHelper.ExecuteJSCommand(currentReport, param, callbackResult, requestParams);
                }
                #endregion

                #region ChangeDashboardStyle
                else if (command == StiDesignerCommand.ChangeDashboardStyle)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDashboardHelper.ChangeDashboardStyle(currentReport, param, callbackResult);
                }
                #endregion

                #region SetGaugeProperties
                else if (command == StiDesignerCommand.SetGaugeProperties)
                {
                    StiGaugeHelper.SetGaugeProperties(currentReport, param, callbackResult);
                }
                #endregion

                #region Open Dictionary
                else if (command == StiDesignerCommand.OpenDictionary)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.OpenDictionary(requestParams, currentReport, callbackResult);
                }
                #endregion

                #region Merge Dictionary
                else if (command == StiDesignerCommand.MergeDictionary)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.MergeDictionary(requestParams, currentReport, callbackResult);
                }
                #endregion

                #region Download Dictionary
                else if (command == StiDesignerCommand.DownloadDictionary)
                {
                    var slServices = StiSLService.GetDictionarySLServices(StiSLActions.Save);
                    if (slServices.Count > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            currentReport.Dictionary.Save(slServices[0], stream);
                            return new StiWebActionResult(stream, "application/octet-stream", "Dictionary.dct");
                        }
                    }
                }
                #endregion

                #region Open Wizard Dashboard
                else if (command == StiDesignerCommand.OpenWizardDashboard || command == StiDesignerCommand.OpenWizardReport)
                {
                    try
                    {
                        Assembly assembly = typeof(StiDesignerResourcesHelper).Assembly;
                        var resourcePath = $"Stimulsoft.Report.Web.Designer.Wizard{(command == StiDesignerCommand.OpenWizardDashboard ? "Dashboards" : "Reports")}." +
                            $"{(command == StiDesignerCommand.OpenWizardDashboard ? param["dashboardName"] : param["reportName"])}.mrt";

                        using (var stream = assembly.GetManifestResourceStream(resourcePath))
                        {
                            currentReport = new StiReport();
                            currentReport.Load(stream);
                            currentReport.Info.ForceDesigningMode = true;
                            StiDesignerOptionsHelper.ApplyDesignerOptionsToReport(StiDesignerOptionsHelper.GetDesignerOptions(requestParams), currentReport);

                            var zoom = param["zoom"] != null ? Convert.ToDouble(param["zoom"]) : 1d;
                            string reportObjectStr = StiReportEdit.WriteReportToJsObject(currentReport, zoom);

                            if (reportObjectStr != null)
                            {
                                callbackResult["reportObject"] = reportObjectStr;
                                callbackResult["reportGuid"] = requestParams.Cache.ClientGuid;
                            }
                            else
                            {
                                callbackResult["errorMessage"] = "Loading report error: Json parser error!";
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        callbackResult["reportGuid"] = null;
                        callbackResult["reportObject"] = null;
                        callbackResult["error"] = $"Loading report error. {GetErrorMessageText(e)}";
                    }
                    StiReportEdit.ClearUndoArray(requestParams);
                }
                #endregion

                #region SetPreviewSettings
                else if (command == StiDesignerCommand.SetPreviewSettings)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiReportEdit.SetPreviewSettingsProperty(currentReport, (Hashtable)param["previewSettings"], callbackResult);
                }
                #endregion

                #region UpdateElementDataFilters
                else if (command == StiDesignerCommand.UpdateElementDataFilters)
                {
                    var component = currentReport.Pages.GetComponentByName((string)param["componentName"]);
                    if (component != null && component is IStiElement)
                    {
                        new StiElementDataFiltersHelper(component as IStiElement).ExecuteJSCommand(param["updateParameters"] as Hashtable, callbackResult);
                    }
                }
                #endregion

                #region RepaintAllDbsElements
                else if (command == StiDesignerCommand.RepaintAllDbsElements)
                {
                    callbackResult["svgContents"] = StiReportEdit.GetAllDbsElementsSvgContents(currentReport);
                }
                #endregion

                #region ChangeTypeElement
                else if (command == StiDesignerCommand.ChangeTypeElement)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiChangeTypeElementHelper.ChangeTypeElement(currentReport, param, callbackResult);
                }
                #endregion

                #region GetDbsElementSortItems
                else if (command == StiDesignerCommand.GetDbsElementSortItems)
                {
                    var element = currentReport.Pages.GetComponentByName((string)param["elementName"]) as IStiElement;
                    if (element != null)
                    {
                        callbackResult["sortItems"] = StiDataSortsHelper.GetSortMenuItems(element);
                    }
                }
                #endregion

                #region ApplySortsToDashboardElement
                else if (command == StiDesignerCommand.ApplySortsToDashboardElement)
                {
                    var element = currentReport.Pages.GetComponentByName((string)param["elementName"]) as IStiElement;
                    if (element != null)
                    {
                        StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                        StiDataSortsHelper.ApplySortsToElement(element, param["sorts"] as ArrayList);
                        callbackResult["svgContent"] = StiReportEdit.GetSvgContent(element as StiComponent, StiReportEdit.StrToDouble((string)param["zoom"]));
                    }
                }
                #endregion

                #region UpdateTextFormatItemsByReportCulture
                else if (command == StiDesignerCommand.UpdateTextFormatItemsByReportCulture)
                {
                    StiTextFormatHelper.UpdateTextFormatItemsByReportCulture(currentReport, param, callbackResult);
                }
                #endregion

                #region ChangeDashboardViewMode
                else if (command == StiDesignerCommand.ChangeDashboardViewMode)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDashboardHelper.ChangeDashboardViewMode(currentReport, param, callbackResult);
                }
                #endregion

                #region GetMobileViewUnplacedElements
                else if (command == StiDesignerCommand.GetMobileViewUnplacedElements)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDashboardHelper.GetMobileViewUnplacedElements(currentReport, param, callbackResult);
                }
                #endregion

                #region PrepareReportBeforeGetData
                else if (command == StiDesignerCommand.PrepareReportBeforeGetData)
                {
                    currentReport = StiWizardHelper.PrepareReportBeforeGetData(currentReport, param, callbackResult, requestParams);
                }
                #endregion

                #region ChangeReportType
                else if (command == StiDesignerCommand.ChangeReportType)
                {
                    StiWizardHelper.ChangeReportType(currentReport, param, callbackResult, requestParams);
                }
                #endregion

                #region RestoreOldReport
                else if (command == StiDesignerCommand.RestoreOldReport)
                {
                    StiWizardHelper.RestoreOldReport(currentReport, param, callbackResult, requestParams);
                }
                #endregion

                #region AddDemoDataToReport
                else if (command == StiDesignerCommand.AddDemoDataToReport)
                {
                    StiWizardHelper.AddDemoDataToReport(currentReport, param, callbackResult, requestParams);
                }
                #endregion

                #region GetSpecialSymbols
                else if (command == StiDesignerCommand.GetSpecialSymbols)
                {
                    StiSpecialSymbolsHelper.GetSpecialSymbols(param, callbackResult);
                }
                #endregion

                #region EmbedAllDataToResources
                else if (command == StiDesignerCommand.EmbedAllDataToResources)
                {
                    StiDictionaryHelper.EmbedAllDataToResources(currentReport, param, callbackResult);
                }
                #endregion

                #region AssociatedGoogleAuthWithYourAccount
                else if (command == StiDesignerCommand.AssociatedGoogleAuthWithYourAccount)
                {
#if CLOUD
                    StiGoogleAccountHelper.AssociatedGoogleAuthWithYourAccount(param, callbackResult);
#endif
                }
                #endregion

                #region GetQuickBooksAuthorizationUrl
                else if (command == StiDesignerCommand.GetQuickBooksAuthorizationUrl)
                {
                    StiQuickBooksHelper.GetAuthorizationUrl(currentReport, param, callbackResult);
                }
                #endregion

                #region GetQuickBooksTokens
                else if (command == StiDesignerCommand.GetQuickBooksTokens)
                {
                    StiQuickBooksHelper.GetTokens(currentReport, param, callbackResult);
                }
                #endregion

                #region RefreshQuickBooksTokens
                else if (command == StiDesignerCommand.RefreshQuickBooksTokens)
                {
                    StiQuickBooksHelper.RefreshTokens(currentReport, param, callbackResult);
                }
                #endregion

                #region UpdateChartProperties
                else if (command == StiDesignerCommand.UpdateChart)
                {
                    StiChartHelper.UpdateChart(currentReport, param, callbackResult);
                }
                #endregion

                #region UpdateSparklineProperties
                else if (command == StiDesignerCommand.UpdateSparkline)
                {
                    StiSparklineHelper.UpdateSparkline(currentReport, param, callbackResult);
                }
                #endregion

                #region ViewQuery
                else if (command == StiDesignerCommand.ViewQuery)
                {
                    StiDataTransformationHelper.GetViewQueryContent(currentReport, param, callbackResult);
                }
                #endregion

                #region SaveReportThumbnail
                else if (command == StiDesignerCommand.SaveReportThumbnail)
                {
                    StiShareHelper.SaveReportThumbnail(currentReport, param, callbackResult);
                }
                #endregion

                #region TestODataConnection
                else if (command == StiDesignerCommand.TestODataConnection)
                {
                    StiDictionaryHelper.TestODataConnection(currentReport, param, callbackResult);
                }
                #endregion

                #region GetTableOfContentsIdents
                else if (command == StiDesignerCommand.GetTableOfContentsIdents)
                {
                   StiTableOfContentsHelper.GetIdentsCollection(currentReport, param, callbackResult);
                }
                #endregion

                #region UpdateComponentsPointerValues
                else if (command == StiDesignerCommand.UpdateComponentsPointerValues)
                {
                    StiTableOfContentsHelper.UpdateComponentsPointerValues(currentReport, param, callbackResult);
                }
                #endregion

                #region ChangeDashboardSettingsValue
                else if (command == StiDesignerCommand.ChangeDashboardSettingsValue)
                {
                    StiDashboardScalingHelper.ChangeDashboardSettingsValue(currentReport, param, callbackResult);
                }
                #endregion

                #region GetAzureBlobStorageContainerNamesItems
                else if (command == StiDesignerCommand.GetAzureBlobStorageContainerNamesItems)
                {
                    StiDictionaryHelper.GetAzureBlobStorageContainerNamesItems(param, callbackResult);
                }
                #endregion

                #region GetAzureBlobStorageBlobNameItems
                else if (command == StiDesignerCommand.GetAzureBlobStorageBlobNameItems)
                {
                    StiDictionaryHelper.GetAzureBlobStorageBlobNameItems(param, callbackResult);
                }
                #endregion

                #region GetAzureBlobContentTypeOrDefault
                else if (command == StiDesignerCommand.GetAzureBlobContentTypeOrDefault)
                {
                    StiDictionaryHelper.GetAzureBlobContentTypeOrDefault(param, callbackResult);
                }
                #endregion

                #region GetMathFormulaInfo
                else if (command == StiDesignerCommand.GetMathFormulaInfo)
                {
                    StiMathFormulaHelper.GetMathFormulaInfo(currentReport, param, callbackResult);
                }
                #endregion

                #region GetGoogleAnalyticsParameters
                else if (command == StiDesignerCommand.GetGoogleAnalyticsParameters)
                {
                    StiGoogleAnalyticsHelper.GetGoogleAnalyticsParameters(param, callbackResult);
                }
                #endregion

                #region DuplicateDictionaryElement
                else if (command == StiDesignerCommand.DuplicateDictionaryElement)
                {
                    if (StiDashboardAssembly.IsAssemblyLoaded)
                        StiCacheCleaner.Clean(currentReport);

                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.DuplicateDictionaryElement(currentReport, param, callbackResult);
                }
                #endregion

                #region GetBlocklyParameters
                else if (command == StiDesignerCommand.GetBlocklyInitParameters)
                {
                    StiBlocklyHelper.GetBlocklyInitParameters(currentReport, param, callbackResult);
                }
                #endregion

                #region Download Blockly
                else if (command == StiDesignerCommand.DownloadBlockly)
                {
                    var blocklyContent = StiEncodingHelper.DecodeString(param["blocklyContent"] as string);
                    var byteArray = Encoding.UTF8.GetBytes(blocklyContent);

                    using (var stream = new MemoryStream(byteArray))
                    {
                        return new StiWebActionResult(stream, "application/octet-stream", $"{param["eventName"]}.blockly");
                    }
                }
                #endregion

                #region SetDictionaryElementProperty
                else if (command == StiDesignerCommand.SetDictionaryElementProperty)
                {
                    StiReportEdit.AddReportToUndoArray(requestParams, currentReport);
                    StiDictionaryHelper.SetDictionaryElementProperty(currentReport, param, callbackResult);
                }
                #endregion

                #region GetFontsForSignature
                else if (command == StiDesignerCommand.GetStylesForSignature)
                {
                    callbackResult["styles"] = StiElectronicSignatureHelper.GetStylesForSignature();
                }
                #endregion

                #region EncryptMachineName
                else if (command == StiDesignerCommand.EncryptMachineName)
                {
                    StiCIDHelper.EncryptMachineName(param, callbackResult);
                }
                #endregion

                #region GetGitHubAuthorizationUrl
                else if (command == StiDesignerCommand.GetGitHubAuthorizationUrl)
                {
                    StiGitHubAccountHelper.GetAuthorizationUrl(currentReport, param, callbackResult);
                }
                #endregion

                #region MoveImageToResource
                else if (command == StiDesignerCommand.MoveImageToResource)
                {
                    StiReportResourcesHelper.MoveImageToResource(currentReport, param, callbackResult);
                }
                #endregion

                #region GetFacebookAuthorizationUrl
                else if (command == StiDesignerCommand.GetFacebookAuthorizationUrl)
                {
                    StiFacebookAccountHelper.GetAuthorizationUrl(currentReport, param, callbackResult);
                }
                #endregion

                #region GetStylesSamples
                else if (command == StiDesignerCommand.GetStylesContentByType)
                {
                    StiStylesHelper.GetStylesContentByType(currentReport, param, callbackResult);
                }
                #endregion

                #region CreateStyleBasedAnotherStyle
                else if (command == StiDesignerCommand.CreateStyleBasedAnotherStyle)
                {
                    StiStylesHelper.CreateStyleBasedAnotherStyle(currentReport, param, callbackResult);
                }
                #endregion

                // Update report in cache
                if (currentReport != null) requestParams.Cache.Helper.SaveReportInternal(requestParams, currentReport);
                else requestParams.Cache.Helper.RemoveReportInternal(requestParams);
            }
#if CLOUD
            catch (StiMaxDataRowsException)
            {
                callbackResult["cloudNotificationMaxDataRows"] = true;
            }
#endif
            catch (Exception e)
            {
#if SERVER
                //StiLog.WriteException(e);
#endif
                callbackResult["error"] = GetErrorMessageText(e);
                return StiWebActionResult.JsonResult(requestParams, callbackResult);
            }

            return StiWebActionResult.JsonResult(requestParams, callbackResult);
        }

        public static async Task<StiWebActionResult> CommandResultAsync(StiRequestParams requestParams, StiReport currentReport)
        {
            return await Task.Run(() => CommandResult(requestParams, currentReport));
        }
        #endregion
    }
}
