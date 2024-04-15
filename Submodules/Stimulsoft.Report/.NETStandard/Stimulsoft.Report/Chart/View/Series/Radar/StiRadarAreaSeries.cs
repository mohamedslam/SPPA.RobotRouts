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

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiRadarAreaSeries : 
        StiRadarSeries,
        IStiRadarAreaSeries
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyColor("LineColor", LineColor, Color.Black);
            jObject.AddPropertyEnum("LineStyle", LineStyle, StiPenStyle.Solid);
            jObject.AddPropertyBool("Lighting", Lighting, true);
            jObject.AddPropertyFloat("LineWidth", LineWidth, 2f);
            jObject.AddPropertyBrush("Brush", brush);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "LineColor":
                        this.LineColor = property.DeserializeColor();
                        break;

                    case "LineStyle":
                        this.LineStyle = property.DeserializeEnum<StiPenStyle>();
                        break;

                    case "Lighting":
                        this.Lighting = property.DeserializeBool();
                        break;

                    case "LineWidth":
                        this.LineWidth = property.DeserializeFloat();
                        break;

                    case "Brush":
                        this.brush = property.DeserializeBrush();
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
                return StiComponentId.StiRadarAreaSeries;
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
                propHelper.Brush(),
                propHelper.Lighting(),
                propHelper.LineColor(),
                propHelper.LineStyle(),
                propHelper.LineWidth(),
                propHelper.ShowShadow()
            };
            objHelper.Add(StiPropertyCategories.Appearance, list);

            // Behavior
            list = new[] 
            {
                propHelper.AllowApplyStyle(),
                propHelper.ShowInLegend(),
                propHelper.ShowNulls(),
                propHelper.ShowSeriesLabels(),
                propHelper.Title(),
                propHelper.YAxis()
            };
            objHelper.Add(StiPropertyCategories.Behavior, list);

            return objHelper;
        }
        #endregion

        #region Properties
        private Color lineColor = Color.Black;
        /// <summary>
        /// Gets or sets line color of series.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Appearance")]
        [Description("Gets or sets line color of series.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual Color LineColor
        {
            get
            {
                return lineColor;
            }
            set
            {
                lineColor = value;
            }
        }


        private StiPenStyle lineStyle = StiPenStyle.Solid;
        /// <summary>
        /// Gets or sets a line style of series.
        /// </summary>
        [Editor(StiEditors.PenStyle, typeof(UITypeEditor))]
        [DefaultValue(StiPenStyle.Solid)]
        [Description("Gets or sets a line style of series.")]
        [StiSerializable]
        [StiCategory("Common")]
        [TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiPenStyle LineStyle
        {
            get
            {
                return lineStyle;
            }
            set
            {
                lineStyle = value;
            }
        }


        private bool lighting = true;
        /// <summary>
        /// Gets or sets value which indicates that light effect will be shown.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("Appearance")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that light effect will be shown.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual bool Lighting
        {
            get
            {
                return lighting;
            }
            set
            {
                lighting = value;
            }
        }


        private float lineWidth = 2f;
        /// <summary>
        /// Gets or sets line width of series.
        /// </summary>
        [DefaultValue(2f)]
        [StiSerializable]
        [StiCategory("Common")]
        [Description("Gets or sets line width of series.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual float LineWidth
        {
            get
            {
                return lineWidth;
            }
            set
            {
                if (value > 0)
                {
                    lineWidth = value;
                }
            }
        }


        private StiBrush brush = new StiSolidBrush(Color.Gainsboro);
        /// <summary>
        /// Gets or sets brush which will be used to fill area.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets brush which will be used to fill area.")]
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
        #endregion

        #region Methods.Types
        public override Type GetDefaultAreaType()
        {
            return typeof(StiRadarAreaArea);
        }
        #endregion

        #region Methods.override
        public override StiSeries CreateNew()
        {
            return new StiRadarAreaSeries();
        }
        #endregion

        public StiRadarAreaSeries()
        {
            this.Core = new StiRadarAreaSeriesCoreXF(this);
        }
	}
}