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
    public class StiXAxisCoreXF3D : StiAxisCoreXF3D
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

        private List<StiAxisLabelInfoXF3D> MeasureStripLines(StiContext context, StiRectangle3D? rectangle3D = null)
        {
            var infos = new List<StiAxisLabelInfoXF3D>();

            if (this.Axis.Info.LabelsCollection != null)
            {
                //var titleSize = GetAxisTitleSize(context);

                int index = 0;
                foreach (StiStripPositionXF label in this.Axis.Info.LabelsCollection)
                {
                    if (label.StripLine.ValueObject == null) continue;

                    string text = GetLabelText(label.StripLine.ValueObject, label.StripLine.Value, null);

                    #region Init Start Y Position
                    float posX = label.Position;
                    #endregion

                    //If point does not contains in area rectangle then skip this line
                    #region Labels
                    var sf = context.GetGenericStringFormat();
                    var font = StiFontGeom.ChangeFontSize(Axis.Labels.Font, Axis.Labels.Font.Size * context.Options.Zoom);
                    var rotationMode = Base.Drawing.StiRotationMode.CenterTop;

                    StiPoint3D? point = null;
                    //new PointF(rect.Width /*- GetTicksMaxLength(context) - GetLabelsSpaceAxis(context)*/, posY);
                    if (rectangle3D != null)
                    {
                        var tempRectangle3D = rectangle3D.GetValueOrDefault();
                        point = new StiPoint3D(posX, tempRectangle3D.Y, tempRectangle3D.Front);
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


        public override StiCellGeom Render3D(StiContext context, StiRectangle3D rect3D, StiRender3D render)
        {
            if (!this.Axis.Visible)
                return null;

            var geom = new StiXAxisGeom3D(rect3D, render);

            RenderLabels(context, rect3D, geom, render);

            return geom;
        }

        private void RenderLabels(StiContext context, StiRectangle3D rect3D, StiXAxisGeom3D geom, StiRender3D render)
        {
            //if (Axis.Labels.Placement != StiLabelsPlacement.None)
            {
                var infos = MeasureStripLines(context, rect3D);

                geom.CreateChildGeoms();

                foreach (StiAxisLabelInfoXF3D info in infos)
                {
                    if (!info.ClientSize.IsEmpty)
                    {
                        var labelGeom = new StiAxisLabelGeom3D(this.Axis,
                            info.ClientSize, info.TextPoint, info.Text, info.StripLine, info.RotationMode, render);
                        geom.ChildGeoms.Add(labelGeom);
                    }
                }
            }
        }
        #endregion

        public StiXAxisCoreXF3D(IStiAxis3D axis) : base(axis)
        {
        }
    }
}
