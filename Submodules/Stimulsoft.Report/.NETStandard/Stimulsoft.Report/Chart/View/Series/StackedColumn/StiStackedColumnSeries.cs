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
using System.Drawing;
using System.Drawing.Design;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Design;
using Stimulsoft.Report.Helpers;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiStackedColumnSeries : 
        StiSeries,
        IStiStackedColumnSeries
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("TrendLine");

            jObject.AddPropertyBool("ShowZeros", ShowZeros);
            jObject.AddPropertyFloat("Width", width, 0.8f);
            jObject.AddPropertyColor("BorderColor", borderColor, Color.Black);
            jObject.AddPropertyInt("BorderThickness", BorderThickness, 1);
            jObject.AddPropertyBrush("Brush", brush);
            jObject.AddPropertyBrush("BrushNegative", brushNegative);
            jObject.AddPropertyBool("AllowApplyBrushNegative", allowApplyBrushNegative);
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
                    case "ShowZeros":
                        this.ShowZeros = property.DeserializeBool();
                        break;

                    case "Width":
                        this.width = property.DeserializeFloat();
                        break;

                    case "BorderColor":
                        this.borderColor = property.DeserializeColor();
                        break;

                    case "BorderThickness":
                        this.BorderThickness = property.DeserializeInt();
                        break;

                    case "Brush":
                        this.brush = property.DeserializeBrush();
                        break;

                    case "BrushNegative":
                        this.brushNegative = property.DeserializeBrush();
                        break;

                    case "AllowApplyBrushNegative":
                        this.allowApplyBrushNegative = property.DeserializeBool();
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

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiStackedColumnSeries;
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
                propHelper.BrushNegative(),
                propHelper.ShowShadow()
            };
            objHelper.Add(StiPropertyCategories.Appearance, list);

            // Behavior
            list = new[] 
            {
                propHelper.AllowApplyStyle(),
                propHelper.AllowApplyBrushNegative(),
                propHelper.ShowInLegend(),
                propHelper.ShowSeriesLabels(),
                propHelper.ShowZeros(),
                propHelper.Title(),
                propHelper.YAxis(),
                propHelper.fWidth()
            };
            objHelper.Add(StiPropertyCategories.Behavior, list);

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
			IStiStackedColumnSeries series =	base.Clone() as IStiStackedColumnSeries;
			series.Brush =	this.Brush.Clone() as StiBrush;
			
			return series;
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

        private bool showZeros = false;
        /// <summary>
        /// Gets or sets value which indicates whether it is necessary to show the series element, if the series value of this column is 0.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [DefaultValue(false)]
        [Description("Gets or sets value which indicates whether it is necessary to show the series element, if the series value of this column is 0.")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool ShowZeros
        {
            get
            {
                return showZeros;
            }
            set
            {
                showZeros = value;
            }
        }

		private float width = 0.8f;
		[StiSerializable]
		[StiCategory("Common")]
		[DefaultValue(0.8f)]
        [StiPropertyLevel(StiLevel.Standard)]
        public float Width
		{
			get
			{
				return width;
			}
			set
			{
				if (value >= 0.01f && value <= 1f)width = value;
			}
		}

        private Color borderColor = Color.Black;
		[StiSerializable]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiCategory("Appearance")]
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
        [RefreshProperties(RefreshProperties.All)]
		[StiSerializable]
		[StiCategory("Appearance")]
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

        private StiBrush brushNegative = new StiSolidBrush(Color.Firebrick);
        [RefreshProperties(RefreshProperties.All)]
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets a brush which will be used to fill negative values.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiBrush BrushNegative
        {
            get
            {
                return brushNegative;
            }
            set
            {
                brushNegative = value;
            }
        }

        private bool allowApplyBrushNegative = false;
        [StiSerializable]
        [StiCategory("Appearance")]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates that the specific brush for filling negative values will be used.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public bool AllowApplyBrushNegative
        {
            get
            {
                return allowApplyBrushNegative;
            }
            set
            {
                allowApplyBrushNegative = value;
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

        #region Methods.Types
        public override Type GetDefaultAreaType()
        {
            return typeof(StiStackedColumnArea);
        }
        #endregion

        #region Methods.override
        public override StiSeries CreateNew()
        {
            return new StiStackedColumnSeries();
        }
        #endregion

        public StiStackedColumnSeries()
        {
            this.Core = new StiStackedColumnSeriesCoreXF(this);
        }
    }
}