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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Dictionary;
using System;
using System.ComponentModel;
using System.Drawing;

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// The class describes the condition with Data Bar indicator.
    /// </summary>
    [RefreshProperties(RefreshProperties.All)]
	[TypeConverter(typeof(Stimulsoft.Report.Components.Design.StiConditionConverter))]
    public class StiDataBarCondition : 
        StiBaseCondition, 
        IStiDataBarIndicator,
        IStiIndicatorCondition
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiDataBarCondition
            jObject.AddPropertyEnum("BrushType", BrushType, StiBrushType.Gradient);
            jObject.AddPropertyColor("PositiveColor", PositiveColor, Color.Green);
            jObject.AddPropertyColor("NegativeColor", NegativeColor, Color.Red);
            jObject.AddPropertyColor("PositiveBorderColor", PositiveBorderColor, Color.DarkGreen);
            jObject.AddPropertyColor("NegativeBorderColor", NegativeBorderColor, Color.DarkRed);
            jObject.AddPropertyBool("ShowBorder", ShowBorder);
            jObject.AddPropertyEnum("Direction", Direction, StiDataBarDirection.Default);
            jObject.AddPropertyEnum("MinimumType", MinimumType, StiMinimumType.Auto);
            jObject.AddPropertyFloat("MinimumValue", MinimumValue, 0f);
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
                    case "BrushType":
                        this.BrushType = property.DeserializeEnum<StiBrushType>();
                        break;

                    case "PositiveColor":
                        this.PositiveColor = property.DeserializeColor();
                        break;

                    case "NegativeColor":
                        this.NegativeColor = property.DeserializeColor();
                        break;

                    case "PositiveBorderColor":
                        this.PositiveBorderColor = property.DeserializeColor();
                        break;

                    case "NegativeBorderColor":
                        this.NegativeBorderColor = property.DeserializeColor();
                        break;

                    case "ShowBorder":
                        this.ShowBorder = property.DeserializeBool();
                        break;

                    case "Direction":
                        this.Direction = property.DeserializeEnum<StiDataBarDirection>();
                        break;

                    case "MinimumType":
                        this.MinimumType = property.DeserializeEnum<StiMinimumType>();
                        break;

                    case "MinimumValue":
                        this.MinimumValue = property.DeserializeFloat();
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
                component.Report.WriteToReportRenderingMessages($"Column of Data Bar Condition of '{component.Name}' component is not specified!");
                return null;
            }
            #endregion

            if (!(this.MinimumType == StiMinimumType.Value && this.MaximumType == StiMaximumType.Value))
            {
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
                                floatValue = (float)StiReport.ChangeType(value, typeof(float));
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
                                floatValue = (float)StiReport.ChangeType(value, typeof(float));
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

                    if (minimum > 0 && this.MinimumType != StiMinimumType.Minimum) minimum = 0;
                    if (maximum < 0 && this.MaximumType != StiMaximumType.Maximum) maximum = 0;
                }
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

            #region Get current value
            object currentValue = StiDataColumn.GetDataFromDataColumn(component.Report.Dictionary, Column);
            float floatCurrentValue = 0;
            try
            {
                floatCurrentValue = (float)StiReport.ChangeType(currentValue, typeof(float));
            }
            catch
            {
                
            }

            if (floatCurrentValue > maximumToUse)
                floatCurrentValue = maximumToUse;

            if (floatCurrentValue < minimumToUse)
                floatCurrentValue = minimumToUse;
            #endregion

            if (minimumToUse > 0)
            {
                floatCurrentValue -= minimumToUse;
                maximumToUse -= minimumToUse;
                minimumToUse = 0;
            }

            if (maximumToUse < 0)
            {
                floatCurrentValue -= maximumToUse;
                minimumToUse -= maximumToUse;
                maximumToUse = 0;
            }

            return new StiDataBarIndicator
            {
                BrushType = this.BrushType,
                PositiveColor = this.PositiveColor,
                NegativeColor = this.NegativeColor,
                ShowBorder = this.ShowBorder,
                PositiveBorderColor = this.PositiveBorderColor,
                NegativeBorderColor = this.NegativeBorderColor,
                Direction = this.Direction,
                Minimum = minimumToUse,
                Maximum = maximumToUse,
                Value = floatCurrentValue
            };
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

        #region IStiDataBarIndicator
        /// <summary>
        /// Gets or sets value which indicates which type of brush will be used for Data Bar indicator drawing.
        /// </summary>    
        [StiSerializable]
        public StiBrushType BrushType { get; set; } = StiBrushType.Gradient;


        /// <summary>
        /// Gets or sets a color of positive values for data bar indicator.
        /// </summary>
        [StiSerializable]
        public Color PositiveColor { get; set; } = Color.Green;

        /// <summary>
        /// Gets or sets a color of negative values for data bar indicator.
        /// </summary>
        [StiSerializable]
        public Color NegativeColor { get; set; } = Color.Red;

        /// <summary>
        /// Gets or sets a border color of positive values for Data Bar indicator.
        /// </summary>
        [StiSerializable]
        public Color PositiveBorderColor { get; set; } = Color.DarkGreen;

        /// <summary>
        /// Gets or sets a border color of negative values for Data Bar indicator.
        /// </summary>
        [StiSerializable]
        public Color NegativeBorderColor { get; set; } = Color.DarkRed;

        /// <summary>
        /// Gets or sets value which indicates that border is drawing.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        public bool ShowBorder { get; set; }

        /// <summary>
        /// Gets or sets value which direction data bar will be filled by brush, from left to right or from right to left.
        /// </summary>        
        [StiSerializable]
        [DefaultValue(StiDataBarDirection.Default)]
        public StiDataBarDirection Direction { get; set; } = StiDataBarDirection.Default;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets processing type of minimal values for data bar indicator.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiMinimumType.Auto)]
        public StiMinimumType MinimumType { get; set; }

        /// <summary>
        /// Gets or sets minimal value for data bar indicator.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0f)]
        public float MinimumValue { get; set; }

        /// <summary>
        /// Gets or sets processing type of maximal values for data bar indicator.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiMaximumType.Auto)]
        public StiMaximumType MaximumType { get; set; }

        /// <summary>
        /// Gets or sets minimal value for data bar indicator.
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
            var condition = obj as StiDataBarCondition;
			if (condition == null)return false;

            return
                this.Column.Equals(condition.Column) &&
                this.BrushType.Equals(condition.BrushType) &&
                this.PositiveColor.Equals(condition.PositiveColor) &&
                this.NegativeColor.Equals(condition.NegativeColor) &&
                this.ShowBorder.Equals(condition.ShowBorder) &&
                this.PositiveBorderColor.Equals(condition.PositiveBorderColor) &&
                this.NegativeBorderColor.Equals(condition.NegativeBorderColor) &&
                this.MinimumType.Equals(condition.MinimumType) &&
                this.MinimumValue.Equals(condition.MinimumValue) &&
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
        /// Creates a new object of the type StiDataBarCondition.
		/// </summary>
		public StiDataBarCondition()
        {
        }

        /// <summary>
        /// Creates a new object of the type StiDataBarCondition.
		/// </summary>
		public StiDataBarCondition(string column, StiBrushType brushType,
            Color positiveColor, Color negativeColor,
            bool showBorder, Color positiveBorderColor, Color negativeBorderColor, 
            StiDataBarDirection direction,
            StiMinimumType minimumType, float minimumValue,
            StiMaximumType maximumType, float maximumValue)
		{
            this.Column = column;
            this.BrushType = brushType;
            this.PositiveColor = positiveColor;
            this.NegativeColor = negativeColor;
            this.ShowBorder = showBorder;
            this.PositiveBorderColor = positiveBorderColor;
            this.NegativeBorderColor = negativeBorderColor;
            this.Direction = direction;
            this.MinimumType = minimumType;
            this.MinimumValue = minimumValue;
            this.MaximumType = maximumType;
            this.MaximumValue = maximumValue;
        }
    }
}
