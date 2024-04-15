#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

namespace Stimulsoft.Report.Chart
{
    public class StiLineSeries3D :
        StiBaseLineSeries3D,
        IStiLineSeries3D
    {
        public float Width { get; internal set; } = 0.8f;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            throw new NotImplementedException();
        }

        #region Methods.Types
        public override Type GetDefaultAreaType()
        {
            return typeof(StiClusteredColumnArea3D);
        }
        #endregion

        #region Methods.override
        public override StiSeries CreateNew()
        {
            return new StiLineSeries3D();
        }
        #endregion

        public StiLineSeries3D()
        {
            Core = new StiLineSeriesCoreXF3D(this);
        }
    }
}
