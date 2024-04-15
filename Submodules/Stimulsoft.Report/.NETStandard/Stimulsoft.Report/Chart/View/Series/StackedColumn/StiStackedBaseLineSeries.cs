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
using System.Drawing.Design;
using System.ComponentModel;
using System.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    public abstract class StiStackedBaseLineSeries : 
        StiSeries,
        IStiStackedBaseLineSeries
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("TrendLine");
            jObject.RemoveProperty("Conditions");

            jObject.AddPropertyBool("ShowNulls", showNulls, true);
            jObject.AddPropertyJObject("Marker", marker.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("LineMarker", LineMarker.SaveToJsonObject(mode));
            jObject.AddPropertyBool("Lighting", lighting, true);
            jObject.AddPropertyColor("LineColor", lineColor, Color.MediumBlue);
            jObject.AddPropertyFloat("LineWidth", lineWidth, 2f);
            jObject.AddPropertyEnum("LineStyle", lineStyle, StiPenStyle.Solid);
            jObject.AddPropertyColor("LineColorNegative", LineColorNegative, Color.Firebrick);
            jObject.AddPropertyBool("AllowApplyColorNegative", allowApplyColorNegative);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "ShowNulls":
                        this.showNulls = property.DeserializeBool();
                        break;

                    case "Marker":
                        this.marker.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "LineMarker":
                        this.LineMarker.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "Lighting":
                        this.lighting = property.DeserializeBool();
                        break;

                    case "LineColor":
                        this.lineColor = property.DeserializeColor();
                        break;

                    case "LineWidth":
                        this.lineWidth = property.DeserializeFloat();
                        break;

                    case "LineStyle":
                        this.lineStyle = property.DeserializeEnum<StiPenStyle>();
                        break;

                    case "LineColorNegative":
                        this.LineColorNegative = property.DeserializeColor();
                        break;

                    case "AllowApplyColorNegative":
                        this.allowApplyColorNegative = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

		#region ICloneable override
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			IStiStackedBaseLineSeries series =	base.Clone() as IStiStackedBaseLineSeries;
			series.LineStyle = this.LineStyle;
			
			return series;
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

        private bool showNulls = true;
        /// <summary>
        /// Gets or sets value which indicates whether it is necessary to show the series element, if the series value of this bar is null.
        /// </summary>
        [StiSerializable]
        [StiCategory("Common")]
        [DefaultValue(true)]
        [Description("Gets or sets value which indicates whether it is necessary to show the series element, if the series value of this bar is null.")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Standard)]
        public bool ShowNulls
        {
            get
            {
                return showNulls;
            }
            set
            {
                showNulls = value;
            }
        }

        [Obsolete("Please use Marker.Visible property instead ShowMarker property.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [StiNonSerialized]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        public bool ShowMarker
        {
            get
            {
                return this.Marker.Visible;
            }
            set
            {
                this.Marker.Visible = value;
            }
        }


        [Obsolete("Please use Marker.Brush property instead MarkerColor property.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [StiNonSerialized]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color MarkerColor
        {
            get
            {
                return StiBrush.ToColor(this.Marker.Brush);
            }
            set
            {
                this.Marker.Brush = new StiSolidBrush(value);
                this.Marker.BorderColor = StiColorUtils.Dark(value, 50);
            }
        }


        [Obsolete("Please use Marker.Size property instead MarkerSize property.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [StiNonSerialized]
        [DefaultValue(6f)]
        public float MarkerSize
        {
            get
            {
                return this.Marker.Size;
            }
            set
            {
                this.Marker.Size = value;
            }
        }


        [Obsolete("Please use Marker.Type property instead MarkerType property.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [StiNonSerialized]
        [DefaultValue(StiMarkerType.Circle)]
        [TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
        public StiMarkerType MarkerType
        {
            get
            {
                return ((StiMarker)this.Marker).Type;
            }
            set
            {
                ((StiMarker)this.Marker).Type = value;
            }
        }


        private IStiMarker marker = new StiMarker();
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiCategory("Appearance")]
        [Description("Gets or sets marker settings.")]
        [Browsable(false)]
        [TypeConverter(typeof(Stimulsoft.Report.Chart.Design.StiMarkerConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        public IStiMarker Marker
        {
            get
            {
                return marker;
            }
            set
            {
                marker = value;
            }
        }


        private IStiLineMarker lineMarker = new StiLineMarker();
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiCategory("Appearance")]
        [Description("Gets or sets line marker settings.")]
        [Browsable(false)]
        [TypeConverter(typeof(Stimulsoft.Report.Chart.Design.StiLineMarkerConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual IStiLineMarker LineMarker
        {
            get
            {
                return lineMarker;
            }
            set
            {
                lineMarker = value;
            }
        }


		[StiNonSerialized]
		[Browsable(false)]
		public override StiChartConditionsCollection Conditions
		{
			get
			{
				return base.Conditions;
			}
			set
			{
				base.Conditions = value;
			}
		}


	    private bool lighting = true;
		[DefaultValue(true)]
		[StiSerializable]
		[StiCategory("Appearance")]
		[TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        public bool Lighting
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

       
		private Color lineColor = Color.MediumBlue;
		[StiSerializable]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiCategory("Appearance")]
        [StiPropertyLevel(StiLevel.Standard)]
        public Color LineColor
		{
			get
			{
				return lineColor;
			}
			set
			{
				lineColor = value;
			}
		}


		private float lineWidth = 2f;
		[DefaultValue(2f)]
		[StiSerializable]
		[StiCategory("Common")]
        [StiPropertyLevel(StiLevel.Standard)]
        public float LineWidth
		{
			get
			{
				return lineWidth;
			}
			set
			{
				if (value > 0)
				{
					lineWidth = value;
				}
			}
		}


		private StiPenStyle lineStyle = StiPenStyle.Solid;
        /// <summary>
        /// Gets or sets a border style.
        /// </summary>
        [Editor(StiEditors.PenStyle, typeof(UITypeEditor))]
        [DefaultValue(StiPenStyle.Solid)]
		[Description("Gets or sets a border style.")]
		[StiSerializable]
		[StiCategory("Common")]
		[TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiPenStyle LineStyle
		{
			get
			{
				return lineStyle;
			}
			set
			{
				lineStyle = value;
			}
		}


        private Color lineColorNegative = Color.Firebrick;
        [StiSerializable]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Appearance")]
        [Description("Gets or sets a line color of series for negative values.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual Color LineColorNegative
        {
            get
            {
                return lineColorNegative;
            }
            set
            {
                lineColorNegative = value;
            }
        }

        private bool allowApplyColorNegative = false;
        [StiSerializable]
        [StiCategory("Common")]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates that the specific color for negative values will be used.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public bool AllowApplyColorNegative
        {
            get
            {
                return allowApplyColorNegative;
            }
            set
            {
                allowApplyColorNegative = value;
            }
        }
        #endregion  
      
        #region Methods.Types
        public override Type GetDefaultAreaType()
        {
            return typeof(StiStackedColumnArea);
        }
        #endregion
    }
}
