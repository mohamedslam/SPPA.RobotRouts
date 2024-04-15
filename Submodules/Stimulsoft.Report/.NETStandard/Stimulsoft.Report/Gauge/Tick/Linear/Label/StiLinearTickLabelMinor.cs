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
using Stimulsoft.Report.Components.Gauge.Primitives;
using Stimulsoft.Report.Gauge;
using System.Collections.Generic;
using System.ComponentModel;
using Stimulsoft.Report.PropertyGrid;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components.Gauge
{
    public class StiLinearTickLabelMinor :
        StiLinearTickLabelBase
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            jObject.AddPropertyBool(nameof(SkipMajorValues), SkipMajorValues, true);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(SkipMajorValues):
                        this.SkipMajorValues = property.DeserializeBool();
                        break;                        
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiLinearTickLabelMinor;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var checkBoxHelper = new StiPropertyCollection();

            // ValueCategory
            var list = new[]
            {
                propHelper.NullableMaximumValue(),
                propHelper.NullableMinimumValue(),
                propHelper.Offset(),
                propHelper.SkipIndices(),
                propHelper.SkipValues()
            };
            checkBoxHelper.Add(StiPropertyCategories.Value, list);

            // TextAdditionalCategory
            list = new[]
            {
                propHelper.Font(),
                propHelper.SkipMajorValues(),
                propHelper.TextBrush(),
                propHelper.TextFormatStr(),
            };
            checkBoxHelper.Add(StiPropertyCategories.TextAdditional, list);

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
                this.TextBrush = style.Core.TickLabelMinorTextBrush;
                this.Font = style.Core.TickLabelMinorFont;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets whether to pass major values. 
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        [StiCategory("TextAdditional")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets whether to pass major values.")]
        public bool SkipMajorValues { get; set; } = true;
        #endregion

        #region Properties override
        protected override bool IsSkipMajorValues => this.SkipMajorValues;

        public override string LocalizeName => "LinearTickLabelMinor";
        #endregion

        #region Methods override
        public override StiGaugeElement CreateNew() => new StiLinearTickLabelMinor();

        protected override Dictionary<float, float> GetPointCollection() => GetMinorCollections();
        #endregion
    }
}
