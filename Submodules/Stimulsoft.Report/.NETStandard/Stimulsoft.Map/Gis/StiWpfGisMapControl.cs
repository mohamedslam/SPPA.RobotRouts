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

using Stimulsoft.Base;
using Stimulsoft.Base.Gis;
using Stimulsoft.Map.Gis.Core;
using Stimulsoft.Map.Gis.Geography;
using Stimulsoft.Map.Gis.Projections;
using Stimulsoft.Map.Gis.Providers;
using Stimulsoft.Map.Gis.Providers.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Stimulsoft.Report.Helpers;

#if NETSTANDARD
using Stimulsoft.System.ComponentModel;
using Stimulsoft.System.Windows;
using Stimulsoft.System.Windows.Controls;
using Stimulsoft.System.Windows.Input;
using Stimulsoft.System.Windows.Media;
#else
using System.Windows.Controls;
using System.Windows.Media;
#endif

namespace Stimulsoft.Map.Gis
{
    [DesignTimeVisible(false)]
    public sealed class StiWpfGisMapControl : 
        Control,
        IStiGisMapControl,
        IDisposable
    {
        public StiWpfGisMapControl(object fontFamily)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            tileFlipXYAttributes.SetWrapMode(WrapMode.TileFlipXY);

            this.viewData = new StiGisMapViewData(this.core);
            this.ClipToBounds = true;

            this.core.WpfFontFamily = fontFamily as System.Windows.Media.FontFamily;

            this.Loaded += This_Loaded;
            this.SizeChanged += This_SizeChanged;
        }

        #region Fields
        private StiGisMapViewData viewData;
        private Brush selectedAreaFill = new SolidColorBrush(Extensions.FromArgb(33, Colors.RoyalBlue));
        private StiGisCore core = new StiGisCore(StiGeoRenderMode.Wpf);
        private ImageAttributes tileFlipXYAttributes = new ImageAttributes();
        private Cursor cursorBefore = Cursors.Arrow;
        private bool mouseIn;
        #endregion

        #region Properties
        public int MaxZoom => this.core.Provider.MaxZoom;

        public int MinZoom => this.core.Provider.MinZoom;

        public StiGeoMouseWheelZoomType MouseWheelZoomType => core.MouseWheelZoomType;

        private double zoomReal = 1;
        public double Zoom
        {
            get
            {
                return zoomReal;
            }
            set
            {
                if (zoomReal != value)
                {
                    if (value > MaxZoom)
                        zoomReal = MaxZoom;
                    else if (value < MinZoom)
                        zoomReal = MinZoom;
                    else
                        zoomReal = value;

                    SetZoomStep((int)Math.Floor(value));

                    if (core.IsStarted && !IsDragging)
                    {
                        ForceUpdateOverlays();
                    }
                }
            }
        }

        public StiGisPointLatLng Position
        {
            get
            {
                return core.Position;
            }
            set
            {
                core.Position = value;

                if (core.IsStarted)
                {
                    ForceUpdateOverlays();
                }
            }
        }

        public bool AllowLocalCache
        {
            get
            {
                return this.core.AllowLocalCache;
            }
            set
            {
                this.core.AllowLocalCache = value;
            }
        }

        private bool IsDragging { get; set; }

        public StiGisRectLatLng ViewArea => core.ViewArea;

        public StiGisMapProvider Provider
        {
            get
            {
                return core.Provider;
            }
            set
            {
                if (core.Provider == null || !core.Provider.Equals(value))
                {
                    var viewarea = ViewArea;

                    core.Provider = value;

                    if (core.IsStarted)
                    {
                        if (core.zoomToArea)
                        {
                            // restore zoomrect as close as possible
                            if (viewarea != StiGisRectLatLng.Empty && viewarea != ViewArea)
                            {
                                int bestZoom = core.GetMaxZoomToFitRect(viewarea);
                                if (bestZoom > 0 && Zoom != bestZoom)
                                {
                                    Zoom = bestZoom;
                                }
                            }
                        }
                        else
                        {
                            ForceUpdateOverlays();
                        }
                    }
                }
            }
        }
        #endregion

        #region IDisposable.override
        public void Dispose()
        {
            this.core.Dispose();
            this.viewData.Dispose();
        }
        #endregion

        #region Methods.override
        private void This_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= This_Loaded;

            var worker = this.core.OpenMap();

            worker.ProgressChanged += new ProgressChangedEventHandler(Map_ProgressChanged);

            core.OnMapSizeChanged((int)this.ActualWidth, (int)this.ActualHeight);

            ForceUpdateOverlays();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            DrawGraphics(drawingContext);
        }

        private void This_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            if (this.ActualWidth == 0 || this.ActualHeight == 0)
                return;
            if (this.ActualWidth == core.Width && this.ActualHeight == core.Height)
                return;

            core.OnMapSizeChanged((int)this.ActualWidth, (int)this.ActualHeight);

            if (Visibility == Visibility.Visible && core.IsStarted)
            {
                ForceUpdateOverlays();
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.ChangedButton == MouseButton.Left)
            {
                var pos = e.GetPosition(this);
                core.mouseDown = ApplyRotationInversion((int)pos.X, (int)pos.Y);
                this.InvalidateVisual();
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (core.IsDragging)
            {
                if (IsDragging)
                {
                    IsDragging = false;
                    this.Cursor = cursorBefore;
                    cursorBefore = null;
                }
                core.EndDrag();
            }
            else
            {
                
                if (e.ChangedButton == MouseButton.Left)
                {
                    core.mouseDown = StiGisPoint.Empty;
                }

                InvalidateVisual();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!core.IsDragging && !core.mouseDown.IsEmpty)
            {
                var pos = e.GetPosition(this);
                var p = ApplyRotationInversion((int)pos.X, (int)pos.Y);
                if (Math.Abs(p.X - core.mouseDown.X) * 2 >= System.Windows.Forms.SystemInformation.DragSize.Width || Math.Abs(p.Y - core.mouseDown.Y) * 2 >= System.Windows.Forms.SystemInformation.DragSize.Height)
                {
                    core.BeginDrag(core.mouseDown);
                }
            }

            if (core.IsDragging)
            {
                if (!IsDragging)
                {
                    IsDragging = true;

                    cursorBefore = this.Cursor;
                    this.Cursor = Cursors.SizeAll;
                }

                var pos = e.GetPosition(this);
                core.mouseCurrent = ApplyRotationInversion((int)pos.X, (int)pos.Y);
                core.Drag(core.mouseCurrent);

                base.InvalidateVisual();
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            Focus();
            mouseIn = true;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            mouseIn = false;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (mouseIn && !core.IsDragging)
            {
                var pos = e.GetPosition(this);

                if (core.mouseLastZoom.X != pos.X && core.mouseLastZoom.Y != pos.Y)
                {
                    if (MouseWheelZoomType == StiGeoMouseWheelZoomType.MousePositionAndCenter)
                    {
                        core.position = FromLocalToLatLng((int)pos.X, (int)pos.Y);
                    }
                    else if (MouseWheelZoomType == StiGeoMouseWheelZoomType.ViewCenter)
                    {
                        core.position = FromLocalToLatLng((int)this.ActualWidth / 2, (int)this.ActualHeight / 2);
                    }
                    else if (MouseWheelZoomType == StiGeoMouseWheelZoomType.MousePositionWithoutCenter)
                    {
                        core.position = FromLocalToLatLng((int)pos.X, (int)pos.Y);
                    }

                    core.mouseLastZoom.X = (int)pos.X;
                    core.mouseLastZoom.Y = (int)pos.Y;
                }

                // set mouse position to map center
                if (MouseWheelZoomType != StiGeoMouseWheelZoomType.MousePositionWithoutCenter)
                {
                    var p = PointToScreen(new Point(this.ActualWidth / 2, this.ActualHeight / 2));
                    StiGisCoreHelper.SetCursorPos((int)p.X, (int)p.Y);
                }

                core.MouseWheelZooming = true;

                if (e.Delta > 0)
                {
                    Zoom = ((int)Zoom) + 1;
                }
                else if (e.Delta < 0)
                {
                    Zoom = ((int)(Zoom + 0.99)) - 1;
                }

                core.MouseWheelZooming = false;
            }
        }
        #endregion

        #region Methods
        public void SetProviderType(StiGeoMapProviderType type, global::System.Drawing.Color geometryColor, 
            double geometryLineSize, bool showPlacemark, StiLanguageType language, StiFontIcons icon, global::System.Drawing.Color iconColor)
        {
            this.core.InitGeometryColor(geometryColor, geometryLineSize);
            this.Provider = StiGisMapProviderHelper.CreateProvider(type);
            this.core.ShowPlacemark = showPlacemark;
            this.core.Language = language;
            this.core.Icon = icon;
            this.core.IconColor = iconColor;
            this.Provider.LanguageStr = this.core.LanguageStr;
        }

        public void InitGeoCommands(List<string> commands, List<string> colors, List<object> lineSizes, List<string> descriptions)
        {
            if (commands == null || commands.Count == 0) return;

            this.viewData.Clear();

            #region test
            //var commands1 = new List<string>()
            //{
            //    "POINT(-112.93945312500003 30.97760909334869)",
            //    "POINT(-101.95312499999999 29.6116701151974)",
            //    "POINT(-106.611328125 41.211721510547875)",
            //    "POINT(-116.630859375 41.211721510547875)",
            //    "POLYGON((-127.177734375 58.67693767258692,-92.02148437499999 50.68079714532166,-84.111328125 58.26328705248602,-127.177734375 58.67693767258692))",
            //    "POLYGON((-117.77343750000003 43.45291889355465,-104.45800781249997 31.128199299111955,-86.83593749999997 43.580390855607845,-117.77343750000003 43.45291889355465),(-110.830078125 41.44272637767213,-103.8427734375 35.49645605658418,-93.603515625 41.541477666790286,-110.830078125 41.44272637767213))",
            //};

            //viewData.Markers.Add(new GisMarkerMapGeometry(new GisPointLatLng(29.6761217847243, -85.36102294921882), this.Core));
            //viewData.Markers.Add(new GisMarkerMapGeometry(new GisPointLatLng(31.380606373669707, -81.27891540527341), this.Core));
            //viewData.Markers.Add(new GisMarkerMapGeometry(new GisPointLatLng(25.66752255134429, -80.15899658203132), this.Core));
            #endregion

            new StiGisCommandsParser(commands, colors, lineSizes, descriptions, viewData).Parse();

            var rect = GetRectOfAllGeoms();
            if (rect != null)
                this.core.SetZoomToFitRect(rect.Value);

            ForceUpdateOverlays();
            this.InvalidateVisual();
        }

        private void DrawGraphics(DrawingContext dc)
        {
            dc.DrawRectangle(Brushes.WhiteSmoke, null, new Rect(0, 0, this.ActualWidth, this.ActualHeight));
            dc.PushTransform(new TranslateTransform(core.renderOffset.X, core.renderOffset.Y));

            DrawMap(dc);
            DrawOverlays(dc);
        }

        private void DrawMap(DrawingContext dc)
        {
            if (core.UpdatingBounds || Provider == null)
                return;

            foreach (var tilePoint in core.TileDrawingList)
            {
                core.tileRect.Location = tilePoint.PosPixel;
                core.tileRect.OffsetNegative(core.compensationOffset);

                var tile = core.Matrix.Get(core.Zoom, tilePoint.PosXY);
                if (tile.NotEmpty)
                {
                    if (tile.Image != null && tile.Image.Data != null)
                    {
                        var bi = new System.Windows.Media.Imaging.BitmapImage();
                        bi.BeginInit();
                        bi.StreamSource = new MemoryStream(tile.Image.Data);
                        bi.EndInit();

                        dc.DrawImage(bi, new Rect(core.tileRect.X, core.tileRect.Y, core.tileRect.Width, core.tileRect.Height));
                    }
                }
                else if (Provider.Projection is StiMercatorGisProjection)
                {
                    #region fill empty lines
                    int zoomOffset = 1;
                    var parentTile = StiGisTile.Empty;
                    int ix = 0;

                    while (!parentTile.NotEmpty && zoomOffset < core.Zoom && zoomOffset <= StiGisCore.LevelsKeepInMemmory)
                    {
                        ix = (int)Math.Pow(2, zoomOffset);
                        parentTile = core.Matrix.Get(core.Zoom - zoomOffset++, new StiGisPoint((int)(tilePoint.PosXY.X / ix), (int)(tilePoint.PosXY.Y / ix)));
                    }

                    if (parentTile.NotEmpty)
                    {
                        int xOff = Math.Abs(tilePoint.PosXY.X - (parentTile.Pos.X * ix));
                        int yOff = Math.Abs(tilePoint.PosXY.Y - (parentTile.Pos.Y * ix));

                        // render tile 
                        if (parentTile.Image != null && parentTile.Image.Data != null)
                        {
                            var bi = new System.Windows.Media.Imaging.BitmapImage();
                            bi.BeginInit();
                            bi.StreamSource = new MemoryStream(parentTile.Image.Data);
                            bi.EndInit();

                            var srcRect = new Rect((float)(xOff * (bi.Width / ix)), (float)(yOff * (bi.Height / ix)), (bi.Width / ix), (bi.Height / ix));
                            var dst = new Rect((int)core.tileRect.X, (int)core.tileRect.Y, (int)core.tileRect.Width, (int)core.tileRect.Height);

                            dc.DrawImage(bi, new Rect(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height));
                            dc.DrawRectangle(selectedAreaFill, null, dst);
                        }
                    }
                    #endregion
                }
            }
        }

        private void DrawOverlays(DrawingContext dc)
        {
            this.viewData.Draw(dc);
            dc.Pop();

            #region Copyright
            if (!string.IsNullOrEmpty(core.Provider.Copyright))
            {
#if NETCOREAPP
                var ft = new FormattedText(core.Provider.Copyright, CultureInfo.CurrentCulture, this.FlowDirection, 
                    new Typeface("Arial"), 7, Brushes.Navy, StiScale.Factor);
#else
                var ft = new FormattedText(core.Provider.Copyright, CultureInfo.CurrentCulture, this.FlowDirection, 
                    new Typeface("Arial"), 7, Brushes.Navy);
#endif
                dc.DrawText(ft, new Point(3, this.ActualHeight - ft.Height - 5));
            }
            #endregion
        }

        private void SetZoomStep(int value)
        {
            if (value > MaxZoom)
            {
                core.Zoom = MaxZoom;
            }
            else if (value < MinZoom)
            {
                core.Zoom = MinZoom;
            }
            else
            {
                core.Zoom = value;
            }
        }

        internal void ForceUpdateOverlays()
        {
            this.viewData.ForceUpdate();

            this.InvalidateVisual();
        }

        private StiGisRectLatLng? GetRectOfAllGeoms()
        {
            var points = viewData.GetAllPoints();
            if (points.Count == 0) return null;

            double left = double.MaxValue;
            double top = double.MinValue;
            double right = double.MinValue;
            double bottom = double.MaxValue;

            foreach (var point in points)
            {
                // left
                if (point.Lng < left)
                    left = point.Lng;

                // top
                if (point.Lat > top)
                    top = point.Lat;

                // right
                if (point.Lng > right)
                    right = point.Lng;

                // bottom
                if (point.Lat < bottom)
                    bottom = point.Lat;
            }

            if (left != double.MaxValue && right != double.MinValue && top != double.MinValue && bottom != double.MaxValue)
                return StiGisRectLatLng.FromLTRB(left, top, right, bottom);

            return null;
        }

        private StiGisPoint ApplyRotationInversion(int x, int y) => new StiGisPoint(x, y);

        internal void RestoreCursorOnLeave()
        {
            if (cursorBefore != null)
            {
                this.Cursor = this.cursorBefore;
                cursorBefore = null;
            }
        }

        private StiGisPointLatLng FromLocalToLatLng(int x, int y) => core.FromLocalToLatLng(x, y);
        #endregion

        #region Handlers
        private void Map_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            InvalidateVisual();
        }
        #endregion
    }
}