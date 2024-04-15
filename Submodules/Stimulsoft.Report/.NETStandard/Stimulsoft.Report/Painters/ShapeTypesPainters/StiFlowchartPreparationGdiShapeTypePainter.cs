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
    public class StiFlowchartPreparationGdiShapeTypePainter : StiGdiShapeTypePainter
    {
        #region Methods.Painter
        public override void Paint(object context, StiShape shape, StiShapeTypeService shapeType, RectangleF rect, float zoom)
        {
            if (!(context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var g = (Graphics)context;
            var flowchartPreparation = (StiFlowchartPreparationShapeType)shapeType;

            #region Draw Fill
            rect = GetRect(shape, rect, zoom);

            using (var path = new GraphicsPath())
            {
                float restWidth = rect.Width/5;
                float restHeight = rect.Height/5;
                float xCenter = rect.Width/2;
                float yCenter = rect.Height/2;

                switch (flowchartPreparation.Direction)
                {
                    case StiShapeDirection.Left:
                    case StiShapeDirection.Right:
                        path.AddLines(new[]
                        {
                            new PointF(rect.X, rect.Y + yCenter),
                            new PointF(rect.X + restWidth, rect.Y),
                            new PointF(rect.Right - restWidth, rect.Y),
                            new PointF(rect.Right, rect.Y + yCenter),
                            new PointF(rect.Right - restWidth, rect.Bottom),
                            new PointF(rect.X + restWidth, (rect.Bottom)),
                            new PointF(rect.X, rect.Y + yCenter)
                        });
                        break;

                    case StiShapeDirection.Down:
                    case StiShapeDirection.Up:
                        path.AddLines(new[]
                        {
                            new PointF(rect.X + xCenter, rect.Y),
                            new PointF(rect.Right, rect.Y + restHeight),
                            new PointF(rect.Right, rect.Bottom - restHeight),
                            new PointF(rect.X + xCenter, rect.Bottom),
                            new PointF(rect.X, rect.Bottom - restHeight),
                            new PointF(rect.X, rect.Y + restHeight),
                            new PointF(rect.X + xCenter, rect.Y)
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
