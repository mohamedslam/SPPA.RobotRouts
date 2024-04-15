#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using System.Drawing;

#if STIDRAWING
using Pen = Stimulsoft.Drawing.Pen;
#endif

namespace Stimulsoft.Base.Drawing
{
	/// <summary>
	/// Pens for StiControls colors.
	/// </summary>
	public static class StiPens
	{
		#region Properties
	    /// <summary>
		/// A defined Pen object with width of 1 and a ControlText color.
		/// </summary>
		public static Pen ControlText { get; } = new Pen(StiColors.ControlText);

	    /// <summary>
		/// A defined Pen object with width of 1 and a Selected color.
		/// </summary>
		public static Pen Selected { get; } = new Pen(StiColors.Selected);

	    /// <summary>
		/// A defined Pen object with width of 1 and a SelectedText color.
		/// </summary>
		public static Pen SelectedText { get; } = new Pen(StiColors.SelectedText);
	    #endregion
	}
}
