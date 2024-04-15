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
using System.Drawing;
using System.Drawing.Design;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiOutsidePieLabels : 
        StiCenterPieLabels,
        IStiOutsidePieLabels
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("Angle");

            jObject.AddPropertyBool("ShowValue", ShowValue);
            jObject.AddPropertyFloat("LineLength", LineLength, 20f);
            jObject.AddPropertyColor("LineColor", LineColor, Color.Black);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "ShowValue":
                        this.ShowValue = property.DeserializeBool();
                        break;

                    case "LineLength":
                        this.LineLength = property.DeserializeFloat();
                        break;

                    case "LineColor":
                        this.LineColor = property.DeserializeColor();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiOutsidePieLabels;

	    public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[] 
            {
                propHelper.SeriesOutsidePieLabels()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }
        #endregion

		#region Properties
	    /// <summary>
        /// Gets or sets value which indicates that values from series will be shown in series labels.
        /// </summary>
		[StiSerializable]
        [StiOrder(StiSeriesLabelsPropertyOrder.ShowValue)]
		[DefaultValue(false)]
		[TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that values from series will be shown in series labels.")]
		public virtual bool ShowValue { get; set; }

	    [StiNonSerialized]
		[Browsable(false)]
		public override float Angle
		{
			get
			{
				return base.Angle;
			}
			set
			{
				base.Angle = value;
			}
		}

	    /// <summary>
        /// Gets or sets line length between border of series labels and border of pie series.
        /// </summary>
		[StiSerializable]
        [StiOrder(StiSeriesLabelsPropertyOrder.LineLength)]
		[DefaultValue(20f)]
        [Description("Gets or sets line length between border of series labels and border of pie series.")]
		public virtual float LineLength { get; set; } = 20f;

	    /// <summary>
        /// Gets or sets color of line.
        /// </summary>
        [StiSerializable]
        [StiOrder(StiSeriesLabelsPropertyOrder.LineColor)]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets color of line.")]
        public virtual Color LineColor { get; set; } = Color.Black;
	    #endregion

        #region Methods.override
        public override StiSeriesLabels CreateNew()
        {
            return new StiOutsidePieLabels();
        }
        #endregion

        public StiOutsidePieLabels()
        {
            this.Core = new StiOutsidePieLabelsCoreXF(this);
        }
  	}
}
