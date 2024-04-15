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
    public class StiOctagonGdiShapeTypePainter : StiGdiShapeTypePainter
    {
        #region Methods.Painter
        public override void Paint(object context, StiShape shape, StiShapeTypeService shapeType, RectangleF rect, float zoom)
        {
            if (!(context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var octagonShape = (StiOctagonShapeType)shapeType;
            var g = (Graphics)context;
            rect = GetRect(shape, rect, zoom);

            float bevelx = (shape.Report != null ? (float)shape.Report.Unit.ConvertToHInches(octagonShape.Bevel) : octagonShape.Bevel) * zoom;
            float bevely = bevelx;
            if (octagonShape.AutoSize)
            {
                bevelx = rect.Width / (2.414f * 1.414f);
                bevely = rect.Height / (2.414f * 1.414f);
            }
            if (bevelx > rect.Width / 2) bevelx = rect.Width / 2;
            if (bevely > rect.Height / 2) bevely = rect.Height / 2;

            try
            {
                float size = Math.Max(shape.Size, 1);
                bevelx = (float)Math.Round(bevelx, 2);
                bevely = (float)Math.Round(bevely, 2);
                using (var path = new GraphicsPath())
                {
                    path.StartFigure();
                    path.AddLine(rect.X + bevelx, rect.Y, rect.Right - bevelx, rect.Y);
                    path.AddLine(rect.Right - bevelx, rect.Y, rect.Right, rect.Y + bevely);
                    path.AddLine(rect.Right, rect.Y + bevely, rect.Right, rect.Bottom - bevely);
                    path.AddLine(rect.Right, rect.Bottom - bevely, rect.Right - bevelx, rect.Bottom);
                    path.AddLine(rect.Right - bevelx, rect.Bottom, rect.X + bevelx, rect.Bottom);
                    path.AddLine(rect.X + bevelx, rect.Bottom, rect.X, rect.Bottom - bevely);
                    path.AddLine(rect.X, rect.Bottom - bevely, rect.X, rect.Y + bevely);
                    path.CloseFigure();

                    using (var brush = StiBrush.GetBrush(shape.Brush, rect))
                    {
                        g.FillPath(brush, path);
                    }
                    if (shape.Style != StiPenStyle.None)
                    {
                        using (var pen = new Pen(shape.BorderColor, size * zoom))
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
        #endregion
    }
}
