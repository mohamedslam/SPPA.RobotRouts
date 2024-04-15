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
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart.Geoms.Series.Pie
{
    public class StiPieEmptySeriesElementGeom : StiCellGeom
    {
        public override void Draw(StiContext context)
        {
            var rectPie = this.ClientRectangle;
            var pen = new StiPenGeom(Color.LightGray);

            context.PushSmoothingModeToAntiAlias();
            context.FillEllipse(Color.FromArgb(50, Color.LightGray), rectPie, null);
            context.DrawEllipse(pen, rectPie);
            context.DrawLine(pen, rectPie.X + rectPie.Width / 2, rectPie.Y + rectPie.Height / 2, rectPie.Right, rectPie.Y + rectPie.Height / 2);
            context.DrawLine(pen, rectPie.X + rectPie.Width / 2, rectPie.Y + rectPie.Height / 2, rectPie.X + rectPie.Width / 2, rectPie.Bottom);
            context.PopSmoothingMode();
        }

        public StiPieEmptySeriesElementGeom(RectangleF clientRectangle) : base(clientRectangle)
        {
        }
    }
}
