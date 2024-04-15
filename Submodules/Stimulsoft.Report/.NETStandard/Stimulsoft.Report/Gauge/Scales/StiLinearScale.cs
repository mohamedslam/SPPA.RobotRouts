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

using System.ComponentModel;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components.Gauge.Primitives;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.PropertyGrid;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Windows.Forms;
#else
using System.Drawing.Design;
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Components.Gauge
{
    public class StiLinearScale : 
        StiScaleBase
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            jObject.AddPropertyEnum(nameof(Orientation), Orientation);
            jObject.AddPropertyFloat(nameof(RelativeHeight), RelativeHeight, 0.9f);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Orientation):
                        this.Orientation = property.DeserializeEnum<Orientation>();
                        break;

                    case nameof(RelativeHeight):
                        this.RelativeHeight = property.DeserializeFloat();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiLinearScale;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var checkBoxHelper = new StiPropertyCollection();

            // ScaleCategory
            var list = new[]
            {
                propHelper.Minimum(),
                propHelper.Maximum(),
                propHelper.MajorInterval(),
                propHelper.MinorInterval(),
                propHelper.StartWidth(),
                propHelper.EndWidth(),
                propHelper.IsReversed(),
                propHelper.Orientation(),
                propHelper.RelativeHeight()
            };
            checkBoxHelper.Add(StiPropertyCategories.Scale, list);

            // PositionCategory
            list = new[]
            {
                propHelper.LeftF(),
                propHelper.TopF(),
            };
            checkBoxHelper.Add(StiPropertyCategories.Position, list);

            // AppearanceCategory
            list = new[]
            {
                propHelper.Brush(),
                propHelper.BorderBrush()
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

        #region IStiApplyStyleGauge
        public override void ApplyStyle(IStiGaugeStyle style)
        {
            //if (!StiGaugeV2InitHelper.AllowOldEditor)
            if (this.AllowApplyStyle)
                this.Brush = style.Core.LinearScaleBrush;

            foreach (StiGaugeElement item in this.Items)
            {
                item.ApplyStyle(style);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets linearScale orientation.
        /// </summary>
        [StiSerializable]
        [DefaultValue(Orientation.Vertical)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Scale")]
        [Description("Gets or sets linearScale orientation.")]
        [StiOrder(StiPropertyOrder.ScaleOrientation)]
        [StiPropertyLevel(StiLevel.Basic)]
        public Orientation Orientation { get; set; } = Orientation.Vertical;

        /// <summary>
        /// Gets or sets the height factor relative to the height of the basic component.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0.9f)]
        [StiCategory("Scale")]
        [StiOrder(StiPropertyOrder.ScaleRelativeHeight)]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the height factor relative to the height of the basic component.")]
        public float RelativeHeight { get; set; } = 0.9f;
        #endregion

        #region Properties overrider
        public override StiGaugeElemenType ScaleType => StiGaugeElemenType.LinearElement;
        #endregion

        #region Methods override
        protected override void InteractiveClick(MouseEventArgs e)
        {

        }

        public override StiScaleBase CreateNew() => new StiLinearScale();
        #endregion

        public StiLinearScale()
        {
            barGeometry = new StiLinearBarGeometry(this);
        }
    }
}