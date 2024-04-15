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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiCylinderSeriesElementGeom3D : 
        StiGeom3D,
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
                BuildData(value);
            }
        }
        #endregion

        private int count = 360;

        #region Methods
        private void BuildData(StiRectangle3D r)
        {
            var points = GetCirclePoints();

            #region Vertexes
            var array = new double[2 * count, 4];

            for (int i = 0; i < count; i++)
            {
                var p = points[i];

                array[i, 0] = p.X;
                array[i, 1] = r.Z;
                array[i, 2] = p.Y;
                array[i, 3] = 1;
            }

            for (int i = count; i < 2 * count; i++)
            {
                var p = points[i - count];

                array[i, 0] = p.X;
                array[i, 1] = r.Z + r.Height;
                array[i, 2] = p.Y;
                array[i, 3] = 1;
            }

            this.Vertexes = new StiMatrix(array);
            #endregion

            #region Faces

            var arrayTop = new double[count];
            for (int i = 0; i < count; i++)
                arrayTop[i] = i;

            var arrayBottom = new double[count];
            for (int i = 0; i < count; i++)
                arrayBottom[i] = count + i;

            var faces = new List<double[]>();

            faces.Add(arrayBottom);
            for (int i = 0; i < count - 1; i++)
            {
                var faceArray = new double[4] {i, i+1, i+ count + 1, i+ count };
                faces.Add(faceArray);
            }
            faces.Add(arrayTop);

            this.Faces = faces;
            #endregion
        }
        #endregion

        private List<PointF> GetCirclePoints()
        {
            float X = (float)(this.ColumnRect3D.Length / 2 + this.columnRect3D.X);
            float Y = (float)(this.ColumnRect3D.Width / 2 + this.columnRect3D.Z);
            float R = (float)(this.ColumnRect3D.Length / 2);

            var list = new List<PointF>();

            double angle;
            for (var i = 0; i < this.count; i++)
            {
                angle = 2 * Math.PI * i / this.count;
                var x = R * Math.Cos(angle) + X;
                var y = R * Math.Sin(angle) + Y;

                list.Add(new PointF((float)x, (float)y));
            }

            return list;
        }

        public StiCylinderSeriesElementGeom3D(StiRectangle3D columnRect3D, Color color, Color borderColor, StiRender3D render) :
            base(render)
        {
            this.ColumnRect3D = columnRect3D;
            this.Color = color;
            this.BorderColor = borderColor;

            var colors = new Color[2 * count + 2];

            colors[0] = color;
            for (var i = 1; i <= 2 * this.count; i++)
            {
                var count34 = this.count * 3 / 4;

                if (i > this.count / 2 && i < count34)
                    colors[i] = StiColorUtils.ChangeDarkness(color, 0.4f * (float)i / (float)count34);

                else if (i > count34 && i < this.count)
                    colors[i] = StiColorUtils.ChangeDarkness(color, 0.4f * (1 - (float)i / (float)count));

                else
                    colors[i] = StiColorUtils.ChangeDarkness(color, 0.4f);
            };

            colors[2 * count] = color;

            this.ColorsFaces = colors;
        }
    }
}
