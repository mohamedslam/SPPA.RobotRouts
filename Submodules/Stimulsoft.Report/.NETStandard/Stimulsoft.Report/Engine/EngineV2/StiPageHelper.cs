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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Table;
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Stimulsoft.Report.Engine
{
    /// <summary>
    /// A class helps to render a page.
    /// </summary>
    internal class StiPageHelper
    {
        public const string CheckCanShrinkLaterMark = "*CheckCanShrinkLaterMark*";

        /// <summary>
        /// Заполняет: коллекцию DataBand's, которые лежит прямо на странице и не имеют 
        /// MasterComponent (или MasterComponent равен page.MasterDataBand); коллекцию 
        /// DataBand's, которые лежат в контейнерах расположенных прямо на странице; коллекцию 
        /// SubReport's, которые расположены прямо на странице. 
        /// </summary>
        /// <param name="page">A page for searching components.</param>
        /// <param name="dataBandsOnPage">Коллекция DataBand's, которые лежит прямо на странице (для заполнения).</param>
        /// <param name="dataBandsInContainers">Коллекция DataBand's, которые лежат в контейнерах расположенных прямо на странице (для заполнения).</param>
        /// <param name="subReportsOnPage">A collection of SubReports which are placed directly on a page (for filling).</param>
        internal static void CreateListOfDataBands(StiPage page, List<StiDataBand> dataBandsOnPage,
            List<StiDataBand> dataBandsInContainers,
            List<StiSubReport> subReportsOnPage, 
            List<StiCrossTab> crossTabsOnPage)
        {
            string lastGuid = null;
            var comps = page.GetComponents();
            lock (((ICollection) comps).SyncRoot)
            {
                foreach (StiComponent comp in comps)
                {
                    #region DataBand
                    var dataBand = comp as StiDataBand;
                    if (dataBand != null &&
                        (dataBand.MasterComponent == null || dataBand.MasterComponent == page.PageInfoV2.MasterDataBand))
                    {
                        if (dataBand is StiTable && !dataBand.Enabled) continue;

                        #region Is Detailed Business Object
                        var isSubBusinessObject = false;
                        if (!dataBand.IsBusinessObjectEmpty)
                        {
                            if (dataBand.BusinessObject.ParentBusinessObject != null)
                            {
                                StiDataBand parentDataBand = null;
                                if (dataBand.Parent is StiPage && dataBandsOnPage.Count > 0)
                                    parentDataBand = dataBandsOnPage[dataBandsOnPage.Count - 1];

                                else if (dataBandsInContainers.Count > 0)
                                    parentDataBand = dataBandsInContainers[dataBandsInContainers.Count - 1];

                                if (parentDataBand != null)
                                {
                                    if (!parentDataBand.IsBusinessObjectEmpty && parentDataBand.BusinessObjectGuid == dataBand.BusinessObject.ParentBusinessObject.Guid)
                                        isSubBusinessObject = true;

                                    else if (dataBand.BusinessObject.ParentBusinessObject.Guid == lastGuid)
                                        isSubBusinessObject = true;
                                }
                            }

                            lastGuid = dataBand.BusinessObjectGuid;
                        }
                        #endregion

                        if (!isSubBusinessObject)
                        {
                            if (dataBand.Parent is StiPage)
                                dataBandsOnPage.Add(dataBand);

                            else
                            {
                                /* Добавляем DataBand только если он не лежит в другом Band */
                                if (StiSubReportsHelper.GetParentBand(dataBand) == null)
                                    dataBandsInContainers.Add(dataBand);
                            }

                            lastGuid = null;
                        }
                    }
                    #endregion

                    #region SubReports
                    var subReport = comp as StiSubReport;
                    if (subReport != null && subReport.Enabled)
                    {
                        /* Add SubReport only if it is not placed on another Band */
                        if (StiSubReportsHelper.GetParentBand(subReport) == null && !page.PrintOnPreviousPage)
                            subReportsOnPage.Add(subReport);
                    }
                    #endregion

                    #region CrossTabs
                    var crossTab = comp as StiCrossTab;
                    if (crossTab != null && comp.Enabled)
                    {
                        /* Add CrossTab only if it is not placed on another Band */
                        if (StiSubReportsHelper.GetParentBand(comp as StiContainer) == null)
                            crossTabsOnPage.Add(crossTab);
                    }
                    #endregion
                }
            }

            #region Автоматическое добавление DataBand-ов для бизнес-объектов
            var checkAgain = true;
            while (checkAgain)
            {
                checkAgain = false;
                var list = new Hashtable();
                var index = 0;
                while (index < dataBandsOnPage.Count)
                {
                    var dataBand = dataBandsOnPage[index];
                    if (!dataBand.IsBusinessObjectEmpty)
                    {
                        var businessObject = dataBand.BusinessObject;

                        if (businessObject.ParentBusinessObject != null && list[businessObject.ParentBusinessObject] == null
                            && dataBand.MasterComponent == null)
                        {
                            var newDataBand = new StiDataBand
                            {
                                BusinessObjectGuid = businessObject.ParentBusinessObject.Guid,
                                Height = 0,
                                Page = page,
                                Parent = dataBand.Parent,
                                Name = dataBand.Name + "_" + index.ToString()
                            };
                            newDataBand.Prepare();

                            if (!newDataBand.DataBandInfoV2.DetailDataBands.Contains(dataBand))
                                newDataBand.DataBandInfoV2.DetailDataBands.Add(dataBand);

                            dataBandsOnPage[index] = newDataBand;
                            checkAgain = true;
                        }
                        else
                            list[businessObject] = businessObject;
                    }
                    index++;
                }
            }
            #endregion
        }

        /// <summary>
        /// Returns the list of ReportTitleBands. If the TitleBeforeHeader property is true then empty collection is returned, because
        /// all ReportTitleBand in this case a statis bands.
        /// </summary>
        internal static List<StiReportTitleBand> GetReportTitles(StiPage page)
        {
            var reportTitles = new List<StiReportTitleBand>();
            if (page.TitleBeforeHeader) return reportTitles;

            lock (((ICollection) page.Components).SyncRoot)
            {
                foreach (StiComponent comp in page.Components)
                {
                    var titleBand = comp as StiReportTitleBand;
                    if (titleBand != null)
                        reportTitles.Add(titleBand);
                }
            }

            return reportTitles;
        }

        /// <summary>
        /// Returns the list of ReportSummaryBands.
        /// </summary>
        internal static List<StiReportSummaryBand> GetReportSummaries(StiPage page)
        {
            var reportSummaries = new List<StiReportSummaryBand>();
            lock (((ICollection) page.Components).SyncRoot)
            {
                foreach (StiComponent comp in page.Components)
                {
                    var summaryBand = comp as StiReportSummaryBand;
                    if (summaryBand != null)
                        reportSummaries.Add(summaryBand);
                }
            }

            return reportSummaries;
        }

        /// <summary>
        /// Prints all simple components on a page. DataBands and their  и их зависимые бэнды не обрабытываются.
        /// The result of print is output into the specified container.
        /// </summary>
        /// <param name="page">A page for simple components processing.</param>
        /// <param name="outContainer">A container to output the result of printing.</param>
        internal static void RenderSimpleComponents(StiPage page, StiContainer outContainer)
        {
            var columnsCount = 0;
            double fullColumnWidth = 0;

            var panel = page as StiPanel;
            if (panel != null)
            {
                columnsCount = panel.Columns < 2 ? 1 : panel.Columns;
                fullColumnWidth = panel.GetColumnWidth() + panel.ColumnGaps;
            }

            for (var index = 0; index < columnsCount; index++)
            {
                page.Report.Column = index + 1;

                lock (((ICollection) page.Components).SyncRoot)
                {
                    foreach (StiComponent component in page.Components)
                    {
                        if (component.ComponentType == StiComponentType.Simple)
                        {
                            component.ParentBookmark = outContainer.CurrentBookmark;
                            component.ParentPointer = outContainer.CurrentPointer;
                            StiComponent renderedComponent = null;

                            #region Skip CanShrink if Container placed directly on the page and contain only bands
                            Hashtable hashShrink = new Hashtable();
                            if (CheckContainerForBandsAndOtherContainers(component, hashShrink) && hashShrink.Count > 0)
                            {
                                foreach (StiComponent comp in hashShrink.Keys) comp.CanShrink = false;
                                renderedComponent = component.Render();
                                foreach (StiComponent comp in hashShrink.Keys)
                                {
                                    comp.CanShrink = true;
                                    if ((renderedComponent != null) && (renderedComponent.TagValue == null)) renderedComponent.TagValue = CheckCanShrinkLaterMark;
                                }
                                hashShrink.Clear();
                            }
                            else
                            {
                                renderedComponent = component.Render();
                            }
                            #endregion

                            if (renderedComponent == null) continue;

                            if (panel != null && panel.Columns > 1 && !(renderedComponent is StiCrossLinePrimitive))
                            {
                                if (panel.RightToLeft)
                                {
                                    renderedComponent.Left +=
                                        page.Width - fullColumnWidth * (index + 1) + panel.ColumnGaps;
                                }
                                else
                                    renderedComponent.Left += fullColumnWidth * index;

                            }

                            outContainer.Components.Add(renderedComponent);

                            #region StiOptions.Engine.AllowInteractionInChartWithComponents
                            if (StiOptions.Engine.AllowInteractionInChartWithComponents)
                            {
                                var chart = renderedComponent as StiChart;
                                if (chart != null && chart.ChartInfoV2.InteractiveComps != null)
                                {
                                    foreach (var comp in chart.ChartInfoV2.InteractiveComps)
                                    {
                                        comp.Left += renderedComponent.Left;
                                        comp.Top += renderedComponent.Top;
                                        outContainer.Components.Add(comp);
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
            }

			StiContainerHelper.CheckSize(page.Report.Engine.ContainerForRender);

            var compsHash = new Hashtable();
            var comps = outContainer.GetComponents();
            lock (((ICollection) page.Components).SyncRoot)
            {
                foreach (StiComponent comp in comps)
                {
                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var list = compsHash[cont.Name] as ArrayList;
                        if (list == null)
                        {
                            list = new ArrayList();
                            compsHash[cont.Name] = list;
                        }

                        list.Add(cont);
                    }
                }
            }

            foreach (ArrayList list in compsHash.Values)
            {
                var index = 1;
                lock (((ICollection) list).SyncRoot)
                {
                    foreach (StiContainer cont in list)
                    {
                        cont.ContainerInfoV2.RenderStep = index;
                        index++;
                    }
                }
            }

            page.Report.Column = 1;

			/* Вызов этого метода необходим для того чтобы 
			 * компоненты использующие свойство Dock правильно
			 * пристыковались к контейнеру */
            outContainer.DockToContainer();
        }

        private static bool CheckContainerForBandsAndOtherContainers(StiComponent component, Hashtable hash)
        {
            var cont = component as StiContainer;
            if (cont != null && cont.Components.Count > 0)
            {
                if (cont.CanShrink) hash[cont] = null;
                foreach (StiComponent comp in cont.Components)
                {
                    if (comp is StiBand) 
                        return true;

                    if (comp == cont)
                        return false;

                    if (CheckContainerForBandsAndOtherContainers(comp, hash)) 
                        return true;

                    break;
                }
            }
            return false;
        }

        internal static void PrepareBookmark(StiPage page)
        {
            page.ParentBookmark = page.Report.Bookmark;
            page.DoBookmark();
        }

        internal static void PreparePointer(StiPage page)
        { 
            page.ParentPointer = page.Report.Pointer;
            page.DoPointer();
        }

        /// <summary>
        /// Prints the specified page.
        /// </summary>
        internal static void RenderPage(StiPage page)
        {
            page.InvokeBeginRender();

			//Reset page number if needed.
            if (page.ResetPageNumber)
            {
                if (page.PageInfoV2 != null && page.PageInfoV2.IndexOfStartRenderedPages != -1)
                    page.Report.Engine.PageNumbers.ResetPageNumber(page.PageInfoV2.IndexOfStartRenderedPages);

                else
                    page.Report.Engine.PageNumbers.ResetPageNumber();
            }
            
            var reportTitles = GetReportTitles(page);
            var reportSummaries = GetReportSummaries(page);

            var dataBandsOnPage = new List<StiDataBand>();
            var dataBandsInContainers = new List<StiDataBand>();
            var subReportsOnPage = new List<StiSubReport>();

            var crossTabsOnPage = new List<StiCrossTab>();
            CreateListOfDataBands(page, dataBandsOnPage, dataBandsInContainers, subReportsOnPage, crossTabsOnPage);

            var masterEngine = page.Report.Engine;

            #region Create List of Slaves Engines
            lock (((ICollection) dataBandsInContainers).SyncRoot)
            {
                foreach (var dataBand in dataBandsInContainers)
                {
                    var containerEngine =
                        masterEngine.Threads.CreateContainerEngine(
                            dataBand.Parent.Name, page.Report, masterEngine, page.PageInfoV2.IndexOfStartRenderedPages);
                    page.Report.Engine.SlaveEngines[dataBand.Parent.Name] = containerEngine;

                    containerEngine.PrintOnAllPagesIgnoreList = masterEngine.PrintOnAllPagesIgnoreList;
                }
            }

            lock (((ICollection) subReportsOnPage).SyncRoot)
            {
                foreach (var subReport in subReportsOnPage)
                {
                    var containerEngine =
                        masterEngine.Threads.CreateContainerEngine(
                            subReport.Name, page.Report, masterEngine, page.PageInfoV2.IndexOfStartRenderedPages);
                    page.Report.Engine.SlaveEngines[subReport.Name] = containerEngine;
                }
            }

            lock (((ICollection) crossTabsOnPage).SyncRoot)
            {
                foreach (var crossTab in crossTabsOnPage)
                {
                    var containerEngine =
                        masterEngine.Threads.CreateContainerEngine(
                            crossTab.Name, page.Report, masterEngine, page.PageInfoV2.IndexOfStartRenderedPages);
                    page.Report.Engine.SlaveEngines[crossTab.Name] = containerEngine;
                }
            }
            #endregion

            #region Render DataBand's which placed directly on page
            if (dataBandsOnPage.Count > 0)
            {
                var index = 1;
                lock (((ICollection) dataBandsOnPage).SyncRoot)
                {
                    foreach (var dataBand in dataBandsOnPage)
                    {
                        if (dataBand is StiTable && !dataBand.Enabled) continue;
                        try
                        {
                            if (index == 1)
                            {
                                page.Report.Engine.IsFirstDataBandOnPage = true;
                                dataBand.DataBandInfoV2.ReportTitles = reportTitles;
                            }

                            if (index == dataBandsOnPage.Count)
                            {
                                page.Report.Engine.IsLastDataBandOnPage = true;
                                dataBand.DataBandInfoV2.ReportSummaries = reportSummaries;
                            }

                            dataBand.ParentBookmark = (page.Report.Engine.Page != null) ? page.Report.Engine.Page.CurrentBookmark : page.CurrentBookmark;
                            dataBand.ParentPointer = (page.Report.Engine.Page != null) ? page.Report.Engine.Page.CurrentPointer : page.CurrentPointer;
                            dataBand.RenderMaster();
                        }
                        finally
                        {
                            dataBand.DataBandInfoV2.ReportTitles = null;
                            dataBand.DataBandInfoV2.ReportSummaries = null;

                            page.Report.Engine.IsFirstDataBandOnPage = false;
                            page.Report.Engine.IsLastDataBandOnPage = false;
                        }

                        index++;
                    }
                }
            }
            #endregion

            #region Render Report Titles & Summaries
            /* If there are no DataBands then Report Titles or Report Summaries
             * are present then render them. */
            else
            {
                if (reportTitles != null)
                {
                    lock (((ICollection) reportTitles).SyncRoot)
                    {
                        foreach (var title in reportTitles)
                        {
                            if (title.PrintIfEmpty)
                            {
                                title.ParentBookmark = (page.Report.Engine.Page != null) ? page.Report.Engine.Page.CurrentBookmark : page.CurrentBookmark;
                                bool isNewGuidCreated = title.DoBookmark();

                                title.ParentPointer = (page.Report.Engine.Page != null) ? page.Report.Engine.Page.CurrentPointer : page.CurrentPointer;
                                title.DoPointer(!isNewGuidCreated);

                                page.Report.Engine.RenderBand(title);
                            }
                        }
                    }
                }

                if (reportSummaries != null)
                {
                    lock (((ICollection) reportSummaries).SyncRoot)
                    {
                        foreach (var summary in reportSummaries)
                        {
                            if (summary.PrintIfEmpty)
                            {
                                summary.ParentBookmark = (page.Report.Engine.Page != null) ? page.Report.Engine.Page.CurrentBookmark : page.CurrentBookmark;
                                bool isNewGuidCreated = summary.DoBookmark();

                                summary.ParentPointer = (page.Report.Engine.Page != null) ? page.Report.Engine.Page.CurrentPointer : page.CurrentPointer;
                                summary.DoPointer(!isNewGuidCreated);

                                page.Report.Engine.RenderBand(summary);
                            }
                        }
                    }
                }
            }
            #endregion

            #region Render DataBand's which placed in containers
            try
            {
                foreach (var dataBand in dataBandsInContainers)
                {
                    var containerEngine = masterEngine.SlaveEngines[dataBand.Parent.Name] as StiEngine;
                    if (containerEngine.FreeSpace == 0)
                        containerEngine.FreeSpace = dataBand.Parent.Height;

                    if (dataBand.Parent != null && dataBand.Parent.CanGrow && dataBand.Parent.Parent is StiPage)
                    {
                        if ((containerEngine.FreeSpace < StiEngine.SpecialContainerHeight / 2) && (masterEngine.FreeSpace < StiEngine.SpecialContainerHeight / 2))
                        {
                            containerEngine.FreeSpace = masterEngine.PositionBottomY - dataBand.Parent.Top;
                            if (containerEngine.ContainerForRender != null)
                                containerEngine.ContainerForRender.Height = containerEngine.FreeSpace;

                            if (page.UnlimitedHeight)
                            {
                                containerEngine.FreeSpace = StiEngine.SpecialContainerHeight2;
                            }
                        }
                    }

                    page.Report.Engine = containerEngine;

                    if (containerEngine.ContainerForRender != null)
                    {
                        dataBand.ParentBookmark = containerEngine.ContainerForRender.CurrentBookmark;
                        dataBand.ParentPointer = containerEngine.ContainerForRender.CurrentPointer;
                    }

                    dataBand.RenderMaster();
                }
            }
            finally
            {
                page.Report.Engine = masterEngine;
            }
            #endregion

            #region Render SubReport's which placed in containers or on page
            try
            {
                foreach (var subReport in subReportsOnPage)
                {
                    var containerEngine = masterEngine.SlaveEngines[subReport.Name] as StiEngine;

                    page.Report.Engine = containerEngine;
                    var tempContainer = new StiContainer(subReport.ClientRectangle)
                    {
                        Name = "Temp Container",
                        CanBreak = true
                    };

                    StiSubReportsHelper.RenderSubReport(tempContainer, subReport);

                    double maxHeight = 0;
                    foreach (StiComponent comp in tempContainer.Components)
                    {
                        maxHeight = Math.Max(comp.Bottom, maxHeight);
                    }
                    tempContainer.Height = maxHeight;

                    if (containerEngine.ContainerForRender != null)
                        containerEngine.RenderContainer(tempContainer);
                }
            }
            finally
            {
                page.Report.Engine = masterEngine;
            }
            #endregion

            #region Render CrossTabs which are placed in containers or on a page
            try
            {
                foreach (var crossTab in crossTabsOnPage)
                {
                    var containerEngine = masterEngine.SlaveEngines[crossTab.Name] as StiEngine;

                    page.Report.Engine = containerEngine;

                    var pars = new StiCrossTabParams();
					StiCrossTabHelper.CreateCross(crossTab);

                    while (!pars.RenderingIsFinished && containerEngine.ContainerForRender != null)
                    {
                        #region Корректируем ширину компонента
                        if (crossTab.HorAlignment == StiCrossHorAlignment.None)                            
                            containerEngine.ContainerForRender.Width = containerEngine.ContainerForRender.Parent.Width - containerEngine.ContainerForRender.Left;

                        else
                            containerEngine.ContainerForRender.Width = containerEngine.ContainerForRender.Parent.Width;
                        #endregion

						#region Special check for container which contain Cross-Tab and have CanShrink property setted to true
						var doCanShrink = false;
                        if (containerEngine.ContainerForRender.Parent.CanShrink && containerEngine.ContainerForRender.Parent.Height == 0)
                        {
                            var parent = page.GetComponents()[containerEngine.ContainerForRender.Parent.Name];
                            doCanShrink = true;
                            containerEngine.ContainerForRender.Parent.Width = parent.Width;
                            containerEngine.ContainerForRender.Parent.Height = parent.Height;

                            containerEngine.ContainerForRender.Width = parent.Width;
                            containerEngine.ContainerForRender.Height = parent.Height;
                        }
                        else
                            containerEngine.ContainerForRender.Height = containerEngine.ContainerForRender.Parent.Height - containerEngine.ContainerForRender.Top;
                        #endregion

                        pars.DestinationRectangle = containerEngine.ContainerForRender.ClientRectangle.ToRectangleM();
                        
                        /*Render a CrossTab into the current container containerEngine.*/
                        pars.DestinationContainer = containerEngine.ContainerForRender;
						var builder = StiV2Builder.GetBuilder(typeof(StiCrossTab)) as StiCrossTabV2Builder;
                        builder.RenderCrossTab(pars, crossTab);

						#region If we switch off size checking before cross-tab rendering then we need recheck size of panel
						if (doCanShrink)
						{
							var size = containerEngine.ContainerForRender.Parent.GetActualSize();
							containerEngine.ContainerForRender.Parent.Width = size.Width;
							containerEngine.ContainerForRender.Parent.Height = size.Height;
						}
						#endregion

                        if (!pars.RenderingIsFinished)
                        {
                            if (containerEngine.ContainerForRender.Parent.Height > StiEngine.SpecialContainerHeight2)
                            {
                                var newPageCont = new StiNewPageContainer
                                {
                                    Top = containerEngine.PositionY,
                                    Height = containerEngine.TemplatePage.Height < StiEngine.SpecialContainerHeight2 ? containerEngine.TemplatePage.Height : containerEngine.TemplatePage.Unit.ConvertFromHInches(1170d)
                                };
                                containerEngine.PositionY += newPageCont.Height;
                                containerEngine.FreeSpace -= newPageCont.Height;
                                containerEngine.OffsetNewColumnY = containerEngine.PositionY - containerEngine.StaticBands.ReservedPositionY;
                                containerEngine.ContainerForRender.Parent.Components.Add(newPageCont);

                                var newCont = containerEngine.ContainerForRender.Clone(true, false) as StiContainer;
                                newCont.Top = containerEngine.PositionY;
                                containerEngine.ContainerForRender.Parent.Components.Add(newCont);
                                containerEngine.ContainerForRender = newCont;
                            }
                            else
                            {
                                containerEngine.NewDestination();
                            }
                        }
                    }

                    StiCrossTabHelper.ClearCross(crossTab);
                }
            }
            finally
            {
                page.Report.Engine = masterEngine;
            }
            #endregion

            page.InvokeEndRender();
            page.InvokeColumnEndRender();

            page.IsRendered = true;

            #region Final Clear
            lock (masterEngine.SlaveEngines.Values.SyncRoot)
            {
                foreach (StiEngine engine in masterEngine.SlaveEngines.Values)
                {
                    //page.Report.Engine = engine;
                    engine.FinalClear();
                }
            }

            page.Report.Engine.SlaveEngines.Clear();
            #endregion
        }

		public static void RenderOverlays(StiPage masterPage, StiPage renderedPage)
		{
			var overlays = masterPage.PageInfoV2.Overlays;
		    if (overlays == null) return;

		    var overlayTop = new List<StiComponent>();
		    var overlayCenter = new List<StiComponent>();
		    var overlayBottom = new List<StiComponent>();

		    foreach (StiOverlayBand overlay in overlays)
		    {
		        //overlay.Render(ref renderedComponent, outContainer);					
		        var renderedComponent = overlay.Render();
		        if (renderedComponent == null) continue;

		        renderedComponent.DockStyle = StiDockStyle.None;
		        renderedPage.Components.Add(renderedComponent);

		        switch (overlay.VertAlignment)
		        {
		            case StiVertAlignment.Top:
		                overlayTop.Add(renderedComponent);
		                break;

		            case StiVertAlignment.Center:
		                overlayCenter.Add(renderedComponent);
		                break;

		            case StiVertAlignment.Bottom:
		                overlayBottom.Add(renderedComponent);
		                break;
		        }
		    }

		    var top = -masterPage.Margins.Top;
		    lock (((ICollection) overlayTop).SyncRoot)
		    {
		        foreach (var comp in overlayTop)
		        {
		            comp.Top = top;
		            top += comp.Height;
		        }
		    }

		    var bottom = masterPage.Height + masterPage.Margins.Bottom;
		    lock (((ICollection) overlayBottom).SyncRoot)
		    {
		        foreach (var comp in overlayBottom)
		        {
		            comp.Top = bottom - comp.Height;
		            bottom -= comp.Height;
		        }
		    }

		    double height = 0;
		    lock (((ICollection) overlayCenter).SyncRoot)
		    {
		        foreach (var comp in overlayCenter)
		        {
		            height += comp.Height;
		        }
		    }

		    top = (masterPage.Height - height) / 2;
		    lock (((ICollection) overlayCenter).SyncRoot)
		    {
		        foreach (var comp in overlayCenter)
		        {
		            comp.Top = top;
		            top += comp.Height;
		        }
		    }
		}

        /// <summary>
        /// Creates a new page on base of the page of the template.
        /// </summary>
        internal static StiPage GetPageFromTemplate(StiPage templatePage)
        {	
			var page = templatePage.Clone(false, false) as StiPage;
            var watermark = page.Watermark;

            #region Parse Watermark
            if (watermark != null)
            {
                if (StiOptions.Engine.Watermark.AllowExpression)
                    watermark.Text = StiExpressionHelper.ParseText(page, watermark.Text);

                if (!string.IsNullOrEmpty(watermark.EnabledExpression))
                    watermark.Enabled = StiExpressionHelper.ParseBool(page, watermark.EnabledExpression);
            }
            #endregion

            if (templatePage.PageInfoV2.RenderedCount > 0)
                page.Guid = StiGuidUtils.NewGuid();

            page.InvokeEvents();
			RenderOverlays(templatePage, page);
            templatePage.PageInfoV2.RenderedCount++;			
            
            return page;
        }
    }
}
