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

#if NETSTANDARD
using Stimulsoft.System.Windows;
using Stimulsoft.System.Windows.Media;
#else
using System.Windows;
using System.Windows.Media;
#endif

namespace Stimulsoft.Map.Gis.Geography.Helpers
{
    internal static class StiGisCurveHelper
    {
        public static PointCollection CardinalSpline(Point[] pts, bool closed)
        {
            int i, nrRetPts;
            Point p1, p2;
            double tension = 0.5 * (1d / 3d);

            if (closed)
                nrRetPts = (pts.Length + 1) * 3 - 2;
            else
                nrRetPts = pts.Length * 3 - 2;

            var retPnt = new Point[nrRetPts];
            for (i = 0; i < nrRetPts; i++)
                retPnt[i] = new Point();

            if (!closed)
            {
                CalcCurveEnd(pts[0], pts[1], tension, out p1);
                retPnt[0] = pts[0];
                retPnt[1] = p1;
            }
            for (i = 0; i < pts.Length - (closed ? 1 : 2); i++)
            {
                CalcCurve(new Point[] { pts[i], pts[i + 1], pts[(i + 2) % pts.Length] }, tension, out p1, out p2);
                retPnt[3 * i + 2] = p1;
                retPnt[3 * i + 3] = pts[i + 1];
                retPnt[3 * i + 4] = p2;
            }
            if (closed)
            {
                CalcCurve(new Point[] { pts[pts.Length - 1], pts[0], pts[1] }, tension, out p1, out p2);
                retPnt[nrRetPts - 2] = p1;
                retPnt[0] = pts[0];
                retPnt[1] = p2;
                retPnt[nrRetPts - 1] = retPnt[0];
            }
            else
            {
                CalcCurveEnd(pts[pts.Length - 1], pts[pts.Length - 2], tension, out p1);
                retPnt[nrRetPts - 2] = p1;
                retPnt[nrRetPts - 1] = pts[pts.Length - 1];
            }
            return new PointCollection(retPnt);
        }

        public static PointCollection CardinalSpline(global::System.Drawing.PointF[] pts, bool closed)
        {
            int i, nrRetPts;
            Point p1, p2;
            double tension = 0.5 * (1d / 3d);

            if (closed)
                nrRetPts = (pts.Length + 1) * 3 - 2;
            else
                nrRetPts = pts.Length * 3 - 2;

            var retPnt = new Point[nrRetPts];
            for (i = 0; i < nrRetPts; i++)
                retPnt[i] = new Point();

            if (!closed)
            {
                Point p0 = new Point(pts[0].X, pts[0].Y);
                CalcCurveEnd(p0, new Point(pts[1].X, pts[1].Y), tension, out p1);
                retPnt[0] = p0;
                retPnt[1] = p1;
            }
            for (i = 0; i < pts.Length - (closed ? 1 : 2); i++)
            {
                var p0 = new Point(pts[i + 1].X, pts[i + 1].Y);
                CalcCurve(new Point[] { new Point(pts[i].X, pts[i].Y) , p0,
                    new Point(pts[(i + 2) % pts.Length].X, pts[(i + 2) % pts.Length].Y) }, tension, out p1, out p2);
                retPnt[3 * i + 2] = p1;
                retPnt[3 * i + 3] = p0;
                retPnt[3 * i + 4] = p2;
            }
            if (closed)
            {
                var p0 = new Point(pts[0].X, pts[0].Y);
                CalcCurve(new Point[] { new Point(pts[pts.Length - 1].X, pts[pts.Length - 1].Y),
                    p0, new Point(pts[1].X, pts[1].Y) }, tension, out p1, out p2);
                retPnt[nrRetPts - 2] = p1;
                retPnt[0] = p0;
                retPnt[1] = p2;
                retPnt[nrRetPts - 1] = retPnt[0];
            }
            else
            {
                var p0 = new Point(pts[pts.Length - 1].X, pts[pts.Length - 1].Y);
                CalcCurveEnd(p0, new Point(pts[pts.Length - 2].X, pts[pts.Length - 2].Y), tension, out p1);
                retPnt[nrRetPts - 2] = p1;
                retPnt[nrRetPts - 1] = p0;
            }
            return new PointCollection(retPnt);
        }


        private static void CalcCurveEnd(Point end, Point adj, double tension, out Point p1)
        {
            p1 = new Point(((tension * (adj.X - end.X) + end.X)), ((tension * (adj.Y - end.Y) + end.Y)));
        }

        private static void CalcCurve(Point[] pts, double tenstion, out Point p1, out Point p2)
        {
            double deltaX, deltaY;
            deltaX = pts[2].X - pts[0].X;
            deltaY = pts[2].Y - pts[0].Y;
            p1 = new Point((pts[1].X - tenstion * deltaX), (pts[1].Y - tenstion * deltaY));
            p2 = new Point((pts[1].X + tenstion * deltaX), (pts[1].Y + tenstion * deltaY));
        }
    }
}