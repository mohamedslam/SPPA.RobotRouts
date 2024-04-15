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

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiRadarAxisGeom : StiCellGeom
    {
        #region Properties
        private IStiYRadarAxis axis;
        public IStiYRadarAxis Axis
        {
            get
            {
                return axis;
            }
        }
        #endregion

        #region Methods
		private void DrawAxisLine(StiContext context, RectangleF rect)
        {
            float posX = rect.Right;

            var penLine = new StiPenGeom(Axis.LineColor, Axis.LineWidth);
            penLine.PenStyle = Axis.LineStyle;
            context.DrawLine(penLine, posX, rect.Y, posX, rect.Bottom);
        }

        private void DrawMinorTicks(StiContext context, StiPenGeom pen, float posX, float posY1, float posY2, IStiAxisTicks ticks)
        {
            float step = posY2 - posY1;
            float minorStep = step / (ticks.MinorCount + 1);

            float minorLength = ticks.MinorLength * context.Options.Zoom;
            for (int minorIndex = 1; minorIndex <= ticks.MinorCount; minorIndex++)
            {
                float posY = posY1 + minorStep * minorIndex;
                float posX2 = posX - minorLength;

                context.DrawLine(pen, posX, posY, posX2, posY);
            }
        }

        private void DrawTicks(StiContext context, RectangleF rect, IStiAxisTicks ticks, StiPenGeom penLine)
        {
            if (!ticks.Visible) return;

            float ticksLength = ticks.Length * context.Options.Zoom;
            float posX1 = rect.Right;
            float posX2 = posX1 - ticksLength;


            int index = 0;
            foreach (StiStripPositionXF strip in Axis.Info.TicksCollection)
            {
                float posY = strip.Position;
                context.DrawLine(penLine, posX1, posY, posX2, posY);
                if (ticks.MinorVisible && index != Axis.Info.TicksCollection.Count - 1)
                {
                    float posY2 = Axis.Info.TicksCollection[index + 1].Position;
                    DrawMinorTicks(context, penLine, posX1, posY, posY2, ticks);
                }
                index++;
            }
        }

        private void DrawAxis(StiContext context, RectangleF rect)
        {
            var penLine = new StiPenGeom(Axis.LineColor, Axis.LineWidth);
            penLine.PenStyle = Axis.LineStyle;

            DrawTicks(context, rect, ((IStiRadarArea)Axis.Area).YAxis.Ticks, penLine);
			DrawAxisLine(context, rect);
        }
        

        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            RectangleF rect = this.ClientRectangle;
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            DrawAxis(context, rect);


            //Red line
            //StiPenGeom pen = new StiPenGeom(Color.Red);
            //pen.PenStyle = StiPenStyle.Dash;
            //context.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }
        #endregion

        public StiRadarAxisGeom(IStiYRadarAxis axis, RectangleF clientRectangle)
            : base(clientRectangle)
        {
            this.axis = axis;
        }
    }
}
