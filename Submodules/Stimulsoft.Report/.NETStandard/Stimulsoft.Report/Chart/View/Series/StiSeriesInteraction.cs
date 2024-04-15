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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.PropertyGrid;
using System.ComponentModel;
using System.Drawing.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(Stimulsoft.Report.Components.Design.StiSeriesInteractionConverter))]
    [RefreshProperties(RefreshProperties.All)]
    public class StiSeriesInteraction :
        IStiInteractionClass,
        IStiReportProperty,
        IStiDefault,
        IStiSeriesInteraction,
        IStiPropertyGridObject
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyStringNullOrEmpty("DrillDownPageGuid", DrillDownPageGuid);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "DrillDownPageGuid":
                        this.DrillDownPageGuid = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public StiComponentId ComponentId => StiComponentId.StiSeriesInteraction;

        [Browsable(false)]
        public string PropName => string.Empty;

        public StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var objHelper = new StiPropertyCollection();

            objHelper.Add(StiPropertyCategories.Main, new[]
            {
                propertyGrid.PropertiesHelper.SeriesInteraction()
            });

            return objHelper;
        }

        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return null;
        }
        #endregion

        #region IStiReportProperty
        public object GetReport()
        {
            return (ParentSeries?.Chart as StiChart)?.Report;
        }
        #endregion

        #region ICloneable override
        public object Clone()
        {
            return (StiSeriesInteraction)base.MemberwiseClone();
        }
        #endregion

        #region IStiDefault
        [Browsable(false)]
        public virtual bool IsDefault
        {
            get
            {
                return
                    string.IsNullOrEmpty(HyperlinkDataColumn) &&
                    string.IsNullOrEmpty(TagDataColumn) &&
                    string.IsNullOrEmpty(ToolTipDataColumn) &&
                    string.IsNullOrEmpty(Hyperlink.Value) &&
                    string.IsNullOrEmpty(Tag.Value) &&
                    string.IsNullOrEmpty(ToolTip.Value) &&
                    string.IsNullOrEmpty(ListOfHyperlinks.Value) &&
                    string.IsNullOrEmpty(ListOfTags.Value) &&
                    string.IsNullOrEmpty(ListOfToolTips.Value);
            }
        }
        #endregion

        #region Properties
        #region Hyperlink
        /// <summary>
        /// Gets or sets an expression to fill a series hyperlink.
        /// </summary>
        [StiOrder(StiSeriesPropertyOrder.Hyperlink)]
        [StiCategory("Hyperlink")]
        [Description("Gets or sets an expression to fill a series hyperlink.")]
        public virtual StiHyperlinkExpression Hyperlink
        {
            get
            {
                return ParentSeries.Hyperlink;
            }
            set
            {
                ParentSeries.Hyperlink = value;
            }
        }
        #endregion

        #region Tag
        /// <summary>
        /// Gets or sets the expression to fill a series tag.
        /// </summary>
        [Description("Gets or sets the expression to fill a series tag.")]
        [StiCategory("Tag")]
        [StiOrder(StiSeriesPropertyOrder.Tag)]
        public virtual StiTagExpression Tag
        {
            get
            {
                return ParentSeries.Tag;
            }
            set
            {
                ParentSeries.Tag = value;
            }
        }
        #endregion

        #region ToolTip
        /// <summary>
        /// Gets or sets the expression to fill a series tooltip.
        /// </summary>
        [StiOrder(StiSeriesPropertyOrder.ToolTip)]
        [StiCategory("ToolTip")]
        [Description("Gets or sets the expression to fill a series tooltip.")]
        public virtual StiToolTipExpression ToolTip
        {
            get
            {
                return ParentSeries.ToolTip;
            }
            set
            {
                ParentSeries.ToolTip = value;
            }
        }
        #endregion

        #region HyperlinkDataColumn
        /// <summary>
        /// Gets or sets a name of the column that contains the hyperlink value.
        /// </summary>
        [StiOrder(StiSeriesPropertyOrder.HyperlinkDataColumn)]
        [Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [DefaultValue("")]
        [StiCategory("Hyperlink")]
        [Description("Gets or sets a name of the column that contains the hyperlink value.")]
        public string HyperlinkDataColumn
        {
            get
            {
                return ParentSeries.HyperlinkDataColumn;
            }
            set
            {
                ParentSeries.HyperlinkDataColumn = value;
            }
        }
        #endregion

        #region TagDataColumn
        /// <summary>
        /// Gets or sets a name of the column that contains the tag value.
        /// </summary>
        [StiOrder(StiSeriesPropertyOrder.TagDataColumn)]
        [Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [DefaultValue("")]
        [StiCategory("Tag")]
        [Description("Gets or sets a name of the column that contains the tag value.")]
        public string TagDataColumn
        {
            get
            {
                return ParentSeries.TagDataColumn;
            }
            set
            {
                ParentSeries.TagDataColumn = value;
            }
        }
        #endregion

        #region ToolTipDataColumn
        /// <summary>
        /// Gets or sets a name of the column that contains the tool tip value.
        /// </summary>
        [StiOrder(StiSeriesPropertyOrder.ToolTipDataColumn)]
        [Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [DefaultValue("")]
        [StiCategory("ToolTip")]
        [Description("Gets or sets a name of the column that contains the tool tip value.")]
        public string ToolTipDataColumn
        {
            get
            {
                return ParentSeries.ToolTipDataColumn;
            }
            set
            {
                ParentSeries.ToolTipDataColumn = value;
            }
        }
        #endregion

        #region ListOfHyperlinks
        /// <summary>
        /// Gets or sets the expression to fill a list of hyperlinks.  Example: 1;2;3
        /// </summary>
        [StiOrder(StiSeriesPropertyOrder.ListOfHyperlinks)]
        [StiCategory("Hyperlink")]
        [Description("Gets or sets the expression to fill a list of hyperlinks. Example: 1;2;3")]
        public virtual StiListOfHyperlinksExpression ListOfHyperlinks
        {
            get
            {
                return ParentSeries.ListOfHyperlinks;
            }
            set
            {
                ParentSeries.ListOfHyperlinks = value;
            }
        }
        #endregion                

        #region ListOfTags
        /// <summary>
        /// Gets or sets the expression to fill a list of tags.  Example: 1;2;3
        /// </summary>
        [StiOrder(StiSeriesPropertyOrder.ListOfTags)]
        [StiCategory("Tag")]
        [Description("Gets or sets the expression to fill a list of tags. Example: 1;2;3")]
        public virtual StiListOfTagsExpression ListOfTags
        {
            get
            {
                return ParentSeries.ListOfTags;
            }
            set
            {
                ParentSeries.ListOfTags = value;
            }
        }
        #endregion                

        #region ListOfToolTips
        /// <summary>
        /// Gets or sets the expression to fill a list of tool tips.  Example: 1;2;3
        /// </summary>
        [StiOrder(StiSeriesPropertyOrder.ListOfToolTips)]
        [StiCategory("ToolTip")]
        [Description("Gets or sets the expression to fill a list of tool tips. Example: 1;2;3")]
        public virtual StiListOfToolTipsExpression ListOfToolTips
        {
            get
            {
                return ParentSeries.ListOfToolTips;
            }
            set
            {
                ParentSeries.ListOfToolTips = value;
            }
        }
        #endregion

        /// <summary>
        /// Gets or sets value which indicates that the Drill-Down operation can be executed for Series.
        /// </summary>
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("Behavior")]
        [Description("Gets or sets a value which indicates that the Drill-Down operation can be executed for Series.")]
        [StiOrder(StiSeriesPropertyOrder.AllowSeries)]
        public virtual bool AllowSeries
        {
            get
            {
                return ParentSeries.AllowSeries;
            }
            set
            {
                ParentSeries.AllowSeries = value;
            }
        }

        /// <summary>
        /// Gets or sets value which indicates that the Drill-Down operation can be executed for Series Elements.
        /// </summary>
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("Behavior")]
        [Description("Gets or sets a value which indicates that the Drill-Down operation can be executed for Series Elements.")]
        [StiOrder(StiSeriesPropertyOrder.AllowSeriesElements)]
        public virtual bool AllowSeriesElements
        {
            get
            {
                return ParentSeries.AllowSeriesElements;
            }
            set
            {
                ParentSeries.AllowSeriesElements = value;
            }
        }

        /// <summary>
        /// Gets or sets value which indicates whether the Drill-Down operation can be executed.
        /// </summary>
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("Behavior")]
        [Description("Gets or sets a value which indicates whether the Drill-Down operation can be executed.")]
        [StiOrder(StiSeriesPropertyOrder.DrillDownEnabled)]
        public virtual bool DrillDownEnabled
        {
            get
            {
                return ParentSeries.DrillDownEnabled;
            }
            set
            {
                ParentSeries.DrillDownEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a path to a report for the Drill-Down operation.
        /// </summary>
        [DefaultValue("")]
        [StiCategory("Behavior")]
        [Description("Gets or sets a path to a report for the Drill-Down operation.")]
        [StiOrder(StiSeriesPropertyOrder.DrillDownReport)]
        public virtual string DrillDownReport
        {
            get
            {
                return ParentSeries.DrillDownReport;
            }
            set
            {
                ParentSeries.DrillDownReport = value;
            }
        }

        /// <summary>
        /// Gets or sets a page for the Drill-Down operation.
        /// </summary>
        [Editor("Stimulsoft.Report.Components.Design.StiInteractionDrillDownPageEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [DefaultValue(null)]
        [StiCategory("Behavior")]
        [Description("Gets or sets a page for the Drill-Down operation.")]
        [StiOrder(StiSeriesPropertyOrder.DrillDownPage)]
        public virtual StiPage DrillDownPage
        {
            get
            {
                return ParentSeries.DrillDownPage;
            }
            set
            {
                ParentSeries.DrillDownPage = value;
            }
        }

        [Browsable(false)]
        [StiSerializable]
        [DefaultValue(null)]
        public string DrillDownPageGuid
        {
            get
            {
                return ParentSeries.DrillDownPageGuid;
            }
            set
            {
                ParentSeries.DrillDownPageGuid = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public StiComponent ParentComponent => ParentSeries?.Chart as StiChart;

        [Browsable(false)]
        public StiSeries ParentSeries { get; set; }
        #endregion
    }
}
