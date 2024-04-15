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
using System.Text;
using System.Web;
using System.Collections;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base;
using System.Xml;
using System.IO;

#if NETSTANDARD
using Stimulsoft.System.Web;
using Stimulsoft.System.Web.Caching;
#else
using System.Web.Caching;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiRequestParamsHelper
    {
        internal static string EncryptingKey = "8pTP5X15uKADcSw7";

        public static StiRequestParams Get()
        {
            return Get(HttpContext.Current);
        }

        public static StiRequestParams Get(HttpContext httpContext)
        {
            var requestParams = new StiRequestParams();
            requestParams.HttpContext = httpContext;

            if (httpContext != null)
            {
                foreach (string key in httpContext.Request.Params.Keys)
                {
                    if (key != null && key.StartsWith("stiweb_")) requestParams.All[key] = httpContext.Request.Params[key].ToString();
                }

                if (requestParams.Contains("stiweb_component")) requestParams.ComponentType = (StiComponentType)requestParams.GetEnum("stiweb_component", typeof(StiComponentType));
                if (requestParams.Contains("stiweb_action")) requestParams.Action = (StiAction)requestParams.GetEnum("stiweb_action", typeof(StiAction));
                if (requestParams.Contains("stiweb_theme")) requestParams.Theme = requestParams.GetString("stiweb_theme");
                if (requestParams.Contains("stiweb_cloudmode")) requestParams.CloudMode = true;
                if (requestParams.Contains("stiweb_version")) requestParams.Version = requestParams.GetString("stiweb_version");

                if (requestParams.Contains("stiweb_imagesscalingfactor")) requestParams.All["imagesScalingFactor"] = requestParams.GetString("stiweb_imagesscalingfactor");
                if (requestParams.Contains("stiweb_sharinglocalization")) requestParams.All["sharingLocalization"] = requestParams.GetString("stiweb_sharinglocalization");
                if (requestParams.Contains("stiweb_usecompression")) requestParams.All["useCompression"] = requestParams.GetBoolean("stiweb_usecompression");

                if (requestParams.Contains("stiweb_loc"))
                {
                    string base64 = requestParams.GetString("stiweb_loc");
                    requestParams.Localization = StiEncryption.Decrypt(Encoding.UTF8.GetString(Convert.FromBase64String(base64)), EncryptingKey);
                }
                if (requestParams.Contains("stiweb_data"))
                {
                    if (requestParams.Action == StiAction.Resource) requestParams.Resource = requestParams.GetString("stiweb_data");
                    else requestParams.Data = Convert.FromBase64String(requestParams.GetString("stiweb_data"));
                }
                if (requestParams.Contains("stiweb_parameters"))
                {
                    var data = Convert.FromBase64String(requestParams.GetString("stiweb_parameters"));
                    var json = Encoding.UTF8.GetString(data);
                    var container = JsonConvert.DeserializeObject<JContainer>(json);
                    ParseParameters(container, requestParams.All, null);
                    requestParams.HasParameters = true;
                    requestParams.Server.UseCompression = false;
                }
                if (requestParams.Contains("stiweb_packed_parameters"))
                {
                    var bytes = Convert.FromBase64String(requestParams.GetString("stiweb_packed_parameters"));
                    var data = StiGZipHelper.Unpack(bytes);
                    var json = Encoding.UTF8.GetString(data);
                    var container = JsonConvert.DeserializeObject<JContainer>(json);
                    ParseParameters(container, requestParams.All, null);
                    requestParams.HasParameters = true;
                    requestParams.Server.UseCompression = true;
                }
                if (requestParams.Contains("stiweb_cachemode"))
                {
                    var cacheMode = requestParams.GetString("stiweb_cachemode");
                    if (cacheMode == "none") requestParams.Cache.Mode = StiServerCacheMode.None;
                    else if (cacheMode == "cache") requestParams.Cache.Mode = StiServerCacheMode.ObjectCache;
                    else if (cacheMode == "session") requestParams.Cache.Mode = StiServerCacheMode.ObjectSession;
                }

                #region Get HTML5 Viewer parameters
                if (requestParams.HasParameters && requestParams.ComponentType == StiComponentType.Viewer)
                {
                    requestParams.Id = requestParams.GetString("viewerId");
                    requestParams.Routes = requestParams.GetNameValueCollection("routes");
                    requestParams.FormValues = requestParams.GetNameValueCollection("formValues");
                    requestParams.UserValues = requestParams.GetHashtable("userValues");
                    requestParams.CloudMode = requestParams.CloudMode || requestParams.GetBoolean("cloudMode");
                    requestParams.ServerMode = requestParams.ServerMode || requestParams.GetBoolean("serverMode");
                    requestParams.ExportFormat = (StiExportFormat)requestParams.GetEnum("exportFormat", typeof(StiExportFormat));
                    requestParams.ExportSettings = requestParams.GetHashtable("exportSettings");
                    requestParams.Version = requestParams.GetString("version");
                    requestParams.Localization = requestParams.GetString("localization");
                    requestParams.Cache.ClientGuid = requestParams.GetString("clientGuid");
                    requestParams.Cache.DrillDownGuid = requestParams.GetString("drillDownGuid");
                    requestParams.Cache.DashboardDrillDownGuid = requestParams.GetString("dashboardDrillDownGuid");
                    requestParams.Cache.Timeout = new TimeSpan(0, int.Parse(requestParams.GetString("cacheTimeout")), 0);
                    requestParams.Cache.Mode = (StiServerCacheMode)requestParams.GetEnum("cacheMode", typeof(StiServerCacheMode));
                    requestParams.Cache.Priority = (CacheItemPriority)requestParams.GetEnum("cacheItemPriority", typeof(CacheItemPriority));
                    requestParams.Cache.UseLocalizedCache = requestParams.GetBoolean("useLocalizedCache");
                    requestParams.Interaction.Variables = requestParams.GetHashtable("variables");
                    requestParams.Interaction.Signatures = requestParams.GetArray("signatures");
                    requestParams.Interaction.Sorting = requestParams.GetHashtable("sortingParameters");
                    requestParams.Interaction.Collapsing = requestParams.GetHashtable("collapsingParameters");
                    requestParams.Interaction.DrillDown = requestParams.GetArray("drillDownParameters");
                    requestParams.Interaction.Editable = requestParams.GetHashtable("editableParameters");
                    requestParams.Interaction.DashboardFiltering = requestParams.GetHashtable("dashboardFilteringParameters");
                    requestParams.Interaction.DashboardSorting = requestParams.GetHashtable("dashboardSortingParameters");
                    requestParams.Interaction.DashboardElementDrillDownParameters = requestParams.GetHashtable("dashboardElementDrillDownParameters");
                    requestParams.Interaction.DrillDownReportFile = requestParams.GetBoolean("drillDownReportFile");
                    requestParams.Viewer.PageNumber = requestParams.GetInt("pageNumber");
                    requestParams.Viewer.OriginalPageNumber = requestParams.GetInt("originalPageNumber");
                    requestParams.Viewer.Zoom = Convert.ToDouble(requestParams.GetString("zoom")) / 100;
                    requestParams.Viewer.ShowBookmarks = requestParams.GetBoolean("showBookmarks");
                    requestParams.Viewer.BookmarksPrint = requestParams.GetBoolean("bookmarksPrint");
                    requestParams.Viewer.OpeningFileName = requestParams.GetString("openingFileName");
                    requestParams.Viewer.OpeningFilePassword = requestParams.GetString("openingFilePassword");
                    requestParams.Viewer.ViewMode = (StiWebViewMode)requestParams.GetEnum("viewMode", typeof(StiWebViewMode));
                    requestParams.Viewer.ChartRenderType = (StiChartRenderType)requestParams.GetEnum("chartRenderType", typeof(StiChartRenderType));
                    requestParams.Viewer.ReportDisplayMode = (StiReportDisplayMode)requestParams.GetEnum("reportDisplayMode", typeof(StiReportDisplayMode));
                    requestParams.Viewer.PrintAction = (StiPrintAction)requestParams.GetEnum("printAction", typeof(StiPrintAction));
                    requestParams.Viewer.OpenLinksWindow = requestParams.GetString("openLinksWindow");
                    requestParams.Viewer.ReportDesignerMode = requestParams.GetBoolean("reportDesignerMode");
                    requestParams.Viewer.DashboardWidth = requestParams.GetInt("dashboardWidth");
                    requestParams.Viewer.DashboardHeight = requestParams.GetInt("dashboardHeight");
                    requestParams.Viewer.ReportType = (StiReportType)requestParams.GetEnum("reportType", typeof(StiReportType));
                    requestParams.Viewer.ElementName = requestParams.GetString("elementName");
                    requestParams.Viewer.ImagesQuality = (StiImagesQuality)requestParams.GetEnum("imagesQuality", typeof(StiImagesQuality));
                    requestParams.Viewer.ParametersPanelSortDataItems = requestParams.GetBoolean("parametersPanelSortDataItems");
                    requestParams.Viewer.CombineReportPages = requestParams.GetBoolean("combineReportPages");
                    requestParams.Server.UseRelativeUrls = requestParams.GetBoolean("useRelativeUrls");
                    requestParams.Server.PassQueryParametersForResources = requestParams.GetBoolean("passQueryParametersForResources");
                    requestParams.Server.PassQueryParametersToReport = requestParams.GetBoolean("passQueryParametersToReport");
                    requestParams.Server.AllowAutoUpdateCookies = requestParams.GetBoolean("allowAutoUpdateCookies");
                    requestParams.ReportResourceParams = requestParams.GetHashtable("reportResourceParams");
                }
                #endregion

                #region Get HTML5 Designer parameters
                if (requestParams.HasParameters && requestParams.ComponentType == StiComponentType.Designer)
                {
                    requestParams.Id = requestParams.GetString("designerId");
                    requestParams.Routes = requestParams.GetNameValueCollection("routes");
                    requestParams.FormValues = requestParams.GetNameValueCollection("formValues");
                    requestParams.CloudMode = requestParams.CloudMode || requestParams.GetBoolean("cloudMode");
                    requestParams.ServerMode = requestParams.ServerMode || requestParams.GetBoolean("serverMode");
                    requestParams.Version = requestParams.GetString("version");
                    requestParams.Cache.ClientGuid = requestParams.GetString("clientGuid");
                    requestParams.Cache.Timeout = new TimeSpan(0, int.Parse(requestParams.GetString("cacheTimeout")), 0);
                    requestParams.Cache.Mode = (StiServerCacheMode)requestParams.GetEnum("cacheMode", typeof(StiServerCacheMode));
                    requestParams.Cache.Priority = (CacheItemPriority)requestParams.GetEnum("cacheItemPriority", typeof(CacheItemPriority));
                    requestParams.Cache.UseCacheHelper = requestParams.GetBoolean("useCacheHelper");
                    requestParams.Designer.Command = (StiDesignerCommand)requestParams.GetEnum("command", typeof(StiDesignerCommand));
                    requestParams.Designer.IsSaveAs = requestParams.Designer.Command == StiDesignerCommand.SaveAsReport;
                    requestParams.Designer.IsNewReport = requestParams.GetBoolean("isNewReport");
                    requestParams.Designer.UndoMaxLevel = requestParams.GetInt("undoMaxLevel");
                    requestParams.Designer.CurrentCultureName = requestParams.GetString("currentCultureName");
                    requestParams.Designer.NewReportDictionary = (StiNewReportDictionary)requestParams.GetEnum("newReportDictionary", typeof(StiNewReportDictionary));
                    requestParams.Server.UseRelativeUrls = requestParams.GetBoolean("useRelativeUrls");
                    requestParams.Server.PassQueryParametersForResources = requestParams.GetBoolean("passQueryParametersForResources");
                    requestParams.Server.AllowAutoUpdateCookies = requestParams.GetBoolean("allowAutoUpdateCookies");

                    if (requestParams.Contains("checkReportBeforePreview")) requestParams.Designer.CheckReportBeforePreview = requestParams.GetBoolean("checkReportBeforePreview");
                    if (requestParams.Contains("localization")) requestParams.Localization = requestParams.GetString("localization");

                    if (requestParams.Contains("openReportFile")) requestParams.Designer.FileName = requestParams.GetString("openReportFile");
                    else requestParams.Designer.FileName = requestParams.GetString("reportFile");

                    if (requestParams.Contains("encryptedPassword")) requestParams.Designer.Password = requestParams.GetString("encryptedPassword");
                    else requestParams.Designer.Password = requestParams.GetString("password");

                    if (requestParams.Contains("saveType")) requestParams.Designer.SaveType = requestParams.GetString("saveType");

                    #region Correct designer action
                    switch (requestParams.Designer.Command)
                    {
                        case StiDesignerCommand.GetReportForDesigner:
                            requestParams.Action = StiAction.GetReport;
                            break;

                        case StiDesignerCommand.OpenReport:
                            requestParams.Action = StiAction.OpenReport;
                            break;

                        case StiDesignerCommand.CreateReport:               
                        case StiDesignerCommand.WizardResult:
                        case StiDesignerCommand.CreateDashboard:
                        case StiDesignerCommand.CreateForm:
                            requestParams.Action = StiAction.CreateReport;
                            break;

                        case StiDesignerCommand.SaveReport:
                        case StiDesignerCommand.SaveAsReport:
                            requestParams.Action = StiAction.SaveReport;
                            break;

                        case StiDesignerCommand.LoadReportToViewer:
                            requestParams.Action = StiAction.PreviewReport;
                            break;

                        case StiDesignerCommand.ExitDesigner:
                            requestParams.Action = StiAction.Exit;
                            break;
                    }
                    #endregion
                }
                #endregion
            }

            return requestParams;
        }

        private static void ParseParameters(JContainer container, Hashtable hash, ArrayList array)
        {
            if (hash == null && array == null) return;

            var count = -1;
            foreach (JToken token in container.Children())
            {
                count++;
                string name = token.Type == JTokenType.Property ? ((JProperty)token).Name : count.ToString();
                JToken value = token.Type == JTokenType.Property ? ((JProperty)token).Value : token;

                var childHash = value is JObject ? new Hashtable() : null;
                var childArray = value is JArray ? new ArrayList() : null;
                if (value is JContainer) ParseParameters((JContainer)value, childHash != null ? childHash : null, childArray != null ? childArray : null);

                if (hash != null)
                {
                    if (childHash != null) hash[name] = childHash;
                    else if (childArray != null) hash[name] = childArray;
                    else hash[name] = ((JValue)value).Value;
                }
                else
                {
                    if (childHash != null) array.Add(childHash);
                    else if (childArray != null) array.Add(childArray);
                    else array.Add(((JValue)value).Value);
                }
            }
        }
    }
}
