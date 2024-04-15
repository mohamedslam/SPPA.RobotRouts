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
using Stimulsoft.Base.Json.Linq;


namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
	/// Describes the Data Source realizing access to dBase files.
	/// </summary>
	[TypeConverter(typeof(Stimulsoft.Report.Dictionary.Design.StiDBaseSourceConverter))]
	public class StiDBaseSource : StiFileDataSource
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiDBaseSource
            jObject.AddPropertyStringNullOrEmpty("Path", Path);
            jObject.AddPropertyInt("CodePage", CodePage);

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
                }
            }
        }
        #endregion

        #region Properties
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiDBaseSource;
            }
        }
        #endregion

        #region Methods
	    protected override Type GetDataAdapterType()
	    {
	        return typeof(StiDBaseAdapterService);
	    }

        public override StiDataSource CreateNew()
        {
            return new StiDBaseSource();
        }
        #endregion

		/// <summary>
		/// Creates a new object of the type StiDBaseSource.
		/// </summary>
		public StiDBaseSource() : this("", "", "", 0)
		{
		}

		public StiDBaseSource(string path, string name, string alias, int codePage) : 
			base(path, name, alias, codePage)
		{
		}

        public StiDBaseSource(string path, string name, string alias, int codePage, string key) :
            base(path, name, alias, codePage, key)
        {
        }
	}
}