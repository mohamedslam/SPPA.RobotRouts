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

using Stimulsoft.Base;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dictionary;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;
using System.Xml;

#if NETSTANDARD
using Stimulsoft.System.Web;
using Stimulsoft.System.Web.Caching;
#else
using System.Web.Caching;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiCacheGuid
    {
        public string ReportKey { get; set; }
        public List<string> ReportGuids { get; set; } = new List<string>();
        public DateTime UpdateTime { get; set; }
    }

    public class StiCacheHelper
    {
        private static ConcurrentDictionary<string, StiCacheGuid> CacheGuids = new ConcurrentDictionary<string, StiCacheGuid>();

        #region Constants: Cache guids
        public const string GUID_ReportSnapshot = "";
        public const string GUID_ReportTemplate = "template";
        public const string GUID_Clipboard = "clipboard";
        public const string GUID_ComponentClone = "clone";
        public const string GUID_ReportCheckers = "checkers";
        public const string GUID_UndoArray = "undo";
        public const string GUID_DataTransformation = "datatransform";
        #endregion

        #region Properties
        public HttpContext HttpContext { get; set; }

        public StiServerCacheMode CacheMode { get; set; }

        public TimeSpan Timeout { get; set; }

        public CacheItemPriority Priority { get; set; }
        #endregion

        #region Methods: Helpers
        public StiCacheObjectType GetCacheObjectType(string guid)
        {
            if (string.IsNullOrEmpty(guid)) return StiCacheObjectType.Undefined;
            if (guid.EndsWith(GUID_ReportTemplate)) return StiCacheObjectType.ReportTemplate;
            if (guid.EndsWith(GUID_Clipboard)) return StiCacheObjectType.Clipboard;
            if (guid.EndsWith(GUID_ComponentClone)) return StiCacheObjectType.ComponentClone;
            if (guid.EndsWith(GUID_ReportCheckers)) return StiCacheObjectType.ReportCheckers;
            if (guid.EndsWith(GUID_UndoArray)) return StiCacheObjectType.UndoArray;
            if (guid.EndsWith(GUID_DataTransformation)) return StiCacheObjectType.DataTransformation;
            return StiCacheObjectType.ReportSnapshot;
        }

        private void UpdateCacheGuids(StiRequestParams requestParams, string reportGuid, string reportKey)
        {
            var clientGuid = requestParams.Cache.ClientGuid;
            var componentID = requestParams.Id;
            StiCacheGuid currentCacheGuid;
            CacheGuids.TryGetValue(clientGuid, out currentCacheGuid);
            if (currentCacheGuid == null) currentCacheGuid = new StiCacheGuid();

            currentCacheGuid.UpdateTime = DateTime.Now;
            currentCacheGuid.ReportKey = reportKey;
            if (!currentCacheGuid.ReportGuids.Contains(reportGuid)) currentCacheGuid.ReportGuids.Add(reportGuid);

            CacheGuids.AddOrUpdate(clientGuid, currentCacheGuid, (prevClientGuid, prevCurrentCacheGuid) => currentCacheGuid);

            RemoveObsoleteCacheGuids(requestParams);
        }

        private void RemoveObsoleteCacheGuids(StiRequestParams requestParams)
        {
            var currentDateTime = DateTime.Now;
            foreach (var clientGuid in CacheGuids.Keys)
            {
                StiCacheGuid currentCacheGuid;
                CacheGuids.TryGetValue(clientGuid, out currentCacheGuid);

                if (currentCacheGuid != null)
                {
                    var elapsedTime = new TimeSpan((currentDateTime - currentCacheGuid.UpdateTime).Ticks);
                    if (elapsedTime > Timeout)
                    {
                        StiCacheCleaner.Clean(currentCacheGuid.ReportKey);
                        foreach (var reportGuid in currentCacheGuid.ReportGuids) RemoveReport(reportGuid);

                        RemoveAllAdditionalCaches(requestParams);

                        CacheGuids.TryRemove(clientGuid, out currentCacheGuid);
                    }
                }
            }
        }

        private void RemoveAllAdditionalCaches(StiRequestParams requestParams)
        {
            var clientGuid = requestParams.Cache.ClientGuid;
            var componentID = requestParams.Id;
            var additionalGuids = new string[] { GUID_Clipboard, GUID_ComponentClone, GUID_ReportCheckers, GUID_UndoArray, GUID_DataTransformation };

            foreach (var additionalGuid in additionalGuids)
                RemoveObject($"{componentID}_{clientGuid}_{additionalGuid}");
        }
        #endregion

        #region Methods: Public virtual
        /// <summary>
        /// Get the object container for designer from cache.
        /// </summary>
        public virtual object GetObject(string guid)
        {
            object cacheData = null;

            // Update server cache
            if (CacheMode == StiServerCacheMode.ObjectCache || CacheMode == StiServerCacheMode.StringCache)
            {
                cacheData = HttpContext.Cache[guid];
                if (cacheData != null)
                {
                    HttpContext.Cache.Remove(guid);
                    HttpContext.Cache.Add(guid, cacheData, null, Cache.NoAbsoluteExpiration, Timeout, Priority, null);
                }
            }
            else if (CacheMode == StiServerCacheMode.ObjectSession || CacheMode == StiServerCacheMode.StringSession)
            {
                cacheData = HttpContext.Session[guid];
                if (cacheData != null)
                {
                    HttpContext.Session.Remove(guid);
                    HttpContext.Session.Add(guid, cacheData);
                }
            }

            // Try to get the report as StiReport object
            if (cacheData is StiReport) return cacheData as StiReport;

            // Try to parse the string cache data
            if (cacheData is string && (CacheMode == StiServerCacheMode.StringCache || CacheMode == StiServerCacheMode.StringSession))
            {
                var cacheObjectType = GetCacheObjectType(guid);

                if (cacheObjectType == StiCacheObjectType.UndoArray || cacheObjectType == StiCacheObjectType.ReportCheckers ||
                    cacheObjectType == StiCacheObjectType.Clipboard || cacheObjectType == StiCacheObjectType.ComponentClone ||
                    cacheObjectType == StiCacheObjectType.DataTransformation)
                {
                    var buffer = StiGZipHelper.Unpack(Convert.FromBase64String((string)cacheData));
                    return GetObjectFromCacheData(buffer);
                }

                var report = new StiReport();
                if (cacheObjectType == StiCacheObjectType.ReportTemplate) report.LoadPackedReportFromString(cacheData as string);
                else report.LoadPackedDocumentFromString(cacheData as string);
                report.Info.Zoom = 1;

                return report;
            }

            return cacheData;
        }

        /// <summary>
        /// Save the object container for designer to cache.
        /// </summary>
        public virtual void SaveObject(object obj, string guid)
        {
            string packedData = null;

            // Remove report object from server cache if exist
            if (CacheMode == StiServerCacheMode.ObjectCache || CacheMode == StiServerCacheMode.StringCache) HttpContext.Cache.Remove(guid);
            else HttpContext.Session.Remove(guid);
            
            // Save the report to server cache or session
            if (CacheMode == StiServerCacheMode.ObjectCache) HttpContext.Cache.Add(guid, obj, null, Cache.NoAbsoluteExpiration, Timeout, Priority, null);
            else if (CacheMode == StiServerCacheMode.ObjectSession) HttpContext.Session.Add(guid, obj);
            else if (CacheMode == StiServerCacheMode.StringCache || CacheMode == StiServerCacheMode.StringSession)
            {
                var cacheObjectType = GetCacheObjectType(guid);

                if (obj is StiReport)
                    packedData = cacheObjectType == StiCacheObjectType.ReportTemplate 
                        ? ((StiReport)obj).SavePackedReportToString()
                        : ((StiReport)obj).SavePackedDocumentToString();
                else
                    packedData = Convert.ToBase64String(StiGZipHelper.Pack(GetCacheDataFromObject(obj)));

                if (CacheMode == StiServerCacheMode.StringCache) HttpContext.Cache.Add(guid, packedData, null, Cache.NoAbsoluteExpiration, Timeout, Priority, null);
                else HttpContext.Session.Add(guid, packedData);
            }
        }

        /// <summary>
        /// Remove the object container for designer from cache.
        /// </summary>
        public virtual void RemoveObject(string guid)
        {
            if (CacheMode == StiServerCacheMode.ObjectCache || CacheMode == StiServerCacheMode.StringCache) HttpContext.Cache.Remove(guid);
            else if (CacheMode != StiServerCacheMode.None) HttpContext.Session.Remove(guid);
        }

        /// <summary>
        /// Get the report object from cache.
        /// </summary>
        public virtual StiReport GetReport(string guid)
        {
            return GetObject(guid, CacheMode, Timeout, Priority) as StiReport;
        }
        
        /// <summary>
        /// Save the report object to cache.
        /// </summary>
        public virtual void SaveReport(StiReport report, string guid)
        {
            SaveObject(report, guid, CacheMode, Timeout, Priority);
            SaveObject(report, guid + "_animations", CacheMode, Timeout, Priority);
        }
        
        /// <summary>
        /// Remove the report object from cache
        /// </summary>
        public virtual void RemoveReport(string guid)
        {
            RemoveObject(guid, CacheMode);
            if (guid.IndexOf(GUID_ReportTemplate) < 0) RemoveObject($"{guid}_{GUID_ReportTemplate}", CacheMode);
        }

        /// <summary>
        /// Get the scripts or styles of the Web component from cache.
        /// </summary>
        public virtual string GetResource(string guid)
        {
            if (CacheMode == StiServerCacheMode.StringCache) CacheMode = StiServerCacheMode.ObjectCache;
            else if (CacheMode == StiServerCacheMode.StringSession) CacheMode = StiServerCacheMode.ObjectSession;

            var cacheData = GetObject(guid);
            if (cacheData is string) return (string)cacheData;

            return null;
        }

        /// <summary>
        /// Save the scripts or styles of the Web component to cache.
        /// </summary>
        public virtual void SaveResource(string text, string guid)
        {
            if (CacheMode == StiServerCacheMode.StringCache) CacheMode = StiServerCacheMode.ObjectCache;
            else if (CacheMode == StiServerCacheMode.StringSession) CacheMode = StiServerCacheMode.ObjectSession;

            SaveObject(text, guid);
        }

        /// <summary>
        /// Remove the scripts or styles of the Web component from cache
        /// </summary>
        public virtual void RemoveResource(string guid)
        {
            RemoveObject(guid);
        }
        #endregion

        #region Methods: Internal

        #region Object
        internal object GetObjectInternal(StiRequestParams requestParams, string objectGuid)
        {
            ApplyRequestParameters(requestParams);
            var cacheGuid = string.IsNullOrEmpty(objectGuid)
                ? $"{requestParams.Id}_{requestParams.Cache.ClientGuid}"
                : $"{requestParams.Id}_{requestParams.Cache.ClientGuid}_{objectGuid}";

            return GetObject(cacheGuid, CacheMode, Timeout, Priority);
        }

        internal void SaveObjectInternal(object obj, StiRequestParams requestParams, string objectGuid)
        {
            ApplyRequestParameters(requestParams);
            var cacheGuid = string.IsNullOrEmpty(objectGuid)
                ? $"{requestParams.Id}_{requestParams.Cache.ClientGuid}"
                : $"{requestParams.Id}_{requestParams.Cache.ClientGuid}_{objectGuid}";

            SaveObject(obj, cacheGuid, CacheMode, Timeout, Priority);
        }

        internal void UpdateObjectCacheInternal(StiRequestParams requestParams, string objectGuid)
        {
            GetObjectInternal(requestParams, objectGuid);
            if (string.IsNullOrEmpty(objectGuid))
            {
                GetObjectInternal(requestParams, GUID_ReportTemplate);
                if (!string.IsNullOrEmpty(requestParams.Cache.DashboardDrillDownGuid)) GetObjectInternal(requestParams, requestParams.Cache.DashboardDrillDownGuid);
                if (!string.IsNullOrEmpty(requestParams.Cache.DrillDownGuid)) GetObjectInternal(requestParams, requestParams.Cache.DrillDownGuid);
            }
        }
        #endregion

        #region StiReport
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        /// <summary>
        /// Internal use only.
        /// </summary>
        public StiReport GetReportInternal(StiRequestParams requestParams)
        {
            return GetReportInternal(requestParams, true);
        }

        internal StiReport GetReportInternal(StiRequestParams requestParams, bool useDrillDownReportSnapshot)
        {
            var report = requestParams.Report;
            if (report == null && requestParams.Cache.Mode != StiServerCacheMode.None && !string.IsNullOrEmpty(requestParams.Cache.ClientGuid) &&
                requestParams.Action != StiAction.Undefined && requestParams.Action != StiAction.Resource)
            {
                ApplyRequestParameters(requestParams);

                // Try to get the drilldown dashboard
                if (!string.IsNullOrEmpty(requestParams.Cache.DashboardDrillDownGuid) && string.IsNullOrEmpty(requestParams.Cache.DrillDownGuid) && (
                    requestParams.Action == StiAction.DashboardDrillDown ||
                    requestParams.Action == StiAction.DashboardElementDrillDown ||
                    requestParams.Action == StiAction.DashboardElementDrillUp ||
                    requestParams.Action == StiAction.DashboardSorting ||
                    requestParams.Action == StiAction.DashboardFiltering ||
                    requestParams.Action == StiAction.DashboardResetAllFilters ||
                    requestParams.Action == StiAction.DashboardGettingFilterItems||
                    requestParams.Action == StiAction.DashboardViewData ||
                    requestParams.Action == StiAction.RefreshReport ||
                    requestParams.Action == StiAction.GetPages||
                    requestParams.Action == StiAction.ExportDashboard ||
                    requestParams.Action == StiAction.ExportReport))
                {
                    var cacheGuid = GetReportCacheGuid(requestParams, $"{requestParams.Cache.DashboardDrillDownGuid}_{GUID_ReportTemplate}");
                    var drillDownReport = GetReport(cacheGuid, CacheMode, Timeout, Priority);
                    if (drillDownReport != null)
                    {
                        UpdateCacheGuids(requestParams, cacheGuid, drillDownReport.Key);
                        return drillDownReport;
                    }
                }

                // Check if require only a report template
                var isReportTemplateRequire = (
                    requestParams.ComponentType == StiComponentType.Designer &&
                    requestParams.Action != StiAction.PrintReport &&
                    requestParams.Action != StiAction.ExportReport &&
                    requestParams.Action != StiAction.EmailReport
                ) || useDrillDownReportSnapshot && (
                        (requestParams.Action == StiAction.InitVars && !requestParams.Interaction.DrillDownReportFile) ||
                        requestParams.Action == StiAction.Variables ||
                        requestParams.Action == StiAction.Sorting ||
                        requestParams.Action == StiAction.Collapsing ||
                        requestParams.Action == StiAction.DrillDown ||
                        requestParams.Action == StiAction.DashboardDrillDown ||
                        requestParams.Action == StiAction.ExportDashboard ||
                        requestParams.Action == StiAction.RefreshReport);

                // Get the report object from server cache
                if (isReportTemplateRequire)
                {
                    // Try to get the report template
                    var cacheGuid = GetReportCacheGuid(requestParams, GUID_ReportTemplate);
                    report = GetReport(cacheGuid, CacheMode, Timeout, Priority);
                    if (report != null && requestParams.Action != StiAction.InitVars)
                    {
                        report.IsRendered = false;
                        UpdateCacheGuids(requestParams, cacheGuid, report.Key);
                    }
                }
                else
                {
                    // Try to get the report snapshot
                    var cacheGuid = (useDrillDownReportSnapshot || requestParams.Interaction.DrillDownReportFile) && !string.IsNullOrEmpty(requestParams.Cache.DrillDownGuid)
                        ? GetReportCacheGuid(requestParams, requestParams.Cache.DrillDownGuid)
                        : GetReportCacheGuid(requestParams);

                    report = GetReport(cacheGuid, CacheMode, Timeout, Priority);
                    
                    // If there is no report snapshot, try to take the report template
                    if (report == null)
                    {
                        cacheGuid = GetReportCacheGuid(requestParams, GUID_ReportTemplate);
                        report = GetReport(cacheGuid, CacheMode, Timeout, Priority);
                    }

                    if (report != null)
                        UpdateCacheGuids(requestParams, cacheGuid, report.Key);
                }
            }

            if (report != null)
            {
                if (report.CookieContainer == null || report.CookieContainer.Count == 0 || requestParams.Server.AllowAutoUpdateCookies)
                    report.CookieContainer = StiReportHelper.GetCookieContainer(requestParams.HttpContext.Request);

                if (requestParams.ComponentType == StiComponentType.Designer && (requestParams.Cache.Mode == StiServerCacheMode.StringCache || requestParams.Cache.Mode == StiServerCacheMode.StringSession || requestParams.Cache.UseCacheHelper))
                {
                    report.Info.ForceDesigningMode = true;
                    report.Pages.ToList().ForEach(p => { p.CheckLargeHeight(); });
                }
            }

            return report;
        }

        internal void SaveReportInternal(StiRequestParams requestParams, StiReport report)
        {
            if (report != null && requestParams.Cache.Mode != StiServerCacheMode.None && !string.IsNullOrEmpty(requestParams.Cache.ClientGuid) &&
                requestParams.Action != StiAction.Undefined && requestParams.Action != StiAction.Resource)
            {
                ApplyRequestParameters(requestParams);

                if (report.CookieContainer == null || report.CookieContainer.Count == 0 || requestParams.Server.AllowAutoUpdateCookies)
                    report.CookieContainer = StiReportHelper.GetCookieContainer(requestParams.HttpContext.Request);

                // Save drilldown dashboard
                if (!string.IsNullOrEmpty(requestParams.Cache.DashboardDrillDownGuid) && (
                    requestParams.Action == StiAction.DashboardDrillDown ||
                    requestParams.Action == StiAction.DashboardElementDrillDown ||
                    requestParams.Action == StiAction.DashboardElementDrillUp ||
                    requestParams.Action == StiAction.DashboardSorting ||
                    requestParams.Action == StiAction.DashboardFiltering ||
                    requestParams.Action == StiAction.DashboardResetAllFilters ||
                    requestParams.Action == StiAction.DashboardGettingFilterItems ||
                    requestParams.Action == StiAction.DashboardViewData ||
                    requestParams.Action == StiAction.RefreshReport))
                {
                    var cacheGuid = GetReportCacheGuid(requestParams, $"{requestParams.Cache.DashboardDrillDownGuid}_{GUID_ReportTemplate}");
                    SaveReport(report, cacheGuid, CacheMode, Timeout, Priority);

                    UpdateCacheGuids(requestParams, cacheGuid, report.Key);
                    return;
                }

                // Save only rendered report, if one of the specified actions
                var isReportSnapshot =
                    requestParams.Action == StiAction.Variables ||
                    requestParams.Action == StiAction.Sorting ||
                    requestParams.Action == StiAction.Collapsing ||
                    requestParams.Action == StiAction.DrillDown ||
                    requestParams.Action == StiAction.Signatures ||
                    requestParams.Action == StiAction.PreviewReport ||
                    requestParams.Action == StiAction.RefreshReport;

                // Save the report template to cache if it needed in the future
                if (!isReportSnapshot && !report.IsDocument &&
                    (requestParams.ComponentType == StiComponentType.Designer ||
                     report.Dictionary.IsRequestFromUserVariablesPresent || StiReportHelper.IsReportHasInteractions(report) ||
                     report.ContainsDashboard || report.RefreshTime > 0))
                {
                    var cacheGuid = GetReportCacheGuid(requestParams, GUID_ReportTemplate);
                    SaveReport(report, cacheGuid, CacheMode, Timeout, Priority);

                    // Skip render process if report template for Designer
                    if (requestParams.ComponentType == StiComponentType.Designer)
                    {
                        UpdateCacheGuids(requestParams, cacheGuid, report.Key);
                        return;
                    }
                }
                // Remove old report from cache if GetReport or OpenReport actions received
                else if (!requestParams.Viewer.ReportDesignerMode && (requestParams.Action == StiAction.GetReport || requestParams.Action == StiAction.OpenReport))
                {
                    var cacheGuid = GetReportCacheGuid(requestParams);
                    RemoveReport(cacheGuid);
                }

                // Render report if not rendered and not document
                if (!report.IsRendered && !report.IsDocument && !report.ContainsOnlyDashboard)
                {
                    try
                    {
                        report.Render(false);
                    }
                    catch
                    {
                    }
                }

                // Save the report snapshot to cache
                if (!report.ContainsDashboard)
                {
                    var snapshotCacheGuid = !string.IsNullOrEmpty(requestParams.Cache.DrillDownGuid)
                        ? GetReportCacheGuid(requestParams, requestParams.Cache.DrillDownGuid)
                        : GetReportCacheGuid(requestParams);

                    report.SaveInteractionParametersToDocument = true;
                    SaveReport(report, snapshotCacheGuid, CacheMode, Timeout, Priority);
                    report.SaveInteractionParametersToDocument = false;

                    UpdateCacheGuids(requestParams, snapshotCacheGuid, report.Key);
                }
            }
        }

        internal void SavePreviewReportInternal(StiRequestParams requestParams, StiReport report, string guid)
        {
            guid = GetReportCacheGuid(requestParams, null, guid);

            ApplyRequestParameters(requestParams);
            RemoveReport(guid);

            // Save report template
            var cacheGuid = $"{guid}_{GUID_ReportTemplate}";
            SaveReport(report, cacheGuid);

            // Save rendered report
            if (report.IsRendered && !report.ContainsDashboard)
            {
                cacheGuid = guid;
                report.SaveInteractionParametersToDocument = true;
                SaveReport(report, cacheGuid);
                report.SaveInteractionParametersToDocument = false;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        /// <summary>
        /// Internal use only.
        /// </summary>
        public virtual void RemoveReportInternal(StiRequestParams requestParams)
        {
            ApplyRequestParameters(requestParams);

            var cacheGuid = GetReportCacheGuid(requestParams);
            RemoveReport(cacheGuid, CacheMode);
        }
        #endregion

        #region Resources
        internal string GetResourceInternal(StiRequestParams requestParams, string guid)
        {
            ApplyRequestParameters(requestParams);
            return GetResource(guid);
        }

        internal void SaveResourceInternal(StiRequestParams requestParams, string text, string guid)
        {
            ApplyRequestParameters(requestParams);
            SaveResource(text, guid);
        }
        #endregion

        internal void ApplyRequestParameters(StiRequestParams requestParams)
        {
            HttpContext = requestParams.HttpContext;
            CacheMode = requestParams.Cache.Mode;
            Timeout = requestParams.Cache.Timeout;
            Priority = requestParams.Cache.Priority;
        }

        private string GetReportCacheGuid(StiRequestParams requestParams)
        {
            return GetReportCacheGuid(requestParams, null, null);
        }

        private string GetReportCacheGuid(StiRequestParams requestParams, string typeGuid)
        {
            return GetReportCacheGuid(requestParams, typeGuid, null);
        }

        private string GetReportCacheGuid(StiRequestParams requestParams, string typeGuid, string baseGuid)
        {
            var cacheGuid = string.IsNullOrEmpty(baseGuid)
                ? $"{requestParams.Id}_{requestParams.Cache.ClientGuid}"
                : baseGuid;

            if (requestParams.Cache.UseLocalizedCache)
                cacheGuid = $"{cacheGuid}_{StiMD5Helper.ComputeHash(requestParams.Localization).Substring(0, 8)}";

            if (!string.IsNullOrEmpty(typeGuid))
                cacheGuid = $"{cacheGuid}_{typeGuid}";

            return cacheGuid;
        }
        #endregion

        #region Methods: Object serializer
        private static class DataSerializer
        {
            public static byte[] Serialize(object obj)
            {
                if (obj == null) return null;
                if (obj is byte[]) return (byte[])obj;
                return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, Stimulsoft.Base.Json.Formatting.None, new StringEnumConverter()));
            }

            public static T Deserialize<T>(byte[] bytes)
            {
                if (bytes == null) return default(T);
                if (GetReportContentType(bytes) != StiReportContentType.Undefined) return (T)(bytes as object);
                return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes), new StringEnumConverter());
            }
        }

        public static object GetObjectFromCacheData(byte[] fileData)
        {
            var data = DataSerializer.Deserialize<object>(fileData);
            try
            {
                if (data is byte[])
                {
                    var bytes = data as byte[];
                    var contentType = GetReportContentType(bytes);
                    if (contentType != StiReportContentType.Undefined)
                    {
                        var report = new StiReport();
                        if (contentType == StiReportContentType.XmlTemplate || contentType == StiReportContentType.JsonTemplate)
                        {
                            report.Info.Zoom = 1;
                            report.Info.ForceDesigningMode = true;
                            report.Load(bytes);
                        }
                        else if (contentType == StiReportContentType.XmlDocument || contentType == StiReportContentType.JsonDocument)
                        {
                            report.LoadDocument(bytes);
                        }
                        return report;
                    }
                }

                if (data is string && data.ToString().Trim().IndexOf("{\r\n  \"Ident\": \"StiDataTransformation\"") == 0)
                {
                    var dataTransformation = new StiDataTransformation();
                    dataTransformation.LoadFromJsonObject(JsonConvert.DeserializeObject<JObject>(data.ToString()));
                    return dataTransformation;
                }

                if (data is string)
                    return data;


                if (data is JArray)
                {
                    #region Parse JArray
                    var array = new ArrayList();
                    var jsArray = (JArray)data;
                    var doubleList = new List<double?[]>();

                    foreach (object value in jsArray.Children())
                    {
                        if (value is JValue)
                            array.Add((value as JValue).Value);
                        else if (value is JObject)
                        {
                            var jObject = value as JObject;
                            var resultObject = new Hashtable();
                            foreach (var property in jObject.Properties())
                            {
                                resultObject[property.Name] = (property.Value as JValue)?.Value;
                            }
                            array.Add(resultObject);
                        }
                        else if (value is JArray)
                        {
                            var innerArray = new ArrayList(); 
                            var innerJsArray = (JArray)value;
                            foreach (object innerValue in innerJsArray.Children())
                            {
                                if (innerValue is JValue)
                                    innerArray.Add((innerValue as JValue).Value);
                            }
                            doubleList.Add(innerArray.ToArray(typeof(double?)) as double?[]);
                        }
                    }
                    #endregion

                    if (doubleList.Count > 0 && array.Count == 0)
                        return doubleList;

                    for (var i = 0; i < array.Count; i++)
                    {
                        if (array[i] != null)
                        {
                            if (array[i] is string)
                            {
                                var str = (string)array[i];
                                if (!string.IsNullOrEmpty(str))
                                {   
                                    if (str.IsBase64String())
                                    {
                                        var content = Convert.FromBase64String(str);
                                        var contentType = GetReportContentType(content);
                                        if (contentType == StiReportContentType.XmlTemplate || contentType == StiReportContentType.JsonTemplate)
                                        {
                                            var report = new StiReport();
                                            report.Info.ForceDesigningMode = true;
                                            report.Info.Zoom = 1;
                                            report.Load(content);
                                            array[i] = report;
                                        }
                                        else if(contentType == StiReportContentType.XmlDocument || contentType == StiReportContentType.JsonDocument)
                                        {
                                            var report = new StiReport();
                                            report.LoadDocument(content);
                                            array[i] = report;
                                        }
                                    }
                                    else if (!str.StartsWith("<?xml"))
                                    {
                                        var typeComponent = str.Substring(0, str.IndexOf(";"));
                                        var strComponent = str.Substring(str.IndexOf(";") + 1);

                                        var assembly = typeof(StiReport).Assembly;
                                        var component = assembly.CreateInstance("Stimulsoft.Report.Components." + typeComponent) as StiComponent;
                                        if (component == null) component = assembly.CreateInstance("Stimulsoft.Report.Chart." + typeComponent) as StiComponent;
                                        else if (component == null) component = assembly.CreateInstance("Stimulsoft.Report.CrossTab." + typeComponent) as StiComponent;

                                        if (component != null)
                                        {
                                            using (var stringReader = new StringReader(strComponent))
                                            {
                                                var sr = new StiSerializing(new StiReportObjectStringConverter());
                                                sr.Deserialize(component, stringReader, "StiComponent");
                                                stringReader.Close();
                                            }
                                        }
                                        array[i] = component;
                                    }
                                }
                            }
                            else if (array[i] is Hashtable)
                            {
                                var containerParams = array[i] as Hashtable;
                                var report = new StiReport();
                                report.Info.ForceDesigningMode = true;
                                report.Info.Zoom = 1;
                                report.Load(Convert.FromBase64String(containerParams["report"] as string));
                                var command = (StiDesignerCommand)Enum.Parse(typeof(StiDesignerCommand), containerParams["command"] as string);
                                var reportContainer = new StiReportContainer(report, (bool)containerParams["resourcesIncluded"], command);
                                array[i] = reportContainer;
                            }
                        }
                    }
                    return array;
                }
            }
            finally
            {
            }

            return null;
        }

        public static byte[] GetCacheDataFromObject(object obj)
        {
            byte[] data = null;

            if (obj is StiReport)
            {
                var report = (StiReport)obj;
                data = DataSerializer.Serialize(report.IsDocument && !report.ContainsDashboard && !report.Info.ForceDesigningMode ? report.SaveDocumentToByteArray() : report.SaveToByteArray());
            }

            else if (obj is string)
                data = DataSerializer.Serialize((string)obj);

            else if (obj is List<double?[]>)
            {
                var array = new ArrayList();
                ((List<double?[]>)obj).ForEach(a => { array.Add(a); });

                data = DataSerializer.Serialize(array);
            }

            else if (obj is StiDataTransformation)
                data = DataSerializer.Serialize(((StiDataTransformation)obj).SaveToJsonObject(StiJsonSaveMode.Report).ToString());

            else if (obj is ArrayList)
            {
                var array = (ArrayList)obj;
                for (var i = 0; i < array.Count; i++)
                {
                    if (array[i] != null)
                    {
                        if (array[i] is StiReport)
                        {
                            array[i] = ((StiReport)array[i]).SaveToByteArray();
                        }
                        else if (array[i] is StiComponent)
                        {
                            var sr = new StiSerializing(new StiReportObjectStringConverter());
                            var sb = new StringBuilder();
                            using (var stringWriter = new StringWriter(sb))
                            {
                                sr.Serialize(array[i], stringWriter, "StiComponent", StiSerializeTypes.SerializeToAll);
                                stringWriter.Close();
                                array[i] = array[i].GetType().Name + ";" + sb.ToString();
                            }
                        }
                        else if (array[i] is StiReportContainer)
                        {
                            var reportContainer = new Hashtable();
                            reportContainer["command"] = (array[i] as StiReportContainer).command;
                            reportContainer["resourcesIncluded"] = (array[i] as StiReportContainer).resourcesIncluded;
                            reportContainer["report"] = (array[i] as StiReportContainer).report.SaveToByteArray();
                            array[i] = reportContainer;
                        }
                    }
                }

                data = DataSerializer.Serialize(array);
            }            

            return data;
        }

        public static StiReportContentType GetReportContentType(byte[] bytes)
        {
            try
            {
                var stream = new MemoryStream(bytes);
                if (StiReport.IsJsonFile(stream))
                {
                    var serializer = new JsonSerializer();
                    using (var sr = new StreamReader(stream))
                    using (var jsonTextReader = new JsonTextReader(sr))
                    {
                        var jObject = serializer.Deserialize(jsonTextReader) as JObject;
                        foreach (var property in jObject.Properties())
                        {
                            if (property.Name == "RenderedPages")
                                return StiReportContentType.JsonDocument;

                            if (property.Name == "Pages")
                                return StiReportContentType.JsonTemplate;
                        }
                    }
                }
                else
                {
                    var reader = new XmlTextReader(stream);
                    reader.DtdProcessing = DtdProcessing.Ignore;

                    reader.Read();
                    reader.Read();
                    if (reader.IsStartElement() && reader.Name == "StiSerializer")
                    {
                        var applicationAttr = reader.GetAttribute("application");
                        
                        if (applicationAttr == "StiDocument")
                            return StiReportContentType.XmlDocument;
                        
                        if (applicationAttr == "StiReport")
                            return StiReportContentType.XmlTemplate;
                    }
                }
            }
            catch
            {
            }
            return StiReportContentType.Undefined;
        }
        #endregion


        #region Obsolete

        #region Hidden / Delete this methods in the 2019.1 release

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public virtual object GetObject(string guid, StiServerCacheMode cacheMode, TimeSpan timeout, CacheItemPriority priority)
        {
            return GetObject(guid);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public virtual void SaveObject(object obj, string guid, StiServerCacheMode cacheMode, TimeSpan timeout, CacheItemPriority priority)
        {
            SaveObject(obj, guid);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public virtual void RemoveObject(string guid, StiServerCacheMode cacheMode)
        {
            RemoveObject(guid);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public virtual StiReport GetReport(string guid, StiServerCacheMode cacheMode, TimeSpan timeout, CacheItemPriority priority)
        {
            return GetReport(guid);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public virtual void SaveReport(StiReport report, string guid, StiServerCacheMode cacheMode, TimeSpan timeout, CacheItemPriority priority)
        {
            SaveReport(report, guid);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public virtual void RemoveReport(string guid, StiServerCacheMode cacheMode)
        {
            RemoveReport(guid);
        }

        #endregion

        [Obsolete("This method is obsolete. It will be removed in next versions. Please use the GetObject() method instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public virtual object GetObjectFromCache(string guid)
        {
            return null;
        }

        [Obsolete("This method is obsolete. It will be removed in next versions. Please use the SaveObject() method instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public virtual void SaveObjectToCache(object obj, string guid)
        {
        }

        #endregion
    }
}
