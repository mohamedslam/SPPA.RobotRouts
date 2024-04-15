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
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.SignatureFonts;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components
{
    [TypeConverter(typeof(StiUniversalConverter))]
    public class StiSignatureType :
        IStiSignatureType
    {
        #region enum Order
        public enum Order
        {
            FullName = 1,
            Initials = 2,
            Style = 3,
            CustomFont = 4
        }
        #endregion

        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyStringNullOrEmpty(nameof(FullName), FullName);
            jObject.AddPropertyStringNullOrEmpty(nameof(Initials), Initials);
            jObject.AddPropertyEnum(nameof(Style), Style, StiSignatureStyle.Style1);
            jObject.AddPropertyStringNullOrEmpty(nameof(CustomFont), CustomFont);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(FullName):
                        this.FullName = property.DeserializeString();
                        break;

                    case nameof(Initials):
                        this.Initials = property.DeserializeString();
                        break;

                    case nameof(Style):
                        this.Style = property.DeserializeEnum<StiSignatureStyle>();
                        break;

                    case nameof(CustomFont):
                        this.CustomFont = property.DeserializeString();
                        break;
                }
            }
        }

        internal static StiSignatureType CreateFromJsonObject(JObject jObject)
        {
            var title = new StiSignatureType();
            title.LoadFromJsonObject(jObject);

            return title;
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone() => this.MemberwiseClone();
        #endregion

        #region IStiSignatureSelectStyle
        [StiSerializable]
        [StiCategory("Common")]
        [StiPropertyLevel(StiLevel.Basic)]
        [StiOrder((int)Order.FullName)]
        public string FullName { get; set; } = string.Empty;

        private bool ShouldSerializeFullName()
        {
            return FullName != string.Empty;
        }


        [StiSerializable]
        [StiCategory("Common")]
        [StiPropertyLevel(StiLevel.Basic)]
        [StiOrder((int)Order.Initials)]
        public string Initials { get; set; } = string.Empty;

        private bool ShouldSerializeInitials()
        {
            return Initials != string.Empty;
        }


        [StiSerializable]
        [DefaultValue(StiSignatureStyle.Style1)]
        [StiCategory("Common")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        [StiOrder((int)Order.Style)]
        public StiSignatureStyle Style { get; set; } = StiSignatureStyle.Style1;

        private bool ShouldSerializeStyle()
        {
            return Style != StiSignatureStyle.Style1;
        }


        [Browsable(false)]
        [StiSerializable]
        [StiCategory("Common")]
        [StiPropertyLevel(StiLevel.Basic)]
        [StiOrder((int)Order.CustomFont)]
        public string CustomFont { get; set; } = "Arial";

        private bool ShouldSerializeCustomFont()
        {
            return CustomFont != "Arial";
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public virtual bool IsDefault =>
            !ShouldSerializeFullName() &&
            !ShouldSerializeInitials() &&
            !ShouldSerializeStyle() &&
            !ShouldSerializeCustomFont();
        #endregion

        public StiSignatureType()
        {
        }

        [StiUniversalConstructor("Type")]
        public StiSignatureType(string fullName, string initials, StiSignatureStyle style)
        {
            this.FullName = fullName;
            this.Initials = initials;
            this.Style = style;
        }
    }
}