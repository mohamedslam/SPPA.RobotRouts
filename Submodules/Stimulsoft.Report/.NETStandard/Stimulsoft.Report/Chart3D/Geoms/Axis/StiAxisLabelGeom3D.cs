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
    public class StiAxisLabelGeom3D : StiGeom3D
    {
        #region Fields
        private IStiAxis3D axis;
        private SizeF clientSize;
        private string text;
        private StiStripLineXF stripLine;
        private StiRotationMode rotationMode;
        #endregion

        #region Methods
        public override void DrawElements(StiContext context, StiMatrix vertices)
        {
            var x = (float)vertices.Grid[0, 0];
            var y = (float)vertices.Grid[0, 1];

            var point = GetPoint(x, y);
            var font = axis.Core.GetFontGeom(context);
            var sf = context.GetGenericStringFormat();
            var labelBrush = new StiSolidBrush(axis.Labels.Color);

            context.DrawRotatedString(this.text, font, labelBrush, point, sf, rotationMode, 0, true);
        }

        public override RectangleF MeasureCientRect()
        {
            GlobalTransform();
            var vertices = this.ScreenProjection();

            var x = (float)vertices.Grid[0, 0];
            var y = (float)vertices.Grid[0, 1];

            var point = GetPoint(x, y);

            return new RectangleF(point.X, point.Y, this.clientSize.Width, this.clientSize.Height);
        }
        #endregion

        public StiAxisLabelGeom3D(IStiAxis3D axis, SizeF clientSize, StiPoint3D? textPoint, string text, StiStripLineXF stripLine, StiRotationMode rotationMode, StiRender3D render)
            : base(render)
        {
            this.axis = axis;
            this.clientSize = clientSize;
            this.text = text;
            this.stripLine = stripLine;
            this.rotationMode = rotationMode;

            var point3D = textPoint.GetValueOrDefault();
            var point = this.GetPoint((float)point3D.X, (float)point3D.Y);

            this.ClientRectangle = new RectangleF(point.X, point.Y, clientSize.Width, clientSize.Height);

            this.Vertexes = new StiMatrix(new double[,] {
                 {point3D.X, point3D.Y, point3D.Z, 1}
            });
        }
    }
}
