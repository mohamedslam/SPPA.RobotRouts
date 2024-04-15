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
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
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
    public abstract class StiRangeBase : 
        ICloneable,
        IStiPropertyGridObject
    {
        #region IStiJsonReportObject.override

        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();
            
            jObject.AddPropertyBrush(nameof(Brush), Brush);
            jObject.AddPropertyBrush(nameof(BorderBrush), BorderBrush);
            jObject.AddPropertyFloat(nameof(BorderWidth), BorderWidth, 1f);
            jObject.AddPropertyFloat(nameof(StartValue), StartValue);
            jObject.AddPropertyFloat(nameof(EndValue), EndValue);
            jObject.AddPropertyFloat(nameof(StartWidth), StartWidth);
            jObject.AddPropertyFloat(nameof(EndWidth), EndWidth);
            jObject.AddPropertyEnum(nameof(Placement), Placement);
            jObject.AddPropertyFloat(nameof(Offset), Offset);

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Brush):
                        this.Brush = property.DeserializeBrush();
                        break;

                    case nameof(BorderBrush):
                        this.BorderBrush = property.DeserializeBrush();
                        break;

                    case nameof(BorderWidth):
                        this.BorderWidth = property.DeserializeFloat();
                        break;

                    case nameof(StartValue):
                        this.StartValue = property.DeserializeFloat();
                        break;

                    case nameof(EndValue):
                        this.EndValue = property.DeserializeFloat();
                        break;

                    case nameof(StartWidth):
                        this.StartWidth = property.DeserializeFloat();
                        break;

                    case nameof(EndWidth):
                        this.EndWidth = property.DeserializeFloat();
                        break;

                    case nameof(Placement):
                        this.Placement = property.DeserializeEnum<StiPlacement>();
                        break;

                    case nameof(Offset):
                        this.Offset = property.DeserializeFloat();
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
        public object Clone()
        {
            var range = (StiRangeBase)this.MemberwiseClone();

            range.Brush = (StiBrush)this.Brush.Clone();
            range.BorderBrush = (StiBrush)this.BorderBrush.Clone();

            return range;
        }
        #endregion

        #region Properties
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
        public StiBrush BorderBrush { get; set; } = new StiEmptyBrush();

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

        /// <summary>
        /// Gets or sets start value of the range.
        /// </summary>        
        [DefaultValue(0f)]
        [StiSerializable]
        [StiCategory("Value")]
        [Description("Gets or sets start value of the range.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float StartValue { get; set; } 

        /// <summary>
        /// Gets or sets end value of the range.
        /// </summary>        
        [DefaultValue(0f)]
        [StiSerializable]
        [StiCategory("Value")]
        [Description("Gets or sets end value of the range.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float EndValue { get; set; }

        /// <summary>
        /// Gets or sets start width of the range bar.
        /// </summary>        
        [DefaultValue(0f)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [Description("Gets or sets start width of the range bar.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float StartWidth { get; set; }

        /// <summary>
        /// Gets or sets end width of the range bar.
        /// </summary>        
        [DefaultValue(0f)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [Description("Gets or sets end width of the range bar.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float EndWidth { get; set; }

        /// <summary>
        /// Gets or sets the placement of the component.
        /// </summary>
        [DefaultValue(StiPlacement.Overlay)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the placement of the component.")]
        public StiPlacement Placement { get; set; } = StiPlacement.Overlay;

        /// <summary>
        /// Gets or sets the offset ratio of an item.
        /// </summary>
        [DefaultValue(0f)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the offset ratio of an item.")]
        public float Offset { get; set; }

        internal StiScaleRangeList rangeList;
        [Browsable(false)]
        public StiScaleRangeList RangeList => this.rangeList;

        [Browsable(false)]
        public abstract string LocalizeName { get; }
        #endregion

        #region Methods abstract
        protected internal abstract void DrawRange(StiGaugeContextPainter context, StiScaleBase scale);
        #endregion

        #region Methods override

        public virtual StiRangeBase CreateNew() => throw new NotImplementedException();
        #endregion
    }
}