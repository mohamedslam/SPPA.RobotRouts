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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using Stimulsoft.Report.Helpers;
using System.Drawing.Design;
using Stimulsoft.Base.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiPictorialStackedSeries : 
        StiSeries,
        IStiPictorialStackedSeries
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyEnum("Icon", Icon, StiFontIcons.QuarterFull);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Icon":
                        this.Icon = property.DeserializeEnum<StiFontIcons>();
                        break;
                }
            }
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

        private StiFontIcons? icon = StiFontIcons.Coffee;
        [DefaultValue(StiFontIcons.Coffee)]
        [StiSerializable]
        [StiCategory("Common")]
        [Editor("Stimulsoft.Report.Design.StiFontIconEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(Stimulsoft.Report.Design.StiFontIconConverter))]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiFontIcons? Icon
        {
            get
            {
                return icon;
            }
            set
            {
                if (value != null)
                {
                    icon = value;
                }
            }
        }
        #endregion

        #region Methods.Types
        public override Type GetDefaultAreaType()
        {
            return typeof(StiPictorialStackedArea);
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiPictorialStackedSeries;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            objHelper.Add(StiPropertyCategories.Value, new[]
            {
                propHelper.ValueDataColumn(),
                propHelper.Value(),
                propHelper.ListOfValues()
            });

            objHelper.Add(StiPropertyCategories.Argument, new[]
            {
                propHelper.ArgumentDataColumn(),
                propHelper.Argument(),
                propHelper.ListOfArguments()
            });

            objHelper.Add(StiPropertyCategories.Data, new[]
            {
                propHelper.Conditions(),
                propHelper.FilterMode(),
                propHelper.Filters(),
                propHelper.Format(),
                propHelper.SortBy(),
                propHelper.SortDirection(),
                //propHelper.TopN()
            });

            objHelper.Add(StiPropertyCategories.Appearance, new[]
            {
                propHelper.AllowApplyStyle()
            });

            objHelper.Add(StiPropertyCategories.Behavior, new[]
            {
                propHelper.Icon(),
                propHelper.SeriesInteraction(),
                //propHelper.SeriesLabels(),
                propHelper.ShowInLegend(),
                propHelper.ShowSeriesLabels(),
                propHelper.Title()
            });

            objHelper.Add(StiPropertyCategories.Series, new[]
            {
                propHelper.AutoSeriesKeyDataColumn(),
                propHelper.AutoSeriesColorDataColumn(),
                propHelper.AutoSeriesTitleDataColumn(),
            });

            return objHelper;
        }
        #endregion

        #region Methods.override
        public override StiSeries CreateNew()
        {
            return new StiPictorialStackedSeries();
        }
        #endregion

        public StiPictorialStackedSeries()
        {
            this.Core = new StiPictorialStackedSeriesCoreXF(this);
        }
    }
}