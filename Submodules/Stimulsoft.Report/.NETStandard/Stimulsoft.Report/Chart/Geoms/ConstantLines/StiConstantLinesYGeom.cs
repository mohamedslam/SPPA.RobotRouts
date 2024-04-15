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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;
using Stimulsoft.Report.Dashboard;
using System;

namespace Stimulsoft.Report.Chart
{
    public class StiConstantLinesYGeom : StiCellGeom
    {
        #region Properties
        private IStiConstantLines line;
        public IStiConstantLines Line
        {
            get
            {
                return line;
            }
        }

        private PointF point;
        public PointF Point
        {
            get
            {
                return point;
            }
        }

        private StiRotationMode mode;
        public StiRotationMode Mode
        {
            get
            {
                return mode;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            var rect = this.ClientRectangle;

            #region Draw Lines
            var pen = new StiPenGeom(line.LineColor, line.LineWidth);
            pen.PenStyle = line.LineStyle;
            context.DrawLine(pen, rect.Left, rect.Top, rect.Left + rect.Width, rect.Top);
            #endregion

            #region Draw Text
            if (line.TitleVisible)
            {
                var brush = new StiSolidBrush(line.LineColor);
                var font = StiFontGeom.ChangeFontSize(line.Font, (float)(line.Font.Size * context.Options.Zoom));

                var sf = context.GetGenericStringFormat();

                var text = StiReportParser.Parse(Line.Text, Line.Chart as StiChart);
                context.DrawRotatedString(text, font, brush, Point, sf, Mode, 0, line.Antialiasing, 0);
            }
            #endregion
        }

        public override bool Contains(float x, float y)
        {
            if (Invisible) return false;

            var lineHalfWidth = (float)Math.Ceiling(line.LineWidth / 2);
            var rect = new RectangleF(this.ClientRectangle.Left, this.ClientRectangle.Top - lineHalfWidth, this.ClientRectangle.Width, line.LineWidth);

            return rect.Contains(x, y);
        }
        #endregion

        public StiConstantLinesYGeom(IStiConstantLines line, RectangleF clientRectangle, PointF point, StiRotationMode mode)
            :
            base(clientRectangle)
        {
            this.line = line;
            this.point = point;
            this.mode = mode;
        }
    }
}
