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

using Stimulsoft.Report.Dashboard;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using Stimulsoft.Base;
using Stimulsoft.Base.Plans;
using System.ComponentModel;

#if BLAZOR
using Microsoft.AspNetCore.Components;
#endif

#if NETSTANDARD
using Stimulsoft.System.Web;
using Stimulsoft.System.Web.Caching;
#else
using System.Web.Caching;
#endif

namespace Stimulsoft.Report.Web
{
    public class StiRequestParams
    {
        #region StiCacheParams

        public class StiCacheParams
        {
            internal StiCacheHelper Helper { get; set; } = null;

            public string ClientGuid { get; set; }

            public string DrillDownGuid { get; set; }

            public string DashboardDrillDownGuid { get; set; }

            public StiServerCacheMode Mode { get; set; } = StiServerCacheMode.ObjectCache;

            public TimeSpan Timeout { get; set; } = new TimeSpan(0, 20, 0);

            public CacheItemPriority Priority { get; set; } = CacheItemPriority.Default;

            internal bool UseLocalizedCache { get; set; }

            internal bool UseCacheHelper { get; set; }
        }

        #endregion

        #region StiInteractionParams

        public class StiInteractionParams
        {
            /// <summary>
            /// The the values of the request from user report variables after Submit.
            /// </summary>
            public Hashtable Variables { get; set; } = new Hashtable();

            /// <summary>
            /// The the values of the sorting parameters after Sorting action.
            /// </summary>
            public Hashtable Sorting { get; set; } = new Hashtable();

            /// <summary>
            /// The the values of the collapsing parameters after Collapsing action.
            /// </summary>
            public Hashtable Collapsing { get; set; } = new Hashtable();

            /// <summary>
            /// The the values of the parameters for all levels for drill-down report after DrillDown action.
            /// </summary>
            public ArrayList DrillDown { get; set; } = new ArrayList();

            /// <summary>
            /// The the values of the edited report fields.
            /// </summary>
            public Hashtable Editable { get; set; } = new Hashtable();

            /// <summary>
            /// The the values of the parameters for all levels for drill-down report after DrillDown action.
            /// </summary>
            public ArrayList Signatures { get; set; } = new ArrayList();

            /// <summary>
            /// The values of the signature parameters after Signature action.
            /// </summary>
            public Hashtable DashboardFiltering { get; set; } = new Hashtable();

            /// <summary>
            /// The the values of the sorting  parameters after DashboardSorting action.
            /// </summary>
            public Hashtable DashboardSorting { get; set; } = new Hashtable();

            /// <summary>
            /// The the values of the drilldown  parameters after DashboardElementDrillDown action.
            /// </summary>
            public Hashtable DashboardElementDrillDownParameters { get; set; } = new Hashtable();

            public bool DrillDownReportFile { get; set; }
        }

        #endregion

        #region StiViewerParams

        public class StiViewerParams
        {
            public int PageNumber { get; set; }

            public int OriginalPageNumber { get; set; }

            public double Zoom { get; set; } = 1;

            public StiWebViewMode ViewMode { get; set; } = StiWebViewMode.SinglePage;

            public bool ShowBookmarks { get; set; } = true;

            public string OpenLinksWindow { get; set; } = StiTargetWindow.Blank;

            public StiChartRenderType ChartRenderType { get; set; } = StiChartRenderType.AnimatedVector;

            public StiReportDisplayMode ReportDisplayMode { get; set; } = StiReportDisplayMode.FromReport;

            public StiPrintAction PrintAction { get; set; }

            public bool BookmarksPrint { get; set; } = true;

            public string OpeningFileName { get; set; }

            public string OpeningFilePassword { get; set; }

            internal bool ReportDesignerMode { get; set; }

            internal int DashboardWidth { get; set; }

            internal int DashboardHeight { get; set; }

            public StiReportType ReportType { get; set; }

            internal StiImagesQuality ImagesQuality { get; set; } = StiImagesQuality.Normal;

            // Current dashboard element for fullscreen
            internal string ElementName { get; set; }

            internal bool ParametersPanelSortDataItems { get; set; }

            internal bool CombineReportPages { get; set; }
        }

        #endregion
        
        #region StiDesignerParams

        public class StiDesignerParams
        {
            public StiDesignerCommand Command { get; set; } = StiDesignerCommand.Undefined;

            public bool IsAutoSave { get; set; }

            public bool IsNewReport { get; set; }

            public bool IsSaveAs { get; set; }

            public string FileName { get; set; }

            public string Password { get; set; }

            public string SaveType { get; set; }

            internal int UndoMaxLevel { get; set; } = 6;

            internal bool CheckReportBeforePreview { get; set; } = true;

            internal string CurrentCultureName { get; set; }

            internal StiNewReportDictionary NewReportDictionary { get; set; } = StiNewReportDictionary.DictionaryNew;
        }

        #endregion
        
        #region StiDictionaryParams

        public class StiDictionaryParams
        {
            public string ConnectionType { get; set; }

            public string ConnectionString { get; set; }

            public string Query { get; set; }

            public string UserName { get; set; }

            public string Password { get; set; }

            public string DataPath { get; set; }

            public string SchemaPath { get; set; }
        }

        #endregion

        #region StiServerParams

        public class StiServerParams
        {
            public bool UseRelativeUrls { get; set; } = true;

            public bool UseCompression { get; set; }

            public bool PassQueryParametersForResources { get; set; } = true;

            public bool PassQueryParametersToReport { get; set; }

            public bool AllowAutoUpdateCookies { get; set; }
        }

        #endregion

        #region StiScriptsParams

        internal class StiScriptsParams
        {
            public bool IncludeDashboards
            {
                get
                {
                    return StiDashboardAssembly.IsAssemblyLoaded && StiDashboardExportAssembly.IsAssemblyLoaded && StiDashboardDrawingAssembly.IsAssemblyLoaded;
                }
            }

            public bool IncludeClient { get; set; }
        }

        #endregion

        #region Properties: Internal
        internal StiScriptsParams Scripts { get; set; } = new StiScriptsParams();

        internal bool CloudMode { get; set; }

        internal bool ServerMode { get; set; }

        internal string CloudPlanIdent { get; set; }
        #endregion

        #region Properties

        /// <summary>
        /// All parameters of the Web component received from the client side.
        /// </summary>
        public Hashtable All { get; } = new Hashtable();

        private HttpContext httpContext = null;
        public HttpContext HttpContext
        {
            get
            {
                return httpContext ?? (httpContext = HttpContext.Current);
            }
            set
            {
                httpContext = value;
            }
        }

#if NETSTANDARD
        public string HttpMethod => HttpContext?.Request.Method;
#else
        public string HttpMethod => HttpContext?.Request.HttpMethod;
#endif

#if BLAZOR
        public ComponentBase Component { get; set; }
#else
        [Obsolete("This property is obsolete. It will be removed in next versions. Please use the ComponentType property instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public StiComponentType Component
        {
            get
            {
                return ComponentType;
            }
        }
#endif

        /// <summary>
        /// Web component type.
        /// </summary>
        public StiComponentType ComponentType { get; set; }

        /// <summary>
        /// Current action in the web component request.
        /// </summary>
        public StiAction Action { get; set; }
        
        public string Resource { get; set; }

        public byte[] Data { get; set; }

        /// <summary>
        /// Report for preview.
        /// </summary>
        public StiReport Report { get; set; }
        
        /// <summary>
        /// Web component ID
        /// </summary>
        public string Id { get; set; }

        public bool HasParameters { get; set; }

        public StiCacheParams Cache { get; set; } = new StiCacheParams();

        public StiInteractionParams Interaction { get; set; } = new StiInteractionParams();

        public NameValueCollection Routes { get; set; } = new NameValueCollection();

        public NameValueCollection FormValues { get; set; } = new NameValueCollection();

        /// <summary>
        /// A collection of values to be passed between component requests. It is designed to store custom values.
        /// </summary>
        public Hashtable UserValues { get; set; }

        public Hashtable ExportSettings { get; set; } = new Hashtable();

        public StiExportFormat ExportFormat { get; set; }

        public string Localization { get; set; } = "default";

        public string Theme { get; set; } = string.Empty;

        public string Version { get; set; } = string.Empty;

        public StiViewerParams Viewer { get; set; } = new StiViewerParams();

        public StiDesignerParams Designer { get; set; } = new StiDesignerParams();

        public StiDictionaryParams Dictionary { get; set; } = new StiDictionaryParams();

        public StiServerParams Server { get; set; } = new StiServerParams();

        public Hashtable ReportResourceParams { get; set; } = new Hashtable();
        #endregion

        #region Methods

        public bool Contains(string name)
        {
            return All.ContainsKey(name);
        }

        public string GetString(string name)
        {
            if (All == null || !Contains(name) || All[name] == null) return null;
            return All[name].ToString();
        }

        public bool GetBoolean(string name)
        {
            return Convert.ToBoolean(GetString(name));
        }

        public int GetInt(string name)
        {
            return (int)StiObjectConverter.ConvertToInt64(GetString(name));
        }

        public double GetDouble(string name)
        {
            return StiObjectConverter.ConvertToDouble(GetString(name));
        }

        public Hashtable GetHashtable(string name)
        {
            if (!All.Contains(name)) return null;
            return (Hashtable)All[name];
        }

        public ArrayList GetArray(string name)
        {
            if (!All.Contains(name)) return null;
            return (ArrayList)All[name];
        }

        public object GetEnum(string name, Type type)
        {
            var value = GetString(name);
            if (value == null || value.Length == 0) return 0;

            try
            {
                return Enum.Parse(type, value);
            }
            catch
            {
                try
                {
                    return Enum.Parse(type, value.Substring(0, 1).ToUpper() + value.Substring(1));
                }
                catch
                {
                    return 0;
                }
            }
        }

        public NameValueCollection GetNameValueCollection(string name)
        {
            if (!All.Contains(name) || All[name] == null || !(All[name] is Hashtable)) return null;
            var hash = (Hashtable)All[name];
            var collection = new NameValueCollection();
            foreach (var key in hash.Keys)
            {
                collection.Add(key.ToString(), hash[key] != null ? hash[key].ToString() : null);
            }

            return collection;
        }

        #endregion
    }
}
