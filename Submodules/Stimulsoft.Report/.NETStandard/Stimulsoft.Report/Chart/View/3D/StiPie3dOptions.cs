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
    public class StiPie3dOptions : Sti3dOptions
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyFloat("Height", Height, 25);
            jObject.AddPropertyFloat("Distance", Distance, 4);
            jObject.AddPropertyEnum("Lighting", this.Lighting);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {

                    case "Height":
                        this.Height = property.DeserializeFloat();
                        break;

                    case "Distance":
                        this.Distance = property.DeserializeFloat();
                        break;

                    case "Lighting":
                        this.Lighting = property.DeserializeEnum<StiPie3dLightingStyle>();
                        break;
                }
            }
        }
        #endregion

        #region IStiDefault
        [Browsable(false)]
        public override bool IsDefault
        {
            get
            {
                return base.IsDefault
                    && Lighting == StiPie3dLightingStyle.Gradient
                    && Height == 25
                    && Distance == 4;
            }
        }
        #endregion

        #region Properties
        [StiSerializable]
        [DefaultValue(StiPie3dLightingStyle.Gradient)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        public StiPie3dLightingStyle Lighting { get; set; } = StiPie3dLightingStyle.Gradient;

        private float height = 25;
        [StiSerializable]
        [DefaultValue(25f)]
        public float Height
        {
            get
            {
                return this.height;
            }
            set
            {
                if (value >= 0)
                    this.height = value;
            }
        }

        private float distance = 4f;
        [StiSerializable]
        [DefaultValue(4f)]
        [Description("Gets or sets distance between the center of series and the center of each segment.")]
        public virtual float Distance
        {
            get
            {
                return distance;
            }
            set
            {
                if (value >= 0)
                    distance = value;
            }
        }
        #endregion
    }
}
