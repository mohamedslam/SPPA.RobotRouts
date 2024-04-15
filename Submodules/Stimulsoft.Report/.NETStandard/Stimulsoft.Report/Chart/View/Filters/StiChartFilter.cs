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
using Stimulsoft.Report.Chart.Design;
using Stimulsoft.Report.Components;
using System;
using System.ComponentModel;

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(StiChartFilterConverter))]
    [StiSerializable]
    public class StiChartFilter :
        IStiChartFilter,
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            jObject.AddPropertyEnum("Condition", Condition, StiFilterCondition.EqualTo);
            jObject.AddPropertyEnum("DataType", DataType, StiFilterDataType.String);
            jObject.AddPropertyEnum("Item", Item, StiFilterItem.Argument);
            jObject.AddPropertyStringNullOrEmpty("Value", Value);

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Condition":
                        this.Condition = property.DeserializeEnum<StiFilterCondition>();
                        break;

                    case "DataType":
                        this.DataType = property.DeserializeEnum<StiFilterDataType>();
                        break;

                    case "Item":
                        this.Item = property.DeserializeEnum<StiFilterItem>();
                        break;

                    case "Value":
                        this.Value = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public int Index
        {
            get
            {
                if (Filters == null)
                    return -1;

                return Filters.IndexOf(this);
            }
        }

        [DefaultValue(StiFilterCondition.EqualTo)]
        public StiFilterCondition Condition { get; set; }

        [DefaultValue(StiFilterDataType.String)]
        public StiFilterDataType DataType { get; set; }

        [DefaultValue(StiFilterItem.Argument)]
        public StiFilterItem Item { get; set; }

        [DefaultValue("")]
        public string Value { get; set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            var sign = "";
            switch (Condition)
            {
                case StiFilterCondition.EqualTo:
                    sign = "=";
                    break;

                case StiFilterCondition.NotEqualTo:
                    sign = "<>";
                    break;

                case StiFilterCondition.GreaterThan:
                    sign = ">";
                    break;

                case StiFilterCondition.GreaterThanOrEqualTo:
                    sign = ">=";
                    break;

                case StiFilterCondition.LessThan:
                    sign = "<";
                    break;

                case StiFilterCondition.LessThanOrEqualTo:
                    sign = "=<";
                    break;
            }

            var item = StiLocalization.Get("PropertyMain", "Argument");
            switch (Item)
            {
                case StiFilterItem.Expression:
                    item = StiLocalization.Get("PropertyMain", "Expression");
                    break;

                case StiFilterItem.ValueClose:
                    item = StiLocalization.Get("PropertyMain", "ValueClose");
                    break;

                case StiFilterItem.ValueHigh:
                    item = StiLocalization.Get("PropertyMain", "ValueHigh");
                    break;

                case StiFilterItem.ValueLow:
                    item = StiLocalization.Get("PropertyMain", "ValueLow");
                    break;

                case StiFilterItem.ValueOpen:
                    item = StiLocalization.Get("PropertyMain", "ValueOpen");
                    break;

                case StiFilterItem.Value:
                    item = StiLocalization.Get("PropertyMain", "Value");
                    break;

                case StiFilterItem.ValueEnd:
                    item = StiLocalization.Get("PropertyMain", "Value");
                    break;
            }

            return $"{item} {sign} {Value}";
        }
        #endregion

        #region Properties
        internal StiChartFiltersCollection Filters { get; set; }
        #endregion

        /// <summary>
		/// Creates a new object of the type StiChartFilter.
		/// </summary>
		public StiChartFilter() : this(
            StiFilterItem.Argument,
            StiFilterDataType.String,
            StiFilterCondition.EqualTo, string.Empty)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiChartFilter.
        /// </summary>
        public StiChartFilter(StiFilterItem item, StiFilterDataType dataType,
            StiFilterCondition condition, string value)
        {
            this.Item = item;
            this.DataType = dataType;
            this.Condition = condition;
            this.Value = value;
        }
    }
}
