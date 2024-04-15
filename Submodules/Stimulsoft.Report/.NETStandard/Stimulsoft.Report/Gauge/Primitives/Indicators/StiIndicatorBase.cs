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
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Gauge.Events;
using Stimulsoft.Report.Gauge.Helpers;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components.Gauge.Primitives
{
    public abstract class StiIndicatorBase :
        StiGaugeElement
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyEnum(nameof(Placement), Placement);
            jObject.AddPropertyBrush(nameof(BorderBrush), BorderBrush);
            jObject.AddPropertyBrush(nameof(Brush), Brush);
            jObject.AddPropertyFloat(nameof(BorderWidth), BorderWidth);

            jObject.AddPropertyJObject(nameof(GetValueEvent), GetValueEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(Value), Value.SaveToJsonObject(mode));
            jObject.AddPropertyFloat(nameof(ValueObj), ValueObj);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Placement):
                        this.Placement = property.DeserializeEnum<StiPlacement>();
                        break;

                    case nameof(ValueObj):
                        this.valueObj = property.DeserializeFloat();
                        break;

                    case nameof(BorderBrush):
                        this.BorderBrush = property.DeserializeBrush();
                        break;

                    case nameof(Brush):
                        this.Brush = property.DeserializeBrush();
                        break;

                    case nameof(GetValueEvent):
                        {
                            var _getValueEvent = new StiGetValueEvent();
                            _getValueEvent.LoadFromJsonObject((JObject)property.Value);
                            this.GetValueEvent = _getValueEvent;
                        }
                        break;

                    case nameof(Value):
                        {
                            var _valueObj = new StiValueExpression();
                            _valueObj.LoadFromJsonObject((JObject)property.Value);
                            this.Value = _valueObj;
                        }
                        break;
                }
            }
        }
        #endregion

        #region ICloneable
        public override object Clone()
        {
            var indicator = (StiIndicatorBase)base.Clone();

            indicator.Brush = (StiBrush)this.Brush.Clone();
            indicator.BorderBrush = (StiBrush)this.BorderBrush.Clone();

            indicator.Value = (this.Value != null)
                ? (StiValueExpression)this.Value.Clone()
                : indicator.Value = null;

            return indicator;
        }
        #endregion

        #region Properties
        private float valueObj;
        [Browsable(false)]
        [DefaultValue(0f)]
        [StiSerializable]
        public float ValueObj
        {
            get
            {
                return valueObj;
            }
            set
            {
                valueObj = value;
                OnValueChanged();
            }
        }

        /// <summary>
        /// Gets or sets the placement of the component.
        /// </summary>
        [DefaultValue(StiPlacement.Overlay)]
        [StiSerializable]
        [StiCategory("Indicator")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the placement of the component.")]
        public virtual StiPlacement Placement { get; set; } = StiPlacement.Overlay;

        /// <summary>
        /// Gets or sets a brush to fill a component.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets a brush to fill a component.")]
        [StiOrder(StiPropertyOrder.AppearanceBrush)]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual StiBrush Brush { get; set; } = new StiSolidBrush(Color.White);

        /// <summary>
        /// Gets or sets the border of the component.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceBorder)]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the border of the component.")]
        public virtual StiBrush BorderBrush { get; set; } = new StiEmptyBrush();

        /// <summary>
        /// Gets or sets the border thickness of the component.
        /// </summary>
        [DefaultValue(0f)]
        [StiSerializable]
        [StiCategory("Appearance")]
        [StiOrder(200)]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the border thickness of the component.")]
        public virtual float BorderWidth { get; set; }
        #endregion

        #region Events
        #region GetValue
        /// <summary>
        /// Occurs when getting the property Value.
        /// </summary>
        public event StiGetValueEventHandler GetValue;

        /// <summary>
        /// Raises the GetValue event.
        /// </summary>
        protected virtual void OnGetValue(StiGetValueEventArgs e)
        {
        }


        /// <summary>
        /// Raises the GetValue event.
        /// </summary>
        public virtual void InvokeGetValue(StiGaugeElement sender, StiGetValueEventArgs e)
        {
            try
            {
                OnGetValue(e);
                if (sender.Scale.Gauge.Report != null && sender.Scale.Gauge.Report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    object parserResult = Engine.StiParser.ParseTextValue(Value.Value, sender.Scale.Gauge);
                    e.Value = sender.Scale.Gauge.Report.ToString(parserResult);
                }
                this.GetValue?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                string str = string.Format("Expression in GetValue property of '{0}' series from '{1}' chart can't be evaluated!", "Indicator", (this.Scale.Gauge).Name);
                StiLogService.Write(this.GetType(), str);
                StiLogService.Write(this.GetType(), ex.Message);
                (this.Scale.Gauge).Report.WriteToReportRenderingMessages(str);
            }
        }


        /// <summary>
        /// Occurs when getting the property Value.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when getting the property Value.")]
        public StiGetValueEvent GetValueEvent { get; set; } = new StiGetValueEvent();
        #endregion
        #endregion

        #region Expression
        #region Value
        /// <summary>
        /// Gets or sets the current position of an indicator.
        /// </summary>
        [StiCategory("Value")]
        [StiSerializable]
        [Description("Gets or sets the current position of an indicator. Example: {Order.Value}")]
        public virtual StiValueExpression Value { get; set; } = new StiValueExpression();
        #endregion
        #endregion

        #region Methods override
        protected internal override void PrepareGaugeElement()
        {
            var e = new StiGetValueEventArgs();
            this.InvokeGetValue(this, e);
            this.ValueObj = StiGaugeHelper.GetFloatValueFromObject(e.Value, this.Scale);
        }
        #endregion

        #region Methods abstract
        protected internal abstract void InteractiveClick(RectangleF rect, Point p);

        protected virtual void OnValueChanged()
        {

        }
        #endregion

        #region Methods
        public float? GetActualValue()
        {
            if (this.Scale == null || this.Scale.Gauge == null) 
                return null;

            try
            {
                if (this.Scale.Gauge.IsDesigning && this.ValueObj == 0)
                {
                    var obj = (this.Value.Value.StartsWith("{"))
                            ? StiParser.ParseTextValue(this.Value.Value, this.Scale.Gauge)
                            : StiParser.ParseTextValue("{" + this.Value.Value + "}", this.Scale.Gauge);

                    if (obj == null) return 0f;
                    if (obj is string)
                    {
                        var result = float.Parse((obj as string).Trim().Replace(",", "."), new CultureInfo("en-us"));
                        return result;
                    }

                    return Convert.ToSingle(obj);
                }
                else
                {
                    return this.ValueObj;
                }
            }
            catch 
            {
                return 0f; 
            }
        }
        #endregion
    }
}
