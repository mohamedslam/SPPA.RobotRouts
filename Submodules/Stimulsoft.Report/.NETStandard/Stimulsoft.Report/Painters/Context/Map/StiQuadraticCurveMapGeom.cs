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
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
#endif

namespace Stimulsoft.Base.Maps.Geoms
{
    public class StiQuadraticCurveMapGeom : StiMapGeom
    {
        #region Properties
        public double StartX { get; set; }

        public double StartY { get; set; }

        public double ControlPointX { get; set; }

        public double ControlPointY { get; set; }

        public double EndX { get; set; }

        public double EndY { get; set; }

        private PointF FirstControlPoint
        {
            get
            {
                var x1 = StartX + (ControlPointX - StartX) * 2 / 3;
                var y1 = StartY + (ControlPointY - StartY) * 2 / 3;

                return new PointF((float)x1, (float)y1);
            }
        }

        private PointF SecondControlPoint
        {
            get
            {
                var x2 = ControlPointX + (EndX - ControlPointX) / 3;
                var y2 = ControlPointY + (EndY - ControlPointY) / 3;

                return new PointF((float)x2, (float)y2);
            }
        }
        #endregion

        #region Properties.Override
        public override StiMapGeomType GeomType => StiMapGeomType.QuadraticCurve;
        #endregion

        #region Methods
        public void AddToPath(GraphicsPath graphicsPath)
        {
            graphicsPath.AddBezier(new PointF((float)StartX, (float)StartY), FirstControlPoint, SecondControlPoint, new PointF((float)EndX, (float)EndY));
        }
        #endregion

        #region Methods.Override
        public override PointD GetLastPoint() => new PointD(EndX, EndY);

        public override string ToString() => $"QuadraticCurve={StartX},{StartX}";
        #endregion
    }
}