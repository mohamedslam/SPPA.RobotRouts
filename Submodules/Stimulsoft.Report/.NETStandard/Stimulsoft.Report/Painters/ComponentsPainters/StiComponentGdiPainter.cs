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
using Stimulsoft.Base.Dashboard;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dialogs;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Images;
using Stimulsoft.Report.QuickButtons;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
using Brushes = Stimulsoft.Drawing.Brushes;
using Font = Stimulsoft.Drawing.Font;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Pens = Stimulsoft.Drawing.Pens;
using Image = Stimulsoft.Drawing.Image;
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiComponentGdiPainter : StiGdiPainter
    {
        #region Methods
        /// <summary>
        /// Paints events of a component.
        /// </summary>
        /// <param name="g">The Graphics to paint on.</param>
        /// <param name="rect">The rectangle.</param>		
        public virtual void PaintEvents(StiComponent component, Graphics g, RectangleD rect)
        {
            if (!component.IsDesigning || component.IsEventEmpty || !component.Report.Info.DrawEventMarkers) return;

            if (StiOptions.Viewer.Pins.EventsRightToLeft)
            {
                var x = (float)rect.Right;
                var y = (float)rect.Y;

                g.FillPolygon(Brushes.Red, new[]
                {
                    new PointF(x, y),
                    new PointF(x - StiScale.I4, y),
                    new PointF(x, y + StiScale.I4)
                });
            }
            else
            {
                var x = (float)rect.X;
                var y = (float)rect.Y;

                g.FillPolygon(Brushes.Red, new[]
                {
                    new PointF(x, y),
                    new PointF(x + StiScale.I4, y),
                    new PointF(x, y + StiScale.I4)
                });
            }
        }

        /// <summary>
        /// Paints conditions and filters of a component.
        /// </summary>
        /// <param name="g">The Graphics to paint on.</param>
        /// <param name="rect">The rectangle.</param>		
        public virtual void PaintConditions(StiComponent component, Graphics g, RectangleD rect)
        {
            PaintConditionsAndFilter(component, g, rect);
        }

        /// <summary>
        /// Paints conditions and filters of a component.
        /// </summary>
        /// <param name="g">The Graphics to paint on.</param>
        /// <param name="rect">The rectangle.</param>		
        public virtual void PaintConditionsAndFilter(StiComponent component, Graphics g, RectangleD rect)
        {
            if (!component.IsDesigning || component.Page == null || component.Page.Zoom < .5) return;

            float leftWidth = 0;
            float rightWidth = 0;

            #region Conditions
            if (component.Conditions != null && component.Conditions.Count > 0)
            {
                var rect2 = rect.ToRectangleF();
                var image = StiReportImages.Engine.Condition();

                var x = rect2.X + 1;
                if (StiOptions.Viewer.Pins.ConditionsRightToLeft)
                {
                    x = rect2.Right - 1 - image.Width;
                    rightWidth = 1 + image.Width;
                }
                else
                {
                    leftWidth = 1 + image.Width;
                }
                var compRect = component.GetPaintRectangle().ToRectangleF();

                var y = rect2.Bottom - image.Height - 1;
                if (y < compRect.Top)
                    y = compRect.Top - StiScale.I1;

                g.DrawImage(image, x, y, image.Width, image.Height);
            }
            #endregion

            #region Interaction
            if (component.IsDesigning)
            {
                var interaction = component.Interaction;

                if (interaction != null && !interaction.IsDefaultExt)
                {
                    var image = StiReportImages.Engine.Interactions();
                    var rect2 = rect.ToRectangleF();

                    var x = rect2.X + 1 + leftWidth;
                    if (StiOptions.Viewer.Pins.FiltersRightToLeft)
                    {
                        x = rect2.Right - 1 - image.Width - rightWidth;
                        rightWidth += 1 + image.Width;
                    }
                    else
                    {
                        leftWidth += 1 + image.Width;
                    }

                    g.DrawImage(image, x, rect2.Bottom - image.Height - 1, image.Width, image.Height);
                }
            }
            #endregion

            #region Filters
            var filter = component as IStiFilter;
            if (filter?.Filters == null || filter.Filters.Count <= 0 || !filter.FilterOn) return;
            {
                var rect2 = rect.ToRectangleF();
                var image = StiReportImages.Engine.Filter();

                var x = rect2.X + 1 + leftWidth;
                if (StiOptions.Viewer.Pins.FiltersRightToLeft)
                    x = rect2.Right - 1 - image.Width - rightWidth;

                g.DrawImage(image, x, rect2.Bottom - image.Height - 1, image.Width, image.Height);
            }
            #endregion
        }

        public virtual void PaintInteraction(StiComponent component, Graphics g)
        {
            if (component.IsDesigning || component.IsPrinting ||  component.Page == null || component.Page.Zoom < .5 || 
                component?.Report?.Info == null || !component.Report.Info.ShowInteractive) return;

            var interaction = component as IStiInteraction;
            if (interaction.Interaction == null || interaction.Interaction.SortingDirection == StiInteractionSortDirection.None) return;

            var rect = component.GetPaintRectangle().ToRectangleF();
            var image = interaction.Interaction.SortingDirection == StiInteractionSortDirection.Ascending 
                ? StiReportImages.Engine.SortAsc() 
                : StiReportImages.Engine.SortDesc();

            float strWidth = 2;
            if (interaction.Interaction.SortingIndex > 0)
            {
                var strSortIndex = interaction.Interaction.SortingIndex.ToString();
                using (var font = new Font("Arial", 6))
                using (var sf = new StringFormat())
                {
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Near;

                    var size = g.MeasureString(strSortIndex, font);
                    strWidth = size.Width + 4;

                    var strRect = new RectangleF(rect.Right - strWidth, rect.Y, strWidth, rect.Height);
                    if (StiOptions.Viewer.Pins.InteractionSortRightToLeft)
                        strRect.X = rect.X;

                    g.DrawString(strSortIndex, font, Brushes.Black, strRect, sf);
                }
            }

            var x = rect.Right - image.Width - strWidth;

            if (StiOptions.Viewer.Pins.InteractionSortRightToLeft)
                x = rect.X + strWidth;

            g.DrawImage(
                image,
                x, rect.Y + (rect.Height - image.Height) / 2,
                image.Width, image.Height);
        }

        public virtual void PaintNoDefinedStatus(Graphics g, RectangleF rect, StiComponent comp, string imagePath, string defaultText)
        {
            rect.Inflate(-StiScale.I10, -StiScale.I10);

            var isImageVisible = rect.Width > StiScale.XXI(60) && rect.Height > StiScale.YYI(60);
            var isTextVisible = rect.Width > StiScale.XXI(200) && rect.Height > StiScale.YYI(60);

            var factor = StiScale.Factor > 1 ? "_x2" : "";

            using (var image = StiImageUtils.GetImage(typeof(StiReport), $"Stimulsoft.Report.Images.NotDefined.{imagePath}{factor}.png"))
            using (var font = StiElementConsts.DragDrop.Font.GetGdiFont(comp.Report.Info.Zoom))
            {
                var imageWidth = image.Width;
                var imageHeight = image.Height;
                var totalHeight = imageHeight + font.GetHeight() + StiScale.I4;
                var rectI = new RectangleD(rect.X, rect.Y + (rect.Height - totalHeight) / 2, rect.Width, totalHeight).ToRectangle();

                if (isImageVisible)
                {
                    if (isTextVisible)
                        g.DrawImage(image, rectI.X + (rectI.Width - imageWidth) / 2, rectI.Y, imageWidth, imageHeight);
                    else
                        g.DrawImage(image, rectI.X + (rectI.Width - imageWidth) / 2, rectI.Y + (rectI.Height - imageHeight) / 2, imageWidth, imageHeight);
                }

                if (isTextVisible)
                {
                    using (var sf = new StringFormat())
                    {
                        sf.LineAlignment = StringAlignment.Far;
                        sf.Alignment = StringAlignment.Center;

                        var textColor = Color.Gray;
                        var textRect = new RectangleD(rectI.X, rectI.Y, rectI.Width, rectI.Height);
                        StiTextDrawing.DrawString(g, defaultText, font, textColor, textRect, sf);
                    }
                }
            }
        }

        public virtual void PaintInheritedImage(StiComponent component, Graphics g)
        {
            if (!(component.IsDesigning && component.Report != null) || component.Page.Zoom < .25)
                return;

            if (component.Inherited)
            {
                var rect = component.GetPaintRectangle().ToRectangleF();
                var image = StiReportImages.Engine.Locked();

                var x = StiOptions.Viewer.Pins.InheritedRightToLeft
                    ? rect.Right - image.Width
                    : rect.X;

                g.DrawImage(image, x, rect.Y, image.Width, image.Height);
            }

            var container = component as StiContainer;
            if (container == null) return;

            foreach (StiComponent comp in container.Components)
            {
                PaintInheritedImage(comp, g);
            }
        }

        /// <summary>
        /// Paints order numbers and quick info of a component.
        /// </summary>
        /// <param name="g">The Graphics to paint on.</param>
        /// <param name="number">A number of the component order for painting.</param>
        public virtual void PaintOrderAndQuickInfo(StiComponent component, Graphics g, string number)
        {
            if (!(component is StiPage) && !component.Report.Info.IsComponentsMoving)
            {
                var rect = component.GetPaintRectangle().ToRectangleF();
                var sizeOrder = SizeF.Empty;

                #region Paint Order
                if (component.Report.Info.ShowOrder)
                {
                    number += (component.Parent.Components.IndexOf(component)).ToString();

                    sizeOrder = g.MeasureString(number, StiOptions.Engine.QuickInfo.QuickInfoSystemFont);
                    var rectOrder = new RectangleF(rect.X, rect.Y, sizeOrder.Width, sizeOrder.Height);

                    if (StiOptions.Viewer.Pins.OrderAndQuickInfoRightToLeft)
                        rectOrder.X = rect.Right - sizeOrder.Width;

                    g.FillRectangle(Brushes.Blue, rectOrder);
                    g.DrawString(number, StiOptions.Engine.QuickInfo.QuickInfoSystemFont, Brushes.White, rectOrder);
                }
                #endregion

                #region Paint Quick Info
                if (component.Report.Info.QuickInfoOverlay && component.Report.Info.QuickInfoType != StiQuickInfoType.None)
                {
                    var quickInfoText = component.GetQuickInfo();
                    var sizeQuickInfo = g.MeasureString(quickInfoText, StiOptions.Engine.QuickInfo.QuickInfoSystemFont);
                    var rectQuickInfo = new RectangleF(rect.X + sizeOrder.Width, rect.Y, sizeQuickInfo.Width, sizeQuickInfo.Height);
                    if (rectQuickInfo.Width > rect.Width - sizeOrder.Width)
                        rectQuickInfo.Width = rect.Width - sizeOrder.Width;

                    if (StiOptions.Viewer.Pins.OrderAndQuickInfoRightToLeft)
                        rectQuickInfo.X = rect.Right - sizeOrder.Width - sizeQuickInfo.Width;

                    g.FillRectangle(Brushes.Green, rectQuickInfo);
                    g.DrawString(quickInfoText, StiOptions.Engine.QuickInfo.QuickInfoSystemFont, Brushes.White, rectQuickInfo, StiOptions.Engine.QuickInfo.QuickInfoStringFormat);
                }
                #endregion
            }

            var container = component as StiContainer;
            if (container == null) return;

            if (number.Length > 0)
                number += '.';

            foreach (StiComponent comp in container.Components)
                PaintOrderAndQuickInfo(comp, g, number);
        }

        /// <summary>
        /// Paints the QuickButtons.
        /// </summary>
        /// <param name="g">The Graphics to paint on.</param>
        public virtual void PaintQuickButtons(StiComponent component, Graphics g)
        {
            if (!component.IsDesigning || !component.ShowQuickButtons || component.Report.Info.IsComponentsMoving 
                || !StiRestrictionsHelper.IsAllowChange(component)) return;

            var quickButtons = component.Report.Designer.GetQuickButtons(component);
            if (quickButtons == null || quickButtons.Length == 0) return;
            var rect = component.GetPaintRectangle();

            var band = component as StiBand;
            if (band != null)
            {
                rect.Y -= band.HeaderSize * component.Page.Zoom * StiScale.Factor;
                rect.Height = band.HeaderSize * component.Page.Zoom * StiScale.Factor;
            }

            if (StiQuickButtonHelper.CheckVisibleQuickButtons(rect))
            {
                var buttonIndex = 0;
                foreach (var button in quickButtons)
                {
                    if (button.Image != null && button.SelectedImage != null)
                    {
                        var image = button.IsSelected ? button.SelectedImage : button.Image;
                        var buttonRect = StiQuickButtonHelper.GetQuickButtonRect(quickButtons, buttonIndex, rect).ToRectangle();
                        buttonRect = new Rectangle(
                            buttonRect.X - (image.Width - buttonRect.Width) / 2,
                            buttonRect.Y - (image.Height - buttonRect.Height) / 2,
                            image.Width, image.Height);

                        g.DrawImage(image, buttonRect);
                    }
                    buttonIndex++;
                }
            }
        }

        /// <summary>
        /// Paints a markers specified by a Rectangle structure.
        /// </summary>
        /// <param name="g">The Graphics to draw on.</param>
        /// <param name="rect">RectangleD structure that represents the rectangle to draw markers.</param>
        public virtual void PaintMarkers(StiComponent component, Graphics g, RectangleD rect, Color? color = null)
        {
            if (!component.IsDesigning || component.Report.Info.MarkersStyle == StiMarkersStyle.None) return;
            if (component.Report.Info.IsComponentsMoving && (!component.Report.Info.DrawMarkersWhenMoving || !component.Report.Info.IsComponentsMoving)) return;

            if (component.Report.Info.MarkersStyle == StiMarkersStyle.Corners)
            {
                var size = StiScale.I3;
                if (color == null) color = Color.DimGray;

                using (var pen = new Pen(color.Value))
                {
                    StiDrawing.DrawLine(g, pen, rect.X, rect.Y, rect.X + size, rect.Y);
                    StiDrawing.DrawLine(g, pen, rect.X, rect.Y, rect.X, rect.Y + size);

                    StiDrawing.DrawLine(g, pen, rect.Right, rect.Y, rect.Right - size, rect.Y);
                    StiDrawing.DrawLine(g, pen, rect.Right, rect.Y, rect.Right, rect.Y + size);

                    StiDrawing.DrawLine(g, pen, rect.Right, rect.Bottom, rect.Right, rect.Bottom - size);
                    StiDrawing.DrawLine(g, pen, rect.Right, rect.Bottom, rect.Right - size, rect.Bottom);

                    StiDrawing.DrawLine(g, pen, rect.X, rect.Bottom, rect.X, rect.Bottom - size);
                    StiDrawing.DrawLine(g, pen, rect.X, rect.Bottom, rect.X + size, rect.Bottom);
                }
            }
            else
            {
                StiDrawing.DrawRectangle(g, StiOptions.Engine.QuickInfo.DashStylePen, rect);
            }
        }

        public virtual void PaintBorder(StiComponent component, Graphics g, RectangleD rect)
        {
            PaintBorder(component, g, rect, true, true);
        }

        public virtual void PaintBorder(StiComponent component, Graphics g, RectangleD rect, bool drawBorderFormatting, bool drawBorderSides)
        {
            PaintBorder(component, g, rect, component.Page?.Zoom ?? 1f, drawBorderFormatting, drawBorderSides);
        }

        public virtual void PaintBorder(StiComponent component, Graphics g, RectangleD rect, double zoom, bool drawBorderFormatting, bool drawBorderSides)
        {
            PaintBorder(component, g, rect.ToRectangleF(), (float)zoom, drawBorderFormatting, drawBorderSides);
        }

        public virtual void PaintBorder(StiComponent component, Graphics g, RectangleF rect, float zoom, bool drawBorderFormatting, bool drawBorderSides)
        {
            var border = component as IStiBorder;
            if (border == null || border.Border == null) return;

            if (border.Border.Style == StiPenStyle.Double)
            {
                var emptyColor = Color.White;

                var brush = component as IStiBrush;
                if (brush != null && brush.Brush != null) emptyColor = StiBrush.ToColor(brush.Brush);
                if (emptyColor == Color.Transparent)
                    emptyColor = Color.White;

                border.Border.Draw(g, rect, zoom, emptyColor, drawBorderFormatting, drawBorderSides);
            }
            else
            {
                border.Border.Draw(g, rect, zoom, Color.White, drawBorderFormatting, drawBorderSides);
            }
        }
        #endregion

        #region Methods.Paint
        public override Image GetImage(StiComponent component, ref float zoom, StiExportFormat format)
        {
            return null;
        }

        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
        }

        /// <summary>
        /// Paints the selection.
        /// </summary>
        public override void PaintSelection(StiComponent component, StiPaintEventArgs e)
        {
            var g = e.Graphics;
            if (component.IsDesigning && component.IsSelected && !component.Report.Info.IsComponentsMoving)
            {
                var rect = component.GetPaintRectangle();

                var size = StiScale.I2;
                if (component.Linked)
                    size = StiScale.I3;

                var color = GetSelectionCornerColor(component);
                using (var brush = new SolidBrush(color))
                {
                    StiDrawing.DrawSelectedRectangle(g, size, brush, rect);
                }
            }
        }

        public virtual Color GetSelectionCornerColor(StiComponent comp)
        {
            return comp.Locked ? Color.Red : Color.DimGray;
        }

        /// <summary>
        /// Paints the highlight of the specified component.
        /// </summary>
        public override void PaintHighlight(StiComponent component, StiPaintEventArgs e)
        {
            var g = e.Graphics;

            if (component.HighlightState == StiHighlightState.Show || component.HighlightState == StiHighlightState.Active)
            {
                var rect = component.GetPaintRectangle();
                rect.Inflate(StiScale.I1, StiScale.I1);

                var color = StiOptions.Viewer.HighlightShowStateColor;
                if (color == Color.Red)
                    color = Color.FromArgb(0xff, 0x2b, 0x57, 0x9a);

                using (var pen = new Pen(color))
                {
                    if (component.HighlightState == StiHighlightState.Show)
                    {
                        StiDrawing.DrawRectangle(g, pen, rect);
                    }
                    else if (component.HighlightState == StiHighlightState.Active)
                    {
                        pen.Width = StiScale.I2;
                        StiDrawing.DrawRectangle(g, pen, rect);
                    }
                }
            }

            var container = component as StiContainer;
            if (container != null)
            {
                foreach (StiComponent comp in container.Components)
                    comp.PaintHighlight(e);
            }
        }

        /// <summary>
        /// Gets a thumbnail image of the component.
        /// </summary>
        /// <param name="width">Width of the thumbnail image.</param>
        /// <param name="height">Height of the thumbnail image.</param>
        /// <returns>A thumbnail image of the specified size.</returns>
        public override Bitmap GetThumbnail(StiComponent component, int width, int height, bool isDesignTime)
        { 
            StiScale.Lock();

            var unit = component is StiPage ? ((StiPage)component).Unit : component.Page.Unit;

            var paintZoomWidth = width / unit.ConvertToHInches(component.DisplayRectangle.Width);
            var paintZoomHeight = height / unit.ConvertToHInches(component.DisplayRectangle.Height);
            var paintZoom = Math.Min(paintZoomWidth, paintZoomHeight);

            var resZoom = component.Report.Info.Zoom;
            var resForceDesigningMode = component.Report.Info.ForceDesigningMode;
            var resShowGrid = component.Report.Info.ShowGrid;
            var resMarkersStyle = component.Report.Info.MarkersStyle;
            var resDrawEventMarkers = component.Report.Info.DrawEventMarkers;
            var resFillComponent = component.Report.Info.FillComponent;

            component.Report.Info.Zoom = paintZoom;
            component.Report.Info.ForceDesigningMode = isDesignTime;
            component.Report.Info.ShowGrid = false;
            component.Report.Info.MarkersStyle = StiMarkersStyle.None;
            component.Report.Info.DrawEventMarkers = false;
            component.Report.Info.FillComponent = false;

            var imageWidth = (int)(unit.ConvertToHInches(component.DisplayRectangle.Width) * paintZoom);
            var imageHeight = (int)(unit.ConvertToHInches(component.DisplayRectangle.Height) * paintZoom);

            if (imageWidth == 0)
                imageWidth = 1;

            if (imageHeight == 0)
                imageHeight = 1;

            Bitmap bmp;

            #region Form
            if (component is StiForm)
            {
                var formBmp = new Bitmap((int)component.DisplayRectangle.Width, (int)component.DisplayRectangle.Height);
                using (var g = Graphics.FromImage(formBmp))
                {
                    g.Clear(Color.White);
                    component.Paint(new StiPaintEventArgs(g, RectangleD.Empty));
                }

                bmp = new Bitmap(imageWidth, imageHeight);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(formBmp, 0, 0, imageWidth, imageHeight);
                    g.ResetTransform();
                    g.DrawRectangle(Pens.Gray, 0, 0, imageWidth - 2, imageHeight - 2);
                }
            }
            #endregion

            #region Page
            else if (component is StiPage)
            {
                bmp = new Bitmap(imageWidth, imageHeight);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);

                    ((StiPage)component).LockHighlight = true;
                    component.Paint(new StiPaintEventArgs(g, RectangleD.Empty));
                    ((StiPage)component).LockHighlight = false;
                    g.ResetTransform();
                    g.DrawRectangle(Pens.Gray, 0, 0, imageWidth - 2, imageHeight - 2);
                }
            }
            #endregion

            #region Component
            else
            {
                component.Report.Info.Zoom = 1;

                var rect = component.GetPaintRectangle();

                using (var bmpComp = new Bitmap((int)Math.Round(rect.Width), (int)Math.Round(rect.Height)))
                using (var g = Graphics.FromImage(bmpComp))
                {
                    g.Clear(Color.White);

                    g.TranslateTransform(-(float)rect.X, -(float)rect.Y);
                    component.Paint(new StiPaintEventArgs(g, RectangleD.Empty));
                    g.ResetTransform();

                    if (bmpComp.Width > bmpComp.Height)
                    {
                        float factor = bmpComp.Width / bmpComp.Height;
                        bmp = bmpComp.GetThumbnailImage(width, (int)(height / factor), null, IntPtr.Zero) as Bitmap;
                    }
                    else
                    {
                        float factor = bmpComp.Height / bmpComp.Width;
                        bmp = bmpComp.GetThumbnailImage((int)(width / factor), height, null, IntPtr.Zero) as Bitmap;
                    }
                }
            }
            #endregion

            component.Report.Info.Zoom = resZoom;
            component.Report.Info.ForceDesigningMode = resForceDesigningMode;
            component.Report.Info.ShowGrid = resShowGrid;
            component.Report.Info.MarkersStyle = resMarkersStyle;
            component.Report.Info.DrawEventMarkers = resDrawEventMarkers;
            component.Report.Info.FillComponent = resFillComponent;

            StiScale.Unlock();

            return bmp;
        }

        /// <summary>
        /// Paints progress of a component.
        /// </summary>
        /// <param name="g">The Graphics to paint on.</param>
        /// <param name="rect">The rectangle.</param>	
        public static void PaintProgress(Graphics g, RectangleD rect, StiProgressStatus progressStatus, float? value = null)
        {
            if (progressStatus == StiProgressStatus.Short)
                PaintShortProgress(g, rect);

            else if (progressStatus == StiProgressStatus.Long)
                PaintLongProgress(g, rect, value);
        }

        private static void PaintShortProgress(Graphics g, RectangleD rect)
        {
            using (var font = new Font("Arial", 8))
            using (var sf = new StringFormat())
            {
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                var str = Loc.Get("A_WebViewer", "Loading");
                var size = g.MeasureString(str, font);
                if (size.Width > rect.Width || size.Height > rect.Height) return;

                g.DrawString(str, font, Brushes.LightGray, rect.ToRectangleF(), sf);
            }
        }

        private static void PaintLongProgress(Graphics g, RectangleD rect, float? value)
        {
            var side = Math.Min(rect.Width, rect.Height) * 0.8f;
            if (side <= 0) return;

            if (side > 80 * StiScale.Factor) side = 80 * StiScale.Factor;
            var thickness = (int) (side * 0.1);

            var clientRect = new RectangleF()
            {
                X = (float) (rect.X + (rect.Width - side) / 2),
                Y = (float) (rect.Y + (rect.Height - side) / 2),
                Width = (float) side,
                Height = (float) side
            };

            var oldMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var path = new GraphicsPath())
            {
                if (value == null)
                    value = StiComponentProgressHelper.CurrentValue;

                path.AddArc(clientRect, value.Value, 65);

                using (var penEllipds = new Pen(Color.FromArgb(230, 230, 230), thickness))
                    g.DrawEllipse(penEllipds, clientRect);

                using (var pen = new Pen(Color.FromArgb(200, 74, 125, 177), thickness))
                    g.DrawPath(pen, path);
            }

            g.SmoothingMode = oldMode;
        }
        #endregion
    }
}
