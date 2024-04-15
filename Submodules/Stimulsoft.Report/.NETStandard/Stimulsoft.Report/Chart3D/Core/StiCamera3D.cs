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

namespace Stimulsoft.Report.Chart
{
    public class StiCamera3D
    {
        #region Fields
        private StiVector4 forward = new StiVector4(0, 0, 1, 1);

        private StiVector4 up = new StiVector4(0, 1, 0, 1);

        private StiVector4 right = new StiVector4(1, 0, 0, 1);

        public double h_fov;

        public double v_fov;

        public double NearPlane = 0.1;

        public double FarPlane = 100;

        #endregion

        #region Properties
        public StiVector4 Position { get; private set; }

        public StiRender3D Render { get; }
        #endregion

        #region Methods
        public StiMatrix TranslateMatrix()
        {
            return new StiMatrix(new double[,]
            {
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 },
                { -Position.X, -Position.Y, -Position.Z, 1 }
            });
        }

        public StiMatrix RotateMatrix()
        {
            return new StiMatrix(new double[,]
            {
                { right.X, up.X, forward.X, 0 },
                { right.Y, up.Y, forward.Y, 0 },
                { right.Z, up.Z, forward.Z, 0 },
                { 0, 0, 0, 1 }
            });
        }

        public StiMatrix CameraMatrix()
        {
            return TranslateMatrix() * RotateMatrix();
        }
        #endregion

        public StiCamera3D(StiRender3D render, StiVector3 point)
        {
            this.Render = render;
            this.Position = new StiVector4(point.X, point.Y, point.Z, 1);

            this.h_fov = 1;// Math.PI / 3;
            this.v_fov = this.h_fov * (render.Height / render.Width);
        }

    }
}
