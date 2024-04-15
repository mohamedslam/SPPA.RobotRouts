#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	StimulReport.Net												}
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

namespace Stimulsoft.Report.Components
{
    public interface IStiPageBreak
    {
		
		/// <summary>
		/// If the value of this property is true, then, before output of a band,
		/// a new column will be generated. Output of a band will be continued on
		///	the next column.
		/// </summary>
        bool NewColumnBefore
        {
            get;
            set;
        }

		/// <summary>
		/// If the value of this property is true, then, after output of a band, a
		/// new column will be generated.
		/// </summary>
        bool NewColumnAfter
        {
            get;
            set;
        }

		/// <summary>
		/// If the value of this property is true, then, before output of a band, a new page will be
		/// generated. Output of a band will be continued on the next page.
		/// </summary>
        bool NewPageBefore
        {
            get;
            set;
        }

		/// <summary>
		/// If the value of this property is true, then, after output of a band, a new page will be
		/// generated.
		/// </summary>
        bool NewPageAfter
        {
            get;
            set;
        }

		/// <summary>
		/// Gets or sets value which indicates how much free space is on a page
		/// (in per cent) should be
		/// reserved for formation of a new page or a new column. The value
		/// should be set in the range from 0 to 100.
		/// If the value is 100 then, in any case, a new page or a new column
		/// will be formed. This property is used
		/// together with NewPageBefore, NewPageAfter, NewColumnBefore,
		/// NewColumnAfter properties.
		/// </summary>
        float BreakIfLessThan
        {
            get;
            set;
        }

        /// <summary>
        /// If the value of this property is true, then, a new page/column will be
        /// generated only starting from the second case.
        /// </summary>
        bool SkipFirst
        {
            get;
            set;
        }
    }
}
