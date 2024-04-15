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

#if STIDRAWING
using Pen = Stimulsoft.Drawing.Pen;
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiDiagonalDownLineGdiShapeTypePainter : StiGdiShapeTypePainter
    {
        #region Methods.Painter
        public override void Paint(object context, StiShape shape, StiShapeTypeService shapeType, RectangleF rect, float zoom)
        {
            if (!(context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var g = (Graphics)context;
            rect = GetRect(shape, rect, zoom);

            using (var brush = StiBrush.GetBrush(shape.Brush, rect))
            {
                g.FillRectangle(brush, rect);
            }
            if (shape.Style != StiPenStyle.None)
            {
                float offsetDouble = zoom > 1 ? zoom : 1;
                using (var pen = new Pen(shape.BorderColor, shape.Style == StiPenStyle.Double ? offsetDouble : shape.Size * zoom))
                {
                    pen.DashStyle = StiPenUtils.GetPenStyle(shape.Style);
                    if (shape.Style == StiPenStyle.Double)
                    {
                        float offset = 1.55f * offsetDouble;
                        g.DrawLine(pen, rect.X + offset, rect.Y, rect.Right, rect.Bottom - offset);
                        g.DrawLine(pen, rect.X, rect.Y + offset, rect.Right - offset, rect.Bottom);
                    }
                    else
                    {
                        g.DrawLine(pen, rect.X, rect.Y, rect.Right, rect.Bottom);
                    }
                }
            }
        }
        #endregion
    }
}
