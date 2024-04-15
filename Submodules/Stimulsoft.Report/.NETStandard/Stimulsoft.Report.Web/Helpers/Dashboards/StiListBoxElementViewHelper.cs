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

using Stimulsoft.Base.Dashboard;
using Stimulsoft.Data.Engine;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Helpers;
using Stimulsoft.Report.Dashboard.Styles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiListBoxElementViewHelper
    {
        #region Methods
        public static List<Hashtable> GetElementItems(IStiListBoxElement listBoxElement)
        {
            var dataTable = StiElementDataCache.GetOrCreate(listBoxElement);
            if (dataTable == null) return null;

            if (dataTable == null || dataTable.Rows.Count == 0) return null;

            var nameIndex = GetNameMeterIndex(dataTable);
            var keyIndex = GetKeyMeterIndex(dataTable);
            if (nameIndex == -1 && keyIndex == 1) return null;

            if (keyIndex == -1)
                keyIndex = nameIndex;

            if (nameIndex == -1)
                nameIndex = keyIndex;

            var items = new List<Hashtable>();

            foreach (var row in dataTable.Rows)
            {
                var keyData = row[keyIndex];
                if (keyData == null) continue;

                var nameData = row[nameIndex];

                if (listBoxElement.ShowBlanks || !StiDataFiltersHelper.IsBlankData(row[nameIndex]))
                    items.Add(ListBoxItem(StiDashboardElementViewHelper.Format(listBoxElement, row[nameIndex]), row[keyIndex]));
            }

            return items;
        }

        public static Hashtable ListBoxItem(string label, object value)
        {
            var item = new Hashtable();
            item["label"] = label;
            item["value"] = StiDataFiltersHelper.ToFilterString(value);

            return item;
        }
        #endregion

        #region Methods.Helpers
        public static Hashtable GetSettings(IStiListBoxElement listBoxElement)
        {
            var settings = StiDashboardElementViewHelper.GetControlElementSettings(listBoxElement);
            settings["itemHeight"] = StiElementConsts.ListBox.ItemHeight;

            return settings;
        }

        protected static int GetNameMeterIndex(StiDataTable table)
        {
            if (table == null) return -1;

            var meter = table.Meters.FirstOrDefault(f => f.GetType().Name == "StiNameListBoxMeter");
            return meter != null ? table.Meters.IndexOf(meter) : -1;
        }

        protected static int GetKeyMeterIndex(StiDataTable table)
        {
            if (table == null) return -1;

            var meter = table.Meters.FirstOrDefault(f => f.GetType().Name == "StiKeyListBoxMeter");
            return meter != null ? table.Meters.IndexOf(meter) : -1;
        }

        public static string GetColumnPath(IStiListBoxElement listBoxElement)
        {
            if (listBoxElement.GetKeyMeter() != null)
                return listBoxElement.GetKeyMeter()?.Expression;

            if (listBoxElement.GetNameMeter() != null)
                return listBoxElement.GetNameMeter()?.Expression;

            return null;
        }
        #endregion
    }
}
