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
using Stimulsoft.Report.Gauge.Helpers;
using System;
using System.ComponentModel;
using System.Drawing;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components.Gauge.Primitives
{
    public abstract class StiTickMarkBase : StiTickBase
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);
            
            jObject.AddPropertyFloat(nameof(RelativeHeight), RelativeHeight, 0.1f);
            jObject.AddPropertyFloat(nameof(RelativeWidth), RelativeWidth, 0.1f);
            jObject.AddPropertyEnum(nameof(Skin), Skin);
            jObject.AddPropertyBrush(nameof(Brush), Brush);
            jObject.AddPropertyBrush(nameof(BorderBrush), BorderBrush);
            jObject.AddPropertyFloat(nameof(BorderWidth), BorderWidth, 1f);
            
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
                        this.RelativeHeight = property.DeserializeFloat();
                        break;

                    case nameof(RelativeWidth):
                        this.RelativeWidth = property.DeserializeFloat();
                        break;

                    case nameof(Skin):
                        this.Skin = property.DeserializeEnum<StiTickMarkSkin>();
                        break;

                    case nameof(Brush):
                        this.Brush = property.DeserializeBrush();
                        break;

                    case nameof(BorderBrush):
                        this.BorderBrush = property.DeserializeBrush();
                        break;

                    case nameof(BorderWidth):
                        this.BorderWidth = property.DeserializeFloat();
                        break;
                }
            }
        }
        #endregion

        #region ICloneable
        public override object Clone()
        {
            var tickMarker = (StiTickMarkBase)base.Clone();

            tickMarker.Brush = (StiBrush)this.Brush.Clone();
            tickMarker.BorderBrush = (StiBrush)this.BorderBrush.Clone();

            return tickMarker;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the height factor relative to the height of the basic component.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0.1f)]
        [StiCategory("Tick")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the height factor relative to the height of the basic component.")]
        public float RelativeHeight { get; set; } = 0.1f;

        /// <summary>
        /// Gets or sets the width ratio relative to the height of the basic component.
        /// </summary>
        [DefaultValue(0.1f)]
        [StiSerializable]
        [StiCategory("Tick")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the width ratio relative to the height of the basic component.")]
        public float RelativeWidth { get; set; } = 0.1f;

        /// <summary>
        /// Gets or sets the skin of the component rendering.
        /// </summary>
        [DefaultValue(StiTickMarkSkin.Rectangle)]
        [StiSerializable]
        [StiCategory("Tick")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the skin of the component rendering.")]
        public StiTickMarkSkin Skin { get; set; } = StiTickMarkSkin.Rectangle;

        [Browsable(false)]
        [DefaultValue(null)]
        [StiSerializable]
        public StiGaugeElementSkin CustomSkin { get; set; }

        /// <summary>
        /// Gets or sets a brush to fill a component.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets a brush to fill a component.")]
        [StiOrder(StiPropertyOrder.AppearanceBrush)]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiBrush Brush { get; set; } = new StiSolidBrush(Color.White);

        /// <summary>
        /// Gets or sets the border of the component.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceBorder)]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the border of the component.")]
        public StiBrush BorderBrush { get; set; } = new StiSolidBrush(Color.FromArgb(89, 87, 87));

        /// <summary>
        /// Gets or sets the border thickness of the component.
        /// </summary>
        [DefaultValue(1f)]
        [StiSerializable]
        [StiCategory("Appearance")]
        [StiOrder(200)]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the border thickness of the component.")]
        public float BorderWidth { get; set; } = 1f;
        #endregion

        #region Methods
        protected StiGaugeElementSkin GetActualSkin()
        {
            return (this.CustomSkin != null) ? this.CustomSkin : StiGaugeSkinHelper.GetTickMarkSkin(this.Skin);
        }

        protected float GetRelativeWidth(float? value)
        {
            return (value == null) ? this.RelativeWidth : value.Value;
        }

        protected float GetRelativeHeight(float? value)
        {
            return (value == null) ? this.RelativeHeight : value.Value;
        }
        #endregion
    }
}
