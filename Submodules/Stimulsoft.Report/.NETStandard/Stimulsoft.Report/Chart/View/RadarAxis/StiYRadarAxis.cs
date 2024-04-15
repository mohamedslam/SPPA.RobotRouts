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
using System.Drawing.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(StiUniversalConverter))]
    public class StiYRadarAxis :
        StiRadarAxis,
        IStiYRadarAxis,
        ICloneable,
        IStiPropertyGridObject
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyJObject("Labels", Labels.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Ticks", Ticks.SaveToJsonObject(mode));
            jObject.AddPropertyEnum("LineStyle", LineStyle, StiPenStyle.Solid);
            jObject.AddPropertyColor("LineColor", LineColor, Color.Gray);
            jObject.AddPropertyFloat("LineWidth", LineWidth, 1f);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Labels":
                        this.Labels.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "Ticks":
                        this.Ticks.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "LineStyle":
                        this.LineStyle = property.DeserializeEnum<StiPenStyle>();
                        break;

                    case "LineColor":
                        this.LineColor = property.DeserializeColor();
                        break;

                    case "LineWidth":
                        this.LineWidth = property.DeserializeFloat();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public StiComponentId ComponentId => StiComponentId.StiYRadarAxis;

        [Browsable(false)]
        public string PropName => string.Empty;

        public StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var objHelper = new StiPropertyCollection();
            var propHelper = propertyGrid.PropertiesHelper;

            var list = new[]
            {
                propHelper.YRadarAxis()
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
        public override object Clone()
        {
            var axis = base.Clone() as IStiYRadarAxis;

            axis.LineStyle = this.LineStyle;
            axis.Labels = this.Labels.Clone() as IStiAxisLabels;
            axis.Ticks = this.Ticks.Clone() as IStiAxisTicks;

            return axis;
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public StiYRadarAxisCoreXF YCore => Core as StiYRadarAxisCoreXF;
        
        [Browsable(true)]
        public override bool AllowApplyStyle
        {
            get
            {
                return base.AllowApplyStyle;
            }
            set
            {
                base.AllowApplyStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets axis labels settings.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        [StiCategory("Common")]
        public IStiAxisLabels Labels { get; set; } = new StiAxisLabels();

        /// <summary>
        /// Gets or sets ticks settings.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        [StiCategory("Common")]
        public IStiAxisTicks Ticks { get; set; } = new StiAxisTicks();

        /// <summary>
        /// Gets or sets line style of axis.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiPenStyle.Solid)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Common")]
        public StiPenStyle LineStyle { get; set; } = StiPenStyle.Solid;

        /// <summary>
        /// Gets or sets line color which used to draw axis.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Appearance")]
        public Color LineColor { get; set; } = Color.Gray;

        /// <summary>
        /// Gets or sets line width which used to draw axis.
        /// </summary>
        [DefaultValue(1f)]
        [StiSerializable]
        [StiCategory("Common")]
        public float LineWidth { get; set; } = 1f;

        [Browsable(false)]
        public StiAxisInfoXF Info { get; set; } = new StiAxisInfoXF();
        #endregion

        public StiYRadarAxis() : base()
        {
            this.Core = new StiYRadarAxisCoreXF(this);
        }

        public StiYRadarAxis(
            IStiAxisLabels labels,
            IStiAxisTicks ticks,
            StiPenStyle lineStyle,
            Color lineColor,
            float lineWidth,
            bool visible,
            bool allowApplyStyle) :
            base
            (
                visible,
                allowApplyStyle
            )
        {
            this.Core = new StiYRadarAxisCoreXF(this);

            this.Labels = labels;
            this.Ticks = ticks;
            this.LineStyle = lineStyle;
            this.LineColor = lineColor;
            this.LineWidth = lineWidth;
        }

        [StiUniversalConstructor("Axis")]
        public StiYRadarAxis(
            IStiAxisLabels labels,
            IStiAxisTicks ticks,
            StiPenStyle lineStyle,
            IStiAxisRange range,
            Color lineColor,
            float lineWidth,
            bool visible,
            bool allowApplyStyle) :
            base
            (
                range,
                visible,
                allowApplyStyle
            )
        {
            this.Core = new StiYRadarAxisCoreXF(this);

            this.Range = range;
            this.Labels = labels;
            this.Ticks = ticks;
            this.LineStyle = lineStyle;
            this.LineColor = lineColor;
            this.LineWidth = lineWidth;
        }
    }
}
