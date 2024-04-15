#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using System.Collections.Generic;

namespace Stimulsoft.Report.Chart
{
    public class StiZYAreaGeom3D : StiAreaGeom3D
    {
        public StiZYAreaGeom3D(double width, double height, StiRender3D render)
            : base(render)
        {
            this.Vertexes = new StiMatrix(new double[,]
            {
                {0, 0, 0, 1 },
                {0, height, 0, 1 },
                {0, height, -width, 1 },
                {0, 0, -width, 1 }
            });

            this.Faces = new List<double[]>()
            {
                new double[]{0, 1, 2, 3 }
            };
        }
    }
}
