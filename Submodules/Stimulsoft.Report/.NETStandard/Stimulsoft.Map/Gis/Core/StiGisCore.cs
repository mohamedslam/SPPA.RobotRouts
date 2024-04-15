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

using Stimulsoft.Base.Gis;
using Stimulsoft.Map.Gis.Cache;
using Stimulsoft.Map.Gis.Providers;
using Stimulsoft.Report.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Threading;

namespace Stimulsoft.Map.Gis.Core
{
    public sealed class StiGisCore : IDisposable
    {
        internal StiGisCore(StiGeoRenderMode mode, bool isSimpleMode = false) 
        {
            this.RenderMode = mode;
            this.isSimpleMode = isSimpleMode;

            ServicePointManager.DefaultConnectionLimit = 5;
        }

        ~StiGisCore()
        {
            Dispose(false);
        }

        #region Fields
        private readonly StiGisMemoryCache memoryCache = new StiGisMemoryCache();
        public const int LevelsKeepInMemmory = 5;
        private const int GThreadPoolSize = 4;

        public bool AllowLocalCache;
        private bool isSimpleMode;

        public StiGisPointLatLng position = new StiGisPointLatLng(38.90813299596704, -77.01416015624999);
        public StiGisPoint positionPixel;
        public StiGisPoint renderOffset;
        public StiGisPoint centerTileXYLocation;
        public StiGisPoint centerTileXYLocationLast;
        public StiGisPoint dragPoint;
        public StiGisPoint compensationOffset;
        public StiGisPoint mouseDown;
        public StiGisPoint mouseCurrent;
        public StiGisPoint mouseLastZoom;

        internal StiGisSize sizeOfMapArea;
        internal StiGisSize minOfTiles;
        internal StiGisSize maxOfTiles;
        internal StiGisRect tileRect;

        private BackgroundWorker invalidateWorker;
        private AutoResetEvent refreshEvent = new AutoResetEvent(false);
        internal StiGisTileMatrix Matrix = new StiGisTileMatrix();
        internal List<StiGisDrawTile> TileDrawingList = new List<StiGisDrawTile>();
        internal List<StiGisLoadTask> ThreadTaskPool = new List<StiGisLoadTask>();
        private Stack<StiGisLoadTask> tileLoadQueue = new Stack<StiGisLoadTask>();

        internal int Width;
        internal int Height;
        internal bool IsDragging;
        internal bool zoomToArea = true;
        internal bool MouseWheelZooming;
        internal volatile int okZoom = 0;
        internal volatile int skipOverZoom = 0;
        internal volatile bool IsStarted;
        internal bool UpdatingBounds;

        internal Color GeometryColor;
        internal double GeometryLineSize;
        private StiLanguageType language = StiLanguageType.English;
        #endregion

        #region Properties
        public System.Windows.Media.FontFamily WpfFontFamily { get; set; }

        public string LanguageStr { get; private set; } = "en";

        /// <summary>
        /// map language
        /// </summary>
        public StiLanguageType Language
        {
            get
            {
                return language;
            }
            set
            {
                language = value;
                LanguageStr = StiGisCoreHelper.EnumToString(value);
            }
        }

        internal StiFontIcons Icon { get; set; }

        internal Color IconColor { get; set; }

        internal bool ShowPlacemark { get; set; }

        public StiGeoRenderMode RenderMode { get; }

        public StiGeoMouseWheelZoomType MouseWheelZoomType { get; set; } = StiGeoMouseWheelZoomType.MousePositionAndCenter;

        private int zoom = 8;
        public int Zoom
        {
            get
            {
                return zoom;
            }
            set
            {
                if (zoom != value && !IsDragging)
                {
                    zoom = value;

                    minOfTiles = provider.Projection.GetTileMatrixMinXY(value);
                    maxOfTiles = provider.Projection.GetTileMatrixMaxXY(value);

                    positionPixel = provider.Projection.FromLatLngToPixel(Position, value);

                    if (IsStarted || this.isSimpleMode)
                    {
                        CancelAsyncTasks();

                        Matrix.ClearLevelsBelove(zoom - LevelsKeepInMemmory);
                        Matrix.ClearLevelsAbove(zoom + LevelsKeepInMemmory);

                        GoToCurrentPositionOnZoom();
                        UpdateBounds();

                        OnMapZoomChanged?.Invoke();
                    }
                }
            }
        }

        public StiGisPointLatLng Position
        {
            get
            {

                return position;
            }
            set
            {
                position = value;
                positionPixel = provider.Projection.FromLatLngToPixel(value, Zoom);

                if (IsStarted)
                {
                    if (!IsDragging)
                    {
                        GoToCurrentPosition();
                    }

                    OnCurrentPositionChanged?.Invoke(position);
                }
            }
        }

        private StiGisMapProvider provider;
        public StiGisMapProvider Provider
        {
            get
            {
                return provider;
            }
            set
            {
                if (provider == null || provider.ProviderType != value.ProviderType)
                {
                    bool diffProjection = (provider == null || provider.Projection != value.Projection);

                    provider = value;

                    if (!provider.IsInitialized)
                    {
                        provider.IsInitialized = true;
                        provider.OnInitialized();
                    }

                    if (provider.Projection != null && diffProjection)
                    {
                        tileRect = new StiGisRect(StiGisPoint.Empty, new StiGisSize(provider.Projection.TileSize, provider.Projection.TileSize));

                        minOfTiles = provider.Projection.GetTileMatrixMinXY(Zoom);
                        maxOfTiles = provider.Projection.GetTileMatrixMaxXY(Zoom);
                        positionPixel = provider.Projection.FromLatLngToPixel(Position, Zoom);
                    }

                    if (IsStarted || this.isSimpleMode)
                    {
                        CancelAsyncTasks();
                        if (diffProjection)
                            OnMapSizeChanged(Width, Height);

                        ReloadMap();

                        zoomToArea = true;

                        if (provider.Area.HasValue && !provider.Area.Value.Contains(Position))
                        {
                            SetZoomToFitRect(provider.Area.Value);
                            zoomToArea = false;
                        }

                        this.OnMapTypeChanged?.Invoke(value);
                    }
                }
            }
        }

        public StiGisRectLatLng ViewArea
        {
            get
            {
                if (provider != null && provider.Projection != null)
                {
                    var p = FromLocalToLatLng(0, 0);
                    var p2 = FromLocalToLatLng(Width, Height);

                    return StiGisRectLatLng.FromLTRB(p.Lng, p.Lat, p2.Lng, p2.Lat);
                }

                return StiGisRectLatLng.Empty;
            }
        }
        #endregion

        #region Methods
        internal Color GetIconColorGdi() => IconColor;

        internal System.Windows.Media.Brush GetIconColorWpf() => Extensions.ToWpfBrush(IconColor);

        internal void FullCleanMemoryCache()
        {
            memoryCache.Clear();
        }

        internal void InitGeometryColor(Color color, double lineSize)
        {
            this.GeometryColor = Color.FromArgb(255, color);
            this.GeometryLineSize = lineSize;
        }

        internal void OnRefresh()
        {
            this.refreshEvent?.Set();
        }

        internal void SetZoomToFitRect(StiGisRectLatLng rect)
        {
            int maxZoom = GetMaxZoomToFitRect(rect);
            if (maxZoom > 0)
            {
                this.Position = new StiGisPointLatLng(rect.Lat - (rect.HeightLat / 2), rect.Lng + (rect.WidthLng / 2));

                if (maxZoom > provider.MaxZoom)
                    maxZoom = provider.MaxZoom;

                if (Zoom != maxZoom)
                    Zoom = maxZoom;
            }
        }

        internal BackgroundWorker OpenMap()
        {
            if (!IsStarted)
            {
                IsStarted = true;
                GoToCurrentPosition();

                invalidateWorker = new BackgroundWorker
                {
                    WorkerSupportsCancellation = true,
                    WorkerReportsProgress = true
                };
                invalidateWorker.DoWork += new DoWorkEventHandler(InvalidatorWatch);
                invalidateWorker.RunWorkerAsync();
            }

            return invalidateWorker;
        }

        private void UpdateCenterTileXYLocation()
        {
            var center = FromLocalToLatLng(Width / 2, Height / 2);
            var centerPixel = provider.Projection.FromLatLngToPixel(center, Zoom);
            centerTileXYLocation = provider.Projection.FromPixelToTileXY(centerPixel);
        }

        public void OnMapSizeChanged(int width, int height)
        {
            this.Width = width;
            this.Height = height;

            sizeOfMapArea.Width = (Width / provider.Projection.TileSize);
            sizeOfMapArea.Height = (Height / provider.Projection.TileSize);

            if (sizeOfMapArea.Width == 0)
                sizeOfMapArea.Width = 1;
            if (sizeOfMapArea.Height == 0)
                sizeOfMapArea.Height = 1;

            sizeOfMapArea.Width++;
            sizeOfMapArea.Height++;

            if (IsStarted)
            {
                UpdateBounds();
                GoToCurrentPosition();
            }
        }

        public StiGisPointLatLng FromLocalToLatLng(int x, int y)
        {
            var p = new StiGisPoint(x, y);
            p.OffsetNegative(renderOffset);
            p.Offset(compensationOffset);

            return provider.Projection.FromPixelToLatLng(p, Zoom);
        }

        public StiGisPoint FromLatLngToLocal(StiGisPointLatLng latlng)
        {
            var pLocal = provider.Projection.FromLatLngToPixel(latlng, Zoom);
            pLocal.Offset(renderOffset);
            pLocal.OffsetNegative(compensationOffset);
            return pLocal;
        }

        public int GetMaxZoomToFitRect(StiGisRectLatLng rect)
        {
            int zoom = provider.MinZoom;

            if (rect.HeightLat == 0 || rect.WidthLng == 0)
            {
                zoom = provider.MaxZoom / 2;
            }
            else
            {
                for (int i = zoom; i <= provider.MaxZoom; i++)
                {
                    var p1 = provider.Projection.FromLatLngToPixel(rect.LocationTopLeft, i);
                    var p2 = provider.Projection.FromLatLngToPixel(rect.LocationRightBottom, i);

                    if (((p2.X - p1.X) <= Width + 10) && (p2.Y - p1.Y) <= Height + 10)
                    {
                        zoom = i;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return zoom;
        }

        public void BeginDrag(StiGisPoint pt)
        {
            dragPoint.X = pt.X - renderOffset.X;
            dragPoint.Y = pt.Y - renderOffset.Y;
            IsDragging = true;
        }

        public void EndDrag()
        {
            IsDragging = false;
            mouseDown = StiGisPoint.Empty;

            refreshEvent.Set();
        }

        public void ReloadMap()
        {
            if (IsStarted || this.isSimpleMode)
            {
                okZoom = 0;
                skipOverZoom = 0;

                CancelAsyncTasks();

                Matrix.ClearAllLevels();

                if (!this.isSimpleMode)
                    refreshEvent.Set();

                UpdateBounds();
            }
            else
            {
                throw new Exception("Please, do not call ReloadMap before form is loaded, it's useless");
            }
        }

        public void GoToCurrentPosition()
        {
            compensationOffset = positionPixel;
            renderOffset = StiGisPoint.Empty;
            dragPoint = StiGisPoint.Empty;

            var d = new StiGisPoint(Width / 2, Height / 2);

            this.Drag(d);
        }

        internal void GoToCurrentPositionOnZoom()
        {
            compensationOffset = positionPixel;

            // reset stuff
            renderOffset = StiGisPoint.Empty;
            dragPoint = StiGisPoint.Empty;

            // goto location and centering
            if (MouseWheelZooming)
            {
                if (MouseWheelZoomType != StiGeoMouseWheelZoomType.MousePositionWithoutCenter)
                {
                    var pt = new StiGisPoint(-(positionPixel.X - Width / 2), -(positionPixel.Y - Height / 2));
                    pt.Offset(compensationOffset);
                    renderOffset.X = pt.X - dragPoint.X;
                    renderOffset.Y = pt.Y - dragPoint.Y;
                }
                else // without centering
                {
                    renderOffset.X = -positionPixel.X - dragPoint.X;
                    renderOffset.Y = -positionPixel.Y - dragPoint.Y;
                    renderOffset.Offset(mouseLastZoom);
                    renderOffset.Offset(compensationOffset);
                }
            }
            else // use current map center
            {
                mouseLastZoom = StiGisPoint.Empty;

                var pt = new StiGisPoint(-(positionPixel.X - Width / 2), -(positionPixel.Y - Height / 2));
                pt.Offset(compensationOffset);
                renderOffset.X = pt.X - dragPoint.X;
                renderOffset.Y = pt.Y - dragPoint.Y;
            }

            UpdateCenterTileXYLocation();
        }

        public void DragOffset(StiGisPoint offset)
        {
            renderOffset.Offset(offset);

            UpdateCenterTileXYLocation();

            if (centerTileXYLocation != centerTileXYLocationLast)
            {
                centerTileXYLocationLast = centerTileXYLocation;
                UpdateBounds();
            }

            IsDragging = true;
            Position = FromLocalToLatLng((int)Width / 2, (int)Height / 2);
            IsDragging = false;
        }

        public void Drag(StiGisPoint pt)
        {
            renderOffset.X = pt.X - dragPoint.X;
            renderOffset.Y = pt.Y - dragPoint.Y;

            UpdateCenterTileXYLocation();

            if (centerTileXYLocation != centerTileXYLocationLast)
            {
                centerTileXYLocationLast = centerTileXYLocation;
                UpdateBounds();
            }

            if (IsDragging)
            {
                Position = FromLocalToLatLng((int)Width / 2, (int)Height / 2);
            }
        }

        public void CancelAsyncTasks()
        {
            if (!IsStarted) return;

            lock (tileLoadQueue)
            {
                tileLoadQueue.Clear();
            }
        }

        private void UpdateBounds()
        {
            if (!IsStarted) return;

            UpdatingBounds = true;

            #region find tiles around
            TileDrawingList.Clear();

            for (int indexX = -sizeOfMapArea.Width, countI = sizeOfMapArea.Width; indexX <= countI; indexX++)
            {
                for (int indexY = -sizeOfMapArea.Height, countJ = sizeOfMapArea.Height; indexY <= countJ; indexY++)
                {
                    var p = centerTileXYLocation;
                    p.X += indexX;
                    p.Y += indexY;

                    if (p.X >= minOfTiles.Width && p.Y >= minOfTiles.Height && p.X <= maxOfTiles.Width && p.Y <= maxOfTiles.Height)
                    {
                        var dt = new StiGisDrawTile()
                        {
                            PosXY = p,
                            PosPixel = new StiGisPoint(p.X * tileRect.Width, p.Y * tileRect.Height),
                            DistanceSqr = (centerTileXYLocation.X - p.X) * (centerTileXYLocation.X - p.X) + (centerTileXYLocation.Y - p.Y) * (centerTileXYLocation.Y - p.Y)
                        };

                        if (!TileDrawingList.Contains(dt))
                        {
                            TileDrawingList.Add(dt);
                        }
                    }
                }
            }

            TileDrawingList.Sort();
            #endregion

            lock (tileLoadQueue)
            {
                foreach (var p in TileDrawingList)
                {
                    if (!this.Matrix.Contains(this.Zoom, p.PosXY))
                    {
                        var task = new StiGisLoadTask(this, p.PosXY, Zoom);
                        if (!tileLoadQueue.Contains(task))
                        {
                            tileLoadQueue.Push(task);
                        }
                    }
                }

                #region starts loader threads if needed
                lock (ThreadTaskPool)
                {
                    while (tileLoadQueue.Count > 0 && ThreadTaskPool.Count < GThreadPoolSize)
                    {
                        var task = tileLoadQueue.Pop();
                        ThreadTaskPool.Add(task);
                        task.RunInThread();
                    }
                }
                #endregion
            }

            UpdatingBounds = false;
        }

        internal void UpdateBoundsInternal()
        {
            lock (tileLoadQueue)
            {
                lock (ThreadTaskPool)
                {
                    if (tileLoadQueue.Count == 0)
                    {
                        this.memoryCache.ClearMemory(this.zoom);
                    }
                    else
                    {
                        while (tileLoadQueue.Count > 0 && ThreadTaskPool.Count < GThreadPoolSize)
                        {
                            var task = tileLoadQueue.Pop();
                            ThreadTaskPool.Add(task);
                            task.RunInThread();
                        }
                    }
                }
            }
        }

        public StiGisMapImage GetImageFrom(StiGisMapProvider provider, StiGisPoint pos, int zoom, out Exception error)
        {
            error = null;

            try
            {
                var tile = new StiGisRawTile(provider.ProviderType, pos, zoom);

                // check in memory cache
                var buffer = memoryCache.Get(tile);
                if (buffer != null)
                {
                    var cacheResult = StiGisMapImage.FromByteArray(buffer, this.RenderMode);
                    if (cacheResult != null)
                        return cacheResult;
                }

                // check in local cache
                if (this.AllowLocalCache)
                {
                    var cacheImage = StiGisLocalCacheCache.Get(provider.ProviderType, this.language, this.RenderMode, pos, zoom);
                    if (cacheImage != null)
                    {
                        memoryCache.Add(tile, cacheImage.Data);
                        return cacheImage;
                    }
                }

                var result = provider.GetTileImage(pos, zoom, this.RenderMode);
                if (result != null && result.Data != null)
                {
                    if (this.AllowLocalCache)
                    {
                        StiGisLocalCacheCache.Set(result.Data, provider.ProviderType, this.language, pos, zoom);
                    }

                    memoryCache.Add(tile, result.Data);
                    return result;
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            return null;
        }
        #endregion

        #region Events
        public event StiGisPositionChanged OnCurrentPositionChanged;
        public event StiGisMapZoomChanged OnMapZoomChanged;
        public event StiGisMapTypeChanged OnMapTypeChanged;
        #endregion

        #region Handlers
        private void InvalidatorWatch(object sender, DoWorkEventArgs e)
        {
            var w = (BackgroundWorker)sender;

            while (refreshEvent != null)
            {
                refreshEvent.WaitOne();
                w.ReportProgress(1);
            }
        }
        #endregion

        #region IDisposable.override
        public void Dispose(bool disposing)
        {
            if (IsStarted)
            {
                if (invalidateWorker != null)
                {
                    invalidateWorker.CancelAsync();
                    invalidateWorker.DoWork -= new DoWorkEventHandler(InvalidatorWatch);
                    invalidateWorker.Dispose();
                    invalidateWorker = null;
                }

                if (refreshEvent != null)
                {
                    refreshEvent.Set();
                    refreshEvent.Close();
                    refreshEvent = null;
                }

                CancelAsyncTasks();
                IsStarted = false;

                if (Matrix != null)
                {
                    Matrix.Dispose();
                    Matrix = null;
                }

                TileDrawingList.Clear();

                // cancel waiting loaders
                lock (tileLoadQueue)
                {
                    tileLoadQueue.Clear();
                }

                this.memoryCache?.Clear();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}