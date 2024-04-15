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
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Chart.Design;
using Stimulsoft.Report.Components;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(StiChartConditionConverter))]
    public class StiChartCondition :
        StiChartFilter,
        IStiChartCondition
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyColor("Color", Color, Color.White);
            jObject.AddPropertyEnum("MarkerType", MarkerType, StiMarkerType.Circle);
            jObject.AddPropertyFloat("MarkerAngle", MarkerAngle);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Color":
                        this.Color = property.DeserializeColor();
                        break;

                    case "MarkerType":
                        this.MarkerType = property.DeserializeEnum<StiMarkerType>();
                        break;

                    case "MarkerAngle":
                        this.MarkerAngle = property.DeserializeFloat();
                        break;
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a series color.
        /// </summary>
        [StiCategory("Appearance")]
        [StiSerializable()]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets a series color.")]
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Gets or sets rotation angle of the marker.
        /// </summary>
        [DefaultValue(0f)]
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets rotation angle of the marker.")]
        public virtual float MarkerAngle { get; set; } = 0f;

        /// <summary>
        /// Gets or sets type of the marker.
        /// </summary>
        [DefaultValue(StiMarkerType.Circle)]
        [StiSerializable]
        [StiCategory("Appearance")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets type of the marker.")]
        public StiMarkerType MarkerType { get; set; } = StiMarkerType.Circle;

        [Browsable(false)]
        internal StiChartConditionsCollection Conditions { get; set; }
        #endregion

        /// <summary>
		/// Creates a new object of the type StiChartCondition.
		/// </summary>
		public StiChartCondition()
        {
        }

        /// <summary>
        /// Creates a new object of the type StiChartCondition.
        /// </summary>
        public StiChartCondition(Color color, StiFilterItem item, StiFilterDataType dataType,
            StiFilterCondition condition, string value) :
            base(item, dataType, condition, value)
        {
            this.Color = color;
        }

        /// <summary>
        /// Creates a new object of the type StiChartCondition.
        /// </summary>
        public StiChartCondition(Color color, StiFilterItem item, StiFilterDataType dataType,
            StiFilterCondition condition, string value, StiMarkerType markerType, float markerAngle) :
            base(item, dataType, condition, value)
        {
            this.Color = color;
            this.MarkerType = markerType;
            this.MarkerAngle = markerAngle;
        }
    }
}