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
using System.Collections.Generic;

namespace Stimulsoft.Map.Gis.Projections
{
    internal sealed class StiLKS92GisProjection : 
        StiGisProjection
    {
        #region Fields
        public static readonly StiLKS92GisProjection Instance = new StiLKS92GisProjection();
        private Dictionary<int, StiGisSize> extentMatrixMin;
        private Dictionary<int, StiGisSize> extentMatrixMax;
        #endregion

        #region consts
        private const double MinLatitude = 55.55;
        private const double MaxLatitude = 58.22;
        private const double MinLongitude = 20.22;
        private const double MaxLongitude = 28.28;
        private const double orignX = -5120900;
        private const double orignY = 3998100;
        private const double scaleFactor = 0.9996;                    // scale factor				
        private const double centralMeridian = 0.41887902047863912;// Center longitude (projection center) 
        private const double latOrigin = 0.0;                    // center latitude			
        private const double falseNorthing = -6000000.0;            // y offset in meters			
        private const double falseEasting = 500000.0;        // x offset in meters			
        private const double semiMajor = 6378137.0;           // major axis
        private const double semiMinor = 6356752.3141403561; // minor axis
        private const double semiMinor2 = 6356752.3142451793;     // minor axis
        private const double metersPerUnit = 1.0;
        private const double COS_67P5 = 0.38268343236508977; // cosine of 67.5 degrees
        private const double AD_C = 1.0026000;               // Toms region 1 constant
        #endregion

        #region Properties.override
        public override StiGisRectLatLng Bounds => StiGisRectLatLng.FromLTRB(MinLongitude, MaxLatitude, MaxLongitude, MinLatitude);

        public override double Axis => 6378137;

        public override double Flattening => (1.0 / 298.257222101);
        #endregion

        #region Methods.override
        public override StiGisPoint FromLatLngToPixel(double lat, double lng, int zoom)
        {
            lat = Clip(lat, MinLatitude, MaxLatitude);
            lng = Clip(lng, MinLongitude, MaxLongitude);

            var lks = new double[] { lng, lat };
            lks = DTM10(lks);
            lks = MTD10(lks);
            lks = DTM00(lks);

            double res = GetTileMatrixResolution(zoom);
            return LksToPixel(lks, res);
        }

        public override StiGisPointLatLng FromPixelToLatLng(int x, int y, int zoom)
        {
            double res = GetTileMatrixResolution(zoom);

            var lks = new double[] { (x * res) + orignX, orignY - (y * res) };
            lks = MTD11(lks);
            lks = DTM10(lks);
            lks = MTD10(lks);

            var ret = StiGisPointLatLng.Empty;
            ret.Lat = Clip(lks[1], MinLatitude, MaxLatitude);
            ret.Lng = Clip(lks[0], MinLongitude, MaxLongitude);

            return ret;
        }

        public override double GetGroundResolution(int zoom, double latitude)
        {
            return GetTileMatrixResolution(zoom);
        }

        public override StiGisSize GetTileMatrixMinXY(int zoom)
        {
            if (extentMatrixMin == null)
                GenerateExtents();

            return extentMatrixMin[zoom];
        }

        public override StiGisSize GetTileMatrixMaxXY(int zoom)
        {
            if (extentMatrixMax == null)
                GenerateExtents();

            return extentMatrixMax[zoom];
        }
        #endregion

        #region Methods
        private static StiGisPoint LksToPixel(double[] lks, double res)
        {
            return new StiGisPoint((int)Math.Floor((lks[0] - orignX) / res), (int)Math.Floor((orignY - lks[1]) / res));
        }

        private double[] DTM10(double[] lonlat)
        {
            // Eccentricity squared : (a^2 - b^2)/a^2
            double es = 1.0 - (semiMinor2 * semiMinor2) / (semiMajor * semiMajor); // e^2

            double lon = DegreesToRadians(lonlat[0]);
            double lat = DegreesToRadians(lonlat[1]);
            double h = lonlat.Length < 3 ? 0 : lonlat[2].Equals(double.NaN) ? 0 : lonlat[2];
            double v = semiMajor / Math.Sqrt(1 - es * Math.Pow(Math.Sin(lat), 2));
            double x = (v + h) * Math.Cos(lat) * Math.Cos(lon);
            double y = (v + h) * Math.Cos(lat) * Math.Sin(lon);
            double z = ((1 - es) * v + h) * Math.Sin(lat);

            return new double[] { x, y, z, };
        }

        private double[] MTD10(double[] pnt)
        {
            // Eccentricity squared : (a^2 - b^2)/a^2
            double es = 1.0 - (semiMinor * semiMinor) / (semiMajor * semiMajor); // e^2

            // Second eccentricity squared : (a^2 - b^2)/b^2
            double ses = (Math.Pow(semiMajor, 2) - Math.Pow(semiMinor, 2)) / Math.Pow(semiMinor, 2);

            bool AtPole = false; // is location in polar region
            double Z = pnt.Length < 3 ? 0 : pnt[2].Equals(double.NaN) ? 0 : pnt[2];

            double lon;
            double lat = 0;
            double height;
            if (pnt[0] != 0.0)
            {
                lon = Math.Atan2(pnt[1], pnt[0]);
            }
            else
            {
                if (pnt[1] > 0)
                {
                    lon = Math.PI / 2;
                }
                else if (pnt[1] < 0)
                {
                    lon = -Math.PI * 0.5;
                }
                else
                {
                    AtPole = true;
                    lon = 0.0;
                    if (Z > 0.0) // north pole
                    {
                        lat = Math.PI * 0.5;
                    }
                    else if (Z < 0.0) // south pole
                    {
                        lat = -Math.PI * 0.5;
                    }
                    else // center of earth
                    {
                        return new double[] { RadiansToDegrees(lon), RadiansToDegrees(Math.PI * 0.5), -semiMinor, };
                    }
                }
            }

            double w2 = pnt[0] * pnt[0] + pnt[1] * pnt[1]; // Square of distance from Z axis
            double w = Math.Sqrt(w2); // distance from Z axis
            double t0 = Z * AD_C; // initial estimate of vertical component
            double s0 = Math.Sqrt(t0 * t0 + w2); // initial estimate of horizontal component
            double sin_B0 = t0 / s0; // sin(B0), B0 is estimate of Bowring aux variable
            double cos_B0 = w / s0; // cos(B0)
            double sin3_B0 = Math.Pow(sin_B0, 3);
            double t1 = Z + semiMinor * ses * sin3_B0; // corrected estimate of vertical component
            double sum = w - semiMajor * es * cos_B0 * cos_B0 * cos_B0; // numerator of cos(phi1)
            double s1 = Math.Sqrt(t1 * t1 + sum * sum); // corrected estimate of horizontal component
            double sin_p1 = t1 / s1; // sin(phi1), phi1 is estimated latitude
            double cos_p1 = sum / s1; // cos(phi1)
            double rn = semiMajor / Math.Sqrt(1.0 - es * sin_p1 * sin_p1); // Earth radius at location

            if (cos_p1 >= COS_67P5)
            {
                height = w / cos_p1 - rn;
            }
            else if (cos_p1 <= -COS_67P5)
            {
                height = w / -cos_p1 - rn;
            }
            else
            {
                height = Z / sin_p1 + rn * (es - 1.0);
            }

            if (!AtPole)
            {
                lat = Math.Atan(sin_p1 / cos_p1);
            }

            return new double[] { RadiansToDegrees(lon), RadiansToDegrees(lat), height, };
        }

        private double[] DTM00(double[] lonlat)
        {
            double e0, e1, e2, e3;  // eccentricity constants		
            double es, esp;      // eccentricity constants		
            double ml0;              // small value m			

            es = 1.0 - Math.Pow(semiMinor / semiMajor, 2);
            e0 = E0fn(es);
            e1 = E1fn(es);
            e2 = E2fn(es);
            e3 = E3fn(es);
            ml0 = semiMajor * Mlfn(e0, e1, e2, e3, latOrigin);
            esp = es / (1.0 - es);

            double lon = DegreesToRadians(lonlat[0]);
            double lat = DegreesToRadians(lonlat[1]);

            double sin_phi, cos_phi; // sin and cos value				
            double al, als;         // temporary values				
            double c, t, tq;           // temporary values				
            double con, n, ml;      // cone constant, small m			

            // Delta longitude (Given longitude - center)
            var delta_lon = AdjustLongitude(lon - centralMeridian);
            SinCos(lat, out sin_phi, out cos_phi);

            al = cos_phi * delta_lon;
            als = Math.Pow(al, 2);
            c = esp * Math.Pow(cos_phi, 2);
            tq = Math.Tan(lat);
            t = Math.Pow(tq, 2);
            con = 1.0 - es * Math.Pow(sin_phi, 2);
            n = semiMajor / Math.Sqrt(con);
            ml = semiMajor * Mlfn(e0, e1, e2, e3, lat);

            double x = scaleFactor * n * al * (1.0 + als / 6.0 * (1.0 - t + c + als / 20.0 *
                (5.0 - 18.0 * t + Math.Pow(t, 2) + 72.0 * c - 58.0 * esp))) + falseEasting;

            double y = scaleFactor * (ml - ml0 + n * tq * (als * (0.5 + als / 24.0 *
                (5.0 - t + 9.0 * c + 4.0 * Math.Pow(c, 2) + als / 30.0 * (61.0 - 58.0 * t
                + Math.Pow(t, 2) + 600.0 * c - 330.0 * esp))))) + falseNorthing;

            return (lonlat.Length < 3)
                ? new double[] { x / metersPerUnit, y / metersPerUnit }
                : new double[] { x / metersPerUnit, y / metersPerUnit, lonlat[2] };
        }

        private double[] MTD11(double[] p)
        {
            double e0, e1, e2, e3;  // eccentricity constants		
            double es, esp;      // eccentricity constants		
            double ml0;         // small value m

            es = 1.0 - Math.Pow(semiMinor / semiMajor, 2);
            e0 = E0fn(es);
            e1 = E1fn(es);
            e2 = E2fn(es);
            e3 = E3fn(es);
            ml0 = semiMajor * Mlfn(e0, e1, e2, e3, latOrigin);
            esp = es / (1.0 - es);

            double con, phi;
            double delta_phi;
            int i;
            double sin_phi, cos_phi, tan_phi;
            double c, cs, t, ts, n, r, d, ds;
            int max_iter = 6;

            double x = p[0] * metersPerUnit - falseEasting;
            double y = p[1] * metersPerUnit - falseNorthing;

            con = (ml0 + y / scaleFactor) / semiMajor;
            phi = con;
            for (i = 0; ; i++)
            {
                delta_phi = ((con + e1 * Math.Sin(2.0 * phi) - e2 * Math.Sin(4.0 * phi) + e3 * Math.Sin(6.0 * phi)) / e0) - phi;
                phi += delta_phi;

                if (Math.Abs(delta_phi) <= EPSLoN)
                    break;

                if (i >= max_iter)
                    throw new ArgumentException("Latitude failed to converge");
            }

            if (Math.Abs(phi) < HALF_PI)
            {
                SinCos(phi, out sin_phi, out cos_phi);
                tan_phi = Math.Tan(phi);
                c = esp * Math.Pow(cos_phi, 2);
                cs = Math.Pow(c, 2);
                t = Math.Pow(tan_phi, 2);
                ts = Math.Pow(t, 2);
                con = 1.0 - es * Math.Pow(sin_phi, 2);
                n = semiMajor / Math.Sqrt(con);
                r = n * (1.0 - es) / con;
                d = x / (n * scaleFactor);
                ds = Math.Pow(d, 2);

                double lat = phi - (n * tan_phi * ds / r) * (0.5 - ds / 24.0 * (5.0 + 3.0 * t +
                    10.0 * c - 4.0 * cs - 9.0 * esp - ds / 30.0 * (61.0 + 90.0 * t +
                    298.0 * c + 45.0 * ts - 252.0 * esp - 3.0 * cs)));

                double lon = AdjustLongitude(centralMeridian + (d * (1.0 - ds / 6.0 * (1.0 + 2.0 * t +
                    c - ds / 20.0 * (5.0 - 2.0 * c + 28.0 * t - 3.0 * cs + 8.0 * esp +
                    24.0 * ts))) / cos_phi));

                return (p.Length < 3)
                    ? new double[] { RadiansToDegrees(lon), RadiansToDegrees(lat) }
                    : new double[] { RadiansToDegrees(lon), RadiansToDegrees(lat), p[2] };
            }
            else
            {
                return (p.Length < 3)
                    ? new double[] { RadiansToDegrees(HALF_PI * Sign(y)), RadiansToDegrees(centralMeridian) }
                    : new double[] { RadiansToDegrees(HALF_PI * Sign(y)), RadiansToDegrees(centralMeridian), p[2] };
            }
        }

        public static double GetTileMatrixResolution(int zoom)
        {
            switch (zoom)
            {
                case 0: return 1587.5031750063501;
                case 1: return 793.7515875031751;
                case 2: return 529.1677250021168;
                case 3: return 264.5838625010584;
                case 4: return 132.2919312505292;
                case 5: return 52.91677250021167;
                case 6: return 26.458386250105836;
                case 7: return 13.229193125052918;
                case 8: return 6.614596562526459;
                case 9: return 2.6458386250105836;
                case 10: return 1.3229193125052918;
                case 11: return 0.5291677250021167;
            }

            return 0;
        }

        private void GenerateExtents()
        {
            extentMatrixMin = new Dictionary<int, StiGisSize>();
            extentMatrixMax = new Dictionary<int, StiGisSize>();

            for (int i = 0; i <= 11; i++)
            {
                extentMatrixMin.Add(i, new StiGisSize(FromPixelToTileXY(FromLatLngToPixel(Bounds.LocationTopLeft, i))));
                extentMatrixMax.Add(i, new StiGisSize(FromPixelToTileXY(FromLatLngToPixel(Bounds.LocationRightBottom, i))));
            }
        }
        #endregion
    }
}