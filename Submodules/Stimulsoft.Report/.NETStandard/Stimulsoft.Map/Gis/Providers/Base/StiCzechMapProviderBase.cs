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

namespace Stimulsoft.Map.Gis.Providers
{
    public abstract class StiCzechMapProviderBase : 
        StiGisMapProvider
    {
        public StiCzechMapProviderBase()
        {
            RefererUrl = "http://www.mapy.cz/";
            Area = new StiGisRectLatLng(51.2024819920053, 11.8401353319027, 7.22833716731277, 2.78312271922872);
        }

        #region Properties.override
        public override int MaxZoom => 18;

        internal override StiGisProjection Projection => StiMercatorGisProjection.Instance;
        #endregion
    }
}