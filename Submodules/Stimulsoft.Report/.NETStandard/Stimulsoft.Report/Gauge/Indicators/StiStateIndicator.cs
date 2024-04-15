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
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components.Gauge.Primitives;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Gauge.Collections;
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
    public class StiStateIndicator :
        StiIndicatorBase,
        IStiGaugeMarker
    {
        #region IStiJsonReportObject.override

        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            jObject.AddPropertyStringNullOrEmpty(nameof(Format), Format);
            jObject.AddPropertyBool(nameof(ShowValue), ShowValue);
            jObject.AddPropertyBrush(nameof(TextBrush), TextBrush);
            jObject.AddPropertyFontArial7(nameof(Font), Font);
            jObject.AddPropertyFloat(nameof(Left), Left);
            jObject.AddPropertyFloat(nameof(Top), Top);
            jObject.AddPropertyFloat(nameof(RelativeHeight), RelativeHeight, 0.05f);
            jObject.AddPropertyFloat(nameof(RelativeWidth), RelativeWidth, 0.05f);
            jObject.AddPropertyEnum(nameof(Skin), Skin);

            if (mode == StiJsonSaveMode.Report)
            {
                jObject.AddPropertyJObject(nameof(Filters), Filters.SaveToJsonObject(mode));
            }

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

                    case nameof(Left):
                        this.Left = property.DeserializeFloat();
                        break;

                    case nameof(Top):
                        this.Top = property.DeserializeFloat();
                        break;                    

                    case nameof(RelativeHeight):
                        this.RelativeHeight = property.DeserializeFloat();
                        break;

                    case nameof(RelativeWidth):
                        this.RelativeWidth = property.DeserializeFloat();
                        break;

                    case nameof(Skin):
                        this.Skin = property.DeserializeEnum<StiStateSkin>();
                        break;

                    case nameof(Filters):
                        this.Filters.LoadFromJsonObject((JObject)property.Value);
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiStateIndicator;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var checkBoxHelper = new StiPropertyCollection();
            StiPropertyObject[] list;

            // ValueCategory
            list = new[]
            {
                propHelper.Value()
            };
            checkBoxHelper.Add(StiPropertyCategories.Value, list);

            // IndicatorCategory
            list = new[]
            {
                propHelper.Placement(),
                propHelper.Skin()
            };
            checkBoxHelper.Add(StiPropertyCategories.Indicator, list);

            // TextAdditionalCategory
            list = new[]
            {
                propHelper.Font(),
                propHelper.Format(),
                propHelper.ShowValue(),
                propHelper.TextBrush()
            };
            checkBoxHelper.Add(StiPropertyCategories.TextAdditional, list);

            // TextAdditionalCategory
            list = new[]
            {
                propHelper.LeftF(),
                propHelper.RelativeHeight(),
                propHelper.RelativeWidth(),
                propHelper.TopF()
            };
            checkBoxHelper.Add(StiPropertyCategories.Position, list);

            // AppearanceCategory
            list = new[]
            {
                propHelper.Brush(),
                propHelper.BorderBrush(),
                propHelper.BorderWidth()
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

        #region Fields
        private StiStateIndicatorFilter lastFilter;
        #endregion

        #region IStiGaugeMarker.Properties
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
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets a value indicating whether to display a current value of the indicator.")]
        public bool ShowValue { get; set; }

        /// <summary>
        /// The brush, which is used to display text.
        /// </summary>
        [StiSerializable]
        [StiCategory("TextAdditional")]
        [Description("The brush, which is used to display text.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiBrush TextBrush { get; set; } = new StiSolidBrush(Color.DimGray);

        /// <summary>
        /// Gets or sets font of component.
        /// </summary>
        [StiSerializable]
        [StiCategory("TextAdditional")]
        [Description("Gets or sets font of component.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public Font Font { get; set; } = new Font("Atial", 7f);
        #endregion

        #region Properties override
        public override StiGaugeElemenType ElementType => StiGaugeElemenType.All;

        public override string LocalizeName => "StateIndicator";
        #endregion

        #region Properties
        [Browsable(false)]
        [StiSerializable(StiSerializationVisibility.List)]
        public StiFilterCollection Filters { get; set; } = new StiFilterCollection();

        /// <summary>
        /// Gets or sets X coordinate of the indicator relative to the scale dimension.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0f)]
        [StiCategory("Position")]
        [Description("Gets or sets X coordinate of the indicator relative to the scale dimension.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float Left { get; set; } = 0f;

        /// <summary>
        /// Gets or sets Y coordinate of the indicator relative to the scale dimension.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0f)]
        [StiCategory("Position")]
        [Description("Gets or sets Y coordinate of the indicator relative to the scale dimension.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float Top { get; set; } = 0f;

        /// <summary>
        /// Gets or sets relative width of the indicator.  It is given as part of the scale dimension.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0.05f)]
        [StiCategory("Position")]
        [Description("Gets or sets relative width of the indicator.  It is given as part of the scale dimension.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float RelativeWidth { get; set; } = 0.05f;

        /// <summary>
        /// Gets or sets relative height of the indicator.  It is given as part of the scale dimension.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0.05f)]
        [StiCategory("Position")]
        [Description("Gets or sets relative height of the indicator.  It is given as part of the scale dimension.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float RelativeHeight { get; set; } = 0.05f;

        /// <summary>
        /// Gets or sets the skin of the component rendering.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiStateSkin.Ellipse)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Indicator")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the skin of the component rendering.")]
        public StiStateSkin Skin { get; set; } = StiStateSkin.Ellipse;

        [Browsable(false)]
        [StiSerializable]
        [DefaultValue(null)]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiGaugeElementSkin CustomSkin { get; set; }
        #endregion

        #region Methods override

        public override StiGaugeElement CreateNew() => new StiStateIndicator();

        protected override void OnValueChanged()
        {
            if (this.Filters != null)
            {
                StiStateIndicatorFilter currentFilter = null;

                float value = GetActualValue().GetValueOrDefault();
                for (int index = 0; index < this.Filters.Count; index++)
                {
                    if (value >= this.Filters[index].StartValue && value <= this.Filters[index].EndValue)
                    {
                        currentFilter = this.Filters[index];
                        break;
                    }
                }

                if (this.lastFilter != currentFilter)
                {
                    this.lastFilter = currentFilter;

                    if (currentFilter != null)
                    {
                        this.Brush = currentFilter.Brush;
                        this.BorderBrush = currentFilter.BorderBrush;
                    }
                }
            }
        }

        protected internal override void InteractiveClick(RectangleF rect, Point p)
        {

        }

        protected internal override void DrawElement(StiGaugeContextPainter context)
        {
            var size = this.Scale.barGeometry.Size;
            var rectF = context.Rect;
            var rect = new RectangleF(rectF.X + size.Width * this.Left, rectF.Y + size.Height * this.Top,
                size.Width * this.RelativeWidth, size.Height * this.RelativeHeight);

            GetActualSkin().Draw(context, this, rect, null, null);
        }
        #endregion

        #region Methods
        protected StiGaugeElementSkin GetActualSkin()
        {
            return (this.CustomSkin != null)
                ? this.CustomSkin
                : StiGaugeSkinHelper.GetStateIndicatorSkin(this.Skin);
        }
        #endregion
    }
}