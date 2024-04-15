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

using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiCenterAxisLabelsGeom3D : StiSeriesLabelsGeom3D
    {
        #region Properties
        public string Text { get; }

        public Color LabelColor { get; }

        public Color LabelBorderColor { get; }

        public StiBrush SeriesBrush { get; }

        public StiBrush SeriesLabelsBrush { get; }

        public Color SeriesBorderColor { get; }

        public StiFontGeom Font { get; }
        #endregion

        #region Properties
        public override void DrawElements(StiContext context, StiMatrix vertices)
        {
            var size = GetLabelRect(context);

            var x = (float)vertices.Grid[0, 0];
            var y = (float)vertices.Grid[0, 1];

            var point = GetPoint(x, y);

            var rect = new RectangleF(point.X - size.Width / 2, point.Y - size.Height / 2, size.Width, size.Height);

            DrawLabelArea(context, rect);
            DrawLabelText(context, rect);
        }

        protected void DrawLabelArea(StiContext context, RectangleF rect)
        {
            if (rect.IsEmpty) return;

            var borderPen = SeriesLabels.DrawBorder ? new StiPenGeom(LabelBorderColor, 1) : null;

            context.FillRectangle(SeriesLabelsBrush, Rectangle.Ceiling(rect));
            if (SeriesLabels.DrawBorder)
                context.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        protected void DrawLabelText(StiContext context, RectangleF rect)
        {
            var labelBrush = new StiSolidBrush(LabelColor);
            var font = StiFontGeom.ChangeFontSize(SeriesLabels.Font, SeriesLabels.Font.Size * context.Options.Zoom);
            var sf = SeriesLabels.Core.GetStringFormatGeom(context);
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            context.DrawRotatedString(Text, font, labelBrush, Rectangle.Ceiling(rect), sf, StiRotationMode.CenterCenter, 0, SeriesLabels.Antialiasing, (int)rect.Width);
        }

        protected SizeF GetLabelRect(StiContext context)
        {
            return context.MeasureString(Text, Font);
        }
        #endregion

        public StiCenterAxisLabelsGeom3D(IStiSeriesLabels seriesLabels, IStiSeries series, int indeх, double value,
            string labelText, Color labelColor, Color labelBorderColor, StiBrush seriesBrush, StiBrush seriesLabelsBrush,
            Color seriesBorderColor, StiFontGeom font, StiPoint3D point3D, StiRender3D render3D)
            : base(seriesLabels, series, indeх, value, render3D)
        {
            Text = labelText;
            LabelColor = labelColor;
            LabelBorderColor = labelBorderColor;
            SeriesBrush = seriesBrush;
            SeriesLabelsBrush = seriesLabelsBrush;
            SeriesBorderColor = seriesBorderColor;
            Font = font;

            this.Vertexes = new StiMatrix(new double[,] {
                 {point3D.X, point3D.Y, point3D.Z, 1}
            });
        }        
    }
}
