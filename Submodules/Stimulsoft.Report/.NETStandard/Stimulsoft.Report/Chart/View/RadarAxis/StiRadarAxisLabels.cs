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
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Components;
using System.Drawing;
using Stimulsoft.Base.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Chart
{    
	
	[TypeConverter(typeof(StiUniversalConverter))]
    public class StiRadarAxisLabels : 
        IStiRadarAxisLabels,
        IStiFont
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyBool("RotationLabels", RotationLabels, true);
            jObject.AddPropertyStringNullOrEmpty("TextBefore", TextBefore);
            jObject.AddPropertyStringNullOrEmpty("TextAfter", TextAfter);
            jObject.AddPropertyBool("AllowApplyStyle", AllowApplyStyle, true);
            jObject.AddPropertyBool("DrawBorder", DrawBorder);
            jObject.AddPropertyStringNullOrEmpty("Format", Format);
            jObject.AddPropertyFontTahoma8("Font", Font);
            jObject.AddPropertyBool("Antialiasing", Antialiasing, true);
            jObject.AddPropertyColor("Color", Color, Color.Black);
            jObject.AddPropertyColor("BorderColor", BorderColor, Color.Black);
            jObject.AddPropertyBrush("Brush", Brush);
            jObject.AddPropertyFloat("Width", Width);
            jObject.AddPropertyBool("WordWrap", WordWrap);
            jObject.AddPropertyBool("PreventIntersection", PreventIntersection);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "PreventIntersection":
                        this.PreventIntersection = property.DeserializeBool();
                        break;

                    case "RotationLabels":
                        this.RotationLabels = property.DeserializeBool();
                        break;

                    case "TextBefore":
                        this.TextBefore = property.DeserializeString();
                        break;

                    case "TextAfter":
                        this.TextAfter = property.DeserializeString();
                        break;

                    case "AllowApplyStyle":
                        this.AllowApplyStyle = property.DeserializeBool();
                        break;

                    case "DrawBorder":
                        this.DrawBorder = property.DeserializeBool();
                        break;

                    case "Format":
                        this.Format = property.DeserializeString();
                        break;

                    case "Font":
                        this.Font = property.DeserializeFont(Font);
                        break;

                    case "Antialiasing":
                        this.Antialiasing = property.DeserializeBool();
                        break;

                    case "Color":
                        this.Color = property.DeserializeColor();
                        break;

                    case "BorderColor":
                        this.BorderColor = property.DeserializeColor();
                        break;

                    case "Brush":
                        this.Brush = property.DeserializeBrush();
                        break;

                    case "Width":
                        this.Width = property.DeserializeFloat();
                        break;

                    case "WordWrap":
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
			var labels = this.MemberwiseClone() as IStiRadarAxisLabels;

			labels.Font = this.Font.Clone() as Font;
            labels.Brush = this.Brush.Clone() as StiBrush;

            if (this.Core != null)
            {
                labels.Core = this.Core.Clone() as StiRadarAxisLabelsCoreXF;
                labels.Core.Labels = labels;
            }
			
			return labels;
		}
		#endregion

		#region Properties
        [Browsable(false)]
        public StiRadarAxisLabelsCoreXF Core { get; set; }

        /// <summary>
        /// Gets or sets a value which indicates that Axis Labels will be rotated.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates that Axis Labels will be rotated.")]
        [StiCategory("Common")]
        public bool RotationLabels { get; set; } = true;

        /// <summary>
        /// Gets or sets string which will be output before argument string representation.
        /// </summary>
        [DefaultValue("")]
        [StiSerializable]
        [Description("Gets or sets string which will be output before argument string representation.")]
        [StiCategory("Common")]
        public string TextBefore { get; set; } = "";

        /// <summary>
        /// Gets or sets string which will be output after argument string representation.
        /// </summary>
        [DefaultValue("")]
        [StiSerializable]
        [Description("Gets or sets string which will be output after argument string representation.")]
        [StiCategory("Common")]
        public string TextAfter { get; set; } = "";

        /// <summary>
        /// Gets or sets value which indicates that chart style will be used.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that chart style will be used.")]
        [DefaultValue(true)]
        public bool AllowApplyStyle { get; set; } = true;

        /// <summary>
        /// Gets or sets value which indicates that label border will be shown.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that label border will be shown.")]
        [DefaultValue(false)]
        [StiCategory("Common")]
        public bool DrawBorder { get; set; }

        /// <summary>
        /// Gets os sets format string which is used for formating argument values.
        /// </summary>
		[DefaultValue("")]
		[StiSerializable]
		[Editor("Stimulsoft.Report.Chart.Design.StiFormatEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets os sets format string which is used for formating argument values.")]
        [StiCategory("Common")]
		public string Format { get; set; } = "";

        /// <summary>
        /// Gets or sets font which will be used for axis label drawing.
        /// </summary>
		[StiSerializable]
        [Description("Gets or sets font which will be used for axis label drawing.")]
        [StiCategory("Common")]
        [Editor(StiEditors.Font, typeof(UITypeEditor))]
        public Font Font { get; set; } = new Font("Tahoma", 8);

        /// <summary>
        /// Gets or sets value which control antialiasing drawing mode.
        /// </summary>
		[StiSerializable]
		[DefaultValue(true)]
		[TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which control antialiasing drawing mode.")]
        [StiCategory("Appearance")]
		public bool Antialiasing { get; set; } = true;

        /// <summary>
        /// Gets or sets color of labels drawing.
        /// </summary>
		[StiSerializable]
		[TypeConverter(typeof(StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets color of labels drawing.")]
        [StiCategory("Appearance")]
		public Color Color { get; set; } = Color.Black;

        /// <summary>
        /// Gets or sets color of labels drawing.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets color of labels drawing.")]
        [StiCategory("Appearance")]
        public Color BorderColor { get; set; } = Color.Black;

        /// <summary>
        /// Gets or sets brush which will used to fill label area.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets brush which will used to fill label area.")]
        public StiBrush Brush { get; set; } = new StiSolidBrush(Color.Gainsboro);

        /// <summary>
        /// Gets or sets fixed width of axis labels.
        /// </summary>
        [DefaultValue(0f)]
        [StiSerializable]
        [Description("Gets or sets fixed width of axis labels.")]
        [StiCategory("Common")]
        public float Width { get; set; } = 0f;

        /// <summary>
        /// Gets or sets word wrap.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets word wrap.")]
        [StiCategory("Common")]
        public bool WordWrap { get; set; }

        [StiSerializable]
        [StiOrder(StiSeriesLabelsPropertyOrder.PreventIntersection)]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("Common")]
        public bool PreventIntersection { get; set; }
        #endregion

        public StiRadarAxisLabels()
		{
            this.Core = new StiRadarAxisLabelsCoreXF(this);
		}

        public StiRadarAxisLabels(
            string format,
            Font font,
            bool antialiasing,
            bool drawBorder,
            Color color,
            Color borderColor,
            StiBrush brush,
            bool allowApplyStyle
            )
        {
            this.Format = format;
            this.Font = font;
            this.Antialiasing = antialiasing;
            this.DrawBorder = drawBorder;
            this.Color = color;
            this.BorderColor = borderColor;
            this.AllowApplyStyle = allowApplyStyle;
            this.Brush = brush;
            this.Width = 0;
            this.WordWrap = false;

            this.Core = new StiRadarAxisLabelsCoreXF(this);
        }
        
        public StiRadarAxisLabels(
			string format,
			Font font,
			bool antialiasing,
            bool drawBorder,
			Color color,
            Color borderColor,	
		    StiBrush brush,
            bool allowApplyStyle,
            bool rotationLabels
			)
		{
			this.Format = format;
			this.Font = font;
			this.Antialiasing = antialiasing;
            this.DrawBorder = drawBorder;
			this.Color = color;
            this.BorderColor = borderColor;
            this.AllowApplyStyle = allowApplyStyle;
            this.Brush = brush;
            this.RotationLabels = rotationLabels;
            this.Width = 0;
            this.WordWrap = false;

            this.Core = new StiRadarAxisLabelsCoreXF(this);
		}

        public StiRadarAxisLabels(
            string format,
            Font font,
            bool antialiasing,
            bool drawBorder,
            Color color,
            Color borderColor,
            StiBrush brush,
            bool allowApplyStyle,
            bool rotationLabels,
            float width,
            bool wordWrap
            )
        {
            this.Format = format;
            this.Font = font;
            this.Antialiasing = antialiasing;
            this.DrawBorder = drawBorder;
            this.Color = color;
            this.BorderColor = borderColor;
            this.AllowApplyStyle = allowApplyStyle;
            this.Brush = brush;
            this.RotationLabels = rotationLabels;
            this.Width = width;
            this.WordWrap = wordWrap;

            this.Core = new StiRadarAxisLabelsCoreXF(this);
        }

        [StiUniversalConstructor("Labels")]
        public StiRadarAxisLabels(
            string format,
            Font font,
            bool antialiasing,
            bool drawBorder,
            Color color,
            Color borderColor,
            StiBrush brush,
            bool allowApplyStyle,
            bool rotationLabels,
            float width,
            bool wordWrap,
            bool preventIntersection
            )
        {
            this.Format = format;
            this.Font = font;
            this.Antialiasing = antialiasing;
            this.DrawBorder = drawBorder;
            this.Color = color;
            this.BorderColor = borderColor;
            this.AllowApplyStyle = allowApplyStyle;
            this.Brush = brush;
            this.RotationLabels = rotationLabels;
            this.Width = width;
            this.WordWrap = wordWrap;
            this.PreventIntersection = preventIntersection;

            this.Core = new StiRadarAxisLabelsCoreXF(this);
        }
    }
}
