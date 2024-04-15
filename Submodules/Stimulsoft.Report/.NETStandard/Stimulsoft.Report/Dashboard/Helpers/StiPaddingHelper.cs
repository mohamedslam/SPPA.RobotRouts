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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.App;

namespace Stimulsoft.Report.Dashboard.Helpers
{
    public class StiPaddingHelper
    {
        public static RectangleD ApplyPadding(IStiElement element, RectangleD rect)
        {
            return ApplyPadding(element, rect, StiElementScale.Factor(element));
        }

        public static RectangleD ApplyPadding(IStiComponentUI comp, RectangleD rect)
        {
            return ApplyPadding(comp, rect, StiElementScale.Factor(comp));
        }

        public static RectangleD ApplyPadding(object comp, RectangleD rect, double scale)
        {
            var padding = (comp as IStiPadding)?.Padding;
            if (padding == null)
                return rect;

            rect.X += padding.Left * scale;
            rect.Y += padding.Top * scale;
            rect.Width -= padding.Left * scale + padding.Right * scale;
            rect.Height -= padding.Top * scale + padding.Bottom * scale;

            return rect;
        }
    }
}