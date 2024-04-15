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
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Gauge;
using System;
using System.ComponentModel;
using Stimulsoft.Report.PropertyGrid;

namespace Stimulsoft.Report.Components.Gauge.Primitives
{
    public abstract class StiGaugeElement : 
        StiElementBase,
        IStiPropertyGridObject,
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();
            
            jObject.AddPropertyBool(nameof(AllowApplyStyle), AllowApplyStyle, true);            

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(AllowApplyStyle):
                        this.AllowApplyStyle = property.DeserializeBool();
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

        #region Properties
        [Browsable(false)]
        public StiAnimation Animation { get; set; }

        [Browsable(false)]
        public virtual StiGaugeElemenType ElementType => StiGaugeElemenType.LinearElement;

        [Browsable(false)]
        public virtual string LocalizeName => "GaugeElement";

        [Browsable(false)]
        [StiSerializable(StiSerializationVisibility.Class, StiSerializeTypes.SerializeToAll)]
        [DefaultValue(null)]
        public StiScaleBase Scale { get; set; }
        #endregion

        #region Methods virtual
        public virtual StiGaugeElement CreateNew() => throw new NotImplementedException();
        #endregion

        #region Methods
        protected internal virtual void PrepareGaugeElement()
        {

        }
        #endregion
    }
}