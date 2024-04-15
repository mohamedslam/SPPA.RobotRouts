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

namespace Stimulsoft.Base.Gis
{
    public static class StiGisMapHelper
    {
        #region Properties
        public static string CustomOpenStreetMapUrl { get; set; }

        internal static string YandexMapAPIKey { get; set; }
        #endregion

        #region Methods
        public static string GetProviderName(StiGeoMapProviderType id)
        {
            switch (id)
            {
                case StiGeoMapProviderType.OpenStreetMap:
                    return "Open Street Map";

                case StiGeoMapProviderType.OpenCycleMap:
                    return "Open Cycle Map";

                case StiGeoMapProviderType.OpenCycleMapLandscape:
                    return "Open Cycle Map Landscape";

                case StiGeoMapProviderType.OpenCycleMapTransport:
                    return "Open Cycle Map Transport";

                case StiGeoMapProviderType.Wikimapia:
                    return "Wikimapia";

                case StiGeoMapProviderType.Bing:
                    return "Bing";

                case StiGeoMapProviderType.BingSatellite:
                    return "Bing Satellite";

                case StiGeoMapProviderType.BingHybrid:
                    return "Bing Hybrid";

                case StiGeoMapProviderType.BingOS:
                    return "Bing OS";

                case StiGeoMapProviderType.Google:
                    return "Google";

                case StiGeoMapProviderType.GoogleSatellite:
                    return "Google Satellite";

                case StiGeoMapProviderType.GoogleTerrain:
                    return "Google Terrain";

                case StiGeoMapProviderType.GoogleChina:
                    return "Google China";

                case StiGeoMapProviderType.GoogleChinaSatellite:
                    return "Google China Satellite";

                case StiGeoMapProviderType.GoogleChinaTerrain:
                    return "Google China Terrain";

                case StiGeoMapProviderType.YandexMap:
                    return "Yandex Map";

                case StiGeoMapProviderType.YandexSatelliteMap:
                    return "Yandex Satellite Map";

                case StiGeoMapProviderType.Czech:
                    return "Czech";

                case StiGeoMapProviderType.CzechSatellite:
                    return "Czech Satellite";

                case StiGeoMapProviderType.CzechTurist:
                    return "Czech Turist";

                case StiGeoMapProviderType.CzechTuristWinter:
                    return "Czech Turist Winter";

                case StiGeoMapProviderType.CzechGeographic:
                    return "Czech Geographic";

                case StiGeoMapProviderType.ArcGISStreetMapWorld2D:
                    return "Arc GIS StreetMap World 2D";
            }

            throw new NotSupportedException($"Type '{id}' is not supported");
        }
        #endregion
    }
}