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
using Stimulsoft.Report.Design;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Drawing;
using Stimulsoft.Base.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report
{
    [TypeConverter(typeof(StiUniversalConverter))]
    public class StiHeatmapWithGroupStyleData : 
        IStiJsonReportObject,
        ICloneable
    {
        #region enum Order
        public enum Order
        {
            Colors = 1,
            ZeroColor = 2,
            Mode = 3
        }
        #endregion

        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyArrayColor("Colors", Colors);
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
                    case "Colors":
                        this.Colors = property.DeserializeArrayColor();
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

        #region consts
        private Color[] DefaultColors =
        {
            ColorTranslator.FromHtml("#70ad47"),
            ColorTranslator.FromHtml("#ffc000")
        };
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
        /// The property allows the creation of a collection of colors. Each color of the collection will be used as the primary color of the specific group.
        /// </summary>
        [StiSerializable]
        [StiCategory("Common")]
        [TypeConverter(typeof(StiStyleColorsConverter))]
        [Editor("Stimulsoft.Report.Components.Design.StiColorsCollectionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("The property allows the creation of a collection of colors. Each color of the collection will be used as the primary color of the specific group.")]
        [StiOrder((int)Order.Colors)]
        public Color[] Colors { get; set; }

        private bool ShouldSerializeColor()
        {
            return Colors == null ||
                Colors.Length != DefaultColors.Length ||
                !Colors.ToList().SequenceEqual(DefaultColors);
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
        public bool IsDefault()
        {
            return 
                !ShouldSerializeColor() &&
                !ShouldSerializeZeroColor() &&
                !ShouldSerializeMode();
        }
        #endregion

        public StiHeatmapWithGroupStyleData()
        {
            this.Colors = DefaultColors.Clone() as Color[];
        }

        [StiUniversalConstructor("HeatmapWithGroup")]
        public StiHeatmapWithGroupStyleData(Color[] colors, Color zeroColor, StiHeatmapFillMode mode)
        {
            this.Colors = colors;
            this.ZeroColor = zeroColor;
            this.Mode = mode;
        }
    }
}