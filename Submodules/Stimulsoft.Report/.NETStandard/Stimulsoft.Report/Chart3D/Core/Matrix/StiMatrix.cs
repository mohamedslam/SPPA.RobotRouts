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

namespace Stimulsoft.Report.Chart
{
    public class StiMatrix : IEquatable<StiMatrix>
    {
        #region IEquatable
        public bool Equals(StiMatrix other)
        {
            if (this.Grid.GetLength(0) != other.Grid.GetLength(0))
            {
                return false;
            }
            if (this.Grid.GetLength(1) != other.Grid.GetLength(1))
            {
                return false;
            }

            for (int i = 0; i < this.Grid.GetLength(0); i++)
            {
                for (int j = 0; j < this.Grid.GetLength(1); j++)
                {
                    if (this.Grid[i, j] != other.Grid[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        #endregion

        #region Properties
        public double[,] Grid { get; set; }

        public int Rows => Grid?.GetLength(0) ?? 0;

        public int Columns => Grid?.GetLength(1) ?? 0;
        #endregion

        #region Methods
        private static double[,] DeepCopy(double[,] matrix)
        {
            if (matrix == null)
                throw new ArgumentException("cannot copy null matrix");

            int n = matrix.GetLength(0), m = matrix.GetLength(1);

            var copy = new double[n, m];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    copy[i, j] = matrix[i, j];
            return copy;
        }

        public double this[int i, int j]
        {
            get => Grid[i, j];
            set => Grid[i, j] = value;
        }
        #endregion

        #region Operators
        public static StiMatrix operator +(StiMatrix a) => a;

        public static StiMatrix operator -(StiMatrix a)
        {
            StiMatrix b = new StiMatrix(a);
            for (int i = 0; i < b.Rows; i++)
                for (int j = 0; j < b.Columns; j++)
                    b[i, j] = -a[i, j];
            return b;
        }

        public static StiMatrix operator +(StiMatrix a, StiMatrix b)
        {
            if (a.Rows != b.Rows || a.Columns != b.Columns)
                throw new ArgumentException("matrices must be the same size in rows and columns to add");
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Columns; i++)
                    a[i, j] = a[i, j] + b[i, j];
            return a;
        }

        public static StiMatrix operator -(StiMatrix a, StiMatrix b)
        {
            if (a.Rows != b.Rows || a.Columns != b.Columns)
                throw new ArgumentException("matrices must be the same size in rows and columns to add");
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Columns; j++)
                    a[i, j] -= b[i, j];
            return a;
        }

        public static StiMatrix operator *(StiMatrix firstMatrix, StiMatrix secondMatrix)
        {
            if (firstMatrix.Columns != secondMatrix.Rows)
                throw new ArgumentException("cannot multiply matrices with different sized columns/rows");

            int x1 = firstMatrix.Columns;
            int y1 = firstMatrix.Rows;
            var matrixProduct = new StiMatrix(new double[y1, x1]);

            for (int y = 0; y < matrixProduct.Rows; ++y)
                for (int x = 0; x < matrixProduct.Columns; ++x)
                    for (int i = 0; i < firstMatrix.Columns; ++i)
                        matrixProduct[y, x] += firstMatrix[y, i] * secondMatrix[i, x];

            return new StiMatrix(matrixProduct);
        }
        #endregion

        public StiMatrix(double[,] matrix)
        { 
            Grid = DeepCopy(matrix); 
        }

        public StiMatrix(StiMatrix a)
        {
            Grid = DeepCopy(a.Grid);
        }
    }
}
