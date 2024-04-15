#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
using Stimulsoft.Report.Components;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.Dashboard.Helpers
{
    public class StiCrossLinkedFilterHelper
    {
        public static bool IsCrossLinkedFilter(IStiFilterElement filterElement)
        {
            if (filterElement == null)
                return false;

            var elements = (filterElement as StiComponent)?.Page.GetComponents().ToList()
                .Where(e => e is IStiFilterElement).ToList();

            if (elements == null)
                return false;
                    
            var chain = new List<IStiFilterElement>();
            while (true)
            {
                var parentKey = filterElement.GetParentKey();
                if (string.IsNullOrWhiteSpace(parentKey))
                    return false;

                filterElement = elements.Cast<IStiAppCell>()
                    .FirstOrDefault(e => e != null && e.GetKey() == parentKey) as IStiFilterElement;

                if (filterElement == null)
                    return false;

                if (chain.Contains(filterElement))
                    return true;

                chain.Add(filterElement);
            }
        }
    }
}