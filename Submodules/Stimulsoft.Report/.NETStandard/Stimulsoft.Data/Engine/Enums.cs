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

using System;

using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;

namespace Stimulsoft.Data.Engine
{
    #region StiDataJoinType
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiDataJoinType
    {
        Inner = 1,
        Left,
        Right,
        Cross,
        Full
    }
    #endregion

    #region StiDataSortDirection
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiDataSortDirection
    {
        Ascending = 1,
        Descending,
        None
    }
    #endregion

    #region StiDataFilterCondition
    public enum StiDataFilterCondition
    {
        EqualTo,
        NotEqualTo,
        GreaterThan,
        GreaterThanOrEqualTo,
        LessThan,
        LessThanOrEqualTo,
        Between,
        NotBetween,

        Containing,
        NotContaining,
        BeginningWith,
        EndingWith,

        IsNull,
        IsNotNull,
        IsBlank,
        IsNotBlank,

        IsBlankOrNull,

        IsFalse,

        PairEqualTo,
        MapEqualTo,
    }
    #endregion

    #region StiDataFilterOperation
    public enum StiDataFilterOperation
    {
        AND,
        OR
    }
    #endregion

    #region StiDataActionType
    // Sorted by order of use in the table, do not reorder!
    public enum StiDataActionType
    {
        Limit,
        Replace,
        RunningTotal,
        Percentage
    }
    #endregion

    #region StiDataActionPriority
    public enum StiDataActionPriority
    {
        BeforeTransformation,
        AfterGroupingData,
        AfterSortingData
    }
    #endregion

    #region StiDataFilterConditionGroupType
    public enum StiDataFilterConditionGroupType
    {
        Equal,
        NotEqual,
        Custom,
        Empty
    }
    #endregion
    
    #region StiDataRequestOption
    [Flags]
    public enum StiDataRequestOption
    {
        None = 0,
        All = AllowOpenConnections + AllowDataSort,
        AllowOpenConnections = 1,
        AllowDataSort = 2,
        DisallowTransform = 4
    }
    #endregion

    #region StiDataTopNMode
    public enum StiDataTopNMode
    {
        None,
        Top,
        Bottom
    }
    #endregion

    #region StiTableFiltersGroupsType
    public enum StiTableFiltersGroupsType
    {
        None,
        Simple,
        Complex
    }
    #endregion

    #region StiDataFormatKind
    public enum StiDataFormatKind
    {
        General,
        Boolean,
        Currency,
        Date,
        Number,
        Percentage,
        Time
    }
    #endregion
}