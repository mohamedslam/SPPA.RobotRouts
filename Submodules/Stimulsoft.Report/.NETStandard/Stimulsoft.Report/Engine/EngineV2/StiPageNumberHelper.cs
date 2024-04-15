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

using Stimulsoft.Report.Components;
using System;

namespace Stimulsoft.Report.Engine
{
    /// <summary>
    /// A class helps to work with page numbers.
    /// </summary>
    internal class StiPageNumberHelper
	{
        #region Fields
		private bool finished;
        #endregion

        #region Properties
	    internal StiEngine Engine { get; }

	    public bool ClearPageNumbersOnFinish { get; set; } = true;

	    public StiPageNumberCollection PageNumbers { get; set; } = new StiPageNumberCollection();

        internal bool IsFinished => finished;
	    #endregion

		#region Methods
        internal void ResetPageNumber()
        {
            ResetPageNumber(this.PageNumbers.Count - 1);
        }

		internal void ResetPageNumber(int pageIndex)
		{
		    if (pageIndex >= PageNumbers.Count || pageIndex < 0) return;

		    this.PageNumbers[pageIndex].ResetPageNumber = true;
		}

		internal void AddPageNumber(int pageIndex, int segmentPerWidth, int segmentPerHeight)
		{
		    if (pageIndex < this.PageNumbers.Count) return;

		    this.PageNumbers.Add(new StiPageNumber
		    {
		        SegmentPerWidth = segmentPerWidth,
		        SegmentPerHeight = segmentPerHeight
		    });
		}

		internal int GetPageNumber(int pageIndex)
		{
			if (!finished)
			{
				if (Engine.Report.RenderedPages.Count == 0)
				    return 1;

			    if (PageNumbers != null && pageIndex >= 0 && pageIndex < PageNumbers.Count && PageNumbers[pageIndex].ResetPageNumber)
			        return 1;

			    return Engine.Report.RenderedPages.Count;
			}
			if (this.PageNumbers.Count == 0)
			    return pageIndex + 1;

            pageIndex = Math.Max(pageIndex, 0);
            pageIndex = Math.Min(pageIndex, this.PageNumbers.Count - 1);

			return this.PageNumbers[pageIndex].PageNumber;
		}

		internal int GetPageNumber(StiPage page)
		{
			return GetPageNumber(Engine.Report.RenderedPages.IndexOf(page));
		}

		internal int GetTotalPageCount(int pageIndex)
		{
            if (!finished)
            {
                if (Engine.Report.RenderedPages.Count == 0)
                    return 1;

                return Engine.Report.RenderedPages.Count;
            }

		    if (this.PageNumbers.Count == 0)
                return 1;

            pageIndex = Math.Max(pageIndex, 0);
            pageIndex = Math.Min(pageIndex, this.PageNumbers.Count - 1);

		    return this.PageNumbers[pageIndex].TotalPageCount;
		}

        internal int GetTotalPageCount(StiPage page)
        {
            return GetTotalPageCount(Engine.Report.RenderedPages.IndexOf(page));
        }

		internal int GetPageNumberThrough(int pageIndex)
		{
            if (!finished)
            {
                if (Engine.Report.RenderedPages.Count == 0)
                    return 1;

                return Engine.Report.RenderedPages.Count;
            }

            if (this.PageNumbers.Count == 0)
                return 1;

            pageIndex = Math.Max(pageIndex, 0);
            pageIndex = Math.Min(pageIndex, this.PageNumbers.Count - 1);

			return this.PageNumbers[pageIndex].PageNumberThrough;
		}

        internal int GetPageNumberThrough(StiPage page)
        {
            return GetPageNumberThrough(Engine.Report.RenderedPages.IndexOf(page));
        }

		internal int GetTotalPageCountThrough(int pageIndex)
		{
            if (this.PageNumbers.Count == 0)
                return 1;

            pageIndex = Math.Max(pageIndex, 0);
            pageIndex = Math.Min(pageIndex, this.PageNumbers.Count - 1);
			return this.PageNumbers[pageIndex].TotalPageCountThrough;
		}

		/// <summary>
		/// Calculates the PageNumber and TotalPageCount values for each page in the specified range.
		/// </summary>
		/// <param name="startIndex">Range start.</param>
		/// <param name="endIndex">Range end.</param>
		private void SetSystemVariables(int startIndex, int endIndex)
		{		
			if (startIndex == endIndex)return;

			var totalPageCount = 0;

			#region Count number of pages
			for (var index = startIndex; index < endIndex; index++)
			{
				totalPageCount += this.PageNumbers[index].Step;
			}
			#endregion

			#region Расставляем системные переменные PageNumber и TotalPageCount для каждой страницы
			var pageNumber = 1;
			for (var index = startIndex; index < endIndex; index++)
			{
				this.PageNumbers[index].PageNumber = pageNumber;
				this.PageNumbers[index].TotalPageCount = totalPageCount;

                pageNumber += this.PageNumbers[index].Step;
			}
			#endregion
		}


		/// <summary>
		/// Counts the PageNumber, TotalPageCount, PageNumberThrough, and TotalPageCountThrough values for every page.
		/// </summary>
		internal void ProcessPageNumbers()
		{
            #region Correction PageNumbers count
            if (Engine != null && Engine.Report != null && Engine.Report.RenderedPages != null)
            {
                while (PageNumbers.Count > Engine.Report.RenderedPages.Count)
                {
                    PageNumbers.RemoveAt(Engine.Report.RenderedPages.Count);
                }
            }
            #endregion

			#region Count number of pages TotalPageCountThrough
            var totalPageCountThrough = 0;
            var pageIndex = 0;
			foreach (StiPageNumber pageNumber in this.PageNumbers)
            {
                #region Correction pageNumber info
                if (Engine != null && Engine.Report != null && Engine.Report.RenderedPages != null && pageIndex < Engine.Report.RenderedPages.Count)
                {
                    //Get page from report without use cache
                    var page = Engine.Report.RenderedPages.GetPageWithoutCache(pageIndex);
                    pageNumber.SegmentPerWidth = page.SegmentPerWidth;
                    pageNumber.SegmentPerHeight = page.SegmentPerHeight;
                }
                #endregion

                totalPageCountThrough += pageNumber.Step;
                pageIndex++;
			}
			#endregion

			var pageNumberThrough = 1;
            var startIndex = 0;
            var endIndex = 0;

			foreach (StiPageNumber pageNumber in this.PageNumbers)
			{
				if (pageNumber.ResetPageNumber)
				{
					SetSystemVariables(startIndex, endIndex);
					startIndex = endIndex;
				}
				pageNumber.PageNumberThrough = pageNumberThrough++;
				pageNumber.TotalPageCountThrough = totalPageCountThrough;
                endIndex++;
			}
			SetSystemVariables(startIndex, endIndex);

			finished = true;
		}

		internal void Clear()
		{
            if (ClearPageNumbersOnFinish)
                PageNumbers.Clear();			
		}

        internal void ClearNotFixed()
        {
            if (!ClearPageNumbersOnFinish || PageNumbers.Count <= 0) return;

            //PageNumbers.Clear();
            var pagePos = PageNumbers.Count - 1;
            while (pagePos >= 0 && !PageNumbers[pagePos].FixedPosition)
            {
                PageNumbers.RemoveAt(pagePos);
                pagePos--;
            }
        }
        #endregion

		internal StiPageNumberHelper(StiEngine engine)
		{
			this.Engine = engine;
		}
	}
}
