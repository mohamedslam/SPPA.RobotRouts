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

using Stimulsoft.Map.Gis.Core;
using Stimulsoft.Map.Gis.Projections;
using System;

namespace Stimulsoft.Map.Gis.Providers
{
    public abstract class StiArcGISMapPlateCarreeProviderBase : 
        StiGisMapProvider
    {
        public StiArcGISMapPlateCarreeProviderBase()
        {
            Copyright = $"©{DateTime.Today.Year} ESRI - Map data ©{DateTime.Today.Year} ArcGIS";
        }

        #region GMapProvider Members
        public override int MaxZoom => 15;

        internal override StiGisProjection Projection => StiPlateCarreeGisProjection.Instance;
        #endregion
    }
}