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
using System.ComponentModel;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(Stimulsoft.Report.Chart.Design.StiLineMarkerConverter))]
    public class StiLineMarker : 
        StiMarker,
        IStiLineMarker
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyDouble("Step", Step, 15d);
            jObject.AddPropertyFloat("Size", Size, 5f);
            jObject.AddPropertyBool("Visible", Visible);
            jObject.AddPropertyColor("BorderColor", BorderColor, Color.Transparent);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Step":
                        this.Step = property.DeserializeDouble();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiLineMarker;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var objHelper = new StiPropertyCollection();
            var propHelper = propertyGrid.PropertiesHelper;

            var list = new[] 
            { 
                propHelper.LineMarker()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }
        #endregion

        #region IStiDefault
        public override bool IsDefault
        {
            get
            {
                return
                    base.IsDefault
                    && Step == 15d;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets step of the line marker.
        /// </summary>
        [DefaultValue(15d)]
        [StiSerializable]
        [Description("Gets or sets step of the line marker.")]
        public virtual double Step { get; set; } = 15d;

        /// <summary>
        /// Gets or sets size of marker.
        /// </summary>
        [DefaultValue(5f)]
        [StiSerializable]
        [Description("Gets or sets size of marker.")]
        public override float Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
            }
        }

        protected override bool ShouldSerializeSize()
        {
            return Size != 5f;
        }

        /// <summary>
        /// Gets or sets visibility of marker.
        /// </summary>
        [DefaultValue(false)]
        [Description("Gets or sets visibility of marker.")]
        public override bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                base.Visible = value;
            }
        }

        protected override bool ShouldSerializeVisible()
        {
            return Visible;
        }
        #endregion

        public StiLineMarker()
        {
            this.Visible = false;
            this.BorderColor = Color.Transparent;
            this.Size = 5f;
        }
    }
}
