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

using System.Collections.Generic;

namespace Stimulsoft.Base.Map
{
    public interface IStiMapKeyHelper
    {
        List<string> GetMapIdents(string key);

        string GetIsoAlpha2FromName(string country, string mapId, string lang, IStiReport report = null);

        string GetIsoAlpha3FromName(string country, string mapId, string lang, IStiReport report = null);

        string GetNameFromIsoAlpha2(string alpha3, string mapId, string lang, IStiReport report = null);

        string GetNameFromIsoAlpha3(string alpha3, string mapId, string lang, IStiReport report = null);

        string NormalizeName(string name, string mapId, string lang, IStiReport report = null);
    }
}