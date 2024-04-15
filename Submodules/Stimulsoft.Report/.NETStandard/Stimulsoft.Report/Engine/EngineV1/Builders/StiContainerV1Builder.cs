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
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
    public class StiContainerV1Builder : StiComponentV1Builder
    {
        #region Methods.Helpers
        /// <summary>
        /// Returns a collection of clones.
        /// </summary>
        /// <returns>Components collection.</returns>
        public static StiComponentsCollection GetClones(StiContainer masterContainer)
        {
            var comps = masterContainer.Page.GetComponents();
            var cloneComponents = new StiComponentsCollection(null);

            foreach (StiComponent comp in comps)
            {
                if (comp is StiClone && ((StiClone)comp).Container == masterContainer) cloneComponents.Add(comp);
            }

            return cloneComponents;
        }

        /// <summary>
        /// Returns a panel for the component rendering.
        /// </summary>
        /// <param name="comp">The component that is a base for a component preparation.</param>
        /// <returns>Panel for rendering.</returns>
        public static StiContainer GetRenderContainer(StiContainer outContainer, StiComponent comp)
        {
            StiContainer container = null;
            StiContainer cont = comp as StiContainer;

            if (cont.ContainerInfoV1.IsColumn &&
                comp.Page.Columns > 1 &&
                comp.Page.PrintOnPreviousPage &&
                comp.Page.PrintHeadersFootersFromPreviousPage)
            {
                foreach (StiComponent pComp in outContainer.Components)
                {
                    if (pComp.Name == comp.Name) return pComp as StiContainer;
                }
            }

            RectangleD rect = new RectangleD(
                comp.ClientRectangle.X,
                comp.ClientRectangle.Y,
                comp.ClientRectangle.Width,
                comp.ClientRectangle.Height);

            container = new StiContainer(rect);

            container.GetPointer += comp.InvokeGetPointer;
            container.GetBookmark += comp.InvokeGetBookmark;
            container.GetTag += comp.InvokeGetTag;
            container.Click += comp.InvokeClick;
            container.MouseEnter += comp.InvokeMouseEnter;
            container.MouseLeave += comp.InvokeMouseLeave;
            container.AfterPrint += comp.InvokeAfterPrint;
            container.BeforePrint += comp.InvokeBeforePrint;

            container.MinSize = comp.MinSize;
            container.MaxSize = comp.MaxSize;
            container.DockStyle = comp.DockStyle;
            container.Name = comp.Name;
            container.ParentComponentIsBand = comp is StiBand;
            container.CanGrow = comp.CanGrow;
            container.CanShrink = comp.CanShrink;
            container.GrowToHeight = comp.GrowToHeight;
            container.ShiftMode = comp.ShiftMode;
            container.Printable = comp.Printable;
            container.ComponentStyle = comp.ComponentStyle;

            if (comp is StiContainer)
                container.CollapsingIndex = ((StiContainer)comp).CollapsingIndex;

            container.Guid = comp.Guid;
            container.ContainerInfoV1.IsColumn = cont.ContainerInfoV1.IsColumn;
            container.Brush = cont.Brush.Clone() as StiBrush;
            container.Border = cont.Border.Clone() as StiBorder;
            container.Interaction = comp.Interaction;

            return container;
        }

        public void CheckBandsAtBottom(StiContainer masterContainer, StiComponent renderedComponent, StiContainer outContainer)
        {
            bool emptyBandResult = RenderEmptyBands(masterContainer, renderedComponent as StiContainer);
            RenderGroupFootersAtBottom(masterContainer, !emptyBandResult);
            RenderFootersAtBottom(masterContainer, !emptyBandResult);
            RenderDataBandsAtBottom(masterContainer, outContainer, !emptyBandResult);
            RenderHeadersAtBottom(masterContainer, !emptyBandResult);
        }

        /// <summary>
        /// Move to the first component.
        /// </summary>
        public void FirstComponent(StiContainer masterContainer)
        {
            foreach (StiComponent component in masterContainer.Components)
            {
                if (component.ComponentType == StiComponentType.Master)
                {
                    masterContainer.ContainerInfoV1.CurrentComponent = component;
                    return;
                }
            }

            masterContainer.ContainerInfoV1.CurrentComponent = null;
        }

        /// <summary>
        /// Move to the next component.
        /// </summary>
        public void NextComponent(StiContainer masterContainer)
        {
            for (int index = masterContainer.Components.IndexOf(masterContainer.ContainerInfoV1.CurrentComponent) + 1;
                index < masterContainer.Components.Count;
                index++)
            {
                if (masterContainer.Components[index].ComponentType == StiComponentType.Master)
                {
                    masterContainer.ContainerInfoV1.CurrentComponent = masterContainer.Components[index];
                    return;
                }
            }

            masterContainer.ContainerInfoV1.CurrentComponent = null;
        }

        /// <summary>
        /// Renders a container in the specified place. This is a basic method of container rendering.
        /// </summary>
        /// <param name="renderedComponent">Rendered container.</param>
        /// <param name="outContainer">Container for rendering.</param>
        /// <returns>If the container is rendered completely then true.</returns>
        public bool InternalCoreRenderContainer(StiContainer masterContainer, ref StiComponent renderedComponent, StiContainer outContainer)
        {
            bool isRendered = true;

            masterContainer.IsRendered = true;

            if (renderedComponent == null) renderedComponent = GetRenderContainer(outContainer, masterContainer);

            if (renderedComponent != outContainer)
            {
                outContainer.Components.Add(renderedComponent);
                if (renderedComponent.Dockable)
                    renderedComponent.DisplayRectangle = renderedComponent.DockToContainer(renderedComponent.DisplayRectangle);
            }

            #region Render simple components
            foreach (StiComponent comp in masterContainer.Components)
            {
                if (comp.ComponentType == StiComponentType.Simple || comp.ComponentType == StiComponentType.Static)
                {
                    comp.ParentBookmark = masterContainer.CurrentBookmark;

                    bool pr = comp.Render(renderedComponent as StiContainer);
                    if (!pr) isRendered = false;
                }
            }

            StiContainerHelper.CheckSize(renderedComponent);
            #endregion

            if (outContainer is StiPage && masterContainer is StiPage)
            {
                StiPage page = outContainer as StiPage;
                RectangleD rect = outContainer.GetDockRegion(outContainer);
                page.PageInfoV1.TopPageContentPosition = rect.Y;
                page.PageInfoV1.BottomPageContentPosition = rect.Bottom;
            }

            do
            {
                //If no current object that gets first complex object
                if (masterContainer.ContainerInfoV1.CurrentComponent == null)
                {
                    FirstComponent(masterContainer);
                    //If don't care no current object, that leaves - no complex object for printing
                    //All have printed successfully, printing the container is finished
                    if (masterContainer.ContainerInfoV1.CurrentComponent == null) return isRendered;
                }

                masterContainer.ContainerInfoV1.CurrentComponent.ParentBookmark = masterContainer.CurrentBookmark;

                //If an object is not printed, that leaves
                //Printing shall continue in the next time
                if (!masterContainer.ContainerInfoV1.CurrentComponent.Render(renderedComponent as StiContainer)) return false;
                else
                {
                    //Printing the current object is passed successfully,
                    //get the next object
                    NextComponent(masterContainer);
                    //If an object is not received, signifies all objects is printed
                    //All have printed successfully, printing the container is finished
                    if (masterContainer.ContainerInfoV1.CurrentComponent == null) return isRendered;
                    //Continue to print
                }

            }
            while (true);
        }

        /// <summary>
        /// Renders a container without invokes events. These methods process bands at bottom.
        /// </summary>
        /// <param name="renderedComponent">A rendered container.</param>
        /// <param name="outContainer">A container for rendering.</param>
        /// <returns>If a container is rendered completely then true.</returns>
        public virtual bool InternalRenderContainer(StiContainer masterContainer, ref StiComponent renderedComponent, StiContainer outContainer)
        {
            bool result = InternalCoreRenderContainer(masterContainer, ref renderedComponent, outContainer);

            //if (ContainsBottomRenderedBands)
            CheckBandsAtBottom(masterContainer, renderedComponent, outContainer);

            return result;
        }

        /// <summary>
        /// Renders a container without invokes events. This method processes Childs of container. Method calls InternalRenderContainer.
        /// </summary>
        /// <param name="renderedComponent">A rendered container.</param>
        /// <param name="outContainer">A container for rendering.</param>
        /// <returns>If a container is rendered completely then true.</returns>
        public virtual bool RenderContainer(StiContainer masterContainer, ref StiComponent renderedComponent, StiContainer outContainer)
        {
            bool results = true;

            results = InternalRenderContainer(masterContainer, ref renderedComponent, outContainer);
            if (masterContainer is StiBand && (!(masterContainer is StiChildBand)))
            {
                StiBandV1Builder.RenderChilds(masterContainer as StiBand, (StiContainer)renderedComponent);
            }

            if (!(masterContainer is StiPage)) renderedComponent.InvokeEvents();

            return results;
        }
        #endregion

        #region Methods.Helper.PrintAtBottom
        private bool RenderEmptyBands(StiContainer masterContainer, StiContainer outContainer)
        {
            StiEmptyBand selectedBand = null;
            StiComponentsCollection emptyBands = null;
            foreach (StiComponent comp in masterContainer.Components)
            {
                var band = comp as StiEmptyBand;
                if (band == null || !(band.Height > 0) || !band.IsEnabled) continue;

                if (emptyBands == null)
                    emptyBands = new StiComponentsCollection();

                emptyBands.Add(band);
            }

            if (emptyBands != null && emptyBands.Count > 0)
                selectedBand = emptyBands[0] as StiEmptyBand;

            if (selectedBand != null)
            {
                StiEmptyBandsV1Helper.Render(masterContainer.Report, outContainer, selectedBand);
                return true;
            }
            return false;
        }

        private void RenderDataBandsAtBottom(StiContainer masterContainer, StiContainer outContainer, bool dockToBottom)
        {
            if (masterContainer.ContainerInfoV1.BottomRenderedParentsDataBands != null)
            {
                foreach (StiDataBand band in masterContainer.ContainerInfoV1.BottomRenderedParentsDataBands)
                {
                    if (band.PrintAtBottom)
                    {
                        var comps = masterContainer.ContainerInfoV1.BottomRenderedDataBands[band] as StiComponentsCollection;

                        for (int index = comps.Count - 1; index >= 0; index--)
                        {
                            StiComponent comp = comps[index];
                            StiContainer parent = comp.Parent;

                            if (parent != null && parent.Components.IndexOf(comp) != -1)
                            {
                                parent.Components.Remove(comp);
                                parent.Components.Add(comp);
                                if (dockToBottom) comp.DockStyle = StiDockStyle.Bottom;
                            }
                        }
                    }
                }
                masterContainer.ContainerInfoV1.BottomRenderedParentsDataBands.Clear();
                masterContainer.ContainerInfoV1.BottomRenderedParentsDataBands = null;

                masterContainer.ContainerInfoV1.BottomRenderedDataBands.Clear();
                masterContainer.ContainerInfoV1.BottomRenderedDataBands = null;
            }
        }

        private void RenderHeadersAtBottom(StiContainer masterContainer, bool dockToBottom)
        {
            if (masterContainer.ContainerInfoV1.BottomRenderedParentsHeaders != null)
            {
                foreach (StiHeaderBand header in masterContainer.ContainerInfoV1.BottomRenderedParentsHeaders)
                {
                    if (header.PrintAtBottom)
                    {
                        var comps = masterContainer.ContainerInfoV1.BottomRenderedHeaders[header] as StiComponentsCollection;

                        for (int index = comps.Count - 1; index >= 0; index--)
                        {
                            StiComponent comp = comps[index];
                            StiContainer parent = comp.Parent;

                            if (parent != null && parent.Components.IndexOf(comp) != -1)
                            {
                                parent.Components.Remove(comp);
                                parent.Components.Add(comp);
                                if (dockToBottom) comp.DockStyle = StiDockStyle.Bottom;

                                break;
                            }
                        }
                    }
                }
                masterContainer.ContainerInfoV1.BottomRenderedParentsHeaders.Clear();
                masterContainer.ContainerInfoV1.BottomRenderedParentsHeaders = null;

                masterContainer.ContainerInfoV1.BottomRenderedHeaders.Clear();
                masterContainer.ContainerInfoV1.BottomRenderedHeaders = null;
            }
        }

        private void RenderFootersAtBottom(StiContainer masterContainer, bool dockToBottom)
        {
            if (masterContainer.ContainerInfoV1.BottomRenderedParentsFooters != null)
            {
                if (!dockToBottom)
                {
                    for (int indexFooter = masterContainer.ContainerInfoV1.BottomRenderedParentsFooters.Count - 1; indexFooter >= 0; indexFooter--)
                    {
                        StiFooterBand footer = masterContainer.ContainerInfoV1.BottomRenderedParentsFooters[indexFooter] as StiFooterBand;
                        RenderOneFooterAtBottom(masterContainer, dockToBottom, footer);
                    }
                }
                else
                {
                    foreach (StiFooterBand footer in masterContainer.ContainerInfoV1.BottomRenderedParentsFooters)
                    {
                        RenderOneFooterAtBottom(masterContainer, dockToBottom, footer);
                    }
                }

                masterContainer.ContainerInfoV1.BottomRenderedParentsFooters.Clear();
                masterContainer.ContainerInfoV1.BottomRenderedParentsFooters = null;

                masterContainer.ContainerInfoV1.BottomRenderedFooters.Clear();
                masterContainer.ContainerInfoV1.BottomRenderedFooters = null;
            }
        }

        private void RenderOneFooterAtBottom(StiContainer masterContainer, bool dockToBottom, StiFooterBand footer)
        {
            if (footer.PrintAtBottom)
            {
                StiComponentsCollection comps = masterContainer.ContainerInfoV1.BottomRenderedFooters[footer] as StiComponentsCollection;

                for (int index = comps.Count - 1; index >= 0; index--)
                {
                    StiComponent comp = comps[index];
                    StiContainer parent = comp.Parent;

                    if (parent != null && parent.Components.IndexOf(comp) != -1)
                    {
                        parent.Components.Remove(comp);
                        parent.Components.Add(comp);
                        if (dockToBottom) comp.DockStyle = StiDockStyle.Bottom;

                        break;
                    }
                }
            }
        }

        private void RenderGroupFootersAtBottom(StiContainer masterContainer, bool dockToBottom)
        {
            if (masterContainer.ContainerInfoV1.BottomRenderedParentsGroupFooters != null)
            {
                if (!dockToBottom)
                {
                    for (int indexFooter = masterContainer.ContainerInfoV1.BottomRenderedParentsGroupFooters.Count - 1; indexFooter >= 0; indexFooter--)
                    {
                        StiGroupFooterBand footer = masterContainer.ContainerInfoV1.BottomRenderedParentsGroupFooters[indexFooter] as StiGroupFooterBand;
                        RenderOneGroupFooterAtBottom(masterContainer, dockToBottom, footer);
                    }
                }
                else
                {
                    foreach (StiGroupFooterBand footer in masterContainer.ContainerInfoV1.BottomRenderedParentsGroupFooters)
                    {
                        RenderOneGroupFooterAtBottom(masterContainer, dockToBottom, footer);
                    }
                }

                masterContainer.ContainerInfoV1.BottomRenderedParentsGroupFooters.Clear();
                masterContainer.ContainerInfoV1.BottomRenderedParentsGroupFooters = null;

                masterContainer.ContainerInfoV1.BottomRenderedGroupFooters.Clear();
                masterContainer.ContainerInfoV1.BottomRenderedGroupFooters = null;
            }
        }

        private void RenderOneGroupFooterAtBottom(StiContainer masterContainer, bool dockToBottom, StiGroupFooterBand footer)
        {
            if (footer.PrintAtBottom)
            {
                StiComponentsCollection comps = masterContainer.ContainerInfoV1.BottomRenderedGroupFooters[footer] as StiComponentsCollection;

                for (int index = comps.Count - 1; index >= 0; index--)
                {
                    StiComponent comp = comps[index];
                    StiContainer parent = comp.Parent;

                    if (parent != null && parent.Components.IndexOf(comp) != -1)
                    {
                        parent.Components.Remove(comp);
                        parent.Components.Add(comp);
                        if (dockToBottom) comp.DockStyle = StiDockStyle.Bottom;

                        break;
                    }
                }
            }
        }
        #endregion

        #region Methods.Render
        /// <summary>
        /// Prepares a component for rendering.
        /// </summary>
        public override void Prepare(StiComponent masterComp)
        {
            base.Prepare(masterComp);

            StiContainer masterContainer = masterComp as StiContainer;

            foreach (StiComponent comp in masterContainer.Components) comp.Prepare();

            masterContainer.ContainerInfoV1.CurrentComponent = null;
            masterContainer.ContainerInfoV1.CloneComponents = GetClones(masterContainer);
        }

        /// <summary>
        /// Clears a component after rendering.
        /// </summary>
        public override void UnPrepare(StiComponent masterComp)
        {
            base.UnPrepare(masterComp);

            StiContainer masterContainer = masterComp as StiContainer;

            foreach (StiComponent comp in masterContainer.Components) comp.UnPrepare();
        }

        /// <summary>
        /// Renders a component in the specified container without taking generation of BeforePrintEvent and AfterPrintEvent into consideration and without taking 
        /// Conditions into consideration. A rendered component is returned in the renderedComponent argument.
        /// </summary>
        /// <param name="renderedComponent">A rendered component.</param>
        /// <param name="outContainer">A panel in what rendering will be done.</param>
        /// <returns>Is rendering finished or not.</returns>
        public override bool InternalRender(StiComponent masterComp, ref StiComponent renderedComponent, StiContainer outContainer)
        {
            StiContainer masterContainer = masterComp as StiContainer;

            #region Render Container
            #region If container is first column
            if (masterContainer.Name == "#Column#1")
                masterContainer.Page.InvokeColumnBeginRender(masterContainer, EventArgs.Empty);
            #endregion

            #region Runtime variable
            if (masterContainer.ContainerInfoV1.CurrentColumn != 0) masterContainer.Report.Column = masterContainer.ContainerInfoV1.CurrentColumn;
            #endregion

            //Render master component
            bool masterResult = masterContainer.RenderContainer(ref renderedComponent, outContainer);
            ((StiContainer)renderedComponent).ContainerInfoV1.CurrentColumn = 1;

            #region If container is first column
            if (masterContainer.Name == "#Column#1")
                masterContainer.Page.InvokeColumnEndRender(masterContainer, EventArgs.Empty);
            #endregion
            #endregion

            #region Render clones
            if (masterContainer.ContainerInfoV1.CloneComponents != null && masterContainer.ContainerInfoV1.CloneComponents.Count > 0)
            {
                bool detailResult = false;
                int index = 2;
                foreach (StiClone clone in masterContainer.ContainerInfoV1.CloneComponents)
                {
                    clone.ContainerInfoV1.CurrentColumn = index;

                    StiComponent comp = null;

                    #region If clone is column
                    if (clone.Name.StartsWith("#Column#", StringComparison.InvariantCulture))
                        masterContainer.Page.InvokeColumnBeginRender(masterContainer, EventArgs.Empty);
                    #endregion

                    bool result = clone.Render(ref comp, outContainer);


                    #region If clone is column
                    if (clone.Name.StartsWith("#Column#", StringComparison.InvariantCulture))
                        masterContainer.Page.InvokeColumnEndRender(masterContainer, EventArgs.Empty);
                    #endregion

                    if (result)
                    {
                        detailResult = true;
                    }
                    index++;
                }
                if (detailResult) masterResult = true;
            }
            #endregion

            #region Runtime variable
            if (masterContainer.ContainerInfoV1.CurrentColumn != 0) masterContainer.Report.Column = 0;
            #endregion

            return masterResult;
        }
        #endregion
    }
}
