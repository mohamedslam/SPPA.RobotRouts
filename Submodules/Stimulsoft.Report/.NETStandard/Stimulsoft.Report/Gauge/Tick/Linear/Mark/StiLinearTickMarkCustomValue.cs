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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Gauge;
using System.ComponentModel;
using Stimulsoft.Report.PropertyGrid;

namespace Stimulsoft.Report.Components.Gauge
{
    public sealed class StiLinearTickMarkCustomValue :
        StiCustomValueBase
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            jObject.AddPropertyFloatNullable(nameof(RelativeHeight), RelativeHeight, null);
            jObject.AddPropertyFloatNullable(nameof(RelativeWidth), RelativeWidth, null);

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
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiLinearTickMarkCustomValue;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var checkBoxHelper = new StiPropertyCollection();
            
            // ValueCategory
            var list = new[]
            {
                propHelper.NullableOffset(),
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

            return checkBoxHelper;
        }
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
        /// Gets or sets the skin of the component rendering.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [StiCategory("Tick")]
        [DefaultValue(null)]
        [Description("Gets or sets the skin of the component rendering.")]
        public StiGaugeElementSkin Skin { get; set; }
        #endregion

        #region Properties override
        public override string LocalizedName => "LinearTickMarkCustom";
        #endregion

        #region Methods override
        public override string ToString() => $"Value={this.Value}";

        public override StiCustomValueBase CreateNew() => new StiLinearTickMarkCustomValue();
        #endregion

        public StiLinearTickMarkCustomValue()
        {

        }

        public StiLinearTickMarkCustomValue(float value)
        {
            this.Value = value;
        }

        public StiLinearTickMarkCustomValue(float value, float? offset,
            float? relativeWidth, float? relativeHeight,
            StiPlacement? placement, StiGaugeElementSkin skin)
        {
            this.Value = value;
            this.Offset = offset;
            this.RelativeWidth = relativeWidth;
            this.RelativeHeight = relativeHeight;
            this.Placement = placement;
            this.Skin = skin;
        }
    }
}
