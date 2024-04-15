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
using System.Collections.Generic;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report.Painters
{
    public abstract class StiGaugeContextPainter
    {
        #region Properties
        public float Zoom { get; set; }

        public RectangleF Rect { get; set; }

        public StiGauge Gauge { get; set; }

        public List<StiGaugeGeom> Geoms { get; set; } = new List<StiGaugeGeom>();

        public List<StiAnimation> Animations { get; }
        #endregion

        #region Methods
        public static Font ChangeFontSize(Font font, float zoom)
        {
            float newFontSize = font.Size * zoom;
            if (newFontSize < 1) newFontSize = 1;
            return new Font(
                font.FontFamily.Name,
                newFontSize,
                font.Style,
                font.Unit,
                font.GdiCharSet,
                font.GdiVerticalFont);
        }
        #endregion

        #region Methods.Add
        public void AddPieGaugeGeom(RectangleF rect, StiBrush background, StiBrush borderBrush, float borderWidth, float startAngle, float sweepAngle)
        {
            if (rect.Width > 0 && rect.Height > 0)
                this.Geoms.Add(new StiPieGaugeGeom(rect, background, borderBrush, borderWidth, startAngle, sweepAngle));
        }

        public void AddEllipseGaugeGeom(RectangleF rect, StiBrush background, StiBrush borderBrush, float borderWidth)
        {
            if (rect.Width > 0 && rect.Height > 0)
                this.Geoms.Add(new StiEllipseGaugeGeom(rect, background, borderBrush, borderWidth));
        }

        public void AddGraphicsArcGeometryGaugeGeom(RectangleF rect, StiBrush background, StiBrush borderBrush, float borderWidth, float startAngle,
            float sweepAngle, float startWidth, float endWidth)
        {
            if (rect.Width > 0 && rect.Height > 0)
                this.Geoms.Add(new StiGraphicsArcGeometryGaugeGeom(rect, background, borderBrush, borderWidth, startAngle, sweepAngle, startWidth, endWidth));
        }

        public void AddPopTranformGaugeGeom()
        {
            this.Geoms.Add(new StiPopTranformGaugeGeom());
        }

        public void AddPushMatrixGaugeGeom(float angle, PointF centerPoint)
        {
            this.Geoms.Add(new StiPushMatrixGaugeGeom(angle, centerPoint));
        }

        public void AddRadialRangeGaugeGeom(RectangleF rect, StiBrush background, StiBrush borderBrush, float borderWidth, PointF centerPoint,
            float startAngle, float sweepAngle, float radius1, float radius2, float radius3, float radius4)
        {
            if (rect.Width > 0 && rect.Height > 0)
            {
                this.Geoms.Add(new StiRadialRangeGaugeGeom(rect, background, borderBrush, borderWidth, centerPoint,
                    startAngle, sweepAngle, radius1, radius2, radius3, radius4));
            }
        }

        public void AddRectangleGaugeGeom(RectangleF rect, StiBrush background, StiBrush borderBrush, float borderWidth)
        {
            if (rect.Width > 0 && rect.Height > 0)
            {
                this.Geoms.Add(new StiRectangleGaugeGeom(rect, background, borderBrush, borderWidth));
            }
        }

        public void AddRoundedRectangleGaugeGeom(RectangleF rect, StiBrush background, StiBrush borderBrush, float borderWidth, int leftTop, int rightTop, int rightBottom, int leftBottom)
        {
            if (rect.Width > 0 && rect.Height > 0)
            {
                this.Geoms.Add(new StiRoundedRectangleGaugeGeom(rect, background, borderBrush, borderWidth, leftTop, rightTop, rightBottom, leftBottom));
            }
        }

        public void AddTextGaugeGeom(string text, Font font, StiBrush foreground, RectangleF rect, StringFormat sf)
        {
            if (rect.Width > 0 && rect.Height > 0)
                this.Geoms.Add(new StiTextGaugeGeom(text, font, foreground, rect, sf));
        }

        public void AddGraphicsPathGaugeGeom(StiGraphicsPathGaugeGeom geom)
        {
            if (geom.Rect.Width > 0 && geom.Rect.Height > 0)
            {
                Animations.Add(geom.Animation);
                this.Geoms.Add(geom);
            }
        }
        #endregion

        #region Methods.Render
        public abstract void Render();
        #endregion

        #region Methods abstract
        public abstract SizeF MeasureString(string text, Font font);
        #endregion

        public StiGaugeContextPainter(StiGauge gauge, RectangleF rect, float zoom)
        {
            this.Gauge = gauge;
            this.Rect = rect;
            this.Zoom = zoom;
            Animations = new List<StiAnimation>();
        }
    }
}
