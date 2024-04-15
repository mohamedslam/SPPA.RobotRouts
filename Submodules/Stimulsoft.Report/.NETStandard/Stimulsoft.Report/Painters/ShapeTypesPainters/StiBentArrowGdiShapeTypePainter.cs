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
    public class StiBentArrowGdiShapeTypePainter : StiGdiShapeTypePainter
    {
        #region Methods.Painter
        public override void Paint(object context, StiShape shape, StiShapeTypeService shapeType, RectangleF rect, float zoom)
        {
            if (!(context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var g = (Graphics)context;
            var bentArrow = (StiBentArrowShapeType)shapeType;

            #region Create Figure
            rect = GetRect(shape, rect, zoom);

            using (var path = new GraphicsPath())
            {
                float lineHeight = 0;
                float arrowWidth = 0;
                float space = 0;
                if (rect.Height > rect.Width)
                {
                    arrowWidth = rect.Width/4;
                    lineHeight = arrowWidth;
                    space = arrowWidth/2;
                }
                else
                {
                    lineHeight = (int) (rect.Height/4);
                    arrowWidth = lineHeight;
                    space = arrowWidth/2;
                }

                switch (bentArrow.Direction)
                {
                    #region Up

                    case StiShapeDirection.Up:
                        path.AddLines(new[]
                        {
                            new PointF(rect.X, rect.Bottom),
                            new PointF(rect.X, rect.Bottom - lineHeight),
                            new PointF(rect.Right - (space + lineHeight), rect.Bottom - lineHeight),
                            new PointF(rect.Right - (space + lineHeight), rect.Y + arrowWidth),
                            new PointF(rect.Right - (arrowWidth*2), rect.Y + arrowWidth),
                            new PointF(rect.Right - arrowWidth, rect.Y),
                            new PointF(rect.Right, rect.Y + arrowWidth),
                            new PointF(rect.Right - space, rect.Y + arrowWidth),
                            new PointF(rect.Right - space, rect.Bottom),
                            new PointF(rect.X, rect.Bottom)
                        });
                        break;

                    #endregion

                    #region Left

                    case StiShapeDirection.Left:
                        path.AddLines(new[]
                        {
                            new PointF(rect.Right, rect.Bottom),
                            new PointF(rect.Right, rect.Y + space),
                            new PointF(rect.X + arrowWidth, rect.Y + space),
                            new PointF(rect.X + arrowWidth, rect.Y),
                            new PointF(rect.X, rect.Y + arrowWidth),
                            new PointF(rect.X + arrowWidth, rect.Y + arrowWidth*2),
                            new PointF(rect.X + arrowWidth, rect.Y + arrowWidth + space),
                            new PointF(rect.Right - lineHeight, rect.Y + arrowWidth + space),
                            new PointF(rect.Right - lineHeight, rect.Bottom),
                            new PointF(rect.Right, rect.Bottom)
                        });
                        break;

                    #endregion

                    #region Down

                    case StiShapeDirection.Down:
                        path.AddLines(new[]
                        {
                            new PointF(rect.Right, rect.Y),
                            new PointF(rect.X + space, rect.Y),
                            new PointF(rect.X + space, rect.Bottom - arrowWidth),
                            new PointF(rect.X, rect.Bottom - arrowWidth),
                            new PointF(rect.X + arrowWidth, rect.Bottom),
                            new PointF(rect.X + (arrowWidth*2), rect.Bottom - arrowWidth),
                            new PointF(rect.X + arrowWidth + space, rect.Bottom - arrowWidth),
                            new PointF(rect.X + arrowWidth + space, rect.Y + lineHeight),
                            new PointF(rect.Right, rect.Y + lineHeight),
                            new PointF(rect.Right, rect.Y)
                        });
                        break;

                    #endregion

                    #region Right

                    case StiShapeDirection.Right:
                        path.AddLines(new[]
                        {
                            new PointF(rect.X, rect.Y),
                            new PointF(rect.X, rect.Bottom - space),
                            new PointF(rect.Right - arrowWidth, rect.Bottom - space),
                            new PointF(rect.Right - arrowWidth, rect.Bottom),
                            new PointF(rect.Right, rect.Bottom - arrowWidth),
                            new PointF(rect.Right - arrowWidth, rect.Bottom - (arrowWidth*2)),
                            new PointF(rect.Right - arrowWidth, rect.Bottom - arrowWidth - space),
                            new PointF(rect.X + lineHeight, rect.Bottom - arrowWidth - space),
                            new PointF(rect.X + lineHeight, rect.Y),
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