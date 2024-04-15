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
using System;
using System.ComponentModel;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    public abstract class StiRadarArea :
        StiArea,
        IStiRadarArea
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyJObject(nameof(InterlacingHor), interlacingHor.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(InterlacingVert), interlacingVert.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(GridLinesHor), gridLinesHor.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(GridLinesVert), gridLinesHor.SaveToJsonObject(mode));
            jObject.AddPropertyEnum(nameof(RadarStyle), RadarStyle, StiRadarStyle.Circle);
            jObject.AddPropertyJObject(nameof(XAxis), xAxis.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(YAxis), yAxis.SaveToJsonObject(mode));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(InterlacingHor):
                        this.interlacingHor.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(InterlacingVert):
                        this.interlacingVert.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(GridLinesHor):
                        this.gridLinesHor.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(GridLinesVert):
                        this.gridLinesVert.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(RadarStyle):
                        this.RadarStyle = property.DeserializeEnum<StiRadarStyle>();
                        break;

                    case nameof(XAxis):
                        {
                            this.xAxis.LoadFromJsonObject((JObject)property.Value);
                            if (((StiRadarAxis)this.xAxis).jsonLoadFromJsonObjectArea)
                            {
                                ((StiRadarAxis)this.xAxis).jsonLoadFromJsonObjectArea = false;
                                this.xAxis.Area = this;
                            }
                        }
                        break;

                    case nameof(YAxis):
                        {
                            this.yAxis.LoadFromJsonObject((JObject)property.Value);
                            if (((StiRadarAxis)this.yAxis).jsonLoadFromJsonObjectArea)
                            {
                                ((StiRadarAxis)this.yAxis).jsonLoadFromJsonObjectArea = false;
                                this.yAxis.Area = this;
                            }
                        }
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
            var area = base.Clone() as IStiRadarArea;

            area.InterlacingHor = this.interlacingHor.Clone() as IStiInterlacingHor;
            area.InterlacingVert = this.interlacingVert.Clone() as IStiInterlacingVert;
            area.GridLinesHor = this.gridLinesHor.Clone() as IStiRadarGridLinesHor;
            area.GridLinesVert = this.gridLinesVert.Clone() as IStiRadarGridLinesVert;

            area.XAxis = this.XAxis.Clone() as IStiXRadarAxis;
            area.YAxis = this.YAxis.Clone() as IStiYRadarAxis;

            return area;
        }
        #endregion

        #region Methods.Types
        public override Type GetDefaultSeriesLabelsType()
        {
            return typeof(StiNoneLabels);
        }

        public override Type[] GetSeriesLabelsTypes()
        {
            return new[]
            {
                typeof(StiNoneLabels),
                typeof(StiOutsideBaseAxisLabels),
                typeof(StiOutsideEndAxisLabels),
                typeof(StiOutsideAxisLabels),
                typeof(StiValueAxisLabels)
			};
        }
        #endregion

        #region Properties
        [DefaultValue(true)]
        [StiNonSerialized]
        [Browsable(false)]
        public override bool ColorEach
        {
            get
            {
                return base.ColorEach;
            }
            set
            {
                base.ColorEach = value;
            }
        }

        private IStiInterlacingHor interlacingHor;
        /// <summary>
        /// Gets or sets interlacing settings on horizontal axis.
        /// </summary>
        [Description("Gets or sets interlacing settings on horizontal axis.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        [Browsable(false)]
        public IStiInterlacingHor InterlacingHor
        {
            get
            {
                return interlacingHor;
            }
            set
            {
                interlacingHor = value;
                if (interlacingHor != null)
                    interlacingHor.Area = this;
            }
        }

        private IStiInterlacingVert interlacingVert;
        /// <summary>
        /// Gets or sets interlacing settings on vertical axis.
        /// </summary>
        [Description("Gets or sets interlacing settings on vertical axis.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        [Browsable(false)]
        public IStiInterlacingVert InterlacingVert
        {
            get
            {
                return interlacingVert;
            }
            set
            {
                interlacingVert = value;
                if (interlacingVert != null)
                    interlacingVert.Area = this;
            }
        }

        private IStiRadarGridLinesHor gridLinesHor;
        /// <summary>
        /// Gets or sets horizontal grid lines on left axis.
        /// </summary>
        [Description("Gets or sets horizontal grid lines on left axis.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        [Browsable(false)]
        public IStiRadarGridLinesHor GridLinesHor
        {
            get
            {
                return gridLinesHor;
            }
            set
            {
                gridLinesHor = value;
                if (gridLinesHor != null)
                    gridLinesHor.Area = this;
            }
        }

        private IStiRadarGridLinesVert gridLinesVert;
        /// <summary>
        /// Gets or sets grid lines on vertical axis.
        /// </summary>
        [Description("Gets or sets grid lines on vertical axis.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        [Browsable(false)]
        public IStiRadarGridLinesVert GridLinesVert
        {
            get
            {
                return gridLinesVert;
            }
            set
            {
                gridLinesVert = value;
                if (gridLinesVert != null)
                    gridLinesVert.Area = this;
            }
        }

        /// <summary>
        /// Gets or sets style of radar area.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets style of radar area.")]
        [DefaultValue(StiRadarStyle.Circle)]
        [StiCategory("Common")]
        public StiRadarStyle RadarStyle { get; set; } = StiRadarStyle.Circle;
        
        private IStiXRadarAxis xAxis;
        /// <summary>
        /// Gets or sets settings of X Axis.
        /// </summary>
        [Description("Gets or sets settings of X Axis.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        [Browsable(false)]
        public IStiXRadarAxis XAxis
        {
            get
            {
                return xAxis;
            }
            set
            {
                xAxis = value;
                if (xAxis != null)
                    xAxis.Area = this;
            }
        }

        private IStiYRadarAxis yAxis;
        /// <summary>
        /// Gets or sets settings of Y Axis.
        /// </summary>
        [Description("Gets or sets settings of Y Axis.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        [Browsable(false)]
        public IStiYRadarAxis YAxis
        {
            get
            {
                return yAxis;
            }
            set
            {
                yAxis = value;
                if (yAxis != null)
                    yAxis.Area = this;
            }
        }
        #endregion

        public StiRadarArea()
        {
            XAxis = new StiXRadarAxis();
            YAxis = new StiYRadarAxis();

            InterlacingHor = new StiInterlacingHor();
            InterlacingVert = new StiInterlacingVert();

            GridLinesHor = new StiRadarGridLinesHor();
            GridLinesVert = new StiRadarGridLinesVert();

        }
    }
}
