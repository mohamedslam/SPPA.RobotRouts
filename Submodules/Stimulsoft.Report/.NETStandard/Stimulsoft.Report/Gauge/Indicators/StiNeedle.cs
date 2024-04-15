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
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Gauge.Helpers;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Drawing;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Components.Gauge
{
    public class StiNeedle : StiIndicatorBase
    {
        public StiNeedle()
        {
            this.Brush = new StiSolidBrush(Color.FromArgb(158, 158, 158));
        }

        #region IStiJsonReportObject.override

        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            jObject.AddPropertyStringNullOrEmpty(nameof(Format), Format);
            jObject.AddPropertyBool(nameof(ShowValue), ShowValue);
            jObject.AddPropertyBrush(nameof(TextBrush), TextBrush);
            jObject.AddPropertyFontArial7(nameof(Font), Font);
            jObject.AddPropertyBrush(nameof(CapBrush), CapBrush);
            jObject.AddPropertyBrush(nameof(CapBorderBrush), CapBorderBrush);
            jObject.AddPropertyFloat(nameof(CapBorderWidth), CapBorderWidth);
            jObject.AddPropertyFloat(nameof(OffsetNeedle), OffsetNeedle);
            jObject.AddPropertyFloat(nameof(StartWidth), StartWidth, 0.1f);
            jObject.AddPropertyFloat(nameof(EndWidth), EndWidth, 1f);
            jObject.AddPropertyBool(nameof(AutoCalculateCenterPoint), AutoCalculateCenterPoint, true);
            jObject.AddPropertyPointF(nameof(CenterPoint), CenterPoint);
            jObject.AddPropertyFloat(nameof(RelativeHeight), RelativeHeight, 0.04f);
            jObject.AddPropertyFloat(nameof(RelativeWidth), RelativeWidth, 0.4f);
            jObject.AddPropertyEnum(nameof(Skin), Skin);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Format):
                        this.Format = property.DeserializeString();
                        break;

                    case nameof(ShowValue):
                        this.ShowValue = property.DeserializeBool();
                        break;

                    case nameof(TextBrush):
                        this.TextBrush = property.DeserializeBrush();
                        break;

                    case nameof(Font):
                        this.Font = property.DeserializeFont(Font);
                        break;

                    case nameof(CapBrush):
                        this.CapBrush = property.DeserializeBrush();
                        break;

                    case nameof(CapBorderBrush):
                        this.CapBorderBrush = property.DeserializeBrush();
                        break;

                    case nameof(CapBorderWidth):
                        this.CapBorderWidth = property.DeserializeFloat();
                        break;

                    case nameof(OffsetNeedle):
                        this.OffsetNeedle = property.DeserializeFloat();
                        break;

                    case nameof(StartWidth):
                        this.StartWidth = property.DeserializeFloat();
                        break;

                    case nameof(EndWidth):
                        this.EndWidth = property.DeserializeFloat();
                        break;

                    case nameof(AutoCalculateCenterPoint):
                        this.AutoCalculateCenterPoint = property.DeserializeBool();
                        break;

                    case nameof(CenterPoint):
                        this.CenterPoint = property.DeserializePointF();
                        break;

                    case nameof(RelativeHeight):
                        this.RelativeHeight = property.DeserializeFloat();
                        break;

                    case nameof(RelativeWidth):
                        this.RelativeWidth = property.DeserializeFloat();
                        break;

                    case nameof(Skin):
                        this.Skin = property.DeserializeEnum<StiNeedleSkin>();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiNeedle;

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

            // TextAdditionalCategory
            list = new[]
            {
                propHelper.Font(),
                propHelper.Format(),
                propHelper.ShowValue(),
                propHelper.TextBrush()
            };
            checkBoxHelper.Add(StiPropertyCategories.TextAdditional, list);

            // PositionCategory
            list = new[]
            {
                propHelper.AutoCalculateCenterPoint(),
                propHelper.CenterPoint()
            };
            checkBoxHelper.Add(StiPropertyCategories.Position, list);

            // CapNeedleCategory
            list = new[]
            {
                propHelper.CapBrush(),
                propHelper.CapBorderBrush(),
                propHelper.CapBorderWidth()
            };
            checkBoxHelper.Add(StiPropertyCategories.CapNeedle, list);

            // NeedleCategory
            list = new[]
            {
                propHelper.Brush(),
                propHelper.BorderBrush(),
                propHelper.BorderWidth(),
                propHelper.OffsetNeedle(),
                propHelper.StartWidth(),
                propHelper.EndWidth()
            };
            checkBoxHelper.Add(StiPropertyCategories.Needle, list);

            // MiscCategory
            list = new[]
            {
                propHelper.RelativeHeight(),
                propHelper.RelativeWidth()
            };
            checkBoxHelper.Add(StiPropertyCategories.Size, list);

            return checkBoxHelper;
        }
        #endregion

        #region IStiApplyStyleGauge
        public override void ApplyStyle(IStiGaugeStyle style)
        {
            if (this.AllowApplyStyle)
            {
                this.Brush = style.Core.NeedleBrush;
                this.BorderBrush = style.Core.NeedleBorderBrush;
                this.CapBrush = style.Core.NeedleCapBrush;
                this.CapBorderBrush = style.Core.NeedleCapBorderBrush;

                this.BorderWidth = style.Core.NeedleBorderWidth;
                this.CapBorderWidth = style.Core.NeedleCapBorderWidth;

                this.StartWidth = style.Core.NeedleStartWidth;
                this.EndWidth = style.Core.NeedleEndWidth;
                this.RelativeHeight = style.Core.NeedleRelativeHeight;
                this.RelativeWidth = style.Core.NeedleRelativeWith;
            }
        }

        /// <summary>
        /// Gets or sets the format string for the ShowValue property.
        /// </summary>
        [StiSerializable]
        [DefaultValue("{0:F0}")]
        [StiCategory("TextAdditional")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the format string for the ShowValue property.")]
        public string Format { get; set; } = "{0:F0}";

        /// <summary>
        /// Gets or sets a value indicating whether to display a current value of the indicator.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        [StiCategory("TextAdditional")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets a value indicating whether to display a current value of the indicator.")]
        public bool ShowValue { get; set; } = false;

        /// <summary>
        /// The brush, which is used to display text.
        /// </summary>
        [StiSerializable]
        [StiCategory("TextAdditional")]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiBrush TextBrush { get; set; } = new StiSolidBrush(Color.DimGray);

        /// <summary>
        /// Gets or sets font of component.
        /// </summary>
        [StiSerializable]
        [StiCategory("TextAdditional")]
        [StiPropertyLevel(StiLevel.Basic)]
        public Font Font { get; set; } = new Font("Arial", 7f);

        /// <summary>
        /// Gets or sets a brush to fill a cap.
        /// </summary>
        [StiSerializable]
        [StiCategory("Cap Needle")]
        [Description("Gets or sets a brush to fill a cap.")]
        [StiOrder(StiPropertyOrder.AppearanceBrush)]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiBrush CapBrush { get; set; } = new StiSolidBrush(Color.FromArgb(158, 158, 158));

        /// <summary>
        /// Gets or sets the border of the cap.
        /// </summary>
        [StiSerializable]
        [StiCategory("Cap Needle")]
        [StiOrder(StiPropertyOrder.AppearanceBorder)]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the border of the cap.")]
        public StiBrush CapBorderBrush { get; set; } = new StiEmptyBrush();

        /// <summary>
        /// Gets or sets the border thickness of the cap.
        /// </summary>
        [DefaultValue(0f)]
        [StiSerializable]
        [StiCategory("Cap Needle")]
        [StiOrder(200)]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the border thickness of the cap.")]
        public float CapBorderWidth { get; set; } = 0f;

        [DefaultValue(0f)]
        [StiOrder(418)]
        [StiSerializable]
        [StiCategory("Needle")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float OffsetNeedle { get; set; } = 0f;

        [DefaultValue(0f)]
        [StiOrder(419)]
        [StiSerializable]
        [StiCategory("Needle")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float StartWidth { get; set; } = 0.1f;

        [DefaultValue(1f)]
        [StiOrder(420)]
        [StiSerializable]
        [StiCategory("Needle")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float EndWidth { get; set; } = 1f;

        /// <summary>
        /// Gets or sets a value indicating whether to calculate CenterPoint for the indicator automatically.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("Position")]
        [StiPropertyLevel(StiLevel.Basic)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value indicating whether to calculate CenterPoint for the indicator automatically.")]
        public bool AutoCalculateCenterPoint { get; set; } = true;

        /// <summary>
        /// Gets or sets the center coordinate, around which it rotates. The minimum value - Point (0,0), the maximum - Point (1,1).
        /// </summary>
        [StiSerializable]
        [StiCategory("Position")]
        [StiPropertyLevel(StiLevel.Basic)]
        [TypeConverter(typeof(StiPointFConverter))]
        [Description("Gets or sets the center coordinate, around which it rotates. The minimum value - Point (0,0), the maximum - Point (1,1).")]
        public PointF CenterPoint { get; set; } = new PointF();

        /// <summary>
        /// Gets or sets the height factor relative to the height of the basic component.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0.1f)]
        [StiCategory("Size")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the height factor relative to the height of the basic component.")]
        public float RelativeHeight { get; set; } = 0.04f;

        /// <summary>
        /// Gets or sets the width ratio relative to the height of the basic component.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0.6f)]
        [StiCategory("Size")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the width ratio relative to the height of the basic component.")]
        public float RelativeWidth { get; set; } = 0.4f;

        /// <summary>
        /// Gets or sets the skin of the component rendering.
        /// </summary>
        [Browsable(false)]
        [StiSerializable]
        [DefaultValue(StiNeedleSkin.SimpleNeedle)]
        [StiPropertyLevel(StiLevel.Basic)]
        [TypeConverter(typeof(StiEnumConverter))]
        public StiNeedleSkin Skin { get; set; } = StiNeedleSkin.SimpleNeedle;

        [Browsable(false)]
        [StiSerializable]
        [DefaultValue(null)]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiGaugeElementSkin CustomSkin { get; set; }
        #endregion

        #region Properties override
        [StiCategory("Needle")]
        public override StiBrush Brush
        {
            get
            {
                return base.Brush;
            }
            set
            {
                base.Brush = value;
            }
        }

        [StiCategory("Needle")]
        public override StiBrush BorderBrush
        {
            get
            {
                return base.BorderBrush;
            }
            set
            {
                base.BorderBrush = value;
            }
        }

        [StiCategory("Needle")]
        public override float BorderWidth
        {
            get
            {
                return base.BorderWidth;
            }
            set
            {
                base.BorderWidth = value;
            }
        }

        public override StiGaugeElemenType ElementType => StiGaugeElemenType.RadialElement;

        public override string LocalizeName => "Needle";

        [Browsable(false)]
        public override StiPlacement Placement
        {
            get
            {
                return base.Placement;
            }
            set
            {
                base.Placement = value;
            }
        }
        #endregion

        #region Methods override

        public override StiGaugeElement CreateNew() => new StiNeedle();

        protected internal override void DrawElement(StiGaugeContextPainter context)
        {
            var radialScale = this.Scale as StiRadialScale;
            if (radialScale == null) return;

            var center = Scale.barGeometry.Center;
            var actualSize = new SizeF(Scale.barGeometry.Diameter * this.RelativeWidth, Scale.barGeometry.Diameter * this.RelativeHeight);

            float offsetX;
            float offsetY;

            if (actualSize.Width > actualSize.Height)
            {
                offsetX = offsetY = actualSize.Height / 2;
            }
            else
            {
                offsetX = actualSize.Width / 2;
                offsetY = actualSize.Height / 2;
            }

            float x = center.X - offsetX;
            float y = center.Y - offsetY;

            var rect = new RectangleF(new PointF(x, y), actualSize);

            float angle;
            var nullValue = GetActualValue();
            if (nullValue == null) return;

            float value = nullValue.GetValueOrDefault();

            var angleValue = radialScale.GetCurrentAngle(value);
            if (angleValue > (radialScale.SweepAngle + radialScale.StartAngle))
            {
                angle = Scale.IsReversed ? radialScale.GetCurrentAngle(radialScale.Minimum) : radialScale.GetCurrentAngle(radialScale.Maximum);
            }
            else
            {
                angle = Scale.IsReversed ? (radialScale.SweepAngle - angleValue) : angleValue;
            }

            float rotationAngle = -radialScale.GetPosition(value) * radialScale.SweepAngle;

            if (Scale.IsReversed)
                rotationAngle = -rotationAngle;

            if (context.Gauge.IsAnimation)
            {
                var animation = new StiRotationAnimation(rotationAngle, 0, center, StiGaugeHelper.GlobalDurationElement, TimeSpan.Zero);
                animation.Id = Scale.SeriesKey?.ToString() + "_needle_" + radialScale.Items.IndexOf(this);
                animation.ApplyPreviousAnimation(context.Gauge.PreviousAnimations);

                this.Animation = animation;
            }

            var skin = this.GetActualSkin();
            skin.Draw(context, this, rect, angle, center);
        }

        protected internal override void InteractiveClick(RectangleF rect, Point p)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Methods
        private StiGaugeElementSkin GetActualSkin()
        {
            return (this.CustomSkin != null) ? this.CustomSkin : StiGaugeSkinHelper.GetNeedleIndicatorSkin(this.Skin);
        }
        #endregion
    }
}