#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	Stimulsoft.Report Library										}
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
using Stimulsoft.Report.Components;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;

namespace Stimulsoft.Report.Engine
{
	/// <summary>
	/// A class helps to render a report.
	/// </summary>
	public class StiRender
	{
		#region Properties
		/// <summary>
		/// Gets or sets amount rendered component.
		/// </summary>
		public long RenderedComponents { get; set; }

		/// <summary>
		/// Gets or sets amount typed component necessities for stop of rendering of a report.
		/// </summary>
		public long RenderStop { get; set; }

        /// <summary>
        /// Gets or sets a rendered report.
        /// </summary>
        public StiReport Report { get; set; }

	    internal bool AllowCheckPrevPage { get; set; }

	    internal StiPage PrevPage
	    {
	        get
	        {
	            return Report != null && Report.RenderedPages != null && Report.RenderedPages.Count > 0
	                ? Report.RenderedPages[Report.RenderedPages.Count - 1]
	                : null;
	        }
	    }

	    internal StiComponentsCollection PageHeaders { get; } = new StiComponentsCollection();

	    internal StiComponentsCollection PageFooters { get; } = new StiComponentsCollection();
        #endregion

        #region Fields
        private bool requreResetPage;
		private bool checkPageAfterRender;
		#endregion

		#region Methods
		/// <summary>
		/// Returns the next rendered page.
		/// If more nothing to return - returns null.
		/// </summary>
		/// <returns>Rendered page.</returns>
		public StiPage GetNextPage()
		{
			StiPage renderedPage = null;
			
			if (Report.CurrentPage >= Report.Pages.Count)
			{
				//Report.IsStopped = true;//Старый хвост, перебор всех страниц отмечался установкой флага, выше по дереву вызовов, флаг ловился
				return null;
			}
            //Если флаг IsStopped установлен в событии Rendering, 
            //то отчет должен остановиться. Но этот флаг не должен сбрасываться самим генератором отчетов.
            //else Report.IsStopped = false;
			
			StiPage tmpPage = Report.GetCurrentPage();
			tmpPage.ParentBookmark = tmpPage.Report.Bookmark;
			tmpPage.ParentPointer = tmpPage.Report.Pointer;

			tmpPage.InvokeBeforePrint(tmpPage, EventArgs.Empty);

			#region If page is not enabled that necessary its skip
			if (!tmpPage.Enabled)
			{				
				tmpPage.InvokeAfterPrint(tmpPage, EventArgs.Empty);
				tmpPage.ProcessPageAfterRender();

				NextPage();
                //Если флаг IsStopped установлен в событии Rendering, 
                //то отчет должен остановиться. Но этот флаг не должен сбрасываться самим генератором отчетов.
                //Report.IsStopped = false;
				return null;
			}
			else if (tmpPage.RenderedCount == 0)
			{
				tmpPage.ProcessPageBeforeRender();
				tmpPage.Prepare();
			}
			#endregion

			tmpPage.InvokeRendering();
			renderedPage = GetPageFromTemplate(tmpPage);
			bool newPage = renderedPage.Components.Count == 0;

			renderedPage.PageInfoV1.IsPrevPage = tmpPage.PrintOnPreviousPage && 
				(!tmpPage.PrintHeadersFootersFromPreviousPage);
            
			if (Report.EngineV1.RequreResetPageNumber || requreResetPage)
			{
				Report.EngineV1.ProcessResetPageNumber(Report);
				requreResetPage = false;
				Report.EngineV1.RequreResetPageNumber = false;
			}

			Report.EngineV1.PageInProgress = renderedPage;

			bool result = false;
			
			try
			{				
				if (tmpPage.PrintHeadersFootersFromPreviousPage)
				{
					if ((!tmpPage.PrintOnPreviousPage) || 
						(tmpPage.PrintOnPreviousPage && newPage))
					{
						AddPageHeadersAndFooters(tmpPage);
					}
				}
				else ClearPageHeadersAndFooters();

				int countBefore = 0;
				if (tmpPage.PrintOnPreviousPage)countBefore = renderedPage.GetComponentsCount();

				result = tmpPage.Render(renderedPage);

				#region Copy values to previous page if new components is detected
				if (tmpPage.PrintOnPreviousPage)
				{
					int countAfter = renderedPage.GetComponentsCount();
					if (tmpPage.Columns > 1)countAfter -= tmpPage.Columns;
					if (countAfter != countBefore && checkPageAfterRender)
					{
						CopyValues(renderedPage, tmpPage);
					}
				}
				checkPageAfterRender = false;
				#endregion
			}
			finally
			{
				if (tmpPage.PrintHeadersFootersFromPreviousPage)
				{
					if ((!tmpPage.PrintOnPreviousPage) || 
						(tmpPage.PrintOnPreviousPage && newPage))
					{
						RemovePageHeadersAndFooters(tmpPage);
					}
				}
			}

			tmpPage.InvokeAfterPrint(this, EventArgs.Empty);
			bool clearRenderedPage = false;

			#region PrintOnPreviousPage
			if (AllowCheckPrevPage && PrevPage != null && tmpPage.PrintOnPreviousPage)
			{				
				clearRenderedPage = true;
			}
			else renderedPage.PageInfoV1.PageNumber = Report.PageNumber;
			
			AllowCheckPrevPage = false;
			#endregion

			#region If page reached border of the forced stop, that she is rendered successfully
			if (tmpPage.StopBeforePrint != 0 &&
				tmpPage.RenderedCount >= tmpPage.StopBeforePrint)result = true;
			#endregion

			#region Page is rendered successfully
			if (result)
			{
				tmpPage.InvokeEndRender();
				NextPage();
			}
			#endregion

			#region Runtime variable
			if (renderedPage != null)
			{
				this.Report.CurrentPrintPage++;
			}
			#endregion

			#region PrintOnPreviousPage
			if (clearRenderedPage)renderedPage = null;
			#endregion			

			return renderedPage;
		}	

		/// <summary>
		/// Prepares columns on a previous page to render new columns from the next page
		/// </summary>
		private void PreparePrevPageForRenderColumns(StiPage page)
		{
			if (page.Columns > 1)
			{
				#region Create list of container columns
                var list = new List<StiContainer>();
				foreach (StiComponent comp in page.Components)
				{
					var cont = comp as StiContainer;
					if (cont != null && cont.ContainerInfoV1.IsColumn)list.Add(cont);
				}
				#endregion

				if (list.Count != 0)
				{
					for (int index = 0; index < page.Columns - 1; index++)
					{
						var container = list[index];
						var nextContainer = list[index + 1];

						if (container.Components.Count == 0)break;
						if (nextContainer.Components.Count == 0)break;

						double maxHeight = 0;
						foreach (StiComponent comp in container.Components)
						{
							maxHeight = Math.Max(comp.Bottom, maxHeight);
						}

						#region Create new container for filling empty space of last printed column
						StiContainer emptyCont = new StiContainer();
						emptyCont.Name = container.Name + "_EmptyCont";
						emptyCont.Height = container.Height - maxHeight;
						emptyCont.DockStyle = StiDockStyle.Bottom;
						#endregion

						container.Components.Add(emptyCont);
					}
				}
			}
		}
		
		private void MoveColumnContainersAndClonesToSpecialContainer(StiPage page)
		{
			StiContainer columnContainer = null;
			foreach (StiComponent comp in page.Components)
			{
				StiContainer cont = comp as StiContainer;
				if (cont != null && cont.ContainerInfoV1.IsColumn)
				{
					if (columnContainer == null)
					{
						columnContainer = new StiContainer();
					}
					cont.CanGrow = true;
					cont.CanShrink = true;
					SizeD size2 = cont.GetActualSize();
					cont.Width = size2.Width;
					cont.Height = size2.Height;
					columnContainer.Components.Add(cont);
				}
			}

			if (columnContainer != null)
			{
				int index = page.Components.IndexOf(columnContainer.Components[0]);
				page.Components.Insert(index, columnContainer);
				columnContainer.DockStyle = StiDockStyle.Top;

				foreach (StiContainer comp in columnContainer.Components)
				{
					page.Components.Remove(comp);
					comp.Parent = columnContainer;
					comp.Page = page;
					comp.Top = 0;
				}

				columnContainer.CanGrow = true;
				columnContainer.CanShrink = true;
				SizeD size = columnContainer.GetActualSize();
				columnContainer.Name = "test";
				columnContainer.Height = size.Height;							
			}
		}

		private void ClearPageHeadersAndFooters()
		{
			PageHeaders.Clear();
			PageFooters.Clear();
		}

		/// <summary>
		/// Returns a collection of page headers from the specified page.
		/// </summary>
		private StiComponentsCollection GetPageHeaders(StiPage page)
		{
			StiComponentsCollection comps = new StiComponentsCollection();
			foreach (StiComponent comp in page.Components)
			{
				if (comp is StiPageHeaderBand)comps.Add(comp);
			}
			return comps;
		}

		/// <summary>
		/// Returns a collection of page footers from the specified page.
		/// </summary>
		private StiComponentsCollection GetPageFooters(StiPage page)
		{
			StiComponentsCollection comps = new StiComponentsCollection();
			foreach (StiComponent comp in page.Components)
			{
				if (comp is StiPageFooterBand)comps.Add(comp);
			}
			return comps;
		}

		/// <summary>
		/// Adds a collection of page headers to the specified page.
		/// </summary>
		private void AddPageHeadersAndFooters(StiPage page)
		{
			page.Components.InsertRange(0, PageHeaders);
			page.Components.InsertRange(0, PageFooters);
		}

		/// <summary>
		/// Removes a collection of page footers to the specified page.
		/// </summary>
		private void RemovePageHeadersAndFooters(StiPage page)
		{
			foreach (StiComponent comp in PageHeaders)
			{
				page.Components.Remove(comp);
			}

			foreach (StiComponent comp in PageFooters)
			{
				page.Components.Remove(comp);
			}			
		}

		/// <summary>
		/// Creates a new page on the base of a page of a template.
		/// </summary>
		public StiPage GetPageFromTemplate(StiPage tmpPage)
		{
			#region PrintOnPreviousPage
			if (AllowCheckPrevPage && PrevPage != null && tmpPage.PrintOnPreviousPage)
			{
				#region !PrintHeadersFootersFromPreviousPage
				if (!tmpPage.PrintHeadersFootersFromPreviousPage)
				{
					MoveColumnContainersAndClonesToSpecialContainer(PrevPage);
					StiContainer cont = new StiContainer();
					cont.TagValue = PrevPage.TagValue;
					cont.PointerValue = PrevPage.PointerValue;
					cont.BookmarkValue = PrevPage.BookmarkValue;					
					cont.HyperlinkValue = PrevPage.HyperlinkValue;
					cont.ToolTipValue = PrevPage.ToolTipValue;

					cont.Name = "PrevPageCont";
					cont.DockStyle = StiDockStyle.Top;
					
					cont.Components.AddRange(PrevPage.Components);

					double height = 0;
					foreach (StiComponent comp in cont.Components)
					{
						if (comp.DockStyle == StiDockStyle.Top || comp.DockStyle == StiDockStyle.Bottom)
							height += comp.Height;
					}

					cont.Height = height;

					foreach (StiComponent comp in cont.Components)
					{
						comp.Parent = cont;
					}

					PrevPage.Components.Clear();
					PrevPage.Components.Add(cont);	
				}
				#endregion

				checkPageAfterRender = true;

				if (tmpPage.PrintHeadersFootersFromPreviousPage)
					PreparePrevPageForRenderColumns(PrevPage);

				return PrevPage;
			}			
			#endregion

			StiPage page = tmpPage.Clone(false, false) as StiPage;
			page.Guid = StiGuidUtils.NewGuid();
			return page;
		}

		private void CopyValues(StiPage prevPage, StiPage tmpPage)
		{
			tmpPage.InvokeEvents();
			if (tmpPage.TagValue is string && ((string)tmpPage.TagValue).Length > 0) PrevPage.TagValue = tmpPage.TagValue;
			if (tmpPage.BookmarkValue is string && ((string)tmpPage.BookmarkValue).Length > 0) PrevPage.BookmarkValue = tmpPage.BookmarkValue;
			if (tmpPage.PointerValue is string && ((string)tmpPage.PointerValue).Length > 0) PrevPage.PointerValue = tmpPage.PointerValue;
			if (tmpPage.HyperlinkValue is string && ((string)tmpPage.HyperlinkValue).Length > 0) PrevPage.HyperlinkValue = tmpPage.HyperlinkValue;
			if (tmpPage.ToolTipValue is string && ((string)tmpPage.ToolTipValue).Length > 0) PrevPage.ToolTipValue = tmpPage.ToolTipValue;
			if (tmpPage.ExcelSheetValue != null && ((string)tmpPage.ExcelSheetValue).Length > 0) PrevPage.ExcelSheetValue = tmpPage.ExcelSheetValue;
		}

		/// <summary>
		/// Displaces the pointer on the following printed page.
		/// </summary>
		public void NextPage()
		{
			#region Prepare collection of pageheaders and pagefooters
			if (Report.CurrentPage >= 0 && Report.CurrentPage < Report.Pages.Count)
			{
				StiPage page = Report.Pages[Report.CurrentPage];
				PageHeaders.InsertRange(0, GetPageHeaders(page));
				PageFooters.InsertRange(0, GetPageFooters(page));
			}
			#endregion

			if (Report.CurrentPage >= 0)
			{
				Report.Pages[Report.CurrentPage].ProcessPageAfterRender();
			}

			bool useCurrentPage = false;
			if (this.Report.CurrentPage >= 0 && this.Report.CurrentPage < this.Report.Pages.Count)
			{
                StiPage page = this.Report.Pages[this.Report.CurrentPage];
                page.PageInfoV1.NumberOfCopiesToPrint--;
                useCurrentPage = page.PageInfoV1.NumberOfCopiesToPrint > 0;
                if (useCurrentPage)
                {
                    page.ProcessPageBeforeRender();
                    page.Prepare();
                    Report.PageCopyNumber = page.NumberOfCopies - page.PageInfoV1.NumberOfCopiesToPrint + 1;
                }
                else
                {
                    Report.PageCopyNumber = 1;
                }
			}

			if (!useCurrentPage)
			{
				do
				{
					this.AllowCheckPrevPage = true;
					this.Report.CurrentPage++;

					if (this.Report.CurrentPage >= this.Report.Pages.Count)break;
				}
				while (
					Report.Pages[Report.CurrentPage].Skip || 
					(!Report.Pages[Report.CurrentPage].Enabled));
			}

			if (Report.CurrentPage < Report.Pages.Count)
			{
				Report.Pages[Report.CurrentPage].InvokeBeginRender();
				if (Report.Pages[Report.CurrentPage].ResetPageNumber)requreResetPage = true;
			}
		}
		#endregion

		/// <summary>
		/// Creates a new object of the StiRender type.
		/// </summary>
		public StiRender(StiReport report)
		{
			#region Page counter zeroizing
			foreach (StiPage pg in report.Pages)pg.RenderedCount = 0;
			#endregion

			#region Set NumberOfCopies
			foreach (StiPage page in report.Pages)
			{
				page.PageInfoV1.NumberOfCopiesToPrint = Math.Max(page.NumberOfCopies, 1);
			}
			#endregion

			this.Report = report;
			this.Report.CurrentPrintPage = 0;
			this.Report.CurrentPage = -1;
			NextPage();
		}
	}
}
