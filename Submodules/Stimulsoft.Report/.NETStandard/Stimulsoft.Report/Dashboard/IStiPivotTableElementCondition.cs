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
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Helpers;
using System;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Dashboard
{
    public interface IStiPivotTableElementCondition: ICloneable, IStiJsonReportObject
    {
        string KeyValueMeter { get; set; }

        StiFilterDataType DataType { get; set; }

        StiFilterCondition Condition { get; set; }

        string Value { get; set; }

        int GetUniqueCode();

        Color TextColor { get; set; }

        Color BackColor { get; set; } 

        Font Font { get; set; }

        StiConditionPermissions Permissions { get; set; } 

        byte[] CustomIcon { get; set; }

        StiFontIcons Icon { get; set; }

        StiIconAlignment IconAlignment { get; set; }

        Size? IconSize { get; }

        Color IconColor { get; set; }

        byte[] GetIcon(bool isExporting);
    }
}