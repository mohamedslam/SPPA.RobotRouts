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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using System.Collections.Generic;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiLineSeriesGeom3D :
        StiGeom3D,
        IStiBorderColor,
        IStiColor
    {
        #region Properties
        public Color Color { get; set; }

        public Color BorderColor { get; set; }
        #endregion

        public StiLineSeriesGeom3D(StiPoint3D sPoint1, StiPoint3D sPoint2, StiPoint3D ePoint1, StiPoint3D ePoint2, Color color, Color borderColor, StiRender3D render) :
            base(render)
        {
            this.Color = color;
            this.BorderColor = borderColor;

            this.Vertexes = new StiMatrix(new double[,]
               {
                {sPoint1.X, sPoint1.Y, sPoint1.Z, 1 },
                {sPoint2.X, sPoint2.Y, sPoint2.Z, 1 },
                {ePoint1.X, ePoint1.Y, ePoint1.Z, 1 },
                {ePoint2.X, ePoint2.Y, ePoint2.Z, 1 },
               });

            this.Faces = new List<double[]>()
            {
                new double[]{0, 1, 2, 3 }
            };

            var colorCorrect = sPoint1.Y >= ePoint1.Y ? color : StiColorUtils.ChangeDarkness(color, 0.4f);
            this.ColorsFaces = new Color[]
            {
                colorCorrect
            };
        }
    }
}
