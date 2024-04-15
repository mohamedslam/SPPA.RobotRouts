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

using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Stimulsoft.Drawing.Drawing2D
{
    public class Matrix : IDisposable
    {
        internal Matrix3x2 matrix;
        internal System.Drawing.Drawing2D.Matrix netMatrix;

        private float offsetX = 0;
        public float OffsetX { get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netMatrix.OffsetX;
                else
                    return offsetX;
            }
        }

        private float offsetY = 0;
        public float OffsetY
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netMatrix.OffsetY;
                else
                    return offsetY;
            }
        }

        public float[] Elements {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netMatrix.Elements;
                else
                    return new float[] {
                    matrix.M11,
                    matrix.M12,
                    matrix.M21,
                    matrix.M22,
                    matrix.M31,
                    matrix.M32
                };
            }
        }

        public void Translate(float offsetX, float offsetY)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netMatrix.Translate(offsetX, offsetY);
            else
                Translate(offsetX, offsetY, System.Drawing.Drawing2D.MatrixOrder.Prepend);
        }

        public void Translate(float offsetX, float offsetY, System.Drawing.Drawing2D.MatrixOrder order)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netMatrix.Translate(offsetX, offsetY, (System.Drawing.Drawing2D.MatrixOrder)order);
            else
            {
                this.offsetX += offsetX;
                this.offsetY += offsetY;

                if (order == System.Drawing.Drawing2D.MatrixOrder.Prepend)
                    matrix = Matrix3x2.CreateTranslation(offsetX, offsetY) * matrix;
                else
                    matrix *= Matrix3x2.CreateTranslation(offsetX, offsetY);
            }
        }

        public void Rotate(float angle)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netMatrix.Rotate(angle);
            else
                Rotate(angle, System.Drawing.Drawing2D.MatrixOrder.Prepend);
        }

        public void Rotate(float angle, System.Drawing.Drawing2D.MatrixOrder order)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netMatrix.Rotate(angle, (System.Drawing.Drawing2D.MatrixOrder)order);
            else
            {
                if (order == System.Drawing.Drawing2D.MatrixOrder.Prepend)
                    matrix = Matrix3x2.CreateRotation(angle * (float)Math.PI / 180) * matrix;
                else
                    matrix *= Matrix3x2.CreateRotation(angle * (float)Math.PI / 180);
            }
        }

        public void RotateAt(float angle, PointF centerPoint)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netMatrix.RotateAt(angle, centerPoint);
            else
                matrix = Matrix3x2.CreateRotation(angle * (float)Math.PI / 180, new Vector2(centerPoint.X, centerPoint.Y)) * matrix;
        }

        public void Scale(float scaleX, float scaleY)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netMatrix.Scale(scaleX, scaleY);
            else
                matrix = Matrix3x2.CreateScale(scaleX, scaleY) * matrix;
        }

        public void TransformPoints(PointF[] points)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netMatrix.TransformPoints(points);
            else
            {
                for (var index = 0; index < points.Length; index++)
                {
                    var vector = Vector2.Transform(new Vector2(points[index].X, points[index].Y), matrix);
                    points[index].X += vector.X;
                    points[index].Y += vector.Y;
                }
            }
        }

        public void Multiply(Matrix matrix, System.Drawing.Drawing2D.MatrixOrder order)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netMatrix.Multiply(matrix.netMatrix, order);
            else
            {
                if (order == System.Drawing.Drawing2D.MatrixOrder.Prepend)
                    this.matrix = matrix.matrix * this.matrix;
                else
                    this.matrix *= matrix.matrix;
            }
        }

        public void Reset()
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netMatrix.Reset();
            else
                matrix = new Matrix3x2(1, 0, 0, 1, 0, 0);
        }

        public Matrix Clone()
        {
            return (Matrix)MemberwiseClone();
        }

        public void Dispose()
        {
        }

        public Matrix() : this(1, 0, 0, 1, 0, 0)
        {
        }

        public Matrix(float m11, float m12, float m21, float m22, float dx, float dy)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netMatrix = new System.Drawing.Drawing2D.Matrix(m11, m12, m21, m22, dx, dy);
            else
                matrix = new Matrix3x2(m11, m12, m21, m22, dx, dy);
        }

        public static implicit operator Matrix(System.Drawing.Drawing2D.Matrix netMatrix)
        {
            var matrix = new Matrix(
                netMatrix.Elements[0],
                netMatrix.Elements[1],
                netMatrix.Elements[2],
                netMatrix.Elements[3],
                netMatrix.Elements[4],
                netMatrix.Elements[5]);

            matrix.offsetX = netMatrix.OffsetX;
            matrix.offsetY = netMatrix.OffsetY;
            matrix.netMatrix = netMatrix;

            return matrix;
        }
    }
}
