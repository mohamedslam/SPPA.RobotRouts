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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Painters.Context.Animation;
using System.Drawing;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Report.Events
{
	/// <summary>
	/// Represents the method that handles the Painting and Painted events.
	/// </summary>
	public delegate void StiPaintEventHandler(object sender, StiPaintEventArgs e);

	/// <summary>
	/// Describes an argument for the events Painted and Painting.
	/// </summary>
	public class StiPaintEventArgs : 
	    EventArgs, 
	    ICloneable
    {
        #region ICloneable
        public object Clone()
        {
            return base.MemberwiseClone();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets context for painting.
        /// </summary>
        public virtual object Context { get; }

        /// <summary>
		/// Gets Graphics for painting.
		/// </summary>
		public virtual Graphics Graphics
		{
			get 
			{
				return Context as Graphics;
			}
		}

        /// <summary>
		/// Gets the rectangle in which to paint.
		/// </summary>
		public virtual RectangleD ClipRectangle { get; }

        /// <summary>
        /// Gets or sets value indicates that child components must be drawed.
        /// </summary>
        public virtual bool DrawChilds { get; set; } = true;

        /// <summary>
		/// Gets or sets value indicates that drawing is canceled.
		/// </summary>
		public virtual bool Cancel { get; set; }

        /// <summary>
        /// Gets or sets value indicates that formatting will be draw.
        /// </summary>
        public virtual bool DrawBorderFormatting { get; set; } = true;

        /// <summary>
        /// Gets or sets value indicates that topmost border sides will be draw.
        /// </summary>
        public virtual bool DrawTopmostBorderSides { get; set; } = true;

        /// <summary>
        /// Internal use only.
        /// </summary>
        public virtual bool IsPaintThumbnail { get; set; }

        /// <summary>
        /// Internal use only.
        /// </summary>
        internal virtual bool IsPaintWinFormsViewer { get; set; }

        internal StiAnimationEngine AnimationEngine { get; set; }
        #endregion

        /// <summary>
		/// Creates a new object of the type StiPaintEventArgs with specified arguments.
		/// </summary>
        /// <param name="context">Context for painting.</param>
		/// <param name="clipRectangle">The rectangle in which to paint.</param>
        public StiPaintEventArgs(object context, RectangleD clipRectangle)
		{
            this.Context = context;
			this.ClipRectangle = clipRectangle;
		}
	}
}
