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

using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Gauge.GaugeGeoms;
using Stimulsoft.Report.Gauge.Primitives;
using Stimulsoft.Report.Painters;
using System;
using System.Drawing;

namespace Stimulsoft.Report.Components.Gauge.Primitives
{
    internal sealed class StiRadialBarGeometry : IStiScaleBarGeometry
    {
        #region Fields
        private readonly StiRadialScale scale;
        #endregion

        #region Properties
        private SizeF size;
        public SizeF Size => this.size;

        private RectangleF rectGeometry;
        public RectangleF RectGeometry => rectGeometry;

        public float Radius { get; private set; }
        public float Diameter { get; private set; }

        private PointF center;
        public PointF Center => this.center;
        #endregion

        #region Methods
        public void CheckRectGeometry(RectangleF rect)
        {
            this.size = rect.Size;

            PointF centerPoint = scale.Center;
            centerPoint = new PointF(rect.X + this.size.Width * centerPoint.X, rect.Y + this.size.Height * centerPoint.Y);

            float width;
            float height;

            if (scale.RadiusMode == StiRadiusMode.Auto)
            {
                width = Math.Min(this.size.Width, this.size.Height) * scale.GetRadius();
                height = width;

                Diameter = width;
                Radius = width / 2;
            }
            else if (scale.RadiusMode == StiRadiusMode.Width)
            {
                width = this.size.Width * scale.GetRadius();
                height = this.size.Height;

                Diameter = width;
                Radius = width / 2;
            }
            else
            {
                width = this.size.Width;
                height = this.size.Height * scale.GetRadius();

                Diameter = height;
                Radius = height / 2;
            }

            this.rectGeometry = new RectangleF(centerPoint.X - width / 2, centerPoint.Y - height / 2, width, height);
            this.center = new PointF(this.rectGeometry.Left + this.rectGeometry.Width / 2, this.rectGeometry.Top + this.rectGeometry.Height / 2);
        }

        public void DrawScaleGeometry(StiGaugeContextPainter context)
        {
            float startWidth, endWidth;
            if (scale.IsReversed)
            {
                startWidth = scale.GetEndWidth();
                endWidth = scale.GetStartWidth();
            }
            else
            {
                startWidth = scale.GetStartWidth();
                endWidth = scale.GetEndWidth();
            }

            if (rectGeometry.Width > 0 && rectGeometry.Height > 0)
            {
                context.AddGraphicsArcGeometryGaugeGeom(rectGeometry, this.scale.Brush, this.scale.BorderBrush, 1f, scale.StartAngle, scale.GetSweepAngle(), startWidth, endWidth);
            }
        }
        #endregion

        #region IStiScaleBarGeometry Members
        public float GetRestToLenght()
        {
            throw new NotImplementedException();
        }

        public StiGraphicsPathLinesGaugeGeom DrawGeometry(StiGaugeContextPainter context, float StartValue, float EndValue, float StartWidth, float EndWidth, float Offset, StiPlacement Placement, ref RectangleF rect, bool returnOnlyRect)
        {
            throw new NotImplementedException();
        }
        #endregion

        public StiRadialBarGeometry(StiRadialScale scale)
        {
            this.size = new SizeF(0, 0);
            this.center = new PointF(0, 0);
            this.rectGeometry = RectangleF.Empty;
            this.Radius = 0f;
            this.Diameter = 0f;

            this.scale = scale;
        }
    }
}