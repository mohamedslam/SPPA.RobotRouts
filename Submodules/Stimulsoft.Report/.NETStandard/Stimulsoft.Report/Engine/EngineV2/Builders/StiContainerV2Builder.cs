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
using System.Collections;
using Stimulsoft.Base.Blocks;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;

namespace Stimulsoft.Report.Engine
{
    public class StiContainerV2Builder : StiComponentV2Builder
    {
        #region Methods.Helpers
        /// <summary>
        /// Returns a panel for the component rendering.
        /// </summary>
        /// <param name="comp">The component that is a base for a component preparation.</param>
        /// <returns>Panel for rendering.</returns>
        public virtual StiContainer GetRenderContainer(StiComponent comp, Type type = null)
        {
            StiContainer container;

            if (type != null)
                container = Activator.CreateInstance(type, comp.ClientRectangle) as StiContainer;

            else
            {
                if (comp is StiPanel)
                {
                    container = new StiPanel(comp.ClientRectangle);
                    ((StiPanel) container).Columns = ((StiPanel) comp).Columns;
                }
                else
                    container = new StiContainer(comp.ClientRectangle);
            }

            if (!comp.IsGetPointerHandlerEmpty)
                container.GetPointer += comp.InvokeGetPointer;

            if (!comp.IsGetBookmarkHandlerEmpty)
                container.GetBookmark += comp.InvokeGetBookmark;

            if (!comp.IsGetTagHandlerEmpty)
                container.GetTag += comp.InvokeGetTag;

            if (!comp.IsGetHyperlinkHandlerEmpty)
                container.GetHyperlink += comp.InvokeGetHyperlink;

            if (!comp.IsClickHandlerEmpty)
                container.Click += comp.InvokeClick;

            if (!comp.IsDoubleClickHandlerEmpty)
                container.DoubleClick += comp.InvokeDoubleClick;

            if (!comp.IsMouseEnterHandlerEmpty)
                container.MouseEnter += comp.InvokeMouseEnter;

            if (!comp.IsMouseLeaveHandlerEmpty)
                container.MouseLeave += comp.InvokeMouseLeave;

            container.AfterPrint += comp.InvokeAfterPrint;
            container.BeforePrint += comp.InvokeBeforePrint;

            if (comp.Properties.IsPresent("GetTagEvent"))
            {
                var stiEvent = comp.GetTagEvent;
                if (stiEvent.Script.StartsWith(StiBlocksConst.IdentXml))
                    container.GetTagEvent = stiEvent;
            }
            if (comp.Properties.IsPresent("GetToolTipEvent"))
            {
                var stiEvent = comp.GetToolTipEvent;
                if (stiEvent.Script.StartsWith(StiBlocksConst.IdentXml))
                    container.GetToolTipEvent = stiEvent;
            }
            if (comp.Properties.IsPresent("GetBookmarkEvent"))
            {
                var stiEvent = comp.GetBookmarkEvent;
                if (stiEvent.Script.StartsWith(StiBlocksConst.IdentXml))
                    container.GetBookmarkEvent = stiEvent;
            }
            if (comp.Properties.IsPresent("GetHyperlinkEvent"))
            {
                var stiEvent = comp.GetHyperlinkEvent;
                if (stiEvent.Script.StartsWith(StiBlocksConst.IdentXml))
                    container.GetHyperlinkEvent = stiEvent;
            }

            container.MinSize = comp.MinSize;
            container.MaxSize = comp.MaxSize;
            container.DockStyle = comp.DockStyle;
            container.Name = comp.Name;
            container.ParentComponentIsBand = comp is StiBand;
            container.ParentComponentIsCrossBand = comp is StiCrossDataBand;
            container.CanBreak = ((StiContainer)comp).CanBreak;
            container.CanGrow = comp.CanGrow;
            container.CanShrink = comp.CanShrink;
            container.GrowToHeight = comp.GrowToHeight;
            container.ShiftMode = comp.ShiftMode;
            container.Printable = comp.Printable;
            container.PrintOn = comp.PrintOn;
            container.ComponentStyle = comp.ComponentStyle;
            container.CurrentBookmark = comp.CurrentBookmark;
            container.ParentBookmark = comp.ParentBookmark;
            container.CurrentPointer = comp.CurrentPointer;
            container.ParentPointer = comp.ParentPointer;
            container.Guid = comp.Guid;
            container.Interaction = comp.Interaction;
            container.DrillDownParameters = comp.DrillDownParameters;

            if (comp is StiGroupHeaderBand)
            {
                container.CollapsingIndex = ((StiGroupHeaderBand)comp).CollapsingIndex;
                container.CollapsedValue = ((StiGroupHeaderBand)comp).CollapsedValue;
            }

            if (comp is StiDataBand)
            {
                container.CollapsingIndex = ((StiDataBand)comp).CollapsingIndex;
                container.CollapsedValue = ((StiDataBand)comp).CollapsedValue;
                container.CollapsingTreePath = ((StiDataBand)comp).CollapsingTreePath;
            }

            var band = comp as StiBand;
            if (band != null)
            {
                if (band.BandInfoV2.ForceCanBreak) container.CanBreak = true;
                if (band.BandInfoV2.ForceCanGrow) container.CanGrow = true;
            }

            var cont = comp as StiContainer;
            container.Brush = cont.Brush.Clone() as StiBrush;
            container.Border = cont.Border.Clone() as StiBorder;

            if (StiOptions.Export.OptimizeDataOnlyMode)
                container.ComponentPlacement = comp.ComponentPlacement;

            return container;
        }
        #endregion

        #region Methods.Render
        public override StiComponent InternalRender(StiComponent masterComp)
        {
            var masterContainer = masterComp as StiContainer;
            var isSubReportMode = false;
            var renderedContainer = GetRenderContainer(masterContainer);

            if (masterContainer.Report.CalculationMode == StiCalculationMode.Interpretation)
            {
                renderedContainer.Pointer = masterContainer.Pointer;
                renderedContainer.Bookmark = masterContainer.Bookmark;
                renderedContainer.Hyperlink = masterContainer.Hyperlink;
                renderedContainer.Tag = masterContainer.Tag;
            }

            #region Render Simple Components
            if (!(masterContainer is IStiComponentsOwnerRenderer))
            {
                lock (((ICollection) masterContainer.Components).SyncRoot)
                {
                    foreach (StiComponent component in masterContainer.Components)
                    {
                        if (component.ParentBookmark == null)
                            component.ParentBookmark = masterContainer.CurrentBookmark;

                        if (component.ParentPointer == null)
                            component.ParentPointer = masterContainer.CurrentPointer;

                        if (component.ComponentType == StiComponentType.Simple)
                        {
                            component.ParentBookmark = masterContainer.CurrentBookmark;
                            component.ParentPointer = masterContainer.CurrentPointer;

                            var comp = component.Render();
                            if (comp != null)
                            {
                                var cont = comp as StiContainer;
                                if (cont != null && cont.ContainerInfoV2.SetSegmentPerWidth != -1)
                                    renderedContainer.ContainerInfoV2.SetSegmentPerWidth = cont.ContainerInfoV2.SetSegmentPerWidth;

                                renderedContainer.Components.Add(comp);

                                #region StiOptions.Engine.AllowInteractionInChartWithComponents
                                if (StiOptions.Engine.AllowInteractionInChartWithComponents)
                                {
                                    var chart = comp as Stimulsoft.Report.Chart.StiChart;
                                    if (chart != null && chart.ChartInfoV2.InteractiveComps != null)
                                    {
                                        foreach (var comp2 in chart.ChartInfoV2.InteractiveComps)
                                        {
                                            comp2.Left += comp.Left;
                                            comp2.Top += comp.Top;
                                            renderedContainer.Components.Add(comp2);
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }
            }
            #endregion

            if (!masterContainer.Report.Engine.DenyRenderMasterComponentsInContainer)
            {
                if (!(masterContainer is IStiComponentsOwnerRenderer))
                {
                    lock (((ICollection) masterContainer.Components).SyncRoot)
                    {
                        foreach (StiComponent component in masterContainer.Components)
                        {
                            component.ParentBookmark = masterContainer.CurrentBookmark;
                            component.ParentPointer = masterContainer.CurrentPointer;

                            if (component.ComponentType == StiComponentType.Master)
                            {
                                component.Render();

                                if (component is StiDataBand) 
                                    isSubReportMode = true;
                            }
                        }
                    }
                }
            }

            if (isSubReportMode)
            {
                var skipStaticBands = false;
                var container = masterContainer;
                while (true)
                {
                    if (container is StiPageHeaderBand ||
                        container is StiPageFooterBand ||
                        container is StiReportTitleBand ||
                        container is StiReportSummaryBand)
                    {
                        skipStaticBands = true;
                        break;
                    }

                    if (container == null || container is StiPage)
                        break;
                    container = container.Parent;
                }
                StiSubReportsHelper.RenderDataBandsInContainer(renderedContainer, masterContainer, skipStaticBands);
            }

            var reservedWidth = renderedContainer.Width;
            StiContainerHelper.CheckSize(renderedContainer);
            if (!renderedContainer.ParentComponentIsCrossBand)
                renderedContainer.Width = reservedWidth;

            return renderedContainer;
        }
        #endregion
    }
}
