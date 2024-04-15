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

using Stimulsoft.Base.Gis;
using System;

namespace Stimulsoft.Map.Gis.Providers.Helpers
{
    public static class StiGisMapProviderHelper
    {
        #region Methods
        public static StiGisMapProvider CreateProvider(StiGeoMapProviderType id)
        {
            switch (id)
            {
                case StiGeoMapProviderType.OpenStreetMap:
                    return new StiOpenStreetMapProvider();

                case StiGeoMapProviderType.OpenCycleMap:
                    return new StiOpenCycleMapProvider();

                case StiGeoMapProviderType.OpenCycleMapLandscape:
                    return new StiOpenCycleLandscapeMapProvider();

                case StiGeoMapProviderType.OpenCycleMapTransport:
                    return new StiOpenCycleTransportMapProvider();

                case StiGeoMapProviderType.Wikimapia:
                    return new StiWikimapiaMapProvider();

                case StiGeoMapProviderType.Bing:
                    return new StiBingMapProvider();

                case StiGeoMapProviderType.BingSatellite:
                    return new StiBingSatelliteMapProvider();

                case StiGeoMapProviderType.BingHybrid:
                    return new StiBingHybridMapProvider();

                case StiGeoMapProviderType.BingOS:
                    return new StiBingOSMapProvider();

                case StiGeoMapProviderType.Google:
                    return new StiGoogleMapProvider();

                case StiGeoMapProviderType.GoogleSatellite:
                    return new StiGoogleSatelliteMapProvider();

                case StiGeoMapProviderType.GoogleTerrain:
                    return new StiGoogleTerrainMapProvider();

                case StiGeoMapProviderType.GoogleChina:
                    return new StiGoogleChinaMapProvider();

                case StiGeoMapProviderType.GoogleChinaSatellite:
                    return new StiGoogleChinaSatelliteMapProvider();

                case StiGeoMapProviderType.GoogleChinaTerrain:
                    return new StiGoogleChinaTerrainMapProvider();

                case StiGeoMapProviderType.YandexMap:
                    return new StiYandexMapProvider();

                case StiGeoMapProviderType.YandexSatelliteMap:
                    return new StiYandexSatelliteMapProvider();

                case StiGeoMapProviderType.Czech:
                    return new StiCzechMapProvider();

                case StiGeoMapProviderType.CzechSatellite:
                    return new StiCzechSatelliteMapProvider();

                case StiGeoMapProviderType.CzechTurist:
                    return new StiCzechTuristMapProvider();

                case StiGeoMapProviderType.CzechTuristWinter:
                    return new StiCzechTuristWinterMapProvider();

                case StiGeoMapProviderType.CzechGeographic:
                    return new StiCzechGeographicMapProvider();

                case StiGeoMapProviderType.ArcGISStreetMapWorld2D:
                    return new StiArcGISStreetMapWorld2DMapProvider();
            }

            throw new NotSupportedException($"Type '{id}' is not supported.");
        }
        #endregion
    }
}