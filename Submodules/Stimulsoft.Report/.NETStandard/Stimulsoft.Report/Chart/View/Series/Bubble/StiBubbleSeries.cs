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
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Design;
using Stimulsoft.Report.Helpers;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiBubbleSeries :
        StiScatterSeries,
        IStiFontIconsSeries,
        IStiBubbleSeries
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("LineMarker");
            jObject.RemoveProperty("Marker");

            jObject.AddPropertyColor("BorderColor", borderColor, Color.Gray);
            jObject.AddPropertyInt("BorderThickness", BorderThickness, 1);
            jObject.AddPropertyBrush("Brush", brush);
            jObject.AddPropertyStringNullOrEmpty("WeightDataColumn", weightDataColumn);
            jObject.AddPropertyJObject("GetWeightEvent", getWeightEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetListOfWeightsEvent", getListOfWeightsEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Weight", Weight.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("ListOfWeights", ListOfWeights.SaveToJsonObject(mode));
            if (this.Icon != null)
                jObject.AddPropertyEnum("Icon", this.Icon);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "BorderColor":
                        this.borderColor = property.DeserializeColor();
                        break;

                    case "BorderThickness":
                        this.BorderThickness = property.DeserializeInt();
                        break;

                    case "Brush":
                        this.brush = property.DeserializeBrush();
                        break;

                    case "WeightDataColumn":
                        this.weightDataColumn = property.DeserializeString();
                        break;

                    case "GetWeightEvent":
                        this.getWeightEvent.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "GetListOfWeightsEvent":
                        this.getListOfWeightsEvent.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "Weight":
                        this.Weight.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "ListOfWeights":
                        this.ListOfWeights.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "Icon":
                        this.Icon = property.DeserializeEnum<StiFontIcons>();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiBubbleSeries;
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

            // Argument
            list = new[] 
            {
                propHelper.ArgumentDataColumn(),
                propHelper.Argument(),
                propHelper.ListOfArguments()
            };
            objHelper.Add(StiPropertyCategories.Argument, list);

            // Weight
            list = new[] 
            {
                propHelper.WeightDataColumn(),
                propHelper.Weight(),
                propHelper.ListOfWeights()
            };
            objHelper.Add(StiPropertyCategories.Weight, list);

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
                propHelper.LabelsOffset(),
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
                propHelper.ShowNulls(),
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
            IStiBubbleSeries series = base.Clone() as IStiBubbleSeries;

            return series;
        }
        #endregion

        #region Methods.Types
        public override Type GetDefaultAreaType()
        {
            return typeof(StiBubbleArea);
        }
        #endregion

        #region Properties
        [Browsable(false)]
        [StiNonSerialized]
        public override IStiLineMarker LineMarker
        {
            get
            {
                return base.LineMarker;
            }
            set
            {
                base.LineMarker = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public override IStiMarker Marker
        {
            get
            {
                return base.Marker;
            }
            set
            {
                base.Marker = value;
            }
        }

        [Browsable(false)]
        public override Color LineColor
        {
            get
            {
                return base.LineColor;
            }
            set
            {
                base.LineColor = value;
            }
        }
        [Browsable(false)]
        public override Color LineColorNegative
        {
            get
            {
                return base.LineColorNegative;
            }
            set
            {
                base.LineColorNegative = value;
            }
        }

        [Browsable(false)]
        public override bool AllowApplyColorNegative
        {
            get
            {
                return base.AllowApplyColorNegative;
            }
            set
            {
                base.AllowApplyColorNegative = value;
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
        public Color BorderColor
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

        /// <summary>
        /// Gets or sets border thickness of series bar.
        /// </summary>
		[StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets border thickness of series bar.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public int BorderThickness { get; set; } = 1;

        private StiBrush brush = new StiSolidBrush(Color.Gainsboro);
        /// <summary>
        /// Gets or sets brush which will used to fill bar area.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets brush which will used to fill bar area.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiBrush Brush
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


        private double[] weights = { 1, 3, 2 };
        [Browsable(false)]
        public double[] Weights
        {
            get
            {
                if (Chart == null || ((StiChart) Chart).Report == null || !Chart.IsDesigning || IsDashboard)
                    return weights;

                if (!string.IsNullOrEmpty(ListOfWeights.Value))
                    return GetValuesFromString(ListOfWeights.Value);

                var seriesIndex = Chart.Series.IndexOf(this);
                if (seriesIndex == 0)
                    return weights;

                return new double[] { 3 + seriesIndex * 3, 5 + seriesIndex * 4, 9 + seriesIndex * 3 };
            }
            set
            {
                weights = value;
            }
        }


        [Browsable(false)]
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        public string WeightsString
        {
            get
            {
                var sb = new StringBuilder();
                bool first = true;
                foreach (double value in weights)
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
                    weights = new double[0];
                }
                else
                {
                    string[] strs = value.Split(new char[] { ';' });

                    weights = new double[strs.Length];

                    int index = 0;
                    foreach (string str in strs)
                    {
                        weights[index++] = double.Parse(str);
                    }
                }
            }
        }


        private string weightDataColumn = string.Empty;
        /// <summary>
        /// Gets or sets a name of the column that contains the weight value.
        /// </summary>
        [DefaultValue("")]
        [StiSerializable]
        [StiOrder(StiSeriesPropertyOrder.WeightWeightDataColumn)]
        [Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Weight")]
        [Description("Gets or sets a name of the column that contains the weight value.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public string WeightDataColumn
        {
            get
            {
                return weightDataColumn;
            }
            set
            {
                weightDataColumn = value;
            }
        }

        [DefaultValue(null)]
        [StiSerializable]
        [StiCategory("Common")]
        [Editor("Stimulsoft.Report.Design.StiFontIconEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(Stimulsoft.Report.Design.StiFontIconConverter))]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiFontIcons? Icon { get; set; }
        #endregion

        #region Events
        #region GetWeight
        public event StiGetValueEventHandler GetWeight;

        /// <summary>
        /// Raises the GetWeight event.
        /// </summary>
        protected virtual void OnGetWeight(StiGetValueEventArgs e)
        {
        }


        /// <summary>
        /// Raises the GetWeight event.
        /// </summary>
        public virtual void InvokeGetWeight(StiComponent sender, StiGetValueEventArgs e)
        {
            try
            {
                OnGetWeight(e);
                if (this.GetWeight != null) this.GetWeight(sender, e);

                StiBlocklyHelper.InvokeBlockly(((StiChart)this.Chart).Report, sender, GetWeightEvent, e);
            }
            catch (Exception ex)
            {
                StiLogService.Write(this.GetType(), ((StiChart)Chart).Name + "Series InvokeGetWeight...ERROR");
                StiLogService.Write(this.GetType(), ((StiChart)Chart).Name + "Series " + ex.Message);

                ((StiChart)Chart).Report.WriteToReportRenderingMessages(((StiChart)Chart).Name + "Series InvokeGetWeight...ERROR");
            }
        }


        private StiGetValueEvent getWeightEvent = new StiGetValueEvent();
        /// <summary>
        /// Gets or sets a script of the event GetWeightEvent.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Gets or sets a script of the event GetWeightEvent.")]
        public StiGetValueEvent GetWeightEvent
        {
            get
            {
                return getWeightEvent;
            }
            set
            {
                getWeightEvent = value;
            }
        }
        #endregion

        #region GetListOfWeights
        /// <summary>
        /// Occurs when getting the list of weight.
        /// </summary>
        public event StiGetValueEventHandler GetListOfWeights;

        /// <summary>
        /// Raises the values end event.
        /// </summary>
        protected virtual void OnGetListOfWeights(StiGetValueEventArgs e)
        {
        }


        /// <summary>
        /// Raises the GetListOfWeights event.
        /// </summary>
        public void InvokeGetListOfWeights(StiComponent sender, StiGetValueEventArgs e)
        {
            try
            {
                OnGetListOfWeights(e);
                if (sender.Report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    var tempText = new StiText
                    {
                        Name = "**ChartSeriesListOfWeights**",
                        Page = sender.Report.Pages[0]
                    };
                    var parserResult = Engine.StiParser.ParseTextValue(ListOfWeights, tempText);
                    e.Value = sender.Report.ToString(parserResult);
                }
                if (this.GetListOfWeights != null) this.GetListOfWeights(sender, e);
            }
            catch (Exception ex)
            {
                StiLogService.Write(this.GetType(), "InvokeGetListOfWeights...Warning");
                StiLogService.Write(this.GetType(), ex.Message);

                ((StiChart)Chart).Report.WriteToReportRenderingMessages(((StiChart)Chart).Name + "InvokeGetListOfWeights...ERROR");
            }
        }


        private StiGetListOfWeightsEvent getListOfWeightsEvent = new StiGetListOfWeightsEvent();
        /// <summary>
        /// Gets or sets a script of the event GetListOfWeightsEvent.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Gets or sets a script of the event GetListOfWeightsEvent.")]
        public StiGetListOfWeightsEvent GetListOfWeightsEvent
        {
            get
            {
                return getListOfWeightsEvent;
            }
            set
            {
                getListOfWeightsEvent = value;
            }
        }
        #endregion
        #endregion

        #region Expressions
        #region Weight
        private StiExpression weight = new StiExpression();
        /// <summary>
        /// Gets or sets expression for weight calculating. Example: {Order.Value}
        /// </summary>
        [StiOrder(StiSeriesPropertyOrder.WeightWeight)]
        [StiCategory("Weight")]
        [StiSerializable]
        [Description("Gets or sets expression for weight calculating. Example: {Order.Value}")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiExpression Weight
        {
            get
            {
                return this.weight;
            }
            set
            {
                this.weight = value;
            }
        }
        #endregion

        #region ListOfWeights
        private StiListOfWeightsExpression listOfWeights = new StiListOfWeightsExpression();
        /// <summary>
        /// Gets or sets the expression to fill a list of weights.  Example: 1;2;3
        /// </summary>
        [StiCategory("Weight")]
        [StiOrder(StiSeriesPropertyOrder.WeightListOfWeights)]
        [StiSerializable(
             StiSerializeTypes.SerializeToCode |
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets the expression to fill a list of weights.  Example: 1;2;3")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiListOfWeightsExpression ListOfWeights
        {
            get
            {
                return listOfWeights;
            }
            set
            {
                listOfWeights = value;
            }
        }
        #endregion
        #endregion        
        
        #region Methods.override
        public override StiSeries CreateNew()
        {
            return new StiBubbleSeries();
        }
        #endregion

        public StiBubbleSeries()
        {
            this.Core = new StiBubbleSeriesCoreXF(this);
        }
    }
}