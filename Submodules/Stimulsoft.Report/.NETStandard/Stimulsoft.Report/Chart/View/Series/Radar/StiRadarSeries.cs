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

using System.ComponentModel;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    public abstract class StiRadarSeries : 
        StiSeries,
        IStiRadarSeries
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("TrendLine");

            jObject.AddPropertyBool("ShowNulls", showNulls, true);
            jObject.AddPropertyJObject("Marker", Marker.SaveToJsonObject(mode));

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
                        this.Marker.LoadFromJsonObject((JObject)property.Value);
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
            IStiRadarSeries series = base.Clone() as IStiRadarSeries;

            if (this.marker != null)
                series.Marker = this.marker.Clone() as IStiMarker;
			
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

        private IStiMarker marker = new StiMarker();
        /// <summary>
        /// Gets or sets marker settings.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiCategory("Common")]
        [Description("Gets or sets marker settings.")]
        [Browsable(false)]
        [TypeConverter(typeof(Stimulsoft.Report.Chart.Design.StiLineMarkerConverter))]
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
        #endregion
	}
}
