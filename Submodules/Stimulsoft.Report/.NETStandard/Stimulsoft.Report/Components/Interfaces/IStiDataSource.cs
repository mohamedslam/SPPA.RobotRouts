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
using Stimulsoft.Report.Dictionary;

namespace Stimulsoft.Report.Components
{
	/// <summary>
	/// Inteface describes a data source in the component.
	/// </summary>
	public interface IStiDataSource : IStiEnumerator
	{
		/// <summary>
		/// Gets Data Source.
		/// </summary>
		StiDataSource DataSource
		{
			get;
		}

		/// <summary>
		/// Gets or sets the name of Data Source.
		/// </summary>
		string DataSourceName
		{
			get;
			set;
		}

		/// <summary>
		/// Returns true if DataSource property is not assigned.
		/// </summary>
		bool IsDataSourceEmpty
		{
			get;
		}
	}
}
