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
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Dictionary.Design;
using Stimulsoft.Report.PropertyGrid;

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
	/// Describes the Data Source realizing access to Csv files.
	/// </summary>
	[TypeConverter(typeof(StiCsvSourceConverter))]
	public class StiCsvSource : StiFileDataSource
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiCsvSource
            jObject.AddPropertyStringNullOrEmpty("Path", Path);
            jObject.AddPropertyInt("CodePage", CodePage);
            jObject.AddPropertyStringNullOrEmpty("Separator", Separator);
            jObject.AddPropertyBool("ConvertEmptyStringToNull", ConvertEmptyStringToNull, true);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Path":
                        this.Path = property.DeserializeString();
                        break;

                    case "CodePage":
                        this.CodePage = property.DeserializeInt();
                        break;

                    case "Separator":
                        this.Separator = property.DeserializeString();
                        break;

                    case "ConvertEmptyStringToNull":
                        this.ConvertEmptyStringToNull = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiCsvSource;
            }
        }

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // DataCategory
            var list = new[]
            {
                propHelper.Name(),
                propHelper.Alias(),
                propHelper.NameInSource(),
                propHelper.ConnectOnStart(),
                propHelper.Path(),
                propHelper.CodePage(),
                propHelper.Separator(),
                propHelper.ConvertEmptyStringToNull()
            };
            objHelper.Add(StiPropertyCategories.Data, list);

            return objHelper;
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
        /// Gets or sets a value indicating whether it is necessary to convert empty string to null.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [DefaultValue(true)]
        [Description("Gets or sets a value indicating whether it is necessary to convert empty string to null.")]
        public bool ConvertEmptyStringToNull { get; set; }
        #endregion

        #region Methods
        protected override Type GetDataAdapterType()
        {
            return typeof(StiCsvAdapterService);
        }

        public override StiDataSource CreateNew()
        {
            return new StiCsvSource();
        }
        #endregion

	    /// <summary>
		/// Creates a new object of the type StiCsvSource.
		/// </summary>
		public StiCsvSource() : this("", "", "", 0, null)
		{
		}

		public StiCsvSource(string path, string name, string alias, int codePage) : 
			this(path, name, alias, codePage, null)
		{
		}

        public StiCsvSource(string path, string name, string alias, int codePage, string separator) :
            base(path, name, alias, codePage)
        {
            this.ConvertEmptyStringToNull = true;
            this.Separator = separator;
        }

        public StiCsvSource(string path, string name, string alias, int codePage, string separator, string key) :
            base(path, name, alias, codePage, key)
        {
            this.ConvertEmptyStringToNull = true;
            this.Separator = separator;
        }
	}
}

