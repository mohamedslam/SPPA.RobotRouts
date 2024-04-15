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
using Stimulsoft.Base.Plans;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Table;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Stimulsoft.Report.Engine
{
    /// <summary>
    /// Report engine core.
    /// </summary>
    public class StiEngine
    {
        #region Consts
        public const double SpecialContainerHeight = 100000000000;
        public const double SpecialContainerHeight2 = 100000000000 - 100;
        #endregion

        #region Properties
        /// <summary>
        /// Saved pointer from the first pass of the report rendering. This pointer is requied for rendering the table of contents component.
        /// </summary>
        internal StiBookmark FirstPassPointer { get; set; }

        /// <summary>
        /// If true then it is allowed to add any Bookmarks. If false then 
        /// add Bookmarks of components which the IsRendered property = false 
        /// (in other words it is rendered first time). This property allows adding Bookmarks
        /// for static components only once (to avoid duplication).
        /// </summary>
        public bool IsDynamicBookmarksMode { get; set; }

        /// <summary>
        /// This property is set to true for printing CrossBands.
        /// </summary>
        public bool IsCrossBandsMode { get; set; }

        /// <summary>
        /// Returns true if the first DataBand is printed on the current page.
        /// </summary>
        public bool IsFirstDataBandOnPage { get; set; }

        /// <summary>
        /// Returns true if the last DataBand is printed on the current page.
        /// </summary>
        public bool IsLastDataBandOnPage { get; set; }

        /// <summary>
        /// Contains the list of bands which should be passed when rendering.
        /// </summary>
        internal Hashtable PrintOnAllPagesIgnoreList { get; set; } = new Hashtable();

        /// <summary>
        /// A class helps to output the progress bar when report rendering.
        /// </summary>
        internal StiProgressHelperV2 ProgressHelper { get; }

        /// <summary>
        /// Contains a collection of bands which should be output on all pages.
        /// </summary>
        internal StiBandsOnAllPages BandsOnAllPages { get; }

        /// <summary>
        /// If true then the Render method of a container will not render components of the 
        /// Master type. This property is used with the RenderSimpleComponents method of a page. 
        /// It is used to deny master components and render simple ones.
        /// </summary>
        internal bool DenyRenderMasterComponentsInContainer { get; set; }

        /// <summary>
        /// Contains a collection of bands which should be output on the bottom of a page.
        /// The list is filled when stream rendering and the list is cleared when rendering 
        /// of the current stream is finished.
        /// </summary>
        internal StiPrintAtBottom PrintAtBottom { get; }

        /// <summary>
        /// Cotnains a collection of Footers which should be output on the bottom of a page.
        /// The list is filled when stream rendering and the list is cleared when rendering 
        /// of the current stream is finished.
        /// </summary>
        internal StiFootersOnAllPages FootersOnAllPages { get; }

        /// <summary>
        /// An object helps to output static bands on a page.
        /// </summary>
        internal StiStaticBandsHelper StaticBands { get; }

        /// <summary>
        /// This object helps to print groups of bands from containers which are placed directly on a page.
        /// </summary>
        internal StiThreads Threads { get; }

        /// <summary>
        /// An object helps to work with IStiBreakable interface.
        /// </summary>
        internal StiBreakableHelper Breakable { get; }

        /// <summary>
        /// If true then it is impossible to change stream of printing.
        /// </summary>
        internal bool DenyChangeThread { get; set; }

        /// <summary>
        /// Contains a list of Slave Engines.
        /// </summary>
        internal Hashtable SlaveEngines { get; } = new Hashtable();

        /// <summary>
        /// If an engine is slave then this reference indicates the parent report engine. 
        /// </summary>
        internal StiEngine MasterEngine { get; set; }

        /// <summary>
        /// Used to output EmptyBands in the current container.
        /// </summary>
        internal StiEmptyBandsV2Helper EmptyBands { get; }

        internal StiPageNumberHelper PageNumbers { get; }

        /// <summary>
        /// Used to output columns on the DataBand.
        /// </summary>
        internal StiColumnsOnDataBand ColumnsOnDataBand { get; }

        /// <summary>
        /// Used to output columns on the Panel.
        /// </summary>
        internal StiColumnsOnPanel ColumnsOnPanel { get; }

        /// <summary>
        /// Contains a freespace in a container in what the printing is done.
        /// </summary>
        public double FreeSpace { get; set; }

        /// <summary>
        /// Contains a freespace in a container in what the printing is done. Used to output 
        /// Cross bands only.
        /// </summary>
        public double CrossFreeSpace { get; set; }

        /// <summary>
        /// Indicates the current position bands output on the X axis.
        /// </summary>
        public double PositionX { get; set; }

        /// <summary>
        /// Indicates the current position bands output on the Y axis.
        /// </summary>
        public double PositionY { get; set; }

        /// <summary>
        /// Indicates the current position bands output on the Y axis on the bottom of a page.
        /// </summary>
        public double PositionBottomY { get; set; }

        /// <summary>
        /// Gets or sets a container in what rendering of bands is done.
        /// </summary>
        public StiContainer ContainerForRender { get; set; }

        /// <summary>
        /// Gets or sets a page in what rendering of bands is done.
        /// </summary>
        public StiPage Page { get; set; }

        /// <summary>
        /// Gets or sets a page from a template. This page is being rendered in the current moment.
        /// </summary>
        public StiPage TemplatePage { get; set; }

        /// <summary>
        /// Gets or sets a container from a template. This page is being rendered in the current moment. If a page is output then 
        /// the TemplateContainer property is equal in TemplatePage.
        /// </summary>
        public StiContainer TemplateContainer { get; set; }

        /// <summary>
        /// Gets or sets a report that is being rendered in the current moment.
        /// </summary>
        public StiReport Report { get; set; }

        /// <summary>
        /// Gets or sets a master report that is being rendered in the current moment.
        /// </summary>
        public StiReport MasterReport { get; set; }

        /// <summary>
        /// If true then UnlimitedHeight property is ignored when NewPage method is called.
        /// </summary>
        public bool IgnoreUnlimitedHeightForNewPage { get; set; }

        /// <summary>
        /// Contains a list of stored bands for keepFirstDetailTogether property
        /// </summary>
        internal Hashtable KeepFirstDetailTogetherList { get; } = new Hashtable();

        /// <summary>
        /// Contains a list of bands created from the tables.
        /// </summary>
        internal Hashtable KeepFirstDetailTogetherTablesList { get; } = new Hashtable();

        /// <summary>
        /// Contain start index of page for page total calculation. Used for running total
        /// </summary>
        internal int StartIndexPageForPageTotal { get; set; } = -1;

        /// <summary>
        /// Contain index of page for page total calculation.
        /// </summary>
        internal int IndexPageForPageTotal { get; set; } = -1;

        /// <summary>
        /// If a variable is true then output as a band is not done.
        /// </summary>
        internal bool SilentMode { get; set; }

        /// <summary>
        /// Contains an information that is necessary to show progress of report rendering.
        /// </summary>
        internal StiRenderState RenderState { get; set; }

        /// <summary>
        /// An index of the last rendered DataBand. The Index is used to 
        /// put the AddLevel before this DataBand and make KeepFooterTogether properties work correct.
        /// </summary>
        internal StiIndex IndexOfLatestDataBand { get; set; }

        /// <summary>
        /// It is required to generate a new page before the next band. 
        /// A flag is used to prevent empty pages after a band. 
        /// A new page is not generated after the specified band but not before the next band.
        /// If there is no the next band then there is no a new page.
        /// </summary>
        internal bool GenerateNewPageBeforeBand { get; set; }

        /// <summary>
        /// If true then the engine should ignore the value of the SkipFirst property of a band. 
        /// It is necessary if NewPageAfter or NewColumnAfter are processed.
        /// </summary>
        internal bool IgnoreSkipFirst { get; set; }

        /// <summary>
        /// Requires to generate a new column before the next band.
        /// A flag is used to prevent empty columns after a band. 
        /// A new column is not generated after the specified band but not before the next band.
        /// If there is no the next band then there is no a new column.
        /// </summary>
        internal bool GenerateNewColumnBeforeBand { get; set; }

        /// <summary>
        /// Коллекция служит для определения случая пропуска генерации новой страницы или новой колонки при помощи свойства SkipFirst.
        /// Если бэнд уже пропускал один раз генерацию новой страницы или колонки, то он заносится в эту коллекцию.
        /// </summary>
        internal Hashtable PageBreakSkipFirstCollection { get; } = new Hashtable();

        /// <summary>
        /// Contains an index that indicates a position of the beginning the current column output. If there are no columns then contains 0.
        /// </summary>
        internal int IndexOfStartList { get; set; }

        /// <summary>
        /// Флаг служит для игнорирования первого BeforePrintEvent для страницы,
        /// так как это событие вызывается ранее при первом обращении к шаблону страницы
        /// </summary>
        internal bool SkipFirstPageBeforePrintEvent { get; set; }

        /// <summary>
        /// Флаг устанавливается каждый раз для новой страницы шаблона.
        /// Это гарантирует, что страница со включенным свойством UnlimitedHeight начнет рендериться на новой странице
        /// а не добавит сегменты к предыдущей странице
        /// </summary>
        internal bool FirstCallNewPage { get; set; }

        /// <summary>
        /// Флаг устанавливается во время рендеринга StiBandsOnAllPages для того, 
        /// чтобы датабэнды, которые выводятся на каждой странице, не обнуляли этот список
        /// </summary>
        internal bool DenyClearPrintOnAllPagesIgnoreList { get; set; }

        internal Hashtable DuplicatesLastValues { get; set; }

        internal Hashtable AnchorsArguments { get; set; }

        private Hashtable parserConversionStore;
        /// <summary>
        /// Хранилище готовых списков команд для каждого TextExpression
        /// </summary>
        internal Hashtable ParserConversionStore
        {
            get
            {
                return parserConversionStore ?? (parserConversionStore = new Hashtable());
            }
            set
            {
                parserConversionStore = value;
            }
        }

        internal Hashtable HashParentStyles { get; set; }

        private Hashtable hashUseParentStyles;
        /// <summary>
        /// Хранилище контейнеров, у которых есть компоненты с установленным UseParentStyles.
        /// for speed optimization.
        /// </summary>
        internal Hashtable HashUseParentStyles
        {
            get
            {
                if (hashUseParentStyles == null)
                {
                    hashUseParentStyles = new Hashtable();

                    #region Fill hash
                    foreach (StiComponent comp in Report.GetComponents())
                    {
                        var tempCont = comp as StiContainer;
                        if (tempCont == null) continue;

                        foreach (StiComponent comp2 in tempCont.Components)
                        {
                            if (!comp2.UseParentStyles) continue;

                            hashUseParentStyles[comp] = null;
                            break;
                        }
                    }
                    #endregion
                }
                return hashUseParentStyles;
            }
            set
            {
                hashUseParentStyles = value;
            }
        }

        internal object LastInvokeTextProcessValueEventArgsValue { get; set; }

        internal object LastInvokeTextProcessIndexEventArgsValue { get; set; }

        internal bool AtLeastOneDatabandRenderedOnPage { get; set; }

        internal double LastFreeSpaceOnPageAfterNewList { get; set; }

        internal List<StiBand> BandsInProgress { get; } = new List<StiBand>();

        internal bool AllowEndOfPageProcessing { get; set; }

        internal Hashtable HashCheckSize { get; set; }

        internal Hashtable HashDataSourceReferencesCounter { get; set; }

        internal Hashtable HashDataBandLastLine { get; set; } = new Hashtable();

        internal double OffsetNewColumnY { get; set; }

        internal decimal LatestProgressValue { get; set; }

        internal int RenderingStartTicks { get; set; } = 0;
        #endregion

        #region Fields
        private Hashtable printOnAllPagesIgnoreList2 = new Hashtable();

        private Hashtable childsBandHash;

        /// <summary>
        /// Флаг устанавливается в методе ProcessNewContainerBefore для корректной обработки свойства ResetPageNumber во время первого прохода.
        /// </summary>
        private bool needResetPageNumberForNewPage;

        private bool flagRenderColumnsOnDataBandOnNewPage;

        private Hashtable componentPlacementRemakeTable;
        #endregion

        #region Methods.New
        /// <summary>
        /// A method is called for each new page or column. If the first column is output or thre are no columns then  
        /// a method outputs statis bands. Кроме этого он обнуляет указатель на последний выведенный DataBand. Also a method  
        /// prints bands which should be output on all pages.
        /// </summary>
        public void NewList()
        {
            NewList(false);
        }

        /// <summary>
        /// A method is called for each new page or column. Если выводится первая колонка или колонок нет вообще, то 
        /// метод выводит статические бэнды. Кроме этого он обнуляет указатель на последний выведенный DataBand. Также метод 
        /// выводит на печать те бэнды, которые должны быть выведены на всех страницах.
        /// </summary>
        /// <param name="skipStaticBands">If true then static bands will not be rendered.
        /// It is used to render cross-bands which are placed in static bands.</param>
        public void NewList(bool skipStaticBands)
        {
            if (ContainerForRender == null) return;

            /* Render static bands if the first column is output or there are no columns on a page. */
            if (!skipStaticBands && (ColumnsOnPanel.CurrentColumn == 1 || ColumnsOnPanel.Count < 2))
                StaticBands.Render();

            IndexOfLatestDataBand = null;
            BandsOnAllPages.Render();

            //Clear HashCheckSize
            if (HashCheckSize != null)
            {
                if (HashCheckSize.Count > StiOptions.Engine.ReportCache.AmountOfProcessedPagesForStartGCCollect * 1000) //empiric value
                {
                    HashCheckSize.Clear();
                }
            }
        }

        /// <summary>
        /// A method forms a new column. 
        /// </summary>
        public void NewColumn()
        {
            NewColumn(true);
        }

        /// <summary>
        /// A method forms a new column.
        /// </summary>
        /// <param name="ignoreKeepContainers">Если равен true, то при формировании новой колонки, команды удержания контейнеров будут игнорироваться.</param>
        internal void NewColumn(bool ignoreKeepContainers)
        {
            ColumnsOnPanel.CurrentColumn++;

            var currentPage = Page ?? Report.RenderedPages[0];
            if (ColumnsOnPanel.CurrentColumn > ColumnsOnPanel.Count && StiOptions.Engine.ForceNewPageForExtraColumns || IsCrossBandsMode)
            {
                if (!IsCrossBandsMode)
                {
                    currentPage.InvokeColumnEndRender();

                    ColumnsOnPanel.CurrentColumn = 1;
                    PositionX = 0;
                }
                NewPage(ignoreKeepContainers);
            }
            else
            {
                StiContainer oldContainerForRender = ContainerForRender;

                currentPage.InvokeColumnEndRender();
                FinishColumns(oldContainerForRender);
                var selectedContainer = SearchStartOfKeepContainer(oldContainerForRender, IndexOfStartList);
                if (ignoreKeepContainers)
                    selectedContainer = null;

                if (oldContainerForRender != null && selectedContainer != null && oldContainerForRender.Components.IndexOf(selectedContainer) == 0)
                    selectedContainer = null;

                ChangeEngineParamsByKeep(oldContainerForRender, selectedContainer);

                RenderFootersOnAllPages(ContainerForRender, IndexOfStartList, ref selectedContainer);
                RenderPrintAtBottom(ContainerForRender, IndexOfStartList, selectedContainer);
                RenderEmptyBands(oldContainerForRender, selectedContainer);

                SetNewColumnParameters();

                if (ColumnsOnPanel.CurrentColumn > ColumnsOnPanel.Count && !StiOptions.Engine.ForceNewPageForExtraColumns && ContainerForRender == oldContainerForRender)
                {
                    ColumnsOnPanel.CurrentColumn = 1;
                    PositionX = ColumnsOnPanel.RightToLeft ? TemplateContainer.Width - ColumnsOnPanel.GetColumnWidth() : 0;

                    double maxBottom = 0;
                    foreach (StiComponent comp in oldContainerForRender.Components)
                    {
                        if (comp.Bottom > maxBottom)
                            maxBottom = comp.Bottom;
                    }
                    OffsetNewColumnY = maxBottom - StaticBands.ReservedPositionY;
                    PositionY = maxBottom;
                }

                NewList();

                MoveKeepComponentsOnNextContainer(oldContainerForRender, selectedContainer);
                IndexOfStartList = ContainerForRender.Components.Count > 0 ? ContainerForRender.Components.Count - 1 : 0;
                FinishResetPageNumberContainer(oldContainerForRender, true);   //must be false - not final, but it's fix for columns
                FinishContainer(oldContainerForRender);

                currentPage.InvokeColumnBeginRender();
            }
        }

        /// <summary>
        /// A method forms a new page in a report.
        /// </summary>
        public void NewPage()
        {
            NewPage(true);
        }

        /// <summary>
        /// A method forms a new page in a report.
        /// </summary>
        /// <param name="ignoreKeepContainers">If true, then при формировании новой страницы, команды удержания контейнеров будут игнорироваться.</param>
        internal void NewPage(bool ignoreKeepContainers)
        {
            if (!IsCrossBandsMode && ContainerForRender != null && ContainerForRender.Height > SpecialContainerHeight2)
            {
                if (!StiOptions.Engine.ForceNewPageInSubReports)
                {
                    var newPageCont = new StiNewPageContainer
                    {
                        Top = PositionY,
                        Height = TemplatePage.Height < SpecialContainerHeight2
                            ? TemplatePage.Height
                            : TemplatePage.Unit.ConvertFromHInches(1170d)
                    };
                    PositionY += newPageCont.Height;
                    FreeSpace -= newPageCont.Height;
                    ContainerForRender.Components.Add(newPageCont);

                    OffsetNewColumnY = PositionY - StaticBands.ReservedPositionY;

                    return;
                }
            }

            if (Threads.IsActive)
            {
                NewContainer(ignoreKeepContainers);
            }
            else
            {
                #region CrossBands
                if (IsCrossBandsMode && Page != null)
                {
                    decimal pageWidth = (decimal)(Page.PageWidth - Page.Margins.Left - Page.Margins.Right);
                    while ((decimal)PositionX + pageWidth > (decimal)Page.Width)
                    {
                        Page.SegmentPerWidth++;
                    }

                    if (TemplatePage.UnlimitedBreakable)
                    {
                        PositionX += CrossFreeSpace;
                        CrossFreeSpace = TemplatePage.Width;
                    }
                    else
                    {
                        CrossFreeSpace += TemplatePage.Width;
                    }

                    return;
                }
                #endregion

                #region Support the Unlimited Height property
                if (Page != null && TemplatePage.UnlimitedHeight && !FirstCallNewPage
                    && !IgnoreUnlimitedHeightForNewPage && ColumnsOnPanel.Count < 2)
                {
                    while ((int)(decimal)(PositionY / TemplatePage.Height + 1) > Page.SegmentPerHeight)
                    {
                        Page.SegmentPerHeight++;

                        if (!TemplatePage.UnlimitedBreakable)
                            FreeSpace += TemplatePage.Height;
                    }

                    if (TemplatePage.UnlimitedBreakable)
                    {
                        PositionY += FreeSpace > 0
                            ? TemplatePage.Height - PositionY % TemplatePage.Height
                            : FreeSpace;

                        FreeSpace = PositionBottomY;
                        while ((int)(decimal)(PositionY / TemplatePage.Height + 1) > Page.SegmentPerHeight)
                        {
                            Page.SegmentPerHeight++;
                        }
                    }
                    return;
                }
                #endregion

                LastFreeSpaceOnPageAfterNewList = 0;

                FirstCallNewPage = false;

                if (!AtLeastOneDatabandRenderedOnPage && PrintOnAllPagesIgnoreList.Count > 0)
                    PrintOnAllPagesIgnoreList.Clear();

                ColumnsOnPanel.CurrentColumn = 1;

                StiContainer oldContainerForRender = ContainerForRender;

                if (!StiOptions.Engine.FixPageNumberInEvents)
                    TemplatePage.InvokeRendering();

                if (Page != null)
                    ProcessPageAfterRendering(Page, false);

                Page = StiPageHelper.GetPageFromTemplate(TemplatePage);

                #region Increase page number
                PageNumbers.AddPageNumber(Report.CurrentPrintPage, Page.SegmentPerWidth, Page.SegmentPerHeight);
                #endregion

                #region Reset page number if needed.
                if (Page.ResetPageNumber)
                {
                    if (Page.PageInfoV2 != null && Page.PageInfoV2.IndexOfStartRenderedPages != -1)
                        Page.Report.Engine.PageNumbers.ResetPageNumber(Page.PageInfoV2.IndexOfStartRenderedPages);

                    else
                        Page.Report.Engine.PageNumbers.ResetPageNumber();
                }

                if (needResetPageNumberForNewPage)
                    Page.Report.Engine.PageNumbers.ResetPageNumber(Report.CurrentPrintPage);
                #endregion

                if (!StiOptions.Engine.FixPageNumberInEvents)
                    ProcessRendering();

                AddPageToRenderedPages(Page);
                FinishColumns(oldContainerForRender);

                StiPageHelper.PrepareBookmark(Page);
                StiPageHelper.PreparePointer(Page);

                Page.InvokeColumnBeginRender();

                var selectedContainer = SearchStartOfKeepContainer(oldContainerForRender, IndexOfStartList);
                if (ignoreKeepContainers)
                    selectedContainer = null;

                if (oldContainerForRender != null && selectedContainer != null &&
                    oldContainerForRender.Components.IndexOf(selectedContainer) == 0)
                {
                    selectedContainer = null;
                }

                ChangeEngineParamsByKeep(oldContainerForRender, selectedContainer);

                RenderFootersOnAllPages(oldContainerForRender, IndexOfStartList, ref selectedContainer);
                RenderPrintAtBottom(oldContainerForRender, IndexOfStartList, selectedContainer);
                RenderEmptyBands(oldContainerForRender, selectedContainer);

                OffsetNewColumnY = 0;

                SetNewPageParameters();

                ContainerForRender = Page;

                CorrectPrintOnAllPagesIgnoreListBeforeNewList(oldContainerForRender, selectedContainer, false);

                NewList();

                CorrectPrintOnAllPagesIgnoreListBeforeNewList(oldContainerForRender, selectedContainer, true);

                AtLeastOneDatabandRenderedOnPage = false;
                LastFreeSpaceOnPageAfterNewList = FreeSpace;

                MoveKeepComponentsOnNextContainer(oldContainerForRender, selectedContainer);
                IndexOfStartList = 0;

                FinishResetPageNumberContainer(oldContainerForRender, false);
                FinishContainer(oldContainerForRender);
            }
        }

        /// <summary>
        /// A method forms a new container. A method is used when output a group of bands in a container that is placed directly on a page.
        /// </summary>
        /// <param name="ignoreKeepContainers">If true then, when formation a new container, command of keeping containers will be ingnored.</param>
        private void NewContainer(bool ignoreKeepContainers)
        {
            if (!AtLeastOneDatabandRenderedOnPage && PrintOnAllPagesIgnoreList.Count > 0)
                PrintOnAllPagesIgnoreList.Clear();

            ColumnsOnPanel.CurrentColumn = 1;

            var oldContainerForRender = ContainerForRender;

            FinishColumns(oldContainerForRender);

            var selectedContainer = SearchStartOfKeepContainer(oldContainerForRender, IndexOfStartList);
            if (ignoreKeepContainers)
                selectedContainer = null;

            if (oldContainerForRender != null && selectedContainer != null &&
                oldContainerForRender.Components.IndexOf(selectedContainer) == 0)
            {
                selectedContainer = null;
            }

            ChangeEngineParamsByKeep(oldContainerForRender, selectedContainer);

            RenderFootersOnAllPages(oldContainerForRender, IndexOfStartList, ref selectedContainer);
            RenderPrintAtBottom(oldContainerForRender, IndexOfStartList, selectedContainer);
            RenderEmptyBands(oldContainerForRender, selectedContainer);

            Threads.NewPage();
            NewList();

            AtLeastOneDatabandRenderedOnPage = false;

            MoveKeepComponentsOnNextContainer(oldContainerForRender, selectedContainer);
            IndexOfStartList = 0;

            FinishResetPageNumberContainer(oldContainerForRender, false);
            FinishContainer(oldContainerForRender);

            //fix 2016.05.13
            if (ContainerForRender != null && (ContainerForRender.Parent is StiPage) &&
                ContainerForRender.CanGrow && (MasterEngine != null))
            {
                if (FreeSpace < 100000000000 / 2)
                {
                    FreeSpace = MasterEngine.PositionBottomY - ContainerForRender.Top - PositionY;
                    ContainerForRender.Height = FreeSpace;
                }
            }
        }

        /// <summary>
        /// Forms a new page of a new column (if there are some).
        /// </summary>
        public void NewDestination()
        {
            NewDestination(false);
        }

        /// <summary>
        /// Forms a new page of a new column (if there are some).
        /// </summary>
        /// <param name="ignoreKeepContainers">Если равен true, то при формировании новой страницы (или колонки), команды удержания контейнеров будут игнорироваться.</param>
        public void NewDestination(bool ignoreKeepContainers)
        {
            if (ColumnsOnPanel.Count > 1)
                NewColumn(ignoreKeepContainers);

            else
                NewPage(ignoreKeepContainers);
        }
        #endregion

        #region Methods.FooterMarker
        /// <summary>
        /// Adds a FooterMarker (special container) into the current container of output.
        /// A container-marker is used for the engine to know on what place in a container
        /// of output FooterBands for PrintOnAllPages should be replaced after their rendering is complete.
        /// </summary>
        public void AddFooterMarker(StiFooterBand footerMaster)
        {
            var footerMarker = new StiFooterMarkerContainer
            {
                Top = PositionY,
                Left = PositionX,
                Height = 0,
                Name = footerMaster.Name
            };
            footerMarker.ContainerInfoV2.ParentBand = footerMaster;

            AddContainerToDestination(footerMarker);
        }
        #endregion

        #region Methods.Keep
        /// <summary>
        /// Adds a container-marker of the beginning of grouping before the last rendered DataBand.
        /// </summary>
        public void AddKeepLevelAtLatestDataBand()
        {
            if (IndexOfLatestDataBand != null)
            {
                StiContainer container = ContainerForRender;
                int index = IndexOfLatestDataBand.Index;

                #region If it is required to add to columns
                if ((IndexOfLatestDataBand.IndexInColumnContainer != -1) &&
                    (container.Components.Count > IndexOfLatestDataBand.IndexInColumnContainer))
                {
                    container = container.Components[IndexOfLatestDataBand.IndexInColumnContainer] as StiContainer;
                    var columns = container as StiColumnsContainer;

                    //If the direction is AcrossThenDown then correct the index so as do not 
                    //take the last DataBand but to take the last row. For the DownThenAcross range the last 
                    //DataBand is переносится.
                    if (columns.ColumnDirection == StiColumnDirection.AcrossThenDown)
                        index -= columns.GetLengthOfLastRow() - 1;
                }
                #endregion

                if (index < container.Components.Count && index >= 0)
                    container.Components.Insert(index, new StiLevelStartContainer());
            }
        }

        /// <summary>
        /// Adds a container-marker of the beginning of grouping into the current position of output in the stream.
        /// </summary>
        public void AddLevel()
        {
            if (ColumnsOnDataBand.Enabled)
            {
                var columns = ColumnsOnDataBand.GetColumns();
                columns.AddContainer(new StiLevelStartContainer());
            }
            else
            {
                AddContainerToDestination(new StiLevelStartContainer());
            }
        }

        /// <summary>
        /// Adds a container-marker of the end of grouping into the current position of output in the stream.
        /// </summary>
        public void RemoveLevel()
        {
            if (ColumnsOnDataBand.Enabled)
            {
                var columns = ColumnsOnDataBand.GetColumns();
                columns.AddContainer(new StiLevelEndContainer());
            }
            else
            {
                AddContainerToDestination(new StiLevelEndContainer());
            }
        }
        #endregion

        #region Methods
        public void ClearCachedTotals()
        {
            if (Report != null && Report.CachedTotals != null)
            {
                Report.CachedTotals.Clear();
            }
        }

        private StiComponentsCollection GetChildBands(StiBand band)
        {
            if (childsBandHash == null)
                childsBandHash = new Hashtable();

            var childs = childsBandHash[band] as StiComponentsCollection;

            if (childs != null)
                return childs;

            childs = StiBandV2Builder.GetChildBands(band);
            childsBandHash[band] = childs;
            return childs;
        }

        /// <summary>
        /// Clears a collection of bands which once passed the generation of a new page or column.
        /// </summary>
        public void ClearPageBreakSkipFirst()
        {
            PageBreakSkipFirstCollection.Clear();
        }

        /// <summary>
        /// Returns true if the specified container can generate a new page or a column in this time. If the SkipFirst property is false then, in any case
        /// true is returned. If true, то на первый раз метод запрещает генерировать новую колонку или новую страницу и заносит
        /// бэнд в коллекцию.
        /// </summary>
        /// <param name="pageBreak"></param>
        /// <returns></returns>
        internal bool CanGenerateNewContainer(IStiPageBreak pageBreak)
        {
            if (pageBreak == null)
                return true;

            if (!pageBreak.SkipFirst)
                return true;

            if (PageBreakSkipFirstCollection[pageBreak] != null)
                return true;

            PageBreakSkipFirstCollection[pageBreak] = pageBreak;

            return false;
        }

        public void RemoveBandFromPageBreakSkipList(IStiPageBreak pageBreak)
        {
            if (pageBreak == null) return;

            if (PageBreakSkipFirstCollection[pageBreak] != null)
                PageBreakSkipFirstCollection.Remove(pageBreak);
        }

        internal void ProcessPageAfterRendering(StiPage page, bool final)
        {
            #region Process EndOfPage
            if (AllowEndOfPageProcessing)
            {
                var comps = page.GetComponents();
                foreach (StiComponent comp in comps)
                {
                    var text = comp as StiText;
                    if (text != null && text.ProcessAt == StiProcessAt.EndOfPage)
                    {
                        var e = new Stimulsoft.Report.Events.StiGetValueEventArgs();
                        text.InvokeGetValue(comp, e);
                        text.Text.Value = e.Value;
                    }
                }
            }
            #endregion

            var state = RenderState;
            if (MasterEngine != null)
                state = MasterEngine.RenderState;

            StiRenderProviderV2.ProcessPageToCache(Report, page, final);
            if (state != null)
                StiRenderProviderV2.ClearPagesWhichLessThenFromPageAndGreaterThenToPage(Report, state);

            StiRenderProviderV2.ClearPagesForFirstPass(Report);
        }

        internal void ProcessLastPageAfterRendering()
        {
            var page = Report.RenderedPages[Report.RenderedPages.Count - 1];
            ProcessPageAfterRendering(page, false);
        }

        private void ProcessRendering()
        {
            //If report stopped then stop report rendering
            if (Report.IsStopped)
                throw new StiReportRenderingStopException();

            if ((Report.StopBeforeTime > 0) && (Environment.TickCount - RenderingStartTicks > Report.StopBeforeTime * 1000))
                    throw new StiReportRenderingStopException();

            Report.InvokeRendering();

            if (MasterReport != null)
            {
                MasterReport.InvokeRendering();

                //If report stopped then stop report rendering
                if (MasterReport.IsStopped)
                    throw new StiReportRenderingStopException();
            }

            ProgressHelper.Process();
        }

        /// <summary>
        /// This methods processes Runtime variables to avoid incorrect numbers of pages in containers,
        /// which were generated on one page, but rendered on other page (the page was generated).
        /// </summary>
        /// <param name="container"></param>
        private void ReprocessRuntimeVariables(StiContainer container)
        {
            var comps = container.GetComponents();
            lock (((ICollection)comps).SyncRoot)
            {
                foreach (StiComponent comp in comps)
                {
                    if (!(comp is StiSimpleText)) continue;

                    var list = Report.Totals[comp.Name] as ArrayList;
                    if (list == null || list.Count == 0) continue;

                    lock (((ICollection)list).SyncRoot)
                    {
                        foreach (StiRuntimeVariables runtime in list)
                        {
                            if (runtime.TextBox == comp)
                            {
                                runtime.PageIndex = Report.RenderedPages.Count;
                                runtime.CurrentPrintPage = Report.RenderedPages.Count;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// A method is called to finalize operations over the output stream.
        /// </summary>
        public void FinalClear()
        {
            ChangeEngineParamsByKeep(ContainerForRender, null);

            StiContainer tempCont = null;
            RenderFootersOnAllPages(null, 0, ref tempCont);
            RenderPrintAtBottom(null, 0, null);
            RenderEmptyBands(ContainerForRender, null);
            FinishColumns(ContainerForRender);
            FinishResetPageNumberContainer(ContainerForRender, true);
            FinishContainer(null);

            EmptyBands.Clear();

            childsBandHash?.Clear();
            childsBandHash = null;

            DuplicatesLastValues?.Clear();
            DuplicatesLastValues = null;

            IndexOfLatestDataBand = null;

            HashCheckSize?.Clear();
            HashCheckSize = null;

            HashParentStyles?.Clear();
            HashParentStyles = null;

            ClearPageBreakSkipFirst();

            ProgressHelper?.Dispose();
        }

        /// <summary>
        /// Returns true if the PrintAtBottom of the ParentBand of the specified container is true
        /// and the PrintOnAllPages property is true and itis the FooterBand.
        /// </summary>
        private bool IsPrintAtBottomOrFooterOnAllPages(StiContainer container)
        {
            if (container == null)
                return false;

            var printAtBottom = container.ContainerInfoV2.ParentBand as IStiPrintAtBottom;
            if (printAtBottom != null && printAtBottom.PrintAtBottom)
                return true;

            var footer = container.ContainerInfoV2.ParentBand as StiFooterBand;
            return footer != null && footer.PrintOnAllPages;
        }

        /// <summary>
        /// A method changes the PositionY and FreeSpace on the height of components 
        /// which will be moved on a new page.
        /// </summary>
        private void ChangeEngineParamsByKeep(StiContainer containerForRender, StiContainer selectedContainer)
        {
            if (selectedContainer == null) return;

            int startIndex = containerForRender.Components.IndexOf(selectedContainer);

            for (int index = startIndex; index < containerForRender.Components.Count; index++)
            {
                var cont = containerForRender.Components[index];

                /* Change the PositionY only if the current container is not the Footer of 
                 * OnAllPages or PrintAtBottom because, in this case, the PositionY is not changed and 
                 * is not required correction. */
                if (cont is StiContainer && (!IsPrintAtBottomOrFooterOnAllPages(cont as StiContainer)))
                    PositionY -= cont.Height;

                FreeSpace += cont.Height;
            }
        }


        /// <summary>
        /// Sets parameters for the current to putput a new column.
        /// </summary>
        private void SetNewColumnParameters()
        {
            if (IsCrossBandsMode)
            {
                CrossFreeSpace = StaticBands.ReservedCrossFreeSpace;
                PositionX = StaticBands.ReservedPositionX;
                PositionY = StaticBands.ReservedPositionY;
            }
            else
            {
                FreeSpace = StaticBands.ReservedFreeSpace;

                if (ColumnsOnPanel.RightToLeft)
                {
                    PositionX -= ColumnsOnPanel.GetColumnWidth() + ColumnsOnPanel.ColumnGaps;
                }
                else
                {
                    PositionX += ColumnsOnPanel.GetColumnWidth() + ColumnsOnPanel.ColumnGaps;
                }

                PositionY = StaticBands.ReservedPositionY + OffsetNewColumnY;
                PositionBottomY = StaticBands.ReservedPositionBottomY;
            }
        }

        /// <summary>
        /// Sets parameters to output a new page.
        /// </summary>
        internal void SetNewPageParameters()
        {
            if (IsCrossBandsMode)
            {
                CrossFreeSpace = TemplateContainer.Width;
                PositionX = 0;
                PositionY = 0;
            }
            else
            {
                FreeSpace = TemplateContainer.Height;

                if (ColumnsOnPanel.RightToLeft)
                    PositionX = TemplateContainer.Width - ColumnsOnPanel.GetColumnWidth();

                else
                    PositionX = 0;

                PositionY = 0;
                PositionBottomY = TemplateContainer.Height;
            }
            ColumnsOnPanel.CurrentColumn = 1;
        }

        /// <summary>
        /// Generate new column or new page if required before band rendering.
        /// </summary>
        private void ProcessNewContainerBefore(StiBand band)
        {
            needResetPageNumberForNewPage = band.ResetPageNumber;

            if (GenerateNewPageBeforeBand)
            {
                GenerateNewPageBeforeBand = false;
                bool canGenerateNewContainer = CanGenerateNewContainer(band as IStiPageBreak);

                bool isNewPage = false;
                if (IgnoreSkipFirst)
                {
                    IgnoreSkipFirst = false;
                    NewPage();
                    isNewPage = true;
                }
                else if (canGenerateNewContainer)
                {
                    NewPage();
                    isNewPage = true;
                }

                if (isNewPage && (band is StiDataBand) && (((StiDataBand)band).Columns > 1) && (ColumnsOnDataBand.GetColumns() == null))
                    ColumnsOnDataBand.RenderColumns(band as StiDataBand);
            }
            else if (GenerateNewColumnBeforeBand)
            {
                GenerateNewColumnBeforeBand = false;
                var canGenerateNewContainer = CanGenerateNewContainer(band as IStiPageBreak);

                if (IgnoreSkipFirst)
                {
                    IgnoreSkipFirst = false;
                    NewColumn();
                }
                else if (canGenerateNewContainer)
                {
                    NewColumn();
                }
            }
            else
            {
                var pageBreak = band as IStiPageBreak;
                if (pageBreak == null) return;

                if (pageBreak.NewPageBefore || pageBreak.NewColumnBefore)
                {
                    var factor = 100 * FreeSpace / ContainerForRender.Height;
                    if (pageBreak.BreakIfLessThan > factor || pageBreak.BreakIfLessThan == 100)
                    {
                        if (pageBreak.NewPageBefore && CanGenerateNewContainer(band as IStiPageBreak))
                        {
                            NewPage();
                            ProcessNewContainerInDetailBands(band, true, false);
                        }
                        else if (pageBreak.NewColumnBefore && CanGenerateNewContainer(band as IStiPageBreak))
                        {
                            NewColumn();
                            ProcessNewContainerInDetailBands(band, false, true);
                        }
                    }
                }
            }

            needResetPageNumberForNewPage = false;
        }

        /// <summary>
        /// Generate a new column or a new page if required after band rendering.
        /// </summary>
        private void ProcessNewContainerAfter(StiBand band)
        {
            var pageBreak = band as IStiPageBreak;
            if (pageBreak == null) return;

            if (pageBreak.NewPageAfter || pageBreak.NewColumnAfter)
            {
                var factor = 100 * FreeSpace / ContainerForRender.Height;
                if (pageBreak.BreakIfLessThan > factor || pageBreak.BreakIfLessThan == 100)
                {
                    if (pageBreak.NewPageAfter)
                    {
                        GenerateNewPageBeforeBand = true;
                        IgnoreSkipFirst = true;
                    }
                    else if (pageBreak.NewColumnAfter)
                    {
                        GenerateNewColumnBeforeBand = true;
                        IgnoreSkipFirst = true;
                    }
                }
            }
        }

        /// <summary>
        /// This method helps to generate the new page or new column for a child DataBand,
        /// which are located in different containers with their MasterDataBand.
        /// </summary>
        private void ProcessNewContainerInDetailBands(StiBand band, bool newPage, bool newColumn)
        {
            //Check data bands in other containers
            var dataBand = band as StiDataBand;
            if (dataBand == null) return;

            lock (((ICollection)dataBand.DataBandInfoV2.DetailDataBands).SyncRoot)
            {
                foreach (StiBand detailBand in dataBand.DataBandInfoV2.DetailDataBands)
                {
                    if (detailBand.Parent == dataBand.Parent) continue;

                    var masterEngine = MasterEngine ?? this;

                    var slaveEngine = masterEngine.SlaveEngines[detailBand.Parent.Name] as StiEngine;
                    if (slaveEngine == null) continue;

                    if (newPage)
                    {
                        slaveEngine.NewPage();
                    }
                    else if (newColumn)
                    {
                        /* If there are no columns on a page the generate a new page but not the column for the DetailBands*/
                        if (band.Page != null && band.Page.Columns < 2)
                            slaveEngine.NewPage();

                        else
                            slaveEngine.NewColumn();
                    }
                }
            }
        }

        /// <summary>
        /// Returns marked container from which all components should are moved to the next page or column.
        /// If all groups in a container are closed then return null.
        /// </summary>
        private StiContainer SearchStartOfKeepContainer(StiContainer cont, int oldIndexOfStartList)
        {
            if (cont == null)
                return null;

            var index = oldIndexOfStartList;
            var level = -1;
            var selectedStartContIndex = -1;
            StiContainer selectedStartCont = null;

            while (index < cont.Components.Count)
            {
                #region Start Container
                var idStartCont = cont.Components[index] as StiLevelStartContainer;
                if (idStartCont != null)
                {
                    if (level == -1)
                    {
                        selectedStartCont = idStartCont;
                        selectedStartContIndex = index;
                    }
                    level++;
                }
                #endregion

                #region End Container
                var idEndCont = cont.Components[index] as StiLevelEndContainer;
                if (idEndCont != null)
                {
                    level--;
                    if (level < 0)
                    {
                        level = -1;
                        selectedStartCont = null;
                        selectedStartContIndex = -1;
                    }
                }
                #endregion

                if (idStartCont != null || idEndCont != null)
                {
                    cont.Components.RemoveAt(index);
                }
                else index++;
            }

            if (selectedStartCont != null)
            {
                if (selectedStartContIndex >= cont.Components.Count)
                    selectedStartCont = null;
                else
                    selectedStartCont = cont.Components[selectedStartContIndex] as StiContainer;
            }
            return selectedStartCont;
        }


        /// <summary>
        /// Перемещает все компоненты начиная с контейнера - маркера на следующию страницу или колонку.
        /// </summary>
        /// <param name="cont">Контейнер из которого происходит перемещение.</param>
        /// <param name="selectedStartCont">Контейнер - маркер, с которого начинается перемещение.</param>
        private void MoveKeepComponentsOnNextContainer(StiContainer cont, StiContainer selectedStartCont)
        {
            if (cont == null || selectedStartCont == null) return;

            var selectedStartIndex = cont.Components.IndexOf(selectedStartCont);
            if (selectedStartIndex == -1) return;

            var newList = new List<StiContainer>();
            for (int indexCont = selectedStartIndex; indexCont < cont.Components.Count; indexCont++)
            {
                var container = cont.Components[indexCont] as StiContainer;
                if (container == null) continue;

                if (container is StiFooterMarkerContainer) continue;
                if (!container.ContainerInfoV2.IsAutoRendered)
                {
                    newList.Add(container);

                    //If not PrintAtBottom, then moving component up by container height
                    for (int indexCont2 = indexCont + 1; indexCont2 < cont.Components.Count; indexCont2++)
                    {
                        var contt = cont.Components[indexCont2] as StiContainer;
                        var flagColumnHeader = false;
                        if (contt != null)
                        {
                            if (contt.ContainerInfoV2.ParentBand is StiColumnHeaderBand)
                                flagColumnHeader = true;

                            var printAtBottom = contt.ContainerInfoV2.ParentBand as IStiPrintAtBottom;
                            if (printAtBottom != null && printAtBottom.PrintAtBottom) break;
                        }

                        var flag2 = cont.Components[indexCont2].Top > container.Top;
                        if (!flagColumnHeader && flag2)
                            cont.Components[indexCont2].Top -= container.Height;
                    }
                }
            }

            lock (((ICollection)newList).SyncRoot)
            {
                foreach (StiContainer cont2 in newList)
                {
                    Breakable.SetCanBreak(cont2);
                    cont.Components.Remove(cont2);
                    cont2.Parent = null;

                    if (!IsNeedToSkip(cont2.ContainerInfoV2.ParentBand))
                    {
                        RenderContainer(cont2,
                            PrintAtBottom.CanProcess(cont2.ContainerInfoV2.ParentBand),
                            FootersOnAllPages.CanProcess(cont2.ContainerInfoV2.ParentBand));
                    }
                }
            }

            #region Clear the list of ingoring
            //Очистить список игнорирования бэндов выводимых на всех страницах если последний
            //контейнер, перенесенный с предыдущей страницы, является DataBand
            if (ContainerForRender != null)
            {
                int countOfComponents = ContainerForRender.Components.Count;
                if (countOfComponents > 0)
                {
                    var latestContainer = ContainerForRender.Components[countOfComponents - 1] as StiContainer;

                    if (latestContainer != null && latestContainer.ContainerInfoV2.ParentBand is StiDataBand)
                    {
                        PrintOnAllPagesIgnoreList.Clear();
                        AtLeastOneDatabandRenderedOnPage = true;
                    }
                }
            }
            #endregion

        }

        /// <summary>
        /// Добавляет в PrintOnAllPagesIgnoreList бэнды, которые уже отрендерены на предыдущей странице
        /// и будут переноситься на следующую страницу, чтобы эти бэнды не выводились в методе NewList.
        /// После вызова NewList этот метод вызывается повторно, чтобы удалить добавленные ранее бэнды.
        /// </summary>
        /// <param name="cont">Контейнер из которого происходит перемещение.</param>
        /// <param name="selectedStartCont">Контейнер - маркер, с которого начинается перемещение.</param>
        private void CorrectPrintOnAllPagesIgnoreListBeforeNewList(StiContainer cont, StiContainer selectedStartCont, bool clear)
        {
            if (!clear)
            {
                if (cont == null || selectedStartCont == null) return;

                printOnAllPagesIgnoreList2.Clear();

                var selectedStartIndex = cont.Components.IndexOf(selectedStartCont);
                for (var indexCont = selectedStartIndex; indexCont < cont.Components.Count; indexCont++)
                {
                    var container = cont.Components[indexCont] as StiContainer;
                    if (container != null && !container.ContainerInfoV2.IsAutoRendered)
                    {
                        if (BandsOnAllPages.IsBandInBandsList(container.ContainerInfoV2.ParentBand) ||
                            (container.ContainerInfoV2.ParentBand is StiChildBand &&
                            BandsOnAllPages.IsBandInBandsList(((StiChildBand)container.ContainerInfoV2.ParentBand).GetMaster())))
                        {
                            if (!PrintOnAllPagesIgnoreList.ContainsKey(container.ContainerInfoV2.ParentBand))
                                printOnAllPagesIgnoreList2[container.ContainerInfoV2.ParentBand] = container.ContainerInfoV2.ParentBand;

                            PrintOnAllPagesIgnoreList[container.ContainerInfoV2.ParentBand] = container.ContainerInfoV2.ParentBand;
                        }
                    }
                }
            }
            else
            {
                foreach (DictionaryEntry de in printOnAllPagesIgnoreList2)
                {
                    PrintOnAllPagesIgnoreList.Remove(de.Key);
                }
                printOnAllPagesIgnoreList2.Clear();
            }
        }

        /// <summary>
        /// Returns true if the specified band can be output and returns false is the specified band cannot be output. 
        /// Критерием является свойство PrintOnEvenOddPages интерфейса IStiPrintOnEvenOddPages.
        /// </summary>
        private bool IsNeedToPrintOddEven(StiBand band)
        {
            var bandEvenOdd = band as IStiPrintOnEvenOddPages;
            if (bandEvenOdd == null)
                return true;

            if (bandEvenOdd.PrintOnEvenOddPages == StiPrintOnEvenOddPagesType.Ignore)
                return true;

            var pageEvenOdd = Report.PageNumber & 1;

            if (bandEvenOdd.PrintOnEvenOddPages == StiPrintOnEvenOddPagesType.PrintOnEvenPages && pageEvenOdd == 0)
                return true;

            if (bandEvenOdd.PrintOnEvenOddPages == StiPrintOnEvenOddPagesType.PrintOnOddPages && pageEvenOdd == 1)
                return true;

            return false;
        }

        /// <summary>
        /// Returns true, if the specified band should be passed and should not be output.
        /// </summary>
        private bool IsNeedToSkip(StiBand band)
        {
            if (band == null)
                return false;

            if (PrintOnAllPagesIgnoreList[band] != null)
                return true;

            if (band is StiDataBand && ContainerForRender != null)
            {
                var comps = ContainerForRender.Components;
                if (comps.Count > 0)
                {
                    var cont = comps[comps.Count - 1] as StiContainer;
                    if (cont != null && cont.ContainerInfoV2.ParentBand == band && cont.ContainerInfoV2.IsAutoRendered)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Производит поиск и вывод внизу страницы, всех Footers из указанного контейнера вывода.
        /// Выводятся только те элементы, которые будут найдены до контейнера - маркера.
        /// Остальные элементы будут перенесены позднее на следующию страницу. 
        /// Все выводимые элементы должны быть найдены в составленном ранее списке выводимых на всех 
        /// страницах Footers. При обработке, контейнерам устанавливается новая позиция по вертикали 
        /// внизу страницы.
        /// </summary>
        /// <param name="outContainer">Контейнер, в который нужно вывести Footers.</param>
        /// <param name="startIndex">Индекс, начиная с которого, необходимо произвести поиск 
        /// контейнера-маркера. Индекс указывается для того, чтобы не производить повторную обработку 
        /// выведенных ранее колонок на странице.</param>
        /// <param name="markerContainer">Контейнер - маркер, после которого все контейнеры 
        /// будут перенесены на следующию страницу.</param>
        internal void RenderFootersOnAllPages(StiContainer outContainer, int startIndex, ref StiContainer markerContainer)
        {
            if (outContainer != null)
                FootersOnAllPages.Render(outContainer, startIndex, ref markerContainer);

            else
                FootersOnAllPages.Render(ContainerForRender, startIndex, ref markerContainer);
        }

        /// <summary>
        /// Render selected EmptyBand in the specified container.
        /// </summary>
        internal void RenderEmptyBands(StiContainer containerForRender, StiContainer selectedContainer)
        {
            if (containerForRender != null)
                EmptyBands.Render(containerForRender, selectedContainer);
        }

        /// <summary>
        /// Производит поиск и вывод на новой странице, всех элементов из указанного контейнера вывода.
        /// Выводятся только те элементы, которые будут найдены до контейнера - маркера.
        /// Остальные элементы будут перенесены позднее на следующию страницу. 
        /// Все выводимые элементы должны быть найдены в составленном ранее списке выводимых внизу 
        /// страницы бэндов. При обработке, контейнерам устанавливается новая позиция по вертикали 
        /// внизу страницы.
        /// </summary>
        /// <param name="startIndex">Индекс, начиная с которого, необходимо произвести поиск 
        /// контейнера-маркера. Индекс указывается для того, чтобы не производить повторную обработку 
        /// выведенных ранее колонок на странице.</param>
        /// <param name="markerContainer">Контейнер - маркер, после которого все контейнеры 
        /// будут перенесены на следующию страницу.</param>
        internal void RenderPrintAtBottom(StiContainer container, int startIndex, StiContainer markerContainer)
        {
            PrintAtBottom.Render(container ?? ContainerForRender, startIndex, markerContainer);
        }

        /// <summary>
        /// Find all containers and zero the ParentBand property.
        /// </summary>
        /// <param name="containerForRender"></param>
        public void FinishContainer(StiContainer containerForRender)
        {
            if (containerForRender == null) return;

            lock (((ICollection)containerForRender.Components).SyncRoot)
            {
                foreach (StiComponent component in containerForRender.Components)
                {
                    var container = component as StiContainer;
                    if (container == null) continue;

                    if (!((container.ComponentType == StiComponentType.Static) && (container.Page != null) && (container.Page.Columns > 1)))
                        container.ContainerInfoV2.ParentBand = null;
                }
            }
        }

        /// <summary>
        /// Метод проверяет свойство ResetPageNumber и если необходимо сбрасывает номер страницы.
        /// </summary>
        /// <param name="containerForRender"></param>
        public void FinishResetPageNumberContainer(StiContainer containerForRender, bool isFinal)
        {
            if (containerForRender == null) return;

            lock (((ICollection)containerForRender.Components).SyncRoot)
            {
                foreach (StiComponent component in containerForRender.Components)
                {
                    var container = component as StiContainer;
                    if (container == null) continue;

                    if (container.ContainerInfoV2.ParentBand != null &&
                        (container.ContainerInfoV2.ParentBand.ResetPageNumber &&
                        (!container.ContainerInfoV2.IgnoreResetPageNumber)) &&
                        (!container.ContainerInfoV2.IsAutoRendered))
                    {
                        var indexOfLastPage = Report.RenderedPages.Count - 2;
                        if (isFinal)
                            indexOfLastPage++;

                        PageNumbers.ResetPageNumber(indexOfLastPage);
                    }
                }
            }
        }

        /// <summary>
        /// Находит все контейнеры - колонки и завершает их. Процедура завершения включает в себя вызов метода
        /// FinishColumns. Этот метод располагает контейнеры в колонки.
        /// </summary>
        public void FinishColumns(StiContainer containerForRender)
        {
            if (containerForRender == null) return;

            int index = 0;
            while (index < containerForRender.Components.Count)
            {
                var columnsCont = containerForRender.Components[index] as StiColumnsContainer;
                if (columnsCont != null)
                {
                    columnsCont.FinishColumns();
                    containerForRender.Components.Remove(columnsCont);

                    lock (((ICollection)columnsCont.Components).SyncRoot)
                    {
                        foreach (StiComponent comp in columnsCont.Components)
                        {
                            comp.Left += columnsCont.Left;
                            comp.Top += columnsCont.Top;
                            comp.Parent = containerForRender;
                            containerForRender.Components.Insert(index, comp);
                            index++;
                        }
                    }
                }
                else
                {
                    index++;
                }
            }
        }

        /// <summary>
        /// Adds a specified container into the container for output.
        /// </summary>
        public void AddContainerToDestination(StiContainer container)
        {
            if (ContainerForRender == null) return;

            if (container.ContainerInfoV2.SetSegmentPerWidth != -1)
            {
                if (Page != null && container.ContainerInfoV2.SetSegmentPerWidth > Page.SegmentPerWidth)
                    Page.SegmentPerWidth = container.ContainerInfoV2.SetSegmentPerWidth;
            }

            ContainerForRender.Components.Add(container);
        }

        internal void InvokePageAfterPrint()
        {
            if (TemplatePage != null && TemplatePage.Report != null && TemplatePage.Report.RenderedPages.Count != 0)
            {
                var prevPage = Report.RenderedPages[Report.RenderedPages.Count - 1];
                if (prevPage == null) return;

                prevPage.InvokeAfterPrint(StiOptions.Engine.UseTemplateForPagePrintEvents ? TemplatePage : prevPage, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Adds a specified page into the collection of rendered pages.
        /// </summary>
        public void AddPageToRenderedPages(StiPage page)
        {
            InvokePageAfterPrint();

            /*Check the StopBeforePrint property of a page */
            if (TemplatePage.StopBeforePrint != 0 && (TemplatePage.PageInfoV2.RenderedCount - 1) == TemplatePage.StopBeforePrint)
                throw new StiStopBeforePrintException();

            /*Check the StopBeforePage property of a report */
            if (Report.StopBeforePage != 0 && Report.StopBeforePage <= Report.CurrentPrintPage)
                throw new StiStopBeforePageException();

#if CLOUD
            var maxReportPages = StiCloudReport.GetMaxReportPages(Report.ReportGuid);
            if (maxReportPages <= Report.CurrentPrintPage)
            {
                StiCloudReportResults.InitMaxReportPages(Report.ReportGuid, maxReportPages);
                throw new StiStopBeforePageException();
            }
#endif

            if (StiOptions.Engine.FixPageNumberInEvents)
            {
                Report.CurrentPrintPage++;
                Report.RenderedPages.Add(page);

                ProcessRendering();
                TemplatePage.InvokeRendering();
            }
            else
            {
                Report.RenderedPages.Add(page);
                Report.CurrentPrintPage++;
            }

            if (Page.Report.RenderedPages.Count != 0)
            {
                if (!SkipFirstPageBeforePrintEvent)
                    Page.InvokeBeforePrint(StiOptions.Engine.UseTemplateForPagePrintEvents ? TemplatePage : Page, EventArgs.Empty);

                SkipFirstPageBeforePrintEvent = false;

                Page.PaperSize = TemplatePage.PaperSize;
                Page.Orientation = TemplatePage.Orientation;
                Page.PageWidth = TemplatePage.PageWidth;
                Page.PageHeight = TemplatePage.PageHeight;
                Page.Margins = new StiMargins(
                    TemplatePage.Margins.Left,
                    TemplatePage.Margins.Right,
                    TemplatePage.Margins.Top,
                    TemplatePage.Margins.Bottom);
            }
        }

        /// <summary>
        /// Checks a freespace on a page. Если свободного места недостаточно для размещения 
        /// контейнера, то генерируется новая страница. Возвращает true, если сгенерирована новая страница.
        /// </summary>
        private bool CheckFreeSpace(StiContainer container)
        {
            if (IsCrossBandsMode)
            {
                return CheckFreeSpace(container.Width);
            }
            else
            {
                var result = CheckFreeSpace(container.Height);
                if (result)
                {
                    Breakable.SetCanBreak(container);
                    return false;
                }
                return result;
            }
        }

        /// <summary>
        /// Checks a freespace on a page. If there is no free space to place a container 
        /// of the specified height then a new page is generated. Returns true if 
        /// a new page is generated.
        /// </summary>
        private bool CheckFreeSpace(double value)
        {
            if (IsCrossBandsMode)
            {
                if ((decimal)value > (decimal)CrossFreeSpace)
                {
                    NewDestination();
                    return (decimal)value <= (decimal)CrossFreeSpace;
                }
            }
            else
            {
                if ((decimal)value > (decimal)FreeSpace)
                {
                    if (TemplatePage.UnlimitedHeight && TemplatePage.Columns < 2)
                    {
                        if (Page == null)
                        {
                            FreeSpace += TemplatePage.PageHeight - TemplatePage.Margins.Top - TemplatePage.Margins.Bottom;
                            return false;
                        }

                        while ((int)((PositionY + value) / TemplatePage.Height + 1) > Page.SegmentPerHeight)
                        {
                            Page.SegmentPerHeight++;
                            if (!TemplatePage.UnlimitedBreakable) FreeSpace += TemplatePage.Height;
                        }

                        if (value > FreeSpace + (TemplatePage.Height - PositionBottomY))
                        {
                            if (TemplatePage.UnlimitedBreakable)
                            {
                                if (PositionY > 0)
                                {
                                    FreeSpace += TemplatePage.Height - PositionBottomY;
                                    bool storedValue = IgnoreUnlimitedHeightForNewPage;
                                    IgnoreUnlimitedHeightForNewPage = false;
                                    NewDestination();
                                    IgnoreUnlimitedHeightForNewPage = storedValue;
                                }

                                if (value > TemplatePage.Height)
                                    return true;
                            }
                            return false;
                        }
                    }
                    else
                    {
                        NewDestination();
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Saves theband index if it is the DataBand. The index is used to indicate the last 
        /// printed DataBand and is used in the KeepFooter function.
        /// </summary>
        private void StoreLatestDataBand(StiBand band)
        {
            if (!(band is StiDataBand)) return;

            if (ColumnsOnDataBand.Enabled)
            {
                var columns = ColumnsOnDataBand.GetColumns();
                if (columns == null)
                {
                    IndexOfLatestDataBand = new StiIndex(0, ContainerForRender.Components.Count);
                }
                else
                {
                    var indexOfColumns = ContainerForRender.Components.IndexOf(columns);
                    IndexOfLatestDataBand = new StiIndex(columns.Components.Count, indexOfColumns);
                }
            }
            else
            {
                IndexOfLatestDataBand = new StiIndex(ContainerForRender.Components.Count);
            }
        }

        private void SetReportVariables(StiBand band)
        {
            band.SetReportVariables();

            #region Set Column Number
            if (ColumnsOnDataBand.Enabled && band is StiDataBand)
            {
                var columns = ColumnsOnDataBand.GetColumns();
                if (columns != null)
                    Report.Column = columns.GetCurrentColumn();
            }
            else
            {
                Report.Column = ColumnsOnPanel.CurrentColumn;
            }
            #endregion
        }


        public bool CheckForDuplicate(string textName, string value, string tag)
        {
            if (DuplicatesLastValues == null)
                DuplicatesLastValues = new Hashtable();

            if (DuplicatesLastValues.ContainsKey(textName) &&
                value == (string)DuplicatesLastValues[textName] &&
                tag == (string)DuplicatesLastValues[textName + "_tag"])
            {
                return true;
            }

            DuplicatesLastValues[textName] = value;
            DuplicatesLastValues[textName + "_tag"] = tag;

            return false;
        }

        public void ResetProcessingDuplicates(string componentName)
        {
            if (DuplicatesLastValues != null && DuplicatesLastValues.ContainsKey(componentName))
                DuplicatesLastValues[componentName] = null;
        }

        public void ResetProcessingDuplicates(StiSimpleText component)
        {
            var componentName = $"{component.Left}_{component.Width}";
            if (DuplicatesLastValues != null && DuplicatesLastValues.ContainsKey(componentName))
                DuplicatesLastValues[componentName] = null;
        }

        public decimal GetSumTagsOnPage(StiPage page, string componentName)
        {
            decimal result = 0;
            foreach (StiComponent comp in page.GetComponents())
            {
                if (comp.Name == componentName && comp.TagValue != null)
                    result += global::System.Convert.ToDecimal(comp.TagValue);
            }
            return result;
        }

        public StiComponent GetComponentByNameFromRenderedPage(StiPage page, string componentName)
        {
            foreach (StiComponent comp in page.GetComponents())
            {
                if (comp.Name == componentName)
                    return comp;
            }
            return null;
        }
        #endregion

        #region Methods.Render
        /// <summary>
        /// Renders a specified band taking Child bands into consideration.
        /// </summary>
        /// <param name="band"></param>
        /// <returns></returns>
        public StiComponentsCollection RenderBand(StiBand band)
        {
            return RenderBand(band, false, false);
        }

        /// <summary>
        /// Renders a specified band taking Child bands into consideration.
        /// </summary>
        /// <param name="band">Tha band that should be rendered.</param>
        /// <param name="ignorePageBreaks">True, if it is necessary to ignore NewPageAfter, NewPageBefore,
        /// NewColumnAfter, and NewColumnBefore properties.</param>
        /// <param name="allowRenderingEvents">True, if it isnecessary to call the Rendering event.
        /// для DataBand и для GroupHeaderBand.</param>
        /// <returns>The list of rendered containers for the specified band.</returns>
        internal StiComponentsCollection RenderBand(StiBand band, bool ignorePageBreaks, bool allowRenderingEvents)
        {
            //Add the output band into the collection of output bands
            BandsInProgress.Add(band);

            try
            {
                var renderedContainers = new StiComponentsCollection();

                #region Keep Child Bands
                var allowChilds = true;
                if (band is StiDataBand && ((StiDataBand)band).Columns > 1)
                    allowChilds = false;

                StiComponentsCollection childs = null;
                if (allowChilds)
                {
                    childs = GetChildBands(band);

                    lock (((ICollection)childs).SyncRoot)
                    {
                        foreach (StiChildBand child in childs)
                        {
                            if (child.KeepChildTogether)
                                AddLevel();
                        }
                    }
                }
                #endregion

                #region Render Master Band
                var isChildsEnabled = true;
                var container = InternalRenderBand(band, ignorePageBreaks, allowRenderingEvents, ref isChildsEnabled);
                if (container != null)
                    renderedContainers.Add(container);
                #endregion

                #region Render Child Bands
                if (allowChilds)
                {
                    lock (((ICollection)childs).SyncRoot)
                    {
                        foreach (StiChildBand child in childs)
                        {
                            var saveEnabled = child.Enabled;
                            if (!isChildsEnabled && !child.PrintIfParentDisabled)
                                child.Enabled = false;

                            var tempEnabled = !(!isChildsEnabled && !child.PrintIfParentDisabled);
                            var childContainer = InternalRenderBand(child, ignorePageBreaks, allowRenderingEvents, ref tempEnabled);
                            if (childContainer != null)
                                renderedContainers.Add(childContainer);

                            if (child.KeepChildTogether)
                                RemoveLevel();

                            child.Enabled = saveEnabled;
                        }
                    }
                }
                #endregion

                #region Remake ComponentPlacement of headers and footers of detail bands for exports
                if (((band is StiHeaderBand && (band as StiHeaderBand).PrintOnAllPages) ||
                    (band is StiFooterBand && (band as StiFooterBand).PrintOnAllPages)) &&
                    container != null && container.Components.Count > 0)
                {
                    var engine = this;
                    if (Report?.Engine != null)
                        engine = Report.Engine;

                    if (engine.componentPlacementRemakeTable == null)
                        engine.componentPlacementRemakeTable = new Hashtable();

                    if (!engine.componentPlacementRemakeTable.ContainsKey(band))
                    {
                        engine.componentPlacementRemakeTable[band] = null;
                        StiDataBand detailBand = null;

                        if (band is StiHeaderBand)
                            detailBand = StiHeaderBandV2Builder.GetMaster(band as StiHeaderBand);

                        if (band is StiFooterBand)
                            detailBand = StiFooterBandV2Builder.GetMaster(band as StiFooterBand);

                        if (detailBand?.MasterComponent is StiDataBand)
                            engine.componentPlacementRemakeTable[band] = detailBand.MasterComponent;
                    }

                    var masterBand = engine.componentPlacementRemakeTable[band];
                    if (masterBand != null && masterBand is StiDataBand)
                    {
                        //Remake ComponentPlacement string
                        var componentPlacement = container.Components[0].ComponentPlacement;
                        if (!string.IsNullOrEmpty(componentPlacement))
                        {
                            componentPlacement += $"_r{(masterBand as StiDataBand).Position}";
                            foreach (StiComponent comp in container.Components)
                            {
                                comp.ComponentPlacement = componentPlacement;
                            }
                        }
                    }
                }
                #endregion

                return renderedContainers;
            }
            finally
            {
                //Remove output band from the collection of output bands
                BandsInProgress.RemoveAt(BandsInProgress.Count - 1);
            }
        }

        /// <summary>
        /// Renders a specified band without taking Child bands into consideration. 
        /// </summary>
        /// <param name="band">A band that should be rendered.</param>
        /// <param name="ignorePageBreaks">True, if it is necessary to output NewPageAfter, NewPageBefore,
        /// NewColumnAfter, and NewColumnBefore properties.</param>
        /// <param name="allowRenderingEvents">True, if it is necessary to call the Rendering event 
        /// for the DataBand and for the GroupHeaderBand.</param>
        /// <param name="isChildsEnabled"></param>
        /// <returns>Rendered band as a container.</returns>
        private StiContainer InternalRenderBand(StiBand band, bool ignorePageBreaks, bool allowRenderingEvents, ref bool isChildsEnabled)
        {
            StiContainer renderedContainer = null;

            //Saves the last DataBand
            StoreLatestDataBand(band);

            #region Conditions
            var brush = band;
            var border = band;

            StiBrush savedBrush = null;
            var savedBorderSides = StiBorderSides.None;

            if (brush != null)
                savedBrush = brush.Brush;

            if (border != null && border.Border != null)
                savedBorderSides = border.Border.Side;

            var savedEnabled = band.Enabled;
            #endregion

            #region Check UseParentStyles part1
            var report = band.Report;
            StiBaseStyle parentStyle = null;
            var needHashParentStyles = false;

            if (band.UseParentStyles && (band.Parent != null))
            {
                if ((report != null) && (report.Engine != null) && (report.Engine.HashParentStyles != null) &&
                    (report.Engine.HashParentStyles.Count > 0))
                {
                    parentStyle = report.Engine.HashParentStyles[band.Parent] as StiBaseStyle;
                }

                if (parentStyle == null)
                    parentStyle = StiBaseStyle.GetStyle(band.Parent);

                if (parentStyle != null)
                {
                    parentStyle.SetStyleToComponent(band);
                    needHashParentStyles = true;

                    var compStyle = band.GetComponentStyle();
                    compStyle?.SetStyleToComponent(band);
                }
            }
            #endregion

            if (!StiOptions.Engine.UseParentStylesOldMode)
            {
                //Apply style to databand
                var tempStyle = StiOddEvenStylesHelper.ApplyOddEvenStyles(Report, band as StiDataBand, band);
                if (tempStyle != null)
                    parentStyle = tempStyle;
            }

            band.InvokeBeforePrint(band, EventArgs.Empty);

            #region Check UseParentStyles part2
            if ((report != null) && (report.Engine != null) && (report.Engine.HashUseParentStyles.ContainsKey(band)))
            {
                if ((parentStyle == null) && !string.IsNullOrWhiteSpace(band.ComponentStyle))
                    parentStyle = report.Styles[band.ComponentStyle];

                if ((report.Engine.HashParentStyles != null) && (report.Engine.HashParentStyles.Count > 0))
                {
                    var tempStyle = report.Engine.HashParentStyles[band] as StiBaseStyle;
                    if (tempStyle != null)
                        parentStyle = tempStyle;
                }

                var tempStyle2 = StiBaseStyle.GetStyle(band, parentStyle);
                if (report.Engine.HashParentStyles == null)
                    report.Engine.HashParentStyles = new Hashtable();

                report.Engine.HashParentStyles[band] = tempStyle2;
                needHashParentStyles = true;
            }
            #endregion

            var dataBand = band as StiDataBand;
            var businessObject = dataBand?.BusinessObject;

            #region Invoke rendering events in old mode
            if (StiOptions.Engine.OldModeOfRenderingEventInEngineV2 && dataBand != null && allowRenderingEvents)
            {
                if (band.Enabled)
                {
                    dataBand.InvokeRendering();
                    dataBand.InvokeGroupRendering();
                }
                else if (dataBand.CalcInvisible)
                {
                    dataBand.InvokeRendering();
                    dataBand.InvokeGroupRendering();
                }
            }
            #endregion

            //Clear PrintOnAllPagesIgnoreList in first time.
            if (band is StiDataBand && !IsNeedToSkip(band) && !DenyClearPrintOnAllPagesIgnoreList)
            {
                PrintOnAllPagesIgnoreList.Clear();
                AtLeastOneDatabandRenderedOnPage = true;
            }

            if (!isChildsEnabled)
                band.Enabled = false;

            isChildsEnabled = band.Enabled;

            #region Render Band
            //Process OddEven band
            if (IsNeedToPrintOddEven(band) && (!IsNeedToSkip(band)) && band.IsEnabled)
            {
                //Set report variable for specified band
                SetReportVariables(band);

                //New page/column before band rendering
                if (!ignorePageBreaks)
                    ProcessNewContainerBefore(band);

                #region Check for possible looping - part1
                int oldPosition = 0;
                if (dataBand != null)
                    oldPosition = dataBand.Position;
                #endregion

                //Render band to container
                renderedContainer = band.InternalRender() as StiContainer;
                if (dataBand != null)
                {
                    renderedContainer.ContainerInfoV2.DataBandPosition = oldPosition;   //cached, was dataBand.Position

                    var needStore = (dataBand.FilterMethodHandler != null) ||
                        (dataBand.Sort != null && dataBand.Sort.Length > 0) ||
                        (report.DataBandsUsedInPageTotals != null && Array.IndexOf(report.DataBandsUsedInPageTotals, band.Name) != -1);

                    if (needStore && dataBand.DataSource != null)
                        renderedContainer.ContainerInfoV2.DataSourceRow = dataBand.DataSource.GetDataRow(dataBand.DataSource.Position);

                    if ((businessObject != null) && (dataBand.MasterComponent != null || needStore))
                        renderedContainer.ContainerInfoV2.BusinessObjectCurrent = businessObject.Current;
                }

                #region Check for possible looping - part2; possible infinite loop, most often with BusinessObjects
                if (dataBand != null)
                {
                    if (businessObject != null && !businessObject.isEnumeratorCreated)
                        businessObject.SetDetails();

                    var pos2 = dataBand.Position;
                    if (pos2 < oldPosition)
                        dataBand.Position = oldPosition;
                    else
                        oldPosition = pos2;
                }
                #endregion

                #region Assign parent width of a container
                if ((band.Parent != null) && !band.IsCross)
                {
                    if (band.Parent is StiPage) //Assign page width
                    {
                        if (band.ComponentType == StiComponentType.Static)
                        {
                            renderedContainer.Width = ((StiPage)band.Parent).Width;
                        }
                        else
                        {
                            renderedContainer.Width = ((StiPage)band.Parent).GetColumnWidth();
                        }
                    }
                    else
                    {
                        renderedContainer.Width = band.Parent.Width;
                    }
                }
                #endregion

                #region Runs all events
                var storedPage = renderedContainer.Page;
                renderedContainer.Page = band.Page;

                renderedContainer.InvokeEvents();

                renderedContainer.Page = storedPage;
                #endregion

                renderedContainer.ContainerInfoV2.ParentBand = band;

                if (band.ComponentType != StiComponentType.Static)
                    renderedContainer.DockStyle = StiDockStyle.None;

                if (StiOptions.Engine.UseParentStylesOldMode)
                {
                    //Apply style to databand
                    StiOddEvenStylesHelper.ApplyOddEvenStyles(Report, band as StiDataBand, renderedContainer);
                }

                if (!SilentMode)
                {
                    //oldPosition = 0;
                    //if (dataBand != null) oldPosition = dataBand.Position;

                    RenderContainer(renderedContainer,
                        PrintAtBottom.CanProcess(band),
                        FootersOnAllPages.CanProcess(band));

                    //Possible infinite loop, most often with BusinessObjects
                    if ((dataBand != null) && (dataBand.Position < oldPosition))
                        dataBand.Position = oldPosition;
                }

                //New page/column after band
                if (!ignorePageBreaks)
                    ProcessNewContainerAfter(band);
            }
            #endregion

            #region Invoke rendering events
            if ((!StiOptions.Engine.OldModeOfRenderingEventInEngineV2) && dataBand != null && allowRenderingEvents)
            {
                if (band.Enabled)
                {
                    dataBand.InvokeRendering();
                    dataBand.InvokeGroupRendering();
                }
                else if (dataBand.CalcInvisible)
                {
                    dataBand.InvokeRendering();
                    dataBand.InvokeGroupRendering();
                }
            }
            #endregion

            band.InvokeAfterPrint(band, EventArgs.Empty);

            #region Check UseParentStyles part3
            if (needHashParentStyles && (report.Engine.HashParentStyles != null))
            {
                report.Engine.HashParentStyles.Remove(band);
            }
            #endregion

            #region Conditions
            if (brush != null)
                brush.Brush = savedBrush;

            if (border?.Border != null)
                border.Border.Side = savedBorderSides;

            band.Enabled = savedEnabled;
            #endregion

            #region Optimize border objects
            var masterBorderObject = band;
            if (masterBorderObject != null)
            {
                var renderedBorderObject = renderedContainer;
                if (renderedBorderObject != null)
                {
                    if (!ReferenceEquals(masterBorderObject.Border, renderedBorderObject.Border) &&
                        masterBorderObject.Border.Equals(renderedBorderObject.Border))
                    {
                        renderedBorderObject.Border = masterBorderObject.Border;
                    }
                }
            }
            #endregion

            #region StiTable
            CheckContainerOnTable(renderedContainer);
            #endregion
            return renderedContainer;
        }

        #region StiTable
        private void CheckContainerOnTable(StiContainer panel)
        {
            if (panel?.ContainerInfoV2?.ParentBand == null) return;

            #region StiHeaderBand
            if (panel.ContainerInfoV2.ParentBand is StiHeaderBand)
            {
                if (!((StiHeaderBand)panel.ContainerInfoV2.ParentBand).HeaderBandInfoV2.IsTableHeader) return;

                var coll = panel.GetComponents();

                foreach (StiComponent comp in coll)
                {
                    var cell = comp as IStiTableCell;
                    if (cell == null || !cell.Join || cell.ParentJoinCell == null) continue;

                    if (comp.Parent != null)
                        comp.Parent.MinSize = comp.Parent.MaxSize = new SizeD(comp.Parent.Width, comp.Parent.Height);

                    var parentJoinCell = coll[cell.ParentJoinCell.Name];
                    if (parentJoinCell != null)
                    {
                        comp.Height = parentJoinCell.Parent.Bottom - comp.Parent.Top;

                        if (cell.CellDockStyle != StiDockStyle.None)
                            comp.MaxSize = comp.MinSize = new SizeD(comp.Width, comp.Height);
                    }
                    continue;
                }

                return;
            }
            #endregion

            #region StiGroupHeaderBand
            if (panel.ContainerInfoV2.ParentBand is StiGroupHeaderBand)
            {
                if (!((StiGroupHeaderBand)panel.ContainerInfoV2.ParentBand).GroupHeaderBandInfoV2.IsTableGroupHeader) return;

                var coll = panel.GetComponents();

                foreach (StiComponent comp in coll)
                {
                    var cell = comp as IStiTableCell;
                    if (cell == null || !cell.Join || cell.ParentJoinCell == null) continue;

                    if (comp.Parent != null)
                        comp.Parent.MinSize = comp.Parent.MaxSize = new SizeD(comp.Parent.Width, comp.Parent.Height);

                    var parentJoinCell = coll[cell.ParentJoinCell.Name];
                    if (parentJoinCell != null)
                    {
                        comp.Height = parentJoinCell.Parent.Bottom - comp.Parent.Top;

                        if (cell.CellDockStyle != StiDockStyle.None)
                            comp.MaxSize = comp.MinSize = new SizeD(comp.Width, comp.Height);
                    }
                    continue;
                }

                return;
            }
            #endregion

            #region StiTable
            if (panel.ContainerInfoV2.ParentBand is StiTable)
            {
                var coll = panel.GetComponents();
                foreach (StiComponent comp in coll)
                {
                    var cell = comp as IStiTableCell;
                    if (cell == null || !cell.Join || cell.ParentJoinCell == null) continue;

                    if (comp.Parent != null)
                        comp.Parent.MinSize = comp.Parent.MaxSize = new SizeD(comp.Parent.Width, comp.Parent.Height);

                    var parentJoinCell = coll[cell.ParentJoinCell.Name];
                    if (parentJoinCell != null)
                    {
                        comp.Height = parentJoinCell.Parent.Bottom - comp.Parent.Top;

                        if (cell.CellDockStyle != StiDockStyle.None)
                            comp.MaxSize = comp.MinSize = new SizeD(comp.Width, comp.Height);
                    }
                    continue;
                }

                return;
            }
            #endregion

            #region StiFooterBand
            if (panel.ContainerInfoV2.ParentBand is StiFooterBand)
            {
                if (!((StiFooterBand)panel.ContainerInfoV2.ParentBand).FooterBandInfoV2.IsTableFooter) return;

                var coll = panel.GetComponents();
                foreach (StiComponent comp in coll)
                {
                    var cell = comp as IStiTableCell;
                    if (cell == null || !cell.Join || cell.ParentJoinCell == null) continue;

                    if (comp.Parent != null)
                        comp.Parent.MinSize = comp.Parent.MaxSize = new SizeD(comp.Parent.Width, comp.Parent.Height);

                    var parentJoinCell = coll[cell.ParentJoinCell.Name];
                    if (parentJoinCell != null)
                    {
                        comp.Height = parentJoinCell.Parent.Bottom - comp.Parent.Top;

                        if (cell.CellDockStyle != StiDockStyle.None)
                            comp.MaxSize = comp.MinSize = new SizeD(comp.Width, comp.Height);
                    }
                    continue;
                }

                return;
            }
            #endregion

            #region StiGroupFooterBand
            if (panel.ContainerInfoV2.ParentBand is StiGroupFooterBand)
            {
                if (!((StiGroupFooterBand)panel.ContainerInfoV2.ParentBand).GroupFooterBandInfoV2.IsTableGroupFooter) return;

                var coll = panel.GetComponents();

                foreach (StiComponent comp in coll)
                {
                    var cell = comp as IStiTableCell;
                    if (cell == null || !cell.Join || cell.ParentJoinCell == null) continue;

                    if (comp.Parent != null)
                        comp.Parent.MinSize = comp.Parent.MaxSize = new SizeD(comp.Parent.Width, comp.Parent.Height);

                    var parentJoinCell = coll[cell.ParentJoinCell.Name];
                    if (parentJoinCell != null)
                    {
                        comp.Height = parentJoinCell.Parent.Bottom - comp.Parent.Top;

                        if (cell.CellDockStyle != StiDockStyle.None)
                            comp.MaxSize = comp.MinSize = new SizeD(comp.Width, comp.Height);
                    }
                    continue;
                }

                return;
            }
            #endregion
        }
        #endregion

        /// <summary>
        /// Renders a specified container in the current container for output.
        /// </summary>
        /// <param name="container">A container that should be rendered.</param>
        /// <returns>Rendered container.</returns>
        internal StiContainer RenderContainer(StiContainer container)
        {
            return RenderContainer(container, false, false);
        }

        /// <summary>
        /// Renders a specified container.
        /// </summary>
        /// <param name="container">A container that should be rendered.</param>
        /// <param name="isPrintAtBottom">True, if a container should be output on the bottom.</param>
        /// <param name="isFooterOnAllPages">True, if a container is a Footer that is output on all pages.</param>
        /// <returns>Rendered container.</returns>
        private StiContainer RenderContainer(StiContainer container,
            bool isPrintAtBottom, bool isFooterOnAllPages)
        {
            #region If the specified container is a container of columns
            if (container is StiColumnsContainer)
            {
                return InternalRenderColumnsContainer(container);
            }
            #endregion

            #region If it is necessary to form a new container of columns and output it
            if (container.ContainerInfoV2.IsColumns && ColumnsOnDataBand.GetColumns(container) == null)
            {
                ColumnsOnDataBand.RenderColumns(container.ContainerInfoV2.ParentBand as StiDataBand);
            }
            #endregion

            #region If it is necessary to putput the specified container into colums on the DataBand (container of columns)
            var dataBand = container.ContainerInfoV2.ParentBand as StiDataBand;
            if (dataBand != null && dataBand.Columns > 1 && ColumnsOnDataBand.Enabled)
            {
                return InternalRenderContainerToColumns(container, isPrintAtBottom, isFooterOnAllPages);
            }
            #endregion

            #region If it is necessary to output a container into the current container
            return InternalRenderContainer(container, isPrintAtBottom, isFooterOnAllPages);
            #endregion
        }

        /// <summary>
        /// Renders the specified container of columns into the current container for output.
        /// </summary>
        /// <param name="container">A container that should be rendered.</param>
        /// <returns>Rendered container.</returns>
        private StiContainer InternalRenderColumnsContainer(StiContainer container)
        {
            if (IsCrossBandsMode)
            {
                container.Top = PositionY;
                container.Left = PositionX;
                AddContainerToDestination(container);
                CrossFreeSpace -= container.Width;
                PositionX += container.Width;
            }
            else
            {
                container.Left = PositionX;
                container.Top = PositionY;
                AddContainerToDestination(container);
                FreeSpace -= container.Height;
                PositionY += container.Height;
            }

            return container;
        }

        /// <summary>
        /// Outputs the specified container into the last container of columns.
        /// </summary>
        /// <param name="container">A container that should be rendered.</param>
        /// <param name="isPrintAtBottom">True, if a container should be output on the bottom.</param>
        /// <param name="isFooterOnAllPages">True, if a container is a Footer that is placed on all pages.</param>
        /// <returns>Rendered container.</returns>
        private StiContainer InternalRenderContainerToColumns(StiContainer container,
            bool isPrintAtBottom, bool isFooterOnAllPages)
        {
            container.ContainerInfoV2.IsColumns = true;
            var columns = ColumnsOnDataBand.GetColumns();
            if (columns == null)
                return null;

            var additionalSpace = columns.HowMuchAdditionalSpaceNeeded(columns.Height, container);

            if (additionalSpace > 0)
            {
                double correction = 0;
                if (columns.Page != null && columns.Page.UnlimitedHeight && columns.Page.UnlimitedBreakable)
                {
                    double pageHeight = columns.Page.PageHeight - columns.Page.Margins.Top - columns.Page.Margins.Bottom;
                    if ((int)((columns.Top + columns.Height) / pageHeight) !=
                        (int)((columns.Top + columns.Height + additionalSpace) / pageHeight))
                    {
                        correction += pageHeight - (columns.Top + columns.Height) % pageHeight;
                        additionalSpace -= correction;
                    }
                }

                if ((FreeSpace < additionalSpace) && !flagRenderColumnsOnDataBandOnNewPage &&
                    !(PositionY == 0 || LastFreeSpaceOnPageAfterNewList == FreeSpace || additionalSpace > LastFreeSpaceOnPageAfterNewList))
                {
                    if (columns.Page != null && columns.Page.UnlimitedHeight && !columns.Page.UnlimitedBreakable)
                    {
                        var storeY = PositionY;
                        PositionY += container.Height;
                        NewDestination();
                        PositionY = storeY;
                    }
                    else
                    {
                        NewDestination();
                    }

                    var columns2 = ColumnsOnDataBand.GetColumns();
                    if (columns2 != null && (columns2.Name == columns.Name))
                        ColumnsOnDataBand.Enabled = true;

                    flagRenderColumnsOnDataBandOnNewPage = true;

                    RenderContainer(container, isPrintAtBottom, isFooterOnAllPages);
                    return container;
                }

                flagRenderColumnsOnDataBandOnNewPage = false;

                columns.Height += additionalSpace + correction;
                PositionY += additionalSpace;
                FreeSpace -= additionalSpace;
            }
            columns.AddContainer(container);

            if (columns.Components.Count % columns.Columns == 0)
                CheckBreakColumnsContainer(columns);

            return container;
        }

        internal void CheckBreakColumnsContainer(StiColumnsContainer columns)
        {
            if (columns == null || FreeSpace >= 0) return;

            FreeSpace += columns.Height;
            PositionY -= columns.Height;
            columns.CanBreak = true;

            var cont = Breakable.ProcessBreakable(columns);
            var needSecondPass = false;
            var size = cont.GetActualSize(true, ref needSecondPass);
            cont.Height = size.Height;

            RenderContainer(cont);

            ColumnsOnDataBand.Enabled = true;
        }

        /// <summary>
        /// Renders a specified container.
        /// </summary>
        /// <param name="container">A container that should be rendered.</param>
        /// <param name="isPrintAtBottom">True, if a container should be output on the bottom.</param>
        /// <param name="isFooterOnAllPages">True, if a container is a Footеr that is output on all pages.</param>
        /// <returns>Rendered container.</returns>
        private StiContainer InternalRenderContainer(StiContainer container,
            bool isPrintAtBottom, bool isFooterOnAllPages)
        {
            //Because the full band is rendered then disable the list columns output on the DataBand
            ColumnsOnDataBand.Enabled = false;

            #region Correct the width of a container if there are columns on a page
            if (ColumnsOnPanel.Count > 0)
            {
                if ((container.ContainerInfoV2 != null) && (container.ContainerInfoV2.ParentBand != null) &&
                    (container.ContainerInfoV2.ParentBand.Parent is StiPage) &&
                    (container.ContainerInfoV2.ParentBand.ComponentType == StiComponentType.Static))
                {
                    container.Width = ((StiPage)(container.ContainerInfoV2.ParentBand.Parent)).Width;
                }
                else
                {
                    container.Width = ColumnsOnPanel.GetColumnWidth();
                }
            }
            #endregion

            //Checks whether the container can be printed. This cjeck is used to
            //prevent repeated printing of the DataBand, which has the PrintOnAllPages flag.
            if (IsNeedToSkip(container.ContainerInfoV2.ParentBand))
                return null;

            #region Automatically set the CanBreak for the ChildBand of static bands
            if (container.ContainerInfoV2.ParentBand is StiChildBand)
            {
                var masterBand = ((StiChildBand)container.ContainerInfoV2.ParentBand).GetMaster();
                if (masterBand.ComponentType == StiComponentType.Static)
                    Breakable.SetCanBreak(container);
            }
            #endregion

            var isUnlimitedHeight =
                container.ContainerInfoV2.ParentBand != null &&
                container.ContainerInfoV2.ParentBand.Page != null &&
                container.ContainerInfoV2.ParentBand.Page.UnlimitedHeight;

            if ((LastFreeSpaceOnPageAfterNewList > 0) && (container.Height > LastFreeSpaceOnPageAfterNewList) &&
                !container.CanBreak && !isUnlimitedHeight)
            {
                Breakable.SetCanBreak(container);

                var position = -1;
                if (container.ContainerInfoV2.ParentBand is StiDataBand)
                    position = (container.ContainerInfoV2.ParentBand as StiDataBand).Position;

                string str = string.Format(
                    "Whilst the Can Break property was set to False the component {0}{1} " +
                    "was split because it couldn't be placed on the page entirely.",
                    container.Name,
                    position >= 0 ? $" (position {position})" : string.Empty);

                StiLogService.Write(str);
                Report.WriteToReportRenderingMessages(str);
            }

            container = Breakable.ProcessBreakable(container);

            var canBreakStored = container.CanBreak;
            var prevPagesCount = Report.RenderedPages.Count;

            //Check free space until the specified container is placed on a page.
            while (CheckFreeSpace(container)) { }

            //If method CheckFreeSpace generate new page then try to replace StiRuntimeVariables
            if (prevPagesCount != Report.RenderedPages.Count)
                ReprocessRuntimeVariables(container);

            //Check the CanBreak procedure for containers which were marked as CanBreak
            if (canBreakStored != container.CanBreak)
                container = Breakable.ProcessBreakable(container);

            //Set current coordinate to rendered container
            if (IsCrossBandsMode)
            {
                container.Top = PositionY;
                CrossFreeSpace -= container.Width;
            }
            else
            {
                container.Left = PositionX;
                FreeSpace = (double)((decimal)(FreeSpace - container.Height));
            }

            #region Is PageFooterBand
            if (container.ContainerInfoV2.ParentBand is StiPageFooterBand)
            {
                container.Top = PositionBottomY - container.Height;
                PositionBottomY -= container.Height;
            }
            #endregion

            #region Standard Band
            else
            {
                if (IsCrossBandsMode)
                    container.Left = PositionX;
                else
                    container.Top = PositionY;

                #region isFooterOnAllPages || isPrintAtBottom
                if (isFooterOnAllPages || isPrintAtBottom)
                {
                    if (isFooterOnAllPages)
                        FootersOnAllPages.Add(container);

                    if (isPrintAtBottom)
                        PrintAtBottom.Add(container);
                }
                #endregion

                else
                {
                    if (IsCrossBandsMode)
                        PositionX += container.Width;
                    else
                        PositionY += container.Height;
                }
            }
            #endregion

            AddContainerToDestination(container);

            //Clear PrintOnAllPagesIgnoreList in the second time. We need this check because first check may occurs on previous page and
            //second check may occurs on new page
            if (container.ContainerInfoV2.ParentBand is StiDataBand && !DenyClearPrintOnAllPagesIgnoreList)
            {
                PrintOnAllPagesIgnoreList.Clear();
                AtLeastOneDatabandRenderedOnPage = true;
            }

            if (!IsCrossBandsMode && !isUnlimitedHeight && FreeSpace < 0)
                NewDestination();

            return container;
        }
        #endregion

        public StiEngine(StiReport report)
        {
            PageNumbers = new StiPageNumberHelper(this);
            EmptyBands = new StiEmptyBandsV2Helper(this);
            BandsOnAllPages = new StiBandsOnAllPages(this);
            PrintAtBottom = new StiPrintAtBottom(this);
            ProgressHelper = new StiProgressHelperV2(this);
            FootersOnAllPages = new StiFootersOnAllPages(this);
            ColumnsOnDataBand = new StiColumnsOnDataBand(this);
            ColumnsOnPanel = new StiColumnsOnPanel(this);
            StaticBands = new StiStaticBandsHelper(this);
            Breakable = new StiBreakableHelper(this);
            Threads = new StiThreads(this);

            if (report != null && report.Engine != null)
                hashUseParentStyles = report.Engine.hashUseParentStyles;

            Report = report;
        }
    }
}
