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

using Stimulsoft.Report.Components;
using Stimulsoft.Report.Helpers;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Dashboard
{
    public interface IStiIndicatorElementCondition
    {
        StiIndicatorFieldCondition Field { get; set; }

        StiFilterCondition Condition { get; set; }

        string Value { get; set; }

        StiIconAlignment IconAlignment { get; set; }

        StiIconAlignment TargetIconAlignment { get; set; }

        Color IconColor { get; set; }

        StiFontIcons Icon { get; set; }

        byte[] CustomIcon { get; set; }

        StiIndicatorConditionPermissions Permissions { get; set; }

        Font Font { get; set; }

        Color TextColor { get; set; }

        Color BackColor { get; set; }

        Color TargetIconColor { get; set; }

        StiFontIcons TargetIcon { get; set; }
    }
}