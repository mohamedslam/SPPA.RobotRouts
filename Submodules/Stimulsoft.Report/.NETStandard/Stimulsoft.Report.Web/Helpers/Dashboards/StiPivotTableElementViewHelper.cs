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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Engine;
using Stimulsoft.Data.Helpers;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Helpers;
using Stimulsoft.Report.Dashboard.Styles;
using Stimulsoft.Report.Dictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiPivotTableElementViewHelper
    {
        #region Methods
        public static Hashtable GetPivotTableData(IStiPivotTableElement pivotElement)
        {
            return (Hashtable)StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard", "Helpers.StiPivotTableHelper", "GetViewerData", new object[] { pivotElement});
        }

        public static Hashtable GetPivotTableSettings(IStiPivotTableElement tableElement)
        {
            var settings = new Hashtable();
            var style = StiDashboardStyleHelper.GetPivotTableStyle(tableElement);

            settings["alternatingCellBackColor"] = StiReportHelper.GetHtmlColor(style.AlternatingCellBackColor);
            settings["alternatingCellForeColor"] = StiReportHelper.GetHtmlColor(style.AlternatingCellForeColor);
            settings["cellBackColor"] = StiReportHelper.GetHtmlColor(style.CellBackColor);
            settings["cellForeColor"] = StiReportHelper.GetHtmlColor(style.CellForeColor);
            settings["columnHeaderBackColor"] = StiReportHelper.GetHtmlColor(style.ColumnHeaderBackColor);
            settings["columnHeaderForeColor"] = StiReportHelper.GetHtmlColor(style.ColumnHeaderForeColor);
            settings["hotColumnHeaderBackColor"] = StiReportHelper.GetHtmlColor(style.HotColumnHeaderBackColor);
            settings["hotRowHeaderBackColor"] = StiReportHelper.GetHtmlColor(style.HotRowHeaderBackColor);
            settings["lineColor"] = StiReportHelper.GetHtmlColor(style.LineColor);
            settings["rowHeaderBackColor"] = StiReportHelper.GetHtmlColor(style.RowHeaderBackColor);
            settings["rowHeaderForeColor"] = StiReportHelper.GetHtmlColor(style.RowHeaderForeColor);
            settings["selectedCellBackColor"] = StiReportHelper.GetHtmlColor(style.SelectedCellBackColor);
            settings["selectedCellForeColor"] = StiReportHelper.GetHtmlColor(style.SelectedCellForeColor);

            settings["fontName"] = StiElementConsts.Table.Font.Name;//
            settings["fontSize"] = StiElementConsts.Table.Font.Size;//
            settings["fontIsBold"] = StiElementConsts.Table.Font.IsBold;
            settings["cellHeight"] = StiElementConsts.Table.Height;

            return settings;
        }

        private static string GetCellAlignment(object component)
        {
            var compAlignment = component as IStiHorAlignment;

            if (compAlignment != null)
            {
                switch (compAlignment.HorAlignment)
                {
                    case StiHorAlignment.Left:
                        return "left";

                    case StiHorAlignment.Center:
                        return "center";

                    case StiHorAlignment.Right:
                        return "right";
                }
            }

            return "center";
        }      
        #endregion
    }
}
