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

using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Dictionary.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
	/// Describes a base Parameter class.
	/// </summary>	
	[StiSerializable]
	[TypeConverter(typeof(Design.StiDataParameterConverter))]
	public class StiDataParameter : 
		StiExpression,
		IStiName,
        IStiInherited,
        IStiPropertyGridObject
    {
        #region enum Order
        public enum Order
        {
            Name = 100,
            Expression = 200,
            Size = 300,
            Type = 400
        }
        #endregion

        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("Value");

            // StiDataParameter
            jObject.AddPropertyString("Name", Name);
            jObject.AddPropertyStringNullOrEmpty("Expression", Expression);
            jObject.AddPropertyInt("Type", Type);
            jObject.AddPropertyInt("Size", Size);
            jObject.AddPropertyStringNullOrEmpty("Key", Key);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Name":
                        this.Name = property.DeserializeString();
                        break;

                    case "Expression":
                        this.Expression = property.DeserializeString();
                        break;

                    case "Type":
                        this.Type = property.DeserializeInt();
                        break;

                    case "Size":
                        this.Size = property.DeserializeInt();
                        break;

                    case "Key":
                        this.Key = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public StiComponentId ComponentId => StiComponentId.StiDataParameter;

        [Browsable(false)]
        public string PropName => this.Name;

        public StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // MainCategory
            var list = new[] 
            { 
                propHelper.Name(),
                propHelper.Expression(), 
                propHelper.DataParameterSize(), 
                propHelper.DataParameterType()
            };
            objHelper.Add(StiPropertyCategories.Data, list);

            return objHelper;
        }
        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return null;
        }
        #endregion

		#region IStiInherited
		[Browsable(false)]
		[DefaultValue(false)]
		public bool Inherited
		{
			get
			{
			    return DataSource != null && DataSource.Inherited;
			}
			set
			{
			}
		}
		#endregion

		#region IStiName
        /// <summary>
		/// Gets or sets a parameter name.
		/// </summary>
		[DefaultValue("Parameter")]
		[RefreshProperties(RefreshProperties.All)]
		[StiCategory("Data")]
		[Description("Gets or sets a parameter name.")]
		[ParenthesizePropertyName(true)]
        [StiOrder((int)Order.Name)]
		public string Name { get; set; }
        #endregion

		#region Properties
		[Browsable(false)]
		public override bool ApplyFormat => false;

        [Browsable(false)]
		[StiNonSerialized]
		public override string Value
		{
			get
			{
				return base.Value;
			}
			set
			{
				base.Value = value;
			}
		}

        /// <summary>
        /// Gets or sets expression of the calculated column.
        /// </summary>
		[StiSerializable(
			 StiSerializeTypes.SerializeToDesigner | 
			 StiSerializeTypes.SerializeToSaveLoad | 
			 StiSerializeTypes.SerializeToDocument)]
		[StiCategory("Data")]
		[Editor("Stimulsoft.Report.Components.Design.StiExpressionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiOrder((int)Order.Expression)]
        [Description("Gets or sets expression of the calculated column.")]
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

		[Browsable(false)]
		public object ParameterValue { get; set; }

        /// <summary>
		/// Gets or sets the Data Source in what the parameter is described.
		/// </summary>
		[Browsable(false)]
		public StiDataSource DataSource { get; set; }

        /// <summary>
		/// Gets or sets type of parameter.
		/// </summary>
		[StiSerializable]
		[StiCategory("Data")]
		[TypeConverter(typeof(StiDataParameterTypeConverter))]
		[Editor("Stimulsoft.Report.Dictionary.Design.StiDataParameterTypeEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiOrder((int)Order.Type)]
        [Description("Gets or sets type of parameter.")]
		public virtual int Type { get; set; }

        /// <summary>
		/// Gets or sets a size of the parameter.
		/// </summary>
		[StiSerializable]
		[StiCategory("Data")]
        [StiOrder((int)Order.Size)]
        [Description("Gets or sets a size of the parameter.")]
		public virtual int Size { get; set; }

        /// <summary>
        /// Gets or sets the key of the dictionary object.
        /// </summary>
        [DefaultValue(null)]
        [StiSerializable]
        [Browsable(false)]
        public string Key { get; set; }
        #endregion

        #region Methods
        public object GetParameterValue()
        {
            Hashtable variables = null;
            if (DataSource != null && DataSource.Dictionary != null && DataSource.Dictionary.Report != null)
                variables = DataSource.Dictionary.Report.Variables;

            object value;
            if (variables != null && variables.ContainsKey(this.Name))
            {
                value = variables[this.Name];
                
                //if user need assign null value he can assign string "#null#"
                if (value is string && (string)value == "#null#")
                    value = null;
            }
            else
                value = this.ParameterValue;

            return value;
        }

        public override string ToString()
		{
			return Name;
		}
		#endregion	

		#region Fields
		internal StiDataParametersCollection dataParametersCollection = null;
		#endregion

		/// <summary>
		/// Creates a new object of the type StiDataParameter.
		/// </summary>
		public StiDataParameter() : this("Parameter", 0, 0)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiDataParameter.
		/// </summary>
		public StiDataParameter(string name, int type, int size) : this(name, string.Empty, type, size)
		{
		}

        /// <summary>
        /// Creates a new object of the type StiDataParameter.
        /// </summary>
        public StiDataParameter(string name, int type, int size, string key)
            : this(name, string.Empty, type, size, key)
        {
        }

		/// <summary>
		/// Creates a new object of the type StiDataParameter.
		/// </summary>
		public StiDataParameter(string name, string value, int type, int size)
		{
			this.Type = type;
			this.Name = name;
			this.Value = value;
			this.Size = size;
		}

        /// <summary>
        /// Creates a new object of the type StiDataParameter.
        /// </summary>
        public StiDataParameter(string name, string value, int type, int size, string key)
        {
            this.Type = type;
            this.Name = name;
            this.Value = value;
            this.Size = size;
            this.Key = key;
        }
	}
}
