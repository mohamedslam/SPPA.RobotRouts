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
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components.Gauge
{
    public sealed class StiRadialTickLabelCustomValue :
        StiCustomValueBase
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyString(nameof(Text), Text);
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
                    case nameof(Text):
                        this.Text = property.DeserializeString();
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
        public override StiComponentId ComponentId => StiComponentId.StiRadialTickLabelCustomValue;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var checkBoxHelper = new StiPropertyCollection();

            // ValueCategory
            var list = new[]
            {
                propHelper.NullableOffset(),
                propHelper.TextStr(),
                propHelper.ValueF()
            };
            checkBoxHelper.Add(StiPropertyCategories.Value, list);
            
            // TickCategory
            list = new[]
            {
                propHelper.NullablePlacement()
            };
            checkBoxHelper.Add(StiPropertyCategories.Tick, list);

            // TextAdditionalCategory
            list = new[]
            {
                propHelper.NullableLabelRotationMode(),
                propHelper.NullableOffsetAngle()
            };
            checkBoxHelper.Add(StiPropertyCategories.TextAdditional, list);

            return checkBoxHelper;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets text corresponds to this tick label.
        /// </summary>
        [DefaultValue(null)]
        [StiSerializable]
        [StiCategory("Value")]
        [Description("Gets or sets text corresponds to this tick label.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets an additional rotation angle.
        /// </summary>
        [DefaultValue(null)]
        [StiSerializable]
        [StiCategory("TextAdditional")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets an additional rotation angle.")]
        public float? OffsetAngle { get; set; }

        /// <summary>
        /// Gets or sets the rotation mode of labels.
        /// </summary>
        [DefaultValue(null)]
        [StiSerializable]
        [StiCategory("TextAdditional")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the rotation mode of labels.")]
        public StiLabelRotationMode? LabelRotationMode { get; set; }
        #endregion

        #region Properties override
        public override string LocalizedName => "RadialTickLabelCustom";
        #endregion

        #region Methods override
        public override string ToString()
        {
            return string.Format("Value={0}, Text={1}", this.Value, this.Text);
        }

        public override StiCustomValueBase CreateNew() => new StiRadialTickLabelCustomValue();
        #endregion

        #region this
        public StiRadialTickLabelCustomValue()
        {

        }

        public StiRadialTickLabelCustomValue(float value, string text)
        {
            this.Value = value;
            this.Text = text;
        }

        public StiRadialTickLabelCustomValue(float value, string text, float offset)
        {
            this.Value = value;
            this.Text = text;
            this.Offset = offset;
        }

        public StiRadialTickLabelCustomValue(float value, string text, float? offset, 
            float? offsetAngle, StiLabelRotationMode? labelRotationMode, 
            StiPlacement? placement)
        {
            this.Value = value;
            this.Text = text;
            this.Offset = offset;
            this.OffsetAngle = offsetAngle;
            this.LabelRotationMode = labelRotationMode;
            this.Placement = placement;
        }
        #endregion
    }
}
