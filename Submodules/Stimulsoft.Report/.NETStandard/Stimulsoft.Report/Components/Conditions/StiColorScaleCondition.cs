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
using Stimulsoft.Report.Dictionary;

using System;
using System.ComponentModel;
using System.Drawing;

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// The class describes the condition with Color Scale indicator.
    /// </summary>
    [RefreshProperties(RefreshProperties.All)]
    [TypeConverter(typeof(Stimulsoft.Report.Components.Design.StiConditionConverter))]
    public class StiColorScaleCondition :
        StiBaseCondition,
        IStiIndicatorCondition
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiColorScaleCondition
            jObject.AddPropertyEnum("ScaleType", ScaleType, StiColorScaleType.Color2);
            jObject.AddPropertyColor("MinimumColor", MinimumColor, Color.Red);
            jObject.AddPropertyColor("MidColor", MidColor, Color.Yellow);
            jObject.AddPropertyColor("MaximumColor", MaximumColor, Color.Green);
            jObject.AddPropertyEnum("MinimumType", MinimumType, StiMinimumType.Auto);
            jObject.AddPropertyFloat("MinimumValue", MinimumValue);
            jObject.AddPropertyEnum("MidType", MidType, StiMidType.Auto);
            jObject.AddPropertyFloat("MidValue", MidValue, 50f);
            jObject.AddPropertyEnum("MaximumType", MaximumType, StiMaximumType.Auto);
            jObject.AddPropertyFloat("MaximumValue", MaximumValue, 100f);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "ScaleType":
                        this.ScaleType = property.DeserializeEnum<StiColorScaleType>();
                        break;

                    case "MinimumColor":
                        this.MinimumColor = property.DeserializeColor();
                        break;

                    case "MidColor":
                        this.MidColor = property.DeserializeColor();
                        break;

                    case "MaximumColor":
                        this.MaximumColor = property.DeserializeColor();
                        break;

                    case "MinimumType":
                        this.MinimumType = property.DeserializeEnum<StiMinimumType>();
                        break;

                    case "MinimumValue":
                        this.MinimumValue = property.DeserializeFloat();
                        break;

                    case "MidType":
                        this.MidType = property.DeserializeEnum<StiMidType>();
                        break;

                    case "MidValue":
                        this.MidValue = property.DeserializeFloat();
                        break;

                    case "MaximumType":
                        this.MaximumType = property.DeserializeEnum<StiMaximumType>();
                        break;

                    case "MaximumValue":
                        this.MaximumValue = property.DeserializeFloat();
                        break;
                }
            }
        }
        #endregion

        #region IStiIndicatorCondition
        /// <summary>
        /// Creates new indicator for specified condition.
        /// </summary>
        public StiIndicator CreateIndicator(StiText component)
        {
            #region Column is not specified
            if (string.IsNullOrEmpty(this.Column))
            {
                component.Report.WriteToReportRenderingMessages($"Column of Color Scale Condition of '{component.Name}' component is not specified!");
                return null;
            }
            #endregion

            if (minimum == null || maximum == null)
            {
                minimum = 0;
                maximum = 0;

                #region Process DataSource
                StiDataSource dataSource = StiDataColumn.GetDataSourceFromDataColumn(component.Report.Dictionary, Column);
                if (dataSource != null)
                {
                    dataSource.SaveState("Indicator");
                    dataSource.First();

                    int index = 0;
                    while (!dataSource.IsEof)
                    {
                        object value = StiDataColumn.GetDataFromDataColumn(component.Report.Dictionary, Column);
                        float floatValue = 0;
                        try
                        {
                            floatValue = (float) StiReport.ChangeType(value, typeof(float));
                        }
                        catch
                        {
                        }

                        if (index == 0)
                        {
                            minimum = floatValue;
                            maximum = floatValue;
                        }
                        else
                        {
                            minimum = Math.Min(minimum.GetValueOrDefault(), floatValue);
                            maximum = Math.Max(maximum.GetValueOrDefault(), floatValue);
                        }

                        index++;
                        dataSource.Next();
                    }

                    dataSource.RestoreState("Indicator");
                }
                #endregion

                #region Process Business Object
                if (dataSource == null)
                {
                    StiBusinessObject businessObject = StiDataColumn.GetBusinessObjectFromDataColumn(component.Report.Dictionary, Column);
                    if (businessObject == null)
                    {
                        component.Report.WriteToReportRenderingMessages($"Column of Data Bar Condition of '{component.Name}' component is not found!");
                        return null;
                    }

                    businessObject.SaveState("Indicator");
                    businessObject.CreateEnumerator();

                    int index = 0;
                    while (!businessObject.IsEof)
                    {
                        object value = StiDataColumn.GetDataFromDataColumn(component.Report.Dictionary, Column);
                        float floatValue = 0;
                        try
                        {
                            floatValue = (float) StiReport.ChangeType(value, typeof(float));
                        }
                        catch
                        {
                        }

                        if (index == 0)
                        {
                            minimum = floatValue;
                            maximum = floatValue;
                        }
                        else
                        {
                            minimum = Math.Min(minimum.GetValueOrDefault(), floatValue);
                            maximum = Math.Max(maximum.GetValueOrDefault(), floatValue);
                        }

                        index++;
                        businessObject.Next();
                    }

                    businessObject.RestoreState("Indicator");
                }
                #endregion
            }

            float minimumToUse = this.minimum.GetValueOrDefault();
            float maximumToUse = this.maximum.GetValueOrDefault();

            float dist = maximumToUse - minimumToUse;

            float minimumPercent = this.MinimumValue;
            minimumPercent = Math.Min(minimumPercent, 100f);
            minimumPercent = Math.Max(minimumPercent, 0f);

            float maximumPercent = this.MaximumValue;
            maximumPercent = Math.Min(maximumPercent, 100f);
            maximumPercent = Math.Max(maximumPercent, 0f);

            if (minimumPercent > maximumPercent)
                minimumPercent = maximumPercent;

            float midPercent = this.MidValue;

            #region Process RangeType for Minimum
            if (this.MinimumType == StiMinimumType.Value)
                minimumToUse = this.MinimumValue;

            else if (this.MinimumType == StiMinimumType.Percent)
                minimumToUse = minimumToUse + dist * minimumPercent / 100f;
            #endregion

            #region Process RangeType for Maximum
            if (this.MaximumType == StiMaximumType.Value)
                maximumToUse = this.MaximumValue;

            else if (this.MaximumType == StiMaximumType.Percent)
                maximumToUse = minimumToUse + dist * maximumPercent / 100f;
            #endregion

            #region Process RangeType for Mid
            float midToUse = (maximumToUse - minimumToUse) / 2;

            if (this.MidType == StiMidType.Value)
                midToUse = this.MidValue;

            else if (this.MidType == StiMidType.Percent)
                midToUse = minimumToUse + dist * midPercent / 100f;
            #endregion

            if (midToUse < minimumToUse)
                midToUse = minimumToUse;

            if (midToUse > maximumToUse)
                midToUse = maximumToUse;

            #region Get current value
            object currentValue = StiDataColumn.GetDataFromDataColumn(component.Report.Dictionary, Column);
            float floatCurrentValue = 0;
            try
            {
                floatCurrentValue = (float) StiReport.ChangeType(currentValue, typeof(float));
            }
            catch
            {
            }

            if (floatCurrentValue > maximumToUse)
                floatCurrentValue = maximumToUse;

            if (floatCurrentValue < minimumToUse)
                floatCurrentValue = minimumToUse;
            #endregion

            #region Create Indicator
            float percent;
            Color minColor;
            Color maxColor;
            if (this.ScaleType == StiColorScaleType.Color2)
            {
                percent = (floatCurrentValue - minimumToUse) / (maximumToUse - minimumToUse);
                minColor = this.MinimumColor;
                maxColor = this.MaximumColor;
            }
            else
            {
                if (floatCurrentValue > midToUse)
                {
                    percent = (floatCurrentValue - midToUse) / (maximumToUse - midToUse);
                    minColor = this.MidColor;
                    maxColor = this.MaximumColor;
                }
                else
                {
                    percent = (floatCurrentValue - minimumToUse) / (midToUse - minimumToUse);

                    minColor = this.MinimumColor;
                    maxColor = this.MidColor;
                }
            }

            float a = ((maxColor.A - minColor.A) * percent) + minColor.A;
            float r = ((maxColor.R - minColor.R) * percent) + minColor.R;
            float g = ((maxColor.G - minColor.G) * percent) + minColor.G;
            float b = ((maxColor.B - minColor.B) * percent) + minColor.B;

            a = Math.Min(a, 255);
            r = Math.Min(r, 255);
            g = Math.Min(g, 255);
            b = Math.Min(b, 255);

            global::System.Diagnostics.Debug.WriteLine($"percent = {floatCurrentValue} - {percent}, {a},{r},{g},{b}");

            component.Brush = new StiSolidBrush(Color.FromArgb((byte) a, (byte) r, (byte) g, (byte) b));
            #endregion

            return null;
        }

        /// <summary>
        /// Reset condition to base state.
        /// </summary>
        public void Reset()
        {
            this.minimum = null;
            this.maximum = null;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets type of Color Scale indicator.
        /// </summary> 
        [StiSerializable]
        [DefaultValue(StiColorScaleType.Color2)]
        public StiColorScaleType ScaleType { get; set; } = StiColorScaleType.Color2;

        /// <summary>
        /// Gets or sets a color for minimum values.
        /// </summary>
        [StiSerializable]
        public Color MinimumColor { get; set; } = Color.Red;

        /// <summary>
        /// Gets or sets a color for mid values.
        /// </summary>
        [StiSerializable]
        public Color MidColor { get; set; } = Color.Yellow;

        /// <summary>
        /// Gets or sets a color for maximal values.
        /// </summary>
        [StiSerializable]
        public Color MaximumColor { get; set; } = Color.Green;

        /// <summary>
        /// Gets or sets processing type of minimal values for color scale indicator.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiMinimumType.Auto)]
        public StiMinimumType MinimumType { get; set; }

        /// <summary>
        /// Gets or sets minimal value for color scale indicator.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0f)]
        public float MinimumValue { get; set; }

        /// <summary>
        /// Gets or sets processing type of mid values for color scale indicator.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiMidType.Auto)]
        public StiMidType MidType { get; set; }

        /// <summary>
        /// Gets or sets mid value for color scale indicator.
        /// </summary>
        [StiSerializable]
        [DefaultValue(50f)]
        public float MidValue { get; set; } = 50f;

        /// <summary>
        /// Gets or sets processing type of maximal values for color scale indicator.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiMaximumType.Auto)]
        public StiMaximumType MaximumType { get; set; }

        /// <summary>
        /// Gets or sets minimal value for color scale indicator.
        /// </summary>
        [StiSerializable]
        [DefaultValue(100f)]
        public float MaximumValue { get; set; } = 100f;
        #endregion

        #region Methods
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var condition = obj as StiColorScaleCondition;
            if (condition == null) return false;

            return
                this.Column.Equals(condition.Column) &&
                this.ScaleType.Equals(condition.ScaleType) &&
                this.MinimumColor.Equals(condition.MinimumColor) &&
                this.MidColor.Equals(condition.MidColor) &&
                this.MaximumColor.Equals(condition.MaximumColor) &&
                this.MinimumType.Equals(condition.MinimumType) &&
                this.MinimumValue.Equals(condition.MinimumValue) &&
                this.MidType.Equals(condition.MidType) &&
                this.MidValue.Equals(condition.MidValue) &&
                this.MaximumType.Equals(condition.MaximumType) &&
                this.MaximumValue.Equals(condition.MaximumValue);
        }
        #endregion

        #region Fields
        /// <summary>
        /// Gets or sets minimum amount.
        /// </summary>
        private float? minimum;

        /// <summary>
        /// Gets or sets minimum amount.
        /// </summary>
        private float? maximum;
        #endregion

        /// <summary>
        /// Creates a new object of the type StiColorScaleCondition.
        /// </summary>
        public StiColorScaleCondition()
        {
        }

        /// <summary>
        /// Creates a new object of the type StiColorScaleCondition.
        /// </summary>
        public StiColorScaleCondition(string column,
            StiColorScaleType scaleType,
            Color minimumColor, Color midColor, Color maximumColor,
            StiMinimumType minimumType, float minimumValue,
            StiMidType midType, float midValue,
            StiMaximumType maximumType, float maximumValue)
        {
            this.Column = column;
            this.ScaleType = scaleType;
            this.MinimumColor = minimumColor;
            this.MidColor = midColor;
            this.MaximumColor = maximumColor;
            this.MinimumType = minimumType;
            this.MinimumValue = minimumValue;
            this.MidType = midType;
            this.MidValue = midValue;
            this.MaximumType = maximumType;
            this.MaximumValue = maximumValue;
        }
    }
}