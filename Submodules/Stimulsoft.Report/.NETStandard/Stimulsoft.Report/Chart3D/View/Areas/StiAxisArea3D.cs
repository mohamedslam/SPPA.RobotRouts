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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Serializing;
using System;
using System.ComponentModel;

namespace Stimulsoft.Report.Chart
{
    public abstract class StiAxisArea3D :
        StiArea,
        IStiAxisArea3D
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyJObject(nameof(InterlacingHor), interlacingHor.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(InterlacingVert), interlacingVert.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(GridLinesHor), gridLinesHor.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(GridLinesVert), gridLinesVert.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(YAxis), YAxis.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(XAxis), XAxis.SaveToJsonObject(mode));

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
                        this.YAxis.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(XAxis):
                        this.XAxis.LoadFromJsonObject((JObject)property.Value);
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
            var area = base.Clone() as IStiAxisArea3D;

            area.InterlacingHor = this.interlacingHor.Clone() as IStiInterlacingHor;
            area.InterlacingVert = this.interlacingVert.Clone() as IStiInterlacingVert;
            area.GridLinesHor = this.gridLinesHor.Clone() as IStiGridLinesHor;
            area.GridLinesVert = this.gridLinesVert.Clone() as IStiGridLinesVert;
            area.XAxis = this.xAxis.Clone() as IStiXAxis3D;
            area.YAxis = this.yAxis.Clone() as IStiYAxis3D;
            area.ZAxis = this.zAxis.Clone() as IStiAxis3D;

            return area;
        }
        #endregion

        #region Properties
        private IStiXAxis3D xAxis;
        /// <summary>
        /// Gets or sets settings of XAxis.
        /// </summary>
        [Description("Gets or sets settings of XAxis.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        public IStiXAxis3D XAxis
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

        private IStiYAxis3D yAxis;
        /// <summary>
        /// Gets or sets settings of YAxis.
        /// </summary>
        [Description("Gets or sets settings of YAxis.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        public IStiYAxis3D YAxis
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

        private IStiAxis3D zAxis;
        [Browsable(false)]
        public IStiAxis3D ZAxis
        {
            get
            {
                return zAxis;
            }
            set
            {
                zAxis = value;

                if (zAxis != null)
                    zAxis.Area = this;
            }
        }

        private double rotationX = 0;
        [Browsable(false)]
        public double RotationX
        {
            get
            {
                return rotationX;
            }
            set
            {
                if (value < -60)
                    rotationX = -60;

                else if (value > 0)
                    rotationX = 0;

                else
                    rotationX = value;
            }
        }

        private double rotationY = 20;
        [Browsable(false)]
        public double RotationY
        {
            get
            {
                return rotationY;
            }
            set
            {
                if (value < 10)
                    rotationY = 10;

                else if (value > 60)
                    rotationY = 60;

                else
                    rotationY = value;
            }
        }

        [Browsable(false)]
        public double Scale { get; set; } = 20.0;

        [Browsable(false)]
        public double CameraX { get; set; } = 3;

        [Browsable(false)]
        public double CameraY { get; set; } = 3;

        [Browsable(false)]
        public double CameraZ { get; set; } = -80;

        [Browsable(false)]
        public double NearPlane { get; set; } = 0.1;

        [Browsable(false)]
        public StiAxisAreaCoreXF3D AxisCore => Core as StiAxisAreaCoreXF3D;

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
        /// Gets or sets horizontal grid lines.
        /// </summary>
        [Description("Gets or sets horizontal grid lines.")]
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
        #endregion

        #region Methods.Types
        /// <summary>
        /// Returns default for this area series labels type.
        /// </summary>
        /// <returns></returns>
        public override Type GetDefaultSeriesLabelsType()
        {
            return typeof(StiCenterAxisLabels3D);
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
                typeof(StiCenterAxisLabels3D),
                typeof(StiOutsideAxisLabels3D)
            };
        }
        #endregion

        public StiAxisArea3D()
        {
            this.XAxis = new StiXAxis3D();
            this.YAxis = new StiYAxis3D();
            this.ZAxis = new StiZAxis3D();

            GridLinesHor = new StiGridLinesHor();
            GridLinesVert = new StiGridLinesVert();

            InterlacingHor = new StiInterlacingHor();
            InterlacingVert = new StiInterlacingVert();
        }
    }
}
