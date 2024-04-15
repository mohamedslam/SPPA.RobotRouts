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
using Stimulsoft.Report.Components.Design;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;

namespace Stimulsoft.Report.Components
{
    [TypeConverter(typeof(StiFilterConverter))]
	[StiSerializable]
	public class StiFilter : 
        ICloneable,
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            // StiFilter
            jObject.AddPropertyEnum("Condition", Condition, StiFilterCondition.EqualTo);
            jObject.AddPropertyEnum("DataType", DataType, StiFilterDataType.String);
            jObject.AddPropertyStringNullOrEmpty("Column", Column);
            jObject.AddPropertyEnum("Item", Item, StiFilterItem.Value);
            jObject.AddPropertyStringNullOrEmpty("Value1", Value1);
            jObject.AddPropertyStringNullOrEmpty("Value2", Value2);
            jObject.AddPropertyJObject("Expression", Expression.SaveToJsonObject(mode));

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

                    case "Column":
                        this.Column = property.DeserializeString();
                        break;

                    case "Item":
                        this.Item = property.DeserializeEnum<StiFilterItem>();
                        break;

                    case "Value1":
                        this.Value1 = property.DeserializeString();
                        break;

                    case "Value2":
                        this.Value2 = property.DeserializeString();
                        break;

                    case "Expression":
                        this.Expression.LoadFromJsonObject((JObject)property.Value);
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
		public virtual object Clone()
		{
			return this.MemberwiseClone();
		}
		#endregion

		#region Properties
        [DefaultValue(StiFilterCondition.EqualTo)]
		[StiSerializable]
		public virtual StiFilterCondition Condition { get; set; } = StiFilterCondition.EqualTo;

        [DefaultValue(StiFilterDataType.String)]
		[StiSerializable]
		public virtual StiFilterDataType DataType { get; set; } = StiFilterDataType.String;

        [DefaultValue("")]
		[StiSerializable]
		public virtual string Column { get; set; } = string.Empty;

        [DefaultValue(StiFilterItem.Value)]
		[StiSerializable]
		public virtual StiFilterItem Item { get; set; } = StiFilterItem.Value;

        [DefaultValue("")]
		[StiSerializable]
		public virtual string Value1 { get; set; } = string.Empty;

        [DefaultValue("")]
		[StiSerializable]
		public virtual string Value2 { get; set; } = string.Empty;

        /// <summary>
		/// Gets or sets the filter expression.
		/// </summary>
		[StiCategory("Data")]
		[StiSerializable]
		[Description("Gets or sets the filter expression.")]
		public virtual StiExpression Expression { get; set; } = new StiFilterExpression();
        #endregion

		#region Methods
		private static string ConvertData(DateTime date)
		{
			var currentCulture = Thread.CurrentThread.CurrentCulture;

			try
			{
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
				return date.ToShortDateString();
			}
			finally
			{
                Thread.CurrentThread.CurrentCulture = currentCulture;
			}
		}
		#endregion

		/// <summary>
		/// Creates a new object of the type StiFilter.
		/// </summary>
		public StiFilter()
		{
		}

		/// <summary>
		/// Creates a new object of the type StiFilter.
		/// </summary>
		public StiFilter(string expression)
		{
            if (!(this is StiMultiCondition))
            {
                this.Expression.Value = expression;
                this.Item = StiFilterItem.Expression;
            }
        }

		/// <summary>
		/// Creates a new object of the type StiFilter.
		/// </summary>
		public StiFilter(string column, StiFilterCondition condition, DateTime date1) :
			this(column, condition, ConvertData(date1), string.Empty, StiFilterDataType.DateTime)
		{			
		}

		/// <summary>
		/// Creates a new object of the type StiFilter.
		/// </summary>
		public StiFilter(string column, StiFilterCondition condition, DateTime date1, DateTime date2) :
			this(column, condition, ConvertData(date1), ConvertData(date2), StiFilterDataType.DateTime)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiFilter.
		/// </summary>
		public StiFilter(string column, StiFilterCondition condition, string value, StiFilterDataType dataType) :
			this(column, condition, value, string.Empty, dataType)
		{
			
		}

		/// <summary>
		/// Creates a new object of the type StiFilter.
		/// </summary>
		public StiFilter(string column, StiFilterCondition condition, string value1, string value2, StiFilterDataType dataType) :
			this(StiFilterItem.Value, column, condition, value1, value2, dataType, string.Empty)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiFilter.
		/// </summary>
		public StiFilter(StiFilterItem item, string column, StiFilterCondition condition, 
			string value1, string value2, StiFilterDataType dataType, string expression)
		{
			this.Item = item;
			this.Column = column;
			this.Condition = condition;
			this.Value1 = value1;
			this.Value2 = value2;
			this.DataType = dataType;
			this.Expression.Value = expression;
		}
	}
}
