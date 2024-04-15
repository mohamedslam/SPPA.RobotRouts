#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using Stimulsoft.Base.Drawing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Threading;
using System.Timers;

namespace Stimulsoft.Base
{
    public static class StiDownloadCache
    {
        #region class FileInfo
        private class FileInfo
        {
            public string Url;
            public byte[] Data;
            public int Ticks;

            public FileInfo(string url, byte[] data, int ticks)
            {
                this.Url = url;
                this.Data = data;
                this.Ticks = ticks;
            }
        }
        #endregion

        #region Fields
        private static Hashtable cache = new Hashtable();
        private static object lockCache = new object();
        private static global::System.Threading.Timer expireTimer;
        #endregion

        #region Properties
        public static bool Enabled { get; set; } = true;
        public static int TicksToExpired { get; set; } = 120000;
        public static int TicksToFailedAttempt { get; set; } = 2000;
        public static int TicksDelayInProcess { get; set; } = 50;
        #endregion

        #region Methods
        public static void Clear()
        {
            lock (lockCache)
            {
                cache.Clear();
            }
        }

        public static byte[] Get(string url, CookieContainer cookieContainer = null, NameValueCollection headers = null)
        {
            if (!Enabled)
                return StiBytesFromURL.Load(url, cookieContainer, headers);

            //make hash key
            int key = url.GetHashCode();
            if (cookieContainer != null) key = (key * 397) ^ cookieContainer.GetHashCode();
            if (headers != null) key = (key * 397) ^ headers.GetHashCode();

            //check cache for url
            bool needLoad = false;
            bool inProcess = false;
            lock (lockCache)
            {
                if (cache.ContainsKey(key))
                {
                    object res = cache[key];
                    if (res == null)
                    {
                        inProcess = true;
                    }
                    else
                    {
                        var info2 = cache[key] as FileInfo;
                        if (info2 != null)
                            info2.Ticks = Environment.TickCount;
                    }
                }
                else
                {
                    needLoad = true;
                    cache[key] = null;
                }
            }

            //url is already downloaded somewhere in another thread, waiting
            if (inProcess)
            {
                int counter = 20000 / TicksDelayInProcess;   //protect of cycling
                while (cache.ContainsKey(key) && (cache[key] == null) && (counter > 0))
                {
                    Thread.Sleep(TicksDelayInProcess);
                    counter--;
                }
                
                if (counter == 0)
                    needLoad = true;
            }

            //no data in cache, need load
            if (needLoad)
            {
                byte[] data;

                try
                {
                    data = StiBytesFromURL.Load(url, cookieContainer, headers);
                }
                catch
                {
                    var fileInfo2 = new FileInfo(url, null, Environment.TickCount - TicksToExpired + TicksToFailedAttempt);
                    lock (lockCache)
                    {
                        cache[key] = fileInfo2;
                    }
                    throw;
                }

                var fileInfo = new FileInfo(url, data, Environment.TickCount);
                lock (lockCache)
                {
                    cache[key] = fileInfo;
                }
            }

            //get data from cache
            var info = cache[key] as FileInfo;
            if (info == null)
                throw new Exception("StiDownloadCache: Something wrong ...");

            info.Ticks = Environment.TickCount;

            CheckExpiredUrls();

            return info.Data;
        }

        public static void CheckExpiredUrls()
        {
            int currentTicks = Environment.TickCount;
            var keysToRemove = new List<string>();

            lock (lockCache)
            {
                foreach (var element in cache.Values)
                {
                    var info = element as FileInfo;
                    if ((info != null) && (currentTicks - info.Ticks > TicksToExpired))
                        keysToRemove.Add(info.Url);
                }

                foreach (string st in keysToRemove)
                {
                    cache.Remove(st);
                }
            }
        }

        private static void OnTimedEvent(object obj)
        {
            CheckExpiredUrls();
        }
        #endregion

        static StiDownloadCache()
        {
            expireTimer = new global::System.Threading.Timer(OnTimedEvent, null, 5000, TicksToExpired / 10);
        }
    }
}
