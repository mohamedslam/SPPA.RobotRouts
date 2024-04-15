#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiLineGeom3D : StiGeom3D
    {
        #region Properties
        public Color Color { get; set; }

        public StiPenStyle PenStyle { get; set; }
        #endregion

        public override void DrawElements(StiContext context, StiMatrix vertices)
        {
            var pen = new StiPenGeom(Color, 1);
            pen.PenStyle = PenStyle;

            var sPoint = GetPoint((float)vertices[0, 0], (float)vertices[0, 1]);
            var ePoint = GetPoint((float)vertices[1, 0], (float)vertices[1, 1]);

            context.PushSmoothingModeToAntiAlias();
            context.DrawLine(pen, sPoint.X, sPoint.Y, ePoint.X, ePoint.Y);
            context.PopSmoothingMode();
        }

        public StiLineGeom3D(StiPoint3D startPoint, StiPoint3D endPoint, Color color, StiPenStyle style, StiRender3D render3D) :
        base(render3D)
        {
            this.Color = color;
            this.PenStyle = style;

            this.Vertexes = new StiMatrix(new double[,]
            {
                {startPoint.X, startPoint.Y, startPoint.Z, 1 },
                {endPoint.X, endPoint.Y, endPoint.Z, 1 }
            });
        }

    }
}
