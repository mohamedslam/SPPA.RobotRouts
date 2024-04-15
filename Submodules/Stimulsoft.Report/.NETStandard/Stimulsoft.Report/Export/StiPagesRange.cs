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
using System.ComponentModel;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report
{
	/// <summary>
	/// Class describes pages range.
	/// </summary>
	public sealed class StiPagesRange
    {
        #region Fields
        public static readonly StiPagesRange All = new StiPagesRange();
        #endregion

        #region Properties
        /// <summary>
		/// Gets type of pages range.
		/// </summary>
		[DefaultValue(StiRangeType.All)]
		[JsonConverter(typeof(StringEnumConverter))]
		public StiRangeType RangeType { get; set; }
        
        /// <summary>
		/// Gets range of pages.
		/// </summary>
        [DefaultValue("")]
		public string PageRanges { get; set; }
        
        /// <summary>
		/// Gets current page.
		/// </summary>
        [DefaultValue(0)]
		public int CurrentPage { get; set; }
        #endregion

        #region Methods.override
	    public override int GetHashCode()
	    {
	        return base.GetHashCode();
	    }

	    public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var obj1 = (StiPagesRange)obj;

            return RangeType == obj1.RangeType && PageRanges == obj1.PageRanges && CurrentPage == obj1.CurrentPage;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns collection of selected pages.
        /// </summary>
        /// <param name="originalPages">Collection of pages for filtering.</param>
        /// <returns>Collection of selected pages.</returns>
        public StiPagesCollection GetSelectedPages(StiPagesCollection originalPages)
		{
		    var pages = new StiPagesCollection(originalPages.Report, originalPages)
		    {
		        CacheMode = originalPages.CacheMode
		    };

		    StiPagesCollection.CopyEventsOfPagesCollection(originalPages, pages);

            if (PageRanges == "All")
                RangeType = StiRangeType.All;  //fix

			#region Pages
			if (RangeType == StiRangeType.Pages)
			{
				if (!string.IsNullOrEmpty(PageRanges))
				{
					var pageNumbers = PageRanges.Split(',');
					foreach (var number in pageNumbers)
					{
                        if (number.IndexOf("-", StringComparison.InvariantCulture) == -1)
						{
							try
							{
							    int pageIndex;
                                var result = int.TryParse(number.Trim(), out pageIndex);
							    if (result && pageIndex >= 1 && pageIndex <= originalPages.Count)
							        pages.AddV2Internal(originalPages[pageIndex - 1]);
							}
							catch
							{
							}
						}
						else
						{
							var numbers = number.Split('-');

							try
							{
								var startIndex = int.Parse(numbers[0].Trim());
								var endIndex = int.Parse(numbers[1].Trim());
								endIndex = Math.Min(endIndex, originalPages.Count);

								if (startIndex >= 1 && startIndex <= originalPages.Count && 
									endIndex >= 1 && endIndex <= originalPages.Count)
                                {
                                    if (startIndex <= endIndex)
                                    {
                                        for (var index = startIndex; index <= endIndex; index++)
                                            pages.AddV2Internal(originalPages[index - 1]);
                                    }
                                    else
                                    {
                                        for (var index = startIndex; index >= endIndex; index--)
                                            pages.AddV2Internal(originalPages[index - 1]);
                                    }
                                }
							}
							catch
							{
							}
						}
					}
				}
                if (pages.Count != 0)
                    return pages;
			}
			#endregion
			
			#region CurrentPage
			else if (RangeType == StiRangeType.CurrentPage)
			{
				if (CurrentPage >= 0 && CurrentPage < originalPages.Count)
				    pages.AddV2Internal(originalPages[CurrentPage]);

                if (pages.Count != 0)
                    return pages;
			}
			#endregion

			foreach (StiPage page in originalPages)
			{
				pages.AddV2Internal(page);
			}
			
			return pages;
		}
        #endregion

        /// <summary>
        /// Creates a new instance of the StiPagesRange class.
        /// </summary>
        public StiPagesRange() : this(StiRangeType.All, "", 0)
		{
		}

        /// <summary>
        /// Creates a new instance of the StiPagesRange class.
        /// </summary>
        /// <param name="currentPage">Current page.</param>
        public StiPagesRange(int currentPage) : this(StiRangeType.CurrentPage, string.Empty, currentPage)
        {
        }

        /// <summary>
        /// Creates a new instance of the StiPagesRange class.
        /// </summary>
        /// <param name="pageRanges">Range of pages.</param>
        public StiPagesRange(string pageRanges)
            : this(StiRangeType.Pages, pageRanges, 0)
        {
        }

		/// <summary>
		/// Creates a new instance of the StiPagesRange class.
		/// </summary>
		/// <param name="rangeType">Type of pages range.</param>
		/// <param name="pageRanges">Range of pages.</param>
		/// <param name="currentPage">Current page.</param>
		public StiPagesRange(StiRangeType rangeType, string pageRanges, int currentPage)
		{
			this.RangeType = rangeType;
			this.PageRanges = pageRanges;
			this.CurrentPage = currentPage;
		}
	}
}
