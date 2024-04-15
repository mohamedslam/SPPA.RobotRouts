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

namespace Stimulsoft.Report.Dashboard.Helpers
{
    public static class StiPivotTableElementMouseOverHelper
    {
        #region Properties
        private static IStiPivotTableElement TableElement { get; set; }

        private static PointD? MouseOverPoint { get; set; }
        #endregion

        #region Methods
        public static void SetMouseOverPoint(IStiPivotTableElement table, PointD point)
        {
            TableElement = table;
            MouseOverPoint = point;
        }

        public static PointD? GetMouseOverPoint(IStiPivotTableElement table)
        {
            if (TableElement?.GetKey() == table?.GetKey() && MouseOverPoint != null)
            {
                return new PointD(
                    MouseOverPoint.Value.X * table.Zoom * StiScale.Factor, 
                    MouseOverPoint.Value.Y * table.Zoom * StiScale.Factor);
            }

            return null;
        }

        public static void ResetMouseOverPoint(IStiPivotTableElement table)
        {
            if (TableElement?.GetKey() == table?.GetKey())
            {
                TableElement = null;
                MouseOverPoint = null;
            }
        }
        #endregion
    }
}
