#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

namespace Stimulsoft.Report.Controls
{
    #region StiRichTextAlignment
    public enum StiRichTextAlignment
    {
        /// <summary>
        /// The text is aligned to the left.
        /// </summary>
        Left = 1,

        /// <summary>
        /// The text is aligned to the right.
        /// </summary>
        Right = 2,

        /// <summary>
        /// The text is aligned in the center.
        /// </summary>
        Center = 3,

        /// <summary>
        /// The text is justified.
        /// </summary>
        Justify = 4
    }
    #endregion

    #region StiBorderStyle
    public enum StiBorderStyle
    {
        RaisedOuter = 1,
        SunkenOuter = 2,
        RaisedInner = 4,
        SunkenInner = 8,
        Bump = 9,
        Etched = 6,
        Flat = 0x400a,
        Raised = 5,
        Sunken = 10,
        None = 0
    }
    #endregion

    #region StiTreeViewDrawMode
    public enum StiTreeViewDrawMode
    {
        Normal = 0,
        OwnerDraw = 1,
    }
    #endregion
}
