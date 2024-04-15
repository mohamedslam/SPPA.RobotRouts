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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Helpers;
using System.Drawing;

namespace Stimulsoft.Report.Dashboard.Helpers
{
    public static class StiDashboardExpressionHelper
    {
        #region Methods
        public static Color GetBackColor(object component, Color defaultColor)
        {
            return GetColor(component, defaultColor, "BackColor");
        }

        public static Color GetForeColor(object component, Color defaultColor)
        {
            return GetColor(component, defaultColor, "ForeColor");
        }

        private static Color GetColor(object component, Color defaultColor, string propertyName)
        {
            var comp = component as StiComponent;

            if (StiAppExpressionHelper.IsExpressionSpecified(comp, propertyName))
                return StiAppExpressionParser.ParseColorExpression(comp, propertyName, true);

            else
                return defaultColor;
        }
        #endregion
    }
}
