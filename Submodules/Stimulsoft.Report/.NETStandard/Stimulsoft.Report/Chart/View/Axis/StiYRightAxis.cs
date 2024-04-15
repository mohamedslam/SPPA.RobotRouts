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
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.PropertyGrid;
using System.ComponentModel;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiYRightAxis :
        StiYAxis,
        IStiYRightAxis
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.RemoveProperty(nameof(ShowYAxis));

            jObject.AddPropertyBool(nameof(Visible), Visible);

            return jObject;
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiYRightAxis;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var objHelper = new StiPropertyCollection();

            objHelper.Add(StiPropertyCategories.Main, new[]
            {
                propertyGrid.PropertiesHelper.YRightAxis()
            });

            return objHelper;
        }
        #endregion

        #region Properties
        [DefaultValue(false)]
        public override bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                base.Visible = value;
            }
        }

        protected override bool ShouldSerializeVisible()
        {
            return false;
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override StiShowYAxis ShowYAxis
        {
            get
            {
                return base.ShowYAxis;
            }
            set
            {
                base.ShowYAxis = value;
            }
        }

        protected override bool ShouldSerializeShowYAxis()
        {
            return false;
        }
        #endregion

        public StiYRightAxis()
        {
            Visible = false;
            Labels.TextAlignment = StiHorAlignment.Left;

            this.Core = new StiYRightAxisCoreXF(this);
        }

        public StiYRightAxis(
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
            StiShowYAxis.Both,
            allowApplyStyle)
        {
            this.Labels.TextAlignment = StiHorAlignment.Left;
            this.Core = new StiYRightAxisCoreXF(this);
        }

        public StiYRightAxis(
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
            StiShowYAxis.Both,
            allowApplyStyle)
        {
            this.Labels.TextAlignment = StiHorAlignment.Left;
            this.Core = new StiYRightAxisCoreXF(this);
        }

        [StiUniversalConstructor("YRightAxis")]
        public StiYRightAxis(
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
            StiShowYAxis.Both,
            allowApplyStyle,
            logarithmicScale)
        {
            this.Labels.TextAlignment = StiHorAlignment.Left;
            this.Core = new StiYRightAxisCoreXF(this);
        }
    }
}
