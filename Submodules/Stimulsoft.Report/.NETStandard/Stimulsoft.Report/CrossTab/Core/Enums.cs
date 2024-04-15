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

namespace Stimulsoft.Report.CrossTab.Core
{
	#region StiSortDirection
	public enum StiSortDirection
	{
		Asc,
		Desc,
		None
	}
	#endregion

	#region StiSummaryType
	public enum StiSummaryType
	{
		None,
		Sum,
		Average,
		Min,
		Max,
		Count,
		CountDistinct,
		Image,
        Median
    }
	#endregion

    #region StiSummaryValues
    public enum StiSummaryValues
    {
        AllValues,
        SkipZerosAndNulls,
        SkipNulls
    }
    #endregion
	
	#region StiSortType
	public enum StiSortType
	{
		ByValue,
		ByDisplayValue
	}
	#endregion

	#region StiFieldType
	public enum StiFieldType
	{
		Column,
		Row,
		Cell
	}
	#endregion

	#region StiSummaryDirection
	public enum StiSummaryDirection
	{
		LeftToRight,
		UpToDown
	}
	#endregion

    #region StiEnumeratorType
	public enum StiEnumeratorType
	{
		None,
		Arabic,
		Roman,
		ABC
	}
    #endregion

    #region StiEnumeratorSeparator
	public enum StiEnumeratorSeparator
	{
		Dot,
		Dash,
		Colon,
		RoundBrackets,
		SquareBrackets
	}
    #endregion
}
