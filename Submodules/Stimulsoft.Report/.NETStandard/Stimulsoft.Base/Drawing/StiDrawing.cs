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

using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Brush = Stimulsoft.Drawing.Brush;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Font = Stimulsoft.Drawing.Font;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Pen = Stimulsoft.Drawing.Pen;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Graphics = Stimulsoft.Drawing.Graphics;
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
#endif

namespace Stimulsoft.Base.Drawing
{	
	/// <summary>
	/// Class contains statistical methods for drawing.
	/// </summary>
	public sealed class StiDrawing
	{
		#region Methods
		/// <summary>
		/// Draws a rectangle specified by a coordinate pair, a width, and a height.
		/// </summary>
		/// <param name="g">The Graphics to draw on.</param>
		/// <param name="color">Color to draw Rectangle.</param>
		/// <param name="x">The x-coordinate of the upper-left corner of the rectangle to draw.</param>
		/// <param name="y">The y-coordinate of the upper-left corner of the rectangle to draw.</param>
		/// <param name="width">The width of the rectangle to draw.</param>
		/// <param name="height">The height of the rectangle to draw.</param>
		public static void DrawRectangle(Graphics g, Color color, double x, double y, double width, double height)
		{
			using (var pen = new Pen(color))
				DrawRectangle(g, pen, new RectangleD(x, y, width, height));
		}
		
		/// <summary>
		/// Draws a rectangle specified by a RectangleD structure.
		/// </summary>
		/// <param name="g">The Graphics to draw on.</param>
		/// <param name="color">Color to draw Rectangle.</param>
		/// <param name="rect">A Rectangle structure that represents the rectangle to draw.</param>
		public static void DrawRectangle(Graphics g, Color color, RectangleD rect)
		{
			using (var pen = new Pen(color))
				DrawRectangle(g, pen, rect);
		}

		/// <summary>
		/// Draws a rectangle specified by a RectangleF structure.
		/// </summary>
		/// <param name="g">The Graphics to draw on.</param>
		/// <param name="color">Color to draw RectangleF.</param>
		/// <param name="rect">A Rectangle structure that represents the rectangle to draw.</param>
		public static void DrawRectangle(Graphics g, Color color, RectangleF rect)
		{
			using (var pen = new Pen(color))
				DrawRectangle(g, pen, rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>
		/// Draws a rectangle specified by a Rectangle structure.
		/// </summary>
		/// <param name="g">The Graphics to draw on.</param>
		/// <param name="color">Color to draw Rectangle.</param>
		/// <param name="rect">A Rectangle structure that represents the rectangle to draw.</param>
		public static void DrawRectangle(Graphics g, Color color, Rectangle rect)
		{
			using (var pen = new Pen(color))
				DrawRectangle(g, pen, rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>
		/// Draws a rectangle specified by a coordinate pair, a width, and a height.
		/// </summary>
		/// <param name="g">The Graphics to draw on.</param>
		/// <param name="pen">A Pen object that determines the color, width, and style of the rectangle.</param>
		/// <param name="x">The x-coordinate of the upper-left corner of the rectangle to draw.</param>
		/// <param name="y">The y-coordinate of the upper-left corner of the rectangle to draw.</param>
		/// <param name="width">The width of the rectangle to draw.</param>
		/// <param name="height">The height of the rectangle to draw.</param>
		public static void DrawRectangle(Graphics g, Pen pen, double x, double y, double width, double height)
		{
			DrawRectangle(g, pen, new RectangleD(x, y, width, height));
		}

		/// <summary>
		/// Draws a rectangle specified by a Rectangle structure.
		/// </summary>
		/// <param name="g">The Graphics to draw on.</param>
		/// <param name="pen">A Pen object that determines the color, width, and style of the rectangle.</param>
		/// <param name="rect">A Rectangle structure that represents the rectangle to draw.</param>
		public static void DrawRectangle(Graphics g, Pen pen, RectangleD rect)
		{
			g.DrawRectangle(pen, (float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
		}
		
		/// <summary>
		/// Fills the interior of a rectangle specified by a pair of coordinates, a width, and a height.
		/// </summary>
		/// <param name="g">The Graphics to draw on.</param>
		/// <param name="color">Color to fill.</param>
		/// <param name="x">x-coordinate of the upper-left corner of the rectangle to fill.</param>
		/// <param name="y">y-coordinate of the upper-left corner of the rectangle to fill.</param>
		/// <param name="width">Width of the rectangle to fill.</param>
		/// <param name="height">Height of the rectangle to fill.</param>
		public static void FillRectangle(Graphics g, Color color, double x, double y, double width, double height)
		{
			FillRectangle(g, color, new RectangleD(x, y, width, height));
		}
		
		/// <summary>
		/// Fills the interior of a rectangle specified by a RectangleD structure.
		/// </summary>
		/// <param name="g">The Graphics to draw on.</param>
		/// <param name="color">Color to fill.</param>
		/// <param name="rect">RectangleD structure that represents the rectangle to fill.</param>
		public static void FillRectangle(Graphics g, Color color, RectangleD rect)
		{
		    if (color == Color.Transparent) return;

		    using (Brush brush = new SolidBrush(color))
		    {
		        FillRectangle(g, brush, rect);
		    }
		}

	    /// <summary>
	    /// Fills the interior of a rectangle specified by a RectangleD structure.
	    /// </summary>
	    /// <param name="g">The Graphics to draw on.</param>
	    /// <param name="color">Color to fill.</param>
	    /// <param name="rect">RectangleD structure that represents the rectangle to fill.</param>
	    public static void FillRectangle(Graphics g, Color color, RectangleF rect)
	    {
	        if (color == Color.Transparent) return;

	        using (Brush brush = new SolidBrush(color))
	        {
	            FillRectangle(g, brush, rect);
	        }
	    }

        /// <summary>
        /// Fills the interior of a rectangle specified by a pair of coordinates, a width, and a height.
        /// </summary>
        /// <param name="g">The Graphics to draw on.</param>
        /// <param name="brush">StiBrush object that determines the characteristics of the fill.</param>
        /// <param name="x">x-coordinate of the upper-left corner of the rectangle to fill.</param>
        /// <param name="y">y-coordinate of the upper-left corner of the rectangle to fill.</param>
        /// <param name="width">Width of the rectangle to fill.</param>
        /// <param name="height">Height of the rectangle to fill.</param>
        public static void FillRectangle(Graphics g, StiBrush brush, double x, double y, double width, double height)
		{
			FillRectangle(g, brush, new RectangleD(x, y, width, height));
		}
		
		/// <summary>
		/// Fills the interior of a rectangle specified by a RectangleD structure.
		/// </summary>
		/// <param name="g">The Graphics to draw on.</param>
		/// <param name="brush">StiBrush object that determines the characteristics of the fill.</param>
		/// <param name="rect">RectangleD structure that represents the rectangle to fill.</param>
		public static void FillRectangle(Graphics g, StiBrush brush, RectangleD rect)
		{
			if (brush == null) return;

            using (var br = StiBrush.GetBrush(brush, rect))
            {
                var gradient = brush as StiGradientBrush;
                if (StiBaseOptions.FixBandedGradients && (g.PageUnit != GraphicsUnit.Display) && (gradient != null) && (gradient.Angle != 0) && (gradient.Angle != 180))
                {
                    var imgSize = new[] { new PointF((float)rect.Width, (float)rect.Height) };
                    g.TransformPoints(CoordinateSpace.Page, CoordinateSpace.Device, imgSize);

                    using (var bmp = new Bitmap((int)(imgSize[0].X / g.PageScale), (int)(imgSize[0].Y / g.PageScale), g))
                    {
                        using (var gb = Graphics.FromImage(bmp))
                        {
                            gb.PageUnit = g.PageUnit;
                            gb.PageScale = g.PageScale;
                            gb.TranslateTransform(-(float)rect.Left, -(float)rect.Top);
                            FillRectangle(gb, br, new RectangleD(rect.Left, rect.Top, rect.Width, rect.Height));
                        }
                        g.DrawImage(bmp, (float)rect.Left, (float)rect.Top);
                    }
                }
                else
                {
                    FillRectangle(g, br, rect);
                }
            }
		}

		/// <summary>
		/// Fills the interior of a rectangle specified by a RectangleF structure.
		/// </summary>
		/// <param name="g">The Graphics to draw on.</param>
		/// <param name="brush">StiBrush object that determines the characteristics of the fill.</param>
		/// <param name="rect">RectangleD structure that represents the rectangle to fill.</param>
		public static void FillRectangle(Graphics g, StiBrush brush, RectangleF rect)
		{
			if (brush == null) return;

			using (var br = StiBrush.GetBrush(brush, rect))
			{
                var gradient = brush as StiGradientBrush;
                if (StiBaseOptions.FixBandedGradients && g.PageUnit != GraphicsUnit.Display && gradient != null && gradient.Angle != 0 && gradient.Angle != 180)
                {
                    var imgSize = new[] { new PointF(rect.Width, rect.Height) };
                    g.TransformPoints(CoordinateSpace.Page, CoordinateSpace.Device, imgSize);

                    using (var bmp = new Bitmap((int)(imgSize[0].X / g.PageScale), (int)(imgSize[0].Y / g.PageScale), g))
                    using (var gb = Graphics.FromImage(bmp))
                    {
                        gb.PageUnit = g.PageUnit;
                        gb.PageScale = g.PageScale;
                        gb.TranslateTransform(-rect.Left, -rect.Top);
                        FillRectangle(gb, br, new RectangleD(rect.Left, rect.Top, rect.Width, rect.Height));
                        g.DrawImage(bmp, rect.Left, rect.Top);
                    }
                }
                else
                {
                    FillRectangle(g, br, rect);
                }
            }
		}
		
		/// <summary>
		/// Fills the interior of a rectangle specified by a pair of coordinates, a width, and a height.
		/// </summary>
		/// <param name="g">The Graphics to draw on.</param>
		/// <param name="brush">Brush object that determines the characteristics of the fill.</param>
		/// <param name="x">x-coordinate of the upper-left corner of the rectangle to fill.</param>
		/// <param name="y">y-coordinate of the upper-left corner of the rectangle to fill.</param>
		/// <param name="width">Width of the rectangle to fill.</param>
		/// <param name="height">Height of the rectangle to fill.</param>
		public static void FillRectangle(Graphics g, Brush brush, double x, double y, double width, double height)
		{
			FillRectangle(g, brush, new RectangleD(x, y, width, height));
		}
		
		/// <summary>
		/// Fills the interior of a rectangle specified by a RectangleD structure.
		/// </summary>
		/// <param name="g">The Graphics to draw on.</param>
		/// <param name="brush">Brush object that determines the characteristics of the fill.</param>
		/// <param name="rect">RectangleD structure that represents the rectangle to fill.</param>
		public static void FillRectangle(Graphics g, Brush brush, RectangleD rect)
		{
		    if (brush == null || (brush is SolidBrush && ((SolidBrush) brush).Color == Color.Transparent)) return;

		    g.FillRectangle(brush, rect.ToRectangleF());
		}

		/// <summary>
		/// Fills the interior of a rectangle specified by a RectangleF structure.
		/// </summary>
		/// <param name="g">The Graphics to draw on.</param>
		/// <param name="brush">Brush object that determines the characteristics of the fill.</param>
		/// <param name="rect">RectangleD structure that represents the rectangle to fill.</param>
		public static void FillRectangle(Graphics g, Brush brush, RectangleF rect)
		{
		    if (brush == null || (brush is SolidBrush && ((SolidBrush) brush).Color == Color.Transparent)) return;

		    g.FillRectangle(brush, rect);
		}

        /// <summary>
        /// Fills the interior of a rectangle specified by a RectangleF structure.
        /// </summary>
        /// <param name="g">The Graphics to draw on.</param>
        /// <param name="color">Color object that determines the characteristics of the fill.</param>
        /// <param name="rect">Rectangle structure that represents the rectangle to fill.</param>
        public static void FillRectangle(Graphics g, Color color, Rectangle rect)
        {
            using (var brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, rect);
            }
        }

		/// <summary>
		/// Fills the interior of an ellipse specified by a RectangleD structure.
		/// </summary>
		/// <param name="g">The Graphics to draw on.</param>
		/// <param name="color">Color to fill.</param>
		/// <param name="rect">RectangleD structure that represents the ellipse to fill.</param>
		public static void FillEllipse(Graphics g, Color color, RectangleD rect)
		{
			if (color == Color.Transparent) return;

			using (Brush brush = new SolidBrush(color))
			{
				g.FillEllipse(brush, rect.ToRectangleF());
			}
		}

		/// <summary>
		/// Fills the interior of an ellipse specified by a RectangleD structure.
		/// </summary>
		/// <param name="g">The Graphics to draw on.</param>
		/// <param name="color">Color to fill.</param>
		/// <param name="rect">RectangleF structure that represents the ellipse to fill.</param>
		public static void FillEllipse(Graphics g, Color color, RectangleF rect)
		{
			if (color == Color.Transparent) return;

			using (Brush brush = new SolidBrush(color))
			{
				g.FillEllipse(brush, rect);
			}
		}

		/// <summary>
		/// Fills the interior of an ellipse specified by a RectangleF structure.
		/// </summary>
		/// <param name="g">The Graphics to draw on.</param>
		/// <param name="brush">Brush object that determines the characteristics of the fill.</param>
		/// <param name="rect">RectangleD structure that represents the ellipse to fill.</param>
		public static void FillEllipse(Graphics g, Brush brush, RectangleF rect)
		{
			if (brush == null || (brush is SolidBrush && ((SolidBrush)brush).Color == Color.Transparent)) return;

			g.FillEllipse(brush, rect);
		}

		/// <summary>
		/// Fills the interior of an ellipse specified by a RectangleF structure.
		/// </summary>
		/// <param name="g">The Graphics to draw on.</param>
		/// <param name="brush">StiBrush object that determines the characteristics of the fill.</param>
		/// <param name="rect">RectangleD structure that represents the ellipse to fill.</param>
		public static void FillEllipse(Graphics g, StiBrush brush, RectangleF rect)
		{
			if (brush == null) return;

			using (var br = StiBrush.GetBrush(brush, rect))
			{
				var gradient = brush as StiGradientBrush;
				if (StiBaseOptions.FixBandedGradients && g.PageUnit != GraphicsUnit.Display && gradient != null && gradient.Angle != 0 && gradient.Angle != 180)
				{
					var imgSize = new[] { new PointF(rect.Width, rect.Height) };
					g.TransformPoints(CoordinateSpace.Page, CoordinateSpace.Device, imgSize);

					using (var bmp = new Bitmap((int)(imgSize[0].X / g.PageScale), (int)(imgSize[0].Y / g.PageScale), g))
					using (var gb = Graphics.FromImage(bmp))
					{
						gb.PageUnit = g.PageUnit;
						gb.PageScale = g.PageScale;
						gb.TranslateTransform(-rect.Left, -rect.Top);
						g.FillEllipse(br, new RectangleF(rect.Left, rect.Top, rect.Width, rect.Height));
						g.DrawImage(bmp, rect.Left, rect.Top);
					}
				}
				else
				{
					FillEllipse(g, br, rect);
				}
			}
		}

		public static void DrawLine(Graphics g, Pen pen, double x1, double y1, double x2, double y2)
        {
            g.DrawLine(pen, (float)x1, (float)y1, (float)x2, (float)y2);
        }

	    public static void DrawLine(Graphics g, Color color, double x1, double y1, double x2, double y2)
	    {
	        using (var pen = new Pen(color))
	        {
	            g.DrawLine(pen, (float) x1, (float) y1, (float) x2, (float) y2);
	        }
	    }

        /// <summary>
        /// Draws a selected point specified by a coordinate pair.
        /// </summary>
        /// <param name="g">The Graphics to draw on.</param>
        /// <param name="size">The size of selected point.</param>
        /// <param name="brush">Brush to draw selected point.</param>
        /// <param name="x">x-coordinate of selected point.</param>
        /// <param name="y">y-coordinate of selected point</param>
        public static void DrawSelectedPoint(Graphics g, float size, Brush brush, double x, double y)
		{
			FillRectangle(g, brush, x - size, y - size, size * 2 + 1, size * 2 + 1);
		}

		/// <summary>
		/// Draws a selected rectangle specified by a Rectangle structure.
		/// </summary>
		/// <param name="g">The Graphics to draw on.</param>
		/// <param name="size">The size of selected point.</param>
		/// <param name="brush">Brush to draw selected point.</param>
		/// <param name="rect">RectangleD structure that represents the rectangle to draw selections.</param>
		public static void DrawSelectedRectangle(Graphics g, int size, Brush brush, RectangleD rect)
		{
			DrawSelectedPoint(g, size, brush, rect.Left, rect.Top);
			DrawSelectedPoint(g, size, brush, rect.Right, rect.Top);
			DrawSelectedPoint(g, size, brush, rect.Left, rect.Bottom);
			DrawSelectedPoint(g, size, brush, rect.Right, rect.Bottom);

			DrawSelectedPoint(g, size, brush, rect.Left + rect.Width / 2, rect.Bottom);
			DrawSelectedPoint(g, size, brush, rect.Left, rect.Top + rect.Height / 2);
			DrawSelectedPoint(g, size, brush, rect.Left + rect.Width / 2, rect.Top);
			DrawSelectedPoint(g, size, brush, rect.Right, rect.Top + rect.Height / 2);
		}

	    /// <summary>
	    /// Draws a string with a specified parameters.
	    /// </summary>
	    /// <param name="g">The Graphics to draw on.</param>
	    /// <param name="text">The text for drawing.</param>
	    /// <param name="font">The font which uses to draw text.</param>
	    /// <param name="color">The color to draw selected point.</param>
	    /// <param name="rect">The RectangleD structure that represents the rectangle to draw selections.</param>
	    /// <param name="sf">The string format which uses to draw text.</param>
	    public static void DrawString(Graphics g, string text, Font font, Color color, Rectangle rect, StringFormat sf)
	    {
	        if (color == Color.Transparent) return;
            using (var foreBrush = new SolidBrush(color))
	        {
	            g.DrawString(text, font, foreBrush, rect, sf);
	        }
	    }

        /// <summary>
        /// Draws a string with a specified parameters.
        /// </summary>
        /// <param name="g">The Graphics to draw on.</param>
        /// <param name="text">The text for drawing.</param>
        /// <param name="font">The font which uses to draw text.</param>
        /// <param name="brush">The Brush to draw text.</param>
        /// <param name="rect">The RectangleD structure that represents the rectangle to draw selections.</param>
        /// <param name="sf">The string format which uses to draw text.</param>
	    public static void DrawString(Graphics g, string text, Font font, StiBrush brush, RectangleD rect, StringFormat sf)
	    {
	        if (StiBrush.IsTransparent(brush)) return;

            using (var foreBrush = StiBrush.GetBrush(brush, rect))
	        {
	            g.DrawString(text, font, foreBrush, rect.ToRectangleF(), sf);
	        }
	    }

	    /// <summary>
	    /// Draws a string with a specified parameters.
	    /// </summary>
	    /// <param name="g">The Graphics to draw on.</param>
	    /// <param name="text">The text for drawing.</param>
	    /// <param name="font">The font which uses to draw text.</param>
	    /// <param name="color">The color to draw text.</param>
	    /// <param name="rect">The RectangleD structure that represents the rectangle to draw selections.</param>
	    /// <param name="sf">The string format which uses to draw text.</param>
	    public static void DrawString(Graphics g, string text, Font font, Color color, RectangleD rect, StringFormat sf)
	    {
	        if (color == Color.Transparent) return;

            using (var brush = new SolidBrush(color))
	        {
	            g.DrawString(text, font, brush, rect.ToRectangleF(), sf);
	        }
	    }

	    /// <summary>
	    /// Draws a string with a specified parameters.
	    /// </summary>
	    /// <param name="g">The Graphics to draw on.</param>
	    /// <param name="text">The text for drawing.</param>
	    /// <param name="font">The font which uses to draw text.</param>
	    /// <param name="color">The color to draw text.</param>
	    /// <param name="rect">The RectangleF structure that represents the rectangle to draw selections.</param>
	    /// <param name="sf">The string format which uses to draw text.</param>
	    public static void DrawString(Graphics g, string text, Font font, Color color, RectangleF rect, StringFormat sf)
	    {
            if (color == Color.Transparent) return;

	        using (var brush = new SolidBrush(color))
	        {
	            g.DrawString(text, font, brush, rect, sf);
	        }
	    }

        /// <summary>
        /// Draws a string with a specified parameters.
        /// </summary>
        /// <param name="g">The Graphics to draw on.</param>
        /// <param name="text">The text for drawing.</param>
        /// <param name="font">The font which uses to draw text.</param>
        /// <param name="brush">The Brush to draw text.</param>
        /// <param name="rect">The RectangleD structure that represents the rectangle to draw selections.</param>
        /// <param name="sf">The string format which uses to draw text.</param>
        public static void DrawString(Graphics g, string text, Font font, StiBrush brush, Rectangle rect, StringFormat sf)
	    {
	        if (StiBrush.IsTransparent(brush)) return;

            using (var gdiBrush = StiBrush.GetBrush(brush, rect))
	        {
	            g.DrawString(text, font, gdiBrush, rect, sf);
	        }
	    }

	    /// <summary>
	    /// Draws a string with a specified parameters.
	    /// </summary>
	    /// <param name="g">The Graphics to draw on.</param>
	    /// <param name="text">The text for drawing.</param>
	    /// <param name="font">The font which uses to draw text.</param>
	    /// <param name="brush">The Brush to draw text.</param>
	    /// <param name="rect">The RectangleD structure that represents the rectangle to draw selections.</param>
	    /// <param name="sf">The string format which uses to draw text.</param>
	    public static void DrawString(Graphics g, string text, Font font, StiBrush brush, RectangleF rect, StringFormat sf)
	    {
	        if (StiBrush.IsTransparent(brush)) return;

            using (var gdiBrush = StiBrush.GetBrush(brush, rect))
	        {
	            g.DrawString(text, font, gdiBrush, rect, sf);
	        }
	    }

		/// <summary>
		/// Draws a graphical path specified by a GraphicsPath structure.
		/// </summary>
		/// <param name="g">The Graphics to draw on.</param>
		/// <param name="color"> color to draw RectangleF.</param>
		/// <param name="path">A GraphicsPath structure that represents the path to draw.</param>
		public static void DrawPath(Graphics g, Color color, GraphicsPath path)
        {
			using (Pen pen = new Pen(color))
			{
				g.DrawPath(pen, path);
			}
        }

		/// <summary>
		/// Fills the interior of a graphical path specified by a GraphicsPath structure.
		/// </summary>
		/// <param name="g">The Graphics to draw on.</param>
		/// <param name="color">A color object that determines the characteristics of the fill.</param>
		/// <param name="path">GraphicsPath structure that represents the path to fill.</param>
		public static void FillPath(Graphics g, Color color, GraphicsPath path)
		{
			using (var brush = new SolidBrush(color))
			{
				g.FillPath(brush, path);
			}
		}

		/// <summary>
		/// Fills the interior of a graphical path specified by a GraphicsPath structure.
		/// </summary>
		/// <param name="g">The Graphics to draw on.</param>
		/// <param name="brush">A brush object that determines the characteristics of the fill.</param>
		/// <param name="path">GraphicsPath structure that represents the path to fill.</param>
		public static void FillPath(Graphics g, StiBrush brush, GraphicsPath path)
		{
			using (var gdiBrush = StiBrush.GetBrush(brush, path.GetBounds()))
			{
				g.FillPath(gdiBrush, path);
			}
		}
		#endregion
	}
}
