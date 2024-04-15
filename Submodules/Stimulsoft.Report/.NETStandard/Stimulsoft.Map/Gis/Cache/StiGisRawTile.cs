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

using Stimulsoft.Base.Gis;
using Stimulsoft.Map.Gis.Core;

namespace Stimulsoft.Map.Gis.Cache
{
    internal struct StiGisRawTile
    {
        public StiGisRawTile(StiGeoMapProviderType type, StiGisPoint pos, int zoom)
        {
            this.Type = 1400000 + (int)type;
            this.Pos = pos;
            this.Zoom = zoom;
        }

        #region Fields
        public int Type;
        public StiGisPoint Pos;
        public int Zoom;
        #endregion

        #region Methods.override
        public override string ToString() => $"{Type}, zoom={Zoom}, pos={Pos}";
        #endregion
    }
}