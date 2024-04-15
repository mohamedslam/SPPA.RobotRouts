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

using System;
using System.Collections.Generic;
using Stimulsoft.Base.Drawing;
using System.Threading;
using Stimulsoft.Base.Context;
using Stimulsoft.Report.Components.TextFormats;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiYRadarAxisCoreXF : StiRadarAxisCoreXF
    {
        #region IStiApplyStyle
        public override void ApplyStyle(IStiChartStyle style)
        {
            if (this.Axis.AllowApplyStyle)
            {
                this.YAxis.LineColor = style.Core.AxisLineColor;

                this.YAxis.Labels.Core.ApplyStyle(style);
            }
        }
        #endregion

        #region Methods
        public StiCellGeom Render(StiContext context, RectangleF rect)
        {
            if (!this.Axis.Visible)
                return null;

            RectangleF axisRect = GetAxisRect(context, rect, false, false, true);
            StiRadarAxisGeom geom = new StiRadarAxisGeom(this.Axis as IStiYRadarAxis, axisRect);
            RenderLabels(context, axisRect, geom);

            return geom;
        }

        private List<StiAxisLabelInfoXF> MeasureStripLines(StiContext context, RectangleF rect)
        {
            List<StiAxisLabelInfoXF> infos = new List<StiAxisLabelInfoXF>();

            if (this.YAxis.Info.LabelsCollection != null && this.YAxis.Labels.Placement != StiLabelsPlacement.None)
            {
                int index = 0;
                foreach (StiStripPositionXF label in this.YAxis.Info.LabelsCollection)
                {
                    string text = GetLabelText(label.StripLine, null);

                    #region Init Start Y Position
                    float posY = label.Position;
                    #endregion

                    //If point does not contains in area rectangle then skip this line
                    #region Labels
                    StiStringFormatGeom sf = context.GetGenericStringFormat();
                    StiFontGeom font = StiFontGeom.ChangeFontSize(YAxis.Labels.Font, YAxis.Labels.Font.Size * context.Options.Zoom);
                    StiHorAlignment alignment = GetTextAlignment();

                    #region Choose Rotation Mode, Angle and Point
                    StiRotationMode rotationMode;
                    float angle = -this.YAxis.Labels.Angle;

                    PointF point = PointF.Empty;
                    if (alignment == StiHorAlignment.Left)
                    {
                        rotationMode = StiRotationMode.LeftCenter;
                        point = new PointF(0, posY);
                        angle = 0;
                    }
                    else
                    {
                        rotationMode = StiRotationMode.RightCenter;
                        point = new PointF((rect.Width - GetTicksMaxLength(context) - GetLabelsSpaceAxis(context)), posY);
                    }
                    #endregion

                    #region Placement Two Lines
                    if (this.YAxis.Labels.Placement == StiLabelsPlacement.TwoLines && ((index & 1) != 0))
                    {
                        point.X -= GetLabelsTwoLinesDestination(context);
                    }
                    #endregion

                    RectangleF textRectF = context.MeasureRotatedString(text, font, point, sf,
                        rotationMode, angle, (int)(YAxis.Labels.Width * context.Options.Zoom));

                    #region AxisLabelInfo
                    StiAxisLabelInfoXF info = new StiAxisLabelInfoXF();
                    info.Angle = angle;
                    info.ClientRectangle = textRectF;
                    info.RotationMode = rotationMode;
                    info.TextPoint = point;
                    info.Text = text;
                    info.StripLine = label.StripLine;
                    infos.Add(info);
                    #endregion
                    #endregion

                    index++;
                }
            }
            return infos;
        }

        private void RenderLabels(StiContext context, RectangleF rect, StiRadarAxisGeom geom)
        {
            if (YAxis.Labels.Placement != StiLabelsPlacement.None)
            {
                List<StiAxisLabelGeom> labelGeoms = new List<StiAxisLabelGeom>();
                List<StiAxisLabelInfoXF> infos = MeasureStripLines(context, rect);

                geom.CreateChildGeoms();

                foreach (StiAxisLabelInfoXF info in infos)
                {
                    if (!info.ClientRectangle.IsEmpty)
                    {
                        StiYRadarAxisLabelGeom labelGeom = new StiYRadarAxisLabelGeom(this.YAxis,
                            info.ClientRectangle, info.TextPoint, info.Text, info.StripLine, info.Angle, info.RotationMode);
                        geom.ChildGeoms.Add(labelGeom);
                    }
                }
            }
        }

        protected internal void CalculateStripPositions(float topPosition, float bottomPosition)
        {
            bottomPosition -= topPosition;
            topPosition = 0;
            if (this.YAxis.Info.StripLines == null || this.YAxis.Info.StripLines.Count < 2)
            {
                this.YAxis.Info.StripPositions = new float[0];
            }
            else
            {
                this.YAxis.Info.StripPositions = new float[this.YAxis.Info.StripLines.Count];
                this.YAxis.Info.StripPositions[0] = topPosition;
                this.YAxis.Info.StripPositions[this.YAxis.Info.StripPositions.Length - 1] = bottomPosition;

                for (int index = 1; index < this.YAxis.Info.StripPositions.Length - 1; index++)
                {
                    this.YAxis.Info.StripPositions[index] = topPosition + index * this.YAxis.Info.Step;
                }
            }
        }

        public RectangleF GetAxisRect(StiContext context, RectangleF rect, bool includeAxisArrow, bool includeLabelsHeight, bool isDrawing)
        {
            RectangleF axisRect = new RectangleF(rect.Width / 2 - GetTicksMaxLength(context), 0, GetTicksMaxLength(context), rect.Height / 2);
            return axisRect;
        }

        protected internal float GetTicksMaxLength(StiContext context)
        {
            return this.YAxis.YCore.TicksMaxLength * context.Options.Zoom;
        }

        protected internal float GetLabelsSpaceAxis(StiContext context)
        {
            return 5 * context.Options.Zoom;
        }

        protected internal float GetLabelsTwoLinesDestination(StiContext context)
        {
            return this.YAxis.Labels.Font.SizeInPoints * 2 * context.Options.Zoom;
        }


        protected internal StiHorAlignment GetTextAlignment()
        {
            if (this.YAxis.Labels.Placement == StiLabelsPlacement.TwoLines)
            {
                return StiHorAlignment.Right;
            }
            return this.YAxis.Labels.TextAlignment;
        }

        private void SetTotalNumberCapacity()
        {
            var numberService = this.YAxis.Labels.FormatService as StiNumberFormatService;
            if (numberService != null)
            {
                var step = this.YAxis.Labels.CalculatedStep;
                var sign = step < 1 ? -1 : 1;
                numberService.TotalNumberCapacity = (int)(uint)Math.Floor(sign * Math.Log10(step) + 1);
            }
        }

        internal string GetLabelText(StiStripLineXF line, IStiSeries series)
        {
            object value = line?.ValueObject??"";

            try
            {
                if (this.YAxis.Labels.FormatService != null && !(this.YAxis.Labels.FormatService is StiGeneralFormatService))
                {
                    SetTotalNumberCapacity();
                    return $"{this.YAxis.Labels.TextBefore}{this.YAxis.Labels.FormatService.Format(line.Value)}{this.YAxis.Labels.TextAfter}";
                }

                if (this.YAxis.Labels.Format != null && this.YAxis.Labels.Format.Trim().Length != 0)
                {
                    try
                    {
                        #region If value is string try to convert it to decimal value
                        if (value is string)
                        {
                            decimal result;
                            if (decimal.TryParse(value.ToString().Replace(".", ",").Replace(",", Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator), out result))
                            {
                                value = result;
                            }
                        }
                        #endregion

                        if (!StiChartOptions.OldChartPercentMode && this.YAxis.Labels.Format.StartsWith("P", StringComparison.InvariantCulture))
                        {
                            int signs = 0;
                            if (this.YAxis.Labels.Format.Length > 1)
                            {
                                int.TryParse(this.YAxis.Labels.Format.Remove(0, 1), out signs);
                            }

                            return string.Format("{0}{1:N" + signs.ToString() + "}{2}{3}", this.YAxis.Labels.TextBefore, value, "%", this.YAxis.Labels.TextAfter);
                        }
                        else return string.Format("{0}{1:" + this.YAxis.Labels.Format + "}{2}", this.YAxis.Labels.TextBefore, value, this.YAxis.Labels.TextAfter);
                    }
                    catch
                    {
                    }
                }
                return string.Format("{0}{1}{2}", this.YAxis.Labels.TextBefore, value, this.YAxis.Labels.TextAfter);
            }
            catch
            {
            }
            return value.ToString();
        }

        protected internal StiStringFormatGeom GetStringFormatGeom(StiContext context)
        {
            StiStringFormatGeom sf = context.GetGenericStringFormat();
            sf.Trimming = StringTrimming.None;
            sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
            if (this.YAxis.Labels.Width > 0)
            {
                StiHorAlignment alignment = GetTextAlignment();

                if (alignment == StiHorAlignment.Left)
                    sf.Alignment = StringAlignment.Near;
                else if (alignment == StiHorAlignment.Right)
                    sf.Alignment = StringAlignment.Far;
                else if (alignment == StiHorAlignment.Center)
                    sf.Alignment = StringAlignment.Center;
            }
            return sf;
        }

        protected internal StiFontGeom GetFontGeom(StiContext context)
        {
            StiFontGeom font = StiFontGeom.ChangeFontSize(this.YAxis.Labels.Font, this.YAxis.Labels.Font.Size * context.Options.Zoom);
            return font;
        }
        #endregion

        #region Properties
        public IStiYRadarAxis YAxis
        {
            get
            {
                return Axis as IStiYRadarAxis;
            }
        }

        public StiAxisInfoXF Info
        {
            get
            {
                return this.YAxis.Info;
            }
            set
            {
                this.YAxis.Info = value;
            }
        }

        internal float TicksMaxLength
        {
            get
            {
                if (Axis.Visible)
                {
                    return Math.Max(
                        YAxis.Ticks.Visible ? YAxis.Ticks.Length : 0,
                        YAxis.Ticks.MinorVisible ? YAxis.Ticks.MinorLength : 0);
                }
                else return 0;
            }
        }
        #endregion

        public StiYRadarAxisCoreXF(IStiRadarAxis axis) : base(axis)
        {
        }
	}
}
