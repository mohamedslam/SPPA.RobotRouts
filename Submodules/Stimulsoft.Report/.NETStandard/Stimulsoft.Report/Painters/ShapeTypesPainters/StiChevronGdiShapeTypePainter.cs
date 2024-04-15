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
    public class StiChevronGdiShapeTypePainter : StiGdiShapeTypePainter
    {
        #region Methods.Painter
        public override void Paint(object context, StiShape shape, StiShapeTypeService shapeType, RectangleF rect, float zoom)
        {
            if (!(context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var g = (Graphics)context;
            var chevron = (StiChevronShapeType)shapeType;

            #region Create Figure
            rect = GetRect(shape, rect, zoom);

            using (var path = new GraphicsPath())
            {
                float rest = (rect.Width > rect.Height) ? (rect.Height/2) : (rect.Width/2);
                switch (chevron.Direction)
                {
                        #region Right

                    case StiShapeDirection.Right:
                        path.AddLines(new[]
                        {
                            new PointF(rect.X, rect.Y),
                            new PointF(rect.X + rest, rect.Y + (rect.Height/2)),
                            new PointF(rect.X, rect.Bottom),
                            new PointF(rect.Right - rest, rect.Bottom),
                            new PointF(rect.Right, rect.Y + (rect.Height/2)),
                            new PointF(rect.Right - rest, rect.Y),
                            new PointF(rect.X, rect.Y)
                        });
                        break;

                        #endregion

                        #region Left

                    case StiShapeDirection.Left:
                        path.AddLines(new[]
                        {
                            new PointF(rect.Right, rect.Y),
                            new PointF(rect.X + rest, rect.Y),
                            new PointF(rect.X, rect.Y + (rect.Height/2)),
                            new PointF(rect.X + rest, rect.Bottom),
                            new PointF(rect.Right, rect.Bottom),
                            new PointF(rect.Right - rest, rect.Y + (rect.Height/2)),
                            new PointF(rect.Right, rect.Y)
                        });
                        break;

                        #endregion

                        #region Up

                    case StiShapeDirection.Up:
                        path.AddLines(new[]
                        {
                            new PointF(rect.X, rect.Y + rest),
                            new PointF(rect.X + (rect.Width/2), rect.Y),
                            new PointF(rect.Right, rect.Y + rest),
                            new PointF(rect.Right, rect.Bottom),
                            new PointF(rect.X + (rect.Width/2), rect.Bottom - rest),
                            new PointF(rect.X, rect.Bottom),
                            new PointF(rect.X, rect.Y + rest)
                        });
                        break;

                        #endregion

                        #region Down

                    case StiShapeDirection.Down:
                        path.AddLines(new[]
                        {
                            new PointF(rect.X, rect.Y),
                            new PointF(rect.X + (rect.Width/2), rect.Y + rest),
                            new PointF(rect.Right, rect.Y),
                            new PointF(rect.Right, rect.Bottom - rest),
                            new PointF(rect.X + (rect.Width/2), rect.Bottom),
                            new PointF(rect.X, rect.Bottom - rest),
                            new PointF(rect.X, rect.Y)
                        });
                        break;

                        #endregion
                }

                DrawShape(g, shape, path, rect, zoom);
            }
            #endregion
        }
        #endregion
    }
}
