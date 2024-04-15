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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Data.Engine;
using System.Collections;

namespace Stimulsoft.Report.Dashboard.Helpers
{
    public static class StiPivotTableElementClickHelper
    {
        #region Properties
        private static Hashtable hash { get; set; } = new Hashtable();
        #endregion

        #region Events
        public static event StiPivotTableElementClickEventHandler TableHeaderClick;

        public static void InvokeTableHeaderClick(IStiPivotTableElement table, string meterKey)
        {
            ResetClick(table);

            TableHeaderClick?.Invoke(table, new StiPivotTableElementClickEventArgs
            {
                MeterKey = meterKey
            });
        }
        #endregion

        #region Methods
        public static void SetPointClick(IStiElement table, PointD point)
        {
            hash[table.GetKey()] = point;
        }
        
        public static PointD? GetPointClick(IStiPivotTableElement table)
        {
            if (table != null && hash.ContainsKey(table.GetKey()))
            {
                var pointClick = (PointD)hash[table.GetKey()];                
                return new PointD(
                    pointClick.X * table.Zoom * StiScale.Factor, 
                    pointClick.Y * table.Zoom * StiScale.Factor);
            }
            return null;
        }

        public static void ResetClick(IStiPivotTableElement table)
        {
            if (table != null && hash.ContainsKey(table.GetKey()))
                hash.Remove(table.GetKey());
        }
        #endregion
    }
}
