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
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Engine;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components
{
    [TypeConverter(typeof(StiInteractionConverter))]
    [RefreshProperties(RefreshProperties.All)]
    public class StiInteraction :
        ICloneable,
        IStiSerializeToCodeAsClass,
        IStiInteractionClass,
        IStiReportProperty,
        IStiDefault,
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            // StiInteraction
            jObject.AddPropertyBool(nameof(SortingEnabled), SortingEnabled, true);
            jObject.AddPropertyStringNullOrEmpty(nameof(SortingColumn), SortingColumn);
            jObject.AddPropertyInt(nameof(SortingIndex), SortingIndex);
            jObject.AddPropertyEnum(nameof(SortingDirection), SortingDirection, StiInteractionSortDirection.None);
            jObject.AddPropertyBool(nameof(DrillDownEnabled), DrillDownEnabled);
            jObject.AddPropertyStringNullOrEmpty(nameof(DrillDownReport), DrillDownReport);
            jObject.AddPropertyEnum(nameof(DrillDownMode), DrillDownMode, StiDrillDownMode.MultiPage);
            jObject.AddPropertyStringNullOrEmpty(nameof(DrillDownPageGuid), DrillDownPageGuid);
            
            jObject.AddPropertyJObject(nameof(DrillDownParameter1), DrillDownParameter1.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(DrillDownParameter2), DrillDownParameter2.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(DrillDownParameter3), DrillDownParameter3.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(DrillDownParameter4), DrillDownParameter4.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(DrillDownParameter5), DrillDownParameter5.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(DrillDownParameter6), DrillDownParameter6.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(DrillDownParameter7), DrillDownParameter7.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(DrillDownParameter8), DrillDownParameter8.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(DrillDownParameter9), DrillDownParameter9.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(DrillDownParameter10), DrillDownParameter10.SaveToJsonObject(mode));

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(SortingEnabled):
                        this.SortingEnabled = property.DeserializeBool();
                        break;

                    case nameof(SortingColumn):
                        this.SortingColumn = property.DeserializeString();
                        break;

                    case nameof(SortingIndex):
                        this.SortingIndex = property.DeserializeInt();
                        break;

                    case nameof(SortingDirection):
                        this.SortingDirection = property.DeserializeEnum<StiInteractionSortDirection>();
                        break;

                    case nameof(DrillDownEnabled):
                        this.DrillDownEnabled = property.DeserializeBool();
                        break;

                    case nameof(DrillDownReport):
                        this.DrillDownReport = property.DeserializeString();
                        break;

                    case nameof(DrillDownMode):
                        this.DrillDownMode = property.DeserializeEnum<StiDrillDownMode>();
                        break;

                    case nameof(DrillDownPageGuid):
                        this.DrillDownPageGuid = property.DeserializeString();
                        break;

                    case nameof(DrillDownParameter1):
                        this.DrillDownParameter1.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(DrillDownParameter2):
                        this.DrillDownParameter2.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(DrillDownParameter3):
                        this.DrillDownParameter3.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(DrillDownParameter4):
                        this.DrillDownParameter4.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(DrillDownParameter5):
                        this.DrillDownParameter5.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(DrillDownParameter6):
                        this.DrillDownParameter6.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(DrillDownParameter7):
                        this.DrillDownParameter7.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(DrillDownParameter8):
                        this.DrillDownParameter8.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(DrillDownParameter9):
                        this.DrillDownParameter9.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(DrillDownParameter10):
                        this.DrillDownParameter10.LoadFromJsonObject((JObject)property.Value);
                        break;
                }
            }
        }

        internal static StiInteraction LoadInteractionFromJsonObject(JObject jObject)
        {
            var ident = jObject.Properties().FirstOrDefault(x => x.Name == "Ident").Value.ToObject<string>();

            StiInteraction interaction = null;
            switch (ident)
            {
                case "StiInteraction":
                    interaction = new StiInteraction();
                    break;

                case "StiBandInteraction":
                    interaction = new StiBandInteraction();
                    break;

                case "StiCrossHeaderInteraction":
                    interaction = new StiCrossHeaderInteraction();
                    break;
            }

            interaction.LoadFromJsonObject(jObject);
            return interaction;
        }
        #endregion

        #region IStiReportProperty
        public object GetReport()
        {
            return ParentComponent == null ? null : ParentComponent.Report;
        }
        #endregion

        #region ICloneable override
        public object Clone()
        {
            return (StiInteraction)base.MemberwiseClone();
        }
        #endregion

        #region IStiDefault
        [Browsable(false)]
        public virtual bool IsDefault
        {
            get
            {
                return
                    SortingEnabled &&
                    string.IsNullOrEmpty(SortingColumn) &&
                    IsDefaultDrillDown;
            }
        }

        [Browsable(false)]
        public virtual bool IsDefaultSorting => !(!string.IsNullOrWhiteSpace(SortingColumn) && SortingEnabled);

        [Browsable(false)]
        public virtual bool IsDefaultDrillDown
        {
            get
            {
                return !DrillDownEnabled &&
                    string.IsNullOrEmpty(DrillDownReport) &&
                    DrillDownPageGuid == null &&
                    DrillDownParameter1.IsDefault &&
                    DrillDownParameter2.IsDefault &&
                    DrillDownParameter3.IsDefault &&
                    DrillDownParameter4.IsDefault &&
                    DrillDownParameter5.IsDefault &&
                    DrillDownParameter6.IsDefault &&
                    DrillDownParameter7.IsDefault &&
                    DrillDownParameter8.IsDefault &&
                    DrillDownParameter9.IsDefault &&
                    DrillDownParameter10.IsDefault;
            }
        }

        [Browsable(false)]
        public virtual bool IsDefaultExt
        {
            get
            {
                return 
                    !(SortingEnabled && !string.IsNullOrEmpty(SortingColumn)) &&
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
        /// Gets or sets value which indicates whether it is allowed or not, using given component, data re-sorting in the report viewer.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates whether it is allowed or not, using given component, data re-sorting in the report viewer.")]
        [StiOrder(StiPropertyOrder.InteractionSortingEnabled)]
        public virtual bool SortingEnabled { get; set; } = true;

        /// <summary>
		/// Gets or sets a column by what data should be re-sorted in the report viewer.
		/// </summary>
		[StiSerializable]
        [DefaultValue("")]
        public virtual string SortingColumn { get; set; } = string.Empty;

        [Browsable(false)]
        public virtual int SortingIndex { get; set; }

        [Browsable(false)]
        public virtual StiInteractionSortDirection SortingDirection { get; set; } = StiInteractionSortDirection.None;
        #endregion

        #region Properties.DrillDown
        /// <summary>
        /// Gets or sets value which indicates whether the Drill-Down operation can be executed.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates whether the Drill-Down operation can be executed.")]
        [StiOrder(StiPropertyOrder.InteractionDrillDownEnabled)]
        public virtual bool DrillDownEnabled 
        { 
            get; 
            set; 
        }

        /// <summary>
		/// Gets or sets a path to a report for the Drill-Down operation.
		/// </summary>
		[StiSerializable]
        [DefaultValue("")]
        [Description("Gets or sets a path to a report for the Drill-Down operation.")]
        [StiOrder(StiPropertyOrder.InteractionDrillDownReport)]
        public virtual string DrillDownReport { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value which indicates opening of sub-reports in the one tab.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiDrillDownMode.MultiPage)]
        [Description("Gets or sets a value which indicates opening of sub-reports in the one tab.")]
        [StiOrder(StiPropertyOrder.InteractionDrillDownReport)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        public virtual StiDrillDownMode DrillDownMode { get; set; } = StiDrillDownMode.MultiPage;

        /// <summary>
		/// Gets or sets first Drill-Down parameter.
		/// </summary>
		[StiSerializable(StiSerializationVisibility.Class,
            StiSerializeTypes.SerializeToDesigner |
            StiSerializeTypes.SerializeToDocument |
            StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets first Drill-Down parameter.")]
        [StiOrder(StiPropertyOrder.InteractionDrillDownParameter1)]
        public virtual StiDrillDownParameter DrillDownParameter1 { get; set; }

        /// <summary>
		/// Gets or sets second Drill-Down parameter.
		/// </summary>
		[StiSerializable(StiSerializationVisibility.Class)]
        [Description("Gets or sets second Drill-Down parameter.")]
        [StiOrder(StiPropertyOrder.InteractionDrillDownParameter2)]
        public virtual StiDrillDownParameter DrillDownParameter2 { get; set; }

        /// <summary>
		/// Gets or sets third Drill-Down parameter.
		/// </summary>
		[StiSerializable(StiSerializationVisibility.Class)]
        [Description("Gets or sets third Drill-Down parameter.")]
        [StiOrder(StiPropertyOrder.InteractionDrillDownParameter3)]
        public virtual StiDrillDownParameter DrillDownParameter3 { get; set; }

        /// <summary>
        /// Gets or sets fourth Drill-Down parameter.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [Description("Gets or sets fourth Drill-Down parameter.")]
        [StiOrder(StiPropertyOrder.InteractionDrillDownParameter4)]
        public virtual StiDrillDownParameter DrillDownParameter4 { get; set; }

        /// <summary>
        /// Gets or sets fifth Drill-Down parameter.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [Description("Gets or sets fifth Drill-Down parameter.")]
        [StiOrder(StiPropertyOrder.InteractionDrillDownParameter5)]
        public virtual StiDrillDownParameter DrillDownParameter5 { get; set; }

        /// <summary>
        /// Gets or sets sixth Drill-Down parameter.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [Description("Gets or sets sixth Drill-Down parameter.")]
        [StiOrder(StiPropertyOrder.InteractionDrillDownParameter6)]
        public virtual StiDrillDownParameter DrillDownParameter6 { get; set; }

        /// <summary>
        /// Gets or sets seventh Drill-Down parameter.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [Description("Gets or sets seventh Drill-Down parameter.")]
        [StiOrder(StiPropertyOrder.InteractionDrillDownParameter7)]
        public virtual StiDrillDownParameter DrillDownParameter7 { get; set; }

        /// <summary>
        /// Gets or sets eighth Drill-Down parameter.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [Description("Gets or sets eighth Drill-Down parameter.")]
        [StiOrder(StiPropertyOrder.InteractionDrillDownParameter8)]
        public virtual StiDrillDownParameter DrillDownParameter8 { get; set; }

        /// <summary>
        /// Gets or sets nineth Drill-Down parameter.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [Description("Gets or sets nineth Drill-Down parameter.")]
        [StiOrder(StiPropertyOrder.InteractionDrillDownParameter9)]
        public virtual StiDrillDownParameter DrillDownParameter9 { get; set; }

        /// <summary>
        /// Gets or sets tenth Drill-Down parameter.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [Description("Gets or sets tenth Drill-Down parameter.")]
        [StiOrder(StiPropertyOrder.InteractionDrillDownParameter10)]
        public virtual StiDrillDownParameter DrillDownParameter10 { get; set; }

        /// <summary>
		/// Gets or sets a page for the Drill-Down operation.
		/// </summary>
		[StiNonSerialized]
        [Editor("Stimulsoft.Report.Components.Design.StiInteractionDrillDownPageEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [DefaultValue(null)]
        [Description("Gets or sets a page for the Drill-Down operation.")]
        [StiOrder(StiPropertyOrder.InteractionDrillDownPage)]
        public virtual StiPage DrillDownPage
        {
            get
            {
                if (ParentComponent == null || ParentComponent.Report == null) return null;
                foreach (StiPage page in ParentComponent.Report.Pages)
                {
                    if (page.Guid == this.DrillDownPageGuid)
                        return page;
                }

                return null;
            }
            set
            {
                if (value == null)
                {
                    this.DrillDownPageGuid = null;
                }
                else
                {
                    if (value.Guid == null)
                        value.Guid = StiGuidUtils.NewGuid();

                    this.DrillDownPageGuid = value.Guid;
                }
            }
        }

        [Browsable(false)]
        [StiSerializable]
        [DefaultValue(null)]
        public string DrillDownPageGuid { get; set; }
        #endregion

        #region Properties.Expression
        #region Bookmark
        /// <summary>
        /// Gets or sets the expression to fill a component bookmark.
        /// </summary>
        [StiOrder(StiPropertyOrder.InteractionBookmark)]
        [Description("Gets or sets the expression to fill a component bookmark.")]
        public virtual StiBookmarkExpression Bookmark
        {
            get
            {
                return ParentComponent.Bookmark;
            }
            set
            {
                ParentComponent.Bookmark = value;
            }
        }
        #endregion

        #region Hyperlink
        /// <summary>
        /// Gets or sets an expression to fill a component hyperlink.
        /// </summary>
        [StiOrder(StiPropertyOrder.InteractionHyperlink)]
        [Description("Gets or sets an expression to fill a component hyperlink.")]
        public virtual StiHyperlinkExpression Hyperlink
        {
            get
            {
                return ParentComponent.Hyperlink;
            }
            set
            {
                ParentComponent.Hyperlink = value;
            }
        }
        #endregion

        #region Tag
        /// <summary>
        /// Gets or sets the expression to fill a component tag.
        /// </summary>
        [Description("Gets or sets the expression to fill a component tag.")]
        [StiOrder(StiPropertyOrder.InteractionTag)]
        public virtual StiTagExpression Tag
        {
            get
            {
                return ParentComponent.Tag;
            }
            set
            {
                ParentComponent.Tag = value;
            }
        }
        #endregion

        #region ToolTip
        /// <summary>
        /// Gets or sets the expression to fill a component tooltip.
        /// </summary>
        [StiOrder(StiPropertyOrder.InteractionToolTip)]
        [Description("Gets or sets the expression to fill a component tooltip.")]
        public virtual StiToolTipExpression ToolTip
        {
            get
            {
                return ParentComponent.ToolTip;
            }
            set
            {
                ParentComponent.ToolTip = value;
            }
        }
        #endregion
        #endregion

        #region Methods
        internal void ResetParametersToDefaultState()
        {
            this.DrillDownParameter1 = new StiDrillDownParameter();
            this.DrillDownParameter2 = new StiDrillDownParameter();
            this.DrillDownParameter3 = new StiDrillDownParameter();
            this.DrillDownParameter4 = new StiDrillDownParameter();
            this.DrillDownParameter5 = new StiDrillDownParameter();
            this.DrillDownParameter6 = new StiDrillDownParameter();
            this.DrillDownParameter7 = new StiDrillDownParameter();
            this.DrillDownParameter8 = new StiDrillDownParameter();
            this.DrillDownParameter9 = new StiDrillDownParameter();
            this.DrillDownParameter10 = new StiDrillDownParameter();
        }

        public string GetSortDataBandName()
        {
            if (string.IsNullOrEmpty(SortingColumn) || !SortingEnabled)
                return string.Empty;

            var indexOfDot = SortingColumn.IndexOfInvariant(".");
            return indexOfDot != -1 ? SortingColumn.Substring(0, indexOfDot) : string.Empty;
        }

        public string[] GetSortColumns()
        {
            var str = GetSortColumnsString();

            return str.Length == 0 ? null : str.Split('.');
        }

        public string GetSortColumnsString()
        {
            if (string.IsNullOrEmpty(SortingColumn) || !SortingEnabled)
                return string.Empty;

            var indexOfDot = this.SortingColumn.IndexOfInvariant(".");
            return indexOfDot != -1 ? this.SortingColumn.Substring(indexOfDot + 1) : string.Empty;
        }
        #endregion

        #region Properties
        public StiComponent ParentComponent { get; set; }
        #endregion

        public StiInteraction()
        {
            ResetParametersToDefaultState();
        }        
    }
}
