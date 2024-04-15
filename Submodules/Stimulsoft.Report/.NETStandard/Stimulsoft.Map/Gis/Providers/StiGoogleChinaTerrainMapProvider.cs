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
using Stimulsoft.Map.Gis.Core;
using Stimulsoft.Map.Gis.Providers.Providers;

namespace Stimulsoft.Map.Gis.Providers
{
    public class StiGoogleChinaTerrainMapProvider : 
        StiGoogleMapProviderBase
    {
        public StiGoogleChinaTerrainMapProvider()
        {
            RefererUrl = string.Format("http://ditu.{0}/", ServerChina);
        }

        #region Fields
        private static readonly string chinaLanguage = "zh-CN";
        private static readonly string urlFormatServer = "mt";
        private static readonly string urlFormatRequest = "vt";
        private static readonly string urlFormat = "http://{0}{1}.{10}/{2}/lyrs={3}&hl={4}&gl=cn&x={5}{6}&y={7}&z={8}&s={9}";
        #endregion

        #region Properties.override
        public override StiGeoMapProviderType ProviderType => StiGeoMapProviderType.GoogleChinaTerrain;
        #endregion

        #region Methods.override
        public override StiGisMapImage GetTileImage(StiGisPoint pos, int zoom, StiGeoRenderMode mode)
        {
            var url = MakeTileImageUrl(pos, zoom);
            return GetTileImageUsingHttp(url, mode);
        }
        #endregion

        #region Methods
        private string MakeTileImageUrl(StiGisPoint pos, int zoom)
        {
            string sec1; // after &x=...
            string sec2; // after &zoom=...
            GetSecureWords(pos, out sec1, out sec2);

            return string.Format(urlFormat, urlFormatServer, GetServerNum(pos, 4), 
                urlFormatRequest, StiGisGoogleProviderHelper.GoogleChinaTerrainMapVersion, 
                chinaLanguage, pos.X, sec1, pos.Y, zoom, sec2, ServerChina);
        }
        #endregion
    }
}