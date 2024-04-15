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
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public abstract class StiAxisCoreXF : 
        ICloneable,
        IStiApplyStyle        
	{
        #region ICloneable
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region IStiGeomInteraction.States
        private bool isMouseOverDecreaseButton = false;
        [Browsable(false)]
        public bool IsMouseOverDecreaseButton
        {
            get
            {
                return isMouseOverDecreaseButton;
            }
            set
            {
                isMouseOverDecreaseButton = value;
            }
        }

        private bool isMouseOverIncreaseButton = false;
        [Browsable(false)]
        public bool IsMouseOverIncreaseButton
        {
            get
            {
                return isMouseOverIncreaseButton;
            }
            set
            {
                isMouseOverIncreaseButton = value;
            }
        }

        private bool isMouseOverTrackBar = false;
        [Browsable(false)]
        public bool IsMouseOverTrackBar
        {
            get
            {
                return isMouseOverTrackBar;
            }
            set
            {
                isMouseOverTrackBar = value;
            }
        }
        #endregion

        #region IStiApplyStyle
        public virtual void ApplyStyle(IStiChartStyle style)
        {
            if (this.Axis.AllowApplyStyle)
            {
                this.Axis.LineColor = style.Core.AxisLineColor;

                this.Axis.Labels.Core.ApplyStyle(style);
                this.Axis.Title.Core.ApplyStyle(style);
            }
        }
        #endregion        

        #region Methods
        public virtual bool GetStartFromZero()
        {
            if (this.Axis.Area.AxisCore.ValuesCount == 1)
                return true;
            if (this.Axis != null && this.Axis.Range != null && (!this.Axis.Range.Auto)) return true;
            return this.Axis != null ? this.Axis.StartFromZero : true;
        }

        public abstract StiCellGeom Render(StiContext context, RectangleF rect);
        
        public abstract StiCellGeom RenderView(StiContext context, RectangleF rect);

        protected internal void CalculateStripPositions(float topPosition, float bottomPosition)
        {
            bottomPosition -= topPosition;
            topPosition = 0;
            if (this.Axis.Info.StripLines == null || this.Axis.Info.StripLines.Count < 2)
            {
                this.Axis.Info.StripPositions = new float[0];
            }
            else
            {
                bool isScatterSeries = false;

                if (this.Axis.Area.Chart.Series.Count > 0 && this.Axis.Area.Chart.Series[0] is StiScatterSeries)
                    isScatterSeries = true;

                if (this.Axis.LogarithmicScale && isScatterSeries)
                {
                    int countStrip = this.Axis.Info.StripLines.Count;
                    this.Axis.Info.StripPositions = new float[this.Axis.Info.StripLines.Count];
                    this.Axis.Info.StripPositions[0] = topPosition;
                    this.Axis.Info.StripPositions[this.Axis.Info.StripPositions.Length - 1] = bottomPosition;

                    double startValue = this.Axis.Info.StripLines[0].Value;
                    double endValue = this.Axis.Info.StripLines[countStrip - 1].Value;

                    double decadeX = bottomPosition / (Math.Log10(endValue) - Math.Log10(startValue));

                    for (int index = 1; index < this.Axis.Info.StripPositions.Length - 1; index++)
                    {
                        double value = this.Axis.Info.StripLines[index].Value;

                        double x = Math.Abs(Math.Log10(Math.Abs(this.Axis.Info.StripLines[index].Value)) * decadeX
                            - Math.Log10(Math.Abs(this.Axis.Info.StripLines[index - 1].Value)) * decadeX);

                        this.Axis.Info.StripPositions[index] = (float)(topPosition + x);
                        topPosition = this.Axis.Info.StripPositions[index];
                    }
                }
                else
                {
                    this.Axis.Info.StripPositions = new float[this.Axis.Info.StripLines.Count];
                    this.Axis.Info.StripPositions[0] = topPosition;
                    this.Axis.Info.StripPositions[this.Axis.Info.StripPositions.Length - 1] = bottomPosition;

                    for (int index = 1; index < this.Axis.Info.StripPositions.Length - 1; index++)
                    {
                        this.Axis.Info.StripPositions[index] = topPosition + index * this.Axis.Info.Step;
                    }
                }                
            }
        }

        protected internal float GetTicksMaxLength(StiContext context)
        {
            return this.Axis.Core.TicksMaxLength * context.Options.Zoom;
        }

        protected internal float GetArrowHeight(StiContext context)
        {
            return this.Axis.Core.ArrowHeight * 4 * context.Options.Zoom;
        }

        protected internal float GetLabelsSpaceAxis(StiContext context)
        {
            return 5 * context.Options.Zoom;
        }

        protected internal float GetLabelsTwoLinesDestination(StiContext context)
        {
            return this.Axis.Labels.Font.SizeInPoints * 2 * context.Options.Zoom;
        }

        protected internal StiFontGeom GetFontGeom(StiContext context)
        {
            return StiFontGeom.ChangeFontSize(this.Axis.Labels.Font, this.Axis.Labels.Font.Size * context.Options.Zoom); ;
        }

        protected internal StiHorAlignment GetTextAlignment()
        {
            if (this.Axis is IStiYAxis)
            {
                IStiYAxis yAxis = this.Axis as IStiYAxis;
                if (this.Axis.Labels.Placement == StiLabelsPlacement.TwoLines)
                {
                    if (((StiYAxisCoreXF)yAxis.Core).IsLeftSide) return StiHorAlignment.Right;
                    if (((StiYAxisCoreXF)yAxis.Core).IsRightSide) return StiHorAlignment.Left;
                }
            }
            return this.Axis.Labels.TextAlignment;
        }

        protected internal StiStringFormatGeom GetStringFormatGeom(StiContext context, bool wordWrap)
        {
            StiStringFormatGeom sf = context.GetGenericStringFormat();
            sf.Trimming = StringTrimming.None;
            if (!wordWrap)
                sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
            if (this.Axis.Labels.Width > 0)
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

        protected SizeF GetAxisTitleSize(StiContext context)
        {
            if (string.IsNullOrEmpty(this.Axis.Title.Text))
                return Size.Empty;

            StiFontGeom font = StiFontGeom.ChangeFontSize(this.Axis.Title.Font, this.Axis.Title.Font.Size * context.Options.Zoom);
            SizeF sizeF = context.MeasureString(this.Axis.Title.Text, font);

            switch (this.Axis.Title.Direction)
            {
                case StiDirection.LeftToRight:
                case StiDirection.RightToLeft:
                    return sizeF;

                case StiDirection.BottomToTop:
                case StiDirection.TopToBottom:
                    return new SizeF(sizeF.Height, sizeF.Width);
            }
            return Size.Empty;
        }
        
        protected internal float GetAngleTitle()
        {
            float angle = 0f;
            switch (Axis.Title.Direction)
            {
                case StiDirection.LeftToRight:
                    angle = 0f;
                    break;

                case StiDirection.RightToLeft:
                    angle = 180f;
                    break;

                case StiDirection.BottomToTop:
                    angle = -90f;
                    break;

                case StiDirection.TopToBottom:
                    angle = 90f;
                    break;
            }

            return angle;
        }

        protected internal float GetCorrectionFontSize(RectangleF axisRect, RectangleF titleRect, float currentFontSize)
        {
            switch (Axis.Title.Direction)
            {
                case StiDirection.LeftToRight:
                case StiDirection.RightToLeft:
                    if (axisRect.Width < titleRect.Width)
                    {
                        currentFontSize = axisRect.Width / titleRect.Width * currentFontSize;
                    }
                    break;

                case StiDirection.BottomToTop:
                case StiDirection.TopToBottom:
                    if (axisRect.Height < titleRect.Height)
                    {
                        currentFontSize = axisRect.Height / titleRect.Height * currentFontSize;
                    }
                    break;
            }

            return currentFontSize;
        }

        protected internal bool CheckUseMaxWidth(RectangleF axisRect, RectangleF titleRect, out float maxWidth)
        {
            maxWidth = 0;
            switch (Axis.Title.Direction)
            {
                case StiDirection.LeftToRight:
                case StiDirection.RightToLeft:
                    if (axisRect.Width < titleRect.Width)
                    {
                        maxWidth = axisRect.Width;
                        return true;
                    }
                    break;

                case StiDirection.BottomToTop:
                case StiDirection.TopToBottom:
                    if (axisRect.Height < titleRect.Height)
                    {
                        maxWidth = axisRect.Width;
                        return true;
                    }
                    break;
            }

            return false;
        }

        #endregion

        #region Properties
        public static float DefaultScrollBarSize = 12f;

        public static float DefaultScrollBarSmallFactor = 0.01f;

        public static float DefaultScrollBarFirstRecallTime = 0.3f;

        public static float DefaultScrollBarOtherRecallTime = 0.05f;

        internal float TicksMaxLength
        {
            get
            {
                if (Axis.Visible)
                {
                    var mainLenght = Axis.Ticks.Visible
                        ? Axis.Ticks.LengthUnderLabels > 0 ? Axis.Ticks.LengthUnderLabels
                        : Axis.Ticks.Length : 0;

                    var minorLenght = Axis.Ticks.MinorVisible ? Axis.Ticks.MinorLength : 0;

                    return Math.Max(mainLenght, minorLenght);
                }
                else return 0;
            }
        }


        internal float ArrowWidth
        {
            get
            {
                return 3f;
            }
        }


        internal float ArrowHeight
        {
            get
            {
                if (this is StiXAxisCoreXF)
                {
                    if (Axis.Area.ReverseHor) return -5f;
                    else return 5f;
                }
                if (this is StiYAxisCoreXF)
                {
                    if (Axis.Area.ReverseVert) return -5f;
                    else return 5f;
                }
                return 0f;
            }
        }

        private IStiAxis axis;
        public IStiAxis Axis
        {
            get
            {
                return axis;
            }
            set
            {
                axis = value;
            }
        }

        public StiAxisInfoXF Info
        {
            get
            {
                return this.Axis.Info;
            }
            set
            {
                this.Axis.Info = value;
            }
        } 
        #endregion

        public StiAxisCoreXF(IStiAxis axis)
        {
            this.axis = axis;
        }
    }
}
