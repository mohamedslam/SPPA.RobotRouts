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
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiRoundedRectanglePrimitiveGdiPainter : StiRectanglePrimitiveGdiPainter
    {
        #region Methods.Painter
        protected override void PaintPrimitive(StiRectanglePrimitive primitive, Graphics g, RectangleD rectD)
        {
            var rect = rectD.ToRectangleF();
            var roundedPrimitive = (StiRoundedRectanglePrimitive)primitive;

            #region HighlightState
            if ((primitive.HighlightState == StiHighlightState.Show || primitive.HighlightState == StiHighlightState.Active) &&
                (!primitive.Report.Info.IsComponentsMoving))
            {
                var oldSmoothingMode = g.SmoothingMode;
                if (!StiOptions.Engine.DisableAntialiasingInPainters) g.SmoothingMode = SmoothingMode.AntiAlias;

                using (var pen2 = new Pen(Color.FromArgb(150, Color.Gold), StiScale.I3))
                using (var path = new GraphicsPath())
                {
                    pen2.Alignment = PenAlignment.Outset;
                    pen2.StartCap = LineCap.Round;
                    pen2.EndCap = LineCap.Round;

                    AddRoundedRectangleToPath(roundedPrimitive, rect, path);

                    if (primitive.IsSelected)
                    {
                        float size = StiScale.I3;
                        AddDotToPath(path, rect.X, rect.Y, size);
                        AddDotToPath(path, rect.Right, rect.Y, size);
                        AddDotToPath(path, rect.X, rect.Bottom, size);
                        AddDotToPath(path, rect.Right, rect.Bottom, size);

                        AddDotToPath(path, rect.X + rect.Width / 2, rect.Y, size);
                        AddDotToPath(path, rect.Right, rect.Y + rect.Height / 2, size);
                        AddDotToPath(path, rect.X + rect.Width / 2, rect.Bottom, size);
                        AddDotToPath(path, rect.X, rect.Y + rect.Height / 2, size);
                    }

                    g.DrawPath(pen2, path);
                }

                g.SmoothingMode = oldSmoothingMode;
            }
            #endregion

            float space = (float)Math.Min((rect.Height < rect.Width ? rect.Height : rect.Width), 100 * roundedPrimitive.Page.Zoom * StiScale.Factor) * roundedPrimitive.Round;

            if (rect.Width >= space * 2 && rect.Height >= space * 2)
            {
                try
                {
                    //space = (float)Math.Round(space, 2);                    
                    using (var pen = new Pen(primitive.Color, (float)(primitive.Size * primitive.Page.Zoom * StiScale.Factor)))
                    {
                        pen.DashStyle = StiPenUtils.GetPenStyle(roundedPrimitive.Style); 

						if (primitive.Style == StiPenStyle.Double)
						{
							var rectIn = rect;
							var rectOut = rect;
							rectIn.Inflate(-1, -1);
							rectOut.Inflate(1, 1);
							pen.Width = 1;
							
							DrawRoundedRectangleToPath(g, pen, roundedPrimitive, rectIn);
							DrawRoundedRectangleToPath(g, pen, roundedPrimitive, rectOut);
						}
						else
						{
                            if (primitive.Style != StiPenStyle.None || StiOptions.Engine.PrimitivesStyleNoneBackCompatibility)
    							DrawRoundedRectangleToPath(g, pen, roundedPrimitive, rect);
						}
                    }
                }
                catch
                {
                }
            }
        }

		private void AddRoundedRectangleToPath(StiRoundedRectanglePrimitive roundedPrimitive, RectangleF rect, GraphicsPath path)
		{
            float space = (float)Math.Min((rect.Height < rect.Width ? rect.Height : rect.Width), 100 * roundedPrimitive.Page.Zoom * StiScale.Factor) * roundedPrimitive.Round;

			if (rect.Width >= space * 2 && rect.Height >= space * 2)
			{
				try
				{
					space = (float)Math.Round(space, 2);
                    if (space < 0.01) space = 0.01f;

					path.StartFigure();
					path.AddLine(rect.X + space, rect.Y, rect.Right - space, rect.Y);
					path.AddArc(rect.Right - 2 * space, rect.Y, 2 * space, 2 * space, -90, 90);

					path.AddLine(rect.Right, rect.Y + space, rect.Right, rect.Bottom - space);
					path.AddArc(rect.Right - 2 * space, rect.Bottom - 2 * space, 2 * space, 2 * space, 0, 90);

					path.AddLine(rect.Right - space, rect.Bottom, rect.X + space, rect.Bottom);
					path.AddArc(rect.X, rect.Bottom - 2 * space, 2 * space, 2 * space, 90, 90);

					path.AddLine(rect.X, rect.Bottom - space, rect.X, rect.Y + space);
					path.AddArc(rect.X, rect.Y, 2 * space, 2 * space, 180, 90);
					path.CloseFigure();
				}
				catch
				{
				}
			}
		}

		private void DrawRoundedRectangleToPath(Graphics g, Pen pen, StiRoundedRectanglePrimitive roundedPrimitive, RectangleF rect)
		{
            float space = (float)Math.Min((rect.Height < rect.Width ? rect.Height : rect.Width), 100 * roundedPrimitive.Page.Zoom * StiScale.Factor) * roundedPrimitive.Round;

			if (rect.Width >= space * 2 && rect.Height >= space * 2)
			{
				try
				{
					space = (float)Math.Round(space, 2);
                    if (space < 0.01) space = 0.01f;

					if (roundedPrimitive.TopSide && roundedPrimitive.LeftSide && roundedPrimitive.BottomSide && roundedPrimitive.RightSide)
					{
						using (var path = new GraphicsPath())
						{
							path.StartFigure();
							path.AddLine(rect.X + space, rect.Y, rect.Right - space, rect.Y);
							path.AddArc(rect.Right - 2 * space, rect.Y, 2 * space, 2 * space, -90, 90);

							path.AddLine(rect.Right, rect.Y + space, rect.Right, rect.Bottom - space);
							path.AddArc(rect.Right - 2 * space, rect.Bottom - 2 * space, 2 * space, 2 * space, 0, 90);

							path.AddLine(rect.Right - space, rect.Bottom, rect.X + space, rect.Bottom);
							path.AddArc(rect.X, rect.Bottom - 2 * space, 2 * space, 2 * space, 90, 90);

							path.AddLine(rect.X, rect.Bottom - space, rect.X, rect.Y + space);
							path.AddArc(rect.X, rect.Y, 2 * space, 2 * space, 180, 90);
							path.CloseFigure();

							g.DrawPath(pen, path);
						}
					}
					else
					{
						float space1 = 0;
						float space2 = 0;

						if (roundedPrimitive.TopSide)
						{
							if (roundedPrimitive.LeftSide) space1 = space;
							if (roundedPrimitive.RightSide) space2 = space;

							g.DrawLine(pen, rect.X + space1, rect.Y, rect.Right - space2, rect.Y);
							if (roundedPrimitive.RightSide)
								g.DrawArc(pen, rect.Right - 2 * space, rect.Y, 2 * space, 2 * space, -90, 90);

							space1 = 0;
							space2 = 0;
						}

						if (roundedPrimitive.RightSide)
						{
							if (roundedPrimitive.TopSide) space1 = space;
							if (roundedPrimitive.BottomSide) space2 = space;

							g.DrawLine(pen, rect.Right, rect.Y + space1, rect.Right, rect.Bottom - space2);
							if (roundedPrimitive.BottomSide)
								g.DrawArc(pen, rect.Right - 2 * space, rect.Bottom - 2 * space, 2 * space, 2 * space, 0, 90);

							space1 = 0;
							space2 = 0;
						}

						if (roundedPrimitive.BottomSide)
						{
							if (roundedPrimitive.RightSide) space1 = space;
							if (roundedPrimitive.LeftSide) space2 = space;

							g.DrawLine(pen, rect.Right - space1, rect.Bottom, rect.X + space2, rect.Bottom);
							if (roundedPrimitive.LeftSide)
								g.DrawArc(pen, rect.X, rect.Bottom - 2 * space, 2 * space, 2 * space, 90, 90);

							space1 = 0;
							space2 = 0;
						}

						if (roundedPrimitive.LeftSide)
						{
							if (roundedPrimitive.BottomSide) space1 = space;
							if (roundedPrimitive.TopSide) space2 = space;

							g.DrawLine(pen, rect.X, rect.Bottom - space1, rect.X, rect.Y + space2);
							if (roundedPrimitive.TopSide)
								g.DrawArc(pen, rect.X, rect.Y, 2 * space, 2 * space, 180, 90);

							space1 = 0;
							space2 = 0;
						}
					}
				}
				catch
				{
				}
			}
		}
        #endregion
    }
}
