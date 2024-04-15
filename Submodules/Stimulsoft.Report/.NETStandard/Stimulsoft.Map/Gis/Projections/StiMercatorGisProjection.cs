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
    internal sealed class StiMercatorGisProjection : 
        StiGisProjection
    {
        #region Fields
        public static readonly StiMercatorGisProjection Instance = new StiMercatorGisProjection();
        #endregion

        #region consts
        private const double MinLatitude = -85.05112878;
        private const double MaxLatitude = 85.05112878;
        private const double MinLongitude = -180;
        private const double MaxLongitude = 180;
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

            double x = (lng + 180) / 360;
            double sinLatitude = Math.Sin(lat * Math.PI / 180);
            double y = 0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI);

            var s = GetTileMatrixSizePixel(zoom);
            int mapSizeX = s.Width;
            int mapSizeY = s.Height;

            var ret = StiGisPoint.Empty;
            ret.X = (int)Clip(x * mapSizeX + 0.5, 0, mapSizeX - 1);
            ret.Y = (int)Clip(y * mapSizeY + 0.5, 0, mapSizeY - 1);

            return ret;
        }

        public override StiGisPointLatLng FromPixelToLatLng(int x, int y, int zoom)
        {
            var s = GetTileMatrixSizePixel(zoom);
            double mapSizeX = s.Width;
            double mapSizeY = s.Height;

            double xx = (Clip(x, 0, mapSizeX - 1) / mapSizeX) - 0.5;
            double yy = 0.5 - (Clip(y, 0, mapSizeY - 1) / mapSizeY);

            var ret = StiGisPointLatLng.Empty;
            ret.Lat = 90 - 360 * Math.Atan(Math.Exp(-yy * 2 * Math.PI)) / Math.PI;
            ret.Lng = 360 * xx;

            return ret;
        }

        public override StiGisSize GetTileMatrixMinXY(int zoom) => new StiGisSize(0, 0);

        public override StiGisSize GetTileMatrixMaxXY(int zoom)
        {
            int xy = (1 << zoom);
            return new StiGisSize(xy - 1, xy - 1);
        }
        #endregion
    }
}
