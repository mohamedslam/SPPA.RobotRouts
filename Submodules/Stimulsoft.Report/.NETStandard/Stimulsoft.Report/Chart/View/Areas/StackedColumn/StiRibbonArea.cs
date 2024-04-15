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
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;

namespace Stimulsoft.Report.Chart
{
    public class StiRibbonArea :
        StiAxisArea,
        IStiRibbonArea
    {
        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiRibbonArea;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var objHelper = new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Main,
                    new[]
                    {
                        propertyGrid.PropertiesHelper.RibbonColumnArea(),
                    }
                }
            };

            return objHelper;
        }
        #endregion

        #region Methods.Types
        public override Type GetDefaultSeriesType() => typeof(StiRibbonSeries);

        public override Type[] GetSeriesTypes()
        {
            return new[]
            {
                typeof(StiRibbonSeries)
            };
        }
        #endregion

        #region Methods.override
        public override StiArea CreateNew() => new StiRibbonArea();
        #endregion

        [StiUniversalConstructor("Area")]
        public StiRibbonArea()
        {
            this.Core = new StiStackedColumnAreaCoreXF(this);
        }
    }
}
