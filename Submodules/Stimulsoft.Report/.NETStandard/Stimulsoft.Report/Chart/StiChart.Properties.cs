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
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Chart.Design;
using Stimulsoft.Report.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    public partial class StiChart
    {
        [Browsable(false)]
        public StiChartCoreXF Core { get; set; }

        /// <summary>
        /// Gets or sets value which indicates how to rotate an chart before output.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(StiImageRotation.None)]
        [StiSerializable]
        [StiCategory("ChartAdditional")]
        [StiOrder(StiPropertyOrder.ChartRotation)]
        [Description("Gets or sets value which indicates how to rotate an chart before output.")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiImageRotation Rotation { get; set; } = StiImageRotation.None;

        /// <summary>
        /// Gets or sets a type of the chart editor.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(StiChartEditorType.Advanced)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        public StiChartEditorType EditorType { get; set; } = StiChartEditorType.Advanced;

        private StiSeriesCollection series;
        /// <summary>
        /// Gets list of series.
        /// </summary>
        [Description("Gets list of series.")]
        [StiSerializable(StiSerializationVisibility.List)]
        [StiCategory("Chart")]
        [StiOrder(StiPropertyOrder.ChartSeries)]
        [Editor("Stimulsoft.Report.Chart.Design.StiChartSeriesEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(StiSeriesConverter))]
        public StiSeriesCollection Series
        {
            get
            {
                return series;
            }
            set
            {
                series = value;
                series.Chart = this;
            }
        }

        private bool ShouldSerializeSeries()
        {
            return Series == null || Series.Count > 0;
        }

        private IStiArea area;
        /// <summary>
        /// Gets area of the chart.
        /// </summary>
		[StiSerializable(StiSerializationVisibility.Class)]
        [StiCategory("Chart")]
        [StiOrder(StiPropertyOrder.ChartArea)]
        [Description("Gets area of the chart.")]
        [TypeConverter(typeof(StiUniversalConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        public IStiArea Area
        {
            get
            {
                return area;
            }
            set
            {
                if (area != value)
                {
                    if (value is StiRadarArea)
                        area = value;

                    area = value;

                    if (value != null)
                        area.Chart = this;
                    else
                        area = null;
                }
            }
        }

        private IStiChartTable table;
        /// <summary>
        /// Gets table of the chart.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiCategory("Chart")]
        [StiOrder(StiPropertyOrder.ChartTable)]
        [Description("Gets table of the chart.")]
        [TypeConverter(typeof(StiUniversalConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        public IStiChartTable Table
        {
            get
            {
                return table;
            }
            set
            {
                if (table != value)
                {
                    table = value;
                    table.Chart = this;
                }
            }
        }

        private bool ShouldSerializeTable()
        {
            return Table == null || !Table.IsDefault;
        }

        private IStiChartStyle style = new StiStyle29();
        /// <summary>
        /// Gets or sets style of the chart.
        /// </summary>
		[StiSerializable(StiSerializationVisibility.Class)]
        [StiCategory("Chart")]
        [StiOrder(StiPropertyOrder.ChartStyle)]
        [TypeConverter(typeof(StiChartStyleConverter))]
        [Browsable(false)]
        [Description("Gets or sets style of the chart.")]
        public IStiChartStyle Style
        {
            get
            {
                return style;
            }
            set
            {
                if (style != value)
                {
                    style = value;
                    if (value != null)
                        value.Core.Chart = this;
                }
            }
        }

        private bool allowApplyStyle = true;
        /// <summary>
        /// Gets or sets value which indicates that chart style will be used.
        /// </summary>
        [StiSerializable]
        [StiCategory("ChartAdditional")]
        [StiOrder(StiPropertyOrder.ChartAllowApplyStyle)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that chart style will be used.")]
        [DefaultValue(true)]
        [StiPropertyLevel(StiLevel.Standard)]
        public bool AllowApplyStyle
        {
            get
            {
                return allowApplyStyle;
            }
            set
            {
                if (allowApplyStyle != value)
                {
                    allowApplyStyle = value;
                    if (value)
                        this.Core.ApplyStyle(this.Style);
                }
            }
        }

        [StiSerializable]
        [Browsable(false)]
        public string CustomStyleName { get; set; } = "";

        private int horSpacing = 10;
        /// <summary>
        /// Gets os sets horizontal space between border of the chart and the chart.
        /// </summary>
		[StiSerializable]
        [StiCategory("ChartAdditional")]
        [StiOrder(StiPropertyOrder.ChartHorSpacing)]
        [DefaultValue(10)]
        [Description("Gets os sets horizontal space between border of the chart and the chart.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public int HorSpacing
        {
            get
            {
                return horSpacing;
            }
            set
            {
                if (value >= 0)
                    horSpacing = value;
            }
        }

        private int vertSpacing = 10;
        /// <summary>
        /// Gets os sets vertical space between border of the chart and the chart.
        /// </summary>
		[StiSerializable]
        [StiCategory("ChartAdditional")]
        [StiOrder(StiPropertyOrder.ChartVertSpacing)]
        [DefaultValue(10)]
        [Description("Gets os sets vertical space between border of the chart and the chart.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public int VertSpacing
        {
            get
            {
                return vertSpacing;
            }
            set
            {
                vertSpacing = value;
            }
        }

        private IStiSeriesLabels seriesLabels;
        /// <summary>
        /// Gets or sets series labels settings.
        /// </summary>
		[StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiSeriesLabelsConverter))]
        [Browsable(false)]
        public IStiSeriesLabels SeriesLabels
        {
            get
            {
                return seriesLabels;
            }
            set
            {
                seriesLabels = value;
                if (value != null)
                    seriesLabels.Chart = this;
            }
        }

        /// <summary>
        /// Gets or sets series labels settings.
        /// </summary>
        [StiCategory("Chart")]
        [StiNonSerialized]
        [StiOrder(StiPropertyOrder.ChartLabels)]
        [Description("Gets or sets series labels.")]
        [TypeConverter(typeof(StiSeriesLabelsConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        [Editor("Stimulsoft.Report.Chart.Design.StiChartLabelsEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public IStiSeriesLabels Labels
        {
            get
            {
                return SeriesLabels;
            }
            set
            {
                SeriesLabels = value;
            }
        }

        private IStiLegend legend;
        /// <summary>
        /// Gets or sets legend settings.
        /// </summary>
		[StiSerializable(StiSerializationVisibility.Class)]
        [StiCategory("Chart")]
        [StiOrder(StiPropertyOrder.ChartLegend)]
        [Description("Gets or sets legend settings.")]
        [TypeConverter(typeof(StiUniversalConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        public IStiLegend Legend
        {
            get
            {
                return legend;
            }
            set
            {
                legend = value;
                legend.Chart = this;
            }
        }

        private bool ShouldSerializeLegend()
        {
            return Legend == null || !Legend.IsDefault;
        }

        private IStiChartTitle title;
        /// <summary>
        /// Gets or sets chart title settings.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiCategory("Chart")]
        [StiOrder(StiPropertyOrder.ChartTitle)]
        [Description("Gets or sets chart title settings.")]
        [TypeConverter(typeof(StiUniversalConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        public IStiChartTitle Title
        {
            get
            {
                return title;
            }
            set
            {
                if (title != value)
                {
                    title = value;
                    title.Chart = this;
                }
            }
        }

        private bool ShouldSerializeTitle()
        {
            return Title == null || !Title.IsDefault;
        }

        private StiStripsCollection strips;
        /// <summary>
        /// Gets os sets strips settings of the chart.
        /// </summary>
        [StiCategory("Chart")]
        [StiSerializable(StiSerializationVisibility.List)]
        [StiOrder(StiPropertyOrder.ChartStrips)]
        [Description("Gets os sets strips settings of the chart.")]
        [Editor("Stimulsoft.Report.Chart.Design.StiChartStripsEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(StiStripsConverter))]
        public virtual StiStripsCollection Strips
        {
            get
            {
                return strips;
            }
            set
            {
                strips = value;
                strips.Chart = this;
            }
        }

        private bool ShouldSerializeStrips()
        {
            return Strips == null || Strips.Count > 0;
        }

        private StiConstantLinesCollection constantLines;
        /// <summary>
        /// Gets os sets constant lines settings of the chart.
        /// </summary>
        [StiCategory("Chart")]
        [StiSerializable(StiSerializationVisibility.List)]
        [StiOrder(StiPropertyOrder.ChartConstantLines)]
        [Description("Gets os sets constant lines settings of the chart.")]
        [Editor("Stimulsoft.Report.Chart.Design.StiChartConstantLinesEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(StiConstantLinesConverter))]
        public virtual StiConstantLinesCollection ConstantLines
        {
            get
            {
                return constantLines;
            }
            set
            {
                constantLines = value;
                constantLines.Chart = this;
            }
        }

        private bool ShouldSerializeConstantLines()
        {
            return ConstantLines == null || ConstantLines.Count > 0;
        }

        /// <summary>
        /// Gets or sets a value which indicates that the chart will be animated.
        /// </summary>
        [StiCategory("Chart")]
        [StiSerializable(StiSerializationVisibility.None)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Description("Gets or sets a value which indicates that the chart will be animated.")]
        [DefaultValue(false)]
        [Browsable(false)]
        public bool IsAnimation { get; set; } = false;

        private bool isAnimationChangingValues = true;
        /// <summary>
        /// Gets or sets a value which indicates that the chart will be animated changing values.
        /// </summary>
        [StiSerializable]
        [StiSerializable(StiSerializationVisibility.None)]
        [StiCategory("Chart")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Description("Gets or sets a value which indicates that the chart will be animated changing values.")]
        [DefaultValue(true)]
        [Browsable(false)]
        public bool IsAnimationChangingValues
        {
            get
            {
                return isAnimationChangingValues;
            }
            set
            {
                if (isAnimationChangingValues != value)
                {
                    isAnimationChangingValues = value;
                    IsAnimation = value;
                }
            }
        }

        [Browsable(false)]
        public List<StiAnimation> PreviousAnimations { get; set; }
    }
}