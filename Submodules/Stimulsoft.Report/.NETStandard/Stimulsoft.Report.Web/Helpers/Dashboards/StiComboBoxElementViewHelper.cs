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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiComboBoxElementViewHelper
    {
        #region Methods
        public static List<Hashtable> GetElementItems(IStiComboBoxElement comboBoxElement)
        {
            var dataTable = StiElementDataCache.GetOrCreate(comboBoxElement);
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

                if (comboBoxElement.ShowBlanks || !StiDataFiltersHelper.IsBlankData(nameData))
                    items.Add(ComboBoxItem(StiDashboardElementViewHelper.Format(comboBoxElement, row[nameIndex]), row[keyIndex]));
            }

            return items;
        }

        public static Hashtable ComboBoxItem(string label, object value)
        {
            var item = new Hashtable();
            item["label"] = label;
            item["value"] = StiDataFiltersHelper.ToFilterString(value);

            return item;
        }
        #endregion

        #region Methods.Helpers

        public static Hashtable GetSettings(IStiComboBoxElement comboBoxElement)
        {
            var settings = StiDashboardElementViewHelper.GetControlElementSettings(comboBoxElement);
            settings["itemHeight"] = StiElementConsts.ComboBox.ItemHeight;

            return settings;
        }

        protected static int GetNameMeterIndex(StiDataTable table)
        {
            if (table == null) return -1;

            var meter = table.Meters.FirstOrDefault(f => f.GetType().Name == "StiNameComboBoxMeter");
            return meter != null ? table.Meters.IndexOf(meter) : -1;
        }

        protected static int GetKeyMeterIndex(StiDataTable table)
        {
            if (table == null) return -1;

            var meter = table.Meters.FirstOrDefault(f => f.GetType().Name == "StiKeyComboBoxMeter");
            return meter != null ? table.Meters.IndexOf(meter) : -1;
        }

        public static string GetColumnPath(IStiComboBoxElement comboBoxElement)
        {
            if (comboBoxElement.GetKeyMeter() != null)
                return comboBoxElement.GetKeyMeter()?.Expression;

            if (comboBoxElement.GetNameMeter() != null)
                return comboBoxElement.GetNameMeter()?.Expression;

            return null;
        }
        #endregion
    }
}
