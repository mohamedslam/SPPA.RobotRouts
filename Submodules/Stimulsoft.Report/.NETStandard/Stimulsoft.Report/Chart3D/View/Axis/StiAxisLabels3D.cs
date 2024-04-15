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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.TextFormats;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(StiUniversalConverter))]
    public class StiAxisLabels3D :
        IStiAxisLabels3D,
        IStiFont
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyBool(nameof(AllowApplyStyle), AllowApplyStyle, true);
            jObject.AddPropertyStringNullOrEmpty(nameof(Format), Format);
            jObject.AddPropertyFontTahoma8(nameof(Font), Font);
            jObject.AddPropertyColor(nameof(Color), Color, Color.Black);
            jObject.AddPropertyStringNullOrEmpty(nameof(TextBefore), TextBefore);
            jObject.AddPropertyStringNullOrEmpty(nameof(TextAfter), TextAfter);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(AllowApplyStyle):
                        this.AllowApplyStyle = property.DeserializeBool();
                        break;

                    case nameof(Format):
                        this.Format = property.DeserializeString();
                        break;

                    case nameof(Font):
                        this.Font = property.DeserializeFont(Font);
                        break;

                    case nameof(Color):
                        this.Color = property.DeserializeColor();
                        break;

                    case nameof(TextBefore):
                        this.TextBefore = property.DeserializeString();
                        break;

                    case nameof(TextAfter):
                        this.TextAfter = property.DeserializeString();
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
            var labels = this.MemberwiseClone() as IStiAxisLabels;

            labels.Font = this.Font.Clone() as Font;

            if (this.Core != null)
            {
                labels.Core = this.Core.Clone() as StiAxisLabelsCoreXF;
                labels.Core.Labels = labels;
            }

            return labels;
        }
        #endregion

        #region IStiDefault
        [Browsable(false)]
        public bool IsDefault
        {
            get
            {
                //Color is not taken into consideration because AllowApplyStyle.
                return
                    AllowApplyStyle
                    && (Format != null && Format.Length == 0)                    
                    && !ShouldSerializeFont()
                    && (TextAfter != null && TextAfter.Length == 0)
                    && (TextBefore != null && TextBefore.Length == 0);
            }
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public StiAxisLabelsCoreXF3D Core { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that chart style will be used.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Description("Gets or sets value which indicates that chart style will be used.")]
        [DefaultValue(true)]
        public bool AllowApplyStyle { get; set; } = true;

        /// <summary>
        /// Gets os sets format string which is used for formating argument values.
        /// </summary>
		[DefaultValue("")]
        [StiSerializable]
        [Editor("Stimulsoft.Report.Chart.Design.StiFormatEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets os sets format string which is used for formating argument values.")]
        public string Format { get; set; } = "";

        /// <summary>
        /// Gets or sets font which will be used for axis label drawing.
        /// </summary>
		[StiSerializable]
        [Editor("Stimulsoft.Report.Design.Components.StiFontEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets font which will be used for axis label drawing.")]
        public Font Font { get; set; } = new Font("Tahoma", 8);

        private bool ShouldSerializeFont()
        {
            return !(Font != null && Font.Name == "Tahoma" && Font.SizeInPoints == 8 && Font.Style == FontStyle.Regular);
        }

        /// <summary>
        /// Gets or sets color of labels drawing.
        /// </summary>
		[StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets color of labels drawing.")]
        public Color Color { get; set; } = Color.Black;

        private bool ShouldSerializeColor()
        {
            return Color != Color.Black;
        }

        /// <summary>
        /// Gets or sets string which will be output before argument string representation.
        /// </summary>
		[DefaultValue("")]
        [StiSerializable]
        [Description("Gets or sets string which will be output before argument string representation.")]
        public string TextBefore { get; set; } = "";

        /// <summary>
        /// Gets or sets string which will be output after argument string representation.
        /// </summary>
		[DefaultValue("")]
        [StiSerializable]
        [Description("Gets or sets string which will be output after argument string representation.")]
        public string TextAfter { get; set; } = "";

        /// <summary>
        /// DBS use only!
        /// </summary>
        [Browsable(false)]
        public StiFormatService FormatService { get; set; }

        /// <summary>
        /// DBS use only!
        /// </summary>
        [Browsable(false)]
        public float CalculatedStep { get; set; }
        #endregion

        public StiAxisLabels3D()
        {
            this.Core = new StiAxisLabelsCoreXF3D(this);
        }

        [StiUniversalConstructor("Labels")]
        public StiAxisLabels3D(
            string format,
            string textBefore,
            string textAfter,
            Font font,
            Color color,
            bool allowApplyStyle)
        {
            this.Format = format;
            this.TextBefore = textBefore;
            this.TextAfter = textAfter;
            this.Font = font;
            this.Color = color;
            this.AllowApplyStyle = allowApplyStyle;

            this.Core = new StiAxisLabelsCoreXF3D(this);
        }
    }
}
