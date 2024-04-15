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
using System;
using System.ComponentModel;
using Stimulsoft.Report.PropertyGrid;

namespace Stimulsoft.Report.Components.Gauge
{
    public abstract class StiIndicatorRangeInfo :
        ICloneable,
        IStiPropertyGridObject
    {
        #region IStiJsonReportObject.override

        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();            
            
            jObject.AddPropertyFloat(nameof(Value), Value);

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
        public object Clone() => this.MemberwiseClone();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the value of the indicator.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0f)]
        [StiCategory("Value")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the value of the indicator.")]
        public float Value { get; set; } = 0f;
        #endregion

        #region Properties abstract
        [Browsable(false)]
        internal abstract StiBarRangeListType RangeListType { get; }
        #endregion

        #region Methods virtual
        public virtual StiIndicatorRangeInfo CreateNew() => throw new NotImplementedException();
        #endregion
    }
}