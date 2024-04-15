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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Pen = Stimulsoft.Drawing.Pen;
using Graphics = Stimulsoft.Drawing.Graphics;
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
#endif

namespace Stimulsoft.Report.Painters
{
    public abstract class StiGdiShapeTypePainter : StiShapeTypePainter
    {
        #region Draw
        protected RectangleF GetRect(StiShape shape, RectangleF rect, float zoom)
        {
            if (shape.Size > 1f)
            {
                float step = shape.Size / 2 * zoom;
                if (rect.Width - step - step > 0 && rect.Height - step - step > 0)
                    return new RectangleF(rect.X + step, rect.Y + step, rect.Width - step - step, rect.Height - step - step);
            }

            return rect;
        }

        protected RectangleF GetRectLeftRight(StiShape shape, RectangleF rect, float zoom)
        {
            if (shape.Size > 1f)
            {
                float step = shape.Size / 2 * zoom;
                if (rect.Width - step - step > 0)
                    return new RectangleF(rect.X + step, rect.Y, rect.Width - step - step, rect.Height);
            }

            return rect;
        }

        protected RectangleF GetRectTopBottom(StiShape shape, RectangleF rect, float zoom)
        {
            if (shape.Size > 1f)
            {
                float step = shape.Size / 2 * zoom;
                if (rect.Height - step - step > 0)
                    return new RectangleF(rect.X, rect.Y + step, rect.Width, rect.Height - step - step);
            }

            return rect;
        }


        public static void DrawShape(Graphics g, StiShape shape, Point[] points, RectangleF rect)
        {
            using (var path = new GraphicsPath())
            {
                path.AddLines(points);

                using (var brush = StiBrush.GetBrush(shape.Brush, rect))
                {
                    g.FillPath(brush, path);
                }
                if (shape.Style != StiPenStyle.None)
                {
                    using (var pen = new Pen(shape.BorderColor, shape.Size))
                    {
                        pen.DashStyle = StiPenUtils.GetPenStyle(shape.Style);
                        g.DrawLines(pen, points);
                    }
                }
            }
        }

        public static void DrawShape(Graphics g, StiShape shape, GraphicsPath path, RectangleF rect, float zoom)
        {
            path.CloseFigure();
            using (var brush = StiBrush.GetBrush(shape.Brush, rect))
            {
                g.FillPath(brush, path);
            }
            if (shape.Style != StiPenStyle.None)
            {
                using (var pen = new Pen(shape.BorderColor, shape.Size * zoom))
                {
                    pen.DashStyle = StiPenUtils.GetPenStyle(shape.Style);
                    g.DrawPath(pen, path);
                }
            }
        }
        #endregion
    }
}
