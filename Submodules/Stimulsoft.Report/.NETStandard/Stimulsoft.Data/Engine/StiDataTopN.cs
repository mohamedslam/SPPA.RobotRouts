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
using Stimulsoft.Data.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Data.Engine
{
    [TypeConverter(typeof(StiDataTopNConverter))]
    public class StiDataTopN : 
        IStiJsonReportObject, 
        ICloneable
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyEnum("Mode", this.Mode, StiDataTopNMode.None);
            jObject.AddPropertyInt("Count", Count, 5);
            jObject.AddPropertyBool("ShowOthers", ShowOthers, true);
            jObject.AddPropertyString("OthersText", OthersText, "");
            jObject.AddPropertyString("MeasureField", MeasureField, "");

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Mode":
                        var value = property.DeserializeString();  // fix 'null' enum value in template
                        if (value != null)
                            this.Mode = (StiDataTopNMode)Enum.Parse(typeof(StiDataTopNMode), value);
                        break;

                    case "Count":
                        this.Count = property.DeserializeInt();
                        break;

                    case "ShowOthers":
                        this.ShowOthers = property.DeserializeBool();
                        break;

                    case "OthersText":
                        this.OthersText = property.DeserializeString();
                        break;

                    case "MeasureField":
                        this.MeasureField = property.DeserializeString();
                        break;
                }
            }
        }

        internal static StiDataTopN CreateFromJsonObject(JObject jObject)
        {
            var topN = new StiDataTopN();

            topN.LoadFromJsonObject(jObject);

            return topN;
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return this.MemberwiseClone() as StiDataTopN;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets output values mode.
        /// </summary>
        [DefaultValue(StiDataTopNMode.None)]
        [StiSerializable]
        [StiCategory("Common")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets output values mode.")]
        [RefreshProperties(RefreshProperties.All)]
        public StiDataTopNMode Mode { get; set; } = StiDataTopNMode.None;

        /// <summary>
        /// Gets or sets the number of output values.
        /// </summary>
        [DefaultValue(5)]
        [StiSerializable]
        [StiCategory("Common")]
        [Description("Gets or sets the number of output values.")]
        [RefreshProperties(RefreshProperties.All)]
        public int Count { get; set; } = 5;

        /// <summary>
        /// Gets or sets value which indicates whether to display other values.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("Common")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates whether to display other values.")]
        [RefreshProperties(RefreshProperties.All)]
        public bool ShowOthers { get; set; } = true;

        /// <summary>
        /// Gets or sets signature for other values.
        /// </summary>
        [StiSerializable]
        [StiCategory("Common")]
        [DefaultValue("")]
        [Description("Gets or sets signature for other values.")]
        [RefreshProperties(RefreshProperties.All)]
        public string OthersText { get; set; } = "";

        /// <summary>
        /// Gets or sets meausure field name
        /// </summary>
        [StiSerializable]
        [DefaultValue("")]
        [Description("Gets or sets a measure field name.")]
        [Browsable(false)]
        public string MeasureField { get; set; } = "";

        [Browsable(false)]
        public bool IsDefault => Mode == StiDataTopNMode.None && Count == 5 && ShowOthers && OthersText == "" && MeasureField == "";
        #endregion

        #region Methods.Override
        public override string ToString()
        {
            if (Mode == StiDataTopNMode.None)
                return Loc.GetEnum("SelectionModeNone");
            else
                return $"{(Mode == StiDataTopNMode.Bottom ? Loc.GetMain("Bottom") : Loc.GetMain("Top"))} {Count}{(!string.IsNullOrEmpty(MeasureField) ? $" [{MeasureField}]": "" ) }";
        }
        #endregion

        #region Methods
        public int GetUniqueCode()
        {
            unchecked
            {
                var hashCode = (int)Mode;
                hashCode = (hashCode * 397) ^ Count;
                hashCode = (hashCode * 397) ^ ShowOthers.GetHashCode();
                hashCode = (hashCode * 397) ^ (OthersText != null ? OthersText.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (MeasureField != null ? MeasureField.GetHashCode() : 0);
                return hashCode;
            }
        }
        #endregion

        public StiDataTopN()
        {
        }

        public StiDataTopN(StiDataTopNMode mode, int count, bool showOthers, string othersText, string measureField)
        {
            this.Mode = mode;
            this.Count = count;
            this.ShowOthers = showOthers;
            this.OthersText = othersText;
            this.MeasureField = measureField;
        }
    }
}
