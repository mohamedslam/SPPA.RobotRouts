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
    public abstract class StiBaseLineSeries : 
        StiSeries,
        IStiBaseLineSeries
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyBool("ShowZeros", ShowZeros, true);
            jObject.AddPropertyBool("ShowNulls", ShowNulls, true);
            jObject.AddPropertyJObject("Marker", Marker.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("LineMarker", LineMarker.SaveToJsonObject(mode));
            jObject.AddPropertyColor("LineColor", LineColor, Color.Black);
            jObject.AddPropertyEnum("LineStyle", LineStyle, StiPenStyle.Solid);
            jObject.AddPropertyBool("Lighting", Lighting, true);
            jObject.AddPropertyFloat("LineWidth", LineWidth, 2f);
            jObject.AddPropertyInt("LabelsOffset", labelsOffset);
            jObject.AddPropertyColor("LineColorNegative", LineColorNegative, Color.Firebrick);
            jObject.AddPropertyBool("AllowApplyColorNegative", AllowApplyColorNegative);
            jObject.AddPropertyEnum("ShowNullsAs", ShowNullsAs, StiShowEmptyCellsAs.Gap);
            jObject.AddPropertyEnum("ShowZerosAs", ShowZerosAs, StiShowEmptyCellsAs.Gap);

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

                    case "ShowNulls":
                        this.ShowNulls = property.DeserializeBool();
                        break;

                    case "Marker":
                        this.Marker.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "LineMarker":
                        this.LineMarker.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "LineColor":
                        this.LineColor = property.DeserializeColor();
                        break;
                        
                    case "LineStyle":
                        this.LineStyle = property.DeserializeEnum<StiPenStyle>();
                        break;

                    case "Lighting":
                        this.Lighting = property.DeserializeBool();
                        break;
                        
                    case "LineWidth":
                        this.LineWidth = property.DeserializeFloat();
                        break;
                        
                    case "LabelsOffset":
                        this.labelsOffset = property.DeserializeInt();
                        break;
                        
                    case "LineColorNegative":
                        this.LineColorNegative = property.DeserializeColor();
                        break;
                        
                    case "AllowApplyColorNegative":
                        this.AllowApplyColorNegative = property.DeserializeBool();
                        break;

                    case "ShowNullsAs":
                        this.ShowNullsAs = property.DeserializeEnum<StiShowEmptyCellsAs>();
                        break;

                    case "ShowZerosAs":
                        this.ShowZerosAs = property.DeserializeEnum<StiShowEmptyCellsAs>(); 
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
			IStiBaseLineSeries series =	base.Clone() as IStiBaseLineSeries;
            
            series.Marker = this.Marker.Clone() as IStiMarker;

            series.LineStyle = this.LineStyle;            

			return series;
		}
		#endregion

        #region Properties
        private bool showNulls = true;
        /// <summary>
        /// Gets or sets value which indicates whether it is necessary to show the series element, if the series value of this bar is null.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [DefaultValue(true)]
        [Description("Gets or sets value which indicates whether it is necessary to show the series element, if the series value of this bar is null.")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool ShowNulls
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

        private bool showZeros = true;
        /// <summary>
        /// Gets or sets value which indicates whether it is necessary to show the series element, if the series value is 0.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("Data")]
        [Description("Gets or sets value which indicates whether it is necessary to show the series element, if the series value is 0.")]
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


        [Obsolete("Please use Marker.Visible property instead ShowMarker property.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [StiNonSerialized]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
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
        /// <summary>
        /// Gets or sets marker settings.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiCategory("Appearance")]
        [Description("Gets or sets marker settings.")]
        [TypeConverter(typeof(Stimulsoft.Report.Chart.Design.StiMarkerConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual IStiMarker Marker
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
        /// <summary>
        /// Gets or sets line marker settings.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiCategory("Appearance")]
        [Description("Gets or sets line marker settings.")]
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

        
        private Color lineColor = Color.Black;
        /// <summary>
        /// Gets or sets line color of series.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Appearance")]
        [Description("Gets or sets line color of series.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual Color LineColor
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

                
        private StiPenStyle lineStyle = StiPenStyle.Solid;
        /// <summary>
        /// Gets or sets a line style of series.
        /// </summary>
        [Editor(StiEditors.PenStyle, typeof(UITypeEditor))]
        [DefaultValue(StiPenStyle.Solid)]
        [Description("Gets or sets a line style of series.")]
		[StiSerializable]
		[StiCategory("Common")]
		[TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiPenStyle LineStyle
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


		private bool lighting = true;
        /// <summary>
        /// Gets or sets value which indicates that light effect will be shown.
        /// </summary>
		[DefaultValue(true)]
		[StiSerializable]
		[StiCategory("Appearance")]
		[TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that light effect will be shown.")]
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
		

		private float lineWidth = 2f;
        /// <summary>
        /// Gets or sets line width of series.
        /// </summary>
		[DefaultValue(2f)]
		[StiSerializable]
		[StiCategory("Common")]
        [Description("Gets or sets line width of series.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual float LineWidth
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


        private int labelsOffset = 0;
        /// <summary>
        /// Gets or sets vertical labels offset.
        /// </summary>
        [DefaultValue(0)]
        [StiSerializable]
        [StiCategory("Common")]
        [Description("Gets or sets vertical labels offset.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public int LabelsOffset
        {
            get
            {
                return labelsOffset;
            }                                   
            set
            {
                labelsOffset = value;
            }
        }

        private Color lineColorNegative = Color.Firebrick;
        /// <summary>
        /// Gets or sets a line color of series for negative values.
        /// </summary>
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
        /// <summary>
        /// Gets or sets a value which indicates that the specific color for negative values will be used.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates that the specific color for negative values will be used.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool AllowApplyColorNegative
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

        /// <summary>
        /// Internal use only. Special for DBS.
        /// </summary>
        [StiNonSerialized]
        [Browsable(false)]
        public StiShowEmptyCellsAs ShowNullsAs { get; set; } = StiShowEmptyCellsAs.Gap;

        /// <summary>
        /// Internal use only. Special for DBS.
        /// </summary>
        [StiNonSerialized]
        [Browsable(false)]
        public StiShowEmptyCellsAs ShowZerosAs { get; set; } = StiShowEmptyCellsAs.Gap;
        #endregion

        public StiBaseLineSeries()
        {
            this.SeriesLabels = new StiOutsideEndAxisLabels();
        }
    }
}
