#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Engine;
using System;
using System.Collections;
using System.Linq;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiButtonElementHelper
    {
        #region Methods
        public static void ApplyButtonEvent(StiReport report, Hashtable parameters)
        {
            if (parameters == null || parameters.Count == 0) return;

            var buttonElement = report.Pages.GetComponentByName(parameters["elementName"] as string) as IStiButtonElement;
            if (buttonElement != null) 
            {
                StiCacheCleaner.Clean(report);
                
                (buttonElement as StiComponent)?.InvokeClick(buttonElement, new EventArgs());

                if (buttonElement.Type == StiButtonType.CheckBox || buttonElement.Type == StiButtonType.RadioButton)
                {
                    buttonElement?.InvokeCheckedChanged(buttonElement, new EventArgs());
                    buttonElement.Checked = buttonElement.Type == StiButtonType.RadioButton ? true : Convert.ToBoolean(parameters["isChecked"]);

                    if (buttonElement.Type == StiButtonType.RadioButton)
                    {
                        buttonElement.Page.GetComponents().ToList()
                            .Where(c => c is IStiButtonElement)
                            .Cast<IStiButtonElement>()
                            .Where(b => b.Group == buttonElement.Group && b != buttonElement)
                            .ToList().ForEach(b =>
                            {
                                b.Checked = false;
                            });
                    }
                }

                StiPivotToContainerCache.Clean(report.Key);
                StiPivotTableToCrossTabCache.Clean(report.Key);
                StiPivotToConvertedStateCache.Clean(report.Key);
            }
        }
        #endregion
    }
}