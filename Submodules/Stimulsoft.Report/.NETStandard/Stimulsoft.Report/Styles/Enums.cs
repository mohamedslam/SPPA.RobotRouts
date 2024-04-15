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

using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;
using System;

namespace Stimulsoft.Report
{
	#region StiStyleCode
    /// <summary>
    /// StiStyleCode enumeration is obsolete!
    /// </summary>
	public enum StiStyleCode
	{
        None,
		Header,
        Data,
        Footer,
        SubHeader,
        SubData,
        SubFooter,
        SubDetailHeader,
        SubDetailData,
        SubDetailFooter
	}
	#endregion

    #region StiNestedFactor
    /// <summary>
    /// Enum provide variants of nested factor.
    /// </summary>
    public enum StiNestedFactor
    {
        High,
        Normal,
        Low
    }
    #endregion

    #region StiStyleConditionType
    /// <summary>
    /// Enum provide types of style condition.
    /// </summary>
    [Flags]
    public enum StiStyleConditionType
    {        
        ComponentType = 1,
        Placement = 2,
        PlacementNestedLevel = 4,
        ComponentName = 8,
        Location = 16
    }
    #endregion

    #region StiStyleComponentPlacement
    /// <summary>
    /// Enum provide type of bands on which component can be placed.
    /// </summary>
    [Flags]
    public enum StiStyleComponentPlacement
	{
        None = 0,
        ReportTitle = 1,
        ReportSummary = 2,
        PageHeader = 4,
        PageFooter = 8,
        GroupHeader = 16,
        GroupFooter = 32,
		Header = 64,        
        Footer = 128,
        ColumnHeader = 256,
        ColumnFooter = 512,
        Data = 1024,
        DataEvenStyle = 2048,
        DataOddStyle = 4096,
        Table = 8192,
        Hierarchical = 16384,
        Child = 32768,
        Empty = 65536,
        Overlay = 131072,
        Panel = 262144,
        Page = 524288,
        AllExeptStyles = 
            ReportTitle + ReportSummary + PageHeader + PageFooter + GroupHeader + GroupFooter + Header + Footer + ColumnHeader + ColumnFooter +
            Data + Table + Hierarchical + Child + Empty + Overlay + Panel + Page
	}
	#endregion

    #region StiStyleComponentType
    /// <summary>
    /// Enum provide component type which can be detected by style condition.
    /// </summary>
    [Flags]
    public enum StiStyleComponentType
    {
        Text = 1,
        Primitive = 2,
        Image = 4,
        CrossTab = 8,
        Chart = 16,
        CheckBox = 32
    }
    #endregion   

    #region StiStyleLocation
    /// <summary>
    /// Enum provide all variants of location component on parent component area.
    /// </summary>
    [Flags]
    public enum StiStyleLocation
    {        
        None = 0,

        TopLeft = 1,
        TopCenter = 2,
        TopRight = 4,

        MiddleLeft = 8,
        MiddleCenter = 16,
        MiddleRight = 32,
                
        BottomLeft = 64,
        BottomCenter = 128,
        BottomRight = 256,

        Left = 512,        
        Right = 1024,

        Top = 2048,
        Bottom = 4096,

        CenterHorizontal = 8192,
        CenterVertical = 16384,
    }
    #endregion

    #region StiStyleConditionOperation
    public enum StiStyleConditionOperation
    {
        EqualTo,
        NotEqualTo,
        GreaterThan,
        GreaterThanOrEqualTo,
        LessThan,
        LessThanOrEqualTo,
        Containing,
        NotContaining,
        BeginningWith,
        EndingWith
    }
    #endregion

    #region StiStyleType
    public enum StiStyleType
    {
        Cards,
        Chart,
        CrossTab,
        Table,
        Gauge,
        Map,
        Indicator,
        Progress
    }
    #endregion

    #region StiHeatmapFillType
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiHeatmapFillMode
    {
        Lightness,
        Darkness
    }
    #endregion
}