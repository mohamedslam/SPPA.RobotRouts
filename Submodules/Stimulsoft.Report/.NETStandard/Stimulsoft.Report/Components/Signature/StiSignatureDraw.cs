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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components.Design;
using System.ComponentModel;
using System.Drawing.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components
{
    [TypeConverter(typeof(StiUniversalConverter))]
    public class StiSignatureDraw :
        IStiSignatureDraw
    {
        #region enum Order
        public enum Order
        {
            Image = 1,
            AspectRatio = 2,
            HorAlignment = 3,
            VertAlignment = 4,
            Stretch = 5
        }
        #endregion

        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            if (Image != null)
                jObject.AddPropertyByteArray(nameof(Image), Image);
            
            jObject.AddPropertyBool(nameof(AspectRatio), AspectRatio);
            jObject.AddPropertyEnum(nameof(HorAlignment), HorAlignment, StiHorAlignment.Center);
            jObject.AddPropertyEnum(nameof(VertAlignment), VertAlignment, StiVertAlignment.Center);
            jObject.AddPropertyBool(nameof(Stretch), Stretch);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Image):
                        this.Image = property.DeserializeByteArray();
                        break;

                    case nameof(AspectRatio):
                        this.AspectRatio = property.DeserializeBool();
                        break;

                    case nameof(HorAlignment):
                        this.HorAlignment = property.DeserializeEnum<StiHorAlignment>();
                        break;

                    case nameof(VertAlignment):
                        this.VertAlignment = property.DeserializeEnum<StiVertAlignment>();
                        break;

                    case nameof(Stretch):
                        this.Stretch = property.DeserializeBool();
                        break;
                }
            }
        }

        internal static StiSignatureDraw CreateFromJsonObject(JObject jObject)
        {
            var title = new StiSignatureDraw();
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

        #region IStiSignatureImage
        [StiSerializable()]
        [StiCategory("Common")]
        [TypeConverter(typeof(StiSimpeImageConverter))]
        [StiPropertyLevel(StiLevel.Basic)]
        [StiOrder((int)Order.Image)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public byte[] Image { get; set; }

        private bool ShouldSerializeImage()
        {
            return Image != null;
        }

        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Common")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value, indicates that the image will save its aspect ratio.")]
        [StiOrder((int)Order.AspectRatio)]
        public bool AspectRatio { get; set; }

        private bool ShouldSerializeAspectRatio()
        {
            return AspectRatio != false;
        }

        [StiSerializable]
        [StiCategory("Common")]
        [StiPropertyLevel(StiLevel.Basic)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiOrder((int)Order.HorAlignment)]
        public StiHorAlignment HorAlignment { get; set; } = StiHorAlignment.Center;

        private bool ShouldSerializeHorAlignment()
        {
            return HorAlignment != StiHorAlignment.Center;
        }


        [StiSerializable]
        [StiCategory("Common")]
        [StiPropertyLevel(StiLevel.Basic)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiOrder((int)Order.VertAlignment)]
        public StiVertAlignment VertAlignment { get; set; } = StiVertAlignment.Center;

        private bool ShouldSerializeVertAlignment()
        {
            return VertAlignment != StiVertAlignment.Center;
        }

        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Common")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value, indicates that the image will save its aspect ratio.")]
        [StiOrder((int)Order.Stretch)]
        public bool Stretch { get; set; }

        private bool ShouldSerializeStretch()
        {
            return Stretch != false;
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public virtual bool IsDefault =>
            !ShouldSerializeImage() &&
            !ShouldSerializeAspectRatio() &&
            !ShouldSerializeHorAlignment() &&
            !ShouldSerializeVertAlignment() && 
            !ShouldSerializeStretch();
        #endregion

        public StiSignatureDraw()
        {
        }

        [StiUniversalConstructor("Draw")]
        public StiSignatureDraw(byte[] image, bool aspectRatio, StiHorAlignment horAlignment, 
            StiVertAlignment vertAlignment, bool stretch)
        {
            this.Image = image;
            this.AspectRatio = aspectRatio;
            this.HorAlignment = horAlignment;
            this.VertAlignment = vertAlignment;
            this.Stretch = stretch;
        }
    }
}