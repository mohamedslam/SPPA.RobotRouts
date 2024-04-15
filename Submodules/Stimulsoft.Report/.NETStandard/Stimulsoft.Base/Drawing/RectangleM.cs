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
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Json.Linq;

namespace Stimulsoft.Base.Drawing
{
	/// <summary>
	/// Stores a set of four decimal numbers that represent the location and size of a rectangle.
	/// </summary>
	[TypeConverter(typeof(RectangleMConverter))]
	public struct RectangleM
    {
        #region Properties
        /// <summary>
        /// Gets or sets the x-coordinate of the upper-left corner of this RectangleM structure.
        /// </summary>
        public decimal X { get; set; }

		/// <summary>
        /// Gets or sets the y-coordinate of the upper-left corner of this RectangleM structure.
		/// </summary>
        public decimal Y { get; set; }

		/// <summary>
        /// Gets or sets the width of this RectangleM structure.
		/// </summary>
        public decimal Width { get; set; }
		
		/// <summary>
        /// Gets or sets the height of this RectangleM structure.
		/// </summary>
        public decimal Height { get; set; }

		/// <summary>
        /// Gets or sets the x-coordinate of the upper-left corner of this RectangleM structure.
		/// </summary>
        public decimal Left => X;

        /// <summary>
        /// Gets or sets the y-coordinate of the upper-left corner of this RectangleM structure.
		/// </summary>
        public decimal Top => Y;

        /// <summary>
        /// Gets the x-coordinate of the right edge of this RectangleM structure.
		/// </summary>
        public decimal Right => X + Width;

        /// <summary>
        /// Gets the y-coordinate of the bottom edge of this RectangleM structure.
		/// </summary>
        public decimal Bottom => Y + Height;

        /// <summary>
        /// Tests whether all numeric properties of this RectangleM have values of zero.
		/// </summary>
		public bool IsEmpty => Width == 0 && Height == 0 && X == 0 && Y == 0;

        /// <summary>
        /// Gets or sets the coordinates of the upper-left corner of this RectangleM structure.
		/// </summary>
		public PointM Location => new PointM(X, Y);

        /// <summary>
        /// Gets or sets the size of this RectangleM.
		/// </summary>
		public SizeM Size => new SizeM(Width, Height);
        #endregion

        #region Fields.Static
        /// <summary>
        /// Represents an instance of the RectangleM class with its members uninitialized.
        /// </summary>
        public static RectangleM Empty = new RectangleM(0, 0, 0, 0);
        #endregion

        #region Methods
        public void Inflate(decimal width, decimal height)
		{
            X -= width;
            Y -= height;
            Width += 2 * width;
            Height += 2 * height;
		}

		/// <summary>
        /// Converts the specified RectangleM to a Rectangle.
		/// </summary>
		public Rectangle ToRectangle()
		{
			return new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
		}

		/// <summary>
        /// Converts the specified RectangleM to a RectangleF.
		/// </summary>
		public RectangleF ToRectangleF()
		{
			return new RectangleF((float)X, (float)Y, (float)Width, (float)Height);
		}

        /// <summary>
        /// Converts the specified RectangleM to a RectangleD.
        /// </summary>
        public RectangleD ToRectangleD()
        {
            return new RectangleD((double)X, (double)Y, (double)Width, (double)Height);
        }

		/// <summary>
		/// Normalizes (convert all negative values) rectangle.
		/// </summary>
		/// <returns>Normalized rectangle.</returns>
		public RectangleM Normalize()
		{
			var rect = this;

			if (rect.Width < 0)
			{
				rect.X += rect.Width;
				rect.Width = - rect.Width;
			}

			if (rect.Height < 0)
			{
				rect.Y += rect.Height;
				rect.Height = - rect.Height;
			}

			return rect;
		}

		/// <summary>
		/// Multiplies the rectangle on number.
		/// </summary>
		/// <param name="multipleFactor">Number.</param>
		/// <returns>Multiplied rectangle.</returns>
		public RectangleM Multiply(decimal multipleFactor)
		{
			return new RectangleM(
                X * multipleFactor,
                Y * multipleFactor,
                Width * multipleFactor,
                Height * multipleFactor);
		}

		/// <summary>
		/// Divides rectangle on number.
		/// </summary>
		/// <param name="divideFactor">Number.</param>
		/// <returns>Divided rectangle.</returns>
        public RectangleM Divide(decimal divideFactor)
		{
			return new RectangleM(
                X / divideFactor,
                Y / divideFactor,
                Width / divideFactor,
                Height / divideFactor);
		}

		/// <summary>
        /// Creates the specified RectangleM from a Rectangle.
		/// </summary>
        public static RectangleM CreateFromRectangle(Rectangle rect)
		{
            return new RectangleM(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>
        /// Creates the specified RectangleM from a RectangleF.
		/// </summary>
        public static RectangleM CreateFromRectangle(RectangleF rect)
		{
            return new RectangleM((decimal)rect.X, (decimal)rect.Y, (decimal)rect.Width, (decimal)rect.Height);
		}

		/// <summary>
		/// Changes the sizes of the rectangle.
		/// </summary>
		/// <param name="offsettingRectangle">Data for change the size.</param>
		/// <returns>Changed rectangle.</returns>
        public RectangleM OffsetSize(RectangleM offsettingRectangle)
		{
            return new RectangleM(
				X + offsettingRectangle.Width,
				Y + offsettingRectangle.Height,
				Width,
				Height);
		}

		/// <summary>
		/// Changes the sizes of the rectangle.
		/// </summary>
		/// <param name="offsettingRectangle">Data for change the size.</param>
		/// <returns>Changed rectangle.</returns>
        public RectangleM OffsetRect(RectangleM offsettingRectangle)
		{
            return new RectangleM(
				X - offsettingRectangle.X,
				Y - offsettingRectangle.Y,
				Width + offsettingRectangle.Width,
				Height + offsettingRectangle.Height);
		}

		/// <summary>
		/// Determines if this rectangle intersects with rect.
		/// </summary>
        public bool IntersectsWith(RectangleM rect)
		{
			var rectX = Math.Round(rect.X, 2);
			var rectY = Math.Round(rect.Y, 2);
			var rectRight = Math.Round(rect.Right, 2);
			var rectBottom = Math.Round(rect.Bottom, 2);
			var thisX = Math.Round(X, 2);
			var thisY = Math.Round(Y, 2);
			var thisRight = Math.Round(Right, 2);
			var thisBottom = Math.Round(Bottom, 2);

			return
				rectX < thisRight && 
				rectY < thisBottom &&
				rectRight > thisX && 
				rectBottom > thisY;
		}
		
		/// <summary>
		/// Align the rectangle to grid.
		/// </summary>
		/// <param name="gridSize">Grid size.</param>
		/// <param name="aligningToGrid">Align or no.</param>
		/// <returns>Aligned rectangle.</returns>
        public RectangleM AlignToGrid(decimal gridSize, bool aligningToGrid)
		{
		    if (aligningToGrid)
		    {
		        return new RectangleM(
		            Math.Round((X / gridSize)) * gridSize,
		            Math.Round((Y / gridSize)) * gridSize,
		            Math.Round((Width / gridSize)) * gridSize,
		            Math.Round((Height / gridSize)) * gridSize);
		    }
            
		    return new RectangleM(Left, Top, Width, Height);
		}
	
		/// <summary>
		/// Fit rectangle to rectangle.
		/// </summary>
		/// <param name="rectangle">Rectangle, which will be fited.</param>
		/// <returns>Result rectangle.</returns>
        public RectangleM FitToRectangle(RectangleM rectangle)
		{
			if (IsEmpty) return rectangle;
			if (rectangle.IsEmpty)return this;

            var rectangle2 = this;

			if (rectangle2.Left > rectangle.Left)
			{
				rectangle2.Width += rectangle2.Left - rectangle.Left;
				rectangle2.X = rectangle.Left;
			}

			if (rectangle2.Top > rectangle.Top)
			{
				rectangle2.Height += rectangle2.Top - rectangle.Top;
				rectangle2.Y = rectangle.Top;
			}

		    if (rectangle2.Right < rectangle.Right)
		        rectangle2.Width += rectangle.Right - rectangle2.Right;

		    if (rectangle2.Bottom < rectangle.Bottom)
		        rectangle2.Height += rectangle.Bottom - rectangle2.Bottom;

		    return rectangle2;
		}

		/// <summary>
        /// Determines if the specified point is contained within this RectangleM structure.
		/// </summary>
		/// <param name="pt">The PointM to test.</param>
        /// <returns>This method returns true if the point represented by the pt parameter is contained within this RectangleM structure; otherwise false.</returns>
        public bool Contains(PointM pt)
		{
			return Contains(pt.X, pt.Y); 
		} 

		/// <summary>
        /// Determines if the rectangular region represented by rect is entirely contained within this RectangleM structure.
		/// </summary>
        /// <param name="rect">The RectangleM to test.</param>
        /// <returns>This method returns true if the rectangular region represented by rect is entirely contained within the rectangular region represented by this RectangleM; otherwise false.</returns>
        public bool Contains(RectangleM rect)
		{
		    return X < rect.X && rect.X + rect.Width < X + Width && Y < rect.Y && !(rect.Y + rect.Height > Y + Height);
		} 

		/// <summary>
        /// Determines if the specified point is contained within this RectangleM structure.
		/// </summary>
		/// <param name="x">The x-coordinate of the point to test.</param>
		/// <param name="y">The y-coordinate of the point to test.</param>
        /// <returns>This method returns true if the point defined by x and y is contained within this RectangleM structure; otherwise false.</returns>
        public bool Contains(decimal x, decimal y)
		{
		    return X <= x && x < X + Width && Y <= y && y < Y + Height;
		}

        public static RectangleM Intersect(RectangleM a, RectangleM b)
		{
            var num1 = Math.Max(a.X, b.X);
			var num2 = Math.Min(a.X + a.Width, b.X + b.Width);
			var num3 = Math.Max(a.Y, b.Y);
			var num4 = Math.Min(a.Y + a.Height, b.Y + b.Height);

		    return num2 >= num1 && num4 >= num3 
		        ? new RectangleM(num1, num3, num2 - num1, num4 - num3) 
		        : Empty;
		}

        public void Intersect(RectangleM rect)
		{
            rect = Intersect(rect, this);
            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
		}

		/// <summary>
        /// Tests whether obj is a RectangleF with the same location and size of this RectangleM.
		/// </summary>
		/// <param name="obj">The Object to test.</param>
        /// <returns>This method returns true if obj is a RectangleM and its X, Y, Width, and Height properties are equal to the corresponding properties of this RectangleM; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
            var rect = (RectangleM)obj;
			return rect.X == X && rect.Y == Y && rect.Width == Width && rect.Height == Height;
		}

		/// <summary>
        /// Returns a hash code for this RectangleM structure.
		/// </summary>
        /// <returns>An integer value that specifies a hash value for this RectangleM structure.</returns>
		public override int GetHashCode()
		{
			return (int)X ^ (((int)Y << 13) | 
			        ((int)Y >> 19)) ^ (((int)Width << 26) | 
			        ((int)Width >> 6)) ^ 
			       (((int)Height << 7) | ((int)Height >> 25)); 
		}

        public override string ToString()
        {
            return $"X={X} Y={Y} W={Width} H={Height}";
        }
        #endregion

        #region Methods.Json
        public void LoadFromJson(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "X":
                        X = property.DeserializeDecimal();
                        break;

                    case "Y":
                        Y = property.DeserializeDecimal();
                        break;

                    case "Width":
                        Width = property.DeserializeDecimal();
                        break;

                    case "Height":
                        Height = property.DeserializeDecimal();
                        break;
                }
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the RectangleM class with the specified location and size.
        /// </summary>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        public RectangleM(decimal x, decimal y, decimal width, decimal height) : this()
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
 		} 


		/// <summary>
        /// Initializes a new instance of the RectangleM class with the specified location and size.
		/// </summary>
        /// <param name="location">A PointM that represents the upper-left corner of the rectangular region.</param>
        /// <param name="size">A SizeM that represents the width and height of the rectangular region.</param>
		public RectangleM(PointM location, SizeM size) : this()
		{
            X = location.X;
            Y = location.Y;
            Width = size.Width;
            Height = size.Height;
		} 

	}
}
