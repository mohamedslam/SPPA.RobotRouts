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
using Stimulsoft.Report.Components;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiDefaultConditions
    {
        public static Hashtable GetItems()
        {
            #region HighlightCondition
            Hashtable conditions = new Hashtable();
            var newHighlightCondition = new StiCondition
                (
                    StiFilterItem.Value,
                    string.Empty,
                    StiFilterCondition.EqualTo,
                    string.Empty,
                    string.Empty,
                    StiFilterDataType.String,
                    string.Empty,
                    Color.Red,
                    Color.Transparent,
                    new Font("Arial", 8),
                    true,
                    false,
                    string.Empty
                );
            newHighlightCondition.BorderSides = StiConditionBorderSides.NotAssigned;
            newHighlightCondition.Permissions = StiConditionPermissions.All;
            newHighlightCondition.Style = string.Empty;
            newHighlightCondition.Tag = null;

            conditions["StiHighlightCondition"] = StiReportEdit.GetHighlightConditionObject(newHighlightCondition);
            #endregion

            #region DataBarCondition
            var newDataBarCondition = new StiDataBarCondition
                (
                    string.Empty,
                    Stimulsoft.Report.Components.StiBrushType.Gradient,
                    Color.Green,
                    Color.Red,
                    false,
                    Color.DarkGreen,
                    Color.DarkRed,
                    StiDataBarDirection.Default,
                    StiMinimumType.Auto,
                    0f,
                    StiMaximumType.Auto,
                    100f
                );

            newDataBarCondition.Tag = null;

            conditions["StiDataBarCondition"] = StiReportEdit.GetDataBarConditionObject(newDataBarCondition);
            #endregion

            #region ColorScaleCondition
            var newColorScaleCondition = new StiColorScaleCondition
            (
                string.Empty,
                StiColorScaleType.Color2,
                Color.Red,
                Color.Yellow,
                Color.Green,
                StiMinimumType.Auto,
                0f,
                StiMidType.Auto,
                50f,
                StiMaximumType.Auto,
                100f
            );

            newColorScaleCondition.Tag = null;

            conditions["StiColorScaleCondition"] = StiReportEdit.GetColorScaleConditionObject(newColorScaleCondition);
            #endregion

            #region IconSetCondition
            var newIconSetCondition = new StiIconSetCondition
            (
                string.Empty,
                StiIconSet.TrafficLightsUnrimmed3,
                ContentAlignment.MiddleLeft,
                new StiIconSetItem(StiIcon.CircleGreen, StiIconSetOperation.MoreThanOrEqual, StiIconSetValueType.Percent, 67),
                new StiIconSetItem(StiIcon.CircleYellow, StiIconSetOperation.MoreThanOrEqual, StiIconSetValueType.Percent, 33),
                new StiIconSetItem(StiIcon.CircleRed, StiIconSetOperation.MoreThanOrEqual, StiIconSetValueType.Percent, 0f),
                null,
                null
            );

            newIconSetCondition.Tag = null;

            conditions["StiIconSetCondition"] = StiReportEdit.GetIconSetConditionObject(newIconSetCondition);
            #endregion

            return conditions;
        }
    }
}