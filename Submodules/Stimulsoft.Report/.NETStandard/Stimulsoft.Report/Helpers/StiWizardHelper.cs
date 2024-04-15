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

using Stimulsoft.Base;
using System.Reflection;

namespace Stimulsoft.Report.Helpers
{
    public static class StiWizardHelper
    {
        public static void InitServices()
        {
            #region Init wizards
            try
            {
                var type = StiTypeFinder.GetType("Stimulsoft.Wizard.Wpf.Services.StiWizardService, Stimulsoft.Wizard.Wpf");
                if (type == null) return;

                var mi = type.GetMethod("Init", BindingFlags.Static | BindingFlags.Public);
                if (mi != null)
                    mi.Invoke(null, null);
            }
            catch
            {
            }
            #endregion
        }
    }
}