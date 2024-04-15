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
using Stimulsoft.Report.PropertyGrid;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiXAxis3D :
        StiAxis3D,
        IStiXAxis3D,
        IStiPropertyGridObject
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyBool(nameof(ShowEdgeValues), ShowEdgeValues);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(ShowEdgeValues):
                        this.ShowEdgeValues = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public virtual StiComponentId ComponentId => StiComponentId.StiXAxis3D;

        [Browsable(false)]
        public string PropName => string.Empty;

        public virtual StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var objHelper = new StiPropertyCollection();

            //objHelper.Add(StiPropertyCategories.Main, new[]
            //{
            //    propertyGrid.PropertiesHelper.XAxis()
            //});

            return objHelper;
        }

        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return null;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets value which indicates that first and last arguments on axis will be shown anyway.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that first and last arguments on axis will be shown anyway.")]
        public bool ShowEdgeValues { get; set; }
        #endregion

        public StiXAxis3D()
        {
            this.Core = new StiXAxisCoreXF3D(this);
        }

        [StiUniversalConstructor("XAxis3D")]
        public StiXAxis3D(
            bool showEdgeValues,
            IStiAxisLabels3D labels,
            Color lineColor,
            bool visible,
            bool allowApplyStyle) : base(labels, lineColor, visible, allowApplyStyle)
        {
            this.ShowEdgeValues = showEdgeValues;
            this.Core = new StiXAxisCoreXF3D(this);
        }
    }
}
