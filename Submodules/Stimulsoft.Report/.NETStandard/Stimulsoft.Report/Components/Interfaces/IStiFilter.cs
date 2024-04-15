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
using Stimulsoft.Report.Events;

namespace Stimulsoft.Report.Components
{
	

	/// <summary>
	/// Allows to control filtering of data.
	/// </summary>
	public interface IStiFilter
	{
		/// <summary>
		/// Gets or sets a method for filtration.
		/// </summary>
		StiFilterEventHandler FilterMethodHandler
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets filter mode.
		/// </summary>
		StiFilterMode FilterMode
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the collection of data filters.
		/// </summary>
		StiFiltersCollection Filters
		{
			get;
			set;
		}
        
		/// <summary>
		/// Gets or sets value indicates, that filter is turn on.
		/// </summary>
		bool FilterOn
		{
			get;
			set;
		}
	}
}
