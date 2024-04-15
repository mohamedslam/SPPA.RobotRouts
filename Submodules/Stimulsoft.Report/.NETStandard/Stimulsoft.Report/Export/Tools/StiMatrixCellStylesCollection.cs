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
    public class StiMatrixCellStylesCollection
    {
        #region Fields
        private StiMatrixCacheManager manager;
        private StiMatrix matrix;
        #endregion

        public StiCellStyle this[int row, int column]
        {
            get
            {
                if (matrix.useCacheMode)
                {
                    var styleIndex = manager.GetMatrixLineData(row).CellStyles[column];
                    if (styleIndex == 0) return null;

                    return matrix.Styles[styleIndex - 1] as StiCellStyle;
                }
                else
                    return matrix.cellStyles2[row, column];
            }
            set
            {
                if (matrix.useCacheMode)
                    manager.GetMatrixLineData(row).CellStyles[column] = matrix.Styles.IndexOf(value) + 1;

                else
                    matrix.cellStyles2[row, column] = value;
            }
        }

        public StiMatrixCellStylesCollection(StiMatrixCacheManager manager, StiMatrix matrix)
        {
            this.manager = manager;
            this.matrix = matrix;
        }
    }
}