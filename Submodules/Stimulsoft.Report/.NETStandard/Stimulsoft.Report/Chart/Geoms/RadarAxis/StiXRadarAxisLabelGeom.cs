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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiXRadarAxisLabelGeom : StiCellGeom
    {
        #region Properties
        public Color BorderColor { get; }

        public StiBrush LabelBrush { get; }

        public string Text { get; }

        public float Angle { get; }

        public PointF Point { get; set; }

        public RectangleF LabelRect { get; }

        public IStiXRadarAxis Axis { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            var rect = Rectangle.Round(this.ClientRectangle);

            var borderPen = new StiPenGeom(BorderColor);
            var font = StiFontGeom.ChangeFontSize(Axis.Labels.Font, (float)(Axis.Labels.Font.Size * context.Options.Zoom));
            var sf = context.GetGenericStringFormat();

            sf.Trimming = StringTrimming.None;

            if (!this.Axis.Labels.WordWrap)
                sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;

            #region Draw Not Rotation Labels
            if (!this.Axis.Labels.RotationLabels)
            {
                #region Draw label area
                if (Axis.Labels.Antialiasing)
                    context.PushSmoothingModeToAntiAlias();

                if (!rect.IsEmpty)
                {
                    context.FillRectangle(Axis.Labels.Brush, GetDrawRectangle(), null);

                    if (Axis.Labels.DrawBorder)
                        context.DrawRectangle(borderPen, GetDrawRectangle());
                }


                if (Axis.Labels.Antialiasing)
                    context.PopSmoothingMode();
                #endregion                

                context.DrawRotatedString(
                    Text, font, LabelBrush, GetDrawRectangle(), sf, StiRotationMode.CenterCenter, 0, Axis.Labels.Antialiasing, (int)(Axis.Labels.Width * context.Options.Zoom));
            }
            #endregion

            #region Draw Rotation Labels
            else
            {
                #region Draw label area
                int distX = (int)Math.Round(LabelRect.X + LabelRect.Width / 2, 0);
                int distY = (int)Math.Round(LabelRect.Y + LabelRect.Height / 2, 0);

                if (Axis.Labels.Antialiasing)
                    context.PushSmoothingModeToAntiAlias();

                context.PushTranslateTransform(distX, distY);
                context.PushRotateTransform(Angle);

                if (!rect.IsEmpty)
                {
                    context.FillRectangle(Axis.Labels.Brush, rect, null);

                    if (Axis.Labels.DrawBorder)
                        context.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width, rect.Height);
                }

                context.PopTransform();
                context.PopTransform();

                if (Axis.Labels.Antialiasing)
                    context.PopSmoothingMode();
                #endregion

                var mode = StiRotationMode.CenterBottom;

                float textAngle = Angle;
                if (Angle >= 90 && Angle <= 270)
                {
                    textAngle = Angle + 180;
                    mode = StiRotationMode.CenterTop;
                }

                context.DrawRotatedString(
                    Text, font, LabelBrush, Point,
                    sf, mode, textAngle, Axis.Labels.Antialiasing, (int)(Axis.Labels.Width * context.Options.Zoom));
            }
            #endregion
        }
        #endregion

        public Rectangle GetDrawRectangle()
        {
            var drawRectangleF = new RectangleF(this.Point.X, Point.Y, this.ClientRectangle.Width, this.ClientRectangle.Height);
            var drawRectangle = Rectangle.Round(drawRectangleF);

            var mode = StiRotationMode.CenterBottom;

            #region Rotation Labels
            if (!this.Axis.Labels.RotationLabels)
            {
                if (Angle > 0 && Angle < 180)
                {
                    mode = StiRotationMode.LeftCenter;
                }
                else if (Angle < 360 && Angle > 180)
                {
                    mode = StiRotationMode.RightCenter;
                }
                else if (Angle == 0)
                {
                    mode = StiRotationMode.CenterBottom;
                }
                else if (Angle == 180)
                {
                    mode = StiRotationMode.CenterTop;
                }
            }

            switch (mode)
            {
                case StiRotationMode.CenterBottom:
                    drawRectangle = new Rectangle(drawRectangle.X - drawRectangle.Width/2, drawRectangle.Y - drawRectangle.Height, drawRectangle.Width, drawRectangle.Height);
                    break;

                case StiRotationMode.CenterCenter:
                    drawRectangle = new Rectangle(drawRectangle.X - drawRectangle.Width / 2, drawRectangle.Y - drawRectangle.Height/2, drawRectangle.Width, drawRectangle.Height);
                    break;

                case StiRotationMode.CenterTop:
                    drawRectangle = new Rectangle(drawRectangle.X - drawRectangle.Width / 2, drawRectangle.Y, drawRectangle.Width, drawRectangle.Height);
                    break;

                case StiRotationMode.LeftBottom:
                    drawRectangle = new Rectangle(drawRectangle.X, drawRectangle.Y - drawRectangle.Height, drawRectangle.Width, drawRectangle.Height);
                    break;

                case StiRotationMode.LeftCenter:
                    drawRectangle = new Rectangle(drawRectangle.X, drawRectangle.Y - drawRectangle.Height / 2, drawRectangle.Width, drawRectangle.Height);
                    break;

                case StiRotationMode.LeftTop:
                    break;

                case StiRotationMode.RightBottom:
                    drawRectangle = new Rectangle(drawRectangle.X - drawRectangle.Width, drawRectangle.Y - drawRectangle.Height, drawRectangle.Width, drawRectangle.Height);
                    break;

                case StiRotationMode.RightCenter:
                    drawRectangle = new Rectangle(drawRectangle.X - drawRectangle.Width, drawRectangle.Y - drawRectangle.Height/2, drawRectangle.Width, drawRectangle.Height);
                    break;

                case StiRotationMode.RightTop:
                    drawRectangle = new Rectangle(drawRectangle.X - drawRectangle.Width, drawRectangle.Y, drawRectangle.Width, drawRectangle.Height);
                    break;

            }

            return drawRectangle;
        }
        #endregion

        public StiXRadarAxisLabelGeom(IStiXRadarAxis axis, string text, StiBrush labelBrush, Color borderColor,
            float angle, RectangleF clientRectangle, RectangleF labelRect, PointF point)
            : base(clientRectangle)
        {
            this.Axis = axis;
            this.LabelRect = labelRect;
            this.Text = text;
            this.Angle = angle;
            this.Point = point;
            this.LabelBrush = labelBrush;
            this.BorderColor = borderColor;

        }
    }
}