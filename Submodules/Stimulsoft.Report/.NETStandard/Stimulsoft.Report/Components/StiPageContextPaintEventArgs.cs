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

using System.Drawing;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Report.Events
{
	/// <summary>
	/// Describes an argument for the events PagePainting and PagePainted.
	/// </summary>
	public class StiPageContextPaintEventArgs : StiPagePaintEventArgs
	{
        #region Properties
        /// <summary>
        /// Gets page context.
        /// </summary>
        public object Context { get; }
        #endregion

        public StiPageContextPaintEventArgs(object context, Rectangle clipRectangle, Rectangle clientRectangle, Rectangle fullRectangle,
			bool isPrinting, bool isDesigning) : base(context as Graphics, clipRectangle, clientRectangle, fullRectangle, isPrinting, isDesigning)
		{
			this.Context = context;
		}
	}
}
