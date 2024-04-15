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
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace Stimulsoft.Report.Components
{
    public partial class StiPagesCollection
    {
        #region PageMetric
	    public class PageMetric
	    {
            public int BlockIndex;
	        public int PageIndex;
	        public int OffsetInBlock;
	        public int SizeInBlock;
	    }
        #endregion

        #region NestedClass.CacheL2
        public class CacheL2
        {
            private const int MaxBlockSize = 5000000;
            private const int MaxQuickCacheSize = 5;

            #region Block
            public class Block
            {
                public CacheL2 ParentCache = null;
                public List<int> StoredPageNumbers = null;
                public string Guid;
                public bool IsSaved = false;
                public bool IsChanged = false;
                public List<PageMetric> Metrics = null;

                public bool HasFreeSpace
                {
                    get
                    {
                        //for server quick load of first pages
                        if ((ParentCache.blocks.IndexOf(this) == 0) && (StoredPageNumbers.Count > 4))
                        {
                            return false;
                        }

                        //calculate summary size
                        int sum = 0;
                        foreach (int pn in StoredPageNumbers)
                        {
                            if (ParentCache.pagesData[pn] != null)
                            {
                                sum += ((byte[])ParentCache.pagesData[pn]).Length;
                            }
                        }
                        return sum < MaxBlockSize;
                    }
                }

                public bool NeedLoad
                {
                    get
                    {
                        if (IsSaved == false) return false;
                        if (StoredPageNumbers.Count == 0) return false;
                        if (ParentCache.pagesData[StoredPageNumbers[0]] == null) return true;
                        return false;
                    }
                }

                public void Save()
                {
                    #if DebugCache
                    ParentCache.pages.AddMessageToLog(string.Format("Block.Save: {0}", ParentCache.blocks.IndexOf(this)));
                    #endif

                    if (ParentCache.pages.SavePageBlockToStorageIncoming != null)
                    {
                        using (var stream = new MemoryStream())
                        {
                            SaveToStream(stream);
                            ParentCache.pages.SavePageBlockToStorageIncoming(stream.ToArray(), Guid, Metrics);
                        }
                    }
                    else
                    {
                        var path = StiReportCache.GetPageCacheName(ParentCache.ReportCachePath, Guid);
                        StiFileUtils.ProcessReadOnly(path);
                        using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                        {
                            SaveToStream(stream);
                        }
                    }
                    IsSaved = true;
                    IsChanged = false;
                }
                private void SaveToStream(Stream stream)
                {
                    for (var index = 0; index < StoredPageNumbers.Count; index++)
                    {
                        var pageIndex = StoredPageNumbers[index];
                        var buf = (byte[])ParentCache.pagesData[pageIndex];
                        Metrics[index].OffsetInBlock = (int)stream.Position;
                        Metrics[index].SizeInBlock = buf.Length;
                        stream.Write(buf, 0, buf.Length);
                    }
                }

                public void Load()
                {
                    #if DebugCache
                    ParentCache.pages.AddMessageToLog(string.Format("Block.Load: {0}", ParentCache.blocks.IndexOf(this)));
                    #endif

                    if (ParentCache.pages.LoadPageBlockFromStorageIncoming != null)
                    {
                        var bytes = ParentCache.pages.LoadPageBlockFromStorageIncoming(Guid);
                        using (var stream = new MemoryStream(bytes))
                        {
                            LoadFromStream(stream);
                        }
                    }
                    else
                    {
                        var path = StiReportCache.GetPageCacheName(ParentCache.ReportCachePath, Guid);
                        StiFileUtils.ProcessReadOnly(path);
                        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            LoadFromStream(stream);
                        }
                    }
                    IsChanged = false;
                }
                private void LoadFromStream(Stream stream)
                {
                    for (var index = 0; index < StoredPageNumbers.Count; index++)
                    {
                        var pageIndex = StoredPageNumbers[index];
                        var buf = new byte[Metrics[index].SizeInBlock];
                        stream.Read(buf, 0, buf.Length);
                        ParentCache.pagesData[pageIndex] = buf;
                    }
                }

                #region Delegates
                public delegate void SavePageBlockToStorageDelegate(byte[] bytes, string blockGuid, List<PageMetric> pageMetric);
                public delegate byte[] LoadPageBlockFromStorageDelegate(string blockGuid);
                #endregion

                public override string ToString()
                {
                    return string.Format("CacheBlock, pos={0}", ParentCache.blocks.IndexOf(this));
                }

                public Block(CacheL2 parent)
                {
                    ParentCache = parent;
                    StoredPageNumbers = new List<int>();
                    Guid = global::System.Guid.NewGuid().ToString().Replace("-", "");
                    Metrics = new List<PageMetric>();
                }
            }
            #endregion

            internal class PageToSave
            {
                public StiPage Page;
                public int PageNumber;
                public byte[] ResultData;
                public BackgroundWorker Worker;
                public bool ClearContent;
            }

            private Hashtable pageToBlock = null;
            private Hashtable pagesData = null;
            private List<Block> blocks = null;
            private List<Block> quickCache = null;

            internal List<PageToSave> pagesToSave = null;
            private BackgroundWorker mainWorker = null;
            private object lockPagesToSave = new object();
            private object lockBlocks = new object();

            internal string ReportCachePath = null;
            internal StiPagesCollection pages = null;


            #region Pages processing

            public void AddPageToProcess(StiPage page, int pageNumber, bool clearContent)
            {
                #if DebugCache
                pages.AddMessageToLog(string.Format("AddPageToProcess: {0}, clear={1}", pageNumber, clearContent));
                #endif

                //optimization - on the first pass, all the pages have been already cleaned
                if ((page.Report != null) && (page.Report.ReportPass == StiReportPass.First))
                {
                    //page.Components.Clear();
                    page.DisposeImagesAndClearComponents();
                    return;
                }

                PageToSave pts = new PageToSave();
                pts.Page = page;
                pts.PageNumber = pageNumber;
                pts.ClearContent = clearContent;
                pts.Worker = new BackgroundWorker();
                pts.Worker.DoWork += new DoWorkEventHandler(AddPageToProcess_Worker_DoWork);

                lock(lockPagesToSave)
                {
                    pagesToSave.Add(pts);
                }

                pts.Worker.RunWorkerAsync(pts);

                if (mainWorker == null)
                {
                    mainWorker = new BackgroundWorker();
                    mainWorker.DoWork += new DoWorkEventHandler(mainWorker_DoWork);
                    mainWorker.RunWorkerAsync();
                }
            }

            private void AddPageToProcess_Worker_DoWork(object sender, DoWorkEventArgs e)
            {
                PageToSave pts = e.Argument as PageToSave;

                #if DebugCache
                pages.AddMessageToLog(string.Format("AddPageToProcess_DoWork: {0}, clear={1}", pts.PageNumber, pts.ClearContent));
                #endif

                try
                {
                    StiPage page = pts.Page;
                    StiReport report = pages.Report;

                    if (page.Report != null && page.Report.EngineVersion == StiEngineVersion.EngineV2)
                    {
                        StiRenderProviderV2.ProcessPageToCache(page.Report, page, true);
                    }

                    if (page != null && report.RenderedPages.NotCachedPages != null)
                    {
                        lock (report.RenderedPages.NotCachedPages)
                        {
                            int indexPage = report.RenderedPages.NotCachedPages.IndexOf(page);
                            if (indexPage != -1)
                            {
                                report.RenderedPages.NotCachedPages.RemoveAt(indexPage);
                            }
                        }
                    }

                    page.Report = null;

                    //init ReportCachePath if it not initialized before
                    if (string.IsNullOrEmpty(report.ReportCachePath))
                    {
                        report.ReportCachePath = StiReportCache.CreateNewCache();
                        StiOptions.Engine.GlobalEvents.InvokeReportCacheCreated(report, EventArgs.Empty);
                        ReportCachePath = report.ReportCachePath;
                    }

                    //process page
                    var sr = new StiSerializing(new StiReportObjectStringConverter());
                    using (MemoryStream ms = new MemoryStream())
                    {
                        pages.SerializePage(ms, sr, pts.Page, ((pages.SavePageBlockToStorageIncoming != null) && !pagesData.ContainsKey(pts.PageNumber)));
                        pts.ResultData = ms.ToArray();
                    }

                    page.Report = report;
                }
                catch
                {
                    pts.ResultData = new byte[0];
                }

                pts.Worker.DoWork -= AddPageToProcess_Worker_DoWork;
            }


            void mainWorker_DoWork(object sender, DoWorkEventArgs e)
            {
                int dataSize = 0;
                int dataSize2 = 0;

                while (true)
                {
                    lock (lockPagesToSave)
                    {
                        bool flagRepeat = true;
                        while (flagRepeat)
                        {
                            flagRepeat = false;
                            if (pagesToSave.Count > 0)
                            {
                                foreach (PageToSave pts in pagesToSave)
                                {
                                    if (pts.ResultData != null)
                                    {
                                        pagesToSave.Remove(pts);
                                        Save(pts.PageNumber, pts.ResultData);
                                        pts.Worker.Dispose();

                                        if (pts.ClearContent)
                                        {
                                            //pts.Page.Components.Clear();
                                            pts.Page.DisposeImagesAndClearComponents();
                                        }

                                        if (StiOptions.Engine.ReportCache.AllowGCCollect)
                                        {
                                            dataSize += pts.ResultData.Length;
                                            if (dataSize > MaxBlockSize * 5)
                                            {
                                                dataSize = 0;
                                                dataSize2++;
                                                GC.Collect();
                                                if (dataSize2 >= 4)
                                                {
                                                    dataSize2 = 0;

                                                    if (StiOptions.Engine.AllowWaitForPendingFinalizers)
                                                        GC.WaitForPendingFinalizers();
                                                }
                                            }
                                        }

                                        flagRepeat = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    Thread.Sleep(20);
                }
            }


            public byte[] GetPage(int pageNumber)
            {
                while (true)
                {
                    if (pagesData.ContainsKey(pageNumber))
                    {
                        return Load(pageNumber);
                    }

                    //search for unprocessed pages
                    bool flag = false;
                    lock (lockPagesToSave)
                    {
                        foreach (PageToSave pts in pagesToSave)
                        {
                            if (pts.PageNumber == pageNumber)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag) return null;
                    }

                    Thread.Sleep(20);
                }
            }

            public void CheckForPageInPagesToSave(int pageNumber)
            {
                while (true)
                {
                    //search for unprocessed pages
                    bool flag = false;
                    lock (lockPagesToSave)
                    {
                        foreach (PageToSave pts in pagesToSave)
                        {
                            if (pts.PageNumber == pageNumber)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag) return;
                    }

                    Thread.Sleep(20);
                }
            }
            #endregion

            #region Blocks processing
            private void Save(int pageNumber, byte[] pageData)
            {
                lock (lockBlocks)
                {
                    #if DebugCache
                    pages.AddMessageToLog(string.Format("CacheL2.SavePage: {0}", pageNumber));
                    #endif

                    Block block = GetBlock(pageNumber);

                    if (!pagesData.ContainsKey(pageNumber))
                    {
                        int blockIndex = blocks.IndexOf(block);
                        block.StoredPageNumbers.Add(pageNumber);
                        block.Metrics.Add(new PageMetric() { BlockIndex = blockIndex, PageIndex = pageNumber });
                        pageToBlock[pageNumber] = blockIndex;
                    }

                    byte[] oldData = (byte[])pagesData[pageNumber];
                    if (!isDataChanged(pageData, oldData)) return;
                    pagesData[pageNumber] = pageData;

                    block.IsChanged = true;
                }
            }

            private bool isDataChanged(byte[] newData, byte[] oldData)
            {
                if (oldData == null && newData == null) return false;
                if (oldData == null && newData != null) return true;
                if (oldData != null && newData == null) return true;
                if (oldData.Length != newData.Length) return true;
                for (int index = 0; index < newData.Length; index++)
                {
                    if (oldData[index] != newData[index]) return true;
                }
                return false;
            }

            private byte[] Load(int pageNumber)
            {
                lock (lockBlocks)
                {
                    #if DebugCache
                    pages.AddMessageToLog(string.Format("CacheL2.LoadPage: {0}", pageNumber));
                    #endif

                    Block block = GetBlock(pageNumber);
                    if (pagesData.ContainsKey(pageNumber))
                    {
                        return (byte[])pagesData[pageNumber];
                    }
                    return null;
                }
            }


            private Block GetBlock(int pageNumber)
            {
                int blockIndex = GetBlockIndex(pageNumber);
                Block block = blocks[blockIndex];

                if (block.NeedLoad)
                {
                    block.Load();
                    //block.IsChanged = false;
                }

                if (quickCache.Contains(block)) return block;

                quickCache.Add(block);
                if (quickCache.Count > MaxQuickCacheSize)
                {
                    var prevBlock = quickCache[0];
                    quickCache.RemoveAt(0);
                    if (!prevBlock.IsSaved || prevBlock.IsChanged)
                    {
                        prevBlock.Save();
                    }
                    //prevBlock.IsChanged = false;
                    foreach (int pageIndex in prevBlock.StoredPageNumbers)
                    {
                        pagesData[pageIndex] = null;
                    }
                    prevBlock.IsSaved = true;
                }
                return block;
            }

            private int GetBlockIndex(int pageNumber)
            {
                if (pageToBlock.ContainsKey(pageNumber))
                {
                    return (int)pageToBlock[pageNumber];
                }

                //blocks are empty
                if (blocks.Count == 0)
                {
                    blocks.Add(new Block(this));
                }

                //get last block
                int blockIndex = blocks.Count - 1;
                Block block = blocks[blockIndex];

                //last block has free space
                if (block.HasFreeSpace)
                {
                    return blockIndex;
                }

                //server optimization
                if (blockIndex == 0) block.Save();

                blocks.Add(new Block(this));
                return blocks.Count - 1;
            }
            #endregion

            #region Clear
            public void Clear()
            {
                StopMainWorker(true);
                pageToBlock.Clear();
                pagesData.Clear();
                blocks.Clear();
                quickCache.Clear();
            }

            private void StopMainWorker(bool final)
            {
                #if DebugCache
                pages.AddMessageToLog(string.Format("StopMainWorker, final={0}", final));
                #endif

                if (mainWorker != null)
                {
                    while (pagesToSave.Count != 0)
                    {
                        Thread.Sleep(20);
                    }
                    if (final)
                    {
                        mainWorker.Dispose();
                        mainWorker = null;
                    }
                }
            }

            public void Flush(bool final)
            {
                #if DebugCache
                pages.AddMessageToLog(string.Format("CacheL2.Flush, final={0}", final));
                #endif

                StopMainWorker(final);
                lock (lockBlocks)
                {
                    foreach (var block in quickCache)
                    {
                        if (!block.IsSaved || block.IsChanged)
                        {
                            block.Save();
                        }
                    }
                }
            }
            #endregion

            public CacheL2(StiPagesCollection pages)
            {
                this.pages = pages;
                pageToBlock = new Hashtable();
                pagesData = new Hashtable();
                blocks = new List<Block>();
                quickCache = new List<Block>();
                pagesToSave = new List<PageToSave>();
            }
        }
        #endregion

        #if DebugCache
        #region Logging
        private List<string> logs;
        public string Logs
        {
            get
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (var st in logs)
                {
                    sb.AppendLine(st);
                }
                return sb.ToString();
            }
        }

        private object objLock = new object();

        internal void AddMessageToLog(string message)
        {
            if (logs == null)
            {
                logs = new List<string>();

                //System.Diagnostics.ConsoleTraceListener myWriter = new System.Diagnostics.ConsoleTraceListener();
                //System.Diagnostics.Trace.Listeners.Add(myWriter);
            }
            lock (objLock)
            {
                logs.Add(message);

                //System.Diagnostics.Trace.WriteLine(message);
            }
        }
        #endregion
        #endif

        public void CheckCacheL2()
        {
            if ((this.SavePageToCache == null) && ((StiOptions.Engine.ReportCache.ThreadMode == StiReportCacheThreadMode.On) ||
                (StiOptions.Engine.ReportCache.ThreadMode == StiReportCacheThreadMode.Auto && Environment.ProcessorCount > 1 && !StiOptions.Configuration.IsWeb)))
            {
                bool collate = Report != null && Report.Collate > 1;
                if (this.cacheL2 == null && !collate)
                {
                    this.cacheL2 = new CacheL2(this);
                    if (Report != null) this.cacheL2.ReportCachePath = Report.ReportCachePath;
                }
            }
        }

	}
}