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

namespace Stimulsoft.Report.Components.Table
{

    #region StiTableStyleIdent
    public enum StiTableStyleIdent
    {
        Style21,
        Style24,
        Style25,
        Style26,
        Style27
    }
    #endregion

    #region StiTableStyle
    public enum StiTableStyle
    {
        StyleNone = 0,
        Style11 = 11,
        Style12 = 12,
        Style13 = 13,
        Style14 = 14,
        Style15 = 15,
        Style16 = 16,
        Style17 = 17,
        Style18 = 18,
        Style19 = 19,
        Style31 = 31,
        Style32 = 32,
        Style33 = 33,
        Style34 = 34,
        Style35 = 35,
        Style36 = 36,
        Style37 = 37,
        Style38 = 38,
        Style39 = 39,
        Style41 = 41,
        Style42 = 42,
        Style43 = 43,
        Style44 = 44,
        Style45 = 45,
        Style46 = 46,
        Style47 = 47,
        Style48 = 48,
        Style49 = 49,
        Style51 = 51,
        Style52 = 52,
        Style53 = 53,
        Style54 = 54,
        Style55 = 55,
        Style56 = 56,
        Style57 = 57,
        Style58 = 58,
        Style59 = 59
    }

    #endregion

    #region StiTablceCellType
    public enum StiTablceCellType
    {
        Text = 0,
        Image = 1,
        CheckBox = 2,
        RichText = 3
    }
    #endregion

    #region StiTableAutoWidth
    public enum StiTableAutoWidth
    {
        None = 0,
        Page = 1,
        Table = 2
    }
    #endregion

    #region StiTableAutoWidthType
    public enum StiTableAutoWidthType
    {
        None = 0,
        LastColumns = 1,
        FullTable = 2
    }
    #endregion
}
