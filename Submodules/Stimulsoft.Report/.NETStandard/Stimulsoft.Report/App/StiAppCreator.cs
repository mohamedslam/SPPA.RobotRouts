#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Apps 												}
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

using Stimulsoft.Report.App;
using Stimulsoft.Report.Components;
using System.Linq;

namespace Stimulsoft.Report.App
{
    public static class StiAppCreator
	{
        public static IStiScreen CreateScreen(StiReport report)
        {
            var comp = StiOptions.Services.Components.FirstOrDefault(c => c is IStiScreen)?.Clone() as StiComponent;
            if (comp != null)
            {
                comp.Report = report;

                if (comp.Guid != null)
                    comp.NewGuid();
            }

            return comp as IStiScreen;
        }
    }
}
