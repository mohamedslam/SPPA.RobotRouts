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
using Stimulsoft.Report.App;
using Stimulsoft.Report.Components;
using System;

namespace Stimulsoft.Report.Dashboard.Helpers
{
    public class StiBorderElementHelper
    {
        #region Methods
        public static RectangleD GetBorderContentRect(RectangleD rect, IStiElement element, bool skipMinimalSize = true, float scale = 1)
        {
            var borderComp = element as IStiSimpleBorder;
            if (borderComp?.Border == null)
                return rect;

            return GetBorderContentRect(rect, borderComp.Border, StiElementScale.Factor(element) * StiScale.Factor * scale, skipMinimalSize);
        }

        public static RectangleD GetBorderContentRect(RectangleD rect, IStiComponentUI comp, bool skipMinimalSize = true, float scale = 1)
        {
            var borderComp = comp as IStiSimpleBorder;
            if (borderComp?.Border == null)
                return rect;

            return GetBorderContentRect(rect, borderComp.Border, StiElementScale.Factor(comp) * StiScale.Factor * scale, skipMinimalSize);
        }

        public static RectangleD GetBorderContentRect(RectangleD rect, StiSimpleBorder border, double scale, bool skipMinimalSize = true)
        {
            var size = border.GetSizeIncludingSide();
            if (size >= 1)
                size -= skipMinimalSize ? 1 : 0;

            rect.Inflate(Math.Ceiling(-size * scale), Math.Ceiling(-size * scale));

            return rect;
        }

        public static RectangleD GetBorderContentRect(RectangleD rect, StiBorder border, double scale, bool skipMinimalSize = true)
        {
            var size = border.GetSizeIncludingSide();
            if (size >= 1)
                size -= skipMinimalSize ? 1 : 0;

            rect.Inflate(Math.Ceiling(-size * scale), Math.Ceiling(-size * scale));

            return rect;
        }
        #endregion
    }
}
