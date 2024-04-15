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
using System.Threading.Tasks;
using Stimulsoft.Base.Plans;
using Stimulsoft.Base.Exceptions;
using static Stimulsoft.Report.Export.PdfFonts.StiOpenTypeHelper.Cff;
using System.IO;
using Stimulsoft.Base.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dictionary
{
    [TypeConverter(typeof(StiCsvDatabaseConverter))]
    public class StiCsvDatabase : StiFileDatabase
    {
        #region Methods.override
        public override StiDatabase CreateNew()
        {
            return new StiCsvDatabase();
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiCsvDatabase;
        #endregion

        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiCsvDatabase
            jObject.AddPropertyStringNullOrEmpty("Separator", this.Separator);
            jObject.AddPropertyInt("CodePage", this.CodePage, 0);

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

                    case "CodePage":
                        this.CodePage = property.DeserializeInt();
                        break;
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a list separator.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [DefaultValue(null)]
        [Description("Gets or sets a list separator.")]
        public string Separator { get; set; }

        /// <summary>
        /// Gets or sets a code page.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [DefaultValue(0)]
        [Description("Gets or sets a code page.")]
        public int CodePage { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Returns new data connector for this database.
        /// </summary>
        /// <returns>Created connector.</returns>
        public override StiDataConnector CreateConnector(string connectionString = null)
        {
            return StiCsvConnector.Get();
        }

        public override DialogResult Edit(StiDictionary dictionary, bool newDatabase)
        {
            return StiDataEditorsHelper.Get().CsvDatabaseEdit(this, dictionary, newDatabase);
        }

        public override Task<DialogResult> EditAsync(StiDictionary dictionary, bool newDatabase)
        {
            var dialogResult = StiDataEditorsHelper.Get().CsvDatabaseEdit(this, dictionary, newDatabase);

            return Task<DialogResult>.Factory.StartNew(() => dialogResult);
        }

        protected override StiFileDataOptions GetConnectorOptions(StiReport report, bool isSchema)
        {
            StiCsvOptions options = null;
            var path = ParsePath(PathData, report);
            if (Directory.Exists(path))
            {
                options = new StiCsvOptions(path, CodePage, Separator);
            }
            else
            {
                var data = StiUniversalDataLoader.LoadSingle(report, path);
                if (data != null && data.Array != null)
                    options = new StiCsvOptions(data.Array, data.Name, CodePage, Separator);
            }

            if (options != null)
            {
#if CLOUD
                options.MaxDataRows = StiCloudReport.GetMaxDataRows(report?.ReportGuid);
#else
                options.MaxDataRows = 0;
#endif

                if (isSchema) options.MaxDataRows = 1;
            }
            return options;
        }
        #endregion

        public StiCsvDatabase()
        {
        }

        public StiCsvDatabase(string name, string pathData)
            : this(name, pathData, 0, null)
        {
        }

        public StiCsvDatabase(string name, string pathData, int codePage, string separator)
            : base(name, pathData)
        {
            this.CodePage = codePage;
            this.Separator = separator;
        }

        public StiCsvDatabase(string name, string pathData, int codePage, string separator, string key)
            : base(name, pathData, key)
        {
            this.CodePage = codePage;
            this.Separator = separator;
        }
    }
}
