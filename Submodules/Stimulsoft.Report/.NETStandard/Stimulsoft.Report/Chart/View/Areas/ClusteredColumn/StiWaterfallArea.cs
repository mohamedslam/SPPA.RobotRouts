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
using System.ComponentModel;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Design;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiWaterfallArea :
        StiAxisArea,
        IStiWaterfallArea
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyBool(nameof(RoundValues), RoundValues);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(RoundValues):
                        this.RoundValues = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiWaterfallArea;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var objHelper = new StiPropertyCollection();

            objHelper.Add(StiPropertyCategories.Main, new[]
            {
                propertyGrid.PropertiesHelper.WaterfallArea(),
            });

            return objHelper;
        }
        #endregion

        #region Properties
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [DefaultValue(false)]
        public bool RoundValues { get; set; } 
        #endregion

        #region Methods.Types
        public override Type GetDefaultSeriesType()
        {
            return typeof(StiWaterfallSeries);
        }

        public override Type[] GetSeriesTypes()
        {
            return new[]
            {
                 typeof(StiWaterfallSeries)
            };
        }
        public override Type[] GetSeriesLabelsTypes()
        {
            return new[]
            {
                typeof(StiNoneLabels),
                typeof(StiInsideBaseAxisLabels),
                typeof(StiInsideEndAxisLabels),
                typeof(StiCenterAxisLabels),
                typeof(StiOutsideBaseAxisLabels),
                typeof(StiOutsideEndAxisLabels),
                typeof(StiOutsideAxisLabels),
                typeof(StiValueAxisLabels),
            };
        }
        #endregion

        #region Methods.override
        public override StiArea CreateNew()
        {
            return new StiWaterfallArea();
        }
        #endregion

        [StiUniversalConstructor("Area")]
        public StiWaterfallArea()
        {
            this.Core = new StiWaterfallAreaCoreXF(this);
        }
    }
}
