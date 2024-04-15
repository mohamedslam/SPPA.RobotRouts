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
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Components.TextFormats;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing;
using Stimulsoft.Base.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Font = Stimulsoft.Drawing.Font;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(StiUniversalConverter))]
    public class StiAxisLabels : 
        IStiAxisLabels,
        IStiFont
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyBool(nameof(AllowApplyStyle), AllowApplyStyle, true);
            jObject.AddPropertyStringNullOrEmpty(nameof(Format), Format);
            jObject.AddPropertyFloat(nameof(Angle), Angle, 0f);
            jObject.AddPropertyFloat(nameof(Width), Width, 0f);
            jObject.AddPropertyStringNullOrEmpty(nameof(TextBefore), TextBefore);
            jObject.AddPropertyStringNullOrEmpty(nameof(TextAfter), TextAfter);
            jObject.AddPropertyFontTahoma8(nameof(Font), Font);
            jObject.AddPropertyBool(nameof(Antialiasing), Antialiasing, true);
            jObject.AddPropertyEnum(nameof(Placement), Placement, StiLabelsPlacement.OneLine);
            jObject.AddPropertyColor(nameof(Color), Color, Color.Black);
            jObject.AddPropertyEnum(nameof(TextAlignment), TextAlignment, StiHorAlignment.Right);
            jObject.AddPropertyFloat(nameof(Step), Step, 0f);
            jObject.AddPropertyBool(nameof(WordWrap), WordWrap);

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

                    case nameof(Angle):
                        this.Angle = property.DeserializeFloat();
                        break;

                    case nameof(Width):
                        this.Width = property.DeserializeFloat();
                        break;

                    case nameof(TextBefore):
                        this.TextBefore = property.DeserializeString();
                        break;

                    case nameof(TextAfter):
                        this.TextAfter = property.DeserializeString();
                        break;

                    case nameof(Font):
                        this.Font = property.DeserializeFont(Font);
                        break;

                    case nameof(Antialiasing):
                        this.Antialiasing = property.DeserializeBool();
                        break;

                    case nameof(Placement):
                        this.Placement = property.DeserializeEnum<StiLabelsPlacement>();
                        break;

                    case nameof(Color):
                        this.Color = property.DeserializeColor();
                        break;

                    case nameof(TextAlignment):
                        this.TextAlignment = property.DeserializeEnum<StiHorAlignment>(); 
                        break;

                    case nameof(Step):
                        this.Step = property.DeserializeFloat();
                        break;

                    case nameof(WordWrap):
                        this.WordWrap = property.DeserializeBool();
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

            labels.Placement = this.Placement;
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
                    && Angle == 0f
                    && Width == 0f
                    && (TextAfter != null && TextAfter.Length == 0)
                    && (TextBefore != null && TextBefore.Length == 0)
                    && !ShouldSerializeFont()
                    && Antialiasing
                    && Placement == StiLabelsPlacement.OneLine
                    && TextAlignment == StiHorAlignment.Right
                    && Step == 0f
                    && !WordWrap;
            }
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public StiAxisLabelsCoreXF Core { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that chart style will be used.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
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
        /// Gets or sets angle of label rotation.
        /// </summary>
		[DefaultValue(0f)]
        [StiSerializable]
        [Description("Gets or sets angle of label rotation.")]
        public float Angle { get; set; }

        /// <summary>
        /// Gets or sets fixed width of axis labels.
        /// </summary>
		[DefaultValue(0f)]
        [StiSerializable]
        [Description("Gets or sets fixed width of axis labels.")]
        public float Width { get; set; }

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
        /// Gets or sets font which will be used for axis label drawing.
        /// </summary>
		[StiSerializable]
        [Editor(StiEditors.Font, typeof(UITypeEditor))]
        [Description("Gets or sets font which will be used for axis label drawing.")]
        public Font Font { get; set; } = new Font("Tahoma", 8);

        private bool ShouldSerializeFont()
        {
            return !(Font != null && Font.Name == "Tahoma" && Font.SizeInPoints == 8 && Font.Style == FontStyle.Regular);
        }

        /// <summary>
        /// Gets or sets value which control antialiasing drawing mode.
        /// </summary>
		[StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which control antialiasing drawing mode.")]
        public bool Antialiasing { get; set; } = true;

        /// <summary>
        /// Gets or set mode of labels placement on axis.
        /// </summary>
		[StiSerializable]
        [DefaultValue(StiLabelsPlacement.OneLine)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or set mode of labels placement on axis.")]
        public StiLabelsPlacement Placement { get; set; } = StiLabelsPlacement.OneLine;

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
        /// Gets or sets label text alignment.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiHorAlignment.Right)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets label text alignment.")]
        public virtual StiHorAlignment TextAlignment { get; set; } = StiHorAlignment.Right;

        /// <summary>
        /// Gets or sets value which indicates with what steps do labels be shown on axis.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0f)]
        [Description("Gets or sets value which indicates with what steps do labels be shown on axis.")]
        public virtual float Step { get; set; }

        /// <summary>
        /// DBS use only!
        /// </summary>
        [Browsable(false)]
        public float CalculatedStep { get; set; }

        [StiSerializable]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets word wrap.")]
        public bool WordWrap { get; set; }

        /// <summary>
        /// DBS use only!
        /// </summary>
        [Browsable(false)]
        public StiFormatService FormatService { get; set; }
        #endregion

        public StiAxisLabels()
        {
            this.Core = new StiAxisLabelsCoreXF(this);
        }

        public StiAxisLabels(
            string format,
            string textBefore,
            string textAfter,
            float angle,
            Font font,
            bool antialiasing,
            StiLabelsPlacement placement,
            Color color,
            float width,
            StiHorAlignment textAlignment,
            float step,
            bool allowApplyStyle)
            : this(format, textBefore, textAfter, angle, font, antialiasing, placement, color, width, textAlignment, step, allowApplyStyle, false)
        {

        }

        [StiUniversalConstructor("Labels")]
        public StiAxisLabels(
            string format,
            string textBefore,
            string textAfter,
            float angle,
            Font font,
            bool antialiasing,
            StiLabelsPlacement placement,
            Color color,
            float width,
            StiHorAlignment textAlignment,
            float step,
            bool allowApplyStyle,
            bool wordWrap)
        {
            this.Format = format;
            this.TextBefore = textBefore;
            this.TextAfter = textAfter;
            this.Angle = angle;
            this.Font = font;
            this.Antialiasing = antialiasing;
            this.Placement = placement;
            this.Color = color;
            this.Width = width;
            this.TextAlignment = textAlignment;
            this.Step = step;
            this.AllowApplyStyle = allowApplyStyle;
            this.WordWrap = wordWrap;

            this.Core = new StiAxisLabelsCoreXF(this);
        }
    }
}
