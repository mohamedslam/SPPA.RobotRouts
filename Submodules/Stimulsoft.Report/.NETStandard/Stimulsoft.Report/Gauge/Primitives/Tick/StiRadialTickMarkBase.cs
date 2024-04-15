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
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Painters;
using System.ComponentModel;
using System.Drawing;

namespace Stimulsoft.Report.Components.Gauge.Primitives
{
    public abstract class StiRadialTickMarkBase :
        StiTickMarkBase
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);
            
            jObject.AddPropertyFloat(nameof(OffsetAngle), OffsetAngle);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(OffsetAngle):
                        this.OffsetAngle = property.DeserializeFloat();
                        break;                        
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets an additional rotation angle.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0f)]
        [StiCategory("Tick")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets an additional rotation angle.")]
        public float OffsetAngle { get; set; } = 0f;
        #endregion

        #region Properties override
        public override StiGaugeElemenType ElementType => StiGaugeElemenType.RadialElement;
        #endregion

        #region Methods
        protected internal override void DrawElement(StiGaugeContextPainter context)
        {
            var radialScale = this.Scale as StiRadialScale;
            if (radialScale == null) return;

            var rect = Scale.barGeometry.RectGeometry;
            if (rect.Width <= 0 || rect.Height <= 0) return;
            var collection = GetPointCollection();
            if (collection.Count == 0) return;

            #region Temporary Variables
            var skipValues = base.SkipValuesObj;
            var skipIndices = base.SkipIndicesObj;
            var skin = this.GetActualSkin();

            var centerPoint = radialScale.barGeometry.Center;
            float sweepAngle = radialScale.GetSweepAngle();
            float startAngle = radialScale.StartAngle;

            float startValue = this.Scale.ScaleHelper.ActualMinimum;
            float endValue = this.Scale.ScaleHelper.ActualMaximum;

            float diameter = Scale.barGeometry.Diameter;
            float radius = Scale.barGeometry.Radius;
            float minWidth = this.Scale.ScaleHelper.MinWidth;
            float maxWidth = this.Scale.ScaleHelper.MaxWidth;
            float restWidth;

            maxWidth *= Scale.barGeometry.RectGeometry.Width;
            minWidth *= Scale.barGeometry.RectGeometry.Width;
            restWidth = maxWidth - minWidth;

            radius = (this.Placement == StiPlacement.Outside) ? (radius * (1 + this.Offset)) : (radius * (1 - this.Offset));

            float tickWidth = diameter * this.RelativeWidth;
            float tickHeight = diameter * this.RelativeHeight;
            #endregion

            int index = -1;
            foreach (float key in collection.Keys)
            {
                index++;

                #region Check Value
                if (key < startValue) continue;
                if (key > endValue) continue;
                if (CheckTickValue(skipValues, skipIndices, key, index)) continue;
                if (this.MinimumValue != null && key < this.MinimumValue.Value) continue;
                if (this.MaximumValue != null && key > this.MaximumValue.Value) continue;
                #endregion

                float tickOffset = sweepAngle * GetRelativeHeight(this.RelativeHeight) / 4;
                float angle = (Scale.IsReversed) 
                    ? (startAngle + sweepAngle - collection[key] * sweepAngle + this.OffsetAngle) + tickOffset
                    : (startAngle + collection[key] * sweepAngle + this.OffsetAngle) - tickOffset;
                float currentRadius;

                if (this.Placement == StiPlacement.Outside)
                {
                    currentRadius = radius;
                }
                else if (this.Placement == StiPlacement.Overlay)
                {
                    if (Scale.IsUp)
                    {
                        currentRadius = radius - ((minWidth + restWidth * collection[key] + tickWidth) / 2);
                    }
                    else
                    {
                        currentRadius = radius - ((maxWidth - restWidth * collection[key] + tickWidth) / 2);
                    }
                }
                else
                {
                    if (Scale.IsUp)
                    {
                        currentRadius = radius - minWidth - (restWidth * collection[key]) - tickWidth;
                    }
                    else
                    {
                        currentRadius = radius - maxWidth + (restWidth * collection[key]) - tickWidth;
                    }
                }

                var tickRect = new RectangleF(centerPoint.X + currentRadius, centerPoint.Y, tickWidth, tickHeight);
                context.AddPushMatrixGaugeGeom(angle, centerPoint);
                skin.Draw(context, this, tickRect);
                context.AddPopTranformGaugeGeom();
            }
        }
        #endregion
    }
}
