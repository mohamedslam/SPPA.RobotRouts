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
using System.Collections;
using System.ComponentModel;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
    public class StiDataBandV1Builder : StiBandV1Builder
    {
        #region Methods.Helpers.Breakable
        public void SecondPassBreak(StiDataBand masterDataBand, StiComponent renderedComponent, StiComponent masterComponent, ref double freeSpace)
        {
            masterDataBand.DataBandInfoV1.IsFirstPassOfBreak = true;
            if ((masterDataBand.DataBandInfoV1.BreakableComps != null) && (masterDataBand.DataBandInfoV1.BreakableComps.Count > 0))
            {
                masterDataBand.DataBandInfoV1.IsFirstPassOfBreak = false;
                StiComponentDivider.ProcessPreviousBreakedContainer(renderedComponent as StiContainer, masterDataBand.DataBandInfoV1.BreakableComps, ref freeSpace);

                masterDataBand.DataBandInfoV1.BreakableComps = null;
            }
        }

        public bool FirstPassBreak(StiDataBand masterDataBand, StiComponent renderedComponent, StiComponent masterComponent, ref double freeSpace)
        {
            renderedComponent.Height += freeSpace;
            masterDataBand.DataBandInfoV1.BreakableComps = StiComponentDivider.BreakContainer(renderedComponent as StiContainer);

            if ((renderedComponent.Parent is StiPage) && !masterDataBand.DataBandInfoV1.IsFirstPassOfBreak && ((renderedComponent as StiContainer).Components.Count == 0))
            {
                (renderedComponent as StiContainer).Components.AddRange(masterDataBand.DataBandInfoV1.BreakableComps);
                masterDataBand.DataBandInfoV1.BreakableComps = null;
                masterDataBand.DataBandInfoV1.IsFirstPassOfBreak = true;
                freeSpace = 0;
                return false;
            }
            return true;
        }
        #endregion

        #region Methods.Helpers.GetComponents
        /// <summary>
        /// Returns a collection of group headers.
        /// </summary>
        /// <returns>Components collection.</returns>
        public virtual StiComponentsCollection GetGroupHeaders(StiDataBand masterDataBand)
        {
            StiComponentsCollection comps = new StiComponentsCollection();
            int index = masterDataBand.Parent.Components.IndexOf(masterDataBand) - 1;

            if (masterDataBand is Stimulsoft.Report.Components.Table.StiTable && !masterDataBand.Enabled)
            {
                string name = masterDataBand.Name + "_";
                while (index >= 0 && masterDataBand.Parent.Components[index].Name.StartsWith(name)) index--;
            }

            while (index >= 0)
            {
                var tempComp = masterDataBand.Parent.Components[index];
                if (!(tempComp is StiHeaderBand))
                {
                    if (tempComp is StiChildBand)
                    {
                    }
                    else if (tempComp is StiEmptyBand)
                    {
                    }
                    else if (tempComp is StiGroupHeaderBand)
                    {
                        comps.Insert(0, tempComp);
                    }
                    else
                        break;
                }

                index--;
            }

            return comps;
        }

        /// <summary>
        /// Returns a collection of group footers.
        /// </summary>
        /// <returns>Components collection.</returns>
        public virtual StiComponentsCollection GetGroupFooters(StiDataBand masterDataBand)
        {
            StiComponentsCollection comps = new StiComponentsCollection();
            int index = masterDataBand.Parent.Components.IndexOf(masterDataBand) + 1;

            while (index < masterDataBand.Parent.Components.Count)
            {
                if (!(masterDataBand.Parent.Components[index] is StiFooterBand))
                {
                    if (masterDataBand.Parent.Components[index] is StiChildBand)
                    {
                    }
                    else if (masterDataBand.Parent.Components[index] is StiEmptyBand)
                    {
                    }
                    else if (masterDataBand.Parent.Components[index] is StiGroupFooterBand)
                    {
                        comps.Add(masterDataBand.Parent.Components[index]);
                    }
                    else
                        break;
                }

                index++;
            }

            return comps;
        }

        /// <summary>
        /// Returns a collection of childs components.
        /// </summary>
        /// <returns>Components collection.</returns>
        public virtual StiComponentsCollection GetDetails(StiDataBand masterDataBand)
        {
            StiComponentsCollection comps = new StiComponentsCollection();

            if (masterDataBand.Parent != null)
            {
                StiComponentsCollection childComps = masterDataBand.Parent.Components;

                foreach (StiComponent component in childComps)
                {
                    if ((!(component is StiEmptyBand)) &&
                        component != masterDataBand &&
                        component is IStiMasterComponent &&
                        ((IStiMasterComponent) component).MasterComponent == masterDataBand)
                    {
                        comps.Add(component);
                    }
                }
            }

            return comps;
        }

        /// <summary>
        /// Returns a collection of data childs components.
        /// </summary>
        /// <returns>Components collection.</returns>
        public virtual StiComponentsCollection GetDataDetails(StiDataBand masterDataBand)
        {
            StiComponentsCollection comps = new StiComponentsCollection();

            if (masterDataBand.Parent != null)
            {
                StiComponentsCollection childComps = masterDataBand.Page.GetComponents();

                foreach (StiComponent component in childComps)
                {
                    if (component is IStiMasterComponent &&
                        ((IStiMasterComponent) component).MasterComponent == masterDataBand)
                    {
                        comps.Add(component);
                    }
                }
            }

            return comps;
        }

        /// <summary>
        /// Returns a collection of headers for the object.
        /// </summary>
        public virtual StiComponentsCollection GetHeaders(StiDataBand masterDataBand)
        {
            var comps = new StiComponentsCollection();
            int index = masterDataBand.Parent.Components.IndexOf(masterDataBand) - 1;

            while (index >= 0)
            {
                if (!(masterDataBand.Parent.Components[index] is StiGroupHeaderBand))
                {
                    if (masterDataBand.Parent.Components[index] is StiChildBand)
                    {
                    }
                    else if (masterDataBand.Parent.Components[index] is StiEmptyBand)
                    {
                    }
                    else if (masterDataBand.Parent.Components[index] is StiHeaderBand)
                    {
                        comps.Insert(0, masterDataBand.Parent.Components[index]);
                    }
                    else
                        break;
                }

                index--;
            }

            return comps;
        }

        /// <summary>
        /// Returns a collection of footers of an object.
        /// </summary>
        public virtual StiComponentsCollection GetFooters(StiDataBand masterDataBand)
        {
            StiComponentsCollection comps = new StiComponentsCollection();
            int index = masterDataBand.Parent.Components.IndexOf(masterDataBand) + 1;

            while (index < masterDataBand.Parent.Components.Count)
            {
                if (!(masterDataBand.Parent.Components[index] is StiGroupFooterBand))
                {
                    if (masterDataBand.Parent.Components[index] is StiChildBand)
                    {
                    }
                    else if (masterDataBand.Parent.Components[index] is StiEmptyBand)
                    {
                    }
                    else if (masterDataBand.Parent.Components[index] is StiFooterBand)
                    {
                        comps.Add(masterDataBand.Parent.Components[index]);
                    }
                    else
                        break;
                }

                index++;
            }

            return comps;
        }

        /// <summary>
        /// Returns a collection of SubReport components.
        /// </summary>
        /// <returns>Components collection.</returns>
        public virtual StiComponentsCollection GetSubReports(StiDataBand masterDataBand)
        {
            StiComponentsCollection comps = new StiComponentsCollection();

            foreach (StiComponent component in masterDataBand.Components)
            {
                if (component is StiSubReport) comps.Add(component);
            }

            return comps;
        }
        #endregion

        #region Methods.Helpers.SubReports
        internal bool GetSubReportRenderResult(StiDataBand masterDataBand)
        {
            foreach (StiComponent comp in masterDataBand.DataBandInfoV1.SubReportsComponents)
            {
                if (!((StiSubReport) comp).SubReportInfoV1.SubReportRenderResult) return false;
            }

            return true;
        }
        #endregion

        #region Methods.Helpers.Headers
        /// <summary>
        /// Removes rendered headers.
        /// </summary>
        public void RemoveRenderedHeaders(StiDataBand masterDataBand, StiContainer outContainer)
        {
            #region Remove all rendered headers.
            foreach (StiComponent hd in masterDataBand.DataBandInfoV1.RenderedHeaders)
            {
                IncFreeSpace(masterDataBand, hd);
                outContainer.Components.Remove(hd);
            }
            #endregion

            foreach (StiHeaderBand header in masterDataBand.DataBandInfoV1.HeaderComponents)
            {
                header.Prepare();
            }
        }

        /// <summary>
        /// Render headers.
        /// </summary>
        public virtual bool RenderHeaders(StiDataBand masterDataBand, StiContainer outContainer)
        {
            masterDataBand.DataBandInfoV1.RenderedHeaders.Clear();
            if (masterDataBand.DataBandInfoV1.HeaderComponents != null)
            {
                bool resIsEof = masterDataBand.IsEof;

                try
                {
                    foreach (StiHeaderBand header in masterDataBand.DataBandInfoV1.HeaderComponents)
                    {
                        #region Conditions
                        StiBrush savedBrush = header.Brush;
                        bool savedEnabled = header.Enabled;
                        #endregion

                        try
                        {
                            header.InvokeBeforePrint(header, EventArgs.Empty);
                            if (header.IsEnabled)
                            {
                                #region Print if header is not rendered or if PrintOnAllPage and there is data or render header if no data
                                if (((!header.IsRendered) || header.PrintOnAllPages) && ((!masterDataBand.IsEmpty) || header.PrintIfEmpty))
                                {
                                    if (header.StartNewPage && (!header.HeaderBandInfoV1.StartNewPageProcessed))
                                    {
                                        double factor = 100 * masterDataBand.DataBandInfoV1.FreeSpace / masterDataBand.Page.Height;
                                        if (header.StartNewPageIfLessThan > factor || header.StartNewPageIfLessThan == 100)
                                        {
                                            header.HeaderBandInfoV1.StartNewPageProcessed = true;
                                            return false;
                                        }
                                    }

                                    if ((!header.IsRendered) && header.ResetPageNumber) masterDataBand.Report.EngineV1.ResetPageNumber();

                                    StiComponent outHeader = null;
                                    header.ParentBookmark = masterDataBand.CurrentBookmark;
                                    header.Render(ref outHeader, outContainer);

                                    if (outHeader != null)
                                    {
                                        //if (!header.PrintOnAllPages) masterDataBand.DataBandInfoV1.RenderedHeaders.Add(outHeader);
                                        masterDataBand.DataBandInfoV1.RenderedHeaders.Add(outHeader);

                                        if (header.PrintAtBottom)
                                        {
                                            AddBottomRenderedHeaders(masterDataBand, outHeader, header);
                                        }

                                        DecFreeSpace(masterDataBand, outHeader);

                                        double freeSpaceTemp = masterDataBand.DataBandInfoV1.FreeSpace;
                                        StiHeaderBandV1Builder.SecondPassBreak(header, outHeader, header, ref freeSpaceTemp);
                                        masterDataBand.DataBandInfoV1.FreeSpace = freeSpaceTemp;

                                        #region If was beyond the scope of
                                        if (CheckFreeSpace(masterDataBand, outContainer))
                                        {
                                            if (header.HeaderBandInfoV1.ForceCanBreak)
                                            {
                                                double freeSpaceTemp2 = masterDataBand.DataBandInfoV1.FreeSpace;
                                                StiHeaderBandV1Builder.FirstPassBreak(header, outHeader, header, ref freeSpaceTemp2);
                                                masterDataBand.DataBandInfoV1.FreeSpace = freeSpaceTemp;

                                                //Prepare header for rendering once again
                                                header.Prepare();
                                            }
                                            else
                                            {
                                                #region	If necessary to keep all headers together with data
                                                if (masterDataBand.DataBandInfoV1.FirstRow && IsKeepHeaderTogether(masterDataBand) && (!masterDataBand.DataBandInfoV1.LatestDataBandBreaked))
                                                {
                                                    RemoveRenderedHeaders(masterDataBand, outContainer);
                                                }
                                                #endregion

                                                else
                                                {
                                                    //Prepare header for rendering once again
                                                    header.Prepare();
                                                }

                                                if (outContainer.Components.IndexOf(outHeader) != -1)
                                                    outContainer.Components.Remove(outHeader);
                                            }

                                            return false;
                                        }
                                        #endregion
                                    }
                                }
                                #endregion
                            }

                            header.InvokeAfterPrint(header, EventArgs.Empty);
                        }
                        finally
                        {
                            #region Restore Conditions
                            header.Brush = savedBrush;
                            header.Enabled = savedEnabled;
                            #endregion
                        }
                    }
                }
                finally
                {
                    if (!(masterDataBand is StiCrossDataBand)) masterDataBand.IsEof = resIsEof;
                }
            }

            return true;
        }

        internal bool IsKeepHeaderTogether(StiDataBand masterDataBand)
        {
            return masterDataBand.KeepHeaderTogether && (!masterDataBand.DataBandInfoV1.CrossTabExistOnDataBand);
        }
        #endregion

        #region Methods.Helpers.Footers
        /// <summary>
        /// Prints footers with the specified option [PrintOnAllPages]
        /// (early pass) for to take them their own place.
        /// </summary>
        /// <param name="footers">A collection of rendered footers.</param>
        public bool RenderFootersOnAllPagesFirst(
            StiDataBand masterDataBand,
            ref StiComponentsCollection footers,
            ref StiComponentsCollection parentFooters,
            StiContainer outContainer)
        {
            if (masterDataBand.DataBandInfoV1.FooterComponents != null)
            {
                bool resIsEof = masterDataBand.IsEof;

                try
                {
                    foreach (StiFooterBand footer in masterDataBand.DataBandInfoV1.FooterComponents)
                    {
                        #region Conditions
                        StiBrush savedBrush = footer.Brush;
                        bool savedEnabled = footer.Enabled;
                        #endregion

                        try
                        {
                            if ((footer.PrintOnAllPages) && (!masterDataBand.IsEmpty || footer.PrintIfEmpty))
                            {
                                footer.InvokeBeforePrint(footer, EventArgs.Empty);
                                if (footer.IsEnabled)
                                {
                                    StiComponent outFooter = null;
                                    footer.ParentBookmark = masterDataBand.CurrentBookmark;
                                    footer.Render(ref outFooter, outContainer);

                                    if (outFooter != null)
                                    {
                                        DecFreeSpace(masterDataBand, outFooter);
                                        footers.Add(outFooter);
                                        parentFooters.Add(footer);

                                        if (CheckFreeSpace(masterDataBand, outContainer))
                                        {
                                            IncFreeSpace(masterDataBand, outFooter);
                                            footers.Remove(outFooter);
                                            outContainer.Components.Remove(outFooter);
                                            return false;
                                        }
                                    }
                                }

                                footer.InvokeAfterPrint(footer, EventArgs.Empty);
                            }
                        }
                        finally
                        {
                            #region Restore Conditions
                            footer.Brush = savedBrush;
                            footer.Enabled = savedEnabled;
                            #endregion
                        }
                    }
                }
                finally
                {
                    if (!(masterDataBand is StiCrossDataBand)) masterDataBand.IsEof = resIsEof;
                }
            }

            return true;
        }

        /// <summary>
        /// Print footers with the specified option [PrintOnAllPages] (second pass) to take them their own place.
        /// </summary>
        /// <param name="footers">Collection of rendered footers.</param>
        public void RenderFootersOnAllPagesSecond(
            StiDataBand masterDataBand,
            ref StiComponentsCollection footers,
            ref StiComponentsCollection parentFooters,
            StiContainer outContainer)
        {
            if (masterDataBand.DataBandInfoV1.FooterComponents != null)
            {
                bool resIsEof = masterDataBand.IsEof;

                try
                {
                    #region Remove all earlier rendered footers from container
                    foreach (StiComponent footer in footers)
                    {
                        outContainer.Components.Remove(footer);
                    }
                    #endregion

                    #region Render again
                    int index = 0;
                    foreach (StiContainer footer in footers)
                    {
                        #region Conditions
                        StiBrush savedBrush = footer.Brush;
                        bool savedEnabled = footer.Enabled;
                        #endregion

                        try
                        {
                            StiFooterBand masterFooter = null;

                            foreach (StiFooterBand footerBand in masterDataBand.DataBandInfoV1.FooterComponents)
                            {
                                if (footerBand.Name == footer.Name)
                                {
                                    masterFooter = footerBand;
                                    break;
                                }
                            }

                            StiComponent outFooter = null;
                            footer.ParentBookmark = masterDataBand.DataBandInfoV1.ResParentBookmark;

                            #region If footer is ColumnFooter do not rerender component
                            if (masterFooter is StiColumnFooterBand)
                            {
                                outFooter = footer;
                                outContainer.Components.Add(outFooter);
                            }
                            #endregion

                            #region In other way render footer again
                            else
                            {
                                footer.InternalRender(ref outFooter, outContainer);
                            }
                            #endregion

                            #region Support PrintAtBottom property
                            StiFooterBand footerParent = parentFooters[index++] as StiFooterBand;

                            if (footerParent.PrintAtBottom)
                            {
                                AddBottomRenderedFooters(masterDataBand, outFooter, footerParent);
                            }
                            #endregion
                        }
                        finally
                        {
                            #region Restore Conditions
                            footer.Brush = savedBrush;
                            footer.Enabled = savedEnabled;
                            #endregion
                        }
                    }
                    #endregion
                }
                finally
                {
                    if (!(masterDataBand is StiCrossDataBand)) masterDataBand.IsEof = resIsEof;
                }
            }
        }

        private void AddAllFootersToRemittedCollection(StiDataBand masterDataBand, int index, ref StiComponent outFooter, ref StiContainer outContainer)
        {
            for (int pos = index; pos < masterDataBand.DataBandInfoV1.FooterComponents.Count; pos++)
            {
                StiFooterBand footer = masterDataBand.DataBandInfoV1.FooterComponents[pos] as StiFooterBand;
                if ((!footer.PrintOnAllPages) && masterDataBand.IsEof && footer.IsEnabled)
                {
                    outFooter = null;
                    footer.Render(ref outFooter, outContainer);

                    //Remove last render footer
                    outContainer.Components.Remove(outFooter);

                    //Shall render it in next time
                    masterDataBand.DataBandInfoV1.RemmitedCollection.Add(outFooter);
                }
            }
        }

        /// <summary>
        /// Print footers with the specified option [PrintOnAllPages](second pass) for to take them their own place.
        /// </summary>
        public bool RenderFootersSecond(StiDataBand masterDataBand, out ArrayList renderedFooters, StiContainer outContainer, ref bool newPageStarted, ref bool isBreaked)
        {
            renderedFooters = new ArrayList();
            bool renderResult = true;
            if (masterDataBand.DataBandInfoV1.FooterComponents != null)
            {
                bool resIsEof = masterDataBand.IsEof;

                try
                {
                    for (int index = 0; index < masterDataBand.DataBandInfoV1.FooterComponents.Count; index++)
                    {
                        StiFooterBand footer = masterDataBand.DataBandInfoV1.FooterComponents[index] as StiFooterBand;

                        #region Conditions
                        StiBrush savedBrush = footer.Brush;
                        bool savedEnabled = footer.Enabled;
                        #endregion

                        try
                        {
                            if ((!footer.PrintOnAllPages) && masterDataBand.IsEof && (!masterDataBand.IsEmpty || footer.PrintIfEmpty))
                            {
                                footer.InvokeBeforePrint(footer, EventArgs.Empty);
                                if (footer.IsEnabled)
                                {
                                    StiComponent outFooter = null;
                                    footer.ParentBookmark = masterDataBand.DataBandInfoV1.ResParentBookmark;
                                    footer.Render(ref outFooter, outContainer);

                                    #region Support PrintAtBottom property
                                    if (footer.PrintAtBottom)
                                    {
                                        AddBottomRenderedFooters(masterDataBand, outFooter, footer);
                                    }
                                    #endregion

                                    renderedFooters.Add(outFooter);
                                    DecFreeSpace(masterDataBand, outFooter);

                                    double freeSpaceTemp = masterDataBand.DataBandInfoV1.FreeSpace;
                                    SecondPassBreak(masterDataBand, outFooter, footer, ref freeSpaceTemp);
                                    masterDataBand.DataBandInfoV1.FreeSpace = freeSpaceTemp;


                                    #region If footer component was beyond the scope of
                                    if (CheckFreeSpace(masterDataBand, outContainer))
                                    {
                                        if (footer.FooterBandInfoV1.ForceCanBreak)
                                        {
                                            double height = outFooter.Height;

                                            double freeSpaceTemp2 = masterDataBand.DataBandInfoV1.FreeSpace;
                                            StiFooterBandV1Builder.FirstPassBreak(footer, outFooter, footer, ref freeSpaceTemp2);
                                            masterDataBand.DataBandInfoV1.FreeSpace = freeSpaceTemp2;
                                            StiContainer newCont = outFooter.Clone() as StiContainer;
                                            newCont.Components.Clear();
                                            newCont.Components.AddRange(footer.FooterBandInfoV1.BreakableComps);

                                            renderedFooters.Remove(outFooter);
                                            outFooter = newCont;

                                            masterDataBand.DataBandInfoV1.RemmitedCollection.Add(outFooter);
                                            outFooter.Height = height - outFooter.Height;
                                            index++;

                                            isBreaked = true;
                                        }
                                        else
                                        {
                                            //Remove last rendered footer
                                            outContainer.Components.Remove(outFooter);
                                            renderedFooters.Remove(outFooter);
                                        }

                                        if (!(masterDataBand is StiCrossDataBand)) masterDataBand.IsEof = resIsEof;
                                        AddAllFootersToRemittedCollection(masterDataBand, index, ref outFooter, ref outContainer);

                                        //Render not all
                                        renderResult = false;

                                        masterDataBand.IsRendered = false;
                                        return renderResult;
                                    }
                                    #endregion

                                    if (footer.StartNewPage)
                                    {
                                        double factor = 100 * masterDataBand.DataBandInfoV1.FreeSpace / masterDataBand.Page.Height;
                                        if (footer.StartNewPageIfLessThan > factor || footer.StartNewPageIfLessThan == 100)
                                        {
                                            //Changed in version 2007.1, fix problem with startnewpage on second page
                                            newPageStarted = true;

                                            outContainer.Components.Remove(outFooter);
                                            renderedFooters.Remove(outFooter);

                                            if (!(masterDataBand is StiCrossDataBand)) masterDataBand.IsEof = resIsEof;
                                            AddAllFootersToRemittedCollection(masterDataBand, index, ref outFooter, ref outContainer);

                                            return false;
                                        }
                                    }
                                }

                                footer.InvokeAfterPrint(footer, EventArgs.Empty);
                            }
                        }
                        finally
                        {
                            #region Restore Conditions
                            footer.Brush = savedBrush;
                            footer.Enabled = savedEnabled;
                            #endregion
                        }
                    }
                }
                finally
                {
                    if (!(masterDataBand is StiCrossDataBand)) masterDataBand.IsEof = resIsEof;
                }
            }

            return renderResult;
        }
        #endregion

        #region Methods.Helpers.Groups
        internal void RemoveFromContainerAt(StiDataBand masterDataBand, StiContainer outContainer, int startIndex)
        {
            int count = outContainer.Components.Count;
            for (int index = startIndex; index < count; index++)
            {
                int ind = outContainer.Components.Count - 1;
                IncFreeSpace(masterDataBand, outContainer.Components[ind]);
                outContainer.Components.RemoveAt(ind);
            }
        }

        internal void RemoveFromContainerAt(StiDataBand masterDataBand, StiContainer outContainer, int startIndex, int endIndex)
        {
            int count = endIndex - startIndex;
            while (count > 0)
            {
                IncFreeSpace(masterDataBand, outContainer.Components[startIndex]);
                outContainer.Components.RemoveAt(startIndex);
                count--;
            }
        }

        /// <summary>
        /// Gets a group comparison result for the Group Header.
        /// </summary>
        internal bool GetGroupHeaderResult(StiDataBand masterDataBand, StiGroupHeaderBand band)
        {
            #region Begin of data
            if (masterDataBand.IsBof) return true;
            #endregion

            #region If prev value and next value equals then render Group Header
            else
            {
                if (masterDataBand.Position > 0)
                {
                    masterDataBand.Position--;
                    object nextValue = StiGroupHeaderBandV1Builder.GetCurrentConditionValue(band);
                    masterDataBand.Position++;
                    object currentValue = StiGroupHeaderBandV1Builder.GetCurrentConditionValue(band);
                    bool result = !object.Equals(nextValue, currentValue);
                    if (result) return true;
                }
            }
            #endregion

            #region Check Parent group for begin
            int groupIndex = masterDataBand.DataBandInfoV1.GroupHeaderComponents.IndexOf(band);
            for (int index = groupIndex - 1; index >= 0; index--)
            {
                StiGroupHeaderBand parentBand = masterDataBand.DataBandInfoV1.GroupHeaderComponents[index] as StiGroupHeaderBand;
                if (parentBand.IsEnabled)
                {
                    if (GetGroupHeaderResult(masterDataBand, parentBand)) return true;
                }
            }
            #endregion

            return false;
        }

        /// <summary>
        /// Render group headers.
        /// </summary>
        public virtual bool RenderGroupHeaders(StiDataBand masterDataBand, StiContainer outContainer, ref bool startNewPage)
        {
            int startIndex = outContainer.Components.Count;
            int startGrpIndex = -1;

            masterDataBand.ParentBookmark = masterDataBand.DataBandInfoV1.ResParentBookmark;

            foreach (StiGroupHeaderBand band in masterDataBand.DataBandInfoV1.GroupHeaderComponents)
            {
                if (IsPrintIfDetailEmpty(masterDataBand))
                {
                    #region Bookmarks
                    if (band.IsEnabled)
                    {
                        band.ParentBookmark = masterDataBand.ParentBookmark;
                        bool isNewGuidCreated = band.DoBookmark();
                        
                        if (band.ParentBookmark != band.CurrentBookmark) 
                            masterDataBand.ParentBookmark = band.CurrentBookmark;

                        band.ParentPointer = masterDataBand.ParentPointer;
                        band.DoPointer(!isNewGuidCreated);

                        if (band.ParentPointer != band.CurrentPointer)
                            masterDataBand.ParentPointer = band.CurrentPointer;
                    }
                    #endregion

                    bool render = GetGroupHeaderResult(masterDataBand, band);
                    if (band.GroupHeaderBandInfoV1.LastPositionLineRendering == masterDataBand.Position && (!IsKeepHeaderTogether(masterDataBand))) render = false;
                    bool newGroup = render;

                    if (band.GroupHeaderBandInfoV1.Rerender)
                    {
                        band.IsRendered = true;
                        render = true;
                        band.GroupHeaderBandInfoV1.Rerender = false;
                    }

                    if (render)
                    {
                        if (masterDataBand.Report != null)
                        {
                            StiReport report = masterDataBand.Report;
                            if ((report.CacheTotals) && (report.CachedTotals != null))
                            {
                                report.CachedTotals[band] = null;
                            }
                        }

                        #region StartNewPage
                        if (band.StartNewPage && (masterDataBand.DataBandInfoV1.FirstRowInPath == false || masterDataBand.DataBandInfoV1.ForceStartNewPage))
                        {
                            double factor = 100 * masterDataBand.DataBandInfoV1.FreeSpace / masterDataBand.Page.Height;
                            if (band.StartNewPageIfLessThan > factor || band.StartNewPageIfLessThan == 100)
                            {
                                startNewPage = true;
                                if (startGrpIndex != -1) masterDataBand.DataBandInfoV1.StartGroupIndex = startGrpIndex;
                                return false;
                            }
                        }
                        #endregion

                        if (newGroup)
                        {
                            band.InvokeBeginRender(masterDataBand, masterDataBand.Position);

                            #region Set variables report
                            masterDataBand.DataBandInfoV1.StartLine = masterDataBand.DataBandInfoV1.RuntimeLine;
                            #endregion
                        }

                        if (startGrpIndex == -1)
                        {
                            masterDataBand.Report.SaveState(masterDataBand.Name + "Group");
                            startGrpIndex = outContainer.Components.Count;
                        }

                        StiComponent outHeader = null;

                        #region Conditions
                        StiBrush savedBrush = band.Brush;
                        bool savedEnabled = band.Enabled;
                        #endregion

                        try
                        {
                            band.InvokeBeforePrint(band, EventArgs.Empty);
                            if (band.IsEnabled)
                            {
                                band.Render(ref outHeader, outContainer);

                                bool groupRenderedFine = true;

                                if (outHeader != null)
                                {
                                    #region Decreasing header
                                    DecFreeSpace(masterDataBand, outHeader);
                                    #endregion

                                    double freeSpaceTemp = masterDataBand.DataBandInfoV1.FreeSpace;
                                    StiGroupHeaderBandV1Builder.SecondPassBreak(band, outHeader, band, ref freeSpaceTemp);
                                    masterDataBand.DataBandInfoV1.FreeSpace = freeSpaceTemp;

                                    #region If was beyond the scope of
                                    if (CheckFreeSpace(masterDataBand, outContainer))
                                    {
                                        if (band.GroupHeaderBandInfoV1.ForceCanBreak)
                                        {
                                            double freeSpaceTemp2 = masterDataBand.DataBandInfoV1.FreeSpace;
                                            StiGroupHeaderBandV1Builder.FirstPassBreak(band, outHeader, band, ref freeSpaceTemp2);
                                            masterDataBand.DataBandInfoV1.FreeSpace = freeSpaceTemp;

                                            //Prepare header for rendering once again
                                            band.Prepare();
                                        }
                                        else
                                        {
                                            groupRenderedFine = false;

                                            #region If necessary to keep all headers together with data
                                            if (IsKeepHeaderTogether(masterDataBand) && (!masterDataBand.DataBandInfoV1.LatestDataBandBreaked))
                                            {
                                                RemoveFromContainerAt(masterDataBand, outContainer, startIndex);
                                                masterDataBand.Report.RestoreState(masterDataBand.Name + "Group");
                                                foreach (StiGroupHeaderBand band2 in masterDataBand.DataBandInfoV1.GroupHeaderComponents)
                                                {
                                                    band2.Prepare();
                                                }
                                            }
                                            else
                                            {
                                                band.GroupHeaderBandInfoV1.Rerender = true;

                                                IncFreeSpace(masterDataBand, outHeader);

                                                outContainer.Components.Remove(outHeader);

                                                #region Prepare for rendering once again
                                                band.Prepare();
                                                #endregion
                                            }

                                            if (startGrpIndex != -1) masterDataBand.DataBandInfoV1.StartGroupIndex = startGrpIndex;
                                            #endregion
                                        }

                                        return false;
                                    }
                                    #endregion
                                }

                                #region Dock last PrintAtBottom GroupFooter to top
                                if (groupRenderedFine)
                                {
                                    if (band.GroupHeaderBandInfoV1.GroupFooter != null && band.GroupHeaderBandInfoV1.GroupFooter.GroupFooterBandInfoV1.PrintAtBottomComponent != null)
                                    {
                                        RemoveBottomRenderedGroupFooters(masterDataBand, band.GroupHeaderBandInfoV1.GroupFooter.GroupFooterBandInfoV1.PrintAtBottomComponent,
                                            band.GroupHeaderBandInfoV1.GroupFooter);
                                    }
                                }
                                #endregion
                            }

                            band.InvokeAfterPrint(band, EventArgs.Empty);
                        }
                        finally
                        {
                            #region Restore Conditions
                            band.Brush = savedBrush;
                            band.Enabled = savedEnabled;
                            #endregion
                        }
                    }
                }
            }

            if (startGrpIndex != -1) masterDataBand.DataBandInfoV1.StartGroupIndex = startGrpIndex;
            return true;
        }


        /// <summary>
        /// Gets group comparison result for a Group Footer.
        /// </summary>
        internal bool GetGroupFooterResult(StiDataBand masterDataBand, StiGroupFooterBand band)
        {
            return GetGroupFooterResult(masterDataBand, band.GroupFooterBandInfoV1.GroupHeader);
        }


        /// <summary>
        /// Gets group comparison result for a Group Header.
        /// </summary>
        internal bool GetGroupFooterResult(StiDataBand masterDataBand, StiGroupHeaderBand band)
        {
            #region If isEof render GroupFooter
            if (masterDataBand.Position >= (masterDataBand.Count - 1)) return true;
            #endregion

            #region If next value and current value not equals render GroupFooter
            masterDataBand.Position++;
            object nextValue = StiGroupHeaderBandV1Builder.GetCurrentConditionValue(band);
            masterDataBand.Position--;
            object currentValue = StiGroupHeaderBandV1Builder.GetCurrentConditionValue(band);
            bool result = !object.Equals(nextValue, currentValue);
            if (result) return true;
            #endregion

            #region Check Parent group for end
            int groupIndex = masterDataBand.DataBandInfoV1.GroupHeaderComponents.IndexOf(band);
            for (int index = groupIndex - 1; index >= 0; index--)
            {
                StiGroupHeaderBand parentBand = masterDataBand.DataBandInfoV1.GroupHeaderComponents[index] as StiGroupHeaderBand;
                result = GetGroupFooterResult(masterDataBand, parentBand);

                if (result) return true;
            }
            #endregion

            return false;
        }


        internal void RenderOneGroupFooter(StiDataBand masterDataBand, StiGroupFooterBand groupFooter,
            ref StiComponent outFooter, StiContainer outContainer)
        {
            #region Conditions
            StiBrush savedBrush = groupFooter.Brush;
            bool savedEnabled = groupFooter.Enabled;
            #endregion

            try
            {
                groupFooter.InvokeBeforePrint(groupFooter, EventArgs.Empty);
                if (groupFooter.IsEnabled)
                {
                    #region Bookmarks
                    groupFooter.ParentBookmark = masterDataBand.ParentBookmark;
                    bool isNewGuidCreated = groupFooter.DoBookmark();

                    groupFooter.ParentPointer = masterDataBand.ParentPointer;
                    groupFooter.DoPointer(!isNewGuidCreated);
                    #endregion

                    groupFooter.Render(ref outFooter, outContainer);
                }

                groupFooter.InvokeAfterPrint(groupFooter, EventArgs.Empty);
            }
            finally
            {
                #region Restore Conditions
                groupFooter.Brush = savedBrush;
                groupFooter.Enabled = savedEnabled;
                #endregion
            }
        }


        internal bool IsGroupFooterResult(StiDataBand masterDataBand)
        {
            for (int index = masterDataBand.DataBandInfoV1.GroupHeaderComponents.Count - 1; index >= 0; index--)
            {
                StiGroupHeaderBand groupHeader = masterDataBand.DataBandInfoV1.GroupHeaderComponents[index] as StiGroupHeaderBand;
                if (groupHeader.IsEnabled && IsPrintIfDetailEmpty(masterDataBand))
                {
                    int pos = masterDataBand.Position;
                    if (masterDataBand.Position > 0) masterDataBand.Position--;
                    bool render = GetGroupFooterResult(masterDataBand, groupHeader);
                    masterDataBand.Position = pos;
                    if (render) return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Render group footers.
        /// </summary>
        public virtual bool RenderGroupFooters(StiDataBand masterDataBand, ref int footerIndex, StiContainer outContainer,
            out StiComponentsCollection renderedGroupFooters)
        {
            renderedGroupFooters = new StiComponentsCollection();

            bool renderResult = true;
            for (int index = masterDataBand.DataBandInfoV1.GroupHeaderComponents.Count - 1; index >= 0; index--)
            {
                StiGroupHeaderBand groupHeader = masterDataBand.DataBandInfoV1.GroupHeaderComponents[index] as StiGroupHeaderBand;
                if (groupHeader.IsEnabled && IsPrintIfDetailEmpty(masterDataBand))
                {
                    bool render = GetGroupFooterResult(masterDataBand, groupHeader);
                    if (render)
                    {
                        groupHeader.InvokeEndRender();

                        StiGroupFooterBand groupFooter = groupHeader.GroupHeaderBandInfoV1.GroupFooter;
                        StiComponent outFooter = null;

                        if (groupFooter != null)
                        {
                            groupFooter.ParentBookmark = groupHeader.CurrentBookmark;

                            RenderOneGroupFooter(masterDataBand, groupFooter, ref outFooter, outContainer);

                            if (outFooter != null) DecFreeSpace(masterDataBand, outFooter);
                        }

                        if (outFooter != null)
                        {
                            if (CheckFreeSpace(masterDataBand, outContainer) || (!renderResult))
                            {
                                masterDataBand.DataBandInfoV1.RemmitedCollection.Add(outFooter);
                                outContainer.Components.Remove(outFooter);
                                renderResult = false;
                            }
                            else renderedGroupFooters.Add(groupHeader);
                        }
                    }
                }
            }

            return renderResult;
        }


        /// <summary>
        /// Returns count of rows in current groups.
        /// </summary>
        public int GetGroupLinesCount(StiDataBand masterDataBand, StiGroupHeaderBand band)
        {
            int pos = masterDataBand.Position;
            int prevCount = 0;
            int nextCount = 0;

            while (1 == 1)
            {
                if (masterDataBand.Position > 0)
                {
                    object nextValue = StiGroupHeaderBandV1Builder.GetCurrentConditionValue(band);
                    masterDataBand.Position--;
                    object currentValue = StiGroupHeaderBandV1Builder.GetCurrentConditionValue(band);
                    bool result = !object.Equals(nextValue, currentValue);
                    if (result) break;
                }
                else break;

                prevCount++;
            }

            masterDataBand.Position = pos;

            while (1 == 1)
            {
                if (masterDataBand.Position < (masterDataBand.Count - 1))
                {
                    object nextValue = StiGroupHeaderBandV1Builder.GetCurrentConditionValue(band);
                    masterDataBand.Position++;
                    object currentValue = StiGroupHeaderBandV1Builder.GetCurrentConditionValue(band);

                    bool result = !object.Equals(nextValue, currentValue);
                    if (result) break;
                }
                else break;

                nextCount++;
            }

            masterDataBand.Position = pos;

            return prevCount + nextCount + 1;
        }


        /// <summary>
        /// All GroupFooters are placed on their GroupHeader(is set property - Header).
        /// </summary>
        public virtual void GroupsComparison(StiDataBand masterDataBand)
        {
            foreach (StiGroupHeaderBand band in masterDataBand.DataBandInfoV1.GroupHeaderComponents) band.GroupHeaderBandInfoV1.GroupFooter = null;

            for (int footerIndex = 0; footerIndex < masterDataBand.DataBandInfoV1.GroupFooterComponents.Count; footerIndex++)
            {
                int headerIndex = masterDataBand.DataBandInfoV1.GroupHeaderComponents.Count - footerIndex - 1;
                if (headerIndex >= 0)
                {
                    ((StiGroupFooterBand) masterDataBand.DataBandInfoV1.GroupFooterComponents[footerIndex]).GroupFooterBandInfoV1.GroupHeader =
                        (StiGroupHeaderBand) masterDataBand.DataBandInfoV1.GroupHeaderComponents[headerIndex];

                    ((StiGroupHeaderBand) masterDataBand.DataBandInfoV1.GroupHeaderComponents[headerIndex]).GroupHeaderBandInfoV1.GroupFooter =
                        (StiGroupFooterBand) masterDataBand.DataBandInfoV1.GroupFooterComponents[footerIndex];
                }
                else ((StiGroupFooterBand) masterDataBand.DataBandInfoV1.GroupFooterComponents[footerIndex]).GroupFooterBandInfoV1.GroupHeader = null;
            }
        }
        #endregion

        #region Methods.Helpers.FreeSpace
        public virtual void IncFreeSpace(StiDataBand masterDataBand, StiComponent component)
        {
            masterDataBand.DataBandInfoV1.FreeSpace += component.Height;
        }


        public virtual void DecFreeSpace(StiDataBand masterDataBand, StiComponent component)
        {
            masterDataBand.DataBandInfoV1.FreeSpace -= component.Height;
        }


        public virtual double GetFreeSpaceFromRectangle(RectangleD rect)
        {
            return rect.Height;
        }


        public virtual double GetFreeSpaceFromComponent(StiComponent component)
        {
            return component.Height;
        }
        #endregion

        #region Methods.Helpers.Details
        /// <summary>
        /// Sets detail.
        /// </summary>
        public static void SetDetails(StiDataBand masterDataBand)
        {
            if (!masterDataBand.IsDataSourceEmpty)
            {
                #region Set Details
                if (masterDataBand.DataBandInfoV1.DetailDataComponents != null)
                {
                    foreach (StiComponent comp in masterDataBand.DataBandInfoV1.DetailDataComponents)
                    {
                        StiDataHelper.SetData(comp, false);

                        StiDataBand dataBand = comp as StiDataBand;
                        if (dataBand != null)
                        {
                            StiDataBandV1Builder builder = StiDataBandV1Builder.GetBuilder(typeof(StiDataBand)) as StiDataBandV1Builder;

                            StiComponentsCollection comps = builder.GetGroupHeaders(dataBand);
                            foreach (StiGroupHeaderBand band in comps)
                            {
                                band.GroupHeaderBandInfoV1.LastPositionLineRendering = -1;
                                band.GroupHeaderBandInfoV1.LastPositionRendering = -1;
                            }
                        }
                    }
                }
                #endregion

                #region Set Cross
                foreach (StiComponent comp in masterDataBand.Components)
                {
                    if (comp.IsCross)
                    {
                        if (comp is StiCrossDataBand && ((StiCrossDataBand) comp).ColumnMode) continue;
                        StiDataHelper.SetData(comp, false);
                    }
                }
                #endregion

                #region Set SubReport
                if (masterDataBand.DataBandInfoV1.SubReportsComponents != null)
                {
                    foreach (StiSubReport subReport in masterDataBand.DataBandInfoV1.SubReportsComponents)
                    {
                        if (subReport.SubReportPage != null)
                        {
                            StiComponentsCollection comps = subReport.SubReportPage.GetComponents();
                            foreach (StiComponent comp in comps)
                            {
                                StiDataHelper.SetData(comp, false);
                            }
                        }
                    }
                }
                #endregion
            }
        }


        /// <summary>
        /// Gets value indicates that all detail components are empty.
        /// </summary>
        [Browsable(false)]
        public static bool IsDetailDataSourcesEmpty(StiDataBand masterDataBand)
        {
            if (masterDataBand.DataBandInfoV1.DetailComponents == null || masterDataBand.DataBandInfoV1.DetailComponents.Count == 0) return false;

            foreach (StiComponent comp in masterDataBand.DataBandInfoV1.DetailComponents)
            {
                if (comp is IStiDataSource)
                {
                    if (!((IStiDataSource) comp).IsEmpty) return false;
                }
            }

            return true;
        }


        public void SetData(StiDataBand masterDataBand)
        {
            if (!(masterDataBand is StiCrossDataBand)) StiDataHelper.SetData(masterDataBand, false);
            else if (!((StiCrossDataBand) masterDataBand).ColumnMode)
                StiDataHelper.SetData(masterDataBand, false);
            SetDataIsPrepared(masterDataBand, true);
        }


        public void ResetStartNewPageProcessed(StiDataBand masterDataBand)
        {
            foreach (StiDataBand dataBand in masterDataBand.DataBandInfoV1.DetailComponents)
            {
                foreach (StiHeaderBand header in dataBand.DataBandInfoV1.HeaderComponents)
                {
                    header.HeaderBandInfoV1.StartNewPageProcessed = false;
                }
            }
        }


        /// <summary>
        /// Comes on the first detail component. If there are no such components then sets the current detail component to null.
        /// </summary>
        public virtual void FirstDetail(StiDataBand masterDataBand)
        {
            masterDataBand.DataBandInfoV1.CurrentDetailComponent = null;

            int index = 0;
            while (index < masterDataBand.DataBandInfoV1.DetailComponents.Count)
            {
                masterDataBand.DataBandInfoV1.CurrentDetailComponent = masterDataBand.DataBandInfoV1.DetailComponents[index];

                StiDataBand dataBand = masterDataBand.DataBandInfoV1.CurrentDetailComponent as StiDataBand;
                if (dataBand != null && dataBand.Columns > 0 && (!dataBand.IsEnabled))
                {
                    masterDataBand.DataBandInfoV1.CurrentDetailComponent = null;
                    index++;
                }
                else return;
            }
        }


        /// <summary>
        /// Comes on the first detail component. If there are no such components then sets the current subordinate component to null.
        /// </summary>
        public virtual void NextDetail(StiDataBand masterDataBand)
        {
            int index = masterDataBand.DataBandInfoV1.DetailComponents.IndexOf(masterDataBand.DataBandInfoV1.CurrentDetailComponent) + 1;
            if (index == masterDataBand.DataBandInfoV1.DetailComponents.Count) masterDataBand.DataBandInfoV1.CurrentDetailComponent = null;
            else masterDataBand.DataBandInfoV1.CurrentDetailComponent = masterDataBand.DataBandInfoV1.DetailComponents[index];
        }


        /// <summary>
        /// Render detail components.
        /// </summary>
        public virtual void RenderDetails(StiDataBand masterDataBand, StiContainer outContainer)
        {
            #region If no current detail component
            if (masterDataBand.DataBandInfoV1.CurrentDetailComponent == null)
            {
                #region Get first component
                FirstDetail(masterDataBand);
                if (masterDataBand.DataBandInfoV1.CurrentDetailComponent != null) masterDataBand.DataBandInfoV1.CurrentDetailComponent.Prepare();
                #endregion
            }
            #endregion

            bool isRendered = true;

            if (masterDataBand.DataBandInfoV1.CurrentDetailComponent != null)
            {
                while (1 == 1)
                {
                    #region Render current detail component
                    masterDataBand.DataBandInfoV1.CurrentDetailComponent.ParentBookmark = masterDataBand.CurrentBookmark;
                    StiComponent currentDetail = masterDataBand.DataBandInfoV1.CurrentDetailComponent;
                    isRendered = masterDataBand.DataBandInfoV1.CurrentDetailComponent.Render(outContainer);
                    masterDataBand.DataBandInfoV1.CurrentDetailComponent = currentDetail;
                    #endregion

                    #region if not rendered that breaks
                    if (!isRendered) break;
                    #endregion

                    #region Get next component
                    NextDetail(masterDataBand);
                    #endregion

                    if (masterDataBand.DataBandInfoV1.CurrentDetailComponent == null) break;
                    else masterDataBand.DataBandInfoV1.CurrentDetailComponent.Prepare();
                }
            }
        }


        public bool IsPrintIfDetailEmpty(StiDataBand masterDataBand)
        {
            return masterDataBand.PrintIfDetailEmpty || (!IsDetailDataSourcesEmpty(masterDataBand));
        }
        #endregion

        #region Methods.Helpers.PrintAtBottom
        internal void AddBottomRenderedHeaders(StiDataBand masterDataBand, StiComponent header, StiHeaderBand parentHeader)
        {
            if (parentHeader.PrintAtBottom)
            {
                StiDataBand band = masterDataBand;

                while (band.MasterComponent != null && band.MasterComponent is StiDataBand)
                {
                    band = band.MasterComponent as StiDataBand;
                }

                StiContainer parent = band.Parent;

                if (parent.ContainerInfoV1.BottomRenderedHeaders == null)
                {
                    parent.ContainerInfoV1.BottomRenderedHeaders = new Hashtable();
                    parent.ContainerInfoV1.BottomRenderedParentsHeaders = new StiComponentsCollection();
                }

                StiComponentsCollection comps = parent.ContainerInfoV1.BottomRenderedHeaders[parentHeader] as StiComponentsCollection;
                if (comps == null)
                {
                    comps = new StiComponentsCollection();
                    parent.ContainerInfoV1.BottomRenderedHeaders[parentHeader] = comps;
                }

                comps.Add(header);

                if (parent.ContainerInfoV1.BottomRenderedParentsHeaders.IndexOf(parentHeader) == -1)
                {
                    parent.ContainerInfoV1.BottomRenderedParentsHeaders.Insert(0, parentHeader);
                }
            }
        }


        internal void AddBottomRenderedFooters(StiDataBand masterDataBand, StiComponent footer, StiFooterBand parentFooter)
        {
            if (parentFooter.PrintAtBottom)
            {
                StiDataBand band = masterDataBand;

                while (band.MasterComponent != null && band.MasterComponent is StiDataBand)
                {
                    band = band.MasterComponent as StiDataBand;
                }

                StiContainer parent = band.Parent;

                if (parent.ContainerInfoV1.BottomRenderedFooters == null)
                {
                    parent.ContainerInfoV1.BottomRenderedFooters = new Hashtable();
                    parent.ContainerInfoV1.BottomRenderedParentsFooters = new StiComponentsCollection();
                }

                StiComponentsCollection comps = parent.ContainerInfoV1.BottomRenderedFooters[parentFooter] as StiComponentsCollection;
                if (comps == null)
                {
                    comps = new StiComponentsCollection();
                    parent.ContainerInfoV1.BottomRenderedFooters[parentFooter] = comps;
                }

                comps.Add(footer);

                if (parent.ContainerInfoV1.BottomRenderedParentsFooters.IndexOf(parentFooter) == -1)
                {
                    parent.ContainerInfoV1.BottomRenderedParentsFooters.Insert(0, parentFooter);
                }
            }
        }


        internal void AddBottomRenderedDataBands(StiDataBand masterDataBand, StiComponent band, StiDataBand parentDataBand)
        {
            if (parentDataBand.PrintAtBottom)
            {
                StiContainer parent = parentDataBand.Parent;

                if (parent.ContainerInfoV1.BottomRenderedDataBands == null)
                {
                    parent.ContainerInfoV1.BottomRenderedDataBands = new Hashtable();
                    parent.ContainerInfoV1.BottomRenderedParentsDataBands = new StiComponentsCollection();
                }

                StiComponentsCollection comps = parent.ContainerInfoV1.BottomRenderedDataBands[parentDataBand] as StiComponentsCollection;
                if (comps == null)
                {
                    comps = new StiComponentsCollection();
                    parent.ContainerInfoV1.BottomRenderedDataBands[parentDataBand] = comps;
                }

                comps.Add(band);

                if (parent.ContainerInfoV1.BottomRenderedParentsDataBands.IndexOf(parentDataBand) == -1)
                {
                    parent.ContainerInfoV1.BottomRenderedParentsDataBands.Insert(0, parentDataBand);
                }
            }
        }


        internal void AddBottomRenderedGroupFooters(StiDataBand masterDataBand, StiComponent footer, StiGroupFooterBand parentFooter)
        {
            if (parentFooter.PrintAtBottom)
            {
                StiContainer parent = parentFooter.Parent;

                if (parent.ContainerInfoV1.BottomRenderedGroupFooters == null)
                {
                    parent.ContainerInfoV1.BottomRenderedGroupFooters = new Hashtable();
                    parent.ContainerInfoV1.BottomRenderedParentsGroupFooters = new StiComponentsCollection();
                }

                StiComponentsCollection comps = parent.ContainerInfoV1.BottomRenderedGroupFooters[parentFooter] as StiComponentsCollection;
                if (comps == null)
                {
                    comps = new StiComponentsCollection();
                    parent.ContainerInfoV1.BottomRenderedGroupFooters[parentFooter] = comps;
                }

                comps.Add(footer);

                if (parent.ContainerInfoV1.BottomRenderedParentsGroupFooters.IndexOf(parentFooter) == -1)
                {
                    parent.ContainerInfoV1.BottomRenderedParentsGroupFooters.Insert(0, parentFooter);
                }
            }
        }


        internal void RemoveBottomRenderedGroupFooters(StiDataBand masterDataBand, StiComponent footer, StiGroupFooterBand parentFooter)
        {
            StiContainer parent = parentFooter.Parent;

            if (parent.ContainerInfoV1.BottomRenderedGroupFooters == null) return;
            StiComponentsCollection comps = parent.ContainerInfoV1.BottomRenderedGroupFooters[parentFooter] as StiComponentsCollection;
            if (comps == null) return;
            if (comps.Contains(footer)) comps.Remove(footer);
        }


        internal void SetLastDataBand(StiDataBand masterDataBand)
        {
            StiDataBand band = masterDataBand;

            while (band.MasterComponent != null && band.MasterComponent is StiDataBand)
            {
                band = band.MasterComponent as StiDataBand;
            }

            StiContainer parent = band.Parent;
            parent.ContainerInfoV1.LastDataBand = masterDataBand;
        }
        #endregion

        #region Methods.Helpers
        /// <summary>
        /// Moves on the next row.
        /// </summary>
        public virtual void MoveNext(StiDataBand masterDataBand)
        {
            ResetStartNewPageProcessed(masterDataBand);
            masterDataBand.Next();
            StiDataBandV1Builder.SetDetails(masterDataBand);
        }

        internal void ApplyStyle(StiDataBand masterDataBand, StiContainer cont)
        {
            if (masterDataBand.Report != null)
            {
                if ((masterDataBand.LineThrough & 1) == 1)
                {
                    if (!string.IsNullOrEmpty(masterDataBand.EvenStyle))
                    {
                        StiBaseStyle st = masterDataBand.Report.Styles[masterDataBand.EvenStyle];
                        if (st != null)
                        {
                            st.SetStyleToComponent(cont);

                            StiBaseStyle newStyle = StiBaseStyle.GetStyle(cont);
                            cont.SetParentStylesToChilds(newStyle);
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(masterDataBand.OddStyle))
                    {
                        StiBaseStyle st = masterDataBand.Report.Styles[masterDataBand.OddStyle];
                        if (st != null)
                        {
                            st.SetStyleToComponent(cont);

                            StiBaseStyle newStyle = StiBaseStyle.GetStyle(cont);
                            cont.SetParentStylesToChilds(newStyle);
                        }
                    }
                }
            }
        }


        internal bool IsAcrossThenDown(StiDataBand masterDataBand)
        {
            return masterDataBand.Columns > 1 && masterDataBand.ColumnDirection == StiColumnDirection.AcrossThenDown;
        }


        /// <summary>
        /// Used for support column mode with grouping, Across Then Down
        /// </summary>
        internal bool IsCrossColumn(StiDataBand masterDataBand)
        {
            return (masterDataBand is StiCrossDataBand && ((StiCrossDataBand) masterDataBand).ColumnMode);
        }

        public virtual void SetDataIsPrepared(StiDataBand masterDataBand, bool value)
        {
            masterDataBand.DataBandInfoV1.DataIsPrepared = value;
        }


        public virtual bool GetDataIsPrepared(StiDataBand masterDataBand)
        {
            return masterDataBand.DataBandInfoV1.DataIsPrepared;
        }

        /// <summary>
        /// Checks availability of free space in the container. And adds it, if it may check availability of free space in the container.
        /// </summary>
        /// <param name="outContainer">Container to check free space.</param>
        /// <returns>Result: if true then no free space.</returns>
        public virtual bool CheckFreeSpace(StiDataBand masterDataBand, StiContainer outContainer)
        {
            if (masterDataBand.DataBandInfoV1.FreeSpace < 0)
            {
                if (masterDataBand.Page.UnlimitedHeight)
                {
                    DecFreeSpace(masterDataBand, outContainer);
                    outContainer.AddSize(false);
                    IncFreeSpace(masterDataBand, outContainer);
                }
                else return true;
            }

            return false;
        }


        public virtual void SetReportVariables(StiDataBand masterDataBand)
        {
            masterDataBand.Report.LineThrough = masterDataBand.LineThrough;
            masterDataBand.Report.Line = masterDataBand.DataBandInfoV1.RuntimeLine - masterDataBand.DataBandInfoV1.StartLine + 1;
        }


        private void SaveStateColumnsAcrossThenDown(StiDataBand masterDataBand, string name)
        {
            if (masterDataBand.ColumnDirection == StiColumnDirection.AcrossThenDown)
            {
                if ((!IsCrossColumn(masterDataBand)) && masterDataBand.Columns > 1)
                    masterDataBand.Report.SaveState(masterDataBand.Name + name);
            }
        }


        private void RestoreStateColumnsAcrossThenDown(StiDataBand masterDataBand, string name)
        {
            if (masterDataBand.ColumnDirection == StiColumnDirection.AcrossThenDown)
            {
                if ((!IsCrossColumn(masterDataBand)) && masterDataBand.Columns > 1)
                {
                    masterDataBand.Page.PageInfoV1.RestoreCurrentDetailComponent = false;
                    masterDataBand.Report.RestoreState(masterDataBand.Name + name);
                    masterDataBand.Page.PageInfoV1.RestoreCurrentDetailComponent = true;
                    return;
                }
            }

            masterDataBand.Report.RestoreState("DataBand" + masterDataBand.Name);
        }


        internal bool IsDataBandInMasterBand(StiDataBand masterDataBand)
        {
            if (masterDataBand.IsCross) return false;

            StiContainer parent = masterDataBand.Parent;

            while (parent != null)
            {
                if (parent is StiDataBand) return true;
                parent = parent.Parent;
            }

            return false;
        }


        internal void CheckParentContainerBeforeRendering(StiDataBand masterDataBand, StiContainer outContainer)
        {
            bool resize = false;
            if (masterDataBand.Parent != null && masterDataBand.Parent.Parent is StiDataBand) resize = true;

            if ((!(masterDataBand is StiCrossDataBand)) && (resize || outContainer.CanGrow || outContainer.CanShrink) && masterDataBand.Columns < 2)
            {
                double top = outContainer.ComponentToPage(outContainer.Top);
                masterDataBand.DataBandInfoV1.ResHeightOfContainerBeforeRendering = outContainer.Height;
                outContainer.Height = outContainer.Page.Height - top;
            }
        }


        internal void CheckParentContainerAfterRendering(StiDataBand masterDataBand, StiContainer outContainer)
        {
            bool resize = false;
            if (masterDataBand.Parent != null && masterDataBand.Parent.Parent is StiDataBand) resize = true;

            if ((!(masterDataBand is StiCrossDataBand)) && (resize || outContainer.CanGrow || outContainer.CanShrink) && masterDataBand.Columns < 2)
            {
                double maxHeight = 0;
                foreach (StiComponent comp in outContainer.Components)
                {
                    maxHeight = Math.Max(comp.Bottom, maxHeight);
                }

                if (maxHeight > masterDataBand.DataBandInfoV1.ResHeightOfContainerBeforeRendering && (outContainer.CanGrow || resize))
                {
                    outContainer.Height = maxHeight;
                    return;
                }

                if (maxHeight < masterDataBand.DataBandInfoV1.ResHeightOfContainerBeforeRendering && (outContainer.CanShrink || resize))
                {
                    outContainer.Height = maxHeight;
                    return;
                }

                outContainer.Height = masterDataBand.DataBandInfoV1.ResHeightOfContainerBeforeRendering;

                #region Correction for Databand.GrowToheight in other databand
                if (masterDataBand.Parent.GrowToHeight && outContainer.Components.Count > 0)
                {
                    outContainer.Components[outContainer.Components.Count - 1].GrowToHeight = true;
                }
                #endregion
            }
        }


        internal void SetParentGrowToHeightInContainer(StiDataBand masterDataBand, StiContainer outContainer)
        {
            if ((!(masterDataBand is StiCrossDataBand)) && masterDataBand.Columns < 2)
            {
                if (masterDataBand.Parent.GrowToHeight && outContainer.Components.Count > 0)
                {
                    outContainer.Components[outContainer.Components.Count - 1].GrowToHeight = true;
                }
            }
        }


        /// <summary>
        /// Move databands placed on a container, that is placed on databands, to a master databand.
        /// </summary>
        internal void MoveDataBandsToMasterDataband(StiDataBand masterDataBand, StiContainer outContainer)
        {
            outContainer.DockToContainer();

            foreach (StiComponent comp in outContainer.Components)
            {
                comp.DockStyle = StiDockStyle.None;
                comp.Left += outContainer.Left;
                outContainer.Parent.Components.Add(comp);
            }

            outContainer.Height = 0;
            outContainer.Components.Clear();
        }


        protected virtual void ProcessRenderedContainer(StiDataBand masterDataBand, StiContainer container)
        {
        }


        private void ProcessDataItemsOnDataBand(StiContainer container)
        {
            foreach (StiComponent comp in container.Components)
            {
                if (comp.IsCross) comp.Prepare();
                if (comp is StiDataBand && (!(comp is StiCrossDataBand))) comp.Prepare();

                StiContainer cont = comp as StiContainer;
                if (cont != null)
                {
                    ProcessDataItemsOnDataBand(cont);
                }
            }
        }


        private StiContainer GetRenderContainer(StiDataBand masterDataBand, StiContainer outContainer)
        {
            if (IsAcrossThenDown(masterDataBand))
            {
                if (masterDataBand.DataBandInfoV1.ColumnIndex == 1)
                {
                    masterDataBand.DataBandInfoV1.ParentColumnContainer = new StiContainer();
                    masterDataBand.DataBandInfoV1.ParentColumnContainer.Width = masterDataBand.Width;
                    masterDataBand.DataBandInfoV1.ParentColumnContainer.Height = masterDataBand.Height;
                    masterDataBand.DataBandInfoV1.ParentColumnContainer.DockStyle = StiDockStyle.Top;
                    outContainer.Components.Add(masterDataBand.DataBandInfoV1.ParentColumnContainer);
                }

                masterDataBand.Report.Column = masterDataBand.DataBandInfoV1.ColumnIndex;
                StiContainer newCont = new StiContainer();
                newCont.DockStyle = masterDataBand.RightToLeft ? StiDockStyle.Right : StiDockStyle.Left;
                newCont.Width = masterDataBand.GetColumnWidth() + masterDataBand.ColumnGaps;
                newCont.Height = masterDataBand.Height * 2;

                masterDataBand.DataBandInfoV1.ParentColumnContainer.Components.Add(newCont);

                masterDataBand.DataBandInfoV1.ColumnIndex++;
                if (masterDataBand.DataBandInfoV1.ColumnIndex > masterDataBand.Columns) masterDataBand.DataBandInfoV1.ColumnIndex = 1;

                masterDataBand.DataBandInfoV1.FreeSpace = newCont.Height;

                return newCont;
            }
            else return outContainer;
        }


        /// <summary>
        ///  Renders one row with all details.
        /// </summary>
        /// <param name="breaked">If true, then rendering is finished because there is no free space.
        public virtual bool RenderItem(StiDataBand masterDataBand, out bool breaked, StiContainer outContainer)
        {
            bool forceCanBreak = masterDataBand.CanBreak;

            StiComponent renderedComponent = null;

            #region If Columns > 1 then SaveState
            SaveStateColumnsAcrossThenDown(masterDataBand, "Columns");
            #endregion

            #region Prepare Cross Components
            if (!masterDataBand.IsCross)
            {
                if (masterDataBand.DataBandInfoV1.CrossTabExistOnDataBand) forceCanBreak = true;
                if (masterDataBand.DataBandInfoV1.SubReportsComponents.Count > 0) forceCanBreak = true;

                ProcessDataItemsOnDataBand(masterDataBand);
            }
            #endregion

            #region Prepare Subreport
            bool subReportRenderResult = GetSubReportRenderResult(masterDataBand);
            if (subReportRenderResult)
            {
                foreach (StiComponent comp in masterDataBand.DataBandInfoV1.SubReportsComponents) comp.Prepare();
            }
            #endregion

            breaked = false;
            bool renderResult = true;

            int curCount = outContainer.Components.Count;
            masterDataBand.DataBandInfoV1.SkipStartNewPage = true;

            #region Render Item
            if ((!IsDetailDataSourcesEmpty(masterDataBand)) || masterDataBand.PrintIfDetailEmpty)
            {
                masterDataBand.DataBandInfoV1.SkipStartNewPage = false;

                SetReportVariables(masterDataBand);
                masterDataBand.Report.SaveState("DataBand" + masterDataBand.Name);

                #region Conditions
                StiBrush savedBrush = masterDataBand.Brush;
                bool savedEnabled = masterDataBand.Enabled;
                #endregion

                masterDataBand.InvokeBeforePrint(masterDataBand, EventArgs.Empty);

                if (masterDataBand.CalcInvisible)
                {
                    masterDataBand.InvokeRendering();
                    masterDataBand.InvokeGroupRendering();
                }

                try
                {
                    if (masterDataBand.Enabled)
                    {
                        masterDataBand.DoGetBookmark();

                        if (!masterDataBand.CalcInvisible)
                        {
                            masterDataBand.InvokeRendering();
                            masterDataBand.InvokeGroupRendering();
                        }

                        masterDataBand.RenderedCount++;

                        masterDataBand.DoBookmark();
                        masterDataBand.DoPointer();

                        #region Render Master component
                        subReportRenderResult = true;
                        //Render if - 
                        //object is not rendered,
                        //object render on all detail,
                        //amount details component is a zero
                        if ((!masterDataBand.IsRendered) || masterDataBand.PrintOnAllPages || masterDataBand.DataBandInfoV1.DetailComponents.Count == 0)
                        {
                            masterDataBand.RenderContainer(ref renderedComponent, outContainer);
                            ProcessRenderedContainer(masterDataBand, renderedComponent as StiContainer);
                            if (masterDataBand.PrintAtBottom)
                            {
                                AddBottomRenderedDataBands(masterDataBand, renderedComponent, masterDataBand);
                            }

                            ApplyStyle(masterDataBand, renderedComponent as StiContainer);

                            if (masterDataBand.DataBandInfoV1.ItemsActive) masterDataBand.DataBandInfoV1.RenderedItems.Add(renderedComponent);

                            DecFreeSpace(masterDataBand, renderedComponent);

                            #region Check SubReport
                            subReportRenderResult = GetSubReportRenderResult(masterDataBand);
                            #endregion
                        }
                        #endregion

                        double freeSpaceTemp = masterDataBand.DataBandInfoV1.FreeSpace;
                        SecondPassBreak(masterDataBand, renderedComponent, masterDataBand, ref freeSpaceTemp);
                        masterDataBand.DataBandInfoV1.FreeSpace = freeSpaceTemp;

                        bool outFreeSpace = CheckFreeSpace(masterDataBand, outContainer);

                        #region Render Detail component
                        //Render if - 
                        //there is free place and 
                        //amount details components more zero
                        if (masterDataBand.Columns < 2 && ((!outFreeSpace) &&
                                                           masterDataBand.DataBandInfoV1.DetailComponents != null && masterDataBand.DataBandInfoV1.DetailComponents.Count > 0))
                        {
                            RenderDetails(masterDataBand, outContainer);

                            //Get free space
                            masterDataBand.DataBandInfoV1.FreeSpace = GetFreeSpaceFromRectangle(masterDataBand.GetDockRegion(outContainer));

                            #region If there is current Detail component, that detail components is not rendered - break renders
                            if (masterDataBand.DataBandInfoV1.CurrentDetailComponent is IStiBreakable && ((IStiBreakable)
                                    masterDataBand.DataBandInfoV1.CurrentDetailComponent).CanBreak)
                            {
                                masterDataBand.DataBandInfoV1.LatestDataBandBreaked = true;
                            }
                            else masterDataBand.DataBandInfoV1.LatestDataBandBreaked = false;

                            if (masterDataBand.DataBandInfoV1.CurrentDetailComponent != null)
                            {
                                renderResult = false;

                                #region If render Keep Child Together
                                bool canBreakFlag = false;
                                IStiBreakable breakable = masterDataBand.DataBandInfoV1.CurrentDetailComponent as IStiBreakable;
                                if (breakable != null) canBreakFlag = breakable.CanBreak;

                                if (masterDataBand.KeepChildTogether && (masterDataBand.DataBandInfoV1.FirstRow == false || masterDataBand.DataBandInfoV1.AlwaysKeepChildTogether) && (!canBreakFlag))
                                {
                                    #region Remove all that have rendered
                                    masterDataBand.IsRendered = false;

                                    masterDataBand.DataBandInfoV1.CurrentDetailComponent = null;
                                    masterDataBand.Report.RestoreState("DataBand" + masterDataBand.Name);

                                    int cnt = outContainer.Components.Count;
                                    for (int index = 0; index < cnt - curCount; index++)
                                        outContainer.Components.RemoveAt(curCount);

                                    if (IsKeepHeaderTogether(masterDataBand) && masterDataBand.DataBandInfoV1.FirstRow && (!masterDataBand.DataBandInfoV1.LatestDataBandBreaked))
                                        RemoveRenderedHeaders(masterDataBand, outContainer);

                                    if (masterDataBand.DataBandInfoV1.ItemsActive) masterDataBand.DataBandInfoV1.RenderedItems.RemoveAt(masterDataBand.DataBandInfoV1.RenderedItems.Count - 1);
                                    masterDataBand.DataBandInfoV1.BreakableComps = null;
                                    #endregion
                                }
                                #endregion

                                breaked = true;
                                return renderResult;
                            }
                            else masterDataBand.IsRendered = false;
                            #endregion
                        }
                        #endregion

                        #region If master component was beyond the scope of
                        if (outFreeSpace)
                        {
                            #region If last rendered component is breaked, decrease size of component
                            bool forceRender = false;

                            if ((!forceCanBreak) && (masterDataBand.DataBandInfoV1.LastRenderBreaked || forceRender))
                                //changed in version 1.60, before change : )if ((!CanBreak) && (lastRenderBreaked || forceRender || firstRow))
                                //changed in version 1.40, before change : )//if (lastRenderBreaked || firstRow)
                            {
                                double oldWidth = renderedComponent.Width;
                                double oldHeight = renderedComponent.Height;

                                StiContainer renderedCont = renderedComponent as StiContainer;
                                renderedComponent.Height += masterDataBand.DataBandInfoV1.FreeSpace;
                                foreach (StiComponent comp in renderedCont.Components)
                                {
                                    if (comp.Bottom >= renderedComponent.Height)
                                    {
                                        comp.Height -= comp.Bottom - renderedComponent.Height;
                                        comp.Height = Math.Max(0, comp.Height);
                                    }
                                }

                                outFreeSpace = false;

                                masterDataBand.DataBandInfoV1.FreeSpace = 0;
                            }
                            #endregion

                            #region Out of free space
                            else
                            {
                                #region Have rendered not all
                                renderResult = false;
                                masterDataBand.IsRendered = false;
                                breaked = true;
                                masterDataBand.DataBandInfoV1.LastRenderBreaked = true;
                                #endregion

                                if (forceCanBreak)
                                {
                                    double freeSpaceTemp2 = masterDataBand.DataBandInfoV1.FreeSpace;
                                    bool wasBreaked = FirstPassBreak(masterDataBand, renderedComponent, masterDataBand, ref freeSpaceTemp2);
                                    if (!wasBreaked)
                                    {
                                        breaked = false;
                                        renderResult = true;
                                    }
                                    masterDataBand.DataBandInfoV1.FreeSpace = freeSpaceTemp2;

                                    RestoreStateColumnsAcrossThenDown(masterDataBand, "Columns");
                                }
                                else
                                {
                                    //Remove last rendered component
                                    if (renderedComponent != null) outContainer.Components.Remove(renderedComponent);
                                    if (masterDataBand.DataBandInfoV1.ItemsActive) masterDataBand.DataBandInfoV1.RenderedItems.RemoveAt(masterDataBand.DataBandInfoV1.RenderedItems.Count - 1);
                                    IncFreeSpace(masterDataBand, renderedComponent);

                                    #region Failed render band - restore state
                                    RestoreStateColumnsAcrossThenDown(masterDataBand, "Columns");
                                    #endregion

                                    #region Remove rendered headers (if it is necessary)
                                    if (masterDataBand.DataBandInfoV1.FirstRow && IsKeepHeaderTogether(masterDataBand) && (!masterDataBand.DataBandInfoV1.LatestDataBandBreaked))
                                    {
                                        RemoveRenderedHeaders(masterDataBand, outContainer);
                                    }
                                    #endregion
                                }

                                return renderResult;
                            }
                            #endregion
                        }

                        #region Check SubReport
                        if (!subReportRenderResult)
                        {
                            renderResult = false;
                            breaked = true;
                            return renderResult;
                        }
                        #endregion

                        if (!outFreeSpace)
                        {
                            masterDataBand.InvokeAfterPrint(this, EventArgs.Empty);
                            masterDataBand.DataBandInfoV1.LastRenderBreaked = false;
                            masterDataBand.DataBandInfoV1.LastComponent = renderedComponent;
                        }
                        #endregion
                    }
                    else masterDataBand.InvokeAfterPrint(masterDataBand, EventArgs.Empty);
                }
                finally
                {
                    #region Restore Conditions
                    masterDataBand.Brush = savedBrush;
                    masterDataBand.Enabled = savedEnabled;
                    #endregion
                }
            }
            else
            {
                renderResult = true;
            }
            #endregion

            return renderResult;
        }


        /// <summary>
        /// Band rendering without event.
        /// </summary>
        /// <param name="renderedComponent">Rendered component.</param>
        /// <param name="outContainer">The panel in which rendering will be done.</param>
        /// <returns>Is rendering finished or not?</returns>		
        public virtual bool InternalRender(StiDataBand masterDataBand, ref StiComponent renderedComponent, StiContainer outContainer, bool value)
        {
            //Prepare detail
            if (masterDataBand.Position == 0 && (!masterDataBand.IsRendered)) SetDetails(masterDataBand);

            //Prepare for rerender band with resetdatasource
            if (masterDataBand.DataBandInfoV1.FirstCall && masterDataBand.ResetDataSource)
            {
                StiDataHelper.SetData(masterDataBand, true);
                SetDataIsPrepared(masterDataBand, true);
            }
            else if (!GetDataIsPrepared(masterDataBand))
                SetData(masterDataBand);

            masterDataBand.DataBandInfoV1.FirstCall = false;
            masterDataBand.DataBandInfoV1.FirstGroupOnPass = true;

            #region Columns DownThenCross
            int columnsPosition = masterDataBand.Position;
            int columnsPage = 0;
            if (masterDataBand.Columns > 1)
            {
                columnsPage = (masterDataBand.Count - columnsPosition) / masterDataBand.Columns;
                if ((columnsPage * masterDataBand.Columns + columnsPosition) < masterDataBand.Count) columnsPage++;
                if (masterDataBand.MinRowsInColumn > 0) columnsPage = Math.Max(masterDataBand.MinRowsInColumn, columnsPage);
            }
            #endregion

            masterDataBand.DataBandInfoV1.RenderedItems = new List<StiComponent>();

            masterDataBand.DataBandInfoV1.ResParentBookmark = masterDataBand.ParentBookmark;
            masterDataBand.CurrentBookmark = masterDataBand.ParentBookmark;

            masterDataBand.DataBandInfoV1.LastComponent = null;
            masterDataBand.DataBandInfoV1.FirstRowInPath = true;
            masterDataBand.DataBandInfoV1.ForceStartNewPage = false;

            bool renderResult = true; //Render result
            masterDataBand.DataBandInfoV1.FirstRow = true;

            #region StiGroupHeaderBand PrintOnAllPages
            foreach (StiGroupHeaderBand band in masterDataBand.DataBandInfoV1.GroupHeaderComponents)
            {
                if (band.PrintOnAllPages) band.GroupHeaderBandInfoV1.Rerender = true;
            }
            #endregion

            //Gets free space
            masterDataBand.DataBandInfoV1.FreeSpace = GetFreeSpaceFromRectangle(masterDataBand.GetDockRegion(outContainer));

            #region Render not rendered footers
            ArrayList remmitedBottomRenderedFooters = null;
            bool doNotRenderFooters = masterDataBand.DataBandInfoV1.RemmitedCollection.Count > 0;
            foreach (StiComponent comp in masterDataBand.DataBandInfoV1.RemmitedCollection)
            {
                masterDataBand.DataBandInfoV1.ForceStartNewPage = true;

                outContainer.Components.Add(comp);

                #region PrintAtBottom
                bool printAtBottom = false;
                foreach (StiFooterBand footer in masterDataBand.DataBandInfoV1.FooterComponents)
                {
                    if (footer.Name == comp.Name && footer.PrintAtBottom)
                    {
                        printAtBottom = true;
                        break;
                    }
                }

                if (!printAtBottom)
                {
                    foreach (StiGroupFooterBand footer in masterDataBand.DataBandInfoV1.GroupFooterComponents)
                    {
                        if (footer.Name == comp.Name && footer.PrintAtBottom)
                        {
                            if (remmitedBottomRenderedFooters == null) remmitedBottomRenderedFooters = new ArrayList();
                            remmitedBottomRenderedFooters.Add(comp);
                            printAtBottom = true;
                            break;
                        }
                    }
                }

                if (printAtBottom) comp.DockStyle = StiDockStyle.Bottom;
                #endregion

                if (comp.Dockable) comp.DisplayRectangle = outContainer.DockToContainer(comp.DisplayRectangle);
                DecFreeSpace(masterDataBand, comp);
            }

            masterDataBand.DataBandInfoV1.RemmitedCollection.Clear();
            int lastIndexOfRemmitedFooters = outContainer.Components.Count;
            #endregion

            if (masterDataBand.IsEof && masterDataBand.Count > 0) return true;

            #region Render headers
            if (!RenderHeaders(masterDataBand, outContainer)) return false;
            #endregion

            #region Render footers (First pass)
            StiComponentsCollection footers = new StiComponentsCollection();
            StiComponentsCollection parentFooters = new StiComponentsCollection();
            int startFooterIndex = outContainer.Components.Count;
            if (RenderFootersOnAllPagesFirst(masterDataBand, ref footers, ref parentFooters, outContainer) == false) return false;
            int footersOnAllPagesCount = outContainer.Components.Count - startFooterIndex;
            #endregion

            int footerIndex = 0;
            masterDataBand.DataBandInfoV1.StartGroupIndex = -1;
            masterDataBand.DataBandInfoV1.StartMasterIndex = outContainer.Components.Count;

            int startIndex = outContainer.Components.Count;

            #region Render item
            while ((!masterDataBand.IsEof) && (!masterDataBand.IsEmpty))
            {
                doNotRenderFooters = false;

                //Remember position
                startIndex = outContainer.Components.Count;
                masterDataBand.Report.SaveState(masterDataBand.Name + "Item");

                #region Render group headers
                bool startNewPage = false;
                renderResult = RenderGroupHeaders(masterDataBand, outContainer, ref startNewPage);

                //Headers did not fit - break
                if (!renderResult) break;
                #endregion

                int endIndex = outContainer.Components.Count;

                bool breaked = false;

                #region RenderItem
                #region Support groups in Across Then Down
                if (masterDataBand.Columns >= 2 && masterDataBand.ColumnDirection == StiColumnDirection.AcrossThenDown)
                {
                    if (masterDataBand.DataBandInfoV1.GroupHeaderComponents.Count > 0 && IsGroupFooterResult(masterDataBand))
                    {
                        masterDataBand.DataBandInfoV1.ColumnIndex = 1;
                    }
                }
                #endregion

                double resFreeSpace = masterDataBand.DataBandInfoV1.FreeSpace;
                StiContainer renderContainer = GetRenderContainer(masterDataBand, outContainer);
                renderResult = RenderItem(masterDataBand, out breaked, renderContainer);

                #region Correct FreeSpace for Across Then Down mode
                if (masterDataBand.Columns >= 2 && masterDataBand.ColumnDirection == StiColumnDirection.AcrossThenDown)
                {
                    masterDataBand.DataBandInfoV1.FreeSpace = resFreeSpace;
                    if (endIndex < outContainer.Components.Count)
                    {
                        masterDataBand.DataBandInfoV1.FreeSpace -= outContainer.Components[endIndex].Height;

                        if (masterDataBand.DataBandInfoV1.FreeSpace < 0)
                        {
                            renderResult = false;
                            breaked = true;
                            outContainer.Components.RemoveAt(endIndex);
                            masterDataBand.Report.RestoreState(masterDataBand.Name + "Item");
                        }
                    }

                    #region Correct components location in right to left mode
                    if (masterDataBand.RightToLeft)
                    {
                        renderContainer = renderContainer.Components[0] as StiContainer;
                        foreach (StiComponent comp in renderContainer.Components)
                        {
                            comp.Left += masterDataBand.ColumnGaps;
                        }
                    }
                    #endregion
                }
                #endregion
                #endregion

                //If iteration not passed
                if (!renderResult)
                {
                    #region If it is necessary to keep group together, that removes whole group
                    if (masterDataBand.DataBandInfoV1.StartGroupIndex != -1 &&
                        masterDataBand.KeepGroupTogether && masterDataBand.DataBandInfoV1.StartGroupIndex != endIndex &&
                        (masterDataBand.DataBandInfoV1.FirstGroupOnPass == false || StiOptions.Engine.IgnoreFirstPassForGroup)) //Changed in 2007.2
                    {
                        RemoveFromContainerAt(masterDataBand, outContainer, masterDataBand.DataBandInfoV1.StartGroupIndex);
                        masterDataBand.Report.RestoreState(masterDataBand.Name + "Group");
                        //Fix bug with new group and subreports line and totals
                        foreach (StiComponent comp in masterDataBand.DataBandInfoV1.SubReportsComponents) comp.Prepare();
                        masterDataBand.DataBandInfoV1.BreakableComps = null;
                    }
                    #endregion

                    #region If it is necessary to keep headers, that removes all that have rendered for this pass
                    else if (IsKeepHeaderTogether(masterDataBand) && startIndex != endIndex && (!masterDataBand.DataBandInfoV1.LatestDataBandBreaked))
                    {
                        if (masterDataBand.DataBandInfoV1.GroupHeaderComponents.Count > 0)
                        {
                            RemoveFromContainerAt(masterDataBand, outContainer, startIndex);
                            masterDataBand.Report.RestoreState(masterDataBand.Name + "Group");
                        }

                        //Fix bug with new group and subreports line and totals
                        foreach (StiComponent comp in masterDataBand.DataBandInfoV1.SubReportsComponents) comp.Prepare();
                        masterDataBand.DataBandInfoV1.BreakableComps = null;
                    }
                    #endregion
                }

                StiComponentsCollection renderedGroupFooters = null;

                #region If iteration is passed successfully then render group footers
                if (renderResult)
                {
                    //Where have begun to render footers
                    footerIndex = outContainer.Components.Count;

                    renderResult = RenderGroupFooters(masterDataBand, ref footerIndex, outContainer, out renderedGroupFooters);
                    //Unchancy attempt
                    if (!renderResult)
                    {
                        #region If it is necessary to keep group - remove whole group
                        if (masterDataBand.DataBandInfoV1.StartGroupIndex != -1 &&
                            masterDataBand.KeepGroupTogether && masterDataBand.DataBandInfoV1.StartGroupIndex != endIndex &&
                            (masterDataBand.DataBandInfoV1.FirstGroupOnPass == false || StiOptions.Engine.IgnoreFirstPassForGroup)) //Changed in 2007.2
                        {
                            RemoveFromContainerAt(masterDataBand, outContainer, masterDataBand.DataBandInfoV1.StartGroupIndex);
                            masterDataBand.Report.RestoreState(masterDataBand.Name + "Group");
                            renderedGroupFooters = null;
                            masterDataBand.DataBandInfoV1.RemmitedCollection.Clear();
                            masterDataBand.DataBandInfoV1.BreakableComps = null;
                        }
                        #endregion

                        #region If it is necessary to keep footers - remove all that have rendered for this pass
                        else if (masterDataBand.KeepFooterTogether && (!masterDataBand.DataBandInfoV1.FirstRowInPath))
                        {
                            RemoveFromContainerAt(masterDataBand, outContainer, startIndex);
                            masterDataBand.Report.RestoreState(masterDataBand.Name + "Item");
                            renderedGroupFooters = null;
                            masterDataBand.DataBandInfoV1.RemmitedCollection.Clear();
                            masterDataBand.DataBandInfoV1.BreakableComps = null;
                        }
                        #endregion

                        #region If footers to keep not it is necessary, that removes only footers
                        else
                        {
                            //for (int index = footerIndex; index < outContainer.Components.Count; index++)
                            //{
                            //	remmitedCollection.Add(outContainer.Components[index]);
                            //}

                            //RemoveFromContainerAt(outContainer, footerIndex);
                            //foreach (StiGroupHeaderBand band in GroupHeaderComponents)band.Prepare();

                            MoveNext(masterDataBand);
                        }
                        #endregion

                        breaked = true;
                    }
                }
                #endregion

                #region If group footers present
                if (renderedGroupFooters != null && renderedGroupFooters.Count > 0)
                {
                    RemoveFromContainerAt(masterDataBand, outContainer, footerIndex);
                    foreach (StiGroupHeaderBand groupHeader in renderedGroupFooters)
                    {
                        StiGroupFooterBand groupFooter = groupHeader.GroupHeaderBandInfoV1.GroupFooter;
                        StiComponent outFooter = null;

                        if (groupFooter != null)
                        {
                            RenderOneGroupFooter(masterDataBand, groupFooter, ref outFooter, outContainer);
                            if (outFooter != null) DecFreeSpace(masterDataBand, outFooter);
                        }

                        //groupHeader.InvokeEndRender();								
                        masterDataBand.DataBandInfoV1.FirstGroupOnPass = false;

                        #region Support PrintAtBottom property
                        if (groupFooter != null && groupFooter.PrintAtBottom && outFooter != null)
                        {
                            AddBottomRenderedGroupFooters(masterDataBand, outFooter, groupFooter);
                            groupFooter.GroupFooterBandInfoV1.PrintAtBottomComponent = outFooter;
                        }
                        #endregion
                    }
                }
                #endregion

                #region Break column on DownThenAcross
                if (masterDataBand.ColumnDirection == StiColumnDirection.DownThenAcross &&
                    masterDataBand.DataBandInfoV1.ItemsActive && (masterDataBand.Position >= (columnsPosition + columnsPage - 1))) breaked = true;
                #endregion

                #region Move to Next
                if (renderResult)
                {
                    MoveNext(masterDataBand);

                    if (masterDataBand.IsEof) breaked = true;

                    #region IStiStartNewPage
                    else if (masterDataBand.StartNewPage && (!masterDataBand.DataBandInfoV1.SkipStartNewPage))
                    {
                        #region If current detail set is empty then search next not empty
                        if (!masterDataBand.PrintIfDetailEmpty)
                        {
                            while ((!masterDataBand.IsEof) && IsDetailDataSourcesEmpty(masterDataBand))
                            {
                                MoveNext(masterDataBand);
                            }
                        }
                        #endregion

                        if (!masterDataBand.IsEof)
                        {
                            double factor = 100 * masterDataBand.DataBandInfoV1.FreeSpace / masterDataBand.Page.Height;
                            if (masterDataBand.StartNewPageIfLessThan > factor || masterDataBand.StartNewPageIfLessThan == 100)
                            {
                                renderResult = false;
                                breaked = true;
                            }
                        }
                    }
                    #endregion

                    masterDataBand.DataBandInfoV1.FirstRow = false;
                    masterDataBand.DataBandInfoV1.FirstRowInPath = false;
                }
                #endregion

                if (breaked) break;
            }
            #endregion

            #region Render footers (Second pass)
            //Render obligatory footers
            RenderFootersOnAllPagesSecond(masterDataBand, ref footers, ref parentFooters, outContainer);

            int startIndexFooter = outContainer.Components.Count;

            //Render footers at the end data or groups
            ArrayList renderedFooters = null;

            if (masterDataBand.IsEof) masterDataBand.InvokeEndRender();

            bool footerResult = false;
            if (doNotRenderFooters) footerResult = true;
            if (!doNotRenderFooters)
            {
                bool newPageStarted = false;
                bool isBreaked = false;

                footerResult = RenderFootersSecond(masterDataBand, out renderedFooters, outContainer, ref newPageStarted, ref isBreaked);

                if (!footerResult) renderResult = false;
                //Footers did not fit;
                if (!footerResult && masterDataBand.KeepFooterTogether && (!newPageStarted) && (!isBreaked))
                {
                    //newPageStarted = false;
                    if (masterDataBand.DataBandInfoV1.FooterComponents.Count > 0)
                    {
                        //Remove rendered footers
                        foreach (StiComponent compFooter in renderedFooters)
                        {
                            if (outContainer.Components.Contains(compFooter))
                                outContainer.Components.Remove(compFooter);
                        }

                        RemoveFromContainerAt(masterDataBand, outContainer,
                            startIndex - footersOnAllPagesCount, outContainer.Components.Count - footersOnAllPagesCount);

                        //if (masterDataBand.Report.States.IsExist(masterDataBand.Name + "Item", masterDataBand))
                        masterDataBand.Report.RestoreState(masterDataBand.Name + "Item");
                        SetDetails(masterDataBand);

                        masterDataBand.IsRendered = false;
                        renderResult = false;

                        masterDataBand.DataBandInfoV1.RemmitedCollection.Clear();
                    }
                }
            }
            #endregion

            if (masterDataBand.IsEof)
            {
                if (masterDataBand.DataBandInfoV1.RemmitedCollection.Count == 0) renderResult = true;
            }

            if (!footerResult)
            {
                renderResult = false;
            }

            #region Process Remmited Bottom Rendered Footers
            if (outContainer.Components.Count != lastIndexOfRemmitedFooters && remmitedBottomRenderedFooters != null)
            {
                foreach (StiComponent comp in remmitedBottomRenderedFooters)
                {
                    comp.DockStyle = StiDockStyle.Top;
                }
            }
            #endregion

            return renderResult;
        }
        #endregion

        #region Methods.Render
        /// <summary>
        /// Prepares a component for rendering.
        /// </summary>
        public override void Prepare(StiComponent masterComp)
        {
            base.Prepare(masterComp);

            StiDataBand masterDataBand = masterComp as StiDataBand;
            masterDataBand.DataBandInfoV1.DataIsPrepared = false;

            if (masterDataBand.IsDataSourceEmpty)
            {
                masterDataBand.IsEof = false;
            }

            masterDataBand.DataBandInfoV1.FirstCall = true;

            #region Init headers, footers, details
            masterDataBand.DataBandInfoV1.HeaderComponents = GetHeaders(masterDataBand);
            masterDataBand.DataBandInfoV1.FooterComponents = GetFooters(masterDataBand);
            masterDataBand.DataBandInfoV1.DetailComponents = GetDetails(masterDataBand);
            masterDataBand.DataBandInfoV1.DetailDataComponents = GetDataDetails(masterDataBand);
            masterDataBand.DataBandInfoV1.SubReportsComponents = GetSubReports(masterDataBand);
            masterDataBand.DataBandInfoV1.GroupHeaderComponents = GetGroupHeaders(masterDataBand);
            masterDataBand.DataBandInfoV1.GroupFooterComponents = GetGroupFooters(masterDataBand);
            #endregion

            StiFilterHelper.SetFilter(masterDataBand);

            GroupsComparison(masterDataBand);

            #region Prepare headers and footers
            foreach (StiComponent comp in masterDataBand.DataBandInfoV1.HeaderComponents) comp.Prepare();
            foreach (StiComponent comp in masterDataBand.DataBandInfoV1.FooterComponents) comp.Prepare();

            foreach (StiGroupHeaderBand comp in masterDataBand.DataBandInfoV1.GroupHeaderComponents)
            {
                comp.Line = 0;
                comp.Prepare();
            }

            foreach (StiComponent comp in masterDataBand.DataBandInfoV1.GroupFooterComponents)
                if (comp.Parent != null)
                    comp.Prepare();
            #endregion

            #region Clear collection remitted component
            masterDataBand.DataBandInfoV1.RemmitedCollection.Clear();
            #endregion

            //SetDetail();

            //Invoke events only if not column mode
            if (!IsCrossColumn(masterDataBand)) masterDataBand.InvokeBeginRender();

            //Required for two pass reports
            masterDataBand.DataBandInfoV1.LastPositionRendering = -1;
        }

        /// <summary>
        /// Очищает компонент после рендеринга.
        /// </summary>
        public override void UnPrepare(StiComponent masterComp)
        {
            base.UnPrepare(masterComp);

            StiDataBand masterDataBand = masterComp as StiDataBand;

            masterDataBand.DataBandInfoV1.HeaderComponents = null;
            masterDataBand.DataBandInfoV1.FooterComponents = null;
            masterDataBand.DataBandInfoV1.DetailComponents = null;
            masterDataBand.DataBandInfoV1.DetailDataComponents = null;
            masterDataBand.DataBandInfoV1.SubReportsComponents = null;
            masterDataBand.DataBandInfoV1.GroupHeaderComponents = null;
            masterDataBand.DataBandInfoV1.GroupFooterComponents = null;
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
            var masterDataBand = masterComp as StiDataBand;

            if (masterDataBand.Columns > 1 && masterDataBand.ColumnDirection == StiColumnDirection.DownThenAcross) masterDataBand.Report.Column = 1;

            #region Render main or render first column if DownThenAcross
            bool result = false;
            if (masterDataBand.Columns > 1) masterDataBand.DataBandInfoV1.ItemsActive = true;
            result = InternalRender(masterDataBand, ref renderedComponent, outContainer, true);
            masterDataBand.DataBandInfoV1.ItemsActive = false;
            #endregion

            #region Columns DownThenAcross
            if (masterDataBand.Columns > 1 && masterDataBand.ColumnDirection == StiColumnDirection.DownThenAcross)
            {
                masterDataBand.DataBandInfoV1.FreeSpace = masterDataBand.Page.Height * masterDataBand.Columns * 2;
                if (!masterDataBand.IsEof)
                {
                    for (int columnIndex = 0; columnIndex < masterDataBand.Columns; columnIndex++)
                    {
                        masterDataBand.Report.Column = columnIndex + 1;
                        foreach (var item in masterDataBand.DataBandInfoV1.RenderedItems)
                        {
                            var baseCont = item as StiContainer;
                            var newCont = StiContainerV1Builder.GetRenderContainer(outContainer, baseCont);
                            newCont.DockStyle = masterDataBand.RightToLeft ? StiDockStyle.Right : StiDockStyle.Left;
                            newCont.Width = masterDataBand.GetColumnWidth() + masterDataBand.ColumnGaps;

                            if (columnIndex == 0)
                            {
                                var al = new StiComponentsCollection();

                                foreach (StiComponent comp in baseCont.Components) al.Add(comp);

                                newCont.Components.AddRange(al);
                                baseCont.Components.Remove(al);
                                newCont.Components.SetParent(newCont);
                                baseCont.Components.Add(newCont);
                                newCont.Parent = baseCont;
                                newCont.Left = masterDataBand.RightToLeft ? masterDataBand.Width - masterDataBand.GetColumnWidth() : 0;

                                if (masterDataBand.RightToLeft) newCont.OffsetLocation(masterDataBand.ColumnGaps, 0);
                            }
                            else
                            {
                                int a = masterDataBand.Position;

                                bool breaked = false;
                                RenderItem(masterDataBand, out breaked, newCont);
                                baseCont.Components.Add(newCont);
                                if (masterDataBand.RightToLeft && newCont.Components[0] is StiContainer)
                                    (newCont.Components[0] as StiContainer).OffsetLocation(masterDataBand.ColumnGaps, 0);

                                newCont.Parent = baseCont;
                                MoveNext(masterDataBand);
                                if (masterDataBand.IsEof)
                                {
                                    masterDataBand.InvokeEndRender();
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            return result;
        }


        /// <summary>
        /// Renders a component in the specified container with taking events generation into consideration. The rendered component is returned in the renderedComponent argument.
        /// </summary>
        /// <param name="renderedComponent">A component which is being rendered.</param>
        /// <param name="outContainer">A container in which rendering will be done.</param>
        /// <returns>A value which indicates whether rendering of the component is finished or not.</returns>
        public override bool Render(StiComponent masterComp, ref StiComponent renderedComponent, StiContainer outContainer)
        {
            StiDataBand masterDataBand = masterComp as StiDataBand;

            masterDataBand.DataBandInfoV1.ColumnIndex = 1;

            #region Check CrossTabs on DataBand
            masterDataBand.DataBandInfoV1.CrossTabExistOnDataBand = false;
            foreach (StiComponent comp in masterDataBand.Components)
            {
                if (comp is IStiCrossTab)
                {
                    masterDataBand.DataBandInfoV1.CrossTabExistOnDataBand = true;
                    break;
                }
            }
            #endregion

            #region Process StartNewPage for second databand on page
            if (masterDataBand.StartNewPage && (!masterDataBand.Page.PageInfoV1.IsFirstDataBandOnPage) && masterDataBand.Parent is StiPage)
            {
                //Gets free space
                double freeSpace = GetFreeSpaceFromRectangle(masterDataBand.GetDockRegion(outContainer));

                double factor = 100 * freeSpace / masterDataBand.Page.Height;
                if (masterDataBand.StartNewPageIfLessThan > factor || masterDataBand.StartNewPageIfLessThan == 100)
                {
                    return false;
                }
            }
            #endregion

            CheckParentContainerBeforeRendering(masterDataBand, outContainer);

            SetLastDataBand(masterDataBand);

            //Used for databands placed in container which placed on master databands
            if (IsDataBandInMasterBand(masterDataBand)) outContainer.Height = 1000000000;

            bool result = InternalRender(masterDataBand, ref renderedComponent, outContainer);

            CheckParentContainerAfterRendering(masterDataBand, outContainer);

            SetParentGrowToHeightInContainer(masterDataBand, outContainer);

            if (IsDataBandInMasterBand(masterDataBand) &&
                masterDataBand.Parent.Parent is StiDataBand &&
                ((StiDataBand) masterDataBand.Parent.Parent).KeepChildTogether)
                MoveDataBandsToMasterDataband(masterDataBand, outContainer);

            if (masterDataBand.MasterComponent == null) masterDataBand.Page.PageInfoV1.IsFirstDataBandOnPage = false;

            #region Clear all GroupFooters settings
            foreach (StiGroupFooterBand band in masterDataBand.DataBandInfoV1.GroupFooterComponents)
            {
                band.GroupFooterBandInfoV1.PrintAtBottomComponent = null;
            }
            #endregion

            //Clear column container
            masterDataBand.DataBandInfoV1.ParentColumnContainer = null;

            return result;
        }
        #endregion
    }
}