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
using Stimulsoft.Base.Plans;
using Stimulsoft.Base.Drawing;
using System.IO;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dictionary
{
    [TypeConverter(typeof(StiDBaseDatabaseConverter))]
    public class StiDBaseDatabase : StiFileDatabase
    {
        #region Methods.override
        public override StiDatabase CreateNew()
        {
            return new StiDBaseDatabase();
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiDBaseDatabase;
        #endregion

        #region Properties
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
            return StiDBaseConnector.Get();
        }

        public override DialogResult Edit(StiDictionary dictionary, bool newDatabase)
        {
            return StiDataEditorsHelper.Get().DBaseDatabaseEdit(this, dictionary, newDatabase);
        }
        
        public override Task<DialogResult> EditAsync(StiDictionary dictionary, bool newDatabase)
        {
            var dialogResult = StiDataEditorsHelper.Get().DBaseDatabaseEdit(this, dictionary, newDatabase);

            return Task<DialogResult>.Factory.StartNew(() => dialogResult);
        }

        protected override StiFileDataOptions GetConnectorOptions(StiReport report, bool isSchema)
        {
            StiDBaseOptions options = null;

            var path = ParsePath(PathData, report);
            if (Directory.Exists(path))
            {
                options = new StiDBaseOptions(path, CodePage);
            }
            else
            {
                var data = StiUniversalDataLoader.LoadSingle(report, path);
                if (data != null && data.Array != null)
                    options = new StiDBaseOptions(data.Array, data.Name, CodePage);
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

        public StiDBaseDatabase()
        {
        }

        public StiDBaseDatabase(string name, string pathData)
            : this(name, pathData, 0)
        {
        }

        public StiDBaseDatabase(string name, string pathData, int codePage)
            : base(name, pathData)
        {
            this.CodePage = codePage;
        }

        public StiDBaseDatabase(string name, string pathData, int codePage, string key)
            : base(name, pathData, key)
        {
            this.CodePage = codePage;
        }
    }
}
