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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
	[TypeConverter(typeof(StiUniversalConverter))]
    public abstract class StiRadarGridLines :
        IStiRadarGridLines,
        ICloneable,
        IStiPropertyGridObject
	{
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyBool("AllowApplyStyle", allowApplyStyle, true);
            jObject.AddPropertyColor("Color", Color, Color.Silver);
            jObject.AddPropertyEnum("Style", Style, StiPenStyle.Solid);
            jObject.AddPropertyBool("Visible", Visible, true);

            if (Area != null)
                jObject.AddPropertyBool("Area", true);

            return jObject;
        }

        internal bool needSetAreaJsonPropertyInternal = false;
        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "AllowApplyStyle":
                        this.allowApplyStyle = property.DeserializeBool();
                        break;

                    case "Color":
                        this.Color = property.DeserializeColor();
                        break;

                    case "Style":
                        this.Style = property.DeserializeEnum<StiPenStyle>();
                        break;

                    case "Visible":
                        this.Visible = property.DeserializeBool();
                        break;

                    case "Area":
                        this.needSetAreaJsonPropertyInternal = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public StiComponentId ComponentId => StiComponentId.StiRadarGridLines;

        [Browsable(false)]
        public string PropName => string.Empty;

        public StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var objHelper = new StiPropertyCollection();
            var propHelper = propertyGrid.PropertiesHelper;

            var list = new[] 
            { 
                propHelper.RadarGridLines()
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
            var gridLines = this.MemberwiseClone() as IStiRadarGridLines;
			gridLines.Style = this.Style;

            if (this.Core != null)
            {
                gridLines.Core = this.Core.Clone() as StiRadarGridLinesCoreXF;
                gridLines.Core.GridLines = gridLines;
            }
			
			return gridLines;
		}
        #endregion

        #region Properties
        [Browsable(false)]
        public StiRadarGridLinesCoreXF Core { get; set; }

        private bool allowApplyStyle = true;
        /// <summary>
        /// Gets or sets value which indicates that chart style will be used.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that chart style will be used.")]
        [DefaultValue(true)]
        public bool AllowApplyStyle
        {
            get
            {
                return allowApplyStyle;
            }
            set
            {
                if (allowApplyStyle != value)
                {
                    allowApplyStyle = value;

                    if (value && Area != null && Area.Chart != null)
                        this.Core.ApplyStyle(this.Area.Chart.Style);
                }
            }
        }

        /// <summary>
        /// Gets or sets color which will be used for drawing major grid lines.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets color which will be used for drawing major grid lines.")]
        public Color Color { get; set; } = Color.Silver;

        /// <summary>
        /// Gets or sets style which will be used for drawing major grid lines.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiPenStyle.Solid)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.PenStyle, typeof(UITypeEditor))]
        [Description("Gets or sets style which will be used for drawing major grid lines.")]
        [StiCategory("Common")]
        public StiPenStyle Style { get; set; } = StiPenStyle.Solid;

        /// <summary>
        /// Gets or sets visibility of major grid lines.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets visibility of major grid lines.")]
        [StiCategory("Common")]
        public bool Visible { get; set; } = true;

        [StiSerializable(StiSerializationVisibility.Reference)]
        [Browsable(false)]
        public IStiArea Area { get; set; }
        #endregion

        public StiRadarGridLines()
		{
            this.Core = new StiRadarGridLinesCoreXF(this);
		}

        public StiRadarGridLines(
			Color color,
			StiPenStyle style,
			bool visible,
            bool allowApplyStyle
			)
		{
			this.Color = color;
			this.Style = style;
			this.Visible = visible;
            this.allowApplyStyle = allowApplyStyle;

            this.Core = new StiRadarGridLinesCoreXF(this);
		}
	}
}
