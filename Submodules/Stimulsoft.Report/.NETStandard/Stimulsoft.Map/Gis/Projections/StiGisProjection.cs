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

using Stimulsoft.Base;
using Stimulsoft.Map.Gis.Core;
using System;
using System.Collections.Generic;

namespace Stimulsoft.Map.Gis.Projections
{
    internal abstract class StiGisProjection
    {
        protected StiGisProjection()
        {
            this.TileSize = StiScale.XXI(TileValue);
            for (int i = 0; i < FromLatLngToPixelCache.Capacity; i++)
            {
                FromLatLngToPixelCache.Add(new Dictionary<StiGisPointLatLng, StiGisPoint>());
                FromPixelToLatLngCache.Add(new Dictionary<StiGisPoint, StiGisPointLatLng>());
            }
        }

        #region consts
        protected const double HALF_PI = (Math.PI * 0.5);
        protected const double TWO_PI = (Math.PI * 2.0);
        protected const double EPSLoN = 1.0e-10;
        protected const double MAX_VAL = 4;
        protected const double MAXLONG = 2147483647;
        protected const double DBLLONG = 4.61168601e18;
        protected const double R2D = 180 / Math.PI;
        protected const double D2R = Math.PI / 180;
        #endregion

        #region Fields
        private readonly List<Dictionary<StiGisPointLatLng, StiGisPoint>> FromLatLngToPixelCache = new List<Dictionary<StiGisPointLatLng, StiGisPoint>>(33);
        private readonly List<Dictionary<StiGisPoint, StiGisPointLatLng>> FromPixelToLatLngCache = new List<Dictionary<StiGisPoint, StiGisPointLatLng>>(33);
        #endregion

        #region Properties.abstract
        protected virtual int TileValue => 256;

        public int TileSize { get; }

        public abstract double Axis { get; }

        public abstract double Flattening { get; }
        #endregion

        #region Properties.virtual
        public virtual StiGisRectLatLng Bounds => StiGisRectLatLng.FromLTRB(-180, 90, 180, -90);
        #endregion

        #region Methods.abstract
        public abstract StiGisPoint FromLatLngToPixel(double lat, double lng, int zoom);

        public abstract StiGisPointLatLng FromPixelToLatLng(int x, int y, int zoom);
        #endregion

        #region Methods
        public StiGisPoint FromLatLngToPixel(StiGisPointLatLng p, int zoom)
        {
            return FromLatLngToPixel(p.Lat, p.Lng, zoom);
        }

        public StiGisPointLatLng FromPixelToLatLng(StiGisPoint p, int zoom)
        {
            return FromPixelToLatLng(p.X, p.Y, zoom);
        }

        public int GetTileMatrixItemCount(int zoom)
        {
            var s = GetTileMatrixSizeXY(zoom);
            return (s.Width * s.Height);
        }

        public List<StiGisPoint> GetAreaTileList(StiGisRectLatLng rect, int zoom, int padding)
        {
            var ret = new List<StiGisPoint>();

            var topLeft = FromPixelToTileXY(FromLatLngToPixel(rect.LocationTopLeft, zoom));
            var rightBottom = FromPixelToTileXY(FromLatLngToPixel(rect.LocationRightBottom, zoom));

            for (int x = (topLeft.X - padding); x <= (rightBottom.X + padding); x++)
            {
                for (int y = (topLeft.Y - padding); y <= (rightBottom.Y + padding); y++)
                {
                    var p = new StiGisPoint(x, y);
                    if (!ret.Contains(p) && p.X >= 0 && p.Y >= 0)
                    {
                        ret.Add(p);
                    }
                }
            }

            return ret;
        }
        #endregion

        #region Methods.virtual
        public virtual StiGisPoint FromPixelToTileXY(StiGisPoint p)
        {
            return new StiGisPoint(p.X / TileSize, p.Y / TileSize);
        }

        public virtual StiGisPoint FromTileXYToPixel(StiGisPoint p)
        {
            return new StiGisPoint(p.X * TileSize, p.Y * TileSize);
        }

        public virtual StiGisSize GetTileMatrixSizeXY(int zoom)
        {
            var sMin = GetTileMatrixMinXY(zoom);
            var sMax = GetTileMatrixMaxXY(zoom);

            return new StiGisSize(sMax.Width - sMin.Width + 1, sMax.Height - sMin.Height + 1);
        }

        public virtual StiGisSize GetTileMatrixSizePixel(int zoom)
        {
            var s = GetTileMatrixSizeXY(zoom);
            return new StiGisSize(s.Width * TileSize, s.Height * TileSize);
        }

        public virtual double GetGroundResolution(int zoom, double latitude)
        {
            return (Math.Cos(latitude * (Math.PI / 180)) * 2 * Math.PI * Axis) / GetTileMatrixSizePixel(zoom).Width;
        }
        #endregion

        #region Methods.abstract
        public abstract StiGisSize GetTileMatrixMinXY(int zoom);

        public abstract StiGisSize GetTileMatrixMaxXY(int zoom);
        #endregion

        #region Methods.Math
        public static double DegreesToRadians(double deg) => D2R * deg;

        public static double RadiansToDegrees(double rad) => R2D * rad;

        protected static double Sign(double x) => (x < 0.0) ? -1 : 1;

        protected static double AdjustLongitude(double x)
        {
            int count = 0;
            while (true)
            {
                if (Math.Abs(x) <= Math.PI)
                {
                    break;
                }
                else if (((int)Math.Abs(x / Math.PI)) < 2)
                {
                    x = x - (Sign(x) * TWO_PI);
                }
                else if (((int)Math.Abs(x / TWO_PI)) < MAXLONG)
                {
                    x = x - (((int)(x / TWO_PI)) * TWO_PI);
                }
                else if (((int)Math.Abs(x / (MAXLONG * TWO_PI))) < MAXLONG)
                {
                    x = x - (((int)(x / (MAXLONG * TWO_PI))) * (TWO_PI * MAXLONG));
                }
                else if (((int)Math.Abs(x / (DBLLONG * TWO_PI))) < MAXLONG)
                {
                    x = x - (((int)(x / (DBLLONG * TWO_PI))) * (TWO_PI * DBLLONG));
                }
                else
                {
                    x = x - (Sign(x) * TWO_PI);
                }

                count++;
                if (count > MAX_VAL)
                    break;
            }

            return x;
        }

        protected static void SinCos(double val, out double sin, out double cos)
        {
            sin = Math.Sin(val);
            cos = Math.Cos(val);
        }

        protected static double E0fn(double x)
        {
            return (1.0 - 0.25 * x * (1.0 + x / 16.0 * (3.0 + 1.25 * x)));
        }

        protected static double E1fn(double x)
        {
            return (0.375 * x * (1.0 + 0.25 * x * (1.0 + 0.46875 * x)));
        }

        protected static double E2fn(double x)
        {
            return (0.05859375 * x * x * (1.0 + 0.75 * x));
        }

        protected static double E3fn(double x)
        {
            return (x * x * x * (35.0 / 3072.0));
        }

        protected static double Mlfn(double e0, double e1, double e2, double e3, double phi)
        {
            return (e0 * phi - e1 * Math.Sin(2.0 * phi) + e2 * Math.Sin(4.0 * phi) - e3 * Math.Sin(6.0 * phi));
        }

        protected static int GetUTMzone(double lon)
        {
            return ((int)(((lon + 180.0) / 6.0) + 1.0));
        }

        protected static double Clip(double n, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(n, minValue), maxValue);
        }

        public double GetDistance(StiGisPointLatLng p1, StiGisPointLatLng p2)
        {
            double dLat1InRad = p1.Lat * (Math.PI / 180);
            double dLong1InRad = p1.Lng * (Math.PI / 180);
            double dLat2InRad = p2.Lat * (Math.PI / 180);
            double dLong2InRad = p2.Lng * (Math.PI / 180);
            double dLongitude = dLong2InRad - dLong1InRad;
            double dLatitude = dLat2InRad - dLat1InRad;
            double a = Math.Pow(Math.Sin(dLatitude / 2), 2) + Math.Cos(dLat1InRad) * Math.Cos(dLat2InRad) * Math.Pow(Math.Sin(dLongitude / 2), 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double dDistance = (Axis / 1000.0) * c;
            return dDistance;
        }

        public double GetDistanceInPixels(StiGisPoint point1, StiGisPoint point2)
        {
            double a = (double)(point2.X - point1.X);
            double b = (double)(point2.Y - point1.Y);

            return Math.Sqrt(a * a + b * b);
        }

        public double GetBearing(StiGisPointLatLng p1, StiGisPointLatLng p2)
        {
            var latitude1 = DegreesToRadians(p1.Lat);
            var latitude2 = DegreesToRadians(p2.Lat);
            var longitudeDifference = DegreesToRadians(p2.Lng - p1.Lng);

            var y = Math.Sin(longitudeDifference) * Math.Cos(latitude2);
            var x = Math.Cos(latitude1) * Math.Sin(latitude2) - Math.Sin(latitude1) * Math.Cos(latitude2) * Math.Cos(longitudeDifference);

            return (RadiansToDegrees(Math.Atan2(y, x)) + 360) % 360;
        }

        public void FromGeodeticToCartesian(double Lat, double Lng, double Height, out double X, out double Y, out double Z)
        {
            Lat = (Math.PI / 180) * Lat;
            Lng = (Math.PI / 180) * Lng;

            double B = Axis * (1.0 - Flattening);
            double ee = 1.0 - (B / Axis) * (B / Axis);
            double N = (Axis / Math.Sqrt(1.0 - ee * Math.Sin(Lat) * Math.Sin(Lat)));

            X = (N + Height) * Math.Cos(Lat) * Math.Cos(Lng);
            Y = (N + Height) * Math.Cos(Lat) * Math.Sin(Lng);
            Z = (N * (B / Axis) * (B / Axis) + Height) * Math.Sin(Lat);
        }

        public void FromCartesianTGeodetic(double X, double Y, double Z, out double Lat, out double Lng)
        {
            double E = Flattening * (2.0 - Flattening);
            Lng = Math.Atan2(Y, X);

            double P = Math.Sqrt(X * X + Y * Y);
            double Theta = Math.Atan2(Z, (P * (1.0 - Flattening)));
            double st = Math.Sin(Theta);
            double ct = Math.Cos(Theta);
            Lat = Math.Atan2(Z + E / (1.0 - Flattening) * Axis * st * st * st, P - E * Axis * ct * ct * ct);

            Lat /= (Math.PI / 180);
            Lng /= (Math.PI / 180);
        }
        #endregion
    }
}