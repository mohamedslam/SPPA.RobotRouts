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
    public class StiMatrixLineData
    {
        public StiCell[] Cells;
        public StiCell[] CellsMap;
        public int[] BordersX;
        public int[] BordersY;
        public int[] CellStyles;
        public string[] Bookmarks;
        //надо оптимизировать - если еще не было обращений то null, создавать при первом обращении

        public StiMatrixLineData(int size)
        {
            Cells = new StiCell[size];
            CellsMap = new StiCell[size];
            BordersX = new int[size];
            BordersY = new int[size];
            CellStyles = new int[size];
            Bookmarks = new string[size];
        }
    }
}