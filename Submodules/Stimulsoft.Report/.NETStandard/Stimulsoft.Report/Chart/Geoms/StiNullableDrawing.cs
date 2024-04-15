#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using System;
using System.Drawing;
using System.Collections.Generic;
using Stimulsoft.Base;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;

namespace Stimulsoft.Report.Chart
{
    internal static class StiNullableDrawing
    {
        public static void DrawLines(StiContext context, StiPenGeom penGeom, PointF? []points)
        {
            DrawLines(context, penGeom, null, points, null);
        }

        public static void DrawLines(StiContext context, StiPenGeom penGeom, PointF?[] points, StiAnimation animation)
        {
            DrawLines(context, penGeom, null, points, animation);
        }

        public static void DrawLines(StiContext context, StiPenGeom penGeom, PointF?[] pointsStart, PointF?[] points, StiAnimation animation)
        {
            var newPoints = new List<PointF>();
            var newPointsStart = new List<PointF>();

            for (var index = 0; index < points.Length; index++ )
            {
                if (points[index] != null)
                {
                    newPoints.Add(points[index].Value);

                    if (pointsStart != null && pointsStart.Length > index)
                        newPointsStart.Add(pointsStart[index].GetValueOrDefault());
                }                    
                else
                {
                    if (newPoints.Count > 1)
                        DrawLines(context, penGeom, newPointsStart, newPoints, animation);

                    newPoints.Clear();
                }
            }            

            if (newPoints.Count > 1)
                DrawLines(context, penGeom, newPointsStart, newPoints, animation);
        }

        private static void DrawLines(StiContext context, StiPenGeom penGeom, List<PointF> newPointsStart, List<PointF> newPoints, StiAnimation animation)
        {
            if (animation != null)
                context.DrawAnimationLines(penGeom, newPoints.ToArray(), animation);
            else
                context.DrawLines(penGeom, newPoints.ToArray());
        }

        public static void DrawCurve(StiContext context, StiPenGeom penGeom, PointF?[] points, float tension)
        {
            DrawCurve(context, penGeom, points, tension, null);
        }

        public static void DrawCurve(StiContext context, StiPenGeom penGeom, PointF?[] points, float tension, StiAnimation animation)
        {
            DrawCurve(context, penGeom, null, points, tension, animation);
        }

        public static void DrawCurve(StiContext context, StiPenGeom penGeom, PointF?[] pointsStart, PointF?[] points, float tension, StiAnimation animation)
        {
            List<PointF> newPoints = new List<PointF>();
            var newPointsStart = new List<PointF>();

            for (var index = 0; index < points.Length; index++)
            {
                if (points[index] != null)
                {
                    newPoints.Add(points[index].Value);

                    if (pointsStart != null && pointsStart.Length > index)
                        newPointsStart.Add(pointsStart[index].GetValueOrDefault());
                }
                else
                {
                    if (newPoints.Count > 1)
                        DrawCurve(context, penGeom, newPointsStart, newPoints, tension, animation);

                    newPoints.Clear();
                }
            }

            if (newPoints.Count > 1)
                DrawCurve(context, penGeom, newPointsStart, newPoints, tension, animation);
        }

        private static void DrawCurve(StiContext context, StiPenGeom penGeom, List<PointF> newPointsStart, List<PointF> newPoints, float tension, StiAnimation animation)
        {
            if (animation != null)
            {
                context.DrawAnimationCurve(penGeom, newPoints.ToArray(), tension, animation);
            }
            else
                context.DrawCurve(penGeom, newPoints.ToArray(), tension);
        }

        public static List<PointF[]> GetPointsList(PointF?[] points)
        {
            List<PointF[]> list = new List<PointF[]>();

            List<PointF> newPoints = new List<PointF>();

            foreach (PointF? point in points)
            {
                if (point != null)
                    newPoints.Add(point.Value);
                else
                {
                    if (newPoints.Count > 1)
                        list.Add(newPoints.ToArray());

                    newPoints.Clear();
                }
            }

            if (newPoints.Count > 1)
                list.Add(newPoints.ToArray());

            return list;
        }


        public static List<PointF?[]> GetNullablePointsList(PointF?[] points)
        {
            List<PointF?[]> list = new List<PointF?[]>();

            List<PointF?> newPoints = new List<PointF?>();

            foreach (PointF? point in points)
            {
                if (point != null)
                    newPoints.Add(point);
                else
                {
                    if (newPoints.Count > 1)
                        list.Add(newPoints.ToArray());

                    newPoints.Clear();
                }
            }

            //12.01.2012 Артем. При условии, что одно значение точка терялась и для неё не прорисовывалась линия. (newPoints.Count > 1)
            if (newPoints.Count >= 1)
                list.Add(newPoints.ToArray());

            return list;
        }

        public static void GetPointsList(PointF?[] points1, PointF?[] points2, out List<PointF[]> list1, out List<PointF[]> list2)
        {
            list1 = new List<PointF[]>();
            list2 = new List<PointF[]>();

            List<PointF> newPoints1 = new List<PointF>();
            List<PointF> newPoints2 = new List<PointF>();

            int index = 0;
            foreach (PointF? point1 in points1)
            {
                PointF? point2 = points2[index];

                if (point1 != null && point2 != null)
                {
                    newPoints1.Add(point1.Value);
                    newPoints2.Add(point2.Value);
                }
                else
                {
                    if (newPoints1.Count > 1)
                    {
                        list1.Add(newPoints1.ToArray());
                        list2.Add(newPoints2.ToArray());
                    }

                    newPoints1.Clear();
                    newPoints2.Clear();
                }
                index++;
            }

            if (newPoints1.Count > 1)
            {
                list1.Add(newPoints1.ToArray());
                list2.Add(newPoints2.ToArray());
            }
        }
    }
}
