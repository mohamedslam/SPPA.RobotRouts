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
using Stimulsoft.Map.Gis.Providers.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;

namespace Stimulsoft.Map.Gis
{
    public sealed class StiGisMapSimpleView :
        IStiGisMapSimpleView
    {
        public StiGisMapSimpleView(StiGeoMapProviderType provider, StiGisMapData data, StiGisMapRepaintHelper.InvokeTickDelegate invokeTick)
        {
            this.provider = provider;
            this.size = data.Size;
            this.commands = data.Commands;
            this.colors = data.Colors;
            this.invokeTick = invokeTick;

            this.core.Provider = StiGisMapProviderHelper.CreateProvider(provider);
            this.core.AllowLocalCache = true;
            this.core.Language = data.Language;
            this.core.Provider.LanguageStr = this.core.LanguageStr;

            this.core.InitGeometryColor(data.GeometryColor, data.GeometryLineSize);
            this.viewData = new StiGisMapViewData(this.core);
        }

        #region Fields
        private Thread thread;
        private StiGisMapRepaintHelper.InvokeTickDelegate invokeTick;
        private StiGisCore core = new StiGisCore(StiGeoRenderMode.Gdi, true);
        private StiGisMapViewData viewData;
        private List<string> commands;
        private List<string> colors;
        private List<object> lineSizes;
        private List<string> descriptions;
        private Size size;
        private StiGeoMapProviderType provider;
        private Color geometryColor;
        #endregion

        #region Properties
        public bool IsComplete { get; private set; }
        #endregion

        #region Methods
        public bool IsChanged(StiGeoMapProviderType provider, StiGisMapData data)
        {
            if (this.geometryColor != data.GeometryColor) return true;
            if (this.provider != provider) return true;
            if (this.size != data.Size) return true;
            if (this.core.ShowPlacemark != data.ShowPlacemark) return true;
            if (this.core.Language != data.Language) return true;
            if (this.core.Icon != data.Icon) return true;
            if (this.core.IconColor != data.IconColor) return true;

            if (data.Commands == null)
            {
                if (this.commands != null) return true;
            }
            else
            {
                if (this.commands == null) return true;
                if (this.commands.Count != data.Commands.Count) return true;

                for (int index = 0; index < data.Commands.Count; index++)
                {
                    if (this.commands[index] != data.Commands[index]) return true;
                }
            }

            if (data.Colors == null)
            {
                if (this.colors != null) return true;
            }
            else
            {
                if (this.colors == null) return true;
                if (this.colors.Count != data.Colors.Count) return true;

                for (int index = 0; index < data.Colors.Count; index++)
                {
                    if (this.colors[index] != data.Colors[index]) return true;
                }
            }

            if (data.LineSizes == null)
            {
                if (this.lineSizes != null) return true;
            }
            else
            {
                if (this.lineSizes == null) return true;
                if (this.lineSizes.Count != data.LineSizes.Count) return true;

                for (int index = 0; index < data.LineSizes.Count; index++)
                {
                    if (this.lineSizes[index] != data.LineSizes[index]) return true;
                }
            }

            if (data.Descriptions == null)
            {
                if (this.descriptions != null) return true;
            }
            else
            {
                if (this.descriptions == null) return true;
                if (this.descriptions.Count != data.Descriptions.Count) return true;

                for (int index = 0; index < data.Descriptions.Count; index++)
                {
                    if (this.descriptions[index] != data.Descriptions[index]) return true;
                }
            }

            return false;
        }

        public void Run()
        {
            if (thread != null)
                throw new Exception("Already running");

            thread = new Thread(new ThreadStart(Do));
            thread.Start();
        }

        public void RunAndWait()
        {
            if (thread != null)
                throw new Exception("Already running");

            thread = new Thread(new ThreadStart(Do));
            thread.Start();

            while (true)
            {
                if (thread == null || !thread.IsAlive)
                    break;

                Thread.Sleep(600);
            }
        }

        public void Restart(StiGeoMapProviderType provider, StiGisMapData data)
        {
            if (thread != null)
            {
                try
                {
                    thread.Abort();
                    thread = null;
                }
                catch { }
            }

            this.provider = provider;
            this.size = data.Size;
            this.commands = data.Commands;
            this.colors = data.Colors;
            this.lineSizes = data.LineSizes;
            this.descriptions = data.Descriptions;
            this.geometryColor = data.GeometryColor;
            this.core.ShowPlacemark = data.ShowPlacemark;
            this.core.Icon = data.Icon;
            this.core.IconColor = data.IconColor;

            if (this.core.Language != data.Language)
                this.core.FullCleanMemoryCache();

            this.core.Language = data.Language;

            this.core.InitGeometryColor(data.GeometryColor, data.GeometryLineSize);
            this.core.Provider = StiGisMapProviderHelper.CreateProvider(provider);
            this.core.Provider.LanguageStr = this.core.LanguageStr;

            thread = new Thread(new ThreadStart(Do));
            thread.Start();
        }

        private void Do()
        {
            this.IsComplete = false;
            try
            {
                this.core.Zoom = 8;

                // делаем полную имитацию метод GisMapControl.OnLoad
                ////////////////////////////////////////////////////

                core.OnMapSizeChanged(size.Width, size.Height);

                // затем сразу парсим полученные данных, которые нужно отрисовать в этой карте, чтобы заранее расчитать
                // область их вывода и заранее задать Zoom и правильную область. Это позволит не загружать лишние картинки и не тратить время и ресурсы
                InitGeoCommands();

                // затем нам надо сделать выполнение метод UpdateBounds(), который должен был выполниться в OnMapSizeChanged()
                UpdateBounds();

                // затем загружаем в память все нужные картинки для отрисовки
                LoadImages();

                // Загрузка данные завершена, теперь можно сообщить дизайнеру, что нужно перерисовать наш элемент
                this.IsComplete = true;

                this.invokeTick();
            }
            catch
            {
                this.IsComplete = false;
            }

            this.thread = null;
        }

        private void InitGeoCommands()
        {
            if (commands == null || commands.Count == 0)
                return;

            this.viewData.Clear();

            new StiGisCommandsParser(commands, colors, lineSizes, descriptions, viewData).Parse();

            var rect = GetRectOfAllGeoms();

            // т.к. IsStarted в core у нас не запущен - можно вызывать метод SetZoomToFitRect, 
            // никаких дополнительных обработок и запуска новых потоков не будет
            if (rect != null)
                this.core.SetZoomToFitRect(rect.Value);

            this.viewData.ForceUpdate();
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

        private void UpdateBounds()
        {
            #region find tiles around
            this.core.TileDrawingList.Clear();

            var screenRect = new Rectangle(new Point(0, 0), this.size);
            for (int indexX = -this.core.sizeOfMapArea.Width, countI = this.core.sizeOfMapArea.Width; indexX < countI; indexX++)
            {
                for (int indexY = -this.core.sizeOfMapArea.Height, countJ = this.core.sizeOfMapArea.Height; indexY < countJ; indexY++)
                {
                    var p = this.core.centerTileXYLocation;
                    p.X += indexX;
                    p.Y += indexY;

                    if (p.X >= this.core.minOfTiles.Width && p.Y >= this.core.minOfTiles.Height && p.X <= this.core.maxOfTiles.Width && p.Y <= this.core.maxOfTiles.Height)
                    {
                        var tileRect = new Rectangle(p.X * this.core.tileRect.Width - core.compensationOffset.X + core.renderOffset.X, 
                            p.Y * this.core.tileRect.Height - core.compensationOffset.Y + core.renderOffset.Y,
                            this.core.tileRect.Width, this.core.tileRect.Height);

                        if (screenRect.IntersectsWith(tileRect))
                        {
                            var dt = new StiGisDrawTile()
                            {
                                PosXY = p,
                                PosPixel = new StiGisPoint(p.X * this.core.tileRect.Width, p.Y * this.core.tileRect.Height),
                                DistanceSqr = (this.core.centerTileXYLocation.X - p.X) * (this.core.centerTileXYLocation.X - p.X) + (this.core.centerTileXYLocation.Y - p.Y) * (this.core.centerTileXYLocation.Y - p.Y)
                            };

                            if (!this.core.TileDrawingList.Contains(dt))
                                this.core.TileDrawingList.Add(dt);
                        }
                    }
                }
            }

            this.core.TileDrawingList.Sort();
            #endregion

            GoToCurrentPosition();
        }

        private void GoToCurrentPosition()
        {
            this.core.compensationOffset = this.core.positionPixel;
            this.core.renderOffset = StiGisPoint.Empty;
            this.core.dragPoint = StiGisPoint.Empty;

            var d = new StiGisPoint(this.core.Width / 2, this.core.Height / 2);

            this.core.renderOffset.X = d.X - this.core.dragPoint.X;
            this.core.renderOffset.Y = d.Y - this.core.dragPoint.Y;

            var center = this.core.FromLocalToLatLng(this.core.Width / 2, this.core.Height / 2);
            var centerPixel = this.core.Provider.Projection.FromLatLngToPixel(center, this.core.Zoom);
            this.core.centerTileXYLocation = this.core.Provider.Projection.FromPixelToTileXY(centerPixel);

            if (this.core.centerTileXYLocation != this.core.centerTileXYLocationLast)
            {
                this.core.centerTileXYLocationLast = this.core.centerTileXYLocation;
                UpdateBounds();
            }
        }

        private void LoadImages()
        {
            var zoom = this.core.Zoom;
            int index = 1;

            var list = this.core.TileDrawingList.ToArray();
            foreach (var p in list)
            {
                if (!this.core.Matrix.Contains(zoom, p.PosXY))
                    LoadImage(p.PosXY, zoom);

                index++;
            }
        }

        private void LoadImage(StiGisPoint pos, int zoom)
        {
            try
            {
                var m = this.core.Matrix.Get(zoom, pos);
                if (!m.NotEmpty)
                {
                    var tile = new StiGisTile(zoom, pos);

                    StiGisMapImage img = null;
                    Exception ex = null;

                    var provider = this.core.Provider;
                    if (zoom >= provider.MinZoom && zoom <= provider.MaxZoom)
                    {
                        img = this.core.GetImageFrom(provider, pos, zoom, out ex);
                    }

                    if (img != null)
                    {
                        tile.Image = img;
                        this.core.Matrix.Set(tile);
                    }
                }
            }
            catch { }
        }

        public void DrawMap(Graphics g)
        {
            if (!this.IsComplete) return;

            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TranslateTransform(core.renderOffset.X, core.renderOffset.Y);

            using (var fill = new SolidBrush(Color.WhiteSmoke))
            {
                g.FillRectangle(fill, new Rectangle(0, 0, this.size.Width, this.size.Height));
            }

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
                else if (core.Provider.Projection is StiMercatorGisProjection)
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

                            //g.DrawImage(parentTile.Image.ImageObj, dst, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, tileFlipXYAttributes);
                            //g.FillRectangle(selectedAreaFill, dst);
                        }
                    }
                    #endregion
                }

                // draw grid
                //g.DrawRectangle(Pens.Red, this.core.tileRect.X, this.core.tileRect.Y, this.core.tileRect.Width, this.core.tileRect.Height);
            }

            DrawOverlays(g);

            g.ResetTransform();
        }

        private void DrawOverlays(Graphics g)
        {
            this.viewData.Draw(g);

            #region Сopyright
            if (!string.IsNullOrEmpty(core.Provider.Copyright))
            {
                using (var font = new Font(FontFamily.GenericSansSerif, 7, FontStyle.Regular))
                {
                    g.DrawString(core.Provider.Copyright, font, Brushes.Navy, 3, this.core.Height - font.Height - 5);
                }
            }
            #endregion
        }
        #endregion
    }
}