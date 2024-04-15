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
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(StiUniversalConverter))]
    public class StiXRadarAxis :
        StiRadarAxis,
        IStiXRadarAxis,
        ICloneable,
        IStiPropertyGridObject
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyJObject("Labels", Labels.SaveToJsonObject(mode));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Labels":
                        this.Labels.LoadFromJsonObject((JObject)property.Value);
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public StiComponentId ComponentId => StiComponentId.StiXRadarAxis;

        [Browsable(false)]
        public string PropName => string.Empty;

        public StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var objHelper = new StiPropertyCollection();
            var propHelper = propertyGrid.PropertiesHelper;

            var list = new[]
            {
                propHelper.XRadarAxis()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }

        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return null;
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            var axis = base.Clone() as IStiXRadarAxis;

            axis.Labels = this.Labels.Clone() as IStiRadarAxisLabels;

            return axis;
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public StiXRadarAxisCoreXF XCore => Core as StiXRadarAxisCoreXF;

        /// <summary>
        /// Gets or sets axis labels settings.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        [StiCategory("Common")]
        public IStiRadarAxisLabels Labels { get; set; } = new StiRadarAxisLabels();
        #endregion

        public StiXRadarAxis()
        {
            this.Core = new StiXRadarAxisCoreXF(this);
        }

        [StiUniversalConstructor("Axis")]
        public StiXRadarAxis(
            IStiRadarAxisLabels labels,
            bool visible,
            bool allowApplyStyle
            ) :
            base(visible, allowApplyStyle)
        {
            this.Core = new StiXRadarAxisCoreXF(this);

            this.Labels = labels;
        }
    }
}
