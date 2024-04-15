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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiNoneLabels : 
        StiSeriesLabels,
        IStiNoneLabels
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.RemoveProperty("Antialiasing");
            jObject.RemoveProperty("Visible");
            jObject.RemoveProperty("LabelColor");
            jObject.RemoveProperty("BorderColor");
            jObject.RemoveProperty("Brush");
            jObject.RemoveProperty("Font");

            return jObject;
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiNoneLabels;

	    public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[] 
            {
                propHelper.SeriesNoneLabels()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }
        #endregion

		#region Properties
		[StiNonSerialized]
		[Browsable(false)]
		public override bool Antialiasing
		{
			get
			{
				return base.Antialiasing;
			}
			set
			{
				base.Antialiasing = value;
			}
		}

		[StiNonSerialized]
		[Browsable(false)]
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
		
		[StiNonSerialized]
		[Browsable(false)]
		public override Color LabelColor
		{
			get
			{
				return base.LabelColor;
			}
			set
			{
				base.LabelColor = value;
			}
		}

		[StiNonSerialized]
		[Browsable(false)]
		public override Color BorderColor
		{
			get
			{
				return base.BorderColor;
			}
			set
			{
				base.BorderColor = value;
			}
		}

		[StiNonSerialized]
		[Browsable(false)]
		public override StiBrush Brush
		{
			get 
			{
				return base.Brush;
			}
			set 
			{
				base.Brush = value;
			}
		}

		[StiNonSerialized]
		[Browsable(false)]
		public override Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
				base.Font = value;
			}
		}
        #endregion

        #region Methods.override
        public override StiSeriesLabels CreateNew()
        {
            return new StiNoneLabels();
        }
        #endregion

        public StiNoneLabels()
        {
            this.Core = new StiNoneLabelsCoreXF(this);
        }
	}
}