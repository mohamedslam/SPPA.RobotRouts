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

namespace Stimulsoft.Base.Drawing
{
	/// <summary>
	/// Describes a class that contains methods which serve for operation with a rectangle and cursor.
	/// </summary>
	public sealed class StiActionUtils
	{
		/// <summary>
		/// Checks is this point gets into the specified corner.
		/// </summary>
		/// <param name="x">X point.</param>
		/// <param name="y">Y point.</param>
		/// <param name="point">Angle coordinates.</param>
		/// <param name="size">Tolerable limit size.</param>
		/// <returns>True, if is this point gets into the specified corner, and false, if not.</returns>
		public static bool PointInEdge(double x, double y, PointD point, double size)
		{
			var pointX = Math.Round((decimal)point.X, 2);
			var pointY = Math.Round((decimal)point.Y, 2);
			var xx = Math.Round((decimal)x, 2);
			var yy = Math.Round((decimal)y, 2);
			var sizeD = Math.Round((decimal)size, 2);

			return pointX - sizeD <= xx && pointY - sizeD <= yy && pointX + sizeD >= xx && pointY + sizeD >= yy;
		}
		
		/// <summary>
		/// Returns the action fits to the position of a point in the specified rectangle.
		/// </summary>
		/// <param name="size">Tolerable limit size.</param>
		/// <param name="x">X point.</param>
		/// <param name="y">Y point.</param>
		/// <param name="sizeRect">Rectangle of sizes.</param>
		/// <param name="selectRect">Rectangle of selection.</param>
		/// <param name="isSelected">Indicates, is this rectangle selected or not?</param>
		/// <param name="locked">Indicates, is this ectangle locked or not?</param>
		/// <returns>Action.</returns>
		public static StiAction PointInRect(int size, int x, int y, 
			Rectangle sizeRect, Rectangle selectRect, bool isSelected, bool locked)
		{
			return PointInRect(size, x, y, 
				RectangleD.CreateFromRectangle(sizeRect),
				RectangleD.CreateFromRectangle(selectRect), 
				isSelected, locked);
		}
	
		/// <summary>
		/// Returns the action fits to the position of a point in the specified rectangle.
		/// </summary>
		/// <param name="size">Tolerable limit size.</param>
		/// <param name="x">X point.</param>
		/// <param name="y">Y point.</param>
		/// <param name="sizeRect">Rectangle of sizes.</param>
		/// <param name="selectRect">Rectangle of selection.</param>
		/// <param name="isSelected">Indicates, is this rectangle selected or not?</param>
		/// <param name="locked">Indicates, is this ectangle locked or not?</param>
		/// <returns>Action.</returns>
		public static StiAction PointInRect(float size, float x, float y, 
			RectangleF sizeRect, RectangleF selectRect, bool isSelected, bool locked)
		{
			return PointInRect(size, x, y, 
				RectangleD.CreateFromRectangle(sizeRect),
				RectangleD.CreateFromRectangle(selectRect), 
				isSelected, locked);
		}

		/// <summary>
		/// Returns the action fits to the position of a point in the specified rectangle.
		/// </summary>
		/// <param name="size">Tolerable limit size.</param>
		/// <param name="x">X point.</param>
		/// <param name="y">Y point.</param>
		/// <param name="sizeRect">Rectangle of sizes.</param>
		/// <param name="selectRect">Rectangle of selection.</param>
		/// <param name="isSelected">Indicates, is this rectangle selected or not?</param>
		/// <param name="locked">Indicates, is this ectangle locked or not?</param>
		/// <returns>Action.</returns>
		public static StiAction PointInRect(double size, double x, double y, 
			RectangleD sizeRect, RectangleD selectRect, bool isSelected, bool locked)
		{
			if (isSelected && locked == false)
			{
			    //SizeLeftTop
				if (PointInEdge(x, y, new PointD(sizeRect.X, sizeRect.Y), size))
					return StiAction.SizeLeftTop;
					
					//SizeRightTop
			    if (PointInEdge(x, y, new PointD(sizeRect.X + sizeRect.Width, sizeRect.Y), size))
			        return StiAction.SizeRightTop;
				
			    //SizeLeftBottom
			    if (PointInEdge(x, y, new PointD(sizeRect.X, sizeRect.Y + sizeRect.Height), size))
			        return StiAction.SizeLeftBottom;
				
			    //SizeRightBottom
			    if (PointInEdge(x, y, new PointD(sizeRect.X + sizeRect.Width, sizeRect.Y + sizeRect.Height), size))
			        return StiAction.SizeRightBottom;
				
			    //SizeTop
			    if (PointInEdge(x, y, new PointD(sizeRect.X + sizeRect.Width/2, sizeRect.Y), size))
			        return StiAction.SizeTop;
				
			    //SizeBottom
			    if (PointInEdge(x, y, new PointD(sizeRect.X + sizeRect.Width/2, sizeRect.Y + sizeRect.Height), size))
			        return StiAction.SizeBottom;

			    //SizeLeft
			    if (PointInEdge(x, y, new PointD(sizeRect.X, sizeRect.Y + sizeRect.Height/2), size))
			        return StiAction.SizeLeft;
				
			    //SizeRight
			    if (PointInEdge(x, y, new PointD(sizeRect.X + sizeRect.Width, sizeRect.Y + sizeRect.Height/2), size))
			        return StiAction.SizeRight;
			}
			
			var selectRectLeft = Math.Round((decimal)selectRect.Left, 2);
			var selectRectTop = Math.Round((decimal)selectRect.Top, 2);
			var selectRectRight = Math.Round((decimal)selectRect.Right, 2);
			var selectRectBottom = Math.Round((decimal)selectRect.Bottom, 2);

			var xx = Math.Round((decimal)x, 2);
			var yy = Math.Round((decimal)y, 2);

			if ((selectRectLeft <= xx) & (selectRectTop <= yy) & 
				(selectRectBottom >= yy) & (selectRectRight >= xx))
				return StiAction.Move;
						
			return StiAction.None;
		}

        /// <summary>
        /// Returns the action fits to the position of a point in the specified rectangle.
        /// </summary>
        public static StiAction PointInRect(double size, double x, double y,
            RectangleD sizeRect, bool isSelected)
        {
            if (isSelected)
            {
                //SizeLeftTop
                if (PointInEdge(x, y, new PointD(sizeRect.X, sizeRect.Y), size))
                    return StiAction.SizeLeftTop;

                //SizeRightTop
                if (PointInEdge(x, y, new PointD(sizeRect.X + sizeRect.Width, sizeRect.Y), size))
                    return StiAction.SizeRightTop;

                //SizeLeftBottom
                if (PointInEdge(x, y, new PointD(sizeRect.X, sizeRect.Y + sizeRect.Height), size))
                    return StiAction.SizeLeftBottom;

                //SizeRightBottom
                if (PointInEdge(x, y, new PointD(sizeRect.X + sizeRect.Width, sizeRect.Y + sizeRect.Height), size))
                    return StiAction.SizeRightBottom;

                //SizeTop
                if (PointInEdge(x, y, new PointD(sizeRect.X + sizeRect.Width / 2, sizeRect.Y), size))
                    return StiAction.SizeTop;

                //SizeBottom
                if (PointInEdge(x, y, new PointD(sizeRect.X + sizeRect.Width / 2, sizeRect.Y + sizeRect.Height), size))
                    return StiAction.SizeBottom;

                //SizeLeft
                if (PointInEdge(x, y, new PointD(sizeRect.X, sizeRect.Y + sizeRect.Height / 2), size))
                    return StiAction.SizeLeft;

                //SizeRight
                if (PointInEdge(x, y, new PointD(sizeRect.X + sizeRect.Width, sizeRect.Y + sizeRect.Height / 2), size))
                    return StiAction.SizeRight;
            }

            var selectRectLeft = Math.Round((decimal)sizeRect.Left, 2);
            var selectRectTop = Math.Round((decimal)sizeRect.Top, 2);
            var selectRectRight = Math.Round((decimal)sizeRect.Right, 2);
            var selectRectBottom = Math.Round((decimal)sizeRect.Bottom, 2);

            var xx = Math.Round((decimal)x, 2);
            var yy = Math.Round((decimal)y, 2);

            if ((selectRectLeft <= xx) & (selectRectTop <= yy) &
                (selectRectBottom >= yy) & (selectRectRight >= xx))
                return StiAction.Move;

            return StiAction.None;
        }

        /// <summary>
        /// Checks does the point fit into the position.
        /// </summary>
        /// <param name="x">X point.</param>
        /// <param name="y">Y point.</param>
        /// <param name="rect"></param>
        /// <returns>true, if the point fits to the position, and false, if not.</returns>
        public static bool PointInRect(float x, float y, RectangleF rect)
		{
			return PointInRect(x, y, RectangleD.CreateFromRectangle(rect));
		}

		/// <summary>
		/// Checks does the point fit into the position.
		/// </summary>
		/// <param name="x">X point.</param>
		/// <param name="y">Y point.</param>
		/// <param name="rect"></param>
		/// <returns>true, if the point fits to the position, and false, if not.</returns>
		public static bool PointInRect(double x, double y, RectangleD rect)
		{
			var rectLeft = Math.Round((decimal)rect.Left, 2);
			var rectTop = Math.Round((decimal)rect.Top, 2);
			var rectRight = Math.Round((decimal)rect.Right, 2);
			var rectBottom = Math.Round((decimal)rect.Bottom, 2);

			var xx = Math.Round((decimal)x, 2);
			var yy = Math.Round((decimal)y, 2);

			return rectLeft <= xx & rectTop <= yy & rectBottom > yy & rectRight > xx;
		}
	}
}
