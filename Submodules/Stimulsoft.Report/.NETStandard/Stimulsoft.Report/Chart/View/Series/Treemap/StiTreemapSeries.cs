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
using Stimulsoft.Base;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Drawing;
using System.ComponentModel;
using Stimulsoft.Base.Localization;
using System.Drawing;
using System.Drawing.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Design;
using Stimulsoft.Report.Helpers;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiTreemapSeries :
        StiSeries,
        IStiTreemapSeries
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);
            jObject.RemoveProperty("SortBy");

            jObject.AddPropertyEnum("SortBy", SortBy, StiSeriesSortType.Value);

            jObject.AddPropertyColor("BorderColor", borderColor, Color.Gray);
            jObject.AddPropertyInt("BorderThickness", BorderThickness, 1);
            jObject.AddPropertyBrush("Brush", brush);
            jObject.AddPropertyCornerRadius(nameof(CornerRadius), CornerRadius);

            if (this.Icon != null)
                jObject.AddPropertyEnum("Icon", this.Icon);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "BorderColor":
                        this.borderColor = property.DeserializeColor();
                        break;

                    case "BorderThickness":
                        this.BorderThickness = property.DeserializeInt();
                        break;

                    case "Brush":
                        this.brush = property.DeserializeBrush();
                        break;

                    case "Icon":
                        this.Icon = property.DeserializeEnum<StiFontIcons>();
                        break;

                    case nameof(CornerRadius):
                        CornerRadius = property.DeserializeCornerRadius();
                        break;
                }
            }
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            var series = base.Clone() as IStiTreemapSeries;
            series.Brush = this.Brush.Clone() as StiBrush;

            return series;
        }
        #endregion

        #region Methods.Types
        public override Type GetDefaultAreaType()
        {
            return typeof(StiTreemapArea);
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiTreemapSeries;
            }
        }

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // Value
            var list = new[]
            {
                propHelper.ValueDataColumn(),
                propHelper.Value(),
                propHelper.ListOfValues()
            };
            objHelper.Add(StiPropertyCategories.Value, list);

            // Argument
            list = new[]
            {
                propHelper.ArgumentDataColumn(),
                propHelper.Argument(),
                propHelper.ListOfArguments()
            };
            objHelper.Add(StiPropertyCategories.Argument, list);

            // Data
            list = new[]
            {
                propHelper.Format(),
                propHelper.SortBy(),
                propHelper.SortDirection(),
                propHelper.AutoSeriesKeyDataColumn(),
                propHelper.AutoSeriesColorDataColumn(),
                propHelper.AutoSeriesTitleDataColumn()
            };
            objHelper.Add(StiPropertyCategories.Data, list);

            // Appearance
            list = new[]
            {
                propHelper.BorderColor(),
                propHelper.Brush(),
                propHelper.ShowShadow()
            };
            objHelper.Add(StiPropertyCategories.Appearance, list);

            // Behavior
            list = new[]
            {
                propHelper.AllowApplyStyle(),
                propHelper.ShowInLegend(),
                propHelper.ShowSeriesLabels(),
                propHelper.Title(),
                propHelper.fWidth()
            };
            objHelper.Add(StiPropertyCategories.Behavior, list);

            return objHelper;
        }
        #endregion

        #region Methods.override
        public override StiSeries CreateNew()
        {
            return new StiTreemapSeries();
        }
        #endregion

        #region Properties
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

        [StiNonSerialized]
        [Browsable(false)]
        public override StiSeriesYAxis YAxis
        {
            get
            {
                return base.YAxis;
            }
            set
            {
                base.YAxis = value;
            }
        }

        private StiSeriesSortType sortBy = StiSeriesSortType.Value;
        /// <summary>
        /// Gets or sets mode of series values sorting.
        /// </summary>
        [DefaultValue(StiSeriesSortType.Value)]
        [StiSerializable]
        [TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Common")]
        [Description("Gets or sets mode of series values sorting.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public override StiSeriesSortType SortBy
        {
            get
            {
                return sortBy;
            }
            set
            {
                sortBy = value;
            }
        }


        private StiSeriesSortDirection sortDirection = StiSeriesSortDirection.Descending;
        /// <summary>
        /// Gets or sets sort direction.
        /// </summary>
        [DefaultValue(StiSeriesSortDirection.Descending)]
        [StiSerializable]
        [TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Common")]
        [Description("Gets or sets sort direction.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public override StiSeriesSortDirection SortDirection
        {
            get
            {
                return sortDirection;
            }
            set
            {
                sortDirection = value;
            }
        }

        private Color borderColor = Color.Gray;
        /// <summary>
        /// Gets or sets border color of series box.
        /// </summary>
		[StiSerializable]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Appearance")]
        [Description("Gets or sets border color of series box.")]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        public Color BorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                borderColor = value;
            }
        }

        /// <summary>
        /// Gets or sets border thickness of series element.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets border thickness of series element.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public int BorderThickness { get; set; } = 1;

        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Represents the value to which the corners are rounded.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiCornerRadius CornerRadius { get; set; } = new StiCornerRadius();

        private StiBrush brush = new StiSolidBrush(Color.Gainsboro);
        /// <summary>
        /// Gets or sets brush which will used to fill treemap box.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets brush which will used to fill treemap box.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiBrush Brush
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

        [DefaultValue(null)]
        [StiSerializable]
        [StiCategory("Common")]
        [Editor("Stimulsoft.Report.Design.StiFontIconEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(Stimulsoft.Report.Design.StiFontIconConverter))]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiFontIcons? Icon { get; set; }
        #endregion

        public StiTreemapSeries()
        {
            this.Core = new StiTreemapSeriesCoreXF(this);
        }
    }
}
