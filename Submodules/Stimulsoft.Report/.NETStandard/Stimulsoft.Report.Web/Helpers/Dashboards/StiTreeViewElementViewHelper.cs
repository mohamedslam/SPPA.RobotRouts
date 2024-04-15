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
using Stimulsoft.Base.Meters;
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
    internal class StiTreeViewElementViewHelper
    {
        #region Methods
        public static List<Hashtable> GetElementItems(IStiTreeViewElement treeViewElement)
        {
            var dataTable = StiElementDataCache.GetOrCreate(treeViewElement);
            if (dataTable == null) return null;
            if (dataTable == null || dataTable.Rows.Count == 0) return null;

            var parentItem = TreeViewItem(treeViewElement);

            foreach (var row in dataTable.Rows)
            {
                var dataIndex = 0;
                var currentItem = parentItem;
                foreach (var data in row)
                {
                    var isDataEmpty = data == null || data == DBNull.Value;
                    var isStrEmpty = data is string && string.IsNullOrEmpty(data as string);
                    var isEmpty = isDataEmpty || isStrEmpty;

                    if (treeViewElement.ShowBlanks || !isEmpty)
                    {
                        var items = currentItem["items"] as List<Hashtable>;

                        var selectedItem = items?.FirstOrDefault(n => object.Equals(n["key"], StiDataFiltersHelper.ToFilterString(data)));
                        if (selectedItem == null)
                        {
                            selectedItem = TreeViewItem(treeViewElement, data, dataTable.Meters[dataIndex]);

                            if (currentItem["items"] == null)
                                currentItem["items"] = new List<Hashtable>();

                            (currentItem["items"] as List<Hashtable>).Add(selectedItem);
                        }

                        currentItem = selectedItem;
                    }

                    dataIndex++;
                }
            }

            return parentItem["items"] as List<Hashtable>;
        }

        public static Hashtable TreeViewItem(IStiTreeViewElement treeViewElement, object key = null, IStiMeter meter = null)
        {
            var item = new Hashtable();
            item["key"] = StiDataFiltersHelper.ToFilterString(key);
            item["text"] = StiDashboardElementViewHelper.Format(treeViewElement, key);
            item["columnPath"] = meter != null ? meter.Expression : null;
            item["meterKey"] = meter != null ? meter.Key : null;
            item["items"] = null;

            return item;
        }
        #endregion

        #region Methods.Helpers
        public static Hashtable GetSettings(IStiTreeViewElement treeViewElement)
        {
            var settings = StiDashboardElementViewHelper.GetControlElementSettings(treeViewElement);
            settings["itemHeight"] = StiElementConsts.TreeView.ItemHeight;

            return settings;
        }

        public static string GetColumnPath(IStiTreeViewElement treeViewElement)
        {
            return treeViewElement.FetchAllMeters()?.LastOrDefault()?.Expression;
        }

        public static string GetMeterKey(IStiTreeViewElement treeViewElement)
        {
            return treeViewElement.FetchAllMeters()?.LastOrDefault()?.Key;
        }
        #endregion
    }
}
