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
    /// <summary>
    /// Describes base class for all axis areas.
    /// </summary>
    public abstract class StiAxisArea :
        StiArea,
        IStiAxisArea
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyJObject(nameof(InterlacingHor), interlacingHor.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(InterlacingVert), interlacingVert.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(GridLinesHor), gridLinesHor.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(GridLinesHorRight), gridLinesHorRight.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(GridLinesVert), gridLinesVert.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(YAxis), yAxis.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(YRightAxis), yRightAxis.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(XAxis), xAxis.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(XTopAxis), xTopAxis.SaveToJsonObject(mode));
            jObject.AddPropertyBool(nameof(ReverseHor), ReverseHor);
            jObject.AddPropertyBool(nameof(ReverseVert), ReverseVert);

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
                        {
                            this.interlacingHor.LoadFromJsonObject((JObject)property.Value);
                            if (((StiInterlacing)this.interlacingHor).needSetAreaJsonPropertyInternal)
                            {
                                ((StiInterlacing)this.interlacingHor).needSetAreaJsonPropertyInternal = false;
                                this.interlacingHor.Area = this;
                            }
                        }
                        break;

                    case nameof(InterlacingVert):
                        {
                            this.interlacingVert.LoadFromJsonObject((JObject)property.Value);
                            if (((StiInterlacing)this.interlacingVert).needSetAreaJsonPropertyInternal)
                            {
                                ((StiInterlacing)this.interlacingVert).needSetAreaJsonPropertyInternal = false;
                                this.interlacingVert.Area = this;
                            }
                        }
                        break;

                    case nameof(GridLinesHor):
                        {
                            this.gridLinesHor.LoadFromJsonObject((JObject)property.Value);
                            if (((StiGridLines)this.gridLinesHor).needSetAreaJsonPropertyInternal)
                            {
                                ((StiGridLines)this.gridLinesHor).needSetAreaJsonPropertyInternal = false;
                                this.gridLinesHor.Area = this;
                            }
                        }
                        break;

                    case nameof(GridLinesHorRight):
                        {
                            this.gridLinesHorRight.LoadFromJsonObject((JObject)property.Value);
                            if (((StiGridLines)this.gridLinesHorRight).needSetAreaJsonPropertyInternal)
                            {
                                ((StiGridLines)this.gridLinesHorRight).needSetAreaJsonPropertyInternal = false;
                                this.gridLinesHorRight.Area = this;
                            }
                        }
                        break;

                    case nameof(GridLinesVert):
                        {
                            this.gridLinesVert.LoadFromJsonObject((JObject)property.Value);
                            if (((StiGridLines)this.gridLinesVert).needSetAreaJsonPropertyInternal)
                            {
                                ((StiGridLines)this.gridLinesVert).needSetAreaJsonPropertyInternal = false;
                                this.gridLinesVert.Area = this;
                            }
                        }
                        break;

                    case nameof(YAxis):
                        this.yAxis.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(YRightAxis):
                        this.yRightAxis.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(XAxis):
                        this.xAxis.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(XTopAxis):
                        this.xTopAxis.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(ReverseHor):
                        this.ReverseHor = property.DeserializeBool();
                        break;

                    case nameof(ReverseVert):
                        this.ReverseVert = property.DeserializeBool();
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
            var area = base.Clone() as IStiAxisArea;

            area.InterlacingHor = this.interlacingHor.Clone() as IStiInterlacingHor;
            area.InterlacingVert = this.interlacingVert.Clone() as IStiInterlacingVert;
            area.GridLinesHor = this.gridLinesHor.Clone() as IStiGridLinesHor;
            area.GridLinesHorRight = this.gridLinesHorRight.Clone() as IStiGridLinesHor;
            area.GridLinesVert = this.gridLinesVert.Clone() as IStiGridLinesVert;
            area.YAxis = this.yAxis.Clone() as IStiYAxis;
            area.YRightAxis = this.yRightAxis.Clone() as IStiYAxis;
            area.XAxis = this.xAxis.Clone() as IStiXAxis;
            area.XTopAxis = this.xTopAxis.Clone() as IStiXAxis;

            return area;
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public StiAxisAreaCoreXF AxisCore => Core as StiAxisAreaCoreXF;
        
        private IStiInterlacingHor interlacingHor;
        /// <summary>
        /// Gets or sets interlacing settings on horizontal axis.
        /// </summary>
        [Description("Gets or sets interlacing settings on horizontal axis.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
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

        private bool ShouldSerializeInterlacingHor()
        {
            return InterlacingHor == null || !InterlacingHor.IsDefault;
        }

        private IStiInterlacingVert interlacingVert;
        /// <summary>
        /// Gets or sets interlacing settings on vertical axis.
        /// </summary>
        [Description("Gets or sets interlacing settings on vertical axis.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
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

        private bool ShouldSerializeInterlacingVert()
        {
            return InterlacingVert == null || !InterlacingVert.IsDefault;
        }

        private IStiGridLinesHor gridLinesHor;
        /// <summary>
        /// Gets or sets horizontal grid lines on left axis.
        /// </summary>
        [Description("Gets or sets horizontal grid lines on left axis.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        public IStiGridLinesHor GridLinesHor
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

        private bool ShouldSerializeGridLinesHor()
        {
            return GridLinesHor == null || !GridLinesHor.IsDefault;
        }

        private IStiGridLinesHor gridLinesHorRight;
        /// <summary>
        /// Gets or sets horizontal grid lines on right axis.
        /// </summary>
        [Description("Gets or sets horizontal grid lines on right axis.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        public IStiGridLinesHor GridLinesHorRight
        {
            get
            {
                return gridLinesHorRight;
            }
            set
            {
                gridLinesHorRight = value;

                if (gridLinesHorRight != null)
                    gridLinesHorRight.Area = this;
            }
        }

        private bool ShouldSerializeGridLinesHorRight()
        {
            return GridLinesHorRight == null || !GridLinesHorRight.IsDefault;
        }

        private IStiGridLinesVert gridLinesVert;
        /// <summary>
        /// Gets or sets grid lines on vertical axis.
        /// </summary>
        [Description("Gets or sets grid lines on vertical axis.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        public IStiGridLinesVert GridLinesVert
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

        private bool ShouldSerializeGridLinesVert()
        {
            return GridLinesVert == null || !GridLinesVert.IsDefault;
        }

        private IStiYAxis yAxis;
        /// <summary>
        /// Gets or sets settings of YAxis.
        /// </summary>
        [Description("Gets or sets settings of YAxis.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        public IStiYAxis YAxis
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

        private bool ShouldSerializeYAxis()
        {
            return YAxis == null || !YAxis.IsDefault;
        }

        private IStiYAxis yRightAxis;
        /// <summary>
        /// Gets or sets settings of YRightAxis.
        /// </summary>
        [Description("Gets or sets settings of YRightAxis.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        public IStiYAxis YRightAxis
        {
            get
            {
                return yRightAxis;
            }
            set
            {
                yRightAxis = value;

                if (yRightAxis != null)
                    yRightAxis.Area = this;
            }
        }

        private bool ShouldSerializeYRightAxis()
        {
            return YRightAxis == null || !YRightAxis.IsDefault;
        }

        private IStiXAxis xAxis;
        /// <summary>
        /// Gets or sets settings of XAxis.
        /// </summary>
        [Description("Gets or sets settings of XAxis.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        public IStiXAxis XAxis
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

        private bool ShouldSerializeXAxis()
        {
            return XAxis == null || !XAxis.IsDefault;
        }

        private IStiXAxis xTopAxis;
        /// <summary>
        /// Gets or sets settings of XTopAxis.
        /// </summary>
        [Description("Gets or sets settings of XTopAxis.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        public IStiXAxis XTopAxis
        {
            get
            {
                return xTopAxis;
            }
            set
            {
                xTopAxis = value;

                if (xTopAxis != null)
                    xTopAxis.Area = this;
            }
        }

        private bool ShouldSerializeXTopAxis()
        {
            return XTopAxis == null || !XTopAxis.IsDefault;
        }

        /// <summary>
        /// Gets or sets value which indicate that all values on horizontal axis is reverse.
        /// </summary>
        [Description("Gets or sets value which indicate that all values on horizontal axis is reverse.")]
        [StiSerializable]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        public virtual bool ReverseHor { get; set; }

        /// <summary>
        /// Gets or sets value which indicate that all values on vertical axis is reverse.
        /// </summary>
        [Description("Gets or sets value which indicate that all values on vertical axis is reverse.")]
        [StiSerializable]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        public virtual bool ReverseVert { get; set; }
        #endregion

        #region Methods.Types
        /// <summary>
        /// Returns default for this area series labels type.
        /// </summary>
        /// <returns></returns>
        public override Type GetDefaultSeriesLabelsType()
        {
            return typeof(StiCenterAxisLabels);
        }

        /// <summary>
        /// Returns array of types which contains all series labels types for this area.
        /// </summary>
        /// <returns></returns>
        public override Type[] GetSeriesLabelsTypes()
        {
            return new[]
            {
                typeof(StiNoneLabels),
                typeof(StiInsideBaseAxisLabels),
                typeof(StiInsideEndAxisLabels),
                typeof(StiCenterAxisLabels),
                typeof(StiOutsideBaseAxisLabels),
                typeof(StiOutsideEndAxisLabels),
                typeof(StiOutsideAxisLabels),
                typeof(StiLeftAxisLabels),
                typeof(StiValueAxisLabels),
                typeof(StiRightAxisLabels),
            };
        }
        #endregion

        /// <summary>
		/// Creates new object of StiAxisArea type.
		/// </summary>
		public StiAxisArea()
        {
            InterlacingHor = new StiInterlacingHor();
            InterlacingVert = new StiInterlacingVert();

            GridLinesHor = new StiGridLinesHor();
            GridLinesHorRight = new StiGridLinesHor();
            GridLinesVert = new StiGridLinesVert();

            GridLinesHorRight.Visible = false;

            XAxis = new StiXBottomAxis();
            YAxis = new StiYLeftAxis();
            XTopAxis = new StiXTopAxis();
            YRightAxis = new StiYRightAxis();
        }
    }
}
