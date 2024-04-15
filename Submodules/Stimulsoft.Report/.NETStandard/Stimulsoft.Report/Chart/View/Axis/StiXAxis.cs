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
    public abstract class StiXAxis :
        StiAxis,
        IStiXAxis,
        IStiPropertyGridObject
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyBool(nameof(ShowEdgeValues), ShowEdgeValues);
            jObject.AddPropertyEnum(nameof(ShowXAxis), ShowXAxis, StiShowXAxis.Both);
            jObject.AddPropertyJObject(nameof(DateTimeStep), DateTimeStep.SaveToJsonObject(mode));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(ShowEdgeValues):
                        this.ShowEdgeValues = property.DeserializeBool();
                        break;

                    case nameof(ShowXAxis):
                        this.ShowXAxis = property.DeserializeEnum<StiShowXAxis>();
                        break;

                    case nameof(DateTimeStep):
                        this.DateTimeStep.LoadFromJsonObject((JObject)property.Value);
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public virtual StiComponentId ComponentId => StiComponentId.StiXAxis;

        [Browsable(false)]
        public string PropName => string.Empty;

        public virtual StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var objHelper = new StiPropertyCollection();

            objHelper.Add(StiPropertyCategories.Main, new[]
            {
                propertyGrid.PropertiesHelper.XAxis()
            });

            return objHelper;
        }

        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return null;
        }
        #endregion

        #region IStiDefault
        public override bool IsDefault
        {
            get
            {
                return
                    base.IsDefault
                    && !ShowEdgeValues
                    && !ShouldSerializeShowXAxis()
                    && !ShouldSerializeDateTimeStep();
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets value which indicates that first and last arguments on axis will be shown anyway.
        /// Always Show Edge Values (Added 2007.02.26)
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that first and last arguments on axis will be shown anyway.")]
        public bool ShowEdgeValues { get; set; }

        /// <summary>
        /// Gets or sets type of drawing X axis.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiShowXAxis.Both)]
        [Description("Gets or sets type of drawing X axis.")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        public virtual StiShowXAxis ShowXAxis { get; set; } = StiShowXAxis.Both;

        protected virtual bool ShouldSerializeShowXAxis()
        {
            return ShowXAxis != StiShowXAxis.Both;
        }

        /// <summary>
        /// Gets or sets date time step settings.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        [Description("Gets or sets date time step settings.")]
        public virtual IStiAxisDateTimeStep DateTimeStep { get; set; } = new StiAxisDateTimeStep();

        protected virtual bool ShouldSerializeDateTimeStep()
        {
            return DateTimeStep == null || !DateTimeStep.IsDefault;
        }
        #endregion

        public StiXAxis()
        {
        }

        public StiXAxis(
            IStiAxisLabels labels,
            IStiAxisRange range,
            IStiAxisTitle title,
            IStiAxisTicks ticks,
            StiArrowStyle arrowStyle,
            StiPenStyle lineStyle,
            Color lineColor,
            float lineWidth,
            bool visible,
            bool startFromZero,
            StiShowXAxis showXAxis,
            bool allowApplyStyle
            ) : this(labels, range, title, ticks, arrowStyle,
                    lineStyle, lineColor, lineWidth, visible,
                    startFromZero, showXAxis, false, allowApplyStyle)
        {
        }

        public StiXAxis(
            IStiAxisLabels labels,
            IStiAxisRange range,
            IStiAxisTitle title,
            IStiAxisTicks ticks,
            StiArrowStyle arrowStyle,
            StiPenStyle lineStyle,
            Color lineColor,
            float lineWidth,
            bool visible,
            bool startFromZero,
            StiShowXAxis showXAxis,
            bool showEdgeValues,
            bool allowApplyStyle)
            :

            base(
            labels,
            range,
            title,
            ticks,
            arrowStyle,
            lineStyle,
            lineColor,
            lineWidth,
            visible,
            startFromZero,
            allowApplyStyle)
        {
            this.ShowXAxis = showXAxis;
            this.ShowEdgeValues = showEdgeValues;
        }

        public StiXAxis(
            IStiAxisLabels labels,
            IStiAxisRange range,
            IStiAxisTitle title,
            IStiAxisTicks ticks,
            IStiAxisInteraction interaction,
            StiArrowStyle arrowStyle,
            StiPenStyle lineStyle,
            Color lineColor,
            float lineWidth,
            bool visible,
            bool startFromZero,
            StiShowXAxis showXAxis,
            bool showEdgeValues,
            bool allowApplyStyle)
            :

            base(
            labels,
            range,
            title,
            ticks,
            interaction,
            arrowStyle,
            lineStyle,
            lineColor,
            lineWidth,
            visible,
            startFromZero,
            allowApplyStyle)
        {
            this.ShowXAxis = showXAxis;
            this.DateTimeStep = new StiAxisDateTimeStep();
            this.ShowEdgeValues = showEdgeValues;
        }

        public StiXAxis(
            IStiAxisLabels labels,
            IStiAxisRange range,
            IStiAxisTitle title,
            IStiAxisTicks ticks,
            IStiAxisInteraction interaction,
            StiArrowStyle arrowStyle,
            StiPenStyle lineStyle,
            Color lineColor,
            float lineWidth,
            bool visible,
            bool startFromZero,
            StiShowXAxis showXAxis,
            bool showEdgeValues,
            bool allowApplyStyle,
            IStiAxisDateTimeStep dateTimeStep)
            :

            base(
            labels,
            range,
            title,
            ticks,
            interaction,
            arrowStyle,
            lineStyle,
            lineColor,
            lineWidth,
            visible,
            startFromZero,
            allowApplyStyle)
        {
            this.ShowXAxis = showXAxis;
            this.DateTimeStep = dateTimeStep;
            this.ShowEdgeValues = showEdgeValues;
        }

        public StiXAxis(
            IStiAxisLabels labels,
            IStiAxisRange range,
            IStiAxisTitle title,
            IStiAxisTicks ticks,
            IStiAxisInteraction interaction,
            StiArrowStyle arrowStyle,
            StiPenStyle lineStyle,
            Color lineColor,
            float lineWidth,
            bool visible,
            bool startFromZero,
            StiShowXAxis showXAxis,
            bool showEdgeValues,
            bool allowApplyStyle,
            IStiAxisDateTimeStep dateTimeStep,
            bool logarithmicScale)
            :

            base(
            labels,
            range,
            title,
            ticks,
            interaction,
            arrowStyle,
            lineStyle,
            lineColor,
            lineWidth,
            visible,
            startFromZero,
            allowApplyStyle,
            logarithmicScale)
        {
            this.ShowXAxis = showXAxis;
            this.DateTimeStep = dateTimeStep;
            this.ShowEdgeValues = showEdgeValues;
        }
    }
}
