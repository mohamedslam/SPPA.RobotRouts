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
    public class StiMarginHelper
    {
        public static RectangleD ApplyMargin(IStiElement element, RectangleD rect, bool isScale = false)
        {
            var scale = isScale ? StiElementScale.Factor(element) * StiScale.Factor : 1;
            return ApplyMargin(element, rect, scale);
        }

        public static RectangleD ApplyMargin(IStiElement element, RectangleD rect, double scale)
        {
            var margin = (element as IStiMargin)?.Margin;
            if (margin == null)
                return rect;
            
            rect.X += margin.Left * scale;
            rect.Y += margin.Top * scale;
            rect.Width -= margin.Left * scale + margin.Right * scale;
            rect.Height -= margin.Top * scale + margin.Bottom * scale;

            return rect;
        }
    }
}