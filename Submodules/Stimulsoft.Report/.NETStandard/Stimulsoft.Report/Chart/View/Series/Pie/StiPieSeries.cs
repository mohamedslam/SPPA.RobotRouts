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
using System.Drawing;
using System.Drawing.Design;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
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
    public class StiPieSeries : 
        StiSeries,
        IStiPieSeries
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("TrendLine");

            jObject.AddPropertyBool("ShowZeros", ShowZeros);
            jObject.AddPropertyBool("AllowApplyBrush", allowApplyBrush, true);
            jObject.AddPropertyBool("AllowApplyBorderColor", allowApplyBorderColor, true);
            jObject.AddPropertyFloat("StartAngle", StartAngle, 0f);
            jObject.AddPropertyColor("BorderColor", borderColor, Color.Gray);
            jObject.AddPropertyInt("BorderThickness", BorderThickness, 1);
            jObject.AddPropertyBrush("Brush", brush);
            jObject.AddPropertyBool("Lighting", lighting, true);
            jObject.AddPropertyFloat("Diameter", Diameter, 0f);
            jObject.AddPropertyFloat("Distance", Distance, 0f);
            jObject.AddPropertyJObject("CutPieList", cutPieList.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetCutPieListEvent", getCutPieListEvent.SaveToJsonObject(mode));

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
                    case "ShowZeros":
                        this.ShowZeros = property.DeserializeBool();
                        break;

                    case "AllowApplyBrush":
                        this.allowApplyBrush = property.DeserializeBool();
                        break;

                    case "AllowApplyBorderColor":
                        this.allowApplyBorderColor = property.DeserializeBool();
                        break;

                    case "StartAngle":
                        this.StartAngle = property.DeserializeFloat();
                        break;

                    case "BorderColor":
                        this.borderColor = property.DeserializeColor();
                        break;

                    case "BorderThickness":
                        this.BorderThickness = property.DeserializeInt();
                        break;

                    case "Brush":
                        this.brush = property.DeserializeBrush();
                        break;

                    case "Lighting":
                        this.lighting = property.DeserializeBool();
                        break;

                    case "Diameter":
                        this.Diameter = property.DeserializeFloat();
                        break;

                    case "Distance":
                        this.Distance = property.DeserializeFloat();
                        break;

                    case "CutPieList":
                        this.cutPieList.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "GetCutPieListEvent":
                        this.getCutPieListEvent.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "Icon":
                        this.Icon = property.DeserializeEnum<StiFontIcons>();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiPieSeries;

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
                propHelper.AllowApplyBorderColor(),
                propHelper.AllowApplyBrush(),
                propHelper.Diameter(),
                propHelper.BorderColor(),
                propHelper.Brush(),
                propHelper.Lighting(),
                propHelper.ShowShadow()
            };
            objHelper.Add(StiPropertyCategories.Appearance, list);

            // Behavior
            list = new[] 
            {
                propHelper.AllowApplyStyle(),
                propHelper.CutPieList(),
                propHelper.Distance(),
                propHelper.ShowInLegend(),
                propHelper.ShowSeriesLabels(),
                propHelper.ShowZeros(),
                propHelper.StartAngle(),
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
            var series = base.Clone() as IStiPieSeries;

            return series;
        }
		#endregion

        #region Methods.Types
        public override Type GetDefaultAreaType()
        {
            return typeof(StiPieArea);
        }
        #endregion

        #region Properties
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

        private bool showZeros = false;
        /// <summary>
        /// Gets or sets value which indicates whether it is necessary to show the series element, if the series value of this column is 0.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [DefaultValue(false)]
        [Description("Gets or sets value which indicates whether it is necessary to show the series element, if the series value of this column is 0.")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool ShowZeros
        {
            get
            {
                return showZeros;
            }
            set
            {
                showZeros = value;
            }
        }

        private bool allowApplyBrush = true;
        /// <summary>
        /// Gets or sets value which allow to use brush from series settings.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which allow to use brush from series settings.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public bool AllowApplyBrush
        {
            get
            {
                return allowApplyBrush;
            }
            set
            {
                if (allowApplyBrush != value)
                {
                    allowApplyBrush = value;
                }
            }
        }


        private bool allowApplyBorderColor = true;
        /// <summary>
        /// Gets or sets value which allow to use border color from series settings.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which allow to use border color from series settings.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public bool AllowApplyBorderColor
        {
            get
            {
                return allowApplyBorderColor;
            }
            set
            {
                if (allowApplyBorderColor != value)
                {
                    allowApplyBorderColor = value;
                }
            }
        }


		public override object[] Arguments
		{
			get
			{
			    if (base.Arguments.Length != 0 || Chart == null || ((StiChart) Chart).Report == null || !((StiChart) Chart).Report.IsDesigning || IsDashboard)
			        return base.Arguments;

			    return new object[]
			    {
			        "Arg1",
			        "Arg2",
			        "Arg3"
			    };
			}
			set
			{
				base.Arguments = value;
			}
		}


		private float startAngle = 0f;
        /// <summary>
        /// Gets or sets start rotation angle of series.
        /// </summary>
		[StiSerializable]
		[DefaultValue(0f)]
		[StiCategory("Common")]
        [Description("Gets or sets start rotation angle of series.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual float StartAngle
		{
			get
			{
				return startAngle;
			}
			set
			{
				startAngle = value;
			}
		}


		private Color borderColor = Color.Gray;
        /// <summary>
        /// Gets or sets border color of series pie.
        /// </summary>
		[StiSerializable]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiCategory("Appearance")]
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
        /// Gets or sets border thickness of series pie.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets border thickness of series pie.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual int BorderThickness { get; set; } = 1;


        private StiBrush brush = new StiSolidBrush(Color.Gainsboro);
        /// <summary>
        /// Gets or sets brush which will used to fill pie area.
        /// </summary>
		[RefreshProperties(RefreshProperties.All)]
		[StiSerializable]
		[StiCategory("Appearance")]
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


		private bool lighting = true;
		[DefaultValue(true)]
		[StiSerializable]
		[StiCategory("Appearance")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual bool Lighting
		{
			get
			{
				return lighting;
			}
			set
			{
				lighting = value;
			}
		}


        private float diameter = 0f;
        /// <summary>
        /// Gets or sets fixed size of diameter of pie series.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0f)]
        [StiCategory("Common")]
        [Description("Gets or sets fixed size of diameter of pie series.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual float Diameter
        {
            get
            {
                return diameter;
            }
            set
            {
                diameter = value;
                if (value < 5) diameter = 0;
            }
        }


        private float distance = 0f;
        /// <summary>
        /// Gets or sets distance between the center of series and the center of each segment.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0f)]
        [StiCategory("Common")]
        [Description("Gets or sets distance between the center of series and the center of each segment.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual float Distance
        {
            get
            {
                return distance;
            }
            set
            {
                distance = Math.Max(0, value);
            }
        }

        private double[] cutPieListValues = { };
        [Browsable(false)]
        public double[] CutPieListValues
        {
            get
            {
                if (Chart == null || ((StiChart) Chart).Report == null || !Chart.IsDesigning || IsDashboard)
                    return cutPieListValues;

                if (!string.IsNullOrEmpty(CutPieList.Value))
                    return GetValuesFromString(CutPieList.Value);

                return new double[] { };
            }
            set
            {
                cutPieListValues = value;
            }
        }

        [DefaultValue(null)]
        [StiSerializable]
        [StiCategory("Common")]
        [Editor("Stimulsoft.Report.Design.StiFontIconEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(Stimulsoft.Report.Design.StiFontIconConverter))]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual StiFontIcons? Icon { get; set; }
        #endregion

        #region Expressions
        /// <summary>
        /// Gets or sets the expression to fill a list of cut pie segments. Example: 1;4;6
        /// </summary>
        [Browsable(false)]
        [StiNonSerialized]
        [Obsolete("This property is obsolete, use the CutPieList property.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiCutPieListExpression CuttedPieList
        {
            get
            {
                return CutPieList;
            }
            set
            {
                CutPieList = value;
            }
        }

        private StiCutPieListExpression cutPieList = new StiCutPieListExpression();
        /// <summary>
        /// Gets or sets the expression to fill a list of cut pie segments. Example: 1;4;6
        /// </summary>
        [StiSerializable(
             StiSerializeTypes.SerializeToCode |
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad)]
        [StiCategory("Behavior")]
        [Description("Gets or sets the expression to fill a list of cut pie segments. Example: 1;4;6")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiCutPieListExpression CutPieList
        {
            get
            {
                return cutPieList;
            }
            set
            {
                cutPieList = value;
            }
        }        
        #endregion

		#region Events
		#region GetCutPieList
		/// <summary>
        /// Occurs when getting the cut pie list.
        /// </summary>
        public event StiGetValueEventHandler GetCutPieList;

        /// <summary>
        /// Raises the values event.
        /// </summary>
        protected virtual void OnGetCutPieList(StiGetValueEventArgs e)
        {
        }


        /// <summary>
        /// Raises the GetCutPieList event.
        /// </summary>
        public void InvokeGetCutPieList(StiComponent sender, StiGetValueEventArgs e)
        {
            try
            {
                OnGetCutPieList(e);
                if (sender.Report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    StiText tempText = new StiText();
                    tempText.Name = "**ChartPieSeriesCutPieList**";
                    tempText.Page = sender.Report.Pages[0];
                    object parserResult = Engine.StiParser.ParseTextValue(CutPieList.Value, tempText);
                    e.Value = sender.Report.ToString(parserResult);
                }
                if (this.GetCutPieList != null) this.GetCutPieList(sender, e);
            }
            catch (Exception ex)
            {
                string str = string.Format("Expression in GetCutPieList property of '{0}' series from '{1}' chart can't be evaluated!", this.ServiceName, ((StiChart)Chart).Name);
                StiLogService.Write(this.GetType(), str);
                StiLogService.Write(this.GetType(), ex.Message);
                ((StiChart)Chart).Report.WriteToReportRenderingMessages(str);
            }
        }


        private StiGetCutPieListEvent getCutPieListEvent = new StiGetCutPieListEvent();
        /// <summary>
        /// Occurs when getting the cut pie list.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when getting the cut pie list.")]
        public StiGetCutPieListEvent GetCutPieListEvent
        {
            get
            {
                return getCutPieListEvent;
            }
            set
            {
                getCutPieListEvent = value;
            }
        }
        #endregion
		#endregion        
        
        #region Methods.override
        public override StiSeries CreateNew()
        {
            return new StiPieSeries();
        }
        #endregion

        public StiPieSeries()
        {
            this.Core = new StiPieSeriesCoreXF(this);
        }
	}
}
