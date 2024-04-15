#region Copyright (C) 2003-2019 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{																	}
{	Copyright (C) 2003-2019 Stimulsoft     							}
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
#endregion Copyright (C) 2003-2019 Stimulsoft

using Stimulsoft.Base.Drawing;
using System;
using System.Drawing.Drawing2D;

#if STIDRAWING
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
#endif

namespace Stimulsoft.Base.Maps.Geoms
{
    public class StiEllipticalArcMapGeom : StiMapGeom
    {
        #region Fields
        private const double RadiansPerDegree = Math.PI / 180.0;
        private const double DoublePI = Math.PI * 2;
        #endregion

        #region Properties
        public double StartX { get; set; }
        public double StartY { get; set; }
        public double RadiusX { get; set; }
        public double RadiusY { get; set; }
        public double Angle { get; set; }
        public StiSvgArcSize Size { get; set; }
        public StiSvgArcSweep Sweep { get; set; }
        public double EndX { get; set; }
        public double EndY { get; set; }
        #endregion

        #region Properties.Override
        public override StiMapGeomType GeomType => StiMapGeomType.EllipticalArc;
        #endregion

        #region Methods
        private static double CalculateVectorAngle(double ux, double uy, double vx, double vy)
        {
            var ta = Math.Atan2(uy, ux);
            var tb = Math.Atan2(vy, vx);

            if (tb >= ta)
            {
                return tb - ta;
            }

            return DoublePI - (ta - tb);
        }

        public void AddToPath(GraphicsPath graphicsPath)
        {
            if (StartX == EndX && StartY == EndY)
                return;

            if (RadiusX == 0.0f && RadiusY == 0.0f)
            {
                graphicsPath.AddLine((float)StartX, (float)StartY, (float)EndX, (float)EndY);
                return;
            }

            var sinPhi = Math.Sin(Angle * RadiansPerDegree);
            var cosPhi = Math.Cos(Angle * RadiansPerDegree);

            var x1dash = cosPhi * (StartX - EndX) / 2.0 + sinPhi * (StartY - EndY) / 2.0;
            var y1dash = -sinPhi * (StartX - EndX) / 2.0 + cosPhi * (StartY - EndY) / 2.0;

            double root;
            var numerator = RadiusX * RadiusX * RadiusY * RadiusY - RadiusX * RadiusX * y1dash * y1dash - RadiusY * RadiusY * x1dash * x1dash;

            var rx = RadiusX;
            var ry = RadiusY;

            if (numerator < 0.0)
            {
                var s = (float)Math.Sqrt(1.0 - numerator / (RadiusX * RadiusX * RadiusY * RadiusY));

                rx *= s;
                ry *= s;
                root = 0.0;
            }
            else
            {
                root = ((Size == StiSvgArcSize.Large && Sweep == StiSvgArcSweep.Positive) || (Size == StiSvgArcSize.Small && Sweep == StiSvgArcSweep.Negative) ? -1.0 : 1.0) * Math.Sqrt(numerator / (RadiusX * RadiusX * y1dash * y1dash + RadiusY * RadiusY * x1dash * x1dash));
            }

            var cxdash = root * rx * y1dash / ry;
            var cydash = -root * ry * x1dash / rx;

            var cx = cosPhi * cxdash - sinPhi * cydash + (StartX + EndX) / 2.0;
            var cy = sinPhi * cxdash + cosPhi * cydash + (StartY + EndY) / 2.0;

            var theta1 = CalculateVectorAngle(1.0, 0.0, (x1dash - cxdash) / rx, (y1dash - cydash) / ry);
            var dtheta = CalculateVectorAngle((x1dash - cxdash) / rx, (y1dash - cydash) / ry, (-x1dash - cxdash) / rx, (-y1dash - cydash) / ry);

            if (Sweep == StiSvgArcSweep.Negative && dtheta > 0)
            {
                dtheta -= 2.0 * Math.PI;
            }
            else if (Sweep == StiSvgArcSweep.Positive && dtheta < 0)
            {
                dtheta += 2.0 * Math.PI;
            }

            var segments = (int)Math.Ceiling((double)Math.Abs(dtheta / (Math.PI / 2.0)));
            var delta = dtheta / segments;
            var t = 8.0 / 3.0 * Math.Sin(delta / 4.0) * Math.Sin(delta / 4.0) / Math.Sin(delta / 2.0);

            var startX = StartX;
            var startY = StartY;

            for (var i = 0; i < segments; ++i)
            {
                var cosTheta1 = Math.Cos(theta1);
                var sinTheta1 = Math.Sin(theta1);
                var theta2 = theta1 + delta;
                var cosTheta2 = Math.Cos(theta2);
                var sinTheta2 = Math.Sin(theta2);

                var endpointX = cosPhi * rx * cosTheta2 - sinPhi * ry * sinTheta2 + cx;
                var endpointY = sinPhi * rx * cosTheta2 + cosPhi * ry * sinTheta2 + cy;

                var dx1 = t * (-cosPhi * rx * sinTheta1 - sinPhi * ry * cosTheta1);
                var dy1 = t * (-sinPhi * rx * sinTheta1 + cosPhi * ry * cosTheta1);

                var dxe = t * (cosPhi * rx * sinTheta2 + sinPhi * ry * cosTheta2);
                var dye = t * (sinPhi * rx * sinTheta2 - cosPhi * ry * cosTheta2);

                graphicsPath.AddBezier((float)startX, (float)startY, (float)(startX + dx1), (float)(startY + dy1),
                    (float)(endpointX + dxe), (float)(endpointY + dye), (float)endpointX, (float)endpointY);

                theta1 = theta2;
                startX = (float)endpointX;
                startY = (float)endpointY;
            }
        }
        #endregion

        #region Methods.Override
        public override PointD GetLastPoint() => new PointD(EndX, EndY);

        public override string ToString() => $"EllipticalArc={StartX},{StartY}, {RadiusX},{RadiusY}, {Angle},{Size},{Sweep},{EndX},{EndY}";
        #endregion
    }
}