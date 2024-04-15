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
using Stimulsoft.Base.Drawing;
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

namespace Stimulsoft.Report.Components.Gauge
{
    public class StiRadialTickMarkCustom : 
        StiRadialTickMarkBase,
        IStiTickCustom
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            jObject.AddPropertyJObject(nameof(GetValueEvent), GetValueEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(Value), value.SaveToJsonObject(mode));
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
                    case nameof(GetSkipValuesEvent):
                        {
                            var _getSkipValuesEvent = new StiGetValueEvent();
                            _getSkipValuesEvent.LoadFromJsonObject((JObject)property.Value);
                            this.GetValueEvent = _getSkipValuesEvent;
                        }
                        break;

                    case nameof(GetSkipIndicesEvent):
                        {
                            var _valueExpression = new StiValueExpression();
                            _valueExpression.LoadFromJsonObject((JObject)property.Value);
                            this.value = _valueExpression;
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
        public override StiComponentId ComponentId => StiComponentId.StiRadialTickMarkCustom;

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
                propHelper.Value()
            };
            checkBoxHelper.Add(StiPropertyCategories.Value, list);

            // TickCategory
            list = new[]
            {
                propHelper.OffsetAngle(),
                propHelper.Placement(),
                propHelper.RelativeHeight(),
                propHelper.RelativeWidth(),
                propHelper.Skin()
            };
            checkBoxHelper.Add(StiPropertyCategories.Tick, list);

            // AppearanceCategory
            list = new[]
            {
                propHelper.Brush(),
                propHelper.BorderBrush(),
                propHelper.BorderWidth()
            };
            checkBoxHelper.Add(StiPropertyCategories.Appearance, list);

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
            var radialTickMark = (StiRadialTickMarkCustom)base.Clone();

            if (this.value != null) radialTickMark.value = (StiValueExpression)this.value.Clone();
            else radialTickMark.value = null;

            radialTickMark.Values = new StiCustomValuesCollection();
            lock (((ICollection)this.Values).SyncRoot)
            {
                foreach (StiCustomValueBase customValue in this.Values)
                {
                    radialTickMark.Values.Add((StiCustomValueBase)customValue.Clone());
                }
            }

            return radialTickMark;
        }
        #endregion

        #region Properties
        [Browsable(false)]
        [DefaultValue(0f)]
        [StiSerializable]
        public float ValueObj { get; set; } = 0f;

        [Browsable(false)]
        [StiSerializable(StiSerializationVisibility.List)]
        public StiCustomValuesCollection Values { get; set; } = new StiCustomValuesCollection();
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
        #endregion

        #region Expression
        #region Value
        private StiValueExpression value = new StiValueExpression();
        /// <summary>
        /// Gets or sets value corresponds to this tick mark.
        /// </summary>
        [StiCategory("Value")]
        [StiSerializable]
        [Description("Gets or sets value corresponds to this tick mark. Example: {Order.Value}")]
        public virtual StiValueExpression Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }
        #endregion
        #endregion

        #region Properties override
        public override StiGaugeElemenType ElementType => StiGaugeElemenType.RadialElement;

        public override string LocalizeName => "RadialTickMarkCustom";
        #endregion

        #region Methods override
        public override StiGaugeElement CreateNew() => new StiRadialTickMarkCustom();

        protected internal override void PrepareGaugeElement()
        {
            base.PrepareGaugeElement();

            var e = new StiGetValueEventArgs();
            this.InvokeGetValue(this, e);
            this.ValueObj = StiGaugeHelper.GetFloatValueFromObject(e.Value, 0f);
        }

        protected internal override void DrawElement(StiGaugeContextPainter context)
        {
            var radialScale = Scale as StiRadialScale;
            if (radialScale == null) return;

            var rect = Scale.barGeometry.RectGeometry;
            if (rect.Width <= 0 || rect.Height <= 0) return;

            #region Temporary Variables
            var centerPoint = radialScale.barGeometry.Center;
            float sweepAngle = radialScale.GetSweepAngle();
            float startAngle = radialScale.StartAngle;
            float diameter = Scale.barGeometry.Diameter;
            float barRadius = Scale.barGeometry.Radius;

            float startValue = this.Scale.ScaleHelper.ActualMinimum;
            float endValue = this.Scale.ScaleHelper.ActualMaximum;
            float minWidth = this.Scale.ScaleHelper.MinWidth;
            float maxWidth = this.Scale.ScaleHelper.MaxWidth; 
            float restWidth;

            maxWidth *= Scale.barGeometry.RectGeometry.Width;
            minWidth *= Scale.barGeometry.RectGeometry.Width;
            restWidth = maxWidth - minWidth;
            var actualSkin = this.GetActualSkin();

            var skipValues = base.SkipValuesObj;
            var skipIndices = base.SkipIndicesObj;
            var coll = this.Values;
            if (coll == null || coll.Count == 0)
            {
                coll = new StiCustomValuesCollection
                {
                    new StiRadialTickMarkCustomValue(this.ValueObj, this.Offset, this.RelativeWidth, this.RelativeHeight, this.OffsetAngle, this.Placement, actualSkin)
                };
            }
            #endregion

            int index = -1;
            foreach (StiRadialTickMarkCustomValue key in coll)
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
                float tickWidth = diameter * GetRelativeWidth(key.RelativeWidth);
                float tickHeight = diameter * GetRelativeHeight(key.RelativeHeight);
                float radius = (GetPlacement(key.Placement) == StiPlacement.Outside) ? 
                    (barRadius * (1 + GetOffset(key.Offset))) : (barRadius * (1 - GetOffset(key.Offset)));

                float tickOffset = sweepAngle * GetRelativeHeight(key.RelativeHeight) / 4;
                float angle = (Scale.IsReversed) 
                    ? (startAngle + sweepAngle - value1 * sweepAngle - GetOffsetAngle(key.OffsetAngle)) + tickOffset 
                    : (startAngle + value1 * sweepAngle + GetOffsetAngle(key.OffsetAngle)) - tickOffset;
                float currentRadius;

                if (GetPlacement(key.Placement) == StiPlacement.Outside)
                {
                    currentRadius = radius;
                }
                else if (GetPlacement(key.Placement) == StiPlacement.Overlay)
                {
                    if (Scale.IsUp)
                    {
                        currentRadius = radius - ((minWidth + restWidth * value1 + tickWidth) / 2);
                    }
                    else
                    {
                        currentRadius = radius - ((maxWidth - restWidth * value1 + tickWidth) / 2);
                    }
                }
                else
                {
                    if (Scale.IsUp)
                    {
                        currentRadius = radius - minWidth - (restWidth * value1) - tickWidth;
                    }
                    else
                    {
                        currentRadius = radius - maxWidth + (restWidth * value1) - tickWidth;
                    }
                }

                StiBrush currentBackground = null;
                StiBrush currentBorderBrush;
                float currentBorderWidth = 0f;

                if (key.useBrush)
                {
                    currentBackground = this.Brush;
                    this.Brush = key.Brush;
                }
                if (key.useBorderBrush)
                {
                    currentBorderBrush = this.BorderBrush;
                    this.BorderBrush = key.BorderBrush;
                }
                if (key.useBorderWidth)
                {
                    currentBorderWidth = this.BorderWidth;
                    this.BorderWidth = (key.BorderWidth == null) ? 0 : key.BorderWidth.Value;
                }

                var tickRect = new RectangleF(centerPoint.X + currentRadius, centerPoint.Y, tickWidth, tickHeight);
                context.AddPushMatrixGaugeGeom(angle, centerPoint);
                var skin = (key.Skin == null) ? actualSkin : key.Skin;
                skin.Draw(context, this, tickRect);
                context.AddPopTranformGaugeGeom();

                if (key.useBrush)
                    this.Brush = currentBackground;
                if (key.useBorderBrush)
                    this.BorderBrush = currentBackground;
                if (key.useBorderWidth)
                    this.BorderWidth = currentBorderWidth;
            }
        }

        private float GetOffsetAngle(float? value)
        {
            if (value == null)
            {
                return (this.Scale.IsReversed) ? -this.OffsetAngle : this.OffsetAngle;
            }
            else                            
                return value.Value;
        }
        #endregion
    }
}