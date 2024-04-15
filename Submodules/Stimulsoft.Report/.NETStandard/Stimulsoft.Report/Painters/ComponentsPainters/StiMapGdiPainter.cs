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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Maps;
using Stimulsoft.Report.Maps.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
using Pens = Stimulsoft.Drawing.Pens;
using Brushes = Stimulsoft.Drawing.Brushes;
using Image = Stimulsoft.Drawing.Image;
using Font = Stimulsoft.Drawing.Font;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Painters
{
    public partial class StiMapGdiPainter : StiComponentGdiPainter
    {
        #region Fields
        public bool UseBackground { get; set; } = true;
        #endregion

        #region Methods
        public void DrawMap(StiMap map, Graphics g, RectangleF rect, bool drawBorder, bool drawFormatting, bool useZoom)
        {
            #region Fill rectangle
            if (drawFormatting)
            {
                var rect1 = new RectangleF(rect.Location, rect.Size);
                StiMapStyle style = StiMap.GetMapStyle(map);
                
                if (style != null)
                {
                    StiDrawing.FillRectangle(g, style.BackColor, rect1.Left, rect1.Top, rect1.Width, rect1.Height);
                }
                else
                {
                    if (map.Brush is StiSolidBrush &&
                        ((StiSolidBrush)map.Brush).Color == Color.Transparent &&
                        map.Report.Info.FillComponent &&
                        map.IsDesigning)
                    {
                        var color = Color.FromArgb(150, Color.White);
                        StiDrawing.FillRectangle(g, color, rect1.Left, rect1.Top, rect1.Width, rect1.Height);
                    }
                    else
                    {
                        StiDrawing.FillRectangle(g, map.GetStyleBackground(), rect1);
                    }
                }

                var state = g.Save();

                g.SetClip(rect1, CombineMode.Intersect);

                g.TranslateTransform((int)rect1.X, (int)rect1.Y);

                rect1.X = 0;
                rect1.Y = 0;

                if (!drawBorder)
                {
                    rect1.Width--;
                    rect1.Height--;
                }

                if (map.MapMode == StiMapMode.Online)
                {
                    PaintOnlineMap(g, rect1, map);
                }
                else
                {
                    map.GetMapData();
                    var context = new StiGdiMapContextPainter(map)
                    {
                        AnimationEngine = AnimationEngine
                    };

                    if (AnimationEngine != null)
                        AnimationEngine.RegisterContextPainter(map, context);

                    context.Render(g, useZoom);
                }

                g.Restore(state);
            }
            #endregion

            #region Draw Border
            if ((drawBorder && map.Border.Topmost) || (drawFormatting && (!map.Border.Topmost)))
            {
                if (map.Border.Side == StiBorderSides.None && map.IsDesigning)
                {
                    using (var pen = new Pen(Color.Gray))
                    {
                        pen.DashStyle = DashStyle.Dash;
                        StiDrawing.DrawRectangle(g, pen, rect.Left, rect.Top, rect.Width, rect.Height);
                    }
                }
                if (map.HighlightState == StiHighlightState.Hide)
                {
                    PaintBorder(map, g, RectangleD.CreateFromRectangle(rect), true, true);
                }
            }
            #endregion
        }

        public void PaintOnlineMap(Graphics g, RectangleF rect, StiMap map)
        {
            var image = StiMapDrawingCache.GetLastImage(map) as Bitmap;
            var size = rect.Size.ToSize();

            try
            {
                if (image == null)
                {
                    var pushPins = new List<string>();
                    if (!string.IsNullOrEmpty(map.PushPins))
                    {
                        pushPins = (List<string>)(JsonConvert.DeserializeObject(map.PushPins) as JArray).ToObject(typeof(List<string>));
                    }
                    else
                    {
                        pushPins.Add($"pp=0,0;60");
                    }

                    if (map.IsDesigning)
                    {
                        if (UseBackground)
                            StiBingMapDataHelper.CreateImageBackground(map, pushPins, size);
                        else
                        {
                            try
                            {
                                image = StiBingMapHelper.GetImage(size, pushPins);
                            }
                            catch { }
                            if (image != null)
                                g.DrawImage(image, new Rectangle(0, 0, (int)rect.Width, (int)rect.Height));
                        }                            
                    }
                    else
                    {
                        image = StiBingMapHelper.GetImage(size, pushPins);
                        if (image != null)
                        {
                            StiMapDrawingCache.StoreLastImage(map, image);
                            g.DrawImage(image, new Rectangle(0, 0, (int)rect.Width, (int)rect.Height));
                        }
                    }
                    
                }
                else
                    g.DrawImage(image, new Rectangle(0, 0, (int)rect.Width, (int)rect.Height));
            }
            catch (Exception ex)
            {
                g.DrawRectangle(Pens.White, new Rectangle(0, 0, (int)rect.Width, (int)rect.Height));

                using (var font = new Font("Arial", 10))
                {
                    string text = $"BING: {ex.Message}";
                    var textSize = g.MeasureString(text, font);
                    g.DrawString(text, font, Brushes.Gray, (rect.Width - textSize.Width) / 2, (rect.Height - textSize.Height) / 2);
                }
            }
        }

        #endregion

        #region Methods.Painter
        public Bitmap GetScaleImage(StiComponent component)
        {
            var map = component as StiMap;

            double resZoom = map.Report.Info.Zoom;
            map.Report.Info.Zoom = 2.0;

            var rect = map.GetPaintRectangle();
            rect.X = 0;
            rect.Y = 0;

            int imageWidth = (int)rect.Width + 2;
            int imageHeight = (int)rect.Height + 2;

            var bmp = new Bitmap(imageWidth, imageHeight);
            using (var g = Graphics.FromImage(bmp))
            {
                g.PageUnit = GraphicsUnit.Pixel;
                g.Clear(StiBrush.ToColor(map.GetStyleBackground()));

                rect.X = 0;
                rect.Y = 0;

                DrawMap(map, g, rect.ToRectangleF(), false, true, true);
            }
            map.Report.Info.Zoom = resZoom;

            return bmp;
        }

        public override Image GetImage(StiComponent component, ref float zoom, StiExportFormat format)
        {
            var map = component as StiMap;

            double resZoom = map.Report.Info.Zoom;
            if (format != StiExportFormat.HtmlSpan &&
                format != StiExportFormat.HtmlDiv &&
                format != StiExportFormat.HtmlTable &&
                UseBackground) zoom *= 2;
            map.Report.Info.Zoom = zoom;

            var rect = map.GetPaintRectangle();
            rect.X = 0;
            rect.Y = 0;

            int imageWidth = (int)rect.Width + 2;
            int imageHeight = (int)rect.Height + 2;

            var bmp = new Bitmap(imageWidth, imageHeight);
            using (var g = Graphics.FromImage(bmp))
            {
                g.PageUnit = GraphicsUnit.Pixel;

                if (((format == StiExportFormat.Pdf) || (format == StiExportFormat.ImagePng)) && (map.Brush != null) &&
                    (((map.Brush is StiSolidBrush) && ((map.Brush as StiSolidBrush).Color.A == 0)) || (map.Brush is StiEmptyBrush)))
                {
                    g.Clear(Color.FromArgb(1, 255, 255, 255));
                }
                else
                {
                    //g.Clear(Color.White);
                    g.Clear(StiBrush.ToColor(map.GetStyleBackground()));
                }

                rect.X = 0;
                rect.Y = 0;

                DrawMap(map, g, rect.ToRectangleF(), true, true, true);
            }
            map.Report.Info.Zoom = resZoom;

            return bmp;
        }

        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var map = component as StiMap;
            map.InvokePainting(map, e);

            if (!e.Cancel && (!(map.Enabled == false && map.IsDesigning == false)))
            {
                var g = e.Graphics;

                var rect = map.GetPaintRectangle();
                if (rect.Width > 0 && rect.Height > 0 &&
                    (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
                {
                    if (StiOptions.Print.ChartAsBitmap && map.IsPrinting)
                    {
                        float zoom = 1;
                        using (var image = GetImage(map, ref zoom, StiExportFormat.None))
                        {
                            g.DrawImage(image, rect.ToRectangleF());
                        }
                    }
                    else
                    {
                        var progressStatus = StiComponentProgressHelper.Contains(component);
                        if (progressStatus != StiProgressStatus.None && map.IsDesigning)
                        {
                            StiDrawing.FillRectangle(g, Color.FromArgb(0x99, Color.White), rect);
                            PaintProgress(g, rect, progressStatus);
                            return;
                        }

                        DrawMap(map, g, rect.ToRectangleF(), e.DrawTopmostBorderSides, e.DrawBorderFormatting, true);
                    }

                    if (e.DrawBorderFormatting)
                    {
                        if (map.IsDesigning)
                            PaintQuickButtons(map, e.Graphics);

                        PaintEvents(map, e.Graphics, rect);
                        PaintConditions(map, e.Graphics, rect);
                    }

                    #region Markers
                    if (map.HighlightState == StiHighlightState.Hide && map.Border.Side != StiBorderSides.All) 
                        PaintMarkers(map, g, rect);
                    #endregion
                }
            }
            e.Cancel = false;
            map.InvokePainted(map, e);

        }

        #endregion
    }
}