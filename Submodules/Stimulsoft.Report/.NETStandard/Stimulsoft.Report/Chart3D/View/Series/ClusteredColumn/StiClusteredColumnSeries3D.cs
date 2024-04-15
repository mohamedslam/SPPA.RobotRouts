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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Json.Linq;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiClusteredColumnSeries3D :
        StiSeries3D,
        IStiClusteredColumnSeries3D
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyFloat(nameof(Width), width, 0.9f);
            jObject.AddPropertyFloat(nameof(Length), length, 0.9f);
            jObject.AddPropertyColor(nameof(BorderColor), BorderColor, Color.Gray);
            jObject.AddPropertyEnum(nameof(ColumnShape), ColumnShape);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Length):
                        this.length = property.DeserializeFloat();
                        break;

                    case nameof(Width):
                        this.width = property.DeserializeFloat();
                        break;

                    case nameof(BorderColor):
                        this.BorderColor = property.DeserializeColor();
                        break;

                    case nameof(ColumnShape):
                        this.ColumnShape = property.DeserializeEnum<StiColumnShape3D>();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiClusteredColumnSeries3D;
            }
        }

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // Value
            var list = new[]
            {
                propHelper.ValueDataColumn(),
                propHelper.Value(),
                propHelper.ListOfValues()
            };
            objHelper.Add(StiPropertyCategories.Value, list);

            // Argument
            list = new[]
            {
                propHelper.ArgumentDataColumn(),
                propHelper.Argument(),
                propHelper.ListOfArguments()
            };
            objHelper.Add(StiPropertyCategories.Argument, list);

            return objHelper;
        }
        #endregion

        #region Properties
        private float length = 0.9f;
        /// <summary>
        /// Gets or sets the width factor of one bar series. Value 1 is equal to 100%.
        /// </summary>
		[StiSerializable]
        [StiCategory("Common")]
        [DefaultValue(0.9f)]
        [Description("Gets or sets the width factor of one bar series. Value 1 is equal to 100%.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual float Length
        {
            get
            {
                return length;
            }
            set
            {
                if (value >= 0.01f && value <= 1f) length = value;
            }
        }

        private float width = 0.9f;
        /// <summary>
        /// Gets or sets the width factor of one bar series. Value 1 is equal to 100%.
        /// </summary>
		[StiSerializable]
        [StiCategory("Common")]
        [DefaultValue(0.9f)]
        [Description("Gets or sets the length factor of one bar series. Value 1 is equal to 100%.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual float Width
        {
            get
            {
                return width;
            }
            set
            {
                if (value >= 0.01f && value <= 1f) width = value;
            }
        }

        /// <summary>
        /// Gets or sets border color of series bar.
        /// </summary>
        [StiSerializable]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Appearance")]
        [Description("Gets or sets border color of series bar.")]
        [TypeConverter(typeof(Base.Drawing.Design.StiColorConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        public Color BorderColor { get; set; } = Color.Gray;

        /// <summary>
        /// Gets or sets border color of series bar.
        /// </summary>
		[StiSerializable]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Appearance")]
        [Description("Gets or sets border color of series bar.")]
        [TypeConverter(typeof(Base.Drawing.Design.StiColorConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        public Color Color { get; set; } = Color.DarkGray;

        [DefaultValue(StiColumnShape3D.Box)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [StiCategory("Appearance")]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiColumnShape3D ColumnShape { get; set; } = StiColumnShape3D.Box;
        #endregion

        #region Methods.Types
        public override Type GetDefaultAreaType()
        {
            return typeof(StiClusteredColumnArea3D);
        }
        #endregion

        #region Methods.override
        public override StiSeries CreateNew()
        {
            return new StiClusteredColumnSeries3D();
        }
        #endregion

        public StiClusteredColumnSeries3D()
        {
            SeriesLabels = new StiOutsideAxisLabels3D();
            Core = new StiClusteredColumnSeriesCoreXF3D(this);
        }
    }
}
