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

using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using System.Collections.Generic;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiPyramidSeriesElementGeom3D :
        StiSeriesElementGeom3D,
        IStiBorderColor,
        IStiColor
    {
        #region Properties
        public Color Color { get; set; }

        public Color BorderColor { get; set; }

        private StiRectangle3D columnRect3D;
        public StiRectangle3D ColumnRect3D
        {
            get
            {
                return columnRect3D;
            }
            set
            {
                this.columnRect3D = value;
                SetVertexes(value);
            }
        }

        public double[] BackFace => new double[] { 3, 1, 4 };

        public double[] LeftFace => new double[] { 0, 1, 3 };

        public double[] BottomFace => new double[] { 0, 3, 4, 2 };

        public double[] RightFace => new double[] { 2, 1, 4 };

        public double[] FrontFace => new double[] { 0, 1, 2 };
        #endregion

        #region Methods
        private void SetVertexes(StiRectangle3D r)
        {
            this.Vertexes = new StiMatrix(new double[,]
               {
                {r.X, r.Y, r.Z + r.Width, 1 },
                {r.X + r.Length/2, r.Y + r.Height, r.Z + r.Width/2, 1 },
                {r.X + r.Length, r.Y, r.Z + r.Width, 1 },
                {r.X, r.Y, r.Z, 1 },
                {r.X + r.Length, r.Y, r.Z, 1 }
               });
        }

        public override void DrawBorder(StiContext context, StiMatrix vertices)
        {
            var color = GetBorderColor();

            if (color == null) return;

            DrawFaceBorder(context, vertices, this.FrontFace, color.GetValueOrDefault());
            DrawFaceBorder(context, vertices, this.RightFace, color.GetValueOrDefault());
        }
        #endregion

        public StiPyramidSeriesElementGeom3D(StiRectangle3D columnRect3D, double value, int index, IStiSeries series, Color color, Color borderColor, StiRender3D render) :
            base(render, value, index, series, color)
        {
            this.ColumnRect3D = columnRect3D;
            this.Color = color;
            this.BorderColor = borderColor;

            this.Faces = new List<double[]>()
            {
                BackFace,
                LeftFace,
                BottomFace,
                RightFace,
                FrontFace
            };

            this.ColorsFaces = new Color[]
            {
                StiColorUtils.ChangeDarkness(color, 0.4f),
                StiColorUtils.ChangeDarkness(color, 0.4f),
                StiColorUtils.ChangeDarkness(color, 0.4f),
                StiColorUtils.ChangeDarkness(color, 0.4f),
                color
            };
        }
    }
}
