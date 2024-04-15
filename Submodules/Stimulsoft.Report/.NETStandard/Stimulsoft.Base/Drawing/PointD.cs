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
using System.ComponentModel;

namespace Stimulsoft.Base.Drawing
{
	/// <summary>
	/// Represents an ordered pair of double x- and y-coordinates that defines a point in a two-dimensional plane.
	/// </summary>
	[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.PointDConverter))]
	public struct PointD
	{
        #region Properties
        /// <summary>
        /// Gets or sets the x-coordinate of this PointD.
        /// </summary>
        public double X { get; set; }

	    /// <summary>
		/// Gets or sets the y-coordinate of this PointD.
		/// </summary>
		public double Y { get; set; }

        /// <summary>
        /// Gets a value indicating whether this PointD is empty.
        /// </summary>
        [Browsable(false)]
        public bool IsEmpty => X == 0 && Y == 0;
        #endregion

	    #region Fields.Static
        /// <summary>
        /// Represents a null PointD.
        /// </summary>
        public static PointD Empty = new PointD();
        #endregion

	    #region Methods
        /// <summary>
        /// Converts this PointD to a human readable string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
		{
			return $"{{X={X}, Y={Y}}}"; 
		}

		/// <summary>
		/// Specifies whether this PointD contains the same coordinates as the specified Object.
		/// </summary>
		/// <param name="obj">The Object to test.</param>
		/// <returns>This method returns true if obj is a PointD and has the same coordinates as this Point.</returns>
		public override bool Equals(object obj)
		{
			var point = (PointD)obj;

		    return point.X == X && point.Y == Y;
		}

		/// <summary>
		/// Returns a hash code for this PointD structure.
		/// </summary>
		/// <returns>An integer value that specifies a hash value for this PointD structure.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode(); 
		}

		public PointF ToPointF()
		{
			return new PointF((float)X, (float)Y);
		}

		public Point ToPoint()
		{
			return new Point((int)X, (int)Y);
		}
        #endregion

        /// <summary>
        /// Initializes a new instance of the PointD class with the specified coordinates.
        /// </summary>
        /// <param name="x">The horizontal position of the point.</param>
        /// <param name="y">The vertical position of the point.</param>
        public PointD(double x, double y)
		{
			this.X = x;
			this.Y = y;
		}
	}
}
