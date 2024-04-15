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
using System.Globalization;

namespace Stimulsoft.Data.Engine
{
    [TypeConverter(typeof(StiDataFilterRuleConverter))]
    public class StiDataFilterRule : 
        StiDataRule, 
        IStiJsonReportObject
    {
        #region IStiJsonReportObject
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyStringNullOrEmpty("Key", Key);
            jObject.AddPropertyStringNullOrEmpty("Path", Path);
            jObject.AddPropertyStringNullOrEmpty("Path2", Path2);
            jObject.AddPropertyEnum("Condition", Condition, StiDataFilterCondition.EqualTo);
            jObject.AddPropertyEnum("Operation", Operation, StiDataFilterOperation.AND);
            jObject.AddPropertyStringNullOrEmpty("Value", Value);
            jObject.AddPropertyStringNullOrEmpty("Value2", Value2);
            jObject.AddPropertyBool("IsEnabled", IsEnabled, true);
            jObject.AddPropertyBool("IsExpression", IsExpression);

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Key":
                        Key = property.DeserializeString();
                        break;

                    case "Path":
                        Path = property.DeserializeString();
                        break;

                    case "Path2":
                        Path2 = property.DeserializeString();
                        break;

                    case "Condition":
                        Condition = property.DeserializeEnum<StiDataFilterCondition>();
                        break;

                    case "Operation":
                        Operation = property.DeserializeEnum<StiDataFilterOperation>(); 
                        break;

                    case "Value":
                        Value = property.DeserializeString();
                        break;

                    case "Value2":
                        Value2 = property.DeserializeString();
                        break;

                    case "IsEnabled":
                        IsEnabled = property.DeserializeBool();
                        break;

                    case "IsExpression":
                        IsExpression = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region Methods.Static
        public static StiDataFilterRule LoadFromJson(string json)
        {
            var rule = new StiDataFilterRule();
            rule.LoadFromJsonObject(JObject.Parse(json));
            return rule;
        }

		public static StiDataFilterRule LoadFromJson(JObject json)
        {
            var rule = new StiDataFilterRule();
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
            switch (Condition)
            {
                case StiDataFilterCondition.Between:
                case StiDataFilterCondition.NotBetween:
                case StiDataFilterCondition.PairEqualTo:
                    return $"{Path} {Condition} {Value} - {Value2}";

                default:
                    return $"{Path} {Condition} {Value}";
            }
        }

        public override int GetUniqueCode()
        {
            unchecked
            {
                var hashCode = Key != null ? Key.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (Path != null ? Path.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Path2 != null ? Path2.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Condition.GetHashCode();
                hashCode = (hashCode * 397) ^ Operation.GetHashCode();
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Value2 != null ? Value2.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsEnabled.GetHashCode();
                hashCode = (hashCode * 397) ^ IsExpression.GetHashCode();
                return hashCode;
            }
        }

        public List<StiDataFilterRule> ToList()
        {
            return new List<StiDataFilterRule> { this };
        }

        public string GetStringRepresentation()
        {
            var path1 = Path;
            if (string.IsNullOrWhiteSpace(path1))
                path1 = $"[{Loc.GetMain("Field")}]";

            var path2 = Path2;
            if (string.IsNullOrWhiteSpace(path2))
                path2 = $"[{Loc.GetMain("Field")}]";

            var value1 = GetValue(Value);
            if (string.IsNullOrWhiteSpace(value1))
                value1 = $"[{Loc.GetMain("Value")}]";

            var value2 = GetValue(Value2);
            if (string.IsNullOrWhiteSpace(value2))
                value2 = $"[{Loc.GetMain("Value")}]";

            switch (Condition)
            {
                case StiDataFilterCondition.IsBlank:
                    return $"{path1} {Loc.GetEnum("StiFilterConditionIsBlank")}";

                case StiDataFilterCondition.IsNotBlank:
                    return $"{path1} {Loc.GetEnum("StiFilterConditionIsNotBlank")}";

                case StiDataFilterCondition.IsNull:
                    return $"{path1} {Loc.GetEnum("StiFilterConditionIsNull")}";

                case StiDataFilterCondition.IsNotNull:
                    return $"{path1} {Loc.GetEnum("StiFilterConditionIsNotNull")}";

                case StiDataFilterCondition.Containing:
                    return $"{path1} {Loc.GetEnum("StiFilterConditionContaining")} {value1}";

                case StiDataFilterCondition.NotContaining:
                    return $"{path1} {Loc.GetEnum("StiFilterConditionNotContaining")} {value1}";

                case StiDataFilterCondition.PairEqualTo:
                    return $"{path1} = {value1} AND {path2} = {value2}";

                case StiDataFilterCondition.EqualTo:
                    return $"{path1} = {value1}";

                case StiDataFilterCondition.NotEqualTo:
                    return $"{path1} <> {value1}";

                case StiDataFilterCondition.GreaterThan:
                    return $"{path1} > {value1}";

                case StiDataFilterCondition.GreaterThanOrEqualTo:
                    return $"{path1} >= {value1}";

                case StiDataFilterCondition.LessThan:
                    return $"{path1} < {value1}";

                case StiDataFilterCondition.LessThanOrEqualTo:
                    return $"{path1} <= {value1}";

                case StiDataFilterCondition.BeginningWith:
                    return $"{path1} {Loc.GetEnum("StiFilterConditionBeginningWith")} {value1}";

                case StiDataFilterCondition.EndingWith:
                    return $"{path1} {Loc.GetEnum("StiFilterConditionEndingWith")} {value1}";

                case StiDataFilterCondition.Between:
                    return $"{path1} {Loc.GetEnum("StiFilterConditionBetween")} {value1} {Loc.GetEnum("StiFilterModeAnd").ToLowerInvariant()} {value2}";

                case StiDataFilterCondition.NotBetween:
                    return $"{path1} {Loc.GetEnum("StiFilterConditionNotBetween")} {value1} {Loc.GetEnum("StiFilterModeAnd").ToLowerInvariant()} {value2}";

                default:
                    return "";
            }
        }

        private string GetValue(string value)
        {
            value = value ?? "";

            DateTime dateValue;
            if (DateTime.TryParseExact(value, "MM'/'dd'/'yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
                return $"'{dateValue.ToShortDateString()}'";

            decimal decimalValue;
            if (decimal.TryParse(value, out decimalValue))
                return $"{decimalValue}";

            if (value != null && (value.ToLowerInvariant() == "true" || value.ToLowerInvariant() == "false"))
                return value;

            return $"'{value}'";
        }
        #endregion

        #region Properties
        [StiSerializable]
        [DefaultValue(null)]
        public string Key { get; set; }

        [Browsable(false)]
        [StiNonSerialized]
        public string ElementKey { get; set; }

        [StiSerializable]
        [DefaultValue(null)]
        public string Path { get; set; }

        [StiSerializable]
        [DefaultValue(null)]
        public string Path2 { get; set; }

        [StiSerializable]
        [DefaultValue(StiDataFilterCondition.EqualTo)]
        public StiDataFilterCondition Condition { get; set; }

        [StiSerializable]
        [DefaultValue(StiDataFilterOperation.AND)]
        public StiDataFilterOperation Operation { get; set; }

        [StiSerializable]
        [DefaultValue(null)]
        public string Value { get; set; }

        [StiSerializable]
        [DefaultValue(null)]
        public string Value2 { get; set; }

        [StiSerializable]
        [DefaultValue(true)]
        public bool IsEnabled { get; set; }

        [StiSerializable]
        [DefaultValue(false)]
        public bool IsExpression { get; set; }
        #endregion

        public StiDataFilterRule()
            : this(null, null, StiDataFilterCondition.EqualTo, null, null, true, false)
        {
        }

        public StiDataFilterRule(string path, StiDataFilterCondition condition)
            : this(null, path, condition, null, null, true, false)
        {
        }

        public StiDataFilterRule(string path, StiDataFilterCondition condition, string value)
            : this(null, path, condition, value, null, true, false)
        {
        }

        public StiDataFilterRule(string key, string path, StiDataFilterCondition condition)
            : this(key, path, condition, null, null, true, false)
        {
        }

        public StiDataFilterRule(string key, string path, StiDataFilterCondition condition, string value)
            : this(key, path, condition, value, null, true, false)
        {
        }

        public StiDataFilterRule(string path, StiDataFilterCondition condition, string value1, string value2)
            : this(null, path, condition, value1, value2, true, false)
        {
        }

        public StiDataFilterRule(string key, string path, StiDataFilterCondition condition, string value, string value2, bool isEnabled, bool isExpression)
            : this(key, path, null, condition, value, value2, isEnabled, isExpression)
        {
        }

        public StiDataFilterRule(string key, string path, string path2, StiDataFilterCondition condition, string value, string value2, bool isEnabled, bool isExpression)
            : this(key, path, path2, condition, StiDataFilterOperation.AND, value, value2, isEnabled, isExpression)
        {
        }

        public StiDataFilterRule(string key, string path, string path2, StiDataFilterCondition condition, StiDataFilterOperation operation, string value, string value2, bool isEnabled, bool isExpression)
        {
            Key = key;
            Path = path;
            Path2 = path2;
            Condition = condition;
            Operation = operation;
            Value = value;
            Value2 = value2;
            IsEnabled = isEnabled;
            IsExpression = isExpression;
        }
    }
}