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
    internal sealed class StiMercatorYandexGisProjection : 
        StiGisProjection
    {
        #region Fields
        public static readonly StiMercatorYandexGisProjection Instance = new StiMercatorYandexGisProjection();
        #endregion

        #region consts
        private const double MinLatitude = -85.05112878;
        private const double MaxLatitude = 85.05112878;
        private const double MinLongitude = -177;
        private const double MaxLongitude = 177;
        private const double RAD_DEG = 180 / Math.PI;
        //private const double DEG_RAD = Math.PI / 180;
        //private const double MathPiDiv4 = Math.PI / 4;
        #endregion

        #region Properties.override
        public override StiGisRectLatLng Bounds => StiGisRectLatLng.FromLTRB(MinLongitude, MaxLatitude, MaxLongitude, MinLatitude);

        public override double Axis => 6356752.3142;

        public override double Flattening => (1.0 / 298.257223563);
        #endregion

        #region Methods.override
        public override StiGisPoint FromLatLngToPixel(double lat, double lng, int zoom)
        {
            lat = Clip(lat, MinLatitude, MaxLatitude);
            lng = Clip(lng, MinLongitude, MaxLongitude);

            double e = 0.0818191908426;
            var rho = Math.Pow(2, zoom + 8) / 2;
            var beta = lat * Math.PI / 180;
            var phi = (1 - e * Math.Sin(beta)) / (1 + e * Math.Sin(beta));
            var theta = Math.Tan(Math.PI / 4 + beta / 2) * Math.Pow(phi, e / 2);

            var x_p = rho * (1 + lng / 180);
            var y_p = rho * (1 - Math.Log(theta) / Math.PI);

            var ret = StiGisPoint.Empty;
            ret.X = (int)x_p;
            ret.Y = (int)y_p;

            return ret;
        }

        public override StiGisPointLatLng FromPixelToLatLng(int x, int y, int zoom)
        {
            double a = 6378137;
            double c1 = 0.00335655146887969;
            double c2 = 0.00000657187271079536;
            double c3 = 0.00000001764564338702;
            double c4 = 0.00000000005328478445;
            double z1 = (23 - zoom);
            double mercX = (x * Math.Pow(2, z1)) / 53.5865938 - 20037508.342789;
            double mercY = 20037508.342789 - (y * Math.Pow(2, z1)) / 53.5865938;

            double g = Math.PI / 2 - 2 * Math.Atan(1 / Math.Exp(mercY / a));
            double z = g + c1 * Math.Sin(2 * g) + c2 * Math.Sin(4 * g) + c3 * Math.Sin(6 * g) + c4 * Math.Sin(8 * g);

            var ret = StiGisPointLatLng.Empty;
            ret.Lat = z * RAD_DEG;
            ret.Lng = mercX / a * RAD_DEG;

            return ret;
        }

        public override StiGisSize GetTileMatrixMinXY(int zoom)
        {
            return new StiGisSize(0, 0);
        }

        public override StiGisSize GetTileMatrixMaxXY(int zoom)
        {
            int xy = (1 << zoom);
            return new StiGisSize(xy - 1, xy - 1);
        }
        #endregion
    }
}