#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using System.Collections.Generic;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    internal static class StiJarvisMarchAlgorithm
    {
        #region Fields
        private static LinkedList<PointF> points;
        private static LinkedList<PointF> convexHull;
        #endregion

        #region Methods
        public static LinkedList<PointF> Calculate(LinkedList<PointF> points)
        {
            if (points.Count < 3)
                return null;

            StiJarvisMarchAlgorithm.points = new LinkedList<PointF>(points);
            convexHull = new LinkedList<PointF>();

            var s0 = GetLowerLeftPoint();
            convexHull.AddLast(s0);
            StiJarvisMarchAlgorithm.points.Remove(s0);
            StiJarvisMarchAlgorithm.points.AddLast(s0);

            while (true)
            {
                var s1 = StiJarvisMarchAlgorithm.points.First.Value;
                foreach (var p in StiJarvisMarchAlgorithm.points)
                    if (IsLeftDirection(s0, s1, p))
                        s1 = p;
                if (s1 == convexHull.First.Value)
                    break;

                convexHull.AddLast(s1);
                StiJarvisMarchAlgorithm.points.Remove(s1);
                s0 = s1;
            }

            return convexHull;
        }

        private static bool IsLeftDirection(PointF a, PointF b, PointF c)
        {
            // move points b and c to origin
            var newB = new PointF(b.X - a.X, b.Y - a.Y);
            var newC = new PointF(c.X - a.X, c.Y - a.Y);
            // calculate pseudoinner product
            var prod = newC.Y * newB.X - newB.Y * newC.X;

            return prod > 0;
        }

        private static PointF GetLowerLeftPoint()
        {
            // points have more then 2 elements
            var res = points.First;
            var node = res.Next;

            while (node != null)
            {
                if (node.Value.X < res.Value.X)
                    res = node;

                if (node.Value.X == res.Value.X &&
                    node.Value.Y < res.Value.Y)
                    res = node;

                node = node.Next;
            }

            return res.Value;
        } 
        #endregion
    }
}
