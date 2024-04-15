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

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Components.Gauge
{
    public class StiLinearTickMarkCustom : 
        StiLinearTickMarkBase,
        IStiTickCustom
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            jObject.AddPropertyJObject(nameof(GetValueEvent), GetValueEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(Value), Value.SaveToJsonObject(mode));
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
                            this.Value = _valueExpression;
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
        public override StiComponentId ComponentId => StiComponentId.StiLinearTickMarkCustom;

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
            var tickMark = (StiLinearTickMarkCustom)base.Clone();

            tickMark.Value = (this.Value != null)
                ? (StiValueExpression)this.Value.Clone()
                : null;

            tickMark.Values = new StiCustomValuesCollection();
            lock (((ICollection)this.Values).SyncRoot)
            {
                foreach (StiCustomValueBase customValue in this.Values)
                {
                    tickMark.Values.Add((StiCustomValueBase)customValue.Clone());
                }
            }

            return tickMark;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets value corresponds to this tick mark.
        /// </summary>
        [Browsable(false)]
        [StiSerializable]
        [DefaultValue(0f)]
        public float ValueObj { get; set; } = 0f;

        [Browsable(false)]
        [StiSerializable(StiSerializationVisibility.List)]
        public StiCustomValuesCollection Values { get; set; } = new StiCustomValuesCollection();
        #endregion

        #region Properties override
        public override string LocalizeName => "LinearTickMarkCustom";
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
        /// <summary>
        /// Gets or sets value corresponds to this tick mark.
        /// </summary>
        [StiCategory("Value")]
        [StiSerializable]
        [Description("Gets or sets value corresponds to this tick mark. Example: {Order.Value}")]
        public virtual StiValueExpression Value { get; set; } = new StiValueExpression();
        #endregion
        #endregion

        #region Methods override
        public override StiGaugeElement CreateNew() => new StiLinearTickMarkCustom();

        protected internal override void PrepareGaugeElement()
        {
            base.PrepareGaugeElement();

            var e = new StiGetValueEventArgs();
            this.InvokeGetValue(this, e);
            this.ValueObj = StiGaugeHelper.GetFloatValueFromObject(e.Value, 0f);
        }

        protected internal override void DrawElement(StiGaugeContextPainter context)
        {
            var linearScale = this.Scale as StiLinearScale;
            if (linearScale == null) return;

            var size = linearScale.barGeometry.Size;
            var rect = linearScale.barGeometry.RectGeometry;

            #region Set Position
            float rest = Scale.barGeometry.GetRestToLenght();
            float scaleOffset = (linearScale.Orientation == Orientation.Horizontal) ? size.Height : size.Width;
            scaleOffset *= base.Offset;
            float startValue = this.Scale.ScaleHelper.ActualMinimum;
            float endValue = this.Scale.ScaleHelper.ActualMaximum;

            var actualSkin = this.GetActualSkin();
            var skipValues = base.SkipValuesObj;
            var skipIndices = base.SkipIndicesObj;
            var coll = this.Values;
            if (coll == null || coll.Count == 0)
            {
                coll = new StiCustomValuesCollection
                {
                    new StiLinearTickMarkCustomValue(this.ValueObj, this.Offset, this.RelativeWidth, this.RelativeHeight, this.Placement, actualSkin)
                };
            }

            int index = -1;
            foreach (StiLinearTickMarkCustomValue key in coll)
            {
                index++;

                #region Check Value
                if (key.Value < startValue) continue;
                if (key.Value > endValue) continue;
                if (CheckTickValue(skipValues, skipIndices, key.Value, index)) continue;
                if (this.MinimumValue != null && key.Value < this.MinimumValue.Value) continue;
                if (this.MaximumValue != null && key.Value > this.MaximumValue.Value) continue;
                #endregion

                float offset, left, top;
                float position = Scale.GetPosition(key.Value);
                float tickWidth = size.Width * GetRelativeWidth(key.RelativeWidth);
                float tickHeight = size.Height * GetRelativeHeight(key.RelativeHeight);

                if (linearScale.Orientation == Orientation.Horizontal)
                {
                    offset = (linearScale.IsReversed) ? (rect.Width - (rect.Width * position)) : (rect.Width * position);

                    if (GetPlacement(key.Placement) == StiPlacement.Overlay)
                    {
                        left = rect.Left + offset - (tickWidth / 2);
                        top = StiRectangleHelper.CenterY(rect) - tickHeight / 2 - scaleOffset;
                    }
                    else
                    {
                        float restValue = (linearScale.StartWidth < linearScale.EndWidth) ? ((1 - position) * rest) : (rest * position);

                        if (GetPlacement(key.Placement) == StiPlacement.Outside)
                        {
                            left = rect.Left + offset - (tickWidth / 2);
                            top = rect.Top - tickHeight - scaleOffset + restValue;
                        }
                        else
                        {
                            left = rect.Left + offset - (tickWidth / 2);
                            top = rect.Bottom + scaleOffset - restValue;
                        }
                    }
                }
                else
                {
                    offset = (linearScale.IsReversed) ? rect.Height * position
                        : rect.Height - (rect.Height * position);

                    if (GetPlacement(key.Placement) == StiPlacement.Overlay)
                    {
                        left = StiRectangleHelper.CenterX(rect) - tickWidth / 2;
                        top = rect.Top + offset - (tickHeight / 2);
                    }
                    else
                    {
                        float restValue = (linearScale.StartWidth < linearScale.EndWidth) ? ((1 - position) * rest) : (rest * position);

                        if (GetPlacement(key.Placement) == StiPlacement.Outside)
                        {
                            left = rect.Left - tickWidth - 1 - scaleOffset + restValue;
                            top = rect.Top + offset - (tickHeight / 2);
                        }
                        else
                        {
                            left = rect.Right + 1 + scaleOffset - restValue;
                            top = rect.Top + offset - (tickHeight / 2);
                        }
                    }
                }

                var skin = (key.Skin == null) ? actualSkin : key.Skin;
                skin.Draw(context, this, new RectangleF(left, top, tickWidth, tickHeight));
            }
            #endregion
        }
        #endregion
    }
}