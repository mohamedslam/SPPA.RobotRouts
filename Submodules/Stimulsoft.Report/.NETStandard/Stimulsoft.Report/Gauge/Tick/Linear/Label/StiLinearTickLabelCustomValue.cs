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
    public sealed class StiLinearTickLabelCustomValue :
        StiCustomValueBase
    {
        #region IStiJsonReportObject
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyString(nameof(Text), Text);

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
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiLinearTickLabelCustomValue;

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

            return checkBoxHelper;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets text corresponds to this tick label.
        /// </summary>
        [StiSerializable]
        [DefaultValue(null)]
        [StiCategory("Value")]
        [Description("Gets or sets text corresponds to this tick label.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public string Text { get; set; }
        #endregion

        #region Properties override
        public override string LocalizedName => "LinearTickLabelCustom";
        #endregion

        #region Methods override
        public override string ToString() => $"Value={this.Value}, Text={this.Text}";
        
        public override StiCustomValueBase CreateNew() => new StiLinearTickLabelCustomValue();
        #endregion

        public StiLinearTickLabelCustomValue()
        {

        }

        public StiLinearTickLabelCustomValue(float value, string text)
        {
            this.Value = value;
            this.Text = text;
        }

        public StiLinearTickLabelCustomValue(float value, string text, 
            float? offset, StiPlacement? placement)
        {
            this.Value = value;
            this.Text = text;
            this.Offset = offset;
            this.Placement = placement;
        }
    }
}
