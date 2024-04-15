﻿#region Copyright (C) 2003-2022 Stimulsoft
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
    public sealed class StiBingHybridMapProvider : 
        StiBingMapProviderBase
    {
        #region Fields
        private string urlDynamicFormat = string.Empty;
        private static readonly string urlFormat = "http://ecn.t{0}.tiles.virtualearth.net/tiles/h{1}.jpeg?g={2}&mkt={3}&n=z{4}";
        #endregion

        #region Properties.override
        public override StiGeoMapProviderType ProviderType => StiGeoMapProviderType.BingHybrid;
        #endregion

        #region Methods.override
        public override StiGisMapImage GetTileImage(StiGisPoint pos, int zoom, StiGeoRenderMode mode)
        {
            var url = MakeTileImageUrl(pos, zoom, LanguageStr);
            return GetTileImageUsingHttp(url, mode);
        }

        public override void OnInitialized()
        {
            base.OnInitialized();

            this.urlDynamicFormat = GetTileUrl("AerialWithLabels");
            if (!string.IsNullOrEmpty(this.urlDynamicFormat))
            {
                this.urlDynamicFormat = this.urlDynamicFormat.Replace("{subdomain}", "t{0}").Replace("{quadkey}", "{1}").Replace("{culture}", "{2}");
            }
        }
        #endregion

        #region Methods
        private string MakeTileImageUrl(StiGisPoint pos, int zoom, string language)
        {
            string key = TileXYToQuadKey(pos.X, pos.Y, zoom);

            if (!string.IsNullOrEmpty(urlDynamicFormat))
            {
                return string.Format(urlDynamicFormat, GetServerNum(pos, 4), key, language);
            }

            return string.Format(urlFormat, GetServerNum(pos, 4), key, Version, language, string.Empty);
        }
        #endregion
    }
}