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

namespace Stimulsoft.Report.CrossTab
{
    #region StiCrossHorAlignment
    /// <summary>
    /// Horizontal alignment of an Cross-Tab.
    /// </summary>
    public enum StiCrossHorAlignment
    {
        /// <summary>
        /// Align to left side.
        /// </summary>
        Left,
        /// <summary>
        /// Align to center.
        /// </summary>
        Center,
        /// <summary>
        /// Align to right side.
        /// </summary>
        Right,
        /// <summary>
        /// Without align.
        /// </summary>
        None,
        /// <summary>
        /// Adjust CrossTab by Width.
        /// </summary>
        Width
    }
    #endregion


    #region CellType
     public enum StiCellType
    {
        HeaderCol,
        HeaderColMain,
        HeaderColTotal,
        HeaderColTotalMain,
        HeaderColSummary,
        HeaderColSummaryTotal,
        HeaderRow,
        HeaderRowMain,
        HeaderRowTotal,
        HeaderRowTotalMain,
        HeaderRowSummary,
        HeaderRowSummaryTotal,

        Cell,
        CellTotal,
        LeftTopLine,
        LeftTopLineMain,
        RightTopLine,
        RightTopLineMain,
        CornerCol,
        CornerColMain,
        CornerRow,
        CornerRowMain
    }
    #endregion
}
