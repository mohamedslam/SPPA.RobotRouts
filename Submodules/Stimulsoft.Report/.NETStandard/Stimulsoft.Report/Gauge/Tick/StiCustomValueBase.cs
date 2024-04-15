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

using System;
using System.ComponentModel;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Design;
using Stimulsoft.Base;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.PropertyGrid;

namespace Stimulsoft.Report.Components.Gauge
{
    public abstract class StiCustomValueBase : 
        ICloneable,
        IStiPropertyGridObject
    {
        #region IStiJsonReportObject
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyFloat(nameof(Value), Value);
            jObject.AddPropertyFloatNullable(nameof(Offset), Offset, null);

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Value):
                        this.Value = property.DeserializeFloat();
                        break;

                    case nameof(Offset):
                        this.Offset = property.DeserializeFloatNullable();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public abstract StiComponentId ComponentId { get; }

        [Browsable(false)]
        public string PropName => null;

        public abstract StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level);

        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid) => null;
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public virtual object Clone() => this.MemberwiseClone();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets value corresponds to this tick element.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0f)]
        [StiCategory("Value")]
        [Description("Gets or sets value corresponds to this tick element.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float Value { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the placement of the component.
        /// </summary>
        [StiSerializable]
        [DefaultValue(null)]
        [TypeConverter(typeof(StiNullableEnumConverter))]
        [StiCategory("Tick")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the placement of the component.")]
        public StiPlacement? Placement { get; set; }

        /// <summary>
        /// Gets or sets the offset ratio of an item.
        /// </summary>
        [StiSerializable]
        [DefaultValue(null)]
        [StiCategory("Value")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the offset ratio of an item.")]
        public float? Offset { get; set; }
        #endregion

        #region Properties abstract
        [Browsable(false)]
        public abstract string LocalizedName { get; }
        #endregion

        #region Methods virtual
        public virtual StiCustomValueBase CreateNew() => throw new NotImplementedException();
        #endregion
    }
}