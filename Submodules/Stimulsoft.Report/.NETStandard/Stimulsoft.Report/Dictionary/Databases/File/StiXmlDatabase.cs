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
using System.Data;
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Dictionary.Design;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.PropertyGrid;
using System.Threading.Tasks;
using System.Linq;
using Stimulsoft.Base.Design;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dictionary
{
    [TypeConverter(typeof(StiXmlDatabaseConverter))]
    public class StiXmlDatabase : StiFileDatabase
    {
        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiXmlDatabase;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // DataCategory
            var list = new[]
            {
                propHelper.Name(),
                propHelper.Alias(),

                propHelper.PathData(),
                propHelper.PathSchema(),
                propHelper.XmlType()
            };
            objHelper.Add(StiPropertyCategories.Data, list);

            return objHelper;
        }
        #endregion

        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiXmlDatabase
            jObject.AddPropertyStringNullOrEmpty("PathSchema", PathSchema);
            jObject.AddPropertyStringNullOrEmpty("PathData", PathData);
            jObject.AddPropertyEnum("XmlType", XmlType, StiXmlType.AdoNetXml);
            jObject.AddPropertyEnum("RelationDirection", RelationDirection, StiRelationDirection.ParentToChild);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "PathSchema":
                        this.PathSchema = property.DeserializeString();
                        break;

                    case "PathData":
                        this.PathData = property.DeserializeString();
                        break;

                    case "XmlType":
                        this.XmlType = property.DeserializeEnum<StiXmlType>();
                        break;

                    case "RelationDirection":
                        this.RelationDirection = property.DeserializeEnum<StiRelationDirection>(); 
                        break;
                }
            }
        }
        #endregion

        #region Properties
        [StiSerializable]
        [Description("Specifies direction of the relation processing.")]
        [TypeConverter(typeof(StiEnumConverter))]
        [DefaultValue(StiRelationDirection.ParentToChild)]
        [Browsable(false)]
        public StiRelationDirection RelationDirection { get; set; } = StiRelationDirection.ParentToChild;

        /// <summary>
        /// Gets or sets a path to the xml schema.
        /// </summary>
        [StiSerializable]
        [Description("Gets or sets a path to the xml schema.")]
        [StiOrder((int) Order.PathSchema)]
        public string PathSchema { get; set; }

        [StiSerializable]        
        [DefaultValue(typeof(StiXmlType), "AdoNetXml")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiOrder((int) Order.XmlType)]
        public StiXmlType XmlType { get; set; } = StiXmlType.AdoNetXml;
        #endregion

        #region Methods
        public override StiDatabase CreateNew()
        {
            return new StiXmlDatabase();
        }

        /// <summary>
        /// Returns new data connector for this database.
        /// </summary>
        /// <returns>Created connector.</returns>
        public override StiDataConnector CreateConnector(string connectionString = null)
        {
            return StiXmlConnector.Get();
        }

        public override DialogResult Edit(StiDictionary dictionary, bool newDatabase)
        {
            return StiDataEditorsHelper.Get().XmlDatabaseEdit(this, dictionary, newDatabase);
        }

        public override Task<DialogResult> EditAsync(StiDictionary dictionary, bool newDatabase)
        {
            var dialogResult = StiDataEditorsHelper.Get().XmlDatabaseEdit(this, dictionary, newDatabase);

            return Task<DialogResult>.Factory.StartNew(() => dialogResult);
        }

        protected override StiFileDataOptions GetConnectorOptions(StiReport report, bool isSchema)
        {
            byte[] content = StiUniversalDataLoader.LoadSingle(report, ParsePath(PathData, report))?.Array;
            byte[] schema = null;

            if (XmlType == StiXmlType.AdoNetXml)
                schema = StiUniversalDataLoader.LoadSingle(report, ParsePath(PathSchema, report))?.Array;

            return new StiXmlOptions(schema, content, XmlType == StiXmlType.AdoNetXml);
        }
        #endregion

        public StiXmlDatabase()
		{
		}

        public StiXmlDatabase(string name, string pathData)
            : this(name, null, pathData)
        {
            this.XmlType = StiXmlType.Xml;
        }

		public StiXmlDatabase(string name, string pathSchema, string pathData) : base(name, pathData)
		{
			this.PathSchema = pathSchema;
		}

        public StiXmlDatabase(string name, string pathSchema, string pathData, string key)
            : base(name, pathData, key)
        {
            this.PathSchema = pathSchema;
        }

        public StiXmlDatabase(string name, string pathSchema, string pathData, string key, StiXmlType xmlType)
            : base(name, pathData, key)
        {
            this.PathSchema = pathSchema;
            this.XmlType = xmlType;
        }
	}
}
