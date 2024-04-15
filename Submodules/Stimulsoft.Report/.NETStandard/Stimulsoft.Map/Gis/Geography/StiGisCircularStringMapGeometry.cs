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
using Stimulsoft.Map.Gis.Geography.Helpers;
using System.Collections.Generic;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Media;
#else
using System.Windows.Media;
#endif

namespace Stimulsoft.Map.Gis.Geography
{
    public sealed class StiGisCircularStringMapGeometry : 
        StiGisMapGeometry
    {
        public StiGisCircularStringMapGeometry(List<StiGisPointLatLng> points)
        {
            this.points = points;
        }

        #region Fields
        private List<StiGisPointLatLng> points;
        private PointF[] localPoints;
        #endregion

        #region Methods.override
        public override void Draw(Graphics g, StiGisCore core)
        {
            if (localPoints == null) return;
            
            using (var stroke = new global::System.Drawing.Pen(GetStrokeGdiColor(core), (float)GetLineSize(core)))
            {
                g.DrawCurve(stroke, localPoints);
            }
        }

        public override void Draw(DrawingContext dc, StiGisCore core)
        {
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                var pnts = StiGisCurveHelper.CardinalSpline(localPoints, false);

                context.BeginFigure(pnts[0], true, false);
                for (int index = 1; index < pnts.Count; index += 3)
                {
                    context.BezierTo(pnts[index], pnts[index + 1], pnts[index + 2], true, false);
                }
            }

            var stroke = new System.Windows.Media.Pen(GetStrokeWpfColor(core), GetLineSize(core));
            dc.DrawGeometry(null, stroke, geometry);
        }

        public override void UpdateLocalPosition(StiGisCore core)
        {
            if (this.points != null)
            {
                this.localPoints = new PointF[this.points.Count];

                for (int index = 0; index < this.points.Count; index++)
                {
                    var point = core.FromLatLngToLocal(this.points[index]);
                    point.OffsetNegative(core.renderOffset);

                    this.localPoints[index] = new PointF(point.X, point.Y);
                }
            }
        }

        public override void GetAllPoints(ref List<StiGisPointLatLng> points)
        {
            points.AddRange(this.points);
        }

        public override void Dispose()
        {
            points?.Clear();
            points = null;
            localPoints = null;
        }
        #endregion
    }
}