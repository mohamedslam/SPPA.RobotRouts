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
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Data.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Stimulsoft.Data.Engine
{
    [TypeConverter(typeof(StiDataSortRuleConverter))]
    public class StiDataSortRule : 
        StiDataRule,
        IStiJsonReportObject
    {
        #region IStiJsonReportObject
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyStringNullOrEmpty("Key", Key);
            jObject.AddPropertyEnum("Direction", Direction, StiDataSortDirection.Ascending);

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Key":
                        this.Key = property.DeserializeString();
                        break;

                    case "Direction":
                        this.Direction = property.DeserializeEnum<StiDataSortDirection>();
                        break;
                }
            }
        }
        #endregion

        #region Methods.Static
        public static StiDataSortRule LoadFromJson(string json)
        {
            var rule = new StiDataSortRule();
            rule.LoadFromJsonObject(JObject.Parse(json));
            return rule;
        }

        public static StiDataSortRule LoadFromJson(JObject json)
        {
            var rule = new StiDataSortRule();
            rule.LoadFromJsonObject(json);
            return rule;
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

        public override string ToString()
        {
            return $"{Direction} {Key}";
        }

        public override int GetUniqueCode()
        {
            unchecked
            {
                return ((Key != null ? Key.GetHashCode() : 0) * 397) ^ (int)Direction;
            }
        }

        public string GetStringRepresentation()
        {
            var path = Key;
            if (string.IsNullOrWhiteSpace(path))
                path = $"[{Loc.GetMain("Field")}]";

            switch (Direction)
            {
                case StiDataSortDirection.Ascending:
                    return $"{path} {Loc.GetEnum("StiSortDirectionAsc")}";

                case StiDataSortDirection.Descending:
                    return $"{path} {Loc.GetEnum("StiSortDirectionDesc")}";

                default:
                    return "";
            }
        }

        public List<StiDataSortRule> ToList()
        {
            return new List<StiDataSortRule> { this };
        }
        #endregion

        #region Properties
        [StiSerializable]
        [DefaultValue(null)]
        public string Key { get; set; }

        [StiSerializable]
        [DefaultValue(StiDataSortDirection.Ascending)]
        public StiDataSortDirection Direction { get; set; }
        #endregion

        public StiDataSortRule() 
            : this(StiKeyHelper.GenerateKey(), StiDataSortDirection.Ascending)
        {
        }

        public StiDataSortRule(string key, StiDataSortDirection direction)
        {
            this.Key = key;
            this.Direction = direction;
        }
    }
}