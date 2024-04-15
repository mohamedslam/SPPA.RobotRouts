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
using Stimulsoft.Base;
using Stimulsoft.Report.CrossTab.Core;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections;
using Stimulsoft.Base.Design;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.CrossTab
{
    public class StiCrossSummary : StiCrossCell
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.AddPropertyEnum("HorAlignment", HorAlignment, StiTextHorAlignment.Right);
            jObject.AddPropertyBool("HideZeros", HideZeros, true);

            // StiCrossSummary
            jObject.AddPropertyBool("AspectRatio", AspectRatio);
            jObject.AddPropertyBool("Stretch", Stretch, true);
            jObject.AddPropertyEnum("Summary", Summary, StiSummaryType.Sum);
            jObject.AddPropertyEnum("SummaryValues", SummaryValues, StiSummaryValues.AllValues);
            jObject.AddPropertyBool("UseStyleOfSummaryInRowTotal", UseStyleOfSummaryInRowTotal);
            jObject.AddPropertyBool("UseStyleOfSummaryInColumnTotal", UseStyleOfSummaryInColumnTotal);
            jObject.AddPropertyBool("ShowPercents", ShowPercents);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "AspectRatio":
                        this.AspectRatio = property.DeserializeBool();
                        break;

                    case "Stretch":
                        this.Stretch = property.DeserializeBool();
                        break;

                    case "Summary":
                        this.Summary = property.DeserializeEnum<StiSummaryType>();
                        break;

                    case "SummaryValues":
                        this.SummaryValues = property.DeserializeEnum<StiSummaryValues>();
                        break;

                    case "UseStyleOfSummaryInRowTotal":
                        this.UseStyleOfSummaryInRowTotal = property.DeserializeBool();
                        break;

                    case "UseStyleOfSummaryInColumnTotal":
                        this.UseStyleOfSummaryInColumnTotal = property.DeserializeBool();
                        break;

                    case "ShowPercents":
                        this.ShowPercents = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiCrossSummary;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var objHelper = new StiPropertyCollection();

            var propHelper = propertyGrid.PropertiesHelper;
            StiPropertyObject[] list;

            var isSummaryImage = this.Summary == StiSummaryType.Image;

            // DataCategory
            list = new[]
            {
                propHelper.ShowPercents(),
                propHelper.Summary(),
                propHelper.SummaryValues(),
                propHelper.Value()
            };
            objHelper.Add(StiPropertyCategories.Data, list);

            // ImageAdditionalCategory
            if (isSummaryImage)
            {
                list = new[]
                {
                    propHelper.ImageHorAlignment(),
                    propHelper.ImageVertAlignment(),
                    propHelper.AspectRatio(),
                    propHelper.Stretch()
                };
                objHelper.Add(StiPropertyCategories.ImageAdditional, list);
            }

            // TextAdditionalCategory
            if (!isSummaryImage)
            {
                if (level == StiLevel.Basic)
                {
                    list = new[]
                    {
                        propHelper.TextBrush(),
                        propHelper.fAngle(),
                        propHelper.Font(),
                        propHelper.HideZeros(),
                        propHelper.HorAlignment(),
                        propHelper.VertAlignment(),
                        propHelper.TextFormat(),
                        propHelper.WordWrap()
                    };
                }
                else if (level == StiLevel.Standard)
                {
                    list = new[]
                    {
                        propHelper.AllowHtmlTags(),
                        propHelper.TextBrush(),
                        propHelper.fAngle(),
                        propHelper.Font(),
                        propHelper.HideZeros(),
                        propHelper.HorAlignment(),
                        propHelper.VertAlignment(),
                        propHelper.Margins(),
                        propHelper.TextFormat(),
                        propHelper.WordWrap()
                    };
                }
                else
                {
                    list = new[]
                    {
                        propHelper.AllowHtmlTags(),
                        propHelper.TextBrush(),
                        propHelper.fAngle(),
                        propHelper.Font(),
                        propHelper.HideZeros(),
                        propHelper.HorAlignment(),
                        propHelper.VertAlignment(),
                        propHelper.Margins(),
                        propHelper.TextFormat(),
                        propHelper.TextOptions(),
                        propHelper.WordWrap()
                    };
                }

                objHelper.Add(StiPropertyCategories.TextAdditional, list);
            }

            // PositionCategory
            if (level != StiLevel.Basic)
            {
                list = new[]
                {
                    propHelper.MinSize(),
                    propHelper.MaxSize()
                };
                objHelper.Add(StiPropertyCategories.Position, list);
            }

            // AppearanceCategory
            if (level == StiLevel.Basic)
            {
                list = new[]
                {
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.Conditions(),
                    propHelper.ComponentStyle(),
                    propHelper.UseStyleOfSummaryInRowTotal(),
                    propHelper.UseStyleOfSummaryInColumnTotal(),
                };
            }
            else
            {
                list = new[]
                {
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.Conditions(),
                    propHelper.ComponentStyle(),
                    propHelper.UseParentStyles(),
                    propHelper.UseStyleOfSummaryInRowTotal(),
                    propHelper.UseStyleOfSummaryInColumnTotal(),
                };
            }

            objHelper.Add(StiPropertyCategories.Appearance, list);

            // BehaviorCategory
            if (level == StiLevel.Basic)
            {
                list = new[]
                {
                    propHelper.MergeHeaders()
                };
            }
            else
            {
                list = new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.MergeHeaders()
                };
            }

            objHelper.Add(StiPropertyCategories.Behavior, list);

            return objHelper;
        }
        #endregion

        #region ICloneable override
        public override object Clone(bool cloneProperties)
        {
            var crossSummary = (StiCrossSummary) base.Clone(cloneProperties);

            crossSummary.AspectRatio = this.AspectRatio;
            crossSummary.Stretch = this.Stretch;
            crossSummary.ImageHorAlignment = this.ImageHorAlignment;
            crossSummary.ImageVertAlignment = this.ImageVertAlignment;
            crossSummary.Summary = this.Summary;
            crossSummary.SummaryValues = this.SummaryValues;
            crossSummary.UseStyleOfSummaryInRowTotal = this.UseStyleOfSummaryInRowTotal;
            crossSummary.UseStyleOfSummaryInColumnTotal = this.UseStyleOfSummaryInColumnTotal;

            return crossSummary;
        }
        #endregion

        #region IStiTextHorAlignment override
        protected override StiTextHorAlignment DefaultHorAlignment => StiTextHorAlignment.Right;

        /// <summary>
        /// Gets or sets the text horizontal alignment.
        /// </summary>
        [StiOrder(StiPropertyOrder.TextHorAlignment)]
        [StiCategory("TextAdditional")]
        [DefaultValue(StiTextHorAlignment.Right)]
        public override StiTextHorAlignment HorAlignment
        {
            get
            {
                return base.HorAlignment;
            }
            set
            {
                base.HorAlignment = value;
            }
        }
        #endregion

        #region IStiVertAlignment
        /// <summary>
        /// Gets or sets the vertical alignment of an object.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiVertAlignment.Center)]
        [StiCategory("TextAdditional")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets the vertical alignment of an object.")]
        [StiOrder(StiPropertyOrder.TextVertAlignment)]
        public override StiVertAlignment VertAlignment
        {
            get
            {
                return base.VertAlignment;
            }
            set
            {
                base.VertAlignment = value;
            }
        }
        #endregion

        #region Image Properties
        /// <summary>
        /// Gets or sets value, indicates that the image will save its aspect ratio.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("ImageAdditional")]
        [StiOrder(StiPropertyOrder.ImageAspectRatio)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value, indicates that the image will save its aspect ratio.")]
        public bool AspectRatio { get; set; }

        /// <summary>
        /// Gets or sets the horizontal alignment of an object.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiHorAlignment.Left)]
        [StiCategory("ImageAdditional")]
        [StiOrder(StiPropertyOrder.ImageHorAlignment)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets the horizontal alignment of an object.")]
        public StiHorAlignment ImageHorAlignment { get; set; } = StiHorAlignment.Left;

        /// <summary>
        /// Gets or sets the vertical alignment of an object.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiVertAlignment.Top)]
        [StiCategory("ImageAdditional")]
        [StiOrder(StiPropertyOrder.ImageVertAlignment)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets the vertical alignment of an object.")]
        public StiVertAlignment ImageVertAlignment { get; set; } = StiVertAlignment.Top;

        /// <summary>
        /// Gets or sets value, indicates that this component will stretch the image till the image will get size equal in its size on the page.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("ImageAdditional")]
        [StiOrder(StiPropertyOrder.ImageStretch)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value, indicates that this component will stretch the image till the image will get size equal in its size on the page.")]
        public bool Stretch { get; set; } = true;
        #endregion

        #region Properties
        [StiNonSerialized]
        [Browsable(false)]
        public string CrossColumnValue { get; set; }

        [StiNonSerialized]
        [Browsable(false)]
        public string CrossRowValue { get; set; }

        [StiNonSerialized]
        [Browsable(false)]
        public Hashtable Arguments { get; set; } = new Hashtable();

        /// <summary>
        /// Internal use only.
        /// </summary>
        [Browsable(false)]
        public int IndexOfSelectValue { get; set; } = -1;

        public override string CellText => "0";

        /// <summary>
        /// Gets or sets value indicates that no need show zeroes.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("TextAdditional")]
        [StiOrder(StiPropertyOrder.TextHideZeros)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that no need show zeroes.")]
        public override bool HideZeros
        {
            get
            {
                return base.HideZeros;
            }
            set
            {
                base.HideZeros = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of values summation.
        /// </summary>
        [Description("Gets or sets the type of values summation.")]
        [DefaultValue(StiSummaryType.Sum)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiSerializable]
        [StiCategory("Data")]
        public StiSummaryType Summary { get; set; } = StiSummaryType.Sum;

        /// <summary>
        /// Gets or sets the type of zeros and nulls values summation.
        /// </summary>
        [Description("Gets or sets the type of values summation.")]
        [DefaultValue(StiSummaryValues.AllValues)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiSerializable]
        [StiCategory("Data")]
        public StiSummaryValues SummaryValues { get; set; } = StiSummaryValues.AllValues;

        /// <summary>
        /// Gets or sets value which indicates that style of summary cell will be used in row total.
        /// </summary>
        [Description("Gets or sets value which indicates that style of summary cell will be used in row total.")]
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceUseStyleOfSummaryInRowTotal)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        public bool UseStyleOfSummaryInRowTotal { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that style of summary cell will be used in column total.
        /// </summary>
        [Description("Gets or sets value which indicates that style of summary cell will be used in column total.")]
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceUseStyleOfSummaryInColumnTotal)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        public bool UseStyleOfSummaryInColumnTotal { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that value in cell must be shown as percents.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Data")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that value in cell must be shown as percents.")]
        public virtual bool ShowPercents { get; set; }
        #endregion

        #region StiComponent override
        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiCrossSummary");
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiCrossSummary();
        }
        #endregion

        public StiCrossSummary()
        {
            base.HorAlignment = StiTextHorAlignment.Right;
            base.HideZeros = true;
        }
    }
}