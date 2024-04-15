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
using Stimulsoft.Report.Engine;
using System.ComponentModel;

namespace Stimulsoft.Report.Components
{
    public class StiBandInteraction : StiInteraction
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiBandInteraction
            jObject.AddPropertyBool("CollapsingEnabled", CollapsingEnabled);
            jObject.AddPropertyBool("SelectionEnabled", SelectionEnabled);
            jObject.AddPropertyBool("CollapseGroupFooter", CollapseGroupFooter);
            
            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "CollapsingEnabled":
                        this.CollapsingEnabled = property.DeserializeBool();
                        break;

                    case "SelectionEnabled":
                        this.SelectionEnabled = property.DeserializeBool();
                        break;

                    case "CollapseGroupFooter":
                        this.CollapseGroupFooter = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

		#region Properties
		[Browsable(false)]
		public override bool IsDefault => base.IsDefault && !CollapsingEnabled && !SelectionEnabled;

        [Browsable(false)]
        public override bool IsDefaultExt
        {
            get
            {
                return 
                    IsDefaultSorting &&
                    IsDefaultDrillDown &&
                    string.IsNullOrEmpty(Bookmark?.Value) &&
                    string.IsNullOrEmpty(Hyperlink?.Value) &&
                    string.IsNullOrEmpty(Tag?.Value) &&
                    string.IsNullOrEmpty(ToolTip?.Value);
            }
        }
        #endregion

        #region Properties.Sorting
        /// <summary>
        /// Gets or sets value which indicates whether it is allowed or not data collapsing in the report viewer.
        /// </summary>
        [StiSerializable]
		[DefaultValue(false)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Description("Gets or sets value which indicates whether it is allowed or not data collapsing in the report viewer.")]
		[StiOrder(StiPropertyOrder.InteractionCollapsingEnabled)]
		[StiEngine(StiEngineVersion.EngineV2)]
		public virtual bool CollapsingEnabled { get; set; }

	    /// <summary>
        /// Gets or sets a value which indicates whether it is allowed to select one data row which is output by this DataBand.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Description("Gets or sets a value which indicates whether it is allowed to select one data row which is output by this DataBand.")]
        [StiOrder(StiPropertyOrder.InteractionSelectionEnabled)]
        [StiEngine(StiEngineVersion.EngineV2)]
        public virtual bool SelectionEnabled { get; set; }

	    /// <summary>
		/// Gets or sets value which indicates whether it is necessary GroupFooter collapsing.
		/// </summary>
		[StiSerializable]
		[DefaultValue(false)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Description("Gets or sets value which indicates whether it is necessary GroupFooter collapsing.")]
		[StiOrder(StiPropertyOrder.InteractionCollapseGroupFooter)]
		[StiEngine(StiEngineVersion.EngineV2)]
		public virtual bool CollapseGroupFooter { get; set; }

	    /// <summary>
		/// Gets or sets the boolean expression which indicates whether a group, when rendering, should be collapsed or not.
		/// </summary>
		[Description("Gets or sets the boolean expression which indicates whether a group, when rendering, should be collapsed or not.")]
		[StiOrder(StiPropertyOrder.InteractionCollapsed)]
		[StiEngine(StiEngineVersion.EngineV2)]
		public virtual StiCollapsedExpression Collapsed
		{
			get
			{
			    return ParentComponent is StiGroupHeaderBand 
			        ? ((StiGroupHeaderBand)ParentComponent).Collapsed 
			        : (ParentComponent is StiDataBand ? ((StiDataBand)ParentComponent).Collapsed : new StiCollapsedExpression());
			}
			set
			{
				if (ParentComponent is StiGroupHeaderBand)
					((StiGroupHeaderBand)ParentComponent).Collapsed = value;
				else
					((StiDataBand)ParentComponent).Collapsed = value;
			}
		}

        [Browsable(false)]
        [StiNonSerialized]
        [DefaultValue(null)]
        public int? SelectedLine { get; set; }
		#endregion
	}
}
