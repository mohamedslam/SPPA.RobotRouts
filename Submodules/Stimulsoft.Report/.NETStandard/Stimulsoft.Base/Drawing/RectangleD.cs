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
using Stimulsoft.Base.Json.Linq;

namespace Stimulsoft.Base.Drawing
{
	/// <summary>
	/// Stores a set of four double-point numbers that represent the location and size of a rectangle.
	/// </summary>
	[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.RectangleDConverter))]
	public struct RectangleD
    {
        #region Properties
        /// <summary>
		/// Gets or sets the x-coordinate of the upper-left corner of this RectangleD structure.
		/// </summary>
		public double X { get; set; }

        /// <summary>
		/// Gets or sets the y-coordinate of the upper-left corner of this RectangleD structure.
		/// </summary>
		public double Y { get; set; }

        /// <summary>
		/// Gets or sets the width of this RectangleD structure.
		/// </summary>
		public double Width { get; set; }

        /// <summary>
		/// Gets or sets the height of this RectangleD structure.
		/// </summary>
		public double Height { get; set; }

        /// <summary>
		/// Gets or sets the x-coordinate of the upper-left corner of this RectangleD structure.
		/// </summary>
		public double Left => X;

        /// <summary>
        /// Gets or sets the y-coordinate of the upper-left corner of this RectangleD structure.
        /// </summary>
        public double Top => Y;

        /// <summary>
        /// Gets the x-coordinate of the right edge of this RectangleD structure.
        /// </summary>
        public double Right => X + Width;

        /// <summary>
        /// Gets the y-coordinate of the bottom edge of this RectangleD structure.
        /// </summary>
        public double Bottom => Y + Height;

        /// <summary>
        /// Tests whether all numeric properties of this RectangleD have values of zero.
        /// </summary>
        public bool IsEmpty => Width == 0 && Height == 0 && X == 0 && Y == 0;

        /// <summary>
        /// Gets or sets the coordinates of the upper-left corner of this RectangleD structure.
        /// </summary>
        public PointD Location => new PointD(X, Y);

        /// <summary>
        /// Gets or sets the size of this RectangleD.
        /// </summary>
        public SizeD Size => new SizeD(Width, Height);
        #endregion

        #region Fields.Static
        /// <summary>
        /// Represents an instance of the RectangleD class with its members uninitialized.
        /// </summary>
        public static RectangleD Empty = new RectangleD(0, 0, 0, 0);
		#endregion

		#region Methods
		public static bool operator !=(RectangleD left, RectangleD right)
		{
			return (left.X != right.X ||
				left.Y != right.Y ||
				left.Width != right.Width ||
				left.Height != right.Height);
		}

		public static bool operator ==(RectangleD left, RectangleD right)
        {
			return (left.X == right.X &&
				left.Y == right.Y &&
				left.Width == right.Width &&
				left.Height == right.Height);
        }

		public static RectangleD Ceiling(RectangleD value)
        {
            return new RectangleD(Math.Ceiling(value.X), Math.Ceiling(value.Y), Math.Ceiling(value.Width), Math.Ceiling(value.Height));
        }

        public static RectangleD Round(RectangleD value)
        {
            return new RectangleD(Math.Round(value.X), Math.Round(value.Y), Math.Round(value.Width), Math.Round(value.Height));
        }

        public static RectangleD Union(RectangleD a, RectangleD b)
        {
            var x = Math.Min(a.X, b.X);
            var num1 = Math.Max(a.X + a.Width, b.X + b.Width);
            var y = Math.Min(a.Y, b.Y);
            var num2 = Math.Max(a.Y + a.Height, b.Y + b.Height);
            return new RectangleD(x, y, num1 - x, num2 - y);
        }

        public void Inflate(double width, double height)
		{
            X -= width;
            Y -= height;
            Width += 2 * width;
            Height += 2 * height;
		}
        
		/// <summary>
		/// Converts the specified RectangleD to a Rectangle.
		/// </summary>
		public Rectangle ToRectangle()
		{
			return new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
		}

		/// <summary>
		/// Converts the specified RectangleD to a RectangleF.
		/// </summary>
		public RectangleF ToRectangleF()
		{
			return new RectangleF((float)X, (float)Y, (float)Width, (float)Height);
		}

        public RectangleM ToRectangleM()
        {
            return new RectangleM((decimal)X, (decimal)Y, (decimal)Width, (decimal)Height);
        }

		/// <summary>
		/// Normalizes (convert all negative values) rectangle.
		/// </summary>
		/// <returns>Normalized rectangle.</returns>
		public RectangleD Normalize()
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
		public RectangleD Multiply(double multipleFactor)
		{
			return new RectangleD(
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
		public RectangleD Divide(double divideFactor)
		{
			return new RectangleD(
                X / divideFactor,
                Y / divideFactor,
                Width / divideFactor,
                Height / divideFactor);
		}

		/// <summary>
		/// Creates the specified RectangleD from a Rectangle.
		/// </summary>
		public static RectangleD CreateFromRectangle(Rectangle rect)
		{
			return new RectangleD(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>
		/// Creates the specified RectangleD from a RectangleF.
		/// </summary>
		public static RectangleD CreateFromRectangle(RectangleF rect)
		{
			return new RectangleD(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>
		/// Changes the sizes of the rectangle.
		/// </summary>
		/// <param name="offsettingRectangle">Data for change the size.</param>
		/// <returns>Changed rectangle.</returns>
		public RectangleD OffsetSize(RectangleD offsettingRectangle)
		{
			return new RectangleD(
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
		public RectangleD OffsetRect(RectangleD offsettingRectangle)
		{
			return new RectangleD(
				X - offsettingRectangle.X,
				Y - offsettingRectangle.Y,
				Width + offsettingRectangle.Width,
				Height + offsettingRectangle.Height);
		}
        
		/// <summary>
		/// Determines if this rectangle intersects with rect.
		/// </summary>
		public bool IntersectsWith(RectangleD rect)
		{
			var rectX = Math.Round((decimal)rect.X, 2);
			var rectY = Math.Round((decimal)rect.Y, 2);
			var rectRight = Math.Round((decimal)rect.Right, 2);
			var rectBottom = Math.Round((decimal)rect.Bottom, 2);
			var thisX = Math.Round((decimal)X, 2);
			var thisY = Math.Round((decimal)Y, 2);
			var thisRight = Math.Round((decimal)Right, 2);
			var thisBottom = Math.Round((decimal)Bottom, 2);
			
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
		public RectangleD AlignToGrid(double gridSize, bool aligningToGrid)
		{
		    if (aligningToGrid)
		    {
		        return new RectangleD(
		            Math.Round(X / gridSize) * gridSize,
		            Math.Round(Y / gridSize) * gridSize,
		            Math.Round(Width / gridSize) * gridSize,
		            Math.Round(Height / gridSize) * gridSize);
		    }

		    return new RectangleD(Left, Top, Width, Height);
		}
	
		/// <summary>
		/// Fit rectangle to rectangle.
		/// </summary>
		/// <param name="rectangle">Rectangle, which will be fited.</param>
		/// <returns>Result rectangle.</returns>
		public RectangleD FitToRectangle(RectangleD rectangle)
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
			{
				rectangle2.Width += rectangle.Right - rectangle2.Right;
			}
			
			if (rectangle2.Bottom < rectangle.Bottom)
			{
				rectangle2.Height += rectangle.Bottom - rectangle2.Bottom;
			}
			
			return rectangle2;
		}

		/// <summary>
		/// Determines if the specified point is contained within this RectangleD structure.
		/// </summary>
		/// <param name="pt">The PointD to test.</param>
		/// <returns>This method returns true if the point represented by the pt parameter is contained within this RectangleD structure; otherwise false.</returns>
		public bool Contains(PointD pt)
		{
			return Contains(pt.X, pt.Y); 
		} 

		/// <summary>
		/// Determines if the rectangular region represented by rect is entirely contained within this RectangleD structure.
		/// </summary>
		/// <param name="rect">The RectangleD to test.</param>
		/// <returns>This method returns true if the rectangular region represented by rect is entirely contained within the rectangular region represented by this RectangleD; otherwise false.</returns>
		public bool Contains(RectangleD rect)
		{
		    return X < rect.X && rect.X + rect.Width < X + Width && Y < rect.Y && !(rect.Y + rect.Height > Y + Height);
		} 

		/// <summary>
		/// Determines if the specified point is contained within this RectangleD structure.
		/// </summary>
		/// <param name="x">The x-coordinate of the point to test.</param>
		/// <param name="y">The y-coordinate of the point to test.</param>
		/// <returns>This method returns true if the point defined by x and y is contained within this RectangleD structure; otherwise false.</returns>
		public bool Contains(double x, double y)
		{
		    return X <= x && x < X + Width && Y <= y && y < Y + Height;
		}

		public static RectangleD Intersect(RectangleD a, RectangleD b)
		{
			var num1 = Math.Max(a.X, b.X);
			var num2 = Math.Min(a.X + a.Width, b.X + b.Width);
			var num3 = Math.Max(a.Y, b.Y);
			var num4 = Math.Min(a.Y + a.Height, b.Y + b.Height);
			
			if ((num2 >= num1) && (num4 >= num3))
			{
				return new RectangleD(num1, num3, num2 - num1, num4 - num3);
			}
			return Empty;
		}

		public void Intersect(RectangleD rect)
		{
			rect = Intersect(rect, this);

            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
		}

		/// <summary>
		/// Tests whether obj is a RectangleF with the same location and size of this RectangleD.
		/// </summary>
		/// <param name="obj">The Object to test.</param>
		/// <returns>This method returns true if obj is a RectangleD and its X, Y, Width, and Height properties are equal to the corresponding properties of this RectangleD; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			var rect = (RectangleD)obj;
			return rect.X == X && rect.Y == Y && rect.Width == Width && rect.Height == Height;
		}

		/// <summary>
		/// Returns a hash code for this RectangleD structure.
		/// </summary>
		/// <returns>An integer value that specifies a hash value for this RectangleD structure.</returns>
		public override int GetHashCode()
		{
			return (((((int)X) ^ ((((int)Y) << 13) | 
				(((int)Y) >> 19))) ^ ((((int)Width) << 26) | 
				(((int)Width) >> 6))) ^ 
				((((int)Height) << 7) | (((int)Height) >> 25))); 
		}

        public override string ToString()
        {
            return $"X={(decimal)X} Y={(decimal)Y} W={(decimal)Width} H={(decimal)Height}";
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
                        X = property.DeserializeDouble();
                        break;

                    case "Y":
                        Y = property.DeserializeDouble();
                        break;

                    case "Width":
                        Width = property.DeserializeDouble();
                        break;

                    case "Height":
                        Height = property.DeserializeDouble();
                        break;
                }
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the RectangleD class with the specified location and size.
        /// </summary>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        public RectangleD(double x, double y, double width, double height)
		{
            X = x;
            Y = y;
            Width = width;
            Height = height;
 		} 


		/// <summary>
		/// Initializes a new instance of the RectangleD class with the specified location and size.
		/// </summary>
		/// <param name="location">A PointD that represents the upper-left corner of the rectangular region.</param>
		/// <param name="size">A SizeD that represents the width and height of the rectangular region.</param>
		public RectangleD(PointD location, SizeD size)
		{
            X = location.X;
            Y = location.Y;
            Width = size.Width;
            Height = size.Height;
		} 
	}
}
