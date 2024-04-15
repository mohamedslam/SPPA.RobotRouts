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
using Stimulsoft.Report.Dictionary.Design;
using Stimulsoft.Report.Helpers;
using System.Threading.Tasks;
using System.Linq;
using Stimulsoft.Base.Json.Linq;
using static Stimulsoft.Base.StiDataLoaderHelper;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dictionary
{
    [TypeConverter(typeof(StiExcelDatabaseConverter))]
    public class StiExcelDatabase : StiFileDatabase
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyBool("FirstRowIsHeader", FirstRowIsHeader, true);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "FirstRowIsHeader":
                        this.FirstRowIsHeader = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiExcelDatabase;
        #endregion

        #region Properties
        [StiSerializable]
        [StiOrder((int)Order.FirstRowIsHeader)]
        [DefaultValue(true)]
        public bool FirstRowIsHeader { get; set; }
        #endregion

        #region Methods
        public override StiDatabase CreateNew()
        {
            return new StiExcelDatabase();
        }

        /// <summary>
        /// Returns new data connector for this database.
        /// </summary>
        /// <returns>Created connector.</returns>
        public override StiDataConnector CreateConnector(string connectionString = null)
        {
            return StiExcelConnector.Get();
        }

        public override DialogResult Edit(StiDictionary dictionary, bool newDatabase)
        {
            return StiDataEditorsHelper.Get().ExcelDatabaseEdit(this, dictionary, newDatabase);
        }

        public override Task<DialogResult> EditAsync(StiDictionary dictionary, bool newDatabase)
        {
            var dialogResult = StiDataEditorsHelper.Get().ExcelDatabaseEdit(this, dictionary, newDatabase);

            return Task<DialogResult>.Factory.StartNew(() => dialogResult);
        }

        protected override StiFileDataOptions GetConnectorOptions(StiReport report, bool isSchema)
        {
            var data = StiUniversalDataLoader.LoadSingle(report, ParsePath(PathData, report));
            if (data != null && data.Array != null)
                return new StiExcelOptions(data.Array, FirstRowIsHeader);

            return null;
        }
        #endregion

        public StiExcelDatabase()
        {
            this.FirstRowIsHeader = true;
        }

        public StiExcelDatabase(string name, string pathData) : base(name, pathData)
        {
            this.PathData = pathData;
            this.FirstRowIsHeader = true;
        }

        public StiExcelDatabase(string name, string pathData, string key)
            : base(name, pathData, key)
        {
            this.PathData = pathData;
            this.FirstRowIsHeader = true;
        }

        public StiExcelDatabase(string name, string pathData, string key, bool firstRowIsHeader)
            : base(name, pathData, key)
        {
            this.PathData = pathData;
            this.FirstRowIsHeader = firstRowIsHeader;
        }
    }
}