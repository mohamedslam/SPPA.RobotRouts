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
using System.Linq;

namespace Stimulsoft.Report.Components.Table
{
    public class StiColumnSize
    {
        #region Fields
        private double[] widths;
        private bool[] fixedColumns;
        #endregion

        #region Properties
        public int Length => widths.Length;
        #endregion

        #region Methods
        public void SetFixedColumn(int indexCol, double width)
        {
            fixedColumns[indexCol] = true;
            if (widths[indexCol] < width)
                widths[indexCol] = width;
        }

        public void Add(int indexCol, double width)
        {
            if (!fixedColumns[indexCol])
                widths[indexCol] += width;
        }

        public void AddLastNotFixed(double width)
        {
            for (var indexCol = fixedColumns.Length - 1; indexCol != 0; indexCol--)
            {
                if (fixedColumns[indexCol]) continue;

                widths[indexCol] += width;
                return;
            }
        }

        public void Subtract(int indexCol, double width)
        {
            if (!fixedColumns[indexCol])
                widths[indexCol] -= width;
        }

        public void SetWidth(int indexCol, double width)
        {
            if (!fixedColumns[indexCol])
                widths[indexCol] = width;
        }

        public bool GetFixed(int index)
        {
            return fixedColumns[index];
        }

        public int GetCountNotFixedColumn()
        {
            return fixedColumns.Count(column => !column);
        }

        public double GetWidth(int indexCol)
        {
            return widths[indexCol];
        }

        public void Normalize()
        {
            var sum = 0m;
            var lastValue = 0m;
            for (var index = 0; index < widths.Length; index++)
            {
                sum += (decimal)widths[index];
                var value = Math.Round(sum - lastValue, 2);
                widths[index] = (double)value;
                lastValue += value;
            }
        }

        public StiColumnSize(int size)
        {
            widths = new double[size];
            fixedColumns = new bool[size];
            for (var index = 0; index < size; index++)
            {
                widths[index] = 0;
                fixedColumns[index] = false;
            }
        }
        #endregion
    }
}