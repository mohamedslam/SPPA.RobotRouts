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

using System;
using System.Threading;

namespace Stimulsoft.Map.Gis.Core
{
    internal sealed class StiGisLoadTask : 
        IEquatable<StiGisLoadTask>
    {
        public StiGisLoadTask(StiGisCore core, StiGisPoint pos, int zoom)
        {
            this.Core = core;
            this.Pos = pos;
            this.Zoom = zoom;
        }

        #region Fields
        public StiGisCore Core;
        public StiGisPoint Pos;
        public int Zoom;
        #endregion

        #region Methods.override
        public override string ToString() => $"{Zoom} - {Pos}";
        #endregion

        #region Methods
        public bool Equals(StiGisLoadTask other)
        {
            return (Zoom == other.Zoom && Pos == other.Pos);
        }

        public void RunInThread()
        {
            var thread = new Thread(new ThreadStart(Do))
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };

            thread.Start();
        }

        private void Do()
        {
            ProcessLoad();

            lock (Core.ThreadTaskPool)
            {
                if (Core.ThreadTaskPool.Contains(this))
                    Core.ThreadTaskPool.Remove(this);
            }

            this.Core.OnRefresh();
            // start next thread
            this.Core.UpdateBoundsInternal();
        }

        private void ProcessLoad()
        {
            try
            {
                if (this.Core == null || this.Core.Matrix == null)
                    return;

                var m = this.Core.Matrix.Get(this.Zoom, this.Pos);
                if (!m.NotEmpty)
                {
                    var tile = new StiGisTile(this.Zoom, this.Pos);

                    StiGisMapImage img = null;
                    Exception ex = null;

                    var provider = this.Core.Provider;
                    if (this.Zoom >= provider.MinZoom && this.Zoom <= provider.MaxZoom)
                    {
                        if (this.Core.skipOverZoom == 0 || this.Zoom <= this.Core.skipOverZoom)
                        {
                            img = this.Core.GetImageFrom(provider, this.Pos, this.Zoom, out ex);
                        }
                    }

                    if (img != null && ex == null)
                    {
                        if (this.Core.okZoom < this.Zoom)
                        {
                            this.Core.okZoom = this.Zoom;
                            this.Core.skipOverZoom = 0;
                        }
                    }
                    else if (ex != null)
                    {
                        if ((this.Core.skipOverZoom != this.Core.okZoom) && (this.Zoom > this.Core.okZoom))
                        {
                            if (ex.Message.Contains("(404) Not Found"))
                            {
                                this.Core.skipOverZoom = this.Core.okZoom;
                            }
                        }
                    }

                    if (img != null && this.Core.IsStarted)
                    {
                        tile.Image = img;
                        this.Core.Matrix.Set(tile);
                    }
                }
            }
            catch { }
        }
        #endregion
    }
}