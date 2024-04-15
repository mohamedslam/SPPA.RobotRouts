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
using System.ComponentModel;
using System.Drawing.Design;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
	/// Describes a calculated column with expression.
	/// </summary>
	public class StiCalcDataColumn : 
	    StiDataColumn, 
	    IStiAppCalcDataColumn
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyIdent("Ident", "Calc");

            // StiCalcDataColumn
            jObject.AddPropertyString("Expression", Expression);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Expression":
                        this.Expression = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiCalcDataColumn;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // DataCategory
            var list = new[]
            {
                propHelper.Alias(),
                propHelper.Name(),
                propHelper.NameInSource(),
                propHelper.StringExpression(),
                propHelper.Type()
            };
            objHelper.Add(StiPropertyCategories.Data, list);

            return objHelper;
        }
        #endregion

		#region Properties
        [Browsable(false)]
        public override string NameInSource
        {
            get
            {
                return base.NameInSource;
            }
            set
            {
                base.NameInSource = value;
            }
        }

	    /// <summary>
		/// Gets or sets a column value.
		/// </summary>
		[DefaultValue("Column")]
		[RefreshProperties(RefreshProperties.All)]
		[StiCategory("Data")]
		[Browsable(false)]
		[StiNonSerialized]
		public string Value { get; set; }

	    /// <summary>
        /// Gets or sets an expression of the calculated column.
        /// </summary>
		[StiSerializable(
			 StiSerializeTypes.SerializeToDesigner | 
			 StiSerializeTypes.SerializeToSaveLoad | 
			 StiSerializeTypes.SerializeToDocument)]
		[StiCategory("Data")]
		[Editor("Stimulsoft.Report.Components.Design.StiExpressionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiOrder((int)Order.Expression)]
        [Description("Gets or sets an expression of the calculated column.")]
		public string Expression
		{
			get
			{
				return Value;
			}
			set
			{
				Value = value;
			}
		}
		#endregion

		/// <summary>
		/// Creates a new object of the type StiCalcDataColumn.
		/// </summary>
		public StiCalcDataColumn() : this("CalcColumn", "CalcColumn", typeof(int), "")
		{
		}

        /// <summary>
        /// Creates a new object of the type StiCalcDataColumn.
        /// </summary>
        /// <param name="name">A name of column.</param>
        /// <param name="type">A type of data of column.</param>
        public StiCalcDataColumn(string name, Type type) : this(name, name, type)
		{
		}

        /// <summary>
        /// Creates a new object of the type StiCalcDataColumn.
        /// </summary>
        /// <param name="name">A name of column.</param>
        /// <param name="alias">An alias of column.</param>
        /// <param name="type">A type of data of column.</param>
        public StiCalcDataColumn(string name, string alias, Type type) :
			base(name, alias, type)
		{
		}

        /// <summary>
        /// Creates a new object of the type StiCalcDataColumn.
        /// </summary>
        /// <param name="name">A name of column.</param>
        /// <param name="alias">An alias of column.</param>
        /// <param name="type">A type of data of column.</param>
        /// <param name="value">A value for calculated data column.</param>
        public StiCalcDataColumn(string name, string alias, Type type, string value) :
			base(name, alias, type)
		{
			this.Value = value;
		}

        /// <summary>
        /// Creates a new object of the type StiCalcDataColumn.
        /// </summary>
        /// <param name="name">A name of column.</param>
        /// <param name="alias">An alias of column.</param>
        /// <param name="type">A type of data of column.</param>
        /// <param name="value">A value for calculated data column.</param>
        /// <param name="key">A key string.</param>
        public StiCalcDataColumn(string name, string alias, Type type, string value, string key) :
            base(name, alias, type)
        {
            this.Value = value;
            this.Key = key;
        }
    }
}
