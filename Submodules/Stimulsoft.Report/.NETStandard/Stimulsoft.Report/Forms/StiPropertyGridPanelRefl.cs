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

namespace Stimulsoft.Report.Forms
{
    internal class StiPropertyGridPanelRefl
    {
        #region Properties
        internal object PropertyGridPanel { get; private set; }
        #endregion

        #region Methods
        public void SetAllPropertiesSetter(object value)
        {
            var properyInfo = this.PropertyGridPanel.GetType().GetProperty("AllPropertiesSetter");
            if (properyInfo != null)
            {
                properyInfo.SetValue(this.PropertyGridPanel, value);
            }
        } 
        #endregion

        public StiPropertyGridPanelRefl(object propertyGridPanel)
        {
            this.PropertyGridPanel = propertyGridPanel;

            var properyInfo = this.PropertyGridPanel.GetType().GetProperty("IsReportDesigner");
            if (properyInfo != null)
            {
                properyInfo.SetValue(this.PropertyGridPanel, true);
            }            
        }
    }
}
