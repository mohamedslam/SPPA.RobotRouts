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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using System;
using System.ComponentModel;

namespace Stimulsoft.Report.Chart
{
    public class StiWaterfallSeries :
        StiClusteredColumnSeries,
        IStiWaterfallSeries
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiWaterfallSeries;
            }
        }
        #endregion

        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);
            
            jObject.AddPropertyJObject("ConnectorLine", ConnectorLine.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Total", Total.SaveToJsonObject(mode));

            return jObject;
        }
        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "ConnectorLine":
                        this.ConnectorLine.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "Total":
                        this.Total.LoadFromJsonObject((JObject)property.Value);
                        break;
                }
            }
        }
        #endregion

        #region Properties
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiCategory("Common")]
        [TypeConverter(typeof(StiUniversalConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        public IStiWaterfallConnectorLine ConnectorLine { get; set; }

        [StiSerializable(StiSerializationVisibility.Class)]
        [StiCategory("Common")]
        [TypeConverter(typeof(StiUniversalConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        public IStiWaterfallTotal Total { get; set; }
        #endregion

        #region Methods.Types
        public override Type GetDefaultAreaType()
        {
            return typeof(StiWaterfallArea);
        }
        #endregion

        #region Methods.override
        public override StiSeries CreateNew()
        {
            return new StiWaterfallSeries();
        }
        #endregion

        public StiWaterfallSeries()
        {
            this.Core = new StiWaterfallSeriesCoreXF(this);
            this.ConnectorLine = new StiWaterfallConnectorLine();
            this.Total = new StiWaterfallTotal();
        }
    }
}
