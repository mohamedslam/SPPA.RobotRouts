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

using System;
using System.Collections.Generic;

namespace Stimulsoft.Base.Wizards
{
    public static class StiWizardSettings
    {
        #region Properties
        public static List<IStiWizardService> CurrentWizards { get; set; }

        public static IStiWizardData CurrentData { get; private set; }
        #endregion

        #region Methods
        public static void Init()
        {
            try
            {
                var type = Type.GetType("Stimulsoft.Report.Design.Wizards.StiWinFormsWizardData, Stimulsoft.Report.Design");
                if (type != null)
                {
                    CurrentData = StiActivator.CreateObject(type) as IStiWizardData;
                    if (CurrentData != null) return;
                }
            }
            catch
            {
            }

            try
            {
                var type = Type.GetType("Stimulsoft.Client.Designer.Wizards.StiWizardData, Stimulsoft.Client.Designer");
                if (type != null)
                {
                    CurrentData = StiActivator.CreateObject(type) as IStiWizardData;
                    if (CurrentData != null) return;
                }
            }
            catch
            {
            }

            try
            {
                var type = Type.GetType("Stimulsoft.Report.WpfDesign.Wizards.StiWpfWizardData, Stimulsoft.Report.WpfDesign");
                if (type != null)
                {
                    CurrentData = StiActivator.CreateObject(type) as IStiWizardData;
                    if (CurrentData != null) return;
                }
            }
            catch
            {
            }
        }
        #endregion
    }
}