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
using System.Drawing.Design;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Components;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(StiUniversalConverter))]
    public class StiChartTableHeader :
        IStiChartTableHeader,
        IStiFont
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyStringNullOrEmpty("TextAfter", TextAfter);
            jObject.AddPropertyBrush("Brush", Brush);
            jObject.AddPropertyFontArial8("Font", Font);
            jObject.AddPropertyColor("TextColor", TextColor, Color.DarkGray);
            jObject.AddPropertyBool("WordWrap", WordWrap);
            jObject.AddPropertyStringNullOrEmpty("Format", Format);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "TextAfter":
                        this.TextAfter = property.DeserializeString();
                        break;

                    case "Brush":
                        this.Brush = property.DeserializeBrush();
                        break;

                    case "Font":
                        this.Font = property.DeserializeFont(Font);
                        break;

                    case "TextColor":
                        this.TextColor = property.DeserializeColor();
                        break;

                    case "WordWrap":
                        this.WordWrap = property.DeserializeBool();
                        break;

                    case "Format":
                        this.Format = property.DeserializeString();
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
            var header = this.MemberwiseClone() as IStiChartTableHeader;

            header.Brush = this.Brush.Clone() as StiBrush;
            header.Font = this.Font.Clone() as Font;

            return header;
        }
        #endregion

        #region IStiDefault
        [Browsable(false)]
        public bool IsDefault
        {
            get
            {
                return
                    (TextAfter != null && TextAfter.Length == 0)
                    && !ShouldSerializeBrush()
                    && !ShouldSerializeFont()
                    && !ShouldSerializeTextColor()
                    && string.IsNullOrEmpty(Format)
                    && !WordWrap;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets string which will be output after text.
        /// </summary>
		[DefaultValue("")]
        [StiSerializable]
        [Description("Gets or sets string which will be output after text.")]
        public string TextAfter { get; set; } = "";

        /// <summary>
        /// Gets os sets format string which is used for formating table header.
        /// </summary>
		[DefaultValue("")]
        [StiSerializable]
        [Editor("Stimulsoft.Report.Chart.Design.StiFormatEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets os sets format string which is used for formating table header.")]
        public string Format { get; set; } = "";

        /// <summary>
        /// Gets or sets brush which will be used to fill chart table header.
        /// </summary>
        [StiOrder(StiSeriesLabelsPropertyOrder.Brush)]
        [StiSerializable]
        [Description("Gets or sets brush which will be used to fill chart table header.")]
        public StiBrush Brush { get; set; } = new StiSolidBrush(Color.White);

        private bool ShouldSerializeBrush()
        {
            return !(Brush is StiSolidBrush && ((StiSolidBrush)Brush).Color == Color.White);
        }

        /// <summary>
        /// Gets or sets font which will be used to draw chart table header.
        /// </summary>
        [StiSerializable]
        [StiOrder(StiSeriesLabelsPropertyOrder.Font)]
        [Description("Gets or sets font which will be used to draw chart table header.")]
        [Editor(StiEditors.Font, typeof(UITypeEditor))]
        public Font Font { get; set; } = new Font("Arial", 8);

        private bool ShouldSerializeFont()
        {
            return !(Font != null && Font.Name == "Arial" && Font.SizeInPoints == 8 && Font.Style == FontStyle.Regular);
        }

        /// <summary>
        /// Gets or sets text color.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets text color.")]
        public Color TextColor { get; set; } = Color.DarkGray;

        private bool ShouldSerializeTextColor()
        {
            return TextColor != Color.DarkGray;
        }

        /// <summary>
        /// Gets or sets word wrap.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets word wrap.")]
        public bool WordWrap { get; set; }
        #endregion

        public StiChartTableHeader()
        {
        }

        public StiChartTableHeader(
            StiBrush brush,
            Font font,
            Color textColor,
            bool wordWrap) : this(
            string.Empty, brush, font, textColor, wordWrap)
        {
        }

        public StiChartTableHeader(
            string textAfter,
            StiBrush brush,
            Font font,
            Color textColor,
            bool wordWrap) :
            this(string.Empty, brush, font, textColor, wordWrap, string.Empty)
        {
        }

        [StiUniversalConstructor("Header")]
        public StiChartTableHeader(
            string textAfter,
            StiBrush brush,
            Font font,
            Color textColor,
            bool wordWrap,
            string format)
        {
            this.TextAfter = textAfter;
            this.Brush = brush;
            this.Font = font;
            this.TextColor = textColor;
            this.WordWrap = wordWrap;
            this.Format = format;
        }
    }
}
