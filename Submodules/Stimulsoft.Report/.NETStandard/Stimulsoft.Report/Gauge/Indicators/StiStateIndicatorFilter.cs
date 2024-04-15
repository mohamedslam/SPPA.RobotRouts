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
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;

namespace Stimulsoft.Report.Components.Gauge
{
    public class StiStateIndicatorFilter : 
        ICloneable, 
        IStiPropertyGridObject
    {
        #region IStiJsonReportObject.override
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            jObject.AddPropertyFloat(nameof(StartValue), StartValue);
            jObject.AddPropertyFloat(nameof(EndValue), EndValue);
            jObject.AddPropertyBrush(nameof(Brush), Brush);
            jObject.AddPropertyBrush(nameof(BorderBrush), BorderBrush);

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(StartValue):
                        this.StartValue = property.DeserializeFloat();
                        break;

                    case nameof(EndValue):
                        this.EndValue = property.DeserializeFloat();
                        break;
                        
                    case nameof(Brush):
                        this.Brush = property.DeserializeBrush();
                        break;

                    case nameof(BorderBrush):
                        this.BorderBrush = property.DeserializeBrush();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public StiComponentId ComponentId => StiComponentId.StiStateIndicatorFilter;

        [Browsable(false)]
        public string PropName => null;

        public StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var checkBoxHelper = new StiPropertyCollection();

            // ValueCategory
            var list = new[]
            {
                propHelper.EndValue(),
                propHelper.StartValue()
            };
            checkBoxHelper.Add(StiPropertyCategories.Value, list);

            // AppearanceCategory
            list = new[]
            {
                propHelper.Brush(),
                propHelper.BorderBrush()
            };
            checkBoxHelper.Add(StiPropertyCategories.Appearance, list);

            return checkBoxHelper;
        }

        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid) => null;
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone() => this.MemberwiseClone();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets start value of the filter.
        /// </summary>   
        [StiSerializable]
        [DefaultValue(0f)]
        [StiCategory("Value")]
        [Description("Gets or sets start value of the filter.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float StartValue { get; set; } = 0f;

        /// <summary>
        /// Gets or sets end value of the filter.
        /// </summary>     
        [StiSerializable]
        [DefaultValue(0f)]
        [StiCategory("Value")]
        [Description("Gets or sets end value of the filter.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float EndValue { get; set; } = 0f;

        /// <summary>
        /// Gets or sets a brush to fill a component.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets a brush to fill a component.")]
        [StiOrder(StiPropertyOrder.AppearanceBrush)]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiBrush Brush { get; set; } = new StiEmptyBrush();

        /// <summary>
        /// Gets or sets the border of the component.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceBorder)]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the border of the component.")]
        public StiBrush BorderBrush { get; set; } = new StiEmptyBrush();
        #endregion

        #region Methods override
        public override string ToString()
        {
            return string.Format("StartValue={0}, EndValue={1}", this.StartValue, this.EndValue);
        }
        #endregion
    }
}