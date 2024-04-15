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

using System;
using System.Drawing;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using System.ComponentModel;
using Stimulsoft.Report.PropertyGrid;

namespace Stimulsoft.Report.Chart
{
    public class StiScatterLineSeries : 
        StiScatterSeries,
        IStiScatterLineSeries
    {
        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiScatterLineSeries;
            }
        }

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // Value
            var list = new[] 
            {
                propHelper.ValueDataColumn(),
                propHelper.Value(),
                propHelper.ListOfValues()
            };
            objHelper.Add(StiPropertyCategories.Value, list);

            // Argument
            list = new[] 
            {
                propHelper.ArgumentDataColumn(),
                propHelper.Argument(),
                propHelper.ListOfArguments()
            };
            objHelper.Add(StiPropertyCategories.Argument, list);

            // Data
            list = new[] 
            { 
                propHelper.Format(),
                propHelper.SortBy(), 
                propHelper.SortDirection(),
                propHelper.AutoSeriesKeyDataColumn(),
                propHelper.AutoSeriesColorDataColumn(),
                propHelper.AutoSeriesTitleDataColumn()
            };
            objHelper.Add(StiPropertyCategories.Data, list);

            // Appearance
            list = new[] 
            {
                propHelper.LabelsOffset(),
                propHelper.Lighting(),
                propHelper.LineColor(),
                propHelper.LineStyle(),
                propHelper.LineWidth(),
                propHelper.ShowShadow()
            };
            objHelper.Add(StiPropertyCategories.Appearance, list);

            // Behavior
            list = new[] 
            {
                propHelper.AllowApplyStyle(),
                propHelper.ShowInLegend(),
                propHelper.ShowNulls(),
                propHelper.ShowSeriesLabels(),
                propHelper.Title(),
                propHelper.YAxis()
            };
            objHelper.Add(StiPropertyCategories.Behavior, list);

            return objHelper;
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            IStiScatterLineSeries series = base.Clone() as IStiScatterLineSeries;

            return series;
        }
        #endregion

        #region Methods.Types
        public override Type GetDefaultAreaType()
        {
            return typeof(StiScatterArea);
        }
        #endregion

        #region Properties
        [Browsable(true)]
        public override Color LineColor
        {
            get
            {
                return base.LineColor;
            }
            set
            {
                base.LineColor = value;
            }
        }


        [Browsable(true)]
        public override StiPenStyle LineStyle
        {
            get
            {
                return base.LineStyle;
            }
            set
            {
                base.LineStyle = value;
            }
        }


        [Browsable(true)]
        public override bool Lighting
        {
            get
            {
                return base.Lighting;
            }
            set
            {
                base.Lighting = value;
            }
        }


        [Browsable(true)]
        public override float LineWidth
        {
            get
            {
                return base.LineWidth;
            }
            set
            {
                base.LineWidth = value;
            }
        }

        #endregion        
        
        #region Methods.override
        public override StiSeries CreateNew()
        {
            return new StiScatterLineSeries();
        }
        #endregion

        public StiScatterLineSeries()
        {
            this.Core = new StiScatterLineSeriesCoreXF(this);
        }
    }
}