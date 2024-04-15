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

using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Serializing;
using System;
using System.ComponentModel;

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// The class describes the icon set items.
    /// </summary>
    [RefreshProperties(RefreshProperties.All)]
    [TypeConverter(typeof(Stimulsoft.Report.Components.Design.StiIconSetItemConverter))]
    public class StiIconSetItem : IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            // StiIconSetItem
            jObject.AddPropertyEnum("Icon", Icon, StiIcon.None);
            jObject.AddPropertyEnum("Operation", Operation, StiIconSetOperation.MoreThan);
            jObject.AddPropertyEnum("ValueType", ValueType, StiIconSetValueType.Percent);
            jObject.AddPropertyFloat("Value", Value, 0f);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Icon":
                        this.Icon = property.DeserializeEnum<StiIcon>();
                        break;

                    case "Operation":
                        this.Operation = property.DeserializeEnum<StiIconSetOperation>();
                        break;

                    case "ValueType":
                        this.ValueType = property.DeserializeEnum<StiIconSetValueType>();
                        break;

                    case "Value":
                        this.Value = property.DeserializeFloat();
                        break;
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets icon for current item of IconSets.
        /// </summary>
        [StiSerializable]
        public StiIcon Icon { get; set; } = StiIcon.None;

        /// <summary>
        /// Gets or sets type of operation for current item of IconSets.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiIconSetOperation.MoreThan)]
        public StiIconSetOperation Operation { get; set; } = StiIconSetOperation.MoreThan;

        /// <summary>
        /// Gets or sets type of value for current item of IconSets.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiIconSetValueType.Percent)]
        public StiIconSetValueType ValueType { get; set; } = StiIconSetValueType.Percent;

        /// <summary>
        /// Gets or sets value for current item of IconSets.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0f)]
        public float Value { get; set; }
        #endregion

        /// <summary>
        /// Creates a new object of the type StiIconSetItem.
		/// </summary>
        public StiIconSetItem()
        {
        }

        /// <summary>
        /// Creates a new object of the type StiIconSetItem.
		/// </summary>
        public StiIconSetItem(StiIcon icon, StiIconSetOperation operation, StiIconSetValueType valueType, float value)
		{
            this.Icon = icon;
            this.Operation = operation;
            this.ValueType = valueType;
            this.Value = value;
		}
    }
}
