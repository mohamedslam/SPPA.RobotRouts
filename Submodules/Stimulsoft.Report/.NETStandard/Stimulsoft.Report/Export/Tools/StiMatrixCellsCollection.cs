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

namespace Stimulsoft.Report.Export
{
    public class StiMatrixCellsCollection
    {
        #region Fields
        private StiMatrixCacheManager manager;
        private StiMatrix matrix;
        #endregion

        #region Properties
        public StiCell this[int row, int column]
        {
            get
            {
                return matrix.useCacheMode 
                    ? manager.GetMatrixLineData(row).Cells[column] 
                    : matrix.cells2[row, column];
            }
            set
            {
                if (matrix.useCacheMode)
                    manager.GetMatrixLineData(row).Cells[column] = value;

                else
                    matrix.cells2[row, column] = value;
            }
        }
        #endregion

        public StiMatrixCellsCollection(StiMatrixCacheManager manager, StiMatrix matrix)
        {
            this.manager = manager;
            this.matrix = matrix;
        }
    }
}