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
using System.Drawing.Drawing2D;

#if NETSTANDARD
using Stimulsoft.System.Windows.Media;
#else
using System.Windows.Media;
#endif

namespace Stimulsoft.Map.Gis.Geography
{
    public sealed class StiGisPolygonMapGeometry : 
        StiGisMapGeometry
    {
        public StiGisPolygonMapGeometry(List<StiGisPointLatLng> points)
        {
            this.points = points;
            this.localPoints = new List<StiGisPoint>(points.Count);
            this.path = new GraphicsPath();
        }

        public StiGisPolygonMapGeometry(List<StiGisPointLatLng> points, List<StiGisPointLatLng> clipPoints)
        {
            this.points = points;
            this.clipPoints = clipPoints;

            this.localPoints = new List<StiGisPoint>(points.Count);
            this.localClipPoints = new List<StiGisPoint>(clipPoints.Count);
        }

        #region Fields
        private List<StiGisPointLatLng> points;
        private List<StiGisPointLatLng> clipPoints;
        private List<StiGisPoint> localPoints;
        private List<StiGisPoint> localClipPoints;
        private GraphicsPath path;
        private GraphicsPath pathClip;
        private PathGeometry pathWpf;
        private PathGeometry pathClipWpf;
        #endregion

        #region Methods.override
        public override void Draw(Graphics g, StiGisCore core)
        {
            if (localPoints == null || localPoints.Count == 0) return;

            if (path == null && localPoints.Count > 0)
            {
                path = new GraphicsPath();

                var pnts = new Point[localPoints.Count];
                for (int index = 0; index < localPoints.Count; index++)
                {
                    var p2 = new Point((int)localPoints[index].X, (int)localPoints[index].Y);
                    pnts[pnts.Length - 1 - index] = p2;
                }

                if (pnts.Length > 2)
                {
                    path.AddPolygon(pnts);
                }
                else if (pnts.Length == 2)
                {
                    path.AddLines(pnts);
                }
            }

            if (pathClip == null && localClipPoints != null && localClipPoints.Count > 0)
            {
                pathClip = new GraphicsPath();

                var pnts = new Point[localClipPoints.Count];
                for (int index = 0; index < localClipPoints.Count; index++)
                {
                    var p2 = new Point((int)localClipPoints[index].X, (int)localClipPoints[index].Y);
                    pnts[pnts.Length - 1 - index] = p2;
                }

                if (pnts.Length > 2)
                {
                    pathClip.AddPolygon(pnts);
                }
                else if (pnts.Length == 2)
                {
                    pathClip.AddLines(pnts);
                }
            }

            if (pathClip != null)
                g.SetClip(pathClip, CombineMode.Exclude);

            using (var fill = new SolidBrush(GetFillGdiColor(core)))
            {
                g.FillPath(fill, path);
            }
            using (var stroke = new global::System.Drawing.Pen(GetStrokeGdiColor(core), (float)GetLineSize(core)))
            {
                g.DrawPath(stroke, path);
            }

            if (pathClip != null)
            {
                g.ResetClip();
                using (var stroke = new global::System.Drawing.Pen(GetStrokeGdiColor(core), (float)GetLineSize(core)))
                {
                    g.DrawPath(stroke, pathClip);
                }
            }
        }

        public override void Draw(DrawingContext dc, StiGisCore core)
        {
            if (localPoints == null || localPoints.Count == 0) return;

            if (pathWpf == null && localPoints != null)
                pathWpf = StiWpfGisPathHelper.GetForPolygon(localPoints);
            if (pathClipWpf == null && localClipPoints != null)
                pathClipWpf = StiWpfGisPathHelper.GetForPolygon(localClipPoints);

            if (pathClipWpf != null)
            {
                var combinedGeomClip = new CombinedGeometry(
                    GeometryCombineMode.Exclude, pathWpf, pathClipWpf);
                dc.PushClip(combinedGeomClip);
            }

            var fill = GetFillWpfColor(core);
            var stroke = new System.Windows.Media.Pen(GetStrokeWpfColor(core), GetLineSize(core));

            dc.DrawGeometry(fill, stroke, pathWpf);

            if (pathClipWpf != null)
            {
                dc.Pop();
                dc.DrawGeometry(fill, stroke, pathClipWpf);
            }
        }

        public override void UpdateLocalPosition(StiGisCore core)
        {
            if (this.points != null)
            {
                this.localPoints.Clear();

                for (int index = 0; index < this.points.Count; index++)
                {
                    var p = core.FromLatLngToLocal(this.points[index]);
                    p.OffsetNegative(core.renderOffset);

                    this.localPoints.Add(p);
                }
            }

            if (this.clipPoints != null)
            {
                this.localClipPoints.Clear();

                for (int index = 0; index < this.clipPoints.Count; index++)
                {
                    var point = core.FromLatLngToLocal(this.clipPoints[index]);
                    point.OffsetNegative(core.renderOffset);

                    this.localClipPoints.Add(point);
                }
            }

            this.UpdateGraphicsPath();
        }

        public override void GetAllPoints(ref List<StiGisPointLatLng> points)
        {
            points.AddRange(this.points);
        }

        public override void Dispose()
        {
            points?.Clear();
            points = null;
            localPoints?.Clear();
            localPoints = null;
            localPoints?.Clear();
            localPoints = null;
            localClipPoints?.Clear();
            localClipPoints = null;

            path?.Dispose();
            path = null;
            pathClip?.Dispose();
            pathClip = null;
        }
        #endregion

        #region Methods
        private void UpdateGraphicsPath()
        {
            path?.Dispose();
            path = null;
            pathClip?.Dispose();
            pathClip = null;
            pathWpf = null;
            pathClipWpf = null;
        }
        #endregion
    }
}