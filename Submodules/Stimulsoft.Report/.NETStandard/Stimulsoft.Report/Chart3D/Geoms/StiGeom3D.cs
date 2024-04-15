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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Stimulsoft.Report.Chart
{
    public abstract class StiGeom3D : StiCellGeom
    {
        #region Properties
        public StiRender3D Render { get; set; }

        public StiMatrix Vertexes { get; set; }

        public List<double[]> Faces { get; set; }

        public override StiGeomType Type { get; }

        public bool DrawVertexes { get; set; }

        public bool DrawEdges { get; set; } = true;

        public Color[] ColorsFaces { get; set; }

        public List<PointF> RenderPoints { get; private set; }
        #endregion

        #region Methods
        private static StiMatrix MultiplyVertexes(StiMatrix vertexes, StiMatrix matrix)
        {
            var result = new StiMatrix(new double[vertexes.Rows, vertexes.Columns]);

            for (int y = 0; y < vertexes.Rows; y++)
            {
                var dataRow = new double[1, vertexes.Columns];
                for (int x = 0; x < vertexes.Columns; x++)
                    dataRow[0, x] = vertexes.Grid[y, x];

                var matrixRow = new StiMatrix(dataRow);
                var matrixMul = matrixRow * matrix;
                for (int x = 0; x < matrixMul.Columns; x++)
                    result.Grid[y, x] = matrixMul.Grid[0, x];
            }

            return result;
        }

        private StiMatrix NormilizeVertixes(StiMatrix matrix)
        {
            for (int r = 0; r < matrix.Rows; r++)
            {
                var x = matrix.Grid[r, 0];
                var y = matrix.Grid[r, 1];
                var z = matrix.Grid[r, 2];
                var w = matrix.Grid[r, 3];

                matrix.Grid[r, 0] = x / w;
                matrix.Grid[r, 1] = y / w;
                matrix.Grid[r, 2] = z / w;
                matrix.Grid[r, 3] = w / w;
            }

            return matrix;
        }

        private bool isGlobalTransform;

        protected void GlobalTransform()
        {
            if (isGlobalTransform) return;

            this.Translate(new StiVector3(-0.5, -0.5, 1));

            this.RotateX(this.Render.GlobalRotationX * Math.PI / 180);
            this.RotateY(this.Render.GlobalRotationY * Math.PI / 180);

            this.Scale(this.Render.GlobalScale);

            isGlobalTransform = true;
        }

        internal List<PointF> GetPoints()
        {
            GlobalTransform();
            var vertices = this.ScreenProjection();

            var points = new List<PointF>();

            for (int y = 0; y < vertices.Rows; y++)
            {
                var vertX = vertices.Grid[y, 0];
                var vertY = vertices.Grid[y, 1];

                points.Add(GetPoint((float)vertX, (float)vertY));
            }

            return points;
        }

        public override void Draw(StiContext context)
        {
            if (this.Vertexes == null) return;

            GlobalTransform();
            var vertices = this.ScreenProjection();

            DrawElements(context, vertices);
        }

        public virtual RectangleF MeasureCientRect()
        {
            GlobalTransform();
            var vertices = this.ScreenProjection();

            var points = new List<PointF>();

            for (int y = 0; y < vertices.Rows; y++)
            {
                var vertX = vertices.Grid[y, 0];
                var vertY = vertices.Grid[y, 1];

                points.Add(new PointF((float)vertX, (float)vertY));
            }

            return GetClientRectangle(points);
        }

        private RectangleF GetClientRectangle(List<PointF> points)
        {
            var minX = 0f;
            var minY = 0f;
            var maxX = 0f;
            var maxY = 0f;

            var first = true;

            foreach (var p in points)
            {
                if (first)
                {
                    minX = p.X;
                    minY = p.Y;
                    maxX = p.X;
                    maxX = p.Y;

                    first = false;
                }

                else
                {
                    minX = Math.Min(minX, p.X);
                    minY = Math.Min(minY, p.Y);
                    maxX = Math.Max(maxX, p.X);
                    maxY = Math.Max(maxY, p.Y);
                }
            }

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        public StiMatrix ScreenProjection()
        {
            var vertices = MultiplyVertexes(this.Vertexes, this.Render.Camera.CameraMatrix());
            vertices = MultiplyVertexes(vertices, this.Render.Projection.ProjectionMatrix);
            vertices = NormilizeVertixes(vertices);

            //
            return MultiplyVertexes(vertices, this.Render.Projection.ToScreenMatrix);
        }

        public virtual void DrawElements(StiContext context, StiMatrix vertices)
        {
            context.PushSmoothingModeToAntiAlias();

            DrawFaces(context, vertices);
            DrawBorder(context, vertices);

            //test draw vertices
            //if (false)
            //{
            //    var points = new List<PointF>();

            //    for (int y = 0; y < vertices.Rows; y++)
            //    {
            //        var vertX = vertices.Grid[y, 0];
            //        var vertY = vertices.Grid[y, 1];

            //        points.Add(GetPoint((float)vertX, (float)vertY));
            //    }

            //    foreach (var p in points)
            //    {
            //        context.DrawEllipse(new StiPenGeom(Brushes.Red, 2), new RectangleF(p.X - 1, p.Y - 1, 2, 2));
            //    }
            //}

            context.PopSmoothingMode();
        }

        public virtual void DrawFaces(StiContext context, StiMatrix vertices)
        {
            for (var rowIndex = 0; rowIndex < this.Faces.Count; rowIndex++)
            {
                var polygonPoints = GetFacePolygonPoints(this.Faces[rowIndex], vertices);

                if (this is IStiColor color && polygonPoints.Count > 2)
                {
                    var list = new List<StiSegmentGeom>();

                    list.Add(new StiLinesSegmentGeom(polygonPoints.ToArray()));

                    var colorFill = ColorsFaces == null ? color.Color : ColorsFaces[rowIndex];

                    if (this is StiSeriesElementGeom3D seriesElementGeom3D)
                    {
                        if (seriesElementGeom3D.IsSelected)
                            colorFill = StiColorUtils.Dark(colorFill, 30);

                        else if (seriesElementGeom3D.IsMouseOver)
                            colorFill = StiColorUtils.Light(colorFill, 25);
                    }

                    context.FillPath(colorFill, list, null, null, -1, GetToolTip());
                }
            }
        }

        protected virtual List<PointF> GetFacePolygonPoints(double[] face, StiMatrix vertices)
        {
            var polygonPoints = new List<PointF>();

            for (var colIndex = 0; colIndex < face.Length; colIndex++)
            {
                var vertIndex = (int)face[colIndex];

                var x = (float)vertices.Grid[vertIndex, 0];
                var y = (float)vertices.Grid[vertIndex, 1];

                polygonPoints.Add(GetPoint(x, y));
            }

            return polygonPoints;
        }

        public virtual void DrawBorder(StiContext context, StiMatrix vertices)
        {
            var color = GetBorderColor();

            if (color == null) return;

            for (var rowIndex = 0; rowIndex < this.Faces.Count; rowIndex++)
            {
                var face = this.Faces[rowIndex];

                DrawFaceBorder(context, vertices, face, color.GetValueOrDefault());
            }
        }

        protected void DrawFaceBorder(StiContext context, StiMatrix vertices, double[] face, Color color)
        {
            var polygonPoints = new List<PointF>();

            for (var colIndex = 0; colIndex < face.Length; colIndex++)
            {
                var vertIndex = (int)face[colIndex];
                var point = GetPoint(vertices, vertIndex);

                polygonPoints.Add(point);
            }

            polygonPoints.Add(polygonPoints[0]);            

            context.DrawLines(new StiPenGeom(color, 1), polygonPoints.ToArray());
        }

        protected Color? GetBorderColor()
        {
            if (this is IStiBorderColor borderColor && borderColor.BorderColor.A != 0)
                return borderColor.BorderColor.A > 10 ? borderColor.BorderColor : Color.White;

            return null;
        }

        protected PointF GetPoint(StiMatrix vertices, int vertIndex)
        {
            var x = (float)vertices.Grid[vertIndex, 0];
            var y = (float)vertices.Grid[vertIndex, 1];

            return GetPoint(x, y);
        }

        protected PointF GetPoint(float x, float y)
        {
            var corX = x * this.Render.ContextScale - this.Render.ContextTranslate.X;
            var corY = y * this.Render.ContextScale - this.Render.ContextTranslate.Y;

            return new PointF((float)corX, (float)corY);
        }

        internal virtual string GetToolTip()
        {
            return null;
        }

        public void Translate(StiVector3 vector)
        {
            var matrix = StiMatrixHelper.Translate(vector.X, vector.Y, vector.Z);
            this.Vertexes = MultiplyVertexes(this.Vertexes, matrix);
        }

        public void Scale(double scaleTo)
        {
            var matrix = StiMatrixHelper.Scale(scaleTo);
            this.Vertexes = MultiplyVertexes(this.Vertexes, matrix);
        }

        public void RotateX(double angle)
        {
            var matrix = StiMatrixHelper.RotateX(angle);
            this.Vertexes = MultiplyVertexes(this.Vertexes, matrix);
        }

        public void RotateY(double angle)
        {
            var matrix = StiMatrixHelper.RotateY(angle);
            this.Vertexes = MultiplyVertexes(this.Vertexes, matrix);
        }

        public void RotateZ(double angle)
        {
            var matrix = StiMatrixHelper.RotateZ(angle);
            this.Vertexes = MultiplyVertexes(this.Vertexes, matrix);
        }
        #endregion

        public StiGeom3D(StiRender3D render) : base(RectangleF.Empty)
        {
            this.Render = render;
        }
    }
}
