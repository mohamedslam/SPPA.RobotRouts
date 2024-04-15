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
using Stimulsoft.Report.Components;
using System;
using System.ComponentModel;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(StiUniversalConverter))]
    public class StiAxisRange :
        IStiAxisRange,
        ICloneable
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyDouble(nameof(Minimum), Minimum, 0d);
            jObject.AddPropertyDouble(nameof(Maximum), Maximum, 0d);
            jObject.AddPropertyBool(nameof(Auto), Auto, true);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Minimum):
                        this.Minimum = property.DeserializeDouble();
                        break;

                    case nameof(Maximum):
                        this.Maximum = property.DeserializeDouble();
                        break;

                    case nameof(Auto):
                        this.Auto = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region IStiDefault
        [Browsable(false)]
        public bool IsDefault => Minimum == 0d && Maximum == 0d && Auto;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets minimum value of axis range.
        /// </summary>
		[DefaultValue(0d)]
        [StiSerializable]
        [Description("Gets or sets minimum value of axis range.")]
        [StiOrder(StiPropertyOrder.RangeMinimum)]
        public double Minimum { get; set; }

        /// <summary>
        /// Gets or sets maximum value of axis range.
        /// </summary>
		[DefaultValue(0d)]
        [StiSerializable]
        [Description("Gets or sets maximum value of axis range.")]
        [StiOrder(StiPropertyOrder.RangeMaximum)]
        public double Maximum { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that minimum and maximum values will be calculated automatically.
        /// </summary>
		[StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that minimum and maximum values will be calculated automatically.")]
        [StiOrder(StiPropertyOrder.RangeAuto)]
        public bool Auto { get; set; } = true;
        #endregion
                
        public StiAxisRange()
        {
        }

        [StiUniversalConstructor("Range")]
        public StiAxisRange(
            bool auto,
            double minimum,
            double maximum
            )
        {
            this.Auto = auto;
            this.Minimum = minimum;
            this.Maximum = maximum;
        }
    }
}
