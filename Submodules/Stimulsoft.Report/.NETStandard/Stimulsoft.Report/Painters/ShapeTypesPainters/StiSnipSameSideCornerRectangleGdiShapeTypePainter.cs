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
using System.Collections;
using System.Text;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.ShapeTypes;
using Stimulsoft.Report.Dialogs;
using Stimulsoft.Report.Design;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiSnipSameSideCornerRectangleGdiShapeTypePainter : StiGdiShapeTypePainter
    {
        #region Methods.Painter
        public override void Paint(object context, StiShape shape, StiShapeTypeService shapeType, RectangleF rect, float zoom)
        {
            if (!(context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var g = (Graphics)context;

            #region Draw Fill
            rect = GetRect(shape, rect, zoom);

            using (var path = new GraphicsPath())
            {
                float restWidth = rect.Width/7.2f;
                float restHeight = rect.Height/4.6f;

                path.AddLines(new[]
                {
                    new PointF(rect.X, rect.Y + restHeight),
                    new PointF(rect.X + restWidth, rect.Y),
                    new PointF(rect.Right - restWidth, rect.Y),
                    new PointF(rect.Right, rect.Y + restHeight),
                    new PointF(rect.Right, rect.Bottom),
                    new PointF(rect.X, rect.Bottom),
                    new PointF(rect.X, rect.Y + restHeight)
                });

                DrawShape(g, shape, path, rect, zoom);
            }

            #endregion
        }
        #endregion
    }
}