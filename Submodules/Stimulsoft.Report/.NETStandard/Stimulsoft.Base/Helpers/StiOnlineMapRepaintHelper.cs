#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Base.Drawing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Base.Helpers
{
    public class StiOnlineMapRepaintHelper
    {
        #region class StiOnlineMapObject
        private class StiOnlineMapObject
        {
            public IStiAppComponent Comp { get; set; }
            public int Key { get; set; }
            public string Script { get; set; }
            public WebBrowser Browser { get; set; }
            public long Tick { get; set; }
            public string ElementKey { get; set; } = "";
        }
        #endregion

        #region class StiExportData
        private class StiExportData
        {
            public Graphics G { get; set; }

            public String Script { get; set; }

            public Size Size { get; set; }

            public Size OrigialSize { get; set; }

            public bool IsReady { get; set; } = false;

            public RectangleD Location { get; set; }
        }
        #endregion

        #region Consts
        public const int TimerInterval = 1200;

        public const int BrowserLifetime = 1000 * 60 * 5;
        #endregion

        #region Fields
        private static System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        private static ConcurrentDictionary<IStiAppComponent, StiOnlineMapObject> componentBrowserHash = new ConcurrentDictionary<IStiAppComponent, StiOnlineMapObject>();
        #endregion

        #region Events
        public static event EventHandler Tick;
        #endregion

        #region Handlers
        private static void Timer_Tick(object sender, EventArgs e)
        {
            Tick?.Invoke(null, EventArgs.Empty);
        }
        #endregion

        #region Methods
        public static void Init()
        {
            timer.Interval = TimerInterval;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public static WebBrowser Get(IStiAppComponent comp, int key, bool ignoreKey = false)
        {
            lock (componentBrowserHash)
            {
                if (componentBrowserHash.ContainsKey(comp) && (ignoreKey || componentBrowserHash[comp].Key == key))
                {
                    componentBrowserHash[comp].Tick = Environment.TickCount;
                    return componentBrowserHash[comp].Browser;
                }
                return null;
            }
        }

        public static void ExportImage(Graphics g, String script, Size size, Size origialSize, RectangleD location)
        {
            var exportData = new StiExportData()
            {
                Script = script,
                Size = size,
                OrigialSize = origialSize,
                G = g,
                Location = location
            };

            var myThread = new Thread(RenderExportImage);
            myThread.SetApartmentState(ApartmentState.STA);
            myThread.Start(exportData);

            while (!exportData.IsReady)
                Thread.Sleep(1000);
        }

        static void RenderExportImage(object exportData)
        {
            var ed = (StiExportData)exportData;
            try
            {
                var webBrowser = new WebBrowser()
                {
                    Size = ed.Size,
                    ScrollBarsEnabled = false,
                    ScriptErrorsSuppressed = true,
                    DocumentText = ed.Script
                };

                var now = DateTime.Now.Ticks;
                while (now + 300000000 > DateTime.Now.Ticks)
                    Application.DoEvents();

                using (var image = new Bitmap(ed.Size.Width, ed.Size.Height))
                {
                    webBrowser.DrawToBitmap(image, new Rectangle(0, 0, ed.Size.Width, ed.Size.Height));
                    if (ed.Size.Equals(ed.OrigialSize))
                        ed.G.DrawImageUnscaled(image, ed.Location.ToRectangle());
                    else
                        ed.G.DrawImage(image, ed.Location.ToRectangle());
                }
            }
            catch { }
            finally
            {
                ed.IsReady = true;
            }
        }

        public static WebBrowser Add(IStiAppComponent comp, int key, String script, Size size, string elementKey)
        {
            lock (componentBrowserHash)
            {
                if (!componentBrowserHash.ContainsKey(comp) || componentBrowserHash[comp].Key != key)
                {
                    if (componentBrowserHash.ContainsKey(comp))
                    {
                        componentBrowserHash[comp].Browser.Dispose();
                        componentBrowserHash.TryRemove(comp, out StiOnlineMapObject removed);
                    }

                    componentBrowserHash[comp] = new StiOnlineMapObject()
                    {
                        Comp = comp,
                        Browser = new WebBrowser()
                        {
                            Size = size,
                            ScrollBarsEnabled = false,
                            ScriptErrorsSuppressed = true,
                            DocumentText = script
                        },
                        Script = script,
                        Key = key,
                        ElementKey = elementKey
                    };
                }
                componentBrowserHash[comp].Tick = Environment.TickCount;
                return componentBrowserHash[comp].Browser;
            }
        }

        public static List<IStiAppComponent> FetchAllComponents()
        {
            lock (componentBrowserHash)
            {
                if (componentBrowserHash.Count > 0)
                {
                    var result = new List<IStiAppComponent>();
                    foreach (var key in componentBrowserHash.Keys)
                    {
                        if (componentBrowserHash[key].Tick + BrowserLifetime > Environment.TickCount)
                        {
                            result.Add(key);
                        }
                        else
                        {
                            componentBrowserHash[key].Browser.Dispose();
                            componentBrowserHash.TryRemove(key, out StiOnlineMapObject removed);
                        }
                    }                        

                    return result;
                }
            }
            return null;
        }

        public static void Clean(string reportKey)
        {
            lock (componentBrowserHash)
            {
                if (reportKey == null)
                {
                    componentBrowserHash.Clear();
                }
                else
                {
                    componentBrowserHash.Keys
                        .Where(k => componentBrowserHash[k].ElementKey.StartsWith(reportKey))
                        .ToList()
                        .ForEach(k => componentBrowserHash.TryRemove(k, out StiOnlineMapObject removed));
                }
            }
        }

        public static void Dispose()
        {
            timer.Tick -= Timer_Tick;
        }
        #endregion
    }
}
