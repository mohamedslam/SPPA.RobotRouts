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
using System.Text;
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Components;
using System.Drawing.Design;
using System.Drawing;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiCandlestickSeries :
        StiSeries,
        IStiCandlestickSeries
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("TopN");
            jObject.RemoveProperty("TrendLine");
            jObject.RemoveProperty("ValueDataColumn");
            jObject.RemoveProperty("Value");
            jObject.RemoveProperty("ListOfValues");

            jObject.AddPropertyStringNullOrEmpty("ValueDataColumnOpen", valueDataColumnOpen);
            jObject.AddPropertyStringNullOrEmpty("ValueDataColumnClose", valueDataColumnClose);
            jObject.AddPropertyStringNullOrEmpty("ValueDataColumnHigh", valueDataColumnHigh);
            jObject.AddPropertyStringNullOrEmpty("ValueDataColumnLow", valueDataColumnLow);
            jObject.AddPropertyColor("BorderColor", BorderColor, Color.Gray);
            jObject.AddPropertyColor("BorderColorNegative", BorderColorNegative, Color.Gray);
            jObject.AddPropertyFloat("BorderWidth", BorderWidth, 2f);
            jObject.AddPropertyBrush("Brush", Brush);
            jObject.AddPropertyBrush("BrushNegative", BrushNegative);
            jObject.AddPropertyJObject("GetValueOpenEvent", getValueOpenEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetListOfValuesOpenEvent", getListOfValuesOpenEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetValueCloseEvent", getValueCloseEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetListOfValuesCloseEvent", getListOfValuesCloseEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetValueHighEvent", getValueHighEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetListOfValuesHighEvent", getListOfValuesHighEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetValueLowEvent", getValueLowEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetListOfValuesLowEvent", getListOfValuesLowEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("ValueOpen", ValueOpen.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("ListOfValuesOpen", ListOfValuesOpen.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("ValueClose", ValueClose.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("ListOfValuesClose", ListOfValuesClose.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("ValueHigh", ValueHigh.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("ListOfValuesHigh", ListOfValuesHigh.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("ValueLow", ValueLow.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("ListOfValuesLow", ListOfValuesLow.SaveToJsonObject(mode));

            if (mode == StiJsonSaveMode.Document)
            {
                jObject.RemoveProperty("ValuesString");

                jObject.AddPropertyStringNullOrEmpty("ValuesStringClose", ValuesStringClose);
                jObject.AddPropertyStringNullOrEmpty("ValuesStringHigh", ValuesStringHigh);
                jObject.AddPropertyStringNullOrEmpty("ValuesStringLow", ValuesStringLow);
                jObject.AddPropertyStringNullOrEmpty("ValuesStringOpen", ValuesStringOpen);
            }

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "ValuesStringClose":
                        this.ValuesStringClose = property.DeserializeString();
                        break;

                    case "ValuesStringHigh":
                        this.ValuesStringHigh = property.DeserializeString();
                        break;

                    case "ValuesStringLow":
                        this.ValuesStringLow = property.DeserializeString();
                        break;

                    case "ValuesStringOpen":
                        this.ValuesStringOpen = property.DeserializeString();
                        break;

                    case "ValueDataColumnOpen":
                        this.valueDataColumnOpen = property.DeserializeString();
                        break;

                    case "ValueDataColumnClose":
                        this.valueDataColumnClose = property.DeserializeString();
                        break;

                    case "ValueDataColumnHigh":
                        this.valueDataColumnHigh = property.DeserializeString();
                        break;

                    case "ValueDataColumnLow":
                        this.valueDataColumnLow = property.DeserializeString();
                        break;

                    case "BorderColor":
                        this.BorderColor = property.DeserializeColor();
                        break;

                    case "BorderColorNegative":
                        this.BorderColorNegative = property.DeserializeColor();
                        break;

                    case "BorderWidth":
                        this.BorderWidth = property.DeserializeFloat();
                        break;

                    case "Brush":
                        this.Brush = property.DeserializeBrush();
                        break;

                    case "BrushNegative":
                        this.BrushNegative = property.DeserializeBrush();
                        break;

                    case "GetValueOpenEvent":
                        this.getValueOpenEvent.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "GetListOfValuesOpenEvent":
                        this.getListOfValuesOpenEvent.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "GetValueCloseEvent":
                        this.getValueCloseEvent.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "GetListOfValuesCloseEvent":
                        this.getListOfValuesCloseEvent.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "GetValueHighEvent":
                        this.getValueHighEvent.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "GetListOfValuesHighEvent":
                        this.getListOfValuesHighEvent.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "GetValueLowEvent":
                        this.getValueLowEvent.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "GetListOfValuesLowEvent":
                        this.getListOfValuesLowEvent.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "ValueOpen":
                        this.ValueOpen.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "ListOfValuesOpen":
                        this.ListOfValuesOpen.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "ValueClose":
                        this.ValueClose.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "ListOfValuesClose":
                        this.ListOfValuesClose.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "ValueHigh":
                        this.ValueHigh.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "ListOfValuesHigh":
                        this.ListOfValuesHigh.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "ValueLow":
                        this.ValueLow.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "ListOfValuesLow":
                        this.ListOfValuesLow.LoadFromJsonObject((JObject)property.Value);
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
                return StiComponentId.StiCandlestickSeries;
            }
        }

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // Value Open
            var list = new[] 
            {
                propHelper.ValueDataColumnOpen(),
                propHelper.ValueOpen(),
                propHelper.ListOfValuesOpen()
            };
            objHelper.Add(StiPropertyCategories.ValueOpen, list);

            // Value Close
            list = new[] 
            {
                propHelper.ValueDataColumnClose(),
                propHelper.ValueClose(),
                propHelper.ListOfValuesClose()
            };
            objHelper.Add(StiPropertyCategories.ValueClose, list);

            //Value High
            list = new[] 
            {
                propHelper.ValueDataColumnHigh(),
                propHelper.ValueHigh(),
                propHelper.ListOfValuesHigh()
            };
            objHelper.Add(StiPropertyCategories.ValueHigh, list);

            //Value Low
            list = new[] 
            {
                propHelper.ValueDataColumnLow(),
                propHelper.ValueLow(),
                propHelper.ListOfValuesLow()
            };
            objHelper.Add(StiPropertyCategories.ValueLow, list);

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
                propHelper.BorderWidth(),
                propHelper.BorderColor(),
                propHelper.Brush(),
                propHelper.BrushNegative(),
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
                propHelper.YAxis()
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
            var series = base.Clone() as IStiCandlestickSeries;

            return series;
        }
        #endregion

        #region Methods.Types
        public override Type GetDefaultAreaType()
        {
            return typeof(StiCandlestickArea);
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
        [StiNonSerialized]
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

        [Browsable(false)]
        [StiNonSerialized]
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

        [Browsable(false)]
        [StiNonSerialized]
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

        private double?[] valuesOpen = { 4, 7, 5 };
        [Browsable(false)]
        public double?[] ValuesOpen
        {
            get
            {
                if (Chart == null || ((StiChart) Chart).Report == null || !Chart.IsDesigning || IsDashboard)
                    return valuesOpen;

                if (!string.IsNullOrEmpty(ListOfValuesOpen.Value))
                    return GetNullableValuesFromString(this, ListOfValuesOpen.Value);

                var seriesIndex = Chart.Series.IndexOf(this);
                var offset = GetOffsetForValues();

                return new double?[] { offset + 4 + seriesIndex * 7, offset + 7 + seriesIndex * 7, offset + 5 + seriesIndex * 7 };
            }
            set
            {
                valuesOpen = value;
            }
        }

        private double?[] valuesClose = { 2, 3, 5 };
        [Browsable(false)]
        public double?[] ValuesClose
        {
            get
            {
                if (Chart == null || ((StiChart) Chart).Report == null || !Chart.IsDesigning || IsDashboard)
                    return valuesClose;

                if (!string.IsNullOrEmpty(ListOfValuesClose.Value))
                    return GetNullableValuesFromString(this, ListOfValuesClose.Value);

                var seriesIndex = Chart.Series.IndexOf(this);
                var offset = GetOffsetForValues();

                return new double?[] { offset + 2 + seriesIndex * 7, offset + 3 + seriesIndex * 7, offset + 5 + seriesIndex * 7 };
            }
            set
            {
                valuesClose = value;
            }
        }

        [Browsable(false)]
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        public string ValuesStringOpen
        {
            get
            {
                var sb = new StringBuilder();
                bool first = true;
                foreach (double value in valuesOpen)
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
                    valuesOpen = new double?[0];
                }
                else
                {
                    string[] strs = value.Split(new char[] { ';' });

                    valuesOpen = new double?[strs.Length];

                    int index = 0;
                    foreach (string str in strs)
                    {
                        valuesOpen[index++] = double.Parse(str);
                    }
                }
            }
        }

        [Browsable(false)]
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        public string ValuesStringClose
        {
            get
            {
                var sb = new StringBuilder();
                bool first = true;
                foreach (double value in valuesClose)
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
                    valuesClose = new double?[0];
                }
                else
                {
                    string[] strs = value.Split(new char[] { ';' });

                    valuesClose = new double?[strs.Length];

                    int index = 0;
                    foreach (string str in strs)
                    {
                        valuesClose[index++] = double.Parse(str);
                    }
                }
            }
        }

        [Browsable(false)]
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        public string ValuesStringHigh
        {
            get
            {
                var sb = new StringBuilder();
                bool first = true;
                foreach (double value in valuesHigh)
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
                    valuesHigh = new double?[0];
                }
                else
                {
                    string[] strs = value.Split(new char[] { ';' });

                    valuesHigh = new double?[strs.Length];

                    int index = 0;
                    foreach (string str in strs)
                    {
                        valuesHigh[index++] = double.Parse(str);
                    }
                }
            }
        }

        [Browsable(false)]
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        public string ValuesStringLow
        {
            get
            {
                var sb = new StringBuilder();
                bool first = true;
                foreach (double value in valuesLow)
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
                    valuesLow = new double?[0];
                }
                else
                {
                    string[] strs = value.Split(new char[] { ';' });

                    valuesLow = new double?[strs.Length];

                    int index = 0;
                    foreach (string str in strs)
                    {
                        valuesLow[index++] = double.Parse(str);
                    }
                }
            }
        }

        private double?[] valuesHigh = { 6, 9, 7 };
        [Browsable(false)]
        public double?[] ValuesHigh
        {
            get
            {
                if (Chart == null || ((StiChart) Chart).Report == null || !Chart.IsDesigning || IsDashboard)
                    return valuesHigh;

                if (!string.IsNullOrEmpty(ListOfValuesHigh.Value))
                    return GetNullableValuesFromString(this, ListOfValuesHigh.Value);

                var seriesIndex = Chart.Series.IndexOf(this);

                var offset = GetOffsetForValues();

                return new double?[] { offset + 6 + seriesIndex * 7, offset + 9 + seriesIndex * 7, offset + 7 + seriesIndex * 7 };
            }
            set
            {
                valuesHigh = value;
            }
        }

        private double?[] valuesLow = { 1, 3, 4 };
        [Browsable(false)]
        public double?[] ValuesLow
        {
            get
            {
                if (Chart == null || ((StiChart) Chart).Report == null || !Chart.IsDesigning || IsDashboard)
                    return valuesLow;

                if (!string.IsNullOrEmpty(ListOfValuesLow.Value))
                    return GetNullableValuesFromString(this, ListOfValuesLow.Value);

                var seriesIndex = Chart.Series.IndexOf(this);

                var offset = GetOffsetForValues();

                return new double?[] { offset + 1 + seriesIndex * 7, offset + 3 + seriesIndex * 7, offset + 4 + seriesIndex * 7 };
            }
            set
            {
                valuesLow = value;
            }
        }

        private string valueDataColumnOpen = string.Empty;
        /// <summary>
        /// Gets or sets a name of the column that contains the open value.
        /// </summary>
        [DefaultValue("")]
        [StiSerializable]
        [StiOrder(StiSeriesPropertyOrder.ValueValueDataColumnOpen)]
        [Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("ValueOpen")]
        [Description("Gets or sets a name of the column that contains the open value.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public string ValueDataColumnOpen
        {
            get
            {
                return valueDataColumnOpen;
            }
            set
            {
                valueDataColumnOpen = value;
            }
        }


        private string valueDataColumnClose = string.Empty;
        /// <summary>
        /// Gets or sets a name of the column that contains the close value.
        /// </summary>
        [DefaultValue("")]
        [StiSerializable]
        [StiOrder(StiSeriesPropertyOrder.ValueValueDataColumnClose)]
        [Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("ValueClose")]
        [Description("Gets or sets a name of the column that contains the close value.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public string ValueDataColumnClose
        {
            get
            {
                return valueDataColumnClose;
            }
            set
            {
                valueDataColumnClose = value;
            }
        }

        private string valueDataColumnHigh = string.Empty;
        /// <summary>
        /// Gets or sets a name of the column that contains the high value.
        /// </summary>
        [DefaultValue("")]
        [StiSerializable]
        [StiOrder(StiSeriesPropertyOrder.ValueValueDataColumnHigh)]
        [Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("ValueHigh")]
        [Description("Gets or sets a name of the column that contains the high value.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public string ValueDataColumnHigh
        {
            get
            {
                return valueDataColumnHigh;
            }
            set
            {
                valueDataColumnHigh = value;
            }
        }

        private string valueDataColumnLow = string.Empty;
        /// <summary>
        /// Gets or sets a name of the column that contains the low value.
        /// </summary>
        [DefaultValue("")]
        [StiSerializable]
        [StiOrder(StiSeriesPropertyOrder.ValueValueDataColumnLow)]
        [Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("ValueLow")]
        [Description("Gets or sets a name of the column that contains the low value.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public string ValueDataColumnLow
        {
            get
            {
                return valueDataColumnLow;
            }
            set
            {
                valueDataColumnLow = value;
            }
        }

        private Color borderColor = Color.Gray;
        /// <summary>
        /// Gets or sets border color of series bar.
        /// </summary>
        [StiSerializable]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Appearance")]
        [Description("Gets or sets border color of series bar.")]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual Color BorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                borderColor = value;
            }
        }

        private Color borderColorNegative = Color.Gray;
        /// <summary>
        /// Gets or sets border color negative of series bar.
        /// </summary>
        [StiSerializable]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Appearance")]
        [Description("Gets or sets border color of series bar.")]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual Color BorderColorNegative
        {
            get
            {
                return borderColorNegative;
            }
            set
            {
                borderColorNegative = value;
            }
        }

        private float borderWidth = 2f;
        /// <summary>
        /// Gets or sets border width of series.
        /// </summary>
        [DefaultValue(2f)]
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets border width of series.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual float BorderWidth
        {
            get
            {
                return borderWidth;
            }
            set
            {
                if (value > 0)
                {
                    borderWidth = value;
                }
            }
        }

        private StiBrush brush = new StiSolidBrush(Color.Gainsboro);
        /// <summary>
        /// Gets or sets brush which will used to fill bar area.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets brush which will used to fill bar area.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiBrush Brush
        {
            get
            {
                return brush;
            }
            set
            {
                brush = value;
            }
        }

        private StiBrush brushNegative = new StiSolidBrush(Color.Transparent);
        /// <summary>
        /// Gets or sets a brush which will be used to fill negative values.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets a brush which will be used to fill negative values.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiBrush BrushNegative
        {
            get
            {
                return brushNegative;
            }
            set
            {
                brushNegative = value;
            }
        }
        #endregion

        #region Events
        #region GetValueOpen
        public event StiGetValueEventHandler GetValueOpen;

        /// <summary>
        /// Raises the GetValueOpen event.
        /// </summary>
        protected virtual void OnGetValueOpen(StiGetValueEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetValueOpen event.
        /// </summary>
        public virtual void InvokeGetValueOpen(StiComponent sender, StiGetValueEventArgs e)
        {
            try
            {
                OnGetValueOpen(e);
                if (sender.Report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    StiText tempText = new StiText();
                    tempText.Name = "**ChartGanttSeriesValueOpen**";
                    tempText.Page = sender.Report.Pages[0];
                    object parserResult = Engine.StiParser.ParseTextValue(ValueOpen.Value, tempText);
                    e.Value = sender.Report.ToString(parserResult);
                }
                if (this.GetValueOpen != null) this.GetValueOpen(sender, e);
            }
            catch (Exception ex)
            {
                StiLogService.Write(this.GetType(), ((StiChart)Chart).Name + "Series InvokeGetValueOpen...ERROR");
                StiLogService.Write(this.GetType(), ((StiChart)Chart).Name + "Series " + ex.Message);

                ((StiChart)Chart).Report.WriteToReportRenderingMessages(((StiChart)Chart).Name + "Series InvokeGetValueOpen...ERROR");
            }
        }

        private StiGetValueOpenEvent getValueOpenEvent = new StiGetValueOpenEvent();
        /// <summary>
        /// Gets or sets a script of the event GetValueOpen.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Gets or sets a script of the event GetValueOpen.")]
        public StiGetValueOpenEvent GetValueOpenEvent
        {
            get
            {
                return getValueOpenEvent;
            }
            set
            {
                getValueOpenEvent = value;
            }
        }
        #endregion

        #region GetListOfValuesOpen
        /// <summary>
        /// Occurs when getting the list of values open.
        /// </summary>
        public event StiGetValueEventHandler GetListOfValuesOpen;

        /// <summary>
        /// Raises the values open event.
        /// </summary>
        protected virtual void OnGetListOfValuesOpen(StiGetValueEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetListOfValuesOpen event.
        /// </summary>
        public void InvokeGetListOfValuesOpen(StiComponent sender, StiGetValueEventArgs e)
        {
            try
            {
                OnGetListOfValuesOpen(e);
                if (sender.Report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    StiText tempText = new StiText();
                    tempText.Name = "**ChartGanttSeriesListOfValuesOpen**";
                    tempText.Page = sender.Report.Pages[0];
                    object parserResult = Engine.StiParser.ParseTextValue(listOfValuesOpen, tempText);
                    e.Value = sender.Report.ToString(parserResult);
                }
                if (this.GetListOfValuesOpen != null) this.GetListOfValuesOpen(sender, e);
            }
            catch (Exception ex)
            {
                StiLogService.Write(this.GetType(), "InvokeGetListOfValuesOpen...Warning");
                StiLogService.Write(this.GetType(), ex.Message);

                ((StiChart)Chart).Report.WriteToReportRenderingMessages(((StiChart)Chart).Name + "InvokeGetListOfValuesOpen...ERROR");
            }
        }

        private StiGetListOfValuesOpenEvent getListOfValuesOpenEvent = new StiGetListOfValuesOpenEvent();
        /// <summary>
        /// Gets or sets a script of the event GetListOfValuesOpen.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Gets or sets a script of the event GetListOfValuesOpen.")]
        public StiGetListOfValuesOpenEvent GetListOfValuesOpenEvent
        {
            get
            {
                return getListOfValuesOpenEvent;
            }
            set
            {
                getListOfValuesOpenEvent = value;
            }
        }
        #endregion

        #region GetValueClose
        public event StiGetValueEventHandler GetValueClose;

        /// <summary>
        /// Raises the GetValueClose event.
        /// </summary>
        protected virtual void OnGetValueClose(StiGetValueEventArgs e)
        {
        }


        /// <summary>
        /// Raises the GetValueClose event.
        /// </summary>
        public virtual void InvokeGetValueClose(StiComponent sender, StiGetValueEventArgs e)
        {
            try
            {
                OnGetValueClose(e);
                if (sender.Report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    StiText tempText = new StiText();
                    tempText.Name = "**ChartGanttSeriesValueClose**";
                    tempText.Page = sender.Report.Pages[0];
                    object parserResult = Engine.StiParser.ParseTextValue(ValueClose.Value, tempText);
                    e.Value = sender.Report.ToString(parserResult);
                }
                if (this.GetValueClose != null) this.GetValueClose(sender, e);
            }
            catch (Exception ex)
            {
                StiLogService.Write(this.GetType(), ((StiChart)Chart).Name + "Series InvokeGetValueClose...ERROR");
                StiLogService.Write(this.GetType(), ((StiChart)Chart).Name + "Series " + ex.Message);

                ((StiChart)Chart).Report.WriteToReportRenderingMessages(((StiChart)Chart).Name + "Series InvokeGetValueClose...ERROR");
            }
        }


        private StiGetValueCloseEvent getValueCloseEvent = new StiGetValueCloseEvent();
        /// <summary>
        /// Gets or sets a script of the event GetValueClose.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Gets or sets a script of the event GetValueClose.")]
        public StiGetValueCloseEvent GetValueCloseEvent
        {
            get
            {
                return getValueCloseEvent;
            }
            set
            {
                getValueCloseEvent = value;
            }
        }
        #endregion

        #region GetListOfValuesClose
        /// <summary>
        /// Occurs when getting the list of values close.
        /// </summary>
        public event StiGetValueEventHandler GetListOfValuesClose;

        /// <summary>
        /// Raises the values close event.
        /// </summary>
        protected virtual void OnGetListOfValuesClose(StiGetValueEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetListOfValuesClose event.
        /// </summary>
        public void InvokeGetListOfValuesClose(StiComponent sender, StiGetValueEventArgs e)
        {
            try
            {
                OnGetListOfValuesClose(e);
                if (sender.Report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    StiText tempText = new StiText();
                    tempText.Name = "**ChartGanttSeriesListOfValuesClose**";
                    tempText.Page = sender.Report.Pages[0];
                    object parserResult = Engine.StiParser.ParseTextValue(listOfValuesClose, tempText);
                    e.Value = sender.Report.ToString(parserResult);
                }
                if (this.GetListOfValuesClose != null) this.GetListOfValuesClose(sender, e);
            }
            catch (Exception ex)
            {
                StiLogService.Write(this.GetType(), "InvokeGetListOfValuesClose...Warning");
                StiLogService.Write(this.GetType(), ex.Message);

                ((StiChart)Chart).Report.WriteToReportRenderingMessages(((StiChart)Chart).Name + "InvokeGetListOfValuesClose...ERROR");
            }
        }

        private StiGetListOfValuesCloseEvent getListOfValuesCloseEvent = new StiGetListOfValuesCloseEvent();
        /// <summary>
        /// Gets or sets a script of the event GetListOfValuesClose.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Gets or sets a script of the event GetListOfValuesClose.")]
        public StiGetListOfValuesCloseEvent GetListOfValuesCloseEvent
        {
            get
            {
                return getListOfValuesCloseEvent;
            }
            set
            {
                getListOfValuesCloseEvent = value;
            }
        }
        #endregion

        #region GetValueHigh
        public event StiGetValueEventHandler GetValueHigh;

        /// <summary>
        /// Raises the GetValueHigh event.
        /// </summary>
        protected virtual void OnGetValueHigh(StiGetValueEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetValueHigh event.
        /// </summary>
        public virtual void InvokeGetValueHigh(StiComponent sender, StiGetValueEventArgs e)
        {
            try
            {
                OnGetValueHigh(e);
                if (sender.Report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    StiText tempText = new StiText();
                    tempText.Name = "**ChartGanttSeriesValueHigh**";
                    tempText.Page = sender.Report.Pages[0];
                    object parserResult = Engine.StiParser.ParseTextValue(ValueHigh.Value, tempText);
                    e.Value = sender.Report.ToString(parserResult);
                }
                if (this.GetValueHigh != null) this.GetValueHigh(sender, e);
            }
            catch (Exception ex)
            {
                StiLogService.Write(this.GetType(), ((StiChart)Chart).Name + "Series InvokeGetValueHigh...ERROR");
                StiLogService.Write(this.GetType(), ((StiChart)Chart).Name + "Series " + ex.Message);

                ((StiChart)Chart).Report.WriteToReportRenderingMessages(((StiChart)Chart).Name + "Series InvokeGetValueHigh...ERROR");
            }
        }

        private StiGetValueHighEvent getValueHighEvent = new StiGetValueHighEvent();
        /// <summary>
        /// Gets or sets a script of the event GetValueHigh.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Gets or sets a script of the event GetValueHigh.")]
        public StiGetValueHighEvent GetValueHighEvent
        {
            get
            {
                return getValueHighEvent;
            }
            set
            {
                getValueHighEvent = value;
            }
        }
        #endregion

        #region GetListOfValuesHigh
        /// <summary>
        /// Occurs when getting the list of values high.
        /// </summary>
        public event StiGetValueEventHandler GetListOfValuesHigh;

        /// <summary>
        /// Raises the values high event.
        /// </summary>
        protected virtual void OnGetListOfValuesHigh(StiGetValueEventArgs e)
        {
        }


        /// <summary>
        /// Raises the GetListOfValuesHigh event.
        /// </summary>
        public void InvokeGetListOfValuesHigh(StiComponent sender, StiGetValueEventArgs e)
        {
            try
            {
                OnGetListOfValuesHigh(e);
                if (sender.Report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    StiText tempText = new StiText();
                    tempText.Name = "**ChartGanttSeriesListOfValuesHigh**";
                    tempText.Page = sender.Report.Pages[0];
                    object parserResult = Engine.StiParser.ParseTextValue(listOfValuesHigh, tempText);
                    e.Value = sender.Report.ToString(parserResult);
                }
                if (this.GetListOfValuesHigh != null) this.GetListOfValuesHigh(sender, e);
            }
            catch (Exception ex)
            {
                StiLogService.Write(this.GetType(), "InvokeGetListOfValuesHigh...Warning");
                StiLogService.Write(this.GetType(), ex.Message);

                ((StiChart)Chart).Report.WriteToReportRenderingMessages(((StiChart)Chart).Name + "InvokeGetListOfValuesHigh...ERROR");
            }
        }


        private StiGetListOfValuesHighEvent getListOfValuesHighEvent = new StiGetListOfValuesHighEvent();
        /// <summary>
        /// Gets or sets a script of the event GetListOfValuesHigh.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Gets or sets a script of the event GetListOfValuesHigh.")]
        public StiGetListOfValuesHighEvent GetListOfValuesHighEvent
        {
            get
            {
                return getListOfValuesHighEvent;
            }
            set
            {
                getListOfValuesHighEvent = value;
            }
        }
        #endregion

        #region GetValueLow
        public event StiGetValueEventHandler GetValueLow;

        /// <summary>
        /// Raises the GetValueLow event.
        /// </summary>
        protected virtual void OnGetValueLow(StiGetValueEventArgs e)
        {
        }


        /// <summary>
        /// Raises the GetValueLow event.
        /// </summary>
        public virtual void InvokeGetValueLow(StiComponent sender, StiGetValueEventArgs e)
        {
            try
            {
                OnGetValueLow(e);
                if (sender.Report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    StiText tempText = new StiText();
                    tempText.Name = "**ChartGanttSeriesValueLow**";
                    tempText.Page = sender.Report.Pages[0];
                    object parserResult = Engine.StiParser.ParseTextValue(ValueLow.Value, tempText);
                    e.Value = sender.Report.ToString(parserResult);
                }
                if (this.GetValueLow != null) this.GetValueLow(sender, e);
            }
            catch (Exception ex)
            {
                StiLogService.Write(this.GetType(), ((StiChart)Chart).Name + "Series InvokeGetValueLow...ERROR");
                StiLogService.Write(this.GetType(), ((StiChart)Chart).Name + "Series " + ex.Message);

                ((StiChart)Chart).Report.WriteToReportRenderingMessages(((StiChart)Chart).Name + "Series InvokeGetValueLow...ERROR");
            }
        }


        private StiGetValueLowEvent getValueLowEvent = new StiGetValueLowEvent();
        /// <summary>
        /// Gets or sets a script of the event GetValueLow.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Gets or sets a script of the event GetValueLow.")]
        public StiGetValueLowEvent GetValueLowEvent
        {
            get
            {
                return getValueLowEvent;
            }
            set
            {
                getValueLowEvent = value;
            }
        }
        #endregion

        #region GetListOfValuesLow
        /// <summary>
        /// Occurs when getting the list of values low.
        /// </summary>
        public event StiGetValueEventHandler GetListOfValuesLow;

        /// <summary>
        /// Raises the values low event.
        /// </summary>
        protected virtual void OnGetListOfValuesLow(StiGetValueEventArgs e)
        {
        }
        
        /// <summary>
        /// Raises the GetListOfValuesLow event.
        /// </summary>
        public void InvokeGetListOfValuesLow(StiComponent sender, StiGetValueEventArgs e)
        {
            try
            {
                OnGetListOfValuesLow(e);
                if (sender.Report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    StiText tempText = new StiText();
                    tempText.Name = "**ChartGanttSeriesListOfValuesLow**";
                    tempText.Page = sender.Report.Pages[0];
                    object parserResult = Engine.StiParser.ParseTextValue(listOfValuesLow, tempText);
                    e.Value = sender.Report.ToString(parserResult);
                }
                if (this.GetListOfValuesLow != null) this.GetListOfValuesLow(sender, e);
            }
            catch (Exception ex)
            {
                StiLogService.Write(this.GetType(), "InvokeGetListOfValuesLow...Warning");
                StiLogService.Write(this.GetType(), ex.Message);

                ((StiChart)Chart).Report.WriteToReportRenderingMessages(((StiChart)Chart).Name + "InvokeGetListOfValuesLow...ERROR");
            }
        }

        private StiGetListOfValuesLowEvent getListOfValuesLowEvent = new StiGetListOfValuesLowEvent();
        /// <summary>
        /// Gets or sets a script of the event GetListOfValuesLow.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Gets or sets a script of the event GetListOfValuesLow.")]
        public StiGetListOfValuesLowEvent GetListOfValuesLowEvent
        {
            get
            {
                return getListOfValuesLowEvent;
            }
            set
            {
                getListOfValuesLowEvent = value;
            }
        }
        #endregion
        #endregion

        #region Expressions
        #region ValuesOpen
        private StiValueOpenExpression valueObjOpen = new StiValueOpenExpression();
        /// <summary>
        /// Gets or sets open value expression. Example: {Order.Value}
        /// </summary>
        [StiOrder(StiSeriesPropertyOrder.ValueValueOpen)]
        [StiCategory("ValueOpen")]
        [StiSerializable]
        [Description("Gets or sets open value expression. Example: {Order.Value}")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiValueOpenExpression ValueOpen
        {
            get
            {
                return this.valueObjOpen;
            }
            set
            {
                this.valueObjOpen = value;
            }
        }

        private StiListOfValuesOpenExpression listOfValuesOpen = new StiListOfValuesOpenExpression();
        /// <summary>
        /// Gets or sets the expression to fill a list of open values.  Example: 1;2;3
        /// </summary>
        [StiCategory("ValueOpen")]
        [StiOrder(StiSeriesPropertyOrder.ValueListOfValuesOpen)]
        [StiSerializable(
             StiSerializeTypes.SerializeToCode |
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets the expression to fill a list of open values.  Example: 1;2;3")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiListOfValuesOpenExpression ListOfValuesOpen
        {
            get
            {
                return listOfValuesOpen;
            }
            set
            {
                listOfValuesOpen = value;
            }
        }
        #endregion

        #region ValuesClose
        private StiValueCloseExpression valueObjClose = new StiValueCloseExpression();
        /// <summary>
        /// Gets or sets close value expression. Example: {Order.Value}
        /// </summary>
        [StiOrder(StiSeriesPropertyOrder.ValueValueClose)]
        [StiCategory("ValueClose")]
        [StiSerializable]
        [Description("Gets or sets close value expression. Example: {Order.Value}")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiValueCloseExpression ValueClose
        {
            get
            {
                return this.valueObjClose;
            }
            set
            {
                this.valueObjClose = value;
            }
        }

        private StiListOfValuesCloseExpression listOfValuesClose = new StiListOfValuesCloseExpression();
        /// <summary>
        /// Gets or sets the expression to fill a list of close values.  Example: 1;2;3
        /// </summary>
        [StiCategory("ValueClose")]
        [StiOrder(StiSeriesPropertyOrder.ValueListOfValuesClose)]
        [StiSerializable(
             StiSerializeTypes.SerializeToCode |
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets the expression to fill a list of close values.  Example: 1;2;3")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiListOfValuesCloseExpression ListOfValuesClose
        {
            get
            {
                return listOfValuesClose;
            }
            set
            {
                listOfValuesClose = value;
            }
        }
        #endregion

        #region ValuesHigh
        private StiValueHighExpression valueObjHigh = new StiValueHighExpression();
        /// <summary>
        /// Gets or sets high value expression. Example: {Order.Value}
        /// </summary>
        [StiOrder(StiSeriesPropertyOrder.ValueValueHigh)]
        [StiCategory("ValueHigh")]
        [StiSerializable]
        [Description("Gets or sets high value expression. Example: {Order.Value}")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiValueHighExpression ValueHigh
        {
            get
            {
                return this.valueObjHigh;
            }
            set
            {
                this.valueObjHigh = value;
            }
        }

        private StiListOfValuesHighExpression listOfValuesHigh = new StiListOfValuesHighExpression();
        /// <summary>
        /// Gets or sets the expression to fill a list of high values.  Example: 1;2;3
        /// </summary>
        [StiCategory("ValueHigh")]        
        [StiOrder(StiSeriesPropertyOrder.ValueListOfValuesHigh)]
        [StiSerializable(
             StiSerializeTypes.SerializeToCode |
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets the expression to fill a list of high values.  Example: 1;2;3")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiListOfValuesHighExpression ListOfValuesHigh
        {
            get
            {
                return listOfValuesHigh;
            }
            set
            {
                listOfValuesHigh = value;
            }
        }
        #endregion

        #region ValuesLow
        private StiValueLowExpression valueObjLow = new StiValueLowExpression();
        /// <summary>
        /// Gets or sets low value expression. Example: {Order.Value}
        /// </summary>
        [StiOrder(StiSeriesPropertyOrder.ValueValueLow)]
        [StiCategory("ValueLow")]
        [StiSerializable]
        [Description("Gets or sets low value expression. Example: {Order.Value}")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiValueLowExpression ValueLow
        {
            get
            {
                return this.valueObjLow;
            }
            set
            {
                this.valueObjLow = value;
            }
        }

        private StiListOfValuesLowExpression listOfValuesLow = new StiListOfValuesLowExpression();
        /// <summary>
        /// Gets or sets the expression to fill a list of low values.  Example: 1;2;3
        /// </summary>
        [StiCategory("ValueLow")]
        [StiOrder(StiSeriesPropertyOrder.ValueListOfValuesLow)]
        [StiSerializable(
             StiSerializeTypes.SerializeToCode |
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets the expression to fill a list of low values.  Example: 1;2;3")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiListOfValuesLowExpression ListOfValuesLow
        {
            get
            {
                return listOfValuesLow;
            }
            set
            {
                listOfValuesLow = value;
            }
        }
        #endregion
        #endregion

        #region Methods.override
        public override StiSeries CreateNew()
        {
            return new StiCandlestickSeries();
        }
        #endregion

        public StiCandlestickSeries()
        {
            this.Core = new StiCandlestickSeriesCoreXF(this);
        }
    }
}