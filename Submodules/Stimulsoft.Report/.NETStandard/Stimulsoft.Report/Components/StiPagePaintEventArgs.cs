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
using Stimulsoft.Report.Components;
using System.Drawing;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Report.Events
{
	/// <summary>
	/// Represents the method that is invoked for page painting.
	/// </summary>
	public delegate void StiPagePaintEventHandler(StiPage sender, StiPagePaintEventArgs e);

	/// <summary>
	/// Describes an argument for the events PagePainting and PagePainted.
	/// </summary>
	public class StiPagePaintEventArgs : EventArgs
	{
        #region Properties
        /// <summary>
        /// Gets page graphics.
        /// </summary>
        public Graphics Graphics { get; }

	    /// <summary>
		/// Gets clip rectangle.
		/// </summary>
		public Rectangle ClipRectangle { get; }

	    /// <summary>
		/// Gets client rectangle.
		/// </summary>
		public Rectangle ClientRectangle { get; }

	    /// <summary>
		/// Gets full rectangle.
		/// </summary>
		public Rectangle FullRectangle { get; }

	    /// <summary>
		/// Gets value which indicates page is printing.
		/// </summary>
		public bool IsPrinting { get; }

	    /// <summary>
		/// Gets value which indicates page is designing.
		/// </summary>
		public bool IsDesigning { get; }
        #endregion

        public StiPagePaintEventArgs(Graphics graphics, Rectangle clipRectangle, Rectangle clientRectangle, Rectangle fullRectangle,
			bool isPrinting, bool isDesigning)
		{
			this.Graphics = graphics;
			this.ClipRectangle = clipRectangle;
			this.ClientRectangle = clientRectangle;
			this.FullRectangle = fullRectangle;
			this.IsPrinting = isPrinting;
			this.IsDesigning = isDesigning;
		}
	}
}
