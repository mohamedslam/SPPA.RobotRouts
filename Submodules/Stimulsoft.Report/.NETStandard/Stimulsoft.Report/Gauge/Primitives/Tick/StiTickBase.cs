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
using Stimulsoft.Report.Gauge.Events;
using Stimulsoft.Report.Gauge.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components.Gauge.Primitives
{
    public abstract class StiTickBase :
        StiGaugeElement
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);
            
            jObject.AddPropertyJObject(nameof(GetSkipValuesEvent), GetSkipValuesEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(GetSkipIndicesEvent), GetSkipIndicesEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(SkipValues), SkipValues.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(SkipIndices), SkipIndices.SaveToJsonObject(mode));
            jObject.AddPropertyEnum(nameof(Placement), Placement);
            jObject.AddPropertyFloat(nameof(Offset), Offset);
            jObject.AddPropertyFloatNullable(nameof(MinimumValue), MinimumValue, null);
            jObject.AddPropertyFloatNullable(nameof(MaximumValue), MaximumValue, null);

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
                            var _getSkipValuesEvent = new StiGetSkipValuesEvent();
                            _getSkipValuesEvent.LoadFromJsonObject((JObject)property.Value);
                            this.GetSkipValuesEvent = _getSkipValuesEvent;
                        }
                        break;

                    case nameof(GetSkipIndicesEvent):
                        {
                            var _getSkipIndicesEvent = new StiGetSkipIndicesEvent();
                            _getSkipIndicesEvent.LoadFromJsonObject((JObject)property.Value);
                            this.GetSkipIndicesEvent = _getSkipIndicesEvent;
                        }
                        break;

                    case nameof(SkipValues):
                        {
                            var skipValuesExspression = new StiSkipValuesExpression();
                            skipValuesExspression.LoadFromJsonObject((JObject)property.Value);
                            this.SkipValues = skipValuesExspression;
                        }
                        break;

                    case nameof(SkipIndices):
                        {
                            var skipIndices = new StiSkipIndicesExpression();
                            skipIndices.LoadFromJsonObject((JObject)property.Value);
                            this.SkipIndices = skipIndices;
                        }
                        break;

                    case nameof(Placement):
                        this.Placement = property.DeserializeEnum<StiPlacement>();
                        break;

                    case nameof(Offset):
                        this.Offset = property.DeserializeFloat();
                        break;

                    case nameof(MinimumValue):
                        this.MinimumValue = property.DeserializeFloatNullable();
                        break;


                    case nameof(MaximumValue):
                        this.MaximumValue = property.DeserializeFloatNullable();
                        break;
                }
            }
        }
        #endregion

        #region ICloneable
        public override object Clone()
        {
            var tick = (StiTickBase)base.Clone();

            if (this.SkipValues != null) tick.SkipValues = (StiSkipValuesExpression)this.SkipValues.Clone();
            else tick.SkipValues = null;
            if (this.SkipIndices != null) tick.SkipIndices = (StiSkipIndicesExpression)this.SkipIndices.Clone();
            else tick.SkipIndices = null;

            return tick;
        }
        #endregion

        #region Events
        #region GetSkipValues
        /// <summary>
        /// Occurs when getting the property SkipValues.
        /// </summary>
        public event StiGetValueEventHandler GetSkipValues;

        /// <summary>
        /// Raises the GetValue event.
        /// </summary>
        protected virtual void OnGetSkipValues(StiGetValueEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetSkipValues event.
        /// </summary>
        public virtual void InvokeGetSkipValues(StiGaugeElement sender, StiGetValueEventArgs e)
        {
            try
            {
                OnGetSkipValues(e);
                this.GetSkipValues?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                string str = string.Format("Expression in GetSkipValues property of '{0}' series from '{1}' chart can't be evaluated!", "StiTickBase", (this.Scale.Gauge).Name);
                StiLogService.Write(this.GetType(), str);
                StiLogService.Write(this.GetType(), ex.Message);
                (this.Scale.Gauge).Report.WriteToReportRenderingMessages(str);
            }
        }


        /// <summary>
        /// Occurs when getting the property SkipValues.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when getting the property SkipValues.")]
        public StiGetSkipValuesEvent GetSkipValuesEvent { get; set; } = new StiGetSkipValuesEvent();
        #endregion

        #region GetSkipIndices
        /// <summary>
        /// Occurs when getting the property SkipIndices.
        /// </summary>
        public event StiGetValueEventHandler GetSkipIndices;

        /// <summary>
        /// Raises the GetSkipIndices event.
        /// </summary>
        protected virtual void OnGetSkipIndices(StiGetValueEventArgs e)
        {
        }


        /// <summary>
        /// Raises the GetSkipIndices event.
        /// </summary>
        public virtual void InvokeGetSkipIndices(StiGaugeElement sender, StiGetValueEventArgs e)
        {
            try
            {
                OnGetSkipIndices(e);
                this.GetSkipIndices?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                string str = string.Format("Expression in GetSkipIndices property of '{0}' series from '{1}' chart can't be evaluated!", "StiTickBase", (this.Scale.Gauge).Name);
                StiLogService.Write(this.GetType(), str);
                StiLogService.Write(this.GetType(), ex.Message);
                (this.Scale.Gauge).Report.WriteToReportRenderingMessages(str);
            }
        }


        /// <summary>
        /// Occurs when getting the property SkipIndices.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when getting the property SkipIndices.")]
        public StiGetSkipIndicesEvent GetSkipIndicesEvent { get; set; } = new StiGetSkipIndicesEvent();
        #endregion
        #endregion

        #region Expression
        #region SkipValues
        /// <summary>
        /// Gets or sets the values that should not be rendered. Example: 1;2;3
        /// </summary>
        [StiCategory("Value")]
        [StiSerializable(
            StiSerializeTypes.SerializeToCode |
            StiSerializeTypes.SerializeToDesigner |
            StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets the values that should not be rendered. Example: 1;2;3")]
        public virtual StiSkipValuesExpression SkipValues { get;set; } = new StiSkipValuesExpression();
        #endregion

        #region SkipIndices
        /// <summary>
        /// Gets or sets the indexes of values that cannot be displayed. Example: 1;2;3
        /// </summary>
        [StiCategory("Value")]
        [StiSerializable(
            StiSerializeTypes.SerializeToCode |
            StiSerializeTypes.SerializeToDesigner |
            StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets the indexes of values that cannot be displayed. Example: 1;2;3")]
        public virtual StiSkipIndicesExpression SkipIndices { get; set; } = new StiSkipIndicesExpression();
        #endregion
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the placement of the tick relative to the scale.
        /// </summary>
        [DefaultValue(StiPlacement.Outside)]
        [StiSerializable]
        [StiCategory("Tick")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets the placement of the tick relative to the scale.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiPlacement Placement { get; set; } = StiPlacement.Outside;

        [Browsable(false)]
        [DefaultValue(null)]
        [StiSerializable]
        public float[] SkipValuesObj { get; set; }

        [Browsable(false)]
        [DefaultValue(null)]
        [StiSerializable]
        public float[] SkipIndicesObj { get; set; }

        /// <summary>
        /// Gets or sets the offset from the scale based on the StiPlacement.
        /// </summary>
        [DefaultValue(0f)]
        [StiSerializable]
        [StiCategory("Value")]
        [Description("Gets or sets the offset from the scale based on the StiPlacement.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float Offset { get; set; } = 0f;


        /// <summary>
        /// Gets or sets the minimum value, with which displaying should begin.
        /// </summary>
        [DefaultValue(null)]
        [StiSerializable]
        [StiCategory("Value")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the minimum value, with which displaying should begin.")]
        public float? MinimumValue { get; set; } = null;

        /// <summary>
        /// Gets or sets the maximum value to which displaying should be.
        /// </summary>
        [DefaultValue(null)]
        [StiSerializable]
        [StiCategory("Value")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the maximum value to which displaying should be.")]
        public float? MaximumValue { get; set; } = null;

        [Browsable(false)]
        protected virtual bool IsSkipMajorValues => false;
        #endregion

        #region Methods.GetCollections
        protected virtual Dictionary<float, float> GetPointCollection() => null;

        protected Dictionary<float, float> GetMinorCollections()
        {
            var minorCollection = new Dictionary<float, float>();
            var majorCollection = !IsSkipMajorValues ? null : GetMajorCollections();

            float minorInterval = Math.Abs(this.Scale.MinorInterval);
            if (minorInterval == 0) minorInterval = 1;
            float minimum = this.Scale.ScaleHelper.ActualMinimum;
            float maximum = this.Scale.ScaleHelper.ActualMaximum;
            float length = this.Scale.ScaleHelper.TotalLength;

            float index = minimum;
            float step = minorInterval;
            float offset = 0;
            minorCollection.Add(index, 0);
            float proportion = length / 100;
            index += minorInterval;

            while (index <= maximum)// && minorCollection.Count < 200)//Protection from huge amount
            {
                offset += step;

                if (!minorCollection.ContainsKey(index))
                    minorCollection.Add(index, (offset / proportion) * 0.01f);

                index += minorInterval;
            }

            if (majorCollection != null)
            {
                foreach (float key in majorCollection.Keys)
                {
                    if (minorCollection.ContainsKey(key))
                        minorCollection.Remove(key);
                }
            }

            return minorCollection;
        }

        protected Dictionary<float, float> GetMajorCollections()
        {
            var majorCollection = new Dictionary<float, float>();

            float majorInterval = Math.Abs(this.Scale.MajorInterval);
            if (majorInterval == 0) majorInterval = 1;

            float minimum = this.Scale.ScaleHelper.ActualMinimum;
            float maximum = this.Scale.ScaleHelper.ActualMaximum;
            float length = this.Scale.ScaleHelper.TotalLength;
            float proportion = length / 100;

            float step = majorInterval;
            float index = minimum;
            float offset = 0;

            majorCollection.Add(index, 0f);
            index += majorInterval;

            float percent = 0f;
            while (index <= maximum)// && majorCollection.Count < 200)//Protection from huge amount
            {
                offset += step;

                if (!majorCollection.ContainsKey(index))
                {
                    percent = (offset / proportion) * 0.01f;
                    majorCollection.Add(index, percent);
                }

                index += majorInterval;
            }

            if (percent < 0.9f && majorCollection.Count < 5)
            {
                majorCollection.Add(maximum, 1f);
            }

            return majorCollection;
        }
        #endregion

        #region Methods
        protected bool CheckTickValue(float[] skipValues, float[] skipIndices, double key, int value)
        {
            if (skipIndices != null)
            {
                int index = 0;
                while (index < skipIndices.Length)
                {
                    if (skipIndices[index] == value)
                        return true;
                    index++;
                }
            }
            
            if (skipValues != null)
            {
                int index = 0;
                while (index < skipValues.Length)
                {
                    if (skipValues[index] == key)
                        return true;
                    index++;
                }
            }

            return false;
        }
        #endregion

        #region Methods override
        protected internal override void PrepareGaugeElement()
        {
            var e1 = new StiGetValueEventArgs();
            this.InvokeGetSkipValues(this, e1);
            this.SkipValuesObj = StiGaugeHelper.GetFloatArrayValueFromString(e1.Value);

            var e2 = new StiGetValueEventArgs();
            this.InvokeGetSkipIndices(this, e2);
            this.SkipIndicesObj = StiGaugeHelper.GetFloatArrayValueFromString(e2.Value);
        }
        #endregion

        #region Methods.Helper
        protected float GetOffset(float? value)
        {
            return (value == null) ? this.Offset : value.Value;
        }

        protected StiPlacement GetPlacement(StiPlacement? value)
        {
            return (value == null) ? this.Placement : value.Value;
        }
        #endregion
    }
}
