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
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using System.Drawing.Design;
using System.Drawing;
using Stimulsoft.Report.PropertyGrid;
using System.Collections.Generic;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiStockSeries :
        StiCandlestickSeries,
        IStiStockSeries
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("BorderColor");
            jObject.RemoveProperty("BorderWidth");
            jObject.RemoveProperty("Brush");
            jObject.RemoveProperty("BrushNegative");

            jObject.AddPropertyColor("LineColor", LineColor, Color.Black);
            jObject.AddPropertyEnum("LineStyle", LineStyle, StiPenStyle.Solid);
            jObject.AddPropertyFloat("LineWidth", LineWidth, 2f);
            jObject.AddPropertyColor("LineColorNegative", LineColorNegative, Color.Firebrick);
            jObject.AddPropertyBool("AllowApplyColorNegative", AllowApplyColorNegative);

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

                    case "LineWidth":
                        this.LineWidth = property.DeserializeFloat();
                        break;

                    case "LineColorNegative":
                        this.LineColorNegative = property.DeserializeColor();
                        break;

                    case "AllowApplyColorNegative":
                        this.AllowApplyColorNegative = property.DeserializeBool();
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
                return StiComponentId.StiStockSeries;
            }
        }

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // Value Open
            var list = new[] 
            {
                propHelper.ValueDataColumnOpen(),
                propHelper.ValueOpen(),
                propHelper.ListOfValuesOpen()
            };
            objHelper.Add(StiPropertyCategories.ValueOpen, list);

            // Value Close
            list = new[] 
            {
                propHelper.ValueDataColumnClose(),
                propHelper.ValueClose(),
                propHelper.ListOfValuesClose()
            };
            objHelper.Add(StiPropertyCategories.ValueClose, list);

            //Value High
            list = new[] 
            {
                propHelper.ValueDataColumnHigh(),
                propHelper.ValueHigh(),
                propHelper.ListOfValuesHigh()
            };
            objHelper.Add(StiPropertyCategories.ValueHigh, list);

            //Value Low
            list = new[] 
            {
                propHelper.ValueDataColumnLow(),
                propHelper.ValueLow(),
                propHelper.ListOfValuesLow()
            };
            objHelper.Add(StiPropertyCategories.ValueLow, list);

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
                propHelper.LineColor(),
                propHelper.LineColorNegative(),
                propHelper.LineStyle(),
                propHelper.LineWidth(),
                propHelper.ShowShadow()
            };
            objHelper.Add(StiPropertyCategories.Appearance, list);

            // Behavior
            list = new[] 
            {
                propHelper.AllowApplyStyle(),
                propHelper.AllowApplyColorNegative(),
                propHelper.ShowInLegend(),
                propHelper.ShowSeriesLabels(),
                propHelper.Title(),
                propHelper.YAxis()
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
            IStiStockSeries series = base.Clone() as IStiStockSeries;

            return series;
        }
        #endregion

        #region Methods.Types
        public override Type GetDefaultAreaType()
        {
            return typeof(StiStockArea);
        }
        #endregion

        #region Properties
        [Browsable(false)]
        [StiNonSerialized]
        public override Color BorderColor
        {
            get
            {
                return base.BorderColor;
            }
            set
            {
                base.BorderColor = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public override float BorderWidth
        {
            get
            {
                return base.BorderWidth;
            }
            set
            {
                if (value > 0)
                {
                    base.BorderWidth = value;
                }
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public override StiBrush Brush
        {
            get
            {
                return base.Brush;
            }
            set
            {
                base.Brush = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public override StiBrush BrushNegative
        {
            get
            {
                return base.BrushNegative;
            }
            set
            {
                base.BrushNegative = value;
            }
        }

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

        private Color lineColorNegative = Color.Firebrick;
        [StiSerializable]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Appearance")]
        [Description("Gets or sets a line color of series for negative values.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual Color LineColorNegative
        {
            get
            {
                return lineColorNegative;
            }
            set
            {
                lineColorNegative = value;
            }
        }

        private bool allowApplyColorNegative = false;
        [StiSerializable]
        [StiCategory("Common")]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates that the specific color for negative values will be used.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool AllowApplyColorNegative
        {
            get
            {
                return allowApplyColorNegative;
            }
            set
            {
                allowApplyColorNegative = value;
            }
        }
        #endregion

        #region Methods.override
        public override StiSeries CreateNew()
        {
            return new StiStockSeries();
        }
        #endregion

        public StiStockSeries()
        {
            this.Core = new StiStockSeriesCoreXF(this);
        }
    }
}