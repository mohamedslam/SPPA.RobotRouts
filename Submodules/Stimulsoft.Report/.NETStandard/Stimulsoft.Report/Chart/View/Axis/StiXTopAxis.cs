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
    public class StiXTopAxis :
        StiXAxis,
        IStiXTopAxis
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty(nameof(Range));
            jObject.RemoveProperty(nameof(ShowXAxis));
            jObject.RemoveProperty(nameof(DateTimeStep));

            jObject.AddPropertyBool(nameof(Visible), Visible);

            return jObject;
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiXTopAxis;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var objHelper = new StiPropertyCollection();

            objHelper.Add(StiPropertyCategories.Main, new[]
            {
                propertyGrid.PropertiesHelper.XTopAxis()
            });

            return objHelper;
        }
        #endregion

        #region Properties
        [Browsable(false)]
        [StiNonSerialized]
        public override IStiAxisRange Range
        {
            get
            {
                return base.Range;
            }
            set
            {
                base.Range = value;
            }
        }

        protected override bool ShouldSerializeRange()
        {
            return false;
        }

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
        public override StiShowXAxis ShowXAxis
        {
            get
            {
                return base.ShowXAxis;
            }
            set
            {
                base.ShowXAxis = value;
            }
        }

        protected override bool ShouldSerializeShowXAxis()
        {
            return false;
        }

        [Browsable(false)]
        [StiNonSerialized]
        public override IStiAxisDateTimeStep DateTimeStep
        {
            get
            {
                return base.DateTimeStep;
            }
            set
            {
                base.DateTimeStep = value;
            }
        }

        protected override bool ShouldSerializeDateTimeStep()
        {
            return false;
        }
        #endregion

        public StiXTopAxis()
        {
            Visible = false;
            this.Core = new StiXTopAxisCoreXF(this);
        }

        public StiXTopAxis(
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
            bool allowApplyStyle
            ) :
            this(
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
            false,
            allowApplyStyle)
        {
        }

        public StiXTopAxis(
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
            bool showEdgeValues,
            bool allowApplyStyle
            )
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
            StiShowXAxis.Both,
            showEdgeValues,
            allowApplyStyle)
        {
            this.Core = new StiXTopAxisCoreXF(this);
        }

        public StiXTopAxis(
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
            bool showEdgeValues,
            bool allowApplyStyle
            )
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
            StiShowXAxis.Both,
            showEdgeValues,
            allowApplyStyle)
        {
            this.Core = new StiXTopAxisCoreXF(this);
        }

        [StiUniversalConstructor("XTopAxis")]
        public StiXTopAxis(
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
            bool showEdgeValues,
            bool allowApplyStyle,
            bool logarithmicScale
            ) :

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
            StiShowXAxis.Both,
            showEdgeValues,
            allowApplyStyle,
            new StiAxisDateTimeStep(),
            logarithmicScale)
        {
            this.Core = new StiXTopAxisCoreXF(this);
        }
    }
}
