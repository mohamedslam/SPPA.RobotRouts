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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Dictionary.Design;
using System.Threading.Tasks;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Helpers;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dictionary
{
    [TypeConverter(typeof(StiGisDatabaseConverter))]
    public class StiGisDatabase : StiFileDatabase
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyStringNullOrEmpty("Separator", this.Separator);
            jObject.AddPropertyEnum("DataType", DataType, StiGisDataType.Wkt);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Separator":
                        this.Separator = property.DeserializeString();
                        break;

                    case "DataType":
                        this.DataType = property.DeserializeEnum<StiGisDataType>();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiGisDatabase;
        #endregion

        #region Properties
        [StiSerializable]
        [StiCategory("Data")]
        [Description("Gets or sets a list separator.")]
        [DefaultValue("|")]
        [Browsable(false)]
        public string Separator { get; set; } = "|";

        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [DefaultValue(StiGisDataType.Wkt)]
        [Browsable(false)]
        public StiGisDataType DataType { get; set; } = StiGisDataType.Wkt;
        #endregion

        #region Methods
        public override StiDatabase CreateNew()
        {
            return new StiGisDatabase();
        }

        /// <summary>
        /// Returns new data connector for this database.
        /// </summary>
        /// <returns>Created connector.</returns>
        public override StiDataConnector CreateConnector(string connectionString = null)
        {
            return StiGisConnector.Get();
        }

        public override DialogResult Edit(StiDictionary dictionary, bool newDatabase)
		{
            return StiDataEditorsHelper.Get().GisDatabaseEdit(this, dictionary, newDatabase);
		}

        public override Task<DialogResult> EditAsync(StiDictionary dictionary, bool newDatabase)
        {
            var dialogResult = StiDataEditorsHelper.Get().GisDatabaseEdit(this, dictionary, newDatabase);

            return Task<DialogResult>.Factory.StartNew(() => dialogResult);
        }

        protected override StiFileDataOptions GetConnectorOptions(StiReport report, bool isSchema)
        {
            var data = StiUniversalDataLoader.LoadSingle(report, ParsePath(PathData, report));
            if (data != null && data.Array != null)
                return new StiGisOptions(data.Array, Separator);

            return null;
        }
        #endregion

        public StiGisDatabase()
		{
		}

		public StiGisDatabase(string name, string pathData) : base(name, pathData)
		{
			this.PathData = pathData;
		}

        public StiGisDatabase(string name, string pathData, string key)
            : base(name, pathData, key)
        {
            this.PathData = pathData;
        }

        public StiGisDatabase(string name, string pathData, string key, StiGisDataType dataType)
            : base(name, pathData, key)
        {
            this.PathData = pathData;
            this.DataType = dataType;
        }
    }
}
