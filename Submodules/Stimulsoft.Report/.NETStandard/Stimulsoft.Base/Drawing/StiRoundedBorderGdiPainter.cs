#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
#endif

namespace Stimulsoft.Base.Drawing
{
    public static class StiRoundedBorderGdiPainter
    {
        #region Properties
        public static bool AllowAntiAlias { get; set; } = true;
        #endregion

        #region Methods
        public static void PaintBorder(Graphics g, RectangleF rect, StiSimpleBorder border, StiCornerRadius cornerRadius, 
            StiButtonShapeType shapeType = StiButtonShapeType.Rectangle, float scale = 1f, bool isDesignTime = false)
        {           
            switch (shapeType)
            {
                case StiButtonShapeType.Rectangle:
                    PaintRoundedBorder(g, rect, border, cornerRadius, scale, isDesignTime);
                    break;

                case StiButtonShapeType.Circle:
                    PaintCircledBorder(g, rect, border, scale, isDesignTime);
                    break;
            }                        
        }

        private static void PaintRoundedBorder(Graphics g, RectangleF rect, StiSimpleBorder border, StiCornerRadius cornerRadius, 
            float scale, bool isDesignTime = false)
        {
            var borderThickness = (float)(border.Size * scale);
            if (border.Style == StiPenStyle.Double)
                borderThickness = 1;

            var mode = g.SmoothingMode;

            if (AllowAntiAlias && !cornerRadius.IsEmpty)
                g.SmoothingMode = SmoothingMode.AntiAlias;

            try
            {
                if (border.Side == StiBorderSides.None && isDesignTime)
                    PaintRoundedBorderDesign(g, rect, cornerRadius, scale);
                
                else if (border.Side == StiBorderSides.All)
                    PaintRoundedBorderAllSides(g, rect, border, cornerRadius, scale, borderThickness);

                else                
                    PaintRoundedBorderDefinedSides(g, rect, border, cornerRadius, scale, borderThickness, isDesignTime);
            }
            finally
            {
                g.SmoothingMode = mode;
            }
        }

        private static void PaintCircledBorder(Graphics g, RectangleF rect, StiSimpleBorder border, float scale, bool isDesignTime)
        {
            if (border.Side != StiBorderSides.All && !isDesignTime) return;

            rect = FitRectangleInCircle(rect);

            var borderThickness = (float)(border.Size * scale * StiScale.Factor);
            if (border.Style == StiPenStyle.Double)
                borderThickness = 1;

            var mode = g.SmoothingMode;

            if (AllowAntiAlias)
                g.SmoothingMode = SmoothingMode.AntiAlias;

            try
            {
                using (var pen = GetBorderPen(border, borderThickness, isDesignTime))
                {
                    g.DrawEllipse(pen, rect);

                    if (border.Style == StiPenStyle.Double && border.Side != StiBorderSides.All)
                    {
                        rect.Inflate(-StiScale.I2, -StiScale.I2);
                        g.DrawEllipse(pen, rect);
                    }
                }
            }
            finally
            {
                g.SmoothingMode = mode;
            }
        }

        private static void PaintRoundedBorderDefinedSides(Graphics g, RectangleF rect, StiSimpleBorder border, StiCornerRadius cornerRadius,
            float scale, float borderThickness, bool isDesignTime)
        {
            using (var pen = GetBorderPen(border, borderThickness, isDesignTime))
            {
                StiRoundedRectangleCreator.DrawRoundedRect(g, pen, rect, cornerRadius, border.Side, scale);

                if (border.Style == StiPenStyle.Double)
                {
                    rect.Inflate(-StiScale.I2, -StiScale.I2);

                    StiRoundedRectangleCreator.DrawRoundedRect(g, pen, rect, cornerRadius, border.Side, scale);
                }
            }
        }

        private static void PaintRoundedBorderAllSides(Graphics g, RectangleF rect, StiSimpleBorder border, StiCornerRadius cornerRadius, float scale, float borderThickness)
        {
            using (var pen = GetBorderPen(border, borderThickness))
            {
                if (cornerRadius.IsEmpty)
                {
                    g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);

                    if (border.Style == StiPenStyle.Double)
                    {
                        rect.Inflate(-StiScale.I2, -StiScale.I2);
                        g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                    }
                }
                else
                {
                    using (var path = StiRoundedRectangleCreator.Create(rect, cornerRadius, scale))
                    {
                        g.DrawPath(pen, path);

                        if (border.Style == StiPenStyle.Double)
                        {
                            rect.Inflate(-StiScale.I2, -StiScale.I2);

                            using (var path2 = StiRoundedRectangleCreator.Create(rect, cornerRadius, scale))
                            {
                                g.DrawPath(pen, path2);
                            }
                        }
                    }
                }
            }
        }

        private static void PaintRoundedBorderDesign(Graphics g, RectangleF rect, StiCornerRadius cornerRadius, float scale)
        {
            using (var pen = new Pen(Color.Gray, 1))
            {
                if (cornerRadius == null || cornerRadius.IsEmpty)
                {
                    g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                }
                else
                {
                    using (var path = StiRoundedRectangleCreator.Create(rect, cornerRadius, scale))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }
                
        public static RectangleF FitRectangleInCircle(RectangleF rect)
        {
            if (rect.Width > rect.Height)
                return new RectangleF(rect.X + (rect.Width - rect.Height) / 2, rect.Y, rect.Height, rect.Height);
            else
                return new RectangleF(rect.X, rect.Y + (rect.Height - rect.Width) / 2, rect.Width, rect.Width);
        }

        public static RectangleD FitRectangleInCircle(RectangleD rect)
        {
            if (rect.Width > rect.Height)
                return new RectangleD(rect.X + (rect.Width - rect.Height) / 2, rect.Y, rect.Height, rect.Height);
            else
                return new RectangleD(rect.X, rect.Y + (rect.Height - rect.Width) / 2, rect.Width, rect.Width);
        }

        private static Pen GetBorderPen(StiSimpleBorder border, float borderThickness, bool isDesignTime = false)
        {
            var dashStyle = StiPenUtils.GetPenStyle(border.Style);
            var color = border.Color;

            if (border.Side == StiBorderSides.None && isDesignTime)
            {
                dashStyle = DashStyle.Dash;
                color = Color.LightGray;
            }

            return new Pen(color, borderThickness)
            {
                DashStyle = dashStyle,
                Alignment = PenAlignment.Center,
                StartCap = LineCap.Square,
                EndCap = LineCap.Square
            };
        }        
        #endregion
    }
}