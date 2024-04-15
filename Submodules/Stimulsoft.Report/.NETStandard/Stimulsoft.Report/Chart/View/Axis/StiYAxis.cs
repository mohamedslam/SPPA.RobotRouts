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
    public abstract class StiYAxis :
        StiAxis,
        IStiYAxis,
        IStiPropertyGridObject
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyEnum(nameof(ShowYAxis), ShowYAxis, StiShowYAxis.Both);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(ShowYAxis):
                        this.ShowYAxis = property.DeserializeEnum<StiShowYAxis>();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public virtual StiComponentId ComponentId => StiComponentId.StiYAxis;

        [Browsable(false)]
        public string PropName => string.Empty;

        public virtual StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var objHelper = new StiPropertyCollection();

            objHelper.Add(StiPropertyCategories.Main, new[]
            {
                propertyGrid.PropertiesHelper.StiYAxis()
            });

            return objHelper;
        }

        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return null;
        }
        #endregion

        #region IStiDefault
        public override bool IsDefault => base.IsDefault && !ShouldSerializeShowYAxis();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets type of drawing Y axis.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiShowYAxis.Both)]
        [Description("Gets or sets type of drawing Y axis.")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        public virtual StiShowYAxis ShowYAxis { get; set; } = StiShowYAxis.Both;

        protected virtual bool ShouldSerializeShowYAxis()
        {
            return ShowYAxis == StiShowYAxis.Both;
        }
        #endregion

        public StiYAxis()
        {
        }

        public StiYAxis(
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
            StiShowYAxis showYAxis,
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
            this.ShowYAxis = showYAxis;
        }

        public StiYAxis(
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
            StiShowYAxis showYAxis,
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
            this.ShowYAxis = showYAxis;
        }

        public StiYAxis(
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
            StiShowYAxis showYAxis,
            bool allowApplyStyle,
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
            this.ShowYAxis = showYAxis;
        }
    }
}
