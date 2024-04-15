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
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Design;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiSplineAreaSeries : 
        StiSplineSeries,
        IStiSplineAreaSeries
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyBool("TopmostLine", topmostLine, true);
            jObject.AddPropertyBrush("Brush", brush);
            jObject.AddPropertyBrush("BrushNegative", brushNegative);
            jObject.AddPropertyBool("AllowApplyBrushNegative", allowApplyBrushNegative);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "TopmostLine":
                        this.topmostLine = property.DeserializeBool();
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
                return StiComponentId.StiSplineAreaSeries;
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
                propHelper.LabelsOffset(),
                propHelper.Lighting(),
                propHelper.LineColor(),
                propHelper.LineColorNegative(),
                propHelper.LineStyle(),
                propHelper.LineWidth(),
                propHelper.BrushNegative(),
                propHelper.ShowShadow(),
                propHelper.TopmostLine()
            };
            objHelper.Add(StiPropertyCategories.Appearance, list);

            // Behavior
            list = new[] 
            {
                propHelper.AllowApplyStyle(),
                propHelper.AllowApplyBrushNegative(),
                propHelper.AllowApplyColorNegative(),
                propHelper.ShowInLegend(),
                propHelper.ShowNulls(),
                propHelper.ShowSeriesLabels(),
                propHelper.Tension(),
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
			IStiSplineAreaSeries series =	base.Clone() as IStiSplineAreaSeries;
			series.Brush =	this.Brush.Clone() as StiBrush;
		
			return series;
		}
		#endregion

        #region Methods.Types
        public override Type GetDefaultAreaType()
        {
            return typeof(StiClusteredColumnArea);
        }
        #endregion

        #region Properties
        private bool topmostLine = true;
        /// <summary>
        /// Gets or sets a value that indicates whether the line be displayed as a topmost.
        /// </summary>
        [StiSerializable]
        [StiCategory("Common")]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value that indicates whether the line be displayed as a topmost.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public bool TopmostLine
        {
            get
            {
                return topmostLine;
            }
            set
            {
                topmostLine = value;
            }
        }

		private StiBrush brush = new StiSolidBrush(Color.Gainsboro);
        /// <summary>
        /// Gets or sets brush which will used to fill area.
        /// </summary>
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
        /// <summary>
        /// Gets or sets a brush which will be used to fill negative values.
        /// </summary>
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
        /// <summary>
        /// Gets or sets a value which indicates that the specific brush for filling negative values will be used.
        /// </summary>
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
        #endregion  
      
        #region Methods.override
        public override StiSeries CreateNew()
        {
            return new StiSplineAreaSeries();
        }
        #endregion

        public StiSplineAreaSeries()
        {
            this.Core = new StiSplineAreaSeriesCoreXF(this);
        }
	}
}