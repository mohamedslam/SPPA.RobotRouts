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

namespace Stimulsoft.Map.Gis.Providers
{
    // http://www.mapy.cz/
    public sealed class StiCzechTuristMapProvider : 
        StiCzechMapProviderBase
    {
        #region Fields
        static readonly string UrlFormat = "https://mapserver.mapy.cz/turist-m/{1}-{2}-{3}";
        #endregion

        #region Properties.override
        public override StiGeoMapProviderType ProviderType => StiGeoMapProviderType.CzechTurist;
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
            // http://m3.mapserver.mapy.cz/wtourist-m/14-8802-5528
            // https://mapserver.mapy.cz/turist-m/11-1101-692
            return string.Format(UrlFormat, GetServerNum(pos, 3) + 1, zoom, pos.X, pos.Y);
        }
        #endregion
    }
}