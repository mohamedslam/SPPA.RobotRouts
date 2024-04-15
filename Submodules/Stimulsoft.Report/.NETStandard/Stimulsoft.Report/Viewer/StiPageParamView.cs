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
using System.Drawing;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Viewer
{
	/// <summary>
	/// This class is used for keeping parameter pages for preview window.
	/// </summary>
	public class StiPageParamView
	{
	    /// <summary>
		/// Gets or sets page, which parameters describes the object.
		/// </summary>
		public StiPage Page { get; set; }

	    /// <summary>
		/// Gets or sets X coordinate in window of the preview.
		/// </summary>
		public int X { get; set; }

	    /// <summary>
		/// Gets or sets Y coordinate in window of the preview.
		/// </summary>
		public int Y { get; set; }

	    /// <summary>
		/// Gets or sets width in window of the preview.
		/// </summary>
		public int Width { get; set; }

	    /// <summary>
		/// Gets or sets height in window of the preview.
		/// </summary>
		public int Height { get; set; }

	    /// <summary>
		/// Gets or sets number to lines in which is placed page.
		/// </summary>
		public int Line { get; set; }

	    /// <summary>
		/// Gets rectangle, which occupies the page in window of the preview.
		/// </summary>
		public Rectangle DisplayRectangle => new Rectangle(X, Y, Width, Height);

	    /// <summary>
		/// Creates a new object of the type StiPageParamView.
		/// </summary>
		public StiPageParamView()
		{
		}

		/// <summary>
		/// Creates a new object of the type StiPageParamView.
		/// </summary>
		/// <param name="x">X coordinate in window of the preview.</param>
		/// <param name="y">Y coordinate in window of the preview.</param>
		/// <param name="width">Width in window of the preview.</param>
		/// <param name="height">Height in window of the preview.</param>
		/// <param name="line">Number of line in which is placed page.</param>
		public StiPageParamView(int x, int y, int width, int height, int line)
		{
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
			this.Line = line;
		}
	}
}
