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
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Drawing;

namespace Stimulsoft.Report.Chart
{
    internal class StiPie3dQuadrilateral
    {
        #region Fields
        private readonly StiAnimation animation;
        private readonly RectangleF rectangle;
        private readonly PointF point1;
        private readonly PointF point2;
        private readonly PointF point3;
        private readonly PointF point4;
        private readonly bool toClose;
        public static readonly StiPie3dQuadrilateral Empty = new StiPie3dQuadrilateral();
        #endregion

        #region Methods
        internal void Draw(StiContext context, StiPenGeom pen, StiBrush brush, bool isMouseOver)
        {
            var path = new List<StiSegmentGeom>();
            path.Add(new StiLineSegmentGeom(point1, point2));
            path.Add(new StiLineSegmentGeom(point2, point3));
            path.Add(new StiLineSegmentGeom(point3, point4));

            if (toClose)
                path.Add(new StiCloseFigureSegmentGeom());

            var rect = context.GetPathBounds(path);

            if (animation != null)
            {
                context.DrawAnimationPathElement(brush, brush, pen, path, rectangle, null, this, animation, null);
            }
            else
            {
                context.FillPath(brush, path, rect);
                context.DrawPath(pen, path, rect);
            }

            if (isMouseOver)
                context.FillPath(StiMouseOverHelper.GetMouseOverColor(), path, rect);
        }

        public bool Contains(PointF point)
        {
            var points = new PointF[] { point1, point2, point3, point4 };

            if (points.Length == 0)
                return false;
            return Contains(point, points);
        }

        public static bool Contains(PointF point, PointF[] cornerPoints)
        {
            int intersections = 0;
            for (int i = 1; i < cornerPoints.Length; ++i)
            {
                if (DoesIntersect(point, cornerPoints[i], cornerPoints[i - 1]))
                    ++intersections;
            }
            if (DoesIntersect(point, cornerPoints[cornerPoints.Length - 1], cornerPoints[0]))
                ++intersections;
            return (intersections % 2 != 0);
        }

        private static bool DoesIntersect(PointF point, PointF point1, PointF point2)
        {
            float x2 = point2.X;
            float y2 = point2.Y;
            float x1 = point1.X;
            float y1 = point1.Y;
            if ((x2 < point.X && x1 >= point.X) || (x2 >= point.X && x1 < point.X))
            {
                float y = (y2 - y1) / (x2 - x1) * (point.X - x1) + y1;
                return y > point.Y;
            }
            return false;
        }
        #endregion

        protected StiPie3dQuadrilateral()
        {
        }

        public StiPie3dQuadrilateral(RectangleF rectangle, PointF point1, PointF point2, PointF point3, PointF point4, bool toClose, StiAnimation animation)
        {
            this.rectangle = rectangle;
            this.point1 = point1;
            this.point2 = point2;
            this.point3 = point3;
            this.point4 = point4;
            this.toClose = toClose;
            this.animation = animation;
        }
    }
}
