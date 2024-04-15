#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	Stimulsoft.Report Library										}
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

using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Design;
using System.ComponentModel;

namespace Stimulsoft.Report
{
    [TypeConverter(typeof(StiGlobalizationItemConverter))]
	[StiSerializable]
    public sealed class StiGlobalizationItem : IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyStringNullOrEmpty("PropertyName", PropertyName);
            jObject.AddPropertyStringNullOrEmpty("Text", Text);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "PropertyName":
                        this.PropertyName = property.DeserializeString();
                        break;

                    case "Text":
                        this.Text = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region Properties
        [StiSerializable]
		[DefaultValue("")]
		public string PropertyName { get; set; } = "";

        [StiSerializable]
		[DefaultValue("")]
		public string Text 
        { 
            get; 
            set; 
        } = "";
        #endregion

        #region Methods.override
        public override string ToString() => PropertyName;
        #endregion

        public StiGlobalizationItem()
		{
		}

		public StiGlobalizationItem(string propertyName, string text)
		{			
			this.PropertyName = propertyName;
			this.Text = text;
		}
    }
}
