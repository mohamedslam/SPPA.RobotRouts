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

using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiRender3D
    {
        #region Properties
        public StiCamera3D Camera { get; set; }

        public StiProjection Projection { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double HalfWidth { get; set; }

        public double HalfHeight { get; set; }

        public double GlobalRotationX { get; set; }

        public double GlobalRotationY { get; set; }

        public double GlobalScale { get; set; }

        public double ContextScale { get; set; }

        public PointF ContextTranslate { get; set; }
        #endregion

        public StiRender3D(double width, double height)
        {
            this.Width = width;
            this.Height = height;
            this.HalfWidth = width / 2;
            this.HalfHeight = height / 2;

            this.Camera = new StiCamera3D(this, new StiVector3(0, 0, -30/*-5, 6, -55*/));
            this.Projection = new StiProjection(this);
        }
    }
}
