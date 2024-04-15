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
using System.ComponentModel;

namespace Stimulsoft.Report.Maps
{
    #region StiMapSource
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiMapSource
    {
        Manual,
        DataColumns,
    }
    #endregion
    
    #region StiDisplayNameType
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiDisplayNameType
    {
        None = 1,
        Full,
        Short
    }
    #endregion

    #region StiMapMode
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiMapMode
    {
        Choropleth,
        Online
    }
    #endregion

    #region StiMapCategory
    public enum StiMapCategory
    {
        All = 0,
        Europe = 1,
        NorthAmerica = 2,
        SouthAmerica = 3,
        Asia = 4,
        Oceania = 5,
        Africa = 6,
        Custom = 7,
        PopularMaps = 8,
    }
    #endregion

    #region StiMapID
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiMapID
    {
        World = 1,
        Australia,
        Austria,
        Brazil,
        Canada,
        China,
        ChinaWithHongKongAndMacau,
        ChinaWithHongKongMacauAndTaiwan,
        Taiwan,
        EU,
        EUWithUnitedKingdom,
        Europe,
        EuropeWithRussia,
        France,
        Germany,
        Italy,
        Netherlands,
        Russia,
        UK,
        UKCountries,
        USAAndCanada,
        NorthAmerica,
        SouthAmerica,
        USA,
        Albania,
        Andorra,
        Argentina,
        ArgentinaFD,
        Afghanistan,
        Armenia,
        Azerbaijan,
        Belarus,
        Belgium,
        Bolivia,
        BosniaAndHerzegovina,
        Bulgaria,
        Chile,
        Colombia,
        Croatia,
        Cyprus,
        CzechRepublic,
        Denmark,
        Ecuador,
        Estonia,
        FalklandIslands,
        Finland,
        Georgia,
        Greece,
        Guyana,
        Hungary,
        Iceland,
        India,
        Indonesia,
        Ireland,
        Israel,
        Japan,
        Kazakhstan,
        Latvia,
        Liechtenstein,
        Lithuania,
        Luxembourg,
        Macedonia,
        Malaysia,
        Malta,
        Mexico,
        Moldova,
        Monaco,
        Montenegro,
        NewZealand,
        Norway,
        Paraguay,
        Peru,
        Philippines,
        Poland,
        Portugal,
        Romania,
        SanMarino,
        SaudiArabia,
        Serbia,
        Slovakia,
        Slovenia,
        SouthAfrica,
        SouthKorea,
        Spain,
        Suriname,
        Sweden,
        Switzerland,
        Thailand,
        Turkey,
        Ukraine,
        Uruguay,
        Vatican,
        Venezuela,
        Vietnam,
        MiddleEast,
        Oman,
        Qatar,
        Benelux,
        Scandinavia,
        FranceDepartments,
        France18Regions,
        CentralAfricanRepublic,
        Asia,
        SoutheastAsia,
        Oceania
    }
    #endregion

    #region StiMapStyleIdent
    public enum StiMapStyleIdent
    {
        Style21,
        Style24,
        Style25,
        Style26,
        Style27,
        Style28,
        Style29,
        Style30,
        Style31,
        Style32,
        Style33,
        Style34,
        Style35
    }
    #endregion

    #region StiMapType
    public enum StiMapType
    {
        [Obsolete("Please use StiMapType.Individual")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        None = 0,

        Individual = 4,
        Group = 1,
        Heatmap = 2,
        HeatmapWithGroup = 3,
    }
    #endregion
}