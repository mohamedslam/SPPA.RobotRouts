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

using System.Collections;
using Stimulsoft.Base;

namespace Stimulsoft.Report.Web
{
    internal class StiCodePageHelper
    {
        public static ArrayList GetDBaseCodePageItems()
        {
            ArrayList items = new ArrayList();
            for (int i = 0; i < StiDBaseHelper.CodePageNames.Length; i++) {
                var item = new Hashtable();
                item["key"] = StiDBaseHelper.CodePageCodes[i, 1];
                item["name"] = StiDBaseHelper.CodePageNames[i];
                items.Add(item);
            }
            
            return items;
        }

        public static ArrayList GetCsvCodePageItems()
        {
            ArrayList items = new ArrayList();
            for (int i = 0; i < StiCsvHelper.CodePageNames.Length; i++)
            {
                var item = new Hashtable();
                item["key"] = StiCsvHelper.CodePageCodes[i];
                item["name"] = StiCsvHelper.CodePageNames[i];
                items.Add(item);
            }

            return items;
        }
    }
}