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
using System;

namespace Stimulsoft.Report.Components
{
    [TypeConverter(typeof(Stimulsoft.Report.Components.Design.StiDrillDownParameterConverter))]
    [RefreshProperties(RefreshProperties.All)]
	public class StiDrillDownParameter : 
        IStiDefault, 
        IStiJsonReportObject,
        ICloneable
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyStringNullOrEmpty("Name", Name);
            jObject.AddPropertyJObject("Expression", Expression.SaveToJsonObject(mode));

            if (jObject.Count == 0)
                return null;

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

        #region ICloneable
        public object Clone()
        {
            var parameter = this.MemberwiseClone() as StiDrillDownParameter;
            parameter.Expression = this.Expression.Clone() as StiExpression;
            return parameter;
        }
        #endregion

        #region IStiDefault
        [Browsable(false)]
		public bool IsDefault => string.IsNullOrEmpty(Name) && (Expression == null || Expression.Value.Length == 0);
        #endregion

		#region Properties
        [DefaultValue("")]
		[StiSerializable]
		[StiOrder(100)]
		[RefreshProperties(RefreshProperties.All)]
        [Description("Gets or sets name of drill-down parameter.")]
		public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets expression to fill drill-down parameter.
        /// </summary>
		[StiSerializable]
		[StiOrder(200)]
		[RefreshProperties(RefreshProperties.All)]
        [Description("Gets or sets expression to fill drill-down parameter.")]
		public StiExpression Expression { get; set; } = new StiExpression();
        #endregion

        [StiUniversalConstructor("")]
        public StiDrillDownParameter()
        {
        }

        public StiDrillDownParameter(string name)
        {
            this.Name = name;
        }
    }
}
