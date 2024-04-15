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

namespace Stimulsoft.Base.Wizards
{
    public static class StiCharterMapRunner
    {
        #region Fields
        private static Type windowType;
        private static bool isInit = false;
        #endregion

        #region Methods
        private static void Init()
        {
            if (isInit) return;
            isInit = true;

            try
            {
                windowType = Type.GetType("Stimulsoft.Wizard.Wpf.Chartermap.StiChartermapWindow, Stimulsoft.Wizard.Wpf");
            }
            catch { }
        }

        public static bool AllowChartermapWindow()
        {
            Init();
            return windowType != null;
        }

        public static IStiCharterMapWindow StiWindow(string fontFamily)
        {
            Init();

            try
            {
                if (windowType != null)
                    return StiActivator.CreateObject(windowType, new object[] { fontFamily }) as IStiCharterMapWindow;
            }
            catch
            {
            }

            return null;
        }
        #endregion
    }
}
