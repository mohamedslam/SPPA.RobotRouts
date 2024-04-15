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
using Stimulsoft.Report.Gauge;
using System.ComponentModel;
using Stimulsoft.Report.PropertyGrid;

namespace Stimulsoft.Report.Components.Gauge
{
    public sealed class StiRadialTickMarkCustomValue :
        StiCustomValueBase
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyIdent("Ident", this.GetType().Name);
            
            jObject.AddPropertyFloatNullable(nameof(RelativeHeight), RelativeHeight, null);
            jObject.AddPropertyFloatNullable(nameof(RelativeWidth), RelativeWidth, null); 
                jObject.AddPropertyFloatNullable(nameof(OffsetAngle), OffsetAngle, null);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(RelativeHeight):
                        this.RelativeHeight = property.DeserializeFloatNullable();
                        break;

                    case nameof(RelativeWidth):
                        this.RelativeWidth = property.DeserializeFloatNullable();
                        break;

                    case nameof(OffsetAngle):
                        this.OffsetAngle = property.DeserializeFloatNullable();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiRadialTickMarkCustomValue;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var checkBoxHelper = new StiPropertyCollection();
            
            // ValueCategory
            var list = new[]
            {
                propHelper.NullableOffset(),
                propHelper.NullableOffsetAngle(),
                propHelper.ValueF()
            };
            checkBoxHelper.Add(StiPropertyCategories.Value, list);

            // TickCategory
            list = new[]
            {
                propHelper.NullablePlacement(),
                propHelper.NullableRelativeHeight(),
                propHelper.NullableRelativeWidth()
            };
            checkBoxHelper.Add(StiPropertyCategories.Tick, list);
            
            // AppearanceCategory
            list = new[]
            {
                propHelper.Brush(),
                propHelper.BorderBrush(),
                propHelper.NullableBorderWidth()
            };
            checkBoxHelper.Add(StiPropertyCategories.Appearance, list);

            return checkBoxHelper;
        }
        #endregion

        #region ICloneable
        public override object Clone()
        {
            var clone = (StiRadialTickMarkCustomValue)base.Clone();

            clone.brush = (StiBrush)this.brush.Clone();
            clone.borderBrush = (StiBrush)this.borderBrush.Clone();

            return clone;
        }
        #endregion

        #region Fields
        internal bool useBrush;
        internal bool useBorderBrush;
        internal bool useBorderWidth;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the width ratio relative to the height of the basic component.
        /// </summary>
        [StiSerializable]
        [DefaultValue(null)]
        [StiCategory("Tick")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the width ratio relative to the height of the basic component.")]
        public float? RelativeWidth { get; set; }

        /// <summary>
        /// Gets or sets the height factor relative to the height of the basic component.
        /// </summary>
        [StiSerializable]
        [DefaultValue(null)]
        [StiCategory("Tick")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the height factor relative to the height of the basic component.")]
        public float? RelativeHeight { get; set; }

        /// <summary>
        /// Gets or sets an additional rotation angle.
        /// </summary>
        [StiSerializable]
        [DefaultValue(null)]
        [StiCategory("Value")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets an additional rotation angle.")]
        public float? OffsetAngle { get; set; }

        /// <summary>
        /// Gets or sets the skin of the component rendering.
        /// </summary>
        [StiSerializable]
        [DefaultValue(null)]
        [Browsable(false)]
        [StiCategory("Tick")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the skin of the component rendering.")]
        public StiGaugeElementSkin Skin { get; set; }

        private StiBrush brush = new StiEmptyBrush();
        /// <summary>
        /// Gets or sets a brush to fill a component.
        /// </summary>
        [StiSerializable]
        [StiCategory("Behavior")]
        [Description("Gets or sets a brush to fill a component.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiBrush Brush
        {
            get
            {
                return this.brush;
            }
            set
            {
                this.brush = value;
                this.useBrush = true;
            }
        }

        private StiBrush borderBrush = new StiEmptyBrush();
        /// <summary>
        /// Gets or sets the border of the component.
        /// </summary>
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the border of the component.")]
        public StiBrush BorderBrush
        {
            get
            {
                return this.borderBrush;
            }
            set
            {
                this.borderBrush = value;
                this.useBorderBrush = true;
            }
        }

        private float? borderWidth = null;
        /// <summary>
        /// Gets or sets the border thickness of the component.
        /// </summary>
        [DefaultValue(null)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the border thickness of the component.")]
        public float? BorderWidth
        {
            get
            {
                return this.borderWidth;
            }
            set
            {
                this.borderWidth = value;
                this.useBorderWidth = true;
            }
        }
        #endregion

        #region Properties override
        public override string LocalizedName => "RadialTickMarkCustom";
        #endregion

        #region Methods override
        public override string ToString() => $"Value={this.Value}";
        
        public override StiCustomValueBase CreateNew() => new StiRadialTickMarkCustomValue();
        #endregion

        public StiRadialTickMarkCustomValue()
        {

        }

        public StiRadialTickMarkCustomValue(float value)
        {
            this.Value = value;
        }

        public StiRadialTickMarkCustomValue(float value, float? offset,
            float? relativeWidth, float? relativeHeight,
            float? offsetAngle, StiPlacement? placement, StiGaugeElementSkin skin)
        {
            this.Value = value;
            this.Offset = offset;
            this.RelativeWidth = relativeWidth;
            this.RelativeHeight = relativeHeight;
            this.OffsetAngle = offsetAngle;
            this.Placement = placement;
            this.Skin = skin;
        }

        public StiRadialTickMarkCustomValue(float value, float? offset,
            float? relativeWidth, float? relativeHeight,
            float? offsetAngle, StiPlacement? placement, 
            StiBrush brush, StiBrush borderBrush,
            float? borderWidth, StiGaugeElementSkin skin)
        {
            this.Value = value;
            this.Offset = offset;
            this.RelativeWidth = relativeWidth;
            this.RelativeHeight = relativeHeight;
            this.OffsetAngle = offsetAngle;
            this.Placement = placement;
            this.Brush = brush;
            this.BorderBrush = borderBrush;
            this.BorderWidth = borderWidth;
            this.Skin = skin;
        }
    }
}