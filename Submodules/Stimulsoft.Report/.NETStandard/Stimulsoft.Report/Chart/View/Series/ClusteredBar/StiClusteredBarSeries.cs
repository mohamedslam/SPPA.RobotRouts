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
using System.ComponentModel;
using System.Drawing.Design;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.PropertyGrid;

#if NETSTANDARD
using UITypeEditor = Stimulsoft.System.Drawing.Design.UITypeEditor;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiClusteredBarSeries : 
        StiClusteredColumnSeries,
        IStiClusteredBarSeries
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiClusteredBarSeries;
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
                propHelper.BorderColor(),
                propHelper.Brush(),
                propHelper.BrushNegative(),
                propHelper.ShowShadow()
            };
            objHelper.Add(StiPropertyCategories.Appearance, list);

            // Behavior
            list = new[] 
            {
                propHelper.AllowApplyStyle(),
                propHelper.AllowApplyBrushNegative(),
                propHelper.ShowInLegend(),
                propHelper.ShowSeriesLabels(),
                propHelper.ShowZeros(),
                propHelper.Title(),
                propHelper.YAxis(),
                propHelper.fWidth()
            };
            objHelper.Add(StiPropertyCategories.Behavior, list);

            return objHelper;
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public override StiSeriesYAxis YAxis { get; set; }

        /// <summary>
        /// Gets or sets Y Axis for series on which will output string representation of arguments.
        /// </summary>
        /// 
        [StiSerializable]
        [DefaultValue(StiSeriesXAxis.BottomXAxis)]
        [StiCategory("Common")]
        [Description("Gets or sets X Axis for series on which will output string representation of arguments.")]
        [StiPropertyLevel(StiLevel.Standard)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        public StiSeriesXAxis XAxis
        {
            get
            {
                if (this.YAxis == StiSeriesYAxis.LeftYAxis)
                    return StiSeriesXAxis.BottomXAxis;

                return StiSeriesXAxis.TopXAxis;
            }
            set
            {
                this.YAxis = value == StiSeriesXAxis.BottomXAxis ? StiSeriesYAxis.LeftYAxis : StiSeriesYAxis.RightYAxis;
            }
        }
        #endregion

        #region Methods.Types
        public override Type GetDefaultAreaType()
        {
            return typeof(StiClusteredBarArea);
        }
        #endregion

        #region Methods.override
        public override StiSeries CreateNew()
        {
            return new StiClusteredBarSeries();
        }
        #endregion

        public StiClusteredBarSeries()
        {
            this.Core = new StiClusteredBarSeriesCoreXF(this);
        }
	}
}