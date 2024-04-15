#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Base.Drawing
{
    [Editor("Stimulsoft.Base.Drawing.Design.StiSimpleShadowEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
    [JsonObject]
    [TypeConverter(typeof(StiSimpleShadowConverter))]
    public class StiSimpleShadow :
        ICloneable,
        IStiJsonReportObject
    {   
        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public virtual object Clone()
        {
            return (StiSimpleShadow)MemberwiseClone();
        }
        #endregion

        #region IStiJsonReportObject
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            if (this.IsDefault)
                return null;

            var jObject = new JObject();

            jObject.AddPropertyColor(nameof(Color), Color, "#44222222");
            jObject.AddPropertyPoint(nameof(Location), Location);
            jObject.AddPropertyInt(nameof(Size), Size, 10);
            jObject.AddPropertyBool(nameof(Visible), Visible);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Color):
                        Color = property.DeserializeColor();
                        break;

                    case nameof(Location):
                        Location = property.DeserializePoint();
                        break;

                    case nameof(Size):
                        Size = property.DeserializeInt();
                        break;

                    case nameof(Visible):
                        Visible = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a shadow color.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets a border color.")]
        [RefreshProperties(RefreshProperties.All)]
        [StiOrder(100)]
        public virtual Color Color { get; set; } = StiColor.Get("#44222222");

        private bool ShouldSerializeColor()
        {
            return Color != StiColor.Get("#44222222");
        }

        private Point location = new Point(2, 2);
        /// <summary>
        /// Gets or sets a shadow color.
        /// </summary>
        [StiSerializable]
        [Description("Gets or sets a shadow location.")]
        [RefreshProperties(RefreshProperties.All)]
        [StiOrder(200)]        
        public virtual Point Location
        {
            get 
            { 
                return location; 
            }
            set
            {
                if (value != location)
                {
                    if (value.X < -10)
                        value.X = -10;

                    if (value.X > 10)
                        value.X = 10;

                    if (value.Y < -10)
                        value.Y = -10;

                    if (value.Y > 10)
                        value.Y = 10;

                    location = value;
                }
            }
        }
        private bool ShouldSerializeLocation()
        {
            return Location == null || Location.X != 2 || Location.Y != 2;
        }

        private int size = 5;
        /// <summary>
        /// Gets or sets a border size.
        /// </summary>
        [StiSerializable]
        [DefaultValue(5)]
        [Description("Gets or sets a shadow size.")]
        [RefreshProperties(RefreshProperties.All)]
        [StiOrder(300)]
        public virtual int Size
        {
            get
            {
                return size;
            }
            set
            {
                if (size != value)
                {
                    if (value < 1)
                        value = 1;

                    if (value > 10)
                        value = 10;

                    size = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets value which indicates that shadow is visible or not.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that shadow is visible or not.")]
        [RefreshProperties(RefreshProperties.All)]
        [StiOrder(400)]
        public virtual bool Visible { get; set; }

        /// <summary>
        /// Gets value indicates, that this object-frame is by default.
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public virtual bool IsDefault =>
            !ShouldSerializeColor() &&
            !ShouldSerializeLocation() &&
            Size == 5 &&
            !Visible;
        #endregion

        #region Methods
        /// <summary>
        /// Tests to see whether the specified object is a StiSimpleShadow with the same dimensions as this StiSimpleShadow.
        /// </summary>
        /// <param name="obj">The Object to test.</param>
        /// <returns>This method returns true if obj is a StiSimpleShadow and has the same width and height as this StiSimpleShadow; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            var shadow = (StiSimpleShadow)obj;
            return shadow != null
                   && shadow.Color == Color
                   && shadow.Location == Location
                   && shadow.Size == Size
                   && shadow.Visible == Visible;
        }

        /// <summary>
        /// Returns a hash code for this StiMargins structure.
        /// </summary>
        /// <returns>An integer value that specifies a hash value for this StiMargins structure.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public int GetUniqueCode()
        {
            unchecked
            {
                var hashCode = 0;

                hashCode = (hashCode * 397) ^ Color.GetHashCode();
                hashCode = (hashCode * 397) ^ Location.GetHashCode();
                hashCode = (hashCode * 397) ^ Size.GetHashCode();
                hashCode = (hashCode * 397) ^ Visible.GetHashCode();                

                return hashCode;
            }
        }
        #endregion

        public StiSimpleShadow()
        {
        }

        public StiSimpleShadow(Color color, Point location, int size, bool visible)
        {
            this.Color = color;
            this.Location = location;
            this.Size = size;
            this.Visible = visible;
        }
    }
}
