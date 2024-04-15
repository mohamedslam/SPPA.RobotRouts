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
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.Painters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    #region StiErrorFormatEntity
    internal struct StiErrorFormatEntity
    {
        #region Properties
        public static HashSet<StiErrorFormatEntity> Hash { get; private set; } = new HashSet<StiErrorFormatEntity>();

        public string Format { get; set; }

        public object Value { get; set; }
        #endregion
    }
    #endregion

    public abstract class StiXAxisCoreXF : StiAxisCoreXF
    {
        #region Methods
        public override bool GetStartFromZero()
        {
            if (this.Axis.Info.LabelsCollection != null && IsArgumentDateTime(this.Axis.Info.LabelsCollection) &&
                ((this.Axis.Area is StiScatterArea) || (this.Axis.Area is StiGanttArea)))
                return false;

            return base.GetStartFromZero();
        }

        public override StiCellGeom Render(StiContext context, RectangleF rect)
        {
            if (!this.Axis.Visible)
                return null;

            var axisRect = GetAxisRect(context, rect, false, false, true, false);
            axisRect.X = 0;
            axisRect.Y = 0;

            if (Axis.Interaction.ShowScrollBar &&
                ((Axis is IStiXBottomAxis && !Axis.Area.ReverseVert) || (Axis is IStiXTopAxis && Axis.Area.ReverseVert)))
                axisRect.Y += StiAxisCoreXF.DefaultScrollBarSize * context.Options.Zoom;

            var geom = new StiXAxisGeom(this.Axis as IStiXAxis, axisRect, false);

            RenderLabels(context, axisRect, geom);
            RenderTitle(context, axisRect, geom);

            return geom;
        }

        public override StiCellGeom RenderView(StiContext context, RectangleF rect)
        {
            if (!this.Axis.Visible)
                return null;

            RectangleF axisRect = GetAxisRect(context, rect, false, false, true, true);

            return new StiXAxisViewGeom(this.Axis as IStiXAxis, axisRect, false);
        }

        public void RenderScrollBar(StiContext context, RectangleF axisRect, StiXAxisViewGeom axisGeom)
        {
            if (this.Axis.Interaction.ShowScrollBar)
            {
                RectangleF scrollBarRect = axisRect;
                scrollBarRect.X = 0;

                scrollBarRect.Height = StiAxisCoreXF.DefaultScrollBarSize * context.Options.Zoom;

                if ((this.Axis is IStiXTopAxis && (!this.Axis.Area.ReverseVert)) ||
                    (this.Axis is IStiXBottomAxis && (this.Axis.Area.ReverseVert)))
                    scrollBarRect.Y = axisRect.Height - scrollBarRect.Height;
                else
                    scrollBarRect.Y = 0;

                StiHorzScrollBarGeom scrollBarGeom = new StiHorzScrollBarGeom(this.Axis as IStiXAxis, scrollBarRect);
                axisGeom.CreateChildGeoms();
                axisGeom.ChildGeoms.Add(scrollBarGeom);

                scrollBarRect.X = 0;
                scrollBarRect.Y = 0;
                scrollBarRect.Inflate(-2, -2);

                if (Axis.Interaction.RangeScrollEnabled)
                {
                    #region Left Button
                    RectangleF leftButtonRect = scrollBarRect;
                    leftButtonRect.Width = scrollBarRect.Height;
                    StiLeftButtonGeom leftButtonGeom = new StiLeftButtonGeom(this.Axis as IStiXAxis, leftButtonRect);

                    scrollBarGeom.CreateChildGeoms();
                    scrollBarGeom.ChildGeoms.Add(leftButtonGeom);
                    #endregion

                    #region Right Button
                    RectangleF rightButtonRect = scrollBarRect;
                    rightButtonRect.Width = scrollBarRect.Height;
                    rightButtonRect.X = scrollBarRect.Right - rightButtonRect.Width;
                    StiRightButtonGeom rightButtonGeom = new StiRightButtonGeom(this.Axis as IStiXAxis, rightButtonRect);

                    scrollBarGeom.CreateChildGeoms();
                    scrollBarGeom.ChildGeoms.Add(rightButtonGeom);
                    #endregion

                    scrollBarRect.Inflate(-scrollBarRect.Height - 2, 0);
                }

                #region Track Bar
                RectangleF trackBarRect = scrollBarRect;
                IStiXAxis rollAxis = Axis.Area.XAxis;
                StiAxisAreaCoreXF axisCore = Axis.Area.Core as StiAxisAreaCoreXF;

                if (!rollAxis.Range.Auto)
                {
                    float dpi = (float)(scrollBarRect.Width / axisCore.ScrollRangeX);
                    float x = (float)axisCore.ScrollValueX * dpi;
                    float width = axisCore.ScrollViewX < axisCore.ScrollRangeX?
                        (float)axisCore.ScrollViewX * dpi : (float)axisCore.ScrollRangeX * dpi;

                    trackBarRect = new RectangleF(x + scrollBarRect.X, scrollBarRect.Y, width, scrollBarRect.Height);
                }

                StiHorzTrackBarGeom trackBarGeom = new StiHorzTrackBarGeom(this.Axis as IStiXAxis, trackBarRect, scrollBarGeom);
                scrollBarGeom.CreateChildGeoms();
                scrollBarGeom.ChildGeoms.Add(trackBarGeom);
                #endregion
            }
        }

        public StiCellGeom RenderCenter(StiContext context, RectangleF rect)
        {
            if (!this.Axis.Visible)
                return null;

            if (this.Axis.Area.YAxis.Info.Minimum >= 0)
                return null;

            RectangleF axisRect = GetCenterAxisRect(context, rect, false, false, true);
            axisRect.X = 0;
            axisRect.Y = 0;
            return new StiXAxisGeom(this.Axis as IStiXAxis, axisRect, true);
        }

        public StiCellGeom RenderCenterView(StiContext context, RectangleF rect)
        {
            if (!this.Axis.Visible)
                return null;

            if (this.Axis.Area.YAxis.Info.Minimum >= 0)
                return null;

            var axisRect = GetCenterAxisRect(context, rect, false, false, true);
            var axisGeom = new StiXAxisViewGeom(this.Axis as IStiXAxis, axisRect, true);

            return axisGeom;
        }

        internal string GetLabelText(StiStripLineXF line, IStiSeries series)
        {
            return GetLabelText(line.ValueObject, line.Value, series);
        }

        internal string GetLabelText(object objectValue, double value, IStiSeries series)
        {
            CultureInfo storedCulture = null;

            try
            {
                #region DBS
                if (Axis.Labels.FormatService != null && !(Axis.Labels.FormatService is StiGeneralFormatService))
                {
                    if (objectValue is DateTime)
                    {
                        if (Axis.Labels.FormatService is StiDateFormatService || Axis.Labels.FormatService is StiTimeFormatService)
                            return $"{Axis.Labels.TextBefore}{Axis.Labels.FormatService.Format(objectValue)}{Axis.Labels.TextAfter}";
                        else
                            return $"{Axis.Labels.TextBefore}{objectValue}{Axis.Labels.TextAfter}";
                    }

                    return $"{Axis.Labels.TextBefore}{Axis.Labels.FormatService.Format(value)}{Axis.Labels.TextAfter}";
                }
                #endregion

                var culture = ((StiChart)this.Axis.Area.Chart)?.Report?.GetParsedCulture();
                if (!string.IsNullOrEmpty(culture))
                {
                    storedCulture = Thread.CurrentThread.CurrentCulture;
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                }

                string format = null;
                if (series != null) format = series.Format;
                if (format == null || format.Trim().Length == 0) format = this.Axis.Labels.Format;

                if (format != null && format.Trim().Length != 0)
                {
                    #region If value is string try to convert it to decimal value
                    if (objectValue is string)
                    {
                        string strValue = objectValue.ToString().Replace(".", ",").Replace(",", Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                        decimal result;
                        if (decimal.TryParse(strValue, out result))
                        {
                            objectValue = result;
                        }
                        else
                        {
                            DateTime resultDateTime;
                            var cultureName = string.IsNullOrEmpty(culture) ? "en-US" : culture;
                            if (DateTime.TryParse(objectValue.ToString(), new CultureInfo(cultureName, true), DateTimeStyles.None, out resultDateTime))
                            {
                                objectValue = resultDateTime;
                            }
                            else
                            {
                                if (DateTime.TryParse(objectValue.ToString(), out resultDateTime))
                                {
                                    objectValue = resultDateTime;
                                }
                            }
                        }
                    }
                    #endregion

                    else if (objectValue == null)
                        return string.Empty;

                    if (!StiChartOptions.OldChartPercentMode && format.StartsWith("P", StringComparison.InvariantCulture))
                    {
                        int signs = 0;
                        if (format.Length > 1)
                        {
                            int.TryParse(format.Remove(0, 1), out signs);
                        }

                        return string.Format("{0}{1:N" + signs.ToString() + "}{2}{3}", this.Axis.Labels.TextBefore, objectValue, "%", this.Axis.Labels.TextAfter);
                    }
                    else
                    {
                        var errorEntity = new StiErrorFormatEntity()
                        {
                            Format = format,
                            Value = objectValue
                        };

                        try
                        {
                            if (!StiErrorFormatEntity.Hash.Contains(errorEntity))
                                return string.Format("{0}{1:" + format + "}{2}", this.Axis.Labels.TextBefore, objectValue, this.Axis.Labels.TextAfter);
                        }
                        catch
                        {
                            StiErrorFormatEntity.Hash.Add(errorEntity);
                        }
                    }
                }
                return string.Format("{0}{1}{2}", this.Axis.Labels.TextBefore, objectValue, this.Axis.Labels.TextAfter);
            }
            catch
            {
            }
            finally
            {
                if (storedCulture != null)
                    Thread.CurrentThread.CurrentCulture = storedCulture;
            }
            return objectValue.ToString();
        }

        [Browsable(false)]
        private bool IsLabelsAngleByWidth
        {
            get
            {
                return Axis.Labels.Angle < 30 || Axis.Labels.Angle > 150;
            }
        }

        private List<StiAxisLabelInfoXF> MeasureAxisLabelInfos(RectangleF rect, int start, int end, StiContext context)
        {
            var infos1 = new List<StiAxisLabelInfoXF>();

            var contextClone = context.ContextPainter.Clone();

            var sf = GetStringFormatGeom(context, this.Axis.Labels.WordWrap);
            var font = GetFontGeom(context);

            #region Choose Rotation Mode, Angle and Point
            StiRotationMode rotationMode = StiRotationMode.CenterCenter;
            float angle = -this.Axis.Labels.Angle;
            PointF point = PointF.Empty;

            if (IsTopSide)
            {
                rotationMode = StiRotationMode.CenterBottom;
            }
            else
            {
                rotationMode = StiRotationMode.CenterTop;
            }
            #endregion

            bool sideFlag = this.Axis.Area.ReverseVert ? this.IsBottomSide : this.IsTopSide;

            for (int index = start; index < end && index < this.Axis.Info.LabelsCollection.Count; index++)
            {
                #region Remove First Label on YRightAxis if YRightAxis have arrow
                if (sideFlag && this.Axis.Area.YRightAxis.ArrowStyle != StiArrowStyle.None && index == 0)
                {
                    infos1.Add(new StiAxisLabelInfoXF());
                    index++;
                    continue;
                }
                #endregion

                #region Remove Last Label on YAxis if YAxis have arrow
                if (sideFlag && this.Axis.Area.YAxis.ArrowStyle != StiArrowStyle.None && index == this.Axis.Info.StripLines.Count - 1)
                {
                    infos1.Add(new StiAxisLabelInfoXF());
                    index++;
                    continue;
                }
                #endregion

                var label = this.Axis.Info.LabelsCollection[index];
                string text = GetLabelText(label.StripLine, null);

                #region Init Start X Position
                float posX = label.Position;
                #endregion

                if (IsTopSide)
                    point = new PointF(posX, rect.Height - (GetTicksMaxLength(context) + GetLabelsSpaceAxis(context)));
                else
                    point = new PointF(posX, (GetTicksMaxLength(context) + GetLabelsSpaceAxis(context)));

                if (this.Axis.Labels.Placement != StiLabelsPlacement.None)
                {
                    var textRectF = contextClone.MeasureRotatedString(text, font, point, sf,
                        rotationMode, angle, (int)(this.Axis.Labels.Width * context.Options.Zoom));

                    #region AxisLabelInfo
                    var info = new StiAxisLabelInfoXF
                    {
                        Angle = angle,
                        ClientRectangle = Rectangle.Round(textRectF),
                        RotationMode = rotationMode,
                        TextPoint = point,
                        Text = text,
                        StripLine = label.StripLine
                    };
                    infos1.Add(info);
                    #endregion
                }
                else
                {
                    infos1.Add(new StiAxisLabelInfoXF());
                }
            }

            contextClone.Dispose();

            return infos1;
        }

        private bool CheckAutoAngleLabels(StiContext context, RectangleF rect)
        {
            if (Axis.Labels.Placement != StiLabelsPlacement.AutoRotation) return false;

            var infos = new List<StiAxisLabelInfoXF>();

            if (this.Axis.Info.LabelsCollection != null && this.Axis.Labels.Placement != StiLabelsPlacement.None)
            {
                if (this.Axis.Info.LabelsCollection.Count < 400)
                {
                    infos = MeasureAxisLabelInfos(rect, 0, this.Axis.Info.LabelsCollection.Count, context);
                }
                else
                {
                    #region Use tasks
                    Func<object, object[]> action = (object obj) =>
                    {
                        object[] objs = (object[])obj;
                        int start = (int)objs[0];
                        int end = (int)objs[1];
                        var context1 = (StiContext)objs[2];

                        var infos1 = MeasureAxisLabelInfos(rect, start, end, context1);

                        return new object[] { infos1 };
                    };

                    var tasks = new List<Task<object[]>>();
                    var count = 5;

                    int step = (int)(this.Axis.Info.LabelsCollection.Count / count);

                    for (int indexTask = 0; indexTask < count; indexTask++)
                    {
                        int endIndexTask = (indexTask + 1) * step;

                        if (indexTask == count - 1)
                            endIndexTask = this.Axis.Info.LabelsCollection.Count;

                        object[] param1 = new object[]
                        {
                            indexTask*step, endIndexTask, context
                        };

                        tasks.Add(Task<object[]>.Factory.StartNew(action, param1));
                    }

                    Task.WaitAll(tasks.ToArray());
                    foreach (var task in tasks)
                    {
                        infos.AddRange((List<StiAxisLabelInfoXF>)task.Result[0]);
                    }
                    #endregion
                }

                for (var index = 1; index < infos.Count; index++)
                {
                    var rect1 = infos[index - 1].ClientRectangle;
                    var rect2 = infos[index].ClientRectangle;
                    
                    var rectIntersect = RectangleF.Intersect(rect1, rect2);
                    if (rectIntersect.Width > 0)
                        return true;
                }
            }
            return false;
        }



        private List<StiAxisLabelInfoXF> MeasureAxisLabelInfos(RectangleF rect, float angle, int start, int end, StiContext context, float labelsWidth, StiRotationMode? rotation, bool wordWrap, out float maxLabelWidth)
        {
            var infos1 = new List<StiAxisLabelInfoXF>();
            maxLabelWidth = 0;

            var contextClone = context.ContextPainter.Clone();

            var sf = GetStringFormatGeom(context, wordWrap);
            var font = GetFontGeom(context);
            var alignment = GetTextAlignment();

            #region Choose Rotation Mode, Angle and Point
            var rotationMode = StiRotationMode.CenterCenter;

            var point = PointF.Empty;

            if (rotation != null)
            {
                rotationMode = rotation.GetValueOrDefault();
            }
            else
            {
                if (IsTopSide)
                {
                    if (angle == 0)
                        rotationMode = StiRotationMode.CenterBottom;
                    else
                    {
                        if (this.Axis.Labels.TextAlignment == StiHorAlignment.Center)
                            rotationMode = StiRotationMode.CenterCenter;
                        else
                            rotationMode = StiRotationMode.LeftCenter;
                    }
                }
                else
                {
                    if (angle == 0)
                        rotationMode = StiRotationMode.CenterTop;
                    else
                    {
                        if (this.Axis.Labels.TextAlignment == StiHorAlignment.Center)
                            rotationMode = StiRotationMode.CenterCenter;
                        else
                            rotationMode = StiRotationMode.RightCenter;
                    }
                }
            }
            #endregion

            bool sideFlag = this.Axis.Area.ReverseVert ? this.IsBottomSide : this.IsTopSide;

            for (int index = start; index < end && index < this.Axis.Info.LabelsCollection.Count; index++)
            {
                #region Remove First Label on YRightAxis if YRightAxis have arrow
                if (sideFlag && this.Axis.Area.YRightAxis.ArrowStyle != StiArrowStyle.None && index == 0)
                {
                    infos1.Add(new StiAxisLabelInfoXF());
                    index++;
                    continue;
                }
                #endregion

                #region Remove Last Label on YAxis if YAxis have arrow
                if (sideFlag && this.Axis.Area.YAxis.ArrowStyle != StiArrowStyle.None && index == this.Axis.Info.StripLines.Count - 1)
                {
                    infos1.Add(new StiAxisLabelInfoXF());
                    index++;
                    continue;
                }
                #endregion

                var label = this.Axis.Info.LabelsCollection[index];
                string text = GetLabelText(label.StripLine, null);
                if (!wordWrap && labelsWidth > 0)
                    text = StiTextContentHelper.GetMeasureText(context, text, this.Axis.Labels.Font, labelsWidth);

                #region Init Start X Position
                float posX = label.Position;
                #endregion

                if (IsTopSide)
                    point = new PointF(posX, rect.Height - (GetTicksMaxLength(context) + GetLabelsSpaceAxis(context)));
                else
                    point = new PointF(posX, (GetTicksMaxLength(context) + GetLabelsSpaceAxis(context)));

                if (this.Axis.Labels.Placement != StiLabelsPlacement.None)
                {
                    #region Labels
                    #region Placement Two Lines
                    if (this.Axis.Labels.Placement == StiLabelsPlacement.TwoLines && ((index & 1) != 0))
                    {
                        if (IsTopSide) point.Y -= GetLabelsTwoLinesDestination(context);
                        if (IsBottomSide) point.Y += GetLabelsTwoLinesDestination(context);
                    }
                    #endregion

                    var textRectF = contextClone.MeasureRotatedString(text, font, point, sf,
                        rotationMode, angle, (int)(labelsWidth * context.Options.Zoom));

                    var textPoint = PointF.Empty;
                    if (rotationMode == StiRotationMode.CenterCenter)
                    {
                        point.Y = IsTopSide ? point.Y - textRectF.Height / 2 : point.Y + textRectF.Height / 2;
                    }

                    #region AxisLabelInfo
                    var info = new StiAxisLabelInfoXF
                    {
                        Angle = angle,
                        Width = labelsWidth,
                        WordWrap = wordWrap,
                        ClientRectangle = Rectangle.Round(textRectF),
                        RotationMode = rotationMode,
                        TextPoint = point,
                        Text = text,
                        StripLine = label.StripLine
                    };

                    infos1.Add(info);
                    #endregion

                    if (IsLabelsAngleByWidth)
                    {
                        maxLabelWidth = Math.Max(maxLabelWidth, textRectF.Width);
                    }
                    #endregion
                }
                else
                {
                    infos1.Add(new StiAxisLabelInfoXF());
                }
            }

            contextClone.Dispose();

            return infos1;
        }

        private float GetMaxHeightLabels(List<StiAxisLabelInfoXF> infos)
        {
            var axisRect = RectangleF.Empty;
            foreach (var info in infos)
            {
                if (info.ClientRectangle.IsEmpty) continue;
                if (axisRect.IsEmpty)
                    axisRect = info.ClientRectangle;
                else
                    axisRect = RectangleF.Union(axisRect, info.ClientRectangle);
            }

            return axisRect.Height;
        }

        private List<StiAxisLabelInfoXF> MeasureStripLines(StiContext context, RectangleF rect)
        {
            var labelsWidth = this.Axis.Labels.Width;
            StiRotationMode? rotationMode = null;
            bool wordWrap = this.Axis.Labels.WordWrap;

            #region Auto Rotation Mode
            var angle = -this.Axis.Labels.Angle;
            var isAutoAngleLabels = CheckAutoAngleLabels(context, rect);
            if (isAutoAngleLabels)
            {
                angle = -45;
                rotationMode = this.Axis is IStiXTopAxis? StiRotationMode.LeftBottom : StiRotationMode.RightTop;
                wordWrap = false;
                var rootHeight = ((StiChart)this.Axis.Area.Chart).Core.FullRectangle.Height;
                labelsWidth = rootHeight / 4;
            }
            #endregion

            var infos = new List<StiAxisLabelInfoXF>();

            if (this.Axis.Info.LabelsCollection != null && this.Axis.Labels.Placement != StiLabelsPlacement.None)
            {
                float maxLabelWidth = 0;

                if (this.Axis.Info.LabelsCollection.Count < 400)
                {
                    infos = MeasureAxisLabelInfos(rect, angle, 0, this.Axis.Info.LabelsCollection.Count, context, labelsWidth, rotationMode, wordWrap, out maxLabelWidth);
                }
                else
                {
                    #region Using tasks
                    var tasks = new List<Task<object[]>>();
                    Func<object, object[]> action = (object obj) =>
                    {
                        object[] objs = (object[])obj;
                        int start = (int)objs[0];
                        int end = (int)objs[1];
                        var context1 = (StiContext)objs[2];
                        var maxLabelWidth1 = 0f; 
                        var infos1 = MeasureAxisLabelInfos(rect, angle, start, end, context1, labelsWidth, rotationMode, wordWrap, out maxLabelWidth1);

                        return new object[] { infos1, maxLabelWidth };
                    };

                    var count = 5;

                    int step = (int)(this.Axis.Info.LabelsCollection.Count / count);

                    for (int indexTask = 0; indexTask < count; indexTask++)
                    {
                        int endIndexTask = (indexTask + 1) * step;

                        if (indexTask == count - 1)
                            endIndexTask = this.Axis.Info.LabelsCollection.Count;

                        object[] param1 = new object[]
                        {
                            indexTask *step, endIndexTask, context
                        };

                        tasks.Add(Task<object[]>.Factory.StartNew(action, param1));
                    }

                    Task.WaitAll(tasks.ToArray());
                    foreach (var task in tasks)
                    {
                        infos.AddRange((List<StiAxisLabelInfoXF>)task.Result[0]);

                        maxLabelWidth = Math.Max(maxLabelWidth, (float)task.Result[1]);
                    }
                    #endregion
                }

                #region Calculate Label Step
                int labelsStep = 1;
                if (this.Axis.Range.Auto)
                {
                    if (isAutoAngleLabels)
                    {
                        #region DBS
                        maxLabelWidth = Axis.Labels.Font.Height * 1.2f * context.Options.Zoom;
                        float widthPerLabel = rect.Width / Axis.Info.StripLines.Count;
                        labelsStep = (int)Math.Ceiling(maxLabelWidth / widthPerLabel);
                        #endregion
                    }
                    else
                    {

                        if (!IsLabelsAngleByWidth) maxLabelWidth = Axis.Labels.Font.Height * 1.5f * context.Options.Zoom;

                        float widthPerLabel = rect.Width / Axis.Info.StripLines.Count;
                        float count = maxLabelWidth / widthPerLabel;

                        if (count > 1)
                        {
                            labelsStep = (int)StiStripLineCalculatorXF.GetInterval(0, this.Axis.Info.LabelsCollection.Count, 6);
                            if (this.Axis.LogarithmicScale)
                                labelsStep = 1;
                        }
                    }
                }
                else labelsStep = (int)Math.Max(1, this.Axis.Step);
                #endregion

                #region Process labels with label step
                bool twoLinesLabels = false;
                float posYTwoLine = 0;
                float posYOneLine = 0;
                if ((isAutoAngleLabels || this.Axis.Labels.Placement == StiLabelsPlacement.TwoLines) && infos.Count > 1)
                {
                    posYOneLine = infos[0].TextPoint.Y;
                    posYTwoLine = infos[1].TextPoint.Y;
                }

                if (labelsStep == 1 || this.Axis.Labels.Step != 0) return infos;
                else
                {
                    int labelsIndex = 0;
                    List<StiAxisLabelInfoXF> infos2 = new List<StiAxisLabelInfoXF>();
                    foreach (StiAxisLabelInfoXF info in infos)
                    {
                        if (labelsIndex == 0)
                        {
                            #region Placement Two Lines
                            if (this.Axis.Labels.Placement == StiLabelsPlacement.TwoLines && infos.Count > 1)
                            {
                                if (twoLinesLabels)
                                {
                                    info.TextPoint.Y = posYTwoLine;
                                    twoLinesLabels = false;
                                }
                                else
                                {
                                    info.TextPoint.Y = posYOneLine;
                                    twoLinesLabels = true;
                                }
                            }
                            #endregion

                            infos2.Add(info);
                        }

                        labelsIndex++;
                        if (labelsIndex == labelsStep) labelsIndex = 0;
                    }
                    return infos2;
                }
                #endregion

            }
            return infos;
        }
        
        public RectangleF GetCenterAxisRect(StiContext context, RectangleF rect, bool includeAxisArrow, bool includeLabelsHeight, bool isDrawing)
        {
            if (this.Axis.Area.YAxis.Info.Minimum >= 0)
                return RectangleF.Empty;

            float posY = -GetTicksMaxLength(context);
            return new RectangleF(0, posY, rect.Width, GetTicksMaxLength(context));
        }

        public RectangleF GetAxisRect(StiContext context, RectangleF rect,
            bool includeAxisArrow, bool includeLabelsWidth, bool isDrawing, bool includeScrollBar)
        {
            var axisRect = RectangleF.Empty;
            if (!this.Axis.Visible) return axisRect;
            
            var infos = MeasureStripLines(context, rect);

            if (infos.Count == 0)
            {
                if (IsTopSide)
                    axisRect = new RectangleF(0, -GetTicksMaxLength(context), rect.Width, GetTicksMaxLength(context));
                else
                    axisRect = new RectangleF(0, rect.Height, rect.Width, GetTicksMaxLength(context));
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

                //Correction if angle label a negative 
                var isGdi = context.ContextPainter is StiGdiContextPainter;
                if (axisRect.Y < 0 && !IsTopSide && this.Axis.Labels.TextAlignment == StiHorAlignment.Right && isGdi)
                {
                    axisRect.Height += axisRect.Y;
                    axisRect.Y = 0;
                }

                if (Axis.Interaction.ShowScrollBar && includeScrollBar)
                    axisRect.Height += StiAxisCoreXF.DefaultScrollBarSize * context.Options.Zoom;


                //Add size of Ticks and size of space between labels and ticks
                axisRect.Height += GetTicksMaxLength(context) + GetLabelsSpaceAxis(context);
                #endregion

                RectangleF axisRect2 = axisRect;

                if (IsTopSide)
                    axisRect = new RectangleF(0, -axisRect.Height, rect.Width, axisRect.Height);

                if (IsBottomSide)
                    axisRect = new RectangleF(0, rect.Height, rect.Width, axisRect.Height);

                if (includeLabelsWidth)
                {
                    axisRect.X = axisRect2.X;
                    axisRect.Width = axisRect2.Width;
                }
            }

            #region Add Place for Arrow
            if (this.Axis.ArrowStyle != StiArrowStyle.None && includeAxisArrow)
            {
                float arrowHeight = GetArrowHeight(context);
                if (this.Axis.Area.ReverseHor)
                    arrowHeight = -arrowHeight;

                axisRect = RectangleF.Union(axisRect, new RectangleF(rect.Width, axisRect.Y + axisRect.Height / 2, arrowHeight, 1));
            }
            #endregion

            #region Title
            SizeF titleSize = GetAxisTitleSize(context);
            if (!titleSize.IsEmpty && this.Axis.Title.Position == StiTitlePosition.Outside)
            {
                axisRect.Height += titleSize.Height;

                if (axisRect.Width == 0)
                    axisRect.Width = titleSize.Width;

                if (IsTopSide)
                    axisRect.Y -= titleSize.Height;
            }
            #endregion

            return axisRect;
        }

        private void RenderLabels(StiContext context, RectangleF rect, StiXAxisGeom geom)
        {
            if (this.Axis.Info.StripLines != null && this.Axis.Labels.Placement != StiLabelsPlacement.None)
            {
                var infos = MeasureStripLines(context, rect);

                geom.CreateChildGeoms();

                var axisX = this.Axis.Area.XAxis as StiXBottomAxis;

                if (IsArgumentDateTime(infos) && axisX != null && axisX.DateTimeStep.Step != StiTimeDateStep.None)
                {

                    DateTime date = new DateTime();
                    DateTime dateNext = new DateTime();

                    var tempLabels = axisX.Info.LabelsCollection;
                    var infosFirstLine = new List<StiAxisLabelInfoXF>();

                    string text = string.Empty;
                    bool first = true;
                    float startX = 0;

                    #region First line
                    for (int index = 0; index < tempLabels.Count; index++)
                    {
                        if (tempLabels[index].StripLine.ValueObject == null ||
                            !(tempLabels[index].StripLine.ValueObject is DateTime))
                            continue;

                        date = (DateTime)tempLabels[index].StripLine.ValueObject;
                        if (index < tempLabels.Count - 2 && !(axisX.Area is StiScatterArea))
                        {
                            dateNext = (DateTime)tempLabels[index + 1].StripLine.ValueObject;
                        }
                        else
                        {
                            dateNext = date;
                        }

                        if (first)
                        {
                            startX = tempLabels[index].Position;
                            first = false;
                        }

                        switch (((StiXBottomAxis)this.Axis.Area.XAxis).DateTimeStep.Step)
                        {
                            case StiTimeDateStep.Day:
                                if (date.ToString("yyyyMMMMdd") != dateNext.ToString("yyyyMMMMdd") || index == tempLabels.Count - 2)
                                    text = date.ToString("dd");
                                break;

                            case StiTimeDateStep.Hour:
                                if (date.ToString("yyyyMMMMddHH") != dateNext.ToString("yyyyMMMMddHH") || index == tempLabels.Count - 2)
                                    text = date.ToString("HH");
                                break;

                            case StiTimeDateStep.Minute:
                                if (date.ToString("yyyyMMMMddHHmm") != dateNext.ToString("yyyyMMMMddHHmm") || index == tempLabels.Count - 2)
                                    text = date.ToString("mm");
                                break;

                            case StiTimeDateStep.Month:
                                if (date.ToString("yyyyMMMM") != dateNext.ToString("yyyyMMMM") || index == tempLabels.Count - 2)
                                    text = StiLocalization.Get("A_WebViewer", "Month" + date.ToString("MMMM", CultureInfo.GetCultureInfo("en-US")));
                                break;

                            case StiTimeDateStep.Second:
                                if (date.ToString("yyyyMMMMddHHmmss") != dateNext.ToString("yyyyMMMMddHHmmss") || index == tempLabels.Count - 2)
                                    text = date.ToString("ss");
                                break;

                            case StiTimeDateStep.Year:
                                if (date.ToString("yyyy") != dateNext.ToString("yyyy") || index == tempLabels.Count - 2)
                                    text = date.ToString("yyyy");
                                break;
                        }

                        if (text != string.Empty)
                        {
                            var point = new PointF((tempLabels[index].Position + startX) / 2, (GetTicksMaxLength(context) + GetLabelsSpaceAxis(context)));
                            var angle = 0f;
                            var info = new StiAxisLabelInfoXF();
                            info.Angle = angle;
                            info.ClientRectangle = context.MeasureRotatedString(text, GetFontGeom(context), point, GetStringFormatGeom(context, info.WordWrap),
                                StiRotationMode.CenterCenter, angle, (int)(this.Axis.Labels.Width * context.Options.Zoom));
                            info.RotationMode = StiRotationMode.CenterCenter;
                            info.StripLine = tempLabels[index].StripLine;
                            info.Text = text;
                            info.TextPoint = point;

                            infosFirstLine.Add(info);

                            first = true;
                            text = string.Empty;
                        }
                    }
                    #endregion

                    #region Second line
                    first = true;
                    var infosSecondLine = new List<StiAxisLabelInfoXF>();

                    for (int index = 0; index < infosFirstLine.Count - 1; index++)
                    {
                        date = (DateTime)infosFirstLine[index].StripLine.ValueObject;
                        dateNext = (DateTime)infosFirstLine[index + 1].StripLine.ValueObject;

                        if (first)
                        {
                            startX = infosFirstLine[index].TextPoint.X;
                            first = false;
                        }

                        switch (((StiXBottomAxis)this.Axis.Area.XAxis).DateTimeStep.Step)
                        {
                            case StiTimeDateStep.Day:
                                if (date.ToString("yyyyMMMM") != dateNext.ToString("yyyyMMMM") || index == infosFirstLine.Count - 2)
                                    text = StiLocalization.Get("A_WebViewer", "Month" + date.ToString("MMMM", CultureInfo.GetCultureInfo("en-US")));
                                break;

                            case StiTimeDateStep.Hour:
                                if (date.ToString("yyyyMMMMdd") != dateNext.ToString("yyyyMMMMdd") || index == infosFirstLine.Count - 2)
                                    text = date.ToString("dd");
                                break;

                            case StiTimeDateStep.Minute:
                                if (date.ToString("yyyyMMMMddhh") != dateNext.ToString("yyyyMMMMddhh") || index == infosFirstLine.Count - 2)
                                    text = date.ToString("hh");
                                break;

                            case StiTimeDateStep.Month:
                                if (date.ToString("yyyy") != dateNext.ToString("yyyy") || index == infosFirstLine.Count - 2)
                                    text = date.ToString("yyyy");
                                break;

                            case StiTimeDateStep.Second:
                                if (date.ToString("yyyyMMMMddhhmm") != dateNext.ToString("yyyyMMMMddhhmm") || index == infosFirstLine.Count - 2)
                                    text = date.ToString("mm");
                                break;

                            case StiTimeDateStep.Year:
                                if (date.ToString("yyyy") != dateNext.ToString("yyyy") || index == infosFirstLine.Count - 2)
                                    continue;
                                break;
                        }

                        if (text != string.Empty)
                        {
                            var point = new PointF((infosFirstLine[index + 1].TextPoint.X + startX) / 2, (GetTicksMaxLength(context) + 2 * GetLabelsSpaceAxis(context)));
                            var angle = 0f;
                            var info = new StiAxisLabelInfoXF();
                            info.Angle = angle;
                            info.ClientRectangle = context.MeasureRotatedString(text, GetFontGeom(context), point, GetStringFormatGeom(context, info.WordWrap),
                                StiRotationMode.CenterCenter, angle, (int)(this.Axis.Labels.Width * context.Options.Zoom));
                            info.RotationMode = StiRotationMode.CenterTop;
                            info.StripLine = infosFirstLine[index].StripLine;
                            info.Text = text;
                            info.TextPoint = point;

                            infosSecondLine.Add(info);

                            first = true;
                            text = string.Empty;
                        }
                    }
                    #endregion

                    infosFirstLine.AddRange(infosSecondLine);

                    foreach (StiAxisLabelInfoXF info in infosFirstLine)
                    {
                        if (!info.ClientRectangle.IsEmpty)
                        {
                            var labelGeom = new StiAxisLabelGeom(this.Axis,
                                info.ClientRectangle, info.TextPoint, info.Text, info.StripLine, info.Angle, info.Width, info.RotationMode, info.WordWrap);
                            geom.ChildGeoms.Add(labelGeom);
                        }
                    }

                    this.Axis.Info.LabelInfoCollection = infosFirstLine;
                }
                else
                {
                    foreach (StiAxisLabelInfoXF info in infos)
                    {
                        if (!info.ClientRectangle.IsEmpty)
                        {
                            var labelGeom = new StiAxisLabelGeom(this.Axis,
                                info.ClientRectangle, info.TextPoint, info.Text, info.StripLine, info.Angle, info.Width, info.RotationMode, info.WordWrap);
                            geom.ChildGeoms.Add(labelGeom);
                        }
                    }

                    this.Axis.Info.LabelInfoCollection = infos;
                }
            }
        }

        private void RenderTitle(StiContext context, RectangleF axisRect, StiXAxisGeom geom)
        {
            if (string.IsNullOrEmpty(this.Axis.Title.Text))
                return;

            var titleSize = GetAxisTitleSize(context);

            var titleRect = RectangleF.Empty;

            if (IsTopSide)
            {
                switch (this.Axis.Title.Alignment)
                {
                    case StringAlignment.Near:
                        titleRect = new RectangleF(0, 0, titleSize.Width, titleSize.Height);
                        break;

                    case StringAlignment.Center:
                        titleRect = new RectangleF((axisRect.Width - titleSize.Width) / 2, 0, titleSize.Width, titleSize.Height);
                        break;

                    case StringAlignment.Far:
                        titleRect = new RectangleF(axisRect.Width - titleSize.Width, 0, titleSize.Width, titleSize.Height);
                        break;
                }

                if (this.Axis.Title.Position == StiTitlePosition.Inside)
                    titleRect.Y += axisRect.Height;
            }

            if (IsBottomSide)
            {
                switch (this.Axis.Title.Alignment)
                {
                    case StringAlignment.Near:
                        titleRect = new RectangleF(0, axisRect.Height - titleSize.Height, titleSize.Width, titleSize.Height);
                        break;

                    case StringAlignment.Center:
                        titleRect = new RectangleF((axisRect.Width - titleSize.Width) / 2, axisRect.Height - titleSize.Height, titleSize.Width, titleSize.Height);
                        break;

                    case StringAlignment.Far:
                        titleRect = new RectangleF(axisRect.Width - titleSize.Width, axisRect.Height - titleSize.Height, titleSize.Width, titleSize.Height);
                        break;
                }

                if (this.Axis.Title.Position == StiTitlePosition.Inside)
                    titleRect.Y -= axisRect.Height;

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

        private bool IsArgumentDateTime(List<StiStripPositionXF> positions)
        {
            foreach (StiStripPositionXF position in positions)
            {
                if (position.StripLine.ValueObject is DateTime)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsArgumentDateTime(List<StiAxisLabelInfoXF> infos)
        {
            foreach (StiAxisLabelInfoXF info in infos)
            {
                if (info.StripLine != null && info.StripLine.ValueObject is DateTime)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public abstract StiXAxisDock Dock
        {
            get;
        }

        [Browsable(false)]
        public bool IsTopSide
        {
            get
            {
                return
                    (Dock == StiXAxisDock.Top && (!this.Axis.Area.ReverseVert)) ||
                    (Dock == StiXAxisDock.Bottom && this.Axis.Area.ReverseVert);
            }
        }

        [Browsable(false)]
        public bool IsBottomSide
        {
            get
            {
                return
                    (Dock == StiXAxisDock.Bottom && (!this.Axis.Area.ReverseVert)) ||
                    (Dock == StiXAxisDock.Top && this.Axis.Area.ReverseVert);
            }
        }
        #endregion

        public StiXAxisCoreXF(IStiAxis axis)
            : base(axis)
        {
        }
    }
}
