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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.ShapeTypes;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Pen = Stimulsoft.Drawing.Pen;
using Graphics = Stimulsoft.Drawing.Graphics;
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiTriangleGdiShapeTypePainter : StiGdiShapeTypePainter
    {
        #region Methods.Painter
        public override void Paint(object context, StiShape shape, StiShapeTypeService shapeType, RectangleF rect, float zoom)
        {
            if (!(context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var triangleShape = (StiTriangleShapeType)shapeType;
            var g = (Graphics)context;

            rect = GetRect(shape, rect, zoom);
            using (var path = new GraphicsPath())
            {
                path.StartFigure();

                switch (triangleShape.Direction)
                {
                    case StiShapeDirection.Up:
                        path.AddLines(new[]
                        {
                            new PointF(rect.X, rect.Y + rect.Height),
                            new PointF(rect.X + rect.Width, rect.Y + rect.Height),
                            new PointF(rect.X + rect.Width/2, rect.Y)
                        });
                        break;

                    case StiShapeDirection.Down:
                        path.AddLines(new[]
                        {
                            new PointF(rect.X, rect.Y),
                            new PointF(rect.X + rect.Width, rect.Y),
                            new PointF(rect.X + rect.Width/2, rect.Y + rect.Height)
                        });
                        break;

                    case StiShapeDirection.Left:
                        path.AddLines(new[]
                        {
                            new PointF(rect.X + rect.Width, rect.Y),
                            new PointF(rect.X + rect.Width, rect.Y + rect.Height),
                            new PointF(rect.X, rect.Y + rect.Height/2)
                        });
                        break;

                    case StiShapeDirection.Right:
                        path.AddLines(new[]
                        {
                            new PointF(rect.X, rect.Y),
                            new PointF(rect.X, rect.Y + rect.Height),
                            new PointF(rect.X + rect.Width, rect.Y + rect.Height/2)
                        });
                        break;
                }

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
        }
        #endregion
    }
}
