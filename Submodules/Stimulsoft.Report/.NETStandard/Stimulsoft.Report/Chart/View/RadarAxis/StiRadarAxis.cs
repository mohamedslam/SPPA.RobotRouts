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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using System;
using System.ComponentModel;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(StiUniversalConverter))]
    public class StiRadarAxis : IStiRadarAxis
    {
        #region IStiJsonReportObject.override
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyBool("AllowApplyStyle", allowApplyStyle, true);
            jObject.AddPropertyBool("Visible", Visible, true);

            if (Area != null)
                jObject.AddPropertyBool("Area", true);
            if (Range != null)
                jObject.AddPropertyJObject("Range", this.Range.SaveToJsonObject(mode));

            return jObject;
        }

        internal bool jsonLoadFromJsonObjectArea = false;
        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "AllowApplyStyle":
                        this.allowApplyStyle = property.DeserializeBool();
                        break;

                    case "Visible":
                        this.Visible = property.DeserializeBool();
                        break;

                    case "Area":
                        this.jsonLoadFromJsonObjectArea = property.DeserializeBool();
                        break;

                    case "Range":
                        this.Range.LoadFromJsonObject((JObject)property.Value);
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
        public virtual object Clone()
        {
            var axis = this.MemberwiseClone() as IStiRadarAxis;
            if (this.Core != null)
            {
                axis.Core = this.Core.Clone() as StiRadarAxisCoreXF;
                axis.Core.Axis = axis;
            }

            return axis;
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public StiRadarAxisCoreXF Core { get; set; }

        private bool allowApplyStyle = true;
        /// <summary>
        /// Gets or sets value which indicates that chart style will be used.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that chart style will be used.2")]
        [DefaultValue(true)]
        [Browsable(false)]
        public virtual bool AllowApplyStyle
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

                    if (value && this.Area != null && this.Area.Chart != null)
                        this.Core.ApplyStyle(this.Area.Chart.Style);
                }
            }
        }

        /// <summary>
        /// Gets or sets visibility of axis.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets visibility of axis.")]
        [StiCategory("Common")]
        public virtual bool Visible { get; set; } = true;

        /// <summary>
        /// Gets or sets axis range settings.
        /// </summary>
		[StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        [Description("Gets or sets axis range settings.")]
        [StiCategory("Common")]
        public virtual IStiAxisRange Range { get; set; } = new StiAxisRange();

        [StiSerializable(StiSerializationVisibility.Reference)]
        [Browsable(false)]
        public IStiRadarArea Area { get; set; }
        #endregion

        public StiRadarAxis()
        {
        }

        public StiRadarAxis(
            bool visible,
            bool allowApplyStyle
            )
        {
            this.Visible = visible;
            this.allowApplyStyle = allowApplyStyle;
        }

        [StiUniversalConstructor("Axis")]
        public StiRadarAxis(
            IStiAxisRange range,
            bool visible,
            bool allowApplyStyle
            )
        {
            this.Range = range;
            this.Visible = visible;
            this.allowApplyStyle = allowApplyStyle;
        }
    }
}
