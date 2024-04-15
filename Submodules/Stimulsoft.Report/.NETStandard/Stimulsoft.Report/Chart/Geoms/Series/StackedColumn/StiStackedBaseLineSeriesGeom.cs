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
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiStackedBaseLineSeriesGeom : StiSeriesGeom
    {
        #region Properties
        private PointF?[] points;
        public PointF?[] Points
        {
            get
            {
                return points;
            }
        }
        #endregion

        #region Methods
        internal static RectangleF GetClientRectangle(PointF?[] points)
        {
            if (points == null || points.Length == 0)
                return RectangleF.Empty;

            PointF minPoint = PointF.Empty;
            PointF maxPoint = PointF.Empty;
            foreach (PointF? point in points)
            {
                if (point == null) continue;

                if (minPoint == PointF.Empty)
                {
                    minPoint = point.Value;
                    maxPoint = point.Value;
                }
                else
                {
                    minPoint.X = Math.Min(minPoint.X, point.Value.X);
                    minPoint.Y = Math.Min(minPoint.Y, point.Value.Y);

                    maxPoint.X = Math.Max(maxPoint.X, point.Value.X);
                    maxPoint.Y = Math.Max(maxPoint.Y, point.Value.Y);
                }
            }

            return new RectangleF(minPoint.X, minPoint.Y, maxPoint.X - minPoint.X, maxPoint.Y - minPoint.Y);
        }

        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
        }
        #endregion

        public StiStackedBaseLineSeriesGeom(StiAreaGeom areaGeom, PointF?[] points, IStiSeries series)
            : base(areaGeom, series, GetClientRectangle(points))
        {
            this.points = points;
        }
    }
}
