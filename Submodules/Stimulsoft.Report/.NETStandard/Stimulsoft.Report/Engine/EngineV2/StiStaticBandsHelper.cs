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
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
    /// <summary>
    /// A class helps to output PageHeaderBand, PageFooterBand, ReportTitleBand
    /// </summary>
    internal class StiStaticBandsHelper
    {
        #region Properties
        internal StiEngine Engine { get; }

        /// <summary>
        /// Returns the FreeSpace value saved in the Render method. The property is used in the SetNewColumnParameters method.
        /// </summary>
        internal double ReservedFreeSpace { get; private set; }

        internal double ReservedCrossFreeSpace { get; private set; }

        /// <summary>
        /// Returns the PositionX value saves in the Render method. The property is used in the SetNewColumnParameters method.
        /// </summary>
        internal double ReservedPositionX { get; private set; }

        /// <summary>
        /// Returns the PositionY value saves in the Render method. The property is used in the SetNewColumnParameters method.
        /// </summary>
        internal double ReservedPositionY { get; private set; }

        /// <summary>
        /// Returns the PositionBottomY value saves in the Render method. The property is used in the SetNewColumnParameters method.
        /// </summary>
        internal double ReservedPositionBottomY { get; private set; }
        #endregion

        #region Fields
        /// <summary>
        /// If true, then output of ReportTitleBands and ReportSummaryBands is denied.
        /// </summary>
        private bool denyReportBands;

        /// <summary>
        /// If true, then output of PageHeaderBands and PageFooterBands is denied.
        /// </summary>
        private bool denyPageBands;
        #endregion

        #region Methods
        /// <summary>
        /// Prints PageHeaderBand, PageFooterBand, ReportTitleBand
        /// </summary>
        public void Render()
        {
            #region Print static bands and simple components only if output a page but not a container on a page.
            if (!Engine.Threads.IsActive && !Engine.DenyChangeThread)
            {
                #region Render Static Bands
                if (Engine.TemplatePage.TitleBeforeHeader)
                    RenderTitleBeforeHeader();

                else
                    RenderHeaderBeforeTitle();
                #endregion

                #region Render Simple Components
                Engine.DenyRenderMasterComponentsInContainer = true;
                StiPageHelper.RenderSimpleComponents(Engine.TemplatePage, Engine.ContainerForRender);
                Engine.DenyRenderMasterComponentsInContainer = false;
                #endregion
                                
            }
            #endregion

            #region Initialize variables of high and low position of output for the CrossTab
            if (Engine.Page != null)
            {
                Engine.Page.PageInfoV2.PositionFromTop = Engine.PositionY;
                Engine.Page.PageInfoV2.PositionFromBottom = Engine.PositionBottomY;
            }
            #endregion

            /* Get a rectangle that Получаем прямоугольник, который описывает свободное пространство в контейнере */
            var rect = Engine.ContainerForRender.GetDockRegion(Engine.ContainerForRender, false);

            Engine.CrossFreeSpace = rect.Width;
            Engine.FreeSpace = rect.Height;
            
            #region Если в контейнере есть колонки и они должны выводиться справа налево то ...
            if (Engine.ColumnsOnPanel.Count > 1 && Engine.ColumnsOnPanel.RightToLeft && (!Engine.IsCrossBandsMode))
                Engine.PositionX = rect.Right - Engine.ColumnsOnPanel.GetColumnWidth();

            else
                Engine.PositionX = rect.Left;
            #endregion

            Engine.PositionY = rect.Y;
            Engine.PositionBottomY = rect.Bottom;

            #region Save parameters for the SetNewColumnParameters method
            this.ReservedCrossFreeSpace = Engine.CrossFreeSpace;
            this.ReservedFreeSpace = Engine.FreeSpace;
            this.ReservedPositionX = Engine.PositionX;
            this.ReservedPositionY = Engine.PositionY;
            this.ReservedPositionBottomY = Engine.PositionBottomY;
            #endregion

            if (!StiOptions.Engine.ForceNewPageForExtraColumns)
                Engine.PositionY = Engine.OffsetNewColumnY + this.ReservedPositionY;
        }

        /// <summary>
        /// Prints static bands in the TitleBeforeHeader mode. In this mode the ReportTitleBand 
        /// is output before the PageHeaderBand and it is a header of a page. This header is output only once.
        /// ReportTitleBand не выводится DataBand.
        /// </summary>
        private void RenderTitleBeforeHeader()
        {
            if (!denyReportBands)
            {
                var resDenyReportTitleBands = denyReportBands;
                var resdenyPageBands = denyPageBands;

                //When rendering the ReportTitleBand deny rendering of all static bands
                denyPageBands = true;
                denyReportBands = true;

                RenderReportTitleBands();

                denyPageBands = resdenyPageBands;
                denyReportBands = resDenyReportTitleBands;
            }

            if (!denyPageBands)
            {
                var resDenyPageBands = denyPageBands;
                var resDenyReportTitleBands = denyReportBands;

                //When rendering the PageBand deny rendering of all static bands
                denyPageBands = true;
                denyReportBands = true;

                RenderPageHeaderBands();
                RenderPageFooterBands();

                denyPageBands = resDenyPageBands;
                denyReportBands = resDenyReportTitleBands;
            }
        }

        /// <summary>
        /// Print static bands in the HeaderBeforeTitle mode. In this mode the ReportTitleBand is output before 
        /// output of the first DataBand. ReportTitleBand is output Renderer DataBand.
        /// </summary>
        private void RenderHeaderBeforeTitle()
        {
            if (denyPageBands) return;

            var resDenyPageBands = denyPageBands;

            //When rendering the PageBand deny rendering of all static bands
            denyPageBands = true;

            RenderPageHeaderBands();
            RenderPageFooterBands();

            denyPageBands = resDenyPageBands;
        }

        /// <summary>
        /// Prints ReportTitleBands. The method is used only in the TitleBeforeHeader mode.
        /// </summary>
        private void RenderReportTitleBands()
        {
            if (Engine.TemplatePage.PageInfoV2.IsReportTitlesRendered) return;

            foreach (StiComponent component in Engine.TemplatePage.Components)
            {
                var reportTitleBand = component as StiReportTitleBand;
                if (reportTitleBand != null)
                {
                    reportTitleBand.ParentBookmark = Engine.ContainerForRender.CurrentBookmark;
                    bool isNewGuidCreated = reportTitleBand.DoBookmark();

                    reportTitleBand.ParentPointer = Engine.ContainerForRender.CurrentPointer;
                    reportTitleBand.DoPointer(!isNewGuidCreated);

                    Engine.RenderBand(reportTitleBand);
                }
            }
            Engine.TemplatePage.PageInfoV2.IsReportTitlesRendered = true;
        }

        /// <summary>
        /// Prints PageHeaderBands. Bands are output considering bands from previous pages (if the appropriate option is enabled).
        /// </summary>
        private void RenderPageHeaderBands()
        {
            var components = GetPageHeaders();
            foreach (StiComponent component in components)
            {
                var pageHeaderBand = component as StiPageHeaderBand;
                if (pageHeaderBand != null)
                {
                    pageHeaderBand.ParentBookmark = Engine.ContainerForRender.CurrentBookmark;
                    bool isNewGuidCreated = pageHeaderBand.DoBookmark();

                    pageHeaderBand.ParentPointer = Engine.ContainerForRender.CurrentPointer;
                    pageHeaderBand.DoPointer(!isNewGuidCreated);

                    Engine.RenderBand(pageHeaderBand);
                }
            }
        }

        /// <summary>
        /// Prints PageFooterBands. Bands are output considering bands from previous pages (if the appropriate option is enabled).
        /// </summary>
        private void RenderPageFooterBands()
        {
            var components = GetPageFooters();
            foreach (StiComponent component in components)
            {
                var pageFooterBand = component as StiPageFooterBand;
                if (pageFooterBand != null)
                {
                    pageFooterBand.ParentBookmark = Engine.ContainerForRender.CurrentBookmark;
                    bool isNewGuidCreated = pageFooterBand.DoBookmark();

                    pageFooterBand.ParentPointer = Engine.ContainerForRender.CurrentPointer;
                    pageFooterBand.DoPointer(!isNewGuidCreated);

                    Engine.RenderBand(pageFooterBand);
                }
            }
        }

        /// <summary>
        /// Returns the collectrion of PageHeaderBands. Bands from previous pages are added to the collection (if the appropriate option is enabled).
        /// </summary>
        private StiComponentsCollection GetPageHeaders()
        {
            var comps = new StiComponentsCollection();

            #region Get index of a page from the report template, starting from what it is necessary to take PageHeaderBands
            var indexCurrent = Engine.Report.Pages.IndexOf(Engine.TemplatePage);
            var indexPage = indexCurrent;
            while (indexPage >= 0)
            {
                var page = Engine.Report.Pages[indexPage];
                if (!page.PrintHeadersFootersFromPreviousPage) break;
                indexPage--;
            }
            #endregion

            if (indexPage <= -1) indexPage = 0;

            #region Add PageHeaderBands in one collection in the correct order
            for (var index = indexPage; index <= indexCurrent; index++)
            {
                var page = Engine.Report.Pages[index];
                comps.AddRange(GetPageHeadersFromPage(page));
            }
            #endregion

            return comps;
        }

        /// <summary>
        /// Returns a collection PageFooterBands. Bands from previous pages are added to the collection (if the appropriate option is enabled).
        /// </summary>
        private StiComponentsCollection GetPageFooters()
        {
            var comps = new StiComponentsCollection();

            #region Get index of a page from the report template, starting with what it is necessary to take PageFooterBands
            var indexCurrent = Engine.Report.Pages.IndexOf(Engine.TemplatePage);
            var indexPage = indexCurrent;
            while (indexPage >= 0)
            {
                var page = Engine.Report.Pages[indexPage];
                if (!page.PrintHeadersFootersFromPreviousPage) break;
                indexPage--;
            }
            #endregion

            if (indexPage <= -1) indexPage = 0;

            #region Add PageFooterBands into one collection in the correct order
            for (var index = indexPage; index <= indexCurrent; index++)
            {
                var page = Engine.Report.Pages[index];
                comps.AddRange(GetPageFootersFromPage(page));
            }
            #endregion

            return comps;
        }

        /// <summary>
        /// Returns a collection of PageHeaderBands, which are positioned on the specified page.
        /// </summary>
        private StiComponentsCollection GetPageHeadersFromPage(StiPage page)
        {
            var comps = new StiComponentsCollection();
            foreach (StiComponent component in page.Components)
            {
                var pageHeaderBand = component as StiPageHeaderBand;
                if (pageHeaderBand != null)
                    comps.Add(pageHeaderBand);
            }
            return comps;
        }

        /// <summary>
        /// Returns a collection of PageFooterBands, which are positioned on the specified page.
        /// </summary>
        private StiComponentsCollection GetPageFootersFromPage(StiPage page)
        {
            var comps = new StiComponentsCollection();
            foreach (StiComponent component in page.Components)
            {
                var pageFooterBand = component as StiPageFooterBand;
                if (pageFooterBand != null)
                    comps.Add(pageFooterBand);
            }
            return comps;
        }
        #endregion

        internal StiStaticBandsHelper(StiEngine engine)
        {
            this.Engine = engine;
        }
    }
}
