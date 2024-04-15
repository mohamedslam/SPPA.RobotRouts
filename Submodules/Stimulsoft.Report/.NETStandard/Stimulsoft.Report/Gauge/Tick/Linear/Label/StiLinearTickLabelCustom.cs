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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components.Gauge.Primitives;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Gauge.Collections;
using Stimulsoft.Report.Gauge.Events;
using Stimulsoft.Report.Gauge.Helpers;
using Stimulsoft.Report.Gauge.Primitives;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Components.Gauge
{
    public class StiLinearTickLabelCustom : 
        StiLinearTickLabelBase,
        IStiTickCustom
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            jObject.AddPropertyJObject(nameof(GetValueEvent), GetValueEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(GetTextEvent), GetTextEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(Value), Value.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(Text), Text.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(Values), Values.SaveToJsonObject(mode));
            
            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(GetValueEvent):
                        {
                            var _getValueEvent = new StiGetValueEvent();
                            _getValueEvent.LoadFromJsonObject((JObject)property.Value);
                            this.GetValueEvent = _getValueEvent;
                        }
                        break;

                    case nameof(GetTextEvent):
                        {
                            var _getTextEvent = new StiGetTextEvent();
                            _getTextEvent.LoadFromJsonObject((JObject)property.Value);
                            this.GetTextEvent = _getTextEvent;
                        }
                        break;

                    case nameof(Value):
                        {
                            var _valueObj = new StiValueExpression();
                            _valueObj.LoadFromJsonObject((JObject)property.Value);
                            this.Value = _valueObj;
                        }
                        break;

                    case nameof(Text):
                        {
                            var _textObj = new StiTextExpression();
                            _textObj.LoadFromJsonObject((JObject)property.Value);
                            this.Text = _textObj;
                        }
                        break;
                        
                    case nameof(Values):
                        this.Values.LoadFromJsonObject((JObject)property.Value);
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiLinearTickLabelCustom;

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
                propHelper.SkipValues(),
                propHelper.Text(),
                propHelper.Value()
            };
            checkBoxHelper.Add(StiPropertyCategories.Value, list);

            // TickCategory
            list = new[]
            {
                propHelper.Placement()
            };
            checkBoxHelper.Add(StiPropertyCategories.Tick, list);

            // TextAdditionalCategory
            list = new[]
            {
                propHelper.Font(),
                propHelper.BorderBrush(),
                propHelper.TextFormatStr()
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

        #region ICloneable
        public override object Clone()
        {
            var label = (StiLinearTickLabelCustom)base.Clone();

            label.Value = (this.Value != null)
                 ? (StiValueExpression)this.Value.Clone()
                : null;

            label.Text = (this.Text != null)
                ? (StiTextExpression)this.Text.Clone()
                : null;

            label.Values = new StiCustomValuesCollection();
            lock (((ICollection)this.Values).SyncRoot)
            {
                foreach (StiCustomValueBase customValue in this.Values)
                {
                    label.Values.Add((StiCustomValueBase)customValue.Clone());
                }
            }

            return label;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets value corresponds to this tick label.
        /// </summary>
        [Browsable(false)]
        [StiSerializable]
        [DefaultValue(0f)]
        public float ValueObj { get; set; } = 0f;

        /// <summary>
        /// Gets or sets text corresponds to this tick label.
        /// </summary>
        [Browsable(false)]
        [StiSerializable]
        [DefaultValue(null)]
        public string TextObj { get; set; }

        [Browsable(false)]
        [StiSerializable(StiSerializationVisibility.List)]
        public StiCustomValuesCollection Values { get; set; } = new StiCustomValuesCollection();
        #endregion

        #region Properties override
        public override string LocalizeName => "LinearTickLabelCustom";
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
                if (sender.Scale.Gauge.Report.CalculationMode == StiCalculationMode.Interpretation)
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

        #region GetText
        /// <summary>
        /// Occurs when getting the property Text.
        /// </summary>
        public event StiGetTextEventHandler GetText;

        /// <summary>
        /// Raises the GetText event.
        /// </summary>
        protected virtual void OnGetText(StiGetTextEventArgs e)
        {
        }


        /// <summary>
        /// Raises the GetText event.
        /// </summary>
        public virtual void InvokeGetText(StiGaugeElement sender, StiGetTextEventArgs e)
        {
            try
            {
                OnGetText(e);
                this.GetText?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                string str = string.Format("Expression in GetText property of '{0}' series from '{1}' chart can't be evaluated!", "GaugeElement", (this.Scale.Gauge).Name);
                StiLogService.Write(this.GetType(), str);
                StiLogService.Write(this.GetType(), ex.Message);
                (this.Scale.Gauge).Report.WriteToReportRenderingMessages(str);
            }
        }


        /// <summary>
        /// Occurs when getting the property Text.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when getting the property Text.")]
        public StiGetTextEvent GetTextEvent { get; set; } = new StiGetTextEvent();
        #endregion
        #endregion

        #region Expression
        #region Value
        /// <summary>
        /// Gets or sets value corresponds to this tick label.
        /// </summary>
        [StiCategory("Value")]
        [StiSerializable]
        [Description("Gets or sets value corresponds to this tick label. Example: {Order.Value}")]
        public virtual StiValueExpression Value { get; set; } = new StiValueExpression();
        #endregion

        #region Text
        /// <summary>
        /// Gets or sets text corresponds to this tick label.
        /// </summary>
        [StiCategory("Value")]
        [StiSerializable]
        [Description("Gets or sets text corresponds to this tick label. Example: {Order.Value}")]
        public virtual StiTextExpression Text { get; set; } = new StiTextExpression();
        #endregion
        #endregion

        #region Methods override
        public override StiGaugeElement CreateNew() => new StiLinearTickLabelCustom();
        #endregion

        #region Methods
        protected internal override void PrepareGaugeElement()
        {
            base.PrepareGaugeElement();

            var e1 = new StiGetValueEventArgs();
            this.InvokeGetValue(this, e1);
            this.ValueObj = StiGaugeHelper.GetFloatValueFromObject(e1.Value, 0f);

            var e2 = new StiGetTextEventArgs();
            this.InvokeGetText(this, e2);
            this.TextObj = e2.Value;
        }

        protected internal override void DrawElement(StiGaugeContextPainter context)
        {
            var linearScale = this.Scale as StiLinearScale;
            if (linearScale == null || string.IsNullOrEmpty(this.TextObj)) return;

            var size = linearScale.barGeometry.Size;
            var rect = linearScale.barGeometry.RectGeometry;

            #region Set Position
            float offset, left, top;
            var rest = Scale.barGeometry.GetRestToLenght();
            var scaleOffset = (linearScale.Orientation == Orientation.Horizontal) ? size.Height : size.Width;
            scaleOffset *= base.Offset;

            var startValue = this.Scale.ScaleHelper.ActualMinimum;
            var endValue = this.Scale.ScaleHelper.ActualMaximum;

            var textFormat = base.TextFormat;
            var skipValues = base.SkipValuesObj;
            var skipIndices = base.SkipIndicesObj;
            var values = this.Values;
            if (values == null || values.Count == 0)
            {
                values = new StiCustomValuesCollection
                {
                    new StiLinearTickLabelCustomValue(this.ValueObj, this.TextObj, this.Offset, this.Placement)
                };
            }

            int index = -1;
            foreach (StiLinearTickLabelCustomValue key in values)
            {
                index++;

                #region Check Value
                if (key.Value < startValue) continue;
                if (key.Value > endValue) continue;
                if (CheckTickValue(skipValues, skipIndices, key.Value, index)) continue;
                if (this.MinimumValue != null && key.Value < this.MinimumValue.Value) continue;
                if (this.MaximumValue != null && key.Value > this.MaximumValue.Value) continue;
                #endregion

                float actualScaleOffset = scaleOffset * this.GetOffset(key.Offset);
                float position = linearScale.GetPosition(key.Value);
                string text = "";
                if (linearScale.DateTimeMode)
                {
                    var collection = new Dictionary<float, float>()
                    {
                        {  key.Value , 0 }
                    };
                    var collectionPrepare = StiTickLabelHelper.GetLabels(collection, linearScale);

                    text = collectionPrepare[key.Value];
                }
                else
                {
                    text = GetTextForRender(key.Text, textFormat);
                }

                var zoomFont = StiGaugeContextPainter.ChangeFontSize(this.Font, context.Zoom);
                var textSize = context.MeasureString(text, zoomFont);

                if (linearScale.Orientation == Orientation.Horizontal)
                {
                    offset = (linearScale.IsReversed) ? rect.Width - (rect.Width * position) : rect.Width * position;

                    if (GetPlacement(key.Placement) == StiPlacement.Overlay)
                    {
                        left = rect.Left + offset - (textSize.Width / 2);
                        top = StiRectangleHelper.CenterY(rect) - textSize.Height / 2 - actualScaleOffset;
                    }
                    else
                    {
                        float restValue = (linearScale.StartWidth < linearScale.EndWidth) ? (1 - position) * rest : rest * position;

                        if (GetPlacement(key.Placement) == StiPlacement.Outside)
                        {
                            left = rect.Left + offset - (textSize.Width / 2);
                            top = rect.Top - textSize.Height - actualScaleOffset + restValue;
                        }
                        else
                        {
                            left = rect.Left + offset - (textSize.Width / 2);
                            top = rect.Bottom + actualScaleOffset - restValue;
                        }
                    }
                }
                else
                {
                    offset = (linearScale.IsReversed) ? rect.Height * position
                        : rect.Height - (rect.Height * position);

                    if (GetPlacement(key.Placement) == StiPlacement.Overlay)
                    {
                        left = StiRectangleHelper.CenterX(rect) - textSize.Width / 2;
                        top = rect.Top + offset - (textSize.Height / 2);
                    }
                    else
                    {
                        float restValue = (linearScale.StartWidth < linearScale.EndWidth) ? (1 - position) * rest : rest * position;

                        if (GetPlacement(key.Placement) == StiPlacement.Outside)
                        {
                            left = rect.Left - textSize.Width - 3 - actualScaleOffset + restValue;
                            top = rect.Top + offset - (textSize.Height / 2);
                        }
                        else
                        {
                            left = rect.Right + 3 + actualScaleOffset - restValue;
                            top = rect.Top + offset - (textSize.Height / 2);
                        }
                    }
                }

                context.AddTextGaugeGeom(text, zoomFont, this.TextBrush, new RectangleF(new PointF(left, top), textSize), null);
            }
            #endregion
        }
        #endregion
    }
}
