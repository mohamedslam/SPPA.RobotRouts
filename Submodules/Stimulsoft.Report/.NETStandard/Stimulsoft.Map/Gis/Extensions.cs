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

namespace Stimulsoft.Map.Gis
{
    public static class Extensions
    {
        public static Color FromArgb(byte alpha, Color baseColor)
        {
            return Color.FromArgb(alpha, baseColor.R, baseColor.G, baseColor.B);
        }

        public static System.Windows.Media.Color ToWpfColor(global::System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static System.Windows.Media.Brush ToWpfBrush(global::System.Drawing.Color color)
        {
            return new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        public static void AddLineSegment(this PathSegmentCollection pathColl, double x, double y)
        {
            var lineSegment = new LineSegment();
            var point = new Point(x, y);
            lineSegment.Point = point;

            pathColl.Add(lineSegment);
        }
    }
}