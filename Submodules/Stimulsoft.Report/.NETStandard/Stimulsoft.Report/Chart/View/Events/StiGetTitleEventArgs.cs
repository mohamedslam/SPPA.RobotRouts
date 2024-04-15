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
using Stimulsoft.Report.Chart;


namespace Stimulsoft.Report.Events
{
	/// <summary>
	/// Represents the method that handles the GetTitle event.
	/// </summary>
	public delegate void StiGetTitleEventHandler(object sender, StiGetTitleEventArgs e);

	/// <summary>
	/// Describes an argument for the event GetTitle.
	/// </summary>
	public class StiGetTitleEventArgs : EventArgs
	{
		private string valueObject;
		/// <summary>
		/// Gets or sets the title value.
		/// </summary>
		public virtual string Value
		{
			get 
			{
				return valueObject;
			}
			set 
			{
				valueObject = value;
			}
		}

		private int index;
		/// <summary>
		/// Gets or sets the index of series.
		/// </summary>
		public virtual int Index
		{
			get 
			{
				return index;
			}
			set 
			{
				index = value;
			}
		}

		private IStiSeries series = null;
		/// <summary>
		/// Gets or sets the series.
		/// </summary>
		public virtual IStiSeries Series
		{
			get 
			{
				return series;
			}
			set 
			{
				series = value;
			}
		}
	}
}
