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
using Stimulsoft.Report.Helpers;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Dictionary.Design;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Linq;
using static Stimulsoft.Base.StiDataLoaderHelper;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dictionary
{
    [TypeConverter(typeof(StiJsonDatabaseConverter))]
    public class StiJsonDatabase : StiFileDatabase
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);
            
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
                    case "RelationDirection":
                        this.RelationDirection = property.DeserializeEnum<StiRelationDirection>();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiJsonDatabase;
        #endregion

        #region Properties
        private StiRelationDirection relationDirection = StiRelationDirection.ParentToChild;

        [StiSerializable]
        [Description("Specifies direction of the relation processing.")]
        [TypeConverter(typeof(StiEnumConverter))]
        [DefaultValue(StiRelationDirection.ParentToChild)]
        [Browsable(false)]
        public StiRelationDirection RelationDirection
        {
            get
            {
                return relationDirection;
            }
            set
            {
                relationDirection = value;
                DataSchema = null;
            }
        }

        /// <summary>
        /// List of headers used for http requests to load data.
        /// </summary>
        [Description("List of headers used for http requests to load data.")]
        public NameValueCollection Headers { get; protected set; } = new NameValueCollection();
        #endregion

        #region Methods
        public override StiDatabase CreateNew()
        {
            return new StiJsonDatabase();
        }

        /// <summary>
        /// Returns new data connector for this database.
        /// </summary>
        /// <returns>Created connector.</returns>
        public override StiDataConnector CreateConnector(string connectionString = null)
        {
            return StiJsonConnector.Get();
        }

        public override DialogResult Edit(StiDictionary dictionary, bool newDatabase)
		{
            return StiDataEditorsHelper.Get().JsonDatabaseEdit(this, dictionary, newDatabase);
		}

        public override Task<DialogResult> EditAsync(StiDictionary dictionary, bool newDatabase)
        {
            var dialogResult = StiDataEditorsHelper.Get().JsonDatabaseEdit(this, dictionary, newDatabase);

            return Task<DialogResult>.Factory.StartNew(() => dialogResult);
        }

        protected override StiFileDataOptions GetConnectorOptions(StiReport report, bool isShema)
        {
            var data = StiUniversalDataLoader.LoadSingle(report, ParsePath(PathData, report));
            if (data != null && data.Array != null)
                return new StiJsonOptions(data.Array, RelationDirection);

            return null;
        }
        #endregion

        public StiJsonDatabase()
		{
		}

		public StiJsonDatabase(string name, string pathData) : base(name, pathData)
		{
			this.PathData = pathData;
		}

        public StiJsonDatabase(string name, string pathData, string key)
            : base(name, pathData, key)
        {
            this.PathData = pathData;
        }
	}
}
