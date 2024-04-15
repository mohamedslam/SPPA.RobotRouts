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
using System.Collections.Generic;

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Design;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Engine;
using Stimulsoft.Base;
using Stimulsoft.Report.Dashboard;
using System.Drawing;
using System.Drawing.Drawing2D;
using Stimulsoft.Report.Images;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Brushes = Stimulsoft.Drawing.Brushes;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Font = Stimulsoft.Drawing.Font;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Pen = Stimulsoft.Drawing.Pen;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiContainerGdiPainter : StiComponentGdiPainter
    {
        #region Consts
        internal const string TopmostToken = "#drawtopmost";
        #endregion

        #region Methods
        public virtual void PaintColumns(StiContainer container, Graphics g)
        {
            var panel = container as StiPanel;
            if (panel == null || !container.IsDesigning || panel.Columns < 2) return;

            var zoom = container.Page.Zoom * StiScale.Factor;
            var rect = container.GetPaintRectangle();
            var columnWidth = container.Page.Unit.ConvertToHInches(panel.GetColumnWidth()) * zoom;
            var columnGaps = container.Page.Unit.ConvertToHInches(panel.ColumnGaps) * zoom;
            var pos = columnWidth + rect.Left;

            using (var pen = new Pen(Color.Red))
            {
                pen.DashStyle = DashStyle.Dash;

                var countOfColumns = panel.Columns;
                if (panel.ColumnWidth == 0 && panel.ColumnGaps == 0)
                    countOfColumns--;

                for (var index = 1; index < countOfColumns; index++)
                {
                    g.DrawLine(pen,
                        (float) pos, (float) rect.Top,
                        (float) pos,
                        (float) (rect.Top + rect.Height));

                    g.DrawLine(pen,
                        (float) (pos + columnGaps), (float) rect.Top,
                        (float) (pos + columnGaps),
                        (float) (rect.Top + rect.Height));

                    pos += columnWidth + columnGaps;
                }

                g.DrawLine(pen,
                    (float) pos, (float) rect.Top,
                    (float) pos,
                    (float) (rect.Top + rect.Height));
            }

            rect.X += columnWidth + columnGaps;

            var comps = container.GetComponents();
            foreach (StiComponent comp in comps)
            {
                var compRect = comp.GetPaintRectangle();
                if (!(compRect.Width > 0) || !(compRect.Height > 0)) continue;

                using (var pen = new Pen(Color.Blue, 1))
                {
                    pen.DashStyle = DashStyle.Dash;

                    for (int index = 0; index < panel.Columns - 1; index++)
                    {
                        compRect.X += columnWidth + columnGaps;

                        StiDrawing.FillRectangle(g, Color.FromArgb(20, Color.Blue), compRect);
                        StiDrawing.DrawRectangle(g, pen, compRect);
                    }
                }
            }
        }

        public virtual void PaintBandInteraction(StiComponent component, Graphics g)
        {
            if (!(component is StiContainer)) return;
            if (component.Report.Info.Zoom < .5) return;
            if (!component.Report.Info.ShowInteractive) return;
            if (component.Report.EngineVersion == StiEngineVersion.EngineV1) return;

            var container = component as StiContainer;
            var interaction = component as IStiInteraction;
            if (!(interaction.Interaction is StiBandInteraction) || !((StiBandInteraction) interaction.Interaction).CollapsingEnabled) return;

            var rect = component.GetPaintRectangle().ToRectangleF();

            var collapsed = StiDataBandV2Builder.IsCollapsed(container, false);
            var image = collapsed ? StiReportImages.Engine.Collapsed() : StiReportImages.Engine.Expanded();

            var x = StiOptions.Viewer.Pins.EventsRightToLeft
                ? (int) rect.Right - StiScale.I3 - image.Width
                : (int) rect.Left + StiScale.I3;

            g.DrawImage(image, x, (int) rect.Y + StiScale.I3, image.Width, image.Height);
        }

        /// <summary>
        /// Paints components.
        /// </summary>
        /// <param name="e">Argument for painting.</param>
        public virtual void PaintComponents(StiContainer container, StiPaintEventArgs e)
        {
            if (!e.DrawChilds) return;

            #region Paint Page as Dashboard
            if (container.Page.IsDashboard)
            {
                foreach (StiComponent component in container.Components)
                {
                    if (!component.IsSelected)
                        component.Paint(e);
                }

                foreach (StiComponent component in container.Components)
                {
                    if (component.IsSelected)
                        component.Paint(e);
                }
            }
            #endregion

            #region Paint Page as Form
            else if (container.Page.IsForm)
            {
                foreach (StiComponent component in container.Components)
                {
                    component.Paint(e);
                }
            }
            #endregion

            #region Paint components
            #region Paint In Printing Mode
            else if (container.IsPrinting)
            {
                var ee = e.Clone() as StiPaintEventArgs;
                var topmostComponents = new List<StiComponent>();

                ee.DrawBorderFormatting = true;
                ee.DrawTopmostBorderSides = false;

                #region Paint Components (Except Primitive & Band)::Except Topmost Border Sides
                foreach (StiComponent component in container.Components)
                {
                    if (component is StiPrimitive) continue;
                    if (component is StiBand) continue;
                    if (component.Printable)
                    {
                        if (component.TagValue != null && component.TagValue.ToString().ToLowerInvariant() == TopmostToken)
                            topmostComponents.Add(component);
                        else
                            component.Paint(ee);
                    }
                }
                #endregion

                #region Paint Bands::Except Topmost Border Sides
                foreach (StiComponent component in container.Components)
                {
                    if (component.Printable && component is StiBand)
                        component.Paint(ee);
                }
                #endregion

                ee.DrawBorderFormatting = false;
                ee.DrawTopmostBorderSides = true;

                #region Paint Components (Except Primitive & Band)::Only Topmost Border Sides
                foreach (StiComponent component in container.Components)
                {
                    if (component is StiPrimitive) continue;
                    if (component is StiBand) continue;

                    if (component.Printable)
                        component.Paint(ee);
                }
                #endregion

                #region Paint Bands::Only Topmost Border Sides
                foreach (StiComponent component in container.Components)
                {
                    if (component.Printable && component is StiBand)
                        component.Paint(ee);
                }
                #endregion

                ee.DrawBorderFormatting = true;
                ee.DrawTopmostBorderSides = true;

                #region Paint Primitives
                foreach (StiComponent component in container.Components)
                {
                    if (component.Printable && component is StiPrimitive)
                        component.Paint(ee);
                }
                #endregion

                #region Paint topmost components
                foreach (var component in topmostComponents)
                {
                    component.Paint(ee);
                }

                topmostComponents.Clear();
                #endregion
            }
            #endregion

            #region Paint In Drawing Mode
            else
            {
                var ee = e.Clone() as StiPaintEventArgs;
                var isDesigning = container.IsDesigning;
                var isOffsetNotEmpty = !container.Page.OffsetRectangle.IsEmpty;
                var topmostComponents = new List<StiComponent>();

                ee.DrawBorderFormatting = true;
                ee.DrawTopmostBorderSides = false;
                ee.AnimationEngine = AnimationEngine;

                foreach (StiComponent component in container.Components)
                {
                    var text = component as StiText;
                    if (text != null && text.ExceedMargins != StiExceedMargins.None)
                    {
                        var paintRect = component.GetPaintRectangle(true, true);

                        if (component is StiText)
                            new StiTextGdiPainter().PaintBackground(component as StiText, ee.Graphics, paintRect, true);
                    }
                }

                #region Paint Not Selected Components (Except Primitive & Band)::Except Topmost Border Sides
                foreach (StiComponent component in container.Components)
                {
                    if (component is StiPrimitive) continue;
                    if (component is StiBand) continue;

                    if (component.IsSelected && isDesigning && isOffsetNotEmpty)
                        container.Page.SelectedComponents.Add(component);

                    else
                    {
                        if (component.TagValue != null && component.TagValue.ToString().ToLowerInvariant() == TopmostToken)
                            topmostComponents.Add(component);

                        else
                            component.Paint(ee);
                    }
                }
                #endregion

                #region Paint Not Selected Bands::Except Topmost Border Sides
                foreach (StiComponent component in container.Components)
                {
                    if (!(component is StiBand)) continue;

                    if (component.IsSelected && isDesigning && isOffsetNotEmpty)
                        container.Page.SelectedComponents.Add(component);

                    else
                        component.Paint(ee);
                }
                #endregion

                ee.DrawBorderFormatting = false;
                ee.DrawTopmostBorderSides = true;

                #region Paint Not Selected Components (Except Primitive & Band)::Only Topmost Border Sides
                foreach (StiComponent component in container.Components)
                {
                    if (component is StiPrimitive) continue;
                    if (component is StiBand) continue;
                    if (component.IsSelected && isDesigning && isOffsetNotEmpty) continue;

                    component.Paint(ee);
                }
                #endregion

                #region Paint Not Selected Bands::Only Topmost Border Sides
                foreach (StiComponent component in container.Components)
                {
                    if (!(component is StiBand)) continue;
                    if (component.IsSelected && isDesigning && isOffsetNotEmpty) continue;

                    component.Paint(ee);
                }
                #endregion

                ee.DrawBorderFormatting = true;
                ee.DrawTopmostBorderSides = true;

                #region Paint Not Selected Primitives
                foreach (StiComponent component in container.Components)
                {
                    if (!(component is StiPrimitive)) continue;
                    if (component.IsSelected && isDesigning && isOffsetNotEmpty)
                        container.Page.SelectedComponents.Add(component);

                    else
                        component.Paint(ee);
                }
                #endregion

                #region Paint topmost components
                foreach (var component in topmostComponents)
                {
                    component.Paint(ee);
                }

                topmostComponents.Clear();
                #endregion

                #region Paint Band Interaction
                if (!container.IsDesigning)
                {
                    foreach (StiComponent component in container.Components)
                    {
                        PaintBandInteraction(component, e.Graphics);
                    }
                }
                #endregion
            }
            #endregion
            #endregion

            PaintSelectionComponents(container, e.Graphics);
        }

        /// <summary>
        /// Paints selection of components.
        /// </summary>
        public virtual void PaintSelectionComponents(StiContainer container, Graphics g)
        {
            if (!container.IsDesigning || container.Report.Info.IsComponentsMoving) return;

            if (!StiTableHelper.IsTableMode(container.Report.Designer))
            {
                foreach (StiComponent component in container.Components)
                {
                    if (component is IStiElement && component.ClientRectangle.IsEmpty) continue;
                    component.PaintSelection(new StiPaintEventArgs(g, RectangleD.Empty));
                }
            }
            else
            {
                var rect = StiTableHelper.GetSelectedRectangle(container.Report.Designer, true);
                
                if (rect.IsEmpty && container.Report.GetCurrentPage().IsDashboard)
                    StiDrawing.DrawSelectedRectangle(g, 2, Brushes.DimGray, rect);
            }
        }
        #endregion

        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            var container = component as StiContainer;

            if (!e.DrawBorderFormatting && e.DrawTopmostBorderSides && !container.Border.Topmost)
                return;

            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            if (e.DrawBorderFormatting)
                component.InvokePainting(component, e);

            var advWatermark = component.TagValue as StiAdvancedWatermark;
            if (advWatermark != null)
            {
                var rect = component.GetPaintRectangle();
                var scale = component.Page.Zoom * StiScale.Factor;
                StiAdvancedWatermarkGdiPainter.PaintWatermark(e.Graphics, advWatermark, null, rect, scale);
            }

            if (!e.Cancel && !(component.Enabled == false && component.IsDesigning == false))
            {
                var rect = component.GetPaintRectangle();

                if (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle))
                    PaintContainerContent(e, rect, container);
            }

            e.Cancel = false;

            if (e.DrawBorderFormatting)
            {
                component.InvokePainted(component, e);

                PaintColumns(component as StiContainer, e.Graphics);
            }

            PaintComponents(component as StiContainer, e);
        }

        internal void PaintContainerContent(StiPaintEventArgs e, RectangleD rect, StiContainer container)
        {
            if (!(rect.Width > 0) || !(rect.Height > 0)) return;

            var g = e.Graphics;

            #region Fill rectangle
            if (e.DrawBorderFormatting)
            {
                if (StiBrush.IsTransparent(container.Brush) &&
                    container.Report != null &&
                    container.Report.Info.FillContainer &&
                    container.IsDesigning)
                {
                    var color = Color.FromArgb(150, Color.White);
                    StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
                }
                else
                    StiDrawing.FillRectangle(g, container.Brush, rect);
            }
            #endregion

            #region Container name
            if (e.DrawBorderFormatting && container.IsDesigning)
            {
                using (var stringFormat = new StringFormat())
                using (var font = new Font("Segoe UI", (float) (15 * container.Page.Zoom)))
                using (var brush = new SolidBrush(Color.FromArgb(146, 146, 146)))
                {
                    stringFormat.LineAlignment = StringAlignment.Center;
                    stringFormat.Alignment = StringAlignment.Center;

                    StiTextDrawing.DrawString(g, container.Name, font, brush,
                        new RectangleD(rect.Left, rect.Top, rect.Width, rect.Height), stringFormat);
                }
            }
            #endregion

            #region Markers
            if (e.DrawBorderFormatting && container.HighlightState == StiHighlightState.Hide && container.Border.Side != StiBorderSides.All)
                PaintMarkers(container, g, rect);
            #endregion

            #region Border
            if (e.DrawBorderFormatting && container.Border.Side == StiBorderSides.None && container.IsDesigning)
            {
                using (var pen = new Pen(Color.FromArgb(128, 128, 128)))
                {
                    pen.DashStyle = DashStyle.Dash;
                    StiDrawing.DrawRectangle(g, pen, rect.Left, rect.Top, rect.Width, rect.Height);
                }
            }

            if (container.HighlightState == StiHighlightState.Hide)
            {
                var zoom = container.Page != null ? container.Page.Zoom : 1f;
                PaintBorder(container, g, rect, zoom, e.DrawBorderFormatting, e.DrawTopmostBorderSides || !container.Border.Topmost);
            }
            #endregion

            if (e.DrawBorderFormatting)
            {
                PaintEvents(container, g, rect);
                PaintConditions(container, g, rect);
            }
        }
        #endregion
    }
}