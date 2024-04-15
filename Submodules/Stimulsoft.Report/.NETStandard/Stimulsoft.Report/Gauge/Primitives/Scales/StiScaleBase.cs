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
using System.Drawing;
using System.ComponentModel;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Design;
using Stimulsoft.Base;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.Gauge.Collections;
using Stimulsoft.Report.Gauge.Helpers;
using Stimulsoft.Report.Gauge.Primitives;
using Stimulsoft.Report.Gauge;
using System.Collections;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.PropertyGrid;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components.Gauge.Primitives
{
    public abstract class StiScaleBase : 
        StiElementBase,
        IStiPropertyGridObject,
        IStiJsonReportObject
    {
        #region IStiJsonReportObject
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyBool(nameof(AllowApplyStyle), AllowApplyStyle, true);
            jObject.AddPropertyFloat(nameof(Left), Left);
            jObject.AddPropertyFloat(nameof(Top), Top);
            jObject.AddPropertyFloat(nameof(StartWidth), startWidth, 0.1f);
            jObject.AddPropertyFloat(nameof(EndWidth), endWidth, 0.1f);
            jObject.AddPropertyFloat(nameof(MajorInterval), MajorInterval, 10f);
            jObject.AddPropertyFloat(nameof(MinorInterval), MinorInterval, 5f);
            jObject.AddPropertyBool(nameof(IsReversed), IsReversed);
            jObject.AddPropertyFloat(nameof(Minimum), minimum);
            jObject.AddPropertyFloat(nameof(Maximum), maximum, 100f);
            jObject.AddPropertyBrush(nameof(Brush), Brush);
            jObject.AddPropertyBrush(nameof(BorderBrush), BorderBrush);
            jObject.AddPropertyJObject(nameof(Items), Items.SaveToJsonObject(mode));
            jObject.AddPropertyBool(nameof(DateTimeMode), DateTimeMode);

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

                    case nameof(Left):
                        this.Left = property.DeserializeFloat();
                        break;

                    case nameof(Top):
                        this.Top = property.DeserializeFloat();
                        break;

                    case nameof(StartWidth):
                        this.StartWidth = property.DeserializeFloat();
                        break;

                    case nameof(EndWidth):
                        this.EndWidth = property.DeserializeFloat();
                        break;

                    case nameof(MajorInterval):
                        this.MajorInterval = property.DeserializeFloat();
                        break;

                    case nameof(MinorInterval):
                        this.MinorInterval = property.DeserializeFloat();
                        break;

                    case nameof(IsReversed):
                        this.IsReversed = property.DeserializeBool();
                        break;

                    case nameof(DateTimeMode):
                        this.DateTimeMode = property.DeserializeBool();
                        break;

                    case nameof(Minimum):
                        this.Minimum = property.DeserializeFloat();
                        break;

                    case nameof(Maximum):
                        this.Maximum = property.DeserializeFloat();
                        break;
                        
                    case nameof(Brush):
                        this.Brush = property.DeserializeBrush();
                        break;

                    case nameof(BorderBrush):
                        this.BorderBrush = property.DeserializeBrush();
                        break;

                    case nameof(Items):
                        this.Items.LoadFromJsonObject((JObject)property.Value);
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
        public override object Clone()
        {
            var scale = (StiScaleBase)base.Clone();

            scale.ScaleHelper = this.ScaleHelper.Clone();
            scale.Brush = (StiBrush)this.Brush.Clone();
            scale.BorderBrush = (StiBrush)this.BorderBrush.Clone();

            scale.Items = new StiGaugeElementCollection(scale);
            lock (((ICollection)this.Items).SyncRoot)
            {
                foreach (StiGaugeElement element in this.Items)
                {
                    scale.Items.Add((StiGaugeElement)element.Clone());
                }
            }

            if (this is StiLinearScale)
            {
                scale.barGeometry = new StiLinearBarGeometry((StiLinearScale)scale);
            }
            else if(this is StiRadialScale)
            {
                scale.barGeometry = new StiRadialBarGeometry((StiRadialScale)scale);
            }
            
            return scale;
        }
        #endregion

        #region class ScaleHelper
        internal class StiScaleHelper
        {
            #region Fields
            public float ActualMinimum;
            public float ActualMaximum = 100f;
            public float MinWidth = 0.1f;
            public float MaxWidth = 0.1f;
            #endregion

            #region Properties
            private float totalLength = 100f;
            public float TotalLength
            {
                get
                {
                    return this.totalLength;
                }
                set
                {
                    if (value == 0)
                        this.totalLength = 1f;
                    else
                        this.totalLength = value;
                }
            }
            #endregion

            #region Methods
            public StiScaleHelper Clone()
            {
                return new StiScaleHelper
                {
                    ActualMinimum = ActualMinimum,
                    ActualMaximum = ActualMaximum,
                    MinWidth = MinWidth,
                    MaxWidth = MaxWidth,
                    totalLength = totalLength
                };
            }
            #endregion
        }
        #endregion

        #region Fields
        internal IStiScaleBarGeometry barGeometry;
        internal StiScaleHelper ScaleHelper = new StiScaleHelper();
        #endregion

        #region Properties.Internal
        [Browsable(false)]
        internal bool IsUp
        {
            get
            {
                bool isUp = (this.startWidth < this.endWidth);
                if (this.IsReversed) isUp = !isUp;
                return isUp;
            }
        }

        [Browsable(false)]
        [StiSerializable(StiSerializationVisibility.Class, StiSerializeTypes.SerializeToAll)]
        [DefaultValue(null)]
        public StiGauge Gauge { get; set; }

        [Browsable(false)]
        internal object SeriesKey { get; set; }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets X coordinate of the scale relative to height of the scale container.
        /// </summary>
        [DefaultValue(0f)]
        [StiSerializable]
        [StiCategory("Position")]
        [Description("Gets or sets X coordinate of the scale relative to height of the scale container.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float Left { get; set; }

        /// <summary>
        /// Gets or sets Y coordinate of the scale relative to height of the scale container.
        /// </summary>
        [DefaultValue(0f)]
        [StiSerializable]
        [StiCategory("Position")]
        [Description("Gets or sets Y coordinate of the scale relative to height of the scale container.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float Top { get; set; }

        private float startWidth = 0.1f;
        /// <summary>
        /// Gets or sets start width of the scale bar.
        /// </summary>
        [DefaultValue(0.1f)]
        [StiSerializable]
        [StiCategory("Scale")]
        [Description("Gets or sets start width of the scale bar.")]
        [StiOrder(StiPropertyOrder.ScaleStartWidth)]
        [StiPropertyLevel(StiLevel.Basic)]
        public float StartWidth
        {
            get
            {
                return this.startWidth;
            }
            set
            {
                this.startWidth = value;
                CalculateWidthScaleHelper();
            }
        }

        private float endWidth = 0.1f;
        /// <summary>
        /// Gets or sets end width of the scale bar.
        /// </summary>
        [DefaultValue(0.1f)]
        [StiSerializable]
        [StiCategory("Scale")]
        [Description("Gets or sets end width of the scale bar.")]
        [StiOrder(StiPropertyOrder.ScaleEndWidth)]
        [StiPropertyLevel(StiLevel.Basic)]
        public float EndWidth
        {
            get
            {
                return this.endWidth;
            }
            set
            {
                this.endWidth = value;
                CalculateWidthScaleHelper();
            }
        }

        /// <summary>
        /// Gets or sets the major interval.
        /// </summary>
        [DefaultValue(10f)]
        [StiSerializable]
        [StiCategory("Scale")]
        [Description("Gets or sets the major interval.")]
        [StiOrder(StiPropertyOrder.ScaleMajorInterval)]
        [StiPropertyLevel(StiLevel.Basic)]
        public float MajorInterval { get; set; } = 10f;

        /// <summary>
        /// Gets or sets the minor interval.
        /// </summary>
        [DefaultValue(5f)]
        [StiSerializable]
        [StiCategory("Scale")]
        [Description("Gets or sets the minor interval.")]
        [StiOrder(StiPropertyOrder.ScaleMinorInterval)]
        [StiPropertyLevel(StiLevel.Basic)]
        public float MinorInterval { get; set; } = 5f;

        private float minimum = 0f;
        /// <summary>
        /// Gets or sets start value of the scale.
        /// </summary>
        [DefaultValue(0f)]
        [StiSerializable]
        [StiCategory("Scale")]
        [Description("Gets or sets start value of the scale.")]
        [StiOrder(StiPropertyOrder.ScaleMinimum)]
        [StiPropertyLevel(StiLevel.Basic)]
        public float Minimum
        {
            get
            {
                return this.minimum;
            }
            set
            {
                this.minimum = value;
                CalculateMinMaxScaleHelper();
            }
        }

        private float maximum = 100f;
        /// <summary>
        /// Gets or sets end value of the scale.
        /// </summary>
        [DefaultValue(100f)]
        [StiSerializable]
        [StiCategory("Scale")]
        [Description("Gets or sets end value of the scale.")]
        [StiOrder(StiPropertyOrder.ScaleMaximum)]
        [StiPropertyLevel(StiLevel.Basic)]
        public float Maximum
        {
            get
            {
                return this.maximum;
            }
            set
            {
                this.maximum = value;
                CalculateMinMaxScaleHelper();
            }
        }

        /// <summary>
        /// Gets or sets value that indicates whether the scale should be shown in reverse mode.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Scale")]
        [Description("Gets or sets value that indicates whether the scale should be shown in reverse mode.")]
        [StiOrder(StiPropertyOrder.ScaleIsReversed)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        public bool IsReversed { get; set; }

        /// <summary>
        /// Gets or sets a brush to fill a component.
        /// </summary>
        [StiSerializable]
        [StiPropertyLevel(StiLevel.Basic)]
        [StiCategory("Appearance")]
        [Description("Gets or sets a brush to fill a component.")]
        [StiOrder(StiPropertyOrder.AppearanceBrush)]
        public StiBrush Brush { get; set; } = new StiSolidBrush(Color.FromArgb(50, Color.White));

        /// <summary>
        /// Gets or sets the border of the component.
        /// </summary>
        [StiSerializable]
        [StiPropertyLevel(StiLevel.Basic)]
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceBorder)]
        [Description("Gets or sets the border of the component.")]
        public StiBrush BorderBrush { get; set; } = new StiSolidBrush(Color.FromArgb(150, Color.White));

        [Browsable(false)]
        [StiSerializable(StiSerializationVisibility.List)]
        public StiGaugeElementCollection Items { get; set; }

        [StiSerializable]
        [DefaultValue(false)]
        [Browsable(false)]
        public bool DateTimeMode { get; set; }
        #endregion

        #region Properties abstract
        [Browsable(false)]
        public abstract StiGaugeElemenType ScaleType { get; }
        #endregion

        #region Methods
        internal void Prepare()
        {
            foreach (StiGaugeElement element in this.Items)
            {
                element.PrepareGaugeElement();
            }
        }

        internal void CalculateMinMaxScaleHelper()
        {
            this.ScaleHelper.ActualMaximum = Math.Max(this.Maximum, this.Minimum);
            this.ScaleHelper.ActualMinimum = Math.Min(this.Maximum, this.Minimum);

            this.ScaleHelper.TotalLength = this.ScaleHelper.ActualMaximum - this.ScaleHelper.ActualMinimum;
        }

        internal void CalculateWidthScaleHelper()
        {
            if (this.StartWidth > this.EndWidth)
            {
                this.ScaleHelper.MaxWidth = this.StartWidth;
                this.ScaleHelper.MinWidth = this.EndWidth;
            }
            else
            {
                this.ScaleHelper.MaxWidth = this.EndWidth;
                this.ScaleHelper.MinWidth = this.StartWidth;
            }
        }

        internal float GetPosition(float value)
        {
            var value1 = value;
            if (value1 < this.ScaleHelper.ActualMinimum)
                value1 = this.ScaleHelper.ActualMinimum;
            else if (value1 > this.ScaleHelper.ActualMaximum)
                value1 = this.ScaleHelper.ActualMaximum;

            return StiMathHelper.Length(this.ScaleHelper.ActualMinimum, value1) / this.ScaleHelper.TotalLength;
        }
        #endregion

        #region Methods abstract
        protected abstract void InteractiveClick(MouseEventArgs e);
        #endregion

        #region Methods virtual
        public virtual StiScaleBase CreateNew() => throw new NotImplementedException();
        #endregion

        #region Methods override
        protected internal override void DrawElement(StiGaugeContextPainter context)
        {
            if (this.Gauge != null)
            {
                barGeometry.DrawScaleGeometry(context);

                foreach (StiElementBase item in this.Items)
                {
                    item.DrawElement(context);
                }
            }
        }
        #endregion

        public StiScaleBase()
        {
            this.Items = new StiGaugeElementCollection(this);
        }
    }
}