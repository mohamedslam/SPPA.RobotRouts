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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    /// <summary>
    /// Describes base class for all chart areas.
    /// </summary>
    [StiServiceBitmap(typeof(StiArea), "Stimulsoft.Report.Images.Components.StiChart.png")]
    [StiServiceCategoryBitmap(typeof(StiArea), "Stimulsoft.Report.Images.Components.StiChart.png")]
    [TypeConverter(typeof(StiUniversalConverter))]
    public abstract class StiArea :
        StiService,
        IStiArea,
        IStiSerializeToCodeAsClass,
        IStiPropertyGridObject
    {
        #region IStiJsonReportObject.override
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyIdent("Ident", GetType().Name);
            jObject.AddPropertyBool(nameof(AllowApplyStyle), allowApplyStyle, true);
            jObject.AddPropertyBool(nameof(ColorEach), ColorEach);
            jObject.AddPropertyBool(nameof(ShowShadow), ShowShadow);
            jObject.AddPropertyColor(nameof(BorderColor), BorderColor, Color.Gray);
            jObject.AddPropertyFloat(nameof(BorderThickness), BorderThickness, 1);
            jObject.AddPropertyBrush(nameof(Brush), Brush);

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(AllowApplyStyle):
                        this.allowApplyStyle = property.DeserializeBool();
                        break;

                    case nameof(ColorEach):
                        this.ColorEach = property.DeserializeBool();
                        break;

                    case nameof(ShowShadow):
                        this.ShowShadow = property.DeserializeBool();
                        break;

                    case nameof(BorderColor):
                        this.BorderColor = property.DeserializeColor();
                        break;

                    case nameof(Brush):
                        this.Brush = property.DeserializeBrush();
                        break;

                    case nameof(BorderThickness):
                        this.BorderThickness = property.DeserializeInt();
                        break;
                }
            }
        }

        internal static IStiArea CreateFromJsonObject(JObject jObject)
        {
            var ident = jObject.Properties().FirstOrDefault(x => x.Name == "Ident").Value.ToObject<string>();
            var service = StiOptions.Services.ChartAreas.FirstOrDefault(x => x.GetType().Name == ident);

            if (service == null)
                throw new Exception($"Type {ident} is not found!");

            var area = service.CreateNew();
            area.LoadFromJsonObject(jObject);

            return area;
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public abstract StiComponentId ComponentId { get; }

        [Browsable(false)]
        public string PropName => string.Empty;

        public abstract StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level);

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
        public override object Clone()
        {
            var area = base.Clone() as IStiArea;
            area.Brush = this.Brush.Clone() as StiBrush;

            if (this.Core != null)
            {
                area.Core = this.Core.Clone() as StiAreaCoreXF;
                area.Core.Area = area;
            }

            return area;
        }
        #endregion

        #region Methods
        public abstract StiArea CreateNew();

        public override string ToString()
        {
            return ServiceName;
        }
        #endregion

        #region Methods.Types
        public abstract Type GetDefaultSeriesType();

        public abstract Type[] GetSeriesTypes();

        public abstract Type GetDefaultSeriesLabelsType();

        public abstract Type[] GetSeriesLabelsTypes();
        #endregion

        #region StiService override
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string ServiceName => Core.LocalizedName;

        /// <summary>
        /// Gets a service category.
        /// </summary>
        [Browsable(false)]
        public sealed override string ServiceCategory => "Chart";

        /// <summary>
        /// Gets a service type.
        /// </summary>
        [Browsable(false)]
        public sealed override Type ServiceType => typeof(StiArea);
        #endregion

        #region Properties
        [Browsable(false)]
        public bool IsDefaultSeriesTypeFullStackedColumnSeries => GetDefaultSeriesType() == typeof(StiFullStackedColumnSeries);
        
        [Browsable(false)]
        public bool IsDefaultSeriesTypeFullStackedBarSeries => GetDefaultSeriesType() == typeof(StiFullStackedBarSeries);

        [Browsable(false)]
        public StiAreaCoreXF Core { get; set; }

        /// <summary>
        /// Gets or sets reference to chart component which contain this area.
        /// </summary>
        [Browsable(false)]
        [StiSerializable(StiSerializationVisibility.Reference)]
        public virtual IStiChart Chart { get; set; }

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

                    if (value && Chart != null)
                        this.Core.ApplyStyle(this.Chart.Style);
                }
            }
        }

        /// <summary>
        /// Gets or sets value which indicates that each series is drawn by its own colour.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that each series is drawn by its own colour.")]
        public virtual bool ColorEach { get; set; }

        [StiNonSerialized]
        [Browsable(false)]
        public virtual bool ColorEachAllowed => true;

        /// <summary>
        /// Gets or sets value which indicates necessary draw shadod or no.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates necessary draw shadod or no.")]
        public bool ShowShadow { get; set; }

        /// <summary>
        /// Gets or sets border color of this area.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets border color of this area.")]
        public Color BorderColor { get; set; } = Color.Gray;

        private float borderThickness = 1f;
        /// <summary>
        /// Gets or sets border thickness of this area.
        /// </summary>
		[StiSerializable]
        [Description("Gets or sets border thickness of this area.")]
        public float BorderThickness 
        {
            get
            {
                return borderThickness;
            }
            set
            {
                if (value > 0)
                {
                    borderThickness = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets brush to fill a area.
        /// </summary>
        [StiSerializable]
        [Description("Gets or sets a brush to fill a area.")]
        public StiBrush Brush { get; set; } = new StiSolidBrush(Color.White);
        #endregion
    }
}
