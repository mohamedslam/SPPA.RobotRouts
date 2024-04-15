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
using System;


namespace Stimulsoft.Report.Chart
{
    public class StiStackedSplineAreaArea :
        StiStackedColumnArea,
        IStiStackedSplineAreaArea
    {
        #region Methods.Types
        public override Type GetDefaultSeriesType()
        {
            return typeof(StiStackedSplineAreaSeries);
        }
        #endregion

        #region Methods.override
        public override StiComponentId ComponentId => StiComponentId.StiStackedSplineAreaArea;

        public override StiArea CreateNew()
        {
            return new StiStackedSplineAreaArea();
        }
        #endregion

        [StiUniversalConstructor("Area")]
        public StiStackedSplineAreaArea()
        {
            this.Core = new StiStackedSplineAreaAreaCoreXF(this);
        }
    }
}
