#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
using Stimulsoft.Base.Serializing;
using Stimulsoft.Data.Design;
using System;
using System.ComponentModel;

namespace Stimulsoft.Data.Engine
{
    [TypeConverter(typeof(StiDataActionRuleConverter))]
    public class StiDataActionRule :
        StiDataRule,
        IStiJsonReportObject
    {
        #region IStiJsonReportObject
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyEnum("Type", Type);
            jObject.AddPropertyStringNullOrEmpty("Key", Key);
            jObject.AddPropertyStringNullOrEmpty("Path", Path);
            jObject.AddPropertyInt("StartIndex", StartIndex);
            jObject.AddPropertyInt("RowsCount", RowsCount, -1);
            jObject.AddPropertyStringNullOrEmpty("InitialValue", InitialValue);
            jObject.AddPropertyStringNullOrEmpty("ValueFrom", ValueFrom);
            jObject.AddPropertyStringNullOrEmpty("ValueTo", ValueTo);
            jObject.AddPropertyBool("MatchCase", MatchCase);
            jObject.AddPropertyBool("MatchWholeWord", MatchWholeWord);
            jObject.AddPropertyEnum("Priority", Priority);

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Type":
                        Type = property.DeserializeEnum<StiDataActionType>();
                        break;

                    case "Key":
                        Key = property.DeserializeString();
                        break;

                    case "Path":
                        Path = property.DeserializeString();
                        break;

                    case "StartIndex":
                        StartIndex = property.DeserializeInt();
                        break;

                    case "RowsCount":
                        RowsCount = property.DeserializeInt();
                        break;

                    case "InitialValue":
                        InitialValue = property.DeserializeString();
                        break;

                    case "ValueFrom":
                        ValueFrom = property.DeserializeString();
                        break;

                    case "ValueTo":
                        ValueTo = property.DeserializeString();
                        break;

                    case "MatchCase":
                        MatchCase = property.DeserializeBool();
                        break;

                    case "MatchWholeWord":
                        MatchWholeWord = property.DeserializeBool();
                        break;

                    case "AfterGroupingData": // Support for older versions before 2022.5.1
                        var value = property.DeserializeBool();
                        if (value)
                            Priority = StiDataActionPriority.AfterGroupingData;
                        break;

                    case "Priority":
                        Priority = property.DeserializeEnum<StiDataActionPriority>();
                        break;
                }
            }
        }
        #endregion

        #region Properties
        [StiSerializable]
        public StiDataActionType Type { get; set; }

        [StiSerializable]
        [DefaultValue(null)]
        public string Key { get; set; }

        [StiSerializable]
        [DefaultValue(null)]
        public string Path { get; set; }

        [StiSerializable]
        [DefaultValue(0)]
        public int StartIndex { get; set; }

        [StiSerializable]
        [DefaultValue(-1)]
        public int RowsCount { get; set; } = -1;

        [StiSerializable]
        [DefaultValue(null)]
        public string InitialValue { get; set; }

        [StiSerializable]
        [DefaultValue(null)]
        public string ValueFrom { get; set; }

        [StiSerializable]
        [DefaultValue(null)]
        public string ValueTo { get; set; }

        [StiSerializable]
        [DefaultValue(false)]
        public bool MatchCase { get; set; }

        [StiSerializable]
        [DefaultValue(false)]
        public bool MatchWholeWord { get; set; }

        [StiSerializable]
        [DefaultValue(StiDataActionPriority.AfterGroupingData)]
        public StiDataActionPriority Priority { get; set; } = StiDataActionPriority.AfterGroupingData;
        #endregion

        #region Methods
        public static StiDataActionRule LoadFromJson(JObject json)
        {
            var link = new StiDataActionRule();
            link.LoadFromJsonObject(json);
            return link;
        }

        public override int GetUniqueCode()
        {
            unchecked
            {
                var hashCode = (int)Type;

                hashCode = (hashCode * 397) ^ (Key != null ? Key.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Path != null ? Path.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ StartIndex;
                hashCode = (hashCode * 397) ^ RowsCount;
                hashCode = (hashCode * 397) ^ (InitialValue != null ? InitialValue.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ValueFrom != null ? ValueFrom.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ValueTo != null ? ValueTo.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ MatchCase.GetHashCode();
                hashCode = (hashCode * 397) ^ MatchWholeWord.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)Priority;

                return hashCode;
            }
        }
        #endregion

        public StiDataActionRule()
        {
        }

        public StiDataActionRule(string key, string path)
            : this(key, path, StiDataActionType.Percentage, 0, -1, StiDataActionPriority.AfterGroupingData, null, null, false, false, null)
        {
        }

        public StiDataActionRule(string key, string path, int startIndex, int rowsCount, StiDataActionPriority priority)
            : this(key, path, StiDataActionType.Limit, startIndex, rowsCount, priority, null, null, false, false, null)
        {
        }

        public StiDataActionRule(string key, string path, string valueFrom, string valueTo, bool matchCase, bool matchWholeWord)
            : this(key, path, StiDataActionType.Replace, 0, -1, StiDataActionPriority.BeforeTransformation, valueFrom, valueTo, matchCase, matchWholeWord, null)
        {
        }

        public StiDataActionRule(string key, string path, string initialValue)
            : this(key, path, StiDataActionType.RunningTotal, 0, -1, StiDataActionPriority.AfterGroupingData, null, null, false, false, initialValue)
        {
        }
        
        public StiDataActionRule(string key, string path, StiDataActionType type, int startIndex, int rowsCount, StiDataActionPriority priority,
            string valueFrom, string valueTo, bool matchCase, bool matchWholeWord, string initialValue)
        {
            Key = key;
            Path = path;
            Type = type;
            StartIndex = startIndex;
            RowsCount = rowsCount;
            ValueFrom = valueFrom;
            ValueTo = valueTo;
            MatchCase = matchCase;
            MatchWholeWord = matchWholeWord;
            InitialValue = initialValue;
            Priority = priority;
        }
    }
}
