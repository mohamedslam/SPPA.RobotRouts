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

using Stimulsoft.Base.Server;
using System;

namespace Stimulsoft.Base.Wizards
{
    public class StiWizardsCreater
    {
        #region Methods
        internal static IStiWizardStartScreenControl GetWizardStartScreenControl(bool isWinFormsMode, object designer)
        {
            try
            {
                var type = Type.GetType("Stimulsoft.Wizard.Wpf.Wizards.StiWizardStartScreenControl, Stimulsoft.Wizard.Wpf");
                if (type != null)
                    return StiActivator.CreateObject(type, new object[] { isWinFormsMode, designer }) as IStiWizardStartScreenControl;
            }
            catch
            {
                return null;
            }
            return null;
        }

        internal static IStiNewReportWizardControl GetNewReportWizardControl(bool isWinFormsMode, object designer, IStiReport baseReport)
        {
            try
            {
                var type = Type.GetType("Stimulsoft.Wizard.Wpf.Wizards.StiNewReportWizardControl, Stimulsoft.Wizard.Wpf");
                if (type != null)
                    return StiActivator.CreateObject(type, new object[] { isWinFormsMode, designer, baseReport }) as IStiNewReportWizardControl;
            }
            catch
            {
                return null;
            }
            return null;
        }
        #endregion
    }
}
