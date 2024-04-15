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

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Components.Gauge.Primitives
{
    public abstract class StiMarkerBase :
        StiIndicatorBase,
        IStiGaugeMarker
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);
            
            jObject.AddPropertyFloat(nameof(Offset), Offset);
            jObject.AddPropertyFloat(nameof(RelativeWidth), RelativeWidth, 0.05f);
            jObject.AddPropertyFloat(nameof(RelativeHeight), RelativeHeight, 0.05f);
            jObject.AddPropertyEnum(nameof(Skin), Skin);
            jObject.AddPropertyString(nameof(Format), Format);
            jObject.AddPropertyBool(nameof(ShowValue), ShowValue);
            jObject.AddPropertyBrush(nameof(TextBrush), TextBrush);
            jObject.AddPropertyFontArial7(nameof(Font), Font);
            
            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Offset):
                        this.Offset = property.DeserializeFloat();
                        break;

                    case nameof(RelativeWidth):
                        this.RelativeWidth = property.DeserializeFloat();
                        break;

                    case nameof(RelativeHeight):
                        this.RelativeHeight = property.DeserializeFloat();
                        break;

                    case nameof(Skin):
                        this.Skin = property.DeserializeEnum<StiMarkerSkin>();
                        break;

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
                }
            }
        }
        #endregion

        #region ICloneable
        public override object Clone()
        {
            var indicator = (StiMarkerBase)base.Clone();
            indicator.TextBrush = (StiBrush)this.TextBrush.Clone();

            return indicator;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets offset of the scale object from the scale bar.
        /// </summary>
        [DefaultValue(0f)]
        [StiSerializable]
        [StiCategory("Indicator")]
        [Description("Gets or sets offset of the scale object from the scale bar.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float Offset { get; set; } = 0f;

        /// <summary>
        /// Gets or sets relative width of the indicator. It is given as part of the scale dimension.
        /// </summary>
        [DefaultValue(0.05f)]
        [StiSerializable]
        [StiCategory("Indicator")]
        [Description("Gets or sets relative width of the indicator. It is given as part of the scale dimension.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float RelativeWidth { get; set; } = 0.05f;

        /// <summary>
        /// Gets or sets relative height of the indicator. It is given as part of the scale dimension.
        /// </summary>
        [DefaultValue(0.05f)]
        [StiSerializable]
        [StiCategory("Indicator")]
        [Description("Gets or sets relative height of the indicator. It is given as part of the scale dimension.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float RelativeHeight { get; set; } = 0.05f;

        /// <summary>
        /// Gets or sets the skin of the component rendering.
        /// </summary>
        [DefaultValue(StiMarkerSkin.Diamond)]
        [StiSerializable]
        [StiCategory("Indicator")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the skin of the component rendering.")]
        public StiMarkerSkin Skin { get; set; } = StiMarkerSkin.Diamond;

        [Browsable(false)]
        [DefaultValue(null)]
        [StiSerializable]
        public StiGaugeElementSkin CustomSkin { get; set; }
        #endregion

        #region IStiGaugeMarker.Properties
        /// <summary>
        /// Gets or sets the format string for the ShowValue property.
        /// </summary>
        [DefaultValue("{0:F0}")]
        [StiSerializable]
        [StiCategory("TextAdditional")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the format string for the ShowValue property.")]
        public string Format { get; set; } = "{0:F0}";

        /// <summary>
        /// Gets or sets a value indicating whether to display a current value of the indicator.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("TextAdditional")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
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
        public Font Font { get; set; } = new Font("Arial", 7f);
        #endregion

        #region Methods
        protected StiGaugeElementSkin GetActualSkin()
        {
            return (this.CustomSkin != null) ? this.CustomSkin : StiGaugeSkinHelper.GetMarkerSkin(this.Skin);
        }
        #endregion
    }
}
