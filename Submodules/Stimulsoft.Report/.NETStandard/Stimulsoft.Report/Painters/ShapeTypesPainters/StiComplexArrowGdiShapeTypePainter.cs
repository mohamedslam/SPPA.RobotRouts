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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.ShapeTypes;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiComplexArrowGdiShapeTypePainter : StiGdiShapeTypePainter
    {
        #region Methods.Painter
        public override void Paint(object context, StiShape shape, StiShapeTypeService shapeType, RectangleF rect, float zoom)
        {
            if (!(context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var g = context as Graphics;
            var complexArrow = (StiComplexArrowShapeType)shapeType;

            #region Create Figure
            rect = GetRect(shape, rect, zoom);

            float restHeight = (rect.Width < rect.Height) ? rect.Width / 2 : rect.Height / 2;
            float topBottomSpace = (rect.Height / 3.8f);
            float leftRightSpace = (rect.Width / 3.8f);
            float restWidth = (rect.Height < rect.Width) ? rect.Height / 2 : rect.Width / 2;

            using (var path = new GraphicsPath())
            {
                switch (complexArrow.Direction)
                {
                    case StiShapeDirection.Left:
                    case StiShapeDirection.Right:
                        path.AddLines(new[]
                        {
                            new PointF(rect.X, rect.Y + (rect.Height/2)),
                            new PointF(rect.X + restHeight, rect.Y),
                            new PointF(rect.X + restHeight, rect.Y + topBottomSpace),
                            new PointF(rect.Right - restHeight, rect.Y + topBottomSpace),
                            new PointF(rect.Right - restHeight, rect.Y),
                            new PointF(rect.Right, rect.Y + (rect.Height/2)),
                            new PointF(rect.Right - restHeight, rect.Bottom),
                            new PointF(rect.Right - restHeight, rect.Bottom - topBottomSpace),
                            new PointF(rect.X + restHeight, rect.Bottom - topBottomSpace),
                            new PointF(rect.X + restHeight, rect.Bottom),
                            new PointF(rect.X, rect.Y + (rect.Height/2))
                        });
                        break;

                    case StiShapeDirection.Down:
                    case StiShapeDirection.Up:
                        path.AddLines(new[]
                        {
                            new PointF(rect.X, rect.Y + restWidth),
                            new PointF(rect.X + (rect.Width/2), rect.Y),
                            new PointF(rect.Right, rect.Y + restWidth),
                            new PointF(rect.Right - leftRightSpace, rect.Y + restWidth),
                            new PointF(rect.Right - leftRightSpace, rect.Bottom - restWidth),
                            new PointF(rect.Right, rect.Bottom - restWidth),
                            new PointF(rect.X + (rect.Width/2), rect.Bottom),
                            new PointF(rect.X, rect.Bottom - restWidth),
                            new PointF(rect.X + leftRightSpace, rect.Bottom - restWidth),
                            new PointF(rect.X + leftRightSpace, rect.Y + restWidth),
                            new PointF(rect.X, rect.Y + restWidth)
                        });
                        break;
                }

                DrawShape(g, shape, path, rect, zoom);
            }

            #endregion
        }
        #endregion
    }
}
