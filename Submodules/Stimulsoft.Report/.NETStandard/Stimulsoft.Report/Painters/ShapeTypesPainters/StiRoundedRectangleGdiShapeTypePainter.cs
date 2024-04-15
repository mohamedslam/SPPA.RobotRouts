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
    public class StiRoundedRectangleGdiShapeTypePainter : StiGdiShapeTypePainter
    {
        #region Methods.Painter
        public override void Paint(object context, StiShape shape, StiShapeTypeService shapeType, RectangleF rect, float zoom)
        {
            if (!(context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var roundedShape = (StiRoundedRectangleShapeType)shapeType;
            var g = (Graphics)context;

            rect = GetRect(shape, rect, zoom);
            float space = (float)Math.Min((rect.Height < rect.Width ? rect.Height : rect.Width), 100 * zoom) * roundedShape.Round;

            if (rect.Width >= space * 2 && rect.Height >= space * 2)
            {
                try
                {
                    space = (float)Math.Round(space, 2);
                    if (space < 0.01) space = 0.01f;
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
                catch
                {
                }
            }           
        }
        #endregion
    }
}
