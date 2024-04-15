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

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
	/// Represents the method that handles the UserGetData event.
	/// </summary>
	public delegate void StiUserGetDataEventHandler(object sender, StiUserGetDataEventArgs e);

	/// <summary>
	/// Class describes the arguments for event StiUserGetDataEvent.
	/// </summary>
	public class StiUserGetDataEventArgs
	{
	    /// <summary>
		/// Gets user source.
		/// </summary>
		public StiUserSource UserSource { get; }
        
	    /// <summary>
		/// Gets current position in data.
		/// </summary>
		public int Position { get; }

	    /// <summary>
		/// Gets name of the column with data.
		/// </summary>
		public string ColumnName { get; }

	    /// <summary>
		/// Gets or sets data.
		/// </summary>
		public object Data { get; set; }

	    /// <summary>
		/// Creates a new object of the type StiUserGetDataEventArgs.
		/// </summary>
		/// <param name="userSource">Data source.</param>
		/// <param name="position">Current position in data.</param>
		/// <param name="columnName">Name of the column with data.</param>
		public StiUserGetDataEventArgs(StiUserSource userSource, int position, string columnName)
		{
			this.UserSource = userSource;
			this.Position = position;
            this.ColumnName = columnName;			
		}
	}
}
