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

using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Gauge.GaugeGeoms;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.Painters.Context.Animation;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

#if STIDRAWING
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
using GraphicsState = Stimulsoft.Drawing.Drawing2D.GraphicsState;
using Graphics = Stimulsoft.Drawing.Graphics;
using Font = Stimulsoft.Drawing.Font;
using Brush = Stimulsoft.Drawing.Brush;
using Pen = Stimulsoft.Drawing.Pen;
using Matrix = Stimulsoft.Drawing.Drawing2D.Matrix;
#endif

namespace Stimulsoft.Gauge.Painters
{
    public class StiGdiGaugeContextPainter : StiGaugeContextPainter, IStiAnimationContextPainter<StiGaugeGeom>
    {
        #region Fields
        private readonly Graphics g;
        private List<GraphicsState> states = new List<GraphicsState>();
        #endregion

        #region Properties
        public StiAnimationEngine AnimationEngine { get; set; }
        #endregion

        #region Methods.Helpers
        public override SizeF MeasureString(string text, Font font)
        {
            return g.MeasureString(text, font);
        }
        #endregion

        #region Methods.Render
        public override void Render()
        {
            var smoothingMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var textRenderingHint = this.g.TextRenderingHint;
            this.g.TextRenderingHint = TextRenderingHint.AntiAlias;

            foreach (StiGaugeGeom geom in this.Geoms)
            {
                switch (geom.Type)
                {
                    case StiGaugeGeomType.RoundedRectangle:
                        DrawRoundedRectangle(geom as StiRoundedRectangleGaugeGeom);
                        break;

                    case StiGaugeGeomType.Rectangle:
                        DrawRectangle(geom as StiRectangleGaugeGeom);
                        break;

                    case StiGaugeGeomType.GraphicsPath:
                        DrawGraphicsPath(geom as StiGraphicsPathGaugeGeom);
                        break;

                    case StiGaugeGeomType.Pie:
                        DrawPie(geom as StiPieGaugeGeom);
                        break;

                    case StiGaugeGeomType.Ellipse:
                        DrawEllipse(geom as StiEllipseGaugeGeom);
                        break;

                    case StiGaugeGeomType.GraphicsArcGeometry:
                        DrawGraphicsPathArcGeometry(geom as StiGraphicsArcGeometryGaugeGeom);
                        break;

                    case StiGaugeGeomType.PushMatrix:
                        PushMatrix(geom as StiPushMatrixGaugeGeom);
                        break;

                    case StiGaugeGeomType.PopTranform:
                        PopState();
                        break;

                    case StiGaugeGeomType.Text:
                        DrawString(geom as StiTextGaugeGeom);
                        break;

                    case StiGaugeGeomType.RadialRange:
                        RadialRange(geom as StiRadialRangeGaugeGeom);
                        break;
                }
            }

            g.SmoothingMode = smoothingMode;
            this.g.TextRenderingHint = textRenderingHint;
        }
        #endregion

        #region Methods.Render.Helper
        private void DrawRoundedRectangle(StiRoundedRectangleGaugeGeom geom)
        {
            GraphicsPath gp = Stimulsoft.Report.Gauge.Helpers.StiDrawingHelper.GetRoundedPath(geom.Rect, 1, geom.LeftTop, geom.RightTop, geom.RightBottom, geom.LeftBottom);

            if (geom.Background != null)
            {
                using (Brush brush = StiBrush.GetBrush(geom.Background, geom.Rect))
                {
                    g.FillPath(brush, gp);
                }
            }

            if (geom.BorderBrush != null && geom.BorderWidth > 0)
            {
                using (Pen pen = new Pen(StiBrush.GetBrush(geom.BorderBrush, Rectangle.Empty), geom.BorderWidth))
                {
                    g.DrawPath(pen, gp);
                }
            }

            gp.Dispose();
            gp = null;
        }

        private void DrawRectangle(StiRectangleGaugeGeom geom)
        {
            if (geom.Background != null)
            {
                using (Brush brush = StiBrush.GetBrush(geom.Background, geom.Rect))
                {
                    g.FillRectangle(brush, geom.Rect);
                }
            }

            if (geom.BorderBrush != null && geom.BorderWidth > 0)
            {
                using (Pen pen = new Pen(StiBrush.GetBrush(geom.BorderBrush, Rectangle.Empty), geom.BorderWidth))
                {
                    g.DrawRectangle(pen, geom.Rect.X, geom.Rect.Y, geom.Rect.Width, geom.Rect.Height);
                }
            }
        }

        private void DrawGraphicsPath(StiGraphicsPathGaugeGeom geom)
        {
            GraphicsPath gp = new GraphicsPath();
            foreach (StiGaugeGeom gm in geom.Geoms)
            {
                if (gm.Type == StiGaugeGeomType.GraphicsPathArc)
                {
                    StiGraphicsPathArcGaugeGeom arcGeom = gm as StiGraphicsPathArcGaugeGeom;
                    gp.AddArc(arcGeom.X, arcGeom.Y, arcGeom.Width, arcGeom.Height, arcGeom.StartAngle, arcGeom.SweepAngle);
                }
                else if (gm.Type == StiGaugeGeomType.GraphicsPathLines)
                {
                    StiGraphicsPathLinesGaugeGeom linesGeom = gm as StiGraphicsPathLinesGaugeGeom;
                    var points = linesGeom.Points;

                    switch (geom.Animation)
                    {
                        case StiTranslationAnimation animation:
                            points = StiAnimationEngine.GetAnimationTranslation(this, points, animation);
                            break;
                        case StiScaleAnimation animation:
                            points = StiAnimationEngine.GetAnimationScale(this, points, animation);
                            break;
                        case StiRotationAnimation animation:
                            points = StiAnimationEngine.GetAnimationRotation(this, points, animation);
                            break;
                    }

                    gp.AddLines(points);
                }
                else if (gm.Type == StiGaugeGeomType.GraphicsPathLine)
                {
                    StiGraphicsPathLineGaugeGeom lineGeom = gm as StiGraphicsPathLineGaugeGeom;
                    gp.AddLine(lineGeom.P1, lineGeom.P2);
                }
                else if (gm.Type == StiGaugeGeomType.GraphicsPathCloseFigure)
                {
                    gp.CloseFigure();
                }
            }

            if (geom.Background != null)
            {
                using (Brush brush = StiBrush.GetBrush(geom.Background, geom.Rect))
                {
                    g.FillPath(brush, gp);
                }
            }


            if (geom.BorderBrush != null && geom.BorderWidth > 0)
            {
                using (Pen pen = new Pen(StiBrush.GetBrush(geom.BorderBrush, Rectangle.Empty), geom.BorderWidth))
                {
                    g.DrawPath(pen, gp);
                }
            }

            gp.Dispose();
            gp = null;
        }

        private void DrawPie(StiPieGaugeGeom geom)
        {
            if (geom.Background != null)
            {
                using (Brush brush = StiBrush.GetBrush(geom.Background, geom.Rect))
                {
                    g.FillPie(brush, geom.Rect.X, geom.Rect.Y, geom.Rect.Width, geom.Rect.Height, geom.StartAngle, geom.SweepAngle);
                }
            }

            if (geom.BorderBrush != null && geom.BorderWidth > 0)
            {
                using (Pen pen = new Pen(StiBrush.GetBrush(geom.BorderBrush, Rectangle.Empty), geom.BorderWidth))
                {
                    g.DrawPie(pen, geom.Rect, geom.StartAngle, geom.SweepAngle);
                }
            }
        }

        private void DrawEllipse(StiEllipseGaugeGeom geom)
        {
            if (geom.Background != null)
            {
                using (Brush brush = StiBrush.GetBrush(geom.Background, geom.Rect))
                {
                    g.FillEllipse(brush, geom.Rect);
                }
            }

            if (geom.BorderBrush != null && geom.BorderWidth > 0)
            {
                using (Pen pen = new Pen(StiBrush.GetBrush(geom.BorderBrush, Rectangle.Empty), geom.BorderWidth))
                {
                    g.DrawEllipse(pen, geom.Rect);
                }
            }
        }

        private void DrawGraphicsPathArcGeometry(StiGraphicsArcGeometryGaugeGeom geom)
        {
            var gp = Stimulsoft.Report.Gauge.Helpers.StiDrawingHelper.GetArcGeometry(geom.Rect, geom.StartAngle, geom.SweepAngle, geom.StartWidth, geom.EndWidth);
            if (gp != null)
            {
                if (geom.Background != null)
                {
                    using (Brush brush = StiBrush.GetBrush(geom.Background, geom.Rect))
                        g.FillPath(brush, gp);
                }

                if (geom.BorderBrush != null)
                {
                    using (Pen pen = new Pen(StiBrush.GetBrush(geom.BorderBrush, Rectangle.Empty), geom.BorderWidth))
                        g.DrawPath(pen, gp);
                }
            }
        }

        private void DrawString(StiTextGaugeGeom geom)
        {
            using (Brush brush = StiBrush.GetBrush(geom.Foreground, geom.Rect))
                g.DrawString(geom.Text, geom.Font, brush, geom.Rect, geom.StringFormat);

            //if (geom.StringFormat != null)
            //geom.StringFormat.Dispose();
        }

        private void RadialRange(StiRadialRangeGaugeGeom geom)
        {
            GraphicsPath gp = Stimulsoft.Report.Gauge.Helpers.StiDrawingHelper.GetRadialRangeGeometry(geom.CenterPoint, geom.StartAngle, geom.SweepAngle, geom.Radius1, geom.Radius2, geom.Radius3, geom.Radius4);

            if (gp != null)
            {
                if (geom.Background != null)
                {
                    using (Brush brush = StiBrush.GetBrush(geom.Background, geom.Rect))
                    {
                        g.FillPath(brush, gp);
                    }
                }

                if (geom.BorderBrush != null && geom.BorderWidth > 0)
                {
                    using (Pen pen = new Pen(StiBrush.GetBrush(geom.BorderBrush, Rectangle.Empty), geom.BorderWidth))
                    {
                        g.DrawPath(pen, gp);
                    }
                }
            }
        }

        private void PushMatrix(StiPushMatrixGaugeGeom geom)
        {
            PushState();
            Matrix matrix = new Matrix();
            matrix.RotateAt(geom.Angle, geom.CenterPoint);
            g.MultiplyTransform(matrix);
        }

        private void PushState()
        {
            states.Add(g.Save());
        }

        private void PopState()
        {
            g.Restore(states[states.Count - 1]);
            states.RemoveAt(states.Count - 1);
        }
        #endregion

        public StiGdiGaugeContextPainter(Graphics g, StiGauge gauge, RectangleF rect, float zoom)
            : base(gauge, rect, zoom)
        {
            this.g = g;
        }
    }
}
