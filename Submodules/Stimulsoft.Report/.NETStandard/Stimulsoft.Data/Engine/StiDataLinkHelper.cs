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
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Stimulsoft.Data.Engine
{
    public static class StiDataLinkHelper
    {
        #region Methods
        public static List<StiDataLink> GetLinks(IStiAppDictionary dictionary)
        {
            if (dictionary == null)
                return null;

            return dictionary.FetchDataRelations()
                .Select(r => new StiDataLink(
                    r.GetParentDataSource()?.GetName(),
                    r.GetChildDataSource()?.GetName(),
                    r.FetchParentColumns()?.ToArray(),
                    r.FetchChildColumns()?.ToArray(),
                    r.GetActiveState(),
                    r.GetKey())).ToList();
        }
        #endregion
    }
}