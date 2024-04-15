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
    /// The class describes the condition with Icon Set indicator.
    /// </summary>
    [RefreshProperties(RefreshProperties.All)]
	[TypeConverter(typeof(Stimulsoft.Report.Components.Design.StiConditionConverter))]
    public class StiIconSetCondition : 
        StiBaseCondition,
        IStiIndicatorCondition
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiIconSetCondition
            jObject.AddPropertyEnum("IconSet", IconSet, StiIconSet.None);
            jObject.AddPropertyEnum("ContentAlignment", ContentAlignment, ContentAlignment.MiddleLeft);
            if (this.IconSetItem1 != null)
                jObject.AddPropertyJObject("IconSetItem1", this.IconSetItem1.SaveToJsonObject(mode));
            if (this.IconSetItem2 != null)
                jObject.AddPropertyJObject("IconSetItem2", this.IconSetItem2.SaveToJsonObject(mode));
            if (this.IconSetItem3 != null)
                jObject.AddPropertyJObject("IconSetItem3", this.IconSetItem3.SaveToJsonObject(mode));
            if (this.IconSetItem4 != null)
                jObject.AddPropertyJObject("IconSetItem4", this.IconSetItem4.SaveToJsonObject(mode));
            if (this.IconSetItem5 != null)
                jObject.AddPropertyJObject("IconSetItem5", this.IconSetItem5.SaveToJsonObject(mode));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "IconSet":
                        this.IconSet = property.DeserializeEnum<StiIconSet>();
                        break;

                    case "ContentAlignment":
                        this.ContentAlignment = property.DeserializeEnum<ContentAlignment>();
                        break;

                    case "IconSetItem1":
                        {
                            var icon = new StiIconSetItem();
                            icon.LoadFromJsonObject((JObject)property.Value);
                            this.IconSetItem1 = icon;
                        }
                        break;

                    case "IconSetItem2":
                        {
                            var icon = new StiIconSetItem();
                            icon.LoadFromJsonObject((JObject)property.Value);
                            this.IconSetItem2 = icon;
                        }
                        break;

                    case "IconSetItem3":
                        {
                            var icon = new StiIconSetItem();
                            icon.LoadFromJsonObject((JObject)property.Value);
                            this.IconSetItem3 = icon;
                        }
                        break;

                    case "IconSetItem4":
                        {
                            var icon = new StiIconSetItem();
                            icon.LoadFromJsonObject((JObject)property.Value);
                            this.IconSetItem4 = icon;
                        }
                        break;

                    case "IconSetItem5":
                        {
                            var icon = new StiIconSetItem();
                            icon.LoadFromJsonObject((JObject)property.Value);
                            this.IconSetItem5 = icon;
                        }
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
                component.Report.WriteToReportRenderingMessages($"Column of Icon Set Condition of '{component.Name}' component is not specified!");
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

                    //fix - calculate all rows
                    if ((dataSource.DetailRows != null) && (dataSource.DataTable != null))
                    {
                        dataSource.DetailRows = null;
                    }

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
            }

            float minimumToUse = this.minimum.GetValueOrDefault();
            float maximumToUse = this.maximum.GetValueOrDefault();

            float dist = maximumToUse - minimumToUse;
            
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

            float floatCurrentValuePercent = (floatCurrentValue - minimumToUse) / dist * 100f;
            #endregion

            #region Create Indicator
            var icon = StiIcon.None;
            if (this.IconSetItem4 == null && this.IconSetItem5 == null)
            {
                if (InRange(this.IconSetItem1, floatCurrentValue, floatCurrentValuePercent))
                    icon = this.IconSetItem1.Icon;

                else if (InRange(this.IconSetItem2, floatCurrentValue, floatCurrentValuePercent))
                    icon = this.IconSetItem2.Icon;

                else if (this.IconSetItem3 != null)
                    icon = this.IconSetItem3.Icon;
            }
            else if (this.IconSetItem5 == null)
            {
                if (InRange(this.IconSetItem1, floatCurrentValue, floatCurrentValuePercent))
                    icon = this.IconSetItem1.Icon;

                else if (InRange(this.IconSetItem2, floatCurrentValue, floatCurrentValuePercent))
                    icon = this.IconSetItem2.Icon;

                else if (InRange(this.IconSetItem3, floatCurrentValue, floatCurrentValuePercent))
                    icon = this.IconSetItem3.Icon;

                else if (this.IconSetItem4 != null)
                    icon = this.IconSetItem4.Icon;
            }
            else
            {
                if (InRange(this.IconSetItem1, floatCurrentValue, floatCurrentValuePercent))
                    icon = this.IconSetItem1.Icon;

                else if (InRange(this.IconSetItem2, floatCurrentValue, floatCurrentValuePercent))
                    icon = this.IconSetItem2.Icon;

                else if (InRange(this.IconSetItem3, floatCurrentValue, floatCurrentValuePercent))
                    icon = this.IconSetItem3.Icon;

                else if (InRange(this.IconSetItem4, floatCurrentValue, floatCurrentValuePercent))
                    icon = this.IconSetItem4.Icon;

                else if (this.IconSetItem5 != null)
                    icon = this.IconSetItem5.Icon;
            }            
            #endregion

            if (icon == StiIcon.None)
                return null;

            return new StiIconSetIndicator
            {
                Icon = icon,
                Alignment = this.ContentAlignment
            };
        }

        private bool InRange(StiIconSetItem item, float floatCurrentValue, float floatCurrentValuePercent)
        {
            if (item == null)
                return false;

            if (item.ValueType == StiIconSetValueType.Percent)
            {
                if (item.Operation == StiIconSetOperation.MoreThan)
                {
                    if (floatCurrentValuePercent > item.Value)
                        return true;
                }
                else
                {
                    if (floatCurrentValuePercent >= item.Value)
                        return true;
                }
            }
            else
            {
                if (item.Operation == StiIconSetOperation.MoreThan)
                {
                    if (floatCurrentValue > item.Value)
                        return true;
                }
                else
                {
                    if (floatCurrentValue >= item.Value)
                        return true;
                }
            }
            return false;
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
        /// Gets or sets icon set.
        /// </summary> 
        [StiSerializable]
        [DefaultValue(StiIconSet.None)]
        public StiIconSet IconSet { get; set; } = StiIconSet.None;

        /// <summary>
        /// Gets or sets icon alignment.
        /// </summary>
        [StiSerializable]
        [DefaultValue(ContentAlignment.MiddleLeft)]
        public ContentAlignment ContentAlignment { get; set; } = ContentAlignment.MiddleLeft;

        /// <summary>
        /// Gets or sets first Icon Set item.
        /// </summary> 
        [StiSerializable(StiSerializationVisibility.Class)]
        public StiIconSetItem IconSetItem1 { get; set; }

        /// <summary>
        /// Gets or sets second Icon Set item.
        /// </summary> 
        [StiSerializable(StiSerializationVisibility.Class)]
        public StiIconSetItem IconSetItem2 { get; set; }

        /// <summary>
        /// Gets or sets third Icon Set item.
        /// </summary> 
        [StiSerializable(StiSerializationVisibility.Class)]
        public StiIconSetItem IconSetItem3 { get; set; }

        /// <summary>
        /// Gets or sets fourth Icon Set item.
        /// </summary> 
        [StiSerializable(StiSerializationVisibility.Class)]
        public StiIconSetItem IconSetItem4 { get; set; }

        /// <summary>
        /// Gets or sets fifth Icon Set item.
        /// </summary> 
        [StiSerializable(StiSerializationVisibility.Class)]
        public StiIconSetItem IconSetItem5 { get; set; }
        #endregion

        #region Methods
        public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
            var condition = obj as StiIconSetCondition;
			if (condition == null)return false;

			return
                this.IconSet.Equals(condition.IconSet) &&
                this.ContentAlignment.Equals(condition.ContentAlignment);
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
        /// Creates a new object of the type StiIconSetCondition.
        /// </summary>
        public StiIconSetCondition()
        {
        }

        /// <summary>
        /// Creates a new object of the type StiIconSetCondition.
		/// </summary>
        public StiIconSetCondition(string column, StiIconSet iconSet, ContentAlignment contentAlignment, 
            StiIconSetItem item1, StiIconSetItem item2, StiIconSetItem item3, StiIconSetItem item4, StiIconSetItem item5)
		{
            this.Column = column;
            this.IconSet = iconSet;
            this.ContentAlignment = contentAlignment;
            this.IconSetItem1 = item1;
            this.IconSetItem2 = item2;
            this.IconSetItem3 = item3;
            this.IconSetItem4 = item4;
            this.IconSetItem5 = item5;
        }
    }
}
