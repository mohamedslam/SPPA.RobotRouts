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
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Drawing;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(StiUniversalConverter))]
    public abstract class StiInterlacing :
        IStiInterlacing,
        ICloneable,
        IStiPropertyGridObject,
        IStiDefault
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyBool("AllowApplyStyle", allowApplyStyle, true);
            jObject.AddPropertyBrush("InterlacedBrush", InterlacedBrush);
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

                    case "InterlacedBrush":
                        this.InterlacedBrush = property.DeserializeBrush();
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
        public StiComponentId ComponentId => StiComponentId.StiInterlacing;

        [Browsable(false)]
        public string PropName => string.Empty;

        public StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var objHelper = new StiPropertyCollection();
            var propHelper = propertyGrid.PropertiesHelper;

            var list = new[]
            {
                propHelper.Interlacing()
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
            var interlacing = this.MemberwiseClone() as IStiInterlacing;
            interlacing.InterlacedBrush = this.InterlacedBrush.Clone() as StiBrush;

            if (this.Core != null)
            {
                interlacing.Core = this.Core.Clone() as StiInterlacingCoreXF;
                interlacing.Core.Interlacing = interlacing;
            }

            return interlacing;
        }

        [Browsable(false)]
        public StiInterlacingCoreXF Core { get; set; }

        private bool allowApplyStyle = true;
        /// <summary>
        /// Gets or sets value which indicates that chart style will be used.
        /// </summary>
        [StiSerializable]
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

                    if (value && Area?.Chart != null)
                        Core.ApplyStyle(Area.Chart.Style);
                }
            }
        }

        /// <summary>
        /// Gets or sets brush which used for drawing interlaced bars.
        /// </summary>
		[StiSerializable]
        [Description("Gets or sets brush which used for drawing interlacing bar.")]
        public StiBrush InterlacedBrush { get; set; } = new StiSolidBrush(Color.Transparent);

        private bool ShouldSerializeInterlacedBrush()
        {
            return !(InterlacedBrush is StiSolidBrush && ((StiSolidBrush)InterlacedBrush).Color == Color.Transparent);
        }

        /// <summary>
        /// Gets or sets visibility of interlaced bars.
        /// </summary>
		[StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets visibility of interlaced bars.")]
        public bool Visible { get; set; } = true;

        [StiSerializable(StiSerializationVisibility.Reference)]
        [Browsable(false)]
        public IStiArea Area { get; set; }
        #endregion

        #region IStiDefault
        [Browsable(false)]
        public bool IsDefault
        {
            get
            {
                //We specially don't check other properties because AllowApplyStyle should be checked only!
                return AllowApplyStyle;
            }
        }
        #endregion

        public StiInterlacing()
        {
            this.Core = new StiInterlacingCoreXF(this);
        }

        public StiInterlacing(StiBrush interlacedBrush, bool visible, bool allowApplyStyle)
        {
            this.InterlacedBrush = interlacedBrush;
            this.Visible = visible;
            this.allowApplyStyle = allowApplyStyle;

            this.Core = new StiInterlacingCoreXF(this);
        }
    }
}
