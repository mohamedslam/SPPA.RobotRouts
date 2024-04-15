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

using System.Drawing;
using System.Collections.Generic;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart.Geoms.Series.Doughnut
{
    public class StiDoughnutEmptySeriesElementGeom : StiCellGeom
    {
        #region Methods
        public override void Draw(StiContext context)
        {
            var rectPie = this.ClientRectangle;
            var pen = new StiPenGeom(Color.LightGray);

            var path = new List<StiSegmentGeom>();

            path.Add(new StiArcSegmentGeom(rectPie, 0, 360));
            path.Add(new StiLineSegmentGeom(rectPie.Right, rectPie.Y + rectPie.Height / 2, rectPie.X + rectPie.Width * 3 / 4, rectPie.Y + rectPie.Height / 2));
            path.Add(new StiArcSegmentGeom(new RectangleF(rectPie.X + rectPie.Width / 4, rectPie.Y + rectPie.Height / 4, rectPie.Width / 2, rectPie.Height / 2), 0, 360));


            context.PushSmoothingModeToAntiAlias();

            context.FillPath(Color.FromArgb(50, Color.LightGray), path, rectPie);
            context.DrawPath(pen, path, rectPie);

            context.DrawLine(pen, rectPie.X + rectPie.Width / 2, rectPie.Y + rectPie.Height * 3 / 4, rectPie.X + rectPie.Width / 2, rectPie.Bottom);
            context.PopSmoothingMode();
        }
        #endregion

        public StiDoughnutEmptySeriesElementGeom(RectangleF clientRectangle) : base(clientRectangle)
        {
        }
    }
}
