﻿#region Copyright (C) 2003-2022 Stimulsoft
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
using Stimulsoft.Report.PropertyGrid;

namespace Stimulsoft.Report.Chart
{
    public class StiOutsideLeftFunnelLabels : 
        StiFunnelSeriesLabels,
        IStiOutsideLeftFunnelLabels
	{
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiOutsideLeftFunnelLabels;

	    public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[] 
            {
                propHelper.SeriesOutsideLeftFunnelLabels()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }
        #endregion

        #region Methods.override
        public override StiSeriesLabels CreateNew()
        {
            return new StiOutsideLeftFunnelLabels();
        }
        #endregion

        public StiOutsideLeftFunnelLabels()
        {
            this.Core = new StiOutsideLeftFunnelLabelsCoreXF(this);
        }
	}
}