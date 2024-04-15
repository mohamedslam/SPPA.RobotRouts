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

using System.Collections;
using System.Drawing;

namespace Stimulsoft.Report.Export
{
    public class StiMatrixBookmarksCollection
    {
        #region Fields
        private StiMatrixCacheManager manager;
        private StiMatrix matrix;
        #endregion

        #region Properties
        public string this[int row, int column]
        {
            get
            {
                return matrix.useCacheMode 
                    ? manager.GetMatrixLineData(row).Bookmarks[column] 
                    : matrix.bookmarks2[row, column];
            }
            set
            {
                if (matrix.useCacheMode)
                    manager.GetMatrixLineData(row).Bookmarks[column] = value;

                else
                    matrix.bookmarks2[row, column] = value;

                if (BookmarksTable == null || value == null) return;

                if (BookmarksTable.ContainsKey(value))
                {
                    var pos = (Size) BookmarksTable[value];
                    if (row < pos.Height || (row == pos.Height && column < pos.Width))
                        BookmarksTable[value] = new Size(column, row);
                }

                else
                    BookmarksTable[value] = new Size(column, row);
            }
        }

        public Hashtable BookmarksTable { get; }
        #endregion

        public StiMatrixBookmarksCollection(StiMatrixCacheManager manager, StiMatrix matrix)
        {
            this.manager = manager;
            this.matrix = matrix;

            if (matrix != null && (matrix.exportFormat == StiExportFormat.Excel || matrix.exportFormat == StiExportFormat.Excel2007))
                BookmarksTable = new Hashtable();
        }
    }
}