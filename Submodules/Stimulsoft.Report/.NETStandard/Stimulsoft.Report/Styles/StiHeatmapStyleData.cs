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
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing;
using Stimulsoft.Base.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report
{
    [TypeConverter(typeof(StiUniversalConverter))]
    public class StiHeatmapStyleData : 
        IStiJsonReportObject,
        ICloneable
    {
        #region enum Order
        public enum Order
        {
            Color = 1,
            ZeroColor = 2,
            Mode = 3,
        }
        #endregion

        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyColor("Color", Color, "#70ad47");
            jObject.AddPropertyColor("ZeroColor", ZeroColor, Color.Transparent);
            jObject.AddPropertyEnum("Mode", Mode, StiHeatmapFillMode.Lightness);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Color":
                        this.Color = property.DeserializeColor();
                        break;

                    case "ZeroColor":
                        this.ZeroColor = property.DeserializeColor();
                        break;

                    case "Mode":
                        Mode = property.DeserializeEnum<StiHeatmapFillMode>();
                        break;
                }
            }
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone() => this.MemberwiseClone();
        #endregion

        #region Properties
        /// <summary>
        /// This property defines the color used to fill the geographic object Map, which contains the max value.
        /// </summary>
        [StiSerializable]
        [StiCategory("Common")]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(StiColorConverter))]
        [Description("This property defines the color used to fill the geographic object Map, which contains the max value.")]
        [StiOrder((int)Order.Color)]
        public Color Color { get; set; } = ColorTranslator.FromHtml("#70ad47");

        private bool ShouldSerializeColor()
        {
            return Color != ColorTranslator.FromHtml("#70ad47");
        }


        /// <summary>
        /// This property defines the color used to fill the geographic object Map, which contains the zero value.
        /// </summary>
        [StiSerializable]
        [StiCategory("Common")]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(StiColorConverter))]
        [Description("This property defines the color used to fill the geographic object Map, which contains the zero value.")]
        [StiOrder((int)Order.ZeroColor)]
        public Color ZeroColor { get; set; } = Color.Transparent;

        private bool ShouldSerializeZeroColor()
        {
            return ZeroColor != Color.Transparent;
        }


        /// <summary>
        /// The property defines the mode which will be used to form the colors collection of the heatmap.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiHeatmapFillMode.Lightness)]
        [StiCategory("Common")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("The property defines the mode which will be used to form the colors collection of the heatmap.")]
        [StiOrder((int)Order.Mode)]
        public StiHeatmapFillMode Mode { get; set; } = StiHeatmapFillMode.Lightness;

        private bool ShouldSerializeMode()
        {
            return Mode != StiHeatmapFillMode.Lightness;
        }
        #endregion

        #region Methods
        public virtual bool IsDefault()
        {
            return this.Color == ColorTranslator.FromHtml("#70ad47") &&
                ZeroColor == Color.Transparent &&
                Mode == StiHeatmapFillMode.Lightness;
        }
        #endregion

        public StiHeatmapStyleData()
        {
        }

        [StiUniversalConstructor("Heatmap")]
        public StiHeatmapStyleData(Color color, Color zeroColor, StiHeatmapFillMode mode)
        {
            this.Color = color;
            this.ZeroColor = zeroColor;
            this.Mode = mode;
        }
    }
}