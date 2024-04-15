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
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
	/// <summary>
    /// Summary description for StiEngineV1.
	/// </summary>
	public class StiEngineV1
	{
	    #region Properties
        internal decimal LatestProgressValue { get; set; }

		public bool RequreResetPageNumber { get; set; }

	    /// <summary>
	    /// Internal use only.
	    /// </summary>
	    public StiPage PageInProgress { get; set; }
	
		/// <summary>
		/// Gets or sets an index of the current page printed taking into consideration segmented pages. Number starts from 1.
		/// </summary>
		public int RealPageNumber { get; set; }

		public bool IsUsedResetPageNumber { get; set; }

        public Hashtable ReportPageNumbers { get; set; }

        public Hashtable ReportTotalPageCounts { get; set; }

	    public StiReport Report { get; }

	    /// <summary>
        /// A class helps to output the progress bar when report rendering.
        /// </summary>
        internal StiProgressHelperV1 ProgressHelper { get; }
	    #endregion

        #region Methods
        /// <summary>
		/// Internal use only.
		/// </summary>
		public void ResetPageNumber()
		{
			RequreResetPageNumber = true;
		}

		/// <summary>
		/// Internal use only.
		/// </summary>
		public void ProcessResetPageNumber(StiReport report)
		{
			foreach (StiPage page in report.RenderedPages)
			{
			    if (page.PageInfoV1.TotalPageCount == -1)
			        page.PageInfoV1.TotalPageCount = report.TotalPageCount;
			}
			report.PageNumber = 1;
			report.TotalPageCount = 0;
		}
		#endregion

        public StiEngineV1(StiReport report)
        {
            this.Report = report;
            this.ProgressHelper = new StiProgressHelperV1(this);
        }
	}
}
