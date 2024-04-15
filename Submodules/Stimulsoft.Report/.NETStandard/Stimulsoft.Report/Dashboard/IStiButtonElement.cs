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
using Stimulsoft.Report.Components;
using System;

namespace Stimulsoft.Report.Dashboard
{
    public interface IStiButtonElement :
        IStiControlElement,
        IStiCornerRadius,
        IStiSimpleShadow,
        IStiSimpleBorder,
        IStiFont
    {
        string Text { get; set; }

        string Group { get; set; }

        bool Checked { get; set; }

        StiButtonShapeType ShapeType { get; set; }

        StiButtonType Type { get; set; }

        StiIconAlignment IconAlignment { get; set; }

        StiButtonStretch Stretch { get; set; }

        StiBrush IconBrush { get; set; }

        IStiButtonElementIconSet GetIconSet();

        IStiButtonVisualStates GetVisualStates();

        void InvokeCheckedChanged(object sender, EventArgs e);
    }
}
