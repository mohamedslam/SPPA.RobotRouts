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
using System.ComponentModel;
using System.Collections.Generic;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;
using System.Threading;
using System.Globalization;
using Stimulsoft.Report.Components.TextFormats;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public abstract class StiYAxisCoreXF : StiAxisCoreXF
    {
        #region Methods
        public override StiCellGeom Render(StiContext context, RectangleF rect)
        {
            if (!this.Axis.Visible)
                return null;

            var axisRect = GetAxisRect(context, rect, false, false, true, false);
            axisRect.X = 0;
            axisRect.Y = 0;

            if (Axis.Interaction.ShowScrollBar && 
                ((Axis is IStiYRightAxis && !Axis.Area.ReverseHor) || (Axis is IStiYLeftAxis && Axis.Area.ReverseHor)))
                axisRect.X += StiAxisCoreXF.DefaultScrollBarSize * context.Options.Zoom;

            var geom = new StiYAxisGeom(this.Axis as IStiYAxis, axisRect, false);
            
            RenderLabels(context, axisRect, geom);
            if (!this.Axis.Range.Auto)
            {
                if (this.Axis.Area.ReverseVert)
                {
                    axisRect.Y = (float)((this.Axis.Info.Minimum - this.Axis.Range.Minimum) * this.Axis.Info.Dpi);
                }
                else
                {
                    axisRect.Y = (float)((this.Axis.Info.Maximum - this.Axis.Range.Maximum) * this.Axis.Info.Dpi);
                }
                axisRect.Height = (float)((this.Axis.Range.Maximum - this.Axis.Range.Minimum) * this.Axis.Info.Dpi);
            }
            RenderTitle(context, axisRect, geom);

            return geom;
        }

        public override StiCellGeom RenderView(StiContext context, RectangleF rect)
        {
            if (!this.Axis.Visible)
                return null;

            var axisRect = GetAxisRect(context, rect, false, false, true, true);
            return new StiYAxisViewGeom(Axis as IStiYAxis, axisRect, false);
        }

        public void RenderScrollBar(StiContext context, RectangleF axisRect, StiYAxisViewGeom axisGeom)
        {
            if (this.Axis.Interaction.ShowScrollBar)
            {
                var scrollBarRect = axisRect;
                scrollBarRect.Y = 0;

                scrollBarRect.Width = StiAxisCoreXF.DefaultScrollBarSize * context.Options.Zoom;

                if ((this.Axis is IStiYLeftAxis && (!this.Axis.Area.ReverseHor)) ||
                    (this.Axis is IStiYRightAxis && (this.Axis.Area.ReverseHor)))
                    scrollBarRect.X = axisRect.Width - scrollBarRect.Width;
                else
                    scrollBarRect.X = 0;

                var scrollBarGeom = new StiVertScrollBarGeom(this.Axis as IStiYAxis, scrollBarRect);
                axisGeom.CreateChildGeoms();
                axisGeom.ChildGeoms.Add(scrollBarGeom);

                scrollBarRect.X = 0;
                scrollBarRect.Y = 0;
                scrollBarRect.Inflate(-2, -2);

                if (Axis.Interaction.RangeScrollEnabled)
                {
                    #region Up Button
                    var upButtonRect = scrollBarRect;
                    upButtonRect.Height = scrollBarRect.Width;
                    var upButtonGeom = new StiUpButtonGeom(this.Axis as IStiYAxis, upButtonRect);

                    scrollBarGeom.CreateChildGeoms();
                    scrollBarGeom.ChildGeoms.Add(upButtonGeom);
                    #endregion

                    #region Down Button
                    var downButtonRect = scrollBarRect;
                    downButtonRect.Height = scrollBarRect.Width;
                    downButtonRect.Y = scrollBarRect.Bottom - downButtonRect.Height;
                    var downButtonGeom = new StiDownButtonGeom(this.Axis as IStiYAxis, downButtonRect);

                    scrollBarGeom.CreateChildGeoms();
                    scrollBarGeom.ChildGeoms.Add(downButtonGeom);
                    #endregion

                    scrollBarRect.Inflate(0, -scrollBarRect.Width - 2);
                }

                #region Track Bar
                var trackBarRect = scrollBarRect;
                var rollAxis = Axis.Area.YAxis;
                var axisCore = Axis.Area.Core as StiAxisAreaCoreXF;

                if (!rollAxis.Range.Auto)
                {
                    float dpi = (float)(scrollBarRect.Height / axisCore.ScrollRangeY);
                    float y = (float)axisCore.ScrollValueY * dpi;
                    float height = (float)axisCore.ScrollViewY * dpi;

                    trackBarRect = new RectangleF(scrollBarRect.X, y + scrollBarRect.Y, scrollBarRect.Width, height);
                }

                var trackBarGeom = new StiVertTrackBarGeom(this.Axis as IStiYAxis, trackBarRect, scrollBarGeom);
                scrollBarGeom.CreateChildGeoms();
                scrollBarGeom.ChildGeoms.Add(trackBarGeom);
                #endregion
            }
        }

        public StiCellGeom RenderCenter(StiContext context, RectangleF rect)
        {
            if (!this.Axis.Visible)
                return null;

            if (this.Axis.Area.XAxis.Info.Minimum >= 0)
                return null;

            var axisRect = GetCenterAxisRect(context, rect, false, false, true);
            axisRect.X = 0;
            axisRect.Y = 0;

            var geom = new StiYAxisGeom(this.Axis as IStiYAxis, axisRect, true);
            return geom;
        }

        public StiCellGeom RenderCenterView(StiContext context, RectangleF rect)
        {
            if (!this.Axis.Visible)
                return null;

            if (this.Axis.Area.XAxis.Info.Minimum >= 0)
                return null;

            var axisRect = GetCenterAxisRect(context, rect, false, false, true);
            var geom = new StiYAxisViewGeom(Axis as IStiYAxis, axisRect, true);
            return geom;
        }

        private void SetTotalNumberCapacity()
        {
            var numberService = Axis.Labels.FormatService as StiNumberFormatService;
            if (numberService != null)
            {
                var step = Axis.Labels.CalculatedStep;
                var sign = step < 1 ? -1 : 1;
                numberService.TotalNumberCapacity = (int)(uint)Math.Floor(sign * Math.Log10(step) + 1);
            }
        }

        internal string GetLabelText(StiStripLineXF line)
        {
            object value = line.ValueObject;
            CultureInfo storedCulture = null;

            try
            {
                if (Axis.Labels.FormatService != null && !(Axis.Labels.FormatService is StiGeneralFormatService))
                {
                    SetTotalNumberCapacity();
                    if (value is DateTime)
                    {
                        if (Axis.Labels.FormatService is StiDateFormatService || Axis.Labels.FormatService is StiTimeFormatService)
                            return $"{Axis.Labels.TextBefore}{Axis.Labels.FormatService.Format(value)}{Axis.Labels.TextAfter}";
                        else
                            return $"{Axis.Labels.TextBefore}{value}{Axis.Labels.TextAfter}";
                    }

                    return $"{Axis.Labels.TextBefore}{Axis.Labels.FormatService.Format(line.Value)}{Axis.Labels.TextAfter}";
                }

                if (this.Axis.Labels.Format != null && this.Axis.Labels.Format.Trim().Length != 0)
                {
                    var culture = ((StiChart)this.Axis.Area.Chart)?.Report?.GetParsedCulture();
                    if (!string.IsNullOrEmpty(culture))
                    {
                        storedCulture = Thread.CurrentThread.CurrentCulture;
                        Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                    }

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

                        if (!StiChartOptions.OldChartPercentMode && this.Axis.Labels.Format.StartsWith("P", StringComparison.InvariantCulture))
                        {
                            int signs = 0;
                            if (this.Axis.Labels.Format.Length > 1)
                            {
                                int.TryParse(this.Axis.Labels.Format.Remove(0, 1), out signs);
                            }

                            return string.Format("{0}{1:N" + signs.ToString() + "}{2}{3}", this.Axis.Labels.TextBefore, value, "%", this.Axis.Labels.TextAfter);
                        }
                        else return string.Format("{0}{1:" + this.Axis.Labels.Format + "}{2}", this.Axis.Labels.TextBefore, value, this.Axis.Labels.TextAfter);
                    }
                    catch
                    {
                    }
                }
                return string.Format("{0}{1}{2}", this.Axis.Labels.TextBefore, value, this.Axis.Labels.TextAfter);
            }
            catch
            {
            }
            finally
            {
                if (storedCulture != null)
                    Thread.CurrentThread.CurrentCulture = storedCulture;
            }
            return value.ToString();
        }

        private List<StiAxisLabelInfoXF> MeasureStripLines(StiContext context, RectangleF rect)
        {
            var infos = new List<StiAxisLabelInfoXF>();

            if (this.Axis.Info.LabelsCollection != null && this.Axis.Labels.Placement != StiLabelsPlacement.None)
            {
                var titleSize = GetAxisTitleSize(context);

                int index = 0;
                foreach (StiStripPositionXF label in this.Axis.Info.LabelsCollection)
                {
                    bool sideFlag = this.Axis.Area.ReverseHor ? this.IsLeftSide : this.IsRightSide;

                    #region Remove First Label on YRightAxis if XTopAxis have arrow
                    if (sideFlag && this.Axis.Area.XTopAxis.ArrowStyle != StiArrowStyle.None && index == 0)
                    {
                        infos.Add(new StiAxisLabelInfoXF());
                        index++;
                        continue;
                    }
                    #endregion

                    #region Remove Last Label on YRightAxis if XAxis have arrow
                    if (sideFlag && this.Axis.Area.XAxis.ArrowStyle != StiArrowStyle.None && index == Axis.Info.StripLines.Count - 1)
                    {
                        infos.Add(new StiAxisLabelInfoXF());
                        index++;
                        continue;
                    }
                    #endregion

                    string text = GetLabelText(label.StripLine);

                    #region Init Start Y Position
                    float posY = label.Position;
                    #endregion

                    //If point does not contains in area rectangle then skip this line
                    #region Labels
                    var sf = context.GetGenericStringFormat();
                    var font = StiFontGeom.ChangeFontSize(Axis.Labels.Font, Axis.Labels.Font.Size * context.Options.Zoom);
                    var alignment = GetTextAlignment();

                    #region Choose Rotation Mode, Angle and Point
                    StiRotationMode rotationMode;
                    float angle = -this.Axis.Labels.Angle;

                    var point = PointF.Empty;
                    if (IsLeftSide)
                    {
                        if (alignment == StiHorAlignment.Left)
                        {
                            rotationMode = StiRotationMode.LeftCenter;
                            point = new PointF(titleSize.Width, posY);
                            angle = 0;
                        }
                        else if (alignment == StiHorAlignment.Center)
                        {
                            rotationMode = StiRotationMode.CenterCenter;
                            point = new PointF((rect.Width - titleSize.Width)/2, posY);
                            angle = 0;
                        }
                        else
                        {
                            rotationMode = StiRotationMode.RightCenter;
                            point = new PointF((rect.Width - GetTicksMaxLength(context) - GetLabelsSpaceAxis(context)), posY);
                        }
                    }
                    else
                    {
                        if (alignment == StiHorAlignment.Left)
                        {
                            rotationMode = StiRotationMode.LeftCenter;
                            point = new PointF(GetTicksMaxLength(context) + GetLabelsSpaceAxis(context), posY);
                        }
                        else if (alignment == StiHorAlignment.Center)
                        {
                            rotationMode = StiRotationMode.CenterCenter;
                            point = new PointF((rect.Width - titleSize.Width)/2, posY);
                            angle = 0;
                        }
                        else
                        {
                            rotationMode = StiRotationMode.RightCenter;
                            point = new PointF(rect.Width - titleSize.Width, posY);
                            angle = 0;
                        }
                    }
                    #endregion

                    #region Placement Two Lines
                    if (this.Axis.Labels.Placement == StiLabelsPlacement.TwoLines && ((index & 1) != 0))
                    {
                        if (IsLeftSide) point.X -= GetLabelsTwoLinesDestination(context);
                        if (IsRightSide) point.X += GetLabelsTwoLinesDestination(context);
                    }
                    #endregion

                    var textRectF = context.MeasureRotatedString(text, font, point, sf,
                        rotationMode, angle, (int)(Axis.Labels.Width * context.Options.Zoom));

                    #region Check Custom Label Width 
                    var customWidth = (int)(Axis.Labels.Width * context.Options.Zoom);
                    if (customWidth != 0)
                    {
                        switch (rotationMode)
                        {
                            case StiRotationMode.CenterBottom:
                            case StiRotationMode.CenterCenter:
                            case StiRotationMode.CenterTop:
                                textRectF.X -= (customWidth - textRectF.Width) / 2;
                                break;

                            case StiRotationMode.RightBottom:
                            case StiRotationMode.RightCenter:
                            case StiRotationMode.RightTop:
                                textRectF.X -= customWidth - textRectF.Width;
                                break;
                        }

                        textRectF.Width = customWidth;
                    }
                    #endregion

                    #region AxisLabelInfo
                    var info = new StiAxisLabelInfoXF
                    {
                        Angle = angle,
                        ClientRectangle = textRectF,
                        RotationMode = rotationMode,
                        TextPoint = point,
                        Text = text,
                        StripLine = label.StripLine
                    };
                    infos.Add(info);
                    #endregion
                    #endregion

                    index++;
                }
            }

            if (this.Axis.Range.Auto)
            {
                var maxLabelHeight = this.Axis.Labels.Font.Height * context.Options.Zoom;
                float heightPerLabel = rect.Height / Axis.Info.StripLines.Count * 1.5f;
                var labelsStep = Math.Ceiling(maxLabelHeight / heightPerLabel);

                int labelsIndex = 0;
                var infos2 = new List<StiAxisLabelInfoXF>();
                infos.Reverse();
                foreach (StiAxisLabelInfoXF info in infos)
                {
                    if (labelsIndex == 0)
                    {
                        infos2.Add(info);
                    }

                    labelsIndex++;
                    if (labelsIndex == labelsStep) labelsIndex = 0;
                }
                return infos2;
            }

            return infos;
        }

        public RectangleF GetCenterAxisRect(StiContext context, RectangleF rect, bool includeAxisArrow, bool includeLabelsHeight, bool isDrawing)
        {
            if (this.Axis.Area.XAxis.Info.Minimum >= 0)
                return RectangleF.Empty;

            float posX = - GetTicksMaxLength(context);
            return new RectangleF(posX, 0, GetTicksMaxLength(context), rect.Height);
        }

        public virtual RectangleF GetAxisRect(StiContext context, RectangleF rect, 
            bool includeAxisArrow, bool includeLabelsHeight, bool isDrawing, bool includeScrollBar)
        {
            var axisRect = RectangleF.Empty;
            if (!Axis.Visible) return axisRect;

            var infos = MeasureStripLines(context, rect);

            if (infos.Count == 0)
            {
                if (IsLeftSide)
                    axisRect = new RectangleF(-GetTicksMaxLength(context), 0, GetTicksMaxLength(context), rect.Height);
                else
                    axisRect = new RectangleF(rect.Width, 0, GetTicksMaxLength(context), rect.Height);
            }
            else
            {
                #region Calculate total labels area
                foreach (StiAxisLabelInfoXF info in infos)
                {
                    if (info.ClientRectangle.IsEmpty) continue;
                    if (axisRect.IsEmpty)
                        axisRect = info.ClientRectangle;
                    else
                        axisRect = RectangleF.Union(axisRect, info.ClientRectangle);
                }

                if (Axis.Interaction.ShowScrollBar && includeScrollBar)
                    axisRect.Width += StiAxisCoreXF.DefaultScrollBarSize * context.Options.Zoom;

                //Add size of Ticks and size of space between labels and ticks
                axisRect.Width += GetTicksMaxLength(context) + GetLabelsSpaceAxis(context);
                #endregion

                var axisRect2 = axisRect;

                if (IsLeftSide)
                    axisRect = new RectangleF(-axisRect.Width, 0, axisRect.Width, rect.Height);

                if (IsRightSide)
                    axisRect = new RectangleF(rect.Width, 0, axisRect.Width, rect.Height);

                if (includeLabelsHeight)
                {
                    axisRect.Y = axisRect2.Y;
                    axisRect.Height = axisRect2.Height;
                }
            }

            #region Add Place for Arrow
            if (Axis.ArrowStyle != StiArrowStyle.None && includeAxisArrow)
            {
                //if (!Area.ReverseVert) axisRect.Y -= GetArrowHeight(context);
                //axisRect.Height += GetArrowHeight(context);

                float arrowHeight = GetArrowHeight(context);
                if (Axis.Area.ReverseVert)
                    arrowHeight = -arrowHeight;

                axisRect = RectangleF.Union(axisRect, new RectangleF(axisRect.X, -arrowHeight, 1, arrowHeight));
            }
            #endregion

            #region Title
            SizeF titleSize = GetAxisTitleSize(context);
            if (!titleSize.IsEmpty && this.Axis.Title.Position == StiTitlePosition.Outside)
            {
                axisRect.Width += titleSize.Width;
                if (IsLeftSide)
                    axisRect.X -= titleSize.Width;
            }
            #endregion

            return axisRect;
        }

        private void RenderLabels(StiContext context, RectangleF rect, StiYAxisGeom geom)
        {
            if (Axis.Labels.Placement != StiLabelsPlacement.None)
            {
                var infos = MeasureStripLines(context, rect);

                geom.CreateChildGeoms();

                foreach (StiAxisLabelInfoXF info in infos)
                {
                    if (!info.ClientRectangle.IsEmpty)
                    {
                        var labelGeom = new StiAxisLabelGeom(this.Axis,
                            info.ClientRectangle, info.TextPoint, info.Text, info.StripLine, info.Angle, Axis.Labels.Width, info.RotationMode, Axis.Labels.WordWrap);
                        geom.ChildGeoms.Add(labelGeom);
                    }
                }
            }
        }

        private void RenderTitle(StiContext context, RectangleF axisRect, StiYAxisGeom geom)
        {
            if (string.IsNullOrEmpty(this.Axis.Title.Text))
                return;
            
            var titleSize = GetAxisTitleSize(context);
            var titleRect = RectangleF.Empty;

            if (IsLeftSide)
            {
                switch (Axis.Title.Alignment)
                {
                    case StringAlignment.Near:
                        titleRect = new RectangleF(0, axisRect.Height - titleSize.Height + axisRect.Y, titleSize.Width, titleSize.Height);
                        break;

                    case StringAlignment.Center:
                        titleRect = new RectangleF(0, (axisRect.Height - titleSize.Height) / 2 + axisRect.Y, titleSize.Width, titleSize.Height);
                        break;

                    case StringAlignment.Far:
                        titleRect = new RectangleF(0, axisRect.Y, titleSize.Width, titleSize.Height);
                        break;
                }

                if (this.Axis.Title.Position == StiTitlePosition.Inside)
                    titleRect.X += axisRect.Width;
            }

            if (IsRightSide)
            {
                switch (Axis.Title.Alignment)
                {
                    case StringAlignment.Near:
                        titleRect = new RectangleF(axisRect.Width - titleSize.Width, axisRect.Height - titleSize.Height + axisRect.Y, titleSize.Width, titleSize.Height);
                        break;

                    case StringAlignment.Center:
                        titleRect = new RectangleF(axisRect.Width - titleSize.Width, (axisRect.Height - titleSize.Height) / 2 + axisRect.Y, titleSize.Width, titleSize.Height);
                        break;

                    case StringAlignment.Far:
                        titleRect = new RectangleF(axisRect.Width - titleSize.Width, axisRect.Y, titleSize.Width, titleSize.Height);
                        break;
                }

                if (this.Axis.Title.Position == StiTitlePosition.Inside)
                    titleRect.X -= axisRect.Width;
            }
            
            var fontSize = Axis.Title.Font.Size * context.Options.Zoom;
            var maxWidth = 0f;
            if (((StiChart)this.Axis.Area.Chart).IsDashboard && CheckUseMaxWidth(axisRect, titleRect, out maxWidth))
                fontSize = GetCorrectionFontSize(axisRect, titleRect, fontSize);

            var font = StiFontGeom.ChangeFontSize(Axis.Title.Font, fontSize);
            var angle = GetAngleTitle();

            var titleGeom = new StiAxisTitleGeom(this.Axis, titleRect, angle, font);
            geom.CreateChildGeoms();
            geom.ChildGeoms.Add(titleGeom);
        }        
        #endregion

        #region Properties
        [Browsable(false)]
        public abstract StiYAxisDock Dock
        {
            get;
        }

        [Browsable(false)]
        public bool IsLeftSide
        {
            get
            {
                return
                    (Dock == StiYAxisDock.Left && (!Axis.Area.ReverseHor)) ||
                    (Dock == StiYAxisDock.Right && Axis.Area.ReverseHor);
            }
        }

        [Browsable(false)]
        public bool IsRightSide
        {
            get
            {
                return
                    (Dock == StiYAxisDock.Right && (!Axis.Area.ReverseHor)) ||
                    (Dock == StiYAxisDock.Left && Axis.Area.ReverseHor);
            }
        }
        #endregion

        public StiYAxisCoreXF(IStiAxis axis)
            : base(axis)
        {
        }

	}
}
