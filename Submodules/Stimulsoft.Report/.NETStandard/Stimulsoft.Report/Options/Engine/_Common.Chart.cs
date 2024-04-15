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
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Chart;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// A class which controls of settings of the report engine. 
        /// </summary>
		public sealed partial class Engine
        {
            /// <summary>
            /// Gets or sets value which indicates that bug with loading old charts (without Direction property) will be fixed during loading of report.
            /// </summary>
            public static bool AllowFixOldChartTitle
            {
                get
                {
                    return StiSerializing.AllowFixOldChartTitle;
                }
                set
                {
                    StiSerializing.AllowFixOldChartTitle = value;
                }
            }

            /// <summary>
            /// This value indicates that it is necessary to simulate interactivity for charts with text elements.
            /// </summary>
            [DefaultValue(true)]
            [Description("This value indicates that it is necessary to simulate interactivity for charts with text elements.")]
            [StiSerializable]
            [Category("Chart")]
            public static bool AllowFixPieChartMarkerAlignment { get; set; }

            /// <summary>
            /// This value indicates that it is necessary to simulate interactivity for charts with text elements.
            /// </summary>
            [DefaultValue(false)]
            [Description("This value indicates that it is necessary to simulate interactivity for charts with text elements.")]
            [StiSerializable]
            [Category("Chart")]
            public static bool AllowInteractionInChartWithComponents { get; set; }

            /// <summary>
            /// The default value for the AllowApplyStyle property of the StiChart component.
            /// </summary>
            [DefaultValue(true)]
            [Description("The default value for the AllowApplyStyle property of the StiChart component.")]
            [StiSerializable]
            [Category("Chart")]
            public static bool DefaultValueOfAllowApplyStyleProperty { get; set; } = true;

            /// <summary>
            /// The value allows applying a chart style to self-series.
            /// </summary>
            [DefaultValue(true)]
            [Description("The value allows applying a chart style to self-series.")]
            [StiSerializable]
            [Category("Chart")]
            public static bool ApplyStylesInAutoSeries { get; set; } = true;

            /// <summary>
            /// Allow using the old mode of formatting percent in charts.
            /// </summary>
            [DefaultValue(false)]
            [Description("Allow using the old mode of formatting percent in charts.")]
            [StiSerializable]
            [Category("Chart")]
            public static bool OldChartPercentMode
            {
                get
                {
                    return StiChartOptions.OldChartPercentMode;
                }
                set
                {
                    StiChartOptions.OldChartPercentMode = value;
                }
            }

            /// <summary>
            /// Gets or sets a value which controls of output charts as bitmaps.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which controls of output charts as bitmaps.")]
            [StiSerializable]
            [Category("Chart")]
            public static bool RenderChartAsBitmap { get; set; }

            /// <summary>
            /// Gets or sets a scalling factor for rendering charts as bitmaps.
            /// </summary>
            [DefaultValue(2f)]
            [Description("Gets or sets a scalling factor for rendering charts as bitmaps.")]
            [StiSerializable]
            [Category("Chart")]
            public static float RenderChartAsBitmapZoom { get; set; } = 2f;

            /// <summary>
            /// A value indicates that the InvokeProcessChart event will be invoked for the template chart component.
            /// </summary>
            [DefaultValue(false)]
            [Description("A value indicates that the InvokeProcessChart event will be invoked for the template chart component.")]
            [StiSerializable]
            [Category("Chart")]
            public static bool AllowInvokeProcessChartEventForTemplateOfChart { get; set; }

            /// <summary>
            /// A value indicates that the chart values will be simplified
            /// </summary>
            [DefaultValue(false)]
            [Description("A value indicates that the chart values will be simplified.")]
            [StiSerializable]
            [Category("Chart")]
            public static bool AllowSimplifyChartValues { get; set; }

            /// <summary>
            /// If the property is set to true, then, before chart rendering, the report engine will not remember a position in the data source used for the chart.
            /// </summary>
            [DefaultValue(false)]
            [Description("If the property is set to true, then, before chart rendering, the report engine will not remember a position in the data source used for the chart.")]
            [StiSerializable]
            [Category("Chart")]
            public static bool DontSaveDataSourceBeforeChartRendering { get; set; }

            /// <summary>
            /// A value indicates that the text in chart can contain an html-tags
            /// </summary>
            [DefaultValue(false)]
            [Description("A value indicates that the text in chart can contain an html-tags.")]
            [StiSerializable]
            [Category("Chart")]
            public static bool AllowHtmlTagsInChart { get; set; }

        }
    }
}
