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

using System.Drawing.Design;
using System.ComponentModel;
using System.Drawing;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using System;
using Stimulsoft.Base.Design;
using Stimulsoft.Report.Helpers;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(Design.StiMarkerConverter))]
	public class StiMarker: 
        IStiMarker,
		IStiSerializeToCodeAsClass,
        IStiPropertyGridObject
    {
        #region IStiJsonReportObject.override
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyBool("ShowInLegend", ShowInLegend, true);
            jObject.AddPropertyBool("Visible", Visible, true);
            jObject.AddPropertyBrush("Brush", Brush);
            jObject.AddPropertyColor("BorderColor", BorderColor, Color.Black);
            jObject.AddPropertyFloat("Size", Size, 7f);
            jObject.AddPropertyFloat("Angle", Angle, 0f);
            jObject.AddPropertyEnum("Type", Type, StiMarkerType.Circle);
            if (this.Icon != null)
                jObject.AddPropertyEnum("Icon", this.Icon);

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "ShowInLegend":
                        this.ShowInLegend = property.DeserializeBool();
                        break;

                    case "Visible":
                        this.Visible = property.DeserializeBool();
                        break;

                    case "Brush":
                        this.Brush = property.DeserializeBrush();
                        break;

                    case "BorderColor":
                        this.BorderColor = property.DeserializeColor();
                        break;

                    case "Size":
                        this.Size = property.DeserializeFloat();
                        break;

                    case "Angle":
                        this.Angle = property.DeserializeFloat();
                        break;

                    case "Type":
                        this.Type = property.DeserializeEnum<StiMarkerType>();
                        break;

                    case "Icon":
                        this.Icon = property.DeserializeEnum<StiFontIcons>(); 
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public virtual StiComponentId ComponentId => StiComponentId.StiMarker;

        [Browsable(false)]
        public string PropName => string.Empty;

        public virtual StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var objHelper = new StiPropertyCollection();
            var propHelper = propertyGrid.PropertiesHelper;

            var list = new[] 
            { 
                propHelper.Marker()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }

        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return null;
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            var marker = this.MemberwiseClone() as IStiMarker;

            if (this.Core != null)
            {
                marker.Core = this.Core.Clone() as StiMarkerCoreXF;
                marker.Core.Marker = marker;
            }

            return marker;
        }
        #endregion

        #region IStiDefault
        [Browsable(false)]
        public virtual bool IsDefault
        {
            get
            {
                return                    
                    !ShouldSerializeVisible()                    
                    && !ShouldSerializeBrush()
                    && !ShouldSerializeBorderColor()
                    && !ShouldSerializeSize()
                    && ShowInLegend
                    && ExtendedVisible == StiExtendedStyleBool.FromStyle
                    && Angle == 0f
                    && Type == StiMarkerType.Circle;
            }
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public StiMarkerCoreXF Core { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that marker will be visible in legend marker.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [Browsable(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that marker will be visible in legend marker.")]
        public virtual bool ShowInLegend { get; set; } = true;

        /// <summary>
        /// Gets or sets visibility of marker.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("Common")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets visibility of marker.")]
        public virtual bool Visible { get; set; } = true;

        protected virtual bool ShouldSerializeVisible()
        {
            return !Visible;
        }

        /// <summary>
        /// DBS use only!
        /// </summary>
        [StiSerializable(StiSerializationVisibility.None)]
        [Browsable(false)]
        internal StiExtendedStyleBool ExtendedVisible { get; set; } = StiExtendedStyleBool.FromStyle;

        /// <summary>
        /// Gets or sets brush which will be used to fill marker area.
        /// </summary>
        [StiSerializable]
        [StiCategory("Common")]
        [Description("Gets or sets brush which will be used to fill marker area.")]
        public virtual StiBrush Brush { get; set; } = new StiSolidBrush(Color.White);

        private bool ShouldSerializeBrush()
        {
            return !(Brush is StiSolidBrush) || ((StiSolidBrush)Brush).Color != Color.White;
        }

        /// <summary>
        /// Gets or sets border color of marker.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
        [StiCategory("Common")]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets border color of marker.")]
        public Color BorderColor { get; set; } = Color.Black;

        private bool ShouldSerializeBorderColor()
        {
            return BorderColor != Color.Black;
        }

        /// <summary>
        /// Gets or sets size of the marker.
        /// </summary>
        [DefaultValue(7f)]
        [StiSerializable]
        [StiCategory("Common")]
        [Description("Gets or sets size of the marker.")]
        public virtual float Size { get; set; } = 7f;

        protected virtual bool ShouldSerializeSize()
        {
            return Size != 7f;
        }

        /// <summary>
        /// Gets or sets rotation angle of the marker.
        /// </summary>
        [DefaultValue(0f)]
        [StiSerializable]
        [StiCategory("Common")]
        [Description("Gets or sets rotation angle of the marker.")]
        public virtual float Angle { get; set; } = 0f;

        /// <summary>
        /// Gets or sets type of the marker.
        /// </summary>
        [DefaultValue(StiMarkerType.Circle)]
        [StiSerializable]
        [StiCategory("Common")]
        [TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets type of the marker.")]
        public StiMarkerType Type { get; set; } = StiMarkerType.Circle;

        [DefaultValue(null)]
        [StiSerializable]
        [StiCategory("Common")]
        [Editor("Stimulsoft.Report.Design.StiFontIconEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(Stimulsoft.Report.Design.StiFontIconConverter))]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiFontIcons? Icon { get; set; }
        #endregion

        public StiMarker()
        {
            this.Core = new StiMarkerCoreXF(this);
        }
    }
}
