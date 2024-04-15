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

namespace Stimulsoft.Report.Chart
{
    public class StiSunburstEmptySeriesElementGeom : StiCellGeom
    {
        #region Methods
        public override void Draw(StiContext context)
        {
            var rectPie = this.ClientRectangle;

            var widthElement = rectPie.Width / 6;
            DrawSunburst(context, this.ClientRectangle, widthElement*0.85f);
            DrawSunburst(context, new RectangleF(this.ClientRectangle.X + widthElement, this.ClientRectangle.Y + widthElement, this.ClientRectangle.Width - 2 * widthElement, this.ClientRectangle.Height - 2 * widthElement), widthElement * 0.85f);
        } 

        private void DrawSunburst(StiContext context, RectangleF rectPie, float widthElement)
        {
            var pen = new StiPenGeom(Color.LightGray);

            var path = new List<StiSegmentGeom>();

            path.Add(new StiArcSegmentGeom(rectPie, 0, 360));
            path.Add(new StiLineSegmentGeom(rectPie.Right, rectPie.Y + rectPie.Height / 2, rectPie.X + (rectPie.Width - widthElement), rectPie.Y + rectPie.Height / 2));
            path.Add(new StiArcSegmentGeom(new RectangleF(rectPie.X + widthElement, rectPie.Y + widthElement, rectPie.Width - 2 * widthElement, rectPie.Height - 2 * widthElement), 0, 360));


            context.PushSmoothingModeToAntiAlias();

            context.FillPath(Color.FromArgb(50, Color.LightGray), path, rectPie);
            context.DrawPath(pen, path, rectPie);

            context.DrawLine(pen, rectPie.X + rectPie.Width / 2, rectPie.Y + (rectPie.Height - widthElement), rectPie.X + rectPie.Width / 2, rectPie.Bottom);
            context.PopSmoothingMode();
        }
        #endregion

        public StiSunburstEmptySeriesElementGeom(RectangleF clientRectangle) : base(clientRectangle)
        {
        }
    }
}
