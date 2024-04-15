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
using System.Collections.Generic;
using System.Text;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;

namespace Stimulsoft.Report.Engine
{
    /// <summary>
    /// A class helps to render SubReport and DataBand components which are places in containers.
    /// These containers are placed on bands.
    /// </summary>
    public class StiSubReportsHelper
    {
        #region Methods
        /// <summary>
        /// Returns DataBand which is one of Parent components of the specified container.
        /// </summary>
        public static StiDataBand GetMasterDataBand(StiContainer parent)
        {
            parent = parent.Parent;
            while (true)
            {
                if (parent is StiDataBand) return parent as StiDataBand;
                if (parent is StiPage) return null;
                if (parent == null) return null;
                if (parent is StiChildBand)
                {
                    StiContainer parentTemp = (parent as StiChildBand).GetMaster();
                    if (parentTemp != null)
                    {
                        parent = parentTemp;
                        continue;
                    }
                }

                parent = parent.Parent;
            }
        }

        /// <summary>
        /// Returns Band which is one of Parent components of the specified container.
        /// </summary>
        public static StiBand GetParentBand(StiContainer parent)
        {
            parent = parent.Parent;
            while (true)
            {
                if (parent is StiBand) return parent as StiBand;
                if (parent is StiPage) return null;
                if (parent == null) return null;

                parent = parent.Parent;
            }
        }
        #endregion

        #region Methods.Render
        /// <summary>
        /// Prints SubReport component into the specified container.
        /// </summary>
        /// <param name="containerOfSubReport">A container in what the SubReport should be printed.</param>
        /// <param name="subReport">SubReport that should be printed in the specified container.</param>
        public static void RenderSubReport(StiContainer containerOfSubReport, StiSubReport subReport)
        {
            StiPage subReportPage = subReport.SubReportPage;
            double storedHeight = subReportPage != null ? subReportPage.Height : 0;
            StiEngine storedEngine = subReport.Report.Engine;

            /* Set the MasterDataBand for the SubPage. It is necessary to output the Master-Detail reports when the
             * Detail is the SubReport.*/
			if (subReportPage != null) 
				subReportPage.PageInfoV2.MasterDataBand = GetMasterDataBand(subReport);

            //set init flag for datasources
            foreach (Dictionary.StiDataSource datasource in subReport.Report.Dictionary.DataSources)
            {
                datasource.InitForSubreport = true;
            }

            //fix for CrossData and CrossTab
            if (subReportPage != null)
            {
                subReportPage.UnlimitedBreakable = false;
            }

            try
            {
                StiReport externalReport = subReport.GetExternalSubReport();

                StiFillParametersEventArgs e = new StiFillParametersEventArgs();
                subReport.InvokeFillParameters(subReport, e);
                if (e.Value != null && e.Value.Count > 0)
                {
                    StiReport report = externalReport ?? subReport.Report;
                    foreach (KeyValuePair<string, object> entry in e.Value)
                    {
                        report[entry.Key] = entry.Value;
                    }
                }

                if (externalReport != null)
                {
                    StiContainer renderedContainer = RenderExternalSubReport(subReport, externalReport);
                    containerOfSubReport.Components.AddRange(renderedContainer.Components);
                }
                else
                {
                    StiContainer renderedContainer = RenderInternalSubReport(subReport);
                    containerOfSubReport.Components.AddRange(renderedContainer.Components);
                }

                #region Correct the width of components in a subreport
                CorrectComponentRecursive(containerOfSubReport, containerOfSubReport.Width);
                #endregion

            }
            finally
            {
                subReport.Report.Engine = storedEngine;
                //this.Report.RenderedPages = storedRenderedPages;
				if (subReportPage != null)
				{
					subReportPage.Height = storedHeight;
					subReportPage.PageInfoV2.MasterDataBand = null;
				}

                //reset init flag for datasources
                foreach (Dictionary.StiDataSource datasource in subReport.Report.Dictionary.DataSources)
                {
                    datasource.InitForSubreport = false;
                }
            }
        }

        private static void CorrectComponentRecursive(StiContainer container, double  right)
        {
            lock (((ICollection)container.Components).SyncRoot)
                foreach (StiComponent comp in container.Components)
                {
                    if (comp.Left > right) comp.Left = right;
                    if (comp.Right > right)
                        comp.Width = right - comp.Left;
                    StiContainer cont = comp as StiContainer;
                    if (cont != null)
                    {
                        CorrectComponentRecursive(cont, right - cont.Left);
                    }
                }
        }

        public const double SpecialSubReportHeight = 100000000000;

        /// <summary>
        /// Prints the internal SubReport.
        /// </summary>
        private static StiContainer RenderInternalSubReport(StiSubReport subReport)
        {
            StiPage subReportPage = subReport.SubReportPage;
            
            #region Create a temporary container to output the SubReport
            StiContainer containerForRender = new StiContainer();

            if (subReportPage == null) return containerForRender;

            containerForRender.Width = subReportPage.Width;
            containerForRender.Height = SpecialSubReportHeight;
            #endregion

            var storedBookmark1 = subReportPage.CurrentBookmark;
            var storedBookmark2 = subReportPage.ParentBookmark;
            var storedPointer1 = subReportPage.CurrentPointer;
            var storedPointer2 = subReportPage.ParentPointer;

            subReportPage.CurrentBookmark = subReport.CurrentBookmark;
            subReportPage.ParentBookmark = subReport.ParentBookmark;
            containerForRender.CurrentBookmark = subReport.ParentBookmark;

            subReportPage.CurrentPointer = subReport.CurrentPointer;
            subReportPage.ParentPointer = subReport.ParentPointer;
            containerForRender.CurrentPointer = subReport.ParentPointer;

            #region Create a temporary engine to output the content of the SubReport
            StiEngine containerEngine = new StiEngine(subReport.Report);
            containerEngine.ParserConversionStore = (Hashtable)subReport.Report.Engine.ParserConversionStore.Clone();
            if (subReport.Report.Engine.HashDataSourceReferencesCounter != null)
                containerEngine.HashDataSourceReferencesCounter = (Hashtable)subReport.Report.Engine.HashDataSourceReferencesCounter.Clone();
            containerEngine.RenderingStartTicks = subReport.Report.Engine.RenderingStartTicks;

            containerEngine.TemplatePage = subReportPage;
            containerEngine.TemplateContainer = subReport.SubReportPage;
            containerEngine.ContainerForRender = containerForRender;
            subReport.Report.Engine = containerEngine;

            containerEngine.SetNewPageParameters();
            containerEngine.FreeSpace = SpecialSubReportHeight;
            containerEngine.PositionBottomY = SpecialSubReportHeight;
            containerEngine.NewList();
            //subReportPage.Render();
            StiPageHelper.RenderPage(subReportPage);
            containerEngine.FinalClear();
            containerEngine.ParserConversionStore = null;
            #endregion

            #region Correct the position of components which are output on the bottom of a page
            double dist = containerEngine.PositionBottomY - containerEngine.PositionY;

            lock (((ICollection)containerForRender.Components).SyncRoot)
            foreach (StiComponent comp in containerForRender.Components)
            {
                if (comp.Top >= containerEngine.PositionY && (comp.Top - dist) >= 0)
                    comp.Top -= dist;
            }
            #endregion

			StiPostProcessProviderV2.PostProcessPrimitivesInContainer(containerForRender);

            subReportPage.CurrentBookmark = storedBookmark1;
            subReportPage.ParentBookmark = storedBookmark2;

            subReportPage.CurrentPointer = storedPointer1;
            subReportPage.ParentPointer = storedPointer2;

            return containerForRender;
        }

        //for backward compatibility with 2019.x if RenderExternalSubReportsWithHelpOfUnlimitedHeightPages=false
        public static bool RenderExternalSubReportsWithoutHelpOfUnlimitedHeightPagesOldMode = false;

        /// <summary>
        /// Prints the external SubReport.
        /// </summary>
        private static StiContainer RenderExternalSubReport(StiSubReport subReport, StiReport externalReport)
        {
            StiContainer containerForRender = new StiContainer();

            if (externalReport != null)
            {
                bool renderExternalSubReportsWithHelpOfUnlimitedHeightPages = StiOptions.Engine.RenderExternalSubReportsWithHelpOfUnlimitedHeightPages;

                if (renderExternalSubReportsWithHelpOfUnlimitedHeightPages)
                {
                    foreach (StiPage page in externalReport.Pages)
                    {
                        page.UnlimitedHeight = true;
                        page.UnlimitedBreakable = false;
                        page.Height = SpecialSubReportHeight;
                    }
                }

                if (!externalReport.IsDocument)
                {
                    externalReport.Render(false);
                }

                double specialLimit = SpecialSubReportHeight * 0.9;
                double posX = 0;
                double posY = 0;
                int pageNumber = 0;
                foreach (StiPage page in externalReport.RenderedPages)
                {
                    #region Find components maximum offset
                    double maxPosY = 0;
                    double maxPosBottomY = SpecialSubReportHeight;
                    double maxX = 0;
                    double maxY = 0;
                    foreach (StiComponent comp2 in page.Components)
                    {
                        if (comp2.Top > specialLimit)
                        {
                            maxPosBottomY = Math.Min(comp2.Top, maxPosBottomY);
                        }
                        else
                        {
                            maxPosY = Math.Max(comp2.Bottom, maxPosY);
                        }
                        if (!renderExternalSubReportsWithHelpOfUnlimitedHeightPages)
                        {
                            if (comp2.Right > maxX) maxX = comp2.Right;
                            if (comp2.Bottom > maxY) maxY = comp2.Bottom;
                        }
                    }
                    double dist = maxPosBottomY - maxPosY;
                    #endregion

                    if (renderExternalSubReportsWithHelpOfUnlimitedHeightPages || RenderExternalSubReportsWithoutHelpOfUnlimitedHeightPagesOldMode || page.UnlimitedHeight)
                    {
                        foreach (StiComponent comp2 in page.Components)
                        {
                            comp2.Top += posY;
                            comp2.Left += posX;
                            comp2.GrowToHeight = false; //fix, иначе растягивает на всю высоту сабрепорта
                            comp2.CanShrink = false;    //fix, иначе "схлопывает" пустые контейнеры, т.к. уже сделано MoveToPage

                            if (comp2.Top >= specialLimit)
                                comp2.Top -= dist;
                        }

                        containerForRender.Components.AddRange(page.Components);
                    }
                    else
                    {
                        StiContainer cont = new StiContainer();
                        cont.Name = $"SubPage{pageNumber++}";
                        cont.Width = maxX;
                        cont.Height = maxY;
                        cont.Top = posY;
                        cont.Left = posX;
                        cont.Components.AddRange(page.Components);
                        cont.Page = subReport.Page;
                        cont.TagValue = StiContainerHelper.NotCheckSizeMark;

                        foreach (StiComponent comp2 in page.Components)
                        {
                            comp2.GrowToHeight = false; //fix, иначе растягивает на всю высоту сабрепорта
                            comp2.CanShrink = false;    //fix, иначе "схлопывает" пустые контейнеры, т.к. уже сделано MoveToPage
                            comp2.Page = subReport.Page;

                            if (comp2.Top >= specialLimit)
                                comp2.Top -= dist;
                        }

                        containerForRender.Components.Add(cont);

                        if ((cont.Height < subReport.Height) && (pageNumber < externalReport.RenderedPages.Count))
                        {
                            var newPageCont = new StiNewPageContainer
                            {
                                Top = posY + maxY,
                                Height = subReport.Height
                            };
                            containerForRender.Components.Add(newPageCont);
                            posY += newPageCont.Height;
                        }
                    }

                    page.Components.Clear();

                    if (page.Height > specialLimit)
                    {
                        posY += maxPosY + (SpecialSubReportHeight - maxPosBottomY);
                    }
                    else
                    {
                        if (renderExternalSubReportsWithHelpOfUnlimitedHeightPages || RenderExternalSubReportsWithoutHelpOfUnlimitedHeightPagesOldMode)
                            posY += page.Height;
                        else
                            posY += maxY;
                    }
                }
            }

            return containerForRender;
        }

        /// <summary>
        /// Prints a group of bands from the specified container into the container of destination.
        /// </summary>
        /// <param name="containerOfDataBands">A container in what printing is done.</param>
        /// <param name="container">A container that contains a group of bands.</param>
        public static void RenderDataBandsInContainer(StiContainer containerOfDataBands, StiContainer container)
        {
            RenderDataBandsInContainer(containerOfDataBands, container, false);
        }

        /// <summary>
        /// Prints a group of bands from the specified container into the container of destination.
        /// </summary>
        /// <param name="containerOfDataBands">A container in what printing is done.</param>
        /// <param name="container">A container that contains a group of bands for printing.</param>
        /// <param name="skipStaticBands">If true then static bands will not be rendered.
        /// It is used to render cross-tabs which are placed on static bands.</param>
        public static void RenderDataBandsInContainer(StiContainer containerOfDataBands, StiContainer container,
            bool skipStaticBands)
        {
            containerOfDataBands.CanGrow = true;
            StiEngine storedEngine = container.Report.Engine;
            double storedPositionFromTop = 0;
            double storedPositionFromBottom = 0;
            if (container.Report.Engine.Page != null)
            {
                storedPositionFromTop = container.Report.Engine.Page.PageInfoV2.PositionFromTop;
                storedPositionFromBottom = container.Report.Engine.Page.PageInfoV2.PositionFromBottom;
            }

            try
            {
                double specialHeight = SpecialSubReportHeight;

                bool isCrossEngineMode = false;

                #region Create a temporary container to output groups of bands
                StiContainer containerForRender = new StiContainer();
                containerForRender.Width = container.Width;
                containerForRender.Height = specialHeight;
                #endregion

                #region Create a temporary engine to output groups of bands
                StiEngine containerEngine = new StiEngine(container.Report);
                containerEngine.DenyChangeThread = true;
                containerEngine.ParserConversionStore = (Hashtable)container.Report.Engine.ParserConversionStore.Clone();
                if (container.Report.Engine.HashDataSourceReferencesCounter != null)
                    containerEngine.HashDataSourceReferencesCounter = (Hashtable)container.Report.Engine.HashDataSourceReferencesCounter.Clone();
                if ((container.Report.Engine.HashParentStyles != null) && (container.Report.Engine.HashParentStyles.Count > 0))
                {
                    containerEngine.HashParentStyles = (Hashtable)container.Report.Engine.HashParentStyles.Clone();
                }
                containerEngine.RenderingStartTicks = container.Report.Engine.RenderingStartTicks;

                containerEngine.Page = container.Report.Engine.Page;
                containerEngine.TemplatePage = container.Report.Engine.TemplatePage;
                containerEngine.TemplateContainer = container;
                containerEngine.ContainerForRender = containerForRender;
                container.Report.Engine = containerEngine;

                containerEngine.SetNewPageParameters();
                containerEngine.FreeSpace = specialHeight;
                containerEngine.PositionBottomY = specialHeight;
                containerEngine.NewList(skipStaticBands);
                
                /* Remove all simple components rendered in a container
                 * because they were rendered earlier */
                containerForRender.Components.Clear();
                #endregion

                #region Process GetDockRegion
                //add components with DockStyle, only for GetDockRegion correct processing
                foreach (StiComponent comp1 in containerOfDataBands.Components)
                {
                    if (comp1.DockStyle != StiDockStyle.None)
                        containerForRender.Components.Add(comp1);
                }

                RectangleD rect = containerForRender.GetDockRegion(containerForRender, false);

                foreach (StiComponent comp1 in containerForRender.Components)
                {
                    comp1.Parent = containerOfDataBands;
                }
                containerForRender.Components.Clear();
                #endregion

                //containerEngine.CrossFreeSpace = rect.Width;
                containerEngine.CrossFreeSpace = storedEngine.TemplatePage.Width - (container.Left + rect.Left);

                containerEngine.FreeSpace = rect.Height;
                containerEngine.PositionX = rect.Left;
                containerEngine.PositionY = rect.Top;

                #region Print a group of bands
                foreach (StiComponent component in container.Components)
                {
                    component.ParentBookmark = container.CurrentBookmark;
                    component.ParentPointer = container.CurrentPointer;

                    if (component.ComponentType == StiComponentType.Master)
                    {
                        if (component is StiCrossDataBand) 
                            isCrossEngineMode = true;

                        IStiRenderMaster renderMaster = component as IStiRenderMaster;
                        if (renderMaster != null)
                        {
                            renderMaster.RenderMaster();
                        }
                    }
                }

                containerEngine.FinalClear();
                containerEngine.ParserConversionStore = null;
                containerEngine.HashParentStyles = null;
                #endregion

                if (!isCrossEngineMode)
                {
                    #region Correct a position of components which were output on the bottom of page
                    double dist = containerEngine.PositionBottomY - containerEngine.PositionY;
                    if ((containerOfDataBands.Bottom - containerEngine.PositionY > specialHeight - containerEngine.PositionBottomY) &&
                        (!container.CanShrink))
                    {
                        dist = specialHeight - containerOfDataBands.Height;
                    }

                    foreach (StiComponent comp in containerForRender.Components)
                    {
                        if (comp.Top >= containerEngine.PositionY && (comp.Top - dist) >= 0)
                            comp.Top -= dist;
                    }
                    #endregion
                }
                else
                {
                    #region Find maximal SegmentPerWidth for further setting
                    double maxRight = 0;
                    foreach (StiComponent comp in containerForRender.Components)
                    {
                        maxRight = Math.Max(comp.Right, maxRight);
                    }

                    int segmentPerWidth = 1;
                    do
                    {
                        decimal pageWidth = (decimal)storedEngine.TemplatePage.Width * segmentPerWidth;
                        if ((decimal)(maxRight + containerOfDataBands.Left) <= (decimal)pageWidth) break;
                        segmentPerWidth++;
                    }
                    while (1 == 1);

                    containerOfDataBands.ContainerInfoV2.SetSegmentPerWidth = segmentPerWidth;
                    #endregion
                }

                containerOfDataBands.Components.AddRange(containerForRender.Components);

                if (!isCrossEngineMode)
                {
                    #region Correct width of components in a subreport
                    foreach (StiComponent comp in containerOfDataBands.Components)
                    {
                        if (comp.Right > containerOfDataBands.Width)
                            comp.Width = containerOfDataBands.Width - comp.Left;
                        StiContainer cont = comp as StiContainer;
                        if (cont != null)
                        {
                            foreach (StiComponent comp2 in cont.Components)
                            {
                                if (comp2.Left > containerOfDataBands.Width) comp2.Left = containerOfDataBands.Width;
                                if ((comp2.Right + comp.Left) > containerOfDataBands.Width)
                                    comp2.Width = containerOfDataBands.Width - (comp2.Left + comp.Left);
                            }
                        }
                    }
                    #endregion
                }

            }
            finally
            {
                container.Report.Engine = storedEngine;
                if (container.Report.Engine.Page != null)
                {
                    container.Report.Engine.Page.PageInfoV2.PositionFromTop = storedPositionFromTop;
                    container.Report.Engine.Page.PageInfoV2.PositionFromBottom = storedPositionFromBottom;
                }
            }
        }
        #endregion
    }
}
