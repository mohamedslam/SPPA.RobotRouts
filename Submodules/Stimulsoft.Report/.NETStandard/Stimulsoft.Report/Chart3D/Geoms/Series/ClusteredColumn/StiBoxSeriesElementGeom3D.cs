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
    public class StiBoxSeriesElementGeom3D : 
        StiSeriesElementGeom3D,
        IStiBorderColor,
        IStiColor,
        IStiDrawSidesGeom3D
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

        public double[] BackFace => new double[] { 4, 5, 6, 7 };

        public double[] LeftFace => new double[] { 0, 4, 5, 1 };

        public double[] BottomFace => new double[] { 0, 3, 7, 4 };

        public double[] TopFace => new double[] { 1, 2, 6, 5 };

        public double[] RightFace => new double[] { 2, 3, 7, 6 };

        public double[] FrontFace => new double[] { 0, 1, 2, 3 };

        private bool drawLeft = true;
        public bool DrawLeft
        {
            get
            {
                return drawLeft;
            }
            set
            {
                drawLeft = value;
                BuildFaces();
                BuildColorFaces();
            }
        }

        private bool drawBack = true;
        public bool DrawBack
        {
            get
            {
                return drawBack;
            }
            set
            {
                drawBack = value;
                BuildFaces();
                BuildColorFaces();
            }
        }

        private bool drawTop = true;
        public bool DrawTop
        {
            get
            {
                return drawTop;
            }
            set
            {
                drawTop = value;
                BuildFaces();
                BuildColorFaces();
            }
        }

        private bool drawBottom = true;
        public bool DrawBottom
        {
            get
            {
                return drawBottom;
            }
            set
            {
                drawBottom = value;
                BuildFaces();
                BuildColorFaces();
            }
        }
        #endregion

        #region Methods
        private void SetVertexes(StiRectangle3D r)
        {
            this.Vertexes = new StiMatrix(new double[,]
               {
                {r.X, r.Y, r.Z + r.Width, 1 },
                {r.X, r.Y + r.Height, r.Z + r.Width, 1 },
                {r.X + r.Length, r.Y + r.Height, r.Z + r.Width, 1 },
                {r.X + r.Length, r.Y, r.Z + r.Width, 1 },
                {r.X, r.Y, r.Z, 1 },
                {r.X, r.Y + r.Height, r.Z, 1 },
                {r.X + r.Length, r.Y + r.Height, r.Z, 1 },
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

        protected override List<PointF> GetFacePolygonPoints(double[] face, StiMatrix vertices)
        {
            var polygonPoints = new List<PointF>();

            if (this.IsTopFace(face) && !IsDrawTopFace(vertices))
                return polygonPoints;

            if (this.IsBottomFace(face) && !IsDrawBottomFace(vertices))
                return polygonPoints;

            for (var colIndex = 0; colIndex < face.Length; colIndex++)
            {
                var vertIndex = (int)face[colIndex];

                var x = (float)vertices.Grid[vertIndex, 0];
                var y = (float)vertices.Grid[vertIndex, 1];

                polygonPoints.Add(GetPoint(x, y));
            }

            return polygonPoints;
        }

        private bool IsTopFace(double[] face)
        {
            if (face.Length != this.TopFace.Length) return false;

            for( var index = 0; index < face.Length; index++)
            {
                if (face[index] != this.TopFace[index]) 
                    return false;
            }

            return true;
        }

        private bool IsBottomFace(double[] face)
        {
            if (face.Length != this.BottomFace.Length) return false;

            for (var index = 0; index < face.Length; index++)
            {
                if (face[index] != this.BottomFace[index])
                    return false;
            }

            return true;
        }

        private bool IsDrawTopFace(StiMatrix vertices)
        {
            var point1 = GetPoint(vertices, 1);
            var point2 = GetPoint(vertices, 5);

            if (this.ColumnRect3D.Height > 0)
            {
                return point1.Y > point2.Y;
            }
            else
            {
                return point1.Y < point2.Y;
            }
        }

        private bool IsDrawBottomFace(StiMatrix vertices)
        {
            var point1 = GetPoint(vertices, 0);
            var point2 = GetPoint(vertices, 4);
            
            if (this.ColumnRect3D.Height > 0)
            {
                return point1.Y < point2.Y;
            }
            else
            {
                return point1.Y > point2.Y;
            }
        }

        public void BuildFaces()
        {
            var listFaces = new List<double[]>()
            {
                RightFace,
                FrontFace
            };

            if (DrawTop)
                listFaces.Insert(0, TopFace);

            if (DrawBottom)
                listFaces.Insert(0, BottomFace);

            if (DrawBack)
                listFaces.Insert(0, BackFace);

            if (DrawLeft)
                listFaces.Insert(0, LeftFace);

            this.Faces = listFaces;
        }

        public void BuildColorFaces()
        {
            var colorsFaces = new List<Color>()
            {
                StiColorUtils.ChangeDarkness(Color, 0.4f),
                Color
            };

            if (DrawTop)
                colorsFaces.Insert(0, StiColorUtils.ChangeDarkness(Color, 0.4f));

            if (DrawBottom)
                colorsFaces.Insert(0, StiColorUtils.ChangeDarkness(Color, 0.4f));

            if (DrawBack)
                colorsFaces.Insert(0, StiColorUtils.ChangeDarkness(Color, 0.4f));

            if (DrawLeft)
                colorsFaces.Insert(0, StiColorUtils.ChangeDarkness(Color, 0.4f));

            this.ColorsFaces = colorsFaces.ToArray();
        }
        #endregion

        public StiBoxSeriesElementGeom3D(StiRectangle3D columnRect3D, double value, int index, IStiSeries series, Color color, Color borderColor, StiRender3D render) :
            base(render, value, index, series, color)
        {
            this.ColumnRect3D = columnRect3D;
            this.Color = color;
            this.BorderColor = borderColor;

            this.BuildFaces();
            this.BuildColorFaces();
        }
    }
}
