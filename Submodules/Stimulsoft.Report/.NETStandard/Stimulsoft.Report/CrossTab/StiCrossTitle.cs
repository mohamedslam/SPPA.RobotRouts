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

using System.Drawing;
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Design;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.CrossTab
{
	public class StiCrossTitle : StiCrossField
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("TextFormat");
            jObject.RemoveProperty("HideZeros");
            jObject.RemoveProperty("Conditions");

            // StiCrossTitle
            jObject.AddPropertyBool("PrintOnAllPages", PrintOnAllPages, true);
            jObject.AddPropertyStringNullOrEmpty("TypeOfComponent", TypeOfComponent);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "PrintOnAllPages":
                        this.PrintOnAllPages = property.DeserializeBool();
                        break;

                    case "TypeOfComponent":
                        this.TypeOfComponent = property.DeserializeString();
                        break;
                }
            }
        }

        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiCrossTitle;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var collection = new StiPropertyCollection();
            var propHelper = propertyGrid.PropertiesHelper;

            collection.Add(StiPropertyCategories.Text, new[]
            {
                propHelper.Text()
            });

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.TextAdditional, new[]
                {
                    propHelper.TextBrush(),
                    propHelper.fAngle(),
                    propHelper.Font(),
                    propHelper.WordWrap()
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.TextAdditional, new[]
                {
                    propHelper.AllowHtmlTags(),
                    propHelper.TextBrush(),
                    propHelper.fAngle(),
                    propHelper.Font(),
                    propHelper.Margins(),
                    propHelper.WordWrap()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.TextAdditional, new[]
                {
                    propHelper.AllowHtmlTags(),
                    propHelper.TextBrush(),
                    propHelper.fAngle(),
                    propHelper.Font(),
                    propHelper.Margins(),
                    propHelper.TextOptions(),
                    propHelper.WordWrap()
                });
            }
            
            if (level != StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.MinSize(),
                    propHelper.MaxSize()
                });
            }

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Appearance, new[]
                {
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.ComponentStyle(),
                    propHelper.HorAlignment(),
                    propHelper.VertAlignment()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Appearance, new[]
                {
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.ComponentStyle(),
                    propHelper.HorAlignment(),
                    propHelper.VertAlignment(),
                    propHelper.UseParentStyles()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.MergeHeaders(),
                    propHelper.Enabled(),
                    propHelper.PrintOnAllPages()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.MergeHeaders(),
                    propHelper.Enabled(),
                    propHelper.PrintOnAllPages()
                });
            }

            return collection;
        }
        #endregion

        #region StiComponent override
        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiCrossTitle");
        #endregion

        #region Properties
        /// <summary>
		/// Gets or sets value indicates that the component is printed on all pages.
		/// </summary>
		[DefaultValue(true)]
		[StiSerializable]
		[StiCategory("Behavior")]
		[StiOrder(StiPropertyOrder.BehaviorPrintOnAllPages)]
		[TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that the component is printed on all pages.")]
		public virtual bool PrintOnAllPages { get; set; } = true;

        [StiSerializable]
		[Browsable(false)]
		public string TypeOfComponent { get; set; } = string.Empty;

        [Browsable(true)]
		public sealed override bool Enabled
		{
			get 
			{
				return base.Enabled;
			}
			set 
			{
				base.Enabled = value;
			}
		}

		[StiNonSerialized]
		[Browsable(false)]
		public override StiFormatService TextFormat
		{
			get 
			{
				return base.TextFormat;
			}
			set 
			{
			}
		}

		[StiNonSerialized]
		[Browsable(false)]
		public override bool HideZeros
		{
			get 
			{
				return base.HideZeros;
			}
			set 
			{
			}
		}

		[StiNonSerialized]
		[Browsable(false)]
		public override StiConditionsCollection Conditions
		{
			get
			{
				return base.Conditions;
			}
			set
			{
			}
		}

        public override string CellText => this.GetTextInternal();
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiCrossTitle();
        }
        #endregion

        public StiCrossTitle()
		{
			Brush = new StiSolidBrush(Color.LightGray);
		}
	}
}