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
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using System.Drawing;
using System.Drawing.Design;
using Stimulsoft.Base.Drawing.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiOutsideAxisLabels : 
        StiAxisSeriesLabels,
        IStiOutsideAxisLabels
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyFloat("LineLength", LineLength, 15f);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "LineLength":
                        this.LineLength = property.DeserializeFloat();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiOutsideAxisLabels;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[] 
            {
                propHelper.SeriesOutsideAxisLabels()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets length of line between border of series labels and border of series element.
        /// </summary>
        [StiSerializable]
        [StiOrder(StiSeriesLabelsPropertyOrder.LineLength)]
        [DefaultValue(15f)]
        [Description("Gets or sets length of line between border of series labels and border of series element.")]
        public float LineLength { get; set; } = 15f;

        /// <summary>
        /// Gets or sets color of line.
        /// </summary>
        [StiSerializable]
        [StiOrder(StiSeriesLabelsPropertyOrder.LineColor)]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets color of line.")]
        public virtual Color LineColor { get; set; } = Color.Black;
        #endregion

        #region Methods.override
        public override StiSeriesLabels CreateNew()
        {
            return new StiOutsideAxisLabels();
        }
        #endregion

        public StiOutsideAxisLabels()
        {
            this.Core = new StiOutsideAxisLabelsCoreXF(this);
        }
    }
}