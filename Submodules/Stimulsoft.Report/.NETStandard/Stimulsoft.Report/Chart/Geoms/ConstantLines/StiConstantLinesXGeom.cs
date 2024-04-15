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
    public class StiConstantLinesVerticalGeom : StiCellGeom
    {
        #region Properties
        public IStiConstantLines Line { get; }

        public PointF Point { get; }

        public StiRotationMode Mode { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            var rect = this.ClientRectangle;

            #region Draw Lines
            var pen = new StiPenGeom(Line.LineColor, Line.LineWidth);
            pen.PenStyle = Line.LineStyle;
            context.DrawLine(pen, rect.Left, rect.Top, rect.Left, rect.Bottom);
            #endregion

            #region Draw Text
            if (Line.TitleVisible)
            {
                var brush = new StiSolidBrush(Line.LineColor);
                var font = StiFontGeom.ChangeFontSize(Line.Font, (float)(Line.Font.Size * context.Options.Zoom));

                var sf = context.GetGenericStringFormat();

                var text = StiReportParser.Parse(Line.Text, Line.Chart as StiChart);
                context.DrawRotatedString(text, font, brush, Point, sf, Mode, 90, Line.Antialiasing, 0);
            }
            #endregion
        }

        public override bool Contains(float x, float y)
        {
            if (Invisible) return false;

            var lineHalfWidth = (float)Math.Ceiling(Line.LineWidth / 2);
            var rect = new RectangleF(this.ClientRectangle.Left - lineHalfWidth, this.ClientRectangle.Top, Line.LineWidth, this.ClientRectangle.Height);

            return rect.Contains(x, y);
        }
        #endregion

        public StiConstantLinesVerticalGeom(IStiConstantLines line, RectangleF clientRectangle, PointF point, StiRotationMode mode)
            :
            base(clientRectangle)
        {
            this.Line = line;
            this.Point = point;
            this.Mode = mode;
        }
    }
}
