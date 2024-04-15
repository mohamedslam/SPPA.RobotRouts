#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using Stimulsoft.Report.Components;
using Stimulsoft.Report.Helper;

namespace Stimulsoft.Report.Check
{
    public class StiVerySmallSizesOfComponentsAction : StiAction
    {
        public override string Name => StiLocalizationExt.Get("CheckActions", "StiVerySmallSizesOfComponentsShort");

        public override string Description => StiLocalizationExt.Get("CheckActions", "StiVerySmallSizesOfComponentsLong");

        public override void Invoke(StiReport report, object element, string elementName)
        {
            base.Invoke(report, null, null);

            var comp = element as StiComponent;
            if (comp == null || report?.Designer?.Info == null)return;

            if (comp.Width < report.Designer.Info.GridSize)
                comp.Width = report.Designer.Info.GridSize;

            if (comp.Height < report.Designer.Info.GridSize)
                comp.Height = report.Designer.Info.GridSize;

            if (report.Designer.Info.AlignToGrid && comp.Page != null)
                comp.Page.AlignToGrid(comp);
        }
    }
}