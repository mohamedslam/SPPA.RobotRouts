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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Components
{
    [TypeConverter(typeof(StiUniversalConverter))]
    public class StiSignatureText :
        IStiSignatureText
    {
        #region enum Order
        public enum Order
        {
            Text = 1,
            HorAlignment = 2,
            VertAlignment = 3,
            Font = 4,
            Color = 5,
        }
        #endregion

        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyStringNullOrEmpty(nameof(Text), Text);
            jObject.AddPropertyEnum(nameof(HorAlignment), HorAlignment, StiTextHorAlignment.Center);
            jObject.AddPropertyEnum(nameof(VertAlignment), VertAlignment, StiVertAlignment.Center);
            jObject.AddPropertyFontSegoeUI12Bold(nameof(Font), Font);
            jObject.AddPropertyColor(nameof(Color), Color, Color.Black);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Text):
                        this.Text = property.DeserializeString();
                        break;

                    case nameof(HorAlignment):
                        this.HorAlignment = property.DeserializeEnum<StiTextHorAlignment>();
                        break;

                    case nameof(VertAlignment):
                        this.VertAlignment = property.DeserializeEnum<StiVertAlignment>();
                        break;

                    case nameof(Font):
                        this.Font = property.DeserializeFont(Font);
                        break;

                    case nameof(Color):
                        this.Color = property.DeserializeColor();
                        break;
                }
            }
        }

        internal static StiSignatureText CreateFromJsonObject(JObject jObject)
        {
            var title = new StiSignatureText();
            title.LoadFromJsonObject(jObject);

            return title;
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone() => this.MemberwiseClone();
        #endregion

        #region IStiSignatureText
        [StiSerializable]
        [StiCategory("Common")]
        [StiPropertyLevel(StiLevel.Basic)]
        [StiOrder((int)Order.Text)]
        public string Text { get; set; } = string.Empty;

        private bool ShouldSerializeText()
        {
            return Text != string.Empty;
        }

        [StiSerializable]
        [DefaultValue(StiTextHorAlignment.Center)]
        [StiCategory("Common")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        [StiOrder((int)Order.HorAlignment)]
        public StiTextHorAlignment HorAlignment { get; set; } = StiTextHorAlignment.Center;

        private bool ShouldSerializeHorAlignment()
        {
            return HorAlignment != StiTextHorAlignment.Center;
        }

        [StiSerializable]
        [Browsable(false)]
        [DefaultValue(StiVertAlignment.Center)]
        [StiCategory("Common")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        [StiOrder((int)Order.VertAlignment)]
        public StiVertAlignment VertAlignment { get; set; } = StiVertAlignment.Center;

        private bool ShouldSerializeVertAlignment()
        {
            return VertAlignment != StiVertAlignment.Center;
        }

        [StiSerializable]
        [StiCategory("Common")]
        [StiPropertyLevel(StiLevel.Basic)]
        [StiOrder((int)Order.Font)]
        [Editor(StiEditors.Font, typeof(UITypeEditor))]
        public Font Font { get; set; } = new Font("Segoe UI", 20);

        private bool ShouldSerializeFont()
        {
            return !(Font != null && Font.Name == "Segoe UI" && Font.SizeInPoints == 20 && Font.Style == FontStyle.Regular);
        }

        [StiSerializable]
        [StiCategory("Common")]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor(StiEditors.ExpressionColor, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        [StiOrder((int)Order.Color)]
        public Color Color { get; set; } = Color.Black;

        private bool ShouldSerializeColor()
        {
            return Color != Color.Black;
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public virtual bool IsDefault =>
            !ShouldSerializeText() &&
            !ShouldSerializeHorAlignment() &&
            !ShouldSerializeVertAlignment() &&
            !ShouldSerializeFont() &&
            !ShouldSerializeColor();
        #endregion

        public StiSignatureText()
        {
        }

        [StiUniversalConstructor("Text")]
        public StiSignatureText(string text, StiTextHorAlignment horAlignment, StiVertAlignment vertAlignment, Font font, Color color)
        {
            this.Text = text;
            this.HorAlignment = horAlignment;
            this.VertAlignment = vertAlignment;
            this.Font = font;
            this.Color = color;
        }
    }
}