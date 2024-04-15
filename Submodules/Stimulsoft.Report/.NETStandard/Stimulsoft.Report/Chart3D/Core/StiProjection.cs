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

using System;

namespace Stimulsoft.Report.Chart
{
    public class StiProjection
    {
        #region Fields
        private double near;
        private double far;
        private double right;
        private double left;
        private double top;
        private double bottom;
        #endregion

        #region Properties
        public StiMatrix ProjectionMatrix { get; }

        public StiMatrix ToScreenMatrix { get; }
        #endregion

        public StiProjection(StiRender3D render)
        {
            var camera = render.Camera;
            this.near = camera.NearPlane;
            this.far = camera.FarPlane;
            this.right = Math.Tan(camera.h_fov / 2);
            this.left = -this.right;
            this.top = Math.Tan(camera.v_fov / 2);
            this.bottom = -this.top;

            var m00 = 2 / (right - left);
            var m11 = 2 / (top - bottom);
            var m22 = (far + near) / (far - near);
            var m32 = -2 * near * far / (far - near);
            this.ProjectionMatrix = new StiMatrix(new double[,] {

                { m00, 0, 0, 0},
                {0, m11, 0, 0},
                {0, 0, m22, 1},
                {0, 0, m32, 0}
            });

            var hw = render.HalfWidth;
            var hh = render.HalfHeight;

            this.ToScreenMatrix = new StiMatrix(new double[,] {

                { hw, 0, 0, 0},
                {0, -hh, 0, 0},
                {0, 0, 1, 0},
                {hw, hh, 0, 1}
            });
        }
    }
}
