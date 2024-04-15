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
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(StiUniversalConverter))]
    public abstract class StiAxis3D : IStiAxis3D
    {
        #region IStiJsonReportObject.override
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            jObject.AddPropertyBool(nameof(AllowApplyStyle), AllowApplyStyle, true);
            jObject.AddPropertyJObject(nameof(Labels), Labels.SaveToJsonObject(mode));
            jObject.AddPropertyBool(nameof(Visible), Visible, true);
            jObject.AddPropertyColor(nameof(LineColor), LineColor, Color.Gray);

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(AllowApplyStyle):
                        this.AllowApplyStyle = property.DeserializeBool();
                        break;

                    case nameof(Visible):
                        this.Visible = property.DeserializeBool();
                        break;

                    case nameof(Labels):
                        this.Labels.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(LineColor):
                        this.LineColor = property.DeserializeColor();
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
        public object Clone()
        {
            var axis = this.MemberwiseClone() as IStiAxis3D;
            
            if (this.Core != null)
            {
                axis.Core = this.Core.Clone() as StiAxisCoreXF3D;
                axis.Core.Axis = axis;
            }

            return axis;
        }
        #endregion

        #region Properties
        [StiSerializable(StiSerializationVisibility.Reference)]
        [Browsable(false)]
        public IStiAxisArea3D Area { get; set; }

        [Browsable(false)]
        public StiAxisInfoXF3D Info { get; set; } = new StiAxisInfoXF3D();
        
        [Browsable(false)]
        public StiAxisCoreXF3D Core { get; set; }

        private bool allowApplyStyle = true;
        /// <summary>
        /// Gets or sets value which indicates that chart style will be used.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Description("Gets or sets value which indicates that chart style will be used.")]
        [DefaultValue(true)]
        public bool AllowApplyStyle
        {
            get
            {
                return allowApplyStyle;
            }
            set
            {
                if (allowApplyStyle != value)
                {
                    allowApplyStyle = value;

                    //if (value && this.Area != null && this.Area.Chart != null)
                    //    this.Core.ApplyStyle(this.Area.Chart.Style);
                }
            }
        }

        /// <summary>
        /// Gets or sets axis labels settings.
        /// </summary>
		[StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        public IStiAxisLabels3D Labels { get; set; } = new StiAxisLabels3D();

        private bool ShouldSerializeLabels()
        {
            return Labels == null || !Labels.IsDefault;
        }

        /// <summary>
        /// Gets or sets line color which used to draw axis.
        /// </summary>
		[StiSerializable]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color LineColor { get; set; } = Color.Gray;

        private bool ShouldSerializeLineColor()
        {
            return LineColor != Color.Gray;
        }

        /// <summary>
        /// Gets or sets visibility of axis.
        /// </summary>
		[StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Description("Gets or sets visibility of axis.")]
        public virtual bool Visible { get; set; } = true;

        protected virtual bool ShouldSerializeVisible()
        {
            return !Visible;
        }
        #endregion

        public StiAxis3D()
        {
        }

        public StiAxis3D(
            IStiAxisLabels3D labels,
            Color lineColor,
            bool visible,
            bool allowApplyStyle)
        {
            this.Labels = labels;
            this.LineColor = lineColor;
            this.Visible = visible;
            this.allowApplyStyle = allowApplyStyle;
        }
    }
}
