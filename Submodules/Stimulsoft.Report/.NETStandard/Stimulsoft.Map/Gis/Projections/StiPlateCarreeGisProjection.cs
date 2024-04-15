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

using Stimulsoft.Map.Gis.Core;
using System;

namespace Stimulsoft.Map.Gis.Projections
{
    internal sealed class StiPlateCarreeGisProjection :
        StiGisProjection
    {
        #region Fields
        public static readonly StiPlateCarreeGisProjection Instance = new StiPlateCarreeGisProjection();
        #endregion

        #region consts
        private const double MinLatitude = -85.05112878;
        private const double MaxLatitude = 85.05112878;
        private const double MinLongitude = -180;
        private const double MaxLongitude = 180;
        #endregion

        #region Properties.override
        protected override int TileValue => 512;

        public override StiGisRectLatLng Bounds => StiGisRectLatLng.FromLTRB(MinLongitude, MaxLatitude, MaxLongitude, MinLatitude);

        public override double Axis => 6378137;

        public override double Flattening => (1.0 / 298.257223563);
        #endregion

        #region Methods.override
        public override StiGisPoint FromLatLngToPixel(double lat, double lng, int zoom)
        {
            lat = Clip(lat, MinLatitude, MaxLatitude);
            lng = Clip(lng, MinLongitude, MaxLongitude);

            var s = GetTileMatrixSizePixel(zoom);
            double scale = 360.0 / s.Width;

            var ret = StiGisPoint.Empty;
            ret.Y = (int)((90.0 - lat) / scale);
            ret.X = (int)((lng + 180.0) / scale);

            return ret;
        }

        public override StiGisPointLatLng FromPixelToLatLng(int x, int y, int zoom)
        {
            var s = GetTileMatrixSizePixel(zoom);

            double scale = 360.0 / s.Width;

            var ret = StiGisPointLatLng.Empty;
            ret.Lat = 90 - (y * scale);
            ret.Lng = (x * scale) - 180;

            return ret;
        }

        public override StiGisSize GetTileMatrixMaxXY(int zoom)
        {
            int y = (int)Math.Pow(2, zoom);
            return new StiGisSize((2 * y) - 1, y - 1);
        }

        public override StiGisSize GetTileMatrixMinXY(int zoom)
        {
            return new StiGisSize(0, 0);
        }
        #endregion
    }
}
