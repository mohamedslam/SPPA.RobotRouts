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

namespace Stimulsoft.Report.Export
{
    public class StiMatrixBorderSidesYCollection
    {
        #region Fields
        private StiMatrixCacheManager manager;
        private StiMatrix matrix;
        #endregion

        #region Properties
        public StiBorderSide this[int row, int column]
        {
            get
            {
                if (matrix.useCacheMode)
                {
                    var sideIndex = manager.GetMatrixLineData(row).BordersY[column];
                    if (sideIndex == 0) return null;
                    return matrix.BorderSides[sideIndex - 1];
                }
                else
                    return matrix.bordersY2[row, column];
            }
            set
            {
                if (matrix.useCacheMode)
                {
                    var sideIndex = matrix.GetBorderSideIndex(value);
                    manager.GetMatrixLineData(row).BordersY[column] = sideIndex;
                }
                else
                    matrix.bordersY2[row, column] = value;
            }
        }
        #endregion

        public StiMatrixBorderSidesYCollection(StiMatrixCacheManager manager, StiMatrix matrix)
        {
            this.manager = manager;
            this.matrix = matrix;
        }
    }
}