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
using System.Collections.Generic;

#if NETSTANDARD
using Stimulsoft.System.Windows;
using Stimulsoft.System.Windows.Media;
#else
using System.Windows;
using System.Windows.Media;
#endif

namespace Stimulsoft.Map.Gis.Geography.Helpers
{
    internal static class StiWpfGisPathHelper
    {
        #region Methods
        public static PathGeometry GetForLineString(global::System.Drawing.PointF[] localPoints)
        {
            if (localPoints == null) return null;

            var path = new PathFigure();
            for (int index = 0; index < localPoints.Length; index++)
            {
                var points = new List<Point>();
                if (path.Segments.Count == 0 && index == 0)
                {
                    path.StartPoint = new Point(localPoints[index].X, localPoints[index].Y);
                }
                else
                {
                    if (index != 0)
                        points.Add(new Point(localPoints[index].X, localPoints[index].Y));
                }
                path.Segments.Add(new PolyLineSegment(points, true));
            }

            return new PathGeometry(new List<PathFigure>() { path });
        }

        public static PathGeometry GetForPolygon(List<StiGisPoint> localPoints)
        {
            if (localPoints == null) return null;

            var path = new PathFigure();

            var points = new List<Point>();
            for (int index = 0; index < localPoints.Count; index++)
            {
                if (path.Segments.Count == 0 && index == 0)
                {
                    path.StartPoint = new Point(localPoints[index].X, localPoints[index].Y);
                }
                    
                points.Add(new Point(localPoints[index].X, localPoints[index].Y));
            }

            path.Segments.Add(new PolyLineSegment(points, true));

            return new PathGeometry(new List<PathFigure>() { path });
        }
        #endregion
    }
}