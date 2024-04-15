#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dialogs;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.App;
using Stimulsoft.Report.SaveLoad;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Stimulsoft.Report.Design.Forms;

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// The class describes a collection of pages.
    /// </summary>
    [Serializable]
    public partial class StiPagesCollection :
        CollectionBase,
        IStiStateSaveRestore,
        IStiJsonReportObject
    {
        #region class PageStore
        private class PageStore
        {
            public bool SavePage { get; set; }

            public StiPage Page { get; set; }

            public bool IsNotSaved => SavePage;


            public PageStore(StiPage page, bool savePage)
            {
                this.Page = page;
                this.SavePage = savePage;
            }
        }
        #endregion

        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            if (List.Count == 0)
                return null;

            var jObject = new JObject();

            int index = 0;
            foreach (StiPage page in List)
            {
                jObject.AddPropertyJObject(index.ToString(), page.SaveToJsonObject(mode));
                index++;
            }

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                LoadFromJsonObjectInternal((JObject)property.Value);
            }
        }
        internal void LoadFromJsonObjectInternal(JObject propJObject)
        {
            StiPage page = null;

            var ident = propJObject.Properties().FirstOrDefault(x => x.Name == "Ident").Value.ToObject<string>();

            switch (ident)
            {
                case "StiDashboard":
                    page = StiDashboardCreator.CreateDashboard(Report) as StiPage;
                    if (page == null)
                        throw new StiDashboardNotSupportedException();
                    break;

                case "StiScreen":
                    page = StiAppCreator.CreateScreen(Report) as StiPage;
                    if (page == null)
                        throw new StiAppNotSupportedException();
                    break;

                case "StiPage":
                    page = StiActivator.CreateObject("Stimulsoft.Report.Components.StiPage") as StiPage;
                    break;

                case "StiForm":
                    page = new StiForm();
                    break;

                case "StiFormContainer":
                    page = new StiFormContainer();
                    break;
            }

            Add(page);
            page.LoadFromJsonObject(propJObject);
        }
        #endregion

        #region Methods
        public List<StiPage> ToList()
        {
            return this.InnerList.Cast<StiPage>().ToList();
        }

        public void Add(StiPage page)
        {
            if (Report != null)
            {
                if (Report.Unit != page.Unit)
                    page.Convert(page.Unit, Report.Unit);
            }

            if ((Report != null && Report.IsDesigning) && string.IsNullOrEmpty(page.Name))
                page.Name = StiNameCreation.CreateName(Report, StiNameCreation.GenerateName(page));

            if (string.IsNullOrEmpty(page.Name))
            {
                if (Report != null && Report.IsDesigning)
                    page.Name = StiNameCreation.CreateName(Report, StiNameCreation.GenerateName(page));
                else
                    page.Name = StiNameCreation.CreateSimpleName(Report, StiNameCreation.GenerateName(page));
            }

            AddV2Internal(page);
        }

        internal void AddV2Internal(StiPage page)
        {
            if (page.Report == null) 
                page.Report = Report;

            List.Add(page);

            if (Report != null && CanUseCacheMode)
            {
                if (CacheMode)
                {
                    AddPageToQuickCache(page, true);
                }
                else if (Report.ReportCacheMode == StiReportCacheMode.Auto)
                {
                    if (Count > StiOptions.Engine.ReportCache.LimitForStartUsingCache)
                    {
                        this.CacheMode = true;
                        CheckCacheL2();

                        int index = this.Count;
                        lock (((ICollection)this).SyncRoot)
                        {
                            foreach (StiPage pg in this)
                            {
                                if (Report != null && Report.EngineVersion == StiEngineVersion.EngineV1)
                                {
                                    StiPostProcessProviderV1.PostProcessPage(pg,
                                        page.PageInfoV1.PageNumber == 1,
                                        false);
                                    StiPostProcessProviderV1.PostProcessPrimitives(pg);
                                }

                                AddPageToQuickCache(pg, true);

                                if (pg.Report != null && pg.Report.EngineVersion == StiEngineVersion.EngineV2)
                                    StiRenderProviderV2.ProcessPageToCache(pg.Report, pg, false);

                                index--;
                            }
                        }
                    }
                }
            }
        }

        public void AddRange(StiPage[] pages)
        {
            lock (pages.SyncRoot)
            {
                foreach (StiPage page in pages)
                {
                    Add(page);
                }
            }
        }

        public void AddRange(StiPagesCollection pages)
        {
            lock (((ICollection)pages).SyncRoot)
            {
                foreach (StiPage page in pages)
                {
                    Add(page);
                }
            }
        }

        public bool Contains(StiPage page)
        {
            return List.Contains(page);
        }

        public int IndexOf(StiPage page)
        {
            return List.IndexOf(page);
        }

        public void Insert(int index, StiPage page)
        {
            List.Insert(index, page);
        }

        public void Remove(StiPage page)
        {
            List.Remove(page);
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        public void Remove(int startIndex, int endCount)
        {
            while (endCount > 0)
            {
                RemoveAt(Count - 1);
                endCount--;
            }

            while (startIndex > 0)
            {
                RemoveAt(0);
                startIndex--;
            }
        }

        internal StiPage GetPageWithoutCache(int pageIndex)
        {
            return List[pageIndex] as StiPage;
        }

        internal void SetPageWithoutCache(int pageIndex, StiPage page)
        {
            List[pageIndex] = page;
        }

        public StiPage this[int index]
        {
            get
            {
                var page = List[index] as StiPage;
                GetPage(page);
                return page;
            }
            set
            {
                GetPage(value);
                List[index] = value;
            }
        }

        public StiPage this[string name]
        {
            get
            {
                name = name.ToLowerInvariant();

                lock (List.SyncRoot)
                {
                    foreach (StiPage page in List)
                    {
                        if (page.Name.ToLowerInvariant() == name)
                        {
                            GetPage(page);
                            return page;
                        }
                    }
                }
                return null;
            }
            set
            {
                name = name.ToLowerInvariant();
                for (int index = 0; index < List.Count; index++)
                {
                    var comp = List[index] as StiComponent;

                    if (comp.Name.ToLowerInvariant() == name)
                    {
                        GetPage(value);
                        List[index] = value;
                        return;
                    }
                }
                Add(value);
            }
        }
        
        public void SortByPriority()
        {
            lock (((ICollection)this).SyncRoot)
            {
                foreach (StiPage page in this)
                {
                    page.Components.SortByPriority();
                }
            }
        }

        protected override void OnInsertComplete(int index, object value)
        {
            base.OnInsertComplete(index, value);

            InvokePageAdded(value, EventArgs.Empty);
        }

        protected override void OnRemoveComplete(int index, object value)
        {
            base.OnRemoveComplete(index, value);

            InvokePageRemoved(value, EventArgs.Empty);
        }

        protected override void OnClearComplete()
        {
            base.OnClearComplete();

            InvokePageCleared(this, EventArgs.Empty);
        }

        public StiComponent GetComponentByName(string componentName)
        {
            lock (((ICollection)this).SyncRoot)
            {
                foreach (StiPage page in this)
                {
                    var comp = page.Components.GetComponentByName(componentName, page);
                    if (comp != null)
                        return comp;
                }
            }

            return null;
        }

        public StiComponent GetComponentByGuid(string guid)
        {
            lock (((ICollection)this).SyncRoot)
            {
                foreach (StiPage page in this)
                {
                    var comp = page.Components.GetComponentByGuid(guid, page);
                    if (comp != null)
                        return comp;
                }
            }
            return null;
        }

        private static void SetParent(StiContainer parent)
        {
            lock (((ICollection)parent.Components).SyncRoot)
            {
                foreach (StiComponent comp in parent.Components)
                {
                    comp.Parent = parent;

                    var cont = comp as StiContainer;
                    if (cont != null) 
                        SetParent(cont);
                }
            }
        }
        #endregion

        #region IStiStateSaveRestore
        /// <summary>
        /// Saves the current state of an object.
        /// </summary>
        /// <param name="stateName">A name of the state being saved.</param>
        public virtual void SaveState(string stateName)
        {
            lock (((ICollection)this).SyncRoot)
            {
                foreach (StiPage page in this)
                {
                    page.SaveState(stateName);
                }
            }
        }

        /// <summary>
        /// Restores the earlier saved object state.
        /// </summary>
        /// <param name="stateName">A name of the state being restored.</param>
        public virtual void RestoreState(string stateName)
        {
            lock (((ICollection)this).SyncRoot)
            {
                foreach (StiPage page in this)
                {
                    page.RestoreState(stateName);
                }
            }
        }

        /// <summary>
        /// Clear all earlier saved object states.
        /// </summary>
        public virtual void ClearAllStates()
        {
            lock (((ICollection)this).SyncRoot)
            {
                foreach (StiPage page in this)
                {
                    page.ClearAllStates();
                }
            }
        }
        #endregion

        #region Properties
        public StiPage[] Items => (StiPage[])InnerList.ToArray(typeof(StiPage));

        public bool CanUseCacheMode { get; set; }

        public StiReport Report { get; set; }

        public bool CacheMode { get; set; }

        public bool ContainsDashboards
        {
            get
            {
                foreach (var page in this)
                {
                    if (page is IStiDashboard)
                        return true;
                }

                return false;
            }
        }

        public bool ContainsScreens
        {
            get
            {
                foreach (var page in this)
                {
                    if (page is IStiScreen)
                        return true;
                }

                return false;
            }
        }

        public bool ContainsOnlyDashboards
        {
            get
            {
                foreach (var page in this)
                {
                    if (!(page is IStiDashboard))
                        return false;
                }

                return true;
            }
        }

        public bool ContainsOnlyScreens
        {
            get
            {
                foreach (var page in this)
                {
                    if (!(page is IStiScreen))
                        return false;
                }

                return true;
            }
        }

        internal StiSerializing Serialization { get; set; }

        private List<PageStore> QuickCachedPages { get; set; }

        internal List<StiPage> NotCachedPages { get; set; }
        #endregion
        
        #region Fields
        private int amountOfProcessedPagesForStartGCCollect = 0;
        private CacheL2 cacheL2 = null;
        private Hashtable pageHashToCacheIndex = null;
        #endregion

        #region Methods.ReportCache
        internal void AddPageToQuickCache(StiPage page, bool savePage)
        {
#if DebugCache
            AddMessageToLog(string.Format("AddPageToQuickCache: {0}, save={1}", this.List.IndexOf(page), savePage));
#endif

            if (QuickCachedPages == null)
                QuickCachedPages = new List<PageStore>();

            PageStore findedStore = null;

            foreach (var store in QuickCachedPages)
            {
                if (store.Page == page)
                {
                    findedStore = store;
                    break;
                }
            }

            if (findedStore == null)
            {
                QuickCachedPages.Add(new PageStore(page, savePage));
            }
            else
            {
                QuickCachedPages.Remove(findedStore);
                QuickCachedPages.Add(new PageStore(page, savePage || findedStore.SavePage));
            }

            if (QuickCachedPages.Count > StiOptions.Engine.ReportCache.AmountOfQuickAccessPages)
            {
                var prevPage = QuickCachedPages[0];
                bool needClearPage = true;
                if (cacheL2 == null)
                {
                    if (prevPage.SavePage)
                        SavePage(prevPage.Page);
                }
                else
                {
                    if (prevPage.SavePage)
                    {
                        cacheL2.AddPageToProcess(prevPage.Page, GetCacheIndexFromPageHash(prevPage.Page), true);
                        needClearPage = false;
                    }
                }

                if (needClearPage)
                    prevPage.Page.DisposeImagesAndClearComponents();

                QuickCachedPages.RemoveAt(0);

                if ((cacheL2 == null) && StiOptions.Engine.ReportCache.AllowGCCollect)
                {
                    int amount = StiOptions.Engine.ReportCache.AmountOfProcessedPagesForStartGCCollect;
                    
                    amountOfProcessedPagesForStartGCCollect++;
                    if (amountOfProcessedPagesForStartGCCollect >= amount)
                    {
                        amountOfProcessedPagesForStartGCCollect = 0;

                        GC.Collect();

                        if (StiOptions.Engine.AllowWaitForPendingFinalizers)
                            GC.WaitForPendingFinalizers();
                    }
                }
            }
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        public bool IsNotSavedPage(StiPage page)
        {
            lock (((ICollection)QuickCachedPages).SyncRoot)
            {
                foreach (var store in QuickCachedPages)
                {
                    if (store.Page == page) 
                        return store.IsNotSaved;
                }
            }
            return false;
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        public void MarkPageAsNotSaved(StiPage page)
        {
            lock (((ICollection)QuickCachedPages).SyncRoot)
            {
                foreach (var store in QuickCachedPages)
                {
                    if (store.Page == page) 
                        store.SavePage = true;
                }
            }
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        public void GetPage(StiPage page)
        {
            if (LoadPageFromServer != null)
            {
                if (page.Components.Count == 0)
                    LoadPageFromServer(page, EventArgs.Empty);

                return;
            }

            if (Report != null && (!CacheMode || (Report.ReportPass == StiReportPass.First))) return;

            if (cacheL2 != null)
                cacheL2.CheckForPageInPagesToSave(this.IndexOf(page));

            if (page.Components.Count != 0) return;

            #region EngineV1
            if (Report?.EngineV1?.ProgressHelper != null && Report.EngineV1.ProgressHelper.AllowCachedPagesCache)
            {
                Report.InvokeRendering();
                Report.EngineV1.ProgressHelper.ProcessInCache(page);
            }
            #endregion

            #region EngineV2
            if (Report?.Engine?.ProgressHelper != null && Report.Engine.ProgressHelper.AllowCachedPagesCache)
            {
                Report.InvokeRendering();
                Report.Engine.ProgressHelper.ProcessInCache(page);
            }
            #endregion

            LoadPage(page);

            AddPageToQuickCache(page, false);
        }

        /// <summary>
        /// Internal use only.
        /// </summary>		
        public void SavePage(StiPage page, bool clearContent = true)
        {
#if DebugCache
            AddMessageToLog(string.Format("SavePage: {0}, clear={1}", this.List.IndexOf(page), clearContent));
#endif

            if (page.Report != null && page.Report.EngineVersion == StiEngineVersion.EngineV2)
                StiRenderProviderV2.ProcessPageToCache(page.Report, page, true);

            if (page != null && Report.RenderedPages.NotCachedPages != null)
            {
                int indexPage = Report.RenderedPages.NotCachedPages.IndexOf(page);
                if (indexPage != -1)
                    Report.RenderedPages.NotCachedPages.RemoveAt(indexPage);
            }

            //optimization - on the first pass, all the pages have been already cleaned
            if ((page.Report != null) && (page.Report.ReportPass == StiReportPass.First)) return;

            if (Serialization == null) 
                Serialization = new StiSerializing(new StiReportObjectStringConverter());

            var path = StiReportCache.GetPageCacheName(Report.ReportCachePath, page.CacheGuid);

            #region Save page
            page.Report = null;

            if (this.SavePageToCache != null)
            {
                SavePageToCache(page, new StiSaveLoadPageEventArgs(page, this.IndexOf(page), path));
            }
            else
            {
                //init ReportCachePath if it not initialized before
                if (string.IsNullOrEmpty(Report.ReportCachePath))
                {
                    Report.ReportCachePath = StiReportCache.CreateNewCache();
                    StiOptions.Engine.GlobalEvents.InvokeReportCacheCreated(Report, EventArgs.Empty);
                    
                    if (cacheL2 != null) 
                        cacheL2.ReportCachePath = Report.ReportCachePath;

                    path = StiReportCache.GetPageCacheName(Report.ReportCachePath, page.CacheGuid);
                }

                if (cacheL2 != null)
                {
                    cacheL2.AddPageToProcess(page, GetCacheIndexFromPageHash(page), clearContent);
                }
                else
                {
                    StiFileUtils.ProcessReadOnly(path);
                    using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        SerializePage(stream, Serialization, page);
                        stream.Close();
                    }
                }
            }

            page.Report = Report;
            #endregion
        }

        internal void SerializePage(Stream stream, StiSerializing sr, StiPage page, bool convertToHInches = false)
        {
            var oldReport = page.Report;
            page.Report = null;
            
            if (convertToHInches && Report.ReportUnit != StiReportUnitType.HundredthsOfInch)
                page.Convert(Report.Unit, Stimulsoft.Report.Units.StiUnit.HundredthsOfInch);

            StiXmlDocumentSLService.RegPropertyNames(sr);

            sr.SortProperties = false;
            sr.CheckSerializable = true;
            sr.IgnoreSerializableForContainer = true;
            sr.Serialize(page, stream, "StiCache", StiSerializeTypes.SerializeToDocument);
            stream.Flush();

            page.Report = oldReport;
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        public void LoadPage(StiPage page)
        {
#if DebugCache
            AddMessageToLog(string.Format("LoadPage: {0}", this.List.IndexOf(page)));
#endif

            if (Serialization == null) 
                Serialization = new StiSerializing(new StiReportObjectStringConverter(true));

            var path = StiReportCache.GetPageCacheName(Report.ReportCachePath, page.CacheGuid);

            #region LoadPageFromCache
            if (this.LoadPageFromCache != null)
            {
                LoadPageFromCache(page, new StiSaveLoadPageEventArgs(page, this.IndexOf(page), path));
            }
            else
            {
                if (cacheL2 != null)
                {
                    var pageData = cacheL2.GetPage(GetCacheIndexFromPageHash(page));
                    if (pageData != null && pageData.Length != 0)
                    {
                        using (MemoryStream ms = new MemoryStream(pageData))
                        {
                            DeserializePage(ms, Serialization, page);
                        }
                    }
                }
                else
                {
                    if (File.Exists(path))
                    {
                        using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            DeserializePage(stream, Serialization, page);
                        }
                    }
                }
            }
            #endregion

            page.Report = Report;

            SetParent(page);

            var comps = page.GetComponents();
            lock (((ICollection)comps).SyncRoot)
            {
                foreach (StiComponent comp in comps)
                {
                    comp.Page = page;
                }
            }

            if ((cacheL2 != null) && StiOptions.Engine.ReportCache.AllowGCCollect)
            {
                int amount = StiOptions.Engine.ReportCache.AmountOfProcessedPagesForStartGCCollect;

                amountOfProcessedPagesForStartGCCollect++;
                if (amountOfProcessedPagesForStartGCCollect == amount)
                {
                    GC.Collect();
                }
                if (amountOfProcessedPagesForStartGCCollect >= amount * 2)
                {
                    GC.Collect();

                    if (StiOptions.Engine.AllowWaitForPendingFinalizers)
                        GC.WaitForPendingFinalizers();

                    amountOfProcessedPagesForStartGCCollect = 0;
                }
            }
        }

        private int GetCacheIndexFromPageHash(StiPage page)
        {
            if (pageHashToCacheIndex == null)
                pageHashToCacheIndex = new Hashtable();

            var obj = pageHashToCacheIndex[page.CacheGuid];
            if (obj != null)
                return (int)obj;

            var index = pageHashToCacheIndex.Count;
            pageHashToCacheIndex[page.CacheGuid] = index;
            return index;
        }

        private void DeserializePage(Stream stream, StiSerializing sr, StiPage page)
        {
            StiXmlDocumentSLService.RegPropertyNames(sr);
            sr.IgnoreSerializableForContainer = true;
            sr.Deserialize(page, stream, "StiCache");
            stream.Flush();
            stream.Close();
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        public void SaveQuickPagesToCache()
        {
            if (QuickCachedPages == null) return;

            lock (((ICollection)QuickCachedPages).SyncRoot)
            {
                foreach (var store in QuickCachedPages)
                {
                    if (store.IsNotSaved)
                    {
                        if (cacheL2 != null)                        
                            cacheL2.CheckForPageInPagesToSave(this.List.IndexOf(store.Page));                        
                        
                        SavePage(store.Page, false);
                        store.SavePage = false;
                    }
                }
            }
        }

        public void Flush(bool final = false)
        {
#if DebugCache
            AddMessageToLog(string.Format("Flush: {0}", final));
#endif

            if (cacheL2 != null)
            {
                if (final) 
                    SaveQuickPagesToCache();

                cacheL2.Flush(final);
            }
        }
        #endregion

        #region Events
        #region SavePageToCache
        public event StiSaveLoadPageEventHandler SavePageToCache;
        #endregion

        #region LoadPageFromCache
        public event StiSaveLoadPageEventHandler LoadPageFromCache;
        #endregion

        #region LoadPageFromServer
        public event StiLoadPageFromServerEventHandler LoadPageFromServer;
        #endregion

        #region PageAdded
        public event EventHandler PageAdded;

        protected virtual void OnPageAdded(EventArgs e)
        {
        }

        public virtual void InvokePageAdded(object sender, EventArgs e)
        {
            OnPageAdded(e);
            
            PageAdded?.Invoke(sender, e);
        }
        #endregion

        #region PageRemoved
        public event EventHandler PageRemoved;

        protected virtual void OnPageRemoved(EventArgs e)
        {
        }

        public virtual void InvokePageRemoved(object sender, EventArgs e)
        {
            OnPageRemoved(e);
            
            PageRemoved?.Invoke(sender, e);
        }
        #endregion

        #region PageCleared
        public event EventHandler PageCleared;

        protected virtual void OnPageCleared(EventArgs e)
        {
        }

        public virtual void InvokePageCleared(object sender, EventArgs e)
        {
            OnPageCleared(e);
            PageCleared?.Invoke(sender, e);

            QuickCachedPages?.Clear();
            cacheL2?.Clear();
            pageHashToCacheIndex?.Clear();
        }
        #endregion

        #region SavePageBlockToStorageIncoming
        public CacheL2.Block.SavePageBlockToStorageDelegate SavePageBlockToStorageIncoming;
        #endregion

        #region LoadPageBlockFromStorageIncoming
        public CacheL2.Block.LoadPageBlockFromStorageDelegate LoadPageBlockFromStorageIncoming;
        #endregion

        internal static void CopyEventsOfPagesCollection(StiPagesCollection sourcePages, StiPagesCollection destinationPages)
        {
            destinationPages.LoadPageFromCache = sourcePages.LoadPageFromCache;
            destinationPages.SavePageToCache = sourcePages.SavePageToCache;
        }
        #endregion

        public StiPagesCollection(StiReport report, StiPagesCollection originalPages)
        {
            this.Report = report;
            this.cacheL2 = originalPages.cacheL2;
            this.LoadPageFromServer = originalPages.LoadPageFromServer;

            CopyEventsOfPagesCollection(originalPages, this);
            QuickCachedPages = originalPages.QuickCachedPages;
            pageHashToCacheIndex = originalPages.pageHashToCacheIndex;
        }

        public StiPagesCollection(StiReport report)
        {
            this.Report = report;
            
            if (report != null && report.ReportCacheMode != StiReportCacheMode.Off)            
                CheckCacheL2();            
        }
    }
}