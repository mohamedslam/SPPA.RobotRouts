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
using Stimulsoft.Report.Components.TextFormats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading;

namespace Stimulsoft.Report.Chart
{
    public class StiYAxisCoreXF3D : StiAxisCoreXF3D
    {
        #region Methods
        public virtual SizeF GetAxisRect(StiContext context, RectangleF rect,
            bool includeAxisArrow, bool includeLabelsHeight, bool isDrawing, bool includeScrollBar)
        {
            var axisRect = SizeF.Empty;
            if (!Axis.Visible) return axisRect;

            var infos = MeasureStripLines(context);

            if (infos.Count == 0)
            {
                axisRect = new SizeF(0, 0);
            }
            else
            {
                #region Calculate total labels area
                foreach (StiAxisLabelInfoXF3D info in infos)
                {
                    if (info.ClientSize.IsEmpty) continue;

                    if (axisRect.IsEmpty)
                        axisRect = info.ClientSize;
                    else
                        axisRect = new SizeF(Math.Max(axisRect.Width, info.ClientSize.Width), Math.Max(axisRect.Height, info.ClientSize.Height));
                }

                /*if (Axis.Interaction.ShowScrollBar && includeScrollBar)
                    axisRect.Width += StiAxisCoreXF.DefaultScrollBarSize * context.Options.Zoom;

                //Add size of Ticks and size of space between labels and ticks
                axisRect.Width += GetTicksMaxLength(context) + GetLabelsSpaceAxis(context);*/
                #endregion

                //var axisRect2 = axisRect;

                //axisRect = new RectangleF(-axisRect.Width, 0, axisRect.Width, rect.Height);

                //if (includeLabelsHeight)
                //{
                //    axisRect.Y = axisRect2.Y;
                //    axisRect.Height = axisRect2.Height;
                //}
            }

            return axisRect;
        }

        private List<StiAxisLabelInfoXF3D> MeasureStripLines(StiContext context, /*RectangleF rect,*/ StiRectangle3D? rectangle3D = null)
        {
            var infos = new List<StiAxisLabelInfoXF3D>();

            if (this.Axis.Info.LabelsCollection != null)
            {
                //var titleSize = GetAxisTitleSize(context);

                int index = 0;
                foreach (StiStripPositionXF label in this.Axis.Info.LabelsCollection)
                {
                    string text = GetLabelText(label.StripLine);

                    #region Init Start Y Position
                    float posY = label.Position;
                    #endregion

                    //If point does not contains in area rectangle then skip this line
                    #region Labels
                    var sf = context.GetGenericStringFormat();
                    var font = StiFontGeom.ChangeFontSize(Axis.Labels.Font, Axis.Labels.Font.Size * context.Options.Zoom);
                    var rotationMode = Base.Drawing.StiRotationMode.RightCenter;

                    StiPoint3D? point = null;
                        //new PointF(rect.Width /*- GetTicksMaxLength(context) - GetLabelsSpaceAxis(context)*/, posY);
                    if (rectangle3D != null)
                    {
                        var tempRectangle3D = rectangle3D.GetValueOrDefault();
                        point = new StiPoint3D(tempRectangle3D.X, posY, tempRectangle3D.Front);
                    }

                    var sizeF = context.MeasureString(text, font);

                    //var textRectF = context.MeasureRotatedString(text, font, point, sf, angle, (int)(Axis.Labels.Width * context.Options.Zoom));

                    /*#region Check Custom Label Width 
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
                    #endregion*/

                    #region AxisLabelInfo
                    var info = new StiAxisLabelInfoXF3D
                    {
                        ClientSize = sizeF,
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

            return infos;        
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

        public override StiCellGeom Render3D(StiContext context, StiRectangle3D rect3D, StiRender3D render)
        {
            if (!this.Axis.Visible)
                return null;

            var geom = new StiYAxisGeom3D(rect3D, render);

            RenderLabels(context, rect3D, geom, render);

            return geom;
        }

        private void RenderLabels(StiContext context, StiRectangle3D rect3D, StiYAxisGeom3D geom, StiRender3D render)
        {
            //if (Axis.Labels.Placement != StiLabelsPlacement.None)
            {
                var infos = MeasureStripLines(context, rect3D);

                #region Measure and Filter
                var rectPrev = RectangleF.Empty;
                var infosFilter = new List<StiAxisLabelInfoXF3D>();

                foreach (StiAxisLabelInfoXF3D info in infos)
                {
                    if (!info.ClientSize.IsEmpty)
                    {
                        var labelGeom = new StiAxisLabelGeom3D(this.Axis,
                            info.ClientSize, info.TextPoint, info.Text, info.StripLine, info.RotationMode, render);

                        var addLabel = false;

                        var currentRect = labelGeom.MeasureCientRect();

                        if (rectPrev == RectangleF.Empty)
                        {
                            rectPrev = currentRect;
                            addLabel = true;
                        }
                        else
                        {
                            addLabel = !rectPrev.IntersectsWith(currentRect);
                        }

                        if (addLabel)
                        {
                            infosFilter.Add(info);
                            rectPrev = currentRect;
                        }
                    }
                } 
                #endregion

                foreach (StiAxisLabelInfoXF3D info in infosFilter)
                {
                    if (!info.ClientSize.IsEmpty)
                    {
                        var labelGeom = new StiAxisLabelGeom3D(this.Axis,
                            info.ClientSize, info.TextPoint, info.Text, info.StripLine, info.RotationMode, render);

                        geom.CreateChildGeoms();
                        geom.ChildGeoms.Add(labelGeom);
                    }
                }
            }
        }
        #endregion

        public StiYAxisCoreXF3D(IStiAxis3D axis) : base(axis)
        {
        }
    }
}
