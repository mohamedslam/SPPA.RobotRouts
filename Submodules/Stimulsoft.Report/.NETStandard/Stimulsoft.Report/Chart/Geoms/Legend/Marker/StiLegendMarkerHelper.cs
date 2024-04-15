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

using Stimulsoft.Base.Context;
using System.Collections.Generic;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    internal static class StiLegendMarkerHelper
    {
        internal static List<StiSegmentGeom> GetSteppedMarkerPath(RectangleF rect)
        {
            var path = new List<StiSegmentGeom>();

            path.Add(new StiLineSegmentGeom(rect.X, rect.Y, rect.X + rect.Width, rect.Y));
            path.Add(new StiLineSegmentGeom(rect.X + rect.Width, rect.Y, rect.X + rect.Width, rect.Y + rect.Height));
            path.Add(new StiLineSegmentGeom(rect.X + rect.Width, rect.Y + rect.Height, rect.X, rect.Y + rect.Height));
            path.Add(new StiLineSegmentGeom(rect.X, rect.Y + rect.Height, rect.X, rect.Y));

            return path;
        }

        internal static List<StiSegmentGeom> GetAreaMarkerPath(RectangleF rect)
        {
            var width1 = rect.Width / 5;
            var height1 = rect.Height / 5;

            var path = new List<StiSegmentGeom>();
            path.Add(new StiLineSegmentGeom(rect.X, rect.Y + height1 * 3.5f, rect.X, rect.Y + height1 * 3.5f));
            path.Add(new StiLineSegmentGeom(rect.X, rect.Y + height1 * 3.5f, rect.X + width1 * 2, rect.Y + height1 * 0.5f));
            path.Add(new StiLineSegmentGeom(rect.X + width1 * 2, rect.Y + height1 * 0.5f, rect.X + width1 * 4, rect.Y + height1 * 2.5f));
            path.Add(new StiLineSegmentGeom(rect.X + width1 * 4, rect.Y + height1 * 2.5f, rect.X + width1 * 5, rect.Y + height1 * 1.5f));
            path.Add(new StiLineSegmentGeom(rect.X + width1 * 5, rect.Y + height1 * 1.5f, rect.X + width1 * 5, rect.Y + height1 * 5));
            path.Add(new StiLineSegmentGeom(rect.X + width1 * 5, rect.Y + height1 * 5, rect.X, rect.Y + height1 * 5));
            path.Add(new StiLineSegmentGeom(rect.X, rect.Y + height1 * 5, rect.X, rect.Y + height1 * 3));

            return path;
        }

        internal static PointF[] GetAreaMarkerLinePoints(RectangleF rect)
        {
            var width1 = rect.Width / 5;
            var height1 = rect.Height / 5;

            return new PointF[]
            {
                new PointF(rect.X, rect.Y + height1 * 3.5f),
                new PointF(rect.X + width1 * 2, rect.Y + height1 * 0.5f),
                new PointF(rect.X + width1 * 4, rect.Y + height1 * 2.5f),
                new PointF(rect.X + width1 * 5, rect.Y + height1 * 1.5f)
            };
        }

        internal static List<StiSegmentGeom> GetSplineAreaMarkerPath(RectangleF rect)
        {
            var width1 = rect.Width / 5;
            var height1 = rect.Height / 5;

            var path = new List<StiSegmentGeom>();
            path.Add(new StiLineSegmentGeom(rect.X, rect.Y + height1 * 5, rect.X, rect.Y + height1 * 3.5f));

            var points = StiLegendMarkerHelper.GetSplineAreaMarkerLinePoints(rect);

            path.Add(new StiCurveSegmentGeom(points, 0.55f));

            path.Add(new StiLineSegmentGeom(rect.X + width1 * 5, rect.Y + height1 * 1.5f, rect.X + width1 * 5, rect.Y + height1 * 5));
            path.Add(new StiLineSegmentGeom(rect.X + width1 * 5, rect.Y + height1 * 5, rect.X, rect.Y + height1 * 5));

            return path;
        }

        internal static PointF[] GetSplineAreaMarkerLinePoints(RectangleF rect)
        {
            var width1 = rect.Width / 5;
            var height1 = rect.Height / 5;

            return new PointF[]
            {
                new PointF(rect.X, rect.Y + height1 * 3.5f),
                new PointF(rect.X + width1 * 2, rect.Y + height1 * 0.5f),
                new PointF(rect.X + width1 * 4, rect.Y + height1 * 2.5f),
                new PointF(rect.X + width1 * 5, rect.Y + height1 * 1.5f)
            };
        }
    }
}
