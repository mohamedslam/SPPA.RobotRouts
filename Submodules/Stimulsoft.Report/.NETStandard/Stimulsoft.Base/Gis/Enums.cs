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

using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;
using System.ComponentModel;

namespace Stimulsoft.Base.Gis
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiGeoMapProviderType
    {
        OpenStreetMap = 0,
        OpenCycleMap,
        OpenCycleMapLandscape,
        OpenCycleMapTransport,
        Wikimapia,
        Bing,
        BingSatellite,
        BingHybrid,
        BingOS,
        Google,
        GoogleSatellite,
        GoogleTerrain,
        GoogleChina,
        GoogleChinaSatellite,
        GoogleChinaTerrain,
        YandexMap,
        YandexSatelliteMap,
        Czech,
        CzechSatellite,
        CzechTurist,
        CzechTuristWinter,
        CzechGeographic,
        ArcGISStreetMapWorld2D
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiLanguageType
    {
        [Description("ar")]
        Arabic,
        [Description("bg")]
        Bulgarian,
        [Description("bn")]
        Bengali,
        [Description("ca")]
        Catalan,
        [Description("cs")]
        Czech,
        [Description("da")]
        Danish,
        [Description("de")]
        German,
        [Description("el")]
        Greek,
        [Description("en")]
        English,
        [Description("en-AU")]
        EnglishAustralian,
        [Description("en-GB")]
        EnglishGreatBritain,
        [Description("es")]
        Spanish,
        [Description("eu")]
        Basque,
        [Description("fa")]
        FARSI,
        [Description("fi")]
        Finnish,
        [Description("fil")]
        Filipino,
        [Description("fr")]
        French,
        [Description("gl")]
        Galician,
        [Description("gu")]
        Gujarati,
        [Description("hi")]
        Hindi,
        [Description("hr")]
        Croatian,
        [Description("hu")]
        Hungarian,
        [Description("id")]
        Indonesian,
        [Description("it")]
        Italian,
        [Description("iw")]
        Hebrew,
        [Description("ja")]
        Japanese,
        [Description("kn")]
        Kannada,
        [Description("ko")]
        Korean,
        [Description("lt")]
        Lithuanian,
        [Description("lv")]
        Latvian,
        [Description("ml")]
        Malayalam,
        [Description("mr")]
        Marathi,
        [Description("nl")]
        Dutch,
        [Description("nn")]
        NorwegianNynorsk,
        [Description("no")]
        Norwegian,
        [Description("or")]
        Oriya,
        [Description("pl")]
        Polish,
        [Description("pt")]
        Portuguese,
        [Description("pt-BR")]
        PortugueseBrazil,
        [Description("pt-PT")]
        PortuguesePortugal,
        [Description("rm")]
        Romansch,
        [Description("ro")]
        Romanian,
        [Description("ru")]
        Russian,
        [Description("sk")]
        Slovak,
        [Description("sl")]
        Slovenian,
        [Description("sr")]
        Serbian,
        [Description("sv")]
        Swedish,
        [Description("tl")]
        TAGALOG,
        [Description("ta")]
        Tamil,
        [Description("te")]
        Telugu,
        [Description("th")]
        Thai,
        [Description("tr")]
        Turkish,
        [Description("uk")]
        Ukrainian,
        [Description("vi")]
        Vietnamese,
        [Description("zh-CN")]
        ChineseSimplified,
        [Description("zh-TW")]
        ChineseTraditional,
    }
}