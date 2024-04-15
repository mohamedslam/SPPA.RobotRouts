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
using Stimulsoft.Report.Chart.Design;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(StiSeriesTopNConverter))]
    public class StiSeriesTopN :
        IStiSeriesTopN,
        IStiSerializeToCodeAsClass,
        IStiPropertyGridObject
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyEnum("Mode", this.Mode, StiTopNMode.None);
            jObject.AddPropertyInt("Count", Count, 5);
            jObject.AddPropertyBool("ShowOthers", ShowOthers, true);
            jObject.AddPropertyString("OthersText", OthersText, "Others");

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Mode":
                        this.Mode = property.DeserializeEnum<StiTopNMode>();
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
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public StiComponentId ComponentId => StiComponentId.StiSeriesTopN;

        [Browsable(false)]
        public string PropName => string.Empty;

        public StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var objHelper = new StiPropertyCollection();
            var propHelper = propertyGrid.PropertiesHelper;

            var list = new[] 
            { 
                propHelper.ChartTopN()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }

        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return null;
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return this.MemberwiseClone() as IStiSeriesTopN;
        }
        #endregion

        #region IStiDefault
        [Browsable(false)]
        public bool IsDefault
        {
            get
            {
                return
                    Mode == StiTopNMode.None
                    && Count == 5
                    && ShowOthers
                    && OthersText == "Others";
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets output values mode.
        /// </summary>
        [DefaultValue(StiTopNMode.None)]
        [StiSerializable]
        [StiCategory("Common")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets output values mode.")]
        public StiTopNMode Mode { get; set; } = StiTopNMode.None;

        /// <summary>
        /// Gets or sets the number of output values.
        /// </summary>
        [DefaultValue(5)]
        [StiSerializable]
        [StiCategory("Common")]
        [Description("Gets or sets the number of output values.")]
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
        public bool ShowOthers { get; set; } = true;

        /// <summary>
        /// Gets or sets signature for other values.
        /// </summary>
        [StiSerializable]
        [StiCategory("Common")]
        [DefaultValue("Others")]
        [Description("Gets or sets signature for other values.")]
        public string OthersText { get; set; } = "Others";
        #endregion
    }
}
