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

using System.ComponentModel;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base;

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
	/// Describes the Data Source realizing access to Csv files.
	/// </summary>
	public abstract class StiFileDataSource : StiDataStoreSource
    {
        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiFileDataSource;
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
                propHelper.CodePage()
            };
            objHelper.Add(StiPropertyCategories.Data, list);

            return objHelper;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a path to the data file.
        /// </summary>
        [StiSerializable]
		[StiCategory("Data")]
		[DefaultValue("")]
        [Description("Gets or sets a path to the data file.")]
		public string Path
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
        /// Gets or sets a code page.
        /// </summary>
		[StiSerializable]
		[StiCategory("Data")]
		[DefaultValue("")]
        [Description("Gets or sets a code page.")]
		public int CodePage { get; set; }
        #endregion

	    /// <summary>
		/// Creates a new object of the type StiCsvSource.
		/// </summary>
        public StiFileDataSource() : this("", "", "", 0, null)
		{
		}

		public StiFileDataSource(string path, string name, string alias, int codePage) : 
			this(path, name, alias, codePage, null)
		{
		}

        public StiFileDataSource(string path, string name, string alias, int codePage, string key) :
            base(name, name, alias, key)
        {
            this.Path = path;
            this.CodePage = codePage;
        }
	}
}

