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
using Stimulsoft.Report.CrossTab.Core;

namespace Stimulsoft.Report.CrossTab
{
	/// <summary>
    /// Represents the method that handles the ProcessCell event.
	/// </summary>
    public delegate void StiProcessCellEventHandler(object sender, StiProcessCellEventArgs e);

	/// <summary>
    /// Describes an argument for the event ProcessCell.
	/// </summary>
    public class StiProcessCellEventArgs : EventArgs
	{
	    /// <summary>
        /// Gets or sets the column index.
        /// </summary>
        public StiCell Cell { get; set; }

	    /// <summary>
        /// Gets or sets the column index.
        /// </summary>
        public int Column { get; set; }

	    /// <summary>
        /// Gets or sets the row index.
        /// </summary>
        public int Row { get; set; }

	    /// <summary>
		/// Gets or sets the cell value in decimal format.
		/// </summary>
		public decimal Value { get; set; }

	    /// <summary>
        /// Gets or sets the cell text.
        /// </summary>
        public string Text { get; set; }
	}
}
