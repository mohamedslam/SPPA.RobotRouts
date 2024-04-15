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

using Stimulsoft.Base.Design;
using Stimulsoft.Base;
using System;
using System.ComponentModel;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.PropertyGrid;

namespace Stimulsoft.Report.Chart
{
    public class StiPie3dSeries :
        StiPieSeries,
        IStiPie3dSeries
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyJObject("Options3D", Options3D.SaveToJsonObject(mode));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Options3D":
                        this.Options3D.LoadFromJsonObject((JObject)property.Value);
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
            var series = base.Clone() as IStiPie3dSeries;

            return series;
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiPie3dSeries;

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
                //propHelper.StyleConditions(),
                propHelper.FilterMode(),
                propHelper.Filters(),
                propHelper.Format(),
                propHelper.ShowZeros(),
                propHelper.SortBy(),
                propHelper.SortDirection(),
                propHelper.ChartTopN(),
            };
            objHelper.Add(StiPropertyCategories.Data, list);

            // Appearance
            list = new[]
            {
                propHelper.AllowApplyBorderColor(),
                propHelper.AllowApplyBrush(),
                propHelper.AllowApplyStyle(),
                propHelper.ChartOptions3D(),
            };
            objHelper.Add(StiPropertyCategories.Appearance, list);

            // Common
            list = new[]
            {
                //propHelper.InteractionEditor(),
                //propHelper.SeriesLabels(),
                propHelper.ShowInLegend(),
                propHelper.ShowSeriesLabels(),
                propHelper.StartAngle(),
                propHelper.Title(),
                propHelper.YAxis(),
            };
            objHelper.Add(StiPropertyCategories.Common, list);

            // Series
            list = new[]
            {
                propHelper.AutoSeriesKeyDataColumn(),
                propHelper.AutoSeriesColorDataColumn(),
                propHelper.AutoSeriesTitleDataColumn()
            };
            objHelper.Add(StiPropertyCategories.Series, list);

            return objHelper;
        }
        #endregion

        #region Methods.Types
        public override Type GetDefaultAreaType()
        {
            return typeof(StiPie3dArea);
        }
        #endregion

        #region Methods.override
        public override StiSeries CreateNew()
        {
            return new StiPie3dSeries();
        }
        #endregion

        #region Properties     

        [StiCategory("Appearance")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiPropertyLevel(StiLevel.Standard)]
        [TypeConverter(typeof(Design.Sti3dOptionsConverter))]
        [StiOrder(StiPropertyOrder.AppearanceOption3d)]
        public StiPie3dOptions Options3D { get; set; } = new StiPie3dOptions();

        private bool ShouldSerializeOptions3d()
        {
            return Options3D == null || !Options3D.IsDefault;
        }

        [Browsable(false)]
        [StiNonSerialized]
        public override int BorderThickness
        {
            get
            {
                return base.BorderThickness;
            }
            set
            {
                base.BorderThickness = value;
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override StiCutPieListExpression CutPieList
        {
            get
            {
                return base.CutPieList;
            }
            set
            {
                base.CutPieList = value;
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override float Diameter
        {
            get
            {
                return base.Diameter;
            }
            set
            {
                base.Diameter = value;
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override bool Lighting
        {
            get
            {
                return base.Lighting;
            }
            set
            {
                base.Lighting = value;
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override float Distance
        {
            get
            {
                return base.Distance;
            }
            set
            {
                base.Distance = value;
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override StiFontIcons? Icon
        {
            get
            {
                return base.Icon;
            }
            set
            {
                base.Icon = value;
            }
        }
        #endregion

        public StiPie3dSeries()
        {
            this.Core = new StiPie3dSeriesCoreXF(this);
        }
    }
}
