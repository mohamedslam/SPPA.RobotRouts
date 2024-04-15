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
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;

namespace Stimulsoft.Report.Chart
{
    public class StiHistogramSeries :
        StiClusteredColumnSeries,
        IStiHistogramSeries
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiHistogramSeries;
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

            return objHelper;
        }
        #endregion

        #region Methods.Types
        public override Type GetDefaultAreaType()
        {
            return typeof(StiHistogramArea);
        }
        #endregion

        #region Methods.override
        public override StiSeries CreateNew()
        {
            return new StiHistogramSeries();
        }
        #endregion

        #region Properties
        private float width = 0.98f;
        [StiSerializable]
        [StiCategory("Common")]
        [DefaultValue(0.98f)]
        [Description("Gets or sets the width factor of one column series. Value 1 is equal to 100%.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public override float Width
        {
            get
            {
                return width;
            }
            set
            {
                if (value >= 0.01f && value <= 1f) width = value;
            }
        }
        #endregion

        public StiHistogramSeries()
        {
            this.Core = new StiHistogramSeriesCoreXF(this);
        }
    }
}
