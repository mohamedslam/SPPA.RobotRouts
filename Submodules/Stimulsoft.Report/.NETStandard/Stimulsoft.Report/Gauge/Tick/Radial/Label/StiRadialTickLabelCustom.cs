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
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using Stimulsoft.Report.PropertyGrid;
using System.Collections.Generic;

namespace Stimulsoft.Report.Components.Gauge
{
    public class StiRadialTickLabelCustom : 
        StiRadialTickLabelBase,
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
        public override StiComponentId ComponentId => StiComponentId.StiRadialTickLabelCustom;

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
                propHelper.LabelRotationMode(),
                propHelper.OffsetAngle(),
                propHelper.TextBrush(),
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
            var radialTickLabel = (StiRadialTickLabelCustom)base.Clone();

            radialTickLabel.Value = (this.Value != null)
                 ? (StiValueExpression)this.Value.Clone()
                : null;

            radialTickLabel.Text = (this.Text != null)
                ? (StiTextExpression)this.Text.Clone()
                : null;

            radialTickLabel.Values = new StiCustomValuesCollection();
            lock (((ICollection)this.Values).SyncRoot)
            {
                foreach (StiCustomValueBase customValue in this.Values)
                {
                    radialTickLabel.Values.Add((StiCustomValueBase)customValue.Clone());
                }
            }

            return radialTickLabel;
        }
        #endregion

        #region Properties
        [Browsable(false)]
        [StiSerializable]
        [DefaultValue(0f)]
        public float ValueObj { get; set; } = 0f;

        [Browsable(false)]
        [StiSerializable]
        [DefaultValue(null)]
        public string TextObj { get; set; }

        [Browsable(false)]
        [StiSerializable(StiSerializationVisibility.List)]
        public StiCustomValuesCollection Values { get; set; } = new StiCustomValuesCollection();
        #endregion

        #region Properties override
        public override StiGaugeElemenType ElementType => StiGaugeElemenType.RadialElement;

        public override string LocalizeName => "RadialTickLabelCustom";
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
                if (this.GetValue != null) this.GetValue(sender, e);
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
        public override StiGaugeElement CreateNew() => new StiRadialTickLabelCustom();
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
            var radialScale = this.Scale as StiRadialScale;
            if (radialScale == null) return;

            var rect = Scale.barGeometry.RectGeometry;
            if (rect.Width <= 0 || rect.Height <= 0) return;

            #region Temporary Variables
            var centerPoint = radialScale.barGeometry.Center;
            float radiusMain = Scale.barGeometry.Radius;

            float sweepAngle = radialScale.GetSweepAngle();
            float startAngle = radialScale.StartAngle;

            float startValue = this.Scale.ScaleHelper.ActualMinimum;
            float endValue = this.Scale.ScaleHelper.ActualMaximum;
            float maxWidth = this.Scale.ScaleHelper.MaxWidth;
            float minWidth = this.Scale.ScaleHelper.MinWidth;

            maxWidth *= (radiusMain * 2);
            minWidth *= (radiusMain * 2);
            float restWidth = maxWidth - minWidth;

            string textFormat = base.TextFormat;
            var skipValues = base.SkipValuesObj;
            var skipIndices = base.SkipIndicesObj;
            var values = this.Values;
            if (values == null || values.Count == 0)
            {
                values = new StiCustomValuesCollection
                {
                    new StiRadialTickLabelCustomValue(this.ValueObj, this.TextObj, this.Offset, this.OffsetAngle,
                    this.LabelRotationMode, this.Placement)
                };
            }
            #endregion

            var zoomFont = StiGaugeContextPainter.ChangeFontSize(this.Font, context.Zoom);
            int index = -1;
            foreach (StiRadialTickLabelCustomValue key in values)
            {
                index++;
                #region Check Value
                if (key.Value < startValue) continue;
                if (key.Value > endValue) continue;
                if (CheckTickValue(skipValues, skipIndices, key.Value, index)) continue;
                if (this.MinimumValue != null && key.Value < this.MinimumValue.Value) continue;
                if (this.MaximumValue != null && key.Value > this.MaximumValue.Value) continue;
                #endregion

                float value1 = Scale.GetPosition(key.Value);
                string text = "";

                if (radialScale.DateTimeMode)
                {
                    var collection = new Dictionary<float, float>()
                    {
                        {  key.Value , 0 }
                    };
                    var collectionPrepare = StiTickLabelHelper.GetLabels(collection, radialScale);

                    text = collectionPrepare[key.Value];
                }
                else
                {
                    text = string.IsNullOrEmpty(key.Text) ? string.Empty : GetTextForRender(key.Text, textFormat);
                }

                float actualRadius = (GetPlacement(key.Placement) == StiPlacement.Outside) ? 
                    (radiusMain * (1 + GetOffset(key.Offset))) : 
                    (radiusMain * (1 - GetOffset(key.Offset)));

                SizeF textSize = context.MeasureString(text, zoomFont);

                float angle = (Scale.IsReversed) ?
                    (startAngle + sweepAngle - value1 * sweepAngle + GetOffsetAngle(key.OffsetAngle)) :
                    (startAngle + value1 * sweepAngle - GetOffsetAngle(key.OffsetAngle));
                int countMatrix = 0;
                PointF point = new PointF();

                if (GetPlacement(key.Placement) == StiPlacement.Outside)
                {
                    countMatrix = GetMatrixRotation(context, centerPoint, textSize, GetLabelRotationMode(key.LabelRotationMode), actualRadius, angle, out point);
                }
                else if (GetPlacement(key.Placement) == StiPlacement.Overlay)
                {
                    float radius;
                    if (Scale.IsUp)
                        radius = actualRadius - ((minWidth + restWidth * value1) / 2) - textSize.Width / 2;
                    else
                        radius = actualRadius - ((maxWidth - restWidth * value1) / 2) - textSize.Width / 2;

                    countMatrix = GetMatrixRotation(context, centerPoint, textSize, GetLabelRotationMode(key.LabelRotationMode), radius, angle, out point);
                }
                else
                {
                    float radius = 0f;

                    if (Scale.IsUp)
                        radius = actualRadius - minWidth - restWidth * value1 - textSize.Width;
                    else
                        radius = actualRadius - maxWidth + restWidth * value1 - textSize.Width;

                    countMatrix = GetMatrixRotation(context, centerPoint, textSize, GetLabelRotationMode(key.LabelRotationMode), radius, angle, out point);
                }

                context.AddTextGaugeGeom(text, zoomFont, this.TextBrush, new RectangleF(point, textSize), null);
                for (int index1 = 0; index1 < countMatrix; index1++)
                {
                    context.AddPopTranformGaugeGeom();
                }
            }
        }
        #endregion

        #region Methods.Helper
        private float GetOffsetAngle(float? value)
        {
            return (value == null) ? this.OffsetAngle : value.Value;
        }

        private StiLabelRotationMode GetLabelRotationMode(StiLabelRotationMode? value)
        {
            return (value == null) ? this.LabelRotationMode : value.Value;
        }
        #endregion
    }
}