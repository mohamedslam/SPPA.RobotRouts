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
using Stimulsoft.Report.Gauge.Helpers;
using Stimulsoft.Report.Painters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components.Gauge.Primitives
{
    public abstract class StiRadialTickLabelBase :
        StiTickLabelBase
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);
                        
            jObject.AddPropertyFloat(nameof(OffsetAngle), OffsetAngle);
            jObject.AddPropertyEnum(nameof(LabelRotationMode), LabelRotationMode);

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

                    case nameof(LabelRotationMode):
                        this.LabelRotationMode = property.DeserializeEnum<StiLabelRotationMode>();
                        break;
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the rotation mode of labels. 
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiLabelRotationMode.None)]
        [StiCategory("TextAdditional")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the rotation mode of labels.")]
        public StiLabelRotationMode LabelRotationMode { get; set; } = StiLabelRotationMode.None;

        /// <summary>
        /// Gets or sets an additional rotation angle.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0f)]
        [StiCategory("TextAdditional")]
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

            var rect = this.Scale.barGeometry.RectGeometry;
            if (rect.Width <= 0 || rect.Height <= 0) return;

            var collection = GetPointCollection();
            if (collection.Count == 0) return;

            #region remove unnecessary items 
            var minSize = Math.Min(rect.Width * StiScale.System, rect.Height * StiScale.System);
            if (minSize < 100) return;
            #endregion

            var collectionPrepare = StiTickLabelHelper.GetLabels(collection, radialScale);

            #region Temporary Variables
            string textFormat = this.TextFormat;

            float startValue = this.Scale.ScaleHelper.ActualMinimum;
            float endValue = this.Scale.ScaleHelper.ActualMaximum;

            var centerPoint = radialScale.barGeometry.Center;
            float radiusMain = Scale.barGeometry.Radius;
            float diameterMain = Scale.barGeometry.Diameter;
            float sweepAngle = radialScale.GetSweepAngle();
            float startAngle = radialScale.StartAngle;

            float maxWidth = this.Scale.ScaleHelper.MaxWidth;
            float minWidth = this.Scale.ScaleHelper.MinWidth;

            maxWidth *= diameterMain;
            minWidth *= diameterMain;
            float restWidth = maxWidth - minWidth;

            float actualRadius = (this.Placement == StiPlacement.Outside) 
                ? (radiusMain * (1 + this.Offset)) 
                : (radiusMain * (1 - this.Offset));

            var skipValues = base.SkipValuesObj;
            var skipIndices = base.SkipIndicesObj;
            #endregion

            int maxLength = 1;
            var cacheText = new Dictionary<float, string>();
            foreach (float key in collection.Keys)
            {
                string text;

                if (radialScale.DateTimeMode)
                {
                    text = collectionPrepare[key];
                }
                else
                {
                    if (this.FormatService != null)
                    {
                        text = this.FormatService.Format(key);
                    }
                    else
                    {
                        text = (string.IsNullOrEmpty(textFormat) && this.Scale.Gauge.ShortValue)
                            ? collectionPrepare[key]
                            : GetTextForRender(key, textFormat);
                    }
                }

                cacheText.Add(key, text);

                if (text.Length > 0)
                {
                    if (maxLength < text.Length)
                        maxLength = text.Length;
                }
            }

            if (maxLength < 3)
                maxLength = 3;

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

                string text = cacheText[key];

                var zoomFont = StiGaugeContextPainter.ChangeFontSize(this.Font, context.Zoom);

                string lastText = null;
                if (text.Length < maxLength)
                {
                    lastText = text;

                    int count3 = maxLength - text.Length;
                    for (int index3 = 0; index3 < count3; index3++)
                    {
                        text += "0";
                    }
                }

                var textSize = context.MeasureString(text, zoomFont);
                if (lastText != null)
                    text = lastText;

                float angle = (Scale.IsReversed) ?
                    (startAngle + sweepAngle - collection[key] * sweepAngle + this.OffsetAngle) :
                    (startAngle + collection[key] * sweepAngle - this.OffsetAngle);

                float currentRadius = actualRadius;

                PointF point;
                int countMatrix;

                if (this.Placement == StiPlacement.Outside)
                {
                    countMatrix = GetMatrixRotation(context, centerPoint, textSize, this.LabelRotationMode, currentRadius, angle, out point);
                }
                else
                {
                    float radius;

                    if (this.Placement == StiPlacement.Overlay)
                    {
                        radius = (Scale.IsUp)
                            ? currentRadius - ((minWidth + restWidth * collection[key]) / 2) - textSize.Width / 2
                            : currentRadius - ((maxWidth - restWidth * collection[key]) / 2) - textSize.Width / 2;
                    }
                    else
                    {
                        radius = (Scale.IsUp)
                            ? currentRadius - minWidth - restWidth * collection[key] - textSize.Width
                            : currentRadius - maxWidth + restWidth * collection[key] - textSize.Width;
                    }

                    countMatrix = GetMatrixRotation(context, centerPoint, textSize, this.LabelRotationMode, radius, angle, out point);
                }

                context.AddTextGaugeGeom(text, zoomFont, this.TextBrush, new RectangleF(point, textSize), null);
                int index1 = -1;
                while(++index1 < countMatrix)
                {
                    context.AddPopTranformGaugeGeom();
                }
            }
        }

        protected int GetMatrixRotation(StiGaugeContextPainter context, PointF centerPoint, SizeF textSize, StiLabelRotationMode rotateMode, float radius, float angle, out PointF position)
        {
            float angle1 = 0f;

            if (rotateMode == StiLabelRotationMode.Automatic)
            {
                switch (GetRadialPosition(angle))
                {
                    case StiRadialPosition.TopCenter:
                    case StiRadialPosition.TopRight:
                    case StiRadialPosition.TopLeft:
                        angle1 = 90;
                        break;

                    case StiRadialPosition.BottomRight:
                    case StiRadialPosition.LeftCenter:
                    case StiRadialPosition.BottonLeft:
                        angle1 = -90;
                        break;

                    case StiRadialPosition.BottomCenter:
                        angle1 = -angle;
                        break;

                    case StiRadialPosition.RightCenter:
                        angle1 = -angle - 90;
                        break;
                }
            }
            else if (rotateMode == StiLabelRotationMode.None)
            {
                angle1 = -angle;
            }
            else if (rotateMode == StiLabelRotationMode.SurroundIn)
            {
                switch (GetRadialPosition(angle))
                {
                    case StiRadialPosition.TopCenter:
                        angle1 = -angle - 180;
                        break;

                    default:
                        angle1 = -90;
                        break;
                }
            }
            else
            {
                angle1 = 90;
            }

            position = new PointF(centerPoint.X + radius, centerPoint.Y - (textSize.Height / 2));

            int countMatrix = 0;
            if (angle != 0)
            {
                context.AddPushMatrixGaugeGeom(angle, centerPoint);
                countMatrix++;
            }
            if (angle1 != 0)
            {
                context.AddPushMatrixGaugeGeom(angle1, new PointF(position.X + textSize.Width / 2, position.Y + textSize.Height / 2));
                countMatrix++;
            }

            return countMatrix;
        }

        private StiRadialPosition GetRadialPosition(float angle)
        {
            angle += 90;
            while (angle > 360)
            {
                angle -= 360;
            }

            if (angle == 0 || angle == 360)
                return StiRadialPosition.TopCenter;
            else if (angle > 0 && angle < 90)
                return StiRadialPosition.TopRight;
            else if (angle == 90)
                return StiRadialPosition.RightCenter;
            else if (angle > 90 && angle < 180)
                return StiRadialPosition.BottomRight;
            else if (angle == 180)
                return StiRadialPosition.BottomCenter;
            else if (angle > 180 && angle < 270)
                return StiRadialPosition.BottonLeft;
            else if (angle == 270)
                return StiRadialPosition.LeftCenter;
            else
                return StiRadialPosition.TopLeft;
        }
        #endregion
    }
}