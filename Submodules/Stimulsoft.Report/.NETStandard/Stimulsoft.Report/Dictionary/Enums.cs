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

namespace Stimulsoft.Report.Dictionary
{
    #region StiVariableInitBy
    /// <summary>
    /// Enum contains types of initialization of variable on start of report rendering.
    /// </summary>
    public enum StiVariableInitBy
    {
        /// <summary>
        /// Initialization based on specified value.
        /// </summary>
        Value,
        /// <summary>
        /// Initialization based on specified expression.
        /// </summary>
        Expression
    }
    #endregion

    #region StiVariableSortField
    /// <summary>
    /// Enum contains types of the fields for sorting.
    /// </summary>
    public enum StiVariableSortField
    {
        Key,
        Label
    }
    #endregion

    #region StiVariableSortDirection
    /// <summary>
    /// Enum contains types of the sort direction.
    /// </summary>
    public enum StiVariableSortDirection
    {
        None,
        Asc,
        Desc
    }
    #endregion

    #region StiDateTimeType
    /// <summary>
    /// Enum contains types of using DateTime variable in dialogs.
    /// </summary>
    public enum StiDateTimeType
    {
        /// <summary>
        /// Only date.
        /// </summary>
        Date,
        /// <summary>
        /// Date and time.
        /// </summary>
        DateAndTime,
        /// <summary>
        /// Only time.
        /// </summary>
        Time
    }
    #endregion

    #region StiItemsInitializationType
    /// <summary>
    /// Enum describes methods of items elements initialization.
    /// </summary>
    public enum StiItemsInitializationType
    {
        /// <summary>
        /// User defined values.
        /// </summary>
        Items,
        /// <summary>
        /// Keys and values from data columns.
        /// </summary>
        Columns
    }
    #endregion

    #region StiTypeMode
    /// <summary>
    /// Enum contains types of type using: Single Value, Single Nullable Value, Array or Range of values.
    /// </summary>
    public enum StiTypeMode
    {
        /// <summary>
        /// One single value.
        /// </summary>
        Value,
        /// <summary>
        /// One single value. Null values acceptable.
        /// </summary>
        NullableValue,
        /// <summary>
        /// List of values.
        /// </summary>
        List,
        /// <summary>
        /// Range of values.
        /// </summary>
        Range
    }
    #endregion

	#region StiSortOrder
	public enum StiSortOrder
	{
		Asc,
		Desc
	}
	#endregion

	#region StiAutoSynchronizeMode
	/// <summary>
	/// Sets the time of the Synchronization dictionary.
	/// </summary>
	public enum StiAutoSynchronizeMode
	{
		None,
		IfDictionaryEmpty,
		Always
	}
	#endregion

	#region StiRestrictionTypes
	[Flags]
	public enum StiRestrictionTypes
	{
		None = 0,
		DenyEdit = 1,
		DenyDelete = 2,
		DenyMove = 4,
		DenyShow = 8		
	}
	#endregion

	#region StiDataType
	public enum StiDataType
	{
        BusinessObject,
		DataSource,
		DataRelation,
		DataColumn,
		Database,
        Resource,
		Variable,
        Total
	}
	#endregion

    #region StiTotalEvent
    public enum StiTotalEvent
    {
        Never,
        OnEachRecord,
        OnGroupChanged,
        OnPageChanged,
        OnColumnChanged,
        OnEachNewBand,
        OnExpressionChanged
    }
    #endregion

    #region StiResourceType
    public enum StiResourceType
    {
        Image,
        Csv,
        Dbf,
        Json,
        Gis,
        Xml,
        Xsd,
        Excel,
        Rtf,
        Txt,
        Report,
        ReportSnapshot,
        FontTtc,
        FontTtf,
        FontOtf,
        FontEot,
        FontWoff,
        Pdf,
        Word,
        Map
    }
    #endregion

	#region StiPropertiesProcessingType
	public enum StiPropertiesProcessingType
	{
		All,
		Browsable
	}
	#endregion

    #region StiFieldsProcessingType
    public enum StiFieldsProcessingType
    {
        All,
        Browsable
    }
    #endregion

	#region StiConnectionOrder
	public enum StiConnectionOrder
	{
		None = 0x00,
		Standard = 0x01,		
		Sql = 0x02
	}
	#endregion

    #region StiSqlSourceType
    public enum StiSqlSourceType
    {
        Table,
        StoredProcedure
    }
    #endregion

    #region StiColumnsSynchronizationMode
    public enum StiColumnsSynchronizationMode
    {
        KeepAbsentColumns,
        RemoveAbsentColumns
    }
    #endregion

    #region StiColumnTypeSynchronizationMode
    public enum StiColumnTypeSynchronizationMode
    {
        KeepAsIs,
        Update
    }
    #endregion

    #region StiSelectionMode
    public enum StiSelectionMode
    {
        FromVariable,
        Nothing,
        First
    }
    #endregion
}