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
using Stimulsoft.Report.Gauge.Collections;
using System.Collections;
using System.ComponentModel;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components.Gauge.Primitives
{
    public abstract class StiBarBase : StiIndicatorBase
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);
                        
            jObject.AddPropertyBrush(nameof(EmptyBrush), EmptyBrush);
            jObject.AddPropertyBrush(nameof(EmptyBorderBrush), EmptyBorderBrush);
            jObject.AddPropertyFloat(nameof(EmptyBorderWidth), EmptyBorderWidth);
            jObject.AddPropertyFloat(nameof(Offset), Offset);
            jObject.AddPropertyFloat(nameof(StartWidth), StartWidth);
            jObject.AddPropertyFloat(nameof(EndWidth), EndWidth, 0.05f);
            jObject.AddPropertyBool(nameof(UseRangeColor), UseRangeColor);
            jObject.AddPropertyJObject(nameof(RangeList), RangeList.SaveToJsonObject(mode));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(EmptyBrush):
                        this.EmptyBrush = property.DeserializeBrush();
                        break;

                    case nameof(EmptyBorderBrush):
                        this.EmptyBorderBrush = property.DeserializeBrush();
                        break;

                    case nameof(EmptyBorderWidth):
                        this.EmptyBorderWidth = property.DeserializeFloat();
                        break;

                    case nameof(Offset):
                        this.Offset = property.DeserializeFloat();
                        break;

                    case nameof(StartWidth):
                        this.StartWidth = property.DeserializeFloat();
                        break;

                    case nameof(EndWidth):
                        this.EndWidth = property.DeserializeFloat();
                        break;

                    case nameof(UseRangeColor):
                        this.UseRangeColor = property.DeserializeBool();
                        break;

                    case nameof(RangeList):
                        this.RangeList.LoadFromJsonObject((JObject)property.Value);
                        break;
                }
            }
        }
        #endregion

        #region ICloneable
        public override object Clone()
        {
            var indicator = (StiBarBase)base.Clone();

            indicator.EmptyBrush = (StiBrush)this.EmptyBrush.Clone();
            indicator.EmptyBorderBrush = (StiBrush)this.EmptyBorderBrush.Clone();

            indicator.RangeList = new StiBarRangeListCollection(BarType);
            lock (((ICollection)this.RangeList).SyncRoot)
            {
                foreach (StiIndicatorRangeInfo info in this.RangeList) indicator.RangeList.Add((StiIndicatorRangeInfo)info.Clone());
            }

            return indicator;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the brush for the empty part of the bar indicator.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [StiOrder(210)]
        [Description("Gets or sets the brush for the empty part of the bar indicator.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiBrush EmptyBrush { get; set; } = new StiEmptyBrush();

        /// <summary>
        /// Gets or sets the border brush for the empty part of the bar indicator.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [StiOrder(220)]
        [Description("Gets or sets the border brush for the empty part of the bar indicator.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiBrush EmptyBorderBrush { get; set; } = new StiEmptyBrush();

        /// <summary>
        /// Gets or sets the border width for the empty part of the bar indicator.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0f)]
        [StiOrder(230)]
        [Description("Gets or sets the border width for the empty part of the bar indicator.")]
        [StiCategory("Appearance")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float EmptyBorderWidth { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the offset ratio of an item.
        /// </summary>
        [DefaultValue(0f)]
        [StiSerializable]
        [StiCategory("Indicator")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the offset ratio of an item.")]
        public float Offset { get; set; } = 0f;

        /// <summary>
        /// Gets or sets start width of the bar indicator.
        /// </summary>
        [DefaultValue(0.0f)]
        [StiSerializable]
        [StiCategory("Indicator")]
        [Description("Gets or sets start width of the bar indicator.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float StartWidth { get; set; } = 0.05f;

        /// <summary>
        /// Gets or sets end width of the bar indicator.
        /// </summary>
        [DefaultValue(0.0f)]
        [StiSerializable]
        [StiCategory("Indicator")]
        [Description("Gets or sets end width of the bar indicator.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public float EndWidth { get; set; } = 0.05f;

        private bool useRangeColor = false;
        /// <summary>
        /// Gets or sets value which indicates whether we should use Brush property of the StiIndicatorBase class as brush for this indicator.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Indicator")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates whether we should use Brush property of the StiIndicatorBase class as brush for this indicator.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public bool UseRangeColor
        {
            get
            {
                return this.useRangeColor;
            }
            set
            {
                this.useRangeColor = value;
                OnRangeColorChanged();
            }
        }

        [Browsable(false)]
        [StiSerializable(StiSerializationVisibility.List)]
        public StiBarRangeListCollection RangeList { get; set; }
        #endregion

        #region Properties abstract
        [Browsable(false)]
        protected abstract StiBarRangeListType BarType { get; }
        #endregion

        #region Methods
        protected abstract void OnRangeColorChanged();
        protected abstract void CheckActualBrushForTopGeometry();
        #endregion

        #region Methods override
        protected override void OnValueChanged()
        {
            this.CheckActualBrushForTopGeometry();
        }
        #endregion

        public StiBarBase()
        {
            this.RangeList = new StiBarRangeListCollection(BarType);
        }
    }
}