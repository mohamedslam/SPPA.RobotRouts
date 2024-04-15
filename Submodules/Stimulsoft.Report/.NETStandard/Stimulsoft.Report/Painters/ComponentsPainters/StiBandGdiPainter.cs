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

using System;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Engine;
using Stimulsoft.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
using Pens = Stimulsoft.Drawing.Pens;
using Brushes = Stimulsoft.Drawing.Brushes;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiBandGdiPainter : StiContainerGdiPainter
    {
        #region Methods
        /// <summary>
        /// Paints the columns.
        /// </summary>
        /// <param name="g">The Graphics to paint on.</param>
        public override void PaintColumns(StiContainer container, Graphics g)
        {
            var dataBand = container as StiDataBand;
            if (dataBand == null)
            {
                if (container is StiColumnHeaderBand) 
                    dataBand = StiHeaderBandV1Builder.GetMaster(container as StiColumnHeaderBand) as StiDataBand;

                if (container is StiColumnFooterBand) 
                    dataBand = StiFooterBandV1Builder.GetMaster(container as StiColumnFooterBand) as StiDataBand;
            }

            if (container.IsDesigning && dataBand != null && dataBand.Columns >= 2)
            {
                var zoom = dataBand.Page.Zoom;
                var rect = dataBand.GetPaintRectangle();

                var columnWidth = dataBand.Page.Unit.ConvertToHInches(dataBand.GetColumnWidth()) * zoom * StiScale.Factor;
                var columnGaps = dataBand.Page.Unit.ConvertToHInches(dataBand.ColumnGaps) * zoom * StiScale.Factor;

                rect.X += columnWidth + columnGaps;
                var comps = dataBand.GetComponents();

                foreach (StiComponent comp in comps)
                {
                    var compRect = comp.GetPaintRectangle();
                    if (compRect.Width > 0 && compRect.Height > 0)
                    {
                        using (var pen = new Pen(Color.Blue, 1))
                        {
                            pen.DashStyle = DashStyle.Dash;

                            for (int index = 0; index < dataBand.Columns - 1; index++)
                            {
                                compRect.X += columnWidth + columnGaps;

                                StiDrawing.FillRectangle(g, Color.FromArgb(20, Color.Blue), compRect);
                                StiDrawing.DrawRectangle(g, pen, compRect);
                            }
                        }
                    }
                }
            }
        }

        public override void PaintSelection(StiComponent component, StiPaintEventArgs e)
        {            
            if (!component.IsDesigning || !component.IsSelected || component.Report.Info.IsComponentsMoving)return;

            var g = e.Graphics;
            var rect = component.GetPaintRectangle().ToRectangleF();
            var size = component.Linked ? StiScale.I3 : StiScale.I2;

            DrawNoSelectedPoint(g, size, rect.Left, rect.Top);
            DrawNoSelectedPoint(g, size, rect.Right, rect.Top);
            DrawNoSelectedPoint(g, size, rect.Left, rect.Bottom);
            DrawNoSelectedPoint(g, size, rect.Right, rect.Bottom);

            if (component.IsCross)
            {
                DrawSelectedPoint(g, size, rect.Right - size / 2, rect.Top + rect.Height / 2 - size / 2);
            }
            else
            {
                if (StiOptions.Engine.DockPageFooterToBottom && component is StiPageFooterBand)
                {
                    DrawSelectedPoint(g, size, rect.X + rect.Width / 2 - size / 2, rect.Top - size / 2);
                }
                else
                {
                    DrawSelectedPoint(g, size, rect.X + rect.Width / 2 - size / 2, rect.Bottom - size / 2);                    
                }
            }
        }

        private void DrawNoSelectedPoint(Graphics g, float size, float x, float y)
        {
            var rect = new RectangleF(x - size, y - size, size * 2 + 1, size * 2 + 1);
            g.FillRectangle(Brushes.White, rect);
            g.DrawRectangle(Pens.DimGray, rect.X, rect.Y, rect.Width, rect.Height);
        }

        private void DrawSelectedPoint(Graphics g, float size, float x, float y)
        {
            var rect = new RectangleF(x - size, y - size, size * 2 + 1, size * 2 + 1);
            g.FillRectangle(Brushes.DimGray, rect);
            g.DrawRectangle(Pens.DimGray, rect.X, rect.Y, rect.Width, rect.Height);
        }
        #endregion

        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            var band = component as StiBand;

            if ((!e.DrawBorderFormatting) && e.DrawTopmostBorderSides && (!band.Border.Topmost))
                return;

            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            if (e.DrawBorderFormatting)
                band.InvokePainting(band, e);

            var g = e.Graphics;
            var zoom = band.Page.Zoom * StiScale.Y;

            if (!e.Cancel)
            {
                if (band.IsDesigning)
                {
                    var rect = band.GetPaintRectangle();

                    var headerRect = new RectangleD(rect.Left, rect.Top - band.HeaderSize * zoom, rect.Width, band.HeaderSize * zoom);
                    var fullRect = headerRect;
                    fullRect.Height += rect.Height;

                    if (fullRect.Width > 0 && fullRect.Height > 0 && (e.ClipRectangle.IsEmpty || fullRect.IntersectsWith(e.ClipRectangle)))
                    {
                        #region Fill bands
                        if (e.DrawBorderFormatting)
                        {
                            if (band.Brush is StiSolidBrush && ((StiSolidBrush)band.Brush).Color == Color.Transparent &&
                                band.Report.Info.FillBands && band.IsDesigning)
                            {
                                var color = band.Report.Info.GetFillColor(band.HeaderEndColor);
                                StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
                            }
                            else
                            {
                                StiDrawing.FillRectangle(g, band.Brush, rect);
                            }

                            PaintBandContent(band, g, rect);
                        }
                        #endregion

                        #region Header
                        if (band.Report.Info.ShowHeaders && e.DrawBorderFormatting)                        
                            PaintBandHeader(band, g, rect, headerRect);

                        #region Border
                        if (band.Border.Side == StiBorderSides.None && band.IsDesigning)
                        {
                            if (e.DrawBorderFormatting)
                            {
                                var headerColor = Color.FromArgb(220, band.HeaderStartColor);
                                using (var pen = new Pen(headerColor))
                                {
                                    pen.DashStyle = DashStyle.Dash;
                                    StiDrawing.DrawRectangle(g, pen, rect.Left, rect.Top, rect.Width, rect.Height);
                                }
                            }
                        }
                        else
                        {
                            if (band.HighlightState == StiHighlightState.Hide)
                                PaintBorder(band, g, rect, zoom, e.DrawBorderFormatting, e.DrawTopmostBorderSides || (!band.Border.Topmost));
                        }
                        #endregion
                        #endregion

                        if (band.IsDesigning && e.DrawBorderFormatting)
                        {
                            PaintColumns(band, g);
                            PaintDataBandColumns(band, g, rect);
                            PaintSpecialDockService(band, g, headerRect, fullRect);
                        }

                        if (e.DrawBorderFormatting)
                        {
                            PaintEvents(band, g, rect);
                            PaintConditions(band, g, rect);
                        }
                    }
                }
            }
            e.Cancel = false;
            
            if (e.DrawBorderFormatting)
            {
                band.InvokePainted(band, e);

                PaintComponents(band, e);
                PaintQuickButtons(band, e.Graphics);
            }
        }

        public virtual void PaintBandContent(StiBand band, Graphics g, RectangleD rect)
        {            
        }

        public virtual void PaintBandHeader(StiBand band, Graphics g, RectangleD rect, RectangleD headerRect)
        {
            var zoom = band.Page.Zoom * StiScale.Factor;

            var headerColor = Color.FromArgb(220, band.HeaderStartColor);
            using (var brush = new SolidBrush(headerColor))
            {
                g.FillRectangle(brush, headerRect.ToRectangleF());
            }

            using (var stringFormat = new StringFormat())
            using (var font = new Font("Segoe UI", (float)(9 * zoom / StiScale.Y)))
            {
                stringFormat.LineAlignment = StringAlignment.Center;
                stringFormat.Alignment = StringAlignment.Near;
                stringFormat.Trimming = StringTrimming.EllipsisCharacter;
                stringFormat.FormatFlags = StringFormatFlags.NoWrap;

                StiTextDrawing.DrawString(g, band.GetHeaderText(), font, Brushes.Black, headerRect, stringFormat);

                PaintDataBandHeader(band, g, rect, headerRect, stringFormat, font);
            }
        }

        public virtual void PaintDataBandHeader(StiBand band, Graphics g, RectangleD rect, RectangleD headerRect, StringFormat stringFormat, Font font)
        {
            var dataBand = band as StiDataBand;
            if (dataBand == null) return;

            var width = g.MeasureString(band.GetHeaderText(), font, new SizeF((float)rect.Width, (float)rect.Height)).Width;
            if (dataBand.MasterComponent == null || width <= 0)return;
            
            stringFormat.LineAlignment = StringAlignment.Center;
            stringFormat.Alignment = StringAlignment.Far;
            stringFormat.Trimming = StringTrimming.EllipsisCharacter;
            stringFormat.FormatFlags = StringFormatFlags.NoWrap;

            var rectMaster = new RectangleD(headerRect.Left + width, headerRect.Top, headerRect.Width - width, headerRect.Height);

            StiTextDrawing.DrawString(g, $"{Loc.GetMain("MasterComponent")}: {dataBand.MasterComponent.Name}",
                font, Brushes.Black, rectMaster, stringFormat);
        }

        public virtual void PaintDataBandColumns(StiBand band, Graphics g, RectangleD rect)
        {
            var dataBand = band as StiDataBand;
            if (dataBand == null)
            {
                if (band is StiColumnHeaderBand)
                    dataBand = StiHeaderBandV1Builder.GetMaster(band as StiHeaderBand) as StiDataBand;

                if (band is StiColumnFooterBand)
                    dataBand = StiFooterBandV1Builder.GetMaster(band as StiFooterBand) as StiDataBand;
            }

            if (dataBand == null || dataBand.Columns <= 1)return;
            
            var zoom = dataBand.Page.Zoom * StiScale.Factor;
            var columnWidth = dataBand.Page.Unit.ConvertToHInches(dataBand.GetColumnWidth()) * zoom;
            var columnGaps = dataBand.Page.Unit.ConvertToHInches(dataBand.ColumnGaps) * zoom;

            var pos = columnWidth + rect.Left;
            using (var pen = new Pen(Color.Red))
            {
                pen.DashStyle = DashStyle.Dash;

                for (int index = 1; index < dataBand.Columns; index++)
                {
                    g.DrawLine(pen,
                        (float)pos, (float)rect.Top,
                        (float)pos,
                        (float)(rect.Top + rect.Height));

                    g.DrawLine(pen,
                        (float)(pos + columnGaps), (float)rect.Top,
                        (float)(pos + columnGaps),
                        (float)(rect.Top + rect.Height));

                    pos += columnWidth + columnGaps;
                }

                g.DrawLine(pen,
                    (float)pos, (float)rect.Top,
                    (float)pos,
                    (float)(rect.Top + rect.Height));
            }
        }

        public virtual void PaintSpecialDockService(StiBand band, Graphics g, RectangleD headerRect, RectangleD fullRect)
        {
            if (band.RectangleMoveComponent == null)return;
            
            var zoom = band.Page.Zoom * StiScale.Factor;
            var rect = band.Page.Unit.ConvertToHInches((RectangleD)band.RectangleMoveComponent).Multiply(zoom);
            rect.X += fullRect.Left;
            rect.Y += fullRect.Top;

            if (band.Report.Info.ShowHeaders)
                rect.Y += headerRect.Height;

            using (var brush = new SolidBrush(Color.FromArgb(100, Color.Red)))
            {
                StiDrawing.FillRectangle(g, brush, rect);
            }
        }
        #endregion
    }
}