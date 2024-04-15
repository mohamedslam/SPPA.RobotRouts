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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Design;
using System;
using System.ComponentModel;
using System.Drawing.Design;
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
    public class StiAxisTitle : 
        IStiAxisTitle,
        IStiFont
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyBool(nameof(AllowApplyStyle), AllowApplyStyle, true);
            jObject.AddPropertyFontTahoma12Bold(nameof(Font), Font);
            jObject.AddPropertyStringNullOrEmpty(nameof(Text), Text);
            jObject.AddPropertyColor(nameof(Color), Color, Color.Black);
            jObject.AddPropertyBool(nameof(Antialiasing), Antialiasing, true);
            jObject.AddPropertyEnum(nameof(Alignment), Alignment, StringAlignment.Center);
            jObject.AddPropertyEnum(nameof(Position), Position, StiTitlePosition.Outside);
            jObject.AddPropertyEnum(nameof(Direction), Direction, StiDirection.LeftToRight);

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

                    case nameof(Font):
                        this.Font = property.DeserializeFont(Font);
                        break;

                    case nameof(Text):
                        this.Text = property.DeserializeString();
                        break;

                    case nameof(Color):
                        this.Color = property.DeserializeColor();
                        break;

                    case nameof(Antialiasing):
                        this.Antialiasing = property.DeserializeBool();
                        break;

                    case nameof(Alignment):
                        this.Alignment = property.DeserializeEnum<StringAlignment>();
                        break;

                    case nameof(Position):
                        this.Position = property.DeserializeEnum<StiTitlePosition>(); 
                        break;

                    case nameof(Direction):
                        this.Direction = property.DeserializeEnum<StiDirection>(); 
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
            var title = this.MemberwiseClone() as IStiAxisTitle;

            title.Alignment = this.Alignment;
            title.Direction = this.Direction;
            title.Font = this.Font.Clone() as Font;

            if (this.Core != null)
            {
                title.Core = this.Core.Clone() as StiAxisTitleCoreXF;
                title.Core.Title = title;
            }

            return title;
        }
        #endregion

        #region IStiDefault
        [Browsable(false)]
        public bool IsDefault
        {
            get
            {
                //Color is not taken into considaration because AllowApplyStyle.
                return
                    AllowApplyStyle
                    && !ShouldSerializeFont()
                    && (Text != null && Text.Length == 0)
                    && Antialiasing
                    && Alignment == StringAlignment.Center
                    && Position == StiTitlePosition.Outside
                    && Direction == StiDirection.LeftToRight;
            }
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public StiAxisTitleCoreXF Core { get; set; }

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
        /// Gets or set font which will be used for axis title drawing.
        /// </summary>
		[StiSerializable]
        [Editor(StiEditors.Font, typeof(UITypeEditor))]
        [Description("Gets or set font which will be used for axis title drawing.")]
        public Font Font { get; set; } = new Font("Tahoma", 12, FontStyle.Bold);

        private bool ShouldSerializeFont()
        {
            return !(Font != null && Font.Name == "Tahoma" && Font.SizeInPoints == 12 && Font.Style == FontStyle.Bold);
        }

        /// <summary>
        /// Gets or sets title text.
        /// </summary>
		[DefaultValue("")]
        [StiSerializable]
        [Description("Gets or sets title text.")]
        public string Text { get; set; } = "";

        /// <summary>
        /// Gets or sets color which will be used for title drawing.
        /// </summary>
		[StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets color which will be used for title drawing.")]
        public Color Color { get; set; } = Color.Black;

        private bool ShouldSerializeColor()
        {
            return Color != Color.Black;
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
        /// Gets or sets title text alignment.
        /// </summary>
		[StiSerializable]
        [DefaultValue(StringAlignment.Center)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets title text alignment.")]
        public StringAlignment Alignment { get; set; } = StringAlignment.Center;

        /// <summary>
        /// Gets or sets title text position.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiTitlePosition.Outside)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets title text position.")]
        public StiTitlePosition Position { get; set; } = StiTitlePosition.Outside;

        /// <summary>
        /// Gets or set text direction for axis title drawing.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiDirection.LeftToRight)]
        [Description("Gets or set text direction for axis title drawing.")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        public virtual StiDirection Direction { get; set; } = StiDirection.LeftToRight;
        #endregion

        public StiAxisTitle()
        {
            this.Core = new StiAxisTitleCoreXF(this);
        }

        public StiAxisTitle(
            Font font,
            string text,
            Color color,
            bool antialiasing,
            StringAlignment alignment,
            StiDirection direction,
            bool allowApplyStyle
            )
        {
            this.Font = font;
            this.Text = text;
            this.Color = color;
            this.Antialiasing = antialiasing;
            this.Alignment = alignment;
            this.Direction = direction;
            this.AllowApplyStyle = allowApplyStyle;

            this.Core = new StiAxisTitleCoreXF(this);
        }

        [StiUniversalConstructor("Title")]
        public StiAxisTitle(
            Font font,
            string text,
            Color color,
            bool antialiasing,
            StringAlignment alignment,
            StiDirection direction,
            bool allowApplyStyle,
            StiTitlePosition position
            )
        {
            this.Font = font;
            this.Text = text;
            this.Color = color;
            this.Antialiasing = antialiasing;
            this.Alignment = alignment;
            this.Direction = direction;
            this.AllowApplyStyle = allowApplyStyle;
            this.Position = position;

            this.Core = new StiAxisTitleCoreXF(this);
        }
    }
}
