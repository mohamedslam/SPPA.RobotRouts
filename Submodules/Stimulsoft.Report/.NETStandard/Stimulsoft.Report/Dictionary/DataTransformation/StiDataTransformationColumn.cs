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
using Stimulsoft.Data.Helpers;
using System.Linq;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
	/// Describes a data transformation column.
	/// </summary>
	public class StiDataTransformationColumn : StiDataColumn
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyIdent("Ident", "Transform");

            // StiCalcDataColumn
            jObject.AddPropertyString("Expression", Expression);
            jObject.AddPropertyEnum("Mode", Mode, StiDataTransformationMode.Dimension);

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

                    case "Mode":
                        this.Mode = property.DeserializeEnum<StiDataTransformationMode>();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiDataTransformation;

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
		public string Expression { get; set; }

        /// <summary>
        /// Gets or sets the type of the resource.
        /// </summary>
        [DefaultValue(StiDataTransformationMode.Dimension)]
        [StiSerializable]
        [Browsable(false)]
        public StiDataTransformationMode Mode { get; set; }
        #endregion

        #region Methods
        public StiDataColumn GetDictionaryColumn()
        {
            var transformation = (StiDataTransformation)DataSource;
            if (transformation != null)
            {
                var columnPath = StiExpressionHelper.RemoveFunction(Expression);
                var dataSource = transformation.GetDataSources(new[] { columnPath }).FirstOrDefault() as StiDataSource;
                if (dataSource != null)
                    return dataSource.Columns.ToList().FirstOrDefault(c => c.GetColumnPath().ToLowerInvariant() == columnPath.ToLowerInvariant());
            }

            return null;
        }
        #endregion

        /// <summary>
        /// Creates a new object of the type StiCalcDataColumn.
        /// </summary>
        public StiDataTransformationColumn() : this("DataTransformationColumn", "DataTransformationColumn", typeof(int), "", "", StiDataTransformationMode.Dimension)
		{
		}

        public StiDataTransformationColumn(string name, string alias, Type type, string expression) : base(name, alias, type)
        {
            this.Expression = expression;
        }

        /// <summary>
        /// Creates a new object of the type StiCalcDataColumn.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <param name="alias">Alias of column.</param>
        /// <param name="type">Type of data of column.</param>
        /// <param name="key">Key string.</param>
        public StiDataTransformationColumn(string name, string alias, Type type, string expression, string key, StiDataTransformationMode mode) 
            : base(name, alias, type)
        {
            this.Expression = expression;
            this.Key = key;
            this.Mode = mode;
        }
    }
}
