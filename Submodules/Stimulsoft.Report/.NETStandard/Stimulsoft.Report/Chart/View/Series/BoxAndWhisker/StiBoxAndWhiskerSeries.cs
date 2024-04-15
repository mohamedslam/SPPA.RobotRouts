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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiBoxAndWhiskerSeries:
        StiSeries,
        IStiBoxAndWhiskerSeries
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("TopN");
            jObject.RemoveProperty("TrendLine");

            jObject.AddPropertyBool("ShowInnerPoints", this.ShowInnerPoints);
            jObject.AddPropertyBool("ShowMeanMarkers", this.ShowMeanMarkers, true);
            jObject.AddPropertyBrush("Brush", Brush);
            jObject.AddPropertyInt("BorderThickness", BorderThickness, 1);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {

                    case "ShowInnerPoints":
                        this.ShowInnerPoints = property.DeserializeBool();
                        break;

                    case "ShowMeanMarkers":
                        this.ShowMeanMarkers = property.DeserializeBool();
                        break;

                    case "Brush":
                        this.Brush = property.DeserializeBrush();
                        break;

                    case "BorderThickness":
                        this.BorderThickness = property.DeserializeInt();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiBoxAndWhiskerSeries;
            }
        }

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            return objHelper;
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            var series = base.Clone() as IStiBoxAndWhiskerSeries;

            return series;
        }
        #endregion

        #region Methods.Types
        public override Type GetDefaultAreaType()
        {
            return typeof(StiBoxAndWhiskerArea);
        }
        #endregion

        #region Properties
        [Browsable(false)]
        [StiNonSerialized]
        public override StiArgumentExpression Argument { get; set; } = new StiArgumentExpression();

        [Browsable(false)]
        [StiNonSerialized]
        public override string ArgumentDataColumn { get; set; } = string.Empty;

        [Browsable(false)]
        [StiNonSerialized]
        public override StiListOfArgumentsExpression ListOfArguments { get; set; } = new StiListOfArgumentsExpression();

        [StiNonSerialized]
        [Browsable(false)]
        public override StiChartConditionsCollection Conditions
        {
            get
            {
                return base.Conditions;
            }
            set
            {
                base.Conditions = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public override IStiSeriesTopN TopN
        {
            get
            {
                return base.TopN;
            }
            set
            {
                base.TopN = value;
            }
        }
        [StiNonSerialized]
        [Browsable(false)]
        public override bool TopNAllowed
        {
            get
            {
                return false;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public override StiTrendLinesCollection TrendLines
        {
            get
            {
                return base.TrendLines;
            }
            set
            {
                base.TrendLines = value;
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override bool TrendLineAllowed
        {
            get
            {
                return false;
            }
        }

        private StiBrush brush = new StiSolidBrush(Color.Gainsboro);
        /// <summary>
        /// Gets or sets brush which will used to fill box area.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets brush which will used to fill bar area.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiBrush Brush
        {
            get
            {
                return brush;
            }
            set
            {
                brush = value;
            }
        }

        private bool allowApplyBrush = true;
        /// <summary>
        /// Gets or sets value which allow to use brush from series settings.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which allow to use brush from series settings.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public bool AllowApplyBrush
        {
            get
            {
                return allowApplyBrush;
            }
            set
            {
                if (allowApplyBrush != value)
                {
                    allowApplyBrush = value;
                }
            }
        }

        /// <summary>
        /// Displays the data points that lie between the lower whisker line and the upper whisker line.
        /// </summary>
        [StiSerializable]
        [StiCategory("Common")]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Displays the data points that lie between the lower whisker line and the upper whisker line.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public bool ShowInnerPoints { get; set; }

        /// <summary>
        /// Displays the mean marker of the selected series.
        /// </summary>
        [StiSerializable]
        [StiCategory("Common")]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Displays the mean marker of the selected series.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public bool ShowMeanMarkers { get; set; } = true;

        /// <summary>
        /// Gets or sets border thickness of series element.
        /// </summary>
		[StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets border thickness of series element.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public int BorderThickness { get; set; } = 1;
        #endregion

        #region Methods.override
        public override StiSeries CreateNew()
        {
            return new StiBoxAndWhiskerSeries();
        }
        #endregion

        public StiBoxAndWhiskerSeries()
        {
            this.SeriesLabels = new StiNoneLabels();
            this.Core = new StiBoxAndWhiskerSeriesCoreXF(this);
        }
    }
}
