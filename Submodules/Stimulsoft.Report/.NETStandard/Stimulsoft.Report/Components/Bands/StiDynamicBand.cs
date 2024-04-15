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
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Engine;
using Stimulsoft.Base.Json.Linq;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Describes the base class for static bands.
    /// </summary>
    [StiToolbox(false)]
    public abstract class StiDynamicBand :
        StiBand,
        IStiPageBreak,
        IStiPrintAtBottom
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiDynamicBand
            jObject.AddPropertyBool("PrintAtBottom", PrintAtBottom);
            jObject.AddPropertyBool("NewPageBefore", NewPageBefore);
            jObject.AddPropertyBool("NewPageAfter", NewPageAfter);
            jObject.AddPropertyBool("NewColumnBefore", NewColumnBefore);
            jObject.AddPropertyBool("NewColumnAfter", NewColumnAfter);
            jObject.AddPropertyBool("SkipFirst", SkipFirst, true);
            jObject.AddPropertyFloat("BreakIfLessThan", BreakIfLessThan, 100f);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "PrintAtBottom":
                        this.printAtBottom = property.DeserializeBool();
                        break;

                    case "NewPageBefore":
                        this.NewPageBefore = property.DeserializeBool();
                        break;

                    case "NewPageAfter":
                        this.NewPageAfter = property.DeserializeBool();
                        break;

                    case "NewColumnBefore":
                        this.NewColumnBefore = property.DeserializeBool();
                        break;

                    case "NewColumnAfter":
                        this.NewColumnAfter = property.DeserializeBool();
                        break;

                    case "SkipFirst":
                        this.skipFirst = property.DeserializeBool();
                        break;

                    case "BreakIfLessThan":
                        this.breakIfLessThan = property.DeserializeFloat();
                        break;
                }
            }
        }
        #endregion

        #region IStiPrintAtBottom
        private bool printAtBottom;
        /// <summary>
        /// Gets or sets value indicates that the footer is printed at bottom of page.
        /// </summary>
        [DefaultValue(false)]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorPrintAtBottom)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that the footer is printed at bottom of page.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool PrintAtBottom
        {
            get
            {
                return printAtBottom;
            }
            set
            {
                if (printAtBottom != value)
                {
                    CheckBlockedException("PrintAtBottom");
                    printAtBottom = value;
                }
            }
        }
        #endregion

        #region IStiBreakable override
        /// <summary>
        /// Gets or sets value which indicates whether the component can or cannot break its contents on several pages.
        /// </summary>        
        [Browsable(true)]
        [Description("Gets or sets value which indicates whether the component can or cannot break its contents on several pages.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Basic)]
        public override bool CanBreak
        {
            get
            {
                return base.CanBreak;
            }
            set
            {
                base.CanBreak = value;
            }
        }

        /// <summary>
        /// Divides content of components in two parts. Returns result of dividing. If true, then component is successful divided.
        /// </summary>
        /// <param name="dividedComponent">Component for store part of content.</param>
        /// <returns>If true, then component is successful divided.</returns>
        public override bool Break(StiComponent dividedComponent, double devideFactor, ref double divLine)
        {
            return false;
        }
        #endregion

        #region IStiPageBreak
        /// <summary>
		/// If the value of this property is true, then, before output of a band, a new page will be
		/// generated. Output of a band will be continued on the next page.
		/// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("PageColumnBreak")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("If the value of this property is true, then, before output of a band, a new page will be generated. Output of a band will be continued on the next page.")]
        [StiOrder(StiPropertyOrder.PageColumnBreakNewPageBefore)]
        [StiEngine(StiEngineVersion.EngineV2)]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual bool NewPageBefore { get; set; }

        /// <summary>
		/// If the value of this property is true, then, after output of a band, a new page will be
		/// generated.
		/// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("PageColumnBreak")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("If the value of this property is true, then, after output of a band, a new page will be generated.")]
        [StiOrder(StiPropertyOrder.PageColumnBreakNewPageAfter)]
        [StiEngine(StiEngineVersion.EngineV2)]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual bool NewPageAfter { get; set; }

        /// <summary>
		/// If the value of this property is true, then, before output of a band,
		/// a new column will be generated. Output of a band will be continued on
		///	the next column.
		/// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("PageColumnBreak")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("If the value of this property is true, then, before output of a band, a new column will be generated. Output of a band will be continued on the next column.")]
        [StiOrder(StiPropertyOrder.PageColumnBreakNewColumnBefore)]
        [StiEngine(StiEngineVersion.EngineV2)]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual bool NewColumnBefore { get; set; }

        /// <summary>
		/// If the value of this property is true, then, after output of a band, a
		/// new column will be generated.
		/// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("PageColumnBreak")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("If the value of this property is true, then, after output of a band, a new column will be generated.")]
        [StiOrder(StiPropertyOrder.PageColumnBreakNewColumnAfter)]
        [StiEngine(StiEngineVersion.EngineV2)]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual bool NewColumnAfter { get; set; }

        private bool skipFirst = true;
        /// <summary>
        /// If the value of this property is true, then, a new page/column will be
        /// generated only starting from the second case.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("PageColumnBreak")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("If the value of this property is true, then, a new page/column will be generated only starting from the second case.")]
        [StiOrder(StiPropertyOrder.PageColumnBreakSkipFirst)]
        [StiEngine(StiEngineVersion.EngineV2)]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool SkipFirst
        {
            get
            {
                return skipFirst;
            }
            set
            {
                if (skipFirst != value)
                {
                    CheckBlockedException("SkipFirst");
                    skipFirst = value;
                }
            }
        }

        private float breakIfLessThan = 100f;
        /// <summary>
        /// Gets or sets value which indicates how much free space is on a page
        /// (in per cent) should be
        /// reserved for formation of a new page or a new column. The value
        /// should be set in the range from 0 to 100.
        /// If the value is 100 then, in any case, a new page or a new column
        /// will be formed. This property is used
        /// together with NewPageBefore, NewPageAfter, NewColumnBefore,
        /// NewColumnAfter properties.
        /// </summary>
        [DefaultValue(100f)]
        [StiSerializable]
        [StiCategory("PageColumnBreak")]
        [Browsable(true)]
        [StiOrder(StiPropertyOrder.PageColumnBreakBreakIfLessThan)]
        [StiEngine(StiEngineVersion.EngineV2)]
        [Description("Gets or sets value which indicates how much free space is on a page " +
            "(in per cent) should be " +
            "reserved for formation of a new page or a new column. The value " +
            "should be set in the range from 0 to 100. " +
            "If the value is 100 then, in any case, a new page or a new column " +
            "will be formed. This property is used " +
            "together with NewPageBefore, NewPageAfter, NewColumnBefore, " +
            "NewColumnAfter properties.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual float BreakIfLessThan
        {
            get
            {
                return breakIfLessThan;
            }
            set
            {
                if (breakIfLessThan == value) return;

                if (value < 0 || value > 100)
                {
                    throw new ArgumentOutOfRangeException(
                        "BreakIfLessThan",
                        $"Value of '{value}' is not valid for 'BreakIfLessThan'. 'BreakIfLessThan' should be between 0% and 100%.");
                }

                if (value >= 0 && value <= 100)
                    breakIfLessThan = value;
            }
        }
        #endregion

        /// <summary>
        /// Creates a new dynamic band.
        /// </summary>
        public StiDynamicBand() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new dynamic band.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the component.</param>
        public StiDynamicBand(RectangleD rect) : base(rect)
        {
        }
    }
}
