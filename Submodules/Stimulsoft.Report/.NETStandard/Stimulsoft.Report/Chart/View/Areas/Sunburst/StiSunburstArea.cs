#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;

namespace Stimulsoft.Report.Chart
{
    public class StiSunburstArea :
        StiArea, 
        IStiSunburstArea
    {
        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiSunburstArea;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            return new StiPropertyCollection();
        }
        #endregion

        #region Methods.Types
        public override Type GetDefaultSeriesLabelsType()
        {
            return typeof(StiCenterPieLabels);
        }

        public override Type[] GetSeriesLabelsTypes()
        {
            return new[]
            {
                typeof(StiNoneLabels),
                typeof(StiCenterPieLabels)
            };
        }

        public override Type GetDefaultSeriesType()
        {
            return typeof(StiSunburstSeries);
        }

        public override Type[] GetSeriesTypes()
        {
            return new[]
            {
                 typeof(StiSunburstSeries)
            };
        }
        #endregion

        #region Methods.override
        public override StiArea CreateNew()
        {
            return new StiSunburstArea();
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public override bool ColorEach => false;
        #endregion

        [StiUniversalConstructor("Area")]
        public StiSunburstArea()
        {
            this.Core = new StiSunburstAreaCoreXF(this);
        }
    }
}
