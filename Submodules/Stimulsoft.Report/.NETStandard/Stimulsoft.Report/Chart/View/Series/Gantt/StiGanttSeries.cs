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

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiGanttSeries : 
        StiClusteredBarSeries,
        IStiGanttSeries
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("TopN");
            jObject.RemoveProperty("TrendLine");

            jObject.AddPropertyString("ValueDataColumnEnd", valueDataColumnEnd);
            jObject.AddPropertyJObject("GetValueEndEvent", getValueEndEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetListOfValuesEndEvent", getListOfValuesEndEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("ValueEnd", ValueEnd.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("ListOfValuesEnd", ListOfValuesEnd.SaveToJsonObject(mode));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "ValueDataColumnEnd":
                        this.valueDataColumnEnd = property.DeserializeString();
                        break;

                    case "GetValueEndEvent":
                        this.getValueEndEvent.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "GetListOfValuesEndEvent":
                        this.getListOfValuesEndEvent.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "ValueEnd":
                        this.ValueEnd.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "ListOfValuesEnd":
                        this.ListOfValuesEnd.LoadFromJsonObject((JObject)property.Value);
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiGanttSeries;
            }
        }

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // Value
            var list = new[] 
            {
                propHelper.ValueDataColumn(),
                propHelper.Value(),
                propHelper.ListOfValues()
            };
            objHelper.Add(StiPropertyCategories.Value, list);

            //ValueEnd
            list = new[] 
            {
                propHelper.ValueDataColumnEnd(),
                propHelper.ValueEnd(),
                propHelper.ListOfValuesEnd()
            };
            objHelper.Add(StiPropertyCategories.ValueEnd, list);

            // Argument 
            list = new[]
            {
                propHelper.ArgumentDataColumn(),
                propHelper.Argument(),
                propHelper.ListOfArguments()
            };
            objHelper.Add(StiPropertyCategories.Argument, list);

            // Data
            list = new[] 
            { 
                propHelper.Format(),
                propHelper.SortBy(), 
                propHelper.SortDirection(),
                propHelper.AutoSeriesKeyDataColumn(),
                propHelper.AutoSeriesColorDataColumn(),
                propHelper.AutoSeriesTitleDataColumn()
            };
            objHelper.Add(StiPropertyCategories.Data, list);

            // Appearance
            list = new[] 
            {
                propHelper.BorderColor(),
                propHelper.Brush(),
                propHelper.ShowShadow()
            };
            objHelper.Add(StiPropertyCategories.Appearance, list);

            // Behavior
            list = new[] 
            {
                propHelper.AllowApplyStyle(),
                propHelper.ShowInLegend(),
                propHelper.ShowSeriesLabels(),
                propHelper.Title(),
                propHelper.YAxis(),
                propHelper.fWidth()
            };
            objHelper.Add(StiPropertyCategories.Behavior, list);

            return objHelper;
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            var series = base.Clone() as IStiGanttSeries;

            return series;
        }
        #endregion

        #region Methods.Types
        public override Type GetDefaultAreaType()
        {
            return typeof(StiGanttArea);
        }
        #endregion

        #region Properties
        [Browsable(false)]
        [StiNonSerialized]
        public override IStiSeriesTopN TopN
        {
            get
            {
                return base.TopN;
            }
            set
            {
                base.TopN = value;
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override bool TopNAllowed
        {
            get
            {
                return false;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public override StiTrendLinesCollection TrendLines
        {
            get
            {
                return base.TrendLines;
            }
            set
            {
                base.TrendLines = value;
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override bool TrendLineAllowed
        {
            get
            {
                return false;
            }
        }

        [Browsable(false)]
        public override StiBrush BrushNegative
        {
            get
            {
                return base.BrushNegative;
            }
            set
            {
                base.BrushNegative = value;
            }
        }

        [Browsable(false)]
        public override bool AllowApplyBrushNegative
        {
            get
            {
                return base.AllowApplyBrushNegative;
            }
            set
            {
                base.AllowApplyBrushNegative = value;
            }
        }

        /// <summary>
        /// Gets or sets value which indicates whether it is necessary to show the series element, if the series value of this column is 0.
        /// </summary>
        [Browsable(false)]
        public override bool ShowZeros
        {
            get
            {
                return base.ShowZeros;
            }
            set
            {
                base.ShowZeros = value;
            }
        }

        private double?[] valuesEnd = { 3, 5, 9 };
        [Browsable(false)]
        public double?[] ValuesEnd
        {
            get
            {
                if (Chart == null || ((StiChart) Chart).Report == null || !Chart.IsDesigning || IsDashboard)
                    return valuesEnd;

                if (!string.IsNullOrEmpty(ListOfValuesEnd.Value))
                    return GetNullableValuesFromString(this, ListOfValuesEnd.Value);

                var seriesIndex = Chart.Series.IndexOf(this);
                if (seriesIndex == 0)
                    return valuesEnd;

                return new double?[] { 3 + seriesIndex * 3, 5 + seriesIndex * 4, 9 + seriesIndex * 3 };
            }
            set
            {
                valuesEnd = value;
            }
        }


        [Browsable(false)]
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        public string ValuesStringEnd
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                bool first = true;
                foreach (double value in valuesEnd)
                {
                    if (first) sb.AppendFormat("{0}", value);
                    else sb.AppendFormat(";{0}", value);
                    first = false;
                }
                return sb.ToString();
            }
            set
            {
                if (value == null || value.Trim().Length == 0)
                {
                    valuesEnd = new double?[0];
                }
                else
                {
                    string[] strs = value.Split(new char[] { ';' });

                    valuesEnd = new double?[strs.Length];

                    int index = 0;
                    foreach (string str in strs)
                    {
                        valuesEnd[index++] = double.Parse(str);
                    }
                }
            }
        }



        /// <summary>
        /// Gets or sets a name of the column that contains the start value.
        /// </summary>
        [DefaultValue("")]
        [StiSerializable]
        [StiOrder(StiSeriesPropertyOrder.ValueValueDataColumn)]
        [Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Value")]
        [Description("Gets or sets a name of the column that contains the start value.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public override string ValueDataColumn
        {
            get
            {
                return base.ValueDataColumn;
            }
            set
            {
                base.ValueDataColumn = value;
            }
        }


        private string valueDataColumnEnd = string.Empty;
        /// <summary>
        /// Gets or sets a name of the column that contains the end value.
        /// </summary>
        [DefaultValue("")]
        [StiSerializable]
		[StiOrder(StiSeriesPropertyOrder.ValueValueDataColumnEnd)]
        [Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("ValueEnd")]
        [Description("Gets or sets a name of the column that contains the end value.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public string ValueDataColumnEnd
        {
            get
            {
                return valueDataColumnEnd;
            }
            set
            {
                valueDataColumnEnd = value;
            }
        }
        #endregion

        #region Events
        #region GetValueEnd
        public event StiGetValueEventHandler GetValueEnd;

        /// <summary>
        /// Raises the GetValueEnd event.
        /// </summary>
        protected virtual void OnGetValueEnd(StiGetValueEventArgs e)
        {
        }


        /// <summary>
        /// Raises the GetValueEnd event.
        /// </summary>
        public virtual void InvokeGetValueEnd(StiComponent sender, StiGetValueEventArgs e)
        {
            try
            {
                OnGetValueEnd(e);
                if (sender.Report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    StiText tempText = new StiText();
                    tempText.Name = "**ChartGanttSeriesValueEnd**";
                    tempText.Page = sender.Report.Pages[0];
                    object parserResult = Engine.StiParser.ParseTextValue(ValueEnd.Value, tempText);
                    e.Value = sender.Report.ToString(parserResult);
                }
                if (this.GetValueEnd != null) this.GetValueEnd(sender, e);
            }
            catch (Exception ex)
            {
                StiLogService.Write(this.GetType(), ((StiChart)Chart).Name + "Series InvokeGetValueEnd...ERROR");
                StiLogService.Write(this.GetType(), ((StiChart)Chart).Name + "Series " + ex.Message);

                ((StiChart)Chart).Report.WriteToReportRenderingMessages(((StiChart)Chart).Name + "Series InvokeGetValueEnd...ERROR");
            }
        }


        private StiGetValueEndEvent getValueEndEvent = new StiGetValueEndEvent();
        /// <summary>
        /// Gets or sets a script of the event GetValueEnd.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Gets or sets a script of the event GetValueEnd.")]
        public StiGetValueEndEvent GetValueEndEvent
        {
            get
            {
                return getValueEndEvent;
            }
            set
            {
                getValueEndEvent = value;
            }
        }
        #endregion	

        #region GetListOfValuesEnd
        /// <summary>
        /// Occurs when getting the list of values end.
        /// </summary>
        public event StiGetValueEventHandler GetListOfValuesEnd;

        /// <summary>
        /// Raises the values end event.
        /// </summary>
        protected virtual void OnGetListOfValuesEnd(StiGetValueEventArgs e)
        {
        }


        /// <summary>
        /// Raises the GetListOfValuesEnd event.
        /// </summary>
        public void InvokeGetListOfValuesEnd(StiComponent sender, StiGetValueEventArgs e)
        {
            try
            {
                OnGetListOfValuesEnd(e);
                if (sender.Report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    StiText tempText = new StiText();
                    tempText.Name = "**ChartGanttSeriesListOfValuesEnd**";
                    tempText.Page = sender.Report.Pages[0];
                    object parserResult = Engine.StiParser.ParseTextValue(listOfValuesEnd, tempText);
                    e.Value = sender.Report.ToString(parserResult);
                }
                if (this.GetListOfValuesEnd != null) this.GetListOfValuesEnd(sender, e);
            }
            catch (Exception ex)
            {
                StiLogService.Write(this.GetType(), "InvokeGetListOfValuesEnd...Warning");
                StiLogService.Write(this.GetType(), ex.Message);

                ((StiChart)Chart).Report.WriteToReportRenderingMessages(((StiChart)Chart).Name + "InvokeGetListOfValuesEnd...ERROR");
            }
        }


        private StiGetListOfValuesEndEvent getListOfValuesEndEvent = new StiGetListOfValuesEndEvent();
        /// <summary>
        /// Gets or sets a script of the event GetListOfValuesEnd.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Gets or sets a script of the event GetListOfValuesEnd.")]
        public StiGetListOfValuesEndEvent GetListOfValuesEndEvent
        {
            get
            {
                return getListOfValuesEndEvent;
            }
            set
            {
                getListOfValuesEndEvent = value;
            }
        }
        #endregion

        #endregion

        #region Expressions
        #region ValueEnd
        /// <summary>
        /// Gets or sets start value expression. Example: {Order.Value}
        /// </summary>
        [StiCategory("Value")]
        [StiOrder(StiSeriesPropertyOrder.ValueValue)]
        [StiSerializable]
        [Description("Gets or sets start value expression. Example: {Order.Value}")]
        [StiPropertyLevel(StiLevel.Standard)]
        public override StiExpression Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                base.Value = value;
            }
        }

        private StiValueEndExpression valueObjEnd = new StiValueEndExpression();
        /// <summary>
        /// Gets or sets end value expression. Example: {Order.Value}
        /// </summary>
		[StiOrder(StiSeriesPropertyOrder.ValueValueEnd)]
        [StiCategory("ValueEnd")]
        [StiSerializable]
        [Description("Gets or sets end value expression. Example: {Order.Value}")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiValueEndExpression ValueEnd
        {
            get
            {
                return this.valueObjEnd;
            }
            set
            {
                this.valueObjEnd = value;
            }
        }
        #endregion

        #region ListOfValuesEnd
        /// <summary>
        /// Gets or sets the expression to fill a list of start values.  Example: 1;2;3
        /// </summary>
        [StiCategory("Value")]
        [StiOrder(StiSeriesPropertyOrder.ValueListOfValues)]
        [StiSerializable(
             StiSerializeTypes.SerializeToCode |
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets the expression to fill a list of start values.  Example: 1;2;3")]
        public override StiListOfValuesExpression ListOfValues
        {
            get
            {
                return base.ListOfValues;
            }
            set
            {
                base.ListOfValues = value;
            }
        }

        private StiListOfValuesEndExpression listOfValuesEnd = new StiListOfValuesEndExpression();
        /// <summary>
        /// Gets or sets the expression to fill a list of end values.  Example: 1;2;3
        /// </summary>
        [StiCategory("ValueEnd")]
		[StiOrder(StiSeriesPropertyOrder.ValueListOfValuesEnd)]
        [StiSerializable(
             StiSerializeTypes.SerializeToCode |
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets the expression to fill a list of end values.  Example: 1;2;3")]
        public virtual StiListOfValuesEndExpression ListOfValuesEnd
        {
            get
            {
                return listOfValuesEnd;
            }
            set
            {
                listOfValuesEnd = value;
            }
        }
        #endregion

        #endregion        
        
        #region Methods.override
        public override StiSeries CreateNew()
        {
            return new StiGanttSeries();
        }
        #endregion

        public StiGanttSeries()
        {
            this.Core = new StiGanttSeriesCoreXF(this);
        }
    }
}