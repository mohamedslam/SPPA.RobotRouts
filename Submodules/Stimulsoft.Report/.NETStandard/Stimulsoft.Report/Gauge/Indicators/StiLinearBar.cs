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

using Stimulsoft.Base;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components.Gauge.Primitives;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Gauge.Events;
using Stimulsoft.Report.Gauge.GaugeGeoms;
using Stimulsoft.Report.Gauge.Helpers;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Windows.Forms;
#else
using System.Drawing.Design;
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Components.Gauge
{
    public class StiLinearBar : StiBarBase
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            jObject.AddPropertyEnum(nameof(Skin), Skin);
            jObject.AddPropertyEnum(nameof(RangeColorMode), RangeColorMode);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Skin):
                        this.Skin = property.DeserializeEnum<StiLinearBarSkin>();
                        break;

                    case nameof(RangeColorMode):
                        this.RangeColorMode = property.DeserializeEnum<StiLinearRangeColorMode>();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiLinearBar;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var checkBoxHelper = new StiPropertyCollection();

            // ValueCategory
            var list = new[]
            {
                propHelper.Value()
            };
            checkBoxHelper.Add(StiPropertyCategories.Value, list);

            // IndicatorCategory
            list = new[]
            {
                propHelper.EndWidth(),
                propHelper.Offset(),
                propHelper.Placement(),
                propHelper.RangeColorMode(),
                propHelper.Skin(),
                propHelper.StartWidth(),
                propHelper.UseRangeColor()
            };
            checkBoxHelper.Add(StiPropertyCategories.Indicator, list);

            // AppearanceCategory
            list = new[]
            {
                propHelper.Brush(),
                propHelper.BorderBrush(),
                propHelper.BorderWidth(),
                propHelper.EmptyBrush(),
                propHelper.EmptyBorderBrush(),
                propHelper.EmptyBorderWidth()
            };
            checkBoxHelper.Add(StiPropertyCategories.Appearance, list);

            // MiscCategory
            list = new[]
            {
                propHelper.AllowApplyStyle()
            };
            checkBoxHelper.Add(StiPropertyCategories.Misc, list);

            return checkBoxHelper;
        }
        #endregion

        #region IStiApplyStyleGauge
        public override void ApplyStyle(IStiGaugeStyle style)
        {
            if (this.AllowApplyStyle)
            {
                this.Brush = style.Core.LinearBarBrush;
                this.BorderBrush = style.Core.LinearBarBorderBrush;
                this.EmptyBrush = style.Core.LinearBarEmptyBrush;
                this.EmptyBorderBrush = style.Core.LinearBarEmptyBorderBrush;

                this.StartWidth = style.Core.LinearBarStartWidth;
                this.EndWidth = style.Core.LinearBarEndWidth;
            }
        }
        #endregion

        #region class LinearColorModeHelper
        private class LinearColorModeHelper
        {
            public bool standardBackground = false;
            public int defaultIndex = -99;
            public int mixedColorIndex = -99;

            public void Reset()
            {
                standardBackground = false;

                defaultIndex = -99;
                mixedColorIndex = -99;
            }
        }
        #endregion

        #region class LinearBarGeometryHelper
        private class LinearBarGeometryHelper
        {
            public RectangleF rect;
            public float maxWidth;
            public float minWidth;
            public bool isStartGreaterEnd;
            public bool isThisStartGreaterEnd;
            public StiLinearScale scale;

            public bool state = false;
            public float offset = 0f;
        }
        #endregion

        #region Fields
        private LinearColorModeHelper colorModeHelper = new LinearColorModeHelper();
        private StiBrush actualBackground;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the skin of the component rendering.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiLinearBarSkin.Default)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Indicator")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the skin of the component rendering.")]
        public StiLinearBarSkin Skin { get; set; } = StiLinearBarSkin.Default;

        private StiLinearRangeColorMode rangeColorMode = StiLinearRangeColorMode.Default;
        /// <summary>
        /// Gets or sets value which indicates the mode that is used to build bar indicator background.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiLinearRangeColorMode.Default)]
        [StiCategory("Indicator")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates the mode that is used to build bar indicator background.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiLinearRangeColorMode RangeColorMode
        {
            get
            {
                return this.rangeColorMode;
            }
            set
            {
                this.rangeColorMode = value;
                OnRangeColorChanged();
            }
        }

        protected override void OnRangeColorChanged()
        {
            this.colorModeHelper.Reset();
        }
        #endregion

        #region Properties override
        protected override StiBarRangeListType BarType => StiBarRangeListType.LinearBar;

        public override string LocalizeName => "LinearBar";
        #endregion

        #region Methods.RangeBrush
        protected override void CheckActualBrushForTopGeometry()
        {
            if (this.UseRangeColor)
            {
                actualBackground = GetRangeBrush();
            }
            else
            {
                actualBackground = this.Brush;
            }
        }

        private StiBrush GetRangeBrush()
        {
            var nullValue = this.GetActualValue();
            if (nullValue == null) return null;

            float value = nullValue.GetValueOrDefault();

            switch (this.RangeColorMode)
            {
                case StiLinearRangeColorMode.Default:
                    #region Default
                    for (int index = RangeList.Count - 1; index >= 0; index--)
                    {
                        StiLinearIndicatorRangeInfo info = this.RangeList[index] as StiLinearIndicatorRangeInfo;
                        if (value >= info.Value)
                        {
                            if (colorModeHelper.defaultIndex != index)
                            {
                                colorModeHelper.defaultIndex = index;
                                return (info.Brush != null) ? info.Brush : new StiSolidBrush(info.Color);
                            }
                        }
                    }
                    #endregion
                    break;

                case StiLinearRangeColorMode.MixedColor:
                    #region MixedColor
                    for (int index = RangeList.Count - 1; index >= 0; index--)
                    {
                        StiLinearIndicatorRangeInfo info = this.RangeList[index] as StiLinearIndicatorRangeInfo;
                        if (value >= info.Value)
                        {
                            if (colorModeHelper.mixedColorIndex != index)
                            {
                                colorModeHelper.mixedColorIndex = index;
                                List<Color> colors = new List<Color>();
                                for (int index1 = 0; index1 <= index; index1++)
                                {
                                    colors.Add(((StiLinearIndicatorRangeInfo)RangeList[index1]).Color);
                                }
                                return new StiSolidBrush(StiMixedColorHelper.ColorMixed(colors));
                            }

                            break;
                        }
                    }
                    #endregion
                    break;
            }
            return null;
        }
        #endregion

        #region Methods override
        public override StiGaugeElement CreateNew() => new StiLinearBar();

        protected internal override void InteractiveClick(RectangleF rect, Point p)
        {
        }

        private float GetValueObj()
        {
            if (this.Scale.Gauge.Report == null) return this.ValueObj;

            var nullValue = this.GetActualValue();
            if (nullValue == null) return 0;

            return nullValue.GetValueOrDefault();
        }

        protected internal override void DrawElement(StiGaugeContextPainter context)
        {
            if (this.Scale == null) return;

            switch (this.Skin)
            {
                case StiLinearBarSkin.HorizontalThermometer:
                    DrawHorizontalThermometer(context);
                    break;
                case StiLinearBarSkin.VerticalThermometer:
                    DrawVerticalThermometer(context);
                    break;
            }

            #region Draw BottomPath
            var rect = RectangleF.Empty;
            var linesGeom = this.Scale.barGeometry.DrawGeometry(context, Scale.Minimum, Scale.Maximum, this.StartWidth, this.EndWidth,
                this.Offset, this.Placement, ref rect, false);

            if (linesGeom != null)
            {
                var pathGeom = new StiGraphicsPathGaugeGeom(rect, linesGeom.Points[0], this.EmptyBrush, this.EmptyBorderBrush, this.EmptyBorderWidth);
                pathGeom.Geoms.Add(linesGeom);
                pathGeom.AddGraphicsPathCloseFigureGaugeGeom();

                context.AddGraphicsPathGaugeGeom(pathGeom);
            }
            #endregion

            #region Draw TopPath
            var valueTemp = GetValueObj();
            var geomHelper = this.GetGeometryHelperForTopIndicator(valueTemp);
            RectangleF rect1;
            var lines1Geom = this.GetTopGeometry(geomHelper, out rect1);

            if (lines1Geom != null)
            {
                CheckActualBrushForTopGeometry();
                var pathGeom = new StiGraphicsPathGaugeGeom(rect1, lines1Geom.Points[0], this.actualBackground, this.BorderBrush, this.BorderWidth);
                pathGeom.Geoms.Add(lines1Geom);
                pathGeom.AddGraphicsPathCloseFigureGaugeGeom();

                var startScaleX = 0;
                var endScaleX = 1;
                var startScaleY = 1;
                var endScaleY = 1;

                var centerX = rect.X;
                var centerY = rect.Y + rect.Height / 2;

                var lineScale = this.Scale as StiLinearScale;

                if (lineScale != null && lineScale.Orientation == Orientation.Vertical)
                {
                    startScaleX = 1;
                    endScaleX = 1;
                    startScaleY = 0;
                    endScaleY = 1;

                    centerX = rect.X + rect.Width / 2;
                    centerY = rect.Y + rect.Height;
                }

                if (this.Scale.Gauge.IsAnimation)
                    pathGeom.Animation = new StiScaleAnimation(startScaleX, endScaleX, startScaleY, endScaleY, centerX, centerY, StiGaugeHelper.GlobalDurationElement, TimeSpan.Zero);
                context.AddGraphicsPathGaugeGeom(pathGeom);
            }
            #endregion
        }
        #endregion

        #region Methods
        private void DrawHorizontalThermometer(StiGaugeContextPainter context)
        {
            var rect = RectangleF.Empty;
            this.Scale.barGeometry.DrawGeometry(context, this.Scale.Minimum, this.Scale.Maximum, this.StartWidth, this.EndWidth,
                this.Offset, this.Placement, ref rect, false);

            rect.Y -= 2f;
            rect.Height += 4f;
            rect.X -= 3;
            rect.Width += 3;

            var pathGeom = new StiGraphicsPathGaugeGeom(rect, rect.Location,
                new StiGradientBrush(Color.FromArgb(225, 230, 233), Color.FromArgb(242, 243, 244), 90f),
                new StiSolidBrush(Color.FromArgb(157, 157, 157)), 0.4f);

            pathGeom.AddGraphicsPathLineGaugeGeom(rect.Location, new PointF(rect.Right, rect.Top));
            pathGeom.AddGraphicsPathArcGaugeGeom(rect.Right, rect.Top, 4, rect.Height, 270, 180);
            pathGeom.AddGraphicsPathLineGaugeGeom(new PointF(rect.Right, rect.Bottom), new PointF(rect.Left, rect.Bottom));
            pathGeom.AddGraphicsPathCloseFigureGaugeGeom();

            context.AddGraphicsPathGaugeGeom(pathGeom);
        }

        private void DrawVerticalThermometer(StiGaugeContextPainter context)
        {
            var rect = RectangleF.Empty;
            this.Scale.barGeometry.DrawGeometry(context, this.Scale.Minimum, this.Scale.Maximum, this.StartWidth, this.EndWidth,
                this.Offset, this.Placement, ref rect, false);

            rect.X -= 2;
            rect.Width += 4;
            rect.Y -= 3f;
            rect.Height += 3f;

            var pathGeom = new StiGraphicsPathGaugeGeom(rect, rect.Location,
                new StiGradientBrush(Color.FromArgb(225, 230, 233), Color.FromArgb(242, 243, 244), 90f),
                new StiSolidBrush(Color.FromArgb(157, 157, 157)), 0.4f);

            pathGeom.AddGraphicsPathArcGaugeGeom(rect.Left, rect.Top - 4, rect.Width, 4, 180, 180);
            pathGeom.AddGraphicsPathLineGaugeGeom(new PointF(rect.Right, rect.Top), new PointF(rect.Right, rect.Bottom));
            pathGeom.AddGraphicsPathLineGaugeGeom(new PointF(rect.Right, rect.Bottom), new PointF(rect.Left, rect.Bottom));
            pathGeom.AddGraphicsPathCloseFigureGaugeGeom();

            context.AddGraphicsPathGaugeGeom(pathGeom);
        }

        private LinearBarGeometryHelper GetGeometryHelperForTopIndicator(float value)
        {
            var linearScale = this.Scale as StiLinearScale;
            var geomHelper = new LinearBarGeometryHelper();
            var size = Scale.barGeometry.Size;
            var rect = this.Scale.barGeometry.RectGeometry;
            float parentRest = StiMathHelper.MaxMinusMin(Scale.StartWidth, Scale.EndWidth);
            parentRest /= 2;
            parentRest = (linearScale.Orientation == Orientation.Horizontal) ? parentRest * size.Height : parentRest * size.Width;
            var centerRect = (linearScale.Orientation == Orientation.Horizontal)
                ? new RectangleF(rect.X, rect.Y + parentRest, rect.Width, rect.Height - (2 * parentRest))
                : new RectangleF(rect.X + parentRest, rect.Y, rect.Width - (2 * parentRest), rect.Height);

            #region Обрезам лишнюю область в зависимости от значения Value
            float minimum = (Scale.Minimum > Scale.Maximum) ? Scale.Maximum : Scale.Minimum;
            float fullLength = Math.Abs(Scale.Maximum) - Math.Abs(Scale.Minimum);
            float length = StiMathHelper.Length(minimum, value);

            if (length < 0)
            {
                length = 0;
            }
            else if (length > fullLength)
            {
                length = fullLength;
            }

            float percentOffset = length / fullLength;
            #endregion

            #region Helper
            float? geomMaxWidth = null;
            float? geomMinWidth = null;
            var finishRect = new RectangleF(0, 0, 0, 0);

            bool isStartGreaterEnd = Scale.StartWidth > Scale.EndWidth;
            bool isThisStartGreaterEnd = this.StartWidth > this.EndWidth;
            float actualWidth = (linearScale.Orientation == Orientation.Horizontal)
                ? rect.Width * percentOffset
                : rect.Height * percentOffset;

            float maxOffsetThisWidth;
            float minOffsetThisWidth;
            float maxThisWidth;
            float minThisWidth;

            if (this.StartWidth > EndWidth)
            {
                maxOffsetThisWidth = this.StartWidth;
                minOffsetThisWidth = this.EndWidth;
            }
            else
            {
                minOffsetThisWidth = this.StartWidth;
                maxOffsetThisWidth = this.EndWidth;
            }
            #endregion

            if (linearScale.Orientation == Orientation.Horizontal)
            #region Horizontal
            {
                maxThisWidth = maxOffsetThisWidth * size.Height;
                minThisWidth = minOffsetThisWidth * size.Height;
                float y;

                if (Scale.IsReversed)
                {
                    #region IsReversed
                    if (isStartGreaterEnd)
                    #region isStartGreaterEnd
                    {
                        switch (this.Placement)
                        {
                            case StiPlacement.Outside:
                                if (isThisStartGreaterEnd)
                                {
                                    finishRect = new RectangleF(rect.X + (centerRect.Width - actualWidth), centerRect.Y - parentRest - maxThisWidth, actualWidth, parentRest + maxThisWidth - (parentRest * (1 - percentOffset)));
                                    geomMinWidth = finishRect.Height - ((parentRest + maxThisWidth - minThisWidth) * percentOffset);
                                }
                                else
                                {
                                    if (maxThisWidth > parentRest + minThisWidth)
                                    {
                                        y = ((maxThisWidth - parentRest - minThisWidth) * (1 - percentOffset));
                                        finishRect = new RectangleF(centerRect.X + (centerRect.Width - actualWidth), centerRect.Y - maxThisWidth + y, actualWidth, maxThisWidth - (parentRest * (1 - percentOffset)) - y);
                                        geomHelper.offset = parentRest * percentOffset;
                                    }
                                    else
                                    {
                                        y = parentRest * (1 - percentOffset);
                                        finishRect = new RectangleF(centerRect.X + (centerRect.Width - actualWidth), centerRect.Y - parentRest - minThisWidth, actualWidth, parentRest + minThisWidth - y);
                                        geomMaxWidth = finishRect.Height - ((parentRest + minThisWidth - maxThisWidth) * percentOffset);
                                        geomHelper.state = true;
                                    }
                                }
                                break;

                            case StiPlacement.Overlay:
                                if (isThisStartGreaterEnd)
                                {
                                    finishRect = new RectangleF(centerRect.X + (centerRect.Width - actualWidth), StiRectangleHelper.CenterY(centerRect) - (maxThisWidth / 2), actualWidth, maxThisWidth);
                                    geomMinWidth = finishRect.Height - ((maxThisWidth - minThisWidth) * percentOffset);
                                }
                                else
                                {
                                    y = ((maxThisWidth - minThisWidth) / 2) * (1 - percentOffset);
                                    finishRect = new RectangleF(centerRect.X + (centerRect.Width - actualWidth), StiRectangleHelper.CenterY(centerRect) - (maxThisWidth / 2) + y, actualWidth, maxThisWidth - (2 * y));
                                }
                                break;

                            default:
                                if (isThisStartGreaterEnd)
                                {
                                    y = parentRest * (1 - percentOffset);
                                    finishRect = new RectangleF(centerRect.X + (centerRect.Width - actualWidth), centerRect.Bottom + y, actualWidth, parentRest + maxThisWidth - y);
                                    geomMinWidth = finishRect.Height - ((parentRest + maxThisWidth - minThisWidth) * percentOffset);
                                }
                                else
                                {
                                    if (maxThisWidth > parentRest + minThisWidth)
                                    {
                                        y = parentRest * (1 - percentOffset);
                                        float y1 = ((maxThisWidth - parentRest - minThisWidth) * (1 - percentOffset));
                                        finishRect = new RectangleF(centerRect.X + (centerRect.Width - actualWidth), centerRect.Bottom + y, actualWidth, maxThisWidth - y - y1);
                                        geomHelper.offset = (maxThisWidth - parentRest - minThisWidth) - y1;
                                    }
                                    else
                                    {
                                        y = parentRest * (1 - percentOffset);
                                        finishRect = new RectangleF(centerRect.X + (centerRect.Width - actualWidth), centerRect.Bottom + y, actualWidth, parentRest + minThisWidth - y);
                                        geomMaxWidth = finishRect.Height - ((parentRest + minThisWidth - maxThisWidth) * percentOffset);
                                        geomHelper.state = true;
                                    }
                                }
                                break;
                        }
                    }
                    #endregion
                    else
                    #region !isStartGreaterEnd
                    {
                        switch (this.Placement)
                        {
                            case StiPlacement.Outside:
                                if (isThisStartGreaterEnd)
                                {
                                    if (maxThisWidth > parentRest + minThisWidth)
                                    {
                                        finishRect = new RectangleF(centerRect.X + (centerRect.Width - actualWidth), centerRect.Y - maxThisWidth, actualWidth, maxThisWidth);
                                        geomHelper.offset = (maxThisWidth - parentRest - minThisWidth) * percentOffset;
                                        geomMinWidth = finishRect.Height - geomHelper.offset - (parentRest * percentOffset);
                                    }
                                    else
                                    {
                                        y = (parentRest + minThisWidth - maxThisWidth) * (1 - percentOffset);
                                        finishRect = new RectangleF(rect.X + (rect.Width - actualWidth), rect.Y - minThisWidth + y, actualWidth, parentRest + minThisWidth - y);
                                        geomMinWidth = finishRect.Height - (parentRest * percentOffset);
                                        geomHelper.state = true;
                                    }
                                }
                                else
                                {
                                    y = (parentRest + maxThisWidth - minThisWidth) * (1 - percentOffset);
                                    finishRect = new RectangleF(rect.X + (rect.Width - actualWidth), rect.Y - maxThisWidth + y, actualWidth, maxThisWidth + parentRest - y);
                                    geomMaxWidth = finishRect.Height - (parentRest * percentOffset);
                                }
                                break;

                            case StiPlacement.Overlay:
                                if (isThisStartGreaterEnd)
                                {
                                    finishRect = new RectangleF(centerRect.X + (rect.Width - actualWidth), StiRectangleHelper.CenterY(centerRect) - (maxThisWidth / 2), actualWidth, maxThisWidth);
                                    geomMinWidth = finishRect.Height - (2 * (((maxThisWidth - minThisWidth) / 2) * percentOffset));
                                }
                                else
                                {
                                    y = ((maxThisWidth - minThisWidth) / 2) * (1 - percentOffset);
                                    finishRect = new RectangleF(centerRect.X + (rect.Width - actualWidth), StiRectangleHelper.CenterY(centerRect) - (maxThisWidth / 2) + y, actualWidth, maxThisWidth - (2 * y));
                                }
                                break;

                            default:
                                if (isThisStartGreaterEnd)
                                {
                                    if (maxThisWidth > parentRest + minThisWidth)
                                    {
                                        finishRect = new RectangleF(centerRect.X + (rect.Width - actualWidth), centerRect.Bottom, actualWidth, maxThisWidth);
                                        geomHelper.offset = parentRest * percentOffset;
                                        geomMinWidth = finishRect.Height - ((maxThisWidth - parentRest - minThisWidth) * percentOffset) - geomHelper.offset;
                                    }
                                    else
                                    {
                                        y = (parentRest + minThisWidth - maxThisWidth) * percentOffset;
                                        finishRect = new RectangleF(centerRect.X + (rect.Width - actualWidth), centerRect.Bottom, actualWidth, maxThisWidth + y);
                                        geomMinWidth = finishRect.Height - (parentRest * percentOffset);
                                        geomHelper.state = true;
                                    }
                                }
                                else
                                {
                                    y = (parentRest + maxThisWidth - minThisWidth) * (1 - percentOffset);
                                    finishRect = new RectangleF(centerRect.X + (rect.Width - actualWidth), centerRect.Bottom, actualWidth, parentRest + maxThisWidth - y);
                                    geomMaxWidth = finishRect.Height - (parentRest * percentOffset);
                                }
                                break;
                        }
                    }
                    #endregion
                    #endregion
                }
                else
                {
                    #region !IsReversed
                    if (isStartGreaterEnd)
                    #region isStartGreaterEnd
                    {
                        switch (this.Placement)
                        {
                            case StiPlacement.Outside:
                                if (isThisStartGreaterEnd)
                                {
                                    finishRect = new RectangleF(rect.X, rect.Y - maxThisWidth, actualWidth, maxThisWidth + (parentRest * percentOffset));
                                    geomMinWidth = finishRect.Height - ((parentRest + maxThisWidth - minThisWidth) * (percentOffset));
                                }
                                else
                                {
                                    finishRect = new RectangleF(rect.X, rect.Y - minThisWidth, actualWidth, parentRest + minThisWidth - (parentRest * (1 - percentOffset)));
                                    geomMaxWidth = finishRect.Height - ((parentRest + minThisWidth - maxThisWidth) * percentOffset);
                                }
                                break;

                            case StiPlacement.Overlay:
                                if (isThisStartGreaterEnd)
                                {
                                    finishRect = new RectangleF(centerRect.X, StiRectangleHelper.CenterY(centerRect) - (maxThisWidth / 2), actualWidth, maxThisWidth);
                                    geomMinWidth = minThisWidth + (((maxThisWidth - minThisWidth) / 2) * percentOffset);
                                }
                                else
                                {
                                    y = ((maxThisWidth - minThisWidth) / 2) * (1 - percentOffset);
                                    finishRect = new RectangleF(centerRect.X, StiRectangleHelper.CenterY(centerRect) - (maxThisWidth / 2) + y, actualWidth, maxThisWidth - (2 * y));
                                }
                                break;

                            default:
                                if (isThisStartGreaterEnd)
                                {
                                    y = parentRest * percentOffset;
                                    finishRect = new RectangleF(rect.X, rect.Bottom - y, actualWidth, y + maxThisWidth);
                                    geomMinWidth = finishRect.Height - ((parentRest + maxThisWidth - minThisWidth) * percentOffset);
                                }
                                else
                                {
                                    if (maxThisWidth > parentRest + minThisWidth)
                                    {
                                        y = parentRest * (1 - percentOffset);
                                        geomHelper.offset = parentRest - y;
                                        finishRect = new RectangleF(centerRect.X, centerRect.Bottom + y, actualWidth, maxThisWidth - y - ((maxThisWidth - parentRest - minThisWidth) * (1 - percentOffset)));
                                    }
                                    else
                                    {
                                        y = (parentRest * percentOffset);
                                        finishRect = new RectangleF(rect.X, rect.Bottom - y, actualWidth, y + minThisWidth);
                                        geomMaxWidth = finishRect.Height - ((parentRest + minThisWidth - maxThisWidth) * percentOffset);
                                        geomHelper.state = true;
                                    }
                                }
                                break;
                        }
                    }
                    #endregion
                    else
                    #region !isStartGreaterEnd
                    {
                        float offset = (parentRest + maxThisWidth - minThisWidth) * percentOffset + minThisWidth;

                        switch (this.Placement)
                        {
                            case StiPlacement.Outside:
                                if (isThisStartGreaterEnd)
                                {
                                    if (maxThisWidth > parentRest + minThisWidth)
                                    {
                                        finishRect = new RectangleF(centerRect.X, centerRect.Y - maxThisWidth, actualWidth, maxThisWidth);
                                        geomHelper.offset = ((finishRect.Height - parentRest - minThisWidth) * percentOffset);
                                        geomMinWidth = finishRect.Height - geomHelper.offset - (parentRest * percentOffset);
                                    }
                                    else
                                    {
                                        y = (parentRest + minThisWidth - maxThisWidth) * percentOffset;
                                        finishRect = new RectangleF(centerRect.X, centerRect.Y - maxThisWidth - y, actualWidth, maxThisWidth + y);
                                        geomMinWidth = finishRect.Height - (parentRest * percentOffset);
                                        geomHelper.state = true;
                                    }
                                }
                                else
                                {
                                    finishRect = new RectangleF(centerRect.X, centerRect.Y - offset, actualWidth, offset);
                                    geomMaxWidth = finishRect.Height - (parentRest * percentOffset);
                                }
                                break;

                            case StiPlacement.Overlay:
                                y = ((maxThisWidth - minThisWidth) / 2) * (1 - percentOffset);
                                if (isThisStartGreaterEnd)
                                {
                                    finishRect = new RectangleF(rect.X, StiRectangleHelper.CenterY(centerRect) - (maxThisWidth / 2), actualWidth, maxThisWidth);
                                    geomMinWidth = minThisWidth + (((maxThisWidth - minThisWidth) / 2) * (1 - percentOffset));
                                }
                                else
                                {
                                    finishRect = new RectangleF(rect.X, StiRectangleHelper.CenterY(centerRect) - (maxThisWidth / 2) + y, actualWidth, maxThisWidth - 2 * y);
                                }
                                break;

                            default:
                                if (isThisStartGreaterEnd)
                                {
                                    if (maxThisWidth > parentRest + minThisWidth)
                                    {
                                        finishRect = new RectangleF(centerRect.X, centerRect.Bottom, actualWidth, maxThisWidth);
                                        geomHelper.offset = parentRest * percentOffset;
                                        geomMinWidth = (finishRect.Height - ((maxThisWidth - parentRest - minThisWidth) * percentOffset)) - geomHelper.offset;
                                    }
                                    else
                                    {
                                        y = (parentRest + minThisWidth - maxThisWidth) * (percentOffset);
                                        finishRect = new RectangleF(centerRect.X, centerRect.Bottom, actualWidth, maxThisWidth + y);
                                        geomMinWidth = finishRect.Height - (parentRest * percentOffset);
                                        geomHelper.state = true;
                                    }
                                }
                                else
                                {
                                    finishRect = new RectangleF(centerRect.X, centerRect.Bottom, actualWidth, offset);
                                    geomMaxWidth = minThisWidth + (maxThisWidth - minThisWidth) * percentOffset;
                                }
                                break;
                        }
                    }
                    #endregion
                    #endregion
                }
            }
            #endregion
            else
            #region Vertical
            {
                maxThisWidth = maxOffsetThisWidth * size.Width;
                minThisWidth = minOffsetThisWidth * size.Width;
                float x;

                if (Scale.IsReversed)
                {
                    #region IsReversed
                    if (isStartGreaterEnd)
                    #region isStartGreaterEnd
                    {
                        switch (this.Placement)
                        {
                            case StiPlacement.Outside:
                                x = parentRest * (1 - percentOffset);
                                if (isThisStartGreaterEnd)
                                {
                                    finishRect = new RectangleF(centerRect.X - parentRest - maxThisWidth, centerRect.Y, parentRest + maxThisWidth - x, actualWidth);
                                    geomMinWidth = finishRect.Width - ((parentRest + maxThisWidth - minThisWidth) * percentOffset);
                                }
                                else
                                {
                                    if (maxThisWidth > parentRest + minThisWidth)
                                    {
                                        float x1 = (maxThisWidth - minThisWidth - parentRest) * (1 - percentOffset);
                                        finishRect = new RectangleF(centerRect.X - maxThisWidth + x1, centerRect.Y, maxThisWidth - x - x1, actualWidth);
                                        geomHelper.offset = parentRest * percentOffset;
                                    }
                                    else
                                    {
                                        finishRect = new RectangleF(rect.X - minThisWidth, rect.Y, parentRest + minThisWidth - x, actualWidth);
                                        geomMaxWidth = finishRect.Width - ((parentRest + minThisWidth - maxThisWidth) * percentOffset);
                                        geomHelper.state = true;
                                    }
                                }
                                break;

                            case StiPlacement.Overlay:
                                if (isThisStartGreaterEnd)
                                {
                                    finishRect = new RectangleF(StiRectangleHelper.CenterX(centerRect) - (maxThisWidth / 2), centerRect.Y, maxThisWidth, actualWidth);
                                    geomHelper.offset = ((maxThisWidth - minThisWidth) / 2) * percentOffset;
                                }
                                else
                                {
                                    x = ((maxThisWidth - minThisWidth) / 2) * (1 - percentOffset);
                                    finishRect = new RectangleF(StiRectangleHelper.CenterX(centerRect) - (maxThisWidth / 2) + x, centerRect.Y, maxThisWidth - (2 * x), actualWidth);
                                }
                                break;

                            default:
                                x = parentRest * (1 - percentOffset);

                                if (isThisStartGreaterEnd)
                                {
                                    finishRect = new RectangleF(centerRect.Right + x, centerRect.Y, parentRest + maxThisWidth - x, actualWidth);
                                    geomMinWidth = finishRect.Width - ((parentRest + maxThisWidth - minThisWidth) * percentOffset);
                                }
                                else
                                {
                                    if (maxThisWidth > parentRest + minThisWidth)
                                    {
                                        float x1 = (maxThisWidth - parentRest - minThisWidth) * (1 - percentOffset);
                                        finishRect = new RectangleF(centerRect.Right + x, centerRect.Y, maxThisWidth - x - x1, actualWidth);
                                        geomMaxWidth = (maxThisWidth - parentRest - minThisWidth) * percentOffset;
                                        geomHelper.offset = parentRest * percentOffset;
                                    }
                                    else
                                    {
                                        finishRect = new RectangleF(centerRect.Right + x, centerRect.Y, parentRest + minThisWidth - x, actualWidth);
                                        geomMaxWidth = finishRect.Width - ((parentRest + minThisWidth - maxThisWidth) * percentOffset);
                                        geomHelper.state = true;
                                    }
                                }
                                break;
                        }
                    }
                    #endregion
                    else
                    #region !isStartGreaterEnd
                    {
                        switch (this.Placement)
                        {
                            case StiPlacement.Outside:
                                if (isThisStartGreaterEnd)
                                {
                                    if (maxThisWidth > parentRest + minThisWidth)
                                    {
                                        finishRect = new RectangleF(centerRect.X - maxThisWidth, centerRect.Y, maxThisWidth, actualWidth);
                                        geomHelper.offset = parentRest * percentOffset;
                                        geomMinWidth = finishRect.Width - geomHelper.offset - ((maxThisWidth - minimum - parentRest) * (1 - percentOffset));
                                    }
                                    else
                                    {
                                        x = (parentRest + minThisWidth - maxThisWidth) * (1 - percentOffset);
                                        finishRect = new RectangleF(centerRect.X - parentRest - minThisWidth + x, centerRect.Y, parentRest + minThisWidth - x, actualWidth);
                                        geomMinWidth = finishRect.Width - (parentRest * percentOffset);
                                        geomHelper.state = true;
                                    }
                                }
                                else
                                {
                                    x = (parentRest + maxThisWidth - minThisWidth) * (1 - percentOffset);
                                    finishRect = new RectangleF(centerRect.X - parentRest - maxThisWidth + x, centerRect.Y, parentRest + maxThisWidth - x, actualWidth);
                                    geomMaxWidth = finishRect.Width - (parentRest * percentOffset);
                                }
                                break;

                            case StiPlacement.Overlay:
                                if (isThisStartGreaterEnd)
                                {
                                    finishRect = new RectangleF(StiRectangleHelper.CenterX(centerRect) - (maxThisWidth / 2), centerRect.Y, maxThisWidth, actualWidth);
                                    geomHelper.offset = (((maxThisWidth - minThisWidth) / 2) * percentOffset);
                                }
                                else
                                {
                                    x = ((maxThisWidth - minThisWidth) / 2) * (1 - percentOffset);
                                    finishRect = new RectangleF(StiRectangleHelper.CenterX(centerRect) - (maxThisWidth / 2) + x, centerRect.Y, maxThisWidth - (2 * x), actualWidth);
                                }
                                break;

                            default:
                                if (isThisStartGreaterEnd)
                                {
                                    if (maxThisWidth > parentRest + minThisWidth)
                                    {
                                        finishRect = new RectangleF(centerRect.Right, centerRect.Y, maxThisWidth, actualWidth);
                                        geomHelper.offset = parentRest * percentOffset;
                                        geomMinWidth = finishRect.Width - geomHelper.offset - ((maxThisWidth - minThisWidth - parentRest) * percentOffset);
                                    }
                                    else
                                    {
                                        x = (parentRest + minThisWidth - maxThisWidth) * percentOffset;
                                        finishRect = new RectangleF(centerRect.Right, centerRect.Y, parentRest + minThisWidth - x, actualWidth);
                                        geomMinWidth = finishRect.Width - (parentRest * percentOffset);
                                        geomHelper.state = true;
                                    }
                                }
                                else
                                {
                                    x = (parentRest + maxThisWidth - minThisWidth) * (1 - percentOffset);
                                    finishRect = new RectangleF(centerRect.Right, rect.Y, parentRest + maxThisWidth - x, actualWidth);
                                    geomMaxWidth = finishRect.Width - (parentRest * percentOffset);
                                }
                                break;
                        }
                    }
                    #endregion
                    #endregion
                }
                else
                {
                    #region !IsReversed
                    if (isStartGreaterEnd)
                    #region isStartGreaterEnd
                    {
                        switch (this.Placement)
                        {
                            case StiPlacement.Outside:
                                if (isThisStartGreaterEnd)
                                {
                                    x = parentRest * (1 - percentOffset);
                                    finishRect = new RectangleF(centerRect.X - parentRest - maxThisWidth, centerRect.Y + (rect.Height - actualWidth), parentRest + maxThisWidth - x, actualWidth);
                                    geomMinWidth = finishRect.Width - ((parentRest + maxThisWidth - minThisWidth) * percentOffset);
                                }
                                else
                                {
                                    x = parentRest * (1 - percentOffset);
                                    if (maxThisWidth > parentRest + minThisWidth)
                                    {
                                        float x1 = (maxThisWidth - parentRest - minThisWidth) * (1 - percentOffset);
                                        finishRect = new RectangleF(centerRect.X - maxThisWidth + x1, centerRect.Y + (rect.Height - actualWidth), maxThisWidth - x - x1, actualWidth);
                                        geomHelper.offset = parentRest * percentOffset;
                                    }
                                    else
                                    {
                                        finishRect = new RectangleF(centerRect.X - parentRest - minThisWidth, centerRect.Y + (rect.Height - actualWidth), parentRest + minThisWidth - x, actualWidth);
                                        geomMaxWidth = finishRect.Width - ((parentRest + minThisWidth - maxThisWidth) * (percentOffset));
                                        geomHelper.state = true;
                                    }
                                }
                                break;

                            case StiPlacement.Overlay:
                                if (isThisStartGreaterEnd)
                                {
                                    finishRect = new RectangleF(StiRectangleHelper.CenterX(centerRect) - (maxThisWidth / 2), centerRect.Y + (rect.Height - actualWidth), maxThisWidth, actualWidth);
                                    geomHelper.offset = ((maxThisWidth - minThisWidth) / 2) * percentOffset;
                                }
                                else
                                {
                                    x = ((maxThisWidth - minThisWidth) / 2) * (1 - percentOffset);
                                    finishRect = new RectangleF(StiRectangleHelper.CenterX(centerRect) - (maxThisWidth / 2) + x, centerRect.Y + (rect.Height - actualWidth), maxThisWidth - (2 * x), actualWidth);
                                }
                                break;

                            default:
                                x = parentRest * (1 - percentOffset);

                                if (isThisStartGreaterEnd)
                                {
                                    finishRect = new RectangleF(centerRect.Right + x, centerRect.Y + (rect.Height - actualWidth), maxThisWidth + parentRest - x, actualWidth);
                                    geomMinWidth = finishRect.Width - ((parentRest + maxThisWidth - minThisWidth) * percentOffset);
                                }
                                else
                                {
                                    if (maxThisWidth >= parentRest + minThisWidth)
                                    {
                                        float x1 = (maxThisWidth - parentRest - minThisWidth) * (1 - percentOffset);
                                        finishRect = new RectangleF(centerRect.Right + x, centerRect.Y + (rect.Height - actualWidth), maxThisWidth - x - x1, actualWidth);
                                        geomHelper.offset = (maxThisWidth - parentRest - minThisWidth) * percentOffset;
                                    }
                                    else
                                    {
                                        finishRect = new RectangleF(centerRect.Right + x, centerRect.Y + (rect.Height - actualWidth), parentRest + minThisWidth - x, actualWidth);
                                        geomMaxWidth = finishRect.Width - ((parentRest + minThisWidth - maxThisWidth) * (percentOffset));
                                        geomHelper.state = true;
                                    }
                                }
                                break;
                        }
                    }
                    #endregion
                    else
                    #region !isStartGreaterEnd
                    {
                        switch (this.Placement)
                        {
                            case StiPlacement.Outside:
                                if (isThisStartGreaterEnd)
                                {
                                    if (maxThisWidth > parentRest + minThisWidth)
                                    {
                                        finishRect = new RectangleF(centerRect.X - maxThisWidth, centerRect.Y + (rect.Height - actualWidth), maxThisWidth, actualWidth);
                                        geomHelper.offset = parentRest * percentOffset;
                                        x = (maxThisWidth - minThisWidth - parentRest) * percentOffset;
                                        geomMinWidth = finishRect.Width - geomHelper.offset - x;
                                    }
                                    else
                                    {
                                        x = (parentRest + minThisWidth - maxThisWidth) * (1 - percentOffset);
                                        finishRect = new RectangleF(rect.X - minThisWidth + x, rect.Y + (rect.Height - actualWidth), parentRest + minThisWidth - x, actualWidth);
                                        geomMinWidth = finishRect.Width - (parentRest * percentOffset);
                                        geomHelper.state = true;
                                    }
                                }
                                else
                                {
                                    x = (parentRest + maxThisWidth - minThisWidth) * (1 - percentOffset);
                                    finishRect = new RectangleF(rect.X - maxThisWidth + x, rect.Y + (rect.Height - actualWidth), maxThisWidth + parentRest - x, actualWidth);
                                    geomMaxWidth = finishRect.Width - parentRest * percentOffset;
                                }
                                break;

                            case StiPlacement.Overlay:
                                if (isThisStartGreaterEnd)
                                {
                                    finishRect = new RectangleF(StiRectangleHelper.CenterX(centerRect) - (maxThisWidth / 2), rect.Y + (rect.Height - actualWidth), maxThisWidth, actualWidth);
                                    geomHelper.offset = ((((maxThisWidth - minThisWidth)) * (percentOffset))) / 2;
                                }
                                else
                                {
                                    x = ((maxThisWidth - minThisWidth) / 2) * (1 - percentOffset);
                                    finishRect = new RectangleF(StiRectangleHelper.CenterX(centerRect) - (maxThisWidth / 2) + x, rect.Y + (rect.Height - actualWidth), maxThisWidth - (2 * x), actualWidth);
                                }
                                break;

                            default:
                                if (isThisStartGreaterEnd)
                                {
                                    if (maxThisWidth > parentRest + minThisWidth)
                                    {
                                        finishRect = new RectangleF(centerRect.Right, rect.Y + (rect.Height - actualWidth), maxThisWidth, actualWidth);
                                        geomHelper.offset = (parentRest * percentOffset);
                                        geomMinWidth = finishRect.Width - geomHelper.offset - ((maxThisWidth - parentRest - minThisWidth) * percentOffset);
                                    }
                                    else
                                    {
                                        x = (parentRest + minThisWidth - maxThisWidth) * percentOffset;
                                        finishRect = new RectangleF(centerRect.Right, rect.Y + (rect.Height - actualWidth), parentRest + minThisWidth - x, actualWidth);
                                        geomMinWidth = finishRect.Width - (parentRest * percentOffset);
                                        geomHelper.state = true;
                                    }
                                }
                                else
                                {
                                    x = ((parentRest + maxThisWidth - minThisWidth) * (1 - percentOffset));
                                    finishRect = new RectangleF(centerRect.Right, rect.Y + (rect.Height - actualWidth), parentRest + maxThisWidth - x, actualWidth);
                                    geomMaxWidth = finishRect.Width - (parentRest * percentOffset);
                                }
                                break;
                        }
                    }
                    #endregion
                    #endregion
                }
            }
            #endregion

            #region Create Helper
            geomHelper.rect = finishRect;
            geomHelper.isStartGreaterEnd = isStartGreaterEnd;
            geomHelper.isThisStartGreaterEnd = isThisStartGreaterEnd;
            geomHelper.scale = linearScale;
            geomHelper.maxWidth = (geomMaxWidth == null) ? maxThisWidth : geomMaxWidth.Value;
            geomHelper.minWidth = (geomMinWidth == null) ? minThisWidth : geomMinWidth.Value;
            #endregion

            return geomHelper;
        }

        private StiGraphicsPathLinesGaugeGeom GetTopGeometry(LinearBarGeometryHelper helper, out RectangleF rect)
        {
            rect = new RectangleF(0, 0, helper.rect.Width, helper.rect.Height);
            var points = new PointF[4];

            if (helper.scale.Orientation == Orientation.Horizontal)
            #region Horizontal
            {
                if (helper.scale.IsReversed)
                {
                    #region IsReversed
                    if (helper.isStartGreaterEnd)
                    {
                        if (helper.isThisStartGreaterEnd)
                        {
                            switch (this.Placement)
                            {
                                case StiPlacement.Outside:
                                    points[0] = new PointF(0, rect.Height - helper.minWidth);
                                    points[1] = new PointF(rect.Width, 0);
                                    points[2] = new PointF(rect.Width, helper.maxWidth);
                                    points[3] = new PointF(0, rect.Height);
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF(0, (rect.Height - helper.minWidth) / 2);
                                    points[1] = new PointF(rect.Width, 0);
                                    points[2] = new PointF(rect.Width, rect.Height);
                                    points[3] = new PointF(0, rect.Height - ((rect.Height - helper.minWidth) / 2));
                                    break;

                                default:
                                    points[0] = new PointF(0, 0);
                                    points[1] = new PointF(rect.Width, rect.Height - helper.maxWidth);
                                    points[2] = new PointF(rect.Width, rect.Height);
                                    points[3] = new PointF(0, helper.minWidth);
                                    break;
                            }
                        }
                        else
                        {
                            switch (this.Placement)
                            {
                                case StiPlacement.Outside:
                                    if (helper.state)
                                    {
                                        points[0] = new PointF(0, rect.Height - helper.maxWidth);
                                        points[1] = new PointF(rect.Width, 0);
                                        points[2] = new PointF(rect.Width, helper.minWidth);
                                        points[3] = new PointF(0, rect.Height);
                                    }
                                    else
                                    {
                                        points[0] = new PointF(0, 0);
                                        points[1] = new PointF(rect.Width, rect.Height - helper.offset - helper.minWidth);
                                        points[2] = new PointF(rect.Width, rect.Height - helper.offset);
                                        points[3] = new PointF(0, rect.Height);
                                    }
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF(0, 0);
                                    points[1] = new PointF(rect.Width, StiRectangleHelper.CenterY(rect) - (helper.minWidth / 2));
                                    points[2] = new PointF(rect.Width, StiRectangleHelper.CenterY(rect) + (helper.minWidth / 2));
                                    points[3] = new PointF(0, rect.Height);
                                    break;

                                default:
                                    if (helper.state)
                                    {
                                        points[0] = new PointF(0, 0);
                                        points[1] = new PointF(rect.Width, rect.Height - helper.minWidth);
                                        points[2] = new PointF(rect.Width, rect.Height);
                                        points[3] = new PointF(0, helper.maxWidth);
                                    }
                                    else
                                    {
                                        points[0] = new PointF(0, 0);
                                        points[1] = new PointF(rect.Width, rect.Height - helper.offset - helper.minWidth);
                                        points[2] = new PointF(rect.Width, rect.Height - helper.offset);
                                        points[3] = new PointF(0, rect.Height);
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (helper.isThisStartGreaterEnd)
                        {
                            switch (this.Placement)
                            {
                                case StiPlacement.Outside:
                                    if (helper.state)
                                    {
                                        points[0] = new PointF(0, 0);
                                        points[1] = new PointF(rect.Width, rect.Height - helper.maxWidth);
                                        points[2] = new PointF(rect.Width, rect.Height);
                                        points[3] = new PointF(0, helper.minWidth);
                                    }
                                    else
                                    {
                                        points[0] = new PointF(0, helper.offset);
                                        points[1] = new PointF(rect.Width, 0);
                                        points[2] = new PointF(rect.Width, rect.Height);
                                        points[3] = new PointF(0, helper.minWidth + helper.offset);
                                    }
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF(0, StiRectangleHelper.CenterY(rect) - helper.minWidth / 2);
                                    points[1] = new PointF(rect.Width, 0);
                                    points[2] = new PointF(rect.Width, rect.Height);
                                    points[3] = new PointF(0, StiRectangleHelper.CenterY(rect) + helper.minWidth / 2);
                                    break;

                                default:
                                    if (helper.state)
                                    {
                                        points[0] = new PointF(0, rect.Height - helper.minWidth);
                                        points[1] = new PointF(rect.Width, 0);
                                        points[2] = new PointF(rect.Width, helper.maxWidth);
                                        points[3] = new PointF(0, rect.Height);
                                    }
                                    else
                                    {
                                        points[0] = new PointF(0, helper.offset);
                                        points[1] = new PointF(rect.Width, 0);
                                        points[2] = new PointF(rect.Width, rect.Height);
                                        points[3] = new PointF(0, helper.offset + helper.minWidth);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            switch (this.Placement)
                            {
                                case StiPlacement.Outside:
                                    points[0] = new PointF(0, 0);
                                    points[1] = new PointF(rect.Width, rect.Height - helper.minWidth);
                                    points[2] = new PointF(rect.Width, rect.Height);
                                    points[3] = new PointF(0, helper.maxWidth);
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF(0, 0);
                                    points[1] = new PointF(rect.Width, StiRectangleHelper.CenterY(rect) - helper.minWidth / 2);
                                    points[2] = new PointF(rect.Width, StiRectangleHelper.CenterY(rect) + helper.minWidth / 2);
                                    points[3] = new PointF(0, rect.Height);
                                    break;

                                default:
                                    points[0] = new PointF(0, rect.Height - helper.maxWidth);
                                    points[1] = new PointF(rect.Width, 0);
                                    points[2] = new PointF(rect.Width, helper.minWidth);
                                    points[3] = new PointF(0, rect.Height);
                                    break;
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region !IsReversed
                    if (helper.isStartGreaterEnd)
                    #region isStartGreaterEnd
                    {
                        if (helper.isThisStartGreaterEnd)
                        {
                            switch (this.Placement)
                            {
                                case StiPlacement.Outside:
                                    points[0] = new PointF(0, 0);
                                    points[1] = new PointF(rect.Width, rect.Height - helper.minWidth);
                                    points[2] = new PointF(rect.Width, rect.Height);
                                    points[3] = new PointF(0, helper.maxWidth);
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF(0, 0);
                                    points[1] = new PointF(rect.Width, StiRectangleHelper.CenterY(rect) - (helper.minWidth / 2));
                                    points[2] = new PointF(rect.Width, StiRectangleHelper.CenterY(rect) + (helper.minWidth / 2));
                                    points[3] = new PointF(0, rect.Height);
                                    break;

                                default:
                                    points[0] = new PointF(0, rect.Height - helper.maxWidth);
                                    points[1] = new PointF(rect.Width, 0);
                                    points[2] = new PointF(rect.Width, helper.minWidth);
                                    points[3] = new PointF(0, rect.Height);
                                    break;
                            }
                        }
                        else
                        {
                            switch (this.Placement)
                            {
                                case StiPlacement.Outside:
                                    points[0] = new PointF(0, 0);
                                    points[1] = new PointF(rect.Width, rect.Height - helper.maxWidth);
                                    points[2] = new PointF(rect.Width, rect.Height);
                                    points[3] = new PointF(0, helper.minWidth);
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF(0, StiRectangleHelper.CenterY(rect) - helper.minWidth / 2);
                                    points[1] = new PointF(rect.Width, 0);
                                    points[2] = new PointF(rect.Width, rect.Height);
                                    points[3] = new PointF(0, StiRectangleHelper.CenterY(rect) + helper.minWidth / 2);
                                    break;

                                default:
                                    if (helper.state)
                                    {
                                        points[0] = new PointF(0, rect.Height - helper.minWidth);
                                        points[1] = new PointF(rect.Width, 0);
                                        points[2] = new PointF(rect.Width, helper.maxWidth);
                                        points[3] = new PointF(0, rect.Height);
                                    }
                                    else
                                    {
                                        points[0] = new PointF(0, helper.offset);
                                        points[1] = new PointF(rect.Width, 0);
                                        points[2] = new PointF(rect.Width, rect.Height);
                                        points[3] = new PointF(0, helper.offset + helper.minWidth);
                                    }
                                    break;
                            }
                        }
                    }
                    #endregion
                    else
                    #region !isStartGreaterEnd
                    {
                        if (helper.isThisStartGreaterEnd)
                        {
                            switch (this.Placement)
                            {
                                case StiPlacement.Outside:
                                    if (helper.state)
                                    {
                                        points[0] = new PointF(0, rect.Height - helper.maxWidth);
                                        points[1] = new PointF(rect.Width, 0);
                                        points[2] = new PointF(rect.Width, helper.minWidth);
                                        points[3] = new PointF(0, rect.Height);
                                    }
                                    else
                                    {
                                        points[0] = new PointF(0, 0);
                                        points[1] = new PointF(rect.Width, helper.offset);
                                        points[2] = new PointF(rect.Width, helper.offset + helper.minWidth);
                                        points[3] = new PointF(0, rect.Height);
                                    }
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF(0, 0);
                                    points[1] = new PointF(rect.Width, StiRectangleHelper.CenterY(rect) - helper.minWidth / 2);
                                    points[2] = new PointF(rect.Width, StiRectangleHelper.CenterY(rect) + helper.minWidth / 2);
                                    points[3] = new PointF(0, rect.Height);
                                    break;

                                default:
                                    if (helper.state)
                                    {
                                        points[0] = new PointF(0, 0);
                                        points[1] = new PointF(rect.Width, rect.Height - helper.minWidth);
                                        points[2] = new PointF(rect.Width, rect.Height);
                                        points[3] = new PointF(0, helper.maxWidth);
                                    }
                                    else
                                    {
                                        points[0] = new PointF(0, 0);
                                        points[1] = new PointF(rect.Width, helper.offset);
                                        points[2] = new PointF(rect.Width, helper.offset + helper.minWidth);
                                        points[3] = new PointF(0, rect.Height);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            switch (this.Placement)
                            {
                                case StiPlacement.Outside:
                                    points[0] = new PointF(0, rect.Height - helper.minWidth);
                                    points[1] = new PointF(rect.Width, 0);
                                    points[2] = new PointF(rect.Width, helper.maxWidth);
                                    points[3] = new PointF(0, rect.Height);
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF(0, StiRectangleHelper.CenterY(rect) - helper.minWidth / 2);
                                    points[1] = new PointF(rect.Width, 0);
                                    points[2] = new PointF(rect.Width, rect.Height);
                                    points[3] = new PointF(0, StiRectangleHelper.CenterY(rect) + helper.minWidth / 2);
                                    break;

                                default:
                                    points[0] = new PointF(0, 0);
                                    points[1] = new PointF(rect.Width, rect.Height - helper.maxWidth);
                                    points[2] = new PointF(rect.Width, rect.Height);
                                    points[3] = new PointF(0, helper.minWidth);
                                    break;
                            }
                        }
                    }
                    #endregion
                    #endregion
                }
            }
            #endregion
            else
            #region Vertical
            {
                if (helper.scale.IsReversed)
                {
                    #region IsReversed
                    if (helper.isStartGreaterEnd)
                    #region isStartGreaterEnd
                    {
                        if (helper.isThisStartGreaterEnd)
                        {
                            switch (this.Placement)
                            {
                                case StiPlacement.Outside:
                                    points[0] = new PointF(0, 0);
                                    points[1] = new PointF(helper.maxWidth, 0);
                                    points[2] = new PointF(rect.Width, rect.Height);
                                    points[3] = new PointF(rect.Width - helper.minWidth, rect.Height);
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF(0, 0);
                                    points[1] = new PointF(rect.Width, 0);
                                    points[2] = new PointF(rect.Width - helper.offset, rect.Height);
                                    points[3] = new PointF(helper.offset, rect.Height);
                                    break;

                                default:
                                    points[0] = new PointF(rect.Width - helper.maxWidth, 0);
                                    points[1] = new PointF(rect.Width, 0);
                                    points[2] = new PointF(helper.minWidth, rect.Height);
                                    points[3] = new PointF(0, rect.Height);
                                    break;
                            }
                        }
                        else
                        {
                            switch (this.Placement)
                            {
                                case StiPlacement.Outside:
                                    if (helper.state)
                                    {
                                        points[0] = new PointF(0, 0);
                                        points[1] = new PointF(helper.minWidth, 0);
                                        points[2] = new PointF(rect.Width, rect.Height);
                                        points[3] = new PointF(rect.Width - helper.maxWidth, rect.Height);
                                    }
                                    else
                                    {
                                        points[0] = new PointF(rect.Width - helper.offset - helper.minWidth, 0);
                                        points[1] = new PointF(rect.Width - helper.offset, 0);
                                        points[2] = new PointF(rect.Width, rect.Height);
                                        points[3] = new PointF(0, rect.Height);
                                    }
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF((rect.Width - helper.minWidth) / 2, 0);
                                    points[1] = new PointF(rect.Width - ((rect.Width - helper.minWidth) / 2), 0);
                                    points[2] = new PointF(rect.Width, rect.Height);
                                    points[3] = new PointF(0, rect.Height);
                                    break;

                                default:
                                    if (helper.state)
                                    {
                                        points[0] = new PointF(rect.Width - helper.minWidth, 0);
                                        points[1] = new PointF(rect.Width, 0);
                                        points[2] = new PointF(helper.maxWidth, rect.Height);
                                        points[3] = new PointF(0, rect.Height);
                                    }
                                    else
                                    {
                                        points[0] = new PointF(helper.offset, 0);
                                        points[1] = new PointF(helper.offset + helper.minWidth, 0);
                                        points[2] = new PointF(rect.Width, rect.Height);
                                        points[3] = new PointF(0, rect.Height);
                                    }
                                    break;
                            }
                        }
                    }
                    #endregion
                    else
                    #region !isStartGreaterEnd
                    {
                        if (helper.isThisStartGreaterEnd)
                        {
                            switch (this.Placement)
                            {
                                case StiPlacement.Outside:
                                    if (helper.state)
                                    {
                                        points[0] = new PointF(rect.Width - helper.maxWidth, 0);
                                        points[1] = new PointF(rect.Width, 0);
                                        points[2] = new PointF(helper.minWidth, rect.Height);
                                        points[3] = new PointF(0, rect.Height);
                                    }
                                    else
                                    {
                                        points[0] = new PointF(0, 0);
                                        points[1] = new PointF(rect.Width, 0);
                                        points[2] = new PointF(rect.Width - helper.offset, rect.Height);
                                        points[3] = new PointF(rect.Width - helper.offset - helper.minWidth, rect.Height);
                                    }
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF(0, 0);
                                    points[1] = new PointF(rect.Width, 0);
                                    points[2] = new PointF(rect.Width - helper.offset, rect.Height);
                                    points[3] = new PointF(helper.offset, rect.Height);
                                    break;

                                default:
                                    if (helper.state)
                                    {
                                        points[0] = new PointF(0, 0);
                                        points[1] = new PointF(helper.maxWidth, 0);
                                        points[2] = new PointF(rect.Width, rect.Height);
                                        points[3] = new PointF(rect.Width - helper.minWidth, rect.Height);
                                    }
                                    else
                                    {
                                        points[0] = new PointF(0, 0);
                                        points[1] = new PointF(rect.Width, 0);
                                        points[2] = new PointF(helper.offset + helper.minWidth, rect.Height);
                                        points[3] = new PointF(helper.offset, rect.Height);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            switch (this.Placement)
                            {
                                case StiPlacement.Outside:
                                    points[0] = new PointF(rect.Width - helper.minWidth, 0);
                                    points[1] = new PointF(rect.Width, 0);
                                    points[2] = new PointF(helper.maxWidth, rect.Height);
                                    points[3] = new PointF(0, rect.Height);
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF((rect.Width - helper.minWidth) / 2, 0);
                                    points[1] = new PointF(rect.Width - ((rect.Width - helper.minWidth) / 2), 0);
                                    points[2] = new PointF(rect.Width, rect.Height);
                                    points[3] = new PointF(0, rect.Height);
                                    break;

                                default:
                                    points[0] = new PointF(0, 0);
                                    points[1] = new PointF(helper.minWidth, 0);
                                    points[2] = new PointF(rect.Width, rect.Height);
                                    points[3] = new PointF(rect.Width - helper.maxWidth, rect.Height);
                                    break;
                            }
                        }
                    }
                    #endregion
                    #endregion
                }
                else
                {
                    #region !IsReversed
                    if (helper.isStartGreaterEnd)
                    #region isStartGreaterEnd
                    {
                        if (helper.isThisStartGreaterEnd)
                        {
                            switch (this.Placement)
                            {
                                case StiPlacement.Outside:
                                    points[0] = new PointF(rect.Width - helper.minWidth, 0);
                                    points[1] = new PointF(rect.Width, 0);
                                    points[2] = new PointF(helper.maxWidth, rect.Height);
                                    points[3] = new PointF(0, rect.Height);
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF(helper.offset, 0);
                                    points[1] = new PointF(rect.Width - helper.offset, 0);
                                    points[2] = new PointF(rect.Width, rect.Height);
                                    points[3] = new PointF(0, rect.Height);
                                    break;

                                default:
                                    points[0] = new PointF(0, 0);
                                    points[1] = new PointF(helper.minWidth, 0);
                                    points[2] = new PointF(rect.Width, rect.Height);
                                    points[3] = new PointF(rect.Width - helper.maxWidth, rect.Height);
                                    break;
                            }
                        }
                        else
                        {
                            switch (this.Placement)
                            {
                                case StiPlacement.Outside:
                                    if (helper.state)
                                    {
                                        points[0] = new PointF(rect.Width - helper.maxWidth, 0);
                                        points[1] = new PointF(rect.Width, 0);
                                        points[2] = new PointF(helper.minWidth, rect.Height);
                                        points[3] = new PointF(0, rect.Height);
                                    }
                                    else
                                    {
                                        points[0] = new PointF(0, 0);
                                        points[1] = new PointF(rect.Width, 0);
                                        points[2] = new PointF(rect.Width - helper.offset, rect.Height);
                                        points[3] = new PointF(rect.Width - helper.offset - helper.minWidth, rect.Height);
                                    }
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF(0, 0);
                                    points[1] = new PointF(rect.Width, 0);
                                    points[2] = new PointF(rect.Width - ((rect.Width - helper.minWidth) / 2), rect.Height);
                                    points[3] = new PointF((rect.Width - helper.minWidth) / 2, rect.Height);
                                    break;

                                default:
                                    if (helper.state)
                                    {
                                        points[0] = new PointF(0, 0);
                                        points[1] = new PointF(helper.maxWidth, 0);
                                        points[2] = new PointF(rect.Width, rect.Height);
                                        points[3] = new PointF(rect.Width - helper.minWidth, rect.Height);
                                    }
                                    else
                                    {
                                        points[0] = new PointF(0, 0);
                                        points[1] = new PointF(rect.Width, 0);
                                        points[2] = new PointF(rect.Width - helper.offset, rect.Height);
                                        points[3] = new PointF(rect.Width - helper.offset - helper.minWidth, rect.Height);
                                    }
                                    break;
                            }
                        }
                    }
                    #endregion
                    else
                    #region !isStartGreaterEnd
                    {
                        if (helper.isThisStartGreaterEnd)
                        {
                            switch (this.Placement)
                            {
                                case StiPlacement.Outside:
                                    if (helper.state)
                                    {
                                        points[0] = new PointF(0, 0);
                                        points[1] = new PointF(helper.minWidth, 0);
                                        points[2] = new PointF(rect.Width, rect.Height);
                                        points[3] = new PointF(rect.Width - helper.maxWidth, rect.Height);
                                    }
                                    else
                                    {
                                        points[0] = new PointF(rect.Width - helper.offset - helper.minWidth, 0);
                                        points[1] = new PointF(rect.Width - helper.offset, 0);
                                        points[2] = new PointF(rect.Width, rect.Height);
                                        points[3] = new PointF(0, rect.Height);
                                    }
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF(helper.offset, 0);
                                    points[1] = new PointF(rect.Width - helper.offset, 0);
                                    points[2] = new PointF(rect.Width, rect.Height);
                                    points[3] = new PointF(0, rect.Height);
                                    break;

                                default:
                                    if (helper.state)
                                    {
                                        points[0] = new PointF(rect.Width - helper.minWidth, 0);
                                        points[1] = new PointF(rect.Width, 0);
                                        points[2] = new PointF(helper.maxWidth, rect.Height);
                                        points[3] = new PointF(0, rect.Height);
                                    }
                                    else
                                    {
                                        points[0] = new PointF(helper.offset, 0);
                                        points[1] = new PointF(helper.offset + helper.minWidth, 0);
                                        points[2] = new PointF(rect.Width, rect.Height);
                                        points[3] = new PointF(0, rect.Height);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            switch (this.Placement)
                            {
                                case StiPlacement.Outside:
                                    points[0] = new PointF(0, 0);
                                    points[1] = new PointF(helper.maxWidth, 0);
                                    points[2] = new PointF(rect.Width, rect.Height);
                                    points[3] = new PointF(rect.Width - helper.minWidth, rect.Height);
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF(0, 0);
                                    points[1] = new PointF(rect.Width, 0);
                                    points[2] = new PointF(StiRectangleHelper.CenterX(rect) + (helper.minWidth / 2), rect.Height);
                                    points[3] = new PointF(StiRectangleHelper.CenterX(rect) - (helper.minWidth / 2), rect.Height);
                                    break;

                                default:
                                    points[0] = new PointF(rect.Width - helper.maxWidth, 0);
                                    points[1] = new PointF(rect.Width, 0);
                                    points[2] = new PointF(helper.minWidth, rect.Height);
                                    points[3] = new PointF(0, rect.Height);
                                    break;
                            }
                        }
                    }
                    #endregion
                    #endregion
                }
            }
            #endregion

            #region Смешаем по X и Y
            float left = helper.rect.Left;
            float top = helper.rect.Top;
            float value = Scale.barGeometry.Size.Width * this.Offset;

            if (helper.scale.Orientation == Orientation.Horizontal)
            {
                if (this.Placement == StiPlacement.Outside)
                    top -= value;
                else
                    top += value;
            }
            else
            {
                if (this.Placement == StiPlacement.Outside)
                    left -= value;
                else
                    left += value;
            }

            points[0].X += left;
            points[1].X += left;
            points[2].X += left;
            points[3].X += left;
            points[0].Y += top;
            points[1].Y += top;
            points[2].Y += top;
            points[3].Y += top;
            #endregion

            return new StiGraphicsPathLinesGaugeGeom(points);
        }
        #endregion
    }
}