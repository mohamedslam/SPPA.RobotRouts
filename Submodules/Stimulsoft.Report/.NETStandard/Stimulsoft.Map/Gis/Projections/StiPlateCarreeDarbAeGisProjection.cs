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
    internal sealed class StiPlateCarreeDarbAeGisProjection : 
        StiGisProjection
    {
        #region Fields
        public static readonly StiPlateCarreeDarbAeGisProjection Instance = new StiPlateCarreeDarbAeGisProjection();
        #endregion

        #region consts
        private const double MinLatitude = 18.7071563263201;
        private const double MaxLatitude = 29.4052130085331;
        private const double MinLongitude = 41.522866508209;
        private const double MaxLongitude = 66.2882966568906;
        private const double orignX = -400;
        private const double orignY = 400;
        #endregion

        #region Properties.override
        public override StiGisRectLatLng Bounds => StiGisRectLatLng.FromLTRB(MinLongitude, MaxLatitude, MaxLongitude, MinLatitude);

        public override double Axis => 6378137;

        public override double Flattening => (1.0 / 298.257223563);
        #endregion

        #region Methods.override
        public override StiGisPoint FromLatLngToPixel(double lat, double lng, int zoom)
        {
            lat = Clip(lat, MinLatitude, MaxLatitude);
            lng = Clip(lng, MinLongitude, MaxLongitude);

            double res = GetTileMatrixResolution(zoom);

            var ret = StiGisPoint.Empty;
            ret.X = (int)Math.Floor((lng - orignX) / res);
            ret.Y = (int)Math.Floor((orignY - lat) / res);

            return ret;
        }

        public override StiGisPointLatLng FromPixelToLatLng(int x, int y, int zoom)
        {
            double res = GetTileMatrixResolution(zoom);

            var ret = StiGisPointLatLng.Empty;
            ret.Lat = orignY - (y * res);
            ret.Lng = (x * res) + orignX;

            return ret;
        }

        public override double GetGroundResolution(int zoom, double latitude)
        {
            return GetTileMatrixResolution(zoom);
        }

        public override StiGisSize GetTileMatrixMaxXY(int zoom)
        {
            var maxPx = FromLatLngToPixel(MinLatitude, MaxLongitude, zoom);
            return new StiGisSize(FromPixelToTileXY(maxPx));
        }

        public override StiGisSize GetTileMatrixMinXY(int zoom)
        {
            var minPx = FromLatLngToPixel(MaxLatitude, MinLongitude, zoom);
            return new StiGisSize(FromPixelToTileXY(minPx));
        }
        #endregion

        #region Methods
        public static double GetTileMatrixResolution(int zoom)
        {
            switch (zoom)
            {
                case 0: return 0.0118973050291514;
                case 1: return 0.0059486525145757;
                case 2: return 0.00297432625728785;
                case 3: return 0.00118973050291514;
                case 4: return 0.00059486525145757;
                case 5: return 0.000356919150874542;
                case 6: return 0.000178459575437271;
                case 7: return 0.000118973050291514;
                case 8: return 5.9486525145757E-05;
                case 9: return 3.56919150874542E-05;
                case 10: return 1.90356880466422E-05;
                case 11: return 9.51784402332112E-06;
                case 12: return 4.75892201166056E-06;
            }

            return 0;
        }
        #endregion
    }
}