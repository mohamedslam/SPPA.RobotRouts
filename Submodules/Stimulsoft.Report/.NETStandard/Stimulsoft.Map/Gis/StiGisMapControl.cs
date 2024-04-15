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
using Stimulsoft.Report.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Map.Gis
{
    [ToolboxItem(false)]
    public sealed partial class StiGisMapControl : 
        UserControl,
        IStiGisMapControl
    {
        public StiGisMapControl()
        {
            if (DesignMode) return;

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.Opaque, true);
            this.ResizeRedraw = true;
            this.Dock = DockStyle.Fill;

            tileFlipXYAttributes.SetWrapMode(WrapMode.TileFlipXY);
            
            this.viewData = new StiGisMapViewData(this.core);
        }

        #region Fields
        private StiGisMapViewData viewData;
        private StiGisMapToolTip toolTipMain;
        private Brush selectedAreaFill = new SolidBrush(Color.FromArgb(33, Color.RoyalBlue));
        private StiGisCore core = new StiGisCore(StiGeoRenderMode.Gdi);
        private ImageAttributes tileFlipXYAttributes = new ImageAttributes();
        private Cursor cursorBefore = Cursors.Default;
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

        #region Methods.override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode) return;

            var worker = this.core.OpenMap();
            worker.ProgressChanged += new ProgressChangedEventHandler(Map_ProgressChanged);

            core.OnMapSizeChanged(Width, Height);

            ForceUpdateOverlays();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                core.Dispose();

                this.toolTipMain?.Dispose();
                this.viewData.Dispose();
                this.selectedAreaFill.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            DrawGraphics(e.Graphics);

            base.OnPaint(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            
            if (Width == 0 || Height == 0)
                return;
            if (Width == core.Width && Height == core.Height)
                return;

            if (!DesignMode)
            {
                core.OnMapSizeChanged(Width, Height);

                if (Visible && IsHandleCreated && core.IsStarted)
                {
                    ForceUpdateOverlays();
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                core.mouseDown = ApplyRotationInversion(e.X, e.Y);
                this.Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
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
                if (e.Button == MouseButtons.Left)
                {
                    core.mouseDown = StiGisPoint.Empty;
                }

                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!core.IsDragging && !core.mouseDown.IsEmpty)
            {
                var p = ApplyRotationInversion(e.X, e.Y);
                if (Math.Abs(p.X - core.mouseDown.X) * 2 >= SystemInformation.DragSize.Width || Math.Abs(p.Y - core.mouseDown.Y) * 2 >= SystemInformation.DragSize.Height)
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

                core.mouseCurrent = ApplyRotationInversion(e.X, e.Y);
                core.Drag(core.mouseCurrent);

                base.Invalidate();
            }

            if (core.ShowPlacemark)
            {
                var pos = new Point(e.X - core.renderOffset.X, e.Y - core.renderOffset.Y);

                bool isFind = false;
                foreach (var placemark in viewData.Placemarks)
                {
                    if (placemark.Contains(pos))
                    {
                        if (toolTipMain == null)
                            toolTipMain = new StiGisMapToolTip();

                        toolTipMain.Body = placemark.Text;

                        if (toolTipMain.GetToolTip(this) != "key")
                            toolTipMain.SetToolTip(this, "key");

                        isFind = true;
                        break;
                    }
                }

                if (!isFind)
                    toolTipMain?.Hide(this);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            Focus();
            mouseIn = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            mouseIn = false;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (mouseIn && !core.IsDragging)
            {
                if (core.mouseLastZoom.X != e.X && core.mouseLastZoom.Y != e.Y)
                {
                    if (MouseWheelZoomType == StiGeoMouseWheelZoomType.MousePositionAndCenter)
                    {
                        core.position = FromLocalToLatLng(e.X, e.Y);
                    }
                    else if (MouseWheelZoomType == StiGeoMouseWheelZoomType.ViewCenter)
                    {
                        core.position = FromLocalToLatLng(Width / 2, Height / 2);
                    }
                    else if (MouseWheelZoomType == StiGeoMouseWheelZoomType.MousePositionWithoutCenter)
                    {
                        core.position = FromLocalToLatLng(e.X, e.Y);
                    }

                    core.mouseLastZoom.X = e.X;
                    core.mouseLastZoom.Y = e.Y;
                }

                // set mouse position to map center
                if (MouseWheelZoomType != StiGeoMouseWheelZoomType.MousePositionWithoutCenter)
                {
                    var p = PointToScreen(new Point(Width / 2, Height / 2));
                    StiGisCoreHelper.SetCursorPos(p.X, p.Y);
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
        public void SetProviderType(StiGeoMapProviderType type, Color geometryColor, 
            double geometryLineSize, bool showPlacemark, StiLanguageType language, StiFontIcons icon, Color iconColor)
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
            this.Invalidate();
        }

        private void DrawGraphics(Graphics g)
        {
            g.Clear(Color.WhiteSmoke);
            g.TranslateTransform(core.renderOffset.X, core.renderOffset.Y);

            DrawMap(g);
            DrawOverlays(g);
        }

        private void DrawMap(Graphics g)
        {
            if (core.UpdatingBounds || Provider == null)
                return;

            var scaleValue = (StiScale.Factor == 1.0) ? 0 : StiScale.I2;
            foreach (var tilePoint in core.TileDrawingList)
            {
                core.tileRect.Location = tilePoint.PosPixel;
                core.tileRect.OffsetNegative(core.compensationOffset);

                var t = core.Matrix.Get(core.Zoom, tilePoint.PosXY);
                if (t.NotEmpty)
                {
                    if (t.Image != null && t.Image.BitmapGdi != null)
                    {
                        g.DrawImage(t.Image.BitmapGdi, core.tileRect.X, core.tileRect.Y, core.tileRect.Width + scaleValue, core.tileRect.Height + scaleValue);
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
                        if (parentTile.Image != null && parentTile.Image.BitmapGdi != null)
                        {
                            var srcRect = new RectangleF((float)(xOff * (parentTile.Image.BitmapGdi.Width / ix)), (float)(yOff * (parentTile.Image.BitmapGdi.Height / ix)), (parentTile.Image.BitmapGdi.Width / ix), (parentTile.Image.BitmapGdi.Height / ix));
                            var dst = new Rectangle((int)core.tileRect.X, (int)core.tileRect.Y, (int)core.tileRect.Width, (int)core.tileRect.Height);

                            g.DrawImage(parentTile.Image.BitmapGdi, dst, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, tileFlipXYAttributes);
                            g.FillRectangle(selectedAreaFill, dst);
                        }
                    }
                    #endregion
                }
            }
        }

        private void DrawOverlays(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;
            this.viewData.Draw(g);
            g.ResetTransform();

            #region Copyright
            if (!string.IsNullOrEmpty(core.Provider.Copyright))
            {
                using (var copyrightFont = new Font(FontFamily.GenericSansSerif, 7, FontStyle.Regular))
                {
                    g.DrawString(core.Provider.Copyright, copyrightFont, Brushes.Navy, 3, Height - copyrightFont.Height - 5);
                }
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

            Refresh();
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

        private StiGisPoint ApplyRotationInversion(int x, int y)
        {
            return new StiGisPoint(x, y);
        }

        internal void RestoreCursorOnLeave()
        {
            if (cursorBefore != null)
            {
                this.Cursor = this.cursorBefore;
                cursorBefore = null;
            }
        }

        private StiGisPointLatLng FromLocalToLatLng(int x, int y)
        {
            return core.FromLocalToLatLng(x, y);
        }
        #endregion

        #region Handlers
        private void Map_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            base.Invalidate();
        }
        #endregion
    }
}