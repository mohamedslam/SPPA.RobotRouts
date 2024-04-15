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
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Json.Linq;

namespace Stimulsoft.Report.Components
{
    [TypeConverter(typeof(Stimulsoft.Report.Components.Design.StiParameterConverter))]
    [RefreshProperties(RefreshProperties.All)]
	public class StiParameter : 
        IStiDefault,
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyStringNullOrEmpty("Name", Name);
            jObject.AddPropertyJObject("Expression", Expression.SaveToJsonObject(mode));

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Name":
                        this.Name = property.DeserializeString();
                        break;

                    case "Expression":
                        this.Expression.LoadFromJsonObject((JObject)property.Value);
                        break;
                }
            }
        }
        #endregion

        #region IStiDefault
        [Browsable(false)]
		public bool IsDefault => string.IsNullOrEmpty(Name) && (Expression == null || Expression.Value.Length == 0);
        #endregion

		#region Properties
        /// <summary>
        /// Gets or sets name of parameter.
        /// </summary>
        [DefaultValue("")]
		[StiSerializable]
		[StiOrder(100)]
		[RefreshProperties(RefreshProperties.All)]
        [Description("Gets or sets name of parameter.")]
		public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets expression to fill parameter.
        /// </summary>
		[StiSerializable]
		[StiOrder(200)]
		[RefreshProperties(RefreshProperties.All)]
        [Description("Gets or sets expression to fill parameter.")]
		public StiExpression Expression { get; set; } = new StiExpression();
        #endregion

        #region Methods
        public override string ToString()
        {
            return Name != null ? Name : GetType().ToString();
        }
        #endregion

        [StiUniversalConstructor("")]
        public StiParameter()
        {
        }
	}
}