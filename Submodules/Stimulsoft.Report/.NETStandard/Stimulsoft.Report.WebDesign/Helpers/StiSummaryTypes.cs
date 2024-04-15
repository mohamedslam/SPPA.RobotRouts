#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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

using System;
using System.Collections;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Web
{
    internal class StiSummaryTypes
    {
        private static Hashtable Item(string value, string key) 
        {
            Hashtable item = new Hashtable();
            item["key"] = key;
            item["value"] = value;

            return item;
        }

        public static ArrayList GetItems()
        {
            ArrayList items = new ArrayList();
            int count = Enum.GetValues(typeof(StiGroupSummaryType)).Length;
            for (int i = 0; i < count; i++)
                items.Add(Item(((StiGroupSummaryType)i).ToString(), i.ToString()));

            return items;
        }
    }
}