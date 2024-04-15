#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Dashboard.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Dashboard
{
    [TypeConverter(typeof(StiUserViewStateConverter))]
    public class StiUserViewState : 
        ICloneable,
        IStiJsonReportObject
    {
        #region ICloneable
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region IStiJsonReportObject
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyStringNullOrEmpty(nameof(Key), Key);
            jObject.AddPropertyStringNullOrEmpty(nameof(Name), Name);
            jObject.AddPropertyStringNullOrEmpty(nameof(State), State);
            jObject.AddPropertyEnum(nameof(SeriesType), SeriesType, StiChartSeriesType.ClusteredColumn);

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Key):
                        this.Key = property.DeserializeString();
                        break;

                    case nameof(Name):
                        this.Name = property.DeserializeString();
                        break;

                    case nameof(State):
                        this.State = property.DeserializeString();
                        break;

                    case nameof(SeriesType):
                        this.SeriesType = property.DeserializeEnum<StiChartSeriesType>();
                        break;
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Saves element to string.
        /// </summary>
        /// <returns>String representation which contains schema.</returns>
        public virtual string SaveToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, StiJsonHelper.DefaultSerializerSettings);
        }

        public List<StiUserViewState> ToList()
        {
            return new List<StiUserViewState> { this };
        }
        #endregion

        #region Methods.Static
        public static StiUserViewState LoadFromJson(string json)
        {
            var rule = new StiUserViewState();
            rule.LoadFromJsonObject(JObject.Parse(json));
            return rule;
        }

        public static StiUserViewState LoadFromJson(JObject json)
        {
            var rule = new StiUserViewState();
            rule.LoadFromJsonObject(json);
            return rule;
        }
        #endregion

        #region Properties
        [StiSerializable]
        [DefaultValue(null)]
        public string Key { get; set; }

        [StiSerializable]
        [DefaultValue(null)]
        public string Name { get; set; }

        [StiSerializable]
        [DefaultValue(null)]
        public string State { get; set; }

        [StiSerializable]
        [DefaultValue(StiChartSeriesType.ClusteredColumn)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        public StiChartSeriesType SeriesType { get; set; } = StiChartSeriesType.ClusteredColumn;
        #endregion

        public StiUserViewState() 
            : this(StiKeyHelper.GenerateKey(), "Name", null, StiChartSeriesType.ClusteredColumn)
        {
        }

        public StiUserViewState(string key, string name, string state, StiChartSeriesType seriesType)
        {
            this.Key = key;
            this.Name = name;
            this.State = state;
            this.SeriesType = seriesType;
        }
    }
}